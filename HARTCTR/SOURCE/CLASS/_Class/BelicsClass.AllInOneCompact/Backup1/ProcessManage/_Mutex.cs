using System;
using System.Threading;
using System.Security.AccessControl;

namespace BelicsClass.ProcessManage
{
    /// <summary>
    /// 排他制御クラス（複数プロセス間）
    /// </summary>
    public class BL_Mutex : IDisposable
    {
        //private bool locked = false;
        private Mutex mutex = null;

        /// <summary>
        /// コンストラクタ
        /// Mutexインスタンスを名前なしで初期化します
        /// オープンされます
        /// オープンエラーで例外が発生します
        /// </summary>
        public BL_Mutex()
        {
            string err = Open("");
            if (err != "") throw new Exception(err);
        }

        /// <summary>
        /// コンストラクタ
        /// Mutexインスタンスを名前付きで初期化します
        /// オープンされます
        /// オープンエラーで例外が発生します
        /// </summary>
        /// <param name="name">名前</param>
        public BL_Mutex(string name)
        {
            string err = Open(name);
            if (err != "") throw new Exception(err);
        }

        /// <summary>
        /// Mutexインスタンスを破棄します
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Mutexインスタンスを破棄します
        /// </summary>
        /// <param name="flag">未使用</param>
        protected virtual void Dispose(bool flag)
        {
            Close();
        }

        /// <summary>
        /// 指定された名称のMutexをオープンします
        /// </summary>
        /// <param name="name">名称</param>
        /// <returns>エラーメッセージを返します</returns>
        public string Open(string name)
        {
            bool doesNotExist = false;
            bool unauthorized = false;

            bool mutexWasCreated = false;

            try
            {
                mutex = Mutex.OpenExisting(name);
            }
            catch (WaitHandleCannotBeOpenedException)
            {
                doesNotExist = true;
            }
            catch (UnauthorizedAccessException)
            {
                unauthorized = true;
            }

            if (doesNotExist)
            {
                string user = Environment.UserDomainName + "\\" + Environment.UserName;
                MutexSecurity mSec = new MutexSecurity();
                MutexAccessRule rule = new MutexAccessRule(user, MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Deny);
                mSec.AddAccessRule(rule);

                rule = new MutexAccessRule(user, MutexRights.ReadPermissions | MutexRights.ChangePermissions, AccessControlType.Allow);
                mSec.AddAccessRule(rule);

                mutex = new Mutex(false, name, out mutexWasCreated, mSec);

                if (!mutexWasCreated)
                {
                    return "[Create mutex failed]";
                }
            }
            else if (unauthorized)
            {
                try
                {
                    mutex = Mutex.OpenExisting(name, MutexRights.ReadPermissions | MutexRights.ChangePermissions);

                    string user = Environment.UserDomainName + "\\" + Environment.UserName;
                    MutexSecurity mSec = mutex.GetAccessControl();
                    MutexAccessRule rule = new MutexAccessRule(user, MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Deny);
                    mSec.RemoveAccessRule(rule);

                    rule = new MutexAccessRule(user, MutexRights.Synchronize | MutexRights.Modify, AccessControlType.Allow);
                    mSec.AddAccessRule(rule);

                    mutex.SetAccessControl(mSec);

                    mutex = Mutex.OpenExisting(name);
                }
                catch (UnauthorizedAccessException ex)
                {
                    return "[" + ex.Message + "]";
                }
            }

            return "";
        }

        /// <summary>
        /// Mutexをクローズします
        /// ロックされている場合、アンロックされます。
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (mutex != null)
            {
                //if (IsLocked) Unlock();
                Unlock();
                mutex.Close();
                mutex = null;
                return true;
            }

            return false;
        }

        /// <summary>
        /// ロックします
        /// ロックができるまでブロッキングします
        /// </summary>
        /// <returns>常にtrueを返します</returns>
        public bool Lock()
        {
            return Lock(Timeout.Infinite);
        }

        /// <summary>
        /// ロックします
        /// 指定時間だけブロッキングします
        /// 指定時間内にロックできない場合、タイムアウトします
        /// </summary>
        /// <param name="millisecondsTimeout">指定時間msec</param>
        /// <returns>ロックが成功するとtrueを返します</returns>
        public bool Lock(int millisecondsTimeout)
        {
            //if (!locked)
            //{
                //try
                //{
                    if (mutex.WaitOne(millisecondsTimeout, false))
                    {
                        //locked = true;
                        return true;
                    }
                //}
                //catch { }

                //locked = false;
            //}
            //else
            //{
            //}
            
            return false;
        }

        /// <summary>
        /// アンロックします
        /// ロックされていなくても例外を発生しません
        /// </summary>
        /// <returns>アンロックが行われるとtrueを返します。ロックされていない場合falseを返します。</returns>
        public bool Unlock()
        {
            //if (locked)
            {
                try
                {
                    //locked = false;
                    mutex.ReleaseMutex();

                    return true;
                }
                catch { }
            }
            //else
            //{
            //}

            return false;
        }

        ///// <summary>
        ///// ロック状態を取得します(設定はできません)
        ///// </summary>
        //public bool IsLocked
        //{
        //    get
        //    {
        //        return locked;
        //    }
        //}
    }
}
