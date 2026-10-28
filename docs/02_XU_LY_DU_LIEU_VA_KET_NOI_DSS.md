# BÁO CÁO TASK 06: XỬ LÝ DỮ LIỆU & KẾT NỐI DATABASE VỚI DSS
**Môn học:** IS402 - Hệ thống thông tin hỗ trợ ra quyết định  
**Đề tài:** Hệ hỗ trợ ra quyết định lựa chọn sản phẩm và chiến lược kinh doanh bán hàng  
**Mô hình ra quyết định:** AHP (Analytic Hierarchy Process - Thomas L. Saaty)  
**Cơ sở dữ liệu:** Microsoft SQL Server (`DSS_SalesDB`)  

---

## MỤC TIÊU CỐT LÕI
> **Luồng xử lý:** `Database (SQL Server)` $\longrightarrow$ `SQL Query` $\longrightarrow$ `DSS Model (AHP Engine)` $\longrightarrow$ `Result (Decision Results)`  
> **Nguyên tắc vàng:** Tuyệt đối không hard-code dữ liệu trong mã nguồn. Dữ liệu thay đổi trong Database $\implies$ Kết quả DSS phải tự động thay đổi theo.

---

## 6.1. VIẾT SQL LẤY DỮ LIỆU (DATASET EXTRACTION)

Hệ thống trích xuất tập dataset đầu vào hoàn chỉnh cho phiên quyết định (Session ID = 1):
- Thông tin phiên họp (`DECISION_SESSIONS`)
- Danh sách các phương án ứng viên (`ALTERNATIVES`) liên kết với thông tin sản phẩm kho (`PRODUCTS`)
- Cây tiêu chí đánh giá (`CRITERIA`)
- Bảng điểm đo lường và điểm chuẩn hóa (`ALTERNATIVE_SCORES`)

```sql
SELECT 
    ds.session_id, ds.session_code, ds.session_title,
    alt.alternative_id, alt.alternative_code, alt.alternative_name,
    p.product_code, p.cost_price, p.selling_price, p.stock_quantity,
    c.criterion_id, c.criterion_code, c.criterion_name, c.criterion_type,
    s.raw_value, s.normalized_score
FROM dbo.DECISION_SESSIONS ds
INNER JOIN dbo.ALTERNATIVES alt ON ds.session_id = alt.session_id
LEFT JOIN dbo.PRODUCTS p ON alt.product_id = p.product_id
INNER JOIN dbo.ALTERNATIVE_SCORES s ON alt.alternative_id = s.alternative_id AND ds.session_id = s.session_id
INNER JOIN dbo.CRITERIA c ON s.criterion_id = c.criterion_id
WHERE ds.session_id = 1
ORDER BY alt.alternative_id ASC, c.criterion_id ASC;
```

---

## 6.2. SQL CRUD (CREATE - READ - UPDATE - DELETE)

Triển khai đầy đủ 4 thao tác quản trị trên CSDL:

1. **CREATE (INSERT):**
   ```sql
   INSERT INTO dbo.PRODUCTS (category_id, product_code, product_name, cost_price, selling_price, stock_quantity)
   VALUES (1, 'PRD_IPHONE_16PM', N'iPhone 16 Pro Max 256GB', 26500000, 33990000, 30);
   ```
2. **READ (SELECT):** Truy vấn dữ liệu sản phẩm kèm tính toán động biên lợi nhuận % và phân loại nhóm biên độ:
   ```sql
   SELECT product_code, product_name, cost_price, selling_price,
          (selling_price - cost_price) AS unit_profit,
          CAST(((selling_price - cost_price) / selling_price) * 100.0 AS DECIMAL(5,2)) AS margin_pct
   FROM dbo.PRODUCTS WHERE product_code = 'PRD_IPHONE_16PM';
   ```
3. **UPDATE:** Cập nhật thông số giá bán và biến động tồn kho:
   ```sql
   UPDATE dbo.PRODUCTS
   SET selling_price = 34490000, stock_quantity = stock_quantity + 10
   WHERE product_code = 'PRD_IPHONE_16PM';
   ```
4. **DELETE:** Xóa dòng dữ liệu thử nghiệm an toàn:
   ```sql
   DELETE FROM dbo.PRODUCTS WHERE product_code = 'PRD_IPHONE_16PM';
   ```

---

## 6.3. SQL THỐNG KÊ (AGGREGATION)

Sử dụng đầy đủ các hàm thống kê: `COUNT`, `SUM`, `AVG`, `MIN`, `MAX`, `GROUP BY`, `HAVING`, `ORDER BY`:
- Phân tích hiệu quả kinh doanh thực tế theo từng ngành hàng (Laptop, Điện thoại, Phụ kiện...).
- Cung cấp dữ liệu định lượng (`Fact`) làm cơ sở đánh giá các tiêu chí Doanh thu và Biên lợi nhuận trong DSS.

```sql
SELECT 
    c.category_name AS [Ngành hàng],
    COUNT(DISTINCT p.product_id) AS [Số mặt hàng kinh doanh],
    ISNULL(SUM(od.quantity), 0) AS [Tổng số lượng đã bán],
    ISNULL(SUM(od.line_total), 0) AS [Tổng doanh thu thực tế],
    ISNULL(AVG(od.line_total), 0) AS [Doanh thu trung bình / đơn],
    MIN(CAST(((p.selling_price - p.cost_price) / p.selling_price) * 100.0 AS DECIMAL(5,2))) AS [Biên LN thấp nhất (%)],
    MAX(CAST(((p.selling_price - p.cost_price) / p.selling_price) * 100.0 AS DECIMAL(5,2))) AS [Biên LN cao nhất (%)]
FROM dbo.CATEGORIES c
INNER JOIN dbo.PRODUCTS p ON c.category_id = p.category_id
LEFT JOIN dbo.ORDER_DETAILS od ON p.product_id = od.product_id
LEFT JOIN dbo.ORDERS o ON od.order_id = o.order_id AND o.order_status = 'Completed'
GROUP BY c.category_name
HAVING COUNT(DISTINCT p.product_id) > 0
ORDER BY [Tổng doanh thu thực tế] DESC;
```

---

## 6.4. SQL JOIN (KẾT HỢP NHIỀU BẢNG LIÊN PHÂN HỆ)

Kết hợp **7 bảng** giữa phân hệ Quản lý bán hàng và phân hệ DSS:
`DECISION_SESSIONS` + `USERS` + `ALTERNATIVES` + `PRODUCTS` + `CATEGORIES` + `ALTERNATIVE_SCORES` + `CRITERIA`.

Truy vấn này tạo nên một bức tranh toàn diện cho Hội đồng thẩm định: từng sản phẩm thuộc ngành hàng nào, tồn kho bao nhiêu, người chủ trì là ai và được chấm điểm từng tiêu chí AHP thế nào.

---

## 6.5. SQL PHỤC VỤ DSS (TÍNH ĐIỂM, TÌM PHƯƠNG ÁN TỐT NHẤT, XẾP HẠNG, LỌC)

### 1. Tính điểm tổng hợp AHP và Xếp hạng tự động bằng Window Function:
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
AlternativeCompositeScores AS (
    SELECT s.alternative_id, SUM(w.criterion_weight * s.normalized_score) AS composite_ahp_score
    FROM dbo.ALTERNATIVE_SCORES s
    INNER JOIN WeightCalculated w ON s.criterion_id = w.criterion_id
    WHERE s.session_id = 1 GROUP BY s.alternative_id
)
SELECT 
    DENSE_RANK() OVER (ORDER BY acs.composite_ahp_score DESC) AS [Xếp hạng],
    alt.alternative_code AS [Mã PA],
    alt.alternative_name AS [Tên Phương Án],
    CAST(acs.composite_ahp_score AS DECIMAL(8,4)) AS [Điểm AHP],
    CASE 
        WHEN DENSE_RANK() OVER (ORDER BY acs.composite_ahp_score DESC) = 1 THEN 'Selected'
        WHEN acs.composite_ahp_score >= 0.60 THEN 'UnderReview'
        ELSE 'Rejected'
    END AS [Quyết Định Đề Xuất]
FROM AlternativeCompositeScores acs
INNER JOIN dbo.ALTERNATIVES alt ON acs.alternative_id = alt.alternative_id
ORDER BY [Xếp hạng] ASC;
```

### 2. Lọc phương án tối ưu theo ràng buộc kinh doanh kép:
Lọc ra các phương án thỏa mãn đồng thời:
- Điểm AHP tổng hợp $\ge 0.70$
- Biên lợi nhuận gộp $\ge 25\%$
- Tồn kho an toàn $\le 50$ chiếc.

### 3. So sánh đối đầu (Head-to-Head Comparison):
Đối chiếu chi tiết điểm số của từng tiêu chí giữa **Hạng 1** (Galaxy S24 Ultra) và **Hạng 2** (Dell Gaming G15) để thấy rõ ưu thế cạnh tranh của từng phương án.

---

## 6.6. KẾT NỐI DATABASE VỚI DSS & CHỨNG MINH TIÊU CHÍ "DONE"

### 1. Kiến trúc kết nối
- File thực thi: [`src/dss_engine.py`](file:///g:/Decision%20System/src/dss_engine.py)
- Driver: `pyodbc` kết nối trực tiếp `MSSQL$SQLEXPRESS25` (Database: `DSS_SalesDB`).
- Thuật toán: AHP tiêu chuẩn tính trọng số $w_i$, $\lambda_{max}$, $CI$, $CR$ và tổng hợp điểm $\sum w_i \times s_{ij}$.
- Cơ chế ghi ngược: Kết quả xếp hạng được ghi đè tự động vào bảng `DECISION_RESULTS` trong SQL Server.

### 2. Kiểm chứng tiêu chí DONE qua Script [`src/test_dss_pipeline.py`](file:///g:/Decision%20System/src/test_dss_pipeline.py)

| Giai đoạn | Hành động trên Database | Kết quả DSS (Hạng 1) | Kết quả DSS (Hạng 2) | Trạng thái ghi nhận |
| :--- | :--- | :--- | :--- | :--- |
| **Giai đoạn 1 (Gốc)** | Dữ liệu khởi tạo chuẩn | **Samsung Galaxy S24 Ultra**<br>*(Điểm: 0.9102 - Selected)* | **Dell Gaming G15**<br>*(Điểm: 0.7270 - UnderReview)* | Đã lưu vào `DECISION_RESULTS` |
| **Giai đoạn 2 (Biến động)** | Cập nhật trong SQL Server:<br>- Tăng điểm doanh thu/nhu cầu Dell G15<br>- Tăng rủi ro tồn kho S24 Ultra | **Dell Gaming G15**<br>*(Điểm: 0.9119 - Selected)* | **Samsung Galaxy S24 Ultra**<br>*(Điểm: 0.8610 - UnderReview)* | **Kết quả DSS đảo chiều ngay lập tức theo CSDL!** |
| **Giai đoạn 3 (Hoàn nguyên)**| Phục hồi dữ liệu gốc | **Samsung Galaxy S24 Ultra**<br>*(Điểm: 0.9102 - Selected)* | **Dell Gaming G15**<br>*(Điểm: 0.7270 - UnderReview)* | Dữ liệu trở về chuẩn an toàn |

> ✅ **KẾT LUẬN NGHIỆM THU:**  
> Hệ thống hoàn toàn không hard-code dữ liệu. Khi dữ liệu trong SQL Server thay đổi, việc chạy lại hệ thống DSS lập tức phản ánh kết quả xếp hạng và quyết định phê duyệt mới. **Tiêu chuẩn DONE của Task 06 đạt 100%.**
