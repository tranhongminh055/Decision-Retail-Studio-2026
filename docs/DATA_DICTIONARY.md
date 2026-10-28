# TỪ ĐIỂN DỮ LIỆU (DATA DICTIONARY)
## HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH (DSS) TRONG BÁN HÀNG
**Tên cơ sở dữ liệu:** `DSS_SalesDB`  
**Hệ quản trị CSDL:** Microsoft SQL Server 2022 / 2025  
**Phương pháp DSS:** AHP (Analytic Hierarchy Process)

---

## MỤC LỤC
1. [Phân hệ Quản lý Bán hàng (Sales Operational Tables)](#1-phân-hệ-quản-lý-bán-hàng)
   - [1.1. Bảng USERS](#11-bảng-users)
   - [1.2. Bảng CATEGORIES](#12-bảng-categories)
   - [1.3. Bảng PRODUCTS](#13-bảng-products)
   - [1.4. Bảng CUSTOMERS](#14-bảng-customers)
   - [1.5. Bảng ORDERS](#15-bảng-orders)
   - [1.6. Bảng ORDER_DETAILS](#16-bảng-order_details)
2. [Phân hệ Hỗ trợ ra Quyết định AHP (DSS Tables)](#2-phân-hệ-hỗ-trợ-ra-quyết-định-ahp)
   - [2.1. Bảng DECISION_SESSIONS](#21-bảng-decision_sessions)
   - [2.2. Bảng CRITERIA](#22-bảng-criteria)
   - [2.3. Bảng ALTERNATIVES](#23-bảng-alternatives)
   - [2.4. Bảng PAIRWISE_COMPARISONS](#24-bảng-pairwise_comparisons)
   - [2.5. Bảng ALTERNATIVE_SCORES](#25-bảng-alternative_scores)
   - [2.6. Bảng DECISION_RESULTS](#26-bảng-decision_results)

---

## 1. PHÂN HỆ QUẢN LÝ BÁN HÀNG

### 1.1. Bảng USERS
- **Mục đích:** Quản lý tài khoản người dùng, phân quyền đăng nhập, xác định người chủ trì phiên quyết định hoặc chuyên gia đánh giá AHP.

| Tên Cột | Kiểu Dữ Liệu | NULL | Khóa | Mặc Định / Ràng Buộc | Diễn Giải Nghiệp Vụ |
| :--- | :--- | :---: | :---: | :--- | :--- |
| `user_id` | INT | NO | PK | IDENTITY(1,1) | Mã định danh người dùng. |
| `username` | VARCHAR(50) | NO | UNIQUE | NOT NULL | Tên tài khoản đăng nhập hệ thống. |
| `password_hash` | VARCHAR(255) | NO | | NOT NULL | Mật khẩu đã được mã hóa băm (SHA256/BCrypt). |
| `full_name` | NVARCHAR(100) | NO | | NOT NULL | Họ và tên đầy đủ của người dùng. |
| `email` | VARCHAR(100) | NO | UNIQUE | NOT NULL | Địa chỉ thư điện tử liên lạc. |
| `phone_number` | VARCHAR(20) | YES | | NULL | Số điện thoại cá nhân. |
| `role` | VARCHAR(20) | NO | | CHECK in ('Admin', 'DecisionMaker', 'SalesManager', 'SalesStaff') | Vai trò người dùng trong hệ thống bán hàng và DSS. |
| `status` | BIT | NO | | DEFAULT 1 | Trạng thái hoạt động (1 = Hoạt động, 0 = Bị khóa). |
| `created_at` | DATETIME2 | NO | | DEFAULT SYSDATETIME() | Thời điểm tạo tài khoản. |

---

### 1.2. Bảng CATEGORIES
- **Mục đích:** Phân loại danh mục ngành hàng kinh doanh (Laptop, Điện thoại, Phụ kiện...).

| Tên Cột | Kiểu Dữ Liệu | NULL | Khóa | Mặc Định / Ràng Buộc | Diễn Giải Nghiệp Vụ |
| :--- | :--- | :---: | :---: | :--- | :--- |
| `category_id` | INT | NO | PK | IDENTITY(1,1) | Mã định danh danh mục. |
| `category_code` | VARCHAR(30) | NO | UNIQUE | NOT NULL | Mã viết tắt danh mục (vd: CAT_LAPTOP, CAT_PHONE). |
| `category_name` | NVARCHAR(100) | NO | | NOT NULL | Tên hiển thị ngành hàng. |
| `description` | NVARCHAR(500) | YES | | NULL | Mô tả đặc tính ngành hàng. |
| `status` | BIT | NO | | DEFAULT 1 | 1: Đang kinh doanh, 0: Ngừng kinh doanh. |

---

### 1.3. Bảng PRODUCTS
- **Mục đích:** Lưu trữ danh mục hàng hóa, giá vốn, giá bán, tồn kho. Đóng vai trò là nguồn dữ liệu ứng viên cho bài toán DSS lựa chọn sản phẩm chiến lược.

| Tên Cột | Kiểu Dữ Liệu | NULL | Khóa | Mặc Định / Ràng Buộc | Diễn Giải Nghiệp Vụ |
| :--- | :--- | :---: | :---: | :--- | :--- |
| `product_id` | INT | NO | PK | IDENTITY(1,1) | Mã định danh sản phẩm. |
| `category_id` | INT | NO | FK | REFERENCES CATEGORIES(category_id) | Danh mục hàng hóa thuộc về. |
| `product_code` | VARCHAR(50) | NO | UNIQUE | NOT NULL | Mã SKU sản phẩm. |
| `product_name` | NVARCHAR(200) | NO | | NOT NULL | Tên thương mại đầy đủ của sản phẩm. |
| `cost_price` | DECIMAL(18,2) | NO | | CHECK (cost_price >= 0) | Giá nhập / Giá vốn đơn vị (VNĐ). |
| `selling_price` | DECIMAL(18,2) | NO | | CHECK (selling_price >= cost_price) | Giá niêm yết bán lẻ ra thị trường (VNĐ). |
| `stock_quantity` | INT | NO | | DEFAULT 0, CHECK (stock_quantity >= 0) | Số lượng hàng tồn kho thực tế. |
| `min_stock_level`| INT | NO | | DEFAULT 10 | Ngưỡng cảnh báo tồn kho an toàn. |
| `lead_time_days` | INT | NO | | DEFAULT 7 | Thời gian đặt hàng từ nhà cung cấp (ngày). |
| `is_active` | BIT | NO | | DEFAULT 1 | Trạng thái sản phẩm (1: Hoạt động, 0: Tạm ngừng). |
| `created_at` | DATETIME2 | NO | | DEFAULT SYSDATETIME() | Ngày thêm sản phẩm vào hệ thống. |

---

### 1.4. Bảng CUSTOMERS
- **Mục đích:** Quản lý thông tin khách hàng và phân khúc thị trường để phục vụ đánh giá hành vi mua hàng.

| Tên Cột | Kiểu Dữ Liệu | NULL | Khóa | Mặc Định / Ràng Buộc | Diễn Giải Nghiệp Vụ |
| :--- | :--- | :---: | :---: | :--- | :--- |
| `customer_id` | INT | NO | PK | IDENTITY(1,1) | Mã định danh khách hàng. |
| `customer_code` | VARCHAR(30) | NO | UNIQUE | NOT NULL | Mã khách hàng (vd: CUST001). |
| `customer_name` | NVARCHAR(150) | NO | | NOT NULL | Tên cá nhân hoặc tổ chức mua hàng. |
| `phone` | VARCHAR(20) | YES | UNIQUE | NULL | Số điện thoại liên lạc. |
| `email` | VARCHAR(100) | YES | | NULL | Email liên hệ. |
| `address` | NVARCHAR(255) | YES | | NULL | Địa chỉ nhận hàng. |
| `customer_segment` | VARCHAR(20) | NO | | CHECK in ('Retail', 'Wholesale', 'VIP') | Phân khúc khách hàng (Bán lẻ, Bán buôn, Khách VIP). |
| `total_spent` | DECIMAL(18,2) | NO | | DEFAULT 0 | Tổng giá trị chi tiêu lũy kế (VNĐ). |

---

### 1.5. Bảng ORDERS
- **Mục đích:** Quản lý hóa đơn bán hàng tổng quát.

| Tên Cột | Kiểu Dữ Liệu | NULL | Khóa | Mặc Định / Ràng Buộc | Diễn Giải Nghiệp Vụ |
| :--- | :--- | :---: | :---: | :--- | :--- |
| `order_id` | INT | NO | PK | IDENTITY(1,1) | Mã số đơn hàng. |
| `order_code` | VARCHAR(50) | NO | UNIQUE | NOT NULL | Số hiệu hóa đơn chứng từ (vd: ORD-2026-0001). |
| `customer_id` | INT | NO | FK | REFERENCES CUSTOMERS(customer_id) | Khách hàng mua đơn hàng này. |
| `user_id` | INT | NO | FK | REFERENCES USERS(user_id) | Nhân viên kinh doanh phụ trách chốt đơn. |
| `order_date` | DATETIME2 | NO | | DEFAULT SYSDATETIME() | Ngày giờ lập hóa đơn. |
| `order_status` | VARCHAR(20) | NO | | CHECK in ('Pending', 'Completed', 'Cancelled') | Trạng thái xử lý hóa đơn. |
| `total_amount` | DECIMAL(18,2) | NO | | DEFAULT 0, CHECK (total_amount >= 0) | Tổng tiền hàng trước chiết khấu (VNĐ). |
| `discount_amount`| DECIMAL(18,2) | NO | | DEFAULT 0, CHECK (discount_amount >= 0)| Tiền giảm giá / voucher khuyến mãi (VNĐ). |
| `final_amount` | DECIMAL(18,2) | NO | | DEFAULT 0, CHECK (final_amount >= 0) | Tiền thực tế khách phải thanh toán (VNĐ). |

---

### 1.6. Bảng ORDER_DETAILS
- **Mục đích:** Dòng chi tiết từng sản phẩm trong hóa đơn; là nguồn trích xuất dữ liệu doanh thu thực và lợi nhuận gộp cho DSS.

| Tên Cột | Kiểu Dữ Liệu | NULL | Khóa | Mặc Định / Ràng Buộc | Diễn Giải Nghiệp Vụ |
| :--- | :--- | :---: | :---: | :--- | :--- |
| `order_detail_id`| INT | NO | PK | IDENTITY(1,1) | Mã định danh dòng chi tiết. |
| `order_id` | INT | NO | FK | REFERENCES ORDERS(order_id) ON DELETE CASCADE | Thuộc đơn hàng nào. |
| `product_id` | INT | NO | FK | REFERENCES PRODUCTS(product_id) | Sản phẩm bán ra. |
| `quantity` | INT | NO | | CHECK (quantity > 0) | Số lượng bán ra trong đơn. |
| `unit_price` | DECIMAL(18,2) | NO | | CHECK (unit_price >= 0) | Đơn giá bán tại thời điểm lập đơn. |
| `unit_cost` | DECIMAL(18,2) | NO | | CHECK (unit_cost >= 0) | Giá vốn tại thời điểm lập đơn (để tính lãi gộp). |
| `discount_rate` | DECIMAL(5,2) | NO | | DEFAULT 0, CHECK (0 <= rate <= 100) | Tỷ lệ chiết khấu cho dòng sản phẩm (%). |
| `line_total` | DECIMAL(18,2) | NO | | AS ((quantity * unit_price) * (1 - discount_rate/100.0)) PERSISTED | Thành tiền tính toán tự động lưu trữ trên đĩa. |

---

## 2. PHÂN HỆ HỖ TRỢ RA QUYẾT ĐỊNH AHP (DSS TABLES)

### 2.1. Bảng DECISION_SESSIONS
- **Mục đích:** Quản lý từng phiên ra quyết định của hội đồng quản trị / ban giám đốc kinh doanh.

| Tên Cột | Kiểu Dữ Liệu | NULL | Khóa | Mặc Định / Ràng Buộc | Diễn Giải Nghiệp Vụ |
| :--- | :--- | :---: | :---: | :--- | :--- |
| `session_id` | INT | NO | PK | IDENTITY(1,1) | Mã định danh phiên quyết định. |
| `session_code` | VARCHAR(50) | NO | UNIQUE | NOT NULL | Mã phiên (vd: DSS-2026-Q4-PROD). |
| `session_title` | NVARCHAR(200) | NO | | NOT NULL | Tiêu đề phiên ra quyết định. |
| `description` | NVARCHAR(MAX) | YES | | NULL | Mô tả chi tiết bối cảnh, mục tiêu và ràng buộc. |
| `created_by` | INT | NO | FK | REFERENCES USERS(user_id) | Người khởi tạo phiên (Decision Maker). |
| `start_date` | DATETIME2 | NO | | DEFAULT SYSDATETIME() | Thời điểm mở phiên thảo luận. |
| `end_date` | DATETIME2 | YES | | NULL | Thời điểm đóng phiên / ban hành quyết định. |
| `consistency_ratio` | DECIMAL(8,4) | YES | | NULL | Tỷ số nhất quán $CR$ tính toán từ ma trận AHP. |
| `is_consistent` | INT | NO | | AS (CASE WHEN consistency_ratio IS NOT NULL AND consistency_ratio <= 0.10 THEN 1 ELSE 0 END) PERSISTED | Cờ kiểm định độ tin cậy ($CR \le 0.10$ đạt chuẩn Saaty). |
| `status` | VARCHAR(20) | NO | | DEFAULT 'Draft', CHECK in ('Draft', 'Calculated', 'Approved', 'Rejected') | Trạng thái phê duyệt của phiên quyết định. |

---

### 2.2. Bảng CRITERIA
- **Mục đích:** Cây tiêu chí đánh giá AHP (hỗ trợ phân cấp cha-con đa tầng).

| Tên Cột | Kiểu Dữ Liệu | NULL | Khóa | Mặc Định / Ràng Buộc | Diễn Giải Nghiệp Vụ |
| :--- | :--- | :---: | :---: | :--- | :--- |
| `criterion_id` | INT | NO | PK | IDENTITY(1,1) | Mã định danh tiêu chí. |
| `parent_id` | INT | YES | FK | REFERENCES CRITERIA(criterion_id) | Khóa ngoại tự tham chiếu (nếu là tiêu chí con). |
| `criterion_code` | VARCHAR(30) | NO | UNIQUE | NOT NULL | Ký hiệu tiêu chí (vd: C_PROFIT, C_REVENUE, C_RISK). |
| `criterion_name` | NVARCHAR(150) | NO | | NOT NULL | Tên tiêu chí hiển thị. |
| `description` | NVARCHAR(500) | YES | | NULL | Giải thích định nghĩa và cách đo lường. |
| `criterion_type` | VARCHAR(10) | NO | | CHECK in ('Benefit', 'Cost') | 'Benefit': Càng cao càng tốt; 'Cost': Càng thấp càng tốt. |
| `data_source_type` | VARCHAR(30) | NO | | CHECK in ('Calculated_OLTP', 'Expert_Survey', 'Market_Data') | Cơ chế thu thập dữ liệu tiêu chí. |
| `is_active` | BIT | NO | | DEFAULT 1 | 1: Tiêu chí đang được sử dụng. |

---

### 2.3. Bảng ALTERNATIVES
- **Mục đích:** Danh sách các phương án ứng viên cạnh tranh trong phiên quyết định (Sản phẩm kinh doanh hoặc Chiến lược bán hàng).

| Tên Cột | Kiểu Dữ Liệu | NULL | Khóa | Mặc Định / Ràng Buộc | Diễn Giải Nghiệp Vụ |
| :--- | :--- | :---: | :---: | :--- | :--- |
| `alternative_id` | INT | NO | PK | IDENTITY(1,1) | Mã phương án. |
| `session_id` | INT | NO | FK | REFERENCES DECISION_SESSIONS(session_id) ON DELETE CASCADE | Thuộc phiên quyết định nào. |
| `product_id` | INT | YES | FK | REFERENCES PRODUCTS(product_id) | Liên kết tới sản phẩm trong kho (nếu có). |
| `alternative_code`| VARCHAR(50) | NO | | NOT NULL | Mã phương án (vd: ALT_01, ALT_02). |
| `alternative_name`| NVARCHAR(200) | NO | | NOT NULL | Tên phương án đề xuất. |
| `description` | NVARCHAR(1000) | YES | | NULL | Thuyết minh tóm tắt phương án. |
| `notes` | NVARCHAR(500) | YES | | NULL | Ghi chú của hội đồng thẩm định. |

---

### 2.4. Bảng PAIRWISE_COMPARISONS
- **Mục đích:** Lưu trữ giá trị so sánh cặp giữa 2 tiêu chí ($a_{ij}$) theo thang đo Saaty 1-9 để tính trọng số $w_i$.

| Tên Cột | Kiểu Dữ Liệu | NULL | Khóa | Mặc Định / Ràng Buộc | Diễn Giải Nghiệp Vụ |
| :--- | :--- | :---: | :---: | :--- | :--- |
| `comparison_id` | INT | NO | PK | IDENTITY(1,1) | Mã định danh phép so sánh. |
| `session_id` | INT | NO | FK | REFERENCES DECISION_SESSIONS(session_id) ON DELETE CASCADE | Thuộc phiên quyết định. |
| `criterion_id_1` | INT | NO | FK | REFERENCES CRITERIA(criterion_id) | Tiêu chí dòng ($i$). |
| `criterion_id_2` | INT | NO | FK | REFERENCES CRITERIA(criterion_id) | Tiêu chí cột ($j$). |
| `saaty_value` | DECIMAL(10,5) | NO | | CHECK (saaty_value > 0) | Điểm so sánh Saaty ($a_{ij}$). Ví dụ: 1, 3, 5, 7, 9 hoặc nghịch đảo 0.33333, 0.2, v.v. |
| `evaluation_notes`| NVARCHAR(255) | YES | | NULL | Ghi chú căn cứ so sánh của chuyên gia. |
| `evaluated_by` | INT | NO | FK | REFERENCES USERS(user_id) | Chuyên gia thực hiện so sánh. |
| *Ràng buộc* | UNIQUE(session_id, criterion_id_1, criterion_id_2) | | | Đảm bảo mỗi cặp chỉ so sánh 1 lần trong 1 phiên. |

---

### 2.5. Bảng ALTERNATIVE_SCORES
- **Mục đích:** Lưu điểm đo lường của từng phương án theo từng tiêu chí trong phiên làm việc.

| Tên Cột | Kiểu Dữ Liệu | NULL | Khóa | Mặc Định / Ràng Buộc | Diễn Giải Nghiệp Vụ |
| :--- | :--- | :---: | :---: | :--- | :--- |
| `score_id` | INT | NO | PK | IDENTITY(1,1) | Mã dòng điểm. |
| `session_id` | INT | NO | FK | REFERENCES DECISION_SESSIONS(session_id) ON DELETE CASCADE | Thuộc phiên quyết định. |
| `alternative_id` | INT | NO | FK | REFERENCES ALTERNATIVES(alternative_id) | Phương án được đánh giá. |
| `criterion_id` | INT | NO | FK | REFERENCES CRITERIA(criterion_id) | Theo tiêu chí nào. |
| `raw_value` | DECIMAL(18,4) | NO | | NOT NULL | Giá trị thực tế đo lường (triệu đồng, số ngày tồn, tỷ lệ %). |
| `normalized_score`| DECIMAL(10,6) | NO | | CHECK (normalized_score BETWEEN 0 AND 1) | Điểm chuẩn hóa AHP (nằm trong đoạn $[0, 1]$). |
| *Ràng buộc* | UNIQUE(session_id, alternative_id, criterion_id) | | | Mỗi phương án chỉ có 1 điểm theo mỗi tiêu chí trong phiên. |

---

### 2.6. Bảng DECISION_RESULTS
- **Mục đích:** Lưu kết quả tổng hợp cuối cùng của mô hình AHP, thứ hạng và quyết định phê duyệt.

| Tên Cột | Kiểu Dữ Liệu | NULL | Khóa | Mặc Định / Ràng Buộc | Diễn Giải Nghiệp Vụ |
| :--- | :--- | :---: | :---: | :--- | :--- |
| `result_id` | INT | NO | PK | IDENTITY(1,1) | Mã định danh kết quả. |
| `session_id` | INT | NO | FK | REFERENCES DECISION_SESSIONS(session_id) ON DELETE CASCADE | Thuộc phiên quyết định. |
| `alternative_id` | INT | NO | FK | REFERENCES ALTERNATIVES(alternative_id) | Phương án được xếp hạng. |
| `final_score` | DECIMAL(10,6) | NO | | CHECK (final_score BETWEEN 0 AND 1) | Điểm tổng hợp AHP: $S_j = \sum (w_i \times s_{ij})$. |
| `ranking` | INT | NO | | CHECK (ranking > 0) | Thứ hạng ưu tiên (Hạng 1 là tối ưu nhất). |
| `decision_outcome`| VARCHAR(30) | NO | | CHECK in ('Selected', 'UnderReview', 'Rejected') | Phê duyệt: 'Selected' (Được chọn), 'UnderReview' (Xem xét thêm), 'Rejected' (Bị loại). |
| `decision_date` | DATETIME2 | NO | | DEFAULT SYSDATETIME() | Ngày ghi nhận quyết định. |
| `comments` | NVARCHAR(500) | YES | | NULL | Ý kiến kết luận của ban giám đốc. |
| *Ràng buộc* | UNIQUE(session_id, alternative_id) | | | Mỗi phương án có 1 kết quả duy nhất trong 1 phiên. |
