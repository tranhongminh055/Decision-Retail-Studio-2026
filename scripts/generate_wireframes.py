# -*- coding: utf-8 -*-
"""
Script tao tron bo 10 ban ve Wireframe truc quan chat luong cao
He thong Ho tro Ra quyet dinh DSS Retail Decision Studio 2026
IS 402 A - Nhom 05 - Duy Tan University
"""

import os
import matplotlib.pyplot as plt
import matplotlib.patches as patches
from matplotlib.patches import FancyBboxPatch, Rectangle, Circle, Polygon
import numpy as np

os.makedirs('docs/wireframes', exist_ok=True)
plt.rcParams['font.sans-serif'] = ['Segoe UI', 'Arial', 'DejaVu Sans']
plt.rcParams['axes.unicode_minus'] = False

def setup_canvas(width=1000, height=650):
    fig, ax = plt.subplots(figsize=(10, 6.5), dpi=150)
    ax.set_xlim(0, width)
    ax.set_ylim(0, height)
    ax.axis('off')
    fig.patch.set_facecolor('#F1F5F9')
    return fig, ax

def draw_window_frame(ax, title="DSS Retail Decision Studio 2026 - Windows Forms Desktop Client"):
    bg = FancyBboxPatch((0, 0), 1000, 650, boxstyle="square,pad=0", facecolor='#F8FAFC', edgecolor='#64748B', linewidth=1.2, zorder=0)
    ax.add_patch(bg)
    title_bar = Rectangle((0, 620), 1000, 30, facecolor='#0F172A', edgecolor='#0F172A', zorder=1)
    ax.add_patch(title_bar)
    
    # Window controls
    ax.add_patch(Circle((980, 635), 5.5, facecolor='#EF4444', zorder=2))
    ax.add_patch(Circle((962, 635), 5.5, facecolor='#F59E0B', zorder=2))
    ax.add_patch(Circle((944, 635), 5.5, facecolor='#10B981', zorder=2))
    
    ax.text(20, 635, f"🪟 {title}", color='#F8FAFC', fontsize=9, fontweight='bold', va='center', zorder=2)

def draw_app_header(ax, active_user="Nguyen Huy Hoang [DecisionMaker]"):
    header = Rectangle((0, 545), 1000, 75, facecolor='#0F172A', zorder=1)
    ax.add_patch(header)
    
    logo = FancyBboxPatch((18, 558), 48, 48, boxstyle="round,pad=2", facecolor='#4F46E5', edgecolor='none', zorder=2)
    ax.add_patch(logo)
    ax.text(42, 582, "DSS", color='white', fontsize=11.5, fontweight='bold', ha='center', va='center', zorder=3)
    
    ax.text(78, 592, "DSS RETAIL DECISION STUDIO 2026", color='white', fontsize=12, fontweight='bold', va='center', zorder=2)
    ax.text(78, 570, "Hệ Thống Phân Tích Thứ Bậc Saaty (AHP) & Hỗ Trợ Ra Quyết Định Bán Hàng", color='#94A3B8', fontsize=8.2, va='center', zorder=2)
    
    user_pill = FancyBboxPatch((660, 563), 215, 36, boxstyle="round,pad=3", facecolor='#1E293B', edgecolor='#334155', zorder=2)
    ax.add_patch(user_pill)
    ax.text(767, 581, f"👤 {active_user}", color='#38BDF8', fontsize=8.2, fontweight='bold', ha='center', va='center', zorder=3)
    
    btn_logout = FancyBboxPatch((890, 563), 90, 36, boxstyle="round,pad=3", facecolor='#DC2626', edgecolor='none', zorder=2)
    ax.add_patch(btn_logout)
    ax.text(935, 581, "🚪 Đăng xuất", color='white', fontsize=8.2, fontweight='bold', ha='center', va='center', zorder=3)

def draw_tab_bar(ax, active_tab_idx=0):
    tab_bar = Rectangle((0, 505), 1000, 40, facecolor='#E2E8F0', zorder=1)
    ax.add_patch(tab_bar)
    
    tabs = [
        "📥 1. Đầu Vào (Inputs)",
        "📊 2. Kết Quả (Outputs)",
        "📈 3. What-If (Độ Nhạy)",
        "➕ 4. Thêm Sản Phẩm",
        "📦 5. OLAP Bán Hàng",
        "📋 6. Báo Cáo & AI"
    ]
    
    x = 8
    for idx, name in enumerate(tabs):
        w = 158
        is_active = (idx == active_tab_idx)
        bg_col = '#FFFFFF' if is_active else '#CBD5E1'
        txt_col = '#4F46E5' if is_active else '#475569'
        font_weight = 'bold' if is_active else 'normal'
        
        tab_patch = FancyBboxPatch((x, 508), w, 37, boxstyle="round,pad=2", facecolor=bg_col, edgecolor='#94A3B8' if is_active else 'none', zorder=2)
        ax.add_patch(tab_patch)
        ax.text(x + w/2, 526, name, color=txt_col, fontsize=8.2, fontweight=font_weight, ha='center', va='center', zorder=3)
        
        if is_active:
            indicator = Rectangle((x + 6, 505), w - 12, 4, facecolor='#4F46E5', zorder=4)
            ax.add_patch(indicator)
        x += w + 6

# =========================================================================
# 1. WIREFRAME GUI-01: SPLASH SCREEN FORM
# =========================================================================
def generate_gui_01():
    fig, ax = setup_canvas()
    
    # Outer dark desktop background
    ax.add_patch(Rectangle((0, 0), 1000, 650, facecolor='#0B1120', zorder=0))
    ax.text(500, 620, "WIREFRAME GUI-01: MÀN HÌNH KHỞI ĐỘNG (SPLASH SCREEN FORM)", color='#94A3B8', fontsize=11, fontweight='bold', ha='center', zorder=2)
    
    # Centered Splash Dialog
    dlg_x, dlg_y, dlg_w, dlg_h = 160, 110, 680, 470
    dialog = FancyBboxPatch((dlg_x, dlg_y), dlg_w, dlg_h, boxstyle="round,pad=8", facecolor='#0F172A', edgecolor='#38BDF8', linewidth=1.5, zorder=1)
    ax.add_patch(dialog)
    
    # 4 Colored square blocks logo
    bx = dlg_x + dlg_w/2 - 24
    by = dlg_y + dlg_h - 75
    ax.add_patch(Rectangle((bx, by), 20, 20, facecolor='#38BDF8', zorder=2))
    ax.add_patch(Rectangle((bx + 26, by), 20, 20, facecolor='#60A5FA', zorder=2))
    ax.add_patch(Rectangle((bx, by - 26), 20, 20, facecolor='#818CF8', zorder=2))
    ax.add_patch(Rectangle((bx + 26, by - 26), 20, 20, facecolor='#C084FC', zorder=2))
    
    # System Title
    ax.text(dlg_x + dlg_w/2, dlg_y + dlg_h - 120, "SPORT ZONE DSS PORTAL", color='#38BDF8', fontsize=11, fontweight='bold', ha='center', zorder=2)
    ax.text(dlg_x + dlg_w/2, dlg_y + dlg_h - 150, "DSS RETAIL DECISION STUDIO 2026", color='#FFFFFF', fontsize=16, fontweight='bold', ha='center', zorder=2)
    ax.text(dlg_x + dlg_w/2, dlg_y + dlg_h - 180, "Hệ Thống Phân Tích Thứ Bậc Saaty (AHP) & Hỗ Trợ Ra Quyết Định Bán Hàng", color='#94A3B8', fontsize=9.5, ha='center', zorder=2)
    
    # Version Tag
    ver_box = FancyBboxPatch((dlg_x + dlg_w/2 - 130, dlg_y + dlg_h - 225), 260, 26, boxstyle="round,pad=3", facecolor='#1E293B', edgecolor='#475569', zorder=2)
    ax.add_patch(ver_box)
    ax.text(dlg_x + dlg_w/2, dlg_y + dlg_h - 212, "Release v2.6.0 • .NET 10.0 • SQL Server DSS_SalesDB", color='#E2E8F0', fontsize=8, ha='center', va='center', zorder=3)
    
    # Progress Bar container
    pb_x, pb_y, pb_w, pb_h = dlg_x + 60, dlg_y + 140, dlg_w - 120, 20
    pb_bg = FancyBboxPatch((pb_x, pb_y), pb_w, pb_h, boxstyle="round,pad=2", facecolor='#1E293B', edgecolor='#475569', zorder=2)
    ax.add_patch(pb_bg)
    pb_fill = FancyBboxPatch((pb_x, pb_y), pb_w * 0.82, pb_h, boxstyle="round,pad=2", facecolor='#10B981', edgecolor='none', zorder=3)
    ax.add_patch(pb_fill)
    
    ax.text(pb_x, pb_y + 32, "⏳ Đang kiểm tra cấu trúc CSDL và nạp ma trận so sánh cặp Saaty...", color='#E2E8F0', fontsize=8.5, zorder=2)
    ax.text(pb_x + pb_w, pb_y + 32, "82%", color='#10B981', fontsize=8.5, fontweight='bold', ha='right', zorder=2)
    
    # DatabaseGuard Status Box
    dbg_box = FancyBboxPatch((pb_x, dlg_y + 70), pb_w, 42, boxstyle="round,pad=3", facecolor='#132338', edgecolor='#0284C7', zorder=2)
    ax.add_patch(dbg_box)
    ax.text(pb_x + 16, dlg_y + 91, "🛡️ DatabaseGuard Status: SQL Server Instance [.\\SQLEXPRESS25] Online • Ping: 3.2ms", color='#38BDF8', fontsize=8.5, va='center', zorder=3)
    ax.text(pb_x + pb_w - 16, dlg_y + 91, "🟢 CONNECTED", color='#4ADE80', fontsize=8.5, fontweight='bold', ha='right', va='center', zorder=3)
    
    # Academic footer note
    ax.text(dlg_x + dlg_w/2, dlg_y + 25, "Đại Học Duy Tân • Đồ Án IS 402 A - Nhóm 05 • GVHD: ThS. Nguyễn Quang Ánh", color='#64748B', fontsize=8, ha='center', zorder=2)
    
    plt.tight_layout()
    plt.savefig('docs/wireframes/GUI_01_Splash_Screen.png', dpi=150, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print("Generated GUI_01_Splash_Screen.png")

# =========================================================================
# 2. WIREFRAME GUI-02: LOGIN FORM
# =========================================================================
def generate_gui_02():
    fig, ax = setup_canvas()
    
    ax.add_patch(Rectangle((0, 0), 1000, 650, facecolor='#0B1120', zorder=0))
    ax.text(500, 620, "WIREFRAME GUI-02: MÀN HÌNH ĐĂNG NHẬP & XÁC THỰC BẢO MẬT (LOGIN FORM)", color='#94A3B8', fontsize=11, fontweight='bold', ha='center', zorder=2)
    
    # Main Login Box
    lx, ly, lw, lh = 110, 75, 780, 520
    box = FancyBboxPatch((lx, ly), lw, lh, boxstyle="round,pad=6", facecolor='#FFFFFF', edgecolor='#CBD5E1', linewidth=1.5, zorder=1)
    ax.add_patch(box)
    
    # Navy Top Header
    head_h = 75
    header = FancyBboxPatch((lx, ly + lh - head_h), lw, head_h, boxstyle="square,pad=0", facecolor='#0F172A', edgecolor='none', zorder=2)
    ax.add_patch(header)
    
    # 4 squares icon
    ax.add_patch(Rectangle((lx + 24, ly + lh - 48), 12, 12, facecolor='#38BDF8', zorder=3))
    ax.add_patch(Rectangle((lx + 40, ly + lh - 48), 12, 12, facecolor='#60A5FA', zorder=3))
    ax.add_patch(Rectangle((lx + 24, ly + lh - 64), 12, 12, facecolor='#818CF8', zorder=3))
    ax.add_patch(Rectangle((lx + 40, ly + lh - 64), 12, 12, facecolor='#C084FC', zorder=3))
    
    ax.text(lx + 68, ly + lh - 38, "CỔNG ĐĂNG NHẬP HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH (DSS PORTAL)", color='#94A3B8', fontsize=8, fontweight='bold', zorder=3)
    ax.text(lx + 68, ly + lh - 58, "ĐĂNG NHẬP TÀI KHOẢN NGƯỜI DÙNG DOANH NGHIỆP", color='#FFFFFF', fontsize=11.5, fontweight='bold', zorder=3)
    
    # Left: Form inputs (width 400)
    fx = lx + 36
    fy = ly + lh - head_h - 40
    
    ax.text(fx, fy, "Tên đăng nhập / Mã nhân viên (*):", color='#1E293B', fontsize=8.5, fontweight='bold', zorder=2)
    inp1 = FancyBboxPatch((fx, fy - 36), 360, 32, boxstyle="round,pad=2", facecolor='#F8FAFC', edgecolor='#CBD5E1', zorder=2)
    ax.add_patch(inp1)
    ax.text(fx + 12, fy - 20, "NV000 (Nguyễn Huy Hoàng)", color='#0F172A', fontsize=8.5, va='center', zorder=3)
    
    fy -= 70
    ax.text(fx, fy, "Mật khẩu bảo mật (*):", color='#1E293B', fontsize=8.5, fontweight='bold', zorder=2)
    inp2 = FancyBboxPatch((fx, fy - 36), 360, 32, boxstyle="round,pad=2", facecolor='#F8FAFC', edgecolor='#CBD5E1', zorder=2)
    ax.add_patch(inp2)
    ax.text(fx + 12, fy - 20, "••••••••••••", color='#0F172A', fontsize=11, va='center', zorder=3)
    ax.text(fx + 335, fy - 20, "👁", fontsize=10, va='center', zorder=3)
    
    fy -= 50
    # Remember check
    ax.add_patch(Rectangle((fx, fy - 12), 14, 14, facecolor='#4F46E5', edgecolor='#4F46E5', zorder=2))
    ax.text(fx + 3, fy - 4, "✓", color='white', fontsize=8, fontweight='bold', zorder=3)
    ax.text(fx + 24, fy - 5, "Ghi nhớ tài khoản trên thiết bị này", color='#475569', fontsize=8, zorder=2)
    
    fy -= 45
    # Login button
    btn_login = FancyBboxPatch((fx, fy - 38), 360, 38, boxstyle="round,pad=3", facecolor='#4F46E5', edgecolor='none', zorder=2)
    ax.add_patch(btn_login)
    ax.text(fx + 180, fy - 19, "🔐 ĐĂNG NHẬP HỆ THỐNG", color='white', fontsize=9.5, fontweight='bold', ha='center', va='center', zorder=3)
    
    fy -= 55
    ax.text(fx + 180, fy, "Chưa có tài khoản? 👉 Đăng ký tài khoản nhân viên mới", color='#2563EB', fontsize=8.2, ha='center', zorder=2)
    
    # Right: Quick accounts list (width 320)
    rx = lx + 430
    ry = ly + 25
    rw = 315
    rh = lh - head_h - 45
    
    q_card = FancyBboxPatch((rx, ry), rw, rh, boxstyle="round,pad=4", facecolor='#F8FAFC', edgecolor='#E2E8F0', zorder=2)
    ax.add_patch(q_card)
    
    ax.text(rx + rw/2, ry + rh - 22, "🎯 PHÍM CHỌN NHANH TÀI KHOẢN MẪU", color='#0F172A', fontsize=8.5, fontweight='bold', ha='center', zorder=3)
    ax.text(rx + rw/2, ry + rh - 38, "(1-Click Tự Động Điền Tài Khoản Kiểm Thử)", color='#64748B', fontsize=7.5, ha='center', zorder=3)
    
    demo_roles = [
        ("👑 DecisionMaker (NV000)", "Nguyễn Huy Hoàng • Toàn quyền duyệt DSS", '#DCFCE7', '#166534'),
        ("⚙️ Administrator (admin)", "Quản trị viên • Toàn quyền CSDL & Users", '#F1F5F9', '#334155'),
        ("📊 SalesManager (NV002)", "Trần Thị Mai • Phân tích OLAP & What-If", '#EFF6FF', '#1E40AF'),
        ("💼 SalesStaff (NV003)", "Lê Văn An • Tra cứu báo cáo DSS", '#FEF3C7', '#92400E')
    ]
    
    btn_y = ry + rh - 85
    for title, desc, bg_c, tx_c in demo_roles:
        d_btn = FancyBboxPatch((rx + 15, btn_y), rw - 30, 46, boxstyle="round,pad=3", facecolor=bg_c, edgecolor='#CBD5E1', zorder=3)
        ax.add_patch(d_btn)
        ax.text(rx + 25, btn_y + 30, title, color=tx_c, fontsize=8, fontweight='bold', zorder=4)
        ax.text(rx + 25, btn_y + 14, desc, color='#64748B', fontsize=7, zorder=4)
        btn_y -= 54
        
    ax.text(rx + rw/2, ry + 18, "🔒 Mật khẩu mặc định: 123456 (Băm SHA-256)", color='#94A3B8', fontsize=7.5, ha='center', zorder=3)
    
    plt.tight_layout()
    plt.savefig('docs/wireframes/GUI_02_Login_Form.png', dpi=150, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print("Generated GUI_02_Login_Form.png")

# =========================================================================
# 3. WIREFRAME GUI-03: REGISTER FORM
# =========================================================================
def generate_gui_03():
    fig, ax = setup_canvas()
    
    ax.add_patch(Rectangle((0, 0), 1000, 650, facecolor='#0B1120', zorder=0))
    ax.text(500, 620, "WIREFRAME GUI-03: MÀN HÌNH ĐĂNG KÝ TÀI KHOẢN MỚI (REGISTER FORM)", color='#94A3B8', fontsize=11, fontweight='bold', ha='center', zorder=2)
    
    lx, ly, lw, lh = 90, 65, 820, 530
    box = FancyBboxPatch((lx, ly), lw, lh, boxstyle="round,pad=6", facecolor='#FFFFFF', edgecolor='#CBD5E1', linewidth=1.5, zorder=1)
    ax.add_patch(box)
    
    # Header
    head_h = 70
    header = FancyBboxPatch((lx, ly + lh - head_h), lw, head_h, boxstyle="square,pad=0", facecolor='#0F172A', edgecolor='none', zorder=2)
    ax.add_patch(header)
    
    ax.text(lx + 30, ly + lh - 32, "CẤP PHÁT TÀI KHOẢN NGƯỜI DÙNG NỘI BỘ (USER REGISTRATION)", color='#38BDF8', fontsize=8, fontweight='bold', zorder=3)
    ax.text(lx + 30, ly + lh - 54, "ĐĂNG KÝ TÀI KHOẢN & PHÂN QUYỀN HỆ THỐNG DSS", color='#FFFFFF', fontsize=12, fontweight='bold', zorder=3)
    
    # 2 Columns Layout
    col_w = 360
    c1_x = lx + 35
    c2_x = lx + 425
    top_y = ly + lh - head_h - 35
    
    fields_col1 = [
        ("Họ và tên nhân viên (*):", "Trần Hồng Minh"),
        ("Mã nhân viên (*):", "NV005"),
        ("Tên đăng nhập (*):", "minh_th"),
        ("Email doanh nghiệp (*):", "minh.tran@sportzone.vn")
    ]
    
    f_y = top_y
    for label, placeholder in fields_col1:
        ax.text(c1_x, f_y, label, color='#1E293B', fontsize=8.2, fontweight='bold', zorder=2)
        inp = FancyBboxPatch((c1_x, f_y - 32), col_w, 28, boxstyle="round,pad=2", facecolor='#F8FAFC', edgecolor='#CBD5E1', zorder=2)
        ax.add_patch(inp)
        ax.text(c1_x + 10, f_y - 18, placeholder, color='#0F172A', fontsize=8, va='center', zorder=3)
        f_y -= 58
        
    fields_col2 = [
        ("Mật khẩu truy cập (*):", "••••••••••••"),
        ("Xác nhận lại mật khẩu (*):", "••••••••••••"),
        ("Vai trò trong hệ thống (RBAC) (*):", "DecisionMaker (Người Ra Quyết Định) ▼")
    ]
    
    f_y2 = top_y
    for label, placeholder in fields_col2:
        ax.text(c2_x, f_y2, label, color='#1E293B', fontsize=8.2, fontweight='bold', zorder=2)
        inp = FancyBboxPatch((c2_x, f_y2 - 32), col_w, 28, boxstyle="round,pad=2", facecolor='#F8FAFC', edgecolor='#CBD5E1', zorder=2)
        ax.add_patch(inp)
        ax.text(c2_x + 10, f_y2 - 18, placeholder, color='#0F172A', fontsize=8, va='center', zorder=3)
        f_y2 -= 58
        
    # Role description box
    role_box = FancyBboxPatch((c2_x, f_y2 - 40), col_w, 42, boxstyle="round,pad=3", facecolor='#EFF6FF', edgecolor='#BFDBFE', zorder=2)
    ax.add_patch(role_box)
    ax.text(c2_x + 10, f_y2 - 16, "💡 Quyền hạn DecisionMaker: Toàn quyền thiết lập ma trận Saaty,", color='#1E40AF', fontsize=7.2, zorder=3)
    ax.text(c2_x + 10, f_y2 - 30, "chạy mô hình DSS và phê duyệt kế hoạch phân bổ hàng hóa.", color='#1E40AF', fontsize=7.2, zorder=3)
    
    # Bottom policy & Buttons
    bot_y = ly + 40
    btn_reg = FancyBboxPatch((c1_x, bot_y), 240, 38, boxstyle="round,pad=3", facecolor='#10B981', edgecolor='none', zorder=2)
    ax.add_patch(btn_reg)
    ax.text(c1_x + 120, bot_y + 19, "✅ ĐĂNG KÝ TÀI KHOẢN", color='white', fontsize=9, fontweight='bold', ha='center', va='center', zorder=3)
    
    btn_back = FancyBboxPatch((c1_x + 255, bot_y), 190, 38, boxstyle="round,pad=3", facecolor='#64748B', edgecolor='none', zorder=2)
    ax.add_patch(btn_back)
    ax.text(c1_x + 350, bot_y + 19, "↩ Quay lại Đăng nhập", color='white', fontsize=8.5, fontweight='bold', ha='center', va='center', zorder=3)
    
    plt.tight_layout()
    plt.savefig('docs/wireframes/GUI_03_Register_Form.png', dpi=150, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print("Generated GUI_03_Register_Form.png")

# =========================================================================
# 4. WIREFRAME GUI-04: MAIN FORM - TAB 1 (INPUTS & SAATY MATRIX)
# =========================================================================
def generate_gui_04():
    fig, ax = setup_canvas()
    draw_window_frame(ax, "DSS Retail Decision Studio 2026 - [Tab 1: Đầu Vào Dữ Liệu & Ma Trận Saaty]")
    draw_app_header(ax)
    draw_tab_bar(ax, active_tab_idx=0)
    
    # Content Area
    content_y = 15
    content_h = 475
    
    # Split Container: Left 52% (Alternatives), Right 48% (Pairwise Matrix)
    split_x = 515
    
    # Left Panel: Alternatives DataGridView
    pnl_left = FancyBboxPatch((15, content_y), split_x - 20, content_h, boxstyle="square,pad=0", facecolor='#FFFFFF', edgecolor='#CBD5E1', zorder=1)
    ax.add_patch(pnl_left)
    
    ax.text(28, content_y + content_h - 24, "1. DANH SÁCH PHƯƠNG ÁN / SẢN PHẨM ỨNG VIÊN (ALTERNATIVES):", color='#0F172A', fontsize=8.5, fontweight='bold', zorder=2)
    
    # Table Header
    th_y = content_y + content_h - 55
    ax.add_patch(Rectangle((20, th_y), split_x - 30, 24, facecolor='#1E293B', zorder=2))
    ax.text(32, th_y + 12, "Mã PA", color='white', fontsize=7.5, fontweight='bold', va='center', zorder=3)
    ax.text(82, th_y + 12, "Tên Sản Phẩm / Phương Án", color='white', fontsize=7.5, fontweight='bold', va='center', zorder=3)
    ax.text(250, th_y + 12, "Mã SKU", color='white', fontsize=7.5, fontweight='bold', va='center', zorder=3)
    ax.text(330, th_y + 12, "Giá Niêm Yết", color='white', fontsize=7.5, fontweight='bold', va='center', zorder=3)
    ax.text(415, th_y + 12, "Tồn", color='white', fontsize=7.5, fontweight='bold', va='center', zorder=3)
    ax.text(455, th_y + 12, "Biên LN", color='white', fontsize=7.5, fontweight='bold', va='center', zorder=3)
    
    rows_alt = [
        ("ALT_01", "Laptop Asus ROG Strix G16", "PRD_ASUS_ROG16", "31,490,000 đ", "25", "30.14%"),
        ("ALT_02", "iPhone 15 Pro Max 256GB", "PRD_IPHONE15PM", "34,990,000 đ", "18", "21.43%"),
        ("ALT_03", "Samsung Galaxy S24 Ultra", "PRD_SAMSUNGS24U", "29,990,000 đ", "30", "23.34%"),
        ("ALT_04", "Tai nghe Sony WH-1000XM5", "PRD_SONYWHXM5", "7,490,000 đ", "42", "26.70%"),
        ("ALT_05", "Máy tính bảng iPad Air M2", "PRD_IPADAIRM2", "16,990,000 đ", "20", "18.83%"),
        ("ALT_06", "Đồng hồ Garmin Fenix 7 Pro", "PRD_GARMINF7P", "19,490,000 đ", "15", "25.65%"),
        ("ALT_07", "Chuột Logitech MX Master 3S", "PRD_LOGIMX3S", "2,490,000 đ", "65", "35.74%"),
        ("ALT_08", "Bàn phím cơ Keychron Q1 Pro", "PRD_KEYCHRONQ1", "4,290,000 đ", "38", "32.40%"),
        ("ALT_09", "Loa Bluetooth Marshall Acton 3", "PRD_MARSHALLA3", "6,990,000 đ", "28", "24.18%")
    ]
    
    row_y = th_y - 24
    for idx, (pa, name, sku, price, stock, margin) in enumerate(rows_alt):
        bg_r = '#F8FAFC' if idx % 2 == 0 else '#FFFFFF'
        ax.add_patch(Rectangle((20, row_y), split_x - 30, 22, facecolor=bg_r, edgecolor='#E2E8F0', linewidth=0.5, zorder=2))
        ax.text(32, row_y + 11, pa, color='#4F46E5', fontsize=7.2, fontweight='bold', va='center', zorder=3)
        ax.text(82, row_y + 11, name[:24], color='#0F172A', fontsize=7.2, va='center', zorder=3)
        ax.text(250, row_y + 11, sku[:11], color='#64748B', fontsize=7, va='center', zorder=3)
        ax.text(330, row_y + 11, price, color='#0F172A', fontsize=7.2, va='center', zorder=3)
        ax.text(415, row_y + 11, stock, color='#0F172A', fontsize=7.2, va='center', zorder=3)
        ax.text(455, row_y + 11, margin, color='#16A34A', fontsize=7.2, fontweight='bold', va='center', zorder=3)
        row_y -= 24
        
    ax.text(28, content_y + 20, "📌 Tổng số: 9 phương án ứng viên đã nạp từ bảng ALTERNATIVES & PRODUCTS.", color='#64748B', fontsize=7.5, zorder=2)
    
    # Right Panel: Saaty Pairwise Matrix DataGridView
    rx = split_x + 10
    rw = 1000 - rx - 15
    pnl_right = FancyBboxPatch((rx, content_y), rw, content_h, boxstyle="square,pad=0", facecolor='#FFFFFF', edgecolor='#CBD5E1', zorder=1)
    ax.add_patch(pnl_right)
    
    ax.text(rx + 15, content_y + content_h - 24, "2. MA TRẬN SO SÁNH CẶP GIỮA CÁC TIÊU CHÍ (SAATY 1 - 9):", color='#0F172A', fontsize=8.5, fontweight='bold', zorder=2)
    
    # 5x5 Matrix Header
    m_y = content_y + content_h - 55
    ax.add_patch(Rectangle((rx + 10, m_y), rw - 20, 24, facecolor='#1E293B', zorder=2))
    ax.text(rx + 20, m_y + 12, "Tiêu Chí Dòng", color='white', fontsize=7.5, fontweight='bold', va='center', zorder=3)
    ax.text(rx + 140, m_y + 12, "C1: Biên LN", color='white', fontsize=7.5, fontweight='bold', va='center', zorder=3)
    ax.text(rx + 205, m_y + 12, "C2: Doanh Thu", color='white', fontsize=7.5, fontweight='bold', va='center', zorder=3)
    ax.text(rx + 275, m_y + 12, "C3: Nhu Cầu", color='white', fontsize=7.5, fontweight='bold', va='center', zorder=3)
    ax.text(rx + 340, m_y + 12, "C4: Rủi Ro", color='white', fontsize=7.5, fontweight='bold', va='center', zorder=3)
    ax.text(rx + 400, m_y + 12, "C5: Chi Phí", color='white', fontsize=7.5, fontweight='bold', va='center', zorder=3)
    
    matrix_rows = [
        ("C1: Biên Lợi Nhuận", "1.000", "3.000", "2.000", "4.000", "5.000"),
        ("C2: Doanh Thu Kỳ Vọng", "0.333", "1.000", "0.500", "2.000", "3.000"),
        ("C3: Nhu Cầu Thị Trường", "0.500", "2.000", "1.000", "3.000", "4.000"),
        ("C4: Rủi Ro Tồn Kho", "0.250", "0.500", "0.333", "1.000", "2.000"),
        ("C5: Chi Phí Marketing", "0.200", "0.333", "0.250", "0.500", "1.000")
    ]
    
    m_row_y = m_y - 28
    for idx, (crit, c1, c2, c3, c4, c5) in enumerate(matrix_rows):
        bg_m = '#EFF6FF' if idx == 0 else ('#F8FAFC' if idx % 2 == 1 else '#FFFFFF')
        ax.add_patch(Rectangle((rx + 10, m_row_y), rw - 20, 26, facecolor=bg_m, edgecolor='#E2E8F0', linewidth=0.5, zorder=2))
        ax.text(rx + 20, m_row_y + 13, crit, color='#0F172A', fontsize=7.5, fontweight='bold', va='center', zorder=3)
        ax.text(rx + 155, m_row_y + 13, c1, color='#1E40AF' if c1=="1.000" else '#0F172A', fontsize=7.5, va='center', zorder=3)
        ax.text(rx + 225, m_row_y + 13, c2, color='#1E40AF' if c2=="1.000" else '#0F172A', fontsize=7.5, va='center', zorder=3)
        ax.text(rx + 295, m_row_y + 13, c3, color='#1E40AF' if c3=="1.000" else '#0F172A', fontsize=7.5, va='center', zorder=3)
        ax.text(rx + 355, m_row_y + 13, c4, color='#1E40AF' if c4=="1.000" else '#0F172A', fontsize=7.5, va='center', zorder=3)
        ax.text(rx + 415, m_row_y + 13, c5, color='#1E40AF' if c5=="1.000" else '#0F172A', fontsize=7.5, va='center', zorder=3)
        m_row_y -= 28
        
    # Consistency status box
    stat_box = FancyBboxPatch((rx + 15, content_y + 80), rw - 30, 48, boxstyle="round,pad=3", facecolor='#ECFDF5', edgecolor='#10B981', linewidth=1.2, zorder=2)
    ax.add_patch(stat_box)
    ax.text(rx + 28, content_y + 110, "🟢 TỶ SỐ NHẤT QUÁN SAATY: CR = 0.0152 (1.52% ≤ 10%) - ĐẠT CHUẨN NHẤT QUÁN", color='#065F46', fontsize=8, fontweight='bold', zorder=3)
    ax.text(rx + 28, content_y + 92, "λmax = 5.068 • CI = 0.017 • RI = 1.120 • Trọng số w = [39.1%, 15.3%, 25.8%, 11.7%, 8.1%]", color='#047857', fontsize=7.2, zorder=3)
    
    # Big Action Button
    btn_run = FancyBboxPatch((rx + 15, content_y + 20), rw - 30, 46, boxstyle="round,pad=3", facecolor='#4F46E5', edgecolor='none', zorder=2)
    ax.add_patch(btn_run)
    ax.text(rx + rw/2, content_y + 43, "🚀 CHẠY MÔ HÌNH DSS NGAY (TÍNH ĐIỂM & XẾP HẠNG)", color='white', fontsize=10, fontweight='bold', ha='center', va='center', zorder=3)
    
    plt.tight_layout()
    plt.savefig('docs/wireframes/GUI_04_Main_Tab1_Inputs.png', dpi=150, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print("Generated GUI_04_Main_Tab1_Inputs.png")

# =========================================================================
# 5. WIREFRAME GUI-05: MAIN FORM - TAB 2 (OUTPUTS & RANKING)
# =========================================================================
def generate_gui_05():
    fig, ax = setup_canvas()
    draw_window_frame(ax, "DSS Retail Decision Studio 2026 - [Tab 2: Kết Quả Ra Quyết Định & Xếp Hạng]")
    draw_app_header(ax)
    draw_tab_bar(ax, active_tab_idx=1)
    
    # 4 KPI Cards Row
    card_w = 230
    card_h = 75
    cx = 15
    cy = 415
    
    kpis = [
        ("LỰA CHỌN TỐI ƯU (RANK 1)", "Asus ROG Strix G16", "Điểm: 0.2458 • Phê Duyệt", '#ECFDF5', '#10B981', '#065F46'),
        ("TỶ SỐ NHẤT QUÁN (CR)", "0.0152 (1.52%)", "Đạt chuẩn Saaty (≤ 10%)", '#EFF6FF', '#3B82F6', '#1E40AF'),
        ("DOANH THU KỲ VỌNG", "850 Triệu VNĐ", "Biên LN gộp: 30.14%", '#FAF5FF', '#8B5CF6', '#6B21A8'),
        ("TỔNG PHƯƠNG ÁN", "9 Phương Án", "1 Chọn • 3 Xem Xét • 5 Loại", '#FFFBEB', '#F59E0B', '#92400E')
    ]
    
    for title, val, sub, bg, border, txt_c in kpis:
        ax.add_patch(FancyBboxPatch((cx, cy), card_w, card_h, boxstyle="round,pad=3", facecolor=bg, edgecolor=border, linewidth=1.2, zorder=2))
        ax.text(cx + 12, cy + card_h - 18, title, color='#64748B', fontsize=7, fontweight='bold', zorder=3)
        ax.text(cx + 12, cy + card_h - 42, val, color=txt_c, fontsize=10.5, fontweight='bold', zorder=3)
        ax.text(cx + 12, cy + 12, sub, color='#475569', fontsize=7, zorder=3)
        cx += card_w + 18
        
    # Split Row: Left 60% (Ranking DataGridView), Right 40% (AHP Chart + Advisory)
    split_x = 590
    main_y = 15
    main_h = 390
    
    # Left: Ranking Grid
    ax.add_patch(FancyBboxPatch((15, main_y), split_x - 20, main_h, boxstyle="square,pad=0", facecolor='#FFFFFF', edgecolor='#CBD5E1', zorder=1))
    ax.text(28, main_y + main_h - 22, "BẢNG XẾP HẠNG PHƯƠNG ÁN & PHÊ DUYỆT (DSS OUTPUT):", color='#0F172A', fontsize=8.2, fontweight='bold', zorder=2)
    
    # Action Buttons on Table Header
    btn_rerun = FancyBboxPatch((split_x - 250, main_y + main_h - 32), 110, 26, boxstyle="round,pad=2", facecolor='#4F46E5', edgecolor='none', zorder=2)
    ax.add_patch(btn_rerun)
    ax.text(split_x - 195, main_y + main_h - 19, "🔄 Chạy Lại", color='white', fontsize=7.5, fontweight='bold', ha='center', va='center', zorder=3)
    
    btn_exp = FancyBboxPatch((split_x - 130, main_y + main_h - 32), 115, 26, boxstyle="round,pad=2", facecolor='#10B981', edgecolor='none', zorder=2)
    ax.add_patch(btn_exp)
    ax.text(split_x - 72, main_y + main_h - 19, "📄 Xuất Báo Cáo", color='white', fontsize=7.5, fontweight='bold', ha='center', va='center', zorder=3)
    
    # Table Header
    th_y = main_y + main_h - 58
    ax.add_patch(Rectangle((22, th_y), split_x - 34, 22, facecolor='#1E293B', zorder=2))
    ax.text(32, th_y + 11, "Hạng", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    ax.text(70, th_y + 11, "Mã SKU", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    ax.text(175, th_y + 11, "Sản Phẩm / Phương Án", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    ax.text(355, th_y + 11, "Điểm AHP", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    ax.text(425, th_y + 11, "Quyết Định", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    ax.text(505, th_y + 11, "Đề Xuất Nhập", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    
    ranking_data = [
        ("1", "#F59E0B", "PRD_ASUS_ROG16", "Laptop Asus ROG Strix G16", "0.2458", "✓ Phê Duyệt", "#DCFCE7", "#166534", "85 chiếc"),
        ("2", "#94A3B8", "PRD_IPHONE15PM", "iPhone 15 Pro Max 256GB", "0.1984", "⏳ Xem Xét", "#FEF3C7", "#92400E", "40 chiếc"),
        ("3", "#D97706", "PRD_SAMSUNGS24U", "Samsung Galaxy S24 Ultra", "0.1562", "⏳ Xem Xét", "#FEF3C7", "#92400E", "35 chiếc"),
        ("4", "#CBD5E1", "PRD_SONYWHXM5", "Tai nghe Sony WH-1000XM5", "0.1145", "⏳ Xem Xét", "#FEF3C7", "#92400E", "20 chiếc"),
        ("5", "#CBD5E1", "PRD_LOGIMX3S", "Chuột Logitech MX Master 3S", "0.0862", "✕ Loại Bỏ", "#FEE2E2", "#991B1B", "0 chiếc"),
        ("6", "#CBD5E1", "PRD_IPADAIRM2", "Máy tính bảng iPad Air M2", "0.0715", "✕ Loại Bỏ", "#FEE2E2", "#991B1B", "0 chiếc"),
        ("7", "#CBD5E1", "PRD_GARMINF7P", "Đồng hồ Garmin Fenix 7 Pro", "0.0520", "✕ Loại Bỏ", "#FEE2E2", "#991B1B", "0 chiếc"),
        ("8", "#CBD5E1", "PRD_KEYCHRONQ1", "Bàn phím cơ Keychron Q1", "0.0418", "✕ Loại Bỏ", "#FEE2E2", "#991B1B", "0 chiếc"),
        ("9", "#CBD5E1", "PRD_MARSHALLA3", "Loa Bluetooth Marshall 3", "0.0336", "✕ Loại Bỏ", "#FEE2E2", "#991B1B", "0 chiếc")
    ]
    
    r_y = th_y - 25
    for rank, r_col, sku, name, score, outcome, o_bg, o_tx, qty in ranking_data:
        ax.add_patch(Rectangle((22, r_y), split_x - 34, 23, facecolor='#FFFFFF', edgecolor='#E2E8F0', linewidth=0.5, zorder=2))
        # Rank badge circle
        ax.add_patch(Circle((42, r_y + 11.5), 8, facecolor=r_col, zorder=3))
        ax.text(42, r_y + 11.5, rank, color='white' if rank!="4" else '#1E293B', fontsize=7, fontweight='bold', ha='center', va='center', zorder=4)
        ax.text(70, r_y + 11.5, sku[:12], color='#64748B', fontsize=7, va='center', zorder=3)
        ax.text(175, r_y + 11.5, name[:22], color='#0F172A', fontsize=7.2, fontweight='bold' if rank=="1" else 'normal', va='center', zorder=3)
        ax.text(355, r_y + 11.5, score, color='#4F46E5' if rank=="1" else '#0F172A', fontsize=7.2, fontweight='bold', va='center', zorder=3)
        
        # Pill status
        ax.add_patch(FancyBboxPatch((420, r_y + 3), 75, 17, boxstyle="round,pad=2", facecolor=o_bg, edgecolor='none', zorder=3))
        ax.text(457, r_y + 11.5, outcome, color=o_tx, fontsize=6.8, fontweight='bold', ha='center', va='center', zorder=4)
        ax.text(525, r_y + 11.5, qty, color='#0F172A', fontsize=7.2, va='center', zorder=3)
        r_y -= 25
        
    # Right: Upper AHP Bar Chart & Lower Advisory
    rx = split_x + 10
    rw = 1000 - rx - 15
    
    # AHP Chart Panel
    chart_h = 220
    ax.add_patch(FancyBboxPatch((rx, main_y + main_h - chart_h), rw, chart_h, boxstyle="square,pad=0", facecolor='#FFFFFF', edgecolor='#CBD5E1', zorder=1))
    ax.text(rx + 15, main_y + main_h - 20, "TỶ TRỌNG ĐIỂM SỐ AHP CÁC PHƯƠNG ÁN (AHP BAR CHART):", color='#0F172A', fontsize=7.8, fontweight='bold', zorder=2)
    
    # Horizontal bars
    bar_y = main_y + main_h - 45
    bars = [
        ("Asus ROG Strix G16", 0.2458, '#10B981'),
        ("iPhone 15 Pro Max", 0.1984, '#3B82F6'),
        ("Samsung Galaxy S24", 0.1562, '#6366F1'),
        ("Sony WH-1000XM5", 0.1145, '#8B5CF6'),
        ("Logitech MX Master 3S", 0.0862, '#94A3B8'),
        ("iPad Air M2", 0.0715, '#CBD5E1'),
        ("Garmin Fenix 7 Pro", 0.0520, '#E2E8F0')
    ]
    for b_name, b_val, b_col in bars:
        ax.text(rx + 15, bar_y + 5, b_name[:18], color='#334155', fontsize=6.8, va='center', zorder=2)
        ax.add_patch(Rectangle((rx + 130, bar_y - 2), rw - 190, 12, facecolor='#F1F5F9', zorder=2))
        ax.add_patch(Rectangle((rx + 130, bar_y - 2), (rw - 190) * (b_val / 0.26), 12, facecolor=b_col, zorder=3))
        ax.text(rx + 135 + (rw - 190) * (b_val / 0.26), bar_y + 4, f"{b_val:.4f}", color='#0F172A', fontsize=6.8, fontweight='bold', va='center', zorder=4)
        bar_y -= 24
        
    # Lower Executive Advisory Box
    adv_h = main_h - chart_h - 15
    ax.add_patch(FancyBboxPatch((rx, main_y), rw, adv_h, boxstyle="round,pad=3", facecolor='#F0FDF4', edgecolor='#86EFAC', linewidth=1.2, zorder=1))
    ax.text(rx + 15, main_y + adv_h - 20, "💡 KHUYẾN NGHỊ ĐIỀU HÀNH (EXECUTIVE COMMENTS):", color='#166534', fontsize=8, fontweight='bold', zorder=2)
    ax.text(rx + 15, main_y + adv_h - 40, "• Hạng 1 (Asus ROG Strix G16): Ưu tiên giải ngân 45% ngân sách marketing.", color='#15803D', fontsize=7.2, zorder=2)
    ax.text(rx + 15, main_y + adv_h - 60, "• Đề xuất đặt hàng 85 chiếc dựa trên chu kỳ 30 ngày và lead time 7 ngày.", color='#15803D', fontsize=7.2, zorder=2)
    ax.text(rx + 15, main_y + adv_h - 80, "• Nhóm Rejected: Dừng nhập bổ sung, tiến hành xả tồn thu hồi dòng tiền.", color='#15803D', fontsize=7.2, zorder=2)
    
    plt.tight_layout()
    plt.savefig('docs/wireframes/GUI_05_Main_Tab2_Outputs.png', dpi=150, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print("Generated GUI_05_Main_Tab2_Outputs.png")

# =========================================================================
# 6. WIREFRAME GUI-06: MAIN FORM - TAB 3 (WHAT-IF SENSITIVITY)
# =========================================================================
def generate_gui_06():
    fig, ax = setup_canvas()
    draw_window_frame(ax, "DSS Retail Decision Studio 2026 - [Tab 3: Mô Phỏng Biến Động Thị Trường What-If]")
    draw_app_header(ax)
    draw_tab_bar(ax, active_tab_idx=2)
    
    top_y = 485
    ax.text(20, top_y, "BIẾN ĐỘNG THỊ TRƯỜNG & PHÂN TÍCH ĐỘ NHẠY (WHAT-IF SENSITIVITY ANALYSIS):", color='#0F172A', fontsize=9, fontweight='bold', zorder=2)
    
    # 4 Scenario Buttons
    sc_y = top_y - 40
    scenarios = [
        ("🔥 Kịch Bản 1: Bùng Nổ Gaming (+40% Nhu Cầu)", 310, '#D97706'),
        ("⚡ Kịch Bản 2: Đứt Gãy Cung Ứng (Rủi Ro Tăng)", 290, '#E11D48'),
        ("📉 Kịch Bản 3: Lạm Phát - Giá Rẻ", 210, '#0EA5E9'),
        ("🔄 Baseline Gốc", 130, '#475569')
    ]
    sx = 20
    for title, w, bg in scenarios:
        ax.add_patch(FancyBboxPatch((sx, sc_y), w, 32, boxstyle="round,pad=2", facecolor=bg, edgecolor='none', zorder=2))
        ax.text(sx + w/2, sc_y + 16, title, color='white', fontsize=7.8, fontweight='bold', ha='center', va='center', zorder=3)
        sx += w + 12
        
    # Fine-Tuning Bar
    tune_y = sc_y - 45
    ax.add_patch(FancyBboxPatch((20, tune_y), 960, 36, boxstyle="round,pad=2", facecolor='#F1F5F9', edgecolor='#CBD5E1', zorder=1))
    ax.text(32, tune_y + 18, "🎯 Tùy biến SP: [Laptop Asus ROG Strix G16 ▼]", color='#0F172A', fontsize=7.8, fontweight='bold', va='center', zorder=2)
    ax.text(330, tune_y + 18, "Nhu cầu (C3): [0.95 ▲▼]", color='#334155', fontsize=7.8, va='center', zorder=2)
    ax.text(490, tune_y + 18, "Biên LN (C1): [0.90 ▲▼]", color='#334155', fontsize=7.8, va='center', zorder=2)
    ax.text(650, tune_y + 18, "An toàn vốn (C4): [0.75 ▲▼]", color='#334155', fontsize=7.8, va='center', zorder=2)
    
    btn_apply = FancyBboxPatch((835, tune_y + 4), 135, 28, boxstyle="round,pad=2", facecolor='#4F46E5', edgecolor='none', zorder=2)
    ax.add_patch(btn_apply)
    ax.text(902, tune_y + 18, "⚡ Áp Dụng", color='white', fontsize=8, fontweight='bold', ha='center', va='center', zorder=3)
    
    # View Switcher Tabs: Delta Chart vs Hierarchy Tree
    view_y = tune_y - 35
    ax.add_patch(FancyBboxPatch((20, view_y), 190, 26, boxstyle="round,pad=2", facecolor='#4F46E5', edgecolor='none', zorder=2))
    ax.text(115, view_y + 13, "📈 Biểu Đồ Đường Độ Nhạy", color='white', fontsize=7.8, fontweight='bold', ha='center', va='center', zorder=3)
    
    ax.add_patch(FancyBboxPatch((220, view_y), 210, 26, boxstyle="round,pad=2", facecolor='#E2E8F0', edgecolor='none', zorder=2))
    ax.text(325, view_y + 13, "🌳 Sơ Đồ Cây Phân Cấp AHP", color='#475569', fontsize=7.8, fontweight='bold', ha='center', va='center', zorder=3)
    
    # Central Canvas: Delta Comparison Chart
    diag_y = 80
    diag_h = 240
    ax.add_patch(FancyBboxPatch((20, diag_y), 960, diag_h, boxstyle="square,pad=0", facecolor='#FFFFFF', edgecolor='#CBD5E1', zorder=1))
    
    # Grid lines & Delta Chart Mockup
    ax.text(35, diag_y + diag_h - 22, "SO SÁNH BIẾN ĐỘNG ĐIỂM SỐ AHP: KỊCH BẢN GỐC (BASELINE) vs BIẾN ĐỘNG (WHAT-IF):", color='#0F172A', fontsize=8.2, fontweight='bold', zorder=2)
    
    # Chart Legend
    ax.add_patch(Rectangle((680, diag_y + diag_h - 25), 20, 4, facecolor='#2563EB', zorder=2))
    ax.text(710, diag_y + diag_h - 23, "Baseline Điểm Gốc", color='#2563EB', fontsize=7.5, va='center', zorder=2)
    ax.add_patch(Rectangle((830, diag_y + diag_h - 25), 20, 4, facecolor='#DC2626', linestyle='--', zorder=2))
    ax.text(860, diag_y + diag_h - 23, "Điểm What-If Mới", color='#DC2626', fontsize=7.5, va='center', zorder=2)
    
    # Points
    prods = ["Asus ROG", "iPhone 15", "Galaxy S24", "Sony XM5", "iPad Air", "Garmin F7", "Logi MX3", "Keychron", "Marshall"]
    base_pts = [0.2458, 0.1984, 0.1562, 0.1145, 0.0862, 0.0715, 0.0520, 0.0418, 0.0336]
    whatif_pts = [0.2910, 0.1750, 0.1420, 0.1280, 0.0790, 0.0650, 0.0510, 0.0390, 0.0300]
    
    px = np.linspace(80, 920, 9)
    # Scale Y to canvas
    def scale_y(v): return diag_y + 40 + (v / 0.32) * 150
    
    for i in range(len(px) - 1):
        ax.plot([px[i], px[i+1]], [scale_y(base_pts[i]), scale_y(base_pts[i+1])], color='#2563EB', linewidth=2, zorder=3)
        ax.plot([px[i], px[i+1]], [scale_y(whatif_pts[i]), scale_y(whatif_pts[i+1])], color='#DC2626', linewidth=2, linestyle='--', zorder=3)
        
    for i in range(len(px)):
        ax.plot(px[i], scale_y(base_pts[i]), marker='o', markersize=6, color='#2563EB', zorder=4)
        ax.plot(px[i], scale_y(whatif_pts[i]), marker='s', markersize=6, color='#DC2626', zorder=4)
        ax.text(px[i], diag_y + 18, prods[i], color='#334155', fontsize=7, ha='center', zorder=2)
        diff = whatif_pts[i] - base_pts[i]
        diff_txt = f"+{diff:.3f}" if diff > 0 else f"{diff:.3f}"
        diff_col = '#16A34A' if diff > 0 else '#DC2626'
        ax.text(px[i], scale_y(whatif_pts[i]) + 10, diff_txt, color=diff_col, fontsize=6.8, fontweight='bold', ha='center', zorder=4)
        
    # Bottom Advisory
    ax.add_patch(FancyBboxPatch((20, 15), 960, 52, boxstyle="round,pad=3", facecolor='#ECFDF5', edgecolor='#10B981', linewidth=1.2, zorder=1))
    ax.text(35, 52, "📊 ĐÁNH GIÁ ĐỘ NHẠY & TÁC ĐỘNG CHIẾN LƯỢC KINH DOANH (DSS SENSITIVITY IMPACT):", color='#065F46', fontsize=8, fontweight='bold', zorder=2)
    ax.text(35, 32, "• Kịch Bản 1 (Gaming): Nhu cầu tăng mạnh đưa Asus ROG Strix bứt phá (+18.4% điểm số), dẫn đầu tuyệt đối. Khuyến nghị tập trung nhập hàng.", color='#047857', fontsize=7.2, zorder=2)
    
    plt.tight_layout()
    plt.savefig('docs/wireframes/GUI_06_Main_Tab3_WhatIf.png', dpi=150, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print("Generated GUI_06_Main_Tab3_WhatIf.png")

# =========================================================================
# 7. WIREFRAME GUI-07: MAIN FORM - TAB 4 (ADD PRODUCT & DYNAMIC EVALUATION)
# =========================================================================
def generate_gui_07():
    fig, ax = setup_canvas()
    draw_window_frame(ax, "DSS Retail Decision Studio 2026 - [Tab 4: Thêm Sản Phẩm Mới & Thẩm Định Động]")
    draw_app_header(ax)
    draw_tab_bar(ax, active_tab_idx=3)
    
    content_y = 20
    content_h = 470
    card_w = 465
    
    # Card 1: Product Information (OLTP)
    c1_x = 20
    ax.add_patch(FancyBboxPatch((c1_x, content_y), card_w, content_h, boxstyle="square,pad=0", facecolor='#FFFFFF', edgecolor='#CBD5E1', zorder=1))
    ax.text(c1_x + 20, content_y + content_h - 26, "1. THÔNG TIN SẢN PHẨM MỚI (OLTP DATA):", color='#0F172A', fontsize=9, fontweight='bold', zorder=2)
    ax.add_patch(Rectangle((c1_x + 20, content_y + content_h - 38), card_w - 40, 1, facecolor='#E2E8F0', zorder=2))
    
    fields_p = [
        ("Tên Sản Phẩm (*):", "Laptop Asus ROG Strix G16"),
        ("Mã SKU Sản Phẩm (*):", "PRD_ASUS_ROG16"),
        ("Ngành Hàng (*):", "Máy tính xách tay (Laptops) ▼"),
        ("Giá Vốn Nhập Hàng (VNĐ) (*):", "22,000,000 đ ▲▼"),
        ("Giá Bán Niêm Yết (VNĐ) (*):", "31,490,000 đ ▲▼"),
        ("Số Lượng Tồn Kho Ban Đầu (*):", "25 chiếc ▲▼")
    ]
    
    f_y = content_y + content_h - 75
    for label, val in fields_p:
        ax.text(c1_x + 24, f_y, label, color='#334155', fontsize=8, fontweight='bold', zorder=2)
        ax.add_patch(FancyBboxPatch((c1_x + 210, f_y - 20), 230, 28, boxstyle="round,pad=2", facecolor='#F8FAFC', edgecolor='#CBD5E1', zorder=2))
        ax.text(c1_x + 220, f_y - 6, val, color='#0F172A', fontsize=7.8, va='center', zorder=3)
        f_y -= 52
        
    # Auto Calculated Margin Pill
    ax.add_patch(FancyBboxPatch((c1_x + 24, content_y + 40), card_w - 48, 38, boxstyle="round,pad=3", facecolor='#ECFDF5', edgecolor='#10B981', zorder=2))
    ax.text(c1_x + card_w/2, content_y + 59, "⚡ Tỷ Suất Biên LN Gộp Tự Động: 30.14% (ĐẠT CHUẨN TỐI ƯU ≥ 20%)", color='#065F46', fontsize=8, fontweight='bold', ha='center', va='center', zorder=3)
    
    # Card 2: 5 Criteria DSS Evaluation
    c2_x = 515
    ax.add_patch(FancyBboxPatch((c2_x, content_y), card_w, content_h, boxstyle="square,pad=0", facecolor='#FFFFFF', edgecolor='#CBD5E1', zorder=1))
    ax.text(c2_x + 20, content_y + content_h - 26, "2. ĐÁNH GIÁ 5 TIÊU CHÍ DSS (AHP EVALUATION):", color='#0F172A', fontsize=9, fontweight='bold', zorder=2)
    ax.add_patch(Rectangle((c2_x + 20, content_y + content_h - 38), card_w - 40, 1, facecolor='#E2E8F0', zorder=2))
    
    fields_c = [
        ("C1: Biên Lợi Nhuận (%):", "30.14% (Khóa - Tự động tính) 🔒"),
        ("C2: Doanh Thu Kỳ Vọng (Triệu đ):", "850 Triệu VNĐ ▲▼"),
        ("C3: Nhu Cầu Thị Trường (1 - 100):", "87 điểm ▲▼"),
        ("C4: Tồn Kho / Quay Vòng (Ngày):", "19 Ngày ▲▼"),
        ("C5: Chi Phí Marketing (Triệu đ):", "35 Triệu VNĐ ▲▼")
    ]
    
    f_y = content_y + content_h - 75
    for label, val in fields_c:
        ax.text(c2_x + 24, f_y, label, color='#334155', fontsize=8, fontweight='bold', zorder=2)
        ax.add_patch(FancyBboxPatch((c2_x + 230, f_y - 20), 210, 28, boxstyle="round,pad=2", facecolor='#F1F5F9' if "Khóa" in val else '#F8FAFC', edgecolor='#CBD5E1', zorder=2))
        ax.text(c2_x + 240, f_y - 6, val, color='#16A34A' if "Khóa" in val else '#0F172A', fontsize=7.8, fontweight='bold' if "Khóa" in val else 'normal', va='center', zorder=3)
        f_y -= 52
        
    # Big Green Save Button
    btn_save = FancyBboxPatch((c2_x + 24, content_y + 35), card_w - 48, 48, boxstyle="round,pad=3", facecolor='#10B981', edgecolor='none', zorder=2)
    ax.add_patch(btn_save)
    ax.text(c2_x + card_w/2, content_y + 59, "💾 LƯU SẢN PHẨM & CẬP NHẬT VÀO HỆ THỐNG DSS", color='white', fontsize=9.5, fontweight='bold', ha='center', va='center', zorder=3)
    
    plt.tight_layout()
    plt.savefig('docs/wireframes/GUI_07_Main_Tab4_AddProduct.png', dpi=150, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print("Generated GUI_07_Main_Tab4_AddProduct.png")

# =========================================================================
# 8. WIREFRAME GUI-08: MAIN FORM - TAB 5 (SALES & OLAP)
# =========================================================================
def generate_gui_08():
    fig, ax = setup_canvas()
    draw_window_frame(ax, "DSS Retail Decision Studio 2026 - [Tab 5: Dữ Liệu Bán Hàng Lịch Sử OLAP]")
    draw_app_header(ax)
    draw_tab_bar(ax, active_tab_idx=4)
    
    content_y = 15
    content_h = 475
    ax.add_patch(FancyBboxPatch((15, content_y), 970, content_h, boxstyle="square,pad=0", facecolor='#FFFFFF', edgecolor='#CBD5E1', zorder=1))
    
    ax.text(28, content_y + content_h - 24, "DỮ LIỆU BÁN HÀNG THỰC TẾ & PHÂN TÍCH ĐA CHIỀU OLAP (ROLLUP & CUBE):", color='#0F172A', fontsize=9, fontweight='bold', zorder=2)
    
    # 3 Mode Buttons
    btn_y = content_y + content_h - 65
    ax.add_patch(FancyBboxPatch((25, btn_y), 290, 32, boxstyle="round,pad=2", facecolor='#4F46E5', edgecolor='none', zorder=2))
    ax.text(170, btn_y + 16, "📊 1. Hiệu Suất Sản Phẩm (Fact/View)", color='white', fontsize=7.8, fontweight='bold', ha='center', va='center', zorder=3)
    
    ax.add_patch(FancyBboxPatch((325, btn_y), 310, 32, boxstyle="round,pad=2", facecolor='#F1F5F9', edgecolor='#CBD5E1', zorder=2))
    ax.text(480, btn_y + 16, "📦 2. OLAP ROLLUP: Phân Bổ Hàng Tồn Kho", color='#334155', fontsize=7.8, fontweight='bold', ha='center', va='center', zorder=3)
    
    ax.add_patch(FancyBboxPatch((645, btn_y), 325, 32, boxstyle="round,pad=2", facecolor='#F1F5F9', edgecolor='#CBD5E1', zorder=2))
    ax.text(807, btn_y + 16, "🎯 3. OLAP CUBE: Đa Chiều Khuyến Mãi", color='#334155', fontsize=7.8, fontweight='bold', ha='center', va='center', zorder=3)
    
    # Description note
    ax.text(28, btn_y - 18, "💡 View Hiệu Suất Sản Phẩm: Thống kê định lượng doanh thu, lợi nhuận gộp, biên lợi nhuận % và tồn kho thực tế của các SKU làm căn cứ cho tiêu chí DSS.", color='#64748B', fontsize=7.2, fontstyle='italic', zorder=2)
    
    # DataGridView Table Header
    th_y = btn_y - 45
    ax.add_patch(Rectangle((25, th_y), 950, 24, facecolor='#1E293B', zorder=2))
    ax.text(35, th_y + 12, "Mã SKU", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    ax.text(125, th_y + 12, "Tên Sản Phẩm", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    ax.text(285, th_y + 12, "Ngành Hàng", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    ax.text(410, th_y + 12, "Giá Niêm Yết", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    ax.text(505, th_y + 12, "Đã Bán", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    ax.text(575, th_y + 12, "Tổng Doanh Thu", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    ax.text(690, th_y + 12, "Lợi Nhuận Gộp", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    ax.text(800, th_y + 12, "Biên LN %", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    ax.text(875, th_y + 12, "Trạng Thái Tồn", color='white', fontsize=7.2, fontweight='bold', va='center', zorder=3)
    
    olap_rows = [
        ("PRD_ASUS_ROG16", "Laptop Asus ROG Strix G16", "Máy tính xách tay", "31,490,000 đ", "142", "4,471,580,000 đ", "1,348,180,000 đ", "30.14%", "Tối Ưu"),
        ("PRD_IPHONE15PM", "iPhone 15 Pro Max 256GB", "Điện thoại thông minh", "34,990,000 đ", "195", "6,823,050,000 đ", "1,462,050,000 đ", "21.43%", "An Toàn"),
        ("PRD_SAMSUNGS24U", "Samsung Galaxy S24 Ultra", "Điện thoại thông minh", "29,990,000 đ", "120", "3,598,800,000 đ", "840,000,000 đ", "23.34%", "An Toàn"),
        ("PRD_SONYWHXM5", "Tai nghe Sony WH-1000XM5", "Phụ kiện kỹ thuật số", "7,490,000 đ", "310", "2,321,900,000 đ", "620,000,000 đ", "26.70%", "Tối Ưu"),
        ("PRD_IPADAIRM2", "iPad Air M2 11 inch", "Máy tính bảng", "16,990,000 đ", "88", "1,495,120,000 đ", "281,600,000 đ", "18.83%", "Cần Đẩy Hàng"),
        ("PRD_GARMINF7P", "Đồng hồ Garmin Fenix 7 Pro", "Thiết bị đeo", "19,490,000 đ", "45", "877,050,000 đ", "225,000,000 đ", "25.65%", "Chậm Luân Chuyển"),
        ("PRD_LOGIMX3S", "Chuột Logitech MX Master 3S", "Phụ kiện kỹ thuật số", "2,490,000 đ", "450", "1,120,500,000 đ", "400,500,000 đ", "35.74%", "Tối Ưu")
    ]
    
    r_y = th_y - 25
    for idx, (sku, name, cat, price, sold, rev, profit, margin, status) in enumerate(olap_rows):
        bg_c = '#F8FAFC' if idx % 2 == 0 else '#FFFFFF'
        ax.add_patch(Rectangle((25, r_y), 950, 23, facecolor=bg_c, edgecolor='#E2E8F0', linewidth=0.5, zorder=2))
        ax.text(35, r_y + 11.5, sku[:12], color='#4F46E5', fontsize=7, fontweight='bold', va='center', zorder=3)
        ax.text(125, r_y + 11.5, name[:22], color='#0F172A', fontsize=7, va='center', zorder=3)
        ax.text(285, r_y + 11.5, cat, color='#64748B', fontsize=7, va='center', zorder=3)
        ax.text(410, r_y + 11.5, price, color='#0F172A', fontsize=7, va='center', zorder=3)
        ax.text(505, r_y + 11.5, sold, color='#0F172A', fontsize=7, va='center', zorder=3)
        ax.text(575, r_y + 11.5, rev, color='#0F172A', fontsize=7, va='center', zorder=3)
        ax.text(690, r_y + 11.5, profit, color='#0F172A', fontsize=7, va='center', zorder=3)
        ax.text(800, r_y + 11.5, margin, color='#16A34A', fontsize=7, fontweight='bold', va='center', zorder=3)
        ax.text(875, r_y + 11.5, status, color='#2563EB' if status=="Tối Ưu" else '#D97706', fontsize=7, va='center', zorder=3)
        r_y -= 25
        
    plt.tight_layout()
    plt.savefig('docs/wireframes/GUI_08_Main_Tab5_OLAP.png', dpi=150, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print("Generated GUI_08_Main_Tab5_OLAP.png")

# =========================================================================
# 9. WIREFRAME GUI-09: MAIN FORM - TAB 6 (REPORT & AUDIT TRAIL)
# =========================================================================
def generate_gui_09():
    fig, ax = setup_canvas()
    draw_window_frame(ax, "DSS Retail Decision Studio 2026 - [Tab 6: Biểu Mẫu Báo Cáo Thẩm Định & Phê Duyệt]")
    draw_app_header(ax)
    draw_tab_bar(ax, active_tab_idx=5)
    
    content_y = 15
    content_h = 475
    ax.add_patch(FancyBboxPatch((15, content_y), 970, content_h, boxstyle="square,pad=0", facecolor='#FFFFFF', edgecolor='#CBD5E1', zorder=1))
    
    # Top Action Toolbar
    tb_y = content_y + content_h - 55
    ax.text(28, tb_y + 30, "📋 BIỂU MẪU BÁO CÁO THẨM ĐỊNH & PHÊ DUYỆT CHIẾN LƯỢC KINH DOANH", color='#0F172A', fontsize=9.5, fontweight='bold', zorder=2)
    ax.text(28, tb_y + 12, "Mã biểu mẫu: BM-06/DSS-2026/Q4 • Trạng thái: 🟢 ĐÃ THẨM ĐỊNH & PHÊ DUYỆT • Mã phiên: DSS-SESSION-001", color='#64748B', fontsize=7.2, zorder=2)
    
    # 3 Buttons Right
    btn_w = 145
    ax.add_patch(FancyBboxPatch((520, tb_y + 8), btn_w, 32, boxstyle="round,pad=2", facecolor='#185ABD', edgecolor='none', zorder=2))
    ax.text(592, tb_y + 24, "📝 Xuất File Word", color='white', fontsize=7.5, fontweight='bold', ha='center', va='center', zorder=3)
    
    ax.add_patch(FancyBboxPatch((675, tb_y + 8), btn_w, 32, boxstyle="round,pad=2", facecolor='#10B981', edgecolor='none', zorder=2))
    ax.text(747, tb_y + 24, "🖨️ In Báo Cáo (PDF)", color='white', fontsize=7.5, fontweight='bold', ha='center', va='center', zorder=3)
    
    ax.add_patch(FancyBboxPatch((830, tb_y + 8), btn_w, 32, boxstyle="round,pad=2", facecolor='#F1F5F9', edgecolor='#CBD5E1', zorder=2))
    ax.text(902, tb_y + 24, "🔄 Cập Nhật Dữ Liệu", color='#334155', fontsize=7.5, fontweight='bold', ha='center', va='center', zorder=3)
    
    ax.add_patch(Rectangle((20, tb_y - 5), 960, 1, facecolor='#E2E8F0', zorder=2))
    
    # 4 Executive Cards
    cy = tb_y - 70
    cw = 225
    c_x = 25
    cards = [
        ("SẢN PHẨM HÀNG ĐẦU", "Asus ROG Strix G16", "Điểm AHP: 0.2458 • Phê duyệt", '#ECFDF5', '#10B981'),
        ("TỶ SỐ NHẤT QUÁN CR", "0.0152 (1.52%)", "Đạt chuẩn Saaty (≤ 10%)", '#EFF6FF', '#3B82F6'),
        ("TỔNG SL NHẬP ĐỀ XUẤT", "160 Chiếc", "Chu kỳ 30 ngày + Lead time", '#FAF5FF', '#8B5CF6'),
        ("TỔNG NGÂN SÁCH MKT", "450 Triệu VNĐ", "Phân bổ 45% cho Hạng 1", '#FFFBEB', '#F59E0B')
    ]
    for title, val, sub, bg, border in cards:
        ax.add_patch(FancyBboxPatch((c_x, cy), cw, 55, boxstyle="round,pad=2", facecolor=bg, edgecolor=border, linewidth=1, zorder=2))
        ax.text(c_x + 10, cy + 42, title, color='#64748B', fontsize=6.8, fontweight='bold', zorder=3)
        ax.text(c_x + 10, cy + 24, val, color='#0F172A', fontsize=9, fontweight='bold', zorder=3)
        ax.text(c_x + 10, cy + 8, sub, color='#475569', fontsize=6.5, zorder=3)
        c_x += cw + 18
        
    # Allocation Table
    table_y = cy - 145
    ax.text(28, cy - 15, "BẢNG KẾ HOẠCH PHÂN BỔ HÀNG HÓA & NGÂN SÁCH MARKETING QUÝ 4/2026:", color='#0F172A', fontsize=8, fontweight='bold', zorder=2)
    
    ax.add_patch(Rectangle((25, table_y + 95), 950, 22, facecolor='#1E293B', zorder=2))
    ax.text(35, table_y + 106, "Hạng", color='white', fontsize=7, fontweight='bold', va='center', zorder=3)
    ax.text(80, table_y + 106, "Mã SKU", color='white', fontsize=7, fontweight='bold', va='center', zorder=3)
    ax.text(190, table_y + 106, "Tên Sản Phẩm", color='white', fontsize=7, fontweight='bold', va='center', zorder=3)
    ax.text(380, table_y + 106, "Điểm AHP", color='white', fontsize=7, fontweight='bold', va='center', zorder=3)
    ax.text(460, table_y + 106, "SL Nhập Đề Xuất", color='white', fontsize=7, fontweight='bold', va='center', zorder=3)
    ax.text(580, table_y + 106, "Vốn Nhập Dự Kiến", color='white', fontsize=7, fontweight='bold', va='center', zorder=3)
    ax.text(710, table_y + 106, "Tỷ Lệ MKT %", color='white', fontsize=7, fontweight='bold', va='center', zorder=3)
    ax.text(820, table_y + 106, "Trạng Thái Phê Duyệt", color='white', fontsize=7, fontweight='bold', va='center', zorder=3)
    
    plan_rows = [
        ("1", "PRD_ASUS_ROG16", "Laptop Asus ROG Strix G16", "0.2458", "85 chiếc", "1,870,000,000 đ", "45.0%", "✓ PHÊ DUYỆT RÓT VỐN"),
        ("2", "PRD_IPHONE15PM", "iPhone 15 Pro Max 256GB", "0.1984", "40 chiếc", "1,100,000,000 đ", "25.0%", "⏳ NHẬP THĂM DÒ"),
        ("3", "PRD_SAMSUNGS24U", "Samsung Galaxy S24 Ultra", "0.1562", "35 chiếc", "805,000,000 đ", "20.0%", "⏳ NHẬP THĂM DÒ"),
        ("4..9", "--- (6 SKU Còn lại)", "Logitech, iPad, Sony, Marshall...", "< 0.12", "0 chiếc", "0 đ", "10.0%", "✕ TẠM DỪNG / XẢ TỒN")
    ]
    p_y = table_y + 72
    for r, sku, name, score, qty, cap, mkt, status in plan_rows:
        ax.add_patch(Rectangle((25, p_y), 950, 22, facecolor='#F8FAFC' if r=="1" else '#FFFFFF', edgecolor='#E2E8F0', linewidth=0.5, zorder=2))
        ax.text(35, p_y + 11, r, color='#4F46E5', fontsize=7, fontweight='bold', va='center', zorder=3)
        ax.text(80, p_y + 11, sku, color='#64748B', fontsize=7, va='center', zorder=3)
        ax.text(190, p_y + 11, name, color='#0F172A', fontsize=7, fontweight='bold' if r=="1" else 'normal', va='center', zorder=3)
        ax.text(380, p_y + 11, score, color='#0F172A', fontsize=7, va='center', zorder=3)
        ax.text(460, p_y + 11, qty, color='#0F172A', fontsize=7, fontweight='bold', va='center', zorder=3)
        ax.text(580, p_y + 11, cap, color='#0F172A', fontsize=7, va='center', zorder=3)
        ax.text(710, p_y + 11, mkt, color='#16A34A', fontsize=7, fontweight='bold', va='center', zorder=3)
        ax.text(820, p_y + 11, status, color='#15803D' if "✓" in status else ('#B45309' if "⏳" in status else '#B91C1C'), fontsize=7, fontweight='bold', va='center', zorder=3)
        p_y -= 23
        
    # 3 Strategic Directives Box
    ax.add_patch(FancyBboxPatch((25, 20), 950, 65, boxstyle="round,pad=3", facecolor='#F8FAFC', edgecolor='#CBD5E1', zorder=2))
    ax.text(38, 70, "🎯 CHỈ THỊ ĐIỀU HÀNH CHO BAN GIÁM ĐỐC (MANAGEMENT DIRECTIVES):", color='#0F172A', fontsize=7.5, fontweight='bold', zorder=3)
    ax.text(38, 52, "1. Phê duyệt giải ngân 1.87 tỷ VNĐ đặt hàng 85 chiếc Asus ROG Strix G16 trước ngày 05/11/2026 đảm bảo tồn kho mùa cao điểm.", color='#334155', fontsize=6.8, zorder=3)
    ax.text(38, 38, "2. Nhóm iPhone 15 Pro Max và Galaxy S24 Ultra tiếp tục theo dõi biến động giá thị trường, duy trì tồn kho an toàn 30 ngày.", color='#334155', fontsize=6.8, zorder=3)
    ax.text(38, 24, "3. Triển khai chương trình khuyến mãi xả tồn cho 6 SKU bán chậm để thu hồi vốn lưu động.", color='#334155', fontsize=6.8, zorder=3)
    
    plt.tight_layout()
    plt.savefig('docs/wireframes/GUI_09_Main_Tab6_Report.png', dpi=150, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print("Generated GUI_09_Main_Tab6_Report.png")

# =========================================================================
# 10. WIREFRAME GUI-10: DATABASEGUARD SECURITY OVERLAY
# =========================================================================
def generate_gui_10():
    fig, ax = setup_canvas()
    
    # Dark Overlay background simulating full screen block
    ax.add_patch(Rectangle((0, 0), 1000, 650, facecolor='#0B1120', zorder=0))
    ax.text(500, 620, "WIREFRAME GUI-10: MÀN HÌNH KHÓA AN TOÀN KHI MẤT KẾT NỐI CSDL (DATABASEGUARD OVERLAY)", color='#F87171', fontsize=10.5, fontweight='bold', ha='center', zorder=2)
    
    # Centered Alert Box
    bx, by, bw, bh = 150, 100, 700, 480
    box = FancyBboxPatch((bx, by), bw, bh, boxstyle="round,pad=8", facecolor='#0F172A', edgecolor='#EF4444', linewidth=2, zorder=1)
    ax.add_patch(box)
    
    # Large Warning Icon
    ax.text(bx + bw/2, by + bh - 70, "⛔", fontsize=48, ha='center', va='center', zorder=3)
    
    # Title
    ax.text(bx + bw/2, by + bh - 140, "MẤT KẾT NỐI SQL SERVER – HỆ THỐNG TẠM DỪNG", color='#F87171', fontsize=14, fontweight='bold', ha='center', zorder=3)
    ax.text(bx + bw/2, by + bh - 170, "DATABASEGUARD ACTIVE SHIELD • TOÀN BỘ GIAO DIỆN ĐÃ ĐƯỢC KHÓA AN TOÀN", color='#E2E8F0', fontsize=8.5, ha='center', zorder=3)
    
    # Error Message Terminal Card
    t_w = bw - 80
    t_h = 135
    t_x = bx + 40
    t_y = by + bh - 325
    ax.add_patch(FancyBboxPatch((t_x, t_y), t_w, t_h, boxstyle="round,pad=3", facecolor='#020617', edgecolor='#334155', zorder=2))
    
    err_lines = [
        "🔴 [System.Data.SqlClient.SqlException]: A network-related or instance-specific error occurred.",
        "• Server: .\\SQLEXPRESS25 | Database: DSS_SalesDB | State: OFFLINE",
        "• Lý do: Dịch vụ SQL Server bị dừng hoặc đứt cáp mạng LAN nội bộ.",
        "• Cơ chế bảo vệ: Đóng băng toàn bộ thao tác DSS để chống ra quyết định trên dữ liệu sai.",
        "• Tự động quét nhịp tim (Heartbeat Timer): Quét lại sau mỗi 4000ms..."
    ]
    line_y = t_y + t_h - 22
    for el in err_lines:
        ax.text(t_x + 15, line_y, el, color='#FCA5A5' if "🔴" in el else '#94A3B8', fontsize=7.2, fontfamily='monospace', zorder=3)
        line_y -= 22
        
    # Action Buttons
    btn_y = by + 55
    btn_retry = FancyBboxPatch((bx + bw/2 - 200, btn_y), 190, 42, boxstyle="round,pad=3", facecolor='#4F46E5', edgecolor='none', zorder=2)
    ax.add_patch(btn_retry)
    ax.text(bx + bw/2 - 105, btn_y + 21, "↻ Thử Kết Nối Lại", color='white', fontsize=9.5, fontweight='bold', ha='center', va='center', zorder=3)
    
    btn_exit = FancyBboxPatch((bx + bw/2 + 10, btn_y), 180, 42, boxstyle="round,pad=3", facecolor='#334155', edgecolor='none', zorder=2)
    ax.add_patch(btn_exit)
    ax.text(bx + bw/2 + 100, btn_y + 21, "Thoát Ứng Dụng", color='white', fontsize=9.5, fontweight='bold', ha='center', va='center', zorder=3)
    
    ax.text(bx + bw/2, by + 25, "🛡️ Khi SQL Server hoạt động trở lại, màn hình này sẽ tự động biến mất và nạp lại dữ liệu.", color='#64748B', fontsize=7.5, ha='center', zorder=3)
    
    plt.tight_layout()
    plt.savefig('docs/wireframes/GUI_10_DatabaseGuard_Overlay.png', dpi=150, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print("Generated GUI_10_DatabaseGuard_Overlay.png")

if __name__ == '__main__':
    print("Starting Wireframes Generation...")
    generate_gui_01()
    generate_gui_02()
    generate_gui_03()
    generate_gui_04()
    generate_gui_05()
    generate_gui_06()
    generate_gui_07()
    generate_gui_08()
    generate_gui_09()
    generate_gui_10()
    print("ALL 10 WIREFRAME IMAGES SUCCESSFULLY GENERATED IN docs/wireframes/")
