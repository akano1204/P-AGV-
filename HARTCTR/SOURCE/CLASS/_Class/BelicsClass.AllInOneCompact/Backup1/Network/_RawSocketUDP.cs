using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using BelicsClass.Common;

namespace BelicsClass.Network
{
    /// <summary>
    /// ソケットクラス(UDP)
    /// </summary>
    public class BL_RawSocketUDP : BL_RawSocketBase
    {
        private enum EventType
        {
            RECEIVE,
        }

        /// <summary>
        /// イベントを処理するメソッドを表します。
        /// </summary>
        public delegate void ReceiveEventHandler(object sender, ReceiveEventArgs e);

        /// <summary>
        /// 受信すると発生します。
        /// </summary>
        public event ReceiveEventHandler ReceiveEvent;

        /// <summary>
        /// 受信すると発生するイベントの引数。
        /// </summary>
        public class ReceiveEventArgs : EventArgs
        {
            internal byte[] bytes_data;

            /// <summary>
            /// 受信したデータが格納されているバイト配列を取得します。
            /// </summary>
            public byte[] BytesData
            {
                get
                {
                    return bytes_data;
                }
            }
        }

        #region 列挙型
        /// <summary>
        /// フォーマット
        /// </summary>
        public enum FormatType
        {
            /// <summary>
            /// なし
            /// </summary>
            None,
            /// <summary>
            /// ～CR
            /// </summary>
            CR,
            /// <summary>
            /// ～CR+LF
            /// </summary>
            CR_LF,
            /// <summary>
            /// STX～ETX
            /// </summary>
            STX_ETX,
            /// <summary>
            /// 特殊フォーマット
            /// </summary>
            BL_FORMAT,
        }
        #endregion

        private Socket Socket_worker = null;
        private IPEndPoint Ip_end_point;
        private FormatType Format_type;
        private int Error_code;
        private string Error_message;
        private int Receive_count = 0;
        private byte[] Receive_data = new byte[2048];
        private byte[] Temp_data = new byte[2048];
        private AutoResetEvent Wait_event;

        //====================================================================================================
        // コンストラクタ
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	int				port				ﾎﾟｰﾄ№
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	なし
        //
        //====================================================================================================
        /// <summary>
        /// ソケットクラス
        /// </summary>
        /// <param name="port">使用するソケットのポート№。</param>
        public BL_RawSocketUDP(int port)
        {
            Ip_end_point = new IPEndPoint(IPAddress.Any, port);

            Wait_event = new AutoResetEvent(false);
        }

        //====================================================================================================
        // オープン
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	FormatType		formatType			ﾌｫｰﾏｯﾄﾀｲﾌﾟ
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常		-1		予約済み
        //
        //====================================================================================================
        /// <summary>
        /// ソケットのオープンをします。
        /// </summary>
        /// <param name="formatType">オープンするソケットのフォーマット。</param>
        /// <returns></returns>
        public bool Open(FormatType formatType)
        {
            errors();

            if (Socket_worker != null)
            {
                errors(-1, "すでに使用されています。");

                return false;
            }

            Format_type = formatType;
            Receive_count = 0;
            Receive_data = new byte[2048];

            try
            {
                Socket_worker = new Socket(Ip_end_point.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

                Socket_worker.Bind(Ip_end_point);

                Socket_worker.BeginReceive(Temp_data, 0, Temp_data.Length, 0, new AsyncCallback(OnReceive), null);
            }
            catch (SocketException ex)
            {
                errors(ex.ErrorCode, ex.Message);

                this.Close();
            }
            catch (Exception ex)
            {
                errors(-1, ex.Message);

                this.Close();
            }

            return Error_code == 0 ? true : false;
        }

        //====================================================================================================
        // クローズ
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	なし
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	なし
        //
        //====================================================================================================
        /// <summary>
        /// ソケットのクローズをします。
        /// </summary>
        public void Close()
        {
            lock (this)
            {
                if (Socket_worker != null)
                {
                    try
                    {
                        Socket_worker.Shutdown(SocketShutdown.Both);
                    }
                    catch
                    {
                    }

                    try
                    {
                        Socket_worker.Close();
                    }
                    catch
                    {
                    }

                    Socket_worker = null;
                }
            }
        }

        //====================================================================================================
        // 送信
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	string			data				ﾃﾞｰﾀ
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常		-99		無効
        //													-1000	その他異常
        //
        //====================================================================================================
        /// <summary>
        /// ソケットにデータを書き込みます。
        /// </summary>
        /// <param name="data">書き込むデータが格納されている文字列。</param>
        /// <param name="remoteEP"></param>
        /// <returns></returns>
        public bool Send(string data, IPEndPoint remoteEP)
        {
            return SendBytes(Encoding.Default.GetBytes(data), remoteEP);
        }

        //====================================================================================================
        // 受信
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	なし
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	string			data				ﾃﾞｰﾀ
        //
        //====================================================================================================
        /// <summary>
        /// ソケットからデータを読み込みます。
        /// </summary>
        /// <returns></returns>
        public string Receive()
        {
            return Encoding.Default.GetString(ReceiveBytes());
        }
        /// <summary>
        /// ソケットからデータを読み込みます。
        /// </summary>
        /// <param name="timeOut">タイムアウト値。</param>
        /// <returns></returns>
        public string Receive(int timeOut)
        {
            return Encoding.Default.GetString(ReceiveBytes(timeOut));
        }

        //====================================================================================================
        // 送信（バイト配列）
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	byte[]			data				ﾃﾞｰﾀ
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常		-99		無効
        //													-1000	その他異常
        //
        //====================================================================================================
        /// <summary>
        /// ソケットにバイト配列のデータを書き込みます。
        /// </summary>
        /// <param name="data">書き込むデータが格納されているバイト配列。</param>
        /// <param name="remoteEP"></param>
        /// <returns></returns>
        public bool SendBytes(byte[] data, IPEndPoint remoteEP)
        {
            byte[] send_data;
            byte[] prefix_data, suffix_data;

            errors();

            if (Socket_worker == null)
            {
                errors(-99, "無効です。");

                return false;
            }

            switch (Format_type)
            {
                case FormatType.STX_ETX:
                    send_data = new byte[data.Length + 2];

                    send_data[0] = 0x02;
                    Array.Copy(data, 0, send_data, 1, data.Length);
                    send_data[data.Length + 1] = 0x03;
                    break;

                case FormatType.CR_LF:
                    send_data = new byte[data.Length + 2];

                    Array.Copy(data, 0, send_data, 0, data.Length);
                    send_data[data.Length] = 0x0D;
                    send_data[data.Length + 1] = 0x0A;
                    break;

                case FormatType.CR:
                    send_data = new byte[data.Length + 1];

                    Array.Copy(data, 0, send_data, 0, data.Length);
                    send_data[data.Length] = 0x0D;
                    break;

                case FormatType.BL_FORMAT:
                    prefix_data = Encoding.Default.GetBytes(PREFIX);
                    suffix_data = Encoding.Default.GetBytes(SUFFIX);

                    send_data = new byte[prefix_data.Length + 4 + data.Length + suffix_data.Length];

                    Array.Copy(prefix_data, 0, send_data, 0, prefix_data.Length);
                    Array.Copy(BL_Bytes.Int32ToBytes(data.Length), 0, send_data, prefix_data.Length, 4);
                    Array.Copy(data, 0, send_data, prefix_data.Length + 4, data.Length);
                    Array.Copy(suffix_data, 0, send_data, prefix_data.Length + 4 + data.Length, suffix_data.Length);
                    break;

                default:
                    send_data = new byte[data.Length];

                    Array.Copy(data, 0, send_data, 0, data.Length);
                    break;
            }

            try
            {
                Socket_worker.SendTo(send_data, remoteEP);
            }
            catch (Exception ex)
            {
                errors(-1002, ex.Message);
            }

            return Error_code == 0 ? true : false;
        }

        //====================================================================================================
        // 受信（バイト配列）
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	なし
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	out int			bytes_size			受信ﾊﾞｲﾄ数(-1:未受信)
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	byte[]			data				ﾃﾞｰﾀ
        //
        //====================================================================================================
        /// <summary>
        /// ソケットからバイト配列でデータを読み込みます。
        /// </summary>
        /// <param name="bytes_size">受信したバイト数。未受信のときは、-1。</param>
        /// <returns></returns>
        public byte[] ReceiveBytes(out int bytes_size)
        {
            byte[] receive_data, temp;
            byte[] prefix_data, suffix_data;
            bool flag = false;
            int count = 0, length = 0, check = 0, pos = 0, index1, index2;

            lock (this)
            {
                receive_data = new byte[Receive_data.Length];

                switch (Format_type)
                {
                    case FormatType.STX_ETX:
                        for (count = 0; count < Receive_count; count++)
                        {
                            if (check == 0)
                            {
                                if (Receive_data[count] == 0x02) check = 1;

                            }
                            else if (check == 1)
                            {
                                if (Receive_data[count] == 0x03)
                                {
                                    length = pos;
                                    Receive_count -= (count + 1);
                                    Array.Copy(Receive_data, count + 1, Receive_data, 0, Receive_count);
                                    Array.Clear(Receive_data, Receive_count, Receive_data.Length - Receive_count);

                                    flag = true;
                                    break;
                                }
                                else
                                {
                                    receive_data[pos++] = Receive_data[count];
                                }
                            }
                        }
                        break;

                    case FormatType.CR_LF:
                        for (count = 0; count < Receive_count; count++)
                        {
                            if (Receive_data[count] == 0x0D &&
                                Receive_data[count + 1] == 0x0A && (count + 1) < Receive_count)
                            {
                                length = pos;
                                Receive_count -= (count + 2);
                                Array.Copy(Receive_data, count + 2, Receive_data, 0, Receive_count);
                                Array.Clear(Receive_data, Receive_count, Receive_data.Length - Receive_count);

                                flag = true;
                                break;
                            }
                            else
                            {
                                receive_data[pos++] = Receive_data[count];
                            }
                        }
                        break;

                    case FormatType.CR:
                        for (count = 0; count < Receive_count; count++)
                        {
                            if (Receive_data[count] == 0x0D)
                            {
                                length = pos;
                                Receive_count -= (count + 1);
                                Array.Copy(Receive_data, count + 1, Receive_data, 0, Receive_count);
                                Array.Clear(Receive_data, Receive_count, Receive_data.Length - Receive_count);

                                flag = true;
                                break;
                            }
                            else
                            {
                                receive_data[pos++] = Receive_data[count];
                            }
                        }
                        break;

                    case FormatType.BL_FORMAT:
                        prefix_data = Encoding.Default.GetBytes(PREFIX);
                        suffix_data = Encoding.Default.GetBytes(SUFFIX);

                        index1 = BL_Bytes.IndexOf(Receive_data, 0, prefix_data, 0, prefix_data.Length);
                        index2 = -1;

                        if (index1 >= 0)
                        {
                            index2 = BL_Bytes.IndexOf(Receive_data, index1 + prefix_data.Length + 4, suffix_data, 0, suffix_data.Length);
                        }

                        if (index1 >= 0 && index2 >= 0)
                        {
                            pos = BL_Bytes.BytesToInt32(Receive_data, index1 + prefix_data.Length);

                            if (pos == (index2 - index1 - prefix_data.Length - 4))
                            {
                                length = pos;
                                Array.Copy(Receive_data, index1 + prefix_data.Length + 4, receive_data, 0, length);

                                flag = true;
                            }

                            Receive_count -= (index2 + suffix_data.Length);
                            Array.Copy(Receive_data, index2 + suffix_data.Length, Receive_data, 0, Receive_count);
                            Array.Clear(Receive_data, Receive_count, Receive_data.Length - Receive_count);
                        }
                        break;

                    default:
                        if (Receive_count > 0)
                        {
                            length = Receive_count;
                            Array.Copy(Receive_data, 0, receive_data, 0, Receive_count);
                            Array.Clear(Receive_data, 0, Receive_data.Length);
                            Receive_count = 0;

                            flag = true;
                        }
                        break;
                }
            }

            if (flag) bytes_size = length;
            else bytes_size = -1;

            temp = new byte[length];
            Array.Copy(receive_data, 0, temp, 0, length);

            return temp;
        }
        /// <summary>
        /// ソケットからバイト配列でデータを読み込みます。
        /// </summary>
        /// <returns></returns>
        public byte[] ReceiveBytes()
        {
            int count;

            return ReceiveBytes(out count);
        }
        /// <summary>
        /// ソケットからバイト配列でデータを読み込みます。
        /// </summary>
        /// <param name="timeOut">タイムアウト値。</param>
        /// <returns></returns>
        public byte[] ReceiveBytes(int timeOut)
        {
            int count;

            return ReceiveBytes(out count, timeOut);
        }
        /// <summary>
        /// ソケットからバイト配列でデータを読み込みます。
        /// </summary>
        /// <param name="bytes_size">受信したバイト数。未受信のときは、-1。</param>
        /// <param name="timeOut">タイムアウト値。</param>
        /// <returns></returns>
        public byte[] ReceiveBytes(out int bytes_size, int timeOut)
        {
            int time_out, wait = 0;
            byte[] data = new byte[0];

            bytes_size = -1;

            if (timeOut < 0)
            {
                timeOut = 0;
            }

            time_out = BL_Win32API.TickCount;

            do
            {
                Wait_event.WaitOne(timeOut - wait, false);

                data = ReceiveBytes(out bytes_size);

                if (bytes_size != -1)
                {
                    break;
                }

                wait = BL_Win32API.TickCount - time_out;

            } while (timeOut > wait);

            return data;
        }

        //====================================================================================================
        // 送信（バイト配列）
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	byte[]			data				ﾃﾞｰﾀ
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常
        //
        //====================================================================================================
        /// <summary>
        /// ソケットに FormatType列挙型 の設定を付加せずにデータを書き込みます。
        /// </summary>
        /// <param name="data">書き込むデータが格納されているバイト配列。</param>
        /// <param name="remoteEP"></param>
        /// <returns></returns>
        public bool SendBytesDirect(byte[] data, IPEndPoint remoteEP)
        {
            errors();

            if (Socket_worker == null)
            {
                errors(-99, "無効です。");

                return false;
            }

            try
            {
                //Socket_worker.BeginSend(data, 0, data.Length, 0, new AsyncCallback(OnSend), null);
                Socket_worker.SendTo(data, remoteEP);
            }
            catch (Exception ex)
            {
                errors(-1003, ex.Message);
            }

            return Error_code == 0 ? true : false;
        }

        //====================================================================================================
        // 受信（バイト配列）
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	なし
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	byte[]			data				ﾃﾞｰﾀ
        //
        //====================================================================================================
        /// <summary>
        /// ソケットから FormatType列挙型 の設定を無視してバイト配列でデータを読み込みます。
        /// </summary>
        /// <returns></returns>
        public byte[] ReceiveBytesDirect()
        {
            byte[] receive_data, temp;
            int length = 0;

            lock (this)
            {
                receive_data = new byte[Receive_data.Length];

                length = Receive_count;
                Array.Copy(Receive_data, 0, receive_data, 0, Receive_count);
                Array.Clear(Receive_data, 0, Receive_data.Length);
                Receive_count = 0;
            }

            temp = new byte[length];
            Array.Copy(receive_data, 0, temp, 0, length);

            return temp;
        }

        //====================================================================================================
        // 接続先IPアドレスプロパティ
        //====================================================================================================
        /// <summary>
        /// 接続先のIPアドレスを取得します。
        /// </summary>
        public string RemoteIpAddress
        {
            get
            {
                if (Socket_worker != null)
                {
                    return ((IPEndPoint)Socket_worker.RemoteEndPoint).Address.ToString();
                }
                else
                {
                    return "";
                }
            }
        }

        //====================================================================================================
        // 接続先ポート№プロパティ
        //====================================================================================================
        /// <summary>
        /// 接続先のポート№を取得します。
        /// </summary>
        public int RemotePort
        {
            get
            {
                if (Socket_worker != null)
                {
                    return ((IPEndPoint)Socket_worker.RemoteEndPoint).Port;
                }
                else
                {
                    return -1;
                }
            }
        }

        //====================================================================================================
        // 異常コードプロパティ
        //====================================================================================================
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

        //====================================================================================================
        // 異常内容プロパティ
        //====================================================================================================
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

        //====================================================================================================
        // 送信（コールバック）
        //====================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        protected void OnSend(IAsyncResult ar)
        {
            Socket_worker.EndSend(ar);
        }

        //====================================================================================================
        // 受信（コールバック）
        //====================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        protected void OnReceive(IAsyncResult ar)
        {
            int count = 0;
            byte[] data;

            try
            {
                count = Socket_worker.EndReceive(ar);

                if (count > 0)
                {
                    lock (this)
                    {
                        if (Receive_data.Length < Receive_count + count)
                        {
                            data = new byte[Receive_count + count];

                            Array.Copy(Receive_data, 0, data, 0, Receive_count);

                            Receive_data = data;
                        }

                        Array.Copy(Temp_data, 0, Receive_data, Receive_count, count);
                        Receive_count += count;
                    }

                    Array.Clear(Temp_data, 0, Temp_data.Length);

                    Socket_worker.BeginReceive(Temp_data, 0, Temp_data.Length, 0, new AsyncCallback(OnReceive), null);

                    Wait_event.Set();

                    call_event(EventType.RECEIVE);
                }
                else
                {
                    Wait_event.Set();
                }
            }
            catch (Exception ex)
            {
                errors(-1005, ex.Message);

                Wait_event.Set();
            }
        }

        //====================================================================================================
        // イベントコール
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	EventType		type				ｲﾍﾞﾝﾄ№
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	なし
        //
        //====================================================================================================
        private void call_event(EventType type)
        {
            byte[] data;

            if (type == EventType.RECEIVE && this.ReceiveEvent != null)
            {
                while (true)
                {
                    data = this.ReceiveBytes();

                    if (data.Length > 0)
                    {
                        ReceiveEventArgs e = new ReceiveEventArgs();

                        e.bytes_data = data;

                        this.ReceiveEvent(this, e);
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        //====================================================================================================
        // 異常の設定
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	int				code				異常ｺｰﾄﾞ
        //	string			message				異常内容
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	なし
        //
        //====================================================================================================
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
    }
}
