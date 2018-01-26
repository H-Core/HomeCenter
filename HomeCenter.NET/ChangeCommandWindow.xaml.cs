using System.Windows;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET
{
    public partial class ChangeCommandWindow
    {
        #region Properties

        public Command Command { get; set; }

        #endregion

        #region Constructors

        public ChangeCommandWindow()
        {
            InitializeComponent();
        }

        public ChangeCommandWindow(Command command) : this()
        {
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
