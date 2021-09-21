using System;
using System.Text;
using System.Text.RegularExpressions;

using BelicsClass.Common;
using BelicsClass.Rs232c;
using BelicsClass.Network;

namespace BelicsClass.PLC
{
	#region デバイスコード

	/// <summary>デバイスコードを定義します</summary>
	public enum DeviceCode : byte
	{
		/// <summary>チャネルI/O（ビット）</summary>
		CIO_B = 0x30,
		/// <summary>内部補助リレー（ビット）</summary>
		WR_B = 0x31,
		/// <summary>保持リレー（ビット）</summary>
		HR_B = 0x32,
		/// <summary>特殊保持リレー（ビット）</summary>
		AR_B = 0x33,

		/// <summary>チャネルI/O（強制ON/OFF付ビット）</summary>
		CIO_BF = 0x70,
		/// <summary>内部補助リレー（強制ON/OFF付ビット）</summary>
		WR_BF = 0x71,
		/// <summary>保持リレー（強制ON/OFF付ビット）</summary>
		HR_BF = 0x72,

		/// <summary>チャネルI/O（ワード）</summary>
		CIO = 0xB0,
		/// <summary>内部補助リレー（ワード）</summary>
		WR = 0xB1,
		/// <summary>保持リレー（ワード）</summary>
		HR = 0xB2,
		/// <summary>特殊保持リレー（ワード）</summary>
		AR = 0xB3,

		/// <summary>チャネルI/O（強制ON/OFF付ワード）</summary>
		CIO_F = 0xF0,
		/// <summary>内部補助リレー（強制ON/OFF付ワード）</summary>
		WR_F = 0xF1,
		/// <summary>保持リレー（強制ON/OFF付ワード）</summary>
		HR_F = 0xF2,

		/// <summary>タイマ（アップフラグ）</summary>
		TIM = 0x09,
		/// <summary>カウンタ（アップフラグ）</summary>
		CNT = 0x09,
		/// <summary>タイマ（強制ON/OFF付きアップフラグ）</summary>
		TIM2 = 0x49,
		/// <summary>カウンタ（強制ON/OFF付きアップフラグ）</summary>
		CNT2 = 0x49,
		/// <summary>タイマ（現在値）</summary>
		TIM_N = 0x89,
		/// <summary>カウンタ（現在値）</summary>
		CNT_N = 0x89,
		/// <summary>データメモリ（ビット）</summary>
		DM_B = 0x02,
		/// <summary>データメモリ（ワード）</summary>
		DM = 0x82,
		/// <summary>EMバンク0（ビット）</summary>
		EM0_B = 0x20,
		/// <summary>EMバンク1（ビット）</summary>
		EM1_B = 0x21,
		/// <summary>EMバンク2（ビット）</summary>
		EM2_B = 0x22,
		/// <summary>EMバンク3（ビット）</summary>
		EM3_B = 0x23,
		/// <summary>EMバンク4（ビット）</summary>
		EM4_B = 0x24,
		/// <summary>EMバンク5（ビット）</summary>
		EM5_B = 0x25,
		/// <summary>EMバンク6（ビット）</summary>
		EM6_B = 0x26,
		/// <summary>EMバンク7（ビット）</summary>
		EM7_B = 0x27,
		/// <summary>EMバンク8（ビット）</summary>
		EM8_B = 0x28,
		/// <summary>EMバンク9（ビット）</summary>
		EM9_B = 0x29,
		/// <summary>EMバンクA（ビット）</summary>
		EMA_B = 0x2A,
		/// <summary>EMバンクB（ビット）</summary>
		EMB_B = 0x2B,
		/// <summary>EMバンクC（ビット）</summary>
		EMC_B = 0x2C,
		/// <summary>EMバンクD（ビット）</summary>
		EMD_B = 0x2D,
		/// <summary>EMバンクE（ビット）</summary>
		EME_B = 0x2E,
		/// <summary>EMバンクF（ビット）</summary>
		EMF_B = 0x2F,
		/// <summary>EMバンク10（ビット）</summary>
		EM10_B = 0xE0,
		/// <summary>EMバンク11（ビット）</summary>
		EM11_B = 0xE1,
		/// <summary>EMバンク12（ビット）</summary>
		EM12_B = 0xE2,
		/// <summary>EMバンク13（ビット）</summary>
		EM13_B = 0xE3,
		/// <summary>EMバンク14（ビット）</summary>
		EM14_B = 0xE4,
		/// <summary>EMバンク15（ビット）</summary>
		EM15_B = 0xE5,
		/// <summary>EMバンク16（ビット）</summary>
		EM16_B = 0xE6,
		/// <summary>EMバンク17（ビット）</summary>
		EM17_B = 0xE7,
		/// <summary>EMバンク18（ビット）</summary>
		EM18_B = 0xE8,
		/// <summary>EMバンク0（ワード）</summary>
		EM0 = 0xA0,
		/// <summary>EMバンク1（ワード）</summary>
		EM1 = 0xA1,
		/// <summary>EMバンク2（ワード）</summary>
		EM2 = 0xA2,
		/// <summary>EMバンク3（ワード）</summary>
		EM3 = 0xA3,
		/// <summary>EMバンク4（ワード）</summary>
		EM4 = 0xA4,
		/// <summary>EMバンク5（ワード）</summary>
		EM5 = 0xA5,
		/// <summary>EMバンク6（ワード）</summary>
		EM6 = 0xA6,
		/// <summary>EMバンク7（ワード）</summary>
		EM7 = 0xA7,
		/// <summary>EMバンク8（ワード）</summary>
		EM8 = 0xA8,
		/// <summary>EMバンク9（ワード）</summary>
		EM9 = 0xA9,
		/// <summary>EMバンクA（ワード）</summary>
		EMA = 0xAA,
		/// <summary>EMバンクB（ワード）</summary>
		EMB = 0xAB,
		/// <summary>EMバンクC（ワード）</summary>
		EMC = 0xAC,
		/// <summary>EMバンクD（ワード）</summary>
		EMD = 0xAD,
		/// <summary>EMバンクE（ワード）</summary>
		EME = 0xAE,
		/// <summary>EMバンクF（ワード）</summary>
		EMF = 0xAF,
		/// <summary>EMバンク10（ワード）</summary>
		EM10 = 0x60,
		/// <summary>EMバンク11（ワード）</summary>
		EM11 = 0x61,
		/// <summary>EMバンク12（ワード）</summary>
		EM12 = 0x62,
		/// <summary>EMバンク13（ワード）</summary>
		EM13 = 0x63,
		/// <summary>EMバンク14（ワード）</summary>
		EM14 = 0x64,
		/// <summary>EMバンク15（ワード）</summary>
		EM15 = 0x65,
		/// <summary>EMバンク16（ワード）</summary>
		EM16 = 0x66,
		/// <summary>EMバンク17（ワード）</summary>
		EM17 = 0x67,
		/// <summary>EMバンク18（ワード）</summary>
		EM18 = 0x68,
		/// <summary>EMカレントバンク（ビット）</summary>
		EM_BC = 0x0A,
		/// <summary>EMカレントバンク（ワード）</summary>
		EM_C = 0x98,
		/// <summary>EMカレントバンクNo</summary>
		EM_CBN = 0xBC,
		/// <summary>タスクフラグ（ビット）</summary>
		TK_B = 0x06,
		/// <summary>タスクフラグ（ステータス）</summary>
		TK_S = 0x46,
		/// <summary>インデックスレジスタ</summary>
		IR = 0xDC,
		/// <summary>データレジスタ</summary>
		DR = 0xBC,
		/// <summary>クロックパルス（ビット）</summary>
		CP_B = 0x07,
		/// <summary>コンディションフラグ（ビット）</summary>
		CF_B = 0x07,
	}

	#endregion

    #region PLC通信クラス（オムロン　ＦＩＮＳコマンド用）

    /// <summary>
    /// PLC通信クラス（オムロン　ＦＩＮＳコマンド用）
    /// </summary>
    public class OMRON_FINS : BL_PLC
    {
        private byte ServerNode = 0x00;
        private byte ClientNode = 0x00;

        #region 接続

        /// <summary>
        /// PLCと接続します。
        /// </summary>
        /// <param name="ip_address">接続するPLCのIPアドレス。</param>
        /// <param name="port">接続するPLCのポート№。</param>
        /// <returns>true:正常,false:異常</returns>
        public override bool Connect(string ip_address, int port)
        {
            bool status = false;

            errors();

            ReceiveBytes = new byte[0];

            if (Device == DeviceType.NONE)
            {
                Socket = new BL_RawSocket(ip_address, port);

                status = Socket.Open(BL_RawSocket.OpenMode.Client, BL_RawSocket.FormatType.None);

                if (status)
                {
                    Device = DeviceType.LAN;
                    Command = CommandType.OMRON_FINS;

                    ServerNode = 0x00;
                    ClientNode = 0x00;

                    //ノード情報の取得
                    {
                        if (!NodeInfoCommand())
                        {
                            errors(-10001, "ノード情報が取得できません。");
                            return false;
                        }

                        BL_Stopwatch swTimeout = new BL_Stopwatch();
                        swTimeout.Start();

                        while (!NodeInfoCommandAck())
                        {
							System.Threading.Thread.Sleep(20);

							if (3000 < swTimeout.ElapsedMilliseconds)
                            {
                                errors(-10001, "ノード情報が取得できません。");
                                return false;
                            }
                        }
                    }
                }
                else
                {
                    errors(Socket.ErrorCode, Socket.ErrorMessage);

                    Socket = null;
                }
            }
            else
            {
                errors(-10000, "既にオープンされています。");
            }

            return status;
        }

        #endregion

        #region 接続解除

        /// <summary>
        /// PLCとの接続を解除します。
        /// </summary>
        public override void Disconnect()
        {
            if (Device == DeviceType.LAN)
            {
                Socket.Close();

                Socket = null;
            }

            Device = DeviceType.NONE;
        }

        #endregion

        #region ノードアドレス情報

        /// <summary>
        /// PLCにノードアドレス情報コマンドを送信します。
        /// </summary>
        /// <returns>true:正常,false:異常</returns>
        public bool NodeInfoCommand()
        {
            byte[] data;
            bool status = false;

            errors();

            if (Device == DeviceType.LAN)
            {
                data = new byte[4 + 4 + 4 + 4 + 4];

                PLC_DATA.UInt32ToBytesB(0x46494E53, ref data, 0);
                PLC_DATA.UInt32ToBytesB(12, ref data, 4);
                PLC_DATA.UInt32ToBytesB(0, ref data, 8);
                PLC_DATA.UInt32ToBytesB(0, ref data, 12);
                PLC_DATA.UInt32ToBytesB(0, ref data, 16);

                status = Socket.SendBytes(data);

                if (!status)
                {
                    errors(Socket.ErrorCode, Socket.ErrorMessage);
                }

                ReceiveBytes = new byte[0];
            }

            return status;
        }

        #endregion

        #region ノードアドレス情報応答

        /// <summary>
        /// PLCからノードアドレス情報コマンドの返答を受信します。
        /// </summary>
        /// <returns>true:正常,false:異常</returns>
        public bool NodeInfoCommandAck()
        {
            byte[] data, temp;
            bool status = false;

            data = new byte[0];

            if (Device == DeviceType.LAN)
            {
                temp = Socket.ReceiveBytes();

                if (temp.Length > 0)
                {
                    BL_Bytes.Join(temp, ref ReceiveBytes, temp.Length);
                }

                if (ReceiveBytes.Length >= 24)
                {
                    if (0x46494E53 == PLC_DATA.BytesToUInt32B(ReceiveBytes, 0))
                    {
                        if (16 == PLC_DATA.BytesToUInt32B(ReceiveBytes, 4) && 1 == PLC_DATA.BytesToUInt32B(ReceiveBytes, 8) && 0 == PLC_DATA.BytesToUInt32B(ReceiveBytes, 12))
                        {
                            ClientNode = ReceiveBytes[19];
                            ServerNode = ReceiveBytes[23];

                            status = true;
                        }
                    }
                }
            }

            return status;
        }

        #endregion

        #region メモリエリアの読出

        /// <summary>
        /// PLCにメモリエリアの読出コマンドを送信します。
        /// </summary>
        /// <param name="deviceCode">読み込むデバイスのデバイスコード。</param>
        /// <param name="offset">読み込むデバイスのオフセット値。</param>
        /// <param name="point">読み込むデバイスの点数。</param>
        /// <returns>true:正常,false:異常</returns>
        public override bool ReadCommand(byte deviceCode, int offset, int point)
        {
            byte[] data;
            bool status = false;

            errors();

            if (Device == DeviceType.LAN)
            {
                if (point < 1 || point > 999)
                {
                    errors(-10005, "コマンドが不正です。");

                    return status;
                }

                if (ServerNode == 0x00 || ClientNode == 0x00)
                {
                    errors(-10010, "ノードアドレスが不正です。");

                    return status;
                }

                data = new byte[16 + 10 + 8];

                PLC_DATA.UInt32ToBytesB(0x46494E53, ref data, 0);
                PLC_DATA.UInt32ToBytesB(8 + 10 + 8, ref data, 4);
                PLC_DATA.UInt32ToBytesB(2, ref data, 8);
                PLC_DATA.UInt32ToBytesB(0, ref data, 12);

                data[16] = 0x80;
                data[17] = 0x00;
                data[18] = 0x02;
                data[19] = 0x00;
                data[20] = ServerNode;
                data[21] = 0x00;
                data[22] = 0x00;
                data[23] = ClientNode;
                data[24] = 0x00;
                data[25] = 0x00;

                data[26] = 0x01;
                data[27] = 0x01;
                data[28] = deviceCode;
                data[29] = (byte)((offset >> 8) & 0x000000FF);
                data[30] = (byte)(offset & 0x000000FF);
                data[31] = 0x00;
                data[32] = (byte)((point >> 8) & 0x000000FF);
                data[33] = (byte)(point & 0x000000FF);

                status = Socket.SendBytes(data);

                if (!status)
                {
                    errors(Socket.ErrorCode, Socket.ErrorMessage);
                }

                ReceiveBytes = new byte[0];
            }

            return status;
        }

        #endregion

        #region メモリエリアの読出応答

        /// <summary>
        /// PLCからメモリエリアの読出コマンドの返答を受信します。
        /// </summary>
        /// <param name="length">読み込んだデバイスのバイト数。</param>
        /// <returns>受信したデータのバイト配列</returns>
        public override byte[] ReadCommandAck(out int length)
        {
            byte[] data, temp;

            data = new byte[0];
            length = -1;

            if (Device == DeviceType.LAN)
            {
                temp = Socket.ReceiveBytes();

                if (temp.Length > 0)
                {
                    BL_Bytes.Join(temp, ref ReceiveBytes, temp.Length);
                }

                if (ReceiveBytes.Length >= 30)
                {
                    if (0x46494E53 == PLC_DATA.BytesToUInt32B(ReceiveBytes, 0))
                    {
                        if ((ReceiveBytes.Length - 8) >= PLC_DATA.BytesToUInt32B(ReceiveBytes, 4))
                        {
                            if (2 == PLC_DATA.BytesToUInt32B(ReceiveBytes, 8) && 0 == PLC_DATA.BytesToUInt32B(ReceiveBytes, 12))
                            {
                                if (ReceiveBytes[20] == ClientNode && ReceiveBytes[23] == ServerNode)
                                {
                                    length = (int)PLC_DATA.BytesToUInt32B(ReceiveBytes, 4) - 22;

                                    if (0x01010000 == PLC_DATA.BytesToUInt32B(ReceiveBytes, 26))
                                    {
                                        data = new byte[length];
                                        Array.Copy(ReceiveBytes, 30, data, 0, length);
                                    }
                                    else
                                    {
                                        length = 0;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return data;
        }

        #endregion

        #region メモリエリアの書込

        /// <summary>
        /// PLCにメモリエリアの書込コマンドを送信します。
        /// </summary>
        /// <param name="deviceCode">書き込むデバイスのデバイスコード。</param>
        /// <param name="offset">書き込むデバイスのオフセット値。</param>
        /// <param name="point">書き込むデバイスの点数。</param>
        /// <param name="writeData">書き込むデバイスのデータが格納されているバイト配列。</param>
        /// <returns>true:正常,false:異常</returns>
        public override bool WriteCommand(byte deviceCode, int offset, int point, byte[] writeData)
        {
            byte[] data;
            bool status = false;

            errors();

            if (Device == DeviceType.LAN)
            {
                if (point < 1 || point > 999)
                {
                    errors(-10005, "コマンドが不正です。");

                    return status;
                }

                if (ServerNode == 0x00 || ClientNode == 0x00)
                {
                    errors(-10010, "ノードアドレスが不正です。");

                    return status;
                }

                data = new byte[16 + 10 + 8 + point * 2];

                PLC_DATA.UInt32ToBytesB(0x46494E53, ref data, 0);
                PLC_DATA.UInt32ToBytesB((uint)(8 + 10 + 8 + point * 2), ref data, 4);
                PLC_DATA.UInt32ToBytesB(2, ref data, 8);
                PLC_DATA.UInt32ToBytesB(0, ref data, 12);

                data[16] = 0x80;
                data[17] = 0x00;
                data[18] = 0x02;
                data[19] = 0x00;
                data[20] = ServerNode;
                data[21] = 0x00;
                data[22] = 0x00;
                data[23] = ClientNode;
                data[24] = 0x00;
                data[25] = 0x00;

                data[26] = 0x01;
                data[27] = 0x02;
                data[28] = deviceCode;
                data[29] = (byte)((offset >> 8) & 0x000000FF);
                data[30] = (byte)(offset & 0x000000FF);
                data[31] = 0x00;
                data[32] = (byte)((point >> 8) & 0x000000FF);
                data[33] = (byte)(point & 0x000000FF);

                if (writeData.Length > (point * 2))
                {
                    Array.Copy(writeData, 0, data, 34, point * 2);
                }
                else
                {
                    Array.Copy(writeData, 0, data, 34, writeData.Length);
                }

                status = Socket.SendBytes(data);

                if (!status)
                {
                    errors(Socket.ErrorCode, Socket.ErrorMessage);
                }

                ReceiveBytes = new byte[0];
            }

            return status;
        }

        #endregion

        #region メモリエリアの書込応答

        /// <summary>
        /// PLCからメモリエリアの書込コマンドの返答を受信します。
        /// </summary>
        /// <returns></returns>
        public override byte[] WriteCommandAck()
        {
            int status = -1;
            byte[] temp;

            byte[] ret = new byte[0];

            if (Device == DeviceType.LAN)
            {
                temp = Socket.ReceiveBytes();

                if (temp.Length > 0)
                {
                    BL_Bytes.Join(temp, ref ReceiveBytes, temp.Length);
                }

                if (ReceiveBytes.Length >= 30)
                {
                    if (0x46494E53 == PLC_DATA.BytesToUInt32B(ReceiveBytes, 0))
                    {
                        if ((ReceiveBytes.Length - 8) >= PLC_DATA.BytesToUInt32B(ReceiveBytes, 4))
                        {
                            if (2 == PLC_DATA.BytesToUInt32B(ReceiveBytes, 8) && 0 == PLC_DATA.BytesToUInt32B(ReceiveBytes, 12))
                            {
                                if (ReceiveBytes[20] == ClientNode && ReceiveBytes[23] == ServerNode)
                                {
                                    status = (int)PLC_DATA.BytesToUInt32B(ReceiveBytes, 26);

                                    if (status == 0x01020000)
                                    {
                                        status = 0;
                                        ret = new byte[1];
                                        ret[0] = 0;
                                    }
                                    else
                                    {
                                        status &= 0x0000FFFF;
                                        ret = PLC_DATA.UInt16ToBytes((ushort)status);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return ret;
        }

        #endregion

        #region 接続状態プロパティ

        /// <summary>
        /// ソケットの接続状態を取得します。接続している場合は true。それ以外の場合は false。
        /// </summary>
        public override bool IsConnected
        {
            get
            {
                if (Device == DeviceType.LAN)
                {
                    return Socket.IsConnected;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion
    }

    #endregion

    #region PLC通信クラス（オムロン　Ｃモードコマンド用）

    /// <summary>
    /// PLC通信クラス（オムロン　Ｃコマンドモード用）
    /// </summary>
    public class OMRON : BL_PLC
    {
        #region オープン

        /// <summary>
        /// デバイスをオープンします。
        /// </summary>
        /// <param name="portNo">オープンするデバイスのポート№。</param>
        /// <param name="baudRate">オープンするデバイスのボーレート。</param>
        /// <param name="byteSize">オープンするデバイスのデータビット長。</param>
        /// <param name="parity">オープンするデバイスのパリティビット。</param>
        /// <param name="stopBits">オープンするデバイスのストップビット。</param>
        /// <returns>true:正常,false:異常</returns>
        public override bool Connect(BL_Rs232c.PortNo portNo, BL_Rs232c.BaudRate baudRate, BL_Rs232c.ByteSize byteSize, BL_Rs232c.Parity parity, BL_Rs232c.StopBits stopBits)
        {
            bool status;

            errors();

            status = Com.Open(portNo, baudRate, byteSize, parity, stopBits, BL_Rs232c.FormatType.CR);

            if (!status)
            {
                errors(Com.ErrorCode, Com.ErrorMessage);
            }
            else
            {
                Device = DeviceType.RS_232C;
                Command = CommandType.OMRON_C;
            }

            return status;
        }

        #endregion

        #region クローズ

        /// <summary>
        /// デバイスをクローズします。
        /// </summary>
        public void Close()
        {
            Com.Close();
        }

        #endregion

        #region 送信（バイト配列）

        /// <summary>
        /// デバイスにバイト配列のデータを書き込みます。
        /// </summary>
        /// <param name="data">書き込むデータが格納されているバイト配列。</param>
        /// <returns>true:正常,false:異常</returns>
        public bool Send(byte[] data)
        {
            int step = 0, length, pos = 0, wait = 0;
            byte[] join, bcc = new byte[2];
            bool status = false, loop = true;

            length = data.Length;

            while (loop)
            {
                switch (step)
                {
                    case 0:
                        if ((data.Length - pos) <= 120)
                        {
                            length = data.Length - pos;

                            this.bcc_set(data, pos, length, ref bcc);

                            join = new byte[length];

                            BL_Bytes.Copy(data, pos, join, 0, length);
                            BL_Bytes.Join(bcc, ref join, bcc.Length);
                            BL_Bytes.Join(Encoding.Default.GetBytes("*"), ref join, 1);

                            status = Com.SendBytes(join);

                            loop = false;
                            break;
                        }
                        else
                        {
                            length = 120;

                            this.bcc_set(data, pos, length, ref bcc);

                            join = new byte[length + bcc.Length];

                            BL_Bytes.Copy(data, pos, join, 0, length);
                            BL_Bytes.Copy(bcc, 0, join, length, bcc.Length);

                            status = Com.SendBytes(join);

                            pos += length;

                            wait = BL_Win32API.TickCount + 500;

                            step = 1;
                        }
                        break;

                    case 1:
                        if (wait >= BL_Win32API.TickCount)
                        {
                            join = Com.ReceiveBytes(out length);

                            if (join.Length == 0 && length == 0)
                            {
                                step = 0;
                            }
                        }
                        else
                        {
                            status = false;

                            loop = false;
                        }
                        break;
                }

                BL_Win32API.Sleep(0);
            }

            return status;
        }

        #endregion

        #region 受信（バイト配列）

        /// <summary>
        /// デバイスからバイト配列でデータを読み込みます。
        /// </summary>
        /// <param name="bytes_size">受信したバイト数。未受信のときは、-1。BCCが異常のときは、-2。</param>
        /// <returns>受信したデータのバイト配列</returns>
        public byte[] Receive(out int bytes_size)
        {
            int wait = 0;
            byte[] data, join = new byte[0];
            bool loop = true;

            bytes_size = 0;

            wait = BL_Win32API.TickCount;

            while (loop)
            {
                data = Com.ReceiveBytes();

                if (data.Length > 0)
                {
                    if (Encoding.Default.GetString(data, data.Length - 1, 1) == "*")
                    {
                        if (this.bcc_check(data, 0, data.Length - 3, data, data.Length - 3))
                        {
                            BL_Bytes.Join(data, ref join, data.Length - 3);

                            bytes_size = join.Length;

                            loop = false;
                        }
                        else
                        {
                            bytes_size = -2;

                            loop = false;
                        }

                    }
                    else
                    {
                        if (this.bcc_check(data, 0, data.Length - 2, data, data.Length - 2))
                        {
                            BL_Bytes.Join(data, ref join, data.Length - 2);

                            data = new byte[1];
                            data[0] = 0x0D;

                            Com.SendBytesDirect(data);
                        }
                        else
                        {
                            bytes_size = -2;

                            loop = false;
                        }
                    }

                    wait = BL_Win32API.TickCount + 500;
                }
                else
                {
                    if (wait <= BL_Win32API.TickCount)
                    {
                        bytes_size = -1;

                        loop = false;
                    }
                }

                BL_Win32API.Sleep(0);
            }

            return join;
        }

        #endregion

        #region BCCチェック

        /// <summary>
        /// BCCチェック
        /// </summary>
        /// <param name="data">チェックするデータ</param>
        /// <param name="pos">開始位置</param>
        /// <param name="size">チェックする長さ</param>
        /// <param name="bcc">BCCデータ</param>
        /// <param name="bcc_pos">BCC位置</param>
        /// <returns>true:正常,false:異常</returns>
        private bool bcc_check(byte[] data, int pos, int size, byte[] bcc, int bcc_pos)
        {
            int count;
            byte check;
            byte[] buff;

            check = data[pos];

            for (count = pos + 1; count < pos + size; count++)
            {
                check ^= data[count];
            }

            buff = Encoding.Default.GetBytes(String.Format("{0:X2}", check));

            if (0 == BL_Bytes.Compare(bcc, bcc_pos, buff, 0, 2)) return true;
            else return false;
        }

        #endregion

        #region BCCセット

        /// <summary>
        /// BCCセット
        /// </summary>
        /// <param name="data">チェックするデータ</param>
        /// <param name="pos">開始位置</param>
        /// <param name="size">チェックする長さ</param>
        /// <param name="bcc">BCCデータ</param>
        private void bcc_set(byte[] data, int pos, int size, ref byte[] bcc)
        {
            int count;
            byte check;
            byte[] buff;

            check = data[pos];

            for (count = pos + 1; count < pos + size; count++)
            {
                check ^= data[count];
            }

            buff = Encoding.Default.GetBytes(String.Format("{0:X2}", check));

            BL_Bytes.Copy(buff, 0, bcc, 0, 2);
        }

        #endregion
    }

    #endregion
}
