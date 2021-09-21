using System;
using System.Text;

using HokushoClass.Rs232c;
using HokushoClass.Sockets;

namespace HokushoClass.Use.Labeler
{
	/// <summary>
	/// ラベラークラス（野崎印刷用）
	/// </summary>
	public class NOZAKI
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
		public NOZAKI_Commands Commands = new NOZAKI_Commands();

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
			return Open(port, H_Rs232c.BaudRate._19200);
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
		/// <param name="port">オープンするソケットのポート№。</param>
		/// <returns>true:正常,false:異常</returns>
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
			return Open(ip_address, 8000);
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

		#region 編集データ送信

		/// <summary>
		/// デバイスに編集データを送信します。
		/// </summary>
		/// <param name="data">送信するデータが格納されているバイト配列。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool PrintOut(byte[] data)
		{
			Bytes.Join(Encoding.Default.GetBytes("(#701)"), ref data, 6);

			return this.SendBytesDirect(data);
		}
		/// <summary>
		/// デバイスに編集データを送信します。
		/// </summary>
		/// <param name="commands">編集済みのラベラーコマンド編集クラス</param>
		/// <returns>true:正常,false:異常</returns>
		public bool PrintOut(NOZAKI_Commands commands)
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

		#region リセット要求送信

		/// <summary>
		/// デバイスにリセット要求を送信します。
		/// </summary>
		/// <param name="timeOut">タイムアウト値。</param>
		/// <returns>true:正常,false:異常</returns>
		public bool ResetSend(int timeOut)
		{
			errors();

			TimeOut = System.Environment.TickCount + timeOut;

			return SendBytesDirect(Encoding.Default.GetBytes("(#X)"));
		}
		/// <summary>
		/// デバイスにリセット要求を送信します。
		/// </summary>
		/// <returns>true:正常,false:異常</returns>
		public bool ResetSend()
		{
			return ResetSend(3000);
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

			data = Encoding.Default.GetBytes("(#703)");

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
					statusMessage = "データ入力待機中";
					break;

				case "01":
					statusMessage = "印字中";
					break;

				case "02":
					statusMessage = "印字停止中";
					break;

				case "10":
					statusMessage = "タグ・ラベルエンド";
					break;

				case "11":
					statusMessage = "リボンエンド";
					break;

				case "12":
					statusMessage = "ヘッドアップエラー";
					break;

				case "13":
					statusMessage = "通信フォーマットエラー";
					break;

				case "14":
					statusMessage = "受信キューバッファニアーフル";
					break;

				case "15":
					statusMessage = "ヘッド故障";
					break;

				case "17":
					statusMessage = "タグ・ラベルづまりエラー";
					break;

				case "18":
					statusMessage = "カッターエラー";
					break;

				case "99":
					statusMessage = "データロス";
					break;

				default:
					statusMessage = "その他エラー[" + statusCode + "]";
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
	/// ラベラーコマンド編集クラス（野崎印刷用）
	/// </summary>
	public class NOZAKI_Commands
	{
		#region 列挙型

		/// <summary>
		/// 機種名
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
		/// 回転指定
		/// </summary>
		public enum Turn
		{
			/// <summary>
			/// 回転しない
			/// </summary>
			_0 = 0,
			/// <summary>
			/// 90ﾟ右回転
			/// </summary>
			_90 = 3,
			/// <summary>
			/// 180ﾟ右回転
			/// </summary>
			_180 = 2,
			/// <summary>
			/// 270ﾟ右回転
			/// </summary>
			_270 = 1,
		}

		/// <summary>
		/// フォントピッチ
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
		/// フォントタイプＡ
		/// </summary>
		public enum FontTypeA
		{
			/// <summary>
			/// ＬＬ文字
			/// </summary>
			L,
			/// <summary>
			/// Ｌ文字
			/// </summary>
			H,
			/// <summary>
			/// ＯＣＲＢ
			/// </summary>
			M,
			/// <summary>
			/// ＯＣＲＡ
			/// </summary>
			N,
			/// <summary>
			/// Ｓ文字
			/// </summary>
			S,
			/// <summary>
			/// ＳＳ文字
			/// </summary>
			Q,
			/// <summary>
			/// Ｋ漢字
			/// </summary>
			K,
		}

		/// <summary>
		/// フォントタイプＫ
		/// </summary>
		public enum FontTypeK
		{
			/// <summary>
			/// ＪＩＳ漢字（１６×１６ドット）
			/// </summary>
			I,
			/// <summary>
			/// ＪＩＳ漢字（２４×２４ドット）
			/// </summary>
			J,
			/// <summary>
			/// ＪＩＳ漢字（３２×３２ドット）
			/// </summary>
			O,
		}

		/// <summary>
		/// バーコード種
		/// </summary>
		public enum BarcodeType
		{
			/// <summary>
			/// CODE39
			/// </summary>
			CODE39 = '1',
			/// <summary>
			/// インターリーブド2of5
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

		#region コンストラクタ

		/// <summary>
		/// ラベラーコマンド編集クラス
		/// </summary>
		public NOZAKI_Commands()
		{
			ModelType = NOZAKI_Commands.Model.FAD400;

			RawDataCount = 0;
			RawData = new byte[4096];
		}
		/// <summary>
		/// ラベラーコマンド編集クラス
		/// </summary>
		/// <param name="modelType">機種を指定します。</param>
		public NOZAKI_Commands(NOZAKI_Commands.Model modelType)
		{
			ModelType = modelType;

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

			append(Encoding.Default.GetBytes("("));
		}
		/// <summary>
		/// 編集の開始を宣言します。
		/// </summary>
		/// <param name="xOffset">X座標のオフセットを0.1mm単位で指定。</param>
		/// <param name="yOffset">Y座標のオフセットを0.1mm単位で指定。</param>
		/// <param name="dark">印字濃度を指定。(1～10)</param>
		public void Start(int xOffset, int yOffset, int dark)
		{
			Start();

			xOffset = value_check(xOffset, 0, 1040);
			yOffset = value_check(yOffset, 0, 2700);

			dark = value_check(dark, 1, 10);

			append(Encoding.Default.GetBytes(String.Format("#AX{0:0000}Y{1:0000}D{2:00}#F", xOffset, yOffset, dark)));
		}

		#endregion

		#region 編集終了

		/// <summary>
		/// 編集の終了を宣言します。
		/// </summary>
		/// <param name="copies">発行枚数を指定。(1～9999)</param>
		/// <param name="cut">カット間隔を指定。(0～100)</param>
		public void End(int copies, int cut)
		{
			copies = value_check(copies, 1, 9999);

			cut = value_check(cut, 0, 100);

			append(Encoding.Default.GetBytes(String.Format("#TP#Q{0}#VP{1:000}#Z)", copies, cut)));
		}
		/// <summary>
		/// 編集の終了を宣言します。
		/// </summary>
		/// <param name="cut">カットする場合は true。それ以外の場合は false。</param>
		public void End(bool cut)
		{
			End(1, cut ? 1 : 0);
		}
		/// <summary>
		/// 編集の終了を宣言します。
		/// </summary>
		public void End()
		{
			End(1, 0);
		}

		#endregion

		#region クリアキュー

		/// <summary>
		/// すでに受信しているデータを全てクリアします。
		/// </summary>
		public void Clear()
		{
			append(Encoding.Default.GetBytes("(#K)"));
		}

		#endregion

		#region 罫線指定

		/// <summary>
		/// 直線や四角形を印字します。
		/// </summary>
		/// <param name="x1">罫線の始点X座標を0.1mm単位で指定。</param>
		/// <param name="y1">罫線の始点Y座標を0.1mm単位で指定。</param>
		/// <param name="x2">罫線の終点X座標を0.1mm単位で指定。</param>
		/// <param name="y2">罫線の終点Y座標を0.1mm単位で指定。</param>
		/// <param name="width">罫線の幅を0.1mm単位で指定。(1～99)</param>
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

		#region 印字パラメータ指定（ANK）

		/// <summary>
		/// 印字する文字の種類や座標、印字方向、横縦倍率などを設定します。(ANK)
		/// </summary>
		/// <param name="fieldNo">フィールド№。</param>
		/// <param name="x">文字の基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">文字の基点Y座標を0.1mm単位で指定。</param>
		/// <param name="font">フォントの種類を指定。</param>
		/// <param name="xSize">文字の横倍率を指定。(1～9)</param>
		/// <param name="ySize">文字の縦倍率を指定。(1～9)</param>
		/// <param name="fontPitch">文字間のピッチをドット単位で指定。</param>
		/// <param name="textData">データが格納されている文字列。</param>
		/// <param name="turn">回転方向を指定。</param>
		/// <param name="option">拡張パラメータが格納されている文字列。</param>
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
		/// 印字する文字の種類や座標、印字方向、横縦倍率などを設定します。(ANK)
		/// </summary>
		/// <param name="fieldNo">フィールド№。</param>
		/// <param name="x">文字の基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">文字の基点Y座標を0.1mm単位で指定。</param>
		/// <param name="font">フォントの種類を指定。</param>
		/// <param name="xSize">文字の横倍率を指定。(1～9)</param>
		/// <param name="ySize">文字の縦倍率を指定。(1～9)</param>
		/// <param name="fontPitch">文字間のピッチをドット単位で指定。</param>
		/// <param name="textData">データが格納されている文字列。</param>
		public void TextA(int fieldNo, int x, int y, FontTypeA font, int xSize, int ySize, FontPitch fontPitch, string textData)
		{
			TextA(fieldNo, x, y, font, xSize, ySize, fontPitch, textData, Turn._0, "");
		}

		#endregion

		#region 印字パラメータ指定（漢字）

		/// <summary>
		/// 印字する文字の種類や座標、印字方向、横縦倍率などを設定します。(漢字)
		/// </summary>
		/// <param name="fieldNo">フィールド№。</param>
		/// <param name="x">文字の基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">文字の基点Y座標を0.1mm単位で指定。</param>
		/// <param name="font">フォントの種類を指定。</param>
		/// <param name="xSize">文字の横倍率を指定。(1～9)</param>
		/// <param name="ySize">文字の縦倍率を指定。(1～9)</param>
		/// <param name="fontPitch">文字間のピッチをドット単位で指定。</param>
		/// <param name="textData">データが格納されている文字列。</param>
		/// <param name="turn">回転方向を指定。</param>
		/// <param name="option">拡張パラメータが格納されている文字列。</param>
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
		/// 印字する文字の種類や座標、印字方向、横縦倍率などを設定します。(漢字)
		/// </summary>
		/// <param name="fieldNo">フィールド№。</param>
		/// <param name="x">文字の基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">文字の基点Y座標を0.1mm単位で指定。</param>
		/// <param name="font">フォントの種類を指定。</param>
		/// <param name="xSize">文字の横倍率を指定。(1～9)</param>
		/// <param name="ySize">文字の縦倍率を指定。(1～9)</param>
		/// <param name="fontPitch">文字間のピッチをドット単位で指定。</param>
		/// <param name="textData">データが格納されている文字列。</param>
		public void TextK(int fieldNo, int x, int y, FontTypeK font, int xSize, int ySize, FontPitch fontPitch, string textData)
		{
			TextK(fieldNo, x, y, font, xSize, ySize, fontPitch, textData, Turn._0, "");
		}

		#endregion

		#region バーコードパラメータ指定

		/// <summary>
		/// 印字するバーコードの種類や座標、印字方向、横倍率などを設定します。
		/// </summary>
		/// <param name="fieldNo">フィールド№。</param>
		/// <param name="x">バーコードの基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">バーコードの基点Y座標を0.1mm単位で指定。</param>
		/// <param name="barcodeType">バーコードの種類を指定。</param>
		/// <param name="width">横倍率を指定。(1～9)</param>
		/// <param name="height">バーコード高を0.1mm単位で指定。(1～2700)</param>
		/// <param name="barcodeData">バーコードデータが格納されている文字列。</param>
		/// <param name="subscript">添え字を付加する場合は true。それ以外の場合は false。</param>
		/// <param name="turn">回転方向を指定。</param>
		/// <param name="option">拡張パラメータが格納されている文字列。</param>
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
		/// 印字するバーコードの種類や座標、印字方向、横倍率などを設定します。
		/// </summary>
		/// <param name="fieldNo">フィールド№。</param>
		/// <param name="x">バーコードの基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">バーコードの基点Y座標を0.1mm単位で指定。</param>
		/// <param name="barcodeType">バーコードの種類を指定。</param>
		/// <param name="width">横倍率を指定。(1～9)</param>
		/// <param name="height">バーコード高を0.1mm単位で指定。(1～2700)</param>
		/// <param name="barcodeData">バーコードデータが格納されている文字列。</param>
		/// <param name="subscript">添え字を付加する場合は true。それ以外の場合は false。</param>
		public void Barcode(int fieldNo, int x, int y, BarcodeType barcodeType, int width, int height, string barcodeData, bool subscript)
		{
			Barcode(fieldNo, x, y, barcodeType, width, height, barcodeData, subscript, Turn._0, "");
		}
		/// <summary>
		/// 印字するバーコードの種類や座標、印字方向、横倍率などを設定します。
		/// </summary>
		/// <param name="fieldNo">フィールド№。</param>
		/// <param name="x">バーコードの基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">バーコードの基点Y座標を0.1mm単位で指定。</param>
		/// <param name="barcodeType">バーコードの種類を指定。</param>
		/// <param name="width">横倍率を指定。(1～9)</param>
		/// <param name="height">バーコード高を0.1mm単位で指定。(1～2700)</param>
		/// <param name="barcodeData">バーコードデータが格納されている文字列。</param>
		/// <param name="turn">回転方向を指定。</param>
		public void Barcode(int fieldNo, int x, int y, BarcodeType barcodeType, int width, int height, string barcodeData, Turn turn)
		{
			Barcode(fieldNo, x, y, barcodeType, width, height, barcodeData, false, turn, "");
		}
		/// <summary>
		/// 印字するバーコードの種類や座標、印字方向、横倍率などを設定します。
		/// </summary>
		/// <param name="fieldNo">フィールド№。</param>
		/// <param name="x">バーコードの基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">バーコードの基点Y座標を0.1mm単位で指定。</param>
		/// <param name="barcodeType">バーコードの種類を指定。</param>
		/// <param name="width">横倍率を指定。(1～9)</param>
		/// <param name="height">バーコード高を0.1mm単位で指定。(1～2700)</param>
		/// <param name="barcodeData">バーコードデータが格納されている文字列。</param>
		public void Barcode(int fieldNo, int x, int y, BarcodeType barcodeType, int width, int height, string barcodeData)
		{
			Barcode(fieldNo, x, y, barcodeType, width, height, barcodeData, false, Turn._0, "");
		}

		#endregion

		#region 拡張バーコードパラメータ

		/// <summary>
		/// バーコードのドットサイズを変更する設定を取得します。（JAN/EAN,UPC-A,CODE128）
		/// </summary>
		/// <param name="width">細バーサイズを0.1mm単位で指定。</param>
		public static string BarcodeParameter1(int width)
		{
			width = value_check(width, 0, 99);

			return String.Format("#8{0:00}{1:00}{2:0000}{3:000}{4}", 0, width, 0, 0, "M");
		}
		/// <summary>
		/// バーコードのドットサイズを変更する設定を取得します。（JAN/EAN,UPC-A,CODE128以外）
		/// </summary>
		/// <param name="width1">細バーサイズを0.1mm単位で指定。</param>
		/// <param name="width2">太バーサイズを0.1mm単位で指定。</param>
		public static string BarcodeParameter2(int width1, int width2)
		{
			width1 = value_check(width1, 0, 99);
			width2 = value_check(width2, 0, 99);

			return String.Format("#8{0:00}{1:00}{2:00}{3:00}{4:00}{5}{6}", width1, width1, width2, width2, width1, 0, "M");
		}

		#endregion

		#region ナンバリングパラメータ

		/// <summary>
		/// ナンバリングの設定を取得します。
		/// </summary>
		/// <param name="counter">増減値を指定。</param>
		public static string NumberingParameter(int counter)
		{
			counter = value_check(counter, -99, 99);

			return String.Format("#5{0}{1:00}{2:00}", counter < 0 ? 1 : 0, 1, Math.Abs(counter));
		}

		#endregion

		#region 黒白反転

		/// <summary>
		/// 文字を黒白反転して印字します。
		/// </summary>
		/// <param name="x1">反転の始点X座標を0.1mm単位で指定。</param>
		/// <param name="y1">反転の始点Y座標を0.1mm単位で指定。</param>
		/// <param name="x2">反転の終点X座標を0.1mm単位で指定。</param>
		/// <param name="y2">反転の終点Y座標を0.1mm単位で指定。</param>
		public void Reverse(int x1, int y1, int x2, int y2)
		{
			x1 = value_check(x1, 0, 1040);
			y1 = value_check(y1, 0, 2700);
			x2 = value_check(x2, 0, 1040);
			y2 = value_check(y2, 0, 2700);

			append(Encoding.Default.GetBytes(String.Format("#N{0:0000}{1:0000}{2:0000}{3:0000}", x1, y1, x2, y2)));
		}

		#endregion

		#region コマンド指定

		/// <summary>
		///コマンドを直接指定します。
		/// </summary>
		/// <param name="command">コマンドが格納されている文字列。</param>
		public void Format(string command)
		{
			append(Encoding.Default.GetBytes(command));
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

		#region 値のチェック

		private static int value_check(int value, int min, int max)
		{
			value = (value < min ? min : value);
			value = (value > max ? max : value);

			return value;
		}

		#endregion
	}
}
