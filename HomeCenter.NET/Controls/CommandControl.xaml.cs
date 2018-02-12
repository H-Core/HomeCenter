using System;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Controls
{
    public partial class CommandControl
    {
        #region Properties

        public Command Command { get; }

        #endregion

        #region Events

        public event EventHandler CommandEdited;
        public event EventHandler CommandDeleted;

        #endregion

        #region Constructors

        public CommandControl(Command command)
        {
            Command = command.Clone() as Command ?? throw new ArgumentNullException(nameof(command));

            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void Edit_Click(object sender, System.Windows.RoutedEventArgs e) =>
            CommandEdited?.Invoke(this, EventArgs.Empty);

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e) =>
            CommandDeleted?.Invoke(this, EventArgs.Empty);

        #endregion
    }
}
