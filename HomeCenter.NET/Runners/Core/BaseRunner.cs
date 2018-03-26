using System;
using System.Linq;
using H.Storages;
using VoiceActions.NET.Utilities;

namespace HomeCenter.NET.Runners.Core
{
    public abstract class BaseRunner : IRunner
    {
        #region Properties

        private InvariantStringDictionary<(string description, Action<string> action)> HandlerDictionary { get; } = 
            new InvariantStringDictionary<(string, Action<string>)>();

        #endregion

        #region Events

        public event EventHandler<RunnerEventArgs> BeforeRun;
        private void OnBeforeRun(string text) => BeforeRun?.Invoke(this, CreateArgs(text));

        public event EventHandler<RunnerEventArgs> AfterRun;
        private void OnAfterRun(string text) => AfterRun?.Invoke(this, CreateArgs(text));

        public event EventHandler<RunnerEventArgs> NewSpeech;
        protected void OnNewSpeech(object sender, RunnerEventArgs args) => NewSpeech?.Invoke(sender, args);
        protected void Say(string text) => OnNewSpeech(this, CreateArgs(text));

        public event EventHandler<RunnerEventArgs> NewOutput;
        protected void OnNewOutput(object sender, RunnerEventArgs args) => NewOutput?.Invoke(sender, args);
        protected void Print(string text) => OnNewOutput(this, CreateArgs(text));

        public event EventHandler<RunnerEventArgs> NewCommand;
        protected void OnNewCommand(object sender, RunnerEventArgs args) => NewCommand?.Invoke(sender, args);
        protected void RunCommand(string text) => OnNewCommand(this, CreateArgs(text));

        private RunnerEventArgs CreateArgs(string text) => new RunnerEventArgs { Runner = this, Text = text };

        #endregion

        #region Public methods

        public void Run(string key, Command command)
        {
            OnBeforeRun(key);

            try
            {
                RunInternal(key, command);
                OnAfterRun(key);
            }
            catch (Exception exception)
            {
                Print($"Exception while running command: \"{command.Data}\": {exception}");
            }
        }

        public string[] GetSupportedCommands() => HandlerDictionary.Select(i => $"{i.Key} {i.Value.description}").ToArray();

        public virtual bool IsSupport(string key, Command command)
        {
            var data = command?.Data;
            if (string.IsNullOrWhiteSpace(data))
            {
                return false;
            }

            (var prefix, var _) = data.SplitOnlyFirst(' ');

            return HandlerDictionary.ContainsKey(prefix);
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
        }

        #endregion

        #region Protected methods

        protected abstract void RunInternal(string key, Command command);

        protected void AddAction(string key, Action<string> action, string description = null) => HandlerDictionary[key] = (description, action);

        protected (string description, Action<string> action) GetHandler(string key) => 
            HandlerDictionary.TryGetValue(key, out var handler) ? handler : (string.Empty, null);

        #endregion
    }
}