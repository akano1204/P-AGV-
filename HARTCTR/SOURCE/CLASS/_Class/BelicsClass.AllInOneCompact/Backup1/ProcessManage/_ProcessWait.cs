using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace BelicsClass.ProcessManage
{
    /// <summary>
    /// 外部プロセスの終了を待機するクラスです。
    /// </summary>
    public class BL_ProcessWait
    {
            private Thread Sub_thread;
            private ProcessStartInfo Mode;

            //====================================================================================================
            // コンストラクタ
            //
            //- INPUT --------------------------------------------------------------------------------------------
            //	string			fileName			ﾌｧｲﾙ名
            //
            //- OUTPUT -------------------------------------------------------------------------------------------
            //	なし
            //
            //- RETURN -------------------------------------------------------------------------------------------
            //	なし
            //
            //====================================================================================================
            /// <summary>
            /// プロセスが終了するまで現在のスレッドの実行をブロックします。
            /// </summary>
            /// <param name="fileName">プロセスを起動するときに使用するファイル名を指定します。</param>
            public BL_ProcessWait(string fileName)
            {
                Sub_thread = new Thread(new ThreadStart(wait));
                Mode = new ProcessStartInfo(fileName);

                check();
            }
            /// <summary>
            /// プロセスが終了するまで現在のスレッドの実行をブロックします。
            /// </summary>
            /// <param name="fileName">プロセスを起動するときに使用するファイル名を指定します。</param>
            /// <param name="option">プロセスを起動するときに使用するコマンドライン引数を指定します。</param>
            public BL_ProcessWait(string fileName, string option)
            {
                Sub_thread = new Thread(new ThreadStart(wait));
                Mode = new ProcessStartInfo(fileName);
                Mode.Arguments = option;

                check();
            }
            /// <summary>
            /// プロセスが終了するまで現在のスレッドの実行をブロックします。
            /// </summary>
            /// <param name="mode">プロセスを起動するときに使用する値のセットを指定します。</param>
            public BL_ProcessWait(ProcessStartInfo mode)
            {
                Sub_thread = new Thread(new ThreadStart(wait));
                Mode = mode;

                check();
            }

            //====================================================================================================
            // 終了待機（スレッド）
            //
            //- INPUT --------------------------------------------------------------------------------------------
            //	なし
            //
            //- OUTPUT -------------------------------------------------------------------------------------------
            //	なし
            //
            //- RETURN -------------------------------------------------------------------------------------------
            //	なし
            //
            //====================================================================================================
            private void wait()
            {
                Process.Start(Mode).WaitForExit();
            }

            //====================================================================================================
            // 終了確認
            //
            //- INPUT --------------------------------------------------------------------------------------------
            //	なし
            //
            //- OUTPUT -------------------------------------------------------------------------------------------
            //	なし
            //
            //- RETURN -------------------------------------------------------------------------------------------
            //	なし
            //
            //====================================================================================================
            private void check()
            {
                Sub_thread.Start();

                while (Sub_thread.IsAlive)
                {
                    Application.DoEvents();

                    Thread.Sleep(100);
                }
            }
    }
}
