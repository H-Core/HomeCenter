using System.Windows;

namespace HomeCenter.NET
{
    public partial class SettingsWindow
    {
        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            Close();
        }

        private void Close(object sender, RoutedEventArgs e) => Close();
    }
}
