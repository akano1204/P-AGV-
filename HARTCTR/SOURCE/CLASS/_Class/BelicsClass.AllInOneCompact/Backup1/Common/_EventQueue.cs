using System;
using System.Threading;
using System.Collections;
using System.Runtime.InteropServices;

namespace BelicsClass.Common
{
    /// <summary>
    /// EventQueueはFIFOとイベントを持ちます。
    /// イベントはFIFOにデータがある場合にシグナル状態になり、
    /// データがない場合にノンシグナル状態になります。
    /// イベントはGetWaitHandleにより取得でき、System.Threading.WaitHandleクラスの
    /// Wait*メソッドによりイベント発生を待つことが可能です。
    /// </summary>
    /// <remarks>
    /// メンバは全てスレッドセーフです。<br/>
    /// 排他状態でキューを操作したい場合は、インスタンスに対してlockを行ってください。
    /// </remarks>
    public class BL_EventQueue
    {
        /// <summary>
        /// キュー登録制限数です。
        /// これを超えてキュー登録できません。
        /// 0の場合、無制限です。
        /// </summary>
        public readonly int LimitCount;

        /// <summary>
        /// デフォルトコンストラクタです。
        /// </summary>
        public BL_EventQueue()
        {
            //イベント生成
            m_Event = new ManualResetEvent(false);

            //FIFO生成
            m_Queue = new Queue();

            //キュー登録制限数設定
            LimitCount = 0;
        }

        /// <summary>
        /// デフォルトコンストラクタです。
        /// </summary>
        public BL_EventQueue(int limitCount)
        {
            //イベント生成
            m_Event = new ManualResetEvent(false);

            //FIFO生成
            m_Queue = new Queue();

            //キュー登録制限数設定(0未満なら無制限を設定する)
            if (limitCount < 0) limitCount = 0;
            LimitCount = limitCount;
        }

        /// <summary>
        /// イベントハンドル取得を取得します。
        /// </summary>
        /// <returns>イベントハンドルを返します。</returns>
        public virtual WaitHandle GetWaitHandle()
        {
            lock (this)
            {
                return m_Event;
            }
        }

        /// <summary>
        /// キューの登録数を取得します。
        /// </summary>
        public virtual int Count
        {
            get
            {
                lock (this)
                {
                    return m_Queue.Count;
                }
            }
        }

        /// <summary>キューへ登録します。</summary>
        /// <param name="obj">登録するオブジェクトを指定します。</param>
        /// <returns>登録成功=true/登録失敗=false(キュー登録数制限)</returns>
        public virtual bool Enqueue(object obj)
        {
            lock (this)
            {
                if (LimitCount != 0 && m_Queue.Count >= LimitCount) return false;

                //FIFOに登録し、イベントをセット
                m_Queue.Enqueue(obj);
                m_Event.Set();
            }
            return true;
        }

        /// <summary>
        /// キューから読み出します。
        /// </summary>
        /// <returns>読み出したオブジェクトを返します。
        /// キューが空の場合はnullを返します。</returns>
        public virtual object Dequeue()
        {
            lock (this)
            {
                //データ無し
                if (m_Queue.Count <= 0)
                {
                    return null;
                }

                //FIFOからデキュー
                object rtnObj = m_Queue.Dequeue();

                //FIFOはカラになったらイベントをリセット
                if (m_Queue.Count <= 0)
                {
                    m_Event.Reset();
                }
                return rtnObj;
            }
        }

        /// <summary>
        /// キューをクリアします。
        /// </summary>
        public virtual void Clear()
        {
            lock (this)
            {
                //FIFOのクリアとイベントのリセット
                m_Queue.Clear();
                m_Event.Reset();
            }
        }

        /// <summary>
        /// キューにオブジェクトが登録されるまで待機します。
        /// </summary>
        /// <returns>キューにオブジェクトが登録された場合true、登録されなかった場合falseを返します。</returns>
        public virtual bool WaitOne()
        {
            return m_Event.WaitOne();
        }

        /// <summary>
        /// キューにオブジェクトが登録されるまで待機します。
        /// </summary>
        /// <param name="millisecondsTimeout">待機する時間(ミリ秒)を指定します。</param>
        /// <returns>キューにオブジェクトが登録された場合true、登録されなかった場合falseを返します。</returns>
        public virtual bool WaitOne(int millisecondsTimeout)
        {
            return m_Event.WaitOne(millisecondsTimeout, false);
        }

        /// <summary>
        /// キューにオブジェクトが登録されるまで待機します。
        /// </summary>
        /// <param name="millisecondsTimeout">待機する時間(ミリ秒)を指定します。</param>
        /// <param name="exitContext"></param>
        /// <returns>キューにオブジェクトが登録された場合true、登録されなかった場合falseを返します。</returns>
        public virtual bool WaitOne(int millisecondsTimeout, bool exitContext)
        {
            return m_Event.WaitOne(millisecondsTimeout, exitContext);
        }

        ///// <summary>
        ///// キューにオブジェクトが登録されるまで待機します。
        ///// </summary>
        ///// <param name="timeout">待機する時間を指定します。</param>
        ///// <returns>キューにオブジェクトが登録された場合true、登録されなかった場合falseを返します。</returns>
        //public virtual bool WaitOne( TimeSpan timeout )
        //{
        //    return m_Event.WaitOne( timeout, false );
        //}

        ///// <summary>
        ///// キューにオブジェクトが登録されるまで待機します。
        ///// </summary>
        ///// <param name="timeout">待機する時間を指定します。</param>
        ///// <param name="exitContext"></param>
        ///// <returns>キューにオブジェクトが登録された場合true、登録されなかった場合falseを返します。</returns>
        //public virtual bool WaitOne( TimeSpan timeout, bool exitContext )
        //{
        //    return m_Event.WaitOne( timeout, exitContext );
        //}

        /// <summary>キューにオブジェクトが登録されている場合にシグナル状態となるイベントです。</summary>
        protected ManualResetEvent m_Event;

        /// <summary>キューです。</summary>
        protected Queue m_Queue;
    }
}
