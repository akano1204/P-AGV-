using System;
using System.Text;

using HokushoClass.Rs232c;
using HokushoClass.Sockets;

namespace HokushoClass.Use.Labeler
{
	/// <summary>
	/// ���x���[�N���X�i���Ńe�b�N�p�j
	/// </summary>
	public class TEC
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
		public TEC_Commands Commands = new TEC_Commands();

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

				status = Com.Open(H_Rs232c.PortNo._01 + (port - 1), baudRate, H_Rs232c.ByteSize._8, H_Rs232c.Parity.None, H_Rs232c.StopBits._1, H_Rs232c.FormatType.CR_LF);

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
			return this.SendBytesDirect(data);
		}
		/// <summary>
		/// �f�o�C�X�ɕҏW�f�[�^�𑗐M���܂��B
		/// </summary>
		/// <param name="commands">�ҏW�ς݂̃��x���[�R�}���h�ҏW�N���X</param>
		/// <returns>true:����,false:�ُ�</returns>
		public bool PrintOut(TEC_Commands commands)
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
			byte[] data;

			errors();

			TimeOut = System.Environment.TickCount + timeOut;

			data = Encoding.Default.GetBytes("{WR|}");

			return SendBytesDirect(data);
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

			data = Encoding.Default.GetBytes("{WS|}");

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

			statusCode = "";
			statusMessage = "";

			data = ReceiveBytes(out length);

			if (data.Length == 11)
			{
				statusCode = Encoding.Default.GetString(data, 4, 1);

				if (statusCode == "1")
				{
					statusCode = Encoding.Default.GetString(data, 2, 2);

					switch (statusCode)
					{
					case "00":
						statusMessage = "�ҋ@��";
						break;

					case "01":
						statusMessage = "�w�b�h�I�[�v��";
						break;

					case "02":
						statusMessage = "���쒆";
						break;

					case "04":
						statusMessage = "�|�[�Y";
						break;

					case "05":
						statusMessage = "�����҂�";
						break;

					case "06":
						statusMessage = "�R�}���h�G���[";
						break;

					case "07":
						statusMessage = "�ʐM�G���[";
						break;

					case "11":
						statusMessage = "�y�[�p�[�G���[";
						break;

					case "12":
						statusMessage = "�J�b�^�[�G���[";
						break;

					case "13":
						statusMessage = "�y�[�p�[�G���h";
						break;

					case "15":
						statusMessage = "�w�b�h�I�[�v��(���쒆)";
						break;

					case "17":
						statusMessage = "�T�[�}���w�b�h�f���G���[";
						break;

					case "18":
						statusMessage = "�T�[�}���w�b�h���x�G���[";
						break;

					case "22":
						statusMessage = "�J�o�[�I�[�v��(���쒆)";
						break;

					case "40":
						statusMessage = "���s�I��";
						break;

					case "41":
						statusMessage = "�t�B�[�h�I��";
						break;

					default:
						statusMessage = "���̑��G���[[" + statusCode + "]";
						break;
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
	/// ���x���[�R�}���h�ҏW�N���X�i���Ńe�b�N�p�j
	/// </summary>
	public class TEC_Commands
	{
		#region �񋓌^

		/// <summary>
		/// �@�햼
		/// </summary>
		public enum Model
		{
			/// <summary>
			/// B-SA4T
			/// </summary>
			B_SA4T,
		}

		/// <summary>
		/// �Z���T�[
		/// </summary>
		public enum Sensor
		{
			/// <summary>
			/// �Z���T�[�Ȃ�
			/// </summary>
			None = 0,
			/// <summary>
			/// ���˃Z���T�[
			/// </summary>
			Reflect = 1,
			/// <summary>
			/// ���߃Z���T�[
			/// </summary>
			Transmit = 2,
		}

		/// <summary>
		/// ���x��
		/// </summary>
		public enum Label
		{
			/// <summary>
			/// ���M��
			/// </summary>
			ThermalPaper = 0,
			/// <summary>
			/// �M�]��
			/// </summary>
			HeatTranscription = 1,
		}

		/// <summary>
		/// ���s���[�h
		/// </summary>
		public enum PrintMode
		{
			/// <summary>
			/// �A�����
			/// </summary>
			C,
			/// <summary>
			/// �������s
			/// </summary>
			D,
			/// <summary>
			/// �������s(�Z���T�[����)
			/// </summary>
			E,
		}

		/// <summary>
		/// �󎚕���
		/// </summary>
		public enum Direction
		{
			/// <summary>
			/// ���o����
			/// </summary>
			Standard = 1,
			/// <summary>
			/// �K�o����
			/// </summary>
			Back = 0,
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
			_90 = 1,
			/// <summary>
			/// 180߉E��]
			/// </summary>
			_180 = 2,
			/// <summary>
			/// 270߉E��]
			/// </summary>
			_270 = 3,
		}

		/// <summary>
		/// �r�b�g�}�b�v�t�H���g
		/// </summary>
		public enum BitmapFonts
		{
			/// <summary>
			/// �^�C���X���[�}��(��)
			/// </summary>
			A,
			/// <summary>
			/// �^�C���X���[�}��(��)
			/// </summary>
			B,
			/// <summary>
			/// �^�C���X���[�}��(����)
			/// </summary>
			C,
			/// <summary>
			/// �^�C���X���[�}��(����)
			/// </summary>
			D,
			/// <summary>
			/// �^�C���X���[�}��(����)
			/// </summary>
			E,
			/// <summary>
			/// �^�C���X���[�}��(�Α�)
			/// </summary>
			F,
			/// <summary>
			/// �w���x�`�J(��)
			/// </summary>
			G,
			/// <summary>
			/// �w���x�`�J(��)
			/// </summary>
			H,
			/// <summary>
			/// �w���x�`�J(��)
			/// </summary>
			I,
			/// <summary>
			/// �w���x�`�J(����)
			/// </summary>
			J,
			/// <summary>
			/// �w���x�`�J(����)
			/// </summary>
			K,
			/// <summary>
			/// �w���x�`�J(�Α�)
			/// </summary>
			L,
			/// <summary>
			/// �v���[���e�[�V����(����)
			/// </summary>
			M,
			/// <summary>
			/// ���^�[�S�V�b�N(��)
			/// </summary>
			N,
			/// <summary>
			/// �v���X�e�[�W�G���[�g(��)
			/// </summary>
			O,
			/// <summary>
			/// �v���X�e�[�W�G���[�g(����)
			/// </summary>
			P,
			/// <summary>
			/// �N�[���G(��)
			/// </summary>
			Q,
			/// <summary>
			/// �N�[���G(����)
			/// </summary>
			R,
			/// <summary>
			/// OCR-A
			/// </summary>
			S,
			/// <summary>
			/// OCR-B
			/// </summary>
			T,
			/// <summary>
			/// ����(16x16) �p�S�V�b�N��
			/// </summary>
			U,
			/// <summary>
			/// ����(24x24) �p�S�V�b�N��
			/// </summary>
			V,
			/// <summary>
			/// ����(32x32) �p�S�V�b�N��
			/// </summary>
			W,
			/// <summary>
			/// ����(48x48) �p�S�V�b�N��
			/// </summary>
			X,
			/// <summary>
			/// �S�V�b�N725�u���b�N
			/// </summary>
			q,
			/// <summary>
			/// ����(24x24) ������
			/// </summary>
			v,
			/// <summary>
			/// ����(32x32) ������
			/// </summary>
			w,
		}

		/// <summary>
		/// �A�E�g���C���t�H���g
		/// </summary>
		public enum OutlineFonts
		{
			/// <summary>
			/// TEC FONT1(�w���׃`�J[����])
			/// </summary>
			A,
			/// <summary>
			/// TEC FONT1(�w���׃`�J[����]�v���|�[�V���i��)
			/// </summary>
			B,
			/// <summary>
			/// ���i�t�H���g1
			/// </summary>
			E,
			/// <summary>
			/// ���i�t�H���g2
			/// </summary>
			F,
			/// <summary>
			/// ���i�t�H���g3
			/// </summary>
			G,
			/// <summary>
			/// DUTCH801�{�[���h(�^�C���X���[�}�� �v���|�[�V���i��)
			/// </summary>
			H,
			/// <summary>
			/// BRUSH738���M�����[(�|�b�v �v���|�[�V���i��)
			/// </summary>
			I,
			/// <summary>
			/// GOTHIC725�u���b�N(�v���|�[�V���i��)
			/// </summary>
			J,
		}

		/// <summary>
		/// �o�[�R�[�h��(JAN,UPC,CODE93,CODE128)
		/// </summary>
		public enum BarcodeType1
		{
			/// <summary>
			/// JAN8/EAN8
			/// </summary>
			JAN8 = '0',
			/// <summary>
			/// JAN13/EAN13
			/// </summary>
			JAN13 = '5',
			/// <summary>
			/// UPC-E
			/// </summary>
			UPC_E = '6',
			/// <summary>
			/// UPC-A
			/// </summary>
			UPC_A = 'K',
			/// <summary>
			/// CODE128
			/// </summary>
			CODE128 = 'A',
			/// <summary>
			/// CODE93
			/// </summary>
			CODE93 = 'C',
		}

		/// <summary>
		/// �o�[�R�[�h��(ITF,CODE39,NW7)
		/// </summary>
		public enum BarcodeType2
		{
			/// <summary>
			/// �C���^�[���[�u�h2of5
			/// </summary>
			ITF = '2',
			/// <summary>
			/// CODE39(�X�^���_�[�h)
			/// </summary>
			CODE39 = '3',
			/// <summary>
			/// NW7
			/// </summary>
			NW7 = '4',
			/// <summary>
			/// CODE39(�t���A�X�L�[)
			/// </summary>
			CODE39_FULL = 'B',
		}

		#endregion

		private Model ModelType = TEC_Commands.Model.B_SA4T;
		private Sensor SensorType = TEC_Commands.Sensor.Transmit;
		private Label LabelType = TEC_Commands.Label.ThermalPaper;
		private int RawDataCount = 0;
		private byte[] RawData = new byte[4096];

		#region �R���X�g���N�^

		/// <summary>
		/// ���x���[�R�}���h�ҏW�N���X
		/// </summary>
		public TEC_Commands()
		{
			ModelType = TEC_Commands.Model.B_SA4T;
			SensorType = TEC_Commands.Sensor.Transmit;
			LabelType = TEC_Commands.Label.ThermalPaper;

			RawDataCount = 0;
			RawData = new byte[4096];
		}
		/// <summary>
		/// ���x���[�R�}���h�ҏW�N���X
		/// </summary>
		/// <param name="modelType">�@����w�肵�܂��B</param>
		/// <param name="sensorType">�Z���T�[��ʂ��w�肵�܂��B</param>
		/// <param name="labelType">���x����ʂ��w�肵�܂��B</param>
		public TEC_Commands(TEC_Commands.Model modelType, TEC_Commands.Sensor sensorType, TEC_Commands.Label labelType)
		{
			ModelType = modelType;
			SensorType = sensorType;
			LabelType = labelType;

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
		}
		/// <summary>
		/// �ҏW�̊J�n��錾���܂��B
		/// </summary>
		/// <param name="width">�p���̕���0.1mm�P�ʂŎw��B</param>
		/// <param name="length">�p���̒�����0.1mm�P�ʂŎw��B</param>
		/// <param name="gap">�p���̌��Ԃ�0.1mm�P�ʂŎw��B</param>
		public void Start(int width, int length, int gap)
		{
			byte[] data;

			Start();

			data = Encoding.Default.GetBytes(String.Format("{{D{0:0000},{1:0000},{2:0000}|}}{{C|}}", length + gap, width, length));

			append(data);
		}

		#endregion

		#region �ҏW�I��

		/// <summary>
		/// �ҏW�̏I����錾���܂��B
		/// </summary>
		/// <param name="direction">�󎚕������w��B</param>
		/// <param name="speed">�󎚃X�s�[�h���w��B(2,4,6)</param>
		/// <param name="copies">���s�������w��B(1�`9999)</param>
		/// <param name="cut">�J�b�g�Ԋu���w��B(0�`100)</param>
		/// <param name="printMode">���s���[�h���w��B</param>
		public void End(Direction direction, int speed, int copies, int cut, PrintMode printMode)
		{
			byte[] data;

			copies = (copies < 1 ? 1 : copies);
			copies = (copies > 9999 ? 9999 : copies);

			cut = (cut < 0 ? 0 : cut);
			cut = (cut > 100 ? 100 : cut);

			speed = (speed < 2 ? 2 : speed);
			speed = (speed > 6 ? 6 : speed);
			speed = speed / 2 * 2;

			data = Encoding.Default.GetBytes(String.Format("{{XS;I,{0:0000},{1:000}{2}{3}{4}{5}{6}0|}}", copies, cut, (int)SensorType, System.Enum.GetName(typeof(PrintMode), printMode), speed, (int)LabelType, (int)direction));

			append(data);
		}
		/// <summary>
		/// �ҏW�̏I����錾���܂��B
		/// </summary>
		/// <param name="direction">�󎚕������w��B</param>
		/// <param name="speed">�󎚃X�s�[�h���w��B(2,4,6)</param>
		/// <param name="copies">���s�������w��B(1�`9999)</param>
		/// <param name="cut">�J�b�g�Ԋu���w��B(0�`100)</param>
		public void End(Direction direction, int speed, int copies, int cut)
		{
			End(direction, speed, copies, cut, PrintMode.C);
		}
		/// <summary>
		/// �ҏW�̏I����錾���܂��B
		/// </summary>
		/// <param name="cut">�J�b�g����ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		public void End(bool cut)
		{
			End(Direction.Standard, 6, 1, cut ? 1 : 0);
		}
		/// <summary>
		/// �ҏW�̏I����錾���܂��B
		/// </summary>
		public void End()
		{
			End(Direction.Standard, 6, 1, 0);
		}

		#endregion

		#region �p���T�C�Y�w��

		/// <summary>
		/// �p���T�C�Y���w�肵�܂��B
		/// </summary>
		/// <param name="width">�p���̕���0.1mm�P�ʂŎw��B</param>
		/// <param name="length">�p���̒�����0.1mm�P�ʂŎw��B</param>
		/// <param name="gap">�p���̌��Ԃ�0.1mm�P�ʂŎw��B</param>
		public void LabelSize(int width, int length, int gap)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(String.Format("{{D{0:0000},{1:0000},{2:0000}|}}", length + gap, width, length));

			append(data);
		}

		#endregion

		#region �C���[�W�o�b�t�@�N���A

		/// <summary>
		/// �C���[�W�o�b�t�@���N���A���܂��B
		/// </summary>
		public void Clear()
		{
			byte[] data;

			data = Encoding.Default.GetBytes("{C|}");

			append(data);
		}

		#endregion

		#region �r���󎚎w��

		/// <summary>
		/// �r�����w�肵�܂��B
		/// </summary>
		/// <param name="x1">�r���̎n�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y1">�r���̎n�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="x2">�r���̏I�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y2">�r���̏I�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="width">�r���̕���0.1mm�P�ʂŎw��B(1�`9)</param>
		public void Line(int x1, int y1, int x2, int y2, int width)
		{
			byte[] data;

			width = (width < 1 ? 1 : width);
			width = (width > 9 ? 9 : width);

			data = Encoding.Default.GetBytes(String.Format("{{LC;{0:0000},{1:0000},{2:0000},{3:0000},0,{4}|}}", x1, y1, x2, y2, width));

			append(data);
		}

		#endregion

		#region �g���󎚎w��

		/// <summary>
		/// �g�����w�肵�܂��B
		/// </summary>
		/// <param name="x1">�g���̎n�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y1">�g���̎n�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="x2">�g���̏I�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y2">�g���̏I�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="width">�g���̕���0.1mm�P�ʂŎw��B(1�`9)</param>
		/// <param name="round">�p�ۂߔ��a��0.1mm�P�ʂŎw��B</param>
		public void Frame(int x1, int y1, int x2, int y2, int width, int round)
		{
			byte[] data;

			width = (width < 1 ? 1 : width);
			width = (width > 9 ? 9 : width);

			data = Encoding.Default.GetBytes(String.Format("{{LC;{0:0000},{1:0000},{2:0000},{3:0000},1,{4},{5:000}|}}", x1, y1, x2, y2, width, round));

			append(data);
		}
		/// <summary>
		/// �g�����w�肵�܂��B
		/// </summary>
		/// <param name="x1">�g���̎n�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y1">�g���̎n�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="x2">�g���̏I�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y2">�g���̏I�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="width">�g���̕���0.1mm�P�ʂŎw��B(1�`9)</param>
		public void Frame(int x1, int y1, int x2, int y2, int width)
		{
			Frame(x1, y1, x2, y2, width, 0);
		}

		#endregion

		#region �r�b�g�}�b�v�t�H���g�w��

		/// <summary>
		/// �����i�r�b�g�}�b�v�t�H���g�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�����̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�����̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="font">�t�H���g�̎�ނ��w��B</param>
		/// <param name="xSize">�����̉��{�����w��B(1�`9)</param>
		/// <param name="ySize">�����̏c�{�����w��B(1�`9)</param>
		/// <param name="fontPitch">�����Ԃ̃s�b�`���h�b�g�P�ʂŎw��B(-99�`99)</param>
		/// <param name="textData">�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="turn">��]�������w��B</param>
		/// <param name="fontOption">����������w��B</param>
		public void Text1(int x, int y, BitmapFonts font, int xSize, int ySize, int fontPitch, string textData, Turn turn, string fontOption)
		{
			byte[] data;
			string pitch = "+00";
			string option = "B";

			xSize = (xSize < 1 ? 1 : xSize);
			xSize = (xSize > 9 ? 9 : xSize);

			ySize = (ySize < 1 ? 1 : ySize);
			ySize = (ySize > 9 ? 9 : ySize);

			if (fontPitch > 0 && fontPitch < 100)
			{
				pitch = String.Format("+{0:00}", fontPitch);
			}
			else if (fontPitch < 0 && fontPitch > -100)
			{
				pitch = String.Format("{0:00}", fontPitch);
			}

			if (fontOption.Length > 0)
			{
				option = fontOption;
			}

			data = Encoding.Default.GetBytes(String.Format("{{PC{0:000};{1:0000},{2:0000},{3},{4},{5},{6},{7}{8},{9}={10}|}}", 0, x, y, xSize, ySize, System.Enum.GetName(typeof(BitmapFonts), font), pitch, (int)turn, (int)turn, option, textData));

			append(data);
		}
		/// <summary>
		/// �����i�r�b�g�}�b�v�t�H���g�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�����̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�����̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="font">�t�H���g�̎�ނ��w��B</param>
		/// <param name="xSize">�����̉��{�����w��B(1�`9)</param>
		/// <param name="ySize">�����̏c�{�����w��B(1�`9)</param>
		/// <param name="textData">�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="turn">��]�������w��B</param>
		public void Text1(int x, int y, BitmapFonts font, int xSize, int ySize, string textData, Turn turn)
		{
			Text1(x, y, font, xSize, ySize, 0, textData, turn, "");
		}
		/// <summary>
		/// �����i�r�b�g�}�b�v�t�H���g�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�����̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�����̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="font">�t�H���g�̎�ނ��w��B</param>
		/// <param name="xSize">�����̉��{�����w��B(1�`9)</param>
		/// <param name="ySize">�����̏c�{�����w��B(1�`9)</param>
		/// <param name="textData">�f�[�^���i�[����Ă��镶����B</param>
		public void Text1(int x, int y, BitmapFonts font, int xSize, int ySize, string textData)
		{
			Text1(x, y, font, xSize, ySize, 0, textData, Turn._0, "");
		}

		#endregion

		#region �A�E�g���C���t�H���g�w��

		/// <summary>
		/// �����i�A�E�g���C���t�H���g�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�����̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�����̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="font">�t�H���g�̎�ނ��w��B</param>
		/// <param name="width">��������0.1mm�P�ʂŎw��B(20�`850)</param>
		/// <param name="height">��������0.1mm�P�ʂŎw��B(20�`850)</param>
		/// <param name="fontPitch">�����Ԃ̃s�b�`���h�b�g�P�ʂŎw��B(-512�`512)</param>
		/// <param name="textData">�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="turn">��]�������w��B</param>
		/// <param name="fontOption">����������w��B</param>
		public void Text2(int x, int y, OutlineFonts font, int width, int height, int fontPitch, string textData, Turn turn, string fontOption)
		{
			byte[] data;
			string pitch = "+000";
			string option = "B";

			width = (width < 20 ? 20 : width);
			width = (width > 850 ? 850 : width);

			height = (height < 20 ? 20 : height);
			height = (height > 850 ? 850 : height);

			if (fontPitch > 0 && fontPitch <= 512)
			{
				pitch = String.Format("+{0:000}", fontPitch);
			}
			else if (fontPitch < 0 && fontPitch >= -512)
			{
				pitch = String.Format("{0:000}", fontPitch);
			}

			if (fontOption.Length > 0)
			{
				option = fontOption;
			}

			data = Encoding.Default.GetBytes(String.Format("{{PV{0:00};{1:0000},{2:0000},{3:0000},{4:0000},{5},{6},{7}{8},{9}={10}|}}", 0, x, y, width, height, System.Enum.GetName(typeof(OutlineFonts), font), pitch, (int)turn, (int)turn, option, textData));

			append(data);
		}
		/// <summary>
		/// �����i�A�E�g���C���t�H���g�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�����̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�����̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="width">��������0.1mm�P�ʂŎw��B(20�`850)</param>
		/// <param name="height">��������0.1mm�P�ʂŎw��B(20�`850)</param>
		/// <param name="font">�t�H���g�̎�ނ��w��B</param>
		/// <param name="textData">�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="turn">��]�������w��B</param>
		public void Text2(int x, int y, OutlineFonts font, int width, int height, string textData, Turn turn)
		{
			Text2(x, y, font, width, height, 0, textData, turn, "");
		}
		/// <summary>
		/// �����i�A�E�g���C���t�H���g�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�����̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�����̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="width">��������0.1mm�P�ʂŎw��B(20�`850)</param>
		/// <param name="height">��������0.1mm�P�ʂŎw��B(20�`850)</param>
		/// <param name="font">�t�H���g�̎�ނ��w��B</param>
		/// <param name="textData">�f�[�^���i�[����Ă��镶����B</param>
		public void Text2(int x, int y, OutlineFonts font, int width, int height, string textData)
		{
			Text2(x, y, font, width, height, 0, textData, Turn._0, "");
		}

		#endregion

		#region �o�[�R�[�h�w��iJAN,UPC,CODE93,CODE128�j

		/// <summary>
		/// �o�[�R�[�h�iJAN,UPC,CODE93,CODE128�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�o�[�R�[�h�̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�o�[�R�[�h�̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="barcodeType">�o�[�R�[�h�̎�ނ��w��B</param>
		/// <param name="checkdigit">�`�F�b�N�f�B�W�b�g��t������ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		/// <param name="width">���W���[�������h�b�g�P�ʂŎw��B(1�`15)</param>
		/// <param name="height">�o�[�R�[�h����0.1mm�P�ʂŎw��B(0�`1000)</param>
		/// <param name="barcodeData">�o�[�R�[�h�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="subscript">�Y������t������ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		/// <param name="turn">��]�������w��B</param>
		public void Barcode1(int x, int y, BarcodeType1 barcodeType, bool checkdigit, int width, int height, string barcodeData, bool subscript, Turn turn)
		{
			byte[] data;

			width = (width < 1 ? 1 : width);
			width = (width > 15 ? 15 : width);

			height = (height < 0 ? 0 : height);
			height = (height > 1000 ? 1000 : height);

			if (subscript)
			{
				data = Encoding.Default.GetBytes(String.Format("{{XB{0:00};{1:0000},{2:0000},{3},{4},{5:00},{6},{7:0000},+0000000000,015,1,00={8}|}}", 0, x, y, (char)barcodeType, checkdigit ? 3 : 1, width, (int)turn, height, barcodeData));
			}
			else
			{
				data = Encoding.Default.GetBytes(String.Format("{{XB{0:00};{1:0000},{2:0000},{3},{4},{5:00},{6},{7:0000}={8}|}}", 0, x, y, (char)barcodeType, checkdigit ? 3 : 1, width, (int)turn, height, barcodeData));
			}

			append(data);
		}
		/// <summary>
		/// �o�[�R�[�h�iJAN,UPC,CODE93,CODE128�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�o�[�R�[�h�̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�o�[�R�[�h�̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="barcodeType">�o�[�R�[�h�̎�ނ��w��B</param>
		/// <param name="width">���W���[�������h�b�g�P�ʂŎw��B(1�`15)</param>
		/// <param name="height">�o�[�R�[�h����0.1mm�P�ʂŎw��B(0�`1000)</param>
		/// <param name="barcodeData">�o�[�R�[�h�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="subscript">�Y������t������ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		public void Barcode1(int x, int y, BarcodeType1 barcodeType, int width, int height, string barcodeData, bool subscript)
		{
			Barcode1(x, y, barcodeType, false, width, height, barcodeData, subscript, Turn._0);
		}
		/// <summary>
		/// �o�[�R�[�h�iJAN,UPC,CODE93,CODE128�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�o�[�R�[�h�̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�o�[�R�[�h�̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="barcodeType">�o�[�R�[�h�̎�ނ��w��B</param>
		/// <param name="width">���W���[�������h�b�g�P�ʂŎw��B(1�`15)</param>
		/// <param name="height">�o�[�R�[�h����0.1mm�P�ʂŎw��B(0�`1000)</param>
		/// <param name="barcodeData">�o�[�R�[�h�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="turn">��]�������w��B</param>
		public void Barcode1(int x, int y, BarcodeType1 barcodeType, int width, int height, string barcodeData, Turn turn)
		{
			Barcode1(x, y, barcodeType, false, width, height, barcodeData, false, turn);
		}
		/// <summary>
		/// �o�[�R�[�h�iJAN,UPC,CODE93,CODE128�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�o�[�R�[�h�̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�o�[�R�[�h�̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="barcodeType">�o�[�R�[�h�̎�ނ��w��B</param>
		/// <param name="width">���W���[�������h�b�g�P�ʂŎw��B(1�`15)</param>
		/// <param name="height">�o�[�R�[�h����0.1mm�P�ʂŎw��B(0�`1000)</param>
		/// <param name="barcodeData">�o�[�R�[�h�f�[�^���i�[����Ă��镶����B</param>
		public void Barcode1(int x, int y, BarcodeType1 barcodeType, int width, int height, string barcodeData)
		{
			Barcode1(x, y, barcodeType, false, width, height, barcodeData, false, Turn._0);
		}

		#endregion

		#region �o�[�R�[�h�w��iITF,CODE39,NW7�j

		/// <summary>
		/// �o�[�R�[�h�iITF,CODE39,NW7�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�o�[�R�[�h�̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�o�[�R�[�h�̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="barcodeType">�o�[�R�[�h�̎�ނ��w��B</param>
		/// <param name="checkdigit">�`�F�b�N�f�B�W�b�g��t������ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		/// <param name="narrowBar">�׃o�[�����h�b�g�P�ʂŎw��B(1�`99)</param>
		/// <param name="wideBar">���o�[�����h�b�g�P�ʂŎw��B(1�`99)</param>
		/// <param name="height">�o�[�R�[�h����0.1mm�P�ʂŎw��B(0�`1000)</param>
		/// <param name="barcodeData">�o�[�R�[�h�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="subscript">�Y������t������ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		/// <param name="turn">��]�������w��B</param>
		public void Barcode2(int x, int y, BarcodeType2 barcodeType, bool checkdigit, int narrowBar, int wideBar, int height, string barcodeData, bool subscript, Turn turn)
		{
			byte[] data;
			int space = 0;

			narrowBar = (narrowBar < 1 ? 1 : narrowBar);
			narrowBar = (narrowBar > 99 ? 99 : narrowBar);

			wideBar = (wideBar < 1 ? 1 : wideBar);
			wideBar = (wideBar > 99 ? 99 : wideBar);

			height = (height < 0 ? 0 : height);
			height = (height > 1000 ? 1000 : height);

			if (barcodeType != BarcodeType2.ITF)
			{
				space = narrowBar;
			}

			if (subscript)
			{
				data = Encoding.Default.GetBytes(String.Format("{{XB{0:00};{1:0000},{2:0000},{3},{4},{5:00},{6:00},{7:00},{8:00},{9:00},{10},{11:0000},+0000000000,1,00={12}|}}", 0, x, y, (char)barcodeType, checkdigit ? 3 : 1, narrowBar, narrowBar, wideBar, wideBar, space, (int)turn, height, barcodeData));
			}
			else
			{
				data = Encoding.Default.GetBytes(String.Format("{{XB{0:00};{1:0000},{2:0000},{3},{4},{5:00},{6:00},{7:00},{8:00},{9:00},{10},{11:0000}={12}|}}", 0, x, y, (char)barcodeType, checkdigit ? 3 : 1, narrowBar, narrowBar, wideBar, wideBar, space, (int)turn, height, barcodeData));
			}

			append(data);
		}
		/// <summary>
		/// �o�[�R�[�h�iITF,CODE39,NW7�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�o�[�R�[�h�̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�o�[�R�[�h�̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="barcodeType">�o�[�R�[�h�̎�ނ��w��B</param>
		/// <param name="narrowBar">�׃o�[�����h�b�g�P�ʂŎw��B(1�`99)</param>
		/// <param name="wideBar">���o�[�����h�b�g�P�ʂŎw��B(1�`99)</param>
		/// <param name="height">�o�[�R�[�h����0.1mm�P�ʂŎw��B(0�`1000)</param>
		/// <param name="barcodeData">�o�[�R�[�h�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="subscript">�Y������t������ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		public void Barcode2(int x, int y, BarcodeType2 barcodeType, int narrowBar, int wideBar, int height, string barcodeData, bool subscript)
		{
			Barcode2(x, y, barcodeType, false, narrowBar, wideBar, height, barcodeData, subscript, Turn._0);
		}
		/// <summary>
		/// �o�[�R�[�h�iITF,CODE39,NW7�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�o�[�R�[�h�̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�o�[�R�[�h�̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="barcodeType">�o�[�R�[�h�̎�ނ��w��B</param>
		/// <param name="narrowBar">�׃o�[�����h�b�g�P�ʂŎw��B(1�`99)</param>
		/// <param name="wideBar">���o�[�����h�b�g�P�ʂŎw��B(1�`99)</param>
		/// <param name="height">�o�[�R�[�h����0.1mm�P�ʂŎw��B(0�`1000)</param>
		/// <param name="barcodeData">�o�[�R�[�h�f�[�^���i�[����Ă��镶����B</param>
		/// <param name="turn">��]�������w��B</param>
		public void Barcode2(int x, int y, BarcodeType2 barcodeType, int narrowBar, int wideBar, int height, string barcodeData, Turn turn)
		{
			Barcode2(x, y, barcodeType, false, narrowBar, wideBar, height, barcodeData, false, turn);
		}
		/// <summary>
		/// �o�[�R�[�h�iITF,CODE39,NW7�j���w�肵�܂��B
		/// </summary>
		/// <param name="x">�o�[�R�[�h�̊�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y">�o�[�R�[�h�̊�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="barcodeType">�o�[�R�[�h�̎�ނ��w��B</param>
		/// <param name="narrowBar">�׃o�[�����h�b�g�P�ʂŎw��B(1�`99)</param>
		/// <param name="wideBar">���o�[�����h�b�g�P�ʂŎw��B(1�`99)</param>
		/// <param name="height">�o�[�R�[�h����0.1mm�P�ʂŎw��B(0�`1000)</param>
		/// <param name="barcodeData">�o�[�R�[�h�f�[�^���i�[����Ă��镶����B</param>
		public void Barcode2(int x, int y, BarcodeType2 barcodeType, int narrowBar, int wideBar, int height, string barcodeData)
		{
			Barcode2(x, y, barcodeType, false, narrowBar, wideBar, height, barcodeData, false, Turn._0);
		}

		#endregion

		#region �R�}���h�w��

		/// <summary>
		///�R�}���h�𒼐ڎw�肵�܂��B
		/// </summary>
		/// <param name="command">�R�}���h���i�[����Ă��镶����B</param>
		public void Format(string command)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(command);

			append(data);
		}

		#endregion

		#region �������]

		/// <summary>
		/// �������]���w�肵�܂��B
		/// </summary>
		/// <param name="x1">�������]�̎n�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y1">�������]�̎n�_Y���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="x2">�������]�̏I�_X���W��0.1mm�P�ʂŎw��B</param>
		/// <param name="y2">�������]�̏I�_Y���W��0.1mm�P�ʂŎw��B</param>
		public void Reverse(int x1, int y1, int x2, int y2)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(String.Format("{{XR;{0:0000},{1:0000},{2:0000},{3:0000},B|}}", x1, y1, x2, y2));

			append(data);
		}

		#endregion

		#region �r�o

		/// <summary>
		/// �r�o���܂��B
		/// </summary>
		public void Eject()
		{
			byte[] data;

			data = Encoding.Default.GetBytes("{IB|}");

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
