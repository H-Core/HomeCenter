using System;
using System.Windows;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET
{
    public partial class ChangeCommandWindow
    {
        #region Properties

        public Command Command { get; }

        #endregion

        #region Constructors

        public ChangeCommandWindow(Command command)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));

            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Close(object sender, RoutedEventArgs e) => Close();

        #endregion
    }
}
