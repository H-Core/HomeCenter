using System;

namespace VoiceActions.NET.Runners.Core
{
    public class BaseRunner : IRunner
    {
        #region Properties

        protected Action<string> Action { private get; set; }

        #endregion

        #region Events

        public event EventHandler<VoiceActionsEventArgs> BeforeRun;
        private void OnBeforeRun(VoiceActionsEventArgs args) => BeforeRun?.Invoke(this, args);

        public event EventHandler<VoiceActionsEventArgs> AfterRun;
        private void OnAfterRun(VoiceActionsEventArgs args) => AfterRun?.Invoke(this, args);

        private VoiceActionsEventArgs CreateArgs(string command) => new VoiceActionsEventArgs{ Text = command };

        #endregion

        #region Public methods

        public virtual void Run(string command)
        {
            OnBeforeRun(CreateArgs(command));

            Action?.Invoke(command);

            OnAfterRun(CreateArgs(command));
        }

        public virtual string[] GetSupportedCommands() => new string[0];

        public virtual string GetSupportedCommandsText() => $@"Supported commands:
{string.Join(Environment.NewLine, GetSupportedCommands())}";

        #endregion

        #region IDisposable

        public virtual void Dispose()
        {
        }

        #endregion
    }
}
