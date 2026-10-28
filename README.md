# ĐỒ ÁN IS402 - HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH (DSS)
## BÁO CÁO TIẾN ĐỘ THỰC HIỆN TASK 05 & TASK 06

---

## 📌 TỔNG QUAN ĐỀ TÀI
- **Lĩnh vực ứng dụng:** Bán hàng / Quản lý kinh doanh bán lẻ (Sales Decision Support System).
- **Mô hình hỗ trợ ra quyết định:** **AHP (Analytic Hierarchy Process)** - Phân tích thứ bậc Saaty.
- **Bài toán DSS:** *"Lựa chọn Danh mục Sản phẩm Chiến lược để Phân bổ Ngân sách Nhập hàng & Tiếp thị Quý 4/2026"*.
- **Hệ quản trị CSDL:** Microsoft SQL Server 2022 / 2025 (`DSS_SalesDB`).

---

## 📁 CẤU TRÚC DỰ ÁN TRÊN HỆ THỐNG

```
Decision System/
│
├── DSS_App/                                # PHẦN MỀM HỆ THỐNG C# WINDOWS FORMS (SYSTEM SOFTWARE)
│   ├── Form1.cs                            # Giao diện chính: 4 Tabs (Xếp hạng, Ma trận AHP, OLAP, AI Assistant)
│   ├── Form1.Designer.cs                   # Thành phần Designer của Form
│   ├── Program.cs                          # Entry point của ứng dụng C# .NET
│   └── DSS_App.csproj                      # Cấu hình dự án .NET 10.0 + Microsoft.Data.SqlClient
│
├── docs/                                   # TÀI LIỆU PHÂN TÍCH, THIẾT KẾ & THUYẾT MINH
│   ├── 01_PHAN_TICH_THIET_KE_DATABASE_DSS.md # Chi tiết Task 05 (5.1 -> 5.5, ERD Mermaid, Chuẩn hóa 1NF-3NF)
│   ├── DATA_DICTIONARY.md                  # Từ điển dữ liệu cho 100% các bảng và thuộc tính
│   ├── DATASET_DEMO_AND_TEST_CASES.md      # Chi tiết 4 kịch bản kiểm thử (Tốt, Trung bình, Kém, Edge case)
│   └── 02_XU_LY_DU_LIEU_VA_KET_NOI_DSS.md  # Chi tiết Task 06 (CRUD, Thống kê, JOIN, DSS Queries, Logic Kết nối)
│
├── sql/                                    # MÃ NGUỒN SQL SERVER (Đã test chạy độc lập)
│   ├── 01_create_database.sql              # DDL: CREATE DATABASE, 12 TABLES, PK, FK, CHECK, INDEXES
│   ├── 02_seed_data.sql                    # DML: Dữ liệu bán hàng & Ma trận AHP chuẩn toán học (CR = 0.0152)
│   ├── 03_dss_reports_and_queries.sql      # VIEWs, Stored Procedures tính trọng số & Báo cáo xếp hạng DSS
│   └── 04_data_processing_and_crud.sql     # Script SQL thực hiện 6.1 -> 6.5 (CRUD, Thống kê, JOIN, Phục vụ DSS)
│
├── src/                                    # MÃ NGUỒN DSS ENGINE KẾT NỐI DATABASE (PYTHON)
│   ├── dss_engine.py                       # Pipeline: SQL Server -> SQL Query -> AHP Model -> Ghi kết quả vào DB
│   └── test_dss_pipeline.py                # Script kiểm chứng tiêu chí DONE (Đổi DB -> DSS thay đổi theo)
│
├── CHAY_HE_THONG_GUI_CSHARP.bat            # Phím tắt 1-click khởi chạy phần mềm C# WinForms
└── README.md                               # Tài liệu tổng quan dự án
```

---

## 🎯 BẢNG ĐỐI CHIẾU TIẾN ĐỘ THEO YÊU CẦU ĐỒ ÁN

### TASK 05: THIẾT KẾ DATABASE TRÊN SQL SERVER
| Mục | Nội dung | File thực thi / Tài liệu | Kết quả nghiệm thu |
| :---: | :--- | :--- | :---: |
| **5.1** | Xác định Entity | [01_PHAN_TICH_THIET_KE_DATABASE_DSS.md](file:///g:/Decision%20System/docs/01_PHAN_TICH_THIET_KE_DATABASE_DSS.md#51-xác-định-các-thực-thể-entities) | ✅ 12 thực thể |
| **5.2** | Xác định thuộc tính | [DATA_DICTIONARY.md](file:///g:/Decision%20System/docs/DATA_DICTIONARY.md) | ✅ Đầy đủ kiểu & ràng buộc |
| **5.3** | Xác định PK / FK | [01_PHAN_TICH_THIET_KE_DATABASE_DSS.md](file:///g:/Decision%20System/docs/01_PHAN_TICH_THIET_KE_DATABASE_DSS.md#53-xác-định-khóa-chính-pk-khóa-ngoại-fk-và-bảng-liên-kết) | ✅ Ràng buộc toàn vẹn |
| **5.4** | Vẽ sơ đồ ERD | [01_PHAN_TICH_THIET_KE_DATABASE_DSS.md](file:///g:/Decision%20System/docs/01_PHAN_TICH_THIET_KE_DATABASE_DSS.md#54-sơ-đồ-thực-thể-mối-quan-hệ-erd---mermaid) | ✅ Sơ đồ Mermaid chuẩn |
| **5.5** | Chuẩn hóa CSDL | [01_PHAN_TICH_THIET_KE_DATABASE_DSS.md](file:///g:/Decision%20System/docs/01_PHAN_TICH_THIET_KE_DATABASE_DSS.md#55-chuẩn-hóa-dữ-liệu-database-normalization) | ✅ Đạt chuẩn 1NF $\to$ 3NF |
| **5.6** | Tạo Database thật | [01_create_database.sql](file:///g:/Decision%20System/sql/01_create_database.sql) | ✅ Chạy thành công trên SQL Server |
| **5.7** | Nạp dữ liệu mẫu | [02_seed_data.sql](file:///g:/Decision%20System/sql/02_seed_data.sql) | ✅ 4 test case: Tốt, TB, Kém, Edge |

### TASK 06: XỬ LÝ DỮ LIỆU & KẾT NỐI DATABASE VỚI DSS
| Mục | Nội dung | File thực thi / Tài liệu | Kết quả nghiệm thu |
| :---: | :--- | :--- | :---: |
| **6.1** | SQL lấy dữ liệu | [04_data_processing_and_crud.sql](file:///g:/Decision%20System/sql/04_data_processing_and_crud.sql) | ✅ Dataset đa bảng phục vụ AHP |
| **6.2** | SQL CRUD | [04_data_processing_and_crud.sql](file:///g:/Decision%20System/sql/04_data_processing_and_crud.sql) | ✅ Đầy đủ INSERT, SELECT, UPDATE, DELETE |
| **6.3** | SQL thống kê | [04_data_processing_and_crud.sql](file:///g:/Decision%20System/sql/04_data_processing_and_crud.sql) | ✅ COUNT, SUM, AVG, MIN, MAX, GROUP BY, HAVING |
| **6.4** | SQL JOIN | [04_data_processing_and_crud.sql](file:///g:/Decision%20System/sql/04_data_processing_and_crud.sql) | ✅ Kết hợp 7 bảng liên phân hệ |
| **6.5** | SQL phục vụ DSS | [04_data_processing_and_crud.sql](file:///g:/Decision%20System/sql/04_data_processing_and_crud.sql) | ✅ Tính điểm AHP, DENSE_RANK, Filter, Compare |
| **6.6** | Kết nối DB với DSS | [dss_engine.py](file:///g:/Decision%20System/src/dss_engine.py) & [test_dss_pipeline.py](file:///g:/Decision%20System/src/test_dss_pipeline.py) | ✅ Không hard-code. Đổi DB làm kết quả đổi theo! |

---

## 🚀 HƯỚNG DẪN CHẠY VÀ DEMO NHANH

### 1. Chạy các truy vấn SQL trên SSMS hoặc PowerShell:
```powershell
# Chạy trọn bộ xử lý dữ liệu và truy vấn DSS
sqlcmd -S ".\SQLEXPRESS25" -E -I -i "g:\Decision System\sql\04_data_processing_and_crud.sql"
```

### 2. Chạy Engine tính toán DSS AHP (Python):
```powershell
python "g:\Decision System\src\dss_engine.py"
```

### 3. Chạy kịch bản kiểm chứng tiêu chí DONE:
```powershell
python "g:\Decision System\src\test_dss_pipeline.py"
```
*(Script sẽ tự động: 1. Chạy DSS lần 1 $\to$ 2. Cập nhật dữ liệu thị trường trong SQL Server $\to$ 3. Chạy lại DSS và chứng minh Hạng 1 đảo chiều theo dữ liệu mới $\to$ 4. Hoàn nguyên dữ liệu gốc an toàn).*

### 4. Khởi chạy Phần mềm Windows Forms (.NET C#):
- **Cách 1:** Nhấp đúp chuột vào file `CHAY_HE_THONG_GUI_CSHARP.bat` ngoài thư mục gốc.
- **Cách 2:** Khởi chạy bằng lệnh PowerShell:
```powershell
dotnet run --project "g:\Decision System\DSS_App\DSS_App.csproj"
```

---

## 🔐 HỆ THỐNG ĐĂNG NHẬP, PHÂN QUYỀN & ĐĂNG XUẤT (AUTH & ROLES)

Giao diện đăng nhập (`LoginForm`) được thiết kế đồng bộ với hệ thống DSS:
- **Header:** Tone Navy Deep Blue sang trọng với Tag nhận diện thương hiệu `SPORT ZONE DSS`, tiêu đề hệ thống và phụ đề kiểm soát tồn kho.
- **Form:** Ô nhập **Mã nhân viên** và **Mật khẩu** (có nút bật/tắt hiển thị mật khẩu 👁), hỗ trợ phím **Enter** đăng nhập nhanh và **Esc** để thoát.
- **Phím chọn nhanh tài khoản mẫu:** 1-click điền tự động tài khoản demo.
- **Đăng xuất (Logout):** Nút `🚪 Đăng xuất` màu đỏ đặt trực tiếp tại góc trên bên phải thanh Header của Dashboard, kèm hộp thoại xác nhận an toàn và quay về màn hình đăng nhập.

### 📋 Bảng tài khoản mẫu & Phân quyền:
| Mã NV / Username | Mật khẩu | Họ và Tên | Vai trò (Role) | Quyền hạn trong hệ thống |
| :--- | :---: | :--- | :---: | :--- |
| **`NV000`** *(dm_hoang)* | `123456` | Nguyễn Huy Hoàng | **DecisionMaker** | Toàn quyền thiết lập trọng số Saaty, kích hoạt chạy DSS AHP và phê duyệt nhập hàng / khuyến mãi. |
| **`admin`** | `123456` | Quản trị viên Hệ thống | **Admin** | Toàn quyền quản trị hệ thống, dữ liệu và mô hình quyết định. |
| **`NV002`** *(lead_analyst)*| `123456` | Trần Thị Mai | **SalesManager** | Trưởng phòng PT: Phân tích OLAP Rollup/Cube, thiết lập kịch bản What-If. |
| **`NV003`** *(sales_an)* | `123456` | Lê Văn An | **SalesStaff** | Chuyên viên bán hàng: Tra cứu báo cáo, theo dõi đơn hàng và đề xuất SKU mới. |

