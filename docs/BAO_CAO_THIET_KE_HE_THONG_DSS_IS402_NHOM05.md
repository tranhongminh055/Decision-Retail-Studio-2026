# DUY TÂN UNIVERSITY – INTERNATIONAL SCHOOL
## HỌC PHẦN: IS 402 A – HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH (DSS)
**Date: 28/10/2026**

---

# HỆ THỐNG HỖ TRỢ RA QUYẾT ĐỊNH CHIẾN LƯỢC KHUYẾN MÃI & PHÂN BỔ HÀNG HÓA
### TÀI LIỆU THIẾT KẾ KIẾN TRÚC & ĐẶC TẢ HỆ THỐNG (DSS SYSTEM DESIGN DOCUMENT)
**Ứng dụng Mô hình Phân tích Thứ bậc AHP (Thomas L. Saaty) và Cơ sở Dữ liệu SQL Server**

* **Nhóm thực hiện**: Nhóm 05
  1. Trần Hồng Minh *(Trưởng nhóm)*
  2. Hồ Thị Thuận
  3. Lê Thị Nga
  4. Phạm Ngọc Tú
  5. Lê Thanh Diệu
* **Giảng viên hướng dẫn (Approved by)**: ThS. Nguyễn Quang Ánh

---

## 1. MỤC TIÊU VÀ PHẠM VI (GOALS & SCOPE)

### ▪ Mục tiêu của tài liệu:
Tài liệu này chuyển hóa mô hình lý thuyết DSS (Hệ thống Hỗ trợ Ra Quyết định) và phương pháp Phân tích Thứ bậc AHP (*Analytic Hierarchy Process* của Thomas L. Saaty, 1980) thành thiết kế kiến trúc hệ thống phần mềm thực tế ở mức chức năng hoàn chỉnh. Tài liệu xác định tường minh các tác nhân tương tác (Actors), các nhóm chức năng nghiệp vụ, biểu đồ ca sử dụng (Use Case Diagram), đặc tả chi tiết các luồng xử lý và các dạng đầu ra hỗ trợ nhà quản lý đưa ra quyết định phân bổ hàng hóa và chiến lược xúc tiến bán lẻ chuẩn xác nhất.

### ▪ Khái quát kiến trúc vận hành hệ thống:
Hệ thống tiếp nhận dữ liệu bán hàng, lịch sử đơn hàng, giá vốn niêm yết và mức tồn kho từ các hệ thống giao dịch bên ngoài thông qua cơ sở dữ liệu Microsoft SQL Server (database: `DSS_SalesDB`). Hệ thống ứng dụng xử lý phân tích dữ liệu đa chiều OLAP (*Slicing, Dicing, Roll-up, Drill-down*); đánh giá đa tiêu chí và xếp hạng phương án sản phẩm bằng thuật toán AHP kết hợp chuẩn hóa theo hàm Benefit / Cost; mô phỏng các kịch bản biến động thị trường (*What-If Sensitivity Analysis*); và trực quan hóa toàn bộ kết quả lên Dashboard điều hành, Executive Summary, Cây cấu trúc thứ bậc phân cấp và Biểu đồ đối chiếu độ nhạy giúp ban quản lý nhanh chóng ra quyết định phê duyệt kế hoạch nhập hàng và phân bổ ngân sách marketing tối ưu.

### ▪ Phạm vi dự án (Project Scope):
* **Phạm vi In-scope**:
  * Tích hợp, làm sạch và kiểm tra toàn vẹn dữ liệu từ CSDL bán lẻ (`ORDERS`, `ORDER_DETAILS`, `PRODUCTS`, `CATEGORIES`).
  * Phân tích dữ liệu bán hàng đa chiều OLAP theo thời gian, chi nhánh và ngành hàng.
  * Thiết lập ma trận so sánh cặp Saaty 1-9 và kiểm tra độ nhất quán ($CR \le 0.10$).
  * Chuẩn hóa và tính điểm AHP cho danh mục sản phẩm theo 5 tiêu chí nghiệp vụ cốt lõi:
    * $C_1$: Biên lợi nhuận gộp (*Profit Margin* - Benefit)
    * $C_2$: Doanh thu kỳ vọng (*Expected Revenue* - Benefit)
    * $C_3$: Nhu cầu & Xu hướng thị trường (*Market Demand Trend* - Benefit)
    * $C_4$: Rủi ro tồn kho & Thời gian hoàn vốn (*Inventory Risk & Days of Supply* - Cost)
    * $C_5$: Chi phí tiếp thị & Khuyến mãi (*Marketing & Promotional Cost* - Cost)
  * Mô phỏng độ nhạy What-if với 3 kịch bản vi mô/vĩ mô (*Bùng nổ Gaming +40%, Đứt gãy cung ứng, Lạm phát thắt chặt*) và tùy biến tham số linh hoạt.
  * Tổng hợp Dashboard, phân loại phương án (*Selected / UnderReview / Rejected*), tự động sinh khuyến nghị số lượng nhập hàng dựa trên lead time, chu kỳ bổ sung 30 ngày và xuất báo cáo thẩm định DSS.
* **Phạm vi Out-of-scope**:
  * Hệ thống DSS không trực tiếp đóng vai trò máy tính tiền (POS) tại quầy bán lẻ, không tự động trừ hàng trong kho vật lý, không quản lý đội xe logistics giao hàng thực tế và không tự động thay đổi giá niêm yết trên website thương mại điện tử.
  * Mọi đề xuất số lượng đặt hàng và phân bổ ngân sách từ DSS đóng vai trò công cụ tham mưu cấp cao; việc triển khai vật lý sẽ do cán bộ có thẩm quyền lập phiếu đặt hàng hoặc kế hoạch khuyến mãi trên hệ thống POS/ERP hoặc WMS/ERP của doanh nghiệp.

---

## 2. XÁC ĐỊNH ACTOR (TÁC NHÂN TƯƠNG TÁC)

Actor là người dùng hoặc hệ thống bên ngoài tương tác trực tiếp hoặc gián tiếp với hệ thống DSS để cung cấp dữ liệu đầu vào hoặc khai thác các chức năng ra quyết định. Trong hệ thống *DSS Retail Decision Studio 2026*, các tác nhân được xác định như sau:

| Actor (Tác nhân) | Vai trò & Trách nhiệm trong Hệ thống DSS |
| :--- | :--- |
| **Admin** *(Quản trị viên)* | Quản lý tài khoản người dùng và phân quyền truy cập (`Admin`, `Analyst`, `Manager`) lưu trữ tại bảng `USERS`; tiếp nhận, làm sạch dữ liệu và kiểm tra ràng buộc toàn vẹn khóa ngoại CSDL; sao lưu và phục hồi dữ liệu; giám sát kết nối CSDL SQL Server (`DSS_SalesDB`) và quản lý cấu hình hệ thống. |
| **Data Analyst** *(Chuyên viên phân tích)* | Khai thác dữ liệu OLAP đa chiều; xây dựng và thiết lập trọng số 5 tiêu chí đánh giá; nhập ma trận so sánh cặp Saaty 1-9; kiểm tra tỷ số nhất quán $CR$; thực thi mô hình DSS để tính toán điểm số chuẩn hóa và xếp hạng sản phẩm; thiết lập các kịch bản mô phỏng What-if (độ nhạy thị trường) và đề xuất phương án tối ưu lên cấp quản lý. |
| **Manager** *(Quản lý / Giám đốc)* | Khai thác Dashboard chỉ số điều hành, xem xét bảng xếp hạng sản phẩm và báo cáo tổng hợp (*Executive Summary*); đối chiếu kết quả mô phỏng các kịch bản độ nhạy thị trường; xem xét đề xuất phân bổ ngân sách marketing và số lượng hàng đặt bổ sung; đưa ra quyết định phê duyệt (`Selected`), xem xét thêm (`UnderReview`) hoặc từ chối loại bỏ (`Rejected`). |
| **POS/ERP** *(Hệ thống ngoài)* | Hệ thống bán hàng tại điểm bán (Point of Sale) hoặc hệ thống hoạch định nguồn lực doanh nghiệp bên ngoài. Đóng vai trò cung cấp dữ liệu giao dịch lịch sử, thông tin đơn hàng (`ORDERS`), chi tiết hóa đơn (`ORDER_DETAILS`), doanh thu bán lẻ, mã SKU và danh mục sản phẩm vào kho dữ liệu của DSS. |
| **WMS/ERP** *(Hệ thống ngoài)* | Hệ thống quản lý kho hàng (Warehouse Management System). Cung cấp dữ liệu tồn kho hiện tại (`stock_quantity`), ngưỡng an toàn tối thiểu (`min_stock_level`), thời gian giao hàng dự kiến (`lead_time_days`); tiếp nhận thông tin số lượng hàng hóa đề xuất bổ sung từ kết quả phê duyệt DSS để nhân viên kho tiến hành đặt hàng thủ công. |

---

## 3. XÁC ĐỊNH CHỨC NĂNG NGHIỆP VỤ (BẢNG F01 – F06)

Hệ thống DSS được phân rã thành 6 nhóm chức năng nghiệp vụ then chốt (F01 – F06), kết nối chặt chẽ từ khâu tiếp nhận dữ liệu thô đến khâu ban hành quyết định quản trị chiến lược:

| Mã | Nhóm chức năng | Chức năng cụ thể (Matching Kiến trúc Thực tế) |
| :---: | :--- | :--- |
| **F01** | **Quản lý dữ liệu bán hàng & CSDL** | Tích hợp dữ liệu từ SQL Server / POS / ERP; thực hiện ETL làm sạch dữ liệu giao dịch; kiểm tra tính toàn vẹn và khử trùng lặp đơn hàng; cập nhật thông số giá vốn, giá bán, mức tồn kho an toàn và thời gian chờ nhập hàng (lead time). |
| **F02** | **Phân tích OLAP bán hàng đa chiều** | Xoay/Cắt lát dữ liệu (*Slicing/Dicing*) theo Chi nhánh và Ngành hàng; Cuộn lên/Khoan sâu (*Roll-up/Drill-down*) theo chiều Thời gian (Năm, Quý, Tháng, Ngày); tổng hợp doanh số bán lẻ, số lượng xuất kho và đóng góp lợi nhuận theo SKU. |
| **F03** | **Đánh giá & Xếp hạng sản phẩm (Mô hình AHP)** | Thiết lập ma trận so sánh cặp Saaty 1-9 cho 5 tiêu chí nghiệp vụ ($C_1 - C_5$); kiểm tra độ nhất quán AHP ($CR \le 0.10$); tự động chuẩn hóa điểm số thô theo hàm Benefit ($v/v_{\max}$) và Cost ($v_{\min}/v$); tổng hợp vector điểm số AHP; xếp hạng tự động và phân loại phương án: Tối ưu nhất (`Selected`), Xem xét (`UnderReview`), Loại bỏ (`Rejected`); tính toán số lượng đề xuất nhập hàng dựa trên lead time, chu kỳ bổ sung 30 ngày và phân bổ ngân sách marketing. |
| **F04** | **Mô phỏng độ nhạy What-if** | Cung cấp 3 kịch bản thị trường vĩ mô định sẵn: Kịch bản 1 (*Bùng nổ Gaming +40%*), Kịch bản 2 (*Đứt gãy cung ứng - Tăng rủi ro vốn*), Kịch bản 3 (*Lạm phát - Ưu tiên biên lợi nhuận cao*); cho phép tùy biến trực tiếp tham số Nhu cầu ($C_3$), Biên lợi nhuận ($C_1$), Rủi ro vốn ($C_4$) trên từng sản phẩm; tái tính toán tức thời điểm số AHP mới; đối chiếu trực quan bằng Biểu đồ đường (*Delta Chart*) và Sơ đồ cây cấu trúc thứ bậc phân cấp có mũi tên định hướng. |
| **F05** | **Báo cáo và đề xuất quyết định** | Hiển thị Dashboard trực quan (Thẻ KPI điều hành, Bảng xếp hạng chi tiết, Biểu đồ cột tỷ trọng điểm số AHP); sinh Báo cáo tóm tắt điều hành (*Executive Summary*) tự động dựa trên số liệu thực tế; đưa ra khuyến nghị hành động (*Nhập hàng tối ưu, Nhập thăm dò, Xả tồn/Giảm quy mô*); hỗ trợ xuất báo cáo thẩm định phục vụ họp giao ban. |
| **F06** | **Quản lý cấu hình & Thẩm định động** | Quản lý danh mục tiêu chí đánh giá, ma trận so sánh cặp; cấu hình ngưỡng điểm phê duyệt (Approve Threshold = 0.60) và chu kỳ đặt hàng (30 ngày); cho phép thêm sản phẩm mới trực tiếp vào hệ thống (Tab 4) và kích hoạt chạy lại mô hình DSS tự động mà không cần khởi động lại ứng dụng. |

### ▪ Luồng xử lý nghiệp vụ tổng quát của hệ thống DSS:
$$\text{Dữ liệu giao dịch & kho (SQL Server)} \longrightarrow \text{Kiểm tra, làm sạch \& chuẩn hóa ETL} \longrightarrow \text{Phân tích đa chiều OLAP} \longrightarrow \text{Ma trận Saaty \& Kiểm tra CR } (\le 10\%)$$
$$\longrightarrow \text{Chuẩn hóa đa tiêu chí } (C_1-C_5) \text{ \& Tính điểm AHP} \longrightarrow \text{Mô phỏng What-if \& Độ nhạy} \longrightarrow \text{Dashboard, Executive Summary \& Đề xuất phân bổ} \longrightarrow \text{Manager phê duyệt}$$

---

## 4. THIẾT KẾ BFD & USE CASE DIAGRAM

### 4.3. Vẽ BFD (Business Function Diagram)
Biểu đồ phân rã chức năng BFD thể hiện cấu trúc phân cấp từ mục tiêu kinh doanh tổng thể của hệ thống DSS xuống các nhóm chức năng nghiệp vụ cấp 1 (F01 đến F06) và các mô-đun chức năng con. BFD tập trung phân rã "Hệ thống làm những công việc gì", hoàn toàn độc lập và không phụ thuộc vào thứ tự thời gian xử lý.

> **[GIỮ NGUYÊN HÌNH ẢNH SƠ ĐỒ BFD TẠI ĐÂY - KHÔNG THAY ĐỔI HÌNH VẼ]**

---

### 4.4. Vẽ Use Case Diagram

#### 4.4.1. Bảng phân công Use Case theo Actor
| Actor | Danh sách Use Case chính phụ trách |
| :--- | :--- |
| **Admin** | Quản lý dữ liệu bán hàng; Kiểm tra/Làm sạch dữ liệu; Quản lý tài khoản & Cấu hình tham số DSS. |
| **Data Analyst** | Phân tích OLAP bán hàng; Thiết lập tiêu chí & Trọng số Saaty; Đánh giá & Xếp hạng sản phẩm AHP; Thêm sản phẩm mới thẩm định động; Chạy DSS Engine; Mô phỏng What-if độ nhạy. |
| **Manager** | Xem Dashboard & Báo cáo tổng hợp; Xem bảng xếp hạng phương án; Xem đề xuất kế hoạch khuyến mãi; Xem đề xuất số lượng phân bổ hàng hóa & ngân sách; Đưa ra quyết định phê duyệt áp dụng đề xuất. |
| **POS/ERP** | Cung cấp dữ liệu giao dịch bán lẻ, doanh số lịch sử, giá bán niêm yết và danh mục sản phẩm. |
| **WMS/ERP** | Cung cấp số liệu tồn kho thực tế, ngưỡng tồn an toàn, lead time; tiếp nhận kế hoạch đặt hàng phân bổ khi quản lý phê duyệt. |

#### 4.4.2. Sơ đồ Use Case tổng quát
Sơ đồ thể hiện biên hệ thống (*System Boundary*), các Actor tương tác, các Use Case nghiệp vụ cốt lõi và các mối quan hệ cấu trúc `<<include>>`, `<<extend>>` theo chuẩn ký pháp UML.

> **[GIỮ NGUYÊN HÌNH ẢNH SƠ ĐỒ USE CASE TỔNG QUÁT TẠI ĐÂY - KHÔNG THAY ĐỔI HÌNH VẼ]**

#### 4.4.3. Giải thích các mối quan hệ trong Use Case Diagram
* **Quan hệ `<<include>>` (Bao hàm bắt buộc)**: Được áp dụng khi một Use Case chính luôn luôn phải thực thi các hành vi của Use Case con để hoàn thành mục tiêu. Cụ thể trong hệ thống: Khi thực hiện "Mô phỏng What-if", hệ thống bắt buộc phải `<<include>>` "Ước tính doanh thu / lợi nhuận dự kiến" và "So sánh các kịch bản độ nhạy". Tương tự, ca sử dụng "Quản lý dữ liệu" bắt buộc phải `<<include>>` "Kiểm tra và làm sạch dữ liệu" trước khi nạp vào CSDL; "Đánh giá và xếp hạng sản phẩm" bắt buộc phải `<<include>>` "Chạy mô hình AHP".
* **Quan hệ `<<extend>>` (Mở rộng có điều kiện)**: Được sử dụng khi hành vi bổ sung chỉ được kích hoạt tùy theo nhu cầu hoặc điều kiện của người dùng tại điểm mở rộng (*extension point*). Ví dụ: Từ Use Case trung tâm "Xem Dashboard / Báo cáo", Manager có thể tùy chọn mở rộng `<<extend>>` sang "Xem đề xuất kế hoạch khuyến mãi" hoặc "Xem đề xuất phân bổ hàng hóa & ngân sách" khi cần thẩm định chi tiết.
* **Đường kết hợp Actor – Use Case**: Đường nối thẳng xác định Actor nào có quyền truy cập, tương tác hoặc kích hoạt Use Case tương ứng. Các hệ thống ngoài (POS/ERP, WMS/ERP) đóng vai trò hỗ trợ cung cấp dữ liệu hoặc tiếp nhận báo cáo đầu ra.
* **Nguyên tắc nghiệp vụ DSS**: Hệ thống DSS đóng vai trò công cụ hỗ trợ ra quyết định thông minh (*Decision Support*), không can thiệp trực tiếp làm biến đổi dữ liệu giao dịch kế toán hoặc trừ tồn kho tự động trên các hệ thống nghiệp vụ cốt lõi bên ngoài.

---

### 4.5. Đặc tả Chi Tiết Use Case (UC01 – UC05)

#### UC01 – Quản lý dữ liệu bán hàng & Kho vận
| Thuộc tính | Mô tả chi tiết (Matching Hệ thống Thực tế) |
| :--- | :--- |
| **Actor chính** | Admin (Quản trị viên) |
| **Actor phụ** | POS/ERP, WMS/ERP |
| **Mục tiêu** | Đảm bảo nguồn dữ liệu đầu vào sạch, đầy đủ, không trùng lặp và sẵn sàng cho các mô-đun phân tích AHP và OLAP. |
| **Điều kiện tiên quyết** | Hệ thống CSDL SQL Server (`DSS_SalesDB`) hoạt động bình thường, tài khoản Admin đăng nhập thành công. |
| **Dữ liệu đầu vào** | Bảng dữ liệu giao dịch bán hàng (`ORDERS`, `ORDER_DETAILS`), danh mục sản phẩm (`PRODUCTS`), phân loại (`CATEGORIES`), giá vốn, giá bán, tồn kho và lead time. |
| **Luồng chính** | 1. Admin kích hoạt chức năng nạp dữ liệu từ CSDL.<br>2. Hệ thống kiểm tra cấu trúc schema, kiểu dữ liệu và ràng buộc khóa ngoại (*foreign key*).<br>3. Tự động phát hiện dữ liệu thiếu sót, trùng mã sản phẩm/đơn hàng hoặc giá bán không hợp lệ ($\le 0$).<br>4. Thực thi ETL làm sạch, loại bỏ bản ghi lỗi và ghi nhận vào cơ sở dữ liệu quan hệ.<br>5. Hệ thống hiển thị thông báo trạng thái đồng bộ dữ liệu thành công. |
| **Luồng ngoại lệ** | Nếu kết nối CSDL SQL Server thất bại hoặc file dữ liệu nguồn sai định dạng, hệ thống ghi nhật ký lỗi (*error log*) và thông báo yêu cầu kiểm tra chuỗi kết nối. |
| **Đầu ra** | Bộ dữ liệu sạch, toàn vẹn được lưu trong `DSS_SalesDB`, sẵn sàng cung cấp cho mô-đun OLAP và AHP. |

#### UC02 – Phân tích OLAP bán hàng đa chiều
| Thuộc tính | Mô tả chi tiết (Matching Hệ thống Thực tế) |
| :--- | :--- |
| **Actor chính** | Data Analyst (Chuyên viên phân tích) |
| **Actor phụ** | Manager |
| **Mục tiêu** | Cung cấp cái nhìn toàn cảnh và phân tích chuyên sâu về hiệu quả kinh doanh bán lẻ theo nhiều chiều thông tin. |
| **Điều kiện tiên quyết** | Dữ liệu bán hàng lịch sử đã được làm sạch và nạp đầy đủ vào hệ thống. |
| **Dữ liệu đầu vào** | Khoảng thời gian phân tích, danh mục ngành hàng/SKU, các chiều phân tích (Thời gian, Sản phẩm, Chi nhánh), số lượng bán và doanh thu. |
| **Luồng chính** | 1. Analyst chọn khoảng thời gian cần phân tích (Quý, Tháng, Năm).<br>2. Chọn chiều phân tích: cắt lát (*Slicing*) theo chi nhánh, khoan sâu (*Drill-down*) từ Tháng xuống Ngày, hoặc xoay chiều (*Pivot*) theo SKU.<br>3. Hệ thống xử lý tổng hợp dữ liệu OLAP tự động theo thời gian thực.<br>4. Hiển thị báo cáo doanh số, số lượng bán, cơ cấu lợi nhuận gộp và vòng quay hàng tồn kho. |
| **Luồng ngoại lệ** | Nếu không có dữ liệu bán hàng trong khoảng thời gian hoặc chi nhánh đã chọn, hệ thống thông báo trạng thái không tìm thấy dữ liệu và gợi ý mở rộng bộ lọc. |
| **Đầu ra** | Báo cáo đa chiều OLAP, biểu đồ cơ cấu doanh thu và các chỉ số thống kê phục vụ việc gán điểm tiêu chí cho mô hình DSS. |

#### UC03 – Đánh giá và xếp hạng sản phẩm (Mô hình AHP Saaty)
| Thuộc tính | Mô tả chi tiết (Matching Hệ thống Thực tế) |
| :--- | :--- |
| **Actor chính** | Data Analyst (Chuyên viên phân tích) |
| **Actor phụ** | Manager |
| **Mục tiêu** | Ứng dụng phương pháp Phân tích Thứ bậc AHP của Thomas L. Saaty để xác định sản phẩm tối ưu nhất, xếp hạng danh mục và đề xuất số lượng hàng nhập bổ sung. |
| **Điều kiện tiên quyết** | Dữ liệu danh mục sản phẩm (`PRODUCTS`) và các chỉ số tiêu chí (`ALTERNATIVE_SCORES`) đã có trong CSDL; ma trận so sánh cặp đã được thiết lập. |
| **Dữ liệu đầu vào** | Ma trận so sánh cặp Saaty 1-9 giữa 5 tiêu chí ($C_1 - C_5$), điểm đánh giá thô của từng sản phẩm trên 5 tiêu chí, chu kỳ đặt hàng (30 ngày), lead time và tồn an toàn. |
| **Luồng chính** | 1. Analyst thiết lập ma trận so sánh cặp Saaty cho 5 tiêu chí: Biên lợi nhuận ($C_1$), Doanh thu kỳ vọng ($C_2$), Nhu cầu thị trường ($C_3$), Rủi ro tồn kho ($C_4$), Chi phí MKT ($C_5$).<br>2. Hệ thống tính toán vector trọng số $w_i$, Eigenvalue $\lambda_{\max}$, Chỉ số nhất quán $CI = (\lambda_{\max} - n)/(n - 1)$ và Tỷ số nhất quán $CR = CI / RI$.<br>3. Kiểm tra điều kiện nhất quán Saaty: $CR \le 0.10$. Nếu hợp lệ, chuyển sang bước chuẩn hóa điểm.<br>4. Chuẩn hóa ma trận điểm thô: Tiêu chí Benefit lấy $v / v_{\max}$; Tiêu chí Cost lấy $v_{\min} / v$.<br>5. Nhân vector trọng số với ma trận chuẩn hóa để tính Điểm tổng hợp AHP (*Final Score*) cho từng sản phẩm: $Score_j = \sum w_i \times s_{ij}$.<br>6. Tự động xếp hạng và phân loại quyết định: Hạng 1 ➔ `Selected` (Tối ưu nhất); Điểm $\ge 0.60$ ➔ `UnderReview` (Khả quan); Điểm $< 0.60$ ➔ `Rejected` (Loại bỏ).<br>7. Tự động tính số lượng nhập hàng đề xuất: $Q_{suggested} = DailyDemand \times (LeadTime + 30) + MinStock - Stock$; phân bổ tỷ trọng ngân sách marketing theo điểm số AHP.<br>8. Lưu kết quả xếp hạng vào bảng `DECISION_RESULTS` và hiển thị lên giao diện. |
| **Luồng ngoại lệ** | Nếu ma trận so sánh cặp không nhất quán ($CR > 0.10$), hệ thống cảnh báo đỏ và yêu cầu Analyst điều chỉnh lại các giá trị so sánh trước khi tính điểm. |
| **Đầu ra** | Vector trọng số tiêu chí ($w_i$), giá trị $CR$, Bảng xếp hạng điểm số AHP, trạng thái quyết định và khuyến nghị số lượng nhập hàng / phân bổ ngân sách. |

#### UC04 – Mô phỏng độ nhạy What-if (Sensitivity Analysis)
| Thuộc tính | Mô tả chi tiết (Matching Hệ thống Thực tế) |
| :--- | :--- |
| **Actor chính** | Data Analyst (Chuyên viên phân tích) |
| **Actor phụ** | Manager |
| **Mục tiêu** | Đánh giá mức độ nhạy cảm và sự thay đổi thứ hạng của các phương án khi các điều kiện thị trường hoặc tham số tiêu chí biến động. |
| **Điều kiện tiên quyết** | Hệ thống đã có kết quả tính toán AHP cơ sở của phiên hiện tại. |
| **Dữ liệu đầu vào** | Kịch bản thị trường lựa chọn (*Bùng nổ Gaming +40%, Đứt gãy cung ứng, Lạm phát*) hoặc các tham số tùy chỉnh: Nhu cầu ($C_3$), Biên lợi nhuận ($C_1$), An toàn vốn ($C_4$). |
| **Luồng chính** | 1. Analyst chọn kịch bản định sẵn hoặc chọn sản phẩm và điều chỉnh tham số độ nhạy trên giao diện.<br>2. Nhấn nút "Áp Dụng Tham Số" để kích hoạt mô-đun What-If.<br>3. Hệ thống tái chuẩn hóa ma trận điểm thô mới dựa trên các tham số đã biến động.<br>4. Tái tính toán điểm số tổng hợp AHP mới và xác định lại thứ hạng cho toàn bộ danh mục sản phẩm.<br>5. Hiển thị đối chiếu trực quan bằng Biểu đồ đường phân tích độ nhạy (*Delta Chart*) và Cây cấu trúc thứ bậc phân cấp có mũi tên định hướng.<br>6. Đưa ra nhận xét tự động về mức độ dịch chuyển thứ hạng của sản phẩm dẫn đầu. |
| **Luồng ngoại lệ** | Nếu tham số nhập vào nằm ngoài biên độ hợp lệ (ví dụ: nhu cầu $< 0$ hoặc $> 1.0$), hệ thống hiển thị cảnh báo và giữ nguyên giá trị hợp lệ gần nhất. |
| **Đầu ra** | Bảng so sánh đối chiếu Điểm AHP cũ vs Điểm AHP mới, Biểu đồ đường độ nhạy và nhận xét tác động chiến lược kinh doanh. |

#### UC05 – Xem báo cáo và đề xuất quyết định
| Thuộc tính | Mô tả chi tiết (Matching Hệ thống Thực tế) |
| :--- | :--- |
| **Actor chính** | Manager (Quản lý / Giám đốc) |
| **Actor phụ** | Data Analyst |
| **Mục tiêu** | Hỗ trợ nhà quản lý có đầy đủ căn cứ khoa học và số liệu định lượng để phê duyệt kế hoạch kinh doanh và phân bổ hàng hóa. |
| **Điều kiện tiên quyết** | Hệ thống DSS đã hoàn tất việc chạy mô hình AHP và cập nhật dữ liệu vào bảng `DECISION_RESULTS`. |
| **Dữ liệu đầu vào** | Kết quả xếp hạng AHP, báo cáo phân tích độ nhạy What-if, số liệu tồn kho thực tế và doanh số lịch sử. |
| **Luồng chính** | 1. Manager đăng nhập hệ thống và mở Dashboard ra quyết định.<br>2. Xem thẻ tóm tắt KPI điều hành, tỷ số nhất quán $CR$ và sản phẩm dẫn đầu (*Top Choice*).<br>3. Xem bảng xếp hạng chi tiết có mã màu trực quan kèm nhận xét phân tích điều hành chuyên sâu (*Executive Comments*).<br>4. Xem biểu đồ cột trực quan hóa phân bổ điểm số AHP của toàn bộ danh mục sản phẩm.<br>5. Xem xét các khuyến nghị hành động cụ thể: Nhập hàng tối ưu (Hạng 1), Nhập thăm dò (Hạng 2), Xả tồn/Giảm quy mô (Hạng 3 trở đi).<br>6. Manager phê duyệt, điều chỉnh hoặc từ chối đề xuất; xuất báo cáo thẩm định DSS phục vụ lưu trữ. |
| **Luồng ngoại lệ** | Nếu dữ liệu phiên quyết định chưa được tính toán hoặc có sản phẩm mới chưa thẩm định, hệ thống thông báo trạng thái và hiển thị nút "Chạy Lại Mô Hình". |
| **Đầu ra** | Quyết định kinh doanh được phê duyệt chính thức; báo cáo thẩm định xuất file phục vụ triển khai thực tế trên POS/ERP và WMS. |

---

## 4.6. VẼ FLOWCHART (LƯU ĐỒ QUY TRÌNH XỬ LÝ)

### 4.6.1. Flowchart tổng quát – Quy trình xử lý DSS toàn diện
Lưu đồ mô tả chu trình khép kín từ lúc tiếp nhận dữ liệu từ các hệ thống POS/ERP và WMS/ERP, thực hiện kiểm tra làm sạch, phân tích OLAP, tính toán AHP đa tiêu chí, phân tích kịch bản What-If, cho đến khi Dashboard tạo báo cáo và Manager xem xét phê duyệt quyết định cuối cùng:

> **[GIỮ NGUYÊN HÌNH ẢNH SƠ ĐỒ FLOWCHART TỔNG QUÁT TẠI ĐÂY - KHÔNG THAY ĐỔI HÌNH VẼ]**

*Thuyết minh luồng xử lý*: Dữ liệu giao dịch khi nhận từ hệ thống ngoài sẽ trải qua bước rẽ nhánh kiểm tra tính hợp lệ. Nếu dữ liệu không hợp lệ (sai định dạng, thiếu giá vốn, trùng mã), luồng xử lý sẽ quay về bước thông báo lỗi và yêu cầu làm sạch dữ liệu trước khi cho phép đi tiếp vào kho lưu trữ chuẩn hóa. Sau đó, quy trình đi tuần tự qua các bước phân tích OLAP, AHP và What-if trước khi trình diễn kết quả cho nhà quản lý.

---

### 4.6.2. Flowchart quy trình đánh giá và xếp hạng sản phẩm (Mô hình AHP)
Lưu đồ mô tả chi tiết thuật toán nội bộ của bộ máy ra quyết định DSS Engine: nhận diện danh mục sản phẩm, kiểm tra điều kiện đủ dữ liệu, chuẩn hóa tiêu chí theo loại Benefit / Cost, nhân trọng số AHP, tính điểm tổng hợp, xếp hạng và phân loại tiềm năng khuyến mãi:

> **[GIỮ NGUYÊN HÌNH ẢNH SƠ ĐỒ FLOWCHART ĐÁNH GIÁ & XẾP HẠNG TẠI ĐÂY - KHÔNG THAY ĐỔI HÌNH VẼ]**

> **Lưu ý nghiệp vụ DSS cốt lõi**: Tiêu chí/ngưỡng xác định sản phẩm có tiềm năng khuyến mãi cần được quy định chặt chẽ trong mô hình DSS. Sản phẩm bán chậm không mặc nhiên phải giảm giá; hệ thống cần đối chiếu đồng thời giữa mức tồn kho khả dụng, biên lợi nhuận, chi phí tiếp thị và nhu cầu thị trường. Nếu sản phẩm bán chậm do thiếu hàng (*out of stock*) hoặc do tính chất mùa vụ, hệ thống sẽ đề xuất bổ sung nguồn cung thay vì giảm giá làm xói mòn lợi nhuận.

---

### 4.6.3. Flowchart quy trình mô phỏng độ nhạy What-if
Lưu đồ mô tả các bước người dùng thử nghiệm các giả định thị trường: chọn sản phẩm, nhập thông số giả định (mức giảm giá, biến động nhu cầu, chi phí vốn), hệ thống kiểm tra tính hợp lệ của tham số, ước tính doanh thu và lợi nhuận dự kiến, lưu kịch bản và tiến hành so sánh đối chiếu đa kịch bản để tìm ra phương án phù hợp nhất:

> **[GIỮ NGUYÊN HÌNH ẢNH SƠ ĐỒ FLOWCHART MÔ PHỎNG WHAT-IF TẠI ĐÂY - KHÔNG THAY ĐỔI HÌNH VẼ]**

*Lưu ý phân tích định lượng*: Kết quả phân tích What-if là dự báo mô phỏng dựa trên dữ liệu lịch sử và các giả định đầu vào của chuyên viên phân tích, đóng vai trò công cụ kiểm thử độ vững chắc của quyết định trước những rủi ro thị trường bất ngờ, không phải là cam kết doanh thu hay lợi nhuận thực tế cố định.

---

## 7. TỔNG HỢP ĐẦU RA VÀ TIÊU CHÍ HOÀN THÀNH

Bảng tổng hợp đối soát các hạng mục công việc, sản phẩm chuyển giao (*deliverables*) và tiêu chí nghiệm thu thiết kế hệ thống thông tin hỗ trợ ra quyết định (DSS) của Nhóm 05:

| Hạng mục đầu ra | Nội dung đặc tả chi tiết | Kết quả nghiệm thu thực tế |
| :--- | :--- | :--- |
| **Danh sách Actor** | Xác định đầy đủ người dùng nội bộ (Admin, Data Analyst, Manager) và các hệ thống bên ngoài (POS/ERP, WMS/ERP) tương tác với DSS. | *Đã xác định đầy đủ 5 Actor và phân quyền chi tiết.* |
| **Danh sách chức năng** | Phân rã hệ thống thành 6 nhóm chức năng nghiệp vụ then chốt (F01 đến F06) kèm các chức năng con cụ thể. | *Đã lập bảng F01–F06 matching 100% ứng dụng thực tế.* |
| **BFD (Business Function Diagram)** | Phân rã cấu trúc chức năng tổng thể thành các nhóm chức năng con phân cấp, độc lập thời gian. | *Có sơ đồ cấu trúc phân rã hoàn chỉnh.* |
| **Use Case Diagram** | Biên hệ thống, các Actor, Use Case nghiệp vụ chính và các mối quan hệ `<<include>>`, `<<extend>>` theo chuẩn UML. | *Có sơ đồ tổng quan; đã rà soát ký pháp UML và giải thích quan hệ.* |
| **Use Case Specification** | Đặc tả cấu trúc chi tiết 5 Use Case cốt lõi: Actor, mục tiêu, điều kiện tiên quyết, dữ liệu vào/ra, luồng chính và ngoại lệ. | *Đặc tả đầy đủ 5 Use Case cốt lõi (UC01 – UC05) gắn với CSDL SQL Server và thuật toán AHP.* |
| **Flowchart tổng quát** | Lưu đồ tiến trình luồng xử lý toàn diện từ khâu nạp dữ liệu đến bước Manager xem xét và ban hành quyết định. | *Có sơ đồ quy trình DSS tổng quát hoàn chỉnh.* |
| **Flowchart từng quyết định** | Lưu đồ chi tiết hóa thuật toán đánh giá xếp hạng AHP Saaty (4.6.2) và quy trình mô phỏng độ nhạy What-If (4.6.3). | *Có đầy đủ 2 sơ đồ lưu đồ chi tiết hóa thuật toán DSS.* |

---

### KẾT LUẬN & ĐÁNH GIÁ TÍNH KHẢ THI CỦA HỆ THỐNG:
Tài liệu thiết kế hệ thống DSS của Nhóm 05 đã hoàn thành toàn diện các yêu cầu kỹ thuật của học phần **IS 402 A**. Mô hình lý thuyết AHP của Thomas L. Saaty kết hợp phân tích đa chiều OLAP và mô phỏng What-If không chỉ dừng lại ở mặt lý thuyết mà đã được lập trình hiện thực hóa hoàn chỉnh thành phần mềm ứng dụng **DSS Retail Decision Studio 2026** kết nối CSDL SQL Server thực tế. Hệ thống đảm bảo tính nhất quán logic cao, dữ liệu được chuẩn hóa chặt chẽ và giao diện trực quan, giải quyết xuất sắc bài toán hỗ trợ nhà quản lý đưa ra quyết định tối ưu hóa danh mục hàng hóa và chiến lược xúc tiến bán lẻ.
