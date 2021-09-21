using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace HokushoClass.Rs232c
{
	/// <summary>
	/// RS-232C�N���X
	/// </summary>
	public class H_Rs232c
	{
		/// <summary>
		/// �C�x���g���������郁�\�b�h��\���܂��B
		/// </summary>
		public delegate void ReceiveEventHandler(object sender, ReceiveEventArgs e);

		/// <summary>
		/// ��M����Ɣ������܂��B
		/// </summary>
		public event ReceiveEventHandler ReceiveEvent;

		/// <summary>
		/// ��M����Ɣ�������C�x���g�̈����B
		/// </summary>
		public class ReceiveEventArgs : EventArgs
		{
			internal byte[] bytes_data;

			/// <summary>
			/// ��M�����f�[�^���i�[����Ă���o�C�g�z����擾���܂��B
			/// </summary>
			public byte[] BytesData
			{
				get
				{
					return bytes_data;
				}
			}
		}

		#region �񋓌^
		/// <summary>
		/// �|�[�g��
		/// </summary>
		public enum PortNo
		{
			/// <summary>
			/// 
			/// </summary>
			_01 = 1,
			/// <summary>
			/// 
			/// </summary>
			_02 = 2,
			/// <summary>
			/// 
			/// </summary>
			_03 = 3,
			/// <summary>
			/// 
			/// </summary>
			_04 = 4,
			/// <summary>
			/// 
			/// </summary>
			_05 = 5,
			/// <summary>
			/// 
			/// </summary>
			_06 = 6,
			/// <summary>
			/// 
			/// </summary>
			_07 = 7,
			/// <summary>
			/// 
			/// </summary>
			_08 = 8,
			/// <summary>
			/// 
			/// </summary>
			_09 = 9,
			/// <summary>
			/// 
			/// </summary>
			_10 = 10,
			/// <summary>
			/// 
			/// </summary>
			_11 = 11,
			/// <summary>
			/// 
			/// </summary>
			_12 = 12,
			/// <summary>
			/// 
			/// </summary>
			_13 = 13,
			/// <summary>
			/// 
			/// </summary>
			_14 = 14,
			/// <summary>
			/// 
			/// </summary>
			_15 = 15,
			/// <summary>
			/// 
			/// </summary>
			_16 = 16,
			/// <summary>
			/// 
			/// </summary>
			_17 = 17,
			/// <summary>
			/// 
			/// </summary>
			_18 = 18,
			/// <summary>
			/// 
			/// </summary>
			_19 = 19,
			/// <summary>
			/// 
			/// </summary>
			_20 = 20,
		}

		/// <summary>
		/// �{�[���[�g
		/// </summary>
		public enum BaudRate
		{
			/// <summary>
			/// 1200bps
			/// </summary>
			_1200 = 1200,
			/// <summary>
			/// 2400bps
			/// </summary>
			_2400 = 2400,
			/// <summary>
			/// 4800bps
			/// </summary>
			_4800 = 4800,
			/// <summary>
			/// 9600bps
			/// </summary>
			_9600 = 9600,
			/// <summary>
			/// 14400bps
			/// </summary>
			_14400 = 14400,
			/// <summary>
			/// 19200bps
			/// </summary>
			_19200 = 19200,
			/// <summary>
			/// 38400bps
			/// </summary>
			_38400 = 38400,
			/// <summary>
			/// 57600bps
			/// </summary>
			_57600 = 57600,
			/// <summary>
			/// 115200bps
			/// </summary>
			_115200 = 115200,
			/// <summary>
			/// 230400bps
			/// </summary>
			_230400 = 230400,
		}

		/// <summary>
		/// �f�[�^��
		/// </summary>
		public enum ByteSize
		{
			/// <summary>
			/// 7�r�b�g
			/// </summary>
			_7 = 7,
			/// <summary>
			/// 7�r�b�g
			/// </summary>
			_8 = 8,
		}

		/// <summary>
		/// �p���e�B
		/// </summary>
		public enum Parity
		{
			/// <summary>
			/// �Ȃ�
			/// </summary>
			None = 0,
			/// <summary>
			/// �
			/// </summary>
			Odd = 1,
			/// <summary>
			/// ����
			/// </summary>
			Even = 2,
		}

		/// <summary>
		/// �X�g�b�v�r�b�g
		/// </summary>
		public enum StopBits
		{
			/// <summary>
			/// 1�r�b�g
			/// </summary>
			_1 = 0,
			/// <summary>
			/// 2�r�b�g
			/// </summary>
			_2 = 2,
		}

		/// <summary>
		/// �t�H�[�}�b�g
		/// </summary>
		public enum FormatType
		{
			/// <summary>
			/// �Ȃ�
			/// </summary>
			None,
			/// <summary>
			/// �`CR
			/// </summary>
			CR,
			/// <summary>
			/// �`CR+LF
			/// </summary>
			CR_LF,
			/// <summary>
			/// STX�`ETX
			/// </summary>
			STX_ETX,
		}
		#endregion

		private SerialPort Port = null;
		private bool Event_enable = false;
		private Thread Sub_thread = null;
		private FormatType Format_type;
		private int Error_code;
		private string Error_message;
		private int Receive_count = 0;
		private byte[] Receive_data = new byte[4096];

		//====================================================================================================
		// �R���X�g���N�^
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	bool			eventEnable			����ď���(true:����,false:���Ȃ�)
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// RS-232C�N���X
		/// </summary>
		public H_Rs232c()
		{
			Event_enable = false;
		}
		/// <summary>
		/// RS-232C�N���X
		/// </summary>
		/// <param name="eventEnable">�C�x���g�^�ɂ���ꍇ�� true�B����ȊO�̏ꍇ�� false�B</param>
		public H_Rs232c(bool eventEnable)
		{
			Event_enable = eventEnable;
		}

		//====================================================================================================
		// �f�X�g���N�^
		//====================================================================================================
		/// <summary>
		///  RS-232C�N���X
		/// </summary>
		~H_Rs232c()
		{
			this.Close();
		}

		/// <summary>
		/// CTS(Clear To Send)�V�O�i���̏�Ԃ��擾���܂��B
		/// </summary>
		public bool CtsHolding
		{
			get
			{
				return Port.CtsHolding;
			}
		}

		/// <summary>
		/// RTS(Request To Send)�V�O�i�����L���ɂ���l���擾�܂��͐ݒ肵�܂��B
		/// </summary>
		public bool RtsEnable
		{
			get
			{
				return Port.RtsEnable;
			}
			set
			{
				Port.RtsEnable = value;
			}
		}

		/// <summary>
		/// DSR(Data Set Ready)�V�O�i���̏�Ԃ��擾���܂��B
		/// </summary>
		public bool DsrHolding
		{
			get
			{
				return Port.DsrHolding;
			}
		}

		/// <summary>
		/// DTR(Data Terminal Ready)�V�O�i����L���ɂ���l���擾�܂��͐ݒ肵�܂��B
		/// </summary>
		public bool DtrEnable
		{
			get
			{
				return Port.DtrEnable;
			}
			set
			{
				Port.DtrEnable = value;
			}
		}

		//====================================================================================================
		// �I�[�v��
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	PortNo			portNo				�߰ć�
		//	BaudRate		baudRate			�ްڰ�
		//	ByteSize		byteSize			�޲Ļ���
		//	Parity			parity				���è
		//	StopBits		stopBits			�į���ޯ�
		//	FormatType		formatType			̫�ϯ�����
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
		/// �f�o�C�X���I�[�v�����܂��B
		/// </summary>
		/// <param name="portNo">�I�[�v������f�o�C�X�̃|�[�g���B</param>
		/// <param name="baudRate">�I�[�v������f�o�C�X�̃{�[���[�g�B</param>
		/// <param name="byteSize">�I�[�v������f�o�C�X�̃f�[�^�r�b�g���B</param>
		/// <param name="parity">�I�[�v������f�o�C�X�̃p���e�B�r�b�g�B</param>
		/// <param name="stopBits">�I�[�v������f�o�C�X�̃X�g�b�v�r�b�g�B</param>
		/// <param name="formatType">�I�[�v������f�o�C�X�̃t�H�[�}�b�g�BFormatType.NONE �ȊO�́A�������ރf�[�^�ɕt������܂��B�܂��A�ǂݍ��݂͂��̃t�H�[�}�b�g�̒P�ʂōs���A�ǂݍ��݃f�[�^���炻�̕����͔r������܂��B</param>
		/// <returns></returns>
		public bool Open(PortNo portNo, BaudRate baudRate, ByteSize byteSize, Parity parity, StopBits stopBits, FormatType formatType)
		{
			errors();

			System.IO.Ports.Parity _parity;
			System.IO.Ports.StopBits _stopBits;

			errors();

			if (parity == Parity.Even)
			{
				_parity = System.IO.Ports.Parity.Even;
			}
			else if (parity == Parity.Odd)
			{
				_parity = System.IO.Ports.Parity.Odd;
			}
			else
			{
				_parity = System.IO.Ports.Parity.None;
			}

			if (stopBits == StopBits._2)
			{
				_stopBits = System.IO.Ports.StopBits.Two;
			}
			else
			{
				_stopBits = System.IO.Ports.StopBits.One;
			}

			try
			{
				if (Port == null)
				{
					Port = new SerialPort("COM" + ((int)portNo).ToString(), (int)baudRate, _parity, (int)byteSize, _stopBits);

					Port.Open();

					Format_type = formatType;

					Receive_count = 0;
					Sub_thread = null;

					if (Event_enable)
					{
						if (Sub_thread == null)
						{
							Sub_thread = new Thread(H_Rs232c.receive_thread);
							Sub_thread.Start(this);
						}
					}
				}
				else
				{
					errors(-1, "���łɎg�p����Ă��܂��B");
				}
			}
			catch (Exception ex)
			{
				errors(-1, ex.Message);
			}

			return Error_code == 0 ? true : false;
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
		/// �f�o�C�X���N���[�Y���܂��B
		/// </summary>
		public void Close()
		{
			if (Port != null)
			{
				Port.Close();

				Port = null;
			}

			if (Sub_thread != null)
			{
				Sub_thread.Abort();
				Sub_thread.Join();

				Sub_thread = null;
			}
		}

		//====================================================================================================
		// ���M
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			data				�ް�
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
		/// �f�o�C�X�Ƀf�[�^���������݂܂��B
		/// </summary>
		/// <param name="data">�������ރf�[�^���i�[����Ă��镶����B</param>
		/// <returns></returns>
		public bool Send(string data)
		{
			return SendBytes(Encoding.Default.GetBytes(data));
		}

		//====================================================================================================
		// ��M
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	string			data				�ް�
		//
		//====================================================================================================
		/// <summary>
		/// �f�o�C�X����f�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <returns></returns>
		public string Receive()
		{
			return Encoding.Default.GetString(ReceiveBytes());
		}

		//====================================================================================================
		// ���M�i�o�C�g�z��j
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
		/// �f�o�C�X�Ƀo�C�g�z��̃f�[�^���������݂܂��B
		/// </summary>
		/// <param name="data">�������ރf�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <returns></returns>
		public bool SendBytes(byte[] data)
		{
			byte[] send_data;

			errors();

			switch (Format_type)
			{
			case FormatType.STX_ETX:
				send_data = new byte[data.Length + 2];

				send_data[0] = 0x02;
				Array.Copy(data, 0, send_data, 1, data.Length);
				send_data[data.Length + 1] = 0x03;
				break;

			case FormatType.CR_LF:
				send_data = new byte[data.Length + 2];

				Array.Copy(data, 0, send_data, 0, data.Length);
				send_data[data.Length] = 0x0D;
				send_data[data.Length + 1] = 0x0A;
				break;

			case FormatType.CR:
				send_data = new byte[data.Length + 1];

				Array.Copy(data, 0, send_data, 0, data.Length);
				send_data[data.Length] = 0x0D;
				break;

			default:
				send_data = new byte[data.Length];

				Array.Copy(data, 0, send_data, 0, data.Length);
				break;
			}

			return SendBytesDirect(send_data);
		}

		//====================================================================================================
		// ��M�i�o�C�g�z��j
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	out int			bytes_size			��M�޲Đ�(-1:����M)
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]			data				�ް�
		//
		//====================================================================================================
		/// <summary>
		/// �f�o�C�X����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="bytes_size">��M�����o�C�g���B����M�̂Ƃ��́A-1�B</param>
		/// <returns></returns>
		public byte[] ReceiveBytes(out int bytes_size)
		{
			byte[] receive_data = new byte[4096], temp = new byte[1];
			bool flag = false;
			int count = 0, length = 0, check = 0, pos = 0;

			lock (this)
			{
				if (!Event_enable)
				{
					while (true)
					{
						count = 0;

						if (Port.BytesToRead > 0)
						{
							count = Port.Read(temp, 0, 1);
						}

						if (count == 1 && Receive_data.Length > Receive_count)
						{
							Receive_data[Receive_count++] = temp[0];
						}
						else
						{
							break;
						}
					}
				}

				switch (Format_type)
				{
				case FormatType.STX_ETX:
					for (count = 0; count < Receive_count; count++)
					{
						if (check == 0)
						{
							if (Receive_data[count] == 0x02) check = 1;

						}
						else if (check == 1)
						{
							if (Receive_data[count] == 0x03)
							{
								length = pos;
								Receive_count -= (count + 1);
								Array.Copy(Receive_data, count + 1, Receive_data, 0, Receive_count);
								Array.Clear(Receive_data, Receive_count, Receive_data.Length - Receive_count);

								flag = true;
								break;
							}
							else
							{
								receive_data[pos++] = Receive_data[count];
							}
						}
					}
					break;

				case FormatType.CR_LF:
					for (count = 0; count < Receive_count; count++)
					{
						if (Receive_data[count] == 0x0D &&
							Receive_data[count + 1] == 0x0A && (count + 1) < Receive_count)
						{
							length = pos;
							Receive_count -= (count + 2);
							Array.Copy(Receive_data, count + 2, Receive_data, 0, Receive_count);
							Array.Clear(Receive_data, Receive_count, Receive_data.Length - Receive_count);

							flag = true;
							break;
						}
						else
						{
							receive_data[pos++] = Receive_data[count];
						}
					}
					break;

				case FormatType.CR:
					for (count = 0; count < Receive_count; count++)
					{
						if (Receive_data[count] == 0x0D)
						{
							length = pos;
							Receive_count -= (count + 1);
							Array.Copy(Receive_data, count + 1, Receive_data, 0, Receive_count);
							Array.Clear(Receive_data, Receive_count, Receive_data.Length - Receive_count);

							flag = true;
							break;
						}
						else
						{
							receive_data[pos++] = Receive_data[count];
						}
					}
					break;

				default:
					if (Receive_count > 0)
					{
						length = Receive_count;
						Array.Copy(Receive_data, 0, receive_data, 0, Receive_count);
						Array.Clear(Receive_data, 0, Receive_data.Length);
						Receive_count = 0;

						flag = true;
					}
					break;
				}
			}

			if (flag) bytes_size = length;
			else bytes_size = -1;

			temp = new byte[length];
			Array.Copy(receive_data, 0, temp, 0, length);

			return temp;
		}
		/// <summary>
		/// �f�o�C�X����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <returns></returns>
		public byte[] ReceiveBytes()
		{
			int count;

			return ReceiveBytes(out count);
		}

		//====================================================================================================
		// ���M�i�o�C�g�z��j
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
		/// �f�o�C�X�� FormatType�񋓌^ �̐ݒ��t�������Ƀf�[�^���������݂܂��B
		/// </summary>
		/// <param name="data">�������ރf�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <returns></returns>
		public bool SendBytesDirect(byte[] data)
		{
			byte[] temp;
			int length, split = 512;

			errors();

			for (length = 0; length < data.Length; length += split)
			{
				if ((length + split) <= data.Length)
				{
					temp = new byte[split];
				}
				else
				{
					temp = new byte[data.Length - length];
				}

				Array.Copy(data, length, temp, 0, temp.Length);

				try
				{
					Port.Write(temp, 0, temp.Length);
				}
				catch (Exception ex)
				{
					errors(-100, ex.Message);

					break;
				}
			}

			return Error_code == 0 ? true : false;
		}

		//====================================================================================================
		// ��M�i�o�C�g�z��j
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]			data				�ް�
		//
		//====================================================================================================
		/// <summary>
		/// �f�o�C�X���� FormatType�񋓌^ �̐ݒ�𖳎����ăo�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <returns></returns>
		public byte[] ReceiveBytesDirect()
		{
			byte[] receive_data = new byte[4096], temp = new byte[1];
			int count = 0, length = 0;

			lock (this)
			{
				if (!Event_enable)
				{
					while (true)
					{
						count = 0;

						if (Port.BytesToRead > 0)
						{
							count = Port.Read(temp, 0, 1);
						}

						if (count == 1 && Receive_data.Length > Receive_count)
						{
							Receive_data[Receive_count++] = temp[0];
						}
						else
						{
							break;
						}
					}
				}

				length = Receive_count;
				Array.Copy(Receive_data, 0, receive_data, 0, Receive_count);
				Array.Clear(Receive_data, 0, Receive_data.Length);
				Receive_count = 0;
			}

			temp = new byte[length];
			Array.Copy(receive_data, 0, temp, 0, length);

			return temp;
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
		// ��M�Ď��X���b�h
		//====================================================================================================
		private static void receive_thread(object target)
		{
			H_Rs232c port = target as H_Rs232c;

			port.receive_check();
		}

		private void receive_check()
		{
			byte[] temp = new byte[1];
			bool check;

			Port.ReadTimeout = 100;

			while (Port.IsOpen)
			{
				check = false;

				lock (this)
				{
					try
					{
						if (Receive_data.Length > Receive_count)
						{
							Receive_data[Receive_count] = (byte)Port.ReadByte();

							Receive_count++;

							check = true;
						}
					}
					catch
					{
					}
				}

				if (check)
				{
					ThreadPool.QueueUserWorkItem(new WaitCallback(ReceiveCallback));
				}
			}
		}

		//====================================================================================================
		// ��M�i�R�[���o�b�N�j
		//====================================================================================================
		private void ReceiveCallback(Object stateInfo)
		{
			byte[] data;

			while (true)
			{
				data = this.ReceiveBytes();

				if (data.Length > 0)
				{
					ReceiveEventArgs e = new ReceiveEventArgs();

					e.bytes_data = data;

					this.ReceiveEvent(this, e);
				}
				else
				{
					break;
				}
			}
		}
	}
}
