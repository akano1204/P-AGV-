using System;
using System.IO;

namespace BelicsClass.File
{
    /// <summary>
    /// 可変長ファイルクラス
    /// </summary>
    public class BL_FreeFile
    {
        private BL_FixedFile _File = new BL_FixedFile();
        private bool EofSeekMode = true;

        /// <summary>
        /// 可変長ファイルクラス
        /// </summary>
        public BL_FreeFile()
        {
        }

        /// <summary>
        /// 可変長ファイルクラス
        /// </summary>
        /// <param name="eofSeekMode">EOFシークモード（true:有効, false:無効）</param>
        public BL_FreeFile(bool eofSeekMode)
        {
            EofSeekMode = eofSeekMode;
        }

        //====================================================================================================
        // オープン
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	string			file_name			ﾌｧｲﾙ名
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
        /// 可変長ファイルをオープンします。
        /// </summary>
        /// <param name="file_name">オープンするファイルの名前を示す文字列を指定します。</param>
        /// <param name="fileMode">ファイルを開く方法または作成する方法を指定します。</param>
        /// <param name="fileAccess">ファイルにアクセスする方法を指定します。</param>
        /// <param name="fileShare">ファイルの共有方法を指定します。</param>
        public bool Open(string file_name, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
        {
            _File.Open(file_name, 1, fileMode, fileAccess, fileShare);

            return _File.ErrorCode == 0 ? true : false;
        }

        /// <summary>
        /// 可変長ファイルをオープンします。
        /// </summary>
        /// <param name="file_name">オープンするファイルの名前を示す文字列を指定します。</param>
        /// <param name="access_mode">オープンするファイルのファイル属性を指定します。</param>
        /// <param name="share_mode">オープンするファイルの共有属性を指定します。</param>
        public bool Open(string file_name, string access_mode, string share_mode)
        {
            _File.Open(file_name, 1, access_mode, share_mode);

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
        /// 可変長ファイルをクローズします。
        /// </summary>
        public void Close()
        {
            _File.Close();
        }

        //====================================================================================================
        // 読み込み
        //
        //- INPUT --------------------------------------------------------------------------------------------
        //	int				length				最大長
        //
        //- OUTPUT -------------------------------------------------------------------------------------------
        //	byte[]			data				ﾃﾞｰﾀ
        //
        //- RETURN -------------------------------------------------------------------------------------------
        //	bool			true				正常
        //					false				異常
        //
        //====================================================================================================
        /// <summary>
        /// ファイルからデータを読み込みます。
        /// </summary>
        /// <param name="data">読み込んだレコードのデータが格納されるバイト配列を指定します。</param>
        /// <param name="length">読み込みを行う最大バイト数を指定します。</param>
        public bool Read(out byte[] data, int length)
        {
            long position;
            int count, check = 0;
            byte[] buff;

            data = new byte[0];

            position = _File.Position;
            _File.SetRecordLength = length;

            if (_File.Read(out buff, -1, false))
            {
                for (count = 0; count < length && count < buff.Length; count++)
                {
                    if (check == 0 && buff[count] == 0x0D) check = 1;
                    else if (check == 1 && buff[count] == 0x0A) check = 2;
                    else if (check == 2) break;
                    else check = 0;
                }

                data = new byte[count];
                Array.Copy(buff, data, count);

                _File.Position = position + count;
            }

            return _File.ErrorCode == 0 ? true : false;
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
        //					false				異常
        //
        //====================================================================================================
        /// <summary>
        /// ファイルにデータを書き込みます。
        /// </summary>
        /// <param name="data">書き込むレコードのデータが格納されているバイト配列を指定します。</param>
        public bool Write(byte[] data)
        {
            if (EofSeekMode)
            {
                _File.Position = -1;
            }

            _File.SetRecordLength = data.Length;

            _File.Write(data, -1, false);

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
