﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using BelicsClass.Common;

namespace BelicsClass.Network
{
    /// <summary>
    /// ソケットクラス(複数接続可)
    /// </summary>
    public class BL_RawSocketServer : BL_RawSocketBase
    {
        /// <summary>
        /// イベントを処理するメソッドを表します。
        /// </summary>
        public delegate void ConnectEventHandler(object sender, ConnectEventArgs e);

        /// <summary>
        /// 接続すると発生します。
        /// </summary>
        public event ConnectEventHandler ConnectEvent;

        /// <summary>
        /// 接続すると発生するイベントの引数。
        /// </summary>
        public class ConnectEventArgs : EventArgs
        {
            internal int no = -1;

            /// <summary>
            /// 接続したソケットの接続№を取得します。
            /// </summary>
            public int ConnectNo
            {
                get
                {
                    return no;
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

        #region インデクサ
        /// <summary>
        /// 接続したソケットの情報
        /// </summary>
        public Connected this[int no]
        {
            get
            {
                return Connect[no];
            }
        }
        #endregion

        private Socket Socket_master = null;
        private IPEndPoint Ip_end_point;
        private FormatType Format_type;
        private bool Enabled = false;
        private int Keep_alive_timer = 0;
        private int Error_code;
        private string Error_message;
        private Connected[] Connect;

        //====================================================================================================
        // コンストラクタ
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	string			ip_address			IPｱﾄﾞﾚｽ
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
        /// <param name="ip_address">使用するソケットのIPアドレス。</param>
        /// <param name="port">使用するソケットのポート№。</param>
        /// <param name="connect_max">最大接続数</param>
        public BL_RawSocketServer(string ip_address, int port, int connect_max)
        {
            Ip_end_point = new IPEndPoint(IPAddress.Parse(ip_address), port);

            initialize(connect_max);
        }
        /// <summary>
        /// ソケットクラス
        /// </summary>
        /// <param name="port">使用するソケットのポート№。</param>
        /// <param name="connect_max">最大接続数</param>
        public BL_RawSocketServer(int port, int connect_max)
        {
            Ip_end_point = new IPEndPoint(IPAddress.Any, port);

            initialize(connect_max);
        }
        /// <summary>
        /// ソケットクラス
        /// </summary>
        /// <param name="port">使用するソケットのポート№。</param>
        public BL_RawSocketServer(int port)
            : this(port, 10)
        {
        }
        private void initialize(int max)
        {
            Connect = new Connected[max];

            for (int no = 0; no < Connect.Length; no++)
            {
                Connect[no] = new Connected();

                Connect[no].socket = null;
            }
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

            if (Socket_master != null)
            {
                errors(-1, "すでに使用されています。");

                return false;
            }

            Format_type = formatType;
            Enabled = false;

            try
            {
                Socket_master = new Socket(Ip_end_point.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                Socket_master.Bind(Ip_end_point);

                Socket_master.Listen(5);

                Socket_master.BeginAccept(new AsyncCallback(OnAccept), Socket_master);
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
            for (int no = 0; no < Connect.Length; no++)
            {
                if (Connect[no].socket != null)
                {
                    Connect[no].Dispose();
                }
            }

            Enabled = true;

            if (Socket_master != null)
            {
                try
                {
                    Socket_master.Close();
                }
                catch
                {
                }

                Socket_master = null;
            }
        }

        //====================================================================================================
        // 接続タイムアウト時間プロパティ
        //====================================================================================================
        /// <summary>
        /// ソケットの生存確認時間を設定します。単位はミリ秒。※接続する前に設定して下さい。
        /// </summary>
        public int KeepAliveTimer
        {
            set
            {
                Keep_alive_timer = value;
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
        // 接続待ち（コールバック）
        //====================================================================================================
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ar"></param>
        protected void OnAccept(IAsyncResult ar)
        {
            Socket server = null;
            int no;

            if (Enabled)
            {
                return;
            }

            try
            {
                server = (Socket)ar.AsyncState;

                for (no = 0; no < Connect.Length; no++)
                {
                    if (Connect[no].socket == null &&
                        Connect[no].Reserved == false)
                    {
                        break;
                    }
                }

                if (no < Connect.Length)
                {
                    Connect[no] = new Connected();
                    Connect[no].socket = server.EndAccept(ar);
                    Connect[no].Format_type = Format_type;
                    Connect[no].Enable = true;
                    Connect[no].Reserved = true;
                    Connect[no].Connect_no = no;
                    //Connect[no].StartReceive();

                    if (Keep_alive_timer > 0)
                    {
                        byte[] inBuffer = new byte[12];
                        BitConverter.GetBytes(1).CopyTo(inBuffer, 0);
                        BitConverter.GetBytes(Keep_alive_timer).CopyTo(inBuffer, 4);
                        BitConverter.GetBytes(1000).CopyTo(inBuffer, 8);

                        Connect[no].socket.IOControl(IOControlCode.KeepAliveValues, inBuffer, null);
                    }

                    if (this.ConnectEvent != null)
                    {
                        ConnectEventArgs e = new ConnectEventArgs();

                        e.no = no;

                        this.ConnectEvent(this, e);
                    }

                    Connect[no].StartReceive();
                }
                else
                {
                    server.EndAccept(ar).Close();
                }

                server.BeginAccept(new AsyncCallback(OnAccept), server);
            }
            catch (Exception ex)
            {
                errors(-1004, ex.Message);

                if (server != null)
                {
                    server.BeginAccept(new AsyncCallback(OnAccept), server);
                }

                //this.Close();
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

        #region ソケット接続クラス
        /// <summary>
        /// ソケット接続クラス
        /// </summary>
        public class Connected : IDisposable
        {
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
                internal int no = -1;
                internal byte[] bytes_data;

                /// <summary>
                /// 接続したソケットの接続№を取得します。
                /// </summary>
                public int ConnectNo
                {
                    get
                    {
                        return no;
                    }
                }

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

            /// <summary>
            /// イベントを処理するメソッドを表します。
            /// </summary>
            public delegate void DisconnectEventHandler(object sender, DisconnectEventArgs e);

            /// <summary>
            /// 切断すると発生します。
            /// </summary>
            public event DisconnectEventHandler DisconnectEvent;

            /// <summary>
            /// 切断すると発生するイベントの引数。
            /// </summary>
            public class DisconnectEventArgs : EventArgs
            {
                internal int no = -1;
                internal int error_code = 0;
                internal string error_message = "";

                /// <summary>
                /// 切断したソケットの接続№を取得します。
                /// </summary>
                public int ConnectNo
                {
                    get
                    {
                        return no;
                    }
                }

                /// <summary>
                /// 異常コード
                /// </summary>
                public int ErrorCode
                {
                    get
                    {
                        return error_code;
                    }
                }

                /// <summary>
                /// 異常内容
                /// </summary>
                public string ErrorMessage
                {
                    get
                    {
                        return error_message;
                    }
                }
            }

            internal Socket socket = null;
            internal FormatType Format_type;
            internal bool Enable = false;
            internal bool Reserved = false;
            internal int Connect_no = -1;
            private int Error_code;
            private string Error_message;
            private int Receive_count = 0;
            private byte[] Receive_data = new byte[2048];
            private byte[] Temp_data = new byte[2048];
            private AutoResetEvent Wait_event = new AutoResetEvent(false);

            //====================================================================================================
            // リソース解放
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
            /// リソースを解放します。
            /// </summary>
            public void Dispose()
            {
                if (socket != null)
                {
                    try
                    {
                        socket.Shutdown(SocketShutdown.Both);
                    }
                    catch
                    {
                    }

                    try
                    {
                        socket.Close();
                    }
                    catch
                    {
                    }

                    socket = null;
                    Enable = false;
                }
            }

            //====================================================================================================
            // 予約解放
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
            /// 予約を解放します。
            /// </summary>
            public void Release()
            {
                Reserved = false;
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
            /// <returns></returns>
            public bool Send(string data)
            {
                return SendBytes(Encoding.Default.GetBytes(data));
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
            /// <returns></returns>
            public bool SendBytes(byte[] data)
            {
                byte[] send_data;
                byte[] prefix_data, suffix_data;

                errors();

                if (socket == null)
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
                    socket.Send(send_data);
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

                    if (bytes_size != -1 || !IsConnected)
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
            /// <returns></returns>
            public bool SendBytesDirect(byte[] data)
            {
                errors();

                if (socket == null)
                {
                    errors(-99, "無効です。");

                    return false;
                }

                try
                {
                    socket.Send(data);
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

            internal void StartReceive()
            {
                socket.BeginReceive(Temp_data, 0, Temp_data.Length, 0, new AsyncCallback(OnReceive), null);
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
                    count = socket.EndReceive(ar);

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

                        socket.BeginReceive(Temp_data, 0, Temp_data.Length, 0, new AsyncCallback(OnReceive), null);

                        Wait_event.Set();

                        if (this.ReceiveEvent != null)
                        {
                            while (true)
                            {
                                data = this.ReceiveBytes();

                                if (data.Length > 0)
                                {
                                    ReceiveEventArgs e = new ReceiveEventArgs();

                                    e.no = Connect_no;
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
                    else
                    {
                        Wait_event.Set();

                        if (this.DisconnectEvent != null)
                        {
                            DisconnectEventArgs e = new DisconnectEventArgs();

                            e.no = Connect_no;

                            this.DisconnectEvent(this, e);
                        }

                        this.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    errors(-1005, ex.Message);

                    if (this.DisconnectEvent != null)
                    {
                        DisconnectEventArgs e = new DisconnectEventArgs();

                        e.no = Connect_no;
                        e.error_code = -1005;
                        e.error_message = ex.Message;

                        this.DisconnectEvent(this, e);
                    }

                    this.Dispose();
                }
            }

            //====================================================================================================
            // 接続状態プロパティ
            //====================================================================================================
            /// <summary>
            /// ソケットの接続状態を取得します。接続している場合は true。それ以外の場合は false。
            /// </summary>
            public bool IsConnected
            {
                get
                {
                    return Enable;
                }
            }

            //====================================================================================================
            // 予約状態プロパティ
            //====================================================================================================
            /// <summary>
            /// ソケットの予約状態を取得します。予約されている場合は true。それ以外の場合は false。
            /// </summary>
            public bool IsReserved
            {
                get
                {
                    return Reserved;
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
        #endregion
    }
}
