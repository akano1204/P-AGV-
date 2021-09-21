using System;
using System.Text;

using HokushoClass.Rs232c;
using HokushoClass.Sockets;

namespace HokushoClass.Use.Labeler
{
	/// <summary>
	/// ���x���[�N���X�i�[�u���p�j
	/// </summary>
	public class ZEBRA
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
		public ZEBRA_Commands Commands = new ZEBRA_Commands();

		private DeviceType Device = DeviceType.NONE;
		private H_Rs232c Com;
		private H_Socket Socket;
		private int Error_code;
		private string Error_message;
		private int TimeOut;
		private string[] Status1;
		private string[] Status2;
		private string[] Status3;

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
		/// <param name="port">�I�[�v������V���A���f�o�C�X�̃|�[�g���B</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool Open(int port)
		{
			return Open(port, H_Rs232c.BaudRate._9600);
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

		#region	�ҏW�f�[�^���M

		/// <summary>
		/// �f�o�C�X�ɕҏW�f�[�^�𑗐M���܂��B
		/// </summary>
		/// <param name="data">���M����f�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool PrintOut(byte[] data)
		{
			return this.SendBytesDirect(data);
		}
		/// <summary>
		/// �f�o�C�X�ɕҏW�f�[�^�𑗐M���܂��B
		/// </summary>
		/// <param name="commands">�ҏW�ς݂̃��x���[�R�}���h�ҏW�N���X</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool PrintOut(ZEBRA_Commands commands)
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
			Status1 = new string[0];
			Status2 = new string[0];
			Status3 = new string[0];

			data = Encoding.Default.GetBytes("~HS");

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
			int length;
			byte[] data;
			bool status = false;
			string[] temp;

			statusCode = "";
			statusMessage = "";

			data = ReceiveBytes(out length);

			if (data.Length > 0)
			{
				temp = Encoding.Default.GetString(data).Split(',');

				if (temp.Length == 12)
				{
					Status1 = temp;
				}
				else if (temp.Length == 11)
				{
					Status2 = temp;
				}
				else if (temp.Length == 2)
				{
					Status3 = temp;
				}

				if (Status1.Length > 0 && Status2.Length > 0 && Status3.Length > 0)
				{
					statusCode = "1";
					statusMessage = "�I�����C��";

					if (Status1[1] == "1")
					{
						statusCode = "b";
						statusMessage = "�p���؂�";
					}
					else if (Status2[3] == "1" && Status2[4] == "1")
					{
						statusCode = "p";
						statusMessage = "���{���؂�";
					}
					else if (Status2[2] == "1")
					{
						statusCode = "o";
						statusMessage = "�w�b�h�I�[�v��";
					}
					else if (Status1[5] == "1")
					{
						statusCode = "f";
						statusMessage = "�o�b�t�@���t";
					}
					else if (Status1[2] == "1")
					{
						statusCode = "c";
						statusMessage = "�|�[�Y";
					}
					else if (Status2[7] == "1")
					{
						statusCode = "3";
						statusMessage = "�I�����C��(�����ҋ@��)";
					}
					else if (Convert.ToInt32(Status1[4]) > 0 || Convert.ToInt32(Status2[8]) > 0)
					{
						statusCode = "2";
						statusMessage = "�I�����C��(������)";
					}

					status = true;
				}
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

			if (status)
			{
				Status1 = new string[0];
				Status2 = new string[0];
				Status3 = new string[0];
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
	/// ���x���[�R�}���h�ҏW�N���X�i�[�u���p�j
	/// </summary>
	public class ZEBRA_Commands
	{
		#region �񋓌^

		/// <summary>
		/// �@�햼
		/// </summary>
		public enum Model
		{
			/// <summary>
			/// �𑜓x 600bpi
			/// </summary>
			_600bpi,
			/// <summary>
			/// �𑜓x 300bpi
			/// </summary>
			_300bpi,
			/// <summary>
			/// �𑜓x 203bpi
			/// </summary>
			_203bpi,
		}

		#endregion

		private int Dpm = 8;
		private int RawDataCount = 0;
		private byte[] RawData = new byte[4096];

		#region �R���X�g���N�^

		/// <summary>
		/// ���x���[�R�}���h�ҏW�N���X
		/// </summary>
		public ZEBRA_Commands()
		{
			Dpm = 8;

			RawDataCount = 0;
			RawData = new byte[4096];
		}
		/// <summary>
		/// ���x���[�R�}���h�ҏW�N���X
		/// </summary>
		/// <param name="modelType">�[�u���̋@����w�肵�܂��B</param>
		public ZEBRA_Commands(ZEBRA_Commands.Model modelType)
		{
			if (modelType == Model._300bpi)
			{
				Dpm = 12;
			}
			else if (modelType == Model._600bpi)
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

			data = Encoding.Default.GetBytes("^XA^CI15");

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

			data = Encoding.Default.GetBytes("^XZ");

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

			data = Encoding.Default.GetBytes(String.Format("^LH{0},{1}", x, y));

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

			data = Encoding.Default.GetBytes(String.Format("^FO{0},{1}", x, y));

			append(data);
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

			data = Encoding.Default.GetBytes(String.Format("^FO{0},{1}", x, y));

			append(data);
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

			data = Encoding.Default.GetBytes(String.Format("^GB{0},{1},{2}^FS", x, y, dot));

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

			data = Encoding.Default.GetBytes(String.Format("^GB{0},{1},{2}^FS", x, y, dot));

			append(data);
		}

		#endregion

		#region �f�t�H���g�t�H���g�w��

		/// <summary>
		/// �f�t�H���g�t�H���g���w�肵�܂��B
		/// </summary>
		/// <param name="type">�t�H���g�̃^�C�v���w��B(A�`Z,0�`9)</param>
		/// <param name="height">�t�H���g�̍������w��B</param>
		/// <param name="width">�t�H���g�̕����w��B</param>
		public void DefaultFont(string type, int height, int width)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(String.Format("^CF{0},{1},{2}", type, height, width));

			append(data);
		}
		/// <summary>
		/// �f�t�H���g�t�H���g���w�肵�܂��B
		/// </summary>
		/// <param name="type">�t�H���g�̃^�C�v���w��B(A�`Z,0�`9)</param>
		/// <param name="height">�t�H���g�̍������w��B</param>
		public void DefaultFont(string type, int height)
		{
			DefaultFont(type, height, 0);
		}

		#endregion

		#region �t�H���g�w��

		/// <summary>
		/// �t�H���g���w�肵�܂��B
		/// </summary>
		/// <param name="type">�t�H���g�̃^�C�v�ƌ������w��B(A�`Z,0�`9) + (N=�W��,R=90�,I=180�,B=270�) �����v����</param>
		/// <param name="height">�t�H���g�̍������w��B</param>
		/// <param name="width">�t�H���g�̕����w��B</param>
		/// <param name="font_name">�t�H���g�����w��B</param>
		public void Font(string type, int height, int width, string font_name)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(String.Format("^A@{0},{1},{2},{3}", type, height, width, font_name));

			append(data);
		}
		/// <summary>
		/// �t�H���g���w�肵�܂��B
		/// </summary>
		/// <param name="type">�t�H���g�̃^�C�v�ƌ������w��B(A�`Z,0�`9) + (N=�W��,R=90�,I=180�,B=270�) �����v����</param>
		/// <param name="height">�t�H���g�̍������w��B</param>
		/// <param name="width">�t�H���g�̕����w��B</param>
		public void Font(string type, int height, int width)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(String.Format("^A{0},{1},{2}", type, height, width));

			append(data);
		}
		/// <summary>
		/// �t�H���g���w�肵�܂��B
		/// </summary>
		/// <param name="type">�t�H���g�̃^�C�v�ƌ������w��B(A�`Z,0�`9) + (N=�W��,R=90�,I=180�,B=270�) �����v����</param>
		/// <param name="height">�t�H���g�̃t�H���g�̍������w��B</param>
		public void Font(string type, int height)
		{
			Font(type, height, 0);
		}

		#endregion

		#region �p�����w��

		/// <summary>
		/// �p�������w�肵�܂��B(�t�H���g�w�肠��)
		/// </summary>
		/// <param name="type">�t�H���g�̃^�C�v�ƌ������w��B(A�`Z,0�`9) + (N=�W��,R=90�,I=180�,B=270�) �����v����</param>
		/// <param name="height">�t�H���g�̍������w��B</param>
		/// <param name="width">�t�H���g�̕����w��B</param>
		/// <param name="text_data">�p�������i�[����Ă��镶����B</param>
		public void TextA(string type, int height, int width, string text_data)
		{
			byte[] data;

			if (type != "")
			{
				Font(type, height, width);
			}

			data = Encoding.Default.GetBytes("^CI12^FD" + text_data + "^FS^CI15");

			append(data);
		}
		/// <summary>
		/// �p�������w�肵�܂��B(�t�H���g�w�肠��)
		/// </summary>
		/// <param name="type">�t�H���g�̃^�C�v�ƌ������w��B(A�`Z,0�`9) + (N=�W��,R=90�,I=180�,B=270�) �����v����</param>
		/// <param name="height">�t�H���g�̍������w��B</param>
		/// <param name="text_data">�p�������i�[����Ă��镶����B</param>
		public void TextA(string type, int height, string text_data)
		{
			TextA(type, height, 0, text_data);
		}
		/// <summary>
		/// �p�������w�肵�܂��B
		/// </summary>
		/// <param name="text_data">�p�������i�[����Ă��镶����B</param>
		public void TextA(string text_data)
		{
			TextA("", 0, 0, text_data);
		}

		#endregion

		#region �����w��

		/// <summary>
		/// �������w�肵�܂��B(�t�H���g�w�肠��)
		/// </summary>
		/// <param name="type">�t�H���g�̃^�C�v�ƌ������w��B(A�`Z,0�`9) + (N=�W��,R=90�,I=180�,B=270�) �����v����</param>
		/// <param name="height">�t�H���g�̍������w��B</param>
		/// <param name="width">�t�H���g�̕����w��B</param>
		/// <param name="text_data">�������i�[����Ă��镶����B</param>
		public void TextB(string type, int height, int width, string text_data)
		{
			byte[] data;

			if (type != "")
			{
				Font(type, height, width);
			}

			data = Encoding.Default.GetBytes("^FD" + text_data + "^FS");

			append(data);
		}
		/// <summary>
		/// �������w�肵�܂��B
		/// </summary>
		/// <param name="text_data">�������i�[����Ă��镶����B</param>
		public void TextB(string text_data)
		{
			TextB("", 0, 0, text_data);
		}

		#endregion

		#region �����w��2

		/// <summary>
		/// �������w�肵�܂��B(�t�H���g�w�肠��)
		/// </summary>
		/// <param name="type">�t�H���g�̃^�C�v�ƌ������w��B(A�`Z,0�`9) + (N=�W��,R=90�,I=180�,B=270�) �����v����</param>
		/// <param name="height">�t�H���g�̍������w��B</param>
		/// <param name="width">�t�H���g�̕����w��B</param>
		/// <param name="text_data">�������i�[����Ă��镶����B</param>
		public void TextC(string type, int height, int width, string text_data)
		{
			StringBuilder work = new StringBuilder();
			byte[] data;

			if (type != "")
			{
				Font(type, height, width);
			}

			data = Encoding.Default.GetBytes(text_data);

			foreach (byte temp in data)
			{
				work.Append(String.Format("_{0:X2}", temp));
			}

			data = Encoding.Default.GetBytes("^FH^FD" + work.ToString() + "^FS");

			append(data);
		}
		/// <summary>
		/// �������w�肵�܂��B
		/// </summary>
		/// <param name="text_data">�������i�[����Ă��镶����B</param>
		public void TextC(string text_data)
		{
			TextC("", 0, 0, text_data);
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

			data = Encoding.Default.GetBytes(command + format);

			append(data);
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
	}
}
