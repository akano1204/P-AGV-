using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Threading;
using System.Security.AccessControl;

namespace HokushoClass.SharedMemory
{
	//====================================================================================================
	// SharedMemory CLass
	//====================================================================================================
	/// <summary>
	/// 共有メモリクラス
	/// </summary>
	public class H_Memory
	{
		private Mutex Mutex;
		private MemoryMappedFile Mmf;
        private MemoryMappedViewAccessor Accs = null;
        private int Error_code;
		private string Error_message;

		//====================================================================================================
		// 共有メモリ作成
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			objectName			共有ﾒﾓﾘ名	
		//	uint			byteSize			共有ﾒﾓﾘｻｲｽﾞ
		//  string          groupName           ｱｸｾｽ許可を追加するｸﾞﾙｰﾌﾟ名またはﾕｰｻﾞｰ名       
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
		/// 共有メモリを作成し､ オープンします。
		/// </summary>
		/// <param name="objectName">作成する共有メモリ名。</param>
		/// <param name="byteSize">作成する共有メモリのバイトサイズ。</param>
		/// <param name="groupName">アクセス許可を追加するグループ名。</param>
		/// <returns></returns>
		public bool CreateMemory(string objectName, uint byteSize, string groupName)
		{
			bool status = true;
            bool cr;

			errors();


			if (groupName.Trim() != "")
			{

                try
                {
                    MutexSecurity security = new MutexSecurity();
                    
                    security.AddAccessRule(new MutexAccessRule(groupName, MutexRights.FullControl, AccessControlType.Allow));

                    Mutex = new Mutex(false, objectName + "_MUTEX", out cr, security);
				}
				catch (Exception ex)
				{
					status = false;

					errors(-1000, ex.Message);
				}

				try
				{
                    

                    //*********************************************************************************
                    AccessRule access = new  AccessRule<MemoryMappedFileRights>(groupName, MemoryMappedFileRights.FullControl, AccessControlType.Allow);
                    MemoryMappedFileSecurity security = new MemoryMappedFileSecurity();

                    security.SetAccessRule(new AccessRule<MemoryMappedFileRights>(groupName, 
                        MemoryMappedFileRights.FullControl, AccessControlType.Allow));
                    
                    Mutex.WaitOne();

                    Mmf = MemoryMappedFile.CreateOrOpen(objectName, byteSize, 
                        MemoryMappedFileAccess.ReadWrite
                        ,MemoryMappedFileOptions.None
                        , security
                        , HandleInheritability.Inheritable

                        );

                    Accs = Mmf.CreateViewAccessor();

                    Mutex.ReleaseMutex();

                    //*********************************************************************************


				}
				catch (Exception ex)
				{
					status = false;

					errors(-1000, ex.Message);
				}
            }
            else
            {
                try
                {
                    Mutex = new Mutex(false, objectName + "_MUTEX");
                }
                catch (Exception ex)
                {
                    status = false;

                    errors(-1000, ex.Message);
                }


                if (status)
                {
                    try
                    {
                        Mutex.WaitOne();

                        Mmf = MemoryMappedFile.CreateOrOpen(objectName, byteSize, MemoryMappedFileAccess.ReadWrite);

                        Accs = Mmf.CreateViewAccessor();

                        Mutex.ReleaseMutex();
                    }
                    catch (Exception ex)
                    {
                        errors(-1000, ex.Message);
                    }
                }
            }
            return Error_code == 0 ? true : false;
		}
		/// <summary>
		/// 共有メモリを作成し､ オープンします。
		/// </summary>
		/// <param name="objectName">作成する共有メモリ名。</param>
		/// <param name="byteSize">作成する共有メモリのバイトサイズ。</param>
		/// <returns></returns>
		public bool CreateMemory(string objectName, uint byteSize)
		{
			return CreateMemory(objectName, byteSize, "");
		}

		//====================================================================================================
		// 共有メモリ解放
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
		/// 共有メモリを解放します。
		/// </summary>
		public void ReleaseMemory()
		{
			Mutex.Close();

            Accs.Dispose();

            Mmf.Dispose();

			Accs = null;
		}

		//====================================================================================================
		// 読み込み
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int				index   			読み込み開始ｲﾝﾃﾞｯｸｽ
		//	uint			byteSize			読み込みｻｲｽﾞ
		//	bool			lockFlag  			ﾛｯｸﾌﾗｸﾞ
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]			data				読み込みﾃﾞｰﾀ	
		//
		//====================================================================================================
		/// <summary>
		/// 共有メモリからバイト配列でデータを読み込みます。
		/// </summary>
		/// <param name="index">共有メモリの読み込みを開始するオフセット値。</param>
		/// <param name="byteSize">共有メモリから読み込むバイト数。</param>
		/// <param name="lockFlag">共有メモリに排他ロックする場合は true。それ以外の場合は false。</param>
		/// <returns></returns>
		public byte[] ReadMemory(int index, uint byteSize, bool lockFlag)
		{
			byte[] data = new byte[byteSize];

			if (lockFlag)
			{
				Mutex.WaitOne();
			}

			try
			{
                Accs.ReadArray(index, data, 0, (int)byteSize);
			}
			catch (Exception ex)
			{
				errors(-1000, ex.Message);
			}

			if (lockFlag)
			{
				Mutex.ReleaseMutex();
			}

			return data;
		}
		/// <summary>
		/// 共有メモリからバイト配列でデータを読み込みます。
		/// </summary>
		/// <param name="index">共有メモリの読み込みを開始するオフセット値。</param>
		/// <param name="byteSize">共有メモリから読み込むバイト数。</param>
		/// <returns></returns>
		public byte[] ReadMemory(int index, uint byteSize)
		{
			return ReadMemory(index, byteSize, true);
		}

		//====================================================================================================
		// 書き込み
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int				index   			書き込み開始ｲﾝﾃﾞｯｸｽ
		//	byte[]			data				書き込みﾃﾞｰﾀ	
		//	uint			byteSize			書き込みｻｲｽﾞ
		//	bool			lockFlag  			ﾛｯｸﾌﾗｸﾞ
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	なし
		//
		//====================================================================================================
		/// <summary>
		/// 共有メモリにバイト配列のデータを書き込みます。
		/// </summary>
		/// <param name="index">共有メモリの書き込みを開始するオフセット値。</param>
		/// <param name="data">共有メモリに書き込むデータが格納されているバイト配列。</param>
		/// <param name="byteSize">共有メモリに書き込むバイト数。</param>
		/// <param name="lockFlag">共有メモリに排他ロックする場合は true。それ以外の場合は false。</param>
		public void WriteMemory(int index, byte[] data, uint byteSize, bool lockFlag)
		{
			if (lockFlag)
			{
				Mutex.WaitOne();
			}

			try
			{
                Accs.WriteArray(index, data, 0, (int)byteSize);
            }
			catch (Exception ex)
			{
				errors(-1000, ex.Message);
			}

			if (lockFlag)
			{
				Mutex.ReleaseMutex();
			}
		}
		/// <summary>
		/// 共有メモリにバイト配列のデータを書き込みます。
		/// </summary>
		/// <param name="index">共有メモリの書き込みを開始するオフセット値。</param>
		/// <param name="data">共有メモリに書き込むデータが格納されているバイト配列。</param>
		/// <param name="byteSize">共有メモリに書き込むバイト数。</param>
		public void WriteMemory(int index, byte[] data, uint byteSize)
		{
			WriteMemory(index, data, byteSize, true);
		}

		//====================================================================================================
		// 明示的ロック
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
		/// 共有メモリへの排他ロックを行います。
		/// </summary>
		public void Lock()
		{
			Mutex.WaitOne();
		}

		//====================================================================================================
		// 明示的ロック解除
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
		/// 共有メモリへの排他ロック解除を行います。
		/// </summary>
		public void Unlock()
		{
			Mutex.ReleaseMutex();
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
		private void errors()
		{
			Error_code = 0;
			Error_message = "";
		}
		private void errors(int error_code, string comment)
		{
			Error_code = error_code;
			Error_message = comment;
		}

		//====================================================================================================
		// 型変換(byte[4] -> Int32)
		//====================================================================================================
		/// <summary>
		/// ４バイトのバイト配列を Int32 に変換します。
		/// </summary>
		/// <param name="data">変換するデータが格納されているバイト配列。</param>
		/// <returns></returns>
		protected int BytesToInt32(byte[] data)
		{
			return BitConverter.ToInt32(data, 0);
		}

		//====================================================================================================
		// 型変換(Int32 -> byte[4])
		//====================================================================================================
		/// <summary>
		/// Int32 を４バイトのバイト配列に変換します。
		/// </summary>
		/// <param name="data">変換する値。</param>
		/// <returns></returns>
		protected byte[] Int32ToBytes(int data)
		{
			return BitConverter.GetBytes(data);
		}

		//====================================================================================================
		// 型変換(byte[2] -> Int16)
		//====================================================================================================
		/// <summary>
		/// ２バイトのバイト配列を Int16 に変換します。
		/// </summary>
		/// <param name="data">変換するデータが格納されているバイト配列。</param>
		/// <returns></returns>
		protected short BytesToInt16(byte[] data)
		{
			return BitConverter.ToInt16(data, 0);
		}

		//====================================================================================================
		// 型変換(Int16 -> byte[2])
		//====================================================================================================
		/// <summary>
		/// Int16 を２バイトのバイト配列に変換します。
		/// </summary>
		/// <param name="data">変換する値。</param>
		/// <returns></returns>
		protected byte[] Int16ToBytes(short data)
		{
			return BitConverter.GetBytes(data);
		}
	}
}
