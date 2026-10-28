-- ====================================================================================
-- ĐỒ ÁN NHÓM IS402 - HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH (DSS)
-- CẬP NHẬT VIEW & STORED PROCEDURE: SỬ DỤNG TÊN CỘT CHUẨN ĐỂ ĐỒNG BỘ 100% VỚI C# GUI
-- ====================================================================================

USE DSS_SalesDB;
GO

SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

-- 1. VIEW HIỆU QUẢ KINH DOANH SẢN PHẨM (DỮ LIỆU ĐẦU VÀO OLAP)
IF OBJECT_ID(N'dbo.vw_DSS_ProductPerformance', N'V') IS NOT NULL DROP VIEW dbo.vw_DSS_ProductPerformance;
GO

CREATE VIEW dbo.vw_DSS_ProductPerformance
AS
SELECT 
    p.product_id,
    p.product_code,
    p.product_name,
    c.category_name,
    p.cost_price,
    p.selling_price,
    p.stock_quantity,
    p.lead_time_days,
    ISNULL(SUM(od.quantity), 0) AS total_sold_quantity,
    ISNULL(SUM(od.line_total), 0) AS total_revenue,
    ISNULL(SUM(od.quantity * od.unit_cost), 0) AS total_cost_of_goods_sold,
    ISNULL(SUM(od.line_total - (od.quantity * od.unit_cost)), 0) AS gross_profit,
    CASE 
        WHEN ISNULL(SUM(od.line_total), 0) > 0 
        THEN CAST((SUM(od.line_total - (od.quantity * od.unit_cost)) / SUM(od.line_total)) * 100.0 AS DECIMAL(8,2))
        ELSE CAST(((p.selling_price - p.cost_price) / p.selling_price) * 100.0 AS DECIMAL(8,2))
    END AS gross_profit_margin_pct,
    CASE 
        WHEN p.stock_quantity > p.min_stock_level * 5 THEN N'Cảnh báo tồn kho quá cao'
        WHEN p.stock_quantity < p.min_stock_level THEN N'Cảnh báo thiếu hàng'
        ELSE N'Tồn kho an toàn'
    END AS stock_health_status
FROM dbo.PRODUCTS p
INNER JOIN dbo.CATEGORIES c ON p.category_id = c.category_id
LEFT JOIN dbo.ORDER_DETAILS od ON p.product_id = od.product_id
LEFT JOIN dbo.ORDERS o ON od.order_id = o.order_id AND o.order_status = 'Completed'
GROUP BY 
    p.product_id, p.product_code, p.product_name, c.category_name, 
    p.cost_price, p.selling_price, p.stock_quantity, p.lead_time_days, p.min_stock_level;
GO

-- 2. VIEW PIVOT MA TRẬN SO SÁNH CẶP AHP (CHUẨN TÊN CỘT)
IF OBJECT_ID(N'dbo.vw_AHP_PairwiseMatrix_Pivot', N'V') IS NOT NULL DROP VIEW dbo.vw_AHP_PairwiseMatrix_Pivot;
GO

CREATE VIEW dbo.vw_AHP_PairwiseMatrix_Pivot
AS
SELECT 
    session_id,
    criterion_row_code AS criterion_code,
    criterion_row_name AS criterion_name,
    ISNULL([C_PROFIT], 1.0)  AS c_profit,
    ISNULL([C_REVENUE], 1.0) AS c_revenue,
    ISNULL([C_DEMAND], 1.0)  AS c_demand,
    ISNULL([C_RISK], 1.0)    AS c_risk,
    ISNULL([C_MKTCOST], 1.0) AS c_mktcost
FROM (
    SELECT 
        pc.session_id,
        c1.criterion_code AS criterion_row_code,
        c1.criterion_name AS criterion_row_name,
        c2.criterion_code AS criterion_col_code,
        pc.saaty_value
    FROM dbo.PAIRWISE_COMPARISONS pc
    INNER JOIN dbo.CRITERIA c1 ON pc.criterion_id_1 = c1.criterion_id
    INNER JOIN dbo.CRITERIA c2 ON pc.criterion_id_2 = c2.criterion_id
) AS SourceTable
PIVOT (
    AVG(saaty_value)
    FOR criterion_col_code IN ([C_PROFIT], [C_REVENUE], [C_DEMAND], [C_RISK], [C_MKTCOST])
) AS PivotTable;
GO

-- 3. VIEW BÁO CÁO XẾP HẠNG TỔNG HỢP (CHUẨN TÊN CỘT)
IF OBJECT_ID(N'dbo.vw_DSS_FinalRankingReport', N'V') IS NOT NULL DROP VIEW dbo.vw_DSS_FinalRankingReport;
GO

CREATE VIEW dbo.vw_DSS_FinalRankingReport
AS
SELECT 
    ds.session_id,
    ds.session_code,
    ds.session_title,
    dr.ranking,
    alt.alternative_code,
    alt.alternative_name,
    p.product_code,
    p.selling_price,
    p.cost_price,
    p.stock_quantity,
    dr.final_score,
    dr.decision_outcome,
    CASE dr.decision_outcome
        WHEN 'Selected' THEN N'ƯU TIÊN HÀNG ĐẦU - Phê duyệt nhập hàng & ngân sách'
        WHEN 'UnderReview' THEN N'XEM XÉT THÊM - Nhập thăm dò số lượng hạn chế'
        WHEN 'Rejected' THEN N'LOẠI BỎ - Không nhập thêm, kích hoạt xả hàng'
    END AS recommendation,
    dr.comments,
    ds.consistency_ratio,
    ds.is_consistent
FROM dbo.DECISION_RESULTS dr
INNER JOIN dbo.DECISION_SESSIONS ds ON dr.session_id = ds.session_id
INNER JOIN dbo.ALTERNATIVES alt ON dr.alternative_id = alt.alternative_id
LEFT JOIN dbo.PRODUCTS p ON alt.product_id = p.product_id;
GO
