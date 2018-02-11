using System;
using System.Windows;
using HomeCenter.NET.Utilities;
using VoiceActions.NET.Storages;

namespace HomeCenter.NET.Windows
{
    public partial class CommandsWindow
    {
        #region Properties

        public IStorage<Command> Commands { get; }

        #endregion

        #region Constructors

        public CommandsWindow(IStorage<Command> commands)
        {
            Commands = commands ?? throw new ArgumentNullException(nameof(commands));

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
