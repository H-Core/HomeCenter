using System;

namespace VoiceActions.NET.Runners.Core
{
    public abstract class BaseRunner : IRunner
    {
        #region Events

        public event EventHandler<VoiceActionsEventArgs> BeforeRun;
        private void OnBeforeRun(VoiceActionsEventArgs args) => BeforeRun?.Invoke(this, args);

        public event EventHandler<VoiceActionsEventArgs> AfterRun;
        private void OnAfterRun(VoiceActionsEventArgs args) => AfterRun?.Invoke(this, args);

        private VoiceActionsEventArgs CreateArgs(string command) => new VoiceActionsEventArgs{ Text = command };

        #endregion

        #region Public methods

        public void Run(string command)
        {
            OnBeforeRun(CreateArgs(command));

            RunInternal(command);

            OnAfterRun(CreateArgs(command));
        }

        public abstract string[] GetSupportedCommands();

        public virtual string GetSupportedCommandsText() => $@"Supported commands:
{string.Join(Environment.NewLine, GetSupportedCommands())}";

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
        }

        #endregion

        #region Protected methods

        protected abstract void RunInternal(string command);

        #endregion
    }
}
