using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using H.NET.Core.Runners;

namespace HomeCenter.NET.Runners
{
    public class WindowsRunner : Runner
    {
        #region Constructors

        public WindowsRunner()
        {
            AddAction("explorer", ExplorerCommand, "path");

            AddAction("show-window", ShowWindowCommand, "process_name");
        }

        #endregion

        #region Private methods

        private static string NormalizePath(string path) =>
            path?.Replace("\\\\", "\\").Replace("//", "\\").Replace("/", "\\");

        private void ExplorerCommand(string command) => Run($"start explorer \"{NormalizePath(command)}\"");

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

        #endregion
    }
}
