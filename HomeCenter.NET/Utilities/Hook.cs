using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace HomeCenter.NET.Utilities
{
    public class Hook
    {
        public static Keys FromString(string text) => Enum.TryParse<Keys>(text, true, out var result) ? result : Keys.None;

        #region PInvoke
        private enum HookType : int
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        public struct KBDLLHOOKSTRUCT
        {
            public UInt32 vkCode;
            public UInt32 scanCode;
            public UInt32 flags;
            public UInt32 time;
            public IntPtr extraInfo;
        }

        private int WM_KEYDOWN = 0x100;
        private int WM_KEYUP = 0x101;

        private int WM_SYSKEYDOWN = 0x0104;
        private int WM_SYSKEYUP = 0x105;

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        /// <summary>
        /// Installs an application-defined hook procedure into a hook chain.
        /// </summary>
        /// <param name="idHook">The type of hook procedure to be installed.</param>
        /// <param name="lpfn">Reference to the hook callback method.</param>
        /// <param name="hMod">A handle to the DLL containing the hook procedure pointed to by the lpfn parameter. The hMod parameter must be set to NULL if the dwThreadId parameter specifies a thread created by the current process and if the hook procedure is within the code associated with the current process.</param>
        /// <param name="dwThreadId">The identifier of the thread with which the hook procedure is to be associated. If this parameter is zero, the hook procedure is associated with all existing threads running in the same desktop as the calling thread.</param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure. If the function fails, the return value is NULL. To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll")]
        private static extern IntPtr SetWindowsHookEx(HookType idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        /// <summary>
        /// Removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="hhk">A handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to SetWindowsHookEx.</param>
        /// <returns>If the function succeeds, the return value is nonzero. If the function fails, the return value is zero. To get extended error information, call GetLastError.</returns>
        [DllImport("user32.dll")]
        private static extern int UnhookWindowsHookEx(IntPtr hhk);

        /// <summary>
        /// Passes the hook information to the next hook procedure in the current hook chain. A hook procedure can call this function either before or after processing the hook information.
        /// </summary>
        /// <param name="hhk">This parameter is ignored.</param>
        /// <param name="nCode">The hook code passed to the current hook procedure. The next hook procedure uses this code to determine how to process the hook information.</param>
        /// <param name="wParam">The wParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
        /// <param name="lParam">The lParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
        /// <returns>This value is returned by the next hook procedure in the chain. The current hook procedure must also return this value. The meaning of the return value depends on the hook type. For more information, see the descriptions of the individual hook procedures.</returns>
        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        /// <summary>
        /// An application-defined or library-defined callback function used with the SetWindowsHookEx function. The system calls this function whenever an application calls the GetMessage or PeekMessage function and there is a keyboard message (WM_KEYUP or WM_KEYDOWN) to be processed.
        /// </summary>
        /// <param name="code">A code the hook procedure uses to determine how to process the message. If code is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing and should return the value returned by CallNextHookEx.</param>
        /// <param name="wParam">The virtual-key code of the key that generated the keystroke message.</param>
        /// <param name="lParam">The repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag. For more information about the lParam parameter, see Keystroke Message Flags.</param>
        /// <returns>If code is less than zero, the hook procedure must return the value returned by CallNextHookEx. If code is greater than or equal to zero, and the hook procedure did not process the message, it is highly recommended that you call CallNextHookEx and return the value it returns; otherwise bad stuff.</returns>
        private delegate int HookProc(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
        #endregion

        public string Name { get; private set; }

        /// <summary>
        /// When true, suspends firing of the hook notification events
        /// </summary>
        public bool isPaused {
            get {
                return _ispaused;
            }
            set {
                if (value != _ispaused && value == true)
                    StopHook();

                if (value != _ispaused && value == false)
                    StartHook();

                _ispaused = value;
            }
        }
        bool _ispaused = false;

        public delegate void KeyDownEventDelegate(KeyboardHookEventArgs e);
        public KeyDownEventDelegate KeyDownEvent = delegate { };

        public delegate void KeyUpEventDelegate(KeyboardHookEventArgs e);
        public KeyUpEventDelegate KeyUpEvent = delegate { };

        HookProc _hookproc;
        IntPtr _hhook;


        public Hook(string name)
        {
            Name = name;
            StartHook();
        }

        private void StartHook()
        {
            Trace.WriteLine(string.Format("Starting hook '{0}'...", Name), string.Format("Hook.StartHook [{0}]", Thread.CurrentThread.Name));

            _hookproc = new HookProc(HookCallback);
            _hhook = SetWindowsHookEx(HookType.WH_KEYBOARD_LL, _hookproc, GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName), 0);
            if (_hhook == null || _hhook == IntPtr.Zero)
            {
                Win32Exception LastError = new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private void StopHook()
        {
            Trace.WriteLine(string.Format("Stopping hook '{0}'...", Name), string.Format("Hook.StartHook [{0}]", Thread.CurrentThread.Name));

            UnhookWindowsHookEx(_hhook);
        }

        private int HookCallback(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            int result = 0;

            try
            {
                if (!isPaused && code >= 0)
                {
                    if (wParam.ToInt32() == WM_SYSKEYDOWN || wParam.ToInt32() == WM_KEYDOWN)
                        KeyDownEvent(new KeyboardHookEventArgs(lParam));

                    if (wParam.ToInt32() == WM_SYSKEYUP || wParam.ToInt32() == WM_KEYUP)
                        KeyUpEvent(new KeyboardHookEventArgs(lParam));
                }
            }
            finally
            {
                result = CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
            }

            return result;
        }

        ~Hook()
        {
            StopHook();
        }
    }

    public class KeyboardHookEventArgs
    {
        #region PInvoke
        [DllImport("user32.dll")]
        static extern short GetKeyState(VirtualKeyStates nVirtKey);

        private enum VirtualKeyStates : int
        {
            VK_LWIN = 0x5B,
            VK_RWIN = 0x5C,
            VK_LSHIFT = 0xA0,
            VK_RSHIFT = 0xA1,
            VK_LCONTROL = 0xA2,
            VK_RCONTROL = 0xA3,
            VK_LALT = 0xA4, //aka VK_LMENU
            VK_RALT = 0xA5, //aka VK_RMENU
        }

        private const int KEY_PRESSED = 0x8000;
        #endregion

        public Keys Key { get; private set; }

        public bool isAltPressed { get { return isLAltPressed || isRAltPressed; } }
        public bool isLAltPressed { get; private set; }
        public bool isRAltPressed { get; private set; }

        public bool isCtrlPressed { get { return isLCtrlPressed || isRCtrlPressed; } }
        public bool isLCtrlPressed { get; private set; }
        public bool isRCtrlPressed { get; private set; }

        public bool isShiftPressed { get { return isLShiftPressed || isRShiftPressed; } }
        public bool isLShiftPressed { get; private set; }
        public bool isRShiftPressed { get; private set; }

        public bool isWinPressed { get { return isLWinPressed || isRWinPressed; } }
        public bool isLWinPressed { get; private set; }
        public bool isRWinPressed { get; private set; }

        internal KeyboardHookEventArgs(Hook.KBDLLHOOKSTRUCT lParam)
        {
            this.Key = (Keys)lParam.vkCode;

            //Control.ModifierKeys doesn't capture alt/win, and doesn't have r/l granularity
            this.isLAltPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LALT) & KEY_PRESSED) || this.Key == Keys.LMenu;
            this.isRAltPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RALT) & KEY_PRESSED) || this.Key == Keys.RMenu;

            this.isLCtrlPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LCONTROL) & KEY_PRESSED) || this.Key == Keys.LControlKey;
            this.isRCtrlPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RCONTROL) & KEY_PRESSED) || this.Key == Keys.RControlKey;

            this.isLShiftPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LSHIFT) & KEY_PRESSED) || this.Key == Keys.LShiftKey;
            this.isRShiftPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RSHIFT) & KEY_PRESSED) || this.Key == Keys.RShiftKey;

            this.isLWinPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_LWIN) & KEY_PRESSED) || this.Key == Keys.LWin;
            this.isRWinPressed = Convert.ToBoolean(GetKeyState(VirtualKeyStates.VK_RWIN) & KEY_PRESSED) || this.Key == Keys.RWin;

            if (new[] { Keys.LMenu, Keys.RMenu, Keys.LControlKey, Keys.RControlKey, Keys.LShiftKey, Keys.RShiftKey, Keys.LWin, Keys.RWin }.Contains(this.Key))
                this.Key = Keys.None;
        }

        public override string ToString()
        {
            return string.Format("Key={0}; Win={1}; Alt={2}; Ctrl={3}; Shift={4}", new object[] { Key, isWinPressed, isAltPressed, isCtrlPressed, isShiftPressed });
        }
    }

    [Flags]
    public enum Keys
    {
        KeyCode = 65535, // 0x0000FFFF
        Modifiers = -65536, // -0x00010000
        None = 0,
        LButton = 1,
        RButton = 2,
        Cancel = RButton | LButton, // 0x00000003
        MButton = 4,
        XButton1 = MButton | LButton, // 0x00000005
        XButton2 = MButton | RButton, // 0x00000006
        Back = 8,
        Tab = Back | LButton, // 0x00000009
        LineFeed = Back | RButton, // 0x0000000A
        Clear = Back | MButton, // 0x0000000C
        Return = Clear | LButton, // 0x0000000D
        Enter = Return, // 0x0000000D
        ShiftKey = 16, // 0x00000010
        ControlKey = ShiftKey | LButton, // 0x00000011
        Menu = ShiftKey | RButton, // 0x00000012
        Pause = Menu | LButton, // 0x00000013
        Capital = ShiftKey | MButton, // 0x00000014
        CapsLock = Capital, // 0x00000014
        KanaMode = CapsLock | LButton, // 0x00000015
        HanguelMode = KanaMode, // 0x00000015
        HangulMode = HanguelMode, // 0x00000015
        JunjaMode = HangulMode | RButton, // 0x00000017
        FinalMode = ShiftKey | Back, // 0x00000018
        HanjaMode = FinalMode | LButton, // 0x00000019
        KanjiMode = HanjaMode, // 0x00000019
        Escape = KanjiMode | RButton, // 0x0000001B
        IMEConvert = FinalMode | MButton, // 0x0000001C
        IMENonconvert = IMEConvert | LButton, // 0x0000001D
        IMEAccept = IMEConvert | RButton, // 0x0000001E
        IMEAceept = IMEAccept, // 0x0000001E
        IMEModeChange = IMEAceept | LButton, // 0x0000001F
        Space = 32, // 0x00000020
        Prior = Space | LButton, // 0x00000021
        PageUp = Prior, // 0x00000021
        Next = Space | RButton, // 0x00000022
        PageDown = Next, // 0x00000022
        End = PageDown | LButton, // 0x00000023
        Home = Space | MButton, // 0x00000024
        Left = Home | LButton, // 0x00000025
        Up = Home | RButton, // 0x00000026
        Right = Up | LButton, // 0x00000027
        Down = Space | Back, // 0x00000028
        Select = Down | LButton, // 0x00000029
        Print = Down | RButton, // 0x0000002A
        Execute = Print | LButton, // 0x0000002B
        Snapshot = Down | MButton, // 0x0000002C
        PrintScreen = Snapshot, // 0x0000002C
        Insert = PrintScreen | LButton, // 0x0000002D
        Delete = PrintScreen | RButton, // 0x0000002E
        Help = Delete | LButton, // 0x0000002F
        D0 = Space | ShiftKey, // 0x00000030
        D1 = D0 | LButton, // 0x00000031
        D2 = D0 | RButton, // 0x00000032
        D3 = D2 | LButton, // 0x00000033
        D4 = D0 | MButton, // 0x00000034
        D5 = D4 | LButton, // 0x00000035
        D6 = D4 | RButton, // 0x00000036
        D7 = D6 | LButton, // 0x00000037
        D8 = D0 | Back, // 0x00000038
        D9 = D8 | LButton, // 0x00000039
        A = 65, // 0x00000041
        B = 66, // 0x00000042
        C = B | LButton, // 0x00000043
        D = 68, // 0x00000044
        E = D | LButton, // 0x00000045
        F = D | RButton, // 0x00000046
        G = F | LButton, // 0x00000047
        H = 72, // 0x00000048
        I = H | LButton, // 0x00000049
        J = H | RButton, // 0x0000004A
        K = J | LButton, // 0x0000004B
        L = H | MButton, // 0x0000004C
        M = L | LButton, // 0x0000004D
        N = L | RButton, // 0x0000004E
        O = N | LButton, // 0x0000004F
        P = 80, // 0x00000050
        Q = P | LButton, // 0x00000051
        R = P | RButton, // 0x00000052
        S = R | LButton, // 0x00000053
        T = P | MButton, // 0x00000054
        U = T | LButton, // 0x00000055
        V = T | RButton, // 0x00000056
        W = V | LButton, // 0x00000057
        X = P | Back, // 0x00000058
        Y = X | LButton, // 0x00000059
        Z = X | RButton, // 0x0000005A
        LWin = Z | LButton, // 0x0000005B
        RWin = X | MButton, // 0x0000005C
        Apps = RWin | LButton, // 0x0000005D
        Sleep = Apps | RButton, // 0x0000005F
        NumPad0 = 96, // 0x00000060
        NumPad1 = NumPad0 | LButton, // 0x00000061
        NumPad2 = NumPad0 | RButton, // 0x00000062
        NumPad3 = NumPad2 | LButton, // 0x00000063
        NumPad4 = NumPad0 | MButton, // 0x00000064
        NumPad5 = NumPad4 | LButton, // 0x00000065
        NumPad6 = NumPad4 | RButton, // 0x00000066
        NumPad7 = NumPad6 | LButton, // 0x00000067
        NumPad8 = NumPad0 | Back, // 0x00000068
        NumPad9 = NumPad8 | LButton, // 0x00000069
        Multiply = NumPad8 | RButton, // 0x0000006A
        Add = Multiply | LButton, // 0x0000006B
        Separator = NumPad8 | MButton, // 0x0000006C
        Subtract = Separator | LButton, // 0x0000006D
        Decimal = Separator | RButton, // 0x0000006E
        Divide = Decimal | LButton, // 0x0000006F
        F1 = NumPad0 | ShiftKey, // 0x00000070
        F2 = F1 | LButton, // 0x00000071
        F3 = F1 | RButton, // 0x00000072
        F4 = F3 | LButton, // 0x00000073
        F5 = F1 | MButton, // 0x00000074
        F6 = F5 | LButton, // 0x00000075
        F7 = F5 | RButton, // 0x00000076
        F8 = F7 | LButton, // 0x00000077
        F9 = F1 | Back, // 0x00000078
        F10 = F9 | LButton, // 0x00000079
        F11 = F9 | RButton, // 0x0000007A
        F12 = F11 | LButton, // 0x0000007B
        F13 = F9 | MButton, // 0x0000007C
        F14 = F13 | LButton, // 0x0000007D
        F15 = F13 | RButton, // 0x0000007E
        F16 = F15 | LButton, // 0x0000007F
        F17 = 128, // 0x00000080
        F18 = F17 | LButton, // 0x00000081
        F19 = F17 | RButton, // 0x00000082
        F20 = F19 | LButton, // 0x00000083
        F21 = F17 | MButton, // 0x00000084
        F22 = F21 | LButton, // 0x00000085
        F23 = F21 | RButton, // 0x00000086
        F24 = F23 | LButton, // 0x00000087
        NumLock = F17 | ShiftKey, // 0x00000090
        Scroll = NumLock | LButton, // 0x00000091
        LShiftKey = F17 | Space, // 0x000000A0
        RShiftKey = LShiftKey | LButton, // 0x000000A1
        LControlKey = LShiftKey | RButton, // 0x000000A2
        RControlKey = LControlKey | LButton, // 0x000000A3
        LMenu = LShiftKey | MButton, // 0x000000A4
        RMenu = LMenu | LButton, // 0x000000A5
        BrowserBack = LMenu | RButton, // 0x000000A6
        BrowserForward = BrowserBack | LButton, // 0x000000A7
        BrowserRefresh = LShiftKey | Back, // 0x000000A8
        BrowserStop = BrowserRefresh | LButton, // 0x000000A9
        BrowserSearch = BrowserRefresh | RButton, // 0x000000AA
        BrowserFavorites = BrowserSearch | LButton, // 0x000000AB
        BrowserHome = BrowserRefresh | MButton, // 0x000000AC
        VolumeMute = BrowserHome | LButton, // 0x000000AD
        VolumeDown = BrowserHome | RButton, // 0x000000AE
        VolumeUp = VolumeDown | LButton, // 0x000000AF
        MediaNextTrack = LShiftKey | ShiftKey, // 0x000000B0
        MediaPreviousTrack = MediaNextTrack | LButton, // 0x000000B1
        MediaStop = MediaNextTrack | RButton, // 0x000000B2
        MediaPlayPause = MediaStop | LButton, // 0x000000B3
        LaunchMail = MediaNextTrack | MButton, // 0x000000B4
        SelectMedia = LaunchMail | LButton, // 0x000000B5
        LaunchApplication1 = LaunchMail | RButton, // 0x000000B6
        LaunchApplication2 = LaunchApplication1 | LButton, // 0x000000B7
        OemSemicolon = MediaStop | Back, // 0x000000BA
        Oem1 = OemSemicolon, // 0x000000BA
        Oemplus = Oem1 | LButton, // 0x000000BB
        Oemcomma = LaunchMail | Back, // 0x000000BC
        OemMinus = Oemcomma | LButton, // 0x000000BD
        OemPeriod = Oemcomma | RButton, // 0x000000BE
        OemQuestion = OemPeriod | LButton, // 0x000000BF
        Oem2 = OemQuestion, // 0x000000BF
        Oemtilde = 192, // 0x000000C0
        Oem3 = Oemtilde, // 0x000000C0
        OemOpenBrackets = Oem3 | Escape, // 0x000000DB
        Oem4 = OemOpenBrackets, // 0x000000DB
        OemPipe = Oem3 | IMEConvert, // 0x000000DC
        Oem5 = OemPipe, // 0x000000DC
        OemCloseBrackets = Oem5 | LButton, // 0x000000DD
        Oem6 = OemCloseBrackets, // 0x000000DD
        OemQuotes = Oem5 | RButton, // 0x000000DE
        Oem7 = OemQuotes, // 0x000000DE
        Oem8 = Oem7 | LButton, // 0x000000DF
        OemBackslash = Oem3 | PageDown, // 0x000000E2
        Oem102 = OemBackslash, // 0x000000E2
        ProcessKey = Oem3 | Left, // 0x000000E5
        Packet = ProcessKey | RButton, // 0x000000E7
        Attn = Oem102 | CapsLock, // 0x000000F6
        Crsel = Attn | LButton, // 0x000000F7
        Exsel = Oem3 | D8, // 0x000000F8
        EraseEof = Exsel | LButton, // 0x000000F9
        Play = Exsel | RButton, // 0x000000FA
        Zoom = Play | LButton, // 0x000000FB
        NoName = Exsel | MButton, // 0x000000FC
        Pa1 = NoName | LButton, // 0x000000FD
        OemClear = NoName | RButton, // 0x000000FE
        Shift = 65536, // 0x00010000
        Control = 131072, // 0x00020000
        Alt = 262144, // 0x00040000
    }
}