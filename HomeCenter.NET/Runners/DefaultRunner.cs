using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;
using H.NET.Core.Runners;
using H.NET.Core.Utilities;
using H.NET.Utilities;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Runners
{
    public class DefaultRunner : Runner
    {
        #region Properties

        public static Action ShowSettingsAction { get; set; }
        public static Action ShowCommandsAction { get; set; }
        public static Action StartRecordAction { get; set; }
        public static Action<string> ClipboardAction { get; set; }
        public static Func<string> ClipboardFunc { get; set; }

        #endregion

        #region Constructors

        public DefaultRunner()
        {
            AddAction("run", RunProcess, "program.exe arguments");
            AddAction("say", Say, "text");
            AddAction("print", Print, "text");
            AddAction("paste", Paste, "text");
            AddAction("clipboard", ClipboardCommand, "text");
            AddAction("keyboard", KeyboardCommand, "CONTROL+V");
            AddAsyncAction("sleep", async command => await Task.Delay(ToInt(command)), "integer");
            AddAction("sync-sleep", command => Thread.Sleep(ToInt(command)), "integer");
            AddAction("show", ShowWindowCommand, "process_name");
            AddAction("explorer", ExplorerCommand, "path");

            AddInternalAction("redirect", RunCommand, "other_command_key");
            AddInternalAction("show-settings", command => ShowSettingsAction?.Invoke());
            AddInternalAction("show-commands", command => ShowCommandsAction?.Invoke());
            AddInternalAction("start-record", command => StartRecordAction?.Invoke());
            AddInternalAction("deskband", DeskBandCommand);
            AddInternalAction("enable-module", command => ModuleManager.Instance.SetInstanceIsEnabled(command, true), "name");
            AddInternalAction("disable-module", command => ModuleManager.Instance.SetInstanceIsEnabled(command, false), "name");

            AddVariable("$clipboard$", () => ClipboardFunc?.Invoke());
        }

        #endregion

        #region Private methods

        private static void ExplorerCommand(string command)
        {
            command = command.Replace("\\\\", "\\").Replace("//", "\\").Replace("/", "\\");

            RunProcess($"explorer \"{command}\"");
        }

        private static async void DeskBandCommand(string command)
        {
            try
            {
                await IpcClient.Write(command, Options.IpcPortToDeskBand);
            }
            catch (Exception exception)
            {
                Log(exception.Message);
            }
        }

        private static void RunProcess(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            var values = command.SplitOnlyFirstIgnoreQuote(' ');

            var path = values[0].Trim('\"', '\\').Replace("\\\"", "\"").Replace("\\\\", "\\").Replace("\\", "/");
            Process.Start(path, values[1]);
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

        private static int ToInt(string text, int defaultValue = 1000) =>
            int.TryParse(text, out var result) ? result : defaultValue;

        private static void ClipboardCommand(string command)
        {
            if (string.IsNullOrWhiteSpace(command))
            {
                return;
            }

            command = command.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
            ClipboardAction?.Invoke(command);
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
