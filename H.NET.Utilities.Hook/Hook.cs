using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace H.NET.Utilities
{
    public abstract class Hook : IDisposable
    {
        public static Keys FromString(string text) => Enum.TryParse<Keys>(text, true, out var result) ? result : Keys.None;

        #region Properties

        public string Name { get; }
        public int HookId { get; }

        private IntPtr HookHandle { get; set; }
        private Win32.HookProc _hookAction;

        #endregion

        #region Events

        public event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        #endregion

        protected Hook(string name, int hookId)
        {
            Name = name;
            HookId = hookId;
        }

        private static void CheckHandle(IntPtr handle)
        {
            if (handle == null || handle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        public void Start()
        {
            Trace.WriteLine($"Starting hook '{Name}'...", $"Hook.StartHook [{Thread.CurrentThread.Name}]");

            _hookAction = Callback;
            var moduleHandle = Win32.GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);

            HookHandle = Win32.SetWindowsHookEx(HookId, _hookAction, moduleHandle, 0);
            CheckHandle(HookHandle);
        }

        protected abstract void InternalCallback(int nCode, int wParam, IntPtr lParam);

        private int Callback(int nCode, int wParam, IntPtr lParam)
        {
            int result;

            try
            {
                InternalCallback(nCode, wParam, lParam);
            }
            catch (Exception exception)
            {
                UnhandledException?.Invoke(this, new UnhandledExceptionEventArgs(exception, false));
            }
            finally
            {
                result = Win32.CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            }

            return result;
        }

        #region IDisposable

        public void Dispose()
        {
            Trace.WriteLine($"Stopping hook '{Name}'...", $"Hook.StartHook [{Thread.CurrentThread.Name}]");

            Win32.UnhookWindowsHookEx(HookHandle);
        }

        #endregion
    }
}