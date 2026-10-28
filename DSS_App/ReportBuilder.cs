using System;
using System.Linq;
using System.Net;
using System.Text;

namespace DSS_App
{
    /// <summary>
    /// Sinh báo cáo (Word / HTML / Text) hoàn toàn từ <see cref="DssSnapshot"/> đọc trực tiếp từ SQL Server.
    /// Không chứa số liệu cố định: trọng số, CR, điểm, thứ hạng, số lượng nhập, ngân sách đều được tính từ dữ liệu.
    /// </summary>
    public static class ReportBuilder
    {
        private static string E(string? s) => WebUtility.HtmlEncode(s ?? "");

        private static string[] BulletLines(string text) =>
            (text ?? "").Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(l => l.Trim().TrimStart('•').Trim())
                        .Where(l => l.Length > 0).ToArray();

        private static string Money(decimal v) => v.ToString("#,##0") + " đ";

        private static string Author(string? fullName, string? role) =>
            string.IsNullOrWhiteSpace(fullName) ? "(chưa xác định người dùng)" : (string.IsNullOrWhiteSpace(role) ? fullName : $"{fullName} [{role}]");

        // ======================= WORD (.doc dạng HTML) =======================
        public static string Word(DssSnapshot s, string? userName, string? userRole)
        {
            var sb = new StringBuilder();
            var top = s.Top!;
            sb.AppendLine("<html xmlns:o='urn:schemas-microsoft-com:office:office' xmlns:w='urn:schemas-microsoft-com:office:word' xmlns='http://www.w3.org/TR/REC-html40'>");
            sb.AppendLine("<head><meta charset='utf-8'><title>Biên Bản Thẩm Định Chiến Lược Kinh Doanh</title>");
            sb.AppendLine("<!--[if gte mso 9]><xml><w:WordDocument><w:View>Print</w:View><w:Zoom>100</w:Zoom><w:DoNotOptimizeForBrowser/></w:WordDocument></xml><![endif]-->");
            sb.AppendLine("<style>");
            sb.AppendLine("@page Section1 { size: 595.3pt 841.9pt; margin: 56.7pt 56.7pt 56.7pt 70.85pt; }");
            sb.AppendLine("div.Section1 { page: Section1; }");
            sb.AppendLine("body { font-family: 'Times New Roman', serif; font-size: 13pt; line-height: 1.35; color: #000; }");
            sb.AppendLine("h1 { font-size: 14pt; font-weight: bold; text-align: center; text-transform: uppercase; margin: 12pt 0 4pt 0; }");
            sb.AppendLine("h2 { font-size: 13pt; font-weight: bold; margin: 12pt 0 4pt 0; }");
            sb.AppendLine(".center { text-align: center; } .small { font-size: 10pt; font-style: italic; }");
            sb.AppendLine("table.grid { border-collapse: collapse; width: 100%; } table.grid th, table.grid td { border: 1px solid #000; padding: 4pt; font-size: 11pt; vertical-align: top; } table.grid th { background: #E7E6E6; }");
            sb.AppendLine("table.sig { width: 100%; margin-top: 24pt; } table.sig td { width: 50%; text-align: center; vertical-align: top; }");
            sb.AppendLine("</style></head><body><div class='Section1'>");

            sb.AppendLine("<p class='center'><strong>CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM</strong><br><strong>Độc lập – Tự do – Hạnh phúc</strong></p>");
            sb.AppendLine($"<p class='center'><em>Thời điểm lập: {DateTime.Now:dd/MM/yyyy HH:mm}</em> • Phiên quyết định: {E(s.SessionCode)}</p>");
            sb.AppendLine("<h1>BIÊN BẢN THẨM ĐỊNH VÀ PHÊ DUYỆT CHIẾN LƯỢC KINH DOANH</h1>");
            sb.AppendLine("<p class='center'><em>(Số liệu được tính trực tiếp từ cơ sở dữ liệu SQL Server DSS_SalesDB bằng phương pháp AHP – Saaty)</em></p>");

            sb.AppendLine("<h2>I. TRỌNG SỐ TIÊU CHÍ VÀ ĐỘ NHẤT QUÁN</h2>");
            sb.AppendLine("<table class='grid'><tr><th>Tiêu chí</th><th>Loại</th><th>Trọng số</th></tr>");
            foreach (var c in s.Ahp.Criteria.OrderByDescending(x => x.Weight))
                sb.AppendLine($"<tr><td>{E(c.Name)}</td><td>{(c.Type == "Cost" ? "Chi phí (càng thấp càng tốt)" : "Lợi ích (càng cao càng tốt)")}</td><td>{c.Weight * 100:0.00}%</td></tr>");
            sb.AppendLine("</table>");
            sb.AppendLine($"<p><strong>Kiểm tra nhất quán Saaty:</strong> {E(DssEngine.CrText(s.Ahp))}, λmax = {s.Ahp.LambdaMax:0.0000}, CI = {s.Ahp.CI:0.0000}, RI = {s.Ahp.RI:0.00} ⇒ {E(DssEngine.CrVerdict(s.Ahp))}.</p>");

            sb.AppendLine("<h2>II. KẾT QUẢ XẾP HẠNG PHƯƠNG ÁN</h2>");
            sb.AppendLine("<table class='grid'><tr><th>Hạng</th><th>Mã SKU</th><th>Phương án</th><th>Giá bán</th><th>Điểm AHP</th><th>Quyết định</th><th>Đề xuất nhập</th><th>Tồn kho</th></tr>");
            foreach (var a in s.Ranked)
                sb.AppendLine($"<tr><td align='center'>{a.Rank}</td><td>{E(a.Sku)}</td><td>{E(a.Name)}</td><td align='right'>{Money(a.Price)}</td><td align='center'>{a.Score:0.0000}</td><td>{E(DssEngine.OutcomeLabel(a.Outcome))}</td><td align='center'>{(a.Outcome == "Rejected" ? "—" : a.SuggestedQty.ToString())}</td><td>{E(DssEngine.StockText(a))}</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<h2>III. QUYẾT NGHỊ</h2>");
            sb.AppendLine($"<p><strong>1. Phương án được phê duyệt – {E(top.ShortName)} (điểm {top.Score:0.0000}):</strong></p><ul>");
            foreach (var l in BulletLines(DssEngine.DirectiveSelected(s))) sb.AppendLine($"<li>{E(l)}</li>");
            sb.AppendLine("</ul>");
            sb.AppendLine("<p><strong>2. Nhóm xem xét / theo dõi:</strong></p><ul>");
            foreach (var l in BulletLines(DssEngine.DirectiveReview(s))) sb.AppendLine($"<li>{E(l)}</li>");
            sb.AppendLine("</ul>");
            sb.AppendLine("<p><strong>3. Nhóm dừng nhập mới và giải phóng tồn kho:</strong></p><ul>");
            foreach (var l in BulletLines(DssEngine.DirectiveRejected(s))) sb.AppendLine($"<li>{E(l)}</li>");
            sb.AppendLine("</ul>");

            sb.AppendLine("<h2>IV. PHƯƠNG PHÁP VÀ THAM SỐ TÍNH TOÁN</h2><ul>");
            sb.AppendLine("<li>Điểm chuẩn hóa: tiêu chí lợi ích = giá trị / giá trị lớn nhất; tiêu chí chi phí = giá trị nhỏ nhất / giá trị. Điểm AHP = Σ (trọng số × điểm chuẩn hóa).</li>");
            sb.AppendLine($"<li>Phê duyệt = hạng 1; xem xét nếu điểm ≥ {DssEngine.ApproveThreshold:0.00}; còn lại loại bỏ. Ma trận chỉ được chấp nhận khi CR ≤ {DssEngine.CrLimit:0.00}.</li>");
            sb.AppendLine($"<li>Số lượng đề xuất nhập = nhu cầu/ngày × (thời gian giao hàng + {DssEngine.ReplenishCycleDays} ngày chu kỳ) + tồn tối thiểu − tồn hiện tại; nhu cầu/ngày = tổng bán đã hoàn tất / {s.HistoryDays} ngày dữ liệu lịch sử.</li>");
            sb.AppendLine("<li>Ngân sách marketing phân bổ theo tỷ trọng điểm AHP giữa các phương án được phê duyệt/xem xét.</li></ul>");

            sb.AppendLine("<table class='sig'><tr>");
            sb.AppendLine($"<td><strong>NGƯỜI LẬP BÁO CÁO</strong><br><em>(Ký, ghi rõ họ tên)</em><br><br><br><br><strong>{E(Author(userName, userRole))}</strong></td>");
            sb.AppendLine("<td><strong>NGƯỜI PHÊ DUYỆT</strong><br><em>(Ký, ghi rõ họ tên)</em><br><br><br><br>&nbsp;</td>");
            sb.AppendLine("</tr></table>");
            sb.AppendLine("</div></body></html>");
            return sb.ToString();
        }

        // ======================= HTML =======================
        public static string Html(DssSnapshot s, string? userName, string? userRole)
        {
            var sb = new StringBuilder();
            var top = s.Top!;
            int nSel = s.Ranked.Count(a => a.Outcome == "Selected");
            int nRev = s.Ranked.Count(a => a.Outcome == "UnderReview");
            int nRej = s.Ranked.Count(a => a.Outcome == "Rejected");

            sb.AppendLine("<!DOCTYPE html><html lang=\"vi\"><head><meta charset=\"UTF-8\">");
            sb.AppendLine("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0\">");
            sb.AppendLine("<title>Hồ Sơ Thẩm Định &amp; Phê Duyệt Chiến Lược Kinh Doanh - DSS Retail Studio 2026</title>");
            sb.AppendLine("<style>");
            sb.AppendLine(":root{--primary:#4F46E5;--ok:#10B981;--warn:#F59E0B;--bad:#EF4444;--ink:#0F172A;--muted:#64748B;--line:#E2E8F0;--bg:#F8FAFC}");
            sb.AppendLine("*{box-sizing:border-box;margin:0;padding:0}body{font-family:'Segoe UI',Roboto,Arial,sans-serif;background:var(--bg);color:var(--ink);line-height:1.6}");
            sb.AppendLine(".bar{position:sticky;top:0;background:#0F172A;color:#fff;padding:12px 24px;display:flex;justify-content:space-between;align-items:center}");
            sb.AppendLine(".bar button{background:var(--primary);color:#fff;border:0;padding:10px 20px;border-radius:8px;font-weight:700;cursor:pointer}");
            sb.AppendLine(".doc{max-width:1000px;margin:28px auto 40px;background:#fff;border:1px solid var(--line);border-radius:14px;padding:44px;box-shadow:0 10px 30px rgba(15,23,42,.06)}");
            sb.AppendLine("h1{font-size:22px;margin-bottom:4px}h2{font-size:16px;margin:26px 0 10px;color:var(--primary);text-transform:uppercase;letter-spacing:.4px}");
            sb.AppendLine(".meta{color:var(--muted);font-size:13px}.kpis{display:grid;grid-template-columns:repeat(4,1fr);gap:12px;margin:20px 0}");
            sb.AppendLine(".kpi{border:1px solid var(--line);border-radius:12px;padding:14px}.kpi small{color:var(--muted);font-weight:700;text-transform:uppercase;font-size:11px}.kpi b{display:block;font-size:17px;margin-top:4px}");
            sb.AppendLine("table{width:100%;border-collapse:collapse;font-size:13.5px}th{background:#F1F5F9;text-align:left}th,td{border:1px solid var(--line);padding:8px 10px}");
            sb.AppendLine(".pill{padding:2px 10px;border-radius:999px;font-weight:700;font-size:12px}.Selected{background:#D1FAE5;color:#065F46}.UnderReview{background:#FEF3C7;color:#92400E}.Rejected{background:#FEE2E2;color:#991B1B}");
            sb.AppendLine(".w{margin:8px 0}.w .t{height:10px;background:#E2E8F0;border-radius:6px}.w .b{height:10px;background:var(--primary);border-radius:6px}.w .h{display:flex;justify-content:space-between;font-size:13px;font-weight:600}");
            sb.AppendLine("ul{margin:6px 0 6px 22px}li{margin:4px 0}.sig{display:flex;gap:40px;margin-top:40px}.sig div{flex:1;text-align:center}");
            sb.AppendLine("@media print{.bar{display:none}body{background:#fff}.doc{border:0;box-shadow:none;margin:0}}");
            sb.AppendLine("</style></head><body>");
            sb.AppendLine("<div class=\"bar\"><span>● Báo cáo sinh trực tiếp từ SQL Server (DSS_SalesDB)</span><button onclick=\"window.print()\">🖨 In / Lưu PDF</button></div>");
            sb.AppendLine("<div class=\"doc\">");
            sb.AppendLine("<h1>HỒ SƠ THẨM ĐỊNH &amp; PHÊ DUYỆT CHIẾN LƯỢC KINH DOANH</h1>");
            sb.AppendLine($"<div class=\"meta\">Phiên quyết định: {E(s.SessionCode)} • Thời điểm lập: {DateTime.Now:dd/MM/yyyy HH:mm:ss} • {E(DssEngine.CrText(s.Ahp))}</div>");

            sb.AppendLine("<div class=\"kpis\">");
            sb.AppendLine($"<div class=\"kpi\"><small>Lựa chọn tối ưu</small><b>{E(top.ShortName)}</b><span class=\"meta\">Điểm AHP {top.Score:0.0000}</span></div>");
            sb.AppendLine($"<div class=\"kpi\"><small>Nhất quán Saaty</small><b>{E(DssEngine.CrText(s.Ahp))}</b><span class=\"meta\">{E(DssEngine.CrVerdict(s.Ahp))}</span></div>");
            sb.AppendLine($"<div class=\"kpi\"><small>Đề xuất nhập hạng 1</small><b>{top.SuggestedQty} sản phẩm</b><span class=\"meta\">Biên LN gộp {top.MarginPct:0.00}%</span></div>");
            sb.AppendLine($"<div class=\"kpi\"><small>Phân loại phương án</small><b>{nSel} chọn • {nRev} xem xét • {nRej} loại</b><span class=\"meta\">{s.Alts.Count} phương án</span></div>");
            sb.AppendLine("</div>");

            sb.AppendLine("<h2>1. Trọng số tiêu chí (từ ma trận Saaty trong CSDL)</h2>");
            foreach (var c in s.Ahp.Criteria.OrderByDescending(x => x.Weight))
                sb.AppendLine($"<div class=\"w\"><div class=\"h\"><span>{E(c.Name)}</span><span>{c.Weight * 100:0.00}%</span></div><div class=\"t\"><div class=\"b\" style=\"width:{(c.Weight * 100).ToString("0.00", System.Globalization.CultureInfo.InvariantCulture)}%\"></div></div></div>");

            sb.AppendLine("<h2>2. Bảng xếp hạng</h2>");
            sb.AppendLine("<table><tr><th>Hạng</th><th>Mã SKU</th><th>Phương án</th><th>Giá bán</th><th>Điểm AHP</th><th>Quyết định</th><th>Đề xuất nhập</th><th>Tồn kho</th></tr>");
            foreach (var a in s.Ranked)
                sb.AppendLine($"<tr><td>{a.Rank}</td><td>{E(a.Sku)}</td><td>{E(a.Name)}</td><td>{Money(a.Price)}</td><td><b>{a.Score:0.0000}</b></td><td><span class=\"pill {E(a.Outcome)}\">{E(DssEngine.OutcomeLabel(a.Outcome))}</span></td><td>{(a.Outcome == "Rejected" ? "—" : a.SuggestedQty.ToString())}</td><td>{E(DssEngine.StockText(a))}</td></tr>");
            sb.AppendLine("</table>");

            sb.AppendLine("<h2>3. Quyết nghị chiến lược</h2>");
            void Block(string title, string text)
            {
                sb.AppendLine($"<p><b>{E(title)}</b></p><ul>");
                foreach (var l in BulletLines(text)) sb.AppendLine($"<li>{E(l)}</li>");
                sb.AppendLine("</ul>");
            }
            Block($"Phê duyệt – {top.ShortName}", DssEngine.DirectiveSelected(s));
            Block("Nhóm xem xét / theo dõi", DssEngine.DirectiveReview(s));
            Block("Nhóm dừng nhập mới & xả tồn", DssEngine.DirectiveRejected(s));

            sb.AppendLine("<h2>4. Phương pháp &amp; tham số</h2><ul>");
            sb.AppendLine("<li>Chuẩn hóa: lợi ích = v / max; chi phí = min / v. Điểm AHP = Σ (trọng số × điểm chuẩn hóa).</li>");
            sb.AppendLine($"<li>Phê duyệt = hạng 1; xem xét nếu điểm ≥ {DssEngine.ApproveThreshold:0.00}; còn lại loại. Chấp nhận ma trận khi CR ≤ {DssEngine.CrLimit:0.00}.</li>");
            sb.AppendLine($"<li>Số lượng nhập = nhu cầu/ngày × (lead time + {DssEngine.ReplenishCycleDays} ngày) + tồn tối thiểu − tồn hiện tại (nhu cầu/ngày tính trên {s.HistoryDays} ngày dữ liệu bán hàng).</li></ul>");

            sb.AppendLine($"<div class=\"sig\"><div><b>NGƯỜI LẬP BÁO CÁO</b><br><i>(Ký, ghi rõ họ tên)</i><br><br><br><b>{E(Author(userName, userRole))}</b></div><div><b>NGƯỜI PHÊ DUYỆT</b><br><i>(Ký, ghi rõ họ tên)</i><br><br><br>&nbsp;</div></div>");
            sb.AppendLine("</div></body></html>");
            return sb.ToString();
        }

        // ======================= TEXT =======================
        public static string Text(DssSnapshot s, string? userName, string? userRole)
        {
            var sb = new StringBuilder();
            sb.AppendLine("============================================================");
            sb.AppendLine(" HỒ SƠ THẨM ĐỊNH & PHÊ DUYỆT CHIẾN LƯỢC KINH DOANH - DSS 2026");
            sb.AppendLine("============================================================");
            sb.AppendLine($"Phiên: {s.SessionCode}  |  Lập lúc: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            sb.AppendLine($"Người lập: {Author(userName, userRole)}");
            sb.AppendLine("Nguồn dữ liệu: SQL Server DSS_SalesDB (tính trực tiếp, không dùng số liệu cố định)");
            sb.AppendLine();
            sb.AppendLine("I. TRỌNG SỐ TIÊU CHÍ");
            foreach (var c in s.Ahp.Criteria.OrderByDescending(x => x.Weight))
                sb.AppendLine($"   - {c.Name} ({(c.Type == "Cost" ? "chi phí" : "lợi ích")}): {c.Weight * 100:0.00}%");
            sb.AppendLine($"   {DssEngine.CrText(s.Ahp)} | λmax = {s.Ahp.LambdaMax:0.0000} => {DssEngine.CrVerdict(s.Ahp)}");
            sb.AppendLine();
            sb.AppendLine("II. XẾP HẠNG");
            foreach (var a in s.Ranked)
                sb.AppendLine($"   #{a.Rank} {a.Name} [{a.Sku}] - điểm {a.Score:0.0000} - {DssEngine.OutcomeLabel(a.Outcome)} - giá {Money(a.Price)} - {DssEngine.StockText(a)}" +
                              (a.Outcome == "Rejected" ? "" : $" - đề xuất nhập {a.SuggestedQty}"));
            sb.AppendLine();
            sb.AppendLine("III. QUYẾT NGHỊ");
            sb.AppendLine(" * Phê duyệt:");
            foreach (var l in BulletLines(DssEngine.DirectiveSelected(s))) sb.AppendLine("     - " + l);
            sb.AppendLine(" * Xem xét / theo dõi:");
            foreach (var l in BulletLines(DssEngine.DirectiveReview(s))) sb.AppendLine("     - " + l);
            sb.AppendLine(" * Dừng nhập mới & xả tồn:");
            foreach (var l in BulletLines(DssEngine.DirectiveRejected(s))) sb.AppendLine("     - " + l);
            sb.AppendLine();
            sb.AppendLine($"Tham số: ngưỡng xem xét {DssEngine.ApproveThreshold:0.00}; CR tối đa {DssEngine.CrLimit:0.00}; chu kỳ nhập {DssEngine.ReplenishCycleDays} ngày; dữ liệu bán hàng {s.HistoryDays} ngày.");
            return sb.ToString();
        }
    }
}
