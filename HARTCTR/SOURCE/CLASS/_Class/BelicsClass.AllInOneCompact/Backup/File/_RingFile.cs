using System;
using System.IO;
using System.Text;

namespace BelicsClass.File
{
    /// <summary>
    /// リングファイルクラス
    /// </summary>
    public class BL_RingFile
    {
        private BL_FixedFile _File = new BL_FixedFile();

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
        //	int				ring				最大ﾚｺｰﾄﾞ数
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常		-1		予約済み
        //													-2		ﾌｧｲﾙなし
        //													-3		ﾃﾞｨﾚｸﾄﾘなし
        //													-100	ﾚｺｰﾄﾞ長違反
        //													-1000	その他異常
        //
        //====================================================================================================
        /// <summary>
        /// リングファイルをオープンします。
        /// </summary>
        /// <param name="file_name">オープンするファイルの名前を示す文字列を指定します。</param>
        /// <param name="length">オープンするファイルの１レコードのバイト数を指定します。</param>
        /// <param name="fileMode">ファイルを開く方法または作成する方法を指定します。</param>
        /// <param name="fileAccess">ファイルにアクセスする方法を指定します。</param>
        /// <param name="fileShare">ファイルの共有方法を指定します。</param>
        /// <param name="ring">書き込みのできる最大件数を指定します。</param>
        public bool Open(string file_name, int length, FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int ring)
        {
            bool status;
            int count;
            byte[] data, buff;

            _File.errors();

            if (length < 20)
            {
                _File.errors(-100, "レコード長は、20バイト以上必要です。");

                return false;
            }

            status = _File.Open(file_name, length, fileMode, fileAccess, fileShare);

            if (status)
            {
                if (0 == _File.FileLength / length)
                {
                    data = new byte[length];

                    for (count = 0; count < length; count++)
                    {
                        data[count] = 0x20;
                    }

                    buff = Encoding.Default.GetBytes(String.Format("{0:0000000000}{1:0000000000}", 1, ring));
                    Array.Copy(buff, data, 20);

                    _File.Write(data, 0, false);
                }
            }

            return _File.ErrorCode == 0 ? true : false;
        }

        /// <summary>
        /// リングファイルをオープンします。
        /// </summary>
        /// <param name="file_name">オープンするファイルの名前を示す文字列を指定します。 </param>
        /// <param name="length">オープンするファイルの１レコードのバイト数を指定します。</param>
        /// <param name="fileAccess_mode">オープンするファイルのファイル属性を指定します。</param>
        /// <param name="share_mode">オープンするファイルの共有属性を指定します。</param>
        /// <param name="ring">書き込みのできる最大件数を指定します。</param>
        public bool Open(string file_name, int length, string fileAccess_mode, string share_mode, int ring)
        {
            bool status;
            int count;
            byte[] data, buff;

            _File.errors();

            if (length < 20)
            {
                _File.errors(-100, "レコード長は、20バイト以上必要です。");

                return false;
            }

            status = _File.Open(file_name, length, fileAccess_mode, share_mode);

            if (status)
            {
                if (0 == _File.FileLength / length)
                {
                    data = new byte[length];

                    for (count = 0; count < length; count++)
                    {
                        data[count] = 0x20;
                    }

                    buff = Encoding.Default.GetBytes(String.Format("{0:0000000000}{1:0000000000}", 1, ring));
                    Array.Copy(buff, data, 20);

                    _File.Write(data, 0, false);
                }
            }

            return _File.ErrorCode == 0 ? true : false;
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
        /// リングファイルをクローズします。
        /// </summary>
        public void Close()
        {
            _File.Close();
        }

        //====================================================================================================
        // 書き込み
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	byte[]			data				ﾃﾞｰﾀ
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	なし
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常		-100	制御ﾚｺｰﾄﾞ破損
        //
        //====================================================================================================
        /// <summary>
        /// ファイルにデータを書き込みます。
        /// </summary>
        /// <param name="data">書き込むレコードのデータが格納されているバイト配列を指定します。</param>
        public bool Write(byte[] data)
        {
            bool status;
            int record_no, ring;
            byte[] buff, control;

            status = _File.Read(out control, 0, true);

            if (status)
            {
                if (control.Length == _File.RecordLength)
                {
                    record_no = Convert.ToInt32(Encoding.Default.GetString(control, 0, 10));
                    ring = Convert.ToInt32(Encoding.Default.GetString(control, 10, 10));

                    status = _File.Write(data, record_no, false);

                    if (status)
                    {
                        if (++record_no > ring) record_no = 1;

                        buff = Encoding.Default.GetBytes(String.Format("{0:0000000000}", record_no));
                        Array.Copy(buff, control, 10);

                        status = _File.Write(control, 0, true);
                    }
                }
                else
                {
                    _File.errors(-110, "制御レコードが壊れています。");
                }
            }

            return _File.ErrorCode == 0 ? true : false;
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
                return _File.FileLength;
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
                return _File.RecordLength;
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
                return _File.ErrorCode;
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
                return _File.ErrorMessage;
            }
        }
    }
}
