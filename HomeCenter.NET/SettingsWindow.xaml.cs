using System.Windows;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET
{
    public partial class SettingsWindow
    {
        #region Properties

        public SettingsWindow()
        {
            InitializeComponent();

            StartupCheckBox.IsChecked = Startup.IsStartup(Options.FileName);
        }

        #endregion

        #region Event handlers

        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            Startup.Set(Options.FileName, StartupCheckBox.IsChecked ?? false);

            Close();
        }

        private void Close(object sender, RoutedEventArgs e) => Close();

        #endregion

    }
}
