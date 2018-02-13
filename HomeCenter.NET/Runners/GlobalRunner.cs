using System.Collections.Generic;
using HomeCenter.NET.Runners.Core;
using HomeCenter.NET.Utilities;
using VoiceActions.NET.Managers;
using VoiceActions.NET.Storages;

namespace HomeCenter.NET.Runners
{
    public class GlobalRunner : BaseRunner
    {
        #region Properties

        private List<IRunner> Runners { get; set; } = new List<IRunner>();
        public List<string> History { get; } = new List<string>();

        #endregion

        #region Events

        public event BaseManager.TextDelegate NotHandledText;

        #endregion

        #region Constructors

        public GlobalRunner(IStorage<Command> storage = null, IRunner defaultRunner = null)
        {
            Storage = storage ?? new InvariantDictionaryStorage<Command>();
            Storage.Load();

            AddRunner(defaultRunner ?? new DefaultRunner());
        }

        #endregion

        #region Public methods

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

        #endregion

        #region Protected methods

        protected override void RunInternal(string key, Command command)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Print("Bad or empty request");
                NotHandledText?.Invoke(key);
                return;
            }

            History.Add(key);

            var runner = GetRunnerFor(key, command.Data);
            var isHandled = runner != null;
            Print(isHandled
                ? $"Run action for text: \"{key}\""
                : $"We don't have handler for text: \"{key}\"");

            runner?.Run(key, command.Data);

            if (!isHandled)
            {
                NotHandledText?.Invoke(key);
            }
        }

        #endregion

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
