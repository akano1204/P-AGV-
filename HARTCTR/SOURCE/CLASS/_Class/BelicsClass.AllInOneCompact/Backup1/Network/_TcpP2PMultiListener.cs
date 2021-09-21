using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace BelicsClass.Network
{
    /// <summary>
    /// マルチ接続可能TCPサーバークラスです
    /// </summary>
    public class BL_TcpP2PMultiServer : BL_TcpP2PServer
    {
        /// <summary>
        /// マルチ接続された際に実際に通信を行うTCP接続クラスです
        /// </summary>
        public class BL_TcpP2PMultiClient : BL_TcpP2P
        {
            /// <summary>
            /// リモートロック状態を保持します
            /// </summary>
            public bool RemoteLock = false;

            //public BL_TcpP2PMultiClient(int sendBufferCount, int receiveBufferCount, Socket linkedSocket, int sessionTimeout)
            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="sendBufferCount">送信バッファ数</param>
            /// <param name="receiveBufferCount">受信バッファ数</param>
            /// <param name="linkedSocket">接続されたソケットオブジェクト</param>
            public BL_TcpP2PMultiClient(int sendBufferCount, int receiveBufferCount, Socket linkedSocket)
                : base(sendBufferCount, receiveBufferCount)
            {
                //this.sessionTimeout = sessionTimeout;
                LinkedSocket = linkedSocket;
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="sendBufferCount">送信バッファ数</param>
            /// <param name="receiveBufferCount">受信バッファ数</param>
            /// <param name="linkedSocket">接続されたソケットオブジェクト</param>
            /// <param name="formatType">フォーマット種別を指定してください。</param>
            public BL_TcpP2PMultiClient(int sendBufferCount, int receiveBufferCount, Socket linkedSocket, BL_FormatType formatType)
                : base(sendBufferCount, receiveBufferCount, formatType)
            {
                //this.sessionTimeout = sessionTimeout;
                LinkedSocket = linkedSocket;
            }

            /// <summary>
            /// 接続されたソケットオブジェクトを取得します
            /// </summary>
            /// <returns></returns>
            protected override Socket Link()
            {
                return LinkedSocket;
            }
        }

        /// <summary>
        /// 接続待機が開始された際に発生するイベントデリゲートです
        /// </summary>
        /// <param name="sender">発生源</param>
        /// <param name="connected_clients">接続クライアント数</param>
        public delegate void Event_Handler_Listening(BL_TcpP2P sender, int connected_clients);

        /// <summary>
        /// 接続待機が開始された際に発生するイベントです
        /// </summary>
        public virtual event Event_Handler_Listening EventListening;

        /// <summary>
        /// クライアントが接続された際に発生するイベントです
        /// </summary>
        public override event Event_Handler_Connected EventConnected;

        /// <summary>
        /// クライアントからデータを受信した際に発生するイベントです
        /// </summary>
        public override event Event_Handler_Received EventReceived;

        /// <summary>
        /// クライアントが切断された際に発生するイベントです
        /// </summary>
        public override event Event_Handler_Closed EventClosed;

        /// <summary>
        /// 通信エラーが発生した際に発生するイベントです
        /// </summary>
        public override event Event_Handler_Error EventError;

        /// <summary>
        /// 接続中のクライアント数を取得します
        /// </summary>
        public int ConnectedClients { get { return sockets.Count; } }

        //public BL_TcpP2PMultiServer(
        //    IPAddress localIP,
        //    int localPort,
        //    int sendBufferCount,
        //    int receiveBufferCount,
        //    int sessionTimeout
        //    )
        //    : this(localIP, localPort, sendBufferCount, receiveBufferCount)
        //{
        //    this.sessionTimeout = sessionTimeout;
        //}

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="localIP">自分のIPアドレスを指定します。特に指定が無い場合はIPAddress.Anyを指定してください。</param>
        /// <param name="localPort">自分のポート番号を指定します。</param>
        /// <param name="sendBufferCount">送信データのバッファ数を指定します。連続送信回数を考慮して設定してください。</param>
        /// <param name="receiveBufferCount">受信データのバッファ数を指定します。連続受信回数を考慮して設定してください。</param>
        public BL_TcpP2PMultiServer(
            IPAddress localIP,
            int localPort,
            int sendBufferCount,
            int receiveBufferCount
            )
            : base(localIP, localPort, sendBufferCount, receiveBufferCount)
        {
            m_ListenerEndPoint = new IPEndPoint(localIP, localPort);
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="localIP">自分のIPアドレスを指定します。特に指定が無い場合はIPAddress.Anyを指定してください。</param>
        /// <param name="localPort">自分のポート番号を指定します。</param>
        /// <param name="sendBufferCount">送信データのバッファ数を指定します。連続送信回数を考慮して設定してください。</param>
        /// <param name="receiveBufferCount">受信データのバッファ数を指定します。連続受信回数を考慮して設定してください。</param>
        /// <param name="formatType">フォーマット種別を指定してください。</param>
        public BL_TcpP2PMultiServer(
            IPAddress localIP,
            int localPort,
            int sendBufferCount,
            int receiveBufferCount,
            BL_FormatType formatType
            )
            : base(localIP, localPort, sendBufferCount, receiveBufferCount, formatType)
        {
            m_ListenerEndPoint = new IPEndPoint(localIP, localPort);
        }

        /// <summary>
        /// 接続待機用のスレッドです
        /// </summary>
        protected Thread m_LinkThread;

        /// <summary>
        /// 接続中のクライアントを管理するコレクションです
        /// </summary>
        protected List<BL_TcpP2PMultiClient> sockets = new List<BL_TcpP2PMultiClient>();

        /// <summary>
        /// スレッドのループです
        /// 接続待機と接続確立処理のみを行います
        /// </summary>
        protected override void Loop()
        {
            for (; ; )
            {
                if (EventListening != null) EventListening(this, sockets.Count);

                Socket serverSocket = null;
                Socket returnSocket = null;

                for (; ; )
                {
                    try
                    {
                        //サーバソケット生成／バインド
                        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                        serverSocket.Bind(m_ListenerEndPoint);

                        //サーバソケット受付数を設定
                        serverSocket.Listen(5);

                        //受付待ち
                        for (; ; )
                        {
                            if (!LoopNow) break;
                            if (serverSocket.Poll(100, SelectMode.SelectRead)) break;
                            if (serverSocket.Poll(100, SelectMode.SelectError)) throw (new Exception("error"));

                            Thread.Sleep(20);
                        }
                        returnSocket = serverSocket.Accept();

                        //サーバソケットクローズ
                        //(サーバソケットはシャットダウンすると例外スローされる)
                        //serverSocket.Shutdown( SocketShutdown.Both );
                        serverSocket.Close();

                        lock (sockets)
                        {
                            //BL_TcpP2PMultiClient client = new BL_TcpP2PMultiClient(SEND_BUFFER_COUNT, RECEIVE_BUFFER_COUNT, returnSocket, sessionTimeout);
                            BL_TcpP2PMultiClient client = new BL_TcpP2PMultiClient(SEND_BUFFER_COUNT, RECEIVE_BUFFER_COUNT, returnSocket, FormatType);
                            client.EventClosed += client_EventClosed;
                            client.EventReceived += client_EventReceived;
                            client.EventConnected += client_EventConnected;
                            client.EventError += client_EventError;

                            SetConnect();
                            client.StartLink();

                            sockets.Add(client);
                        }

                        break;
                    }
                    catch (Exception ex)
                    {
                        //Console.WriteLine(ex.Message);
                        if (EventError != null) EventError(this, ex);

                        //サーバソケットクローズ
                        try { serverSocket.Close(); }
                        catch { }
                    }

                    //待機
                    Thread.Sleep(300);
                }
            }
        }

        private void client_EventConnected(BL_TcpP2P sender)
        {
            if (EventConnected != null) EventConnected(sender);
        }

        private void client_EventReceived(BL_TcpP2P sender)
        {
            if (EventReceived != null) EventReceived(sender);
        }

        private void client_EventClosed(BL_TcpP2P sender)
        {
            ResetConnect();
            LoopNow = false;

            if (EventClosed != null) EventClosed(sender);

            sender.EventClosed -= client_EventClosed;
            sender.EventReceived -= client_EventReceived;
            sender.EventConnected -= client_EventConnected;
            sender.EventError -= client_EventError;

            lock (sockets)
            {
                for (int i = 0; i < sockets.Count; i++)
                {
                    if (sockets[i] == sender)
                    {
                        sockets.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        private void client_EventError(BL_TcpP2P sender, Exception ex)
        {
            if (EventError != null) EventError(sender, ex);
        }

        /// <summary>
        /// 接続待機を終了して、接続中のクライアントをすべて切断します
        /// </summary>
        public override void EndLink()
        {
            base.EndLink();

            lock (sockets)
            {
                for (int i = 0; i < sockets.Count; i++)
                {
                    try
                    {
                        sockets[i].EndLink();
                    }
                    catch { }
                }
            }
        }
    }
}
