using System.Windows;

namespace HomeCenter.NET
{
    public partial class App
    {
        private Windows.MainWindow Window { get; set; }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Window = new Windows.MainWindow();

            Window.Load();
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            Window?.Dispose();
            Window = null;
        }
    }
}
