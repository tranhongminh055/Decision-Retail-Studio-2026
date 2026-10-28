# KỊCH BẢN THUYẾT TRÌNH & BÁO CÁO ĐỒ ÁN DSS (LỜI THOẠI NGUYÊN VĂN TỪNG CHỮ)
## HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH BÁN HÀNG: DSS RETAIL STUDIO 2026
* **Môn học:** Hệ Thống Thông Tin Hỗ Trợ Ra Quyết Định (IS402)
* **Đề tài:** *Lựa chọn Danh mục Sản phẩm Chiến lược để Phân bổ Ngân sách Nhập hàng & Tiếp thị Quý 4/2026*
* **Mô hình toán học:** Phân tích thứ bậc AHP (Analytic Hierarchy Process - Thomas L. Saaty 1980)
* **Cơ sở dữ liệu:** Microsoft SQL Server (`DSS_SalesDB` - Instance: `.\SQLEXPRESS25`)
* **Nền tảng:** C# .NET 10 Windows Forms Desktop Enterprise Client

> 💡 **HƯỚNG DẪN DÀNH CHO BẠN (NGƯỜI THUYẾT TRÌNH):**  
> * Tài liệu này được biên soạn dưới dạng **Lời thoại đọc trực tiếp (Teleprompter Script)**.  
> * Mỗi phần đều có 2 mục rõ ràng:  
>   * 🖥️ **[THAO TÁC TRÊN MÀN HÌNH]:** Hướng dẫn bạn cần click chuột vào đâu, chiếu cái gì.  
>   * 🗣️ **[LỜI THOẠI CỦA BẠN]:** Bạn chỉ việc nhìn vào màn hình và **đọc nguyên văn từng câu chữ**, không cần phải tự nghĩ lời thoại.

---

## MỤC LỤC KỊCH BẢN
1. [Mở Đầu & Đặt Bài Toán Kinh Doanh Thực Tế (0 - 3 phút)](#phần-1-mở-đầu--đặt-bài-toán-thực-tế)
2. [Kiến Trúc CSDL & Ràng Buộc Phụ Thuộc 100% Vào SQL Server (3 - 6 phút)](#phần-2-kiến-trúc-csdl--ràng-buộc-sql-server)
3. [Mô Hình Toán Học AHP Saaty & Khử Bỏ Cảm Tính (6 - 10 phút)](#phần-3-mô-hình-toán-học-ahp-saaty)
4. [Kịch Bản Live Demo 10 Bước Trên Phần Mềm (10 - 20 phút)](#phần-4-kịch-bản-live-demo-10-bước)
5. [Lời Kết Luận & Mời Hội Đồng Đặt Câu Hỏi (20 - 21 phút)](#phần-5-kết-luận--mời-qa)
6. [Lời Thoại Trả Lời 5 Câu Hỏi Phản Biện Hóc Búa Nhất](#phần-6-bộ-câu-trả-lời-phản-biện-nguyên-văn)

---

## PHẦN 1: MỞ ĐẦU & ĐẶT BÀI TOÁN THỰC TẾ
*(Thời lượng: 03 phút)*

🖥️ **[THAO TÁC TRÊN MÀN HÌNH]:**  
* Chiếu Slide mở đầu có tên đề tài: **"HỆ THỐNG DSS RETAIL STUDIO 2026 - PHÂN TÍCH THỨ BẬC SAATY HỖ TRỢ RA QUYẾT ĐỊNH BÁN HÀNG"**.  
* Giữ phong thái tự tin, mắt nhìn thẳng vào Thầy/Cô và các bạn trong nhóm.

🗣️ **[LỜI THOẠI CỦA BẠN - ĐỌC NGUYÊN VĂN]:**  
> *"Kính thưa Thầy/Cô và toàn thể các bạn thành viên trong hội đồng,  
> Hôm nay, em xin đại diện nhóm trình bày báo cáo đồ án môn Hệ Thống Thông Tin Hỗ Trợ Ra Quyết Định với đề tài: **Xây dựng hệ thống DSS Retail Decision Studio 2026 ứng dụng mô hình phân tích thứ bậc AHP của Saaty để giải quyết bài toán lựa chọn danh mục sản phẩm chiến lược nhập hàng Quý 4/2026**.  
> 
> Thưa Thầy/Cô, trong ngành bán lẻ công nghệ, thời điểm Quý 4 và dịp Tết Nguyên Đán luôn là giai đoạn bùng nổ doanh số, nhưng cũng là thời điểm rủi ro tài chính cao nhất trong năm. Một doanh nghiệp nếu ra quyết định nhập hàng dựa trên **cảm tính**, **kinh nghiệm cá nhân** hoặc chỉ nhìn vào một chỉ số đơn lẻ như doanh thu thì chắc chắn sẽ gặp 2 bài toán nan giải:  
> **Thứ nhất:** Nhập quá nhiều các dòng máy cao cấp đắt tiền dẫn đến đọng vốn hàng tỷ đồng khi sức mua thị trường đảo chiều, chi phí lãi vay và lưu kho tăng vọt.  
> **Thứ hai:** Ngược lại, nếu quá dè dặt, không kịp thời rót vốn vào các sản phẩm đang có xu hướng thị trường bùng nổ, doanh nghiệp sẽ bị 'cháy hàng', đánh mất thị phần trực tiếp vào tay đối thủ.  
> 
> Điểm khó khăn nhất ở đây là người ra quyết định phải cùng lúc cân đối **5 tiêu chí mang tính mâu thuẫn lẫn nhau**: Chúng ta vừa muốn biên lợi nhuận cao, doanh thu lớn, thị trường đón nhận, nhưng đồng thời lại muốn thời gian quay vòng vốn phải ngắn và chi phí marketing phải thật tiết kiệm.  
> 
> Chính vì vậy, nhóm chúng em đã nghiên cứu và phát triển phần mềm **DSS Retail Decision Studio 2026**. Hệ thống kết hợp chặt chẽ giữa **Cơ sở dữ liệu quan hệ Microsoft SQL Server** và **Mô hình toán học đa tiêu chí AHP** nhằm chuyển hóa toàn bộ dữ liệu lịch sử bán hàng thành các khuyến nghị kinh doanh định lượng, khách quan và minh bạch 100%, tuyệt đối nói không với các quyết định mang tính cảm tính."*

---

## PHẦN 2: KIẾN TRÚC CSDL & RÀNG BUỘC SQL SERVER
*(Thời lượng: 03 phút)*

🖥️ **[THAO TÁC TRÊN MÀN HÌNH]:**  
* Chuyển sang Slide Sơ đồ Thực thể Mối quan hệ (ERD 12 bảng) và bảng phân hệ CSDL.

🗣️ **[LỜI THOẠI CỦA BẠN - ĐỌC NGUYÊN VĂN]:**  
> *"Tiếp theo, em xin phép trình bày về nền tảng cơ sở dữ liệu của hệ thống.  
> Cơ sở dữ liệu của chúng em có tên là **DSS_SalesDB**, được triển khai trên hệ quản trị **Microsoft SQL Server**, bao gồm **12 bảng thực thể** đã được chuẩn hóa đạt chuẩn **3NF (Dạng chuẩn 3)**, chia làm 3 phân hệ chính:  
> 
> **Phân hệ 1 - Nghiệp vụ Bán lẻ cốt lõi:** Quản lý toàn bộ vòng đời kinh doanh thực tế gồm các bảng: Danh mục (`CATEGORIES`), Sản phẩm (`PRODUCTS`), Khách hàng (`CUSTOMERS`), Chương trình khuyến mãi (`PROMOTIONS`), Đơn hàng (`ORDERS`) và Chi tiết đơn hàng (`ORDER_DETAILS`).  
> **Phân hệ 2 - Hỗ trợ ra quyết định DSS:** Gồm các bảng lưu trữ phiên quyết định (`DECISION_SESSIONS`), Danh mục tiêu chí (`CRITERIA`), Ma trận so sánh cặp Saaty (`PAIRWISE_COMPARISONS`), Phương án ứng viên (`ALTERNATIVES`), Điểm đánh giá thô & chuẩn hóa (`ALTERNATIVE_SCORES`), và Kết quả xếp hạng cuối cùng (`DECISION_RESULTS`).  
> **Phân hệ 3 - Quản trị người dùng & Bảo mật:** Bảng `USERS` lưu trữ thông tin nhân sự với mật khẩu được băm một chiều bằng thuật toán **SHA-256**, phân quyền chặt chẽ theo 4 vai trò: Admin, DecisionMaker, SalesManager và SalesStaff.  
> 
> 💡 **Đặc biệt, hệ thống của nhóm em có một nguyên tắc thiết kế bất biến:**  
> **Hệ thống phụ thuộc 100% vào SQL Server thật.** Phần mềm tuyệt đối không sử dụng tài khoản mẫu gán cứng, không sử dụng dữ liệu offline hay số liệu giả lập trong code.  
> Nhóm em đã xây dựng một cơ chế giám sát kết nối gọi là **Database Guard & Heartbeat Monitor**:  
> Ngay khi khởi động, ứng dụng sẽ gửi tín hiệu kiểm tra dịch vụ `.\SQLEXPRESS25`. Nếu SQL Server chưa chạy, phần mềm lập tức chặn lại và báo lỗi. Đồng thời, trong suốt quá trình người dùng thao tác, một luồng nhịp tim ngầm sẽ kiểm tra trạng thái mỗi 4 giây. Nếu SQL Server bị ngắt đột ngột, toàn bộ màn hình sẽ được khóa lại bởi lớp bảo vệ màu đỏ để ngăn chặn việc ra quyết định sai lệch. Lát nữa trong phần Live Demo, em sẽ thực hiện bài test tắt SQL Server trực tiếp để chứng minh cơ chế này."*

---

## PHẦN 3: MÔ HÌNH TOÁN HỌC AHP SAATY
*(Thời lượng: 04 phút)*

🖥️ **[THAO TÁC TRÊN MÀN HÌNH]:**  
* Chiếu Slide Ma trận so sánh cặp Saaty 5×5 và các công thức toán học AHP.

🗣️ **[LỜI THOẠI CỦA BẠN - ĐỌC NGUYÊN VĂN]:**  
> *"Sau đây, em xin đi sâu vào phần 'bộ não' tính toán của hệ thống: **Phương pháp phân tích thứ bậc AHP của Giáo sư Thomas L. Saaty**.  
> 
> Trong phiên quyết định Quý 4/2026, chúng em thiết lập **5 tiêu chí đánh giá**:  
> * Tiêu chí 1: **Biên lợi nhuận gộp ($C_1$)** – Tiêu chí Lợi ích, nhằm tối đa hóa tỷ suất sinh lời.  
> * Tiêu chí 2: **Doanh thu kỳ vọng ($C_2$)** – Tiêu chí Lợi ích, nhằm mở rộng quy mô dòng tiền.  
> * Tiêu chí 3: **Nhu cầu thị trường ($C_3$)** – Tiêu chí Lợi ích, đo lường độ 'nóng' của sản phẩm.  
> * Tiêu chí 4: **Rủi ro tồn kho ($C_4$)** – Tiêu chí Chi phí, đo lường số ngày đọng vốn, càng thấp càng an toàn.  
> * Tiêu chí 5: **Chi phí Marketing ($C_5$)** – Tiêu chí Chi phí, đo lường ngân sách tiếp thị cần chi tiêu.  
> 
> Quy trình tính toán AHP trong hệ thống diễn ra qua 4 bước toán học nghiêm ngặt:  
> 
> **Bước 1 - Xác định trọng số từ ma trận Saaty:**  
> Ma trận so sánh cặp $5 \times 5$ được lưu trữ trong bảng `PAIRWISE_COMPARISONS`. Hệ thống thực hiện chuẩn hóa ma trận theo từng cột, sau đó tính trung bình cộng theo từng hàng để tìm ra vector trọng số $W$.  
> 
> **Bước 2 - Kiểm định chỉ số nhất quán logic toán học ($CR$):**  
> Đây là ưu điểm vượt trội nhất của AHP so với các phương pháp cho điểm cảm tính thông thường. Hệ thống tính giá trị riêng cực đại $\lambda_{max}$, tính chỉ số nhất quán $CI = (\lambda_{max} - n)/(n - 1)$, và tra bảng chỉ số ngẫu nhiên với $n=5$ thì $RI = 1.12$.  
> Tỷ số nhất quán được tính bằng $CR = CI / RI$.  
> **Quy tắc toán học:** $CR$ bắt buộc phải $\le 0.10$ (tức dưới $10\%$). Nếu ma trận mâu thuẫn logic vượt quá $10\%$, hệ thống sẽ từ chối tính toán và bắt buộc hội đồng phải điều chỉnh lại. Trong cơ sở dữ liệu hiện tại của nhóm, ma trận đạt tỷ số $CR = 0.0138$ (chỉ $1.38\%$), đảm bảo độ tin cậy toán học tuyệt đối.  
> 
> **Bước 3 - Chuẩn hóa điểm thô thành thang đo định lượng từ 0 đến 1:**  
> Để so sánh được các đại lượng có đơn vị khác nhau (như phần trăm lợi nhuận, triệu đồng doanh thu, số ngày tồn kho), hệ thống tự động chuẩn hóa:  
> * Với tiêu chí Lợi ích: Lấy giá trị chia cho giá trị lớn nhất ($v / v_{max}$).  
> * Với tiêu chí Chi phí: Lấy giá trị nhỏ nhất chia cho giá trị thực tế ($v_{min} / v$) để sản phẩm nào chi phí thấp nhất sẽ đạt điểm 1.0 tối đa.  
> 
> **Bước 4 - Tính lượng hàng đề xuất nhập kho từ lịch sử bán hàng thực tế:**  
> Điểm số AHP quyết định thứ hạng: Hạng 1 được **Phê duyệt**, Hạng 2 và 3 thuộc diện **Xem xét**, còn lại bị **Loại bỏ**.  
> Đối với sản phẩm được duyệt, hệ thống tính số lượng nhập bằng công thức:  
> **Lượng đề xuất = Nhu cầu ngày × (Thời gian giao hàng + 30 ngày chu kỳ) + Tồn tối thiểu − Tồn hiện tại.**  
> Trong đó, *Nhu cầu ngày* được tính hoàn toàn tự động bằng hàm `SUM` số lượng các đơn hàng đã hoàn tất trong bảng `ORDERS` chia cho tổng số ngày bán lịch sử thực tế trong SQL Server. Hoàn toàn không có bất kỳ con số gán bừa nào!"*

---

## PHẦN 4: KỊCH BẢN LIVE DEMO 10 BƯỚC
*(Thời lượng: 10 phút)*

🖥️ **[THAO TÁC TRÊN MÀN HÌNH]:**  
* Thu nhỏ Slide, mở màn hình Desktop máy tính.  
* Mở thư mục dự án `g:\Decision System\`.

🗣️ **[LỜI THOẠI CỦA BẠN - ĐỌC NGUYÊN VĂN]:**  
> *"Và sau đây, để Thầy/Cô và các bạn thấy rõ sự vận hành thực tế, em xin phép bắt đầu phần **Live Demonstration trực tiếp trên phần mềm C# .NET 10**."*

---

### 🔹 BƯỚC 1: KHỞI ĐỘNG SPLASH SCREEN & KIỂM TRA SQL SERVER

🖥️ **[THAO TÁC]:**  
* Nhấp đúp chuột vào file `CHAY_HE_THONG_GUI_CSHARP.bat` ngoài màn hình.  
* Màn hình Splash xuất hiện với thanh tiến trình màu xanh tím chạy từ 0% đến 100%.

🗣️ **[LỜI THOẠI CỦA BẠN]:**  
> *"Đầu tiên, em khởi chạy ứng dụng. Màn hình Splash Screen xuất hiện.  
> Quý Thầy/Cô có thể thấy hệ thống đang tự động gửi gói tin ping tới SQL Server instance `.\SQLEXPRESS25` và kiểm tra bảng dữ liệu `DSS_SalesDB`. Khi kết nối thành công, hệ thống mới chính thức cấp quyền mở Cổng đăng nhập."*

---

### 🔹 BƯỚC 2: ĐĂNG KÝ TÀI KHOẢN MỚI & ĐĂNG NHẬP THẬT

🖥️ **[THAO TÁC]:**  
* Màn hình LoginForm xuất hiện.  
* Bấm vào nút màu xanh **"Đăng ký tài khoản mới"**.  
* Form đăng ký mở lên. Nhập lần lượt:  
  * Họ và tên: `Trần Văn Giám Đốc`  
  * Tên đăng nhập: `giamdoc_q4`  
  * Email: `giamdoc@dss-retail.vn`  
  * Số điện thoại: `0912345678`  
  * Vai trò: Chọn trong combo box `DecisionMaker (Người Ra Quyết Định)`  
  * Mật khẩu: `Admin@123`  
  * Xác nhận mật khẩu: `Admin@123`  
* Bấm nút **"ĐĂNG KÝ TÀI KHOẢN MỚI"** $\to$ Hộp thoại báo thành công $\to$ Quay lại màn hình đăng nhập.  
* Nhập `giamdoc_q4` / `Admin@123` $\to$ Bấm **"ĐĂNG NHẬP HỆ THỐNG"**.

🗣️ **[LỜI THOẠI CỦA BẠN]:**  
> *"Tại màn hình đăng nhập, hệ thống không hề có tài khoản mẫu mặc định. Em sẽ nhấn Đăng ký tài khoản mới.  
> Em đăng ký tài khoản cho một Giám đốc kinh doanh có tên là **Trần Văn Giám Đốc**, tên đăng nhập là `giamdoc_q4`, vai trò là `DecisionMaker`, mật khẩu được mã hóa SHA-256 an toàn.  
> Em bấm Đăng ký thành công, và bây giờ em đăng nhập vào hệ thống.  
> Ngay khi đăng nhập thành công, ở góc trên cùng bên phải thanh tiêu đề, hệ thống hiển thị chính xác tên: **TRẦN VĂN GIÁM ĐỐC [DecisionMaker]** được tải trực tiếp từ bảng `dbo.USERS`."*

---

### 🔹 BƯỚC 3: TAB 1 - KIỂM TRA ĐẦU VÀO & MA TRẬN SAATY

🖥️ **[THAO TÁC]:**  
* Giao diện chính Form1 mở ra ở Tab 1 **[1. Đầu Vào Dữ Liệu (Inputs & Parameters)]**.  
* Dùng chuột rê qua bảng bên trái (Danh sách phương án sản phẩm).  
* Sau đó rê chuột qua bảng bên phải (Ma trận Saaty 5×5) và chỉ con trỏ vào dòng chữ xanh lá ở góc dưới bên phải.

🗣️ **[LỜI THOẠI CỦA BẠN]:**  
> *"Tại Tab 1, hệ thống hiển thị toàn bộ tham số đầu vào được nạp trực tiếp từ CSDL:  
> * Phía bên trái là danh sách 5 sản phẩm ứng viên Quý 4 gồm: Samsung Galaxy S24 Ultra, Dell Gaming G15, Apple Watch Ultra 2, Loa Bluetooth SoundMax BT-01 và Laptop Acer Aspire, đi kèm giá vốn, giá bán, tồn kho và biên lợi nhuận thực tế.  
> * Phía bên phải là Ma trận so sánh cặp giữa 5 tiêu chí theo thang đo Saaty từ 1 đến 9.  
> 💡 **Xin Thầy/Cô đặc biệt lưu ý dòng chữ màu xanh lá ở góc dưới bên phải:**  
> Hệ thống ghi rõ: `✓ Tỷ số nhất quán (tính từ SQL Server): CR = 0.0138 (1.38%) [ĐẠT CHUẨN SAATY CR <= 0.10] • λmax = 5.0683`.  
> Con số $CR = 0.0138$ này được thuật toán tính toán động từ dữ liệu ma trận trong CSDL ngay khi mở form. Vì $1.38\% < 10\%$, ma trận hoàn toàn hợp lệ để tiến hành ra quyết định."*

---

### 🔹 BƯỚC 4: TAB 2 - KÍCH HOẠT MÔ HÌNH RA QUYẾT ĐỊNH DSS

🖥️ **[THAO TÁC]:**  
* Bấm chuyển sang Tab 2 **[2. Kết Quả Ra Quyết Định (Outputs & Decision)]**.  
* Màn hình đang ở trạng thái chờ với nút lớn màu tím: **"🚀 BẤM VÀO ĐÂY ĐỂ CHẠY MÔ HÌNH DSS NGAY"**.  
* Bấm chuột vào nút này. Hộp thoại báo *"MÔ HÌNH HỖ TRỢ RA QUYẾT ĐỊNH ĐÃ CHẠY HOÀN TẤT!"* hiện lên $\to$ Bấm OK.

🗣️ **[LỜI THOẠI CỦA BẠN]:**  
> *"Bây giờ em chuyển sang Tab 2 - Kết Quả Đầu Ra. Khi chưa chạy mô hình, hệ thống ở trạng thái chờ an toàn để tránh hiển thị số liệu cũ.  
> Em nhấn nút: **'BẤM VÀO ĐÂY ĐỂ CHẠY MÔ HÌNH DSS NGAY'**.  
> Chỉ mất chưa đầy 1 giây, câu lệnh SQL MERGE và thuật toán AHP đã chạy xong. Kết quả hiện ra cực kỳ trực quan:  
> 1. **Bốn thẻ KPI trên cùng:**  
>    * Sản phẩm tối ưu số 1: **Samsung Galaxy S24 Ultra** đạt điểm AHP cao nhất là **0.9102 / 1.0000**.  
>    * Tỷ số $CR = 0.0138$ đạt chuẩn.  
>    * Doanh thu kỳ vọng: **1,200 Triệu VNĐ**, biên lợi nhuận gộp đạt **26.29%**.  
> 2. **Bảng xếp hạng chi tiết:**  
>    * **Hạng 1 - Huy chương Vàng:** Samsung Galaxy S24 Ultra gắn nhãn xanh `✓ Phê Duyệt`.  
>    * **Hạng 2 & Hạng 3:** Dell Gaming G15 và Apple Watch Ultra 2 gắn nhãn vàng `⏳ Xem Xét` (Nhập thăm dò theo đơn đặt cọc).  
>    * **Hạng 4 & Hạng 5:** Loa BT-01 và Laptop Acer gắn nhãn đỏ `✕ Loại Bỏ` (Dừng nhập mới, xả tồn kho).  
> 3. **Biểu đồ cột trực quan:** So sánh trực quan khoảng cách điểm số giữa phương án dẫn đầu và các phương án còn lại.  
> 4. **Hộp tư vấn AI phía dưới:** Đưa ra chỉ đạo cụ thể: Nhập ngay lô hàng Samsung S24 Ultra, phân bổ ngân sách marketing theo đúng tỷ trọng điểm AHP được phê duyệt."*

---

### 🔹 BƯỚC 5: TAB 3 - MÔ PHỎNG BIẾN ĐỘNG THỊ TRƯỜNG (WHAT-IF SENSITIVITY)

🖥️ **[THAO TÁC]:**  
* Bấm chuyển sang Tab 3 **[3. Biến Động Thị Trường (What-If)]**.  
* Lần lượt bấm:  
  1. Bấm nút màu cam: **"🔥 Kịch Bản 1: Bùng Nổ Gaming (+40% Nhu Cầu)"**.  
  2. Rê chuột chỉ vào Biểu đồ đường Line Chart và Bảng đối chiếu.  
  3. Bấm nút màu xanh lá ở góc phải thanh công cụ biểu đồ: **"🌲 Sơ Đồ Cây Cấu Trúc Thứ Bậc AHP"**.  
  4. Bấm nút màu xám: **"🔄 Khôi Phục Dữ Liệu Gốc (Baseline)"**.

🗣️ **[LỜI THOẠI CỦA BẠN]:**  
> *"Tiếp theo là tính năng rất được Ban Giám đốc quan tâm: **Phân tích độ nhạy What-If**.  
> Thị trường luôn biến động, vì vậy DSS phải giúp nhà quản trị trả lời được câu hỏi: *'Điều gì xảy ra nếu thị trường thay đổi?'*  
> 
> * Em bấm vào **Kịch Bản 1: Bùng Nổ Thị Trường Gaming (+40% Nhu Cầu)**:  
> Thầy/Cô có thể thấy trên Bảng đối chiếu và Biểu đồ đường màu tím, điểm số của dòng máy `Dell Gaming G15` đã lập tức tăng vọt từ 0.74 lên cạnh tranh sát nút vị trí số 1.  
> * Em bấm vào nút **'Sơ Đồ Cây Cấu Trúc Thứ Bậc AHP'**:  
> Màn hình vẽ sơ đồ phân cấp trực quan từ Mục tiêu $\to$ 5 Tiêu chí với trọng số và $CR$ tính động $\to$ Các sản phẩm ứng viên bên dưới.  
> * Cuối cùng, em bấm nút **'Khôi Phục Dữ Liệu Gốc'**:  
> Hệ thống lập tức gọi hàm nạp lại giá trị thô ban đầu trong SQL Server và tính lại Baseline chuẩn xác, không hề làm sai lệch dữ liệu gốc."*

---

### 🔹 BƯỚC 6: TAB 4 - THÊM SẢN PHẨM MỚI (CHUẨN HÓA ĐỘNG)

🖥️ **[THAO TÁC]:**  
* Bấm sang Tab 4 **[4. Thêm Sản Phẩm Mới]**.  
* Nhập nhanh:  
  * Tên sản phẩm: `Bàn Phím Cơ Gaming K90 Pro`  
  * Mã SKU: `PRD_KEY_K90`  
  * Danh mục: Chọn `Phụ Kiện Gaming`  
  * Giá vốn: `850,000`  
  * Giá bán: `1,690,000` (Nhìn ô Biên lợi nhuận tự tính nhảy lên ~49.7%)  
  * Tồn kho: `30`  
  * Doanh thu kỳ vọng: `750`  
  * Nhu cầu thị trường: `85`  
  * Tồn kho/quay vòng: `15`  
  * Chi phí marketing: `25`  
* Bấm nút **"LƯU SẢN PHẨM & CẬP NHẬT DSS"** $\to$ Báo thành công và tự động chuyển về Tab 1.

🗣️ **[LỜI THOẠI CỦA BẠN]:**  
> *"Tại Tab 4, doanh nghiệp có thể đưa thêm bất kỳ dòng sản phẩm mới nào vào phiên thẩm định.  
> Ví dụ em thêm sản phẩm **Bàn Phím Cơ Gaming K90 Pro**. Giá vốn 850 ngàn, giá bán 1 triệu 690 ngàn. Ô biên lợi nhuận bên cạnh tự động tính toán ra gần 50%.  
> Khi em bấm nút **Lưu Sản Phẩm**:  
> Dữ liệu được ghi thẳng vào bảng `PRODUCTS`, `ALTERNATIVES` và `ALTERNATIVE_SCORES`.  
> 💡 **Điểm đáng giá ở đây là:** Thuật toán chuẩn hóa trong `DssEngine` tự động quét lại toàn bộ cực trị lớn nhất và nhỏ nhất của phiên để tính lại điểm chuẩn hóa cho tất cả các sản phẩm, chứ không hề dùng các hằng số chia cố định như 35% hay 1,200 triệu."*

---

### 🔹 BƯỚC 7: TAB 5 - PHÂN TÍCH ĐA CHIỀU OLAP (ROLLUP & CUBE)

🖥️ **[THAO TÁC]:**  
* Bấm sang Tab 5 **[5. Dữ Liệu Bán Hàng Lịch Sử (OLAP)]**.  
* Lần lượt bấm:  
  1. Nút 1: **"📊 1. Hiệu Suất Sản Phẩm (Fact/View)"**  
  2. Nút 2: **"📦 2. OLAP ROLLUP: Phân Bổ Hàng Tồn Kho"**  
  3. Nút 3: **"🎯 3. OLAP CUBE: Đa Chiều Khuyến Mãi"**

🗣️ **[LỜI THOẠI CỦA BẠN]:**  
> *"Tại Tab 5 là phân hệ **Phân tích kinh doanh thông minh OLAP**:  
> * Nút 1 hiển thị View Hiệu Suất Sản Phẩm thực tế: Thống kê số lượng bán, doanh thu gộp, giá vốn và biên lợi nhuận lũy kế.  
> * Nút 2 thực hiện truy vấn **OLAP ROLLUP**: Phân tích đa cấp độ tổng giá trị tồn kho theo từng Danh mục sản phẩm đến từng SKU con.  
> * Nút 3 thực hiện truy vấn **OLAP CUBE**: Khảo sát đa chiều hiệu quả của các Chiến dịch khuyến mãi tác động lên từng nhóm mặt hàng.  
> Đây chính là căn cứ thực tế để các chuyên gia thiết lập ma trận so sánh cặp Saaty ở Tab 1."*

---

### 🔹 BƯỚC 8: TAB 6 - XUẤT BÁO CÁO HỒ SƠ THẨM ĐỊNH (WORD & PDF)

🖥️ **[THAO TÁC]:**  
* Bấm sang Tab 6 **[6. Form Báo Cáo & AI Insights]**.  
* Cuộn chuột xuống dưới cùng.  
* Bấm nút: **"📄 Xuất Biên Bản Microsoft Word (.doc)"** $\to$ Chọn lưu file $\to$ Phần mềm tự động kích hoạt Microsoft Word mở file lên.  
* Sau đó quay lại phần mềm, bấm nút **"🌐 Xuất Báo Cáo Doanh Nghiệp HTML (In PDF)"** $\to$ Trình duyệt tự mở file HTML giao diện cực đẹp.

🗣️ **[LỜI THOẠI CỦA BẠN]:**  
> *"Tại Tab 6 là nơi đóng gói toàn bộ phiên làm việc thành hồ sơ pháp lý doanh nghiệp.  
> Em nhấn nút **Xuất Biên Bản Microsoft Word**:  
> Phần mềm tự động sinh ra một file Word chuẩn theo thể thức văn bản hành chính của **Nghị định 30/2020/NĐ-CP**: Có Quốc hiệu, Tiêu ngữ, Căn cứ pháp lý, Bảng tổng hợp trọng số tiêu chí, Bảng điểm AHP từng sản phẩm, Lượng hàng đề xuất nhập kho, và cuối trang có sẵn khung chữ ký 3 bên giữa Trưởng phòng Kinh doanh, Giám đốc Tài chính và Tổng Giám đốc.  
> Tương tự, nút **Xuất Báo Cáo HTML** sinh ra giao diện hiện đại phục vụ trình chiếu họp Ban điều hành và có nút In/Lưu PDF chỉ với 1 cú click chuột.  
> Toàn bộ số liệu trong 2 bản báo cáo này đều được kéo trực tiếp từ snapshot SQL Server của phiên hiện tại, hoàn toàn không có bất kỳ con số hard-code nào."*

---

### 🔹 BƯỚC 9: MÀN TRÌNH DIỄN ĐỈNH CAO - TẮT SQL SERVER ĐỂ THỬ THÁCH HỆ THỐNG

🖥️ **[THAO TÁC]:**  
* Thu nhỏ ứng dụng DSS xuống một nửa màn hình để người xem nhìn thấy cả 2 cửa sổ.  
* Mở cửa sổ `Services` (`services.msc`).  
* Cuộn đến dòng dịch vụ: **`SQL Server (SQLEXPRESS25)`**.  
* Chuột phải chọn **Stop**.  
* Chờ 2 - 3 giây: Màn hình ứng dụng DSS lập tức bị phủ kín bởi một panel màu đỏ thẫm với biểu tượng ⛔ và dòng chữ lớn:  
  **"MẤT KẾT NỐI SQL SERVER – HỆ THỐNG TẠM DỪNG"**.  
* Thử bấm vào các tab $\implies$ Toàn bộ bị khóa cứng, không thao tác được.  
* Quay lại cửa sổ `Services`, chuột phải vào `SQL Server (SQLEXPRESS25)` chọn **Start**.  
* Quay lại ứng dụng DSS, bấm nút **"↻ Thử kết nối lại"** $\implies$ Màn hình đỏ biến mất, giao diện mở khóa và nạp lại dữ liệu bình thường!

🗣️ **[LỜI THOẠI CỦA BẠN - NÓI VỚI GIỌNG TỰ TIN, DỨT KHOÁT]:**  
> *"Và cuối cùng, để chứng minh một cách thuyết phục nhất rằng: **Phần mềm của nhóm em phụ thuộc 100% vào SQL Server, tuyệt đối không hề chạy giả lập hay dùng dữ liệu offline**, em xin thực hiện một bài kiểm tra thực tế:  
> 
> Em mở trình quản lý dịch vụ Windows Services. Quý Thầy/Cô có thể thấy dịch vụ `SQL Server (SQLEXPRESS25)` đang chạy.  
> Bây giờ, em nhấn **STOP** để tắt hoàn toàn SQL Server...  
> *(Dừng 2 giây để mọi người quan sát)*  
> Ngay lập tức, luồng Heartbeat của ứng dụng phát hiện mất kết nối CSDL và kích hoạt màn hình khóa bảo vệ màu đỏ: **'MẤT KẾT NỐI SQL SERVER – HỆ THỐNG TẠM DỪNG'**. Toàn bộ các thao tác ra quyết định bị chặn đứng hoàn toàn để bảo vệ tính toàn vẹn dữ liệu.  
> 
> Và bây giờ, em bật **START** lại dịch vụ SQL Server...  
> Em nhấn nút **'Thử kết nối lại'** trên ứng dụng...  
> Ngay lập tức, màn hình khóa mở ra, hệ thống tự động đồng bộ lại dữ liệu mới nhất và tiếp tục vận hành hoàn hảo!"*

---

## PHẦN 5: KẾT LUẬN & MỜI Q&A
*(Thời lượng: 01 phút)*

🖥️ **[THAO TÁC TRÊN MÀN HÌNH]:**  
* Chiếu Slide Tổng kết: Các kết quả đạt được & Lời cảm ơn.

🗣️ **[LỜI THOẠI CỦA BẠN - ĐỌC NGUYÊN VĂN]:**  
> *"Kính thưa Thầy/Cô và các bạn,  
> Qua bài báo cáo và phần Live Demo vừa rồi, nhóm chúng em đã chứng minh được:  
> 1. Mô hình AHP của Saaty đã giải quyết triệt để bài toán lựa chọn danh mục sản phẩm nhập hàng đa tiêu chí, chuyển hóa từ quyết định cảm tính sang quyết định định lượng toán học với tỷ số nhất quán $CR = 0.0138$.  
> 2. Cơ sở dữ liệu SQL Server `DSS_SalesDB` đạt chuẩn 3NF, liên kết chặt chẽ và cung cấp dữ liệu tức thời cho toàn bộ quy trình DSS.  
> 3. Hệ thống phần mềm C# WinForms hoàn thiện 100% các chức năng từ phân quyền, What-If, OLAP, xuất báo cáo Word chuẩn hành chính đến cơ chế tự bảo vệ khi mất kết nối.  
> 
> Em xin chân thành cảm ơn Thầy/Cô và các bạn đã chú ý lắng nghe. Nhóm em rất mong nhận được những ý kiến đóng góp quý báu từ Thầy/Cô để đồ án ngày càng hoàn thiện hơn nữa.  
> **Sau đây, nhóm em xin sẵn sàng lắng nghe và trả lời các câu hỏi phản biện ạ!**"*

---

## PHẦN 6: BỘ CÂU TRẢ LỜI PHẢN BIỆN NGUYÊN VĂN
*(Khi Thầy/Cô hoặc nhóm phản biện hỏi câu nào, bạn chỉ việc tra đúng câu đó và đọc câu trả lời dưới đây)*

---

### ❓ CÂU HỎI 1:
**"Tại sao nhóm lại chọn phương pháp AHP của Saaty mà không dùng TOPSIS hay Điểm Trọng Số Giản Đơn (WSM)?"**

🗣️ **[LỜI THOẠI TRẢ LỜI CỦA BẠN]:**  
> *"Dạ thưa Thầy/Cô, nhóm em đã phân tích và so sánh kỹ lưỡng trước khi lựa chọn:  
> Phương pháp Điểm Trọng Số Giản Đơn (WSM) có nhược điểm chí mạng là trọng số tiêu chí thường do con người tự gán một cách chủ quan, hoàn toàn không kiểm tra được tính mâu thuẫn trong tư duy của người đánh giá.  
> Trong khi đó, **AHP của Saaty** giải quyết triệt để bài toán này nhờ cơ chế so sánh cặp (Pairwise Comparison) và **Tỷ số nhất quán logic $CR$ (Consistency Ratio)**. Nếu một chuyên gia đánh giá Biên lợi nhuận quan trọng hơn Doanh thu, Doanh thu quan trọng hơn Nhu cầu, nhưng sau đó lại cho rằng Nhu cầu quan trọng hơn Biên lợi nhuận thì chỉ số $CR$ sẽ vượt quá $10\%$ ngay. Khi đó hệ thống sẽ từ chối ra quyết định và yêu cầu đánh giá lại.  
> Đối với bài toán đầu tư nguồn vốn bán lẻ hàng chục tỷ đồng, kiểm soát được tính nhất quán logic toán học là yếu tố sống còn, và AHP là phương pháp đáp ứng tốt nhất yêu cầu này ạ."*

---

### ❓ CÂU HỎI 2:
**"Làm thế nào để chứng minh rằng kết quả xếp hạng và số lượng đề xuất nhập hàng không bị gán cứng (hard-code) trong mã nguồn?"**

🗣️ **[LỜI THOẠI TRẢ LỜI CỦA BẠN]:**  
> *"Dạ thưa Thầy/Cô, nhóm em chứng minh tính khách quan qua 3 luận cứ kỹ thuật cụ thể:  
> **Thứ nhất:** Toàn bộ điểm số AHP đều được tính bằng phép nhân vô hướng giữa vector trọng số trong bảng `PAIRWISE_COMPARISONS` và ma trận điểm chuẩn hóa trong bảng `ALTERNATIVE_SCORES`, sau đó cập nhật trực tiếp vào bảng `DECISION_RESULTS` bằng câu lệnh SQL MERGE trong transaction an toàn.  
> **Thứ hai:** Công thức tính lượng hàng đề xuất nhập không phải là một con số cố định, mà là hàm bù đắp tồn kho: $\text{Lượng nhập} = \text{Nhu cầu ngày} \times (\text{LeadTime} + 30) + \text{Tồn tối thiểu} - \text{Tồn hiện tại}$. Trong đó, nhu cầu ngày được tính bằng hàm `SUM(quantity)` từ các đơn hàng có trạng thái `Completed` trong CSDL chia cho tổng số ngày bán lịch sử.  
> **Thứ ba:** Bằng chứng rõ nhất là ở Tab 4 và Tab 3 What-If, khi chúng em thêm một sản phẩm mới hoặc thay đổi tham số thị trường, toàn bộ thứ hạng, điểm số và số lượng nhập trong Bảng xếp hạng cũng như trong file Word/HTML xuất ra đều thay đổi theo ngay lập tức ạ."*

---

### ❓ CÂU HỎI 3:
**"Nếu một nhân viên bán hàng (SalesStaff) đăng nhập vào hệ thống thì có thể bấm nút chạy lại mô hình DSS hay không?"**

🗣️ **[LỜI THOẠI TRẢ LỜI CỦA BẠN]:**  
> *"Dạ thưa Thầy/Cô, hệ thống áp dụng cơ chế kiểm soát truy cập dựa trên vai trò (RBAC) rất chặt chẽ:  
> Nhân viên bán hàng (`SalesStaff`) chỉ có quyền tra cứu kết quả xếp hạng, xem thông tin phân tích bán hàng OLAP và đọc khuyến nghị AI để hỗ trợ tư vấn khách hàng.  
> Nếu tài khoản `SalesStaff` bấm vào nút 'Chạy Mô Hình DSS' tại Tab 2, hệ thống sẽ kiểm tra quyền qua đối tượng `UserSession.CurrentUser` và lập tức chặn lại, hiển thị hộp thoại cảnh báo: **'Không đủ quyền: Chỉ Giám đốc kinh doanh / Quản trị viên mới được kích hoạt mô hình DSS'**. Chỉ các vai trò có thẩm quyền như `DecisionMaker`, `SalesManager` hoặc `Admin` mới được phép kích hoạt tính toán phiên quyết định ạ."*

---

### ❓ CÂU HỎI 4:
**"Vì sao đối với tiêu chí Rủi ro tồn kho ($C_4$) và Chi phí Marketing ($C_5$) nhóm lại dùng công thức chuẩn hóa nghịch đảo ($v_{min} / v$)?"**

🗣️ **[LỜI THOẠI TRẢ LỜI CỦA BẠN]:**  
> *"Dạ thưa Thầy/Cô, trong lý thuyết ra quyết định đa tiêu chí, các tiêu chí luôn được phân định thành 2 nhóm:  
> 1. **Tiêu chí Lợi ích (Benefit):** Giá trị thực tế càng cao thì càng tốt (như Biên lợi nhuận hay Doanh thu). Vì vậy ta chuẩn hóa bằng công thức $v / v_{max}$ để sản phẩm có giá trị cao nhất sẽ đạt điểm 1.0.  
> 2. **Tiêu chí Chi phí (Cost):** Giá trị thực tế càng cao thì lại càng bất lợi và rủi ro cho doanh nghiệp (ví dụ số ngày tồn kho càng dài thì vốn bị chôn càng lâu, hoặc chi phí marketing càng lớn thì càng tốn kém).  
> Do đó, bắt buộc phải dùng công thức nghịch đảo $v_{min} / v$. Khi đó, sản phẩm nào có số ngày tồn kho ít nhất (an toàn vốn nhất) hoặc chi phí marketing thấp nhất (tiết kiệm nhất) sẽ đạt điểm 1.0 tối đa, còn sản phẩm nào tồn kho lâu và tốn kém sẽ bị điểm thấp. Nhờ chuẩn hóa về cùng một chiều 'điểm càng cao càng tốt', hàm tổng hợp AHP mới bảo đảm tính chính xác ạ."*

---

### ❓ CÂU HỎI 5:
**"Hệ thống xử lý như thế nào nếu trong lúc đang chạy thì bị cúp điện, rút cáp mạng hoặc dịch vụ SQL Server bị tắt đột ngột?"**

🗣️ **[LỜI THOẠI TRẢ LỜI CỦA BẠN]:**  
> *"Dạ thưa Thầy/Cô, hệ thống được thiết kế theo chuẩn kiến trúc chống lỗi (Fault-Tolerant Enterprise):  
> 1. Mọi truy vấn đọc/ghi đều được bao bọc trong khối `try-catch` kiểm tra lỗi mạng chuyên biệt thông qua lớp `DatabaseGuard.IsConnectionError` (bắt chính xác các mã lỗi mất kết nối máy chủ SQL như -1, 2, 53, 10061).  
> 2. Ứng dụng chạy một Timer nhịp tim nền kiểm tra kết nối định kỳ 4 giây một lần mà không làm đơ giao diện UI. Khi phát hiện mất tín hiệu SQL, một panel bảo vệ toàn màn hình (`pnlDbOverlay`) lập tức kích hoạt để khóa cứng các tab chức năng, ngăn không cho người dùng đưa ra quyết định sai trên dữ liệu đứt gãy.  
> 3. Khi SQL Server hoạt động trở lại, người dùng chỉ cần nhấn nút 'Thử kết nối lại', hệ thống sẽ tự động gọi lệnh `SqlConnection.ClearAllPools()` để giải phóng các kết nối chết, nạp lại dữ liệu mới nhất và mở khóa bình thường mà không cần phải khởi động lại ứng dụng ạ."*

---
*Tài liệu được cập nhật đầy đủ và đồng bộ cùng mã nguồn thực tế tại kho lưu trữ dự án **DSS Retail Decision Studio 2026**.*
