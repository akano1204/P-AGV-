using System;
using System.Text;

using HokushoClass.Rs232c;
using HokushoClass.Sockets;

namespace HokushoClass.Use.Labeler
{
	/// <summary>
	/// ���x���[�N���X�i�T�g�[�p�j
	/// </summary>
	public class SATO
	{
		#region �񋓌^

		/// <summary>
		/// ACK�^�C�v
		/// </summary>
		public enum AckType
		{
			/// <summary>
			/// 
			/// </summary>
			NONE,
			/// <summary>
			/// 
			/// </summary>
			ACK,
			/// <summary>
			/// 
			/// </summary>
			NAK,
		}

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
		public SATO_Commands Commands = new SATO_Commands();

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

				status = Com.Open(H_Rs232c.PortNo._01 + (port - 1), baudRate, H_Rs232c.ByteSize._8, H_Rs232c.Parity.None, H_Rs232c.StopBits._1, H_Rs232c.FormatType.STX_ETX);

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
			return Open(port, H_Rs232c.BaudRate._9600);
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
		/// <returns>true:����,false:�ُ�</returns>
		public bool Open(string ip_address)
		{
			bool status = false;

			errors();

			if (Device == DeviceType.NONE)
			{
				Socket = new H_Socket(ip_address, 1024);

				status = Socket.Open(H_Socket.OpenMode.Client, H_Socket.FormatType.STX_ETX);

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

		#region  ���M�i�o�C�g�z��j

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

		#region  ��M�i�o�C�g�z��j

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
			return this.SendBytes(data);
		}
		/// <summary>
		/// �f�o�C�X�ɕҏW�f�[�^�𑗐M���܂��B
		/// </summary>
		/// <param name="commands">�ҏW�ς݂̃��x���[�R�}���h�ҏW�N���X</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool PrintOut(SATO_Commands commands)
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

		#region �ԓ���M

		/// <summary>
		/// �f�o�C�X����ԓ�����M���܂��B
		/// </summary>
		public AckType Receive()
		{
			int length;
			byte[] data = new byte[0];
			AckType status = AckType.NONE;

			data = ReceiveBytesDirect();

			length = data.Length;

			if (length > 0)
			{
				if (data[length - 1] == 0x06)
				{
					status = AckType.ACK;
				}
				else if (data[length - 1] == 0x15)
				{
					status = AckType.NAK;
				}
			}

			return status;
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
			byte[] data = new byte[1];

			errors();

			TimeOut = System.Environment.TickCount + timeOut;

			data[0] = 0x05;

			return SendBytesDirect(data);
		}
		/// <summary>
		/// �f�o�C�X�ɏ�ԗv���𑗐M���܂��B
		/// </summary>
		/// <returns></returns>
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
			int counter;

			return StatusReceive(out statusCode, out statusMessage, out counter);
		}
		/// <summary>
		/// �f�o�C�X�����ԗv���̕ԓ�����M���܂��B
		/// </summary>
		/// <param name="statusCode">�X�e�[�^�X�R�[�h���i�[����Ă��镶����B</param>
		/// <param name="statusMessage">�X�e�[�^�X���e���i�[����Ă��镶����B</param>
		/// <param name="statusCounter">�X�e�[�^�X���̃J�E���^�[�l�B(�X�e�[�^�XL:�\�t���������A�X�e�[�^�X3:�c��󎚖���)</param>
		/// <returns>true:��M����,false:��M�҂�</returns>
		public bool StatusReceive(out string statusCode, out string statusMessage, out int statusCounter)
		{
			int length;
			byte[] data;
			bool status = false;

			statusCode = "";
			statusMessage = "";
			statusCounter = 0;

			data = ReceiveBytes(out length);

			if (data.Length == 9)
			{
				statusCode = Encoding.Default.GetString(data, 2, 1);
				Int32.TryParse(Encoding.Default.GetString(data, 3, 6), out statusCounter);

				switch (statusCode)
				{
				case "*":
					statusMessage = "�C�j�V����";
					break;

				case "0":
					statusMessage = "�I�t���C��";
					break;

				case "A":
				case "B":
				case "C":
				case "D":
					statusMessage = "�I�����C��";
					break;

				case "G":
					statusMessage = "�I�����C��(�󎚒�)";
					break;

				case "M":
					statusMessage = "�I�����C��(�󎚑҂�)";
					break;

				case "S":
					statusMessage = "�I�����C��(�ҏW��)";
					break;

				case "o":
					statusMessage = "�I�����C��(�\�t��)";
					break;

				case "s":
					statusMessage = "�I�����C��(�ʉߒ�)";
					break;

				case "1":
				case "H":
				case "N":
				case "T":
				case "p":
				case "t":
					statusMessage = "���x���E���{���j�A�G���h";
					break;

				case "2":
				case "I":
				case "O":
				case "U":
				case "q":
				case "u":
					statusMessage = "�o�b�t�@�j�A�t��";
					break;

				case "3":
				case "P":
				case "J":
				case "V":
				case "r":
				case "v":
					statusMessage = "���x���E���{���j�A�G���h���o�b�t�@�j�A�t��";
					break;

				case "a":
					statusMessage = "��M�o�b�t�@�t��";
					break;

				case "b":
					statusMessage = "�w�b�h�I�[�v��";
					break;

				case "c":
					statusMessage = "�y�[�p�[�G���h";
					break;

				case "d":
					statusMessage = "���{���G���h";
					break;

				case "e":
					statusMessage = "���f�B�A�G���[";
					break;

				case "f":
					statusMessage = "�Z���T�[�G���[";
					break;

				case "g":
					statusMessage = "�w�b�h�G���[";
					break;

				case "h":
					statusMessage = "�J�o�[�I�[�v��";
					break;

				case "i":
					statusMessage = "�J�[�h�G���[";
					break;

				case "j":
					statusMessage = "�J�b�^�[�G���[";
					break;

				case "k":
					statusMessage = "���̑��̃G���[";
					break;

				case "4":
				case "l":
					statusMessage = "�\�t�G���[";
					break;

				default:
					statusMessage = "�d��OFF or �ʐM�ُ�";
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

		#region �L�����Z���w�著�M

		/// <summary>
		/// �f�o�C�X�ɃL�����Z���w��𑗐M���܂��B
		/// </summary>
		/// <returns>true:����,false:�ُ�</returns>
		public bool CancelSend()
		{
			byte[] data = new byte[1];

			errors();

			data[0] = 0x18;

			return SendBytesDirect(data);
		}

		#endregion

		#region �L�����Z���w���M

		/// <summary>
		/// �f�o�C�X����L�����Z���w��̕ԓ�����M���܂��B
		/// </summary>
		public AckType CancelReceive()
		{
			return Receive();
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
	/// ���x���[�R�}���h�ҏW�N���X�i�T�g�[�p�j
	/// </summary>
	public class SATO_Commands
	{
		#region �񋓌^

		/// <summary>
		/// �@�햼
		/// </summary>
		public enum Model
		{
			/// <summary>
			/// 
			/// </summary>
			MR400e,
			/// <summary>
			/// 
			/// </summary>
			MR410e,
			/// <summary>
			/// 
			/// </summary>
			MT400e,
			/// <summary>
			/// 
			/// </summary>
			MT410e,
			/// <summary>
			/// 
			/// </summary>
			SR408,
			/// <summary>
			/// 
			/// </summary>
			SR412,
			/// <summary>
			/// 
			/// </summary>
			SR424,
			/// <summary>
			/// 
			/// </summary>
			LespritT8,
			/// <summary>
			/// 
			/// </summary>
			LespritT12,
		}

		#endregion

		private const string ESC = "\x1B";

		private Model ModelType = SATO_Commands.Model.MR400e;
		private int Dpm = 8;
		private int RawDataCount = 0;
		private byte[] RawData = new byte[4096];

		#region �R���X�g���N�^

		/// <summary>
		/// ���x���[�R�}���h�ҏW�N���X
		/// </summary>
		public SATO_Commands()
		{
			ModelType = SATO_Commands.Model.MR400e;
			Dpm = 8;

			RawDataCount = 0;
			RawData = new byte[4096];
		}
		/// <summary>
		/// ���x���[�R�}���h�ҏW�N���X
		/// </summary>
		/// <param name="modelType">�T�g�[�̋@����w�肵�܂��B</param>
		public SATO_Commands(SATO_Commands.Model modelType)
		{
			ModelType = modelType;

			if (modelType == Model.MR410e || modelType == Model.MT410e || modelType == Model.SR412 || modelType == Model.LespritT12)
			{
				Dpm = 12;
			}
			else if (modelType == Model.SR424)
			{
				Dpm = 24;
			}
			else
			{
				Dpm = 8;
			}

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
			byte[] data;

			RawData = new byte[4096];

			Array.Clear(RawData, 0, RawData.Length);
			RawDataCount = 0;

			data = Encoding.Default.GetBytes(ESC + "A");

			append(data);
		}

		#endregion

		#region �ҏW�I��

		/// <summary>
		/// �ҏW�̏I����錾���܂��B
		/// </summary>
		public void End()
		{
			byte[] data;

			data = Encoding.Default.GetBytes(ESC + "Z");

			append(data);
		}

		#endregion

		#region �p���T�C�Y�w��

		/// <summary>
		/// �p���T�C�Y���w�肵�܂��B
		/// </summary>
		/// <param name="x">�p���̕���mm�P�ʂŎw��B</param>
		/// <param name="y">�p���̒�����mm�P�ʂŎw��B</param>
		public void LabelSize(int x, int y)
		{
			byte[] data;

			x *= Dpm;
			y *= Dpm;

			if (ModelType == Model.SR408 || ModelType == Model.SR412 || ModelType == Model.SR424)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("A1V{0:00000}H{1:0000}", y, x));
			}
			else
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("A1{0:0000}{1:0000}", y, x));
			}

			append(data);
		}

		#endregion

		#region ��_�␳

		/// <summary>
		/// ��_�␳���w�肵�܂��B
		/// </summary>
		/// <param name="x">�������̕␳�l��mm�P�ʂŎw��B</param>
		/// <param name="y">�c�����̕␳�l��mm�P�ʂŎw��B</param>
		public void Offset(int x, int y)
		{
			byte[] data;

			x *= Dpm;
			y *= Dpm;

			if (x >= 0 && y >= 0)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("A3V+{0}H+{1}", y, x));
			}
			else if (x >= 0)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("A3V{0}H+{1}", y, x));
			}
			else if (y >= 0)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("A3V+{0}H{1}", y, x));
			}
			else
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("A3V{0}H{1}", y, x));
			}

			append(data);
		}

		#endregion

		#region �󎚈ʒu�w��

		/// <summary>
		/// �󎚈ʒu���w�肵�܂��B
		/// </summary>
		/// <param name="x">�������̍��W��mm�P�ʂŎw��B</param>
		/// <param name="y">�c�����̍��W��mm�P�ʂŎw��B</param>
		public void Pos(int x, int y)
		{
			byte[] data;

			x *= Dpm;
			y *= Dpm;

			if (x >= 0)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("H{0}", x));

				append(data);
			}

			if (y >= 0)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("V{0}", y));

				append(data);
			}
		}
		/// <summary>
		/// �󎚈ʒu���w�肵�܂��B
		/// </summary>
		/// <param name="x">�������̍��W��mm�P�ʂŎw��B</param>
		/// <param name="y">�c�����̍��W��mm�P�ʂŎw��B</param>
		/// <param name="x_half">�������̍��W��0.5mm���Z����ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		/// <param name="y_half">�c�����̍��W��0.5mm���Z����ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		public void Pos(int x, int y, bool x_half, bool y_half)
		{
			byte[] data;

			x *= Dpm;
			y *= Dpm;
			if (x_half) x += Dpm / 2;
			if (y_half) y += Dpm / 2;

			if (x >= 0)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("H{0}", x));

				append(data);
			}

			if (y >= 0)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("V{0}", y));

				append(data);
			}
		}

		#endregion

		#region �r���A�g���󎚎w��

		/// <summary>
		/// �r���A�g���󎚂��w�肵�܂��B
		/// </summary>
		/// <param name="x">�������̐��̏I�����W��mm�P�ʂŎw��B</param>
		/// <param name="y">�c�����̐��̏I�����W��mm�P�ʂŎw��B</param>
		/// <param name="dot">���̕����h�b�g�P�ʂŎw��B</param>
		public void Line(int x, int y, int dot)
		{
			byte[] data;

			x *= Dpm;
			y *= Dpm;

			if (x > 0 && y > 0)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("FW{0:0000}V{1}H{2}", dot * 100 + dot, y, x));
			}
			else if (x > 0)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("FW{0:00}H{1}", dot, x));
			}
			else if (y > 0)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("FW{0:00}V{1}", dot, y));
			}
			else
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("FW{0:00}", dot));
			}

			append(data);
		}
		/// <summary>
		/// �r���A�g���󎚂��w�肵�܂��B
		/// </summary>
		/// <param name="x">�������̐��̏I�����W��mm�P�ʂŎw��B</param>
		/// <param name="y">�c�����̐��̏I�����W��mm�P�ʂŎw��B</param>
		/// <param name="x_half">�������̍��W��0.5mm���Z����ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		/// <param name="y_half">�c�����̍��W��0.5mm���Z����ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		/// <param name="dot">���̕����h�b�g�P�ʂŎw��B</param>
		public void Line(int x, int y, bool x_half, bool y_half, int dot)
		{
			byte[] data;

			x *= Dpm;
			y *= Dpm;
			if (x_half) x += Dpm / 2;
			if (y_half) y += Dpm / 2;

			if (x > 0 && y > 0)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("FW{0:0000}V{1}H{2}", dot * 100 + dot, y, x));
			}
			else if (x > 0)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("FW{0:00}H{1}", dot, x));
			}
			else if (y > 0)
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("FW{0:00}V{1}", dot, y));
			}
			else
			{
				data = Encoding.Default.GetBytes(ESC + String.Format("FW{0:00}", dot));
			}

			append(data);
		}

		#endregion

		#region �t�H���g�T�C�Y�w��

		/// <summary>
		/// �t�H���g�T�C�Y���w�肵�܂��B
		/// </summary>
		/// <param name="x">�������̔{�����w��B</param>
		/// <param name="y">�c�����̔{���Ŏw��B</param>
		public void FontSize(int x, int y)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(ESC + String.Format("L{0:00}{1:00}", x, y));

			append(data);
		}

		#endregion

		#region �f�[�^�w��

		/// <summary>
		/// �f�[�^���w�肵�܂��B
		/// </summary>
		/// <param name="command">�R�}���h���̃f�[�^���i�[����Ă��镶����B</param>
		/// <param name="format">�t�H�[�}�b�g���̃f�[�^���i�[����Ă��镶����B</param>
		public void FormatData(string command, string format)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(ESC + command + format);

			append(data);
		}

        /// <summary>
        /// �f�[�^���w�肵�܂��B(byte��)
        /// </summary>
        /// <param name="command">�R�}���h���̃f�[�^���i�[����Ă��镶����B</param>
        /// <param name="format">�t�H�[�}�b�g���̃f�[�^���i�[����Ă��镶����B</param>
        public void FormatData(string command, byte[] format)
        {
            byte[] data;

            data = Encoding.Default.GetBytes(ESC + command);

            append(data);
            append(format);
        }

        #endregion

        #region �~�����h�b�g�ɕϊ�

        /// <summary>
        /// �~�����[�g�����h�b�g�ɕϊ�
        /// </summary>
        /// <param name="millimeter">�ϊ�����~�����[�g��</param>
        /// <returns>�h�b�g��</returns>
        public int MmToDot(int millimeter)
		{
			return Dpm * millimeter;
		}

		#endregion

		#region �ҏW�f�[�^���擾���܂��B

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

		#region �ҏW�f�[�^�̃o�C�g�����擾���܂��B

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
	}
}
