using System;
using System.Windows;
using H.NET.Core.Utilities;
using H.NET.Storages;
using H.NET.Storages.Extensions;
using HomeCenter.NET.Controls;
using HomeCenter.NET.Runners;

namespace HomeCenter.NET.Windows
{
    public partial class CommandsWindow
    {
        #region Properties

        public GlobalRunner Runner { get; }

        #endregion

        #region Constructors

        public CommandsWindow(GlobalRunner runner)
        {
            Runner = runner ?? throw new ArgumentNullException(nameof(runner));

            InitializeComponent();

            Update();
        }

        #endregion

        #region Event handlers

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var command = new Command();
            command.Keys.Add(new SingleKey(string.Empty));
            command.Lines.Add(new SingleCommand(string.Empty));

            CommandWindow.ShowAndSaveIfNeeded(command, Runner.Storage);
            Update();
        } 

        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            Runner.Storage.Save();

            Close();
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Runner.Storage.Load(); // Cancel changes

            Close();
        }

        #endregion

        #region Private methods

        private void Update()
        {
            UpdateUserCommands();
            UpdateAllCommands();
            UpdateVariables();
        }

        private void UpdateUserCommands()
        {
            UserPanel.Children.Clear();
            foreach (var pair in Runner.Storage.UniqueValues(entry => entry.Value.Data))
            {
                var command = pair.Value;
                var control = new CommandControl(command.KeysString, command.Data, command.HotKey) {Height = 25};
                control.Deleted += (sender, args) =>
                {
                    foreach (var key in command.Keys)
                    {
                        Runner.Storage.Remove(key.Text);
                    }

                    Update();
                };
                control.Edited += (sender, args) =>
                {
                    CommandWindow.ShowAndSaveIfNeeded(command, Runner.Storage);
                    Update();
                };
                control.Run += (sender, args) =>
                {
                    foreach (var line in command.Lines)
                    {
                        MainWindow.GlobalRun(line.Text);
                    }
                };
                UserPanel.Children.Add(control);
            }
        }

        private void UpdateAllCommands()
        {
            AllPanel.Children.Clear();
            foreach (var command in Runner.GetSupportedCommands())
            {
                var values = command.SplitOnlyFirst(' ');
                var control = new CommandControl(values[0], values[1], editable: true)
                {
                    Height = 25
                };
                control.Run += (sender, args) => MainWindow.GlobalRun($"{values[0]} {control.ObjectDescription}");
                AllPanel.Children.Add(control);
            }
        }

        private void UpdateVariables()
        {
            VariablesPanel.Children.Clear();
            foreach (var command in Runner.GetVariables())
            {
                var values = command.SplitOnlyFirst(' ');
                var control = new CommandControl(values[0], values[1])
                {
                    Height = 25
                };
                VariablesPanel.Children.Add(control);
            }
        }

        #endregion
    }
}
