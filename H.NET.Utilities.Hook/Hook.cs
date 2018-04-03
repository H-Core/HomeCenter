using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;

namespace H.NET.Utilities
{
    public class Hook
    {
        public static Keys FromString(string text) => Enum.TryParse<Keys>(text, true, out var result) ? result : Keys.None;


        #region Windows constants

        //values from Winuser.h in Microsoft SDK.
        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level mouse input events.
        /// </summary>
        private const int WH_MOUSE_LL = 14;

        /// <summary>
        /// Windows NT/2000/XP: Installs a hook procedure that monitors low-level keyboard  input events.
        /// </summary>
        private const int WH_KEYBOARD_LL = 13;

        /// <summary>
        /// Installs a hook procedure that monitors mouse messages. For more information, see the MouseProc hook procedure. 
        /// </summary>
        private const int WH_MOUSE = 7;

        /// <summary>
        /// Installs a hook procedure that monitors keystroke messages. For more information, see the KeyboardProc hook procedure. 
        /// </summary>
        private const int WH_KEYBOARD = 2;

        /// <summary>
        /// The WM_MOUSEMOVE message is posted to a window when the cursor moves. 
        /// </summary>
        private const int WM_MOUSEMOVE = 0x200;

        /// <summary>
        /// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button 
        /// </summary>
        private const int WM_LBUTTONDOWN = 0x201;

        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button
        /// </summary>
        private const int WM_RBUTTONDOWN = 0x204;

        /// <summary>
        /// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button 
        /// </summary>
        private const int WM_MBUTTONDOWN = 0x207;

        /// <summary>
        /// The WM_LBUTTONUP message is posted when the user releases the left mouse button 
        /// </summary>
        private const int WM_LBUTTONUP = 0x202;

        /// <summary>
        /// The WM_RBUTTONUP message is posted when the user releases the right mouse button 
        /// </summary>
        private const int WM_RBUTTONUP = 0x205;

        /// <summary>
        /// The WM_MBUTTONUP message is posted when the user releases the middle mouse button 
        /// </summary>
        private const int WM_MBUTTONUP = 0x208;

        /// <summary>
        /// The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button 
        /// </summary>
        private const int WM_LBUTTONDBLCLK = 0x203;

        /// <summary>
        /// The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button 
        /// </summary>
        private const int WM_RBUTTONDBLCLK = 0x206;

        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button 
        /// </summary>
        private const int WM_MBUTTONDBLCLK = 0x209;

        /// <summary>
        /// The WM_MOUSEWHEEL message is posted when the user presses the mouse wheel. 
        /// </summary>
        private const int WM_MOUSEWHEEL = 0x020A;

        private const int WM_XBUTTONDOWN = 0x020B;
        private const int WM_XBUTTONUP = 0x020C;
        private const int WM_XBUTTONDBLCLK = 0x020D;

        private const int WM_NCXBUTTONDOWN = 0x00AB;
        private const int WM_NCXBUTTONUP = 0x00AC;
        private const int WM_NCXBUTTONDBLCLK = 0x00AD;

        /// <summary>
        /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem 
        /// key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
        /// </summary>
        private const int WM_KEYDOWN = 0x100;

        /// <summary>
        /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem 
        /// key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, 
        /// or a keyboard key that is pressed when a window has the keyboard focus.
        /// </summary>
        private const int WM_KEYUP = 0x101;

        /// <summary>
        /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user 
        /// presses the F10 key (which activates the menu bar) or holds down the ALT key and then 
        /// presses another key. It also occurs when no window currently has the keyboard focus; 
        /// in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that 
        /// receives the message can distinguish between these two contexts by checking the context 
        /// code in the lParam parameter. 
        /// </summary>
        private const int WM_SYSKEYDOWN = 0x104;

        /// <summary>
        /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user 
        /// releases a key that was pressed while the ALT key was held down. It also occurs when no 
        /// window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent 
        /// to the active window. The window that receives the message can distinguish between 
        /// these two contexts by checking the context code in the lParam parameter. 
        /// </summary>
        private const int WM_SYSKEYUP = 0x105;

        private const byte VK_SHIFT = 0x10;
        private const byte VK_CAPITAL = 0x14;
        private const byte VK_NUMLOCK = 0x90;

        #endregion

        #region PInvoke
        private enum HookType : int
        {
            WhJournalrecord = 0,
            WhJournalplayback = 1,
            WhKeyboard = 2,
            WhGetmessage = 3,
            WhCallwndproc = 4,
            WhCbt = 5,
            WhSysmsgfilter = 6,
            WhMouse = 7,
            WhHardware = 8,
            WhDebug = 9,
            WhShell = 10,
            WhForegroundidle = 11,
            WhCallwndprocret = 12,
            WhKeyboardLl = 13,
            WhMouseLl = 14
        }

        /// <summary>
        /// The Point structure defines the X- and Y- coordinates of a point. 
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/gdi/rectangl_0tiq.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        private struct Point
        {
            /// <summary>
            /// Specifies the X-coordinate of the point. 
            /// </summary>
            public int X;
            /// <summary>
            /// Specifies the Y-coordinate of the point. 
            /// </summary>
            public int Y;
        }

        /// <summary>
        /// The MSLLHOOKSTRUCT structure contains information about a low-level keyboard input event. 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        private struct MouseLLHookStruct
        {
            /// <summary>
            /// Specifies a Point structure that contains the X- and Y-coordinates of the cursor, in screen coordinates. 
            /// </summary>
            public Point Point;
            /// <summary>
            /// If the message is WM_MOUSEWHEEL, the high-order word of this member is the wheel delta. 
            /// The low-order word is reserved. A positive value indicates that the wheel was rotated forward, 
            /// away from the user; a negative value indicates that the wheel was rotated backward, toward the user. 
            /// One wheel click is defined as WHEEL_DELTA, which is 120. 
            ///If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP,
            /// or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
            /// and the low-order word is reserved. This value can be one or more of the following values. Otherwise, MouseData is not used. 
            ///XBUTTON1
            ///The first X button was pressed or released.
            ///XBUTTON2
            ///The second X button was pressed or released.
            /// </summary>
            public int MouseData;
            /// <summary>
            /// Specifies the event-injected flag. An application can use the following value to test the mouse Flags. Value Purpose 
            ///LLMHF_INJECTED Test the event-injected flag.  
            ///0
            ///Specifies whether the event was injected. The value is 1 if the event was injected; otherwise, it is 0.
            ///1-15
            ///Reserved.
            /// </summary>
            public int Flags;
            /// <summary>
            /// Specifies the Time stamp for this message.
            /// </summary>
            public int Time;
            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            public int ExtraInfo;
        }

        /// <summary>
        /// The KBDLLHOOKSTRUCT structure contains information about a low-level keyboard input event. 
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/library/default.asp?url=/library/en-us/winui/winui/windowsuserinterface/windowing/hooks/hookreference/hookstructures/cwpstruct.asp
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct KeyboardHookStruct
        {
            /// <summary>
            /// Specifies a virtual-key code. The code must be a value in the range 1 to 254. 
            /// </summary>
            public int VirtualKeyCode;
            /// <summary>
            /// Specifies a hardware scan code for the key. 
            /// </summary>
            public int ScanCode;
            /// <summary>
            /// Specifies the extended-key flag, event-injected flag, context code, and transition-state flag.
            /// </summary>
            public int Flags;
            /// <summary>
            /// Specifies the Time stamp for this message.
            /// </summary>
            public int Time;
            /// <summary>
            /// Specifies extra information associated with the message. 
            /// </summary>
            public int ExtraInfo;
        }

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
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, int wParam, IntPtr lParam);

        /// <summary>
        /// An application-defined or library-defined callback function used with the SetWindowsHookEx function. The system calls this function whenever an application calls the GetMessage or PeekMessage function and there is a keyboard message (WM_KEYUP or WM_KEYDOWN) to be processed.
        /// </summary>
        /// <param name="code">A code the hook procedure uses to determine how to process the message. If code is less than zero, the hook procedure must pass the message to the CallNextHookEx function without further processing and should return the value returned by CallNextHookEx.</param>
        /// <param name="wParam">The virtual-key code of the key that generated the keystroke message.</param>
        /// <param name="lParam">The repeat count, scan code, extended-key flag, context code, previous key-state flag, and transition-state flag. For more information about the lParam parameter, see Keystroke Message Flags.</param>
        /// <returns>If code is less than zero, the hook procedure must return the value returned by CallNextHookEx. If code is greater than or equal to zero, and the hook procedure did not process the message, it is highly recommended that you call CallNextHookEx and return the value it returns; otherwise bad stuff.</returns>
        private delegate int HookProc(int code, int wParam, IntPtr lParam);
        #endregion

        public string Name { get; }

        /// <summary>
        /// When true, suspends firing of the hook notification events
        /// </summary>
        private bool _isPause;
        public bool IsPaused {
            get => _isPause;
            set {
                if (value != _isPause && value == true)
                    StopHook();

                if (value != _isPause && value == false)
                    StartHook();

                _isPause = value;
            }
        }

        public bool OneUpEvent { get; set; } = true;

        public event EventHandler<KeyboardHookEventArgs> KeyDown;
        public event EventHandler<KeyboardHookEventArgs> KeyUp;
        public event EventHandler<MouseEventExtArgs> MouseUp;
        public event EventHandler<MouseEventExtArgs> MouseDown;
        public event EventHandler<MouseEventExtArgs> MouseClick;
        public event EventHandler<MouseEventExtArgs> MouseClickExt;
        public event EventHandler<MouseEventExtArgs> MouseDoubleClick;
        public event EventHandler<MouseEventExtArgs> MouseWheel;

        private HookProc _keyboardHookAction;
        private HookProc _mouseHookAction;

        private IntPtr KeyboardHandle { get; set; }
        private IntPtr MouseHandle { get; set; }

        public Hook(string name)
        {
            Name = name;
            StartHook();
        }

        private static void CheckHandle(IntPtr handle)
        {
            if (handle == null || handle == IntPtr.Zero)
            {
                throw new Win32Exception(Marshal.GetLastWin32Error());
            }
        }

        private void StartHook()
        {
            Trace.WriteLine($"Starting hook '{Name}'...", $"Hook.StartHook [{Thread.CurrentThread.Name}]");

            _keyboardHookAction = KeyboardCallback;
            _mouseHookAction = MouseCallback;
            var moduleHandle = GetModuleHandle(Process.GetCurrentProcess().MainModule.ModuleName);

            KeyboardHandle = SetWindowsHookEx(HookType.WhKeyboardLl, _keyboardHookAction, moduleHandle, 0);
            CheckHandle(KeyboardHandle);

            MouseHandle = SetWindowsHookEx(HookType.WhMouseLl, _mouseHookAction, moduleHandle, 0);
            CheckHandle(MouseHandle);
        }

        private void StopHook()
        {
            Trace.WriteLine($"Stopping hook '{Name}'...", $"Hook.StartHook [{Thread.CurrentThread.Name}]");

            UnhookWindowsHookEx(KeyboardHandle);
            UnhookWindowsHookEx(MouseHandle);
        }

        private Tuple<int, int> LastState { get; set; }

        private void SendKeyboardEvent(int code, int wParam, IntPtr lParamPtr)
        {
            if (IsPaused || code < 0) 
            {
                return;
            }

            var lParam = (KeyboardHookStruct)Marshal.PtrToStructure(lParamPtr, typeof(KeyboardHookStruct));

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
            

            //const int _wmKeydown = 0x100;
            //const int _wmKeyup = 0x101;
            //const int _wmSyskeydown = 0x0104;
            //const int _wmSyskeyup = 0x105;
            //Trace.WriteLine($"{isKeyDown}");
            //if (wParam.ToInt32() == _wmSyskeydown || wParam.ToInt32() == _wmKeydown)

            //if (wParam.ToInt32() == _wmSyskeyup || wParam.ToInt32() == _wmKeyup)
        }

        private int KeyboardCallback(int nCode, int wParam, IntPtr lParam)
        {
            int result;

            try
            {
                SendKeyboardEvent(nCode, wParam, lParam);
            }
            finally
            {
                result = CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            }

            return result;
        }

        private int MouseCallback(int nCode, int wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                //Marshall the data from callback.
                var mouseHookStruct = (MouseLLHookStruct)Marshal.PtrToStructure(lParam, typeof(MouseLLHookStruct));

                //detect button clicked
                MouseButtons button = MouseButtons.None;
                short mouseDelta = 0;
                int clickCount = 0;
                bool mouseDown = false;
                bool mouseUp = false;

                switch (wParam)
                {
                    case WM_LBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.Left;
                        clickCount = 1;
                        break;
                    case WM_LBUTTONDBLCLK:
                        button = MouseButtons.Left;
                        clickCount = 2;
                        break;
                    case WM_RBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.Right;
                        clickCount = 1;
                        break;
                    case WM_RBUTTONDBLCLK:
                        button = MouseButtons.Right;
                        clickCount = 2;
                        break;
                    case WM_XBUTTONDOWN:
                    case WM_NCXBUTTONDOWN:
                        mouseDown = true;
                        button = MouseButtons.XButton1;
                        clickCount = 1;
                        break;
                    case WM_XBUTTONUP:
                    case WM_NCXBUTTONUP:
                        mouseUp = true;
                        button = MouseButtons.XButton1;
                        clickCount = 1;
                        break;
                    case WM_XBUTTONDBLCLK:
                    case WM_NCXBUTTONDBLCLK:
                        button = MouseButtons.XButton1;
                        clickCount = 2;
                        break;
                    case WM_MOUSEWHEEL:
                        //If the message is WM_MOUSEWHEEL, the high-order word of MouseData member is the wheel delta. 
                        //One wheel click is defined as WHEEL_DELTA, which is 120. 
                        //(value >> 16) & 0xffff; retrieves the high-order word from the given 32-bit value
                        mouseDelta = (short)((mouseHookStruct.MouseData >> 16) & 0xffff);

                        //TODO: X BUTTONS (I havent them so was unable to test)
                        //If the message is WM_XBUTTONDOWN, WM_XBUTTONUP, WM_XBUTTONDBLCLK, WM_NCXBUTTONDOWN, WM_NCXBUTTONUP, 
                        //or WM_NCXBUTTONDBLCLK, the high-order word specifies which X button was pressed or released, 
                        //and the low-order word is reserved. This value can be one or more of the following values. 
                        //Otherwise, MouseData is not used. 
                        break;

                    case WM_MOUSEMOVE:
                        break;

                    default:
                        mouseDown = true;
                        break;
                }

                //generate event 
                var e = new MouseEventExtArgs(
                                                   button,
                                                   clickCount,
                                                   mouseHookStruct.Point.X,
                                                   mouseHookStruct.Point.Y,
                                                   mouseDelta);

                //Mouse up
                if (mouseUp)
                {
                    MouseUp?.Invoke(null, e);
                }

                //Mouse down
                if (mouseDown)
                {
                    e.SpecialButton = mouseHookStruct.MouseData > 0 ?
                        (int)Math.Log(mouseHookStruct.MouseData, 2) : 0;
                    MouseDown?.Invoke(null, e);
                }

                //If someone listens to click and a click is heppened
                if (clickCount > 0)
                {
                    MouseClick?.Invoke(null, e);
                }

                //If someone listens to click and a click is heppened
                if (clickCount > 0)
                {
                    MouseClickExt?.Invoke(null, e);
                }

                //If someone listens to double click and a click is heppened
                if (clickCount == 2)
                {
                    MouseDoubleClick?.Invoke(null, e);
                }

                //Wheel was moved
                if (mouseDelta != 0)
                {
                    MouseWheel?.Invoke(null, e);
                }
                /*
                //If someone listens to move and there was a change in coordinates raise move event
                if (m_OldX != mouseHookStruct.Point.X || m_OldY != mouseHookStruct.Point.Y)
                {
                    m_OldX = mouseHookStruct.Point.X;
                    m_OldY = mouseHookStruct.Point.Y;
                    if (s_MouseMove != null)
                    {
                        s_MouseMove.Invoke(null, e);
                    }

                    if (s_MouseMoveExt != null)
                    {
                        s_MouseMoveExt.Invoke(null, e);
                    }
                }
                */
                if (e.Handled)
                {
                    return -1;
                }
            }

            //call next hook
            return CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
        }

        ~Hook()
        {
            StopHook();
        }
    }
    /// <summary>
    /// Provides data for the MouseClickExt and MouseMoveExt events. It also provides a property Handled.
    /// Set this property to <b>true</b> to prevent further processing of the event in other applications.
    /// </summary>
    public class MouseEventExtArgs : MouseEventArgs
    {
        public int SpecialButton { get; set; }

        /// <summary>
        /// Initializes a new instance of the MouseEventArgs class. 
        /// </summary>
        /// <param name="buttons">One of the MouseButtons values indicating which mouse button was pressed.</param>
        /// <param name="clicks">The number of times a mouse button was pressed.</param>
        /// <param name="x">The x-coordinate of a mouse click, in pixels.</param>
        /// <param name="y">The y-coordinate of a mouse click, in pixels.</param>
        /// <param name="delta">A signed count of the number of detents the wheel has rotated.</param>
        public MouseEventExtArgs(MouseButtons buttons, int clicks, int x, int y, int delta)
            : base(buttons, clicks, x, y, delta)
        { }

        /// <summary>
        /// Initializes a new instance of the MouseEventArgs class. 
        /// </summary>
        /// <param name="e">An ordinary <see cref="MouseEventArgs"/> argument to be extended.</param>
        internal MouseEventExtArgs(MouseEventArgs e) : base(e.Button, e.Clicks, e.X, e.Y, e.Delta)
        { }

        private bool m_Handled;

        /// <summary>
        /// Set this property to <b>true</b> inside your event handler to prevent further processing of the event in other applications.
        /// </summary>
        public bool Handled {
            get { return m_Handled; }
            set { m_Handled = value; }
        }
    }

    public class MouseEventArgs : EventArgs
    {
        private readonly MouseButtons button;
        private readonly int clicks;
        private readonly int x;
        private readonly int y;
        private readonly int delta;

        /// <summary>Initializes a new instance of the <see cref="T:System.Windows.Forms.MouseEventArgs" /> class.</summary>
        /// <param name="button">One of the <see cref="T:System.Windows.Forms.MouseButtons" /> values that indicate which mouse button was pressed. </param>
        /// <param name="clicks">The number of times a mouse button was pressed. </param>
        /// <param name="x">The x-coordinate of a mouse click, in pixels. </param>
        /// <param name="y">The y-coordinate of a mouse click, in pixels. </param>
        /// <param name="delta">A signed count of the number of detents the wheel has rotated. </param>
        public MouseEventArgs(MouseButtons button, int clicks, int x, int y, int delta)
        {
            this.button = button;
            this.clicks = clicks;
            this.x = x;
            this.y = y;
            this.delta = delta;
        }

        /// <summary>Gets which mouse button was pressed.</summary>
        /// <returns>One of the <see cref="T:System.Windows.Forms.MouseButtons" /> values.</returns>
        public MouseButtons Button {
            get {
                return this.button;
            }
        }

        /// <summary>Gets the number of times the mouse button was pressed and released.</summary>
        /// <returns>An <see cref="T:System.Int32" /> that contains the number of times the mouse button was pressed and released.</returns>
        public int Clicks {
            get {
                return this.clicks;
            }
        }

        /// <summary>Gets the x-coordinate of the mouse during the generating mouse event.</summary>
        /// <returns>The x-coordinate of the mouse, in pixels.</returns>
        public int X {
            get {
                return this.x;
            }
        }

        /// <summary>Gets the y-coordinate of the mouse during the generating mouse event.</summary>
        /// <returns>The y-coordinate of the mouse, in pixels.</returns>
        public int Y {
            get {
                return this.y;
            }
        }

        /// <summary>Gets a signed count of the number of detents the mouse wheel has rotated, multiplied by the WHEEL_DELTA constant. A detent is one notch of the mouse wheel.</summary>
        /// <returns>A signed count of the number of detents the mouse wheel has rotated, multiplied by the WHEEL_DELTA constant.</returns>
        public int Delta {
            get {
                return this.delta;
            }
        }

        /// <summary>Gets the location of the mouse during the generating mouse event.</summary>
        /// <returns>A <see cref="T:System.Drawing.Point" /> that contains the x- and y- mouse coordinates, in pixels, relative to the upper-left corner of the form.</returns>
        //public Point Location {
        //    get {
        //        return new Point(this.x, this.y);
        //    }
        //}
    }

    public class KeyboardHookEventArgs
    {
        #region PInvoke

        [DllImport("user32.dll")]
        private static extern short GetKeyState(VirtualKey key);

        private enum VirtualKey
        {
            LWin = 0x5B,
            RWin = 0x5C,
            LShift = 0xA0,
            RShift = 0xA1,
            LControl = 0xA2,
            RControl = 0xA3,
            LAlt = 0xA4, //aka VK_LMENU
            RAlt = 0xA5, //aka VK_RMENU
        }

        private const int KeyPressed = 0x8000;

        #endregion

        public Keys Key { get; }

        public bool IsAltPressed => IsLAltPressed || IsRAltPressed;
        public bool IsLAltPressed { get; }
        public bool IsRAltPressed { get; }

        public bool IsCtrlPressed => IsLCtrlPressed || IsRCtrlPressed;
        public bool IsLCtrlPressed { get; }
        public bool IsRCtrlPressed { get; }

        public bool IsShiftPressed => IsLShiftPressed || IsRShiftPressed;
        public bool IsLShiftPressed { get; }
        public bool IsRShiftPressed { get; }

        public bool IsWinPressed => IsLWinPressed || IsRWinPressed;
        public bool IsLWinPressed { get; }
        public bool IsRWinPressed { get; }

        private static bool Check(Keys key, VirtualKey virtualKey, Keys realKey) =>
            Convert.ToBoolean(GetKeyState(virtualKey) & KeyPressed) || key == realKey;

        internal KeyboardHookEventArgs(Hook.KeyboardHookStruct lParam)
        {
            Key = (Keys)lParam.VirtualKeyCode;

            //Control.ModifierKeys doesn't capture alt/win, and doesn't have r/l granularity
            IsLAltPressed = Check(Key, VirtualKey.LAlt, Keys.LMenu);
            IsRAltPressed = Check(Key, VirtualKey.RAlt, Keys.RMenu);

            IsLCtrlPressed = Check(Key, VirtualKey.LControl, Keys.LControlKey);
            IsRCtrlPressed = Check(Key, VirtualKey.RControl, Keys.RControlKey);

            IsLShiftPressed = Check(Key, VirtualKey.LShift, Keys.LShiftKey);
            IsRShiftPressed = Check(Key, VirtualKey.RShift, Keys.RShiftKey);

            IsLWinPressed = Check(Key, VirtualKey.LWin, Keys.LWin);
            IsRWinPressed = Check(Key, VirtualKey.RWin, Keys.RWin);

            if (new[] { Keys.LMenu, Keys.RMenu, Keys.LControlKey, Keys.RControlKey, Keys.LShiftKey, Keys.RShiftKey, Keys.LWin, Keys.RWin }.Contains(Key))
            {
                Key = Keys.None;
            }
        }

        public override string ToString() => $"Key={Key}; Win={IsWinPressed}; Alt={IsAltPressed}; Ctrl={IsCtrlPressed}; Shift={IsShiftPressed}";
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
        ImeConvert = FinalMode | MButton, // 0x0000001C
        ImeNonconvert = ImeConvert | LButton, // 0x0000001D
        ImeAccept = ImeConvert | RButton, // 0x0000001E
        ImeAceept = ImeAccept, // 0x0000001E
        ImeModeChange = ImeAceept | LButton, // 0x0000001F
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
        OemPipe = Oem3 | ImeConvert, // 0x000000DC
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

    [Flags]
    public enum MouseButtons
    {
        Left = 1048576, // 0x00100000
        None = 0,
        Right = 2097152, // 0x00200000
        Middle = 4194304, // 0x00400000
        XButton1 = 8388608, // 0x00800000
        XButton2 = 16777216, // 0x01000000
    }
}