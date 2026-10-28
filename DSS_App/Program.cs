namespace DSS_App;

static class Program
{
    /// <summary>
    
    /// </summary>
    [STAThread]
    static void Main()
    {
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
                    return;
                }
            }

            using (LoginForm login = new LoginForm())
            {
                if (login.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
            }

            Application.Run(new Form1());
        }
        catch (System.Exception ex)
        {
            System.Windows.Forms.MessageBox.Show(ex.ToString(), "Lỗi khởi động", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
        }
    }    
}