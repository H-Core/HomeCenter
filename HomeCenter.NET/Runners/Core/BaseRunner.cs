using System;
using HomeCenter.NET.Utilities;
using VoiceActions.NET.Utilities;

namespace HomeCenter.NET.Runners.Core
{
    public abstract class BaseRunner : IRunner
    {
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

        public abstract string[] GetSupportedCommands();

        public virtual string GetSupportedCommandsText() => $@"Supported commands:
{string.Join(Environment.NewLine, GetSupportedCommands())}";

        public virtual bool IsSupport(string key, Command command)
        {
            var data = command?.Data;
            if (string.IsNullOrWhiteSpace(data))
            {
                return false;
            }

            foreach (var supportedCommandText in GetSupportedCommands())
            {
                (var prefix, var _) = supportedCommandText.SplitOnlyFirst(' ');
                if (data.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
        }

        #endregion

        #region Protected methods

        protected abstract void RunInternal(string key, Command command);

        #endregion
    }
}