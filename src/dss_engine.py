import sys
import io
import pyodbc
import numpy as np

if sys.platform == "win32" and hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8", errors="replace")

DB_CONFIG = {
    "server": r".\SQLEXPRESS25",
    "database": "DSS_SalesDB",
    "driver": "{ODBC Driver 17 for SQL Server}",
    "trusted_connection": "yes",
}


def get_connection():
    """Tạo kết nối tới cơ sở dữ liệu SQL Server DSS_SalesDB"""
    conn_str = (
        f"Driver={DB_CONFIG['driver']};"
        f"Server={DB_CONFIG['server']};"
        f"Database={DB_CONFIG['database']};"
        f"Trusted_Connection={DB_CONFIG['trusted_connection']};"
    )
    return pyodbc.connect(conn_str)


def fetch_ahp_criteria_matrix(conn, session_id: int):
    """
    Trích xuất ma trận so sánh cặp Saaty 1-9 từ bảng PAIRWISE_COMPARISONS
    """
    cursor = conn.cursor()

    cursor.execute(
        """
        SELECT DISTINCT c.criterion_id, c.criterion_code, c.criterion_name, c.criterion_type
        FROM dbo.PAIRWISE_COMPARISONS pc
        INNER JOIN dbo.CRITERIA c ON pc.criterion_id_1 = c.criterion_id
        WHERE pc.session_id = ?
        ORDER BY c.criterion_id ASC;
    """,
        session_id,
    )
    criteria = cursor.fetchall()

    crit_ids = [row[0] for row in criteria]
    n = len(crit_ids)
    matrix = np.ones((n, n), dtype=float)

    cursor.execute(
        """
        SELECT criterion_id_1, criterion_id_2, saaty_value
        FROM dbo.PAIRWISE_COMPARISONS
        WHERE session_id = ?;
    """,
        session_id,
    )
    comparisons = cursor.fetchall()

    id_to_idx = {cid: idx for idx, cid in enumerate(crit_ids)}
    for c1, c2, val in comparisons:
        i = id_to_idx[c1]
        j = id_to_idx[c2]
        matrix[i, j] = float(val)

    return criteria, matrix


def calculate_ahp_weights(matrix: np.ndarray):
    """
    Tính toán vector trọng số w_i, eigenvalue lambda_max và Tỷ số nhất quán CR
    theo lý thuyết Analytic Hierarchy Process (Thomas L. Saaty)
    """
    n = matrix.shape[0]

    col_sums = matrix.sum(axis=0)
    norm_matrix = matrix / col_sums

    weights = norm_matrix.mean(axis=1)

    aw = np.dot(matrix, weights)
    lambda_max = float(np.mean(aw / weights))

    ri_table = {
        1: 0.0,
        2: 0.0,
        3: 0.58,
        4: 0.90,
        5: 1.12,
        6: 1.24,
        7: 1.32,
        8: 1.41,
        9: 1.45,
    }
    ri = ri_table.get(n, 1.49)

    if n > 2 and ri > 0:
        ci = (lambda_max - n) / (n - 1)
        cr = ci / ri
    else:
        ci = 0.0
        cr = 0.0

    return weights, lambda_max, cr


def fetch_alternative_scores(conn, session_id: int):
    """
    Trích xuất danh sách phương án và điểm chuẩn hóa từ bảng ALTERNATIVES & ALTERNATIVE_SCORES
    """
    cursor = conn.cursor()
    cursor.execute(
        """
        SELECT 
            alt.alternative_id,
            alt.alternative_code,
            alt.alternative_name,
            s.criterion_id,
            s.normalized_score
        FROM dbo.ALTERNATIVES alt
        INNER JOIN dbo.ALTERNATIVE_SCORES s ON alt.alternative_id = s.alternative_id
        WHERE alt.session_id = ?
        ORDER BY alt.alternative_id ASC, s.criterion_id ASC;
    """,
        session_id,
    )
    rows = cursor.fetchall()

    alternatives = {}
    for alt_id, code, name, crit_id, score in rows:
        if alt_id not in alternatives:
            alternatives[alt_id] = {"code": code, "name": name, "scores": {}}
        alternatives[alt_id]["scores"][crit_id] = float(score)

    return alternatives


def run_dss_pipeline(session_id: int = 1, persist_to_db: bool = True):
    """
    Quy trình tích hợp hoàn chỉnh:
    1. Lấy dữ liệu từ Database (SQL)
    2. Chạy mô hình AHP (DSS Model)
    3. Cập nhật kết quả vào Database (Result)
    """
    print(f"\n{'=' * 75}")
    print(f"BẮT ĐẦU CHẠY DSS ENGINE CHO PHIÊN QUYẾT ĐỊNH (SESSION ID = {session_id})")
    print(f"{'=' * 75}")

    conn = get_connection()
    try:
        criteria, matrix = fetch_ahp_criteria_matrix(conn, session_id)
        weights, lambda_max, cr = calculate_ahp_weights(matrix)

        print("\n1. KẾT QUẢ TÍNH TRỌNG SỐ TIÊU CHÍ (AHP WEIGHTS):")
        print(
            f"{'Mã TC':<12} | {'Tên Tiêu Chí':<32} | {'Loại':<8} | {'Trọng số (wi)':<15}"
        )
        print("-" * 75)
        for i, crit in enumerate(criteria):
            print(
                f"{crit[1]:<12} | {crit[2]:<32} | {crit[3]:<8} | {weights[i]:.4f} ({weights[i] * 100:.2f}%)"
            )

        print(f"\n>> lambda_max = {lambda_max:.4f}")
        print(f">> Tỷ số nhất quán (Consistency Ratio): CR = {cr:.4f}")
        is_consistent = cr <= 0.10
        print(
            f">> Đánh giá nhất quán Saaty: {'ĐẠT CHUẨN (CR <= 0.10)' if is_consistent else 'KHÔNG ĐẠT (CR > 0.10)'}"
        )

        alternatives = fetch_alternative_scores(conn, session_id)
        crit_ids = [crit[0] for crit in criteria]

        results = []
        for alt_id, alt_data in alternatives.items():
            alt_score_vector = np.array(
                [alt_data["scores"].get(cid, 0.0) for cid in crit_ids]
            )
            final_score = float(np.dot(weights, alt_score_vector))
            results.append(
                {
                    "alternative_id": alt_id,
                    "code": alt_data["code"],
                    "name": alt_data["name"],
                    "final_score": final_score,
                }
            )

        results.sort(key=lambda x: x["final_score"], reverse=True)
        for rank, res in enumerate(results, start=1):
            res["ranking"] = rank
            if rank == 1:
                res["outcome"] = "Selected"
                res["comment"] = "PHƯƠNG ÁN TỐI ƯU NHẤT - Ưu tiên rót vốn & triển khai"
            elif res["final_score"] >= 0.60:
                res["outcome"] = "UnderReview"
                res["comment"] = (
                    "KHẢ QUAN - Cần đánh giá thêm điều kiện rủi ro/ngân sách"
                )
            else:
                res["outcome"] = "Rejected"
                res["comment"] = "LOẠI BỎ - Không đạt hiệu quả đa tiêu chí"

        if persist_to_db:
            cursor = conn.cursor()
            cursor.execute(
                """
                UPDATE dbo.DECISION_SESSIONS
                SET consistency_ratio = ?
                WHERE session_id = ?;
            """,
                cr,
                session_id,
            )

            for res in results:
                cursor.execute(
                    """
                    IF EXISTS (SELECT 1 FROM dbo.DECISION_RESULTS WHERE session_id = ? AND alternative_id = ?)
                    BEGIN
                        UPDATE dbo.DECISION_RESULTS
                        SET final_score = ?,
                            ranking = ?,
                            decision_outcome = ?,
                            decision_date = SYSDATETIME(),
                            comments = ?
                        WHERE session_id = ? AND alternative_id = ?;
                    END
                    ELSE
                    BEGIN
                        INSERT INTO dbo.DECISION_RESULTS (session_id, alternative_id, final_score, ranking, decision_outcome, comments)
                        VALUES (?, ?, ?, ?, ?, ?);
                    END
                """,
                    session_id,
                    res["alternative_id"],
                    res["final_score"],
                    res["ranking"],
                    res["outcome"],
                    res["comment"],
                    session_id,
                    res["alternative_id"],
                    session_id,
                    res["alternative_id"],
                    res["final_score"],
                    res["ranking"],
                    res["outcome"],
                    res["comment"],
                )

            conn.commit()
            print(
                "\n>> ĐÃ CẬP NHẬT KẾT QUẢ XẾP HẠNG THÀNH CÔNG VÀO BẢNG [DECISION_RESULTS] TRÊN SQL SERVER!"
            )

        print("\n2. BẢNG XẾP HẠNG PHƯƠNG ÁN RA QUYẾT ĐỊNH (DSS FINAL RANKING):")
        print(
            f"{'Hạng':<6} | {'Mã PA':<16} | {'Tên Phương Án':<38} | {'Điểm AHP':<10} | {'Quyết Định':<12}"
        )
        print("-" * 90)
        for res in results:
            print(
                f"{res['ranking']:<6} | {res['code']:<16} | {res['name']:<38} | {res['final_score']:.4f}     | {res['outcome']:<12}"
            )

        return results, cr
    finally:
        conn.close()


if __name__ == "__main__":
    run_dss_pipeline(session_id=1, persist_to_db=True)
