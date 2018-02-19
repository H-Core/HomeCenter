using System;
using System.Diagnostics;
using System.Windows;
using WindowsInput;
using WindowsInput.Native;
using HomeCenter.NET.Runners.Core;
using HomeCenter.NET.Utilities;
using VoiceActions.NET.Utilities;

namespace HomeCenter.NET.Runners
{
    public class DefaultRunner : BaseRunner
    {
        #region Public methods

        public override string[] GetSupportedCommands() => new[]
        {
            "RUN program.exe arguments",
            "SAY text",
            "PRINT text",
            "REDIRECT command-key",
            "PASTE text"
        };

        #endregion

        #region Private methods

        protected override void RunInternal(string key, Command command)
        {
            var lines = command.Data.Split(Environment.NewLine.ToCharArray());
            foreach (var line in lines)
            {
                RunSingleLine(line);
            }
        }

        private void RunSingleLine(string command)
        {
            (var prefix, var postfix) = command.SplitOnlyFirstIgnoreQuote(' ');

            switch (prefix.ToLowerInvariant())
            {
                case "run":
                    RunProcess(postfix);
                    break;

                case "say":
                    Say(postfix);
                    break;

                case "print":
                    Print(postfix);
                    break;

                case "redirect":
                    RunCommand(postfix);
                    break;

                case "paste":
                    Paste(postfix);
                    break;
            }
        }

        private static void RunProcess(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            (var prefix, var postfix) = command.SplitOnlyFirstIgnoreQuote(' ');

            var path = prefix.Trim('\"', '\\').Replace("\\\"", "\"").Replace("\\\\", "\\").Replace("\\", "/");
            Process.Start(path, postfix);
        }

        private static void Paste(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            Clipboard.SetText(command);
            new InputSimulator().Keyboard.ModifiedKeyStroke(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_V);
        }

        #endregion
    }
}
