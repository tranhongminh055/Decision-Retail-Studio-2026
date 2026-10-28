
USE DSS_SalesDB;
GO

SET NOCOUNT ON;
GO

PRINT '>> Dang nap du lieu USERS...';
INSERT INTO dbo.USERS (username, password_hash, full_name, email, phone_number, role, status)
VALUES
('admin',        'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', N'Quản trị viên Hệ thống',    'admin@dss-retail.vn',    '0901234567', 'Admin',         1),
('dm_hoang',     'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', N'Nguyễn Huy Hoàng (GĐKD)',    'hoang.nh@dss-retail.vn', '0912345678', 'DecisionMaker', 1),
('lead_analyst', 'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', N'Trần Thị Mai (Trưởng PT)',  'mai.tt@dss-retail.vn',   '0923456789', 'SalesManager',  1),
('sales_an',     'e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991b7852b855', N'Lê Văn An (Chuyên viên)',    'an.lv@dss-retail.vn',    '0934567890', 'SalesStaff',    1);
GO

PRINT '>> Dang nap du lieu CATEGORIES...';
INSERT INTO dbo.CATEGORIES (category_code, category_name, description, status)
VALUES
('CAT_PHONE',  N'Điện thoại thông minh', N'Các dòng smartphone cao cấp, cận cao cấp và phổ thông', 1),
('CAT_LAPTOP', N'Máy tính xách tay',     N'Laptop văn phòng, Gaming, Đồ họa chuyên nghiệp',          1),
('CAT_WEAR',   N'Thiết bị đeo thông minh', N'Đồng hồ smartwatch, vòng đeo tay theo dõi sức khỏe',    1),
('CAT_ACC',    N'Phụ kiện kỹ thuật số',  N'Loa Bluetooth, tai nghe không dây, củ cáp sạc',          1);
GO

PRINT '>> Dang nap du lieu PRODUCTS...';
INSERT INTO dbo.PRODUCTS (category_id, product_code, product_name, cost_price, selling_price, stock_quantity, min_stock_level, lead_time_days, is_active)
VALUES
(1, 'PRD_S24_ULTRA', N'Samsung Galaxy S24 Ultra 256GB', 21000000, 28490000,  45, 10,  5, 1),
(2, 'PRD_DELL_G15',  N'Dell Gaming G15 5530 i7-13650HX', 18500000, 24990000,  20,  5, 14, 1),
(3, 'PRD_AW_ULTRA2', N'Apple Watch Ultra 2 GPS + Cellular', 15000000, 21490000, 12,  5, 10, 1),
(4, 'PRD_SPK_BT01',  N'Loa Bluetooth Mini Bass BT-01',     250000,   349000, 250, 20, 30, 1),
(1, 'PRD_IPAD_10',   N'iPad Gen 10 10.9 inch Wifi 64GB',  7900000, 10490000,  35, 10,  7, 1);
GO

PRINT '>> Dang nap du lieu CUSTOMERS...';
INSERT INTO dbo.CUSTOMERS (customer_code, customer_name, phone, email, address, customer_segment, total_spent)
VALUES
('CUST001', N'Công ty TNHH Công Nghệ Á Châu', '0988111222', 'contact@asiatech.vn', N'Quận 1, TP. Hồ Chí Minh', 'Wholesale', 320000000),
('CUST002', N'Bùi Quốc Thái (VIP)',           '0977222333', 'thai.bq@vip.com',      N'Ba Đình, Hà Nội',          'VIP',        85000000),
('CUST003', N'Nguyễn Thu Hương',              '0966333444', 'huong.nt@gmail.com',    N'Hải Châu, Đà Nẵng',        'Retail',     12500000),
('CUST004', N'Phạm Minh Quân',                '0955444555', 'quan.pm@gmail.com',     N'Ninh Kiều, Cần Thơ',       'Retail',      7800000);
GO

PRINT '>> Dang nap du lieu ORDERS va ORDER_DETAILS...';
INSERT INTO dbo.ORDERS (order_code, customer_id, user_id, order_date, order_status, total_amount, discount_amount, final_amount)
VALUES
('ORD-2026-001', 1, 4, '2026-09-05 10:15:00', 'Completed', 142450000, 4245000, 138205000),
('ORD-2026-002', 2, 4, '2026-09-12 14:30:00', 'Completed',  53480000, 1480000,  52000000),
('ORD-2026-003', 3, 4, '2026-09-18 16:45:00', 'Completed',  24990000,       0,  24990000),
('ORD-2026-004', 4, 4, '2026-09-22 09:20:00', 'Completed',   1745000,   45000,   1700000);

INSERT INTO dbo.ORDER_DETAILS (order_id, product_id, quantity, unit_price, unit_cost, discount_rate)
VALUES
(1, 1, 4, 28490000, 21000000, 3.0),
(1, 5, 2, 10490000,  7900000, 2.0);
INSERT INTO dbo.ORDER_DETAILS (order_id, product_id, quantity, unit_price, unit_cost, discount_rate)
VALUES
(2, 1, 1, 28490000, 21000000, 2.0),
(2, 3, 1, 21490000, 15000000, 3.0);

INSERT INTO dbo.ORDER_DETAILS (order_id, product_id, quantity, unit_price, unit_cost, discount_rate)
VALUES
(3, 2, 1, 24990000, 18500000, 0.0);
INSERT INTO dbo.ORDER_DETAILS (order_id, product_id, quantity, unit_price, unit_cost, discount_rate)
VALUES
(4, 4, 5,   349000,   250000, 2.5);
GO

PRINT '>> Dang nap du lieu CRITERIA...';
INSERT INTO dbo.CRITERIA (criterion_code, criterion_name, description, criterion_type, data_source_type, is_active)
VALUES
('C_PROFIT',  N'Biên lợi nhuận gộp',             N'Tỷ lệ lợi nhuận trên doanh thu (Selling Price - Cost Price) / Selling Price', 'Benefit', 'Calculated_OLTP', 1),
('C_REVENUE', N'Doanh thu kỳ vọng',              N'Quy mô doanh thu tiềm năng mang lại cho công ty trong quý',                   'Benefit', 'Calculated_OLTP', 1),
('C_DEMAND',  N'Nhu cầu & Xu hướng thị trường',  N'Sức hút thị trường, độ quan tâm tìm kiếm của người tiêu dùng',                 'Benefit', 'Market_Data',     1),
('C_RISK',    N'Rủi ro tồn kho & Thời gian vốn', N'Nguy cơ ứ đọng vốn, tốc độ mất giá sản phẩm công nghệ (Càng cao càng rủi ro)', 'Cost', 'Calculated_OLTP', 1),
('C_MKTCOST', N'Chi phí tiếp thị & Khuyến mãi',  N'Chi phí quảng cáo (Ads, KOL, Voucher) để thúc đẩy bán hàng',                  'Cost', 'Expert_Survey',   1);
GO

PRINT '>> Dang nap du lieu DECISION_SESSIONS...';
INSERT INTO dbo.DECISION_SESSIONS (session_code, session_title, description, created_by, start_date, end_date, consistency_ratio, status)
VALUES
('DSS-2026-Q4-PROD', 
 N'Lựa chọn Sản phẩm Chiến lược Đẩy mạnh Doanh số Quý 4/2026', 
 N'Hội đồng quản trị và Ban Giám đốc Kinh doanh họp thẩm định lựa chọn danh mục sản phẩm ưu tiên nhập hàng và phân bổ ngân sách tiếp thị dịp Tết 2026 bằng phương pháp AHP.',
 2, 
 '2026-10-20 09:00:00', 
 '2026-10-24 17:30:00', 
 NULL, 
 'Draft');
GO

PRINT '>> Dang nap du lieu ALTERNATIVES (4 test cases)...';
INSERT INTO dbo.ALTERNATIVES (session_id, product_id, alternative_code, alternative_name, description, notes)
VALUES
(1, 1, 'ALT_S24_ULTRA', N'Samsung Galaxy S24 Ultra (Case TỐT)',        N'Sản phẩm flagship cao cấp, doanh số lớn, lợi nhuận cao, nhu cầu mạnh',    N'Ứng viên số 1 cho chiến dịch Quý 4'),
(1, 2, 'ALT_DELL_G15',  N'Dell Gaming G15 (Case TRUNG BÌNH KHÁ)',       N'Dòng laptop gaming hiệu năng cao, doanh thu đơn vị lớn, cạnh tranh trung bình', N'Tiềm năng cho mùa tựu trường và Tết'),
(1, 3, 'ALT_AW_ULTRA2', N'Apple Watch Ultra 2 (Case EDGE CASE - NICHE)', N'Biên lợi nhuận cực cao nhưng tệp khách hàng hẹp, chi phí marketing lớn',  N'Cần cân nhắc rủi ro tồn kho'),
(1, 4, 'ALT_SPK_BT01',  N'Loa Bluetooth Mini BT-01 (Case KÉM - LOẠI)',  N'Phụ kiện giá rẻ, lợi nhuận gộp thấp, tồn kho lâu, doanh số đóng góp nhỏ',  N'Đề xuất xả hàng tồn, không nhập thêm');
GO

PRINT '>> Dang nap du lieu PAIRWISE_COMPARISONS...';
INSERT INTO dbo.PAIRWISE_COMPARISONS (session_id, criterion_id_1, criterion_id_2, saaty_value, evaluation_notes, evaluated_by)
VALUES

(1, 1, 1, 1.00000, N'Tự so sánh chính nó (Bằng nhau)', 3),
(1, 1, 2, 2.00000, N'Lợi nhuận quan trọng hơn doanh thu một chút', 3),
(1, 1, 3, 3.00000, N'Lợi nhuận quan trọng hơn nhu cầu thị trường', 3),
(1, 1, 4, 4.00000, N'Lợi nhuận quan trọng hơn kiểm soát rủi ro', 3),
(1, 1, 5, 5.00000, N'Lợi nhuận quan trọng hơn chi phí marketing đáng kể', 3),

(1, 2, 1, 0.50000, N'Nghịch đảo của (C1 vs C2)', 3),
(1, 2, 2, 1.00000, N'Tự so sánh chính nó', 3),
(1, 2, 3, 2.00000, N'Doanh thu quan trọng hơn xu hướng thị trường một chút', 3),
(1, 2, 4, 3.00000, N'Doanh thu quan trọng hơn rủi ro tồn kho', 3),
(1, 2, 5, 4.00000, N'Doanh thu quan trọng hơn chi phí tiếp thị', 3),

(1, 3, 1, 0.33333, N'Nghịch đảo của (C1 vs C3)', 3),
(1, 3, 2, 0.50000, N'Nghịch đảo của (C2 vs C3)', 3),
(1, 3, 3, 1.00000, N'Tự so sánh chính nó', 3),
(1, 3, 4, 2.00000, N'Xu hướng nhu cầu quan trọng hơn rủi ro tồn kho một chút', 3),
(1, 3, 5, 3.00000, N'Xu hướng nhu cầu quan trọng hơn chi phí marketing', 3),

(1, 4, 1, 0.25000, N'Nghịch đảo của (C1 vs C4)', 3),
(1, 4, 2, 0.33333, N'Nghịch đảo của (C2 vs C4)', 3),
(1, 4, 3, 0.50000, N'Nghịch đảo của (C3 vs C4)', 3),
(1, 4, 4, 1.00000, N'Tự so sánh chính nó', 3),
(1, 4, 5, 2.00000, N'Kiểm soát rủi ro quan trọng hơn chi phí tiếp thị một chút', 3),

(1, 5, 1, 0.20000, N'Nghịch đảo của (C1 vs C5)', 3),
(1, 5, 2, 0.25000, N'Nghịch đảo của (C2 vs C5)', 3),
(1, 5, 3, 0.33333, N'Nghịch đảo của (C3 vs C5)', 3),
(1, 5, 4, 0.50000, N'Nghịch đảo của (C4 vs C5)', 3),
(1, 5, 5, 1.00000, N'Tự so sánh chính nó', 3);
GO

PRINT '>> Dang nap du lieu ALTERNATIVE_SCORES...';
INSERT INTO dbo.ALTERNATIVE_SCORES (session_id, alternative_id, criterion_id, raw_value, normalized_score) VALUES
(1, 1, 1, 26.29, 0.920000), 
(1, 1, 2, 1200.0, 0.950000), 
(1, 1, 3, 90.00, 0.900000), 
(1, 1, 4, 15.00, 0.850000), 
(1, 1, 5, 45.00, 0.800000); 

INSERT INTO dbo.ALTERNATIVE_SCORES (session_id, alternative_id, criterion_id, raw_value, normalized_score) VALUES
(1, 2, 1, 25.97, 0.820000), 
(1, 2, 2, 600.00, 0.650000), 
(1, 2, 3, 70.00, 0.700000), 
(1, 2, 4, 30.00, 0.600000), 
(1, 2, 5, 30.00, 0.700000); 

INSERT INTO dbo.ALTERNATIVE_SCORES (session_id, alternative_id, criterion_id, raw_value, normalized_score) VALUES
(1, 3, 1, 30.25, 0.980000), 
(1, 3, 2, 250.00, 0.350000), 
(1, 3, 3, 50.00, 0.500000), 
(1, 3, 4, 45.00, 0.400000), 
(1, 3, 5, 50.00, 0.450000); 

INSERT INTO dbo.ALTERNATIVE_SCORES (session_id, alternative_id, criterion_id, raw_value, normalized_score) VALUES
(1, 4, 1, 28.37, 0.400000), 
(1, 4, 2,  50.00, 0.150000), 
(1, 4, 3,  35.00, 0.350000), 
(1, 4, 4,  90.00, 0.200000), 
(1, 4, 5,  15.00, 0.500000); 
GO

PRINT '>> Bang DECISION_RESULTS de trong - se duoc tinh toan dong khi nguoi dung chay mo hinh DSS.';
GO

PRINT '>> Kich ban DML Seed Data da nap thanh cong vao [DSS_SalesDB]!';
GO
