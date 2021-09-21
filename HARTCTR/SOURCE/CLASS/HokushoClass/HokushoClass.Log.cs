using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.Text;

namespace HokushoClass.Log 
{
	/// <summary>
	/// ログ記録クラス
	/// </summary>
	public class LogWrite : IDisposable
	{
		private static string	CurrentPath = "";
		private static string	CurrentFile = "";
		private static  FileStream		Target = null;

		private bool	Enable = true;

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
		/// <param name="path">>ログを記録にするディレクトリ名。</param>
		public LogWrite(string path) 
		{
			if (CurrentPath == "")
			{
				if (path == "")
				{
					CurrentPath = Application.StartupPath +"\\LOG\\" +Path.GetFileNameWithoutExtension(Application.ExecutablePath);
				}
				else
				{
					CurrentPath = path + "\\" + Path.GetFileNameWithoutExtension(Application.ExecutablePath);
				}
			}
		}
		/// <summary>
		/// ログ記録クラス
		/// </summary>
		public LogWrite() : this("")
		{
		}
		/// <summary>
		/// ログ記録クラス
		/// </summary>
		/// <param name="logEnable">ログを記録にする場合は true。それ以外の場合は false。</param>
		public LogWrite(bool logEnable) : this("")
		{
			Enable = logEnable;
 		}

		//====================================================================================================
		// デストラクタ
		//====================================================================================================
		/// <summary>
		/// ログ記録クラス
		/// </summary>
		~LogWrite()
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

				CurrentFile = "";
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
		public void Add(string text) 
		{
			StringBuilder	data = new StringBuilder(); 
			byte[]	byte_data;

			if (check())
			{
				byte_data = Encoding.Default.GetBytes(DateTime.Now.ToString("hh:mm:ss.fff>") +text +"\r\n");

				Target.Write(byte_data, 0, byte_data.Length);
				Target.Flush();
			}
		}

		//====================================================================================================
		// リソースのチェック
		//====================================================================================================
		private bool check() 
		{
			ArrayList	files = new ArrayList();
			string	file, full_path;
			bool	status = false;

			if (Enable)
			{
				file = DateTime.Now.ToString("yyyyMMdd.LOG");

				if (CurrentFile != file)
				{
					if (!Directory.Exists(CurrentPath)) 
					{
						Directory.CreateDirectory(CurrentPath);
					}
					
					full_path = CurrentPath +"\\" +file;

					this.Dispose();

					try
					{
						Target = new FileStream(full_path, FileMode.Append, FileAccess.Write, FileShare.Read);

						CurrentFile = file;

						status = true;
					}
					catch
					{
					}
					
					foreach (string data in Directory.GetFiles(CurrentPath, "*.LOG"))
					{
						files.Add(data);
					}
					
					files.Sort();

					for (int count = 0; count < (files.Count -10); count++)
					{
						File.Delete(files[count].ToString());
					}
				}
				else
				{
					if (Target != null)
					{
						status = true;
					}
				}
			}

			return status;
		}
	}
}
