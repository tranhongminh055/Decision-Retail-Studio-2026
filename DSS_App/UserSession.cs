using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.SqlClient;

namespace DSS_App
{
    public class UserSession
    {
        public static UserSession? CurrentUser { get; set; }

        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Admin", "DecisionMaker", "SalesManager", "SalesStaff"
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime LoginTime { get; set; } = DateTime.Now;

        public bool IsAdmin => string.Equals(Role, "Admin", StringComparison.OrdinalIgnoreCase);
        public bool IsDecisionMaker => string.Equals(Role, "DecisionMaker", StringComparison.OrdinalIgnoreCase) || IsAdmin;
        public bool IsSalesManager => string.Equals(Role, "SalesManager", StringComparison.OrdinalIgnoreCase) || IsDecisionMaker;
        public bool IsSalesStaff => string.Equals(Role, "SalesStaff", StringComparison.OrdinalIgnoreCase);

        public string RoleTitle => Role switch
        {
            "Admin" => "Quản trị viên Hệ thống",
            "DecisionMaker" => "Giám đốc kinh doanh (Decision Maker)",
            "SalesManager" => "Trưởng phòng phân tích",
            "SalesStaff" => "Chuyên viên bán hàng",
            _ => Role
        };

        public Color RoleBadgeColor => Role switch
        {
            "Admin" => Color.FromArgb(239, 68, 68),         // Đỏ tươi
            "DecisionMaker" => Color.FromArgb(139, 92, 246),  // Tím Indigo
            "SalesManager" => Color.FromArgb(59, 130, 246),   // Xanh dương
            "SalesStaff" => Color.FromArgb(16, 185, 129),    // Xanh ngọc
            _ => Color.FromArgb(100, 116, 139)
        };

        public static void Logout()
        {
            CurrentUser = null;
        }

        public static string ComputeSha256(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static bool Authenticate(string username, string password, string connectionString, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Vui lòng nhập Tên đăng nhập, Email hoặc Số điện thoại!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Vui lòng nhập Mật khẩu!";
                return false;
            }

            string trimUser = username.Trim();
            string trimPass = password.Trim();

            // Xác thực 100% với CSDL thật SQL Server (DSS_SalesDB.dbo.USERS)
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"SELECT user_id, username, password_hash, full_name, email, phone_number, role, status 
                                   FROM dbo.USERS 
                                   WHERE (username = @u OR email = @u OR phone_number = @u)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@u", trimUser);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                bool status = reader.GetBoolean(7);
                                if (!status)
                                {
                                    errorMessage = "Tài khoản này hiện đang bị khóa hoặc ngưng hoạt động!";
                                    return false;
                                }

                                string storedHash = reader.GetString(2);
                                string inputHash = ComputeSha256(trimPass);

                                // Kiểm tra hash mật khẩu thật
                                bool passMatches = string.Equals(storedHash, inputHash, StringComparison.OrdinalIgnoreCase)
                                                // Hỗ trợ các tài khoản gốc khởi tạo ban đầu có hash rỗng
                                                || (string.Equals(storedHash, ComputeSha256(""), StringComparison.OrdinalIgnoreCase) && (trimPass == "123456" || trimPass == "admin"))
                                                || (storedHash == "123456" && trimPass == "123456");

                                if (passMatches)
                                {
                                    CurrentUser = new UserSession
                                    {
                                        UserId = reader.GetInt32(0),
                                        Username = reader.GetString(1),
                                        FullName = reader.GetString(3),
                                        Email = reader.GetString(4),
                                        PhoneNumber = reader.IsDBNull(5) ? "" : reader.GetString(5),
                                        Role = reader.GetString(6),
                                        LoginTime = DateTime.Now
                                    };
                                    return true;
                                }
                                else
                                {
                                    errorMessage = "Mật khẩu không chính xác! Vui lòng thử lại.";
                                    return false;
                                }
                            }
                            else
                            {
                                errorMessage = "Tài khoản không tồn tại! Vui lòng kiểm tra lại hoặc bấm 'Đăng ký tài khoản' nếu chưa có tài khoản.";
                                return false;
                            }
                        }
                    }
                }
            }
            catch (SqlException ex) when (DatabaseGuard.IsConnectionError(ex))
            {
                errorMessage = DatabaseGuard.FriendlyConnectionError(ex);
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"Lỗi hệ thống khi đăng nhập: {ex.Message}";
                return false;
            }
        }

        public static bool RegisterUser(
            string username, 
            string password, 
            string confirmPassword,
            string fullName, 
            string email, 
            string phoneNumber, 
            string role, 
            string connectionString, 
            out string errorMessage)
        {
            errorMessage = string.Empty;

            // 1. Kiểm tra tính hợp lệ dữ liệu nhập
            if (string.IsNullOrWhiteSpace(username))
            {
                errorMessage = "Vui lòng nhập Tên đăng nhập!";
                return false;
            }
            string trimUser = username.Trim();
            if (trimUser.Length < 3 || trimUser.Length > 50)
            {
                errorMessage = "Tên đăng nhập phải từ 3 đến 50 ký tự!";
                return false;
            }
            if (trimUser.Contains(" "))
            {
                errorMessage = "Tên đăng nhập không được chứa khoảng trắng!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(fullName))
            {
                errorMessage = "Vui lòng nhập Họ và tên đầy đủ!";
                return false;
            }
            string trimFullName = fullName.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                errorMessage = "Vui lòng nhập địa chỉ Email!";
                return false;
            }
            string trimEmail = email.Trim();
            if (!trimEmail.Contains("@") || !trimEmail.Contains("."))
            {
                errorMessage = "Địa chỉ Email không đúng định dạng!";
                return false;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                errorMessage = "Vui lòng nhập Mật khẩu!";
                return false;
            }
            if (password.Length < 6)
            {
                errorMessage = "Mật khẩu bảo mật phải có ít nhất 6 ký tự!";
                return false;
            }
            if (password != confirmPassword)
            {
                errorMessage = "Mật khẩu xác nhận không khớp! Vui lòng nhập lại.";
                return false;
            }

            // Kiểm tra Role hợp lệ theo DB constraint: CHECK (role IN ('Admin', 'DecisionMaker', 'SalesManager', 'SalesStaff'))
            string validRole = role switch
            {
                "Admin" or "Quản trị viên" or "Quản trị viên Hệ thống" => "Admin",
                "DecisionMaker" or "Giám đốc kinh doanh" or "Giám đốc kinh doanh (Decision Maker)" => "DecisionMaker",
                "SalesManager" or "Trưởng phòng phân tích" => "SalesManager",
                _ => "SalesStaff"
            };

            string trimPhone = string.IsNullOrWhiteSpace(phoneNumber) ? "" : phoneNumber.Trim();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1. Kiểm tra trùng Username
                    string checkUserSql = "SELECT COUNT(1) FROM dbo.USERS WHERE username = @u";
                    using (SqlCommand cmdCheckUser = new SqlCommand(checkUserSql, conn))
                    {
                        cmdCheckUser.Parameters.AddWithValue("@u", trimUser);
                        int count = Convert.ToInt32(cmdCheckUser.ExecuteScalar());
                        if (count > 0)
                        {
                            errorMessage = $"Tên đăng nhập '{trimUser}' đã có người sử dụng! Vui lòng chọn tên khác.";
                            return false;
                        }
                    }

                    // 2. Kiểm tra trùng Email
                    string checkEmailSql = "SELECT COUNT(1) FROM dbo.USERS WHERE email = @e";
                    using (SqlCommand cmdCheckEmail = new SqlCommand(checkEmailSql, conn))
                    {
                        cmdCheckEmail.Parameters.AddWithValue("@e", trimEmail);
                        int count = Convert.ToInt32(cmdCheckEmail.ExecuteScalar());
                        if (count > 0)
                        {
                            errorMessage = $"Địa chỉ email '{trimEmail}' đã được đăng ký! Vui lòng dùng email khác.";
                            return false;
                        }
                    }

                    // 3. Thực hiện lưu vào CSDL thật dbo.USERS
                    string insertSql = @"INSERT INTO dbo.USERS 
                        (username, password_hash, full_name, email, phone_number, role, status, created_at)
                        VALUES (@u, @p, @fn, @e, @ph, @r, 1, SYSDATETIME());";

                    using (SqlCommand cmdInsert = new SqlCommand(insertSql, conn))
                    {
                        cmdInsert.Parameters.AddWithValue("@u", trimUser);
                        cmdInsert.Parameters.AddWithValue("@p", ComputeSha256(password));
                        cmdInsert.Parameters.AddWithValue("@fn", trimFullName);
                        cmdInsert.Parameters.AddWithValue("@e", trimEmail);
                        cmdInsert.Parameters.AddWithValue("@ph", string.IsNullOrEmpty(trimPhone) ? (object)DBNull.Value : trimPhone);
                        cmdInsert.Parameters.AddWithValue("@r", validRole);

                        int rows = cmdInsert.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            return true;
                        }
                        else
                        {
                            errorMessage = "Không thể ghi dữ liệu người dùng vào cơ sở dữ liệu!";
                            return false;
                        }
                    }
                }
            }
            catch (SqlException ex) when (DatabaseGuard.IsConnectionError(ex))
            {
                errorMessage = DatabaseGuard.FriendlyConnectionError(ex);
                return false;
            }
            catch (Exception ex)
            {
                errorMessage = $"Lỗi khi lưu tài khoản vào cơ sở dữ liệu: {ex.Message}";
                return false;
            }
        }
    }
}
