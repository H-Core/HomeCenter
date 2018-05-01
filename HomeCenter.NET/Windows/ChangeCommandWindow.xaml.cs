using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using H.NET.Core;
using H.NET.Storages;
using HomeCenter.NET.Controls;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Windows
{
    public partial class ChangeCommandWindow
    {
        #region Static methods

        public static (bool isSaved, Command newCommand) Show(Command command)
        {
            var window = new ChangeCommandWindow(command);
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
                storage.Remove(key.Text);
            }

            foreach (var key in newCommand.Keys)
            {
                storage[key.Text] = newCommand;
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
            Command = command?.Clone() as Command ?? throw new ArgumentNullException(nameof(command));

            InitializeComponent();
        }

        #endregion

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateKeys();
            UpdateData();
            UpdateHotKey();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            SaveKeys();
            SaveData();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void AddKeyButton_Click(object sender, RoutedEventArgs e)
        {
            SaveKeys();
            Command.Keys.Add(new SingleKey(string.Empty));
            UpdateKeys();

            /* TODO: Focus last element
            var control = DataPanel.Children.OfType<TextControl>().LastOrDefault();
            if (control == null)
            {
                return;
            }

            control.TextBox.Focusable = true;
            FocusManager.SetFocusedElement(FocusManager.GetFocusScope(control.TextBox), control.TextBox);
            Keyboard.Focus(control.TextBox);
            control.TextBox.Focus();*/
        }

        private void AddDataButton_Click(object sender, RoutedEventArgs e)
        {
            SaveData();
            Command.Lines.Add(new SingleCommand(string.Empty));
            UpdateData();
        }

        private async void HotKeyButton_Click(object sender, RoutedEventArgs e)
        {
            HotKeyButton.IsEnabled = false;

            var combination = await MainWindow.CatchKey();
            Command.HotKey = combination?.ToString() ?? string.Empty;

            HotKeyButton.IsEnabled = true;

            UpdateHotKey();
        }

        #endregion

        #region Private methods

        #region Keys

        private void SaveKeys(int ignoredLine = -1)
        {
            Command.Keys = KeysPanel
                .Children
                .OfType<TextControl>()
                .Where((c, i) => i != ignoredLine)
                .Select(c => new SingleKey(c.Text))
                .ToList();
        }

        private void UpdateKeys()
        {
            var controls = Command.Keys.Select((key, i) =>
            {
                var control = new TextControl(key.Text)
                {
                    Height = 25
                };
                control.Deleted += (sender, args) =>
                {
                    SaveKeys(ignoredLine: i);
                    Command.Keys.Remove(key);

                    UpdateKeys();
                };

                return (Control)control;
            }).ToList();
            controls.AddButton("Add New", AddKeyButton_Click);

            KeysPanel.Update(controls);
        }

        #endregion

        #region Data

        private void SaveData(int ignoredLine = -1)
        {
            Command.Lines = DataPanel
                .Children
                .OfType<CommandControl>()
                .Where((c, i) => i != ignoredLine)
                .Select(c => new SingleCommand(c.ObjectDescription))
                .ToList();
        }

        private void UpdateData()
        {
            var controls = Command.Lines.Select((line, i) =>
            {
                var control = new CommandControl(null, line.Text, editable: true, run: true, delete: true)
                {
                    MinHeight = 25
                };
                control.Deleted += (sender, args) =>
                {
                    SaveData(ignoredLine: i);

                    Command.Lines.Remove(line);
                    UpdateData();
                };
                control.Run += (sender, args) => MainWindow.GlobalRun(line.Text);

                return (Control)control;
            }).ToList();
            controls.AddButton("Add New", AddDataButton_Click);

            DataPanel.Update(controls);
        }

        #endregion

        #region Hot Key

        private void UpdateHotKey()
        {
            HotKeyButton.Content = Command.HotKey;
        }

        #endregion

        #endregion
    }
}
