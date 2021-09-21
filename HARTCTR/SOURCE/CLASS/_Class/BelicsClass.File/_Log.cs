using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.Text;

using BelicsClass.Common;

namespace BelicsClass.File
{
    /// <summary>
    /// ログ記録クラス
    /// </summary>
    public class BL_Log : IDisposable
    {
        static object sync = new object();
        private string CurrentPath = "";
        private string CurrentDate = "";
        private string CurrentFile = "";
        private FileStream Target = null;
        private bool DateByDirectory = true;
        private bool Enable = true;

        //====================================================================================================
        // コンストラクタ
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	bool			logEnable			ﾛｸﾞ処理(true:する,false:しない)
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	なし
        //
        //====================================================================================================
        /// <summary>
        /// ログ記録クラス
        /// </summary>
        public BL_Log()
            : this(true, "", "", true)
        {
        }

        /// <summary>
        /// ログ記録クラス
        /// </summary>
        /// <param name="path">ログファイル保存先フォルダ</param>
        /// <param name="file">ログファイル保存先ファイル名</param>
        public BL_Log(string path, string file)
            : this(true, path, file, true)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="file"></param>
        /// <param name="date_by_directory">日別にフォルダを生成する</param>
        public BL_Log(string path, string file, bool date_by_directory)
            : this(true, path, file, date_by_directory)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logEnable"></param>
        /// <param name="path"></param>
        /// <param name="file"></param>
        public BL_Log(bool logEnable, string path, string file)
            : this(logEnable, path, file, true)
        {
        }

        /// <summary>
        /// ログ記録クラス
        /// </summary>
        /// <param name="logEnable">ログを記録にする場合は true。それ以外の場合は false。</param>
        /// <param name="path">ログファイル保存先フォルダ</param>
        /// <param name="file">ログファイル保存先ファイル名</param>
        /// <param name="date_by_directory">日別にフォルダを生成する</param>
        public BL_Log(bool logEnable, string path, string file, bool date_by_directory)
        {
            Enable = logEnable;
            DateByDirectory = date_by_directory;

            if (path != "")
            {
                CurrentPath = path;
            }
            else
            {
                CurrentPath = Application.StartupPath + "\\LOG";
            }

            if (file != "")
            {
                if (!Path.HasExtension(file))
                {
                    CurrentFile = file + ".LOG";
                }
                else
                {
                    CurrentFile = file;// + ".LOG";
                }
            }
            else
            {
                CurrentFile = Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".LOG";
            }
        }

        //====================================================================================================
        // デストラクタ
        //====================================================================================================
        /// <summary>
        /// ログ記録クラス
        /// </summary>
        ~BL_Log()
        {
            this.Dispose();
        }

        //====================================================================================================
        // リソースの解放
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
        /// <summary>
        /// すべてのリソースを解放します。
        /// </summary>
        public void Dispose()
        {
            if (Target != null)
            {
                Target.Close();

                Target = null;

                //CurrentDate = "";
            }
        }

        //====================================================================================================
        // リソースの取得
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
        /// <summary>
        /// リソースを取得します。
        /// </summary>
        public void Initialize()
        {
            check();
        }

        //====================================================================================================
        // ログの記録
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	string			text				記録する内容
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	なし
        //
        //====================================================================================================
        /// <summary>
        /// ログを記録します。
        /// </summary>
        /// <param name="text">記録するデータが格納されている文字列。</param>
        public long Add(string text)
        {
            return Add(text, 0);
        }

        /// <summary>
        /// ログを記録します。
        /// </summary>
        /// <param name="text">記録するデータが格納されている文字列。</param>
        /// <param name="level">見出し行とするための[.]を付加する個数</param>
        /// <returns>ログ書き込みに要した時間(ms)を返します。</returns>
        public long Add(string text, int level)
        {
            long sw = BL_Win32API.TickCount64;

            lock (sync)
            {
                StringBuilder data = new StringBuilder();
                byte[] byte_data;

                if (check())
                {
                    string dot = "";
                    for (int i = 0; i < level; i++) dot += ".";

                    System.Diagnostics.Debug.WriteLine(text);
                    byte_data = Encoding.Default.GetBytes(dot + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff>") + text + "\r\n");

                    try
                    {
                        Target.Write(byte_data, 0, byte_data.Length);
                        Target.Flush();
                    }
                    catch { }
                }
            }

            sw = BL_Win32API.TickCount64 - sw;

            return sw;
        }

        /// <summary>
        /// ログを記録します。
        /// </summary>
        /// <param name="text">記録するデータが格納されている文字列。</param>
        public string AddReturn(string text)
        {
            return AddReturn(text, 0);
        }

        /// <summary>
        /// ログを記録します。
        /// 記録した文字列を返します。(改行は含みません)
        /// </summary>
        /// <param name="text">記録するデータが格納されている文字列。</param>
        /// <param name="level">見出し行とするための[.]を付加する個数</param>
        /// <returns>ログ書き込みに要した時間(ms)を返します。</returns>
        public string AddReturn(string text, int level)
        {
            string log = "";

            lock (sync)
            {
                string dot = "";
                for (int i = 0; i < level; i++) dot += ".";

                System.Diagnostics.Debug.WriteLine(text);
                log = dot + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff>") + text;

                if (check())
                {
                    try
                    {
                        byte[] byte_data = Encoding.Default.GetBytes(log + "\r\n");
                        Target.Write(byte_data, 0, byte_data.Length);
                        Target.Flush();
                    }
                    catch { }
                }
            }

            return log;
        }

        //====================================================================================================
        // リソースのチェック
        //====================================================================================================
        private bool check()
        {
            ArrayList files = new ArrayList();
            string date;
            string full_path;
            bool status = false;
            string path = "";

            if (Enable)
            {
                if (DateByDirectory)
                {
                    date = DateTime.Now.ToString("yyyy.MM.dd");

                    if (CurrentDate != date)
                    {
                        path = Path.Combine(CurrentPath, date);
                        
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        full_path = Path.Combine(path, CurrentFile);

                        this.Dispose();

                        try
                        {
                            Target = new FileStream(full_path, FileMode.Append, FileAccess.Write, FileShare.Read);

                            CurrentDate = date;

                            status = true;
                        }
                        catch
                        {
                        }

                        #region 旧フォルダの削除

                        foreach (string data in Directory.GetDirectories(CurrentPath, "????.??.??"))
                        {
                            files.Add(data);
                        }

                        files.Sort();

                        for (int count = 0; count < (files.Count - 30); count++)
                        {
                            try
                            {
                                Directory.Delete(files[count].ToString(), true);
                            }
                            catch
                            {
                                //if (Target != null)
                                //{
                                //    byte[] byte_data;
                                //    byte_data = Encoding.Default.GetBytes(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff>") + files[count].ToString() + " Delete Error\r\n");

                                //    try
                                //    {
                                //        Target.Write(byte_data, 0, byte_data.Length);
                                //        Target.Flush();
                                //    }
                                //    catch { }
                                //}
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        if (Target != null)
                        {
                            status = true;
                        }
                    }
                }
                else
                {
                    if (Target == null)
                    {
                        path = CurrentPath;
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        full_path = Path.Combine(path, CurrentFile);

                        this.Dispose();

                        try
                        {
                            Target = new FileStream(full_path, FileMode.Append, FileAccess.Write, FileShare.Read);

                            status = true;
                        }
                        catch
                        {
                        }

                        #region 旧ファイルの削除

                        foreach (string data in Directory.GetFiles(CurrentPath))
                        {
                            files.Add(data);
                        }

                        files.Sort();

                        for (int count = 0; count < (files.Count - 30); count++)
                        {
                            try
                            {
                                System.IO.File.Delete(files[count].ToString());
                            }
                            catch
                            {
                                //if (Target != null)
                                //{
                                //    byte[] byte_data;
                                //    byte_data = Encoding.Default.GetBytes(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff>") + files[count].ToString() + " Delete Error\r\n");

                                //    try
                                //    {
                                //        Target.Write(byte_data, 0, byte_data.Length);
                                //        Target.Flush();
                                //    }
                                //    catch { }
                                //}
                            }
                        }

                        #endregion
                    }
                    else
                    {
                        status = true;
                    }
                }
            }

            return status;
        }
    }
}
