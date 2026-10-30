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

        public static bool UpdateProfile(
            int userId,
            string fullName,
            string email,
            string phoneNumber,
            string connectionString,
            out string errorMessage)
        {
            errorMessage = string.Empty;
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

            string trimPhone = string.IsNullOrWhiteSpace(phoneNumber) ? "" : phoneNumber.Trim();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Kiểm tra trùng Email với người dùng khác
                    string checkEmailSql = "SELECT COUNT(1) FROM dbo.USERS WHERE email = @e AND user_id <> @id";
                    using (SqlCommand cmdCheckEmail = new SqlCommand(checkEmailSql, conn))
                    {
                        cmdCheckEmail.Parameters.AddWithValue("@e", trimEmail);
                        cmdCheckEmail.Parameters.AddWithValue("@id", userId);
                        int count = Convert.ToInt32(cmdCheckEmail.ExecuteScalar());
                        if (count > 0)
                        {
                            errorMessage = $"Địa chỉ email '{trimEmail}' đã được người dùng khác đăng ký!";
                            return false;
                        }
                    }

                    // Cập nhật CSDL
                    string updateSql = @"UPDATE dbo.USERS 
                                        SET full_name = @fn, email = @e, phone_number = @ph 
                                        WHERE user_id = @id";
                    using (SqlCommand cmdUpdate = new SqlCommand(updateSql, conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@fn", trimFullName);
                        cmdUpdate.Parameters.AddWithValue("@e", trimEmail);
                        cmdUpdate.Parameters.AddWithValue("@ph", string.IsNullOrEmpty(trimPhone) ? (object)DBNull.Value : trimPhone);
                        cmdUpdate.Parameters.AddWithValue("@id", userId);

                        int rows = cmdUpdate.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            if (CurrentUser != null && CurrentUser.UserId == userId)
                            {
                                CurrentUser.FullName = trimFullName;
                                CurrentUser.Email = trimEmail;
                                CurrentUser.PhoneNumber = trimPhone;
                            }
                            return true;
                        }
                        else
                        {
                            errorMessage = "Không tìm thấy người dùng để cập nhật!";
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
                errorMessage = $"Lỗi khi cập nhật hồ sơ người dùng: {ex.Message}";
                return false;
            }
        }

        public static bool ChangePassword(
            int userId,
            string currentPassword,
            string newPassword,
            string confirmPassword,
            string connectionString,
            out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                errorMessage = "Vui lòng nhập mật khẩu hiện tại!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(newPassword))
            {
                errorMessage = "Vui lòng nhập mật khẩu mới!";
                return false;
            }
            if (newPassword.Length < 6)
            {
                errorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự!";
                return false;
            }
            if (newPassword != confirmPassword)
            {
                errorMessage = "Mật khẩu xác nhận không khớp với mật khẩu mới!";
                return false;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Lấy mật khẩu hash hiện tại trong DB
                    string checkSql = "SELECT password_hash FROM dbo.USERS WHERE user_id = @id";
                    using (SqlCommand cmdCheck = new SqlCommand(checkSql, conn))
                    {
                        cmdCheck.Parameters.AddWithValue("@id", userId);
                        object result = cmdCheck.ExecuteScalar();
                        if (result == null || result == DBNull.Value)
                        {
                            errorMessage = "Không tìm thấy thông tin tài khoản!";
                            return false;
                        }

                        string storedHash = result.ToString() ?? "";
                        string inputHash = ComputeSha256(currentPassword.Trim());
                        bool passMatches = string.Equals(storedHash, inputHash, StringComparison.OrdinalIgnoreCase)
                                        || (storedHash == "123456" && currentPassword.Trim() == "123456")
                                        || (string.Equals(storedHash, ComputeSha256(""), StringComparison.OrdinalIgnoreCase) && currentPassword.Trim() == "123456");

                        if (!passMatches)
                        {
                            errorMessage = "Mật khẩu hiện tại không đúng!";
                            return false;
                        }
                    }

                    // Cập nhật mật khẩu mới
                    string newHash = ComputeSha256(newPassword.Trim());
                    string updateSql = "UPDATE dbo.USERS SET password_hash = @p WHERE user_id = @id";
                    using (SqlCommand cmdUpdate = new SqlCommand(updateSql, conn))
                    {
                        cmdUpdate.Parameters.AddWithValue("@p", newHash);
                        cmdUpdate.Parameters.AddWithValue("@id", userId);
                        int rows = cmdUpdate.ExecuteNonQuery();
                        if (rows > 0)
                        {
                            return true;
                        }
                        else
                        {
                            errorMessage = "Không thể cập nhật mật khẩu mới!";
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
                errorMessage = $"Lỗi khi đổi mật khẩu: {ex.Message}";
                return false;
            }
        }
    }
}

