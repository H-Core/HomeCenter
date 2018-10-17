using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using H.NET.Core.Managers;
using H.NET.Core.Recorders;
using H.NET.Core.Runners;
using H.NET.Storages;
using H.NET.Storages.Extensions;
using H.NET.Utilities;
using HomeCenter.NET.Runners;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Services
{
    public class MainService : IDisposable
    {
        #region Properties

        public BaseManager Manager { get; set; } = new BaseManager();
        public GlobalRunner GlobalRunner { get; set; } = new GlobalRunner(new CommandsStorage(Options.CompanyName));
        public IpcServer IpcServer { get; } = new IpcServer(Options.IpcPortToHomeCenter);

        public Dictionary<KeysCombination, Command> Combinations { get; } = new Dictionary<KeysCombination, Command>();

        #endregion

        #region Constructors

        public MainService()
        {
            Manager.NewText += text =>
            {
                if (Runner.IsWaitCommand)
                {
                    Runner.StopWaitCommand(text);
                    return;
                }

                Run(text);
            };


            IpcServer.NewMessage += Run;

            Runner.GetVariableValueGlobalFunc = GlobalRunner.GetVariableValue;
        }

        #endregion

        #region Public methods

        public async Task Load()
        {
            ModuleManager.RunAction = Run; // TODO: Hidden?
            ModuleManager.RunAsyncFunc = HiddenRunAsync;
            await Task.Run(() =>
                {
                    ModuleManager.Instance.Load();
                    ModuleManager.Instance.EnableInstances();
                });
            ModuleManager.AddUniqueInstancesIfNeed();
            ModuleManager.RegisterHandlers();

            UpdateCombinations();
            UpdateActiveModules();
        }

        public void StartRecord(int timeout)
        {
            Manager.ChangeWithTimeout(timeout);
        }

        public void UpdateCombinations()
        {
            Combinations.Clear();
            foreach (var pair in GlobalRunner.Storage.UniqueValues(i => i.Value).Where(i => i.Value.HotKey != null))
            {
                var command = pair.Value;
                var hotKey = command.HotKey;
                var combination = KeysCombination.FromString(hotKey);
                if (combination.IsEmpty)
                {
                    continue;
                }

                Combinations[combination] = command;
            }
        }

        public void UpdateActiveModules()
        {
            Manager.Recorder = Options.Recorder;
            Manager.Converter = Options.Converter;
            Manager.AlternativeConverters = Options.AlternativeConverters;
        }

        #region Run

        public bool RunCombination(KeysCombination combination)
        {
            if (!Combinations.TryGetValue(combination, out var command))
            {
                return false;
            }

            Run(command.Keys.FirstOrDefault()?.Text);
            return true;
        }

        public async void Run(string command)
        {
            await GlobalRunner.Run(command);
        }

        public async Task HiddenRunAsync(string message) => await GlobalRunner.Run(message, false);

        public async void HiddenRun(string message) => await GlobalRunner.Run(message, false);

        #endregion

        #region Restart

        public void RestartWithUpdate(string command) => Restart(command, "/updating");

        public void Restart() => Restart(new List<string>());
        public void Restart(string command, string additionalArguments = null) => Restart(new[] { command }, additionalArguments);

        public void Restart(ICollection<string> commands, string additionalArguments = null)
        {
            var run = commands.Any() ? $"/run \"{string.Join(";", commands)}\"" : string.Empty;

            IpcServer.Stop();
            Process.Start($"\"{Options.FilePath}\"", $"/restart {run} {additionalArguments}");
            Application.Current.Shutdown();
        }

        #endregion

        #region Dispose

        public void Dispose()
        {
            Manager?.Dispose();
            Manager = null;
        }

        #endregion

        #endregion
    }
}
