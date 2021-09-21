using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace HokushoClass.Files
{
	#region �Œ蒷�t�@�C���N���X
	/// <summary>
	/// �Œ蒷�t�@�C���N���X
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
		// �I�[�v��
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			file_name			̧�ٖ�
		//	int				length				ں��ޒ�
		//	string			access_mode			����Ӱ��	"r"  �ǂݐ�p
		//													"w"  ������p
		//													"r+" �ǂݏ��� ����̧��
		//													"w+" �ǂݏ��� �V�Ķ��
		//													"a+" �ǂݏ��� ����or�V�Ķ��
		//	string			share_mode			���LӰ��	"r"  �ǂ݋���
		//													"w"  ��������
		//													"rw" �ǂݏ�������
		//													""   ���Ȃ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�		-1		�\��ς�
		//													-2		̧�قȂ�
		//													-3		�ިڸ�؂Ȃ�
		//													-1000	���̑��ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �Œ蒷�t�@�C�����I�[�v�����܂��B
		/// </summary>
		/// <param name="file_name">�I�[�v������t�@�C���̖��O��������������w�肵�܂��B</param>
		/// <param name="length">�I�[�v������t�@�C���̂P���R�[�h�̃o�C�g�����w�肵�܂��B</param>
		/// <param name="fileMode">�t�@�C�����J�����@�܂��͍쐬������@���w�肵�܂��B</param>
		/// <param name="fileAccess">�t�@�C���ɃA�N�Z�X������@���w�肵�܂��B</param>
		/// <param name="fileShare">�t�@�C���̋��L���@���w�肵�܂��B</param>
		public bool Open(string file_name, int length, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
		{
			errors();

			if (File_stream != null)
			{
				errors(-1, "���łɎg�p����Ă��܂��B");

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
		/// �Œ蒷�t�@�C�����I�[�v�����܂��B
		/// </summary>
		/// <param name="file_name">�I�[�v������t�@�C���̖��O��������������w�肵�܂��B</param>
		/// <param name="length">�I�[�v������t�@�C���̂P���R�[�h�̃o�C�g�����w�肵�܂��B</param>
		/// <param name="access_mode">�I�[�v������t�@�C���̃t�@�C���������w�肵�܂��B</param>
		/// <param name="share_mode">�I�[�v������t�@�C���̋��L�������w�肵�܂��B</param>
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
		// �N���[�Y
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
		/// �Œ蒷�t�@�C�����N���[�Y���܂��B
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
		// �ǂݍ���
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int				record_no			ں��އ�
		//	bool			lock_flag			ۯ��׸�(true:���� false:���Ȃ�)
		//	bool			imperfect_flag		�����S�׸�(true:�L�� false:����)
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	byte[]			data				�ް�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�		-99		����
		//													-1000	���̑��ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �t�@�C������f�[�^��ǂݍ��݂܂��B 
		/// </summary>
		/// <param name="data">�ǂݍ��񂾃��R�[�h�̃f�[�^���i�[�����o�C�g�z����w�肵�܂��B</param>
		/// <param name="record_no">�ǂݍ��݂��s�����R�[�h�����w�肵�܂��B</param>
		/// <param name="lock_flag">���b�N����ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		/// <param name="imperfect_flag">�ǂݍ��񂾃f�[�^�����R�[�h���𖞂����Ă��Ȃ��Ă��L���ɂ���ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		public bool Read(out byte[] data, int record_no, bool lock_flag, bool imperfect_flag)
		{
			int		error_no;
			bool	status;
			
			data = new byte[0];

			errors();

			if (File_stream == null)
			{
				errors(-99, "�����ł��B");

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
								
								errors(-9, "�f�[�^���s���S�ł��B[" + data.Length.ToString() + "]");

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
		/// �t�@�C������f�[�^��ǂݍ��݂܂��B 
		/// </summary>
		/// <param name="data">�ǂݍ��񂾃��R�[�h�̃f�[�^���i�[�����o�C�g�z����w�肵�܂��B</param>
		/// <param name="record_no">�ǂݍ��݂��s�����R�[�h�����w�肵�܂��B</param>
		/// <param name="lock_flag">���b�N����ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		public bool Read(out byte[] data, int record_no, bool lock_flag)
		{
			return Read(out data, record_no, lock_flag, true);
		}

		//====================================================================================================
		// ��������
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			data				�ް�
		//	int				record_no			ں��އ�
		//	bool			unlock_flag			��ۯ��׸�(true:���� false:���Ȃ�)
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�		-99		����
		//													-1000	���̑��ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �t�@�C���Ƀf�[�^���������݂܂��B
		/// </summary>
		/// <param name="data">�������ރ��R�[�h�̃f�[�^���i�[����Ă���o�C�g�z����w�肵�܂��B</param>
		/// <param name="record_no">�������݂��s�����R�[�h�����w�肵�܂��B</param>
		/// <param name="unlock_flag">���b�N����������ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		public bool Write(byte[] data, int record_no, bool unlock_flag)
		{
			int		error_no;
			bool	status;
			
			errors();
				
			if (File_stream == null)
			{
				errors(-99, "�����ł��B");

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
		// �t�@�C���|�C���^�̈ړ�
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int				record_no			�ړ�����ں���No(-1:EOF�ʒu)
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�		-99		����
		//													-1000	���̑��ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �t�@�C���|�C���^�̈ʒu��ݒ肵�܂��B
		/// </summary>
		/// <param name="record_no">���݂̈ʒu�ɐݒ肷�郌�R�[�h�����w�肵�܂��B�mEOF�n�ɐݒ肷��ꍇ�� -1�B </param>
		public bool Seek(int record_no)
		{
			errors();

			if (File_stream == null)
			{
				errors(-99, "�����ł��B");

				return false;
			}

			if (record_no == -1)	File_stream.Seek(0, SeekOrigin.End);
			else					File_stream.Seek(record_no *Record_length, SeekOrigin.Begin);
			
			return Error_code == 0 ? true : false;
		}

		//====================================================================================================
		// ���R�[�h�̃��b�N
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int				record_no			ں��އ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�		-10		ۯ��ᔽ
		//													-99		����
		//													-1000	���̑��ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �t�@�C�����̎w�肵�����R�[�h�����b�N���܂��B 
		/// </summary>
		/// <param name="record_no">���b�N���郌�R�[�h�����w�肵�܂��B</param>
		public bool Lock(int record_no)
		{
			int		error_no;
			
			errors();

			if (File_stream == null)
			{
				errors(-99, "�����ł��B");

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
				errors(-10, "���݂��Ȃ����R�[�h�Ɏ��s���܂����B");
			}

			return Error_code == 0 ? true : false;
		}

		//====================================================================================================
		// ���R�[�h�̃��b�N����
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�		-99		����
		//													-1000	���̑��ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �t�@�C�����̃��b�N���������܂��B
		/// </summary>
		public bool Unlock()
		{
			errors();
			
			if (File_stream == null)
			{
				errors(-99, "�����ł��B");

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
		// �t�@�C�����v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �t�@�C���� (�o�C�g�P��) ���擾���܂��B
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
		// ���R�[�h���v���p�e�B
		//====================================================================================================
		/// <summary>
		/// ���R�[�h�� (�o�C�g�P��) ���擾���܂��B
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
		// �ُ�R�[�h�v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ُ�R�[�h���擾���܂��B
		/// </summary>
		public int ErrorCode
		{
			get
			{
				return Error_code;
			}		
		}

		//====================================================================================================
		// �ُ���e�v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ُ���e���擾���܂��B
		/// </summary>
		public string ErrorMessage
		{
			get
			{
				return Error_message;
			}
		}

		//====================================================================================================
		// �t�@�C���|�C���^�v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �X�g���[���̌��݈ʒu���擾�܂��͐ݒ肵�܂��B
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
		// ���R�[�h���v���p�e�B
		//====================================================================================================
		/// <summary>
		/// ���R�[�h�� (�o�C�g�P��) ��ݒ肵�܂��B
		/// </summary>
		protected internal int SetRecordLength
		{
			set
			{
				Record_length = value;
			}
		}

		//====================================================================================================
		// �ُ�̐ݒ�
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int				code				�ُ���
		//	string			message				�ُ���e
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// �ُ�������������܂��B
		/// </summary>
		protected internal void errors()
		{
			Error_code = 0;
			Error_message = "";
		}

		/// <summary>
		/// �ُ����ݒ肵�܂��B
		/// </summary>
		/// <param name="code">�ُ�R�[�h�B</param>
		/// <param name="message">�ُ���e���i�[����Ă��镶����B</param>
		protected internal void errors(int code, string message)
		{
			Error_code = code;
			Error_message = message;
		}
	}
	#endregion

	#region �ϒ��t�@�C���N���X
	/// <summary>
	/// �ϒ��t�@�C���N���X
	/// </summary>
	public class H_FreeFile
	{
		private H_File	H_file = new H_File();
		private bool EofSeekMode = true;

		/// <summary>
		/// �ϒ��t�@�C���N���X
		/// </summary>
		public H_FreeFile()
		{
		}

		/// <summary>
		/// �ϒ��t�@�C���N���X
		/// </summary>
		/// <param name="eofSeekMode">EOF�V�[�N���[�h�itrue:�L��, false:�����j</param>
		public H_FreeFile(bool eofSeekMode)
		{
			EofSeekMode = eofSeekMode;
		}

		//====================================================================================================
		// �I�[�v��
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			file_name			̧�ٖ�
		//	string			access_mode			����Ӱ��	"r"  �ǂݐ�p
		//													"w"  ������p
		//													"r+" �ǂݏ��� ����̧��
		//													"w+" �ǂݏ��� �V�Ķ��
		//													"a+" �ǂݏ��� ����or�V�Ķ��
		//	string			share_mode			���LӰ��	"r"  �ǂ݋���
		//													"w"  ��������
		//													"rw" �ǂݏ�������
		//													""   ���Ȃ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�		-1		�\��ς�
		//													-2		̧�قȂ�
		//													-3		�ިڸ�؂Ȃ�
		//													-1000	���̑��ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �ϒ��t�@�C�����I�[�v�����܂��B
		/// </summary>
		/// <param name="file_name">�I�[�v������t�@�C���̖��O��������������w�肵�܂��B</param>
		/// <param name="fileMode">�t�@�C�����J�����@�܂��͍쐬������@���w�肵�܂��B</param>
		/// <param name="fileAccess">�t�@�C���ɃA�N�Z�X������@���w�肵�܂��B</param>
		/// <param name="fileShare">�t�@�C���̋��L���@���w�肵�܂��B</param>
		public bool Open(string file_name, FileMode fileMode, FileAccess fileAccess, FileShare fileShare)
		{
			H_file.Open(file_name, 1, fileMode, fileAccess, fileShare);

			return H_file.ErrorCode == 0 ? true : false;
		}
		
		/// <summary>
		/// �ϒ��t�@�C�����I�[�v�����܂��B
		/// </summary>
		/// <param name="file_name">�I�[�v������t�@�C���̖��O��������������w�肵�܂��B</param>
		/// <param name="access_mode">�I�[�v������t�@�C���̃t�@�C���������w�肵�܂��B</param>
		/// <param name="share_mode">�I�[�v������t�@�C���̋��L�������w�肵�܂��B</param>
		public bool Open(string file_name, string access_mode, string share_mode)
		{
			H_file.Open(file_name, 1, access_mode, share_mode);

			return H_file.ErrorCode == 0 ? true : false;
		}

		//====================================================================================================
		// �N���[�Y
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
		/// �ϒ��t�@�C�����N���[�Y���܂��B
		/// </summary>
		public void Close()
		{
			H_file.Close();
		}

		//====================================================================================================
		// �ǂݍ���
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int				length				�ő咷
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	byte[]			data				�ް�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �t�@�C������f�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="data">�ǂݍ��񂾃��R�[�h�̃f�[�^���i�[�����o�C�g�z����w�肵�܂��B</param>
		/// <param name="length">�ǂݍ��݂��s���ő�o�C�g�����w�肵�܂��B</param>
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
		// ��������
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			data				�ް�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �t�@�C���Ƀf�[�^���������݂܂��B
		/// </summary>
		/// <param name="data">�������ރ��R�[�h�̃f�[�^���i�[����Ă���o�C�g�z����w�肵�܂��B</param>
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
		// �t�@�C�����v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �t�@�C���� (�o�C�g�P��) ���擾���܂��B
		/// </summary>
		public long FileLength
		{
			get
			{
				return H_file.FileLength;
			}
		}

		//====================================================================================================
		// �ُ�R�[�h�v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ُ�R�[�h���擾���܂��B
		/// </summary>
		public int ErrorCode
		{
			get
			{
				return H_file.ErrorCode;
			}		
		}

		//====================================================================================================
		// �ُ���e�v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ُ���e���擾���܂��B
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

	#region �����O�t�@�C���N���X
	/// <summary>
	/// �����O�t�@�C���N���X
	/// </summary>
	public class H_RingFile
	{
		private H_File	H_file = new H_File();

		//====================================================================================================
		// �I�[�v��
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			file_name			̧�ٖ�
		//	int				length				ں��ޒ�
		//	string			access_mode			����Ӱ��	"r"  �ǂݐ�p
		//													"w"  ������p
		//													"r+" �ǂݏ��� ����̧��
		//													"w+" �ǂݏ��� �V�Ķ��
		//													"a+" �ǂݏ��� ����or�V�Ķ��
		//	string			share_mode			���LӰ��	"r"  �ǂ݋���
		//													"w"  ��������
		//													"rw" �ǂݏ�������
		//													""   ���Ȃ�
		//	int				ring				�ő�ں��ސ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�		-1		�\��ς�
		//													-2		̧�قȂ�
		//													-3		�ިڸ�؂Ȃ�
		//													-100	ں��ޒ��ᔽ
		//													-1000	���̑��ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �����O�t�@�C�����I�[�v�����܂��B
		/// </summary>
		/// <param name="file_name">�I�[�v������t�@�C���̖��O��������������w�肵�܂��B</param>
		/// <param name="length">�I�[�v������t�@�C���̂P���R�[�h�̃o�C�g�����w�肵�܂��B</param>
		/// <param name="fileMode">�t�@�C�����J�����@�܂��͍쐬������@���w�肵�܂��B</param>
		/// <param name="fileAccess">�t�@�C���ɃA�N�Z�X������@���w�肵�܂��B</param>
		/// <param name="fileShare">�t�@�C���̋��L���@���w�肵�܂��B</param>
		/// <param name="ring">�������݂̂ł���ő匏�����w�肵�܂��B</param>
		public bool Open(string file_name, int length, FileMode fileMode, FileAccess fileAccess, FileShare fileShare, int ring)
		{
			bool	status;
			int		count;
			byte[]	data, buff;
			
			H_file.errors();

			if (length < 20)
			{
				H_file.errors(-100, "���R�[�h���́A20�o�C�g�ȏ�K�v�ł��B");
				
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
		/// �����O�t�@�C�����I�[�v�����܂��B
		/// </summary>
		/// <param name="file_name">�I�[�v������t�@�C���̖��O��������������w�肵�܂��B </param>
		/// <param name="length">�I�[�v������t�@�C���̂P���R�[�h�̃o�C�g�����w�肵�܂��B</param>
		/// <param name="fileAccess_mode">�I�[�v������t�@�C���̃t�@�C���������w�肵�܂��B</param>
		/// <param name="share_mode">�I�[�v������t�@�C���̋��L�������w�肵�܂��B</param>
		/// <param name="ring">�������݂̂ł���ő匏�����w�肵�܂��B</param>
		public bool Open(string file_name, int length, string fileAccess_mode, string share_mode, int ring)
		{
			bool	status;
			int		count;
			byte[]	data, buff;
			
			H_file.errors();

			if (length < 20)
			{
				H_file.errors(-100, "���R�[�h���́A20�o�C�g�ȏ�K�v�ł��B");
				
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
		// �N���[�Y
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
		/// �����O�t�@�C�����N���[�Y���܂��B
		/// </summary>
		public void Close()
		{
			H_file.Close();
		}

		//====================================================================================================
		// ��������
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			data				�ް�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�		-100	����ں��ޔj��
		//
		//====================================================================================================
		/// <summary>
		/// �t�@�C���Ƀf�[�^���������݂܂��B
		/// </summary>
		/// <param name="data">�������ރ��R�[�h�̃f�[�^���i�[����Ă���o�C�g�z����w�肵�܂��B</param>
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
					H_file.errors(-110, "���䃌�R�[�h�����Ă��܂��B");
				}
			} 

			return H_file.ErrorCode == 0 ? true : false;
		}

		//====================================================================================================
		// �t�@�C�����v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �t�@�C���� (�o�C�g�P��) ���擾���܂��B
		/// </summary>
		public long FileLength
		{
			get
			{
				return H_file.FileLength;
			}
		}

		//====================================================================================================
		// ���R�[�h���v���p�e�B
		//====================================================================================================
		/// <summary>
		/// ���R�[�h�� (�o�C�g�P��) ���擾���܂��B
		/// </summary>
		public int RecordLength
		{
			get
			{
				return H_file.RecordLength;
			}		
		}

		//====================================================================================================
		// �ُ�R�[�h�v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ُ�R�[�h���擾���܂��B
		/// </summary>
		public int ErrorCode
		{
			get
			{
				return H_file.ErrorCode;
			}		
		}

		//====================================================================================================
		// �ُ���e�v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ُ���e���擾���܂��B
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

	#region �ݒ�t�@�C���N���X
	/// <summary>
	/// �ݒ�t�@�C���N���X
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
		/// �w�肵���Z�N�V�������當������擾���܂��B
		/// </summary>
		/// <param name="Section">�Z�N�V�����̖��O��������������w�肵�܂��B</param>
		/// <param name="Key">�L�[�̖��O��������������w�肵�܂��B</param>
		/// <param name="Default">�L�[���t�@�C�����Ɍ�����Ȃ��Ƃ��̃f�t�H���g�l��������������w�肵�܂��B</param>
		/// <param name="FileName">�t�@�C���̖��O��������������w�肵�܂��B</param>
		/// <returns></returns>
		public static string Get(string Section, string Key, string Default, string FileName)
		{
			System.Text.StringBuilder buff = new System.Text.StringBuilder(1024);

			GetPrivateProfileString(Section, Key, Default, buff, (uint)buff.Capacity, FileName);
			
			return buff.ToString();
		}

		/// <summary>
		/// �w�肵���Z�N�V�����ɕ������ݒ肵�܂��B
		/// </summary>
		/// <param name="Section">�Z�N�V�����̖��O��������������w�肵�܂��B</param>
		/// <param name="Key">�L�[�̖��O��������������w�肵�܂��B</param>
		/// <param name="EntryString">�l��������������w�肵�܂��B</param>
		/// <param name="FileName">�t�@�C���̖��O��������������w�肵�܂��B</param>
		/// <returns></returns>
		public static uint Set(string Section, string Key, string EntryString, string FileName)
		{
			return WritePrivateProfileString(Section, Key, EntryString, FileName);
		}
	}
	#endregion
}
