#nullable disable
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace DSS_App
{
    // Panel Header tuỳ chỉnh tự vẽ đồ hoạ và văn bản chống lỗi vệt đen Transparent của Windows Forms
    public class HeaderPanel : Panel
    {
        private Image watermarkImage;
        private string tagText = "CỔNG ĐĂNG NHẬP HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH (DSS PORTAL)";
        private string titleText = "";
        private string subtitleText = "";

        public void SetHeaderText(string title, string subtitle, string tag = null)
        {
            if (title != null) titleText = title;
            if (subtitle != null) subtitleText = subtitle;
            if (tag != null) tagText = tag;
            this.Invalidate();
        }

        public void SetWatermarkImage(Image img)
        {
            watermarkImage = img;
            this.Invalidate();
        }

        public HeaderPanel()
        {
            this.DoubleBuffered = true;
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 1. Nền Navy Slate tối
            using (SolidBrush bgBrush = new SolidBrush(Color.FromArgb(15, 23, 42)))
            {
                g.FillRectangle(bgBrush, this.ClientRectangle);
            }

            // 2. Vẽ ảnh Watermark trải đều toàn bộ chiều rộng
            if (watermarkImage != null)
            {
                g.DrawImage(watermarkImage, new Rectangle(0, 0, this.Width, this.Height));
                using (SolidBrush overlay = new SolidBrush(Color.FromArgb(215, 10, 25, 55)))
                {
                    g.FillRectangle(overlay, 0, 0, this.Width, this.Height);
                }
            }
            else
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    this.ClientRectangle,
                    Color.FromArgb(14, 65, 160),
                    Color.FromArgb(5, 22, 65),
                    45f))
                {
                    g.FillRectangle(brush, this.ClientRectangle);
                }
            }

            // 3. Vẽ 4 ô gạch logo màu (Cyan, Blue, Indigo, Purple) đồng bộ SplashForm
            using (SolidBrush b1 = new SolidBrush(Color.FromArgb(56, 189, 248)))
            using (SolidBrush b2 = new SolidBrush(Color.FromArgb(96, 165, 250)))
            using (SolidBrush b3 = new SolidBrush(Color.FromArgb(129, 140, 248)))
            using (SolidBrush b4 = new SolidBrush(Color.FromArgb(192, 132, 252)))
            {
                g.FillRectangle(b1, 38, 42, 9, 9);
                g.FillRectangle(b2, 50, 42, 9, 9);
                g.FillRectangle(b3, 38, 54, 9, 9);
                g.FillRectangle(b4, 50, 54, 9, 9);
            }

            // 4. Vẽ chữ trực tiếp bằng GDI+ để không bị vệt đen / hộp đè
            using (Font tagFont = new Font("Segoe UI", 8.5f, FontStyle.Bold))
            using (SolidBrush tagBrush = new SolidBrush(Color.FromArgb(148, 163, 184)))
            {
                g.DrawString(tagText ?? "", tagFont, tagBrush, 38, 14);
            }

            using (Font brandFont = new Font("Segoe UI", 10.5f, FontStyle.Bold))
            using (SolidBrush brandBrush = new SolidBrush(Color.FromArgb(56, 189, 248)))
            {
                g.DrawString("DSS RETAIL DECISION STUDIO 2026", brandFont, brandBrush, 68, 42);
            }

            using (Font titleFont = new Font("Segoe UI", 17f, FontStyle.Bold))
            using (SolidBrush titleBrush = new SolidBrush(Color.White))
            {
                g.DrawString(titleText ?? "", titleFont, titleBrush, 36, 72);
            }

            using (Font subFont = new Font("Segoe UI", 9.5f, FontStyle.Regular))
            using (SolidBrush subBrush = new SolidBrush(Color.FromArgb(203, 213, 225)))
            {
                g.DrawString(subtitleText ?? "", subFont, subBrush, 38, 114);
            }

            // 5. Đường kẻ line Neon Electric Cyan phân cách Header và Body
            using (Pen borderPen = new Pen(Color.FromArgb(56, 189, 248), 2f))
            {
                g.DrawLine(borderPen, 0, this.Height - 1, this.Width, this.Height - 1);
            }
        }
    }

    public class LoginForm : Form
    {
        [DllImport("user32.dll")]
        private static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);

        private HeaderPanel pnlHeader;
        private Panel pnlBody;
        private Panel pnlUserBox;
        private Panel pnlPassBox;
        private Label lblUsername;
        private Label lblPassword;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Button btnRegister;
        private Label lblError;
        private Button btnTogglePassword;
        private Label btnMaximize;
        private CheckBox chkRemember;
        private Label lblFooter;
        private Image bgWatermarkImage;
        private bool isPasswordMasked = true;

        // Watermark / Placeholder Text
        private const string USER_WATERMARK = "Nhập tên đăng nhập, email hoặc SĐT...";
        private const string PASS_WATERMARK = "Nhập mật khẩu...";
        private bool isUserWatermark = true;
        private bool isPassWatermark = true;

        private readonly string connectionString = @"Server=.\SQLEXPRESS25;Database=DSS_SalesDB;Trusted_Connection=True;TrustServerCertificate=True;Timeout=3;";

        public LoginForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            // Kích thước chuẩn rộng rãi (760 x 600)
            this.Size = new Size(760, 600);
            this.MinimumSize = new Size(700, 560);
            this.BackColor = Color.FromArgb(248, 250, 252);
            this.DoubleBuffered = true;
            this.KeyPreview = true;

            LoadWatermarkBackground();
            BuildUI();

            this.AcceptButton = btnLogin;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    this.DialogResult = DialogResult.Cancel;
                    this.Close();
                }
            };

            this.Resize += (s, e) =>
            {
                if (btnMaximize != null)
                {
                    btnMaximize.Text = this.WindowState == FormWindowState.Maximized ? "❐" : "🗖";
                }
                RepositionPanelsAndControls();
            };
        }

        private void LoadWatermarkBackground()
        {
            try
            {
                string assetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "splash_bg.jpg");
                if (File.Exists(assetPath))
                {
                    bgWatermarkImage = Image.FromFile(assetPath);
                }
                else
                {
                    string devPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", "splash_bg.jpg");
                    if (File.Exists(devPath))
                    {
                        bgWatermarkImage = Image.FromFile(devPath);
                    }
                }
            }
            catch { }
        }

        private void BuildUI()
        {
            // ================= 1. HEADER PANEL =================
            pnlHeader = new HeaderPanel
            {
                Location = new Point(0, 0),
                Size = new Size(this.ClientSize.Width, 150)
            };
            pnlHeader.SetWatermarkImage(bgWatermarkImage);

            pnlHeader.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Left && this.WindowState != FormWindowState.Maximized)
                {
                    ReleaseCapture();
                    SendMessage(this.Handle, 0xA1, 0x2, 0);
                }
            };
            pnlHeader.DoubleClick += (s, e) =>
            {
                this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            };

            // Nút Đóng (✕)
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

            // Nút Phóng to / Thu nhỏ toàn màn hình (🗖 / ❐)
            btnMaximize = new Label
            {
                Text = "🗖",
                Font = new Font("Segoe UI", 11f, FontStyle.Regular),
                ForeColor = Color.FromArgb(203, 213, 225),
                Size = new Size(42, 34),
                Location = new Point(pnlHeader.Width - 92, 6),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnMaximize.MouseEnter += (s, e) => { btnMaximize.BackColor = Color.FromArgb(30, 41, 59); btnMaximize.ForeColor = Color.White; };
            btnMaximize.MouseLeave += (s, e) => { btnMaximize.BackColor = Color.Transparent; btnMaximize.ForeColor = Color.FromArgb(203, 213, 225); };
            btnMaximize.Click += (s, e) =>
            {
                this.WindowState = this.WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
            };

            // Nút Thu nhỏ xuống Taskbar (—)
            Label btnMinimize = new Label
            {
                Text = "—",
                Font = new Font("Segoe UI", 11f, FontStyle.Regular),
                ForeColor = Color.FromArgb(203, 213, 225),
                Size = new Size(40, 34),
                Location = new Point(pnlHeader.Width - 134, 6),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            btnMinimize.MouseEnter += (s, e) => { btnMinimize.BackColor = Color.FromArgb(30, 41, 59); btnMinimize.ForeColor = Color.White; };
            btnMinimize.MouseLeave += (s, e) => { btnMinimize.BackColor = Color.Transparent; btnMinimize.ForeColor = Color.FromArgb(203, 213, 225); };
            btnMinimize.Click += (s, e) => { this.WindowState = FormWindowState.Minimized; };

            pnlHeader.Controls.Add(btnClose);
            pnlHeader.Controls.Add(btnMaximize);
            pnlHeader.Controls.Add(btnMinimize);
            this.Controls.Add(pnlHeader);

            // ================= 2. BODY CONTENT PANEL (BẮT ĐẦU TỪ Y=150, TUYỆT ĐỐI KHÔNG BỊ CHE KHUẤT) =================
            pnlBody = new Panel
            {
                Location = new Point(0, 150),
                Size = new Size(this.ClientSize.Width, this.ClientSize.Height - 150),
                BackColor = Color.White
            };

            // 2.1 Label và Input Tài khoản
            lblUsername = new Label
            {
                Text = "Tên đăng nhập / Email / Số điện thoại:",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                AutoSize = true
            };
            pnlBody.Controls.Add(lblUsername);

            pnlUserBox = CreateInputContainer(46);
            Label lblUserIcon = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI", 12f),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(14, 11),
                Size = new Size(26, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };
            txtUsername = new TextBox
            {
                Text = USER_WATERMARK,
                Font = new Font("Segoe UI", 11f, FontStyle.Italic),
                ForeColor = Color.FromArgb(148, 163, 184),
                BorderStyle = BorderStyle.None,
                Location = new Point(48, 12)
            };

            txtUsername.Enter += (s, e) =>
            {
                pnlUserBox.Invalidate();
                if (isUserWatermark)
                {
                    txtUsername.Text = "";
                    txtUsername.Font = new Font("Segoe UI", 11f, FontStyle.Regular);
                    txtUsername.ForeColor = Color.FromArgb(15, 23, 42);
                    isUserWatermark = false;
                }
            };
            txtUsername.Leave += (s, e) =>
            {
                pnlUserBox.Invalidate();
                if (string.IsNullOrWhiteSpace(txtUsername.Text))
                {
                    txtUsername.Text = USER_WATERMARK;
                    txtUsername.Font = new Font("Segoe UI", 11f, FontStyle.Italic);
                    txtUsername.ForeColor = Color.FromArgb(148, 163, 184);
                    isUserWatermark = true;
                }
            };

            pnlUserBox.Controls.Add(lblUserIcon);
            pnlUserBox.Controls.Add(txtUsername);
            pnlBody.Controls.Add(pnlUserBox);

            // 2.2 Label và Input Mật khẩu
            lblPassword = new Label
            {
                Text = "Mật khẩu hệ thống:",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                AutoSize = true
            };
            pnlBody.Controls.Add(lblPassword);

            pnlPassBox = CreateInputContainer(46);
            Label lblPassIcon = new Label
            {
                Text = "🔒",
                Font = new Font("Segoe UI", 12f),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(14, 11),
                Size = new Size(26, 24),
                TextAlign = ContentAlignment.MiddleCenter
            };
            txtPassword = new TextBox
            {
                Text = PASS_WATERMARK,
                Font = new Font("Segoe UI", 11f, FontStyle.Italic),
                ForeColor = Color.FromArgb(148, 163, 184),
                BorderStyle = BorderStyle.None,
                Location = new Point(48, 12),
                UseSystemPasswordChar = false
            };

            btnTogglePassword = new Button
            {
                Text = "👁",
                Font = new Font("Segoe UI", 11f),
                Size = new Size(36, 32),
                FlatStyle = FlatStyle.Flat,
                ForeColor = Color.FromArgb(100, 116, 139),
                Cursor = Cursors.Hand
            };
            btnTogglePassword.FlatAppearance.BorderSize = 0;
            btnTogglePassword.Click += (s, e) =>
            {
                if (isPassWatermark) return;
                isPasswordMasked = !isPasswordMasked;
                txtPassword.UseSystemPasswordChar = isPasswordMasked;
                btnTogglePassword.Text = isPasswordMasked ? "👁" : "🔒";
            };

            txtPassword.Enter += (s, e) =>
            {
                pnlPassBox.Invalidate();
                if (isPassWatermark)
                {
                    txtPassword.Text = "";
                    txtPassword.Font = new Font("Segoe UI", 11f, FontStyle.Regular);
                    txtPassword.ForeColor = Color.FromArgb(15, 23, 42);
                    txtPassword.UseSystemPasswordChar = isPasswordMasked;
                    isPassWatermark = false;
                }
            };
            txtPassword.Leave += (s, e) =>
            {
                pnlPassBox.Invalidate();
                if (string.IsNullOrWhiteSpace(txtPassword.Text))
                {
                    txtPassword.UseSystemPasswordChar = false;
                    txtPassword.Text = PASS_WATERMARK;
                    txtPassword.Font = new Font("Segoe UI", 11f, FontStyle.Italic);
                    txtPassword.ForeColor = Color.FromArgb(148, 163, 184);
                    isPassWatermark = true;
                }
            };

            pnlPassBox.Controls.Add(lblPassIcon);
            pnlPassBox.Controls.Add(txtPassword);
            pnlPassBox.Controls.Add(btnTogglePassword);
            pnlBody.Controls.Add(pnlPassBox);

            // 2.3 Checkbox Ghi nhớ đăng nhập
            chkRemember = new CheckBox
            {
                Text = "Ghi nhớ thông tin đăng nhập trên thiết bị này",
                Checked = true,
                Font = new Font("Segoe UI", 10f, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105),
                AutoSize = true,
                Cursor = Cursors.Hand
            };
            pnlBody.Controls.Add(chkRemember);

            // 2.4 Label thông báo lỗi
            lblError = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38),
                Height = 24,
                TextAlign = ContentAlignment.MiddleLeft,
                Visible = false
            };
            pnlBody.Controls.Add(lblError);

            // 2.5 Nút Đăng nhập phong cách Indigo Sapphire chuẩn màu hệ thống DSS
            btnLogin = new Button
            {
                Text = "ĐĂNG NHẬP VÀO HỆ THỐNG DSS",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(79, 70, 229), // Indigo 600
                FlatStyle = FlatStyle.Flat,
                Height = 48,
                Cursor = Cursors.Hand
            };
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.MouseEnter += (s, e) => { btnLogin.BackColor = Color.FromArgb(67, 56, 202); };
            btnLogin.MouseLeave += (s, e) => { btnLogin.BackColor = Color.FromArgb(79, 70, 229); };
            btnLogin.Click += (s, e) => PerformLogin();
            pnlBody.Controls.Add(btnLogin);

            // 2.6 Nút Đăng ký tài khoản người dùng mới (Dữ liệu thật lưu vào dbo.USERS)
            btnRegister = new Button
            {
                Text = "⚡ CHƯA CÓ TÀI KHOẢN? ĐĂNG KÝ TÀI KHOẢN MỚI",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(79, 70, 229),
                BackColor = Color.FromArgb(238, 242, 255),
                FlatStyle = FlatStyle.Flat,
                Height = 44,
                Cursor = Cursors.Hand
            };
            btnRegister.FlatAppearance.BorderColor = Color.FromArgb(199, 210, 254);
            btnRegister.FlatAppearance.BorderSize = 1;
            btnRegister.MouseEnter += (s, e) => { btnRegister.BackColor = Color.FromArgb(224, 231, 255); };
            btnRegister.MouseLeave += (s, e) => { btnRegister.BackColor = Color.FromArgb(238, 242, 255); };
            btnRegister.Click += (s, e) => OpenRegisterForm();
            pnlBody.Controls.Add(btnRegister);

            // 2.7 Chú thích chân trang
            lblFooter = new Label
            {
                Text = "DSS Retail Decision Studio 2026 • Đồ án IS402 • Microsoft SQL Server DSS_SalesDB",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                Height = 22,
                TextAlign = ContentAlignment.MiddleCenter
            };
            pnlBody.Controls.Add(lblFooter);

            this.Controls.Add(pnlBody);

            RepositionPanelsAndControls();

            this.Paint += (s, pe) =>
            {
                using (Pen p = new Pen(Color.FromArgb(96, 165, 250), 1.5f))
                {
                    pe.Graphics.DrawRectangle(p, 0, 0, this.Width - 1, this.Height - 1);
                }
            };
        }

        // Định vị toạ độ chính xác 100% cho Header và Body, hỗ trợ căn giữa linh hoạt khi phóng to toàn màn hình
        private void RepositionPanelsAndControls()
        {
            if (pnlHeader != null)
            {
                pnlHeader.Width = this.ClientSize.Width;
                pnlHeader.Invalidate();
            }

            if (pnlBody != null)
            {
                pnlBody.Location = new Point(0, 150);
                pnlBody.Size = new Size(this.ClientSize.Width, Math.Max(400, this.ClientSize.Height - 150));

                // Chiều rộng khối form chính: căn giữa tự động, tối đa 620px
                int contentWidth = Math.Min(620, pnlBody.Width - 80);
                if (contentWidth < 500) contentWidth = pnlBody.Width - 40;
                int startX = (pnlBody.Width - contentWidth) / 2;

                // Căn giữa chiều dọc khi cửa sổ được phóng to
                int totalContentHeight = 350;
                int startY = Math.Max(20, (pnlBody.Height - totalContentHeight) / 2);

                if (lblUsername != null) lblUsername.Location = new Point(startX, startY);

                if (pnlUserBox != null)
                {
                    pnlUserBox.Location = new Point(startX, startY + 24);
                    pnlUserBox.Width = contentWidth;
                    if (txtUsername != null) txtUsername.Width = contentWidth - 64;
                }

                if (lblPassword != null) lblPassword.Location = new Point(startX, startY + 80);

                if (pnlPassBox != null)
                {
                    pnlPassBox.Location = new Point(startX, startY + 104);
                    pnlPassBox.Width = contentWidth;
                    if (txtPassword != null) txtPassword.Width = contentWidth - 110;
                    if (btnTogglePassword != null) btnTogglePassword.Location = new Point(contentWidth - 46, 7);
                }

                if (chkRemember != null) chkRemember.Location = new Point(startX, startY + 160);

                if (lblError != null)
                {
                    lblError.Location = new Point(startX, startY + 190);
                    lblError.Width = contentWidth;
                }

                if (btnLogin != null)
                {
                    btnLogin.Location = new Point(startX, startY + 218);
                    btnLogin.Width = contentWidth;
                }

                if (btnRegister != null)
                {
                    btnRegister.Location = new Point(startX, startY + 276);
                    btnRegister.Width = contentWidth;
                }

                if (lblFooter != null)
                {
                    lblFooter.Location = new Point(startX, startY + 332);
                    lblFooter.Width = contentWidth;
                }
            }
        }

        private Panel CreateInputContainer(int h)
        {
            Panel pnl = new Panel
            {
                Height = h,
                BackColor = Color.White
            };

            pnl.Paint += (s, pe) =>
            {
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                bool isFocused = (txtUsername != null && txtUsername.Focused && pnl == pnlUserBox)
                              || (txtPassword != null && txtPassword.Focused && pnl == pnlPassBox);

                Color borderColor = isFocused ? Color.FromArgb(79, 70, 229) : Color.FromArgb(203, 213, 225);
                float borderWidth = isFocused ? 2f : 1.2f;

                using (Pen borderPen = new Pen(borderColor, borderWidth))
                {
                    pe.Graphics.DrawRectangle(borderPen, 0, 0, pnl.Width - 1, h - 1);
                }
            };

            return pnl;
        }

        private void OpenRegisterForm()
        {
            lblError.Visible = false;
            using (RegisterForm regForm = new RegisterForm(connectionString, bgWatermarkImage))
            {
                if (regForm.ShowDialog(this) == DialogResult.OK && !string.IsNullOrWhiteSpace(regForm.RegisteredUsername))
                {
                    txtUsername.Text = regForm.RegisteredUsername;
                    txtUsername.Font = new Font("Segoe UI", 11f, FontStyle.Regular);
                    txtUsername.ForeColor = Color.FromArgb(15, 23, 42);
                    isUserWatermark = false;

                    txtPassword.Text = "";
                    txtPassword.Font = new Font("Segoe UI", 11f, FontStyle.Regular);
                    txtPassword.ForeColor = Color.FromArgb(15, 23, 42);
                    txtPassword.UseSystemPasswordChar = isPasswordMasked;
                    isPassWatermark = false;

                    lblError.ForeColor = Color.FromArgb(16, 185, 129);
                    lblError.Text = $"✓ Đăng ký tài khoản '{regForm.RegisteredUsername}' thành công! Vui lòng nhập mật khẩu để đăng nhập.";
                    lblError.Visible = true;

                    pnlUserBox.Invalidate();
                    pnlPassBox.Invalidate();
                    txtPassword.Focus();
                }
            }
        }

        private void PerformLogin()
        {
            string username = isUserWatermark ? "" : txtUsername.Text.Trim();
            string password = isPassWatermark ? "" : txtPassword.Text.Trim();

            btnLogin.Enabled = false;
            btnLogin.Text = "Đang kiểm tra thông tin đăng nhập...";

            bool success = UserSession.Authenticate(username, password, connectionString, out string error);

            if (success)
            {
                lblError.ForeColor = Color.FromArgb(16, 185, 129); // Xanh lá
                lblError.Text = $"✓ Đăng nhập thành công! Xin chào {UserSession.CurrentUser?.FullName}";
                lblError.Visible = true;
                btnLogin.Text = "Đang tải không gian làm việc...";

                System.Windows.Forms.Timer t = new System.Windows.Forms.Timer { Interval = 250 };
                t.Tick += (s, e) =>
                {
                    t.Stop();
                    t.Dispose();
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                };
                t.Start();
            }
            else
            {
                btnLogin.Enabled = true;
                btnLogin.Text = "ĐĂNG NHẬP VÀO HỆ THỐNG DSS";
                lblError.ForeColor = Color.FromArgb(220, 38, 38);
                if (error.StartsWith("Không thể kết nối tới SQL Server"))
                {
                    lblError.Text = "✖ SQL Server chưa chạy - hệ thống không thể hoạt động!";
                    MessageBox.Show(this, error, "Lỗi kết nối SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    lblError.Text = error;
                }
                lblError.Visible = true;
                txtPassword.Focus();
                if (!isPassWatermark) txtPassword.SelectAll();
            }
        }
    }
}
