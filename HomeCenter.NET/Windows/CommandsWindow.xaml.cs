using System;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using H.NET.Core.Utilities;
using H.NET.Storages;
using H.NET.Storages.Extensions;
using HomeCenter.NET.Controls;
using HomeCenter.NET.Runners;
using HomeCenter.NET.Utilities;

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

            Runner.BeforeRun += (o, e) => Dispatcher.Invoke(UpdateProcesses);
            Runner.AfterRun += (o, e) => Dispatcher.Invoke(UpdateProcesses);
        }

        #endregion

        #region Event handlers

        private void Window_Loaded(object sender, RoutedEventArgs e) => Update();
        private void Add_Click(object sender, RoutedEventArgs e) => Add();

        private void SaveAndClose(object sender, RoutedEventArgs e)
        {
            Runner.Storage.Save();

            this.SetDialogResultAndClose(true);
        }

        private void Close(object sender, RoutedEventArgs e)
        {
            Runner.Storage.Load(); // Cancel changes

            this.SetDialogResultAndClose(false);
        }

        #endregion

        #region Private methods

        private void Add() => SafeActions.Run(() =>
        {
            var command = new Command();
            command.Keys.Add(new SingleKey(string.Empty));
            command.Lines.Add(new SingleCommand(string.Empty));

            ChangeCommandWindow.ShowAndSaveIfNeeded(command, Runner.Storage);

            UpdateUserCommands();
        });

        private void Update()
        {
            UpdateUserCommands();
            UpdateAllCommands();
            UpdateVariables();
            UpdateProcesses();
        }

        private void UpdateUserCommands() => SafeActions.Run(() =>
        {
            var controls = Runner.Storage.UniqueValues(entry => string.Concat(entry.Value.Lines)).Select(pair =>
            {
                var command = pair.Value;
                var control = new CommandControl(command.FirstKeyText, command.FirstDataText, command.HotKey, 
                    run: true, edit: true, delete: true)
                {
                    Height = 25
                };
                control.Deleted += (sender, args) =>
                {
                    foreach (var key in command.Keys)
                    {
                        Runner.Storage.Remove(key.Text);
                    }

                    UpdateUserCommands();
                };
                control.Edited += (sender, args) =>
                {
                    ChangeCommandWindow.ShowAndSaveIfNeeded(command, Runner.Storage);
                    UpdateUserCommands();
                };
                control.Run += (sender, args) =>
                {
                    foreach (var line in command.Lines)
                    {
                        MainWindow.GlobalRun(line.Text);
                    }
                };

                return control;
            });

            UserPanel.Update(controls);
        });

        private void UpdateAllCommands() => SafeActions.Run(() =>
        {
            var controls = Runner.GetSupportedCommands().Select(tuple =>
            {
                var values = tuple.command.SplitOnlyFirst(' ');
                var control = new CommandControl($"{tuple.runner.GetType().Name}: {values[0]}", values[1],
                    editable: true, run: true, edit: true, delete: true)
                {
                    Height = 25
                };
                control.NameLabel.Width *= 2;
                control.Run += (sender, args) => MainWindow.GlobalRun($"{values[0]} {control.ObjectDescription}");

                return control;
            });

            AllPanel.Update(controls);
        });

        private void UpdateVariables() => SafeActions.Run(() =>
        {
            var controls = Runner.GetSupportedVariables().Select(name =>
            {
                var control = new CommandControl(name, "Please press R to calculate",
                        run: true, edit: true, delete: true)
                {
                    Height = 25
                };
                control.Run += (sender, args) =>
                {
                    control.ObjectDescription = Runner.GetVariableValue(name)?.ToString() ?? string.Empty;
                };

                return control;
            });

            VariablesPanel.Update(controls);
        });

        private void UpdateProcesses() => SafeActions.Run(() =>
        {
            var controls = Runner.Processes.Select(process =>
            {
                var control = new CommandControl(null, process.Name ?? string.Empty,
                    run: true, edit: false, delete: !process.IsCompleted)
                {
                    Height = 25,
                    Color = process.IsCanceled ? Colors.Gold : process.IsCompleted ? Colors.LightGreen : Colors.Lavender
                };
                control.Run += (sender, args) => MainWindow.GlobalRun(process.Name);
                control.Deleted += (sender, args) => process.Cancel();

                return control;
            }).ToArray();

            ProcessesPanel.Update(controls.Reverse());
        });

        #endregion
    }
}
