using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Drawing;

namespace BelicsClass.Common
{
    /// <summary>
    /// Win32APIを操作クラス
    /// </summary>
    public class BL_Win32API
    {
        private BL_Win32API()
        {
        }

        #region Win32API

        /// <summary>
        /// 
        /// </summary>
        /// <param name="szSound"></param>
        /// <param name="hMod"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport("winmm", EntryPoint = "PlaySound", SetLastError = true)]
        public static extern bool API_PlaySound(string szSound, IntPtr hMod, uint flags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="szSound"></param>
        /// <param name="hMod"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport("winmm", EntryPoint = "PlaySound", SetLastError = true)]
        public static extern bool API_PlaySound(byte[] szSound, IntPtr hMod, uint flags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="szSound"></param>
        /// <param name="hMod"></param>
        /// <param name="flags"></param>
        /// <returns></returns>
        [DllImport("winmm", EntryPoint = "PlaySound", SetLastError = true)]
        public static extern bool API_PlaySound(IntPtr szSound, IntPtr hMod, uint flags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uType"></param>
        /// <returns></returns>
        [DllImport("user32", EntryPoint = "MessageBeep", SetLastError = true)]
        public static extern bool API_MessageBeep(uint uType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frequency"></param>
        /// <param name="duration"></param>
        /// <returns></returns>
        [DllImport("kernel32", EntryPoint = "Beep", SetLastError = true)]
        public static extern bool API_Beep(uint frequency, uint duration);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpNetResource"></param>
        /// <param name="lpPassword"></param>
        /// <param name="lpUsername"></param>
        /// <param name="dwFlags"></param>
        /// <returns></returns>
        [DllImport("mpr", EntryPoint = "WNetAddConnection2", SetLastError = true)]
        public static extern uint API_WNetAddConnection2(ref NETRESOURCE lpNetResource, string lpPassword, string lpUsername, int dwFlags);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lpName"></param>
        /// <param name="dwFlags"></param>
        /// <param name="fForce"></param>
        /// <returns></returns>
        [DllImport("mpr", EntryPoint = "WNetCancelConnection2", SetLastError = true)]
        public static extern uint API_WNetCancelConnection2(string lpName, int dwFlags, bool fForce);

        /// <summary></summary>
        public const uint SND_ASYNC = 0x00000001;
        /// <summary></summary>
        public const uint SND_FILENAME = 0x00020000;
        /// <summary></summary>
        public const uint SND_MEMORY = 0x00000004;
        /// <summary></summary>
        public const uint SND_LOOP = 0x00000008;

        /// <summary></summary>
        public const uint RESOURCE_CONNECTED = 0x00000001;
        /// <summary></summary>
        public const uint RESOURCE_GLOBALNET = 0x00000002;
        /// <summary></summary>
        public const uint RESOURCE_REMEMBERED = 0x00000003;

        /// <summary></summary>
        public const uint RESOURCETYPE_ANY = 0x00000000;
        /// <summary></summary>
        public const uint RESOURCETYPE_DISK = 0x00000001;
        /// <summary></summary>
        public const uint RESOURCETYPE_PRINT = 0x00000002;

        /// <summary></summary>
        public const uint RESOURCEDISPLAYTYPE_GENERIC = 0x00000000;
        /// <summary></summary>
        public const uint RESOURCEDISPLAYTYPE_DOMAIN = 0x00000001;
        /// <summary></summary>
        public const uint RESOURCEDISPLAYTYPE_SERVER = 0x00000002;
        /// <summary></summary>
        public const uint RESOURCEDISPLAYTYPE_SHARE = 0x00000003;

        /// <summary></summary>
        public const uint RESOURCEUSAGE_CONNECTABLE = 0x00000001;
        /// <summary></summary>
        public const uint RESOURCEUSAGE_CONTAINER = 0x00000002;

        /// <summary></summary>
        public const uint CONNECT_UPDATE_PROFILE = 0x00000001;

        /// <summary>
        /// 
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct NETRESOURCE
        {
            /// <summary></summary>
            public uint dwScope;
            /// <summary></summary>
            public uint dwType;
            /// <summary></summary>
            public uint dwDisplayType;
            /// <summary></summary>
            public uint dwUsage;
            /// <summary></summary>
            public string lpLocalName;
            /// <summary></summary>
            public string lpRemoteName;
            /// <summary></summary>
            public string lpComment;
            /// <summary></summary>
            public string lpProvider;
        }

        #endregion

        /// <summary>
        /// 指定したミリ秒数の間現在のスレッドをブロックします。
        /// </summary>
        /// <param name="millisecondsTimeout">スレッドがブロックされるミリ秒数を指定します。</param>
        public static void Sleep(int millisecondsTimeout)
        {
            Thread.Sleep(millisecondsTimeout);
        }

        /// <summary>
        /// システム起動後のミリ秒単位の経過時間を取得します。
        /// </summary>
        public static int TickCount
        {
            get
            {
                return System.Environment.TickCount;
            }
        }

        private static long Offset = 0;
        private static int LastTick = 0;

        /// <summary>
        /// システム起動後のミリ秒単位の経過時間を取得します。
        /// </summary>
        public static long TickCount64
        {
            get
            {
                int tick;

                tick = System.Environment.TickCount & int.MaxValue;

                if (LastTick > tick)
                {
                    Offset++;
                }

                LastTick = tick;

                return tick + int.MaxValue * Offset;
            }
        }

    }
}
