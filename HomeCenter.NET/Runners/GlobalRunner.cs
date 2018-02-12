using System.Collections.Generic;
using HomeCenter.NET.Runners.Core;
using HomeCenter.NET.Utilities;
using VoiceActions.NET.Storages;

namespace HomeCenter.NET.Runners
{
    public class GlobalRunner : BaseRunner
    {
        #region Properties

        private List<IRunner> Runners { get; set; } = new List<IRunner>();
        public List<string> History { get; } = new List<string>();

        #endregion

        public GlobalRunner(IStorage<Command> storage = null, IRunner defaultRunner = null)
        {
            Storage = storage ?? new InvariantDictionaryStorage<Command>();
            Storage.Load();

            AddRunner(defaultRunner ?? new DefaultRunner());
        }

        public override string[] GetSupportedCommands() => new string[0];

        public void AddRunner(IRunner runner)
        {
            runner.Storage = Storage;
            runner.NewSpeech += OnNewSpeech;
            runner.NewOutput += OnNewOutput;
            runner.NewCommand += OnNewCommand;

            Runners.Add(runner);
        }

        public IRunner GetRunnerFor(string key, string data)
        {
            foreach (var runner in Runners)
            {
                if (runner.IsSupport(key, data))
                {
                    return runner;
                }
            }

            return null;
        }

        protected override void RunInternal(string key, Command command)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Print("Bad or empty request");
                return;
            }

            History.Add(key);

            var runner = GetRunnerFor(key, command.Data);
            Print(runner != null
                ? $"Run action for text: \"{key}\""
                : $"We don't have handler for text: \"{key}\"");

            runner?.Run(key, command.Data);
        }

        #region IDisposable

        public override void Dispose()
        {
            foreach (var runner in Runners)
            {
                runner.Dispose();
            }
            Runners.Clear();
        }

        #endregion
    }
}
