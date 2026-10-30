#nullable disable
using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;

namespace DSS_App
{
    public partial class Form1 : Form
    {
        // Chuỗi kết nối tới SQL Server cục bộ
        private readonly string connectionString = DatabaseGuard.DatabaseConnectionString;

        // Header & User Session
        private Panel headerPanel;
        private Label lblTitle;
        private Label lblSubtitle;
        private Panel pnlUserBadge;
        private Label lblUserName;
        private Label lblUserRole;
        private Button btnLogout;
        public bool IsLogoutRequested { get; private set; } = false;
        private TabControl tabControl;
        private DecisionHubPanel pnlDecisionHub;
        private Panel pnlMainWorkspaceHost;
        private Panel pnlSolverView;
        private Panel pnlAlertsView;
        private Panel pnlSnapshotsView;
        private Panel pnlWorkflowView;
        public DssSnapshot? CurrentSnapshot => currentSnapshot;
        public void RunDSS() => ExecuteDSSModel();
        public void ApplyScenario(int scenario) => ApplyWhatIfScenario(scenario);
        private bool hasExecutedDSS = false;
       
        private TabPage tabInputFlow;
        private DataGridView dgvInputAlternatives;
        private DataGridView dgvInputMatrix;
        private Label lblInputMatrixStatus;
        private DssSnapshot? currentSnapshot;
        private Label? lblReportChartSub;
        private Dictionary<string, (int rank, decimal score)> baselineRanks = new();

        
        private TabPage tabWhatIf;
        private Button btnScenarioGaming;
        private Button btnScenarioSupplyCrisis;
        private Button btnScenarioInflation;
        private Button btnScenarioReset;
        private ComboBox cboWhatIfProduct;
        private NumericUpDown numWhatIfDemand;
        private NumericUpDown numWhatIfProfit;
        private NumericUpDown numWhatIfRisk;
        private Button btnApplyCustomWhatIf;
        private Label lblWhatIfAdvisory;
        private WhatIfDetailDiagramPanel whatIfDiagramPanel;
        private Button btnViewDeltaChart;
        private Button btnViewHierarchyTree;

        
        private TabPage tabOutputFlow;
        private Panel pnlOutputEmptyState;
        private Panel pnlOutputContent;
        private Label lblEmptyTitle;
        private Label lblEmptyDesc;

        // KPI Cards
        private Label lblCardRank1Val;
        private Label lblCardRank1Sub;
        private Label lblCardCRVal;
        private Label lblCardCRSub;
        private Label lblCardRevenueVal;
        private Label lblCardRevenueSub;
        private Label lblCardCandidatesVal;
        private Label lblCardCandidatesSub;

        private DataGridView dgvRanking;
        private AHPScoreChartPanel ahpChartPanel;
        private Label lblAdvisoryContent;
        private Button btnExportReport;

        
        private TabPage tabAddProduct;
        private TextBox txtProductName;
        private TextBox txtProductCode;
        private ComboBox cboCategory;
        private NumericUpDown numCostPrice;
        private NumericUpDown numSellingPrice;
        private NumericUpDown numStock;
        private NumericUpDown numExpectedRevenue;
        private NumericUpDown numMarketDemand;
        private NumericUpDown numInventoryDays;
        private NumericUpDown numMarketingCost;
        private Label lblCalculatedMargin;
        private TextBox txtAutoMarginC1;
        private Button btnSaveAndRank;

       
        private TabPage tabSales;
        private DataGridView dgvSales;
        private Button btnViewSales;
        private Button btnViewRollup;
        private Button btnViewCube;
        private Label lblOlapDescription;

        // TAB 6: FORM BÁO CÁO & AI INSIGHTS
        private TabPage tabAI;
        private RichTextBox rtbAI;
        private AHPScoreChartPanel reportChartPanel;
        private DataGridView dgvReportTable;
        private Label lblReportTopAltVal;
        private Label lblReportTopAltSub;
        private Label lblReportCRVal;
        private Label lblReportCRSub;
        private Label lblReportQtyVal;
        private Label lblReportQtySub;
        private Label lblReportCapVal;
        private Label lblReportCapSub;
        private Label lblReportDirective1;
        private Label lblReportDirective2;
        private Label lblReportDirective3;
        private Label lblReportHeaderDate;

        // ===== Ràng buộc SQL Server: mất kết nối => khóa toàn bộ giao diện =====
        private Panel? pnlDbOverlay;
        private Label? lblDbOverlayMsg;
        private System.Windows.Forms.Timer? dbHeartbeat;
        private bool dbOnline = false;
        private bool heartbeatBusy = false;

        public Form1()
        {
            InitializeComponentCustom();
            BuildDbOverlay();

            // Kiểm tra SQL Server ngay khi mở: không chạy được thì khóa hệ thống và báo lỗi.
            string err;
            if (DatabaseGuard.Ping(out err))
            {
                dbOnline = true;
                LoadInputData();
            }
            else
            {
                ShowDbOverlay(err);
            }
            UpdateCurrentUserDisplay();

            dbHeartbeat = new System.Windows.Forms.Timer { Interval = 4000 };
            dbHeartbeat.Tick += async (s, e) => await DbHeartbeatTick();
            dbHeartbeat.Start();
        }

        private void BuildDbOverlay()
        {
            pnlDbOverlay = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(15, 23, 42), Visible = false };

            Label lblIcon = new Label
            {
                Text = "⛔",
                Font = new Font("Segoe UI Emoji", 40f),
                ForeColor = Color.FromArgb(248, 113, 113),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 90
            };
            Label lblTitleOv = new Label
            {
                Text = "MẤT KẾT NỐI SQL SERVER – HỆ THỐNG TẠM DỪNG",
                Font = new Font("Segoe UI", 18f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 50
            };
            lblDbOverlayMsg = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 10.5f),
                ForeColor = Color.FromArgb(203, 213, 225),
                AutoSize = false,
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Top,
                Height = 160
            };

            Button btnRetry = new Button
            {
                Text = "↻ Thử kết nối lại",
                Size = new Size(190, 42),
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRetry.FlatAppearance.BorderSize = 0;
            btnRetry.Click += async (s, e) => await DbHeartbeatTick();

            Button btnQuit = new Button
            {
                Text = "Thoát ứng dụng",
                Size = new Size(170, 42),
                BackColor = Color.FromArgb(51, 65, 85),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnQuit.FlatAppearance.BorderSize = 0;
            btnQuit.Click += (s, e) => { IsLogoutRequested = true; Application.Exit(); };

            Panel pnlBtns = new Panel { Dock = DockStyle.Top, Height = 60 };
            pnlBtns.Resize += (s, e) =>
            {
                int total = btnRetry.Width + 16 + btnQuit.Width;
                int x = Math.Max(0, (pnlBtns.Width - total) / 2);
                btnRetry.Location = new Point(x, 8);
                btnQuit.Location = new Point(x + btnRetry.Width + 16, 8);
            };
            pnlBtns.Controls.Add(btnRetry);
            pnlBtns.Controls.Add(btnQuit);

            Panel pnlCenter = new Panel { Dock = DockStyle.Top, Height = 400 };
            pnlCenter.Controls.Add(pnlBtns);
            pnlCenter.Controls.Add(lblDbOverlayMsg);
            pnlCenter.Controls.Add(lblTitleOv);
            pnlCenter.Controls.Add(lblIcon);

            pnlDbOverlay.Controls.Add(pnlCenter);
            pnlDbOverlay.Resize += (s, e) => pnlCenter.Top = Math.Max(0, (pnlDbOverlay.Height - pnlCenter.Height) / 2 - 40);
            pnlCenter.Dock = DockStyle.None;
            pnlCenter.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlCenter.Width = pnlDbOverlay.Width;
            pnlDbOverlay.Resize += (s, e) => { pnlCenter.Left = 0; pnlCenter.Width = pnlDbOverlay.Width; };

            this.Controls.Add(pnlDbOverlay);
            pnlDbOverlay.BringToFront();
        }

        private void ShowDbOverlay(string message)
        {
            dbOnline = false;
            if (pnlDbOverlay == null) return;
            if (lblDbOverlayMsg != null) lblDbOverlayMsg.Text = message;
            pnlDbOverlay.Visible = true;
            pnlDbOverlay.BringToFront();
        }

        private async System.Threading.Tasks.Task DbHeartbeatTick()
        {
            if (heartbeatBusy) return;
            heartbeatBusy = true;
            try
            {
                string err = "";
                bool ok = await System.Threading.Tasks.Task.Run(() => DatabaseGuard.Ping(out err));
                if (ok)
                {
                    if (!dbOnline)
                    {
                        dbOnline = true;
                        if (pnlDbOverlay != null) pnlDbOverlay.Visible = false;
                        LoadInputData();
                    }
                }
                else
                {
                    ShowDbOverlay(err);
                }
            }
            finally
            {
                heartbeatBusy = false;
            }
        }

        private void UpdateMatrixStatus(AhpResult ahp)
        {
            if (lblInputMatrixStatus == null) return;
            if (!ahp.MatrixComplete)
            {
                lblInputMatrixStatus.Text = "✕ Ma trận so sánh cặp trong CSDL chưa đầy đủ – không thể tính trọng số.";
                lblInputMatrixStatus.ForeColor = Color.FromArgb(220, 38, 38);
            }
            else if (ahp.IsConsistent)
            {
                lblInputMatrixStatus.Text = $"✓ Tỷ số nhất quán (tính từ SQL Server): {DssEngine.CrText(ahp)} [ĐẠT CHUẨN SAATY CR ≤ {DssEngine.CrLimit:0.00}] • λmax = {ahp.LambdaMax:0.0000}";
                lblInputMatrixStatus.ForeColor = Color.FromArgb(16, 185, 129);
            }
            else
            {
                lblInputMatrixStatus.Text = $"✕ Ma trận KHÔNG nhất quán: {DssEngine.CrText(ahp)} > {DssEngine.CrLimit:0.00} – cần điều chỉnh ma trận Saaty.";
                lblInputMatrixStatus.ForeColor = Color.FromArgb(220, 38, 38);
            }
        }

        private void InitializeComponentCustom()
        {
            this.Text = "HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH BÁN HÀNG - DSS RETAIL STUDIO 2026";
            this.Size = new Size(1440, 900);
            this.MinimumSize = new Size(1180, 720);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.WindowState = FormWindowState.Maximized;
            this.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            this.BackColor = Color.FromArgb(248, 250, 252);

            // 1. Header
            BuildHeaderPanel();

            // 1b. Left Decision Hub Panel (4 tính năng theo chiều dọc)
            pnlDecisionHub = new DecisionHubPanel(this, connectionString);

            // 2. TabControl với TabSizeMode.Normal tự động vừa vặn chữ
            tabControl = new TabControl
            {
                Dock = DockStyle.Fill,
                Padding = new Point(16, 8),
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                SizeMode = TabSizeMode.Normal
            };

            // Setup 6 Tabs theo đúng quy trình DSS chuẩn: INPUT -> OUTPUT -> WHAT-IF -> ADD -> OLAP -> AI
            SetupTabInputFlow();
            SetupTabOutputFlow();
            SetupTabWhatIf();
            SetupTabAddProduct();
            SetupTabSales();
            SetupTabAI();
            // 2b. Khởi tạo 4 UI riêng biệt cho 4 tính năng
            pnlMainWorkspaceHost = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252)
            };

            pnlSolverView = pnlDecisionHub.BuildSolverFullView();
            pnlAlertsView = pnlDecisionHub.BuildAlertsFullView();
            pnlSnapshotsView = pnlDecisionHub.BuildSnapshotsFullView();
            pnlWorkflowView = pnlDecisionHub.BuildWorkflowFullView();

            pnlMainWorkspaceHost.Controls.Add(tabControl);
            pnlMainWorkspaceHost.Controls.Add(pnlSolverView);
            pnlMainWorkspaceHost.Controls.Add(pnlAlertsView);
            pnlMainWorkspaceHost.Controls.Add(pnlSnapshotsView);
            pnlMainWorkspaceHost.Controls.Add(pnlWorkflowView);

            this.Controls.Add(pnlMainWorkspaceHost);
            this.Controls.Add(pnlDecisionHub);
            this.Controls.Add(headerPanel);

            pnlDecisionHub.SendToBack();
            headerPanel.SendToBack();
            pnlMainWorkspaceHost.BringToFront();

            SwitchToView(0);
        }

        public void SwitchToView(int viewIndex)
        {
            if (tabControl == null) return;

            tabControl.Visible = (viewIndex == 0);
            if (pnlSolverView != null) pnlSolverView.Visible = (viewIndex == 1);
            if (pnlAlertsView != null) pnlAlertsView.Visible = (viewIndex == 2);
            if (pnlSnapshotsView != null) pnlSnapshotsView.Visible = (viewIndex == 3);
            if (pnlWorkflowView != null) pnlWorkflowView.Visible = (viewIndex == 4);

            if (viewIndex == 0) tabControl.BringToFront();
            else if (viewIndex == 1) { pnlDecisionHub?.RefreshSolverFullView(); pnlSolverView?.BringToFront(); }
            else if (viewIndex == 2) { pnlDecisionHub?.RefreshAlertsFullView(); pnlAlertsView?.BringToFront(); }
            else if (viewIndex == 3) { pnlDecisionHub?.RefreshSnapshotsFullView(); pnlSnapshotsView?.BringToFront(); }
            else if (viewIndex == 4) { pnlDecisionHub?.RefreshWorkflowFullView(); pnlWorkflowView?.BringToFront(); }

            pnlDecisionHub?.SetActiveNavIndex(viewIndex);
        }

        private void BuildHeaderPanel()
        {
            headerPanel = new Panel
            {
                Dock = DockStyle.Top,
                Height = 84,
                BackColor = Color.FromArgb(15, 23, 42) // Slate 900
            };

            Panel logoBadge = new Panel
            {
                Size = new Size(50, 50),
                Location = new Point(20, 16),
                BackColor = Color.FromArgb(79, 70, 229) // Indigo 600
            };
            Label lblLogoText = new Label
            {
                Text = "DSS",
                Font = new Font("Segoe UI", 12f, FontStyle.Bold),
                ForeColor = Color.White,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleCenter
            };
            logoBadge.Controls.Add(lblLogoText);
            headerPanel.Controls.Add(logoBadge);

            lblTitle = new Label
            {
                Text = "DSS RETAIL DECISION STUDIO 2026",
                Font = new Font("Segoe UI", 14f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(78, 15)
            };

            lblSubtitle = new Label
            {
                Text = "Hệ Thống Phân Tích Thứ Bậc Saaty (AHP) & Hỗ Trợ Ra Quyết Định Bán Hàng",
                Font = new Font("Segoe UI", 9f, FontStyle.Regular),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Location = new Point(80, 44)
            };

            headerPanel.Controls.Add(lblTitle);
            headerPanel.Controls.Add(lblSubtitle);

            // 1. Nút Đăng xuất màu đỏ nổi bật, kích thước đủ rộng không bị cụt chữ
            btnLogout = new Button
            {
                Text = "🚪 Đăng xuất",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = Color.FromArgb(220, 38, 38), // Red 600
                FlatStyle = FlatStyle.Flat,
                Size = new Size(120, 36),
                Cursor = Cursors.Hand
            };
            btnLogout.FlatAppearance.BorderSize = 0;
            btnLogout.MouseEnter += (s, e) => btnLogout.BackColor = Color.FromArgb(185, 28, 28);
            btnLogout.MouseLeave += (s, e) => btnLogout.BackColor = Color.FromArgb(220, 38, 38);
            btnLogout.Click += BtnLogout_Click;
            headerPanel.Controls.Add(btnLogout);

            // 2. Thẻ Thông tin người dùng (Avatar + Họ tên + Badge Vai trò)
            pnlUserBadge = new Panel
            {
                AutoSize = true,
                Height = 36,
                BackColor = Color.FromArgb(30, 41, 59), // Slate 800
                Padding = new Padding(8, 4, 8, 4)
            };

            FlowLayoutPanel pnlUserInner = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.LeftToRight,
                BackColor = Color.Transparent,
                WrapContents = false
            };

            Label lblAvatar = new Label
            {
                Text = "👤",
                Font = new Font("Segoe UI", 10f),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Margin = new Padding(0, 4, 2, 0)
            };

            lblUserName = new Label
            {
                Text = UserSession.CurrentUser?.FullName ?? "Chưa đăng nhập",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Margin = new Padding(2, 5, 6, 0)
            };

            lblUserRole = new Label
            {
                Text = $"[{UserSession.CurrentUser?.Role ?? "DecisionMaker"}]",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Bold),
                ForeColor = Color.White,
                BackColor = UserSession.CurrentUser?.RoleBadgeColor ?? Color.FromArgb(139, 92, 246),
                Padding = new Padding(6, 2, 6, 2),
                AutoSize = true,
                Margin = new Padding(0, 4, 2, 0)
            };

            Label lblProfileHint = new Label
            {
                Text = "▾",
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                AutoSize = true,
                Margin = new Padding(2, 6, 4, 0),
                Cursor = Cursors.Hand
            };

            pnlUserInner.Controls.Add(lblAvatar);
            pnlUserInner.Controls.Add(lblUserName);
            pnlUserInner.Controls.Add(lblUserRole);
            pnlUserInner.Controls.Add(lblProfileHint);
            pnlUserBadge.Controls.Add(pnlUserInner);

            // Bấm vào badge người dùng hiển thị form hồ sơ cá nhân (User Profile Popup)
            Action openProfile = () =>
            {
                try
                {
                    using (UserProfileForm profileForm = new UserProfileForm(connectionString))
                    {
                        if (profileForm.ShowDialog(this) == DialogResult.OK)
                        {
                            UpdateCurrentUserDisplay();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Không thể mở hồ sơ người dùng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            pnlUserBadge.Cursor = Cursors.Hand;
            pnlUserInner.Cursor = Cursors.Hand;
            lblAvatar.Cursor = Cursors.Hand;
            lblUserName.Cursor = Cursors.Hand;
            lblUserRole.Cursor = Cursors.Hand;
            lblProfileHint.Cursor = Cursors.Hand;

            ToolTip tipUser = new ToolTip();
            string tipText = "👤 Nhấp để xem hồ sơ cá nhân, đổi mật khẩu & quyền hạn hệ thống";
            tipUser.SetToolTip(pnlUserBadge, tipText);
            tipUser.SetToolTip(pnlUserInner, tipText);
            tipUser.SetToolTip(lblAvatar, tipText);
            tipUser.SetToolTip(lblUserName, tipText);
            tipUser.SetToolTip(lblUserRole, tipText);
            tipUser.SetToolTip(lblProfileHint, tipText);

            pnlUserBadge.Click += (s, e) => openProfile();
            pnlUserInner.Click += (s, e) => openProfile();
            lblAvatar.Click += (s, e) => openProfile();
            lblUserName.Click += (s, e) => openProfile();
            lblUserRole.Click += (s, e) => openProfile();
            lblProfileHint.Click += (s, e) => openProfile();

            Color normalBg = Color.FromArgb(30, 41, 59);
            Color hoverBg = Color.FromArgb(51, 65, 85);
            bool isUserBadgeHovered = false;

            pnlUserBadge.Paint += (s, pe) =>
            {
                Color borderClr = isUserBadgeHovered ? Color.FromArgb(56, 189, 248) : Color.FromArgb(71, 85, 105);
                using (Pen p = new Pen(borderClr, 1.2f))
                {
                    pe.Graphics.DrawRectangle(p, 0, 0, pnlUserBadge.Width - 1, pnlUserBadge.Height - 1);
                }
            };

            Action setHover = () =>
            {
                isUserBadgeHovered = true;
                pnlUserBadge.BackColor = hoverBg;
                lblProfileHint.ForeColor = Color.FromArgb(56, 189, 248);
                pnlUserBadge.Invalidate();
            };
            Action setLeave = () =>
            {
                isUserBadgeHovered = false;
                pnlUserBadge.BackColor = normalBg;
                lblProfileHint.ForeColor = Color.FromArgb(148, 163, 184);
                pnlUserBadge.Invalidate();
            };

            pnlUserBadge.MouseEnter += (s, e) => setHover();
            pnlUserBadge.MouseLeave += (s, e) => setLeave();
            pnlUserInner.MouseEnter += (s, e) => setHover();
            pnlUserInner.MouseLeave += (s, e) => setLeave();
            lblAvatar.MouseEnter += (s, e) => setHover();
            lblAvatar.MouseLeave += (s, e) => setLeave();
            lblUserName.MouseEnter += (s, e) => setHover();
            lblUserName.MouseLeave += (s, e) => setLeave();
            lblUserRole.MouseEnter += (s, e) => setHover();
            lblUserRole.MouseLeave += (s, e) => setLeave();
            lblProfileHint.MouseEnter += (s, e) => setHover();
            lblProfileHint.MouseLeave += (s, e) => setLeave();

            headerPanel.Controls.Add(pnlUserBadge);

            // Tự động định vị các pill và nút bên phải khi Resize để không bao giờ bị đè chữ
            headerPanel.Resize += (s, e) => RepositionHeaderControls();
            this.Shown += (s, e) => RepositionHeaderControls();
            this.SizeChanged += (s, e) => RepositionHeaderControls();
            this.Resize += (s, e) => RepositionHeaderControls();
            RepositionHeaderControls();

            this.Controls.Add(headerPanel);
        }

        
        private void SetupTabInputFlow()
        {
            tabInputFlow = new TabPage("📥 1. Đầu Vào Dữ Liệu (Inputs & Parameters)");
            tabInputFlow.BackColor = Color.FromArgb(248, 250, 252);
            tabInputFlow.Padding = new Padding(16);

            // Split 2 phần Đầu Vào: Trái (Phương án ứng viên) & Phải (Ma trận so sánh cặp Saaty 5x5)
            SafeSplitContainer splitInput = new SafeSplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterWidth = 8,
                BackColor = Color.FromArgb(226, 232, 240)
            };
            splitInput.HandleCreated += (s, e) =>
            {
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (splitInput.Width > 400)
                        {
                            int target = Math.Min(750, (int)(splitInput.Width * 0.52));
                            if (target > 100 && target < splitInput.Width - 100)
                                splitInput.SplitterDistance = target;
                        }
                    }
                    catch { }
                }));
            };

            // Phần 1: Bảng phương án ứng viên đưa vào phiên quyết định
            Panel pnlLeftInput = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(16) };
            Label lblLeftTitle = new Label
            {
                Text = "1. DANH SÁCH PHƯƠNG ÁN / SẢN PHẨM ỨNG VIÊN ĐƯA VÀO PHIÊN DSS (ALTERNATIVES):",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Dock = DockStyle.Top,
                Height = 32
            };
            dgvInputAlternatives = CreateStyledDataGridView();
            dgvInputAlternatives.Dock = DockStyle.Fill;

            pnlLeftInput.Controls.Add(dgvInputAlternatives);
            pnlLeftInput.Controls.Add(lblLeftTitle);
            splitInput.Panel1.Controls.Add(pnlLeftInput);

            // Phần 2: Ma trận so sánh cặp Saaty giữa 5 tiêu chí (AHP Criteria Matrix)
            Panel pnlRightInput = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(16) };
            Label lblRightTitle = new Label
            {
                Text = "2. MA TRẬN SO SÁNH CẶP GIỮA CÁC TIÊU CHÍ (THANG ĐO SAATY 1 - 9):",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Dock = DockStyle.Top,
                Height = 32
            };

            Panel pnlInputStatusBottom = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 44,
                BackColor = Color.White,
                Padding = new Padding(0, 4, 0, 0)
            };

            lblInputMatrixStatus = new Label
            {
                Text = "Đang kiểm tra tỷ số nhất quán từ ma trận Saaty trong SQL Server...",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(16, 185, 129),
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft
            };

            Button btnRunFromTab1 = new Button
            {
                Text = "🚀 CHẠY MÔ HÌNH DSS NGAY",
                Size = new Size(240, 36),
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRunFromTab1.FlatAppearance.BorderSize = 0;
            btnRunFromTab1.Click += (s, e) => ExecuteDSSModel();

            pnlInputStatusBottom.Controls.Add(lblInputMatrixStatus);
            pnlInputStatusBottom.Controls.Add(btnRunFromTab1);

            dgvInputMatrix = CreateStyledDataGridView();
            dgvInputMatrix.Dock = DockStyle.Fill;

            pnlRightInput.Controls.Add(dgvInputMatrix);
            pnlRightInput.Controls.Add(pnlInputStatusBottom);
            pnlRightInput.Controls.Add(lblRightTitle);
            splitInput.Panel2.Controls.Add(pnlRightInput);

            tabInputFlow.Controls.Add(splitInput);

            tabControl.TabPages.Add(tabInputFlow);
        }

        
        private void SetupTabOutputFlow()
        {
            tabOutputFlow = new TabPage("📊 2. Kết Quả Ra Quyết Định (Outputs & Decision)");
            tabOutputFlow.BackColor = Color.FromArgb(248, 250, 252);
            tabOutputFlow.Padding = new Padding(16);

            // 1. Panel màn hình chờ khi CHƯA chạy mô hình
            pnlOutputEmptyState = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(40)
            };

            Label lblEmptyIcon = new Label
            {
                Text = "📊",
                Font = new Font("Segoe UI", 48f, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(560, 140)
            };

            lblEmptyTitle = new Label
            {
                Text = "CHƯA THỰC HIỆN TÍNH TOÁN PHIÊN HỖ TRỢ RA QUYẾT ĐỊNH",
                Font = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                AutoSize = true,
                Location = new Point(360, 240)
            };

            lblEmptyDesc = new Label
            {
                Text = "Hệ thống đang ở trạng thái chuẩn bị dữ liệu đầu vào. Vui lòng bấm nút dưới đây\nđể kích hoạt mô hình AHP và tính toán kết quả xếp hạng.",
                Font = new Font("Segoe UI", 11f, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.MiddleCenter,
                AutoSize = true,
                Location = new Point(330, 290)
            };

            Button btnQuickRun = new Button
            {
                Text = "🚀 BẤM VÀO ĐÂY ĐỂ CHẠY MÔ HÌNH DSS NGAY",
                Size = new Size(420, 50),
                Location = new Point(440, 370),
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnQuickRun.FlatAppearance.BorderSize = 0;
            btnQuickRun.Click += (s, e) => ExecuteDSSModel();

            pnlOutputEmptyState.Resize += (s, e) =>
            {
                int cx = pnlOutputEmptyState.Width / 2;
                lblEmptyIcon.Location = new Point(cx - lblEmptyIcon.Width / 2, 140);
                lblEmptyTitle.Location = new Point(cx - lblEmptyTitle.Width / 2, 230);
                lblEmptyDesc.Location = new Point(cx - lblEmptyDesc.Width / 2, 280);
                btnQuickRun.Location = new Point(cx - btnQuickRun.Width / 2, 360);
            };

            pnlOutputEmptyState.Controls.Add(lblEmptyIcon);
            pnlOutputEmptyState.Controls.Add(lblEmptyTitle);
            pnlOutputEmptyState.Controls.Add(lblEmptyDesc);
            pnlOutputEmptyState.Controls.Add(btnQuickRun);

            // 2. Panel nội dung Kết quả ĐẦU RA (khi ĐÃ bấm chạy)
            pnlOutputContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(16, 12, 16, 16),
                Visible = false // Mặc định ẩn cho tới khi bấm Chạy mô hình
            };

            BuildOutputContentComponents();

            tabOutputFlow.Controls.Add(pnlOutputEmptyState);
            tabOutputFlow.Controls.Add(pnlOutputContent);
            pnlOutputEmptyState.Visible = true;
            pnlOutputEmptyState.BringToFront();

            tabControl.TabPages.Add(tabOutputFlow);
        }

        private void BuildOutputContentComponents()
        {
            // Row 1: KPI Cards
            Panel kpiRow = new Panel
            {
                Dock = DockStyle.Top,
                Height = 120,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 14)
            };

            TableLayoutPanel tableKpi = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1
            };
            tableKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 23F));
            tableKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 24F));
            tableKpi.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 23F));

            tableKpi.Controls.Add(CreateKpiCard("LỰA CHỌN TỐI ƯU HÀNG ĐẦU (RANK 1)", 
                out lblCardRank1Val, "---", 
                out lblCardRank1Sub, "Chưa thực hiện phiên DSS",
                Color.FromArgb(236, 253, 245), Color.FromArgb(16, 185, 129), Color.FromArgb(6, 95, 70)), 0, 0);

            tableKpi.Controls.Add(CreateKpiCard("TỶ SỐ NHẤT QUÁN SAATY (CR)", 
                out lblCardCRVal, "---", 
                out lblCardCRSub, "Chưa tính toán",
                Color.FromArgb(239, 246, 255), Color.FromArgb(59, 130, 246), Color.FromArgb(30, 64, 175)), 1, 0);

            tableKpi.Controls.Add(CreateKpiCard("DOANH THU KỲ VỌNG", 
                out lblCardRevenueVal, "---", 
                out lblCardRevenueSub, "Chưa tính toán",
                Color.FromArgb(250, 245, 255), Color.FromArgb(139, 92, 246), Color.FromArgb(107, 33, 168)), 2, 0);

            tableKpi.Controls.Add(CreateKpiCard("TỔNG PHƯƠNG ÁN ĐÁNH GIÁ", 
                out lblCardCandidatesVal, "---", 
                out lblCardCandidatesSub, "Chưa tính toán",
                Color.FromArgb(255, 251, 235), Color.FromArgb(245, 158, 11), Color.FromArgb(146, 64, 14)), 3, 0);

            kpiRow.Controls.Add(tableKpi);

            // Row 2: Split 2 cột kết quả: Left 62% DataGridView Ranking | Right 38% Visuals & Advice
            SafeSplitContainer splitOutput = new SafeSplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                SplitterWidth = 8,
                BackColor = Color.FromArgb(226, 232, 240)
            };
            splitOutput.HandleCreated += (s, e) =>
            {
                this.BeginInvoke(new Action(() =>
                {
                    try
                    {
                        if (splitOutput.Width > 400)
                        {
                            int target = Math.Min(860, (int)(splitOutput.Width * 0.60));
                            if (target > 100 && target < splitOutput.Width - 100)
                                splitOutput.SplitterDistance = target;
                        }
                    }
                    catch { }
                }));
            };

            // Cột trái: Bảng xếp hạng chi tiết
            Panel pnlLeftGrid = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(14) };
            Panel gridHeader = new Panel { Dock = DockStyle.Top, Height = 40 };
            Label lblGridTitle = new Label
            {
                Text = "BẢNG XẾP HẠNG PHƯƠNG ÁN VÀ QUYẾT ĐỊNH PHÊ DUYỆT (DSS OUTPUT):",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Dock = DockStyle.Left,
                AutoSize = true
            };
            Panel pnlHeaderActions = new Panel
            {
                Dock = DockStyle.Right,
                AutoSize = true,
                Height = 36
            };

            Button btnRerunDSS = new Button
            {
                Text = "🔄 Chạy Lại Mô Hình",
                Size = new Size(165, 32),
                Location = new Point(0, 2),
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnRerunDSS.FlatAppearance.BorderSize = 0;
            btnRerunDSS.Click += (s, e) => ExecuteDSSModel();

            btnExportReport = new Button
            {
                Text = "📄 Xuất Biên Bản Báo Cáo",
                Size = new Size(180, 32),
                Location = new Point(173, 2),
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnExportReport.FlatAppearance.BorderSize = 0;
            btnExportReport.Click += (s, e) => ExportReport();

            pnlHeaderActions.Controls.Add(btnRerunDSS);
            pnlHeaderActions.Controls.Add(btnExportReport);

            gridHeader.Controls.Add(lblGridTitle);
            gridHeader.Controls.Add(pnlHeaderActions);

            dgvRanking = CreateStyledDataGridView();
            dgvRanking.Dock = DockStyle.Fill;
            dgvRanking.RowTemplate.Height = 44;
            dgvRanking.CellPainting += DgvRanking_CellPainting;

            pnlLeftGrid.Controls.Add(dgvRanking);
            pnlLeftGrid.Controls.Add(gridHeader);
            splitOutput.Panel1.Controls.Add(pnlLeftGrid);

            // Cột phải: Biểu đồ thanh điểm AHP + Khuyến nghị hành động
            Panel pnlRightVisual = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(248, 250, 252) };
            ahpChartPanel = new AHPScoreChartPanel
            {
                Dock = DockStyle.Top,
                Height = 310,
                BackColor = Color.White
            };

            Panel pnlAdvice = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(16)
            };
            Label lblAdviceHead = new Label
            {
                Text = "KHUYẾN NGHỊ HÀNH ĐỘNG CHIẾN LƯỢC (EXECUTIVE SUMMARY):",
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Dock = DockStyle.Top,
                Height = 28
            };
            lblAdvisoryContent = new Label
            {
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(51, 65, 85),
                Text = ""
            };
            pnlAdvice.Controls.Add(lblAdvisoryContent);
            pnlAdvice.Controls.Add(lblAdviceHead);

            Panel rightWrapper = new Panel { Dock = DockStyle.Fill };
            rightWrapper.Controls.Add(pnlAdvice);
            rightWrapper.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 10 });
            rightWrapper.Controls.Add(ahpChartPanel);
            pnlRightVisual.Controls.Add(rightWrapper);

            splitOutput.Panel2.Controls.Add(pnlRightVisual);

            pnlOutputContent.Controls.Add(splitOutput);
            pnlOutputContent.Controls.Add(kpiRow);
        }

        private Panel CreateKpiCard(string header, out Label lblVal, string defVal, out Label lblSub, string defSub, Color bg, Color border, Color textMain)
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = bg,
                Padding = new Padding(16, 10, 16, 8),
                Margin = new Padding(5, 0, 5, 0)
            };
            card.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(border, 1.5f))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                }
            };

            Label lblHead = new Label
            {
                Text = header,
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(100, 116, 139),
                Dock = DockStyle.Top,
                Height = 18
            };

            lblVal = new Label
            {
                Text = defVal,
                Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
                ForeColor = textMain,
                Dock = DockStyle.Top,
                Height = 32
            };

            lblSub = new Label
            {
                Text = defSub,
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105),
                Dock = DockStyle.Top,
                Height = 22
            };

            card.Controls.Add(lblSub);
            card.Controls.Add(lblVal);
            card.Controls.Add(lblHead);
            return card;
        }

        
        private void LoadInputData()
        {
            string currentStep = "Khởi tạo";
            try
            {
                currentStep = "Mở kết nối SQL Server";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // 1. Nạp danh sách Phương án ứng viên kèm thông số kỹ thuật (Inputs)
                    currentStep = "Truy vấn Alternatives";
                    string sqlAlternatives = @"
                        SELECT 
                            alt.alternative_code AS [Mã PA],
                            alt.alternative_name AS [Tên Phương Án / Sản Phẩm],
                            p.product_code AS [Mã SKU],
                            FORMAT(p.cost_price, '#,##0') + ' đ' AS [Giá Vốn],
                            FORMAT(p.selling_price, '#,##0') + ' đ' AS [Giá Niêm Yết],
                            p.stock_quantity AS [Tồn Kho],
                            CAST(((p.selling_price - p.cost_price) / p.selling_price) * 100.0 AS DECIMAL(5,2)) AS [Biên LN %]
                        FROM dbo.ALTERNATIVES alt
                        LEFT JOIN dbo.PRODUCTS p ON alt.product_id = p.product_id
                        WHERE alt.session_id = 1
                        ORDER BY alt.alternative_id ASC;";

                    using (SqlDataAdapter daAlt = new SqlDataAdapter(sqlAlternatives, conn))
                    {
                        DataTable dtAlt = new DataTable();
                        daAlt.Fill(dtAlt);
                        currentStep = "Gán DataSource dgvInputAlternatives";
                        if (dgvInputAlternatives != null)
                        {
                            dgvInputAlternatives.DataSource = dtAlt;
                            if (dgvInputAlternatives.Columns != null)
                            {
                                if (dgvInputAlternatives.Columns.Contains("Mã PA") && dgvInputAlternatives.Columns["Mã PA"] != null)
                                    dgvInputAlternatives.Columns["Mã PA"].FillWeight = 120;
                                if (dgvInputAlternatives.Columns.Contains("Tên Phương Án / Sản Phẩm") && dgvInputAlternatives.Columns["Tên Phương Án / Sản Phẩm"] != null)
                                    dgvInputAlternatives.Columns["Tên Phương Án / Sản Phẩm"].FillWeight = 240;
                            }
                        }
                    }

                    // 2. Nạp Ma trận so sánh cặp Saaty 5x5 (Inputs)
                    currentStep = "Truy vấn Pairwise Matrix";
                    string sqlMatrix = @"
                        SELECT 
                            criterion_name AS [Tiêu Chí Dòng],
                            c_profit AS [C1: Biên LN],
                            c_revenue AS [C2: Doanh Thu],
                            c_demand AS [C3: Nhu Cầu],
                            c_risk AS [C4: Rủi Ro Vốn],
                            c_mktcost AS [C5: Chi Phí Mkt]
                        FROM dbo.vw_AHP_PairwiseMatrix_Pivot
                        WHERE session_id = 1;";
                    using (SqlDataAdapter daMatrix = new SqlDataAdapter(sqlMatrix, conn))
                    {
                        DataTable dtMatrix = new DataTable();
                        daMatrix.Fill(dtMatrix);
                        currentStep = "Gán DataSource dgvInputMatrix";
                        if (dgvInputMatrix != null)
                        {
                            dgvInputMatrix.DataSource = dtMatrix;
                        }
                    }
                    UpdateMatrixStatus(DssEngine.ComputeAhp(conn, 1));

                    // 3. Nạp Dữ liệu Bán hàng lịch sử & Phân tích Đa chiều OLAP
                    currentStep = "Khởi tạo dữ liệu Tab OLAP";
                    LoadOlapData(0);

                    // 4. Khởi tạo dữ liệu Mô phỏng Biến Động Thị Trường (What-If)
                    currentStep = "Khởi tạo dữ liệu What-If";
                    PopulateWhatIfProducts(conn);
                    ApplyWhatIfScenario(0);
                }
            }
            catch (SqlException ex) when (DatabaseGuard.IsConnectionError(ex))
            {
                MessageBox.Show(DatabaseGuard.FriendlyConnectionError(ex), "Lỗi kết nối SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                try
                {
                    File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup_error.log"), $"[Bước: {currentStep}]\n{ex.ToString()}");
                }
                catch { }
                MessageBox.Show($"Lỗi nạp dữ liệu từ SQL Server tại [{currentStep}]:\n{ex.Message}", "Thông Báo Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

       
        private void ExecuteDSSModel()
        {
            // Phân quyền: SalesStaff chỉ được tra cứu kết quả, không được kích hoạt mô hình DSS
            if (UserSession.CurrentUser != null && UserSession.CurrentUser.IsSalesStaff)
            {
                MessageBox.Show("Chỉ Giám đốc kinh doanh / Quản trị viên được kích hoạt mô hình DSS.",
                    "Không đủ quyền", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Chạy mô hình AHP trên dữ liệu SQL Server: tính trọng số & CR từ ma trận so sánh cặp,
                // chuẩn hóa điểm, xếp hạng và ghi DECISION_RESULTS (từ chối nếu CR > 10%).
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    DssSnapshot snap = DssEngine.Recalculate(conn, 1);

                    // Tải kết quả đầu ra
                    string sqlRanking = @"
                        SELECT 
                            ranking AS [Hạng],
                            product_code AS [Mã SKU],
                            alternative_name AS [Phương Án / Sản Phẩm],
                            FORMAT(selling_price, '#,##0') + ' đ' AS [Giá Bán],
                            CAST(final_score AS DECIMAL(8,4)) AS [Điểm AHP],
                            decision_outcome AS [Phê Duyệt],
                            recommendation AS [Khuyến Nghị Chiến Lược],
                            comments AS [Đánh Giá Chi Tiết]
                        FROM dbo.vw_DSS_FinalRankingReport
                        WHERE session_id = 1
                        ORDER BY ranking ASC;";
                    
                    SqlDataAdapter daRank = new SqlDataAdapter(sqlRanking, conn);
                    DataTable dtRank = new DataTable();
                    daRank.Fill(dtRank);
                    dgvRanking.DataSource = dtRank;

                    if (dgvRanking.Columns != null)
                    {
                        if (dgvRanking.Columns.Contains("Hạng") && dgvRanking.Columns["Hạng"] != null) dgvRanking.Columns["Hạng"].FillWeight = 65;
                        if (dgvRanking.Columns.Contains("Mã SKU") && dgvRanking.Columns["Mã SKU"] != null) dgvRanking.Columns["Mã SKU"].FillWeight = 135;
                        if (dgvRanking.Columns.Contains("Phương Án / Sản Phẩm") && dgvRanking.Columns["Phương Án / Sản Phẩm"] != null) dgvRanking.Columns["Phương Án / Sản Phẩm"].FillWeight = 240;
                        if (dgvRanking.Columns.Contains("Giá Bán") && dgvRanking.Columns["Giá Bán"] != null) dgvRanking.Columns["Giá Bán"].FillWeight = 120;
                        if (dgvRanking.Columns.Contains("Điểm AHP") && dgvRanking.Columns["Điểm AHP"] != null) dgvRanking.Columns["Điểm AHP"].FillWeight = 95;
                        if (dgvRanking.Columns.Contains("Phê Duyệt") && dgvRanking.Columns["Phê Duyệt"] != null) dgvRanking.Columns["Phê Duyệt"].FillWeight = 125;
                    }

                    UpdateOutputView(dtRank, snap);
                }

                hasExecutedDSS = true;
                pnlOutputEmptyState.Visible = false;
                pnlOutputContent.Visible = true;
                pnlOutputContent.BringToFront();

                // Tự động chuyển người dùng sang Tab Kết Quả Đầu Ra
                tabControl.SelectedTab = tabOutputFlow;

                MessageBox.Show("MÔ HÌNH HỖ TRỢ RA QUYẾT ĐỊNH ĐÃ CHẠY HOÀN TẤT!\n\n• Đã tổng hợp trọng số ma trận Saaty và điểm số phương án.\n• Kết quả xếp hạng và biểu đồ so sánh đã được hiển thị tại Tab 2.", "Tính Toán Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message, "Không thể ra quyết định", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (SqlException ex) when (DatabaseGuard.IsConnectionError(ex))
            {
                MessageBox.Show(DatabaseGuard.FriendlyConnectionError(ex), "Mất kết nối SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tính toán mô hình: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private static double? RawByCode(DssSnapshot snap, AltAnalysis alt, string criterionCode)
        {
            var crit = snap.Ahp.Criteria.FirstOrDefault(c => string.Equals(c.Code, criterionCode, StringComparison.OrdinalIgnoreCase));
            if (crit != null && alt.Raw.TryGetValue(crit.Id, out double v)) return v;
            return null;
        }

        private void UpdateOutputView(DataTable dtRank, DssSnapshot snap)
        {
            if (dtRank.Rows.Count == 0 || snap.Top == null) return;
            currentSnapshot = snap;
            if (pnlDecisionHub != null) pnlDecisionHub.RefreshData(snap);
            AltAnalysis top = snap.Top;

            // 1. Lựa chọn tối ưu
            lblCardRank1Val.Text = top.ShortName;
            lblCardRank1Sub.Text = $"Điểm AHP: {top.Score:0.0000} • {DssEngine.OutcomeLabel(top.Outcome)}";

            // 2. Tỷ số nhất quán CR (tính từ ma trận so sánh cặp trong CSDL)
            lblCardCRVal.Text = DssEngine.CrText(snap.Ahp);
            lblCardCRSub.Text = DssEngine.CrVerdict(snap.Ahp);

            // 3. Doanh thu kỳ vọng & biên lợi nhuận của phương án hạng 1 (từ CSDL)
            double? expRev = RawByCode(snap, top, "C_REVENUE");
            lblCardRevenueVal.Text = expRev.HasValue ? $"{expRev.Value:#,##0} Triệu VNĐ" : "Chưa có dữ liệu";
            lblCardRevenueSub.Text = $"Biên lợi nhuận gộp: {top.MarginPct:0.00}%";

            // 4. Tổng phương án đánh giá
            int selectCount = snap.Ranked.Count(a => a.Outcome == "Selected");
            int reviewCount = snap.Ranked.Count(a => a.Outcome == "UnderReview");
            int rejectCount = snap.Ranked.Count(a => a.Outcome == "Rejected");
            lblCardCandidatesVal.Text = $"{dtRank.Rows.Count} Phương Án";
            lblCardCandidatesSub.Text = $"{selectCount} Chọn • {reviewCount} Xem Xét • {rejectCount} Loại";

            if (ahpChartPanel != null)
            {
                ahpChartPanel.SetData(dtRank);
            }

            lblAdvisoryContent.Text = DssEngine.ExecutiveSummary(snap);

            RecordBaselineRanks(dtRank);
            FormatAIMemo(dtRank, snap);
        }

        private void DgvRanking_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            // Tránh lỗi IndexOutOfRange hoặc NullReference khi vẽ RowHeader (-1) hoặc ColumnHeader (-1)
            if (dgvRanking == null || dgvRanking.Columns == null || e.RowIndex < 0 || e.ColumnIndex < 0 || e.ColumnIndex >= dgvRanking.Columns.Count) return;
            var targetCol = dgvRanking.Columns[e.ColumnIndex];
            if (targetCol == null) return;
            string colName = targetCol.Name;

            if (colName == "Hạng")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
                string rankText = e.Value?.ToString() ?? "";
                
                Color badgeBg = rankText switch
                {
                    "1" => Color.FromArgb(245, 158, 11),  // Gold
                    "2" => Color.FromArgb(100, 116, 139), // Silver
                    "3" => Color.FromArgb(180, 83, 9),    // Bronze
                    _   => Color.FromArgb(226, 232, 240)
                };
                Color textCol = rankText == "4" ? Color.FromArgb(51, 65, 85) : Color.White;

                int diameter = 24;
                int x = e.CellBounds.X + (e.CellBounds.Width - diameter) / 2;
                int y = e.CellBounds.Y + (e.CellBounds.Height - diameter) / 2;

                using (SolidBrush brush = new SolidBrush(badgeBg))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.FillEllipse(brush, x, y, diameter, diameter);
                }

                TextRenderer.DrawText(e.Graphics, rankText, new Font("Segoe UI", 9.5f, FontStyle.Bold),
                    new Rectangle(x, y, diameter, diameter), textCol, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
            else if (colName == "Phê Duyệt")
            {
                e.Paint(e.CellBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.ContentForeground);
                string outcome = e.Value?.ToString() ?? "";

                Color pillBg = Color.FromArgb(241, 245, 249);
                Color pillText = Color.FromArgb(71, 85, 105);
                string displayLabel = outcome;

                if (outcome == "Selected")
                {
                    pillBg = Color.FromArgb(209, 250, 229);
                    pillText = Color.FromArgb(6, 95, 70);
                    displayLabel = "✓ Phê Duyệt";
                }
                else if (outcome == "UnderReview")
                {
                    pillBg = Color.FromArgb(254, 243, 199);
                    pillText = Color.FromArgb(146, 64, 14);
                    displayLabel = "⏳ Xem Xét";
                }
                else if (outcome == "Rejected")
                {
                    pillBg = Color.FromArgb(254, 226, 226);
                    pillText = Color.FromArgb(153, 27, 27);
                    displayLabel = "✕ Loại Bỏ";
                }

                int pillWidth = 100;
                int pillHeight = 26;
                int x = e.CellBounds.X + (e.CellBounds.Width - pillWidth) / 2;
                int y = e.CellBounds.Y + (e.CellBounds.Height - pillHeight) / 2;

                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (GraphicsPath path = GetRoundedRectPath(new Rectangle(x, y, pillWidth, pillHeight), 12))
                using (SolidBrush brush = new SolidBrush(pillBg))
                {
                    e.Graphics.FillPath(brush, path);
                }

                TextRenderer.DrawText(e.Graphics, displayLabel, new Font("Segoe UI", 9f, FontStyle.Bold),
                    new Rectangle(x, y, pillWidth, pillHeight), pillText, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);

                e.Handled = true;
            }
        }

        private GraphicsPath GetRoundedRectPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

       
        private void SetupTabAddProduct()
        {
            tabAddProduct = new TabPage("➕ 4. Thêm Sản Phẩm Mới");
            tabAddProduct.BackColor = Color.FromArgb(248, 250, 252);
            tabAddProduct.Padding = new Padding(24);
            tabAddProduct.AutoScroll = true;

            TableLayoutPanel tableContainer = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                ColumnCount = 2,
                RowCount = 1,
                Height = 490
            };
            tableContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            tableContainer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Card 1
            Panel pnlProduct = CreateInputCard("1. THÔNG TIN SẢN PHẨM MỚI (OLTP DATA)");
            AddFormRow(pnlProduct, "Tên Sản Phẩm:", txtProductName = new TextBox { Text = "Laptop Asus ROG Strix G16", Width = 300 }, 68, 210);
            AddFormRow(pnlProduct, "Mã SKU:", txtProductCode = new TextBox { Text = "PRD_ASUS_ROG16", Width = 190 }, 118, 210);
            
            cboCategory = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 250 };
            cboCategory.Items.AddRange(new object[] { "Điện thoại thông minh", "Máy tính xách tay", "Thiết bị đeo thông minh", "Phụ kiện kỹ thuật số" });
            cboCategory.SelectedIndex = 1;
            AddFormRow(pnlProduct, "Ngành Hàng:", cboCategory, 168, 210);

            numCostPrice = new NumericUpDown { Maximum = 200000000, Increment = 500000, Value = 22000000, Width = 190 };
            numCostPrice.ValueChanged += UpdateCalculatedMargin;
            AddFormRow(pnlProduct, "Giá Vốn (VNĐ):", numCostPrice, 218, 210);

            numSellingPrice = new NumericUpDown { Maximum = 200000000, Increment = 500000, Value = 31490000, Width = 190 };
            numSellingPrice.ValueChanged += UpdateCalculatedMargin;
            AddFormRow(pnlProduct, "Giá Niêm Yết (VNĐ):", numSellingPrice, 268, 210);

            lblCalculatedMargin = new Label { Text = "Biên LN: 30.14% (Tối Ưu)", Font = new Font("Segoe UI", 9.5f, FontStyle.Bold), ForeColor = Color.FromArgb(16, 185, 129), AutoSize = true };
            AddFormRow(pnlProduct, "Tỷ Suất Lợi Nhuận:", lblCalculatedMargin, 318, 210);

            numStock = new NumericUpDown { Maximum = 10000, Value = 25, Width = 140 };
            AddFormRow(pnlProduct, "Tồn Kho Ban Đầu:", numStock, 368, 210);

            // Card 2
            Panel pnlCriteria = CreateInputCard("2. ĐÁNH GIÁ 5 TIÊU CHÍ DSS (AHP EVALUATION)");
            txtAutoMarginC1 = new TextBox
            {
                ReadOnly = true,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(16, 185, 129),
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Width = 240,
                Text = "30.14% (Khóa - Tự động tính)"
            };
            AddFormRow(pnlCriteria, "C1: Biên Lợi Nhuận:", txtAutoMarginC1, 68, 290);

            numExpectedRevenue = new NumericUpDown { Maximum = 10000, Value = 850, Width = 150 };
            AddFormRow(pnlCriteria, "C2: Doanh Thu Kỳ Vọng (Triệu đ):", numExpectedRevenue, 118, 290);

            numMarketDemand = new NumericUpDown { Maximum = 100, Minimum = 1, Value = 87, Width = 150 };
            AddFormRow(pnlCriteria, "C3: Nhu Cầu Thị Trường (1 - 100):", numMarketDemand, 168, 290);

            numInventoryDays = new NumericUpDown { Maximum = 365, Value = 19, Width = 150 };
            AddFormRow(pnlCriteria, "C4: Tồn Kho / Quay Vòng (Ngày):", numInventoryDays, 218, 290);

            numMarketingCost = new NumericUpDown { Maximum = 1000, Value = 35, Width = 150 };
            AddFormRow(pnlCriteria, "C5: Chi Phí Marketing (Triệu đ):", numMarketingCost, 268, 290);

            btnSaveAndRank = new Button
            {
                Text = "💾 LƯU SẢN PHẨM & CẬP NHẬT VÀO HỆ THỐNG",
                Size = new Size(480, 52),
                Location = new Point(24, 330),
                BackColor = Color.FromArgb(16, 185, 129),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnSaveAndRank.FlatAppearance.BorderSize = 0;
            btnSaveAndRank.Click += BtnSaveAndRank_Click;
            pnlCriteria.Controls.Add(btnSaveAndRank);

            tableContainer.Controls.Add(pnlProduct, 0, 0);
            tableContainer.Controls.Add(pnlCriteria, 1, 0);

            tabAddProduct.Controls.Add(tableContainer);
            tabControl.TabPages.Add(tabAddProduct);

            UpdateCalculatedMargin(null, null);
        }

        private Panel CreateInputCard(string title)
        {
            Panel card = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(24, 20, 24, 20),
                Margin = new Padding(10)
            };
            Label lblCardTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(24, 18),
                Size = new Size(500, 26),
                AutoSize = false
            };
            Panel divider = new Panel
            {
                Location = new Point(24, 48),
                Size = new Size(520, 1),
                BackColor = Color.FromArgb(226, 232, 240)
            };
            card.Controls.Add(lblCardTitle);
            card.Controls.Add(divider);
            return card;
        }

        private void AddFormRow(Control parent, string labelText, Control inputControl, int top, int inputX = 240)
        {
            Label lbl = new Label
            {
                Text = labelText,
                Location = new Point(24, top + 4),
                AutoSize = true,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(51, 65, 85)
            };
            inputControl.Location = new Point(inputX, top);
            inputControl.Font = new Font("Segoe UI", 9.5f, FontStyle.Regular);
            parent.Controls.Add(lbl);
            parent.Controls.Add(inputControl);
        }

        private void UpdateCalculatedMargin(object sender, EventArgs e)
        {
            if (numSellingPrice.Value > 0 && numSellingPrice.Value >= numCostPrice.Value)
            {
                decimal margin = ((numSellingPrice.Value - numCostPrice.Value) / numSellingPrice.Value) * 100m;
                lblCalculatedMargin.Text = $"Biên LN: {margin:0.00}% " + (margin >= 20 ? "(Tối Ưu)" : "(Cần Thận Trọng)");
                lblCalculatedMargin.ForeColor = margin >= 20 ? Color.FromArgb(16, 185, 129) : Color.FromArgb(217, 119, 6);

                if (txtAutoMarginC1 != null)
                {
                    txtAutoMarginC1.Text = $"{margin:0.00}% (Khóa - Tự động tính)";
                    txtAutoMarginC1.ForeColor = margin >= 20 ? Color.FromArgb(16, 185, 129) : Color.FromArgb(217, 119, 6);
                }
            }
            else
            {
                lblCalculatedMargin.Text = "Giá bán phải lớn hơn hoặc bằng giá vốn!";
                lblCalculatedMargin.ForeColor = Color.FromArgb(239, 68, 68);

                if (txtAutoMarginC1 != null)
                {
                    txtAutoMarginC1.Text = "Giá bán < Giá vốn (Lỗi)!";
                    txtAutoMarginC1.ForeColor = Color.FromArgb(239, 68, 68);
                }
            }
        }

        private void BtnSaveAndRank_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtProductName.Text) || string.IsNullOrWhiteSpace(txtProductCode.Text))
            {
                MessageBox.Show("Vui lòng nhập đầy đủ Tên sản phẩm và Mã SKU!", "Thiếu Thông Tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (numSellingPrice.Value < numCostPrice.Value)
            {
                MessageBox.Show("Giá bán không được nhỏ hơn giá vốn!", "Sai Dữ Liệu", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    int categoryId = cboCategory.SelectedIndex + 1;
                    string sqlProduct = @"
                        IF NOT EXISTS (SELECT 1 FROM dbo.PRODUCTS WHERE product_code = @code)
                        BEGIN
                            INSERT INTO dbo.PRODUCTS (category_id, product_code, product_name, cost_price, selling_price, stock_quantity, min_stock_level, lead_time_days)
                            VALUES (@category_id, @code, @name, @cost, @sell, @stock, 5, 7);
                            SELECT SCOPE_IDENTITY();
                        END
                        ELSE
                        BEGIN
                            UPDATE dbo.PRODUCTS 
                            SET product_name = @name, cost_price = @cost, selling_price = @sell, stock_quantity = @stock
                            WHERE product_code = @code;
                            SELECT product_id FROM dbo.PRODUCTS WHERE product_code = @code;
                        END";

                    int productId;
                    using (SqlCommand cmd = new SqlCommand(sqlProduct, conn))
                    {
                        cmd.Parameters.AddWithValue("@category_id", categoryId);
                        cmd.Parameters.AddWithValue("@code", txtProductCode.Text.Trim());
                        cmd.Parameters.AddWithValue("@name", txtProductName.Text.Trim());
                        cmd.Parameters.AddWithValue("@cost", numCostPrice.Value);
                        cmd.Parameters.AddWithValue("@sell", numSellingPrice.Value);
                        cmd.Parameters.AddWithValue("@stock", (int)numStock.Value);
                        productId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    string altCode = "ALT_" + txtProductCode.Text.Trim().Replace("PRD_", "");
                    string sqlAlt = @"
                        IF NOT EXISTS (SELECT 1 FROM dbo.ALTERNATIVES WHERE session_id = 1 AND product_id = @prod_id)
                        BEGIN
                            INSERT INTO dbo.ALTERNATIVES (session_id, product_id, alternative_code, alternative_name, description, notes)
                            VALUES (1, @prod_id, @alt_code, @alt_name, N'Sản phẩm mới nhập liệu trực tiếp từ giao diện', N'Nhập liệu tự động');
                            SELECT SCOPE_IDENTITY();
                        END
                        ELSE
                        BEGIN
                            SELECT alternative_id FROM dbo.ALTERNATIVES WHERE session_id = 1 AND product_id = @prod_id;
                        END";

                    int alternativeId;
                    using (SqlCommand cmd = new SqlCommand(sqlAlt, conn))
                    {
                        cmd.Parameters.AddWithValue("@prod_id", productId);
                        cmd.Parameters.AddWithValue("@alt_code", altCode);
                        cmd.Parameters.AddWithValue("@alt_name", txtProductName.Text.Trim());
                        alternativeId = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Chỉ lưu giá trị thô; điểm chuẩn hóa được DssEngine tính lại từ toàn bộ dữ liệu của phiên (Benefit = v/max, Cost = min/v).
                    decimal marginPct = ((numSellingPrice.Value - numCostPrice.Value) / numSellingPrice.Value) * 100m;

                    SaveCriterionScore(conn, 1, alternativeId, 1, marginPct, 0m);
                    SaveCriterionScore(conn, 1, alternativeId, 2, numExpectedRevenue.Value, 0m);
                    SaveCriterionScore(conn, 1, alternativeId, 3, numMarketDemand.Value, 0m);
                    SaveCriterionScore(conn, 1, alternativeId, 4, numInventoryDays.Value, 0m);
                    SaveCriterionScore(conn, 1, alternativeId, 5, numMarketingCost.Value, 0m);
                    DssEngine.RefreshNormalization(conn, 1);
                }

                LoadInputData();

                // Đánh dấu dữ liệu đã thay đổi -> Đưa Tab 2 về trạng thái chờ chạy lại mô hình với nút bấm to rõ ràng
                hasExecutedDSS = false;
                if (lblEmptyTitle != null)
                    lblEmptyTitle.Text = "DỮ LIỆU ĐÃ THAY ĐỔI – CẦN CHẠY LẠI MÔ HÌNH DSS";
                if (lblEmptyDesc != null)
                    lblEmptyDesc.Text = $"Đã thêm sản phẩm mới [{txtProductName.Text.Trim()}].\nVui lòng bấm nút dưới đây để chạy lại giải thuật AHP và cập nhật bảng xếp hạng.";

                if (pnlOutputContent != null) pnlOutputContent.Visible = false;
                if (pnlOutputEmptyState != null)
                {
                    pnlOutputEmptyState.Visible = true;
                    pnlOutputEmptyState.BringToFront();
                }

                MessageBox.Show($"ĐÃ LƯU THÀNH CÔNG!\nSản phẩm [{txtProductName.Text}] đã được thêm vào hệ thống.\nĐang chuyển sang Tab 2 [Kết Quả Ra Quyết Định] để bạn bấm 'CHẠY LẠI MÔ HÌNH DSS'!", "Lưu Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                tabControl.SelectedTab = tabOutputFlow;
            }
            catch (SqlException ex) when (DatabaseGuard.IsConnectionError(ex))
            {
                MessageBox.Show(DatabaseGuard.FriendlyConnectionError(ex), "Mất kết nối SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi lưu dữ liệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SaveCriterionScore(SqlConnection conn, int sessionId, int altId, int critId, decimal rawVal, decimal normScore)
        {
            string sql = @"
                IF EXISTS (SELECT 1 FROM dbo.ALTERNATIVE_SCORES WHERE session_id = @s_id AND alternative_id = @alt_id AND criterion_id = @crit_id)
                BEGIN
                    UPDATE dbo.ALTERNATIVE_SCORES 
                    SET raw_value = @raw, normalized_score = @norm
                    WHERE session_id = @s_id AND alternative_id = @alt_id AND criterion_id = @crit_id;
                END
                ELSE
                BEGIN
                    INSERT INTO dbo.ALTERNATIVE_SCORES (session_id, alternative_id, criterion_id, raw_value, normalized_score)
                    VALUES (@s_id, @alt_id, @crit_id, @raw, @norm);
                END";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@s_id", sessionId);
                cmd.Parameters.AddWithValue("@alt_id", altId);
                cmd.Parameters.AddWithValue("@crit_id", critId);
                cmd.Parameters.AddWithValue("@raw", rawVal);
                cmd.Parameters.AddWithValue("@norm", normScore);
                cmd.ExecuteNonQuery();
            }
        }

        
        private void SetupTabSales()
        {
            tabSales = new TabPage("📦 5. Dữ Liệu Bán Hàng Lịch Sử (OLAP)");
            tabSales.BackColor = Color.FromArgb(248, 250, 252);
            tabSales.Padding = new Padding(16);

            Panel pnlCard = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(16) };

            Panel pnlHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 110,
                BackColor = Color.White
            };

            Label lblSalesHeader = new Label
            {
                Text = "DỮ LIỆU BÁN HÀNG THỰC TẾ & PHÂN TÍCH ĐA CHIỀU OLAP (ROLLUP & CUBE):",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 0),
                AutoSize = true
            };

            Panel pnlToolbar = new Panel
            {
                Location = new Point(0, 28),
                Size = new Size(1150, 42),
                BackColor = Color.White
            };

            btnViewSales = new Button
            {
                Text = "📊 1. Hiệu Suất Sản Phẩm (Fact/View)",
                Size = new Size(320, 38),
                Location = new Point(0, 2),
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnViewSales.FlatAppearance.BorderSize = 0;
            btnViewSales.Click += (s, e) => LoadOlapData(0);

            btnViewRollup = new Button
            {
                Text = "📦 2. OLAP ROLLUP: Phân Bổ Hàng Tồn Kho",
                Size = new Size(340, 38),
                Location = new Point(330, 2),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 41, 59),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnViewRollup.FlatAppearance.BorderSize = 0;
            btnViewRollup.Click += (s, e) => LoadOlapData(1);

            btnViewCube = new Button
            {
                Text = "🎯 3. OLAP CUBE: Đa Chiều Khuyến Mãi",
                Size = new Size(340, 38),
                Location = new Point(680, 2),
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(30, 41, 59),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnViewCube.FlatAppearance.BorderSize = 0;
            btnViewCube.Click += (s, e) => LoadOlapData(2);

            pnlToolbar.Controls.Add(btnViewSales);
            pnlToolbar.Controls.Add(btnViewRollup);
            pnlToolbar.Controls.Add(btnViewCube);

            lblOlapDescription = new Label
            {
                Text = "💡 View Hiệu Suất Sản Phẩm: Thống kê định lượng doanh thu, lợi nhuận gộp, biên lợi nhuận % và tồn kho thực tế của các SKU làm căn cứ cho tiêu chí DSS.",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(2, 78),
                AutoSize = true
            };

            pnlHeader.Controls.Add(lblSalesHeader);
            pnlHeader.Controls.Add(pnlToolbar);
            pnlHeader.Controls.Add(lblOlapDescription);

            dgvSales = CreateStyledDataGridView();
            dgvSales.Dock = DockStyle.Fill;

            pnlCard.Controls.Add(dgvSales);
            pnlCard.Controls.Add(pnlHeader);
            tabSales.Controls.Add(pnlCard);
            tabControl.TabPages.Add(tabSales);
        }

        private void LoadOlapData(int mode)
        {
            try
            {
                if (btnViewSales != null && btnViewRollup != null && btnViewCube != null)
                {
                    btnViewSales.BackColor = (mode == 0) ? Color.FromArgb(79, 70, 229) : Color.FromArgb(241, 245, 249);
                    btnViewSales.ForeColor = (mode == 0) ? Color.White : Color.FromArgb(30, 41, 59);

                    btnViewRollup.BackColor = (mode == 1) ? Color.FromArgb(79, 70, 229) : Color.FromArgb(241, 245, 249);
                    btnViewRollup.ForeColor = (mode == 1) ? Color.White : Color.FromArgb(30, 41, 59);

                    btnViewCube.BackColor = (mode == 2) ? Color.FromArgb(79, 70, 229) : Color.FromArgb(241, 245, 249);
                    btnViewCube.ForeColor = (mode == 2) ? Color.White : Color.FromArgb(30, 41, 59);
                }

                string sqlQuery = "";
                if (mode == 0)
                {
                    if (lblOlapDescription != null)
                        lblOlapDescription.Text = "💡 View Hiệu Suất Sản Phẩm: Thống kê định lượng doanh thu, lợi nhuận gộp, biên lợi nhuận % và tồn kho thực tế của các SKU làm căn cứ cho tiêu chí DSS.";

                    sqlQuery = @"
                        SELECT 
                            product_code AS [Mã SKU],
                            product_name AS [Tên Sản Phẩm],
                            category_name AS [Ngành Hàng],
                            FORMAT(cost_price, '#,##0') + ' đ' AS [Giá Vốn],
                            FORMAT(selling_price, '#,##0') + ' đ' AS [Giá Niêm Yết],
                            total_sold_quantity AS [Đã Bán],
                            FORMAT(total_revenue, '#,##0') + ' đ' AS [Tổng Doanh Thu],
                            FORMAT(gross_profit, '#,##0') + ' đ' AS [Lợi Nhuận Gộp],
                            CAST(gross_profit_margin_pct AS VARCHAR) + '%' AS [Biên LN %],
                            stock_quantity AS [Tồn Kho],
                            stock_health_status AS [Trạng Thái Tồn]
                        FROM dbo.vw_DSS_ProductPerformance;";
                }
                else if (mode == 1)
                {
                    if (lblOlapDescription != null)
                        lblOlapDescription.Text = "💡 OLAP ROLLUP Query: GROUP BY ROLLUP(c.category_name, p.product_name) - Phân cấp dữ liệu từ Chi tiết SKU -> Tổng Ngành Hàng -> Tổng Toàn Doanh Nghiệp phục vụ Phân Bổ Hàng Tồn.";

                    sqlQuery = @"
                        SELECT 
                            ISNULL(c.category_name, N'== TỔNG TOÀN DOANH NGHIỆP ==') AS [Ngành Hàng],
                            ISNULL(p.product_name, N'-- Tổng Ngành Hàng --') AS [Sản Phẩm],
                            ISNULL(SUM(p.stock_quantity), 0) AS [Tồn Kho Hiện Tại],
                            ISNULL(SUM(od.quantity), 0) AS [SL Bán Được],
                            FORMAT(ISNULL(SUM(od.line_total), 0), '#,##0') + ' đ' AS [Doanh Thu Thực Thu],
                            FORMAT(ISNULL(SUM(od.line_total - (od.quantity * od.unit_cost)), 0), '#,##0') + ' đ' AS [Lợi Nhuận Gộp],
                            CASE 
                                WHEN GROUPING(c.category_name) = 1 THEN N'Cấp Tổng Công Ty (Grand Total)'
                                WHEN GROUPING(p.product_name) = 1 THEN N'Tổng Ngành Hàng (Subtotal)'
                                ELSE N'Chi Tiết SKU (SKU Level)'
                            END AS [Cấp Độ Phân Cấp OLAP]
                        FROM dbo.CATEGORIES c
                        JOIN dbo.PRODUCTS p ON c.category_id = p.category_id
                        LEFT JOIN dbo.ORDER_DETAILS od ON p.product_id = od.product_id
                        GROUP BY ROLLUP(c.category_name, p.product_name)
                        ORDER BY GROUPING(c.category_name) DESC, c.category_name, GROUPING(p.product_name) DESC;";
                }
                else
                {
                    if (lblOlapDescription != null)
                        lblOlapDescription.Text = "💡 OLAP CUBE Query: GROUP BY CUBE(cust.customer_segment, cat.category_name) - Ma trận đa chiều Phân khúc Khách hàng × Ngành hàng phục vụ Quyết Định Khuyến Mãi.";

                    sqlQuery = @"
                        SELECT 
                            ISNULL(cust.customer_segment, N'== TẤT CẢ PHÂN KHÚC ==') AS [Phân Khúc Khách Hàng],
                            ISNULL(cat.category_name, N'== TẤT CẢ NGÀNH HÀNG ==') AS [Ngành Hàng],
                            COUNT(DISTINCT o.order_id) AS [Số Đơn Hàng],
                            ISNULL(SUM(od.quantity), 0) AS [Tổng Sản Lượng],
                            FORMAT(ISNULL(SUM(od.line_total), 0), '#,##0') + ' đ' AS [Doanh Thu Thu Được],
                            CAST(ROUND(ISNULL(AVG(od.discount_rate), 0), 1) AS VARCHAR) + '%' AS [Tỷ Lệ Giảm Giá TB],
                            FORMAT(ISNULL(SUM(od.line_total - (od.quantity * od.unit_cost)), 0), '#,##0') + ' đ' AS [Lợi Nhuận Gộp]
                        FROM dbo.ORDERS o
                        JOIN dbo.CUSTOMERS cust ON o.customer_id = cust.customer_id
                        JOIN dbo.ORDER_DETAILS od ON o.order_id = od.order_id
                        JOIN dbo.PRODUCTS prod ON od.product_id = prod.product_id
                        JOIN dbo.CATEGORIES cat ON prod.category_id = cat.category_id
                        GROUP BY CUBE(cust.customer_segment, cat.category_name)
                        ORDER BY GROUPING(cust.customer_segment) DESC, cust.customer_segment, GROUPING(cat.category_name) DESC;";
                }

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlQuery, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dgvSales != null)
                        {
                            dgvSales.DataSource = dt;
                            foreach (DataGridViewColumn col in dgvSales.Columns)
                            {
                                col.HeaderCell.Style.WrapMode = DataGridViewTriState.False;
                                col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleLeft;
                                col.MinimumWidth = 95;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi nạp dữ liệu OLAP: " + ex.Message, "Lỗi OLAP", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        
        private void SetupTabAI()
        {
            tabAI = new TabPage("📋 6. Form Báo Cáo & AI Insights");
            tabAI.BackColor = Color.FromArgb(241, 245, 249); // Slate-100 Soft Business Gray
            tabAI.Padding = new Padding(0);

            // =========================================================================
            // 1. TOP TOOLBAR: PHIẾU BÁO CÁO & CÁC NÚT THAO TÁC DOANH NGHIỆP
            // =========================================================================
            Panel pnlReportToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 60,
                BackColor = Color.White,
                Padding = new Padding(20, 8, 20, 8)
            };

            Panel pnlToolbarDivider = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 1,
                BackColor = Color.FromArgb(226, 232, 240)
            };
            pnlReportToolbar.Controls.Add(pnlToolbarDivider);

            Label lblToolbarDocTitle = new Label
            {
                Text = "📋 BIỂU MẪU BÁO CÁO THẨM ĐỊNH & PHÊ DUYỆT CHIẾN LƯỢC KINH DOANH",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(18, 9),
                AutoSize = true
            };

            Label lblToolbarDocSub = new Label
            {
                Text = "Mã biểu mẫu: BM-06/DSS-2026/Q4 • Trạng thái: 🟢 ĐÃ THẨM ĐỊNH & PHÊ DUYỆT • Khóa thẩm định: DSS-2026-Q4-ENT",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(20, 32),
                AutoSize = true
            };

            pnlReportToolbar.Controls.Add(lblToolbarDocTitle);
            pnlReportToolbar.Controls.Add(lblToolbarDocSub);

            // Nhóm nút thao tác ở góc phải
            FlowLayoutPanel pnlActionButtons = new FlowLayoutPanel
            {
                Dock = DockStyle.Right,
                Width = 560,
                Height = 44,
                FlowDirection = FlowDirection.RightToLeft,
                BackColor = Color.White,
                Margin = new Padding(0)
            };

            Button btnExportWord = new Button
            {
                Text = "📝 Xuất File Word (.doc)",
                Size = new Size(185, 38),
                BackColor = Color.FromArgb(24, 90, 189), // Word Blue (#185ABD)
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(6, 0, 0, 0)
            };
            btnExportWord.FlatAppearance.BorderSize = 0;
            btnExportWord.Click += (s, e) => ExportWordDocument();

            Button btnExportHtml = new Button
            {
                Text = "🖨️ In Báo Cáo / Lưu PDF",
                Size = new Size(180, 38),
                BackColor = Color.FromArgb(16, 185, 129), // Emerald Green
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(6, 0, 0, 0)
            };
            btnExportHtml.FlatAppearance.BorderSize = 0;
            btnExportHtml.Click += (s, e) => ExportReport();

            Button btnRefreshReport = new Button
            {
                Text = "🔄 Cập Nhật Dữ Liệu",
                Size = new Size(160, 38),
                BackColor = Color.FromArgb(248, 250, 252),
                ForeColor = Color.FromArgb(30, 41, 59),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 0)
            };
            btnRefreshReport.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
            btnRefreshReport.Click += (s, e) => RefreshReportData();

            pnlActionButtons.Controls.Add(btnExportWord);
            pnlActionButtons.Controls.Add(btnExportHtml);
            pnlActionButtons.Controls.Add(btnRefreshReport);
            pnlReportToolbar.Controls.Add(pnlActionButtons);

           
            Panel pnlReportScroll = new Panel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                BackColor = Color.FromArgb(241, 245, 249),
                Padding = new Padding(24, 16, 24, 28)
            };

           
            Panel pnlCardAdmin;
            Panel pnlWrapAdmin = CreateFormCardWrapper(168, out pnlCardAdmin);
            pnlCardAdmin.Padding = new Padding(24, 14, 24, 12);

            Panel pnlDualLetterhead = new Panel { Dock = DockStyle.Top, Height = 74, BackColor = Color.White };
            Label lblAdminOrg = new Label
            {
                Text = "DSS RETAIL STUDIO 2026\nPHÊ DUYỆT CHIẾN LƯỢC KINH DOANH\nSố lưu văn bản: 88/BB-HĐDSS/2026 • Khóa thẩm định: DSS-2026-Q4-ENT",
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                TextAlign = ContentAlignment.TopLeft,
                Dock = DockStyle.Left,
                Width = 480
            };

            lblReportHeaderDate = new Label
            {
                Text = $"CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM\nĐộc lập - Tự do - Hạnh phúc\nTP. Hồ Chí Minh, ngày {DateTime.Now:dd} tháng {DateTime.Now:MM} năm {DateTime.Now:yyyy}",
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                TextAlign = ContentAlignment.TopRight,
                Dock = DockStyle.Right,
                Width = 430
            };

            pnlDualLetterhead.Controls.Add(lblAdminOrg);
            pnlDualLetterhead.Controls.Add(lblReportHeaderDate);

            Panel pnlAdminDivider = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(226, 232, 240) };

            Label lblAdminTitle = new Label
            {
                Text = "BIÊN BẢN BÁO CÁO KẾT QUẢ THẨM ĐỊNH VÀ PHÊ DUYỆT CHIẾN LƯỢC NHẬP HÀNG QUÝ 4/2026",
                Font = new Font("Segoe UI", 12.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 34
            };

            Label lblAdminSub = new Label
            {
                Text = "Ứng dụng Mô hình Ra quyết định Đa tiêu chí Saaty (AHP) & Trợ lý Trí tuệ Nhân tạo DSS Assistant",
                Font = new Font("Segoe UI", 9.2f, FontStyle.Italic),
                ForeColor = Color.FromArgb(71, 85, 105),
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Top,
                Height = 22
            };

            pnlCardAdmin.Controls.Add(lblAdminSub);
            pnlCardAdmin.Controls.Add(lblAdminTitle);
            pnlCardAdmin.Controls.Add(pnlAdminDivider);
            pnlCardAdmin.Controls.Add(pnlDualLetterhead);

            Panel pnlCardKPI;
            Panel pnlWrapKPI = CreateFormCardWrapper(122, out pnlCardKPI);
            pnlCardKPI.Padding = new Padding(0);
            pnlCardKPI.BorderStyle = BorderStyle.None;
            pnlCardKPI.BackColor = Color.Transparent;

            TableLayoutPanel pnlKPIGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 4,
                RowCount = 1,
                BackColor = Color.Transparent
            };
            pnlKPIGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            pnlKPIGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            pnlKPIGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));
            pnlKPIGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25f));

            var cardTop1 = CreateKPICard("🏆 PHƯƠNG ÁN TỐI ƯU (QUÁN QUÂN)", "Chưa có dữ liệu", "Hãy chạy mô hình DSS để tính từ SQL Server", Color.FromArgb(5, 150, 105), out lblReportTopAltVal, out lblReportTopAltSub);
            var cardCR = CreateKPICard("🎯 ĐỘ TIN CẬY TOÁN HỌC (SAATY)", "CR = ---", "Tính từ ma trận so sánh cặp trong CSDL", Color.FromArgb(37, 99, 235), out lblReportCRVal, out lblReportCRSub);
            var cardQty = CreateKPICard("📦 ĐỀ XUẤT QUY MÔ NHẬP KHO", "---", "Tính từ nhu cầu bán hàng lịch sử & tồn kho", Color.FromArgb(217, 119, 6), out lblReportQtyVal, out lblReportQtySub);
            var cardCap = CreateKPICard("💰 ĐIỀU PHỐI VỐN & GIẢI TỒN", "---", "Tính theo tỷ trọng điểm AHP & tồn kho thực tế", Color.FromArgb(220, 38, 38), out lblReportCapVal, out lblReportCapSub);

            pnlKPIGrid.Controls.Add(cardTop1, 0, 0);
            pnlKPIGrid.Controls.Add(cardCR, 1, 0);
            pnlKPIGrid.Controls.Add(cardQty, 2, 0);
            pnlKPIGrid.Controls.Add(cardCap, 3, 0);
            pnlCardKPI.Controls.Add(pnlKPIGrid);

            Panel pnlCardChart;
            Panel pnlWrapChart = CreateFormCardWrapper(315, out pnlCardChart);
            pnlCardChart.Padding = new Padding(18, 14, 18, 14);

            Label lblChartTitle = new Label
            {
                Text = "I. BIỂU ĐỒ TRỰC QUAN ĐIỂM SỐ AHP (AHP COMPOSITE SCORE VISUAL CHART):",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Dock = DockStyle.Top,
                Height = 24
            };

            Label lblChartSub = new Label
            {
                Text = "Thang đo chuẩn hóa AHP (0.00 - 1.00): trọng số các tiêu chí được tính từ ma trận so sánh cặp lưu trong SQL Server.",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Dock = DockStyle.Top,
                Height = 20
            };
            lblReportChartSub = lblChartSub;

            reportChartPanel = new AHPScoreChartPanel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            pnlCardChart.Controls.Add(reportChartPanel);
            pnlCardChart.Controls.Add(lblChartSub);
            pnlCardChart.Controls.Add(lblChartTitle);

            Panel pnlCardGrid;
            Panel pnlWrapGrid = CreateFormCardWrapper(285, out pnlCardGrid);
            pnlCardGrid.Padding = new Padding(18, 14, 18, 14);

            Label lblGridTitle = new Label
            {
                Text = "II. BẢNG XẾP HẠNG THỨ BẬC ĐỊNH LƯỢNG & ĐỊNH MỨC PHÊ DUYỆT (AHP QUANTITATIVE TABLE):",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Dock = DockStyle.Top,
                Height = 24
            };

            Label lblGridSub = new Label
            {
                Text = "Bảng dữ liệu trích xuất từ Mô hình phân tích thứ bậc Saaty AHP kết hợp Kho dữ liệu bán hàng lịch sử OLAP.",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Dock = DockStyle.Top,
                Height = 20
            };

            dgvReportTable = new DataGridView
            {
                Dock = DockStyle.Fill,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(226, 232, 240),
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Regular)
            };
            dgvReportTable.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvReportTable.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            dgvReportTable.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.2f, FontStyle.Bold);
            dgvReportTable.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dgvReportTable.RowTemplate.Height = 34;

            pnlCardGrid.Controls.Add(dgvReportTable);
            pnlCardGrid.Controls.Add(lblGridSub);
            pnlCardGrid.Controls.Add(lblGridTitle);

            Panel pnlCardDirectives;
            Panel pnlWrapDirectives = CreateFormCardWrapper(310, out pnlCardDirectives);
            pnlCardDirectives.Padding = new Padding(18, 14, 18, 14);

            Label lblDirTitle = new Label
            {
                Text = "III. QUYẾT NGHỊ ĐIỀU HÀNH KINH DOANH & KẾ HOẠCH HÀNH ĐỘNG CHIẾN LƯỢC (+AI ASSISTANT):",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Dock = DockStyle.Top,
                Height = 24
            };

            Label lblDirSub = new Label
            {
                Text = "Kế hoạch phân bổ ngân sách, định mức số lượng nhập kho và phương án xử lý hàng tồn kho theo khuyến nghị AI.",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Dock = DockStyle.Top,
                Height = 20
            };

            TableLayoutPanel pnlDirectiveCards = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.White
            };
            pnlDirectiveCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            pnlDirectiveCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            pnlDirectiveCards.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34f));

            var dirCard1 = CreateDirectiveCard(
                "🟢 1. PHÊ DUYỆT NHẬP CHỦ LỰC",
                "(Selected - Quy mô lớn)",
                "Chưa có dữ liệu. Hãy chạy mô hình DSS để hệ thống tính từ SQL Server.",
                Color.FromArgb(16, 185, 129),
                Color.FromArgb(240, 253, 244),
                out lblReportDirective1
            );

            var dirCard2 = CreateDirectiveCard(
                "🟡 2. NHẬP THĂM DÒ THỊ TRƯỜNG",
                "(Under Review - Thận trọng & Linh hoạt)",
                "Chưa có dữ liệu. Hãy chạy mô hình DSS để hệ thống tính từ SQL Server.",
                Color.FromArgb(245, 158, 11),
                Color.FromArgb(255, 251, 235),
                out lblReportDirective2
            );

            var dirCard3 = CreateDirectiveCard(
                "🔴 3. DỪNG NHẬP & XẢ TỒN THU HỒI VỐN",
                "(Rejected - Xả kho giải phóng dòng tiền)",
                "Chưa có dữ liệu. Hãy chạy mô hình DSS để hệ thống tính từ SQL Server.",
                Color.FromArgb(239, 68, 68),
                Color.FromArgb(254, 242, 242),
                out lblReportDirective3
            );

            pnlDirectiveCards.Controls.Add(dirCard1, 0, 0);
            pnlDirectiveCards.Controls.Add(dirCard2, 1, 0);
            pnlDirectiveCards.Controls.Add(dirCard3, 2, 0);

            pnlCardDirectives.Controls.Add(pnlDirectiveCards);
            pnlCardDirectives.Controls.Add(lblDirSub);
            pnlCardDirectives.Controls.Add(lblDirTitle);

            Panel pnlCardSignatures;
            Panel pnlWrapSignatures = CreateFormCardWrapper(230, out pnlCardSignatures);
            pnlCardSignatures.Padding = new Padding(18, 14, 18, 14);

            Label lblSigTitle = new Label
            {
                Text = "IV. HỘI ĐỒNG THẨM ĐỊNH & BAN ĐIỀU HÀNH KÝ PHÊ DUYỆT BÁO CÁO:",
                Font = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Dock = DockStyle.Top,
                Height = 24
            };

            TableLayoutPanel pnlSignaturesGrid = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 3,
                RowCount = 1,
                BackColor = Color.White
            };
            pnlSignaturesGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            pnlSignaturesGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33f));
            pnlSignaturesGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34f));

            Panel pnlSig1 = CreateSignatureBox("TRƯỞNG PHÒNG KINH DOANH", "(Đã thẩm định nhu cầu thị trường)", "✓ ĐÃ XÁC NHẬN", Color.FromArgb(37, 99, 235), "Nguyễn Văn A");
            Panel pnlSig2 = CreateSignatureBox("GIÁM ĐỐC TÀI CHÍNH (CFO)", "(Đã kiểm soát ngân sách & an toàn vốn)", "✓ ĐÃ THẨM TRA", Color.FromArgb(5, 150, 105), "Trần Thị B");
            Panel pnlSig3 = CreateCEORedSealBox("TỔNG GIÁM ĐỐC (CEO)", "(Phê duyệt thực thi toàn diện)", "Lê Hoàng C");

            pnlSignaturesGrid.Controls.Add(pnlSig1, 0, 0);
            pnlSignaturesGrid.Controls.Add(pnlSig2, 1, 0);
            pnlSignaturesGrid.Controls.Add(pnlSig3, 2, 0);

            Label lblReportFooter = new Label
            {
                Text = "Nơi nhận: - Ban Giám Đốc Tập đoàn; - Phòng Kế hoạch, Quản trị danh mục, Tài chính, Kinh doanh; - Lưu VT, Hồ sơ DSS Q4/2026.",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Dock = DockStyle.Bottom,
                Height = 22
            };

            pnlCardSignatures.Controls.Add(pnlSignaturesGrid);
            pnlCardSignatures.Controls.Add(lblReportFooter);
            pnlCardSignatures.Controls.Add(lblSigTitle);

            pnlReportScroll.Controls.Add(pnlWrapSignatures); // Card 6 (Dưới cùng)
            pnlReportScroll.Controls.Add(pnlWrapDirectives); // Card 5
            pnlReportScroll.Controls.Add(pnlWrapGrid);       // Card 4
            pnlReportScroll.Controls.Add(pnlWrapChart);      // Card 3
            pnlReportScroll.Controls.Add(pnlWrapKPI);        // Card 2
            pnlReportScroll.Controls.Add(pnlWrapAdmin);      // Card 1 (Trên cùng)

            // Hidden RichTextBox dự phòng
            rtbAI = new RichTextBox { Visible = false };

            tabAI.Controls.Add(pnlReportScroll);
            tabAI.Controls.Add(pnlReportToolbar);
            tabControl.TabPages.Add(tabAI);
        }

        private Panel CreateFormCardWrapper(int innerHeight, out Panel innerCard)
        {
            Panel pnlWrap = new Panel
            {
                Dock = DockStyle.Top,
                Height = innerHeight + 14,
                Padding = new Padding(0, 0, 0, 14),
                BackColor = Color.Transparent
            };

            innerCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            pnlWrap.Controls.Add(innerCard);
            return pnlWrap;
        }

        private Panel CreateKPICard(string title, string defaultVal, string defaultSub, Color accentColor, out Label lblVal, out Label lblSub)
        {
            Panel pnlWrap = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(4, 0, 4, 0),
                BackColor = Color.Transparent
            };

            Panel pnlInner = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12, 10, 12, 8)
            };

            Panel pnlTopBar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 3,
                BackColor = accentColor
            };

            Label lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 7.8f, FontStyle.Bold),
                ForeColor = accentColor,
                Dock = DockStyle.Top,
                Height = 18
            };

            lblVal = new Label
            {
                Text = defaultVal,
                Font = new Font("Segoe UI", 11.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Dock = DockStyle.Top,
                Height = 26
            };

            lblSub = new Label
            {
                Text = defaultSub,
                Font = new Font("Segoe UI", 8.2f, FontStyle.Regular),
                ForeColor = Color.FromArgb(100, 116, 139),
                Dock = DockStyle.Fill
            };

            pnlInner.Controls.Add(lblSub);
            pnlInner.Controls.Add(lblVal);
            pnlInner.Controls.Add(lblTitle);
            pnlInner.Controls.Add(pnlTopBar);

            pnlWrap.Controls.Add(pnlInner);
            return pnlWrap;
        }

        private Panel CreateDirectiveCard(string title, string subtitle, string defaultBody, Color borderColor, Color bgColor, out Label lblBody)
        {
            Panel pnlWrap = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(4, 0, 4, 0),
                BackColor = Color.Transparent
            };

            Panel pnlCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = bgColor,
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(12, 10, 12, 10)
            };

            Panel pnlAccent = new Panel
            {
                Dock = DockStyle.Left,
                Width = 5,
                BackColor = borderColor
            };

            Panel pnlContent = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(8, 0, 0, 0)
            };

            Label lblHeader = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = borderColor,
                Dock = DockStyle.Top,
                Height = 20
            };

            Label lblSub = new Label
            {
                Text = subtitle,
                Font = new Font("Segoe UI", 8.2f, FontStyle.Italic),
                ForeColor = Color.FromArgb(71, 85, 105),
                Dock = DockStyle.Top,
                Height = 18
            };

            lblBody = new Label
            {
                Text = defaultBody,
                Font = new Font("Segoe UI", 8.8f, FontStyle.Regular),
                ForeColor = Color.FromArgb(30, 41, 59),
                Dock = DockStyle.Fill
            };

            pnlContent.Controls.Add(lblBody);
            pnlContent.Controls.Add(lblSub);
            pnlContent.Controls.Add(lblHeader);

            pnlCard.Controls.Add(pnlContent);
            pnlCard.Controls.Add(pnlAccent);

            pnlWrap.Controls.Add(pnlCard);
            return pnlWrap;
        }

        private Panel CreateSignatureBox(string role, string responsibility, string badgeText, Color badgeColor, string signerName)
        {
            Panel pnl = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(6)
            };

            Label lblRole = new Label
            {
                Text = role,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Top,
                Height = 22
            };

            Label lblResp = new Label
            {
                Text = responsibility,
                Font = new Font("Segoe UI", 8.2f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Top,
                Height = 20
            };

            Label lblBadge = new Label
            {
                Text = badgeText,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = badgeColor,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };

            Label lblSigner = new Label
            {
                Text = signerName,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                TextAlign = ContentAlignment.BottomCenter,
                Dock = DockStyle.Bottom,
                Height = 24
            };

            pnl.Controls.Add(lblBadge);
            pnl.Controls.Add(lblSigner);
            pnl.Controls.Add(lblResp);
            pnl.Controls.Add(lblRole);
            return pnl;
        }

        private Panel CreateCEORedSealBox(string role, string responsibility, string signerName)
        {
            Panel pnl = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(6)
            };

            Label lblRole = new Label
            {
                Text = role,
                Font = new Font("Segoe UI", 9.2f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Top,
                Height = 22
            };

            Label lblResp = new Label
            {
                Text = responsibility,
                Font = new Font("Segoe UI", 8.2f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Top,
                Height = 20
            };

            Panel pnlSealWrapper = new Panel
            {
                Dock = DockStyle.Fill,
                Padding = new Padding(12, 4, 12, 4),
                BackColor = Color.Transparent
            };

            Panel pnlRedSeal = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(254, 242, 242),
                BorderStyle = BorderStyle.FixedSingle,
                Padding = new Padding(4)
            };

            Label lblSealText = new Label
            {
                Text = "★ TẬP ĐOÀN BÁN LẺ DSS RETAIL ★\n[ ĐÃ PHÊ DUYỆT THỰC THI ]\nMã chuẩn y: DSS-2026-Q4-ENT",
                Font = new Font("Segoe UI", 8f, FontStyle.Bold),
                ForeColor = Color.FromArgb(220, 38, 38), // Red Seal
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Fill
            };
            pnlRedSeal.Controls.Add(lblSealText);
            pnlSealWrapper.Controls.Add(pnlRedSeal);

            Label lblSigner = new Label
            {
                Text = signerName,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(30, 41, 59),
                TextAlign = ContentAlignment.BottomCenter,
                Dock = DockStyle.Bottom,
                Height = 24
            };

            pnl.Controls.Add(pnlSealWrapper);
            pnl.Controls.Add(lblSigner);
            pnl.Controls.Add(lblResp);
            pnl.Controls.Add(lblRole);
            return pnl;
        }

        private void RefreshReportData()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sqlRanking = @"
                        SELECT 
                            ranking AS [Hạng],
                            product_code AS [Mã SKU],
                            alternative_name AS [Phương Án / Sản Phẩm],
                            FORMAT(selling_price, '#,##0') + ' đ' AS [Giá Bán],
                            CAST(final_score AS DECIMAL(8,4)) AS [Điểm AHP],
                            decision_outcome AS [Phê Duyệt],
                            recommendation AS [Khuyến Nghị Chiến Lược],
                            comments AS [Đánh Giá Chi Tiết]
                        FROM dbo.vw_DSS_FinalRankingReport
                        WHERE session_id = 1
                        ORDER BY ranking ASC;";
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlRanking, conn))
                    {
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            DssSnapshot snap = DssEngine.LoadSnapshot(conn, 1);
                            currentSnapshot = snap;
                            FormatAIMemo(dt, snap);
                            MessageBox.Show("Đã làm mới dữ liệu Form Báo Cáo & AI Insights từ SQL Server.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Chưa có kết quả tính toán. Vui lòng nhấn 'Chạy Mô Hình' tại Tab 2.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }
            }
            catch (SqlException ex) when (DatabaseGuard.IsConnectionError(ex))
            {
                MessageBox.Show(DatabaseGuard.FriendlyConnectionError(ex), "Mất kết nối SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi làm mới báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FormatAIMemo(DataTable dtRank, DssSnapshot snap)
        {
            if (dtRank == null || dtRank.Rows.Count == 0 || snap == null || !snap.HasResults) return;

            AltAnalysis top = snap.Top!;
            int reviewQty = snap.Ranked.Where(a => a.Outcome == "UnderReview").Sum(a => a.SuggestedQty);
            int reviewCount = snap.Ranked.Count(a => a.Outcome == "UnderReview");
            var rejected = snap.Ranked.Where(a => a.Outcome == "Rejected").ToList();

            // 1. Thẻ KPI (toàn bộ từ snapshot SQL)
            if (lblReportTopAltVal != null) lblReportTopAltVal.Text = top.ShortName;
            if (lblReportTopAltSub != null)
                lblReportTopAltSub.Text = $"Điểm AHP: {top.Score:0.0000} / 1.0000 • {DssEngine.OutcomeLabel(top.Outcome)}";
            if (lblReportCRVal != null)
            {
                lblReportCRVal.Text = DssEngine.CrText(snap.Ahp);
                lblReportCRSub.Text = DssEngine.CrVerdict(snap.Ahp);
            }
            if (lblReportQtyVal != null)
            {
                lblReportQtyVal.Text = $"{top.SuggestedQty} sản phẩm {top.ShortName}";
                lblReportQtySub.Text = reviewCount > 0
                    ? $"+ {reviewQty} sản phẩm nhóm xem xét ({reviewCount} phương án)"
                    : "Không có phương án nhóm xem xét";
            }
            if (lblReportCapVal != null)
            {
                lblReportCapVal.Text = $"{top.BudgetShare * 100:0.#}% Ngân Sách MKT";
                lblReportCapSub.Text = rejected.Count > 0
                    ? $"Xả tồn: {string.Join(", ", rejected.Select(r => r.ShortName))}"
                    : "Không có phương án cần xả tồn";
            }

            // 2. Biểu đồ
            if (reportChartPanel != null) reportChartPanel.SetData(dtRank);
            if (lblReportChartSub != null)
                lblReportChartSub.Text = "Trọng số tiêu chí (tính từ ma trận Saaty): " + DssEngine.WeightsText(snap.Ahp);

            // 3. Bảng dữ liệu
            if (dgvReportTable != null)
            {
                dgvReportTable.DataSource = dtRank;
                if (dgvReportTable.Columns != null)
                {
                    void Fmt(string col, float fill, DataGridViewContentAlignment? al = null)
                    {
                        if (!dgvReportTable.Columns.Contains(col)) return;
                        dgvReportTable.Columns[col].FillWeight = fill;
                        if (al.HasValue) dgvReportTable.Columns[col].DefaultCellStyle.Alignment = al.Value;
                    }
                    Fmt("Hạng", 55, DataGridViewContentAlignment.MiddleCenter);
                    Fmt("Mã SKU", 110, DataGridViewContentAlignment.MiddleCenter);
                    Fmt("Phương Án / Sản Phẩm", 220);
                    Fmt("Giá Bán", 110, DataGridViewContentAlignment.MiddleRight);
                    Fmt("Điểm AHP", 90, DataGridViewContentAlignment.MiddleCenter);
                    if (dgvReportTable.Columns.Contains("Điểm AHP"))
                        dgvReportTable.Columns["Điểm AHP"].DefaultCellStyle.Font = new Font("Segoe UI", 9.2f, FontStyle.Bold);
                    Fmt("Phê Duyệt", 120, DataGridViewContentAlignment.MiddleCenter);
                }
            }

            // 4. Ba thẻ chỉ đạo (sinh từ dữ liệu)
            if (lblReportDirective1 != null) lblReportDirective1.Text = DssEngine.DirectiveSelected(snap);
            if (lblReportDirective2 != null) lblReportDirective2.Text = DssEngine.DirectiveReview(snap);
            if (lblReportDirective3 != null) lblReportDirective3.Text = DssEngine.DirectiveRejected(snap);

            if (rtbAI != null) rtbAI.Text = DssEngine.ExecutiveSummary(snap);
        }

        private void SetupTabWhatIf()
        {
            tabWhatIf = new TabPage("📈 3. Biến Động Thị Trường (What-If)");
            tabWhatIf.BackColor = Color.FromArgb(248, 250, 252);
            tabWhatIf.Padding = new Padding(16);

            Panel pnlCard = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Padding = new Padding(18)
            };

            // Top Header & Scenario Controller Panel
            Panel pnlTopControl = new Panel
            {
                Dock = DockStyle.Top,
                Height = 155,
                BackColor = Color.White
            };

            Label lblWhatIfHeader = new Label
            {
                Text = "BIẾN ĐỘNG THỊ TRƯỜNG & PHÂN TÍCH ĐỘ NHẠY (WHAT-IF SENSITIVITY ANALYSIS):",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(0, 0),
                AutoSize = true
            };

            Label lblWhatIfSub = new Label
            {
                Text = "💡 Chọn nhanh các kịch bản thị trường vĩ mô hoặc tùy chỉnh tham số để kiểm tra độ nhạy thứ bậc DSS theo thời gian thực:",
                Font = new Font("Segoe UI", 9f, FontStyle.Italic),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(0, 26),
                AutoSize = true
            };

            // Scenario Buttons Toolbar
            FlowLayoutPanel pnlScenarioBar = new FlowLayoutPanel
            {
                Location = new Point(0, 50),
                Size = new Size(1350, 44),
                BackColor = Color.White,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false
            };

            btnScenarioGaming = new Button
            {
                Text = "🔥 Kịch Bản 1: Bùng Nổ Gaming (+40% Nhu Cầu)",
                Size = new Size(310, 38),
                BackColor = Color.FromArgb(217, 119, 6), // Amber
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 10, 0)
            };
            btnScenarioGaming.FlatAppearance.BorderSize = 0;
            btnScenarioGaming.Click += (s, e) => ApplyWhatIfScenario(1);

            btnScenarioSupplyCrisis = new Button
            {
                Text = "⚡ Kịch Bản 2: Đứt Gãy Cung Ứng (Rủi Ro Tăng)",
                Size = new Size(290, 38),
                BackColor = Color.FromArgb(225, 29, 72), // Rose 600
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 10, 0)
            };
            btnScenarioSupplyCrisis.FlatAppearance.BorderSize = 0;
            btnScenarioSupplyCrisis.Click += (s, e) => ApplyWhatIfScenario(2);

            btnScenarioInflation = new Button
            {
                Text = "📉 Kịch Bản 3: Lạm Phát - Ưu Tiên Giá Rẻ",
                Size = new Size(280, 38),
                BackColor = Color.FromArgb(14, 165, 233), // Sky 500
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 10, 0)
            };
            btnScenarioInflation.FlatAppearance.BorderSize = 0;
            btnScenarioInflation.Click += (s, e) => ApplyWhatIfScenario(3);

            btnScenarioReset = new Button
            {
                Text = "🔄 Khôi Phục Dữ Liệu Gốc (Baseline)",
                Size = new Size(260, 38),
                BackColor = Color.FromArgb(71, 85, 105), // Slate 600
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 2, 0, 0)
            };
            btnScenarioReset.FlatAppearance.BorderSize = 0;
            btnScenarioReset.Click += (s, e) => ApplyWhatIfScenario(0);

            pnlScenarioBar.Controls.Add(btnScenarioGaming);
            pnlScenarioBar.Controls.Add(btnScenarioSupplyCrisis);
            pnlScenarioBar.Controls.Add(btnScenarioInflation);
            pnlScenarioBar.Controls.Add(btnScenarioReset);

            // Fine-Tuning Bar (FlowLayoutPanel prevents any overlapping at all DPI scales)
            FlowLayoutPanel pnlFineTuneBar = new FlowLayoutPanel
            {
                Location = new Point(0, 98),
                Size = new Size(1350, 48),
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(10, 8, 10, 8),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoScroll = false
            };

            Label lblCustomProduct = new Label
            {
                Text = "🎯 Tùy biến sản phẩm:",
                AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Margin = new Padding(0, 6, 6, 0)
            };
            cboWhatIfProduct = new ComboBox
            {
                Size = new Size(210, 28),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9f),
                Margin = new Padding(0, 2, 16, 0)
            };
            // Danh sách sản phẩm được nạp động từ CSDL tại PopulateWhatIfProducts (khi LoadInputData)

            Label lblCustomDemand = new Label
            {
                Text = "Nhu cầu (C3):",
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                Margin = new Padding(0, 6, 4, 0)
            };
            numWhatIfDemand = new NumericUpDown
            {
                Size = new Size(68, 26),
                DecimalPlaces = 2,
                Increment = 0.05m,
                Minimum = 0.05m,
                Maximum = 1.00m,
                Value = 0.95m,
                Font = new Font("Segoe UI", 9f),
                Margin = new Padding(0, 3, 16, 0)
            };

            Label lblCustomProfit = new Label
            {
                Text = "Biên LN (C1):",
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                Margin = new Padding(0, 6, 4, 0)
            };
            numWhatIfProfit = new NumericUpDown
            {
                Size = new Size(68, 26),
                DecimalPlaces = 2,
                Increment = 0.05m,
                Minimum = 0.05m,
                Maximum = 1.00m,
                Value = 0.90m,
                Font = new Font("Segoe UI", 9f),
                Margin = new Padding(0, 3, 16, 0)
            };

            Label lblCustomRisk = new Label
            {
                Text = "An toàn vốn (C4):",
                AutoSize = true,
                Font = new Font("Segoe UI", 9f),
                Margin = new Padding(0, 6, 4, 0)
            };
            numWhatIfRisk = new NumericUpDown
            {
                Size = new Size(68, 26),
                DecimalPlaces = 2,
                Increment = 0.05m,
                Minimum = 0.05m,
                Maximum = 1.00m,
                Value = 0.75m,
                Font = new Font("Segoe UI", 9f),
                Margin = new Padding(0, 3, 20, 0)
            };

            btnApplyCustomWhatIf = new Button
            {
                Text = "⚡ Áp Dụng Tham Số",
                Size = new Size(180, 32),
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 1, 0, 0)
            };
            btnApplyCustomWhatIf.FlatAppearance.BorderSize = 0;
            btnApplyCustomWhatIf.Click += (s, e) => ApplyCustomTuning();

            pnlFineTuneBar.Controls.Add(lblCustomProduct);
            pnlFineTuneBar.Controls.Add(cboWhatIfProduct);
            pnlFineTuneBar.Controls.Add(lblCustomDemand);
            pnlFineTuneBar.Controls.Add(numWhatIfDemand);
            pnlFineTuneBar.Controls.Add(lblCustomProfit);
            pnlFineTuneBar.Controls.Add(numWhatIfProfit);
            pnlFineTuneBar.Controls.Add(lblCustomRisk);
            pnlFineTuneBar.Controls.Add(numWhatIfRisk);
            pnlFineTuneBar.Controls.Add(btnApplyCustomWhatIf);

            pnlTopControl.Controls.Add(lblWhatIfHeader);
            pnlTopControl.Controls.Add(lblWhatIfSub);
            pnlTopControl.Controls.Add(pnlScenarioBar);
            pnlTopControl.Controls.Add(pnlFineTuneBar);

            // Bottom Advisory Box
            Panel pnlAdvisoryWrapper = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 82,
                BackColor = Color.FromArgb(240, 253, 250), // Mint/Emerald
                Padding = new Padding(16, 10, 16, 10)
            };
            pnlAdvisoryWrapper.Paint += (s, e) =>
            {
                using (Pen pen = new Pen(Color.FromArgb(52, 211, 153), 1.5f))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, pnlAdvisoryWrapper.Width - 1, pnlAdvisoryWrapper.Height - 1);
                }
            };

            Label lblAdvTitle = new Label
            {
                Text = "📊 ĐÁNH GIÁ ĐỘ NHẠY & TÁC ĐỘNG CHIẾN LƯỢC KINH DOANH (DSS SENSITIVITY IMPACT):",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(6, 95, 70),
                Dock = DockStyle.Top,
                Height = 22
            };

            lblWhatIfAdvisory = new Label
            {
                Text = "• Hệ thống đang ở Kịch Bản Gốc (Baseline). Nhấn chọn một trong các kịch bản ở trên để xem sự đảo chiều thứ hạng và biến động điểm số AHP trên sơ đồ.",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular),
                ForeColor = Color.FromArgb(15, 23, 42),
                Dock = DockStyle.Fill
            };

            pnlAdvisoryWrapper.Controls.Add(lblWhatIfAdvisory);
            pnlAdvisoryWrapper.Controls.Add(lblAdvTitle);

            // Container for Detailed Diagrams (dock Fill - Takes all central area)
            Panel pnlDiagramContainer = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White
            };

            Panel pnlDiagToolbar = new Panel
            {
                Dock = DockStyle.Top,
                Height = 38,
                BackColor = Color.FromArgb(248, 250, 252),
                Padding = new Padding(12, 3, 12, 3)
            };
            pnlDiagToolbar.Paint += (s, e) =>
            {
                using (Pen p = new Pen(Color.FromArgb(226, 232, 240), 1f))
                {
                    e.Graphics.DrawLine(p, 0, pnlDiagToolbar.Height - 1, pnlDiagToolbar.Width, pnlDiagToolbar.Height - 1);
                }
            };

            Label lblDiagTitle = new Label
            {
                Text = "📈 TRỰC QUAN HÓA CẤU TRÚC AHP & BIỂU ĐỒ ĐỐI CHIẾU ĐỘ NHẠY:",
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Dock = DockStyle.Left,
                AutoSize = true,
                Padding = new Padding(0, 6, 10, 0)
            };

            btnViewHierarchyTree = new Button
            {
                Text = "🌲 Sơ Đồ Cây Cấu Trúc Thứ Bậc AHP",
                Size = new Size(270, 32),
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(241, 245, 249),
                ForeColor = Color.FromArgb(51, 65, 85),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnViewHierarchyTree.FlatAppearance.BorderSize = 1;
            btnViewHierarchyTree.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);

            btnViewDeltaChart = new Button
            {
                Text = "📈 Biểu Đồ Đường Đối Chiếu Độ Nhạy (Line Chart)",
                Size = new Size(330, 32),
                Dock = DockStyle.Right,
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            btnViewDeltaChart.FlatAppearance.BorderSize = 0;

            btnViewDeltaChart.Click += (s, e) =>
            {
                if (whatIfDiagramPanel != null)
                {
                    whatIfDiagramPanel.ViewMode = 0;
                    btnViewDeltaChart.BackColor = Color.FromArgb(79, 70, 229);
                    btnViewDeltaChart.ForeColor = Color.White;
                    btnViewDeltaChart.FlatAppearance.BorderSize = 0;

                    btnViewHierarchyTree.BackColor = Color.FromArgb(241, 245, 249);
                    btnViewHierarchyTree.ForeColor = Color.FromArgb(51, 65, 85);
                    btnViewHierarchyTree.FlatAppearance.BorderSize = 1;
                    btnViewHierarchyTree.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
                    whatIfDiagramPanel.Invalidate();
                }
            };

            btnViewHierarchyTree.Click += (s, e) =>
            {
                if (whatIfDiagramPanel != null)
                {
                    whatIfDiagramPanel.ViewMode = 1;
                    btnViewHierarchyTree.BackColor = Color.FromArgb(16, 185, 129);
                    btnViewHierarchyTree.ForeColor = Color.White;
                    btnViewHierarchyTree.FlatAppearance.BorderSize = 0;

                    btnViewDeltaChart.BackColor = Color.FromArgb(241, 245, 249);
                    btnViewDeltaChart.ForeColor = Color.FromArgb(51, 65, 85);
                    btnViewDeltaChart.FlatAppearance.BorderSize = 1;
                    btnViewDeltaChart.FlatAppearance.BorderColor = Color.FromArgb(203, 213, 225);
                    whatIfDiagramPanel.Invalidate();
                }
            };

            pnlDiagToolbar.Controls.Add(lblDiagTitle);
            pnlDiagToolbar.Controls.Add(btnViewDeltaChart);
            pnlDiagToolbar.Controls.Add(new Panel { Dock = DockStyle.Right, Width = 8, BackColor = Color.Transparent });
            pnlDiagToolbar.Controls.Add(btnViewHierarchyTree);

            whatIfDiagramPanel = new WhatIfDetailDiagramPanel
            {
                Dock = DockStyle.Fill
            };

            pnlDiagramContainer.Controls.Add(whatIfDiagramPanel);
            pnlDiagramContainer.Controls.Add(pnlDiagToolbar);

            Panel pnlBotSpacer = new Panel { Dock = DockStyle.Bottom, Height = 8, BackColor = Color.White };

            pnlCard.Controls.Add(pnlDiagramContainer);
            pnlCard.Controls.Add(pnlBotSpacer);
            pnlCard.Controls.Add(pnlAdvisoryWrapper);
            pnlCard.Controls.Add(pnlTopControl);

            pnlTopControl.SendToBack();
            pnlAdvisoryWrapper.SendToBack();
            pnlBotSpacer.SendToBack();
            pnlDiagramContainer.BringToFront();

            tabWhatIf.Controls.Add(pnlCard);
            tabControl.TabPages.Add(tabWhatIf);
        }


        private class WhatIfProductItem
        {
            public int AltId { get; set; }
            public string Name { get; set; } = "";
            public override string ToString() => Name;
        }

        private void PopulateWhatIfProducts(SqlConnection conn)
        {
            if (cboWhatIfProduct == null) return;
            cboWhatIfProduct.Items.Clear();
            using (SqlCommand cmd = new SqlCommand("SELECT alternative_id, alternative_name, alternative_code FROM dbo.ALTERNATIVES WHERE session_id = 1 ORDER BY alternative_id", conn))
            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    int id = rd.GetInt32(0);
                    string name = rd.GetString(1);
                    string code = rd.GetString(2);
                    cboWhatIfProduct.Items.Add(new WhatIfProductItem { AltId = id, Name = $"{name} ({code})" });
                }
            }
            if (cboWhatIfProduct.Items.Count > 0)
                cboWhatIfProduct.SelectedIndex = 0;
        }

        private void RecordBaselineRanks(DataTable dt)
        {
            if (dt == null) return;
            baselineRanks.Clear();
            foreach (DataRow r in dt.Rows)
            {
                string sku = r["Mã SKU"]?.ToString() ?? "";
                if (int.TryParse(r["Hạng"]?.ToString(), out int rk) &&
                    decimal.TryParse(r["Điểm AHP"]?.ToString(), out decimal sc))
                {
                    baselineRanks[sku] = (rk, sc);
                }
            }
        }

        private void ApplyWhatIfScenario(int scenario)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string scenarioName = "";
                    string advisory = "";

                    if (scenario == 0)
                    {
                        // Khôi phục Baseline: tính lại chuẩn hóa và điểm từ giá trị thô gốc trong SQL Server
                        DssEngine.RefreshNormalization(conn, 1);
                        var snap = DssEngine.Recalculate(conn, 1);
                        currentSnapshot = snap;
                        if (whatIfDiagramPanel != null) whatIfDiagramPanel.AhpInfo = snap.Ahp;
                        scenarioName = "Khôi Phục Kịch Bản Gốc (Baseline)";
                        advisory = $"• Đã khôi phục toàn bộ điểm chuẩn hóa về dữ liệu thô gốc trong SQL Server.\n" +
                                   $"• Phương án dẫn đầu hiện tại: [{snap.Top?.ShortName}] (Điểm AHP: {snap.Top?.Score:0.0000}).";
                        ExecuteWhatIfUpdate(conn, "", scenarioName, advisory);
                        return;
                    }

                    // Reset về baseline trước khi áp dụng kịch bản
                    DssEngine.RefreshNormalization(conn, 1);

                    string sqlScenarioUpdate = "";
                    if (scenario == 1)
                    {
                        scenarioName = "Bùng Nổ Nhu Cầu Thị Trường (+40% Tiêu Chí Nhu Cầu)";
                        sqlScenarioUpdate = @"
                            UPDATE s
                            SET s.normalized_score = CASE WHEN s.normalized_score * 1.40 > 1.0 THEN 1.0 ELSE s.normalized_score * 1.40 END
                            FROM dbo.ALTERNATIVE_SCORES s
                            INNER JOIN dbo.CRITERIA c ON c.criterion_id = s.criterion_id
                            WHERE s.session_id = 1 AND c.criterion_code = 'C_DEMAND';";
                        advisory = "• KỊCH BẢN: Thị trường tăng trưởng mạnh, chỉ số nhu cầu tiêu thụ (C_DEMAND) tăng +40%.\n" +
                                   "• TÁC ĐỘNG: Mô hình khuếch đại điểm số cho các phương án có thị phần và nhu cầu cao.";
                    }
                    else if (scenario == 2)
                    {
                        scenarioName = "Đứt Gãy Cung Ứng & Rủi Ro Tồn Kho Gia Tăng";
                        sqlScenarioUpdate = @"
                            UPDATE s
                            SET s.normalized_score = s.normalized_score * 0.70
                            FROM dbo.ALTERNATIVE_SCORES s
                            INNER JOIN dbo.CRITERIA c ON c.criterion_id = s.criterion_id
                            WHERE s.session_id = 1 AND c.criterion_code = 'C_RISK';";
                        advisory = "• KỊCH BẢN: Chuỗi cung ứng bị gián đoạn, rủi ro tồn kho tăng cao (giảm 30% điểm an toàn vốn C_RISK).\n" +
                                   "• TÁC ĐỘNG: Các phương án có chu kỳ tồn kho lớn hoặc thời gian giao hàng dài sẽ bị hạ xếp hạng.";
                    }
                    else if (scenario == 3)
                    {
                        scenarioName = "Lạm Phát & Thắt Chặt Chi Tiêu (Ưu Tiên Chi Phí & Giá)";
                        sqlScenarioUpdate = @"
                            UPDATE s
                            SET s.normalized_score = CASE WHEN s.normalized_score * 1.30 > 1.0 THEN 1.0 ELSE s.normalized_score * 1.30 END
                            FROM dbo.ALTERNATIVE_SCORES s
                            INNER JOIN dbo.CRITERIA c ON c.criterion_id = s.criterion_id
                            WHERE s.session_id = 1 AND c.criterion_code IN ('C_MKTCOST', 'C_PROFIT');";
                        advisory = "• KỊCH BẢN: Người tiêu dùng thắt chặt chi tiêu, ưu tiên sản phẩm có biên LN tốt và tiết kiệm chi phí marketing.\n" +
                                   "• TÁC ĐỘNG: Nhóm sản phẩm tối ưu chi phí tiếp thị và biên lợi nhuận cao được cải thiện vị thế.";
                    }

                    ExecuteWhatIfUpdate(conn, sqlScenarioUpdate, scenarioName, advisory);
                }
            }
            catch (SqlException ex) when (DatabaseGuard.IsConnectionError(ex))
            {
                MessageBox.Show(DatabaseGuard.FriendlyConnectionError(ex), "Mất kết nối SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi chạy kịch bản What-If: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyCustomTuning()
        {
            try
            {
                if (cboWhatIfProduct == null || cboWhatIfProduct.SelectedItem is not WhatIfProductItem item)
                {
                    MessageBox.Show("Vui lòng chọn một sản phẩm để tùy biến tham số.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                int altId = item.AltId;
                string prodName = item.Name.Split('(')[0].Trim();
                decimal demandVal = numWhatIfDemand.Value;
                decimal profitVal = numWhatIfProfit.Value;
                decimal riskVal = numWhatIfRisk.Value;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string sqlCustom = $@"
                        UPDATE s SET s.normalized_score = {profitVal}
                        FROM dbo.ALTERNATIVE_SCORES s
                        INNER JOIN dbo.CRITERIA c ON c.criterion_id = s.criterion_id
                        WHERE s.session_id = 1 AND s.alternative_id = {altId} AND c.criterion_code = 'C_PROFIT';

                        UPDATE s SET s.normalized_score = {demandVal}
                        FROM dbo.ALTERNATIVE_SCORES s
                        INNER JOIN dbo.CRITERIA c ON c.criterion_id = s.criterion_id
                        WHERE s.session_id = 1 AND s.alternative_id = {altId} AND c.criterion_code = 'C_DEMAND';

                        UPDATE s SET s.normalized_score = {riskVal}
                        FROM dbo.ALTERNATIVE_SCORES s
                        INNER JOIN dbo.CRITERIA c ON c.criterion_id = s.criterion_id
                        WHERE s.session_id = 1 AND s.alternative_id = {altId} AND c.criterion_code = 'C_RISK';";

                    string advisory = $"• KỊCH BẢN TÙY CHỈNH: Điều chỉnh tham số thị trường cho [{prodName}].\n" +
                                      $"• Thiết lập mới: Nhu cầu = {demandVal:0.00}, Biên lợi nhuận = {profitVal:0.00}, An toàn vốn = {riskVal:0.00}.\n" +
                                      $"• Hệ thống đã tính lại toàn bộ điểm số AHP và thứ hạng tương ứng từ CSDL.";

                    ExecuteWhatIfUpdate(conn, sqlCustom, $"Tùy Chỉnh Cho {prodName}", advisory);
                }
            }
            catch (SqlException ex) when (DatabaseGuard.IsConnectionError(ex))
            {
                MessageBox.Show(DatabaseGuard.FriendlyConnectionError(ex), "Mất kết nối SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi áp dụng tham số: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExecuteWhatIfUpdate(string sqlUpdate, string scenarioTitle, string advisory)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    ExecuteWhatIfUpdate(conn, sqlUpdate, scenarioTitle, advisory);
                }
            }
            catch (SqlException ex) when (DatabaseGuard.IsConnectionError(ex))
            {
                MessageBox.Show(DatabaseGuard.FriendlyConnectionError(ex), "Mất kết nối SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi cập nhật What-If: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExecuteWhatIfUpdate(SqlConnection conn, string sqlUpdate, string scenarioTitle, string advisory)
        {
            if (!string.IsNullOrWhiteSpace(sqlUpdate))
            {
                using (SqlCommand cmd = new SqlCommand(sqlUpdate, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            // Tính toán lại xếp hạng từ ma trận trọng số Saaty và điểm số đã cập nhật
            string sqlRecalc = @"
                WITH WeightCalculated AS (
                    SELECT pc.criterion_id_1 AS criterion_id,
                           AVG(pc.saaty_value / col_sums.col_sum) AS criterion_weight
                    FROM dbo.PAIRWISE_COMPARISONS pc
                    INNER JOIN (
                        SELECT criterion_id_2, SUM(saaty_value) AS col_sum
                        FROM dbo.PAIRWISE_COMPARISONS WHERE session_id = 1 GROUP BY criterion_id_2
                    ) col_sums ON pc.criterion_id_2 = col_sums.criterion_id_2
                    WHERE pc.session_id = 1 GROUP BY pc.criterion_id_1
                ),
                AltScores AS (
                    SELECT s.alternative_id, SUM(w.criterion_weight * s.normalized_score) AS comp_score
                    FROM dbo.ALTERNATIVE_SCORES s
                    INNER JOIN WeightCalculated w ON s.criterion_id = w.criterion_id
                    WHERE s.session_id = 1 GROUP BY s.alternative_id
                ),
                Ranked AS (
                    SELECT alternative_id, comp_score,
                           DENSE_RANK() OVER (ORDER BY comp_score DESC) as rk
                    FROM AltScores
                )
                MERGE dbo.DECISION_RESULTS AS target
                USING Ranked AS source
                ON (target.session_id = 1 AND target.alternative_id = source.alternative_id)
                WHEN MATCHED THEN
                    UPDATE SET target.final_score = source.comp_score,
                               target.ranking = source.rk,
                               target.decision_outcome = CASE WHEN source.rk = 1 THEN 'Selected' WHEN source.comp_score >= 0.60 THEN 'UnderReview' ELSE 'Rejected' END,
                               target.decision_date = SYSDATETIME()
                WHEN NOT MATCHED THEN
                    INSERT (session_id, alternative_id, final_score, ranking, decision_outcome, comments)
                    VALUES (1, source.alternative_id, source.comp_score, source.rk, 
                            CASE WHEN source.rk = 1 THEN 'Selected' WHEN source.comp_score >= 0.60 THEN 'UnderReview' ELSE 'Rejected' END,
                            N'What-If simulation');";

            using (SqlCommand cmdRecalc = new SqlCommand(sqlRecalc, conn))
            {
                cmdRecalc.ExecuteNonQuery();
            }

            // Đảm bảo có baseline ranks từ CSDL nếu chưa có
            if (baselineRanks.Count == 0)
            {
                using (SqlCommand cmdBase = new SqlCommand(@"
                    SELECT ISNULL(p.product_code, alt.alternative_code), dr.ranking, dr.final_score
                    FROM dbo.DECISION_RESULTS dr
                    INNER JOIN dbo.ALTERNATIVES alt ON dr.alternative_id = alt.alternative_id
                    LEFT JOIN dbo.PRODUCTS p ON alt.product_id = p.product_id
                    WHERE dr.session_id = 1", conn))
                using (SqlDataReader rd = cmdBase.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string sku = rd.GetString(0);
                        int rk = rd.GetInt32(1);
                        decimal sc = rd.GetDecimal(2);
                        baselineRanks[sku] = (rk, sc);
                    }
                }
            }

            // Build Comparison DataTable
            DataTable dtCompare = new DataTable();
            dtCompare.Columns.Add("Mã SKU", typeof(string));
            dtCompare.Columns.Add("Tên Phương Án / Sản Phẩm", typeof(string));
            dtCompare.Columns.Add("Hạng Gốc", typeof(int));
            dtCompare.Columns.Add("Điểm AHP Gốc", typeof(string));
            dtCompare.Columns.Add("Hạng Mới", typeof(int));
            dtCompare.Columns.Add("Điểm AHP Mới", typeof(string));
            dtCompare.Columns.Add("Biến Động Hạng", typeof(string));
            dtCompare.Columns.Add("Đánh Giá Tác Động", typeof(string));

            string sqlQueryNew = @"
                SELECT 
                    ISNULL(p.product_code, alt.alternative_code) AS product_code,
                    alt.alternative_name,
                    dr.ranking,
                    dr.final_score,
                    dr.decision_outcome
                FROM dbo.DECISION_RESULTS dr
                INNER JOIN dbo.ALTERNATIVES alt ON dr.alternative_id = alt.alternative_id
                LEFT JOIN dbo.PRODUCTS p ON alt.product_id = p.product_id
                WHERE dr.session_id = 1
                ORDER BY dr.ranking ASC;";

            string winnerName = "";
            using (SqlDataAdapter da = new SqlDataAdapter(sqlQueryNew, conn))
            {
                DataTable dtNew = new DataTable();
                da.Fill(dtNew);
                foreach (DataRow r in dtNew.Rows)
                {
                    string sku = r["product_code"]?.ToString() ?? "";
                    string name = r["alternative_name"]?.ToString() ?? "";
                    int newRank = Convert.ToInt32(r["ranking"]);
                    decimal newScore = Convert.ToDecimal(r["final_score"]);

                    if (newRank == 1 && string.IsNullOrEmpty(winnerName))
                        winnerName = name.Split('(')[0].Trim();

                    int oldRank = baselineRanks.ContainsKey(sku) ? baselineRanks[sku].rank : newRank;
                    decimal oldScore = baselineRanks.ContainsKey(sku) ? baselineRanks[sku].score : newScore;

                    string shift = "";
                    if (newRank < oldRank) shift = $"🔺 TĂNG +{oldRank - newRank} Hạng";
                    else if (newRank > oldRank) shift = $"🔻 GIẢM -{newRank - oldRank} Hạng";
                    else shift = "➖ Giữ Nguyên Hạng";

                    string impact = newRank == 1 ? "★ PHƯƠNG ÁN TỐI ƯU SỐ 1 (ĐỀ XUẤT NHẬP HÀNG)" :
                                   newRank == 2 ? "Tiềm năng theo dõi (Nhập thăm dò)" :
                                   "Xếp sau (Ưu tiên giảm tồn / xả hàng)";

                    dtCompare.Rows.Add(sku, name, oldRank, $"{oldScore:0.0000}", newRank, $"{newScore:0.0000}", shift, impact);
                }
            }

            if (!string.IsNullOrEmpty(winnerName) && !advisory.Contains(winnerName) && !scenarioTitle.Contains("Gốc"))
            {
                advisory += $"\n• KẾT QUẢ MÔ HÌNH: [{winnerName}] đang giữ vị trí HẠNG 1 trong kịch bản này.";
            }

            if (lblWhatIfAdvisory != null)
            {
                lblWhatIfAdvisory.Text = advisory;
            }

            if (whatIfDiagramPanel != null)
            {
                whatIfDiagramPanel.AhpInfo = DssEngine.ComputeAhp(conn, 1);
                whatIfDiagramPanel.SetData(dtCompare);
            }

            try
            {
                currentSnapshot = DssEngine.Recalculate(conn, 1);
                if (pnlDecisionHub != null) pnlDecisionHub.RefreshData(currentSnapshot);
            }
            catch { }
        }

        private DssSnapshot? GetOrLoadSnapshot()
        {
            if (currentSnapshot != null && currentSnapshot.HasResults) return currentSnapshot;
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    var snap = DssEngine.LoadSnapshot(conn, 1);
                    if (snap.HasResults)
                    {
                        currentSnapshot = snap;
                        return snap;
                    }
                }
            }
            catch { }
            return null;
        }

        private void ExportWordDocument()
        {
            try
            {
                DssSnapshot? snap = GetOrLoadSnapshot();
                if (snap == null || !snap.HasResults)
                {
                    ExecuteDSSModel();
                    snap = currentSnapshot;
                }
                if (snap == null || !snap.HasResults)
                {
                    MessageBox.Show("Chưa có kết quả mô hình để xuất báo cáo. Vui lòng bấm 'Chạy Mô Hình' trước.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Microsoft Word Document (*.doc)|*.doc|Văn Bản Word (*.docx)|*.docx",
                    FileName = $"BaoCao_ThamDinh_DSS_QuangCao_{DateTime.Now:yyyyMMdd_HHmmss}.doc",
                    Title = "Xuất Biên Bản Thẩm Định Ra Microsoft Word"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    string wordContent = ReportBuilder.Word(snap, UserSession.CurrentUser?.FullName, UserSession.CurrentUser?.Role);
                    File.WriteAllText(sfd.FileName, wordContent, Encoding.UTF8);

                    try
                    {
                        Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                    }
                    catch { }

                    MessageBox.Show("Đã xuất hồ sơ báo cáo Microsoft Word thành công!\n\nFile lưu tại: " + sfd.FileName + "\n\nHệ thống đã tự động kích hoạt Microsoft Word cho bạn.", "Xuất Báo Cáo Word", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException ex) when (DatabaseGuard.IsConnectionError(ex))
            {
                MessageBox.Show(DatabaseGuard.FriendlyConnectionError(ex), "Mất kết nối SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất file Word: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ExportReport()
        {
            try
            {
                DssSnapshot? snap = GetOrLoadSnapshot();
                if (snap == null || !snap.HasResults)
                {
                    ExecuteDSSModel();
                    snap = currentSnapshot;
                }
                if (snap == null || !snap.HasResults)
                {
                    MessageBox.Show("Chưa có kết quả mô hình để xuất báo cáo. Vui lòng bấm 'Chạy Mô Hình' trước.", "Thông Báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                SaveFileDialog sfd = new SaveFileDialog
                {
                    Filter = "Báo Cáo Doanh Nghiệp HTML (In PDF / Trình Chiếu) (*.html)|*.html|Microsoft Word Document (*.doc)|*.doc|Văn Bản Báo Cáo Text (*.txt)|*.txt",
                    FileName = $"BaoCao_ThamDinh_DSS_QuangCao_{DateTime.Now:yyyyMMdd_HHmmss}.html",
                    Title = "Xuất Báo Cáo Thẩm Định & Phê Duyệt Chiến Lược DSS"
                };

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    bool isWord = sfd.FileName.EndsWith(".doc", StringComparison.OrdinalIgnoreCase) ||
                                  sfd.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase);
                    bool isHtml = sfd.FileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase) ||
                                  sfd.FileName.EndsWith(".htm", StringComparison.OrdinalIgnoreCase);

                    string content = isWord ? ReportBuilder.Word(snap, UserSession.CurrentUser?.FullName, UserSession.CurrentUser?.Role) :
                                     (isHtml ? ReportBuilder.Html(snap, UserSession.CurrentUser?.FullName, UserSession.CurrentUser?.Role) :
                                               ReportBuilder.Text(snap, UserSession.CurrentUser?.FullName, UserSession.CurrentUser?.Role));

                    File.WriteAllText(sfd.FileName, content, Encoding.UTF8);

                    try
                    {
                        Process.Start(new ProcessStartInfo(sfd.FileName) { UseShellExecute = true });
                    }
                    catch { }

                    string typeName = isWord ? "Microsoft Word (.doc)" : (isHtml ? "HTML In PDF" : "Text");
                    MessageBox.Show($"Đã xuất hồ sơ báo cáo {typeName} thành công!\n\nĐường dẫn: {sfd.FileName}\n\nHệ thống đã tự động mở tệp cho bạn.", "Xuất Báo Cáo Thành Công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (SqlException ex) when (DatabaseGuard.IsConnectionError(ex))
            {
                MessageBox.Show(DatabaseGuard.FriendlyConnectionError(ex), "Mất kết nối SQL Server", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi xuất file báo cáo: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private DataGridView CreateStyledDataGridView()
        {
            DataGridView dgv = new DataGridView
            {
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                EnableHeadersVisualStyles = false,
                RowHeadersVisible = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Font = new Font("Segoe UI", 9.5f, FontStyle.Regular)
            };

            dgv.RowTemplate.Height = 36;

            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(241, 245, 249);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(15, 23, 42);
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.Padding = new Padding(6, 2, 6, 2);
            dgv.ColumnHeadersDefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgv.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.ColumnHeadersHeight = 48;

            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(238, 242, 255);
            dgv.DefaultCellStyle.SelectionForeColor = Color.FromArgb(15, 23, 42);
            dgv.DefaultCellStyle.Padding = new Padding(8, 2, 8, 2);
            dgv.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dgv.DefaultCellStyle.WrapMode = DataGridViewTriState.False;
            dgv.GridColor = Color.FromArgb(241, 245, 249);

            return dgv;
        }

        private void RepositionHeaderControls()
        {
            if (headerPanel == null) return;
            int right = headerPanel.ClientSize.Width - 18;

            if (btnLogout != null)
            {
                btnLogout.Location = new Point(Math.Max(10, right - btnLogout.Width), 24);
                right -= (btnLogout.Width + 12);
            }

            if (pnlUserBadge != null)
            {
                pnlUserBadge.PerformLayout();
                pnlUserBadge.Location = new Point(Math.Max(10, right - pnlUserBadge.Width), 24);
                right -= (pnlUserBadge.Width + 12);
            }
        }

        private void UpdateCurrentUserDisplay()
        {
            if (UserSession.CurrentUser == null)
            {
                // Không tạo tài khoản mặc định: chỉ hiển thị người dùng đã đăng nhập thật từ SQL Server.
                if (lblUserName != null) lblUserName.Text = "Chưa đăng nhập";
                if (lblUserRole != null) lblUserRole.Text = "";
                RepositionHeaderControls();
                return;
            }

            var user = UserSession.CurrentUser;
            if (lblUserName != null)
            {
                lblUserName.Text = user.FullName;
            }
            if (lblUserRole != null)
            {
                lblUserRole.Text = $"[{user.Role}]";
                lblUserRole.BackColor = user.RoleBadgeColor;
            }

            RepositionHeaderControls();
        }

        private void BtnLogout_Click(object sender, EventArgs e)
        {
            string currentName = UserSession.CurrentUser?.FullName ?? "Người dùng";
            DialogResult confirm = MessageBox.Show(
                $"Bạn có chắc chắn muốn đăng xuất khỏi tài khoản [{currentName}]?",
                "Xác nhận đăng xuất hệ thống DSS",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirm != DialogResult.Yes) return;

            UserSession.Logout();
            this.Hide();

            using (LoginForm login = new LoginForm())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    UpdateCurrentUserDisplay();
                    this.Show();
                }
                else
                {
                    IsLogoutRequested = true;
                    this.Close();
                }
            }
        }
    }

    
    public class AHPScoreChartPanel : Panel
    {
        private DataTable data;

        public AHPScoreChartPanel()
        {
            this.DoubleBuffered = true;
            this.Padding = new Padding(12);
        }

        public void SetData(DataTable dt)
        {
            this.data = dt;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int w = this.Width;
            int h = this.Height;

            // 1. Header Title & Subtitle
            using (Font titleFont = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (SolidBrush titleBrush = new SolidBrush(Color.FromArgb(15, 23, 42)))
            {
                g.DrawString("📊 BIỂU ĐỒ CỘT ĐIỂM SỐ AHP (XẾP HẠNG PHƯƠNG ÁN):", titleFont, titleBrush, 14, 10);
            }

            using (Font scaleFont = new Font("Segoe UI", 7.8f, FontStyle.Regular))
            using (SolidBrush scaleBrush = new SolidBrush(Color.FromArgb(148, 163, 184)))
            {
                string note = "Thang điểm chuẩn hóa AHP (0.00 - 1.00)";
                SizeF szNote = g.MeasureString(note, scaleFont);
                g.DrawString(note, scaleFont, scaleBrush, w - szNote.Width - 14, 12);
            }

            if (data == null || data.Rows.Count == 0)
            {
                using (Font subFont = new Font("Segoe UI", 9.5f, FontStyle.Italic))
                using (SolidBrush subBrush = new SolidBrush(Color.FromArgb(148, 163, 184)))
                {
                    g.DrawString("Chưa có kết quả tính toán...", subFont, subBrush, 16, 45);
                }
                return;
            }

            int count = data.Rows.Count;
            int plotX = 40;
            int plotY = 46;
            int rightMargin = 16;
            int bottomMargin = 68;
            int plotW = Math.Max(180, w - plotX - rightMargin);
            int plotH = Math.Max(100, h - plotY - bottomMargin);

            // 2. Y-Axis Grid Lines & Labels (0.00 to 1.00)
            int gridSteps = 4;
            using (Pen gridPen = new Pen(Color.FromArgb(241, 245, 249), 1f) { DashStyle = DashStyle.Dash })
            using (Font axisFont = new Font("Segoe UI", 7.8f, FontStyle.Regular))
            using (SolidBrush axisBrush = new SolidBrush(Color.FromArgb(148, 163, 184)))
            using (StringFormat sfY = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center })
            {
                for (int step = 0; step <= gridSteps; step++)
                {
                    float scoreVal = 1.0f - (float)step / gridSteps;
                    float curY = plotY + step * ((float)plotH / gridSteps);
                    g.DrawLine(gridPen, plotX, curY, plotX + plotW, curY);
                    g.DrawString(scoreVal.ToString("0.00"), axisFont, axisBrush, plotX - 6, curY, sfY);
                }
            }

            // Bottom Axis Line
            using (Pen axisLinePen = new Pen(Color.FromArgb(203, 213, 225), 1.5f))
            {
                g.DrawLine(axisLinePen, plotX, plotY + plotH, plotX + plotW, plotY + plotH);
            }

            // 3. Render Vertical Columns
            float stepX = (float)plotW / count;
            int colW = Math.Max(14, Math.Min(56, (int)(stepX * 0.52f)));

            using (Font scoreFont = new Font("Segoe UI", 7.8f, FontStyle.Bold))
            using (Font rankFont = new Font("Segoe UI", 7.5f, FontStyle.Bold))
            using (Font nameFont = new Font("Segoe UI", 8f, FontStyle.Bold))
            using (Font pctFont = new Font("Segoe UI", 7.2f, FontStyle.Regular))
            using (StringFormat sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            using (StringFormat sfName = new StringFormat { Alignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
            {
                for (int i = 0; i < count; i++)
                {
                    DataRow r = data.Rows[i];
                    string fullName = r["Phương Án / Sản Phẩm"]?.ToString() ?? "";
                    string shortName = fullName.Split('(')[0].Trim();

                    float score = 0f;
                    float.TryParse(r["Điểm AHP"]?.ToString(), out score);

                    int rank = i + 1;
                    int.TryParse(r["Hạng"]?.ToString(), out rank);

                    float centerX = plotX + (i + 0.5f) * stepX;
                    int colX = (int)(centerX - colW / 2f);
                    int colH = (int)(plotH * Math.Max(0.04f, Math.Min(1.0f, score)));
                    int colY = plotY + plotH - colH;

                    // Ghost Column (Background Track to show full 1.00 height)
                    Rectangle ghostRect = new Rectangle(colX, plotY, colW, plotH);
                    using (SolidBrush ghostBrush = new SolidBrush(Color.FromArgb(248, 250, 252)))
                    {
                        FillTopRoundedRect(g, ghostBrush, ghostRect, 4);
                    }

                    // Column Colors & Gradients theo bậc xếp hạng
                    Color topColor, botColor, borderColor;
                    switch (rank)
                    {
                        case 1:
                            topColor = Color.FromArgb(245, 158, 11); // Amber-500
                            botColor = Color.FromArgb(217, 119, 6);  // Amber-600
                            borderColor = Color.FromArgb(251, 191, 36);
                            break;
                        case 2:
                            topColor = Color.FromArgb(16, 185, 129); // Emerald-500
                            botColor = Color.FromArgb(5, 150, 105);  // Emerald-600
                            borderColor = Color.FromArgb(52, 211, 153);
                            break;
                        case 3:
                            topColor = Color.FromArgb(59, 130, 246); // Blue-500
                            botColor = Color.FromArgb(37, 99, 235);  // Blue-600
                            borderColor = Color.FromArgb(96, 165, 250);
                            break;
                        case 4:
                            topColor = Color.FromArgb(99, 102, 241); // Indigo-500
                            botColor = Color.FromArgb(79, 70, 229);  // Indigo-600
                            borderColor = Color.FromArgb(129, 140, 248);
                            break;
                        case 5:
                            topColor = Color.FromArgb(168, 85, 247); // Purple-500
                            botColor = Color.FromArgb(147, 51, 234);  // Purple-600
                            borderColor = Color.FromArgb(192, 132, 252);
                            break;
                        case 6:
                            topColor = Color.FromArgb(14, 165, 233); // Sky-500
                            botColor = Color.FromArgb(2, 132, 199);   // Sky-600
                            borderColor = Color.FromArgb(56, 189, 248);
                            break;
                        case 7:
                            topColor = Color.FromArgb(249, 115, 22); // Orange-500
                            botColor = Color.FromArgb(234, 88, 12);  // Orange-600
                            borderColor = Color.FromArgb(251, 146, 60);
                            break;
                        default:
                            topColor = Color.FromArgb(244, 63, 94);  // Rose-500
                            botColor = Color.FromArgb(225, 29, 72);  // Rose-600
                            borderColor = Color.FromArgb(251, 113, 133);
                            break;
                    }

                    // Column Fill with Linear Gradient
                    Rectangle colRect = new Rectangle(colX, colY, colW, colH);
                    using (LinearGradientBrush colBrush = new LinearGradientBrush(
                        new Rectangle(colX, colY, colW, Math.Max(colH, 1)),
                        topColor, botColor, LinearGradientMode.Vertical))
                    using (Pen colBorder = new Pen(borderColor, 1f))
                    {
                        FillTopRoundedRect(g, colBrush, colRect, 4);
                        DrawTopRoundedRect(g, colBorder, colRect, 4);
                    }

                    // Floating Score Value on Top of Column
                    string valText = score.ToString("0.0000");
                    if (rank == 1) valText = "★ " + valText;

                    int valPillW = Math.Max(colW + 8, Math.Min(56, (int)stepX - 2));
                    int valPillH = 18;
                    int valPillX = (int)(centerX - valPillW / 2f);
                    int valPillY = colY - valPillH - 4;
                    if (valPillY < plotY - 14) valPillY = colY + 4; // if column reaches top

                    Color vBg = rank == 1 ? Color.FromArgb(254, 243, 199) : Color.FromArgb(241, 245, 249);
                    Color vBorder = rank == 1 ? Color.FromArgb(253, 230, 138) : Color.FromArgb(226, 232, 240);
                    Color vText = rank == 1 ? Color.FromArgb(180, 83, 9) : Color.FromArgb(30, 41, 59);

                    using (SolidBrush vBgB = new SolidBrush(vBg))
                    using (Pen vBorderP = new Pen(vBorder, 1f))
                    using (SolidBrush vTextB = new SolidBrush(vText))
                    {
                        FillRoundedRect(g, vBgB, new Rectangle(valPillX, valPillY, valPillW, valPillH), 3);
                        DrawRoundedRect(g, vBorderP, new Rectangle(valPillX, valPillY, valPillW, valPillH), 3);
                        g.DrawString(valText, scoreFont, vTextB, centerX, valPillY + valPillH / 2f, sfCenter);
                    }

                    // Axis Tick Mark
                    using (Pen tickPen = new Pen(Color.FromArgb(203, 213, 225), 1.5f))
                    {
                        g.DrawLine(tickPen, centerX, plotY + plotH, centerX, plotY + plotH + 4);
                    }

                    // X-Axis Item 1: Rank Pill
                    int rankPillW = Math.Min(colW + 8, Math.Min(50, (int)stepX - 2));
                    int rankPillH = 17;
                    int rankPillX = (int)(centerX - rankPillW / 2f);
                    int rankPillY = plotY + plotH + 6;

                    Color rBg = rank == 1 ? Color.FromArgb(254, 243, 199) : Color.FromArgb(241, 245, 249);
                    Color rBorder = rank == 1 ? Color.FromArgb(253, 230, 138) : Color.FromArgb(226, 232, 240);
                    Color rText = rank == 1 ? Color.FromArgb(180, 83, 9) : Color.FromArgb(71, 85, 105);
                    string rTitle = rank == 1 ? "★ #1" : $"#{rank}";

                    using (SolidBrush rBgB = new SolidBrush(rBg))
                    using (Pen rBorderP = new Pen(rBorder, 1f))
                    using (SolidBrush rTextB = new SolidBrush(rText))
                    {
                        FillRoundedRect(g, rBgB, new Rectangle(rankPillX, rankPillY, rankPillW, rankPillH), 3);
                        DrawRoundedRect(g, rBorderP, new Rectangle(rankPillX, rankPillY, rankPillW, rankPillH), 3);
                        g.DrawString(rTitle, rankFont, rTextB, centerX, rankPillY + rankPillH / 2f, sfCenter);
                    }

                    // X-Axis Item 2: Product Name
                    float labelW = Math.Max(40, stepX - 4);
                    using (SolidBrush nameB = new SolidBrush(Color.FromArgb(30, 41, 59)))
                    {
                        RectangleF nameRect = new RectangleF(centerX - labelW / 2f, rankPillY + rankPillH + 4, labelW, 16);
                        g.DrawString(shortName, nameFont, nameB, nameRect, sfName);
                    }

                    // X-Axis Item 3: Percentage Score
                    using (SolidBrush pctB = new SolidBrush(Color.FromArgb(100, 116, 139)))
                    {
                        RectangleF pctRect = new RectangleF(centerX - labelW / 2f, rankPillY + rankPillH + 20, labelW, 14);
                        g.DrawString($"{(score * 100):0.0}%", pctFont, pctB, pctRect, sfName);
                    }
                }
            }
        }

        private void FillTopRoundedRect(Graphics g, Brush brush, Rectangle bounds, int radius)
        {
            if (radius <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
            {
                g.FillRectangle(brush, bounds);
                return;
            }
            using (GraphicsPath path = new GraphicsPath())
            {
                int diameter = radius * 2;
                path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
                path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
                path.AddLine(bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }

        private void DrawTopRoundedRect(Graphics g, Pen pen, Rectangle bounds, int radius)
        {
            if (radius <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
            {
                g.DrawRectangle(pen, bounds);
                return;
            }
            using (GraphicsPath path = new GraphicsPath())
            {
                int diameter = radius * 2;
                path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
                path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
                path.AddLine(bounds.Right, bounds.Bottom, bounds.Left, bounds.Bottom);
                path.CloseFigure();
                g.DrawPath(pen, path);
            }
        }

        private void FillRoundedRect(Graphics g, Brush brush, Rectangle bounds, int radius)
        {
            if (radius <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
            {
                g.FillRectangle(brush, bounds);
                return;
            }
            using (GraphicsPath path = new GraphicsPath())
            {
                int diameter = radius * 2;
                path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
                path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
                path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
                g.FillPath(brush, path);
            }
        }

        private void DrawRoundedRect(Graphics g, Pen pen, Rectangle bounds, int radius)
        {
            if (radius <= 0 || bounds.Width <= 0 || bounds.Height <= 0)
            {
                g.DrawRectangle(pen, bounds);
                return;
            }
            using (GraphicsPath path = new GraphicsPath())
            {
                int diameter = radius * 2;
                path.AddArc(bounds.X, bounds.Y, diameter, diameter, 180, 90);
                path.AddArc(bounds.Right - diameter, bounds.Y, diameter, diameter, 270, 90);
                path.AddArc(bounds.Right - diameter, bounds.Bottom - diameter, diameter, diameter, 0, 90);
                path.AddArc(bounds.X, bounds.Bottom - diameter, diameter, diameter, 90, 90);
                path.CloseFigure();
                g.DrawPath(pen, path);
            }
        }
    }

    /// <summary>

    /// </summary>
    public class WhatIfDetailDiagramPanel : Panel
    {
        private DataTable data;

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int ViewMode { get; set; } = 0; // 0 = Delta Chart, 1 = Hierarchy Tree

        [System.ComponentModel.Browsable(false)]
        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public AhpResult? AhpInfo { get; set; }

        public WhatIfDetailDiagramPanel()
        {
            this.DoubleBuffered = true;
            this.BackColor = Color.White;
            this.Padding = new Padding(12);
        }

        public void SetData(DataTable dt)
        {
            this.data = dt;
            this.Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            if (ViewMode == 0)
            {
                DrawDeltaChart(g);
            }
            else
            {
                DrawHierarchyTree(g);
            }
        }

        private void DrawDeltaChart(Graphics g)
        {
            int w = this.Width;
            int h = this.Height;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // 1. Header Title
            using (Font titleFont = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (SolidBrush titleBrush = new SolidBrush(Color.FromArgb(15, 23, 42)))
            {
                g.DrawString("📈 BIỂU ĐỒ ĐƯỜNG ĐỐI CHIẾU ĐỘ NHẠY ĐIỂM SỐ AHP (CƠ SỞ vs. SAU BIẾN ĐỘNG):", titleFont, titleBrush, 14, 8);
            }

            // 2. Legend (Top-Right)
            int legendX = Math.Max(480, w - 460);
            using (Pen pBase = new Pen(Color.FromArgb(148, 163, 184), 2f) { DashStyle = DashStyle.Dash })
            using (SolidBrush bBaseDot = new SolidBrush(Color.FromArgb(148, 163, 184)))
            using (Font legFont = new Font("Segoe UI", 8.5f, FontStyle.Regular))
            using (SolidBrush legBrush = new SolidBrush(Color.FromArgb(71, 85, 105)))
            {
                g.DrawLine(pBase, legendX, 15, legendX + 22, 15);
                g.FillEllipse(bBaseDot, legendX + 8, 12, 6, 6);
                g.DrawString("Điểm Cơ Sở (Baseline)", legFont, legBrush, legendX + 28, 8);

                using (Pen pWhatIf = new Pen(Color.FromArgb(79, 70, 229), 2.5f))
                using (SolidBrush bWhatIfDot = new SolidBrush(Color.FromArgb(79, 70, 229)))
                {
                    int wX = legendX + 175;
                    g.DrawLine(pWhatIf, wX, 15, wX + 22, 15);
                    g.FillEllipse(bWhatIfDot, wX + 7, 11, 8, 8);
                    g.DrawString("Điểm Sau Biến Động (What-If)", legFont, legBrush, wX + 28, 8);
                }
            }

            if (data == null || data.Rows.Count == 0)
            {
                using (Font italicFont = new Font("Segoe UI", 9.5f, FontStyle.Italic))
                using (SolidBrush grayBrush = new SolidBrush(Color.FromArgb(148, 163, 184)))
                {
                    g.DrawString("Chưa có dữ liệu so sánh...", italicFont, grayBrush, 14, 40);
                }
                return;
            }

            int count = data.Rows.Count;
            int plotX = 54;
            int plotY = 46;
            int rightMargin = 40;
            int bottomMargin = 52; // Compact bottom margin for sleek X-axis labels
            int plotW = Math.Max(200, w - plotX - rightMargin);
            int plotH = Math.Max(120, h - plotY - bottomMargin);

            // 3. Y-Axis Grid Lines & Labels (0.00 to 1.00)
            int gridSteps = 5;
            using (Pen gridPen = new Pen(Color.FromArgb(241, 245, 249), 1f) { DashStyle = DashStyle.Dash })
            using (Font axisFont = new Font("Segoe UI", 8f, FontStyle.Regular))
            using (SolidBrush axisBrush = new SolidBrush(Color.FromArgb(148, 163, 184)))
            using (StringFormat sfY = new StringFormat { Alignment = StringAlignment.Far, LineAlignment = StringAlignment.Center })
            {
                for (int step = 0; step <= gridSteps; step++)
                {
                    float scoreVal = 1.0f - (float)step / gridSteps;
                    float curY = plotY + step * ((float)plotH / gridSteps);
                    g.DrawLine(gridPen, plotX, curY, plotX + plotW, curY);
                    g.DrawString(scoreVal.ToString("0.00"), axisFont, axisBrush, plotX - 8, curY, sfY);
                }
            }

            // Bottom Axis baseline
            using (Pen axisLinePen = new Pen(Color.FromArgb(203, 213, 225), 1.5f))
            {
                g.DrawLine(axisLinePen, plotX, plotY + plotH, plotX + plotW, plotY + plotH);
            }

            // 4. Data Extraction & Coordinate Calculation
            float stepX = (float)plotW / count;
            PointF[] basePts = new PointF[count];
            PointF[] whatIfPts = new PointF[count];
            double[] oldScores = new double[count];
            double[] newScores = new double[count];
            int[] oldRanks = new int[count];
            int[] newRanks = new int[count];
            string[] skuCodes = new string[count];
            string[] names = new string[count];
            string[] shifts = new string[count];

            for (int i = 0; i < count; i++)
            {
                DataRow row = data.Rows[i];
                names[i] = row["Tên Phương Án / Sản Phẩm"]?.ToString() ?? "";
                skuCodes[i] = row["Mã SKU"]?.ToString() ?? "";

                double oSc = 0.0, nSc = 0.0;
                double.TryParse(row["Điểm AHP Gốc"]?.ToString(), out oSc);
                double.TryParse(row["Điểm AHP Mới"]?.ToString(), out nSc);
                oldScores[i] = oSc;
                newScores[i] = nSc;

                int oRk = i + 1, nRk = i + 1;
                int.TryParse(row["Hạng Gốc"]?.ToString(), out oRk);
                int.TryParse(row["Hạng Mới"]?.ToString(), out nRk);
                oldRanks[i] = oRk;
                newRanks[i] = nRk;

                shifts[i] = row["Biến Động Hạng"]?.ToString() ?? "— Giữ Hạng";

                float ptX = plotX + (i + 0.5f) * stepX;
                float yBase = plotY + plotH - (float)(Math.Max(0.0, Math.Min(1.0, oldScores[i])) * plotH);
                float yWhatIf = plotY + plotH - (float)(Math.Max(0.0, Math.Min(1.0, newScores[i])) * plotH);

                basePts[i] = new PointF(ptX, yBase);
                whatIfPts[i] = new PointF(ptX, yWhatIf);
            }

            // 5. Vertical Drop Guides
            using (Pen vGuidePen = new Pen(Color.FromArgb(241, 245, 249), 1f) { DashStyle = DashStyle.Dot })
            {
                for (int i = 0; i < count; i++)
                {
                    g.DrawLine(vGuidePen, basePts[i].X, plotY, basePts[i].X, plotY + plotH);
                }
            }

            // 6. Area Gradient Fill under What-If Line
            if (count >= 2)
            {
                using (GraphicsPath areaPath = new GraphicsPath())
                {
                    areaPath.AddLine(whatIfPts[0].X, plotY + plotH, whatIfPts[0].X, whatIfPts[0].Y);
                    for (int i = 1; i < count; i++)
                    {
                        areaPath.AddLine(whatIfPts[i - 1], whatIfPts[i]);
                    }
                    areaPath.AddLine(whatIfPts[count - 1].X, whatIfPts[count - 1].Y, whatIfPts[count - 1].X, plotY + plotH);
                    areaPath.CloseFigure();

                    using (LinearGradientBrush areaBrush = new LinearGradientBrush(
                        new Rectangle(plotX, plotY, plotW, plotH + 2),
                        Color.FromArgb(32, 99, 102, 241),
                        Color.FromArgb(3, 99, 102, 241),
                        LinearGradientMode.Vertical))
                    {
                        g.FillPath(areaBrush, areaPath);
                    }
                }
            }

            // 7. Delta Connector Bars between Baseline and What-If
            for (int i = 0; i < count; i++)
            {
                double diff = newScores[i] - oldScores[i];
                Color dCol = diff > 0.0001 ? Color.FromArgb(16, 185, 129) :
                             diff < -0.0001 ? Color.FromArgb(239, 68, 68) :
                             Color.FromArgb(148, 163, 184);

                using (Pen dPen = new Pen(dCol, 1.5f) { DashStyle = DashStyle.Dot })
                {
                    g.DrawLine(dPen, basePts[i].X, basePts[i].Y, whatIfPts[i].X, whatIfPts[i].Y);
                }
            }

            // 8. Line 1: Baseline Series (Slate Dashed)
            if (count >= 2)
            {
                using (Pen baseLinePen = new Pen(Color.FromArgb(148, 163, 184), 2.2f) { DashStyle = DashStyle.Dash })
                {
                    g.DrawLines(baseLinePen, basePts);
                }
            }

            // 9. Line 2: What-If Series (Indigo Solid)
            if (count >= 2)
            {
                using (Pen whatIfLinePen = new Pen(Color.FromArgb(79, 70, 229), 3.2f) { LineJoin = LineJoin.Round })
                {
                    g.DrawLines(whatIfLinePen, whatIfPts);
                }
            }

            // 10. Baseline Nodes & Score Labels
            using (SolidBrush bPtBrush = new SolidBrush(Color.FromArgb(148, 163, 184)))
            using (SolidBrush wPtBrush = new SolidBrush(Color.White))
            using (Font baseFont = new Font("Segoe UI", 7.5f, FontStyle.Regular))
            using (SolidBrush baseTextBrush = new SolidBrush(Color.FromArgb(100, 116, 139)))
            using (StringFormat sfCenter = new StringFormat { Alignment = StringAlignment.Center })
            {
                for (int i = 0; i < count; i++)
                {
                    float bx = basePts[i].X;
                    float by = basePts[i].Y;
                    g.FillEllipse(bPtBrush, bx - 5, by - 5, 10, 10);
                    g.FillEllipse(wPtBrush, bx - 2.5f, by - 2.5f, 5, 5);

                    float textY = by + 6;
                    if (Math.Abs(by - whatIfPts[i].Y) < 22 && whatIfPts[i].Y > by)
                    {
                        textY = by - 16;
                    }
                    g.DrawString(oldScores[i].ToString("0.0000") + " (Gốc)", baseFont, baseTextBrush, bx, textY, sfCenter);
                }
            }

            // 11. Attached Data Callouts directly on What-If Line
            using (Font badgeTitleFont = new Font("Segoe UI", 7.5f, FontStyle.Bold))
            using (Font scoreValFont = new Font("Segoe UI", 8.2f, FontStyle.Bold))
            using (Font deltaValFont = new Font("Segoe UI", 7.5f, FontStyle.Bold))
            using (StringFormat sfCenter = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
            {
                for (int i = 0; i < count; i++)
                {
                    float wx = whatIfPts[i].X;
                    float wy = whatIfPts[i].Y;
                    bool isRank1 = newRanks[i] == 1;
                    double diff = newScores[i] - oldScores[i];

                    Color mainCol = isRank1 ? Color.FromArgb(245, 158, 11) : Color.FromArgb(79, 70, 229);
                    Color haloCol = isRank1 ? Color.FromArgb(60, 245, 158, 11) : Color.FromArgb(45, 79, 70, 229);

                    // Glowing Halo & Node
                    using (SolidBrush haloB = new SolidBrush(haloCol))
                    {
                        g.FillEllipse(haloB, wx - 11, wy - 11, 22, 22);
                    }
                    using (SolidBrush nodeB = new SolidBrush(mainCol))
                    {
                        g.FillEllipse(nodeB, wx - 6, wy - 6, 12, 12);
                    }
                    using (SolidBrush wB = new SolidBrush(Color.White))
                    {
                        g.FillEllipse(wB, wx - 2.5f, wy - 2.5f, 5, 5);
                    }

                    // Floating Callout attached directly to the node
                    int cw = 118;
                    int ch = 34;
                    bool placeAbove = wy - ch - 12 >= plotY - 4;
                    float calloutX = wx - cw / 2f;
                    if (calloutX < plotX) calloutX = plotX;
                    if (calloutX + cw > plotX + plotW) calloutX = plotX + plotW - cw;

                    float calloutY = placeAbove ? (wy - ch - 8) : (wy + 10);

                    Rectangle calloutRect = new Rectangle((int)calloutX, (int)calloutY, cw, ch);
                    Color calloutBg = isRank1 ? Color.FromArgb(254, 252, 232) : Color.FromArgb(255, 255, 255);
                    Color calloutBorder = isRank1 ? Color.FromArgb(245, 158, 11) : Color.FromArgb(203, 213, 225);

                    // Callout Box
                    using (SolidBrush cBgB = new SolidBrush(calloutBg))
                    using (Pen cBorderP = new Pen(calloutBorder, isRank1 ? 1.5f : 1f))
                    {
                        FillRoundedRect(g, cBgB, calloutRect, 4);
                        DrawRoundedRect(g, cBorderP, calloutRect, 4);

                        // Small pin pointer
                        PointF[] pinPts;
                        if (placeAbove)
                        {
                            pinPts = new PointF[] {
                                new PointF(wx - 4, calloutY + ch),
                                new PointF(wx + 4, calloutY + ch),
                                new PointF(wx, wy - 6)
                            };
                        }
                        else
                        {
                            pinPts = new PointF[] {
                                new PointF(wx - 4, calloutY),
                                new PointF(wx + 4, calloutY),
                                new PointF(wx, wy + 6)
                            };
                        }
                        g.FillPolygon(cBgB, pinPts);
                        g.DrawLine(cBorderP, pinPts[0], pinPts[2]);
                        g.DrawLine(cBorderP, pinPts[1], pinPts[2]);
                    }

                    // Content Line 1: Rank & Shift Tag
                    string rankTag = isRank1 ? "★ QUÁN QUÂN #1" :
                                     diff > 0.0001 ? $"HẠNG #{newRanks[i]} (▲ Tăng)" :
                                     diff < -0.0001 ? $"HẠNG #{newRanks[i]} (▼ Giảm)" :
                                     $"HẠNG #{newRanks[i]} (— Giữ)";
                    Color rankColor = isRank1 ? Color.FromArgb(180, 83, 9) :
                                      diff > 0.0001 ? Color.FromArgb(22, 101, 52) :
                                      diff < -0.0001 ? Color.FromArgb(153, 27, 27) :
                                      Color.FromArgb(71, 85, 105);

                    using (SolidBrush rankBrush = new SolidBrush(rankColor))
                    {
                        g.DrawString(rankTag, badgeTitleFont, rankBrush, calloutX + cw / 2f, calloutY + 8, sfCenter);
                    }

                    // Content Line 2: Score + Delta
                    string scoreStr = newScores[i].ToString("0.0000");
                    string deltaStr = diff > 0.0001 ? $"+{diff:0.0000}" :
                                      diff < -0.0001 ? $"{diff:0.0000}" :
                                      "0.0000";
                    string arrow = diff > 0.0001 ? "▲" : diff < -0.0001 ? "▼" : "—";
                    string deltaFull = $"({arrow}{deltaStr})";

                    using (SolidBrush scoreBrush = new SolidBrush(Color.FromArgb(15, 23, 42)))
                    using (SolidBrush deltaBrush = new SolidBrush(rankColor))
                    {
                        SizeF szScore = g.MeasureString(scoreStr, scoreValFont);
                        SizeF szDelta = g.MeasureString(deltaFull, deltaValFont);
                        float totalContentW = szScore.Width + szDelta.Width;
                        float startContentX = calloutX + (cw - totalContentW) / 2f;

                        g.DrawString(scoreStr, scoreValFont, scoreBrush, startContentX, calloutY + 18);
                        g.DrawString(deltaFull, deltaValFont, deltaBrush, startContentX + szScore.Width - 1, calloutY + 19);
                    }
                }
            }

            // 12. Minimalist X-Axis Product Labels (Sleek, compact, not bulky cards)
            using (Font nameFont = new Font("Segoe UI", 8.2f, FontStyle.Bold))
            using (Font skuFont = new Font("Segoe UI", 7.2f, FontStyle.Regular))
            using (SolidBrush nameB = new SolidBrush(Color.FromArgb(30, 41, 59)))
            using (SolidBrush skuB = new SolidBrush(Color.FromArgb(148, 163, 184)))
            using (StringFormat sfCenter = new StringFormat { Alignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
            {
                float labelW = Math.Max(100, stepX - 10);
                for (int i = 0; i < count; i++)
                {
                    float cx = basePts[i].X;

                    // Tick mark from axis
                    using (Pen tickPen = new Pen(Color.FromArgb(203, 213, 225), 1.5f))
                    {
                        g.DrawLine(tickPen, cx, plotY + plotH, cx, plotY + plotH + 4);
                    }

                    // Product Name (Clean, single line without bulky container box)
                    string displayName = names[i].Split('(')[0].Trim();
                    RectangleF nameRect = new RectangleF(cx - labelW / 2f, plotY + plotH + 6, labelW, 16);
                    g.DrawString(displayName, nameFont, nameB, nameRect, sfCenter);

                    // SKU Code
                    RectangleF skuRect = new RectangleF(cx - labelW / 2f, plotY + plotH + 22, labelW, 14);
                    g.DrawString(skuCodes[i], skuFont, skuB, skuRect, sfCenter);
                }
            }
        }

        private static void DrawArrowLine(Graphics g, Pen pen, Brush arrowBrush, float x1, float y1, float x2, float y2, float arrowSize = 6f)
        {
            g.DrawLine(pen, x1, y1, x2, y2);

            float dx = x2 - x1;
            float dy = y2 - y1;
            float len = (float)Math.Sqrt(dx * dx + dy * dy);
            if (len < 0.001f) return;

            float udx = dx / len;
            float udy = dy / len;
            float px = -udy;
            float py = udx;

            PointF tip = new PointF(x2, y2);
            PointF basePt = new PointF(x2 - udx * arrowSize, y2 - udy * arrowSize);
            PointF left = new PointF(basePt.X + px * (arrowSize * 0.45f), basePt.Y + py * (arrowSize * 0.45f));
            PointF right = new PointF(basePt.X - px * (arrowSize * 0.45f), basePt.Y - py * (arrowSize * 0.45f));

            g.FillPolygon(arrowBrush, new[] { tip, left, right });
        }

        private void DrawHierarchyTree(Graphics g)
        {
            int w = this.Width;
            int h = this.Height;

            int goalW = Math.Min(640, w - 60);
            int goalH = 42;
            int goalX = (w - goalW) / 2;
            int goalY = 14;

            // Level 2 (Criteria) layout
            int critCount = 5;
            int critMargin = 16;
            int critSpacing = Math.Max(8, Math.Min(16, (w - critMargin * 2 - critCount * 140) / Math.Max(1, critCount - 1)));
            int totalCritAvail = w - critMargin * 2;
            int critW = Math.Min(230, (totalCritAvail - (critCount - 1) * critSpacing) / critCount);
            if (critW < 90) critW = 90;
            int startCritX = (w - (critCount * critW + (critCount - 1) * critSpacing)) / 2;
            int critH = 56;
            int critY = Math.Max(88, (int)(h * 0.30f));

            // Level 3 (Alternatives) layout - DYNAMIC to guarantee ALL products fit on screen without being cut off!
            int altCount = (data != null && data.Rows.Count > 0) ? data.Rows.Count : 9;
            int altMargin = 12;
            int totalAltAvail = w - altMargin * 2;
            int altSpacing = Math.Max(5, Math.Min(10, (totalAltAvail - altCount * 80) / Math.Max(1, altCount - 1)));
            int altW = (totalAltAvail - (altCount - 1) * altSpacing) / altCount;
            if (altW > 155) altW = 155;
            if (altW < 60) altW = 60;
            int startAltX = Math.Max(altMargin, (w - (altCount * altW + (altCount - 1) * altSpacing)) / 2);
            int altH = 62;
            int altY = Math.Max(critY + critH + 45, (int)(h * 0.68f));
            if (altY + altH > h - 10) altY = h - altH - 10;

            // Connector Lines & Arrows: Level 1 (Goal) -> Level 2 (Criteria)
            using (Pen linePen = new Pen(Color.FromArgb(99, 102, 241), 1.5f))
            using (SolidBrush arrowBrush = new SolidBrush(Color.FromArgb(99, 102, 241)))
            {
                int goalBottomX = w / 2;
                int goalBottomY = goalY + goalH;

                for (int c = 0; c < critCount; c++)
                {
                    int cTopX = startCritX + c * (critW + critSpacing) + critW / 2;
                    int cTopY = critY;
                    DrawArrowLine(g, linePen, arrowBrush, goalBottomX, goalBottomY, cTopX, cTopY, 7f);
                }
            }

            // Criteria Colors array
            Color[] critColors = new[]
            {
                Color.FromArgb(37, 99, 235),
                Color.FromArgb(79, 70, 229),
                Color.FromArgb(124, 58, 237),
                Color.FromArgb(217, 119, 6),
                Color.FromArgb(225, 29, 72)
            };

            // Connector Lines & Arrows: Level 2 (Criteria) -> Level 3 (Alternatives)
            for (int c = 0; c < critCount; c++)
            {
                int cBotX = startCritX + c * (critW + critSpacing) + critW / 2;
                int cBotY = critY + critH;
                Color critCol = critColors[c % critColors.Length];

                using (Pen subLinePen = new Pen(Color.FromArgb(55, critCol.R, critCol.G, critCol.B), 1f))
                using (SolidBrush subArrowB = new SolidBrush(Color.FromArgb(140, critCol.R, critCol.G, critCol.B)))
                {
                    for (int a = 0; a < altCount; a++)
                    {
                        int ax = startAltX + a * (altW + altSpacing);
                        // Spread incoming criteria arrows across the top edge of each alternative box
                        float span = Math.Min(36f, altW * 0.45f);
                        float step = span / 4f;
                        float aTargetX = (ax + altW / 2f) - (span / 2f) + c * step;
                        float aTargetY = altY;

                        DrawArrowLine(g, subLinePen, subArrowB, cBotX, cBotY, aTargetX, aTargetY, 4.5f);
                    }
                }
            }

            // Level 1 Box: Goal
            using (SolidBrush goalBrush = new SolidBrush(Color.FromArgb(30, 41, 59)))
            using (Pen goalBorder = new Pen(Color.FromArgb(99, 102, 241), 1.5f))
            {
                FillRoundedRect(g, goalBrush, new Rectangle(goalX, goalY, goalW, goalH), 6);
                DrawRoundedRect(g, goalBorder, new Rectangle(goalX, goalY, goalW, goalH), 6);
            }
            using (Font goalFont = new Font("Segoe UI", 9.5f, FontStyle.Bold))
            using (SolidBrush goalTextB = new SolidBrush(Color.White))
            using (Font goalSubFont = new Font("Segoe UI", 8f, FontStyle.Regular))
            using (SolidBrush goalSubB = new SolidBrush(Color.FromArgb(165, 243, 252)))
            {
                string gTitle = "🎯 MỤC TIÊU: TỐI ƯU HÓA CHIẾN LƯỢC BÁN HÀNG & PHÂN BỔ HÀNG HÓA";
                SizeF sz = g.MeasureString(gTitle, goalFont);
                g.DrawString(gTitle, goalFont, goalTextB, goalX + (goalW - sz.Width) / 2, goalY + 4);

                string gSub = AhpInfo != null && AhpInfo.MatrixComplete
                    ? $"Cấu trúc thứ bậc AHP (Saaty) | {DssEngine.CrText(AhpInfo)} - {DssEngine.CrVerdict(AhpInfo)}"
                    : "Cấu trúc thứ bậc AHP (Saaty 1980)";
                SizeF szSub = g.MeasureString(gSub, goalSubFont);
                g.DrawString(gSub, goalSubFont, goalSubB, goalX + (goalW - szSub.Width) / 2, goalY + 22);
            }

            // Level 2 Boxes: 5 Criteria (nạp động từ AhpInfo nếu có)
            for (int c = 0; c < critCount; c++)
            {
                int cx = startCritX + c * (critW + critSpacing);
                Rectangle cRect = new Rectangle(cx, critY, critW, critH);

                Color cCol = critColors[c % critColors.Length];
                string cCode = $"C{c + 1}";
                string cName = $"Tiêu Chí {c + 1}";
                string cWeight = "--%";

                if (AhpInfo != null && c < AhpInfo.Criteria.Count)
                {
                    var ci = AhpInfo.Criteria[c];
                    cCode = $"C{c + 1}";
                    cName = ci.Name;
                    cWeight = $"{ci.Weight * 100:0.00}%";
                }

                using (SolidBrush bgB = new SolidBrush(Color.FromArgb(248, 250, 252)))
                using (Pen borderP = new Pen(cCol, 1.5f))
                {
                    FillRoundedRect(g, bgB, cRect, 5);
                    DrawRoundedRect(g, borderP, cRect, 5);
                }

                // Criteria tag badge [C1]
                using (SolidBrush tagB = new SolidBrush(cCol))
                using (Font tagFont = new Font("Segoe UI", 7.5f, FontStyle.Bold))
                using (SolidBrush textWhite = new SolidBrush(Color.White))
                {
                    Rectangle tagRect = new Rectangle(cx + 4, critY + 4, 26, 15);
                    FillRoundedRect(g, tagB, tagRect, 3);
                    g.DrawString(cCode, tagFont, textWhite, cx + 6, critY + 5);
                }

                // Criteria Name (constrained inside bounding rectangle with word-wrap / ellipsis to prevent any overlap)
                using (Font cNameFont = new Font("Segoe UI", critW < 140 ? 7.5f : 8f, FontStyle.Bold))
                using (SolidBrush cNameB = new SolidBrush(Color.FromArgb(15, 23, 42)))
                using (StringFormat sf = new StringFormat { Trimming = StringTrimming.EllipsisWord, FormatFlags = StringFormatFlags.LineLimit })
                {
                    RectangleF cNameRect = new RectangleF(cx + 33, critY + 4, critW - 37, 28);
                    g.DrawString(cName, cNameFont, cNameB, cNameRect, sf);
                }

                // Weight
                using (Font wFont = new Font("Segoe UI", 8f, FontStyle.Bold))
                using (SolidBrush wB = new SolidBrush(cCol))
                {
                    g.DrawString($"Trọng số: {cWeight}", wFont, wB, cx + 8, critY + critH - 18);
                }
            }

            // Level 3 Boxes: Alternatives (dynamic from current What-If data)
            for (int a = 0; a < altCount; a++)
            {
                int ax = startAltX + a * (altW + altSpacing);
                Rectangle aRect = new Rectangle(ax, altY, altW, altH);

                string altName = "Phương án " + (a + 1);
                string sku = "";
                int rank = a + 1;
                float score = 0f;

                if (data != null && a < data.Rows.Count)
                {
                    DataRow r = data.Rows[a];
                    altName = r["Tên Phương Án / Sản Phẩm"]?.ToString() ?? "";
                    sku = r["Mã SKU"]?.ToString() ?? "";
                    int.TryParse(r["Hạng Mới"]?.ToString(), out rank);
                    float.TryParse(r["Điểm AHP Mới"]?.ToString(), out score);
                }

                string shortName = altName.Split('(')[0].Trim();
                int maxChars = Math.Max(8, (altW - 20) / 7);
                if (shortName.Length > maxChars) shortName = shortName.Substring(0, maxChars - 2) + "..";

                Color aBg = rank == 1 ? Color.FromArgb(254, 243, 199) :
                            rank == 2 ? Color.FromArgb(239, 246, 255) :
                            Color.FromArgb(255, 255, 255);
                Color aBorder = rank == 1 ? Color.FromArgb(217, 119, 6) :
                                rank == 2 ? Color.FromArgb(59, 130, 246) :
                                Color.FromArgb(203, 213, 225);
                float borderWidth = rank == 1 ? 2f : 1f;

                using (SolidBrush bgB = new SolidBrush(aBg))
                using (Pen borderP = new Pen(aBorder, borderWidth))
                {
                    FillRoundedRect(g, bgB, aRect, 5);
                    DrawRoundedRect(g, borderP, aRect, 5);
                }

                // Rank & Short Name
                float titleFontSize = altW < 95 ? 7.5f : (altW < 120 ? 8f : 8.5f);
                using (Font rFont = new Font("Segoe UI", titleFontSize, FontStyle.Bold))
                using (SolidBrush rB = new SolidBrush(rank == 1 ? Color.FromArgb(180, 83, 9) : Color.FromArgb(15, 23, 42)))
                {
                    RectangleF rTitleRect = new RectangleF(ax + 5, altY + 4, altW - 10, 16);
                    using (StringFormat sfTitle = new StringFormat { Trimming = StringTrimming.EllipsisCharacter, FormatFlags = StringFormatFlags.NoWrap })
                    {
                        g.DrawString($"#{rank} {shortName}", rFont, rB, rTitleRect, sfTitle);
                    }
                }

                // Score text
                float scFontSize = altW < 100 ? 7.5f : 8f;
                using (Font scFont = new Font("Segoe UI", scFontSize, FontStyle.Bold))
                using (SolidBrush scB = new SolidBrush(rank == 1 ? Color.FromArgb(217, 119, 6) : Color.FromArgb(71, 85, 105)))
                {
                    string scoreText = altW < 105 ? $"AHP: {score:0.000}" : $"Điểm: {score:0.0000}";
                    g.DrawString(scoreText, scFont, scB, ax + 5, altY + 22);
                }

                // Outcome recommendation
                string rec = rank == 1 ? "★ NHẬP TỐI ƯU" :
                             rank == 2 ? "• NHẬP THĂM DÒ" :
                             "• XẢ TỒN / GIẢM";
                if (altW < 95)
                {
                    rec = rank == 1 ? "★ TỐI ƯU" : rank == 2 ? "• THĂM DÒ" : "• XẢ TỒN";
                }
                Color recColor = rank == 1 ? Color.FromArgb(180, 83, 9) :
                                 rank == 2 ? Color.FromArgb(37, 99, 235) :
                                 Color.FromArgb(148, 163, 184);

                using (Font recFont = new Font("Segoe UI", altW < 100 ? 6.5f : 7f, FontStyle.Bold))
                using (SolidBrush recB = new SolidBrush(recColor))
                {
                    g.DrawString(rec, recFont, recB, ax + 5, altY + 40);
                }
            }
        }

        private void FillRoundedRect(Graphics g, Brush brush, Rectangle bounds, int radius)
        {
            using (GraphicsPath path = CreateRoundedRectPath(bounds, radius))
            {
                g.FillPath(brush, path);
            }
        }

        private void DrawRoundedRect(Graphics g, Pen pen, Rectangle bounds, int radius)
        {
            using (GraphicsPath path = CreateRoundedRectPath(bounds, radius))
            {
                g.DrawPath(pen, path);
            }
        }

        private GraphicsPath CreateRoundedRectPath(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0 || bounds.Width <= 0 || bounds.Height <= 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            path.AddArc(arc, 180, 90);
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }

    public class SafeSplitContainer : SplitContainer
    {
        protected override void OnLayout(LayoutEventArgs e)
        {
            try
            {
                base.OnLayout(e);
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
                // Bỏ qua lỗi GDI+ tạm thời khi khởi tạo layout của SplitContainer
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            try
            {
                base.OnPaint(e);
            }
            catch (System.Runtime.InteropServices.ExternalException)
            {
            }
        }
    }
}
