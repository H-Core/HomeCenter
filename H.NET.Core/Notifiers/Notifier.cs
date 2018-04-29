using System;
using H.NET.Core.Attributes;

namespace H.NET.Core.Notifiers
{
    [AllowMultipleInstance(false)]
    public class Notifier : Module, INotifier
    {
        #region Properties

        private string Command { get; set; }

        #endregion

        #region Events

        public event EventHandler AfterEvent;
        protected void OnEvent() => AfterEvent?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Constructors

        protected Notifier()
        {
            AddSetting(nameof(Command), o => Command = o, o => true, string.Empty);

            AfterEvent += (sender, args) =>
            {
                Log($"{Name} AfterEvent");
                try
                {
                    if (string.IsNullOrWhiteSpace(Command))
                    {
                        Log("Command is empty");
                        return;
                    }

                    Run(Command);
                    Log($"Run command: {Command}");
                }
                catch (Exception exception)
                {
                    Log($"Exception: {exception}");
                }
            };
        }

        #endregion

    }
}
