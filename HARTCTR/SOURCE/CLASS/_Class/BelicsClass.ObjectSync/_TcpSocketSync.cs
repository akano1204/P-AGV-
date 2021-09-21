using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Windows.Forms;

using BelicsClass.Common;
using BelicsClass.Network;

namespace BelicsClass.ObjectSync
{
    /// <summary>
    /// クラスインスタンスをTCPソケット通信で送受信するクラス
    /// 共有メモリの変化を検出し、イベントを発生させます
    /// 共有メモリを利用しない場合、イベントは発生しません
    /// 
    /// 本クラスから派生して、TCPソケット共有アクセスを行いたいフィールドが定義されたクラスを設計してください
    /// フィールドは本クラスのInitializeメソッドコールによって解析され、バイト配列とオブジェクト間の相互変換が行われます
    /// null値はフィールドの初期値として使用できません。必ずコンストラクタで初期値を設定してください
    /// stringフィールドは、必要文字数分の空白で初期化してください
    /// 
    /// TCPサーバーとTCPクライアントの両機能を保有します
    /// TCPサーバーに接続されるとクライアントからのデータ変更を受け取り、本オブジェクトにデータが反映されます
    /// TCPサーバーはマルチクライアント接続を行い、複数のクライアントからのデータ変更を処理します
    /// TCPサーバーはクライアントからのデータ変更によってEventHandler_Updatedイベントを発生させます
    /// 
    /// TCPクライアントを接続するとサーバー側へ本オブジェクトのデータ変更を送信することができます
    /// 
    /// フィールドには「下記の組み込み型、および下記で構成されたクラス、配列」を使用することができます
    /// Boolean     4byte
    /// Int16       2byte
    /// Int32       4byte
    /// Int64       8byte
    /// UInt16      2byte
    /// UInt32      4byte
    /// UInt64      8byte
    /// Double      8byte
    /// Single      4byte
    /// Byte        1byte
    /// Char        1byte
    /// String      ??? byte
    /// 
    /// </summary>
    [Serializable]
    public class BL_TcpSocketSync : BL_FaceMemorySyncNotify
    {
        #region 例外情報クラス

        /// <summary>
        /// TCP例外情報をカプセル化します
        /// </summary>
        [Serializable]
        public class TcpSocketSyncException
        {
            /// <summary>
            /// 発生源
            /// </summary>
            public BL_TcpP2P sender = null;

            /// <summary>
            /// 例外
            /// </summary>
            public Exception ex = null;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public TcpSocketSyncException() { }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="sender">発生源</param>
            /// <param name="ex">例外</param>
            public TcpSocketSyncException(BL_TcpP2P sender, Exception ex)
            {
                this.sender = sender;
                this.ex = ex;
            }
        }

        #endregion

        #region TCP通信ヘッダークラス

        /// <summary>
        /// TCP通信ヘッダーをカプセル化します
        /// </summary>
        [Serializable]
        public class TcpTransportHeader : BL_ObjectSync
        {
            /// <summary>
            /// 通信ID
            /// 通信ごとにインクリメントされます
            /// </summary>
            [BL_ObjectSyncAttribute] public int Id = 0;

            /// <summary>
            /// 更新対象フィールド名
            /// </summary>
            [BL_ObjectSyncAttribute] public string TargetField = "                                                  ";

            /// <summary>
            /// 送信操作フラグ
            /// </summary>
            [BL_ObjectSyncAttribute] public byte Send = 0;

            /// <summary>
            /// 受信操作フラグ
            /// </summary>
            [BL_ObjectSyncAttribute] public byte Receive = 0;

            /// <summary>
            /// ロック操作フラグ
            /// </summary>
            [BL_ObjectSyncAttribute] public byte Lock = 0;

            /// <summary>
            /// アンロック操作フラグ
            /// </summary>
            [BL_ObjectSyncAttribute] public byte Unlock = 0;

            /// <summary>
            /// 本体データを元に通信データ全体を生成します
            /// </summary>
            /// <param name="bodydata"></param>
            /// <returns></returns>
            public byte[] make_senddata(byte[] bodydata)
            {
                byte[] headerdata = GetBytes();
                byte[] data = new byte[headerdata.Length + bodydata.Length];
                Array.Copy(headerdata, 0, data, 0, headerdata.Length);
                if (0 < bodydata.Length)
                {
                    Array.Copy(bodydata, 0, data, headerdata.Length, bodydata.Length);
                }

                return data;
            }

            /// <summary>
            /// 通信データ全体を元に本体データを取得します
            /// </summary>
            /// <param name="data"></param>
            /// <returns></returns>
            public byte[] get_bodydata(byte[] data)
            {
                byte[] headerdata = new byte[Length];
                byte[] bodydata = new byte[data.Length - Length];

                Array.Copy(data, 0, headerdata, 0, headerdata.Length);

                if (0 < bodydata.Length)
                {
                    Array.Copy(data, headerdata.Length, bodydata, 0, data.Length - Length);
                }

                SetBytes(headerdata);
                return bodydata;
            }

            private byte[] prev_bodydata = null;

            /// <summary>
            /// 直前の本体データを取得または設定します
            /// </summary>
            public byte[] prevBodyData
            {
                get { return prev_bodydata; }
                set
                {
                    prev_bodydata = new byte[value.Length];
                    Array.Copy(value, prev_bodydata, value.Length);
                }
            }
        }

        #endregion

        private int id = 0;

        /// <summary>
        /// 通信IDを取得します
        /// </summary>
        public int Id { get { return id; } }

        [NonSerialized]
        private List<TcpTransportHeader> pendingHeaders = new List<TcpTransportHeader>();

        #region イベントハンドラー

        /// <summary>
        /// サーバー受信によってフィールが更新されたことを通知するイベントデリゲート
        /// </summary>
        /// <param name="sender">発生源</param>
        /// <param name="remote_endpoint">更新通知元のエンドポイント</param>
        /// <param name="field_name">更新フィールド名</param>
        public delegate void Event_Handler_Updated(BL_TcpSocketSync sender, IPEndPoint remote_endpoint, string field_name);

        /// <summary>
        /// サーバー受信によってフィールが更新されたことを通知するイベント
        /// </summary>
        public event Event_Handler_Updated EventHandler_ListenerUpdated;

        /// <summary>
        /// サーバー受信によってフィールが更新されたことを通知するイベント
        /// </summary>
        public event Event_Handler_Updated EventHandler_ClientUpdated;

        /// <summary>
        /// サーバー受信によってフィールが更新されたことを通知するイベント
        /// </summary>
        public event Event_Handler_Updated EventHandler_LoopbackUpdated;

        /// <summary>
        /// 本クラスのサーバーにクライアントが接続されたことを通知するイベント
        /// </summary>
        /// <param name="sender">発生源</param>
        /// <param name="remote_endpoint">接続されたエンドポイント</param>
        public delegate void Event_Handler_ListenerConnected(BL_TcpSocketSync sender, IPEndPoint remote_endpoint);

        /// <summary>
        /// 本クラスのサーバーにクライアントが接続されたことを通知するイベント
        /// </summary>
        public event Event_Handler_ListenerConnected EventHandler_ListenerConnected;

        /// <summary>
        /// 本クラスのサーバーにクライアントが接続されたことを通知するイベント
        /// </summary>
        public event Event_Handler_ListenerConnected EventHandler_ListenerLoopbackConnected;

        /// <summary>
        /// 本クラスのサーバーにクライアントが接続されたことを通知するイベント
        /// </summary>
        /// <param name="sender">発生源</param>
        /// <param name="remote_endpoint">切断されたエンドポイント</param>
        public delegate void Event_Handler_ListenerClosed(BL_TcpSocketSync sender, IPEndPoint remote_endpoint);

        /// <summary>
        /// 本クラスのサーバーにクライアントが接続されたことを通知するイベント
        /// </summary>
        public event Event_Handler_ListenerClosed EventHandler_ListenerClosed;

        /// <summary>
        /// 通信異常が発生したことを通知するイベントデリゲート
        /// </summary>
        /// <param name="sender">発生源</param>
        /// <param name="error">TCP例外情報</param>
        public delegate void Event_Handler_Error(BL_TcpSocketSync sender, TcpSocketSyncException error);

        /// <summary>
        /// 通信異常が発生したことを通知するイベント
        /// </summary>
        public event Event_Handler_Error EventHandler_ListenerError;

        /// <summary>
        /// 本クラスのサーバーにクライアントが接続されたことを通知するイベント
        /// </summary>
        /// <param name="sender">発生源</param>
        /// <param name="remote_endpoint">接続されたエンドポイント</param>
        public delegate void Event_Handler_ClientConnected(BL_TcpSocketSync sender, IPEndPoint remote_endpoint);

        /// <summary>
        /// 本クラスのサーバーにクライアントが接続されたことを通知するイベント
        /// </summary>
        public event Event_Handler_ClientConnected EventHandler_ClientConnected;

        /// <summary>
        /// 本クラスのサーバーにクライアントが接続されたことを通知するイベント
        /// </summary>
        /// <param name="sender">発生源</param>
        /// <param name="remote_endpoint">切断されたエンドポイント</param>
        public delegate void Event_Handler_ClientClosed(BL_TcpSocketSync sender, IPEndPoint remote_endpoint);

        /// <summary>
        /// 本クラスのサーバーにクライアントが接続されたことを通知するイベント
        /// </summary>
        public event Event_Handler_ClientClosed EventHandler_ClientClosed;

        /// <summary>
        /// 通信異常が発生したことを通知するイベント
        /// </summary>
        public event Event_Handler_Error EventHandler_ClientError;

        #endregion

        [NonSerialized]
        private BL_TcpP2PMultiServer multiListener = null;

        [NonSerialized]
        private Dictionary<string, BL_TcpP2PClient> dictClient = new Dictionary<string, BL_TcpP2PClient>();

        private int millisecondsTimeout = -1;

        private bool is_server = false;
        /// <summary>
        /// 全接続を管理するサーバー自身であるかどうかを取得します
        /// </summary>
        public bool IsServer { get { return is_server; } }

        /// <summary>
        /// サーバーに接続しているクライアント数を保持します
        /// </summary>
        [BL_ObjectSyncAttribute] protected int connectedRemoteClients = 0;
        /// <summary>
        /// サーバーに接続しているクライアント数を取得します
        /// </summary>
        public int ConnectedRemoteClients { get { return connectedRemoteClients; } }

        /// <summary>
        /// 最後に送信が行われた接続数
        /// </summary>
        private int lastSendClients = 0;

        /// <summary>
        /// 最後に受信が行われた接続数
        /// </summary>
        private int lastReceiveClients = 0;

        /// <summary>
        /// リモートロック状態
        /// </summary>
        private bool remoteLock = false;

        /// <summary>
        /// 自ロック状態
        /// </summary>
        private bool clientLock = false;

        /// <summary>
        /// 接続準備が行われているクライアント数を取得します
        /// 接続数ではありません
        /// </summary>
        public int Clients { get { return dictClient.Count; } }

        /// <summary>
        /// サーバーに接続しているクライアント数を取得します
        /// </summary>
        public int ConnectedListeners
        {
            get
            {
                if (multiListener == null) return 0;
                return multiListener.ConnectedClients;
            }
        }

        /// <summary>
        /// リスナーの初期化済み状態を取得します
        /// </summary>
        public bool IsInitializedListener { get { return multiListener != null; } }

        /// <summary>
        /// 接続しているクライアント数を取得します
        /// </summary>
        public int ConnectedLocalClients
        {
            get
            {
                int count = 0;

                lock (dictClient)
                {
                    foreach (var v in dictClient.Values)
                    {
                        if (v.Connected) count++;
                    }
                }

                return count;
            }
        }

        /// <summary>
        /// リモートロック状態を取得します
        /// </summary>
        public bool IsRemoteLock
        {
            get
            {
                return multiListener.RemoteLock;
            }
        }

        /// <summary>
        /// クライアントロック状態を取得します
        /// </summary>
        public bool IsClientLock { get { return clientLock; } }

        /// <summary>
        /// 最後に送信が行われた接続数を取得します
        /// </summary>
        public int LastSendClients { get { return lastSendClients; } }

        /// <summary>
        /// 最後に受信が行われた接続数
        /// </summary>
        public int LastReceiveClients { get { return lastReceiveClients; } }

        /// <summary>
        /// 最後に発生したエラーのメッセージを保持します
        /// </summary>
        public string ErrorMessage = "                                                                                ";

        private bool autoSend = false;
        private bool autoReceive = false;

        /// <summary>
        /// 自動送信状態を取得または設定します
        /// </summary>
        public bool AutoSend { get { return autoSend; } set { autoSend = value; } }

        /// <summary>
        /// 自動受信状態を取得または設定します
        /// </summary>
        public bool AutoReceive { get { return autoReceive; } set { autoReceive = value; } }

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// 通信タイムアウトなし
        /// </summary>
        public BL_TcpSocketSync()
            : this(0)
        {
        }

        /// <summary>
        /// コンストラクタ
        /// 指定時間で通信タイムアウトします
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        public BL_TcpSocketSync(int millisecondsTimeout)
        {
            this.millisecondsTimeout = millisecondsTimeout;
        }

        #endregion

        #region デストラクタ

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~BL_TcpSocketSync()
        {
            Dispose();
        }

        /// <summary>
        /// 破棄
        /// クライアント・サーバーの切断と破棄を行います
        /// </summary>
        public override void Dispose()
        {
            lock (dictClient)
            {
                foreach (BL_TcpP2PClient client in dictClient.Values)
                {
                    client.EventReceived -= new BL_TcpP2P.Event_Handler_Received(client_EventReceived);
                    client.EventClosed -= client_EventClosed;
                    client.Dispose();
                }

                dictClient.Clear();
            }

            if (multiListener != null)
            {
                multiListener.Dispose();
                multiListener = null;
            }

            base.Dispose();
        }

        #endregion

        #region 初期化

        /// <summary>
        /// 初期化します
        /// 名前を指定することによって共有メモリが生成されます
        /// 同一名の共有メモリが既存の場合は、共有メモリからデータが読み出されて初期化されます
        /// 自オブジェクトが所持するprivate以外のフィールドを解析して、必要数分のバイト配列を生成するための準備が行われます
        /// 本メソッド実行後、フィールド名文字列によるデータ操作が可能となります
        /// 
        /// 本メソッド実行前に、EnableNotifyをtrueに設定することによってイベントが発生するようになります
        /// 共有メモリの監視インターバルを指定することができます
        /// 
        /// </summary>
        /// <param name="sleep_milliseconds">共有メモリ監視インターバル</param>
        /// <param name="x">自インスタンスの初期化時に反映させる初期化オブジェクト</param>
        /// <param name="sharemem_name">共有メモリの名称</param>
        /// <returns>初期化成功でtrueを返します</returns>
        public override bool Initialize<T>(int sleep_milliseconds, object x, string sharemem_name)
        {
            if (sharemem_name != "" && autoSend)
            {
                this.EventModified += BL_TcpSocketSync_EventModified;
            }

            bool ret = base.Initialize<T>(sleep_milliseconds, x, sharemem_name);

            return ret;
        }

        /// <summary>
        /// 初期化します
        /// 名前を指定することによって共有メモリが生成されます
        /// 同一名の共有メモリが既存の場合は、共有メモリからデータが読み出されて初期化されます
        /// 自オブジェクトが所持するprivate以外のフィールドを解析して、必要数分のバイト配列を生成するための準備が行われます
        /// 本メソッド実行後、フィールド名文字列によるデータ操作が可能となります
        /// 
        /// 本メソッド実行前に、EnableNotifyをtrueに設定することによってイベントを発生させます
        /// 共有メモリの監視インターバルは200msです。
        /// 
        /// </summary>
        /// <param name="x">自インスタンスの初期化時に反映させる初期化オブジェクト</param>
        /// <param name="sharemem_name">共有メモリの名称</param>
        /// <returns>初期化成功でtrueを返します</returns>
        public override bool Initialize(object x, string sharemem_name)
        {
            if (sharemem_name != "" && autoSend)
            {
                this.EventModified += BL_TcpSocketSync_EventModified;
            }

            bool ret = base.Initialize(x, sharemem_name);

            return ret;
        }

        #endregion

        void BL_TcpSocketSync_EventModified(BL_ObjectSync sender, string[] field_names)
        {
            foreach (string field_name in field_names)
            {
                SendMemory(field_name, true, false, false, null);
            }
        }

        /// <summary>
        /// 自インスタンスの指定フィールドデータを共有メモリへ書き込みます
        /// ロック有無を指定できます
        /// 自動送信有効な場合、データ変更を接続済みサーバーに書き込みます
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="lockFlag">ロック有無</param>
        /// <returns>正常に書き込みできたらtrueを返します</returns>
        public override bool WriteMemory(string field_name, bool lockFlag)
        {
            bool ret = base.WriteMemory(field_name, lockFlag);
            if (ret && autoSend)
            {
                ret = (0 < SendMemory(field_name, false, true, false, null));
            }

            return ret;
        }

        /// <summary>
        /// 共有メモリから指定フィールドへデータを読み込みます
        /// ロック有無を指定できます
        /// 自動受信が有効な場合、接続済みサーバーへデータ要求を行い、サーバー側のデータを読み込みます
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="lockFlag">ロック有無</param>
        /// <returns></returns>
        public override object ReadMemory(string field_name, bool lockFlag)
        {
            bool received = false;
            if (autoReceive)
            {
                received = (0 < ReceiveMemory("", field_name, false, true, false));
            }

            if (received) return base.ReadMemory(field_name, lockFlag);

            return null;
        }

        #region リスナー処理

        /// <summary>
        /// リスナー（サーバー）初期化
        /// 任意のIPアドレスで接続待機します
        /// 
        /// 接続待機ポートで待ち受けを行い、任意の空きポートでクライアントとの接続が行われます
        /// クライアント接続後、引き続き接続待機を行います
        /// </summary>
        /// <param name="portno">接続待機用ポートNo</param>
        public void Initialize_Listener(int portno)
        {
            Initialize_Listener(IPAddress.Any, portno);
        }

        /// <summary>
        /// リスナー（サーバー）初期化
        /// 指定IPアドレスで接続待機します
        /// 
        /// 接続待機ポートで待ち受けを行い、任意の空きポートでクライアントとの接続が行われます
        /// クライアント接続後、引き続き接続待機を行います
        /// </summary>
        /// <param name="ipaddress">IPアドレス</param>
        /// <param name="portno">接続待機用ポートNo</param>
        public void Initialize_Listener(string ipaddress, int portno)
        {
            Initialize_Listener(IPAddress.Parse(ipaddress), portno);
        }

        /// <summary>
        /// リスナー（サーバー）初期化
        /// 指定IPアドレスで接続待機します
        /// 
        /// 接続待機ポートで待ち受けを行い、任意の空きポートでクライアントとの接続が行われます
        /// クライアント接続後、引き続き接続待機を行います
        /// </summary>
        /// <param name="ipaddress">IPアドレス</param>
        /// <param name="portno">接続待機用ポートNo</param>
        public virtual void Initialize_Listener(IPAddress ipaddress, int portno)
        {
            if (multiListener != null)
            {
                multiListener.EventConnected -= new BL_TcpP2P.Event_Handler_Connected(listener_EventConnected);
                multiListener.EventReceived -= new BL_TcpP2P.Event_Handler_Received(listener_EventReceived);
                multiListener.EventClosed -= new BL_TcpP2P.Event_Handler_Closed(listener_EventClosed);
                multiListener.EventError -= new BL_TcpP2P.Event_Handler_Error(listener_EventError);
                multiListener.Dispose();
            }

            multiListener = new BL_TcpP2PMultiServer(ipaddress, portno, Length + 8, Length + 8);
            //multiListener = new BL_TcpP2PMultiServer(ipaddress, portno, Length + 8, Length + 8, millisecondsTimeout);
            multiListener.EventConnected += new BL_TcpP2P.Event_Handler_Connected(listener_EventConnected);
            multiListener.EventReceived += new BL_TcpP2P.Event_Handler_Received(listener_EventReceived);
            multiListener.EventClosed += new BL_TcpP2P.Event_Handler_Closed(listener_EventClosed);
            multiListener.EventError += new BL_TcpP2P.Event_Handler_Error(listener_EventError);

            multiListener.StartLink();

        }

        /// <summary>
        /// リスナーを終了します
        /// </summary>
        public void Destroy_Listener()
        {
            multiListener.Dispose();
            multiListener = null;
        }

        /// <summary>
        /// リスナー接続イベント処理
        /// 必要に応じてオーバーライドして処理を追加してください
        /// </summary>
        /// <param name="sender">接続TCPオブジェクト</param>
        protected virtual void listener_EventConnected(BL_TcpP2P sender)
        {
            IPEndPoint listen = new IPEndPoint(sender.RemoteIPAddress, multiListener.ListeningPortNo);
            if (listen.ToString() == sender.LocalEndPoint.ToString())
            {
                is_server = true;

                connectedRemoteClients++;

                bool preAutoSend = AutoSend;
                AutoSend = false;
                WriteMemory("connectedRemoteClients");
                AutoSend = preAutoSend;

                if (EventHandler_ListenerLoopbackConnected != null) EventHandler_ListenerLoopbackConnected(this, listen);
            }
            else
            {
                if (is_server)
                {
                    connectedRemoteClients++;

                    bool preAutoSend = AutoSend;
                    AutoSend = true;
                    WriteMemory("connectedRemoteClients");
                    AutoSend = preAutoSend;
                }

                // 接続してきたクライアント側のリスナーへ双方向接続する
                if (GetClientName(listen) == "")
                {
                    Add_Client("auto:" + listen.ToString() + ":add_client", listen.Address.ToString(), listen.Port);
                }

                if (EventHandler_ListenerConnected != null) EventHandler_ListenerConnected(this, listen);
            }
        }

        /// <summary>
        /// リスナー受信イベント処理
        /// データ変更の受信処理
        /// データ要求の受信でのデータ送信処理
        /// リモートロック処理
        /// リモートアンロック処理
        /// </summary>
        /// <param name="sender">受信TCPオブジェクト</param>
        protected virtual void listener_EventReceived(BL_TcpP2P sender)
        {
            BL_TcpP2PMultiServer.BL_TcpP2PMultiClient server = (BL_TcpP2PMultiServer.BL_TcpP2PMultiClient)sender;
            byte[] data = server.Receive();

            if (data != null)
            {
                TcpTransportHeader recv_header = new TcpTransportHeader();
                recv_header.Initialize();

                if (recv_header.Length <= data.Length)
                {
                    byte[] bodydata = recv_header.get_bodydata(data);
                    string field_name = recv_header.TargetField.Trim();

                    if (0 < recv_header.Send)
                    {
                        if (SetBytes(bodydata, field_name))
                        {
                            base.WriteMemory(field_name, !server.RemoteLock);
                            id = recv_header.Id;

                            try
                            {
                                IPEndPoint listen = new IPEndPoint(sender.RemoteIPAddress, multiListener.ListeningPortNo);

                                if (listen.ToString() == server.LocalEndPoint.ToString())
                                {
                                    if (recv_header.TargetField != "connectedRemoteClients")
                                    {
                                        if (EventHandler_LoopbackUpdated != null) EventHandler_LoopbackUpdated(this, listen, recv_header.TargetField);
                                    }
                                }
                                else
                                {
                                    if (EventHandler_ListenerUpdated != null) EventHandler_ListenerUpdated(this, listen, recv_header.TargetField);

                                    if (is_server)
                                    {
                                        lock (dictClient)
                                        {
                                            foreach (var kv in dictClient)
                                            {
                                                if (listen.ToString() != kv.Value.RemoteEndPoint.ToString())
                                                {
                                                    SendMemory(recv_header.TargetField, false, true, true, kv.Value.RemoteEndPoint);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                recv_header.TargetField = "ErrorMessage";
                                ErrorMessage = ex.Message;

                                listener_EventError(sender, new Exception(ErrorMessage));
                            }

                            byte[] send_bodydata = GetBytes(recv_header.TargetField);
                            byte[] senddata = recv_header.make_senddata(send_bodydata);
                            server.Send(senddata);
                        }
                        else
                        {
                            throw new Exception("リスナー受信データ内容不正");
                        }
                    }
                    else if (0 < recv_header.Receive)
                    {
                        base.ReadMemory(recv_header.TargetField, !server.RemoteLock);

                        byte[] send_bodydata = GetBytes(recv_header.TargetField);
                        byte[] senddata = recv_header.make_senddata(send_bodydata);
                        server.Send(senddata);
                    }
                    else if (0 < recv_header.Lock)
                    {
                        lock (this)
                        {
                            if (!IsRemoteLock)
                            {
                                remoteLock = true;
                                Lock();
                            }

                            server.RemoteLock = true;
                        }

                        byte[] send_bodydata = new byte[0];
                        byte[] senddata = recv_header.make_senddata(send_bodydata);
                        server.Send(senddata);
                    }
                    else if (0 < recv_header.Unlock)
                    {
                        lock (this)
                        {
                            server.RemoteLock = false;

                            if (!IsRemoteLock)
                            {
                                remoteLock = false;
                                Unlock();
                            }
                        }

                        byte[] send_bodydata = new byte[0];
                        byte[] senddata = recv_header.make_senddata(send_bodydata);
                        server.Send(senddata);
                    }
                }
                else
                {
                    throw new Exception("リスナー受信データ長不正");
                }
            }
        }

        /// <summary>
        /// リスナー切断イベント処理
        /// リモートロック中のアンロック処理
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void listener_EventClosed(BL_TcpP2P sender)
        {
            BL_TcpP2PMultiServer.BL_TcpP2PMultiClient server = (BL_TcpP2PMultiServer.BL_TcpP2PMultiClient)sender;

            if (server.RemoteLock)
            {
                server.RemoteLock = false;

                if (!IsRemoteLock)
                {
                    remoteLock = false;
                    Unlock();
                }
            }

            IPEndPoint listen = new IPEndPoint(sender.RemoteIPAddress, multiListener.ListeningPortNo);

            // 切断されたクライアント側のリスナーへ双方向接続を除去する
            string name = GetClientName(listen);
            if (name != "")
            {
                string[] names = name.Split(':');
                if (4 <= names.Length)
                {
                    if (names[0] == "auto" && names[names.Length - 1] == "add_client")
                    {
                        Remove_Client(name);
                    }
                }
            }

            if (is_server)
            {
                connectedRemoteClients--;

                bool preAutoSend = AutoSend;
                AutoSend = true;
                WriteMemory("connectedRemoteClients");
                AutoSend = preAutoSend;
            }

            if (EventHandler_ListenerClosed != null) EventHandler_ListenerClosed(this, listen);
        }

        /// <summary>
        /// リスナー通信エラーイベント処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="ex"></param>
        protected virtual void listener_EventError(BL_TcpP2P sender, Exception ex)
        {
            if (EventHandler_ListenerError != null)
            {
                if (typeof(ThreadAbortException).IsInstanceOfType(ex))
                {
                }
                else
                {
                    EventHandler_ListenerError(this, new TcpSocketSyncException(sender, ex));
                }
            }
        }

        #endregion

        #region クライアント処理

        /// <summary>
        /// クライアント初期化
        /// 指定されたIPアドレス・ポートNoでサーバーへ接続を行います
        /// 接続が確立しなくても本メソッドはブロッキングしません
        /// 同一接続先が既存の場合は、何もしません
        /// </summary>
        /// <param name="name">クライアント識別名称</param>
        /// <param name="ipaddress">接続先IPアドレス</param>
        /// <param name="portno">接続先ポートNo</param>
        /// <returns>接続が確立された場合trueを返します</returns>
        public virtual bool Add_Client(string name, string ipaddress, int portno)
        {
            if (!dictClient.ContainsKey(name))
            {
                //    dictClient[name].EndLink();
                //    dictClient[name].Dispose();
                //    dictClient[name] = null;
                //}

                BL_TcpP2PClient client = new BL_TcpP2PClient(IPAddress.Any, 0, IPAddress.Parse(ipaddress), portno, Length + 8, Length + 8);
                client.EventConnected += new BL_TcpP2P.Event_Handler_Connected(client_EventConnected);
                client.EventReceived += new BL_TcpP2P.Event_Handler_Received(client_EventReceived);
                client.EventClosed += new BL_TcpP2P.Event_Handler_Closed(client_EventClosed);
                client.EventError += new BL_TcpP2P.Event_Handler_Error(client_EventError);

                client.StartLink();
                dictClient[name] = client;

                //BL_StopWatch sw = new BL_StopWatch();
                //sw.Start();
                //while(true)
                //{
                //    if(client.Connected) break;
                //    if(this.millisecondsTimeout <= sw.ElapsedMilliseconds) break;
                //}
            }

            return dictClient[name].Connected;
        }

        /// <summary>
        /// 指定された識別名称のクライアントを切断し、破棄します
        /// </summary>
        /// <param name="name">クライアント識別名称</param>
        public virtual void Remove_Client(string name)
        {
            lock (dictClient)
            {
                if (dictClient.ContainsKey(name))
                {
                    dictClient[name].EndLink();
                    dictClient[name].Dispose();
                    dictClient[name] = null;
                    dictClient.Remove(name);
                }
            }
        }

        /// <summary>
        /// 未接続クライアントの接続を行います
        /// 指定時間でタイムアウトします。
        /// ０を指定するとすべてのクライアント接続が確立するまでブロッキングされます
        /// </summary>
        /// <param name="millisecondsTimeout">タイムアウト時間 msec</param>
        /// <returns>接続が確立されたクライアント数を返します</returns>
        public virtual int Connect(int millisecondsTimeout)
        {
            ErrorMessage = "";
            lock (dictClient)
            {
                foreach (var client in dictClient.Values)
                {
                    client.StartLink();
                }
            }

            Stopwatch sw = new Stopwatch();
            sw.Start();

            int clients = 0;
            while (true)
            {
                clients = 0;
                bool allconnected = true;
                lock (dictClient)
                {
                    foreach (var client in dictClient)
                    {
                        if (!client.Value.Connected)
                        {
                            allconnected = false;
                        }
                        else
                        {
                            clients++;
                        }
                    }
                }

                if (allconnected || (0 <= millisecondsTimeout && millisecondsTimeout < sw.ElapsedMilliseconds))
                {
                    ErrorMessage = "接続タイムアウト:[" + millisecondsTimeout.ToString() + "ms]";
                    break;
                }

                Thread.Sleep(20);
            }

            sw.Stop();

            return clients;
        }

        /// <summary>
        /// すべてのクライアントを切断します
        /// </summary>
        public virtual void Disconnect()
        {
            lock (dictClient)
            {
                foreach (BL_TcpP2PClient client in dictClient.Values)
                {
                    client.EndLink();
                }
            }
        }

        /// <summary>
        /// クライアント接続確立イベント処理
        /// </summary>
        /// <param name="sender"></param>
        protected virtual void client_EventConnected(BL_TcpP2P sender)
        {
            lock (pendingHeaders)
            {
                pendingHeaders.Clear();
            }

            if (EventHandler_ClientConnected != null) EventHandler_ClientConnected(this, sender.RemoteEndPoint);
        }

        /// <summary>
        /// クライアント受信イベント処理
        /// </summary>
        /// <param name="sender">受信TCPオブジェクト</param>
        protected virtual void client_EventReceived(BL_TcpP2P sender)
        {
            byte[] data = sender.Receive();

            if (data != null)
            {
                TcpTransportHeader recv_header = new TcpTransportHeader();
                recv_header.Initialize();

                if (recv_header.Length <= data.Length)
                {
                    byte[] bodydata = recv_header.get_bodydata(data);
                    string field_name = recv_header.TargetField.Trim();

                    TcpTransportHeader send_header = null;
                    lock (pendingHeaders)
                    {
                        for (int i = 0; i < pendingHeaders.Count; i++)
                        {
                            if (pendingHeaders[i].Id == recv_header.Id)
                            {
                                send_header = pendingHeaders[i];
                                break;
                            }
                        }
                    }

                    if (send_header != null)
                    {
                        if (0 < send_header.Send)
                        {
                            if (recv_header.TargetField == "ErrorMessage")
                            {
                                if (SetBytes(bodydata, recv_header.TargetField))
                                {
                                    base.WriteMemory(recv_header.TargetField, !remoteLock);

                                    client_EventError(sender, new Exception(ErrorMessage));
                                }
                                else
                                {
                                    throw new Exception("クライアント受信データ内容不正");
                                }
                            }
                            else if (send_header.TargetField != recv_header.TargetField || recv_header.TargetField == "")
                            {
                                if (SetBytes(bodydata, recv_header.TargetField))
                                {
                                    base.WriteMemory(recv_header.TargetField, !remoteLock);

                                    try
                                    {
                                        if (EventHandler_ClientUpdated != null) EventHandler_ClientUpdated(this, sender.RemoteEndPoint, recv_header.TargetField);
                                    }
                                    catch (Exception ex)
                                    {
                                        ErrorMessage = ex.Message;
                                        client_EventError(sender, new Exception(ErrorMessage));
                                    }
                                }
                                else
                                {
                                    throw new Exception("クライアント受信データ内容不正");
                                }
                            }
                        }
                        else if (0 < send_header.Receive)
                        {
                            if (SetBytes(bodydata, recv_header.TargetField))
                            {
                                base.WriteMemory(recv_header.TargetField, !remoteLock);

                                if (EventHandler_ClientUpdated != null) EventHandler_ClientUpdated(this, sender.RemoteEndPoint, recv_header.TargetField);
                            }
                            else
                            {
                                throw new Exception("クライアント受信データ内容不正");
                            }
                        }
                        else if (0 < send_header.Lock)
                        {

                        }
                        else if (0 < send_header.Unlock)
                        {

                        }

                        lock (pendingHeaders)
                        {
                            for (int i = 0; i < pendingHeaders.Count; i++)
                            {
                                if (pendingHeaders[i].Id == send_header.Id)
                                {
                                    pendingHeaders.RemoveAt(i);
                                    i--;
                                }
                            }
                            //pendingHeaders.Remove(send_header);
                        }
                    }
                    else
                    {
                        throw new Exception("クライアント未送信データレスポンス受信");
                    }
                }
                else
                {
                    throw new Exception("クライアント受信データ長不正");
                }
            }
        }

        /// <summary>
        /// クライアント切断イベント処理
        /// クライアントロックはアンロックされます
        /// </summary>
        /// <param name="sender">切断TCPオブジェクト</param>
        protected virtual void client_EventClosed(BL_TcpP2P sender)
        {
            if (ConnectedLocalClients == 0)
            {
                Unlock();
                clientLock = false;
            }

            lock (pendingHeaders)
            {
                pendingHeaders.Clear();
            }

            if (EventHandler_ClientClosed != null) EventHandler_ClientClosed(this, sender.RemoteEndPoint);
        }

        /// <summary>
        /// クライアント通信エラーイベント処理
        /// </summary>
        /// <param name="sender">通信エラーTCPオブジェクト</param>
        /// <param name="ex">TCP例外</param>
        protected virtual void client_EventError(BL_TcpP2P sender, Exception ex)
        {
            if (typeof(SocketException).IsInstanceOfType(ex))
            {
                lock (pendingHeaders)
                {
                    pendingHeaders.Clear();
                }

                SocketException sex = (SocketException)ex;
                if (sex.ErrorCode == 10061) return;
                if (sex.ErrorCode == 10060) return;
            }

            if (typeof(ThreadAbortException).IsInstanceOfType(ex)) return;

            if (EventHandler_ClientError != null) EventHandler_ClientError(this, new TcpSocketSyncException(sender, ex));
        }

        /// <summary>
        /// IPアドレスからクライアント識別名称を取得します
        /// </summary>
        /// <param name="remote_endpoint">クライアントのエンドポイント</param>
        /// <returns></returns>
        public string GetClientName(IPEndPoint remote_endpoint)
        {
            lock (dictClient)
            {
                foreach (var kv in dictClient)
                {
                    if (kv.Value.RemoteEndPoint.ToString() == remote_endpoint.ToString())
                    {
                        return kv.Key;
                    }
                }
            }

            return "";
        }

        #endregion

        #region データ送信

        /// <summary>
        /// 自オブジェクト全体をサーバーへ送信します
        /// 一連の送受信プロトコルが完了するまでブロッキングされません
        /// 受信待機中にフォームのメッセージポンプを処理します
        /// データ送信前にクライアントの接続確立を再試行します
        /// </summary>
        /// <returns>送信成功接続数を返します</returns>
        public virtual int SendMemory()
        {
            return SendMemory("");
        }

        /// <summary>
        /// 自オブジェクトの指定フィールドをサーバーへ送信します
        /// 一連の送受信プロトコルが完了するまでブロッキングするかどうかを指定できます
        /// 受信待機中にフォームのメッセージポンプを処理します
        /// データ送信前にクライアントの接続確立を再試行します
        /// </summary>
        /// <param name="async">ブロッキングする場合falseを指定してください</param>
        /// <returns>送信成功接続数を返します</returns>
        public virtual int SendMemory(bool async)
        {
            return SendMemory("", async, false);
        }

        /// <summary>
        /// 自オブジェクトの指定フィールドをサーバーへ送信します
        /// 一連の送受信プロトコルが完了するまでブロッキングするかどうかを指定できます
        /// 受信待機中にフォームのメッセージポンプを処理するかどうかを指定できます
        /// データ送信前にクライアントの接続確立を再試行します
        /// 
        /// サーバーのデータ変更イベントによって画面更新を行う処理を行っている場合など
        /// メッセージポンプを処理しないとデッドロックしてしまう場合にdoevent=trueを指定してください
        /// 
        /// </summary>
        /// <param name="async">ブロッキングする場合falseを指定してください</param>
        /// <param name="doevent">メッセージポンプを処理しない場合falseを指定してください</param>
        /// <returns>送信成功接続数を返します</returns>
        public virtual int SendMemory(bool async, bool doevent)
        {
            return SendMemory("", async, doevent);
        }

        /// <summary>
        /// 自オブジェクトの指定フィールドをサーバーへ送信します
        /// 一連の送受信プロトコルが完了するまでブロッキングされません
        /// 受信待機中にフォームのメッセージポンプを処理します
        /// データ送信前にクライアントの接続確立を再試行します
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <returns>送信成功接続数を返します</returns>
        public virtual int SendMemory(string field_name)
        {
            return SendMemory(field_name, false);
        }

        /// <summary>
        /// 自オブジェクトの指定フィールドをサーバーへ送信します
        /// 一連の送受信プロトコルが完了するまでブロッキングするかどうかを指定できます
        /// 受信待機中にフォームのメッセージポンプを処理します
        /// データ送信前にクライアントの接続確立を再試行します
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="async">ブロッキングする場合falseを指定してください</param>
        /// <returns>送信成功接続数を返します</returns>
        public virtual int SendMemory(string field_name, bool async)
        {
            return SendMemory(field_name, async, false);
        }

        /// <summary>
        /// 自オブジェクトの指定フィールドをサーバーへ送信します
        /// 一連の送受信プロトコルが完了するまでブロッキングするかどうかを指定できます
        /// 受信待機中にフォームのメッセージポンプを処理するかどうかを指定できます
        /// データ送信前にクライアントの接続確立を再試行します
        /// 
        /// サーバーのデータ変更イベントによって画面更新を行う処理を行っている場合など
        /// メッセージポンプを処理しないとデッドロックしてしまう場合にdoevent=trueを指定してください
        /// 
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="async">ブロッキングする場合falseを指定してください</param>
        /// <param name="doevent">メッセージポンプを処理しない場合falseを指定してください</param>
        /// <returns>送信成功接続数を返します</returns>
        public virtual int SendMemory(string field_name, bool async, bool doevent)
        {
            return SendMemory(field_name, async, doevent, true, null);
        }

        /// <summary>
        /// 自オブジェクトの指定フィールドをサーバーへ送信します
        /// 一連の送受信プロトコルが完了するまでブロッキングするかどうかを指定できます
        /// 受信待機中にフォームのメッセージポンプを処理するかどうかを指定できます
        /// データ送信前にクライアントの接続確立を再試行するかどうかを指定できます
        /// 
        /// サーバーのデータ変更イベントによって画面更新を行う処理を行っている場合など
        /// メッセージポンプを処理しないとデッドロックしてしまう場合にdoevent=trueを指定してください
        /// 
        /// </summary>
        /// <param name="field_name">指定フィールド</param>
        /// <param name="async">ブロッキングする場合falseを指定してください</param>
        /// <param name="doevent">メッセージポンプを処理しない場合falseを指定してください</param>
        /// <param name="tryconnect">未接続のクライアントの接続を再試行する場合trueを指定してください</param>
        /// <param name="target_ipendpoint">送信先を限定する場合、対象IPEndPoint</param>
        /// <returns>送信成功接続数を返します</returns>
        public virtual int SendMemory(string field_name, bool async, bool doevent, bool tryconnect, IPEndPoint target_ipendpoint)
        {
            int clients = 0;
            ErrorMessage = "";

            lock (dictClient)
            {
                foreach (BL_TcpP2PClient client in dictClient.Values)
                {
                    if (target_ipendpoint != null)
                    {
                        if (target_ipendpoint.ToString() != client.RemoteEndPoint.ToString()) continue;
                    }

                    for (int retry = 3; 0 < retry; retry--)
                    {
                        if (tryconnect)
                        {
                            if (!client.Connected)
                            {
                                client.StartLink();
                                Thread.Sleep(100);
                                continue;
                            }
                        }
                        else if (!client.Connected)
                        {
                            break;
                        }

                        lock (pendingHeaders)
                        {
                            id = (id + 1) % 65536;
                            TcpTransportHeader header = new TcpTransportHeader();
                            header.Initialize();
                            header.Id = id;
                            header.TargetField = field_name;
                            header.Send = 1;

                            byte[] bodydata = GetBytes(field_name);
                            byte[] data = header.make_senddata(bodydata);
                            header.prevBodyData = bodydata;

                            pendingHeaders.Add(header);

                            if (client.Send(data))
                            {
                                clients++;
                            }
                            else
                            {
                                pendingHeaders.Remove(header);

                                client.StartLink();
                                Thread.Sleep(100);
                                continue;
                            }
                        }

                        break;
                    }
                }
            }

            if (!async)
            {
                int timeout_clients = WaitSyncronized(millisecondsTimeout, doevent);
                if (0 < timeout_clients)
                {
                    ErrorMessage = "送信タイムアウト:[" + millisecondsTimeout.ToString() + "ms]";
                    clients -= timeout_clients;
                }
            }

            lastSendClients = clients;

            return clients;
        }

        #endregion

        #region データ受信

        /// <summary>
        /// サーバー側のデータを取得して自オブジェクトを更新します
        /// 取得先のサーバーは全サーバーです
        /// 取得対象は自オブジェクト全体です
        /// 一連の通信プロトコルが完了するまでブロッキングしません
        /// 受信待機中にフォームのメッセージポンプを処理します
        /// 
        /// 取得先のサーバーに空文字を指定した場合、すべてのサーバーからデータを順次取得します
        /// 
        /// </summary>
        /// <returns>取得したサーバー数</returns>
        public virtual int ReceiveMemory()
        {
            return ReceiveMemory("");
        }

        /// <summary>
        /// サーバー側のデータを取得して自オブジェクトを更新します
        /// 取得先のサーバーを指定できます
        /// 取得対象は自オブジェクト全体です
        /// 一連の通信プロトコルが完了するまでブロッキングしません
        /// 受信待機中にフォームのメッセージポンプを処理します
        /// 
        /// 取得先のサーバーに空文字を指定した場合、すべてのサーバーからデータを順次取得します
        /// 
        /// </summary>
        /// <param name="from">取得先クライアント識別名称</param>
        /// <returns>取得したサーバー数</returns>
        public virtual int ReceiveMemory(string from)
        {
            return ReceiveMemory(from, "");
        }

        /// <summary>
        /// サーバー側のデータを取得して自オブジェクトを更新します
        /// 取得先のサーバーを指定できます
        /// 取得対象のフィールドを指定できます
        /// 一連の通信プロトコルが完了するまでブロッキングしません
        /// 受信待機中にフォームのメッセージポンプを処理します
        /// 
        /// 取得先のサーバーに空文字を指定した場合、すべてのサーバーからデータを順次取得します
        /// 取得対象のフィールド名に空文字を指定した場合、自オブジェクトの全データを取得します
        /// 
        /// </summary>
        /// <param name="from">取得先クライアント識別名称</param>
        /// <param name="field_name">取得対象のフィールド名</param>
        /// <returns>取得したサーバー数</returns>
        public virtual int ReceiveMemory(string from, string field_name)
        {
            return ReceiveMemory(from, field_name, false);
        }

        /// <summary>
        /// サーバー側のデータを取得して自オブジェクトを更新します
        /// 取得先のサーバーを指定できます
        /// 取得対象のフィールドを指定できます
        /// 一連の通信プロトコルが完了するまでブロッキングするかどうかを指定できます
        /// 受信待機中にフォームのメッセージポンプを処理します
        /// 
        /// 取得先のサーバーに空文字を指定した場合、すべてのサーバーからデータを順次取得します
        /// 取得対象のフィールド名に空文字を指定した場合、自オブジェクトの全データを取得します
        /// 
        /// </summary>
        /// <param name="from">取得先クライアント識別名称</param>
        /// <param name="field_name">取得対象のフィールド名</param>
        /// <param name="async">ブロッキングする場合falseを指定してください</param>
        /// <returns>取得したサーバー数</returns>
        public virtual int ReceiveMemory(string from, string field_name, bool async)
        {
            return ReceiveMemory(from, field_name, async, false);
        }

        /// <summary>
        /// サーバー側のデータを取得して自オブジェクトを更新します
        /// 取得先のサーバーを指定できます
        /// 取得対象のフィールドを指定できます
        /// 一連の通信プロトコルが完了するまでブロッキングするかどうかを指定できます
        /// 受信待機中にフォームのメッセージポンプを処理します
        /// 
        /// 取得先のサーバーに空文字を指定した場合、すべてのサーバーからデータを順次取得します
        /// 取得対象のフィールド名に空文字を指定した場合、自オブジェクトの全データを取得します
        /// 
        /// </summary>
        /// <param name="async">ブロッキングする場合falseを指定してください</param>
        /// <returns>取得したサーバー数</returns>
        public virtual int ReceiveMemory(bool async)
        {
            return ReceiveMemory("", "", async, false);
        }

        /// <summary>
        /// サーバー側のデータを取得して自オブジェクトを更新します
        /// 取得先のサーバーを指定できます
        /// 取得対象のフィールドを指定できます
        /// 一連の通信プロトコルが完了するまでブロッキングするかどうかを指定できます
        /// 受信待機中にフォームのメッセージポンプを処理するかどうかを指定できます
        /// 
        /// 取得先のサーバーに空文字を指定した場合、すべてのサーバーからデータを順次取得します
        /// 取得対象のフィールド名に空文字を指定した場合、自オブジェクトの全データを取得します
        /// 
        /// </summary>
        /// <param name="async">ブロッキングする場合falseを指定してください</param>
        /// <param name="doevent">メッセージポンプを処理しない場合falseを指定してください</param>
        /// <returns>取得したサーバー数</returns>
        public virtual int ReceiveMemory(bool async, bool doevent)
        {
            return ReceiveMemory("", "", async, doevent, true);
        }

        /// <summary>
        /// サーバー側のデータを取得して自オブジェクトを更新します
        /// 取得先のサーバーを指定できます
        /// 取得対象のフィールドを指定できます
        /// 一連の通信プロトコルが完了するまでブロッキングするかどうかを指定できます
        /// 受信待機中にフォームのメッセージポンプを処理するかどうかを指定できます
        /// 
        /// 取得先のサーバーに空文字を指定した場合、すべてのサーバーからデータを順次取得します
        /// 取得対象のフィールド名に空文字を指定した場合、自オブジェクトの全データを取得します
        /// 
        /// </summary>
        /// <param name="async">ブロッキングする場合falseを指定してください</param>
        /// <param name="doevent">メッセージポンプを処理しない場合falseを指定してください</param>
        /// <param name="tryconnect">未接続のクライアントの再接続を行い場合trueを指定してください</param>
        /// <returns>取得したサーバー数</returns>
        public virtual int ReceiveMemory(bool async, bool doevent, bool tryconnect)
        {
            return ReceiveMemory("", "", async, doevent, tryconnect);
        }


        /// <summary>
        /// サーバー側のデータを取得して自オブジェクトを更新します
        /// 取得先のサーバーを指定できます
        /// 取得対象のフィールドを指定できます
        /// 一連の通信プロトコルが完了するまでブロッキングするかどうかを指定できます
        /// 受信待機中にフォームのメッセージポンプを処理するかどうかを指定できます
        /// 
        /// 取得先のサーバーに空文字を指定した場合、すべてのサーバーからデータを順次取得します
        /// 取得対象のフィールド名に空文字を指定した場合、自オブジェクトの全データを取得します
        /// 
        /// </summary>
        /// <param name="from">取得先クライアント識別名称</param>
        /// <param name="field_name">取得対象のフィールド名</param>
        /// <param name="async">ブロッキングする場合falseを指定してください</param>
        /// <param name="doevent">メッセージポンプを処理しない場合falseを指定してください</param>
        /// <returns>取得したサーバー数</returns>
        public virtual int ReceiveMemory(string from, string field_name, bool async, bool doevent)
        {
            return ReceiveMemory(from, field_name, async, doevent, true);
        }

        /// <summary>
        /// サーバー側のデータを取得して自オブジェクトを更新します
        /// 取得先のサーバーを指定できます
        /// 取得対象のフィールドを指定できます
        /// 一連の通信プロトコルが完了するまでブロッキングするかどうかを指定できます
        /// 受信待機中にフォームのメッセージポンプを処理するかどうかを指定できます
        /// 
        /// 取得先のサーバーに空文字を指定した場合、すべてのサーバーからデータを順次取得します
        /// 取得対象のフィールド名に空文字を指定した場合、自オブジェクトの全データを取得します
        /// 
        /// </summary>
        /// <param name="from">取得先クライアント識別名称</param>
        /// <param name="field_name">取得対象のフィールド名</param>
        /// <param name="async">ブロッキングする場合falseを指定してください</param>
        /// <param name="doevent">メッセージポンプを処理しない場合falseを指定してください</param>
        /// <param name="tryconnect">未接続のクライアントの再接続を行い場合trueを指定してください</param>
        /// <returns>取得したサーバー数</returns>
        public virtual int ReceiveMemory(string from, string field_name, bool async, bool doevent, bool tryconnect)
        {
            int clients = 0;
            ErrorMessage = "";

            lock (dictClient)
            {
                foreach (var kv in dictClient)
                {
                    //for (int i = 0; i < dictClient.Count; i++)
                    //{
                    //    var kv = dictClient.ElementAt(i);

                    string key = kv.Key;
                    if (from != key && (from != "")) continue;
                    BL_TcpP2PClient client = kv.Value;

                    for (int retry = 3; 0 < retry; retry--)
                    {
                        if (tryconnect)
                        {
                            if (!client.Connected)
                            {
                                client.StartLink();
                                Thread.Sleep(100);
                                continue;
                            }
                        }
                        else if (!client.Connected)
                        {
                            break;
                        }

                        id = (id + 1) % 65536;
                        TcpTransportHeader header = new TcpTransportHeader();
                        header.Initialize();
                        header.Id = id;
                        header.TargetField = field_name;
                        header.Receive = 1;

                        byte[] bodydata = new byte[0];
                        byte[] data = header.make_senddata(bodydata);
                        header.prevBodyData = bodydata;

                        pendingHeaders.Add(header);

                        if (client.Send(data))
                        {
                            clients++;
                        }
                        else
                        {
                            pendingHeaders.Remove(header);

                            client.StartLink();
                            Thread.Sleep(100);
                            continue;
                        }

                        break;
                    }
                }
            }

            if (!async)
            {
                int timeout_clients = WaitSyncronized(millisecondsTimeout, doevent);
                if (0 < timeout_clients)
                {
                    ErrorMessage = "送信タイムアウト:[" + millisecondsTimeout.ToString() + "ms]";
                    clients -= timeout_clients;
                }
            }

            lastReceiveClients = clients;

            return clients;
        }

        #endregion

        #region 同期完了待ち

        /// <summary>
        /// 直前の通信プロトコル完了を待機します
        /// 自オブジェクト既定のタイムアウト時間でタイムアウトします
        /// 受信待機中にフォームのメッセージポンプを処理しません
        /// </summary>
        /// <returns>未完了のプロトコル数</returns>
        public int WaitSyncronized()
        {
            return WaitSyncronized(millisecondsTimeout, false);
        }

        /// <summary>
        /// 直前の通信プロトコル完了を待機します
        /// 指定時間でタイムアウトします
        /// 受信待機中にフォームのメッセージポンプを処理しません
        /// </summary>
        /// <param name="millisecondsTimeout">タイムアウト指定時間</param>
        /// <returns>未完了のプロトコル数</returns>
        public virtual int WaitSyncronized(int millisecondsTimeout)
        {
            return WaitSyncronized(millisecondsTimeout, false);
        }

        /// <summary>
        /// 直前の通信プロトコル完了を待機します
        /// 指定時間でタイムアウトします
        /// 受信待機中にフォームのメッセージポンプを処理するかどうかを指定できます
        /// </summary>
        /// <param name="millisecondsTimeout">タイムアウト指定時間</param>
        /// <param name="doevent">メッセージポンプを処理しない場合falseを指定してください</param>
        /// <returns>未完了のプロトコル数</returns>
        public virtual int WaitSyncronized(int millisecondsTimeout, bool doevent)
        {
            ErrorMessage = "";
            Stopwatch sw = new Stopwatch();
            sw.Start();

            Thread.Sleep(5);

            while (true)
            {
                lock (pendingHeaders)
                {
                    if (0 == pendingHeaders.Count) break;

                    if (0 <= millisecondsTimeout && millisecondsTimeout <= sw.ElapsedMilliseconds)
                    {
                        ErrorMessage = "応答タイムアウト:[" + millisecondsTimeout.ToString() + "ms]";

                        int ret = pendingHeaders.Count;
                        pendingHeaders.Clear();
                        return ret;
                    }
                }

                if (doevent) Application.DoEvents();
                Thread.Sleep(2);
            }

            return pendingHeaders.Count;
        }

        #endregion

        #region リモートロック

        /// <summary>
        /// サーバーをリモートロックします
        /// </summary>
        /// <returns></returns>
        public virtual int RemoteLock()
        {
            if (!remoteLock)
            {
                Lock();
                clientLock = true;
            }

            int clients = 0;

            lock (dictClient)
            {
                foreach (var client in dictClient.Values)
                {
                    //for (int i = 0; i < dictClient.Values.Count; i++)
                    //{
                    //    BL_TcpP2PClient client = dictClient.Values.ElementAt(i);

                    if (!client.Connected) continue;

                    id = (id + 1) % 65536;
                    TcpTransportHeader header = new TcpTransportHeader();
                    header.Initialize();
                    header.Id = id;
                    header.Lock = 1;

                    byte[] bodydata = new byte[0];
                    byte[] data = header.make_senddata(bodydata);
                    pendingHeaders.Add(header);

                    if (client.Send(data))
                    {
                        clients++;
                    }
                    else
                    {
                        pendingHeaders.Remove(header);
                    }
                }
            }

            int timeout_clients = WaitSyncronized();
            if (0 < timeout_clients)
            {
                clients -= timeout_clients;
            }

            if (clients == 0) Unlock();

            return clients;
        }

        /// <summary>
        /// サーバーをリモートアンロックします
        /// </summary>
        /// <returns></returns>
        public virtual int RemoteUnlock()
        {
            int clients = 0;

            lock (dictClient)
            {
                foreach (var client in dictClient.Values)
                {
                    //for (int i = 0; i < dictClient.Values.Count; i++)
                    //{
                    //    BL_TcpP2PClient client = dictClient.Values.ElementAt(i);

                    if (!client.Connected) continue;

                    id = (id + 1) % 65536;
                    TcpTransportHeader header = new TcpTransportHeader();
                    header.Initialize();
                    header.Id = id;
                    header.Unlock = 1;

                    byte[] bodydata = new byte[0];
                    byte[] data = header.make_senddata(bodydata);
                    pendingHeaders.Add(header);

                    if (client.Send(data))
                    {
                        clients++;
                    }
                    else
                    {
                        pendingHeaders.Remove(header);
                    }
                }
            }

            int timeout_clients = WaitSyncronized();
            if (0 < timeout_clients)
            {
                clients -= timeout_clients;
            }

            if (!remoteLock)
            {
                Unlock();
                clientLock = false;
            }

            return clients;
        }

        #endregion
    }
}
