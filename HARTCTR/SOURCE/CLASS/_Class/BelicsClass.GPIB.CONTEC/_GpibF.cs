using System;
using System.Collections.Generic;
using System.Text;

using CgpibCs;
using CSubFuncCs;
using BelicsClass.File;

namespace BelicsClass.GPIB
{
    /// <summary>
    /// CONTEC GPIBボード GPIB/Fを制御します
    /// </summary>
    public class BL_GpibF
    {
        private BL_Log log = null;

        /// <summary>
        /// 通信モード
        /// </summary>
        public enum _Mode : int
        {
            /// <summary></summary>
            Master = 0,
            /// <summary></summary>
            Slave = 1,
        }

        /// <summary>
        /// ステータス
        /// </summary>
        public enum _Status : int
        {
            /// <summary></summary>
            SRQ = 0,
            /// <summary></summary>
            SRQ_EOI = 1,
            /// <summary></summary>
            NON = 128,
        }

        private object sync = new object();

        private Cgpib gpib = new Cgpib();
        private CSubFunc SubFunc = new CSubFunc();

        private uint source_addr;                           // マイアドレス
        private List<uint> target_addrs = new List<uint>(); // 相手機器アドレス

        private uint mode;                                  // モード(Master/Slave)

        /// <summary>
        /// モードを取得します
        /// </summary>
        public _Mode Mode { get { return (_Mode)mode; } }

        private uint err_code = 0;
        private string err_message = "";

        /// <summary>エラーコードを取得します。</summary>
        public uint LastErrorCode { get { return err_code; } }
        /// <summary>エラーメッセージを取得します。</summary>
        public string LastErrorMessage { get { return err_message; } }
        /// <summary>エラーをリセットします</summary>
        public void ErrorReset() { err_code = 0; err_message = ""; }

        private bool initialized = false;
        /// <summary>初期化済み状態を取得します。</summary>
        public bool Initialized { get { return initialized; } }

        /// <summary>
        /// 初期化します
        /// </summary>
        /// <param name="target_addrs"></param>
        /// <returns></returns>
        public string Init(int[] target_addrs)
        {
            string err = "";

            lock (sync)
            {
                string logfile = "";
                this.target_addrs.Clear();
                foreach (int addr in target_addrs)
                {
                    this.target_addrs.Add((uint)addr);
                    if (logfile != "") logfile += "-";
                    logfile += addr.ToString();
                }

                log = new BL_Log("", "GPIB_" + logfile + ".log");
                err_code = SubFunc.GpibInit(out err);              // GpibInit関数の呼び出しています。(SubClass.h)

                if (SubFunc.CheckRet("GpibInit", (err_code & 0xFF), out err_message) == 0)
                {
                    gpib.Boardsts(0x08, out source_addr);	// マイアドレスを取得します。
                    gpib.Boardsts(0x0a, out mode);			// スレーブかどうかチェック
                }
            }

            return err;
        }

        /// <summary>
        /// 終了します
        /// </summary>
        public void Exit()
        {
            lock (sync)
            {
                SubFunc.GpibExit();						// ExtGpib関数(サブルーチン)コールしています。
            }
        }

        /// <summary>
        /// 通信を行います（ブロードキャストメッセージ送信）
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public uint Talk(string message)
        {
            lock (sync)
            {
                uint len = (uint)message.Length;						// データの長さ
                uint[] Cmd = new uint[32];						        // コマンド(メッセージ)

                Cmd[0] = (uint)(1 + target_addrs.Count);				// 総コマンド数です。
                Cmd[1] = source_addr;									// トーカアドレスです。
                int index = 2;
                foreach (uint addr in target_addrs)
                {
                    Cmd[index++] = addr;                                // リスナアドレスです。
                    if (Cmd.Length <= index) break;
                }

                err_code = gpib.Talk(Cmd, len, message);				    // この部分が実際のデータを送信する部分です。
                int err_temp = SubFunc.CheckRet("GpTalk", (err_code & 0xFF), out err_message);
                if (err_temp == 0)
                {
                    Console.WriteLine(log.AddReturn("<-[" + message + "]"));
                    
                }
                else
                {
                    err_code = (uint)err_temp;
                    Console.WriteLine(log.AddReturn("xx[" + err_message + "]"));
                }
            }

            return err_code;
        }

        /// <summary>
        /// 通信を行います（１：１メッセージ送受信）
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public string TalkListen(int addr, string message)
        {
            string ans = "";

            lock (sync)
            {
                Talk(message);

                if (err_message == "")
                {
                    if (0 <= message.IndexOf('?'))
                    {
                        ans = Listen(addr);
                        if (err_message != "")
                        {
                            ans = "";
                        }
                    }
                }
            }

            return ans;
        }

        /// <summary>
        /// 通信します（受信）
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
        public string Listen(int addr)
        {
            string message = "";

            lock (sync)
            {
                StringBuilder rvBuffer = new StringBuilder(10000);	    // 受信データのバッファ
                uint[] Cmd = new uint[32];						        // コマンド(メッセージ)

                Cmd[0] = (uint)(1 + target_addrs.Count);				// 総コマンド数です。
                Cmd[1] = (uint)addr;                                    // トーカアドレスです。
                Cmd[2] = source_addr;									// リスナアドレスです。

                rvBuffer.Remove(0, rvBuffer.Length);					// 受信バッファをクリアします。
                uint len = (uint)rvBuffer.Capacity;						// 受信バッファのサイズを指定します。
                err_code = gpib.Listen(Cmd, ref len, rvBuffer);			// 実際に受信を行います。
                int err_temp = SubFunc.CheckRet("GpListen", (err_code & 0xFF), out err_message);
                if (err_temp == 0)
                {
                    int start = Convert.ToInt32(len);
                    int strlen = rvBuffer.Length - start;
                    message = rvBuffer.Remove(start, strlen).ToString(); 		// 受信データ
                    Console.WriteLine(log.AddReturn("->[" + message + "]"));
                }
                else
                {
                    err_code = (uint)err_temp;
                    Console.WriteLine(log.AddReturn("xx[" + err_message + "]"));
                }
            }

            return message;
        }

        /// <summary>
        /// ステータスをポーリングします
        /// </summary>
        /// <param name="talker"></param>
        /// <returns></returns>
        public _Status Polling(int talker)
        {
            uint ret = 0;

            lock (sync)
            {
                uint[] Pstb = new uint[32];					// ステータスバイトの配列
                uint[] Cmd = new uint[32];						// コマンド(メッセージ)
                for (int i = 0; i < 16; i++)
                {
                    Pstb[i] = 0x00;										// 配列の内容をクリアします。
                }
                Cmd[0] = 1;													// メッセージ総数
                Cmd[1] = (uint)talker;												// 相手機器アドレス
                ret = gpib.Poll(Cmd, Pstb);									// シリアルポールを行います。

                if ((ret & 0xFF) == 0)										// [SRQ] のみ確認(0xFFでマスク)
                {
                    //csBuf = string.Format("[SRQ] を認識しました。トーカ=[ {0:x} ]:ステータスバイト=[ {1:x} ]H", Cmd[Pstb[0]], Pstb[Pstb[0]]);
                }
                else if ((ret & 0xFF) == 1)									// [SRQ] [EOI]を確認
                {
                    //csBuf = string.Format("[SRQ] を [EOI]付きで確認。トーカ=[ {0:x} ]:ステータスバイト=[ {1:x} ]H", Cmd[Pstb[0]], Pstb[Pstb[0]]);
                }
                else if ((ret & 0xFF) == 128)								// 何も確認しなかった
                {
                    //csBuf = "[SRQ] を確認できませんでした。";
                }
            }

            return (_Status)(ret & 0xFF);
        }
    }
}
