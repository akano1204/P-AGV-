using System;
using System.Text;

using HokushoClass.Rs232c;
using HokushoClass.Sockets;

namespace HokushoClass.Use.Labeler
{
	/// <summary>
	/// ラベラークラス（ゼブラ用）
	/// </summary>
	public class ZEBRA
	{
		#region 列挙型

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
		/// <param name="port">オープンするシリアルデバイスのポート№。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool Open(int port)
		{
			return Open(port, H_Rs232c.BaudRate._9600);
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

		#region	編集データ送信

		/// <summary>
		/// デバイスに編集データを送信します。
		/// </summary>
		/// <param name="data">送信するデータが格納されているバイト配列。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool PrintOut(byte[] data)
		{
			return this.SendBytesDirect(data);
		}
		/// <summary>
		/// デバイスに編集データを送信します。
		/// </summary>
		/// <param name="commands">編集済みのラベラーコマンド編集クラス</param>
		/// <returns>true:正常,false:異常</returns>
		public bool PrintOut(ZEBRA_Commands commands)
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

		#region 状態要求送信

		/// <summary>
		/// デバイスに状態要求を送信します。
		/// </summary>
		/// <param name="timeOut">タイムアウト値。</param>
		/// <returns>true:正常,false:異常</returns>
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
		/// デバイスに状態要求を送信します。
		/// </summary>
		/// <returns>true:正常,false:異常</returns>
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
					statusMessage = "オンライン";

					if (Status1[1] == "1")
					{
						statusCode = "b";
						statusMessage = "用紙切れ";
					}
					else if (Status2[3] == "1" && Status2[4] == "1")
					{
						statusCode = "p";
						statusMessage = "リボン切れ";
					}
					else if (Status2[2] == "1")
					{
						statusCode = "o";
						statusMessage = "ヘッドオープン";
					}
					else if (Status1[5] == "1")
					{
						statusCode = "f";
						statusMessage = "バッファ満杯";
					}
					else if (Status1[2] == "1")
					{
						statusCode = "c";
						statusMessage = "ポーズ";
					}
					else if (Status2[7] == "1")
					{
						statusCode = "3";
						statusMessage = "オンライン(剥離待機中)";
					}
					else if (Convert.ToInt32(Status1[4]) > 0 || Convert.ToInt32(Status2[8]) > 0)
					{
						statusCode = "2";
						statusMessage = "オンライン(処理中)";
					}

					status = true;
				}
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

			if (status)
			{
				Status1 = new string[0];
				Status2 = new string[0];
				Status3 = new string[0];
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
	/// ラベラーコマンド編集クラス（ゼブラ用）
	/// </summary>
	public class ZEBRA_Commands
	{
		#region 列挙型

		/// <summary>
		/// 機種名
		/// </summary>
		public enum Model
		{
			/// <summary>
			/// 解像度 600bpi
			/// </summary>
			_600bpi,
			/// <summary>
			/// 解像度 300bpi
			/// </summary>
			_300bpi,
			/// <summary>
			/// 解像度 203bpi
			/// </summary>
			_203bpi,
		}

		#endregion

		private int Dpm = 8;
		private int RawDataCount = 0;
		private byte[] RawData = new byte[4096];

		#region コンストラクタ

		/// <summary>
		/// ラベラーコマンド編集クラス
		/// </summary>
		public ZEBRA_Commands()
		{
			Dpm = 8;

			RawDataCount = 0;
			RawData = new byte[4096];
		}
		/// <summary>
		/// ラベラーコマンド編集クラス
		/// </summary>
		/// <param name="modelType">ゼブラの機種を指定します。</param>
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

			data = Encoding.Default.GetBytes("^XA^CI15");

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

			data = Encoding.Default.GetBytes("^XZ");

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

			data = Encoding.Default.GetBytes(String.Format("^LH{0},{1}", x, y));

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

			data = Encoding.Default.GetBytes(String.Format("^FO{0},{1}", x, y));

			append(data);
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

			data = Encoding.Default.GetBytes(String.Format("^FO{0},{1}", x, y));

			append(data);
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

			data = Encoding.Default.GetBytes(String.Format("^GB{0},{1},{2}^FS", x, y, dot));

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

			data = Encoding.Default.GetBytes(String.Format("^GB{0},{1},{2}^FS", x, y, dot));

			append(data);
		}

		#endregion

		#region デフォルトフォント指定

		/// <summary>
		/// デフォルトフォントを指定します。
		/// </summary>
		/// <param name="type">フォントのタイプを指定。(A～Z,0～9)</param>
		/// <param name="height">フォントの高さを指定。</param>
		/// <param name="width">フォントの幅を指定。</param>
		public void DefaultFont(string type, int height, int width)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(String.Format("^CF{0},{1},{2}", type, height, width));

			append(data);
		}
		/// <summary>
		/// デフォルトフォントを指定します。
		/// </summary>
		/// <param name="type">フォントのタイプを指定。(A～Z,0～9)</param>
		/// <param name="height">フォントの高さを指定。</param>
		public void DefaultFont(string type, int height)
		{
			DefaultFont(type, height, 0);
		}

		#endregion

		#region フォント指定

		/// <summary>
		/// フォントを指定します。
		/// </summary>
		/// <param name="type">フォントのタイプと向きを指定。(A～Z,0～9) + (N=標準,R=90ﾟ,I=180ﾟ,B=270ﾟ) ※時計方向</param>
		/// <param name="height">フォントの高さを指定。</param>
		/// <param name="width">フォントの幅を指定。</param>
		/// <param name="font_name">フォント名を指定。</param>
		public void Font(string type, int height, int width, string font_name)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(String.Format("^A@{0},{1},{2},{3}", type, height, width, font_name));

			append(data);
		}
		/// <summary>
		/// フォントを指定します。
		/// </summary>
		/// <param name="type">フォントのタイプと向きを指定。(A～Z,0～9) + (N=標準,R=90ﾟ,I=180ﾟ,B=270ﾟ) ※時計方向</param>
		/// <param name="height">フォントの高さを指定。</param>
		/// <param name="width">フォントの幅を指定。</param>
		public void Font(string type, int height, int width)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(String.Format("^A{0},{1},{2}", type, height, width));

			append(data);
		}
		/// <summary>
		/// フォントを指定します。
		/// </summary>
		/// <param name="type">フォントのタイプと向きを指定。(A～Z,0～9) + (N=標準,R=90ﾟ,I=180ﾟ,B=270ﾟ) ※時計方向</param>
		/// <param name="height">フォントのフォントの高さを指定。</param>
		public void Font(string type, int height)
		{
			Font(type, height, 0);
		}

		#endregion

		#region 英数字指定

		/// <summary>
		/// 英数字を指定します。(フォント指定あり)
		/// </summary>
		/// <param name="type">フォントのタイプと向きを指定。(A～Z,0～9) + (N=標準,R=90ﾟ,I=180ﾟ,B=270ﾟ) ※時計方向</param>
		/// <param name="height">フォントの高さを指定。</param>
		/// <param name="width">フォントの幅を指定。</param>
		/// <param name="text_data">英数字が格納されている文字列。</param>
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
		/// 英数字を指定します。(フォント指定あり)
		/// </summary>
		/// <param name="type">フォントのタイプと向きを指定。(A～Z,0～9) + (N=標準,R=90ﾟ,I=180ﾟ,B=270ﾟ) ※時計方向</param>
		/// <param name="height">フォントの高さを指定。</param>
		/// <param name="text_data">英数字が格納されている文字列。</param>
		public void TextA(string type, int height, string text_data)
		{
			TextA(type, height, 0, text_data);
		}
		/// <summary>
		/// 英数字を指定します。
		/// </summary>
		/// <param name="text_data">英数字が格納されている文字列。</param>
		public void TextA(string text_data)
		{
			TextA("", 0, 0, text_data);
		}

		#endregion

		#region 漢字指定

		/// <summary>
		/// 漢字を指定します。(フォント指定あり)
		/// </summary>
		/// <param name="type">フォントのタイプと向きを指定。(A～Z,0～9) + (N=標準,R=90ﾟ,I=180ﾟ,B=270ﾟ) ※時計方向</param>
		/// <param name="height">フォントの高さを指定。</param>
		/// <param name="width">フォントの幅を指定。</param>
		/// <param name="text_data">漢字が格納されている文字列。</param>
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
		/// 漢字を指定します。
		/// </summary>
		/// <param name="text_data">漢字が格納されている文字列。</param>
		public void TextB(string text_data)
		{
			TextB("", 0, 0, text_data);
		}

		#endregion

		#region 漢字指定2

		/// <summary>
		/// 漢字を指定します。(フォント指定あり)
		/// </summary>
		/// <param name="type">フォントのタイプと向きを指定。(A～Z,0～9) + (N=標準,R=90ﾟ,I=180ﾟ,B=270ﾟ) ※時計方向</param>
		/// <param name="height">フォントの高さを指定。</param>
		/// <param name="width">フォントの幅を指定。</param>
		/// <param name="text_data">漢字が格納されている文字列。</param>
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
		/// 漢字を指定します。
		/// </summary>
		/// <param name="text_data">漢字が格納されている文字列。</param>
		public void TextC(string text_data)
		{
			TextC("", 0, 0, text_data);
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

			data = Encoding.Default.GetBytes(command + format);

			append(data);
		}

		#endregion

		#region 編集データ プロパティ

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

		#region 編集データサイズ プロパティ

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
