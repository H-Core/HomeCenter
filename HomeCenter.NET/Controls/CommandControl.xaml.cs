using System;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Controls
{
    public partial class CommandControl
    {
        #region Properties

        public Command Command { get; }

        #endregion

        #region Constructors

        public CommandControl(Command command)
        {
            Command = command ?? throw new ArgumentNullException(nameof(command));

            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void Edit_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        private void Delete_Click(object sender, System.Windows.RoutedEventArgs e)
        {

        }

        #endregion
    }
}
