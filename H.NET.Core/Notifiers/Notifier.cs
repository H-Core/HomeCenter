using System;
using H.NET.Core.Attributes;

namespace H.NET.Core.Notifiers
{
    [DisableAutoCreateInstance]
    public class Notifier : Module, INotifier
    {
        #region Properties

        public string Command { get; set; }

        #endregion

        #region Events

        public event EventHandler AfterEvent;
        protected void OnEvent() => AfterEvent?.Invoke(this, EventArgs.Empty);

        #endregion

        #region Constructors

        public Notifier()
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

        #region Static methods

        public static Action<string> RunAction { get; set; }
        public static void Run(string command) => RunAction?.Invoke(command);

        #endregion

    }
}
