using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Diagnostics;

namespace BelicsClass.Network
{
    /// <summary>
    /// TcpP2PServer の概要の説明です。
    /// </summary>
    public class BL_TcpP2PServer : BL_TcpP2P
    {
        /// <summary>
        /// 通信エラーが発生した際に発生するイベントです
        /// </summary>
        public override event Event_Handler_Error EventError;

        //public BL_TcpP2PServer(
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
        public BL_TcpP2PServer(
            IPAddress localIP,
            int localPort,
            int sendBufferCount,
            int receiveBufferCount
            )
            : base(sendBufferCount, receiveBufferCount)
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
        public BL_TcpP2PServer(
            IPAddress localIP,
            int localPort,
            int sendBufferCount,
            int receiveBufferCount,
            BL_FormatType formatType
            )
            : base(sendBufferCount, receiveBufferCount, formatType)
        {
            m_ListenerEndPoint = new IPEndPoint(localIP, localPort);
        }

        /// <summary>
        /// 接続待機用のIPエンドポイントを保持します
        /// </summary>
        protected IPEndPoint m_ListenerEndPoint;

        /// <summary>ローカルエンドポイント文字列を取得します。</summary>
        public IPEndPoint ListenerEndPoint { get { return m_ListenerEndPoint; } }

        private bool remoteLock = false;
        private Stopwatch remoteLockWatcher = new Stopwatch();

        /// <summary>
        /// 接続待機用のIPアドレスを取得します
        /// </summary>
        public IPAddress ListeningIPAddress { get { return m_ListenerEndPoint.Address; } }

        /// <summary>
        /// 接続待機用のポートNoを取得します
        /// </summary>
        public int ListeningPortNo { get { return m_ListenerEndPoint.Port; } }

        /// <summary>
        /// リモートロック状態を取得または設定します
        /// リモートロック状態の立ち上がりでストップウォッチを起動します
        /// </summary>
        public bool RemoteLock
        {
            get
            {
                return remoteLock;
            }
            set
            {
                if (remoteLock != value)
                {
                    if (value)
                    {
                        remoteLockWatcher.Reset();
                        remoteLockWatcher.Start();
                    }
                    else
                    {
                        remoteLockWatcher.Stop();
                    }
                }

                remoteLock = value;
            }
        }

        /// <summary>
        /// リンクする
        /// </summary>
        protected override Socket Link()
        {
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
                        if (!LoopNow) return null;
                        if (serverSocket.Poll(100, SelectMode.SelectRead)) break;
                        if (serverSocket.Poll(100, SelectMode.SelectError)) throw (new Exception("error"));

                        Thread.Sleep(20);
                    }
                    returnSocket = serverSocket.Accept();

                    //サーバソケットクローズ
                    //(サーバソケットはシャットダウンすると例外スローされる)
                    //serverSocket.Shutdown( SocketShutdown.Both );
                    serverSocket.Close();

                    return returnSocket;
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

        //protected override bool Loop_Watcher()
        //{
        //    if (sessionTimeout < remoteLockWatcher.ElapsedMilliseconds) return false;

        //    return base.Loop_Watcher();
        //}
    }
}
