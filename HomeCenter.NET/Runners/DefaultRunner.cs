using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
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
        #region Properties

        private InvariantStringDictionary<Action<string>> HandlerDictionary { get; } = new InvariantStringDictionary<Action<string>>();

        #endregion

        #region Constructors

        public DefaultRunner()
        {
            HandlerDictionary["run"] = RunProcess;
            HandlerDictionary["say"] = Say;
            HandlerDictionary["print"] = Print;
            HandlerDictionary["redirect"] = RunCommand;
            HandlerDictionary["paste"] = Paste;
            HandlerDictionary["clipboard"] = ClipboardCommand;
            HandlerDictionary["keyboard"] = KeyboardCommand;
            HandlerDictionary["sleep"] = SleepCommand;
            HandlerDictionary["show"] = ShowWindowCommand;
        }

        #endregion

        #region Public methods

        public override string[] GetSupportedCommands() => new[]
        {
            "RUN program.exe arguments",
            "SAY text",
            "PRINT text",
            "REDIRECT other_command_key",
            "PASTE text",
            "CLIPBOARD text",
            "KEYBOARD CONTROL+V",
            "SHOW process_name"
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
            HandlerDictionary.TryGetValue(prefix, out var handler);
            handler?.Invoke(postfix);
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

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool ShowWindow(HandleRef hwnd, int nCmdShow);
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(HandleRef hwnd);
        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(HandleRef hwnd);

        public static void ShowWindowCommand(string command)
        {
            var processes = Process.GetProcessesByName(command).Where(i => !string.IsNullOrWhiteSpace(i.MainWindowTitle));
            var process = processes.FirstOrDefault();
            if (process == null)
            {
                return;
            }

            var ptr = new HandleRef(null, process.MainWindowHandle);
            const int swShow = 5;
            ShowWindow(ptr, swShow);
            SetForegroundWindow(ptr);
            SetFocus(ptr);
        }

        private static void SleepCommand(string command)
        {
            var timeout = int.TryParse(command, out var result) ? result : 1000;

            // TODO: to sync await
            Thread.Sleep(timeout);
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
            else if (key.ToLowerInvariant() == "enter")
            {
                text = "RETURN";
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
