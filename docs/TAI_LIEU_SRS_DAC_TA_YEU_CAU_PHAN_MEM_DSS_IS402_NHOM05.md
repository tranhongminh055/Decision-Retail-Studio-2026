# DUY TÂN UNIVERSITY – INTERNATIONAL SCHOOL
## KHOA HỆ THỐNG THÔNG TIN QUẢN LÝ (MIS) • HỌC PHẦN: IS 402 A
**Học phần:** Hệ Thống Thông Tin Hỗ Trợ Ra Quyết Định (DSS)  
**Tiêu chuẩn đặc tả:** IEEE Std 830-1998 / ISO/IEC/IEEE 29148:2018  
**Ngày phát hành:** 29/10/2026 (Phiên bản chính thức 1.0)

---

# TÀI LIỆU ĐẶC TẢ YÊU CẦU PHẦN MỀM
### SOFTWARE REQUIREMENTS SPECIFICATION (SRS DOCUMENT)
**ĐỀ TÀI: HỆ THỐNG HỖ TRỢ RA QUYẾT ĐỊNH BÁN HÀNG DSS RETAIL DECISION STUDIO 2026**  
*Ứng Dụng Mô Hình Phân Tích Thứ Bậc AHP (Thomas L. Saaty) và Cơ Sở Dữ Liệu SQL Server*

* **Giảng viên hướng dẫn (Supervisor):** ThS. Nguyễn Quang Ánh
* **Nhóm sinh viên thực hiện (Nhóm 05):**
  1. **Trần Hồng Minh** *(Trưởng nhóm - MSSV: 29210247142)*
  2. **Hồ Thị Thuận** *(MSSV: 29201164457)*
  3. **Lê Thị Nga** *(MSSV: 29206361029)*
  4. **Phạm Ngọc Tú** *(MSSV: 29204347819)*
  5. **Lê Thanh Diệu** *(MSSV: 29204622434)*
* **Ứng dụng kiểm thử:** DSS Retail Decision Studio 2026 (C# .NET 10.0 WinForms Client & SQL Server 2022 Express)
* **File văn bản chính thức (.docx):** [`docs/TAI_LIEU_SRS_DAC_TA_YEU_CAU_PHAN_MEM_DSS_IS402_NHOM05.docx`](file:///g:/Decision%20System/docs/TAI_LIEU_SRS_DAC_TA_YEU_CAU_PHAN_MEM_DSS_IS402_NHOM05.docx)

---

## BẢNG LỊCH SỬ THAY ĐỔI TÀI LIỆU (DOCUMENT REVISION HISTORY)

| Phiên bản | Ngày cập nhật | Tác giả | Mô tả nội dung thay đổi chính | Trạng thái phê duyệt |
| :---: | :---: | :--- | :--- | :---: |
| **v0.1** | 12/10/2026 | Trần Hồng Minh | Khởi tạo phác thảo yêu cầu nghiệp vụ và cấu trúc khung SRS | Nhóm 05 |
| **v0.5** | 18/10/2026 | Lê Thị Nga, Phạm Ngọc Tú | Chi tiết hóa 8 nhóm yêu cầu chức năng (FR) và phi chức năng (NFR) | Trần Hồng Minh |
| **v0.9** | 25/10/2026 | Hồ Thị Thuận, Lê Thanh Diệu | Hoàn thiện ma trận truy vết yêu cầu (RTM) và chuẩn hóa IEEE 830 | Trần Hồng Minh |
| **v1.0** | 29/10/2026 | Toàn thể Nhóm 05 | Đồng bộ mã nguồn thực tế C# .NET 10, CSDL SQL Server và xuất bản file .docx | ThS. Nguyễn Quang Ánh |

> 📌 **CAM KẾT CHUẨN HÓA KỸ THUẬT (ENGINEERING STANDARD):**  
> Tài liệu Đặc tả Yêu cầu Phần mềm (SRS) này được biên soạn tuân thủ tiêu chuẩn quốc tế **IEEE Std 830-1998** và **ISO/IEC/IEEE 29148:2018**. Tài liệu là văn bản pháp lý kỹ thuật ràng buộc giữa đội ngũ phát triển (Nhóm 05) và Giảng viên hướng dẫn/Hội đồng chấm đồ án, mô tả toàn bộ chức năng, hiệu năng và các ràng buộc của hệ thống DSS.

---

## MỤC LỤC TỔNG QUAN
1. [Chương 1: Giới Thiệu (Introduction)](#chương-1-giới-thiệu-introduction)
2. [Chương 2: Mô Tả Tổng Quan Hệ Thống (Overall Description)](#chương-2-mô-tả-tổng-quan-hệ-thống-overall-description)
3. [Chương 3: Đặc Tả Yêu Cầu Chức Năng (Functional Requirements - FR)](#chương-3-đặc-tả-yêu-cầu-chức-năng-functional-requirements---fr)
4. [Chương 4: Đặc Tả Yêu Cầu Phi Chức Năng (Non-Functional Requirements - NFR)](#chương-4-đặc-tả-yêu-cầu-phi-chức-năng-non-functional-requirements---nfr)
5. [Chương 5: Yêu Cầu Giao Diện Hệ Thống (External Interface Requirements)](#chương-5-yêu-cầu-giao-diện-hệ-thống-external-interface-requirements)
6. [Chương 6: Ma Trận Truy Vết Yêu Cầu (Requirements Traceability Matrix - RTM)](#chương-6-ma-trận-truy-vết-yêu-cầu-requirements-traceability-matrix---rtm)
7. [Chương 7: Kết Luận & Xác Nhận Nghiệm Thu Đặc Tả](#chương-7-kết-luận--xác-nhận-nghiệm-thu-đặc-tả)

---

## CHƯƠNG 1: GIỚI THIỆU (INTRODUCTION)

### 1.1. Mục đích tài liệu (Purpose)
Mục đích của tài liệu Đặc tả Yêu cầu Phần mềm (*Software Requirements Specification - SRS*) này là định nghĩa tường minh, chi tiết và lượng hóa toàn bộ các yêu cầu chức năng (*Functional Requirements - FR*), yêu cầu phi chức năng (*Non-Functional Requirements - NFR*), các ràng buộc thiết kế và giao diện của hệ thống **DSS Retail Decision Studio 2026**. Tài liệu đóng vai trò là kim chỉ nam cho giai đoạn kiểm thử, đánh giá mức độ hoàn thiện và nghiệm thu đồ án môn học IS 402 A.

### 1.2. Phạm vi sản phẩm (Product Scope)
Sản phẩm là ứng dụng Desktop Enterprise Client được xây dựng trên nền tảng **C# .NET 10.0 Windows Forms**, tích hợp lõi thuật toán ra quyết định **DssEngine** và hệ quản trị cơ sở dữ liệu **Microsoft SQL Server 2022 Express**. Hệ thống giải quyết bài toán lựa chọn danh mục sản phẩm công nghệ (Smartphones, Laptops, Phụ kiện) chiến lược cần ưu tiên nhập hàng và phân bổ ngân sách tiếp thị trong Quý 4/2026 dựa trên phương pháp Phân tích Thứ bậc AHP (Thomas L. Saaty).
* **Phạm vi chức năng cốt lõi:** Hỗ trợ xác thực bảo mật SHA-256, phân quyền 4 vai trò, cơ chế Fail-Safe giám sát ngắt kết nối CSDL DatabaseGuard.
* **Xử lý dữ liệu:** Quản lý 11 bảng thực thể CSDL 3NF, xử lý ETL làm sạch dữ liệu giao dịch bán lẻ.
* **Bộ não toán học AHP:** Tính toán ma trận Saaty 1-9 cho 5 tiêu chí (C1-C5), kiểm định tỷ số nhất quán $CR \le 0.10$, chuẩn hóa Benefit/Cost, xếp hạng AHP và tự động tính số lượng đặt hàng gợi ý.
* **Mô phỏng & Báo cáo:** Mô phỏng What-If với 3 kịch bản thị trường vi mô/vĩ mô; phân tích bán lẻ đa chiều OLAP 4 thao tác; Dashboard KPI và xuất báo cáo thẩm định Word (.docx)/HTML.

### 1.3. Định nghĩa, thuật ngữ và từ viết tắt (Definitions & Acronyms)

| Thuật ngữ / Viết tắt | Tên tiếng Anh đầy đủ | Định nghĩa & Ý nghĩa nghiệp vụ trong hệ thống |
| :--- | :--- | :--- |
| **DSS** | Decision Support System | Hệ thống thông tin hỗ trợ ra quyết định quản trị chiến lược. |
| **AHP** | Analytic Hierarchy Process | Mô hình phân tích thứ bậc của Thomas L. Saaty (1980) giải quyết bài toán đa tiêu chí. |
| **CR** | Consistency Ratio | Tỷ số nhất quán của ma trận so sánh cặp Saaty; hệ thống bắt buộc $CR \le 0.10$ (10%). |
| **CI** | Consistency Index | Chỉ số nhất quán: $CI = (\lambda_{\max} - n) / (n - 1)$. |
| **RI** | Random Index | Chỉ số ngẫu nhiên thực nghiệm theo cấp ma trận $n$ (với $n = 5$ tiêu chí thì $RI = 1.12$). |
| **OLAP** | Online Analytical Processing | Kỹ thuật phân tích dữ liệu bán lẻ đa chiều (Slicing, Dicing, Roll-up, Drill-down). |
| **3NF** | Third Normal Form | Dạng chuẩn 3 của CSDL quan hệ, triệt tiêu hoàn toàn dư thừa và dị thường dữ liệu. |
| **SHA-256** | Secure Hash Algorithm 256-bit | Thuật toán mã hóa băm mật khẩu một chiều an toàn lưu trữ trong bảng `USERS`. |
| **RBAC** | Role-Based Access Control | Mô hình kiểm soát truy cập và phân quyền theo 4 vai trò người dùng (`UserSession`). |
| **Lead Time** | Supplier Lead Time | Thời gian từ lúc nhà quản lý đặt hàng đến khi hàng cập kho (tính theo ngày). |
| **What-If** | What-If Sensitivity Analysis | Phân tích độ nhạy mô phỏng sự biến thiên của điểm số AHP khi tham số thị trường thay đổi. |

### 1.4. Tài liệu tham khảo (References)
1. **Lý thuyết AHP:** Thomas L. Saaty (1980), *"The Analytic Hierarchy Process: Planning, Priority Setting, Resource Allocation"*, McGraw-Hill, New York.
2. **Chuẩn IEEE 830:** IEEE Computer Society (1998), *"IEEE Recommended Practice for Software Requirements Specifications"*, IEEE Std 830-1998.
3. **Giáo trình IS 402 A:** ThS. Nguyễn Quang Ánh (2026), *Đề cương & Bài giảng học phần IS 402 A - Hệ Thống Thông Tin Hỗ Trợ Ra Quyết Định*, Đại học Duy Tân.
4. **Tài liệu dự án:** Nhóm 05 (2026), *Tài liệu Phân tích Thiết kế CSDL DSS và Kế hoạch Kiểm thử Đồ án IS402*, Lưu hành nội bộ.

---

## CHƯƠNG 2: MÔ TẢ TỔNG QUAN HỆ THỐNG (OVERALL DESCRIPTION)

### 2.1. Bối cảnh sản phẩm & Bài toán kinh doanh Quý 4/2026
Thị trường bán lẻ thiết bị công nghệ Quý 4 luôn có tốc độ luân chuyển vốn nhanh nhưng chứa đựng rủi ro lỗi thời cực lớn. Hệ thống DSS Retail Decision Studio 2026 đóng vai trò là trung tâm cố vấn số, giúp doanh nghiệp đánh giá đồng thời **5 tiêu chí nghiệp vụ có tính chất đối kháng nhau**:
* **$C_1$: Biên lợi nhuận gộp (Profit Margin - Benefit):** Tỷ lệ phần trăm lãi gộp trên giá bán niêm yết (càng cao càng tốt).
* **$C_2$: Doanh thu kỳ vọng (Expected Revenue - Benefit):** Dự báo doanh số bán ra dựa trên dữ liệu giao dịch lịch sử (càng cao càng tốt).
* **$C_3$: Nhu cầu thị trường (Market Trend - Benefit):** Điểm xu hướng thị trường và chỉ số quan tâm của khách hàng (càng cao càng tốt).
* **$C_4$: Rủi ro tồn kho (Inventory Risk - Cost):** Số ngày bán hết lượng hàng tồn kho an toàn - *Days of Supply* (càng ngắn càng tốt).
* **$C_5$: Chi phí tiếp thị (Marketing Cost - Cost):** Chi phí quảng bá, chạy khuyến mãi và chiết khấu thương mại (càng tiết kiệm càng tốt).

### 2.2. Kiến trúc tổng thể 3 tầng (Enterprise 3-Tier Architecture)
1. **Tầng Giao diện (Presentation Layer - C# .NET 10 WinForms UI):** Cung cấp giao diện trực quan WinForms với điều hướng 6 Tab chức năng khép kín; tiếp nhận thao tác người dùng, hiển thị đồ thị và bảng dữ liệu.
2. **Tầng Logic Nghiệp vụ & Mô hình DSS (Business Logic Layer - DssEngine):** Chứa toàn bộ thuật toán tính toán ma trận Saaty, kiểm tra $CR$, chuẩn hóa Benefit/Cost, mô phỏng What-If và công thức tính số lượng đặt hàng.
3. **Tầng Truy cập Dữ liệu (Data Access Layer - SQL Server 2022 Express):** Cơ sở dữ liệu Microsoft SQL Server 2022 Express instance `.\SQLEXPRESS25` quản lý 11 bảng thực thể đạt chuẩn 3NF.

### 2.3. Phân loại người dùng & Đặc tính tác nhân (User Classes & Characteristics)

| Nhóm vai trò (Role) | Quyền hạn trong hệ thống | Trình độ & Yêu cầu kỹ năng tác nghiệp |
| :--- | :--- | :--- |
| **Admin (Quản trị viên)** | Toàn quyền quản trị tài khoản `USERS`, sao lưu phục hồi CSDL `DSS_SalesDB`, giám sát kết nối `DatabaseGuard`. | Thành thạo quản trị Windows Server và Microsoft SQL Server Management Studio (SSMS). |
| **Analyst (Chuyên viên)** | Nhập ma trận so sánh cặp Saaty 1-9, chạy mô hình AHP, điều chỉnh thanh trượt What-If, thực hiện phân tích đa chiều OLAP, thêm SKU mới Tab 4. | Hiểu sâu phương pháp AHP của Saaty, có tư duy thống kê kinh doanh và phân tích dữ liệu bán lẻ. |
| **Manager (Giám đốc)** | Xem Dashboard KPI điều hành, duyệt báo cáo tóm tắt Executive Summary, đối chiếu độ nhạy, ra quyết định phê duyệt danh mục nhập hàng Quý 4. | Có thẩm quyền ký duyệt ngân sách thu mua, duyệt kế hoạch tài chính và chiến dịch tiếp thị. |
| **Staff (Nhân viên)** | Xem tra cứu danh mục sản phẩm, theo dõi mức tồn kho và đơn giá bán lẻ. | Kỹ năng tin học văn phòng cơ bản, thao tác tra cứu trên giao diện. |

### 2.4. Môi trường vận hành (Operating Environment)
* **Hệ điều hành Client:** Microsoft Windows 10 (bản 64-bit) hoặc Windows 11.
* **Framework:** Microsoft .NET Desktop Runtime 10.0 (x64).
* **Hệ quản trị CSDL:** Microsoft SQL Server 2022 Express Edition (hỗ trợ SQL Server Authentication và Windows Authentication).
* **Cấu hình phần cứng tối thiểu:** CPU Intel Core i3 / AMD Ryzen 3 trở lên, RAM 4GB, Ổ cứng trống tối thiểu 500MB.

### 2.5. Ràng buộc thiết kế & Thực thi (Design & Implementation Constraints)
* **Ràng buộc kiểm định Saaty:** Tỷ số nhất quán của ma trận Saaty bắt buộc phải thỏa mãn điều kiện $CR \le 0.10$. Nếu vượt ngưỡng, hệ thống lập tức khóa tính toán và yêu cầu hiệu chỉnh.
* **Ràng buộc chuẩn hóa toán học:** Bảo đảm phân định rõ ràng giữa tiêu chí Lợi ích (càng lớn càng tốt) và tiêu chí Chi phí/Rủi ro (càng nhỏ càng tốt) trong khâu chuẩn hóa.
* **Ràng buộc phòng vệ Fail-Safe:** Khi xảy ra sự cố mất kết nối mạng hoặc SQL Server dừng dịch vụ, hệ thống phải kích hoạt panel `pnlDbOverlay` trong vòng < 1 giây để khóa cứng toàn bộ 6 Tab.
* **Ràng buộc bảo mật mật khẩu:** Mọi mật khẩu người dùng phải được băm một chiều bằng thuật toán SHA-256 trước khi ghi vào CSDL.

---

## CHƯƠNG 3: ĐẶC TẢ YÊU CẦU CHỨC NĂNG (FUNCTIONAL REQUIREMENTS - FR)

### 3.1. FR-01: Quản trị Xác thực Bảo mật & Cơ chế Fail-Safe
* **FR-01.1 (Khởi động an toàn):** Hệ thống phải cung cấp màn hình `SplashForm` tự động kiểm tra chuỗi kết nối SQL Server trước khi khởi chạy form chính.
* **FR-01.2 (Xác thực SHA-256):** Hệ thống phải cho phép người dùng đăng nhập bằng Username và Password; mật khẩu được băm SHA-256 đối chiếu với cột `PasswordHash` trong bảng `USERS`.
* **FR-01.3 (Quản lý phiên RBAC):** Hệ thống phải duy trì phiên làm việc `UserSession` và phân quyền tính năng trên `Form1` theo 4 vai trò (Admin, Analyst, Manager, Staff).
* **FR-01.4 (Đăng ký tài khoản):** Cung cấp `RegisterForm` cho phép người dùng đăng ký tài khoản mới với đầy đủ kiểm tra định dạng email và chống trùng tên đăng nhập.
* **FR-01.5 (Phòng vệ Fail-Safe):** Tích hợp `DatabaseGuard` chạy ngầm kiểm tra kết nối định kỳ; khi mất kết nối CSDL, lập tức che phủ và khóa cứng 6 Tab bằng panel `pnlDbOverlay`.

### 3.2. FR-02: Tiếp nhận & Làm sạch Dữ liệu CSDL 3NF (Data Management & ETL)
* **FR-02.1 (Đồng bộ CSDL 11 bảng):** Hệ thống phải ánh xạ và truy vấn trực tiếp 11 bảng thực thể CSDL `DSS_SalesDB` đạt chuẩn 3NF.
* **FR-02.2 (Làm sạch dữ liệu ETL):** Hệ thống phải thực hiện quy trình ETL tự động: khử trùng lặp đơn hàng, tính tổng doanh thu bán lẻ thực tế và số ngày tồn kho trung bình.
* **FR-02.3 (Toàn vẹn dữ liệu):** Kiểm tra ràng buộc nghiệp vụ: ngăn chặn giá trị âm ở trường giá bán, giá vốn, số lượng tồn kho và lead time.

### 3.3. FR-03: Bộ Não Toán Học AHP Saaty & Kiểm Định Nhất Quán (Tab 1 & Tab 2)
* **FR-03.1 (Ma trận Saaty 1-9):** Cho phép người dùng thiết lập ma trận so sánh cặp giữa 5 tiêu chí ($C_1 - C_5$) theo thang đo 1-9 của Thomas L. Saaty.
* **FR-03.2 (Kiểm định $CR \le 0.10$):** Tự động tính toán vector riêng, giá trị riêng cực đại $\lambda_{\max}$, chỉ số nhất quán $CI = \frac{\lambda_{\max} - n}{n - 1}$ và tỷ số nhất quán $CR = \frac{CI}{1.12}$.
* **FR-03.3 (Khóa an toàn tỷ số CR):** Nếu $CR > 0.10$, hệ thống phải đổi nhãn cảnh báo sang màu đỏ và khóa nút "Chạy DSS", buộc người dùng phải hiệu chỉnh lại tính bắc cầu.
* **FR-03.4 (Chuẩn hóa Benefit/Cost):** Tự động chuẩn hóa dữ liệu thô: Áp dụng hàm Benefit ($r_{ij} = \frac{v_{ij}}{v_j^{\max}}$) cho $C_1, C_2, C_3$ và hàm Cost ($r_{ij} = \frac{v_j^{\min}}{v_{ij}}$) cho $C_4, C_5$.
* **FR-03.5 (Tính điểm tổng hợp AHP):** Tổng hợp điểm AHP cuối cùng theo công thức: $S_i = \sum_{j=1}^5 w_j \times r_{ij}$ và hiển thị bảng kết quả chi tiết.

### 3.4. FR-04: Xếp Hạng & Khuyến Nghị Số Lượng Nhập Hàng (Tab 2)
* **FR-04.1 (Xếp hạng tự động):** Tự động sắp xếp các sản phẩm theo thứ tự điểm AHP giảm dần từ cao xuống thấp (Hạng 1 đến Hạng 9).
* **FR-04.2 (Phân loại 3 nhóm):** Phân loại phương án thành 3 nhóm trạng thái rõ rệt: **Selected** (Phê duyệt - Xanh lá), **UnderReview** (Cần xem xét - Cam), **Rejected** (Loại bỏ - Đỏ).
* **FR-04.3 (Công thức đặt hàng):** Tự động tính số lượng gợi ý đặt hàng dựa trên nhu cầu ngày, Lead Time và chu kỳ bổ sung 30 ngày:
  $$\text{Suggested\_Qty} = \max\Big(0, \, (\text{Daily\_Demand} \times (\text{Lead\_Time} + 30)) - \text{Current\_Stock}\Big)$$
* **FR-04.4 (Phân bổ ngân sách marketing):** Tự động phân bổ tỷ trọng ngân sách tiếp thị theo tỷ lệ điểm AHP của các sản phẩm thuộc nhóm *Selected*.

### 3.5. FR-05: Mô Phỏng What-If & Phân Tích Độ Nhạy Thị Trường (Tab 3)
* **FR-05.1 (3 Kịch bản định sẵn):** Cung cấp 3 kịch bản thị trường vi mô/vĩ mô định sẵn: Kịch bản 1 (Gaming +40%), Kịch bản 2 (Đứt gãy chuỗi cung ứng), Kịch bản 3 (Lạm phát thắt chặt).
* **FR-05.2 (Thanh trượt tùy biến):** Cung cấp thanh trượt TrackBar cho phép người dùng tùy ý điều chỉnh tham số Nhu cầu, Lợi nhuận, Rủi ro của từng SKU theo thời gian thực.
* **FR-05.3 (Trực quan hóa Delta Chart):** Tự động tính toán lại điểm số AHP tức thời và cập nhật Biểu đồ đường so sánh Delta và Sơ đồ cây cấu trúc thứ bậc phân cấp.

### 3.6. FR-06: Quản Lý SKU & Thêm Mới Sản Phẩm Động (Tab 4)
* **FR-06.1 (Nhập liệu SKU mới):** Cung cấp giao diện trực quan cho phép nhập mã SKU mới, tên máy, giá niêm yết, giá vốn, tồn kho, lead time và 5 điểm tiêu chí thô.
* **FR-06.2 (Tái tính toán tức thời):** Tự động lưu vào bảng `PRODUCTS` và `ALTERNATIVES` trong CSDL SQL Server và tái tính toán lại toàn bộ xếp hạng AHP mà không cần khởi động lại ứng dụng.

### 3.7. FR-07: Phân Tích Bán Lẻ Đa Chiều OLAP (Tab 5)
* **FR-07.1 (Cắt lát - Slicing):** Hỗ trợ cắt lát dữ liệu doanh thu và sản lượng theo từng Chi nhánh (Hà Nội, TP.HCM, Đà Nẵng, Cần Thơ).
* **FR-07.2 (Thái hạt lựu - Dicing):** Hỗ trợ thái hạt lựu kết hợp nhiều chiều phân tích (Chi nhánh $\times$ Ngành hàng $\times$ Thời gian).
* **FR-07.3 (Roll-up & Drill-down):** Hỗ trợ cuộn lên từ Ngày/Tháng lên Quý/Năm và khoan sâu từ Ngành hàng xuống từng mã SKU cụ thể.

### 3.8. FR-08: Dashboard Điều Hành KPI & Xuất Báo Cáo Thẩm Định (Tab 6)
* **FR-08.1 (Thẻ KPI điều hành):** Hiển thị trực quan 4 thẻ KPI: Tổng số SKU, Số lượng SKU phê duyệt (*Selected*), Doanh thu kỳ vọng và Tổng ngân sách tiếp thị.
* **FR-08.2 (Tự sinh Executive Summary):** Thuật toán tự động sinh biên bản tóm tắt Executive Summary chỉ rõ sản phẩm chiến lược số 1 và cảnh báo rủi ro xả hàng tồn kho.
* **FR-08.3 (Xuất báo cáo Word & HTML):** Hỗ trợ xuất toàn bộ hồ sơ thẩm định ra file văn bản Microsoft Word (`.docx`) chuyên nghiệp và trang HTML tương tác phục vụ in ấn.

---

## CHƯƠNG 4: ĐẶC TẢ YÊU CẦU PHI CHỨC NĂNG (NON-FUNCTIONAL REQUIREMENTS - NFR)

| Mã NFR | Hạng mục tiêu chuẩn | Chỉ số định lượng cam kết đạt chuẩn (Target Metrics) |
| :---: | :--- | :--- |
| **NFR-01** | **Hiệu năng tính toán (Performance)** | Thời gian tính toán toàn bộ ma trận AHP cho 9 SKU $< 200\text{ ms}$; thời gian cập nhật mô phỏng What-If khi kéo thanh trượt $< 100\text{ ms}$. |
| **NFR-02** | **Thời gian phản hồi CSDL (Database Response)** | Thời gian thực thi truy vấn nạp dữ liệu từ 11 bảng SQL Server khi khởi động $< 1.0\text{ s}$. |
| **NFR-03** | **An toàn bảo mật (Security)** | Mật khẩu băm 100% bằng SHA-256; dữ liệu phiên `UserSession` bị hủy hoàn toàn khi đóng ứng dụng; phân quyền chặt chẽ trên UI. |
| **NFR-04** | **Khả năng chịu lỗi (Fault-Tolerance & Fail-Safe)** | Khi ngắt dịch vụ SQL Server, panel `pnlDbOverlay` phải khóa cứng 6 Tab trong vòng $< 1\text{ s}$; tự động phục hồi kết nối không gây sập ứng dụng (Zero Crash). |
| **NFR-05** | **Khả năng sử dụng (Usability)** | Giao diện tuân thủ luồng điều hướng 6 Tab khép kín; cảnh báo vi phạm $CR$ hiển thị màu đỏ rõ ràng; thời gian người dùng mới làm quen hệ thống $< 15\text{ phút}$. |
| **NFR-06** | **Khả năng tương thích (Compatibility)** | Tương thích hoàn toàn trên hệ điều hành Windows 10 và Windows 11 (64-bit); hoạt động ổn định trên màn hình Full HD ($1920 \times 1080$). |
| **NFR-07** | **Khả năng bảo trì (Maintainability)** | Kiến trúc phân rã 3 tầng độc lập; mã nguồn C# tuân thủ chuẩn đặt tên PascalCase; lớp `DssEngine` độc lập với giao diện WinForms. |

---

## CHƯƠNG 5: YÊU CẦU GIAO DIỆN HỆ THỐNG (EXTERNAL INTERFACES)

### 5.1. Giao diện người dùng (User Interfaces)
Hệ thống cung cấp giao diện đồ họa người dùng (GUI) nhất quán trên nền tảng Windows Forms với bảng màu chủ đạo Slate Navy (`#0F172A`), Indigo (`#4F46E5`) và Emerald Green (`#059669`). Cấu trúc màn hình gồm:
* **Cụm màn hình xác thực:** Gồm `SplashForm` (khởi động kiểm tra CSDL), `LoginForm` (đăng nhập bảo mật SHA-256) và `RegisterForm` (tạo tài khoản mới).
* **Màn hình làm việc Form1:** Giao diện chính `Form1` gồm thanh công cụ phía trên hiển thị thông tin `UserSession` và 6 Tab điều hướng: Tab 1 (Đầu vào Saaty), Tab 2 (Kết quả xếp hạng AHP), Tab 3 (Mô phỏng What-If), Tab 4 (Thêm SKU động), Tab 5 (Phân tích OLAP), Tab 6 (Xuất Báo cáo thẩm định).
* **Màn hình bảo vệ Fail-Safe:** Panel `pnlDbOverlay` màu đỏ cảnh báo che phủ toàn bộ 6 Tab khi phát hiện ngắt kết nối mạng hoặc dừng dịch vụ database.

### 5.2. Giao diện phần mềm & Cơ sở dữ liệu (Software & Database Interfaces)
* **Kết nối CSDL SQL Server:** Sử dụng thư viện `System.Data.SqlClient` trên nền .NET 10.0 để kết nối chuỗi ConnectionString đến SQL Server instance `localhost\SQLEXPRESS25`, Database: `DSS_SalesDB`.
* **Xuất báo cáo văn bản:** Sử dụng thư viện OpenXML / docx để xuất kết quả bảng xếp hạng và biên bản tóm tắt điều hành ra file Microsoft Word (`.docx`).

---

## CHƯƠNG 6: MA TRẬN TRUY VẾT YÊU CẦU (REQUIREMENTS TRACEABILITY MATRIX - RTM)

| Mã Yêu Cầu | Tên Tính Năng Nghiệp Vụ | Thành phần Mã Nguồn (Code Component) | Bảng CSDL Liên Quan | Phương Pháp Kiểm Thử |
| :---: | :--- | :--- | :--- | :--- |
| **FR-01** | Xác thực SHA-256 & Fail-Safe | `LoginForm.cs`, `DatabaseGuard.cs` | `USERS` | Unit Test + Test ngắt SQL Server |
| **FR-02** | ETL Làm Sạch Dữ Liệu 3NF | `DssEngine.cs`, `04_data_processing.sql` | `ORDERS`, `PRODUCTS`,... | Database Integrity Test |
| **FR-03** | Ma trận Saaty & $CR \le 0.10$ | `Form1.cs` (Tab 1), `DssEngine.cs` | `PAIRWISE_COMPARISONS` | Toán học AHP Test Case |
| **FR-04** | Xếp hạng & Gợi ý số lượng đặt | `Form1.cs` (Tab 2), `DssEngine.cs` | `DECISION_RESULTS` | Black-box Acceptance Test |
| **FR-05** | Mô phỏng What-If & Delta Chart | `Form1.cs` (Tab 3), `DssEngine.cs` | `ALTERNATIVES` | UI Simulation Test |
| **FR-06** | Thêm SKU mới động | `Form1.cs` (Tab 4), `DssEngine.cs` | `PRODUCTS`, `ALTERNATIVES` | CRUD Dynamic Re-calc Test |
| **FR-07** | Phân tích bán lẻ OLAP đa chiều | `Form1.cs` (Tab 5), SQL Queries | `ORDERS`, `CATEGORIES` | OLAP Query Benchmark Test |
| **FR-08** | Dashboard KPI & Xuất Word | `Form1.cs` (Tab 6), docx export | `DECISION_SESSIONS` | Report Generation Test |
| **NFR-04** | Cơ chế khóa cứng Fail-Safe | `pnlDbOverlay`, `DatabaseGuard.cs` | Tất cả 11 bảng | Physical Network Disconnect Test |

---

## CHƯƠNG 7: KẾT LUẬN & XÁC NHẬN NGHIỆM THU ĐẶC TẢ

Tài liệu Đặc tả Yêu cầu Phần mềm (SRS) phiên bản 1.0 đã định nghĩa toàn diện, chính xác và đầy đủ tất cả các yêu cầu chức năng và phi chức năng của hệ thống DSS Retail Decision Studio 2026. Đây là căn cứ kỹ thuật chuẩn mực để Hội đồng chấm thi và Giảng viên hướng dẫn đánh giá mức độ hoàn thiện của sản phẩm công nghệ phần mềm trong kỳ bảo vệ đồ án kết thúc học phần IS 402 A.

---

### PHẦN XÁC NHẬN KÝ DUYỆT ĐẶC TẢ

| ĐẠI DIỆN NHÓM SINH VIÊN THỰC HIỆN<br>*(Trưởng nhóm Nhóm 05)* | GIẢNG VIÊN HƯỚNG DẪN PHÊ DUYỆT<br>*(Khoa Hệ Thống Thông Tin Quản Lý)* |
| :---: | :---: |
| *(Đã ký số nội bộ)*<br><br><br>**Trần Hồng Minh** | *(Ký và ghi rõ họ tên)*<br><br><br>**ThS. Nguyễn Quang Ánh** |
