using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace BelicsClass.File
{
    /// <summary>
    /// 固定長ファイルクラス
    /// </summary>
    public class BL_FixedFile
    {
        private const int ERROR_LOCK_VIOLATION = 33;

        private FileStream File_stream = null;
        private BinaryReader Binary_reader = null;
        private BinaryWriter Binary_writer = null;
#if !WindowsCE
        private int Record_lock = -1;
#endif
        private int Record_length = 0;
        private int Error_code;
        private string Error_message;

        //====================================================================================================
        // オープン
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	string			file_name			ﾌｧｲﾙ名
        //	int				length				ﾚｺｰﾄﾞ長
        //	string			access_mode			ｱｸｾｽﾓｰﾄﾞ	"r"  読み専用
        //													"w"  書き専用
        //													"r+" 読み書き 既存ﾌｧｲﾙ
        //													"w+" 読み書き 新規ﾌｧｲﾙ
        //													"a+" 読み書き 既存or新規ﾌｧｲﾙ
        //	string			share_mode			共有ﾓｰﾄﾞ	"r"  読み許可
        //													"w"  書き許可
        //													"rw" 読み書き許可
        //													""   許可なし
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常		-1		予約済み
        //													-2		ﾌｧｲﾙなし
        //													-3		ﾃﾞｨﾚｸﾄﾘなし
        //													-1000	その他異常
        //
        //====================================================================================================
        /// <summary>
        /// 固定長ファイルをオープンします。
        /// </summary>
        /// <param name="file_name">オープンするファイルの名前を示す文字列を指定します。</param>
        /// <param name="length">オープンするファイルの１レコードのバイト数を指定します。</param>
        /// <param name="fileMode">ファイルを開く方法または作成する方法を指定します。</param>
        /// <param name="fileAccess">ファイルにアクセスする方法を指定します。</param>
        /// <param name="fileShare">ファイルの共有方法を指定します。</param>
        public bool Open(string file_name, int length, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            errors();

            if (File_stream != null)
            {
                errors(-1, "すでに使用されています。");

                return false;
            }

            Record_length = length;
#if !WindowsCE
            Record_lock = -1;
#endif
            try
            {
                File_stream = new FileStream(file_name, fileMode, fileAccess, fileShare);

                if (fileAccess != FileAccess.Write)
                {
                    Binary_reader = new BinaryReader(File_stream, Encoding.Default);
                }
                if (fileAccess != FileAccess.Read)
                {
                    Binary_writer = new BinaryWriter(File_stream, Encoding.Default);
                }
            }

            catch (FileNotFoundException ex)
            {
                errors(-2, ex.Message);
            }
            catch (DirectoryNotFoundException ex)
            {
                errors(-3, ex.Message);
            }
            catch (Exception ex)
            {
                errors(-1000, ex.Message);
            }

            if (Error_code != 0) this.Close();

            return Error_code == 0 ? true : false;
        }

        /// <summary>
        /// 固定長ファイルをオープンします。
        /// </summary>
        /// <param name="file_name">オープンするファイルの名前を示す文字列を指定します。</param>
        /// <param name="length">オープンするファイルの１レコードのバイト数を指定します。</param>
        /// <param name="access_mode">オープンするファイルのファイル属性を指定します。</param>
        /// <param name="share_mode">オープンするファイルの共有属性を指定します。</param>
        public bool Open(string file_name, int length, string access_mode, string share_mode)
        {
            FileMode fileMode;
            FileAccess fileAccess;
            FileShare fileShare;

            if (access_mode == "r") fileMode = FileMode.Open;
            else if (access_mode == "w") fileMode = FileMode.Create;
            else if (access_mode == "r+") fileMode = FileMode.Open;
            else if (access_mode == "w+") fileMode = FileMode.Create;
            else if (access_mode == "a+") fileMode = FileMode.Append;
            else fileMode = FileMode.OpenOrCreate;

            if (access_mode == "r") fileAccess = FileAccess.Read;
            else if (access_mode == "w") fileAccess = FileAccess.Write;
            else if (access_mode == "r+") fileAccess = FileAccess.ReadWrite;
            else if (access_mode == "w+") fileAccess = FileAccess.ReadWrite;
            else if (access_mode == "a+") fileAccess = FileAccess.Write;
            else fileAccess = FileAccess.ReadWrite;

            if (share_mode == "r") fileShare = FileShare.Read;
            else if (share_mode == "w") fileShare = FileShare.Write;
            else if (share_mode == "rw") fileShare = FileShare.ReadWrite;
            else fileShare = FileShare.None;

            return Open(file_name, length, fileMode, fileAccess, fileShare);
        }

        //====================================================================================================
        // クローズ
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
        /// 固定長ファイルをクローズします。
        /// </summary>
        public void Close()
        {
            if (File_stream == null)
            {
                return;
            }

#if !WindowsCE
            if (Record_lock != -1) File_stream.Unlock(Record_lock * Record_length, Record_length);
#endif
            if (Binary_reader != null) Binary_reader.Close();
            if (Binary_writer != null) Binary_writer.Close();

            File_stream.Close();

            Record_length = 0;
#if !WindowsCE
            Record_lock = -1;
#endif
            Binary_reader = null;
            Binary_writer = null;
            File_stream = null;
        }

        //====================================================================================================
        // 読み込み
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	int				record_no			ﾚｺｰﾄﾞ№
        //	bool			lock_flag			ﾛｯｸﾌﾗｸﾞ(true:する false:しない)
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	byte[]			data				ﾃﾞｰﾀ
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常		-99		無効
        //													-1000	その他異常
        //
        //====================================================================================================
        /// <summary>
        /// ファイルからデータを読み込みます。 
        /// </summary>
        /// <param name="data">読み込んだレコードのデータが格納されるバイト配列を指定します。</param>
        /// <param name="record_no">読み込みを行うレコード№を指定します。</param>
        /// <param name="lock_flag">ロックする場合は true。それ以外の場合は false。</param>
        public bool Read(out byte[] data, int record_no, bool lock_flag)
        {
            int error_no;
            bool status;

            data = new byte[0];

            errors();

            if (File_stream == null)
            {
                errors(-99, "無効です。");

                return false;
            }

            if (record_no >= 0) File_stream.Position = Record_length * record_no;
            else record_no = (int)File_stream.Position / Record_length;

            try
            {
                status = true;

                if (lock_flag) status = this.Lock(record_no);

                if (status)
                {
                    while (true)
                    {
                        try
                        {
                            data = Binary_reader.ReadBytes(Record_length);

                            if (data.Length == 0) errors(-1, "EOFを検出しました。");
                            break;
                        }
                        catch (IOException ex)
                        {
                            error_no = Marshal.GetLastWin32Error();

                            if (error_no != ERROR_LOCK_VIOLATION)
                            {
                                errors(error_no, ex.Message);
                                break;
                            }
                        }
                        catch (Exception ex)
                        {
                            errors(-1000, ex.Message);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                errors(-1000, ex.Message);
            }

            return Error_code == 0 ? true : false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="record_no"></param>
        /// <returns></returns>
        public bool Read(out byte[] data, int record_no) { return Read(out data, record_no, false); }

        //====================================================================================================
        // 書き込み
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	byte[]			data				ﾃﾞｰﾀ
        //	int				record_no			ﾚｺｰﾄﾞ№
        //	bool			unlock_flag			ｱﾝﾛｯｸﾌﾗｸﾞ(true:する false:しない)
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常		-99		無効
        //													-1000	その他異常
        //
        //====================================================================================================
        /// <summary>
        /// ファイルにデータを書き込みます。
        /// </summary>
        /// <param name="data">書き込むレコードのデータが格納されているバイト配列を指定します。</param>
        /// <param name="record_no">書き込みを行うレコード№を指定します。</param>
        /// <param name="unlock_flag">ロックを解除する場合は true。それ以外の場合は false。</param>
        public bool Write(byte[] data, int record_no, bool unlock_flag)
        {
            int error_no;
#if !WindowsCE
            bool status;
#endif

            errors();

            if (File_stream == null)
            {
                errors(-99, "無効です。");

                return false;
            }

            if (record_no >= 0) File_stream.Position = Record_length * record_no;

            try
            {
#if !WindowsCE
                status = false;
#endif
                while (true)
                {
                    try
                    {
                        Binary_writer.Write(data, 0, Record_length);
                        Binary_writer.Flush();

#if !WindowsCE
                        status = true;
#endif
                        break;
                    }
                    catch (IOException ex)
                    {
                        error_no = Marshal.GetLastWin32Error();

                        if (error_no != ERROR_LOCK_VIOLATION)
                        {
                            errors(error_no, ex.Message);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors(-1000, ex.Message);
                        break;
                    }
                }

#if !WindowsCE
                if (status)
                {
                    if (unlock_flag) this.Unlock();
                }
#endif
            }
            catch (Exception ex)
            {
                errors(-1000, ex.Message);
            }

            return Error_code == 0 ? true : false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="record_no"></param>
        /// <returns></returns>
        public bool Write(byte[] data, int record_no) { return Write(data, record_no, false); }

        //====================================================================================================
        // ファイルポインタの移動
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	int				record_no			移動するﾚｺｰﾄﾞNo(-1:EOF位置)
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常		-99		無効
        //													-1000	その他異常
        //
        //====================================================================================================
        /// <summary>
        /// ファイルポインタの位置を設定します。
        /// </summary>
        /// <param name="record_no">現在の位置に設定するレコード№を指定します。［EOF］に設定する場合は -1。 </param>
        public bool Seek(int record_no)
        {
            errors();

            if (File_stream == null)
            {
                errors(-99, "無効です。");

                return false;
            }

            if (record_no == -1) File_stream.Seek(0, SeekOrigin.End);
            else File_stream.Seek(record_no * Record_length, SeekOrigin.Begin);

            return Error_code == 0 ? true : false;
        }

        //====================================================================================================
        // レコードのロック
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	int				record_no			ﾚｺｰﾄﾞ№
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常		-10		ﾛｯｸ違反
        //													-99		無効
        //													-1000	その他異常
        //
        //====================================================================================================
        /// <summary>
        /// ファイル内の指定したレコードをロックします。 
        /// </summary>
        /// <param name="record_no">ロックするレコード№を指定します。</param>
        public bool Lock(int record_no)
        {
#if !WindowsCE
            int error_no;

            errors();

            if (File_stream == null)
            {
                errors(-99, "無効です。");

                return false;
            }

            this.Unlock();

            if (record_no >= 0 && record_no < (File_stream.Length / Record_length))
            {
                while (true)
                {
                    try
                    {
                        File_stream.Lock(record_no * Record_length, Record_length);

                        Record_lock = record_no;
                        break;
                    }
                    catch (IOException ex)
                    {
                        error_no = Marshal.GetLastWin32Error();

                        if (error_no != ERROR_LOCK_VIOLATION)
                        {
                            errors(error_no, ex.Message);
                            break;
                        }
                    }
                    catch (Exception ex)
                    {
                        errors(-1000, ex.Message);
                        break;
                    }
                }
            }
            else
            {
                errors(-10, "存在しないレコードに実行しました。");
            }

            return Error_code == 0 ? true : false;
#else
            return true;
#endif
        }

        //====================================================================================================
        // レコードのロック解除
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	なし
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常		-99		無効
        //													-1000	その他異常
        //
        //====================================================================================================
        /// <summary>
        /// ファイル内のロックを解除します。
        /// </summary>
        public bool Unlock()
        {
#if !WindowsCE
            errors();

            if (File_stream == null)
            {
                errors(-99, "無効です。");

                return false;
            }

            if (Record_lock >= 0)
            {
                File_stream.Unlock(Record_lock * Record_length, Record_length);

                Record_lock = -1;
            }

            return Error_code == 0 ? true : false;
#else
            return true;
#endif
        }

        //====================================================================================================
        // ファイル長プロパティ
        //====================================================================================================
        /// <summary>
        /// ファイル長 (バイト単位) を取得します。
        /// </summary>
        public long FileLength
        {
            get
            {
                if (File_stream != null) return File_stream.Length;
                else return -1;
            }
        }

        //====================================================================================================
        // レコード長プロパティ
        //====================================================================================================
        /// <summary>
        /// レコード長 (バイト単位) を取得します。
        /// </summary>
        public int RecordLength
        {
            get
            {
                if (File_stream != null) return Record_length;
                else return -1;
            }
        }

        //====================================================================================================
        // 異常コードプロパティ
        //====================================================================================================
        /// <summary>
        /// 異常コードを取得します。
        /// </summary>
        public int ErrorCode
        {
            get
            {
                return Error_code;
            }
        }

        //====================================================================================================
        // 異常内容プロパティ
        //====================================================================================================
        /// <summary>
        /// 異常内容を取得します。
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return Error_message;
            }
        }

        //====================================================================================================
        // ファイルポインタプロパティ
        //====================================================================================================
        /// <summary>
        /// ストリームの現在位置を取得または設定します。
        /// </summary>
        protected internal long Position
        {
            get
            {
                return File_stream.Position;
            }
            set
            {
                if (value == -1) File_stream.Seek(0, SeekOrigin.End);
                else File_stream.Seek(value, SeekOrigin.Begin);
            }
        }

        //====================================================================================================
        // レコード長プロパティ
        //====================================================================================================
        /// <summary>
        /// レコード長 (バイト単位) を設定します。
        /// </summary>
        protected internal int SetRecordLength
        {
            set
            {
                Record_length = value;
            }
        }

        //====================================================================================================
        // 異常の設定
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	int				code				異常ｺｰﾄﾞ
        //	string			message				異常内容
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	なし
        //
        //====================================================================================================
        /// <summary>
        /// 異常情報を初期化します。
        /// </summary>
        protected internal void errors()
        {
            Error_code = 0;
            Error_message = "";
        }

        /// <summary>
        /// 異常情報を設定します。
        /// </summary>
        /// <param name="code">異常コード。</param>
        /// <param name="message">異常内容が格納されている文字列。</param>
        protected internal void errors(int code, string message)
        {
            Error_code = code;
            Error_message = message;
        }
    }
}
