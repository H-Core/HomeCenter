using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H.NET.Core.Managers;
using H.NET.Core.Recorders;
using H.NET.Core.Runners;
using H.NET.Storages;
using H.NET.Storages.Extensions;
using H.NET.Utilities;
using HomeCenter.NET.Runners;

namespace HomeCenter.NET.Services
{
    public class MainService : IDisposable
    {
        #region Properties

        public BaseManager Manager { get; set; } = new BaseManager();
        public GlobalRunner GlobalRunner { get; }

        public Dictionary<KeysCombination, Command> Combinations { get; } = new Dictionary<KeysCombination, Command>();

        #endregion

        #region Constructors

        public MainService(ModuleService moduleService, CommandsStorage storage)
        {
            GlobalRunner = new GlobalRunner(moduleService, storage);
            Manager.NewText += text =>
            {
                if (Runner.IsWaitCommand)
                {
                    Runner.StopWaitCommand(text);
                    return;
                }

                Run(text);
            };

            Runner.GetVariableValueGlobalFunc = GlobalRunner.GetVariableValue;
        }

        #endregion

        #region Public methods

        public async Task Load(ModuleService moduleService)
        {
            await Task.Run(() => moduleService.Load());
            moduleService.AddUniqueInstancesIfNeed();
            moduleService.RegisterHandlers(this);

            UpdateCombinations();
            UpdateActiveModules(moduleService);
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

        public void UpdateActiveModules(ModuleService moduleService)
        {
            Manager.Recorder = moduleService.Recorder;
            Manager.Converter = moduleService.Converter;
            Manager.AlternativeConverters = moduleService.AlternativeConverters;
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
