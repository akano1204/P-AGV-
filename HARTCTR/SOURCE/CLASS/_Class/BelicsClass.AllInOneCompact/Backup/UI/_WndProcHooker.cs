using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Runtime.InteropServices;

using BelicsClass.Common;

namespace BelicsClass.UI
{
    /// <summary>
    /// ウィンドウメッセージのフックを自動処理するクラスです。
    /// </summary>
    public class BL_WndProcHooker
    {
        /// <summary>
        /// A callback to a Win32 window procedure (wndproc):
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate int WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="nIndex"></param>
        /// <param name="dwNewLong"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private extern static IntPtr SetWindowLong(IntPtr hwnd, int nIndex, IntPtr dwNewLong);
        /// <summary></summary>
        private const int GWL_WNDPROC = -4;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpPrevWndFunc"></param>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        private extern static int CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam);

        /// <summary>
        /// コールバック関数のデリゲート
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        public delegate int WndProcCallback(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled);

        private static Dictionary<IntPtr, HookedProcInformation> hwndDict = new Dictionary<IntPtr, HookedProcInformation>();
        private static Dictionary<Control, HookedProcInformation> ctlDict = new Dictionary<Control, HookedProcInformation>();

        /// <summary>
        /// コントロールに特定のメッセージに対応するコールバック関数（フック）を登録します
        /// </summary>
        /// <param name="ctl"></param>
        /// <param name="callback"></param>
        /// <param name="msg"></param>
        public static void HookWndProc(Control ctl, WndProcCallback callback, int msg)
        {
            HookedProcInformation hpi = null;
            if (ctlDict.ContainsKey(ctl))
            {
                hpi = ctlDict[ctl];
            }
            else if (hwndDict.ContainsKey(ctl.Handle))
            {
                hpi = hwndDict[ctl.Handle];
            }

            if (hpi == null)
            {
                hpi = new HookedProcInformation(ctl, new WndProc(BL_WndProcHooker.WindowProc));
                ctl.HandleCreated += new EventHandler(ctl_HandleCreated);
                ctl.HandleDestroyed += new EventHandler(ctl_HandleDestroyed);
                ctl.Disposed += new EventHandler(ctl_Disposed);

                if (ctl.Handle != IntPtr.Zero) hpi.SetHook();
            }

            if (ctl.Handle == IntPtr.Zero)
            {
                ctlDict[ctl] = hpi;
            }
            else
            {
                hwndDict[ctl.Handle] = hpi;
            }

            hpi.messageMap[msg] = callback;
        }

        static void ctl_Disposed(object sender, EventArgs e)
        {
            Control ctl = sender as Control;
            if (ctlDict.ContainsKey(ctl))
            {
                ctlDict.Remove(ctl);
            }
            else
            {
                System.Diagnostics.Debug.Assert(false);
            }
        }

        static void ctl_HandleDestroyed(object sender, EventArgs e)
        {
            Control ctl = sender as Control;
            if (hwndDict.ContainsKey(ctl.Handle))
            {
                HookedProcInformation hpi = hwndDict[ctl.Handle];
                UnhookWndProc(ctl, false);
            }
            else
            {
                System.Diagnostics.Debug.Assert(false);
            }
        }

        static void ctl_HandleCreated(object sender, EventArgs e)
        {
            Control ctl = sender as Control;
            if (ctlDict.ContainsKey(ctl))
            {
                HookedProcInformation hpi = ctlDict[ctl];
                hwndDict[ctl.Handle] = hpi;
                ctlDict.Remove(ctl);
                hpi.SetHook();
            }
            else
            {
                System.Diagnostics.Debug.Assert(false);
            }
        }

        private static int WindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
        {
            //if (hwndDict.ContainsKey(hwnd))
            {
                HookedProcInformation hpi = hwndDict[hwnd];
                if (hpi.messageMap.ContainsKey(msg))
                {
                    WndProcCallback callback = hpi.messageMap[msg];
                    bool handled = false;
                    int retval = callback(hwnd, msg, wParam, lParam, ref handled);
                    if (handled)
                    {
                        return retval;
                    }
                }

                return hpi.CallOldWindowProc(hwnd, msg, wParam, lParam);
            }

            //System.Diagnostics.Debug.Assert(false, "不明なウィンドウハンドルから呼び出されました。");
            //return Win32.DefWindowProc(hwnd, msg, wParam, lParam);
        }

        /// <summary>
        /// コントロールのコールバックを解除します。
        /// </summary>
        /// <param name="ctl"></param>
        /// <param name="msg"></param>
        public static void UnhookWndProc(Control ctl, int msg)
        {
            HookedProcInformation hpi = null;
            if (ctlDict.ContainsKey(ctl))
            {
                hpi = ctlDict[ctl];
            }
            else if (hwndDict.ContainsKey(ctl.Handle))
            {
                hpi = hwndDict[ctl.Handle];
            }

            if (hpi == null) throw new ArgumentException(string.Format("フックされていないコントロールです[{0}]。", ctl));

            if (hpi.messageMap.ContainsKey(msg))
            {
                hpi.messageMap.Remove(msg);
            }
            else
            {
                throw new ArgumentException(string.Format("メッセージが不明です [{0}]", msg));
            }
        }

        /// <summary>
        /// コントロールに登録されているコールバックを解除します。
        /// </summary>
        /// <param name="ctl"></param>
        /// <param name="disposing"></param>
        public static void UnhookWndProc(Control ctl, bool disposing)
        {
            HookedProcInformation hpi = null;
            if (ctlDict.ContainsKey(ctl))
            {
                hpi = ctlDict[ctl];
            }
            else if (hwndDict.ContainsKey(ctl.Handle))
            {
                hpi = hwndDict[ctl.Handle];
            }

            if (hpi == null)
            {
                throw new ArgumentException(string.Format("フックされていないコントロールです[{0}]。", ctl));
            }

            if (ctlDict.ContainsKey(ctl) && disposing)
            {
                ctlDict.Remove(ctl);
            }

            if (hwndDict.ContainsKey(ctl.Handle))
            {
                hpi.Unhook();
                hwndDict.Remove(ctl.Handle);
                if (!disposing)
                {
                    ctlDict[ctl] = hpi;
                }
            }
        }

        /// <summary>
        /// フック情報クラス
        /// </summary>
        public class HookedProcInformation
        {
            /// <summary>
            /// メッセージに対応するコールバック一覧
            /// </summary>
            public Dictionary<int, WndProcCallback> messageMap;
            private IntPtr oldWndProc;
            private WndProc newWndProc;
            private Control control;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="ctl"></param>
            /// <param name="wndproc"></param>
            public HookedProcInformation(Control ctl, WndProc wndproc)
            {
                control = ctl;
                newWndProc = wndproc;
                messageMap = new Dictionary<int, WndProcCallback>();
            }

            /// <summary>
            /// コールバックを登録します。
            /// </summary>
            public void SetHook()
            {
                IntPtr hwnd = control.Handle;
                if (hwnd == IntPtr.Zero)
                {
                    throw new InvalidOperationException("ハンドルが生成されていないコントロールです。");
                }

                oldWndProc = SetWindowLong(hwnd, GWL_WNDPROC, Marshal.GetFunctionPointerForDelegate(newWndProc));
            }

            /// <summary>
            /// コールバックを解除します。
            /// </summary>
            public void Unhook()
            {
                IntPtr hwnd = control.Handle;
                if (hwnd == IntPtr.Zero)
                {
                    throw new InvalidOperationException("ハンドルが生成されていないコントロールです。");
                }

                SetWindowLong(hwnd, GWL_WNDPROC, oldWndProc);
            }

            /// <summary>
            /// 旧コールバックを呼び出して、コールバックリレーを続行します。
            /// </summary>
            /// <param name="hwnd"></param>
            /// <param name="msg"></param>
            /// <param name="wParam"></param>
            /// <param name="lParam"></param>
            /// <returns></returns>
            public int CallOldWindowProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam)
            {
                return CallWindowProc(oldWndProc, hwnd, msg, wParam, lParam);
            }
        }
    }
}
