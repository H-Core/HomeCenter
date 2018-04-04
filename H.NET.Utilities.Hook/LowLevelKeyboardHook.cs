using System;
using System.Runtime.InteropServices;

namespace H.NET.Utilities
{
    public class LowLevelKeyboardHook : Hook
    {
        #region Properties

        public bool OneUpEvent { get; set; } = true;

        private Tuple<int, int> LastState { get; set; }

        #endregion

        #region Events

        public event EventHandler<KeyboardHookEventArgs> KeyDown;
        public event EventHandler<KeyboardHookEventArgs> KeyUp;

        #endregion

        #region Constructors

        public LowLevelKeyboardHook() : base("Low Level Keyboard Hook", Winuser.WH_KEYBOARD_LL)
        {
        }

        #endregion

        #region Protected methods

        protected override void InternalCallback(int code, int wParam, IntPtr lParamPtr)
        {
            if (code < 0)
            {
                return;
            }

            var lParam = (Win32.KeyboardHookStruct)Marshal.PtrToStructure(lParamPtr, typeof(Win32.KeyboardHookStruct));

            if (OneUpEvent)
            {
                if (LastState != null && LastState.Item1 == lParam.VirtualKeyCode && LastState.Item2 == lParam.Flags)
                {
                    return;
                }
                LastState = new Tuple<int, int>(lParam.VirtualKeyCode, lParam.Flags);
            }

            var isKeyDown = lParam.Flags >> 7 == 0;
            if (isKeyDown)
            {
                KeyDown?.Invoke(this, new KeyboardHookEventArgs(lParam));
            }
            else
            {
                KeyUp?.Invoke(this, new KeyboardHookEventArgs(lParam));
            }
        }

        #endregion

    }
}