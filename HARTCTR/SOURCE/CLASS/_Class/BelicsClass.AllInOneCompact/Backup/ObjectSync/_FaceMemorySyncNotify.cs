using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using BelicsClass.ProcessManage;

namespace BelicsClass.ObjectSync
{
    /// <summary>
    /// クラスインスタンスを共有メモリでアクセスを行うクラス
    /// 共有メモリの変化を検出し、イベントを発生させます
    /// 共有メモリを利用しない場合、イベントは発生しません
    /// 
    /// 本クラスから派生して、共有アクセスを行いたいフィールドが定義されたクラスを設計してください
    /// フィールドは本クラスのInitializeメソッドコールによって解析され、バイト配列とオブジェクト間の相互変換が行われます
    /// null値はフィールドの初期値として使用できません
    /// stringフィールドは、必要文字数分の空白で初期化してください
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
    public class BL_FaceMemorySyncNotify : BL_FaceMemorySync
    {
        /// <summary></summary>
        public class threadNotify : BL_ThreadController_Base 
        {
            /// <summary></summary>
            public delegate void Event_Handler_Modified(string[] field_names);
            /// <summary></summary>
            public virtual event Event_Handler_Modified EventModified;

            /// <summary></summary>
            public delegate void Event_Handler_ModifiedObject(BL_ObjectInformation[] fields);
            /// <summary></summary>
            public virtual event Event_Handler_ModifiedObject EventModifiedObject;

            /// <summary></summary>
            public BL_FaceMemorySyncNotify source = null;

            private BL_FaceMemorySync target = null;

            /// <summary></summary>
            public threadNotify(BL_FaceMemorySyncNotify source, Form owner)
                : base(source)
            {
                this.source = source;
                this.owner = owner;
            }

            /// <summary></summary>
            public override string StartControl(int sleep, System.Threading.ThreadPriority priority)
            {
                throw new NotSupportedException("StartControl<T>(...)を使用してください");
            }

            /// <summary></summary>
            public virtual string StartControl<T>(int sleep, System.Threading.ThreadPriority priority) where T : new()
            {
                if (!typeof(BL_FaceMemorySync).IsInstanceOfType(source))
                {
                    throw new Exception("[" + source.GetType().Name + "] はBL_FaceMemorySync型から派生されたデータ型が必要です。");
                }

                object newobj = new T();
                target = (BL_FaceMemorySync)newobj;
                target.Initialize(source.SharememName);

                return base.StartControl(sleep, priority);
            }

            /// <summary></summary>
            protected override bool DoControl(object message)
            {
                //try
                {

                    if (EventModified != null || EventModifiedObject != null)
                    {
                        BL_ObjectInformation[] list = target.IsModified();

                        if (EventModifiedObject != null)
                        {
                            if (0 < list.Length) EventModifiedObject(list);
                        }

                        if (EventModified != null)
                        {
                            string[] fields = new string[list.Length];
                            for (int i = 0; i < list.Length; i++)
                            {
                                fields[i] = list[i].Key;
                            }
                            if (0 < fields.Length) EventModified(fields);
                        }
                    }
                }
                //catch { }

                return base.DoControl(message);
            }
        }

        private bool enableNotify = false;
        private int sleep_milliseconds = 100;

        private Form owner = null;
        /// <summary>
        /// イベント通知先の事前登録
        /// 登録しておくと、Invokeされます
        /// </summary>
        public Form Owner { get { return owner; } set { owner = value; } }

        /// <summary>
        /// イベントの有効状態を取得します
        /// </summary>
        public bool EnableNotify { get { return enableNotify; } }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sleep_milliseconds"></param>
        /// <param name="owner"></param>
        public void StartNotify<T>(int sleep_milliseconds, Form owner) where T : new()
        {
            if (thread == null && SharememName != "")
            {
                enableNotify = true;

                thread = new threadNotify(this, owner);
                thread.EventModified += thread_EventModified;
                thread.StartControl<T>(sleep_milliseconds, System.Threading.ThreadPriority.Normal);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void StopNotify()
        {
            enableNotify = false;

            if (thread != null)
            {
                thread.SuspendControl(true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResumeNotify()
        {
            enableNotify = true;

            if (thread != null)
            {
                thread.SuspendControl(false);
            }
        }

        private threadNotify thread = null;

        /// <summary>
        /// 共有メモリ変化イベントデリゲート
        /// </summary>
        /// <param name="sender">イベント発生源</param>
        /// <param name="field_names">変化フィールド名</param>
        public delegate void Event_Handler_Modified(BL_FaceMemorySyncNotify sender, string[] field_names);

        /// <summary>
        /// 共有メモリ変化イベント
        /// </summary>
        public virtual event Event_Handler_Modified EventModified;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_FaceMemorySyncNotify() : base() { }

        /// <summary>
        /// コンストラクタ
        /// オブジェクトの破棄によって共有メモリを破棄するかしないかを指定できます
        /// </summary>
        /// <param name="notrelease">共有メモリを破棄しない場合trueを指定してください</param>
        public BL_FaceMemorySyncNotify(bool notrelease) : base(notrelease) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="owner">フォームにアクセスする必要がある場合、フォームへの参照を指定してください</param>
        public BL_FaceMemorySyncNotify(Form owner)
            : base()
        {
            this.owner = owner;
        }

        /// <summary>
        /// コンストラクタ
        /// オブジェクトの破棄によって共有メモリを破棄するかしないかを指定できます
        /// </summary>
        /// <param name="owner">フォームにアクセスする必要がある場合、フォームへの参照を指定してください</param>
        /// <param name="notrelease">共有メモリを破棄しない場合trueを指定してください</param>
        public BL_FaceMemorySyncNotify(Form owner, bool notrelease)
            : base(notrelease)
        {
            this.owner = owner;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="owner"></param>
        /// <param name="sleep_millis"></param>
        /// <param name="x"></param>
        /// <param name="sharemem_name"></param>
        /// <returns></returns>
        public virtual bool Initialize<T>(Form owner, int sleep_millis, object x, string sharemem_name) where T : new()
        {
            Owner = owner;
            return Initialize<T>(sleep_millis, x, sharemem_name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sharemem_name"></param>
        /// <returns></returns>
        public virtual bool Initialize<T>(string sharemem_name) where T : new()
        {
            return this.Initialize<T>(this, sharemem_name);
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
        public virtual bool Initialize<T>(object x, string sharemem_name) where T : new()
        {
            bool ret = base.Initialize(x, sharemem_name);

            StartNotify<T>(sleep_milliseconds, owner);

            return ret;
        }

        /// <summary>
        /// 初期化します
        /// 名前を指定することによって共有メモリが生成されます
        /// 同一名の共有メモリが既存の場合は、共有メモリからデータが読み出されて初期化されます
        /// 自オブジェクトが所持するprivate以外のフィールドを解析して、必要数分のバイト配列を生成するための準備が行われます
        /// 本メソッド実行後、フィールド名文字列によるデータ操作が可能となります
        /// 
        /// 共有メモリの監視インターバルを指定することができます
        /// 
        /// </summary>
        /// <param name="sleep_milliseconds">共有メモリ監視インターバル</param>
        /// <param name="x">自インスタンスの初期化時に反映させる初期化オブジェクト</param>
        /// <param name="sharemem_name">共有メモリの名称</param>
        /// <returns>初期化成功でtrueを返します</returns>
        public virtual bool Initialize<T>(int sleep_milliseconds, object x, string sharemem_name) where T : new()
        {
            bool ret = base.Initialize(x, sharemem_name);
            this.sleep_milliseconds = sleep_milliseconds;

            StartNotify<T>(sleep_milliseconds, owner);

            return ret;
        }

        private void thread_EventModified(string[] field_names)
        {
            try
            {
                if (owner == null)
                {
                    if (EventModified != null) EventModified(this, field_names);
                }
                else
                {
                    //オーナーフォームが指定されている場合、インボークします。
                    if (owner.IsHandleCreated)
                    {
                        MethodInvoker process = (MethodInvoker)delegate()
                        {
                            if (EventModified != null) EventModified(this, field_names);
                        };

                        if (owner.InvokeRequired) owner.Invoke(process);
                        else process.Invoke();
                    }
                    else if (EventModified != null) EventModified(this, field_names);
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        /// <summary>
        /// 自インスタンスの破棄
        /// 監視スレッドを停止します
        /// </summary>
        public override void Dispose()
        {
            if (thread != null)
            {
                thread.StopControl();
            }

            base.Dispose();
        }
    }
}
