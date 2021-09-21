using System;
using System.Text;

using HokushoClass.Rs232c;
using HokushoClass.Sockets;

namespace HokushoClass.Use.Labeler
{
	/// <summary>
	/// ラベラークラス（サトー用）
	/// </summary>
	public class SATO
	{
		#region 列挙型

		/// <summary>
		/// ACKタイプ
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
		/// コマンド編集クラス
		/// </summary>
		public SATO_Commands Commands = new SATO_Commands();

		private DeviceType Device = DeviceType.NONE;
		private H_Rs232c Com;
		private H_Socket Socket;
		private int Error_code;
		private string Error_message;
		private int TimeOut;

		#region オープン

		/// <summary>
		/// デバイスをオープンします。
		/// </summary>
		/// <param name="port">オープンするシリアルデバイスのポート№。</param>
		/// <param name="baudRate">オープンするデバイスのボーレート。</param>
		/// <returns>true:正常,false:異常</returns>
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
				errors(-10000, "既にオープンされています。");
			}

			return status;
		}
		/// <summary>
		/// デバイスをオープンします。
		/// </summary>
		/// <param name="port">オープンするシリアルデバイスのポート№。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool Open(int port)
		{
			return Open(port, H_Rs232c.BaudRate._9600);
		}
		/// <summary>
		/// デバイスをオープンします。
		/// </summary>
		/// <param name="port">オープンするシリアルデバイスのポート№。</param>
		/// <param name="baudRate">オープンするシリアルデバイスのボーレート。</param>
		/// <returns>true:正常,false:異常</returns>
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
		/// デバイスをオープンします。
		/// </summary>
		/// <param name="ip_address">オープンするネットワークデバイスのＩＰアドレス。</param>
		/// <returns>true:正常,false:異常</returns>
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
				errors(-10000, "既にオープンされています。");
			}

			return status;
		}

		#endregion

		#region クローズ

		/// <summary>
		/// デバイスをクローズします。
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

		#region  送信（バイト配列）

		/// <summary>
		/// デバイスにバイト配列のデータを送信します。
		/// </summary>
		/// <param name="data">送信するデータが格納されているバイト配列。</param>
		/// <returns>true:正常,false:異常</returns>
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
				errors(-10001, "オープンされていません。");
			}

			return status;
		}

		#endregion

		#region  受信（バイト配列）

		/// <summary>
		/// デバイスからバイト配列でデータを受信します。
		/// </summary>
		/// <param name="bytes_size">受信したバイト数。未受信のときは、-1。</param>
		/// <returns>受信したバイト配列</returns>
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

		#region 編集データ送信

		/// <summary>
		/// デバイスに編集データを送信します。
		/// </summary>
		/// <param name="data">送信するデータが格納されているバイト配列。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool PrintOut(byte[] data)
		{
			return this.SendBytes(data);
		}
		/// <summary>
		/// デバイスに編集データを送信します。
		/// </summary>
		/// <param name="commands">編集済みのラベラーコマンド編集クラス</param>
		/// <returns>true:正常,false:異常</returns>
		public bool PrintOut(SATO_Commands commands)
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

		#region 返答受信

		/// <summary>
		/// デバイスから返答を受信します。
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

		#region 状態要求送信

		/// <summary>
		/// デバイスに状態要求を送信します。
		/// </summary>
		/// <param name="timeOut">タイムアウト値。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool StatusSend(int timeOut)
		{
			byte[] data = new byte[1];

			errors();

			TimeOut = System.Environment.TickCount + timeOut;

			data[0] = 0x05;

			return SendBytesDirect(data);
		}
		/// <summary>
		/// デバイスに状態要求を送信します。
		/// </summary>
		/// <returns></returns>
		public bool StatusSend()
		{
			return StatusSend(3000);
		}

		#endregion

		#region 状態要求受信

		/// <summary>
		/// デバイスから状態要求の返答を受信します。
		/// </summary>
		/// <param name="statusCode">ステータスコードが格納されている文字列。</param>
		/// <param name="statusMessage">ステータス内容が格納されている文字列。</param>
		/// <returns>true:受信完了,false:受信待ち</returns>
		public bool StatusReceive(out string statusCode, out string statusMessage)
		{
			int counter;

			return StatusReceive(out statusCode, out statusMessage, out counter);
		}
		/// <summary>
		/// デバイスから状態要求の返答を受信します。
		/// </summary>
		/// <param name="statusCode">ステータスコードが格納されている文字列。</param>
		/// <param name="statusMessage">ステータス内容が格納されている文字列。</param>
		/// <param name="statusCounter">ステータス内のカウンター値。(ステータスL:貼付完了枚数、ステータス3:残り印字枚数)</param>
		/// <returns>true:受信完了,false:受信待ち</returns>
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
					statusMessage = "イニシャル";
					break;

				case "0":
					statusMessage = "オフライン";
					break;

				case "A":
				case "B":
				case "C":
				case "D":
					statusMessage = "オンライン";
					break;

				case "G":
					statusMessage = "オンライン(印字中)";
					break;

				case "M":
					statusMessage = "オンライン(印字待ち)";
					break;

				case "S":
					statusMessage = "オンライン(編集中)";
					break;

				case "o":
					statusMessage = "オンライン(貼付中)";
					break;

				case "s":
					statusMessage = "オンライン(通過中)";
					break;

				case "1":
				case "H":
				case "N":
				case "T":
				case "p":
				case "t":
					statusMessage = "ラベル・リボンニアエンド";
					break;

				case "2":
				case "I":
				case "O":
				case "U":
				case "q":
				case "u":
					statusMessage = "バッファニアフル";
					break;

				case "3":
				case "P":
				case "J":
				case "V":
				case "r":
				case "v":
					statusMessage = "ラベル・リボンニアエンド＆バッファニアフル";
					break;

				case "a":
					statusMessage = "受信バッファフル";
					break;

				case "b":
					statusMessage = "ヘッドオープン";
					break;

				case "c":
					statusMessage = "ペーパーエンド";
					break;

				case "d":
					statusMessage = "リボンエンド";
					break;

				case "e":
					statusMessage = "メディアエラー";
					break;

				case "f":
					statusMessage = "センサーエラー";
					break;

				case "g":
					statusMessage = "ヘッドエラー";
					break;

				case "h":
					statusMessage = "カバーオープン";
					break;

				case "i":
					statusMessage = "カードエラー";
					break;

				case "j":
					statusMessage = "カッターエラー";
					break;

				case "k":
					statusMessage = "その他のエラー";
					break;

				case "4":
				case "l":
					statusMessage = "貼付エラー";
					break;

				default:
					statusMessage = "電源OFF or 通信異常";
					break;
				}

				status = true;
			}
			else
			{
				if (System.Environment.TickCount >= TimeOut)
				{
					statusCode = "";
					statusMessage = "電源OFF or 通信異常";

					status = true;
				}
			}

			return status;
		}

		#endregion

		#region キャンセル指定送信

		/// <summary>
		/// デバイスにキャンセル指定を送信します。
		/// </summary>
		/// <returns>true:正常,false:異常</returns>
		public bool CancelSend()
		{
			byte[] data = new byte[1];

			errors();

			data[0] = 0x18;

			return SendBytesDirect(data);
		}

		#endregion

		#region キャンセル指定受信

		/// <summary>
		/// デバイスからキャンセル指定の返答を受信します。
		/// </summary>
		public AckType CancelReceive()
		{
			return Receive();
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
	/// ラベラーコマンド編集クラス（サトー用）
	/// </summary>
	public class SATO_Commands
	{
		#region 列挙型

		/// <summary>
		/// 機種名
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

		#region コンストラクタ

		/// <summary>
		/// ラベラーコマンド編集クラス
		/// </summary>
		public SATO_Commands()
		{
			ModelType = SATO_Commands.Model.MR400e;
			Dpm = 8;

			RawDataCount = 0;
			RawData = new byte[4096];
		}
		/// <summary>
		/// ラベラーコマンド編集クラス
		/// </summary>
		/// <param name="modelType">サトーの機種を指定します。</param>
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

		#region 編集開始

		/// <summary>
		/// 編集の開始を宣言します。
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

		#region 編集終了

		/// <summary>
		/// 編集の終了を宣言します。
		/// </summary>
		public void End()
		{
			byte[] data;

			data = Encoding.Default.GetBytes(ESC + "Z");

			append(data);
		}

		#endregion

		#region 用紙サイズ指定

		/// <summary>
		/// 用紙サイズを指定します。
		/// </summary>
		/// <param name="x">用紙の幅をmm単位で指定。</param>
		/// <param name="y">用紙の長さをmm単位で指定。</param>
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

		#region 基点補正

		/// <summary>
		/// 基点補正を指定します。
		/// </summary>
		/// <param name="x">横方向の補正値をmm単位で指定。</param>
		/// <param name="y">縦方向の補正値をmm単位で指定。</param>
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

		#region 印字位置指定

		/// <summary>
		/// 印字位置を指定します。
		/// </summary>
		/// <param name="x">横方向の座標をmm単位で指定。</param>
		/// <param name="y">縦方向の座標をmm単位で指定。</param>
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
		/// 印字位置を指定します。
		/// </summary>
		/// <param name="x">横方向の座標をmm単位で指定。</param>
		/// <param name="y">縦方向の座標をmm単位で指定。</param>
		/// <param name="x_half">横方向の座標に0.5mm加算する場合は true。それ以外の場合は false。</param>
		/// <param name="y_half">縦方向の座標に0.5mm加算する場合は true。それ以外の場合は false。</param>
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

		#region 罫線、枠線印字指定

		/// <summary>
		/// 罫線、枠線印字を指定します。
		/// </summary>
		/// <param name="x">横方向の線の終了座標をmm単位で指定。</param>
		/// <param name="y">縦方向の線の終了座標をmm単位で指定。</param>
		/// <param name="dot">線の幅をドット単位で指定。</param>
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
		/// 罫線、枠線印字を指定します。
		/// </summary>
		/// <param name="x">横方向の線の終了座標をmm単位で指定。</param>
		/// <param name="y">縦方向の線の終了座標をmm単位で指定。</param>
		/// <param name="x_half">横方向の座標に0.5mm加算する場合は true。それ以外の場合は false。</param>
		/// <param name="y_half">縦方向の座標に0.5mm加算する場合は true。それ以外の場合は false。</param>
		/// <param name="dot">線の幅をドット単位で指定。</param>
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

		#region フォントサイズ指定

		/// <summary>
		/// フォントサイズを指定します。
		/// </summary>
		/// <param name="x">横方向の倍率を指定。</param>
		/// <param name="y">縦方向の倍率で指定。</param>
		public void FontSize(int x, int y)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(ESC + String.Format("L{0:00}{1:00}", x, y));

			append(data);
		}

		#endregion

		#region データ指定

		/// <summary>
		/// データを指定します。
		/// </summary>
		/// <param name="command">コマンド部のデータが格納されている文字列。</param>
		/// <param name="format">フォーマット部のデータが格納されている文字列。</param>
		public void FormatData(string command, string format)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(ESC + command + format);

			append(data);
		}

        /// <summary>
        /// データを指定します。(byte版)
        /// </summary>
        /// <param name="command">コマンド部のデータが格納されている文字列。</param>
        /// <param name="format">フォーマット部のデータが格納されている文字列。</param>
        public void FormatData(string command, byte[] format)
        {
            byte[] data;

            data = Encoding.Default.GetBytes(ESC + command);

            append(data);
            append(format);
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
