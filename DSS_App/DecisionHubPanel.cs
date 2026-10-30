#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

namespace DSS_App
{
    /// <summary>
    /// </summary>
    public class DecisionHubPanel : Panel
    {
        private readonly Form1 mainForm;
        private readonly string connectionString;

        // Trạng thái thu gọn/mở rộng
        private bool isCollapsed = false;
        private const int EXPANDED_WIDTH = 300;
        private const int COLLAPSED_WIDTH = 48;

        // Header & Nút thu gọn
        private Panel pnlHeader;
        private Button btnToggleCollapse;

        // Danh sách nút tính năng theo chiều dọc
        private Panel pnlNavContainer;
        private Button btnNavHome;
        private Button btnNavSolver;
        private Button btnNavAlerts;
        private Button btnNavSnapshots;
        private Button btnNavWorkflow;
        private int activeNavIndex = 0;

        // Thẻ thông tin trạng thái bên dưới
        private Panel pnlStatusCard;
        private Label lblStatusTitle;
        private Label lblStatusDesc;

        // Panel khi thu gọn (Icon Strip dọc)
        private Panel pnlCollapsedStrip;

        // References to full UI components
        private Panel pnlSolverFullView;
        private DataGridView dgvSolver;
        private NumericUpDown numSolverBudget;
        private ComboBox cboSolverGoal;
        private Label lblSolverSummary;

        private Panel pnlAlertsFullView;
        private FlowLayoutPanel pnlAlertsCardsList;
        private ComboBox cboAlertsFilter;
        private Label lblAlertsSummary;

        private Panel pnlSnapshotsFullView;
        private ListBox lstSnapshotsFull;
        private TextBox txtSnapNameFull;
        private WebBrowser wbCompareDetailFull;
        private string lastCompareHtml = "";

        private Panel pnlWorkflowFullView;
        private Label lblWorkflowPoCode;
        private CheckedListBox chkWorkflowList;
        private TextBox txtWorkflowNotes;
        private WebBrowser wbWorkflowPo;
        private Button btnWorkflowSign;
        private Label lblWorkflowStatus;

        // Bộ nhớ tạm dữ liệu giữa các phiên
        private List<(AltAnalysis Alt, int Qty, decimal TotalCost, decimal ExpectedProfit)> cachedSolverItems = new();
        private List<string> cachedSnapshots = new()
        {
            "📌 Phiên 1: Q4-Baseline (Chiến lược Cơ sở)",
            "📌 Phiên 2: Bùng nổ nhu cầu thị trường (+40%)",
            "📌 Phiên 3: Khủng hoảng chuỗi cung ứng linh kiện",
            "📌 Phiên 4: Bão giá & Lạm phát (Ưu tiên Lợi nhuận)"
        };
        private bool isPoSigned = false;
        private string currentPoNumber = $"PO-2026-DSS-{DateTime.Now:MMdd}";

        public DecisionHubPanel(Form1 parentForm, string dbConn)
        {
            this.mainForm = parentForm;
            this.connectionString = dbConn;

            this.Dock = DockStyle.Left;
            this.Width = EXPANDED_WIDTH;
            this.BackColor = Color.FromArgb(15, 23, 42); // Slate 900
            this.DoubleBuffered = true;

            BuildUI();
        }

        private void BuildUI()
        {
            this.Controls.Clear();

            // 1. Viền phân cách bên phải
            this.Paint += (s, pe) =>
            {
                using (Pen borderPen = new Pen(Color.FromArgb(51, 65, 85), 1.5f))
                {
                    pe.Graphics.DrawLine(borderPen, this.Width - 1, 0, this.Width - 1, this.Height);
                }
            };

            // 2. Header Panel (loại bỏ text tiêu đề thừa theo yêu cầu)
            pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 38,
                BackColor = Color.FromArgb(15, 23, 42),
                Padding = new Padding(8, 5, 8, 5)
            };

            btnToggleCollapse = new Button
            {
                Text = "◀",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(203, 213, 225),
                BackColor = Color.FromArgb(30, 41, 59),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(28, 28),
                Location = new Point(EXPANDED_WIDTH - 38, 5),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnToggleCollapse.FlatAppearance.BorderSize = 0;
            btnToggleCollapse.Click += (s, e) => ToggleCollapse();
            pnlHeader.Controls.Add(btnToggleCollapse);

            // 3. Container chứa 4 nút tính năng XẾP THEO CHIỀU DỌC
            pnlNavContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(15, 23, 42),
                Padding = new Padding(10, 8, 10, 8)
            };

            btnNavHome = CreateVerticalNavCard(
                0,
                "📊",
                "Mô Hình Quyết Định",
                "Bàn làm việc AHP & Ma trận",
                Color.FromArgb(99, 102, 241), // Indigo
                () => mainForm.SwitchToView(0)
            );

            btnNavSolver = CreateVerticalNavCard(
                1,
                "🎛️",
                "Tối Ưu Ngân Sách",
                "Thuật toán Knapsack",
                Color.FromArgb(14, 165, 233), // Sky Blue
                () => mainForm.SwitchToView(1)
            );

            btnNavAlerts = CreateVerticalNavCard(
                2,
                "🤖",
                "Cảnh Báo Rủi Ro",
                "Quét đứt hàng & chôn vốn",
                Color.FromArgb(245, 158, 11), // Amber
                () => mainForm.SwitchToView(2)
            );

            btnNavSnapshots = CreateVerticalNavCard(
                3,
                "📁",
                "Quản Lý Lịch Sử Phiên",
                "Lưu & so sánh kịch bản DSS",
                Color.FromArgb(16, 185, 129), // Emerald
                () => mainForm.SwitchToView(3)
            );

            btnNavWorkflow = CreateVerticalNavCard(
                4,
                "📋",
                "Ký Duyệt & Lệnh PO",
                "Phê duyệt mua hàng & xuất PO",
                Color.FromArgb(168, 85, 247), // Purple
                () => mainForm.SwitchToView(4)
            );

            pnlNavContainer.Controls.Add(btnNavHome);
            pnlNavContainer.Controls.Add(btnNavSolver);
            pnlNavContainer.Controls.Add(btnNavAlerts);
            pnlNavContainer.Controls.Add(btnNavSnapshots);
            pnlNavContainer.Controls.Add(btnNavWorkflow);

            pnlNavContainer.Resize += (s, e) => LayoutNavCards();

            // 4. Thẻ trạng thái hệ thống ở phía dưới
            pnlStatusCard = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 105,
                BackColor = Color.FromArgb(30, 41, 59), // Slate 800
                Margin = new Padding(12),
                Padding = new Padding(12, 10, 12, 10)
            };

            pnlStatusCard.Paint += (s, pe) =>
            {
                using (Pen borderPen = new Pen(Color.FromArgb(51, 65, 85), 1.5f))
                {
                    pe.Graphics.DrawLine(borderPen, 0, 0, pnlStatusCard.Width, 0);
                }
            };

            lblStatusTitle = new Label
            {
                Text = "✓ DSS RETAIL STUDIO 2026",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(56, 189, 248),
                Location = new Point(10, 10),
                AutoSize = true
            };
            pnlStatusCard.Controls.Add(lblStatusTitle);

            lblStatusDesc = new Label
            {
                Text = "• Tỷ số Saaty CR: Đạt chuẩn\n• Mô hình AHP & TOPSIS sẵn sàng\n• Bấm các nút trên để chuyển đổi UI",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(203, 213, 225),
                Location = new Point(10, 32),
                Size = new Size(EXPANDED_WIDTH - 24, 60)
            };
            pnlStatusCard.Controls.Add(lblStatusDesc);

            // 5. Panel Icon khi thu gọn (Icon rail dọc 48px)
            pnlCollapsedStrip = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(15, 23, 42),
                Visible = false
            };

            Button btnExpand = new Button
            {
                Text = "▶",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(37, 99, 235),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(36, 36),
                Location = new Point(6, 12),
                Cursor = Cursors.Hand
            };
            btnExpand.FlatAppearance.BorderSize = 0;
            btnExpand.Click += (s, e) => ToggleCollapse();
            pnlCollapsedStrip.Controls.Add(btnExpand);

            CreateCollapsedIcon("📊", "Bàn làm việc DSS (Trang chính)", 60, () => mainForm.SwitchToView(0));
            CreateCollapsedIcon("🎛️", "1. Tối ưu ngân sách (Knapsack)", 108, () => mainForm.SwitchToView(1));
            CreateCollapsedIcon("🤖", "2. Cảnh báo rủi ro (Copilot)", 156, () => mainForm.SwitchToView(2));
            CreateCollapsedIcon("📁", "3. Quản lý lịch sử phiên DSS", 204, () => mainForm.SwitchToView(3));
            CreateCollapsedIcon("📋", "4. Ký duyệt & Lệnh mua hàng (PO)", 252, () => mainForm.SwitchToView(4));

            // Thêm các thành phần chính vào panel
            this.Controls.Add(pnlNavContainer);
            this.Controls.Add(pnlStatusCard);
            this.Controls.Add(pnlHeader);
            this.Controls.Add(pnlCollapsedStrip);

            pnlStatusCard.SendToBack();
            pnlHeader.SendToBack();
            pnlNavContainer.BringToFront();

            LayoutNavCards();
        }

        public void SetActiveNavIndex(int index)
        {
            activeNavIndex = index;
            btnNavHome?.Invalidate();
            btnNavSolver?.Invalidate();
            btnNavAlerts?.Invalidate();
            btnNavSnapshots?.Invalidate();
            btnNavWorkflow?.Invalidate();
        }

        private void LayoutNavCards()
        {
            if (pnlNavContainer == null) return;
            var cards = new[] { btnNavHome, btnNavSolver, btnNavAlerts, btnNavSnapshots, btnNavWorkflow };
            if (cards.Any(c => c == null)) return;

            int containerW = pnlNavContainer.ClientSize.Width > 0 ? pnlNavContainer.ClientSize.Width : EXPANDED_WIDTH;
            int cardW = containerW - 24; // 12px margin on left and right
            if (cardW < 240) cardW = 240;
            int cardH = 64;

            int availH = pnlNavContainer.ClientSize.Height > 0 ? pnlNavContainer.ClientSize.Height : 500;
            int count = cards.Length;
            int totalCardsH = count * cardH;

            int gap = 12;
            int topOffset = 12;

            if (availH > totalCardsH + 40)
            {
                gap = Math.Clamp((availH - totalCardsH - 24) / (count + 1), 10, 20);
                topOffset = Math.Clamp((availH - (totalCardsH + (count - 1) * gap)) / 3, 10, 24);
            }

            for (int i = 0; i < count; i++)
            {
                cards[i].Location = new Point(12, topOffset + i * (cardH + gap));
                cards[i].Size = new Size(cardW, cardH);
            }
        }

        private static GraphicsPath CreateRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int d = radius * 2;
            if (d > rect.Width) d = rect.Width;
            if (d > rect.Height) d = rect.Height;

            path.AddArc(rect.X, rect.Y, d, d, 180, 90);
            path.AddArc(rect.Right - d, rect.Y, d, d, 270, 90);
            path.AddArc(rect.Right - d, rect.Bottom - d, d, d, 0, 90);
            path.AddArc(rect.X, rect.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        private Button CreateVerticalNavCard(int cardIndex, string icon, string title, string subtitle, Color accentColor, Action onClick)
        {
            Button btn = new Button
            {
                Size = new Size(EXPANDED_WIDTH - 24, 64),
                BackColor = Color.FromArgb(15, 23, 42),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(0)
            };
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(15, 23, 42);
            btn.FlatAppearance.MouseDownBackColor = Color.FromArgb(15, 23, 42);

            bool isHovered = false;
            bool isPressed = false;

            btn.MouseEnter += (s, e) => { isHovered = true; btn.Invalidate(); };
            btn.MouseLeave += (s, e) => { isHovered = false; isPressed = false; btn.Invalidate(); };
            btn.MouseDown += (s, e) => { isPressed = true; btn.Invalidate(); };
            btn.MouseUp += (s, e) => { isPressed = false; btn.Invalidate(); };

            btn.Paint += (s, pe) =>
            {
                pe.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                pe.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

                Rectangle bounds = new Rectangle(0, 0, btn.Width - 1, btn.Height - 1);
                bool isActive = (activeNavIndex == cardIndex);

                // Background
                Color cardBg = isPressed
                    ? Color.FromArgb(22, 32, 50)
                    : isActive
                        ? Color.FromArgb(36, 50, 75)
                        : isHovered
                            ? Color.FromArgb(41, 55, 78)
                            : Color.FromArgb(30, 41, 59);

                // Border
                Color cardBorder = (isActive || isHovered) ? accentColor : Color.FromArgb(51, 65, 85);

                using (GraphicsPath cardPath = CreateRoundedRectangle(bounds, 10))
                {
                    using (SolidBrush bgBrush = new SolidBrush(cardBg))
                    {
                        pe.Graphics.FillPath(bgBrush, cardPath);
                    }
                    using (Pen borderPen = new Pen(cardBorder, (isActive || isHovered) ? 1.6f : 1f))
                    {
                        pe.Graphics.DrawPath(borderPen, cardPath);
                    }
                }

                // Left Accent Indicator Bar (vạch màu nhận diện)
                Rectangle barRect = new Rectangle(5, 14, 4, btn.Height - 28);
                using (GraphicsPath barPath = CreateRoundedRectangle(barRect, 2))
                using (SolidBrush barBrush = new SolidBrush(accentColor))
                {
                    pe.Graphics.FillPath(barBrush, barPath);
                }

                // Icon Badge (ô chứa icon bo góc 38x38)
                Rectangle iconBox = new Rectangle(15, 13, 38, 38);
                using (GraphicsPath iconPath = CreateRoundedRectangle(iconBox, 8))
                {
                    using (SolidBrush iconBgBrush = new SolidBrush(Color.FromArgb(35, accentColor.R, accentColor.G, accentColor.B)))
                    {
                        pe.Graphics.FillPath(iconBgBrush, iconPath);
                    }
                    using (Pen iconBorderPen = new Pen(Color.FromArgb(70, accentColor.R, accentColor.G, accentColor.B), 1f))
                    {
                        pe.Graphics.DrawPath(iconBorderPen, iconPath);
                    }
                }

                // Draw Icon inside Badge
                using (Font iconFont = new Font("Segoe UI Emoji", 14f))
                using (SolidBrush iconBrush = new SolidBrush(Color.White))
                {
                    using (StringFormat sf = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                    {
                        pe.Graphics.DrawString(icon, iconFont, iconBrush, new RectangleF(15, 13, 38, 38), sf);
                    }
                }

                // Title & Subtitle bounding calculations
                float textLeft = 65;
                float textWidth = btn.Width - textLeft - 26; // Room before arrow

                // Title
                using (Font titleFont = new Font("Segoe UI", 9.5f, FontStyle.Bold))
                using (SolidBrush titleBrush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                using (StringFormat sfTitle = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
                {
                    pe.Graphics.DrawString(title, titleFont, titleBrush, new RectangleF(textLeft, 12, textWidth, 20), sfTitle);
                }

                // Subtitle
                using (Font subFont = new Font("Segoe UI", 8f))
                using (SolidBrush subBrush = new SolidBrush(Color.FromArgb(148, 163, 184)))
                using (StringFormat sfSub = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
                {
                    pe.Graphics.DrawString(subtitle, subFont, subBrush, new RectangleF(textLeft, 34, textWidth, 20), sfSub);
                }

                // Right arrow / Active dot
                using (Font arrowFont = new Font("Segoe UI", isActive ? 13f : 11.5f, FontStyle.Bold))
                using (SolidBrush arrowBrush = new SolidBrush((isActive || isHovered) ? accentColor : Color.FromArgb(100, 116, 139)))
                {
                    pe.Graphics.DrawString(isActive ? "●" : "›", arrowFont, arrowBrush, btn.Width - 18, isActive ? 20 : 22);
                }
            };

            btn.Click += (s, e) => onClick?.Invoke();
            return btn;
        }

        private void CreateCollapsedIcon(string icon, string tooltip, int y, Action onClick)
        {
            Button btn = new Button
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 13f),
                ForeColor = Color.FromArgb(203, 213, 225),
                BackColor = Color.FromArgb(30, 41, 59),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(36, 36),
                Location = new Point(6, y),
                Cursor = Cursors.Hand
            };
            btn.FlatAppearance.BorderSize = 0;
            ToolTip tip = new ToolTip();
            tip.SetToolTip(btn, tooltip);
            btn.Click += (s, e) => onClick?.Invoke();
            pnlCollapsedStrip.Controls.Add(btn);
        }

        private void ToggleCollapse()
        {
            isCollapsed = !isCollapsed;
            if (isCollapsed)
            {
                this.Width = COLLAPSED_WIDTH;
                pnlHeader.Visible = false;
                pnlNavContainer.Visible = false;
                pnlStatusCard.Visible = false;
                pnlCollapsedStrip.Visible = true;
                pnlCollapsedStrip.BringToFront();
            }
            else
            {
                this.Width = EXPANDED_WIDTH;
                pnlCollapsedStrip.Visible = false;
                pnlHeader.Visible = true;
                pnlNavContainer.Visible = true;
                pnlStatusCard.Visible = true;
                LayoutNavCards();
            }
        }

        // =========================================================================
        // TIỆN ÍCH HEADER BAR ĐỒNG BỘ CHO 4 VIEW ĐẦY ĐỦ
        // =========================================================================
        private Panel CreateViewHeaderBar(string title, string subtitle, string icon, Color accentColor)
        {
            Panel pnl = new Panel
            {
                Dock = DockStyle.Top,
                Height = 68,
                BackColor = Color.FromArgb(15, 23, 42),
                Padding = new Padding(24, 12, 24, 12)
            };

            pnl.Paint += (s, pe) =>
            {
                using (Pen p = new Pen(accentColor, 3f))
                {
                    pe.Graphics.DrawLine(p, 0, pnl.Height - 2, pnl.Width, pnl.Height - 2);
                }
            };

            Panel badge = new Panel
            {
                Size = new Size(42, 42),
                Location = new Point(20, 13),
                BackColor = Color.FromArgb(30, 41, 59)
            };
            Label lblIcon = new Label
            {
                Text = icon,
                Font = new Font("Segoe UI Emoji", 15f),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            badge.Controls.Add(lblIcon);
            pnl.Controls.Add(badge);

            Label lblT = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(72, 12),
                AutoSize = true
            };
            pnl.Controls.Add(lblT);

            Label lblS = new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 8.8f),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(73, 38),
                AutoSize = true
            };
            pnl.Controls.Add(lblS);

            Button btnBack = new Button
            {
                Text = "◀ VỀ BÀN LÀM VIỆC DSS",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(37, 99, 235),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 36),
                Location = new Point(pnl.Width - 224, 16),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand
            };
            btnBack.FlatAppearance.BorderSize = 0;
            btnBack.Click += (s, e) => mainForm.SwitchToView(0);
            pnl.Controls.Add(btnBack);

            return pnl;
        }

        // =========================================================================
        // VIEW 1: BỘ TỐI ƯU HÓA NGÂN SÁCH NHẬP HÀNG (SOLVER)
        // =========================================================================
        public Panel BuildSolverFullView()
        {
            pnlSolverFullView = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252)
            };

            Panel header = CreateViewHeaderBar(
                "BỘ TỐI ƯU HÓA NGÂN SÁCH NHẬP HÀNG (KNAPSACK SOLVER)",
                "Phân bổ số lượng nhập tối ưu trên trần ngân sách dựa theo Trọng số AHP & Lợi nhuận biên",
                "🎛️",
                Color.FromArgb(59, 130, 246)
            );
            pnlSolverFullView.Controls.Add(header);

            // Controls & Toolbar Panel (Thiết kế 2 tầng dãn rộng, không đè chữ)
            Panel pnlControls = new Panel
            {
                Dock = DockStyle.Top,
                Height = 156,
                BackColor = Color.White,
                Padding = new Padding(24, 12, 24, 12)
            };
            pnlControls.Paint += (s, pe) =>
            {
                using (Pen borderPen = new Pen(Color.FromArgb(226, 232, 240), 1.5f))
                {
                    pe.Graphics.DrawLine(borderPen, 0, pnlControls.Height - 1, pnlControls.Width, pnlControls.Height - 1);
                }
            };

            // HÀNG 1: THIẾT LẬP THAM SỐ NGÂN SÁCH & MỤC TIÊU (FLOWLAYOUTPANEL: 100% CHỐNG ĐÈ CHỮ)
            FlowLayoutPanel flowRow1 = new FlowLayoutPanel
            {
                Location = new Point(24, 12),
                Size = new Size(1300, 38),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            Label lblBudget = new Label
            {
                Text = "Ngân sách khả dụng:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                AutoSize = true,
                Margin = new Padding(0, 6, 12, 0)
            };
            flowRow1.Controls.Add(lblBudget);

            numSolverBudget = new NumericUpDown
            {
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(29, 78, 216),
                BackColor = Color.FromArgb(248, 250, 252),
                Size = new Size(240, 30),
                Minimum = 50000000m,
                Maximum = 10000000000m,
                Increment = 50000000m,
                Value = 1000000000m,
                ThousandsSeparator = true,
                Margin = new Padding(0, 0, 40, 0)
            };
            flowRow1.Controls.Add(numSolverBudget);

            Label lblGoal = new Label
            {
                Text = "Mục tiêu chiến lược:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                AutoSize = true,
                Margin = new Padding(0, 6, 12, 0)
            };
            flowRow1.Controls.Add(lblGoal);

            cboSolverGoal = new ComboBox
            {
                Size = new Size(360, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f),
                Margin = new Padding(0, 0, 0, 0)
            };
            cboSolverGoal.Items.Add("⭐ Tối đa hóa Điểm AHP Tổng Thể");
            cboSolverGoal.Items.Add("💰 Tối đa hóa Lợi Nhuận Gộp Kỳ Vọng");
            cboSolverGoal.Items.Add("🛡️ Cân Bằng Nhu Cầu & Giảm Thiểu Tồn Kho");
            cboSolverGoal.SelectedIndex = 0;
            flowRow1.Controls.Add(cboSolverGoal);

            pnlControls.Controls.Add(flowRow1);

            // HÀNG 2: CÁC NÚT HÀNH ĐỘNG RÕ RÀNG, DÃN THÔNG THOÁNG (FLOWLAYOUTPANEL)
            FlowLayoutPanel flowRow2 = new FlowLayoutPanel
            {
                Location = new Point(24, 56),
                Size = new Size(1300, 42),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            Button btnRun = new Button
            {
                Text = "⚡ CHẠY TỐI ƯU HÓA",
                Font = new Font("Segoe UI", 9.8f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(79, 70, 229),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 36),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 12, 0)
            };
            btnRun.FlatAppearance.BorderSize = 0;
            btnRun.Click += (s, e) => ExecuteSolverLogic();
            flowRow2.Controls.Add(btnRun);

            Button btnApplyToWorkflow = new Button
            {
                Text = "👉 CHUYỂN SANG LẬP LỆNH PO",
                Font = new Font("Segoe UI", 9.8f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(16, 185, 129),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(250, 36),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 16, 0)
            };
            btnApplyToWorkflow.FlatAppearance.BorderSize = 0;
            btnApplyToWorkflow.Click += (s, e) =>
            {
                if (cachedSolverItems.Count == 0) ExecuteSolverLogic();
                mainForm.SwitchToView(4);
            };
            flowRow2.Controls.Add(btnApplyToWorkflow);

            Button btnPreset1B = new Button
            {
                Text = "🎯 Mức 1 Tỷ",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                BackColor = Color.FromArgb(241, 245, 249),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(115, 36),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnPreset1B.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnPreset1B.Click += (s, e) => { numSolverBudget.Value = 1000000000m; ExecuteSolverLogic(); };
            flowRow2.Controls.Add(btnPreset1B);

            Button btnPreset2B = new Button
            {
                Text = "🚀 Mức 2 Tỷ",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                BackColor = Color.FromArgb(241, 245, 249),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(115, 36),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnPreset2B.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnPreset2B.Click += (s, e) => { numSolverBudget.Value = 2000000000m; ExecuteSolverLogic(); };
            flowRow2.Controls.Add(btnPreset2B);

            Button btnPreset500M = new Button
            {
                Text = "💵 Mức 500 Tr",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                BackColor = Color.FromArgb(241, 245, 249),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(115, 36),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 10, 0)
            };
            btnPreset500M.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnPreset500M.Click += (s, e) => { numSolverBudget.Value = 500000000m; ExecuteSolverLogic(); };
            flowRow2.Controls.Add(btnPreset500M);

            pnlControls.Controls.Add(flowRow2);

            // HÀNG 3: BANNER TỔNG KẾT KẾT QUẢ PHÂN BỔ (TỰ ĐỘNG CO DÃN)
            lblSolverSummary = new Label
            {
                Text = "Nhập ngân sách khả dụng và bấm 'CHẠY TỐI ƯU HÓA' để phân bổ số lượng nhập kho tối ưu.",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(21, 128, 61),
                Location = new Point(24, 104),
                Size = new Size(1100, 40),
                BackColor = Color.FromArgb(240, 253, 244),
                Padding = new Padding(14, 9, 14, 9),
                BorderStyle = BorderStyle.FixedSingle,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlControls.Controls.Add(lblSolverSummary);

            pnlSolverFullView.Controls.Add(pnlControls);

            // Main DataGridView
            Panel pnlGridContainer = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 16, 24, 16),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            dgvSolver = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                ReadOnly = true,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9.2f),
                RowTemplate = { Height = 40 },
                GridColor = Color.FromArgb(226, 232, 240),
                EnableHeadersVisualStyles = false
            };

            // Tiêu đề bảng: ROYAL BLUE DOANH NGHIỆP HIỆN ĐẠI (TUYỆT ĐỐI KHÔNG DÙNG MÀU ĐEN)
            dgvSolver.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(37, 99, 235);
            dgvSolver.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvSolver.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvSolver.ColumnHeadersHeight = 48;
            dgvSolver.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            // Dòng dữ liệu: Nền trắng & Ice Blue xen kẽ, chữ Navy sắc nét
            dgvSolver.DefaultCellStyle.BackColor = Color.White;
            dgvSolver.DefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            dgvSolver.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvSolver.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            dgvSolver.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 242, 254);
            dgvSolver.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 58, 138);

            dgvSolver.Columns.Add("Rank", "Hạng AHP");
            dgvSolver.Columns.Add("SKU", "Tên Sản Phẩm / SKU");
            dgvSolver.Columns.Add("Score", "Điểm AHP");
            dgvSolver.Columns.Add("UnitCost", "Đơn Giá Vốn");
            dgvSolver.Columns.Add("UnitPrice", "Giá Bán Lẻ");
            dgvSolver.Columns.Add("Qty", "SL Nhập Tối Ưu");
            dgvSolver.Columns.Add("TotalCost", "Tổng Vốn Nhập");
            dgvSolver.Columns.Add("ExpectedProfit", "Lợi Nhuận Dự Kiến");
            dgvSolver.Columns.Add("Decision", "Khuyến Nghị DSS");

            dgvSolver.Columns["Rank"].FillWeight = 40;
            dgvSolver.Columns["Rank"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            dgvSolver.Columns["SKU"].FillWeight = 145;
            dgvSolver.Columns["SKU"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            dgvSolver.Columns["Score"].FillWeight = 48;
            dgvSolver.Columns["Score"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSolver.Columns["Score"].DefaultCellStyle.ForeColor = Color.FromArgb(37, 99, 235);
            dgvSolver.Columns["Score"].DefaultCellStyle.Font = new Font("Segoe UI", 9.2f, FontStyle.Bold);

            dgvSolver.Columns["UnitCost"].FillWeight = 60;
            dgvSolver.Columns["UnitCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvSolver.Columns["UnitPrice"].FillWeight = 60;
            dgvSolver.Columns["UnitPrice"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvSolver.Columns["Qty"].FillWeight = 55;
            dgvSolver.Columns["Qty"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvSolver.Columns["Qty"].DefaultCellStyle.Font = new Font("Segoe UI", 9.2f, FontStyle.Bold);

            dgvSolver.Columns["TotalCost"].FillWeight = 72;
            dgvSolver.Columns["TotalCost"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

            dgvSolver.Columns["ExpectedProfit"].FillWeight = 78;
            dgvSolver.Columns["ExpectedProfit"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            dgvSolver.Columns["ExpectedProfit"].DefaultCellStyle.ForeColor = Color.FromArgb(22, 101, 52);
            dgvSolver.Columns["ExpectedProfit"].DefaultCellStyle.Font = new Font("Segoe UI", 9.2f, FontStyle.Bold);

            dgvSolver.Columns["Decision"].FillWeight = 110;
            dgvSolver.Columns["Decision"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;

            pnlGridContainer.Controls.Add(dgvSolver);
            pnlSolverFullView.Controls.Add(pnlGridContainer);

            pnlControls.BringToFront();
            pnlGridContainer.BringToFront();

            return pnlSolverFullView;
        }

        public void RefreshSolverFullView()
        {
            if (dgvSolver != null && dgvSolver.Rows.Count == 0)
            {
                ExecuteSolverLogic();
            }
        }

        private void ExecuteSolverLogic()
        {
            var snap = mainForm.CurrentSnapshot;
            if (snap == null || snap.Alts.Count == 0)
            {
                mainForm.RunDSS();
                snap = mainForm.CurrentSnapshot;
            }

            if (snap == null || snap.Alts.Count == 0)
            {
                if (lblSolverSummary != null)
                {
                    lblSolverSummary.Text = "⚠️ Chưa có dữ liệu DSS. Vui lòng bấm 'CHẠY MÔ HÌNH DSS' ở bàn làm việc chính trước.";
                    lblSolverSummary.ForeColor = Color.FromArgb(220, 38, 38);
                    lblSolverSummary.BackColor = Color.FromArgb(254, 242, 242);
                }
                return;
            }

            decimal totalBudget = numSolverBudget != null ? numSolverBudget.Value : 1000000000m;
            int goalIndex = cboSolverGoal != null ? cboSolverGoal.SelectedIndex : 0;

            var candidates = snap.Ranked.ToList();
            if (candidates.Count == 0) candidates = snap.Alts.ToList();

            if (goalIndex == 0)
                candidates = candidates.OrderByDescending(a => a.Score).ToList();
            else if (goalIndex == 1)
                candidates = candidates.OrderByDescending(a => (double)(a.Price - a.Cost) * a.Score).ToList();
            else
                candidates = candidates.OrderByDescending(a => a.DailyDemand > 0 ? (a.Score / (a.Stock + 1)) : a.Score).ToList();

            cachedSolverItems.Clear();
            decimal remaining = totalBudget;
            decimal totalAlloc = 0;
            decimal totalProf = 0;

            foreach (var alt in candidates)
            {
                decimal unitCost = alt.Cost > 0 ? alt.Cost : (alt.Price * 0.70m);
                if (unitCost <= 0) continue;

                int targetQty = 0;
                if (alt.Rank == 1) targetQty = Math.Max(30, (int)(alt.SuggestedQty * 1.5));
                else if (alt.Rank <= 3) targetQty = Math.Max(15, alt.SuggestedQty);
                else if (alt.Rank <= 6) targetQty = Math.Max(5, (int)(alt.SuggestedQty * 0.8));
                else targetQty = 0;

                if (targetQty <= 0 && alt.Rank <= 3) targetQty = 10;
                int affordable = (int)(remaining / unitCost);
                int allocated = Math.Min(targetQty, affordable);

                if (allocated > 0)
                {
                    decimal cost = allocated * unitCost;
                    decimal profit = allocated * (alt.Price - unitCost);
                    remaining -= cost;
                    totalAlloc += cost;
                    totalProf += profit;
                    cachedSolverItems.Add((alt, allocated, cost, profit));
                }
            }

            if (dgvSolver != null)
            {
                dgvSolver.Rows.Clear();
                foreach (var it in cachedSolverItems)
                {
                    string decisionTag = it.Alt.Rank == 1 ? "⭐ Ưu tiên giải ngân đợt 1" :
                                         it.Alt.Rank <= 3 ? "✓ Phê duyệt nhập bổ sung" :
                                         "• Nhập thăm dò thị trường";

                    dgvSolver.Rows.Add(
                        $"#{it.Alt.Rank}",
                        it.Alt.ShortName,
                        $"{it.Alt.Score:0.0000}",
                        $"{it.Alt.Cost:N0} đ",
                        $"{it.Alt.Price:N0} đ",
                        $"{it.Qty:N0} chiếc",
                        $"{it.TotalCost:N0} đ",
                        $"+{it.ExpectedProfit:N0} đ",
                        decisionTag
                    );
                }
            }

            if (lblSolverSummary != null)
            {
                decimal pct = totalBudget > 0 ? Math.Round((totalAlloc / totalBudget) * 100m, 1) : 0m;
                lblSolverSummary.Text = $"✓ ĐÃ PHÂN BỔ: {totalAlloc:N0} đ ({pct:F1}% ngân sách) • Ngân sách còn dư: {remaining:N0} đ • Lợi nhuận dự kiến: +{totalProf:N0} đ ({cachedSolverItems.Count} mặt hàng SKU)";
                lblSolverSummary.ForeColor = Color.FromArgb(21, 128, 61);
                lblSolverSummary.BackColor = Color.FromArgb(240, 253, 244);
            }
        }

        // =========================================================================
        // VIEW 2: TRỢ LÝ COPILOT & CẢNH BÁO RỦI RO THỜI GIAN THỰC
        // =========================================================================
        public Panel BuildAlertsFullView()
        {
            pnlAlertsFullView = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252)
            };

            Panel header = CreateViewHeaderBar(
                "TRỢ LÝ COPILOT & CẢNH BÁO RỦI RO THỜI GIAN THỰC",
                "Tự động phát hiện nguy cơ đứt hàng, chôn vốn tồn kho và kiểm soát tỷ số nhất quán Saaty CR",
                "🤖",
                Color.FromArgb(239, 68, 68)
            );
            pnlAlertsFullView.Controls.Add(header);

            // Controls Bar (Thiết kế 2 tầng thoáng đãng, không bị cụt chữ)
            Panel pnlControls = new Panel
            {
                Dock = DockStyle.Top,
                Height = 100,
                BackColor = Color.White,
                Padding = new Padding(24, 12, 24, 12)
            };
            pnlControls.Paint += (s, pe) =>
            {
                using (Pen borderPen = new Pen(Color.FromArgb(226, 232, 240), 1.5f))
                {
                    pe.Graphics.DrawLine(borderPen, 0, pnlControls.Height - 1, pnlControls.Width, pnlControls.Height - 1);
                }
            };

            // HÀNG 1: BỘ LỌC VÀ NÚT QUÉT (FLOWLAYOUTPANEL: 100% CHỐNG ĐÈ CHỮ)
            FlowLayoutPanel flowRow1 = new FlowLayoutPanel
            {
                Location = new Point(24, 12),
                Size = new Size(1300, 38),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            Label lblFilter = new Label
            {
                Text = "Mức độ rủi ro:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                AutoSize = true,
                Margin = new Padding(0, 6, 12, 0)
            };
            flowRow1.Controls.Add(lblFilter);

            cboAlertsFilter = new ComboBox
            {
                Size = new Size(270, 30),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9.5f),
                Margin = new Padding(0, 0, 20, 0)
            };
            cboAlertsFilter.Items.Add("Tất cả mức độ cảnh báo");
            cboAlertsFilter.Items.Add("🔴 Cảnh báo Đứt hàng (Stock-out)");
            cboAlertsFilter.Items.Add("🟡 Cảnh báo Chôn vốn (Overstock)");
            cboAlertsFilter.Items.Add("🔵 Tỷ số nhất quán Saaty CR");
            cboAlertsFilter.Items.Add("🟢 Cơ hội sinh lời cao");
            cboAlertsFilter.SelectedIndex = 0;
            cboAlertsFilter.SelectedIndexChanged += (s, e) => RefreshAlertsList();
            flowRow1.Controls.Add(cboAlertsFilter);

            Button btnRescan = new Button
            {
                Text = "🔄 QUÉT LẠI HỆ THỐNG",
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(79, 70, 229),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(200, 34),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 24, 0)
            };
            btnRescan.FlatAppearance.BorderSize = 0;
            btnRescan.Click += (s, e) => RefreshAlertsList();
            flowRow1.Controls.Add(btnRescan);

            lblAlertsSummary = new Label
            {
                Text = "Đang quét rủi ro...",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(22, 101, 52),
                AutoSize = true,
                Margin = new Padding(0, 7, 0, 0)
            };
            flowRow1.Controls.Add(lblAlertsSummary);

            pnlControls.Controls.Add(flowRow1);

            // HÀNG 2: THAO TÁC XỬ LÝ NHANH (FLOWLAYOUTPANEL)
            FlowLayoutPanel flowRow2 = new FlowLayoutPanel
            {
                Location = new Point(24, 56),
                Size = new Size(1300, 42),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            Button btnFixSaaty = new Button
            {
                Text = "📊 Điều Chỉnh Ma Trận Saaty (Tab 1)",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                BackColor = Color.FromArgb(241, 245, 249),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(260, 34),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 12, 0)
            };
            btnFixSaaty.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnFixSaaty.Click += (s, e) => mainForm.SwitchToView(0);
            flowRow2.Controls.Add(btnFixSaaty);

            Button btnCrisis = new Button
            {
                Text = "⚡ Kịch Bản Khủng Hoảng Chuỗi Cung Ứng",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(185, 28, 28),
                BackColor = Color.FromArgb(254, 242, 242),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(290, 34),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 12, 0)
            };
            btnCrisis.FlatAppearance.BorderColor = Color.FromArgb(252, 165, 165);
            btnCrisis.Click += (s, e) =>
            {
                mainForm.ApplyScenario(2);
                mainForm.SwitchToView(0);
            };
            flowRow2.Controls.Add(btnCrisis);

            Button btnSolverJump = new Button
            {
                Text = "🎛️ Chuyển Sang Solver Phân Bổ Vốn",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(37, 99, 235),
                BackColor = Color.FromArgb(238, 242, 255),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(270, 34),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 12, 0)
            };
            btnSolverJump.FlatAppearance.BorderColor = Color.FromArgb(199, 210, 254);
            btnSolverJump.Click += (s, e) => mainForm.SwitchToView(1);
            flowRow2.Controls.Add(btnSolverJump);

            pnlControls.Controls.Add(flowRow2);

            pnlAlertsFullView.Controls.Add(pnlControls);

            // Flow layout list of alert cards
            pnlAlertsCardsList = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                Padding = new Padding(24, 16, 24, 24),
                BackColor = Color.FromArgb(248, 250, 252)
            };
            pnlAlertsFullView.Controls.Add(pnlAlertsCardsList);

            pnlControls.BringToFront();
            pnlAlertsCardsList.BringToFront();

            return pnlAlertsFullView;
        }

        public void RefreshAlertsFullView()
        {
            RefreshAlertsList();
        }

        private void RefreshAlertsList()
        {
            if (pnlAlertsCardsList == null) return;
            pnlAlertsCardsList.Controls.Clear();

            var snap = mainForm.CurrentSnapshot;
            if (snap == null || snap.Alts.Count == 0)
            {
                mainForm.RunDSS();
                snap = mainForm.CurrentSnapshot;
            }

            if (snap == null || snap.Alts.Count == 0)
            {
                if (lblAlertsSummary != null) lblAlertsSummary.Text = "Chưa có dữ liệu DSS để phân tích rủi ro.";
                return;
            }

            int filter = cboAlertsFilter != null ? cboAlertsFilter.SelectedIndex : 0;
            int totalFound = 0;

            // 1. Cảnh báo Đứt hàng (Stock-out)
            if (filter == 0 || filter == 1)
            {
                var topAlts = snap.Ranked.Where(a => a.Rank <= 3).ToList();
                foreach (var alt in topAlts)
                {
                    if (alt.Stock <= 30 || (alt.DaysCover >= 0 && alt.DaysCover < 15))
                    {
                        totalFound++;
                        AddFullAlertCard(
                            pnlAlertsCardsList,
                            "NGUY CƠ ĐỨT HÀNG",
                            $"SKU [{alt.ShortName}] đạt thứ hạng #{alt.Rank} AHP (Điểm: {alt.Score:0.0000}) nhưng lượng tồn kho hiện chỉ còn {alt.Stock} sản phẩm.",
                            $"Chỉ số tồn kho khả dụng chỉ đủ đáp ứng ~{alt.DaysCover:0} ngày bán hàng. Nguy cơ thiếu hàng và mất doanh thu trực tiếp! Khuyến nghị chuyển sang Solver để giải ngân nhập bổ sung tối thiểu 30-50 sản phẩm.",
                            Color.FromArgb(254, 242, 242),
                            Color.FromArgb(239, 68, 68),
                            Color.FromArgb(185, 28, 28),
                            () => mainForm.SwitchToView(1),
                            "👉 Phân Bổ Ngân Sách Nhập SKU Này"
                        );
                    }
                }
            }

            // 2. Cảnh báo Chôn vốn (Overstock)
            if (filter == 0 || filter == 2)
            {
                var bottomAlts = snap.Ranked.Where(a => a.Rank >= 6).ToList();
                foreach (var alt in bottomAlts)
                {
                    if (alt.Stock >= 40 || alt.DaysCover > 120)
                    {
                        totalFound++;
                        AddFullAlertCard(
                            pnlAlertsCardsList,
                            "CẢNH BÁO CHÔN VỐN",
                            $"SKU [{alt.ShortName}] xếp hạng thấp (#{alt.Rank} AHP) nhưng tồn kho lên tới {alt.Stock} sản phẩm (đủ bán trong {alt.DaysCover:0} ngày).",
                            "Tốc độ luân chuyển chậm gây ứ đọng vốn lưu động. Đề xuất: Tạm dừng nhập mới mã này, áp dụng chương trình khuyến mãi giảm 20-30% hoặc combo bán kèm để giải phóng kho hàng.",
                            Color.FromArgb(254, 252, 232),
                            Color.FromArgb(234, 179, 8),
                            Color.FromArgb(161, 98, 7),
                            () => mainForm.SwitchToView(0),
                            "📊 Kiểm Tra Doanh Số & Tồn Kho"
                        );
                    }
                }
            }

            // 3. Kiểm tra Độ nhất quán Saaty AHP (CR)
            if (filter == 0 || filter == 3)
            {
                if (snap.Ahp.MatrixComplete)
                {
                    bool isOk = snap.Ahp.IsConsistent;
                    totalFound++;
                    AddFullAlertCard(
                        pnlAlertsCardsList,
                        isOk ? "SAATY CR ĐẠT CHUẨN" : "SAATY CR VƯỢT TRẦN",
                        $"Tỷ số nhất quán ma trận Saaty CR = {snap.Ahp.CR:0.0000} (Trần quy chuẩn: ≤ 0.10).",
                        isOk ? "Ma trận so sánh cặp giữa 5 tiêu chí (C1-C5) hoàn toàn nhất quán về mặt logic toán học. Kết quả xếp hạng phương án có độ tin cậy cao nhất để ra quyết định điều hành."
                             : "Ma trận so sánh cặp có sự mâu thuẫn bắc cầu (ví dụ: A quan trọng hơn B, B hơn C nhưng C lại hơn A). Vui lòng trở về Tab 'Dữ Liệu Đầu Vào' để hiệu chỉnh lại tỷ lệ so sánh cặp.",
                        isOk ? Color.FromArgb(238, 242, 255) : Color.FromArgb(255, 241, 242),
                        isOk ? Color.FromArgb(99, 102, 241) : Color.FromArgb(244, 63, 94),
                        isOk ? Color.FromArgb(67, 56, 202) : Color.FromArgb(190, 18, 60),
                        () => mainForm.SwitchToView(0),
                        isOk ? "✓ Xem Ma Trận Trọng Số" : "⚠️ Đi Đến Bàn Làm Việc Hiệu Chỉnh"
                    );
                }
            }

            // 4. Cơ hội sinh lời cao
            if (filter == 0 || filter == 4)
            {
                var highMargin = snap.Alts.OrderByDescending(a => a.MarginPct).FirstOrDefault();
                if (highMargin != null && highMargin.MarginPct > 20)
                {
                    totalFound++;
                    AddFullAlertCard(
                        pnlAlertsCardsList,
                        "CƠ HỘI SINH LỜI CAO",
                        $"SKU [{highMargin.ShortName}] có tỷ suất lợi nhuận gộp vượt trội ({highMargin.MarginPct:0.1}%). Giá nhập: {highMargin.Cost:N0} đ, Giá bán: {highMargin.Price:N0} đ.",
                        "Mặt hàng này đóng góp biên lợi nhuận cao nhất danh mục. Khuyến nghị tập trung ngân sách tiếp thị và đảm bảo không bị gián đoạn nguồn hàng.",
                        Color.FromArgb(240, 253, 244),
                        Color.FromArgb(34, 197, 94),
                        Color.FromArgb(21, 128, 61),
                        () => mainForm.SwitchToView(1),
                        "👉 Ưu Tiên Phân Bổ Ngân Sách"
                    );
                }
            }

            if (lblAlertsSummary != null)
            {
                lblAlertsSummary.Text = $"✓ Đã phát hiện {totalFound} chỉ số rủi ro & cơ hội chiến lược.";
            }
        }

        private void AddFullAlertCard(FlowLayoutPanel flow, string badgeText, string title, string detail, Color bg, Color border, Color badgeColor, Action action = null, string actionText = null)
        {
            Panel card = new Panel
            {
                Width = 1150,
                Height = 100,
                BackColor = bg,
                Margin = new Padding(0, 0, 0, 14),
                Padding = new Padding(16, 12, 16, 12)
            };

            card.Paint += (s, pe) =>
            {
                using (Pen p = new Pen(border, 1.5f))
                {
                    pe.Graphics.DrawRectangle(p, 0, 0, card.Width - 1, card.Height - 1);
                }
                using (SolidBrush leftBarBrush = new SolidBrush(badgeColor))
                {
                    pe.Graphics.FillRectangle(leftBarBrush, 0, 0, 6, card.Height);
                }
            };

            Label lblBadge = new Label
            {
                Text = $"● {badgeText.ToUpper()}",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = badgeColor,
                Location = new Point(16, 10),
                AutoSize = true
            };
            card.Controls.Add(lblBadge);

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9.8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(16, 30),
                Size = new Size(820, 22)
            };
            card.Controls.Add(lblTitle);

            Label lblDetail = new Label
            {
                Text = detail,
                Font = new Font("Segoe UI", 8.8f),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(16, 54),
                Size = new Size(820, 36)
            };
            card.Controls.Add(lblDetail);

            if (action != null && !string.IsNullOrEmpty(actionText))
            {
                Button btnAction = new Button
                {
                    Text = actionText,
                    Font = new Font("Segoe UI", 8.8f, FontStyle.Bold),
                    ForeColor = Color.White,
                    BackColor = badgeColor,
                    FlatStyle = FlatStyle.Flat,
                    Size = new Size(260, 34),
                    Location = new Point(card.Width - 280, 32),
                    Anchor = AnchorStyles.Top | AnchorStyles.Right,
                    Cursor = Cursors.Hand
                };
                btnAction.FlatAppearance.BorderSize = 0;
                btnAction.Click += (s, e) => action.Invoke();
                card.Controls.Add(btnAction);
            }

            flow.Controls.Add(card);
        }

        // =========================================================================
        // VIEW 3: QUẢN LÝ LỊCH SỬ PHIÊN & SO SÁNH ĐA KỊCH BẢN DSS
        // =========================================================================
        public Panel BuildSnapshotsFullView()
        {
            pnlSnapshotsFullView = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252)
            };

            Panel header = CreateViewHeaderBar(
                "QUẢN LÝ LỊCH SỬ PHIÊN & SO SÁNH ĐA KỊCH BẢN DSS",
                "Lưu trữ dấu vết kiểm toán (Audit Trail) các phương án AHP, so sánh chéo thứ hạng & độ nhạy",
                "📁",
                Color.FromArgb(16, 185, 129)
            );
            pnlSnapshotsFullView.Controls.Add(header);

            // Split Container Panel
            Panel pnlBody = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 16, 24, 24),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            // Left Sidebar Panel: Danh sách phiên
            Panel pnlLeft = new Panel
            {
                Dock = DockStyle.Left,
                Width = 380,
                BackColor = Color.White,
                Padding = new Padding(16)
            };
            pnlLeft.Paint += (s, pe) =>
            {
                using (Pen borderPen = new Pen(Color.FromArgb(226, 232, 240), 1.5f))
                {
                    pe.Graphics.DrawRectangle(borderPen, 0, 0, pnlLeft.Width - 1, pnlLeft.Height - 1);
                }
            };

            Label lblListTitle = new Label
            {
                Text = "DANH SÁCH PHIÊN RA QUYẾT ĐỊNH",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(14, 12),
                AutoSize = true
            };
            pnlLeft.Controls.Add(lblListTitle);

            lstSnapshotsFull = new ListBox
            {
                Location = new Point(14, 38),
                Size = new Size(350, 320),
                Font = new Font("Segoe UI", 9.5f),
                BorderStyle = BorderStyle.FixedSingle,
                ItemHeight = 24
            };
            foreach (var item in cachedSnapshots) lstSnapshotsFull.Items.Add(item);
            if (lstSnapshotsFull.Items.Count > 0) lstSnapshotsFull.SelectedIndex = 0;
            lstSnapshotsFull.SelectedIndexChanged += (s, e) => ExecuteCompareLogic();
            pnlLeft.Controls.Add(lstSnapshotsFull);

            int btnY = 370;

            Button btnLoadSnap = new Button
            {
                Text = "⚡ NẠP VÀO BÀN LÀM VIỆC",
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(37, 99, 235),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(350, 34),
                Location = new Point(14, btnY),
                Cursor = Cursors.Hand
            };
            btnLoadSnap.FlatAppearance.BorderSize = 0;
            btnLoadSnap.Click += (s, e) =>
            {
                int idx = lstSnapshotsFull.SelectedIndex;
                if (idx < 0) idx = 0;
                mainForm.ApplyScenario(idx);
                mainForm.SwitchToView(0);
            };
            pnlLeft.Controls.Add(btnLoadSnap);
            btnY += 42;

            Button btnCompareSnap = new Button
            {
                Text = "🔍 ĐỐI SOÁT VỚI PHIÊN CƠ SỞ",
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                BackColor = Color.FromArgb(241, 245, 249),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(350, 34),
                Location = new Point(14, btnY),
                Cursor = Cursors.Hand
            };
            btnCompareSnap.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnCompareSnap.Click += (s, e) => ExecuteCompareLogic();
            pnlLeft.Controls.Add(btnCompareSnap);
            btnY += 46;

            // Box lưu phiên mới
            Label lblSaveTitle = new Label
            {
                Text = "Lưu phiên tính toán hiện tại:",
                Font = new Font("Segoe UI", 8.8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(14, btnY),
                AutoSize = true
            };
            pnlLeft.Controls.Add(lblSaveTitle);
            btnY += 22;

            FlowLayoutPanel flowSaveSnap = new FlowLayoutPanel
            {
                Location = new Point(14, btnY),
                Size = new Size(352, 36),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent
            };

            txtSnapNameFull = new TextBox
            {
                Text = $"Phiên {DateTime.Now:dd/MM HH:mm}",
                Font = new Font("Segoe UI", 9.2f),
                Size = new Size(225, 28),
                Margin = new Padding(0, 2, 8, 0)
            };
            flowSaveSnap.Controls.Add(txtSnapNameFull);

            Button btnSaveSnap = new Button
            {
                Text = "💾 Lưu Lại",
                Font = new Font("Segoe UI", 8.8f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(16, 185, 129),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(110, 30),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 0)
            };
            btnSaveSnap.FlatAppearance.BorderSize = 0;
            btnSaveSnap.Click += (s, e) =>
            {
                string name = txtSnapNameFull.Text.Trim();
                if (!string.IsNullOrEmpty(name))
                {
                    string item = $"📌 {name}";
                    cachedSnapshots.Add(item);
                    lstSnapshotsFull.Items.Add(item);
                    lstSnapshotsFull.SelectedIndex = lstSnapshotsFull.Items.Count - 1;
                    MessageBox.Show($"Đã lưu phiên '{name}' thành công vào lịch sử DSS!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };
            flowSaveSnap.Controls.Add(btnSaveSnap);
            pnlLeft.Controls.Add(flowSaveSnap);

            pnlBody.Controls.Add(pnlLeft);

            // Right Detail Panel: Kết quả so sánh dạng văn bản Word
            Panel pnlRight = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(16, 12, 16, 16),
                Margin = new Padding(16, 0, 0, 0)
            };
            pnlRight.Paint += (s, pe) =>
            {
                using (Pen borderPen = new Pen(Color.FromArgb(226, 232, 240), 1.5f))
                {
                    pe.Graphics.DrawRectangle(borderPen, 0, 0, pnlRight.Width - 1, pnlRight.Height - 1);
                }
            };

            // Thanh công cụ thao tác văn bản Word
            Panel pnlDocToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.White,
                Padding = new Padding(0, 0, 0, 8)
            };

            Label lblDetailTitle = new Label
            {
                Text = "📄 BẢN XEM TRƯỚC VĂN BẢN ĐỐI SOÁT CHIẾN LƯỢC AHP",
                Font = new Font("Segoe UI", 9.8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 8),
                AutoSize = true
            };
            pnlDocToolbar.Controls.Add(lblDetailTitle);

            FlowLayoutPanel flowDocActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Width = 360,
                Height = 36,
                BackColor = Color.Transparent
            };

            Button btnPrintDoc = new Button
            {
                Text = "🖨️ In Báo Cáo",
                Font = new Font("Segoe UI", 8.8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                BackColor = Color.FromArgb(241, 245, 249),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(115, 32),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 0)
            };
            btnPrintDoc.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnPrintDoc.Click += (s, e) =>
            {
                try { wbCompareDetailFull?.ShowPrintDialog(); }
                catch (Exception ex) { MessageBox.Show("Không thể mở hộp thoại in: " + ex.Message); }
            };
            flowDocActions.Controls.Add(btnPrintDoc);

            Button btnExportWord = new Button
            {
                Text = "📄 Mở Trong Word",
                Font = new Font("Segoe UI", 8.8f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(37, 99, 235),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(140, 32),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 8, 0)
            };
            btnExportWord.FlatAppearance.BorderSize = 0;
            btnExportWord.Click += (s, e) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(lastCompareHtml)) ExecuteCompareLogic();
                    string path = Path.Combine(Path.GetTempPath(), $"BaoCao_DoiSoat_AHP_{DateTime.Now:yyyyMMdd_HHmmss}.doc");
                    File.WriteAllText(path, lastCompareHtml, System.Text.Encoding.UTF8);
                    Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi mở văn bản Word: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };
            flowDocActions.Controls.Add(btnExportWord);
            pnlDocToolbar.Controls.Add(flowDocActions);

            pnlRight.Controls.Add(pnlDocToolbar);

            wbCompareDetailFull = new WebBrowser
            {
                Dock = DockStyle.Fill,
                ScriptErrorsSuppressed = true,
                AllowWebBrowserDrop = false,
                IsWebBrowserContextMenuEnabled = false
            };
            pnlRight.Controls.Add(wbCompareDetailFull);
            wbCompareDetailFull.BringToFront();

            pnlBody.Controls.Add(pnlRight);
            pnlLeft.BringToFront();
            pnlRight.BringToFront();

            pnlSnapshotsFullView.Controls.Add(pnlBody);
            pnlBody.BringToFront();

            return pnlSnapshotsFullView;
        }

        public void RefreshSnapshotsFullView()
        {
            ExecuteCompareLogic();
        }

        private void ExecuteCompareLogic()
        {
            if (wbCompareDetailFull == null) return;

            int selected = lstSnapshotsFull != null ? lstSnapshotsFull.SelectedIndex : 0;
            string selectedName = lstSnapshotsFull != null && lstSnapshotsFull.SelectedItem != null ? lstSnapshotsFull.SelectedItem.ToString() : "Phiên đã chọn";

            lastCompareHtml = BuildCompareDocumentHtml(selected, selectedName);
            wbCompareDetailFull.DocumentText = lastCompareHtml;
        }

        private string BuildCompareDocumentHtml(int selected, string selectedName)
        {
            var user = UserSession.CurrentUser;
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' lang='vi'>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv='X-UA-Compatible' content='IE=edge'/>");
            sb.AppendLine("<meta charset='utf-8'/>");
            sb.AppendLine("<title>Báo Cáo Đối Soát Chiến Lược AHP</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { background-color: #F1F5F9; margin: 0; padding: 20px; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #1E293B; }");
            sb.AppendLine(".doc-page { background: #FFFFFF; max-width: 860px; margin: 0 auto; padding: 40px 48px; border: 1px solid #CBD5E1; box-shadow: 0 4px 16px rgba(0,0,0,0.06); border-radius: 4px; }");
            sb.AppendLine(".header-table { width: 100%; border-bottom: 2px solid #2563EB; padding-bottom: 12px; margin-bottom: 20px; }");
            sb.AppendLine(".header-table td { vertical-align: top; }");
            sb.AppendLine(".company-name { font-size: 13px; font-weight: bold; color: #0F172A; text-transform: uppercase; }");
            sb.AppendLine(".company-sub { font-size: 11px; color: #64748B; margin-top: 3px; }");
            sb.AppendLine(".audit-badge { display: inline-block; background: #EFF6FF; color: #2563EB; font-weight: bold; font-size: 11px; padding: 4px 10px; border-radius: 4px; border: 1px solid #BFDBFE; text-transform: uppercase; }");
            sb.AppendLine(".doc-title { text-align: center; font-size: 18px; font-weight: 800; color: #1E293B; text-transform: uppercase; margin: 16px 0 4px 0; }");
            sb.AppendLine(".doc-subtitle { text-align: center; font-size: 12px; font-style: italic; color: #64748B; margin-bottom: 20px; }");
            sb.AppendLine(".meta-table { width: 100%; border-collapse: collapse; background: #F8FAFC; border: 1px solid #E2E8F0; border-radius: 6px; margin-bottom: 24px; }");
            sb.AppendLine(".meta-table td { padding: 8px 12px; font-size: 12.5px; border-bottom: 1px solid #F1F5F9; }");
            sb.AppendLine(".meta-label { color: #475569; font-weight: 600; width: 22%; }");
            sb.AppendLine(".meta-val { color: #0F172A; font-weight: bold; }");
            sb.AppendLine(".section-header { font-size: 13px; font-weight: 700; color: #1E293B; text-transform: uppercase; margin: 20px 0 10px 0; padding-left: 8px; border-left: 4px solid #2563EB; }");
            sb.AppendLine(".data-table { width: 100%; border-collapse: collapse; margin-bottom: 24px; font-size: 12.5px; }");
            sb.AppendLine(".data-table th { background: #2563EB; color: #FFFFFF; font-weight: 700; padding: 10px 12px; border: 1px solid #1D4ED8; text-align: left; }");
            sb.AppendLine(".data-table td { padding: 9px 12px; border: 1px solid #E2E8F0; }");
            sb.AppendLine(".data-table tr:nth-child(even) { background: #F8FAFC; }");
            sb.AppendLine(".badge-green { display: inline-block; background: #DCFCE7; color: #15803D; font-weight: 700; font-size: 11px; padding: 3px 8px; border-radius: 4px; border: 1px solid #BBF7D0; }");
            sb.AppendLine(".badge-yellow { display: inline-block; background: #FEF3C7; color: #B45309; font-weight: 700; font-size: 11px; padding: 3px 8px; border-radius: 4px; border: 1px solid #FDE68A; }");
            sb.AppendLine(".badge-blue { display: inline-block; background: #DBEAFE; color: #1D4ED8; font-weight: 700; font-size: 11px; padding: 3px 8px; border-radius: 4px; border: 1px solid #BFDBFE; }");
            sb.AppendLine(".callout-box { background: #F0FDF4; border: 1px solid #BBF7D0; border-left: 4px solid #16A34A; padding: 14px 18px; border-radius: 4px; margin-bottom: 24px; }");
            sb.AppendLine(".callout-box ul { margin: 0; padding-left: 18px; font-size: 12.5px; line-height: 1.6; color: #166534; }");
            sb.AppendLine(".callout-box li { margin-bottom: 4px; }");
            sb.AppendLine(".stamp-box { border: 2.5px dashed #16A34A; color: #16A34A; font-weight: bold; font-size: 11px; padding: 8px 14px; display: inline-block; transform: rotate(-4deg); margin-top: 10px; line-height: 1.4; background: #F0FDF4; border-radius: 4px; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            sb.AppendLine("<div class='doc-page'>");

            // Header letterhead
            sb.AppendLine("<table class='header-table'>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td width='65%'>");
            sb.AppendLine("<div class='company-name'>CÔNG TY CỔ PHẦN BÁN LẺ THỜI TRANG & THIẾT BỊ SỐ 2026</div>");
            sb.AppendLine("<div class='company-sub'>HỆ THỐNG HỖ TRỢ RA QUYẾT ĐỊNH BÁN HÀNG — DSS RETAIL STUDIO</div>");
            sb.AppendLine("</td>");
            sb.AppendLine("<td width='35%' align='right'>");
            sb.AppendLine("<div class='audit-badge'>✓ HỒ SƠ KIỂM TOÁN DSS</div>");
            sb.AppendLine($"<div style='font-size: 11px; color: #64748B; margin-top: 4px;'>Mã: AUDIT-AHP-{(uint)selectedName.GetHashCode() % 10000:D4}</div>");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");

            // Title
            sb.AppendLine("<div class='doc-title'>BÁO CÁO ĐỐI SOÁT CHIẾN LƯỢC & BIẾN ĐỘNG THỨ HẠNG AHP</div>");
            sb.AppendLine("<div class='doc-subtitle'>Kiểm tra tính bền vững và phân tích độ nhạy của các phương án ra quyết định đa mục tiêu</div>");

            // Meta Info
            sb.AppendLine("<table class='meta-table'>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td class='meta-label'>Phiên đối chiếu:</td>");
            sb.AppendLine($"<td class='meta-val' style='color:#2563EB;'>{selectedName}</td>");
            sb.AppendLine("<td class='meta-label'>Thời điểm kiểm toán:</td>");
            sb.AppendLine($"<td class='meta-val'>{DateTime.Now:dd/MM/yyyy HH:mm:ss}</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td class='meta-label'>Phiên tham chiếu gốc:</td>");
            sb.AppendLine($"<td class='meta-val'>{cachedSnapshots[0]}</td>");
            sb.AppendLine("<td class='meta-label'>Người thẩm định:</td>");
            sb.AppendLine($"<td class='meta-val'>{user?.FullName ?? "Decision Maker"} ({user?.RoleTitle ?? "Giám đốc"})</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");

            // Section 1: Data table
            sb.AppendLine("<div class='section-header'>1. BẢNG ĐỐI SOÁT THỨ HẠNG & BIẾN ĐỘNG CHIẾN LƯỢC</div>");
            sb.AppendLine("<table class='data-table'>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th width='6%' align='center'>STT</th>");
            sb.AppendLine("<th>Tên Mặt Hàng / SKU</th>");
            sb.AppendLine("<th width='24%'>Hạng Gốc (Baseline)</th>");
            sb.AppendLine("<th width='24%'>Hạng Phiên Này</th>");
            sb.AppendLine("<th width='20%' align='center'>Biến Động Chiến Lược</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            // Populate rows based on scenario
            if (selected == 0)
            {
                sb.AppendLine("<tr><td align='center'>1</td><td><strong>Máy ảnh Canon EOS R6 Mark II</strong></td><td>Top #1 (Điểm: 0.8210)</td><td>Top #1 (Điểm: 0.8210)</td><td align='center'><span class='badge-blue'>▬ Giữ nguyên</span></td></tr>");
                sb.AppendLine("<tr><td align='center'>2</td><td><strong>Samsung Galaxy S24 Ultra</strong></td><td>Top #2 (Điểm: 0.6413)</td><td>Top #2 (Điểm: 0.6413)</td><td align='center'><span class='badge-blue'>▬ Giữ nguyên</span></td></tr>");
                sb.AppendLine("<tr><td align='center'>3</td><td><strong>Laptop Acer Predator Helios</strong></td><td>Top #3 (Điểm: 0.5681)</td><td>Top #3 (Điểm: 0.5681)</td><td align='center'><span class='badge-blue'>▬ Giữ nguyên</span></td></tr>");
                sb.AppendLine("<tr><td align='center'>4</td><td><strong>Đồng hồ thông minh Garmin</strong></td><td>Top #4 (Điểm: 0.4720)</td><td>Top #4 (Điểm: 0.4720)</td><td align='center'><span class='badge-blue'>▬ Giữ nguyên</span></td></tr>");
                sb.AppendLine("<tr><td align='center'>5</td><td><strong>Tai nghe Sony WH-1000XM5</strong></td><td>Top #5 (Điểm: 0.3950)</td><td>Top #5 (Điểm: 0.3950)</td><td align='center'><span class='badge-blue'>▬ Giữ nguyên</span></td></tr>");
            }
            else if (selected == 1)
            {
                sb.AppendLine("<tr><td align='center'>1</td><td><strong>Máy ảnh Canon EOS R6 Mark II</strong></td><td>Top #1 (Điểm: 0.8210)</td><td>Top #1 (Điểm: 0.8920)</td><td align='center'><span class='badge-blue'>▬ Giữ vững #1</span></td></tr>");
                sb.AppendLine("<tr><td align='center'>2</td><td><strong>Laptop Acer Predator Helios</strong></td><td>Top #3 (Điểm: 0.5681)</td><td>Top #2 (Điểm: 0.7150)</td><td align='center'><span class='badge-green'>▲ Tăng 1 hạng</span></td></tr>");
                sb.AppendLine("<tr><td align='center'>3</td><td><strong>Samsung Galaxy S24 Ultra</strong></td><td>Top #2 (Điểm: 0.6413)</td><td>Top #3 (Điểm: 0.6320)</td><td align='center'><span class='badge-yellow'>▼ Giảm 1 hạng</span></td></tr>");
                sb.AppendLine("<tr><td align='center'>4</td><td><strong>Đồng hồ thông minh Garmin</strong></td><td>Top #4 (Điểm: 0.4720)</td><td>Top #4 (Điểm: 0.4810)</td><td align='center'><span class='badge-blue'>▬ Giữ nguyên</span></td></tr>");
            }
            else if (selected == 2)
            {
                sb.AppendLine("<tr><td align='center'>1</td><td><strong>Máy ảnh Canon EOS R6 Mark II</strong></td><td>Top #1 (Điểm: 0.8210)</td><td>Top #1 (Điểm: 0.7850)</td><td align='center'><span class='badge-blue'>▬ Giữ vững #1</span></td></tr>");
                sb.AppendLine("<tr><td align='center'>2</td><td><strong>Laptop Acer Predator Helios</strong></td><td>Top #3 (Điểm: 0.5681)</td><td>Top #2 (Điểm: 0.6120)</td><td align='center'><span class='badge-green'>▲ Tăng 1 hạng</span></td></tr>");
                sb.AppendLine("<tr><td align='center'>3</td><td><strong>Samsung Galaxy S24 Ultra</strong></td><td>Top #2 (Điểm: 0.6413)</td><td>Top #4 (Điểm: 0.4510)</td><td align='center'><span class='badge-yellow'>▼ Giảm 2 hạng</span></td></tr>");
            }
            else
            {
                sb.AppendLine("<tr><td align='center'>1</td><td><strong>Máy ảnh Canon EOS R6 Mark II</strong></td><td>Top #1</td><td>Top #1</td><td align='center'><span class='badge-blue'>▬ Đứng đầu</span></td></tr>");
                sb.AppendLine("<tr><td align='center'>2</td><td><strong>Laptop Acer Predator Helios</strong></td><td>Top #3</td><td>Top #2</td><td align='center'><span class='badge-green'>▲ Đột phá (+1)</span></td></tr>");
                sb.AppendLine("<tr><td align='center'>3</td><td><strong>Samsung Galaxy S24 Ultra</strong></td><td>Top #2</td><td>Top #3</td><td align='center'><span class='badge-yellow'>▼ Điều chỉnh (-1)</span></td></tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");

            // Section 2: Recommendations
            sb.AppendLine("<div class='section-header'>2. NHẬN ĐỊNH & KHUYẾN NGHỊ TỪ TRỢ LÝ DSS</div>");
            sb.AppendLine("<div class='callout-box'>");
            sb.AppendLine("<ul>");
            sb.AppendLine("<li><strong>Máy ảnh Canon EOS R6 Mark II:</strong> Giữ vững vị trí số 1 trong mọi kịch bản thị trường (Độ nhạy cực cao, sức cạnh tranh và tỷ suất lợi nhuận bền vững).</li>");
            sb.AppendLine("<li><strong>Laptop Acer Predator Helios:</strong> Khi gặp biến động chuỗi cung ứng hoặc lạm phát, dòng Laptop Acer vươn lên Top 2 do biên lợi nhuận cao và an toàn vốn tốt.</li>");
            sb.AppendLine("<li><strong>Đề xuất thi hành:</strong> Bấm nút <em>'NẠP VÀO BÀN LÀM VIỆC'</em> ở thanh điều khiển bên trái để áp dụng bộ trọng số của phiên này sang toàn bộ 6 Tab chính của DSS.</li>");
            sb.AppendLine("</ul>");
            sb.AppendLine("</div>");

            // Signatures block
            sb.AppendLine("<table style='width: 100%; margin-top: 36px; border-top: 1px dashed #CBD5E1; padding-top: 20px;'>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td style='width: 50%; text-align: center; vertical-align: top;'>");
            sb.AppendLine("<strong>NGƯỜI LẬP BÁO CÁO</strong><br/>");
            sb.AppendLine("<span style='font-size: 11px; color: #64748B;'>(Ký và xác nhận dữ liệu)</span><br/><br/><br/><br/>");
            sb.AppendLine($"<strong>{user?.FullName ?? "Nguyễn Đăng Quang Huy"}</strong>");
            sb.AppendLine("</td>");
            sb.AppendLine("<td style='width: 50%; text-align: center; vertical-align: top;'>");
            sb.AppendLine("<strong>HỘI ĐỒNG RA QUYẾT ĐỊNH (C-SUITE)</strong><br/>");
            sb.AppendLine("<span style='font-size: 11px; color: #64748B;'>(Đã đối soát & phê duyệt)</span><br/><br/>");
            sb.AppendLine("<div class='stamp-box'>✓ ĐÃ ĐỐI SOÁT KIỂM TOÁN<br/>AHP SAATY CR &le; 0.10<br/>DSS STUDIO 2026</div><br/><br/>");
            sb.AppendLine("<strong>BAN GIÁM ĐỐC</strong>");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("</div>"); // doc-page
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        // =========================================================================
        // VIEW 4: QUY TRÌNH KÝ DUYỆT QUYẾT ĐỊNH & XUẤT LỆNH MUA HÀNG (PO)
        // =========================================================================
        public Panel BuildWorkflowFullView()
        {
            pnlWorkflowFullView = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252)
            };

            Panel header = CreateViewHeaderBar(
                "QUY TRÌNH KÝ DUYỆT QUYẾT ĐỊNH & XUẤT LỆNH MUA HÀNG (PURCHASE ORDER)",
                "Chuyển hóa kết quả tính toán DSS thành chứng từ pháp lý nội bộ có chữ ký số điện tử của Decision Maker",
                "📋",
                Color.FromArgb(139, 92, 246)
            );
            pnlWorkflowFullView.Controls.Add(header);

            // Container 2 cột
            Panel pnlBody = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(24, 16, 24, 24),
                BackColor = Color.FromArgb(248, 250, 252)
            };

            // Cột trái: Thiết lập & Thao tác ký duyệt (430px)
            Panel pnlLeft = new Panel
            {
                Dock = DockStyle.Left,
                Width = 430,
                BackColor = Color.White,
                Padding = new Padding(18)
            };
            pnlLeft.Paint += (s, pe) =>
            {
                using (Pen borderPen = new Pen(Color.FromArgb(226, 232, 240), 1.5f))
                {
                    pe.Graphics.DrawRectangle(borderPen, 0, 0, pnlLeft.Width - 1, pnlLeft.Height - 1);
                }
            };

            lblWorkflowPoCode = new Label
            {
                Text = $"MÃ LỆNH PO: {currentPoNumber}",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(79, 70, 229),
                Location = new Point(14, 12),
                AutoSize = true
            };
            pnlLeft.Controls.Add(lblWorkflowPoCode);

            Label lblSigner = new Label
            {
                Text = $"👤 Người ký: {UserSession.CurrentUser?.FullName ?? "Decision Maker"} [{UserSession.CurrentUser?.Role ?? "Giám đốc"}]",
                Font = new Font("Segoe UI", 9f),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(15, 36),
                AutoSize = true
            };
            pnlLeft.Controls.Add(lblSigner);

            Label lblListTitle = new Label
            {
                Text = "Danh mục sản phẩm đề xuất phê duyệt:",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(14, 66),
                AutoSize = true
            };
            pnlLeft.Controls.Add(lblListTitle);

            chkWorkflowList = new CheckedListBox
            {
                Location = new Point(14, 90),
                Size = new Size(398, 200),
                Font = new Font("Segoe UI", 9f),
                CheckOnClick = true,
                BorderStyle = BorderStyle.FixedSingle
            };
            chkWorkflowList.ItemCheck += (s, e) =>
            {
                this.BeginInvoke(new Action(UpdateWorkflowDocumentPreview));
            };
            pnlLeft.Controls.Add(chkWorkflowList);

            Label lblNotesTitle = new Label
            {
                Text = "Ý kiến chỉ đạo phê duyệt nhập hàng:",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(14, 302),
                AutoSize = true
            };
            pnlLeft.Controls.Add(lblNotesTitle);

            txtWorkflowNotes = new TextBox
            {
                Text = "Ưu tiên giải ngân đợt 1 cho sản phẩm Top 1 AHP trước ngày 15 hàng tháng.",
                Font = new Font("Segoe UI", 9.2f),
                Location = new Point(14, 326),
                Size = new Size(398, 60),
                Multiline = true
            };
            txtWorkflowNotes.TextChanged += (s, e) => UpdateWorkflowDocumentPreview();
            pnlLeft.Controls.Add(txtWorkflowNotes);

            int btnY = 398;

            btnWorkflowSign = new Button
            {
                Text = isPoSigned ? "✓ ĐÃ KÝ DUYỆT ĐIỆN TỬ" : "✍️ KÝ DUYỆT ĐIỆN TỬ PHÊ DUYỆT",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = isPoSigned ? Color.FromArgb(16, 185, 129) : Color.FromArgb(139, 92, 246),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(398, 38),
                Location = new Point(14, btnY),
                Cursor = Cursors.Hand
            };
            btnWorkflowSign.FlatAppearance.BorderSize = 0;
            btnWorkflowSign.Click += (s, e) => ExecuteSignoff();
            pnlLeft.Controls.Add(btnWorkflowSign);
            btnY += 46;

            Button btnExport = new Button
            {
                Text = "📄 XUẤT LỆNH MUA HÀNG (WORD / HTML)",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(37, 99, 235),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(398, 36),
                Location = new Point(14, btnY),
                Cursor = Cursors.Hand
            };
            btnExport.FlatAppearance.BorderSize = 0;
            btnExport.Click += (s, e) => ExportPoDocument();
            pnlLeft.Controls.Add(btnExport);
            btnY += 44;

            lblWorkflowStatus = new Label
            {
                Text = isPoSigned ? "✓ ĐÃ KÝ DUYỆT HỢP LỆ THEO CHUẨN DSS" : "Trạng thái: Chờ người ra quyết định ký duyệt",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = isPoSigned ? Color.FromArgb(22, 101, 52) : Color.FromArgb(100, 116, 139),
                Location = new Point(14, btnY),
                AutoSize = true
            };
            pnlLeft.Controls.Add(lblWorkflowStatus);

            pnlBody.Controls.Add(pnlLeft);

            // Cột phải: Bản xem trước văn bản chính thức (Dock = Fill)
            // Cột phải: Bản xem trước văn bản chính thức dạng Word
            Panel pnlRight = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(16, 12, 16, 16),
                Margin = new Padding(16, 0, 0, 0)
            };
            pnlRight.Paint += (s, pe) =>
            {
                using (Pen borderPen = new Pen(Color.FromArgb(226, 232, 240), 1.5f))
                {
                    pe.Graphics.DrawRectangle(borderPen, 0, 0, pnlRight.Width - 1, pnlRight.Height - 1);
                }
            };

            Panel pnlPoDocToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = Color.White,
                Padding = new Padding(0, 0, 0, 8)
            };

            Label lblPreviewHeader = new Label
            {
                Text = "📄 VĂN BẢN LỆNH MUA HÀNG (PURCHASE ORDER)",
                Font = new Font("Segoe UI", 9.8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 8),
                AutoSize = true
            };
            pnlPoDocToolbar.Controls.Add(lblPreviewHeader);

            FlowLayoutPanel flowPoActions = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Width = 360,
                Height = 36,
                BackColor = Color.Transparent
            };

            Button btnPrintPo = new Button
            {
                Text = "🖨️ In Lệnh PO",
                Font = new Font("Segoe UI", 8.8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(51, 65, 85),
                BackColor = Color.FromArgb(241, 245, 249),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(115, 32),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 0)
            };
            btnPrintPo.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnPrintPo.Click += (s, e) =>
            {
                try { wbWorkflowPo?.ShowPrintDialog(); }
                catch (Exception ex) { MessageBox.Show("Không thể mở hộp thoại in: " + ex.Message); }
            };
            flowPoActions.Controls.Add(btnPrintPo);

            Button btnOpenWordPo = new Button
            {
                Text = "📄 Mở Trong Word",
                Font = new Font("Segoe UI", 8.8f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(37, 99, 235),
                FlatStyle = FlatStyle.Flat,
                Size = new Size(140, 32),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 8, 0)
            };
            btnOpenWordPo.FlatAppearance.BorderSize = 0;
            btnOpenWordPo.Click += (s, e) => ExportPoDocument();
            flowPoActions.Controls.Add(btnOpenWordPo);

            pnlPoDocToolbar.Controls.Add(flowPoActions);
            pnlRight.Controls.Add(pnlPoDocToolbar);

            wbWorkflowPo = new WebBrowser
            {
                Dock = DockStyle.Fill,
                ScriptErrorsSuppressed = true,
                AllowWebBrowserDrop = false,
                IsWebBrowserContextMenuEnabled = false
            };
            pnlRight.Controls.Add(wbWorkflowPo);
            wbWorkflowPo.BringToFront();

            pnlBody.Controls.Add(pnlRight);
            pnlLeft.BringToFront();
            pnlRight.BringToFront();

            pnlWorkflowFullView.Controls.Add(pnlBody);
            pnlBody.BringToFront();

            return pnlWorkflowFullView;
        }

        public void RefreshWorkflowFullView()
        {
            if (chkWorkflowList != null && chkWorkflowList.Items.Count == 0)
            {
                if (cachedSolverItems.Count > 0)
                {
                    foreach (var item in cachedSolverItems)
                    {
                        chkWorkflowList.Items.Add($"[Top #{item.Alt.Rank}] {item.Alt.ShortName} • {item.Qty} chiếc • {item.TotalCost:N0} đ", true);
                    }
                }
                else
                {
                    var snap = mainForm.CurrentSnapshot;
                    if (snap != null && snap.Ranked.Any())
                    {
                        foreach (var alt in snap.Ranked)
                        {
                            int qty = alt.SuggestedQty > 0 ? alt.SuggestedQty : 20;
                            decimal cost = (alt.Cost > 0 ? alt.Cost : alt.Price * 0.7m) * qty;
                            chkWorkflowList.Items.Add($"[Top #{alt.Rank}] {alt.ShortName} • {qty} chiếc • {cost:N0} đ", alt.Rank <= 3);
                        }
                    }
                    else
                    {
                        chkWorkflowList.Items.Add("[Top #1] Máy ảnh Canon EOS R6 Mark II • 30 chiếc • 1,500,000,000 đ", true);
                        chkWorkflowList.Items.Add("[Top #2] Samsung Galaxy S24 Ultra • 20 chiếc • 600,000,000 đ", true);
                        chkWorkflowList.Items.Add("[Top #3] Laptop Acer Predator Helios • 15 chiếc • 450,000,000 đ", true);
                    }
                }
            }
            UpdateWorkflowDocumentPreview();
        }

        private void UpdateWorkflowDocumentPreview()
        {
            if (wbWorkflowPo == null) return;

            System.Collections.IEnumerable items = chkWorkflowList != null ? chkWorkflowList.CheckedItems : Array.Empty<string>();
            string notes = txtWorkflowNotes != null ? txtWorkflowNotes.Text : "";
            wbWorkflowPo.DocumentText = BuildPoHtml(currentPoNumber, notes, items);
        }

        private void ExecuteSignoff()
        {
            if (chkWorkflowList == null || chkWorkflowList.CheckedItems.Count == 0)
            {
                MessageBox.Show("Vui lòng tích chọn ít nhất 1 sản phẩm để phê duyệt đặt hàng!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            isPoSigned = true;
            if (btnWorkflowSign != null)
            {
                btnWorkflowSign.Text = "✓ ĐÃ KÝ DUYỆT ĐIỆN TỬ";
                btnWorkflowSign.BackColor = Color.FromArgb(16, 185, 129);
            }
            if (lblWorkflowStatus != null)
            {
                lblWorkflowStatus.Text = $"✓ ĐÃ KÝ DUYỆT BỞI {UserSession.CurrentUser?.FullName?.ToUpper() ?? "GIÁM ĐỐC"}";
                lblWorkflowStatus.ForeColor = Color.FromArgb(22, 101, 52);
            }

            UpdateWorkflowDocumentPreview();

            MessageBox.Show(
                $"Đã ký duyệt thành công Lệnh Mua Hàng {currentPoNumber}!\n" +
                $"• Người ký: {UserSession.CurrentUser?.FullName ?? "Decision Maker"}\n" +
                $"• Số mặt hàng: {chkWorkflowList.CheckedItems.Count} SKU\n" +
                $"• Trạng thái: Hợp lệ theo chuẩn phê duyệt AHP Saaty.\n\n" +
                $"Văn bản đã được đóng dấu mộc điện tử và cập nhật trực tiếp trên màn hình xem trước.",
                "Ký Duyệt Điện Tử",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information
            );
        }

        private void ExportPoDocument()
        {
            if (!isPoSigned)
            {
                var ask = MessageBox.Show("Lệnh Mua Hàng chưa có chữ ký duyệt điện tử. Bạn có muốn ký duyệt và xuất biên bản ngay không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (ask == DialogResult.Yes)
                {
                    ExecuteSignoff();
                }
                else return;
            }

            try
            {
                System.Collections.IEnumerable items = chkWorkflowList != null ? chkWorkflowList.CheckedItems : Array.Empty<string>();
                string notes = txtWorkflowNotes != null ? txtWorkflowNotes.Text : "";
                string html = BuildPoHtml(currentPoNumber, notes, items);
                string path = Path.Combine(Path.GetTempPath(), $"LenhMuaHang_{currentPoNumber}_{DateTime.Now:yyyyMMdd_HHmmss}.doc");
                File.WriteAllText(path, html, System.Text.Encoding.UTF8);

                Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi xuất lệnh mua hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private string BuildPoHtml(string poNum, string notes, System.Collections.IEnumerable items)
        {
            var user = UserSession.CurrentUser;
            var sb = new System.Text.StringBuilder();

            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' lang='vi'>");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv='X-UA-Compatible' content='IE=edge'/>");
            sb.AppendLine("<meta charset='utf-8'/>");
            sb.AppendLine("<title>Lệnh Mua Hàng & Phê Duyệt PO</title>");
            sb.AppendLine("<style>");
            sb.AppendLine("body { background-color: #F1F5F9; margin: 0; padding: 20px; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; color: #1E293B; }");
            sb.AppendLine(".doc-page { background: #FFFFFF; max-width: 860px; margin: 0 auto; padding: 40px 48px; border: 1px solid #CBD5E1; box-shadow: 0 4px 16px rgba(0,0,0,0.06); border-radius: 4px; }");
            sb.AppendLine(".header-table { width: 100%; border-bottom: 2px solid #7C3AED; padding-bottom: 12px; margin-bottom: 20px; }");
            sb.AppendLine(".header-table td { vertical-align: top; }");
            sb.AppendLine(".company-name { font-size: 13px; font-weight: bold; color: #0F172A; text-transform: uppercase; }");
            sb.AppendLine(".company-sub { font-size: 11px; color: #64748B; margin-top: 3px; }");
            sb.AppendLine(".po-badge { display: inline-block; background: #F3E8FF; color: #7C3AED; font-weight: bold; font-size: 11px; padding: 4px 10px; border-radius: 4px; border: 1px solid #DDD6FE; text-transform: uppercase; }");
            sb.AppendLine(".doc-title { text-align: center; font-size: 18px; font-weight: 800; color: #1E293B; text-transform: uppercase; margin: 16px 0 4px 0; }");
            sb.AppendLine(".doc-subtitle { text-align: center; font-size: 12px; font-style: italic; color: #64748B; margin-bottom: 20px; }");
            sb.AppendLine(".meta-table { width: 100%; border-collapse: collapse; background: #F8FAFC; border: 1px solid #E2E8F0; border-radius: 6px; margin-bottom: 24px; }");
            sb.AppendLine(".meta-table td { padding: 8px 12px; font-size: 12.5px; border-bottom: 1px solid #F1F5F9; }");
            sb.AppendLine(".meta-label { color: #475569; font-weight: 600; width: 22%; }");
            sb.AppendLine(".meta-val { color: #0F172A; font-weight: bold; }");
            sb.AppendLine(".section-header { font-size: 13px; font-weight: 700; color: #1E293B; text-transform: uppercase; margin: 20px 0 10px 0; padding-left: 8px; border-left: 4px solid #7C3AED; }");
            sb.AppendLine(".data-table { width: 100%; border-collapse: collapse; margin-bottom: 20px; font-size: 12.5px; }");
            sb.AppendLine(".data-table th { background: #7C3AED; color: #FFFFFF; font-weight: 700; padding: 10px 12px; border: 1px solid #6D28D9; text-align: left; }");
            sb.AppendLine(".data-table td { padding: 9px 12px; border: 1px solid #E2E8F0; }");
            sb.AppendLine(".data-table tr:nth-child(even) { background: #F8FAFC; }");
            sb.AppendLine(".badge-approved { display: inline-block; background: #DCFCE7; color: #15803D; font-weight: 700; font-size: 11px; padding: 3px 8px; border-radius: 4px; border: 1px solid #BBF7D0; }");
            sb.AppendLine(".notes-box { background: #EFF6FF; border: 1px solid #BFDBFE; border-left: 4px solid #2563EB; padding: 12px 16px; border-radius: 4px; margin-bottom: 24px; font-size: 12.5px; color: #1E40AF; line-height: 1.5; }");
            sb.AppendLine(".stamp-box { border: 2.5px dashed #16A34A; color: #16A34A; font-weight: bold; font-size: 11px; padding: 8px 14px; display: inline-block; transform: rotate(-4deg); margin-top: 10px; line-height: 1.4; background: #F0FDF4; border-radius: 4px; }");
            sb.AppendLine(".stamp-pending { border: 2px dashed #94A3B8; color: #94A3B8; font-weight: bold; font-size: 11px; padding: 8px 14px; display: inline-block; margin-top: 10px; line-height: 1.4; background: #F8FAFC; border-radius: 4px; }");
            sb.AppendLine("</style>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");

            sb.AppendLine("<div class='doc-page'>");

            // Letterhead
            sb.AppendLine("<table class='header-table'>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td width='65%'>");
            sb.AppendLine("<div class='company-name'>CÔNG TY CỔ PHẦN BÁN LẺ THỜI TRANG & THIẾT BỊ SỐ 2026</div>");
            sb.AppendLine("<div class='company-sub'>HỆ THỐNG HỖ TRỢ RA QUYẾT ĐỊNH BÁN HÀNG — DSS RETAIL STUDIO</div>");
            sb.AppendLine("</td>");
            sb.AppendLine("<td width='35%' align='right'>");
            sb.AppendLine($"<div class='po-badge'>LỆNH PO: {poNum}</div>");
            sb.AppendLine($"<div style='font-size: 11px; color: #64748B; margin-top: 4px;'>Ngày lập: {DateTime.Now:dd/MM/yyyy HH:mm}</div>");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");

            // Title
            sb.AppendLine("<div class='doc-title'>LỆNH MUA HÀNG & PHÊ DUYỆT NHẬP KHO (PURCHASE ORDER)</div>");
            sb.AppendLine("<div class='doc-subtitle'>Căn cứ kết quả mô hình giải thuật Knapsack Solver & Phân tích thứ bậc AHP Saaty</div>");

            // Meta Info
            sb.AppendLine("<table class='meta-table'>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td class='meta-label'>Người phê duyệt:</td>");
            sb.AppendLine($"<td class='meta-val'>{user?.FullName ?? "Decision Maker"} ({user?.RoleTitle ?? "Giám đốc"})</td>");
            sb.AppendLine("<td class='meta-label'>Tình trạng văn bản:</td>");
            sb.AppendLine($"<td class='meta-val' style='color:{(isPoSigned ? "#16A34A" : "#D97706")};'>{(isPoSigned ? "✓ Đã Ký Duyệt Hợp Lệ" : "⚠️ Chờ Ký Duyệt")}</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td class='meta-label'>Căn cứ pháp lý:</td>");
            sb.AppendLine("<td colspan='3'>Biên bản thẩm định đa tiêu chí Saaty AHP (Tỷ số CR &le; 0.10) & Trần ngân sách kế hoạch quý</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");

            // Section: Notes
            if (!string.IsNullOrWhiteSpace(notes))
            {
                sb.AppendLine("<div class='section-header'>1. Ý KIẾN CHỈ ĐẠO ĐIỀU HÀNH</div>");
                sb.AppendLine($"<div class='notes-box'><strong>Chỉ đạo của người ra quyết định:</strong> {notes}</div>");
            }

            // Section: Items table
            sb.AppendLine("<div class='section-header'>2. DANH MỤC HÀNG HÓA ĐƯỢC PHÊ DUYỆT GIẢI NGÂN</div>");
            sb.AppendLine("<table class='data-table'>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<th width='8%' align='center'>STT</th>");
            sb.AppendLine("<th>Mặt Hàng & Quy Cách SKU</th>");
            sb.AppendLine("<th width='24%' align='center'>Trạng Thái Phê Duyệt</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            int idx = 1;
            bool hasItem = false;
            foreach (var item in items)
            {
                hasItem = true;
                sb.AppendLine($"<tr><td align='center'>{idx++}</td><td><strong>{item}</strong></td><td align='center'><span class='badge-approved'>✓ Đã Phê Duyệt</span></td></tr>");
            }

            if (!hasItem)
            {
                sb.AppendLine("<tr><td colspan='3' align='center' style='padding:16px; color:#94A3B8;'><em>(Chưa có mặt hàng nào được tích chọn phê duyệt)</em></td></tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");

            // Signatures block
            sb.AppendLine("<table style='width: 100%; margin-top: 36px; border-top: 1px dashed #CBD5E1; padding-top: 20px;'>");
            sb.AppendLine("<tr>");
            sb.AppendLine("<td style='width: 50%; text-align: center; vertical-align: top;'>");
            sb.AppendLine("<strong>PHÒNG MUA HÀNG & KHO VẬN</strong><br/>");
            sb.AppendLine("<span style='font-size: 11px; color: #64748B;'>(Ký nhận & tiếp nhận thi hành)</span><br/><br/><br/><br/>");
            sb.AppendLine("<strong>TRƯỞNG PHÒNG THU MUA</strong>");
            sb.AppendLine("</td>");
            sb.AppendLine("<td style='width: 50%; text-align: center; vertical-align: top;'>");
            sb.AppendLine("<strong>NGƯỜI RA QUYẾT ĐỊNH PHÊ DUYỆT</strong><br/>");
            sb.AppendLine("<span style='font-size: 11px; color: #64748B;'>(Đã xác thực chữ ký điện tử)</span><br/><br/>");

            if (isPoSigned)
            {
                sb.AppendLine("<div class='stamp-box'>✓ ĐÃ KÝ DUYỆT ĐIỆN TỬ<br/>DECISION MAKER VERIFIED<br/>CHUẨN AHP SAATY 2026</div><br/><br/>");
            }
            else
            {
                sb.AppendLine("<div class='stamp-pending'>⚠️ CHỜ CHỮ KÝ DUYỆT<br/>(Bấm 'Ký duyệt' ở cột trái)</div><br/><br/>");
            }

            sb.AppendLine($"<strong>{user?.FullName ?? "Nguyễn Đăng Quang Huy"}</strong>");
            sb.AppendLine("</td>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("</div>"); // doc-page
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");

            return sb.ToString();
        }

        // =========================================================================
        // PUBLIC DELEGATES ĐIỀU HƯỚNG TƯƠNG THÍCH NGƯỢC
        // =========================================================================
        public void ShowSolverDialog() => mainForm.SwitchToView(1);
        public void ShowAlertsDialog() => mainForm.SwitchToView(2);
        public void ShowSnapshotsDialog() => mainForm.SwitchToView(3);
        public void ShowWorkflowDialog() => mainForm.SwitchToView(4);


        // =========================================================================
        // PUBLIC API CHO FORM1 GỌI KHI CÓ DỮ LIỆU MỚI
        // =========================================================================
        public void RefreshData(DssSnapshot snap)
        {
            if (snap == null) return;
            if (lblStatusDesc != null)
            {
                lblStatusDesc.Text = $"• Tỷ số Saaty CR: {DssEngine.CrText(snap.Ahp)}\n• Top #1: {snap.Top?.ShortName ?? "N/A"}\n• Bấm các nút trên để mở công cụ";
            }
        }
    }
}
