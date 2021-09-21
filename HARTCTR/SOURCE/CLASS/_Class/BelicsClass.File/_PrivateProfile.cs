using System.Runtime.InteropServices;

namespace BelicsClass.File
{
    /// <summary>
    /// 設定ファイルクラス
    /// </summary>
    public sealed class BL_PrivateProfile
    {
        private BL_PrivateProfile()
        {
        }

        #region Win32API
        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        static extern uint GetPrivateProfileString(string Section, string Key, string Default, System.Text.StringBuilder lpReturnedString, uint Size, string FileName);

        [DllImport("kernel32", EntryPoint = "WritePrivateProfileString")]
        static extern uint WritePrivateProfileString(string Section, string Key, string EntryString, string FileName);
        #endregion

        /// <summary>
        /// 指定したセクションから文字列を取得します。
        /// </summary>
        /// <param name="Section">セクションの名前を示す文字列を指定します。</param>
        /// <param name="Key">キーの名前を示す文字列を指定します。</param>
        /// <param name="Default">キーがファイル内に見つからないときのデフォルト値を示す文字列を指定します。</param>
        /// <param name="FileName">ファイルの名前を示す文字列を指定します。</param>
        /// <returns></returns>
        public static string Get(string Section, string Key, string Default, string FileName)
        {
            System.Text.StringBuilder buff = new System.Text.StringBuilder(1024);

            GetPrivateProfileString(Section, Key, Default, buff, (uint)buff.Capacity, FileName);

            return buff.ToString();
        }

        /// <summary>
        /// 指定したセクションに文字列を設定します。
        /// </summary>
        /// <param name="Section">セクションの名前を示す文字列を指定します。</param>
        /// <param name="Key">キーの名前を示す文字列を指定します。</param>
        /// <param name="EntryString">値を示す文字列を指定します。</param>
        /// <param name="FileName">ファイルの名前を示す文字列を指定します。</param>
        /// <returns></returns>
        public static uint Set(string Section, string Key, string EntryString, string FileName)
        {
            return WritePrivateProfileString(Section, Key, EntryString, FileName);
        }
    }
}
