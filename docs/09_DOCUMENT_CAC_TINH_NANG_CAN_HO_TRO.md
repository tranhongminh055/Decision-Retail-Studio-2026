# DUY TÂN UNIVERSITY – INTERNATIONAL SCHOOL
## HỌC PHẦN: IS 402 A – HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH (DSS)
**Thời điểm nghiệm thu & phát hành: 28/10/2026**

---

# TÀI LIỆU SỐ 09: ĐẶC TẢ CÁC TÍNH NĂNG CẦN HỖ TRỢ CỦA HỆ THỐNG
### (SYSTEM FEATURE SPECIFICATION & CAPABILITY SUPPORT DOCUMENT)
**Hệ thống Hỗ trợ Ra Quyết định Chiến lược Khuyến mãi & Phân bổ Hàng hóa**  
*Phần mềm: DSS Retail Decision Studio 2026 (C# .NET 10 WinForms, AHP Saaty & SQL Server DSS_SalesDB)*

* **Nhóm sinh viên thực hiện (Nhóm 05)**:
  1. Trần Hồng Minh *(Trưởng nhóm - MSSV: 29210247142)*
  2. Lê Thị Nga *(MSSV: 29206361029)*
  3. Phạm Ngọc Tú *(MSSV: 29204347819)*
  4. Lê Thanh Diệu *(MSSV: 29204622434)*
  5. Hồ Thị Thuận *(MSSV: 29201164457)*
* **Giảng viên chủ trì (Approved by)**: ThS. Nguyễn Quang Ánh

---

## 1. MỤC TIÊU VÀ PHẠM VI TÀI LIỆU

Tài liệu **"09. Document Các Tính Năng Cần Hỗ Trợ"** (*System Feature Specification & Capability Support Document*) được xây dựng nhằm định nghĩa, chuẩn hóa và đặc tả chi tiết toàn bộ các tính năng nghiệp vụ, tính năng kỹ thuật và các yêu cầu hỗ trợ người dùng mà hệ thống phần mềm **DSS Retail Decision Studio 2026** của Nhóm 05 phải cung cấp. Tài liệu là cơ sở pháp lý và kỹ thuật phục vụ việc đối soát, nghiệm thu và đánh giá mức độ hoàn thiện của hệ thống trước Hội đồng bảo vệ đồ án môn học IS 402 A.

Toàn bộ các tính năng được thiết kế xoay quanh mục tiêu trung tâm: **Ứng dụng mô hình Phân tích Thứ bậc AHP (Thomas L. Saaty)** để giải quyết bài toán đa tiêu chí trong kinh doanh bán lẻ, hỗ trợ nhà quản lý tối ưu hóa danh mục 9 dòng sản phẩm, phân bổ hàng tồn kho và điều tiết ngân sách xúc tiến thương mại một cách khoa học, khách quan và minh bạch.

---

## 2. MA TRẬN PHÂN LOẠI TÍNH NĂNG THEO MÔ HÌNH MOSCOW

| Phân loại | Mức độ ưu tiên | Mô tả phạm vi tính năng trong hệ thống DSS |
| :--- | :--- | :--- |
| **Must Have (Bắt buộc)** | *Cốt lõi (100% Phải có)* | Ma trận so sánh cặp Saaty 5 tiêu chí; tính vector trọng số $w_i$; kiểm tra tỷ số nhất quán $CR \le 0.10$; chuẩn hóa điểm Benefit ($v/v_{\max}$) và Cost ($v_{\min}/v$); tính điểm AHP tổng hợp và xếp hạng danh mục 9 sản phẩm; phân loại phương án (`Selected` / `UnderReview` / `Rejected`); công thức đề xuất nhập hàng dựa trên lead time và chu kỳ 30 ngày; phân bổ ngân sách marketing; kết nối CSDL SQL Server (`DSS_SalesDB`). |
| **Should Have (Rất quan trọng)** | *Mở rộng nâng cao* | Mô phỏng phân tích độ nhạy What-If với 3 kịch bản thị trường (Bùng nổ Gaming, Đứt gãy cung ứng, Lạm phát); thanh trượt tùy biến tham số độ nhạy trực tiếp trên từng sản phẩm; trực quan hóa Biểu đồ đường (*Delta Chart*) và Sơ đồ cây cấu trúc thứ bậc phân cấp AHP có mũi tên định hướng; thẩm định động thêm sản phẩm mới (Tab 4). |
| **Could Have (Khuyến nghị)** | *Tính năng gia tăng* | Phân tích dữ liệu bán hàng đa chiều OLAP (Slicing theo chi nhánh/ngành hàng, Drill-down theo thời gian); sinh nhận xét điều hành (*Executive Comments*) tự động; lưu vết phiên làm việc (*Audit Trail*); xuất báo cáo thẩm định DSS sang tài liệu Word/HTML; kiểm tra giám sát trạng thái CSDL tự động (`DatabaseGuard`). |
| **Won't Have (Ngoài phạm vi)** | *Tách biệt hệ thống ngoài* | Không thay thế máy tính tiền POS trực tiếp tại quầy; không tự động xuất hóa đơn tài chính hoặc trừ hàng vật lý; không tự động thanh toán ngân hàng hay điều phối xe tải giao hàng. Quyết định của DSS mang tính tham mưu chiến lược, việc phê duyệt và triển khai thực tế do Ban Giám đốc và quản lý kho vận quyết định. |

---

## 3. ĐẶC TẢ CHI TIẾT DANH MỤC CÁC TÍNH NĂNG CẦN HỖ TRỢ

### FN-01: Xác thực, Phân quyền & Quản trị Phiên (Authentication & RBAC)

| Mã & Tên Tính Năng | Mô Tả Đặc Tả & Cơ Chế Hoạt Động Trong Hệ Thống |
| :--- | :--- |
| **FN-01.1: Đăng nhập hệ thống bảo mật** | Hỗ trợ giao diện `LoginForm` đăng nhập bằng tài khoản và mật khẩu. Mật khẩu được băm (*hash*) bằng thuật toán SHA-256 trước khi đối chiếu với CSDL SQL Server (bảng `USERS`), ngăn chặn hoàn toàn việc rò rỉ mật khẩu dưới dạng văn bản thô (*plain-text*). |
| **FN-01.2: Phân quyền người dùng theo vai trò (RBAC)** | Hỗ trợ 3 nhóm vai trò: Quản trị viên (`Admin` - toàn quyền cấu hình CSDL và tài khoản), Chuyên viên phân tích (`Analyst` - toàn quyền thiết lập ma trận AHP, chạy DSS, mô phỏng What-if), Quản lý/Giám đốc (`Manager` - quyền xem Dashboard, báo cáo điều hành, phê duyệt hoặc từ chối kế hoạch đề xuất). |
| **FN-01.3: Đăng ký tài khoản & quản trị phiên làm việc** | Hỗ trợ màn hình `RegisterForm` cho phép tạo người dùng mới, kiểm tra tính hợp lệ của email, số điện thoại và chặn trùng lặp username. Class `UserSession` duy trì phiên làm việc an toàn trong suốt thời gian ứng dụng hoạt động. |
| **FN-01.4: Tự động giám sát & bảo vệ kết nối CSDL (DatabaseGuard)** | Hệ thống tích hợp bộ giám sát kết nối tự động; hiển thị đèn báo trạng thái CSDL thời gian thực; tự động kết nối lại khi mạng nội bộ hoặc dịch vụ SQL Server khởi động lại, chống sập ứng dụng (*crash-free*). |

---

### FN-02: Tiếp nhận, Quản lý & Làm sạch Dữ liệu (Data Management & ETL)

| Mã & Tên Tính Năng | Mô Tả Đặc Tả & Cơ Chế Hoạt Động Trong Hệ Thống |
| :--- | :--- |
| **FN-02.1: Đồng bộ dữ liệu bán lẻ & danh mục sản phẩm** | Hỗ trợ đọc và ánh xạ dữ liệu trực tiếp từ 11 bảng CSDL SQL Server (`ORDERS`, `ORDER_DETAILS`, `PRODUCTS`, `CATEGORIES`, `CUSTOMERS`, `CRITERIA`, `PAIRWISE_COMPARISONS`, `DECISION_SESSIONS`, `ALTERNATIVES`, `ALTERNATIVE_SCORES`, `DECISION_RESULTS`). |
| **FN-02.2: Kiểm tra toàn vẹn dữ liệu & ràng buộc nghiệp vụ** | Hệ thống kiểm tra tự động các ràng buộc: không bỏ trống khóa chính/SKU, không cho phép giá bán hoặc tồn kho âm, kiểm tra tính hợp lệ của khóa ngoại, khử trùng lặp bản ghi và xử lý giá trị NULL an toàn không làm gián đoạn tính toán. |
| **FN-02.3: Thẩm định động sản phẩm mới (Tab 4)** | Cung cấp form trực quan cho phép nhập thông tin sản phẩm mới (Mã SKU, tên sản phẩm, giá bán, giá vốn, tồn kho, lead time) và 5 điểm tiêu chí thô; tự động ghi đồng thời vào cả bảng `PRODUCTS` lẫn `ALTERNATIVES` và kích hoạt chạy lại DSS ngay lập tức. |

---

### FN-03: Phân tích Dữ liệu Bán hàng Đa chiều (OLAP Engine - Tab 5)

| Mã & Tên Tính Năng | Mô Tả Đặc Tả & Cơ Chế Hoạt Động Trong Hệ Thống |
| :--- | :--- |
| **FN-03.1: Cắt lát dữ liệu (Slicing / Dicing) theo Ngành hàng & Chi nhánh** | Hỗ trợ lọc động dữ liệu bán lẻ theo danh mục ngành hàng (Điện thoại, Laptop, Phụ kiện...) và chi nhánh bán lẻ để phân tích hiệu quả doanh số theo từng phân khúc thị trường. |
| **FN-03.2: Cuộn lên / Khoan sâu (Roll-up / Drill-down) theo chiều thời gian** | Hỗ trợ tổng hợp dữ liệu giao dịch từ mức chi tiết từng đơn hàng theo ngày lên mức tuần, tháng, quý và năm, giúp phát hiện quy luật mùa vụ và xu hướng tăng trưởng. |
| **FN-03.3: Thống kê tổng hợp doanh thu, biên lợi nhuận & vòng quay tồn kho** | Tự động tính toán các chỉ số kinh doanh then chốt cho từng sản phẩm: Doanh thu thực tế, Số lượng xuất bán, Tỷ suất lợi nhuận gộp thực tế (%) và Tốc độ bán trung bình ngày (*Daily Demand*). |

---

### FN-04: Bộ máy Ra Quyết định AHP Đa tiêu chí (AHP Decision Engine - Tab 1 & Tab 2)

| Mã & Tên Tính Năng | Mô Tả Đặc Tả & Cơ Chế Hoạt Động Trong Hệ Thống |
| :--- | :--- |
| **FN-04.1: Ma trận so sánh cặp Saaty 5 tiêu chí ($C_1 - C_5$)** | Hỗ trợ ma trận kích thước $5 \times 5$ so sánh mức độ quan trọng tương đối giữa 5 tiêu chí: Biên lợi nhuận ($C_1$), Doanh thu kỳ vọng ($C_2$), Nhu cầu thị trường ($C_3$), Rủi ro tồn kho ($C_4$), Chi phí tiếp thị ($C_5$) theo thang đo 1-9 của Saaty. |
| **FN-04.2: Tự động tính toán Trọng số & Kiểm tra Tỷ số nhất quán ($CR \le 0.10$)** | Tự động tính toán: Vector trọng số tiêu chí ($w_i$), Eigenvalue cực đại ($\lambda_{\max}$), Chỉ số nhất quán ($CI$), Tỷ số ngẫu nhiên ($RI=1.12$) và Tỷ số nhất quán $CR = CI / RI$. Hệ thống hiển thị huy hiệu xanh "ĐẠT CHUẨN NHẤT QUÁN" khi $CR \le 10\%$, hoặc cảnh báo đỏ và chặn tính điểm khi $CR > 10\%$. |
| **FN-04.3: Chuẩn hóa đa tiêu chí theo hàm Benefit & Cost** | Hệ thống tự động nhận diện loại tiêu chí để chuẩn hóa ma trận điểm thô: Tiêu chí Lợi ích (Benefit: $C_1, C_2, C_3$) lấy $v / max\_raw$; Tiêu chí Chi phí/Rủi ro (Cost: $C_4, C_5$) lấy $min\_raw / raw$. Mọi giá trị chuẩn hóa đều được ánh xạ về đoạn chuẩn $[0.0, 1.0]$. |
| **FN-04.4: Tính điểm tổng hợp AHP & Xếp hạng tự động 9 dòng sản phẩm** | Thực hiện nhân vector trọng số với ma trận chuẩn hóa: $Score_j = \sum (w_i \times norm_{ij})$. Tự động sắp xếp thứ hạng giảm dần cho toàn bộ 9 dòng sản phẩm từ Hạng 1 đến Hạng 9, hỗ trợ thuật toán giải quyết đồng điểm (*tie-break*) nhất quán. |
| **FN-04.5: Phân loại trạng thái quyết định quản trị tự động** | Hệ thống tự động phân loại kết quả: Hạng 1 ➔ `Selected` (Phương án tối ưu nhất - Ưu tiên rót vốn & triển khai); Điểm AHP $\ge 0.60$ ➔ `UnderReview` (Khả quan - Đề xuất nhập thăm dò); Điểm AHP $< 0.60$ ➔ `Rejected` (Loại bỏ / Xả tồn kho). |

---

### FN-05: Đề xuất Đặt hàng & Phân bổ Ngân sách (Recommendation Engine - Tab 2)

| Mã & Tên Tính Năng | Mô Tả Đặc Tả & Cơ Chế Hoạt Động Trong Hệ Thống |
| :--- | :--- |
| **FN-05.1: Công thức đề xuất số lượng đặt hàng khoa học** | Tự động tính số lượng nhập bổ sung:  <br>$$Q_{suggested} = DailyDemand \times (LeadTime + 30) + MinStock - StockQuantity$$  <br>Đảm bảo không thiếu hàng trong chu kỳ kế tiếp và không gây đọng vốn dư thừa. |
| **FN-05.2: Phân bổ tỷ trọng ngân sách marketing theo điểm số AHP** | Tự động tính tỷ lệ phân bổ ngân sách xúc tiến thương mại dựa trên tỷ trọng điểm số AHP giữa các sản phẩm được duyệt (`Selected` và `UnderReview`), tối đa hóa hiệu quả đồng vốn đầu tư. |
| **FN-05.3: Báo cáo nhận xét điều hành chuyên sâu (Executive Comments)** | Hệ thống tự động sinh các đoạn văn bản phân tích điều hành cho từng sản phẩm: chỉ rõ điểm mạnh (biên lợi nhuận cao, doanh thu vượt trội), cảnh báo điểm yếu (lead time dài, tồn kho quá hạn) và đưa ra hành động cụ thể cho ban giám đốc. |

---

### FN-06: Mô phỏng Độ nhạy Thị trường What-If (Sensitivity Analysis - Tab 3)

| Mã & Tên Tính Năng | Mô Tả Đặc Tả & Cơ Chế Hoạt Động Trong Hệ Thống |
| :--- | :--- |
| **FN-06.1: Cung cấp 3 kịch bản thị trường vi mô & vĩ mô định sẵn** | Hỗ trợ kích hoạt tức thời 3 kịch bản: Kịch bản 1 (*Bùng nổ Gaming +40%*), Kịch bản 2 (*Đứt gãy cung ứng - Rủi ro vốn cao*), Kịch bản 3 (*Lạm phát - Thắt chặt chi tiêu, ưu tiên biên lợi nhuận cao*). |
| **FN-06.2: Tùy biến tham số độ nhạy trực tiếp trên từng sản phẩm** | Cho phép chuyên viên chọn sản phẩm bất kỳ, điều chỉnh các tham số độ nhạy: Nhu cầu thị trường ($C_3$), Biên lợi nhuận ($C_1$), Mức độ an toàn vốn ($C_4$); bấm "Áp Dụng Tham Số" để tái tính toán tức thời điểm số AHP và quan sát sự dịch chuyển thứ hạng. |
| **FN-06.3: Cơ chế In-Memory cách ly an toàn & Phục hồi dữ liệu gốc** | Quá trình mô phỏng What-If chỉ diễn ra trên bộ nhớ tạm (*In-Memory DataTable*), không làm thay đổi hay ghi đè CSDL gốc. Nút "Khôi Phục Dữ Liệu Gốc" cho phép hoàn nguyên dữ liệu về trạng thái ban đầu của SQL Server ngay lập tức. |

---

### FN-07: Trực quan hóa Đồ họa & Giao diện Tương tác (Visualization & UX)

| Mã & Tên Tính Năng | Mô Tả Đặc Tả & Cơ Chế Hoạt Động Trong Hệ Thống |
| :--- | :--- |
| **FN-07.1: Biểu đồ cột phân bổ tỷ trọng điểm AHP (AHPScoreChartPanel)** | Vẽ biểu đồ cột trực quan hiển thị đầy đủ 9 sản phẩm; chiều rộng cột co giãn linh hoạt; có mã màu nhận diện theo thứ hạng (Hạng 1 vàng kim/xanh lá, Hạng 2 xanh dương, Hạng 3-9 xám); hiển thị điểm số và nhãn khuyến nghị rõ ràng. |
| **FN-07.2: Biểu đồ đường đối chiếu độ nhạy (Delta Line Chart)** | Vẽ đồng thời 2 đường đối chiếu: Đường màu xanh thể hiện Điểm AHP cơ sở, Đường nét đứt màu đỏ thể hiện Điểm AHP mới sau biến động What-If; hiển thị các điểm mốc (*node*) trực quan giúp nhận diện ngay mức độ tăng/giảm điểm số. |
| **FN-07.3: Sơ đồ cây cấu trúc thứ bậc AHP phân cấp có mũi tên định hướng** | Hiển thị cây phân cấp 3 tầng (Mục tiêu ➔ 5 Tiêu chí ➔ 9 Phương án). Có mũi tên định hướng kết nối từ Mục tiêu xuống Tiêu chí, và từ từng Tiêu chí xuống Phương án. Bố cục 9 thẻ sản phẩm tự động căn chỉnh theo tỷ lệ màn hình, triệt tiêu hoàn toàn lỗi bị tràn hoặc khuất nội dung. |
| **FN-07.4: Nút điều khiển phản hồi nhanh (Chạy lại mô hình & Chạy DSS ngay)** | Tích hợp nút "Chạy Lại Mô Hình" trên Tab 2 và nút "CHẠY MÔ HÌNH DSS NGAY" trên thanh trạng thái Tab 1, giúp người dùng chủ động kích hoạt tính toán lại sau khi chỉnh sửa tham số mà không cần thao tác phức tạp. |

---

### FN-08: Báo cáo Thẩm định & Lưu vết Lịch sử (Reporting & Audit Trail - Tab 6)

| Mã & Tên Tính Năng | Mô Tả Đặc Tả & Cơ Chế Hoạt Động Trong Hệ Thống |
| :--- | :--- |
| **FN-08.1: Lưu vết lịch sử tính toán (Audit Trail)** | Mỗi lần chạy mô hình DSS, hệ thống tự động ghi nhận một phiên làm việc vào bảng `DECISION_SESSIONS` (mã phiên, ngày chạy, người thực hiện, tỷ số CR) và chi tiết kết quả vào `DECISION_RESULTS`, đảm bảo tính giải trình và minh bạch thông tin. |
| **FN-08.2: Xuất báo cáo thẩm định đa tiêu chí sang file tài liệu** | Hỗ trợ `ReportBuilder` tự động trích xuất toàn bộ ma trận so sánh cặp, bảng xếp hạng 9 sản phẩm, phân bổ ngân sách, đề xuất nhập hàng và nhận xét điều hành sang định dạng file Word (.doc/.docx) hoặc HTML, sẵn sàng in ấn phục vụ họp giao ban. |

---

## 4. YÊU CẦU HỖ TRỢ KỸ THUẬT & MÔI TRƯỜNG VẬN HÀNH

| Hạng mục hỗ trợ | Cấu hình tối thiểu (Minimum) | Cấu hình khuyến nghị (Recommended) |
| :--- | :--- | :--- |
| **Hệ điều hành** | Microsoft Windows 10 x64 (Build 19041+) | Microsoft Windows 11 x64 (Phiên bản mới nhất) |
| **Bộ xử lý (CPU)** | Intel Core i3 thế hệ 8 hoặc AMD Ryzen 3 (2.0 GHz) | Intel Core i5 / i7 thế hệ 11+ hoặc AMD Ryzen 5 / 7 |
| **Bộ nhớ RAM** | 4 GB RAM khả dụng | 8 GB – 16 GB RAM (cho hiệu năng xử lý đa luồng cao) |
| **Dung lượng đĩa cứng** | 500 MB trống cho ứng dụng và runtime | SSD 2 GB trống (đảm bảo tốc độ truy vấn CSDL nhanh) |
| **Dịch vụ Cơ sở dữ liệu** | Microsoft SQL Server 2019 Express | Microsoft SQL Server 2022 Express / Developer Edition |

---

## 5. BẢNG ĐỐI SOÁT MỨC ĐỘ HOÀN THIỆN TÍNH NĂNG (COMPLIANCE MATRIX)

| Mã Cụm | Tên Nhóm Tính Năng | Trạng Thái Hiện Thực Trong Dự Án | Mức Độ Hoàn Thiện |
| :---: | :--- | :--- | :---: |
| **FN-01** | Xác thực, Phân quyền & Quản trị Phiên | Đã hiện thực 100% (LoginForm, RegisterForm, UserSession, SHA-256) | **☑ 100% - Đạt chuẩn** |
| **FN-02** | Tiếp nhận, Quản lý & Làm sạch Dữ liệu | Đã hiện thực 100% (Đồng bộ SQL Server, kiểm tra toàn vẹn, thêm SP) | **☑ 100% - Đạt chuẩn** |
| **FN-03** | Phân tích Dữ liệu Bán hàng Đa chiều (OLAP) | Đã hiện thực 100% (Tab 5: Slicing/Dicing, Roll-up/Drill-down) | **☑ 100% - Đạt chuẩn** |
| **FN-04** | Bộ máy Ra Quyết định AHP Saaty | Đã hiện thực 100% (Ma trận Saaty, CR=0.0152, chuẩn hóa Benefit/Cost, 9 SP) | **☑ 100% - Đạt chuẩn** |
| **FN-05** | Đề xuất Đặt hàng & Phân bổ Ngân sách | Đã hiện thực 100% (Công thức tồn kho chu kỳ 30 ngày, Executive Summary) | **☑ 100% - Đạt chuẩn** |
| **FN-06** | Mô phỏng Độ nhạy Thị trường What-If | Đã hiện thực 100% (3 kịch bản, chỉnh tham số, Delta Chart, In-Memory) | **☑ 100% - Đạt chuẩn** |
| **FN-07** | Trực quan hóa Đồ họa & Giao diện UX | Đã hiện thực 100% (Biểu đồ cột 9 SP, Sơ đồ cây AHP có mũi tên nối) | **☑ 100% - Đạt chuẩn** |
| **FN-08** | Báo cáo Thẩm định & Lưu vết Audit Trail | Đã hiện thực 100% (Lưu bảng DECISION_SESSIONS/RESULTS, xuất Word) | **☑ 100% - Đạt chuẩn** |

---

### KẾT LUẬN & CAM KẾT CHẤT LƯỢNG:
Tài liệu **"09. Document Các Tính Năng Cần Hỗ Trợ"** khẳng định toàn bộ 8 cụm tính năng nghiệp vụ và kỹ thuật cốt lõi đã được Nhóm 05 lập trình hiện thực hóa hoàn chỉnh, chạy ổn định và đạt độ chính xác toán học tuyệt đối trên môi trường thực nghiệm. Hệ thống không chỉ đáp ứng trọn vẹn yêu cầu của học phần IS 402 A mà còn có giá trị ứng dụng thực tiễn cao trong việc chuyển đổi số và tối ưu hóa quản trị chuỗi cung ứng bán lẻ.
