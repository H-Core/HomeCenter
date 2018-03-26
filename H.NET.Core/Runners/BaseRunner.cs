using System;
using System.Linq;
using H.NET.Core.Utilities;

namespace H.NET.Core.Runners
{
    public abstract class BaseRunner : Module, IRunner
    {
        #region Properties

        protected class Information
        {
            public string Description { get; set; }
            public Action<string> Action { get; set; }
        }

        private InvariantStringDictionary<Information> HandlerDictionary { get; } = new InvariantStringDictionary<Information>();

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

        public string[] GetSupportedCommands() => HandlerDictionary.Select(i => $"{i.Key} {i.Value.Description}").ToArray();

        public virtual bool IsSupport(string key, Command command)
        {
            var data = command?.Data;
            if (string.IsNullOrWhiteSpace(data))
            {
                return false;
            }

            var values = data.SplitOnlyFirst(' ');

            return HandlerDictionary.ContainsKey(values[0]);
        }

        #endregion

        #region Protected methods

        protected abstract void RunInternal(string key, Command command);

        protected void AddAction(string key, Action<string> action, string description = null) => 
            HandlerDictionary[key] = new Information
        {
            Description = description,
            Action = action
        };

        protected Information GetHandler(string key) => 
            HandlerDictionary.TryGetValue(key, out var handler) ? handler : new Information();

        #endregion
    }
}