// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.

//*******************************************************************
//                  Code from Windows 7 SDK
//*******************************************************************

using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Security.Permissions;

namespace BelicsClass.UI
{
    /// <summary>
    /// タッチ操作／ジェスチャー操作を管理するクラスです。
    /// </summary>
    public class BL_TouchControl
    {
        /// <summary>
        /// タッチ対象のコントロール
        /// </summary>
        public Control control;

        /// <summary>
        /// タッチイベントハンドラー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void TouchEventHandler(BL_TouchControl sender, WMTouchEventArgs e);

        /// <summary>
        /// ジェスチャーイベントハンドラー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void GestureEventHandler(BL_TouchControl sender, WMGestureEventArgs e);

        /// <summary></summary>
        public event TouchEventHandler Touchdown;
        /// <summary></summary>
        public event TouchEventHandler Touchup;
        /// <summary></summary>
        public event TouchEventHandler TouchMove;

        /// <summary></summary>
        public event GestureEventHandler GestureBegin;
        /// <summary></summary>
        public event GestureEventHandler GestureEnd;
        /// <summary></summary>
        public event GestureEventHandler GestureZoom;
        /// <summary></summary>
        public event GestureEventHandler GesturePan;
        /// <summary></summary>
        public event GestureEventHandler GestureRotate;
        /// <summary></summary>
        public event GestureEventHandler GestureTwoFingerTap;
        /// <summary></summary>
        public event GestureEventHandler GestureRollover;

        /// <summary>
        /// 
        /// </summary>
        public class WMTouchEventArgs
        {
            /// <summary></summary>
            public Point Location;
            /// <summary></summary>
            public int Id;                 // contact ID
            /// <summary></summary>
            public int Mask;               // mask which fields in the structure are valid
            /// <summary></summary>
            public int Flags;              // flags
            /// <summary></summary>
            public int Time;               // touch event time
            /// <summary></summary>
            public Point Contact;
            /// <summary></summary>
            public Dictionary<int, TouchPoints> touchPoints = null;

            /// <summary></summary>
            public bool Handled = false;

            /// <summary></summary>
            public bool IsPrimaryContact
            {
                get { return (Flags & TOUCHEVENTF_PRIMARY) != 0; }
            }

            /// <summary></summary>
            public WMTouchEventArgs()
            {
            }
        }

        /// <summary>
        /// ジェスチャのイベントデータ
        /// </summary>
        public class WMGestureEventArgs
        {
            private int dwFlags;
            private int dwID;
            private IntPtr hwndTarget;
            private POINTS ptsLocation;
            private int dwInstanceID;
            private int dwSequenceID;
            private UInt64 ullArguments;
            private uint cbExtraArgs;

            /// <summary></summary>
            public int Flags { get { return dwFlags; } set { dwFlags = value; } }
            /// <summary></summary>
            public int Id { get { return dwID; } set { dwID = value; } }
            /// <summary></summary>
            public IntPtr TargetHandle { get { return hwndTarget; } set { hwndTarget = value; } }
            /// <summary></summary>
            public short LocationX { get { return ptsLocation.x; } set { ptsLocation.x = value; } }
            /// <summary></summary>
            public short LocationY { get { return ptsLocation.y; } set { ptsLocation.y = value; } }
            /// <summary></summary>
            public int InstanceID { get { return dwInstanceID; } set { dwInstanceID = value; } }
            /// <summary></summary>
            public int SequenceID { get { return dwSequenceID; } set { dwSequenceID = value; } }
            /// <summary></summary>
            public UInt64 Arguments { get { return ullArguments; } set { ullArguments = value; } }
            /// <summary></summary>
            public uint ExtraArgs { get { return cbExtraArgs; } set { cbExtraArgs = value; } }

            /// <summary></summary>
            public WMGestureEventArgs()
            {
            }
        }

        #region DEFINES
        private const int WM_TOUCH = 0x0240;
        private const int WM_GESTURE = 0x0119;

        private const int TOUCHEVENTF_MOVE = 0x0001;
        private const int TOUCHEVENTF_DOWN = 0x0002;
        private const int TOUCHEVENTF_UP = 0x0004;
        private const int TOUCHEVENTF_INRANGE = 0x0008;
        private const int TOUCHEVENTF_PRIMARY = 0x0010;
        private const int TOUCHEVENTF_NOCOALESCE = 0x0020;
        private const int TOUCHEVENTF_PEN = 0x0040;

        private const int TOUCHINPUTMASKF_TIMEFROMSYSTEM = 0x0001; // the dwTime field contains a system generated value
        private const int TOUCHINPUTMASKF_EXTRAINFO = 0x0002; // the dwExtraInfo field is valid
        private const int TOUCHINPUTMASKF_CONTACTAREA = 0x0004; // the cxContact and cyContact fields are valid

        private const int GF_BEGIN = 0x00000001;
        private const int GF_INERTIA = 0x00000002;
        private const int GF_END = 0x00000004;

        private const int GID_BEGIN = 1;
        private const int GID_END = 2;
        private const int GID_ZOOM = 3;
        private const int GID_PAN = 4;
        private const int GID_ROTATE = 5;
        private const int GID_TWOFINGERTAP = 6;
        private const int GID_ROLLOVER = 7;
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        private struct TOUCHINPUT
        {
            public int x;
            public int y;
            public System.IntPtr hSource;
            public int dwID;
            public int dwFlags;
            public int dwMask;
            public int dwTime;
            public System.IntPtr dwExtraInfo;
            public int cxContact;
            public int cyContact;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct GESTUREINFO
        {
            public int cbSize;
            public int dwFlags;
            public int dwID;
            public IntPtr hwndTarget;
            [MarshalAs(UnmanagedType.Struct)]
            public POINTS ptsLocation;
            public int dwInstanceID;
            public int dwSequenceID;
            public UInt64 ullArguments;
            public uint cbExtraArgs;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINTS
        {
            public short x;
            public short y;
        }

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RegisterTouchWindow(System.IntPtr hWnd, uint ulFlags);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnregisterTouchWindow(System.IntPtr hWnd);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetTouchInputInfo(System.IntPtr hTouchInput, int cInputs, [In, Out] TOUCHINPUT[] pInputs, int cbSize);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern void CloseTouchInputHandle(System.IntPtr lParam);


        // ジェスチャ詳細取得・開放
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetGestureInfo(IntPtr hGestureInfo, ref GESTUREINFO pGestureInfo);

        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseGestureInfoHandle(IntPtr hGestureInfo);        

        private int touchInputSize;

        /// <summary>
        /// 
        /// </summary>
        public class TouchPoints
        {
            /// <summary>
            /// 
            /// </summary>
            public class TouchInfo
            {
                /// <summary></summary>
                public DateTime datetime = DateTime.Now;
                /// <summary></summary>
                public Point point = new Point();

                /// <summary></summary>
                public override string ToString()
                {
                    return datetime.ToString("yyyy/MM/dd HH:mm:ss.fff") + " - " + point.ToString();
                }
            }

            /// <summary></summary>
            public List<TouchInfo> listPoint = new List<TouchInfo>();
        }

        /// <summary></summary>
        public Dictionary<int, TouchPoints> touchPoints = new Dictionary<int, TouchPoints>();

        /// <summary></summary>
        [SecurityPermission(SecurityAction.Demand)]
        public BL_TouchControl()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="gesture"></param>
        public void StartControl(Control c, bool gesture)
        {
            control = c;
            touchInputSize = Marshal.SizeOf(new TOUCHINPUT());

            try
            {
                if (gesture)
                {
                    BL_WndProcHooker.HookWndProc(control, new BL_WndProcHooker.WndProcCallback(this.OnGesture), WM_GESTURE);
                }

                //else
                {
                    // Registering the window for multi-touch, using the default settings.
                    // p/invoking into user32.dll
                    if (!RegisterTouchWindow(control.Handle, 0))
                    {
                        Debug.Print("ERROR: Could not register window for multi-touch");
                    }

                    BL_WndProcHooker.HookWndProc(control, new BL_WndProcHooker.WndProcCallback(this.OnTouch), WM_TOUCH);
                }
            }
            catch (Exception exception)
            {
                Debug.Print("ERROR: RegisterTouchWindow API not available");
                Debug.Print(exception.ToString());
                MessageBox.Show("RegisterTouchWindow API not available", "MTScratchpadWMTouch ERROR",
                    MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopControl()
        {
            try
            {
                UnregisterTouchWindow(control.Handle);
            }
            catch { }
        }

        int OnTouch(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            Message m = Message.Create(hwnd, msg, wParam, lParam);
            handled = DecodeTouch(ref m);
            return 0;
        }

        int OnGesture(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            Message m = Message.Create(hwnd, msg, wParam, lParam);
            handled = DecodeGesture(ref m);
            return 0;
        }
        
        private static int LoWord(int number)
        {
            return (number & 0xffff);
        }

        private bool DecodeTouch(ref Message m)
        {
            int inputCount = LoWord(m.WParam.ToInt32());

            TOUCHINPUT[] inputs;
            inputs = new TOUCHINPUT[inputCount];

            if (!GetTouchInputInfo(m.LParam, inputCount, inputs, touchInputSize))
            {
                return false;
            }

            for (int i = 0; i < inputCount; i++)
            {
                TOUCHINPUT ti = inputs[i];

                if ((ti.dwFlags & TOUCHEVENTF_DOWN) != 0)
                {
                    if (!touchPoints.ContainsKey(ti.dwID))
                    {
                        touchPoints[ti.dwID] = new TouchPoints();
                    }
                    else
                    {
                        touchPoints[ti.dwID].listPoint.Clear();
                    }

                    TouchPoints.TouchInfo info = new TouchPoints.TouchInfo();
                    info.point = control.PointToClient(new Point(ti.x / 100, ti.y / 100));
                    touchPoints[ti.dwID].listPoint.Add(info);
                }
                else if ((ti.dwFlags & TOUCHEVENTF_UP) != 0)
                {
                    touchPoints.Remove(ti.dwID);
                }
                else if ((ti.dwFlags & TOUCHEVENTF_MOVE) != 0)
                {
                    if (touchPoints.ContainsKey(ti.dwID))
                    {
                        TouchPoints.TouchInfo info = new TouchPoints.TouchInfo();
                        info.point = control.PointToClient(new Point(ti.x / 100, ti.y / 100));

                        if (0 < touchPoints[ti.dwID].listPoint.Count)
                        {
                            if (touchPoints[ti.dwID].listPoint[touchPoints[ti.dwID].listPoint.Count - 1].point != info.point)
                            {
                                touchPoints[ti.dwID].listPoint.Add(info);
                            }
                        }
                    }
                }
            }

            bool handled = false;
            for (int i = 0; i < inputCount; i++)
            {
                TOUCHINPUT ti = inputs[i];

                TouchEventHandler handler = null;
                if ((ti.dwFlags & TOUCHEVENTF_DOWN) != 0)
                {
                    System.Diagnostics.Debug.WriteLine("TOUCH_DOWN");
                    handler = Touchdown;
                }
                else if ((ti.dwFlags & TOUCHEVENTF_UP) != 0)
                {
                    System.Diagnostics.Debug.WriteLine("TOUCH_UP");
                    handler = Touchup;
                }
                else if ((ti.dwFlags & TOUCHEVENTF_MOVE) != 0)
                {
                    handler = TouchMove;
                }

                if (handler != null)
                {
                    WMTouchEventArgs te = new WMTouchEventArgs(); // Touch event arguments

                    te.Contact = new Point(ti.cxContact / 100, ti.cyContact / 100);
                    te.Id = ti.dwID;
                    {
                        te.Location = control.PointToClient(new Point(ti.x / 100, ti.y / 100));
                    }
                    te.Time = ti.dwTime;
                    te.Mask = ti.dwMask;
                    te.Flags = ti.dwFlags;

                    te.touchPoints = touchPoints;

                    handler(this, te);

                    handled = te.Handled;

					System.Diagnostics.Debug.WriteLine("handled = " + handled.ToString());
                }
            }

            CloseTouchInputHandle(m.LParam);

            return handled;
        }

        private bool DecodeGesture(ref Message m)
        {
            bool handled = false;

            GESTUREINFO gi = new GESTUREINFO();
            gi.cbSize = Marshal.SizeOf(typeof(GESTUREINFO));

            if (GetGestureInfo(m.LParam, ref gi))
            {
                GestureEventHandler handler = null;     // Touch event handler

                switch (gi.dwID)
                {
                    case GID_BEGIN:
                        System.Diagnostics.Debug.WriteLine("GID_BEGIN");
                        if (GestureBegin != null)
                        {
                            handler = GestureBegin;
                        }
                        break;

                    case GID_END:
                        System.Diagnostics.Debug.WriteLine("GID_END");
                        if (GestureEnd != null)
                        {
                            handler = GestureEnd;
                        }
                        break;

                    case GID_ZOOM:
                        System.Diagnostics.Debug.WriteLine("GID_ZOOM");
                        if (GestureZoom != null)
                        {
                            handler = GestureZoom;
                        }
                        break;

                    case GID_PAN:
                        System.Diagnostics.Debug.WriteLine("GID_PAN");
                        if (GesturePan != null)
                        {
                            handler = GesturePan;
                        }
                        break;

                    case GID_ROTATE:
                        System.Diagnostics.Debug.WriteLine("GID_ROTATE");
                        if (GestureRotate != null)
                        {
                            handler = GestureRotate;
                        }
                        break;

                    case GID_TWOFINGERTAP:
                        System.Diagnostics.Debug.WriteLine("GID_TWOFINGERTAP");
                        if (GestureTwoFingerTap != null)
                        {
                            handler = GestureTwoFingerTap;
                        }
                        break;

                    case GID_ROLLOVER:
                        System.Diagnostics.Debug.WriteLine("GID_ROLLOVER");
                        if (GestureRollover != null)
                        {
                            handler = GestureRollover;
                        }
                        break;

                    default:
                        break;
                }

                if (handler != null)
                {
                    WMGestureEventArgs ge = new WMGestureEventArgs();
                    ge.Flags = gi.dwFlags;
                    ge.Id = gi.dwID;
                    ge.LocationX = gi.ptsLocation.x;
                    ge.LocationY = gi.ptsLocation.y;
                    ge.InstanceID = gi.dwInstanceID;
                    ge.SequenceID = gi.dwSequenceID;
                    ge.TargetHandle = gi.hwndTarget;
                    ge.Arguments = gi.ullArguments;
                    ge.ExtraArgs = gi.cbExtraArgs;

                    handler(this, ge);

                    handled = true;
                }

                CloseGestureInfoHandle(m.LParam);
            }

            return handled;
        }
    }
}
