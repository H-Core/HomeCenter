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
        public List<string> History { get; } = new List<string>();

        private static List<IRunner> Runners => Options.Runners;

        #endregion

        #region Events

        public event TextDelegate NotHandledText;

        #endregion

        #region Constructors

        public GlobalRunner(IStorage<Command> storage = null)
        {
            Storage = storage ?? new InvariantDictionaryStorage<Command>();
            Storage.Load();
        }

        #endregion

        #region Public methods

        public (IRunner, string)[] GetSupportedCommands() => Runners
            .Select(runner => (runner: runner, commands: runner.GetSupportedCommands()))
            .SelectMany(i => i.commands, (i, command) => (i.runner, command))
            .ToArray();

        public string[] GetSupportedVariables() => Runners.SelectMany(i => i.GetSupportedVariables()).ToArray();

        public IRunner GetRunnerFor(string key, string data)
        {
            foreach (var runner in Runners)
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
            Runners.FirstOrDefault(i => i.GetSupportedVariables().Contains(key))?.GetVariableValue(key);

        private bool IsInternal(string key, string data) => Runners.Any(i => i.IsInternal(key, data));

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

                    return (key, command);
                }
            }

            return (null, new Command(null, key));
        }

        public async Task Run(string keyOrData, bool show = true)
        {
            if (string.IsNullOrWhiteSpace(keyOrData))
            {
                Print("Bad or empty request");
                return;
            }

            History.Add(keyOrData);

            var (newKey, newCommand) = GetCommand(keyOrData);
            var realActionData = newKey ?? newCommand.Lines.FirstOrDefault()?.Text;
            var isInternal = newCommand.Lines.All(i => IsInternal(newKey, i.Text));
            if (show && !isInternal)
            {
                Print($"Run action for key: \"{realActionData}\"");
            }
            foreach (var line in newCommand.Lines)
            {
                var information = await RunSingleLine(newKey, line.Text);
                if (information.Exception != null)
                {
                    Print($"{information.Exception}");
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

        private async Task<RunInformation> RunSingleLine(string key, string data)
        {
            var runner = GetRunnerFor(key, data);
            var isHandled = runner != null;
            if (!isHandled)
            {
                NotHandledText?.Invoke(key);

                return new RunInformation(new Exception($"Runner for command \"{data}\" is not found"));
            }

            return await Task.Run(() => runner.Run(key, data));
        }


        #endregion
    }
}
