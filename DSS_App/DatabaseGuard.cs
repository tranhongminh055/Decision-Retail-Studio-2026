using System;
using System.IO;
using Microsoft.Data.SqlClient;

namespace DSS_App
{
    /// <summary>
    /// Kiểm tra SQL Server trước khi hệ thống hoạt động. Hệ thống DSS phụ thuộc hoàn toàn vào
    /// SQL Server (DSS_SalesDB): nếu SQL Server không chạy thì không cho vào ứng dụng và báo lỗi.
    /// </summary>
    public static class DatabaseGuard
    {
        public const string ServerName = @".\SQLEXPRESS25";

        public static readonly string MasterConnectionString =
            @"Server=.\SQLEXPRESS25;Database=master;Trusted_Connection=True;TrustServerCertificate=True;Timeout=3;";

        public static readonly string DatabaseConnectionString =
            @"Server=.\SQLEXPRESS25;Database=DSS_SalesDB;Trusted_Connection=True;TrustServerCertificate=True;Timeout=3;";

        public static bool IsConnectionError(SqlException ex)
        {
            // -1/2/53: không tìm thấy server; 233/10060/10061: từ chối/timeout; 4060: không mở được DB; 18456: lỗi đăng nhập SQL
            foreach (SqlError err in ex.Errors)
            {
                switch (err.Number)
                {
                    case -1: case 2: case 53: case 233: case 258:
                    case 10060: case 10061: case 4060: case 18456:
                        return true;
                }
            }
            return ex.Number == -1 || ex.Number == 2 || ex.Number == 53 || ex.Number == 10061;
        }

        public static string FriendlyConnectionError(Exception ex)
        {
            return $"Không thể kết nối tới SQL Server ({ServerName}).\n\n" +
                   "Hệ thống DSS hoạt động dựa trên SQL Server nên không thể sử dụng khi SQL Server chưa chạy.\n" +
                   $"Vui lòng khởi động dịch vụ \"SQL Server ({ServerName.TrimStart('.', '\\')})\" rồi thử lại.\n\n" +
                   $"Chi tiết lỗi: {ex.Message}";
        }

        /// <summary>
        /// Trả về true nếu SQL Server hoạt động và CSDL DSS_SalesDB (bảng USERS) sẵn sàng.
        /// </summary>
        public static bool Check(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                bool needProvision;
                using (SqlConnection master = new SqlConnection(MasterConnectionString))
                {
                    master.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM sys.databases WHERE name = 'DSS_SalesDB'", master))
                    {
                        needProvision = Convert.ToInt32(cmd.ExecuteScalar()) == 0;
                    }
                    if (needProvision)
                    {
                        using (SqlCommand create = new SqlCommand("CREATE DATABASE DSS_SalesDB;", master))
                        {
                            create.ExecuteNonQuery();
                        }
                    }
                }

                using (SqlConnection db = new SqlConnection(DatabaseConnectionString))
                {
                    db.Open();
                    if (!TableExists(db, "DECISION_RESULTS"))
                    {
                        RunScript(db, "01_create_database.sql");
                        RunScript(db, "02_seed_data.sql");
                    }

                    if (!TableExists(db, "USERS"))
                    {
                        errorMessage = "Đã kết nối SQL Server nhưng CSDL DSS_SalesDB chưa có bảng USERS.\n" +
                                       "Vui lòng chạy các script trong thư mục sql\\ (01_create_database.sql, 02_seed_data.sql) rồi thử lại.";
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = FriendlyConnectionError(ex);
                return false;
            }
        }

        /// <summary>
        /// Kiểm tra nhanh SQL Server còn hoạt động và có đọc được dữ liệu DSS_SalesDB hay không.
        /// Dùng làm "nhịp tim" khi ứng dụng đang chạy; lỗi => khóa toàn bộ giao diện.
        /// </summary>
        public static bool Ping(out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                using (SqlConnection db = new SqlConnection(DatabaseConnectionString))
                {
                    db.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM dbo.USERS", db))
                    {
                        cmd.CommandTimeout = 3;
                        cmd.ExecuteScalar();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                try { SqlConnection.ClearAllPools(); } catch { }
                errorMessage = FriendlyConnectionError(ex);
                return false;
            }
        }

        private static bool TableExists(SqlConnection conn, string table)
        {
            using (SqlCommand cmd = new SqlCommand("SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @t", conn))
            {
                cmd.Parameters.AddWithValue("@t", table);
                return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
            }
        }

        private static void RunScript(SqlConnection conn, string fileName)
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\sql\" + fileName);
            if (!File.Exists(path)) return;

            string script = File.ReadAllText(path);
            string[] commands = script.Split(new[] { "GO\r\n", "GO\n", "GO\r" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string cmdText in commands)
            {
                string trimmed = cmdText.Trim();
                if (string.IsNullOrWhiteSpace(trimmed)) continue;
                try
                {
                    using (SqlCommand cmd = new SqlCommand(trimmed, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch { }
            }
        }
    }
}
