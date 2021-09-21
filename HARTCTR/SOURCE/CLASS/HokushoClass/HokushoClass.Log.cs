using System;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Collections;
using System.Text;

namespace HokushoClass.Log 
{
	/// <summary>
	/// ���O�L�^�N���X
	/// </summary>
	public class LogWrite : IDisposable
	{
		private static string	CurrentPath = "";
		private static string	CurrentFile = "";
		private static  FileStream		Target = null;

		private bool	Enable = true;

		//====================================================================================================
		// �R���X�g���N�^
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	bool			logEnable			۸ޏ���(true:����,false:���Ȃ�)
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// ���O�L�^�N���X
		/// </summary>
		/// <param name="path">>���O���L�^�ɂ���f�B���N�g�����B</param>
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
		/// ���O�L�^�N���X
		/// </summary>
		public LogWrite() : this("")
		{
		}
		/// <summary>
		/// ���O�L�^�N���X
		/// </summary>
		/// <param name="logEnable">���O���L�^�ɂ���ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		public LogWrite(bool logEnable) : this("")
		{
			Enable = logEnable;
 		}

		//====================================================================================================
		// �f�X�g���N�^
		//====================================================================================================
		/// <summary>
		/// ���O�L�^�N���X
		/// </summary>
		~LogWrite()
		{
			this.Dispose();
		}

		//====================================================================================================
		// ���\�[�X�̉��
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// ���ׂẴ��\�[�X��������܂��B
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
		// ���\�[�X�̎擾
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// ���\�[�X���擾���܂��B
		/// </summary>
		public void Initialize() 
		{
			check();
		}

		//====================================================================================================
		// ���O�̋L�^
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			text				�L�^������e
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// ���O���L�^���܂��B
		/// </summary>
		/// <param name="text">�L�^����f�[�^���i�[����Ă��镶����B</param>
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
		// ���\�[�X�̃`�F�b�N
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
