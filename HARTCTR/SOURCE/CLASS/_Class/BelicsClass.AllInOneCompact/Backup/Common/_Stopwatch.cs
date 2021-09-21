using System;
using System.Collections.Generic;
using System.Text;

namespace BelicsClass.Common
{
    /// <summary>
    /// ストップウォッチクラス
    /// </summary>
    public class BL_Stopwatch
    {
        private long s_time;
        private long e_time;
        private bool start;

        /// <summary>
        /// タイマー実行中を表します。
        /// </summary>
        public bool IsRunning { get { return start; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_Stopwatch()
        {
            Reset();
        }

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
                s_time = BL_Win32API.TickCount;
                e_time = s_time;
            }

            start = true;
        }

        /// <summary>
        /// ストップウォッチを停止して開始します
        /// </summary>
        /// <returns></returns>
        public int Restart()
        {
            int ret = ElapsedMilliseconds;

            s_time = BL_Win32API.TickCount;
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
                e_time = BL_Win32API.TickCount;
            }

            start = false;
        }

        /// <summary>
        /// 経過ミリ秒を取得します。
        /// </summary>
        public int ElapsedMilliseconds
        {
            get
            {
                long tick;

                if (start == true)
                {
                    tick = BL_Win32API.TickCount - s_time;
                }
                else
                {
                    tick = e_time - s_time;
                }

                if (tick > int.MaxValue)
                {
                    tick = int.MaxValue;
                }

                return (int)tick;
            }
        }
    }
}
