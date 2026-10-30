# -*- coding: utf-8 -*-
"""
Script tao so do truc quan In-Scope vs Out-of-Scope
He thong Ho tro Ra quyet dinh DSS Retail Decision Studio 2026
IS 402 A - Nhom 05 - Duy Tan University
"""

import matplotlib.pyplot as plt
import matplotlib.patches as patches
from matplotlib.lines import Line2D
import os

def create_scope_diagram(output_path="docs/images/sodo_pham_vi_inscope_outscope.png"):
    # Typography
    plt.rcParams['font.family'] = 'sans-serif'
    plt.rcParams['font.sans-serif'] = ['Segoe UI', 'Arial', 'Tahoma', 'DejaVu Sans']
    
    fig, ax = plt.subplots(figsize=(15.5, 9.6), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    # Background
    fig.patch.set_facecolor('#F8FAFC')
    ax.set_facecolor('#F8FAFC')
    
    # Main Title & Header Banner
    banner = patches.FancyBboxPatch((3, 91), 94, 7.5, boxstyle="round,pad=0.3,rounding_size=0.8",
                                    facecolor="#0F172A", edgecolor="#1E293B", linewidth=1.5)
    ax.add_patch(banner)
    
    ax.text(50, 95.8, "SƠ ĐỒ RANH GIỚI PHẠM VI HỆ THỐNG (SYSTEM SCOPE BOUNDARY DIAGRAM)", 
            color="#FFFFFF", fontsize=15, weight='bold', ha='center', va='center')
    ax.text(50, 92.6, "HỆ THỐNG DSS RETAIL DECISION STUDIO 2026 • HỌC PHẦN IS 402 A • ĐẠI HỌC DUY TÂN", 
            color="#94A3B8", fontsize=10, weight='medium', ha='center', va='center')
    
    # =========================================================================
    # 1. IN-SCOPE CONTAINER (Core DSS System - Blue)
    # =========================================================================
    in_box = patches.FancyBboxPatch((24, 8), 52, 79, boxstyle="round,pad=0.5,rounding_size=1.2",
                                    facecolor="#F0F7FF", edgecolor="#2563EB", linewidth=2.8, linestyle='-')
    ax.add_patch(in_box)
    
    # In-Scope Header Badge
    in_badge = patches.FancyBboxPatch((27, 82.5), 46, 4.2, boxstyle="round,pad=0.2,rounding_size=0.5",
                                      facecolor="#2563EB", edgecolor="#1D4ED8", linewidth=1)
    ax.add_patch(in_badge)
    ax.text(50, 84.6, "PHẠM VI TRONG HỆ THỐNG (IN-SCOPE) — NỘI HÀM PHẦN MỀM DSS", 
            color="#FFFFFF", fontsize=11.5, weight='bold', ha='center', va='center')
    
    # Sub-card 1: Giao diện & Điều hướng
    c1 = patches.FancyBboxPatch((26, 65), 48, 15.5, boxstyle="round,pad=0.2,rounding_size=0.5",
                                facecolor="#FFFFFF", edgecolor="#93C5FD", linewidth=1.2)
    ax.add_patch(c1)
    ax.text(28, 78.5, "1. Giao Diện Người Dùng & Quản Trị Bảo Mật (UI / Security)", color="#1E40AF", fontsize=9.5, weight='bold')
    ax.text(28, 75.5, "• SplashForm khởi động kết nối CSDL; LoginForm xác thực băm SHA-256 bảng USERS.", color="#334155", fontsize=8.5)
    ax.text(28, 72.5, "• Quản lý phiên UserSession theo 4 vai trò (Admin, DecisionMaker, SalesManager, Staff).", color="#334155", fontsize=8.5)
    ax.text(28, 69.5, "• Cơ chế Fail-Safe: Panel pnlDbOverlay khóa cứng 6 Tab bảo vệ khi mạng/SQL Server ngắt.", color="#334155", fontsize=8.5)
    ax.text(28, 66.5, "• Hỗ trợ đăng ký tài khoản mới RegisterForm và kiểm soát phân quyền chặt chẽ.", color="#334155", fontsize=8.5)

    # Sub-card 2: Bộ não Toán học AHP
    c2 = patches.FancyBboxPatch((26, 47), 48, 16.5, boxstyle="round,pad=0.2,rounding_size=0.5",
                                facecolor="#FFFFFF", edgecolor="#93C5FD", linewidth=1.2)
    ax.add_patch(c2)
    ax.text(28, 61.5, "2. Bộ Não Toán Học AHP Saaty & Xếp Hạng Phương Án (AHP Engine)", color="#1E40AF", fontsize=9.5, weight='bold')
    ax.text(28, 58.5, "• Ma trận so sánh cặp Saaty 1-9 cho 5 tiêu chí: Lợi nhuận (C1), Doanh thu (C2), Nhu cầu (C3),", color="#334155", fontsize=8.5)
    ax.text(28, 55.8, "  Rủi ro tồn kho (C4), Chi phí tiếp thị (C5). Tự động kiểm định độ nhất quán CR <= 0.10.", color="#334155", fontsize=8.5)
    ax.text(28, 53.0, "• Chuẩn hóa dữ liệu thô theo hàm Benefit (v/vmax) và Cost (vmin/v); tính tổng hợp trọng số.", color="#334155", fontsize=8.5)
    ax.text(28, 50.2, "• Tự động xếp hạng và gắn nhãn phân loại: Selected (Phê duyệt), UnderReview, Rejected.", color="#334155", fontsize=8.5)
    ax.text(28, 47.7, "• Tự động sinh khuyến nghị số lượng đặt hàng tối ưu (Lead Time, chu kỳ 30 ngày).", color="#334155", fontsize=8.5)

    # Sub-card 3: Phân tích Đa chiều OLAP & Mô phỏng What-If
    c3 = patches.FancyBboxPatch((26, 29), 48, 16.5, boxstyle="round,pad=0.2,rounding_size=0.5",
                                facecolor="#FFFFFF", edgecolor="#93C5FD", linewidth=1.2)
    ax.add_patch(c3)
    ax.text(28, 43.5, "3. Mô Phỏng What-If, Mở Rộng SKU & Phân Tích OLAP", color="#1E40AF", fontsize=9.5, weight='bold')
    ax.text(28, 40.5, "• Mô phỏng độ nhạy What-If với 3 kịch bản thị trường (Bùng nổ Gaming, Đứt gãy, Lạm phát).", color="#334155", fontsize=8.5)
    ax.text(28, 37.8, "• Tùy biến trực tiếp tham số Nhu cầu, Biên lợi nhuận, Rủi ro vốn; tái tính toán tức thời.", color="#334155", fontsize=8.5)
    ax.text(28, 35.0, "• Thêm SKU mới động (Tab 4): Cập nhật phương án và tự động tái tính toán AHP toàn bộ.", color="#334155", fontsize=8.5)
    ax.text(28, 32.2, "• OLAP bán lẻ (Tab 5): Cắt lát (Slice), Thái hạt lựu (Dice), Cuộn lên/Khoan sâu (Roll/Drill).", color="#334155", fontsize=8.5)
    ax.text(28, 29.7, "• Trực quan hóa Sơ đồ cây phân cấp và Biểu đồ Delta độ nhạy.", color="#334155", fontsize=8.5)

    # Sub-card 4: CSDL SQL Server 3NF & Báo cáo Xuất bản
    c4 = patches.FancyBboxPatch((26, 10.5), 48, 17, boxstyle="round,pad=0.2,rounding_size=0.5",
                                facecolor="#FFFFFF", edgecolor="#93C5FD", linewidth=1.2)
    ax.add_patch(c4)
    ax.text(28, 25.5, "4. CSDL Chuẩn Hóa 3NF & Xuất Báo Cáo Thẩm Định (Tab 6)", color="#1E40AF", fontsize=9.5, weight='bold')
    ax.text(28, 22.5, "• Cơ sở dữ liệu DSS_SalesDB trên SQL Server với 11 bảng thực thể đạt chuẩn 3NF.", color="#334155", fontsize=8.5)
    ax.text(28, 19.8, "• Lưu vết toàn bộ phiên quyết định DECISION_SESSIONS và kết quả DECISION_RESULTS.", color="#334155", fontsize=8.5)
    ax.text(28, 17.0, "• Dashboard quản trị KPI, bảng xếp hạng chi tiết, đồ thị cột trực quan.", color="#334155", fontsize=8.5)
    ax.text(28, 14.2, "• Xuất báo cáo thẩm định chuyên nghiệp ra định dạng Microsoft Word (.docx) và HTML.", color="#334155", fontsize=8.5)
    ax.text(28, 11.5, "• Tự động sinh biên bản tóm tắt điều hành (Executive Summary) minh bạch 100%.", color="#334155", fontsize=8.5)

    # =========================================================================
    # 2. OUT-OF-SCOPE CONTAINERS (External Systems - Red / Grey Dashed)
    # =========================================================================
    
    # Left 1: POS Điểm bán lẻ
    pos_box = patches.FancyBboxPatch((2, 60), 19.5, 23, boxstyle="round,pad=0.3,rounding_size=0.8",
                                     facecolor="#FFF5F5", edgecolor="#E11D48", linewidth=2, linestyle='--')
    ax.add_patch(pos_box)
    ax.text(11.75, 80.5, "HỆ THỐNG POS BÁN LẺ\n(POINT OF SALE)", color="#9F1239", fontsize=9.5, weight='bold', ha='center')
    ax.text(3.5, 75.5, "[X] KHÔNG in hóa đơn tại quầy", color="#475569", fontsize=8)
    ax.text(3.5, 72.8, "[X] KHÔNG quẹt thẻ / thu tiền mặt", color="#475569", fontsize=8)
    ax.text(3.5, 70.1, "[X] KHÔNG quét barcode bán lẻ", color="#475569", fontsize=8)
    ax.text(3.5, 67.4, "[X] KHÔNG đóng vai trò máy POS", color="#475569", fontsize=8)
    ax.text(3.5, 63.5, "[i] Chỉ nhận dữ liệu đơn hàng\n    lịch sử từ CSDL ORDERS.", color="#1E40AF", fontsize=8, weight='bold')

    # Left 2: WMS Kho vật lý
    wms_box = patches.FancyBboxPatch((2, 28), 19.5, 27, boxstyle="round,pad=0.3,rounding_size=0.8",
                                     facecolor="#FFF5F5", edgecolor="#E11D48", linewidth=2, linestyle='--')
    ax.add_patch(wms_box)
    ax.text(11.75, 52.5, "QUẢN LÝ KHO VẬT LÝ\n(WMS / WAREHOUSE)", color="#9F1239", fontsize=9.5, weight='bold', ha='center')
    ax.text(3.5, 47.5, "[X] KHÔNG tự động trừ hàng kho", color="#475569", fontsize=8)
    ax.text(3.5, 44.8, "[X] KHÔNG in tem nhãn RFID/kệ", color="#475569", fontsize=8)
    ax.text(3.5, 42.1, "[X] KHÔNG điều phối xe nâng pallet", color="#475569", fontsize=8)
    ax.text(3.5, 39.4, "[X] KHÔNG kiểm kê kho tự động", color="#475569", fontsize=8)
    ax.text(3.5, 34.5, "[i] Khuyến nghị số lượng đặt\n    hàng cần thủ kho/quản lý duyệt\n    trên hệ thống ERP/WMS.", color="#1E40AF", fontsize=8, weight='bold')

    # Left 3: Logistics / Vận tải
    log_box = patches.FancyBboxPatch((2, 4), 19.5, 20, boxstyle="round,pad=0.3,rounding_size=0.8",
                                     facecolor="#FFF5F5", edgecolor="#E11D48", linewidth=2, linestyle='--')
    ax.add_patch(log_box)
    ax.text(11.75, 21.5, "LOGISTICS & GIAO VẬN\n(FLEET MANAGEMENT)", color="#9F1239", fontsize=9.5, weight='bold', ha='center')
    ax.text(3.5, 16.5, "[X] KHÔNG điều phối xe tải giao hàng", color="#475569", fontsize=8)
    ax.text(3.5, 13.8, "[X] KHÔNG định vị GPS hành trình", color="#475569", fontsize=8)
    ax.text(3.5, 11.1, "[X] KHÔNG ký nhận giao hàng POD", color="#475569", fontsize=8)
    ax.text(3.5, 7.5, "[i] DSS chỉ dùng Lead Time làm\n    tham số tính số lượng đặt.", color="#1E40AF", fontsize=8, weight='bold')

    # Right 1: E-Commerce & Website
    web_box = patches.FancyBboxPatch((78.5, 60), 19.5, 23, boxstyle="round,pad=0.3,rounding_size=0.8",
                                     facecolor="#FFF5F5", edgecolor="#E11D48", linewidth=2, linestyle='--')
    ax.add_patch(web_box)
    ax.text(88.25, 80.5, "SÀN TMĐT & WEBSITE\n(E-COMMERCE SYSTEM)", color="#9F1239", fontsize=9.5, weight='bold', ha='center')
    ax.text(80, 75.5, "[X] KHÔNG giỏ hàng trực tuyến", color="#475569", fontsize=8)
    ax.text(80, 72.8, "[X] KHÔNG cổng thanh toán online", color="#475569", fontsize=8)
    ax.text(80, 70.1, "[X] KHÔNG tự động đổi giá web", color="#475569", fontsize=8)
    ax.text(80, 67.4, "[X] KHÔNG tiếp thị quảng cáo trực tiếp", color="#475569", fontsize=8)
    ax.text(80, 63.5, "[i] DSS chỉ đề xuất phân bổ\n    ngân sách tiếp thị tối ưu.", color="#1E40AF", fontsize=8, weight='bold')

    # Right 2: Kế toán Tài chính & ERP
    erp_box = patches.FancyBboxPatch((78.5, 28), 19.5, 27, boxstyle="round,pad=0.3,rounding_size=0.8",
                                     facecolor="#FFF5F5", edgecolor="#E11D48", linewidth=2, linestyle='--')
    ax.add_patch(erp_box)
    ax.text(88.25, 52.5, "KẾ TOÁN & TÀI CHÍNH\n(ENTERPRISE ERP)", color="#9F1239", fontsize=9.5, weight='bold', ha='center')
    ax.text(80, 47.5, "[X] KHÔNG hạch toán sổ cái Nợ/Có", color="#475569", fontsize=8)
    ax.text(80, 44.8, "[X] KHÔNG xuất hóa đơn điện tử VAT", color="#475569", fontsize=8)
    ax.text(80, 42.1, "[X] KHÔNG tính lương nhân viên", color="#475569", fontsize=8)
    ax.text(80, 39.4, "[X] KHÔNG quản lý công nợ khách", color="#475569", fontsize=8)
    ax.text(80, 34.5, "[i] DSS chỉ tham mưu chiến lược,\n    không ghi nhận tài chính pháp lý.", color="#1E40AF", fontsize=8, weight='bold')

    # Right 3: Human Decision Maker (Cấp Quản Trị)
    mgr_box = patches.FancyBboxPatch((78.5, 4), 19.5, 20, boxstyle="round,pad=0.3,rounding_size=0.8",
                                     facecolor="#FEFCE8", edgecolor="#CA8A04", linewidth=2, linestyle='-')
    ax.add_patch(mgr_box)
    ax.text(88.25, 21.5, "NHÀ QUẢN LÝ / GIÁM ĐỐC\n(DECISION MAKER)", color="#854D0E", fontsize=9.5, weight='bold', ha='center')
    ax.text(80, 16.5, "[+] Phê duyệt kế hoạch cuối cùng", color="#475569", fontsize=8)
    ax.text(80, 13.8, "[+] Quyết định chi ngân sách thực", color="#475569", fontsize=8)
    ax.text(80, 11.1, "[+] Chịu trách nhiệm kinh doanh", color="#475569", fontsize=8)
    ax.text(80, 7.5, "[*] Human-in-the-Loop: DSS chỉ\n    tham mưu, con người quyết định.", color="#854D0E", fontsize=8, weight='bold')

    # =========================================================================
    # 3. INTERFACE FLOWS / ARROWS
    # =========================================================================
    # Arrow 1: POS / WMS Data In -> In-Scope (Green arrow)
    ax.annotate("", xy=(24, 71.5), xytext=(21.5, 71.5),
                arrowprops=dict(arrowstyle="->", color="#059669", lw=2.2))
    ax.text(22.75, 73.5, "Data In", color="#059669", fontsize=7.5, weight='bold', ha='center')

    ax.annotate("", xy=(24, 41.5), xytext=(21.5, 41.5),
                arrowprops=dict(arrowstyle="->", color="#059669", lw=2.2))
    ax.text(22.75, 43.5, "Tồn kho", color="#059669", fontsize=7.5, weight='bold', ha='center')

    # Arrow 2: In-Scope -> Decision Maker (Yellow / Indigo arrow)
    ax.annotate("", xy=(78.5, 14), xytext=(76, 14),
                arrowprops=dict(arrowstyle="->", color="#4F46E5", lw=2.2))
    ax.text(77.25, 16, "Báo cáo DSS", color="#4F46E5", fontsize=7.5, weight='bold', ha='center')

    # Arrow 3: In-Scope -> ERP / E-Commerce (Advisory output)
    ax.annotate("", xy=(78.5, 41.5), xytext=(76, 41.5),
                arrowprops=dict(arrowstyle="->", color="#94A3B8", lw=1.5, linestyle=':'))
    ax.text(77.25, 43.5, "Tham mưu", color="#64748B", fontsize=7, weight='bold', ha='center')

    ax.annotate("", xy=(78.5, 71.5), xytext=(76, 71.5),
                arrowprops=dict(arrowstyle="->", color="#94A3B8", lw=1.5, linestyle=':'))
    ax.text(77.25, 73.5, "Đề xuất KM", color="#64748B", fontsize=7, weight='bold', ha='center')

    # Legend at bottom
    legend_elements = [
        Line2D([0], [0], color='#2563EB', lw=3, label='Trong phạm vi hệ thống (In-Scope: 100% phần mềm thực thi)'),
        Line2D([0], [0], color='#E11D48', lw=2, linestyle='--', label='Ngoài phạm vi hệ thống (Out-of-Scope: Hệ thống tác nghiệp/vật lý)'),
        Line2D([0], [0], color='#CA8A04', lw=2, label='Tác nhân con người phê duyệt (Human-in-the-Loop)'),
        Line2D([0], [0], color='#059669', lw=2, marker='>', markersize=6, label='Luồng nạp dữ liệu vào DSS (Data In)'),
        Line2D([0], [0], color='#4F46E5', lw=2, marker='>', markersize=6, label='Luồng khuyến nghị tham mưu ra quyết định (Decisions Out)')
    ]
    ax.legend(handles=legend_elements, loc='lower center', bbox_to_anchor=(0.5, -0.01),
              ncol=3, fontsize=8, frameon=True, facecolor='#FFFFFF', edgecolor='#CBD5E1')

    plt.tight_layout()
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    plt.savefig(output_path, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print(f"Diagram successfully created at: {output_path}")

if __name__ == "__main__":
    create_scope_diagram()
