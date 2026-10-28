using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Microsoft.Data.SqlClient;

namespace DSS_App
{
    public class CriterionInfo
    {
        public int Id;
        public string Code = "";
        public string Name = "";
        public string Type = "Benefit"; // Benefit / Cost
        public double Weight;
    }

    public class AhpResult
    {
        public List<CriterionInfo> Criteria = new List<CriterionInfo>();
        public bool MatrixComplete;
        public double LambdaMax;
        public double CI;
        public double RI;
        public double CR;
        public bool IsConsistent => MatrixComplete && CR <= 0.10;
    }

    public class AltAnalysis
    {
        public int AltId;
        public string Code = "";
        public string Name = "";
        public string Sku = "";
        public decimal Price;
        public decimal Cost;
        public int Stock;
        public int MinStock;
        public int LeadTime;
        public double Score;
        public int Rank;
        public string Outcome = "";
        public Dictionary<int, double> Raw = new Dictionary<int, double>();
        public Dictionary<int, double> Norm = new Dictionary<int, double>();
        public double MarginPct;
        public double TotalSold;
        public double DailyDemand;
        public double DaysCover = -1;      // -1: chưa có doanh số lịch sử
        public int SuggestedQty;
        public double BudgetShare;
        public string ShortName => Name.Split('(')[0].Trim();
    }

    public class DssSnapshot
    {
        public int SessionId;
        public string SessionCode = "";
        public AhpResult Ahp = new AhpResult();
        public List<AltAnalysis> Alts = new List<AltAnalysis>();
        public int HistoryDays;
        public bool HasResults => Alts.Count > 0 && Alts.Any(a => a.Rank > 0);
        public AltAnalysis? Top => Alts.OrderBy(a => a.Rank).FirstOrDefault(a => a.Rank > 0);
        public IEnumerable<AltAnalysis> Ranked => Alts.Where(a => a.Rank > 0).OrderBy(a => a.Rank);
    }

    /// <summary>
    /// Bộ máy AHP chạy hoàn toàn trên dữ liệu SQL Server: trọng số, CR, chuẩn hóa điểm, xếp hạng
    /// và khuyến nghị đều được TÍNH từ dữ liệu trong CSDL, không dùng số liệu cố định.
    /// </summary>
    public static class DssEngine
    {
        public const double ApproveThreshold = 0.60;   // Ngưỡng điểm AHP để phương án được "Xem xét" thay vì "Loại bỏ"
        public const double CrLimit = 0.10;            // Ngưỡng nhất quán chuẩn của Saaty
        public const int ReplenishCycleDays = 30;      // Chu kỳ đặt hàng bổ sung dùng trong công thức đề xuất số lượng

        private static readonly double[] RandomIndex = { 0, 0, 0.58, 0.90, 1.12, 1.24, 1.32, 1.41, 1.45, 1.49 };

        public static AhpResult ComputeAhp(SqlConnection conn, int sessionId)
        {
            var res = new AhpResult();

            using (var cmd = new SqlCommand(
                "SELECT criterion_id, criterion_code, criterion_name, criterion_type FROM dbo.CRITERIA WHERE is_active = 1 ORDER BY criterion_id", conn))
            using (var rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    res.Criteria.Add(new CriterionInfo
                    {
                        Id = rd.GetInt32(0),
                        Code = rd.GetString(1),
                        Name = rd.GetString(2),
                        Type = rd.GetString(3)
                    });
                }
            }

            int n = res.Criteria.Count;
            if (n == 0) return res;

            double[,] a = new double[n, n];
            bool[,] has = new bool[n, n];
            var index = res.Criteria.Select((c, i) => new { c.Id, i }).ToDictionary(x => x.Id, x => x.i);

            using (var cmd = new SqlCommand(
                "SELECT criterion_id_1, criterion_id_2, saaty_value FROM dbo.PAIRWISE_COMPARISONS WHERE session_id = @s", conn))
            {
                cmd.Parameters.AddWithValue("@s", sessionId);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        int c1 = rd.GetInt32(0), c2 = rd.GetInt32(1);
                        if (!index.ContainsKey(c1) || !index.ContainsKey(c2)) continue;
                        a[index[c1], index[c2]] = Convert.ToDouble(rd.GetValue(2), CultureInfo.InvariantCulture);
                        has[index[c1], index[c2]] = true;
                    }
                }
            }

            res.MatrixComplete = true;
            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    if (!has[i, j] || a[i, j] <= 0) res.MatrixComplete = false;

            if (!res.MatrixComplete) return res;

            // Chuẩn hóa theo cột, trọng số = trung bình dòng
            double[] colSum = new double[n];
            for (int j = 0; j < n; j++)
                for (int i = 0; i < n; i++) colSum[j] += a[i, j];

            double[] w = new double[n];
            for (int i = 0; i < n; i++)
            {
                double s = 0;
                for (int j = 0; j < n; j++) s += a[i, j] / colSum[j];
                w[i] = s / n;
            }

            // λmax, CI, CR
            double lambda = 0;
            for (int i = 0; i < n; i++)
            {
                double aw = 0;
                for (int j = 0; j < n; j++) aw += a[i, j] * w[j];
                lambda += aw / w[i];
            }
            lambda /= n;

            res.LambdaMax = lambda;
            res.CI = n > 1 ? (lambda - n) / (n - 1) : 0;
            res.RI = n < RandomIndex.Length ? RandomIndex[n] : RandomIndex[RandomIndex.Length - 1];
            res.CR = res.RI > 0 ? Math.Max(0, res.CI) / res.RI : 0;

            for (int i = 0; i < n; i++) res.Criteria[i].Weight = w[i];
            return res;
        }

        /// <summary>
        /// Chạy mô hình: kiểm tra CR, chuẩn hóa điểm thô từ dữ liệu, tính điểm tổng hợp, xếp hạng,
        /// lưu vào DECISION_RESULTS / DECISION_SESSIONS và sinh khuyến nghị từ số liệu thật.
        /// </summary>
        public static DssSnapshot Recalculate(SqlConnection conn, int sessionId)
        {
            AhpResult ahp = ComputeAhp(conn, sessionId);
            if (!ahp.MatrixComplete)
                throw new InvalidOperationException("Ma trận so sánh cặp Saaty trong CSDL chưa đầy đủ nên không thể tính trọng số.");
            if (ahp.CR > CrLimit)
                throw new InvalidOperationException(
                    $"Ma trận so sánh cặp không nhất quán: CR = {ahp.CR:0.0000} ({ahp.CR * 100:0.00}%) vượt ngưỡng {CrLimit * 100:0}%.\nVui lòng điều chỉnh lại ma trận Saaty trước khi ra quyết định.");

            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    NormalizeScores(conn, tx, sessionId, ahp);

                    // Điểm tổng hợp = Σ (trọng số × điểm chuẩn hóa)
                    var scores = new Dictionary<int, double>();
                    using (var cmd = new SqlCommand(
                        "SELECT alternative_id, criterion_id, normalized_score FROM dbo.ALTERNATIVE_SCORES WHERE session_id = @s", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@s", sessionId);
                        using (var rd = cmd.ExecuteReader())
                        {
                            var wById = ahp.Criteria.ToDictionary(c => c.Id, c => c.Weight);
                            while (rd.Read())
                            {
                                int alt = rd.GetInt32(0), crit = rd.GetInt32(1);
                                if (!wById.ContainsKey(crit)) continue;
                                double norm = Convert.ToDouble(rd.GetValue(2), CultureInfo.InvariantCulture);
                                scores[alt] = (scores.ContainsKey(alt) ? scores[alt] : 0) + wById[crit] * norm;
                            }
                        }
                    }

                    if (scores.Count == 0)
                        throw new InvalidOperationException("Chưa có điểm đánh giá phương án trong CSDL (ALTERNATIVE_SCORES).");

                    var ordered = scores.OrderByDescending(kv => Math.Round(kv.Value, 6)).ToList();
                    int rank = 0; double? prev = null;
                    foreach (var kv in ordered)
                    {
                        double rounded = Math.Round(kv.Value, 6);
                        if (prev == null || rounded != prev) { rank++; prev = rounded; }
                        string outcome = rank == 1 ? "Selected" : (rounded >= ApproveThreshold ? "UnderReview" : "Rejected");

                        using (var cmd = new SqlCommand(@"
                            IF EXISTS (SELECT 1 FROM dbo.DECISION_RESULTS WHERE session_id = @s AND alternative_id = @a)
                                UPDATE dbo.DECISION_RESULTS SET final_score = @f, ranking = @r, decision_outcome = @o, decision_date = SYSDATETIME()
                                WHERE session_id = @s AND alternative_id = @a;
                            ELSE
                                INSERT INTO dbo.DECISION_RESULTS (session_id, alternative_id, final_score, ranking, decision_outcome, comments)
                                VALUES (@s, @a, @f, @r, @o, N'');", conn, tx))
                        {
                            cmd.Parameters.AddWithValue("@s", sessionId);
                            cmd.Parameters.AddWithValue("@a", kv.Key);
                            cmd.Parameters.AddWithValue("@f", (decimal)Math.Min(1.0, rounded));
                            cmd.Parameters.AddWithValue("@r", rank);
                            cmd.Parameters.AddWithValue("@o", outcome);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    using (var cmd = new SqlCommand(
                        "UPDATE dbo.DECISION_SESSIONS SET consistency_ratio = @cr, status = 'Calculated' WHERE session_id = @s", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@cr", (decimal)Math.Round(ahp.CR, 4));
                        cmd.Parameters.AddWithValue("@s", sessionId);
                        cmd.ExecuteNonQuery();
                    }

                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }

            // Sinh nhận xét từ số liệu thật rồi ghi lại vào CSDL
            DssSnapshot snap = LoadSnapshot(conn, sessionId);
            foreach (var alt in snap.Ranked)
            {
                using (var cmd = new SqlCommand(
                    "UPDATE dbo.DECISION_RESULTS SET comments = @c WHERE session_id = @s AND alternative_id = @a", conn))
                {
                    string c = AltComment(snap, alt);
                    cmd.Parameters.AddWithValue("@c", c.Length > 500 ? c.Substring(0, 500) : c);
                    cmd.Parameters.AddWithValue("@s", sessionId);
                    cmd.Parameters.AddWithValue("@a", alt.AltId);
                    cmd.ExecuteNonQuery();
                }
            }
            return snap;
        }

        /// <summary>Tính lại điểm chuẩn hóa của cả phiên từ giá trị thô trong CSDL (dùng sau khi thêm/sửa phương án).</summary>
        public static void RefreshNormalization(SqlConnection conn, int sessionId)
        {
            AhpResult ahp = ComputeAhp(conn, sessionId);
            using (var tx = conn.BeginTransaction())
            {
                try
                {
                    NormalizeScores(conn, tx, sessionId, ahp);
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }
        }

        /// <summary>
        /// Chuẩn hóa điểm từ giá trị thô trong CSDL: Benefit = v / max, Cost = min / v.
        /// </summary>
        private static void NormalizeScores(SqlConnection conn, SqlTransaction tx, int sessionId, AhpResult ahp)
        {
            var raw = new List<(int alt, int crit, double v)>();
            using (var cmd = new SqlCommand(
                "SELECT alternative_id, criterion_id, raw_value FROM dbo.ALTERNATIVE_SCORES WHERE session_id = @s", conn, tx))
            {
                cmd.Parameters.AddWithValue("@s", sessionId);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                        raw.Add((rd.GetInt32(0), rd.GetInt32(1), Convert.ToDouble(rd.GetValue(2), CultureInfo.InvariantCulture)));
                }
            }

            foreach (var crit in ahp.Criteria)
            {
                var vals = raw.Where(r => r.crit == crit.Id).ToList();
                if (vals.Count == 0) continue;
                double max = vals.Max(r => r.v);
                double minPositive = vals.Where(r => r.v > 0).Select(r => r.v).DefaultIfEmpty(0).Min();

                foreach (var r in vals)
                {
                    double norm;
                    if (crit.Type == "Cost")
                        norm = r.v <= 0 ? 1.0 : (minPositive > 0 ? minPositive / r.v : 1.0);
                    else
                        norm = max > 0 ? r.v / max : 0.0;
                    norm = Math.Max(0.0, Math.Min(1.0, norm));

                    using (var cmd = new SqlCommand(
                        "UPDATE dbo.ALTERNATIVE_SCORES SET normalized_score = @n WHERE session_id = @s AND alternative_id = @a AND criterion_id = @c", conn, tx))
                    {
                        cmd.Parameters.AddWithValue("@n", (decimal)Math.Round(norm, 6));
                        cmd.Parameters.AddWithValue("@s", sessionId);
                        cmd.Parameters.AddWithValue("@a", r.alt);
                        cmd.Parameters.AddWithValue("@c", r.crit);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static DssSnapshot LoadSnapshot(SqlConnection conn, int sessionId)
        {
            var snap = new DssSnapshot { SessionId = sessionId };
            snap.Ahp = ComputeAhp(conn, sessionId);

            using (var cmd = new SqlCommand("SELECT session_code FROM dbo.DECISION_SESSIONS WHERE session_id = @s", conn))
            {
                cmd.Parameters.AddWithValue("@s", sessionId);
                snap.SessionCode = Convert.ToString(cmd.ExecuteScalar()) ?? "";
            }

            // Số ngày dữ liệu bán hàng lịch sử (đã hoàn tất)
            using (var cmd = new SqlCommand(
                "SELECT MIN(order_date), MAX(order_date) FROM dbo.ORDERS WHERE order_status = 'Completed'", conn))
            using (var rd = cmd.ExecuteReader())
            {
                if (rd.Read() && !rd.IsDBNull(0) && !rd.IsDBNull(1))
                {
                    snap.HistoryDays = Math.Max(1, (int)(rd.GetDateTime(1).Date - rd.GetDateTime(0).Date).TotalDays + 1);
                }
            }

            using (var cmd = new SqlCommand(@"
                SELECT alt.alternative_id, alt.alternative_code, alt.alternative_name,
                       ISNULL(p.product_code, ''), ISNULL(p.selling_price, 0), ISNULL(p.cost_price, 0),
                       ISNULL(p.stock_quantity, 0), ISNULL(p.min_stock_level, 0), ISNULL(p.lead_time_days, 0),
                       ISNULL(dr.final_score, 0), ISNULL(dr.ranking, 0), ISNULL(dr.decision_outcome, ''),
                       ISNULL((SELECT SUM(od.quantity) FROM dbo.ORDER_DETAILS od
                               INNER JOIN dbo.ORDERS o ON o.order_id = od.order_id AND o.order_status = 'Completed'
                               WHERE od.product_id = alt.product_id), 0)
                FROM dbo.ALTERNATIVES alt
                LEFT JOIN dbo.PRODUCTS p ON alt.product_id = p.product_id
                LEFT JOIN dbo.DECISION_RESULTS dr ON dr.alternative_id = alt.alternative_id AND dr.session_id = alt.session_id
                WHERE alt.session_id = @s
                ORDER BY CASE WHEN dr.ranking IS NULL THEN 1 ELSE 0 END, dr.ranking, alt.alternative_id", conn))
            {
                cmd.Parameters.AddWithValue("@s", sessionId);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        var a = new AltAnalysis
                        {
                            AltId = rd.GetInt32(0),
                            Code = rd.GetString(1),
                            Name = rd.GetString(2),
                            Sku = rd.GetString(3),
                            Price = rd.GetDecimal(4),
                            Cost = rd.GetDecimal(5),
                            Stock = Convert.ToInt32(rd.GetValue(6)),
                            MinStock = Convert.ToInt32(rd.GetValue(7)),
                            LeadTime = Convert.ToInt32(rd.GetValue(8)),
                            Score = Convert.ToDouble(rd.GetValue(9), CultureInfo.InvariantCulture),
                            Rank = Convert.ToInt32(rd.GetValue(10)),
                            Outcome = rd.GetString(11),
                            TotalSold = Convert.ToDouble(rd.GetValue(12), CultureInfo.InvariantCulture)
                        };
                        a.MarginPct = a.Price > 0 ? (double)((a.Price - a.Cost) / a.Price) * 100.0 : 0;
                        snap.Alts.Add(a);
                    }
                }
            }

            var byId = snap.Alts.ToDictionary(a => a.AltId);
            using (var cmd = new SqlCommand(
                "SELECT alternative_id, criterion_id, raw_value, normalized_score FROM dbo.ALTERNATIVE_SCORES WHERE session_id = @s", conn))
            {
                cmd.Parameters.AddWithValue("@s", sessionId);
                using (var rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        int altId = rd.GetInt32(0);
                        if (!byId.ContainsKey(altId)) continue;
                        int crit = rd.GetInt32(1);
                        byId[altId].Raw[crit] = Convert.ToDouble(rd.GetValue(2), CultureInfo.InvariantCulture);
                        byId[altId].Norm[crit] = Convert.ToDouble(rd.GetValue(3), CultureInfo.InvariantCulture);
                    }
                }
            }

            // Đề xuất số lượng nhập: nhu cầu/ngày × (lead time + chu kỳ) + tồn tối thiểu − tồn kho hiện tại
            foreach (var a in snap.Alts)
            {
                a.DailyDemand = snap.HistoryDays > 0 ? a.TotalSold / snap.HistoryDays : 0;
                a.DaysCover = a.DailyDemand > 0 ? a.Stock / a.DailyDemand : -1;
                if (a.Outcome == "Selected" || a.Outcome == "UnderReview")
                {
                    double need = a.DailyDemand * (a.LeadTime + ReplenishCycleDays) + a.MinStock - a.Stock;
                    a.SuggestedQty = (int)Math.Max(0, Math.Ceiling(need));
                }
            }

            // Phân bổ ngân sách marketing theo tỷ trọng điểm AHP giữa các phương án được phê duyệt/xem xét
            double sumApproved = snap.Alts.Where(a => a.Outcome != "Rejected" && a.Rank > 0).Sum(a => a.Score);
            if (sumApproved > 0)
            {
                foreach (var a in snap.Alts.Where(a => a.Outcome != "Rejected" && a.Rank > 0))
                    a.BudgetShare = a.Score / sumApproved;
            }
            return snap;
        }

        // ================= Diễn giải từ dữ liệu =================

        public static string CriterionShort(CriterionInfo c) => c.Name;

        public static string WeightsText(AhpResult ahp)
        {
            if (!ahp.MatrixComplete) return "chưa đủ dữ liệu ma trận so sánh cặp";
            return string.Join(", ", ahp.Criteria.Select(c => $"{c.Name} ({c.Weight * 100:0.00}%)"));
        }

        public static string CrText(AhpResult ahp)
        {
            if (!ahp.MatrixComplete) return "CR = chưa xác định";
            return $"CR = {ahp.CR:0.0000} ({ahp.CR * 100:0.00}%)";
        }

        public static string CrVerdict(AhpResult ahp)
        {
            if (!ahp.MatrixComplete) return "Ma trận Saaty chưa đầy đủ";
            return ahp.IsConsistent ? "ĐẠT CHUẨN NHẤT QUÁN (< 10%)" : "CHƯA ĐẠT CHUẨN (>= 10%)";
        }

        /// <summary>Tiêu chí đóng góp nhiều nhất (trọng số × điểm chuẩn hóa) vào điểm của phương án.</summary>
        public static (CriterionInfo? crit, double contribution) TopContribution(DssSnapshot s, AltAnalysis a)
        {
            CriterionInfo? best = null; double bestVal = -1;
            foreach (var c in s.Ahp.Criteria)
            {
                if (!a.Norm.ContainsKey(c.Id)) continue;
                double v = c.Weight * a.Norm[c.Id];
                if (v > bestVal) { bestVal = v; best = c; }
            }
            return (best, Math.Max(0, bestVal));
        }

        /// <summary>Các tiêu chí mà phương án dẫn đầu (điểm chuẩn hóa cao nhất trong phiên).</summary>
        public static List<CriterionInfo> LeadingCriteria(DssSnapshot s, AltAnalysis a)
        {
            var res = new List<CriterionInfo>();
            foreach (var c in s.Ahp.Criteria)
            {
                if (!a.Norm.ContainsKey(c.Id)) continue;
                double max = s.Alts.Where(x => x.Norm.ContainsKey(c.Id)).Select(x => x.Norm[c.Id]).DefaultIfEmpty(0).Max();
                if (a.Norm[c.Id] >= max - 1e-9) res.Add(c);
            }
            return res;
        }

        public static string StockText(AltAnalysis a)
        {
            if (a.DaysCover >= 0) return $"tồn kho {a.Stock} sản phẩm ≈ {a.DaysCover:0} ngày bán";
            return $"tồn kho {a.Stock} sản phẩm (chưa có doanh số lịch sử)";
        }

        public static string QtyFormula(DssSnapshot s, AltAnalysis a)
        {
            return $"{a.DailyDemand:0.##} sp/ngày × ({a.LeadTime} ngày giao hàng + {ReplenishCycleDays} ngày chu kỳ) + {a.MinStock} tồn tối thiểu − {a.Stock} tồn hiện tại";
        }

        public static string AltComment(DssSnapshot s, AltAnalysis a)
        {
            var (crit, contrib) = TopContribution(s, a);
            var sb = new StringBuilder();
            sb.Append($"Điểm AHP {a.Score:0.0000}");
            if (crit != null) sb.Append($"; đóng góp lớn nhất: {crit.Name} ({contrib:0.0000})");
            sb.Append($"; {StockText(a)}");
            if (a.Outcome == "Selected" || a.Outcome == "UnderReview")
                sb.Append($"; đề xuất nhập {a.SuggestedQty} sản phẩm, {a.BudgetShare * 100:0.#}% ngân sách marketing theo tỷ trọng điểm");
            else if (a.Outcome == "Rejected")
                sb.Append($"; điểm < {ApproveThreshold:0.00} nên không nhập mới, ưu tiên xả tồn kho");
            return sb.ToString();
        }

        public static string OutcomeLabel(string outcome) => outcome switch
        {
            "Selected" => "PHÊ DUYỆT",
            "UnderReview" => "XEM XÉT",
            "Rejected" => "LOẠI BỎ",
            _ => outcome
        };

        public static string ExecutiveSummary(DssSnapshot s)
        {
            var top = s.Top;
            if (top == null) return "Chưa có kết quả tính toán.";
            var lead = LeadingCriteria(s, top);
            var sb = new StringBuilder();
            sb.AppendLine($"• LỰA CHỌN TỐI ƯU SỐ 1: [{top.ShortName}] đạt điểm AHP cao nhất ({top.Score:0.0000}).");
            sb.AppendLine($"• KHUYẾN NGHỊ: đề xuất nhập {top.SuggestedQty} sản phẩm và phân bổ {top.BudgetShare * 100:0.#}% ngân sách marketing theo tỷ trọng điểm AHP trong nhóm được phê duyệt/xem xét.");
            sb.AppendLine($"• CƠ SỞ SỐ LƯỢNG: {QtyFormula(s, top)}.");
            if (lead.Count > 0)
                sb.AppendLine($"• DẪN ĐẦU TIÊU CHÍ: {string.Join(", ", lead.Select(c => $"{c.Name} ({c.Weight * 100:0.00}%)"))}.");
            foreach (var a in s.Ranked.Where(x => x.Outcome == "UnderReview"))
                sb.AppendLine($"• THEO DÕI: [{a.ShortName}] hạng {a.Rank} (điểm {a.Score:0.0000}), đề xuất nhập {a.SuggestedQty} sản phẩm.");
            foreach (var a in s.Ranked.Where(x => x.Outcome == "Rejected"))
                sb.AppendLine($"• XẢ HÀNG: [{a.ShortName}] hạng {a.Rank} (điểm {a.Score:0.0000}), {StockText(a)}.");
            return sb.ToString().TrimEnd();
        }

        public static string DirectiveSelected(DssSnapshot s)
        {
            var top = s.Top;
            if (top == null) return "Chưa có dữ liệu. Hãy chạy mô hình DSS.";
            var lead = LeadingCriteria(s, top);
            var sb = new StringBuilder();
            sb.AppendLine($"• Sản phẩm chủ lực số 1: {top.ShortName} (Điểm AHP: {top.Score:0.0000}).");
            sb.AppendLine($"• Định mức phê duyệt: đề xuất nhập {top.SuggestedQty} sản phẩm ({QtyFormula(s, top)}).");
            sb.AppendLine($"• Phân bổ tài chính: {top.BudgetShare * 100:0.#}% ngân sách marketing theo tỷ trọng điểm AHP.");
            if (lead.Count > 0)
                sb.Append($"• Cơ sở phê duyệt: dẫn đầu ở {string.Join(", ", lead.Select(c => $"{c.Name} ({c.Weight * 100:0.00}%)"))}.");
            else
                sb.Append($"• Cơ sở phê duyệt: điểm tổng hợp cao nhất ({top.Score:0.0000}).");
            return sb.ToString();
        }

        public static string DirectiveReview(DssSnapshot s)
        {
            var list = s.Ranked.Where(a => a.Outcome == "UnderReview").ToList();
            if (list.Count == 0) return s.HasResults ? "• Không có phương án nào ở nhóm XEM XÉT trong phiên này." : "Chưa có dữ liệu. Hãy chạy mô hình DSS.";
            var sb = new StringBuilder();
            foreach (var a in list)
                sb.AppendLine($"• {a.ShortName} (Hạng {a.Rank}, điểm {a.Score:0.0000}): đề xuất nhập {a.SuggestedQty} sản phẩm; {StockText(a)}; {a.BudgetShare * 100:0.#}% ngân sách marketing.");
            sb.Append("• Quản trị rủi ro: số lượng nhập tính từ nhu cầu bán thực tế trong CSDL, không vượt nhu cầu dự báo theo chu kỳ.");
            return sb.ToString();
        }

        public static string DirectiveRejected(DssSnapshot s)
        {
            var list = s.Ranked.Where(a => a.Outcome == "Rejected").ToList();
            if (list.Count == 0) return s.HasResults ? "• Không có phương án nào bị loại trong phiên này." : "Chưa có dữ liệu. Hãy chạy mô hình DSS.";
            var sb = new StringBuilder();
            foreach (var a in list)
                sb.AppendLine($"• {a.ShortName} (Hạng {a.Rank}, điểm {a.Score:0.0000} < {ApproveThreshold:0.00}): dừng nhập mới; {StockText(a)}; ưu tiên xả tồn kho để thu hồi vốn.");
            return sb.ToString().TrimEnd();
        }
    }
}
