using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BelicsClass.UI
{
    /// <summary>
    /// キーフック制御クラス
    /// キー操作をフックした際に、イベント「KeyHookEvent」をコールします。
    /// </summary>
    public class BL_KeyHook : IDisposable
    {
        #region Win32API

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int hookType, HookHandler hookDelegate, IntPtr module, uint threadId);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr hook);

        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hook, int code, IntPtr message, IntPtr state);

        /// <summary>
        /// キーボード情報構造体(WIN32)
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class KBDLLHOOKSTRUCT
        {
            /// <summary></summary>
            public uint vkCode = 0;
            /// <summary></summary>
            public uint scanCode = 0;
            /// <summary></summary>
            public KBDLLHOOKSTRUCTFlags flags = 0;
            /// <summary></summary>
            public uint time = 0;
            /// <summary></summary>
            public UIntPtr dwExtraInfo = UIntPtr.Zero;
        }

        /// <summary>
        /// Flags
        /// </summary>
        [Flags]
        public enum KBDLLHOOKSTRUCTFlags : uint
        {
            /// <summary></summary>
            LLKHF_EXTENDED = 0x01,
            /// <summary></summary>
            LLKHF_INJECTED = 0x10,
            /// <summary></summary>
            LLKHF_ALTDOWN = 0x20,
            /// <summary></summary>
            LLKHF_UP = 0x80,
        }

        #endregion

        /// <summary>
        /// キーボード押下情報クラス
        /// </summary>
        public class HookInfo
        {
            /// <summary>キーコード</summary>
            public Keys vkCode = (Keys)0;
            /// <summary>スキャンコード</summary>
            public uint scanCode = 0;
            /// <summary>フラグ</summary>
            public KBDLLHOOKSTRUCTFlags flags = 0;
            /// <summary>時間</summary>
            public uint time = 0;
            /// <summary>拡張情報</summary>
            public UIntPtr dwExtraInfo = UIntPtr.Zero;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public HookInfo() { }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="kb">キーボード押下情報クラス</param>
            public HookInfo(KBDLLHOOKSTRUCT kb)
            {
                if (kb != null)
                {
                    vkCode = (Keys)kb.vkCode;
                    scanCode = kb.scanCode;
                    flags = kb.flags;
                    time = kb.time;
                    dwExtraInfo = kb.dwExtraInfo;
                }
            }
        }

        /// <summary>
        /// キーフックイベントハンドラー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="hookinfo"></param>
        public delegate void KeyHookEventHandler(object sender, HookInfo hookinfo);

        /// <summary>
        /// キーフックイベント
        /// </summary>
        public event KeyHookEventHandler KeyHookEvent;

        private delegate int HookHandler(int code, IntPtr message, IntPtr state);
        private HookHandler hookDelegate;

        private IntPtr hook;

        private Form owner = null;

        private int OnHook(int nCode, IntPtr wParam, IntPtr lParam)
        {
            KBDLLHOOKSTRUCT kb = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lParam, typeof(KBDLLHOOKSTRUCT));

            if (owner == null)
            {
                if (KeyHookEvent != null) KeyHookEvent(this, new HookInfo(kb));
            }
            else
            {
                //オーナーフォームが指定されている場合、遅延インボークします。
                if (owner.IsHandleCreated)
                {
                    MethodInvoker process = (MethodInvoker)delegate()
                    {
                        if (KeyHookEvent != null) KeyHookEvent(this, new HookInfo(kb));
                    };
                    
                    if (owner.InvokeRequired) owner.BeginInvoke(process);
                    else process.Invoke();
                }
                else
                {
                    if (KeyHookEvent != null) KeyHookEvent(this, new HookInfo(kb));
                }
            }

            return CallNextHookEx(hook, nCode, wParam, lParam);
        }

        /// <summary>
        /// コンストラクタ
        /// キーフック開始
        /// フック処理に失敗すると例外を吐きます。
        /// </summary>
        public BL_KeyHook()
        {
            const int KEYBOARD_LL = 13;
            hookDelegate = new HookHandler(OnHook);
            IntPtr hMod = Marshal.GetHINSTANCE(System.Reflection.Assembly.GetExecutingAssembly().GetModules()[0]);
            hook = SetWindowsHookEx(KEYBOARD_LL, hookDelegate, hMod, 0);
            if (hook == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// キーフック開始
        /// フック処理に失敗すると例外を吐きます。
        /// </summary>
        public BL_KeyHook(Form owner)
            : this()
        {
            this.owner = owner;
        }

        /// <summary>
        /// 破棄
        /// キーフック解除
        /// </summary>
        public void Dispose()
        {
            if (hook != IntPtr.Zero)
            {
                UnhookWindowsHookEx(hook);
            }
        }
    }
}
