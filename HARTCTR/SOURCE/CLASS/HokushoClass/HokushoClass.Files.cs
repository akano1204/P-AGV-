using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace HokushoClass.Files
{
	#region 固定長ファイルクラス
	/// <summary>
	/// 固定長ファイルクラス
	/// </summary>
	public class H_File
	{
		private const int		ERROR_LOCK_VIOLATION = 33;

		private FileStream		File_stream = null;
		private BinaryReader	Binary_reader = null;
		private BinaryWriter	Binary_writer = null;
		private int		Record_lock = -1;
		private int		Record_length = 0;
		private int		Error_code;
		private string	Error_message;

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
			Record_lock = -1;

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
			catch(Exception ex)
			{
				errors(-1000, ex.Message);
			}

			if (Error_code != 0)	this.Close();

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
			FileMode	fileMode;
			FileAccess  fileAccess;
			FileShare	fileShare;
			
			if (access_mode == "r")			fileMode = FileMode.Open;
			else if (access_mode == "w")	fileMode = FileMode.Create;
			else if (access_mode == "r+")	fileMode = FileMode.Open;
			else if (access_mode == "w+")	fileMode = FileMode.Create;
			else if (access_mode == "a+")	fileMode = FileMode.Append;
			else							fileMode = FileMode.OpenOrCreate;
		
			if (access_mode == "r")			fileAccess = FileAccess.Read;
			else if (access_mode == "w")	fileAccess = FileAccess.Write;
			else if (access_mode == "r+")	fileAccess = FileAccess.ReadWrite;
			else if (access_mode == "w+")	fileAccess = FileAccess.ReadWrite;
			else if (access_mode == "a+")	fileAccess = FileAccess.Write;
			else							fileAccess = FileAccess.ReadWrite;

			if (share_mode == "r")			fileShare = FileShare.Read;
			else if (share_mode == "w")		fileShare = FileShare.Write;
			else if (share_mode == "rw")	fileShare = FileShare.ReadWrite;
			else							fileShare = FileShare.None;

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

			if (Record_lock != -1)	File_stream.Unlock(Record_lock *Record_length, Record_length);

			if (Binary_reader != null)	Binary_reader.Close();
			if (Binary_writer != null)	Binary_writer.Close();
	
			File_stream.Close();

			Record_length = 0;
			Record_lock = -1;

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
		//	bool			imperfect_flag		未完全ﾌﾗｸﾞ(true:有効 false:無効)
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
		/// <param name="imperfect_flag">読み込んだデータがレコード長を満たしていなくても有効にする場合は true。それ以外の場合は false。</param>
		public bool Read(out byte[] data, int record_no, bool lock_flag, bool imperfect_flag)
		{
			int		error_no;
			bool	status;
			
			data = new byte[0];

			errors();

			if (File_stream == null)
			{
				errors(-99, "無効です。");

				return false;
			}

			if (record_no >= 0)		File_stream.Position = Record_length *record_no;
			else					record_no =	(int)File_stream.Position /Record_length;

			try
			{
				status = true;
				
				if (lock_flag)		status = this.Lock(record_no);

				if (status)
				{
					while (true)
					{
						try
						{
							data = Binary_reader.ReadBytes(Record_length);

							if (data.Length != Record_length && !imperfect_flag)
							{
								if (lock_flag)	this.Unlock();

								File_stream.Position = Record_length *record_no;
								
								errors(-9, "データが不完全です。[" + data.Length.ToString() + "]");

								data = new byte[0];

								status = false;
							}

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
		/// ファイルからデータを読み込みます。 
		/// </summary>
		/// <param name="data">読み込んだレコードのデータが格納されるバイト配列を指定します。</param>
		/// <param name="record_no">読み込みを行うレコード№を指定します。</param>
		/// <param name="lock_flag">ロックする場合は true。それ以外の場合は false。</param>
		public bool Read(out byte[] data, int record_no, bool lock_flag)
		{
			return Read(out data, record_no, lock_flag, true);
		}

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
			int		error_no;
			bool	status;
			
			errors();
				
			if (File_stream == null)
			{
				errors(-99, "無効です。");

				return false;
			}

			if (record_no >= 0)		File_stream.Position = Record_length *record_no;

			try
			{
				status = false;

				while (true)
				{
					try
					{
						Binary_writer.Write(data, 0, Record_length);
						Binary_writer.Flush();

						status = true;
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

				if (status)
				{
					if (unlock_flag)		this.Unlock();
				}
			}
			catch (Exception ex)
			{
				errors(-1000, ex.Message);
			}

			return Error_code == 0 ? true : false;
		}

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

			if (record_no == -1)	File_stream.Seek(0, SeekOrigin.End);
			else					File_stream.Seek(record_no *Record_length, SeekOrigin.Begin);
			
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
			int		error_no;
			
			errors();

			if (File_stream == null)
			{
				errors(-99, "無効です。");

				return false;
			}

			this.Unlock();

			if (record_no >= 0 && record_no < (File_stream.Length /Record_length))
			{
				while (true)
				{
					try
					{
						File_stream.Lock(record_no *Record_length, Record_length);

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
			errors();
			
			if (File_stream == null)
			{
				errors(-99, "無効です。");

				return false;
			}

			if (Record_lock >= 0)
			{
				File_stream.Unlock(Record_lock *Record_length, Record_length);

				Record_lock = -1;
			}

			return Error_code == 0 ? true : false;
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
				if (File_stream != null)	return File_stream.Length;
				else						return -1;
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
				if (File_stream != null)	return Record_length;
				else						return -1;
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
				if (File_stream != null)	return File_stream.Position;
				else						return -1;
			}
			set
			{
				if (File_stream != null)
				{
					if (value == -1)	File_stream.Seek(0, SeekOrigin.End);
					else				File_stream.Seek(value, SeekOrigin.Begin);
				}
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
	#endregion

	#region 可変長ファイルクラス
	/// <summary>
	/// 可変長ファイルクラス
	/// </summary>
	public class H_FreeFile
	{
		private H_File	H_file = new H_File();
		private bool EofSeekMode = true;

		/// <summary>
		/// 可変長ファイルクラス
		/// </summary>
		public H_FreeFile()
		{
		}

		/// <summary>
		/// 可変長ファイルクラス
		/// </summary>
		/// <param name="eofSeekMode">EOFシークモード（true:有効, false:無効）</param>
		public H_FreeFile(bool eofSeekMode)
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
			H_file.Open(file_name, 1, fileMode, fileAccess, fileShare);

			return H_file.ErrorCode == 0 ? true : false;
		}
		
		/// <summary>
		/// 可変長ファイルをオープンします。
		/// </summary>
		/// <param name="file_name">オープンするファイルの名前を示す文字列を指定します。</param>
		/// <param name="access_mode">オープンするファイルのファイル属性を指定します。</param>
		/// <param name="share_mode">オープンするファイルの共有属性を指定します。</param>
		public bool Open(string file_name, string access_mode, string share_mode)
		{
			H_file.Open(file_name, 1, access_mode, share_mode);

			return H_file.ErrorCode == 0 ? true : false;
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
			H_file.Close();
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
			long	position;
			int		count, check = 0;
			byte[]	buff;
			
			data = new byte[0];

			position = H_file.Position;
			H_file.SetRecordLength = length;
			
			if (H_file.Read(out buff, -1, false, true))
			{
				for (count = 0; count < length && count < buff.Length; count++)
				{
					if (check == 0 && buff[count] == 0x0D)			check = 1;
					else if (check == 1 && buff[count] == 0x0A)		check = 2;
					else if (check == 2)							break;
					else											check = 0;
				}

				data = new byte[count];
				Array.Copy(buff, data, count);

				H_file.Position = position +count;
			}

			return H_file.ErrorCode == 0 ? true : false;
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
				H_file.Position = -1;
			}

			H_file.SetRecordLength = data.Length;

			H_file.Write(data, -1, false);

			return H_file.ErrorCode == 0 ? true : false;
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
				return H_file.FileLength;
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
				return H_file.ErrorCode;
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
				return H_file.ErrorMessage;
			}
		}
	}
	#endregion

	#region リングファイルクラス
	/// <summary>
	/// リングファイルクラス
	/// </summary>
	public class H_RingFile
	{
		private H_File	H_file = new H_File();

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
			bool	status;
			int		count;
			byte[]	data, buff;
			
			H_file.errors();

			if (length < 20)
			{
				H_file.errors(-100, "レコード長は、20バイト以上必要です。");
				
				return false;
			}

			status = H_file.Open(file_name, length, fileMode, fileAccess, fileShare);

			if (status)
			{
				if (0 == H_file.FileLength /length)
				{
					data = new byte[length];

					for (count = 0; count < length; count++)
					{
						data[count] = 0x20;
					}
					
					buff = Encoding.Default.GetBytes(String.Format("{0:0000000000}{1:0000000000}", 1, ring));
					Array.Copy(buff, data, 20);
					
					H_file.Write(data, 0, false);
				}
			}

			return H_file.ErrorCode == 0 ? true : false;
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
			bool	status;
			int		count;
			byte[]	data, buff;
			
			H_file.errors();

			if (length < 20)
			{
				H_file.errors(-100, "レコード長は、20バイト以上必要です。");
				
				return false;
			}

			status = H_file.Open(file_name, length, fileAccess_mode, share_mode);

			if (status)
			{
				if (0 == H_file.FileLength /length)
				{
					data = new byte[length];

					for (count = 0; count < length; count++)
					{
						data[count] = 0x20;
					}
					
					buff = Encoding.Default.GetBytes(String.Format("{0:0000000000}{1:0000000000}", 1, ring));
					Array.Copy(buff, data, 20);
					
					H_file.Write(data, 0, false);
				}
			}

			return H_file.ErrorCode == 0 ? true : false;
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
			H_file.Close();
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
			bool	status;
			int		record_no, ring;
			byte[]	buff, control;

			status = H_file.Read(out control, 0, true, true);

			if (status)
			{
				if (control.Length == H_file.RecordLength)
				{
					record_no = Convert.ToInt32(Encoding.Default.GetString(control, 0, 10));
					ring = Convert.ToInt32(Encoding.Default.GetString(control, 10, 10));

					status = H_file.Write(data, record_no, false);

					if (status)
					{
						if (++record_no > ring)		record_no = 1;

						buff = Encoding.Default.GetBytes(String.Format("{0:0000000000}", record_no));
						Array.Copy(buff, control, 10);

						status = H_file.Write(control, 0, true);
					}
				}
				else
				{
					H_file.errors(-110, "制御レコードが壊れています。");
				}
			} 

			return H_file.ErrorCode == 0 ? true : false;
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
				return H_file.FileLength;
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
				return H_file.RecordLength;
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
				return H_file.ErrorCode;
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
				return H_file.ErrorMessage;
			}
		}
	}
	#endregion

	#region 設定ファイルクラス
	/// <summary>
	/// 設定ファイルクラス
	/// </summary>
	public sealed class H_PrivateProfile
	{
		private H_PrivateProfile()
		{
		}

		#region Win32API
		[DllImport("kernel32", EntryPoint="GetPrivateProfileString")]
		static extern uint GetPrivateProfileString(string Section, string Key, string Default, System.Text.StringBuilder lpReturnedString, uint Size, string FileName);
 
		[DllImport("kernel32", EntryPoint="WritePrivateProfileString")] 
		static extern uint WritePrivateProfileString(string Section, string Key, string EntryString, string FileName);  
		#endregion

		/// <summary>
		/// 指定したセクションから文字列を取得します。
		/// </summary>
		/// <param name="Section">セクションの名前を示す文字列を指定します。</param>
		/// <param name="Key">キーの名前を示す文字列を指定します。</param>
		/// <param name="Default">キーがファイル内に見つからないときのデフォルト値を示す文字列を指定します。</param>
		/// <param name="FileName">ファイルの名前を示す文字列を指定します。</param>
		/// <returns></returns>
		public static string Get(string Section, string Key, string Default, string FileName)
		{
			System.Text.StringBuilder buff = new System.Text.StringBuilder(1024);

			GetPrivateProfileString(Section, Key, Default, buff, (uint)buff.Capacity, FileName);
			
			return buff.ToString();
		}

		/// <summary>
		/// 指定したセクションに文字列を設定します。
		/// </summary>
		/// <param name="Section">セクションの名前を示す文字列を指定します。</param>
		/// <param name="Key">キーの名前を示す文字列を指定します。</param>
		/// <param name="EntryString">値を示す文字列を指定します。</param>
		/// <param name="FileName">ファイルの名前を示す文字列を指定します。</param>
		/// <returns></returns>
		public static uint Set(string Section, string Key, string EntryString, string FileName)
		{
			return WritePrivateProfileString(Section, Key, EntryString, FileName);
		}
	}
	#endregion
}
