using System;
using System.Text;

using HokushoClass.Rs232c;
using HokushoClass.Sockets;

namespace HokushoClass.Use.Labeler
{
	/// <summary>
	/// ラベラークラス（東芝テック用）
	/// </summary>
	public class TEC
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
		public TEC_Commands Commands = new TEC_Commands();

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
			return this.SendBytesDirect(data);
		}
		/// <summary>
		/// デバイスに編集データを送信します。
		/// </summary>
		/// <param name="commands">編集済みのラベラーコマンド編集クラス</param>
		/// <returns>true:正常,false:異常</returns>
		public bool PrintOut(TEC_Commands commands)
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
			byte[] data;

			errors();

			TimeOut = System.Environment.TickCount + timeOut;

			data = Encoding.Default.GetBytes("{WR|}");

			return SendBytesDirect(data);
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

			data = Encoding.Default.GetBytes("{WS|}");

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
						statusMessage = "待機中";
						break;

					case "01":
						statusMessage = "ヘッドオープン";
						break;

					case "02":
						statusMessage = "動作中";
						break;

					case "04":
						statusMessage = "ポーズ";
						break;

					case "05":
						statusMessage = "剥離待ち";
						break;

					case "06":
						statusMessage = "コマンドエラー";
						break;

					case "07":
						statusMessage = "通信エラー";
						break;

					case "11":
						statusMessage = "ペーパーエラー";
						break;

					case "12":
						statusMessage = "カッターエラー";
						break;

					case "13":
						statusMessage = "ペーパーエンド";
						break;

					case "15":
						statusMessage = "ヘッドオープン(動作中)";
						break;

					case "17":
						statusMessage = "サーマルヘッド断線エラー";
						break;

					case "18":
						statusMessage = "サーマルヘッド温度エラー";
						break;

					case "22":
						statusMessage = "カバーオープン(動作中)";
						break;

					case "40":
						statusMessage = "発行終了";
						break;

					case "41":
						statusMessage = "フィード終了";
						break;

					default:
						statusMessage = "その他エラー[" + statusCode + "]";
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
	/// ラベラーコマンド編集クラス（東芝テック用）
	/// </summary>
	public class TEC_Commands
	{
		#region 列挙型

		/// <summary>
		/// 機種名
		/// </summary>
		public enum Model
		{
			/// <summary>
			/// B-SA4T
			/// </summary>
			B_SA4T,
		}

		/// <summary>
		/// センサー
		/// </summary>
		public enum Sensor
		{
			/// <summary>
			/// センサーなし
			/// </summary>
			None = 0,
			/// <summary>
			/// 反射センサー
			/// </summary>
			Reflect = 1,
			/// <summary>
			/// 透過センサー
			/// </summary>
			Transmit = 2,
		}

		/// <summary>
		/// ラベル
		/// </summary>
		public enum Label
		{
			/// <summary>
			/// 感熱紙
			/// </summary>
			ThermalPaper = 0,
			/// <summary>
			/// 熱転写
			/// </summary>
			HeatTranscription = 1,
		}

		/// <summary>
		/// 発行モード
		/// </summary>
		public enum PrintMode
		{
			/// <summary>
			/// 連続印刷
			/// </summary>
			C,
			/// <summary>
			/// 剥離発行
			/// </summary>
			D,
			/// <summary>
			/// 剥離発行(センサー無視)
			/// </summary>
			E,
		}

		/// <summary>
		/// 印字方向
		/// </summary>
		public enum Direction
		{
			/// <summary>
			/// 頭出し印字
			/// </summary>
			Standard = 1,
			/// <summary>
			/// 尻出し印字
			/// </summary>
			Back = 0,
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
			_90 = 1,
			/// <summary>
			/// 180ﾟ右回転
			/// </summary>
			_180 = 2,
			/// <summary>
			/// 270ﾟ右回転
			/// </summary>
			_270 = 3,
		}

		/// <summary>
		/// ビットマップフォント
		/// </summary>
		public enum BitmapFonts
		{
			/// <summary>
			/// タイムスローマン(中)
			/// </summary>
			A,
			/// <summary>
			/// タイムスローマン(中)
			/// </summary>
			B,
			/// <summary>
			/// タイムスローマン(肉太)
			/// </summary>
			C,
			/// <summary>
			/// タイムスローマン(肉太)
			/// </summary>
			D,
			/// <summary>
			/// タイムスローマン(肉太)
			/// </summary>
			E,
			/// <summary>
			/// タイムスローマン(斜体)
			/// </summary>
			F,
			/// <summary>
			/// ヘルベチカ(中)
			/// </summary>
			G,
			/// <summary>
			/// ヘルベチカ(中)
			/// </summary>
			H,
			/// <summary>
			/// ヘルベチカ(中)
			/// </summary>
			I,
			/// <summary>
			/// ヘルベチカ(肉太)
			/// </summary>
			J,
			/// <summary>
			/// ヘルベチカ(肉太)
			/// </summary>
			K,
			/// <summary>
			/// ヘルベチカ(斜体)
			/// </summary>
			L,
			/// <summary>
			/// プレゼンテーション(肉太)
			/// </summary>
			M,
			/// <summary>
			/// レターゴシック(中)
			/// </summary>
			N,
			/// <summary>
			/// プレステージエリート(中)
			/// </summary>
			O,
			/// <summary>
			/// プレステージエリート(肉太)
			/// </summary>
			P,
			/// <summary>
			/// クーリエ(中)
			/// </summary>
			Q,
			/// <summary>
			/// クーリエ(肉太)
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
			/// 漢字(16x16) 角ゴシック体
			/// </summary>
			U,
			/// <summary>
			/// 漢字(24x24) 角ゴシック体
			/// </summary>
			V,
			/// <summary>
			/// 漢字(32x32) 角ゴシック体
			/// </summary>
			W,
			/// <summary>
			/// 漢字(48x48) 角ゴシック体
			/// </summary>
			X,
			/// <summary>
			/// ゴシック725ブラック
			/// </summary>
			q,
			/// <summary>
			/// 漢字(24x24) 明朝体
			/// </summary>
			v,
			/// <summary>
			/// 漢字(32x32) 明朝体
			/// </summary>
			w,
		}

		/// <summary>
		/// アウトラインフォント
		/// </summary>
		public enum OutlineFonts
		{
			/// <summary>
			/// TEC FONT1(ヘルべチカ[肉太])
			/// </summary>
			A,
			/// <summary>
			/// TEC FONT1(ヘルべチカ[肉太]プロポーショナル)
			/// </summary>
			B,
			/// <summary>
			/// 価格フォント1
			/// </summary>
			E,
			/// <summary>
			/// 価格フォント2
			/// </summary>
			F,
			/// <summary>
			/// 価格フォント3
			/// </summary>
			G,
			/// <summary>
			/// DUTCH801ボールド(タイムスローマン プロポーショナル)
			/// </summary>
			H,
			/// <summary>
			/// BRUSH738レギュラー(ポップ プロポーショナル)
			/// </summary>
			I,
			/// <summary>
			/// GOTHIC725ブラック(プロポーショナル)
			/// </summary>
			J,
		}

		/// <summary>
		/// バーコード種(JAN,UPC,CODE93,CODE128)
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
		/// バーコード種(ITF,CODE39,NW7)
		/// </summary>
		public enum BarcodeType2
		{
			/// <summary>
			/// インターリーブド2of5
			/// </summary>
			ITF = '2',
			/// <summary>
			/// CODE39(スタンダード)
			/// </summary>
			CODE39 = '3',
			/// <summary>
			/// NW7
			/// </summary>
			NW7 = '4',
			/// <summary>
			/// CODE39(フルアスキー)
			/// </summary>
			CODE39_FULL = 'B',
		}

		#endregion

		private Model ModelType = TEC_Commands.Model.B_SA4T;
		private Sensor SensorType = TEC_Commands.Sensor.Transmit;
		private Label LabelType = TEC_Commands.Label.ThermalPaper;
		private int RawDataCount = 0;
		private byte[] RawData = new byte[4096];

		#region コンストラクタ

		/// <summary>
		/// ラベラーコマンド編集クラス
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
		/// ラベラーコマンド編集クラス
		/// </summary>
		/// <param name="modelType">機種を指定します。</param>
		/// <param name="sensorType">センサー種別を指定します。</param>
		/// <param name="labelType">ラベル種別を指定します。</param>
		public TEC_Commands(TEC_Commands.Model modelType, TEC_Commands.Sensor sensorType, TEC_Commands.Label labelType)
		{
			ModelType = modelType;
			SensorType = sensorType;
			LabelType = labelType;

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
		}
		/// <summary>
		/// 編集の開始を宣言します。
		/// </summary>
		/// <param name="width">用紙の幅を0.1mm単位で指定。</param>
		/// <param name="length">用紙の長さを0.1mm単位で指定。</param>
		/// <param name="gap">用紙の隙間を0.1mm単位で指定。</param>
		public void Start(int width, int length, int gap)
		{
			byte[] data;

			Start();

			data = Encoding.Default.GetBytes(String.Format("{{D{0:0000},{1:0000},{2:0000}|}}{{C|}}", length + gap, width, length));

			append(data);
		}

		#endregion

		#region 編集終了

		/// <summary>
		/// 編集の終了を宣言します。
		/// </summary>
		/// <param name="direction">印字方向を指定。</param>
		/// <param name="speed">印字スピードを指定。(2,4,6)</param>
		/// <param name="copies">発行枚数を指定。(1～9999)</param>
		/// <param name="cut">カット間隔を指定。(0～100)</param>
		/// <param name="printMode">発行モードを指定。</param>
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
		/// 編集の終了を宣言します。
		/// </summary>
		/// <param name="direction">印字方向を指定。</param>
		/// <param name="speed">印字スピードを指定。(2,4,6)</param>
		/// <param name="copies">発行枚数を指定。(1～9999)</param>
		/// <param name="cut">カット間隔を指定。(0～100)</param>
		public void End(Direction direction, int speed, int copies, int cut)
		{
			End(direction, speed, copies, cut, PrintMode.C);
		}
		/// <summary>
		/// 編集の終了を宣言します。
		/// </summary>
		/// <param name="cut">カットする場合は true。それ以外の場合は false。</param>
		public void End(bool cut)
		{
			End(Direction.Standard, 6, 1, cut ? 1 : 0);
		}
		/// <summary>
		/// 編集の終了を宣言します。
		/// </summary>
		public void End()
		{
			End(Direction.Standard, 6, 1, 0);
		}

		#endregion

		#region 用紙サイズ指定

		/// <summary>
		/// 用紙サイズを指定します。
		/// </summary>
		/// <param name="width">用紙の幅を0.1mm単位で指定。</param>
		/// <param name="length">用紙の長さを0.1mm単位で指定。</param>
		/// <param name="gap">用紙の隙間を0.1mm単位で指定。</param>
		public void LabelSize(int width, int length, int gap)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(String.Format("{{D{0:0000},{1:0000},{2:0000}|}}", length + gap, width, length));

			append(data);
		}

		#endregion

		#region イメージバッファクリア

		/// <summary>
		/// イメージバッファをクリアします。
		/// </summary>
		public void Clear()
		{
			byte[] data;

			data = Encoding.Default.GetBytes("{C|}");

			append(data);
		}

		#endregion

		#region 罫線印字指定

		/// <summary>
		/// 罫線を指定します。
		/// </summary>
		/// <param name="x1">罫線の始点X座標を0.1mm単位で指定。</param>
		/// <param name="y1">罫線の始点Y座標を0.1mm単位で指定。</param>
		/// <param name="x2">罫線の終点X座標を0.1mm単位で指定。</param>
		/// <param name="y2">罫線の終点Y座標を0.1mm単位で指定。</param>
		/// <param name="width">罫線の幅を0.1mm単位で指定。(1～9)</param>
		public void Line(int x1, int y1, int x2, int y2, int width)
		{
			byte[] data;

			width = (width < 1 ? 1 : width);
			width = (width > 9 ? 9 : width);

			data = Encoding.Default.GetBytes(String.Format("{{LC;{0:0000},{1:0000},{2:0000},{3:0000},0,{4}|}}", x1, y1, x2, y2, width));

			append(data);
		}

		#endregion

		#region 枠線印字指定

		/// <summary>
		/// 枠線を指定します。
		/// </summary>
		/// <param name="x1">枠線の始点X座標を0.1mm単位で指定。</param>
		/// <param name="y1">枠線の始点Y座標を0.1mm単位で指定。</param>
		/// <param name="x2">枠線の終点X座標を0.1mm単位で指定。</param>
		/// <param name="y2">枠線の終点Y座標を0.1mm単位で指定。</param>
		/// <param name="width">枠線の幅を0.1mm単位で指定。(1～9)</param>
		/// <param name="round">角丸め半径を0.1mm単位で指定。</param>
		public void Frame(int x1, int y1, int x2, int y2, int width, int round)
		{
			byte[] data;

			width = (width < 1 ? 1 : width);
			width = (width > 9 ? 9 : width);

			data = Encoding.Default.GetBytes(String.Format("{{LC;{0:0000},{1:0000},{2:0000},{3:0000},1,{4},{5:000}|}}", x1, y1, x2, y2, width, round));

			append(data);
		}
		/// <summary>
		/// 枠線を指定します。
		/// </summary>
		/// <param name="x1">枠線の始点X座標を0.1mm単位で指定。</param>
		/// <param name="y1">枠線の始点Y座標を0.1mm単位で指定。</param>
		/// <param name="x2">枠線の終点X座標を0.1mm単位で指定。</param>
		/// <param name="y2">枠線の終点Y座標を0.1mm単位で指定。</param>
		/// <param name="width">枠線の幅を0.1mm単位で指定。(1～9)</param>
		public void Frame(int x1, int y1, int x2, int y2, int width)
		{
			Frame(x1, y1, x2, y2, width, 0);
		}

		#endregion

		#region ビットマップフォント指定

		/// <summary>
		/// 文字（ビットマップフォント）を指定します。
		/// </summary>
		/// <param name="x">文字の基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">文字の基点Y座標を0.1mm単位で指定。</param>
		/// <param name="font">フォントの種類を指定。</param>
		/// <param name="xSize">文字の横倍率を指定。(1～9)</param>
		/// <param name="ySize">文字の縦倍率を指定。(1～9)</param>
		/// <param name="fontPitch">文字間のピッチをドット単位で指定。(-99～99)</param>
		/// <param name="textData">データが格納されている文字列。</param>
		/// <param name="turn">回転方向を指定。</param>
		/// <param name="fontOption">文字飾りを指定。</param>
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
		/// 文字（ビットマップフォント）を指定します。
		/// </summary>
		/// <param name="x">文字の基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">文字の基点Y座標を0.1mm単位で指定。</param>
		/// <param name="font">フォントの種類を指定。</param>
		/// <param name="xSize">文字の横倍率を指定。(1～9)</param>
		/// <param name="ySize">文字の縦倍率を指定。(1～9)</param>
		/// <param name="textData">データが格納されている文字列。</param>
		/// <param name="turn">回転方向を指定。</param>
		public void Text1(int x, int y, BitmapFonts font, int xSize, int ySize, string textData, Turn turn)
		{
			Text1(x, y, font, xSize, ySize, 0, textData, turn, "");
		}
		/// <summary>
		/// 文字（ビットマップフォント）を指定します。
		/// </summary>
		/// <param name="x">文字の基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">文字の基点Y座標を0.1mm単位で指定。</param>
		/// <param name="font">フォントの種類を指定。</param>
		/// <param name="xSize">文字の横倍率を指定。(1～9)</param>
		/// <param name="ySize">文字の縦倍率を指定。(1～9)</param>
		/// <param name="textData">データが格納されている文字列。</param>
		public void Text1(int x, int y, BitmapFonts font, int xSize, int ySize, string textData)
		{
			Text1(x, y, font, xSize, ySize, 0, textData, Turn._0, "");
		}

		#endregion

		#region アウトラインフォント指定

		/// <summary>
		/// 文字（アウトラインフォント）を指定します。
		/// </summary>
		/// <param name="x">文字の基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">文字の基点Y座標を0.1mm単位で指定。</param>
		/// <param name="font">フォントの種類を指定。</param>
		/// <param name="width">文字幅を0.1mm単位で指定。(20～850)</param>
		/// <param name="height">文字高を0.1mm単位で指定。(20～850)</param>
		/// <param name="fontPitch">文字間のピッチをドット単位で指定。(-512～512)</param>
		/// <param name="textData">データが格納されている文字列。</param>
		/// <param name="turn">回転方向を指定。</param>
		/// <param name="fontOption">文字飾りを指定。</param>
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
		/// 文字（アウトラインフォント）を指定します。
		/// </summary>
		/// <param name="x">文字の基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">文字の基点Y座標を0.1mm単位で指定。</param>
		/// <param name="width">文字幅を0.1mm単位で指定。(20～850)</param>
		/// <param name="height">文字高を0.1mm単位で指定。(20～850)</param>
		/// <param name="font">フォントの種類を指定。</param>
		/// <param name="textData">データが格納されている文字列。</param>
		/// <param name="turn">回転方向を指定。</param>
		public void Text2(int x, int y, OutlineFonts font, int width, int height, string textData, Turn turn)
		{
			Text2(x, y, font, width, height, 0, textData, turn, "");
		}
		/// <summary>
		/// 文字（アウトラインフォント）を指定します。
		/// </summary>
		/// <param name="x">文字の基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">文字の基点Y座標を0.1mm単位で指定。</param>
		/// <param name="width">文字幅を0.1mm単位で指定。(20～850)</param>
		/// <param name="height">文字高を0.1mm単位で指定。(20～850)</param>
		/// <param name="font">フォントの種類を指定。</param>
		/// <param name="textData">データが格納されている文字列。</param>
		public void Text2(int x, int y, OutlineFonts font, int width, int height, string textData)
		{
			Text2(x, y, font, width, height, 0, textData, Turn._0, "");
		}

		#endregion

		#region バーコード指定（JAN,UPC,CODE93,CODE128）

		/// <summary>
		/// バーコード（JAN,UPC,CODE93,CODE128）を指定します。
		/// </summary>
		/// <param name="x">バーコードの基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">バーコードの基点Y座標を0.1mm単位で指定。</param>
		/// <param name="barcodeType">バーコードの種類を指定。</param>
		/// <param name="checkdigit">チェックディジットを付加する場合は true。それ以外の場合は false。</param>
		/// <param name="width">モジュール幅をドット単位で指定。(1～15)</param>
		/// <param name="height">バーコード高を0.1mm単位で指定。(0～1000)</param>
		/// <param name="barcodeData">バーコードデータが格納されている文字列。</param>
		/// <param name="subscript">添え字を付加する場合は true。それ以外の場合は false。</param>
		/// <param name="turn">回転方向を指定。</param>
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
		/// バーコード（JAN,UPC,CODE93,CODE128）を指定します。
		/// </summary>
		/// <param name="x">バーコードの基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">バーコードの基点Y座標を0.1mm単位で指定。</param>
		/// <param name="barcodeType">バーコードの種類を指定。</param>
		/// <param name="width">モジュール幅をドット単位で指定。(1～15)</param>
		/// <param name="height">バーコード高を0.1mm単位で指定。(0～1000)</param>
		/// <param name="barcodeData">バーコードデータが格納されている文字列。</param>
		/// <param name="subscript">添え字を付加する場合は true。それ以外の場合は false。</param>
		public void Barcode1(int x, int y, BarcodeType1 barcodeType, int width, int height, string barcodeData, bool subscript)
		{
			Barcode1(x, y, barcodeType, false, width, height, barcodeData, subscript, Turn._0);
		}
		/// <summary>
		/// バーコード（JAN,UPC,CODE93,CODE128）を指定します。
		/// </summary>
		/// <param name="x">バーコードの基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">バーコードの基点Y座標を0.1mm単位で指定。</param>
		/// <param name="barcodeType">バーコードの種類を指定。</param>
		/// <param name="width">モジュール幅をドット単位で指定。(1～15)</param>
		/// <param name="height">バーコード高を0.1mm単位で指定。(0～1000)</param>
		/// <param name="barcodeData">バーコードデータが格納されている文字列。</param>
		/// <param name="turn">回転方向を指定。</param>
		public void Barcode1(int x, int y, BarcodeType1 barcodeType, int width, int height, string barcodeData, Turn turn)
		{
			Barcode1(x, y, barcodeType, false, width, height, barcodeData, false, turn);
		}
		/// <summary>
		/// バーコード（JAN,UPC,CODE93,CODE128）を指定します。
		/// </summary>
		/// <param name="x">バーコードの基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">バーコードの基点Y座標を0.1mm単位で指定。</param>
		/// <param name="barcodeType">バーコードの種類を指定。</param>
		/// <param name="width">モジュール幅をドット単位で指定。(1～15)</param>
		/// <param name="height">バーコード高を0.1mm単位で指定。(0～1000)</param>
		/// <param name="barcodeData">バーコードデータが格納されている文字列。</param>
		public void Barcode1(int x, int y, BarcodeType1 barcodeType, int width, int height, string barcodeData)
		{
			Barcode1(x, y, barcodeType, false, width, height, barcodeData, false, Turn._0);
		}

		#endregion

		#region バーコード指定（ITF,CODE39,NW7）

		/// <summary>
		/// バーコード（ITF,CODE39,NW7）を指定します。
		/// </summary>
		/// <param name="x">バーコードの基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">バーコードの基点Y座標を0.1mm単位で指定。</param>
		/// <param name="barcodeType">バーコードの種類を指定。</param>
		/// <param name="checkdigit">チェックディジットを付加する場合は true。それ以外の場合は false。</param>
		/// <param name="narrowBar">細バー幅をドット単位で指定。(1～99)</param>
		/// <param name="wideBar">太バー幅をドット単位で指定。(1～99)</param>
		/// <param name="height">バーコード高を0.1mm単位で指定。(0～1000)</param>
		/// <param name="barcodeData">バーコードデータが格納されている文字列。</param>
		/// <param name="subscript">添え字を付加する場合は true。それ以外の場合は false。</param>
		/// <param name="turn">回転方向を指定。</param>
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
		/// バーコード（ITF,CODE39,NW7）を指定します。
		/// </summary>
		/// <param name="x">バーコードの基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">バーコードの基点Y座標を0.1mm単位で指定。</param>
		/// <param name="barcodeType">バーコードの種類を指定。</param>
		/// <param name="narrowBar">細バー幅をドット単位で指定。(1～99)</param>
		/// <param name="wideBar">太バー幅をドット単位で指定。(1～99)</param>
		/// <param name="height">バーコード高を0.1mm単位で指定。(0～1000)</param>
		/// <param name="barcodeData">バーコードデータが格納されている文字列。</param>
		/// <param name="subscript">添え字を付加する場合は true。それ以外の場合は false。</param>
		public void Barcode2(int x, int y, BarcodeType2 barcodeType, int narrowBar, int wideBar, int height, string barcodeData, bool subscript)
		{
			Barcode2(x, y, barcodeType, false, narrowBar, wideBar, height, barcodeData, subscript, Turn._0);
		}
		/// <summary>
		/// バーコード（ITF,CODE39,NW7）を指定します。
		/// </summary>
		/// <param name="x">バーコードの基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">バーコードの基点Y座標を0.1mm単位で指定。</param>
		/// <param name="barcodeType">バーコードの種類を指定。</param>
		/// <param name="narrowBar">細バー幅をドット単位で指定。(1～99)</param>
		/// <param name="wideBar">太バー幅をドット単位で指定。(1～99)</param>
		/// <param name="height">バーコード高を0.1mm単位で指定。(0～1000)</param>
		/// <param name="barcodeData">バーコードデータが格納されている文字列。</param>
		/// <param name="turn">回転方向を指定。</param>
		public void Barcode2(int x, int y, BarcodeType2 barcodeType, int narrowBar, int wideBar, int height, string barcodeData, Turn turn)
		{
			Barcode2(x, y, barcodeType, false, narrowBar, wideBar, height, barcodeData, false, turn);
		}
		/// <summary>
		/// バーコード（ITF,CODE39,NW7）を指定します。
		/// </summary>
		/// <param name="x">バーコードの基点X座標を0.1mm単位で指定。</param>
		/// <param name="y">バーコードの基点Y座標を0.1mm単位で指定。</param>
		/// <param name="barcodeType">バーコードの種類を指定。</param>
		/// <param name="narrowBar">細バー幅をドット単位で指定。(1～99)</param>
		/// <param name="wideBar">太バー幅をドット単位で指定。(1～99)</param>
		/// <param name="height">バーコード高を0.1mm単位で指定。(0～1000)</param>
		/// <param name="barcodeData">バーコードデータが格納されている文字列。</param>
		public void Barcode2(int x, int y, BarcodeType2 barcodeType, int narrowBar, int wideBar, int height, string barcodeData)
		{
			Barcode2(x, y, barcodeType, false, narrowBar, wideBar, height, barcodeData, false, Turn._0);
		}

		#endregion

		#region コマンド指定

		/// <summary>
		///コマンドを直接指定します。
		/// </summary>
		/// <param name="command">コマンドが格納されている文字列。</param>
		public void Format(string command)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(command);

			append(data);
		}

		#endregion

		#region 白黒反転

		/// <summary>
		/// 白黒反転を指定します。
		/// </summary>
		/// <param name="x1">白黒反転の始点X座標を0.1mm単位で指定。</param>
		/// <param name="y1">白黒反転の始点Y座標を0.1mm単位で指定。</param>
		/// <param name="x2">白黒反転の終点X座標を0.1mm単位で指定。</param>
		/// <param name="y2">白黒反転の終点Y座標を0.1mm単位で指定。</param>
		public void Reverse(int x1, int y1, int x2, int y2)
		{
			byte[] data;

			data = Encoding.Default.GetBytes(String.Format("{{XR;{0:0000},{1:0000},{2:0000},{3:0000},B|}}", x1, y1, x2, y2));

			append(data);
		}

		#endregion

		#region 排出

		/// <summary>
		/// 排出します。
		/// </summary>
		public void Eject()
		{
			byte[] data;

			data = Encoding.Default.GetBytes("{IB|}");

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
