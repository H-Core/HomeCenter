using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using H.NET.Core;
using H.NET.Core.Runners;
using H.NET.Core.Storages;
using H.NET.Storages;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Runners
{
    public class GlobalRunner : Module
    {
        #region Properties

        public IStorage<Command> Storage { get; }
        private List<IRunner> Runners { get; } = new List<IRunner>();
        public List<string> History { get; } = new List<string>();

        private List<IRunner> AllRunners => Options.Runners.Concat(Runners).ToList();

        #endregion

        #region Events

        public event TextDelegate NotHandledText;

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
            runner.NewSpeech += Say;
            runner.NewOutput += Print;
            runner.NewCommand += RunCommand;

            Runners.Add(runner);
        }

        public string[] GetSupportedCommands() => AllRunners.SelectMany(i => i.GetSupportedCommands()).ToArray();

        public IRunner GetRunnerFor(string key, string data)
        {
            foreach (var runner in AllRunners)
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
                        foreach (var line in command.Lines)
                        {
                            line.Text = line.Text.Replace("*", argument);
                        }
                    }

                    return (pair.Key, command);
                }
            }

            return (null, new Command(null, key));
        }

        public async Task Run(string keyOrData)
        {
            if (string.IsNullOrWhiteSpace(keyOrData))
            {
                Print("Bad or empty request");
                NotHandledText?.Invoke(keyOrData);
                return;
            }

            History.Add(keyOrData);

            var (newKey, newCommand) = GetCommand(keyOrData);
            foreach (var line in newCommand.Lines)
            {
                var information = await RunSingleLine(newKey, line.Text);
                if (information.Exception != null)
                {
                    Print($"{information.Exception}");
                    return;
                }
            }
        }

        private async Task<RunInformation> RunSingleLine(string key, string data)
        {
            var runner = GetRunnerFor(key, data);
            var isHandled = runner != null;
            if (!isHandled)
            {
                NotHandledText?.Invoke(key);

                return new RunInformation(new Exception($"Runner for command \"{data}\" is not found"));
            }

            var information = await Task.Run(() => runner.Run(key, data));
            if (information?.IsInternal == false)
            {
                Print($"Run action for key: \"{key}\": \"{data}\"");
            }

            try
            {
                new CommandsHistory(Options.CompanyName).Add(key);
            }
            catch (Exception)
            {
                //ignored
            }

            return information;
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
