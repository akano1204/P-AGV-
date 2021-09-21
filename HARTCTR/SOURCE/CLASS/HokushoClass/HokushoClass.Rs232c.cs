using System;
using System.IO.Ports;
using System.Text;
using System.Threading;

namespace HokushoClass.Rs232c
{
	/// <summary>
	/// RS-232Cクラス
	/// </summary>
	public class H_Rs232c
	{
		/// <summary>
		/// イベントを処理するメソッドを表します。
		/// </summary>
		public delegate void ReceiveEventHandler(object sender, ReceiveEventArgs e);

		/// <summary>
		/// 受信すると発生します。
		/// </summary>
		public event ReceiveEventHandler ReceiveEvent;

		/// <summary>
		/// 受信すると発生するイベントの引数。
		/// </summary>
		public class ReceiveEventArgs : EventArgs
		{
			internal byte[] bytes_data;

			/// <summary>
			/// 受信したデータが格納されているバイト配列を取得します。
			/// </summary>
			public byte[] BytesData
			{
				get
				{
					return bytes_data;
				}
			}
		}

		#region 列挙型
		/// <summary>
		/// ポート№
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
		/// ボーレート
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
		/// データ長
		/// </summary>
		public enum ByteSize
		{
			/// <summary>
			/// 7ビット
			/// </summary>
			_7 = 7,
			/// <summary>
			/// 7ビット
			/// </summary>
			_8 = 8,
		}

		/// <summary>
		/// パリティ
		/// </summary>
		public enum Parity
		{
			/// <summary>
			/// なし
			/// </summary>
			None = 0,
			/// <summary>
			/// 奇数
			/// </summary>
			Odd = 1,
			/// <summary>
			/// 偶数
			/// </summary>
			Even = 2,
		}

		/// <summary>
		/// ストップビット
		/// </summary>
		public enum StopBits
		{
			/// <summary>
			/// 1ビット
			/// </summary>
			_1 = 0,
			/// <summary>
			/// 2ビット
			/// </summary>
			_2 = 2,
		}

		/// <summary>
		/// フォーマット
		/// </summary>
		public enum FormatType
		{
			/// <summary>
			/// なし
			/// </summary>
			None,
			/// <summary>
			/// ～CR
			/// </summary>
			CR,
			/// <summary>
			/// ～CR+LF
			/// </summary>
			CR_LF,
			/// <summary>
			/// STX～ETX
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
		// コンストラクタ
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	bool			eventEnable			ｲﾍﾞﾝﾄ処理(true:する,false:しない)
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	なし
		//
		//====================================================================================================
		/// <summary>
		/// RS-232Cクラス
		/// </summary>
		public H_Rs232c()
		{
			Event_enable = false;
		}
		/// <summary>
		/// RS-232Cクラス
		/// </summary>
		/// <param name="eventEnable">イベント型にする場合は true。それ以外の場合は false。</param>
		public H_Rs232c(bool eventEnable)
		{
			Event_enable = eventEnable;
		}

		//====================================================================================================
		// デストラクタ
		//====================================================================================================
		/// <summary>
		///  RS-232Cクラス
		/// </summary>
		~H_Rs232c()
		{
			this.Close();
		}

		/// <summary>
		/// CTS(Clear To Send)シグナルの状態を取得します。
		/// </summary>
		public bool CtsHolding
		{
			get
			{
				return Port.CtsHolding;
			}
		}

		/// <summary>
		/// RTS(Request To Send)シグナルが有効にする値を取得または設定します。
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
		/// DSR(Data Set Ready)シグナルの状態を取得します。
		/// </summary>
		public bool DsrHolding
		{
			get
			{
				return Port.DsrHolding;
			}
		}

		/// <summary>
		/// DTR(Data Terminal Ready)シグナルを有効にする値を取得または設定します。
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
		// オープン
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	PortNo			portNo				ﾎﾟｰﾄ№
		//	BaudRate		baudRate			ﾎﾞｰﾚｰﾄ
		//	ByteSize		byteSize			ﾊﾞｲﾄｻｲｽﾞ
		//	Parity			parity				ﾊﾟﾘﾃｨ
		//	StopBits		stopBits			ｽﾄｯﾌﾟﾋﾞｯﾄ
		//	FormatType		formatType			ﾌｫｰﾏｯﾄﾀｲﾌﾟ
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
		/// デバイスをオープンします。
		/// </summary>
		/// <param name="portNo">オープンするデバイスのポート№。</param>
		/// <param name="baudRate">オープンするデバイスのボーレート。</param>
		/// <param name="byteSize">オープンするデバイスのデータビット長。</param>
		/// <param name="parity">オープンするデバイスのパリティビット。</param>
		/// <param name="stopBits">オープンするデバイスのストップビット。</param>
		/// <param name="formatType">オープンするデバイスのフォーマット。FormatType.NONE 以外は、書き込むデータに付加されます。また、読み込みはそのフォーマットの単位で行い、読み込みデータからその部分は排除されます。</param>
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
					errors(-1, "すでに使用されています。");
				}
			}
			catch (Exception ex)
			{
				errors(-1, ex.Message);
			}

			return Error_code == 0 ? true : false;
		}

		//====================================================================================================
		// クローズ
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
		/// デバイスをクローズします。
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
		// 送信
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	string			data				ﾃﾞｰﾀ
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
		/// デバイスにデータを書き込みます。
		/// </summary>
		/// <param name="data">書き込むデータが格納されている文字列。</param>
		/// <returns></returns>
		public bool Send(string data)
		{
			return SendBytes(Encoding.Default.GetBytes(data));
		}

		//====================================================================================================
		// 受信
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	なし
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	string			data				ﾃﾞｰﾀ
		//
		//====================================================================================================
		/// <summary>
		/// デバイスからデータを読み込みます。
		/// </summary>
		/// <returns></returns>
		public string Receive()
		{
			return Encoding.Default.GetString(ReceiveBytes());
		}

		//====================================================================================================
		// 送信（バイト配列）
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			data				ﾃﾞｰﾀ
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
		/// デバイスにバイト配列のデータを書き込みます。
		/// </summary>
		/// <param name="data">書き込むデータが格納されているバイト配列。</param>
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
		// 受信（バイト配列）
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	なし
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	out int			bytes_size			受信ﾊﾞｲﾄ数(-1:未受信)
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]			data				ﾃﾞｰﾀ
		//
		//====================================================================================================
		/// <summary>
		/// デバイスからバイト配列でデータを読み込みます。
		/// </summary>
		/// <param name="bytes_size">受信したバイト数。未受信のときは、-1。</param>
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
		/// デバイスからバイト配列でデータを読み込みます。
		/// </summary>
		/// <returns></returns>
		public byte[] ReceiveBytes()
		{
			int count;

			return ReceiveBytes(out count);
		}

		//====================================================================================================
		// 送信（バイト配列）
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	byte[]			data				ﾃﾞｰﾀ
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
		/// デバイスに FormatType列挙型 の設定を付加せずにデータを書き込みます。
		/// </summary>
		/// <param name="data">書き込むデータが格納されているバイト配列。</param>
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
		// 受信（バイト配列）
		//
		//- INPUT --------------------------------------------------------------------------------------------
		//	なし
		//
		//- OUTPUT -------------------------------------------------------------------------------------------
		//	なし
		//
		//- RETURN -------------------------------------------------------------------------------------------
		//	byte[]			data				ﾃﾞｰﾀ
		//
		//====================================================================================================
		/// <summary>
		/// デバイスから FormatType列挙型 の設定を無視してバイト配列でデータを読み込みます。
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
		// 受信監視スレッド
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
		// 受信（コールバック）
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
