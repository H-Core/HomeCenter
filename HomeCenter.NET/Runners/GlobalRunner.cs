using System;
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

        public IStorage<Command> Storage { get; set; }
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
            runner.NewSpeech += OnNewSpeech;
            runner.NewOutput += OnNewOutput;
            runner.NewCommand += OnNewCommand;

            Runners.Add(runner);
        }

        public IRunner GetRunnerFor(string key, Command command)
        {
            foreach (var runner in Runners)
            {
                if (runner.IsSupport(key, command))
                {
                    return runner;
                }
            }

            return null;
        }

        #endregion

        #region Protected methods

        private (string key, Command command) GetCommand(string key)
        {
            key = key ?? throw new ArgumentNullException(nameof(key));

            if (Storage.TryGetValue(key, out var result))
            {
                return (key, result);
            }

            foreach (var pair in Storage)
            {
                var tryKey = pair.Key;
                if (!tryKey.Contains("*"))
                {
                    continue;
                }

                var subKeys = tryKey.Split('*');
                if (subKeys.Length < 2)
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(subKeys[0]))
                {
                    continue;
                }

                if (key.StartsWith(subKeys[0]))
                {
                    var command = pair.Value?.Clone() as Command;
                    if (command != null)
                    {
                        var argument = key.Substring(subKeys[0].Length);
                        foreach (var line in command.Commands)
                        {
                            line.Text = line.Text.Replace("*", argument);
                        }
                    }

                    return (pair.Key, command);
                }
            }

            return (key, null);
        }

        protected override void RunInternal(string key, Command command)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                Print("Bad or empty request");
                NotHandledText?.Invoke(key);
                return;
            }

            History.Add(key);

            var (newKey, newCommand) = GetCommand(key);

            var runner = GetRunnerFor(newKey, newCommand);
            var isHandled = runner != null;
            Print(isHandled
                ? $"Run action for text: \"{key}\""
                : $"We don't have handler for text: \"{key}\"");

            runner?.Run(newKey, newCommand);

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
