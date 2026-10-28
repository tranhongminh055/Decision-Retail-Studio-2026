
USE DSS_SalesDB;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

PRINT '====================================================================================';
PRINT '6.1. VIẾT SQL LẤY DỮ LIỆU (TRÍCH XUẤT DATASET PHỤC VỤ DSS)';
PRINT '====================================================================================';

SELECT 
    ds.session_id,
    ds.session_code,
    ds.session_title,
    alt.alternative_id,
    alt.alternative_code,
    alt.alternative_name,
    p.product_code,
    p.cost_price,
    p.selling_price,
    p.stock_quantity,
    c.criterion_id,
    c.criterion_code,
    c.criterion_name,
    c.criterion_type,
    c.data_source_type,
    s.raw_value,
    s.normalized_score
FROM dbo.DECISION_SESSIONS ds
INNER JOIN dbo.ALTERNATIVES alt ON ds.session_id = alt.session_id
LEFT JOIN dbo.PRODUCTS p ON alt.product_id = p.product_id
INNER JOIN dbo.ALTERNATIVE_SCORES s ON alt.alternative_id = s.alternative_id AND ds.session_id = s.session_id
INNER JOIN dbo.CRITERIA c ON s.criterion_id = c.criterion_id
WHERE ds.session_id = 1
ORDER BY alt.alternative_id ASC, c.criterion_id ASC;
GO

PRINT '====================================================================================';
PRINT '6.2. SQL CRUD (CÁC THAO TÁC CREATE - READ - UPDATE - DELETE CỐT LÕI)';
PRINT '====================================================================================';

PRINT '>> [CRUD: CREATE] Chen mot san pham moi...';
IF NOT EXISTS (SELECT 1 FROM dbo.PRODUCTS WHERE product_code = 'PRD_IPHONE_16PM')
BEGIN
    INSERT INTO dbo.PRODUCTS (category_id, product_code, product_name, cost_price, selling_price, stock_quantity, min_stock_level, lead_time_days, is_active)
    VALUES (1, 'PRD_IPHONE_16PM', N'iPhone 16 Pro Max 256GB Desert Titanium', 26500000, 33990000, 30, 8, 3, 1);
    PRINT '-> Da chen thanh cong san pham moi: PRD_IPHONE_16PM';
END;
GO

PRINT '>> [CRUD: READ] Doc thong tin san pham va tinh bien loi nhuan...';
SELECT 
    product_code,
    product_name,
    cost_price,
    selling_price,
    (selling_price - cost_price) AS unit_profit,
    CAST(((selling_price - cost_price) / selling_price) * 100.0 AS DECIMAL(5,2)) AS margin_pct,
    stock_quantity,
    CASE 
        WHEN ((selling_price - cost_price) / selling_price) >= 0.25 THEN N'Biên lợi nhuận cao (Ưu tiên)'
        WHEN ((selling_price - cost_price) / selling_price) >= 0.15 THEN N'Biên lợi nhuận khá'
        ELSE N'Biên lợi nhuận mỏng'
    END AS profit_category
FROM dbo.PRODUCTS
WHERE product_code = 'PRD_IPHONE_16PM';
GO

PRINT '>> [CRUD: UPDATE] Cap nhat gia ban va ton kho...';
UPDATE dbo.PRODUCTS
SET selling_price = 34490000,
    stock_quantity = stock_quantity + 10
WHERE product_code = 'PRD_IPHONE_16PM';
PRINT '-> Da cap nhat gia ban moi va ton kho cho PRD_IPHONE_16PM';
GO

PRINT '>> [CRUD: DELETE] Xoa san pham thu nghiem vua tao...';
DELETE FROM dbo.PRODUCTS
WHERE product_code = 'PRD_IPHONE_16PM';
PRINT '-> Da xoa thanh cong san pham thu nghiem PRD_IPHONE_16PM';
GO

PRINT '====================================================================================';
PRINT '6.3. SQL THỐNG KÊ (AGGREGATION: COUNT, SUM, AVG, MIN, MAX, GROUP BY, HAVING)';
PRINT '====================================================================================';

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
GO

PRINT '====================================================================================';
PRINT '6.4. SQL JOIN (KẾT HỢP NHIỀU BẢNG LIÊN PHÂN HỆ)';
PRINT '====================================================================================';

SELECT 
    ds.session_code AS [Mã Phiên],
    u.full_name AS [Người Chủ Trì],
    alt.alternative_name AS [Phương Án Ứng Viên],
    cat.category_name AS [Ngành Hàng],
    p.cost_price AS [Giá Vốn],
    p.selling_price AS [Giá Niêm Yết],
    p.stock_quantity AS [Tồn Kho Kho],
    crit.criterion_name AS [Tiêu Chí Đánh Giá],
    crit.criterion_type AS [Loại Tiêu Chí],
    alt_score.raw_value AS [Giá Trị Đo Lường],
    alt_score.normalized_score AS [Điểm Chuẩn Hóa AHP]
FROM dbo.DECISION_SESSIONS ds
INNER JOIN dbo.USERS u ON ds.created_by = u.user_id
INNER JOIN dbo.ALTERNATIVES alt ON ds.session_id = alt.session_id
INNER JOIN dbo.PRODUCTS p ON alt.product_id = p.product_id
INNER JOIN dbo.CATEGORIES cat ON p.category_id = cat.category_id
INNER JOIN dbo.ALTERNATIVE_SCORES alt_score ON alt.alternative_id = alt_score.alternative_id AND ds.session_id = alt_score.session_id
INNER JOIN dbo.CRITERIA crit ON alt_score.criterion_id = crit.criterion_id
WHERE ds.session_id = 1
ORDER BY alt.alternative_id, crit.criterion_id;
GO

PRINT '====================================================================================';
PRINT '6.5. SQL PHỤC VỤ DSS (TÍNH ĐIỂM, TÌM PHƯƠNG ÁN TỐT NHẤT, XẾP HẠNG, LỌC PHƯƠNG ÁN)';
PRINT '====================================================================================';

WITH WeightCalculated AS (

    SELECT 
        pc.criterion_id_1 AS criterion_id,
        AVG(pc.saaty_value / col_sums.col_sum) AS criterion_weight
    FROM dbo.PAIRWISE_COMPARISONS pc
    INNER JOIN (
        SELECT criterion_id_2, SUM(saaty_value) AS col_sum
        FROM dbo.PAIRWISE_COMPARISONS
        WHERE session_id = 1
        GROUP BY criterion_id_2
    ) col_sums ON pc.criterion_id_2 = col_sums.criterion_id_2
    WHERE pc.session_id = 1
    GROUP BY pc.criterion_id_1
),
AlternativeCompositeScores AS (

    SELECT 
        s.alternative_id,
        SUM(w.criterion_weight * s.normalized_score) AS composite_ahp_score
    FROM dbo.ALTERNATIVE_SCORES s
    INNER JOIN WeightCalculated w ON s.criterion_id = w.criterion_id
    WHERE s.session_id = 1
    GROUP BY s.alternative_id
)

SELECT 
    DENSE_RANK() OVER (ORDER BY acs.composite_ahp_score DESC) AS [Xếp hạng],
    alt.alternative_code AS [Mã PA],
    alt.alternative_name AS [Tên Phương Án],
    p.selling_price AS [Giá Bán Niêm Yết],
    CAST(acs.composite_ahp_score AS DECIMAL(8,4)) AS [Điểm AHP],
    CASE 
        WHEN DENSE_RANK() OVER (ORDER BY acs.composite_ahp_score DESC) = 1 THEN 'Selected'
        WHEN acs.composite_ahp_score >= 0.60 THEN 'UnderReview'
        ELSE 'Rejected'
    END AS [Quyết Định Đề Xuất],
    CASE 
        WHEN DENSE_RANK() OVER (ORDER BY acs.composite_ahp_score DESC) = 1 
        THEN N'🏆 PHƯƠNG ÁN TỐI ƯU NHẤT - Ưu tiên rót vốn & triển khai'
        WHEN acs.composite_ahp_score >= 0.60 
        THEN N'⚠️ KHẢ QUAN - Cần đánh giá thêm điều kiện rủi ro/ngân sách'
        ELSE N'❌ LOẠI BỎ - Không đạt hiệu quả đa tiêu chí'
    END AS [Khuyến Nghị]
FROM AlternativeCompositeScores acs
INNER JOIN dbo.ALTERNATIVES alt ON acs.alternative_id = alt.alternative_id
LEFT JOIN dbo.PRODUCTS p ON alt.product_id = p.product_id
ORDER BY [Xếp hạng] ASC;
GO

PRINT '>> [DSS QUERY: FILTER] Loc cac phuong an thoa man dong thoi tieu chi DSS va rang buoc kinh doanh:';
SELECT 
    alt.alternative_code AS [Mã PA],
    alt.alternative_name AS [Tên Phương Án],
    dr.final_score AS [Điểm AHP],
    p.stock_quantity AS [Tồn Kho Hiện Tại],
    CAST(((p.selling_price - p.cost_price) / p.selling_price) * 100.0 AS DECIMAL(5,2)) AS [Biên LN (%)]
FROM dbo.DECISION_RESULTS dr
INNER JOIN dbo.ALTERNATIVES alt ON dr.alternative_id = alt.alternative_id
INNER JOIN dbo.PRODUCTS p ON alt.product_id = p.product_id
WHERE dr.session_id = 1
  AND dr.final_score >= 0.70
  AND ((p.selling_price - p.cost_price) / p.selling_price) >= 0.25
  AND p.stock_quantity <= 50;
GO

-- 6.5.3. SO SÁNH ĐỐI ĐẦU GIỮA 2 PHƯƠNG ÁN DẪN ĐẦU (HEAD-TO-HEAD COMPARISON)
PRINT '>> [DSS QUERY: COMPARE] So sanh doi dau chi tiet giua Hang 1 va Hang 2 theo tung tieu chi:';
SELECT 
    c.criterion_name AS [Tiêu Chí],
    c.criterion_type AS [Loại TC],
    MAX(CASE WHEN alt.alternative_code = 'ALT_S24_ULTRA' THEN s.raw_value END) AS [Galaxy S24 (Hạng 1)],
    MAX(CASE WHEN alt.alternative_code = 'ALT_DELL_G15' THEN s.raw_value END)  AS [Dell Gaming G15 (Hạng 2)],
    CASE 
        WHEN MAX(CASE WHEN alt.alternative_code = 'ALT_S24_ULTRA' THEN s.normalized_score END) > 
             MAX(CASE WHEN alt.alternative_code = 'ALT_DELL_G15' THEN s.normalized_score END) 
        THEN N'S24 Ultra tốt hơn'
        WHEN MAX(CASE WHEN alt.alternative_code = 'ALT_S24_ULTRA' THEN s.normalized_score END) < 
             MAX(CASE WHEN alt.alternative_code = 'ALT_DELL_G15' THEN s.normalized_score END) 
        THEN N'Dell G15 tốt hơn'
        ELSE N'Ngang bằng'
    END AS [Kết Quả Đọ Sức]
FROM dbo.ALTERNATIVE_SCORES s
INNER JOIN dbo.ALTERNATIVES alt ON s.alternative_id = alt.alternative_id
INNER JOIN dbo.CRITERIA c ON s.criterion_id = c.criterion_id
WHERE s.session_id = 1 
  AND alt.alternative_code IN ('ALT_S24_ULTRA', 'ALT_DELL_G15')
GROUP BY c.criterion_id, c.criterion_name, c.criterion_type
ORDER BY c.criterion_id;
GO

PRINT '====================================================================================';
PRINT '6.6. TRUY VẤN PHÂN TÍCH ĐA CHIỀU OLAP (ONLINE ANALYTICAL PROCESSING - ROLLUP & CUBE)';
PRINT '====================================================================================';

-- 6.6.1. OLAP ROLLUP: Phân tích thứ bậc đa cấp độ (Hierarchical Rollup) từ Toàn hệ thống -> Ngành hàng -> Mặt hàng
-- Phục vụ: Quyết định Phân bổ Hàng hóa (Inventory Allocation) và Cảnh báo nguy cơ đọng vốn
PRINT '>> [OLAP 1 - ROLLUP]: Phan tich da cap Doanh so & Ton kho phuc vu Phan bo hang hoa:';
SELECT 
    CASE WHEN GROUPING(c.category_name) = 1 THEN N'-- TẤT CẢ NGÀNH HÀNG --' ELSE c.category_name END AS [Ngành Hàng],
    CASE WHEN GROUPING(p.product_name) = 1 THEN N'-- TỔNG CỘNG NHÓM --' ELSE p.product_name END AS [Sản Phẩm],
    SUM(p.stock_quantity) AS [Tổng Tồn Kho Hiện Tại],
    ISNULL(SUM(od.quantity), 0) AS [Tổng Số Lượng Đã Bán],
    FORMAT(ISNULL(SUM(od.line_total), 0), '#,##0') + N' đ' AS [Tổng Doanh Thu],
    FORMAT(ISNULL(SUM(od.line_total - (od.quantity * od.unit_cost)), 0), '#,##0') + N' đ' AS [Lợi Nhuận Gộp],
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
GO

-- 6.6.2. OLAP CUBE: Ma trận đa chiều Phân khúc Khách hàng x Ngành hàng x Tần suất mua
-- Phục vụ: Quyết định Chiến lược Khuyến mãi (Promotion Strategy) trúng đích
PRINT '>> [OLAP 2 - CUBE]: Ma tran da chieu Phan khuc Khach hang x Nganh hang de dinh huong Khuyen mai:';
SELECT 
    ISNULL(cust.customer_segment, N'-- TẤT CẢ PHÂN KHÚC --') AS [Phân Khúc Khách],
    ISNULL(cat.category_name, N'-- TẤT CẢ NGÀNH HÀNG --') AS [Ngành Hàng],
    COUNT(DISTINCT o.order_id) AS [Số Lượng Đơn],
    ISNULL(SUM(od.quantity), 0) AS [Số Lượng Mua],
    FORMAT(ISNULL(SUM(od.line_total), 0), '#,##0') + N' đ' AS [Doanh Thu Phân Khúc],
    CASE 
        WHEN cust.customer_segment = 'VIP' THEN N'Tặng Voucher Tri Ân 10% & Dịch vụ đặc quyền'
        WHEN cust.customer_segment = 'Retail' AND ISNULL(SUM(od.quantity), 0) < 5 THEN N'Áp Dụng Khuyến Mãi Mua 1 Tặng 1 để kích cầu'
        WHEN cust.customer_segment = 'Wholesale' THEN N'Chiết khấu thương mại theo sản lượng lớn'
        ELSE N'Duy Trì Giá Chuẩn'
    END AS [Chiến Lược Khuyến Mãi Gợi Ý]
FROM dbo.CUSTOMERS cust
INNER JOIN dbo.ORDERS o ON cust.customer_id = o.customer_id
INNER JOIN dbo.ORDER_DETAILS od ON o.order_id = od.order_id
INNER JOIN dbo.PRODUCTS p ON od.product_id = p.product_id
INNER JOIN dbo.CATEGORIES cat ON p.category_id = cat.category_id
GROUP BY CUBE(cust.customer_segment, cat.category_name);
GO

PRINT '====================================================================================';
PRINT 'HOÀN TẤT THỰC THI TOÀN BỘ CÁC TRUY VẤN VÀ BÁO CÁO SQL TASK 06 (BAO GỒM OLAP & DSS)!';
PRINT '====================================================================================';
GO
