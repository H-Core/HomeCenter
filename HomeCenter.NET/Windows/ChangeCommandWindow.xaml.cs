using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using H.NET.Core;
using H.NET.Storages;
using HomeCenter.NET.Controls;

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

        public CommandWindow(Command command)
        {
            Command = command?.Clone() as Command ?? throw new ArgumentNullException(nameof(command));

            InitializeComponent();

            Update();
        }

        #endregion

        #region Event handlers
        
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            Save();

            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) => Close();

        private void AddKeyButton_Click(object sender, RoutedEventArgs e)
        {
            Save();
            Command.Keys.Add(new SingleKey(string.Empty));
            Update();

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
            Save();
            Command.Lines.Add(new SingleCommand(string.Empty));
            Update();
        }

        private async void HotKeyButton_Click(object sender, RoutedEventArgs e)
        {
            HotKeyButton.IsEnabled = false;

            var combination = await MainWindow.CatchKey();
            Command.HotKey = combination?.ToString() ?? string.Empty;

            HotKeyButton.IsEnabled = true;

            Update();
        }

        #endregion

        #region Private methods

        private void Save()
        {
            Command.Lines = DataPanel.Children.OfType<TextControl>().Select(c => new SingleCommand(c.Text)).ToList();
            Command.Keys = KeysPanel.Children.OfType<TextControl>().Select(c => new SingleKey(c.Text)).ToList();
        }

        private void Update()
        {
            HotKeyButton.Content = Command.HotKey;

            KeysPanel.Children.Clear();
            foreach (var key in Command.Keys)
            {
                var control = new TextControl(key.Text)
                {
                    Height = 25
                };
                control.Deleted += (sender, args) =>
                {
                    Command.Keys.Remove(key);
                    Update();
                };
                KeysPanel.Children.Add(control);
            }
            var addKeyButton = new Button {Content = "Add New"};
            addKeyButton.Click += AddKeyButton_Click;
            KeysPanel.Children.Add(addKeyButton);

            DataPanel.Children.Clear();
            foreach (var line in Command.Lines)
            {
                var control = new TextControl(line.Text)
                {
                    MinHeight = 25
                };
                control.Deleted += (sender, args) =>
                {
                    Command.Lines.Remove(line);
                    Update();
                };
                DataPanel.Children.Add(control);
            }
            var addDataButton = new Button { Content = "Add New" };
            addDataButton.Click += AddDataButton_Click;
            DataPanel.Children.Add(addDataButton);
        }

        #endregion
    }
}
