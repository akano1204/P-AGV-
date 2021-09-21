using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using BelicsClass.Common;
using BelicsClass.ProcessManage;

namespace BelicsClass.ObjectSync
{
    /// <summary>
    /// 共有メモリを介してキュー機能を提供します
    /// 
    /// 抽象クラスTは公開デフォルトコンストラクタが実装されていなければなりません
    /// データ追加側とデータ取出側で、同一名称を指定してください
    /// 複数のデータ追加側が存在する場合、データ追加順序は保証されません
    /// 
    /// 不必要になった本オブジェクトはDisposeしなければなりません
    /// 抽象クラスT は BL_Object から派生されたデータ型が必要です。
    /// 
    /// </summary>
    /// <typeparam name="T">キューで取り扱う型</typeparam>
    public class BL_TypedQueueMemorySync<T> : IDisposable where T : new()
    {
        /// <summary>
        /// キューのヘッダーが更新されたことを通知するイベントデリゲート
        /// </summary>
        /// <param name="sender">発生源</param>
        /// <param name="data">更新されたフィールド名</param>
        public delegate void Event_Handler_Queued(BL_TypedQueueMemorySync<T> sender, T data);

        /// <summary>
        /// キューのヘッダーが更新されたことを通知するイベント
        /// </summary>
        public virtual event Event_Handler_Queued EventQueued;

        /// <summary>
        /// キューのヘッダーが更新されたことを通知するイベント
        /// </summary>
        public virtual event Event_Handler_Queued EventDequeued;

        #region キューヘッダー管理クラス

        /// <summary>
        /// キューヘッダー管理クラス
        /// </summary>
        public class QueueHeader : BL_FaceMemorySyncNotify
        {
            /// <summary>
            /// キュー要素を保持するためのクラス
            /// </summary>
            public class QueueData : BL_FaceMemorySync
            {
                ///// <summary>エントリーデータのクラス型名称</summary>
                //[BL_ObjectSyncAttribute]
                //public string TypeName = "                                                  ";

                /// <summary>キュー要素</summary>
                [BL_ObjectSyncAttribute]
                public T Data = new T();

                /// <summary></summary>
                public QueueData() { }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="sharemem_name"></param>
                /// <param name="pos"></param>
                public QueueData(string sharemem_name, long pos)
                {
                    string qd_name = "QD_" + sharemem_name + "_" + pos.ToString("0000") + "";
                    Initialize(qd_name);
                }
            }

            /// <summary>次エントリーを保持するための共有メモリ名に付加する番号</summary>
            [BL_ObjectSyncAttribute]
            public int tail = 0;

            /// <summary>先頭エントリーを保持するための共有メモリ名に付加する番号</summary>
            [BL_ObjectSyncAttribute]
            public int head = 0;

            /// <summary>待機データ数</summary>
            public long Count
            {
                get
                {
                    lock (this)
                    {
                        ReadMemory("head", false);
                        ReadMemory("tail", false);

                        if (tail < head) return tail + queMemories.Length - head;
                        return tail - head;
                    }
                }
            }

            private object syncEnque = new object();
            private object syncDeque = new object();

            /// <summary>キュー要素を保持するための共有メモリ名の一覧</summary>
            public QueueData[] queMemories = null;

            #region コンストラクタ

            /// <summary>
            /// デフォルトコンストラクタ
            /// </summary>
            public QueueHeader()
            {
                queMemories = new QueueData[32 + 1];
                for (int i = 0; i < queMemories.Length; i++)
                {
                    queMemories[i] = new QueueData();
                }
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="que_name">キューヘッダーの共有メモリ名称</param>
            /// <param name="interval">タイムアウト監視周期(ms)</param>
            public QueueHeader(string que_name, int interval) : this(que_name, interval, 32) { }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="que_name">キューヘッダーの共有メモリ名称</param>
            /// <param name="interval">タイムアウト監視周期(ms)</param>
            /// <param name="size">キューに格納できる最大要素数</param>
            public QueueHeader(string que_name, int interval, int size)
            {
                T data = new T();
                if (!typeof(BL_ObjectSync).IsInstanceOfType(data))
                {
                    throw new Exception("<T> は BL_ObjectSync型から派生されたデータ型が必要です。");
                }

                queMemories = new QueueData[size + 1];
                for (int pos = 0; pos < queMemories.Length; pos++)
                {
                    queMemories[pos] = new QueueData(que_name, pos);
                }

                Initialize<QueueHeader>(interval, this, que_name);
            }

            #endregion

            #region ENQUEUE

            /// <summary>
            /// キューにエントリーします
            /// </summary>
            /// <param name="data">エントリーするデータ(BL_ObjectSync派生型クラスのインスタンスが必要です)</param>
            /// <param name="enque_timeout_millseconds">キューがいっぱいの場合、エントリーできるまで待機する時間(ms)</param>
            /// <returns></returns>
            public bool Enqueue(T data, int enque_timeout_millseconds)
            {
                //lock (this)
                {
                    bool ret = true;

                    BL_Stopwatch sw = new BL_Stopwatch();
                    sw.Start();

                    while (true)
                    {
                        //if (!IsLock)
                        {
                            Lock();
                            ReadMemory("head", false);
                            ReadMemory("tail", false);
                            if (head % queMemories.Length != (tail + 1) % queMemories.Length) break;
                            Unlock();

                            if (0 <= enque_timeout_millseconds && enque_timeout_millseconds <= sw.ElapsedMilliseconds)
                            {
                                return false;
                            }
                        }

                        BL_Win32API.Sleep(20);
                    }

                    long pos = tail % (queMemories.Length);

                    queMemories[pos].Lock();
                    queMemories[pos].Data = data;
                    queMemories[pos].ObjectAdjustment();
                    queMemories[pos].WriteMemory(false);
                    queMemories[pos].Unlock();

                    tail = (tail + 1) % queMemories.Length;
                    WriteMemory("tail", false);
                    Unlock();

                    return ret;
                }
            }

            #endregion

            #region DEQUEUE

            /// <summary>
            /// キューから取り出します
            /// </summary>
            /// <returns></returns>
            public T Dequeue(int deque_timeout_millseconds, bool no_delete)
            {
                //lock (this)
                {
                    T ret = default(T);

                    BL_Stopwatch sw = new BL_Stopwatch();
                    sw.Start();

                    while (true)
                    {
                        //if (!IsLock)
                        {
                            Lock();
                            ReadMemory("head", false);
                            ReadMemory("tail", false);
                            if (head % queMemories.Length != tail % queMemories.Length) break;
                            Unlock();

                            if (0 <= deque_timeout_millseconds && deque_timeout_millseconds <= sw.ElapsedMilliseconds)
                            {
                                return ret;
                            }
                        }

                        BL_Win32API.Sleep(20);
                    }

                    long pos = head % queMemories.Length;

                    queMemories[pos].ReadMemory();
                    ret = queMemories[pos].Data;

                    head = (head + 1) % queMemories.Length;
                    WriteMemory("head", false);

                    Unlock();
                    return ret;
                }
            }

            #endregion

            /// <summary>
            /// 
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            public T this[long index]
            {
                get
                {
                    long pos = index % queMemories.Length;
                    queMemories[pos].ReadMemory();
                    return queMemories[pos].Data;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            public override void Dispose()
            {
                base.Dispose();

                for (int i = 0; i < queMemories.Length; i++)
                {
                    queMemories[i].Dispose();
                }
            }
        }

        #endregion

        private QueueHeader queHeader = null;

        /// <summary>
        /// コンストラクタ
        /// キューの最大エントリー数は３２個で初期化されます
        /// </summary>
        /// <param name="que_name"></param>
        public BL_TypedQueueMemorySync(string que_name) : this(que_name, 20, 32) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="que_name">キュー名称</param>
        /// <param name="interval">タイムアウト監視周期の指定時間</param>
        public BL_TypedQueueMemorySync(string que_name, int interval): this(que_name, interval, 32) { }
        
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="que_name">キュー名称</param>
        /// <param name="interval">タイムアウト監視周期の指定時間</param>
        /// <param name="size">エントリー数</param>
        public BL_TypedQueueMemorySync(string que_name, int interval, int size)
        {
            queHeader = new QueueHeader(que_name, interval, size);
            queHeader.EventModified += queHeader_EventModified;
        }

        private void queHeader_EventModified(BL_FaceMemorySyncNotify sender, string[] field_names)
        {
            foreach (string field_name in field_names)
            {
                queHeader.ReadMemory(field_name, false);

                switch (field_name)
                {
                    case "tail":
                        if (EventQueued != null)
                        {
                            int pos = queHeader.tail - 1;
                            if (pos < 0) pos = queHeader.queMemories.Length - 1;
                            EventQueued(this, queHeader[pos]);
                        }
                        break;

                    case "head":
                        if (EventDequeued != null)
                        {
                            int pos = queHeader.head - 1;
                            if (pos < 0) pos = queHeader.queMemories.Length - 1;
                            EventDequeued(this, queHeader[pos]);
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// デストラクタ
        /// </summary>
        ~BL_TypedQueueMemorySync()
        {
            Dispose();
        }

        /// <summary>
        /// 破棄
        /// キューのエントリー監視スレッドを停止します
        /// </summary>
        public void Dispose()
        {
            queHeader.Dispose();
        }

        #region ENQUEUE

        /// <summary>
        /// キューへ指定オブジェクトをエントリーします
        /// 規定のTクラス型データ要素を対象
        /// </summary>
        /// <param name="data">エントリーするオブジェクト</param>
        /// <returns>キューがいっぱいの場合 false を返します</returns>
        public bool Enqueue(T data)
        {
            return Enqueue(data, -1);
        }

        /// <summary>
        /// キューへ指定オブジェクトをエントリーします
        /// 規定のクラス型から派生した任意のUクラス型データ要素を対象
        /// </summary>
        /// <param name="data">エントリーするオブジェクト</param>
        /// <param name="enque_timeout_milliseconds">キューがいっぱいの場合、エントリーできるまで待機する時間(ms)</param>
        /// <returns>キューがいっぱいの場合 false を返します</returns>
        public bool Enqueue(T data, int enque_timeout_milliseconds)
        {
            return queHeader.Enqueue(data, enque_timeout_milliseconds);
        }

        #endregion

        #region DEQUEUE

        /// <summary>
        /// キューの先頭から１件取得して、キューから削除します
        /// 規定のTクラス型データ要素を対象
        /// </summary>
        /// <returns>データが存在しない場合や、異なるクラス型の場合はnullを返します</returns>
        public T Dequeue()
        {
            return Dequeue(0);
        }

        /// <summary>
        /// キューの先頭から１件取得して、キューから削除します
        /// 規定のTクラス型データ要素を対象
        /// </summary>
        /// <param name="deque_timeout_milliseconds">キューが空の場合、エントリーされるまで待機する時間(ms)</param>
        /// <returns>データが存在しない場合や、異なるクラス型の場合はnullを返します</returns>
        public T Dequeue(int deque_timeout_milliseconds)
        {
            return queHeader.Dequeue(deque_timeout_milliseconds, false);
        }

        #endregion

        #region PEEK

        /// <summary>
        /// キューの先頭から１件取得しますが、キューからは削除しません
        /// 規定のTクラス型データ要素を対象
        /// </summary>
        /// <returns>データが存在しない場合や、異なるクラス型の場合はnullを返します</returns>
        public T Peek()
        {
            return Peek(0);
        }

        /// <summary>
        /// キューの先頭から１件取得しますが、キューからは削除しません
        /// 規定のTクラス型データ要素を対象
        /// </summary>
        /// <param name="deque_timeout_milliseconds">キューが空の場合、エントリーされるまで待機する時間(ms)</param>
        /// <returns>データが存在しない場合や、異なるクラス型の場合はnullを返します</returns>
        public T Peek(int deque_timeout_milliseconds)
        {
            return queHeader.Dequeue(deque_timeout_milliseconds, true);
        }

        #endregion

        /// <summary>
        /// キューにエントリーされているデータ数を取得します
        /// </summary>
        public int Count { get { return (int)queHeader.Count; } }

        /// <summary>
        /// キューにエントリーされているデータ数を取得します
        /// </summary>
        public int Size { get { return Count; } }
    }
}
