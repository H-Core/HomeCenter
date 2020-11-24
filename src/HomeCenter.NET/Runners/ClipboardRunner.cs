using System;
using H.Core.Runners;

namespace HomeCenter.NET.Runners
{
    public class ClipboardRunner : Runner
    {
        #region Properties

        public Action<string>? ClipboardAction { private get; set; }
        public Func<string?>? ClipboardFunc { private get; set; }

        #endregion

        #region Constructors

        public ClipboardRunner()
        {
            AddAction("copy", CopyCommand, "text");
            AddAction("paste", PasteCommand, "text");
            AddAction("clipboard", ClipboardCommand, "text");

            AddVariable("$clipboard$", () => ClipboardFunc?.Invoke());
        }

        #endregion

        #region Private methods

        private void ClipboardCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            command = command.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
            ClipboardAction?.Invoke(command);
        }
        
        private void CopyCommand(string command)
        {
            Run("keyboard Control+C");
            Run("sleep 5");

            //await Task.Delay(5);
        }

        private void PasteCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            ClipboardCommand(command);
            Run("keyboard Control+V");
        }

        #endregion
    }
}
