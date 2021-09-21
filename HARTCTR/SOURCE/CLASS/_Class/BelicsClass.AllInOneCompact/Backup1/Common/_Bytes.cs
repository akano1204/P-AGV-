using System;
using System.Text;
using System.Runtime.InteropServices;

namespace BelicsClass.Common
{
    /// <summary>
    /// 拡張バイト配列クラス
    /// </summary>
    public class BL_Bytes
    {
        //====================================================================================================
        // バイト配列の初期化
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	byte			sourceByte			初期化に使用するﾊﾞｲﾄ値
        //	int				length      		初期化するﾊﾞｲﾄ数
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	byte[]			destinationBytes	初期化するﾊﾞｲﾄ配列
        //	int				destinationIndex	初期化するﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	なし
        //
        //====================================================================================================
        /// <summary>
        /// バイト配列を初期化します。
        /// </summary>
        /// <param name="sourceByte">初期化に使用するバイト値。</param>
        /// <param name="destinationBytes">初期化するバイト配列。</param>
        /// <param name="length">初期化するバイト数。</param>
        public static void Clear(byte sourceByte, byte[] destinationBytes, int length)
        {
            clear(sourceByte, destinationBytes, 0, length);
        }
        /// <summary>
        /// バイト配列を初期化します。
        /// </summary>
        /// <param name="sourceByte">初期化に使用するバイト値。</param>
        /// <param name="destinationBytes">初期化するバイト配列。</param>
        /// <param name="destinationIndex">destinationBytes内の初期化を開始するインデックス。</param>
        /// <param name="length">初期化するバイト数。</param>
        public static void Clear(byte sourceByte, byte[] destinationBytes, int destinationIndex, int length)
        {
            clear(sourceByte, destinationBytes, destinationIndex, length);
        }
        private static void clear(byte data1, byte[] data2, int index, int length)
        {
            int count;

            for (count = 0; count < length; count++)
            {
                data2[index + count] = data1;
            }
        }

        //====================================================================================================
        // バイト配列のコピー
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	byte[]			sourceBytes			ｺﾋﾟｰ元のﾊﾞｲﾄ配列
        //	int				sourceIndex			ｺﾋﾟｰ元のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
        //	int				length      		ｺﾋﾟｰするﾊﾞｲﾄ数
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	byte[]			destinationBytes	ｺﾋﾟｰ先のﾊﾞｲﾄ配列
        //	int				destinationIndex	ｺﾋﾟｰ先のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	なし
        //
        //====================================================================================================
        /// <summary>
        /// バイト配列をコピーします。
        /// </summary>
        /// <param name="sourceBytes">コピー元のバイト配列。</param>
        /// <param name="destinationBytes">コピー先のバイト配列。</param>
        /// <param name="length">コピーするバイト数。</param>
        public static void Copy(byte[] sourceBytes, byte[] destinationBytes, int length)
        {
            copy(sourceBytes, 0, destinationBytes, 0, length);
        }
        /// <summary>
        /// バイト配列をコピーします。
        /// </summary>
        /// <param name="sourceBytes">コピー元のバイト配列。</param>
        /// <param name="sourceIndex">sourceBytes内のコピーを開始するインデックス。</param>
        /// <param name="destinationBytes">コピー先のバイト配列。</param>
        /// <param name="destinationIndex">destinationBytes内のコピーを開始するインデックス。</param>
        /// <param name="length">コピーするバイト数。</param>
        public static void Copy(byte[] sourceBytes, int sourceIndex, byte[] destinationBytes, int destinationIndex, int length)
        {
            copy(sourceBytes, sourceIndex, destinationBytes, destinationIndex, length);
        }
        private static void copy(byte[] data1, int index1, byte[] data2, int index2, int length)
        {
            Array.Copy(data1, index1, data2, index2, length);
        }

        //====================================================================================================
        // バイト配列の比較
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	byte[]			sourceBytes			比較元のﾊﾞｲﾄ配列
        //	int				sourceIndex			比較元のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
        //	byte[]			destinationBytes	比較先のﾊﾞｲﾄ配列
        //	int				destinationIndex	比較先のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
        //	int				length      		比較するﾊﾞｲﾄ数
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	int				0					sourceBytes = destinationBytes
        //					1					sourceBytes > destinationBytes
        //					-1					sourceBytes < destinationBytes
        //
        //====================================================================================================
        /// <summary>
        /// バイト配列を比較します。
        /// </summary>
        /// <param name="sourceBytes">比較元のバイト配列。</param>
        /// <param name="destinationBytes">比較先のバイト配列。</param>
        /// <param name="length">比較するバイト数。</param>
        public static int Compare(byte[] sourceBytes, byte[] destinationBytes, int length)
        {
            return compare(sourceBytes, 0, destinationBytes, 0, length);
        }
        /// <summary>
        /// バイト配列を比較します。
        /// </summary>
        /// <param name="sourceBytes">比較元のバイト配列。</param>
        /// <param name="sourceIndex">sourceBytes内の比較を開始するインデックス。</param>
        /// <param name="destinationBytes">比較先のバイト配列。</param>
        /// <param name="destinationIndex">destinationBytes内の比較を開始するインデックス。</param>
        /// <param name="length">比較するバイト数。</param>
        public static int Compare(byte[] sourceBytes, int sourceIndex, byte[] destinationBytes, int destinationIndex, int length)
        {
            return compare(sourceBytes, sourceIndex, destinationBytes, destinationIndex, length);
        }
        private static int compare(byte[] data1, int index1, byte[] data2, int index2, int length)
        {
            int status = 0, count;

            for (count = 0; count < length; count++)
            {
                if (data1[index1 + count] > data2[index2 + count])
                {
                    status = 1;
                    break;
                }
                else if (data1[index1 + count] < data2[index2 + count])
                {
                    status = -1;
                    break;
                }
            }

            return status;
        }

        //====================================================================================================
        // バイト配列の結合
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	byte[]			sourceBytes			結合元のﾊﾞｲﾄ配列
        //	int				sourceIndex			結合元のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
        //	int				length      		結合するﾊﾞｲﾄ数
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	ref byte[]		destinationBytes	結合先のﾊﾞｲﾄ配列
        //	int				destinationIndex	結合先のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	なし
        //
        //====================================================================================================
        /// <summary>
        /// バイト配列を結合します。
        /// </summary>
        /// <param name="sourceBytes">結合元のバイト配列。</param>
        /// <param name="destinationBytes">結合先のバイト配列。</param>
        /// <param name="length">結合するバイト数。</param>
        public static void Join(byte[] sourceBytes, ref byte[] destinationBytes, int length)
        {
            join(sourceBytes, 0, ref destinationBytes, destinationBytes.Length, length);
        }
        /// <summary>
        /// バイト配列を結合します。
        /// </summary>
        /// <param name="sourceBytes">結合元のバイト配列。</param>
        /// <param name="sourceIndex">sourceBytes内の結合を開始するインデックス。</param>
        /// <param name="destinationBytes">結合先のバイト配列。</param>
        /// <param name="destinationIndex">destinationBytes内の結合を開始するインデックス。</param>
        /// <param name="length">結合するバイト数。</param>
        public static void Join(byte[] sourceBytes, int sourceIndex, ref byte[] destinationBytes, int destinationIndex, int length)
        {
            join(sourceBytes, sourceIndex, ref destinationBytes, destinationIndex, length);
        }
        private static void join(byte[] data1, int index1, ref byte[] data2, int index2, int length)
        {
            byte[] data;

            data = new byte[index2 + length];

            Array.Copy(data2, 0, data, 0, index2);
            Array.Copy(data1, index1, data, index2, length);

            data2 = data;
        }

        //====================================================================================================
        // バイト配列の調整
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	byte[]			sourceBytes			調整元のﾊﾞｲﾄ配列
        //	int				sourceIndex			調整元のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
        //	int				length      		調整するﾊﾞｲﾄ数
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	byte[]								調整したﾊﾞｲﾄ配列
        //
        //====================================================================================================
        /// <summary>
        /// バイト配列を調整します。
        /// </summary>
        /// <param name="sourceBytes">調整元のバイト配列。</param>
        /// <param name="length">調整するバイト数。</param>
        public static byte[] Trim(byte[] sourceBytes, int length)
        {
            return trim(sourceBytes, 0, length);
        }
        /// <summary>
        /// バイト配列を調整します。
        /// </summary>
        /// <param name="sourceBytes">調整元のバイト配列。</param>
        /// <param name="sourceIndex">sourceBytes内の調整を開始するインデックス。</param>
        /// <param name="length">調整するバイト数。</param>
        public static byte[] Trim(byte[] sourceBytes, int sourceIndex, int length)
        {
            return trim(sourceBytes, sourceIndex, length);
        }
        private static byte[] trim(byte[] data1, int index1, int length)
        {
            byte[] data;

            data = new byte[length];

            Array.Copy(data1, index1, data, 0, length);

            return data;
        }

        //====================================================================================================
        // バイト配列へ変換
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	string			sourceString		変換元の文字列
        //	int				length      		変換後のﾊﾞｲﾄ数
        //	byte			sourceByte			埋め込みに使用するﾊﾞｲﾄ値
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	byte[]								変換したﾊﾞｲﾄ配列
        //
        //====================================================================================================
        /// <summary>
        /// バイト配列に変換して左寄せします。
        /// </summary>
        /// <param name="sourceString">調整元の文字列。</param>
        /// <param name="length">変換後のバイト数。</param>
        public static byte[] PadRight(string sourceString, int length)
        {
            return pad(1, sourceString, length, (byte)' ');
        }
        /// <summary>
        /// バイト配列に変換して左寄せします。
        /// </summary>
        /// <param name="sourceString">変換元の文字列。</param>
        /// <param name="length">変換後のバイト数。</param>
        /// <param name="sourceByte">埋め込みに使用するバイト値。</param>
        public static byte[] PadRight(string sourceString, int length, byte sourceByte)
        {
            return pad(1, sourceString, length, sourceByte);
        }
        /// <summary>
        /// バイト配列に変換して右寄せします。
        /// </summary>
        /// <param name="sourceString">調整元の文字列。</param>
        /// <param name="length">変換後のバイト数。</param>
        public static byte[] PadLeft(string sourceString, int length)
        {
            return pad(2, sourceString, length, (byte)' ');
        }
        /// <summary>
        /// バイト配列に変換して右寄せします。
        /// </summary>
        /// <param name="sourceString">変換元の文字列。</param>
        /// <param name="length">変換後のバイト数。</param>
        /// <param name="sourceByte">埋め込みに使用するバイト値。</param>
        public static byte[] PadLeft(string sourceString, int length, byte sourceByte)
        {
            return pad(2, sourceString, length, sourceByte);
        }
        private static byte[] pad(int mode, string data1, int length, byte data2)
        {
            int size;
            byte[] data, temp;

            data = new byte[length];

            clear(data2, data, 0, length);

            temp = Encoding.Default.GetBytes(data1);
            size = temp.Length < length ? temp.Length : length;

            if (mode == 1)
            {
                Array.Copy(temp, 0, data, 0, size);
            }
            else
            {
                Array.Copy(temp, 0, data, length - size, size);
            }

            return data;
        }

        //====================================================================================================
        // バイト配列からバイト配列を検索
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	byte[]			sourceBytes			検索元のﾊﾞｲﾄ配列
        //	int				sourceIndex			検索元のﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
        //	byte[]			targetBytes			検索するﾊﾞｲﾄ配列
        //	int				destinationIndex	検索するﾊﾞｲﾄ配列の開始ｲﾝﾃﾞｯｸｽ
        //	int				length      		検索するﾊﾞｲﾄ数
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	int				>=0					見つけた先頭のインデックス
        //					-1					見つからない
        //
        //====================================================================================================
        /// <summary>
        /// バイト配列からバイト配列を検索します。
        /// </summary>
        /// <param name="sourceBytes">検索対象となるバイト配列。</param>
        /// <param name="sourceIndex">sourceBytes内の検索を開始するインデックス。</param>
        /// <param name="targetBytes">検索するバイト配列。</param>
        /// <param name="targetIndex">targetBytes内の検索を開始するインデックス。</param>
        /// <param name="length">検索するバイト数。</param>
        /// <returns></returns>
        public static int IndexOf(byte[] sourceBytes, int sourceIndex, byte[] targetBytes, int targetIndex, int length)
        {
            int pos = -1;
            int pos1, pos2;

            if (length <= 0) return pos;
            if (sourceIndex < 0 || targetIndex < 0) return pos;
            if (sourceIndex >= sourceBytes.Length) return pos;
            if (targetIndex >= targetBytes.Length) return pos;
            if (targetIndex + length > targetBytes.Length) return pos;

            for (pos1 = sourceIndex; pos1 < sourceBytes.Length; pos1++)
            {
                if (sourceBytes.Length - pos1 < length) break;

                for (pos2 = 0; pos2 < length; pos2++)
                {
                    if (sourceBytes[pos1 + pos2] == targetBytes[targetIndex + pos2]) continue;
                    else break;
                }

                if (pos2 == length)
                {
                    pos = pos1;
                    break;
                }
            }

            return pos;
        }

        //====================================================================================================
        // 型変換(byte[4] -> Int32)
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	byte[]			data				変換元ﾃﾞｰﾀ
        //	int				Index				変換元ﾃﾞｰﾀの開始ｲﾝﾃﾞｯｸｽ
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	int									結果
        //
        //====================================================================================================
        /// <summary>
        /// byte[4]をInt32へ変換
        /// </summary>
        /// <param name="data">変換するバイト配列。</param>
        public static int BytesToInt32(byte[] data)
        {
            return bytes_to_int32(data, 0);
        }
        /// <summary>
        /// byte[4]をInt32へ変換
        /// </summary>
        /// <param name="data">変換するバイト配列。</param>
        /// <param name="Index">data内の変換を開始するインデックス。</param>
        public static int BytesToInt32(byte[] data, int Index)
        {
            return bytes_to_int32(data, Index);
        }
        private static int bytes_to_int32(byte[] data, int index)
        {
            int count = 0;

            count += data[index];
            count += data[index + 1] << 8;
            count += data[index + 2] << 16;
            count += data[index + 3] << 24;

            return count;
        }

        //====================================================================================================
        // 型変換(Int32 -> byte[4])
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	int									変換元ﾃﾞｰﾀ
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	byte[]			data				結果
        //
        //====================================================================================================
        /// <summary>
        /// Int32をbyte[4]へ変換
        /// </summary>
        /// <param name="data">変換する値。</param>
        public static byte[] Int32ToBytes(int data)
        {
            byte[] buff = new byte[4];

            buff[0] = (byte)(data & 0xFF);
            buff[1] = (byte)((data >> 8) & 0xFF);
            buff[2] = (byte)((data >> 16) & 0xFF);
            buff[3] = (byte)((data >> 24) & 0xFF);

            return buff;
        }

        //====================================================================================================
        // 型変換(byte[2] -> Int16)
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	byte[]			data				変換元ﾃﾞｰﾀ
        //	int				Index				変換元ﾃﾞｰﾀの開始ｲﾝﾃﾞｯｸｽ
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	short								結果
        //
        //====================================================================================================
        /// <summary>
        /// byte[2]をInt16へ変換
        /// </summary>
        /// <param name="data"></param>
        public static short BytesToInt16(byte[] data)
        {
            return bytes_to_int16(data, 0);
        }
        /// <summary>
        /// byte[2]をInt16へ変換
        /// </summary>
        /// <param name="data">変換するバイト配列。</param>
        /// <param name="Index">data内の変換を開始するインデックス。</param>
        public static short BytesToInt16(byte[] data, int Index)
        {
            return bytes_to_int16(data, Index);
        }
        private static short bytes_to_int16(byte[] data, int index)
        {
            short count = 0;

            count += data[index];
            count += (short)(data[index + 1] << 8);

            return count;
        }

        //====================================================================================================
        // 型変換(Int16 -> byte[2])
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	short								変換元ﾃﾞｰﾀ
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	byte[]			data				結果
        //
        //====================================================================================================
        /// <summary>
        /// Int16をbyte[2]へ変換
        /// </summary>
        /// <param name="data">変換する値。</param>
        public static byte[] Int16ToBytes(short data)
        {
            byte[] buff = new byte[2];

            buff[0] = (byte)(data & 0xFF);
            buff[1] = (byte)((data >> 8) & 0xFF);

            return buff;
        }

        /// <summary>
        /// バイト配列の連続変換クラス
        /// </summary>
        public class Target
        {
            private int index = 0;
            private byte[] data;

            //====================================================================================================
            // コンストラクタ
            //
            //- INPUT --------------------------------------------------------------------------------------------
            //	ref byte[]		base_data			対象のﾊﾞｲﾄ配列
            //
            //- OUTPUT -------------------------------------------------------------------------------------------
            //	なし
            //
            //- RETURN -------------------------------------------------------------------------------------------
            //	なし
            //
            //====================================================================================================
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="base_data">データを格納する(している)バイト配列。</param>
            public Target(ref byte[] base_data)
            {
                index = 0;
                data = base_data;
            }

            //====================================================================================================
            // バイト配列へ追加
            //
            //- INPUT --------------------------------------------------------------------------------------------
            //	object			source				追加するﾃﾞｰﾀ
            //	int				size				追加するﾊﾞｲﾄ数
            //
            //- OUTPUT -------------------------------------------------------------------------------------------
            //	なし
            //
            //- RETURN -------------------------------------------------------------------------------------------
            //	なし
            //
            //====================================================================================================
            /// <summary>
            /// バイト配列へデータを追加します。
            /// </summary>
            /// <param name="source">追加するデータが格納されている文字列。</param>
            /// <param name="size">追加するバイト数。</param>
            public void Add(string source, int size)
            {
                byte[] temp;

                if (source != null)
                {
                    temp = Encoding.Default.GetBytes(source);

                    BL_Bytes.Copy(temp, 0, data, index, temp.Length < size ? temp.Length : size);
                }

                index += size;
            }
            /// <summary>
            /// バイト配列へデータを追加します。
            /// </summary>
            /// <param name="source">追加するデータが格納されているバイト配列。</param>
            /// <param name="size">追加するバイト数。</param>
            public void Add(byte[] source, int size)
            {
                if (source != null)
                {
                    BL_Bytes.Copy(source, 0, data, index, source.Length < size ? source.Length : size);
                }

                index += size;
            }
            /// <summary>
            /// バイト配列へデータを追加します。
            /// </summary>
            /// <param name="source">追加するデータが格納されているInt32。</param>
            public void Add(int source)
            {
                byte[] temp;

                temp = BL_Bytes.Int32ToBytes(source);

                BL_Bytes.Copy(temp, 0, data, index, Marshal.SizeOf(source));

                index += Marshal.SizeOf(source);
            }
            /// <summary>
            /// バイト配列へデータを追加します。
            /// </summary>
            /// <param name="source">追加するデータが格納されているInt16。</param>
            public void Add(short source)
            {
                byte[] temp;

                temp = BL_Bytes.Int16ToBytes(source);

                BL_Bytes.Copy(temp, 0, data, index, Marshal.SizeOf(source));

                index += Marshal.SizeOf(source);
            }

            //====================================================================================================
            // バイト配列から抽出
            //
            //- INPUT --------------------------------------------------------------------------------------------
            //	int				size				抽出するﾊﾞｲﾄ数
            //
            //- OUTPUT -------------------------------------------------------------------------------------------
            //	ref object		source				抽出したﾃﾞｰﾀ
            //
            //- RETURN -------------------------------------------------------------------------------------------
            //	なし
            //
            //====================================================================================================
            /// <summary>
            /// バイト配列からデータを抽出します。
            /// </summary>
            /// <param name="source">抽出したデータを格納する文字列。</param>
            /// <param name="size">抽出するバイト数。</param>
            public void Split(ref string source, int size)
            {
                source = Encoding.Default.GetString(data, index, size);

                index += size;
            }
            /// <summary>
            /// バイト配列からデータを抽出します。
            /// </summary>
            /// <param name="source">抽出したデータを格納するバイト配列。</param>
            /// <param name="size">抽出するバイト数。</param>
            public void Split(ref byte[] source, int size)
            {
                BL_Bytes.Copy(data, index, source, 0, size);

                index += size;
            }
            /// <summary>
            /// バイト配列からデータを抽出します。
            /// </summary>
            /// <param name="source">抽出したデータを格納するInt32。</param>
            public void Split(ref int source)
            {
                source = BL_Bytes.BytesToInt32(data, index);

                index += Marshal.SizeOf(source);
            }
            /// <summary>
            /// バイト配列からデータを抽出します。
            /// </summary>
            /// <param name="source">抽出したデータを格納するInt16。</param>
            public void Split(ref short source)
            {
                source = BL_Bytes.BytesToInt16(data, index);

                index += Marshal.SizeOf(source);
            }
        }
    }
}
