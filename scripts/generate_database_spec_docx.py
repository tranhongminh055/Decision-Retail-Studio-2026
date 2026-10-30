# -*- coding: utf-8 -*-
"""
Script tong hop va sinh Tai lieu Thiet ke va Dac ta Co so du lieu (Database Specification Document)
File Word: docs/TAI_LIEU_THIET_KE_DAC_TA_DATABASE_DSS_IS402_NHOM05.docx
File Markdown: docs/TAI_LIEU_THIET_KE_DAC_TA_DATABASE_DSS_IS402_NHOM05.md
Co so du lieu: DSS_SalesDB (Microsoft SQL Server 2022 / 2025 - 12 Bang Dat Chuan 3NF)
Hoc phan: IS 402 A - Nhom 05 - Duy Tan University
"""

import os
import sys
import docx
from docx import Document
from docx.shared import Inches, Pt, RGBColor
from docx.enum.text import WD_ALIGN_PARAGRAPH
from docx.enum.table import WD_TABLE_ALIGNMENT, WD_ALIGN_VERTICAL

# Add scripts directory to path
sys.path.insert(0, os.path.dirname(os.path.abspath(__file__)))

from docx_helpers import set_cell_background, set_cell_margins, set_table_borders, add_callout
from docx_builder_base import (
    create_document, add_title, add_heading_1, add_heading_2, add_heading_3,
    add_body_p, add_bullet_p, add_image_figure, create_styled_table
)

def build_database_spec_docx():
    print("Khoi tao Document Dac ta Co so du lieu DSS_SalesDB...")
    doc = create_document()
    
    # Custom Header & Footer
    header = doc.sections[0].header
    hp = header.paragraphs[0]
    hp.text = ""
    hp.alignment = WD_ALIGN_PARAGRAPH.LEFT
    hr = hp.add_run("TÀI LIỆU THIẾT KẾ & ĐẶC TẢ CƠ SỞ DỮ LIỆU CHUẨN 3NF (DATABASE SPECIFICATION - DSS_SALESDB)")
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
    r_t = p_title.add_run("BÁO CÁO THIẾT KẾ & ĐẶC TẢ CƠ SỞ DỮ LIỆU\nCHUẨN HÓA 3NF VÀ TỪ ĐIỂN DỮ LIỆU\n(RELATIONAL DATABASE SPECIFICATION & DATA DICTIONARY)\n")
    r_t.font.name = "Segoe UI"
    r_t.font.size = Pt(17)
    r_t.font.bold = True
    r_t.font.color.rgb = RGBColor(15, 23, 42)
    
    p_sub = doc.add_paragraph()
    p_sub.alignment = WD_ALIGN_PARAGRAPH.CENTER
    p_sub.paragraph_format.space_after = Pt(25)
    r_s = p_sub.add_run("CƠ SỞ DỮ LIỆU: DSS_SalesDB (Microsoft SQL Server 2022 / 2025 – 12 Bảng Thực Thể)\n")
    r_s.font.name = "Segoe UI"
    r_s.font.size = Pt(11.5)
    r_s.font.bold = True
    r_s.font.color.rgb = RGBColor(16, 185, 129)
    
    r_s2 = p_sub.add_run("Đề tài: Hệ Thống Hỗ Trợ Ra Quyết Định Chiến Lược Khuyến Mãi & Phân Bổ Hàng Hóa Quý 4/2026")
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
    add_heading_1(doc, "MỤC LỤC VÀ DANH MỤC TÀI LIỆU CƠ SỞ DỮ LIỆU")
    
    add_body_p(doc, "Tài liệu này là hồ sơ thiết kế kỹ thuật chính thức và toàn diện về Cơ sở dữ liệu quan hệ DSS_SalesDB trên hệ quản trị Microsoft SQL Server. Tài liệu cung cấp sơ đồ ngữ cảnh SCD, sơ đồ luồng dữ liệu SDD, sơ đồ thực thể mối quan hệ ERD chuẩn 3NF, từ điển dữ liệu chi tiết 100% thuộc tính của 12 bảng thực thể, các câu lệnh truy vấn phân tích đa chiều OLAP (ROLLUP, CUBE) và quy trình chuẩn hóa dữ liệu bảo vệ tính toàn vẹn hệ thống.")
    
    add_heading_2(doc, "Danh Mục Các Bản Vẽ Sơ Đồ Kiến Trúc Dữ Liệu")
    diag_list = [
        ("Hình 1", "Sơ đồ Ngữ cảnh Hệ thống (SCD - System Context Diagram / DFD Mức 0)", "Mục 1.3"),
        ("Hình 2", "Sơ đồ Phân rã Luồng Dữ liệu (SDD - System Dataflow Diagram / DFD Mức 1)", "Mục 1.4"),
        ("Hình 3", "Sơ đồ Thực thể Mối quan hệ Chuẩn 3NF (ERD - Entity Relationship Diagram)", "Mục 2.2")
    ]
    create_styled_table(doc, ["Mã Hình", "Tên Bản Vẽ Sơ Đồ Thiết Kế Dữ Liệu", "Vị Trí"], diag_list, [Inches(1.0), Inches(4.5), Inches(1.0)])

    add_heading_2(doc, "Danh Mục 12 Bảng Thực Thể Cơ Sở Dữ Liệu DSS_SalesDB")
    tbl_list = [
        ("1", "dbo.USERS", "Phân hệ Vận hành (OLTP)", "Quản lý người dùng, phân quyền RBAC (4 vai trò), băm mật khẩu SHA-256."),
        ("2", "dbo.CATEGORIES", "Phân hệ Vận hành (OLTP)", "Danh mục ngành hàng bán lẻ (Điện thoại, Laptop, Phụ kiện, Smart Home)."),
        ("3", "dbo.PRODUCTS", "Phân hệ Vận hành (OLTP)", "Danh mục hàng hóa, giá vốn, giá bán niêm yết, số lượng tồn kho, lead time."),
        ("4", "dbo.CUSTOMERS", "Phân hệ Vận hành (OLTP)", "Khách hàng, phân khúc thị trường (Retail, Wholesale, VIP), tổng chi tiêu."),
        ("5", "dbo.ORDERS", "Phân hệ Vận hành (OLTP)", "Hóa đơn bán lẻ, ngày đặt hàng, trạng thái xử lý, tổng tiền, chiết khấu."),
        ("6", "dbo.ORDER_DETAILS", "Phân hệ Vận hành (OLTP)", "Chi tiết đơn hàng: số lượng bán, đơn giá bán, cột tính toán line_total."),
        ("7", "dbo.DECISION_SESSIONS", "Phân hệ Ra quyết định (DSS)", "Phiên ra quyết định AHP, tỷ số nhất quán CR, trạng thái phê duyệt."),
        ("8", "dbo.CRITERIA", "Phân hệ Ra quyết định (DSS)", "Cây tiêu chí đánh giá Saaty 5 tiêu chí (C1-C5), loại Benefit/Cost."),
        ("9", "dbo.ALTERNATIVES", "Phân hệ Ra quyết định (DSS)", "Danh mục 9 dòng sản phẩm ứng viên đưa vào phiên thẩm định DSS."),
        ("10", "dbo.PAIRWISE_COMPARISONS", "Phân hệ Ra quyết định (DSS)", "Ma trận so sánh cặp Saaty 1-9 (lưu dạng dọc nguyên tử a_ij và nghịch đảo)."),
        ("11", "dbo.ALTERNATIVE_SCORES", "Phân hệ Ra quyết định (DSS)", "Điểm đánh giá thô (raw_value) và điểm số chuẩn hóa (normalized_score)."),
        ("12", "dbo.DECISION_RESULTS", "Phân hệ Ra quyết định (DSS)", "Điểm tổng hợp AHP, thứ hạng (DENSE_RANK) và nhãn quyết định Selected/Reject.")
    ]
    create_styled_table(doc, ["STT", "Tên Bảng (Table)", "Phân Hệ Trực Thuộc", "Ý Nghĩa Nghiệp Vụ & Đóng Góp Vào Mô Hình DSS"], tbl_list, [Inches(0.4), Inches(1.8), Inches(1.8), Inches(2.5)])

    doc.add_page_break()

    # =========================================================================
    # PHẦN 1: TỔNG QUAN THIẾT KẾ CƠ SỞ DỮ LIỆU & KIẾN TRÚC DỮ LIỆU
    # =========================================================================
    add_heading_1(doc, "PHẦN 1: TỔNG QUAN THIẾT KẾ CƠ SỞ DỮ LIỆU & KIẾN TRÚC DỮ LIỆU")

    add_heading_2(doc, "1.1. Mục Tiêu Và Phạm Vi Thiết Kế CSDL DSS_SalesDB")
    add_body_p(doc, "Cơ sở dữ liệu DSS_SalesDB được thiết kế để giải quyết bài toán cốt lõi của doanh nghiệp bán lẻ: 'Lựa chọn Danh mục Sản phẩm Chiến lược để Phân bổ Ngân sách Nhập hàng & Tiếp thị Quý 4/2026'. CSDL đóng vai trò là nền tảng quản trị dữ liệu (Data Management Subsystem) theo mô hình kiến trúc DSS kinh điển của Efraim Turban.")
    add_body_p(doc, "Các tiêu chí thiết kế kỹ thuật bắt buộc:")
    add_bullet_p(doc, "Đạt chuẩn hóa 3NF (Third Normal Form) 100% trên toàn bộ 12 bảng, triệt tiêu hoàn toàn dư thừa dữ liệu và dị thường thao tác.", "• Tính toàn vẹn chuẩn hóa: ");
    add_bullet_p(doc, "Tích hợp liền mạch giữa dữ liệu vận hành giao dịch bán lẻ (OLTP) và dữ liệu tính toán mô hình toán học đa tiêu chí AHP.", "• Kiến trúc 2 phân hệ hữu cơ: ");
    add_bullet_p(doc, "Cung cấp các góc nhìn đa chiều thông qua Views và các khối phân tích OLAP (ROLLUP và CUBE) phục vụ lãnh đạo ra quyết định.", "• Sẵn sàng cho phân tích đa chiều: ");
    add_bullet_p(doc, "Kiểm soát an toàn dữ liệu thông qua hệ thống ràng buộc khóa ngoại, CHECK constraints, chỉ mục tối ưu hóa và cơ chế Fail-Safe.", "• Bảo mật & An toàn vận hành: ");

    add_heading_2(doc, "1.2. Kiến Trúc Dữ Liệu Tích Hợp Hai Phân Hệ")
    add_body_p(doc, "Hệ thống CSDL gồm 12 bảng được phân bổ hài hòa thành 2 phân hệ:")
    add_bullet_p(doc, "Bao gồm 6 bảng (USERS, CATEGORIES, PRODUCTS, CUSTOMERS, ORDERS, ORDER_DETAILS). Đảm nhiệm thu thập dữ liệu bán lẻ thực tế, lưu vết lịch sử hóa đơn, theo dõi biến động tồn kho và tính toán doanh thu thực thu, biên lợi nhuận gộp thực tế của từng mã hàng.", "1. Phân hệ Vận hành Bán lẻ (OLTP Module): ");
    add_bullet_p(doc, "Bao gồm 6 bảng (DECISION_SESSIONS, CRITERIA, ALTERNATIVES, PAIRWISE_COMPARISONS, ALTERNATIVE_SCORES, DECISION_RESULTS). Đảm nhiệm quản lý phiên thẩm định, thiết lập ma trận so sánh cặp Saaty, lưu trữ điểm chuẩn hóa Benefit/Cost và bảng xếp hạng quyết định cuối cùng.", "2. Phân hệ Hỗ trợ Ra Quyết Định AHP (DSS Module): ");

    add_heading_2(doc, "1.3. Mô Hình Ngữ Cảnh Dữ Liệu (System Context Diagram - SCD / DFD Mức 0)")
    add_body_p(doc, "Sơ đồ ngữ cảnh mô tả luồng trao đổi thông tin giữa Cơ sở dữ liệu trung tâm DSS_SalesDB với các tác nhân và hệ thống bên ngoài:")
    
    add_image_figure(doc, "docs/images/sodo_ngu_canh_scd.png", "Hình 1: Sơ đồ Ngữ cảnh Hệ thống (SCD - System Context Diagram / DFD Mức 0)")
    
    add_body_p(doc, "• Luồng dữ liệu chính qua CSDL:")
    add_bullet_p(doc, "Từ POS Bán Lẻ nạp vào bảng ORDERS và ORDER_DETAILS.", "• Luồng nạp lịch sử giao dịch: ");
    add_bullet_p(doc, "Từ Kho vật lý WMS cập nhật vào trường stock_quantity và lead_time_days của bảng PRODUCTS.", "• Luồng cập nhật tồn kho: ");
    add_bullet_p(doc, "Từ Chuyên viên phân tích thiết lập ma trận vào PAIRWISE_COMPARISONS và tham số kịch bản What-If.", "• Luồng tham số AHP: ");
    add_bullet_p(doc, "Từ CSDL trích xuất sang bảng DECISION_RESULTS để Ban Giám Đốc phê duyệt kế hoạch nhập hàng.", "• Luồng khuyến nghị tham mưu: ");

    add_heading_2(doc, "1.4. Mô Hình Phân Rã Luồng Dữ Liệu (System Dataflow Diagram - SDD / DFD Mức 1)")
    add_body_p(doc, "Sơ đồ luồng dữ liệu mức 1 phân rã toàn bộ kiến trúc xử lý thành 5 tiến trình cốt lõi tương tác với 2 kho dữ liệu:")
    
    add_image_figure(doc, "docs/images/sodo_luong_du_lieu_sdd.png", "Hình 2: Sơ đồ Phân rã Luồng Dữ liệu CSDL (SDD - System Dataflow Diagram / DFD Mức 1)")
    
    add_body_p(doc, "• Hai kho dữ liệu chuẩn hóa trong SQL Server:")
    add_bullet_p(doc, "Kho D1 (CSDL Bán Lẻ): Lưu trữ ORDERS, ORDER_DETAILS, PRODUCTS, CATEGORIES, CUSTOMERS.", "• Kho D1: ");
    add_bullet_p(doc, "Kho D2 (CSDL Quyết Định DSS): Lưu trữ CRITERIA, PAIRWISE_COMPARISONS, ALTERNATIVE_SCORES, DECISION_RESULTS, DECISION_SESSIONS.", "• Kho D2: ");

    doc.add_page_break()

    # =========================================================================
    # PHẦN 2: MÔ HÌNH THỰC THỂ MỐI QUAN HỆ (ERD)
    # =========================================================================
    add_heading_1(doc, "PHẦN 2: MÔ HÌNH THỰC THỂ MỐI QUAN HỆ (ERD - ENTITY RELATIONSHIP DIAGRAM)")

    add_heading_2(doc, "2.1. Sơ Đồ Thực Thể Mối Quan Hệ ERD Chuẩn 3NF")
    add_body_p(doc, "Sơ đồ ERD trực quan dưới đây thể hiện đầy đủ 12 bảng thực thể, khóa chính [PK], khóa ngoại [FK], ràng buộc duy nhất [UQ] và các mối liên kết bản số:")
    
    add_image_figure(doc, "docs/images/sodo_thuc_the_erd.png", "Hình 3: Sơ đồ Thực thể Mối quan hệ Chuẩn 3NF (ERD — DSS_SalesDB)")

    add_heading_2(doc, "2.2. Ma Trận Mối Quan Hệ & Bản Số (Cardinality Matrix)")
    add_body_p(doc, "Bảng ma trận đối soát bản số liên kết giữa các thực thể trong cơ sở dữ liệu:")
    
    card_data = [
        ("CATEGORIES ➔ PRODUCTS", "1 : N (Một - Nhiều)", "Một ngành hàng chứa nhiều sản phẩm; mỗi sản phẩm chỉ thuộc một ngành hàng duy nhất.", "ON DELETE NO ACTION"),
        ("PRODUCTS ➔ ORDER_DETAILS", "1 : N (Một - Nhiều)", "Một sản phẩm xuất hiện trong nhiều dòng hóa đơn; mỗi dòng hóa đơn trỏ về 1 sản phẩm.", "ON DELETE NO ACTION"),
        ("CUSTOMERS ➔ ORDERS", "1 : N (Một - Nhiều)", "Một khách hàng thực hiện nhiều đơn mua; mỗi đơn hàng thuộc về một khách hàng.", "ON DELETE NO ACTION"),
        ("ORDERS ➔ ORDER_DETAILS", "1 : N (Một - Nhiều)", "Một hóa đơn gồm nhiều dòng chi tiết; dòng chi tiết bị xóa nếu xóa đơn hàng.", "ON DELETE CASCADE"),
        ("USERS ➔ ORDERS", "1 : N (Một - Nhiều)", "Một nhân viên thu ngân lập nhiều hóa đơn bán hàng.", "ON DELETE NO ACTION"),
        ("USERS ➔ DECISION_SESSIONS", "1 : N (Một - Nhiều)", "Một DecisionMaker chủ trì nhiều phiên thẩm định DSS (trường created_by).", "ON DELETE NO ACTION"),
        ("PRODUCTS ➔ ALTERNATIVES", "1 : N (Một - Nhiều)", "Cầu nối giữa OLTP và DSS: Một sản phẩm thực tế được ánh xạ làm phương án ứng viên trong các phiên DSS.", "ON DELETE NO ACTION"),
        ("DECISION_SESSIONS ➔ ALTERNATIVES", "1 : N (Một - Nhiều)", "Một phiên ra quyết định đánh giá nhiều phương án ứng viên (mặc định 9 SKU).", "ON DELETE CASCADE"),
        ("DECISION_SESSIONS ➔ PAIRWISE_COMPARISONS", "1 : N (Một - Nhiều)", "Một phiên ra quyết định sở hữu một ma trận so sánh cặp Saaty 5x5.", "ON DELETE CASCADE"),
        ("CRITERIA ➔ PAIRWISE_COMPARISONS", "1 : N (Một - Nhiều x2)", "Mỗi cặp so sánh liên kết với 2 tiêu chí đánh giá (criterion_id_1 và criterion_id_2).", "ON DELETE NO ACTION"),
        ("ALTERNATIVES ➔ ALTERNATIVE_SCORES", "1 : N (Một - Nhiều)", "Một phương án có 5 bản ghi điểm số ứng với 5 tiêu chí đánh giá.", "ON DELETE CASCADE"),
        ("ALTERNATIVES ➔ DECISION_RESULTS", "1 : 1 (Một - Một)", "Mỗi phương án trong phiên chỉ có duy nhất một kết quả xếp hạng và khuyến nghị chung cuộc.", "ON DELETE CASCADE")
    ]
    create_styled_table(doc, ["Cặp Thực Thể Liên Kết", "Bản Số (Cardinality)", "Diễn Giải Nghiệp Vụ Bán Lẻ & Hỗ Trợ Ra Quyết Định", "Quy Tắc Toàn Vẹn Tham Chiếu"], card_data, [Inches(1.8), Inches(1.1), Inches(2.5), Inches(1.1)])

    doc.add_page_break()

    # =========================================================================
    # PHẦN 3: TỪ ĐIỂN DỮ LIỆU CHI TIẾT 12 BẢNG THỰC THỂ (DATA DICTIONARY)
    # =========================================================================
    add_heading_1(doc, "PHẦN 3: TỪ ĐIỂN DỮ LIỆU CHI TIẾT 12 BẢNG THỰC THỂ (DATA DICTIONARY)")
    add_body_p(doc, "Dưới đây là đặc tả kỹ thuật 100% cấu trúc các trường dữ liệu, kiểu dữ liệu, thuộc tính NULL, khóa và ràng buộc nghiệp vụ của 12 bảng trong CSDL DSS_SalesDB:")

    # 3.1. USERS
    add_heading_2(doc, "3.1. Bảng dbo.USERS (Quản Trị Người Dùng & Phân Quyền RBAC)")
    add_body_p(doc, "Quản lý tài khoản đăng nhập hệ thống, kiểm soát truy cập theo 4 vai trò RBAC và lưu trữ mật khẩu băm SHA-256 an toàn.")
    t_users = [
        ("user_id", "INT", "NO", "PK", "IDENTITY(1,1)", "Mã định danh duy nhất của người dùng."),
        ("username", "VARCHAR(50)", "NO", "UQ", "NOT NULL", "Tên tài khoản đăng nhập (chống trùng lặp)."),
        ("password_hash", "VARCHAR(255)", "NO", "", "NOT NULL", "Mật khẩu đã băm bằng mã hóa SHA-256."),
        ("full_name", "NVARCHAR(100)", "NO", "", "NOT NULL", "Họ và tên đầy đủ của cán bộ."),
        ("email", "VARCHAR(100)", "NO", "UQ", "NOT NULL", "Thư điện tử doanh nghiệp (regex chuẩn)."),
        ("phone_number", "VARCHAR(20)", "YES", "", "NULL", "Số điện thoại liên lạc cá nhân."),
        ("role", "VARCHAR(20)", "NO", "", "CHECK in ('Admin', 'DecisionMaker', 'SalesManager', 'SalesStaff')", "Vai trò phân quyền truy cập hệ thống."),
        ("status", "BIT", "NO", "", "DEFAULT 1", "Trạng thái (1: Đang hoạt động, 0: Bị khóa)."),
        ("created_at", "DATETIME2", "NO", "", "DEFAULT SYSDATETIME()", "Thời điểm tạo tài khoản.")
    ]
    create_styled_table(doc, ["Tên Cột", "Kiểu Dữ Liệu", "NULL", "Khóa", "Mặc Định / Ràng Buộc", "Diễn Giải Nghiệp Vụ"], t_users, [Inches(1.2), Inches(1.1), Inches(0.5), Inches(0.5), Inches(1.8), Inches(1.4)])

    # 3.2. CATEGORIES
    add_heading_2(doc, "3.2. Bảng dbo.CATEGORIES (Danh Mục Ngành Hàng Bán Lẻ)")
    add_body_p(doc, "Quản lý 4 ngành hàng kinh doanh chính: Smartphone, Laptop, Wearable và Phụ kiện công nghệ.")
    t_cat = [
        ("category_id", "INT", "NO", "PK", "IDENTITY(1,1)", "Mã định danh ngành hàng."),
        ("category_code", "VARCHAR(30)", "NO", "UQ", "NOT NULL", "Mã viết tắt (ví dụ: CAT_LAPTOP, CAT_PHONE)."),
        ("category_name", "NVARCHAR(100)", "NO", "", "NOT NULL", "Tên hiển thị ngành hàng kinh doanh."),
        ("description", "NVARCHAR(500)", "YES", "", "NULL", "Mô tả đặc tính và phạm vi ngành hàng."),
        ("status", "BIT", "NO", "", "DEFAULT 1", "Trạng thái (1: Đang kinh doanh, 0: Ngừng).")
    ]
    create_styled_table(doc, ["Tên Cột", "Kiểu Dữ Liệu", "NULL", "Khóa", "Mặc Định / Ràng Buộc", "Diễn Giải Nghiệp Vụ"], t_cat, [Inches(1.2), Inches(1.1), Inches(0.5), Inches(0.5), Inches(1.8), Inches(1.4)])

    # 3.3. PRODUCTS
    add_heading_2(doc, "3.3. Bảng dbo.PRODUCTS (Danh Mục Hàng Hóa & Tồn Kho)")
    add_body_p(doc, "Lưu trữ dữ liệu hàng hóa, giá vốn, giá bán, tồn kho thực tế. Đóng vai trò là tập ứng viên nguồn cho bài toán DSS.")
    t_prod = [
        ("product_id", "INT", "NO", "PK", "IDENTITY(1,1)", "Mã định danh sản phẩm."),
        ("category_id", "INT", "NO", "FK", "REFERENCES CATEGORIES(category_id)", "Khóa ngoại trỏ về ngành hàng."),
        ("product_code", "VARCHAR(50)", "NO", "UQ", "NOT NULL", "Mã SKU quản lý kho (ví dụ: PRD_ROG_G16)."),
        ("product_name", "NVARCHAR(200)", "NO", "", "NOT NULL", "Tên thương mại đầy đủ của thiết bị."),
        ("cost_price", "DECIMAL(18,2)", "NO", "", "CHECK (cost_price >= 0)", "Giá vốn nhập khẩu đơn vị (VNĐ)."),
        ("selling_price", "DECIMAL(18,2)", "NO", "", "CHECK (selling_price >= cost_price)", "Giá bán niêm yết (bắt buộc ≥ Giá vốn)."),
        ("stock_quantity", "INT", "NO", "", "DEFAULT 0, CHECK (stock_quantity >= 0)", "Số lượng hàng thực tế trong kho."),
        ("min_stock_level", "INT", "NO", "", "DEFAULT 10", "Ngưỡng cảnh báo tồn kho an toàn tối thiểu."),
        ("lead_time_days", "INT", "NO", "", "DEFAULT 7", "Thời gian đặt hàng từ nhà sản xuất (ngày)."),
        ("is_active", "BIT", "NO", "", "DEFAULT 1", "Trạng thái (1: Đang bán, 0: Ngừng kinh doanh)."),
        ("created_at", "DATETIME2", "NO", "", "DEFAULT SYSDATETIME()", "Thời điểm thêm sản phẩm vào hệ thống.")
    ]
    create_styled_table(doc, ["Tên Cột", "Kiểu Dữ Liệu", "NULL", "Khóa", "Mặc Định / Ràng Buộc", "Diễn Giải Nghiệp Vụ"], t_prod, [Inches(1.2), Inches(1.1), Inches(0.5), Inches(0.5), Inches(1.8), Inches(1.4)])

    # 3.4. CUSTOMERS
    add_heading_2(doc, "3.4. Bảng dbo.CUSTOMERS (Khách Hàng & Phân Khúc Thị Trường)")
    add_body_p(doc, "Lưu trữ thông tin khách hàng và phân khúc thị trường phục vụ các truy vấn khối đa chiều OLAP CUBE.")
    t_cust = [
        ("customer_id", "INT", "NO", "PK", "IDENTITY(1,1)", "Mã định danh khách hàng."),
        ("customer_code", "VARCHAR(30)", "NO", "UQ", "NOT NULL", "Mã khách hàng (ví dụ: CUST001)."),
        ("customer_name", "NVARCHAR(150)", "NO", "", "NOT NULL", "Tên cá nhân hoặc tổ chức mua lẻ."),
        ("phone", "VARCHAR(20)", "YES", "UQ", "NULL", "Số điện thoại liên lạc chính."),
        ("email", "VARCHAR(100)", "YES", "", "NULL", "Địa chỉ email liên hệ."),
        ("address", "NVARCHAR(255)", "YES", "", "NULL", "Địa chỉ giao nhận hàng hóa."),
        ("customer_segment", "VARCHAR(20)", "NO", "", "CHECK in ('Retail', 'Wholesale', 'VIP')", "Phân khúc khách hàng mua sắm."),
        ("total_spent", "DECIMAL(18,2)", "NO", "", "DEFAULT 0", "Tổng giá trị chi tiêu lũy kế (VNĐ).")
    ]
    create_styled_table(doc, ["Tên Cột", "Kiểu Dữ Liệu", "NULL", "Khóa", "Mặc Định / Ràng Buộc", "Diễn Giải Nghiệp Vụ"], t_cust, [Inches(1.2), Inches(1.1), Inches(0.5), Inches(0.5), Inches(1.8), Inches(1.4)])

    # 3.5. ORDERS
    add_heading_2(doc, "3.5. Bảng dbo.ORDERS (Hóa Đơn Bán Lẻ Tổng Quát)")
    add_body_p(doc, "Quản lý hóa đơn bán hàng tổng quát: ngày bán, nhân viên thực hiện, tổng tiền và mức chiết khấu.")
    t_orders = [
        ("order_id", "INT", "NO", "PK", "IDENTITY(1,1)", "Mã số đơn hàng nội bộ."),
        ("order_code", "VARCHAR(50)", "NO", "UQ", "NOT NULL", "Mã chứng từ hóa đơn (ví dụ: ORD-2026-0001)."),
        ("customer_id", "INT", "NO", "FK", "REFERENCES CUSTOMERS(customer_id)", "Khách hàng mua đơn hàng này."),
        ("user_id", "INT", "NO", "FK", "REFERENCES USERS(user_id)", "Nhân viên thu ngân thực hiện hóa đơn."),
        ("order_date", "DATETIME2", "NO", "", "DEFAULT SYSDATETIME()", "Thời điểm xuất hóa đơn bán lẻ."),
        ("order_status", "VARCHAR(20)", "NO", "", "CHECK in ('Pending', 'Completed', 'Cancelled')", "Trạng thái thanh toán của đơn hàng."),
        ("total_amount", "DECIMAL(18,2)", "NO", "", "DEFAULT 0", "Tổng tiền hàng trước chiết khấu (VNĐ)."),
        ("discount_amount", "DECIMAL(18,2)", "NO", "", "DEFAULT 0", "Số tiền chiết khấu khuyến mãi (VNĐ)."),
        ("final_amount", "DECIMAL(18,2)", "NO", "", "DEFAULT 0", "Tổng tiền thực thu từ khách hàng (VNĐ).")
    ]
    create_styled_table(doc, ["Tên Cột", "Kiểu Dữ Liệu", "NULL", "Khóa", "Mặc Định / Ràng Buộc", "Diễn Giải Nghiệp Vụ"], t_orders, [Inches(1.2), Inches(1.1), Inches(0.5), Inches(0.5), Inches(1.8), Inches(1.4)])

    # 3.6. ORDER_DETAILS
    add_heading_2(doc, "3.6. Bảng dbo.ORDER_DETAILS (Chi Tiết Hóa Đơn & Cột Tính Toán)")
    add_body_p(doc, "Lưu trữ chi tiết từng mặt hàng trong đơn hàng. Tích hợp cột tính toán line_total có thuộc tính PERSISTED.")
    t_od = [
        ("order_detail_id", "INT", "NO", "PK", "IDENTITY(1,1)", "Mã định danh dòng chi tiết."),
        ("order_id", "INT", "NO", "FK", "REFERENCES ORDERS(order_id) ON DELETE CASCADE", "Khóa ngoại trỏ về hóa đơn cha."),
        ("product_id", "INT", "NO", "FK", "REFERENCES PRODUCTS(product_id)", "Khóa ngoại trỏ về sản phẩm bán ra."),
        ("quantity", "INT", "NO", "", "CHECK (quantity > 0)", "Số lượng sản phẩm xuất bán (> 0)."),
        ("unit_price", "DECIMAL(18,2)", "NO", "", "CHECK (unit_price >= 0)", "Đơn giá bán thực tế tại thời điểm mua."),
        ("unit_cost", "DECIMAL(18,2)", "NO", "", "CHECK (unit_cost >= 0)", "Đơn giá vốn xuất kho tại thời điểm mua."),
        ("discount_rate", "DECIMAL(5,2)", "NO", "", "DEFAULT 0, CHECK (0 <= rate <= 100)", "Tỷ lệ giảm giá khuyến mãi (%)."),
        ("line_total", "DECIMAL(18,2)", "NO", "", "AS ((qty*price)*(1-disc/100)) PERSISTED", "Thành tiền dòng tính tự động và lưu vật lý.")
    ]
    create_styled_table(doc, ["Tên Cột", "Kiểu Dữ Liệu", "NULL", "Khóa", "Mặc Định / Ràng Buộc", "Diễn Giải Nghiệp Vụ"], t_od, [Inches(1.2), Inches(1.1), Inches(0.5), Inches(0.5), Inches(1.8), Inches(1.4)])

    doc.add_page_break()

    # 3.7. DECISION_SESSIONS
    add_heading_2(doc, "3.7. Bảng dbo.DECISION_SESSIONS (Phiên Thẩm Định & Ra Quyết Định DSS)")
    add_body_p(doc, "Quản lý chu kỳ thẩm định kế hoạch nhập hàng Quý 4/2026, lưu trữ tỷ số nhất quán CR và cờ kiểm định toán học.")
    t_sess = [
        ("session_id", "INT", "NO", "PK", "IDENTITY(1,1)", "Mã định danh phiên ra quyết định."),
        ("session_code", "VARCHAR(50)", "NO", "UQ", "NOT NULL", "Mã phiên làm việc (ví dụ: SES_2026_Q4_RETAIL)."),
        ("session_title", "NVARCHAR(200)", "NO", "", "NOT NULL", "Tiêu đề phiên họp Hội đồng ra quyết định."),
        ("description", "NVARCHAR(MAX)", "YES", "", "NULL", "Ghi chú mục tiêu chiến lược và bối cảnh."),
        ("created_by", "INT", "NO", "FK", "REFERENCES USERS(user_id)", "Cán bộ chủ trì phiên (DecisionMaker)."),
        ("start_date", "DATETIME2", "NO", "", "DEFAULT SYSDATETIME()", "Thời điểm bắt đầu phiên thẩm định."),
        ("end_date", "DATETIME2", "YES", "", "NULL", "Thời điểm chốt và ban hành quyết định."),
        ("consistency_ratio", "DECIMAL(8,4)", "YES", "", "NULL", "Tỷ số nhất quán toán học CR của Saaty."),
        ("is_consistent", "BIT", "NO", "", "AS (CASE WHEN CR <= 0.10 THEN 1 ELSE 0) PERSISTED", "Cờ chỉ thị đạt chuẩn CR <= 10% tự động."),
        ("status", "VARCHAR(20)", "NO", "", "CHECK in ('Draft', 'Calculated', 'Approved', 'Rejected')", "Trạng thái xử lý của phiên quyết định.")
    ]
    create_styled_table(doc, ["Tên Cột", "Kiểu Dữ Liệu", "NULL", "Khóa", "Mặc Định / Ràng Buộc", "Diễn Giải Nghiệp Vụ"], t_sess, [Inches(1.2), Inches(1.1), Inches(0.5), Inches(0.5), Inches(1.8), Inches(1.4)])

    # 3.8. CRITERIA
    add_heading_2(doc, "3.8. Bảng dbo.CRITERIA (Cây Tiêu Chí Đánh Giá Saaty AHP)")
    add_body_p(doc, "Quản lý 5 tiêu chí nghiệp vụ (C1-C5), định nghĩa rõ loại tiêu chí (Benefit/Cost) để áp dụng hàm chuẩn hóa.")
    t_crit = [
        ("criterion_id", "INT", "NO", "PK", "IDENTITY(1,1)", "Mã định danh tiêu chí."),
        ("parent_id", "INT", "YES", "FK", "REFERENCES CRITERIA(criterion_id)", "Hỗ trợ cấu trúc phân cấp thứ bậc đa tầng."),
        ("criterion_code", "VARCHAR(30)", "NO", "UQ", "NOT NULL", "Mã tiêu chí (C1, C2, C3, C4, C5)."),
        ("criterion_name", "NVARCHAR(150)", "NO", "", "NOT NULL", "Tên tiêu chí (Biên LN, Doanh thu, Nhu cầu...)."),
        ("description", "NVARCHAR(500)", "YES", "", "NULL", "Định nghĩa và phương thức đo lường."),
        ("criterion_type", "VARCHAR(10)", "NO", "", "CHECK in ('Benefit', 'Cost')", "Benefit (Càng lớn càng tốt) / Cost (Càng nhỏ càng tốt)."),
        ("data_source_type", "VARCHAR(30)", "NO", "", "CHECK in ('Calculated_OLTP', 'Expert_Survey', 'Market_Data')", "Nguồn trích xuất số liệu đầu vào."),
        ("is_active", "BIT", "NO", "", "DEFAULT 1", "Trạng thái áp dụng (1: Đang dùng, 0: Khóa).")
    ]
    create_styled_table(doc, ["Tên Cột", "Kiểu Dữ Liệu", "NULL", "Khóa", "Mặc Định / Ràng Buộc", "Diễn Giải Nghiệp Vụ"], t_crit, [Inches(1.2), Inches(1.1), Inches(0.5), Inches(0.5), Inches(1.8), Inches(1.4)])

    # 3.9. ALTERNATIVES
    add_heading_2(doc, "3.9. Bảng dbo.ALTERNATIVES (Danh Mục Sản Phẩm Ứng Viên DSS)")
    add_body_p(doc, "Danh mục 9 phương án sản phẩm đưa vào phiên thẩm định AHP. Có khóa ngoại product_id liên kết sang bảng PRODUCTS.")
    t_alt = [
        ("alternative_id", "INT", "NO", "PK", "IDENTITY(1,1)", "Mã định danh phương án trong phiên."),
        ("session_id", "INT", "NO", "FK", "REFERENCES DECISION_SESSIONS(session_id) ON DELETE CASCADE", "Khóa ngoại trỏ về phiên ra quyết định."),
        ("product_id", "INT", "YES", "FK", "REFERENCES PRODUCTS(product_id)", "Khóa ngoại ánh xạ sang sản phẩm thực tế."),
        ("alternative_code", "VARCHAR(50)", "NO", "", "NOT NULL", "Mã phương án (ví dụ: ALT_ROG_G16)."),
        ("alternative_name", "NVARCHAR(200)", "NO", "", "NOT NULL", "Tên phương án sản phẩm ứng viên."),
        ("description", "NVARCHAR(1000)", "YES", "", "NULL", "Tóm tắt ưu nhược điểm kỹ thuật và thị phần.")
    ]
    create_styled_table(doc, ["Tên Cột", "Kiểu Dữ Liệu", "NULL", "Khóa", "Mặc Định / Ràng Buộc", "Diễn Giải Nghiệp Vụ"], t_alt, [Inches(1.2), Inches(1.1), Inches(0.5), Inches(0.5), Inches(1.8), Inches(1.4)])

    # 3.10. PAIRWISE_COMPARISONS
    add_heading_2(doc, "3.10. Bảng dbo.PAIRWISE_COMPARISONS (Ma Trận So Sánh Cặp Saaty)")
    add_body_p(doc, "Lưu trữ ma trận so sánh cặp giữa 2 tiêu chí theo thang đo 1-9 của Saaty dưới dạng bảng dọc chuẩn 1NF.")
    t_pair = [
        ("comparison_id", "INT", "NO", "PK", "IDENTITY(1,1)", "Mã định danh bản ghi so sánh cặp."),
        ("session_id", "INT", "NO", "FK", "REFERENCES DECISION_SESSIONS(session_id) ON DELETE CASCADE", "Khóa ngoại phiên ra quyết định."),
        ("criterion_id_1", "INT", "NO", "FK", "REFERENCES CRITERIA(criterion_id)", "Tiêu chí dòng i trong ma trận."),
        ("criterion_id_2", "INT", "NO", "FK", "REFERENCES CRITERIA(criterion_id)", "Tiêu chí cột j trong ma trận."),
        ("saaty_value", "DECIMAL(10,5)", "NO", "", "CHECK (saaty_value > 0)", "Giá trị so sánh cặp a_ij (Thang đo 1/9 đến 9)."),
        ("evaluation_notes", "NVARCHAR(255)", "YES", "", "NULL", "Lý do ưu tiên hoặc căn cứ chuyên gia."),
        ("evaluated_by", "INT", "NO", "FK", "REFERENCES USERS(user_id)", "Chuyên gia hoặc cán bộ nhập điểm.")
    ]
    create_styled_table(doc, ["Tên Cột", "Kiểu Dữ Liệu", "NULL", "Khóa", "Mặc Định / Ràng Buộc", "Diễn Giải Nghiệp Vụ"], t_pair, [Inches(1.2), Inches(1.1), Inches(0.5), Inches(0.5), Inches(1.8), Inches(1.4)])

    # 3.11. ALTERNATIVE_SCORES
    add_heading_2(doc, "3.11. Bảng dbo.ALTERNATIVE_SCORES (Điểm Đo Lường & Chuẩn Hóa Tiêu Chí)")
    add_body_p(doc, "Lưu trữ điểm đo lường thực tế (raw_value) và điểm số đã được chuẩn hóa về đoạn [0.0, 1.0] theo hàm Benefit/Cost.")
    t_score = [
        ("score_id", "INT", "NO", "PK", "IDENTITY(1,1)", "Mã định danh bản ghi điểm."),
        ("session_id", "INT", "NO", "FK", "REFERENCES DECISION_SESSIONS(session_id) ON DELETE CASCADE", "Khóa ngoại phiên ra quyết định."),
        ("alternative_id", "INT", "NO", "FK", "REFERENCES ALTERNATIVES(alternative_id)", "Khóa ngoại trỏ về phương án ứng viên."),
        ("criterion_id", "INT", "NO", "FK", "REFERENCES CRITERIA(criterion_id)", "Khóa ngoại trỏ về tiêu chí đánh giá."),
        ("raw_value", "DECIMAL(18,4)", "NO", "", "NOT NULL", "Giá trị đo lường thô (%, Triệu đ, Ngày...)."),
        ("normalized_score", "DECIMAL(10,6)", "NO", "", "CHECK (score >= 0.0 AND score <= 1.0)", "Điểm chuẩn hóa trong không gian [0.0, 1.0].")
    ]
    create_styled_table(doc, ["Tên Cột", "Kiểu Dữ Liệu", "NULL", "Khóa", "Mặc Định / Ràng Buộc", "Diễn Giải Nghiệp Vụ"], t_score, [Inches(1.2), Inches(1.1), Inches(0.5), Inches(0.5), Inches(1.8), Inches(1.4)])

    # 3.12. DECISION_RESULTS
    add_heading_2(doc, "3.12. Bảng dbo.DECISION_RESULTS (Kết Quả Xếp Hạng & Khuyến Nghị Chung Cuộc)")
    add_body_p(doc, "Bảng kết quả cuối cùng: lưu điểm AHP tổng hợp, thứ vị xếp hạng (DENSE_RANK) và trạng thái phê duyệt.")
    t_res = [
        ("result_id", "INT", "NO", "PK", "IDENTITY(1,1)", "Mã định danh bản ghi kết quả."),
        ("session_id", "INT", "NO", "FK", "REFERENCES DECISION_SESSIONS(session_id) ON DELETE CASCADE", "Khóa ngoại phiên ra quyết định."),
        ("alternative_id", "INT", "NO", "FK", "REFERENCES ALTERNATIVES(alternative_id)", "Khóa ngoại trỏ về phương án ứng viên."),
        ("final_score", "DECIMAL(10,6)", "NO", "", "CHECK (score >= 0.0 AND score <= 1.0)", "Điểm AHP tổng hợp cuối cùng."),
        ("ranking", "INT", "NO", "", "CHECK (ranking > 0)", "Thứ vị xếp hạng (1 đến 9)."),
        ("decision_outcome", "VARCHAR(30)", "NO", "", "CHECK in ('Selected', 'UnderReview', 'Rejected')", "Trạng thái phê duyệt chiến lược kinh doanh."),
        ("decision_date", "DATETIME2", "NO", "", "DEFAULT SYSDATETIME()", "Thời điểm chốt kết quả thẩm định.")
    ]
    create_styled_table(doc, ["Tên Cột", "Kiểu Dữ Liệu", "NULL", "Khóa", "Mặc Định / Ràng Buộc", "Diễn Giải Nghiệp Vụ"], t_res, [Inches(1.2), Inches(1.1), Inches(0.5), Inches(0.5), Inches(1.8), Inches(1.4)])

    doc.add_page_break()

    # =========================================================================
    # PHẦN 4: TIẾN TRÌNH CHUẨN HÓA CƠ SỞ DỮ LIỆU (1NF -> 3NF)
    # =========================================================================
    add_heading_1(doc, "PHẦN 4: TIẾN TRÌNH CHUẨN HÓA CƠ SỞ DỮ LIỆU (DATABASE NORMALIZATION)")
    add_body_p(doc, "Cơ sở dữ liệu DSS_SalesDB được thiết kế tuân thủ nghiêm ngặt tiến trình chuẩn hóa cơ sở dữ liệu quan hệ của E.F. Codd từ 1NF đến 3NF, nhằm triệt tiêu hoàn toàn sự dư thừa thông tin và ngăn chặn các dị thường thao tác:")

    add_heading_2(doc, "4.1. Chuẩn Hóa Dạng 1NF (First Normal Form - Tính Nguyên Tử)")
    add_body_p(doc, "Điều kiện 1NF đòi hỏi mọi thuộc tính trong các bảng đều phải là đơn vị dữ liệu nguyên tử (atomic), không chứa mảng giá trị (repeating groups) hay chuỗi ghép ký tự (composite strings):")
    add_bullet_p(doc, "Trong các hệ thống kém chuẩn hóa, ma trận so sánh cặp Saaty thường bị lưu dạng chuỗi JSON hoặc mảng chuỗi trong một trường văn bản. DSS_SalesDB chuẩn hóa tuyệt đối bằng cách xây dựng bảng dọc PAIRWISE_COMPARISONS. Mỗi ô so sánh giữa tiêu chí i và tiêu chí j là một bản ghi nguyên tử độc lập có đầy đủ khóa chính và khóa ngoại.", "• Giải pháp ma trận Saaty chuẩn 1NF: ");
    add_bullet_p(doc, "Trường số lượng bán và đơn giá trong hóa đơn được tách riêng thành các dòng độc lập tại ORDER_DETAILS, không lưu gộp nhiều sản phẩm trong một chuỗi ký tự tại ORDERS.", "• Dòng hóa đơn nguyên tử: ");

    add_heading_2(doc, "4.2. Chuẩn Hóa Dạng 2NF (Second Normal Form - Khử Phụ Thuộc Một Phần)")
    add_body_p(doc, "Điều kiện 2NF đòi hỏi CSDL đã đạt 1NF và mọi thuộc tính không khóa phải phụ thuộc hàm toàn phần vào toàn bộ khóa chính, không được phụ thuộc một phần vào tập khóa phức hợp:")
    add_bullet_p(doc, "Tại bảng ORDER_DETAILS, khóa chính là order_detail_id (hoặc cặp order_id, product_id). Các thông tin như product_name, cost_price, selling_price, category_id không được lưu tại ORDER_DETAILS vì chúng chỉ phụ thuộc vào product_id chứ không phụ thuộc vào order_id. Chúng được tách độc lập sang bảng PRODUCTS.", "• Tách bạch thuộc tính sản phẩm: ");
    add_bullet_p(doc, "Tại bảng ALTERNATIVE_SCORES, các thông tin mô tả tiêu chí (criterion_name, criterion_type) được tách về bảng CRITERIA, chỉ lưu khóa ngoại criterion_id.", "• Tách bạch thuộc tính tiêu chí: ");

    add_heading_2(doc, "4.3. Chuẩn Hóa Dạng 3NF (Third Normal Form - Khử Phụ Thuộc Bắc Cầu)")
    add_body_p(doc, "Điều kiện 3NF đòi hỏi CSDL đã đạt 2NF và không tồn tại bất kỳ phụ thuộc hàm bắc cầu nào giữa các thuộc tính không khóa (nghĩa là không có X ➔ Y và Y ➔ Z với Z là thuộc tính không khóa):")
    add_bullet_p(doc, "Trong bảng PRODUCTS, ta có phụ thuộc product_id ➔ category_id. Nếu lưu luôn category_name trong bảng PRODUCTS thì sẽ xảy ra phụ thuộc bắc cầu: product_id ➔ category_id ➔ category_name. Hệ thống loại bỏ hoàn toàn thuộc tính category_name khỏi PRODUCTS và tách riêng sang bảng CATEGORIES.", "• Loại bỏ phụ thuộc ngành hàng: ");
    add_bullet_p(doc, "Trong bảng ORDERS, phụ thuộc order_id ➔ customer_id ➔ customer_name, customer_segment. Thông tin khách hàng được tách riêng 100% sang bảng CUSTOMERS.", "• Loại bỏ phụ thuộc khách hàng: ");

    add_callout(doc, "Chứng minh toàn vẹn 3NF: 100% các thuộc tính không khóa trong cả 12 bảng của DSS_SalesDB đều phụ thuộc trực tiếp, đầy đủ và duy nhất vào khóa chính của bảng đó (Key, the whole Key, and nothing but the Key). Do đó CSDL đạt chuẩn 3NF tuyệt đối, miễn nhiễm với lỗi dư thừa dữ liệu.", "CHỨNG MINH TÍNH ĐÚNG ĐẮN CỦA CHUẨN 3NF:");

    doc.add_page_break()

    # =========================================================================
    # PHẦN 5: RÀNG BUỘC TOÀN VẸN, CHỈ MỤC & TỐI ƯU HIỆU NĂNG
    # =========================================================================
    add_heading_1(doc, "PHẦN 5: RÀNG BUỘC TOÀN VẸN, CHỈ MỤC & TỐI ƯU HIỆU NĂNG")

    add_heading_2(doc, "5.1. Hệ Thống Ràng Buộc Nghiệp Vụ (Domain CHECK Constraints)")
    add_body_p(doc, "Hệ thống cài đặt các ràng buộc CHECK cấp CSDL nhằm ngăn chặn dữ liệu bẩn ngay từ tầng lưu trữ:")
    add_bullet_p(doc, "CK_Products_SellingPrice: selling_price >= cost_price (Giá bán niêm yết không được phép thấp hơn giá vốn nhập hàng).", "1. Ràng buộc giá bán: ");
    add_bullet_p(doc, "CK_Products_StockQuantity: stock_quantity >= 0 (Số lượng tồn kho vật lý không bao giờ mang giá trị âm).", "2. Ràng buộc tồn kho: ");
    add_bullet_p(doc, "CK_OrderDetails_Quantity: quantity > 0 (Số lượng sản phẩm trong đơn hàng bán lẻ bắt buộc phải lớn hơn 0).", "3. Ràng buộc đơn hàng: ");
    add_bullet_p(doc, "CK_AltScores_Normalized: normalized_score >= 0.0 AND normalized_score <= 1.0 (Điểm chuẩn hóa AHP bắt buộc nằm trong đoạn [0, 1]).", "4. Ràng buộc điểm chuẩn hóa: ");
    add_bullet_p(doc, "CK_Pairwise_SaatyValue: saaty_value > 0 (Điểm so sánh cặp Saaty bắt buộc phải là số thực dương).", "5. Ràng buộc thang đo Saaty: ");

    add_heading_2(doc, "5.2. Danh Mục Chỉ Mục Phi Cụm Tối Ưu Hóa Truy Vấn (Indexes)")
    add_body_p(doc, "Để đảm bảo tốc độ truy vấn tức thời (< 50ms) cho các báo cáo DSS và OLAP, hệ thống khởi tạo 9 chỉ mục phi cụm:")
    
    idx_data = [
        ("IX_Products_CategoryId", "dbo.PRODUCTS (category_id)", "Tối ưu hóa phép JOIN giữa bảng ngành hàng và sản phẩm."),
        ("IX_Orders_CustomerId", "dbo.ORDERS (customer_id)", "Tối ưu hóa truy vấn lịch sử mua sắm theo từng khách hàng."),
        ("IX_Orders_OrderDate", "dbo.ORDERS (order_date)", "Tăng tốc lọc dữ liệu bán hàng theo khoảng thời gian báo cáo Quý 4/2026."),
        ("IX_OrderDetails_ProductId", "dbo.ORDER_DETAILS (product_id)", "Tăng tốc tổng hợp doanh thu và sản lượng bán theo từng mã SKU."),
        ("IX_OrderDetails_OrderId", "dbo.ORDER_DETAILS (order_id)", "Tăng tốc trích xuất chi tiết đơn lẻ khi in hóa đơn bán hàng."),
        ("IX_Alternatives_SessionId", "dbo.ALTERNATIVES (session_id)", "Tăng tốc nạp danh mục 9 sản phẩm ứng viên của phiên DSS."),
        ("IX_Pairwise_SessionId", "dbo.PAIRWISE_COMPARISONS (session_id)", "Tăng tốc trích xuất ma trận Saaty 5x5 để tính toán vector trọng số."),
        ("IX_AltScores_SessionId", "dbo.ALTERNATIVE_SCORES (session_id)", "Tăng tốc nạp bảng điểm đa tiêu chí phục vụ nhân ma trận."),
        ("IX_Results_SessionRanking", "dbo.DECISION_RESULTS (session_id, ranking)", "Tối ưu hóa sắp xếp và hiển thị bảng xếp hạng từ Hạng 1 đến Hạng 9.")
    ]
    create_styled_table(doc, ["Tên Chỉ Mục (Index Name)", "Bảng & Cột Được Đánh Chỉ Mục", "Mục Đích Tối Ưu Hóa & Lợi Ích Hiệu Năng Truy Vấn"], idx_data, [Inches(1.8), Inches(2.2), Inches(2.5)])

    doc.add_page_break()

    # =========================================================================
    # PHẦN 6: TRUY VẤN NÂNG CAO & KHỐI PHÂN TÍCH ĐA CHIỀU OLAP
    # =========================================================================
    add_heading_1(doc, "PHẦN 6: TRUY VẤN NÂNG CAO & KHỐI PHÂN TÍCH ĐA CHIỀU OLAP")

    add_heading_2(doc, "6.1. View Hiệu Suất Sản Phẩm Thực Tế (vw_DSS_ProductPerformance)")
    add_body_p(doc, "View tính toán tổng hợp các chỉ số thực tế làm căn cứ dữ liệu thô (raw_value) cho mô hình AHP:")
    add_bullet_p(doc, "SELECT p.product_id, p.product_code, p.product_name, c.category_name, p.cost_price, p.selling_price, ((p.selling_price - p.cost_price)/p.selling_price)*100 AS gross_margin_pct, p.stock_quantity, ISNULL(SUM(od.quantity), 0) AS total_sold_qty, ISNULL(SUM(od.line_total), 0) AS actual_revenue FROM dbo.PRODUCTS p INNER JOIN dbo.CATEGORIES c ON p.category_id = c.category_id LEFT JOIN dbo.ORDER_DETAILS od ON p.product_id = od.product_id GROUP BY p.product_id, p.product_code, p.product_name, c.category_name, p.cost_price, p.selling_price, p.stock_quantity;", "• Mã định nghĩa View: ");

    add_heading_2(doc, "6.2. Thuật Toán AHP Tích Hợp Bằng Common Table Expressions (CTE)")
    add_body_p(doc, "Truy vấn SQL thực thi trọn vẹn giải thuật Saaty AHP trong SQL Server: tính tổng cột ma trận ➔ chuẩn hóa vector trọng số ➔ nhân ma trận điểm ➔ xếp hạng bằng hàm DENSE_RANK():")
    add_bullet_p(doc, "WITH WeightCalculated AS (SELECT pc.criterion_id_1 AS criterion_id, AVG(pc.saaty_value / col_sums.col_sum) AS criterion_weight FROM dbo.PAIRWISE_COMPARISONS pc INNER JOIN (SELECT criterion_id_2, SUM(saaty_value) AS col_sum FROM dbo.PAIRWISE_COMPARISONS WHERE session_id = 1 GROUP BY criterion_id_2) col_sums ON pc.criterion_id_2 = col_sums.criterion_id_2 WHERE pc.session_id = 1 GROUP BY pc.criterion_id_1), AltScores AS (SELECT s.alternative_id, SUM(w.criterion_weight * s.normalized_score) AS comp_score FROM dbo.ALTERNATIVE_SCORES s INNER JOIN WeightCalculated w ON s.criterion_id = w.criterion_id WHERE s.session_id = 1 GROUP BY s.alternative_id) SELECT alternative_id, comp_score, DENSE_RANK() OVER (ORDER BY comp_score DESC) as rk FROM AltScores;", "• Truy vấn CTE: ");

    add_heading_2(doc, "6.3. Khối Phân Tích Phân Cấp Đa Tầng OLAP ROLLUP")
    add_body_p(doc, "Truy vấn phục vụ Tab 5, cho phép phân tích tồn kho cuộn từ mức từng SKU lên mức Ngành hàng và Toàn doanh nghiệp:")
    add_bullet_p(doc, "SELECT CASE WHEN GROUPING(c.category_name)=1 THEN N'-- TẤT CẢ NGÀNH HÀNG --' ELSE c.category_name END AS [Ngành Hàng], CASE WHEN GROUPING(p.product_name)=1 THEN N'-- TỔNG CỘNG NHÓM --' ELSE p.product_name END AS [Sản Phẩm], SUM(p.stock_quantity) AS [Tổng Tồn Kho], ISNULL(SUM(od.quantity), 0) AS [Tổng Đã Bán], FORMAT(ISNULL(SUM(od.line_total), 0), '#,##0') + N' đ' AS [Doanh Thu] FROM dbo.CATEGORIES c INNER JOIN dbo.PRODUCTS p ON c.category_id = p.category_id LEFT JOIN dbo.ORDER_DETAILS od ON p.product_id = od.product_id GROUP BY ROLLUP(c.category_name, p.product_name);", "• Cú pháp T-SQL: ");

    add_heading_2(doc, "6.4. Ma Trận Đa Chiều Phân Khúc Khách Hàng OLAP CUBE")
    add_body_p(doc, "Truy vấn ma trận đa chiều giữa Phân khúc khách hàng (VIP, Wholesale, Retail) và Ngành hàng kinh doanh:")
    add_bullet_p(doc, "SELECT ISNULL(cust.customer_segment, N'-- TẤT CẢ PHÂN KHÚC --') AS [Phân Khúc], ISNULL(cat.category_name, N'-- TẤT CẢ NGÀNH HÀNG --') AS [Ngành Hàng], COUNT(DISTINCT o.order_id) AS [Số Đơn Hàng], FORMAT(ISNULL(SUM(od.line_total), 0), '#,##0') + N' đ' AS [Doanh Thu] FROM dbo.CUSTOMERS cust INNER JOIN dbo.ORDERS o ON cust.customer_id = o.customer_id INNER JOIN dbo.ORDER_DETAILS od ON o.order_id = od.order_id INNER JOIN dbo.PRODUCTS p ON od.product_id = p.product_id INNER JOIN dbo.CATEGORIES cat ON p.category_id = cat.category_id GROUP BY CUBE(cust.customer_segment, cat.category_name);", "• Cú pháp T-SQL: ");

    doc.add_page_break()

    # =========================================================================
    # PHẦN 7: KẾT LUẬN & HƯỚNG DẪN BẢO TRÌ CƠ SỞ DỮ LIỆU
    # =========================================================================
    add_heading_1(doc, "PHẦN 7: KẾT LUẬN & HƯỚNG DẪN BẢO TRÌ CƠ SỞ DỮ LIỆU")

    add_body_p(doc, "Cơ sở dữ liệu DSS_SalesDB là công trình nghiên cứu và hiện thực hóa chuẩn mực của Nhóm 05 trong học phần IS 402 A. CSDL đã kết nối thành công hai thế giới dữ liệu: Giao dịch bán lẻ OLTP và Mô hình toán học đa tiêu chí AHP, tạo bệ phóng vững chắc cho hệ thống phần mềm DSS Retail Decision Studio 2026 hoạt động ổn định và chính xác.")

    add_heading_2(doc, "7.1. Kế Hoạch Sao Lưu & Phục Hồi Dữ Liệu (Backup & Disaster Recovery)")
    add_bullet_p(doc, "Sao lưu toàn bộ CSDL DSS_SalesDB vào lúc 23:00 hàng ngày: BACKUP DATABASE DSS_SalesDB TO DISK = 'D:\\Backup\\DSS_SalesDB_Full.bak' WITH INIT, COMPRESSION;", "• Sao lưu toàn phần (Full Backup): ");
    add_bullet_p(doc, "Sao lưu các thay đổi phát sinh vào 12:00 trưa: BACKUP DATABASE DSS_SalesDB TO DISK = 'D:\\Backup\\DSS_SalesDB_Diff.bak' WITH DIFFERENTIAL;", "• Sao lưu vi sai (Differential): ");
    add_bullet_p(doc, "Khi gặp sự cố, thực hiện lệnh: RESTORE DATABASE DSS_SalesDB FROM DISK = 'D:\\Backup\\DSS_SalesDB_Full.bak' WITH REPLACE;", "• Phục hồi thảm họa (Disaster Recovery): ");

    add_heading_2(doc, "7.2. Thông Tin Đội Ngũ Thực Hiện & Đơn Vị Đào Tạo")
    add_bullet_p(doc, "Nhóm 05 – Lớp IS 402 A – Khoa Hệ thống Thông tin Quản lý (MIS).", "• Đơn vị thực hiện: ");
    add_bullet_p(doc, "Trường Đào tạo Quốc tế (International School) – Đại học Duy Tân, Đà Nẵng, Việt Nam.", "• Cơ sở đào tạo: ");
    add_bullet_p(doc, "Trần Hồng Minh (Trưởng nhóm - MSSV: 29210247142), Lê Thị Nga (MSSV: 29206361029), Phạm Ngọc Tú (MSSV: 29204347819), Lê Thanh Diệu (MSSV: 29204622434), Hồ Thị Thuận (MSSV: 29201164457).", "• Danh sách thành viên: ");
    add_bullet_p(doc, "ThS. Nguyễn Quang Ánh.", "• Giảng viên hướng dẫn: ");

    # Save Word Document
    out_path = 'docs/TAI_LIEU_THIET_KE_DAC_TA_DATABASE_DSS_IS402_NHOM05.docx'
    doc.save(out_path)
    print(f"SUCCESS: Document saved at {out_path}")

if __name__ == '__main__':
    build_database_spec_docx()
