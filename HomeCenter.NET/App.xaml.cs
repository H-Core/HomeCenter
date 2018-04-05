using System.Windows;

namespace HomeCenter.NET
{
    public partial class App
    {
        private void App_Startup(object sender, StartupEventArgs e)
        {
            var window = new Windows.MainWindow();

            window.Load();
        }
    }
}
