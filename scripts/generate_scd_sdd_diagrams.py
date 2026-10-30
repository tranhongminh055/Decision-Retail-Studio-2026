# -*- coding: utf-8 -*-
"""
Script tao cac so do SCD (System Context Diagram) va SDD (System Dataflow Diagram)
cho Tai lieu SRS Dac ta Yeu cau Phan mem DSS Retail Decision Studio 2026
IS 402 A - Nhom 05 - Duy Tan University
"""

import matplotlib.pyplot as plt
import matplotlib.patches as patches
from matplotlib.lines import Line2D
import os

def generate_scd_diagram(output_path="docs/images/sodo_ngu_canh_scd.png"):
    """Vẽ Sơ đồ Ngữ cảnh Hệ thống SCD (System Context Diagram - DFD Level 0)"""
    plt.rcParams['font.family'] = 'sans-serif'
    plt.rcParams['font.sans-serif'] = ['Segoe UI', 'Arial', 'Tahoma', 'DejaVu Sans']
    
    fig, ax = plt.subplots(figsize=(14.5, 9.5), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    fig.patch.set_facecolor('#FFFFFF')
    ax.set_facecolor('#FFFFFF')
    
    # Title Banner
    banner = patches.FancyBboxPatch((2, 91.5), 96, 7.2, boxstyle="round,pad=0.2,rounding_size=0.5",
                                    facecolor="#0F172A", edgecolor="#1E293B", linewidth=1.5)
    ax.add_patch(banner)
    ax.text(50, 95.8, "SƠ ĐỒ NGỮ CẢNH HỆ THỐNG — SCD (SYSTEM CONTEXT DIAGRAM / DFD MỨC 0)", 
            color="#FFFFFF", fontsize=13.5, weight='bold', ha='center', va='center')
    ax.text(50, 93.0, "DỰ ÁN: HỆ THỐNG DSS RETAIL DECISION STUDIO 2026 • HỌC PHẦN IS 402 A • ĐẠI HỌC DUY TÂN", 
            color="#94A3B8", fontsize=9, weight='medium', ha='center', va='center')

    # Central System Bubble (Process 0)
    center_circle = patches.Circle((50, 50), 16, facecolor="#EFF6FF", edgecolor="#2563EB", linewidth=2.8)
    ax.add_patch(center_circle)
    ax.text(50, 53.5, "0.0\nHỆ THỐNG DSS\nRETAIL DECISION\nSTUDIO 2026", 
            color="#1E40AF", fontsize=11, weight='bold', ha='center', va='center')
    ax.text(50, 42.5, "(Phần mềm C# .NET 10 WinForms,\nDssEngine AHP & SQL Server)", 
            color="#475569", fontsize=7.8, style='italic', ha='center', va='center')

    # External Entity 1: POS Bán Lẻ (Top Left)
    pos_box = patches.FancyBboxPatch((4, 70), 24, 15, boxstyle="round,pad=0.2,rounding_size=0.4",
                                     facecolor="#F8FAFC", edgecolor="#64748B", linewidth=1.8)
    ax.add_patch(pos_box)
    ax.text(16, 81.5, "HỆ THỐNG POS BÁN LẺ\n(Point of Sale System)", color="#0F172A", fontsize=9, weight='bold', ha='center')
    ax.text(16, 74.5, "• Quét barcode tính tiền tại quầy\n• In hóa đơn & thu ngân", color="#475569", fontsize=7.5, ha='center')

    # External Entity 2: Quản Lý Kho WMS (Bottom Left)
    wms_box = patches.FancyBboxPatch((4, 15), 24, 15, boxstyle="round,pad=0.2,rounding_size=0.4",
                                     facecolor="#F8FAFC", edgecolor="#64748B", linewidth=1.8)
    ax.add_patch(wms_box)
    ax.text(16, 26.5, "QUẢN LÝ KHO VẬT LÝ (WMS)\n(Warehouse Management)", color="#0F172A", fontsize=9, weight='bold', ha='center')
    ax.text(16, 19.5, "• Tồn kho thực tế (stock_quantity)\n• Thời gian giao hàng (Lead Time)", color="#475569", fontsize=7.5, ha='center')

    # External Entity 3: Chuyên Viên Phân Tích (Top Right)
    analyst_box = patches.FancyBboxPatch((72, 70), 24, 15, boxstyle="round,pad=0.2,rounding_size=0.4",
                                         facecolor="#FEFCE8", edgecolor="#CA8A04", linewidth=1.8)
    ax.add_patch(analyst_box)
    ax.text(84, 81.5, "CHUYÊN VIÊN PHÂN TÍCH\n(Data Analyst / Sales Mgr)", color="#854D0E", fontsize=9, weight='bold', ha='center')
    ax.text(84, 74.5, "• Thiết lập ma trận Saaty 1-9\n• Mô phỏng What-If & OLAP", color="#713F12", fontsize=7.5, ha='center')

    # External Entity 4: Nhà Quản Trị / Giám Đốc (Bottom Right)
    manager_box = patches.FancyBboxPatch((72, 15), 24, 15, boxstyle="round,pad=0.2,rounding_size=0.4",
                                         facecolor="#FEFCE8", edgecolor="#CA8A04", linewidth=1.8)
    ax.add_patch(manager_box)
    ax.text(84, 26.5, "NHÀ QUẢN TRỊ / GIÁM ĐỐC\n(Decision Maker)", color="#854D0E", fontsize=9, weight='bold', ha='center')
    ax.text(84, 19.5, "• Xem xét Dashboard KPI\n• Phê duyệt kế hoạch nhập hàng Q4", color="#713F12", fontsize=7.5, ha='center')

    # External Entity 5: Kế Toán ERP (Top Center)
    erp_box = patches.FancyBboxPatch((38, 77), 24, 11, boxstyle="round,pad=0.2,rounding_size=0.4",
                                     facecolor="#F8FAFC", edgecolor="#64748B", linewidth=1.8)
    ax.add_patch(erp_box)
    ax.text(50, 84.5, "KẾ TOÁN & ERP DOANH NGHIỆP", color="#0F172A", fontsize=8.5, weight='bold', ha='center')
    ax.text(50, 80.0, "• Hạch toán chi ngân sách nhập hàng", color="#475569", fontsize=7.5, ha='center')

    # DATA FLOW ARROWS & LABELS
    # 1. POS -> DSS
    ax.annotate("", xy=(37, 58), xytext=(28, 73),
                arrowprops=dict(arrowstyle="->", color="#059669", lw=2.0))
    ax.text(26, 64, "Lịch sử hóa đơn\n(ORDERS, ORDER_DETAILS)", color="#059669", fontsize=7.5, weight='bold', ha='right')

    # 2. WMS -> DSS
    ax.annotate("", xy=(37, 43), xytext=(28, 27),
                arrowprops=dict(arrowstyle="->", color="#059669", lw=2.0))
    ax.text(26, 36, "Số lượng tồn kho thô &\nLead Time nhà cung cấp", color="#059669", fontsize=7.5, weight='bold', ha='right')

    # 3. Analyst <-> DSS
    ax.annotate("", xy=(62, 59), xytext=(72, 73),
                arrowprops=dict(arrowstyle="<-", color="#2563EB", lw=2.0))
    ax.text(74, 64, "Ma trận Saaty, kịch bản What-If\n& thông số SKU mới (Tab 4)", color="#2563EB", fontsize=7.5, weight='bold', ha='left')

    ax.annotate("", xy=(64, 57), xytext=(73, 70),
                arrowprops=dict(arrowstyle="->", color="#0284C7", lw=1.5, linestyle=':'))
    ax.text(74, 58, "Điểm AHP, Biểu đồ Delta & OLAP", color="#0284C7", fontsize=7, weight='medium', ha='left')

    # 4. DSS -> Manager
    ax.annotate("", xy=(72, 27), xytext=(63, 43),
                arrowprops=dict(arrowstyle="->", color="#4F46E5", lw=2.2))
    ax.text(74, 38, "Báo cáo thẩm định Word (.docx),\nDashboard KPI & Khuyến nghị số lượng", color="#4F46E5", fontsize=7.5, weight='bold', ha='left')

    # 5. Manager -> DSS
    ax.annotate("", xy=(61, 41), xytext=(72, 24),
                arrowprops=dict(arrowstyle="->", color="#CA8A04", lw=1.5))
    ax.text(74, 31, "Quyết định phê duyệt (Selected / Reject)", color="#854D0E", fontsize=7, weight='bold', ha='left')

    # 6. DSS -> ERP
    ax.annotate("", xy=(50, 77), xytext=(50, 66),
                arrowprops=dict(arrowstyle="->", color="#64748B", lw=1.8))
    ax.text(51, 71.5, "Dự toán ngân sách tiếp thị &\nTổng giá trị nhập hàng đề xuất", color="#475569", fontsize=7.2, weight='bold', ha='left')

    # 7. DSS -> WMS
    ax.annotate("", xy=(28, 23), xytext=(40, 36),
                arrowprops=dict(arrowstyle="->", color="#4F46E5", lw=1.8, linestyle='--'))
    ax.text(28, 20.5, "Kế hoạch phân bổ số lượng đặt hàng Q4", color="#4F46E5", fontsize=7, weight='bold', ha='left')

    # Legend at bottom
    legend_elements = [
        Line2D([0], [0], color='#2563EB', lw=2.5, marker='o', markersize=6, label='Tiến trình Hệ thống DSS (Process 0)'),
        Line2D([0], [0], color='#64748B', lw=2, label='Thực thể bên ngoài (External Entity - Out-of-Scope)'),
        Line2D([0], [0], color='#059669', lw=2, marker='>', markersize=5, label='Luồng nạp dữ liệu vào (Data Inflow)'),
        Line2D([0], [0], color='#4F46E5', lw=2, marker='>', markersize=5, label='Luồng khuyến nghị tham mưu (Advisory Outflow)'),
        Line2D([0], [0], color='#CA8A04', lw=2, marker='>', markersize=5, label='Luồng tương tác tác nhân con người')
    ]
    ax.legend(handles=legend_elements, loc='lower center', bbox_to_anchor=(0.5, 0.01),
              ncol=3, fontsize=7.8, frameon=True, facecolor='#FFFFFF', edgecolor='#CBD5E1')

    plt.tight_layout()
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    plt.savefig(output_path, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print(f"SCD diagram successfully created at: {output_path}")

def generate_sdd_diagram(output_path="docs/images/sodo_luong_du_lieu_sdd.png"):
    """Vẽ Sơ đồ Luồng Dữ Liệu Phân Rã SDD (System Dataflow Diagram - DFD Mức 1)"""
    plt.rcParams['font.family'] = 'sans-serif'
    plt.rcParams['font.sans-serif'] = ['Segoe UI', 'Arial', 'Tahoma', 'DejaVu Sans']
    
    fig, ax = plt.subplots(figsize=(15.5, 10.5), dpi=300)
    ax.set_xlim(0, 100)
    ax.set_ylim(0, 100)
    ax.axis('off')
    
    fig.patch.set_facecolor('#FFFFFF')
    ax.set_facecolor('#FFFFFF')
    
    # Title Banner
    banner = patches.FancyBboxPatch((2, 92), 96, 7.2, boxstyle="round,pad=0.2,rounding_size=0.5",
                                    facecolor="#0F172A", edgecolor="#1E293B", linewidth=1.5)
    ax.add_patch(banner)
    ax.text(50, 96.2, "SƠ ĐỒ PHÂN RÃ LUỒNG DỮ LIỆU — SDD (SYSTEM DATAFLOW DIAGRAM / DFD MỨC 1)", 
            color="#FFFFFF", fontsize=13.5, weight='bold', ha='center', va='center')
    ax.text(50, 93.5, "KIẾN TRÚC 5 TIẾN TRÌNH CỐT LÕI & 2 KHO DỮ LIỆU CHUẨN 3NF • DSS RETAIL STUDIO 2026", 
            color="#94A3B8", fontsize=9, weight='medium', ha='center', va='center')

    # Process 1.0 (Top Left)
    p1 = patches.FancyBboxPatch((6, 72), 26, 15, boxstyle="round,pad=0.3,rounding_size=0.6",
                                facecolor="#F0FDF4", edgecolor="#16A34A", linewidth=2.0)
    ax.add_patch(p1)
    ax.text(19, 83.5, "1.0: TIẾP NHẬN & ETL LÀM SẠCH", color="#15803D", fontsize=9.2, weight='bold', ha='center')
    ax.text(19, 77.0, "• Đọc ORDERS, ORDER_DETAILS\n• Khử trùng lặp, tính doanh thu thực\n• Kiểm tra ràng buộc toàn vẹn khóa ngoại", color="#334155", fontsize=7.8, ha='center')

    # Process 2.0 (Top Right)
    p2 = patches.FancyBboxPatch((68, 72), 26, 15, boxstyle="round,pad=0.3,rounding_size=0.6",
                                facecolor="#EFF6FF", edgecolor="#2563EB", linewidth=2.0)
    ax.add_patch(p2)
    ax.text(81, 83.5, "2.0: THIẾT LẬP & TÍNH TOÁN AHP", color="#1E40AF", fontsize=9.2, weight='bold', ha='center')
    ax.text(81, 77.0, "• Ma trận Saaty 1-9 cho 5 tiêu chí\n• Kiểm định tỷ số nhất quán CR <= 0.10\n• Chuẩn hóa Benefit/Cost & tính điểm AHP", color="#334155", fontsize=7.8, ha='center')

    # Data Store D1: Phân Hệ Bán Lẻ (Center Top)
    d1 = patches.Rectangle((39, 73.5), 22, 12, facecolor="#F8FAFC", edgecolor="#0284C7", linewidth=1.8)
    ax.add_patch(d1)
    ax.plot([39, 39], [73.5, 85.5], color="#0284C7", lw=3)
    ax.text(50, 81.5, "D1: CSDL BÁN LẺ", color="#0369A1", fontsize=8.8, weight='bold', ha='center')
    ax.text(50, 76.5, "ORDERS, ORDER_DETAILS,\nPRODUCTS, CATEGORIES", color="#475569", fontsize=7.2, ha='center')

    # Process 3.0 (Center Left)
    p3 = patches.FancyBboxPatch((6, 42), 26, 16, boxstyle="round,pad=0.3,rounding_size=0.6",
                                facecolor="#FDF4FF", edgecolor="#A855F7", linewidth=2.0)
    ax.add_patch(p3)
    ax.text(19, 54.5, "3.0: MÔ PHỎNG WHAT-IF & OLAP", color="#7E22CE", fontsize=9.2, weight='bold', ha='center')
    ax.text(19, 47.5, "• 3 Kịch bản vi mô/vĩ mô định sẵn\n• Thanh trượt tùy biến tham số tức thời\n• Phân tích đa chiều: Slice, Dice, Drill-down", color="#334155", fontsize=7.8, ha='center')

    # Process 4.0 (Center Right)
    p4 = patches.FancyBboxPatch((68, 42), 26, 16, boxstyle="round,pad=0.3,rounding_size=0.6",
                                facecolor="#EFF6FF", edgecolor="#2563EB", linewidth=2.0)
    ax.add_patch(p4)
    ax.text(81, 54.5, "4.0: XẾP HẠNG & KHUYẾN NGHỊ", color="#1E40AF", fontsize=9.2, weight='bold', ha='center')
    ax.text(81, 47.5, "• Phân loại: Selected / Review / Reject\n• Gợi ý số lượng đặt: f(Lead Time, 30 ngày)\n• Phân bổ tỷ trọng ngân sách marketing", color="#334155", fontsize=7.8, ha='center')

    # Data Store D2: Phân Hệ DSS (Center Middle)
    d2 = patches.Rectangle((39, 44), 22, 12, facecolor="#F8FAFC", edgecolor="#0284C7", linewidth=1.8)
    ax.add_patch(d2)
    ax.plot([39, 39], [44, 56], color="#0284C7", lw=3)
    ax.text(50, 52.0, "D2: CSDL QUYẾT ĐỊNH DSS", color="#0369A1", fontsize=8.8, weight='bold', ha='center')
    ax.text(50, 47.0, "CRITERIA, COMPARISONS,\nDECISION_RESULTS, SESSIONS", color="#475569", fontsize=7.2, ha='center')

    # Process 5.0 (Bottom Center)
    p5 = patches.FancyBboxPatch((32, 14), 36, 16, boxstyle="round,pad=0.3,rounding_size=0.6",
                                facecolor="#FEFCE8", edgecolor="#CA8A04", linewidth=2.0)
    ax.add_patch(p5)
    ax.text(50, 26.5, "5.0: DASHBOARD KPI & BÁO CÁO THẨM ĐỊNH", color="#854D0E", fontsize=9.5, weight='bold', ha='center')
    ax.text(50, 19.5, "• Trực quan hóa 4 thẻ KPI, biểu đồ cột AHP & biểu đồ Delta\n• Tự động sinh biên bản tóm tắt Executive Summary\n• Xuất báo cáo thẩm định định dạng Word (.docx) & HTML", color="#334155", fontsize=7.8, ha='center')

    # External Entities
    # POS (Left Top Outside)
    ax.text(3, 85, "[POS] Hệ Thống POS", color="#64748B", fontsize=8.5, weight='bold')
    ax.annotate("", xy=(6, 80), xytext=(1, 80), arrowprops=dict(arrowstyle="->", color="#059669", lw=2.0))

    # Analyst (Right Top Outside)
    ax.text(88, 89, "[Analyst] Chuyên Viên", color="#854D0E", fontsize=8.5, weight='bold')
    ax.annotate("", xy=(88, 83), xytext=(94, 83), arrowprops=dict(arrowstyle="->", color="#CA8A04", lw=1.8))

    # Manager (Bottom Outside)
    ax.text(50, 8, "[Manager] Nhà Quản Trị (Decision Maker - Phê duyệt)", color="#854D0E", fontsize=9, weight='bold', ha='center')
    ax.annotate("", xy=(50, 10), xytext=(50, 14), arrowprops=dict(arrowstyle="<-", color="#CA8A04", lw=2.2))

    # CONNECTING DATA FLOWS
    # P1.0 -> D1
    ax.annotate("", xy=(39, 79.5), xytext=(32, 79.5), arrowprops=dict(arrowstyle="->", color="#059669", lw=1.8))
    ax.text(35.5, 81.2, "Dữ liệu sạch", color="#059669", fontsize=7, weight='bold', ha='center')

    # D1 -> P2.0
    ax.annotate("", xy=(68, 79.5), xytext=(61, 79.5), arrowprops=dict(arrowstyle="->", color="#0284C7", lw=1.8))
    ax.text(64.5, 81.2, "5 Tiêu chí thô", color="#0284C7", fontsize=7, weight='bold', ha='center')

    # P2.0 -> D2
    ax.annotate("", xy=(58, 56), xytext=(72, 72), arrowprops=dict(arrowstyle="->", color="#2563EB", lw=1.8))
    ax.text(67, 63, "Trọng số Saaty & CR", color="#1E40AF", fontsize=7, weight='bold', ha='left')

    # D2 -> P4.0
    ax.annotate("", xy=(68, 50), xytext=(61, 50), arrowprops=dict(arrowstyle="->", color="#0284C7", lw=1.8))
    ax.text(64.5, 51.5, "Điểm AHP", color="#0284C7", fontsize=7, weight='bold', ha='center')

    # P3.0 <-> P4.0
    ax.annotate("", xy=(32, 50), xytext=(39, 50), arrowprops=dict(arrowstyle="<-", color="#A855F7", lw=1.5))
    ax.text(35.5, 51.5, "Dữ liệu SKU", color="#7E22CE", fontsize=7, weight='bold', ha='center')

    ax.annotate("", xy=(68, 46), xytext=(32, 46), arrowprops=dict(arrowstyle="<->", color="#A855F7", lw=1.8, linestyle='--'))
    ax.text(50, 43.5, "Mô phỏng What-If & Tái tính toán xếp hạng AHP", color="#7E22CE", fontsize=7.2, weight='bold', ha='center')

    # P4.0 -> P5.0
    ax.annotate("", xy=(58, 30), xytext=(72, 42), arrowprops=dict(arrowstyle="->", color="#4F46E5", lw=2.0))
    ax.text(68, 35, "Bảng xếp hạng & Số lượng gợi ý", color="#4F46E5", fontsize=7.2, weight='bold', ha='left')

    # D1 -> P3.0 (OLAP)
    ax.annotate("", xy=(24, 58), xytext=(42, 73.5), arrowprops=dict(arrowstyle="->", color="#0284C7", lw=1.5, linestyle=':'))
    ax.text(31, 66, "Lịch sử đơn hàng OLAP", color="#0369A1", fontsize=7, weight='bold', ha='center')

    # P5.0 -> D2 (Save final decision session)
    ax.annotate("", xy=(46, 44), xytext=(46, 30), arrowprops=dict(arrowstyle="->", color="#0284C7", lw=1.5))
    ax.text(47, 37, "Lưu phiên DECISION_SESSIONS", color="#0369A1", fontsize=6.8, weight='bold', ha='left')

    plt.tight_layout()
    os.makedirs(os.path.dirname(output_path), exist_ok=True)
    plt.savefig(output_path, facecolor=fig.get_facecolor(), edgecolor='none')
    plt.close()
    print(f"SDD diagram successfully created at: {output_path}")

if __name__ == "__main__":
    generate_scd_diagram()
    generate_sdd_diagram()
