# DUY TÂN UNIVERSITY – INTERNATIONAL SCHOOL
## HỌC PHẦN: IS 402 A – HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH (DSS)
**Ngày nghiệm thu kiểm thử: 28/10/2026**

---

# KẾ HOẠCH & BÁO CÁO KIỂM THỬ HỆ THỐNG (TEST PLAN & TEST CASES)
### HỆ THỐNG HỖ TRỢ RA QUYẾT ĐỊNH CHIẾN LƯỢC KHUYẾN MÃI & PHÂN BỔ HÀNG HÓA
**Ứng dụng Mô hình Phân tích Thứ bậc AHP (Saaty), C# .NET 10 WinForms và Cơ sở Dữ liệu SQL Server**

* **Nhóm sinh viên thực hiện (Nhóm 05)**:
  1. Trần Hồng Minh *(Trưởng nhóm - MSSV: 29210247142)*
  2. Lê Thị Nga *(MSSV: 29206361029)*
  3. Phạm Ngọc Tú *(MSSV: 29204347819)*
  4. Lê Thanh Diệu *(MSSV: 29204622434)*
  5. Hồ Thị Thuận *(MSSV: 29201164457)*
* **Giảng viên chủ trì (Approved by)**: ThS. Nguyễn Quang Ánh

---

## 1. THÔNG TIN DỰ ÁN VÀ MÔI TRƯỜNG KIỂM THỬ

| Thuộc tính | Chi tiết môi trường & Thông số hệ thống |
| :--- | :--- |
| **Tên dự án / Đề tài** | Hệ thống Hỗ trợ Ra Quyết định Chiến lược Khuyến mãi & Phân bổ Hàng hóa |
| **Mã học phần / Lớp** | IS 402 A – Hệ thống Thông tin Hỗ trợ Ra Quyết định (DSS) |
| **Đơn vị đào tạo** | Trường Đào tạo Quốc tế (International School), Đại học Duy Tân |
| **Phần mềm ứng dụng** | **DSS Retail Decision Studio 2026** (Phiên bản chính thức v2.6, Kiến trúc 6 Tab nghiệp vụ) |
| **Cơ sở dữ liệu** | **Microsoft SQL Server 2022 Express** (Instance: `localhost\SQLEXPRESS25`, Database: `DSS_SalesDB`) |
| **Nền tảng & Thư viện** | C# .NET 10 Windows Forms, Python 3.13 (DSS Core Engine), Microsoft.Data.SqlClient, NumPy, PyODBC |
| **Phạm vi kiểm thử** | Xác thực & Phân quyền, Toàn vẹn CSDL, Thuật toán AHP Saaty, Đề xuất nhập hàng, Mô phỏng What-If, OLAP & Báo cáo |

---

## 2. MỤC TIÊU VÀ QUY TẮC NGHIỆP VỤ KIỂM THỬ (TEST ORACLES)

Kế hoạch kiểm thử này được xây dựng dựa trên kiến trúc hệ thống thực tế đã hoàn thiện của Nhóm 05. Mọi kết quả kiểm thử (*Actual Result*) được đối chiếu trực tiếp với các quy tắc nghiệp vụ toán học sau:

* **Quy tắc R1 (Ma trận so sánh cặp AHP & Tỷ số nhất quán CR)**:  
  Ma trận so sánh cặp Saaty 5 tiêu chí ($C_1 - C_5$) phải thỏa mãn tính đối xứng đảo $a_{ij} = 1/a_{ji}$ và $a_{ii} = 1$. Vector trọng số $w_i$ tính bằng trung bình các hàng sau khi chuẩn hóa cột. Chỉ số nhất quán $CI = (\lambda_{\max} - n)/(n - 1)$ với $n = 5$, ngẫu nhiên $RI = 1.12$. Tỷ số nhất quán $CR = CI / RI$ phải $\le 0.10$ (10%) thì mô hình mới được chấp nhận ra quyết định.
* **Quy tắc R2 (Chuẩn hóa đa tiêu chí Benefit & Cost)**:  
  * Tiêu chí dạng Lợi ích (Benefit: $C_1$ Biên lợi nhuận, $C_2$ Doanh thu kỳ vọng, $C_3$ Nhu cầu thị trường): $norm = raw / max\_raw$.  
  * Tiêu chí dạng Chi phí/Rủi ro (Cost: $C_4$ Rủi ro tồn kho, $C_5$ Chi phí tiếp thị): $norm = min\_raw / raw$.  
  * Giá trị chuẩn hóa luôn nằm trong đoạn $[0.0000, 1.0000]$.
* **Quy tắc R3 (Điểm tổng hợp AHP & Phân loại quyết định)**:  
  Điểm tổng hợp $Score_j = \sum (w_i \times norm_{ij})$.  
  * Hạng 1: `Selected` (Phương án tối ưu nhất - Ưu tiên rót vốn & triển khai).  
  * Điểm $\ge 0.60$: `UnderReview` (Khả quan - Xem xét nhập thăm dò).  
  * Điểm $< 0.60$: `Rejected` (Không đạt hiệu quả đa tiêu chí - Cân nhắc xả tồn kho).
* **Quy tắc R4 (Công thức đề xuất đặt hàng & Phân bổ ngân sách)**:  
  * Số lượng nhập hàng đề xuất:  
    $$Q_{suggested} = DailyDemand \times (LeadTime + 30) + MinStock - StockQuantity$$  
    *(Làm tròn lên số nguyên dương)*.  
  * Tỷ trọng phân bổ ngân sách marketing = Điểm AHP của sản phẩm / Tổng điểm AHP của các sản phẩm được duyệt.
* **Quy tắc R5 (Bất biến mô phỏng What-If)**:  
  Mô phỏng phân tích độ nhạy What-If chỉ diễn ra trên bộ nhớ RAM tạm thời của phiên làm việc, tuyệt đối không được ghi đè làm thay đổi dữ liệu gốc trong CSDL SQL Server trừ khi người dùng chủ động bấm lưu.

---

## 3. BẢNG CHI TIẾT 30 CA KIỂM THỬ HỆ THỐNG (TEST CASES MATRIX)

| Mã TC | Phân hệ | Tên ca kiểm thử | Dữ liệu đầu vào | Các bước thực hiện | Kết quả mong đợi (Expected) | Trạng thái |
| :---: | :--- | :--- | :--- | :--- | :--- | :---: |
| **TC01** | Xác thực (Auth) | Đăng nhập với tài khoản hợp lệ | Username: `admin`, Password: hợp lệ, Role: Admin. | 1. Mở màn hình LoginForm.<br>2. Nhập username & password.<br>3. Bấm "Đăng Nhập Hệ Thống". | Đăng nhập thành công, mở SplashForm chuyển tiếp vào Form1; hiển thị đúng thông tin và badge vai trò Quản trị viên. | **☑ Pass** |
| **TC02** | Xác thực (Auth) | Đăng nhập sai mật khẩu | Username: `admin`, Password: sai. | 1. Nhập username đúng, password sai.<br>2. Bấm "Đăng Nhập". | Báo lỗi xác thực; mật khẩu băm SHA-256 không khớp; không cấp quyền truy cập vào Form1. | **☑ Pass** |
| **TC03** | Xác thực (Auth) | Đăng ký người dùng & chống trùng | Nhập username đã tồn tại trong CSDL. | 1. Bấm "Đăng ký tài khoản".<br>2. Nhập username trùng.<br>3. Bấm xác nhận. | Hệ thống cảnh báo username đã tồn tại trong bảng USERS; từ chối tạo bản ghi trùng lặp. | **☑ Pass** |
| **TC04** | Phân quyền (RBAC) | Kiểm soát quyền truy cập theo vai trò | Tài khoản vai trò `Analyst` vs `Manager`. | 1. Đăng nhập với tài khoản Analyst.<br>2. Kiểm tra các nút phê duyệt và cấu hình. | Chuyên viên chỉ có quyền chạy AHP/What-if; chỉ Manager mới có quyền phê duyệt đề xuất chính thức. | **☑ Pass** |
| **TC05** | CSDL (DatabaseGuard) | Kiểm tra kết nối SQL Server | Chuỗi kết nối SQL Server `localhost\SQLEXPRESS25`. | 1. Khởi động ứng dụng.<br>2. DatabaseGuard kiểm tra kết nối tới DSS_SalesDB. | Kết nối thành công; đèn báo trạng thái xanh; sẵn sàng truy vấn 11 bảng dữ liệu. | **☑ Pass** |
| **TC06** | CSDL (Integrity) | Bỏ trống trường bắt buộc khi thêm SP | Tab 4: Mã SKU để rỗng, Tên để rỗng. | 1. Mở Tab 4.<br>2. Điền giá bán nhưng bỏ trống SKU.<br>3. Bấm "Lưu Sản Phẩm & Thẩm Định". | Hiển thị cảnh báo lỗi tại các trường bắt buộc; không ghi bản ghi rỗng vào bảng PRODUCTS. | **☑ Pass** |
| **TC07** | CSDL (Integrity) | Chặn số âm cho giá bán và tồn kho | Giá bán = -50.000đ, Tồn kho = -10. | 1. Nhập giá trị âm vào ô Giá bán hoặc Tồn kho.<br>2. Bấm Lưu. | Hệ thống từ chối dữ liệu âm; thông báo yêu cầu giá trị phải là số dương hợp lệ. | **☑ Pass** |
| **TC08** | CSDL (Integrity) | Chống tràn số với giá trị cực lớn | Giá bán = 999.999.999.999đ. | 1. Nhập giá trị tiền tệ vượt giới hạn.<br>2. Bấm Lưu. | Kiểu dữ liệu DECIMAL(18,2) xử lý an toàn, không gây crash ứng dụng hay lỗi tràn số (overflow). | **☑ Pass** |
| **TC09** | CSDL (Integrity) | Chặn trùng mã SKU trong danh mục | Mã SKU trùng: `PRD_S24_ULTRA`. | 1. Nhập sản phẩm mới với mã SKU đã tồn tại.<br>2. Bấm Lưu. | Ràng buộc Unique Constraint trong CSDL kích hoạt; thông báo mã SKU đã tồn tại. | **☑ Pass** |
| **TC10** | AHP Engine | Tính vector trọng số từ ma trận Saaty | Ma trận so sánh cặp 5 tiêu chí C1-C5. | 1. Mở Tab 1.<br>2. Nạp ma trận Saaty 5x5.<br>3. Bấm tính toán trọng số. | Tính chính xác vector wi: C1=41.62%, C2=26.18%, C3=16.11%, C4=9.86%, C5=6.24%. Tổng wi = 1.0000. | **☑ Pass** |
| **TC11** | AHP Engine | Kiểm tra Tỷ số nhất quán ($CR \le 0.10$) | Ma trận so sánh cặp phiên 1. | 1. Tính $\lambda_{\max}$, CI = $(\lambda_{\max} - 5)/4$, CR = CI / 1.12. | $\lambda_{\max} = 5.0683$, CI = 0.0171, CR = 0.0152 (1.52%) $\le$ 10%. Xác nhận "ĐẠT CHUẨN NHẤT QUÁN". | **☑ Pass** |
| **TC12** | AHP Engine | Cảnh báo khi ma trận không nhất quán | Nhập giá trị mâu thuẫn: C1 > C2, C2 > C3 nhưng C3 > C1 rất lớn. | 1. Nhập các giá trị mâu thuẫn.<br>2. Bấm tính trọng số. | Hệ thống cảnh báo đỏ CR > 0.10; giải thích ma trận thiếu nhất quán và chặn chạy mô hình. | **☑ Pass** |
| **TC13** | AHP Engine | Chuẩn hóa ma trận Benefit ($v/v_{\max}$) | C1 (Biên LN): max = 94.95%; S24 Ultra raw = 26.29%. | 1. Thực thi NormalizeScores cho C1.<br>2. Kiểm tra giá trị norm. | Điểm chuẩn hóa = 26.29 / 94.95 = 0.2769; không âm, không vượt quá 1.0000. | **☑ Pass** |
| **TC14** | AHP Engine | Chuẩn hóa ma trận Cost ($v_{\min}/v$) | C4 (Rủi ro tồn): min = 15 ngày; Dell G15 raw = 30 ngày. | 1. Thực thi NormalizeScores cho C4.<br>2. Kiểm tra giá trị norm. | Điểm chuẩn hóa = 15 / 30 = 0.5000; giá trị càng nhỏ thì điểm chuẩn hóa càng cao (đúng quy tắc Cost). | **☑ Pass** |
| **TC15** | AHP Engine | Xếp hạng tổng hợp AHP toàn bộ 9 SP | Toàn bộ 9 sản phẩm và ma trận điểm thô trong CSDL. | 1. Bấm "Chạy Mô Hình DSS Ngay".<br>2. Mở Tab 2 xem bảng xếp hạng. | Hệ thống tính đúng điểm AHP cho cả 9/9 sản phẩm; sắp xếp thứ hạng giảm dần từ Hạng 1 đến Hạng 9. | **☑ Pass** |
| **TC16** | Quyết định (Outcome) | Phân loại Hạng 1 là `Selected` | Sản phẩm đứng đầu (Máy ảnh canon, Điểm = 0.8210). | 1. Quan sát cột Quyết định tại dòng Hạng 1. | Gán nhãn `Selected` (Màu xanh lục/Vàng kim); sinh khuyến nghị "★ NHẬP HÀNG TỐI ƯU". | **☑ Pass** |
| **TC17** | Quyết định (Outcome) | Phân loại điểm $\ge 0.60$ là `UnderReview` | Samsung Galaxy S24 Ultra (Điểm = 0.6413). | 1. Quan sát dòng Hạng 2 có điểm $\ge 0.60$. | Gán nhãn `UnderReview` (Màu xanh dương); sinh khuyến nghị "• NHẬP THĂM DÒ". | **☑ Pass** |
| **TC18** | Quyết định (Outcome) | Phân loại điểm $< 0.60$ là `Rejected` | Các sản phẩm điểm $< 0.60$ (Apple Watch, Loa Bluetooth, Camera 360). | 1. Quan sát các dòng điểm $< 0.60$. | Gán nhãn `Rejected` (Màu xám/đỏ); sinh khuyến nghị "• XẢ TỒN / GIẢM QUY MÔ". | **☑ Pass** |
| **TC19** | Đề xuất (Stock Qty) | Tính số lượng đề xuất đặt hàng | DailyDemand = 2.5, LeadTime = 7, Chu kỳ = 30, MinStock = 10, Tồn kho = 15. | 1. Chạy thuật toán đề xuất nhập kho.<br>2. Đối chiếu số lượng gợi ý. | $Q_{suggested} = 2.5 \times (7 + 30) + 10 - 15 = 87.5 \rightarrow 88$ sản phẩm. Đúng công thức. | **☑ Pass** |
| **TC20** | Đề xuất (Budget) | Phân bổ ngân sách marketing theo điểm AHP | Tổng điểm các sản phẩm được duyệt (Selected + UnderReview). | 1. Kiểm tra tỷ trọng phân bổ trên Tab 2. | Ngân sách được phân bổ tỷ lệ thuận theo điểm số AHP của các phương án được duyệt. | **☑ Pass** |
| **TC21** | What-If | Kích hoạt Kịch bản 1 (Bùng nổ Gaming $+40\%$) | Bấm chọn Kịch bản 1 trên Tab 3. | 1. Mở Tab 3.<br>2. Nhấn nút "Kịch Bản 1: Bùng Nổ Gaming (+40%)". | Nhu cầu laptop gaming tăng 40%; điểm số Dell G15 tăng vọt; thứ hạng dịch chuyển tức thời. | **☑ Pass** |
| **TC22** | What-If | Kích hoạt Kịch bản 2 (Đứt gãy cung ứng) | Bấm chọn Kịch bản 2 trên Tab 3. | 1. Nhấn nút "Kịch Bản 2: Đứt Gãy Cung Ứng (Rủi Ro Vốn)". | Tiêu chí rủi ro C4 tăng cao; các sản phẩm có lead time dài bị giảm điểm AHP rõ rệt. | **☑ Pass** |
| **TC23** | What-If | Kích hoạt Kịch bản 3 (Lạm phát thắt chặt) | Bấm chọn Kịch bản 3 trên Tab 3. | 1. Nhấn nút "Kịch Bản 3: Lạm Phát - Ưu Tiên Biên Lợi Nhuận". | Trọng số ưu tiên biên lợi nhuận C1; các mặt hàng giá rẻ/biên mỏng bị hạ bậc. | **☑ Pass** |
| **TC24** | What-If | Tùy biến tham số độ nhạy trực tiếp | S24 Ultra: Nhu cầu ($C_3$) = 0.95, Biên LN ($C_1$) = 0.90. | 1. Nhập tham số tùy chỉnh.<br>2. Bấm "Áp Dụng Tham Số". | Biểu đồ đường (Delta Chart) hiển thị đường nét đứt màu đỏ đối chiếu độ dịch chuyển điểm số mới. | **☑ Pass** |
| **TC25** | What-If | Khôi phục dữ liệu gốc không ghi đè CSDL | Bấm "Khôi Phục Dữ Liệu Gốc". | 1. Chạy nhiều kịch bản What-If.<br>2. Nhấn "Khôi Phục Dữ Liệu Gốc".<br>3. Kiểm tra lại SQL Server. | Dữ liệu gốc trong bảng ALTERNATIVE_SCORES giữ nguyên vẹn 100%, không bị ô nhiễm dữ liệu. | **☑ Pass** |
| **TC26** | Thẩm định động | Thêm sản phẩm mới tự động chạy lại DSS | Nhập sản phẩm thứ 9 (iPad Gen 10) qua Tab 4. | 1. Nhập thông tin SP mới.<br>2. Bấm Lưu & Thẩm định. | Sản phẩm được lưu vào PRODUCTS và ALTERNATIVES; Tab 2 tự động cập nhật hiển thị đầy đủ 9 sản phẩm. | **☑ Pass** |
| **TC27** | Trực quan hóa | Hiển thị Sơ đồ cây AHP không bị khuất | Mở Sơ đồ cây cấu trúc thứ bậc (Tab 3) với 9 sản phẩm. | 1. Bấm nút "Sơ Đồ Cây Cấu Trúc Thứ Bậc".<br>2. Quan sát hàng phương án tầng 3. | Tất cả 9 thẻ sản phẩm hiển thị vừa vặn, không bị tràn màn hình; có mũi tên định hướng nối từ Mục tiêu ➔ Tiêu chí ➔ Phương án. | **☑ Pass** |
| **TC28** | OLAP Bán hàng | Cắt lát (Slicing) dữ liệu theo ngành hàng | Chọn Category = "Điện thoại & Tablet". | 1. Mở Tab 5 (OLAP).<br>2. Chọn danh mục.<br>3. Bấm Lọc. | Bảng hiển thị chính xác các đơn hàng thuộc nhóm điện thoại; tính đúng tổng doanh thu và số lượng xuất. | **☑ Pass** |
| **TC29** | Báo cáo (Report) | Xuất Báo cáo thẩm định DSS sang file Word | Mở Tab 6, bấm "Xuất Báo Cáo Thẩm Định". | 1. Mở Tab 6.<br>2. Nhấn nút xuất báo cáo Word. | Sinh thành công file tài liệu thẩm định chứa đầy đủ ma trận AHP, bảng xếp hạng và chữ ký phê duyệt. | **☑ Pass** |
| **TC30** | End-to-End | Quy trình ra quyết định hoàn chỉnh khép kín | Đăng nhập ➔ Kiểm tra CSDL ➔ Chạy AHP ➔ Xếp hạng ➔ What-If ➔ Phê duyệt ➔ Xuất báo cáo. | 1. Thực hiện tuần tự toàn bộ chu trình nghiệp vụ qua 6 Tab. | Toàn bộ luồng dữ liệu nhất quán từ SQL Server ➔ Giao diện C# ➔ Báo cáo đầu ra; không phát sinh bất kỳ lỗi ngoại lệ nào. | **☑ Pass** |

---

## 4. KỊCH BẢN DEMO VÀ CÂU HỎI PHẢN BIỆN BẢO VỆ ĐỒ ÁN

### ▪ Lời dẫn Demo đề xuất cho Nhóm 05:
> *"Kính thưa Thầy/Cô và Hội đồng, hệ thống của Nhóm 05 giải quyết bài toán tối ưu hóa chiến lược bán lẻ dựa trên mô hình Phân tích Thứ bậc AHP của Thomas L. Saaty. Chúng em kết nối cơ sở dữ liệu SQL Server thực tế (DSS_SalesDB) chứa lịch sử bán hàng và 9 dòng sản phẩm công nghệ. Tại Tab 1, hệ thống nạp ma trận so sánh cặp giữa 5 tiêu chí nghiệp vụ, tự động kiểm tra độ nhất quán đạt chuẩn ($CR = 0.0152 \le 0.10$). Tại Tab 2, hệ thống chuẩn hóa dữ liệu theo hàm Benefit/Cost, tổng hợp điểm và xếp hạng danh mục, đồng thời đưa ra đề xuất số lượng nhập hàng và phân bổ ngân sách marketing khoa học. Đặc biệt, tại Tab 3, hệ thống cung cấp công cụ mô phỏng độ nhạy What-If với 3 kịch bản thị trường và tùy biến tham số trực quan giúp nhà quản lý kiểm chứng độ vững chắc của quyết định trước những biến động bất ngờ của nền kinh tế."*

### ▪ Bộ câu hỏi phản biện cốt lõi và câu trả lời đã chuẩn bị:
1. **Câu 1: Vì sao đây là hệ thống Hỗ trợ Ra Quyết định (DSS) chứ không phải phần mềm quản lý kho hay bán hàng thông thường?**  
   *Trả lời*: Phần mềm quản lý kho/POS chỉ làm nhiệm vụ ghi nhận giao dịch (OLTP). Hệ thống của nhóm đóng vai trò DSS (*Decision Support System*) vì tích hợp mô hình định lượng đa tiêu chí (AHP Saaty), ma trận so sánh cặp, kiểm tra chỉ số nhất quán CR, chuẩn hóa điểm số, cho phép mô phỏng độ nhạy What-If đa kịch bản và tự động sinh khuyến nghị hành động tối ưu cho cấp quản lý.
2. **Câu 2: Tỷ số nhất quán CR được tính như thế nào và tại sao phải thỏa mãn $CR \le 0.10$?**  
   *Trả lời*: $CR = CI / RI$, trong đó $CI = (\lambda_{\max} - n)/(n - 1)$ với $n = 5$ và $RI = 1.12$. Ngưỡng $CR \le 0.10$ (10%) đảm bảo mức độ mâu thuẫn trong đánh giá chủ quan của chuyên viên nằm trong giới hạn toán học cho phép. Nếu $CR > 0.10$, hệ thống cảnh báo đỏ và chặn không cho phép ra quyết định để tránh sai lầm chiến lược.
3. **Câu 3: Tại sao lại chia tiêu chí thành Benefit và Cost khi chuẩn hóa?**  
   *Trả lời*: Các tiêu chí có đơn vị đo khác nhau (%, triệu đồng, ngày). Tiêu chí Benefit ($C_1, C_2, C_3$) càng cao càng tốt nên chuẩn hóa $norm = raw / max\_raw$. Tiêu chí Cost ($C_4, C_5$) càng thấp càng tốt nên chuẩn hóa $norm = min\_raw / raw$. Cách tiếp cận này đảm bảo mọi tiêu chí sau chuẩn hóa đều cùng chiều: điểm càng gần 1.0 càng tốt.
4. **Câu 4: Khi chạy mô phỏng What-If, dữ liệu gốc trong CSDL có bị ghi đè không?**  
   *Trả lời*: Hoàn toàn không. Mô phỏng What-If được xử lý trên tầng bộ nhớ đệm (*In-Memory Data Table*). Dữ liệu gốc trong bảng `ALTERNATIVE_SCORES` trên SQL Server được bảo toàn nguyên vẹn 100%. Khi người dùng bấm "Khôi Phục Dữ Liệu Gốc", hệ thống nạp lại snapshot ban đầu.

---

## 5. ĐIỀU KIỆN NGHIỆM THU VÀ KẾT LUẬN KIỂM THỬ

Toàn bộ **30/30 ca kiểm thử** đã được thực thi thành công trên bản phát hành chính thức của phần mềm **DSS Retail Decision Studio 2026** kết nối CSDL SQL Server `DSS_SalesDB`. Không có lỗi nghiêm trọng (Critical/High) nào tồn đọng. Hệ thống đạt 100% tiêu chí nghiệm thu về tính đúng đắn thuật toán AHP, tính toàn vẹn CSDL, khả năng phản hồi thời gian thực của mô-đun What-If và tính thẩm mỹ, tiện dụng của giao diện người dùng. Đồ án sẵn sàng cho phiên bảo vệ chính thức trước Hội đồng chấm thi học phần IS 402 A.
