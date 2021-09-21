//#define DUPLICATE_LOG

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.CompilerServices;
using System.Reflection;

using BelicsClass.Common;
using BelicsClass.File;
using BelicsClass.UI.Controls;

namespace BelicsClass.ProcessManage
{
    /// <summary>
    /// スレッド制御の基本クラスです。
    /// </summary>
    public class BL_ThreadController_Base
    {
        #region スレッドイベント

        /// <summary>スレッドの状態</summary>
        public enum ThreadState : int
        {
            /// <summary>停止</summary>
            Stop = 0,
            /// <summary>実行中</summary>
            Run = 1,
            /// <summary>一時停止</summary>
            Suspend = 2,
            ///// <summary>再開</summary>
            //Resume = 3,
        }

        /// <summary>
        /// スレッド実行状態変化イベント定義
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="state"></param>
        public delegate void Event_Handler_ChangeThreadState(BL_ThreadController_Base sender, ThreadState state);

        /// <summary>
        /// スレッド実行状態変化イベント
        /// </summary>
        public virtual event Event_Handler_ChangeThreadState EventChangeThreadState;

        #endregion

        /// <summary>
        /// スレッド内のログ１件を管理します
        /// </summary>
        public class BL_Histories
        {
            /// <summary>発生日時</summary>
            public DateTime timestamp = DateTime.Now;
            /// <summary>名前</summary>
            public string name = "";
            /// <summary>呼出元</summary>
            public string callfrom = "";
            /// <summary>内容</summary>
            public string description = "";
            /// <summary>ログレベル</summary>
            public int level = 0;

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public BL_Histories() { }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="name">名前</param>
            /// <param name="callfrom">呼出元</param>
            /// <param name="description">内容</param>
            /// <param name="level">レベル</param>
            public BL_Histories(string name, string callfrom, string description, int level)
            {
                if (callfrom == "")
                {
                    string trace = Environment.StackTrace;
                    string[] traces = trace.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                    foreach (string t in traces)
                    {
                        if (t.IndexOf("   場所 System.") == 0) continue;
                        if (t.IndexOf("   場所 BelicsClass.") == 0) continue;

                        callfrom = t;
                        break;
                    }

                    if (callfrom != "")
                    {
                        traces = callfrom.Split('(');
                        if (0 < traces.Length) callfrom = traces[0].Replace("   場所 ", "");

                        traces = callfrom.Split('.');
                        if (1 < traces.Length)
                        {
                            callfrom = "";
                            for (int i = 1; i < traces.Length; i++)
                            {
                                if (callfrom != "") callfrom += ".";
                                callfrom += traces[i];
                            }
                        }
                    }
                }

                this.timestamp = DateTime.Now;
                this.name = name;
                this.callfrom = callfrom;
                this.description = description;
                this.level = level;
            }
        }

        /// <summary>排他制御クラスインスタンス</summary>
        protected BL_Monitor m_Sync = new BL_Monitor();

        /// <summary>スレッド識別ID</summary>
        protected object m_ID = null;
        /// <summary>スレッド行程</summary>
        protected int m_Step = 0;
        /// <summary>リトライ回数</summary>
        protected int m_Retry = 0;
        /// <summary>汎用計測用</summary>
        protected BL_Stopwatch m_swTemp = new BL_Stopwatch();
        /// <summary>経過時間計測タイマー</summary>
        protected long m_Ticks = 0;
        /// <summary>スレッド処理回数/s</summary>
        protected long m_lPassedTimes = 0;
        /// <summary>スレッド処理回数</summary>
        protected long m_lPassedCount_Temp = 0;
        /// <summary>スレッド処理回数</summary>
        protected long m_lPassedCount = 0;

        /// <summary>スリープ時間</summary>
        protected int m_Sleep = 10;

        /// <summary>スレッド終了可不可</summary>
        protected bool m_bCanExit = true;

        /// <summary>スレッド処理中</summary>
        protected bool m_bDoing = false;

        /// <summary>ログキュー</summary>
        protected Queue<BL_Histories> m_queLog = new Queue<BL_Histories>();

        /// <summary>ログ書き込みクラスのインスタンス</summary>
        protected BL_Log m_Log = null;

        /// <summary>
        /// ログ表示のためのリストビューへの参照
        /// 表示の必要が無ければnull
        /// </summary>
        protected BL_VirtualListView m_listView = null;

        private long log_refresh_watch = 0;

        /// <summary>
        /// フォーム操作のための参照
        /// 操作の必要が無ければnull
        /// </summary>
        protected Form owner = null;

        /// <summary>
        /// スレッド終了可不可
        /// </summary>
        public bool CanFinish { get { return m_bCanExit; } }
        /// <summary>
        /// スレッド動作中
        /// </summary>
        public bool IsDoing { get { return doing_pre; } }
        /// <summary>
        /// スレッド回転数/s
        /// </summary>
        public long PassedCount { get { return m_lPassedCount; } }

        /// <summary>
        /// スレッドスキャンタイム
        /// </summary>
        public double ScanTime = 0;
        //public double ScanTime { get { return 1.0 / (double)m_lPassedCount; } }

        /// <summary>
        /// スレッド工程ステップ
        /// </summary>
        public int Step { get { return m_Step; } }
        /// <summary>
        /// リトライ回数
        /// </summary>
        public int Retry { get { return m_Retry; } }

        /// <summary>
        /// ステップ初期化
        /// </summary>
        public void ResetStep() { m_Step = 0; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (m_ID == null) return this.ToString();
            if (m_ID.GetType() == typeof(object)) return this.ToString();
            return m_ID.ToString();

            //return "\tStep:" + Step.ToString() + "/Rps:" + PassedCount.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public virtual string GetInformation()
        {
            return ToString() + ",Step:" + Step.ToString() + ",R/S:" + PassedCount.ToString();
        }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_ThreadController_Base()
        {
            m_ID = this.GetType().Name;
        }

        public BL_ThreadController_Base(bool log)
            :base()
        {
            m_ID = this.GetType().Name;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">自スレッド識別ID</param>
        public BL_ThreadController_Base(object id)
        {
            if (id != null) m_ID = id;
            else
            {
                m_ID = new object();
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">自スレッド識別ID</param>
        /// <param name="log">ログを表示するためのリストビュー</param>
        public BL_ThreadController_Base(object id, BL_VirtualListView log)
            : this(id)
        {
            m_listView = log;

            if (m_listView != null)
            {
                Control parent = m_listView.Parent;
                while (!typeof(Form).IsInstanceOfType(parent))
                {
                    if (parent == null) break;
                    parent = parent.Parent;
                }

                if (typeof(Form).IsInstanceOfType(parent))
                {
                    owner = (Form)parent;
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="id">自スレッド識別ID</param>
        /// <param name="parent">スレッド内から操作を行う対象となるフォーム</param>
        public BL_ThreadController_Base(object id, Form parent)
            : this(id)
        {
            owner = parent;
        }

        /// <summary>デストラクタ</summary>
        ~BL_ThreadController_Base()
        {
        }

        /// <summary>
        /// スレッド関連インスタンス
        /// </summary>
        [NonSerialized]
        protected Thread m_threadControl = null;

        /// <summary>
        /// 自己終了フラグ
        /// </summary>
        protected bool m_bStop = false;

        /// <summary>
        /// スレッドを一時停止するか否かを保持します
        /// </summary>
        protected bool m_bSuspend = false;

		/// <summary>
		/// スレッドの一時停止状態を取得します
		/// </summary>
		public bool IsSuspend { get { return suspend_pre; } }

        /// <summary>
        /// スレッドのスキャン周期を取得または設定します
        /// </summary>
        public int Interval { get { return m_Sleep; } set { m_Sleep = value; } }

        /// <summary>
        /// ログを追加します。発生日時は自動付加します。
        /// </summary>
        /// <param name="description">ログの内容</param>
        /// <returns>ログ書き込みに要した時間(ms)を返します。</returns>
        public long Log(string description)
        {
            return Log(description, 0);
        }

        private int prev_level = -1;
        private string prev_description = "";
        private int same_log_count = 0;
        private string prev_callfrom = "";

        /// <summary>
        /// ログを追加します。発生日時は自動付加します。
        /// 同一内容のログは書き込みません。
        /// 内容変化時に、直前までの同一内容のログ発生回数を書き込みます。
        /// </summary>
        /// <param name="description">ログの内容</param>
        /// <param name="level">見出し行とするための[.]を付加する個数</param>
        /// <returns>ログ書き込みに要した時間(ms)を返します。</returns>
        public long Log(string description, int level)
        {
            string callfrom = "";
            string trace = Environment.StackTrace;
            string[] traces = trace.Split(new string[] { "\r\n" }, StringSplitOptions.None);
            foreach (string t in traces)
            {
                if (0 <= t.IndexOf("_Base.Log(")) continue;
                if (t.IndexOf("   場所 BelicsClass.UI") != 0)
                {
                    if (t.IndexOf("   場所 System.") == 0) continue;
                    if (t.IndexOf("   場所 BelicsClass.") == 0) continue;
                }

                callfrom = t;
                break;
            }

            if (callfrom != "")
            {
                traces = callfrom.Split('(');
                if (0 < traces.Length) callfrom = traces[0].Replace("   場所 ", "").Replace("BelicsClass.", "");

                traces = callfrom.Split('.');
                if (1 < traces.Length)
                {
                    callfrom = "";
                    for (int i = 1; i < traces.Length; i++)
                    {
                        if (callfrom != "") callfrom += ".";
                        callfrom += traces[i];
                    }
                }
            }

            if (callfrom == "Program.Main") return 0;

            long ret = 0;
            try
            {
                description = description.Replace("\n", "/n");

#if DUPLICATE_LOG
                if (description != prev_description || level != prev_level || callfrom != prev_callfrom)
                {
#endif
                    if (0 < same_log_count)
                    {
                        string log = prev_callfrom + " - " + prev_description + "(REPEAT " + same_log_count.ToString() + " TIMES)";
                        m_queLog.Enqueue(new BL_Histories(this.ToString(), prev_callfrom, prev_description + "(REPEAT " + same_log_count.ToString() + " TIMES)", prev_level));
                        if (m_Log != null) m_Log.Add(log, prev_level);
                    }

                    m_queLog.Enqueue(new BL_Histories(this.ToString(), callfrom, description, level));
                    if (m_Log != null)
                    {
                        ret = m_Log.Add(callfrom + " - " + description, level);
                    }

                    same_log_count = 0;
                    prev_description = description;
                    prev_level = level;
                    prev_callfrom = callfrom;

#if DUPLICATE_LOG
                }
                else
                {
                    same_log_count++;
                }
#endif
                while (1000 < m_queLog.Count) m_queLog.Dequeue();
            }
            catch { }

            return ret;
        }

        /// <summary>
        /// ログから1件取得します。
        /// </summary>
        /// <returns>ログが存在しない場合、nullを返します。</returns>
        public BL_Histories LogGet()
        {
            if (0 < m_queLog.Count) return m_queLog.Dequeue();
            return null;
        }

        #region 制御開始

        /// <summary>
        /// 制御開始
        /// </summary>
        /// <param name="sleep">Sleep時間(ms)</param>
        /// <returns></returns>
        public string StartControl(int sleep)
        {
            return StartControl(sleep, ThreadPriority.Normal);
        }

        /// <summary>
        /// 制御開始
        /// </summary>
        /// <param name="sleep">Sleep時間(ms)</param>
        /// <param name="priority">スレッド優先順位</param>
        /// <returns></returns>
        virtual public string StartControl(int sleep, ThreadPriority priority)
        {
            m_Sleep = sleep;

            if (m_threadControl != null)
            {
                m_bStop = true;
                if (m_bDoing) m_threadControl.Join();
                m_threadControl = null;
            }

            //スレッドの起動
            m_threadControl = new Thread(new ThreadStart(WorkerThread));
            m_threadControl.Name = this.ToString();
            m_threadControl.Priority = priority;
            m_threadControl.Start();
            while (!m_threadControl.IsAlive) ;

            m_bDoing = true;

            BL_ThreadCollector.Add(this);

            m_Log = new BL_Log("", m_ID.ToString());

            //m_Log.Add("---<START>---");

            return "";
        }

        #endregion

        #region 制御終了

        /// <summary>
        /// 制御終了
        /// </summary>
        virtual public void StopControl()
        {
            try
            {
                if (0 < same_log_count)
                {
                    string log = prev_callfrom + " - " + prev_description + "(REPEAT " + same_log_count.ToString() + " TIMES)";
                    m_queLog.Enqueue(new BL_Histories(this.ToString(), prev_callfrom, prev_description + "(REPEAT " + same_log_count.ToString() + " TIMES)", prev_level));
                    if (m_Log != null) m_Log.Add(log, prev_level);
                }

                if (m_threadControl != null)
                {
                    m_bStop = true;
                    if (m_bDoing)
                    {
                        if (!m_threadControl.Join(1000))
                        {
                            m_threadControl.Abort();
                        }
                    }
                    m_threadControl = null;
                }

                m_bDoing = false;

                BL_ThreadCollector.Remove(this);

                if (m_Log != null)
                {
                    m_Log.Dispose();
                    m_Log = null;
                }
            }
            catch { }
        }

        #endregion

        /// <summary>
        /// スレッド内の処理を一時停止／再開
        /// </summary>
        /// <param name="suspend"></param>
        public void SuspendControl(bool suspend)
        {
            m_bSuspend = suspend;
        }

        #region ワーカースレッド

        private bool suspend_pre = false;
        private bool doing_pre = false;

		/// <summary>
		/// スレッドをサスペンドします。
		/// サスペンド状態になるまで待機します。
		/// </summary>
		public void SuspendWait()
		{
			m_bSuspend = true;
			suspend_pre = m_bSuspend;

			if (IsDoing)
			{
				while (true)
				{
					if (IsSuspend) break;
					BL_Win32API.Sleep(10);
				}
			}
		}

		/// <summary>
		/// スレッドをサスペンドから復帰します。
		/// 復帰状態になるまで待機します。
		/// </summary>
		public void ResumeWait()
		{
			m_bSuspend = false;
			suspend_pre = m_bSuspend;

			if (IsDoing)
			{
				while (true)
				{
					if (!IsSuspend) break;
					BL_Win32API.Sleep(10);
				}
			}
		}

        BL_Stopwatch sw_scancycle = new BL_Stopwatch();

		/// <summary>
		/// ワーカースレッド
		/// </summary>
		virtual protected void WorkerThread()
        {
            m_bStop = false;

#if ENABLE_TRYCATCH
            try
            {
#endif
            BL_HRTimer timer = new BL_HRTimer();
            BL_HRTimer scan = new BL_HRTimer();

            //m_Step = 0;

            scan.Restart();

            while (true)
            {
                //if (sw_scancycle.IsRunning && m_Sleep * 100 <= sw_scancycle.ElapsedMilliseconds)
                if (sw_scancycle.IsRunning && 5000 <= sw_scancycle.ElapsedMilliseconds)
                {
                    sw_scancycle.Stop();
                    //Log("THREAD DELAY..." + sw_scancycle.ElapsedMilliseconds.ToString() + "ms >= " + (m_Sleep*10).ToString() + "ms");
                    Log("THREAD DELAY..." + sw_scancycle.ElapsedMilliseconds.ToString() + "ms >= 5000ms");
                }
                sw_scancycle.Restart();

                timer.Restart();

                m_bDoing = true;

                if (doing_pre != m_bDoing)
                {
                    doing_pre = m_bDoing;

                    #region EventChangeThreadState
                    if (EventChangeThreadState != null)
                    {
                        if (owner != null)
                        {
                            if (owner.IsHandleCreated)
                            {
                                MethodInvoker process = (MethodInvoker)delegate()
                                {
                                    if (m_bSuspend) EventChangeThreadState(this, ThreadState.Suspend);
                                    if (!m_bSuspend) EventChangeThreadState(this, ThreadState.Run);
                                };

                                try
                                {
                                    if (owner.InvokeRequired) owner.Invoke(process);
                                    else process.Invoke();
                                }
                                catch (ObjectDisposedException) { }
                                catch (InvalidOperationException) { }
                            }
                            else
                            {
                                if (m_bSuspend) EventChangeThreadState(this, ThreadState.Suspend);
                                if (!m_bSuspend) EventChangeThreadState(this, ThreadState.Run);
                            }
                        }
                        else
                        {
                            if (m_bSuspend) EventChangeThreadState(this, ThreadState.Suspend);
                            if (!m_bSuspend) EventChangeThreadState(this, ThreadState.Run);
                        }
                    }
                    #endregion
                }

                if (suspend_pre != m_bSuspend)
                {
                    suspend_pre = m_bSuspend;

                    #region EventChangeThreadState
                    if (EventChangeThreadState != null)
                    {
                        if (owner != null)
                        {
                            if (owner.IsHandleCreated)
                            {
                                MethodInvoker process = (MethodInvoker)delegate()
                                {
                                    if (m_bSuspend) EventChangeThreadState(this, ThreadState.Suspend);
                                    if (!m_bSuspend) EventChangeThreadState(this, ThreadState.Run);
                                };

                                try
                                {
                                    if (owner.InvokeRequired) owner.Invoke(process);
                                    else process.Invoke();
                                }
                                catch (ObjectDisposedException) { }
                                catch (InvalidOperationException) { }
                            }
                            else
                            {
                                if (m_bSuspend) EventChangeThreadState(this, ThreadState.Suspend);
                                if (!m_bSuspend) EventChangeThreadState(this, ThreadState.Run);
                            }
                        }
                        else
                        {
                            if (m_bSuspend) EventChangeThreadState(this, ThreadState.Suspend);
                            if (!m_bSuspend) EventChangeThreadState(this, ThreadState.Run);
                        }
                    }
					#endregion
                }

                if (m_bCanExit && m_bStop) break;

                if (m_Ticks == 0)
                {
                    m_Ticks = scan.ElapsedMilliseconds;
                }
                else if (1000 <= scan.ElapsedMilliseconds)
                {
                    m_Ticks = scan.ElapsedMilliseconds;

                    m_lPassedCount = (long)((double)m_lPassedCount_Temp / ((double)m_Ticks / 1000));
                    m_lPassedCount_Temp = -1;

                    scan.Restart();
                }

                if (!m_bSuspend || !m_bCanExit)
                {
                    lock (m_ID)
                    {
                        //スレッド処理メソッドの実施
                        object message = new object();

                        if (owner != null)
                        {
                            if (owner.IsHandleCreated)
                            {
                                MethodInvoker process = (MethodInvoker)delegate()
                                {
                                    m_bCanExit = DoControl(message);
                                };

                                try
                                {
                                    if (owner.InvokeRequired) owner.Invoke(process);
                                    else process.Invoke();
                                }
                                //catch (ObjectDisposedException) { }
                                //catch (InvalidOperationException) { }
                                catch (Exception) { }
                            }
                            else m_bCanExit = DoControl(message);
                        }
                        else m_bCanExit = DoControl(message);

                        m_lPassedCount_Temp++;
                    }
                }

                if (log_refresh_watch == 0 || 200 < BL_Win32API.TickCount - log_refresh_watch)
                {
                    log_refresh_watch = BL_Win32API.TickCount;

                    LogPeek();
                }

                long now = timer.ElapsedMilliseconds;
                if (1 < m_Sleep - now)
                {
                    Thread.Sleep((int)(m_Sleep - now) - 1);
                }
                else
                {
                    Thread.Sleep(0);
                }

                ScanTime = timer.ElapsedMilliseconds;
            }

#if ENABLE_TRYCATCH
            }
            catch (Exception ex)
            {
                Exception inner_ex = ex;
                string message = "<" + ex.Message + ">";
                while (inner_ex.InnerException != null)
                {
                    message += "<" + inner_ex.InnerException.Message + ">";
                    inner_ex = inner_ex.InnerException;
                }
            }
#endif
            BL_ThreadCollector.Remove(this);
            m_threadControl = null;
            m_bDoing = false;
            m_Step = -1;

            #region EventChangeThreadState
            if (EventChangeThreadState != null)
            {
                if (owner != null)
                {
                    if (owner.IsHandleCreated)
                    {
                        MethodInvoker process = (MethodInvoker)delegate()
                        {
                            EventChangeThreadState(this, ThreadState.Stop);
                        };

                        try
                        {
                            if (owner.InvokeRequired) owner.Invoke(process);
                            else process.Invoke();
                        }
                        catch (ObjectDisposedException) { }
                        catch (InvalidOperationException) { }
                    }
                    else
                    {
                        EventChangeThreadState(this, ThreadState.Stop);
                    }
                }
                else
                {
                    EventChangeThreadState(this, ThreadState.Stop);
                }
            }
            #endregion
        }

        private void LogPeek()
        {
            if (m_listView != null && 0 < m_queLog.Count)
            {
                if (owner != null)
                {
                    if (owner.IsHandleCreated)
                    {
                        MethodInvoker process = (MethodInvoker)delegate()
                        {
                            int addcount = 0;
                            SortedList<string, BL_VirtualListView.BL_VirtualListViewItem> list = new SortedList<string, BL_VirtualListView.BL_VirtualListViewItem>();

                            while (0 < m_queLog.Count)
                            {
                                BL_Histories log = m_queLog.Dequeue();

                                if (m_listView.Columns.Count != 5)
                                {
                                    m_listView.Columns.Clear();
                                    m_listView.Columns.Add("level", 0);
                                    m_listView.Columns.Add("TimeStamp", 200);
                                    m_listView.Columns.Add("Name", 200);
                                    m_listView.Columns.Add("Caller", 300);
                                    m_listView.Columns.Add("Description", 2000);
                                }

                                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                                item.Tag = log;
                                item.Add(log.level);
                                item.Add(log.timestamp.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                                item.Add(log.name);
                                item.Add(log.callfrom);
                                item.Add(log.description);

                                list.Add(log.timestamp.ToString("yyyy/MM/dd HH:mm:ss.fff - ") + addcount.ToString(), item);
                                addcount++;
                            }

                            if (0 < addcount)
                            {
                                bool automove = false;
                                if (0 == m_listView.SelectedItems.Count) automove = true;

                                m_listView.Items.AddLimit(list, 1000);
                                m_listView.RefreshMe(false);

                                if (automove) m_listView.EnsureVisible(m_listView.Items.Count - 1);
                            }
                        };

                        if (owner.InvokeRequired) owner.BeginInvoke(process);
                        else process.Invoke();
                    }
                }
            }

            //取り出されないログの読み捨て(1000件以前)
            while (1000 < m_queLog.Count)
            {
                m_queLog.Dequeue();
            }
        }

        #endregion

        #region スレッド処理

        /// <summary>
        /// スレッド処理
        /// bool canexit = 終了可能状態;
        /// return canexit;
        /// </summary>
        /// <param name="message"></param>
        /// <returns>終了可の場合Trueを返す</returns>
        virtual protected bool DoControl(object message)
        {
            return true;
        }

        #endregion

        /// <summary>
        /// スリープします
        /// </summary>
        /// <param name="millisecondsTimeout"></param>
        public void Sleep(int millisecondsTimeout)
        {
            Thread.Sleep(millisecondsTimeout);
        }
    }

    /// <summary>
    /// スレッド処理の全コレクター
    /// </summary>
    public class BL_ThreadCollector
    {
        /// <summary>
        /// スレッド処理のインスタンス参照を保持します。
        /// </summary>
        static protected List<BL_ThreadController_Base> m_GlobalThreadCollection = new List<BL_ThreadController_Base>();

        /// <summary>
        /// スレッドをコレクションに追加します。
        /// </summary>
        /// <param name="controller"></param>
        static public void Add(BL_ThreadController_Base controller)
        {
            lock (m_GlobalThreadCollection)
            {
                if (m_GlobalThreadCollection.IndexOf(controller) < 0)
                {
                    m_GlobalThreadCollection.Add(controller);
                }
            }
        }

        /// <summary>
        /// コレクションからスレッドを削除します。
        /// </summary>
        /// <param name="controller"></param>
        static public void Remove(BL_ThreadController_Base controller)
        {
            lock (m_GlobalThreadCollection)
            {
                m_GlobalThreadCollection.Remove(controller);
            }
        }

        /// <summary>
        /// コレクションのスレッド数を取得します。
        /// </summary>
        static public int Count
        {
            get { return m_GlobalThreadCollection.Count; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        static public BL_ThreadController_Base Get(int index)
        {
            return m_GlobalThreadCollection[index];
        }

        /// <summary>
        /// 
        /// </summary>
        static public void Clear()
        {
            m_GlobalThreadCollection.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        static public void StopControl_All()
        {
            while (0 < m_GlobalThreadCollection.Count)
            {
                m_GlobalThreadCollection[0].StopControl();
            }
        }
    }
}
