using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace BelicsClass.File
{
    /// <summary>
    /// IniFileを操作します。
    /// </summary>
    public class BL_IniFile
    {
        /// <summary>
        /// コンストラクタです。<br/>
        /// </summary>
        /// <param name="iniFileName">
        /// iniファイル名を指定します。<br/>
        /// 相対パス、絶対パスのいずれでもOKです。
        /// </param>
        public BL_IniFile(string iniFileName)
        {
            //指定されたファイルの情報を取得
            fileInfo = new System.IO.FileInfo(iniFileName);
        }

        /// <summary>
        /// iniファイルのファイル名称を取得できます。
        /// </summary>
        public string FullName
        {
            get
            {
                return fileInfo.FullName;
            }
        }

        /// <summary>
        /// iniファイルが存在することを確認できます。
        /// </summary>
        public bool Exists
        {
            get
            {
                fileInfo.Refresh();
                return fileInfo.Exists;
            }
        }

        /// <summary>
        /// 指定されたセクション、キーの項目が存在するか調査します。
        /// </summary>
        /// <param name="section">存在を確認するセクションを指定します。</param>
        /// <param name="key">存在を確認するキーを指定します。</param>
        /// <returns>
        /// 指定されたセクション、キーが存在する場合、trueを返します。
        /// 指定されたセクション、キーが存在しない場合、falseを返します。
        /// </returns>
        public bool ExistsKey(string section, string key)
        {
            //値が格納されていればキーは存在する
            if (Read(section, key, "") != "") return true;

            //値がなくてもキーが存在する場合
            if (Read(section, key, "-") != "-") return true;

            //キー無し
            return false;
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、文字列で取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>文字列を返します。</returns>
        public string Read(string section, string key, string defaultValue)
        {
            //読み込みエリア
            System.Text.StringBuilder strEntryString = null;

            //全て読み込めるまでトライする
            for (int tryCount = 1; ; ++tryCount)
            {
                //読み込み用のバッファを作成
                strEntryString = new System.Text.StringBuilder(readValueSizeCapacity * tryCount);

                //読み込み
                uint len = GetPrivateProfileString(section, key, defaultValue, strEntryString, (uint)(strEntryString.Capacity), fileInfo.FullName);

                //全て読み込めた
                if (len + 1 < strEntryString.Capacity) break;
            }

            return strEntryString.ToString(); ;
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、sbyteで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>sbyteを返します。</returns>
        public sbyte Read(string section, string key, sbyte defaultValue)
        {
            return sbyte.Parse(Read(section, key, defaultValue.ToString()));
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、shortで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>shortを返します。</returns>
        public short Read(string section, string key, short defaultValue)
        {
            return short.Parse(Read(section, key, defaultValue.ToString()));
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、intで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>intを返します。</returns>
        public int Read(string section, string key, int defaultValue)
        {
            //return int.Parse(Read(section, key, defaultValue.ToString()));
            string srtRet = Read(section, key, defaultValue.ToString());
            if (srtRet == "") srtRet = defaultValue.ToString();
            return int.Parse(srtRet);
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、longで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>longを返します。</returns>
        public long Read(string section, string key, long defaultValue)
        {
            return long.Parse(Read(section, key, defaultValue.ToString()));
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、byteで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>byteを返します。</returns>
        public byte Read(string section, string key, byte defaultValue)
        {
            return byte.Parse(Read(section, key, defaultValue.ToString()));
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、ushortで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>ushortを返します。</returns>
        public ushort Read(string section, string key, ushort defaultValue)
        {
            return ushort.Parse(Read(section, key, defaultValue.ToString()));
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、uintで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>uintを返します。</returns>
        public uint Read(string section, string key, uint defaultValue)
        {
            return uint.Parse(Read(section, key, defaultValue.ToString()));
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、ulongで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>ulongを返します。</returns>
        public ulong Read(string section, string key, ulong defaultValue)
        {
            return ulong.Parse(Read(section, key, defaultValue.ToString()));
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、floatで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>文字列を返します。</returns>
        public float Read(string section, string key, float defaultValue)
        {
            return float.Parse(Read(section, key, defaultValue.ToString()));
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、doubleで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>doubleを返します。</returns>
        public double Read(string section, string key, double defaultValue)
        {
            return double.Parse(Read(section, key, defaultValue.ToString()));
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、decimalで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>decimalを返します。</returns>
        public decimal Read(string section, string key, decimal defaultValue)
        {
            return decimal.Parse(Read(section, key, defaultValue.ToString()));
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、charで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>charを返します。</returns>
        public char Read(string section, string key, char defaultValue)
        {
            return char.Parse(Read(section, key, defaultValue.ToString()));
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、boolで取得します。
        ///	"0"=true/"1"=false もしく "true"/"false"
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>boolを返します。</returns>
        public bool Read(string section, string key, bool defaultValue)
        {
            string str = Read(section, key, defaultValue.ToString());

            int n = 0;
            if (int.TryParse(str, out n)) return n != 0 ? true : false;

            return bool.Parse(str);
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、IPAddressで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>IPAddressを返します。</returns>
        public System.Net.IPAddress Read(string section, string key, System.Net.IPAddress defaultValue)
        {
            return System.Net.IPAddress.Parse(Read(section, key, defaultValue.ToString()));
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、DateTimeで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>DateTimeを返します。</returns>
        public DateTime Read(string section, string key, DateTime defaultValue)
        {
            try
            {
                string datetime = Read(section, key, defaultValue.ToString());
                return DateTime.Parse(datetime);
            }
            catch { }

            return new DateTime();
        }

        /// <summary>
        ///	指定されたセクション、キーの値を、Colorで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>DateTimeを返します。</returns>
        public Color Read(string section, string key, Color defaultValue)
        {
            string col = Read(section, key, defaultValue.Name);
			return GetColor(col, defaultValue);
        }



        /// <summary>
        ///	指定されたセクション、キーの値を、文字列で取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>文字列を返します。</returns>
        public string Get(string section, string key, string defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、sbyteで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>sbyteを返します。</returns>
        public sbyte Get(string section, string key, sbyte defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、shortで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>shortを返します。</returns>
        public short Get(string section, string key, short defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、intで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>intを返します。</returns>
        public int Get(string section, string key, int defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、longで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>longを返します。</returns>
        public long Get(string section, string key, long defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、byteで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>byteを返します。</returns>
        public byte Get(string section, string key, byte defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、ushortで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>ushortを返します。</returns>
        public ushort Get(string section, string key, ushort defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、uintで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>uintを返します。</returns>
        public uint Get(string section, string key, uint defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、ulongで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>ulongを返します。</returns>
        public ulong Get(string section, string key, ulong defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、floatで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>文字列を返します。</returns>
        public float Get(string section, string key, float defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、doubleで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>doubleを返します。</returns>
        public double Get(string section, string key, double defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、decimalで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>decimalを返します。</returns>
        public decimal Get(string section, string key, decimal defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、charで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>charを返します。</returns>
        public char Get(string section, string key, char defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、boolで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>boolを返します。</returns>
        public bool Get(string section, string key, bool defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、IPAddressで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>IPAddressを返します。</returns>
        public System.Net.IPAddress Get(string section, string key, System.Net.IPAddress defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、DateTimeで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>DateTimeを返します。</returns>
        public DateTime Get(string section, string key, DateTime defaultValue) { return Read(section, key, defaultValue); }

        /// <summary>
        ///	指定されたセクション、キーの値を、Colorで取得します。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="defaultValue">指定されたセクション、キーの値がな場合に返却する値を指定します。</param>
        /// <returns>DateTimeを返します。</returns>
        public Color Get(string section, string key, Color defaultValue) { return Read(section, key, defaultValue); }

























        /// <summary>
        /// 指定されたセクション、キーに指定された文字列を書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, string writeValue)
        {
            return WritePrivateProfileString(section, key, writeValue, fileInfo.FullName) != 0;
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたsbyteを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, sbyte writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたshortを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, short writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたintを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, int writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたlongを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, long writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたbyteを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, byte writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたushortを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, ushort writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたuintを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, uint writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたulongを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, ulong writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたfloatを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, float writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたdoubleを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, double writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたdecimalを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, decimal writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたcharを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, char writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたboolを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, bool writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたIPAddressを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, System.Net.IPAddress writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定されたDateTimeを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Write(string section, string key, DateTime writeValue)
        {
            return Write(section, key, writeValue.ToString());
        }

        /// <summary>
        /// 指定されたセクション、キーに指定された文字列を書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, string writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたsbyteを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, sbyte writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたshortを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, short writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたintを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, int writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたlongを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, long writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたbyteを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, byte writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたushortを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, ushort writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたuintを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, uint writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたulongを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, ulong writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたfloatを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, float writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたdoubleを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, double writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたdecimalを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, decimal writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたcharを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, char writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたboolを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, bool writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたIPAddressを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, System.Net.IPAddress writeValue) { return Write(section, key, writeValue); }

        /// <summary>
        /// 指定されたセクション、キーに指定されたDateTimeを書き込みます。
        /// </summary>
        /// <param name="section">セクションを指定します。</param>
        /// <param name="key">キーを指定します。</param>
        /// <param name="writeValue">書き込む値を指定します。</param>
        /// <returns>
        /// 書き込みに成功した場合、trueを返します。<br/>
        /// 書き込みに失敗した場合、falseを返します。
        /// </returns>
        public bool Set(string section, string key, DateTime writeValue) { return Write(section, key, writeValue); }



















        /// <summary>
        /// GetPrivateProfileStringのDLLインポート<br/>
        /// iniファイルの読み込み用の関数(GetPrivateProfileString)です。<br/>
        /// 本関数の詳細はWin32APIを参照してください。
        /// </summary>
        /// <param name="lpszSection">セクション名のアドレス</param>
        /// <param name="lpszKey">キー名のアドレス</param>
        /// <param name="lpszDefault">デフォルト文字列のアドレス</param>
        /// <param name="lpszReturnBuffer">転送先バッファのアドレス</param>
        /// <param name="cchReturnBuffer">転送先バッファのサイズ</param>
        /// <param name="lpszFile">初期化ファイル名のアドレス</param>
        /// <returns>指定されたバッファにコピーされた文字の数を示します。</returns>
        [DllImport("kernel32.dll", EntryPoint = "GetPrivateProfileString")]
        public static extern uint GetPrivateProfileString(string lpszSection, string lpszKey, string lpszDefault, System.Text.StringBuilder lpszReturnBuffer, uint cchReturnBuffer, string lpszFile);

        /// <summary>
        /// WritePrivateProfileStringのDLLインポート<br/>
        /// iniファイルの書き込み用の関数(WritePrivateProfileString)です。<br/>
        /// 本関数の詳細はWin32APIを参照してください。
        /// </summary>
        /// <param name="lpszSection">セクション名のアドレス</param>
        /// <param name="lpszKey">キー名のアドレス</param>
        /// <param name="lpszString">追加する文字列のアドレス</param>
        /// <param name="lpszFile">初期化ファイル名のアドレス</param>
        /// <returns>関数が文字列を .ini ファイルに格納することに成功すると、0 以外の値が返ります。</returns>
        [DllImport("kernel32.dll", EntryPoint = "WritePrivateProfileString")]
        public static extern uint WritePrivateProfileString(string lpszSection, string lpszKey, string lpszString, string lpszFile);

        /// <summary>
        /// キー値を取得する際のの最大サイズを示します。<br/>
        /// </summary>
        private const int readValueSizeCapacity = 256;


        /// <summary>
        /// iniファイルの情報です。
        /// </summary>
        private System.IO.FileInfo fileInfo = null;

		/// <summary>
		/// 色名称や色コード(#rrggbb)からColorを取得します。
		/// </summary>
		/// <param name="name"></param>
		/// <param name="defaultColor"></param>
		/// <returns></returns>
		public static Color GetColor(string name, Color defaultColor)
		{
			int A = defaultColor.A;
			int R = defaultColor.R;
			int G = defaultColor.G;
			int B = defaultColor.B;

			if (name != "")
			{
				string[] rgb = name.Split(',');
				if (rgb.Length == 1)
				{
					if (0 < name.Length)
					{
						if (name[0] == '#')
						{
							int argb;
							try
							{
								argb = int.Parse(name.Substring(1), System.Globalization.NumberStyles.HexNumber);
								if (argb <= 0xFFFFFF) A = 255;
								else A = argb >> 24 & 0xFF;
								R = argb >> 16 & 0xFF;
								G = argb >> 8 & 0xFF;
								B = argb & 0xFF;

								return Color.FromArgb(A, R, G, B);
							}
							catch { }
						}
					}

					int test = 0;
					if (!int.TryParse(rgb[0], out test))
					{
						return Color.FromName(rgb[0]);
					}
				}

				{
					if (3 < rgb.Length)
					{
						int.TryParse(rgb[0], out A);
						int.TryParse(rgb[1], out R);
						int.TryParse(rgb[2], out G);
						int.TryParse(rgb[3], out B);
					}
					else
					{
						A = 255;
						if (0 < rgb.Length) int.TryParse(rgb[0], out R);
						if (1 < rgb.Length) int.TryParse(rgb[1], out G);
						if (2 < rgb.Length) int.TryParse(rgb[2], out B);
					}
				}
			}

			return Color.FromArgb(A, R, G, B);
		}
    }
}
