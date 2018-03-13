using System;
using System.Windows;
using H.Storages;
using H.Storages.Extensions;
using HomeCenter.NET.Controls;
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

            Update();
        }

        #endregion

        #region Event handlers

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var command = new Command();
            command.Keys.Add(new SingleKey(string.Empty));
            command.Commands.Add(new SingleCommand(string.Empty));

            CommandWindow.ShowAndSaveIfNeeded(command, Storage);
            Update();
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

        private void Update()
        {
            Panel.Children.Clear();
            foreach (var pair in Storage.UniqueValues(entry => entry.Value.Data))
            {
                var command = pair.Value;
                var control = new ObjectControl(command.KeysString, command.Data) { Height = 25 };
                control.Deleted += (sender, args) =>
                {
                    foreach (var key in command.Keys)
                    {
                        Storage.Remove(key.Text);
                    }
                    Update();
                };
                control.Edited += (sender, args) =>
                {
                    CommandWindow.ShowAndSaveIfNeeded(command, Storage);
                    Update();
                };
                Panel.Children.Add(control);
            }
        }

        #endregion
    }
}
