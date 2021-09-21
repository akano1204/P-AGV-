﻿using System;
using System.Threading;
using System.Security.AccessControl;

namespace BelicsClass.ProcessManage
{
    /// <summary>
    /// 排他制御クラス（複数プロセス間）
    /// </summary>
    public class BL_Semaphore
    {
        private bool locked = false;
        private Semaphore semaphore = null;

        /// <summary>
        /// コンストラクタ
        /// Semaphoreインスタンスを名前なしで初期化します
        /// オープンされます
        /// オープンエラーで例外が発生します
        /// </summary>
        public BL_Semaphore()
        {
            string err = Open("");
            if (err != "") throw new Exception(err);
        }

        /// <summary>
        /// コンストラクタ
        /// Semaphoreインスタンスを名前付きで初期化します
        /// オープンされます
        /// オープンエラーで例外が発生します
        /// </summary>
        /// <param name="name">名前</param>
        public BL_Semaphore(string name)
        {
            string err = Open(name);
            if (err != "") throw new Exception(err);
        }

        /// <summary>
        /// Semaphoreインスタンスを破棄します
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Semaphoreインスタンスを破棄します
        /// </summary>
        /// <param name="flag">未使用</param>
        protected virtual void Dispose(bool flag)
        {
            Close();
        }

        /// <summary>
        /// 指定された名称のSemaphoreをオープンします
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
                semaphore = Semaphore.OpenExisting(name);
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
                SemaphoreSecurity mSec = new SemaphoreSecurity();
                SemaphoreAccessRule rule = new SemaphoreAccessRule(user, SemaphoreRights.Synchronize | SemaphoreRights.Modify, AccessControlType.Deny);
                mSec.AddAccessRule(rule);

                rule = new SemaphoreAccessRule(user, SemaphoreRights.ReadPermissions | SemaphoreRights.ChangePermissions, AccessControlType.Allow);
                mSec.AddAccessRule(rule);

                semaphore = new Semaphore(1, 1, name, out mutexWasCreated, mSec);

                if (!mutexWasCreated)
                {
                    return "[Create semaphore failed]";
                }
            }
            else if (unauthorized)
            {
                try
                {
                    semaphore = Semaphore.OpenExisting(name, SemaphoreRights.ReadPermissions | SemaphoreRights.ChangePermissions);

                    string user = Environment.UserDomainName + "\\" + Environment.UserName;
                    SemaphoreSecurity mSec = semaphore.GetAccessControl();
                    SemaphoreAccessRule rule = new SemaphoreAccessRule(user, SemaphoreRights.Synchronize | SemaphoreRights.Modify, AccessControlType.Deny);
                    mSec.RemoveAccessRule(rule);

                    rule = new SemaphoreAccessRule(user, SemaphoreRights.Synchronize | SemaphoreRights.Modify, AccessControlType.Allow);
                    mSec.AddAccessRule(rule);

                    semaphore.SetAccessControl(mSec);

                    semaphore = Semaphore.OpenExisting(name);

                }
                catch (UnauthorizedAccessException ex)
                {
                    return "[" + ex.Message + "]";
                }
            }

            return "";
        }

        /// <summary>
        /// Semaphoreをクローズします
        /// ロックされている場合、アンロックされます。
        /// </summary>
        /// <returns></returns>
        public bool Close()
        {
            if (semaphore != null)
            {
                Unlock();
                semaphore.Close();
                semaphore = null;
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
            try
            {
                locked = true;
                if (semaphore.WaitOne(millisecondsTimeout, false))
                {
                    return true;
                }
            }
            catch { }

            locked = false;

            return false;
        }

        /// <summary>
        /// アンロックします
        /// ロックされていなくても例外を発生しません
        /// </summary>
        /// <returns>アンロックが行われるとtrueを返します。ロックされていない場合falseを返します。</returns>
        public bool Unlock()
        {
            try
            {
                semaphore.Release();
                locked = false;
                
                return true;
            }
            catch { }

            return false;
        }

        /// <summary>
        /// ロック状態を取得します(設定はできません)
        /// </summary>
        public bool IsLocked
        {
            get
            {
                return locked;
                //if (semaphore.WaitOne(0, false))
                //{
                //    semaphore.Release();
                //    return true;
                //}

                //return false;
            }
        }
    }
}
