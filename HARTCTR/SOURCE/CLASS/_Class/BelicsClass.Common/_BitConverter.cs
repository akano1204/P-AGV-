using System;
using System.Runtime.InteropServices;

namespace BelicsClass.Common
{
    /// <summary>
    /// System.BitConverterの処理を補います。
    /// </summary>
    /// <remarks>
    /// オフセットを更新しながら値とバイト配列の変換を行いますので、送受信データフォーマットの変換
    /// が得意です。
    /// </remarks>
    public class BL_BitConverter
    {
        /// <summary>
        /// コンストラクタです。ユーザは使用できません。
        /// </summary>
        private BL_BitConverter()
        {
        }

        /// <summary>
        /// 指定されたバイト配列の指定オフセットから始まるbyteを取得します。
        /// </summary>
        /// <param name="byteArray">バイト配列を指定します。</param>
        /// <param name="offset">取得したい値が格納された偏差を指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">値を格納する参照を指定します。</param>
        public static void ToValue(byte[] byteArray, ref int offset, out byte val)
        {
            val = byteArray[offset];
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたバイト配列の指定オフセットから始まるushotを取得します。
        /// </summary>
        /// <param name="byteArray">バイト配列を指定します。</param>
        /// <param name="offset">取得したい値が格納された偏差を指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">値を格納する参照を指定します。</param>
        public static void ToValue(byte[] byteArray, ref int offset, out ushort val)
        {
            val = BitConverter.ToUInt16(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたバイト配列の指定オフセットから始まるuintを取得します。
        /// </summary>
        /// <param name="byteArray">バイト配列を指定します。</param>
        /// <param name="offset">取得したい値が格納された偏差を指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">値を格納する参照を指定します。</param>
        public static void ToValue(byte[] byteArray, ref int offset, out uint val)
        {
            val = BitConverter.ToUInt32(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたバイト配列の指定オフセットから始まるulongを取得します。
        /// </summary>
        /// <param name="byteArray">バイト配列を指定します。</param>
        /// <param name="offset">取得したい値が格納された偏差を指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">値を格納する参照を指定します。</param>
        public static void ToValue(byte[] byteArray, ref int offset, out ulong val)
        {
            val = BitConverter.ToUInt32(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたバイト配列の指定オフセットから始まるsbyteを取得します。
        /// </summary>
        /// <param name="byteArray">バイト配列を指定します。</param>
        /// <param name="offset">取得したい値が格納された偏差を指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">値を格納する参照を指定します。</param>
        public static void ToValue(byte[] byteArray, ref int offset, out sbyte val)
        {
            val = (sbyte)byteArray[offset];
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたバイト配列の指定オフセットから始まるshortを取得します。
        /// </summary>
        /// <param name="byteArray">バイト配列を指定します。</param>
        /// <param name="offset">取得したい値が格納された偏差を指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">値を格納する参照を指定します。</param>
        public static void ToValue(byte[] byteArray, ref int offset, out short val)
        {
            val = BitConverter.ToInt16(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたバイト配列の指定オフセットから始まるintを取得します。
        /// </summary>
        /// <param name="byteArray">バイト配列を指定します。</param>
        /// <param name="offset">取得したい値が格納された偏差を指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">値を格納する参照を指定します。</param>
        public static void ToValue(byte[] byteArray, ref int offset, out int val)
        {
            val = BitConverter.ToInt32(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたバイト配列の指定オフセットから始まるlongを取得します。
        /// </summary>
        /// <param name="byteArray">バイト配列を指定します。</param>
        /// <param name="offset">取得したい値が格納された偏差を指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">値を格納する参照を指定します。</param>
        public static void ToValue(byte[] byteArray, ref int offset, out long val)
        {
            val = BitConverter.ToInt64(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたバイト配列の指定オフセットから始まるcharを取得します。
        /// </summary>
        /// <param name="byteArray">バイト配列を指定します。</param>
        /// <param name="offset">取得したい値が格納された偏差を指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">値を格納する参照を指定します。</param>
        public static void ToValue(byte[] byteArray, ref int offset, out char val)
        {
            val = BitConverter.ToChar(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたバイト配列の指定オフセットから始まるfloatを取得します。
        /// </summary>
        /// <param name="byteArray">バイト配列を指定します。</param>
        /// <param name="offset">取得したい値が格納された偏差を指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">値を格納する参照を指定します。</param>
        public static void ToValue(byte[] byteArray, ref int offset, out float val)
        {
            val = BitConverter.ToSingle(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたバイト配列の指定オフセットから始まるdoubleを取得します。
        /// </summary>
        /// <param name="byteArray">バイト配列を指定します。</param>
        /// <param name="offset">取得したい値が格納された偏差を指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">値を格納する参照を指定します。</param>
        public static void ToValue(byte[] byteArray, ref int offset, out double val)
        {
            val = BitConverter.ToDouble(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }



        /// <summary>
        /// 指定されたbyte値をバイト配列に変換し、指定された位置に書き込みます。
        /// </summary>
        /// <param name="byteArray">変換した値型を格納するバイト配列を指定します。</param>
        /// <param name="offset">格納するオフセットを指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">バイト配列に変換する値を指定します。</param>
        public static void ToBytes(byte[] byteArray, ref int offset, byte val)
        {
            byteArray[offset] = val;
            offset += Marshal.SizeOf(val);
        }


        /// <summary>
        /// 指定されたushort値をバイト配列に変換し、指定された位置に書き込みます。
        /// </summary>
        /// <param name="byteArray">変換した値型を格納するバイト配列を指定します。</param>
        /// <param name="offset">格納するオフセットを指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">バイト配列に変換する値を指定します。</param>
        public static void ToBytes(byte[] byteArray, ref int offset, ushort val)
        {
            BitConverter.GetBytes(val).CopyTo(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたuint値をバイト配列に変換し、指定された位置に書き込みます。
        /// </summary>
        /// <param name="byteArray">変換した値型を格納するバイト配列を指定します。</param>
        /// <param name="offset">格納するオフセットを指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">バイト配列に変換する値を指定します。</param>
        public static void ToBytes(byte[] byteArray, ref int offset, uint val)
        {
            BitConverter.GetBytes(val).CopyTo(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたulong値をバイト配列に変換し、指定された位置に書き込みます。
        /// </summary>
        /// <param name="byteArray">変換した値型を格納するバイト配列を指定します。</param>
        /// <param name="offset">格納するオフセットを指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">バイト配列に変換する値を指定します。</param>
        public static void ToBytes(byte[] byteArray, ref int offset, ulong val)
        {
            BitConverter.GetBytes(val).CopyTo(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたsbyte値をバイト配列に変換し、指定された位置に書き込みます。
        /// </summary>
        /// <param name="byteArray">変換した値型を格納するバイト配列を指定します。</param>
        /// <param name="offset">格納するオフセットを指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">バイト配列に変換する値を指定します。</param>
        public static void ToBytes(byte[] byteArray, ref int offset, sbyte val)
        {
            byteArray[offset] = (byte)val;
            offset += Marshal.SizeOf(val);
        }


        /// <summary>
        /// 指定されたshort値をバイト配列に変換し、指定された位置に書き込みます。
        /// </summary>
        /// <param name="byteArray">変換した値型を格納するバイト配列を指定します。</param>
        /// <param name="offset">格納するオフセットを指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">バイト配列に変換する値を指定します。</param>
        public static void ToBytes(byte[] byteArray, ref int offset, short val)
        {
            BitConverter.GetBytes(val).CopyTo(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたint値をバイト配列に変換し、指定された位置に書き込みます。
        /// </summary>
        /// <param name="byteArray">変換した値型を格納するバイト配列を指定します。</param>
        /// <param name="offset">格納するオフセットを指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">バイト配列に変換する値を指定します。</param>
        public static void ToBytes(byte[] byteArray, ref int offset, int val)
        {
            BitConverter.GetBytes(val).CopyTo(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたlong値をバイト配列に変換し、指定された位置に書き込みます。
        /// </summary>
        /// <param name="byteArray">変換した値型を格納するバイト配列を指定します。</param>
        /// <param name="offset">格納するオフセットを指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">バイト配列に変換する値を指定します。</param>
        public static void ToBytes(byte[] byteArray, ref int offset, long val)
        {
            BitConverter.GetBytes(val).CopyTo(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたchar値をバイト配列に変換し、指定された位置に書き込みます。
        /// </summary>
        /// <param name="byteArray">変換した値型を格納するバイト配列を指定します。</param>
        /// <param name="offset">格納するオフセットを指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">バイト配列に変換する値を指定します。</param>
        public static void ToBytes(byte[] byteArray, ref int offset, char val)
        {
            BitConverter.GetBytes(val).CopyTo(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたfloat値をバイト配列に変換し、指定された位置に書き込みます。
        /// </summary>
        /// <param name="byteArray">変換した値型を格納するバイト配列を指定します。</param>
        /// <param name="offset">格納するオフセットを指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">バイト配列に変換する値を指定します。</param>
        public static void ToBytes(byte[] byteArray, ref int offset, float val)
        {
            BitConverter.GetBytes(val).CopyTo(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }

        /// <summary>
        /// 指定されたdouble値をバイト配列に変換し、指定された位置に書き込みます。
        /// </summary>
        /// <param name="byteArray">変換した値型を格納するバイト配列を指定します。</param>
        /// <param name="offset">格納するオフセットを指定します。値型のサイズ(バイト)分、更新されます。</param>
        /// <param name="val">バイト配列に変換する値を指定します。</param>
        public static void ToBytes(byte[] byteArray, ref int offset, double val)
        {
            BitConverter.GetBytes(val).CopyTo(byteArray, offset);
            offset += Marshal.SizeOf(val);
        }


        /// <summary>
        /// 文字列をbyte型に変換します。
        /// 変換できなかった場合、destには何も設定されず、例外を放出しません
        /// </summary>
        /// <param name="str">変換する文字列を指定します。</param>
        /// <param name="dest">変換後に代入する参照を指定します。</param>
        /// <returns>変換に成功した場合、trueを返します。変換に失敗した場合、falseを返します。</returns>
        public static bool Parse(string str, ref byte dest)
        {
            try
            {
                dest = byte.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 文字列をushort型に変換します。
        /// 変換できなかった場合、destには何も設定されず、例外を放出しません
        /// </summary>
        /// <param name="str">変換する文字列を指定します。</param>
        /// <param name="dest">変換後に代入する参照を指定します。</param>
        /// <returns>変換に成功した場合、trueを返します。変換に失敗した場合、falseを返します。</returns>
        public static bool Parse(string str, ref ushort dest)
        {
            try
            {
                dest = ushort.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 文字列をulong型に変換します。
        /// 変換できなかった場合、destには何も設定されず、例外を放出しません
        /// </summary>
        /// <param name="str">変換する文字列を指定します。</param>
        /// <param name="dest">変換後に代入する参照を指定します。</param>
        /// <returns>変換に成功した場合、trueを返します。変換に失敗した場合、falseを返します。</returns>
        public static bool Parse(string str, ref ulong dest)
        {
            try
            {
                dest = ulong.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 文字列をuint型に変換します。
        /// 変換できなかった場合、destには何も設定されず、例外を放出しません
        /// </summary>
        /// <param name="str">変換する文字列を指定します。</param>
        /// <param name="dest">変換後に代入する参照を指定します。</param>
        /// <returns>変換に成功した場合trueを返します。変換に失敗した場合falseを返します。</returns>
        public static bool Parse(string str, ref uint dest)
        {
            try
            {
                dest = uint.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 文字列をsbyte型に変換します。
        /// 変換できなかった場合、destには何も設定されず、例外を放出しません
        /// </summary>
        /// <param name="str">変換する文字列を指定します。</param>
        /// <param name="dest">変換後に代入する参照を指定します。</param>
        /// <returns>変換に成功した場合、trueを返します。変換に失敗した場合、falseを返します。</returns>
        public static bool Parse(string str, ref sbyte dest)
        {
            try
            {
                dest = sbyte.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 文字列をshort型に変換します。
        /// 変換できなかった場合、destには何も設定されず、例外を放出しません
        /// </summary>
        /// <param name="str">変換する文字列を指定します。</param>
        /// <param name="dest">変換後に代入する参照を指定します。</param>
        /// <returns>変換に成功した場合、trueを返します。変換に失敗した場合、falseを返します。</returns>
        public static bool Parse(string str, ref short dest)
        {
            try
            {
                dest = short.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 文字列をlong型に変換します。
        /// 変換できなかった場合、destには何も設定されず、例外を放出しません
        /// </summary>
        /// <param name="str">変換する文字列を指定します。</param>
        /// <param name="dest">変換後に代入する参照を指定します。</param>
        /// <returns>変換に成功した場合、trueを返します。変換に失敗した場合、falseを返します。</returns>
        public static bool Parse(string str, ref long dest)
        {
            try
            {
                dest = long.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 文字列をint型に変換します。
        /// 変換できなかった場合、destには何も設定されず、例外を放出しません
        /// </summary>
        /// <param name="str">変換する文字列を指定します。</param>
        /// <param name="dest">変換後に代入する参照を指定します。</param>
        /// <returns>変換に成功した場合、trueを返します。変換に失敗した場合、falseを返します。</returns>
        public static bool Parse(string str, ref int dest)
        {
            try
            {
                dest = int.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }




        /// <summary>
        /// 文字列をfloat型に変換します。
        /// 変換できなかった場合、destには何も設定されず、例外を放出しません
        /// </summary>
        /// <param name="str">変換する文字列を指定します。</param>
        /// <param name="dest">変換後に代入する参照を指定します。</param>
        /// <returns>変換に成功した場合、trueを返します。変換に失敗した場合、falseを返します。</returns>
        public static bool Parse(string str, ref float dest)
        {
            try
            {
                dest = float.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 文字列をdouble型に変換します。
        /// 変換できなかった場合、destには何も設定されず、例外を放出しません
        /// </summary>
        /// <param name="str">変換する文字列を指定します。</param>
        /// <param name="dest">変換後に代入する参照を指定します。</param>
        /// <returns>変換に成功した場合、trueを返します。変換に失敗した場合、falseを返します。</returns>
        public static bool Parse(string str, ref double dest)
        {
            try
            {
                dest = double.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 文字列をdecimal型に変換します。
        /// 変換できなかった場合、destには何も設定されず、例外を放出しません
        /// </summary>
        /// <param name="str">変換する文字列を指定します。</param>
        /// <param name="dest">変換後に代入する参照を指定します。</param>
        /// <returns>変換に成功した場合、trueを返します。変換に失敗した場合、falseを返します。</returns>
        public static bool Parse(string str, ref decimal dest)
        {
            try
            {
                dest = decimal.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 2バイトの配列をdouble型に変換
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="offset"></param>
        /// <param name="decimalBit"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public static bool Fixed2ToDouble(byte[] byteArray, uint offset, int decimalBit, out double outValue)
        {
            //バイナリデータ不足
            if (byteArray.Length < offset + 2)
            {
                outValue = 0.0;
                return false;
            }

            //エンディアン変換
            byte[] wkArray = new byte[2];
            wkArray[1] = byteArray[offset];
            wkArray[0] = byteArray[offset + 1];

            //小数点変換
            outValue = System.BitConverter.ToInt16(wkArray, 0) / Math.Pow(2, decimalBit);

            return true;
        }

        /// <summary>
        /// 4バイトのデータをdouble型に変換
        /// </summary>
        /// <param name="byteArray"></param>
        /// <param name="offset"></param>
        /// <param name="decimalBit"></param>
        /// <param name="outValue"></param>
        /// <returns></returns>
        public static bool Fixed4ToDouble(byte[] byteArray, uint offset, int decimalBit, out double outValue)
        {
            //バイナリデータ不足
            if (byteArray.Length < offset + 4)
            {
                outValue = 0.0;
                return false;
            }

            //エンディアン変換
            byte[] wkArray = new byte[4];
            wkArray[3] = byteArray[offset];
            wkArray[2] = byteArray[offset + 1];
            wkArray[1] = byteArray[offset + 2];
            wkArray[0] = byteArray[offset + 3];

            //小数点変換
            outValue = System.BitConverter.ToInt32(wkArray, 0) / Math.Pow(2, decimalBit);

            return true;
        }

        /// <summary>指定された値のエンディアンを逆転します。</summary>
        /// <param name="val">エンディアンを逆転する値を指定します。</param>
        /// <returns>逆転した値を返します。</returns>
        public static byte HighLow(byte val)
        {
            return val;
        }

        /// <summary>指定された値のエンディアンを逆転します。</summary>
        /// <param name="val">エンディアンを逆転する値を指定します。</param>
        /// <returns>逆転した値を返します。</returns>
        public static ushort HighLow(ushort val)
        {
            s_Offset = 0;
            ToBytes(s_WorkArray, ref s_Offset, val);

            FlipFrop(ref s_WorkArray[0], ref s_WorkArray[1]);

            s_Offset = 0;
            ToValue(s_WorkArray, ref s_Offset, out val);
            return val;
        }

        /// <summary>指定された値のエンディアンを逆転します。</summary>
        /// <param name="val">エンディアンを逆転する値を指定します。</param>
        /// <returns>逆転した値を返します。</returns>
        public static uint HighLow(uint val)
        {
            s_Offset = 0;
            ToBytes(s_WorkArray, ref s_Offset, val);

            FlipFrop(ref s_WorkArray[0], ref s_WorkArray[3]);
            FlipFrop(ref s_WorkArray[1], ref s_WorkArray[2]);

            s_Offset = 0;
            ToValue(s_WorkArray, ref s_Offset, out val);
            return val;
        }

        /// <summary>指定された値のエンディアンを逆転します。</summary>
        /// <param name="val">エンディアンを逆転する値を指定します。</param>
        /// <returns>逆転した値を返します。</returns>
        public static ulong HighLow(ulong val)
        {
            s_Offset = 0;
            ToBytes(s_WorkArray, ref s_Offset, val);

            FlipFrop(ref s_WorkArray[0], ref s_WorkArray[7]);
            FlipFrop(ref s_WorkArray[1], ref s_WorkArray[6]);
            FlipFrop(ref s_WorkArray[2], ref s_WorkArray[5]);
            FlipFrop(ref s_WorkArray[3], ref s_WorkArray[4]);

            s_Offset = 0;
            ToValue(s_WorkArray, ref s_Offset, out val);
            return val;
        }

        /// <summary>指定された値のエンディアンを逆転します。</summary>
        /// <param name="val">エンディアンを逆転する値を指定します。</param>
        /// <returns>逆転した値を返します。</returns>
        public static sbyte HighLow(sbyte val)
        {
            return val;
        }

        /// <summary>指定された値のエンディアンを逆転します。</summary>
        /// <param name="val">エンディアンを逆転する値を指定します。</param>
        /// <returns>逆転した値を返します。</returns>
        public static short HighLow(short val)
        {
            s_Offset = 0;
            ToBytes(s_WorkArray, ref s_Offset, val);

            FlipFrop(ref s_WorkArray[0], ref s_WorkArray[1]);

            s_Offset = 0;
            ToValue(s_WorkArray, ref s_Offset, out val);
            return val;
        }

        /// <summary>指定された値のエンディアンを逆転します。</summary>
        /// <param name="val">エンディアンを逆転する値を指定します。</param>
        /// <returns>逆転した値を返します。</returns>
        public static int HighLow(int val)
        {
            s_Offset = 0;
            ToBytes(s_WorkArray, ref s_Offset, val);

            FlipFrop(ref s_WorkArray[0], ref s_WorkArray[3]);
            FlipFrop(ref s_WorkArray[1], ref s_WorkArray[2]);

            s_Offset = 0;
            ToValue(s_WorkArray, ref s_Offset, out val);
            return val;
        }

        /// <summary>指定された値のエンディアンを逆転します。</summary>
        /// <param name="val">エンディアンを逆転する値を指定します。</param>
        /// <returns>逆転した値を返します。</returns>
        public static long HighLow(long val)
        {
            s_Offset = 0;
            ToBytes(s_WorkArray, ref s_Offset, val);

            FlipFrop(ref s_WorkArray[0], ref s_WorkArray[7]);
            FlipFrop(ref s_WorkArray[1], ref s_WorkArray[6]);
            FlipFrop(ref s_WorkArray[2], ref s_WorkArray[5]);
            FlipFrop(ref s_WorkArray[3], ref s_WorkArray[4]);

            s_Offset = 0;
            ToValue(s_WorkArray, ref s_Offset, out val);
            return val;
        }

        /// <summary>値を入れ替えます。</summary>
        /// <param name="a">入れ替える値を指定します。</param>
        /// <param name="b">入れ替える値を指定します。</param>
        public static void FlipFrop(ref byte a, ref byte b)
        {
            byte back = a;
            a = b;
            b = back;
        }

        /// <summary>
        /// バイナリデータをＢＣＤデータに変換します。
        /// データが９９を超えるときはＢＣＤ表現できないため例外をスローします。
        /// </summary>
        /// <param name="val">ＢＣＤデータに変換するバイトデータを指定します。</param>
        /// <returns>ＢＣＤ変換後のバイトデータを返します。</returns>
        public static byte BinToBcd(byte val)
        {
            //ＢＣＤ変換できない値
            if (val > 99) throw new Exception("ＢＣＤデータに変換できない値です。" + val.ToString());

            //ＢＣＤデータに変換
            return (byte)((val / 10) * 0x10 + val % 10);
        }

        /// <summary>
        /// バイナリデータをＢＣＤデータに変換します。
        /// データが９９を超えるときはＢＣＤ表現できないため例外をスローします。
        /// </summary>
        /// <param name="val">ＢＣＤデータに変換するバイト配列を指定します。</param>
        /// <returns>ＢＣＤ変換後のバイト配列を返します。</returns>
        public static byte[] BinToBcd(byte[] val)
        {
            byte[] rcByte = new byte[val.Length];

            for (int cntByte = 0; cntByte < val.Length; ++cntByte)
            {
                //ＢＣＤデータに変換
                rcByte[cntByte] = BinToBcd(val[cntByte]);
            }
            return rcByte;
        }

        /// <summary>
        /// ＢＣＤデータをバイナリデータに変換します。
        /// 指定データがＢＣＤデータであることのチェックは行いません。
        /// </summary>
        /// <param name="val">バイナリデータに変換するバイトデータを指定します。</param>
        /// <returns>バイナリ変換後のバイトデータを返します。</returns>
        public static byte BcdToBin(byte val)
        {
            return (byte)((val / 0x10) * 10 + val % 0x10);
        }

        /// <summary>
        /// ＢＣＤデータをバイナリデータに変換します。
        /// 指定データがＢＣＤデータであることのチェックは行いません。
        /// </summary>
        /// <param name="val">バイナリデータに変換するバイト配列を渡します。</param>
        /// <returns>バイナリ変換後のバイト配列を返します。</returns>
        public static byte[] BcdToBin(byte[] val)
        {
            byte[] rcByte = new byte[val.Length];

            for (int cntByte = 0; cntByte < val.Length; ++cntByte)
            {
                //バイナリデータに変換
                rcByte[cntByte] = BcdToBin(val[cntByte]);
            }
            return rcByte;
        }

        /// <summary>16進文字列("0x"付き)を数値に変換します。</summary>
        /// <param name="val">変換する文字列を指定します。</param>
        /// <returns>変換後のデータを返します。</returns>
        public static int HexToInt(string val)
        {
            int idx = val.IndexOf("0x");

            if (idx == 0)
            {
                val = val.Substring(idx + 2, val.Length - 2);
                return int.Parse(val, System.Globalization.NumberStyles.HexNumber);
            }
            return int.Parse(val);
        }

        /// <summary>エンディアン逆転用のワークです。</summary>
        private static byte[] s_WorkArray = new byte[Marshal.SizeOf(typeof(decimal))];

        /// <summary>エンディアン逆転用のワークです。</summary>
        private static int s_Offset;
    }
}
