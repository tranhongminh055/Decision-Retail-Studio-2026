using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace DSS_App
{
    public class SplashForm : Form
    {
        private Image? bgImage;
        private Label lblStatus;
        private Panel pnlProgressBar;
        private System.Windows.Forms.Timer? splashTimer;
        private int currentStep = 0;
        private bool isExiting = false;

        public SplashForm()
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(720, 440);
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.DoubleBuffered = true;
            this.BackColor = Color.FromArgb(10, 37, 88);

            LoadBackgroundImage();

          
            Label lblClose = new Label
            {
                Text = "✕",
                Font = new Font("Segoe UI", 12f, FontStyle.Regular),
                ForeColor = Color.FromArgb(200, 225, 255),
                Size = new Size(42, 36),
                Location = new Point(this.Width - 46, 4),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };
            lblClose.MouseEnter += (s, e) =>
            {
                lblClose.ForeColor = Color.White;
                lblClose.BackColor = Color.FromArgb(220, 220, 38, 38); // Đỏ nổi bật khi hover
            };
            lblClose.MouseLeave += (s, e) =>
            {
                lblClose.ForeColor = Color.FromArgb(200, 225, 255);
                lblClose.BackColor = Color.Transparent;
            };
            lblClose.MouseDown += (s, e) => ExitApplication();
            lblClose.Click += (s, e) => ExitApplication();
            this.Controls.Add(lblClose);
            lblClose.BringToFront();

            // Phím Esc để thoát ngay, phím bất kỳ hoặc click chuột để vào thẳng Dashboard ngay lập tức
            this.KeyPreview = true;
            this.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    ExitApplication();
                }
                else
                {
                    FinishAndOpenDashboard();
                }
            };
            this.MouseDown += (s, e) =>
            {
                if (e.X >= this.Width - 52 && e.Y <= 44)
                {
                    ExitApplication();
                }
                else
                {
                    FinishAndOpenDashboard();
                }
            };

            // 2. Tiêu Đề Chính: "DSS Retail Studio 2026" (Chuẩn tối giản, sang trọng như SSMS, KHÔNG CÒN 2 dòng text phụ)
            Label lblTitle = new Label
            {
                Text = "DSS Retail Studio 2026",
                Font = new Font("Segoe UI", 29f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(54, 178)
            };
            lblTitle.Click += (s, e) => FinishAndOpenDashboard();
            this.Controls.Add(lblTitle);

            // 3. Logo & Tên đơn vị ở góc dưới bên trái
            Panel pnlLogoBox = new Panel
            {
                Size = new Size(24, 24),
                Location = new Point(48, this.Height - 56),
                BackColor = Color.Transparent
            };
            pnlLogoBox.Paint += (s, pe) =>
            {
                Graphics g = pe.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (SolidBrush b1 = new SolidBrush(Color.FromArgb(56, 189, 248)))
                using (SolidBrush b2 = new SolidBrush(Color.FromArgb(96, 165, 250)))
                using (SolidBrush b3 = new SolidBrush(Color.FromArgb(129, 140, 248)))
                using (SolidBrush b4 = new SolidBrush(Color.FromArgb(192, 132, 252)))
                {
                    g.FillRectangle(b1, 0, 0, 10, 10);
                    g.FillRectangle(b2, 13, 0, 10, 10);
                    g.FillRectangle(b3, 0, 13, 10, 10);
                    g.FillRectangle(b4, 13, 13, 10, 10);
                }
            };
            pnlLogoBox.Click += (s, e) => FinishAndOpenDashboard();
            this.Controls.Add(pnlLogoBox);

            Label lblVendor = new Label
            {
                Text = "DSS Retail Studio  |  Enterprise Edition",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(224, 242, 254),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(78, this.Height - 53)
            };
            lblVendor.Click += (s, e) => FinishAndOpenDashboard();
            this.Controls.Add(lblVendor);

            // 4. Số Phiên Bản "26" ở góc dưới bên phải
            Label lblVersionNumber = new Label
            {
                Text = "26",
                Font = new Font("Segoe UI Light", 34f, FontStyle.Regular),
                ForeColor = Color.FromArgb(235, 245, 255),
                AutoSize = true,
                BackColor = Color.Transparent,
                Location = new Point(this.Width - 92, this.Height - 74)
            };
            lblVersionNumber.Click += (s, e) => FinishAndOpenDashboard();
            this.Controls.Add(lblVersionNumber);

            // 5. Dòng trạng thái đang tải (Status text)
            lblStatus = new Label
            {
                Text = "Đang kết nối cơ sở dữ liệu SQL Server...",
                Font = new Font("Segoe UI", 8.8f, FontStyle.Regular),
                ForeColor = Color.FromArgb(190, 227, 255),
                Size = new Size(520, 20),
                BackColor = Color.Transparent,
                Location = new Point(48, this.Height - 28)
            };
            this.Controls.Add(lblStatus);

            // 6. Thanh tiến trình chạy mượt ở cạnh đáy (Sleek Electric Blue Loading Line)
            Panel pnlProgressTrack = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 4,
                BackColor = Color.FromArgb(20, 55, 120)
            };

            pnlProgressBar = new Panel
            {
                Dock = DockStyle.Left,
                Width = 0,
                BackColor = Color.FromArgb(56, 189, 248) // Electric Cyan Blue
            };
            pnlProgressTrack.Controls.Add(pnlProgressBar);
            this.Controls.Add(pnlProgressTrack);

            // Hiệu ứng viền phát sáng nhẹ quanh cửa sổ
            this.Paint += (s, pe) =>
            {
                Graphics g = pe.Graphics;
                using (Pen borderPen = new Pen(Color.FromArgb(96, 165, 250), 1.5f))
                {
                    g.DrawRectangle(borderPen, 0, 0, this.Width - 1, this.Height - 1);
                }
            };

            // 7. Chạy animation mượt mà qua Timer (UI Thread tự do xử lý click nút X tức thì)
            this.Shown += (s, e) => CheckDatabaseThenStart();
        }

        private async void CheckDatabaseThenStart()
        {
            while (true)
            {
                lblStatus.Text = "Đang kết nối cơ sở dữ liệu SQL Server (DSS_SalesDB)...";
                string error = string.Empty;
                bool ok = await System.Threading.Tasks.Task.Run(() => DatabaseGuard.Check(out error));
                if (isExiting) return;

                if (ok)
                {
                    StartSplashAnimation();
                    return;
                }

                lblStatus.Text = "Không thể kết nối SQL Server!";
                this.TopMost = false;
                DialogResult choice = MessageBox.Show(this,
                    error + "\n\nBấm [Thử lại] sau khi đã khởi động SQL Server, hoặc [Hủy] để thoát.",
                    "Lỗi kết nối SQL Server - Hệ thống không thể hoạt động",
                    MessageBoxButtons.RetryCancel,
                    MessageBoxIcon.Error);
                this.TopMost = true;

                if (choice != DialogResult.Retry)
                {
                    ExitApplication();
                    return;
                }
            }
        }

        private void StartSplashAnimation()
        {
            var stages = new (int targetPercent, string text)[]
            {
                (30, "Đang kết nối cơ sở dữ liệu SQL Server (DSS_SalesDB)..."),
                (60, "Đang nạp Ma trận so sánh cặp Saaty 5x5 & Kiểm định CR..."),
                (85, "Đang kết nối Kho dữ liệu bán hàng đa chiều OLAP..."),
                (100, "Sẵn sàng khởi động DSS Retail Studio 2026!")
            };

            splashTimer = new System.Windows.Forms.Timer();
            splashTimer.Interval = 18; // ~55 FPS
            int targetWidth = 0;

            splashTimer.Tick += (s, e) =>
            {
                if (isExiting) return;

                if (currentStep < stages.Length)
                {
                    targetWidth = (int)((stages[currentStep].targetPercent / 100.0) * this.Width);
                    lblStatus.Text = stages[currentStep].text;

                    if (pnlProgressBar.Width < targetWidth)
                    {
                        pnlProgressBar.Width = Math.Min(this.Width, pnlProgressBar.Width + 24); // Tốc độ nhanh gọn ~0.8s
                    }
                    else
                    {
                        currentStep++;
                    }
                }
                else
                {
                    pnlProgressBar.Width = this.Width;
                    splashTimer.Stop();
                    FinishAndOpenDashboard();
                }
            };

            splashTimer.Start();
        }

        private void ExitApplication()
        {
            if (isExiting) return;
            isExiting = true;
            if (splashTimer != null) splashTimer.Stop();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
            Environment.Exit(0);
        }

        private void FinishAndOpenDashboard()
        {
            if (isExiting) return;
            isExiting = true;
            if (splashTimer != null) splashTimer.Stop();
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void LoadBackgroundImage()
        {
            try
            {
                string assetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "splash_bg.jpg");
                if (File.Exists(assetPath))
                {
                    bgImage = Image.FromFile(assetPath);
                }
                else
                {
                    string devAssetPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "Assets", "splash_bg.jpg");
                    if (File.Exists(devAssetPath))
                    {
                        bgImage = Image.FromFile(devAssetPath);
                    }
                }
            }
            catch { }
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (bgImage != null)
            {
                e.Graphics.DrawImage(bgImage, new Rectangle(0, 0, this.Width, this.Height));
            }
            else
            {
                using (LinearGradientBrush brush = new LinearGradientBrush(
                    this.ClientRectangle,
                    Color.FromArgb(14, 65, 160),
                    Color.FromArgb(5, 22, 65),
                    45f))
                {
                    e.Graphics.FillRectangle(brush, this.ClientRectangle);
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (splashTimer != null)
                {
                    splashTimer.Stop();
                    splashTimer.Dispose();
                }
                if (bgImage != null)
                {
                    bgImage.Dispose();
                }
            }
            base.Dispose(disposing);
        }
    }
}
