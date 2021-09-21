using System;
using System.Runtime.InteropServices;

namespace BelicsClass.Common
{
    /// <summary>
    /// Win32APIを使用してローカル時刻を操作します。
    /// 管理者権限が無いと設定できません。また、UACが有効な場合設定できません。
    /// </summary>
    public class BL_LocalTime
    {
        /// <summary>
        /// Win32API用のシステム時刻構造体です。
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public class BL_SystemTime
        {
            /// <summary>年</summary>
            public ushort wYear;
            /// <summary>月</summary>
            public ushort wMonth;
            /// <summary>曜日</summary>
            public ushort wDayOfWeek;
            /// <summary>日</summary>
            public ushort wDay;
            /// <summary>時</summary>
            public ushort wHour;
            /// <summary>分</summary>
            public ushort wMinute;
            /// <summary>秒</summary>
            public ushort wSecond;
            /// <summary>ミリ秒</summary>
            public ushort wMilliseconds;
        }

        /// <summary>
        /// ローカル時刻を設定します。
        /// </summary>
        /// <param name="st">設定する時刻を指定します。</param>
        /// <returns>設定できた場合はtrue、失敗した場合はfalseを返します。</returns>
        [DllImport("kernel32.dll", EntryPoint = "SetLocalTime")]
        public static extern Boolean SetLocalTime(BL_SystemTime st);

        /// <summary>
        /// ローカル時刻を取得します。
        /// </summary>
        /// <param name="st">取得した時刻の代入先を指定します。</param>
        [DllImport("kernel32.dll", EntryPoint = "GetLocalTime")]
        public static extern void GetLocalTime(out BL_SystemTime st);


        /// <summary>
        /// ローカル日時を取得/設定します。
        /// </summary>
        public static DateTime Now
        {
            get
            {
                return DateTime.Now;
                /*
                //ローカル日時取り出し
                MySystemTime wkTime = new MySystemTime();
                GetLocalTime( wkTime );

                //DateTime構造体に変換して日時を返す
                return new DateTime(
                    wkTime.wYear, wkTime.wMonth, wkTime.wDay,
                    wkTime.wHour, wkTime.wMinute, wkTime.wSecond,wkTime.wMilliseconds );
                */
            }
            set
            {
                //日時をAPI型に変換(DayOfWeekは日曜日を示す 0 から土曜日を示す 6 )
                BL_SystemTime wkTime = new BL_SystemTime();
                wkTime.wYear = (ushort)value.Year;
                wkTime.wMonth = (ushort)value.Month;
                wkTime.wDayOfWeek = (ushort)value.DayOfWeek;
                wkTime.wDay = (ushort)value.Day;
                wkTime.wHour = (ushort)value.Hour;
                wkTime.wMinute = (ushort)value.Minute;
                wkTime.wSecond = (ushort)value.Second;
                wkTime.wMilliseconds = (ushort)value.Millisecond;

                //ローカル日時を設定		  
                bool ret = SetLocalTime(wkTime);
            }
        }

        /// <summary>
        /// デフォルトコンストラクタです。
        /// 本クラスはインスタンスを持つことができません。
        /// </summary>
        private BL_LocalTime()
        {
        }
    }
}
