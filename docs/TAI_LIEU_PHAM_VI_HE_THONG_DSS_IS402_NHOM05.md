# DUY TÂN UNIVERSITY – INTERNATIONAL SCHOOL
## KHOA HỆ THỐNG THÔNG TIN QUẢN LÝ (MIS) • HỌC PHẦN: IS 402 A
**Học phần:** Hệ Thống Thông Tin Hỗ Trợ Ra Quyết Định (DSS)  
**Ngày phát hành:** 29/10/2026 (Phiên bản chính thức 1.0)

---

# TÀI LIỆU ĐẶC TẢ PHẠM VI HỆ THỐNG (SYSTEM SCOPE SPECIFICATION)
### PHÂN ĐỊNH RANH GIỚI TRONG PHẠM VI (IN-SCOPE) & NGOÀI PHẠM VI (OUT-OF-SCOPE)
**Đề tài:** Hệ Thống Hỗ Trợ Ra Quyết Định Bán Hàng DSS Retail Decision Studio 2026  
*Ứng Dụng Mô Hình AHP Saaty (Analytic Hierarchy Process) và Cơ Sở Dữ Liệu SQL Server*

* **Giảng viên hướng dẫn (Supervisor):** ThS. Nguyễn Quang Ánh
* **Nhóm sinh viên thực hiện (Nhóm 05):**
  1. **Trần Hồng Minh** *(Trưởng nhóm - MSSV: 29210247142)*
  2. **Hồ Thị Thuận** *(MSSV: 29201164457)*
  3. **Lê Thị Nga** *(MSSV: 29206361029)*
  4. **Phạm Ngọc Tú** *(MSSV: 29204347819)*
  5. **Lê Thanh Diệu** *(MSSV: 29204622434)*
* **Ứng dụng kiểm thử:** DSS Retail Decision Studio 2026 (C# .NET 10.0 WinForms Client & SQL Server 2022 Express)
* **File văn bản chính thức (.docx):** [`docs/TAI_LIEU_PHAM_VI_HE_THONG_DSS_IS402_NHOM05.docx`](file:///g:/Decision%20System/docs/TAI_LIEU_PHAM_VI_HE_THONG_DSS_IS402_NHOM05.docx)

---

## BẢNG THEO DÕI LỊCH SỬ PHIÊN BẢN (DOCUMENT REVISION HISTORY)

| Phiên bản | Ngày cập nhật | Tác giả / Nhóm | Mô tả nội dung thay đổi chính | Trạng thái phê duyệt |
| :---: | :---: | :--- | :--- | :---: |
| **v0.1** | 15/10/2026 | Trần Hồng Minh | Khởi tạo tài liệu phác thảo phạm vi sơ bộ | Nhóm 05 |
| **v0.5** | 22/10/2026 | Lê Thị Nga, Phạm Ngọc Tú | Bổ sung ranh giới CSDL SQL Server và thuật toán AHP 5 tiêu chí | Trần Hồng Minh |
| **v0.8** | 26/10/2026 | Hồ Thị Thuận, Lê Thanh Diệu | Đặc tả các thành phần Out-of-Scope (POS, WMS, Logistics, ERP) | Trần Hồng Minh |
| **v1.0** | 29/10/2026 | Toàn thể Nhóm 05 | Hoàn thiện đặc tả In-Scope/Out-of-Scope, tích hợp Sơ đồ ranh giới 300 DPI và xuất bản file .docx | ThS. Nguyễn Quang Ánh |

> 📌 **TÓM TẮT ĐIỀU HÀNH QUẢN LÝ PHẠM VI (EXECUTIVE SCOPE SUMMARY):**  
> Tài liệu này xác định ranh giới chức năng và công nghệ bất biến của dự án *DSS Retail Decision Studio 2026*. Mọi yêu cầu phát sinh nằm ngoài các danh mục In-Scope được định nghĩa tại đây sẽ được phân loại là Out-of-Scope và chỉ được xem xét trong các giai đoạn phát triển tiếp theo nhằm ngăn ngừa rủi ro trượt tiến độ (*Scope Creep*).

---

## 1. TỔNG QUAN BÀI TOÁN & MỤC ĐÍCH PHÂN ĐỊNH PHẠM VI

### 1.1. Bối cảnh bài toán kinh doanh bán lẻ Quý 4/2026
Trong ngành bán lẻ thiết bị công nghệ số (Smartphones, Laptops, Phụ kiện), Quý 4 hàng năm luôn là "mùa vàng" bùng nổ doanh số với các đợt cao điểm mua sắm Black Friday, Giáng Sinh và cận Tết Nguyên Đán. Tuy nhiên, đây cũng chính là giai đoạn tiềm ẩn rủi ro đọng vốn và tồn kho cao nhất của năm tài chính.

Nhà quản trị bán lẻ phải đối mặt với bài toán tối ưu hóa danh mục sản phẩm (*SKU Portfolio Optimization*) dưới áp lực của **5 tiêu chí nghiệp vụ mang tính chất xung đột trực diện**:
1. **Biên lợi nhuận gộp ($C_1$ - Profit Margin - Benefit):** Mong muốn đẩy mạnh các dòng sản phẩm cao cấp có tỷ suất sinh lời lớn.
2. **Doanh thu kỳ vọng ($C_2$ - Expected Revenue - Benefit):** Kỳ vọng dòng tiền lớn và thị phần cao thông qua các mặt hàng bán chạy.
3. **Nhu cầu thị trường ($C_3$ - Market Trend - Benefit):** Đón đầu thị hiếu công nghệ mới và các sản phẩm đang "hot trend" của người tiêu dùng.
4. **Rủi ro tồn kho ($C_4$ - Inventory Risk - Cost):** Đòi hỏi số ngày tồn kho (*Days of Supply*) phải thật ngắn để quay vòng vốn nhanh, hạn chế nguy cơ lỗi thời mất giá.
5. **Chi phí tiếp thị ($C_5$ - Marketing Cost - Cost):** Ngân sách quảng cáo, chiết khấu và khuyến mãi phải được kiểm soát tối ưu, không vượt trần tài chính.

Nếu dựa vào thói quen hoặc trực giác cảm tính, doanh nghiệp rất dễ rơi vào tình trạng đọng vốn hàng chục tỷ đồng ở các mặt hàng ế ẩm hoặc ngược lại bị "cháy hàng" (*Out-of-Stock*) ở các mặt hàng chủ lực, đánh mất khách hàng vào tay đối thủ. Hệ thống **DSS Retail Decision Studio 2026** ra đời nhằm giải quyết triệt để bài toán này thông qua việc lượng hóa 100% quá trình đánh giá bằng mô hình toán học đa tiêu chí AHP kết hợp phân tích đa chiều OLAP.

### 1.2. Mục đích thiết lập ranh giới phạm vi hệ thống
Một trong những nguyên nhân hàng đầu dẫn đến thất bại của các dự án phần mềm là sự nhầm lẫn giữa hệ thống giao dịch tác nghiệp (*Transaction Processing System - TPS/POS/ERP*) và hệ thống hỗ trợ ra quyết định (*Decision Support System - DSS*). Việc phân định ranh giới In-Scope và Out-of-Scope nhằm đạt được các mục tiêu cốt lõi:
* **Tập trung năng lực cốt lõi:** Tập trung tuyệt đối vào mô hình hóa bài toán quyết định, khử bỏ cảm tính cá nhân và đưa ra khuyến nghị khoa học.
* **Phòng chống trượt phạm vi (Scope Creep):** Không ôm đồm các chức năng máy tính tiền tại quầy, quản lý xuất nhập kho vật lý, hạch toán kế toán hoặc điều phối vận tải logistics.
* **Minh bạch ranh giới tích hợp:** Xác định rõ ràng cấu trúc dữ liệu đầu vào (*Data In*) mà DSS cần đọc từ cơ sở dữ liệu và các định dạng đầu ra (*Decisions Out*) phục vụ cuộc họp giao ban quản trị.
* **Chuẩn hóa tiêu chí nghiệm thu:** Cung cấp căn cứ định lượng để Hội đồng đánh giá đồ án, Giảng viên hướng dẫn và các bên liên quan kiểm thử và nghiệm thu chức năng phần mềm.

---

## 2. SƠ ĐỒ TRỰC QUAN HÓA RANH GIỚI PHẠM VI (SCOPE BOUNDARY ARCHITECTURE)

Sơ đồ dưới đây mô tả trực quan và toàn diện ranh giới giữa **Phạm vi Trong hệ thống (In-Scope - Khung màu xanh)** và **Phạm vi Ngoài hệ thống (Out-of-Scope - Các khung nét đứt màu đỏ)**, cùng sự tương tác với Nhà quản trị (**Decision Maker - Khung màu vàng**) thông qua các luồng dữ liệu hai chiều:

![Sơ đồ Ranh giới Phạm vi Hệ thống DSS](file:///g:/Decision%20System/docs/images/sodo_pham_vi_inscope_outscope.png)

### 2.1. Mã Mermaid kiến trúc phân định phạm vi:
```mermaid
flowchart TD
    %% OUT-OF-SCOPE EXTERNAL SYSTEMS
    subgraph OUT_POS ["⛔ NGOÀI PHẠM VI: POS Bán Lẻ"]
        POS1["Máy tính tiền tại quầy"]
        POS2["Quẹt thẻ ngân hàng & Thu tiền mặt"]
        POS3["In hóa đơn giấy (Receipt)"]
    end

    subgraph OUT_WMS ["⛔ NGOÀI PHẠM VI: Kho Vật Lý (WMS)"]
        WMS1["Trừ tồn kho vật lý tự động"]
        WMS2["In tem nhãn RFID / Mã vạch kệ"]
        WMS3["Điều phối xe nâng Pallet"]
    end

    subgraph OUT_LOG ["⛔ NGOÀI PHẠM VI: Logistics & Đội Xe"]
        LOG1["Điều phối xe tải giao hàng"]
        LOG2["Định vị GPS hành trình"]
        LOG3["Biên bản ký nhận chặng cuối (POD)"]
    end

    subgraph OUT_WEB ["⛔ NGOÀI PHẠM VI: Sàn TMĐT & Web"]
        WEB1["Giỏ hàng & Đặt hàng online"]
        WEB2["Cổng thanh toán trực tuyến"]
        WEB3["Tự động đổi giá niêm yết web"]
    end

    subgraph OUT_ERP ["⛔ NGOÀI PHẠM VI: Kế Toán & ERP"]
        ERP1["Hạch toán sổ cái Nợ / Có"]
        ERP2["Xuất hóa đơn điện tử VAT"]
        ERP3["Tính lương & Quản lý công nợ"]
    end

    %% IN-SCOPE DSS SYSTEM
    subgraph IN_SCOPE ["✅ TRONG PHẠM VI (IN-SCOPE): HỆ THỐNG DSS RETAIL DECISION STUDIO 2026"]
        direction TB
        
        subgraph MOD_SEC ["1. Phân hệ Bảo mật & Giám sát (Security & Fail-Safe)"]
            SEC1["SplashForm: Khởi tạo kết nối SQL Server"]
            SEC2["LoginForm: Xác thực mật khẩu băm SHA-256 (USERS)"]
            SEC3["UserSession: Quản lý phiên 4 vai trò"]
            SEC4["DatabaseGuard & pnlDbOverlay: Khóa cứng 6 Tab bảo vệ khi mất mạng"]
            SEC5["RegisterForm: Đăng ký tài khoản mới"]
        end

        subgraph MOD_DB ["2. Phân hệ CSDL Chuẩn Hóa 3NF (SQL Server)"]
            DB1["CSDL DSS_SalesDB: 11 bảng thực thể chuẩn 3NF"]
            DB2["6 Bảng nghiệp vụ: CATEGORIES, PRODUCTS, CUSTOMERS, ORDERS,..."]
            DB3["5 Bảng DSS: SESSIONS, CRITERIA, COMPARISONS, SCORES, RESULTS"]
            DB4["Xử lý ETL & Làm sạch dữ liệu giao dịch"]
        end

        subgraph MOD_AHP ["3. Bộ Não Toán Học AHP Saaty & Xếp Hạng (Tab 1 & 2)"]
            AHP1["Ma trận so sánh cặp Saaty 1-9 cho 5 tiêu chí (C1 - C5)"]
            AHP2["Kiểm định độ nhất quán CR <= 0.10"]
            AHP3["Chuẩn hóa Benefit (v/vmax) và Cost (vmin/v)"]
            AHP4["Tổng hợp vector trọng số & Xếp hạng phương án"]
            AHP5["Phân loại: Selected / UnderReview / Rejected"]
            AHP6["Khuyến nghị số lượng đặt hàng tối ưu (Lead Time, chu kỳ 30 ngày)"]
        end

        subgraph MOD_SIM ["4. Mô Phỏng What-If & Mở Rộng SKU (Tab 3 & 4)"]
            SIM1["3 Kịch bản định sẵn: Gaming +40%, Đứt gãy, Lạm phát"]
            SIM2["Thanh trượt TrackBar tùy biến tham số theo thời gian thực"]
            SIM3["Trực quan hóa Đồ thị Delta & Cây phân cấp AHP"]
            SIM4["Thêm SKU mới động (Tab 4) & Tái tính toán tức thời"]
        end

        subgraph MOD_OLAP ["5. Phân Tích Đa Chiều OLAP (Tab 5)"]
            OLAP1["Cắt lát (Slicing) theo Chi nhánh"]
            OLAP2["Thái hạt lựu (Dicing): Chi nhánh x Ngành hàng x Thời gian"]
            OLAP3["Cuộn lên (Roll-up): Ngày/Tháng lên Quý/Năm"]
            OLAP4["Khoan sâu (Drill-down): Từ Ngành hàng xuống SKU"]
        end

        subgraph MOD_REP ["6. Dashboard Điều Hành & Báo Cáo (Tab 6)"]
            REP1["4 Thẻ chỉ số điều hành KPI Cards"]
            REP2["Tự động sinh Tóm tắt điều hành (Executive Summary)"]
            REP3["Xuất hồ sơ thẩm định chuyên nghiệp ra Word (.docx) & HTML"]
        end
    end

    %% HUMAN DECISION MAKER
    subgraph HUMAN ["👤 NHÀ QUẢN TRỊ (DECISION MAKER)"]
        DEC["Chủ thể ra quyết định cuối cùng<br/>Phê duyệt ngân sách & Ký duyệt nhập hàng"]
    end

    %% DATA FLOWS
    OUT_POS -. "Đọc lịch sử hóa đơn (Read-Only)" .-> DB4
    OUT_WMS -. "Đọc số lượng tồn kho ban đầu" .-> DB4
    REP3 ==> "Hồ sơ tham mưu định lượng" ==> DEC
    DEC -. "Lập phiếu nhập hàng chính thức thủ công" .-> OUT_WMS
    DEC -. "Phân bổ ngân sách chiến dịch" .-> OUT_WEB

    %% STYLING
    classDef inScope fill:#eff6ff,stroke:#2563eb,stroke-width:2px,color:#1e3a8a;
    classDef outScope fill:#fff1f2,stroke:#e11d48,stroke-width:2px,stroke-dasharray: 5 5,color:#881337;
    classDef humanNode fill:#fefce8,stroke:#ca8a04,stroke-width:2px,color:#713f12;

    class IN_SCOPE,MOD_SEC,MOD_DB,MOD_AHP,MOD_SIM,MOD_OLAP,MOD_REP inScope;
    class OUT_POS,OUT_WMS,OUT_LOG,OUT_WEB,OUT_ERP outScope;
    class HUMAN,DEC humanNode;
```

---

## 3. ĐẶC TẢ CHI TIẾT PHẠM VI BÊN TRONG HỆ THỐNG (IN-SCOPE)

Phần mềm **DSS Retail Decision Studio 2026** trực tiếp đảm nhận, xây dựng và cam kết bảo hành vận hành 7 phân hệ nghiệp vụ sau:

### 3.1. Phân hệ Quản trị Người dùng, Bảo mật & Cơ chế Phòng vệ Fail-Safe
* **SplashForm khởi động an toàn:** Tiến hành kiểm tra chuỗi kết nối SQL Server (`DatabaseGuard`). Nếu không kết nối được database, lập tức dừng ứng dụng kèm thông báo lỗi chi tiết, không cho phép truy cập sâu.
* **Xác thực bảo mật băm SHA-256 (LoginForm):** Ứng dụng mật mã học một chiều chuẩn công nghiệp SHA-256 đối chiếu với trường `PasswordHash` trong bảng `USERS`. Tuyệt đối không lưu mật khẩu dạng bản rõ (*plaintext*).
* **Quản lý phiên làm việc toàn cục (UserSession):** Lưu trữ thông tin tài khoản và kiểm soát phân quyền truy cập chức năng theo 4 nhóm quyền:
  * `Admin`: Toàn quyền hệ thống, sao lưu phục hồi dữ liệu, quản lý danh sách tài khoản.
  * `DecisionMaker`: Phê duyệt phương án chiến lược, xem Dashboard điều hành, ký duyệt đề xuất nhập hàng.
  * `SalesManager`: Khai thác toàn diện ma trận Saaty, chạy mô hình AHP, thực hiện mô phỏng What-If và phân tích đa chiều OLAP.
  * `SalesStaff`: Tra cứu danh mục sản phẩm và xem báo cáo tổng hợp.
* **Màn hình Đăng ký tài khoản (RegisterForm):** Cho phép người dùng mới tạo tài khoản, tự động băm SHA-256 và lưu trữ vào CSDL.
* **Cơ chế phòng vệ Fail-Safe (DatabaseGuard & pnlDbOverlay):** Giám sát liên tục trạng thái kết nối SQL Server. Khi mạng bị ngắt hoặc dịch vụ SQL Server dừng đột ngột, panel `pnlDbOverlay` ngay lập tức che phủ và khóa cứng toàn bộ 6 Tab chức năng của `Form1`, bảo vệ dữ liệu chống thao tác sai trên dữ liệu đứt gãy; tự động mở khóa khi kết nối phục hồi.

### 3.2. Phân hệ Cơ sở Dữ liệu Quan hệ Chuẩn Hóa 3NF (SQL Server)
* **Cơ sở dữ liệu trung tâm DSS_SalesDB:** Được thiết kế chuẩn hóa đạt chuẩn Dạng chuẩn 3 (3NF), gồm **11 bảng thực thể** liên kết chặt chẽ qua khóa chính và khóa ngoại:
  * *Phân hệ Bán lẻ:* `CATEGORIES`, `PRODUCTS`, `CUSTOMERS`, `PROMOTIONS`, `ORDERS`, `ORDER_DETAILS`.
  * *Phân hệ Hỗ trợ ra quyết định:* `DECISION_SESSIONS`, `CRITERIA`, `PAIRWISE_COMPARISONS`, `ALTERNATIVES`, `ALTERNATIVE_SCORES`, `DECISION_RESULTS`.
* **Quy trình ETL & Làm sạch dữ liệu:** Truy vấn lịch sử bán hàng, tính toán doanh thu thực tế, biên lợi nhuận trung bình và số ngày bán hết hàng tồn để điền vào ma trận phương án đầu vào của DSS.

### 3.3. Phân hệ Bộ Não Toán Học AHP Saaty & Xếp Hạng Phương Án (Tab 1 & Tab 2)
* **Ma trận so sánh cặp Saaty 1-9 (Tab 1):** Cho phép chuyên gia/nhà quản lý nhập tỷ lệ ưu tiên giữa 5 tiêu chí:
  * $C_1$: Biên lợi nhuận gộp (*Profit Margin* – Benefit)
  * $C_2$: Doanh thu kỳ vọng (*Expected Revenue* – Benefit)
  * $C_3$: Nhu cầu & Xu hướng thị trường (*Market Demand Trend* – Benefit)
  * $C_4$: Rủi ro tồn kho & Số ngày tồn kho (*Inventory Risk & Days of Supply* – Cost)
  * $C_5$: Chi phí tiếp thị & Khuyến mãi (*Marketing & Promotional Cost* – Cost)
* **Kiểm định tỷ số nhất quán ($CR \le 0.10$):** Tự động tính vector trọng số $w$, giá trị riêng cực đại $\lambda_{\max}$, chỉ số $CI = \frac{\lambda_{\max} - n}{n - 1}$ và tỷ số nhất quán $CR = \frac{CI}{RI}$ (với $n=5, RI=1.12$). Cảnh báo màu đỏ và khóa nút bấm nếu $CR > 0.10$, bắt buộc điều chỉnh lại tính bắc cầu.
* **Chuẩn hóa dữ liệu theo hàm Benefit / Cost:**
  * Tiêu chí Lợi ích: $r_{ij} = \frac{v_{ij}}{v_j^{\max}}$
  * Tiêu chí Chi phí/Rủi ro: $r_{ij} = \frac{v_j^{\min}}{v_{ij}}$
* **Tính điểm tổng hợp & Phân loại phương án (Tab 2):**
  * Tính điểm AHP: $S_i = \sum_{j=1}^5 w_j \times r_{ij}$.
  * Tự động sắp xếp giảm dần và gán nhãn trạng thái:
    * **Selected** (Ưu tiên phê duyệt cao nhất – Xanh lá)
    * **UnderReview** (Cần xem xét thêm – Vàng cam)
    * **Rejected** (Loại bỏ / Ngừng nhập hàng – Đỏ)
* **Thuật toán khuyến nghị số lượng nhập hàng:** Tự động tính toán số lượng gợi ý đặt hàng dựa trên nhu cầu ngày, Lead Time và chu kỳ 30 ngày:
  $$\text{Suggested\_Order\_Qty} = \max\Big(0, \, (\text{Daily\_Demand} \times (\text{Lead\_Time} + 30)) - \text{Current\_Stock}\Big)$$

### 3.4. Phân hệ Mô Phỏng What-If & Phân Tích Độ Nhạy Thị Trường (Tab 3)
* **3 Kịch bản thị trường định sẵn:**
  * *Kịch bản 1 (Bùng nổ Gaming +40%):* Nhu cầu dòng máy Gaming tăng vọt, kiểm tra độ nhạy của điểm số AHP.
  * *Kịch bản 2 (Đứt gãy chuỗi cung ứng):* Tăng thời gian chờ hàng (Lead Time) và rủi ro vốn tồn kho.
  * *Kịch bản 3 (Lạm phát thắt chặt chi tiêu):* Ưu tiên các dòng máy có biên lợi nhuận cao và chi phí khuyến mãi thấp.
* **Thanh trượt TrackBar tương tác trực tiếp:** Cho phép điều chỉnh giá trị Nhu cầu, Biên lợi nhuận, Rủi ro vốn theo thời gian thực và tự động tái tính toán điểm số tức thời.
* **Trực quan hóa đồ thị:** Biểu đồ Delta so sánh điểm số trước và sau mô phỏng; sơ đồ cây phân cấp thứ bậc Saaty AHP.

### 3.5. Phân hệ Quản Lý Danh Mục & Thêm SKU Mới Động (Tab 4)
* Cho phép nhập liệu trực tiếp các thông số kỹ thuật của một SKU mới dự kiến đưa vào kinh doanh (Tên SKU, Giá niêm yết, Giá vốn, Tồn kho ban đầu, Lead Time, Điểm nhu cầu, Chi phí tiếp thị).
* Tự động lưu vào bảng `PRODUCTS` và kích hoạt tái tính toán lại toàn bộ ma trận AHP của tất cả phương án, cập nhật lại bảng xếp hạng tức thì mà không cần khởi động lại ứng dụng.

### 3.6. Phân hệ Phân Tích Dữ Liệu Bán Hàng Đa Chiều OLAP (Tab 5)
* Cung cấp 4 kỹ thuật phân tích OLAP bán lẻ chuyên sâu:
  * **Cắt lát (Slicing):** Lọc theo từng Chi nhánh (Hà Nội, TP.HCM, Đà Nẵng, Cần Thơ).
  * **Thái hạt lựu (Dicing):** Giao cắt đa chiều (Chi nhánh $\times$ Ngành hàng $\times$ Thời gian).
  * **Cuộn lên (Roll-up):** Tổng hợp số liệu từ cấp Ngày/Tháng lên cấp Quý và Năm.
  * **Khoan sâu (Drill-down):** Chi tiết hóa từ Doanh số Danh mục chung xuống chi tiết từng mã SKU.
* Trình bày dạng ma trận dữ liệu trực quan (OLAP Data Grid) và đồ thị phân bổ tỷ trọng.

### 3.7. Phân hệ Dashboard Chỉ Số Điều Hành & Xuất Bản Báo Cáo Thẩm Định (Tab 6)
* **Thẻ KPI điều hành:** Hiển thị trực quan Tổng số SKU ứng viên, Số lượng SKU phê duyệt (*Selected*), Doanh thu kỳ vọng toàn danh mục và Tổng ngân sách tiếp thị đề xuất.
* **Tự động sinh Tóm tắt Điều hành (Executive Summary):** Thuật toán tự động sinh văn bản nhận định tình hình kinh doanh, chỉ rõ sản phẩm số 1 cần ưu tiên nhập hàng và danh sách SKU rủi ro cần xả hàng tồn.
* **Xuất báo cáo đa định dạng:** Hỗ trợ xuất hồ sơ thẩm định ra file văn bản Microsoft Word (`.docx`) và trang HTML tương tác, định dạng thẩm mỹ cao phục vụ in ấn báo cáo trước Hội đồng.

---

## 4. ĐẶC TẢ CHI TIẾT PHẠM VI NẰM NGOÀI HỆ THỐNG (OUT-OF-SCOPE)

Để bảo đảm tính khả thi của đồ án và không làm loãng mục tiêu nghiên cứu của học phần DSS, các hạng mục sau đây được tuyên bố dứt khoát là **NẰM NGOÀI PHẠM VI CỦA HỆ THỐNG**:

### 4.1. Không đóng vai trò Máy tính tiền tại Điểm bán (Point of Sale - POS)
* **Không quét mã barcode bán lẻ tại quầy:** Hệ thống không kết nối máy quét mã vạch để đọc từng sản phẩm khi khách thanh toán.
* **Không xử lý thanh toán tài chính:** Hệ thống không tích hợp cổng quẹt thẻ ngân hàng, không quét mã VietQR/MoMo, không quản lý két tiền mặt thu ngân.
* **Không in hóa đơn giấy (Receipt):** Không kết nối máy in nhiệt POS để in biên lai trao tay cho khách hàng.
* *Ranh giới kết nối:* Hệ thống DSS chỉ nhận dữ liệu lịch sử hóa đơn bán lẻ đã hoàn tất từ bảng `ORDERS` trong SQL Server do hệ thống POS đồng bộ về.

### 4.2. Không đóng vai trò Hệ thống Quản lý Kho vật lý (Warehouse Management - WMS)
* **Không tự động trừ tồn kho vật lý:** Việc DSS đề xuất nhập bao nhiêu sản phẩm không làm thay đổi trực tiếp số lượng hàng thực tế trên kệ kho.
* **Không in tem nhãn RFID / Mã vạch giá kệ:** Không quản lý bản đồ tọa độ vị trí ô kệ trong kho hàng.
* **Không điều phối xe nâng / Robot lấy hàng:** Không quản lý tác nghiệp bốc dỡ hàng hóa vật lý.
* *Ranh giới kết nối:* DSS chỉ sử dụng con số tồn kho hiện tại (`stock_quantity`) làm dữ liệu đầu vào để tính toán rủi ro vốn; lệnh nhập hàng vật lý sẽ do thủ kho thao tác trên hệ thống WMS riêng biệt.

### 4.3. Không quản lý Vận tải & Đội xe Giao nhận (Fleet Management)
* **Không điều phối xe tải giao hàng:** Không chia tuyến đường giao hàng và không phân bổ đơn hàng lên phương tiện vận tải.
* **Không định vị GPS phương tiện:** Không theo dõi vị trí xe tải theo thời gian thực trên bản đồ vệ tinh.
* **Không ký nhận bàn giao hàng (Proof of Delivery - POD):** Không thu thập chữ ký điện tử của người nhận hàng.
* *Ranh giới kết nối:* DSS chỉ xem Lead Time (thời gian giao hàng của nhà sản xuất) là một tham số toán học cố định để tính chu kỳ an toàn.

### 4.4. Không can thiệp tự động vào Sàn Thương mại Điện tử & Website Bán hàng
* **Không vận hành giỏ hàng trực tuyến:** Không cung cấp giao diện Web/App cho người tiêu dùng đặt mua online.
* **Không tự động đổi giá bán lẻ:** DSS không tự ý thay đổi giá niêm yết trên các sàn Shopee, Lazada hay website nội bộ.
* **Không trực tiếp chạy quảng cáo tiếp thị:** Không tự động kích hoạt chiến dịch quảng cáo Facebook Ads hay Google Ads.
* *Ranh giới kết nối:* DSS chỉ đưa ra khuyến nghị phân bổ ngân sách tiếp thị định lượng giữa các sản phẩm để phòng Marketing tham khảo.

### 4.5. Không hạch toán Kế toán Tài chính Pháp lý Doanh nghiệp
* **Không ghi nhận bút toán sổ cái:** Không định khoản Nợ/Có theo chế độ kế toán doanh nghiệp.
* **Không phát hành hóa đơn điện tử VAT:** Không kết nối truyền nhận dữ liệu hóa đơn điện tử với Cơ quan Thuế.
* **Không quản lý công nợ & tính lương nhân sự:** Không tính hoa hồng bán hàng hay quản lý nợ phải thu/phải trả.
* *Ranh giới kết nối:* Các chỉ số tài chính trên DSS thuần túy phục vụ tham mưu quản trị kinh doanh, không có giá trị quyết toán thuế pháp lý.

### 4.6. Nguyên lý Tác nhân Con người Giám sát Tối cao (Human-in-the-Loop)
* **Không tự động ra quyết định thay con người:** Phần mềm DSS không tự động đặt hàng với nhà cung ứng và không tự động trừ tiền trong tài khoản công ty.
* **Vai trò tham mưu khoa học:** Mọi báo cáo, điểm số AHP và khuyến nghị số lượng nhập hàng từ DSS đóng vai trò công cụ cố vấn định lượng; quyền quyết định rót vốn cuối cùng hoàn toàn thuộc về Nhà quản lý/Giám đốc (Decision Maker).

---

## 5. BẢNG MA TRẬN PHÂN ĐỊNH RANH GIỚI & CƠ CHẾ GIAO TIẾP

| STT | Nghiệp vụ / Phân hệ | Trách nhiệm IN-SCOPE (DSS thực hiện 100%) | Phạm vi OUT-OF-SCOPE (Hệ thống ngoài) | Ranh giới Giao tiếp & Cơ chế |
| :---: | :--- | :--- | :--- | :--- |
| **1** | **Xác thực & Bảo mật** | Mã hóa băm SHA-256; quản lý phiên UserSession 4 vai trò; panel Fail-Safe `pnlDbOverlay`. | Không quản lý tài khoản nhân viên ERP tổng; không tích hợp Single Sign-On LDAP công ty. | Xác thực cục bộ trên bảng `USERS` của CSDL `DSS_SalesDB`. |
| **2** | **Dữ liệu bán hàng** | Đọc dữ liệu từ bảng `ORDERS`, `ORDER_DETAILS`; làm sạch và kiểm tra toàn vẹn dữ liệu. | Không thu ngân tại quầy; không quét mã vạch sản phẩm; không in biên lai tính tiền. | Đọc dữ liệu (Read-Only) từ CSDL Microsoft SQL Server 2022. |
| **3** | **Phân tích đa chiều** | Xử lý OLAP 4 chiều: Slice, Dice, Roll-up, Drill-down theo Chi nhánh, Ngành hàng và Thời gian. | Không xây dựng Data Warehouse phân tán đa máy chủ; không dùng công cụ BI đám mây. | Thực thi thông qua truy vấn SQL nâng cao trên SQL Server Client. |
| **4** | **Đánh giá & Xếp hạng** | Ma trận Saaty 1-9 cho 5 tiêu chí; kiểm tra $CR \le 0.10$; chuẩn hóa Benefit/Cost; xếp hạng AHP. | Không cho phép quyết định cảm tính vi phạm $CR$; không tự động thay đổi giá vốn gốc. | Lõi thuật toán `DssEngine` viết bằng C# .NET 10.0 và Python. |
| **5** | **Khuyến nghị nhập hàng** | Tự động tính số lượng gợi ý đặt hàng dựa trên Lead Time, chu kỳ 30 ngày và tồn kho hiện tại. | Không tự động gửi đơn đặt hàng (PO) đến nhà cung ứng; không ký chứng từ mua bán. | Xuất ra cột `Suggested_Order_Qty` trên bảng kết quả DSS. |
| **6** | **Mô phỏng What-If** | 3 Kịch bản thị trường định sẵn; thanh trượt tùy biến tham số Nhu cầu, Lợi nhuận, Rủi ro vốn. | Không dự báo thời tiết/kinh tế vĩ mô phức tạp; không can thiệp thị trường thực tế. | Tái tính toán tức thời trên bộ nhớ RAM và vẽ đồ thị Delta. |
| **7** | **Quản lý danh mục SKU** | Cho phép thêm SKU mới trực tiếp trên Tab 4; tự động tái tính toán lại toàn bộ xếp hạng AHP. | Không quản lý vòng đời sản phẩm chi tiết (PLM); không quản lý thông số bảo hành từng máy. | Cập nhật vào bảng `PRODUCTS` và kích hoạt tính toán động. |
| **8** | **Quản lý tồn kho vật lý** | Đọc tồn kho thô (`stock_quantity`) để đánh giá tiêu chí Rủi ro tồn kho $C_4$ và số ngày tồn. | Không trừ kho vật lý tự động; không in tem RFID; không điều phối xe nâng; không kiểm kê. | Chỉ tiếp nhận số liệu tồn kho đầu vào từ bảng `PRODUCTS`. |
| **9** | **Quản lý tiếp thị** | Đề xuất tỷ trọng phân bổ ngân sách marketing tối ưu giữa các sản phẩm được phê duyệt. | Không chạy quảng cáo Facebook/Google; không phát mã coupon; không gửi tin nhắn SMS. | Đưa ra khuyến nghị định lượng trong Báo cáo thẩm định. |
| **10** | **Báo cáo & Phê duyệt** | Dashboard KPI; sinh Executive Summary tự động; xuất báo cáo thẩm định Word (.docx)/HTML. | Hệ thống không tự động phê duyệt; quyền quyết định cuối cùng thuộc về Giám đốc. | File báo cáo Word được lưu trữ cục bộ phục vụ họp giao ban. |

---

## 6. KẾ HOẠCH QUẢN LÝ THAY ĐỔI & TIÊU CHÍ CHẤP THUẬN

### 6.1. Quy trình quản lý yêu cầu thay đổi (Scope Change Management)
Để bảo vệ dự án khỏi các tác động tiêu cực của việc thay đổi yêu cầu liên tục, Nhóm 05 áp dụng quy trình quản lý thay đổi phạm vi nghiêm ngặt gồm 4 bước:
1. **Bước 1 (Tiếp nhận):** Mọi yêu cầu thêm tính năng mới phải được gửi bằng văn bản, nêu rõ lý do kinh doanh và nhóm chức năng bị ảnh hưởng.
2. **Bước 2 (Phân tích):** Trưởng nhóm và chuyên viên kỹ thuật tiến hành đánh giá tác động lên CSDL, thuật toán AHP và tiến độ hoàn thành.
3. **Bước 3 (Phê duyệt ranh giới):** Nếu tính năng thuộc danh mục Out-of-Scope (ví dụ: đòi hỏi in hóa đơn POS hay trừ kho tự động), yêu cầu sẽ bị từ chối chuyển sang giai đoạn tương lai.
4. **Bước 4 (Cập nhật):** Nếu yêu cầu được Giảng viên hướng dẫn chấp thuận, cập nhật lại tài liệu Đặc tả phạm vi này và thông báo đến toàn đội ngũ.

### 6.2. Tiêu chí nghiệm thu và chấp thuận (Acceptance Criteria)
Hệ thống DSS Retail Decision Studio 2026 được coi là hoàn thành đúng phạm vi và đạt tiêu chuẩn nghiệm thu khi đáp ứng trọn vẹn 100% các tiêu chuẩn định lượng sau:

| Mã | Hạng mục nghiệm thu | Tiêu chuẩn chấp thuận đạt (Pass Criteria) | Kết quả kiểm tra |
| :---: | :--- | :--- | :---: |
| **AC-01** | **Bảo mật & Đăng nhập** | Mật khẩu băm SHA-256 đối chiếu chính xác bảng USERS; gán đúng vai trò UserSession. | **ĐẠT (PASS)** |
| **AC-02** | **Cơ chế Fail-Safe** | Khi ngắt dịch vụ SQL Server, panel `pnlDbOverlay` che phủ và khóa cứng 6 Tab trong vòng < 1 giây. | **ĐẠT (PASS)** |
| **AC-03** | **Ma trận Saaty Tab 1** | Nhập ma trận 5 tiêu chí; tính đúng $\lambda_{\max}, CI$; hiển thị cảnh báo đỏ và khóa nút Chạy nếu $CR > 0.10$. | **ĐẠT (PASS)** |
| **AC-04** | **Chuẩn hóa & Xếp hạng Tab 2** | Áp dụng chuẩn xác hàm Benefit ($C_1, C_2, C_3$) và hàm Cost ($C_4, C_5$); phân đúng 3 nhóm Selected/Review/Reject. | **ĐẠT (PASS)** |
| **AC-05** | **Khuyến nghị số lượng đặt** | Công thức số lượng đặt hàng gợi ý tính toán chính xác dựa trên Lead Time và tồn kho thực tế. | **ĐẠT (PASS)** |
| **AC-06** | **Mô phỏng What-If Tab 3** | 3 Kịch bản hoạt động trơn tru; thanh trượt tùy biến cập nhật tức thời đồ thị Delta và Cây AHP. | **ĐẠT (PASS)** |
| **AC-07** | **Thêm SKU Động Tab 4** | Nhập SKU mới, lưu CSDL và tự động tái tính toán xếp hạng AHP toàn bộ danh mục thành công. | **ĐẠT (PASS)** |
| **AC-08** | **Phân tích OLAP Tab 5** | Thực hiện đầy đủ 4 thao tác Slice, Dice, Roll-up, Drill-down trên dữ liệu bán lẻ chính xác. | **ĐẠT (PASS)** |
| **AC-09** | **Dashboard & Báo cáo Tab 6** | Hiển thị đúng 4 KPI cards; sinh Executive Summary tự động; xuất thành công file Word (.docx) và HTML. | **ĐẠT (PASS)** |
| **AC-10** | **Cam kết Out-of-Scope** | Phần mềm không chứa các đoạn code can thiệp trái phép vào máy POS, kho vật lý WMS hay sàn TMĐT. | **ĐẠT (PASS)** |

---

## 7. KẾT LUẬN & XÁC NHẬN CỦA CÁC BÊN LIÊN QUAN

Tài liệu Đặc tả Phạm vi Hệ thống (*System Scope Specification*) đã phân định rõ ràng, khách quan và minh bạch nội hàm của dự án DSS Retail Decision Studio 2026. Bằng việc bám sát tuyệt đối các phân hệ In-Scope và kiên quyết nói không với các tính năng Out-of-Scope, nhóm sinh viên đảm bảo hoàn thành một sản phẩm công nghệ phần mềm chất lượng cao, đậm tính học thuật chuyên sâu của học phần Hệ Thống Thông Tin Hỗ Trợ Ra Quyết Định (IS 402 A), đáp ứng xuất sắc mọi yêu cầu của Nhà trường và Giảng viên hướng dẫn.

---

### PHẦN XÁC NHẬN KÝ DUYỆT

| ĐẠI DIỆN NHÓM SINH VIÊN THỰC HIỆN<br>*(Trưởng nhóm Nhóm 05)* | GIẢNG VIÊN HƯỚNG DẪN DUYỆT<br>*(Khoa Hệ Thống Thông Tin Quản Lý)* |
| :---: | :---: |
| *(Đã ký số nội bộ)*<br><br><br>**Trần Hồng Minh** | *(Ký và ghi rõ họ tên)*<br><br><br>**ThS. Nguyễn Quang Ánh** |
