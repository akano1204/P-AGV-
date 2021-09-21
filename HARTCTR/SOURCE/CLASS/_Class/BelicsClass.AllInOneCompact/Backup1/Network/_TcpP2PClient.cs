using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace BelicsClass.Network
{
    /// <summary>
    /// TcpP2PClient の概要の説明です。
    /// </summary>
    public class BL_TcpP2PClient : BL_TcpP2P
    {
        /// <summary>
        /// TCPエラーが発生した際に発生するイベントです
        /// </summary>
        public override event Event_Handler_Error EventError;

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="localIP">自分のIPアドレスを指定します。特に指定が無い場合はIPAddress.Anyを指定してください。</param>
        /// <param name="localPort">自分のポート番号を指定します。特に指定が無い場合は0を指定してください。</param>
        /// <param name="serverIP">接続先(サーバ)のIPアドレスを指定します。</param>
        /// <param name="serverPort">接続先(サーバ)のポート番号を指定します。</param>
        /// <param name="sendBufferCount">送信データのバッファ数を指定します。連続送信回数を考慮して設定してください。</param>
        /// <param name="receiveBufferCount">受信データのバッファ数を指定します。連続受信回数を考慮して設定してください。</param>
        public BL_TcpP2PClient(
            IPAddress localIP,
            int localPort,
            IPAddress serverIP,
            int serverPort,
            int sendBufferCount,
            int receiveBufferCount
            )
            : base(sendBufferCount, receiveBufferCount)
        {
            //ローカルとリモートのエンドポイントを記憶する
            m_Local = new IPEndPoint(localIP, localPort);
            m_Remote = new IPEndPoint(serverIP, serverPort);
        }

        /// <summary>
        /// コンストラクタです。
        /// </summary>
        /// <param name="localIP">自分のIPアドレスを指定します。特に指定が無い場合はIPAddress.Anyを指定してください。</param>
        /// <param name="localPort">自分のポート番号を指定します。特に指定が無い場合は0を指定してください。</param>
        /// <param name="serverIP">接続先(サーバ)のIPアドレスを指定します。</param>
        /// <param name="serverPort">接続先(サーバ)のポート番号を指定します。</param>
        /// <param name="sendBufferCount">送信データのバッファ数を指定します。連続送信回数を考慮して設定してください。</param>
        /// <param name="receiveBufferCount">受信データのバッファ数を指定します。連続受信回数を考慮して設定してください。</param>
        /// <param name="formatType">フォーマット種別を指定します。</param>
        public BL_TcpP2PClient(
            IPAddress localIP,
            int localPort,
            IPAddress serverIP,
            int serverPort,
            int sendBufferCount,
            int receiveBufferCount,
            BL_FormatType formatType
            )
            : base(sendBufferCount, receiveBufferCount, formatType)
        {
            //ローカルとリモートのエンドポイントを記憶する
            m_Local = new IPEndPoint(localIP, localPort);
            m_Remote = new IPEndPoint(serverIP, serverPort);
        }

        /// <summary>ローカルのエンドポイントです。</summary>
        public IPEndPoint m_Local = null;

        /// <summary>接続先です。</summary>
        public IPEndPoint m_Remote = null;

        /// <summary>
        /// リンクする
        /// </summary>
        protected override Socket Link()
        {
            Socket returnSocket = null;

            for (; ; )
            {
                bool blockingState = true;
                bool connected = false;

                try
                {
                    //ソケット生成／バインド
                    returnSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                    byte[] inBuffer = new byte[12];
                    BitConverter.GetBytes(1).CopyTo(inBuffer, 0);
                    BitConverter.GetBytes(10000).CopyTo(inBuffer, 4);
                    BitConverter.GetBytes(1000).CopyTo(inBuffer, 8);

                    returnSocket.IOControl(IOControlCode.KeepAliveValues, inBuffer, null);

                    returnSocket.Bind(m_Local);

                    //コネクト
                    returnSocket.Connect(m_Remote);

                    connected = true;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(e.ToString());
                    if (EventError != null) EventError(this, ex);

                    //Thread.Sleep(1000);
                }
                finally
                {
                    returnSocket.Blocking = blockingState;
                }

                if (connected)
                {
                    return returnSocket;
                }
                else
                {
                    try { returnSocket.Close(); }
                    catch { }
                }

                //待機
                Thread.Sleep(300);
            }
        }
    }
}
