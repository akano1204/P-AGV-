using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml.Serialization;

//#if !WindowsCE
//using System.Linq.Expressions;
//#endif

using BelicsClass.Common;

namespace BelicsClass.ObjectSync
{
    //#if !WindowsCE
    //    /// <summary>
    //    /// フィールド名取得クラス
    //    /// </summary>
    //    /// <typeparam name="T"></typeparam>
    //    public class BL_FieldHelper<T>
    //    {
    //        /// <summary>
    //        /// インスタンスオブジェクトからフィールド名称文字列を取得します
    //        /// </summary>
    //        /// <typeparam name="U"></typeparam>
    //        /// <param name="expression"></param>
    //        /// <param name="index"></param>
    //        /// <returns></returns>
    //        static public string GetName<U>(MemberExpression<Func<T, U>> expression, params int[] index)
    //        {
    //            string body = expression.Body.ToString();

    //            string b = "";

    //            if (0 < index.Length)
    //            {
    //                int nest = 0;
    //                int p = 0;
    //                for (int pos = 0; pos < body.Length; pos++)
    //                {
    //                    if (body[pos] == '[')
    //                    {
    //                        if (nest == 0) b += "[";
    //                        nest++;
    //                    }
    //                    else if (body[pos] == ']')
    //                    {
    //                        nest--;
    //                        if (nest == 0)
    //                        {
    //                            b += index[p++].ToString();
    //                            b += "]";
    //                        }
    //                    }
    //                    else if (nest == 0)
    //                    {
    //                        b += body[pos];
    //                    }
    //                }
    //            }
    //            else b = body;

    //            string[] members = b.Split('.');
    //            string ret = "";
    //            for (int pos = 1; pos < members.Length; pos++)
    //            {
    //                if (ret != "") ret += ".";
    //                ret += members[pos];
    //            }
    //            return ret;
    //        }
    //    }
    //#endif

    ///// <summary>
    ///// 
    ///// </summary>
    //[System.AttributeUsage(System.AttributeTargets.Field)]
    //public class BL_ObjectSyncAttributeNoEvent : BL_ObjectSyncAttribute
    //{
    //}

    /// <summary>
    /// フィールド情報を保持するクラス
    /// </summary>
    public static class BL_ObjectInformationStocker
    {
        /// <summary>
        /// １クラス分のフィールド情報のセットを保持するクラス
        /// </summary>
        public class Stocker
        {
            /// <summary>
            /// フィールド情報コレクション（ベタ）
            /// </summary>
            public Dictionary<string, BL_ObjectSync.BL_ObjectInformation> dictField = new Dictionary<string, BL_ObjectSync.BL_ObjectInformation>();

            /// <summary>
            /// フィールド情報コレクション（クラス・配列）
            /// </summary>
            public Dictionary<string, List<BL_ObjectSync.BL_ObjectInformation>> dictClasses = new Dictionary<string, List<BL_ObjectSync.BL_ObjectInformation>>();

            /// <summary>
            /// 最終オフセット
            /// </summary>
            public int Offset = 0;
        }

        /// <summary>
        /// フィールド情報セットのコレクション
        /// </summary>
        public static Dictionary<string, Stocker> dictStock = new Dictionary<string, Stocker>();

        /// <summary>
        /// 保持しているフィールド情報セットから１クラス分を取得します
        /// </summary>
        /// <param name="typename">クラス名称</param>
        /// <returns></returns>
        public static Stocker GetStock(string typename)
        {
            if (dictStock.ContainsKey(typename))
            {
                Stocker obj = dictStock[typename];
                Stocker stock = new Stocker();

                foreach (var kv in obj.dictField)
                {
                    BL_ObjectSync.BL_ObjectInformation oi = new BL_ObjectSync.BL_ObjectInformation(kv.Value.Key, kv.Value.Object, kv.Value.Field, kv.Value.Offset, kv.Value.Length, kv.Value.Index);
                    stock.dictField[kv.Key] = oi;
                }

                foreach (var kv in obj.dictClasses)
                {
                    List<BL_ObjectSync.BL_ObjectInformation> list = new List<BL_ObjectSync.BL_ObjectInformation>();
                    foreach (var v in kv.Value)
                    {
                        BL_ObjectSync.BL_ObjectInformation oi = new BL_ObjectSync.BL_ObjectInformation(v.Key, v.Object, v.Field, v.Offset, v.Length, v.Index);
                        list.Add(oi);
                    }
                    stock.dictClasses[kv.Key] = list;
                }

                stock.Offset = obj.Offset;

                return stock;
            }
            return null;
        }

        /// <summary>
        /// 初期化済みフィールド情報をコレクションへ追加します
        /// </summary>
        /// <param name="typename"></param>
        /// <param name="obj"></param>
        public static void AddStock(string typename, BL_ObjectSync obj)
        {
            Stocker stock = new Stocker();

            foreach (var kv in obj.GetFieldDictionary())
            {
                BL_ObjectSync.BL_ObjectInformation oi = new BL_ObjectSync.BL_ObjectInformation(kv.Value.Key, kv.Value.Object, kv.Value.Field, kv.Value.Offset, kv.Value.Length, kv.Value.Index);
                stock.dictField[kv.Key] = oi;
            }

            foreach (var kv in obj.GetClassesDictionary())
            {
                List<BL_ObjectSync.BL_ObjectInformation> list = new List<BL_ObjectSync.BL_ObjectInformation>();
                foreach (var v in kv.Value)
                {
                    BL_ObjectSync.BL_ObjectInformation oi = new BL_ObjectSync.BL_ObjectInformation(v.Key, v.Object, v.Field, v.Offset, v.Length, v.Index);
                    list.Add(oi);
                }
                stock.dictClasses[kv.Key] = list;
            }

            stock.Offset = obj.Length;

            dictStock[typename] = stock;
        }
    }

    /// <summary>
    /// クラスインスタンスとバイト配列データの相互自動変換を行うクラス
    /// 
    /// 自インスタンスが所持するprivate以外のフィールドを解析して、必要数分のバイト配列を生成するための準備が行われます
    /// フィールドは本クラスのInitializeメソッドコールによって解析され、バイト配列とオブジェクト間の相互変換が行われます
    /// null値はフィールドの初期値として使用できません
    /// stringフィールドは、必要文字数分の空白で初期化してください
    /// 
    /// フィールドには「下記の組み込み型、および下記で構成されたクラス、配列」を使用することができます
    /// Boolean     4byte
    /// Int16       2byte
    /// Int32       4byte
    /// Int64       8byte
    /// UInt16      2byte
    /// UInt32      4byte
    /// UInt64      8byte
    /// Double      8byte
    /// Single      4byte
    /// Byte        1byte
    /// Char        1byte
    /// String      ??? byte
    /// DateTime    8byte(Int64 Ticks値で保持)
    /// TimeSpan    8byte(Int64 Ticks値で保持)
    /// 
    /// </summary>
    [Serializable]
    unsafe public class BL_ObjectSync
    {
        /// <summary>
        /// フィールドの情報を保持してデータ変換を行うクラス
        /// </summary>
        [Serializable]
        public class BL_ObjectInformation
        {
            /// <summary>
            /// フィールド名
            /// </summary>
            [XmlIgnore]
            [NonSerialized]
            public string Key;

            /// <summary>
            /// フィールドオブジェクトが所属する親オブジェクトへの参照
            /// </summary>
            [XmlIgnore]
            [NonSerialized]
            public object Object;

            /// <summary>
            /// フィールド情報
            /// </summary>
            [XmlIgnore]
            [NonSerialized]
            public FieldInfo Field;

            /// <summary>
            /// バイト配列のオフセット位置
            /// </summary>
            [XmlIgnore]
            [NonSerialized]
            public int Offset;

            /// <summary>
            /// バイト配列のデータ長
            /// </summary>
            [XmlIgnore]
            [NonSerialized]
            public int Length;

            /// <summary>
            /// 配列の場合のインデックス
            /// </summary>
            [XmlIgnore]
            [NonSerialized]
            public int Index;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="key">フィールド名</param>
            /// <param name="obj">親オブジェクト</param>
            /// <param name="field">フィールド情報</param>
            /// <param name="offset">バイト配列のオフセット位置</param>
            /// <param name="length">バイト配列のデータ長</param>
            public BL_ObjectInformation(string key, object obj, FieldInfo field, int offset, int length)
            {
                Key = key;
                Object = obj;
                Field = field;
                Offset = offset;
                Length = length;
                Index = -1;
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="key">フィールド名</param>
            /// <param name="obj">親オブジェクト</param>
            /// <param name="field">フィールド情報</param>
            /// <param name="offset">バイト配列のオフセット位置</param>
            /// <param name="length">バイト配列のデータ長</param>
            /// <param name="index">配列の場合のインデックス</param>
            public BL_ObjectInformation(string key, object obj, FieldInfo field, int offset, int length, int index)
            {
                Key = key;
                Object = obj;
                Field = field;
                Offset = offset;
                Length = length;
                Index = index;
            }

            /// <summary>
            /// バイト配列を取得します
            /// </summary>
            /// <returns>バイト配列</returns>
            public byte[] GetBuffer()
            {
                return GetBuffer(null);
            }

            /// <summary>
            /// 指定したオブジェクトのバイト配列を取得します
            /// </summary>
            /// <param name="val">指定オブジェクト</param>
            /// <returns>バイト配列</returns>
            public byte[] GetBuffer(object val)
            {
                //byte[] buffer = new byte[Length];
                byte[] buffer = null;

                if (val != null)
                {
                    if (val.GetType().IsArray)
                    {
                        Array ar = (Array)val;
                        val = ar.GetValue(Index);
                    }

                    if (val.GetType().IsClass &&
                        val.GetType() != typeof(String) &&
                        val.GetType() != typeof(BL_BitOperator) &&
                        val.GetType() != typeof(Color)
                        )
                    {
                        val = Field.GetValue(val);
                    }
                }
                else
                {
                    val = Field.GetValue(Object);

                    if (Field.FieldType.IsArray)
                    {
                        Array ar = (Array)Field.GetValue(Object);
                        val = ar.GetValue(Index);
                    }
                }

                if (val == null)
                {
                }
                else if (val.GetType() == typeof(Boolean))
                {
                    buffer = new byte[4];
                    byte[] buf = BitConverter.GetBytes((Boolean)val);
                    buffer[0] = buf[0];
                }
                else if (val.GetType() == typeof(Int16))
                {
                    buffer = BitConverter.GetBytes((Int16)val);
                }
                else if (val.GetType() == typeof(Int32))
                {
                    buffer = BitConverter.GetBytes((Int32)val);
                }
                else if (val.GetType() == typeof(Int64))
                {
                    buffer = BitConverter.GetBytes((Int64)val);
                }
                else if (val.GetType() == typeof(UInt16))
                {
                    buffer = BitConverter.GetBytes((UInt16)val);
                }
                else if (val.GetType() == typeof(UInt32))
                {
                    buffer = BitConverter.GetBytes((UInt32)val);
                }
                else if (val.GetType() == typeof(UInt64))
                {
                    buffer = BitConverter.GetBytes((UInt64)val);
                }
                else if (val.GetType() == typeof(Double))
                {
                    buffer = BitConverter.GetBytes((Double)val);
                }
                else if (val.GetType() == typeof(Single))
                {
                    buffer = BitConverter.GetBytes((Single)val);
                }
                else if (val.GetType() == typeof(Byte))
                {
                    buffer = new byte[1];
                    buffer[0] = BitConverter.GetBytes((Byte)val)[0];
                }
                else if (val.GetType() == typeof(Char))
                {
                    buffer = new byte[1];
                    buffer[0] = BitConverter.GetBytes((Char)val)[0];
                }
                else if (val.GetType() == typeof(String))
                {
                    //byte[] buf = Encoding.Default.GetBytes(((String)val).PadRight(Length));
                    byte[] buf = Encoding.Default.GetBytes(((String)val).Trim('\0'));
                    int len = buf.Length;
                    if (Length < buf.Length) len = Length;
                    buffer = new byte[len];
                    Array.Copy(buf, buffer, len);
                }
                else if (val.GetType() == typeof(DateTime))
                {
                    DateTime dt = (DateTime)val;
                    buffer = BitConverter.GetBytes((Int64)dt.Ticks);
                }
                else if (val.GetType() == typeof(TimeSpan))
                {
                    TimeSpan ts = (TimeSpan)val;
                    buffer = BitConverter.GetBytes((Int64)ts.Ticks);
                }
                else if (val.GetType() == typeof(BL_BitOperator))
                {
                    BL_BitOperator bit = (BL_BitOperator)val;
                    buffer = BitConverter.GetBytes(bit.Value);
                }
                else if (val.GetType() == typeof(Color))
                {
                    Color col = (Color)val;
                    buffer = BitConverter.GetBytes(col.ToArgb());
                }
                else if (Field.FieldType.IsEnum)
                {
                    byte[] buf = Encoding.Default.GetBytes((val.ToString()).Trim('\0'));
                    int len = buf.Length;
                    if (Length < buf.Length) len = Length;
                    buffer = new byte[len];
                    Array.Copy(buf, buffer, len);
                }
                else
                {
                    throw new NotSupportedException("サポートされていない型を変換できません。");
                }

                return buffer;
            }

            /// <summary>
            /// 指定バイト配列でオブジェクトを設定します
            /// </summary>
            /// <param name="buffer">指定バイト配列</param>
            /// <returns>設定されたオブジェクト</returns>
            public object SetBuffer(byte[] buffer)
            {
                object ret = null;
                if (Field.FieldType.IsArray)
                {
                    Array ar = (Array)Field.GetValue(Object);
                    object value = ar.GetValue(Index);

                    if (value.GetType() == typeof(Boolean))
                    {
                        Boolean val = BitConverter.ToBoolean(buffer, 0);
                        ar.SetValue(val, Index);
                        Field.SetValue(Object, ar);
                        ret = val;
                    }
                    else if (value.GetType() == typeof(Int16))
                    {
                        Int16 val = BitConverter.ToInt16(buffer, 0);
                        ar.SetValue(val, Index);
                        Field.SetValue(Object, ar);
                        ret = val;
                    }
                    else if (value.GetType() == typeof(Int32))
                    {
                        Int32 val = BitConverter.ToInt32(buffer, 0);
                        ar.SetValue(val, Index);
                        Field.SetValue(Object, ar);
                        ret = val;
                    }
                    else if (value.GetType() == typeof(Int64))
                    {
                        Int64 val = BitConverter.ToInt64(buffer, 0);
                        ar.SetValue(val, Index);
                        Field.SetValue(Object, ar);
                        ret = val;
                    }
                    else if (value.GetType() == typeof(UInt16))
                    {
                        UInt16 val = BitConverter.ToUInt16(buffer, 0);
                        ar.SetValue(val, Index);
                        Field.SetValue(Object, ar);
                        ret = val;
                    }
                    else if (value.GetType() == typeof(UInt32))
                    {
                        UInt32 val = BitConverter.ToUInt32(buffer, 0);
                        ar.SetValue(val, Index);
                        Field.SetValue(Object, ar);
                        ret = val;
                    }
                    else if (value.GetType() == typeof(UInt64))
                    {
                        UInt64 val = BitConverter.ToUInt64(buffer, 0);
                        ar.SetValue(val, Index);
                        Field.SetValue(Object, ar);
                        ret = val;
                    }
                    else if (value.GetType() == typeof(Double))
                    {
                        Double val = BitConverter.ToDouble(buffer, 0);
                        ar.SetValue(val, Index);
                        Field.SetValue(Object, ar);
                        ret = val;
                    }
                    else if (value.GetType() == typeof(Single))
                    {
                        Single val = BitConverter.ToSingle(buffer, 0);
                        ar.SetValue(val, Index);
                        Field.SetValue(Object, ar);
                        ret = val;
                    }
                    else if (value.GetType() == typeof(Byte))
                    {
                        Byte val = buffer[0];
                        ar.SetValue(val, Index);
                        Field.SetValue(Object, ar);
                        ret = val;
                    }
                    else if (value.GetType() == typeof(Char))
                    {
                        Char val = BitConverter.ToChar(buffer, 0);
                        ar.SetValue(val, Index);
                        Field.SetValue(Object, ar);
                        ret = val;
                    }
                    else if (value.GetType() == typeof(String))
                    {
                        string val = Encoding.Default.GetString(buffer, 0, buffer.Length);
                        //ar.SetValue(val.Trim('\0').PadRight(Length), Index);
                        ar.SetValue(val.Trim('\0'), Index);
                        Field.SetValue(Object, ar);
                        ret = val.Trim('\0');
                        val = null;
                    }
                    else if (value.GetType() == typeof(DateTime))
                    {
                        Int64 val = BitConverter.ToInt64(buffer, 0);
                        DateTime dt = new DateTime(val);
                        ar.SetValue(dt, Index);
                        Field.SetValue(Object, ar);
                        ret = dt;
                    }
                    else if (value.GetType() == typeof(TimeSpan))
                    {
                        Int64 val = BitConverter.ToInt64(buffer, 0);
                        TimeSpan ts = new TimeSpan(val);
                        ar.SetValue(ts, Index);
                        Field.SetValue(Object, ar);
                        ret = ts;
                    }
                    else if (value.GetType() == typeof(BL_BitOperator))
                    {
                        ushort val = BitConverter.ToUInt16(buffer, 0);
                        BL_BitOperator bit = new BL_BitOperator(val);
                        ar.SetValue(bit, Index);
                        Field.SetValue(Object, ar);
                        ret = bit;
                        bit = null;
                    }
                    else if (value.GetType() == typeof(Color))
                    {
                        int val = BitConverter.ToInt32(buffer, 0);
                        Color col = Color.FromArgb(val);
                        ar.SetValue(col, Index);
                        Field.SetValue(Object, ar);
                        ret = col;
                    }
                    else if (Field.FieldType.IsEnum)
                    {
                        string val = Encoding.Default.GetString(buffer, 0, buffer.Length);
                        Array arr = Enum.GetValues(Field.FieldType);

                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (arr.GetValue(i).ToString() == val.Trim())
                            {
                                ar.SetValue(arr.GetValue(i), Index);
                                Field.SetValue(Object, ar);
                            }
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("サポートされていない型を変換できません。");
                    }
                }
                else
                {
                    if (Field.FieldType == typeof(Boolean))
                    {
                        Boolean val = BitConverter.ToBoolean(buffer, 0);
                        Field.SetValue(Object, (Boolean)val);
                        ret = val;
                    }
                    else if (Field.FieldType == typeof(Int16))
                    {
                        Int16 val = BitConverter.ToInt16(buffer, 0);
                        Field.SetValue(Object, val);
                        ret = val;
                    }
                    else if (Field.FieldType == typeof(Int32))
                    {
                        Int32 val = BitConverter.ToInt32(buffer, 0);
                        Field.SetValue(Object, val);
                        ret = val;
                    }
                    else if (Field.FieldType == typeof(Int64))
                    {
                        Int64 val = BitConverter.ToInt64(buffer, 0);
                        Field.SetValue(Object, val);
                        ret = val;
                    }
                    else if (Field.FieldType == typeof(UInt16))
                    {
                        UInt16 val = BitConverter.ToUInt16(buffer, 0);
                        Field.SetValue(Object, val);
                        ret = val;
                    }
                    else if (Field.FieldType == typeof(UInt32))
                    {
                        UInt32 val = BitConverter.ToUInt32(buffer, 0);
                        Field.SetValue(Object, val);
                        ret = val;
                    }
                    else if (Field.FieldType == typeof(UInt64))
                    {
                        UInt64 val = BitConverter.ToUInt64(buffer, 0);
                        Field.SetValue(Object, val);
                        ret = val;
                    }
                    else if (Field.FieldType == typeof(Double))
                    {
                        Double val = BitConverter.ToDouble(buffer, 0);
                        Field.SetValue(Object, val);
                        ret = val;
                    }
                    else if (Field.FieldType == typeof(Single))
                    {
                        Single val = BitConverter.ToSingle(buffer, 0);
                        Field.SetValue(Object, val);
                        ret = val;
                    }
                    else if (Field.FieldType == typeof(Byte))
                    {
                        Byte val = buffer[0];
                        Field.SetValue(Object, val);
                        ret = val;
                    }
                    else if (Field.FieldType == typeof(Char))
                    {
                        byte[] buf = new byte[2];
                        buf[0] = buffer[0];
                        Char val = BitConverter.ToChar(buf, 0);
                        Field.SetValue(Object, val);
                        ret = val;
                        buf = null;
                    }
                    else if (Field.FieldType == typeof(String))
                    {
                        string val = Encoding.Default.GetString(buffer, 0, buffer.Length);
                        //Field.SetValue(Object, val.Trim('\0').PadRight(Length));
                        Field.SetValue(Object, val.Trim('\0'));
                        ret = val.Trim('\0');
                        val = null;
                    }
                    else if (Field.FieldType == typeof(DateTime))
                    {
                        Int64 val = BitConverter.ToInt64(buffer, 0);
                        DateTime dt = new DateTime(val);
                        Field.SetValue(Object, dt);
                        ret = dt;
                    }
                    else if (Field.FieldType == typeof(TimeSpan))
                    {
                        Int64 val = BitConverter.ToInt64(buffer, 0);
                        TimeSpan ts = new TimeSpan(val);
                        Field.SetValue(Object, ts);
                        ret = ts;
                    }
                    else if (Field.FieldType == typeof(BL_BitOperator))
                    {
                        ushort val = BitConverter.ToUInt16(buffer, 0);
                        BL_BitOperator bit = new BL_BitOperator(val);
                        Field.SetValue(Object, bit);
                        ret = bit;
                        bit = null;
                    }
                    else if (Field.FieldType == typeof(Color))
                    {
                        int val = BitConverter.ToInt32(buffer, 0);
                        Color col = Color.FromArgb(val);
                        Field.SetValue(Object, col);
                        ret = col;
                    }
                    else if (Field.FieldType.IsEnum)
                    {
                        string val = Encoding.Default.GetString(buffer, 0, buffer.Length);
                        val = val.Trim();
                        val = val.Trim('\0');
                        val = val.Trim();
                        Array arr = Enum.GetValues(Field.FieldType);

                        for (int i = 0; i < arr.Length; i++)
                        {
                            if (arr.GetValue(i).ToString() == val)
                            {
                                Field.SetValue(Object, arr.GetValue(i));
                                break;
                            }
                        }
                    }
                    else
                    {
                        throw new NotSupportedException("サポートされていない型を変換できません。");
                    }
                }

                return ret;
            }

            /// <summary>
            /// 保持しているデータを取得します
            /// </summary>
            /// <returns></returns>
            public object GetData()
            {
                object val = Field.GetValue(Object);

                if (Field.FieldType.IsArray)
                {
                    Array ar = (Array)Field.GetValue(Object);
                    val = ar.GetValue(Index);
                }

                return val;
            }

            /// <summary>
            /// データを設定します
            /// </summary>
            /// <param name="data">設定データ</param>
            /// <returns></returns>
            public object SetData(object data)
            {
                return SetBuffer(GetBuffer(data));

                //Field.SetValue(Object, data);
                //return GetData();
            }
        }

        /// <summary>
        /// フィールド情報コレクション（ベタ）
        /// </summary>
        [XmlIgnore]
        [NonSerialized]
        protected Dictionary<string, BL_ObjectInformation> dictField = new Dictionary<string, BL_ObjectInformation>();

        /// <summary>
        /// フィールド情報コレクション（クラス・配列）
        /// </summary>
        [XmlIgnore]
        [NonSerialized]
        protected Dictionary<string, List<BL_ObjectInformation>> dictClasses = new Dictionary<string, List<BL_ObjectInformation>>();

        /// <summary>
        /// 最終オフセット
        /// </summary>
        [XmlIgnore]
        [NonSerialized]
        private int Offset = 0;

        /// <summary>
        /// フィールド情報コレクション（ベタ）を取得します
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, BL_ObjectInformation> GetFieldDictionary()
        {
            return dictField;
        }

        /// <summary>
        /// フィールド情報コレクション（クラス・配列）を取得します
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<BL_ObjectInformation>> GetClassesDictionary()
        {
            return dictClasses;
        }

        /// <summary>
        /// 全てのフィールド名を取得します
        /// </summary>
        /// <returns></returns>
        public string[] GetAllFieldNames()
        {
            List<string> list = new List<string>();
            foreach (string s in dictField.Keys) list.Add(s);

            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="field_name"></param>
        /// <param name="index"></param>
        protected void InstanceReplacer(object x, string field_name, int index)
        {
            if (x == null) return;

            Type type = x.GetType();
            string class_name = field_name;
            if (0 <= index) class_name += "[" + index.ToString() + "]";

            FieldInfo[] allfields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var v in allfields)
            {
                object[] field_attributes = v.GetCustomAttributes(typeof(BL_ObjectSyncAttribute), true);
                if (field_attributes.Length == 0)
                {
                    if (
                        type.ToString().IndexOf("System.Collections.Generic.List") != 0 &&
                        type.ToString().IndexOf("System.Collections.Generic.Array") != 0
                        )
                        continue;
                }

                string member_name = v.Name;
                if (class_name != "")
                {
                    if (!dictClasses.ContainsKey(class_name))
                    {
                        dictClasses[class_name] = new List<BL_ObjectInformation>();
                    }
                    member_name = class_name + "." + v.Name;
                }

                if (v.FieldType.IsArray)
                {
                    Array arr = (Array)v.GetValue(x);

                    for (int pos = 0; pos < arr.Length; pos++)
                    {
                        object element = arr.GetValue(pos);
                        if (element == null) break;

                        Type t = element.GetType();

                        if (t.IsClass &&
                            t != typeof(String) &&
                            t != typeof(DateTime) &&
                            t != typeof(TimeSpan) &&
                            t != typeof(BL_BitOperator) &&
                            t != typeof(Color) &&
                            !t.IsEnum
                            )
                        {
                            InstanceReplacer(element, member_name, pos);
                        }
                        else
                        {
                            if (dictField.ContainsKey(member_name + "[" + pos + "]"))
                            {
                                dictField[member_name + "[" + pos + "]"].Object = x;
                            }
                        }
                    }
                }
                else if (v.FieldType.IsClass && v.FieldType != typeof(String))
                {
                    InstanceReplacer(v.GetValue(x), member_name, -1);
                    continue;
                }
                else
                {
                    if (dictField.ContainsKey(member_name))
                    {
                        dictField[member_name].Object = x;
                    }
                }

            }

            allfields = null;
        }

        /// <summary>
        /// クラス情報を解析してフィールドコレクションを生成します
        /// </summary>
        /// <param name="x">対象クラス</param>
        /// <param name="field_name">フィールド名</param>
        /// <param name="index">配列の場合はインデックス、通常[-1]</param>
        protected void ObjectAnalyze(object x, string field_name, int index)
        {
            //if (!typeof(BL_ObjectSync).IsInstanceOfType(x))
            //{
            //    throw new Exception("[" + x.GetType().Name + "] は BL_ObjectSync型から派生されたデータ型が必要です。");
            //}

            if (field_name == "")
            {
                Offset = 0;
                dictClasses.Clear();
                dictField.Clear();
            }

            if (x == null) return;

            Type type = x.GetType();
            string class_name = field_name;
            if (0 <= index) class_name += "[" + index.ToString() + "]";

            FieldInfo[] allfields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            foreach (var v in allfields)
            {
                //if (v.IsPrivate) continue;

                object[] field_attributes = v.GetCustomAttributes(typeof(BL_ObjectSyncAttribute), true);
                if (field_attributes.Length == 0)
                {
                    if (type.ToString().IndexOf("System.Collections.Generic.List") != 0
                        //&& type.ToString().IndexOf("System.Collections.ArrayList") != 0
                        //&& type.ToString().IndexOf("System.Collections.Generic.Dictionary") != 0
                        )
                        continue;
                }

                string member_name = v.Name;
                if (class_name != "")
                {
                    if (!dictClasses.ContainsKey(class_name))
                    {
                        dictClasses[class_name] = new List<BL_ObjectInformation>();
                    }
                    member_name = class_name + "." + v.Name;
                }

                if (v.FieldType.IsArray)
                {
                    dictClasses[member_name] = new List<BL_ObjectInformation>();
					Array arr = (Array)v.GetValue(x);

                    for (int pos = 0; pos < arr.Length; pos++)
                    {
                        object element = arr.GetValue(pos);
                        if (element == null) break;

                        Type t = element.GetType();

                        if (t.IsClass &&
                            t != typeof(String) &&
                            t != typeof(DateTime) &&
                            t != typeof(TimeSpan) &&
                            t != typeof(BL_BitOperator) &&
                            t != typeof(Color) &&
                            !t.IsEnum
                            )
                        {
                            ObjectAnalyze(element, member_name, pos);

							if (class_name != "")
							{
								foreach (var vv in dictClasses[member_name]) dictClasses[class_name].Add(vv);
							}
						}
						else if (t.IsGenericType)
                        {
                            ObjectAnalyze(element, member_name, pos);

							if (class_name != "")
							{
								foreach (var vv in dictClasses[member_name]) dictClasses[class_name].Add(vv);
							}
						}
						else
                        {
                            int length;
                            if (t == typeof(String))
                            {
                                object[] attributes = v.GetCustomAttributes(typeof(MarshalAsAttribute), false);
                                if (0 < attributes.Length)
                                {
                                    MarshalAsAttribute marshal = (MarshalAsAttribute)attributes[0];
                                    length = marshal.SizeConst;
                                }
                                else
                                {
                                    // throw new Exception("string型は[[MarshalAs(UnmanagedType.ByValTStr, SizeConst = n)]]属性を指定しなければなりません");
                                    length = ((string)element).Length;
                                }
                            }
                            else if (t == typeof(DateTime))
                            {
                                long temp = 0;
                                length = Marshal.SizeOf(temp);
                            }
                            else if (t == typeof(TimeSpan))
                            {
                                long temp = 0;
                                length = Marshal.SizeOf(temp);
                            }
                            else if (t == typeof(BL_BitOperator))
                            {
                                ushort temp = 0;
                                length = Marshal.SizeOf(temp);
                            }
                            else if (t == typeof(Color))
                            {
                                int temp = 0;
                                length = Marshal.SizeOf(temp);
                            }
                            else if (t.IsEnum)
                            {
                                length = 32;
                            }
                            else
                            {
                                length = Marshal.SizeOf(element);
                            }

                            BL_ObjectInformation pl = new BL_ObjectInformation(member_name + "[" + pos + "]", x, v, Offset, length, pos);
                            dictField[member_name + "[" + pos + "]"] = pl;

							//???
							if (class_name != "") dictClasses[class_name].Add(pl);

							dictClasses[member_name].Add(pl);


							Offset += length;
                        }
                    }
                }
                else if (v.FieldType.IsClass && v.FieldType != typeof(String))
                {
                    ObjectAnalyze(v.GetValue(x), member_name, -1);

					continue;
                }
                //else if (v.FieldType.IsNested && v.FieldType != typeof(String))
                //{
                //    ObjectAnalyze(v.GetValue(x), member_name, -1);
                //    continue;
                //}
                else if (v.FieldType.IsGenericType)
                {
                    ObjectAnalyze(v.GetValue(x), member_name, -1);
                }
                else
                {
                    int length;
                    if (v.FieldType == typeof(String))
                    {
                        object[] attributes = v.GetCustomAttributes(typeof(MarshalAsAttribute), false);
                        if (0 < attributes.Length)
                        {
                            MarshalAsAttribute marshal = (MarshalAsAttribute)attributes[0];
                            length = marshal.SizeConst;
                        }
                        else
                        {
                            length = ((string)v.GetValue(x)).Length;
                        }
                    }
                    else if (v.FieldType == typeof(DateTime))
                    {
                        long temp = 0;
                        length = Marshal.SizeOf(temp);
                    }
                    else if (v.FieldType == typeof(TimeSpan))
                    {
                        long temp = 0;
                        length = Marshal.SizeOf(temp);
                    }
                    else if (v.FieldType == typeof(BL_BitOperator))
                    {
                        ushort temp = 0;
                        length = Marshal.SizeOf(temp);
                    }
                    else if (v.FieldType == typeof(Color))
                    {
                        int temp = 0;
                        length = Marshal.SizeOf(temp);
                    }
                    else if (v.FieldType.IsEnum)
                    {
                        length = 32;
                        //throw new NotSupportedException("enumはまだサポートしていません。ラッパープロパティを生成してenumを扱ってください。");



                        //例：
                        //  [BL_ObjectSyncAttribute]
                        //  private int ProcKind_ = (int)PROC_KIND.IDLE;
                        //  public PROC_KIND ProcKind { get { return (PROC_KIND)ProcKind_; } set { ProcKind_ = (int)value; } }
                    }
                    else
                    {
                        object value = v.GetValue(x);
                        length = Marshal.SizeOf(value);
                    }

                    BL_ObjectInformation pl = new BL_ObjectInformation(member_name, x, v, Offset, length);
                    dictField[member_name] = pl;
                    if (class_name != "") dictClasses[class_name].Add(pl);
                    Offset += length;

                    if (0 <= index) dictClasses[field_name].Add(pl);
                }

                field_attributes = null;
            }

            allfields = null;
        }

        /// <summary>
        /// バイト配列化した際のデータ長
        /// </summary>
        [XmlIgnore]
        public int Length { get { return Offset; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_ObjectSync()
        {
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~BL_ObjectSync()
        {
            Dispose();
        }

        /// <summary>
        /// オブジェクト破棄
        /// </summary>
        public virtual void Dispose()
        {
            Offset = 0;
            if (dictField != null) dictField.Clear();
            if (dictClasses != null) dictClasses.Clear();
        }

        private void Object_Initialize(object x)
        {
            if (Length <= 0)
            {
                Offset = 0;
                if (dictField != null) dictField.Clear();
                if (dictClasses != null) dictClasses.Clear();

                ObjectAnalyze(x, "", -1);
            }
            else
            {
                ObjectAdjustment();
            }
        }

        /// <summary>
        /// 初期化します
        /// 自オブジェクトが所持するprivate以外のフィールドを解析して、必要数分のバイト配列を生成するための準備が行われます
        /// 本メソッド実行後、フィールド名文字列によるデータ操作が可能となります
        /// </summary>
        /// <returns>初期化成功でtrueを返します</returns>
        public virtual bool Initialize()
        {
            return Initialize(this);
        }

        /// <summary>
        /// 初期化します
        /// 自オブジェクトが所持するprivate以外のフィールドを解析して、必要数分のバイト配列を生成するための準備が行われます
        /// 指定オブジェクトの値が適用されます
        /// 本メソッド実行後、フィールド名文字列によるデータ操作が可能となります
        /// </summary>
        /// <param name="x">指定オブジェクト</param>
        /// <returns>初期化成功でtrueを返します</returns>
        public virtual bool Initialize(object x)
        {
            Object_Initialize(x);

            return true;
        }

        /// <summary>
        /// 自オブジェクトのフィールド情報を再構築します
        /// バイナリーシリアライズなどで、自オブジェクト情報と自インスタンスが保持する参照先が不一致となった時に使用します
        /// </summary>
        public virtual void ObjectAdjustment()
        {
            Dictionary<string, BL_ObjectInformation> dict = new Dictionary<string, BL_ObjectInformation>();
            foreach (var v in dictField)
            {
                dict.Add(v.Key, v.Value);
            }

            ObjectAnalyze(this, "", -1);

            if (0 < dict.Count)
            {
                int offset = 0;

                foreach (var v in dictField)
                {
                    object obj = v.Value.GetData();
                    //if (obj.GetType() == typeof(String))
                    {
                        if (dict.ContainsKey(v.Key))
                        {
                            v.Value.Length = dict[v.Key].Length;
                        }

                        v.Value.Offset = offset;
                        offset += v.Value.Length;
                    }
                }

                Offset = offset;
            }

            dict.Clear();
        }

        /// <summary>
        /// 指定フィールドのバイト配列を取得します
        /// 空文字を指定すると自オブジェクト全体のバイト配列を取得します
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <returns>バイト配列</returns>
        public byte[] GetBytes(string field_name)
        {
            if (dictField == null) throw new NotSupportedException("Initializeされていません[" + field_name + "]");

            //BL_Stopwatch sw = new BL_Stopwatch();
            //sw.Restart();
            InstanceReplacer(this, "", -1);
            //sw.Stop();

            if (field_name == "")
            {
                byte[] buffer = new byte[Offset];

                List<BL_ObjectInformation> list = new List<BL_ObjectInformation>();
                foreach (var v in dictField.Values) list.Add(v);

                Array arr = list.ToArray();

                for (int i = 0; i < arr.Length; i++)
                {
                    BL_ObjectInformation pl = (BL_ObjectInformation)arr.GetValue(i);

                    byte[] temp = pl.GetBuffer();
                    Array.Copy(temp, 0, buffer, pl.Offset, temp.Length);
                }

                return buffer;
            }
            else if (dictField.ContainsKey(field_name))
            {
                BL_ObjectInformation pl = dictField[field_name];

                byte[] buffer = pl.GetBuffer();
                return buffer;
            }
            else if (dictClasses.ContainsKey(field_name))
            {
                int size = 0;
                foreach (BL_ObjectInformation oi in dictClasses[field_name])
                {
                    size += oi.Length;
                }

                byte[] buffer = new byte[size];

                int offset = 0;
                foreach (BL_ObjectInformation oi in dictClasses[field_name])
                {
                    byte[] data = oi.GetBuffer();
                    Array.Copy(data, 0, buffer, offset, data.Length);
                    offset += oi.Length;
                }

                return buffer;
            }
            else
            {
                throw new NotSupportedException("フィールドが存在しません[" + field_name + "]");
            }

            //return null;
        }

        /// <summary>
        /// バイト配列を自オブジェクトの指定フィールドへ設定します
        /// 空文字を指定すると自オブジェクト全体へバイト配列を設定します
        /// </summary>
        /// <param name="buffer">バイト配列</param>
        /// <param name="field_name">指定フィールド</param>
        /// <returns>設定が正常に行われた場合trueを返します</returns>
        public bool SetBytes(byte[] buffer, string field_name)
        {
            if (dictField == null) throw new NotSupportedException("Initializeされていません[" + field_name + "]");
            if (buffer == null) throw new NotSupportedException("bufferがnullです[" + field_name + "]");

            if (field_name == "")
            {
                List<BL_ObjectInformation> list = new List<BL_ObjectInformation>();
                foreach (var v in dictField.Values) list.Add(v);
                Array arr = list.ToArray();
                for (int i = 0; i < arr.Length; i++)
                {
                    BL_ObjectInformation pl = (BL_ObjectInformation)arr.GetValue(i);
                    byte[] temp = new byte[pl.Length];
                    Array.Copy(buffer, pl.Offset, temp, 0, pl.Length);
                    pl.SetBuffer(temp);

                    #region List<T> の要素インスタンス再配置

                    if (pl.Key.Length - 6 == pl.Key.LastIndexOf("._size"))
                    {
                        if (pl.Object.GetType().ToString().IndexOf("System.Collections.Generic.List") == 0)
                        {
                            string name = pl.Key.Substring(0, pl.Key.Length - 6);
                            int size = (int)pl.GetData();

                            FieldInfo f = pl.Object.GetType().GetField("_items", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            Array obj = (Array)f.GetValue(pl.Object);

                            for (int ii = 0; ii < size; ii++)
                            {
                                if (dictClasses.ContainsKey(name + "._items[" + ii.ToString() + "]"))
                                {
                                    List<BL_ObjectInformation> oi = dictClasses[name + "._items[" + ii.ToString() + "]"];
                                    if (0 < oi.Count)
                                    {
                                        obj.SetValue(oi[0].Object, ii);
                                    }
                                }
                            }
                        }
                    }

                    #endregion
                }

                return true;
            }
            else if (dictField.ContainsKey(field_name))
            {
                BL_ObjectInformation pl = dictField[field_name];
                pl.SetBuffer(buffer);
                return true;
            }
            else if (dictClasses.ContainsKey(field_name))
            {
                int offset = 0;
                foreach (BL_ObjectInformation oi in dictClasses[field_name])
                {
                    byte[] data = new byte[oi.Length];
                    Array.Copy(buffer, offset, data, 0, oi.Length);
                    offset += oi.Length;

                    oi.SetBuffer(data);
                }
                return true;
            }
            else
            {
                throw new NotSupportedException("フィールドが存在しません[" + field_name + "]");
            }

            //return false;
        }

        /// <summary>
        /// 自オブジェクト全体のバイト配列を取得します
        /// </summary>
        /// <returns></returns>
        public byte[] GetBytes()
        {
            return GetBytes("");
        }

        /// <summary>
        /// バイト配列を自オブジェクト全体へ設定します
        /// </summary>
        /// <param name="buffer">バイト配列</param>
        /// <returns>設定が正常に行われた場合trueを返します</returns>
        public bool SetBytes(byte[] buffer)
        {
            return SetBytes(buffer, "");
        }

        /// <summary>
        /// 指定フォールドのバイト長を取得します
        /// 空文字を指定すると自オブジェクト全体のバイト長を取得します
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <returns>バイト長</returns>
        public int GetLength(string field_name)
        {
            if (field_name == "")
            {
                return Length;
            }

            if (dictField.ContainsKey(field_name))
            {
                BL_ObjectInformation pl = dictField[field_name];
                return pl.Length;
            }
            else if (dictClasses.ContainsKey(field_name))
            {
                int size = 0;
                foreach (BL_ObjectInformation oi in dictClasses[field_name])
                {
                    size += oi.Length;
                }

                return size;
            }
            else
            {
                throw new NotSupportedException("フィールドが存在しません[" + field_name + "]");
            }

            //return 0;
        }

        /// <summary>
        /// 指定フィールドのフィールドデータを取得します
        /// クラスメンバーに直接アクセスするのと同じ結果を返しますが、名前文字列で対象のデータが取得できます
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <returns></returns>
        public object GetData(string field_name)
        {
            if (dictField == null) throw new NotSupportedException("Initializeされていません[" + field_name + "]");

            if (field_name == "")
            {
                return this;
            }
            else if (dictField.ContainsKey(field_name))
            {
                BL_ObjectInformation pl = dictField[field_name];
                return pl.GetData();
            }
            else if (dictClasses.ContainsKey(field_name))
            {
                foreach (BL_ObjectInformation oi in dictClasses[field_name])
                {
                    return oi.Object;
                }
                return null;
            }
            else
            {
                throw new NotSupportedException("フィールドが存在しません[" + field_name + "]");
            }

            //return null;
        }
    }
}
