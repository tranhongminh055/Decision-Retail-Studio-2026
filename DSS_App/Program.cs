namespace DSS_App;

static class Program
{
    /// <summary>
    
    /// </summary>
    [STAThread]
    static int Main(string[] args)
    {
        if (args != null && args.Contains("--test-standalone"))
        {
            DatabaseGuard.IsStandaloneMode = true;
            LocalDataStore.EnsureInitialized();
            bool authOk = LocalDataStore.Authenticate("admin", "123456", out string authErr);
            var ahp = LocalDataStore.GetAhpResult();
            var snap = LocalDataStore.Recalculate();
            var dtRank = LocalDataStore.GetFinalRankingDataTable();
            var dtOlap = LocalDataStore.GetOlapProductPerformance();
            Console.WriteLine($"[TEST] Standalone Auth: {authOk} (User: {UserSession.CurrentUser?.FullName})");
            Console.WriteLine($"[TEST] AHP CR: {ahp.CR:0.0000}, Consistent: {ahp.IsConsistent}");
            Console.WriteLine($"[TEST] Top Rank: {snap.Top?.ShortName} (Score: {snap.Top?.Score:0.0000})");
            Console.WriteLine($"[TEST] Ranking Rows: {dtRank.Rows.Count}");
            Console.WriteLine($"[TEST] OLAP Rows: {dtOlap.Rows.Count}");
            return (authOk && ahp.IsConsistent && snap.Top != null && dtRank.Rows.Count == 4) ? 0 : 1;
        }

        try
        {
            Application.ThreadException += (s, e) =>
            {
                if (e.Exception is System.Runtime.InteropServices.ExternalException) return;
            };
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            ApplicationConfiguration.Initialize();

           
            using (SplashForm splash = new SplashForm())
            {
                if (splash.ShowDialog() != DialogResult.OK)
                {
                    return 0;
                }
            }

            using (LoginForm login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                {
                    return 0;
                }
            }

            Application.Run(new Form1());
            return 0;
        }
        catch (System.Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(ex.ToString(), "Lỗi khởi động", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            return 1;
        }
    }    
}