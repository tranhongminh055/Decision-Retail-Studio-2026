# -*- coding: utf-8 -*-
"""
Script tao Tai lieu SRS - Dac ta Yeu cau Phan mem (Software Requirements Specification)
Chuan IEEE 830 / ISO/IEC/IEEE 29148
Dinh dang: Microsoft Word (.docx) va Markdown (.md)
He thong Ho tro Ra quyet dinh DSS Retail Decision Studio 2026
IS 402 A - Nhom 05 - Duy Tan University
"""

import os
import docx
from docx import Document
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT, WD_ALIGN_VERTICAL

from docx_helpers import set_cell_background, set_cell_margins, set_table_borders, add_callout
from docx_builder_base import (
    create_document, add_title, add_heading_1, add_heading_2, add_heading_3,
    add_body_p, add_bullet_p, add_image_figure, create_styled_table
)

def build_srs_document():
    print("Khoi tao Document SRS - Dac ta Yeu cau Phan mem...")
    doc = create_document()
    
    # Custom Header / Footer
    header = doc.sections[0].header
    hp = header.paragraphs[0]
    hp.text = ""
    hp.alignment = WD_ALIGN_PARAGRAPH.LEFT
    hr = hp.add_run("TÀI LIỆU ĐẶC TẢ YÊU CẦU PHẦN MỀM (SOFTWARE REQUIREMENTS SPECIFICATION - SRS) • CHUẨN IEEE 830")
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
    
    # Line divider
    p_div = doc.add_paragraph()
    p_div.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_div.paragraph_format.space_after = Pt(30)
    r_div = p_div.add_run("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━")
    r_div.font.name = "Segoe UI"
    r_div.font.color.rgb = RGBColor(203, 213, 225)
    
    p_title = doc.add_paragraph()
    p_title.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_title.paragraph_format.space_before = Pt(15)
    p_title.paragraph_format.space_after = Pt(8)
    r_t = p_title.add_run("ĐẶC TẢ YÊU CẦU PHẦN MỀM\n(SOFTWARE REQUIREMENTS SPECIFICATION - SRS)\n")
    r_t.font.name = "Segoe UI"
    r_t.font.size = Pt(19)
    r_t.font.bold = True
    r_t.font.color.rgb = RGBColor(15, 23, 42)
    
    p_sub = doc.add_paragraph()
    p_sub.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_sub.paragraph_format.space_after = Pt(25)
    r_s = p_sub.add_run("ĐỀ TÀI: HỆ THỐNG HỖ TRỢ RA QUYẾT ĐỊNH BÁN HÀNG DSS RETAIL DECISION STUDIO 2026\n")
    r_s.font.name = "Segoe UI"
    r_s.font.size = Pt(11.5)
    r_s.font.bold = True
    r_s.font.color.rgb = RGBColor(37, 99, 235) # Royal Blue
    
    r_s2 = p_sub.add_run("Ứng Dụng Mô Hình Phân Tích Thứ Bậc AHP Saaty & Cơ Sở Dữ Liệu SQL Server\nTiêu Chuẩn Đặc Tả: IEEE Std 830-1998 / ISO/IEC/IEEE 29148")
    r_s2.font.name = "Segoe UI"
    r_s2.font.size = Pt(10)
    r_s2.font.italic = True
    r_s2.font.color.rgb = RGBColor(100, 116, 139)

    # Info Box
    tbl_info = doc.add_table(rows=7, cols=2)
    tbl_info.alignment = WD_TABLE_ALIGNMENT.CENTER
    set_table_borders(tbl_info, color="CBD5E1", sz="4")
    
    info_data = [
        ("Mã học phần:", "IS 402 A – Hệ Thống Thông Tin Hỗ Ra Quyết Định (DSS)"),
        ("Giảng viên hướng dẫn:", "ThS. Nguyễn Quang Ánh"),
        ("Nhóm sinh viên thực hiện:", "Nhóm 05 – Lớp IS 402 A"),
        ("Danh sách thành viên:", "1. Trần Hồng Minh (Trưởng nhóm - MSSV: 29210247142)\n2. Hồ Thị Thuận (MSSV: 29201164457)\n3. Lê Thị Nga (MSSV: 29206361029)\n4. Phạm Ngọc Tú (MSSV: 29204347819)\n5. Lê Thanh Diệu (MSSV: 29204622434)"),
        ("Ứng dụng kiểm thử:", "DSS Retail Decision Studio 2026 (C# .NET 10 WinForms & SQL Server 2022)"),
        ("Phiên bản tài liệu:", "Phiên bản 1.0 (Chính thức nghiệm thu)"),
        ("Ngày phát hành:", "29/10/2026")
    ]
    
    for idx, (label, val) in enumerate(info_data):
        row = tbl_info.rows[idx]
        cell_lbl, cell_val = row.cells[0], row.cells[1]
        
        cell_lbl.text = ""
        p_l = cell_lbl.paragraphs[0]
        p_l.paragraph_format.space_before = Pt(3)
        p_l.paragraph_format.space_after = Pt(3)
        r_l = p_l.add_run(label)
        r_l.font.name = "Segoe UI"
        r_l.font.size = Pt(9)
        r_l.font.bold = True
        r_l.font.color.rgb = RGBColor(15, 23, 42)
        set_cell_background(cell_lbl, "F1F5F9")
        set_cell_margins(cell_lbl, top=70, bottom=70, left=100, right=100)
        cell_lbl.width = Inches(2.2)
        
        cell_val.text = ""
        p_v = cell_val.paragraphs[0]
        p_v.paragraph_format.space_before = Pt(3)
        p_v.paragraph_format.space_after = Pt(3)
        r_v = p_v.add_run(val)
        r_v.font.name = "Segoe UI"
        r_v.font.size = Pt(9)
        r_v.font.color.rgb = RGBColor(51, 65, 85)
        set_cell_background(cell_val, "FFFFFF")
        set_cell_margins(cell_val, top=70, bottom=70, left=100, right=100)
        cell_val.width = Inches(4.3)
        
    doc.add_page_break()

    # =========================================================================
    # LỊCH SỬ THAY ĐỔI
    # =========================================================================
    add_heading_1(doc, "BẢNG LỊCH SỬ THAY ĐỔI TÀI LIỆU (DOCUMENT REVISION HISTORY)")
    rev_headers = ["Phiên bản", "Ngày cập nhật", "Tác giả", "Mô tả nội dung thay đổi chính", "Phê duyệt"]
    rev_data = [
        ["v0.1", "12/10/2026", "Trần Hồng Minh", "Khởi tạo phác thảo yêu cầu nghiệp vụ và cấu trúc SRS", "Nhóm 05"],
        ["v0.5", "18/10/2026", "Lê Thị Nga, Phạm Ngọc Tú", "Chi tiết hóa 8 nhóm yêu cầu chức năng (FR) và phi chức năng (NFR)", "Trần Hồng Minh"],
        ["v0.9", "25/10/2026", "Hồ Thị Thuận, Lê Thanh Diệu", "Hoàn thiện ma trận truy vết yêu cầu (RTM) và chuẩn hóa IEEE 830", "Trần Hồng Minh"],
        ["v1.0", "29/10/2026", "Toàn thể Nhóm 05", "Đồng bộ mã nguồn thực tế C# .NET 10, CSDL SQL Server và xuất bản", "ThS. Nguyễn Quang Ánh"]
    ]
    create_styled_table(doc, rev_headers, rev_data, col_widths=[0.8, 1.0, 1.3, 2.5, 0.9])
    
    add_callout(
        doc,
        "Tài liệu Đặc tả Yêu cầu Phần mềm (SRS) này được biên soạn tuân thủ tiêu chuẩn quốc tế IEEE Std 830-1998 "
        "và ISO/IEC/IEEE 29148:2018. Tài liệu là văn bản pháp lý kỹ thuật ràng buộc giữa đội ngũ phát triển (Nhóm 05) "
        "và Giảng viên hướng dẫn/Hội đồng chấm đồ án, mô tả toàn bộ chức năng, hiệu năng và các ràng buộc của hệ thống DSS.",
        title="CAM KẾT CHUẨN HÓA KỸ THUẬT (ENGINEERING STANDARD):",
        border_color="2563EB",
        bg_color="EFF6FF"
    )

    # =========================================================================
    # CHƯƠNG 1: GIỚI THIỆU (INTRODUCTION)
    # =========================================================================
    add_heading_1(doc, "CHƯƠNG 1: GIỚI THIỆU (INTRODUCTION)")
    
    add_heading_2(doc, "1.1. Mục đích tài liệu (Purpose)")
    add_body_p(
        doc,
        "Mục đích của tài liệu Đặc tả Yêu cầu Phần mềm (Software Requirements Specification - SRS) này là định nghĩa tường minh, "
        "chi tiết và lượng hóa toàn bộ các yêu cầu chức năng (Functional Requirements - FR), yêu cầu phi chức năng (Non-Functional Requirements - NFR), "
        "các ràng buộc thiết kế và giao diện của hệ thống 'DSS Retail Decision Studio 2026'. "
        "Tài liệu đóng vai trò là kim chỉ nam cho giai đoạn kiểm thử, đánh giá mức độ hoàn thiện và nghiệm thu đồ án môn học IS 402 A."
    )

    add_heading_2(doc, "1.2. Phạm vi sản phẩm (Product Scope)")
    add_body_p(
        doc,
        "Sản phẩm là ứng dụng Desktop Enterprise Client được xây dựng trên nền tảng C# .NET 10.0 Windows Forms, "
        "tích hợp lõi thuật toán ra quyết định DssEngine và hệ quản trị cơ sở dữ liệu Microsoft SQL Server 2022 Express. "
        "Hệ thống giải quyết bài toán lựa chọn danh mục sản phẩm công nghệ (Smartphones, Laptops, Phụ kiện) chiến lược "
        "cần ưu tiên nhập hàng và phân bổ ngân sách tiếp thị trong Quý 4/2026 dựa trên phương pháp Phân tích Thứ bậc AHP (Thomas L. Saaty)."
    )
    add_bullet_p(doc, "Hỗ trợ xác thực bảo mật SHA-256, phân quyền 4 vai trò, cơ chế Fail-Safe giám sát ngắt kết nối CSDL DatabaseGuard.", "Phạm vi chức năng cốt lõi: ")
    add_bullet_p(doc, "Quản lý 11 bảng thực thể CSDL 3NF, xử lý ETL làm sạch dữ liệu giao dịch bán lẻ.", "Xử lý dữ liệu: ")
    add_bullet_p(doc, "Tính toán ma trận Saaty 1-9 cho 5 tiêu chí (C1-C5), kiểm định tỷ số nhất quán CR <= 0.10, chuẩn hóa Benefit/Cost, xếp hạng AHP và tự động tính số lượng đặt hàng gợi ý.", "Bộ não toán học AHP: ")
    add_bullet_p(doc, "Mô phỏng What-If với 3 kịch bản thị trường vi mô/vĩ mô; phân tích bán lẻ đa chiều OLAP 4 thao tác; Dashboard KPI và xuất báo cáo thẩm định Word (.docx)/HTML.", "Mô phỏng & Báo cáo: ")

    add_heading_2(doc, "1.3. Định nghĩa, thuật ngữ và từ viết tắt (Definitions & Acronyms)")
    term_headers = ["Thuật ngữ / Từ viết tắt", "Tên tiếng Anh đầy đủ", "Định nghĩa & Ý nghĩa nghiệp vụ trong hệ thống"]
    term_data = [
        ["DSS", "Decision Support System", "Hệ thống thông tin hỗ trợ ra quyết định quản trị chiến lược."],
        ["AHP", "Analytic Hierarchy Process", "Mô hình phân tích thứ bậc của Thomas L. Saaty (1980) giải quyết bài toán đa tiêu chí."],
        ["CR", "Consistency Ratio", "Tỷ số nhất quán của ma trận so sánh cặp Saaty; hệ thống bắt buộc CR <= 0.10 (10%)."],
        ["CI", "Consistency Index", "Chỉ số nhất quán: CI = (lambda_max - n) / (n - 1)."],
        ["RI", "Random Index", "Chỉ số ngẫu nhiên thực nghiệm theo cấp ma trận n (với n = 5 tiêu chí thì RI = 1.12)."],
        ["OLAP", "Online Analytical Processing", "Kỹ thuật phân tích dữ liệu bán lẻ đa chiều (Slicing, Dicing, Roll-up, Drill-down)."],
        ["3NF", "Third Normal Form", "Dạng chuẩn 3 của CSDL quan hệ, triệt tiêu hoàn toàn dư thừa và dị thường dữ liệu."],
        ["SHA-256", "Secure Hash Algorithm 256-bit", "Thuật toán mã hóa băm mật khẩu một chiều an toàn lưu trữ trong bảng USERS."],
        ["RBAC", "Role-Based Access Control", "Mô hình kiểm soát truy cập và phân quyền theo 4 vai trò người dùng (UserSession)."],
        ["Lead Time", "Supplier Lead Time", "Thời gian từ lúc nhà quản lý đặt hàng đến khi hàng cập kho (tính theo ngày)."],
        ["What-If", "What-If Sensitivity Analysis", "Phân tích độ nhạy mô phỏng sự biến thiên của điểm số AHP khi tham số thị trường thay đổi."]
    ]
    create_styled_table(doc, term_headers, term_data, col_widths=[1.2, 1.8, 3.5])

    add_heading_2(doc, "1.4. Tài liệu tham khảo (References)")
    add_bullet_p(doc, "Thomas L. Saaty (1980), 'The Analytic Hierarchy Process: Planning, Priority Setting, Resource Allocation', McGraw-Hill, New York.", "1. Lý thuyết AHP: ")
    add_bullet_p(doc, "IEEE Computer Society (1998), 'IEEE Recommended Practice for Software Requirements Specifications', IEEE Std 830-1998.", "2. Chuẩn IEEE 830: ")
    add_bullet_p(doc, "ThS. Nguyễn Quang Ánh (2026), Đề cương & Bài giảng học phần IS 402 A - Hệ Thống Thông Tin Hỗ Trợ Ra Quyết Định, Đại học Duy Tân.", "3. Giáo trình IS 402 A: ")
    add_bullet_p(doc, "Nhóm 05 (2026), Tài liệu Phân tích Thiết kế CSDL DSS và Kế hoạch Kiểm thử Đồ án IS402, Lưu hành nội bộ.", "4. Tài liệu dự án: ")

    # =========================================================================
    # CHƯƠNG 2: MÔ TẢ TỔNG QUAN HỆ THỐNG (OVERALL DESCRIPTION)
    # =========================================================================
    add_heading_1(doc, "CHƯƠNG 2: MÔ TẢ TỔNG QUAN HỆ THỐNG (OVERALL DESCRIPTION)")
    
    add_heading_2(doc, "2.1. Bối cảnh sản phẩm & Bài toán kinh doanh Quý 4/2026")
    add_body_p(
        doc,
        "Thị trường bán lẻ thiết bị công nghệ Quý 4 luôn có tốc độ luân chuyển vốn nhanh nhưng chứa đựng rủi ro lỗi thời cực lớn. "
        "Hệ thống DSS Retail Decision Studio 2026 đóng vai trò là trung tâm cố vấn số, giúp doanh nghiệp đánh giá đồng thời "
        "5 tiêu chí nghiệp vụ có tính chất đối kháng nhau:"
    )
    add_bullet_p(doc, "Tỷ lệ phần trăm lãi gộp trên giá bán niêm yết (Hàm Lợi ích - Benefit).", "• C1: Biên lợi nhuận gộp (Profit Margin): ")
    add_bullet_p(doc, "Dự báo doanh số bán ra dựa trên dữ liệu giao dịch lịch sử (Hàm Lợi ích - Benefit).", "• C2: Doanh thu kỳ vọng (Expected Revenue): ")
    add_bullet_p(doc, "Điểm xu hướng thị trường và chỉ số quan tâm của khách hàng (Hàm Lợi ích - Benefit).", "• C3: Nhu cầu thị trường (Market Trend): ")
    add_bullet_p(doc, "Số ngày bán hết lượng hàng tồn kho an toàn - Days of Supply (Hàm Chi phí/Rủi ro - Cost).", "• C4: Rủi ro tồn kho (Inventory Risk): ")
    add_bullet_p(doc, "Chi phí quảng bá, chạy khuyến mãi và chiết khấu thương mại (Hàm Chi phí/Rủi ro - Cost).", "• C5: Chi phí tiếp thị (Marketing Cost): ")

    add_heading_2(doc, "2.2. Kiến trúc tổng thể 3 tầng (Enterprise 3-Tier Architecture)")
    add_body_p(
        doc,
        "Hệ thống được thiết kế theo mô hình 3 tầng chuẩn mực của ứng dụng doanh nghiệp nhằm đảm bảo tính độc lập, "
        "dễ bảo trì và tối ưu hiệu năng:"
    )
    add_bullet_p(doc, "Cung cấp giao diện trực quan WinForms với điều hướng 6 Tab chức năng khép kín; tiếp nhận thao tác người dùng, hiển thị đồ thị và bảng dữ liệu.", "1. Tầng Giao diện (Presentation Layer - C# .NET 10 WinForms UI): ")
    add_bullet_p(doc, "Chứa toàn bộ thuật toán tính toán ma trận Saaty, kiểm tra CR, chuẩn hóa Benefit/Cost, mô phỏng What-If và công thức tính số lượng đặt hàng.", "2. Tầng Logic Nghiệp vụ & Mô hình DSS (Business Logic Layer - DssEngine): ")
    add_bullet_p(doc, "Cơ sở dữ liệu Microsoft SQL Server 2022 Express instance .\\SQLEXPRESS25 quản lý 11 bảng thực thể đạt chuẩn 3NF.", "3. Tầng Truy cập Dữ liệu (Data Access Layer - SQL Server 2022 Express): ")

    add_heading_2(doc, "2.3. Phân loại người dùng & Đặc tính tác nhân (User Classes & Characteristics)")
    user_headers = ["Nhóm vai trò (Role)", "Quyền hạn trong hệ thống", "Trình độ & Yêu cầu kỹ năng tác nghiệp"]
    user_data = [
        ["Admin (Quản trị viên)", "Toàn quyền quản trị tài khoản USERS, sao lưu phục hồi CSDL DSS_SalesDB, giám sát kết nối DatabaseGuard.", "Thành thạo quản trị Windows Server và Microsoft SQL Server Management Studio (SSMS)."],
        ["Analyst (Chuyên viên)", "Nhập ma trận so sánh cặp Saaty 1-9, chạy mô hình AHP, điều chỉnh thanh trượt What-If, thực hiện phân tích đa chiều OLAP, thêm SKU mới Tab 4.", "Hiểu sâu phương pháp AHP của Saaty, có tư duy thống kê kinh doanh và phân tích dữ liệu bán lẻ."],
        ["Manager (Giám đốc)", "Xem Dashboard KPI điều hành, duyệt báo cáo tóm tắt Executive Summary, đối chiếu độ nhạy, ra quyết định phê duyệt danh mục nhập hàng Quý 4.", "Có thẩm quyền ký duyệt ngân sách thu mua, duyệt kế hoạch tài chính và chiến dịch tiếp thị."],
        ["Staff (Nhân viên)", "Xem tra cứu danh mục sản phẩm, theo dõi mức tồn kho và đơn giá bán lẻ.", "Kỹ năng tin học văn phòng cơ bản, thao tác tra cứu trên giao diện."]
    ]
    create_styled_table(doc, user_headers, user_data, col_widths=[1.5, 2.5, 2.5])

    add_heading_2(doc, "2.4. Môi trường vận hành (Operating Environment)")
    add_bullet_p(doc, "Hệ điều hành: Microsoft Windows 10 (bản 64-bit) hoặc Windows 11.", "Hệ điều hành Client: ")
    add_bullet_p(doc, "Nền tảng thực thi: Microsoft .NET Desktop Runtime 10.0 (x64).", "Framework: ")
    add_bullet_p(doc, "Cơ sở dữ liệu: Microsoft SQL Server 2022 Express Edition (hỗ trợ SQL Server Authentication và Windows Authentication).", "Hệ quản trị CSDL: ")
    add_bullet_p(doc, "Phần cứng tối thiểu: CPU Intel Core i3 / AMD Ryzen 3 trở lên, RAM 4GB, Ổ cứng trống tối thiểu 500MB.", "Cấu hình phần cứng: ")

    add_heading_2(doc, "2.5. Ràng buộc thiết kế & Thực thi (Design & Implementation Constraints)")
    add_bullet_p(doc, "Tỷ số nhất quán của ma trận Saaty bắt buộc phải thỏa mãn điều kiện CR <= 0.10. Nếu vượt ngưỡng, hệ thống lập tức khóa tính toán và yêu cầu hiệu chỉnh.", "Ràng buộc kiểm định Saaty: ")
    add_bullet_p(doc, "Bảo đảm phân định rõ ràng giữa tiêu chí Lợi ích (càng lớn càng tốt) và tiêu chí Chi phí/Rủi ro (càng nhỏ càng tốt) trong khâu chuẩn hóa.", "Ràng buộc chuẩn hóa toán học: ")
    add_bullet_p(doc, "Khi xảy ra sự cố mất kết nối mạng hoặc SQL Server dừng dịch vụ, hệ thống phải kích hoạt panel pnlDbOverlay trong vòng < 1 giây để khóa cứng toàn bộ 6 Tab.", "Ràng buộc phòng vệ Fail-Safe: ")
    add_bullet_p(doc, "Mọi mật khẩu người dùng phải được băm một chiều bằng thuật toán SHA-256 trước khi ghi vào CSDL.", "Ràng buộc bảo mật mật khẩu: ")

    # =========================================================================
    # CHƯƠNG 3: ĐẶC TẢ YÊU CẦU CHỨC NĂNG (FUNCTIONAL REQUIREMENTS - FR)
    # =========================================================================
    add_heading_1(doc, "CHƯƠNG 3: ĐẶC TẢ YÊU CẦU CHỨC NĂNG (SYSTEM FUNCTIONAL REQUIREMENTS)")
    
    add_body_p(
        doc,
        "Các yêu cầu chức năng của hệ thống DSS Retail Decision Studio 2026 được phân rã thành 8 nhóm tính năng "
        "nghiệp vụ chính (FR-01 đến FR-08), bám sát toàn diện chu trình hỗ trợ ra quyết định:"
    )

    # FR-01
    add_heading_2(doc, "3.1. FR-01: Quản trị Xác thực Bảo mật & Cơ chế Fail-Safe")
    add_bullet_p(doc, "Hệ thống phải cung cấp màn hình SplashForm tự động kiểm tra chuỗi kết nối SQL Server trước khi khởi chạy form chính.", "FR-01.1: Khởi động an toàn: ")
    add_bullet_p(doc, "Hệ thống phải cho phép người dùng đăng nhập bằng Username và Password; mật khẩu được băm SHA-256 đối chiếu với cột PasswordHash trong bảng USERS.", "FR-01.2: Xác thực SHA-256: ")
    add_bullet_p(doc, "Hệ thống phải duy trì phiên làm việc UserSession và phân quyền tính năng trên Form1 theo 4 vai trò (Admin, Analyst, Manager, Staff).", "FR-01.3: Quản lý phiên RBAC: ")
    add_bullet_p(doc, "Cung cấp RegisterForm cho phép người dùng đăng ký tài khoản mới với đầy đủ kiểm tra định dạng email và chống trùng tên đăng nhập.", "FR-01.4: Đăng ký tài khoản: ")
    add_bullet_p(doc, "Tích hợp DatabaseGuard chạy ngầm kiểm tra kết nối định kỳ; khi mất kết nối CSDL, lập tức che phủ và khóa cứng 6 Tab bằng panel pnlDbOverlay.", "FR-01.5: Phòng vệ Fail-Safe: ")

    # FR-02
    add_heading_2(doc, "3.2. FR-02: Tiếp nhận & Làm sạch Dữ liệu CSDL 3NF (Data Management & ETL)")
    add_bullet_p(doc, "Hệ thống phải ánh xạ và truy vấn trực tiếp 11 bảng thực thể CSDL DSS_SalesDB đạt chuẩn 3NF.", "FR-02.1: Đồng bộ CSDL 11 bảng: ")
    add_bullet_p(doc, "Hệ thống phải thực hiện quy trình ETL tự động: khử trùng lặp đơn hàng, tính tổng doanh thu bán lẻ thực tế và số ngày tồn kho trung bình.", "FR-02.2: Làm sạch dữ liệu ETL: ")
    add_bullet_p(doc, "Kiểm tra ràng buộc nghiệp vụ: ngăn chặn giá trị âm ở trường giá bán, giá vốn, số lượng tồn kho và lead time.", "FR-02.3: Toàn vẹn dữ liệu: ")

    # FR-03
    add_heading_2(doc, "3.3. FR-03: Bộ Não Toán Học AHP Saaty & Kiểm Định Nhất Quán (Tab 1 & Tab 2)")
    add_bullet_p(doc, "Cho phép người dùng thiết lập ma trận so sánh cặp giữa 5 tiêu chí (C1-C5) theo thang đo 1-9 của Thomas L. Saaty.", "FR-03.1: Ma trận Saaty 1-9: ")
    add_bullet_p(doc, "Tự động tính toán vector riêng, giá trị riêng cực đại lambda max, chỉ số nhất quán CI và tỷ số nhất quán CR = CI / 1.12.", "FR-03.2: Kiểm định CR <= 0.10: ")
    add_bullet_p(doc, "Nếu CR > 0.10, hệ thống phải đổi nhãn cảnh báo sang màu đỏ và khóa nút 'Chạy DSS', buộc người dùng phải hiệu chỉnh lại tính bắc cầu.", "FR-03.3: Khóa an toàn tỷ số CR: ")
    add_bullet_p(doc, "Tự động chuẩn hóa dữ liệu thô: Áp dụng hàm Benefit (r_ij = v_ij / v_max) cho C1, C2, C3 và hàm Cost (r_ij = v_min / v_ij) cho C4, C5.", "FR-03.4: Chuẩn hóa Benefit/Cost: ")
    add_bullet_p(doc, "Tổng hợp điểm AHP cuối cùng theo công thức: S_i = tổng(w_j * r_ij) và hiển thị bảng kết quả chi tiết.", "FR-03.5: Tính điểm tổng hợp AHP: ")

    # FR-04
    add_heading_2(doc, "3.4. FR-04: Xếp Hạng & Khuyến Nghị Số Lượng Nhập Hàng (Tab 2)")
    add_bullet_p(doc, "Tự động sắp xếp các sản phẩm theo thứ tự điểm AHP giảm dần từ cao xuống thấp (Hạng 1 đến Hạng 9).", "FR-04.1: Xếp hạng tự động: ")
    add_bullet_p(doc, "Phân loại phương án thành 3 nhóm trạng thái rõ rệt: Selected (Phê duyệt - Xanh lá), UnderReview (Cần xem xét - Cam), Rejected (Loại bỏ - Đỏ).", "FR-04.2: Phân loại 3 nhóm: ")
    add_bullet_p(doc, "Tự động tính số lượng gợi ý đặt hàng dựa trên nhu cầu ngày, Lead Time và chu kỳ bổ sung 30 ngày: Suggested_Qty = max(0, Nhu_cau * (Lead_Time + 30) - Ton_kho).", "FR-04.3: Công thức đặt hàng: ")
    add_bullet_p(doc, "Tự động phân bổ tỷ trọng ngân sách tiếp thị theo tỷ lệ điểm AHP của các sản phẩm thuộc nhóm Selected.", "FR-04.4: Phân bổ ngân sách marketing: ")

    # FR-05
    add_heading_2(doc, "3.5. FR-05: Mô Phỏng What-If & Phân Tích Độ Nhạy Thị Trường (Tab 3)")
    add_bullet_p(doc, "Cung cấp 3 kịch bản thị trường vi mô/vĩ mô định sẵn: Kịch bản 1 (Gaming +40%), Kịch bản 2 (Đứt gãy chuỗi cung ứng), Kịch bản 3 (Lạm phát thắt chặt).", "FR-05.1: 3 Kịch bản định sẵn: ")
    add_bullet_p(doc, "Cung cấp thanh trượt TrackBar cho phép người dùng tùy ý điều chỉnh tham số Nhu cầu, Lợi nhuận, Rủi ro của từng SKU theo thời gian thực.", "FR-05.2: Thanh trượt tùy biến: ")
    add_bullet_p(doc, "Tự động tính toán lại điểm số AHP tức thời và cập nhật Biểu đồ đường so sánh Delta và Sơ đồ cây cấu trúc thứ bậc phân cấp.", "FR-05.3: Trực quan hóa Delta Chart: ")

    # FR-06
    add_heading_2(doc, "3.6. FR-06: Quản Lý SKU & Thêm Mới Sản Phẩm Động (Tab 4)")
    add_bullet_p(doc, "Cung cấp giao diện trực quan cho phép nhập mã SKU mới, tên máy, giá niêm yết, giá vốn, tồn kho, lead time và 5 điểm tiêu chí thô.", "FR-06.1: Nhập liệu SKU mới: ")
    add_bullet_p(doc, "Tự động lưu vào bảng PRODUCTS và ALTERNATIVES trong CSDL SQL Server và tái tính toán lại toàn bộ xếp hạng AHP mà không cần khởi động lại ứng dụng.", "FR-06.2: Tái tính toán tức thời: ")

    # FR-07
    add_heading_2(doc, "3.7. FR-07: Phân Tích Bán Lẻ Đa Chiều OLAP (Tab 5)")
    add_bullet_p(doc, "Hỗ trợ cắt lát (Slicing) dữ liệu doanh thu và sản lượng theo từng Chi nhánh (Hà Nội, TP.HCM, Đà Nẵng, Cần Thơ).", "FR-07.1: Cắt lát (Slicing): ")
    add_bullet_p(doc, "Hỗ trợ thái hạt lựu (Dicing) kết hợp nhiều chiều phân tích (Chi nhánh x Ngành hàng x Thời gian).", "FR-07.2: Thái hạt lựu (Dicing): ")
    add_bullet_p(doc, "Hỗ trợ cuộn lên (Roll-up) từ Ngày/Tháng lên Quý/Năm và khoan sâu (Drill-down) từ Ngành hàng xuống từng mã SKU cụ thể.", "FR-07.3: Roll-up & Drill-down: ")

    # FR-08
    add_heading_2(doc, "3.8. FR-08: Dashboard Điều Hành KPI & Xuất Báo Cáo Thẩm Định (Tab 6)")
    add_bullet_p(doc, "Hiển thị trực quan 4 thẻ KPI: Tổng số SKU, Số lượng SKU phê duyệt (Selected), Doanh thu kỳ vọng và Tổng ngân sách tiếp thị.", "FR-08.1: Thẻ KPI điều hành: ")
    add_bullet_p(doc, "Thuật toán tự động sinh biên bản tóm tắt Executive Summary chỉ rõ sản phẩm chiến lược số 1 và cảnh báo rủi ro xả hàng tồn kho.", "FR-08.2: Tự sinh Executive Summary: ")
    add_bullet_p(doc, "Hỗ trợ xuất toàn bộ hồ sơ thẩm định ra file văn bản Microsoft Word (.docx) chuyên nghiệp và trang HTML tương tác phục vụ in ấn.", "FR-08.3: Xuất báo cáo Word & HTML: ")

    # =========================================================================
    # CHƯƠNG 4: ĐẶC TẢ YÊU CẦU PHI CHỨC NĂNG (NON-FUNCTIONAL REQUIREMENTS - NFR)
    # =========================================================================
    add_heading_1(doc, "CHƯƠNG 4: ĐẶC TẢ YÊU CẦU PHI CHỨC NĂNG (NON-FUNCTIONAL REQUIREMENTS)")
    
    nfr_headers = ["Mã NFR", "Hạng mục tiêu chuẩn", "Chỉ số định lượng cam kết đạt chuẩn (Target Metrics)"]
    nfr_data = [
        ["NFR-01", "Hiệu năng tính toán (Performance)", "Thời gian tính toán toàn bộ ma trận AHP cho 9 SKU < 200 miligiây; thời gian cập nhật mô phỏng What-If khi kéo thanh trượt < 100 miligiây."],
        ["NFR-02", "Thời gian phản hồi CSDL (Database Response)", "Thời gian thực thi truy vấn nạp dữ liệu từ 11 bảng SQL Server khi khởi động < 1.0 giây."],
        ["NFR-03", "An toàn bảo mật (Security)", "Mật khẩu băm 100% bằng SHA-256; dữ liệu phiên UserSession bị hủy hoàn toàn khi đóng ứng dụng; phân quyền chặt chẽ trên UI."],
        ["NFR-04", "Khả năng chịu lỗi (Fault-Tolerance & Fail-Safe)", "Khi ngắt dịch vụ SQL Server, panel pnlDbOverlay phải khóa cứng 6 Tab trong vòng < 1 giây; tự động phục hồi kết nối không gây sập ứng dụng (Zero Crash)."],
        ["NFR-05", "Khả năng sử dụng (Usability)", "Giao diện tuân thủ luồng điều hướng 6 Tab khép kín; cảnh báo vi phạm CR hiển thị màu đỏ rõ ràng; thời gian người dùng mới làm quen hệ thống < 15 phút."],
        ["NFR-06", "Khả năng tương thích (Compatibility)", "Tương thích hoàn toàn trên hệ điều hành Windows 10 và Windows 11 (64-bit); hoạt động ổn định trên màn hình Full HD (1920x1080)."],
        ["NFR-07", "Khả năng bảo trì (Maintainability)", "Kiến trúc phân rã 3 tầng độc lập; mã nguồn C# tuân thủ chuẩn đặt tên PascalCase; lớp DssEngine độc lập với giao diện WinForms."]
    ]
    create_styled_table(doc, nfr_headers, nfr_data, col_widths=[1.0, 2.2, 3.3])

    # =========================================================================
    # CHƯƠNG 5: YÊU CẦU GIAO DIỆN HỆ THỐNG (EXTERNAL INTERFACES)
    # =========================================================================
    add_heading_1(doc, "CHƯƠNG 5: YÊU CẦU GIAO DIỆN HỆ THỐNG (EXTERNAL INTERFACE REQUIREMENTS)")
    
    add_heading_2(doc, "5.1. Giao diện người dùng (User Interfaces)")
    add_body_p(
        doc,
        "Hệ thống cung cấp giao diện đồ họa người dùng (GUI) nhất quán trên nền tảng Windows Forms với bảng màu chủ đạo "
        "Slate Navy (#0F172A), Indigo (#4F46E5) và Emerald Green (#059669). Cấu trúc màn hình gồm:"
    )
    add_bullet_p(doc, "Gồm SplashForm (khởi động kiểm tra CSDL), LoginForm (đăng nhập bảo mật SHA-256) và RegisterForm (tạo tài khoản mới).", "• Cụm màn hình xác thực: ")
    add_bullet_p(doc, "Giao diện chính Form1 gồm thanh công cụ phía trên hiển thị thông tin UserSession và 6 Tab điều hướng: Tab 1 (Đầu vào Saaty), Tab 2 (Kết quả xếp hạng AHP), Tab 3 (Mô phỏng What-If), Tab 4 (Thêm SKU động), Tab 5 (Phân tích OLAP), Tab 6 (Xuất Báo cáo thẩm định).", "• Màn hình làm việc Form1: ")
    add_bullet_p(doc, "Panel pnlDbOverlay màu đỏ cảnh báo che phủ toàn bộ 6 Tab khi phát hiện ngắt kết nối mạng hoặc dừng dịch vụ database.", "• Màn hình bảo vệ Fail-Safe: ")

    add_heading_2(doc, "5.2. Giao diện phần mềm & Cơ sở dữ liệu (Software & Database Interfaces)")
    add_bullet_p(doc, "Sử dụng thư viện System.Data.SqlClient trên nền .NET 10.0 để kết nối chuỗi ConnectionString đến SQL Server instance localhost\\SQLEXPRESS25, Database: DSS_SalesDB.", "• Kết nối CSDL SQL Server: ")
    add_bullet_p(doc, "Sử dụng thư viện OpenXML / docx để xuất kết quả bảng xếp hạng và biên bản tóm tắt điều hành ra file Microsoft Word (.docx).", "• Xuất báo cáo văn bản: ")

    # =========================================================================
    # CHƯƠNG 6: MA TRẬN TRUY VẾT YÊU CẦU (REQUIREMENTS TRACEABILITY MATRIX - RTM)
    # =========================================================================
    add_heading_1(doc, "CHƯƠNG 6: MA TRẬN TRUY VẾT YÊU CẦU (REQUIREMENTS TRACEABILITY MATRIX - RTM)")
    
    add_body_p(
        doc,
        "Bảng ma trận truy vết dưới đây đối soát từng yêu cầu nghiệp vụ với thành phần mã nguồn, bảng CSDL và phương pháp kiểm thử thực tế:"
    )
    
    rtm_headers = ["Mã Yêu Cầu", "Tên Tính Năng Nghiệp Vụ", "Thành phần Mã Nguồn (Code Component)", "Bảng CSDL Liên Quan", "Phương Pháp Kiểm Thử"]
    rtm_data = [
        ["FR-01", "Xác thực SHA-256 & Fail-Safe", "LoginForm.cs, DatabaseGuard.cs", "USERS", "Unit Test + Test ngắt SQL Server"],
        ["FR-02", "ETL Làm Sạch Dữ Liệu 3NF", "DssEngine.cs, 04_data_processing.sql", "ORDERS, PRODUCTS,...", "Database Integrity Test"],
        ["FR-03", "Ma trận Saaty & CR <= 0.10", "Form1.cs (Tab 1), DssEngine.cs", "PAIRWISE_COMPARISONS", "Toán học AHP Test Case"],
        ["FR-04", "Xếp hạng & Gợi ý số lượng đặt", "Form1.cs (Tab 2), DssEngine.cs", "DECISION_RESULTS", "Black-box Acceptance Test"],
        ["FR-05", "Mô phỏng What-If & Delta Chart", "Form1.cs (Tab 3), DssEngine.cs", "ALTERNATIVES", "UI Simulation Test"],
        ["FR-06", "Thêm SKU mới động", "Form1.cs (Tab 4), DssEngine.cs", "PRODUCTS, ALTERNATIVES", "CRUD Dynamic Re-calc Test"],
        ["FR-07", "Phân tích bán lẻ OLAP đa chiều", "Form1.cs (Tab 5), SQL Queries", "ORDERS, CATEGORIES", "OLAP Query Benchmark Test"],
        ["FR-08", "Dashboard KPI & Xuất Word", "Form1.cs (Tab 6), docx export", "DECISION_SESSIONS", "Report Generation Test"],
        ["NFR-04", "Cơ chế khóa cứng Fail-Safe", "pnlDbOverlay, DatabaseGuard.cs", "Tất cả 11 bảng", "Physical Network Disconnect Test"]
    ]
    create_styled_table(doc, rtm_headers, rtm_data, col_widths=[0.8, 1.8, 1.8, 1.2, 1.4])

    # =========================================================================
    # KẾT LUẬN & KÝ DUYỆT
    # =========================================================================
    add_heading_1(doc, "CHƯƠNG 7: KẾT LUẬN & XÁC NHẬN NGHIỆM THU ĐẶC TẢ")
    add_body_p(
        doc,
        "Tài liệu Đặc tả Yêu cầu Phần mềm (SRS) phiên bản 1.0 đã định nghĩa toàn diện, chính xác và đầy đủ "
        "tất cả các yêu cầu chức năng và phi chức năng của hệ thống DSS Retail Decision Studio 2026. "
        "Đây là căn cứ kỹ thuật chuẩn mực để Hội đồng chấm thi và Giảng viên hướng dẫn đánh giá mức độ hoàn thiện "
        "của sản phẩm công nghệ phần mềm trong kỳ bảo vệ đồ án kết thúc học phần IS 402 A."
    )
    
    # Signature Box Table
    tbl_sig = doc.add_table(rows=2, cols=2)
    tbl_sig.alignment = WD_TABLE_ALIGNMENT.CENTER
    set_table_borders(tbl_sig, color="FFFFFF", sz="0")
    
    cell_s1 = tbl_sig.rows[0].cells[0]
    p_s1 = cell_s1.paragraphs[0]
    p_s1.alignment = WD_ALIGN_PARAGRAPH.CENTER
    r_s1 = p_s1.add_run("ĐẠI DIỆN NHÓM SINH VIÊN THỰC HIỆN\nTrưởng nhóm Nhóm 05\n\n\n\n(Ký và ghi rõ họ tên)\n")
    r_s1.font.name = "Segoe UI"
    r_s1.font.bold = True
    r_s1_name = p_s1.add_run("Trần Hồng Minh")
    r_s1_name.font.name = "Segoe UI"
    r_s1_name.font.bold = True
    r_s1_name.font.color.rgb = RGBColor(79, 70, 229)
    cell_s1.width = Inches(3.3)
    
    cell_s2 = tbl_sig.rows[0].cells[1]
    p_s2 = cell_s2.paragraphs[0]
    p_s2.alignment = WD_ALIGN_PARAGRAPH.CENTER
    r_s2 = p_s2.add_run("GIẢNG VIÊN HƯỚNG DẪN PHÊ DUYỆT\nKhoa Hệ Thống Thông Tin Quản Lý\n\n\n\n(Ký và ghi rõ họ tên)\n")
    r_s2.font.name = "Segoe UI"
    r_s2.font.bold = True
    r_s2_name = p_s2.add_run("ThS. Nguyễn Quang Ánh")
    r_s2_name.font.name = "Segoe UI"
    r_s2_name.font.bold = True
    r_s2_name.font.color.rgb = RGBColor(15, 23, 42)
    cell_s2.width = Inches(3.3)

    output_docx = "docs/TAI_LIEU_SRS_DAC_TA_YEU_CAU_PHAN_MEM_DSS_IS402_NHOM05.docx"
    os.makedirs(os.path.dirname(output_docx), exist_ok=True)
    doc.save(output_docx)
    print(f"File Word SRS da duoc luu thanh cong tai: {output_docx}")

if __name__ == "__main__":
    build_srs_document()
