using System;
using System.Text;

using HokushoClass.Rs232c;
using HokushoClass.Sockets;

namespace HokushoClass.Use.Labeler
{
	/// <summary>
	/// ���x���[�N���X�i������p�j
	/// </summary>
	public class NOZAKI
	{
		#region �񋓌^

		private enum DeviceType
		{
			NONE,
			RS_232C,
			LAN,
		}

		#endregion

		/// <summary>
		/// �R�}���h�ҏW�N���X
		/// </summary>
		public NOZAKI_Commands Commands = new NOZAKI_Commands();

		private DeviceType Device = DeviceType.NONE;
		private H_Rs232c Com;
		private H_Socket Socket;
		private int Error_code;
		private string Error_message;
		private int TimeOut;

		#region �I�[�v��

		/// <summary>
		/// �f�o�C�X���I�[�v�����܂��B
		/// </summary>
		/// <param name="port">�I�[�v������V���A���f�o�C�X�̃|�[�g���B</param>
		/// <param name="baudRate">�I�[�v������f�o�C�X�̃{�[���[�g�B</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool Open(int port, H_Rs232c.BaudRate baudRate)
		{
			bool status = false;

			errors();

			if (Device == DeviceType.NONE)
			{
				Com = new H_Rs232c();

				status = Com.Open(H_Rs232c.PortNo._01 + (port - 1), baudRate, H_Rs232c.ByteSize._8, H_Rs232c.Parity.Even, H_Rs232c.StopBits._1, H_Rs232c.FormatType.None);

				if (status)
				{
					Device = DeviceType.RS_232C;
				}
				else
				{
					errors(Com.ErrorCode, Com.ErrorMessage);

					Com = null;
				}
			}
			else
			{
				errors(-10000, "���ɃI�[�v������Ă��܂��B");
			}

			return status;
		}
		/// <summary>
		/// �f�o�C�X���I�[�v�����܂��B
		/// </summary>
		/// <param name="port">�I�[�v������V���A���f�o�C�X�̃|�[�g���B</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool Open(int port)
		{
			return Open(port, H_Rs232c.BaudRate._19200);
		}
		/// <summary>
		/// �f�o�C�X���I�[�v�����܂��B
		/// </summary>
		/// <param name="port">�I�[�v������V���A���f�o�C�X�̃|�[�g���B</param>
		/// <param name="baudRate">�I�[�v������V���A���f�o�C�X�̃{�[���[�g�B</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool Open(int port, int baudRate)
		{
			bool status = false;

			if (System.Enum.IsDefined(typeof(H_Rs232c.BaudRate), baudRate))
			{
				status = Open(port, (H_Rs232c.BaudRate)baudRate);
			}

			return status;
		}
		/// <summary>
		/// �f�o�C�X���I�[�v�����܂��B
		/// </summary>
		/// <param name="ip_address">�I�[�v������l�b�g���[�N�f�o�C�X�̂h�o�A�h���X�B</param>
		/// <param name="port">�I�[�v������\�P�b�g�̃|�[�g���B</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool Open(string ip_address, int port)
		{
			bool status = false;

			errors();

			if (Device == DeviceType.NONE)
			{
				Socket = new H_Socket(ip_address, port);

				status = Socket.Open(H_Socket.OpenMode.Client, H_Socket.FormatType.CR_LF);

				if (status)
				{
					Device = DeviceType.LAN;
				}
				else
				{
					errors(Socket.ErrorCode, Socket.ErrorMessage);

					Socket = null;
				}
			}
			else
			{
				errors(-10000, "���ɃI�[�v������Ă��܂��B");
			}

			return status;
		}
		/// <summary>
		/// �f�o�C�X���I�[�v�����܂��B
		/// </summary>
		/// <param name="ip_address">�I�[�v������l�b�g���[�N�f�o�C�X�̂h�o�A�h���X�B</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool Open(string ip_address)
		{
			return Open(ip_address, 8000);
		}

		#endregion

		#region �N���[�Y

		/// <summary>
		/// �f�o�C�X���N���[�Y���܂��B
		/// </summary>
		public void Close()
		{
			if (Device == DeviceType.RS_232C)
			{
				Com.Close();

				Com = null;
			}
			else if (Device == DeviceType.LAN)
			{
				Socket.Close();

				Socket = null;
			}

			Device = DeviceType.NONE;
		}

		#endregion

		#region ���M�i�o�C�g�z��j

		/// <summary>
		/// �f�o�C�X�Ƀo�C�g�z��̃f�[�^�𑗐M���܂��B
		/// </summary>
		/// <param name="data">���M����f�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool SendBytes(byte[] data)
		{
			bool status = false;

			errors();

			if (Device == DeviceType.RS_232C)
			{
				status = Com.SendBytes(data);

				if (!status)
				{
					errors(Com.ErrorCode, Com.ErrorMessage);
				}
			}
			else if (Device == DeviceType.LAN)
			{
				status = Socket.SendBytes(data);

				if (!status)
				{
					errors(Socket.ErrorCode, Socket.ErrorMessage);
				}
			}
			else
			{
				errors(-10001, "�I�[�v������Ă��܂���B");
			}

			return status;
		}

		#endregion

		#region ��M�i�o�C�g�z��j

		/// <summary>
		/// �f�o�C�X����o�C�g�z��Ńf�[�^����M���܂��B
		/// </summary>
		/// <param name="bytes_size">��M�����o�C�g���B����M�̂Ƃ��́A-1�B</param>
		/// <returns>��M�����o�C�g�z��</returns>
		public byte[] ReceiveBytes(out int bytes_size)
		{
			byte[] status = new byte[0];

			bytes_size = -1;

			if (Device == DeviceType.RS_232C)
			{
				status = Com.ReceiveBytes(out bytes_size);
			}
			else if (Device == DeviceType.LAN)
			{
				status = Socket.ReceiveBytes(out bytes_size);
			}

			return status;
		}

		#endregion

		#region �_�C���N�g���M�i�o�C�g�z��j

		/// <summary>
		/// �f�o�C�X�Ƀo�C�g�z��̃f�[�^�����̂܂ܑ��M���܂��B
		/// </summary>
		/// <param name="data">���M����f�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool SendBytesDirect(byte[] data)
		{
			bool status = false;

			errors();

			if (Device == DeviceType.RS_232C)
			{
				status = Com.SendBytesDirect(data);

				if (!status)
				{
					errors(Com.ErrorCode, Com.ErrorMessage);
				}
			}
			else if (Device == DeviceType.LAN)
			{
				status = Socket.SendBytesDirect(data);

				if (!status)
				{
					errors(Socket.ErrorCode, Socket.ErrorMessage);
				}
			}
			else
			{
				errors(-10001, "�I�[�v������Ă��܂���B");
			}

			return status;
		}

		#endregion

		#region �_�C���N�g��M�i�o�C�g�z��j

		/// <summary>
		/// �f�o�C�X����o�C�g�z��Ńf�[�^�����̂܂܎�M���܂��B
		/// </summary>
		/// <returns>��M�����o�C�g�z��</returns>
		public byte[] ReceiveBytesDirect()
		{
			byte[] status = new byte[0];

			if (Device == DeviceType.RS_232C)
			{
				status = Com.ReceiveBytesDirect();
			}
			else if (Device == DeviceType.LAN)
			{
				status = Socket.ReceiveBytesDirect();
			}

			return status;
		}

		#endregion

		#region �ҏW�f�[�^���M

		/// <summary>
		/// �f�o�C�X�ɕҏW�f�[�^�𑗐M���܂��B
		/// </summary>
		/// <param name="data">���M����f�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool PrintOut(byte[] data)
		{
			Bytes.Join(Encoding.Default.GetBytes("(#701)"), ref data, 6);

			return this.SendBytesDirect(data);
		}
		/// <summary>
		/// �f�o�C�X�ɕҏW�f�[�^�𑗐M���܂��B
		/// </summary>
		/// <param name="commands">�ҏW�ς݂̃��x���[�R�}���h�ҏW�N���X</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool PrintOut(NOZAKI_Commands commands)
		{
			return PrintOut(commands.BytesData);
		}
		/// <summary>
		/// �f�o�C�X�ɕҏW�f�[�^�𑗐M���܂��B
		/// </summary>
		/// <returns>true:����,false:�ُ�</returns>
		public bool PrintOut()
		{
			return PrintOut(Commands.BytesData);
		}

		#endregion

		#region ���Z�b�g�v�����M

		/// <summary>
		/// �f�o�C�X�Ƀ��Z�b�g�v���𑗐M���܂��B
		/// </summary>
		/// <param name="timeOut">�^�C���A�E�g�l�B</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool ResetSend(int timeOut)
		{
			errors();

			TimeOut = System.Environment.TickCount + timeOut;

			return SendBytesDirect(Encoding.Default.GetBytes("(#X)"));
		}
		/// <summary>
		/// �f�o�C�X�Ƀ��Z�b�g�v���𑗐M���܂��B
		/// </summary>
		/// <returns>true:����,false:�ُ�</returns>
		public bool ResetSend()
		{
			return ResetSend(3000);
		}

		#endregion

		#region ��ԗv�����M

		/// <summary>
		/// �f�o�C�X�ɏ�ԗv���𑗐M���܂��B
		/// </summary>
		/// <param name="timeOut">�^�C���A�E�g�l�B</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool StatusSend(int timeOut)
		{
			byte[] data;

			errors();

			TimeOut = System.Environment.TickCount + timeOut;

			data = Encoding.Default.GetBytes("(#703)");

			return SendBytesDirect(data);
		}
		/// <summary>
		/// �f�o�C�X�ɏ�ԗv���𑗐M���܂��B
		/// </summary>
		/// <returns>true:����,false:�ُ�</returns>
		public bool StatusSend()
		{
			return StatusSend(3000);
		}

		#endregion

		#region ��ԗv����M

		/// <summary>
		/// �f�o�C�X�����ԗv���̕ԓ�����M���܂��B
		/// </summary>
		/// <param name="statusCode">�X�e�[�^�X�R�[�h���i�[����Ă��镶����B</param>
		/// <param name="statusMessage">�X�e�[�^�X���e���i�[����Ă��镶����B</param>
		/// <returns>true:��M����,false:��M�҂�</returns>
		public bool StatusReceive(out string statusCode, out string statusMessage)
		{
			byte[] data;
			bool status = false;

			statusCode = "";
			statusMessage = "";

			data = ReceiveBytesDirect();

			if (data.Length == 2)
			{
				statusCode = Encoding.Default.GetString(data, 0, 2);

				switch (statusCode)
				{
				case "00":
					statusMessage = "�f�[�^���͑ҋ@��";
					break;

				case "01":
					statusMessage = "�󎚒�";
					break;

				case "02":
					statusMessage = "�󎚒�~��";
					break;

				case "10":
					statusMessage = "�^�O�E���x���G���h";
					break;

				case "11":
					statusMessage = "���{���G���h";
					break;

				case "12":
					statusMessage = "�w�b�h�A�b�v�G���[";
					break;

				case "13":
					statusMessage = "�ʐM�t�H�[�}�b�g�G���[";
					break;

				case "14":
					statusMessage = "��M�L���[�o�b�t�@�j�A�[�t��";
					break;

				case "15":
					statusMessage = "�w�b�h�̏�";
					break;

				case "17":
					statusMessage = "�^�O�E���x���Â܂�G���[";
					break;

				case "18":
					statusMessage = "�J�b�^�[�G���[";
					break;

				case "99":
					statusMessage = "�f�[�^���X";
					break;

				default:
					statusMessage = "���̑��G���[[" + statusCode + "]";
					break;
				}

				status = true;
			}
			else
			{
				if (System.Environment.TickCount >= TimeOut)
				{
					statusCode = "";
					statusMessage = "�d��OFF or �ʐM�ُ�";

					status = true;
				}
			}

			return status;
		}

		#endregion

		#region �ُ�R�[�h �v���p�e�B

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

		#endregion

		#region �ُ���e �v���p�e�B

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

		#endregion

		#region �ُ�̐ݒ�

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

		#endregion
	}

	/// <summary>
	/// ���x���[�R�}���h�ҏW�N���X�i������p�j
	/// </summary>
	public class NOZAKI_Commands
	{
		#region �񋓌^

		/// <summary>
		/// �@�햼
		/// </summary>
		public enum Model
		{
			/// <summary>
			/// FAD-400
			/// </summary>
			FAD400,
			/// <summary>
			/// FA-400
			/// </summary>
			FA400,
		}

		/// <summary>
		/// ��]�w��
		/// </summary>
		public enum Turn
		{
			/// <summary>
			/// ��]���Ȃ�
			/// </summary>
			_0 = 0,
			/// <summary>
			/// 90߉E��]
			/// </summary>
			_90 = 3,
			/// <summary>
			/// 180߉E��]
			/// </summary>
			_180 = 2,
			/// <summary>
			/// 270߉E��]
			/// </summary>
			_270 = 1,
		}

		/// <summary>
		/// �t�H���g�s�b�`
		/// </summary>
		public enum FontPitch
		{
			/// <summary>
			/// 
			/// </summary>
			_8 = '0',
			/// <summary>
			/// 
			/// </summary>
			_9 = '1',
			/// <summary>
			/// 
			/// </summary>
			_10 = '2',
			/// <summary>
			/// 
			/// </summary>
			_11 = '3',
			/// <summary>
			/// 
			/// </summary>
			_12 = '4',
			/// <summary>
			/// 
			/// </summary>
			_13 = '5',
			/// <summary>
			/// 
			/// </summary>
			_14 = '6',
			/// <summary>
			/// 
			/// </summary>
			_15 = '7',
			/// <summary>
			/// 
			/// </summary>
			_16 = '8',
			/// <summary>
			/// 
			/// </summary>
			_17 = '9',
			/// <summary>
			/// 
			/// </summary>
			_18 = 'A',
			/// <summary>
			/// 
			/// </summary>
			_19 = 'B',
			/// <summary>
			/// 
			/// </summary>
			_20 = 'C',
			/// <summary>
			/// 
			/// </summary>
			_21 = 'D',
			/// <summary>
			/// 
			/// </summary>
			_22 = 'E',
			/// <summary>
			/// 
			/// </summary>
			_23 = 'F',
			/// <summary>
			/// 
			/// </summary>
			_24 = 'G',
			/// <summary>
			/// 
			/// </summary>
			_26 = 'H',
			/// <summary>
			/// 
			/// </summary>
			_28 = 'I',
			/// <summary>
			/// 
			/// </summary>
			_30 = 'J',
			/// <summary>
			/// 
			/// </summary>
			_32 = 'K',
			/// <summary>
			/// 
			/// </summary>
			_34 = 'L',
			/// <summary>
			/// 
			/// </summary>
			_36 = 'M',
			/// <summary>
			/// 
			/// </summary>
			_38 = 'N',
			/// <summary>
			/// 
			/// </summary>
			_40 = 'O',
			/// <summary>
			/// 
			/// </summary>
			_42 = 'P',
			/// <summary>
			/// 
			/// </summary>
			_48 = 'Q',
			/// <summary>
			/// 
			/// </summary>
			_56 = 'R',
			/// <summary>
			/// 
			/// </summary>
			_64 = 'S',
			/// <summary>
			/// 
			/// </summary>
			_72 = 'T',
			/// <summary>
			/// 
			/// </summary>
			_80 = 'U',
			/// <summary>
			/// 
			/// </summary>
			_88 = 'V',
			/// <summary>
			/// 
			/// </summary>
			_96 = 'W',
			/// <summary>
			/// 
			/// </summary>
			_104 = 'X',
			/// <summary>
			/// 
			/// </summary>
			_112 = 'Y',
			/// <summary>
			/// 
			/// </summary>
			_120 = 'Z',
		}

		/// <summary>
		/// �t�H���g�^�C�v�`
		/// </summary>
		public enum FontTypeA
		{
			/// <summary>
			/// �k�k����
			/// </summary>
			L,
			/// <summary>
			/// �k����
			/// </summary>
			H,
			/// <summary>
			/// �n�b�q�a
			/// </summary>
			M,
			/// <summary>
			/// �n�b�q�`
			/// </summary>
			N,
			/// <summary>
			/// �r����
			/// </summary>
			S,
			/// <summary>
			/// �r�r����
			/// </summary>
			Q,
			/// <summary>
			/// �j����
			/// </summary>
			K,
		}

		/// <summary>
		/// �t�H���g�^�C�v�j
		/// </summary>
		public enum FontTypeK
		{
			/// <summary>
			/// �i�h�r�����i�P�U�~�P�U�h�b�g�j
			/// </summary>
			I,
			/// <summary>
			/// �i�h�r�����i�Q�S�~�Q�S�h�b�g�j
			/// </summary>
			J,
			/// <summary>
			/// �i�h�r�����i�R�Q�~�R�Q�h�b�g�j
			/// </summary>
			O,
		}

		/// <summary>
		/// �o�[�R�[�h��
		/// </summary>
		public enum BarcodeType
		{
			/// <summary>
			/// CODE39
			/// </summary>
			CODE39 = '1',
			/// <summary>
			/// �C���^�[���[�u�h2of5
			/// </summary>
			ITF = '2',
			/// <summary>
			/// UPC-A
			/// </summary>
			UPC_A = '3',
			/// <summary>
			/// JAN8/EAN8
			/// </summary>
			JAN8 = '4',
			/// <summary>
			/// JAN13/EAN13
			/// </summary>
			JAN13 = '5',
			/// <summary>
			/// NW7
			/// </summary>
			NW7 = '6',
			/// <summary>
			/// Matrix2of5
			/// </summary>
			Matrix2of5 = '7',
			/// <summary>
			/// CODE128
			/// </summary>
			CODE128 = '8',
		}

		#endregion

		private Model ModelType = NOZAKI_Commands.Model.FAD400;
		private int RawDataCount = 0;
		private byte[] RawData = new byte[4096];

		#region �R���X�g���N�^

		/// <summary>
		/// ���x���[�R�}���h�ҏW�N���X
		/// </summary>
		public NOZAKI_Commands()
		{
			ModelType = NOZAKI_Commands.Model.FAD400;

			RawDataCount = 0;
			RawData = new byte[4096];
		}
		/// <summary>
		/// ���x���[�R�}���h�ҏW�N���X
		/// </summary>
		/// <param name="modelType">�@����w�肵�܂��B</param>
		public NOZAKI_Commands(NOZAKI_Commands.Model modelType)
		{
			ModelType = modelType;

			RawDataCount = 0;
			RawData = new byte[4096];
		}

		#endregion

		#region �ҏW�J�n

		/// <summary>
		/// �ҏW�̊J�n��錾���܂��B
		/// </summary>
		public void Start()
		{
			RawData = new byte[4096];

			Array.Clear(RawData, 0, RawData.Length);
			RawDataCount = 0;

			append(Encoding.Default.GetBytes("("));
		}
		/// <summary>
		/// �ҏW�̊J�n��錾���܂��B
		/// </summary>
		/// <param name="xOffset">X���W�̃I�t�Z�b�g��0.1mm�P�ʂŎw��B</param>
		/// <param name="yOffset">Y���W�̃I�t�Z�b�g��0.1mm�P�ʂŎw��B</param>
		/// <param name="dark">�󎚔Z�x���w��B(1�`10)</param>
		public void Start(int xOffset, int yOffset, int dark)
		{
			Start();

			xOffset = value_check(xOffset, 0, 1040);
			yOffset = value_check(yOffset, 0, 2700);

			dark = value_check(dark, 1, 10);

			append(Encoding.Default.GetBytes(String.Format("#AX{0:0000}Y{1:0000}D{2:00}#F", xOffset, yOffset, dark)));
		}

		#endregion

		#region �ҏW�I��

		/// <summary>
		/// �ҏW�̏I����錾���܂��B
		/// </summary>
		/// <param name="copies">���s�������w��B(1�`9999)</param>
		/// <param name="cut">�J�b�g�Ԋu���w��B(0�`100)</param>
		public void End(int copies, int cut)
		{
			copies = value_check(copies, 1, 9999);

			cut = value_check(cut, 0, 100);

			append(Encoding.Default.GetBytes(String.Format("#TP#Q{0}#VP{1:000}#Z)", copies, cut)));
		}
		/// <summary>
		/// �ҏW�̏I����錾���܂��B
		/// </summary>
		/// <param name="cut">�J�b�g����ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		public void End(bool cut)
		{
			End(1, cut ? 1 : 0);
		}
		/// <summary>
		/// �ҏW�̏I����錾���܂��B
		/// </summary>
		public void End()
		{
			End(1, 0);
		}

		#endregion

		#region �N���A�L���[

		/// <summary>
		/// ���łɎ�M���Ă���f�[�^��S�ăN���A���܂��B
		/// </summary>
		public void Clear()
		{
			append(Encoding.Default.GetBytes("(#K)"));
		}

		#endregion

		#region �r���w��

		/// <summary>
		/// ������l�p�`���󎚂��܂��B
		/// </summary>
		/// <param name="x1">�r���̎n�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y1">�r���̎n�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="x2">�r���̏I�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y2">�r���̏I�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="width">�r���̕���0.1mm�P�ʂŎw��B(1�`99)</param>
		public void Frame(int x1, int y1, int x2, int y2, int width)
		{
			x1 = value_check(x1, 0, 1040);
			y1 = value_check(y1, 0, 2700);
			x2 = value_check(x2, 0, 1040);
			y2 = value_check(y2, 0, 2700);

			width = value_check(width, 1, 99);

			append(Encoding.Default.GetBytes(String.Format("#R{0:0000}{1:0000}{2:0000}{3:0000}{4:00}", x1, y1, x2, y2, width)));
		}

		#endregion

		#region �󎚃p�����[�^�w��iANK�j

		/// <summary>
		/// �󎚂��镶���̎�ނ���W�A�󎚕����A���c�{���Ȃǂ�ݒ肵�܂��B(ANK)
		/// </summary>
		/// <param name="fieldNo">�t�B�[���h���B</param>
		/// <param name="x">�����̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�����̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="font">�t�H���g�̎�ނ��w��B</param>
		/// <param name="xSize">�����̉��{�����w��B(1�`9)</param>
		/// <param name="ySize">�����̏c�{�����w��B(1�`9)</param>
		/// <param name="fontPitch">�����Ԃ̃s�b�`���h�b�g�P�ʂŎw��B</param>
		/// <param name="textData">�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="turn">��]�������w��B</param>
		/// <param name="option">�g���p�����[�^���i�[����Ă��镶����B</param>
		public void TextA(int fieldNo, int x, int y, FontTypeA font, int xSize, int ySize, FontPitch fontPitch, string textData, Turn turn, string option)
		{
			fieldNo = value_check(fieldNo, 1, 255);

			x = value_check(x, 0, 1040);
			y = value_check(y, 0, 2700);

			xSize = value_check(xSize, 1, 9);
			ySize = value_check(ySize, 1, 9);

			textData = textData.Replace('(', '{');
			textData = textData.Replace(')', '}');
			textData = textData.Replace('#', '|');

			if (option.Length > 0)
			{
				append(Encoding.Default.GetBytes(String.Format("{0:000}{1:00}#0{2:0000}{3:0000}{4}{5}0{6}{7}{8}#3{9}{10}#Y", fieldNo, Encoding.Default.GetBytes(textData).Length, x, y, System.Enum.GetName(typeof(FontTypeA), font), (int)turn, xSize, ySize, (char)fontPitch, textData, option)));
			}
			else
			{
				append(Encoding.Default.GetBytes(String.Format("{0:000}{1:00}#0{2:0000}{3:0000}{4}{5}0{6}{7}{8}#3{9}#Y", fieldNo, Encoding.Default.GetBytes(textData).Length, x, y, System.Enum.GetName(typeof(FontTypeA), font), (int)turn, xSize, ySize, (char)fontPitch, textData)));
			}
		}
		/// <summary>
		/// �󎚂��镶���̎�ނ���W�A�󎚕����A���c�{���Ȃǂ�ݒ肵�܂��B(ANK)
		/// </summary>
		/// <param name="fieldNo">�t�B�[���h���B</param>
		/// <param name="x">�����̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�����̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="font">�t�H���g�̎�ނ��w��B</param>
		/// <param name="xSize">�����̉��{�����w��B(1�`9)</param>
		/// <param name="ySize">�����̏c�{�����w��B(1�`9)</param>
		/// <param name="fontPitch">�����Ԃ̃s�b�`���h�b�g�P�ʂŎw��B</param>
		/// <param name="textData">�f�[�^���i�[����Ă��镶����B</param>
		public void TextA(int fieldNo, int x, int y, FontTypeA font, int xSize, int ySize, FontPitch fontPitch, string textData)
		{
			TextA(fieldNo, x, y, font, xSize, ySize, fontPitch, textData, Turn._0, "");
		}

		#endregion

		#region �󎚃p�����[�^�w��i�����j

		/// <summary>
		/// �󎚂��镶���̎�ނ���W�A�󎚕����A���c�{���Ȃǂ�ݒ肵�܂��B(����)
		/// </summary>
		/// <param name="fieldNo">�t�B�[���h���B</param>
		/// <param name="x">�����̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�����̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="font">�t�H���g�̎�ނ��w��B</param>
		/// <param name="xSize">�����̉��{�����w��B(1�`9)</param>
		/// <param name="ySize">�����̏c�{�����w��B(1�`9)</param>
		/// <param name="fontPitch">�����Ԃ̃s�b�`���h�b�g�P�ʂŎw��B</param>
		/// <param name="textData">�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="turn">��]�������w��B</param>
		/// <param name="option">�g���p�����[�^���i�[����Ă��镶����B</param>
		public void TextK(int fieldNo, int x, int y, FontTypeK font, int xSize, int ySize, FontPitch fontPitch, string textData, Turn turn, string option)
		{
			fieldNo = value_check(fieldNo, 1, 255);

			x = value_check(x, 0, 1040);
			y = value_check(y, 0, 2700);

			xSize = value_check(xSize, 1, 9);
			ySize = value_check(ySize, 1, 9);

			textData = textData.Replace('(', '{');
			textData = textData.Replace(')', '}');
			textData = textData.Replace('#', '|');

			if (option.Length > 0)
			{
				append(Encoding.Default.GetBytes(String.Format("{0:000}{1:00}#0{2:0000}{3:0000}{4}{5}0{6}{7}{8}#@5{9}{10}#Y", fieldNo, Encoding.Default.GetBytes(textData).Length, x, y, System.Enum.GetName(typeof(FontTypeK), font), (int)turn, xSize, ySize, (char)fontPitch, textData, option)));
			}
			else
			{
				append(Encoding.Default.GetBytes(String.Format("{0:000}{1:00}#0{2:0000}{3:0000}{4}{5}0{6}{7}{8}#@5{9}#Y", fieldNo, Encoding.Default.GetBytes(textData).Length, x, y, System.Enum.GetName(typeof(FontTypeK), font), (int)turn, xSize, ySize, (char)fontPitch, textData)));
			}
		}
		/// <summary>
		/// �󎚂��镶���̎�ނ���W�A�󎚕����A���c�{���Ȃǂ�ݒ肵�܂��B(����)
		/// </summary>
		/// <param name="fieldNo">�t�B�[���h���B</param>
		/// <param name="x">�����̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�����̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="font">�t�H���g�̎�ނ��w��B</param>
		/// <param name="xSize">�����̉��{�����w��B(1�`9)</param>
		/// <param name="ySize">�����̏c�{�����w��B(1�`9)</param>
		/// <param name="fontPitch">�����Ԃ̃s�b�`���h�b�g�P�ʂŎw��B</param>
		/// <param name="textData">�f�[�^���i�[����Ă��镶����B</param>
		public void TextK(int fieldNo, int x, int y, FontTypeK font, int xSize, int ySize, FontPitch fontPitch, string textData)
		{
			TextK(fieldNo, x, y, font, xSize, ySize, fontPitch, textData, Turn._0, "");
		}

		#endregion

		#region �o�[�R�[�h�p�����[�^�w��

		/// <summary>
		/// �󎚂���o�[�R�[�h�̎�ނ���W�A�󎚕����A���{���Ȃǂ�ݒ肵�܂��B
		/// </summary>
		/// <param name="fieldNo">�t�B�[���h���B</param>
		/// <param name="x">�o�[�R�[�h�̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�o�[�R�[�h�̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="barcodeType">�o�[�R�[�h�̎�ނ��w��B</param>
		/// <param name="width">���{�����w��B(1�`9)</param>
		/// <param name="height">�o�[�R�[�h����0.1mm�P�ʂŎw��B(1�`2700)</param>
		/// <param name="barcodeData">�o�[�R�[�h�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="subscript">�Y������t������ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		/// <param name="turn">��]�������w��B</param>
		/// <param name="option">�g���p�����[�^���i�[����Ă��镶����B</param>
		public void Barcode(int fieldNo, int x, int y, BarcodeType barcodeType, int width, int height, string barcodeData, bool subscript, Turn turn, string option)
		{
			fieldNo = value_check(fieldNo, 1, 255);

			x = value_check(x, 0, 1040);
			y = value_check(y, 0, 2700);

			width = value_check(width, 1, 9);
			height = value_check(height, 1, 2700);

			barcodeData = barcodeData.Replace('(', '{');
			barcodeData = barcodeData.Replace(')', '}');
			barcodeData = barcodeData.Replace('#', '|');

			append(Encoding.Default.GetBytes(String.Format("{0:000}{1:00}#1{2:0000}{3:0000}{4}{5}{6}{7}{8:0000}#3{9}", fieldNo, Encoding.Default.GetBytes(barcodeData).Length, x, y, (char)barcodeType, (int)turn, subscript ? 1 : 0, width, height, barcodeData)));

			if ((barcodeType == BarcodeType.JAN13 && Encoding.Default.GetBytes(barcodeData).Length == 12) || (barcodeType == BarcodeType.JAN8 && Encoding.Default.GetBytes(barcodeData).Length == 7))
			{
				append(Encoding.Default.GetBytes("#5201"));
			}
			else if (barcodeType == BarcodeType.CODE128)
			{
				append(Encoding.Default.GetBytes("#5206"));
			}

			append(Encoding.Default.GetBytes(option));
			append(Encoding.Default.GetBytes("#Y"));
		}
		/// <summary>
		/// �󎚂���o�[�R�[�h�̎�ނ���W�A�󎚕����A���{���Ȃǂ�ݒ肵�܂��B
		/// </summary>
		/// <param name="fieldNo">�t�B�[���h���B</param>
		/// <param name="x">�o�[�R�[�h�̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�o�[�R�[�h�̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="barcodeType">�o�[�R�[�h�̎�ނ��w��B</param>
		/// <param name="width">���{�����w��B(1�`9)</param>
		/// <param name="height">�o�[�R�[�h����0.1mm�P�ʂŎw��B(1�`2700)</param>
		/// <param name="barcodeData">�o�[�R�[�h�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="subscript">�Y������t������ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		public void Barcode(int fieldNo, int x, int y, BarcodeType barcodeType, int width, int height, string barcodeData, bool subscript)
		{
			Barcode(fieldNo, x, y, barcodeType, width, height, barcodeData, subscript, Turn._0, "");
		}
		/// <summary>
		/// �󎚂���o�[�R�[�h�̎�ނ���W�A�󎚕����A���{���Ȃǂ�ݒ肵�܂��B
		/// </summary>
		/// <param name="fieldNo">�t�B�[���h���B</param>
		/// <param name="x">�o�[�R�[�h�̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�o�[�R�[�h�̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="barcodeType">�o�[�R�[�h�̎�ނ��w��B</param>
		/// <param name="width">���{�����w��B(1�`9)</param>
		/// <param name="height">�o�[�R�[�h����0.1mm�P�ʂŎw��B(1�`2700)</param>
		/// <param name="barcodeData">�o�[�R�[�h�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="turn">��]�������w��B</param>
		public void Barcode(int fieldNo, int x, int y, BarcodeType barcodeType, int width, int height, string barcodeData, Turn turn)
		{
			Barcode(fieldNo, x, y, barcodeType, width, height, barcodeData, false, turn, "");
		}
		/// <summary>
		/// �󎚂���o�[�R�[�h�̎�ނ���W�A�󎚕����A���{���Ȃǂ�ݒ肵�܂��B
		/// </summary>
		/// <param name="fieldNo">�t�B�[���h���B</param>
		/// <param name="x">�o�[�R�[�h�̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�o�[�R�[�h�̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="barcodeType">�o�[�R�[�h�̎�ނ��w��B</param>
		/// <param name="width">���{�����w��B(1�`9)</param>
		/// <param name="height">�o�[�R�[�h����0.1mm�P�ʂŎw��B(1�`2700)</param>
		/// <param name="barcodeData">�o�[�R�[�h�f�[�^���i�[����Ă��镶����B</param>
		public void Barcode(int fieldNo, int x, int y, BarcodeType barcodeType, int width, int height, string barcodeData)
		{
			Barcode(fieldNo, x, y, barcodeType, width, height, barcodeData, false, Turn._0, "");
		}

		#endregion

		#region �g���o�[�R�[�h�p�����[�^

		/// <summary>
		/// �o�[�R�[�h�̃h�b�g�T�C�Y��ύX����ݒ���擾���܂��B�iJAN/EAN,UPC-A,CODE128�j
		/// </summary>
		/// <param name="width">�׃o�[�T�C�Y��0.1mm�P�ʂŎw��B</param>
		public static string BarcodeParameter1(int width)
		{
			width = value_check(width, 0, 99);

			return String.Format("#8{0:00}{1:00}{2:0000}{3:000}{4}", 0, width, 0, 0, "M");
		}
		/// <summary>
		/// �o�[�R�[�h�̃h�b�g�T�C�Y��ύX����ݒ���擾���܂��B�iJAN/EAN,UPC-A,CODE128�ȊO�j
		/// </summary>
		/// <param name="width1">�׃o�[�T�C�Y��0.1mm�P�ʂŎw��B</param>
		/// <param name="width2">���o�[�T�C�Y��0.1mm�P�ʂŎw��B</param>
		public static string BarcodeParameter2(int width1, int width2)
		{
			width1 = value_check(width1, 0, 99);
			width2 = value_check(width2, 0, 99);

			return String.Format("#8{0:00}{1:00}{2:00}{3:00}{4:00}{5}{6}", width1, width1, width2, width2, width1, 0, "M");
		}

		#endregion

		#region �i���o�����O�p�����[�^

		/// <summary>
		/// �i���o�����O�̐ݒ���擾���܂��B
		/// </summary>
		/// <param name="counter">�����l���w��B</param>
		public static string NumberingParameter(int counter)
		{
			counter = value_check(counter, -99, 99);

			return String.Format("#5{0}{1:00}{2:00}", counter < 0 ? 1 : 0, 1, Math.Abs(counter));
		}

		#endregion

		#region �������]

		/// <summary>
		/// �������������]���Ĉ󎚂��܂��B
		/// </summary>
		/// <param name="x1">���]�̎n�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y1">���]�̎n�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="x2">���]�̏I�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y2">���]�̏I�_Y���W��0.1mm�P�ʂŎw��B</param>
		public void Reverse(int x1, int y1, int x2, int y2)
		{
			x1 = value_check(x1, 0, 1040);
			y1 = value_check(y1, 0, 2700);
			x2 = value_check(x2, 0, 1040);
			y2 = value_check(y2, 0, 2700);

			append(Encoding.Default.GetBytes(String.Format("#N{0:0000}{1:0000}{2:0000}{3:0000}", x1, y1, x2, y2)));
		}

		#endregion

		#region �R�}���h�w��

		/// <summary>
		///�R�}���h�𒼐ڎw�肵�܂��B
		/// </summary>
		/// <param name="command">�R�}���h���i�[����Ă��镶����B</param>
		public void Format(string command)
		{
			append(Encoding.Default.GetBytes(command));
		}

		#endregion

		#region �ҏW�f�[�^ �v���p�e�B

		/// <summary>
		/// �ҏW�f�[�^���擾���܂��B
		/// </summary>
		public byte[] BytesData
		{
			get
			{
				byte[] data = new byte[RawDataCount];

				Array.Copy(RawData, 0, data, 0, RawDataCount);

				return data;
			}
		}

		#endregion

		#region �ҏW�f�[�^�T�C�Y �v���p�e�B

		/// <summary>
		/// �ҏW�f�[�^�̃o�C�g�����擾���܂��B
		/// </summary>
		public int BytesDataCount
		{
			get
			{
				return RawDataCount;
			}
		}

		#endregion

		#region �f�[�^�̒ǉ�(�I�[�o�[�t���[���)

		private void append(byte[] data)
		{
			byte[] temp;

			if ((RawData.Length - RawDataCount) < data.Length)
			{
				temp = new byte[RawData.Length + data.Length];

				Array.Copy(RawData, 0, temp, 0, RawDataCount);

				RawData = temp;
			}

			Array.Copy(data, 0, RawData, RawDataCount, data.Length);
			RawDataCount += data.Length;
		}

		#endregion

		#region �l�̃`�F�b�N

		private static int value_check(int value, int min, int max)
		{
			value = (value < min ? min : value);
			value = (value > max ? max : value);

			return value;
		}

		#endregion
	}
}
