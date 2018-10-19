using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.NET.Core;
using H.NET.Core.Managers;
using H.NET.Core.Runners;
using H.NET.Storages;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Services
{
    public partial class RunnerService
    {
        #region Properties

        public ModuleService ModuleService { get; }
        public StorageService StorageService { get; }
        public BaseManager Manager { get; }

        public List<string> History { get; } = new List<string>();
        public BlockingCollection<Process> Processes { get; } = new BlockingCollection<Process>();

        #endregion

        #region Events

        public event TextDelegate NotHandledText;

        public event TextDelegate NewOutput;
        private void Print(string text) => NewOutput?.Invoke(text);

        public event EventHandler BeforeRun;
        public event EventHandler AfterRun;

        #endregion

        #region Constructors

        public RunnerService(ModuleService moduleService, StorageService storageService, BaseManager manager)
        {
            ModuleService = moduleService ?? throw new ArgumentNullException(nameof(moduleService));
            StorageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            Manager = manager ?? throw new ArgumentNullException(nameof(manager));

            Runner.GetVariableValueGlobalFunc = GetVariableValue;

            Manager.NewText += text =>
            {
                if (Runner.IsWaitCommand)
                {
                    Runner.StopWaitCommand(text);
                    return;
                }

                Run(text);
            };
        }

        #endregion

        #region Run

        public async void Run(string message) => await RunAsync(message);

        public async void HiddenRun(string message) => await RunAsync(message, false);

        public async Task HiddenRunAsync(string message) => await RunAsync(message, false);


        #endregion

        #region Public methods

        public (IRunner runner, string command)[] GetSupportedCommands() => ModuleService
            .Runners
            .Select(runner => (runner: runner, commands: runner.GetSupportedCommands()))
            .SelectMany(i => i.commands, (i, command) => (i.runner, command))
            .ToArray();

        public string[] GetSupportedVariables() => ModuleService.Runners.SelectMany(i => i.GetSupportedVariables()).ToArray();

        public IRunner GetRunnerFor(string key, string data)
        {
            foreach (var runner in ModuleService.Runners)
            {
                if (runner.IsSupport(key, data))
                {
                    //Log($"Runner: {runner.Name} supported command with {key}{command?.Data}");
                    return runner;
                }

                //Log($"Runner: {runner.Name} is not supported command with {key}{command?.Data}");
            }

            return null;
        }

        public object GetVariableValue(string key) =>
            ModuleService.Runners.FirstOrDefault(i => i.GetSupportedVariables().Contains(key))?.GetVariableValue(key);

        private bool IsInternal(string key, string data) => ModuleService.Runners.Any(i => i.IsInternal(key, data));

        #endregion

        #region Protected methods

        public async Task RunAsync(string keyOrData, bool show = true)
        {
            if (string.IsNullOrWhiteSpace(keyOrData))
            {
                Print("Bad or empty request");
                return;
            }

            if (show)
            {
                History.Add(keyOrData);
            }

            var (newKey, newCommand) = StorageService.GetCommand(keyOrData);
            var realActionData = newKey ?? newCommand.Lines.FirstOrDefault()?.Text;
            var isInternal = newCommand.Lines.All(i => IsInternal(newKey, i.Text));
            if (show && !isInternal)
            {
                Print($"Run action for key: \"{realActionData}\"");
            }
            foreach (var line in newCommand.Lines)
            {
                var process = await RunSingleLine(newKey, line.Text);
                var exception = process.Information?.Exception;
                if (exception != null)
                {
                    if (exception is ThreadAbortException)
                    {
                        Print($"Command \"{line}\" canceled");
                        return;
                    }

                    Print($"{exception}");
                    return;
                }
            }

            if (show && !isInternal)
            {
                try
                {
                    new CommandsHistory(Options.CompanyName).Add(realActionData);
                }
                catch (Exception)
                {
                    //ignored
                }
            }
        }

        private async Task<Process> RunSingleLine(string key, string data)
        {
            var runner = GetRunnerFor(key, data);
            var isHandled = runner != null;
            if (!isHandled)
            {
                NotHandledText?.Invoke(key);

                return new Process(data ?? key, new Exception($"Runner for command \"{data}\" is not found"));
            }

            Thread thread = null;
            var task = Task.Run(() =>
            {
                thread = Thread.CurrentThread;

                return runner.Run(key, data);
            });

            while (thread == null)
            {
                await Task.Delay(1);
            }

            var process = new Process(data ?? key, task, thread);
            Processes.Add(process);

            try
            {
                BeforeRun?.Invoke(this, EventArgs.Empty);

                process.Information = await task;
            }
            catch (Exception exception)
            {
                process.Information = new RunInformation(exception);
            }
            finally
            {
                process.IsCompleted = true;

                AfterRun?.Invoke(this, EventArgs.Empty);
            }

            return process;
        }


        #endregion
    }
}
