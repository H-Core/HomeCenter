using System;
using System.Windows;
using HomeCenter.NET.Controls;
using HomeCenter.NET.Extensions;
using HomeCenter.NET.Utilities;
using VoiceActions.NET.Storages;

namespace HomeCenter.NET.Windows
{
    public partial class CommandsWindow
    {
        #region Properties

        public IStorage<Command> Storage { get; }

        #endregion

        #region Constructors

        public CommandsWindow(IStorage<Command> storage)
        {
            Storage = storage ?? throw new ArgumentNullException(nameof(storage));

            InitializeComponent();

            ShowCommands();
        }

        #endregion

        #region Event handlers

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            CommandWindow.ShowAndSaveIfNeeded(new Command(), Storage);
            ShowCommands();
        } 

        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            Storage.Save();

            DialogResult = true;
            Close();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Storage.Load(); // Cancel changes

            DialogResult = false;
            Close();
        } 

        #endregion

        #region Private methods

        public void ShowCommands()
        {
            CommandsPanel.Children.Clear();
            foreach (var pair in Storage.UniqueValues(entry => entry.Value.Data))
            {
                var command = pair.Value;
                var control = new CommandControl(command) {Height = 25};
                control.CommandDeleted += (sender, args) =>
                {
                    foreach (var key in command.Keys)
                    {
                        Storage.Remove(key);
                    }
                    ShowCommands();
                };
                control.CommandEdited += (sender, args) =>
                {
                    CommandWindow.ShowAndSaveIfNeeded(command, Storage);
                    ShowCommands();
                };
                CommandsPanel.Children.Add(control);
            }
        }

        #endregion
    }
}
