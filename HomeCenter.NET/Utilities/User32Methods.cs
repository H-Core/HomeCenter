using System.Runtime.InteropServices;

namespace HomeCenter.NET.Utilities
{
    public static class User32Methods
    {
        [DllImport("user32.dll")]
        public static extern int GetForegroundWindow();

        [DllImport("user32")]
        private static extern uint GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        public static int GetWindowProcessId(int hwnd)
        {
            GetWindowThreadProcessId(hwnd, out var pid);

            return pid;
        }
    }
}
