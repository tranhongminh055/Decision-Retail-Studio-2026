# -*- coding: utf-8 -*-
"""
Script tong hop va sinh Tai lieu Huong dan Su dung Phan mem (User Manual / Operating Guide)
File Word: docs/TAI_LIEU_HUONG_DAN_SU_DUNG_DSS_IS402_NHOM05.docx
File Markdown: docs/TAI_LIEU_HUONG_DAN_SU_DUNG_DSS_IS402_NHOM05.md
He thong Ho tro Ra quyet dinh DSS Retail Decision Studio 2026
Hoc phan: IS 402 A - Nhom 05 - Duy Tan University
"""

import os
import sys
import docx
from docx import Document
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT, WD_ALIGN_VERTICAL

# Add scripts directory to path if needed
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from docx_helpers import set_cell_background, set_cell_margins, set_table_borders, add_callout
from docx_builder_base import (
    create_document, add_title, add_heading_1, add_heading_2, add_heading_3,
    add_body_p, add_bullet_p, add_image_figure, create_styled_table
)

def build_user_manual_docx():
    print("Khoi tao Document Huong dan Su dung Phan mem DSS...")
    doc = create_document()
    
    # Custom Header & Footer
    header = doc.sections[0].header
    hp = header.paragraphs[0]
    hp.text = ""
    hp.alignment = WD_ALIGN_PARAGRAPH.LEFT
    hr = hp.add_run("TÀI LIỆU HƯỚNG DẪN SỬ DỤNG HỆ THỐNG (DSS SYSTEM USER MANUAL & OPERATING GUIDE)")
    hr.font.name = "Segoe UI"
    hr.font.size = Pt(8)
    hr.font.color.rgb = RGBColor(148, 163, 184)
    
    footer = doc.sections[0].footer
    fp = footer.paragraphs[0]
    fp.text = ""
    fp.alignment = WD_ALIGN_PARAGRAPH.RIGHT
    fr = fp.add_run("IS402 A • Đồ Án DSS Retail Decision Studio 2026 • Nhóm 05")
    fr.font.name = "Segoe UI"
    fr.font.size = Pt(8)
    fr.font.italic = True
    fr.font.color.rgb = RGBColor(148, 163, 184)

    # =========================================================================
    # TRANG BÌA (COVER PAGE)
    # =========================================================================
    p_inst = doc.add_paragraph()
    p_inst.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_inst.paragraph_format.space_before = Pt(12)
    p_inst.paragraph_format.space_after = Pt(2)
    r = p_inst.add_run("DUY TÂN UNIVERSITY – INTERNATIONAL SCHOOL\n")
    r.font.name = "Segoe UI"
    r.font.size = Pt(13)
    r.font.bold = True
    r.font.color.rgb = RGBColor(15, 23, 42)
    
    r2 = p_inst.add_run("KHOA HỆ THỐNG THÔNG TIN QUẢN LÝ (MIS) • HỌC PHẦN: IS 402 A\n")
    r2.font.name = "Segoe UI"
    r2.font.size = Pt(11)
    r2.font.bold = True
    r2.font.color.rgb = RGBColor(79, 70, 229)
    
    p_div = doc.add_paragraph()
    p_div.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_div.paragraph_format.space_after = Pt(25)
    r_div = p_div.add_run("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━")
    r_div.font.name = "Segoe UI"
    r_div.font.color.rgb = RGBColor(203, 213, 225)
    
    p_title = doc.add_paragraph()
    p_title.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_title.paragraph_format.space_before = Pt(15)
    p_title.paragraph_format.space_after = Pt(8)
    r_t = p_title.add_run("TÀI LIỆU HƯỚNG DẪN SỬ DỤNG PHẦN MỀM\nHỆ THỐNG HỖ TRỢ RA QUYẾT ĐỊNH\n(DSS USER MANUAL & SYSTEM OPERATION GUIDE)\n")
    r_t.font.name = "Segoe UI"
    r_t.font.size = Pt(18)
    r_t.font.bold = True
    r_t.font.color.rgb = RGBColor(15, 23, 42)
    
    p_sub = doc.add_paragraph()
    p_sub.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_sub.paragraph_format.space_after = Pt(25)
    r_s = p_sub.add_run("ĐỀ TÀI: HỆ THỐNG HỖ TRỢ RA QUYẾT ĐỊNH CHIẾN LƯỢC KHUYẾN MÃI & PHÂN BỔ HÀNG HÓA\n")
    r_s.font.name = "Segoe UI"
    r_s.font.size = Pt(11.5)
    r_s.font.bold = True
    r_s.font.color.rgb = RGBColor(16, 185, 129)
    
    r_s2 = p_sub.add_run("Phần mềm: DSS Retail Decision Studio 2026 (C# .NET 10.0 WinForms & SQL Server DSS_SalesDB)")
    r_s2.font.name = "Segoe UI"
    r_s2.font.size = Pt(10)
    r_s2.font.italic = True
    r_s2.font.color.rgb = RGBColor(100, 116, 139)
    
    # Metadata Box
    p_box = doc.add_paragraph()
    p_box.paragraph_format.space_before = Pt(40)
    p_box.paragraph_format.space_after = Pt(40)
    
    tbl_meta = doc.add_table(rows=6, cols=2)
    tbl_meta.alignment = WD_TABLE_ALIGNMENT.CENTER
    set_table_borders(tbl_meta, color="E2E8F0", sz="4")
    
    meta_rows = [
        ("Nhóm sinh viên thực hiện:", "NHÓM 05 (Lớp: IS 402 A)"),
        ("1. Trần Hồng Minh (Trưởng nhóm):", "MSSV: 29210247142"),
        ("2. Lê Thị Nga • 3. Phạm Ngọc Tú:", "MSSV: 29206361029 • MSSV: 29204347819"),
        ("4. Lê Thanh Diệu • 5. Hồ Thị Thuận:", "MSSV: 29204622434 • MSSV: 29201164457"),
        ("Giảng viên hướng dẫn (Approved by):", "ThS. Nguyễn Quang Ánh"),
        ("Thời điểm nghiệm thu & phát hành:", "Tháng 10/2026 – Đà Nẵng, Việt Nam")
    ]
    for idx, (c1, c2) in enumerate(meta_rows):
        cell1, cell2 = tbl_meta.rows[idx].cells
        cell1.text = c1
        cell2.text = c2
        set_cell_background(cell1, "F8FAFC" if idx % 2 == 0 else "FFFFFF")
        set_cell_background(cell2, "F8FAFC" if idx % 2 == 0 else "FFFFFF")
        set_cell_margins(cell1, top=60, bottom=60, left=120, right=100)
        set_cell_margins(cell2, top=60, bottom=60, left=100, right=120)
        cell1.paragraphs[0].runs[0].font.name = "Segoe UI"
        cell1.paragraphs[0].runs[0].font.size = Pt(9.5)
        cell1.paragraphs[0].runs[0].font.bold = (idx in [0, 4])
        cell2.paragraphs[0].runs[0].font.name = "Segoe UI"
        cell2.paragraphs[0].runs[0].font.size = Pt(9.5)
        cell2.paragraphs[0].runs[0].font.bold = (idx in [0, 4])
        cell1.width = Inches(3.2)
        cell2.width = Inches(3.3)
        
    doc.add_page_break()

    # =========================================================================
    # DANH MỤC & MỤC LỤC TỔNG QUAN
    # =========================================================================
    add_heading_1(doc, "MỤC LỤC VÀ DANH MỤC HÌNH ẢNH HƯỚNG DẪN")
    
    add_body_p(doc, "Tài liệu này cung cấp hướng dẫn vận hành chi tiết từng bước (Step-by-Step Manual) cho toàn bộ các chức năng của phần mềm DSS Retail Decision Studio 2026, từ bước thiết lập môi trường CSDL SQL Server, quy trình xác thực phân quyền RBAC đến các thao tác tính toán AHP Saaty, mô phỏng What-If độ nhạy và xuất hồ sơ thẩm định doanh nghiệp.")
    
    add_heading_2(doc, "Danh Mục Các Hình Ảnh Minh Họa Hướng Dẫn Thao Tác")
    fig_list = [
        ("Hình 1", "Màn hình Khởi động & Kiểm tra kết nối CSDL tự động (Splash Screen)", "Mục 2.4"),
        ("Hình 2", "Màn hình Đăng nhập xác thực SHA-256 & Chọn nhanh tài khoản Demo (Login Form)", "Mục 3.1"),
        ("Hình 3", "Màn hình Đăng ký tài khoản người dùng mới & Phân quyền RBAC (Register Form)", "Mục 3.2"),
        ("Hình 4", "Tab 1: Kiểm tra danh mục 9 sản phẩm & Thiết lập ma trận Saaty 5x5", "Mục 4.1"),
        ("Hình 5", "Tab 2: Xem kết quả xếp hạng DSS, 4 KPI, biểu đồ AHP & khuyến nghị quản trị", "Mục 4.2"),
        ("Hình 6", "Tab 3: Mô phỏng What-If với 3 kịch bản thị trường & Biểu đồ đường Delta Line Chart", "Mục 4.3"),
        ("Hình 7", "Tab 4: Thêm sản phẩm mới, tự tính biên LN C1 & thẩm định động tức thời", "Mục 4.4"),
        ("Hình 8", "Tab 5: Khai phá dữ liệu bán hàng lịch sử OLAP (Fact View, ROLLUP, CUBE)", "Mục 4.5"),
        ("Hình 9", "Tab 6: Biểu mẫu báo cáo thẩm định BM-06, xuất file Word và in ấn PDF HTML", "Mục 4.6"),
        ("Hình 10", "Màn hình Khóa an toàn Fail-Safe khi ngắt kết nối CSDL (DatabaseGuard Overlay)", "Mục 5.1")
    ]
    create_styled_table(doc, ["Mã Hình", "Nội Dung Thao Tác Minh Họa Trong Hệ Thống DSS", "Vị Trí"], fig_list, [Inches(1.0), Inches(4.5), Inches(1.0)])

    add_heading_2(doc, "Bảng Quy Ước Ký Hiệu & Thuật Ngữ Nghiệp Vụ")
    term_rows = [
        ("AHP (Saaty)", "Analytic Hierarchy Process - Phương pháp phân tích thứ bậc đa tiêu chí do Thomas L. Saaty đề xuất (1980)."),
        ("CR (Consistency Ratio)", "Tỷ số nhất quán ma trận Saaty. Quy chuẩn: CR ≤ 0.10 (≤ 10%) là ma trận hợp lệ, nếu CR > 0.10 hệ thống sẽ cảnh báo đỏ và khóa tính điểm."),
        ("CI / RI", "Consistency Index (Chỉ số nhất quán) và Random Index (Chỉ số ngẫu nhiên = 1.12 với ma trận n=5)."),
        ("Benefit Criteria", "Tiêu chí Lợi ích (Càng lớn càng tốt): C1 (Biên lợi nhuận), C2 (Doanh thu kỳ vọng), C3 (Nhu cầu thị trường). Công thức chuẩn hóa: r = v / v_max."),
        ("Cost Criteria", "Tiêu chí Chi phí/Rủi ro (Càng nhỏ càng tốt): C4 (Tồn kho/Vòng quay), C5 (Chi phí Marketing). Công thức chuẩn hóa: r = v_min / v."),
        ("OLAP", "Online Analytical Processing - Phân tích dữ liệu đa chiều hỗ trợ ra quyết định kinh doanh bán lẻ."),
        ("RBAC", "Role-Based Access Control - Cơ chế kiểm soát quyền truy cập dựa trên 4 vai trò người dùng trong tổ chức."),
        ("DatabaseGuard", "Mô-đun kiểm soát nhịp tim mạng SQL Server thời gian thực, tự động khóa giao diện để bảo vệ tính toàn vẹn dữ liệu.")
    ]
    create_styled_table(doc, ["Ký Hiệu / Thuật Ngữ", "Ý Nghĩa & Ngữ Cảnh Ứng Dụng Trong Hệ Thống"], term_rows, [Inches(1.8), Inches(4.7)])

    doc.add_page_break()

    # =========================================================================
    # PHẦN 1: GIỚI THIỆU CHUNG & YÊU CẦU MÔI TRƯỜNG VẬN HÀNH
    # =========================================================================
    add_heading_1(doc, "PHẦN 1: GIỚI THIỆU CHUNG & YÊU CẦU MÔI TRƯỜNG VẬN HÀNH")

    add_heading_2(doc, "1.1. Giới Thiệu Tổng Quan Hệ Thống DSS Retail Decision Studio 2026")
    add_body_p(doc, "DSS Retail Decision Studio 2026 là phần mềm hệ thống thông tin hỗ trợ ra quyết định (Decision Support System - DSS) chuyên dụng dành cho các chuỗi bán lẻ công nghệ và thiết bị thông minh. Phần mềm được thiết kế và hiện thực hóa trong khuôn khổ đồ án môn học IS 402 A của Khoa Hệ thống Thông tin Quản lý (MIS) – Trường Đào tạo Quốc tế, Đại học Duy Tân.")
    add_body_p(doc, "Phần mềm giải quyết bài toán cốt lõi của doanh nghiệp bán lẻ trước mỗi chu kỳ kinh doanh Quý 4/2026:")
    add_bullet_p(doc, "Đánh giá, so sánh và xếp hạng khoa học danh mục 9 dòng sản phẩm chủ lực (thuộc 4 ngành hàng: Smartphone, Laptop, Phụ kiện, Smart Home).", "• Tối ưu hóa danh mục sản phẩm: ");
    add_bullet_p(doc, "Loại bỏ tính chủ quan, cảm tính nhờ kết hợp 5 tiêu chí định lượng và định tính thông qua ma trận so sánh cặp Saaty.", "• Minh bạch hóa mô hình toán học: ");
    add_bullet_p(doc, "Tự động phân loại phương án thành 3 nhóm (Selected - Phê duyệt, UnderReview - Cần xem xét, Rejected - Loại bỏ), gợi ý số lượng đặt hàng an toàn và điều tiết ngân sách xúc tiến bán lẻ.", "• Khuyến nghị hành động tự động: ");
    add_bullet_p(doc, "Cung cấp công cụ mô phỏng phân tích độ nhạy What-If thời gian thực và khai phá dữ liệu bán hàng lịch sử OLAP.", "• Ứng phó biến động thị trường: ");

    add_heading_2(doc, "1.2. Yêu Cầu Cấu Hình Phần Cứng & Môi Trường Vận Hành")
    add_body_p(doc, "Để đảm bảo phần mềm hoạt động ổn định, mượt mà và không bị gián đoạn tính toán, thiết bị máy tính người dùng cần đáp ứng các tiêu chuẩn cấu hình kỹ thuật sau:")
    
    req_data = [
        ("Bộ vi xử lý (CPU)", "Intel Core i3 thế hệ 8 hoặc AMD Ryzen 3 trở lên", "Intel Core i5 / i7 thế hệ 11+ hoặc Apple Silicon M-series (qua Parallels)"),
        ("Bộ nhớ RAM", "Tối thiểu 4.0 GB RAM trống", "8.0 GB - 16.0 GB RAM DDR4/DDR5 tốc độ cao"),
        ("Ổ đĩa cứng (Storage)", "Tối thiểu 1.0 GB dung lượng trống (SSD)", "SSD NVMe PCIe trống tối thiểu 5.0 GB"),
        ("Hệ điều hành (OS)", "Microsoft Windows 10 x64 (Build 19041+)", "Microsoft Windows 11 x64 Pro (phiên bản mới nhất)"),
        ("Môi trường thực thi (.NET)", ".NET Desktop Runtime 10.0 (x64)", ".NET SDK 10.0 hoặc .NET Desktop Runtime 10.0.0+"),
        ("Hệ quản trị CSDL (DBMS)", "Microsoft SQL Server 2022 / 2025 Express", "SQL Server 2022/2025 Developer hoặc Enterprise Edition"),
        ("Độ phân giải màn hình", "1366 x 768 pixels (Chuẩn HD)", "1920 x 1080 (Full HD) hoặc cao hơn, hỗ trợ High-DPI"),
        ("Phần mềm phụ trợ", "Microsoft Word / Excel 2016+ (xem báo cáo)", "Microsoft Word 365, Google Chrome / MS Edge (xem báo cáo HTML)")
    ]
    create_styled_table(doc, ["Thành Phần Kỹ Thuật", "Cấu Hình Tối Thiểu (Minimum)", "Cấu Hình Khuyến Nghị (Recommended)"], req_data, [Inches(1.8), Inches(2.3), Inches(2.4)])

    add_heading_2(doc, "1.3. Danh Mục Tài Khoản Đăng Nhập Mẫu & Phân Quyền RBAC")
    add_body_p(doc, "Hệ thống cài đặt sẵn 4 tài khoản mẫu đại diện cho 4 vai trò người dùng trong doanh nghiệp bán lẻ, phục vụ quá trình chấm thi và kiểm thử vận hành:")
    
    acc_rows = [
        ("NV000", "Nguyễn Huy Hoàng", "DecisionMaker", "Admin@123", "Toàn quyền ra quyết định, chạy AHP, duyệt kế hoạch nhập hàng, ký duyệt báo cáo thẩm định BM-06."),
        ("admin", "Quản Trị Viên", "Administrator", "Admin@123", "Toàn quyền cấu hình kết nối CSDL, sao lưu/phục hồi dữ liệu, cấp phát tài khoản nhân viên mới."),
        ("NV002", "Trần Thị Mai", "SalesManager", "Admin@123", "Khai phá dữ liệu đa chiều OLAP, phân tích độ nhạy What-If, xuất báo cáo Word và in ấn PDF."),
        ("NV003", "Lê Văn An", "SalesStaff", "Admin@123", "Tra cứu hiệu suất bán lẻ, theo dõi tồn kho, chỉ có quyền xem (Read-Only) trên các bảng tính DSS.")
    ]
    create_styled_table(doc, ["Tài Khoản", "Họ Và Tên Cán Bộ", "Vai Trò (Role)", "Mật Khẩu Mẫu", "Quyền Hạn Và Phạm Vi Tác Vụ Trong Hệ Thống"], acc_rows, [Inches(0.9), Inches(1.3), Inches(1.1), Inches(1.0), Inches(2.2)])

    add_callout(doc, "Mọi mật khẩu trong hệ thống đều được băm bảo mật bằng thuật toán SHA-256 trước khi lưu trữ vào bảng dbo.USERS trong SQL Server. Người dùng có thể sử dụng trực tiếp các nút 1-click Demo trên màn hình đăng nhập để chuyển đổi nhanh giữa các vai trò mà không cần gõ thủ công.", "LƯU Ý VỀ TÀI KHOẢN & BẢO MẬT MẬT KHẨU:")

    doc.add_page_break()

    # =========================================================================
    # PHẦN 2: HƯỚNG DẪN CÀI ĐẶT & THIẾT LẬP CƠ SỞ DỮ LIỆU
    # =========================================================================
    add_heading_1(doc, "PHẦN 2: HƯỚNG DẪN CÀI ĐẶT & THIẾT LẬP CƠ SỞ DỮ LIỆU")

    add_heading_2(doc, "2.1. Thiết Lập Cơ Sở Dữ Liệu SQL Server (DSS_SalesDB)")
    add_body_p(doc, "Hệ thống yêu cầu cơ sở dữ liệu Microsoft SQL Server phải được khởi tạo trước khi chạy phần mềm. Các bước thực hiện tuần tự như sau:")
    add_bullet_p(doc, "Khởi động công cụ Microsoft SQL Server Management Studio (SSMS) hoặc Azure Data Studio trên máy tính.", "Bước 1: Mở công cụ quản trị: ");
    add_bullet_p(doc, "Kết nối tới máy chủ SQL nội bộ (ví dụ: .\\SQLEXPRESS25 hoặc localhost).", "Bước 2: Kết nối Server: ");
    add_bullet_p(doc, "Mở và thực thi file script 'sql/01_create_database.sql'. Script này sẽ tự động tạo CSDL DSS_SalesDB, thiết lập 11 bảng thực thể đạt chuẩn 3NF, cùng hệ thống khóa chính, khóa ngoại, ràng buộc CHECK và chỉ mục (Indexes).", "Bước 3: Tạo cấu trúc Database: ");
    add_bullet_p(doc, "Mở và thực thi file script 'sql/02_seed_data.sql'. Script sẽ nạp dữ liệu mẫu 9 sản phẩm ứng viên, tham số 5 tiêu chí và ma trận so sánh cặp Saaty chuẩn toán học có CR = 0.0152 (1.52% ≤ 10%).", "Bước 4: Nạp dữ liệu hạt giống (Seed Data): ");
    add_bullet_p(doc, "Mở và thực thi file script 'sql/03_dss_reports_and_queries.sql' để khởi tạo các View hiệu suất bán hàng (vw_DSS_ProductPerformance) và Stored Procedures tính toán DSS.", "Bước 5: Tạo Views & Thủ tục: ");

    add_heading_2(doc, "2.2. Kiểm Tra Cấu Hình Chuỗi Kết Nối (Connection String)")
    add_body_p(doc, "Chuỗi kết nối chuẩn tới cơ sở dữ liệu SQL Server được cấu hình trong lớp C# DatabaseGuard / DbConfig như sau:")
    add_bullet_p(doc, "Data Source=.\\SQLEXPRESS25;Initial Catalog=DSS_SalesDB;Integrated Security=True;TrustServerCertificate=True;Connect Timeout=5;", "• Chuỗi kết nối Windows Authentication: ");
    add_body_p(doc, "• Trường hợp sử dụng instance khác: Người dùng có thể sửa tên instance trong file mã nguồn hoặc cấu hình chuỗi kết nối trực tiếp trong file cài đặt trước khi biên dịch.")

    add_heading_2(doc, "2.3. Ba Cách Khởi Chạy Ứng Dụng Nhanh Chóng")
    add_body_p(doc, "Người dùng có thể lựa chọn 1 trong 3 phương thức sau để khởi chạy phần mềm:")
    add_bullet_p(doc, "Nhấp đúp chuột vào tệp tin 'CHAY_HE_THONG_GUI_CSHARP.bat' tại thư mục gốc của dự án. File batch sẽ tự động kiểm tra .NET SDK, biên dịch ứng dụng và kích hoạt màn hình đồ họa chỉ trong 3 giây.", "• Cách 1: 1-Click File Batch: ");
    add_bullet_p(doc, "Mở cửa sổ PowerShell hoặc Command Prompt tại thư mục dự án và thực hiện lệnh: dotnet run --project DSS_App/DSS_App.csproj", "• Cách 2: Lệnh Terminal (.NET CLI): ");
    add_bullet_p(doc, "Mở thư mục dự án bằng Visual Studio 2022/2025, chọn cấu hình 'Debug / Any CPU' và nhấn phím tắt F5 (Start Debugging).", "• Cách 3: Visual Studio IDE: ");

    add_heading_2(doc, "2.4. Màn Hình Khởi Động & Giám Sát Kết Nối (Splash Screen)")
    add_body_p(doc, "Khi phần mềm được kích hoạt, màn hình SplashForm sẽ xuất hiện đầu tiên:")
    
    add_image_figure(doc, "docs/wireframes/GUI_01_Splash_Screen.png", "Hình 1: Màn hình Khởi động & Kiểm tra kết nối CSDL tự động (Splash Screen - GUI-01)")
    
    add_body_p(doc, "• Diễn giải thao tác:")
    add_bullet_p(doc, "Người dùng không cần thực hiện thao tác chuột hay bàn phím trên màn hình này.", "1. Tự động hóa hoàn toàn: ");
    add_bullet_p(doc, "Thanh ProgressBar màu xanh Emerald sẽ tự động tăng dần từ 0% đến 100%.", "2. Tiến trình nạp: ");
    add_bullet_p(doc, "Hệ thống ngầm gọi hàm DatabaseGuard.Ping() kiểm tra máy chủ SQL Server. Nếu hộp trạng thái ở góc dưới bên trái hiển thị 'DatabaseGuard: Online • Latency: 3.2ms', hệ thống đã sẵn sàng.", "3. Giám sát CSDL: ");
    add_bullet_p(doc, "Sau khi đạt 100%, SplashForm sẽ tự động đóng và chuyển tiếp mượt mà sang màn hình Đăng nhập (LoginForm).", "4. Chuyển tiếp tự động: ");

    doc.add_page_break()

    # =========================================================================
    # PHẦN 3: HƯỚNG DẪN XÁC THỰC & BẢO MẬT HỆ THỐNG
    # =========================================================================
    add_heading_1(doc, "PHẦN 3: HƯỚNG DẪN XÁC THỰC & BẢO MẬT HỆ THỐNG")

    add_heading_2(doc, "3.1. Hướng Dẫn Đăng Nhập Hệ Thống (Login Form)")
    add_body_p(doc, "Giao diện LoginForm cho phép xác thực người dùng an toàn:")
    
    add_image_figure(doc, "docs/wireframes/GUI_02_Login_Form.png", "Hình 2: Màn hình Đăng nhập xác thực SHA-256 & 4 Nút Chọn Nhanh Demo (GUI-02)")
    
    add_body_p(doc, "• Quy trình đăng nhập thủ công:")
    add_bullet_p(doc, "Nhập mã nhân viên hoặc tên đăng nhập vào ô 'Tài khoản' (ví dụ: NV000).", "Bước 1: ");
    add_bullet_p(doc, "Nhập mật khẩu vào ô 'Mật khẩu' (mặc định ký tự bị ẩn dưới dạng dấu chấm tròn ••••••••). Nhấp biểu tượng con mắt 👁 để hiển thị hoặc ẩn mật khẩu.", "Bước 2: ");
    add_bullet_p(doc, "Tích chọn ô 'Ghi nhớ tài khoản trên thiết bị này' nếu muốn lưu mã đăng nhập cho các phiên làm việc sau.", "Bước 3: ");
    add_bullet_p(doc, "Nhấn nút '🔐 ĐĂNG NHẬP HỆ THỐNG' màu Indigo hoặc nhấn phím tắt Enter trên bàn phím để đăng nhập.", "Bước 4: ");

    add_body_p(doc, "• Tính năng 1-Click Chọn Nhanh Tài Khoản Demo (Dành cho Giám khảo & Kiểm thử):")
    add_body_p(doc, "Tại khung 'TÀI KHOẢN KIỂM THỬ NHANH (1-CLICK DEMO)', người dùng chỉ cần nhấp vào 1 trong 4 nút định sẵn:")
    add_bullet_p(doc, "Nút '👑 DecisionMaker: NV000 (Nguyễn Huy Hoàng)' ➔ Tự động điền tài khoản Người ra quyết định cao nhất.", "• Tài khoản DecisionMaker: ");
    add_bullet_p(doc, "Nút '⚙️ Administrator: admin (Quản trị hệ thống)' ➔ Tự động điền tài khoản Quản trị viên CSDL.", "• Tài khoản Quản trị: ");
    add_bullet_p(doc, "Nút '📊 SalesManager: NV002 (Trần Thị Mai)' ➔ Tự động điền tài khoản Trưởng phòng Phân tích & OLAP.", "• Tài khoản SalesManager: ");
    add_bullet_p(doc, "Nút '💼 SalesStaff: NV003 (Lê Văn An)' ➔ Tự động điền tài khoản Chuyên viên bán hàng.", "• Tài khoản SalesStaff: ");

    add_heading_2(doc, "3.2. Hướng Dẫn Đăng Ký Tài Khoản Người Dùng Mới (Register Form)")
    add_body_p(doc, "Trường hợp doanh nghiệp tiếp nhận cán bộ mới, người dùng có thể tạo tài khoản từ màn hình RegisterForm:")
    
    add_image_figure(doc, "docs/wireframes/GUI_03_Register_Form.png", "Hình 3: Màn hình Đăng ký tài khoản người dùng mới & Phân quyền RBAC (GUI-03)")
    
    add_body_p(doc, "• Các bước đăng ký tài khoản:")
    add_bullet_p(doc, "Từ màn hình LoginForm, nhấp vào liên kết 'Chưa có tài khoản? 👉 Đăng ký tài khoản mới'.", "Bước 1: ");
    add_bullet_p(doc, "Điền đầy đủ thông tin vào các trường bắt buộc (*): Họ và tên, Mã nhân viên (dạng NVxxx), Tên đăng nhập (4-30 ký tự, không trùng lặp), Email doanh nghiệp (phải có định dạng chuẩn regex @domain.vn), Mật khẩu và Xác nhận lại mật khẩu.", "Bước 2: ");
    add_bullet_p(doc, "Chọn vai trò trong hộp chọn 'Vai trò phân quyền (RBAC)': DecisionMaker, Administrator, SalesManager hoặc SalesStaff.", "Bước 3: ");
    add_bullet_p(doc, "Nhấn nút '✅ ĐĂNG KÝ TÀI KHOẢN'. Hệ thống kiểm tra toàn vẹn dữ liệu, tự động băm SHA-256 mật khẩu và lưu vào CSDL. Thông báo đăng ký thành công sẽ hiển thị và chuyển người dùng về LoginForm để đăng nhập.", "Bước 4: ");

    add_heading_2(doc, "3.3. Thông Tin Người Dùng & Quản Lý Phiên Trên Thanh Header")
    add_body_p(doc, "Sau khi đăng nhập thành công vào màn hình chính (Form1), thanh Header phía trên cùng luôn hiển thị:")
    add_bullet_p(doc, "Tên người dùng đang đăng nhập (ví dụ: 'Nguyễn Huy Hoàng • NV000').", "• Danh tính: ");
    add_bullet_p(doc, "Huy hiệu vai trò nổi bật: Badge 'DecisionMaker' (Vàng gold), 'Administrator' (Xanh dương), 'SalesManager' (Xanh lá), 'SalesStaff' (Xám).", "• Vai trò: ");
    add_bullet_p(doc, "Đèn báo CSDL thời gian thực: '🟢 CSDL: Online • Latency: 2.8ms'.", "• Trạng thái hệ thống: ");
    add_bullet_p(doc, "Nút '🚪 Đăng xuất' màu đỏ ở góc trên bên phải: Nhấp nút này sẽ mở hộp thoại xác nhận 'Bạn có chắc chắn muốn đăng xuất phiên làm việc?'. Nếu chọn Yes, hệ thống xóa phiên làm việc an toàn và quay về LoginForm.", "• Đăng xuất an toàn: ");

    doc.add_page_break()

    # =========================================================================
    # PHẦN 4: HƯỚNG DẪN THAO TÁC CÁC CHỨC NĂNG NGHIỆP VỤ (6 TABS)
    # =========================================================================
    add_heading_1(doc, "PHẦN 4: HƯỚNG DẪN CHI TIẾT THAO TÁC NGHIỆP VỤ DSS (HỆ THỐNG 6 TABS)")

    # -------------------------------------------------------------------------
    # 4.1. TAB 1
    # -------------------------------------------------------------------------
    add_heading_2(doc, "4.1. Tab 1: Thiết Lập Dữ Liệu Đầu Vào & Ma Trận Saaty 5x5 (Inputs & Parameters)")
    add_body_p(doc, "Tab 1 là điểm khởi đầu của quy trình hỗ trợ ra quyết định, nơi người dùng kiểm tra danh mục 9 sản phẩm ứng viên và thiết lập ma trận so sánh cặp giữa 5 tiêu chí:")
    
    add_image_figure(doc, "docs/wireframes/GUI_04_Main_Tab1_Inputs.png", "Hình 4: Tab 1 - Danh Mục 9 Sản Phẩm Ứng Viên & Ma Trận So Sánh Cặp Saaty 5x5 (GUI-04)")
    
    add_body_p(doc, "• Hướng dẫn kiểm tra danh mục sản phẩm (Cột trái):")
    add_bullet_p(doc, "Bảng danh mục bên trái hiển thị 9 sản phẩm ứng viên: Asus ROG Strix G16, Acer Nitro 16, Dell Alienware m16, iPhone 16 Pro Max, Samsung Galaxy S24 Ultra, Xiaomi 14 Pro, Sony WH-1000XM5, Marshall Stanmore III, DJI Mini 4 Pro.", "1. Danh mục 9 SKU: ");
    add_bullet_p(doc, "Các cột hiển thị: Mã PA, Tên Phương Án, SKU, Giá Vốn, Giá Bán, Tồn Kho Hiện Tại và Tỷ Suất Biên Lợi Nhuận (%). Người dùng kiểm tra tính chính xác của các tham số trước khi thực hiện so sánh.", "2. Thông số kinh doanh: ");

    add_body_p(doc, "• Hướng dẫn nhập Ma trận so sánh cặp Saaty 5x5 (Cột phải):")
    add_body_p(doc, "Ma trận đối xứng so sánh mức độ quan trọng giữa 5 tiêu chí: C1 (Biên lợi nhuận), C2 (Doanh thu kỳ vọng), C3 (Nhu cầu thị trường), C4 (Rủi ro tồn kho), C5 (Chi phí Marketing). Thang đo 1-9 của Saaty:")
    add_bullet_p(doc, "1: Hai tiêu chí quan trọng ngang nhau; 3: Tiêu chí dòng quan trọng hơn nhẹ so với tiêu chí cột; 5: Quan trọng hơn nhiều; 7: Rất quan trọng; 9: Cực kỳ quan trọng (2, 4, 6, 8 là các mức trung gian).", "• Thang đo Saaty 1-9: ");
    add_bullet_p(doc, "Người dùng chỉ cần nhập các giá trị ở nửa tam giác trên đường chéo chính. Hệ thống tự động tính phần tử nghịch đảo đối xứng qua đường chéo: a_ji = 1 / a_ij (Ví dụ: nếu C1/C2 = 3 thì C2/C1 tự động = 0.3333).", "• Nghịch đảo tự động: ");

    add_body_p(doc, "• Kiểm soát tỷ số nhất quán CR (Consistency Ratio):")
    add_bullet_p(doc, "Khi CR ≤ 0.10 (≤ 10%): Đèn trạng thái hiển thị màu XANH LÁ '🟢 Tỷ số CR = 0.0152 (1.52% ≤ 10%) - ĐẠT CHUẨN NHẤT QUÁN'. Nút chạy DSS ở trạng thái sẵn sàng.", "• Trạng thái Đạt chuẩn: ");
    add_bullet_p(doc, "Khi CR > 0.10 (> 10%): Đèn trạng thái chuyển sang màu ĐỎ RỰC '🔴 TỶ SỐ CR = 0.1450 > 10% - CẢNH BÁO MÂU THUẪN DỮ LIỆU'. Hệ thống sẽ tự động khóa hoặc chặn thực thi nút 'CHẠY MÔ HÌNH DSS NGAY', yêu cầu người dùng phải điều chỉnh lại các cặp so sánh để đảm bảo tính bắc cầu toán học.", "• Trạng thái Mâu thuẫn: ");

    add_body_p(doc, "• Kích hoạt mô hình DSS:")
    add_bullet_p(doc, "Sau khi ma trận đạt chuẩn CR ≤ 10%, người dùng nhấp nút '🚀 CHẠY MÔ HÌNH DSS NGAY' màu Indigo ở góc dưới bên phải. Hệ thống sẽ tự động thực thi thuật toán AHP và chuyển hướng ngay lập tức sang Tab 2 để hiển thị kết quả xếp hạng.", "• Thực thi: ");

    # -------------------------------------------------------------------------
    # 4.2. TAB 2
    # -------------------------------------------------------------------------
    add_heading_2(doc, "4.2. Tab 2: Xem Kết Quả Ra Quyết Định, Bảng Xếp Hạng & Khuyến Nghị (Outputs & Decision)")
    add_body_p(doc, "Tab 2 là trung tâm điều hành kết quả của toàn bộ hệ thống DSS, nơi chuyển hóa các phép toán ma trận phức tạp thành bảng xếp hạng và khuyến nghị kinh doanh trực quan:")
    
    add_image_figure(doc, "docs/wireframes/GUI_05_Main_Tab2_Outputs.png", "Hình 5: Tab 2 - Bảng Xếp Hạng Phương Án, 4 KPI Điều Hành & Biểu Đồ Cột AHP (GUI-05)")
    
    add_body_p(doc, "• Ý nghĩa 4 Thẻ Chỉ Số KPI Điều Hành (Phía trên cùng):")
    add_bullet_p(doc, "Thẻ 1 - LỰA CHỌN HÀNG ĐẦU: Hiển thị sản phẩm đạt Quán quân AHP (Ví dụ: 'Asus ROG Strix G16 • Điểm 0.2458 • PHÊ DUYỆT') trên nền xanh ngọc.", "• KPI 1 (Top Choice): ");
    add_bullet_p(doc, "Thẻ 2 - TỶ SỐ NHẤT QUÁN CR: Hiển thị giá trị kiểm định toán học Saaty (Ví dụ: '0.0152 (1.52%) • Đạt chuẩn Saaty ≤ 10%') trên nền xanh dương.", "• KPI 2 (Consistency): ");
    add_bullet_p(doc, "Thẻ 3 - DOANH THU KỲ VỌNG: Tổng doanh thu dự kiến thu về từ sản phẩm số 1 (Ví dụ: '850 Triệu VNĐ • Biên LN 30.14%') trên nền tím phong cách.", "• KPI 3 (Revenue): ");
    add_bullet_p(doc, "Thẻ 4 - TỔNG SỐ PHƯƠNG ÁN: Phân nhóm tổng thể danh mục (Ví dụ: '9 Phương Án • 1 Chọn • 3 Xem Xét • 5 Loại') trên nền vàng cam.", "• KPI 4 (Candidates): ");

    add_body_p(doc, "• Hướng dẫn đọc Bảng Xếp Hạng Phương Án (dgvRanking):")
    add_bullet_p(doc, "Cột 'Hạng': Vẽ huy hiệu tròn bằng đồ họa GDI+ với màu quy ước sang trọng: Hạng 1 (Màu Vàng Gold), Hạng 2 (Màu Bạc Silver), Hạng 3 (Màu Đồng Bronze), các hạng còn lại nền xám nhạt.", "• Huy hiệu Thứ Hạng: ");
    add_bullet_p(doc, "Cột 'Quyết Định': Vẽ dạng nhãn bo tròn (Pill Badge) với 3 trạng thái rõ ràng: 'Selected' (Xanh lá - Phê duyệt nhập hàng ngay); 'UnderReview' (Vàng cam - Xem xét thăm dò thị trường); 'Rejected' (Đỏ hồng - Loại bỏ, không giải ngân ngân sách).", "• Huy hiệu Phê Duyệt: ");
    add_bullet_p(doc, "Cột 'SL Đặt Hàng Gợi Ý': Tự động tính số lượng cần nhập kho theo công thức an toàn tồn kho chu kỳ 30 ngày: Suggested_Qty = max(0, Nhu_cau * (Lead_Time + 30) - Ton_kho).", "• Khuyến nghị Nhập Kho: ");
    add_bullet_p(doc, "Cột 'Ngân Sách Marketing': Tự động chia tỷ trọng ngân sách tiếp thị theo tỷ lệ điểm AHP của các sản phẩm thuộc nhóm Selected.", "• Phân bổ Tiếp Thị: ");

    add_body_p(doc, "• Biểu Đồ Cột Tỷ Trọng Điểm AHP & Hộp Khuyến Nghị Điều Hành:")
    add_bullet_p(doc, "Biểu đồ cột trực quan hiển thị tỷ trọng phân bổ điểm AHP của 9 sản phẩm, giúp nhà quản trị thấy rõ khoảng cách vượt trội của phương án Top 1 so với các đối thủ cạnh tranh.", "• Biểu đồ AHP: ");
    add_bullet_p(doc, "Hộp văn bản tự động sinh các đoạn phân tích chiến lược kinh doanh: Nêu rõ lý do sản phẩm Hạng 1 chiến thắng (nhờ biên lợi nhuận cao 30.14% và nhu cầu thị trường vượt trội), cảnh báo các SKU có nguy cơ đọng vốn cần xả hàng thu hồi dòng tiền.", "• Khuyến nghị điều hành: ");

    # -------------------------------------------------------------------------
    # 4.3. TAB 3
    # -------------------------------------------------------------------------
    add_heading_2(doc, "4.3. Tab 3: Mô Phỏng Biến Động Thị Trường & Độ Nhạy What-If (Sensitivity Analysis)")
    add_body_p(doc, "Tab 3 cung cấp năng lực mô phỏng động, cho phép nhà quản trị trả lời câu hỏi: 'Điều gì sẽ xảy ra nếu thị trường biến động ngoài dự kiến?':")
    
    add_image_figure(doc, "docs/wireframes/GUI_06_Main_Tab3_WhatIf.png", "Hình 6: Tab 3 - Mô Phỏng Biến Động Thị Trường What-If & Biểu Đồ Đường Độ Nhạy Delta (GUI-06)")
    
    add_body_p(doc, "• Sử dụng 3 Kịch Bản Thị Trường Vĩ Mô Định Sẵn (1-Click):")
    add_bullet_p(doc, "Nhấn '🔥 Kịch Bản 1: Bùng Nổ Gaming' ➔ Hệ thống mô phỏng thị trường game tăng trưởng mạnh (+40% nhu cầu). Kết quả: Asus ROG Strix G16 và Acer Nitro 16 bứt phá điểm số ngoạn mục.", "• Kịch bản 1 (Gaming Boom): ");
    add_bullet_p(doc, "Nhấn '⚡ Kịch Bản 2: Đứt Gãy Cung Ứng' ➔ Mô phỏng khủng hoảng logistics toàn cầu, tăng gấp đôi rủi ro tồn kho (C4) và lead time. Hệ thống lập tức hạ điểm các dòng máy nhập khẩu cồng kềnh, ưu tiên hàng nội địa sẵn có.", "• Kịch bản 2 (Supply Crisis): ");
    add_bullet_p(doc, "Nhấn '📉 Kịch Bản 3: Lạm Phát Thắt Chặt' ➔ Mô phỏng suy thoái sức mua tiêu dùng, hệ thống hạ điểm phân khúc cao cấp (iPhone 16 Pro Max, Dell Alienware) và đẩy dòng máy giá rẻ, biên LN bền vững lên ngôi.", "• Kịch bản 3 (Inflation): ");
    add_bullet_p(doc, "Nhấn '🔄 Khôi Phục Dữ Liệu Gốc' ➔ Xóa bỏ toàn bộ các biến động giả định, đưa bảng điểm về trạng thái dữ liệu gốc ban đầu.", "• Nút Reset: ");

    add_body_p(doc, "• Tinh Chỉnh Tham Số Vi Mô Theo Từng Sản Phẩm:")
    add_bullet_p(doc, "Chọn 1 sản phẩm mục tiêu trong danh sách dropdown 'Chọn sản phẩm mô phỏng'.", "Bước 1: ");
    add_bullet_p(doc, "Điều chỉnh 3 thanh tham số: Nhu cầu (C3: 0.05 - 1.00), Biên lợi nhuận (C1: 0.05 - 1.00), An toàn vốn (C4: 0.05 - 1.00).", "Bước 2: ");
    add_bullet_p(doc, "Nhấn nút '⚡ Áp Dụng Tham Số': Hệ thống sẽ tái chuẩn hóa ma trận in-memory và tính lại điểm AHP trong chưa đầy 50 mili-giây mà không làm ảnh hưởng đến dữ liệu gốc trong SQL Server.", "Bước 3: ");

    add_body_p(doc, "• Trực Quan Hóa Đồ Họa So Sánh:")
    add_bullet_p(doc, "Nút '📈 Biểu Đồ Đường Độ Nhạy': Vẽ biểu đồ so sánh đường (Delta Line Chart) với 2 đường: Đường nét liền màu Indigo (Dữ liệu gốc) và Đường nét đứt màu Vàng Cam (Sau khi mô phỏng), thể hiện trực quan độ lệch Delta của từng sản phẩm.", "• Delta Chart: ");
    add_bullet_p(doc, "Nút '🌳 Sơ Đồ Cây Phân Cấp AHP': Vẽ sơ đồ cây phân cấp 3 tầng Saaty có mũi tên định hướng từ Mục tiêu tối thượng ➔ 5 Tiêu chí ➔ 9 Sản phẩm ứng viên.", "• Cây phân cấp: ");

    # -------------------------------------------------------------------------
    # 4.4. TAB 4
    # -------------------------------------------------------------------------
    add_heading_2(doc, "4.4. Tab 4: Thêm Sản Phẩm Mới & Thẩm Định Động (Dynamic SKU Evaluation)")
    add_body_p(doc, "Khi doanh nghiệp đàm phán nhập một mẫu máy mới từ nhà sản xuất, Tab 4 cho phép thẩm định nhanh tiềm năng của sản phẩm trước khi ký hợp đồng nhập khẩu:")
    
    add_image_figure(doc, "docs/wireframes/GUI_07_Main_Tab4_AddProduct.png", "Hình 7: Tab 4 - Nhập Sản Phẩm Mới, Tự Tính Biên LN & Thẩm Định Động (GUI-07)")
    
    add_body_p(doc, "• Các bước nhập liệu sản phẩm mới:")
    add_bullet_p(doc, "Tại Thẻ 'THÔNG TIN BÁN HÀNG (OLTP)', nhập Tên sản phẩm, Mã SKU (quy chuẩn PRD_xxx), chọn Ngành hàng (Smartphone, Laptop, Phụ kiện, Smart Home), Giá vốn (VNĐ), Giá bán niêm yết (VNĐ) và Tồn kho ban đầu.", "Bước 1: ");
    add_bullet_p(doc, "Quan sát nhãn 'Biên Lợi Nhuận Tính Toán': Hệ thống tự động tính tỷ suất lợi nhuận gộp theo công thức: ((Giá Bán - Giá Vốn) / Giá Bán) * 100% và hiển thị màu xanh lá (Tối ưu) hoặc vàng cam. Trường C1 ở Thẻ bên cạnh được khóa cứng (ReadOnly) tự động điền giá trị này, ngăn ngừa hoàn toàn lỗi nhập sai lệch.", "Bước 2: ");
    add_bullet_p(doc, "Tại Thẻ 'ĐIỂM ĐÁNH GIÁ 5 TIÊU CHÍ (DSS)', nhập 4 chỉ số còn lại: C2 (Doanh thu kỳ vọng - Triệu VNĐ), C3 (Nhu cầu thị trường: 1 - 100 điểm), C4 (Số ngày vòng quay tồn kho: 1 - 365 ngày), C5 (Chi phí Marketing dự kiến: 0 - 1000 Triệu VNĐ).", "Bước 3: ");
    add_bullet_p(doc, "Nhấn nút '💾 LƯU SẢN PHẨM & CẬP NHẬT': Hệ thống sẽ thực thi Transaction ghi đồng thời vào bảng PRODUCTS và ALTERNATIVES trong CSDL SQL Server, đồng thời kích hoạt thuật toán AHP tái tính toán xếp hạng cho toàn bộ danh mục mới mà không cần phải khởi động lại phần mềm.", "Bước 4: ");

    # -------------------------------------------------------------------------
    # 4.5. TAB 5
    # -------------------------------------------------------------------------
    add_heading_2(doc, "4.5. Tab 5: Khai Phá & Phân Tích Bán Hàng Đa Chiều OLAP (Sales & OLAP Engine)")
    add_body_p(doc, "Tab 5 cung cấp góc nhìn đa chiều về dữ liệu lịch sử bán lẻ, đóng vai trò căn cứ số liệu khách quan để thiết lập trọng số tiêu chí ở Tab 1:")
    
    add_image_figure(doc, "docs/wireframes/GUI_08_Main_Tab5_OLAP.png", "Hình 8: Tab 5 - Dữ Liệu Bán Hàng Lịch Sử & Ba Chế Độ Phân Tích OLAP (GUI-08)")
    
    add_body_p(doc, "• Ba Chế Độ Truy Vấn OLAP Chuyên Sâu:")
    add_bullet_p(doc, "Chế độ 1 - Bấm nút '📊 1. Hiệu Suất Sản Phẩm': Hệ thống truy vấn View 'vw_DSS_ProductPerformance' hiển thị doanh số bán lẻ thực tế, doanh thu thực thu, lợi nhuận gộp thực tế và tốc độ xuất kho hàng ngày của từng SKU.", "• Chế độ 1 (Performance View): ");
    add_bullet_p(doc, "Chế độ 2 - Bấm nút '📦 2. OLAP ROLLUP: Tồn Kho': Thực thi câu lệnh SQL 'GROUP BY ROLLUP(c.category_name, p.product_name)'. Cho phép người dùng xem dữ liệu phân cấp từ mức chi tiết từng SKU, cuộn lên tổng mức Ngành hàng và tổng mức Toàn bộ chuỗi bán lẻ.", "• Chế độ 2 (OLAP Rollup): ");
    add_bullet_p(doc, "Chế độ 3 - Bấm nút '🎯 3. OLAP CUBE: Khuyến Mãi': Thực thi câu lệnh SQL 'GROUP BY CUBE(cust.customer_segment, cat.category_name)'. Xây dựng khối lập phương đa chiều phân tích doanh thu chéo giữa từng Phân khúc khách hàng (VIP, Thân thiết, Mới) và từng Ngành hàng kinh doanh.", "• Chế độ 3 (OLAP Cube): ");
    add_bullet_p(doc, "Khung 'Chú giải câu lệnh SQL Server' phía dưới bảng lưới sẽ tự động hiển thị cấu trúc lệnh T-SQL đang thực thi kèm phân tích ý nghĩa nghiệp vụ.", "• Chú giải SQL: ");

    # -------------------------------------------------------------------------
    # 4.6. TAB 6
    # -------------------------------------------------------------------------
    add_heading_2(doc, "4.6. Tab 6: Biểu Mẫu Báo Cáo Thẩm Định & Phê Duyệt Kế Hoạch (Reporting Tab)")
    add_body_p(doc, "Tab 6 chuẩn hóa toàn bộ kết quả phân tích thành hồ sơ báo cáo hành chính doanh nghiệp phục vụ cuộc họp Ban Giám Đốc:")
    
    add_image_figure(doc, "docs/wireframes/GUI_09_Main_Tab6_Report.png", "Hình 9: Tab 6 - Biểu Mẫu Báo Cáo Thẩm Định BM-06, Xuất File Word & HTML/PDF (GUI-09)")
    
    add_body_p(doc, "• Cấu trúc Hồ sơ thẩm định BM-06/DSS-2026/Q4:")
    add_bullet_p(doc, "Tiêu đề văn bản chuẩn doanh nghiệp, ngày thẩm định, tên cán bộ phân tích và mã phiên làm việc (Session ID).", "1. Tiêu đề hành chính: ");
    add_bullet_p(doc, "Bảng kế hoạch nhập hàng chi tiết gồm: Thứ hạng, Mã SKU, Tên thiết bị, Điểm tổng hợp AHP, Số lượng nhập đề xuất, Tổng vốn giải ngân dự kiến và Tỷ lệ phân bổ ngân sách MKT (%).", "2. Bảng kế hoạch chi tiết: ");
    add_bullet_p(doc, "Ba chỉ thị điều hành tự động: 1. Quyết định rót vốn nhóm Selected; 2. Đặt hàng thăm dò nhóm UnderReview; 3. Dừng giải ngân và xả kho thu hồi vốn đối với nhóm Rejected.", "3. Ba chỉ thị điều hành: ");

    add_body_p(doc, "• Hướng dẫn xuất hồ sơ báo cáo:")
    add_bullet_p(doc, "Xuất file Word: Nhấn nút '📝 Xuất File Word (.doc)' màu xanh dương. Hệ thống xuất toàn bộ dữ liệu bảng và nhận xét ra file Word tại thư mục báo cáo, kèm thông báo đường dẫn lưu file.", "• Xuất Word: ");
    add_bullet_p(doc, "In ấn / Lưu PDF: Nhấn nút '🖨️ In Báo Cáo / Lưu PDF' màu xanh Emerald. Hệ thống tự động tạo trang HTML có định dạng in ấn chuyên nghiệp và mở trực tiếp trên trình duyệt web mặc định (Chrome/Edge), sẵn sàng bấm Ctrl+P để in ra máy in hoặc lưu thành file PDF.", "• In ấn / PDF: ");
    add_bullet_p(doc, "Cập nhật dữ liệu: Nhấn nút '🔄 Cập Nhật Dữ Liệu' để đồng bộ lại phiếu báo cáo nếu vừa có sự thay đổi dữ liệu ở các Tab khác.", "• Refresh: ");

    doc.add_page_break()

    # =========================================================================
    # PHẦN 5: CƠ CHẾ AN TOÀN FAIL-SAFE & XỬ LÝ SỰ CỐ (TROUBLESHOOTING)
    # =========================================================================
    add_heading_1(doc, "PHẦN 5: CƠ CHẾ AN TOÀN FAIL-SAFE & XỬ LÝ SỰ CỐ (TROUBLESHOOTING)")

    add_heading_2(doc, "5.1. Cơ Chế Khóa An Toàn DatabaseGuard Security Overlay")
    add_body_p(doc, "DatabaseGuard là cơ chế bảo vệ cấp doanh nghiệp độc đáo của phần mềm DSS Retail Decision Studio 2026:")
    
    add_image_figure(doc, "docs/wireframes/GUI_10_DatabaseGuard_Overlay.png", "Hình 10: Màn hình Khóa An Toàn Khi Mất Kết Nối CSDL (DatabaseGuard Security Overlay - GUI-10)")
    
    add_body_p(doc, "• Nguyên lý vận hành Fail-Safe:")
    add_bullet_p(doc, "Hệ thống tích hợp bộ định thời dbHeartbeat Timer chạy ngầm quét nhịp tim kết nối SQL Server định kỳ mỗi 4000ms (4 giây).", "1. Quét nhịp tim: ");
    add_bullet_p(doc, "Nếu dịch vụ SQL Server bị dừng, đứt cáp mạng LAN hoặc rút dây mạng, màn hình pnlDbOverlay màu tối sẽ lập tức che phủ toàn bộ 6 Tab chức năng và khóa cứng mọi thao tác.", "2. Kích hoạt lớp phủ: ");
    add_bullet_p(doc, "Mục đích: Ngăn chặn triệt để tình trạng người dùng tiếp tục bấm nút ra quyết định hoặc sửa số liệu trên một tập dữ liệu đứt gãy, loại bỏ hoàn toàn nguy cơ sập ứng dụng (Crash-Free Architecture).", "3. Mục tiêu bảo vệ: ");
    add_bullet_p(doc, "Khi người dùng khắc phục xong mạng hoặc khởi động lại SQL Server, họ có thể nhấn nút '↻ Thử Kết Nối Lại'. Nếu kết nối thông suốt, lớp overlay tự động biến mất và toàn bộ dữ liệu được nạp lại nguyên vẹn.", "4. Phục hồi tự động: ");

    add_heading_2(doc, "5.2. Các Lỗi Thường Gặp Và Hướng Dẫn Khắc Phục Nhanh")
    
    trouble_data = [
        ("Lỗi 1: 'Cannot connect to SQL Server' / Màn hình khóa Overlay kích hoạt", "Dịch vụ SQL Server chưa bật hoặc sai tên instance máy chủ.", "Mở Windows Services (services.msc), tìm dịch vụ 'SQL Server (SQLEXPRESS25)' và nhấn 'Start'. Kiểm tra tên server trong connection string."),
        ("Lỗi 2: Nhãn CR báo đỏ 'Tỷ số CR > 10% - Khóa nút Chạy DSS'", "Ma trận so sánh cặp Saaty vi phạm tính bắc cầu logic toán học (ví dụ: A > B, B > C nhưng lại cho C > A quá mức).", "Kiểm tra lại các ô nhập ma trận ở Tab 1. Hiệu chỉnh giảm bớt các cặp có mức độ chênh lệch quá cực đoan để tỷ số CR giảm về ≤ 0.10 (≤ 10%)."),
        ("Lỗi 3: Không thể lưu sản phẩm mới ở Tab 4", "Người dùng nhập Giá bán nhỏ hơn Giá vốn, hoặc để trống các trường bắt buộc (*), hoặc trùng mã SKU.", "Kiểm tra lại nguyên tắc: Giá Bán Niêm Yết phải ≥ Giá Vốn. Điền đầy đủ tên sản phẩm và đảm bảo mã SKU chưa từng tồn tại trong CSDL."),
        ("Lỗi 4: Không thể xuất file Word báo cáo ở Tab 6", "File báo cáo cũ đang được mở trong ứng dụng Microsoft Word nên bị khóa quyền ghi (File Lock).", "Đóng cửa sổ Microsoft Word đang mở file báo cáo cũ và nhấp lại nút 'Xuất File Word (.doc)'.")
    ]
    create_styled_table(doc, ["Hiện Tượng / Thông Báo Lỗi", "Nguyên Nhân Kỹ Thuật", "Quy Trình Xử Lý Khắc Phục (Troubleshooting)"], trouble_data, [Inches(1.8), Inches(2.2), Inches(2.5)])

    add_heading_2(doc, "5.3. Bảng Câu Hỏi Thường Gặp (FAQ)")
    faq_data = [
        ("Tôi có thể đổi ma trận Saaty thành 6 hoặc 7 tiêu chí không?", "Phiên bản hiện tại được tối ưu hóa cho 5 tiêu chí bán lẻ chuẩn của học phần IS 402 A (C1-C5). Việc mở rộng số tiêu chí có thể thực hiện thông qua cập nhật bảng CRITERIA trong CSDL SQL Server."),
        ("Tại sao ô nhập Biên Lợi Nhuận C1 ở Tab 4 không cho phép gõ phím?", "Đây là cơ chế phòng ngừa lỗi chủ động (Error Prevention). Biên lợi nhuận được phần mềm tự động tính toán từ Giá vốn và Giá bán để đảm bảo tính khách quan tuyệt đối, tránh gian lận số liệu."),
        ("Điểm AHP của các sản phẩm có tổng bằng 1.0 không?", "Chính xác. Điểm AHP tổng hợp của toàn bộ 9 phương án ứng viên luôn tuân thủ nguyên tắc đại số tuyến tính: Tổng điểm của 9 sản phẩm bằng đúng 1.0000 (100%).")
    ]
    create_styled_table(doc, ["Câu Hỏi Thường Gặp (FAQ)", "Giải Đáp Chi Tiết Từ Nhóm Phát Triển DSS"], faq_data, [Inches(2.5), Inches(4.0)])

    doc.add_page_break()

    # =========================================================================
    # PHẦN 6: KẾT LUẬN & LIÊN HỆ HỖ TRỢ KỸ THUẬT
    # =========================================================================
    add_heading_1(doc, "PHẦN 6: KẾT LUẬN & LIÊN HỆ HỖ TRỢ KỸ THUẬT")

    add_body_p(doc, "Tài liệu Hướng dẫn Sử dụng Phần mềm DSS Retail Decision Studio 2026 đã cung cấp bức tranh hoàn chỉnh, chuẩn mực và chi tiết nhất về cách vận hành hệ thống hỗ trợ ra quyết định. Toàn bộ giải pháp được xây dựng với mục tiêu cao nhất: Đơn giản hóa các mô hình toán học phức tạp thành công cụ trực quan, giúp nhà quản trị kinh doanh đưa ra những quyết định đúng đắn, kịp thời và mang lại hiệu quả tài chính tối ưu.")
    
    add_heading_2(doc, "Thông Tin Đội Ngũ Phát Triển & Bản Quyền")
    add_bullet_p(doc, "Nhóm 05 – Lớp IS 402 A – Khoa Hệ thống Thông tin Quản lý (MIS).", "• Đơn vị phát triển: ");
    add_bullet_p(doc, "Trường Đào tạo Quốc tế (International School) – Đại học Duy Tân, Đà Nẵng, Việt Nam.", "• Cơ sở đào tạo: ");
    add_bullet_p(doc, "Trần Hồng Minh (Trưởng nhóm - MSSV: 29210247142), Lê Thị Nga (MSSV: 29206361029), Phạm Ngọc Tú (MSSV: 29204347819), Lê Thanh Diệu (MSSV: 29204622434), Hồ Thị Thuận (MSSV: 29201164457).", "• Danh sách thành viên: ");
    add_bullet_p(doc, "ThS. Nguyễn Quang Ánh.", "• Giảng viên hướng dẫn: ");
    add_bullet_p(doc, "Bản quyền phần mềm thuộc về Nhóm 05 – Học phần IS 402 A (Năm học 2026). Nghiêm cấm sao chép hoặc phân phối lại khi chưa có sự đồng ý của nhóm tác giả.", "• Bản quyền: ");

    # Save Word Document
    out_path = 'docs/TAI_LIEU_HUONG_DAN_SU_DUNG_DSS_IS402_NHOM05.docx'
    doc.save(out_path)
    print(f"SUCCESS: Document saved at {out_path}")

if __name__ == '__main__':
    build_user_manual_docx()
