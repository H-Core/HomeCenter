using System;
using System.Collections.Generic;
using System.Linq;
using H.NET.Core;
using H.NET.Core.Managers;
using H.NET.Core.Runners;
using H.NET.Core.Storages;
using H.NET.Storages;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Runners
{
    public class GlobalRunner : BaseRunner
    {
        #region Properties

        public IStorage<Command> Storage { get; }
        private List<IRunner> Runners { get; } = new List<IRunner>();
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

        public void AddRunner(IRunner runner)
        {
            runner.NewSpeech += OnNewSpeech;
            runner.NewOutput += OnNewOutput;
            runner.NewCommand += OnNewCommand;

            Runners.Add(runner);
        }

        public IRunner GetRunnerFor(string key, Command command)
        {
            var runtimeRunners = ModuleManager.Instance.GetPluginsOfSubtype<IRunner>().Select(i => i.Value);
            foreach (var runner in runtimeRunners.Concat(Runners))
            {
                if (runner.IsSupport(key, command))
                {
                    //Log($"Runner: {runner.Name} supported command with {key}{command?.Data}");
                    return runner;
                }

                //Log($"Runner: {runner.Name} is not supported command with {key}{command?.Data}");
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

            return (null, new Command(null, key));
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
            else
            {
                new CommandsHistory(Options.CompanyName).Add(key);
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
