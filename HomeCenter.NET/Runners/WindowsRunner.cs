using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using H.NET.Core.Runners;
using HomeCenter.NET.Utilities;

namespace HomeCenter.NET.Runners
{
    public class WindowsRunner : Runner
    {
        #region Constructors

        public WindowsRunner()
        {
            AddAction("explorer", ExplorerCommand, "path");

            AddAction("show-window", ShowWindowCommand, "process_name");
            AddAction("show-process-names", ShowProcessNames);
        }

        #endregion

        #region Private methods

        private static string NormalizePath(string path) =>
            path?.Replace("\\\\", "\\").Replace("//", "\\").Replace("/", "\\");

        private void ExplorerCommand(string command) => Run($"start explorer \"{NormalizePath(command)}\"");

        private void ShowProcessNames(string command)
        {
            Print(@"Current process names:");
            var processes = Process
                .GetProcesses()
                .Where(i => !string.IsNullOrWhiteSpace(i.MainWindowTitle));

            foreach (var process in processes)
            {
                Print(process.ProcessName);
            }
        }

        private void ShowWindowCommand(string command)
        {
            var processes = Process
                .GetProcessesByName(command)
                .Where(i => !string.IsNullOrWhiteSpace(i.MainWindowTitle));

            var process = processes.FirstOrDefault();
            if (process == null)
            {
                Print($@"Process: {command} is not found");
                return;
            }

            var ptr = new HandleRef(null, process.MainWindowHandle);
            const int swShow = 5;
            User32Methods.ShowWindow(ptr, swShow);
            User32Methods.SetForegroundWindow(ptr);
            User32Methods.SetFocus(ptr);
        }

        #endregion
    }
}
