using System.Windows;

namespace HomeCenter.NET
{
    public partial class SettingsWindow
    {
        #region Properties

        public SettingsWindow()
        {
            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            Close();
        }

        private void Close(object sender, RoutedEventArgs e) => Close();

        #endregion

    }
}
