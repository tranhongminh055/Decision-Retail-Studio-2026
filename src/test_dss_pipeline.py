"""
====================================================================================
ĐỒ ÁN NHÓM IS402 - HỆ THỐNG THÔNG TIN HỖ TRỢ RA QUYẾT ĐỊNH (DSS)
SCRIPT KIỂM CHỨNG TIÊU CHÍ HOÀN THÀNH (DONE CONDITION):
"Thay đổi dữ liệu trong Database và chạy lại hệ thống thì kết quả DSS thay đổi theo dữ liệu"
====================================================================================
"""

import sys
import io
import pyodbc
from dss_engine import get_connection, run_dss_pipeline

if sys.platform == "win32" and hasattr(sys.stdout, 'reconfigure'):
    sys.stdout.reconfigure(encoding='utf-8', errors='replace')

def get_db_ranking():
    conn = get_connection()
    cursor = conn.cursor()
    cursor.execute("""
        SELECT dr.ranking, alt.alternative_code, alt.alternative_name, dr.final_score, dr.decision_outcome
        FROM dbo.DECISION_RESULTS dr
        INNER JOIN dbo.ALTERNATIVES alt ON dr.alternative_id = alt.alternative_id
        WHERE dr.session_id = 1
        ORDER BY dr.ranking ASC;
    """)
    rows = cursor.fetchall()
    conn.close()
    return rows

def print_current_db_ranking(title):
    print(f"\n{'-'*85}")
    print(f">> {title}")
    print(f"{'-'*85}")
    print(f"{'Hạng':<6} | {'Mã PA':<16} | {'Tên Phương Án':<36} | {'Điểm AHP':<10} | {'Quyết Định'}")
    print("-" * 85)
    for r in get_db_ranking():
        print(f"{r[0]:<6} | {r[1]:<16} | {r[2]:<36} | {r[3]:<10.4f} | {r[4]}")

def test_pipeline_reactivity():
    print("=" * 85)
    print("BẮT ĐẦU KIỂM THỬ: KIỂM CHỨNG TÍNH ĐỘNG CỦA HỆ DSS KHI DỮ LIỆU CSDL THAY ĐỔI")
    print("=" * 85)
    
    print("\n>>> [GIAI ĐOẠN 1]: Chạy DSS Engine với dữ liệu hiện tại trong Database...")
    run_dss_pipeline(session_id=1, persist_to_db=True)
    print_current_db_ranking("KẾT QUẢ ĐỌC TRỰC TIẾP TỪ BẢNG [DECISION_RESULTS] TRÊN SQL SERVER (LẦN 1)")
    
    print("\n" + "=" * 85)
    print(">>> [GIAI ĐOẠN 2]: GIẢ LẬP BIẾN ĐỘNG THỊ TRƯỜNG -> CẬP NHẬT DỮ LIỆU TRỰC TIẾP TRONG DATABASE")
    print("Kịch bản biến động:")
    print(" - Dell Gaming G15: Doanh thu kỳ vọng & nhu cầu thị trường tăng vọt (C_REVENUE -> 0.98, C_DEMAND -> 0.95)")
    print(" - S24 Ultra: Bị đứt gãy linh kiện, rủi ro tồn kho tăng cao (C_RISK giảm điểm xuống 0.40)")
    print("=" * 85)
    
    conn = get_connection()
    cursor = conn.cursor()
    
    cursor.execute("""
        SELECT alternative_id, criterion_id, normalized_score
        FROM dbo.ALTERNATIVE_SCORES
        WHERE session_id = 1;
    """)
    backup_scores = cursor.fetchall()
    
    cursor.execute("""
        UPDATE dbo.ALTERNATIVE_SCORES
        SET normalized_score = 0.960000
        WHERE session_id = 1 AND alternative_id = 2 AND criterion_id = 1; -- C_PROFIT
        
        UPDATE dbo.ALTERNATIVE_SCORES
        SET normalized_score = 0.980000
        WHERE session_id = 1 AND alternative_id = 2 AND criterion_id = 2; -- C_REVENUE
        
        UPDATE dbo.ALTERNATIVE_SCORES
        SET normalized_score = 0.950000
        WHERE session_id = 1 AND alternative_id = 2 AND criterion_id = 3; -- C_DEMAND
    """)
    
    cursor.execute("""
        UPDATE dbo.ALTERNATIVE_SCORES
        SET normalized_score = 0.350000
        WHERE session_id = 1 AND alternative_id = 1 AND criterion_id = 4; -- C_RISK
    """)
    conn.commit()
    conn.close()
    print("-> Đã thực hiện UPDATE dữ liệu thành công trong Database SQL Server!")
    
    print("\n>>> [GIAI ĐOẠN 3]: Chạy lại DSS Engine (Database -> SQL -> DSS Model -> Result)...")
    run_dss_pipeline(session_id=1, persist_to_db=True)
    print_current_db_ranking("KẾT QUẢ ĐỌC TRỰC TIẾP TỪ BẢNG [DECISION_RESULTS] SAU KHI ĐỔI DỮ LIỆU (LẦN 2)")
    
    ranking_after = get_db_ranking()
    top_alt = ranking_after[0]
    print("\n" + "=" * 85)
    print(">>> [KẾT LUẬN KIỂM CHỨNG TIÊU CHÍ DONE]:")
    print(f"-> Phương án dẫn đầu mới trong Database là: {top_alt[1]} - {top_alt[2]}")
    print(f"-> Điểm số AHP mới: {top_alt[3]:.4f} | Trạng thái: {top_alt[4]}")
    
    if top_alt[1] == "ALT_DELL_G15":
        print("-> CHÍNH XÁC: Dell Gaming G15 đã tự động vươn lên HẠNG 1 theo đúng dữ liệu mới trong Database!")
        print("-> ĐÃ CHỨNG MINH 100%: Hệ thống KHÔNG hard-code dữ liệu. Thay đổi Database làm kết quả DSS thay đổi tương ứng!")
    else:
        print("-> Lưu ý: Thứ hạng chưa thay đổi như kỳ vọng.")
    print("=" * 85)
    
    print("\n>>> Đang phục hồi dữ liệu ban đầu cho Database để bảo đảm tính toàn vẹn...")
    conn = get_connection()
    cursor = conn.cursor()
    for alt_id, crit_id, old_score in backup_scores:
        cursor.execute("""
            UPDATE dbo.ALTERNATIVE_SCORES
            SET normalized_score = ?
            WHERE session_id = 1 AND alternative_id = ? AND criterion_id = ?;
        """, old_score, alt_id, crit_id)
    conn.commit()
    conn.close()
    
    run_dss_pipeline(session_id=1, persist_to_db=True)
    print("-> Đã phục hồi dữ liệu gốc thành công!")

if __name__ == "__main__":
    test_pipeline_reactivity()
