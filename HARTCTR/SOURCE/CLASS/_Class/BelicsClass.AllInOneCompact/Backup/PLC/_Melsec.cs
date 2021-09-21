using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Net;

using BelicsClass.Common;
using BelicsClass.Rs232c;
using BelicsClass.Network;

namespace BelicsClass.PLC
{
    #region PLC通信クラス（三菱用）

    /// <summary>
    /// PLC通信クラス（三菱用）
    /// </summary>
    public class MELSEC : BL_PLC
    {
        #region 接続

        /// <summary>
        /// PLCと接続します。
        /// </summary>
        /// <param name="commandType">PLCのコマンドタイプ。</param>
        /// <param name="portNo">PLCと接続するシリアルポート№。</param>
        /// <param name="baudRate">PLCと接続するシリアルポートのボーレート。</param>
        /// <param name="byteSize">PLCと接続するシリアルポートのデータビット長。</param>
        /// <param name="parity">PLCと接続するシリアルポートのパリティビット。</param>
        /// <param name="stopBits">PLCと接続するシリアルポートのストップビット。</param>
        /// <returns>true:正常,false:異常</returns>
        public override bool Connect(CommandType commandType, BL_Rs232c.PortNo portNo, BL_Rs232c.BaudRate baudRate, BL_Rs232c.ByteSize byteSize, BL_Rs232c.Parity parity, BL_Rs232c.StopBits stopBits)
        {
            bool status = false;

            errors();

            ReceiveBytes = new byte[0];

            if (Device == DeviceType.NONE)
            {
                Com = new BL_Rs232c();

                status = Com.Open(portNo, baudRate, byteSize, parity, stopBits, BL_Rs232c.FormatType.STX_ETX);

                if (status)
                {
                    Device = DeviceType.RS_232C;
                    Command = commandType;
                }
                else
                {
                    errors(Com.ErrorCode, Com.ErrorMessage);

                    Com = null;
                }
            }
            else
            {
                errors(-10000, "既にオープンされています。");
            }

            return status;
        }
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
        /// <summary>
        /// PLCと接続します。
        /// </summary>
        /// <param name="ip_address">接続するPLCのIPアドレス。(UDP)</param>
        /// <param name="port">接続するPLCのポート№。(UDP)</param>
        /// <param name="local_port">接続するポート№。(UDP)</param>
        /// <returns>true:正常,false:異常</returns>
        public override bool Connect(string ip_address, int port, int local_port)
        {
            bool status = false;

            errors();

            ReceiveBytes = new byte[0];

            if (Device == DeviceType.NONE)
            {
                SocketUDP = new BL_RawSocketUDP(local_port);

                status = SocketUDP.Open(BL_RawSocketUDP.FormatType.None);

                if (status)
                {
                    Device = DeviceType.LAN_UDP;

                    RemoteEP = new IPEndPoint(IPAddress.Parse(ip_address), port);
                }
                else
                {
                    errors(SocketUDP.ErrorCode, SocketUDP.ErrorMessage);

                    SocketUDP = null;
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
            if (Device == DeviceType.RS_232C)
            {
                Com.Close();

                Com = null;
            }
            else if (Device == DeviceType.LAN)
            {
                Socket.Close();

                Socket = null;
            }
            else if (Device == DeviceType.LAN_UDP)
            {
                SocketUDP.Close();

                SocketUDP = null;
            }

            Device = DeviceType.NONE;
        }

        #endregion

        #region 一括読出し

        /// <summary>
        /// PLCに一括読出しコマンドを送信します。
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

            if (Device == DeviceType.RS_232C)
            {
                if (this.is_bit_device(deviceCode))
                {
                    if (point < 1 || point > 256)
                    {
                        errors(-10005, "コマンドが不正です。");

                        return status;
                    }

                    if (Command == CommandType.MELSEC_A_CPU)
                    {
                        data = new byte[7 + 5 + 2];

                        Encoding.Default.GetBytes("00FFBR0", 0, 7, data, 0);

                        if (this.is_hex_address(deviceCode))
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("X4"), 0, 4, data, 8);
                        }
                        else
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("0000"), 0, 4, data, 8);
                        }

                        Encoding.Default.GetBytes((point & 0x00FF).ToString("X2"), 0, 2, data, 12);
                    }
                    else
                    {
                        data = new byte[7 + 7 + 2];

                        Encoding.Default.GetBytes("00FFJR0", 0, 7, data, 0);

                        if (this.is_hex_address(deviceCode))
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("X6"), 0, 6, data, 8);
                        }
                        else
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("000000"), 0, 6, data, 8);
                        }

                        Encoding.Default.GetBytes((point & 0x00FF).ToString("X2"), 0, 2, data, 14);
                    }
                }
                else
                {
                    if (point < 1 || point > 64)
                    {
                        errors(-10005, "コマンドが不正です。");

                        return status;
                    }

                    if (Command == CommandType.MELSEC_A_CPU)
                    {
                        data = new byte[7 + 5 + 2];

                        Encoding.Default.GetBytes("00FFWR0", 0, 7, data, 0);

                        if (this.is_hex_address(deviceCode))
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("X4"), 0, 4, data, 8);
                        }
                        else
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("0000"), 0, 4, data, 8);
                        }

                        Encoding.Default.GetBytes(point.ToString("X2"), 0, 2, data, 12);
                    }
                    else
                    {
                        data = new byte[7 + 7 + 2];

                        Encoding.Default.GetBytes("00FFQR0", 0, 7, data, 0);

                        if (this.is_hex_address(deviceCode))
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("X6"), 0, 6, data, 8);
                        }
                        else
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("000000"), 0, 6, data, 8);
                        }

                        Encoding.Default.GetBytes(point.ToString("X2"), 0, 2, data, 14);
                    }
                }

                status = Com.SendBytes(data);

                if (!status)
                {
                    errors(Com.ErrorCode, Com.ErrorMessage);
                }
            }
            else if (Device == DeviceType.LAN)
            {
                if (point < 1 || point > 960)
                {
                    errors(-10005, "コマンドが不正です。");

                    return status;
                }

                data = new byte[11 + 4 + 6];

                data[0] = 0x50;
                data[1] = 0x00;
                data[2] = 0x00;
                data[3] = 0xFF;
                data[4] = 0xFF;
                data[5] = 0x03;
                data[6] = 0x00;
                data[7] = 0x0C;
                data[8] = 0x00;
                data[9] = 0x00;
                data[10] = 0x00;

                data[11] = 0x01;
                data[12] = 0x04;
                data[13] = 0x00;
                data[14] = 0x00;

                data[15] = (byte)(offset & 0x000000FF);
                data[16] = (byte)((offset >> 8) & 0x000000FF);
                data[17] = (byte)((offset >> 16) & 0x000000FF);
                data[18] = deviceCode;
                data[19] = (byte)(point & 0x000000FF);
                data[20] = (byte)((point >> 8) & 0x000000FF);
                status = Socket.SendBytes(data);

                if (!status)
                {
                    errors(Socket.ErrorCode, Socket.ErrorMessage);
                }

                ReceiveBytes = new byte[0];
            }
            else if (Device == DeviceType.LAN_UDP)
            {
                if (point < 1 || point > 960)
                {
                    errors(-10005, "コマンドが不正です。");

                    return status;
                }

                data = new byte[11 + 4 + 6];

                data[0] = 0x50;
                data[1] = 0x00;
                data[2] = 0x00;
                data[3] = 0xFF;
                data[4] = 0xFF;
                data[5] = 0x03;
                data[6] = 0x00;
                data[7] = 0x0C;
                data[8] = 0x00;
                data[9] = 0x00;
                data[10] = 0x00;

                data[11] = 0x01;
                data[12] = 0x04;
                data[13] = 0x00;
                data[14] = 0x00;

                data[15] = (byte)(offset & 0x000000FF);
                data[16] = (byte)((offset >> 8) & 0x000000FF);
                data[17] = (byte)((offset >> 16) & 0x000000FF);
                data[18] = deviceCode;
                data[19] = (byte)(point & 0x000000FF);
                data[20] = (byte)((point >> 8) & 0x000000FF);
                status = SocketUDP.SendBytes(data, RemoteEP);

                if (!status)
                {
                    errors(SocketUDP.ErrorCode, SocketUDP.ErrorMessage);
                }

                ReceiveBytes = new byte[0];
            }

            return status;
        }

        #endregion

        #region 一括読出し応答

        /// <summary>
        /// PLCから一括読出しコマンドの返答を受信します。
        /// </summary>
        /// <param name="length">読み込んだデバイスのバイト数。</param>
        /// <returns>受信したデータのバイト配列</returns>
        public override byte[] ReadCommandAck(out int length)
        {
            byte[] data, temp;

            data = new byte[0];
            length = -1;

            if (Device == DeviceType.RS_232C)
            {
                data = Com.ReceiveBytes();

                if (data.Length >= 4)
                {
                    length = data.Length - 4;

                    temp = data;
                    data = new byte[length];
                    Array.Copy(temp, 4, data, 0, length);
                }

            }
            else if (Device == DeviceType.LAN)
            {
                data = Socket.ReceiveBytes();

                if (data.Length >= 11)
                {
                    length = (data[7] + data[8] * 0x100) - 2;
                    if (length != (data.Length - 11)) length = 0;

                    if (data[0] == 0xD0 && data[1] == 0x00 && data[9] == 0x00 && data[10] == 0x00)
                    {
                        temp = data;
                        data = new byte[length];
                        Array.Copy(temp, 11, data, 0, length);
                    }
                    else
                    {
                        length = 0;
                    }
                }
            }
            else if (Device == DeviceType.LAN_UDP)
            {
                data = SocketUDP.ReceiveBytes();

                if (data.Length >= 11)
                {
                    length = (data[7] + data[8] * 0x100) - 2;
                    if (length != (data.Length - 11)) length = 0;

                    if (data[0] == 0xD0 && data[1] == 0x00 && data[9] == 0x00 && data[10] == 0x00)
                    {
                        temp = data;
                        data = new byte[length];
                        Array.Copy(temp, 11, data, 0, length);
                    }
                    else
                    {
                        length = 0;
                    }
                }
            }

            return data;
        }
        /// <summary>
        /// PLCから一括読出しコマンドの返答を受信します。
        /// </summary>
        /// <param name="length">読み込んむデバイスのバイト数。</param>
        /// <returns>受信したデータのバイト配列</returns>
        public override byte[] ReadCommandAck(int length)
        {
            byte[] data, temp;

            data = new byte[0];

            if (Device == DeviceType.RS_232C)
            {
                temp = Com.ReceiveBytes();

                if (temp.Length >= 4)
                {
                    length = temp.Length - 4;

                    data = new byte[length];
                    Array.Copy(temp, 4, data, 0, length);
                }

            }
            else if (Device == DeviceType.LAN)
            {
                temp = Socket.ReceiveBytes();

                if (temp.Length > 0)
                {
                    BL_Bytes.Join(temp, ref ReceiveBytes, temp.Length);
                }

                if (ReceiveBytes.Length >= 11)
                {
                    if (((ReceiveBytes[7] + ReceiveBytes[8] * 0x100) - 2) == length)
                    {
                        if (ReceiveBytes[0] == 0xD0 && ReceiveBytes[1] == 0x00 && ReceiveBytes[9] == 0x00 && ReceiveBytes[10] == 0x00)
                        {
                            data = new byte[length];
                            Array.Copy(ReceiveBytes, 11, data, 0, length);
                        }
                    }
                }
            }
            else if (Device == DeviceType.LAN_UDP)
            {
                temp = SocketUDP.ReceiveBytes();

                if (temp.Length > 0)
                {
                    BL_Bytes.Join(temp, ref ReceiveBytes, temp.Length);
                }

                if (ReceiveBytes.Length >= 11)
                {
                    if (((ReceiveBytes[7] + ReceiveBytes[8] * 0x100) - 2) == length)
                    {
                        if (ReceiveBytes[0] == 0xD0 && ReceiveBytes[1] == 0x00 && ReceiveBytes[9] == 0x00 && ReceiveBytes[10] == 0x00)
                        {
                            data = new byte[length];
                            Array.Copy(ReceiveBytes, 11, data, 0, length);
                        }
                    }
                }
            }

            return data;
        }

        #endregion

        #region 一括書込み

        /// <summary>
        /// PLCに一括書込みコマンドを送信します。
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

            if (Device == DeviceType.RS_232C)
            {
                if (this.is_bit_device(deviceCode))
                {
                    if (point < 1 || point > 160)
                    {
                        errors(-10005, "コマンドが不正です。");

                        return status;
                    }

                    if (Command == CommandType.MELSEC_A_CPU)
                    {
                        data = new byte[7 + 5 + 2 + point];

                        Encoding.Default.GetBytes("00FFBW0", 0, 7, data, 0);

                        if (this.is_hex_address(deviceCode))
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("X4"), 0, 4, data, 8);
                        }
                        else
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("0000"), 0, 4, data, 8);
                        }

                        Encoding.Default.GetBytes((point & 0x00FF).ToString("X2"), 0, 2, data, 12);

                        if (writeData.Length > point)
                        {
                            BL_Bytes.Copy(writeData, 0, data, 14, point);
                        }
                        else
                        {
                            BL_Bytes.Clear((byte)'0', data, 14, point);
                            BL_Bytes.Copy(writeData, 0, data, 14, writeData.Length);
                        }
                    }
                    else
                    {
                        data = new byte[7 + 7 + 2 + point];

                        Encoding.Default.GetBytes("00FFJW0", 0, 7, data, 0);

                        if (this.is_hex_address(deviceCode))
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("X6"), 0, 6, data, 8);
                        }
                        else
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("000000"), 0, 6, data, 8);
                        }

                        Encoding.Default.GetBytes((point & 0x00FF).ToString("X2"), 0, 2, data, 14);

                        if (writeData.Length > point)
                        {
                            BL_Bytes.Copy(writeData, 0, data, 16, point);
                        }
                        else
                        {
                            BL_Bytes.Clear((byte)'0', data, 16, point);
                            BL_Bytes.Copy(writeData, 0, data, 16, writeData.Length);
                        }
                    }
                }
                else
                {
                    if (point < 1 || point > 64)
                    {
                        errors(-10005, "コマンドが不正です。");

                        return status;
                    }

                    if (Command == CommandType.MELSEC_A_CPU)
                    {
                        data = new byte[7 + 5 + 2 + point * 4];

                        Encoding.Default.GetBytes("00FFWW0", 0, 7, data, 0);

                        if (this.is_hex_address(deviceCode))
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("X4"), 0, 4, data, 8);
                        }
                        else
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("0000"), 0, 4, data, 8);
                        }

                        Encoding.Default.GetBytes((point & 0x00FF).ToString("X2"), 0, 2, data, 12);

                        if (writeData.Length > (point * 4))
                        {
                            BL_Bytes.Copy(writeData, 0, data, 14, point * 4);
                        }
                        else
                        {
                            BL_Bytes.Clear((byte)'0', data, 14, point * 4);
                            BL_Bytes.Copy(writeData, 0, data, 14, writeData.Length);
                        }
                    }
                    else
                    {
                        data = new byte[7 + 7 + 2 + point * 4];

                        Encoding.Default.GetBytes("00FFQW0", 0, 7, data, 0);

                        if (this.is_hex_address(deviceCode))
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("X6"), 0, 6, data, 8);
                        }
                        else
                        {
                            data[7] = deviceCode;
                            Encoding.Default.GetBytes(offset.ToString("000000"), 0, 6, data, 8);
                        }

                        Encoding.Default.GetBytes((point & 0x00FF).ToString("X2"), 0, 2, data, 14);

                        if (writeData.Length > (point * 4))
                        {
                            BL_Bytes.Copy(writeData, 0, data, 16, point * 4);
                        }
                        else
                        {
                            BL_Bytes.Clear((byte)'0', data, 16, point * 4);
                            BL_Bytes.Copy(writeData, 0, data, 16, writeData.Length);
                        }
                    }
                }

                status = Com.SendBytes(data);

                if (!status)
                {
                    errors(Com.ErrorCode, Com.ErrorMessage);
                }
            }
            else if (Device == DeviceType.LAN)
            {
                if (point < 1 || point > 960)
                {
                    errors(-10005, "コマンドが不正です。");

                    return status;
                }

                data = new byte[11 + 4 + 6 + point * 2];

                data[0] = 0x50;
                data[1] = 0x00;
                data[2] = 0x00;
                data[3] = 0xFF;
                data[4] = 0xFF;
                data[5] = 0x03;
                data[6] = 0x00;
                data[7] = (byte)((12 + point * 2) & 0x000000FF);
                data[8] = (byte)(((12 + point * 2) >> 8) & 0x000000FF);
                data[9] = 0x00;
                data[10] = 0x00;

                data[11] = 0x01;
                data[12] = 0x14;
                data[13] = 0x00;
                data[14] = 0x00;

                data[15] = (byte)(offset & 0x000000FF);
                data[16] = (byte)((offset >> 8) & 0x000000FF);
                data[17] = (byte)((offset >> 16) & 0x000000FF);
                data[18] = deviceCode;
                data[19] = (byte)(point & 0x000000FF);
                data[20] = (byte)((point >> 8) & 0x000000FF);

                if (writeData.Length > (point * 2))
                {
                    Array.Copy(writeData, 0, data, 21, point * 2);
                }
                else
                {
                    Array.Copy(writeData, 0, data, 21, writeData.Length);
                }

                status = Socket.SendBytes(data);

                if (!status)
                {
                    errors(Socket.ErrorCode, Socket.ErrorMessage);
                }

                ReceiveBytes = new byte[0];
            }
            else if (Device == DeviceType.LAN_UDP)
            {
                if (point < 1 || point > 960)
                {
                    errors(-10005, "コマンドが不正です。");

                    return status;
                }

                data = new byte[11 + 4 + 6 + point * 2];

                data[0] = 0x50;
                data[1] = 0x00;
                data[2] = 0x00;
                data[3] = 0xFF;
                data[4] = 0xFF;
                data[5] = 0x03;
                data[6] = 0x00;
                data[7] = (byte)((12 + point * 2) & 0x000000FF);
                data[8] = (byte)(((12 + point * 2) >> 8) & 0x000000FF);
                data[9] = 0x00;
                data[10] = 0x00;

                data[11] = 0x01;
                data[12] = 0x14;
                data[13] = 0x00;
                data[14] = 0x00;

                data[15] = (byte)(offset & 0x000000FF);
                data[16] = (byte)((offset >> 8) & 0x000000FF);
                data[17] = (byte)((offset >> 16) & 0x000000FF);
                data[18] = deviceCode;
                data[19] = (byte)(point & 0x000000FF);
                data[20] = (byte)((point >> 8) & 0x000000FF);

                if (writeData.Length > (point * 2))
                {
                    Array.Copy(writeData, 0, data, 21, point * 2);
                }
                else
                {
                    Array.Copy(writeData, 0, data, 21, writeData.Length);
                }

                status = SocketUDP.SendBytes(data, RemoteEP);

                if (!status)
                {
                    errors(SocketUDP.ErrorCode, SocketUDP.ErrorMessage);
                }

                ReceiveBytes = new byte[0];
            }

            return status;
        }

        #endregion

        #region 一括書込み応答

        /// <summary>
        /// PLCから一括書込みコマンドの返答を受信します。
        /// </summary>
        /// <param name="length">読み込んだ返答データのバイト数。</param>
        /// <returns>受信したデータのバイト配列</returns>
        public override byte[] WriteCommandAck(out int length)
        {
            byte[] data, temp;

            data = new byte[0];
            length = -1;

            if (Device == DeviceType.RS_232C)
            {
                data = Com.ReceiveBytes();

                if (data.Length >= 6)
                {
                    length = data.Length - 4;

                    temp = data;
                    data = new byte[length];
                    Array.Copy(temp, 4, data, 0, length);
                }
            }
            else if (Device == DeviceType.LAN)
            {
                data = Socket.ReceiveBytes();

                if (data.Length >= 11)
                {
                    length = (data[7] + data[8] * 0x100) - 2;
                    if (length != (data.Length - 11)) length = 0;

                    if (data[0] == 0xD0 && data[1] == 0x00 && data[9] == 0x00 && data[10] == 0x00)
                    {
                        temp = data;
                        data = new byte[length];
                        Array.Copy(temp, 11, data, 0, length);
                    }
                    else
                    {
                        length = 2;

                        temp = data;
                        data = new byte[length];
                        Array.Copy(temp, 9, data, 0, length);
                    }
                }
            }
            else if (Device == DeviceType.LAN_UDP)
            {
                data = SocketUDP.ReceiveBytes();

                if (data.Length >= 11)
                {
                    length = (data[7] + data[8] * 0x100) - 2;
                    if (length != (data.Length - 11)) length = 0;

                    if (data[0] == 0xD0 && data[1] == 0x00 && data[9] == 0x00 && data[10] == 0x00)
                    {
                        temp = data;
                        data = new byte[length];
                        Array.Copy(temp, 11, data, 0, length);
                    }
                    else
                    {
                        length = 2;

                        temp = data;
                        data = new byte[length];
                        Array.Copy(temp, 9, data, 0, length);
                    }
                }
            }

            return data;
        }
        /// <summary>
        /// PLCから一括書込みコマンドの返答を受信します。
        /// </summary>
        /// <returns>受信したデータのバイト配列</returns>
        public override byte[] WriteCommandAck()
        {
            int length;
            byte[] data, temp;

            data = new byte[0];

            if (Device == DeviceType.RS_232C)
            {
                temp = Com.ReceiveBytes();

                if (temp.Length >= 6)
                {
                    length = temp.Length - 4;

                    data = new byte[length];
                    Array.Copy(temp, 4, data, 0, length);
                }
            }
            else if (Device == DeviceType.LAN)
            {
                temp = Socket.ReceiveBytes();

                if (temp.Length > 0)
                {
                    BL_Bytes.Join(temp, ref ReceiveBytes, temp.Length);
                }

                if (ReceiveBytes.Length >= 11)
                {
                    length = (ReceiveBytes[7] + ReceiveBytes[8] * 0x100);

                    if (length == (ReceiveBytes.Length - 9))
                    {
                        if (ReceiveBytes[0] == 0xD0 && ReceiveBytes[1] == 0x00)
                        {
                            data = new byte[length];
                            Array.Copy(ReceiveBytes, 9, data, 0, data.Length);
                        }
                    }
                }
            }
            else if (Device == DeviceType.LAN_UDP)
            {
                temp = SocketUDP.ReceiveBytes();

                if (temp.Length > 0)
                {
                    BL_Bytes.Join(temp, ref ReceiveBytes, temp.Length);
                }

                if (ReceiveBytes.Length >= 11)
                {
                    length = (ReceiveBytes[7] + ReceiveBytes[8] * 0x100);

                    if (length == (ReceiveBytes.Length - 9))
                    {
                        if (ReceiveBytes[0] == 0xD0 && ReceiveBytes[1] == 0x00)
                        {
                            data = new byte[length];
                            Array.Copy(ReceiveBytes, 9, data, 0, data.Length);
                        }
                    }
                }
            }

            return data;
        }

        #endregion

        #region ビットデバイスのチェック

        /// <summary>
        /// ビットデバイスのチェック
        /// </summary>
        /// <param name="deviceCode">デバイスコード</param>
        /// <returns>true:ビットデバイス,false:ワードデバイス</returns>
        private bool is_bit_device(byte deviceCode)
        {
            Regex regex = new Regex("X|Y|M|L|S|B");

            return regex.IsMatch(Encoding.Default.GetString(new byte[] { deviceCode }));
        }

        #endregion

        #region 16進表現のチェック

        /// <summary>
        /// 16進表現のチェック
        /// </summary>
        /// <param name="deviceCode">デバイスコード</param>
        /// <returns>true:16進表現,false:10進表現</returns>
        private bool is_hex_address(byte deviceCode)
        {
            Regex regex = new Regex("X|Y|B|W");

            return regex.IsMatch(Encoding.Default.GetString(new byte[] { deviceCode }));
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
                if (Device == DeviceType.LAN_UDP)
                {
                    return true;
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

    #region PLC通信クラス 簡易版（三菱用）

    /// <summary>
    /// PLC通信クラス 簡易版（三菱用）
    /// </summary>
    public class MELSECez
    {
        private MELSEC Plc = new MELSEC();
        private byte ReadDeviceCode = 0x00;
        private int ReadOffset = 0;
        private int ReadPoint = 0;
        private byte WriteDeviceCode = 0x00;
        private int WriteOffset = 0;
        private int WritePoint = 0;
        private int Step = 0;
        private int TimeOut = 0;
        private int Counter = 0;

        #region 接続

        /// <summary>
        /// PLCと接続します。
        /// </summary>
        /// <param name="ipAddress">接続するPLCのIPアドレス。</param>
        /// <param name="port">接続するPLCのポート№。</param>
        /// <param name="readDeviceCode">読み込むデバイスのデバイスコード。</param>
        /// <param name="readOffset">読み込むデバイスのオフセット値。</param>
        /// <param name="readPoint">読み込むデバイスの点数。</param>
        /// <param name="writeDeviceCode">書き込むデバイスのデバイスコード。</param>
        /// <param name="writeOffset">書き込むデバイスのオフセット値。</param>
        /// <param name="writePoint">書き込むデバイスのデバイスコード。</param>
        /// <returns>true:正常,false:異常</returns>
        public bool Connect(string ipAddress, int port, byte readDeviceCode, int readOffset, int readPoint, byte writeDeviceCode, int writeOffset, int writePoint)
        {
            ReadDeviceCode = readDeviceCode;
            ReadOffset = readOffset;
            ReadPoint = readPoint;
            WriteDeviceCode = writeDeviceCode;
            WriteOffset = writeOffset;
            WritePoint = writePoint;

            Step = 0;
            Counter = 0;

            return Plc.Connect(ipAddress, port);
        }

        #endregion

        #region 接続解除

        /// <summary>
        /// PLCとの接続を解除します。
        /// </summary>
        public void Disconnect()
        {
            Plc.Disconnect();
        }

        #endregion

        #region 通信

        /// <summary>
        /// PLCと通信します。
        /// </summary>
        /// <param name="readData">読み込んだデバイスのデータを格納するバイト配列。</param>
        /// <param name="writeData">書き込むデバイスのデータが格納されているバイト配列。</param>
        /// <returns>true:正常,false:異常</returns>
        public bool Call(ref byte[] readData, byte[] writeData)
        {
            byte[] data;
            bool status = true;

            if (Plc.IsConnected)
            {
                switch (Step)
                {
                    case 0:
                        status = Plc.ReadCommand(ReadDeviceCode, ReadOffset, ReadPoint);

                        TimeOut = BL_Win32API.TickCount + 3000;

                        Step = 1;
                        break;

                    case 1:
                        data = Plc.ReadCommandAck(readData.Length);

                        if (data.Length > 0)
                        {
                            readData = data;

                            status = Plc.WriteCommand(WriteDeviceCode, WriteOffset, WritePoint, writeData);

                            TimeOut = BL_Win32API.TickCount + 3000;

                            Step = 2;
                        }
                        else
                        {
                            if (TimeOut <= BL_Win32API.TickCount)
                            {
                                status = false;
                            }
                        }
                        break;

                    case 2:
                        data = Plc.WriteCommandAck();

                        if (data.Length > 0)
                        {
                            status = Plc.ReadCommand(ReadDeviceCode, ReadOffset, ReadPoint);

                            TimeOut = BL_Win32API.TickCount + 3000;

                            Step = 1;

                            Counter++;
                        }
                        else
                        {
                            if (TimeOut <= BL_Win32API.TickCount)
                            {
                                status = false;
                            }
                        }
                        break;
                }
            }
            else
            {
                status = false;
            }

            return status;
        }

        #endregion

        #region 応答カウンター プロパティ

        /// <summary>
        /// 応答カウンター
        /// </summary>
        public int ResponseCounter
        {
            get
            {
                return Counter;
            }
        }

        #endregion
    }

    #endregion

    #region PLC通信クラス（三菱用[Ｆシリーズ]）

    /// <summary>
    /// PLC通信クラス（三菱用[Ｆシリーズ]）
    /// </summary>
    public class MELSEC_F : BL_PLC
    {
        #region 列挙型

        /// <summary>
        /// デバイスコード
        /// </summary>
        public enum DeviceCode : ushort
        {
            /// <summary>
            /// データレジスタ
            /// </summary>
            D = 0x4420,
            /// <summary>
            /// 拡張レジスタ
            /// </summary>
            R = 0x5220,
        }

        #endregion

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

        #region 一括読出し

        /// <summary>
        /// PLCに一括読出しコマンドを送信します。
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
                if (point < 1 || point > 64)
                {
                    errors(-10005, "コマンドが不正です。");

                    return status;
                }

                data = new byte[4 + 8];

                data[0] = 0x01;
                data[1] = 0xFF;
                data[2] = 0x00;
                data[3] = 0x00;

                data[4] = (byte)(offset & 0x000000FF);
                data[5] = (byte)((offset >> 8) & 0x000000FF);
                data[6] = (byte)((offset >> 16) & 0x000000FF);
                data[7] = (byte)((offset >> 24) & 0x000000FF);

                DeviceCode dev = 0;
                if (deviceCode == 0x00) dev = DeviceCode.D;
                if (deviceCode == 0x01) dev = DeviceCode.R;

                data[8] = (byte)((ushort)dev & 0x000000FF);
                data[9] = (byte)(((ushort)dev >> 8) & 0x000000FF);

                data[10] = (byte)(point & 0x000000FF);
                data[11] = (byte)((point >> 8) & 0x000000FF);

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

        #region 一括読出し応答

        /// <summary>
        /// PLCから一括読出しコマンドの返答を受信します。
        /// </summary>
        /// <param name="length">読み込んむデバイスのバイト数。</param>
        /// <returns>受信したデータのバイト配列</returns>
        public override byte[] ReadCommandAck(int length)
        {
            byte[] data, temp;

            data = new byte[0];

            if (Device == DeviceType.LAN)
            {
                temp = Socket.ReceiveBytes();

                if (temp.Length > 0)
                {
                    BL_Bytes.Join(temp, ref ReceiveBytes, temp.Length);
                }

                if (ReceiveBytes.Length >= 2)
                {
                    if (ReceiveBytes[0] == 0x81 && ReceiveBytes[1] == 0x00 && ReceiveBytes.Length == (2 + length))
                    {
                        data = new byte[length];
                        Array.Copy(ReceiveBytes, 2, data, 0, length);
                    }
                }
            }

            return data;
        }

        #endregion

        #region 一括書込み

        /// <summary>
        /// PLCに一括書込みコマンドを送信します。
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
                if (point < 1 || point > 64)
                {
                    errors(-10005, "コマンドが不正です。");

                    return status;
                }

                data = new byte[4 + 8 + point * 2];

                data[0] = 0x03;
                data[1] = 0xFF;
                data[2] = 0x00;
                data[3] = 0x00;

                data[4] = (byte)(offset & 0x000000FF);
                data[5] = (byte)((offset >> 8) & 0x000000FF);
                data[6] = (byte)((offset >> 16) & 0x000000FF);
                data[7] = (byte)((offset >> 24) & 0x000000FF);

                DeviceCode dev = 0;
                if (deviceCode == 0x00) dev = DeviceCode.D;
                if (deviceCode == 0x01) dev = DeviceCode.R;

                data[8] = (byte)((ushort)dev & 0x000000FF);
                data[9] = (byte)(((ushort)dev >> 8) & 0x000000FF);
                data[10] = (byte)(point & 0x000000FF);
                data[11] = (byte)((point >> 8) & 0x000000FF);

                if (writeData.Length > (point * 2))
                {
                    Array.Copy(writeData, 0, data, 12, point * 2);
                }
                else
                {
                    Array.Copy(writeData, 0, data, 12, writeData.Length);
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

        #region 一括書込み応答

        /// <summary>
        /// PLCから一括書込みコマンドの返答を受信します。
        /// </summary>
        /// <returns>受信したデータのバイト配列</returns>
        public override byte[] WriteCommandAck()
        {
            byte[] data;

            data = new byte[0];

            if (Device == DeviceType.LAN)
            {
                data = Socket.ReceiveBytes();

                if (data.Length > 0)
                {
                    BL_Bytes.Join(data, ref ReceiveBytes, data.Length);
                }

                if (ReceiveBytes.Length >= 2)
                {
                    if (ReceiveBytes[0] == 0x83)
                    {
                        if (ReceiveBytes[1] == 0x00)
                        {
                            data = new byte[1];
                            Array.Copy(ReceiveBytes, 1, data, 0, data.Length);
                        }
                        else if (ReceiveBytes[1] == 0x5B)
                        {
                            if (ReceiveBytes.Length == 4)
                            {
                                data = new byte[3];
                                Array.Copy(ReceiveBytes, 1, data, 0, data.Length);
                            }
                        }
                        else
                        {
                            if (ReceiveBytes.Length == 3)
                            {
                                data = new byte[2];
                                Array.Copy(ReceiveBytes, 1, data, 0, data.Length);
                            }
                        }
                    }
                }
            }

            return data;
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

    #region PLC通信クラス 簡易版（三菱用[Ｆシリーズ]）

    /// <summary>
    /// PLC通信クラス 簡易版（三菱用[Ｆシリーズ]）
    /// </summary>
    public class MELSEC_Fez
    {
        private MELSEC_F Plc = new MELSEC_F();
        private MELSEC_F.DeviceCode ReadDeviceCode;
        private int ReadOffset = 0;
        private int ReadPoint = 0;
        private MELSEC_F.DeviceCode WriteDeviceCode;
        private int WriteOffset = 0;
        private int WritePoint = 0;
        private int Step = 0;
        private int TimeOut = 0;
        private int Counter = 0;

        #region 接続

        /// <summary>
        /// PLCと接続します。
        /// </summary>
        /// <param name="ipAddress">接続するPLCのIPアドレス。</param>
        /// <param name="port">接続するPLCのポート№。</param>
        /// <param name="readDeviceCode">読み込むデバイスのデバイスコード。</param>
        /// <param name="readOffset">読み込むデバイスのオフセット値。</param>
        /// <param name="readPoint">読み込むデバイスの点数。</param>
        /// <param name="writeDeviceCode">書き込むデバイスのデバイスコード。</param>
        /// <param name="writeOffset">書き込むデバイスのオフセット値。</param>
        /// <param name="writePoint">書き込むデバイスの点数。</param>
        /// <returns>true:正常,false:異常</returns>
        public bool Connect(string ipAddress, int port, MELSEC_F.DeviceCode readDeviceCode, int readOffset, int readPoint, MELSEC_F.DeviceCode writeDeviceCode, int writeOffset, int writePoint)
        {
            ReadDeviceCode = readDeviceCode;
            ReadOffset = readOffset;
            ReadPoint = readPoint;
            WriteDeviceCode = writeDeviceCode;
            WriteOffset = writeOffset;
            WritePoint = writePoint;

            Step = 0;
            Counter = 0;

            return Plc.Connect(ipAddress, port);
        }

        #endregion

        #region 接続解除

        /// <summary>
        /// PLCとの接続を解除します。
        /// </summary>
        public void Disconnect()
        {
            Plc.Disconnect();
        }

        #endregion

        #region 通信

        /// <summary>
        /// PLCと通信します。
        /// </summary>
        /// <param name="readData">読み込んだデバイスのデータを格納するバイト配列。</param>
        /// <param name="writeData">書き込むデバイスのデータが格納されているバイト配列。</param>
        /// <returns>true:正常,false:異常</returns>
        public bool Call(ref byte[] readData, byte[] writeData)
        {
            byte[] data;
            bool status = true;

            if (Plc.IsConnected)
            {
                switch (Step)
                {
                    case 0:
                        if (ReadDeviceCode == MELSEC_F.DeviceCode.D) status = Plc.ReadCommand(0x00, ReadOffset, ReadPoint);
                        if (ReadDeviceCode == MELSEC_F.DeviceCode.R) status = Plc.ReadCommand(0x01, ReadOffset, ReadPoint);

                        TimeOut = BL_Win32API.TickCount + 3000;

                        Step = 1;
                        break;

                    case 1:
                        data = Plc.ReadCommandAck(readData.Length);

                        if (data.Length > 0)
                        {
                            readData = data;

                            if (WriteDeviceCode == MELSEC_F.DeviceCode.D) status = Plc.WriteCommand(0x00, WriteOffset, WritePoint, writeData);
                            if (WriteDeviceCode == MELSEC_F.DeviceCode.R) status = Plc.WriteCommand(0x01, WriteOffset, WritePoint, writeData);

                            TimeOut = BL_Win32API.TickCount + 3000;

                            Step = 2;
                        }
                        else
                        {
                            if (TimeOut <= BL_Win32API.TickCount)
                            {
                                status = false;
                            }
                        }
                        break;

                    case 2:
                        data = Plc.WriteCommandAck();

                        if (data.Length > 0)
                        {
                            if (ReadDeviceCode == MELSEC_F.DeviceCode.D) status = Plc.ReadCommand(0x00, ReadOffset, ReadPoint);
                            if (ReadDeviceCode == MELSEC_F.DeviceCode.R) status = Plc.ReadCommand(0x01, ReadOffset, ReadPoint);

                            TimeOut = BL_Win32API.TickCount + 3000;

                            Step = 1;

                            Counter++;
                        }
                        else
                        {
                            if (TimeOut <= BL_Win32API.TickCount)
                            {
                                status = false;
                            }
                        }
                        break;
                }
            }
            else
            {
                status = false;
            }

            return status;
        }

        #endregion

        #region 応答カウンター プロパティ

        /// <summary>
        /// 応答カウンター
        /// </summary>
        public int ResponseCounter
        {
            get
            {
                return Counter;
            }
        }

        #endregion
    }

    #endregion

    #region PLC通信クラス 簡易版（三菱用[1Eフレーム]） ※PLC通信クラス 簡易版（三菱用[Ｆシリーズ]）を使用して下さい。
    /// <summary>
    /// PLC通信クラス 簡易版（三菱用[1Eフレーム]）
    /// </summary>
    public class MELSECez1E
    {
        private BL_RawSocket Socket = null;
        private bool Device = false;
        private ushort ReadDeviceCode = 0x0000;
        private int ReadOffset = 0;
        private int ReadPoint = 0;
        private ushort WriteDeviceCode = 0x0000;
        private int WriteOffset = 0;
        private int WritePoint = 0;
        private int Step = 0;
        private int TimeOut = 0;
        private int Error_code;
        private string Error_message;
        private byte[] ReceiveBytes = new byte[0];

        #region 接続

        /// <summary>
        /// PLCと接続します。
        /// </summary>
        /// <param name="ipAddress">接続するPLCのIPアドレス。</param>
        /// <param name="port">接続するPLCのポート№。</param>
        /// <param name="readDeviceCode">読み込むデバイスのデバイスコード。</param>
        /// <param name="readOffset">読み込むデバイスのオフセット値。</param>
        /// <param name="readPoint">読み込むデバイスの点数。</param>
        /// <param name="writeDeviceCode">書き込むデバイスのデバイスコード。</param>
        /// <param name="writeOffset">書き込むデバイスのオフセット値。</param>
        /// <param name="writePoint">書き込むデバイスの点数。</param>
        /// <returns>true:正常,false:異常</returns>
        public bool Connect(string ipAddress, int port, ushort readDeviceCode, int readOffset, int readPoint, ushort writeDeviceCode, int writeOffset, int writePoint)
        {
            bool status = false;

            errors();

            ReceiveBytes = new byte[0];

            ReadDeviceCode = readDeviceCode;
            ReadOffset = readOffset;
            ReadPoint = readPoint;
            WriteDeviceCode = writeDeviceCode;
            WriteOffset = writeOffset;
            WritePoint = writePoint;

            Step = 0;

            if (!Device)
            {
                Socket = new BL_RawSocket(ipAddress, port);

                status = Socket.Open(BL_RawSocket.OpenMode.Client, BL_RawSocket.FormatType.None);

                if (status)
                {
                    Device = true;
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
        public void Disconnect()
        {
            if (Device)
            {
                Socket.Close();

                Socket = null;
            }

            Device = false;
        }

        #endregion

        #region 通信

        /// <summary>
        /// PLCと通信します。
        /// </summary>
        /// <param name="readData">読み込んだデバイスのデータを格納するバイト配列。</param>
        /// <param name="writeData">書き込むデバイスのデータが格納されているバイト配列。</param>
        /// <returns>true:正常,false:異常</returns>
        public bool Call(ref byte[] readData, byte[] writeData)
        {
            byte[] data;
            bool status = true;

            if (Socket.IsConnected)
            {
                switch (Step)
                {
                    case 0:
                        status = ReadCommand(ReadDeviceCode, ReadOffset, ReadPoint);

                        TimeOut = BL_Win32API.TickCount + 3000;

                        Step = 1;
                        break;

                    case 1:
                        data = ReadCommandAck(readData.Length);

                        if (data.Length > 0)
                        {
                            readData = data;

                            status = WriteCommand(WriteDeviceCode, WriteOffset, WritePoint, writeData);

                            TimeOut = BL_Win32API.TickCount + 3000;

                            Step = 2;
                        }
                        else
                        {
                            if (TimeOut <= BL_Win32API.TickCount)
                            {
                                status = false;
                            }
                        }
                        break;

                    case 2:
                        data = WriteCommandAck();

                        if (data.Length > 0)
                        {
                            status = ReadCommand(ReadDeviceCode, ReadOffset, ReadPoint);

                            TimeOut = BL_Win32API.TickCount + 3000;

                            Step = 1;
                        }
                        else
                        {
                            if (TimeOut <= BL_Win32API.TickCount)
                            {
                                status = false;
                            }
                        }
                        break;
                }
            }
            else
            {
                status = false;
            }

            return status;
        }

        #endregion

        #region 一括読出し

        /// <summary>
        /// PLCに一括読出しコマンドを送信します。
        /// </summary>
        /// <param name="deviceCode">読み込むデバイスのデバイスコード。</param>
        /// <param name="offset">読み込むデバイスのオフセット値。</param>
        /// <param name="point">読み込むデバイスの点数。</param>
        /// <returns>true:正常,false:異常</returns>
        private bool ReadCommand(ushort deviceCode, int offset, int point)
        {
            byte[] data;
            bool status = false;

            if (Device)
            {
                if (point < 1 || point > 64)
                {
                    errors(-10005, "コマンドが不正です。");

                    return status;
                }

                data = new byte[4 + 8];

                data[0] = 0x01;
                data[1] = 0xFF;
                data[2] = 0x00;
                data[3] = 0x00;

                data[4] = (byte)(offset & 0x000000FF);
                data[5] = (byte)((offset >> 8) & 0x000000FF);
                data[6] = (byte)((offset >> 16) & 0x000000FF);
                data[7] = (byte)((offset >> 24) & 0x000000FF);
                data[8] = (byte)(deviceCode & 0x000000FF);
                data[9] = (byte)((deviceCode >> 8) & 0x000000FF);
                data[10] = (byte)(point & 0x000000FF);
                data[11] = (byte)((point >> 8) & 0x000000FF);

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

        #region 一括読出し応答

        /// <summary>
        /// PLCから一括読出しコマンドの返答を受信します。
        /// </summary>
        /// <param name="length">読み込んむデバイスのバイト数。</param>
        /// <returns>受信したデータのバイト配列</returns>
        private byte[] ReadCommandAck(int length)
        {
            byte[] data, temp;

            data = new byte[0];

            if (Device)
            {
                temp = Socket.ReceiveBytes();

                if (temp.Length > 0)
                {
                    BL_Bytes.Join(temp, ref ReceiveBytes, temp.Length);
                }

                if (ReceiveBytes.Length >= 2)
                {
                    if (ReceiveBytes[0] == 0x81 && ReceiveBytes[1] == 0x00 && ReceiveBytes.Length == (2 + length))
                    {
                        data = new byte[length];
                        Array.Copy(ReceiveBytes, 2, data, 0, length);
                    }
                }
            }

            return data;
        }

        #endregion

        #region 一括書込み

        /// <summary>
        /// PLCに一括書込みコマンドを送信します。
        /// </summary>
        /// <param name="deviceCode">書き込むデバイスのデバイスコード。</param>
        /// <param name="offset">書き込むデバイスのオフセット値。</param>
        /// <param name="point">書き込むデバイスの点数。</param>
        /// <param name="writeData">書き込むデバイスのデータが格納されているバイト配列。</param>
        /// <returns>true:正常,false:異常</returns>
        private bool WriteCommand(ushort deviceCode, int offset, int point, byte[] writeData)
        {
            byte[] data;
            bool status = false;

            errors();

            if (Device)
            {
                if (point < 1 || point > 64)
                {
                    errors(-10005, "コマンドが不正です。");

                    return status;
                }

                data = new byte[4 + 8 + point * 2];

                data[0] = 0x03;
                data[1] = 0xFF;
                data[2] = 0x00;
                data[3] = 0x00;

                data[4] = (byte)(offset & 0x000000FF);
                data[5] = (byte)((offset >> 8) & 0x000000FF);
                data[6] = (byte)((offset >> 16) & 0x000000FF);
                data[7] = (byte)((offset >> 24) & 0x000000FF);
                data[8] = (byte)(deviceCode & 0x000000FF);
                data[9] = (byte)((deviceCode >> 8) & 0x000000FF);
                data[10] = (byte)(point & 0x000000FF);
                data[11] = (byte)((point >> 8) & 0x000000FF);

                if (writeData.Length > (point * 2))
                {
                    Array.Copy(writeData, 0, data, 12, point * 2);
                }
                else
                {
                    Array.Copy(writeData, 0, data, 12, writeData.Length);
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

        #region 一括書込み応答

        /// <summary>
        /// PLCから一括書込みコマンドの返答を受信します。
        /// </summary>
        /// <returns>受信したデータのバイト配列</returns>
        private byte[] WriteCommandAck()
        {
            byte[] data;

            data = new byte[0];

            if (Device)
            {
                data = Socket.ReceiveBytes();

                if (data.Length > 0)
                {
                    BL_Bytes.Join(data, ref ReceiveBytes, data.Length);
                }

                if (ReceiveBytes.Length >= 2)
                {
                    if (ReceiveBytes[0] == 0x83)
                    {
                        if (ReceiveBytes[1] == 0x00)
                        {
                            data = new byte[1];
                            Array.Copy(ReceiveBytes, 1, data, 0, data.Length);
                        }
                        else if (ReceiveBytes[1] == 0x5B)
                        {
                            if (ReceiveBytes.Length == 4)
                            {
                                data = new byte[3];
                                Array.Copy(ReceiveBytes, 1, data, 0, data.Length);
                            }
                        }
                        else
                        {
                            if (ReceiveBytes.Length == 3)
                            {
                                data = new byte[2];
                                Array.Copy(ReceiveBytes, 1, data, 0, data.Length);
                            }
                        }
                    }
                }
            }

            return data;
        }

        #endregion

        #region 異常コード プロパティ

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

        #region 異常内容 プロパティ

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

        private void errors()
        {
            Error_code = 0;
            Error_message = "";
        }
        private void errors(int error_code, string comment)
        {
            Error_code = error_code;
            Error_message = comment;
        }

        #endregion
    }

    #endregion
}
