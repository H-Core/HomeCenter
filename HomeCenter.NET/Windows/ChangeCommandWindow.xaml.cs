using System;
using System.Windows;
using HomeCenter.NET.Utilities;
using VoiceActions.NET.Storages;

namespace HomeCenter.NET.Windows
{
    public partial class ChangeCommandWindow
    {
        #region Static methods

        public static bool Show(Command command) => new ChangeCommandWindow(command).ShowDialog() == true;

        public static bool ShowAndSaveIfNeeded(Command command, IStorage<Command> storage)
        {
            var result = Show(command);
            if (!result)
            {
                return false;
            }

            // TODO: delete initial command keys
            foreach (var key in command.Keys)
            {
                storage[key] = command;
            }

            return true;
        }

        #endregion

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
