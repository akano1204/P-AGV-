using System;

namespace BelicsClass.Common
{
    /// <summary>
    /// シリアライズ可能なDateTime型クラスです
    /// シリアライズされるデータはTick値です
    /// </summary>
    [Serializable]
    public class BL_SerializableDateTime
    {
        /// <summary>
        /// チック値
        /// </summary>
        public long Ticks = 0;

        /// <summary>
        /// DateTime型として取得または設定します
        /// </summary>
        public DateTime ToDateTime
        {
            get { return new DateTime(Ticks); }
            set { Ticks = value.Ticks; }
        }

        /// <summary>
        /// コンストラクタ
        /// Tick値は0で初期化されます
        /// </summary>
        public BL_SerializableDateTime() { }

        /// <summary>
        /// コンストラクタ
        /// 指定されたDateTime値で初期化されます
        /// </summary>
        /// <param name="datetime">指定値</param>
        public BL_SerializableDateTime(DateTime datetime)
        {
            ToDateTime = datetime;
        }

        /// <summary>
        /// コンストラクタ
        /// 指定したTick値で初期化されます
        /// </summary>
        /// <param name="ticks">指定値</param>
        public BL_SerializableDateTime(long ticks)
        {
            Ticks = ticks;
        }

        /// <summary>
        /// 年部分を取得します
        /// </summary>
        public int Year
        {
            get { return ToDateTime.Year; }
        }

        /// <summary>
        /// 月部分を取得します
        /// </summary>
        public int Month
        {
            get { return ToDateTime.Month; }
        }

        /// <summary>
        /// 日部分を取得します
        /// </summary>
        public int Day
        {
            get { return ToDateTime.Day; }
        }

        /// <summary>
        /// 時部分を取得します
        /// </summary>
        public int Hour
        {
            get { return ToDateTime.Hour; }
        }

        /// <summary>
        /// 分部分を取得します
        /// </summary>
        public int Minute
        {
            get { return ToDateTime.Minute; }
        }

        /// <summary>
        /// 秒部分を取得します
        /// </summary>
        public int Second
        {
            get { return ToDateTime.Second; }
        }

        /// <summary>
        /// ミリ秒部分を取得します
        /// </summary>
        public int Millisecond
        {
            get { return ToDateTime.Millisecond; }
        }

        /// <summary>
        /// 日付部分を取得します
        /// </summary>
        public DateTime Date
        {
            get { return ToDateTime.Date; }
        }

        /// <summary>
        /// 現在日時を取得します
        /// </summary>
        public static BL_SerializableDateTime Now
        {
            get { return new BL_SerializableDateTime(DateTime.Now); }
        }

        /// <summary>
        /// 日時を表す文字列からBL_DateTime型へ変換します
        /// 変換できない場合、Tick値0のBL_DateTime型が生成されます
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static BL_SerializableDateTime Parse(string val)
        {
            DateTime dt = new DateTime(0);
            DateTime.TryParse(val, out dt);

            return new BL_SerializableDateTime(dt);
        }

        /// <summary>
        /// 日時を表す文字列を取得します
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return ToDateTime.ToString();
        }

        /// <summary>
        /// 指定されたフォーマットで日時を表す文字列を取得します
        /// </summary>
        /// <param name="format"></param>
        /// <returns></returns>
        public string ToString(string format)
        {
            return ToDateTime.ToString(format);
        }
    }
}
