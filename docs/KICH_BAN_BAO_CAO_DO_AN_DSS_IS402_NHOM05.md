# DUY TÂN UNIVERSITY – INTERNATIONAL SCHOOL
## HỌC PHẦN: IS 402 A – HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH (DSS)
**Ngày báo cáo / bảo vệ đồ án: 28/10/2026**

---

# KỊCH BẢN THUYẾT TRÌNH BÁO CÁO TOÀN DIỆN VỀ HỆ THỐNG
### (LỜI THOẠI ĐỌC NGUYÊN VĂN TỪNG CHỮ – TELEPROMPTER SCRIPT BÁO CÁO TRƯỚC LỚP)
**ĐỀ TÀI: HỆ THỐNG DSS RETAIL STUDIO 2026 – PHÂN TÍCH THỨ BẬC SAATY HỖ TRỢ RA QUYẾT ĐỊNH DANH MỤC SẢN PHẨM CHIẾN LƯỢC NHẬP HÀNG & TIẾP THỊ QUÝ 4/2026**

* **Giảng viên hướng dẫn (Instructor):** ThS. Nguyễn Quang Ánh
* **Nhóm sinh viên thực hiện (Nhóm 05):**
  1. **Trần Hồng Minh** *(Trưởng nhóm - MSSV: 29210247142)*
  2. **Lê Thị Nga** *(MSSV: 29206361029)*
  3. **Phạm Ngọc Tú** *(MSSV: 29204347819)*
  4. **Lê Thanh Diệu** *(MSSV: 29204622434)*
  5. **Hồ Thị Thuận** *(MSSV: 29201164457)*
* **Ứng dụng kiểm thử:** DSS Retail Decision Studio 2026 (C# .NET 10 WinForms Desktop Enterprise Client)
* **Cơ sở dữ liệu:** Microsoft SQL Server 2022 Express (`localhost\SQLEXPRESS25` – Database: `DSS_SalesDB`)
* **Thời lượng báo cáo chuẩn:** 15 – 20 Phút (Thuyết trình lý thuyết + 10 Bước Live Demo + Trả lời phản biện Q&A)

> 💡 **HƯỚNG DẪN DÀNH CHO BẠN (NGƯỜI THUYẾT TRÌNH TRƯỚC LỚP):**  
> * Tài liệu này được biên soạn dưới dạng **Lời thoại đọc trực tiếp (Teleprompter Script)**.  
> * Bạn chỉ việc nhìn vào màn hình và **đọc to, rõ ràng, dứt khoát từng câu chữ**, không cần phải tự nghĩ lời thoại.  
> * Mỗi phân đoạn đều có 2 khối trực quan:  
>   * 🖥️ **[THAO TÁC TRÊN MÀN HÌNH / SLIDE / PHẦN MỀM]:** Hướng dẫn chính xác bạn hoặc thành viên phụ trách máy tính cần nhấp chuột vào đâu, chiếu cửa sổ nào.  
>   * 🗣️ **[LỜI THOẠI CỦA BẠN - ĐỌC NGUYÊN VĂN]:** Lời văn trau chuốt, tự tin, mang đậm tính học thuật và nghiệp vụ thực tế.

---

## BẢNG PHÂN CÔNG NHIỆM VỤ THUYẾT TRÌNH (SPEAKER ROLES)

| Thành viên | Vai trò chính | Nhiệm vụ trên bục báo cáo | Phân đoạn phụ trách |
| :--- | :--- | :--- | :--- |
| **Trần Hồng Minh**<br>*(Trưởng nhóm)* | Thuyết trình chính (Speaker 1) & Điều phối | Chào hỏi, Đặt bài toán kinh doanh Q4, Tổng quan giải pháp DSS, Kết luận & Trả lời câu hỏi 1, 2 | Phần 1, Phần 5 & Q&A |
| **Lê Thị Nga** | Thuyết trình kỹ thuật CSDL (Speaker 2) | Trình bày Kiến trúc CSDL SQL Server 3NF, ERD 11 bảng, Cơ chế bảo vệ DatabaseGuard & Trả lời câu hỏi 5 | Phần 2 & Q&A |
| **Phạm Ngọc Tú** | Thuyết trình Toán học AHP (Speaker 3) | Trình bày Bộ não toán học AHP Saaty, 5 tiêu chí, kiểm định $CR \le 0.10$, chuẩn hóa Benefit/Cost & Trả lời câu 3, 4 | Phần 3 & Q&A |
| **Lê Thanh Diệu** | Thao tác máy tính 1 (Operator 1) | Điều khiển màn hình trình chiếu: Đăng nhập hệ thống, Tab 1 Ma trận Saaty, Tab 2 Chạy DSS 9 sản phẩm, Tab 3 What-If | Phần 4 (Bước 1 – 5) |
| **Hồ Thị Thuận** | Thao tác máy tính 2 (Operator 2) & Thuyết minh | Điều khiển màn hình: Tab 3 Sơ đồ cây AHP, Tab 4 Thêm sản phẩm, Tab 5 OLAP, Tab 6 Xuất Word & Màn test tắt SQL Server | Phần 4 (Bước 6 – 10) |

> 📌 **Lưu ý trong trường hợp Thầy yêu cầu 1 người nói toàn bộ:**  
> Bạn Trưởng nhóm **Trần Hồng Minh** sẽ đọc xuyên suốt từ Phần 1 đến Phần 6 theo toàn bộ kịch bản này; các bạn còn lại sẽ đứng bên cạnh hỗ trợ thao tác máy tính và sẵn sàng tiếp ứng phần trả lời câu hỏi phản biện.

---

## MỤC LỤC KỊCH BẢN THUYẾT TRÌNH

1. [Phần 1: Mở Đầu & Đặt Bài Toán Kinh Doanh Thực Tế (00:00 – 02:30)](#phần-1-mở-đầu--đặt-bài-toán-kinh-doanh-thực-tế)
2. [Phần 2: Tổng Quan Kiến Trúc Hệ Thống & Cơ Sở Dữ Liệu SQL Server (02:30 – 05:00)](#phần-2-tổng-quan-kiến-trúc-hệ-thống--cơ-sở-dữ-liệu-sql-server)
3. [Phần 3: Bộ Não Toán Học AHP Saaty & Cơ Chế Khử Bỏ Cảm Tính (05:00 – 08:30)](#phần-3-bộ-não-toán-học-ahp-saaty--cơ-chế-khử-bỏ-cảm-tính)
4. [Phần 4: Kịch Bản Live Demo 10 Bước Thực Tế Trên Phần Mềm (08:30 – 15:30)](#phần-4-kịch-bản-live-demo-10-bước-thực-tế-trên-phần-mềm)
5. [Phần 5: Tổng Kết & Mời Hội Đồng Đặt Câu Hỏi (15:30 – 16:30)](#phần-5-tổng-kết--mời-hội-đồng-đặt-câu-hỏi)
6. [Phần 6: Bộ Câu Trả Lời Phản Biện Chuẩn Xác 100% (7 Câu Hỏi Hóc Búa Nhất)](#phần-6-bộ-câu-trả-lời-phản-biện-chuẩn-xác-100)
7. [Phần 7: Bảng Checklist Chuẩn Bị Trước Giờ Báo Cáo](#phần-7-bảng-checklist-chuẩn-bị-trước-giờ-báo-cáo)

---

## PHẦN 1: MỞ ĐẦU & ĐẶT BÀI TOÁN KINH DOANH THỰC TẾ
*(Thời lượng khuyến nghị: 02 phút 30 giây | Phân vai: Trần Hồng Minh - Trưởng nhóm)*

🖥️ **[THAO TÁC TRÊN MÀN HÌNH]:**  
* Mở Slide thuyết trình chế độ Slide Show (phím F5).  
* Chiếu **Slide 1** hiển thị rõ tên đề tài: **"HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH BÁN HÀNG: DSS RETAIL STUDIO 2026"**.  
* Sau đó chuyển sang **Slide 2**: Biểu đồ thể hiện doanh số Quý 4 bùng nổ nhưng tỷ lệ tồn kho và rủi ro đọng vốn cũng cao nhất năm.  
* Người thuyết trình đứng thẳng, mắt hướng về Thầy ThS. Nguyễn Quang Ánh và toàn thể các bạn trong lớp, giọng nói to rõ, đĩnh đạc và tự tin.

🗣️ **[LỜI THOẠI CỦA BẠN - ĐỌC NGUYÊN VĂN TỪNG CHỮ]:**  
> *"Kính thưa Thầy ThS. Nguyễn Quang Ánh cùng toàn thể các bạn sinh viên có mặt trong buổi báo cáo đồ án ngày hôm nay!  
> 
> Hôm nay, em xin đại diện Nhóm 05 trân trọng trình bày báo cáo đồ án kết thúc học phần Hệ Thống Thông Tin Hỗ Trợ Ra Quyết Định (IS 402 A) với đề tài: **Xây dựng Hệ thống DSS Retail Decision Studio 2026 ứng dụng mô hình phân tích thứ bậc AHP của Saaty để giải quyết bài toán lựa chọn danh mục sản phẩm chiến lược nhập hàng và tiếp thị Quý 4/2026**.  
> 
> Kính thưa Thầy và các bạn, trong ngành bán lẻ thiết bị công nghệ số, Quý 4 và dịp cận Tết Nguyên Đán luôn là 'mùa vàng' bùng nổ doanh số của cả năm. Tuy nhiên, đây cũng chính là 'bãi mìn' rủi ro tài chính lớn nhất đối với các nhà quản trị. Một doanh nghiệp nếu ra quyết định nhập hàng dựa vào thói quen, cảm tính cá nhân hoặc chỉ chăm chăm nhìn vào một chỉ số đơn lẻ như doanh thu thì chắc chắn sẽ gặp phải hai bài toán nan giải:  
> 
> **Thứ nhất:** Nhập quá nhiều các dòng máy cao cấp đắt tiền, dẫn đến đọng vốn hàng chục tỷ đồng khi sức mua thị trường đảo chiều; chi phí lãi vay và chi phí lưu kho tăng vọt, ăn mòn toàn bộ lợi nhuận doanh nghiệp.  
> **Thứ hai:** Ngược lại, nếu quá dè dặt vì sợ rủi ro, không kịp thời rót vốn vào các sản phẩm đang có xu hướng thị trường bùng nổ, doanh nghiệp sẽ bị 'cháy hàng' (out-of-stock), đánh mất trực tiếp khách hàng và thị phần vào tay đối thủ cạnh tranh.  
> 
> Điểm mấu chốt khó khăn nhất ở đây là người ra quyết định phải cùng lúc cân đối **5 tiêu chí mang tính xung đột lẫn nhau**: Doanh nghiệp vừa muốn biên lợi nhuận gộp cao, doanh thu kỳ vọng lớn, nhu cầu thị trường đón nhận mạnh mẽ; nhưng đồng thời lại đòi hỏi số ngày tồn kho phải thật ngắn để quay vòng vốn nhanh, và chi phí tiếp thị marketing phải thật tiết kiệm.  
> 
> Đứng trước bài toán hóc búa đó, nhóm chúng em đã nghiên cứu và phát triển phần mềm **DSS Retail Decision Studio 2026**. Hệ thống kết hợp chặt chẽ giữa **Cơ sở dữ liệu quan hệ Microsoft SQL Server** và **Mô hình toán học đa tiêu chí AHP** nhằm chuyển hóa toàn bộ dữ liệu lịch sử bán hàng thành các khuyến nghị kinh doanh định lượng, khách quan và minh bạch 100%, tuyệt đối nói không với các quyết định mang tính cảm tính."*

---

## PHẦN 2: TỔNG QUAN KIẾN TRÚC HỆ THỐNG & CƠ SỞ DỮ LIỆU SQL SERVER
*(Thời lượng khuyến nghị: 02 phút 30 giây | Phân vai: Lê Thị Nga)*

🖥️ **[THAO TÁC TRÊN MÀN HÌNH]:**  
* Chuyển sang **Slide 3** hiển thị sơ đồ kiến trúc 3 tầng: `C# .NET 10 WinForms UI <---> DssEngine <---> Microsoft SQL Server 2022 Express`.  
* Chuyển sang **Slide 4** phóng to sơ đồ ERD hiển thị rõ mối quan hệ giữa 11 bảng thực thể đạt chuẩn 3NF.  
* Dùng con trỏ chuột khoanh vùng 3 phân hệ CSDL để người xem dễ dàng nắm bắt.

🗣️ **[LỜI THOẠI CỦA BẠN - ĐỌC NGUYÊN VĂN TỪNG CHỮ]:**  
> *"Kính thưa Thầy và các bạn, tiếp theo em xin phép trình bày về nền tảng kiến trúc công nghệ và cơ sở dữ liệu của hệ thống.  
> 
> Hệ thống DSS Retail Decision Studio 2026 được xây dựng theo kiến trúc 3 tầng hiện đại (Enterprise 3-Tier Architecture): Tầng giao diện người dùng C# .NET 10 Windows Forms Desktop Client; Tầng tính toán logic nghiệp vụ và mô hình toán học DssEngine; và Tầng lưu trữ dữ liệu tập trung trên Microsoft SQL Server 2022 Express instance `.\SQLEXPRESS25`.  
> 
> Cơ sở dữ liệu của chúng em có tên là **DSS_SalesDB**, được thiết kế chuẩn hóa đạt chuẩn **3NF (Dạng chuẩn 3)** với **11 bảng thực thể**, phân định rành mạch thành 3 phân hệ cốt lõi:  
> 
> 1. **Phân hệ Nghiệp vụ Bán lẻ:** Quản lý toàn bộ vòng đời kinh doanh thực tế gồm các bảng: Danh mục (`CATEGORIES`), Sản phẩm (`PRODUCTS`), Khách hàng (`CUSTOMERS`), Chương trình khuyến mãi (`PROMOTIONS`), Đơn hàng (`ORDERS`) và Chi tiết đơn hàng (`ORDER_DETAILS`). Toàn bộ dữ liệu doanh thu, sản lượng và thời gian bán đều được tổng hợp từ phân hệ này.  
> 2. **Phân hệ Hỗ trợ Ra Quyết định DSS:** Gồm bảng Phiên quyết định (`DECISION_SESSIONS`), Danh mục 5 tiêu chí (`CRITERIA`), Ma trận so sánh cặp Saaty (`PAIRWISE_COMPARISONS`), Danh sách phương án ứng viên (`ALTERNATIVES`), Điểm đánh giá thô và chuẩn hóa (`ALTERNATIVE_SCORES`), và Bảng kết quả xếp hạng cuối cùng (`DECISION_RESULTS`).  
> 3. **Phân hệ Quản trị Người dùng & Bảo mật:** Bảng `USERS` lưu trữ tài khoản nhân sự với mật khẩu được băm một chiều an toàn bằng thuật toán mã hóa **SHA-256**, áp dụng phân quyền nghiêm ngặt theo 4 vai trò: Admin, DecisionMaker, SalesManager và SalesStaff.  
> 
> 💡 **Đặc biệt, nhóm chúng em đặt ra một nguyên tắc thiết kế bất biến:**  
> **Hệ thống phụ thuộc 100% vào SQL Server thật.** Ứng dụng tuyệt đối không dùng dữ liệu mẫu gán cứng trong code hay tài khoản giả lập. Để đảm bảo tính toàn vẹn, chúng em đã cài đặt cơ chế **DatabaseGuard & Heartbeat Monitor** chạy ngầm mỗi 4 giây. Nếu dịch vụ SQL Server bị ngắt hoặc mất kết nối mạng, hệ thống lập tức kích hoạt màn hình khóa bảo vệ màu đỏ để ngăn chặn việc ra quyết định sai trên dữ liệu đứt gãy. Lát nữa trong phần Live Demo, nhóm em sẽ biểu diễn bài test tắt SQL Server trực tiếp để chứng minh cơ chế này."*

---

## PHẦN 3: BỘ NÃO TOÁN HỌC AHP SAATY & CƠ CHẾ KHỬ BỎ CẢM TÍNH
*(Thời lượng khuyến nghị: 03 phút 30 giây | Phân vai: Phạm Ngọc Tú)*

🖥️ **[THAO TÁC TRÊN MÀN HÌNH]:**  
* Chiếu **Slide 5**: Sơ đồ cấu trúc cây thứ bậc AHP 3 tầng (Tầng 1 Mục tiêu $\to$ Tầng 2: 5 Tiêu chí $\to$ Tầng 3: 9 Sản phẩm ứng viên).  
* Chiếu **Slide 6**: Bảng ma trận so sánh cặp $5 \times 5$, vector trọng số $W$, các công thức toán tính $\lambda_{\max}$, $CI$, $RI$ và tỷ số nhất quán $CR = 0.0138$.  
* Chỉ rõ công thức chuẩn hóa Benefit ($v/v_{\max}$) và Cost ($v_{\min}/v$).

🗣️ **[LỜI THOẠI CỦA BẠN - ĐỌC NGUYÊN VĂN TỪNG CHỮ]:**  
> *"Kính thưa Thầy và các bạn, em xin phép đi sâu vào phần 'bộ não' toán học giúp hệ thống ra quyết định chuẩn xác: **Phương pháp phân tích thứ bậc AHP của Giáo sư Thomas L. Saaty**.  
> 
> Trong phiên quyết định Quý 4/2026, chúng em thiết lập 5 tiêu chí đánh giá đa chiều:  
> * **Tiêu chí $C_1$ – Biên lợi nhuận gộp:** Tiêu chí Lợi ích (Benefit), phản ánh tỷ suất sinh lời trên mỗi đồng vốn bán ra.  
> * **Tiêu chí $C_2$ – Doanh thu kỳ vọng:** Tiêu chí Lợi ích (Benefit), đo lường quy mô dòng tiền đóng góp cho doanh nghiệp.  
> * **Tiêu chí $C_3$ – Nhu cầu thị trường:** Tiêu chí Lợi ích (Benefit), đo lường mức độ quan tâm và độ 'nóng' của người tiêu dùng.  
> * **Tiêu chí $C_4$ – Rủi ro tồn kho:** Tiêu chí Chi phí (Cost), đo lường số ngày lưu kho bình quân, số ngày càng thấp thì rủi ro chôn vốn càng ít.  
> * **Tiêu chí $C_5$ – Chi phí Marketing:** Tiêu chí Chi phí (Cost), đo lường ngân sách tiếp thị cần bỏ ra để bán được hàng, chi phí càng thấp thì càng tối ưu.  
> 
> Quy trình xử lý AHP trong hệ thống diễn ra qua 4 bước toán học nghiêm ngặt:  
> 
> **Bước 1 – Xác định trọng số từ ma trận so sánh cặp Saaty 5×5:**  
> Các chuyên gia chấm điểm mức độ quan trọng tương đối giữa các cặp tiêu chí theo thang điểm từ 1 đến 9 của Saaty. Hệ thống chuẩn hóa từng cột và lấy trung bình cộng theo hàng để suy ra Vector trọng số $W$. Kết quả tính toán thực tế trong CSDL của nhóm là: Trọng số Biên lợi nhuận $C_1$ chiếm **41.62%**; Doanh thu $C_2$ chiếm **26.18%**; Nhu cầu thị trường $C_3$ chiếm **16.11%**; Rủi ro tồn kho $C_4$ chiếm **9.86%**; và Chi phí Marketing $C_5$ chiếm **6.24%**. Tổng trọng số bằng đúng 100%.  
> 
> **Bước 2 – Kiểm định chỉ số nhất quán logic toán học ($CR$):**  
> Đây là ưu thế tuyệt đối của AHP so với các phương pháp cho điểm cảm tính khác. Hệ thống tính giá trị riêng cực đại $\lambda_{\max} = 5.0683$, chỉ số nhất quán $CI = (\lambda_{\max} - 5) / (5 - 1) = 0.0171$, tra bảng chỉ số ngẫu nhiên với $n=5$ thì $RI = 1.12$.  
> Tỷ số nhất quán: $CR = CI / RI = 0.0138$, tức chỉ **1.38%**.  
> Vì $1.38\% \le 10\%$, ma trận đảm bảo tính nhất quán logic toán học tuyệt đối, không hề có sự mâu thuẫn hay cảm tính của chuyên gia.  
> 
> **Bước 3 – Chuẩn hóa điểm số thô đưa về thang đo từ 0 đến 1:**  
> Để có thể cộng gộp các đại lượng có đơn vị đo khác nhau (như phần trăm lợi nhuận, triệu đồng doanh thu, số ngày tồn kho), hệ thống tự động chuẩn hóa:  
> * Đối với tiêu chí Lợi ích: Lấy giá trị chia cho giá trị lớn nhất ($v / v_{\max}$).  
> * Đối với tiêu chí Chi phí: Lấy giá trị nhỏ nhất chia cho giá trị thực tế ($v_{\min} / v$) để sản phẩm nào chi phí thấp nhất sẽ đạt điểm 1.0 tối đa.  
> 
> **Bước 4 – Tính điểm tổng hợp và đề xuất lượng đặt hàng:**  
> Điểm AHP cuối cùng là tích vô hướng giữa Vector trọng số $W$ và Vector điểm chuẩn hóa. Sản phẩm Hạng 1 được gắn nhãn Phê duyệt, Hạng 2 và 3 thuộc diện Xem xét nhập thăm dò, còn lại Loại bỏ. Lượng hàng đề xuất nhập kho được tính tự động từ lịch sử bán hàng trong bảng `ORDERS` chia cho tổng số ngày bán, nhân với chu kỳ đặt hàng 30 ngày cộng tồn kho an toàn trừ tồn kho hiện tại. Hoàn toàn không có bất kỳ con số gán bừa nào!"*

---

## PHẦN 4: KỊCH BẢN LIVE DEMO 10 BƯỚC THỰC TẾ TRÊN PHẦN MỀM
*(Thời lượng khuyến nghị: 07 phút 00 giây | Phân vai: Lê Thanh Diệu & Hồ Thị Thuận thao tác, Trần Hồng Minh thuyết minh)*

🖥️ **[CHUẨN BỊ MÀN HÌNH]:**  
* Thu nhỏ Slide PowerPoint xuống taskbar.  
* Trở về màn hình Desktop máy tính.

---

### 🔹 BƯỚC 1: KHỞI ĐỘNG SPLASH SCREEN & KIỂM TRA KẾT NỐI SQL SERVER
🖥️ **[THAO TÁC]:**  
* Nhấp đúp chuột vào biểu tượng shortcut ngoài màn hình Desktop hoặc chạy file `DSS_App.exe`.  
* Màn hình Splash Screen màu xanh tím hiện đại xuất hiện với logo và thanh tiến trình chạy mượt mà từ 0% đến 100%.  
* Chỉ tay vào dòng thông báo trạng thái dưới thanh tiến trình: *"Đang kiểm tra kết nối SQL Server instance SQLEXPRESS25..."*.

🗣️ **[LỜI THOẠI]:**  
> *"Và sau đây, để Thầy và các bạn thấy rõ sự vận hành thực tế, em xin phép bắt đầu phần Live Demonstration trực tiếp trên phần mềm C# .NET 10.  
> Đầu tiên, em khởi chạy ứng dụng. Màn hình Splash Screen xuất hiện.  
> Quý Thầy và các bạn có thể thấy hệ thống đang tự động gửi gói tin ping kiểm tra dịch vụ SQL Server instance `.\SQLEXPRESS25` và nạp bảng dữ liệu `DSS_SalesDB`. Khi và chỉ khi kết nối thành công, hệ thống mới cấp quyền mở Cổng đăng nhập."*

---

### 🔹 BƯỚC 2: PHÂN QUYỀN RBAC & ĐĂNG NHẬP TÀI KHOẢN THẬT
🖥️ **[THAO TÁC]:**  
* Màn hình LoginForm mở ra với thiết kế kính mờ cao cấp.  
* Nhập Tên đăng nhập: `admin` (hoặc `giamdoc_q4`).  
* Nhập Mật khẩu: `Admin@123` (mật khẩu hiển thị dưới dạng dấu chấm bảo mật).  
* Bấm nút **"ĐĂNG NHẬP HỆ THỐNG"** màu xanh tím.  
* Hệ thống xác thực mã băm SHA-256 trong bảng `dbo.USERS` $\to$ Chuyển vào giao diện chính Form1.  
* Rê chuột chỉ vào góc trên cùng bên phải thanh tiêu đề Form1 hiển thị dòng chữ: **QUẢN TRỊ VIÊN HỆ THỐNG [Admin]** hoặc **TRẦN VĂN GIÁM ĐỐC [DecisionMaker]**.

🗣️ **[LỜI THOẠI]:**  
> *"Tại màn hình đăng nhập, hệ thống áp dụng cơ chế xác thực bảo mật chuẩn Enterprise với mật khẩu băm SHA-256.  
> Em đăng nhập bằng tài khoản của Giám đốc kinh doanh có vai trò DecisionMaker. Khi đăng nhập thành công, ở góc trên cùng bên phải thanh tiêu đề, hệ thống hiển thị chính xác tên người dùng và vai trò được kéo trực tiếp từ CSDL.  
> Cơ chế phân quyền RBAC này đảm bảo chỉ những người có thẩm quyền mới được kích hoạt chạy mô hình ra quyết định."*

---

### 🔹 BƯỚC 3: TAB 1 - KIỂM TRA ĐẦU VÀO DỮ LIỆU & MA TRẬN SAATY 5x5
🖥️ **[THAO TÁC]:**  
* Mặc định mở Form1 ở Tab 1 **[1. Đầu Vào Dữ Liệu (Inputs & Parameters)]**.  
* Dùng chuột cuộn bảng bên trái: Danh mục đầy đủ **9 sản phẩm ứng viên Quý 4** với Giá vốn, Giá bán, Tồn kho, Nhu cầu, Biên lợi nhuận.  
* Chuyển chuột sang bảng bên phải: Ma trận so sánh cặp Saaty 5×5 giữa 5 tiêu chí.  
* Chỉ con trỏ chuột vào dòng trạng thái màu xanh lá ở góc dưới bên phải ma trận.

🗣️ **[LỜI THOẠI]:**  
> *"Tại Tab 1, hệ thống hiển thị toàn bộ tham số đầu vào được nạp trực tiếp từ SQL Server:  
> Phía bên trái là danh mục 9 sản phẩm ứng viên chiến lược đại diện cho các ngành hàng chủ lực: Từ Điện thoại cao cấp Samsung S24 Ultra, Laptop Dell Gaming G15, Đồng hồ Apple Watch Ultra 2, Loa Bluetooth SoundMax, iPad Gen 10, Laptop Acer Aspire 7, cho đến Màn hình Dell UltraSharp, Tai nghe Sony XM5 và Chuột Logitech MX Master 3S.  
> Phía bên phải là Ma trận so sánh cặp giữa 5 tiêu chí.  
> Xin Thầy và các bạn đặc biệt lưu ý dòng chữ màu xanh lá ở góc dưới bên phải: Hệ thống ghi rõ `✓ Tỷ số nhất quán: CR = 0.0138 (1.38%) [ĐẠT CHUẨN SAATY CR <= 0.10] • λmax = 5.0683`. Con số này được tính toán động từ CSDL ngay khi mở form. Vì $1.38\% < 10\%$, dữ liệu hoàn toàn hợp lệ để tiến hành thẩm định."*

---

### 🔹 BƯỚC 4: TAB 2 - KÍCH HOẠT MÔ HÌNH DSS & BẢNG XẾP HẠNG 9 SẢN PHẨM
🖥️ **[THAO TÁC]:**  
* Nhấp chuột chuyển sang Tab 2 **[2. Kết Quả Ra Quyết Định (Outputs & Decision)]**.  
* Màn hình ban đầu hiển thị nút nổi bật: **"🚀 BẤM VÀO ĐÂY ĐỂ CHẠY MÔ HÌNH DSS NGAY"**.  
* Bấm chuột vào nút chạy mô hình $\to$ Hộp thoại thông báo: *"MÔ HÌNH HỖ TRỢ RA QUYẾT ĐỊNH ĐÃ CHẠY HOÀN TẤT!"* $\to$ Bấm OK.  
* Rê chuột qua 4 thẻ KPI trên cùng (Sản phẩm tối ưu số 1, Điểm AHP cao nhất, Doanh thu kỳ vọng, Biên lợi nhuận).  
* Cuộn xem Bảng xếp hạng chi tiết 9 dòng có gắn nhãn màu: Hạng 1 (Huy chương Vàng – Xanh lá Phê duyệt), Hạng 2–3 (Huy chương Bạc/Đồng – Vàng cam Xem xét), Hạng 4–9 (Đỏ Loại bỏ).  
* Chỉ vào Biểu đồ cột AHP Score Chart trực quan hiển thị đầy đủ 9 sản phẩm.  
* Cuộn xuống hộp "Nhận Xét & Khuyến Nghị Điều Hành" và cột "Lượng Đề Xuất Nhập".

🗣️ **[LỜI THOẠI]:**  
> *"Bây giờ em chuyển sang Tab 2 – Kết Quả Đầu Ra. Em nhấn nút: **'BẤM VÀO ĐÂY ĐỂ CHẠY MÔ HÌNH DSS NGAY'**.  
> Chỉ mất chưa đầy 1 giây, câu lệnh SQL MERGE và thuật toán AHP đã thực thi xong. Kết quả hiện ra cực kỳ trực quan:  
> 1. **Bốn thẻ KPI hàng đầu:** Sản phẩm tối ưu số 1 được vinh danh là **Samsung Galaxy S24 Ultra** với Điểm số AHP áp đảo **0.9102 / 1.0000**; Doanh thu kỳ vọng đạt **1,200 Triệu VNĐ**; và Biên lợi nhuận gộp đạt **26.29%**.  
> 2. **Bảng xếp hạng 9 sản phẩm:** Hạng 1 Samsung S24 Ultra nhận Huy chương Vàng và nhãn Phê duyệt. Hạng 2 Dell Gaming G15 (điểm 0.7388) và Hạng 3 Apple Watch Ultra 2 (điểm 0.6725) nhận nhãn Xem xét để nhập thăm dò. Các sản phẩm còn lại từ Hạng 4 đến Hạng 9 nhận nhãn Loại bỏ để tránh đọng vốn.  
> 3. **Biểu đồ cột bên phải:** Vẽ tự động toàn bộ 9 cột điểm số, giúp Ban giám đốc so sánh ngay khoảng cách vượt trội giữa sản phẩm dẫn đầu với các phương án còn lại.  
> 4. **Cột 'Lượng Đề Xuất Nhập' và Hộp chỉ đạo AI phía dưới:** Hệ thống chỉ đạo nhập ngay 50 chiếc Samsung S24 Ultra theo công thức bù đắp tồn kho từ lịch sử bán hàng thực tế, đồng thời phân bổ 45% ngân sách marketing cho sản phẩm này."*

---

### 🔹 BƯỚC 5: TAB 3 - MÔ PHỎNG BIẾN ĐỘNG THỊ TRƯỜNG (WHAT-IF SENSITIVITY)
🖥️ **[THAO TÁC]:**  
* Nhấp chuyển sang Tab 3 **[3. Biến Động Thị Trường (What-If)]**.  
* Bấm nút kịch bản màu cam: **"🔥 Kịch Bản 1: Bùng Nổ Gaming (+40% Nhu Cầu)"**. Quan sát điểm Dell Gaming G15 tăng vọt trên Biểu đồ so sánh Delta và bảng đối chiếu.  
* Bấm nút kịch bản màu đỏ: **"⚠️ Kịch Bản 2: Đứt Gãy Chuỗi Cung Ứng (+35% Tồn Kho)"**.  
* Bấm nút kịch bản màu xanh dương: **"💼 Kịch Bản 3: Thắt Chặt Marketing (-30% Chi Phí)"**.  
* Bấm nút màu xám: **"🔄 Khôi Phục Dữ Liệu Gốc (Baseline)"** để trả về dữ liệu chuẩn ban đầu.

🗣️ **[LỜI THOẠI]:**  
> *"Tiếp theo là tính năng cốt lõi của một hệ thống DSS hiện đại: **Phân tích độ nhạy và Mô phỏng kịch bản What-If**.  
> Môi trường kinh doanh luôn biến động khôn lường, DSS phải giúp nhà quản trị trả lời được câu hỏi: *'Điều gì xảy ra nếu thị trường thay đổi?'*.  
> Em bấm vào **Kịch Bản 1: Bùng Nổ Thị Trường Gaming (+40% Nhu Cầu)**. Thầy và các bạn quan sát thấy ngay trên Biểu đồ Delta: Điểm số của Laptop Dell Gaming G15 lập tức tăng vọt từ 0.73 lên cạnh tranh gay gắt vị trí số 1 với Samsung S24 Ultra.  
> Khi bấm Kịch bản 2 Đứt gãy chuỗi cung ứng, các sản phẩm tồn kho lâu lập tức bị tụt hạng.  
> Và cuối cùng, em bấm nút **'Khôi Phục Dữ Liệu Gốc (Baseline)'**: Hệ thống lập tức gọi hàm nạp lại giá trị thô ban đầu trong SQL Server và tính lại đường cơ sở chuẩn xác, hoàn toàn không làm sai lệch CSDL gốc."*

---

### 🔹 BƯỚC 6: TAB 3 - SƠ ĐỒ CÂY CẤU TRÚC PHÂN CẤP AHP TƯƠNG TÁC
🖥️ **[THAO TÁC]:**  
* Tại Tab 3, bấm vào nút màu xanh lá cây đậm trên thanh công cụ: **"🌲 Sơ Đồ Cây Cấu Trúc Thứ Bậc AHP"**.  
* Cửa sổ Popup *"Cấu Trúc Phân Cấp AHP (Analytic Hierarchy Process)"* mở rộng trực quan.  
* Chỉ vào Node gốc Tầng 1: Mục tiêu tối ưu Quý 4/2026.  
* Chỉ vào 5 Node Tầng 2: 5 Tiêu chí kèm trọng số % và tỷ số $CR = 0.0138$.  
* Chỉ vào 9 Node Tầng 3: 9 Sản phẩm ứng viên dàn đều toàn bộ chiều rộng, có mũi tên vector nối trực tiếp từ các tiêu chí xuống từng sản phẩm mà không bị khuất hay đè chữ.

🗣️ **[LỜI THOẠI]:**  
> *"Tại Tab 3, khi bấm nút 'Sơ Đồ Cây Cấu Trúc Thứ Bậc AHP', hệ thống sẽ trực quan hóa toàn bộ mô hình toán học dưới dạng cây phân cấp 3 tầng theo đúng nguyên lý của Saaty:  
> Tầng 1 là Mục tiêu tổng thể; Tầng 2 là 5 Tiêu chí chiến lược với tỷ trọng phần trăm và chỉ số nhất quán CR; và Tầng 3 là toàn bộ 9 Sản phẩm ứng viên được căn chỉnh tỷ lệ tự động, kết nối bằng các đường mũi tên vector rõ ràng, không hề bị khuất hay đè chữ.  
> Sơ đồ này giúp các nhà quản trị không chuyên về toán học cũng có thể hiểu thấu đáo bản chất của quyết định."*

---

### 🔹 BƯỚC 7: TAB 4 - THẨM ĐỊNH ĐỘNG KHI THÊM SẢN PHẨM MỚI
🖥️ **[THAO TÁC]:**  
* Bấm chuyển sang Tab 4 **[4. Thêm Sản Phẩm Mới]**.  
* Nhập nhanh các trường thông tin cho một sản phẩm mới:  
  * Tên sản phẩm: `Bàn Phím Cơ Gaming K90 Pro`  
  * Mã SKU: `PRD_KEY_K90`  
  * Danh mục: `Phụ Kiện Gaming`  
  * Giá vốn: `850,000` VNĐ | Giá bán: `1,690,000` VNĐ (Ô Biên lợi nhuận bên cạnh tự động nhảy lên ~49.7%)  
  * Tồn kho: `30` chiếc | Doanh thu kỳ vọng: `750` Triệu | Nhu cầu thị trường: `85` điểm  
  * Số ngày tồn kho: `15` ngày | Chi phí tiếp thị: `25` Triệu  
* Bấm nút **"💾 LƯU SẢN PHẨM & CẬP NHẬT DSS"**.  
* Hộp thoại báo thành công $\to$ Ứng dụng tự động điều hướng về Tab 1 và cập nhật danh sách.

🗣️ **[LỜI THOẠI]:**  
> *"Tại Tab 4, doanh nghiệp có thể đưa thêm bất kỳ dòng sản phẩm mới nào vào hội đồng thẩm định.  
> Ví dụ em nhập sản phẩm Bàn Phím Cơ Gaming K90 Pro. Khi gõ Giá vốn 850 ngàn và Giá bán 1 triệu 690 ngàn, ô Biên lợi nhuận bên cạnh tự động tính toán ra gần 50%.  
> Khi em bấm nút 'Lưu Sản Phẩm & Cập Nhật DSS': Dữ liệu được ghi trực tiếp vào bảng `PRODUCTS`, `ALTERNATIVES` và `ALTERNATIVE_SCORES` trong SQL Server.  
> Điểm ưu việt kỹ thuật ở đây là: Thuật toán chuẩn hóa trong DssEngine tự động quét lại toàn bộ cực trị lớn nhất và nhỏ nhất của phiên để chuẩn hóa lại điểm số cho cả danh mục, chứ không hề dùng các hằng số chia cố định."*

---

### 🔹 BƯỚC 8: TAB 5 - PHÂN TÍCH KINH DOANH THÔNG MINH ĐA CHIỀU OLAP
🖥️ **[THAO TÁC]:**  
* Bấm chuyển sang Tab 5 **[5. Dữ Liệu Bán Hàng Lịch Sử (OLAP)]**.  
* Bấm Nút 1: **"📊 1. Hiệu Suất Sản Phẩm (Fact/View)"** $\to$ Bảng hiển thị doanh thu lũy kế, giá vốn và sản lượng bán thực tế.  
* Bấm Nút 2: **"📦 2. OLAP ROLLUP: Phân Bổ Hàng Tồn Kho"** $\to$ Bảng hiển thị đa cấp độ từ Tổng giá trị tồn kho toàn công ty $\to$ Từng ngành hàng $\to$ Từng sản phẩm con.  
* Bấm Nút 3: **"🎯 3. OLAP CUBE: Đa Chiều Khuyến Mãi"** $\to$ Khảo sát hiệu quả cắt lớp đa chiều giữa chiến dịch khuyến mãi và nhóm hàng.

🗣️ **[LỜI THOẠI]:**  
> *"Tại Tab 5 là phân hệ Phân tích kinh doanh thông minh OLAP:  
> Nút 1 hiển thị View Hiệu Suất Sản Phẩm thực tế: Thống kê số lượng bán, doanh thu gộp, giá vốn và biên lợi nhuận lũy kế.  
> Nút 2 thực hiện câu lệnh truy vấn OLAP ROLLUP: Phân tích đa cấp độ tổng giá trị tồn kho theo từng Danh mục sản phẩm đến từng SKU con.  
> Nút 3 thực hiện truy vấn OLAP CUBE: Khảo sát đa chiều hiệu quả của các Chiến dịch khuyến mãi tác động lên từng nhóm mặt hàng.  
> Đây chính là kho dữ liệu thực chứng làm căn cứ để các chuyên gia thiết lập ma trận so sánh cặp Saaty một cách khách quan."*

---

### 🔹 BƯỚC 9: TAB 6 - XUẤT BIÊN BẢN WORD CHUẨN NGHỊ ĐỊNH 30 & BÁO CÁO HTML
🖥️ **[THAO TÁC]:**  
* Bấm chuyển sang Tab 6 **[6. Form Báo Cáo & AI Insights]**.  
* Cuộn chuột xuống cuối trang.  
* Bấm nút: **"📄 Xuất Biên Bản Microsoft Word (.doc)"** $\to$ Chọn nơi lưu file $\to$ Phần mềm tự động kích hoạt Microsoft Word mở file lên.  
* Rê chuột cho Thầy và cả lớp xem file Word: Có Quốc hiệu, Tiêu ngữ, Căn cứ pháp lý, Bảng tổng hợp trọng số, Bảng xếp hạng 9 sản phẩm và Khung chữ ký 3 bên.  
* Quay lại phần mềm, bấm nút: **"🌐 Xuất Báo Cáo Doanh Nghiệp HTML (In PDF)"** $\to$ Trình duyệt mở ra bản báo cáo đồ họa hiện đại.

🗣️ **[LỜI THOẠI]:**  
> *"Tại Tab 6 là nơi đóng gói toàn bộ phiên làm việc thành hồ sơ pháp lý doanh nghiệp.  
> Em nhấn nút Xuất Biên Bản Microsoft Word: Phần mềm tự động sinh ra file Word chuẩn thể thức văn bản quản lý nhà nước theo **Nghị định 30/2020/NĐ-CP**: Có Quốc hiệu, Tiêu ngữ, Bảng tổng hợp trọng số AHP, Bảng kết quả xếp hạng 9 sản phẩm, Lượng đề xuất nhập kho, và cuối trang có sẵn khung chữ ký 3 bên giữa Trưởng phòng Kinh doanh, Giám đốc Tài chính và Tổng Giám đốc.  
> Tương tự, nút Xuất Báo Cáo HTML sinh ra giao diện hiện đại phục vụ họp Ban điều hành và sẵn sàng in ấn PDF chỉ với một cú click.  
> Toàn bộ số liệu trong 2 văn bản này đều được trích xuất trực tiếp từ snapshot CSDL SQL Server của phiên hiện tại."*

---

### 🔹 BƯỚC 10: MÀN TRÌNH DIỄN ĐỈNH CAO - TẮT SQL SERVER ĐỂ THỬ THÁCH HỆ THỐNG
🖥️ **[THAO TÁC]:**  
* Thu nhỏ cửa sổ ứng dụng DSS sang nửa bên trái màn hình.  
* Mở cửa sổ Windows Services (`services.msc`) ở nửa bên phải màn hình.  
* Cuộn tìm dịch vụ: **`SQL Server (SQLEXPRESS25)`**.  
* Nhấp chuột phải chọn **Stop** để dừng dịch vụ SQL Server.  
* Chờ 3 – 4 giây: Màn hình ứng dụng DSS bên trái lập tức bị phủ kín bởi Panel màu đỏ thẫm với biểu tượng cảnh báo ⛔ và dòng chữ lớn: **"MẤT KẾT NỐI SQL SERVER – HỆ THỐNG TẠM DỪNG ĐỂ BẢO VỆ DỮ LIỆU"**.  
* Thử nhấp chuột vào các Tab hoặc nút bấm $\to$ Toàn bộ giao diện bị khóa cứng, không thao tác được.  
* Quay sang cửa sổ Services, chuột phải vào `SQL Server (SQLEXPRESS25)` chọn **Start**.  
* Quay lại ứng dụng DSS, nhấp vào nút: **"↻ Thử kết nối lại"** $\to$ Màn hình đỏ biến mất, hệ thống nạp lại dữ liệu bình thường!

🗣️ **[LỜI THOẠI - NÓI VỚI GIỌNG ĐĨNH ĐẠC, DỨT KHOÁT, TỰ TIN]:**  
> *"Và cuối cùng, để chứng minh một cách thuyết phục nhất rằng: **Phần mềm của nhóm em phụ thuộc 100% vào SQL Server thật, tuyệt đối không dùng dữ liệu giả lập hay gán cứng trong code**, em xin thực hiện một bài kiểm tra thực tế:  
> Em mở trình quản lý dịch vụ Windows Services. Quý Thầy và các bạn có thể thấy dịch vụ `SQL Server (SQLEXPRESS25)` đang hoạt động bình thường.  
> Bây giờ, em nhấn **STOP** để tắt hoàn toàn dịch vụ SQL Server...  
> *(Dừng lại 2–3 giây để cả lớp quan sát)*  
> Ngay lập tức, luồng Heartbeat Monitor của ứng dụng phát hiện mất tín hiệu CSDL và kích hoạt màn hình khóa bảo vệ màu đỏ: **'MẤT KẾT NỐI SQL SERVER – HỆ THỐNG TẠM DỪNG ĐỂ BẢO VỆ DỮ LIỆU'**. Toàn bộ các thao tác ra quyết định bị chặn đứng hoàn toàn nhằm ngăn chặn việc tính toán sai lệch trên dữ liệu đứt gãy.  
> Và bây giờ, em bật **START** lại dịch vụ SQL Server...  
> Em nhấn nút **'Thử kết nối lại'** trên ứng dụng...  
> Màn hình khóa đỏ lập tức biến mất, hệ thống tự động giải phóng kết nối chết, đồng bộ lại dữ liệu mới nhất và tiếp tục vận hành hoàn hảo!"*

---

## PHẦN 5: TỔNG KẾT & MỜI HỘI ĐỒNG ĐẶT CÂU HỎI
*(Thời lượng khuyến nghị: 01 phút 00 giây | Phân vai: Trần Hồng Minh - Trưởng nhóm)*

🖥️ **[THAO TÁC TRÊN MÀN HÌNH]:**  
* Chuyển cửa sổ PowerPoint toàn màn hình ở Slide Tổng kết.  
* Hiển thị 4 giá trị cốt lõi đạt được của đồ án và dòng chữ: *"XIN CHÂN THÀNH CẢM ƠN THẦY VÀ CÁC BẠN ĐÃ LẮNG NGHE"*.  
* Cả 5 thành viên nhóm đứng trang nghiêm trước bục giảng, hướng mắt về Thầy và lớp.

🗣️ **[LỜI THOẠI CỦA BẠN - ĐỌC NGUYÊN VĂN TỪNG CHỮ]:**  
> *"Kính thưa Thầy ThS. Nguyễn Quang Ánh cùng toàn thể các bạn thành viên trong hội đồng!  
> 
> Qua bài báo cáo thuyết trình và phần Live Demonstration 10 bước vừa rồi, Nhóm 05 chúng em xin được đúc kết 4 kết quả cốt lõi mà dự án đã đạt được:  
> 1. **Về mặt Toán học:** Ứng dụng thành công mô hình AHP của Saaty, kiểm soát tỷ số nhất quán $CR = 0.0138$ ($1.38\%$), chuyển hóa quyết định nhập hàng từ cảm tính sang định lượng khách quan.  
> 2. **Về mặt Cơ sở dữ liệu:** Xây dựng CSDL `DSS_SalesDB` trên SQL Server 2022 đạt chuẩn 3NF với 11 bảng quan hệ chặt chẽ, hỗ trợ phân tích đa chiều OLAP Rollup và Cube.  
> 3. **Về mặt Phần mềm:** Phát triển hoàn chỉnh ứng dụng C# .NET 10 WinForms với 6 Tab nghiệp vụ, phân quyền RBAC 4 vai trò, mô phỏng What-If 3 kịch bản, và tự động xuất biên bản Word chuẩn Nghị định 30.  
> 4. **Về mặt Độ tin cậy:** Cơ chế DatabaseGuard & Heartbeat Monitor chống chịu lỗi mạng, chứng minh hệ thống vận hành 100% trên dữ liệu thật.  
> 
> Nhóm chúng em xin gửi lời cảm ơn chân thành và sâu sắc nhất tới Thầy ThS. Nguyễn Quang Ánh đã tận tình hướng dẫn, truyền đạt kiến thức quý báu trong suốt học kỳ vừa qua. Chúng em rất mong nhận được những nhận xét, góp ý từ Thầy để đồ án ngày càng hoàn thiện hơn nữa.  
> **Sau đây, nhóm chúng em xin sẵn sàng lắng nghe và trả lời các câu hỏi phản biện từ Thầy và các bạn ạ!**"*

---

## PHẦN 6: BỘ CÂU TRẢ LỜI PHẢN BIỆN CHUẨN XÁC 100%
*(Khi Giảng viên hoặc Hội đồng hỏi câu nào, bạn chỉ việc tra đúng câu đó và đọc câu trả lời dưới đây)*

---

### ❓ CÂU HỎI 1:
**"Tại sao nhóm lại chọn phương pháp AHP của Saaty mà không dùng TOPSIS, PROMETHEE hay Điểm Trọng Số Giản Đơn (WSM)?"**

🗣️ **[LỜI THOẠI TRẢ LỜI CỦA BẠN]:**  
> *"Dạ thưa Thầy, nhóm em đã phân tích và so sánh kỹ lưỡng trước khi quyết định:  
> Phương pháp Điểm Trọng Số Giản Đơn (WSM) có nhược điểm chí mạng là trọng số tiêu chí thường do con người tự gán một cách chủ quan, hoàn toàn không kiểm tra được tính mâu thuẫn trong tư duy của người đánh giá.  
> Trong khi đó, **AHP của Saaty** giải quyết triệt để bài toán này nhờ cơ chế so sánh cặp (Pairwise Comparison) và **Tỷ số nhất quán logic $CR$ (Consistency Ratio)**. Nếu một chuyên gia đánh giá Biên lợi nhuận quan trọng hơn Doanh thu, Doanh thu quan trọng hơn Nhu cầu, nhưng sau đó lại cho rằng Nhu cầu quan trọng hơn Biên lợi nhuận thì chỉ số $CR$ sẽ vượt quá $10\%$ ngay. Khi đó hệ thống sẽ từ chối ra quyết định và yêu cầu đánh giá lại.  
> Đối với bài toán đầu tư nguồn vốn bán lẻ hàng chục tỷ đồng, kiểm soát được tính nhất quán logic toán học là yếu tố sống còn, và AHP là phương pháp đáp ứng tốt nhất yêu cầu này ạ."*

🔍 **[LUẬN CỨ KỸ THUẬT BỔ TRỢ]:** AHP cho phép phân rã bài toán phức tạp thành cấu trúc phân cấp (Mục tiêu $\to$ Tiêu chí $\to$ Phương án). Khác với TOPSIS cần ma trận giải pháp lý tưởng PIS và NIS có thể bị hiện tượng đảo nghịch thứ hạng (Rank Reversal) khi thêm bớt phương án, AHP kiểm soát được tính nhất quán nội tại qua Vector riêng chính và giá trị riêng cực đại $\lambda_{\max}$.

---

### ❓ CÂU HỎI 2:
**"Làm thế nào để chứng minh rằng kết quả xếp hạng và số lượng đề xuất nhập hàng không bị gán cứng (hard-code) trong mã nguồn?"**

🗣️ **[LỜI THOẠI TRẢ LỜI CỦA BẠN]:**  
> *"Dạ thưa Thầy, nhóm em chứng minh tính khách quan qua 3 luận cứ kỹ thuật cụ thể:  
> **Thứ nhất:** Toàn bộ điểm số AHP đều được tính bằng phép nhân vô hướng giữa Vector trọng số trong bảng `PAIRWISE_COMPARISONS` và Ma trận điểm chuẩn hóa trong bảng `ALTERNATIVE_SCORES`, sau đó cập nhật trực tiếp vào bảng `DECISION_RESULTS` bằng câu lệnh SQL MERGE trong transaction an toàn.  
> **Thứ hai:** Công thức tính lượng hàng đề xuất nhập không phải là một con số cố định, mà là hàm bù đắp tồn kho:  
> $$\text{Lượng nhập} = \text{Nhu cầu ngày} \times (\text{LeadTime} + 30\text{ ngày}) + \text{Tồn tối thiểu} - \text{Tồn hiện tại}$$  
> Trong đó, nhu cầu ngày được tính bằng hàm `SUM(quantity)` từ các đơn hàng có trạng thái `Completed` trong CSDL chia cho tổng số ngày bán lịch sử.  
> **Thứ ba:** Bằng chứng rõ nhất là ở Tab 4 và Tab 3 What-If, khi chúng em thêm một sản phẩm mới hoặc thay đổi tham số thị trường, toàn bộ thứ hạng, điểm số và số lượng nhập trong Bảng xếp hạng cũng như trong file Word/HTML xuất ra đều thay đổi theo ngay lập tức ạ."*

🔍 **[LUẬN CỨ KỸ THUẬT BỔ TRỢ]:** Trong file `DssEngine.cs`, các phương thức `CalculateAHP()`, `NormalizeScores()`, và `CalculateRestockQuantity()` đều truy vấn động từ `SqlConnection`, không có bất kỳ mảng gán sẵn kết quả nào. File `Form1.cs` chỉ đóng vai trò hiển thị `DataTable` trả về từ `DssEngine`.

---

### ❓ CÂU HỎI 3:
**"Nếu một nhân viên bán hàng (SalesStaff) đăng nhập vào hệ thống thì có thể bấm nút chạy lại mô hình DSS hay không?"**

🗣️ **[LỜI THOẠI TRẢ LỜI CỦA BẠN]:**  
> *"Dạ thưa Thầy, hệ thống áp dụng cơ chế kiểm soát truy cập dựa trên vai trò (RBAC) rất chặt chẽ:  
> Nhân viên bán hàng (`SalesStaff`) chỉ có quyền tra cứu kết quả xếp hạng, xem thông tin phân tích bán hàng OLAP và đọc khuyến nghị AI để hỗ trợ tư vấn khách hàng.  
> Nếu tài khoản `SalesStaff` bấm vào nút 'Chạy Mô Hình DSS' tại Tab 2, hệ thống sẽ kiểm tra thuộc tính Role qua đối tượng `UserSession.CurrentUser` và lập tức chặn lại, hiển thị hộp thoại cảnh báo: **'Không đủ quyền: Chỉ Giám đốc kinh doanh (DecisionMaker) hoặc Quản trị viên (Admin) mới được kích hoạt mô hình DSS'**. Chỉ các vai trò có thẩm quyền mới được phép kích hoạt tính toán phiên quyết định ạ."*

🔍 **[LUẬN CỨ KỸ THUẬT BỔ TRỢ]:** Lớp `UserSession` quản lý trạng thái phiên làm việc tĩnh (Singleton Pattern). Tại sự kiện `btnRunModel_Click`, mã nguồn kiểm tra `if (!UserSession.HasRole("Admin", "DecisionMaker")) { MessageBox.Show(...); return; }`.

---

### ❓ CÂU HỎI 4:
**"Vì sao đối với tiêu chí Rủi ro tồn kho (C4) và Chi phí Marketing (C5) nhóm lại dùng công thức chuẩn hóa nghịch đảo (vmin / v)?"**

🗣️ **[LỜI THOẠI TRẢ LỜI CỦA BẠN]:**  
> *"Dạ thưa Thầy, trong lý thuyết ra quyết định đa tiêu chí, các tiêu chí luôn được phân định thành 2 nhóm đối lập:  
> 1. **Tiêu chí Lợi ích (Benefit):** Giá trị thực tế càng cao thì càng tốt (như Biên lợi nhuận hay Doanh thu), vì vậy ta chuẩn hóa bằng công thức $v / v_{\max}$ để sản phẩm có giá trị cao nhất đạt điểm 1.0 tối đa.  
> 2. **Tiêu chí Chi phí (Cost):** Giá trị thực tế càng cao thì lại càng bất lợi và rủi ro cho doanh nghiệp (ví dụ số ngày tồn kho càng dài thì vốn bị chôn càng lâu, hoặc chi phí marketing càng lớn thì càng tốn kém ngân sách).  
> Do đó, bắt buộc phải dùng công thức nghịch đảo $v_{\min} / v$. Khi đó, sản phẩm nào có số ngày tồn kho ít nhất (an toàn vốn nhất) hoặc chi phí marketing thấp nhất (tiết kiệm nhất) sẽ đạt điểm 1.0 tối đa, còn sản phẩm nào tồn kho lâu và tốn kém sẽ bị điểm thấp. Nhờ chuẩn hóa về cùng một chiều 'điểm càng cao càng tốt', hàm tổng hợp AHP mới bảo đảm tính chính xác ạ."*

🔍 **[LUẬN CỨ KỸ THUẬT BỔ TRỢ]:** Nếu không đảo chiều cho tiêu chí Chi phí, sản phẩm có số ngày tồn kho 100 ngày và chi phí quảng cáo 100 triệu sẽ bị cộng điểm cao hơn sản phẩm 10 ngày và 10 triệu, dẫn đến sai lệch hoàn toàn bản chất kinh tế.

---

### ❓ CÂU HỎI 5:
**"Hệ thống xử lý như thế nào nếu trong lúc đang chạy thì bị cúp điện, rút cáp mạng hoặc dịch vụ SQL Server bị tắt đột ngột?"**

🗣️ **[LỜI THOẠI TRẢ LỜI CỦA BẠN]:**  
> *"Dạ thưa Thầy, hệ thống được thiết kế theo chuẩn kiến trúc chống lỗi (Fault-Tolerant Enterprise):  
> **Thứ nhất:** Mọi truy vấn đọc/ghi đều được bao bọc trong khối `try-catch` kiểm tra lỗi mạng chuyên biệt thông qua lớp `DatabaseGuard.IsConnectionError` (bắt chính xác các mã lỗi mất kết nối máy chủ SQL như -1, 2, 53, 10061).  
> **Thứ hai:** Ứng dụng chạy một Timer nhịp tim nền kiểm tra kết nối định kỳ 4 giây một lần mà không làm đơ giao diện UI. Khi phát hiện mất tín hiệu SQL, một panel bảo vệ toàn màn hình (`pnlDbOverlay`) lập tức kích hoạt để khóa cứng các tab chức năng, ngăn không cho người dùng đưa ra quyết định sai trên dữ liệu đứt gãy.  
> **Thứ ba:** Khi SQL Server hoạt động trở lại, người dùng chỉ cần nhấn nút 'Thử kết nối lại', hệ thống sẽ tự động gọi lệnh `SqlConnection.ClearAllPools()` để giải phóng các kết nối chết, nạp lại dữ liệu mới nhất và mở khóa bình thường mà không cần phải khởi động lại ứng dụng ạ."*

🔍 **[LUẬN CỨ KỸ THUẬT BỔ TRỢ]:** Cơ chế Connection Pooling của ADO.NET thường giữ lại kết nối hỏng (stale connection). Việc gọi `ClearAllPools()` kết hợp `ResetConnectionRetry()` đảm bảo ứng dụng kết nối lại máy chủ tức thời mà không bị treo tiến trình.

---

### ❓ CÂU HỎI 6:
**"Tại sao hệ thống lại cần cả phân hệ OLAP (Tab 5) trong khi mô hình AHP (Tab 2) đã đưa ra kết quả xếp hạng rồi?"**

🗣️ **[LỜI THOẠI TRẢ LỜI CỦA BẠN]:**  
> *"Dạ thưa Thầy, đây là mối quan hệ tương hỗ hữu cơ giữa Dữ liệu lịch sử và Quyết định tương lai trong một hệ thống DSS hoàn chỉnh:  
> Mô hình AHP ở Tab 2 hoạt động dựa trên các tham số đánh giá của chuyên gia và điểm số dự báo. Nhưng câu hỏi đặt ra là: *'Căn cứ vào đâu để chuyên gia biết sản phẩm nào bán chạy, nhóm hàng nào tồn kho nhiều, và chương trình khuyến mãi nào thực sự hiệu quả?'*.  
> Câu trả lời chính là phân hệ OLAP ở Tab 5! Thông qua các khối lập phương đa chiều OLAP ROLLUP và OLAP CUBE, nhà quản trị nắm bắt được bức tranh toàn cảnh lịch sử bán hàng theo thời gian, danh mục và chiến dịch tiếp thị. Từ sự thật khách quan đó, chuyên gia mới thiết lập ma trận so sánh cặp Saaty một cách chuẩn xác, tránh được việc phán đoán viển vông, xa rời thực tế kinh doanh ạ."*

🔍 **[LUẬN CỨ KỸ THUẬT BỔ TRỢ]:** Theo tháp kiến trúc DSS của Efraim Turban: Tầng dưới cùng là Kho dữ liệu (Data Warehouse / OLAP), tầng giữa là Mô hình phân tích (Model Base / AHP), và tầng trên cùng là Giao diện hỗ trợ ra quyết định (User Interface / Visualization). Hai phân hệ gắn liền không thể tách rời.

---

### ❓ CÂU HỎI 7:
**"Khi thêm một sản phẩm mới ở Tab 4, thuật toán chuẩn hóa lại như thế nào để không làm xáo trộn sai lệch các sản phẩm cũ?"**

🗣️ **[LỜI THOẠI TRẢ LỜI CỦA BẠN]:**  
> *"Dạ thưa Thầy, trong DssEngine, nhóm em sử dụng cơ chế **Chuẩn hóa động toàn cục (Global Dynamic Normalization)**:  
> Khi một sản phẩm mới được thêm vào, hệ thống không lấy điểm của sản phẩm đó chia cho điểm của sản phẩm cũ, mà hệ thống sẽ thực hiện truy vấn quét lại toàn bộ cực trị MAX và MIN của tất cả các sản phẩm đang có trong phiên cho từng tiêu chí.  
> Ví dụ: Nếu sản phẩm mới có biên lợi nhuận cao hơn mức cao nhất trước đó, $v_{\max}$ mới sẽ được cập nhật và toàn bộ điểm chuẩn hóa $C_1$ của các sản phẩm cũ sẽ được tính lại theo $v_{\max}$ mới này. Sau đó, hệ thống thực hiện nhân lại Vector trọng số AHP cho tất cả các sản phẩm và cập nhật lại thứ hạng 1 đến N. Nhờ vậy, ma trận chuẩn hóa luôn bảo đảm tính tương quan tỷ lệ chính xác tuyệt đối trên cùng một hệ quy chiếu ạ."*

🔍 **[LUẬN CỨ KỸ THUẬT BỔ TRỢ]:** Phương thức `NormalizeScores(sessionId)` trong `DssEngine` duyệt qua từng tiêu chí $c$ trong `CRITERIA`, lấy `SELECT MAX(raw_score), MIN(raw_score) FROM ALTERNATIVE_SCORES WHERE criteria_id = @c`, sau đó `UPDATE` lại `normalized_score` cho toàn bộ các phương án.

---

## PHẦN 7: BẢNG CHECKLIST CHUẨN BỊ TRƯỚC GIỜ BÁO CÁO

| STT | Hạng mục kiểm tra | Tiêu chuẩn bắt buộc phải đạt | Người phụ trách & Trạng thái |
| :---: | :--- | :--- | :--- |
| **01** | Dịch vụ SQL Server | Dịch vụ `SQL Server (SQLEXPRESS25)` đang ở trạng thái Running | Lê Thị Nga — **[ ĐẠT ]** |
| **02** | CSDL `DSS_SalesDB` | Có đủ 11 bảng, 9 sản phẩm, 45 bản ghi điểm số, ma trận Saaty $CR = 0.0138$ | Lê Thị Nga — **[ ĐẠT ]** |
| **03** | Phần mềm `DSS_App.exe` | Biểu tượng Desktop mở lên bình thường, đăng nhập `admin` / `Admin@123` thành công | Lê Thanh Diệu — **[ ĐẠT ]** |
| **04** | Tài khoản kiểm thử | Có sẵn tài khoản `admin` (Admin) và `giamdoc_q4` (DecisionMaker) trong USERS | Lê Thanh Diệu — **[ ĐẠT ]** |
| **05** | Tab 2 Chạy DSS | Bấm nút chạy DSS hiện đủ 9 sản phẩm, Huy chương Vàng Samsung S24 Ultra, $CR = 0.0138$ | Lê Thanh Diệu — **[ ĐẠT ]** |
| **06** | Tab 3 What-If | 3 kịch bản What-If và nút Cây phân cấp AHP mở ra hiển thị 9 sản phẩm có mũi tên | Hồ Thị Thuận — **[ ĐẠT ]** |
| **07** | Xuất Word / HTML | Nút Xuất Word ở Tab 6 mở lên có đầy đủ chữ ký 3 bên và số liệu snapshot | Hồ Thị Thuận — **[ ĐẠT ]** |
| **08** | Windows Services | Mở sẵn cửa sổ `services.msc` thu nhỏ dưới thanh Taskbar cho màn test tắt SQL | Hồ Thị Thuận — **[ ĐẠT ]** |
| **09** | Slide PowerPoint | Mở sẵn chế độ Slide Show (F5), bút laser test chuyển slide mượt mà | Trần Hồng Minh — **[ ĐẠT ]** |
| **10** | Tài liệu in ấn nộp Thầy | In sẵn 01 bản Kế hoạch kiểm thử, 01 bản Báo cáo thiết kế và Kịch bản này | Cả nhóm — **[ ĐẠT ]** |

---

<p align="center">
  <b>✦ CHÚC NHÓM 05 BẢO VỆ ĐỒ ÁN THÀNH CÔNG RỰC RỠ & ĐẠT ĐIỂM SỐ XUẤT SẮC! ✦</b>
</p>
