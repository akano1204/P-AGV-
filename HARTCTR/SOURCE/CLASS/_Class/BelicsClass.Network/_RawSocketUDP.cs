using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Collections.Generic;

using BelicsClass.Common;
using BelicsClass.File;

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
            internal IPEndPoint ep;

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

            /// <summary>
            /// 
            /// </summary>
            public IPEndPoint SenderEndPoint
            {
                get
                {
                    return ep;
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
            /// <summary>
            /// 特殊フォーマット
            /// </summary>
            BL_IDSIZE,
        }
        #endregion

        private UdpClient Socket_worker = null;
        private IPEndPoint Ip_end_point;
        private FormatType Format_type;
        private int Error_code;
        private string Error_message;
        private AutoResetEvent Wait_event;

        private Dictionary<string, List<byte>> Receive_data = new Dictionary<string, List<byte>>();

        private BL_Log log = null;

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

            if (log != null) log.Dispose();
            log = new BL_Log("", "SocketUDP-" + port.ToString() + ".LOG");
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
            //Receive_count = 0;
            //Receive_data = new byte[2048];
            Receive_data = new Dictionary<string, List<byte>>();

            try
            {
                //Socket_worker = new Socket(Ip_end_point.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

                //Socket_worker.Bind(Ip_end_point);

                //Socket_worker.BeginReceive(Temp_data, 0, Temp_data.Length, 0, new AsyncCallback(OnReceive), null);


                Socket_worker = new UdpClient(Ip_end_point);

                Socket_worker.BeginReceive(new AsyncCallback(OnReceive), null);

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

                    //try
                    //{
                    //    Socket_worker.Shutdown(SocketShutdown.Both);
                    //}
                    //catch
                    //{
                    //}

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

        /// <summary>
        /// データIDとデータ数の相互変換を行います。
        /// </summary>
        /// <param name="count">変換する値を指定します。</param>
        /// <returns>変換された値を返します。</returns>
        public static int ChangeDataCount(int count)
        {
            //符号なしに変換
            uint u_cnt = (uint)count;

            //反転
            u_cnt = u_cnt ^ 0xffffffff;

            return (int)u_cnt;
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
        /// <param name="remoteEP">受信する送信元</param>
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

                case FormatType.BL_IDSIZE:
                    {
                        send_data = new byte[data.Length + 8];
                        int length = data.Length;

                        Array.Copy(data, 0, send_data, 8, data.Length);

                        byte[] id = BitConverter.GetBytes(ChangeDataCount(length));
                        byte[] sz = BitConverter.GetBytes(length);

                        send_data[0] = id[3];
                        send_data[1] = id[2];
                        send_data[2] = id[1];
                        send_data[3] = id[0];
                        send_data[4] = sz[3];
                        send_data[5] = sz[2];
                        send_data[6] = sz[1];
                        send_data[7] = sz[0];
                    }
                    break;

                default:
                    send_data = new byte[data.Length];

                    Array.Copy(data, 0, send_data, 0, data.Length);
                    break;
            }

            try
            {
                string l = "";
                foreach (var b in send_data) l += (b.ToString("X2") + " ");
                log.Add("S[" + l + "]");

                //Socket_worker.SendTo(send_data, remoteEP);
                Socket_worker.Send(send_data, send_data.Length, remoteEP);
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
            return ReceiveBytes(out bytes_size, new IPEndPoint(IPAddress.Any, 0));
        }

        /// <summary>
        /// ソケットからバイト配列でデータを読み込みます。
        /// </summary>
        /// <param name="bytes_size">受信したバイト数。未受信のときは、-1。</param>
        /// <param name="remoteEP">受信する送信元</param>
        /// <returns></returns>
        public byte[] ReceiveBytes(out int bytes_size, IPEndPoint remoteEP)
        {
            byte[] receive_data = new byte[0];
            byte[] temp;
            byte[] prefix_data, suffix_data;
            bool flag = false;
            int count = 0, length = 0, check = 0, pos = 0, index1, index2;

            bytes_size = -1;

            lock (this)
            {
                foreach (var v in Receive_data)
                {
                    if (remoteEP.Address != IPAddress.Any || remoteEP.Port != 0)
                    {
                        string key = remoteEP.Address.ToString() + ":" + remoteEP.Port.ToString();
                        if (v.Key != key) continue;
                    }

                    byte[] r = v.Value.ToArray();

                    receive_data = new byte[r.Length];

                    switch (Format_type)
                    {
                        case FormatType.STX_ETX:
                            for (count = 0; count < r.Length; count++)
                            {
                                if (check == 0)
                                {
                                    if (r[count] == 0x02) check = 1;

                                }
                                else if (check == 1)
                                {
                                    if (r[count] == 0x03)
                                    {
                                        length = pos;
                                        v.Value.RemoveRange(0, count + 1);

                                        flag = true;
                                        break;
                                    }
                                    else
                                    {
                                        receive_data[pos++] = r[count];
                                    }
                                }
                            }
                            break;

                        case FormatType.CR_LF:
                            for (count = 0; count < r.Length; count++)
                            {
                                if (r[count] == 0x0D &&
                                    r[count + 1] == 0x0A && (count + 1) < r.Length)
                                {
                                    length = pos;
                                    v.Value.RemoveRange(0, count + 2);

                                    flag = true;
                                    break;
                                }
                                else
                                {
                                    receive_data[pos++] = r[count];
                                }
                            }
                            break;

                        case FormatType.CR:
                            for (count = 0; count < r.Length; count++)
                            {
                                if (r[count] == 0x0D)
                                {
                                    length = pos;
                                    v.Value.RemoveRange(0, count + 1);

                                    flag = true;
                                    break;
                                }
                                else
                                {
                                    receive_data[pos++] = r[count];
                                }
                            }
                            break;

                        case FormatType.BL_FORMAT:
                            prefix_data = Encoding.Default.GetBytes(PREFIX);
                            suffix_data = Encoding.Default.GetBytes(SUFFIX);

                            index1 = BL_Bytes.IndexOf(r, 0, prefix_data, 0, prefix_data.Length);
                            index2 = -1;

                            if (index1 >= 0)
                            {
                                index2 = BL_Bytes.IndexOf(r, index1 + prefix_data.Length + 4, suffix_data, 0, suffix_data.Length);
                            }

                            if (index1 >= 0 && index2 >= 0)
                            {
                                pos = BL_Bytes.BytesToInt32(r, index1 + prefix_data.Length);

                                if (pos == (index2 - index1 - prefix_data.Length - 4))
                                {
                                    length = pos;
                                    Array.Copy(r, index1 + prefix_data.Length + 4, receive_data, 0, length);
                                    v.Value.RemoveRange(0, index2 + suffix_data.Length);

                                    flag = true;
                                }
                            }
                            break;

                        case FormatType.BL_IDSIZE:
                            {
                                if (r.Length < 8) break;

                                //IDとデータサイズを取得
                                int dataID;
                                int dataCount;

                                byte[] id = new byte[4];
                                byte[] sz = new byte[4];

                                id[3] = r[0];
                                id[2] = r[1];
                                id[1] = r[2];
                                id[0] = r[3];
                                sz[3] = r[4];
                                sz[2] = r[5];
                                sz[1] = r[6];
                                sz[0] = r[7];

                                int offset = 0;
                                BL_BitConverter.ToValue(id, ref offset, out dataID);
                                offset = 0;
                                BL_BitConverter.ToValue(sz, ref offset, out dataCount);

                                //ID部を含めたサイズ(全データ長)を求める
                                int DataSize = dataCount + 8;

                                //string l = "";
                                //foreach (var b in r) l += (b.ToString("X2") + " ");
                                //log.Add("[" + remoteEP + "]->[" + l + "]");


                                //データIDとデータ数が不一致なので、受信処理終了
                                if (ChangeDataCount(dataCount) != dataID)
                                {
                                    log.Add("データIDサイズ不一致[" + dataCount.ToString() + "]");

                                    string l = "";
                                    foreach (var b in r) l += (b.ToString("X2") + " ");
                                    log.Add("[" + remoteEP + "]->[" + l + "]");


                                    Close();
                                    Open(Format_type);

                                    //throw (new Exception("データID、データ数不一致"));
                                    //return;
                                    break;
                                }

                                ////データサイズは範囲外なので、受信処理終了
                                //if (DataSize < 8 || 0x7FFFFFFF < DataSize)
                                //{
                                //    Close();
                                //    Open(Format_type);

                                //    //throw (new Exception("データ数異常"));
                                //    //return;
                                //    break;
                                //}

                                if (DataSize <= r.Length)
                                {
                                    length = dataCount;
                                    Array.Copy(r, DataSize - dataCount, receive_data, 0, dataCount);
                                    v.Value.RemoveRange(0, DataSize);

                                    flag = true;
                                }
                            }
                            break;

                        default:
                            if (r.Length > 0)
                            {
                                length = r.Length;
                                Array.Copy(r, 0, receive_data, 0, r.Length);
                                v.Value.Clear();

                                flag = true;
                            }
                            break;
                    }

                    if (flag)
                    {
                        bytes_size = length;
                        break;
                    }
                }
            }

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
        /// <param name="remoteEP">受信する送信元</param>
        /// <returns></returns>
        public byte[] ReceiveBytes(IPEndPoint remoteEP)
        {
            int count;

            return ReceiveBytes(out count, remoteEP);
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
        /// <param name="timeOut">タイムアウト値。</param>
        /// <param name="remoteEP">受信する送信元</param>
        /// <returns></returns>
        public byte[] ReceiveBytes(int timeOut, IPEndPoint remoteEP)
        {
            int count;

            return ReceiveBytes(out count, timeOut, remoteEP);
        }
        /// <summary>
        /// ソケットからバイト配列でデータを読み込みます。
        /// </summary>
        /// <param name="bytes_size">受信したバイト数。未受信のときは、-1。</param>
        /// <param name="timeOut">タイムアウト値。</param>
        /// <returns></returns>
        public byte[] ReceiveBytes(out int bytes_size, int timeOut)
        {
            return ReceiveBytes(out bytes_size, timeOut, new IPEndPoint(IPAddress.Any, 0));
        }

        /// <summary>
        /// ソケットからバイト配列でデータを読み込みます。
        /// </summary>
        /// <param name="bytes_size">受信したバイト数。未受信のときは、-1。</param>
        /// <param name="timeOut">タイムアウト値。</param>
        /// <param name="remoteEP">受信する送信元</param>
        /// <returns></returns>
        public byte[] ReceiveBytes(out int bytes_size, int timeOut, IPEndPoint remoteEP)
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

                data = ReceiveBytes(out bytes_size, remoteEP);

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
                //Socket_worker.SendTo(data, remoteEP);
                Socket_worker.Send(data, data.Length, remoteEP);
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
            return ReceiveBytesDirect(new IPEndPoint(IPAddress.Any, 0));
        }

        /// <summary>
        /// ソケットから FormatType列挙型 の設定を無視してバイト配列でデータを読み込みます。
        /// </summary>
        /// <param name="remoteEP">受信する送信元</param>
        /// <returns></returns>
        public byte[] ReceiveBytesDirect(IPEndPoint remoteEP)
        {
            byte[] receive_data = new byte[0];
            byte[] temp;
            int length = 0;

            lock (this)
            {
                foreach (var v in Receive_data)
                {
                    if (remoteEP.Address != IPAddress.Any || remoteEP.Port != 0)
                    {
                        string key = remoteEP.Address.ToString() + ":" + remoteEP.Port.ToString();
                        if (v.Key != key) continue;
                    }

                    byte[] r = v.Value.ToArray();
                    receive_data = new byte[r.Length];

                    length = r.Length;
                    Array.Copy(r, 0, receive_data, 0, r.Length);
                    v.Value.Clear();
                }
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
                    //return ((IPEndPoint)Socket_worker.RemoteEndPoint).Address.ToString();
                    return ((IPEndPoint)Socket_worker.Client.RemoteEndPoint).Address.ToString();
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
                    //return ((IPEndPoint)Socket_worker.RemoteEndPoint).Port;
                    return ((IPEndPoint)Socket_worker.Client.RemoteEndPoint).Port;
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
            try
            {
                //count = Socket_worker.EndReceive(ar);
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);

                if (Socket_worker != null)
                {
                    byte[] Temp_data = Socket_worker.EndReceive(ar, ref ep);

                    if (Temp_data.Length > 0)
                    {
                        lock (this)
                        {
                            string key = ep.Address + ":" + ep.Port.ToString();
                            if (!Receive_data.ContainsKey(key)) Receive_data[key] = new List<byte>();

                            Receive_data[key].AddRange(Temp_data);
                        }

                        Array.Clear(Temp_data, 0, Temp_data.Length);

                        //Socket_worker.BeginReceive(Temp_data, 0, Temp_data.Length, 0, new AsyncCallback(OnReceive), null);
                        Socket_worker.BeginReceive(new AsyncCallback(OnReceive), null);

                        Wait_event.Set();

                        call_event(EventType.RECEIVE, ep);
                    }
                    else
                    {
                        Wait_event.Set();
                    }
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
        private void call_event(EventType type, IPEndPoint ep)
        {
            byte[] data;

            if (type == EventType.RECEIVE && this.ReceiveEvent != null)
            {
                while (true)
                {
                    data = this.ReceiveBytes(ep);

                    if (data.Length > 0)
                    {
                        ReceiveEventArgs e = new ReceiveEventArgs();

                        e.bytes_data = data;
                        e.ep = ep;

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
