# BÁO CÁO TOÀN DIỆN PHÂN TÍCH & THIẾT KẾ HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH (DSS)
**Môn học:** IS402 - Hệ thống thông tin hỗ trợ ra quyết định (Decision Support Systems)  
**Đề tài:** Hệ hỗ trợ ra quyết định trong Chiến lược Khuyến mãi và Phân bổ Hàng hóa  
**Phương pháp áp dụng:** Analytic Hierarchy Process (AHP - Phân tích thứ bậc Saaty) & OLAP đa chiều  
**Hệ quản trị CSDL:** Microsoft SQL Server  
**Nền tảng ứng dụng:** Desktop Application (.NET 10 WinForms & C#)  

---

## CHƯƠNG 1: MÔ TẢ CÁC CHỨC NĂNG QUẢN LÝ (MANAGEMENT FUNCTIONS)
### 1.1. Sơ đồ phân rã chức năng (BFD - Business Function Decomposition)
Sơ đồ BFD phân rã cấu trúc chức năng của hệ thống theo dạng thứ bậc (Hierarchy Tree), tập trung mô tả hệ thống **LÀM GÌ (What)** về mặt nghiệp vụ quản lý và hỗ trợ ra quyết định, không biểu diễn luồng dữ liệu hay kỹ thuật lập trình:

```mermaid
flowchart LR
    %% Định dạng style BFD chuẩn: Khung vuông vắn, đường nối vô hướng (không mũi tên)
    classDef root fill:#0f172a,stroke:#38bdf8,stroke-width:2.5px,color:#ffffff,font-weight:bold,font-size:12px;
    classDef mod fill:#1e40af,stroke:#60a5fa,stroke-width:2px,color:#ffffff,font-weight:bold,font-size:11px;
    classDef fn fill:#ffffff,stroke:#64748b,stroke-width:1px,color:#0f172a,font-size:10px;

    ROOT["0.0 HỆ THỐNG HỖ TRỢ RA QUYẾT ĐỊNH BÁN LẺ\n(DSS RETAIL DECISION STUDIO)"]:::root

    %% Cấp 1: 5 Khối chức năng nghiệp vụ chính
    F1["1.0 QUẢN TRỊ HỆ THỐNG"]:::mod
    F2["2.0 QUẢN LÝ DỮ LIỆU\nBÁN HÀNG & KHO (OLTP)"]:::mod
    F3["3.0 HỖ TRỢ RA QUYẾT ĐỊNH\nĐA TIÊU CHÍ (AHP)"]:::mod
    F4["4.0 PHÂN TÍCH ĐỘ NHẠY\n& MÔ PHỎNG KỊCH BẢN"]:::mod
    F5["5.0 PHÂN TÍCH ĐA CHIỀU\n& BÁO CÁO QUẢN TRỊ"]:::mod

    ROOT --- F1
    ROOT --- F2
    ROOT --- F3
    ROOT --- F4
    ROOT --- F5

    %% Cấp 2: Khối 1.0 Quản trị hệ thống
    F1 --- F11["1.1 Quản lý người dùng"]:::fn
    F1 --- F12["1.2 Phân quyền vai trò sử dụng"]:::fn
    F1 --- F13["1.3 Quản lý nhật ký truy cập & Thao tác"]:::fn

    %% Cấp 2: Khối 2.0 Quản lý dữ liệu bán hàng & kho
    F2 --- F21["2.1 Quản lý danh mục & Thông tin sản phẩm"]:::fn
    F2 --- F22["2.2 Quản lý tồn kho & Ngưỡng an toàn"]:::fn
    F2 --- F23["2.3 Quản lý đơn hàng & Lịch sử bán hàng"]:::fn
    F2 --- F24["2.4 Quản lý khách hàng & Chương trình khuyến mãi"]:::fn
    F2 --- F25["2.5 Quản lý phương án sản phẩm ứng viên"]:::fn

    %% Cấp 2: Khối 3.0 Hỗ trợ ra quyết định đa tiêu chí (AHP)
    F3 --- F31["3.1 Quản lý phiên ra quyết định"]:::fn
    F3 --- F32["3.2 Thiết lập hệ thống tiêu chí đánh giá"]:::fn
    F3 --- F33["3.3 Xác định trọng số tiêu chí (So sánh cặp Saaty)"]:::fn
    F3 --- F34["3.4 Kiểm định tính nhất quán logic (Tỷ số CR)"]:::fn
    F3 --- F35["3.5 Đánh giá & Xếp hạng phương án sản phẩm"]:::fn
    F3 --- F36["3.6 Đề xuất kế hoạch nhập kho & Ngân sách xúc tiến"]:::fn

    %% Cấp 2: Khối 4.0 Phân tích độ nhạy & Mô phỏng kịch bản
    F4 --- F41["4.1 Thiết lập kịch bản biến động thị trường"]:::fn
    F4 --- F42["4.2 Giả lập thay đổi tham số tiêu chí (What-If)"]:::fn
    F4 --- F43["4.3 Đánh giá mức độ biến thiên thứ hạng phương án"]:::fn
    F4 --- F44["4.4 Khôi phục trạng thái đánh giá chuẩn ban đầu"]:::fn

    %% Cấp 2: Khối 5.0 Phân tích đa chiều & Báo cáo quản trị
    F5 --- F51["5.1 Phân tích tổng hợp tồn kho đa cấp (OLAP ROLLUP)"]:::fn
    F5 --- F52["5.2 Phân tích hiệu quả khuyến mãi đa chiều (OLAP CUBE)"]:::fn
    F5 --- F53["5.3 Lập biên bản họp thẩm định phê duyệt"]:::fn
    F5 --- F54["5.4 Kết xuất báo cáo phân tích quản trị"]:::fn
```

### 1.2. Sơ đồ Use Case tổng quát (Overall Use Case Diagram)
Hệ thống phân quyền cho 3 tác nhân chính:
1. **Quản lý kinh doanh (Decision Maker):** Thiết lập ma trận trọng số Saaty, kích hoạt chạy mô hình DSS, phê duyệt danh sách nhập hàng hoặc xả khuyến mãi.
2. **Chuyên viên phân tích (Data / Business Analyst):** Nhập liệu sản phẩm mới, thiết lập tham số thị trường, khai thác các khối phân tích OLAP.
3. **Quản trị viên (System Admin):** Quản lý tài khoản, cấu hình tham số hệ thống và bảo trì cơ sở dữ liệu.

```mermaid
graph LR
    DM((Quản Lý Kinh Doanh / Giám Đốc))
    BA((Chuyên Viên Phân Tích))
    Admin((Quản Trị Hệ Thống))

    subgraph "Hệ Thống DSS Retail Studio 2026"
        UC1[UC01: Thiết lập ma trận so sánh cặp Saaty]
        UC2[UC02: Chạy mô hình giải thuật AHP]
        UC3[UC03: Xem kết quả xếp hạng & Biểu đồ trực quan]
        UC4[UC04: Phê duyệt phân bổ hàng & khuyến mãi]
        UC5[UC05: Thêm phương án / sản phẩm mới]
        UC6[UC06: Khai thác phân tích đa chiều OLAP]
        UC7[UC07: Xuất biên bản thẩm định + Trợ lý AI]
        UC8[UC08: Quản trị tài khoản & CSDL]
    end

    DM --> UC1
    DM --> UC2
    DM --> UC3
    DM --> UC4
    DM --> UC7

    BA --> UC5
    BA --> UC6
    BA --> UC3

    Admin --> UC8
```

---

## CHƯƠNG 2: CÁC CHỨC NĂNG CẦN HỖ TRỢ RA QUYẾT ĐỊNH (DSS REQUIREMENTS)

Trong bài toán **"Chiến lược khuyến mãi và phân bổ hàng hóa"**, ban lãnh đạo doanh nghiệp thường xuyên đối mặt với các xung đột mục tiêu (trade-offs). Hệ thống DSS được xây dựng để giải quyết 4 bài toán bán hàng thực tế:

1. **Bài toán 1: Lựa chọn sản phẩm chiến lược nhập hàng (Product Selection):**
   - *Thực trạng:* Doanh nghiệp chỉ có ngân sách marketing và diện tích kho hạn chế, nhưng có hàng chục mặt hàng ứng viên.
   - *Giải pháp DSS:* Áp dụng AHP để chấm điểm đa tiêu chí (vừa xem xét Biên LN, Doanh thu kỳ vọng, vừa tính đến Rủi ro tồn kho và Chi phí quảng cáo).
2. **Bài toán 2: Quyết định Phân bổ Hàng hóa (Inventory Allocation):**
   - *Thực trạng:* Một số mặt hàng bán rất chạy nhưng thiếu hàng (`stock_quantity < min_stock_level`), trong khi mặt hàng khác ứ đọng vốn lâu ngày.
   - *Giải pháp DSS:* Tự động kích hoạt cảnh báo phân bổ bổ sung cho nhóm Hạng 1 & 2; hạn chế nhập cho nhóm Hạng 3.
3. **Bài toán 3: Quyết định Kích hoạt Khuyến mãi Xả hàng (Clearance Promotion Strategy):**
   - *Thực trạng:* Mặt hàng xếp hạng thấp (Hạng 5 / Loại bỏ) có thời gian lưu kho trên 90 ngày gây đọng vốn lưu động.
   - *Giải pháp DSS:* Tự động gắn nhãn `Rejected`, đề xuất kịch bản giảm giá sốc 30-40% hoặc combo quà tặng để giải phóng kho hàng.
4. **Bài toán 4: Giả lập biến động thị trường & Đánh giá độ nhạy (What-If / Sensitivity Analysis):**
   - *Giải pháp DSS:* Cho phép quản lý thay đổi nhanh điểm nhu cầu thị trường hoặc chi phí nhập khẩu để quan sát sự thay đổi thứ hạng ngay tức thì.

---

## CHƯƠNG 3: QUI TRÌNH HỖ TRỢ RA QUYẾT ĐỊNH (FLOWCHARTS)

### 3.1. Lưu đồ nghiệp vụ tổng thể (Business Decision Flowchart)
```mermaid
flowchart TD
    Start([Bắt đầu kỳ kế hoạch Q4]) --> InputData[1. Thu thập dữ liệu OLTP: Doanh thu, Tồn kho, Giá vốn]
    InputData --> AddNew{Có phương án/sản phẩm mới?}
    AddNew -- Có --> InsertDB[Thêm sản phẩm & chấm điểm tiêu chí]
    AddNew -- Không --> SelectSession[2. Khởi tạo phiên quyết định DSS]
    InsertDB --> SelectSession
    
    SelectSession --> Pairwise[3. Hội đồng chuyên gia đánh giá Ma trận cặp Saaty 1-9]
    Pairwise --> CalcCR[4. Tính Tỷ số nhất quán CR]
    
    CalcCR --> CheckCR{CR < 0.10?}
    CheckCR -- Không đạt (CR >= 0.10) --> AdjustSaaty[Điều chỉnh lại ma trận so sánh cặp để đảm bảo logic]
    AdjustSaaty --> Pairwise
    
    CheckCR -- Đạt chuẩn (CR < 0.10) --> CalcWeight[5. Tính Vector trọng số tiêu chí W_i]
    CalcWeight --> CalcScore[6. Tính điểm tổng hợp AHP: Sigma W_i * S_ij]
    CalcScore --> RankAlt[7. Xếp hạng phương án bằng DENSE_RANK]
    
    RankAlt --> DecisionRule{Xếp hạng phương án}
    DecisionRule -- Rank = 1 (Tối ưu) --> ActionSelected[Phê duyệt nhập 150 chiếc & phân bổ 50% ngân sách Mkt]
    DecisionRule -- Rank 2-3 (Khả quan) --> ActionReview[Phân bổ thăm dò 30-40 chiếc theo đơn cọc]
    DecisionRule -- Rank cuối (Kém) --> ActionClearance[Kích hoạt khuyến mãi giảm giá 40% xả tồn kho]
    
    ActionSelected --> Report[8. Xuất biên bản họp hội đồng & AI Insights]
    ActionReview --> Report
    ActionClearance --> Report
    Report --> End([Kết thúc phiên ra quyết định])
```

### 3.2. Lưu đồ thuật toán AHP (AHP Algorithm Detailed Flowchart)
```mermaid
flowchart TD
    A[Ma trận so sánh cặp A kích thước n x n] --> B[Bước 1: Tính tổng từng cột: C_j = sum a_ij]
    B --> C[Bước 2: Chuẩn hóa ma trận: a'_ij = a_ij / C_j]
    C --> D[Bước 3: Tính vector trọng số tiêu chí: w_i = average dòng a'_ij]
    D --> E[Bước 4: Tính Lambda Max = sum C_j * w_j]
    E --> F[Bước 5: Tính Chỉ số nhất quán CI = LambdaMax - n / n - 1]
    F --> G[Bước 6: Tính Tỷ số nhất quán CR = CI / RI]
    G --> H{CR < 0.10?}
    H -- Sai --> ReEval[Ma trận mâu thuẫn -> Yêu cầu đánh giá lại cặp tiêu chí]
    H -- Đúng --> Synthesis[Bước 7: Tổng hợp điểm số: FinalScore_j = sum w_i * Score_ij]
    Synthesis --> OutputResult[Ghi nhận vào DECISION_RESULTS và cập nhật thứ hạng]
```

---

## CHƯƠNG 4: THIẾT KẾ CƠ SỞ DỮ LIỆU DSS TRÊN SQL SERVER

### 4.1. Danh mục 12 Thực thể CSDL (Data Entities)
Hệ thống được thiết kế thành 2 phân hệ hữu cơ:
1. **Phân hệ Vận hành Bán hàng (Sales Operational Module):** Cung cấp dữ liệu lịch sử, doanh thu, giá vốn, tồn kho làm căn cứ thực tế (quantitative facts).
2. **Phân hệ Hỗ trợ ra quyết định AHP (DSS Decision Engine Module):** Quản lý phiên quyết định, cây tiêu chí đánh giá, ma trận so sánh cặp (Saaty scale 1-9), trọng số và kết quả xếp hạng phương án.

| STT | Thực thể (Entity) | Phân hệ | Ý nghĩa và vai trò trong hệ thống DSS |
| :---: | :--- | :--- | :--- |
| 1 | **USERS** | Quản lý chung | Lưu trữ người dùng hệ thống: Quản lý kinh doanh (Decision Maker), Chuyên viên phân tích (Analyst), Quản trị viên (Admin). |
| 2 | **CATEGORIES** | Quản lý bán hàng | Danh mục ngành hàng bán (Điện thoại, Laptop, Thiết bị đeo, Phụ kiện). Phục vụ gom nhóm phân tích. |
| 3 | **PRODUCTS** | Quản lý bán hàng | Dữ liệu sản phẩm kinh doanh: Giá nhập, giá niêm yết, tồn kho, mức độ hao hụt/rủi ro. Là nguồn ứng viên cho các phương án DSS. |
| 4 | **CUSTOMERS** | Quản lý bán hàng | Thông tin khách hàng, phân khúc thị trường (VIP, Khách buôn, Khách lẻ), điểm tích lũy. |
| 5 | **ORDERS** | Quản lý bán hàng | Hóa đơn bán hàng: ngày mua, tổng tiền, phương thức thanh toán, trạng thái. Dữ liệu để tính tổng doanh thu. |
| 6 | **ORDER_DETAILS** | Quản lý bán hàng | Chi tiết hóa đơn: số lượng bán, đơn giá bán thực tế, chiết khấu. Dữ liệu tính doanh thu thực và lợi nhuận gộp theo từng sản phẩm. |
| 7 | **DECISION_SESSIONS** | Hỗ trợ ra quyết định | Phiên ra quyết định (Ví dụ: "Phiên Q4/2026: Phân bổ hàng & Khuyến mãi"). Quản lý chu kỳ, mục tiêu, tỷ số nhất quán CR. |
| 8 | **CRITERIA** | Hỗ trợ ra quyết định | Cây tiêu chí đánh giá AHP (C1: Biên LN, C2: Doanh thu kỳ vọng, C3: Nhu cầu thị trường, C4: Rủi ro vốn, C5: Chi phí tiếp thị). |
| 9 | **ALTERNATIVES** | Hỗ trợ ra quyết định | Các phương án đưa vào phiên xét duyệt (Các sản phẩm ứng viên A, B, C hoặc các chiến lược bán hàng cụ thể). Liên kết tới `PRODUCTS`. |
| 10 | **PAIRWISE_COMPARISONS** | Hỗ trợ ra quyết định | Lưu ma trận so sánh cặp giữa 2 tiêu chí theo thang đo Saaty 1-9 ($a_{ij}$ và giá trị nghịch đảo $a_{ji} = 1/a_{ij}$). Cơ sở tính vector trọng số $w_i$. |
| 11 | **ALTERNATIVE_SCORES** | Hỗ trợ ra quyết định | Điểm đánh giá của từng phương án theo từng tiêu chí (Điểm đo lường thực tế `raw_value` và điểm chuẩn hóa `normalized_score`). |
| 12 | **DECISION_RESULTS** | Hỗ trợ ra quyết định | Kết quả tổng hợp cuối cùng của mô hình AHP: Điểm tổng hợp (`final_score`), Xếp hạng (`ranking`), Khuyến nghị hệ thống (`decision_outcome`). |

### 4.2. Sơ đồ quan hệ thực thể (ERD - Entity Relationship Diagram)
```mermaid
erDiagram
    USERS ||--o{ DECISION_SESSIONS : "created_by"
    CATEGORIES ||--o{ PRODUCTS : "contains"
    PRODUCTS ||--o{ ORDER_DETAILS : "ordered_in"
    PRODUCTS ||--o{ ALTERNATIVES : "mapped_to"
    CUSTOMERS ||--o{ ORDERS : "places"
    ORDERS ||--|{ ORDER_DETAILS : "consists_of"

    DECISION_SESSIONS ||--|{ PAIRWISE_COMPARISONS : "has_matrix"
    DECISION_SESSIONS ||--|{ ALTERNATIVES : "evaluates"
    DECISION_SESSIONS ||--|{ ALTERNATIVE_SCORES : "records"
    DECISION_SESSIONS ||--|{ DECISION_RESULTS : "produces"

    CRITERIA ||--o{ PAIRWISE_COMPARISONS : "criterion_1"
    CRITERIA ||--o{ PAIRWISE_COMPARISONS : "criterion_2"
    CRITERIA ||--o{ ALTERNATIVE_SCORES : "criterion"

    ALTERNATIVES ||--|{ ALTERNATIVE_SCORES : "scored_in"
    ALTERNATIVES ||--|| DECISION_RESULTS : "ranked_as"

    PRODUCTS {
        int product_id PK
        int category_id FK
        string product_code
        string product_name
        decimal cost_price
        decimal selling_price
        int stock_quantity
    }

    DECISION_SESSIONS {
        int session_id PK
        string session_code
        decimal consistency_ratio
        bit is_consistent
    }

    CRITERIA {
        int criterion_id PK
        string criterion_code
        string criterion_name
        string criterion_type
        decimal weight
    }

    PAIRWISE_COMPARISONS {
        int comparison_id PK
        int session_id FK
        int criterion_id_1 FK
        int criterion_id_2 FK
        decimal saaty_value
    }

    ALTERNATIVE_SCORES {
        int score_id PK
        int session_id FK
        int alternative_id FK
        int criterion_id FK
        decimal raw_value
        decimal normalized_score
    }

    DECISION_RESULTS {
        int result_id PK
        int session_id FK
        int alternative_id FK
        decimal final_score
        int ranking
        string decision_outcome
    }
```

### 4.3. Chuẩn hóa CSDL (Database Normalization từ 1NF đến 3NF)
- **1NF (Tính nguyên tử):** Mọi trường dữ liệu đều là đơn trị. Không lưu mảng số liệu hay chuỗi JSON trong CSDL. Ma trận so sánh cặp được lưu dạng bảng dọc nguyên tử `PAIRWISE_COMPARISONS`.
- **2NF (Khử phụ thuộc một phần):** Tách bạch thông tin sản phẩm ra khỏi chi tiết đơn hàng `ORDER_DETAILS`. Các tiêu chí được chuẩn hóa độc lập trong `CRITERIA`.
- **3NF (Khử phụ thuộc bắc cầu):** Mọi thuộc tính không khóa đều phụ thuộc trực tiếp vào khóa chính. `category_name` được tách riêng trong `CATEGORIES`, không lưu trực tiếp trong `PRODUCTS`.

---

## CHƯƠNG 5: THIẾT KẾ BÁO CÁO XUẤT THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH (REPORTS)

Hệ thống cung cấp 3 định dạng báo cáo thông tin phục vụ lãnh đạo:
1. **Báo cáo Bảng Xếp Hạng & Khuyến Nghị Quyết Định (Final Priority Report):**
   - Thể hiện trực quan thứ hạng, điểm AHP, nhãn phê duyệt (`Selected`, `UnderReview`, `Rejected`).
   - Đính kèm biểu đồ thanh tỉ lệ phần trăm trực quan.
2. **Báo cáo Thẩm Định Tính Nhất Quán Ma Trận Saaty (AHP Consistency Audit Report):**
   - Báo cáo chi tiết ma trận $5 	imes 5$, vector trọng số tiêu chí và Tỷ số nhất quán ($CR = 0.0152 < 0.10$).
3. **Biên Bản Họp Hội Đồng Chiến Lược Bán Hàng Xuất File (Text / CSV Export):**
   - Cho phép xuất file văn bản pháp lý lưu hồ sơ doanh nghiệp gồm đầy đủ đề xuất phân bổ ngân sách và giải pháp xử lý hàng tồn kho.

---

## CHƯƠNG 6: TRUY VẤN XỬ LÝ DỮ LIỆU & PHÂN TÍCH ĐA CHIỀU OLAP

### 6.1. Thuật toán AHP tích hợp trực tiếp trong SQL Server (CTE & Merge)
Truy vấn SQL thực hiện trọn vẹn thuật toán AHP bằng Common Table Expressions (CTE):
```sql
WITH WeightCalculated AS (
    SELECT pc.criterion_id_1 AS criterion_id,
           AVG(pc.saaty_value / col_sums.col_sum) AS criterion_weight
    FROM dbo.PAIRWISE_COMPARISONS pc
    INNER JOIN (
        SELECT criterion_id_2, SUM(saaty_value) AS col_sum
        FROM dbo.PAIRWISE_COMPARISONS WHERE session_id = 1 GROUP BY criterion_id_2
    ) col_sums ON pc.criterion_id_2 = col_sums.criterion_id_2
    WHERE pc.session_id = 1 GROUP BY pc.criterion_id_1
),
AltScores AS (
    SELECT s.alternative_id, SUM(w.criterion_weight * s.normalized_score) AS comp_score
    FROM dbo.ALTERNATIVE_SCORES s
    INNER JOIN WeightCalculated w ON s.criterion_id = w.criterion_id
    WHERE s.session_id = 1 GROUP BY s.alternative_id
),
Ranked AS (
    SELECT alternative_id, comp_score,
           DENSE_RANK() OVER (ORDER BY comp_score DESC) as rk
    FROM AltScores
)
MERGE dbo.DECISION_RESULTS AS target
USING Ranked AS source
ON (target.session_id = 1 AND target.alternative_id = source.alternative_id)
WHEN MATCHED THEN
    UPDATE SET target.final_score = source.comp_score,
               target.ranking = source.rk,
               target.decision_outcome = CASE WHEN source.rk = 1 THEN 'Selected' WHEN source.comp_score >= 0.60 THEN 'UnderReview' ELSE 'Rejected' END,
               target.decision_date = SYSDATETIME();
```

### 6.2. Truy vấn Phân tích đa chiều OLAP ROLLUP (Phân bổ hàng hóa)
```sql
SELECT 
    CASE WHEN GROUPING(c.category_name) = 1 THEN N'-- TẤT CẢ NGÀNH HÀNG --' ELSE c.category_name END AS [Ngành Hàng],
    CASE WHEN GROUPING(p.product_name) = 1 THEN N'-- TỔNG CỘNG NHÓM --' ELSE p.product_name END AS [Sản Phẩm],
    SUM(p.stock_quantity) AS [Tổng Tồn Kho],
    ISNULL(SUM(od.quantity), 0) AS [Tổng Đã Bán],
    FORMAT(ISNULL(SUM(od.line_total), 0), '#,##0') + N' đ' AS [Doanh Thu],
    CASE 
        WHEN SUM(p.stock_quantity) > 100 AND ISNULL(SUM(od.quantity), 0) < 20 
        THEN N'⚠️ NGUY CƠ ĐỌNG VỐN: ĐỀ XUẤT GIẢM GIÁ KHUYẾN MÃI'
        WHEN SUM(p.stock_quantity) < 15 
        THEN N'⚡ SẮP CHÁY HÀNG: CẦN PHÂN BỔ NHẬP THÊM'
        ELSE N'✅ CÂN BẰNG TỒN KHO'
    END AS [Khuyến Nghị Phân Bổ DSS]
FROM dbo.CATEGORIES c
INNER JOIN dbo.PRODUCTS p ON c.category_id = p.category_id
LEFT JOIN dbo.ORDER_DETAILS od ON p.product_id = od.product_id
GROUP BY ROLLUP(c.category_name, p.product_name);
```

### 6.3. Truy vấn Phân tích đa chiều OLAP CUBE (Chiến lược khuyến mãi)
```sql
SELECT 
    ISNULL(cust.customer_segment, N'-- TẤT CẢ PHÂN KHÚC --') AS [Phân Khúc Khách],
    ISNULL(cat.category_name, N'-- TẤT CẢ NGÀNH HÀNG --') AS [Ngành Hàng],
    COUNT(DISTINCT o.order_id) AS [Số Đơn Hàng],
    FORMAT(ISNULL(SUM(od.line_total), 0), '#,##0') + N' đ' AS [Doanh Thu],
    CASE 
        WHEN cust.customer_segment = 'VIP' THEN N'Tặng Voucher Tri Ân 10% & Dịch Vụ Đặc Quyền'
        WHEN cust.customer_segment = 'Retail' AND ISNULL(SUM(od.quantity), 0) < 5 THEN N'Áp Dụng Khuyến Mãi Mua 1 Tặng 1 Để Kích Cầu'
        ELSE N'Duy Trì Giá Chuẩn'
    END AS [Chiến Lược Khuyến Mãi Gợi Ý]
FROM dbo.CUSTOMERS cust
INNER JOIN dbo.ORDERS o ON cust.customer_id = o.customer_id
INNER JOIN dbo.ORDER_DETAILS od ON o.order_id = od.order_id
INNER JOIN dbo.PRODUCTS p ON od.product_id = p.product_id
INNER JOIN dbo.CATEGORIES cat ON p.category_id = cat.category_id
GROUP BY CUBE(cust.customer_segment, cat.category_name);
```

---

## CHƯƠNG 7: KIẾN TRÚC PHẦN MỀM GUI VÀ TÍCH HỢP TRỢ LÝ AI

### 7.1. Cấu trúc 5 Tab tương tác chuẩn quy trình DSS:
- **Tab 1: 📥 Đầu Vào Dữ Liệu (Inputs & Parameters):** Trình diễn 2 bảng dữ liệu nguồn: Danh sách phương án ứng viên kèm thông số kỹ thuật và Ma trận so sánh cặp Saaty 5x5. Nút bấm kích hoạt chạy mô hình AHP và nút giả lập biến động thị trường.
- **Tab 2: 📊 Kết Quả Ra Quyết Định (Outputs & Decision):** Khởi tạo ở trạng thái Chờ (Empty State). Sau khi người dùng bấm chạy, hiển thị 4 KPI cards động, Bảng xếp hạng chi tiết có gắn huy hiệu (Pill Badges) và Biểu đồ thanh trực quan AHP.
- **Tab 3: ➕ Thêm Sản Phẩm Mới:** Biểu mẫu 2 cột chuẩn hóa giúp người dùng nhập sản phẩm mới và chấm điểm 5 tiêu chí để bổ sung phương án vào phiên làm việc.
- **Tab 4: 📦 Dữ Liệu Bán Hàng Lịch Sử (OLAP):** Cung cấp các góc nhìn phân tích dữ liệu bán hàng lịch sử làm căn cứ định lượng cho mô hình DSS.
- **Tab 5: 🤖 Báo Cáo & AI Insights:** Trợ lý thông minh tự động phân tích kết quả xếp hạng, chỉ ra nguyên nhân dẫn đầu của phương án tối ưu, phân bổ ngân sách 50% tiếp thị và cảnh báo kịch bản xả kho cho phương án xếp chót.

---
**KẾT LUẬN:** Đồ án đáp ứng toàn diện và vượt mức 6 yêu cầu bắt buộc và 2 yêu cầu mở rộng (+AI, OLAP) trong chương trình đào tạo môn IS402.
