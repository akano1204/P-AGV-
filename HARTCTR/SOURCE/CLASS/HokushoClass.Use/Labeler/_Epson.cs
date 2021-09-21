using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using HokushoClass;
using HokushoClass.Sockets;


namespace HokushoClass.Use.Labeler
{
	/// <summary>
	/// ラベラークラス（エプソン用）
	/// </summary>
	public class EPSON
	{
		#region 列挙型

		private enum DeviceType
		{
			NONE,
			LAN,
		}

		#endregion

		private const byte EOT = 0x04;
		private const byte DLE = 0x10;
		private const byte ESC = 0x1B;
		private const byte FS = 0x1C;
		private const byte GS = 0x1D;

		/// <summary>
		/// コマンド編集クラス
		/// </summary>
		public EPSON_Commands Commands = new EPSON_Commands();

		private DeviceType Device = DeviceType.NONE;
		private H_Socket Socket;
		private int Error_code;
		private string Error_message;
		private int TimeOut;
		private int ProcessId = 0;
		private List<byte> PrintOutStatus = new List<byte>();

		#region オープン

		/// <summary>
		/// デバイスをオープンします。
		/// </summary>
		/// <param name="ip_address">オープンするネットワークデバイスのＩＰアドレス。</param>
		/// <param name="port">オープンするソケットのポート№。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool Open(string ip_address, int port)
		{
			bool status = false;

			errors();

			if (Device == DeviceType.NONE)
			{
				Socket = new H_Socket(ip_address, port);

				status = Socket.Open(H_Socket.OpenMode.Client, H_Socket.FormatType.None);

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
				errors(-10000, "既にオープンされています。");
			}

			return status;
		}
		/// <summary>
		/// デバイスをオープンします。
		/// </summary>
		/// <param name="ip_address">オープンするネットワークデバイスのＩＰアドレス。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool Open(string ip_address)
		{
			return Open(ip_address, 9100);
		}

		#endregion

		#region クローズ

		/// <summary>
		/// デバイスをクローズします。
		/// </summary>
		public void Close()
		{
			if (Device == DeviceType.LAN)
			{
				Socket.Close();

				Socket = null;
			}

			Device = DeviceType.NONE;
		}

		#endregion

		#region 送信（バイト配列）

		/// <summary>
		/// デバイスにバイト配列のデータを送信します。
		/// </summary>
		/// <param name="data">送信するデータが格納されているバイト配列。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool SendBytes(byte[] data)
		{
			bool status = false;

			errors();

			if (Device == DeviceType.LAN)
			{
				status = Socket.SendBytes(data);

				if (!status)
				{
					errors(Socket.ErrorCode, Socket.ErrorMessage);
				}
			}
			else
			{
				errors(-10001, "オープンされていません。");
			}

			return status;
		}

		#endregion

		#region 受信（バイト配列）

		/// <summary>
		/// デバイスからバイト配列でデータを受信します。
		/// </summary>
		/// <param name="bytes_size">受信したバイト数。未受信のときは、-1。</param>
		/// <returns>受信したバイト配列</returns>
		public byte[] ReceiveBytes(out int bytes_size)
		{
			byte[] status = new byte[0];

			bytes_size = -1;

			if (Device == DeviceType.LAN)
			{
				status = Socket.ReceiveBytes(out bytes_size);
			}

			return status;
		}

		#endregion

		#region ダイレクト送信（バイト配列）

		/// <summary>
		/// デバイスにバイト配列のデータをそのまま送信します。
		/// </summary>
		/// <param name="data">送信するデータが格納されているバイト配列。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool SendBytesDirect(byte[] data)
		{
			bool status = false;

			errors();

			if (Device == DeviceType.LAN)
			{
				status = Socket.SendBytesDirect(data);

				if (!status)
				{
					errors(Socket.ErrorCode, Socket.ErrorMessage);
				}
			}
			else
			{
				errors(-10001, "オープンされていません。");
			}

			return status;
		}

		#endregion

		#region ダイレクト受信（バイト配列）

		/// <summary>
		/// デバイスからバイト配列でデータをそのまま受信します。
		/// </summary>
		/// <returns>受信したバイト配列</returns>
		public byte[] ReceiveBytesDirect()
		{
			byte[] status = new byte[0];

			if (Device == DeviceType.LAN)
			{
				status = Socket.ReceiveBytesDirect();
			}

			return status;
		}

		#endregion

		#region 編集データ送信

		/// <summary>
		/// デバイスに編集データを送信します。
		/// </summary>
		/// <param name="data">送信するデータが格納されているバイト配列。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool PrintOut(byte[] data)
		{
			var id_data = new byte[] { GS, (byte)'(', (byte)'H', 0x06, 0x00, 0x30, 0x30, 0x00, 0x00, 0x00, 0x00 };
			string id;

			if (++ProcessId > 9999)
			{
				ProcessId = 1;
			}

			id = ProcessId.ToString("0000");

			id_data[7] = (byte)id[0];
			id_data[8] = (byte)id[1];
			id_data[9] = (byte)id[2];
			id_data[10] = (byte)id[3];

			Bytes.Join(id_data, ref data, id_data.Length);

			PrintOutStatus = new List<byte>();

			return this.SendBytesDirect(data);
		}
		/// <summary>
		/// デバイスに編集データを送信します。
		/// </summary>
		/// <param name="commands">編集済みのラベラーコマンド編集クラス</param>
		/// <returns>true:正常,false:異常</returns>
		public bool PrintOut(EPSON_Commands commands)
		{
			return PrintOut(commands.BytesData);
		}
		/// <summary>
		/// デバイスに編集データを送信します。
		/// </summary>
		/// <returns>true:正常,false:異常</returns>
		public bool PrintOut()
		{
			return PrintOut(Commands.BytesData);
		}

		#endregion

		#region 編集データ印字完了受信

		/// <summary>
		/// デバイスから編集データの印字完了を受信します。
		/// </summary>
		/// <param name="timeOut">タイムアウト値</param>
		/// <param name="error">異常（true:あり,false:なし）</param>
		/// <returns>true:正常,false:異常</returns>
		public bool PrintOutReceive(int timeOut, out bool error)
		{
			int flag = 0;
			byte[] data, id = new byte[4];
			bool status = false;

			errors();
			error = false;

			if (Device == DeviceType.LAN)
			{
				TimeOut = System.Environment.TickCount + timeOut;

				flag = PrintOutStatus.Count;

				for (int count = 2; count < flag; count++)
				{
					id[count - 2] = PrintOutStatus[count];
				}

				while (System.Environment.TickCount < TimeOut)
				{
					data = Socket.ReceiveBytes(TimeOut - System.Environment.TickCount);

					for (int count = 0; count < data.Length; count++)
					{
						if (flag == 0)
						{
							if (data[count] == 0x37)
							{
								flag++;
								PrintOutStatus.Add(data[count]);
							}
							else if ((data[count] & 0x93) == 0x10)
							{
								if ((data[count] & 0x08) != 0x00)
								{
									error = true;

									break;
								}
							}
						}
						else if (flag == 1)
						{
							if (data[count] == 0x22)
							{
								flag++;
								PrintOutStatus.Add(data[count]);
							}
							else
							{
								flag = 0;
								PrintOutStatus = new List<byte>();
							}
						}
						else if (flag >= 2 && flag <= 5)
						{
							id[flag - 2] = data[count];
							flag++;

							PrintOutStatus.Add(data[count]);
						}
						else if (flag == 6)
						{
							flag = 0;
							PrintOutStatus = new List<byte>();

							if (data[count] == 0x00)
							{
								if (ProcessId.ToString("0000") == Encoding.Default.GetString(id))
								{
									status = true;

									break;
								}
							}
						}
					}

					if (status || error) break;
				}
			}
			else
			{
				errors(-10001, "オープンされていません。");
			}

			return status;
		}
		/// <summary>
		/// デバイスから編集データの印字完了を受信します。
		/// </summary>
		/// <param name="timeOut">タイムアウト値</param>
		/// <returns>true:正常,false:異常</returns>
		public bool PrintOutReceive(int timeOut)
		{
			bool error;

			return PrintOutReceive(timeOut, out error);
		}

		#endregion

		#region ASB無効送信

		/// <summary>
		/// デバイスにASB無効を送信します。
		/// </summary>
		/// <returns>true:正常,false:異常</returns>
		public bool StopASB()
		{
			var data = new byte[] { GS, (byte)'a', 0x00 };

			return SendBytesDirect(data);
		}

		#endregion

		#region バッファクリア

		/// <summary>
		/// デバイスのバッファをクリアします。
		/// </summary>
		/// <param name="timeOut">タイムアウト値。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool BufferClear(int timeOut)
		{
			int flag = 0;
			var data = new byte[] { DLE, 0x14, 0x08, 0x01, 0x03, 0x14, 0x01, 0x06, 0x02, 0x08 };
			bool status = false;

			errors();

			if (Device == DeviceType.LAN)
			{
				Socket.SendBytesDirect(data);

				// 読み捨て
				Socket.ReceiveBytesDirect();

				TimeOut = System.Environment.TickCount + timeOut;

				while (System.Environment.TickCount < TimeOut)
				{
					data = Socket.ReceiveBytes(TimeOut - System.Environment.TickCount);

					for (int count = 0; count < data.Length; count++)
					{
						if (flag == 0)
						{
							if (data[count] == 0x37)
							{
								flag++;
							}
						}
						else if (flag == 1)
						{
							if (data[count] == 0x25)
							{
								flag++;
							}
							else
							{
								flag = 0;
							}
						}
						else if (flag == 2)
						{
							if (data[count] == 0x00)
							{
								status = true;

								break;
							}
							else
							{
								flag = 0;
							}
						}
					}

					if (status) break;
				}

			}
			else
			{
				errors(-10001, "オープンされていません。");
			}

			return status;
		}

		#endregion

		#region 状態取得

		/// <summary>
		/// デバイスの状態を取得します。
		/// </summary>
		/// <param name="statusCode">ステータスコードが格納されている文字列。</param>
		/// <param name="statusMessage">ステータス内容が格納されている文字列。</param>
		/// <param name="timeOut">タイムアウト値。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool Status(out string statusCode, out string statusMessage, int timeOut)
		{
			int flag = 0;
			var data = new byte[] { DLE, EOT, 0x01, DLE, EOT, 0x02, DLE, EOT, 0x03 };
			var code = new byte[3];
			bool status = false;

			errors();

			statusCode = "";
			statusMessage = "";

			if (Device == DeviceType.LAN)
			{
				Socket.SendBytesDirect(data);

				// 読み捨て
				Socket.ReceiveBytesDirect();

				TimeOut = System.Environment.TickCount + timeOut;

				while (System.Environment.TickCount < TimeOut)
				{
					data = Socket.ReceiveBytes(TimeOut - System.Environment.TickCount);

					for (int count = 0; count < data.Length; count++)
					{
						if ((data[count] & 0x93) == 0x12)
						{
							code[flag] = data[count];

							if (++flag >= 3)
							{
								status = true;

								break;
							}
						}
					}

					if (status) break;
				}

				if (status)
				{
					// ｽﾃｰﾀｽ1
					if ((code[0] & 0x08) != 0x00)
					{
						statusCode = "10";
						statusMessage = "オフライン";
					}
					else
					{
						statusCode = "11";
						statusMessage = "オンライン";
					}

					// ｽﾃｰﾀｽ2
					if ((code[1] & 0x40) != 0x00)
					{
						statusCode = "26";
						statusMessage = "エラー発生";
					}
					else if ((code[1] & 0x20) != 0x00)
					{
						statusCode = "25";
						statusMessage = "ペーパーエンド";
					}
					else if ((code[1] & 0x08) != 0x00)
					{
						statusCode = "23";
						statusMessage = "フィード中";
					}
					else if ((code[1] & 0x04) != 0x00)
					{
						statusCode = "22";
						statusMessage = "カバーオープン";
					}

					// ｽﾃｰﾀｽ3
					if ((code[2] & 0x40) != 0x00)
					{
						statusCode = "36";
						statusMessage = "自動復帰エラー";
					}
					else if ((code[2] & 0x20) != 0x00)
					{
						statusCode = "35";
						statusMessage = "復帰不可能エラー";
					}
					else if ((code[2] & 0x08) != 0x00)
					{
						statusCode = "33";
						statusMessage = "オートカッターエラー";
					}
					else if ((code[2] & 0x04) != 0x00)
					{
						statusCode = "32";
						statusMessage = "復帰可能エラー";
					}
				}
				else
				{
					statusCode = "99";
					statusMessage = "タイムアウト";

					status = true;
				}
			}
			else
			{
				errors(-10001, "オープンされていません。");
			}

			return status;
		}

		#endregion

		#region 状態要求1送信

		/// <summary>
		/// デバイスに状態要求1を送信します。
		/// </summary>
		/// <param name="timeOut">タイムアウト値。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool Status1Send(int timeOut)
		{
			var data = new byte[] { DLE, EOT, 0x01 };

			errors();

			// 読み捨て
			ReceiveBytesDirect();

			TimeOut = System.Environment.TickCount + timeOut;

			return SendBytesDirect(data);
		}
		/// <summary>
		/// デバイスに状態要求1を送信します。
		/// </summary>
		/// <returns>true:正常,false:異常</returns>
		public bool Status1Send()
		{
			return Status1Send(3000);
		}

		#endregion

		#region 状態要求1受信

		/// <summary>
		/// デバイスから状態要求1の返答を受信します。
		/// </summary>
		/// <param name="statusCode">ステータスコードが格納されている文字列。</param>
		/// <param name="statusMessage">ステータス内容が格納されている文字列。</param>
		/// <returns>true:受信完了,false:受信待ち</returns>
		public bool Status1Receive(out string statusCode, out string statusMessage)
		{
			byte[] data;
			bool status = false;

			statusCode = "";
			statusMessage = "";

			data = ReceiveBytesDirect();

			if (data.Length > 0)
			{
				if ((data[data.Length - 1] & 0x93) == 0x12)
				{
					if ((data[data.Length - 1] & 0x08) != 0x00)
					{
						statusCode = "10";
						statusMessage = "オフライン";
					}
					else
					{
						statusCode = "11";
						statusMessage = "オンライン";
					}

					status = true;
				}
			}
			else
			{
				if (System.Environment.TickCount >= TimeOut)
				{
					statusCode = "99";
					statusMessage = "タイムアウト";

					status = true;
				}
			}

			return status;
		}

		#endregion

		#region 状態要求2送信

		/// <summary>
		/// デバイスに状態要求2を送信します。
		/// </summary>
		/// <param name="timeOut">タイムアウト値。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool Status2Send(int timeOut)
		{
			var data = new byte[] { DLE, EOT, 0x02 };

			errors();

			// 読み捨て
			ReceiveBytesDirect();

			TimeOut = System.Environment.TickCount + timeOut;

			return SendBytesDirect(data);
		}
		/// <summary>
		/// デバイスに状態要求2を送信します。
		/// </summary>
		/// <returns>true:正常,false:異常</returns>
		public bool Status2Send()
		{
			return Status2Send(3000);
		}

		#endregion

		#region 状態要求2受信

		/// <summary>
		/// デバイスから状態要求2の返答を受信します。
		/// </summary>
		/// <param name="statusCode">ステータスコードが格納されている文字列。</param>
		/// <param name="statusMessage">ステータス内容が格納されている文字列。</param>
		/// <returns>true:受信完了,false:受信待ち</returns>
		public bool Status2Receive(out string statusCode, out string statusMessage)
		{
			byte[] data;
			bool status = false;

			statusCode = "";
			statusMessage = "";

			data = ReceiveBytesDirect();

			if (data.Length > 0)
			{
				if ((data[data.Length - 1] & 0x93) == 0x12)
				{
					if ((data[data.Length - 1] & 0x40) != 0x00)
					{
						statusCode = "26";
						statusMessage = "エラー発生";
					}
					else if ((data[data.Length - 1] & 0x20) != 0x00)
					{
						statusCode = "25";
						statusMessage = "ペーパーエンド";
					}
					else if ((data[data.Length - 1] & 0x08) != 0x00)
					{
						statusCode = "23";
						statusMessage = "フィード中";
					}
					else if ((data[data.Length - 1] & 0x04) != 0x00)
					{
						statusCode = "22";
						statusMessage = "カバーオープン";
					}
					else
					{
						statusCode = "00";
					}

					status = true;
				}
			}
			else
			{
				if (System.Environment.TickCount >= TimeOut)
				{
					statusCode = "99";
					statusMessage = "タイムアウト";

					status = true;
				}
			}

			return status;
		}

		#endregion

		#region 状態要求3送信

		/// <summary>
		/// デバイスに状態要求3を送信します。
		/// </summary>
		/// <param name="timeOut">タイムアウト値。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool Status3Send(int timeOut)
		{
			var data = new byte[] { DLE, EOT, 0x03 };

			errors();

			// 読み捨て
			ReceiveBytesDirect();

			TimeOut = System.Environment.TickCount + timeOut;

			return SendBytesDirect(data);
		}
		/// <summary>
		/// デバイスに状態要求3を送信します。
		/// </summary>
		/// <returns>true:正常,false:異常</returns>
		public bool Status3Send()
		{
			return Status3Send(3000);
		}

		#endregion

		#region 状態要求3受信

		/// <summary>
		/// デバイスから状態要求3の返答を受信します。
		/// </summary>
		/// <param name="statusCode">ステータスコードが格納されている文字列。</param>
		/// <param name="statusMessage">ステータス内容が格納されている文字列。</param>
		/// <returns>true:受信完了,false:受信待ち</returns>
		public bool Status3Receive(out string statusCode, out string statusMessage)
		{
			byte[] data;
			bool status = false;

			statusCode = "";
			statusMessage = "";

			data = ReceiveBytesDirect();

			if (data.Length > 0)
			{
				if ((data[data.Length - 1] & 0x93) == 0x12)
				{
					if ((data[data.Length - 1] & 0x40) != 0x00)
					{
						statusCode = "36";
						statusMessage = "自動復帰エラー";
					}
					else if ((data[data.Length - 1] & 0x20) != 0x00)
					{
						statusCode = "35";
						statusMessage = "復帰不可能エラー";
					}
					else if ((data[data.Length - 1] & 0x08) != 0x00)
					{
						statusCode = "33";
						statusMessage = "オートカッターエラー";
					}
					else if ((data[data.Length - 1] & 0x04) != 0x00)
					{
						statusCode = "32";
						statusMessage = "復帰可能エラー";
					}
					else
					{
						statusCode = "00";
					}

					status = true;
				}
			}
			else
			{
				if (System.Environment.TickCount >= TimeOut)
				{
					statusCode = "99";
					statusMessage = "タイムアウト";

					status = true;
				}
			}

			return status;
		}

		#endregion

		#region 状態要求4送信

		/// <summary>
		/// デバイスに状態要求4を送信します。
		/// </summary>
		/// <param name="timeOut">タイムアウト値。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool Status4Send(int timeOut)
		{
			var data = new byte[] { DLE, EOT, 0x04 };

			errors();

			// 読み捨て
			ReceiveBytesDirect();

			TimeOut = System.Environment.TickCount + timeOut;

			return SendBytesDirect(data);
		}
		/// <summary>
		/// デバイスに状態要求4を送信します。
		/// </summary>
		/// <returns>true:正常,false:異常</returns>
		public bool Status4Send()
		{
			return Status4Send(3000);
		}

		#endregion

		#region 状態要求4受信

		/// <summary>
		/// デバイスから状態要求4の返答を受信します。
		/// </summary>
		/// <param name="statusCode">ステータスコードが格納されている文字列。</param>
		/// <param name="statusMessage">ステータス内容が格納されている文字列。</param>
		/// <returns>true:受信完了,false:受信待ち</returns>
		public bool Status4Receive(out string statusCode, out string statusMessage)
		{
			byte[] data;
			bool status = false;

			statusCode = "";
			statusMessage = "";

			data = ReceiveBytesDirect();

			if (data.Length > 0)
			{
				if ((data[data.Length - 1] & 0x93) == 0x12)
				{
					if ((data[data.Length - 1] & 0x60) != 0x00)
					{
						statusCode = "46";
						statusMessage = "ロール紙エンド検出器に用紙なし";
					}
					else if ((data[data.Length - 1] & 0x0C) != 0x00)
					{
						statusCode = "43";
						statusMessage = "ロール紙ニヤエンド検出器に用紙なし";
					}
					else
					{
						statusCode = "00";
					}

					status = true;
				}
			}
			else
			{
				if (System.Environment.TickCount >= TimeOut)
				{
					statusCode = "99";
					statusMessage = "タイムアウト";

					status = true;
				}
			}

			return status;
		}

		#endregion

		#region 異常コード プロパティ

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

		#endregion

		#region 異常内容 プロパティ

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

		#endregion

		#region 異常の設定

		/// <summary>
		/// 異常の設定
		/// </summary>
		private void errors()
		{
			Error_code = 0;
			Error_message = "";
		}
		/// <summary>
		/// 異常の設定
		/// </summary>
		/// <param name="error_code">異常コード</param>
		/// <param name="comment">異常内容</param>
		private void errors(int error_code, string comment)
		{
			Error_code = error_code;
			Error_message = comment;
		}

		#endregion
	}

	/// <summary>
	/// ラベラーコマンド編集クラス（エプソン用）
	/// </summary>
	public class EPSON_Commands
	{
		#region 列挙型

		/// <summary>
		/// 機種名
		/// </summary>
		public enum Model
		{
			/// <summary>
			/// TM-T88IV
			/// </summary>
			TM_T88IV,
		}

		/// <summary>
		/// バーコード体系
		/// </summary>
		public enum BarcodeType
		{
			/// <summary>
			/// 
			/// </summary>
			UPCA,
			/// <summary>
			/// 
			/// </summary>
			UPCE,
			/// <summary>
			/// 
			/// </summary>
			JAN13,
			/// <summary>
			/// 
			/// </summary>
			JAN8,
			/// <summary>
			/// 
			/// </summary>
			CODE39,
			/// <summary>
			/// 
			/// </summary>
			ITF,
			/// <summary>
			/// 
			/// </summary>
			CODABAR,
			/// <summary>
			/// 
			/// </summary>
			CODE93,
			/// <summary>
			/// 
			/// </summary>
			CODE128,
		}


		/// <summary>
		/// 揃え
		/// </summary>
		public enum AlignType
		{
			/// <summary>
			/// 
			/// </summary>
			Left = 0,
			/// <summary>
			/// 
			/// </summary>
			Center = 1,
			/// <summary>
			/// 
			/// </summary>
			Right = 2,
		}

		#endregion

		private const byte ESC = 0x1B;
		private const byte FS = 0x1C;
		private const byte GS = 0x1D;

		private Model ModelType = EPSON_Commands.Model.TM_T88IV;
		private int Dpm = 7;
		private int RawDataCount = 0;
		private byte[] RawData = new byte[4096];

		#region コンストラクタ

		/// <summary>
		/// ラベラーコマンド編集クラス
		/// </summary>
		public EPSON_Commands()
			: this(EPSON_Commands.Model.TM_T88IV)
		{
		}
		/// <summary>
		/// ラベラーコマンド編集クラス
		/// </summary>
		/// <param name="modelType">機種を指定します。</param>
		public EPSON_Commands(EPSON_Commands.Model modelType)
		{
			ModelType = modelType;

			if (modelType == Model.TM_T88IV)
			{
				Dpm = 7;
			}

			RawDataCount = 0;
			RawData = new byte[4096];
		}

		#endregion

		#region 編集開始

		/// <summary>
		/// 編集の開始を宣言します。
		/// </summary>
		public void Start()
		{
			RawData = new byte[4096];

			Array.Clear(RawData, 0, RawData.Length);
			RawDataCount = 0;

			Initialize();

			CodeTable(1);

			KanjiCode(1);
		}

		#endregion

		#region 編集終了

		/// <summary>
		/// 編集の終了を宣言します。
		/// </summary>
		public void End()
		{
			if (RawDataCount >= 1)
			{
				if (RawData[RawDataCount - 1] == 0x0D || RawData[RawDataCount - 1] == 0x0A) return;
			}

			LineFeed();
		}

		#endregion

		#region プリンターの初期化

		/// <summary>
		/// プリンターの初期化
		/// </summary>
		public void Initialize()
		{
			var data = new byte[] { ESC, (byte)'@' };

			append(data);
		}

		#endregion

		#region 改行

		/// <summary>
		/// 改行
		/// </summary>
		public void LineFeed()
		{
			var data = new byte[] { 0x0A };

			append(data);
		}

		#endregion

		#region 行紙送り

		/// <summary>
		/// 行紙送り
		/// </summary>
		/// <param name="count">行数（0～255）</param>
		public void LineFeed(int count)
		{
			var data = new byte[] { ESC, (byte)'d', 0x00 };

			if (count >= 0 && count <= 255)
			{
				data[2] = (byte)count;

				append(data);
			}
		}

		#endregion

		#region ドット紙送り

		/// <summary>
		/// ドット紙送り
		/// </summary>
		/// <param name="dot">ドット（0～255）</param>
		public void DotFeed(int dot)
		{
			LineSpacing(dot);
			LineFeed();
			LineSpacing();
		}

		#endregion

		#region 改行量の設定

		/// <summary>
		/// 改行量の設定
		/// </summary>
		/// <param name="dot">ドット（0～255）</param>
		public void LineSpacing(int dot)
		{
			var data = new byte[] { ESC, (byte)'3', 0x00 };

			if (dot >= 0 && dot <= 255)
			{
				data[2] = (byte)dot;

				append(data);
			}
		}
		/// <summary>
		/// 初期改行量の設定
		/// </summary>
		public void LineSpacing()
		{
			var data = new byte[] { ESC, (byte)'2' };

			append(data);
		}

		#endregion

		#region 位置揃えの設定

		/// <summary>
		/// 位置揃えの設定
		/// </summary>
		/// <param name="alignType">揃え</param>
		public void Align(AlignType alignType)
		{
			var data = new byte[] { ESC, (byte)'a', 0x00 };

			data[2] = (byte)alignType;

			append(data);
		}

		#endregion

		#region 左マージンの設定

		/// <summary>
		/// 左マージンの設定
		/// </summary>
		/// <param name="dot">ドット（0～65535）</param>
		public void LeftMargin(int dot)
		{
			var data = new byte[] { GS, (byte)'L', 0x00, 0x00 };

			if (dot >= 0 && dot <= 65535)
			{
				data[2] = (byte)(dot % 0x100);
				data[3] = (byte)(dot / 0x100);

				append(data);
			}
		}
		/// <summary>
		/// 左マージンの設定解除
		/// </summary>
		public void LeftMargin()
		{
			LeftMargin(0);
		}

		#endregion

		#region 文字コードテーブルの選択

		/// <summary>
		/// 文字コードテーブルの選択
		/// </summary>
		/// <param name="codeTable">文字コードテーブル（0-8,16-19,254,255）</param>
		public void CodeTable(int codeTable)
		{
			var data = new byte[] { ESC, (byte)'t', 0x00 };

			if ((codeTable >= 0 && codeTable <= 8) || (codeTable >= 16 && codeTable <= 19) || codeTable == 254 || codeTable == 255)
			{
				data[2] = (byte)codeTable;

				append(data);
			}
		}

		#endregion

		#region 文字フォントの選択

		/// <summary>
		/// 文字フォントの選択
		/// </summary>
		/// <param name="type">フォントタイプ（A,B,C）</param>
		public void Font(char type)
		{
			var data = new byte[] { ESC, (byte)'M', 0x00 };

			if (type == 'A' || type == 'B' || type == 'C')
			{
				data[2] = (byte)((int)type - (int)'A');

				append(data);
			}
		}

		#endregion

		#region 文字サイズの指定

		/// <summary>
		/// 文字サイズの指定
		/// </summary>
		/// <param name="width">横倍率（1～8）</param>
		/// <param name="height">縦倍率（1～8）</param>
		public void Size(int width, int height)
		{
			var data = new byte[] { GS, (byte)'!', 0x00 };

			if (width >= 1 && width <= 8 && height >= 1 && height <= 8)
			{
				data[2] = (byte)((width - 1) * 0x10 + (height - 1));

				append(data);
			}
		}
		/// <summary>
		/// 文字サイズの指定解除
		/// </summary>
		public void Size()
		{
			Size(1, 1);
		}

		#endregion

		#region 拡大文字のスムージング指定・解除

		/// <summary>
		/// 拡大文字のスムージング指定・解除
		/// </summary>
		/// <param name="flag">true:指定, false:解除</param>
		public void Smoothing(bool flag)
		{
			var data = new byte[] { GS, (byte)'b', 0x00 };

			data[2] = (byte)(flag ? 1 : 0);

			append(data);
		}

		#endregion

		#region アンダーラインの指定・解除

		/// <summary>
		/// アンダーラインの指定・解除
		/// </summary>
		/// <param name="type">0:解除,1:1ドット,2:2ドット</param>
		public void UnderLine(int type)
		{
			var data = new byte[] { ESC, (byte)'-', 0x00, FS, (byte)'-', 0x00 };

			if (type >= 0 && type <= 2)
			{
				data[2] = (byte)type;
				data[5] = (byte)type;

				append(data);
			}
		}
		/// <summary>
		/// アンダーラインの解除
		/// </summary>
		public void UnderLine()
		{
			UnderLine(0);
		}

		#endregion

		#region 強調印字の指定・解除

		/// <summary>
		/// 強調印字の指定・解除
		/// </summary>
		/// <param name="flag">true:指定, false:解除</param>
		public void Emphasize(bool flag)
		{
			var data = new byte[] { ESC, (byte)'E', 0x00 };

			data[2] = (byte)(flag ? 1 : 0);

			append(data);
		}

		#endregion

		#region 白黒反転印字の指定・解除

		/// <summary>
		/// 白黒反転印字の指定・解除
		/// </summary>
		/// <param name="flag">true:指定, false:解除</param>
		public void Reverse(bool flag)
		{
			var data = new byte[] { GS, (byte)'B', 0x00 };

			data[2] = (byte)(flag ? 1 : 0);

			append(data);
		}

		#endregion

		#region 漢字コード体系の選択

		/// <summary>
		/// 漢字コード体系の選択
		/// </summary>
		/// <param name="code">0:ＪＩＳ,1:シフトＪＩＳ</param>
		public void KanjiCode(int code)
		{
			var data = new byte[] { FS, (byte)'C', 0x00 };

			if (code >= 0 && code <= 1)
			{
				data[2] = (byte)code;

				append(data);
			}
		}

		#endregion

		#region 漢字サイズの指定

		/// <summary>
		/// 漢字サイズの指定
		/// </summary>
		/// <param name="width">横倍率（1～2）</param>
		/// <param name="height">縦倍率（1～2）</param>
		public void KanjiSize(int width, int height)
		{
			var data = new byte[] { FS, (byte)'!', 0x00 };

			if (width >= 1 && width <= 2 && height >= 1 && height <= 2)
			{
				data[2] = (byte)((width - 1) * 0x04 + (height - 1) * 0x08);

				append(data);
			}
		}

		#endregion

		#region ラインの指定

		/// <summary>
		/// ラインの指定
		/// </summary>
		/// <param name="dot">長さ（1～2047：ドット単位）</param>
		/// <param name="type">タイプ（1:細線,2:太線,3:点線）</param>
		public void Line(int dot, int type)
		{
			int length, dot_y = 1;
			var data = new byte[] { GS, (byte)'(', (byte)'L', 0x00, 0x00, 0x30, 0x70, 0x30, 0x01, 0x01, 0x31, 0x00, 0x00, 0x00, 0x00 };
			byte line_bit = 0xFF;

			if (dot >= 1 && dot <= 2047 && type >= 1 && type <= 3)
			{
				if (type == 2) dot_y = 2;
				if (type == 3) line_bit = 0xF0;

				LineSpacing(dot_y);

				length = (dot / 8 + ((dot % 8) == 0 ? 0 : 1)) * dot_y;
				data[3] = (byte)((length + 10) % 0x100);
				data[4] = (byte)((length + 10) / 0x100);

				data[11] = (byte)(dot % 0x100);
				data[12] = (byte)(dot / 0x100);
				data[13] = (byte)(dot_y % 0x100);
				data[14] = (byte)(dot_y / 0x100);

				append(data);

				data = new byte[length];

				Bytes.Clear(line_bit, data, data.Length);

				append(data);

				LineFeed();

				LineSpacing();
			}
		}

		#endregion

		#region バーコードの印字

		/// <summary>
		/// バーコードの印字
		/// </summary>
		/// <param name="barcodeType">バーコード体系</param>
		/// <param name="narrowBar">ナローバー幅（2～6：ドット単位）</param>
		/// <param name="barHeight">バーコードの高さ（1～255：ドット単位）</param>
		/// <param name="code">バーコードデータのバイト配列</param>
		/// <param name="hriPostion">HRI 文字の印字位置（0:なし,1:バーコード上,2:バーコード下,3:両方）</param>
		/// <param name="hriFont">HRI 文字のフォント（A,B）</param>
		public void Barcode(BarcodeType barcodeType, int narrowBar, int barHeight, byte[] code, int hriPostion, char hriFont)
		{
			var data = new byte[] { GS, (byte)'k', 0x00, 0x00 };
			var data1 = new byte[] { GS, (byte)'w', 0x00 };
			var data2 = new byte[] { GS, (byte)'h', 0x00 };
			var data3 = new byte[] { GS, (byte)'H', 0x00 };
			var data4 = new byte[] { GS, (byte)'f', 0x00 };
			var bytes = new byte[0];
			byte[] barcode;

			barcode = code;

			switch (barcodeType)
			{
			case BarcodeType.UPCA:
				if (code.Length >= 11 && code.Length <= 12)
				{
					data[2] = (byte)'A';
				}
				break;

			case BarcodeType.UPCE:
				if (code.Length >= 11 && code.Length <= 12)
				{
					data[2] = (byte)'B';
				}
				break;

			case BarcodeType.JAN13:
				if (code.Length >= 12 && code.Length <= 13)
				{
					data[2] = (byte)'C';
				}
				break;

			case BarcodeType.JAN8:
				if (code.Length >= 7 && code.Length <= 8)
				{
					data[2] = (byte)'D';
				}
				break;

			case BarcodeType.CODE39:
				if (code.Length >= 1 && code.Length <= 255)
				{
					data[2] = (byte)'E';

					if (code[0] != (byte)'*')
					{
						barcode = new byte[code.Length + 2];

						barcode[0] = (byte)'*';
						Bytes.Copy(code, 0, barcode, 1, code.Length);
						barcode[code.Length + 1] = (byte)'*';
					}
				}
				break;

			case BarcodeType.ITF:
				if (code.Length >= 1 && code.Length <= 255 && code.Length % 2 == 0)
				{
					data[2] = (byte)'F';
				}
				break;

			case BarcodeType.CODABAR:
				if (code.Length >= 1 && code.Length <= 255)
				{
					data[2] = (byte)'G';

					if (!((code[0] >= (byte)'A' && code[0] <= (byte)'D') || (code[0] >= (byte)'a' && code[0] <= (byte)'d')))
					{
						barcode = new byte[code.Length + 2];

						barcode[0] = (byte)'A';
						Bytes.Copy(code, 0, barcode, 1, code.Length);
						barcode[code.Length + 1] = (byte)'A';
					}
				}
				break;

			case BarcodeType.CODE93:
				if (code.Length >= 1 && code.Length <= 255)
				{
					data[2] = (byte)'H';
				}
				break;

			case BarcodeType.CODE128:
				if (code.Length >= 2 && code.Length <= 255)
				{
					data[2] = (byte)'I';

					if (!(code[0] == 0x7B && (code[1] >= 0x41 && code[1] <= 0x43)))
					{
						barcode = new byte[code.Length + 2];

						barcode[0] = 0x7B;
						barcode[1] = 0x42;
						Bytes.Copy(code, 0, barcode, 2, code.Length);
					}
				}
				break;
			}

			if (data[2] != 0x00)
			{
				if (narrowBar >= 2 && narrowBar <= 6 && barHeight >= 1 && barHeight <= 255)
				{
					if (hriPostion >= 0 && hriPostion <= 3 && (hriFont == 'A' || hriFont == 'B'))
					{
						data[3] = (byte)barcode.Length;
						data1[2] = (byte)narrowBar;
						data2[2] = (byte)barHeight;
						data3[2] = (byte)hriPostion;
						data4[2] = (byte)(hriFont == 'A' ? 0 : 1);

						Bytes.Join(data4, ref bytes, data4.Length);
						Bytes.Join(data3, ref bytes, data3.Length);
						Bytes.Join(data2, ref bytes, data2.Length);
						Bytes.Join(data1, ref bytes, data1.Length);
						Bytes.Join(data, ref bytes, data.Length);
						Bytes.Join(barcode, ref bytes, barcode.Length);

						append(bytes);
					}
				}
			}
		}
		/// <summary>
		/// バーコードの印字
		/// </summary>
		/// <param name="barcodeType">バーコード体系</param>
		/// <param name="narrowBar">ナローバー幅（2～6：ドット単位）</param>
		/// <param name="barHeight">バーコードの高さ（1～255：ドット単位）</param>
		/// <param name="code">バーコードデータの文字列</param>
		/// <param name="hriPostion">HRI 文字の印字位置（0:なし,1:バーコード上,2:バーコード下,3:両方）</param>
		/// <param name="hriFont">HRI 文字のフォント（A,B）</param>
		public void Barcode(BarcodeType barcodeType, int narrowBar, int barHeight, string code, int hriPostion, char hriFont)
		{
			Barcode(barcodeType, narrowBar, barHeight, Encoding.Default.GetBytes(code), hriPostion, hriFont);
		}
		/// <summary>
		/// バーコードの印字
		/// </summary>
		/// <param name="barcodeType">バーコード体系</param>
		/// <param name="narrowBar">ナローバー幅（2～6：ドット単位）</param>
		/// <param name="barHeight">バーコードの高さ（1～255：ドット単位）</param>
		/// <param name="code">バーコードデータのバイト配列</param>
		/// <param name="hriFlag">HRI 文字の有無（true:あり,false:なし）</param>
		public void Barcode(BarcodeType barcodeType, int narrowBar, int barHeight, byte[] code, bool hriFlag)
		{
			Barcode(barcodeType, narrowBar, barHeight, code, hriFlag ? 2 : 0, 'A');
		}
		/// <summary>
		/// バーコードの印字
		/// </summary>
		/// <param name="barcodeType">バーコード体系</param>
		/// <param name="narrowBar">ナローバー幅（2～6：ドット単位）</param>
		/// <param name="barHeight">バーコードの高さ（1～255：ドット単位）</param>
		/// <param name="code">バーコードデータの文字列</param>
		/// <param name="hriFlag">HRI 文字の有無（true:あり,false:なし）</param>
		public void Barcode(BarcodeType barcodeType, int narrowBar, int barHeight, string code, bool hriFlag)
		{
			Barcode(barcodeType, narrowBar, barHeight, Encoding.Default.GetBytes(code), hriFlag ? 2 : 0, 'A');
		}

		#endregion

		#region CODE128の自動圧縮

		/// <summary>
		/// CODE128の自動圧縮
		/// </summary>
		/// <param name="data">バーコードの文字列</param>
		/// <returns></returns>
		public static byte[] CompactCode128(string data)
		{
			int[] code;
			int target;
			List<byte> barcode = new List<byte>();
			Byte[] bytes;

			bytes = Encoding.Default.GetBytes(data);
			code = new int[bytes.Length];

			for (int count = 0; count < bytes.Length; count++)
			{
				if (bytes[count] >= 0x30 && bytes[count] <= 0x39)
				{
					code[count] = 3;
				}
				else
				{
					code[count] = 2;
				}
			}

			for (int count = 0; count < code.Length; count++)
			{
				if (code[count] == 3)
				{
					if ((count + 1) < code.Length)
					{
						if (code[count + 1] != 3)
						{
							code[count] = 2;
						}

						count++;
					}
					else
					{
						code[count] = 2;
					}
				}
			}

			target = 0;

			for (int count = 0; count < code.Length; count++)
			{
				if (code[count] == 3)
				{
					target++;
				}
				else
				{
					if (target == 2)
					{
						code[count - 1] = 2;
						code[count - 2] = 2;
					}
				}
			}

			target = 0;

			for (int count = 0; count < code.Length; count++)
			{
				if (target != code[count])
				{
					target = code[count];

					barcode.Add(0x7B);
					barcode.Add((byte)(0x40 + target));
				}

				if (code[count] == 3)
				{
					barcode.Add((byte)((bytes[count] - 0x30) * 10 + bytes[count + 1] - 0x30));

					count++;
				}
				else
				{
					barcode.Add(bytes[count]);
				}
			}

			return barcode.ToArray();
		}

		#endregion

		#region 用紙のカット

		/// <summary>
		/// 用紙のカット
		/// </summary>
		/// <param name="mode">カット動作（true:フル,false:パーシャル）</param>
		/// <param name="pitch">カット位置までの送りピッチ（0～255）</param>
		public void Cut(bool mode, int pitch)
		{
			var data = new byte[] { GS, (byte)'V', 0x00, 0x00 };

			if (pitch >= 0 && pitch <= 255)
			{
				data[2] = (byte)(mode ? 'A' : 'B');
				data[3] = (byte)pitch;

				append(data);
			}
		}
		/// <summary>
		/// 用紙のカット
		/// </summary>
		/// <param name="mode">カット動作（true:フル,false:パーシャル）</param>
		public void Cut(bool mode)
		{
			Cut(mode, 0);
		}
		/// <summary>
		/// 用紙のカット
		/// </summary>
		public void Cut()
		{
			Cut(true, 0);
		}

		#endregion

		#region データの指定

		/// <summary>
		///データの指定
		/// </summary>
		/// <param name="data">データが格納されている文字列</param>
		public void Data(string data)
		{
			append(Encoding.Default.GetBytes(data));
		}
		/// <summary>
		///データの指定
		/// </summary>
		/// <param name="data">データが格納されているバイト配列</param>
		public void Data(byte[] data)
		{
			append(data);
		}

		#endregion

		#region ミリをドットに変換

		/// <summary>
		/// ミリメートルをドットに変換
		/// </summary>
		/// <param name="millimeter">変換するミリメートル</param>
		/// <returns>ドット数</returns>
		public int MmToDot(int millimeter)
		{
			return Dpm * millimeter;
		}

		#endregion

		#region 編集データを取得します。

		/// <summary>
		/// 編集データを取得します。
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

		#region 編集データのバイト数を取得します。

		/// <summary>
		/// 編集データのバイト数を取得します。
		/// </summary>
		public int BytesDataCount
		{
			get
			{
				return RawDataCount;
			}
		}

		#endregion

		#region データの追加(オーバーフロー回避)

		/// <summary>
		/// データの追加(オーバーフロー回避)
		/// </summary>
		/// <param name="data">追加するバイト配列</param>
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
