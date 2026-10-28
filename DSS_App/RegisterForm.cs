#nullable disable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DSS_App
{
    public class RegisterForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        public string RegisteredUsername { get; private set; } = string.Empty;

        private readonly string connectionString;
        private readonly Image bgWatermarkImage;

        private HeaderPanel pnlHeader;
        private Panel pnlBody;

        private TextBox txtFullName;
        private TextBox txtUsername;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private ComboBox cboRole;
        private TextBox txtPassword;
        private TextBox txtConfirmPassword;

        private Panel pnlFullNameBox;
        private Panel pnlUsernameBox;
        private Panel pnlEmailBox;
        private Panel pnlPhoneBox;
        private Panel pnlRoleBox;
        private Panel pnlPassBox;
        private Panel pnlConfirmBox;

        private Label lblError;
        private Button btnRegister;
        private Button btnBackToLogin;

        private bool isPasswordMasked = true;
        private bool isConfirmMasked = true;

        public RegisterForm(string dbConnString, Image watermark = null)
        {
            this.connectionString = dbConnString;
            this.bgWatermarkImage = watermark;

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(760, 680);
            this.MinimumSize = new Size(720, 640);
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.DoubleBuffered = true;
            this.KeyPreview = true;

            BuildUI();

            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            };

            this.Paint += (s, pe) =>
            {
                using (Pen p = new Pen(Color.FromArgb(96, 165, 250), 1.5f))
                {
                    pe.Graphics.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);
                }
            };

            this.Resize += (s, e) => RepositionControls();
        }

        private void BuildUI()
        {
            // ================= 1. HEADER =================
            pnlHeader = new HeaderPanel
            {
                Location = new Point(0, 0),
                Size = new Size(this.ClientSize.Width, 140)
            };
            pnlHeader.SetHeaderText(
                "Đăng Ký Tài Khoản Người Dùng Mới",
                "Dữ liệu người dùng sẽ được lưu trực tiếp vào CSDL Microsoft SQL Server DSS_SalesDB",
                "⚡ CỔNG ĐĂNG KÝ TÀI KHOẢN HỆ THỐNG DSS (REAL DATA)"
            );
            if (bgWatermarkImage != null)
            {
                pnlHeader.SetWatermarkImage(bgWatermarkImage);
            }

            pnlHeader.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left && this.WindowState != FormWindowState.Maximized)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, 0xA1, 0x2, 0);
                }
            };

            // Nút đóng
            Label btnClose = new Label
            {
                Text = "✕",
                Font = new Font("Segoe UI", 12f, FontStyle.Regular),
                ForeColor = Color.FromArgb(203, 213, 225),
                Size = new Size(44, 34),
                Location = new Point(pnlHeader.Width - 48, 6),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClose.MouseEnter += (s, e) => { btnClose.BackColor = Color.FromArgb(239, 68, 68); btnClose.ForeColor = Color.White; };
            btnClose.MouseLeave += (s, e) => { btnClose.BackColor = Color.Transparent; btnClose.ForeColor = Color.FromArgb(203, 213, 225); };
            btnClose.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };
            pnlHeader.Controls.Add(btnClose);

            this.Controls.Add(pnlHeader);

            // ================= 2. BODY =================
            pnlBody = new Panel
            {
                Location = new Point(0, 140),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 140),
                BackColor = Color.White
            };

            // Họ và tên
            pnlFullNameBox = CreateFieldContainer("👤", out txtFullName, false);
            // Tên đăng nhập
            pnlUsernameBox = CreateFieldContainer("🆔", out txtUsername, false);
            // Email
            pnlEmailBox = CreateFieldContainer("✉️", out txtEmail, false);
            // Số điện thoại
            pnlPhoneBox = CreateFieldContainer("📞", out txtPhone, false);

            // ComboBox vai trò
            pnlRoleBox = new Panel { Height = 42, BackColor = Color.White };
            pnlRoleBox.Paint += (s, pe) =>
            {
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen borderPen = new Pen(Color.FromArgb(203, 213, 225), 1.2f))
                {
                    pe.Graphics.DrawRectangle(borderPen, 0, 0, pnlRoleBox.Width - 1, pnlRoleBox.Height - 1);
                }
            };
            Label lblRoleIcon = new Label
            {
                Text = "💼",
                Font = new Font("Segoe UI", 11f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(12, 9),
                Size = new Size(24, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };
            cboRole = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(15, 23, 42),
                FlatStyle = FlatStyle.Flat,
                Location = new Point(44, 9)
            };
            cboRole.Items.AddRange(new object[]
            {
                "Chuyên viên bán hàng (SalesStaff)",
                "Trưởng phòng phân tích (SalesManager)",
                "Giám đốc kinh doanh (DecisionMaker)",
                "Quản trị viên Hệ thống (Admin)"
            });
            cboRole.SelectedIndex = 0;
            pnlRoleBox.Controls.Add(lblRoleIcon);
            pnlRoleBox.Controls.Add(cboRole);

            // Mật khẩu
            pnlPassBox = CreateFieldContainerWithToggle("🔒", out txtPassword, out Button btnEye1, ref isPasswordMasked);
            // Xác nhận mật khẩu
            pnlConfirmBox = CreateFieldContainerWithToggle("🔐", out txtConfirmPassword, out Button btnEye2, ref isConfirmMasked);

            // Label thông báo lỗi
            lblError = new Label
            {
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                Height = 22,
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false
            };

            // Nút Đăng ký
            btnRegister = new Button
            {
                Text = "⚡ HOÀN TẤT ĐĂNG KÝ TÀI KHOẢN VÀO CSDL",
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(79, 70, 229),
                FlatStyle = FlatStyle.Flat,
                Height = 46,
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderSize = 0;
            btnRegister.MouseEnter += (s, e) => { btnRegister.BackColor = Color.FromArgb(67, 56, 202); };
            btnRegister.MouseLeave += (s, e) => { btnRegister.BackColor = Color.FromArgb(79, 70, 229); };
            btnRegister.Click += (s, e) => PerformRegistration();

            // Nút Quay lại Đăng nhập
            btnBackToLogin = new Button
            {
                Text = "← Đã có tài khoản? Quay lại màn hình Đăng nhập",
                Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                ForeColor = Color.FromArgb(79, 70, 229),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Height = 32,
                Cursor = Cursors.Hand
            };
            btnBackToLogin.FlatAppearance.BorderSize = 0;
            btnBackToLogin.Click += (s, e) =>
            {
                this.DialogResult = DialogResult.Cancel;
                this.Close();
            };

            pnlBody.Controls.Add(pnlFullNameBox);
            pnlBody.Controls.Add(pnlUsernameBox);
            pnlBody.Controls.Add(pnlEmailBox);
            pnlBody.Controls.Add(pnlPhoneBox);
            pnlBody.Controls.Add(pnlRoleBox);
            pnlBody.Controls.Add(pnlPassBox);
            pnlBody.Controls.Add(pnlConfirmBox);
            pnlBody.Controls.Add(lblError);
            pnlBody.Controls.Add(btnRegister);
            pnlBody.Controls.Add(btnBackToLogin);

            this.Controls.Add(pnlBody);

            RepositionControls();
        }

        private void RepositionControls()
        {
            if (pnlHeader != null)
            {
                pnlHeader.Width = this.ClientSize.Width;
                pnlHeader.Invalidate();
            }

            if (pnlBody != null)
            {
                pnlBody.Location = new Point(0, 140);
                pnlBody.Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 140);

                int contentWidth = Math.Min(650, pnlBody.Width - 60);
                int startX = (pnlBody.Width - contentWidth) / 2;
                int halfWidth = (contentWidth - 16) / 2;

                int y = 16;

                // Row 1: Họ tên (Left) & Username (Right)
                Label lblRow1Left = GetOrCreateLabel("lblR1L", "Họ và tên đầy đủ (*):", startX, y, halfWidth);
                Label lblRow1Right = GetOrCreateLabel("lblR1R", "Tên đăng nhập / Mã NV (*):", startX + halfWidth + 16, y, halfWidth);

                y += 24;
                pnlFullNameBox.Location = new Point(startX, y);
                pnlFullNameBox.Width = halfWidth;
                txtFullName.Width = halfWidth - 50;

                pnlUsernameBox.Location = new Point(startX + halfWidth + 16, y);
                pnlUsernameBox.Width = halfWidth;
                txtUsername.Width = halfWidth - 50;

                // Row 2: Email (Left) & SĐT (Right)
                y += 52;
                Label lblRow2Left = GetOrCreateLabel("lblR2L", "Địa chỉ Email (*):", startX, y, halfWidth);
                Label lblRow2Right = GetOrCreateLabel("lblR2R", "Số điện thoại di động:", startX + halfWidth + 16, y, halfWidth);

                y += 24;
                pnlEmailBox.Location = new Point(startX, y);
                pnlEmailBox.Width = halfWidth;
                txtEmail.Width = halfWidth - 50;

                pnlPhoneBox.Location = new Point(startX + halfWidth + 16, y);
                pnlPhoneBox.Width = halfWidth;
                txtPhone.Width = halfWidth - 50;

                // Row 3: Vai trò hệ thống
                y += 52;
                Label lblRow3 = GetOrCreateLabel("lblR3", "Vai trò & Phân quyền truy cập DSS:", startX, y, contentWidth);

                y += 24;
                pnlRoleBox.Location = new Point(startX, y);
                pnlRoleBox.Width = contentWidth;
                cboRole.Width = contentWidth - 60;

                // Row 4: Mật khẩu (Left) & Xác nhận mật khẩu (Right)
                y += 52;
                Label lblRow4Left = GetOrCreateLabel("lblR4L", "Mật khẩu bảo mật (tối thiểu 6 ký tự) (*):", startX, y, halfWidth);
                Label lblRow4Right = GetOrCreateLabel("lblR4R", "Xác nhận lại mật khẩu (*):", startX + halfWidth + 16, y, halfWidth);

                y += 24;
                pnlPassBox.Location = new Point(startX, y);
                pnlPassBox.Width = halfWidth;
                txtPassword.Width = halfWidth - 84;

                pnlConfirmBox.Location = new Point(startX + halfWidth + 16, y);
                pnlConfirmBox.Width = halfWidth;
                txtConfirmPassword.Width = halfWidth - 84;

                // Row 5: Lỗi
                y += 52;
                lblError.Location = new Point(startX, y);
                lblError.Width = contentWidth;

                // Row 6: Nút Đăng ký
                y += 26;
                btnRegister.Location = new Point(startX, y);
                btnRegister.Width = contentWidth;

                // Row 7: Nút Quay lại
                y += 52;
                btnBackToLogin.Location = new Point(startX, y);
                btnBackToLogin.Width = contentWidth;
            }
        }

        private Label GetOrCreateLabel(string name, string text, int x, int y, int w)
        {
            Control[] matches = pnlBody.Controls.Find(name, false);
            Label lbl;
            if (matches.Length > 0 && matches[0] is Label l)
            {
                lbl = l;
            }
            else
            {
                lbl = new Label
                {
                    Name = name,
                    Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    AutoSize = false,
                    Height = 20
                };
                pnlBody.Controls.Add(lbl);
            }
            lbl.Text = text;
            lbl.Location = new Point(x, y);
            lbl.Width = w;
            return lbl;
        }

        private Panel CreateFieldContainer(string icon, out TextBox txt, bool isPassword)
        {
            Panel pnl = new Panel { Height = 42, BackColor = Color.White };
            pnl.Paint += (s, pe) =>
            {
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen borderPen = new Pen(Color.FromArgb(203, 213, 225), 1.2f))
                {
                    pe.Graphics.DrawRectangle(borderPen, 0, 0, pnl.Width - 1, pnl.Height - 1);
                }
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 11f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(10, 9),
                Size = new Size(24, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };
            txt = new TextBox
            {
                Font = new Font("Segoe UI", 10.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(15, 23, 42),
                BorderStyle = BorderStyle.None,
                Location = new Point(40, 11),
                UseSystemPasswordChar = isPassword
            };

            pnl.Controls.Add(lblIcon);
            pnl.Controls.Add(txt);
            return pnl;
        }

        private Panel CreateFieldContainerWithToggle(string icon, out TextBox txt, out Button btnEye, ref bool isMasked)
        {
            Panel pnl = new Panel { Height = 42, BackColor = Color.White };
            pnl.Paint += (s, pe) =>
            {
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (Pen borderPen = new Pen(Color.FromArgb(203, 213, 225), 1.2f))
                {
                    pe.Graphics.DrawRectangle(borderPen, 0, 0, pnl.Width - 1, pnl.Height - 1);
                }
            };

            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI", 11f),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(10, 9),
                Size = new Size(24, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };

            TextBox localTxt = new TextBox
            {
                Font = new Font("Segoe UI", 10.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(15, 23, 42),
                BorderStyle = BorderStyle.None,
                Location = new Point(40, 11),
                UseSystemPasswordChar = true
            };
            txt = localTxt;

            Button localEye = new Button
            {
                Text = "👁",
                Font = new Font("Segoe UI", 10f),
                Size = new Size(32, 28),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(100, 116, 139),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            localEye.FlatAppearance.BorderSize = 0;
            localEye.Location = new Point(pnl.Width - 36, 6);
            localEye.Click += (s, e) =>
            {
                localTxt.UseSystemPasswordChar = !localTxt.UseSystemPasswordChar;
                localEye.Text = localTxt.UseSystemPasswordChar ? "👁" : "🔒";
            };
            btnEye = localEye;

            pnl.Resize += (s, e) =>
            {
                localEye.Location = new Point(pnl.Width - 36, 6);
            };

            pnl.Controls.Add(lblIcon);
            pnl.Controls.Add(localTxt);
            pnl.Controls.Add(localEye);
            return pnl;
        }

        private void PerformRegistration()
        {
            string fullName = txtFullName.Text.Trim();
            string username = txtUsername.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();
            string password = txtPassword.Text;
            string confirmPass = txtConfirmPassword.Text;

            // Chuyển role text sang mã role DB
            string selectedRoleText = cboRole.SelectedItem?.ToString() ?? "SalesStaff";
            string role = "SalesStaff";
            if (selectedRoleText.Contains("DecisionMaker")) role = "DecisionMaker";
            else if (selectedRoleText.Contains("SalesManager")) role = "SalesManager";
            else if (selectedRoleText.Contains("Admin")) role = "Admin";

            btnRegister.Enabled = false;
            btnRegister.Text = "Đang lưu dữ liệu người dùng vào CSDL...";

            bool ok = UserSession.RegisterUser(
                username, 
                password, 
                confirmPass, 
                fullName, 
                email, 
                phone, 
                role, 
                connectionString, 
                out string errorMsg);

            if (ok)
            {
                this.RegisteredUsername = username;
                lblError.ForeColor = Color.FromArgb(16, 185, 129);
                lblError.Text = $"✓ Đăng ký thành công! Đang chuyển về màn hình đăng nhập...";
                lblError.Visible = true;

                System.Windows.Forms.Timer t = new System.Windows.Forms.Timer { Interval = 600 };
                t.Tick += (s, e) =>
                {
                    t.Stop();
                    t.Dispose();
                    MessageBox.Show(
                        $"Đăng ký tài khoản '{username}' thành công!\nDữ liệu đã được lưu an toàn vào bảng USERS (CSDL DSS_SalesDB).\nBạn có thể đăng nhập ngay bằng mật khẩu vừa thiết lập.",
                        "Thông báo Đăng ký thành công",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };
                t.Start();
            }
            else
            {
                btnRegister.Enabled = true;
                btnRegister.Text = "⚡ HOÀN TẤT ĐĂNG KÝ TÀI KHOẢN VÀO CSDL";
                lblError.ForeColor = Color.FromArgb(220, 38, 38);
                if (errorMsg.StartsWith("Không thể kết nối tới SQL Server"))
                {
                    lblError.Text = "✖ SQL Server chưa chạy - không thể đăng ký tài khoản!";
                    MessageBox.Show(this, errorMsg, "Lỗi kết nối SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    lblError.Text = errorMsg;
                }
                lblError.Visible = true;
            }
        }
    }
}
