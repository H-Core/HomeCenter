using System;
using System.Windows;
using HomeCenter.NET.Utilities;
using VoiceActions.NET.Storages;

namespace HomeCenter.NET.Windows
{
    public partial class CommandWindow
    {
        #region Static methods

        public static (bool isSaved, Command newCommand) Show(Command command)
        {
            var window = new CommandWindow(command);
            var result = window.ShowDialog() == true;

            return (result, window.Command);
        } 

        public static bool ShowAndSaveIfNeeded(Command command, IStorage<Command> storage)
        {
            var (result, newCommand) = Show(command);
            if (!result)
            {
                return false;
            }

            foreach (var key in command.Keys)
            {
                storage.Remove(key);
            }

            foreach (var key in newCommand.Keys)
            {
                storage[key] = newCommand;
            }

            return true;
        }

        #endregion

        #region Properties

        public Command Command { get; }

        #endregion

        #region Constructors

        public CommandWindow(Command command)
        {
            Command = command?.Clone() as Command ?? throw new ArgumentNullException(nameof(command));

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
