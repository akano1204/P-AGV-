﻿using System.Threading;

namespace BelicsClass.ProcessManage
{
    /// <summary>
    /// プロセスの起動をチェックします。
    /// </summary>
    public class BL_Only
    {
        private Mutex Mutex_flag;

        //====================================================================================================
        // コンストラクタ
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	string			mutexName			ﾌﾗｸﾞ名
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	なし
        //
        //====================================================================================================
        /// <summary>
        /// プロセスの起動をチェックします。
        /// </summary>
        /// <param name="mutexName">プロセスを判断するときに使用するユニーク名を指定します。</param>
        public BL_Only(string mutexName)
        {
            Mutex_flag = new Mutex(false, mutexName);
        }

        //====================================================================================================
        // デストラクタ
        //====================================================================================================
        /// <summary>
        /// 
        /// </summary>
        ~BL_Only()
        {
            Mutex_flag.Close();
        }

        //====================================================================================================
        // 起動チェックプロパティ
        //====================================================================================================
        /// <summary>
        /// プロセスの起動を取得します。起動していない場合は true。それ以外の場合は false。 
        /// </summary>
        public bool IsOnly
        {
            get
            {
                return Mutex_flag.WaitOne(0, false);
            }
        }

        //====================================================================================================
        // ミューテックス廃棄
        //====================================================================================================
        /// <summary>
        /// プロセスの起動チェック用ミューテックスを廃棄します。 
        /// </summary>
        public void Release()
        {
            Mutex_flag.Close();
        }
    }
}
