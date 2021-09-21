using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace HokushoClass.Sockets
{
	#region �\�P�b�g�C���^�[�t�F�[�X
	/// <summary>
	/// �\�P�b�g�C���^�[�t�F�[�X
	/// </summary>
	public class H_SocketBase
	{
		/// <summary>
		/// �v���t�B�b�N�X�i����t�H�[�}�b�g�j
		/// </summary>
		protected const string PREFIX = "<HKS_PREFIX>";
		/// <summary>
		/// �T�t�B�b�N�X�i����t�H�[�}�b�g�j
		/// </summary>
		protected const string SUFFIX = "<HKS_SUFFIX>";
	}
	#endregion

	#region �\�P�b�g�N���X
	/// <summary>
	/// �\�P�b�g�N���X
	/// </summary>
	public class H_Socket : H_SocketBase
	{
		private enum EventType
		{
			DISCONNECT,
			CONNECT,
			RECEIVE,
		}

		/// <summary>
		/// �ڑ�����Ɣ������܂��B
		/// </summary>
		public event System.EventHandler ConnectEvent;

		/// <summary>
		/// �ؒf����Ɣ������܂��B
		/// </summary>
		public event System.EventHandler DisconnectEvent;

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
		/// �I�[�v�����[�h
		/// </summary>
		public enum OpenMode
		{
			/// <summary>
			/// �z�X�g��
			/// </summary>
			Host,
			/// <summary>
			/// �N���C�A���g��
			/// </summary>
			Client,
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
			/// <summary>
			/// ����t�H�[�}�b�g
			/// </summary>
			HKS_FORMAT,
		}
		#endregion

		private Socket Socket_master = null;
		private Socket Socket_worker = null;
		private IPEndPoint Ip_end_point;
		private FormatType Format_type;
		private int Error_code;
		private string Error_message;
		private int Receive_count = 0;
		private byte[] Receive_data = new byte[2048];
		private byte[] Temp_data = new byte[2048];
		private bool Connected = false;
		private bool Disconnected = false;
		private int Connect_timeout = 1000;
		private int Keep_alive_timer = 0;
		private AutoResetEvent Wait_event;
		private ManualResetEvent Connect_done = new ManualResetEvent(false);

		//====================================================================================================
		// �R���X�g���N�^
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			ip_address			IP���ڽ
		//	int				port				�߰ć�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g�N���X
		/// </summary>
		/// <param name="ip_address">�g�p����\�P�b�g��IP�A�h���X�B</param>
		/// <param name="port">�g�p����\�P�b�g�̃|�[�g���B</param>
		public H_Socket(string ip_address, int port)
		{
			Ip_end_point = new IPEndPoint(IPAddress.Parse(ip_address), port);

			Wait_event = new AutoResetEvent(false);
		}
		/// <summary>
		/// �\�P�b�g�N���X
		/// </summary>
		/// <param name="port">�g�p����\�P�b�g�̃|�[�g���B</param>
		public H_Socket(int port)
		{
			Ip_end_point = new IPEndPoint(IPAddress.Any, port);

			Wait_event = new AutoResetEvent(false);
		}

		//====================================================================================================
		// �I�[�v��
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	OpenMode		openMode			�����Ӱ��
		//	FormatType		formatType			̫�ϯ�����
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�		-1		�\��ς�
		//
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g�̃I�[�v�������܂��B
		/// </summary>
		/// <param name="openMode">�I�[�v������\�P�b�g�̃^�C�v�B</param>
		/// <param name="formatType">�I�[�v������\�P�b�g�̃t�H�[�}�b�g�B</param>
		/// <returns></returns>
		public bool Open(OpenMode openMode, FormatType formatType)
		{
			errors();

			if (Socket_master != null || Socket_worker != null)
			{
				errors(-1, "���łɎg�p����Ă��܂��B");

				return false;
			}

			Format_type = formatType;
			Receive_count = 0;
			Receive_data = new byte[2048];
			Connected = false;
			Disconnected = false;

			try
			{
				if (openMode == OpenMode.Host)
				{
					Socket_master = new Socket(Ip_end_point.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

					Socket_master.Bind(Ip_end_point);

					Socket_master.Listen(1);

					Socket_master.BeginAccept(new AsyncCallback(OnAccept), null);
				}
				else
				{
					Socket_worker = new Socket(Ip_end_point.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

					if (Keep_alive_timer > 0)
					{
						byte[] inBuffer = new byte[12];
						BitConverter.GetBytes(1).CopyTo(inBuffer, 0);
						BitConverter.GetBytes(Keep_alive_timer).CopyTo(inBuffer, 4);
						BitConverter.GetBytes(1000).CopyTo(inBuffer, 8);

						Socket_worker.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

						Socket_worker.IOControl(IOControlCode.KeepAliveValues, inBuffer, null);
					}

					Connect_done.Reset();

					Socket_worker.BeginConnect(Ip_end_point, new AsyncCallback(OnConnect), null);

					Connect_done.WaitOne(Connect_timeout, false);

					if (!IsConnected)
					{
						errors(-1, "�ڑ��ł��܂���B");

						this.Close();
					}
				}
			}
			catch (SocketException ex)
			{
				errors(ex.ErrorCode, ex.Message);

				this.Close();
			}
			catch (Exception ex)
			{
				errors(-1, ex.Message);

				this.Close();
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
		/// �\�P�b�g�̃N���[�Y�����܂��B
		/// </summary>
		public void Close()
		{
			lock (this)
			{
				if (Socket_worker != null)
				{
					try
					{
						Socket_worker.Shutdown(SocketShutdown.Both);
					}
					catch
					{
					}

					try
					{
						Socket_worker.Close();
					}
					catch
					{
					}

					Socket_worker = null;
				}

				if (Socket_master != null)
				{
					try
					{
						Socket_master.Close();
					}
					catch
					{
					}

					Socket_master = null;
				}

				Connected = false;
				Disconnected = false;
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
		//					false				�ُ�		-99		����
		//													-1000	���̑��ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g�Ƀf�[�^���������݂܂��B
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
		/// �\�P�b�g����f�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <returns></returns>
		public string Receive()
		{
			return Encoding.Default.GetString(ReceiveBytes());
		}
		/// <summary>
		/// �\�P�b�g����f�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="timeOut">�^�C���A�E�g�l�B</param>
		/// <returns></returns>
		public string Receive(int timeOut)
		{
			return Encoding.Default.GetString(ReceiveBytes(timeOut));
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
		//					false				�ُ�		-99		����
		//													-1000	���̑��ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g�Ƀo�C�g�z��̃f�[�^���������݂܂��B
		/// </summary>
		/// <param name="data">�������ރf�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <returns></returns>
		public bool SendBytes(byte[] data)
		{
			byte[] send_data;
			byte[] prefix_data, suffix_data;

			errors();

			if (Socket_worker == null)
			{
				errors(-99, "�����ł��B");

				return false;
			}

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

			case FormatType.HKS_FORMAT:
				prefix_data = Encoding.Default.GetBytes(PREFIX);
				suffix_data = Encoding.Default.GetBytes(SUFFIX);

				send_data = new byte[prefix_data.Length + 4 + data.Length + suffix_data.Length];

				Array.Copy(prefix_data, 0, send_data, 0, prefix_data.Length);
				Array.Copy(Bytes.Int32ToBytes(data.Length), 0, send_data, prefix_data.Length, 4);
				Array.Copy(data, 0, send_data, prefix_data.Length + 4, data.Length);
				Array.Copy(suffix_data, 0, send_data, prefix_data.Length + 4 + data.Length, suffix_data.Length);
				break;

			default:
				send_data = new byte[data.Length];

				Array.Copy(data, 0, send_data, 0, data.Length);
				break;
			}

			try
			{
				//Socket_worker.BeginSend(send_data, 0, send_data.Length, 0, new AsyncCallback(OnSend), null);
				Socket_worker.Send(send_data);
			}
			catch (Exception ex)
			{
				errors(-1002, ex.Message);
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
		//	out int			bytes_size			��M�޲Đ�(-1:����M)
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]			data				�ް�
		//
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="bytes_size">��M�����o�C�g���B����M�̂Ƃ��́A-1�B</param>
		/// <returns></returns>
		public byte[] ReceiveBytes(out int bytes_size)
		{
			byte[] receive_data, temp;
			byte[] prefix_data, suffix_data;
			bool flag = false;
			int count = 0, length = 0, check = 0, pos = 0, index1, index2;

			lock (this)
			{
				receive_data = new byte[Receive_data.Length];

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

				case FormatType.HKS_FORMAT:
					prefix_data = Encoding.Default.GetBytes(PREFIX);
					suffix_data = Encoding.Default.GetBytes(SUFFIX);

					index1 = Bytes.IndexOf(Receive_data, 0, prefix_data, 0, prefix_data.Length);
					index2 = -1;

					if (index1 >= 0)
					{
						index2 = Bytes.IndexOf(Receive_data, index1 + prefix_data.Length + 4, suffix_data, 0, suffix_data.Length);
					}

					if (index1 >= 0 && index2 >= 0)
					{
						pos = Bytes.BytesToInt32(Receive_data, index1 + prefix_data.Length);

						if (pos == (index2 - index1 - prefix_data.Length - 4))
						{
							length = pos;
							Array.Copy(Receive_data, index1 + prefix_data.Length + 4, receive_data, 0, length);

							flag = true;
						}

						Receive_count -= (index2 + suffix_data.Length);
						Array.Copy(Receive_data, index2 + suffix_data.Length, Receive_data, 0, Receive_count);
						Array.Clear(Receive_data, Receive_count, Receive_data.Length - Receive_count);
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
		/// �\�P�b�g����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <returns></returns>
		public byte[] ReceiveBytes()
		{
			int count;

			return ReceiveBytes(out count);
		}
		/// <summary>
		/// �\�P�b�g����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="timeOut">�^�C���A�E�g�l�B</param>
		/// <returns></returns>
		public byte[] ReceiveBytes(int timeOut)
		{
			int count;

			return ReceiveBytes(out count, timeOut);
		}
		/// <summary>
		/// �\�P�b�g����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="bytes_size">��M�����o�C�g���B����M�̂Ƃ��́A-1�B</param>
		/// <param name="timeOut">�^�C���A�E�g�l�B</param>
		/// <returns></returns>
		public byte[] ReceiveBytes(out int bytes_size, int timeOut)
		{
			int time_out, wait = 0;
			byte[] data = new byte[0];

			bytes_size = -1;

			if (timeOut < 0)
			{
				timeOut = 0;
			}

			time_out = Win32.TickCount;

			do
			{
				Wait_event.WaitOne(timeOut - wait, false);

				data = ReceiveBytes(out bytes_size);

				if (bytes_size != -1 || !IsConnected)
				{
					break;
				}

				wait = Win32.TickCount - time_out;

			} while (timeOut > wait);

			return data;
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
		/// �\�P�b�g�� FormatType�񋓌^ �̐ݒ��t�������Ƀf�[�^���������݂܂��B
		/// </summary>
		/// <param name="data">�������ރf�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <returns></returns>
		public bool SendBytesDirect(byte[] data)
		{
			errors();

			if (Socket_worker == null)
			{
				errors(-99, "�����ł��B");

				return false;
			}

			try
			{
				//Socket_worker.BeginSend(data, 0, data.Length, 0, new AsyncCallback(OnSend), null);
				Socket_worker.Send(data);
			}
			catch (Exception ex)
			{
				errors(-1003, ex.Message);
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
		/// �\�P�b�g���� FormatType�񋓌^ �̐ݒ�𖳎����ăo�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <returns></returns>
		public byte[] ReceiveBytesDirect()
		{
			byte[] receive_data, temp;
			int length = 0;

			lock (this)
			{
				receive_data = new byte[Receive_data.Length];

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
		// �ڑ��^�C���A�E�g���ԃv���p�e�B
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g�̐����m�F���Ԃ�ݒ肵�܂��B�P�ʂ̓~���b�B���ڑ�����O�ɐݒ肵�ĉ������B
		/// </summary>
		public int KeepAliveTimer
		{
			set
			{
				Keep_alive_timer = value;
			}
		}

		//====================================================================================================
		// �ڑ��^�C���A�E�g���ԃv���p�e�B
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g�̐ڑ��^�C���A�E�g���Ԃ��擾�A�ݒ肵�܂��B�P�ʂ̓~���b�B
		/// </summary>
		public int ConnectTimeOut
		{
			get
			{
				return Connect_timeout;
			}
			set
			{
				Connect_timeout = value;
			}
		}

		//====================================================================================================
		// �ڑ���ԃv���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ڑ���Ԃ��擾���܂��B�ڑ����Ă���ꍇ�� true�B����ȊO�̏ꍇ�� false�B
		/// </summary>
		public bool IsConnected
		{
			get
			{
				return Connected;
			}
		}

		//====================================================================================================
		// �ؒf��ԃv���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ؒf��Ԃ��擾���܂��B�ڑ����Ă��Đؒf�����ꍇ�� true�B����ȊO�̏ꍇ�� false�B
		/// </summary>
		public bool IsDisconnected
		{
			get
			{
				return Disconnected;
			}
		}

		//====================================================================================================
		// �ڑ���IP�A�h���X�v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ڑ����IP�A�h���X���擾���܂��B
		/// </summary>
		public string RemoteIpAddress
		{
			get
			{
				if (Socket_worker != null)
				{
					return ((IPEndPoint)Socket_worker.RemoteEndPoint).Address.ToString();
				}
				else
				{
					return "";
				}
			}
		}

		//====================================================================================================
		// �ڑ���|�[�g���v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ڑ���̃|�[�g�����擾���܂��B
		/// </summary>
		public int RemotePort
		{
			get
			{
				if (Socket_worker != null)
				{
					return ((IPEndPoint)Socket_worker.RemoteEndPoint).Port;
				}
				else
				{
					return -1;
				}
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
		// �ڑ��҂��i�R�[���o�b�N�j
		//====================================================================================================
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ar"></param>
		protected void OnAccept(IAsyncResult ar)
		{
			try
			{
				Socket_worker = Socket_master.EndAccept(ar);

				if (Keep_alive_timer > 0)
				{
					byte[] inBuffer = new byte[12];
					BitConverter.GetBytes(1).CopyTo(inBuffer, 0);
					BitConverter.GetBytes(Keep_alive_timer).CopyTo(inBuffer, 4);
					BitConverter.GetBytes(1000).CopyTo(inBuffer, 8);

					Socket_worker.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

					Socket_worker.IOControl(IOControlCode.KeepAliveValues, inBuffer, null);
				}

				Socket_master.Close();

				Socket_master = null;

				Connected = true;
				Disconnected = false;

				call_event(EventType.CONNECT);

				Socket_worker.BeginReceive(Temp_data, 0, Temp_data.Length, 0, new AsyncCallback(OnReceive), null);
			}
			catch (Exception ex)
			{
				errors(-1004, ex.Message);

				this.Close();

				Disconnected = true;
			}
		}

		//====================================================================================================
		// �ڑ��҂��i�R�[���o�b�N�j
		//====================================================================================================
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ar"></param>
		protected void OnConnect(IAsyncResult ar)
		{
			try
			{
				Socket_worker.EndConnect(ar);

				Connected = true;
				Disconnected = false;

				call_event(EventType.CONNECT);

				Socket_worker.BeginReceive(Temp_data, 0, Temp_data.Length, 0, new AsyncCallback(OnReceive), null);

				Connect_done.Set();
			}
			catch
			{
			}
		}

		//====================================================================================================
		// ���M�i�R�[���o�b�N�j
		//====================================================================================================
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ar"></param>
		protected void OnSend(IAsyncResult ar)
		{
			Socket_worker.EndSend(ar);
		}

		//====================================================================================================
		// ��M�i�R�[���o�b�N�j
		//====================================================================================================
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ar"></param>
		protected void OnReceive(IAsyncResult ar)
		{
			int count = 0;
			byte[] data;

			try
			{
				count = Socket_worker.EndReceive(ar);

				if (count > 0)
				{
					lock (this)
					{
						if (Receive_data.Length < Receive_count + count)
						{
							data = new byte[Receive_count + count];

							Array.Copy(Receive_data, 0, data, 0, Receive_count);

							Receive_data = data;
						}

						Array.Copy(Temp_data, 0, Receive_data, Receive_count, count);
						Receive_count += count;
					}

					Array.Clear(Temp_data, 0, Temp_data.Length);

					Socket_worker.BeginReceive(Temp_data, 0, Temp_data.Length, 0, new AsyncCallback(OnReceive), null);

					Wait_event.Set();

					call_event(EventType.RECEIVE);
				}
				else
				{
					Connected = false;
					Disconnected = true;

					Wait_event.Set();

					call_event(EventType.DISCONNECT);
				}
			}
			catch (Exception ex)
			{
				errors(-1005, ex.Message);

				Connected = false;
				Disconnected = true;

				Wait_event.Set();

				call_event(EventType.DISCONNECT);
			}
		}

		//====================================================================================================
		// �C�x���g�R�[��
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	EventType		type				����ć�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		private void call_event(EventType type)
		{
			byte[] data;

			if (type == EventType.CONNECT && this.ConnectEvent != null)
			{
				this.ConnectEvent(this, EventArgs.Empty);
			}
			else if (type == EventType.DISCONNECT && this.DisconnectEvent != null)
			{
				this.DisconnectEvent(this, EventArgs.Empty);
			}
			else if (type == EventType.RECEIVE && this.ReceiveEvent != null)
			{
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
	}
	#endregion

	#region �\�P�b�g�N���X(�����ڑ���)
	/// <summary>
	/// �\�P�b�g�N���X(�����ڑ���)
	/// </summary>
	public class H_SocketServer : H_SocketBase
	{
		/// <summary>
		/// �C�x���g���������郁�\�b�h��\���܂��B
		/// </summary>
		public delegate void ConnectEventHandler(object sender, ConnectEventArgs e);

		/// <summary>
		/// �ڑ�����Ɣ������܂��B
		/// </summary>
		public event ConnectEventHandler ConnectEvent;

		/// <summary>
		/// �ڑ�����Ɣ�������C�x���g�̈����B
		/// </summary>
		public class ConnectEventArgs : EventArgs
		{
			internal int no = -1;

			/// <summary>
			/// �ڑ������\�P�b�g�̐ڑ������擾���܂��B
			/// </summary>
			public int ConnectNo
			{
				get
				{
					return no;
				}
			}
		}

		#region �񋓌^
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
			/// <summary>
			/// ����t�H�[�}�b�g
			/// </summary>
			HKS_FORMAT,
		}
		#endregion

		#region �C���f�N�T
		/// <summary>
		/// �ڑ������\�P�b�g�̏��
		/// </summary>
		public Connected this[int no]
		{
			get
			{
				return Connect[no];
			}
		}
		#endregion

		private Socket Socket_master = null;
		private IPEndPoint Ip_end_point;
		private FormatType Format_type;
		private bool Enabled = false;
		private int Keep_alive_timer = 0;
		private int Error_code;
		private string Error_message;
		private Connected[] Connect;

		//====================================================================================================
		// �R���X�g���N�^
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			ip_address			IP���ڽ
		//	int				port				�߰ć�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g�N���X
		/// </summary>
		/// <param name="ip_address">�g�p����\�P�b�g��IP�A�h���X�B</param>
		/// <param name="port">�g�p����\�P�b�g�̃|�[�g���B</param>
		/// <param name="connect_max">�ő�ڑ���</param>
		public H_SocketServer(string ip_address, int port, int connect_max)
		{
			Ip_end_point = new IPEndPoint(IPAddress.Parse(ip_address), port);

			initialize(connect_max);
		}
		/// <summary>
		/// �\�P�b�g�N���X
		/// </summary>
		/// <param name="port">�g�p����\�P�b�g�̃|�[�g���B</param>
		/// <param name="connect_max">�ő�ڑ���</param>
		public H_SocketServer(int port, int connect_max)
		{
			Ip_end_point = new IPEndPoint(IPAddress.Any, port);

			initialize(connect_max);
		}
		/// <summary>
		/// �\�P�b�g�N���X
		/// </summary>
		/// <param name="port">�g�p����\�P�b�g�̃|�[�g���B</param>
		public H_SocketServer(int port)
			: this(port, 10)
		{
		}
		private void initialize(int max)
		{
			Connect = new Connected[max];

			for (int no = 0; no < Connect.Length; no++)
			{
				Connect[no] = new Connected();

				Connect[no].socket = null;
			}
		}

		//====================================================================================================
		// �I�[�v��
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	FormatType		formatType			̫�ϯ�����
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�		-1		�\��ς�
		//
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g�̃I�[�v�������܂��B
		/// </summary>
		/// <param name="formatType">�I�[�v������\�P�b�g�̃t�H�[�}�b�g�B</param>
		/// <returns></returns>
		public bool Open(FormatType formatType)
		{
			errors();

			if (Socket_master != null)
			{
				errors(-1, "���łɎg�p����Ă��܂��B");

				return false;
			}

			Format_type = formatType;
			Enabled = false;

			try
			{
				Socket_master = new Socket(Ip_end_point.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

				Socket_master.Bind(Ip_end_point);

				Socket_master.Listen(5);

				Socket_master.BeginAccept(new AsyncCallback(OnAccept), Socket_master);
			}
			catch (SocketException ex)
			{
				errors(ex.ErrorCode, ex.Message);

				this.Close();
			}
			catch (Exception ex)
			{
				errors(-1, ex.Message);

				this.Close();
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
		/// �\�P�b�g�̃N���[�Y�����܂��B
		/// </summary>
		public void Close()
		{
			for (int no = 0; no < Connect.Length; no++)
			{
				if (Connect[no].socket != null)
				{
					Connect[no].Dispose();
				}
			}

			Enabled = true;

			if (Socket_master != null)
			{
				try
				{
					Socket_master.Close();
				}
				catch
				{
				}

				Socket_master = null;
			}
		}

		//====================================================================================================
		// �ڑ��^�C���A�E�g���ԃv���p�e�B
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g�̐����m�F���Ԃ�ݒ肵�܂��B�P�ʂ̓~���b�B���ڑ�����O�ɐݒ肵�ĉ������B
		/// </summary>
		public int KeepAliveTimer
		{
			set
			{
				Keep_alive_timer = value;
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
		// �ڑ��҂��i�R�[���o�b�N�j
		//====================================================================================================
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ar"></param>
		protected void OnAccept(IAsyncResult ar)
		{
			Socket server = null;
			int no;

			if (Enabled)
			{
				return;
			}

			try
			{
				server = (Socket)ar.AsyncState;

				for (no = 0; no < Connect.Length; no++)
				{
					if (Connect[no].socket == null &&
						Connect[no].Reserved == false)
					{
						break;
					}
				}

				if (no < Connect.Length)
				{
					Connect[no] = new Connected();
					Connect[no].socket = server.EndAccept(ar);
					Connect[no].Format_type = Format_type;
					Connect[no].Enable = true;
					Connect[no].Reserved = true;
					Connect[no].Connect_no = no;
					//Connect[no].StartReceive();

					if (Keep_alive_timer > 0)
					{
						byte[] inBuffer = new byte[12];
						BitConverter.GetBytes(1).CopyTo(inBuffer, 0);
						BitConverter.GetBytes(Keep_alive_timer).CopyTo(inBuffer, 4);
						BitConverter.GetBytes(1000).CopyTo(inBuffer, 8);

						Connect[no].socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.KeepAlive, true);

						Connect[no].socket.IOControl(IOControlCode.KeepAliveValues, inBuffer, null);
					}

					if (this.ConnectEvent != null)
					{
						ConnectEventArgs e = new ConnectEventArgs();

						e.no = no;

						this.ConnectEvent(this, e);
					}

					Connect[no].StartReceive();
				}
				else
				{
					server.EndAccept(ar).Close();
				}

				server.BeginAccept(new AsyncCallback(OnAccept), server);
			}
			catch (Exception ex)
			{
				errors(-1004, ex.Message);

				if (server != null)
				{
					server.BeginAccept(new AsyncCallback(OnAccept), server);
				}

				//this.Close();
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

		#region �\�P�b�g�ڑ��N���X
		/// <summary>
		/// �\�P�b�g�ڑ��N���X
		/// </summary>
		public class Connected : IDisposable
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
				internal int no = -1;
				internal byte[] bytes_data;

				/// <summary>
				/// �ڑ������\�P�b�g�̐ڑ������擾���܂��B
				/// </summary>
				public int ConnectNo
				{
					get
					{
						return no;
					}
				}

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

			/// <summary>
			/// �C�x���g���������郁�\�b�h��\���܂��B
			/// </summary>
			public delegate void DisconnectEventHandler(object sender, DisconnectEventArgs e);

			/// <summary>
			/// �ؒf����Ɣ������܂��B
			/// </summary>
			public event DisconnectEventHandler DisconnectEvent;

			/// <summary>
			/// �ؒf����Ɣ�������C�x���g�̈����B
			/// </summary>
			public class DisconnectEventArgs : EventArgs
			{
				internal int no = -1;
				internal int error_code = 0;
				internal string error_message = "";

				/// <summary>
				/// �ؒf�����\�P�b�g�̐ڑ������擾���܂��B
				/// </summary>
				public int ConnectNo
				{
					get
					{
						return no;
					}
				}

				/// <summary>
				/// �ُ�R�[�h
				/// </summary>
				public int ErrorCode
				{
					get
					{
						return error_code;
					}
				}

				/// <summary>
				/// �ُ���e
				/// </summary>
				public string ErrorMessage
				{
					get
					{
						return error_message;
					}
				}
			}

			internal Socket socket = null;
			internal FormatType Format_type;
			internal bool Enable = false;
			internal bool Reserved = false;
			internal bool Disconnected = false;
			internal int Connect_no = -1;
			private int Error_code;
			private string Error_message;
			private int Receive_count = 0;
			private byte[] Receive_data = new byte[2048];
			private byte[] Temp_data = new byte[2048];
			private AutoResetEvent Wait_event = new AutoResetEvent(false);

			//====================================================================================================
			// ���\�[�X���
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
			/// ���\�[�X��������܂��B
			/// </summary>
			public void Dispose()
			{
				if (socket != null)
				{
					try
					{
						socket.Shutdown(SocketShutdown.Both);
					}
					catch
					{
					}

					try
					{
						socket.Close();
					}
					catch
					{
					}

					socket = null;
					Enable = false;
				}
			}

			//====================================================================================================
			// �\����
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
			/// �\���������܂��B
			/// </summary>
			public void Release()
			{
				Reserved = false;
				Disconnected = false;
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
			//					false				�ُ�		-99		����
			//													-1000	���̑��ُ�
			//
			//====================================================================================================
			/// <summary>
			/// �\�P�b�g�Ƀf�[�^���������݂܂��B
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
			/// �\�P�b�g����f�[�^��ǂݍ��݂܂��B
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
			//					false				�ُ�		-99		����
			//													-1000	���̑��ُ�
			//
			//====================================================================================================
			/// <summary>
			/// �\�P�b�g�Ƀo�C�g�z��̃f�[�^���������݂܂��B
			/// </summary>
			/// <param name="data">�������ރf�[�^���i�[����Ă���o�C�g�z��B</param>
			/// <returns></returns>
			public bool SendBytes(byte[] data)
			{
				byte[] send_data;
				byte[] prefix_data, suffix_data;

				errors();

				if (socket == null)
				{
					errors(-99, "�����ł��B");

					return false;
				}

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

				case FormatType.HKS_FORMAT:
					prefix_data = Encoding.Default.GetBytes(PREFIX);
					suffix_data = Encoding.Default.GetBytes(SUFFIX);

					send_data = new byte[prefix_data.Length + 4 + data.Length + suffix_data.Length];

					Array.Copy(prefix_data, 0, send_data, 0, prefix_data.Length);
					Array.Copy(Bytes.Int32ToBytes(data.Length), 0, send_data, prefix_data.Length, 4);
					Array.Copy(data, 0, send_data, prefix_data.Length + 4, data.Length);
					Array.Copy(suffix_data, 0, send_data, prefix_data.Length + 4 + data.Length, suffix_data.Length);
					break;

				default:
					send_data = new byte[data.Length];

					Array.Copy(data, 0, send_data, 0, data.Length);
					break;
				}

				try
				{
					socket.Send(send_data);
				}
				catch (Exception ex)
				{
					errors(-1002, ex.Message);
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
			//	out int			bytes_size			��M�޲Đ�(-1:����M)
			//
			//- RETURN -------------------------------------------------------------------------------------------
			//	byte[]			data				�ް�
			//
			//====================================================================================================
			/// <summary>
			/// �\�P�b�g����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
			/// </summary>
			/// <param name="bytes_size">��M�����o�C�g���B����M�̂Ƃ��́A-1�B</param>
			/// <returns></returns>
			public byte[] ReceiveBytes(out int bytes_size)
			{
				byte[] receive_data, temp;
				byte[] prefix_data, suffix_data;
				bool flag = false;
				int count = 0, length = 0, check = 0, pos = 0, index1, index2;

				lock (this)
				{
					receive_data = new byte[Receive_data.Length];

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

					case FormatType.HKS_FORMAT:
						prefix_data = Encoding.Default.GetBytes(PREFIX);
						suffix_data = Encoding.Default.GetBytes(SUFFIX);

						index1 = Bytes.IndexOf(Receive_data, 0, prefix_data, 0, prefix_data.Length);
						index2 = -1;

						if (index1 >= 0)
						{
							index2 = Bytes.IndexOf(Receive_data, index1 + prefix_data.Length + 4, suffix_data, 0, suffix_data.Length);
						}

						if (index1 >= 0 && index2 >= 0)
						{
							pos = Bytes.BytesToInt32(Receive_data, index1 + prefix_data.Length);

							if (pos == (index2 - index1 - prefix_data.Length - 4))
							{
								length = pos;
								Array.Copy(Receive_data, index1 + prefix_data.Length + 4, receive_data, 0, length);

								flag = true;
							}

							Receive_count -= (index2 + suffix_data.Length);
							Array.Copy(Receive_data, index2 + suffix_data.Length, Receive_data, 0, Receive_count);
							Array.Clear(Receive_data, Receive_count, Receive_data.Length - Receive_count);
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
			/// �\�P�b�g����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
			/// </summary>
			/// <returns></returns>
			public byte[] ReceiveBytes()
			{
				int count;

				return ReceiveBytes(out count);
			}
			/// <summary>
			/// �\�P�b�g����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
			/// </summary>
			/// <param name="timeOut">�^�C���A�E�g�l�B</param>
			/// <returns></returns>
			public byte[] ReceiveBytes(int timeOut)
			{
				int count;

				return ReceiveBytes(out count, timeOut);
			}
			/// <summary>
			/// �\�P�b�g����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
			/// </summary>
			/// <param name="bytes_size">��M�����o�C�g���B����M�̂Ƃ��́A-1�B</param>
			/// <param name="timeOut">�^�C���A�E�g�l�B</param>
			/// <returns></returns>
			public byte[] ReceiveBytes(out int bytes_size, int timeOut)
			{
				int time_out, wait = 0;
				byte[] data = new byte[0];

				bytes_size = -1;

				if (timeOut < 0)
				{
					timeOut = 0;
				}

				time_out = Win32.TickCount;

				do
				{
					Wait_event.WaitOne(timeOut - wait, false);

					data = ReceiveBytes(out bytes_size);

					if (bytes_size != -1 || !IsConnected)
					{
						break;
					}

					wait = Win32.TickCount - time_out;

				} while (timeOut > wait);

				return data;
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
			/// �\�P�b�g�� FormatType�񋓌^ �̐ݒ��t�������Ƀf�[�^���������݂܂��B
			/// </summary>
			/// <param name="data">�������ރf�[�^���i�[����Ă���o�C�g�z��B</param>
			/// <returns></returns>
			public bool SendBytesDirect(byte[] data)
			{
				errors();

				if (socket == null)
				{
					errors(-99, "�����ł��B");

					return false;
				}

				try
				{
					socket.Send(data);
				}
				catch (Exception ex)
				{
					errors(-1003, ex.Message);
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
			/// �\�P�b�g���� FormatType�񋓌^ �̐ݒ�𖳎����ăo�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
			/// </summary>
			/// <returns></returns>
			public byte[] ReceiveBytesDirect()
			{
				byte[] receive_data, temp;
				int length = 0;

				lock (this)
				{
					receive_data = new byte[Receive_data.Length];

					length = Receive_count;
					Array.Copy(Receive_data, 0, receive_data, 0, Receive_count);
					Array.Clear(Receive_data, 0, Receive_data.Length);
					Receive_count = 0;
				}

				temp = new byte[length];
				Array.Copy(receive_data, 0, temp, 0, length);

				return temp;
			}

			internal void StartReceive()
			{
				socket.BeginReceive(Temp_data, 0, Temp_data.Length, 0, new AsyncCallback(OnReceive), null);
			}

			//====================================================================================================
			// ��M�i�R�[���o�b�N�j
			//====================================================================================================
			/// <summary>
			/// 
			/// </summary>
			/// <param name="ar"></param>
			protected void OnReceive(IAsyncResult ar)
			{
				int count = 0;
				byte[] data;

				try
				{
					count = socket.EndReceive(ar);

					if (count > 0)
					{
						lock (this)
						{
							if (Receive_data.Length < Receive_count + count)
							{
								data = new byte[Receive_count + count];

								Array.Copy(Receive_data, 0, data, 0, Receive_count);

								Receive_data = data;
							}

							Array.Copy(Temp_data, 0, Receive_data, Receive_count, count);
							Receive_count += count;
						}

						Array.Clear(Temp_data, 0, Temp_data.Length);

						socket.BeginReceive(Temp_data, 0, Temp_data.Length, 0, new AsyncCallback(OnReceive), null);

						Wait_event.Set();

						if (this.ReceiveEvent != null)
						{
							while (true)
							{
								data = this.ReceiveBytes();

								if (data.Length > 0)
								{
									ReceiveEventArgs e = new ReceiveEventArgs();

									e.no = Connect_no;
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
					else
					{
						Wait_event.Set();

						if (this.DisconnectEvent != null)
						{
							DisconnectEventArgs e = new DisconnectEventArgs();

							e.no = Connect_no;

							this.DisconnectEvent(this, e);
						}

						this.Dispose();

						this.Disconnected = true;
					}
				}
				catch (Exception ex)
				{
					errors(-1005, ex.Message);

					if (this.DisconnectEvent != null)
					{
						DisconnectEventArgs e = new DisconnectEventArgs();

						e.no = Connect_no;
						e.error_code = -1005;
						e.error_message = ex.Message;

						this.DisconnectEvent(this, e);
					}

					this.Dispose();

					this.Disconnected = true;
				}
			}

			//====================================================================================================
			// �ڑ���ԃv���p�e�B
			//====================================================================================================
			/// <summary>
			/// �\�P�b�g�̐ڑ���Ԃ��擾���܂��B�ڑ����Ă���ꍇ�� true�B����ȊO�̏ꍇ�� false�B
			/// </summary>
			public bool IsConnected
			{
				get
				{
					return Enable;
				}
			}

			//====================================================================================================
			// �\���ԃv���p�e�B
			//====================================================================================================
			/// <summary>
			/// �\�P�b�g�̗\���Ԃ��擾���܂��B�\�񂳂�Ă���ꍇ�� true�B����ȊO�̏ꍇ�� false�B
			/// </summary>
			public bool IsReserved
			{
				get
				{
					return Reserved;
				}
			}

			//====================================================================================================
			// �ؒf��ԃv���p�e�B
			//====================================================================================================
			/// <summary>
			/// �ؒf��Ԃ��擾���܂��B�ڑ����Ă��Đؒf�����ꍇ�� true�B����ȊO�̏ꍇ�� false�B
			/// </summary>
			public bool IsDisconnected
			{
				get
				{
					return Disconnected;
				}
			}

			//====================================================================================================
			// �ڑ���IP�A�h���X�v���p�e�B
			//====================================================================================================
			/// <summary>
			/// �ڑ����IP�A�h���X���擾���܂��B
			/// </summary>
			public string RemoteIpAddress
			{
				get
				{
					if (socket != null)
					{
						return ((IPEndPoint)socket.RemoteEndPoint).Address.ToString();
					}
					else
					{
						return "";
					}
				}
			}

			//====================================================================================================
			// �ڑ���|�[�g���v���p�e�B
			//====================================================================================================
			/// <summary>
			/// �ڑ���̃|�[�g�����擾���܂��B
			/// </summary>
			public int RemotePort
			{
				get
				{
					if (socket != null)
					{
						return ((IPEndPoint)socket.RemoteEndPoint).Port;
					}
					else
					{
						return -1;
					}
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
		}
		#endregion
	}
	#endregion

	#region �\�P�b�g�N���X(UDP)
	/// <summary>
	/// �\�P�b�g�N���X(UDP)
	/// </summary>
	public class H_SocketUDP : H_SocketBase
	{
		private enum EventType
		{
			RECEIVE,
		}

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
			/// <summary>
			/// ����t�H�[�}�b�g
			/// </summary>
			HKS_FORMAT,
		}
		#endregion

		private Socket Socket_worker = null;
		private IPEndPoint Ip_end_point;
		private FormatType Format_type;
		private int Error_code;
		private string Error_message;
		private int Receive_count = 0;
		private byte[] Receive_data = new byte[2048];
		private byte[] Temp_data = new byte[2048];
		private AutoResetEvent Wait_event;

		//====================================================================================================
		// �R���X�g���N�^
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	int				port				�߰ć�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g�N���X
		/// </summary>
		/// <param name="port">�g�p����\�P�b�g�̃|�[�g���B</param>
		public H_SocketUDP(int port)
		{
			Ip_end_point = new IPEndPoint(IPAddress.Any, port);

			Wait_event = new AutoResetEvent(false);
		}

		//====================================================================================================
		// �I�[�v��
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	FormatType		formatType			̫�ϯ�����
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	bool			true				����
		//					false				�ُ�		-1		�\��ς�
		//
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g�̃I�[�v�������܂��B
		/// </summary>
		/// <param name="formatType">�I�[�v������\�P�b�g�̃t�H�[�}�b�g�B</param>
		/// <returns></returns>
		public bool Open(FormatType formatType)
		{
			errors();

			if (Socket_worker != null)
			{
				errors(-1, "���łɎg�p����Ă��܂��B");

				return false;
			}

			Format_type = formatType;
			Receive_count = 0;
			Receive_data = new byte[2048];

			try
			{
				Socket_worker = new Socket(Ip_end_point.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

				Socket_worker.Bind(Ip_end_point);

				Socket_worker.BeginReceive(Temp_data, 0, Temp_data.Length, 0, new AsyncCallback(OnReceive), null);
			}
			catch (SocketException ex)
			{
				errors(ex.ErrorCode, ex.Message);

				this.Close();
			}
			catch (Exception ex)
			{
				errors(-1, ex.Message);

				this.Close();
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
		/// �\�P�b�g�̃N���[�Y�����܂��B
		/// </summary>
		public void Close()
		{
			lock (this)
			{
				if (Socket_worker != null)
				{
					try
					{
						Socket_worker.Shutdown(SocketShutdown.Both);
					}
					catch
					{
					}

					try
					{
						Socket_worker.Close();
					}
					catch
					{
					}

					Socket_worker = null;
				}
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
		//					false				�ُ�		-99		����
		//													-1000	���̑��ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g�Ƀf�[�^���������݂܂��B
		/// </summary>
		/// <param name="data">�������ރf�[�^���i�[����Ă��镶����B</param>
		/// <param name="remoteEP"></param>
		/// <returns></returns>
		public bool Send(string data, IPEndPoint remoteEP)
		{
			return SendBytes(Encoding.Default.GetBytes(data), remoteEP);
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
		/// �\�P�b�g����f�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <returns></returns>
		public string Receive()
		{
			return Encoding.Default.GetString(ReceiveBytes());
		}
		/// <summary>
		/// �\�P�b�g����f�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="timeOut">�^�C���A�E�g�l�B</param>
		/// <returns></returns>
		public string Receive(int timeOut)
		{
			return Encoding.Default.GetString(ReceiveBytes(timeOut));
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
		//					false				�ُ�		-99		����
		//													-1000	���̑��ُ�
		//
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g�Ƀo�C�g�z��̃f�[�^���������݂܂��B
		/// </summary>
		/// <param name="data">�������ރf�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <param name="remoteEP"></param>
		/// <returns></returns>
		public bool SendBytes(byte[] data, IPEndPoint remoteEP)
		{
			byte[] send_data;
			byte[] prefix_data, suffix_data;

			errors();

			if (Socket_worker == null)
			{
				errors(-99, "�����ł��B");

				return false;
			}

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

			case FormatType.HKS_FORMAT:
				prefix_data = Encoding.Default.GetBytes(PREFIX);
				suffix_data = Encoding.Default.GetBytes(SUFFIX);

				send_data = new byte[prefix_data.Length + 4 + data.Length + suffix_data.Length];

				Array.Copy(prefix_data, 0, send_data, 0, prefix_data.Length);
				Array.Copy(Bytes.Int32ToBytes(data.Length), 0, send_data, prefix_data.Length, 4);
				Array.Copy(data, 0, send_data, prefix_data.Length + 4, data.Length);
				Array.Copy(suffix_data, 0, send_data, prefix_data.Length + 4 + data.Length, suffix_data.Length);
				break;

			default:
				send_data = new byte[data.Length];

				Array.Copy(data, 0, send_data, 0, data.Length);
				break;
			}

			try
			{
				Socket_worker.SendTo(send_data, remoteEP);
			}
			catch (Exception ex)
			{
				errors(-1002, ex.Message);
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
		//	out int			bytes_size			��M�޲Đ�(-1:����M)
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]			data				�ް�
		//
		//====================================================================================================
		/// <summary>
		/// �\�P�b�g����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="bytes_size">��M�����o�C�g���B����M�̂Ƃ��́A-1�B</param>
		/// <returns></returns>
		public byte[] ReceiveBytes(out int bytes_size)
		{
			byte[] receive_data, temp;
			byte[] prefix_data, suffix_data;
			bool flag = false;
			int count = 0, length = 0, check = 0, pos = 0, index1, index2;

			lock (this)
			{
				receive_data = new byte[Receive_data.Length];

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

				case FormatType.HKS_FORMAT:
					prefix_data = Encoding.Default.GetBytes(PREFIX);
					suffix_data = Encoding.Default.GetBytes(SUFFIX);

					index1 = Bytes.IndexOf(Receive_data, 0, prefix_data, 0, prefix_data.Length);
					index2 = -1;

					if (index1 >= 0)
					{
						index2 = Bytes.IndexOf(Receive_data, index1 + prefix_data.Length + 4, suffix_data, 0, suffix_data.Length);
					}

					if (index1 >= 0 && index2 >= 0)
					{
						pos = Bytes.BytesToInt32(Receive_data, index1 + prefix_data.Length);

						if (pos == (index2 - index1 - prefix_data.Length - 4))
						{
							length = pos;
							Array.Copy(Receive_data, index1 + prefix_data.Length + 4, receive_data, 0, length);

							flag = true;
						}

						Receive_count -= (index2 + suffix_data.Length);
						Array.Copy(Receive_data, index2 + suffix_data.Length, Receive_data, 0, Receive_count);
						Array.Clear(Receive_data, Receive_count, Receive_data.Length - Receive_count);
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
		/// �\�P�b�g����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <returns></returns>
		public byte[] ReceiveBytes()
		{
			int count;

			return ReceiveBytes(out count);
		}
		/// <summary>
		/// �\�P�b�g����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="timeOut">�^�C���A�E�g�l�B</param>
		/// <returns></returns>
		public byte[] ReceiveBytes(int timeOut)
		{
			int count;

			return ReceiveBytes(out count, timeOut);
		}
		/// <summary>
		/// �\�P�b�g����o�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <param name="bytes_size">��M�����o�C�g���B����M�̂Ƃ��́A-1�B</param>
		/// <param name="timeOut">�^�C���A�E�g�l�B</param>
		/// <returns></returns>
		public byte[] ReceiveBytes(out int bytes_size, int timeOut)
		{
			int time_out, wait = 0;
			byte[] data = new byte[0];

			bytes_size = -1;

			if (timeOut < 0)
			{
				timeOut = 0;
			}

			time_out = Win32.TickCount;

			do
			{
				Wait_event.WaitOne(timeOut - wait, false);

				data = ReceiveBytes(out bytes_size);

				if (bytes_size != -1)
				{
					break;
				}

				wait = Win32.TickCount - time_out;

			} while (timeOut > wait);

			return data;
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
		/// �\�P�b�g�� FormatType�񋓌^ �̐ݒ��t�������Ƀf�[�^���������݂܂��B
		/// </summary>
		/// <param name="data">�������ރf�[�^���i�[����Ă���o�C�g�z��B</param>
		/// <param name="remoteEP"></param>
		/// <returns></returns>
		public bool SendBytesDirect(byte[] data, IPEndPoint remoteEP)
		{
			errors();

			if (Socket_worker == null)
			{
				errors(-99, "�����ł��B");

				return false;
			}

			try
			{
				//Socket_worker.BeginSend(data, 0, data.Length, 0, new AsyncCallback(OnSend), null);
				Socket_worker.SendTo(data, remoteEP);
			}
			catch (Exception ex)
			{
				errors(-1003, ex.Message);
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
		/// �\�P�b�g���� FormatType�񋓌^ �̐ݒ�𖳎����ăo�C�g�z��Ńf�[�^��ǂݍ��݂܂��B
		/// </summary>
		/// <returns></returns>
		public byte[] ReceiveBytesDirect()
		{
			byte[] receive_data, temp;
			int length = 0;

			lock (this)
			{
				receive_data = new byte[Receive_data.Length];

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
		// �ڑ���IP�A�h���X�v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ڑ����IP�A�h���X���擾���܂��B
		/// </summary>
		public string RemoteIpAddress
		{
			get
			{
				if (Socket_worker != null)
				{
					return ((IPEndPoint)Socket_worker.RemoteEndPoint).Address.ToString();
				}
				else
				{
					return "";
				}
			}
		}

		//====================================================================================================
		// �ڑ���|�[�g���v���p�e�B
		//====================================================================================================
		/// <summary>
		/// �ڑ���̃|�[�g�����擾���܂��B
		/// </summary>
		public int RemotePort
		{
			get
			{
				if (Socket_worker != null)
				{
					return ((IPEndPoint)Socket_worker.RemoteEndPoint).Port;
				}
				else
				{
					return -1;
				}
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
		// ���M�i�R�[���o�b�N�j
		//====================================================================================================
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ar"></param>
		protected void OnSend(IAsyncResult ar)
		{
			Socket_worker.EndSend(ar);
		}

		//====================================================================================================
		// ��M�i�R�[���o�b�N�j
		//====================================================================================================
		/// <summary>
		/// 
		/// </summary>
		/// <param name="ar"></param>
		protected void OnReceive(IAsyncResult ar)
		{
			int count = 0;
			byte[] data;

			try
			{
				count = Socket_worker.EndReceive(ar);

				if (count > 0)
				{
					lock (this)
					{
						if (Receive_data.Length < Receive_count + count)
						{
							data = new byte[Receive_count + count];

							Array.Copy(Receive_data, 0, data, 0, Receive_count);

							Receive_data = data;
						}

						Array.Copy(Temp_data, 0, Receive_data, Receive_count, count);
						Receive_count += count;
					}

					Array.Clear(Temp_data, 0, Temp_data.Length);

					Socket_worker.BeginReceive(Temp_data, 0, Temp_data.Length, 0, new AsyncCallback(OnReceive), null);

					Wait_event.Set();

					call_event(EventType.RECEIVE);
				}
				else
				{
					Wait_event.Set();
				}
			}
			catch (Exception ex)
			{
				errors(-1005, ex.Message);

				Wait_event.Set();
			}
		}

		//====================================================================================================
		// �C�x���g�R�[��
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	EventType		type				����ć�
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	�Ȃ�
		//
		//====================================================================================================
		private void call_event(EventType type)
		{
			byte[] data;

			if (type == EventType.RECEIVE && this.ReceiveEvent != null)
			{
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
	}
	#endregion
}
