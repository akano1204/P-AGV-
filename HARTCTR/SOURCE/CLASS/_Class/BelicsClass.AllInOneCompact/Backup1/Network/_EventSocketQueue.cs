using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

using BelicsClass.Common;

namespace BelicsClass.Network
{
    /// <summary>
    /// EventSocketQueueはFIFOとイベントおよびソケットを持ちます。
    /// FIFOにデータがある場合、イベントはシグナル状態になり、ソケットは受信データありになります。
    /// FIFOにデータがない場合、イベントはノンシグナル状態になり、ソケットは受信データ無しになります。
    /// イベントはGetWaitHandleにより取得でき、System.Threading.WaitHandleクラスの
    /// Wait*メソッドによりイベント発生を待つことが可能です。
    /// ソケットはEventSocketにより取得でき、System.Net.SocketクラスのSelectメソッドに
    /// よりイベント発生を待つことが可能です。
    /// </summary>
    /// <remarks>
    /// EventSocketで取得できるソケット以外のメンバはスレッドセーフです。詳細はSockeEventプロパティを
    /// 参照してください。<br/>
    /// 排他状態でキューを操作したい場合は、インスタンスに対してlockを行ってください。
    /// </remarks>
    public class BL_EventSocketQueue : BL_EventQueue, IDisposable
    {
        /// <summary>
        /// デフォルトコンストラクタです。
        /// </summary>
        public BL_EventSocketQueue()
            : base()
        {
            Initialize();
        }

        /// <summary>
        /// キュー登録数制限付きのコンストラクタです。
        /// </summary>
        public BL_EventSocketQueue(int limitCount)
            : base(limitCount)
        {
            Initialize();
        }

        /// <summary>
        /// メンバの初期化を行います。
        /// </summary>
        /// <remarks>
        /// コンストラクタで呼び出されます。
        /// </remarks>
        private void Initialize()
        {
            //受信用(イベント用)ソケット生成
            //受信専用(送信を無効)
            //ソケット生成
            m_EventSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            m_EventSocket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            m_EventSocket.Shutdown(SocketShutdown.Send);

            //受信用のポイントを記憶
            m_EventEndPoint = m_EventSocket.LocalEndPoint;
        }

        /// <summary>イベント用のソケットです。
        /// IPはループバック、ポートはオートバインドです。</summary>
        private Socket m_EventSocket;

        /// <summary>イベント用ソケットのEndPointです。</summary>
        private EndPoint m_EventEndPoint;

        /// <summary>staticブロックです。</summary>
        static BL_EventSocketQueue()
        {
            //インスタンス共通の送信用(イベント発行用)ソケット生成
            //送信専用(受信を無効)
            s_SendSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            s_SendSocket.Bind(new IPEndPoint(IPAddress.Loopback, 0));
            s_SendSocket.Shutdown(SocketShutdown.Receive);

            //送信用ソケットのIPポイント
            //s_SendSocketEndPoint = (IPEndPoint)s_SendSocket.LocalEndPoint;

            //インスタンス共通の送受信用ワークを生成
            s_WorkBuffer = new byte[1];
            s_WorkEndPoint = new IPEndPoint(IPAddress.Loopback, 0);
        }

        //送信用ソケットのIPポイント
        //static readonly IPEndPoint s_SendSocketEndPoint;

        /// <summary>イベント用ソケットにデータを送信するソケットです。</summary>
        static Socket s_SendSocket;

        /// <summary>カラ送信およびカラ受信用のバッファです。</summary>
        static byte[] s_WorkBuffer;

        /// <summary>カラ送信およびカラ受信用のEndPointです。</summary>
        static EndPoint s_WorkEndPoint;

        /// <summary>ソケットの受信イベントを解除します。</summary>
        private void ResetEvent()
        {
            //受信イベント用のソケットの着信データを廃棄する
            lock (s_SendSocket)
            {
                while (m_EventSocket.Available > 0)
                {
                    m_EventSocket.ReceiveFrom(s_WorkBuffer, ref s_WorkEndPoint);
                }
            }
        }

        /// <summary>ソケットの受信イベントを発行します。</summary>
        private void SetEvent()
        {
            //受信イベント用のソケットへデータを送信
            lock (s_SendSocket)
            {
                s_SendSocket.SendTo(s_WorkBuffer, m_EventEndPoint);
            }
        }

        /// <summary>キューへ登録します。</summary>
        /// <param name="obj">登録するオブジェクトを指定します。</param>
        /// <returns>登録成功=true/登録失敗=false(キュー登録数制限)</returns>
        public override bool Enqueue(object obj)
        {
            lock (this)
            {
                //基底クラスでキュー登録
                if (base.Enqueue(obj) == false)
                {
                    return false;
                }

                //登録できたのでイベントを発行
                SetEvent();
            }
            return true;
        }

        /// <summary>
        /// キューから読み出します。
        /// </summary>
        /// <returns>読み出したオブジェクトを返します。
        /// キューが空の場合はnullを返します。</returns>
        public override object Dequeue()
        {
            lock (this)
            {
                //基底クラスでキュー読み出し
                object rtnObj = base.Dequeue();

                //データ無しになったのでイベント解除
                if (Count <= 0)
                {
                    ResetEvent();
                }
                return rtnObj;
            }
        }

        /// <summary>
        /// キューをクリアします。
        /// </summary>
        public override void Clear()
        {
            lock (this)
            {
                //基底クラスでキューをクリア
                base.Clear();

                //イベントを解除
                ResetEvent();
            }
        }

        /// <summary>
        /// イベント用のソケットを取得します。<b/>
        /// キューにデータが入っている場合、ソケットは受信データありとなります。
        /// キューにデータが入っていない場合、ソケットは受信データ無しとなります。<b/><b/>
        /// 【他のメンバとのスレッドセーフについて】<b/>
        /// EventSocketで得たソケットとDequeueメソッドおよびClearメソッドはスレッドセーフ
        /// ではありません。ソケットとDequeueメソッドおよびClearメソッドは単一のスレッド
        /// で処理してください。<b/><b/>
        /// 【取得したソケットについて】<b/>
        /// 取得したソケットに対してはSocketクラスのSelectメソッドにて受信待機を行う以外、
        /// 一切の操作を行うことを禁じます。<b/>
        /// 上記以外の操作について、一切の保証はありません。<b/>
        /// </summary>
        public Socket EventSocket
        {
            get
            {
                return m_EventSocket;
            }
        }

        /// <summary>インスタンスを無効にします。</summary>
        public void Dispose()
        {
            //既に無効
            if (m_Queue == null) return;

            try
            {
                //イベントソケットソケット解放
                m_EventSocket.Shutdown(SocketShutdown.Both);
                m_EventSocket.Close();
                m_EventSocket = null;

                m_Event = null;
                m_EventEndPoint = null;
                m_Queue.Clear();
                m_Queue = null;
            }
            catch { }
        }
    }
}
