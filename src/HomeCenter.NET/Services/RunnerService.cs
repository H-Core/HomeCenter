using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using H.Core;
using H.Core.Managers;
using H.Core.Runners;
using H.NET.Storages;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Services
{
    public partial class RunnerService
    {
        #region Properties

        //public ModuleService ModuleService { get; }
        public StorageService StorageService { get; }
        public BaseManager Manager { get; }

        public List<string> History { get; } = new List<string>();
        public BlockingCollection<Process> Processes { get; } = new BlockingCollection<Process>();

        #endregion

        #region Events

        public event EventHandler<string?>? NotHandledText;

        public event EventHandler<string>? NewOutput;
        private void Print(string text) => NewOutput?.Invoke(this, text);

        public event EventHandler? BeforeRun;
        public event EventHandler? AfterRun;

        #endregion

        #region Constructors

        public RunnerService(StorageService storageService, BaseManager manager)
        {
            //ModuleService = moduleService ?? throw new ArgumentNullException(nameof(moduleService));
            StorageService = storageService ?? throw new ArgumentNullException(nameof(storageService));
            Manager = manager ?? throw new ArgumentNullException(nameof(manager));

            Module.GetVariableValueGlobalFunc = GetVariableValue;

            Manager.NewText += (_, text) =>
            {
                if (text == null)
                {
                    return;
                }
                
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

        public (IRunner runner, string command)[] GetSupportedCommands() => new (IRunner runner, string command)[0];
            //ModuleService
            //.Runners
            //.Select(runner => (runner, commands: runner.GetSupportedCommands()))
            //.SelectMany(i => i.commands, (i, command) => (i.runner, command))
            //.ToArray();

        public string[] GetSupportedVariables()
        {
            return new string[0]; //ModuleService.Modules
            //.SelectMany(i => i.GetSupportedVariables())
            //.ToArray();
        }

        public IRunner? GetRunnerFor(string key, string data)
        {
            //foreach (var runner in ModuleService.Runners)
            {
                //if (runner.IsSupport(key, data))
                {
                    //Log($"Runner: {runner.Name} supported command with {key}{command?.Data}");
                    //return runner;
                }

                //Log($"Runner: {runner.Name} is not supported command with {key}{command?.Data}");
            }

            return null;
        }

        public object GetVariableValue(string key) =>
            //ModuleService.Modules.FirstOrDefault(i => i.GetSupportedVariables().Contains(key))?.GetModuleVariableValue(key)
            throw new InvalidOperationException("Variable not found.");

        private bool IsInternal(string? key, string data) => false;//ModuleService.Runners.Any(i => i.IsInternal(key ?? string.Empty, data));

        #endregion

        #region Protected methods

        public async Task RunAsync(string keyOrData, bool show = true, CancellationToken cancellationToken = default)
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
            if (newCommand == null)
            {
                return;
            }

            var realActionData = newKey ?? newCommand.Lines.FirstOrDefault()?.Text;
            var isInternal = newCommand.Lines.All(i => IsInternal(newKey, i.Text));
            if (show && !isInternal)
            {
                Print($"Run action for key: \"{realActionData}\"");
            }
            foreach (var line in newCommand.Lines)
            {
                var process = await RunSingleLine(newKey ?? string.Empty, line.Text);
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
                    new CommandsHistory(Options.CompanyName).Add(realActionData ?? string.Empty);
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
            if (runner == null)
            {
                NotHandledText?.Invoke(this, key);

                return new Process(data ?? key, new Exception($"Runner for command \"{data}\" is not found"));
            }

            Thread? thread = null;
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
