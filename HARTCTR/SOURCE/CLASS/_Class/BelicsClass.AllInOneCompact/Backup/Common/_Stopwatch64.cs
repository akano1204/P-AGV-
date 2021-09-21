using System;
using System.Collections.Generic;
using System.Text;

namespace BelicsClass.Common
{
    /// <summary>
    /// ストップウォッチ６４クラス
    /// </summary>
    public class BL_Stopwatch64
    {
        #region フィールド

        private long s_time;
        private long keep_millseconds;
        private bool start;

        #endregion

        /// <summary>
        /// タイマー実行中を表します。
        /// </summary>
        public bool IsRunning { get { return start; } }

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_Stopwatch64()
        {
            Reset();
        }

        #endregion

        #region メソッド

        /// <summary>
        /// ストップウォッチをリセットします。
        /// </summary>
        public void Reset()
        {
            s_time = 0;
            keep_millseconds = 0;
            start = false;
        }

        /// <summary>
        /// ストップウォッチを開始します。
        /// </summary>
        public void Start()
        {
            if (start == false)
            {
                s_time = BL_Win32API.TickCount64;
            }

            start = true;
        }

        /// <summary>
        /// ストップウォッチを停止します。
        /// </summary>
        public void Stop()
        {
            if (start == true)
            {
                keep_millseconds += (BL_Win32API.TickCount64 - s_time);
                s_time = 0;
            }

            start = false;
        }

        #endregion

        #region プロパティー

        /// <summary>
        /// 経過ミリ秒を取得します。
        /// </summary>
        public long ElapsedMilliseconds
        {
            get
            {
                if (start == true)
                {
                    return keep_millseconds + (BL_Win32API.TickCount64 - s_time);
                }
                else
                {
                    return keep_millseconds;
                }
            }
        }

        #endregion
    }
}
