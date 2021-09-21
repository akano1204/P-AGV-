using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections;
using System.Diagnostics;

using BelicsClass.Common;

namespace BelicsClass.Network
{
    /// <summary>
    /// スタンダードプロトコルの送信/受信を行う仮想クラスです。
    /// このクラスにコネクション確立(Link)を実装することで、サーバ用とクライアント用を派生されます。
    /// </summary>
    public abstract class BL_TcpP2P : BL_IPeerToPeer, IDisposable
    {
        /// <summary>
        /// イベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        public delegate void Event_Handler_Connected(BL_TcpP2P sender);
        /// <summary>
        /// イベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        public delegate void Event_Handler_Received(BL_TcpP2P sender);
        /// <summary>
        /// イベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        public delegate void Event_Handler_Closed(BL_TcpP2P sender);
        /// <summary>
        /// イベントハンドラーです。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        public delegate void Event_Handler_Error(BL_TcpP2P sender, Exception ex);

        /// <summary>接続された際に発生するイベントです</summary>
        public virtual event Event_Handler_Connected EventConnected;
        /// <summary>データを受信した際に発生するイベントです</summary>
        public virtual event Event_Handler_Received EventReceived;
        /// <summary>切断された際に発生するイベントです</summary>
        public virtual event Event_Handler_Closed EventClosed;
        /// <summary>通信エラーの際に発生するイベントです</summary>
        public virtual event Event_Handler_Error EventError;

        /// <summary>
        /// フォーマット
        /// </summary>
        public enum BL_FormatType
        {
			/// <summary>
			/// NONE
			/// </summary>
			NONE,
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
            BL_IDSIZE,
        }

        /// <summary>
        /// デフォルトコンストラクタです。
        /// </summary>
        private BL_TcpP2P() { }

        /// <summary>
        /// コンストラクタです。<b/>
        /// 通信は開始されません。
        /// </summary>
        /// <param name="sendBufferCount">送信バッファ数を指定します。</param>
        /// <param name="receiveBufferCount">受信バッファ数を指定します。</param>
        public BL_TcpP2P(int sendBufferCount, int receiveBufferCount)
        {
            //バッファ数の記憶(0以下ならデフォルト値にする)
            if (sendBufferCount <= 0) sendBufferCount = DEFAULT_BUFFER_COUNT;
            if (receiveBufferCount <= 0) receiveBufferCount = DEFAULT_BUFFER_COUNT;
            RECEIVE_BUFFER_COUNT = receiveBufferCount;
            SEND_BUFFER_COUNT = sendBufferCount;

            //送受信キューを生成
            m_SendQueue = new BL_EventSocketQueue(SEND_BUFFER_COUNT);
            m_ReceiveQueue = new BL_EventQueue(RECEIVE_BUFFER_COUNT);

            //スレッドを起動していない
            m_LoopNow = false;

            //ローカルとリモートのエンドポイントを初期化
            m_LocalEndPoint = m_RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, 0);
        }

        /// <summary>
        /// コンストラクタです。
        /// 通信は開始されません。
        /// 電文フォーマット種別を変更できます。
        /// </summary>
        /// <param name="sendBufferCount">送信バッファ数を指定します。</param>
        /// <param name="receiveBufferCount">受信バッファ数を指定します。</param>
        /// <param name="formatType">フォーマット種別を指定します。</param>
        public BL_TcpP2P(int sendBufferCount, int receiveBufferCount, BL_FormatType formatType)
            : this(sendBufferCount, receiveBufferCount)
        {
            m_FormatType = formatType;
        }

        /// <summary>
        /// デストラクタです。<b/>
        /// スレッドを停止してソケットをクローズします。
        /// </summary>
        ~BL_TcpP2P()
        {
            //ソケットおよびスレッドの後始末
            EndLink();
        }

        /// <summary>送信データのキューバッファです。</summary>
        protected BL_EventSocketQueue m_SendQueue = null;

        /// <summary>受信データのキューバッファです。</summary>
        protected BL_EventQueue m_ReceiveQueue = null;

        /// <summary>送信データのキューバッファ数です。</summary>
        public readonly int SEND_BUFFER_COUNT;

        /// <summary>受信データのキューバッファ数です。</summary>
        public readonly int RECEIVE_BUFFER_COUNT;

        /// <summary>デフォルトの送受信データキューバッファ数です。</summary>
        public const int DEFAULT_BUFFER_COUNT = 10;

        /// <summary>同期通信を行うスレッドです。</summary>
        private Thread m_Thread;

        /// <summary>内部でコネクション状態を保持しているフラグです。</summary>
        private bool m_Connected = false;

        ///// <summary>切断するまでの無通信時間</summary>
        //protected int sessionTimeout = 0;

        ///// <summary>無通信時間の計測用</summary>
        //protected Stopwatch watcher = new Stopwatch();

        /// <summary>接続済みソケット</summary>
        protected Socket LinkedSocket = null;

        /// <summary>コネクト状態を接続にします。</summary>
        protected void SetConnect()
        {
            lock (this)
            {
                m_Connected = true;
            }
        }

        /// <summary>コネクト状態を切断にします。</summary>
        protected void ResetConnect()
        {
            lock (this)
            {
                m_Connected = false;
            }
        }

        /// <summary>ローカルエンドポイントです。</summary>
        private IPEndPoint m_LocalEndPoint;

        /// <summary>リモートエンドポイントです。</summary>
        private IPEndPoint m_RemoteEndPoint;

        /// <summary>ローカルエンドポイント文字列を取得します。</summary>
        public IPEndPoint LocalEndPoint { get { return m_LocalEndPoint; } }
        
        /// <summary>リモートエンドポイント文字列を取得します。</summary>
        public IPEndPoint RemoteEndPoint { get { return m_RemoteEndPoint; } }

        /// <summary>電文フォーマット種別</summary>
        private BL_FormatType m_FormatType = BL_FormatType.BL_IDSIZE;

        /// <summary>電文フォーマット種別を取得します。</summary>
        public BL_FormatType FormatType { get { return m_FormatType; } }

        /// <summary>
        /// 同期IOを実現するループです。
        /// </summary>
        protected virtual void Loop()
        {
            //Select用のリスト
            ArrayList sockets = new ArrayList();

            //ソケット
            Socket socket = null;

            // 全体ループ
            for (; ; )
            {
                //ソケット例外を受け止めます
                try
                {
                    //リンク(サーバならAccept、クライアントならConnect)
                    socket = Link();

                    if (socket != null)
                    {
                        //watcher.Reset();
                        //watcher.Start();

                        lock (this)
                        {
                            m_LocalEndPoint = (IPEndPoint)socket.LocalEndPoint;
                            m_RemoteEndPoint = (IPEndPoint)socket.RemoteEndPoint;
                        }

                        //接続を記憶
                        SetConnect();

                        //受信中の状態をクリア
                        m_ReceiveState.Clear();

                        if (EventConnected != null) EventConnected(this);

                        //送受信ループ
                        while (socket.Connected && LoopNow)
                        {
                            //実際のソケット受信と送信用イベントソケット受信(送信要求)を待つ
                            sockets.Clear();
                            sockets.Add(socket);
                            sockets.Add(m_SendQueue.EventSocket);

                            //20ms待機(送受信待機)
                            Socket.Select(sockets, null, null, 20000);

                            //送信処理
                            Loop_Send(socket);

                            //受信処理
                            Loop_Receive(socket);

                            ////通信有効時間経過
                            //if (!Loop_Watcher()) break;
                        }

                        //watcher.Stop();
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message != "切断検知")
                    //&& ex.Message != "データID、データ数不一致"
                    //&& ex.Message != "データ数異常")
                    {
                        if (EventError != null) EventError(this, ex);
                        //Console.WriteLine(ex.ToString());
                    }
                }
                finally
                {
                    //watcher.Stop();

                    //切断を記憶
                    ResetConnect();

                    try
                    {
                        if (EventClosed != null) EventClosed(this);
                    }
                    catch { }

                    if (socket != null)
                    {
                        //ソケットをクローズ
                        try { socket.Shutdown(SocketShutdown.Both); }
                        catch { }
                        try { socket.Close(); }
                        catch { }
                    }

                    if (m_SendQueue != null)
                    {
                        //送信キューをクリア
                        try { m_SendQueue.Clear(); }
                        catch { }
                    }

                    socket = null;
                }

                if (!m_LoopNow) break;

                if (LinkedSocket != null) break;
            }
        }

        //protected virtual bool Loop_Watcher()
        //{
        //    if (0 < sessionTimeout && sessionTimeout < watcher.ElapsedMilliseconds) return false;

        //    return true;
        //}

        /// <summary>
        /// 送信処理です。
        /// </summary>
        /// <param name="socket">送信ソケットを指定します。</param>
        protected void Loop_Send(Socket socket)
        {
            //送信データをすべて送信する
            while (m_SendQueue.Count > 0)
            {
                //送信データ読み出し
                byte[] sendData = (byte[])m_SendQueue.Dequeue();

                //送信データなし
                if (sendData == null)
                {
                    break;
                }

                //送信
                socket.Send(sendData);

                //watcher.Reset();
                //watcher.Start();
            }
        }

        /// <summary>データIDとデータ数の長さです。</summary>
        public const int DATA_ID_SIZE = 8;

        /// <summary>データIDとデータ数を含んだ最大データ長です。</summary>
        public const int MAX_DATA_SIZE = 655350 + 8;

        ///// <summary>データIDとデータ数を含んだ最小データ長です。</summary>
        //public const int MIN_DATA_SIZE = 46;

        /// <summary>
        /// 受信状態を示します。
        /// </summary>
        protected class ReceiveState
        {
            /// <summary>受信データの仮記憶バッファです。</summary>
            public byte[] Buffer;

            /// <summary>Bufferに格納されている受信済みのデータ長(byte)です。</summary>
            public int ReceivedLength;

            /// <summary>受信中のデータサイズ(全データ長)を格納します。</summary>
            public int DataSize;

            /// <summary>STX～ETXフォーマット受信時、STXが検出されたことを表します。</summary>
            public bool StxCheck = false;

            /// <summary>コンストラクタです。</summary>
            public ReceiveState()
            {
                Buffer = new byte[MAX_DATA_SIZE];
                Clear();
            }

            /// <summary>受信状態を初期化します。</summary>
            public void Clear()
            {
                ReceivedLength = 0;
                DataSize = 0;
                StxCheck = false;
            }
        }

        /// <summary>受信中のデータを記憶するエリアです。</summary>
        protected ReceiveState m_ReceiveState = new ReceiveState();

        /// <summary>
        /// 受信です。
        /// </summary>
        /// <param name="socket">受信ソケットを指定します。</param>
        protected void Loop_Receive(Socket socket)
        {
            //受信データがある間は、すべて受信する
            while (socket.Poll(0, System.Net.Sockets.SelectMode.SelectRead))
            {
                //読み込み可能サイズ取得
                int readSize = socket.Available;

                ////受信データありにも関わらず、読み込み可能データなしは切断である
                if (readSize <= 0)
                {
                    throw (new Exception("切断検知"));
                }

                //watcher.Reset();
                //watcher.Start();

                switch (m_FormatType)
                {
                    case BL_FormatType.BL_IDSIZE:
                        {
                            #region ID部データサイズフォーマット

                            //ID部受信中か
                            if (m_ReceiveState.ReceivedLength < DATA_ID_SIZE)
                            {
                                //ID部を受信する
                                int receiveDataSize = socket.Receive(
                                    m_ReceiveState.Buffer,
                                    m_ReceiveState.ReceivedLength,
                                    DATA_ID_SIZE - m_ReceiveState.ReceivedLength,
                                    SocketFlags.None);
                                m_ReceiveState.ReceivedLength += receiveDataSize;

                                //ID部を受信できていない
                                if (m_ReceiveState.ReceivedLength < DATA_ID_SIZE)
                                {
                                    return;
                                }

                                //IDとデータサイズを取得
                                int offset = 0;
                                int dataID;
                                int dataCount;
                                BL_BitConverter.ToValue(m_ReceiveState.Buffer, ref offset, out dataID);
                                BL_BitConverter.ToValue(m_ReceiveState.Buffer, ref offset, out dataCount);

                                //ID部を含めたサイズ(全データ長)を求める
                                m_ReceiveState.DataSize = dataCount + DATA_ID_SIZE;

                                //データIDとデータ数が不一致なので、受信処理終了
                                if (ChangeDataCount(dataCount) != dataID)
                                {
                                    throw (new Exception("データID、データ数不一致"));
                                    //return;
                                }

                                //データサイズは範囲外なので、受信処理終了
                                if (m_ReceiveState.DataSize < DATA_ID_SIZE || MAX_DATA_SIZE < m_ReceiveState.DataSize)
                                {
                                    throw (new Exception("データ数異常"));
                                    //return;
                                }

                                if (0 < dataCount) continue;
                            }

                            if (0 < m_ReceiveState.DataSize - m_ReceiveState.ReceivedLength)
                            {
                                //データ部を受信する
                                m_ReceiveState.ReceivedLength += socket.Receive(
                                    m_ReceiveState.Buffer,
                                    m_ReceiveState.ReceivedLength,
                                    m_ReceiveState.DataSize - m_ReceiveState.ReceivedLength,
                                    SocketFlags.None);
                            }
                            //全データを受信していない
                            if (m_ReceiveState.ReceivedLength < m_ReceiveState.DataSize)
                            {
                                continue;
                            }

                            //受信データを待ち登録
                            byte[] datas = new byte[m_ReceiveState.DataSize - DATA_ID_SIZE];
                            if (0 < datas.Length)
                            {
                                Array.Copy(m_ReceiveState.Buffer, DATA_ID_SIZE, datas, 0, m_ReceiveState.DataSize - DATA_ID_SIZE);
                            }
                            m_ReceiveQueue.Enqueue(datas);

                            #endregion
                        }
                        break;

                    case BL_FormatType.CR:
                        {
                            #region ～CRフォーマット

                            bool detect = false;
                            for (int pos = 0; pos < readSize; pos++)
                            {
                                m_ReceiveState.ReceivedLength += socket.Receive(
                                    m_ReceiveState.Buffer,
                                    m_ReceiveState.ReceivedLength,
                                    1,
                                    SocketFlags.None);

                                if (1 <= m_ReceiveState.ReceivedLength)
                                {
                                    if (m_ReceiveState.Buffer[m_ReceiveState.ReceivedLength - 1] == 0x0D)
                                    {
                                        //受信データを待ち登録
                                        byte[] datas = new byte[m_ReceiveState.ReceivedLength - 1];
                                        if (0 < datas.Length)
                                        {
                                            Array.Copy(m_ReceiveState.Buffer, 0, datas, 0, m_ReceiveState.ReceivedLength - 1);
                                        }
                                        m_ReceiveQueue.Enqueue(datas);

                                        detect = true;
                                        break;
                                    }
                                }
                            }

                            //全データを受信していない
                            if (!detect) continue;

                            #endregion
                        }
                        break;

                    case BL_FormatType.CR_LF:
                        {
                            #region ～CRLFフォーマット

                            bool detect = false;
                            for (int pos = 0; pos < readSize; pos++)
                            {
                                m_ReceiveState.ReceivedLength += socket.Receive(
                                    m_ReceiveState.Buffer,
                                    m_ReceiveState.ReceivedLength,
                                    1,
                                    SocketFlags.None);

                                if (2 <= m_ReceiveState.ReceivedLength)
                                {
                                    if (m_ReceiveState.Buffer[m_ReceiveState.ReceivedLength - 2] == 0x0D &&
                                        m_ReceiveState.Buffer[m_ReceiveState.ReceivedLength - 1] == 0x0A)
                                    {
                                        //受信データを待ち登録
                                        byte[] datas = new byte[m_ReceiveState.ReceivedLength - 1];
                                        if (0 < datas.Length)
                                        {
                                            Array.Copy(m_ReceiveState.Buffer, 0, datas, 0, m_ReceiveState.ReceivedLength - 1);
                                        }
                                        m_ReceiveQueue.Enqueue(datas);

                                        detect = true;
                                        break;
                                    }
                                }
                            }

                            //全データを受信していない
                            if (!detect) continue;

                            #endregion
                        }
                        break;

                    case BL_FormatType.STX_ETX:
                        {
                            #region STX～ETXフォーマット

                            bool detect = false;
                            for (int pos = 0; pos < readSize; pos++)
                            {
                                m_ReceiveState.ReceivedLength += socket.Receive(
                                    m_ReceiveState.Buffer,
                                    m_ReceiveState.ReceivedLength,
                                    1,
                                    SocketFlags.None);

                                if (!m_ReceiveState.StxCheck)
                                {
                                    if (1 <= m_ReceiveState.ReceivedLength)
                                    {
                                        if (m_ReceiveState.Buffer[m_ReceiveState.ReceivedLength - 1] == 0x02)
                                        {
                                            m_ReceiveState.StxCheck = true;
                                        }
                                    }
                                }
                                else
                                {
                                    if (2 <= m_ReceiveState.ReceivedLength)
                                    {
                                        if (m_ReceiveState.Buffer[m_ReceiveState.ReceivedLength - 1] == 0x03)
                                        {
                                            //受信データを待ち登録
                                            byte[] datas = new byte[m_ReceiveState.ReceivedLength - 2];
                                            if (0 < datas.Length)
                                            {
                                                Array.Copy(m_ReceiveState.Buffer, 1, datas, 0, m_ReceiveState.ReceivedLength - 2);
                                            }
                                            m_ReceiveQueue.Enqueue(datas);

                                            detect = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            //全データを受信していない
                            if (!detect) continue;

                            #endregion
                        }
                        break;

					case BL_FormatType.NONE:
						{
							#region NONE

							m_ReceiveState.ReceivedLength = socket.Receive(m_ReceiveState.Buffer, 0, readSize, SocketFlags.None);

							//受信データを待ち登録
							byte[] datas = new byte[m_ReceiveState.ReceivedLength];
							if (0 < datas.Length)
							{
								Array.Copy(m_ReceiveState.Buffer, 0, datas, 0, m_ReceiveState.ReceivedLength);
							}
							m_ReceiveQueue.Enqueue(datas);

							#endregion
						}
						break;
                }


                //受信済みバイト数をクリアして、受信の準備
                m_ReceiveState.Clear();

                try
                {
                    if (EventReceived != null) EventReceived(this);
                }
                catch (Exception ex)
                {
                    if (EventError != null) EventError(this, ex);
                }
            }
        }


        /// <summary>
        /// 指定したデータを送信します。<b/>
        /// 本メソッドは直ちに制御を返します。
        /// </summary>
        /// <param name="datas">送信するデータを指定します。</param>
        /// <returns>通信相手とリンクできていない場合falseを返し、
        /// リンクできている場合trueを返します。<b/>
        /// 送信ができたことを意味していません。</returns>
        public virtual bool Send(byte[] datas)
        {
            return Send(datas, 0, datas.Length);
        }

        /// <summary>
        /// 指定したデータを送信します。<b/>
        /// 本メソッドは直ちに制御を返します。
        /// </summary>
        /// <param name="datas">送信するデータを指定します。</param>
        /// <param name="offset">dataの送信開始オフセットを指定します。</param>
        /// <param name="length">送信するデータ長(byte)を指定します。</param>
        /// <returns>
        /// 通信相手に送信する準備が整っていない場合falseを返します。<b/>
        /// 通信相手に送信する準備が整った場合trueを返します。<b/>
        /// 戻り値は送信ができたことを意味しません。</returns>
        public virtual bool Send(byte[] datas, int offset, int length)
        {
            try
            {
                //未接続なので送信しない
                if (Connected == false)
                {
                    return false;
                }

                //データ長が異常なので送信しない
                if (MAX_DATA_SIZE < length)
                {
                    return false;
                }

                switch (m_FormatType)
                {
                    case BL_FormatType.BL_IDSIZE:
                        {
                            //送信データバッファ編集
                            byte[] sendBuffer = new byte[8 + length];
                            if (0 < length)
                            {
                                Array.Copy(datas, offset, sendBuffer, 8, length);
                            }
                            else
                            {
                            }

                            //データID、データ数(データ長からデータIDとデータ数を除いた長さ)設定
                            offset = 0;
                            BL_BitConverter.ToBytes(sendBuffer, ref offset, ChangeDataCount(length));
                            BL_BitConverter.ToBytes(sendBuffer, ref offset, length);

                            //キュー登録
                            return m_SendQueue.Enqueue(sendBuffer);
                        }

                    case BL_FormatType.CR:
                        {
                            //送信データバッファ編集
                            byte[] sendBuffer = new byte[length + 1];
                            if (0 < length)
                            {
                                Array.Copy(datas, offset, sendBuffer, 0, length);
                                sendBuffer[length] = 0x0D;
                            }
                            else
                            {
                            }

                            //キュー登録
                            return m_SendQueue.Enqueue(sendBuffer);
                        }

                    case BL_FormatType.CR_LF:
                        {
                            //送信データバッファ編集
                            byte[] sendBuffer = new byte[length + 2];
                            if (0 < length)
                            {
                                Array.Copy(datas, offset, sendBuffer, 0, length);
                                sendBuffer[length] = 0x0D;
                                sendBuffer[length + 1] = 0x0A;
                            }
                            else
                            {
                            }

                            //キュー登録
                            return m_SendQueue.Enqueue(sendBuffer);
                        }

                    case BL_FormatType.STX_ETX:
                        {
                            //送信データバッファ編集
                            byte[] sendBuffer = new byte[length + 2];
                            if (0 < length)
                            {
                                sendBuffer[0] = 0x02;
                                Array.Copy(datas, offset, sendBuffer, 1, length);
                                sendBuffer[length + 1] = 0x03;
                            }
                            else
                            {
                            }

                            //キュー登録
                            return m_SendQueue.Enqueue(sendBuffer);
                        }

					case BL_FormatType.NONE:
						{
							//送信データバッファ編集
							byte[] sendBuffer = new byte[length];
							if (0 < length)
							{
								Array.Copy(datas, offset, sendBuffer, 0, length);
							}
							else
							{
							}

							//キュー登録
							return m_SendQueue.Enqueue(sendBuffer);
						}
                }
            }
            catch { }

            return false;
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

        /// <summary>
        /// 受信データを取得します。<b/>
        /// 本メソッドは直ちに制御を返します。
        /// </summary>
        /// <param name="data">受信データを返します。<b/>
        /// 受信データが無い場合、nullを返します。</param>
        /// <returns>受信データがある場合trueを返し、無い場合falseを返します。</returns>
        public virtual bool Receive(out byte[] data)
        {
            //受信キューから読み出し
            data = (byte[])m_ReceiveQueue.Dequeue();
            return data != null;
        }

        /// <summary>
        /// 受信データを取得します。<b/>
        /// 本メソッドは直ちに制御を返します。
        /// </summary>
        /// <returns>
        /// 受信データを返します。<b/>
        /// 受信データが無い場合、nullを返します。
        /// </returns>
        public byte[] Receive()
        {
            byte[] recvBytes;
            Receive(out recvBytes);
            return recvBytes;
        }

        /// <summary>
        /// 受信イベントハンドルを取得します。
        /// </summary>
        public virtual System.Threading.WaitHandle WaitHandle
        {
            get
            {
                return m_ReceiveQueue.GetWaitHandle();
            }
        }

        /// <summary>
        /// 通信相手とのリンク状態を取得します。
        /// </summary>
        public virtual bool Connected
        {
            get
            {
                lock (this)
                {
                    return m_Connected;
                }
            }
        }

        /// <summary>
        /// サーバ/クライアントの特化処理です。
        /// 本メソッドでコネクション確立を行います。<b/>
        /// </summary>
        protected abstract System.Net.Sockets.Socket Link();

        /// <summary>
        /// 通信処理を開始しているかいなかを取得します。
        /// </summary>
        public virtual bool Started
        {
            get
            {
                lock (this)
                {
                    return m_Thread != null;
                }
            }
        }

        /// <summary>
        /// 通信を開始します。<b/>
        /// 通信を開始している場合は何もしません。
        /// </summary>
        public virtual void StartLink()
        {
            lock (this)
            {
                //スレッドのループ中を有効にする
                LoopNow = true;

                //スレッドが生成されているので何もしない。
                if (m_Thread != null)
                {
                    return;
                }

                //スレッドを生成して実行
                m_Thread = new Thread(new ThreadStart(Loop));
                m_Thread.IsBackground = true;
                m_Thread.Priority = ThreadPriority.Normal;
                m_Thread.Name = this.ToString() + "-Loop Thread";
                m_Thread.Start();
            }
        }
        /// <summary>
        /// 通信処理を停止します。<b/>
        /// 通信相手とのコネクションが確立されている場合、切断します。
        /// </summary>
        public virtual void EndLink()
        {
            Thread thread = null;
            lock (this)
            {
                //スレッドのループ中を無効にする
                LoopNow = false;

                //スレッドが生成されていないので何もしない。
                if (m_Thread == null)
                {
                    return;
                }

                //スレッドを止める
                thread = m_Thread;
                m_Thread = null;
            }

            //スレッド停止を待つ
            //[スレッドの終了処理で排他するので、lockしない状態で終了を待機する]
            //thread.Join();
			thread.Abort();
        }

        /// <summary>
        /// スレッド内のループ中フラグです。
        /// </summary>
        protected bool LoopNow
        {
            get
            {
                lock (this)
                {
                    return m_LoopNow;
                }
            }
            set
            {
                lock (this)
                {
                    m_LoopNow = value;
                }
            }
        }

        /// <summary>
        /// スレッド内のループ中フラグです。
        /// </summary>
        protected bool m_LoopNow;

        /// <summary>
        /// 当該インスタンス
        /// </summary>
        public virtual void Dispose()
        {
            if (m_Disposed) return;

            EndLink();
            m_SendQueue.Dispose();
            m_SendQueue = null;

            m_Disposed = true;
        }
        private bool m_Disposed;

        /// <summary>
        /// ローカルIPアドレスを取得します。
        /// 最後に接続した時の値になります。
        /// </summary>
        public virtual IPAddress LocalIPAddress
        {
            get
            {
                lock (this)
                {
                    return m_LocalEndPoint.Address;
                }
            }
        }

        /// <summary>
        /// ローカルポート番号を取得します。
        /// 最後に接続した時の値になります。
        /// </summary>
        public virtual int LocalPort
        {
            get
            {
                lock (this)
                {
                    return m_LocalEndPoint.Port;
                }
            }
        }

        /// <summary>
        /// リモートIPアドレスを取得します。
        /// 最後に接続した時の値になります。
        /// </summary>
        public virtual IPAddress RemoteIPAddress
        {
            get
            {
                lock (this)
                {
					if (m_RemoteEndPoint != null)
					{
						return m_RemoteEndPoint.Address;
					}
					return IPAddress.Any;
                }
            }
        }

        /// <summary>
        /// リモートポート番号を取得します。
        /// 最後に接続した時の値になります。
        /// </summary>
        public virtual int RemotePort
        {
            get
            {
                lock (this)
                {
					if (m_RemoteEndPoint != null)
					{
						return m_RemoteEndPoint.Port;
					}
					else return 0;
                }
            }
        }

		/// <summary>
		/// 文字列を返します。
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			if (RemoteEndPoint != null)
			{
				return "TcpP2P [" + RemoteIPAddress.ToString() + ":" + RemotePort.ToString() + "]";
			}
			else
			{
				return "TcpP2P [null:null]";
			}
		}
    }
}
