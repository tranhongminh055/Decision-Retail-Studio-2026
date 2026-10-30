# -*- coding: utf-8 -*-
"""
Script tao Tai lieu Dac ta Pham vi He thong (In-Scope & Out-of-Scope)
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

def build_scope_document():
    print("Khoi tao Document Dac ta Pham vi He thong...")
    doc = create_document()
    
    # Custom Header / Footer for Scope Document
    header = doc.sections[0].header
    hp = header.paragraphs[0]
    hp.text = ""
    hp.alignment = WD_ALIGN_PARAGRAPH.LEFT
    hr = hp.add_run("TÀI LIỆU ĐẶC TẢ PHẠM VI HỆ THỐNG (SYSTEM SCOPE SPECIFICATION) • IN-SCOPE & OUT-OF-SCOPE")
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
    r_t = p_title.add_run("TÀI LIỆU ĐẶC TẢ PHẠM VI HỆ THỐNG\n(SYSTEM SCOPE SPECIFICATION DOCUMENT)\n")
    r_t.font.name = "Segoe UI"
    r_t.font.size = Pt(19)
    r_t.font.bold = True
    r_t.font.color.rgb = RGBColor(15, 23, 42)
    
    p_sub = doc.add_paragraph()
    p_sub.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_sub.paragraph_format.space_after = Pt(25)
    r_s = p_sub.add_run("PHÂN ĐỊNH RANH GIỚI TRONG PHẠM VI (IN-SCOPE) & NGOÀI PHẠM VI (OUT-OF-SCOPE)\n")
    r_s.font.name = "Segoe UI"
    r_s.font.size = Pt(11.5)
    r_s.font.bold = True
    r_s.font.color.rgb = RGBColor(37, 99, 235) # Royal Blue
    
    r_s2 = p_sub.add_run("Đề tài: Hệ Thống Hỗ Trợ Ra Quyết Định Bán Hàng DSS Retail Decision Studio 2026\nỨng Dụng Mô Hình AHP Saaty & Cơ Sở Dữ Liệu SQL Server")
    r_s2.font.name = "Segoe UI"
    r_s2.font.size = Pt(10.5)
    r_s2.font.italic = True
    r_s2.font.color.rgb = RGBColor(100, 116, 139)

    # Info Box
    tbl_info = doc.add_table(rows=7, cols=2)
    tbl_info.alignment = WD_TABLE_ALIGNMENT.CENTER
    set_table_borders(tbl_info, color="CBD5E1", sz="4")
    
    info_data = [
        ("Mã học phần:", "IS 402 A – Hệ Thống Thông Tin Hỗ Trợ Ra Quyết Định (DSS)"),
        ("Giảng viên hướng dẫn:", "ThS. Nguyễn Quang Ánh"),
        ("Nhóm sinh viên thực hiện:", "Nhóm 05 – Lớp IS 402 A"),
        ("Danh sách thành viên:", "1. Trần Hồng Minh (Trưởng nhóm - 29210247142)\n2. Hồ Thị Thuận (29201164457)\n3. Lê Thị Nga (29206361029)\n4. Phạm Ngọc Tú (29204347819)\n5. Lê Thanh Diệu (29204622434)"),
        ("Ứng dụng kiểm thử:", "DSS Retail Decision Studio 2026 (C# .NET 10 WinForms & SQL Server)"),
        ("Ngày phát hành:", "29/10/2026 (Phiên bản chính thức 1.0)"),
        ("Trạng thái phê duyệt:", "Đã nghiệm thu nội bộ & Sẵn sàng bảo vệ đồ án")
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
    # LỊCH SỬ THAY ĐỔI & TÓM TẮT ĐIỀU HÀNH
    # =========================================================================
    add_heading_1(doc, "BẢNG THEO DÕI LỊCH SỬ PHIÊN BẢN (DOCUMENT REVISION HISTORY)")
    
    rev_headers = ["Phiên bản", "Ngày cập nhật", "Tác giả / Nhóm", "Mô tả nội dung thay đổi chính", "Phê duyệt"]
    rev_data = [
        ["v0.1", "15/10/2026", "Trần Hồng Minh", "Khởi tạo tài liệu phác thảo phạm vi sơ bộ", "Nhóm 05"],
        ["v0.5", "22/10/2026", "Lê Thị Nga, Phạm Ngọc Tú", "Bổ sung ranh giới CSDL SQL Server và thuật toán AHP 5 tiêu chí", "Trần Hồng Minh"],
        ["v0.8", "26/10/2026", "Hồ Thị Thuận, Lê Thanh Diệu", "Đặc tả các thành phần Out-of-Scope (POS, WMS, Logistics, ERP)", "Trần Hồng Minh"],
        ["v1.0", "29/10/2026", "Toàn thể Nhóm 05", "Hoàn thiện đặc tả In-Scope/Out-of-Scope, tích hợp Sơ đồ ranh giới 300 DPI", "ThS. Nguyễn Quang Ánh"]
    ]
    create_styled_table(doc, rev_headers, rev_data, col_widths=[0.8, 1.0, 1.3, 2.5, 0.9])
    
    add_callout(
        doc,
        "Tài liệu này xác định ranh giới chức năng và công nghệ bất biến của dự án DSS Retail Decision Studio 2026. "
        "Mọi yêu cầu phát sinh nằm ngoài các danh mục In-Scope được định nghĩa tại đây sẽ được phân loại là Out-of-Scope "
        "và chỉ được xem xét trong các giai đoạn phát triển tiếp theo nhằm ngăn ngừa rủi ro trượt tiến độ (Scope Creep).",
        title="TÓM TẮT ĐIỀU HÀNH QUẢN LÝ PHẠM VI (EXECUTIVE SCOPE SUMMARY):",
        border_color="2563EB",
        bg_color="EFF6FF"
    )

    # =========================================================================
    # PHẦN 1: TỔNG QUAN & BỐI CẢNH DỰ ÁN
    # =========================================================================
    add_heading_1(doc, "1. TỔNG QUAN BÀI TOÁN & MỤC ĐÍCH PHÂN ĐỊNH PHẠM VI")
    
    add_heading_2(doc, "1.1. Bối cảnh bài toán kinh doanh bán lẻ Quý 4/2026")
    add_body_p(
        doc,
        "Trong ngành bán lẻ thiết bị công nghệ số (Smartphones, Laptops, Phụ kiện), Quý 4 hàng năm luôn là 'mùa vàng' "
        "bùng nổ doanh số với các đợt cao điểm mua sắm Black Friday, Giáng Sinh và cận Tết Nguyên Đán. Tuy nhiên, "
        "đây cũng chính là giai đoạn tiềm ẩn rủi ro đọng vốn và tồn kho cao nhất của năm tài chính."
    )
    add_body_p(
        doc,
        "Nhà quản trị bán lẻ phải đối mặt với bài toán tối ưu hóa danh mục sản phẩm (SKU Portfolio Optimization) "
        "dưới áp lực của 5 tiêu chí nghiệp vụ mang tính chất xung đột trực diện:"
    )
    add_bullet_p(doc, "Mong muốn đẩy mạnh các dòng sản phẩm cao cấp có biên lợi nhuận gộp lớn.", "1. Biên lợi nhuận gộp (Profit Margin): ")
    add_bullet_p(doc, "Kỳ vọng dòng tiền lớn và thị phần cao thông qua các mặt hàng bán chạy.", "2. Doanh thu kỳ vọng (Expected Revenue): ")
    add_bullet_p(doc, "Đón đầu thị hiếu công nghệ mới và các sản phẩm đang 'hot trend' của người tiêu dùng.", "3. Nhu cầu thị trường (Market Trend): ")
    add_bullet_p(doc, "Đòi hỏi số ngày tồn kho (Days of Supply) phải thật ngắn để quay vòng vốn nhanh, hạn chế nguy cơ lỗi thời.", "4. Rủi ro tồn kho (Inventory Risk): ")
    add_bullet_p(doc, "Ngân sách quảng cáo, chiết khấu và khuyến mãi phải được kiểm soát tối ưu, không vượt trần tài chính.", "5. Chi phí tiếp thị (Marketing Cost): ")
    
    add_body_p(
        doc,
        "Nếu dựa vào thói quen hoặc trực giác cảm tính, doanh nghiệp rất dễ rơi vào tình trạng đọng vốn hàng chục tỷ đồng "
        "ở các mặt hàng ế ẩm hoặc ngược lại bị 'cháy hàng' (Out-of-Stock) ở các mặt hàng chủ lực, đánh mất khách hàng vào tay đối thủ. "
        "Hệ thống DSS Retail Decision Studio 2026 ra đời nhằm giải quyết triệt để bài toán này thông qua việc lượng hóa 100% "
        "quá trình đánh giá bằng mô hình toán học đa tiêu chí AHP kết hợp phân tích đa chiều OLAP."
    )

    add_heading_2(doc, "1.2. Mục đích thiết lập ranh giới phạm vi hệ thống")
    add_body_p(
        doc,
        "Một trong những nguyên nhân hàng đầu dẫn đến thất bại của các dự án phần mềm là sự nhầm lẫn giữa hệ thống giao dịch tác nghiệp "
        "(Transaction Processing System - TPS/POS/ERP) và hệ thống hỗ trợ ra quyết định (Decision Support System - DSS). "
        "Việc phân định ranh giới In-Scope và Out-of-Scope nhằm đạt được các mục tiêu cốt lõi sau:"
    )
    add_bullet_p(doc, "Tập trung tuyệt đối vào mô hình hóa bài toán quyết định, khử bỏ cảm tính cá nhân và đưa ra khuyến nghị khoa học.", "Tập trung năng lực cốt lõi: ");
    add_bullet_p(doc, "Không ôm đồm các chức năng máy tính tiền tại quầy, quản lý xuất nhập kho vật lý, hạch toán kế toán hoặc điều phối vận tải logistics.", "Phòng chống trượt phạm vi (Scope Creep): ");
    add_bullet_p(doc, "Xác định rõ ràng cấu trúc dữ liệu đầu vào (Data In) mà DSS cần đọc từ cơ sở dữ liệu và các định dạng đầu ra (Decisions Out) phục vụ cuộc họp giao ban quản trị.", "Minh bạch ranh giới tích hợp: ");
    add_bullet_p(doc, "Cung cấp căn cứ định lượng để Hội đồng đánh giá đồ án, Giảng viên hướng dẫn và các bên liên quan kiểm thử và nghiệm thu chức năng phần mềm.", "Chuẩn hóa tiêu chí nghiệm thu: ");

    # =========================================================================
    # PHẦN 2: SƠ ĐỒ TRỰC QUAN HÓA RANH GIỚI PHẠM VI (DIAGRAM)
    # =========================================================================
    add_heading_1(doc, "2. SƠ ĐỒ TRỰC QUAN HÓA RANH GIỚI PHẠM VI (SYSTEM SCOPE BOUNDARY DIAGRAM)")
    
    add_body_p(
        doc,
        "Sơ đồ dưới đây mô tả trực quan và toàn diện ranh giới giữa Phạm vi Trong hệ thống (In-Scope - Khung màu xanh) "
        "và Phạm vi Ngoài hệ thống (Out-of-Scope - Các khung nét đứt màu đỏ), cùng sự tương tác với Nhà quản trị (Khung màu vàng) "
        "thông qua các luồng dữ liệu hai chiều rõ ràng:"
    )
    
    diagram_path = "docs/images/sodo_pham_vi_inscope_outscope.png"
    add_image_figure(doc, diagram_path, "Hình 2.1: Sơ đồ kiến trúc phân định ranh giới phạm vi In-Scope và Out-of-Scope của DSS Retail Decision Studio 2026")
    
    add_heading_2(doc, "2.1. Thuyết minh kiến trúc luồng dữ liệu qua ranh giới hệ thống")
    add_body_p(
        doc,
        "Kiến trúc ranh giới được vận hành dựa trên 3 nguyên lý liên kết bất đối xứng:"
    )
    add_bullet_p(
        doc, 
        "Hệ thống DSS định kỳ tiếp nhận hoặc truy vấn dữ liệu giao dịch bán lẻ từ bảng ORDERS, ORDER_DETAILS, "
        "thông tin tồn kho hiện tại (stock_quantity) và thông số kỹ thuật sản phẩm (PRODUCTS, CATEGORIES). DSS chỉ đọc dữ liệu (Read-Only) "
        "và tổ chức xử lý ETL, tuyệt đối không chỉnh sửa dữ liệu hóa đơn gốc của hệ thống POS bên ngoài.", 
        "1. Luồng dữ liệu nạp vào (Data Inflow): "
    )
    add_bullet_p(
        doc, 
        "Toàn bộ quá trình tính toán ma trận Saaty, kiểm định tỷ số nhất quán CR, chuẩn hóa hàm Benefit/Cost, "
        "mô phỏng What-If, phân tích khối dữ liệu OLAP và lập bảng xếp hạng phân loại (Selected / UnderReview / Rejected) "
        "được thực thi khép kín 100% bên trong lõi DssEngine của phần mềm.", 
        "2. Xử lý tính toán nội hàm (Internal Decision Engine): "
    )
    add_bullet_p(
        doc, 
        "Đầu ra của DSS là các bảng chỉ số điều hành KPI, đồ thị phân tích độ nhạy, biên bản tóm tắt Executive Summary "
        "và file Báo cáo thẩm định xuất ra định dạng Microsoft Word (.docx)/HTML. Các khuyến nghị số lượng đặt hàng tối ưu "
        "được trình lên Nhà quản trị (Decision Maker) để xem xét phê duyệt theo cơ chế 'Human-in-the-Loop'.", 
        "3. Luồng khuyến nghị tham mưu (Advisory Outflow): "
    )

    # =========================================================================
    # PHẦN 3: ĐẶC TẢ CHI TIẾT PHẠM VI BÊN TRONG HỆ THỐNG (IN-SCOPE)
    # =========================================================================
    add_heading_1(doc, "3. ĐẶC TẢ CHI TIẾT PHẠM VI BÊN TRONG HỆ THỐNG (IN-SCOPE)")
    
    add_body_p(
        doc,
        "Dưới đây là đặc tả chi tiết 7 phân hệ nghiệp vụ cốt lõi mà phần mềm DSS Retail Decision Studio 2026 "
        "chịu trách nhiệm trực tiếp xây dựng, cài đặt và bảo đảm vận hành trơn tru:"
    )

    add_heading_2(doc, "3.1. Phân hệ Quản trị Người dùng, Bảo mật & Cơ chế Phòng vệ Fail-Safe")
    add_bullet_p(doc, "Màn hình khởi động tiến hành kiểm tra kết nối chuỗi kết nối SQL Server (DatabaseGuard). Nếu kết nối thất bại, chặn truy cập và thông báo trực quan.", "Khởi tạo kết nối an toàn (SplashForm): ")
    add_bullet_p(doc, "Sử dụng thuật toán mật mã học chuẩn công nghiệp SHA-256 để băm mật khẩu người dùng trước khi lưu trữ hoặc đối chiếu với trường PasswordHash trong bảng USERS. Tuyệt đối không lưu mật khẩu dạng bản rõ (plaintext).", "Mã hóa băm mật khẩu SHA-256 (LoginForm): ")
    add_bullet_p(doc, "Sau khi xác thực thành công, lưu vết định danh, họ tên và nhóm quyền của tài khoản vào đối tượng toàn cục UserSession để kiểm soát phân quyền trên toàn bộ các tính năng của Form1 theo 4 vai trò: Admin, DecisionMaker, SalesManager, SalesStaff.", "Quản lý phiên làm việc (UserSession): ")
    add_bullet_p(doc, "Cung cấp giao diện trực quan cho phép nhân viên mới đăng ký tài khoản, kiểm tra trùng lặp tên đăng nhập, tự động băm mật khẩu SHA-256 và lưu trữ vào bảng USERS.", "Màn hình Đăng ký (RegisterForm): ")
    add_bullet_p(doc, "Tích hợp luồng giám sát định kỳ trạng thái dịch vụ SQL Server. Nếu phát hiện ngắt mạng hoặc tắt dịch vụ database, panel pnlDbOverlay lập tức hiển thị che phủ và khóa cứng 6 Tab chức năng, ngăn chặn người dùng nhập liệu sai lệch; tự động mở khóa khi kết nối phục hồi.", "Cơ chế Fail-Safe (DatabaseGuard & pnlDbOverlay): ")

    add_heading_2(doc, "3.2. Phân hệ Cơ sở Dữ liệu Quan hệ Chuẩn Hóa 3NF (SQL Server)")
    add_body_p(
        doc,
        "Cơ sở dữ liệu trung tâm DSS_SalesDB được thiết kế đạt Dạng chuẩn 3 (3NF), loại bỏ hoàn toàn các dị thường thêm, "
        "xóa, sửa (anomalies) và đảm bảo tính toàn vẹn tham chiếu thông qua 11 bảng thực thể chia làm 2 phân vùng:"
    )
    add_bullet_p(doc, "Gồm 6 bảng: CATEGORIES (Ngành hàng), PRODUCTS (Sản phẩm), CUSTOMERS (Khách hàng), PROMOTIONS (Khuyến mãi), ORDERS (Đơn hàng) và ORDER_DETAILS (Chi tiết hóa đơn). Lưu trữ toàn bộ dữ liệu lịch sử phục vụ tính toán doanh số và tồn kho.", "Phân vùng Nghiệp vụ Bán lẻ: ")
    add_bullet_p(doc, "Gồm 5 bảng: DECISION_SESSIONS (Phiên quyết định), CRITERIA (Danh mục 5 tiêu chí), PAIRWISE_COMPARISONS (Ma trận so sánh cặp), ALTERNATIVES (Danh sách SKU ứng viên), ALTERNATIVE_SCORES (Điểm thô & điểm chuẩn hóa) và DECISION_RESULTS (Bảng kết quả xếp hạng).", "Phân vùng Mô hình DSS: ")
    add_bullet_p(doc, "Tự động truy vấn lịch sử hóa đơn bán hàng, tính tổng doanh thu bán lẻ thực tế, biên lợi nhuận trung bình và số ngày bán hết hàng tồn để điền vào ma trận phương án đầu vào của DSS.", "Xử lý ETL & Làm sạch dữ liệu: ")

    add_heading_2(doc, "3.3. Phân hệ Bộ Não Toán Học AHP Saaty & Xếp Hạng Phương Án (Tab 1 & Tab 2)")
    add_bullet_p(doc, "Cho phép chuyên viên thiết lập tỷ lệ ưu tiên giữa 5 tiêu chí cốt lõi (Biên lợi nhuận C1, Doanh thu C2, Nhu cầu C3, Rủi ro tồn kho C4, Chi phí tiếp thị C5) theo thang đo 1 – 9 của Saaty.", "Ma trận so sánh cặp tiêu chí (Tab 1): ")
    add_bullet_p(doc, "Tự động tính giá trị riêng cực đại (lambda max), Chỉ số nhất quán CI = (lambda_max - n)/(n - 1) và Tỷ số nhất quán CR = CI / RI (với n=5, RI=1.12). Hệ thống cảnh báo đỏ và khóa tính toán nếu CR > 0.10, bắt buộc người dùng điều chỉnh lại tính bắc cầu.", "Kiểm định tỷ số nhất quán CR <= 0.10: ")
    add_bullet_p(doc, "Tự động áp dụng công thức chuẩn hóa riêng biệt: Tiêu chí Lợi ích Benefit (r_ij = v_ij / v_max) và Tiêu chí Chi phí/Rủi ro Cost (r_ij = v_min / v_ij).", "Chuẩn hóa dữ liệu theo hàm Benefit / Cost: ")
    add_bullet_p(doc, "Tổng hợp điểm số S_i = tổng(w_j * r_ij); tự động sắp xếp theo thứ tự giảm dần và gắn cờ trạng thái: Selected (Ưu tiên phê duyệt), UnderReview (Cần xem xét thêm), Rejected (Loại bỏ).", "Tính điểm tổng hợp & Phân loại phương án (Tab 2): ")
    add_bullet_p(doc, "Tính toán số lượng đặt hàng gợi ý dựa trên nhu cầu kỳ vọng, thời gian chờ hàng (Lead Time) và chu kỳ đặt hàng 30 ngày: Suggested_Qty = max(0, Daily_Demand * (Lead_Time + 30) - Current_Stock).", "Thuật toán khuyến nghị số lượng nhập hàng: ")

    add_heading_2(doc, "3.4. Phân hệ Mô Phỏng What-If & Phân Tích Độ Nhạy Thị Trường (Tab 3)")
    add_bullet_p(doc, "Cung cấp 3 kịch bản vi mô/vĩ mô tích hợp sẵn: Kịch bản 1 (Bùng nổ thiết bị Gaming +40%), Kịch bản 2 (Đứt gãy chuỗi cung ứng - Tăng rủi ro vốn tồn kho), Kịch bản 3 (Lạm phát thắt chặt chi tiêu - Ưu tiên biên lợi nhuận an toàn).", "3 Kịch bản thị trường định sẵn: ")
    add_bullet_p(doc, "Cung cấp các thanh trượt (TrackBar) cho phép người dùng tùy ý điều chỉnh tham số Nhu cầu, Biên lợi nhuận và Rủi ro tồn kho của từng sản phẩm để quan sát sự dịch chuyển điểm số AHP theo thời gian thực.", "Thanh trượt tùy biến tham số linh hoạt: ")
    add_bullet_p(doc, "Hiển thị đồ thị Delta so sánh trực tiếp điểm số gốc và điểm số sau mô phỏng; sơ đồ cây phân cấp thứ bậc từ Mục tiêu tổng thể xuống Tiêu chí và từng SKU kèm mũi tên trực quan.", "Trực quan hóa đồ thị Delta & Cây AHP: ")

    add_heading_2(doc, "3.5. Phân hệ Quản Lý Danh Mục & Thêm SKU Mới Động (Tab 4)")
    add_bullet_p(doc, "Cho phép chuyên viên nhập liệu trực tiếp các thông số kỹ thuật của một SKU mới dự kiến nhập khẩu/phân phối (Tên SKU, Giá niêm yết, Giá vốn, Tồn kho ban đầu, Lead Time, Điểm nhu cầu thị trường, Chi phí marketing).", "Nhập thông số SKU mới trực tiếp: ")
    add_bullet_p(doc, "Hệ thống tự động đồng bộ sản phẩm mới vào bảng PRODUCTS trong CSDL SQL Server và tái tính toán lại toàn bộ ma trận AHP của tất cả phương án, cập nhật lại bảng xếp hạng tức thì mà không cần khởi động lại ứng dụng.", "Tái tính toán tức thời (Dynamic Re-calculation): ")

    add_heading_2(doc, "3.6. Phân hệ Phân Tích Dữ Liệu Bán Hàng Đa Chiều OLAP (Tab 5)")
    add_bullet_p(doc, "Lọc dữ liệu doanh số và sản lượng theo từng Chi nhánh cụ thể (Chi nhánh Hà Nội, TP.HCM, Đà Nẵng, Cần Thơ).", "Cắt lát dữ liệu (Slicing): ")
    add_bullet_p(doc, "Giao cắt nhiều chiều phân tích cùng lúc, ví dụ: Xem doanh thu của Ngành hàng Laptops tại Chi nhánh TP.HCM trong Quý 4.", "Thái hạt lựu (Dicing): ")
    add_bullet_p(doc, "Tổng hợp số liệu từ cấp Ngày/Tháng lên cấp Quý và Năm.", "Cuộn lên (Roll-up): ")
    add_bullet_p(doc, "Chi tiết hóa từ Doanh số Danh mục chung xuống chi tiết từng SKU sản phẩm cụ thể.", "Khoan sâu (Drill-down): ")
    add_bullet_p(doc, "Trình bày ma trận số liệu tổng hợp doanh thu, tỷ trọng lợi nhuận và biểu đồ trực quan phục vụ khảo sát xu hướng trước khi đưa vào mô hình AHP.", "Bảng tổng hợp OLAP Grid & Biểu đồ: ")

    add_heading_2(doc, "3.7. Phân hệ Dashboard Chỉ Số Điều Hành & Xuất Bản Báo Cáo Thẩm Định (Tab 6)")
    add_bullet_p(doc, "Tổng hợp trực quan các chỉ số trọng yếu: Tổng số SKU ứng viên, Số lượng SKU phê duyệt (Selected), Tổng doanh thu kỳ vọng và Tổng ngân sách tiếp thị đề xuất.", "Thẻ chỉ số điều hành (Executive KPI Cards): ")
    add_bullet_p(doc, "Tự động phân tích kết quả xếp hạng và sinh văn bản tóm tắt điều hành (Executive Summary) chỉ ra rõ phương án chiến lược số 1, các sản phẩm rủi ro cần xả hàng và phương án phân bổ ngân sách tối ưu.", "Tự động sinh Tóm tắt Điều hành (Executive Summary): ")
    add_bullet_p(doc, "Hỗ trợ xuất toàn bộ hồ sơ thẩm định ra file văn bản Microsoft Word (.docx) chuyên nghiệp và trang HTML tương tác, sẵn sàng in ấn phục vụ cuộc họp giao ban cấp quản lý.", "Xuất báo cáo đa định dạng (Word .docx & HTML): ")

    # =========================================================================
    # PHẦN 4: ĐẶC TẢ CHI TIẾT PHẠM VI NẰM NGOÀI HỆ THỐNG (OUT-OF-SCOPE)
    # =========================================================================
    add_heading_1(doc, "4. ĐẶC TẢ CHI TIẾT PHẠM VI NẰM NGOÀI HỆ THỐNG (OUT-OF-SCOPE)")
    
    add_body_p(
        doc,
        "Để đảm bảo sự rành mạch trong thiết kế kiến trúc và cam kết chất lượng của đồ án, các tính năng, phân hệ "
        "hoặc quy trình nghiệp vụ dưới đây được tuyên bố dứt khoát là NẰM NGOÀI PHẠM VI CỦA PHẦN MỀM DSS:"
    )

    add_heading_2(doc, "4.1. Không đóng vai trò Hệ thống Bán hàng tại Điểm bán (Point of Sale - POS)")
    add_bullet_p(doc, "Hệ thống DSS không kết nối đầu đọc mã vạch để quét sản phẩm tại quầy thu ngân khi khách hàng mua sắm.", "Không quét mã vạch tính tiền tại quầy: ")
    add_bullet_p(doc, "Hệ thống không xử lý quẹt thẻ POS ngân hàng, không quét mã QR thanh toán ví điện tử và không quản lý ngăn kéo tiền mặt.", "Không xử lý giao dịch thanh toán trực tiếp: ")
    add_bullet_p(doc, "Hệ thống không in hóa đơn giấy (receipt) trao tay cho khách hàng tiêu dùng lẻ.", "Không in biên lai/hóa đơn bán lẻ: ")
    add_bullet_p(doc, "Hệ thống chỉ đọc dữ liệu lịch sử đơn hàng từ bảng ORDERS trong SQL Server do hệ thống POS đồng bộ về.", "Ranh giới kết nối: ")

    add_heading_2(doc, "4.2. Không đóng vai trò Hệ thống Quản lý Kho vật lý (Warehouse Management System - WMS)")
    add_bullet_p(doc, "Kết quả số lượng đặt hàng gợi ý từ DSS không làm biến đổi tức thời trường tồn kho thực tế trong kho vật lý.", "Không tự động trừ tồn kho vật lý: ")
    add_bullet_p(doc, "Hệ thống không quản lý bản đồ vị trí ô kệ, không in tem RFID gắn lên pallet và không điều phối xe nâng trong kho.", "Không quản lý vị trí giá kệ & mã định danh RFID: ")
    add_bullet_p(doc, "Hệ thống không thay thế việc kiểm đếm thực tế của thủ kho; DSS chỉ tiếp nhận con số tồn kho thô để làm mẫu số tính số ngày quay vòng vốn.", "Không kiểm kê kho tự động: ")
    add_bullet_p(doc, "Sau khi DSS đề xuất số lượng đặt hàng, cán bộ quản lý kho sẽ dùng số liệu này để lập phiếu nhập hàng chính thức trên hệ thống WMS/ERP chuyên dụng.", "Ranh giới kết nối: ")

    add_heading_2(doc, "4.3. Không quản lý Vận tải, Logistics & Đội xe Giao nhận (Fleet Management)")
    add_bullet_p(doc, "Hệ thống DSS không lập lộ trình di chuyển cho tài xế giao hàng và không phân bổ đơn hàng lên xe tải.", "Không điều phối xe tải giao hàng: ")
    add_bullet_p(doc, "Hệ thống không tích hợp bản đồ số để theo dõi tọa độ định vị GPS của các xe chở hàng từ nhà máy về kho.", "Không định vị GPS phương tiện giao vận: ")
    add_bullet_p(doc, "Hệ thống không xử lý biên bản ký nhận bàn giao chặng cuối giữa tài xế và cửa hàng.", "Không quản lý ký nhận giao hàng (POD - Proof of Delivery): ")
    add_bullet_p(doc, "DSS chỉ xem Lead Time (số ngày giao hàng của nhà cung cấp) là một hằng số tham số toán học để tính mức tồn kho an toàn.", "Ranh giới kết nối: ")

    add_heading_2(doc, "4.4. Không can thiệp tự động vào Sàn Thương mại Điện tử & Website Bán hàng")
    add_bullet_p(doc, "Hệ thống không cung cấp giao diện Web/Mobile cho khách hàng đặt mua online, chọn giỏ hàng hay thanh toán qua cổng thanh toán.", "Không vận hành giỏ hàng trực tuyến: ")
    add_bullet_p(doc, "Hệ thống DSS không tự động gọi API cập nhật lại giá niêm yết trên website thương mại điện tử (Shopee, Lazada, Tiki, Website nội bộ).", "Không tự động đổi giá bán lẻ trên Internet: ")
    add_bullet_p(doc, "Hệ thống không tự động khởi chạy chiến dịch quảng cáo trên Facebook Ads, Google Ads hay phát voucher khuyến mãi tự động.", "Không trực tiếp chạy chiến dịch Marketing tự động: ")
    add_bullet_p(doc, "DSS chỉ đưa ra khuyến nghị phân bổ tỷ trọng ngân sách marketing cho từng nhóm sản phẩm chiến lược.", "Ranh giới kết nối: ")

    add_heading_2(doc, "4.5. Không hạch toán Kế toán Tài chính & Pháp lý Doanh nghiệp")
    add_bullet_p(doc, "Hệ thống không định khoản tài khoản Nợ/Có theo hệ thống tài khoản kế toán Việt Nam (VAS/IFRS).", "Không ghi nhận bút toán sổ cái kế toán: ")
    add_bullet_p(doc, "Hệ thống không kết nối với cơ quan thuế để phát hành hóa đơn điện tử có mã của cơ quan thuế.", "Không phát hành hóa đơn điện tử VAT: ")
    add_bullet_p(doc, "Hệ thống không quản lý chấm công, tính lương thưởng cho nhân sự bán hàng và không quản lý công nợ người mua.", "Không tính lương & công nợ tài chính: ")
    add_bullet_p(doc, "Mọi thông số doanh thu và lợi nhuận trên DSS mang tính chất tham mưu quản trị kinh doanh, không có giá trị báo cáo tài chính pháp lý.", "Ranh giới kết nối: ")

    add_heading_2(doc, "4.6. Nguyên lý Tác nhân Con người Giám sát Tối cao (Human-in-the-Loop)")
    add_body_p(
        doc,
        "Một triết lý cốt lõi của Hệ thống Thông tin Hỗ trợ Ra Quyết định (DSS) là: 'Hệ thống chỉ hỗ trợ, con người là chủ thể ra quyết định'."
    )
    add_bullet_p(
        doc,
        "Phần mềm DSS Retail Decision Studio 2026 tuyệt đối không tự động phát hành lệnh đặt hàng, không tự động ký hợp đồng mua bán với nhà cung ứng và không tự động trừ ngân sách của công ty.",
        "Không tự động ra quyết định thay con người: "
    )
    add_bullet_p(
        doc,
        "Báo cáo thẩm định và bảng xếp hạng AHP từ DSS là công cụ tham mưu định lượng khoa học 100%. Quyết định rót vốn nhập sản phẩm nào, chấp nhận rủi ro ở mức độ nào hoàn toàn phụ thuộc vào chữ ký phê duyệt cuối cùng của Giám đốc hoặc Trưởng phòng Mua hàng (Decision Maker).",
        "Quyền phê duyệt tối thượng thuộc về Nhà quản trị: "
    )

    # =========================================================================
    # PHẦN 5: BẢNG MA TRẬN PHÂN ĐỊNH RANH GIỚI & GIAO TIẾP HỆ THỐNG
    # =========================================================================
    add_heading_1(doc, "5. BẢNG MA TRẬN PHÂN ĐỊNH RANH GIỚI & CƠ CHẾ GIAO TIẾP (SCOPE BOUNDARY MATRIX)")
    
    add_body_p(
        doc,
        "Bảng ma trận đối chiếu dưới đây làm rõ trách nhiệm cụ thể của Hệ thống DSS và các hệ thống tác nghiệp bên ngoài "
        "trên toàn bộ 10 nghiệp vụ kinh doanh trọng yếu của doanh nghiệp bán lẻ:"
    )
    
    matrix_headers = ["STT", "Nghiệp vụ / Chức năng", "Trách nhiệm IN-SCOPE (DSS thực hiện)", "Phạm vi OUT-OF-SCOPE (Hệ thống ngoài)", "Ranh giới Giao tiếp & Cơ chế"]
    matrix_data = [
        [
            "1",
            "Xác thực & Bảo mật người dùng",
            "Mã hóa băm SHA-256; quản lý phiên UserSession 4 vai trò; panel Fail-Safe pnlDbOverlay.",
            "Không quản lý tài khoản nhân viên của toàn bộ hệ thống ERP tổng; không tích hợp Single Sign-On LDAP.",
            "Xác thực độc lập trên bảng USERS của CSDL DSS_SalesDB."
        ],
        [
            "2",
            "Thu thập dữ liệu bán hàng",
            "Truy vấn dữ liệu từ bảng ORDERS, ORDER_DETAILS; làm sạch và kiểm tra toàn vẹn dữ liệu.",
            "Không thu ngân tại quầy; không quét mã vạch sản phẩm; không in biên lai tính tiền.",
            "Đọc dữ liệu (Read-Only) từ CSDL quan hệ Microsoft SQL Server 2022."
        ],
        [
            "3",
            "Phân tích đa chiều dữ liệu",
            "Xử lý OLAP 4 chiều: Slice, Dice, Roll-up, Drill-down theo Chi nhánh, Ngành hàng và Thời gian.",
            "Không xây dựng Data Warehouse quy mô lớn đa máy chủ; không dùng công cụ BI thương mại đám mây.",
            "Thực thi trực tiếp thông qua truy vấn SQL nâng cao trên SQL Server Client."
        ],
        [
            "4",
            "Đánh giá & Xếp hạng sản phẩm",
            "Xây dựng ma trận Saaty 1-9 cho 5 tiêu chí; kiểm tra CR <= 0.10; chuẩn hóa Benefit/Cost; xếp hạng AHP.",
            "Không cho phép quyết định cảm tính không qua kiểm định CR; không tự động thay đổi giá vốn gốc.",
            "Lõi thuật toán DssEngine viết bằng C# .NET 10.0 và Python DSS Pipeline."
        ],
        [
            "5",
            "Khuyến nghị số lượng đặt hàng",
            "Tự động tính số lượng gợi ý đặt hàng dựa trên Lead Time, chu kỳ 30 ngày và tồn kho hiện tại.",
            "Không tự động gửi đơn đặt hàng (PO) đến nhà cung cấp; không ký chứng từ thương mại.",
            "Xuất ra cột Suggested_Order_Qty trên bảng kết quả để người dùng tham khảo."
        ],
        [
            "6",
            "Mô phỏng kịch bản What-If",
            "Cung cấp 3 kịch bản thị trường định sẵn; thanh trượt tùy biến tham số Nhu cầu, Lợi nhuận, Rủi ro.",
            "Không dự báo thời tiết, vĩ mô bằng AI phức tạp; không can thiệp vào biến động thị trường thực tế.",
            "Tái tính toán tức thời trên bộ nhớ RAM và vẽ đồ thị Delta so sánh."
        ],
        [
            "7",
            "Quản lý danh mục SKU",
            "Cho phép thêm SKU mới trực tiếp trên Tab 4; tự động tái tính toán lại toàn bộ xếp hạng AHP.",
            "Không quản lý vòng đời sản phẩm chi tiết (PLM); không quản lý thông số bảo hành từng máy.",
            "Cập nhật vào bảng PRODUCTS của database và kích hoạt tính toán động."
        ],
        [
            "8",
            "Quản lý tồn kho vật lý",
            "Đọc mức tồn kho thô (stock_quantity) để đánh giá tiêu chí Rủi ro tồn kho C4 và số ngày tồn.",
            "Không trừ kho vật lý tự động; không in tem RFID; không điều phối xe nâng; không kiểm kê kho.",
            "Chỉ tiếp nhận số liệu tồn kho đầu vào từ bảng PRODUCTS."
        ],
        [
            "9",
            "Quản lý chiến dịch tiếp thị",
            "Đề xuất tỷ trọng phân bổ ngân sách marketing tối ưu giữa các sản phẩm được phê duyệt.",
            "Không chạy quảng cáo Facebook/Google; không phát mã coupon; không nhắn tin SMS chăm sóc khách.",
            "Đưa ra khuyến nghị định lượng trong Báo cáo thẩm định."
        ],
        [
            "10",
            "Báo cáo & Phê duyệt",
            "Tổng hợp Dashboard KPI; sinh Executive Summary tự động; xuất báo cáo thẩm định Word (.docx)/HTML.",
            "Hệ thống không tự động phê duyệt; quyền quyết định cuối cùng thuộc về Giám đốc doanh nghiệp.",
            "File báo cáo Word được lưu trữ cục bộ phục vụ họp giao ban thẩm định."
        ]
    ]
    
    create_styled_table(doc, matrix_headers, matrix_data, col_widths=[0.4, 1.2, 2.0, 1.8, 1.3])

    # =========================================================================
    # PHẦN 6: KẾ HOẠCH QUẢN LÝ THAY ĐỔI & TIÊU CHÍ CHẤP THUẬN
    # =========================================================================
    add_heading_1(doc, "6. KẾ HOẠCH QUẢN LÝ THAY ĐỔI PHẠM VI & TIÊU CHÍ CHẤP THUẬN")
    
    add_heading_2(doc, "6.1. Quy trình quản lý yêu cầu thay đổi (Scope Change Management)")
    add_body_p(
        doc,
        "Để bảo vệ dự án khỏi các tác động tiêu cực của việc thay đổi yêu cầu liên tục, Nhóm 05 áp dụng quy trình "
        "quản lý thay đổi phạm vi nghiêm ngặt gồm 4 bước:"
    )
    add_bullet_p(doc, "Mọi yêu cầu thêm tính năng mới phải được gửi bằng văn bản, nêu rõ lý do kinh doanh và nhóm chức năng bị ảnh hưởng.", "Bước 1: Tiếp nhận yêu cầu: ")
    add_bullet_p(doc, "Trưởng nhóm và chuyên viên kỹ thuật tiến hành đánh giá tác động lên CSDL, thuật toán AHP và tiến độ hoàn thành.", "Bước 2: Phân tích tác động: ")
    add_bullet_p(doc, "Nếu tính năng thuộc danh mục Out-of-Scope (ví dụ: đòi hỏi in hóa đơn POS hay trừ kho tự động), yêu cầu sẽ bị từ chối chuyển sang giai đoạn phát triển tương lai.", "Bước 3: Phê duyệt ranh giới: ")
    add_bullet_p(doc, "Nếu yêu cầu được Giảng viên hướng dẫn chấp thuận, cập nhật lại tài liệu Đặc tả phạm vi này và thông báo đến toàn đội ngũ.", "Bước 4: Cập nhật tài liệu: ")

    add_heading_2(doc, "6.2. Tiêu chí nghiệm thu và chấp thuận (Acceptance Criteria)")
    add_body_p(
        doc,
        "Hệ thống DSS Retail Decision Studio 2026 được coi là hoàn thành đúng phạm vi và đạt tiêu chuẩn nghiệm thu "
        "khi đáp ứng trọn vẹn 100% các tiêu chuẩn định lượng sau:"
    )
    
    acc_headers = ["Mã kiểm thử", "Hạng mục nghiệm thu", "Tiêu chuẩn chấp thuận đạt (Pass Criteria)", "Kết quả kiểm tra"]
    acc_data = [
        ["AC-01", "Bảo mật & Đăng nhập", "Mật khẩu băm SHA-256 đối chiếu chính xác bảng USERS; gán đúng vai trò UserSession.", "ĐẠT (PASS)"],
        ["AC-02", "Cơ chế Fail-Safe", "Khi ngắt dịch vụ SQL Server, panel pnlDbOverlay che phủ và khóa cứng 6 Tab trong vòng < 1 giây.", "ĐẠT (PASS)"],
        ["AC-03", "Ma trận Saaty Tab 1", "Nhập ma trận 5 tiêu chí; tính đúng lambda max, CI; hiển thị cảnh báo đỏ và khóa nút Chạy nếu CR > 0.10.", "ĐẠT (PASS)"],
        ["AC-04", "Chuẩn hóa & Xếp hạng Tab 2", "Áp dụng chuẩn xác hàm Benefit (C1,C2,C3) và hàm Cost (C4,C5); phân đúng 3 nhóm Selected/Review/Reject.", "ĐẠT (PASS)"],
        ["AC-05", "Khuyến nghị số lượng đặt", "Công thức số lượng đặt hàng gợi ý tính toán chính xác dựa trên Lead Time và tồn kho thực tế.", "ĐẠT (PASS)"],
        ["AC-06", "Mô phỏng What-If Tab 3", "3 Kịch bản hoạt động trơn tru; thanh trượt tùy biến cập nhật tức thời đồ thị Delta và Cây AHP.", "ĐẠT (PASS)"],
        ["AC-07", "Thêm SKU Động Tab 4", "Nhập SKU mới, lưu CSDL và tự động tái tính toán xếp hạng AHP toàn bộ danh mục thành công.", "ĐẠT (PASS)"],
        ["AC-08", "Phân tích OLAP Tab 5", "Thực hiện đầy đủ 4 thao tác Slice, Dice, Roll-up, Drill-down trên dữ liệu bán lẻ chính xác.", "ĐẠT (PASS)"],
        ["AC-09", "Dashboard & Báo cáo Tab 6", "Hiển thị đúng 4 KPI cards; sinh Executive Summary tự động; xuất thành công file Word (.docx) và HTML.", "ĐẠT (PASS)"],
        ["AC-10", "Cam kết Out-of-Scope", "Phần mềm không chứa các đoạn code can thiệp trái phép vào máy POS, kho vật lý WMS hay sàn TMĐT.", "ĐẠT (PASS)"]
    ]
    create_styled_table(doc, acc_headers, acc_data, col_widths=[0.9, 1.8, 3.0, 1.0])

    # =========================================================================
    # PHẦN 7: KẾT LUẬN & CHỮ KÝ PHÊ DUYỆT
    # =========================================================================
    add_heading_1(doc, "7. KẾT LUẬN & XÁC NHẬN CỦA CÁC BÊN LIÊN QUAN")
    add_body_p(
        doc,
        "Tài liệu Đặc tả Phạm vi Hệ thống (System Scope Specification) đã phân định rõ ràng, khách quan và minh bạch "
        "nội hàm của dự án DSS Retail Decision Studio 2026. Bằng việc bám sát tuyệt đối các phân hệ In-Scope và kiên quyết "
        "nói không với các tính năng Out-of-Scope, nhóm sinh viên đảm bảo hoàn thành một sản phẩm công nghệ phần mềm "
        "chất lượng cao, đậm tính học thuật chuyên sâu của học phần Hệ Thống Thông Tin Hỗ Trợ Ra Quyết Định (IS 402 A), "
        "đáp ứng xuất sắc mọi yêu cầu của Nhà trường và Giảng viên hướng dẫn."
    )
    
    # Signature Box Table
    tbl_sig = doc.add_table(rows=2, cols=2)
    tbl_sig.alignment = WD_TABLE_ALIGNMENT.CENTER
    set_table_borders(tbl_sig, color="FFFFFF", sz="0") # invisible borders
    
    cell_s1 = tbl_sig.rows[0].cells[0]
    p_s1 = cell_s1.paragraphs[0]
    p_s1.alignment = WD_ALIGN_PARAGRAPH.CENTER
    r_s1 = p_s1.add_run("ĐẠI DIỆN NHÓM SINH VIÊN THỰC HIỆN\nTrưởng nhóm\n\n\n\n(Ký và ghi rõ họ tên)\n")
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
    r_s2 = p_s2.add_run("GIẢNG VIÊN HƯỚNG DẪN DUYỆT\nKhoa Hệ Thống Thông Tin Quản Lý\n\n\n\n(Ký và ghi rõ họ tên)\n")
    r_s2.font.name = "Segoe UI"
    r_s2.font.bold = True
    r_s2_name = p_s2.add_run("ThS. Nguyễn Quang Ánh")
    r_s2_name.font.name = "Segoe UI"
    r_s2_name.font.bold = True
    r_s2_name.font.color.rgb = RGBColor(15, 23, 42)
    cell_s2.width = Inches(3.3)

    output_docx = "docs/TAI_LIEU_PHAM_VI_HE_THONG_DSS_IS402_NHOM05.docx"
    os.makedirs(os.path.dirname(output_docx), exist_ok=True)
    doc.save(output_docx)
    print(f"File Word da duoc luu thanh cong tai: {output_docx}")

if __name__ == "__main__":
    build_scope_document()
