using System;
using System.Diagnostics;
using System.Linq;
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
            "PASTE text",
            "CLIPBOARD text",
            "KEYBOARD CONTROL+V"
        };

        #endregion

        #region Private methods

        protected override void RunInternal(string key, Command command)
        {
            foreach (var line in command.Commands)
            {
                RunSingleLine(line.Text);
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

                case "clipboard":
                    ClipboardCommand(postfix);
                    break;

                case "keyboard":
                    KeyboardCommand(postfix);
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

        private static void ClipboardCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            Clipboard.SetText(command);
        }

        private static VirtualKeyCode ToVirtualKey(string key)
        {
            string text;
            if (key.Length == 1)
            {
                text = $"VK_{key}";
            }
            else if (key.ToLowerInvariant() == "alt")
            {
                text = "MENU";
            }
            else if (key.ToLowerInvariant() == "ctrl")
            {
                text = "CONTROL";
            }
            else
            {
                text = key;
            }

            return (VirtualKeyCode)Enum.Parse(typeof(VirtualKeyCode), text, true);
        } 

        private static void KeyboardCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            var keys = command.Split('+');
            var mainKey = ToVirtualKey(keys.LastOrDefault());
            var otherKeys = keys.Take(keys.Length - 1).Select(ToVirtualKey);
            
            new InputSimulator().Keyboard.ModifiedKeyStroke(otherKeys, mainKey);
        }

        private static void Paste(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            ClipboardCommand(command);
            KeyboardCommand("Control+V");
        }

        #endregion
    }
}
