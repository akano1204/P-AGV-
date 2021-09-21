using System;
using System.Diagnostics;

namespace BelicsClass.ProcessManage
{
    /// <summary>
    /// 外部プロセスを起動するクラス
    /// </summary>
    public class BL_Process
    {
        #region プロセス実行(スタティックメンバー)

        /// <summary>
        /// プロセスを実行します
        /// </summary>
        /// <param name="path">プロセスのファイルパス</param>
        /// <param name="args">引数</param>
        /// <returns>成功時:プロセス 失敗時:null</returns>
        public static Process StartProcess(string path, params string[] args)
        {
            return StartProcess(path, ProcessWindowStyle.Normal, args);
        }

        /// <summary>
        /// プロセスを実行します
        /// </summary>
        /// <param name="path">プロセスのファイルパス</param>
        /// <param name="style">起動時のスタイル</param>
        /// <param name="args">引数</param>
        /// <returns>成功時:プロセス 失敗時:null</returns>
        public static Process StartProcess(string path, ProcessWindowStyle style, params string[] args)
        {
            bool status = true;
            string parameter = "";
            Process proc = null;
            ProcessStartInfo info = new ProcessStartInfo();

            info.FileName = path;
            info.WindowStyle = style;
            if (style == ProcessWindowStyle.Hidden)
            {
                info.CreateNoWindow = true; // コンソール・ウィンドウを開かない
                info.UseShellExecute = false; // シェル機能を使用しない
            }

            if (args != null)
            {
                for (int count = 0; count < args.Length; count++)
                {
                    parameter += args[count];
                    if (count < args.Length - 1) parameter += " ";
                }
            }

            info.Arguments = parameter;

            try
            {
                proc = Process.Start(info);
            }
            catch
            {
                status = false;
            }

            if (status == true) return proc;
            else return null;
        }

        #endregion


        /// <summary></summary>
        public Process process = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool Start(string path, params string[] args)
        {
            return Start(path, ProcessWindowStyle.Normal, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="style"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool Start(string path, ProcessWindowStyle style, params string[] args)
        {
            if (process == null)
            {
                process = BL_Process.StartProcess(path, args);

                //if (Process.GetProcessById(process.Id) != null)
                //{
                //    return true;
                //}
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Abort()
        {
            if (process != null)
            {
                if (!process.HasExited)
                {
                    int id = process.Id;

                    process.Kill();

                    //if (Process.GetProcessById(id) == null)
                    //{
                    //    process.Dispose();
                    //    process = null;
                    //    return true;
                    //}
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Stop()
        {
            if (process != null)
            {
                if (!process.HasExited)
                {
                    int id = process.Id;

                    process.CloseMainWindow();

                    //if (Process.GetProcessById(id) == null)
                    //{
                    //    process.Dispose();
                    //    process = null;
                    //    return true;
                    //}
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Wait()
        {
            if (process != null)
            {
                if (!process.HasExited)
                {
                    process.WaitForExit();
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="timeout_millis"></param>
        public void Wait(int timeout_millis)
        {
            if (process != null)
            {
                if (!process.HasExited)
                {
                    process.WaitForExit(timeout_millis);
                    return;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasExited
        {
            get
            {
                if (process != null)
                {
                    return process.HasExited;
                }

                return true;
            }
        }
    }
}
