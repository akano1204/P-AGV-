using System.Threading;

namespace BelicsClass.ProcessManage
{
    /// <summary>
    /// 排他制御クラス(１プロセス内)
    /// </summary>
    public class BL_Monitor
    {
        object sync = new object();

        /// <summary>
        /// 
        /// </summary>
        public void Lock()
        {
            Monitor.Enter(sync);
        }

        /// <summary>
        /// 
        /// </summary>
        public bool Lock(int timeout)
        {
            return Monitor.TryEnter(sync, timeout);
        }

        /// <summary>
        /// 
        /// </summary>
        public void Unlock()
        {
            Monitor.Exit(sync);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsLock()
        {
            if (Monitor.TryEnter(sync, 0))
            {
                Monitor.Exit(sync);
                return false;
            }
            return true;
        }
    }
}
