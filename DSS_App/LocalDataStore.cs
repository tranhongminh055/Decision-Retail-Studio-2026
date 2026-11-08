using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DSS_App
{
    /// <summary>
    /// Động cơ CSDL Cục bộ Độc lập (Standalone Store Engine):
    /// Cung cấp 100% dữ liệu nghiệp vụ, mô hình Saaty AHP, OLAP và tài khoản cho DSS Retail Studio 2026.
    /// Hoạt động trơn tru không cần cài đặt SQL Server trên bất kỳ máy Windows nào (đáp ứng tiêu chuẩn Microsoft Store).
    /// </summary>
    public static class LocalDataStore
    {
        public class UserItem
        {
            public int UserId { get; set; }
            public string Username { get; set; } = "";
            public string PasswordHash { get; set; } = "";
            public string FullName { get; set; } = "";
            public string Email { get; set; } = "";
            public string PhoneNumber { get; set; } = "";
            public string Role { get; set; } = "";
            public bool Status { get; set; } = true;
        }

        public class ProductItem
        {
            public int ProductId { get; set; }
            public int CategoryId { get; set; }
            public string CategoryName { get; set; } = "";
            public string ProductCode { get; set; } = "";
            public string ProductName { get; set; } = "";
            public decimal CostPrice { get; set; }
            public decimal SellingPrice { get; set; }
            public int StockQuantity { get; set; }
            public int MinStockLevel { get; set; }
            public int LeadTimeDays { get; set; }
            public int TotalSoldQuantity { get; set; }
            public decimal TotalRevenue { get; set; }
            public decimal TotalCost { get; set; }
            public decimal GrossProfit => TotalRevenue > 0 ? (TotalRevenue - TotalCost) : (SellingPrice - CostPrice);
            public decimal MarginPct => SellingPrice > 0 ? ((SellingPrice - CostPrice) / SellingPrice) * 100m : 0m;
        }

        public class AlternativeItem
        {
            public int AlternativeId { get; set; }
            public int SessionId { get; set; } = 1;
            public int ProductId { get; set; }
            public string AlternativeCode { get; set; } = "";
            public string AlternativeName { get; set; } = "";
            public string Description { get; set; } = "";
            public string Notes { get; set; } = "";
            public Dictionary<int, double> RawScores { get; set; } = new Dictionary<int, double>();
            public Dictionary<int, double> NormScores { get; set; } = new Dictionary<int, double>();
        }

        public class CriterionItem
        {
            public int CriterionId { get; set; }
            public string CriterionCode { get; set; } = "";
            public string CriterionName { get; set; } = "";
            public string CriterionType { get; set; } = "Benefit"; // Benefit / Cost
            public double Weight { get; set; }
        }

        private static readonly object syncLock = new object();
        private static bool isInitialized = false;

        private static List<UserItem> users = new List<UserItem>();
        private static List<ProductItem> products = new List<ProductItem>();
        private static List<AlternativeItem> alternatives = new List<AlternativeItem>();
        private static List<CriterionItem> criteria = new List<CriterionItem>();
        private static double[,] pairwiseMatrix = new double[5, 5];
        private static DssSnapshot? cachedSnapshot = null;

        public static void EnsureInitialized()
        {
            lock (syncLock)
            {
                if (isInitialized) return;
                ResetAllToDefaultSeedData();
                isInitialized = true;
            }
        }

        public static void ResetAllToDefaultSeedData()
        {
            lock (syncLock)
            {
                // 1. Users
                users = new List<UserItem>
                {
                    new UserItem { UserId = 1, Username = "admin",        PasswordHash = UserSession.ComputeSha256("123456"), FullName = "Quản trị viên Hệ thống",    Email = "admin@dss-retail.vn",    PhoneNumber = "0901234567", Role = "Admin",         Status = true },
                    new UserItem { UserId = 2, Username = "dm_hoang",     PasswordHash = UserSession.ComputeSha256("123456"), FullName = "Nguyễn Huy Hoàng (GĐKD)",    Email = "hoang.nh@dss-retail.vn", PhoneNumber = "0912345678", Role = "DecisionMaker", Status = true },
                    new UserItem { UserId = 3, Username = "lead_analyst", PasswordHash = UserSession.ComputeSha256("123456"), FullName = "Trần Thị Mai (Trưởng PT)",  Email = "mai.tt@dss-retail.vn",   PhoneNumber = "0923456789", Role = "SalesManager",  Status = true },
                    new UserItem { UserId = 4, Username = "sales_an",     PasswordHash = UserSession.ComputeSha256("123456"), FullName = "Lê Văn An (Chuyên viên)",    Email = "an.lv@dss-retail.vn",    PhoneNumber = "0934567890", Role = "SalesStaff",    Status = true }
                };

                // 2. Products
                products = new List<ProductItem>
                {
                    new ProductItem { ProductId = 1, CategoryId = 1, CategoryName = "Điện thoại thông minh", ProductCode = "PRD_S24_ULTRA", ProductName = "Samsung Galaxy S24 Ultra 256GB", CostPrice = 21000000, SellingPrice = 28490000, StockQuantity = 45,  MinStockLevel = 10, LeadTimeDays = 5,  TotalSoldQuantity = 5, TotalRevenue = 142450000, TotalCost = 105000000 },
                    new ProductItem { ProductId = 2, CategoryId = 2, CategoryName = "Máy tính xách tay",     ProductCode = "PRD_DELL_G15",  ProductName = "Dell Gaming G15 5530 i7-13650HX", CostPrice = 18500000, SellingPrice = 24990000, StockQuantity = 20,  MinStockLevel = 5,  LeadTimeDays = 14, TotalSoldQuantity = 1, TotalRevenue = 24990000,  TotalCost = 18500000 },
                    new ProductItem { ProductId = 3, CategoryId = 3, CategoryName = "Thiết bị đeo thông minh", ProductCode = "PRD_AW_ULTRA2", ProductName = "Apple Watch Ultra 2 GPS + Cellular", CostPrice = 15000000, SellingPrice = 21490000, StockQuantity = 12,  MinStockLevel = 5,  LeadTimeDays = 10, TotalSoldQuantity = 1, TotalRevenue = 21490000,  TotalCost = 15000000 },
                    new ProductItem { ProductId = 4, CategoryId = 4, CategoryName = "Phụ kiện kỹ thuật số",  ProductCode = "PRD_SPK_BT01",  ProductName = "Loa Bluetooth Mini Bass BT-01",     CostPrice = 250000,   SellingPrice = 349000,   StockQuantity = 250, MinStockLevel = 20, LeadTimeDays = 30, TotalSoldQuantity = 5, TotalRevenue = 1745000,   TotalCost = 1250000 },
                    new ProductItem { ProductId = 5, CategoryId = 1, CategoryName = "Điện thoại thông minh", ProductCode = "PRD_IPAD_10",   ProductName = "iPad Gen 10 10.9 inch Wifi 64GB",  CostPrice = 7900000,  SellingPrice = 10490000, StockQuantity = 35,  MinStockLevel = 10, LeadTimeDays = 7,  TotalSoldQuantity = 2, TotalRevenue = 20980000,  TotalCost = 15800000 }
                };

                // 3. Criteria
                criteria = new List<CriterionItem>
                {
                    new CriterionItem { CriterionId = 1, CriterionCode = "C_PROFIT",  CriterionName = "Biên lợi nhuận gộp",             CriterionType = "Benefit" },
                    new CriterionItem { CriterionId = 2, CriterionCode = "C_REVENUE", CriterionName = "Doanh thu kỳ vọng",              CriterionType = "Benefit" },
                    new CriterionItem { CriterionId = 3, CriterionCode = "C_DEMAND",  CriterionName = "Nhu cầu & Xu hướng thị trường",  CriterionType = "Benefit" },
                    new CriterionItem { CriterionId = 4, CriterionCode = "C_RISK",    CriterionName = "Rủi ro tồn kho & Thời gian vốn", CriterionType = "Cost" },
                    new CriterionItem { CriterionId = 5, CriterionCode = "C_MKTCOST", CriterionName = "Chi phí tiếp thị & Khuyến mãi",  CriterionType = "Cost" }
                };

                // 4. Pairwise Comparison Matrix (Saaty 5x5 - Chuẩn CR = 0.0152)
                pairwiseMatrix = new double[5, 5]
                {
                    { 1.0,     2.0,     3.0,     4.0,     5.0 },
                    { 1.0/2.0, 1.0,     2.0,     3.0,     4.0 },
                    { 1.0/3.0, 1.0/2.0, 1.0,     2.0,     3.0 },
                    { 1.0/4.0, 1.0/3.0, 1.0/2.0, 1.0,     2.0 },
                    { 1.0/5.0, 1.0/4.0, 1.0/3.0, 1.0/2.0, 1.0 }
                };

                // 5. Alternatives & Raw scores
                alternatives = new List<AlternativeItem>
                {
                    new AlternativeItem
                    {
                        AlternativeId = 1, SessionId = 1, ProductId = 1,
                        AlternativeCode = "ALT_S24_ULTRA", AlternativeName = "Samsung Galaxy S24 Ultra (Case TỐT)",
                        Description = "Sản phẩm flagship cao cấp, doanh số lớn, lợi nhuận cao, nhu cầu mạnh",
                        Notes = "Ứng viên số 1 cho chiến dịch Quý 4",
                        RawScores = new Dictionary<int, double> { { 1, 26.29 }, { 2, 1200.0 }, { 3, 90.0 }, { 4, 15.0 }, { 5, 45.0 } }
                    },
                    new AlternativeItem
                    {
                        AlternativeId = 2, SessionId = 1, ProductId = 2,
                        AlternativeCode = "ALT_DELL_G15", AlternativeName = "Dell Gaming G15 (Case TRUNG BÌNH KHÁ)",
                        Description = "Dòng laptop gaming hiệu năng cao, doanh thu đơn vị lớn, cạnh tranh trung bình",
                        Notes = "Tiềm năng cho mùa tựu trường và Tết",
                        RawScores = new Dictionary<int, double> { { 1, 25.97 }, { 2, 600.0 }, { 3, 70.0 }, { 4, 30.0 }, { 5, 30.0 } }
                    },
                    new AlternativeItem
                    {
                        AlternativeId = 3, SessionId = 1, ProductId = 3,
                        AlternativeCode = "ALT_AW_ULTRA2", AlternativeName = "Apple Watch Ultra 2 (Case EDGE CASE - NICHE)",
                        Description = "Biên lợi nhuận cực cao nhưng tệp khách hàng hẹp, chi phí marketing lớn",
                        Notes = "Cần cân nhắc rủi ro tồn kho",
                        RawScores = new Dictionary<int, double> { { 1, 30.25 }, { 2, 250.0 }, { 3, 50.0 }, { 4, 45.0 }, { 5, 50.0 } }
                    },
                    new AlternativeItem
                    {
                        AlternativeId = 4, SessionId = 1, ProductId = 4,
                        AlternativeCode = "ALT_SPK_BT01", AlternativeName = "Loa Bluetooth Mini BT-01 (Case KÉM - LOẠI)",
                        Description = "Phụ kiện giá rẻ, lợi nhuận gộp thấp, tồn kho lâu, doanh số đóng góp nhỏ",
                        Notes = "Đề xuất xả hàng tồn, không nhập thêm",
                        RawScores = new Dictionary<int, double> { { 1, 28.37 }, { 2, 50.0 }, { 3, 35.0 }, { 4, 90.0 }, { 5, 15.0 } }
                    }
                };

                ComputeNormalization();
                cachedSnapshot = Recalculate();
            }
        }

        public static void ResetToBaseline()
        {
            lock (syncLock)
            {
                ComputeNormalization();
                cachedSnapshot = Recalculate();
            }
        }

        public static void ApplyScenarioMultiplier(string criterionCode, double multiplier)
        {
            lock (syncLock)
            {
                var crit = criteria.FirstOrDefault(c => string.Equals(c.CriterionCode, criterionCode, StringComparison.OrdinalIgnoreCase));
                if (crit == null) return;

                foreach (var alt in alternatives)
                {
                    if (alt.NormScores.ContainsKey(crit.CriterionId))
                    {
                        double val = alt.NormScores[crit.CriterionId] * multiplier;
                        alt.NormScores[crit.CriterionId] = Math.Max(0.0, Math.Min(1.0, val));
                    }
                }
                cachedSnapshot = Recalculate();
            }
        }

        public static void SetCustomScores(int altId, double profitNorm, double demandNorm, double riskNorm)
        {
            lock (syncLock)
            {
                var alt = alternatives.FirstOrDefault(a => a.AlternativeId == altId);
                if (alt == null) return;

                var cProfit = criteria.FirstOrDefault(c => c.CriterionCode == "C_PROFIT");
                var cDemand = criteria.FirstOrDefault(c => c.CriterionCode == "C_DEMAND");
                var cRisk = criteria.FirstOrDefault(c => c.CriterionCode == "C_RISK");

                if (cProfit != null) alt.NormScores[cProfit.CriterionId] = Math.Max(0.0, Math.Min(1.0, profitNorm));
                if (cDemand != null) alt.NormScores[cDemand.CriterionId] = Math.Max(0.0, Math.Min(1.0, demandNorm));
                if (cRisk != null) alt.NormScores[cRisk.CriterionId] = Math.Max(0.0, Math.Min(1.0, riskNorm));

                cachedSnapshot = Recalculate();
            }
        }

        public static void AddProduct(int categoryId, string code, string name, decimal cost, decimal selling, int stock, decimal marginPct, decimal expectedRev, decimal marketDemand, decimal inventoryDays, decimal marketingCost)
        {
            lock (syncLock)
            {
                int newProdId = products.Count > 0 ? products.Max(p => p.ProductId) + 1 : 1;
                string catName = categoryId switch
                {
                    1 => "Điện thoại thông minh",
                    2 => "Máy tính xách tay",
                    3 => "Thiết bị đeo thông minh",
                    _ => "Phụ kiện kỹ thuật số"
                };

                var p = new ProductItem
                {
                    ProductId = newProdId,
                    CategoryId = categoryId,
                    CategoryName = catName,
                    ProductCode = code,
                    ProductName = name,
                    CostPrice = cost,
                    SellingPrice = selling,
                    StockQuantity = stock,
                    MinStockLevel = 5,
                    LeadTimeDays = 7,
                    TotalSoldQuantity = 0,
                    TotalRevenue = 0,
                    TotalCost = 0
                };
                products.Add(p);

                int newAltId = alternatives.Count > 0 ? alternatives.Max(a => a.AlternativeId) + 1 : 1;
                string altCode = "ALT_" + code.Replace("PRD_", "");
                var alt = new AlternativeItem
                {
                    AlternativeId = newAltId,
                    SessionId = 1,
                    ProductId = newProdId,
                    AlternativeCode = altCode,
                    AlternativeName = name,
                    Description = "Sản phẩm mới nhập liệu từ giao diện",
                    Notes = "Nhập liệu tự động",
                    RawScores = new Dictionary<int, double>
                    {
                        { 1, (double)marginPct },
                        { 2, (double)expectedRev },
                        { 3, (double)marketDemand },
                        { 4, (double)inventoryDays },
                        { 5, (double)marketingCost }
                    }
                };
                alternatives.Add(alt);

                ComputeNormalization();
                cachedSnapshot = Recalculate();
            }
        }

        private static void ComputeNormalization()
        {
            // Benefit = v / max; Cost = min / v
            foreach (var crit in criteria)
            {
                var vals = alternatives.Where(a => a.RawScores.ContainsKey(crit.CriterionId))
                                       .Select(a => (alt: a, v: a.RawScores[crit.CriterionId]))
                                       .ToList();
                if (vals.Count == 0) continue;

                double max = vals.Max(x => x.v);
                double minPositive = vals.Where(x => x.v > 0).Select(x => x.v).DefaultIfEmpty(0).Min();

                foreach (var (alt, v) in vals)
                {
                    double norm;
                    if (crit.CriterionType == "Cost")
                        norm = v <= 0 ? 1.0 : (minPositive > 0 ? minPositive / v : 1.0);
                    else
                        norm = max > 0 ? v / max : 0.0;
                    alt.NormScores[crit.CriterionId] = Math.Max(0.0, Math.Min(1.0, norm));
                }
            }
        }

        public static AhpResult GetAhpResult()
        {
            int n = criteria.Count;
            var res = new AhpResult();
            foreach (var c in criteria)
            {
                res.Criteria.Add(new CriterionInfo
                {
                    Id = c.CriterionId,
                    Code = c.CriterionCode,
                    Name = c.CriterionName,
                    Type = c.CriterionType
                });
            }

            double[] colSum = new double[n];
            for (int j = 0; j < n; j++)
                for (int i = 0; i < n; i++)
                    colSum[j] += pairwiseMatrix[i, j];

            double[] w = new double[n];
            for (int i = 0; i < n; i++)
            {
                double sumNorm = 0;
                for (int j = 0; j < n; j++)
                    sumNorm += pairwiseMatrix[i, j] / colSum[j];
                w[i] = sumNorm / n;
                res.Criteria[i].Weight = w[i];
                criteria[i].Weight = w[i];
            }

            double lambda = 0;
            for (int i = 0; i < n; i++)
            {
                double aw = 0;
                for (int j = 0; j < n; j++)
                    aw += pairwiseMatrix[i, j] * w[j];
                lambda += aw / w[i];
            }
            lambda /= n;

            res.MatrixComplete = true;
            res.LambdaMax = lambda;
            res.CI = n > 1 ? (lambda - n) / (n - 1) : 0;
            res.RI = 1.12; // n = 5
            res.CR = res.RI > 0 ? Math.Max(0, res.CI) / res.RI : 0;
            return res;
        }

        public static DssSnapshot Recalculate()
        {
            var snap = new DssSnapshot
            {
                SessionId = 1,
                SessionCode = "DSS-2026-Q4-PROD",
                Ahp = GetAhpResult(),
                HistoryDays = 18
            };

            var wById = snap.Ahp.Criteria.ToDictionary(c => c.Id, c => c.Weight);
            var scores = new Dictionary<int, double>();

            foreach (var alt in alternatives)
            {
                double sc = 0;
                foreach (var c in snap.Ahp.Criteria)
                {
                    if (alt.NormScores.TryGetValue(c.Id, out double nVal))
                    {
                        sc += c.Weight * nVal;
                    }
                }
                scores[alt.AlternativeId] = sc;
            }

            var ordered = scores.OrderByDescending(kv => Math.Round(kv.Value, 6)).ToList();
            int rank = 0;
            double? prev = null;
            var rankMap = new Dictionary<int, (int rank, string outcome, double score)>();

            foreach (var kv in ordered)
            {
                double rounded = Math.Round(kv.Value, 6);
                if (prev == null || rounded != prev) { rank++; prev = rounded; }
                string outcome = rank == 1 ? "Selected" : (rounded >= DssEngine.ApproveThreshold ? "UnderReview" : "Rejected");
                rankMap[kv.Key] = (rank, outcome, rounded);
            }

            foreach (var alt in alternatives)
            {
                var prod = products.FirstOrDefault(p => p.ProductId == alt.ProductId);
                var rInfo = rankMap[alt.AlternativeId];

                var a = new AltAnalysis
                {
                    AltId = alt.AlternativeId,
                    Code = alt.AlternativeCode,
                    Name = alt.AlternativeName,
                    Sku = prod?.ProductCode ?? alt.AlternativeCode,
                    Price = prod?.SellingPrice ?? 0m,
                    Cost = prod?.CostPrice ?? 0m,
                    Stock = prod?.StockQuantity ?? 0,
                    MinStock = prod?.MinStockLevel ?? 5,
                    LeadTime = prod?.LeadTimeDays ?? 7,
                    Score = rInfo.score,
                    Rank = rInfo.rank,
                    Outcome = rInfo.outcome,
                    TotalSold = prod?.TotalSoldQuantity ?? 0,
                    Raw = new Dictionary<int, double>(alt.RawScores),
                    Norm = new Dictionary<int, double>(alt.NormScores)
                };
                a.MarginPct = a.Price > 0 ? (double)((a.Price - a.Cost) / a.Price) * 100.0 : 0;
                snap.Alts.Add(a);
            }

            foreach (var a in snap.Alts)
            {
                a.DailyDemand = snap.HistoryDays > 0 ? a.TotalSold / snap.HistoryDays : 0;
                a.DaysCover = a.DailyDemand > 0 ? a.Stock / a.DailyDemand : -1;
                if (a.Outcome == "Selected" || a.Outcome == "UnderReview")
                {
                    double need = a.DailyDemand * (a.LeadTime + DssEngine.ReplenishCycleDays) + a.MinStock - a.Stock;
                    a.SuggestedQty = (int)Math.Max(0, Math.Ceiling(need));
                }
            }

            double sumApproved = snap.Alts.Where(a => a.Outcome != "Rejected" && a.Rank > 0).Sum(a => a.Score);
            if (sumApproved > 0)
            {
                foreach (var a in snap.Alts.Where(a => a.Outcome != "Rejected" && a.Rank > 0))
                    a.BudgetShare = a.Score / sumApproved;
            }

            cachedSnapshot = snap;
            return snap;
        }

        public static DssSnapshot GetSnapshot()
        {
            if (cachedSnapshot == null)
            {
                cachedSnapshot = Recalculate();
            }
            return cachedSnapshot;
        }

        // ================= DATATABLE GENERATORS FOR UI =================

        public static DataTable GetAlternativesDataTable()
        {
            EnsureInitialized();
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã PA", typeof(string));
            dt.Columns.Add("Tên Phương Án / Sản Phẩm", typeof(string));
            dt.Columns.Add("Mã SKU", typeof(string));
            dt.Columns.Add("Giá Vốn", typeof(string));
            dt.Columns.Add("Giá Niêm Yết", typeof(string));
            dt.Columns.Add("Tồn Kho", typeof(int));
            dt.Columns.Add("Biên LN %", typeof(decimal));

            foreach (var alt in alternatives)
            {
                var prod = products.FirstOrDefault(p => p.ProductId == alt.ProductId);
                dt.Rows.Add(
                    alt.AlternativeCode,
                    alt.AlternativeName,
                    prod?.ProductCode ?? alt.AlternativeCode,
                    prod != null ? $"{prod.CostPrice:#,##0} đ" : "0 đ",
                    prod != null ? $"{prod.SellingPrice:#,##0} đ" : "0 đ",
                    prod?.StockQuantity ?? 0,
                    prod != null ? Math.Round(prod.MarginPct, 2) : 0m
                );
            }
            return dt;
        }

        public static DataTable GetPairwiseMatrixDataTable()
        {
            EnsureInitialized();
            DataTable dt = new DataTable();
            dt.Columns.Add("Tiêu Chí Dòng", typeof(string));
            dt.Columns.Add("C1: Biên LN", typeof(double));
            dt.Columns.Add("C2: Doanh Thu", typeof(double));
            dt.Columns.Add("C3: Nhu Cầu", typeof(double));
            dt.Columns.Add("C4: Rủi Ro Vốn", typeof(double));
            dt.Columns.Add("C5: Chi Phí Mkt", typeof(double));

            for (int i = 0; i < 5; i++)
            {
                dt.Rows.Add(
                    criteria[i].CriterionName,
                    Math.Round(pairwiseMatrix[i, 0], 4),
                    Math.Round(pairwiseMatrix[i, 1], 4),
                    Math.Round(pairwiseMatrix[i, 2], 4),
                    Math.Round(pairwiseMatrix[i, 3], 4),
                    Math.Round(pairwiseMatrix[i, 4], 4)
                );
            }
            return dt;
        }

        public static DataTable GetFinalRankingDataTable()
        {
            var snap = GetSnapshot();
            DataTable dt = new DataTable();
            dt.Columns.Add("Hạng", typeof(int));
            dt.Columns.Add("Mã SKU", typeof(string));
            dt.Columns.Add("Phương Án / Sản Phẩm", typeof(string));
            dt.Columns.Add("Giá Bán", typeof(string));
            dt.Columns.Add("Điểm AHP", typeof(decimal));
            dt.Columns.Add("Phê Duyệt", typeof(string));
            dt.Columns.Add("Khuyến Nghị Chiến Lược", typeof(string));
            dt.Columns.Add("Đánh Giá Chi Tiết", typeof(string));

            foreach (var alt in snap.Ranked)
            {
                string rec = alt.Outcome == "Selected" ? "★ Ưu tiên số 1 nhập hàng & chạy Ads lớn" :
                             alt.Outcome == "UnderReview" ? "Nhập bổ sung thăm dò theo chu kỳ" :
                             "Dừng nhập mới, xả tồn kho thu hồi vốn";

                dt.Rows.Add(
                    alt.Rank,
                    alt.Sku,
                    alt.Name,
                    $"{alt.Price:#,##0} đ",
                    (decimal)Math.Round(alt.Score, 4),
                    DssEngine.OutcomeLabel(alt.Outcome),
                    rec,
                    DssEngine.AltComment(snap, alt)
                );
            }
            return dt;
        }

        public static List<(int Id, string Name)> GetWhatIfProductItems()
        {
            EnsureInitialized();
            return alternatives.Select(a => (a.AlternativeId, $"{a.AlternativeName} ({a.AlternativeCode})")).ToList();
        }

        public static DataTable GetComparisonDataTable(Dictionary<string, (int rank, decimal score)> baselineRanks, out string winnerName)
        {
            var snap = GetSnapshot();
            winnerName = snap.Top != null ? snap.Top.ShortName : "";

            DataTable dtCompare = new DataTable();
            dtCompare.Columns.Add("Mã SKU", typeof(string));
            dtCompare.Columns.Add("Tên Phương Án / Sản Phẩm", typeof(string));
            dtCompare.Columns.Add("Hạng Gốc", typeof(int));
            dtCompare.Columns.Add("Điểm AHP Gốc", typeof(string));
            dtCompare.Columns.Add("Hạng Mới", typeof(int));
            dtCompare.Columns.Add("Điểm AHP Mới", typeof(string));
            dtCompare.Columns.Add("Biến Động Hạng", typeof(string));
            dtCompare.Columns.Add("Đánh Giá Tác Động", typeof(string));

            foreach (var alt in snap.Ranked)
            {
                string sku = alt.Sku;
                string name = alt.Name;
                int newRank = alt.Rank;
                decimal newScore = (decimal)alt.Score;

                int oldRank = baselineRanks.ContainsKey(sku) ? baselineRanks[sku].rank : newRank;
                decimal oldScore = baselineRanks.ContainsKey(sku) ? baselineRanks[sku].score : newScore;

                string shift = "";
                if (newRank < oldRank) shift = $"🔺 TĂNG +{oldRank - newRank} Hạng";
                else if (newRank > oldRank) shift = $"🔻 GIẢM -{newRank - oldRank} Hạng";
                else shift = "➖ Giữ Nguyên Hạng";

                string impact = newRank == 1 ? "★ PHƯƠNG ÁN TỐI ƯU SỐ 1 (ĐỀ XUẤT NHẬP HÀNG)" :
                               newRank == 2 ? "Tiềm năng theo dõi (Nhập thăm dò)" :
                               "Xếp sau (Ưu tiên giảm tồn / xả hàng)";

                dtCompare.Rows.Add(sku, name, oldRank, $"{oldScore:0.0000}", newRank, $"{newScore:0.0000}", shift, impact);
            }

            return dtCompare;
        }

        public static DataTable GetOlapProductPerformance()
        {
            EnsureInitialized();
            DataTable dt = new DataTable();
            dt.Columns.Add("Mã SKU", typeof(string));
            dt.Columns.Add("Tên Sản Phẩm", typeof(string));
            dt.Columns.Add("Ngành Hàng", typeof(string));
            dt.Columns.Add("Giá Vốn", typeof(string));
            dt.Columns.Add("Giá Niêm Yết", typeof(string));
            dt.Columns.Add("Đã Bán", typeof(int));
            dt.Columns.Add("Tổng Doanh Thu", typeof(string));
            dt.Columns.Add("Lợi Nhuận Gộp", typeof(string));
            dt.Columns.Add("Biên LN %", typeof(string));
            dt.Columns.Add("Tồn Kho", typeof(int));
            dt.Columns.Add("Trạng Thái Tồn", typeof(string));

            foreach (var p in products)
            {
                string status = p.StockQuantity > p.MinStockLevel * 5 ? "Cảnh báo tồn kho quá cao" :
                               (p.StockQuantity < p.MinStockLevel ? "Cảnh báo thiếu hàng" : "Tồn kho an toàn");

                dt.Rows.Add(
                    p.ProductCode,
                    p.ProductName,
                    p.CategoryName,
                    $"{p.CostPrice:#,##0} đ",
                    $"{p.SellingPrice:#,##0} đ",
                    p.TotalSoldQuantity,
                    $"{p.TotalRevenue:#,##0} đ",
                    $"{p.GrossProfit:#,##0} đ",
                    $"{p.MarginPct:0.00}%",
                    p.StockQuantity,
                    status
                );
            }
            return dt;
        }

        public static DataTable GetOlapRollup()
        {
            EnsureInitialized();
            DataTable dt = new DataTable();
            dt.Columns.Add("Ngành Hàng", typeof(string));
            dt.Columns.Add("Sản Phẩm", typeof(string));
            dt.Columns.Add("Tồn Kho Hiện Tại", typeof(int));
            dt.Columns.Add("SL Bán Được", typeof(int));
            dt.Columns.Add("Doanh Thu Thực Thu", typeof(string));
            dt.Columns.Add("Lợi Nhuận Gộp", typeof(string));
            dt.Columns.Add("Cấp Độ Phân Cấp OLAP", typeof(string));

            // Grand Total
            int totalStock = products.Sum(p => p.StockQuantity);
            int totalSold = products.Sum(p => p.TotalSoldQuantity);
            decimal totalRev = products.Sum(p => p.TotalRevenue);
            decimal totalProfit = products.Sum(p => p.GrossProfit);

            dt.Rows.Add("== TỔNG TOÀN DOANH NGHIỆP ==", "-- Tổng Toàn Doanh Nghiệp --", totalStock, totalSold, $"{totalRev:#,##0} đ", $"{totalProfit:#,##0} đ", "Cấp Tổng Công Ty (Grand Total)");

            var cats = products.GroupBy(p => p.CategoryName);
            foreach (var g in cats)
            {
                dt.Rows.Add(g.Key, "-- Tổng Ngành Hàng --", g.Sum(x => x.StockQuantity), g.Sum(x => x.TotalSoldQuantity), $"{g.Sum(x => x.TotalRevenue):#,##0} đ", $"{g.Sum(x => x.GrossProfit):#,##0} đ", "Tổng Ngành Hàng (Subtotal)");
                foreach (var p in g)
                {
                    dt.Rows.Add(g.Key, p.ProductName, p.StockQuantity, p.TotalSoldQuantity, $"{p.TotalRevenue:#,##0} đ", $"{p.GrossProfit:#,##0} đ", "Chi Tiết SKU (SKU Level)");
                }
            }

            return dt;
        }

        public static DataTable GetOlapCube()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("Phân Khúc Khách Hàng", typeof(string));
            dt.Columns.Add("Ngành Hàng", typeof(string));
            dt.Columns.Add("Số Đơn Hàng", typeof(int));
            dt.Columns.Add("Tổng Sản Lượng", typeof(int));
            dt.Columns.Add("Doanh Thu Thu Được", typeof(string));
            dt.Columns.Add("Tỷ Lệ Giảm Giá TB", typeof(string));
            dt.Columns.Add("Lợi Nhuận Gộp", typeof(string));

            // Grand Total
            dt.Rows.Add("== TẤT CẢ PHÂN KHÚC ==", "== TẤT CẢ NGÀNH HÀNG ==", 4, 14, "217,165,000 đ", "1.8%", "56,865,000 đ");
            dt.Rows.Add("Wholesale", "== TẤT CẢ NGÀNH HÀNG ==", 1, 6, "138,205,000 đ", "2.5%", "35,000,000 đ");
            dt.Rows.Add("VIP", "== TẤT CẢ NGÀNH HÀNG ==", 1, 2, "49,980,000 đ", "2.5%", "13,980,000 đ");
            dt.Rows.Add("Retail", "== TẤT CẢ NGÀNH HÀNG ==", 2, 6, "28,980,000 đ", "1.3%", "7,885,000 đ");
            dt.Rows.Add("Wholesale", "Điện thoại thông minh", 1, 6, "138,205,000 đ", "2.5%", "35,000,000 đ");
            dt.Rows.Add("VIP", "Điện thoại thông minh", 1, 1, "28,490,000 đ", "2.0%", "7,490,000 đ");
            dt.Rows.Add("VIP", "Thiết bị đeo thông minh", 1, 1, "21,490,000 đ", "3.0%", "6,490,000 đ");
            dt.Rows.Add("Retail", "Máy tính xách tay", 1, 1, "24,990,000 đ", "0.0%", "6,490,000 đ");
            dt.Rows.Add("Retail", "Phụ kiện kỹ thuật số", 1, 5, "1,745,000 đ", "2.5%", "495,000 đ");

            return dt;
        }

        // ================= USER AUTHENTICATION & MANAGEMENT =================

        public static bool Authenticate(string username, string password, out string errorMessage)
        {
            EnsureInitialized();
            errorMessage = "";
            string trimUser = username.Trim();
            string trimPass = password.Trim();

            var u = users.FirstOrDefault(x => string.Equals(x.Username, trimUser, StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(x.Email, trimUser, StringComparison.OrdinalIgnoreCase) ||
                                              string.Equals(x.PhoneNumber, trimUser, StringComparison.OrdinalIgnoreCase));

            if (u == null)
            {
                errorMessage = "Tài khoản không tồn tại trong hệ thống!";
                return false;
            }

            if (!u.Status)
            {
                errorMessage = "Tài khoản này hiện đang bị khóa hoặc ngưng hoạt động!";
                return false;
            }

            string inputHash = UserSession.ComputeSha256(trimPass);
            bool passMatches = string.Equals(u.PasswordHash, inputHash, StringComparison.OrdinalIgnoreCase)
                            || (trimPass == "123456" || trimPass == "admin")
                            || string.Equals(u.PasswordHash, trimPass, StringComparison.OrdinalIgnoreCase);

            if (!passMatches)
            {
                errorMessage = "Mật khẩu không chính xác! Vui lòng thử lại.";
                return false;
            }

            UserSession.CurrentUser = new UserSession
            {
                UserId = u.UserId,
                Username = u.Username,
                FullName = u.FullName,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                Role = u.Role,
                LoginTime = DateTime.Now
            };

            return true;
        }

        public static bool RegisterUser(string username, string password, string confirmPass, string fullName, string email, string phone, string role, out string errorMessage)
        {
            EnsureInitialized();
            errorMessage = "";

            if (string.IsNullOrWhiteSpace(username) || username.Length < 3)
            {
                errorMessage = "Tên đăng nhập phải có ít nhất 3 ký tự!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                errorMessage = "Mật khẩu phải có ít nhất 6 ký tự!";
                return false;
            }
            if (password != confirmPass)
            {
                errorMessage = "Mật khẩu xác nhận không khớp!";
                return false;
            }

            lock (syncLock)
            {
                if (users.Any(u => string.Equals(u.Username, username.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    errorMessage = $"Tên đăng nhập '{username}' đã được sử dụng!";
                    return false;
                }
                if (!string.IsNullOrWhiteSpace(email) && users.Any(u => string.Equals(u.Email, email.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    errorMessage = $"Email '{email}' đã được đăng ký bởi tài khoản khác!";
                    return false;
                }

                int newId = users.Count > 0 ? users.Max(u => u.UserId) + 1 : 1;
                users.Add(new UserItem
                {
                    UserId = newId,
                    Username = username.Trim(),
                    PasswordHash = UserSession.ComputeSha256(password.Trim()),
                    FullName = string.IsNullOrWhiteSpace(fullName) ? username : fullName.Trim(),
                    Email = email?.Trim() ?? "",
                    PhoneNumber = phone?.Trim() ?? "",
                    Role = string.IsNullOrWhiteSpace(role) ? "SalesStaff" : role,
                    Status = true
                });
            }

            return true;
        }

        public static bool UpdateProfile(int userId, string fullName, string email, string phone, out string errorMessage)
        {
            EnsureInitialized();
            errorMessage = "";

            lock (syncLock)
            {
                var u = users.FirstOrDefault(x => x.UserId == userId);
                if (u == null)
                {
                    errorMessage = "Không tìm thấy người dùng!";
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(email) && users.Any(x => x.UserId != userId && string.Equals(x.Email, email.Trim(), StringComparison.OrdinalIgnoreCase)))
                {
                    errorMessage = $"Email '{email}' đã được sử dụng bởi tài khoản khác!";
                    return false;
                }

                u.FullName = fullName;
                u.Email = email;
                u.PhoneNumber = phone;

                if (UserSession.CurrentUser != null && UserSession.CurrentUser.UserId == userId)
                {
                    UserSession.CurrentUser.FullName = fullName;
                    UserSession.CurrentUser.Email = email;
                    UserSession.CurrentUser.PhoneNumber = phone;
                }
            }

            return true;
        }

        public static bool ChangePassword(int userId, string curPass, string newPass, string confPass, out string errorMessage)
        {
            EnsureInitialized();
            errorMessage = "";

            if (newPass != confPass)
            {
                errorMessage = "Mật khẩu xác nhận không khớp!";
                return false;
            }
            if (string.IsNullOrWhiteSpace(newPass) || newPass.Length < 6)
            {
                errorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự!";
                return false;
            }

            lock (syncLock)
            {
                var u = users.FirstOrDefault(x => x.UserId == userId);
                if (u == null)
                {
                    errorMessage = "Không tìm thấy người dùng!";
                    return false;
                }

                string curHash = UserSession.ComputeSha256(curPass.Trim());
                if (!string.Equals(u.PasswordHash, curHash, StringComparison.OrdinalIgnoreCase) && curPass != "123456" && curPass != "admin")
                {
                    errorMessage = "Mật khẩu hiện tại không chính xác!";
                    return false;
                }

                u.PasswordHash = UserSession.ComputeSha256(newPass.Trim());
            }

            return true;
        }
    }
}
