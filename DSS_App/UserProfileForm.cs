#nullable disable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DSS_App
{
    public class UserProfileForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private readonly string connectionString;

        private Panel pnlHeader;
        private Label lblCardName;
        private TabControl tabControl;
        private TabPage tabInfo;
        private TabPage tabSecurity;
        private TabPage tabPermissions;

        // Tab Info Controls
        private TextBox txtUserId;
        private TextBox txtUsername;
        private TextBox txtFullName;
        private TextBox txtEmail;
        private TextBox txtPhone;
        private TextBox txtRoleDisplay;
        private Button btnSaveProfile;

        // Tab Security Controls
        private TextBox txtCurrentPass;
        private TextBox txtNewPass;
        private TextBox txtConfirmPass;
        private Button btnToggleCurrentPass;
        private Button btnToggleNewPass;
        private Button btnToggleConfirmPass;
        private Button btnChangePassword;
        private bool isCurrentMasked = true;
        private bool isNewMasked = true;
        private bool isConfirmMasked = true;

        // Feedback / Status
        private Label lblStatusMessage;

        public bool IsProfileUpdated { get; private set; } = false;

        public UserProfileForm(string dbConnString, Image watermark = null)
        {
            this.connectionString = dbConnString;

            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterParent;
            this.Size = new Size(660, 560);
            this.MinimumSize = new Size(620, 520);
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.DoubleBuffered = true;
            this.KeyPreview = true;

            BuildUI();
            LoadUserData();

            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = IsProfileUpdated ? DialogResult.OK : DialogResult.Cancel;
                    this.Close();
                }
            };

            this.Paint += (s, pe) =>
            {
                using (Pen p = new Pen(Color.FromArgb(96, 165, 250), 2f))
                {
                    pe.Graphics.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);
                }
            };
        }

        private void BuildUI()
        {
            // ================= 1. HEADER TINH GỌN =================
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 70,
                BackColor = Color.FromArgb(15, 23, 42), // Slate 900
                Padding = new Padding(20, 10, 20, 10)
            };

            pnlHeader.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, 0xA1, 0x2, 0);
                }
            };

            // Avatar tròn
            Panel pnlAvatarCircle = new Panel
            {
                Size = new Size(46, 46),
                Location = new Point(20, 12),
                BackColor = Color.FromArgb(30, 41, 59)
            };
            pnlAvatarCircle.Paint += (s, pe) =>
            {
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = new GraphicsPath())
                {
                    path.AddEllipse(0, 0, pnlAvatarCircle.Width - 1, pnlAvatarCircle.Height - 1);
                    pnlAvatarCircle.Region = new Region(path);
                    using (Pen ringPen = new Pen(Color.FromArgb(56, 189, 248), 2f))
                    {
                        pe.Graphics.DrawEllipse(ringPen, 1, 1, pnlAvatarCircle.Width - 3, pnlAvatarCircle.Height - 3);
                    }
                }
            };
            Label lblAvatarIcon = new Label
            {
                Dock = DockStyle.Fill,
                Text = "👤",
                Font = new Font("Segoe UI", 16f),
                ForeColor = Color.FromArgb(56, 189, 248),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand
            };
            pnlAvatarCircle.Controls.Add(lblAvatarIcon);
            pnlHeader.Controls.Add(pnlAvatarCircle);

            // Tên người dùng
            var user = UserSession.CurrentUser;
            lblCardName = new Label
            {
                Text = user?.FullName ?? "Chưa xác định",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(78, 20),
                AutoSize = true
            };
            pnlHeader.Controls.Add(lblCardName);

            // Nút đóng ✕
            Label btnClose = new Label
            {
                Text = "✕",
                Font = new Font("Segoe UI", 12f, FontStyle.Regular),
                ForeColor = Color.FromArgb(203, 213, 225),
                Size = new Size(36, 32),
                Location = new Point(this.Width - 42, 8),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnClose.MouseEnter += (s, e) => { btnClose.BackColor = Color.FromArgb(239, 68, 68); btnClose.ForeColor = Color.White; };
            btnClose.MouseLeave += (s, e) => { btnClose.BackColor = Color.Transparent; btnClose.ForeColor = Color.FromArgb(203, 213, 225); };
            btnClose.Click += (s, e) =>
            {
                this.DialogResult = IsProfileUpdated ? DialogResult.OK : DialogResult.Cancel;
                this.Close();
            };
            pnlHeader.Controls.Add(btnClose);

            // ================= 2. BOTTOM BAR =================
            Panel pnlBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 56,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(20, 8, 20, 8)
            };
            pnlBottom.Paint += (s, pe) =>
            {
                using (Pen topPen = new Pen(Color.FromArgb(226, 232, 240), 1f))
                {
                    pe.Graphics.DrawLine(topPen, 0, 0, pnlBottom.Width, 0);
                }
            };

            lblStatusMessage = new Label
            {
                Location = new Point(20, 8),
                Size = new Size(480, 36),
                Font = new Font("Segoe UI", 9.2f, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleLeft
            };
            pnlBottom.Controls.Add(lblStatusMessage);

            Button btnCloseBottom = new Button
            {
                Text = "Đóng",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(90, 34),
                Location = new Point(pnlBottom.Width - 110, 10),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnCloseBottom.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnCloseBottom.Click += (s, e) =>
            {
                this.DialogResult = IsProfileUpdated ? DialogResult.OK : DialogResult.Cancel;
                this.Close();
            };
            pnlBottom.Controls.Add(btnCloseBottom);

            // ================= 3. TAB CONTROL =================
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                Padding = new Point(16, 8)
            };

            tabInfo = new TabPage("📋 Thông Tin Cá Nhân") { BackColor = Color.White };
            tabSecurity = new TabPage("🔒 Đổi Mật Khẩu") { BackColor = Color.White };
            tabPermissions = new TabPage("🛡️ Quyền Hạn") { BackColor = Color.White };

            BuildTabInfo();
            BuildTabSecurity();
            BuildTabPermissions();

            tabControl.TabPages.Add(tabInfo);
            tabControl.TabPages.Add(tabSecurity);
            tabControl.TabPages.Add(tabPermissions);

            this.Controls.Add(tabControl);
            this.Controls.Add(pnlBottom);
            this.Controls.Add(pnlHeader);

            pnlHeader.SendToBack();
            pnlBottom.SendToBack();
            tabControl.BringToFront();
        }

        private void BuildTabInfo()
        {
            tabInfo.Padding = new Padding(24, 18, 24, 18);
            TableLayoutPanel tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 7,
                AutoScroll = true
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150f));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            int row = 0;
            AddTableRow(tbl, row++, "Mã tài khoản (ID):", txtUserId = CreateTextBox(true));
            AddTableRow(tbl, row++, "Tên đăng nhập:", txtUsername = CreateTextBox(true));
            AddTableRow(tbl, row++, "Vai trò (Role):", txtRoleDisplay = CreateTextBox(true));
            AddTableRow(tbl, row++, "Họ và tên (*):", txtFullName = CreateTextBox(false));
            AddTableRow(tbl, row++, "Email (*):", txtEmail = CreateTextBox(false));
            AddTableRow(tbl, row++, "Số điện thoại:", txtPhone = CreateTextBox(false));

            btnSaveProfile = new Button
            {
                Text = "💾 Lưu Thay Đổi",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(37, 99, 235), // Blue 600
                FlatStyle = FlatStyle.Flat,
                Size = new Size(160, 38),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 10, 0, 0)
            };
            btnSaveProfile.FlatAppearance.BorderSize = 0;
            btnSaveProfile.MouseEnter += (s, e) => btnSaveProfile.BackColor = Color.FromArgb(29, 78, 216);
            btnSaveProfile.MouseLeave += (s, e) => btnSaveProfile.BackColor = Color.FromArgb(37, 99, 235);
            btnSaveProfile.Click += BtnSaveProfile_Click;

            tbl.Controls.Add(new Label(), 0, row);
            tbl.Controls.Add(btnSaveProfile, 1, row);

            tabInfo.Controls.Add(tbl);
        }

        private void BuildTabSecurity()
        {
            tabSecurity.Padding = new Padding(24, 18, 24, 18);
            TableLayoutPanel tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 5,
                AutoScroll = true
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160f));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

            int row = 0;

            // Mật khẩu hiện tại
            Panel pnlCur = CreatePasswordBox(out txtCurrentPass, out btnToggleCurrentPass, ref isCurrentMasked);
            AddTableRow(tbl, row++, "Mật khẩu hiện tại (*):", pnlCur);

            // Mật khẩu mới
            Panel pnlNew = CreatePasswordBox(out txtNewPass, out btnToggleNewPass, ref isNewMasked);
            AddTableRow(tbl, row++, "Mật khẩu mới (*):", pnlNew);

            // Xác nhận mật khẩu mới
            Panel pnlConf = CreatePasswordBox(out txtConfirmPass, out btnToggleConfirmPass, ref isConfirmMasked);
            AddTableRow(tbl, row++, "Xác nhận mật khẩu (*):", pnlConf);

            Label lblNote = new Label
            {
                Text = "📌 Mật khẩu tối thiểu 6 ký tự, mã hóa bảo mật SHA-256.",
                Font = new Font("Segoe UI", 8.8f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                AutoSize = true,
                Margin = new Padding(0, 6, 0, 12)
            };
            tbl.Controls.Add(lblNote, 1, row++);

            btnChangePassword = new Button
            {
                Text = "🔒 Đổi Mật Khẩu",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(16, 185, 129), // Emerald 500
                FlatStyle = FlatStyle.Flat,
                Size = new Size(160, 38),
                Cursor = Cursors.Hand
            };
            btnChangePassword.FlatAppearance.BorderSize = 0;
            btnChangePassword.MouseEnter += (s, e) => btnChangePassword.BackColor = Color.FromArgb(5, 150, 105);
            btnChangePassword.MouseLeave += (s, e) => btnChangePassword.BackColor = Color.FromArgb(16, 185, 129);
            btnChangePassword.Click += BtnChangePassword_Click;

            tbl.Controls.Add(btnChangePassword, 1, row);

            tabSecurity.Controls.Add(tbl);
        }

        private void BuildTabPermissions()
        {
            tabPermissions.Padding = new Padding(24, 16, 24, 16);
            FlowLayoutPanel pnl = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true
            };

            var user = UserSession.CurrentUser;
            Label lblHeader = new Label
            {
                Text = $"Quyền hạn vai trò: [{user?.Role ?? "User"}]",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                AutoSize = true,
                Margin = new Padding(0, 0, 0, 8)
            };
            pnl.Controls.Add(lblHeader);

            AddPermissionItem(pnl, "Tra cứu dữ liệu bán hàng & xem báo cáo DSS", true);
            AddPermissionItem(pnl, "Phân tích đa chiều OLAP & xem Dashboard", user?.IsSalesStaff != true);
            AddPermissionItem(pnl, "Mô phỏng độ nhạy What-If & kịch bản thị trường", user?.IsSalesStaff != true);
            AddPermissionItem(pnl, "Thẩm định động & thêm sản phẩm mới", user?.IsSalesStaff != true);
            AddPermissionItem(pnl, "Thiết lập ma trận so sánh cặp AHP Saaty & Tỷ số CR", user?.IsDecisionMaker == true);
            AddPermissionItem(pnl, "Chạy DSS & phê duyệt kế hoạch nhập hàng", user?.IsDecisionMaker == true);
            AddPermissionItem(pnl, "Xuất báo cáo thẩm định DSS sang Word / HTML", user?.IsDecisionMaker == true || user?.IsSalesManager == true);
            AddPermissionItem(pnl, "Quản trị hệ thống & CSDL SQL Server", user?.IsAdmin == true);

            tabPermissions.Controls.Add(pnl);
        }

        private void AddPermissionItem(FlowLayoutPanel parent, string title, bool isGranted)
        {
            Panel card = new Panel
            {
                Width = 580,
                Height = 36,
                BackColor = isGranted ? Color.FromArgb(240, 253, 244) : Color.FromArgb(248, 250, 252),
                Margin = new Padding(0, 3, 0, 3),
                Padding = new Padding(10, 6, 10, 6)
            };
            card.Paint += (s, pe) =>
            {
                using (Pen border = new Pen(isGranted ? Color.FromArgb(187, 247, 208) : Color.FromArgb(226, 232, 240), 1f))
                {
                    pe.Graphics.DrawRectangle(border, 0, 0, card.Width - 1, card.Height - 1);
                }
            };

            Label lblIcon = new Label
            {
                Text = isGranted ? "✅" : "⛔",
                Font = new Font("Segoe UI", 10f),
                Location = new Point(8, 7),
                Size = new Size(24, 22)
            };
            card.Controls.Add(lblIcon);

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                ForeColor = isGranted ? Color.FromArgb(22, 101, 52) : Color.FromArgb(148, 163, 184),
                Location = new Point(36, 8),
                AutoSize = true
            };
            card.Controls.Add(lblTitle);

            parent.Controls.Add(card);
        }

        private void AddTableRow(TableLayoutPanel tbl, int row, string labelText, Control inputControl)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleRight,
                Padding = new Padding(0, 0, 12, 0)
            };
            tbl.Controls.Add(lbl, 0, row);
            tbl.Controls.Add(inputControl, 1, row);
        }

        private TextBox CreateTextBox(bool isReadOnly)
        {
            TextBox txt = new TextBox
            {
                Font = new Font("Segoe UI", 10f),
                Width = 340,
                ReadOnly = isReadOnly,
                BackColor = isReadOnly ? Color.FromArgb(241, 245, 249) : Color.White,
                ForeColor = isReadOnly ? Color.FromArgb(100, 116, 139) : Color.FromArgb(15, 23, 42),
                BorderStyle = BorderStyle.FixedSingle,
                Margin = new Padding(0, 4, 0, 8)
            };
            return txt;
        }

        private Panel CreatePasswordBox(out TextBox txt, out Button btnEye, ref bool isMasked)
        {
            Panel pnl = new Panel
            {
                Width = 340,
                Height = 34,
                BackColor = Color.White,
                Margin = new Padding(0, 4, 0, 8)
            };
            pnl.Paint += (s, pe) =>
            {
                using (Pen p = new Pen(Color.FromArgb(203, 213, 225), 1.2f))
                {
                    pe.Graphics.DrawRectangle(p, 0, 0, pnl.Width - 1, pnl.Height - 1);
                }
            };

            txt = new TextBox
            {
                Font = new Font("Segoe UI", 10f),
                BorderStyle = BorderStyle.None,
                Location = new Point(8, 7),
                Width = 290,
                UseSystemPasswordChar = true
            };

            bool currentMaskState = isMasked;
            TextBox refTxt = txt;
            btnEye = new Button
            {
                Text = "👁",
                Font = new Font("Segoe UI", 9f),
                Size = new Size(32, 28),
                Location = new Point(304, 3),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnEye.FlatAppearance.BorderSize = 0;
            btnEye.Click += (s, e) =>
            {
                currentMaskState = !currentMaskState;
                refTxt.UseSystemPasswordChar = currentMaskState;
            };

            pnl.Controls.Add(txt);
            pnl.Controls.Add(btnEye);
            return pnl;
        }

        private void LoadUserData()
        {
            var user = UserSession.CurrentUser;
            if (user == null) return;

            txtUserId.Text = user.UserId.ToString();
            txtUsername.Text = user.Username;
            txtRoleDisplay.Text = $"[{user.Role}] - {user.RoleTitle}";
            txtFullName.Text = user.FullName;
            txtEmail.Text = user.Email;
            txtPhone.Text = user.PhoneNumber;
        }

        private void BtnSaveProfile_Click(object sender, EventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user == null) return;

            string fullName = txtFullName.Text.Trim();
            string email = txtEmail.Text.Trim();
            string phone = txtPhone.Text.Trim();

            bool success = UserSession.UpdateProfile(user.UserId, fullName, email, phone, connectionString, out string error);
            if (success)
            {
                IsProfileUpdated = true;
                if (lblCardName != null) lblCardName.Text = fullName;
                lblStatusMessage.ForeColor = Color.FromArgb(22, 101, 52);
                lblStatusMessage.Text = "✓ Cập nhật thành công!";
                MessageBox.Show("Cập nhật thông tin hồ sơ thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblStatusMessage.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatusMessage.Text = $"✗ Lỗi: {error}";
                MessageBox.Show(error, "Lỗi cập nhật", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnChangePassword_Click(object sender, EventArgs e)
        {
            var user = UserSession.CurrentUser;
            if (user == null) return;

            string curPass = txtCurrentPass.Text;
            string newPass = txtNewPass.Text;
            string confPass = txtConfirmPass.Text;

            bool success = UserSession.ChangePassword(user.UserId, curPass, newPass, confPass, connectionString, out string error);
            if (success)
            {
                lblStatusMessage.ForeColor = Color.FromArgb(22, 101, 52);
                lblStatusMessage.Text = "✓ Đổi mật khẩu thành công!";
                txtCurrentPass.Clear();
                txtNewPass.Clear();
                txtConfirmPass.Clear();
                MessageBox.Show("Đổi mật khẩu thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblStatusMessage.ForeColor = Color.FromArgb(220, 38, 38);
                lblStatusMessage.Text = $"✗ Lỗi: {error}";
                MessageBox.Show(error, "Lỗi đổi mật khẩu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
