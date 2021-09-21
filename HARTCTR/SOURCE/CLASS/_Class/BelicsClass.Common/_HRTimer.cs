using System;
using System.Runtime.InteropServices;

namespace BelicsClass.Common
{
    /// <summary>
    /// 高精度タイマー
    /// </summary>
    public sealed class BL_HRTimer : IDisposable
    {
        private uint s_time;
        private uint e_time;
        private bool start;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_HRTimer()
        {
            Native.BeginPeriod(1);
            //this.Start();
        }

        /// <summary>
        /// タイマー実行中を表します。
        /// </summary>
        public bool IsRunning { get { return start; } }

        /// <summary>
        /// ストップウォッチをリセットします。
        /// </summary>
        public void Reset()
        {
            s_time = 0;
            e_time = 0;
            start = false;
        }

        /// <summary>
        /// ストップウォッチを開始します。
        /// </summary>
        public void Start()
        {
            if (s_time == 0)
            {
                this.s_time = Native.GetTime();
                e_time = s_time;
            }

            start = true;
        }

        /// <summary>
        /// 再起動
        /// </summary>
        public uint Restart()
        {
            uint ret = ElapsedMilliseconds;

            s_time = Native.GetTime();
            e_time = s_time;

            start = true;

            return ret;
        }


        /// <summary>
        /// ストップウォッチを停止します。
        /// </summary>
        public void Stop()
        {
            if (e_time != 0)
            {
                e_time = Native.GetTime();
            }

            start = false;
        }

        /// <summary>
        /// 経過時間(ms)
        /// </summary>
        public uint ElapsedMilliseconds
        {
            get
            {
                uint tick;

                if (start == true)
                {
                    tick = Native.GetTime() - s_time;
                }
                else
                {
                    tick = e_time - s_time;
                }

                if (tick > uint.MaxValue)
                {
                    tick = uint.MaxValue;
                }

                return tick;
            }

        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            Native.EndPeriod(1);
        }

        private static class Native
        {
            [DllImport("winmm.dll", EntryPoint = "timeGetTime")]
            public static extern uint GetTime();
            [DllImport("winmm.dll", EntryPoint = "timeBeginPeriod")]
            public static extern uint BeginPeriod(uint uMilliseconds);
            [DllImport("winmm.dll", EntryPoint = "timeEndPeriod")]
            public static extern uint EndPeriod(uint uMilliseconds);
        }
    }
}
