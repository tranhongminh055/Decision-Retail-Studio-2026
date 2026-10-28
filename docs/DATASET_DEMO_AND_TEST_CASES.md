# TÀI LIỆU KỊCH BẢN DEMO & TẬP DỮ LIỆU KIỂM THỬ (TEST DATASET)
## ĐỒ ÁN IS402 - HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH (DSS)
**Chủ đề:** Ra quyết định lựa chọn danh mục sản phẩm chiến lược trong kinh doanh bán hàng  
**Mô hình áp dụng:** Analytic Hierarchy Process (AHP - Saaty)  
**Cơ sở dữ liệu:** `DSS_SalesDB` trên Microsoft SQL Server

---

## 1. MỤC TIÊU VÀ BỐI CẢNH BÀI TOÁN RA QUYẾT ĐỊNH
Vào mỗi kỳ kinh doanh (ví dụ: Quý 4 - mùa cao điểm mua sắm cuối năm và Tết), Ban Giám đốc và Trưởng phòng Kinh doanh phải đối mặt với bài toán tối ưu:
> *"Trong nguồn vốn và ngân sách tiếp thị có hạn, doanh nghiệp nên ưu tiên nhập hàng và phân bổ ngân sách marketing cho sản phẩm nào để đạt lợi nhuận và doanh thu cao nhất, đồng thời giảm thiểu rủi ro đọng vốn tồn kho?"*

Hệ thống DSS sử dụng phương pháp **AHP** để kết hợp giữa:
- **Dữ liệu thực tế từ bán hàng (Quantitative OLTP Data):** Biên lợi nhuận gộp, giá vốn, tồn kho, doanh số bán thực tế.
- **Đánh giá chuyên gia thị trường (Qualitative / Market Survey):** Xu hướng nhu cầu người tiêu dùng, chi phí tiếp thị dự kiến.

---

## 2. CÂY TIÊU CHÍ ĐÁNH GIÁ (5 CRITERIA)

| Mã Tiêu Chí | Tên Tiêu Chí | Loại Tiêu Chí | Trọng Số ($w_i$) | Ý Nghĩa / Cách Đo |
| :--- | :--- | :---: | :---: | :--- |
| **C_PROFIT** | Biên lợi nhuận gộp | **Benefit** (Càng lớn càng tốt) | **41.62%** | $(P_{sell} - P_{cost}) / P_{sell}$. Quyết định khả năng sinh lời. |
| **C_REVENUE** | Doanh thu tiềm năng | **Benefit** (Càng lớn càng tốt) | **26.18%** | Quy mô tiền bán hàng ước tính mang về cho công ty. |
| **C_DEMAND** | Nhu cầu & Xu hướng thị trường | **Benefit** (Càng lớn càng tốt) | **16.11%** | Độ quan tâm, xu hướng tìm kiếm và sức mua thị trường. |
| **C_RISK** | Rủi ro tồn kho & Thời gian hoàn vốn | **Cost** (Càng thấp càng tốt) | **9.86%** | Vòng quay vốn, nguy cơ lỗi thời, hỏng hóc pin khi lưu kho. |
| **C_MKTCOST** | Chi phí tiếp thị & Khuyến mãi | **Cost** (Càng thấp càng tốt) | **6.24%** | Chi phí quảng cáo (Ads, KOL, Voucher) để bán được hàng. |

### Ma trận so sánh cặp Saaty và Kiểm định nhất quán:
- Tỷ số nhất quán tính toán: **$CR = 0.0172 < 0.10$** $\implies$ **Ma trận đạt chuẩn nhất quán của Thomas Saaty**, dữ liệu hoàn toàn hợp lệ để ra quyết định.

---

## 3. CHI TIẾT 4 KỊCH BẢN KIỂM THỬ (TEST CASES)

| Kịch Bản | Phương Án Ứng Viên | Đặc Điểm Dữ Liệu | Điểm AHP | Xếp Hạng | Quyết Định Phê Duyệt | Khuyến Nghị Hệ Thống |
| :--- | :--- | :--- | :---: | :---: | :---: | :--- |
| **Trường hợp 1:<br>TỐT (Best Case)** | **Samsung Galaxy S24 Ultra** (`ALT_S24_ULTRA`) | - Biên LN cao (26.3%), giá trị tuyệt đối lớn.<br>- Doanh thu kỳ vọng: 1.2 tỷ VNĐ.<br>- Nhu cầu thị trường rất mạnh (90/100).<br>- Tồn kho quay vòng nhanh (15 ngày). | **0.9103** | **1** | **Selected**<br>*(Được chọn)* | **Ưu tiên số 1:** Nhập số lượng lớn (150 chiếc), phân bổ 50% ngân sách marketing để chiếm lĩnh thị phần. |
| **Trường hợp 2:<br>TRUNG BÌNH (Average Case)** | **Dell Gaming G15 5530** (`ALT_DELL_G15`) | - Biên LN khá (26.0%).<br>- Doanh thu kỳ vọng: 600 triệu VNĐ.<br>- Nhu cầu ổn định cho sinh viên/game thủ.<br>- Vòng quay vốn 30 ngày (trung bình). | **0.7273** | **2** | **UnderReview**<br>*(Xem xét)* | **Ưu tiên số 2:** Nhập số lượng vừa phải (40 chiếc), phục vụ mùa tựu trường và Tết. |
| **Trường hợp 3:<br>EDGE CASE (Biên/Ngách)** | **Apple Watch Ultra 2** (`ALT_AW_ULTRA2`) | - Biên LN siêu cao (30.25%).<br>- Doanh thu hạn chế do giá cao kén khách (250 tr).<br>- Chi phí marketing lớn để tiếp cận khách cao cấp.<br>- Tồn kho 45 ngày, đọng vốn lớn. | **0.6484** | **3** | **UnderReview**<br>*(Xem xét)* | **Phương án ngách:** Biên lợi nhuận hấp dẫn nhưng rủi ro vốn lớn. Chỉ nhập thăm dò (15 chiếc) theo đơn đặt cọc trước. |
| **Trường hợp 4:<br>KÉM (Poor Case)** | **Loa Bluetooth Mini BT-01** (`ALT_SPK_BT01`) | - Tỷ lệ LN % cao nhưng giá trị tiền mặt nhỏ.<br>- Doanh thu đóng góp rất thấp (50 tr).<br>- Hàng bão hòa, thị trường thoái trào.<br>- Tồn đọng 250 chiếc (>90 ngày), nguy cơ hỏng pin. | **0.3131** | **4** | **Rejected**<br>*(Loại bỏ)* | **Loại bỏ:** Không nhập thêm lô mới; kích hoạt xả hàng tồn (giảm 40% hoặc combo quà tặng) để thu hồi dòng tiền. |

---

## 4. HƯỚNG DẪN CHẠY VÀ KIỂM CHỨNG TRÊN SQL SERVER

Toàn bộ mã nguồn đã được đóng gói thành các file SQL tiêu chuẩn tại thư mục `sql/`:

### Bước 1: Tạo Database và Bảng
Mở SQL Server Management Studio (SSMS) hoặc chạy dòng lệnh:
```bash
sqlcmd -S ".\SQLEXPRESS25" -E -I -i "g:\Decision System\sql\01_create_database.sql"
```

### Bước 2: Nạp dữ liệu mẫu
```bash
sqlcmd -S ".\SQLEXPRESS25" -E -I -i "g:\Decision System\sql\02_seed_data.sql"
```

### Bước 3: Chạy các câu lệnh phân tích & báo cáo DSS
```bash
sqlcmd -S ".\SQLEXPRESS25" -E -I -i "g:\Decision System\sql\03_dss_reports_and_queries.sql"
```

---

## 5. KẾT QUẢ TRUY VẤN MẪU TỪ HỆ THỐNG

### 1. Ma trận so sánh cặp AHP dạng Pivot (Saaty Matrix):
```text
Tiêu chí dòng                  C1: Biên LN  C2: Doanh thu  C3: Nhu cầu  C4: Rủi ro  C5: Chi phí Mkt
-----------------------------------------------------------------------------------------------
Biên lợi nhuận gộp             1.00000      2.00000        3.00000      4.00000     5.00000
Doanh thu kỳ vọng              0.50000      1.00000        2.00000      3.00000     4.00000
Nhu cầu & Xu hướng thị trường  0.33333      0.50000        1.00000      2.00000     3.00000
Rủi ro tồn kho & Thời gian vốn 0.25000      0.33333        0.50000      1.00000     2.00000
Chi phí tiếp thị & Khuyến mãi  0.20000      0.25000        0.33333      0.50000     1.00000
```

### 2. Trọng số tính toán tự động qua Stored Procedure `sp_CalculateAHPWeights`:
```text
criterion_code  criterion_name                  calculated_weight  weight_percentage
--------------  ------------------------------  -----------------  -----------------
C_PROFIT        Biên lợi nhuận gộp              0.4162             41.62 %
C_REVENUE       Doanh thu kỳ vọng               0.2618             26.18 %
C_DEMAND        Nhu cầu & Xu hướng thị trường   0.1611             16.11 %
C_RISK          Rủi ro tồn kho & Thời gian vốn  0.0986              9.86 %
C_MKTCOST       Chi phí tiếp thị & Khuyến mãi   0.0624              6.24 %
```

### 3. Báo cáo xếp hạng quyết định cuối cùng (`vw_DSS_FinalRankingReport`):
```text
Thứ hạng  Tên phương án / Sản phẩm                     Điểm AHP   Quyết định    Khuyến nghị chi tiết
--------  -------------------------------------------  ---------  ------------  -------------------------------------------------------------
1         Samsung Galaxy S24 Ultra (Case TỐT)          0.910300   Selected      ƯU TIÊN HÀNG ĐẦU - Phê duyệt nhập hàng & ngân sách
2         Dell Gaming G15 (Case TRUNG BÌNH KHÁ)        0.727300   UnderReview   XEM XÉT THÊM - Nhập thăm dò số lượng hạn chế
3         Apple Watch Ultra 2 (Case EDGE CASE - NICHE) 0.648400   UnderReview   XEM XÉT THÊM - Nhập thăm dò số lượng hạn chế
4         Loa Bluetooth Mini BT-01 (Case KÉM - LOẠI)   0.313100   Rejected      LOẠI BỎ - Không nhập thêm, kích hoạt xả hàng
```
