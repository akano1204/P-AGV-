using System;
using System.Collections.Generic;
using System.Text;

using System.Net;
using System.IO;
using System.Threading;
using System.Net.Sockets;
using System.Text.RegularExpressions;

using System.Collections.Specialized;

namespace BelicsClass.Network
{
    // ＦＴＰクライアントクラス
    /// <summary>
    /// Windows IIS を 使用したFTPクライアント機能（機能限定版）を提供する。
    /// サーバのファイル一覧の取得、アップロード、ダウンロード、リネーム機能を提供する。
    /// 失敗時のリトライ機能等は、クラスユーザ側で処理を行うこと。
    /// </summary>
    public class BL_FtpClient
    {
        #region データクラス

        /// <summary>
        /// FTPサーバ情報
        /// 接続先の情報を格納します。
        /// 以下の情報が初期値として格納されます。
        /// HostName : "192.168.1.200"
        /// Port : 21
        /// User : "anyoymoause"
        /// Pass : "guest@guest.com"
        /// </summary>
        public class BL_HostInfo
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public BL_HostInfo()
            {

            }
            private string host = "192.168.1.200";

            /// <summary>ホスト名称</summary>
            public string HostName
            {
                get
                {
                    return host;
                }
                set
                {
                    host = value;
                }
            }

            private Int32 port = 21;

            /// <summary>ポート番号（初期値２１）</summary>
            public Int32 Port
            {
                get
                {
                    return port;
                }
                set
                {
                    port = value;
                }
            }

            private string user = "anyoymoause";

            /// <summary>ユーザ名称</summary>
            public string User
            {
                get
                {
                    return user;
                }
                set
                {
                    user = value;
                }
            }

            private string pass = "guest@guest.com";

            /// <summary>ユーザパスワード</summary>
            public string Pass
            {
                get
                {
                    return pass;
                }
                set
                {
                    pass = value;
                }
            }
        }

        // エラー情報構造体
        /// <summary>
        /// エラー情報 構造体
        /// 最後に行ったアクションのエラー情報を保持します。
        /// </summary>
        public struct BL_FtpErrorInfo
        {
            /// <summary>エラーコード</summary>
            public int Code;
            /// <summary>FTPサーバからの応答コード</summary>
            public int Answer;
            /// <summary>エラーメッセージ</summary>
            public string Message;
        }

        #endregion

        #region メンバフィールド
        // メンバ
        private TcpClient tcpClient_Ctrl = null;			// コントロールコネクション用 TCPクライアントクラス

        /// <summary></summary>
        protected string MsgFromServer = "";					// サーバからのメッセージ 保持フィールド
        private NetworkStream netStream_Ctrl = null;		// コントロールコネクション用 ストリームクラス
        /// <summary></summary>
        protected BL_HostInfo hostInfo = new BL_HostInfo();			// ホスト（サーバ）情報

        /// <summary></summary>
        protected bool isLogin = false;						// ログイン状態
        /// <summary></summary>
        protected string RemotePath = "\\";					// サーバのルートディレクトリ指定用

        /// <summary></summary>
        protected BL_FtpErrorInfo m_LastError;					// FTPエラー情報
        private int m_WaitTime = 800;						// サーバの応答待ち時間(ms)
        private int m_SendTransSize = 20480;					// 送信転送サイズ（サイズはパフォーマンスに影響する）
        private int m_RecvTransSize = 20480;					// 受信転送サイズ（サイズはパフォーマンスに影響する）

        // ステータス
        private long m_DownloadSize = 0;

        private long m_UploadSize = 0;

        // 同期用オブジェクト
        private Object SyncUL = new object();
        private Object SyncDL = new object();

        /// <summary>サーバのルートディレクトリにあるFileList</summary>
        protected StringCollection sc = new StringCollection();
        /// <summary>Fileリストに対応した各ファイルサイズ</summary>
        protected StringCollection m_ScFileSize = new StringCollection();
        /// <summary>Fileリストに対応した各ファイルの日付</summary>
        protected StringCollection m_ScFileDate = new StringCollection();
        #endregion

        #region プロパティ

        /// <summary>
        /// アップロードファイルの転送済みサイズを取得します。
        /// アップロードメソッドのCallで初期化され、アップロード中の転送サイズがリアルタイムに更新されます。
        /// </summary>
        public long UploadSize
        {
            get
            {
                lock (SyncUL)
                {
                    return m_UploadSize;
                }
            }
        }

        /// <summary>
        /// ダウンロードファイルの転送済みサイズを取得します。
        /// ダウンロードメソッドのCallで初期化され、ダウンロード中の転送サイズがリアルタイムに更新されます。
        /// </summary>
        public long DownloadSize
        {
            get
            {
                lock (SyncDL)
                {
                    return m_DownloadSize;
                }
            }
        }

        // 最後にアクションを行った際のログイン状態を取得します。
        /// <summary>
        /// FTPサーバへのログイン状態。
        /// 最後にアクションを行った際のログイン状態を取得します。
        /// 本プロパティではリアルタイムでの状態更新は行われません。
        /// </summary>
        public bool IsLogin
        {
            get
            {
                return isLogin;
            }
        }

        /// <summary>
        /// 送信ストリームの転送サイズ。初期値は 2048バイトです。
        /// 値が大きいほど転送のパフォーマンスが向上しますが、負荷が高くなります。
        /// </summary>
        public int SendTransSize
        {
            get
            {
                return m_SendTransSize;
            }
            set
            {
                m_SendTransSize = value;
            }
        }

        ///// <summary>
        ///// ユーザのカレントディレクトリを設定、取得します。
        ///// </summary>
        //public string LocalPath
        //{
        //    get
        //    {
        //        return m_LocalPath;
        //    }
        //    set
        //    {
        //        m_LocalPath = value;
        //    }
        //}

        /// <summary>
        /// 受信ストリームの転送サイズ。初期値は 2048バイトです。
        /// 値が大きいほど転送のパフォーマンスが向上しますが、負荷が高くなります。
        /// </summary>
        public int RecvTransSize
        {
            get
            {
                return m_RecvTransSize;
            }
            set
            {
                m_RecvTransSize = value;
            }
        }


        /// <summary>最後に実行した内容に対するエラー情報</summary>
        public BL_FtpErrorInfo LastError
        {
            get
            {
                return m_LastError;
            }
        }

        /// <summary>コマンド応答待ち時間。初期値は 800ms が設定されます。</summary>
        public int WaitTime
        {
            get
            {
                return m_WaitTime;
            }
            set
            {
                m_WaitTime = value;
            }
        }

        /// <summary>
        /// サーバのルートディレクトリにあるファイル数。
        /// 最新情報が必要な場合には RefreshFileList メソッドを実行してください。
        /// </summary>
        public int ServerFileCount
        {
            get
            {
                return sc.Count;
            }
        }

        /// <summary>
        /// サーバのルートディレクトリにあるファイルサイズのコンテナを取得する。
        /// 最新情報が必要な場合には RefreshFileList メソッドを実行してください。
        /// </summary>
        public StringCollection FileSizeItems
        {
            get
            {
                return m_ScFileSize;
            }
        }

        /// <summary>
        /// サーバのルートディレクトリにあるファイル日付のコンテナを取得する。
        /// 最新情報が必要な場合には RefreshFileList メソッドを実行してください。
        /// </summary>
        public StringCollection FileDateItems
        {
            get
            {
                return m_ScFileDate;
            }
        }

        /// <summary>
        /// サーバのルートディレクトリにあるファイル名称のコンテナを取得する。
        /// 最新情報が必要な場合には RefreshFileList メソッドを実行してください。
        /// </summary>
        public StringCollection FileNameItems
        {
            get
            {
                return sc;
            }
        }

        #endregion

        #region インデクサ
        /// <summary>
        /// ファイルリストからのファイル名称を取得する。
        /// 最新情報が必要な場合には RefreshFileList メソッドを実行してください。
        /// </summary>
        /// <param name="index">
        /// ファイルリスト内の０からのインデックス。
        /// ServerFileCount プロパティでファイル数を取得することが出来ます。
        /// </param>
        /// <returns>ファイル名称</returns>
        public string this[int index]
        {
            get
            {
                if (index < sc.Count)
                {
                    return sc[index];
                }
                return "";
            }
        }

        #endregion

        //////////////////////////////////////////////
        // コンストラクタ
        //////////////////////////////////////////////
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="HostName">ホスト名称（ＩＰアドレスを指す文字列も可能です）</param>
        /// <param name="UserName">ログインするユーザ名称</param>
        /// <param name="Pass">ログインするユーザのパスワード</param>
        public BL_FtpClient(string HostName, string UserName, string Pass)
        {
            // 接続先情報を取得
            hostInfo.HostName = HostName;
            hostInfo.User = UserName;
            hostInfo.Pass = Pass;
            //            hostInfo.Port = 50500;
            hostInfo.Port = 21;

            SetLastError(0, 0, "");
        }

        //////////////////////////////////////////////
        // ホストへのコマンド送信メソッド
        //////////////////////////////////////////////
        /// <summary>
        /// 対象サーバに任意のコマンドを送信する。
        /// </summary>
        /// <param name="command">送信コマンド</param>
        /// <returns>成功/失敗：true/false</returns>
        public bool SendToServer(string command)
        {
            string msg = command + "\r\n";
            byte[] bb = new byte[256];

            // アスキー文字列へ変換
            bb = Encoding.Default.GetBytes(msg);

            try
            {
                netStream_Ctrl.Write(bb, 0, bb.Length);
                netStream_Ctrl.Flush();
            }
            catch
            {
                Console.WriteLine("SendToServer() :コマンド送信に失敗、コントロールコネクションが確立されていない可能性があります");
                return false;
            }
            return true;
        }

        //////////////////////////////////////////////
        // ログインメソッド
        //////////////////////////////////////////////
        /// <summary>
        /// 対象サーバにログインする。
        /// 本メソッドは、コントロールコネクション確立しユーザの認証後に、指定パスをカレントディレクトリとする。
        /// 処理が完了するまでブロッキングされます。
        /// </summary>
        /// <returns>成功/失敗：true/false</returns>
        public virtual bool Login(string remotePath)
        {
            RemotePath = remotePath;

            int check = 0;
            SetLastError(0, 0, "");

            // 接続を試みる
            try
            {
                tcpClient_Ctrl = new TcpClient(hostInfo.HostName, hostInfo.Port);
            }
            catch (Exception ex)
            {
                SetLastError(0x0001, 0, "Login() :" + ex.Message);
                return false;					// 接続失敗
            }

            // 送受信するネットワークストリームを取得する			
            netStream_Ctrl = tcpClient_Ctrl.GetStream();
            check = GetMessage(m_WaitTime);

            // レスポンスコードをチェックする
            if (220 != check)
            {
                SetLastError(0x0011, check, "Login() :" + MsgFromServer);

                // 切断処理
                DisConnect();

                return false;
            }

            // 認証する
            Console.WriteLine("Login() :" + "USER " + hostInfo.User);
            SendToServer("USER " + hostInfo.User);
            check = GetMessage(m_WaitTime);

            // ３３１、または２３０以外か？	
            if (!((check == 331) || (check == 230)))
            {
                SetLastError(0x0021, check, "Login() :" + MsgFromServer);

                // 切断処理
                DisConnect();

                return false;
            }
            // ３３１か？
            else if (check == 331)
            {
                // パスを送信
                SendToServer("PASS " + hostInfo.Pass);

                // 応答受信
                check = GetMessage(m_WaitTime);

                // パスワード送信結果が２３０以外か？				
                if (check != 230)
                {
                    SetLastError(0x0022, check, "Login() :" + MsgFromServer);

                    // 切断処理
                    DisConnect();

                    return false;
                }
                else
                {
                    // 認証ＯＫ
                    Console.WriteLine("Login() :" + "認証完了");
                }
            }

            // カレントディレクトリを移動
            SendToServer("CWD " + RemotePath);
            check = GetMessage(m_WaitTime);

            Console.WriteLine("Login() :" + "SendToServer(CWD  + RemotePath) の応答コード：" + check.ToString());

            // ログイン済み
            isLogin = true;

            return true;
        }


        /// <summary>
        /// カレントディレクトリを移動します
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public virtual bool ChangeDirectory(string remotePath)
        {
            RemotePath = remotePath;

            int check = 0;
            SetLastError(0, 0, "");

            // カレントディレクトリを移動
            SendToServer("CWD " + RemotePath);
            check = GetMessage(m_WaitTime);

            // 成功か？
            if (check == 250)
            {
                return true;
            }
            return false;
        }

        //////////////////////////////////////////////
        // 終了処理
        //////////////////////////////////////////////
        /// <summary>
        /// 対象サーバからログアウトする。
        /// QUITコマンドをサーバに送ります。
        /// 処理が完了するまでブロッキングされます。
        /// </summary>
        public virtual void Logout()
        {
            int check = 0;

            SetLastError(0, 0, "");

            // クライアントからQUITコマンドを送信する
            if (SendToServer("QUIT"))
            {
                check = GetMessage(m_WaitTime);
                Console.WriteLine("Logout() :" + check.ToString());
                DisConnect();
            }
            else
            {
                DisConnect();
            }
        }

        //////////////////////////////////////////////
        // ダウンロード
        //////////////////////////////////////////////
        /// <summary>
        /// 対象サーバからファイルのダウンロードを行う。
        /// 本メソッドはサーバのルートディレクトリにあるファイルをダウンロード対象とします。
        /// 処理が完了するまでブロッキングされます。
        /// </summary>
        /// <param name="remoteFilePath">ダウンロードするファイル名称</param>
        /// <param name="localFilePath">保存先のパス(パスの最後に\\は不要です)</param>
        /// <returns>成功/失敗：true/false</returns>
        public virtual bool DownloadFile(string remoteFilePath, string localFilePath)
        {
            SetLastError(0, 0, "");

            m_DownloadSize = 0;
            int check = 0;

            // 接続状態チェック
            if (isLogin == false)				// １度接続したのか？
            {
                SetLastError(0x1003, 0, "DownloadFile() :" + "Loginしてください");
                return false;					// 接続失敗
            }
            // ファイル転送モードを指定する
            SetFileTransMode(remoteFilePath);

            // 伝送パラメータコマンド（パッシブモード）の転送指定を行い、データコネクションポートを得る
            IPEndPoint Server = null;
            Server = PasvServerInfo();

            // ポート取得失敗？
            if (Server == null)
            {
                SetLastError(0x1001, 0, "DownloadFile() :" + MsgFromServer);
                isLogin = false;				// 未ログイン状態へ

                return false;
            }

            // データコネクション			
            Socket socket = new Socket(AddressFamily.InterNetwork,
                                        SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // データコネクション実行
                socket.Connect(Server);
            }
            catch
            {
                Console.WriteLine("DownloadFile() :" + "DL：データコネクションの確立に失敗しました。");
                SetLastError(0x1002, 0, "DL：データコネクションの確立に失敗しました。");
                return false;
            }

            // サーバへダウンロードコマンド送信
            SendToServer("RETR " + remoteFilePath);

            // 応答受信
            check = GetMessage(m_WaitTime);
            Console.WriteLine("DownloadFile() :" + "RETR 要求に対する応答コード :" + check.ToString());

            // 受信ストリームをファイルに書込み
            WriteToLocal(ref socket, localFilePath);

            // 応答受信
            check = GetMessage(m_WaitTime);
            Console.WriteLine("DownloadFile() :" + "WriteToLocalの実行結果コード :" + check.ToString());

            return true;
        }

        //////////////////////////////////////////////
        // アップロード
        //////////////////////////////////////////////
        /// <summary>
        /// 対象サーバのルートディレクトリにファイルをアップロードする。
        /// 処理が完了するまでブロッキングされます。
        /// </summary>
        /// <param name="remoteFilePath">パスを含むアップロード先のファイル名称</param>
        /// <param name="localFilePath">パスを含むアップロード元のファイル名称</param>
        /// <returns>成功/失敗：true/false</returns>
        public virtual bool UploadFile(string remoteFilePath, string localFilePath)
        {
            int check = 0;
            //			string name;			// パスは含まないファイル名称(サーバのルートディレクトリに置かれるファイル名称)
            m_UploadSize = 0;

            SetLastError(0, 0, "");

            // ファイルチェック
            if (!System.IO.File.Exists(localFilePath))
            {
                SetLastError(0x2001, 0, "UploadFile() :" + "ファイルがありません。 File : " + localFilePath);
                return false;
            }

            // 接続状態チェック
            if (isLogin == false)				// １度もログインしていない？
            {
                SetLastError(0x2002, 0, "UploadFile() :" + "Loginしてください");
                return false;					// 接続失敗
            }

            // 送受信するネットワークストリームを取得する			
            netStream_Ctrl = tcpClient_Ctrl.GetStream();

            // ファイル転送モードを指定する
            SetFileTransMode(remoteFilePath);

            // 伝送パラメータコマンド（パッシブモード）の転送指定を行い、データコネクションポートを得る
            IPEndPoint Server = null;

            // パッシブモード要求→サーバから接続ポートの通知を入力
            Server = PasvServerInfo();

            // 応答ＮＧ？
            if (Server == null)
            {
                isLogin = false;
                SetLastError(0x2003, 0, "UploadFile() :" + "転送モード(Passive)の指定に失敗しました。");
                return false;
            }

            // データコネクション用ソケット作成
            Socket socket = new Socket(AddressFamily.InterNetwork,
                                        SocketType.Stream, ProtocolType.Tcp);
            try
            {
                // データコネクション実行
                socket.Connect(Server);
            }
            catch
            {
                SetLastError(0x2004, 0, "UploadFile() :" + "UL : データコネクションが確立できません。");
                return false;
            }

            // アップロードコマンド
            // ルートディレクトリにアップするため、ここでパラメータに渡すファイル名称に、パスを含んではいけない
            SendToServer("STOR " + remoteFilePath);

            // コマンド応答入力
            check = GetMessage(m_WaitTime);

            // １２５や１５０ではない？
            if (check != 125 && check != 150)
            {
                SetLastError(0x2005, check, "UploadFile() :" + "アップロード要求 STOR ： サーバの応答がNGです。 応答コード：" + check.ToString());

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();

                return false;
            }
            // ファイル転送
            SendFromLocal(ref socket, localFilePath);

            // コマンド応答入力
            check = GetMessage(m_WaitTime);
            Console.WriteLine("UploadFile() :" + "SendFromLocalの実行結果コード :" + check.ToString());

            if(check != 226)
            {
                SetLastError(0x2006, check, "UploadFile() :" + "ファイル転送 ： サーバの応答がNGです。 応答コード：" + check.ToString());
            }

            return true;
        }


        //////////////////////////////////////////////
        // ファイル削除
        //////////////////////////////////////////////
        /// <summary>
        /// 対象サーバのルートディレクトリから指定ファイルを削除する。
        /// 処理完了までブロッキングされます。
        /// </summary>
        /// <param name="targetFilePath">パスを含む削除するファイル名称</param>
        /// <returns>成功/失敗：true/false</returns>
        public virtual bool DeleteFile(string targetFilePath)
        {
            int check = 0;

            SetLastError(0, 0, "");

            // 接続状態チェック
            if (isLogin == false)				// １度もログインしていない？
            {
                SetLastError(0x3001, 0, "DeleteFile() :" + "Loginしてください");
                return false;					// 接続失敗
            }

            // サーバへダウンロードコマンド送信
            SendToServer("DELE " + targetFilePath);

            // コマンド応答入力
            check = GetMessage(m_WaitTime);

            // 成功か？
            if (check == 250)
            {
                return true;
            }
            return false;
        }

        //////////////////////////////////////////////
        // 転送モード指定
        //////////////////////////////////////////////
        /// <summary>
        /// 転送モードの指定。
        /// 対Windows転送なのでイメージモードで行う。
        /// </summary>
        /// <param name="fileName">拡張子を含むファイル名称</param>
        private void SetFileTransMode(string fileName)
        {
            int check = 0;

            // イメージモードで転送
            SendToServer("TYPE I");
            check = GetMessage(m_WaitTime);
            Console.WriteLine("SetFileTransMode() :" + "転送モード指示の応答コード :" + check.ToString());
        }

        //////////////////////////////////////////////
        //
        //////////////////////////////////////////////
        /// <summary>
        /// アップロード時のストリーム送信
        /// </summary>
        /// <param name="socket">参照ソケット</param>
        /// <param name="filePath">アップロードするファイル名(フルパス)</param>
        private void SendFromLocal(ref Socket socket, string filePath)
        {
            Console.WriteLine("SendFromLocal() :" + "アップロードを開始");

            // 対windows間を前提とするので、全部イメージモード。

            // 送信ファイルストリームを準備
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // 送信バイト数の初期化
            int readBytes = 0;
            int sockSend = 0;

            byte[] bb = new byte[m_SendTransSize];

            while ((readBytes = fs.Read(bb, 0, bb.Length)) != 0)
            {
                // ストリームの送信
                sockSend = socket.Send(bb, readBytes, SocketFlags.None);

                // 送信サイズ更新
                lock (SyncUL)
                {
                    m_UploadSize += readBytes;
                    Console.WriteLine("SendFromLocal() :" + "Send : " + m_UploadSize.ToString() + " Bytes");
                }

                if (sockSend != readBytes)
                {
                    Console.WriteLine("送信サイズとソケットへの送信サイズが不一致しています");
                }

            }
            fs.Close();
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }

        /// <summary>
        /// 受信したデータをローカルドライブに書き込む 
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="downloadFile"></param>
        private void WriteToLocal(ref Socket socket, string downloadFile)
        {
            Console.WriteLine("WriteToLocal() :" + "ダウンロードを開始");

            // 対windows間を前提とするので、全部イメージモード。

            int count;
            byte[] bb = new Byte[m_RecvTransSize];		// 転送量に関わるなぁ
            bb.Initialize();

            FileStream fs = new FileStream(downloadFile,
                                            FileMode.OpenOrCreate, FileAccess.Write);

            // 受信ストリームが０バイトより大きい間 ループをおこなう。
            // Receive() の戻り値が 受信データサイズ。
            for (; (count = socket.Receive(bb)) > 0;)
            {
                fs.Write(bb, 0, count);
                fs.Flush();

                // 受信サイズを更新
                lock (SyncDL)
                {
                    m_DownloadSize += count;
                    Console.WriteLine("WriteToLocal() :" + "Receive : " + m_DownloadSize.ToString() + " Bytes");
                }
            }

            try
            {
                fs.Close();
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch
            {
                Console.WriteLine("WriteToLocal() :" + "fs.Close() または socket.Shutdown() socket.Close() に失敗しました。 ");
            }
        }

        //////////////////////////////////////////////
        // サーバからのメッセージ入力
        //////////////////////////////////////////////
        /// <summary>
        /// サーバからのメッセージを取得する。
        /// </summary>
        /// <param name="waitTime">待機時間(ms)</param>
        /// <returns></returns>
        public int GetMessage(int waitTime)
        {
            int Code = 0;
            int LastCode = 0;
            string temp = "";
            string msg = "";

            // ネットワークストリーム無効？（Connectしていない）
            if (netStream_Ctrl == null)
            {
                return 0;
            }
            // ネットワークストリームからストリームリーダーインスタンスを作成
            StreamReader sr = new StreamReader(netStream_Ctrl, Encoding.Default);

            int waitCount = 0;
            bool bWait = true;

            // 応答待ち
            Console.WriteLine("GetMessage() :" + "応答待ちを開始");

            // 待ちが有効な間ループする
            while (bWait)
            {
                // 受信可能なストリームがない？
                if (!netStream_Ctrl.DataAvailable)
                {
                    Console.WriteLine("GetMessage() :" + "待ち... : " + waitCount.ToString());

                    // 100ms待つ
                    Thread.Sleep(100);
                    waitCount++;
                }
                else
                {
                    Console.WriteLine("GetMessage() :" + "ストリームあり。");
                    bWait = false;
                }

                // 指定時間越えたか？
                if ((waitCount * 100) > waitTime)
                {
                    // 応答待ちタイムアウト
                    return 0;
                }
            }

            // サーバからの応答があり、応答コードの取得を行う
            try
            {
                // 読み込んだストリームがNULL（ファイルの末尾）でないならば
                while ((temp = sr.ReadLine()) != null)
                {
                    // なぜか temp が "" の空文字で取得されることがあるんだよなぁ

                    // 読み取ったストリームは文字数０より大きい？					
                    if (temp.Length > 0)
                    {
                        Console.WriteLine("GetMessage() :" + "String[" + temp + "]");
                        msg = msg + temp;

                        try
                        {
                            LastCode = int.Parse(temp.Substring(0, 3));
                        }
                        catch
                        {
                            LastCode = 0;
                        }

                        // 受信したメッセージが最後かどうか
                        // コードの後に'-'がないのか？・・・つまり、最後の文字列か？
                        if ((LastCode != 0) && temp.Substring(3, 1).Equals(" "))
                        {
                            break;
                        }
                        Code = int.Parse(temp.Substring(0, 3));
                    }
                    else
                    {
                        // 改行コードに改行コードが繋がると空文字入力となるので読み飛ばし
                        Console.WriteLine("GetMessage() :" + "String[]");
                    }
                }
            }
            catch (IOException ioe)
            {
                Console.WriteLine("GetMessage() :" + "受信できないか、中断されました！" + ioe.Message);
                Code = 0;
            }
            catch (Exception e)
            {
                Console.WriteLine("GetMessage() :" + e.Message);
                Code = 0;
            }
            Code = LastCode;		// 終端行の応答コード

            // 応答メッセージを取得
            MsgFromServer = msg;

            // 応答コードを返却
            return Code;
        }
        
        /// <summary>
        /// 切断処理
        /// </summary>
        protected virtual void DisConnect()
        {
            // コントロールコネクション用のＴＣＰクライアントソケット無効？
            if (tcpClient_Ctrl == null)
            {
                // 未接続なら処理無し
                return;
            }

            // 切断を行う
            try
            {
                tcpClient_Ctrl.Close();			// コネクトクローズ

                isLogin = false;
                Console.WriteLine("DisConnect() :" + "Connection closed ");
            }
            catch (Exception ex)
            {
                Console.WriteLine("DisConnect() :" + ex.Message);
            }
        }

        /// <summary>
        /// エラー情報格納サブルーチン
        /// </summary>
        /// <param name="code"></param>
        /// <param name="answer"></param>
        /// <param name="message"></param>
        protected virtual void SetLastError(int code, int answer, string message)
        {
            m_LastError.Code = code;
            m_LastError.Message = message;
            m_LastError.Answer = answer;
        }

        //////////////////////////////////////////////
        // サーバに対してパッシブモードの設定
        //////////////////////////////////////////////
        private IPEndPoint PasvServerInfo()
        {
            //if (!SendToServer("PORT " + hostInfo.HostName.Replace(".", ",") + ",0,22"))
            //{
            //    return null;
            //}
            //if (!SendToServer("SYST"))
            //{
            //    return null;
            //}

            //int check = GetMessage(m_WaitTime);
            //Console.WriteLine(MsgFromServer);

            // パッシブモードのコマンド要求
            if (!SendToServer("PASV"))
            {
                return null;
            }

            int check = GetMessage(m_WaitTime);

            if (check != 227)
            {
                Console.WriteLine("PasvServerInfo() :" + "応答コード：" + check.ToString() + "\r\nPassiveモードはサポートされません");
                return null;
            }

            // 応答メッセージから"(" と ")"の位置を取得する。
            int f = MsgFromServer.IndexOf('(');
            int b = MsgFromServer.IndexOf(')');

            // サーバIP、サーバポートを取得
            string Server = MsgFromServer.Substring(f + 1, b - f - 1);

            string[] temp = Server.Split(',');

            string ServerIP = temp[0] + "." +
                              temp[1] + "." +
                              temp[2] + "." +
                              temp[3];

            int ServerPort = int.Parse(temp[4]) * 256 + int.Parse(temp[5]);

            // IPとポート番号のデータクラスを返却する
            //			IPEndPoint myEndPoint = new IPEndPoint(IPAddress.Parse(hostInfo.HostName), 22);
            IPEndPoint myEndPoint = new IPEndPoint(IPAddress.Parse(ServerIP), ServerPort);

            return myEndPoint;
        }

        //////////////////////////////////////////////
        // サーバからファイルリストの読出し
        //////////////////////////////////////////////
        private string ReadListFromServer(ref Socket socket)
        {
            string output = "";
            int count = 0;
            byte[] bb = new byte[8192];
            StringBuilder sb = new StringBuilder();

            socket.ReceiveTimeout = 1000;

            //			for (; (count = socket.Receive(bb)) > 0; )
            while (true)
            {
                try
                {
                    count = socket.Receive(bb);
                }
                catch
                {
                    count = 0;
                }
                if (count > 0)
                {

                    sb.Append(Encoding.Default.GetString(bb, 0, count));

                    if (socket.Available == 0)
                    {
                        break;
                    }
                }
                else
                {
                    break;

                }
            }
            try
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            catch
            {

            }
            output = sb.ToString();
            Console.WriteLine("ReadListFromServer() :" + output + "\r\n");


            // ファイルリストの文字列を返却
            return output;
        }

        //////////////////////////////////////////////
        // サーバルートディレクトリの最新ファイルリストを取得
        //////////////////////////////////////////////
        /// <summary>
        /// サーバのカレントディレクトリにある最新のファイルリストを取得
        /// </summary>
        /// <returns>成功／失敗：true / false</returns>
        public virtual bool RefreshFileList(string targetPath)
        {
            int check = 0;

            // パッシブモード設定
            IPEndPoint ipend = PasvServerInfo();

            if (ipend == null)
            {
                return false;
            }

            // データコネクション作成
            Socket socket = new Socket(AddressFamily.InterNetwork,
                                        SocketType.Stream,
                                        ProtocolType.Tcp);
            if (socket == null)
            {
                return false;
            }

            try
            {
                socket.Connect(ipend);
            }
            catch (Exception ex)
            {
                socket.Close();

                Console.WriteLine("RefreshFileList() :" + "RefreshFileList : データコネクションを確立できません。" + ex.Message);
                return false;
            }

            // ディレクトリ一覧の要求コマンドを送る
            //            SendToServer("LIST " + targetPath);
            if (targetPath == "")
                SendToServer("LIST");
            else
                SendToServer("LIST " + targetPath);
            Console.WriteLine("RefreshFileList() :" + "LIST : " + GetMessage(m_WaitTime).ToString());

            // サーバーからディレクトリ一覧を取得
            string msg = ReadListFromServer(ref socket);

            // string 文字列を元に、１行読み取りを行うクラス
            StringReader sr = new StringReader(msg);		// １本のStringから改行区切りでデータ取得したい

            // ファイルリストコレクションのクリア
            sc.Clear();
            m_ScFileDate.Clear();
            m_ScFileSize.Clear();

            string line = "";

            line = sr.ReadLine();

            // 正規表現を使って、ディレクトリ文字列（１行）を分解する。
            Regex regex = new Regex(@"(?<size>[0-9]{1,20})\s+(?<month>[A-Z]{1}[a-z]{2})\s+(?<day>[0-9]{1,2})\s+(?<yeartime>\S+)\s+(?<filename>\S+)");

            string name = "";
            string size = "";
            string day = "";

            string per = "-";		// 「ファイル」を示す最左端のパーミッション

            // 行がなくなるまで読み込む
            while (line != null)
            {
                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 4)
                {
                    //DOS形式 　<DIR>もしくはファイルサイズをあらわす数値
                    int s;
                    if (parts[2] != "<DIR>" && int.TryParse(parts[2], out s))
                    {
                        day = parts[0] + " " + parts[1];
                        size = parts[2];
                        name = "";
                        for (int i = 3; i < parts.Length; i++)
                        {
                            name += parts[i] + " ";
                        }

                        name = name.Trim();

                        Console.WriteLine("RefreshFileList() :" + name + " : " + size + day);
                        sc.Add(name.Trim());							// ファイル名を記憶
                        m_ScFileSize.Add(size.Trim());					// ファイルサイズを記憶
                        m_ScFileDate.Add(day.Trim());					// ファイル日付を記憶
                    }
                }
                else
                {
                    //UNIX形式かも

                    // パーミッションの最左文字は '-' （ファイル）か？
                    if (line.Substring(0, 1) == per)
                    {
                        Match m = regex.Match(line);
                        name = m.Result("${filename}");
                        size = m.Result("${size}");
                        day = m.Result("${day}");

                        Console.WriteLine("RefreshFileList() :" + name + " : " + size + day);
                        sc.Add(name.Trim());							// ファイル名を記憶
                        m_ScFileSize.Add(size.Trim());					// ファイルサイズを記憶
                        m_ScFileDate.Add(day.Trim());					// ファイル日付を記憶
                    }
                }

                line = sr.ReadLine();
            }

            // ソケットクローズ
            socket.Close();

            check = GetMessage(m_WaitTime);
            Console.WriteLine("RefreshFileList() :" + "FileList 要求結果 ：" + MsgFromServer);

            return true;
        }

        /// <summary>
        /// サーバのカレントディレクトリにある最新のフォルダリストを取得
        /// </summary>
        /// <returns>成功／失敗：true / false</returns>
        public virtual bool RefreshFileList()
        {
            int check = 0;

            // パッシブモード設定
            IPEndPoint ipend = PasvServerInfo();

            if (ipend == null)
            {
                return false;
            }

            // データコネクション作成
            Socket socket = new Socket(AddressFamily.InterNetwork,
                                        SocketType.Stream,
                                        ProtocolType.Tcp);
            if (socket == null)
            {
                return false;
            }

            try
            {
                socket.Connect(ipend);
            }
            catch (Exception ex)
            {
                socket.Close();

                Console.WriteLine("RefreshFileList() :" + "RefreshFileList : データコネクションを確立できません。" + ex.Message);
                return false;
            }

            // ディレクトリ一覧の要求コマンドを送る
            SendToServer("LIST");
            Console.WriteLine("RefreshFileList() :" + "LIST : " + GetMessage(m_WaitTime).ToString());

            // サーバーからディレクトリ一覧を取得
            string msg = ReadListFromServer(ref socket);

            // string 文字列を元に、１行読み取りを行うクラス
            StringReader sr = new StringReader(msg);		// １本のStringから改行区切りでデータ取得したい

            // ファイルリストコレクションのクリア
            sc.Clear();
            m_ScFileDate.Clear();
            m_ScFileSize.Clear();

            string line = "";

            line = sr.ReadLine();

            // 正規表現を使って、ディレクトリ文字列（１行）を分解する。
            Regex regex = new Regex(@"(?<size>[0-9]{1,20})\s+(?<month>[A-Z]{1}[a-z]{2})\s+(?<day>[0-9]{1,2})\s+(?<yeartime>\S+)\s+(?<filename>\S+)");

            string name = "";
            string size = "";
            string day = "";

            //string per = "-";		// 「ファイル」を示す最左端のパーミッション

            // 行がなくなるまで読み込む
            while (line != null)
            {
                string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length >= 4)
                {
                    //DOS形式 　<DIR>ならフォルダ
                    //int s;
                    if (parts[2] == "<DIR>")
                    {
                        day = parts[0] + " " + parts[1];
                        size = parts[2];
                        name = "";
                        for (int i = 3; i < parts.Length; i++)
                        {
                            name += parts[i] + " ";
                        }

                        name = name.Trim();

                        Console.WriteLine("RefreshFileList() :" + name + " : " + size + day);
                        sc.Add(name.Trim());							// ファイル名を記憶
                        //m_ScFileSize.Add(size.Trim());					// ファイルサイズを記憶
                        m_ScFileDate.Add(day.Trim());					// ファイル日付を記憶
                    }
                }

                line = sr.ReadLine();
            }

            // ソケットクローズ
            socket.Close();

            check = GetMessage(m_WaitTime);
            Console.WriteLine("RefreshFileList() :" + "FileList 要求結果 ：" + MsgFromServer);

            return true;
        }

        /// <summary>
        /// サーバー内のファイルの名前変更を行います。
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public virtual bool Rename(string source, string destination)
        {
            int check = 0;
            SetLastError(0, 0, "");

            // 接続状態チェック
            if (isLogin == false)				// １度もログインしていない？
            {
                SetLastError(0x3001, 0, "Rename() :" + "Loginしてください");
                return false;					// 接続失敗
            }

            // サーバへリネームコマンド送信
            SendToServer("RNFR " + source);

            // コマンド応答入力
            check = GetMessage(m_WaitTime);

            // 成功か？
            if (check == 350)
            {
                SendToServer("RNTO " + destination);

                // コマンド応答入力
                check = GetMessage(m_WaitTime);

                // 成功か？
                if (check == 250)
                {
                    return true;
                }
            }

            return false;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class BL_FtpClient_Active : BL_FtpClient
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="HostName"></param>
        /// <param name="UserName"></param>
        /// <param name="Pass"></param>
        ///// <param name="LocalIp"></param>
        ///// <param name="LocalPort"></param>
        //public BL_FtpClient_Active(string HostName, string UserName, string Pass, string LocalIp, int LocalPort)
        public BL_FtpClient_Active(string HostName, string UserName, string Pass)
            : base(HostName, UserName, Pass)
        {
            //this.LocalIp = LocalIp;
            //this.LocalPort = LocalPort;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public override bool Login(string remotePath)
        {
            RemotePath = remotePath;

            return RefreshFileList();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Logout()
        {
            SetLastError(0, 0, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remotePath"></param>
        /// <returns></returns>
        public override bool ChangeDirectory(string remotePath)
        {
            RemotePath = remotePath;

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetPath"></param>
        /// <returns></returns>
        public override bool RefreshFileList(string targetPath)
        {
            bool ret = true;

            string uri = "ftp://" + hostInfo.HostName + "/";
            if (targetPath != "")
            {
                uri = uri + targetPath + "/";
            }
            
            Uri u = new Uri(uri);
            System.Net.FtpWebRequest ftpReq = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(u);
            ftpReq.Credentials = new System.Net.NetworkCredential(hostInfo.User, hostInfo.Pass);
            ftpReq.Method = System.Net.WebRequestMethods.Ftp.ListDirectoryDetails;
            ftpReq.KeepAlive = false;
            ftpReq.UsePassive = false;

            System.Net.FtpWebResponse ftpRes = (System.Net.FtpWebResponse)ftpReq.GetResponse();

            if (ftpRes.StatusCode != FtpStatusCode.OpeningData)
            {
                isLogin = false;

                SetLastError(0x4001, (int)ftpRes.StatusCode, ftpRes.StatusDescription);
                ret = false;
            }
            else
            {
                isLogin = true;

                System.IO.StreamReader sr = new System.IO.StreamReader(ftpRes.GetResponseStream(), Encoding.UTF8);

                //ファイル一覧を取得
                {
                    // ファイルリストコレクションのクリア
                    sc.Clear();
                    m_ScFileDate.Clear();
                    m_ScFileSize.Clear();

                    string line = "";
                    line = sr.ReadLine();

                    // 正規表現を使って、ディレクトリ文字列（１行）を分解する。
                    Regex regex = new Regex(@"(?<size>[0-9]{1,20})\s+(?<month>[A-Z]{1}[a-z]{2})\s+(?<day>[0-9]{1,2})\s+(?<yeartime>\S+)\s+(?<filename>\S+)");

                    string name = "";
                    string size = "";
                    string day = "";

                    // 行がなくなるまで読み込む
                    while (line != null)
                    {
                        string[] parts = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length == 9)
                        {
                            //DOS形式 　<DIR>もしくはファイルサイズをあらわす数値
                            int s;
                            if (line.PadRight(1).Substring(0, 1) != "d" && parts[4] != "<DIR>" && int.TryParse(parts[4], out s))
                            {
                                //day = parts[5] + " " + parts[6];
                                day = DateTime.Now.ToString("MM-dd-yy");
                                size = parts[4];
                                name = "";
                                for (int i = 8; i < parts.Length; i++)
                                {
                                    name += parts[i] + " ";
                                }

                                name = name.Trim();

                                Console.WriteLine("RefreshFileList() :" + name + " : " + size + day);
                                sc.Add(name.Trim());                            // ファイル名を記憶
                                m_ScFileSize.Add(size.Trim());                  // ファイルサイズを記憶
                                m_ScFileDate.Add(day.Trim());                   // ファイル日付を記憶
                            }
                        }
                        else if (parts.Length >= 4)
                        {
                            //DOS形式 　<DIR>ならフォルダ
                            //int s;
                            if (parts[2] == "<DIR>")
                            {
                                day = parts[0] + " " + parts[1];
                                size = parts[2];
                                name = "";
                                for (int i = 3; i < parts.Length; i++)
                                {
                                    name += parts[i] + " ";
                                }

                                name = name.Trim();

                                Console.WriteLine("RefreshFileList() :" + name + " : " + size + day);
                                sc.Add(name.Trim());                            // ファイル名を記憶
                                m_ScFileSize.Add(size.Trim());					// ファイルサイズを記憶
                                m_ScFileDate.Add(day.Trim());                   // ファイル日付を記憶
                            }
                        }
                        else
                        {
                            //UNIX形式かも

                            // パーミッションの最左文字は '-' （ファイル）か？
                            if (line.PadRight(1).Substring(0, 1) == "-")
                            {
                                Match m = regex.Match(line);
                                name = m.Result("${filename}");
                                size = m.Result("${size}");
                                day = m.Result("${day}");

                                Console.WriteLine("RefreshFileList() :" + name + " : " + size + day);
                                sc.Add(name.Trim());                            // ファイル名を記憶
                                m_ScFileSize.Add(size.Trim());                  // ファイルサイズを記憶
                                m_ScFileDate.Add(day.Trim());                   // ファイル日付を記憶
                            }
                        }

                        line = sr.ReadLine();
                    }
                }

                sr.Close();
            }

            ftpRes.Close();

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override bool RefreshFileList()
        {
            return this.RefreshFileList(RemotePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="targetFilePath"></param>
        /// <returns></returns>
        public override bool DeleteFile(string targetFilePath)
        {
            bool ret = true;

            Uri u = new Uri("ftp://" + hostInfo.HostName + "/" + targetFilePath);
            System.Net.FtpWebRequest ftpReq = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(u);
            ftpReq.Credentials = new System.Net.NetworkCredential(hostInfo.User, hostInfo.Pass);
            ftpReq.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
            System.Net.FtpWebResponse ftpRes = (System.Net.FtpWebResponse)ftpReq.GetResponse();

            if (ftpRes.StatusCode != FtpStatusCode.FileActionOK)
            {
                SetLastError(0x4002, (int)ftpRes.StatusCode, ftpRes.StatusDescription);
                ret = false;
            }

            ftpRes.Close();

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public override bool Rename(string source, string destination)
        {
            bool ret = true;

            Uri u = new Uri("ftp://" + hostInfo.HostName + "/" + source);
            string newName = destination;

            System.Net.FtpWebRequest ftpReq = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(u);
            ftpReq.Credentials = new System.Net.NetworkCredential(hostInfo.User, hostInfo.Pass);
            ftpReq.Method = System.Net.WebRequestMethods.Ftp.Rename;

            ftpReq.RenameTo = newName;

            System.Net.FtpWebResponse ftpRes = (System.Net.FtpWebResponse)ftpReq.GetResponse();

            if (ftpRes.StatusCode != FtpStatusCode.FileActionOK)
            {
                SetLastError(0x4003, (int)ftpRes.StatusCode, ftpRes.StatusDescription);
                ret = false;
            }

            ftpRes.Close();

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteFilePath"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public override bool DownloadFile(string remoteFilePath, string localFilePath)
        {
            bool ret = true;

            Uri u = new Uri("ftp://" + hostInfo.HostName + "/" + remoteFilePath);
            string downFile = Path.Combine(localFilePath, Path.GetFileName(remoteFilePath));

            System.Net.FtpWebRequest ftpReq = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(u);
            ftpReq.Credentials = new System.Net.NetworkCredential(hostInfo.User, hostInfo.Pass);
            ftpReq.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
            ftpReq.KeepAlive = false;
            ftpReq.UseBinary = true;
            ftpReq.UsePassive = false;

            System.Net.FtpWebResponse ftpRes = (System.Net.FtpWebResponse)ftpReq.GetResponse();
            if (ftpRes.StatusCode != FtpStatusCode.OpeningData)
            {
                SetLastError(0x4004, (int)ftpRes.StatusCode, ftpRes.StatusDescription);
                ret = false;
            }
            else
            {
                System.IO.Stream resStrm = ftpRes.GetResponseStream();
                System.IO.FileStream fs = new System.IO.FileStream(downFile, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int readSize = resStrm.Read(buffer, 0, buffer.Length);
                    if (readSize == 0)
                        break;
                    fs.Write(buffer, 0, readSize);
                }
                fs.Close();
                resStrm.Close();
            }

            ftpRes.Close();

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="remoteFilePath"></param>
        /// <param name="localFilePath"></param>
        /// <returns></returns>
        public override bool UploadFile(string remoteFilePath, string localFilePath)
        {
            bool ret = true;

            string upFile = localFilePath;

            string uri = "ftp://" + hostInfo.HostName + "/";
            if (remoteFilePath != "")
            {
                uri = uri + remoteFilePath;
            }
            else if (RemotePath != "")
            {
                uri = uri + RemotePath + "/" + Path.GetFileName(localFilePath);
            }
            else
            {
                uri = uri + Path.GetFileName(localFilePath);
            }

            Uri u = new Uri(uri);
            System.Net.FtpWebRequest ftpReq = (System.Net.FtpWebRequest)System.Net.WebRequest.Create(u);
            ftpReq.Credentials = new System.Net.NetworkCredential(hostInfo.User, hostInfo.Pass);
            ftpReq.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
            ftpReq.KeepAlive = false;
            ftpReq.UseBinary = true;
            ftpReq.UsePassive = false;

            try
            {
                System.IO.Stream reqStrm = ftpReq.GetRequestStream();
                System.IO.FileStream fs = new System.IO.FileStream(upFile, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                byte[] buffer = new byte[1024];
                while (true)
                {
                    int readSize = fs.Read(buffer, 0, buffer.Length);
                    if (readSize == 0)
                        break;
                    reqStrm.Write(buffer, 0, readSize);
                }
                fs.Close();
                reqStrm.Close();

                System.Net.FtpWebResponse ftpRes = (System.Net.FtpWebResponse)ftpReq.GetResponse();

                if (ftpRes.StatusCode != FtpStatusCode.FileActionOK)
                {
                    SetLastError(0x4005, (int)ftpRes.StatusCode, ftpRes.StatusDescription);
                    ret = false;
                }

                ftpRes.Close();
            }
            catch (Exception ex)
            {
                SetLastError(0x4006, 0, ex.Message);
                ret = false;
            }

            return ret;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void DisConnect()
        {
        }
    }
}
