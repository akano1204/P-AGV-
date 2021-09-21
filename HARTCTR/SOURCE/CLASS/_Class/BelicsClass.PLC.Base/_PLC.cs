using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

using BelicsClass.Common;
using BelicsClass.Rs232c;
using BelicsClass.Network;

namespace BelicsClass.PLC
{
    /// <summary>
    /// 
    /// </summary>
    public class BL_PLC
    {
        #region PLCデータ変換クラス

        /// <summary>
        /// PLCデータ変換クラス
        /// </summary>
        public class PLC_DATA
        {
            #region Int16配列をバイト配列に変換

            /// <summary>
            /// UInt16配列をバイト配列に変換します。（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されている UInt16配列。</param>
            /// <returns></returns>
            public static byte[] Int16ToBytes(short[] sourceData)
            {
                byte[] data = new byte[sourceData.Length * 2];
                int count;

                for (count = 0; count < sourceData.Length; count++)
                {
                    data[count * 2] = (byte)(sourceData[count] & 0x00FF);
                    data[count * 2 + 1] = (byte)((sourceData[count] >> 8) & 0x00FF);
                }

                return data;
            }

            #endregion

            #region バイト配列を Int16配列に変換します。

            /// <summary>
            /// バイト配列を Int16配列に変換します。
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されているバイト配列。</param>
            /// <returns></returns>
            public static short[] BytesToInt16(byte[] sourceData)
            {
                short[] data = new short[sourceData.Length / 2];
                int count;

                for (count = 0; count < (sourceData.Length / 2); count++)
                {
                    data[count] = BitConverter.ToInt16(sourceData, count * 2);
                }

                return data;
            }

            #endregion

            #region UInt16をバイト配列に変換（リトルエンディアン）

            /// <summary>
            /// UInt16をバイト配列に変換します。（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されている UInt16。</param>
            /// <returns></returns>
            public static byte[] UInt16ToBytes(UInt16 sourceData)
            {
                byte[] data = new byte[2];

                data[0] = (byte)(sourceData & 0x00FF);
                data[1] = (byte)((sourceData >> 8) & 0x00FF);

                return data;
            }
            /// <summary>
            /// UInt16をバイト配列に変換します。（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されている UInt16。</param>
            /// <param name="data">変換したデータを格納するバイト配列。</param>
            /// <param name="offset">変換したデータを格納するバイト配列のオフセット値。</param>
            public static void UInt16ToBytes(UInt16 sourceData, ref byte[] data, int offset)
            {
                data[0 + offset] = (byte)(sourceData & 0x00FF);
                data[1 + offset] = (byte)((sourceData >> 8) & 0x00FF);
            }

            #endregion

            #region UInt16配列をバイト配列に変換（リトルエンディアン）

            /// <summary>
            /// UInt16配列をバイト配列に変換します。（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されている UInt16配列。</param>
            /// <returns></returns>
            public static byte[] UInt16ToBytes(ushort[] sourceData)
            {
                byte[] data = new byte[sourceData.Length * 2];
                int count;

                for (count = 0; count < sourceData.Length; count++)
                {
                    data[count * 2] = (byte)(sourceData[count] & 0x00FF);
                    data[count * 2 + 1] = (byte)((sourceData[count] >> 8) & 0x00FF);
                }

                return data;
            }

            #endregion

            #region バイト配列をUInt16配列に変換（リトルエンディアン）

            /// <summary>
            /// バイト配列を UInt16配列に変換します。（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されているバイト配列。</param>
            /// <returns></returns>
            public static ushort[] BytesToUInt16(byte[] sourceData)
            {
                ushort[] data = new ushort[sourceData.Length / 2];
                int count;

                for (count = 0; count < (sourceData.Length / 2); count++)
                {
                    data[count] = (ushort)(sourceData[count * 2 + 1] * 0x100 + sourceData[count * 2]);
                    //data[count] = BitConverter.ToUInt16(sourceData, count * 2);
                }

                return data;
            }

            /// <summary>
            /// バイト配列を UInt16に変換します。（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されているバイト配列。</param>
            /// <param name="offset_address">変換元のオフセット</param>
            /// <returns></returns>
            public static ushort BytesToUInt16(byte[] sourceData, int offset_address)
            {
                return (ushort)(sourceData[offset_address * 2 + 1] * 0x100 + sourceData[offset_address * 2]);
            }

            #endregion

            #region UInt32をバイト配列に変換（リトルエンディアン）

            /// <summary>
            /// UInt32をバイト配列に変換します。（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されている UInt32。</param>
            /// <returns></returns>
            public static byte[] UInt32ToBytes(UInt32 sourceData)
            {
                byte[] data = new byte[4];

                data[3] = (byte)((sourceData >> 24) & 0x00FF);
                data[2] = (byte)((sourceData >> 16) & 0x00FF);
                data[1] = (byte)((sourceData >> 8) & 0x00FF);
                data[0] = (byte)(sourceData & 0x00FF);

                return data;
            }
            /// <summary>
            /// UInt32をバイト配列に変換します。（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されている UInt32。</param>
            /// <param name="data">変換したデータを格納するバイト配列。</param>
            /// <param name="offset">変換したデータを格納するバイト配列のオフセット値。</param>
            public static void UInt32ToBytes(UInt32 sourceData, ref byte[] data, int offset)
            {
                data[3 + offset] = (byte)((sourceData >> 24) & 0x00FF);
                data[2 + offset] = (byte)((sourceData >> 16) & 0x00FF);
                data[1 + offset] = (byte)((sourceData >> 8) & 0x00FF);
                data[0 + offset] = (byte)(sourceData & 0x00FF);
            }

            #endregion

            #region バイト配列をUInt32に変換（リトルエンディアン）

            /// <summary>
            /// バイト配列を UInt32に変換します。（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されているバイト配列。</param>
            /// <returns></returns>
            public static UInt32 BytesToUInt32(byte[] sourceData)
            {
                return BytesToUInt32(sourceData, 0);
            }
            /// <summary>
            /// バイト配列を UInt32に変換します。（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されているバイト配列。</param>
            /// <param name="offset">変換元のデータが格納されているバイト配列のオフセット値。</param>
            /// <returns></returns>
            public static UInt32 BytesToUInt32(byte[] sourceData, int offset)
            {
                return (UInt32)((sourceData[3 + offset] << 24) + (sourceData[2 + offset] << 16) + (sourceData[1 + offset] << 8) + (sourceData[0 + offset] & 0xFF));
            }

            #endregion

            #region floatをバイト配列に変換（リトルエンディアン）

            /// <summary>
            /// floatをバイト配列に変換（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <returns></returns>
            public static byte[] SingleToBytes(float sourceData)
            {
                byte[] data = new byte[4];

                byte[] ff = BitConverter.GetBytes(sourceData);
                data[0] = ff[0];
                data[1] = ff[1];
                data[2] = ff[2];
                data[3] = ff[3];

                return data;
            }

            #endregion

            #region float配列をバイト配列に変換（リトルエンディアン）

            /// <summary>
            /// float配列をバイト配列に変換（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <returns></returns>
            public static byte[] SingleToBytes(float[] sourceData)
            {
                byte[] data = new byte[sourceData.Length * 4];
                int count;

                for (count = 0; count < sourceData.Length; count++)
                {
                    byte[] ff = BitConverter.GetBytes(sourceData[count]);
                    data[count * 4 + 0] = ff[0];
                    data[count * 4 + 1] = ff[1];
                    data[count * 4 + 2] = ff[2];
                    data[count * 4 + 3] = ff[3];
                }

                return data;
            }

            #endregion

            #region Doubleをバイト配列に変換（リトルエンディアン）

            /// <summary>
            /// Doubleをバイト配列に変換（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <returns></returns>
            public static byte[] DoubleToBytes(Double sourceData)
            {
                byte[] data = new byte[8];

                byte[] ff = BitConverter.GetBytes(sourceData);
                data[0] = ff[0];
                data[1] = ff[1];
                data[2] = ff[2];
                data[3] = ff[3];
                data[4] = ff[4];
                data[5] = ff[5];
                data[6] = ff[6];
                data[7] = ff[7];

                return data;
            }

            #endregion

            #region Double配列をバイト配列に変換（リトルエンディアン）

            /// <summary>
            /// Double配列をバイト配列に変換（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <returns></returns>
            public static byte[] DoubleToBytes(Double[] sourceData)
            {
                byte[] data = new byte[sourceData.Length * 8];
                int count;

                for (count = 0; count < sourceData.Length; count++)
                {
                    byte[] ff = BitConverter.GetBytes(sourceData[count]);
                    data[count * 8 + 0] = ff[0];
                    data[count * 8 + 1] = ff[1];
                    data[count * 8 + 2] = ff[2];
                    data[count * 8 + 3] = ff[3];
                    data[count * 8 + 4] = ff[4];
                    data[count * 8 + 5] = ff[5];
                    data[count * 8 + 6] = ff[6];
                    data[count * 8 + 7] = ff[7];
                }

                return data;
            }

            #endregion

            #region バイト配列をFloat配列に変換（リトルエンディアン）

            /// <summary>
            /// バイト配列をFloat配列に変換（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <returns></returns>
            public static float[] BytesToSingle(byte[] sourceData)
            {
                float[] data = new float[sourceData.Length / 4];
                for (int count = 0; count < sourceData.Length / 4; count++)
                {
                    byte[] ff = new byte[4];
                    ff[0] = sourceData[4 * count + 0];
                    ff[1] = sourceData[4 * count + 1];
                    ff[2] = sourceData[4 * count + 2];
                    ff[3] = sourceData[4 * count + 3];

                    data[count] = BitConverter.ToSingle(ff, 0);
                }
                return data;
            }

            /// <summary>
            /// バイト配列をFloatに変換（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public static float BytesToSingle(byte[] sourceData, int offset)
            {
                byte[] ff = new byte[4];
                ff[0] = sourceData[offset + 0];
                ff[1] = sourceData[offset + 1];
                ff[2] = sourceData[offset + 2];
                ff[3] = sourceData[offset + 3];

                return BitConverter.ToSingle(ff, 0);
            }

            #endregion

            #region バイト配列をDouble配列に変換（リトルエンディアン）

            /// <summary>
            /// バイト配列をDouble配列に変換（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <returns></returns>
            public static double[] BytesToDouble(byte[] sourceData)
            {
                double[] data = new double[sourceData.Length / 8];
                for (int count = 0; count < sourceData.Length / 8; count++)
                {
                    byte[] ff = new byte[4];
                    ff[0] = sourceData[8 * count + 0];
                    ff[1] = sourceData[8 * count + 1];
                    ff[2] = sourceData[8 * count + 2];
                    ff[3] = sourceData[8 * count + 3];
                    ff[4] = sourceData[8 * count + 4];
                    ff[5] = sourceData[8 * count + 5];
                    ff[6] = sourceData[8 * count + 6];
                    ff[7] = sourceData[8 * count + 7];


                    data[count] = BitConverter.ToDouble(ff, 0);
                }
                return data;
            }

            /// <summary>
            /// バイト配列をDoubleに変換（リトルエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public static double BytesToDouble(byte[] sourceData, int offset)
            {
                byte[] ff = new byte[8];
                ff[0] = sourceData[offset + 0];
                ff[1] = sourceData[offset + 1];
                ff[2] = sourceData[offset + 2];
                ff[3] = sourceData[offset + 3];
                ff[4] = sourceData[offset + 4];
                ff[5] = sourceData[offset + 5];
                ff[6] = sourceData[offset + 6];
                ff[7] = sourceData[offset + 7];

                return BitConverter.ToDouble(ff, 0);
            }

            #endregion

            #region UInt32をバイト配列に変換（野口）

            /// <summary>
            /// UInt32をバイト配列に変換します。（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されている UInt32。</param>
            /// <returns></returns>
            public static byte[] UInt32ToBytesBK(UInt32 sourceData)
            {
                byte[] data = new byte[4];

                data[2] = (byte)((sourceData >> 24) & 0x00FF);
                data[3] = (byte)((sourceData >> 16) & 0x00FF);
                data[0] = (byte)((sourceData >> 8) & 0x00FF);
                data[1] = (byte)(sourceData & 0x00FF);

                return data;
            }
            /// <summary>
            /// UInt32をバイト配列に変換します。（野口）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されている UInt32。</param>
            /// <param name="data">変換したデータを格納するバイト配列。</param>
            /// <param name="offset">変換したデータを格納するバイト配列のオフセット値。</param>
            public static void UInt32ToBytesBK(UInt32 sourceData, ref byte[] data, int offset)
            {
                data[2 + offset] = (byte)((sourceData >> 24) & 0x00FF);
                data[3 + offset] = (byte)((sourceData >> 16) & 0x00FF);
                data[0 + offset] = (byte)((sourceData >> 8) & 0x00FF);
                data[1 + offset] = (byte)(sourceData & 0x00FF);
            }

            #endregion

            #region バイト配列をUInt32に変換（野口）

            /// <summary>
            /// バイト配列を UInt32に変換します。（野口）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されているバイト配列。</param>
            /// <returns></returns>
            public static UInt32 BytesToUInt32BK(byte[] sourceData)
            {
                return BytesToUInt32BK(sourceData, 0);
            }
            /// <summary>
            /// バイト配列を UInt32に変換します。（野口）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されているバイト配列。</param>
            /// <param name="offset">変換元のデータが格納されているバイト配列のオフセット値。</param>
            /// <returns></returns>
            public static UInt32 BytesToUInt32BK(byte[] sourceData, int offset)
            {
                return (UInt32)((sourceData[2 + offset] << 24) + (sourceData[3 + offset] << 16) + (sourceData[offset] << 8) + (sourceData[offset + 1] & 0xFF));
            }

            #endregion

            #region UInt16をバイト配列に変換（ビッグエンディアン）

            /// <summary>
            /// UInt16をバイト配列に変換します。（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されている UInt16。</param>
            /// <returns></returns>
            public static byte[] UInt16ToBytesB(UInt16 sourceData)
            {
                byte[] data = new byte[2];

                data[0] = (byte)((sourceData >> 8) & 0x00FF);
                data[1] = (byte)(sourceData & 0x00FF);

                return data;
            }
            /// <summary>
            /// UInt16をバイト配列に変換します。（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されている UInt16。</param>
            /// <param name="data">変換したデータを格納するバイト配列。</param>
            /// <param name="offset">変換したデータを格納するバイト配列のオフセット値。</param>
            public static void UInt16ToBytesB(UInt16 sourceData, ref byte[] data, int offset)
            {
                data[0 + offset] = (byte)((sourceData >> 8) & 0x00FF);
                data[1 + offset] = (byte)(sourceData & 0x00FF);
            }

            #endregion

            #region UInt16配列をバイト配列に変換（ビッグエンディアン）

            /// <summary>
            /// UInt16配列をバイト配列に変換します。（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されている UInt16配列。</param>
            /// <returns></returns>
            public static byte[] UInt16ToBytesB(ushort[] sourceData)
            {
                byte[] data = new byte[sourceData.Length * 2];
                int count;

                for (count = 0; count < sourceData.Length; count++)
                {
                    data[count * 2] = (byte)((sourceData[count] >> 8) & 0x00FF);
                    data[count * 2 + 1] = (byte)(sourceData[count] & 0x00FF);
                }

                return data;
            }

            #endregion

            #region バイト配列をUInt16配列に変換（ビッグエンディアン）

            /// <summary>
            /// バイト配列を UInt16配列に変換します。（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されているバイト配列。</param>
            /// <returns></returns>
            public static ushort[] BytesToUInt16B(byte[] sourceData)
            {
                ushort[] data = new ushort[sourceData.Length / 2];
                int count;

                for (count = 0; count < (sourceData.Length / 2); count++)
                {
                    data[count] = (ushort)(sourceData[count * 2] * 0x100 + sourceData[count * 2 + 1]);
                }

                return data;
            }

            /// <summary>
            /// バイト配列を UInt16に変換します。（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <param name="offset_address"></param>
            /// <returns></returns>
            public static ushort BytesToUInt16B(byte[] sourceData, int offset_address)
            {
                return (ushort)(sourceData[offset_address * 2] * 0x100 + sourceData[offset_address * 2 + 1]);
            }

            #endregion

            #region UInt32をバイト配列に変換（ビッグエンディアン）

            /// <summary>
            /// UInt32をバイト配列に変換します。（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されている UInt32。</param>
            /// <returns></returns>
            public static byte[] UInt32ToBytesB(UInt32 sourceData)
            {
                byte[] data = new byte[4];

                data[0] = (byte)((sourceData >> 24) & 0x00FF);
                data[1] = (byte)((sourceData >> 16) & 0x00FF);
                data[2] = (byte)((sourceData >> 8) & 0x00FF);
                data[3] = (byte)(sourceData & 0x00FF);

                return data;
            }
            /// <summary>
            /// UInt32をバイト配列に変換します。（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されている UInt32。</param>
            /// <param name="data">変換したデータを格納するバイト配列。</param>
            /// <param name="offset">変換したデータを格納するバイト配列のオフセット値。</param>
            public static void UInt32ToBytesB(UInt32 sourceData, ref byte[] data, int offset)
            {
                data[0 + offset] = (byte)((sourceData >> 24) & 0x00FF);
                data[1 + offset] = (byte)((sourceData >> 16) & 0x00FF);
                data[2 + offset] = (byte)((sourceData >> 8) & 0x00FF);
                data[3 + offset] = (byte)(sourceData & 0x00FF);
            }

            #endregion

            #region バイト配列をUInt32に変換（ビッグエンディアン）

            /// <summary>
            /// バイト配列を UInt32に変換します。（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されているバイト配列。</param>
            /// <returns></returns>
            public static UInt32 BytesToUInt32B(byte[] sourceData)
            {
                return BytesToUInt32B(sourceData, 0);
            }
            /// <summary>
            /// バイト配列を UInt32に変換します。（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData">変換元のデータが格納されているバイト配列。</param>
            /// <param name="offset">変換元のデータが格納されているバイト配列のオフセット値。</param>
            /// <returns></returns>
            public static UInt32 BytesToUInt32B(byte[] sourceData, int offset)
            {
                return (UInt32)((sourceData[0 + offset] << 24) + (sourceData[1 + offset] << 16) + (sourceData[2 + offset] << 8) + (sourceData[3 + offset] & 0xFF));
            }

            #endregion

            #region floatをバイト配列に変換（ビッグエンディアン）

            /// <summary>
            /// floatをバイト配列に変換（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <returns></returns>
            public static byte[] SingleToBytesB(float sourceData)
            {
                byte[] data = new byte[4];

                byte[] ff = BitConverter.GetBytes(sourceData);
                data[1] = ff[0];
                data[0] = ff[1];
                data[3] = ff[2];
                data[2] = ff[3];

                return data;
            }

            #endregion

            #region float配列をバイト配列に変換（ビッグエンディアン）

            /// <summary>
            /// float配列をバイト配列に変換（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <returns></returns>
            public static byte[] SingleToBytesB(float[] sourceData)
            {
                byte[] data = new byte[sourceData.Length * 4];
                int count;

                for (count = 0; count < sourceData.Length; count++)
                {
                    byte[] ff = BitConverter.GetBytes(sourceData[count]);
                    data[count * 4 + 1] = ff[0];
                    data[count * 4 + 0] = ff[1];
                    data[count * 4 + 3] = ff[2];
                    data[count * 4 + 2] = ff[3];
                }

                return data;
            }

            #endregion

            #region Doubleをバイト配列に変換（ビッグエンディアン）

            /// <summary>
            /// Doubleをバイト配列に変換（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <returns></returns>
            public static byte[] DoubleToBytesB(Double sourceData)
            {
                byte[] data = new byte[8];

                byte[] ff = BitConverter.GetBytes(sourceData);
                data[1] = ff[0];
                data[0] = ff[1];
                data[3] = ff[2];
                data[2] = ff[3];
                data[5] = ff[4];
                data[4] = ff[5];
                data[7] = ff[6];
                data[6] = ff[7];

                return data;
            }

            #endregion

            #region Double配列をバイト配列に変換（ビッグエンディアン）

            /// <summary>
            /// Double配列をバイト配列に変換（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <returns></returns>
            public static byte[] DoubleToBytesB(Double[] sourceData)
            {
                byte[] data = new byte[sourceData.Length * 8];
                int count;

                for (count = 0; count < sourceData.Length; count++)
                {
                    byte[] ff = BitConverter.GetBytes(sourceData[count]);
                    data[count * 8 + 1] = ff[0];
                    data[count * 8 + 0] = ff[1];
                    data[count * 8 + 3] = ff[2];
                    data[count * 8 + 2] = ff[3];
                    data[count * 8 + 5] = ff[4];
                    data[count * 8 + 4] = ff[5];
                    data[count * 8 + 7] = ff[6];
                    data[count * 8 + 6] = ff[7];
                }

                return data;
            }

            #endregion

            #region バイト配列をFloat配列に変換（ビッグエンディアン）

            /// <summary>
            /// バイト配列をFloat配列に変換（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <returns></returns>
            public static float[] BytesToSingleB(byte[] sourceData)
            {
                float[] data = new float[sourceData.Length / 4];
                for (int count = 0; count < sourceData.Length / 4; count++)
                {
                    byte[] ff = new byte[4];
                    ff[0] = sourceData[4 * count + 1];
                    ff[1] = sourceData[4 * count + 0];
                    ff[2] = sourceData[4 * count + 3];
                    ff[3] = sourceData[4 * count + 2];

                    data[count] = BitConverter.ToSingle(ff, 0);
                }
                return data;
            }

            /// <summary>
            /// バイト配列をFloatに変換（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public static float BytesToSingleB(byte[] sourceData, int offset)
            {
                byte[] ff = new byte[4];
                ff[0] = sourceData[offset + 1];
                ff[1] = sourceData[offset + 0];
                ff[2] = sourceData[offset + 3];
                ff[3] = sourceData[offset + 2];

                return BitConverter.ToSingle(ff, 0);
            }

            #endregion

            #region バイト配列をDouble配列に変換（ビッグエンディアン）

            /// <summary>
            /// バイト配列をDouble配列に変換（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <returns></returns>
            public static double[] BytesToDoubleB(byte[] sourceData)
            {
                double[] data = new double[sourceData.Length / 8];
                for (int count = 0; count < sourceData.Length / 8; count++)
                {
                    byte[] ff = new byte[4];
                    ff[0] = sourceData[8 * count + 1];
                    ff[1] = sourceData[8 * count + 0];
                    ff[2] = sourceData[8 * count + 3];
                    ff[3] = sourceData[8 * count + 2];
                    ff[4] = sourceData[8 * count + 5];
                    ff[5] = sourceData[8 * count + 4];
                    ff[6] = sourceData[8 * count + 7];
                    ff[7] = sourceData[8 * count + 6];


                    data[count] = BitConverter.ToDouble(ff, 0);
                }
                return data;
            }

            /// <summary>
            /// バイト配列をDoubleに変換（ビッグエンディアン）
            /// </summary>
            /// <param name="sourceData"></param>
            /// <param name="offset"></param>
            /// <returns></returns>
            public static double BytesToDoubleB(byte[] sourceData, int offset)
            {
                byte[] ff = new byte[8];
                ff[0] = sourceData[offset + 1];
                ff[1] = sourceData[offset + 0];
                ff[2] = sourceData[offset + 3];
                ff[3] = sourceData[offset + 2];
                ff[4] = sourceData[offset + 5];
                ff[5] = sourceData[offset + 4];
                ff[6] = sourceData[offset + 7];
                ff[7] = sourceData[offset + 6];

                return BitConverter.ToDouble(ff, 0);
            }

            #endregion

            #region バイト配列を16進表記文字バイト配列に変換

            /// <summary>
            /// バイト配列を16進表記文字バイト配列に変換します。
            /// </summary>
            /// <param name="bytesData">変換するバイト配列。</param>
            /// <returns></returns>
            public static byte[] BytesToHexStringBytes(byte[] bytesData)
            {
                StringBuilder data = new StringBuilder();

                foreach (byte temp in bytesData)
                {
                    data.Append(temp.ToString("X2"));
                }

                return Encoding.Default.GetBytes(data.ToString());
            }

            #endregion

            #region 16進表記文字バイト配列をバイト配列に変換

            /// <summary>
            /// 16進表記文字バイト配列をバイト配列に変換します。
            /// </summary>
            /// <param name="hexStringBytesData">変換する16進表記のバイト配列。</param>
            /// <returns></returns>
            public static byte[] HexStringBytesToBytes(byte[] hexStringBytesData)
            {
                int count;
                byte[] data;
                byte temp;
                string check;

                data = new byte[hexStringBytesData.Length / 2];

                for (count = 0; count < data.Length; count++)
                {
                    check = Encoding.Default.GetString(hexStringBytesData, count * 2, 1).ToUpper();

                    if (check[0] >= '0' && check[0] <= '9')
                    {
                        temp = (byte)((check[0] - '0') << 4);
                    }
                    else if (check[0] >= 'A' && check[0] <= 'F')
                    {
                        temp = (byte)((check[0] - 'A' + 10) << 4);
                    }
                    else
                    {
                        break;
                    }

                    check = Encoding.Default.GetString(hexStringBytesData, count * 2 + 1, 1).ToUpper();

                    if (check[0] >= '0' && check[0] <= '9')
                    {
                        temp += (byte)((check[0] - '0'));
                    }
                    else if (check[0] >= 'A' && check[0] <= 'F')
                    {
                        temp += (byte)((check[0] - 'A' + 10));
                    }
                    else
                    {
                        break;
                    }

                    data[count] = temp;
                }

                return data;
            }

            #endregion
        }

        #endregion

        #region 列挙型

        /// <summary>
        /// コマンドタイプ
        /// </summary>
        public enum CommandType
        {
            /// <summary>ACPU共通コマンド</summary>
            MELSEC_A_CPU,

            /// <summary>AnA/AnAUCPU共通コマンド</summary>
            MELSEC_AnA_CPU,

            /// <summary>OMRON FINS</summary>
            OMRON_FINS,

            /// <summary>OMRON Cコマンド</summary>
            OMRON_C,
        }

        /// <summary>
        /// デバイスタイプ
        /// </summary>
        public enum DeviceType
        {
            /// <summary>未接続</summary>
            NONE,
            /// <summary>RS-232C</summary>
            RS_232C,
            /// <summary>TCP</summary>
            LAN,
            /// <summary>UDP</summary>
            LAN_UDP,
        }

        #endregion

        /// <summary></summary>
        protected int Error_code;
        /// <summary></summary>
        protected string Error_message;

        /// <summary></summary>
        protected DeviceType Device = DeviceType.NONE;
        /// <summary></summary>
        protected CommandType Command = CommandType.MELSEC_A_CPU;

        /// <summary></summary>
        protected BL_Rs232c Com;
        /// <summary></summary>
        protected BL_RawSocket Socket;
        /// <summary></summary>
        protected BL_RawSocketUDP SocketUDP;
        /// <summary></summary>
        protected IPEndPoint RemoteEP;
        /// <summary></summary>
        protected byte[] ReceiveBytes = new byte[0];

        /// <summary>
        /// 
        /// </summary>
        /// <param name="portNo"></param>
        /// <param name="baudRate"></param>
        /// <param name="byteSize"></param>
        /// <param name="parity"></param>
        /// <param name="stopBits"></param>
        /// <returns></returns>
        public virtual bool Connect(BL_Rs232c.PortNo portNo, BL_Rs232c.BaudRate baudRate, BL_Rs232c.ByteSize byteSize, BL_Rs232c.Parity parity, BL_Rs232c.StopBits stopBits) { throw new NotSupportedException("派生クラスで処理を実装してください。"); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandType"></param>
        /// <param name="portNo"></param>
        /// <param name="baudRate"></param>
        /// <param name="byteSize"></param>
        /// <param name="parity"></param>
        /// <param name="stopBits"></param>
        /// <returns></returns>
        public virtual bool Connect(CommandType commandType, BL_Rs232c.PortNo portNo, BL_Rs232c.BaudRate baudRate, BL_Rs232c.ByteSize byteSize, BL_Rs232c.Parity parity, BL_Rs232c.StopBits stopBits) { throw new NotSupportedException("派生クラスで処理を実装してください。"); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip_address"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public virtual bool Connect(string ip_address, int port) { throw new NotSupportedException("派生クラスで処理を実装してください。"); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ip_address"></param>
        /// <param name="port"></param>
        /// <param name="local_port"></param>
        /// <returns></returns>
        public virtual bool Connect(string ip_address, int port, int local_port) { throw new NotSupportedException("派生クラスで処理を実装してください。"); }
        /// <summary>
        /// 
        /// </summary>
        public virtual void Disconnect() { throw new NotSupportedException("派生クラスで処理を実装してください。"); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="offset"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public virtual bool ReadCommand(byte deviceCode, int offset, int point) { throw new NotSupportedException("派生クラスで処理を実装してください。"); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public virtual byte[] ReadCommandAck(out int length) { throw new NotSupportedException("派生クラスで処理を実装してください。"); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public virtual byte[] ReadCommandAck(int length) { throw new NotSupportedException("派生クラスで処理を実装してください。"); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="deviceCode"></param>
        /// <param name="offset"></param>
        /// <param name="point"></param>
        /// <param name="writeData"></param>
        /// <returns></returns>
        public virtual bool WriteCommand(byte deviceCode, int offset, int point, byte[] writeData) { throw new NotSupportedException("派生クラスで処理を実装してください。"); }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public virtual byte[] WriteCommandAck(out int length) { throw new NotSupportedException("派生クラスで処理を実装してください。"); }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual byte[] WriteCommandAck() { throw new NotSupportedException("派生クラスで処理を実装してください。"); }

        #region 接続状態プロパティ

        /// <summary>
        /// ソケットの接続状態を取得します。接続している場合は true。それ以外の場合は false。
        /// </summary>
        public virtual bool IsConnected { get { throw new NotSupportedException("派生クラスで処理を実装してください。"); } }

        #endregion

        #region 異常コードプロパティ

        /// <summary>
        /// 異常コードを取得します。
        /// </summary>
        public int ErrorCode
        {
            get
            {
                return Error_code;
            }
        }

        #endregion

        #region 異常内容プロパティ

        /// <summary>
        /// 異常内容を取得します。
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return Error_message;
            }
        }

        #endregion

        #region 異常の設定

        /// <summary>
        /// 
        /// </summary>
        protected void errors()
        {
            Error_code = 0;
            Error_message = "";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="error_code"></param>
        /// <param name="comment"></param>
        protected void errors(int error_code, string comment)
        {
            Error_code = error_code;
            Error_message = comment;
        }

        #endregion
    }
}
