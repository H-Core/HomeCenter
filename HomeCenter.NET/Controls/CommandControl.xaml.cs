using System;
using HomeCenter.NET.Utilities;
using HomeCenter.NET.Windows;

namespace HomeCenter.NET.Controls
{
    public partial class CommandControl
    {
        #region Properties

        public Command Command { get; }

        #endregion

        #region Events

        public event EventHandler CommandDeleted;

        #endregion

        #region Constructors

        public CommandControl(Command command)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));

            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void Edit_Click(object sender, System.Windows.RoutedEventArgs e) => 
            ChangeCommandWindow.Show(Command);

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e) =>
            CommandDeleted?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
