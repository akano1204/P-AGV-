using System;
using System.Collections.Generic;
using System.Text;
using System.Net;

using BelicsClass.Common;
using BelicsClass.Network;
using BelicsClass.Rs232c;
using BelicsClass.ProcessManage;

namespace BelicsClass.PLC
{
	/// <summary>
	/// MCプロトコルを使用してPLCと通信するクラスです。
	/// </summary>
	public class BL_MCProtocol : BL_ThreadController_Base
	{
		/// <summary>ランダム読出し可能(複数ブロック読出し可能)</summary>
		public bool SupportRandomRead = false;
		
		/// <summary>ランダム書込み可能</summary>
		public bool SupportRandomWrite = false;
		
		/// <summary>ASCIIモード</summary>
		public bool isASCII = false;

		/// <summary>
		/// 一括書込み・一括読込みのみ
		/// </summary>
		public bool PackageWriteOnly
		{
			get
			{
				return !SupportRandomWrite;
			}
			set
			{
				if (value)
				{
					SupportRandomRead = false;
					SupportRandomWrite = false;
				}
				else
				{
					SupportRandomRead = true;
					SupportRandomWrite = true;
				}
			}
		}

		/// <summary>
		/// 接続種別
		/// </summary>
		public enum DeviceTypes
		{
			/// <summary>未定</summary>
			Unknown = 0,
			/// <summary>TCP</summary>
			TCP = 1,
			/// <summary>UDP</summary>
			UDP = 2,
			/// <summary>RS-232C(形式3)</summary>
			RS232C = 3,
        }

		/// <summary></summary>
		public DeviceTypes DeviceType = DeviceTypes.Unknown;

		/// <summary>
		/// コマンドタイプ
		/// </summary>
		public enum CommandTypes
		{
			/// <summary>ACPU共通コマンド</summary>
			MELSEC_A_CPU,

			/// <summary>AnA/AnAUCPU共通コマンド</summary>
			MELSEC_AnA_CPU,
		}

		/// <summary></summary>
		public CommandTypes CommandType = CommandTypes.MELSEC_A_CPU;

		/// <summary>
		/// KV-5000で扱うことの出来るアドレスへ丸める
		/// </summary>
		public bool KV5000 = false;

		/// <summary>
		/// アクセス対象のDM情報を管理します。
		/// </summary>
		[Serializable]
		public class DeviceInfo
		{
			#region エンディアン種別を定義します。

			/// <summary>
			/// エンディアン種別を定義します。
			/// </summary>
			public enum Endian
			{
				/// <summary>リトルエンディアン</summary>
				LITTLE,
				/// <summary>ビッグエンディアン</summary>
				BIG,
			}

			#endregion

			#region デバイスのデータ種別を定義します。

			/// <summary>
			/// デバイスの種別を定義します。
			/// </summary>
			public enum DeviceKind
			{
				/// <summary>不明</summary>
				UNKNOWN,
				/// <summary>ビット</summary>
				BIT,
				/// <summary>ワード</summary>
				WORD,
			}

			#endregion

			#region アドレス指定方法の種別を定義します。

			/// <summary>
			/// アドレス指定方法の種別を定義します。
			/// </summary>
			public enum DeviceAddressType
			{
				/// <summary>不明</summary>
				UNKNOWN,
				/// <summary>10進表記</summary>
				DEC,
				/// <summary>16進表記</summary>
				HEX,
			}

			#endregion

			#region アドレスの指定方法を定義します。

			/// <summary>
			/// アドレスの指定方法を定義します。
			/// </summary>
			public enum DeviceAddressSequence
			{
				///<summary>通常の連番指定</summary>
				NORMAL,
				///<summary>１６アドレス毎に桁上がり</summary>
				FLOWBY16,
			}

			#endregion

			#region デバイス種別を定義します。

			/// <summary>
			/// デバイス種別を定義します。
			/// </summary>
			public enum DeviceCode : byte
			{
				/// <summary>不明</summary>
				UNKNOWN = 0x00,
				/// <summary></summary>
				SM = 0x91,
				/// <summary></summary>
				SD = 0xA9,
				/// <summary></summary>
				X = 0x9C,
				/// <summary></summary>
				Y = 0x9D,
				/// <summary></summary>
				M = 0x90,
				/// <summary></summary>
				L = 0x92,
				/// <summary></summary>
				F = 0x93,
				/// <summary></summary>
				V = 0x94,
				/// <summary></summary>
				B = 0xA0,
				/// <summary></summary>
				D = 0xA8,
				/// <summary></summary>
				W = 0xB4,
				/// <summary></summary>
				TS = 0xC1,
				/// <summary></summary>
				TC = 0xC0,
				/// <summary></summary>
				TN = 0xC2,
				/// <summary></summary>
				SS = 0xC7,
				/// <summary></summary>
				SC = 0xC6,
				/// <summary></summary>
				SN = 0xC8,
				/// <summary></summary>
				CS = 0xC4,
				/// <summary></summary>
				CC = 0xC3,
				/// <summary></summary>
				CN = 0xC5,
				/// <summary></summary>
				SB = 0xA1,
				/// <summary></summary>
				SW = 0xB5,
				/// <summary></summary>
				S = 0x98,
				/// <summary></summary>
				DX = 0xA2,
				/// <summary></summary>
				DY = 0xA3,
				/// <summary></summary>
				Z = 0xCC,
				/// <summary></summary>
				R = 0xAF,
				/// <summary></summary>
				ZR = 0xB0,
			}

			#endregion

			#region 値の表現方法の種別を定義します。

			/// <summary>
			/// 値の表現方法の種別を定義します。
			/// </summary>
			public enum ValueType
			{
				/// <summary>UNKNOWN</summary>
				UNKNOWN,
				/// <summary>BIT</summary>
				BIT,
				/// <summary>BINARY 16BIT</summary>
				BIN16,
				/// <summary>UNSIGNED BINARY 16BIT</summary>
				UBIN16,
				/// <summary>BINARY 32BIT</summary>
				BIN32,
				/// <summary>UNSIGNED BINARY 32BIT</summary>
				UBIN32,
				/// <summary>BINARY 64BIT</summary>
				BIN64,
				/// <summary>UNSIGNED BINARY 64BIT</summary>
				UBIN64,
				/// <summary>FLOAT</summary>
				FLOAT,
				/// <summary>DOUBLE</summary>
				DOUBLE,
				/// <summary>STRING</summary>
				STRING,
			}

			#endregion

			#region デバイスのサイズ単位を取得します。

			/// <summary>
			/// デバイスのサイズ単位を取得します。
			/// </summary>
			public int UnitSize
			{
				get
				{
					int unitsize = 1;
					switch (deviceKind)
					{
						case DeviceKind.WORD:
							switch (valueType)
							{
								case ValueType.BIT: unitsize = 2; break;
								case ValueType.BIN16: unitsize = 2; break;
								case ValueType.UBIN16: unitsize = 2; break;
								case ValueType.BIN32: unitsize = 4; break;
								case ValueType.UBIN32: unitsize = 4; break;
								case ValueType.BIN64: unitsize = 8; break;
								case ValueType.UBIN64: unitsize = 8; break;
								case ValueType.FLOAT: unitsize = 4; break;
								case ValueType.DOUBLE: unitsize = 8; break;
							}
							break;
					}
					return unitsize;
				}
			}

			#endregion

			/// <summary>
			/// 対象のデバイスコードを保持します。
			/// </summary>
			public DeviceCode deviceCode = 0x00;

			/// <summary>
			/// 対象のデバイスアドレスを保持します。
			/// </summary>
			public int deviceAddress = 0;

			/// <summary>
			/// デバイスサイズ(ワード数)を取得します。
			/// </summary>
			public int DeviceSize
			{
				get
				{
					if (valueType == ValueType.BIT) return dataLength;
					return (dataLength * UnitSize + 1) / 2;
				}
			}

			/// <summary>
			/// バイトサイズを取得します。
			/// </summary>
			public int ByteSize
			{
				get
				{
					return dataLength * UnitSize;
				}
			}

			/// <summary>
			/// 対象のデータ数を保持します。
			/// </summary>
			public int dataLength = 0;

			/// <summary>
			/// 対象のデバイスビット位置を保持します。
			/// </summary>
			public int deviceBitPosition = 0;

			/// <summary>
			/// 反転属性を保持します。
			/// </summary>
			public bool negative = false;

			#region デバイス種別を取得します。

			/// <summary>
			/// デバイス種別を取得します。
			/// </summary>
			public DeviceKind deviceKind
			{
				get
				{
					switch (deviceCode)
					{
						case DeviceCode.SM: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.SD: return DeviceInfo.DeviceKind.WORD;
						case DeviceCode.X: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.Y: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.M: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.L: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.F: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.V: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.B: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.D: return DeviceInfo.DeviceKind.WORD;
						case DeviceCode.W: return DeviceInfo.DeviceKind.WORD;
						case DeviceCode.TS: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.TC: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.TN: return DeviceInfo.DeviceKind.WORD;
						case DeviceCode.SS: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.SC: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.SN: return DeviceInfo.DeviceKind.WORD;
						case DeviceCode.CS: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.CC: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.CN: return DeviceInfo.DeviceKind.WORD;
						case DeviceCode.SB: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.SW: return DeviceInfo.DeviceKind.WORD;
						case DeviceCode.S: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.DX: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.DY: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.Z: return DeviceInfo.DeviceKind.WORD;
						case DeviceCode.R: return DeviceInfo.DeviceKind.WORD;
						case DeviceCode.ZR: return DeviceInfo.DeviceKind.WORD;
					}
					return DeviceKind.UNKNOWN;
				}
			}

			#endregion

			#region アドレス指定方法の種別を取得します。

			/// <summary>
			/// アドレス指定方法の種別を取得します。
			/// </summary>
			public DeviceAddressType deviceAddresstype
			{
				get
				{
					switch (deviceCode)
					{
						case DeviceCode.SM: return DeviceAddressType.DEC;
						case DeviceCode.SD: return DeviceAddressType.DEC;
						case DeviceCode.X: return DeviceAddressType.HEX;
						case DeviceCode.Y: return DeviceAddressType.HEX;
						case DeviceCode.M: return DeviceAddressType.DEC;
						case DeviceCode.L: return DeviceAddressType.DEC;
						case DeviceCode.F: return DeviceAddressType.DEC;
						case DeviceCode.V: return DeviceAddressType.DEC;
						case DeviceCode.B: return DeviceAddressType.HEX;
						case DeviceCode.D: return DeviceAddressType.DEC;
						case DeviceCode.W: return DeviceAddressType.HEX;
						case DeviceCode.TS: return DeviceAddressType.DEC;
						case DeviceCode.TC: return DeviceAddressType.DEC;
						case DeviceCode.TN: return DeviceAddressType.DEC;
						case DeviceCode.SS: return DeviceAddressType.DEC;
						case DeviceCode.SC: return DeviceAddressType.DEC;
						case DeviceCode.SN: return DeviceAddressType.DEC;
						case DeviceCode.CS: return DeviceAddressType.DEC;
						case DeviceCode.CC: return DeviceAddressType.DEC;
						case DeviceCode.CN: return DeviceAddressType.DEC;
						case DeviceCode.SB: return DeviceAddressType.HEX;
						case DeviceCode.SW: return DeviceAddressType.HEX;
						case DeviceCode.S: return DeviceAddressType.DEC;
						case DeviceCode.DX: return DeviceAddressType.HEX;
						case DeviceCode.DY: return DeviceAddressType.HEX;
						case DeviceCode.Z: return DeviceAddressType.DEC;
						case DeviceCode.R: return DeviceAddressType.DEC;
						case DeviceCode.ZR: return DeviceAddressType.HEX;
					}
					return DeviceAddressType.UNKNOWN;
				}
			}

			#endregion

			#region アドレスの指定方法を取得します。

			/// <summary>
			/// アドレスの指定方法を取得します。
			/// </summary>
			public DeviceAddressSequence deviceAddressSequence
			{
				get
				{
					switch (deviceCode)
					{
						case DeviceCode.SM: return DeviceAddressSequence.NORMAL;
						case DeviceCode.SD: return DeviceAddressSequence.NORMAL;
						case DeviceCode.X: return DeviceAddressSequence.NORMAL;
						case DeviceCode.Y: return DeviceAddressSequence.NORMAL;
						case DeviceCode.M: return DeviceAddressSequence.FLOWBY16;
						case DeviceCode.L: return DeviceAddressSequence.NORMAL;
						case DeviceCode.F: return DeviceAddressSequence.NORMAL;
						case DeviceCode.V: return DeviceAddressSequence.NORMAL;
						case DeviceCode.B: return DeviceAddressSequence.NORMAL;
						case DeviceCode.D: return DeviceAddressSequence.NORMAL;
						case DeviceCode.W: return DeviceAddressSequence.NORMAL;
						case DeviceCode.TS: return DeviceAddressSequence.NORMAL;
						case DeviceCode.TC: return DeviceAddressSequence.NORMAL;
						case DeviceCode.TN: return DeviceAddressSequence.NORMAL;
						case DeviceCode.SS: return DeviceAddressSequence.NORMAL;
						case DeviceCode.SC: return DeviceAddressSequence.NORMAL;
						case DeviceCode.SN: return DeviceAddressSequence.NORMAL;
						case DeviceCode.CS: return DeviceAddressSequence.NORMAL;
						case DeviceCode.CC: return DeviceAddressSequence.NORMAL;
						case DeviceCode.CN: return DeviceAddressSequence.NORMAL;
						case DeviceCode.SB: return DeviceAddressSequence.NORMAL;
						case DeviceCode.SW: return DeviceAddressSequence.NORMAL;
						case DeviceCode.S: return DeviceAddressSequence.NORMAL;
						case DeviceCode.DX: return DeviceAddressSequence.NORMAL;
						case DeviceCode.DY: return DeviceAddressSequence.NORMAL;
						case DeviceCode.Z: return DeviceAddressSequence.NORMAL;
						case DeviceCode.R: return DeviceAddressSequence.NORMAL;
						case DeviceCode.ZR: return DeviceAddressSequence.NORMAL;
					}
					return DeviceAddressSequence.NORMAL;
				}
			}

			#endregion

			/// <summary>
			/// 値の表現方法の種別を保持します。
			/// </summary>
			public ValueType valueType = ValueType.BIN16;

			/// <summary>
			/// エンディアン種別を保持します。
			/// </summary>
			public Endian endian = Endian.LITTLE;

			/// <summary>
			/// 読み出しデータ／書き込みデータを保持します。
			/// </summary>
			public byte[] data = null;

			/// <summary>
			/// 直前の読み出しデータ／書き込みデータを保持します。
			/// </summary>
			public byte[] data_pre = null;

			/// <summary>
			/// 読み込み管理フラグ
			/// </summary>
			[NonSerialized]
			public bool readed = false;

			/// <summary>
			/// 書き込み管理フラグ
			/// </summary>
			[NonSerialized]
			public bool writing = false;

			/// <summary>
			/// 書き込み管理フラグ
			/// </summary>
			[NonSerialized]
			public bool writed = false;

			/// <summary>
			/// イベント不要
			/// </summary>
			public bool noevent = false;

			/// <summary>
			/// 変化フラグ
			/// </summary>
			[NonSerialized]
			public bool changed = false;

			/// <summary>
			/// 自インスタンスのクローンを生成します。
			/// </summary>
			/// <returns></returns>
			public DeviceInfo Clone()
			{
				DeviceInfo dev = new DeviceInfo(deviceCode, deviceAddress, dataLength, valueType);
				dev.negative = negative;
				dev.endian = endian;
				dev.deviceBitPosition = deviceBitPosition;

				dev.data = new byte[data.Length];
				Array.Copy(data, dev.data, data.Length);
				return dev;
			}

			/// <summary>
			/// アドレス文字列を取得します。
			/// </summary>
			public string AddressString
			{
				get
				{
					if (deviceAddresstype == DeviceAddressType.HEX)
					{
						return deviceCode.ToString() + deviceAddress.ToString("X");
					}
					else
					{
						return deviceCode.ToString() + deviceAddress.ToString();
					}
				}
			}

			#region コンストラクタ

			/// <summary>
			/// デフォルトコンストラクタ
			/// </summary>
			public DeviceInfo()
			{
			}

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="deviceCode"></param>
			/// <param name="deviceAddress"></param>
			/// <param name="dataLength"></param>
			/// <param name="valueType"></param>
			public DeviceInfo(DeviceCode deviceCode, int deviceAddress, int dataLength, ValueType valueType)
			{
				this.deviceCode = deviceCode;
				this.deviceAddress = deviceAddress;
				this.valueType = valueType;

				if (dataLength == 0) throw new Exception("指定が不正です。");

				this.dataLength = dataLength;
				this.data = new byte[this.ByteSize];
			}

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="deviceCode"></param>
			/// <param name="deviceAddress"></param>
			/// <param name="dataLength"></param>
			/// <param name="valueType"></param>
			/// <param name="endian"></param>
			public DeviceInfo(DeviceCode deviceCode, int deviceAddress, int dataLength, ValueType valueType, Endian endian)
				: this(deviceCode, deviceAddress, dataLength, valueType)
			{
				this.endian = endian;
			}

			#endregion

			/// <summary>
			/// デバイスコード名称からデバイス種別を取得します。
			/// </summary>
			/// <param name="deviceCode"></param>
			/// <returns></returns>
			public static DeviceCode GetDeviceCode(string deviceCode)
			{
				foreach (DeviceCode v in Enum.GetValues(typeof(DeviceCode)))
				{
					if (Enum.GetName(typeof(DeviceCode), v) == deviceCode)
					{
						return (DeviceCode)v;
					}
				}
				return DeviceCode.UNKNOWN;
			}

			/// <summary>
			/// データ種別名称からデータ種別を取得します。
			/// </summary>
			/// <param name="valueType"></param>
			/// <returns></returns>
			public static ValueType GetValueType(string valueType)
			{
				foreach (ValueType v in Enum.GetValues(typeof(ValueType)))
				{
					if (Enum.GetName(typeof(ValueType), v) == valueType)
					{
						return (ValueType)v;
					}
				}
				return ValueType.UNKNOWN;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="device"></param>
			/// <returns></returns>
			public bool Equal(DeviceInfo device)
			{
				if (deviceCode == device.deviceCode && deviceAddress == device.deviceAddress && dataLength == device.dataLength)
				{
					return true;
				}
				return false;
			}

			#region 値を取得します

			/// <summary>
			/// 値を取得します
			/// </summary>
			/// <returns></returns>
			public object Get()
			{
				return Get(0);
			}

            /// <summary>
            /// 値を取得します
            /// </summary>
            /// <param name="position">オフセット位置</param>
            /// <returns></returns>
            public object Get(int position)
            {
                return Get(position, valueType);
            }

            /// <summary>
            /// 値を取得します
            /// </summary>
            /// <param name="position">オフセット位置</param>
            /// <param name="vt">値種別</param>
            /// <returns></returns>
            public object Get(int position, ValueType vt)
			{
				if (endian == Endian.LITTLE)
				{
					switch (vt)
					{
						case ValueType.BIN16: return BitConverter.ToInt16(data, position * sizeof(Int16));
						case ValueType.UBIN16: return BitConverter.ToUInt16(data, position * sizeof(UInt16));
						case ValueType.BIN32: return BitConverter.ToInt32(data, position * sizeof(Int32));
						case ValueType.UBIN32: return BitConverter.ToUInt32(data, position * sizeof(UInt32));
						case ValueType.BIN64: return BitConverter.ToInt64(data, position * sizeof(Int64));
						case ValueType.UBIN64: return BitConverter.ToUInt64(data, position * sizeof(UInt64));
						case ValueType.FLOAT: return BitConverter.ToSingle(data, position * sizeof(Single));
						case ValueType.DOUBLE: return BitConverter.ToDouble(data, position * sizeof(Double));
						case ValueType.STRING: return Encoding.Default.GetString(data).Substring(position).TrimEnd('\0').PadRight(dataLength).Substring(0, dataLength);
						case ValueType.BIT: return data[position] != 0 ? true : false;
					}
				}
				else
				{
					byte[] bigdata = new byte[data.Length];
					for (int i = 0; i < data.Length; i += 2)
					{
						if (i + 1 < data.Length) bigdata[i] = data[i + 1];
						else bigdata[i] = 0;
						if (i + 1 < bigdata.Length) bigdata[i + 1] = data[i];
					}

					switch (valueType)
					{
						case ValueType.BIN16: return BitConverter.ToInt16(bigdata, position * sizeof(Int16));
						case ValueType.UBIN16: return BitConverter.ToUInt16(bigdata, position * sizeof(UInt16));
						case ValueType.BIN32: return BitConverter.ToInt32(bigdata, position * sizeof(Int32));
						case ValueType.UBIN32: return BitConverter.ToUInt32(bigdata, position * sizeof(UInt32));
						case ValueType.BIN64: return BitConverter.ToInt64(bigdata, position * sizeof(Int64));
						case ValueType.UBIN64: return BitConverter.ToUInt64(bigdata, position * sizeof(UInt64));
						case ValueType.FLOAT: return BitConverter.ToSingle(bigdata, position * sizeof(Single));
						case ValueType.DOUBLE: return BitConverter.ToDouble(bigdata, position * sizeof(Double));
						case ValueType.STRING: return Encoding.Default.GetString(bigdata).Substring(position).TrimEnd('\0').PadRight(dataLength).Substring(0, dataLength);
						case ValueType.BIT: return bigdata[position] != 0 ? true : false;
					}
				}
				return null;
			}

			/// <summary>
			/// 
			/// </summary>
			/// <typeparam name="T"></typeparam>
			/// <returns></returns>
			public T Get<T>()
			{
				if (deviceKind == DeviceKind.BIT) return Get<T>(0);
				if (deviceKind == DeviceKind.WORD) return Get<T>(0);
				return default(T);
			}

            /// <summary>
            /// ビット位置を指定して状態を取得します。
            /// </summary>
            /// <param name="position">オフセット位置</param>
            /// <returns></returns>
            public T Get<T>(int position)
            {
                return Get<T>(position, valueType);
            }

            /// <summary>
            /// ビット位置を指定して状態を取得します。
            /// </summary>
            /// <param name="position">オフセット位置</param>
            /// <param name="vt">値種別</param>
            /// <returns></returns>
            public T Get<T>(int position, ValueType vt)
			{
				T ret = default(T);

                if (endian == Endian.LITTLE)
				{
					switch (vt)
					{
						case ValueType.BIN16: return (T)((object)BitConverter.ToInt16(data, position * sizeof(Int16)));
						case ValueType.UBIN16: return (T)((object)BitConverter.ToUInt16(data, position * sizeof(UInt16)));
						case ValueType.BIN32: return (T)((object)BitConverter.ToInt32(data, position * sizeof(Int32)));
						case ValueType.UBIN32: return (T)((object)BitConverter.ToUInt32(data, position * sizeof(UInt32)));
						case ValueType.BIN64: return (T)((object)BitConverter.ToInt64(data, position * sizeof(Int64)));
						case ValueType.UBIN64: return (T)((object)BitConverter.ToUInt64(data, position * sizeof(UInt64)));
						case ValueType.FLOAT: return (T)((object)BitConverter.ToSingle(data, position * sizeof(Single)));
						case ValueType.DOUBLE: return (T)((object)BitConverter.ToDouble(data, position * sizeof(Double)));
						case ValueType.STRING: return (T)((object)Encoding.Default.GetString(data).Substring(position).TrimEnd('\0').PadRight(dataLength).Substring(0, dataLength));
						case ValueType.BIT: return (T)((object)(data[position] != 0 ? true : false));
					}
				}
				else
				{
					byte[] bigdata = new byte[data.Length];
					for (int i = 0; i < data.Length; i += 2)
					{
						if (i + 1 < data.Length) bigdata[i] = data[i + 1];
						else bigdata[i] = 0;
						if (i + 1 < bigdata.Length) bigdata[i + 1] = data[i];
					}

					switch (valueType)
					{
						case ValueType.BIN16: return (T)((object)BitConverter.ToInt16(bigdata, position * sizeof(Int16)));
						case ValueType.UBIN16: return (T)((object)BitConverter.ToUInt16(bigdata, position * sizeof(UInt16)));
						case ValueType.BIN32: return (T)((object)BitConverter.ToInt32(bigdata, position * sizeof(Int32)));
						case ValueType.UBIN32: return (T)((object)BitConverter.ToUInt32(bigdata, position * sizeof(UInt32)));
						case ValueType.BIN64: return (T)((object)BitConverter.ToInt64(bigdata, position * sizeof(Int64)));
						case ValueType.UBIN64: return (T)((object)BitConverter.ToUInt64(bigdata, position * sizeof(UInt64)));
						case ValueType.FLOAT: return (T)((object)BitConverter.ToSingle(bigdata, position * sizeof(Single)));
						case ValueType.DOUBLE: return (T)((object)BitConverter.ToDouble(bigdata, position * sizeof(Double)));
						case ValueType.STRING: return (T)((object)Encoding.Default.GetString(bigdata).Substring(position).TrimEnd('\0').PadRight(dataLength).Substring(0, dataLength));
						case ValueType.BIT: return (T)((object)(bigdata[position] != 0 ? true : false));
					}
				}
				return ret;
			}

			#endregion

			#region 値を設定します。

			/// <summary>
			/// 
			/// </summary>
			/// <param name="val"></param>
			public void Set(Int16 val) { Set(val, 0); }

			/// <summary>
			/// 値を設定します。
			/// </summary>
			/// <param name="val"></param>
			/// <param name="position"></param>
			public void Set(Int16 val, int position)
			{
				if ((position + 1) * sizeof(Int16) <= data.Length)
				{
					byte[] temp = BitConverter.GetBytes(val);

					if (endian == Endian.LITTLE)
					{
						Array.Copy(temp, 0, data, position * sizeof(Int16), temp.Length);
					}
					else
					{
						byte[] bigdata = new byte[temp.Length];
						for (int i = 0; i < data.Length; i += 2)
						{
							if (i + 1 < data.Length) bigdata[i] = data[i + 1];
							else bigdata[i] = 0;
							if (i + 1 < bigdata.Length) bigdata[i + 1] = data[i];
						}
						Array.Copy(bigdata, 0, data, position * sizeof(Int16), temp.Length);
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="val"></param>
			public void Set(UInt16 val) { Set(val, 0); }

			/// <summary>
			/// 値を設定します。
			/// </summary>
			/// <param name="val"></param>
			/// <param name="position"></param>
			public void Set(UInt16 val, int position)
			{
				if ((position + 1) * sizeof(UInt16) <= data.Length)
				{
					byte[] temp = BitConverter.GetBytes(val);

					if (endian == Endian.LITTLE)
					{
						Array.Copy(temp, 0, data, position * sizeof(UInt16), temp.Length);
					}
					else
					{
						byte[] bigdata = new byte[temp.Length];
						for (int i = 0; i < data.Length; i += 2)
						{
							if (i + 1 < data.Length) bigdata[i] = data[i + 1];
							else bigdata[i] = 0;
							if (i + 1 < bigdata.Length) bigdata[i + 1] = data[i];
						}
						Array.Copy(bigdata, 0, data, position * sizeof(UInt16), temp.Length);
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="val"></param>
			public void Set(Int32 val) { Set(val, 0); }

			/// <summary>
			/// 値を設定します。
			/// </summary>
			/// <param name="val"></param>
			/// <param name="position"></param>
			public void Set(Int32 val, int position)
			{
				if ((position + 1) * sizeof(Int32) <= data.Length)
				{
					byte[] temp = BitConverter.GetBytes(val);

					if (endian == Endian.LITTLE)
					{
						Array.Copy(temp, 0, data, position * sizeof(Int32), temp.Length);
					}
					else
					{
						byte[] bigdata = new byte[temp.Length];
						for (int i = 0; i < data.Length; i += 2)
						{
							if (i + 1 < data.Length) bigdata[i] = data[i + 1];
							else bigdata[i] = 0;
							if (i + 1 < bigdata.Length) bigdata[i + 1] = data[i];
						}
						Array.Copy(bigdata, 0, data, position * sizeof(Int32), temp.Length);
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="val"></param>
			public void Set(UInt32 val) { Set(val, 0); }

			/// <summary>
			/// 値を設定します。
			/// </summary>
			/// <param name="val"></param>
			/// <param name="position"></param>
			public void Set(UInt32 val, int position)
			{
				if ((position + 1) * sizeof(UInt32) <= data.Length)
				{
					byte[] temp = BitConverter.GetBytes(val);

					if (endian == Endian.LITTLE)
					{
						Array.Copy(temp, 0, data, position * sizeof(UInt32), temp.Length);
					}
					else
					{
						byte[] bigdata = new byte[temp.Length];
						for (int i = 0; i < data.Length; i += 2)
						{
							if (i + 1 < data.Length) bigdata[i] = data[i + 1];
							else bigdata[i] = 0;
							if (i + 1 < bigdata.Length) bigdata[i + 1] = data[i];
						}
						Array.Copy(bigdata, 0, data, position * sizeof(UInt32), temp.Length);
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="val"></param>
			public void Set(Int64 val) { Set(val, 0); }

			/// <summary>
			/// 値を設定します。
			/// </summary>
			/// <param name="val"></param>
			/// <param name="position"></param>
			public void Set(Int64 val, int position)
			{
				if ((position + 1) * sizeof(Int64) <= data.Length)
				{
					byte[] temp = BitConverter.GetBytes(val);

					if (endian == Endian.LITTLE)
					{
						Array.Copy(temp, 0, data, position * sizeof(Int64), temp.Length);
					}
					else
					{
						byte[] bigdata = new byte[temp.Length];
						for (int i = 0; i < data.Length; i += 2)
						{
							if (i + 1 < data.Length) bigdata[i] = data[i + 1];
							else bigdata[i] = 0;
							if (i + 1 < bigdata.Length) bigdata[i + 1] = data[i];
						}
						Array.Copy(bigdata, 0, data, position * sizeof(Int64), temp.Length);
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="val"></param>
			public void Set(UInt64 val) { Set(val, 0); }

			/// <summary>
			/// 値を設定します。
			/// </summary>
			/// <param name="val"></param>
			/// <param name="position"></param>
			public void Set(UInt64 val, int position)
			{
				if ((position + 1) * sizeof(UInt64) <= data.Length)
				{
					byte[] temp = BitConverter.GetBytes(val);

					if (endian == Endian.LITTLE)
					{
						Array.Copy(temp, 0, data, position * sizeof(UInt64), temp.Length);
					}
					else
					{
						byte[] bigdata = new byte[temp.Length];
						for (int i = 0; i < data.Length; i += 2)
						{
							if (i + 1 < data.Length) bigdata[i] = data[i + 1];
							else bigdata[i] = 0;
							if (i + 1 < bigdata.Length) bigdata[i + 1] = data[i];
						}
						Array.Copy(bigdata, 0, data, position * sizeof(UInt64), temp.Length);
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="val"></param>
			public void Set(Single val) { Set(val, 0); }

			/// <summary>
			/// 値を設定します。
			/// </summary>
			/// <param name="val"></param>
			/// <param name="position"></param>
			public void Set(Single val, int position)
			{
				if ((position + 1) * sizeof(Single) <= data.Length)
				{
					byte[] temp = BitConverter.GetBytes(val);

					if (endian == Endian.LITTLE)
					{
						Array.Copy(temp, 0, data, position * sizeof(Single), temp.Length);
					}
					else
					{
						byte[] bigdata = new byte[temp.Length];
						for (int i = 0; i < data.Length; i += 2)
						{
							if (i + 1 < data.Length) bigdata[i] = data[i + 1];
							else bigdata[i] = 0;
							if (i + 1 < bigdata.Length) bigdata[i + 1] = data[i];
						}
						Array.Copy(bigdata, 0, data, position * sizeof(Single), temp.Length);
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="val"></param>
			public void Set(Double val) { Set(val, 0); }

			/// <summary>
			/// 値を設定します。
			/// </summary>
			/// <param name="val"></param>
			/// <param name="position"></param>
			public void Set(Double val, int position)
			{
				if ((position + 1) * sizeof(Double) <= data.Length)
				{
					byte[] temp = BitConverter.GetBytes(val);

					if (endian == Endian.LITTLE)
					{
						Array.Copy(temp, 0, data, position * sizeof(Double), temp.Length);
					}
					else
					{
						byte[] bigdata = new byte[temp.Length];
						for (int i = 0; i < data.Length; i += 2)
						{
							if (i + 1 < data.Length) bigdata[i] = data[i + 1];
							else bigdata[i] = 0;
							if (i + 1 < bigdata.Length) bigdata[i + 1] = data[i];
						}
						Array.Copy(bigdata, 0, data, position * sizeof(Double), temp.Length);
					}
				}
			}

			/// <summary>
			/// 
			/// </summary>
			/// <param name="val"></param>
			public void Set(String val) { Set(val, 0); }

			/// <summary>
			/// 値を設定します。
			/// </summary>
			/// <param name="val"></param>
			/// <param name="position"></param>
			public void Set(String val, int position)
			{
				if (position < 0)
				{
					byte[] temp = Encoding.Default.GetBytes(((string)val).PadRight(dataLength, '\0').Substring(0, dataLength));
					if (endian == Endian.LITTLE)
					{
						Array.Copy(temp, 0, data, 0, temp.Length);
					}
					else
					{
						for (int i = 0; i < (temp.Length + 1) / 2; i++)
						{
							if (i * 2 + 1 < temp.Length) data[i * 2 + 0] = temp[i * 2 + 1];
							else data[i * 2 + 0] = 0;
							if (i * 2 + 1 < data.Length) data[i * 2 + 1] = temp[i * 2 + 0];
						}
					}
				}
				else if (position + val.Length <= data.Length)
				{
					byte[] temp = Encoding.Default.GetBytes((string)val);
					if (endian == Endian.LITTLE)
					{
						Array.Copy(temp, 0, data, position, temp.Length);
					}
					else
					{
						for (int i = 0; i < (temp.Length + 1) / 2; i++)
						{
							if (i * 2 + 1 < temp.Length) data[i * 2 + 0] = temp[i * 2 + 1];
							else data[i * 2 + 0] = 0;
							if (i * 2 + 1 < data.Length) data[i * 2 + 1] = temp[i * 2 + 0];
						}
					}
				}
			}

			/// <summary>
			/// 値を設定します。
			/// </summary>
			/// <param name="val"></param>
			public void Set(bool val) { Set(val, 0); }

			/// <summary>
			/// ビット位置を指定して状態を設定します。
			/// </summary>
			/// <param name="val"></param>
			/// <param name="position"></param>
			public void Set(bool val, int position)
			{
				if (position < data.Length)
				{
					//if (valueType == ValueType.BIT)
					{
						data[position] = (byte)(val ? 1 : 0);
					}
				}
			}

			#endregion

			#region 指定ポジションのアドレスを取得します。

			/// <summary>
			/// 指定ポジションのアドレスを取得します。
			/// </summary>
			/// <param name="position"></param>
			/// <returns></returns>
			public int GetAddress(int position)
			{
				if (deviceAddressSequence == DeviceAddressSequence.FLOWBY16)
				{
					int address = deviceAddress + (position / 16) * 100 + position % 16;
					return address;
				}

				return deviceAddress + position * DeviceSize / dataLength;
			}

			/// <summary>
			/// アドレスを取得します。
			/// </summary>
			/// <returns></returns>
			public int GetAddress()
			{
				return GetAddress(0);
			}

			#endregion

			#region デバイスコードを取得します。

			/// <summary>
			/// デバイスコードを取得します。
			/// </summary>
			/// <returns></returns>
			public string GetDeviceCode()
			{
				return Enum.GetName(typeof(DeviceCode), deviceCode);
			}

			#endregion

			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				return deviceCode.ToString() + deviceAddress.ToString() + ((0 < deviceBitPosition) ? "." + deviceBitPosition.ToString("00") : "") + "-" + dataLength.ToString();
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return base.ToString() + " " + ipaddress + ":" + portno;
		}

		/// <summary>
		/// デバイス値の変化イベントのデリゲート宣言
		/// </summary>
		/// <param name="plc"></param>
		/// <param name="deviceinfo"></param>
		public delegate void Event_Handler_ValueChanged(BL_MCProtocol plc, DeviceInfo deviceinfo);

		/// <summary>
		/// デバイス値の変化イベント
		/// </summary>
		public virtual event Event_Handler_ValueChanged EventValueChanged;

		/// <summary>
		/// PLC通信エラーイベントのデリゲート宣言
		/// </summary>
		/// <param name="plc"></param>
		/// <param name="error"></param>
		/// <param name="errordescription"></param>
		public delegate void Event_Handler_PlcError(BL_MCProtocol plc, bool error, string errordescription);

		/// <summary>
		/// PLC通信エラーイベント
		/// </summary>
		public virtual event Event_Handler_PlcError EventPlcError;

		/// <summary>
		/// PLC接続イベントのデリゲート宣言
		/// </summary>
		/// <param name="plc"></param>
		public delegate void Event_Handler_Connected(BL_MCProtocol plc);

		/// <summary>
		/// PLC接続イベント
		/// </summary>
		public virtual event Event_Handler_Connected EventPlcConnected;

		/// <summary>
		/// PLC切断イベントのデリゲート宣言
		/// </summary>
		/// <param name="plc"></param>
		public delegate void Event_Handler_Disconnected(BL_MCProtocol plc);

		/// <summary>
		/// PLC切断イベント
		/// </summary>
		public virtual event Event_Handler_Disconnected EventPlcDisconnected;


		/// <summary>
		/// PLC通信エラーフラグ
		/// </summary>
		public bool Error = false;

		/// <summary>
		/// 通信エラー内容
		/// </summary>
		public string ErrorDescription = "";

		/// <summary>ソケットクラスのインスタンス</summary>
		protected BL_RawSocket Socket = null;

		/// <summary>UDPソケットクラスのインスタンス</summary>
		protected BL_RawSocketUDP SocketUDP = null;
		/// <summary>UDP送信先IPアドレス(IPEndPoint)</summary>
		protected IPEndPoint ipaddressUDP = null;

		/// <summary>接続先のIPエンドポイント</summary>
		protected IPEndPoint RemoteEP = null;
		/// <summary>ソケット読み出しバッファ</summary>
		protected byte[] ReceiveBytes = new byte[0];

		private string ipaddress = "";
		private int portno = 0;
		private int localport = 0;

		/// <summary>シリアル通信クラスのインスタンス</summary>
		protected BL_Rs232c Com = null;
		private BL_Rs232c.PortNo com_portno = BL_Rs232c.PortNo._01;
		private BL_Rs232c.BaudRate com_baudrate = BL_Rs232c.BaudRate._9600;
		private BL_Rs232c.ByteSize com_bytesize = BL_Rs232c.ByteSize._8;
		private BL_Rs232c.Parity com_parity = BL_Rs232c.Parity.None;
		private BL_Rs232c.StopBits com_stopbits = BL_Rs232c.StopBits._1;
		private BL_Rs232c.FormatType com_formattype = BL_Rs232c.FormatType.STX_ETX;

		private bool isConnected = false;
		/// <summary>接続状態を取得します。</summary>
		public bool IsConnected { get { return isConnected; } }

		/// <summary>
		/// 読み込み要求キュー
		/// </summary>
		public Queue<DeviceInfo> queRead = new Queue<DeviceInfo>();
		/// <summary>
		/// 書き込み要求キュー
		/// </summary>
		public Queue<DeviceInfo> queWrite = new Queue<DeviceInfo>();

		/// <summary>
		/// 連続読み込み要求情報
		/// </summary>
		public List<DeviceInfo> ReadDevices = new List<DeviceInfo>();

		private List<DeviceInfo> r_bitdevices = new List<DeviceInfo>();
		private List<DeviceInfo> r_worddevices = new List<DeviceInfo>();
		private List<DeviceInfo> w_bitdevices = new List<DeviceInfo>();
		private List<DeviceInfo> w_worddevices = new List<DeviceInfo>();

		private BL_Stopwatch swRead = new BL_Stopwatch();
		private int read_scantimes = 0;
		private BL_Stopwatch swWrite = new BL_Stopwatch();
		private int write_scantimes = 0;

		/// <summary></summary>
		public List<DeviceInfo> read_eventdevices = new List<DeviceInfo>();
		/// <summary></summary>
		public List<DeviceInfo> write_eventdevices = new List<DeviceInfo>();


		/// <summary>
		/// 読み込み処理の処理速度(ms)を取得します。
		/// </summary>
		public int ReadScanTimes { get { return read_scantimes; } }

		/// <summary>
		/// 書き込み処理の処理速度(ms)を取得します。
		/// </summary>
		public int WriteScanTimes { get { return write_scantimes; } }

		/// <summary>
		/// コンストラクタ(RS232C)
		/// </summary>
		/// <param name="portNo">COMポートNo</param>
		/// <param name="baudRate">ボーレート</param>
		/// <param name="byteSize">データ長</param>
		/// <param name="parity">パリティ</param>
		/// <param name="stopBits">ストップビット</param>
		/// <param name="formatType">フォーマット種別</param>
		public BL_MCProtocol(BL_Rs232c.PortNo portNo, BL_Rs232c.BaudRate baudRate, BL_Rs232c.ByteSize byteSize, BL_Rs232c.Parity parity, BL_Rs232c.StopBits stopBits, BL_Rs232c.FormatType formatType)
		{
			DeviceType = DeviceTypes.RS232C;
			this.ipaddress = "";
			this.com_portno = portNo;
			this.com_baudrate = baudRate;
			this.com_bytesize = byteSize;
			this.com_parity = parity;
			this.com_stopbits = stopBits;
			this.com_formattype = formatType;
			SupportRandomRead = false;
			SupportRandomWrite = false;
		}

		/// <summary>
		/// コンストラクタ(LAN)
		/// </summary>
		/// <param name="isUDP">UDP</param>
		/// <param name="ipaddress">IPアドレス</param>
		/// <param name="portno">ソケットポートNo</param>
		public BL_MCProtocol(bool isUDP, string ipaddress, int portno)
		{
			if (!isUDP)
			{
                DeviceType = DeviceTypes.TCP;

				this.ipaddress = ipaddress;
				this.portno = portno;
			}
			else
			{
                DeviceType = DeviceTypes.UDP;

				this.ipaddress = ipaddress;
				this.portno = portno;

				ipaddressUDP = new IPEndPoint(IPAddress.Parse(ipaddress), portno);
			}
		}

		/// <summary>
		/// コンストラクタ(LAN)
		/// </summary>
		/// <param name="isUDP">UDP</param>
		/// <param name="ipaddress">IPアドレス</param>
		/// <param name="portno">ソケットリモートポートNo</param>
		/// <param name="localport">ソケットローカルポートNo</param>
		public BL_MCProtocol(bool isUDP, string ipaddress, int portno, int localport)
			: this(isUDP, ipaddress, portno)
		{
			this.localport = localport;
		}

		/// <summary>
		/// コンストラクタ(LAN)
		/// </summary>
		/// <param name="isUDP">UDP</param>
		/// <param name="ipaddress">IPアドレス</param>
		/// <param name="portno">ソケットリモートポートNo</param>
		/// <param name="SupportRandomAccess">複数ブロック一括読み出し可能</param>
		public BL_MCProtocol(bool isUDP, string ipaddress, int portno, bool SupportRandomAccess)
			: this(isUDP, ipaddress, portno)
		{
			this.SupportRandomRead = SupportRandomAccess;
			this.SupportRandomWrite = SupportRandomAccess;
		}

        /// <summary>
        /// コンストラクタ(LAN)
        /// </summary>
        /// <param name="isUDP">UDP</param>
        /// <param name="ipaddress">IPアドレス</param>
        /// <param name="portno">ソケットリモートポートNo</param>
		/// <param name="SupportRandomAccess">複数ブロック一括読み出し可能</param>
		/// <param name="isASCII">ASCIIモード</param>
		public BL_MCProtocol(bool isUDP, string ipaddress, int portno, bool SupportRandomAccess, bool isASCII)
			: this(isUDP, ipaddress, portno, SupportRandomAccess)
        {
            this.isASCII = isASCII;
        }

		/// <summary>
		/// コンストラクタ(LAN)
		/// </summary>
		/// <param name="isUDP">UDP</param>
		/// <param name="ipaddress">IPアドレス</param>
		/// <param name="portno">ソケットリモートポートNo</param>
		/// <param name="localport">ソケットローカルポートNo</param>
		/// <param name="SupportRandomAccess">複数ブロック一括読み出し可能</param>
		public BL_MCProtocol(bool isUDP, string ipaddress, int portno, int localport, bool SupportRandomAccess)
			: this(isUDP, ipaddress, portno, localport)
		{
			this.SupportRandomRead = SupportRandomAccess;
			this.SupportRandomWrite = SupportRandomAccess;
		}

        /// <summary>
        /// コンストラクタ(LAN)
        /// </summary>
        /// <param name="isUDP">UDP</param>
        /// <param name="ipaddress">IPアドレス</param>
        /// <param name="portno">ソケットリモートポートNo</param>
        /// <param name="localport">ソケットローカルポートNo</param>
		/// <param name="SupportRandomAccess">複数ブロック一括読み出し可能</param>
		/// <param name="isASCII">ASCIIモード</param>
		public BL_MCProtocol(bool isUDP, string ipaddress, int portno, int localport, bool SupportRandomAccess, bool isASCII)
			: this(isUDP, ipaddress, portno, localport, SupportRandomAccess)
        {
            this.isASCII = isASCII;
        }

		/// <summary>
		/// コンストラクタ(LAN)
		/// </summary>
		/// <param name="ipaddress">IPアドレス</param>
		/// <param name="portno">ソケットポートNo</param>
		public BL_MCProtocol(string ipaddress, int portno)
			: this(false, ipaddress, portno)
		{
		}

		/// <summary>
		/// コンストラクタ(LAN)
		/// </summary>
		/// <param name="ipaddress">IPアドレス</param>
		/// <param name="portno">ソケットポートNo</param>
		/// <param name="localport">ソケットローカルポートNo</param>
		public BL_MCProtocol(string ipaddress, int portno, int localport)
			: this(false, ipaddress, portno, localport)
		{
		}

		/// <summary>
		/// コンストラクタ(LAN)
		/// </summary>
		/// <param name="ipaddress">IPアドレス</param>
		/// <param name="portno">ソケットポートNo</param>
		/// <param name="SupportRandomAccess">複数ブロック一括読み出し可能</param>
		public BL_MCProtocol(string ipaddress, int portno, bool SupportRandomAccess)
			: this(false, ipaddress, portno, SupportRandomAccess)
		{
		}

		/// <summary>
		/// コンストラクタ(LAN)
		/// </summary>
		/// <param name="ipaddress">IPアドレス</param>
		/// <param name="portno">ソケットポートNo</param>
		/// <param name="localport">ソケットローカルポートNo</param>
		/// <param name="SupportRandomAccess">複数ブロック一括読み出し可能</param>
		public BL_MCProtocol(string ipaddress, int portno, int localport, bool SupportRandomAccess)
			: this(false, ipaddress, portno, localport, SupportRandomAccess)
		{
		}

        /// <summary>
        /// コンストラクタ(LAN)
        /// </summary>
        /// <param name="ipaddress">IPアドレス</param>
        /// <param name="portno">ソケットポートNo</param>
        /// <param name="localport">ソケットローカルポートNo</param>
		/// <param name="SupportRandomAccess">複数ブロック一括読み出し可能</param>
        /// <param name="isASCII">ASCIIモード</param>
		public BL_MCProtocol(string ipaddress, int portno, int localport, bool SupportRandomAccess, bool isASCII)
			: this(false, ipaddress, portno, localport, SupportRandomAccess, isASCII)
        {
        }

		/// <summary>
		/// 通信を開始します。
		/// </summary>
		/// <param name="sleep"></param>
		/// <param name="priority"></param>
		/// <returns></returns>
		public override string StartControl(int sleep, System.Threading.ThreadPriority priority)
		{
			return base.StartControl(sleep, priority);
		}

		/// <summary>
		/// 通信を停止します。
		/// </summary>
		public override void StopControl()
		{
			base.StopControl();

			isConnected = false;

			switch (DeviceType)
			{
				case DeviceTypes.TCP:
					if (Socket != null)
					{
						Socket.Close();
						Socket = null;
						m_Step = 0;
					}
					break;

				case DeviceTypes.UDP:
					if (SocketUDP != null)
					{
						SocketUDP.Close();
						SocketUDP = null;
						m_Step = 0;
					}
					break;

				case DeviceTypes.RS232C:
					if (Com != null)
					{
						Com.Close();
						Com = null;
						m_Step = 0;
					}
					break;
			}
		}

		/// <summary>
		/// 通信処理
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		protected override bool DoControl(object message)
		{
			switch (m_Step)
			{
				case 0:
					#region PLC接続
					{
						switch (DeviceType)
						{
							case DeviceTypes.TCP:
								if (Socket != null)
								{
									if (Socket.IsConnected) Socket.Close();
									Socket = null;
								}
								Socket = new BL_RawSocket(ipaddress, portno);
								Socket.ConnectTimeOut = 100;
								break;

							case DeviceTypes.UDP:
								if (SocketUDP != null)
								{
									SocketUDP.Close();
									SocketUDP = null;
								}
								SocketUDP = new BL_RawSocketUDP(localport);
								break;

							case DeviceTypes.RS232C:
								if (Com != null)
								{
									if (Com.IsOpened) Com.Close();
									Com = null;
								}
								Com = new BL_Rs232c();
								SupportRandomRead = false;
								SupportRandomWrite = false;
								break;
						}

						try
						{
							switch (DeviceType)
							{
								case DeviceTypes.TCP:
									if (!Socket.Open(BL_RawSocket.OpenMode.Client, BL_RawSocket.FormatType.None))
									{
										Error = true;
										ErrorDescription = "PLC接続エラー";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;

										Socket = null;
										isConnected = false;
										break;
									}
									else
									{
										if (EventPlcConnected != null) EventPlcConnected(this);

										isConnected = true;
										Error = false;
										ErrorDescription = "";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 100;
									}
									break;

								case DeviceTypes.UDP:
									if (!SocketUDP.Open(BL_RawSocketUDP.FormatType.None))
									{
										if (EventPlcConnected != null) EventPlcConnected(this);

										Error = true;
										ErrorDescription = "PLC接続エラー";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;

										SocketUDP = null;
										isConnected = false;
										break;
									}
									else
									{
										isConnected = true;
										Error = false;
										ErrorDescription = "";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 100;
									}
									break;


								case DeviceTypes.RS232C:
									if (!Com.Open(this.com_portno, this.com_baudrate, this.com_bytesize, this.com_parity, this.com_stopbits, this.com_formattype))
									{
										Error = true;
										ErrorDescription = "PLC接続エラー";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;

										Com = null;
										isConnected = false;
										break;
									}
									else
									{
										if (EventPlcConnected != null) EventPlcConnected(this);

										isConnected = true;
										Error = false;
										ErrorDescription = "";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 100;
									}
									break;
							}
						}
						catch (Exception ex)
						{
							Error = true;
							ErrorDescription = "PLC接続エラー";
							if (EventPlcError != null) EventPlcError(this, Error, ex.Message);
							m_Step = 10;

							Socket = null;
							break;
						}
					}
					#endregion
					break;

				case 10:
					#region 通信異常処理
					{
						isConnected = false;
						switch (DeviceType)
						{
							case DeviceTypes.TCP:
								if (Socket != null)
								{
									if (Socket.IsConnected) Socket.Close();
									Socket = null;
								}
								break;
							case DeviceTypes.UDP:
								//if (SocketUDP != null) SocketUDP.Close();
								break;
							case DeviceTypes.RS232C:
								if (Com != null)
								{
									if (Com.IsOpened) Com.Close();
									Com = null;
								}
								break;
						}

						if (EventPlcDisconnected != null) EventPlcDisconnected(this);

						m_Step = 20;
						m_swTemp.Restart();
					}
					#endregion
					break;

				case 20:
					#region 通信異常復旧待機
					if (1000 < m_swTemp.ElapsedMilliseconds)
					{
						m_Step = 0;
					}
					#endregion
					break;

				case 100:
					#region 読み出し要求コマンド送信(複数ブロック一括読み出し)
					{
						switch (DeviceType)
						{
							case DeviceTypes.TCP:
								if (!Socket.IsConnected) m_Step = 10;
								break;

							case DeviceTypes.UDP:
								break;

							case DeviceTypes.RS232C:
								if (!Com.IsOpened) m_Step = 10;
								break;
						}

						if (m_Step != 100) break;

						swRead.Restart();

						if (queRead.Count == 0)
						{
							lock (queRead)
							{
								lock (ReadDevices) foreach (var v in ReadDevices) queRead.Enqueue(v);
							}
						}
						if (queRead.Count == 0 && queWrite.Count == 0) break;

						if (queRead.Count == 0 && 0 < queWrite.Count)
						{
							//読み出し要求が無ければ書き込み要求処理へ
							m_Step = 400;
							if (!SupportRandomWrite) m_Step = 600;
							break;
						}

						r_bitdevices.Clear();
						r_worddevices.Clear();

						int size = 0;
						int blockcount = 0;

						lock (queRead)
						{
							//List<BL_MCProtocol.DeviceInfo> vlist = new List<DeviceInfo>();

							//while (0 < queRead.Count)
							//{
							//	BL_MCProtocol.DeviceInfo v = queRead.Dequeue();

							//	if (v.DeviceSize <= 960) vlist.Add(v);
							//	else
							//	{
							//		for (int address = v.deviceAddress; address < v.deviceAddress + v.DeviceSize; address += 960)
							//		{
							//			int devicesize = 0;
							//			if (address + 960 < v.deviceAddress + v.DeviceSize) devicesize = 960;
							//			else devicesize = v.deviceAddress + v.DeviceSize - address;

							//			BL_MCProtocol.DeviceInfo vv = new DeviceInfo(v.deviceCode, address, devicesize / (v.UnitSize / 2), v.valueType);
							//			vlist.Add(vv);
							//		}
							//	}
							//}

							//foreach (var v in vlist)
							//{
							//	queRead.Enqueue(v);
							//}

							while (0 < queRead.Count)
							{
								BL_MCProtocol.DeviceInfo v = queRead.Peek();
								v.changed = false;

								if (size + v.DeviceSize <= 960 && blockcount + 1 <= 120)
								{
									queRead.Dequeue();

									if (v.deviceKind == DeviceInfo.DeviceKind.BIT)
									{
										r_bitdevices.Add(v);
										size += v.DeviceSize;
										blockcount++;
									}

									if (v.deviceKind == DeviceInfo.DeviceKind.WORD)
									{
										r_worddevices.Add(v);
										size += v.DeviceSize;
										blockcount++;
									}
								}
								else break;

								if (!SupportRandomRead)
								{
									if (0 < r_bitdevices.Count || 0 < r_worddevices.Count)
									{
										break;
									}
								}
							}
						}

						if (!SupportRandomRead)
						{
							if (0 < r_bitdevices.Count) m_Step = 200;
							else if (0 < r_worddevices.Count) m_Step = 300;
							break;
						}

						List<byte> data = new List<byte>();
						int datalength = 0;

						if (isASCII)
						{
							#region ASCII

							string d = "";
							d += "5000";	//サブヘッダ
							d += "00";		//ネットワーク番号
							d += "FF";		//ＰＣ番号
							d += "03FF";	//要求先ユニットＩ／Ｏ番号
							d += "00";		//要求先ユニット局番号
							d += "@len";	//要求データ長
							d += "0000";	//ＣＰＵ監視タイマ

							d += "0406";	//コマンド
							d += "0000";	//サブコマンド
							d += r_worddevices.Count.ToString("X2");
							d += r_bitdevices.Count.ToString("X2");

							foreach (var v in r_worddevices)
							{
								int address = KV5000 ? v.deviceAddress % 100000 : v.deviceAddress;
								d += v.deviceCode.ToString().PadRight(2, '*');
								if (v.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC) d += address.ToString("D6");
								else d += address.ToString("X6");
								d += v.DeviceSize.ToString("X4");
							}

							foreach (var v in r_bitdevices)
							{
								int address = v.deviceAddress;
								if (v.deviceAddressSequence == DeviceInfo.DeviceAddressSequence.FLOWBY16)
								{
									address = address / 100 * 16 + address % 100;
								}

								address = KV5000 ? address % 100000 : address;

								d += v.deviceCode.ToString().PadRight(2, '*');
								if (v.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC) d += address.ToString("D6");
								else d += address.ToString("X6");
								d += ((v.DeviceSize + 15) / 16).ToString("X4");
							}

							d = d.Replace("@len", (d.Length - 18).ToString("X4"));

							byte[] dd = Encoding.ASCII.GetBytes(d);
							data.AddRange(dd);

							#endregion
						}
						else
						{
							#region BINARY

							data.Clear();
							data.Add(0x50); //サブヘッダ
							data.Add(0x00); //サブヘッダ
							data.Add(0x00); //ネットワーク番号
							data.Add(0xFF); //ＰＣ番号
							data.Add(0xFF); //要求先ユニットＩ／Ｏ番号
							data.Add(0x03); //要求先ユニットＩ／Ｏ番号
							data.Add(0x00); //要求先ユニット局番号
							data.Add(0); //7:要求データ長
							data.Add(0); //8:要求データ長

							data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ
							data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ

							data.Add(0x06); datalength++;   //コマンド
							data.Add(0x04); datalength++;   //コマンド
							data.Add(0x00); datalength++;   //サブコマンド
							data.Add(0x00); datalength++;   //サブコマンド

							data.Add((byte)r_worddevices.Count); datalength++;    //ワードデバイスブロック数
							data.Add((byte)r_bitdevices.Count); datalength++;     //ワードデバイスブロック数

							foreach (var v in r_worddevices)
							{
								int address = KV5000 ? v.deviceAddress % 100000 : v.deviceAddress;
								data.Add((byte)(address & 0x000000FF)); datalength++;
								data.Add((byte)((address >> 8) & 0x000000FF)); datalength++;
								data.Add((byte)((address >> 16) & 0x000000FF)); datalength++;
								data.Add((byte)v.deviceCode); datalength++;
								data.Add((byte)(v.DeviceSize & 0x000000FF)); datalength++;
								data.Add((byte)((v.DeviceSize >> 8) & 0x000000FF)); datalength++;
							}

							foreach (var v in r_bitdevices)
							{
								int address = v.deviceAddress;
								if (v.deviceAddressSequence == DeviceInfo.DeviceAddressSequence.FLOWBY16)
								{
									address = address / 100 * 16 + address % 100;
								}

								address = KV5000 ? address % 100000 : address;

								data.Add((byte)(address & 0x000000FF)); datalength++;
								data.Add((byte)((address >> 8) & 0x000000FF)); datalength++;
								data.Add((byte)((address >> 16) & 0x000000FF)); datalength++;
								data.Add((byte)v.deviceCode); datalength++;
								data.Add((byte)(((v.DeviceSize + 15) / 16) & 0x000000FF)); datalength++;
								data.Add((byte)((((v.DeviceSize + 15) / 16) >> 8) & 0x000000FF)); datalength++;
							}

							data[7] = (byte)(datalength & 0x000000FF);
							data[8] = (byte)((datalength >> 8) & 0x000000FF);

							#endregion
						}

						byte[] senddata = data.ToArray();
						ReceiveBytes = new byte[0];

						switch (DeviceType)
						{
							case DeviceTypes.TCP:
								if (!Socket.SendBytes(senddata))
								{
									m_Step = 10;
									Error = true;
									ErrorDescription = "PLC送信エラー[" + Socket.ErrorMessage + "]";
									if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
									break;
								}
								break;

							case DeviceTypes.UDP:
								if (!SocketUDP.SendBytes(senddata, ipaddressUDP))
								{
									m_Step = 10;
									Error = true;
									ErrorDescription = "PLC送信エラー[" + SocketUDP.ErrorMessage + "]";
									if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
									break;
								}
								break;
						}

						m_Step = 110;
						m_swTemp.Restart();

						data.Clear();
						data = null;
					}
					#endregion
					break;

				case 110:
					#region 読み出し応答コマンド受信(複数ブロック一括読み出し)
					{
						byte[] data = null;
						switch (DeviceType)
						{
							case DeviceTypes.TCP:
								data = Socket.ReceiveBytes();
								break;
							case DeviceTypes.UDP:
								data = SocketUDP.ReceiveBytes();
								break;
						}
						if (data == null)
						{
							Error = true;
							ErrorDescription = "デバイス種別異常";
							if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
							m_Step = 10;
							break;
						}

						BL_Bytes.Join(data, ref ReceiveBytes, data.Length);

						byte[] receive_bytes = new byte[ReceiveBytes.Length];
						Array.Copy(ReceiveBytes, receive_bytes, ReceiveBytes.Length);

						if (isASCII)
						{
							#region ASCII->BINARY

							if (18 <= receive_bytes.Length)
							{
								byte[] d = new byte[receive_bytes.Length / 2];

								string s = "";
								s = Encoding.ASCII.GetString(receive_bytes, 0, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[0]);
								s = Encoding.ASCII.GetString(receive_bytes, 2, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[1]);
								s = Encoding.ASCII.GetString(receive_bytes, 4, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[2]);
								s = Encoding.ASCII.GetString(receive_bytes, 6, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[3]);
								s = Encoding.ASCII.GetString(receive_bytes, 8, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[4]);
								s = Encoding.ASCII.GetString(receive_bytes, 10, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[5]);
								s = Encoding.ASCII.GetString(receive_bytes, 12, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[6]);

								s = Encoding.ASCII.GetString(receive_bytes, 14, 4);
								int len = 0; int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out len);
								len = len / 2;
								d[7] = (byte)(len % 0x100);
								d[8] = (byte)(len / 0x100);

								int dpos = 9;
								for (int pos = 18; pos < receive_bytes.Length - 3; pos += 4)
								{
									s = Encoding.ASCII.GetString(receive_bytes, pos, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[dpos + 1]);
									s = Encoding.ASCII.GetString(receive_bytes, pos + 2, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[dpos]);
									dpos += 2;
								}

								receive_bytes = d;
							}
							else
							{
								if (3000 < m_swTemp.ElapsedMilliseconds)
								{
									Error = true;
									ErrorDescription = "PLC応答タイムアウト";
									if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
									m_Step = 10;
								}
								break;
							}

							#endregion
						}

						while (11 <= receive_bytes.Length)
						{
							if (2 <= receive_bytes.Length && receive_bytes[0] == 0xD0 && receive_bytes[1] == 0x00)
							{
								break;
							}
							else
							{
								receive_bytes = BL_Bytes.Trim(receive_bytes, 1, receive_bytes.Length - 1);
							}
						}

						if (11 <= receive_bytes.Length)
						{
							int length = receive_bytes[7] + receive_bytes[8] * 0x100 + 9;

							if (length <= receive_bytes.Length)
							{
								data = BL_Bytes.Trim(receive_bytes, length);
								ReceiveBytes = new byte[0];

								if (data[9] == 0 && data[10] == 0)
								{
									int position = 11;
									foreach (var v in r_worddevices)
									{
										byte[] d = BL_Bytes.Trim(data, position, v.dataLength * v.UnitSize);
										Array.Copy(d, v.data, v.dataLength * v.UnitSize);
										position += v.dataLength * v.UnitSize;

										if (v.negative) for (int i = 0; i < v.data.Length; i++) v.data[i] = (byte)~v.data[i];

										v.readed = true;
									}

									foreach (var v in r_bitdevices)
									{
										int bitposition = 0;
										for (int spos = 0; spos < (v.dataLength + 15) / 16; spos++)
										{
											ushort b = 0;
											BL_BitConverter.ToValue(data, ref position, out b);
											BL_BitOperator bit = new BL_BitOperator(b);


											for (int i = 0; bitposition < v.dataLength && i < 16; i++)
											{
												if (v.negative) v.data[bitposition] = (byte)(bit[v.deviceBitPosition + i] ? 0 : 1);
												else v.data[bitposition] = (byte)(bit[v.deviceBitPosition + i] ? 1 : 0);
												bitposition++;
											}
										}

										v.readed = true;
									}

									//変化検出
									{
										foreach (var v in r_worddevices)
										{
											if (v.data_pre == null)
											{
												v.data_pre = new byte[v.data.Length];
												for (int i = 0; i < v.data.Length; i++) v.data_pre[i] = (byte)(v.data[i] + 1);
											}

											//前回値と比較(.NET3.5にはSequenceEqualが無いため)
											bool diff = false;
											for (int i = 0; i < v.data.Length; i++)
											{
												if (v.data[i] != v.data_pre[i])
												{
													diff = true;
													break;
												}
											}

											//if (!v.data.SequenceEqual(v.data_pre))
											if (diff)
											{
												if (!v.noevent)
												{
													if (EventValueChanged != null) EventValueChanged(this, v);
												}

												Array.Copy(v.data, v.data_pre, v.data.Length);
												v.changed = true;
											}
										}

										foreach (var v in r_bitdevices)
										{
											if (v.data_pre == null)
											{
												v.data_pre = new byte[v.data.Length];
												for (int i = 0; i < v.data.Length; i++) v.data_pre[i] = (byte)(v.data[i] + 1);
											}

											//前回値と比較(.NET3.5にはSequenceEqualが無いため)
											bool diff = false;
											for (int i = 0; i < v.data.Length; i++)
											{
												if (v.data[i] != v.data_pre[i])
												{
													diff = true;
													break;
												}
											}

											//if (!v.data.SequenceEqual(v.data_pre))
											if (diff)
											{
												if (!v.noevent)
												{
													if (EventValueChanged != null) EventValueChanged(this, v);
												}

												Array.Copy(v.data, v.data_pre, v.data.Length);
												v.changed = true;
											}
										}
									}

									if (0 < queWrite.Count)
									{
										//書き込み要求があれば書き込み要求処理へ
										m_Step = 400;
										if (!SupportRandomWrite) m_Step = 600;
									}
									else
									{
										m_Step = 100;
									}

									if (swRead.IsRunning) if (0 < swRead.ElapsedMilliseconds) read_scantimes = swRead.ElapsedMilliseconds;
								}
								else
								{
									Error = true;
									ErrorDescription = "PLC読込応答エラー[" + data[10].ToString("X2") + data[9].ToString("X2") + "]";
									if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
									m_Step = 10;
								}
							}
						}
						else if (3000 < m_swTemp.ElapsedMilliseconds)
						{
							Error = true;
							ErrorDescription = "PLC応答タイムアウト";
							if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
							m_Step = 10;
						}
					}
					#endregion
					break;


				case 200:
					#region BIT読み込み要求コマンド送信(BIT一括読み出し)
					{
						if (0 < r_bitdevices.Count)
						{
							DeviceInfo v = r_bitdevices[0];
							v.changed = false;

							List<byte> data = new List<byte>();
							int datalength = 0;

							switch (DeviceType)
							{
								case DeviceTypes.TCP:
								case DeviceTypes.UDP:
									#region TCP/UDP
									{
										if (isASCII)
										{
											#region ASCII
											
											string d = "";
											d += "5000";	//サブヘッダ
											d += "00";		//ネットワーク番号
											d += "FF";		//ＰＣ番号
											d += "03FF";	//要求先ユニットＩ／Ｏ番号
											d += "00";		//要求先ユニット局番号
											d += "@len";	//要求データ長
											d += "0000";	//ＣＰＵ監視タイマ
											
											d += "0401";	//コマンド
											d += "0001";	//サブコマンド

											int address = v.deviceAddress;
											if (v.deviceAddressSequence == DeviceInfo.DeviceAddressSequence.FLOWBY16)
											{
												address = address / 100 * 16 + address % 100;
											}

											address = KV5000 ? address % 100000 : address;

											d += v.deviceCode.ToString().PadRight(2, '*');
											if (v.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC) d += address.ToString("D6");
											else d += address.ToString("X6");
											d += v.DeviceSize.ToString("X4");

											d = d.Replace("@len", (d.Length - 18).ToString("X4"));

											byte[] dd = Encoding.ASCII.GetBytes(d);
											data.AddRange(dd);

											#endregion
										}
										else
										{
											#region BINARY

											data.Add(0x50); //サブヘッダ
											data.Add(0x00); //サブヘッダ
											data.Add(0x00); //ネットワーク番号
											data.Add(0xFF); //ＰＣ番号
											data.Add(0xFF); //要求先ユニットＩ／Ｏ番号
											data.Add(0x03); //要求先ユニットＩ／Ｏ番号
											data.Add(0x00); //要求先ユニット局番号
											data.Add(0); //7:要求データ長
											data.Add(0); //8:要求データ長

											data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ
											data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ

											data.Add(0x01); datalength++;   //コマンド
											data.Add(0x04); datalength++;   //コマンド
											data.Add(0x01); datalength++;   //サブコマンド
											data.Add(0x00); datalength++;   //サブコマンド

											int address = v.deviceAddress;
											if (v.deviceAddressSequence == DeviceInfo.DeviceAddressSequence.FLOWBY16)
											{
												address = address / 100 * 16 + address % 100;
											}

											address = KV5000 ? address % 100000 : address;

											data.Add((byte)(address & 0x000000FF)); datalength++;
											data.Add((byte)((address >> 8) & 0x000000FF)); datalength++;
											data.Add((byte)((address >> 16) & 0x000000FF)); datalength++;
											data.Add((byte)v.deviceCode); datalength++;
											data.Add((byte)(v.DeviceSize & 0x000000FF)); datalength++;
											data.Add((byte)((v.DeviceSize >> 8) & 0x000000FF)); datalength++;

											data[7] = (byte)(datalength & 0x000000FF);
											data[8] = (byte)((datalength >> 8) & 0x000000FF);
											
											#endregion
										}

										ReceiveBytes = new byte[0];

										if (DeviceType == DeviceTypes.TCP)
										{
											if (!Socket.SendBytes(data.ToArray()))
											{
												m_Step = 10;
												Error = true;
												ErrorDescription = "PLC送信エラー[" + Socket.ErrorMessage + "]";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
												return true;
											}
										}
										else if (DeviceType == DeviceTypes.UDP)
										{
											if (!SocketUDP.SendBytes(data.ToArray(), ipaddressUDP))
											{
												m_Step = 10;
												Error = true;
												ErrorDescription = "PLC送信エラー[" + SocketUDP.ErrorMessage + "]";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
												return true;
											}
										}
									}
									#endregion
									break;

								case DeviceTypes.RS232C:
									#region RS232C
									{
										switch (CommandType)
										{
											case CommandTypes.MELSEC_A_CPU:
												{
													data.AddRange(Encoding.Default.GetBytes("00FFBR0"));
													data.Add((byte)v.GetDeviceCode()[0]);
													if (v.deviceAddresstype == DeviceInfo.DeviceAddressType.HEX)
													{
														data.AddRange(Encoding.Default.GetBytes(v.deviceAddress.ToString("X4")));
													}
													else if (v.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC)
													{
														data.AddRange(Encoding.Default.GetBytes(v.deviceAddress.ToString("0000")));
													}
													data.AddRange(Encoding.Default.GetBytes((v.DeviceSize & 0x00FF).ToString("X2")));
												}
												break;

											case CommandTypes.MELSEC_AnA_CPU:
												{
													data.AddRange(Encoding.Default.GetBytes("00FFJR0"));
													data.Add((byte)v.GetDeviceCode()[0]);
													if (v.deviceAddresstype == DeviceInfo.DeviceAddressType.HEX)
													{
														data.AddRange(Encoding.Default.GetBytes(v.deviceAddress.ToString("X6")));
													}
													else if (v.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC)
													{
														data.AddRange(Encoding.Default.GetBytes(v.deviceAddress.ToString("000000")));
													}
													data.AddRange(Encoding.Default.GetBytes((v.DeviceSize & 0x00FF).ToString("X2")));
												}
												break;
										}

										ReceiveBytes = new byte[0];
										if (!Com.SendBytes(data.ToArray()))
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC送信エラー[" + Com.ErrorMessage + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											return true;
										}
									}
									#endregion
									break;
							}

							m_Step = 210;
							m_swTemp.Restart();

							data.Clear();
							data = null;
						}
					}
					#endregion
					break;

				case 210:
					#region BIT読み込み応答コマンド受信(BIT一括読み出し)
					{
						DeviceInfo v = r_bitdevices[0];

						switch (DeviceType)
						{
							case DeviceTypes.TCP:
							case DeviceTypes.UDP:
								#region TCP/UDP
								{
									byte[] data = null;
									switch (DeviceType)
									{
										case DeviceTypes.TCP:
											data = Socket.ReceiveBytes();
											break;
										case DeviceTypes.UDP:
											data = SocketUDP.ReceiveBytes();
											break;
									}
									if (data == null)
									{
										Error = true;
										ErrorDescription = "デバイス種別異常";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
										break;
									}

									BL_Bytes.Join(data, ref ReceiveBytes, data.Length);

									byte[] receive_bytes = new byte[ReceiveBytes.Length];
									Array.Copy(ReceiveBytes, receive_bytes, ReceiveBytes.Length);

									if (isASCII)
									{
										#region ASCII->BINARY

										if (18 <= receive_bytes.Length)
										{
											byte[] d = new byte[receive_bytes.Length / 2];

											string s = "";
											s = Encoding.ASCII.GetString(receive_bytes, 0, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[0]);
											s = Encoding.ASCII.GetString(receive_bytes, 2, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[1]);
											s = Encoding.ASCII.GetString(receive_bytes, 4, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[2]);
											s = Encoding.ASCII.GetString(receive_bytes, 6, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[3]);
											s = Encoding.ASCII.GetString(receive_bytes, 8, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[4]);
											s = Encoding.ASCII.GetString(receive_bytes, 10, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[5]);
											s = Encoding.ASCII.GetString(receive_bytes, 12, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[6]);

											s = Encoding.ASCII.GetString(receive_bytes, 14, 4);
											int len = 0; int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out len);
											len = len / 2;
											d[7] = (byte)(len % 0x100);
											d[8] = (byte)(len / 0x100);

											int dpos = 9;
											for (int pos = 18; pos < receive_bytes.Length - 1; pos += 2)
											{
												s = Encoding.ASCII.GetString(receive_bytes, pos, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[dpos]);
												dpos++;
											}

											receive_bytes = d;
										}
										else
										{
											if (3000 < m_swTemp.ElapsedMilliseconds)
											{
												Error = true;
												ErrorDescription = "PLC応答タイムアウト";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
												m_Step = 10;
											}
											break;
										}

										#endregion
									}

									while (11 <= receive_bytes.Length)
									{
										if (receive_bytes[0] == 0xD0 && receive_bytes[1] == 0x00)
										{
											break;
										}
										else
										{
											receive_bytes = BL_Bytes.Trim(receive_bytes, 2, receive_bytes.Length - 2);
										}
									}

									if (11 <= receive_bytes.Length)
									{
										int length = receive_bytes[7] + receive_bytes[8] * 0x100 + 9;

										if (length <= receive_bytes.Length)
										{
											data = BL_Bytes.Trim(receive_bytes, length);
											ReceiveBytes = new byte[0];

											if (data[9] == 0 && data[10] == 0)
											{
												int position = 11;

												for (int i = 0; i < (v.DeviceSize + 1) / 2; i++)
												{
													if ((data[position] & 0xF0) == 0) v.data[i * 2 + 0] = (byte)(v.negative ? 1 : 0);
													else v.data[i * 2 + 0] = (byte)(v.negative ? 0 : 1);

													if (i * 2 + 1 < v.data.Length)
													{
														if ((data[position] & 0x0F) == 0) v.data[i * 2 + 1] = (byte)(v.negative ? 1 : 0);
														else v.data[i * 2 + 1] = (byte)(v.negative ? 0 : 1);
													}

													position++;
												}

												v.readed = true;

												//変化検出
												{
													if (v.data_pre == null)
													{
														v.data_pre = new byte[v.data.Length];
														for (int i = 0; i < v.data.Length; i++) v.data_pre[i] = (byte)(v.data[i] + 1);
													}

													//前回値と比較(.NET3.5にはSequenceEqualが無いため)
													bool diff = false;
													for (int i = 0; i < v.data.Length; i++)
													{
														if (v.data[i] != v.data_pre[i])
														{
															diff = true;
															break;
														}
													}

													//if (!v.data.SequenceEqual(v.data_pre))
													if (diff)
													{
														if (!v.noevent)
														{
															if (EventValueChanged != null) EventValueChanged(this, v);
														}

														Array.Copy(v.data, v.data_pre, v.data.Length);
														v.changed = true;
													}
												}

												r_bitdevices.Remove(v);

												if (0 < r_bitdevices.Count) m_Step = 200;
												else if (0 < r_worddevices.Count) m_Step = 300;
												else if (0 < queWrite.Count)
												{
													m_Step = 400;
                                                    if (!SupportRandomWrite) m_Step = 600;
												}
												else m_Step = 100;
											}
											else
											{
												Error = true;
												ErrorDescription = "PLC読込応答エラー[" + data[10].ToString("X2") + data[9].ToString("X2") + "]";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
												m_Step = 10;
											}
										}
									}
									else if (3000 < m_swTemp.ElapsedMilliseconds)
									{
										Error = true;
										ErrorDescription = "PLC応答タイムアウト";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
									}
								}
								#endregion
								break;

							case DeviceTypes.RS232C:
								#region RS232C
								{
									byte[] data = Com.ReceiveBytes();
									BL_Bytes.Join(data, ref ReceiveBytes, data.Length);

									if (ReceiveBytes.Length >= 8)
									{
										if (ReceiveBytes[4] != 0x4E || ReceiveBytes[5] != 0x4E)
										{
											int position = 4;

											{
												for (int i = 0; i < v.DeviceSize; i++)
												{
													if (data[position] == 0x30) v.data[i] = (byte)(v.negative ? 1 : 0);
													else v.data[i] = (byte)(v.negative ? 0 : 1);

													position++;
												}

												v.readed = true;

												//変化検出
												{
													if (v.data_pre == null)
													{
														v.data_pre = new byte[v.data.Length];
														for (int i = 0; i < v.data.Length; i++) v.data_pre[i] = (byte)(v.data[i] + 1);
													}

													//前回値と比較(.NET3.5にはSequenceEqualが無いため)
													bool diff = false;
													for (int i = 0; i < v.data.Length; i++)
													{
														if (v.data[i] != v.data_pre[i])
														{
															diff = true;
															break;
														}
													}

													//if (!v.data.SequenceEqual(v.data_pre))
													if (diff)
													{
														if (!v.noevent)
														{
															if (EventValueChanged != null) EventValueChanged(this, v);
														}

														Array.Copy(v.data, v.data_pre, v.data.Length);
														v.changed = true;
													}
												}
											}

											r_bitdevices.Remove(v);

											if (0 < r_bitdevices.Count) m_Step = 200;
											else if (0 < r_worddevices.Count) m_Step = 300;
											else if (0 < queWrite.Count)
											{
												m_Step = 400;
                                                if (!SupportRandomWrite) m_Step = 600;
											}
											else m_Step = 100;
										}
										else
										{
											Error = true;
											ErrorDescription = "PLC読込応答エラー[" + ReceiveBytes[7].ToString("X2") + ReceiveBytes[6].ToString("X2") + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											m_Step = 10;
										}
									}
									else if (3000 < m_swTemp.ElapsedMilliseconds)
									{
										Error = true;
										ErrorDescription = "PLC応答タイムアウト";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
									}
								}
								#endregion
								break;
						}
					}
					#endregion
					break;


				case 300:
					#region WORD読み込み要求コマンド送信(WORD一括読み出し)
					{
						if (0 < r_worddevices.Count)
						{
							DeviceInfo v = r_worddevices[0];
							v.changed = false;

							List<byte> data = new List<byte>();
							int datalength = 0;

							switch (DeviceType)
							{
								case DeviceTypes.TCP:
								case DeviceTypes.UDP:
									#region TCP/UDP
									{
										if (isASCII)
										{
											#region ASCII

											string d = "";
											d += "5000";	//サブヘッダ
											d += "00";		//ネットワーク番号
											d += "FF";		//ＰＣ番号
											d += "03FF";	//要求先ユニットＩ／Ｏ番号
											d += "00";		//要求先ユニット局番号
											d += "@len";	//要求データ長
											d += "0000";	//ＣＰＵ監視タイマ

											d += "0401";	//コマンド
											d += "0000";	//サブコマンド

											int address = v.deviceAddress;
											d += v.deviceCode.ToString().PadRight(2, '*');
											if (v.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC) d += address.ToString("D6");
											else d += address.ToString("X6");
											d += v.DeviceSize.ToString("X4");

											d = d.Replace("@len", (d.Length - 18).ToString("X4"));

											byte[] dd = Encoding.ASCII.GetBytes(d);
											data.AddRange(dd);

											#endregion
										}
										else
										{
											#region BINARY
											
											data.Add(0x50); //サブヘッダ
											data.Add(0x00); //サブヘッダ
											data.Add(0x00); //ネットワーク番号
											data.Add(0xFF); //ＰＣ番号
											data.Add(0xFF); //要求先ユニットＩ／Ｏ番号
											data.Add(0x03); //要求先ユニットＩ／Ｏ番号
											data.Add(0x00); //要求先ユニット局番号
											data.Add(0); //7:要求データ長
											data.Add(0); //8:要求データ長

											data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ
											data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ

											data.Add(0x01); datalength++;   //コマンド
											data.Add(0x04); datalength++;   //コマンド
											data.Add(0x00); datalength++;   //サブコマンド
											data.Add(0x00); datalength++;   //サブコマンド

											data.Add((byte)(v.deviceAddress & 0x000000FF)); datalength++;
											data.Add((byte)((v.deviceAddress >> 8) & 0x000000FF)); datalength++;
											data.Add((byte)((v.deviceAddress >> 16) & 0x000000FF)); datalength++;
											data.Add((byte)v.deviceCode); datalength++;
											data.Add((byte)(v.DeviceSize & 0x000000FF)); datalength++;
											data.Add((byte)((v.DeviceSize >> 8) & 0x000000FF)); datalength++;

											data[7] = (byte)(datalength & 0x000000FF);
											data[8] = (byte)((datalength >> 8) & 0x000000FF);

											#endregion
										}

										ReceiveBytes = new byte[0];

										if (DeviceType == DeviceTypes.TCP)
										{
											if (!Socket.SendBytes(data.ToArray()))
											{
												m_Step = 10;
												Error = true;
												ErrorDescription = "PLC送信エラー[" + Socket.ErrorMessage + "]";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
												return true;
											}
										}
										else if (DeviceType == DeviceTypes.UDP)
										{
											if (!SocketUDP.SendBytes(data.ToArray(), ipaddressUDP))
											{
												m_Step = 10;
												Error = true;
												ErrorDescription = "PLC送信エラー[" + SocketUDP.ErrorMessage + "]";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
												return true;
											}
										}
									}
									#endregion
									break;

								case DeviceTypes.RS232C:
									#region RS232C
									{
										switch (CommandType)
										{
											case CommandTypes.MELSEC_A_CPU:
												{
													data.AddRange(Encoding.Default.GetBytes("00FFWR0"));
													data.Add((byte)v.GetDeviceCode()[0]);
													if (v.deviceAddresstype == DeviceInfo.DeviceAddressType.HEX)
													{
														data.AddRange(Encoding.Default.GetBytes(v.deviceAddress.ToString("X4")));
													}
													else if (v.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC)
													{
														data.AddRange(Encoding.Default.GetBytes(v.deviceAddress.ToString("0000")));
													}
													data.AddRange(Encoding.Default.GetBytes((v.DeviceSize & 0x00FF).ToString("X2")));
												}
												break;

											case CommandTypes.MELSEC_AnA_CPU:
												{
													data.AddRange(Encoding.Default.GetBytes("00FFQR0"));
													data.Add((byte)v.GetDeviceCode()[0]);
													if (v.deviceAddresstype == DeviceInfo.DeviceAddressType.HEX)
													{
														data.AddRange(Encoding.Default.GetBytes(v.deviceAddress.ToString("X6")));
													}
													else if (v.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC)
													{
														data.AddRange(Encoding.Default.GetBytes(v.deviceAddress.ToString("000000")));
													}
													data.AddRange(Encoding.Default.GetBytes((v.DeviceSize & 0x00FF).ToString("X2")));
												}
												break;
										}

										ReceiveBytes = new byte[0];
										if (!Com.SendBytes(data.ToArray()))
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC送信エラー[" + Com.ErrorMessage + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											return true;
										}
									}
									#endregion
									break;
							}

							m_Step = 310;
							m_swTemp.Restart();

							data.Clear();
							data = null;
						}
					}
					#endregion
					break;

				case 310:
					#region WORD読み込み応答コマンド受信(WORD一括読み出し)
					{
						DeviceInfo v = r_worddevices[0];

						switch (DeviceType)
						{
							case DeviceTypes.TCP:
							case DeviceTypes.UDP:
								#region TCP/UDP
								{
									byte[] data = null;
									switch (DeviceType)
									{
										case DeviceTypes.TCP:
											data = Socket.ReceiveBytes();
											break;
										case DeviceTypes.UDP:
											data = SocketUDP.ReceiveBytes();
											break;
									}
									if (data == null)
									{
										Error = true;
										ErrorDescription = "デバイス種別異常";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
										break;
									}

									BL_Bytes.Join(data, ref ReceiveBytes, data.Length);

									byte[] receive_bytes = new byte[ReceiveBytes.Length];
									Array.Copy(ReceiveBytes, receive_bytes, ReceiveBytes.Length);

									if (isASCII)
									{
										#region ASCII->BINARY

										if (18 <= receive_bytes.Length)
										{
											byte[] d = new byte[receive_bytes.Length / 2];

											string s = "";
											s = Encoding.ASCII.GetString(receive_bytes, 0, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[0]);
											s = Encoding.ASCII.GetString(receive_bytes, 2, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[1]);
											s = Encoding.ASCII.GetString(receive_bytes, 4, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[2]);
											s = Encoding.ASCII.GetString(receive_bytes, 6, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[3]);
											s = Encoding.ASCII.GetString(receive_bytes, 8, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[4]);
											s = Encoding.ASCII.GetString(receive_bytes, 10, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[5]);
											s = Encoding.ASCII.GetString(receive_bytes, 12, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[6]);

											s = Encoding.ASCII.GetString(receive_bytes, 14, 4);
											int len = 0; int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out len);
											len = len / 2;
											d[7] = (byte)(len % 0x100);
											d[8] = (byte)(len / 0x100);

											int dpos = 9;
											for (int pos = 18; pos < receive_bytes.Length - 3; pos += 4)
											{
												s = Encoding.ASCII.GetString(receive_bytes, pos, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[dpos + 1]);
												s = Encoding.ASCII.GetString(receive_bytes, pos + 2, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[dpos]);
												dpos += 2;
											}

											receive_bytes = d;
										}
										else
										{
											if (3000 < m_swTemp.ElapsedMilliseconds)
											{
												Error = true;
												ErrorDescription = "PLC応答タイムアウト";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
												m_Step = 10;
											}
											break;
										}

										#endregion
									}

									while (11 <= receive_bytes.Length)
									{
										if (receive_bytes[0] == 0xD0 && receive_bytes[1] == 0x00)
										{
											break;
										}
										else
										{
											ReceiveBytes = BL_Bytes.Trim(receive_bytes, 1, receive_bytes.Length - 1);
										}
									}

									if (11 <= receive_bytes.Length)
									{
										int length = receive_bytes[7] + receive_bytes[8] * 0x100 + 9;

										if (length <= receive_bytes.Length)
										{
											data = BL_Bytes.Trim(receive_bytes, length);
											ReceiveBytes = new byte[0];

											if (data[9] == 0 && data[10] == 0)
											{
												int position = 11;
												byte[] d = BL_Bytes.Trim(data, position, v.dataLength * v.UnitSize);
												Array.Copy(d, v.data, v.dataLength * v.UnitSize);
												position += v.dataLength * v.UnitSize;

												if (v.negative) for (int i = 0; i < v.data.Length; i++) v.data[i] = (byte)~v.data[i];

												v.readed = true;

												//変化検出
												{
													if (v.data_pre == null)
													{
														v.data_pre = new byte[v.data.Length];
														for (int i = 0; i < v.data.Length; i++) v.data_pre[i] = (byte)(v.data[i] + 1);
													}

													//前回値と比較(.NET3.5にはSequenceEqualが無いため)
													bool diff = false;
													for (int i = 0; i < v.data.Length; i++)
													{
														if (v.data[i] != v.data_pre[i])
														{
															diff = true;
															break;
														}
													}

													//if (!v.data.SequenceEqual(v.data_pre))
													if (diff)
													{
														if (!v.noevent)
														{
															if (EventValueChanged != null) EventValueChanged(this, v);
														}

														Array.Copy(v.data, v.data_pre, v.data.Length);
														v.changed = true;
													}
												}

												r_worddevices.Remove(v);

												if (0 < r_worddevices.Count) m_Step = 300;
												else if (0 < queWrite.Count)
												{
													m_Step = 400;
                                                    if (!SupportRandomWrite) m_Step = 600;
												}
												else m_Step = 100;
											}
											else
											{
												Error = true;
												ErrorDescription = "PLC読込応答エラー[" + data[10].ToString("X2") + data[9].ToString("X2") + "]";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
												m_Step = 10;
											}
										}
									}
									else if (3000 < m_swTemp.ElapsedMilliseconds)
									{
										Error = true;
										ErrorDescription = "PLC応答タイムアウト";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
									}
								}
								#endregion
								break;

							case DeviceTypes.RS232C:
								#region RS232C
								{
									//byte[] data = Com.ReceiveBytesDirect();
									byte[] data = Com.ReceiveBytes();
									BL_Bytes.Join(data, ref ReceiveBytes, data.Length);

									if (ReceiveBytes.Length >= 8)
									{
										if (ReceiveBytes[4] != 0x4E || ReceiveBytes[5] != 0x4E)
										{
											int position = 4;

											{
												byte[] d = BL_Bytes.Trim(data, position, v.dataLength * v.UnitSize * 2);
												ReceiveBytes = new byte[0];

												int hl = 0;
												for (int pos = 0; pos < d.Length; pos += 2)
												{
													string wdata = "";
													wdata += (char)d[pos];
													wdata += (char)d[pos + 1];

													byte dd = byte.Parse(wdata, System.Globalization.NumberStyles.HexNumber);

													if (hl % 2 == 0) v.data[pos / 2 + 1] = dd;
													else v.data[pos / 2 - 1] = dd;

													hl++;
												}

												position += v.dataLength * v.UnitSize;

												if (v.negative) for (int i = 0; i < v.data.Length; i++) v.data[i] = (byte)~v.data[i];

												v.readed = true;

												//変化検出
												{
													if (v.data_pre == null)
													{
														v.data_pre = new byte[v.data.Length];
														for (int i = 0; i < v.data.Length; i++) v.data_pre[i] = (byte)(v.data[i] + 1);
													}

													//前回値と比較(.NET3.5にはSequenceEqualが無いため)
													bool diff = false;
													for (int i = 0; i < v.data.Length; i++)
													{
														if (v.data[i] != v.data_pre[i])
														{
															diff = true;
															break;
														}
													}

													//if (!v.data.SequenceEqual(v.data_pre))
													if (diff)
													{
														if (!v.noevent)
														{
															if (EventValueChanged != null) EventValueChanged(this, v);
														}

														Array.Copy(v.data, v.data_pre, v.data.Length);
														v.changed = true;
													}
												}
											}
											r_worddevices.Remove(v);

											if (0 < r_worddevices.Count) m_Step = 300;
											else if (0 < queWrite.Count)
											{
												m_Step = 400;
                                                if (!SupportRandomWrite) m_Step = 600;
											}
											else m_Step = 100;
										}
										else
										{
											Error = true;
											ErrorDescription = "PLC読込応答エラー[" + ReceiveBytes[7].ToString("X2") + ReceiveBytes[6].ToString("X2") + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											m_Step = 10;
										}
									}
									else if (3000 < m_swTemp.ElapsedMilliseconds)
									{
										Error = true;
										ErrorDescription = "PLC応答タイムアウト";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
									}
								}
								#endregion
								break;
						}
					}
					#endregion
					break;



				case 400:
					#region BIT書き込み要求コマンド送信(BITランダム書き込み)
					{
						swWrite.Restart();

						w_bitdevices.Clear();
						w_worddevices.Clear();

						int size = 0;

						lock (queWrite)
						{
							while (0 < queWrite.Count)
							{
								BL_MCProtocol.DeviceInfo v = queWrite.Peek();

								if (size + v.DeviceSize <= 160)
								{
									queWrite.Dequeue();

									if (v.deviceKind == DeviceInfo.DeviceKind.BIT)
									{
										w_bitdevices.Add(v);
										size += v.DeviceSize;
									}
									if (v.deviceKind == DeviceInfo.DeviceKind.WORD)
									{
										w_worddevices.Add(v);
										size += v.DeviceSize;
									}
								}
								else break;
							}
						}

						if (w_bitdevices.Count == 0 && w_worddevices.Count == 0)
						{
							//書き込み要求が無ければ読み込み要求処理へ
							m_Step = 100;
							break;
						}
						else if (w_bitdevices.Count == 0 && 0 < w_worddevices.Count)
						{
							//BIT書き込み要求が無ければWORD書き込み要求処理へ
							m_Step = 500;
							break;
						}

						List<byte> data = new List<byte>();
						int datalength = 0;

						int bitcount = 0;
						foreach (var dev in w_bitdevices) bitcount += dev.dataLength;

						switch (DeviceType)
						{
							case DeviceTypes.TCP:
							case DeviceTypes.UDP:
								#region TCP/UDP
								{
									if (isASCII)
									{
										#region ASCII
										
										string d = "";
										d += "5000";	//サブヘッダ
										d += "00";		//ネットワーク番号
										d += "FF";		//ＰＣ番号
										d += "03FF";	//要求先ユニットＩ／Ｏ番号
										d += "00";		//要求先ユニット局番号
										d += "@len";	//要求データ長
										d += "0000";	//ＣＰＵ監視タイマ

										d += "1402";	//コマンド
										d += "0001";	//サブコマンド

										d += bitcount.ToString("X2");

										foreach (var dev in w_bitdevices)
										{
											for (int bit = 0; bit < dev.dataLength; bit++)
											{
												int address = dev.deviceAddress + bit;
												if (dev.deviceAddressSequence == DeviceInfo.DeviceAddressSequence.FLOWBY16)
												{
													address = dev.deviceAddress + (bit / 16) * 100 + bit % 16;
													address = address / 100 * 16 + address % 100;
												}

												address = KV5000 ? address % 100000 : address;

												d += dev.deviceCode.ToString().PadRight(2, '*');
												if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC) d += address.ToString("D6");
												else d += address.ToString("X6");

												if (dev.negative)
												{
													d += dev.data[bit] != 0 ? "00" : "01";
												}
												else
												{
													d += dev.data[bit] != 0 ? "01" : "00";
												}
											}

											dev.writing = true;
											dev.writed = false;
										}

										d = d.Replace("@len", (d.Length - 18).ToString("X4"));

										byte[] dd = Encoding.ASCII.GetBytes(d);
										data.AddRange(dd);

										#endregion
									}
									else
									{
										#region BINARY
										
										data.Add(0x50); //サブヘッダ
										data.Add(0x00); //サブヘッダ
										data.Add(0x00); //ネットワーク番号
										data.Add(0xFF); //ＰＣ番号
										data.Add(0xFF); //要求先ユニットＩ／Ｏ番号
										data.Add(0x03); //要求先ユニットＩ／Ｏ番号
										data.Add(0x00); //要求先ユニット局番号
										data.Add(0); //7:要求データ長
										data.Add(0); //8:要求データ長

										data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ
										data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ

										data.Add(0x02); datalength++;   //コマンド
										data.Add(0x14); datalength++;   //コマンド
										data.Add(0x01); datalength++;   //サブコマンド
										data.Add(0x00); datalength++;   //サブコマンド

										data.Add((byte)bitcount); datalength++;    //ビットデバイス点数

										foreach (var dev in w_bitdevices)
										{
											for (int bit = 0; bit < dev.dataLength; bit++)
											{
												int address = dev.deviceAddress + bit;
												if (dev.deviceAddressSequence == DeviceInfo.DeviceAddressSequence.FLOWBY16)
												{
													address = dev.deviceAddress + (bit / 16) * 100 + bit % 16;
													address = address / 100 * 16 + address % 100;
												}

												address = KV5000 ? address % 100000 : address;

												data.Add((byte)(address & 0x000000FF)); datalength++;
												data.Add((byte)((address >> 8) & 0x000000FF)); datalength++;
												data.Add((byte)((address >> 16) & 0x000000FF)); datalength++;
												data.Add((byte)dev.deviceCode); datalength++;

												if (dev.negative)
												{
													data.Add((byte)(dev.data[bit] != 0 ? 0 : 1)); datalength++;
												}
												else
												{
													data.Add((byte)(dev.data[bit] != 0 ? 1 : 0)); datalength++;
												}
											}

											dev.writing = true;
											dev.writed = false;
										}

										data[7] = (byte)(datalength & 0x000000FF);
										data[8] = (byte)((datalength >> 8) & 0x000000FF);

										#endregion
									}

									ReceiveBytes = new byte[0];
									if (DeviceType == DeviceTypes.TCP)
									{
										if (!Socket.SendBytes(data.ToArray()))
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC送信エラー[" + Socket.ErrorMessage + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											return true;
										}
									}
									else if (DeviceType == DeviceTypes.UDP)
									{
										if (!SocketUDP.SendBytes(data.ToArray(), ipaddressUDP))
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC送信エラー[" + SocketUDP.ErrorMessage + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											return true;
										}
									}
								}
								#endregion
								break;

							case DeviceTypes.RS232C:
								#region RS232C
								{
									switch (CommandType)
									{
										case CommandTypes.MELSEC_A_CPU:
											{
												data.AddRange(Encoding.Default.GetBytes("00FFBT0"));
												data.AddRange(Encoding.Default.GetBytes((bitcount & 0xFF).ToString("X2")));

												foreach (var dev in w_bitdevices)
												{
													for (int pos = 0; pos < dev.dataLength; pos++)
													{
														data.Add((byte)dev.GetDeviceCode()[0]);
														if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.HEX)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("X4")));
														}
														else if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("0000")));
														}
														data.Add((byte)(dev.data[pos] == 0 ? 0x30 : 0x31));
													}
												}
											}
											break;

										case CommandTypes.MELSEC_AnA_CPU:
											{
												data.AddRange(Encoding.Default.GetBytes("00FFJT0"));
												data.AddRange(Encoding.Default.GetBytes((bitcount & 0xFF).ToString("X2")));

												foreach (var dev in w_bitdevices)
												{
													for (int pos = 0; pos < dev.dataLength; pos++)
													{
														data.Add((byte)dev.GetDeviceCode()[0]);
														if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.HEX)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("X6")));
														}
														else if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("000000")));
														}
														data.Add((byte)(dev.data[pos] == 0 ? 0x30 : 0x31));
													}
												}
											}
											break;
									}

									ReceiveBytes = new byte[0];
									if (!Com.SendBytes(data.ToArray()))
									{
										m_Step = 10;
										Error = true;
										ErrorDescription = "PLC送信エラー[" + Com.ErrorMessage + "]";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										return true;
									}
								}
								#endregion
								break;
						}

						m_Step = 410;
						m_swTemp.Restart();

						data.Clear();
						data = null;
					}
					#endregion
					break;

				case 410:
					#region BIT書き込み応答コマンド受信(BITランダム書き込み)
					{
						switch (DeviceType)
						{
							case DeviceTypes.TCP:
							case DeviceTypes.UDP:
								#region TCP/UDP
								{
									byte[] data = null;
									switch (DeviceType)
									{
										case DeviceTypes.TCP:
											data = Socket.ReceiveBytes();
											break;
										case DeviceTypes.UDP:
											data = SocketUDP.ReceiveBytes();
											break;
									}
									if (data == null)
									{
										Error = true;
										ErrorDescription = "デバイス種別異常";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
										break;
									}

									BL_Bytes.Join(data, ref ReceiveBytes, data.Length);

									byte[] receive_bytes = new byte[ReceiveBytes.Length];
									Array.Copy(ReceiveBytes, receive_bytes, ReceiveBytes.Length);

									if (isASCII)
									{
										#region ASCII->BINARY

										if (18 <= receive_bytes.Length)
										{
											byte[] d = new byte[receive_bytes.Length / 2];

											string s = "";
											s = Encoding.ASCII.GetString(receive_bytes, 0, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[0]);
											s = Encoding.ASCII.GetString(receive_bytes, 2, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[1]);
											s = Encoding.ASCII.GetString(receive_bytes, 4, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[2]);
											s = Encoding.ASCII.GetString(receive_bytes, 6, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[3]);
											s = Encoding.ASCII.GetString(receive_bytes, 8, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[4]);
											s = Encoding.ASCII.GetString(receive_bytes, 10, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[5]);
											s = Encoding.ASCII.GetString(receive_bytes, 12, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[6]);

											s = Encoding.ASCII.GetString(receive_bytes, 14, 4);
											int len = 0; int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out len);
											len = len / 2;
											d[7] = (byte)(len % 0x100);
											d[8] = (byte)(len / 0x100);

											receive_bytes = d;
										}
										else
										{
											if (3000 < m_swTemp.ElapsedMilliseconds)
											{
												Error = true;
												ErrorDescription = "PLC応答タイムアウト";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
												m_Step = 10;
											}
											break;
										}

										#endregion
									}

									while (11 <= receive_bytes.Length)
									{
										if (receive_bytes[0] == 0xD0 && receive_bytes[1] == 0x00)
										{
											break;
										}
										else
										{
											receive_bytes = BL_Bytes.Trim(receive_bytes, 1, receive_bytes.Length - 1);
										}
									}

									if (11 <= receive_bytes.Length)
									{
										int length = receive_bytes[7] + receive_bytes[8] * 0x100 + 9;

										if (length <= receive_bytes.Length)
										{
											data = BL_Bytes.Trim(receive_bytes, length);
											ReceiveBytes = new byte[0];

											if (data[9] == 0 && data[10] == 0)
											{
												lock (write_eventdevices)
												{
													for (int i = 0; i < write_eventdevices.Count; i++)
													{
														if (write_eventdevices[i].writing)
														{
															write_eventdevices[i].writing = false;
															write_eventdevices[i].writed = true;
															break;
														}
													}
												}

												//WORD書き込み要求があればWORD書き込みへ
												if (0 < w_worddevices.Count)
												{
													m_Step = 500;
												}
												else
												{
													m_Step = 100;
												}

												if (swWrite.IsRunning) if (0 < swWrite.ElapsedMilliseconds) write_scantimes = swWrite.ElapsedMilliseconds;
											}
											else
											{
												m_Step = 10;
												Error = true;
												ErrorDescription = "PLC書込応答エラー[" + data[10].ToString("X2") + data[9].ToString("X2") + "]";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											}
										}
									}
									else if (3000 < m_swTemp.ElapsedMilliseconds)
									{
										Error = true;
										ErrorDescription = "PLC応答タイムアウト";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
									}
								}
								#endregion
								break;

							case DeviceTypes.RS232C:
								#region RS232C
								{
									byte[] data = Com.ReceiveBytes();
									BL_Bytes.Join(data, ref ReceiveBytes, data.Length);

									if (ReceiveBytes.Length >= 6)
									{
										if (ReceiveBytes[4] == 0x47 || ReceiveBytes[5] == 0x47)
										{
											ReceiveBytes = new byte[0];

											lock (write_eventdevices)
											{
												for (int i = 0; i < write_eventdevices.Count; i++)
												{
													if (write_eventdevices[i].writing)
													{
														write_eventdevices[i].writing = false;
														write_eventdevices[i].writed = true;
														break;
													}
												}
											}

											//WORD書き込み要求があればWORD書き込みへ
											if (0 < w_worddevices.Count)
											{
												m_Step = 500;
											}
											else
											{
												m_Step = 100;
											}

											if (swWrite.IsRunning) if (0 < swWrite.ElapsedMilliseconds) write_scantimes = swWrite.ElapsedMilliseconds;
										}
										else
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC書込応答エラー[" + ReceiveBytes[7].ToString("X2") + data[6].ToString("X2") + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										}
									}
									else if (3000 < m_swTemp.ElapsedMilliseconds)
									{
										Error = true;
										ErrorDescription = "PLC応答タイムアウト";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
									}
								}
								#endregion
								break;
						}
					}
					#endregion
					break;


				case 500:
					#region WORD書き込み要求コマンド送信(WORDランダム書き込み)
					{
						swWrite.Restart();

						int word_size = 0;
                        foreach (var dev in w_worddevices)
                        {
                            word_size += dev.DeviceSize;
                        }

                        //ランダム書込では160ワードまで
                        if (160 < word_size)
                        {
                            m_Step = 700;
                            break;
                        }

						List<byte> data = new List<byte>();
						int datalength = 0;

						switch (DeviceType)
						{
							case DeviceTypes.TCP:
							case DeviceTypes.UDP:
								#region TCP/UDP
								{
									if (isASCII)
									{
										#region ASCII

										string d = "";
										d += "5000";	//サブヘッダ
										d += "00";		//ネットワーク番号
										d += "FF";		//ＰＣ番号
										d += "03FF";	//要求先ユニットＩ／Ｏ番号
										d += "00";		//要求先ユニット局番号
										d += "@len";	//要求データ長
										d += "0000";	//ＣＰＵ監視タイマ

										d += "1402";	//コマンド
										d += "0000";	//サブコマンド

										d += word_size.ToString("X2");	//WORD書き込み点数
										d += "00";						//DWORD書き込み点数

										foreach (var dev in w_worddevices)
										{
											for (int i = 0; i < dev.DeviceSize; i++)
											{
												int address = dev.deviceAddress + i;

												address = KV5000 ? address % 100000 : address;

												d += dev.deviceCode.ToString().PadRight(2, '*');
												if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC) d += address.ToString("D6");
												else d += address.ToString("X6");

												if (dev.negative)
												{
													d += (~dev.data[i * 2 + 1]).ToString("X2");
													d += (~dev.data[i * 2]).ToString("X2");
												}
												else
												{
													d += (dev.data[i * 2 + 1]).ToString("X2");
													d += (dev.data[i * 2]).ToString("X2");
												}
											}

											dev.writing = true;
											dev.writed = false;
										}

										d = d.Replace("@len", (d.Length - 18).ToString("X4"));
										byte[] dd = Encoding.ASCII.GetBytes(d);
										data.AddRange(dd);

										#endregion
									}
									else
									{
										#region BINARY
										
										data.Add(0x50); //サブヘッダ
										data.Add(0x00); //サブヘッダ
										data.Add(0x00); //ネットワーク番号
										data.Add(0xFF); //ＰＣ番号
										data.Add(0xFF); //要求先ユニットＩ／Ｏ番号
										data.Add(0x03); //要求先ユニットＩ／Ｏ番号
										data.Add(0x00); //要求先ユニット局番号
										data.Add(0); //7:要求データ長
										data.Add(0); //8:要求データ長

										data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ
										data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ

										data.Add(0x02); datalength++;   //コマンド
										data.Add(0x14); datalength++;   //コマンド
										data.Add(0x00); datalength++;   //サブコマンド
										data.Add(0x00); datalength++;   //サブコマンド

										data.Add((byte)word_size); datalength++; //WORD書き込み点数
										data.Add(0x00); datalength++;            //DWORD書き込み点数

										foreach (var dev in w_worddevices)
										{
											for (int i = 0; i < dev.DeviceSize; i++)
											{
												int address = dev.deviceAddress + i;

												address = KV5000 ? address % 100000 : address;

												data.Add((byte)(address & 0x000000FF)); datalength++;
												data.Add((byte)((address >> 8) & 0x000000FF)); datalength++;
												data.Add((byte)((address >> 16) & 0x000000FF)); datalength++;
												data.Add((byte)dev.deviceCode); datalength++;

												if (dev.negative)
												{
													data.Add((byte)~dev.data[i * 2]); datalength++;
													data.Add((byte)~dev.data[i * 2 + 1]); datalength++;
												}
												else
												{
													data.Add(dev.data[i * 2]); datalength++;
													data.Add(dev.data[i * 2 + 1]); datalength++;
												}
											}

											dev.writing = true;
											dev.writed = false;
										}

										data[7] = (byte)(datalength & 0x000000FF);
										data[8] = (byte)((datalength >> 8) & 0x000000FF);

										#endregion
									}

									ReceiveBytes = new byte[0];
									if (DeviceType == DeviceTypes.TCP)
									{
										if (!Socket.SendBytes(data.ToArray()))
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC送信エラー[" + Socket.ErrorMessage + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											return true;
										}
									}
									else if (DeviceType == DeviceTypes.UDP)
									{
										if (!SocketUDP.SendBytes(data.ToArray(), ipaddressUDP))
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC送信エラー[" + SocketUDP.ErrorMessage + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											return true;
										}
									}
								}
								#endregion
								break;

							case DeviceTypes.RS232C:
								#region RS232C
								{
									switch (CommandType)
									{
										case CommandTypes.MELSEC_A_CPU:
											{
												data.AddRange(Encoding.Default.GetBytes("00FFWT0"));
												data.AddRange(Encoding.Default.GetBytes((word_size & 0xFF).ToString("X2")));

												foreach (var dev in w_worddevices)
												{
													for (int pos = 0; pos < dev.DeviceSize; pos++)
													{
														data.Add((byte)dev.GetDeviceCode()[0]);
														if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.HEX)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("X4")));
														}
														else if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("0000")));
														}

														if (pos * 2 + 1 < dev.data.Length)
														{
															data.AddRange(Encoding.Default.GetBytes(dev.data[pos * 2 + 1].ToString("X2")));
														}
														else
														{
															data.AddRange(Encoding.Default.GetBytes("00"));
														}

														data.AddRange(Encoding.Default.GetBytes(dev.data[pos * 2].ToString("X2")));
													}
												}
											}
											break;

										case CommandTypes.MELSEC_AnA_CPU:
											{
												data.AddRange(Encoding.Default.GetBytes("00FFQT0"));
												data.AddRange(Encoding.Default.GetBytes((word_size & 0xFF).ToString("X2")));

												foreach (var dev in w_worddevices)
												{
													for (int pos = 0; pos < dev.DeviceSize; pos++)
													{
														data.Add((byte)dev.GetDeviceCode()[0]);
														if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.HEX)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("X6")));
														}
														else if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("000000")));
														}

														if (pos * 2 + 1 < dev.data.Length)
														{
															data.AddRange(Encoding.Default.GetBytes(dev.data[pos * 2 + 1].ToString("X2")));
														}
														else
														{
															data.AddRange(Encoding.Default.GetBytes("00"));
														}

														data.AddRange(Encoding.Default.GetBytes(dev.data[pos * 2].ToString("X2")));
													}
												}
											}
											break;
									}

									ReceiveBytes = new byte[0];
									if (!Com.SendBytes(data.ToArray()))
									{
										m_Step = 10;
										Error = true;
										ErrorDescription = "PLC送信エラー[" + Com.ErrorMessage + "]";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										return true;
									}
								}
								#endregion
								break;
						}

						m_Step = 510;
						m_swTemp.Restart();

						data.Clear();
						data = null;
					}
					#endregion
					break;

				case 510:
					#region WORD書き込み応答コマンド受信(WORDランダム書き込み)
					{
						switch (DeviceType)
						{
							case DeviceTypes.TCP:
							case DeviceTypes.UDP:
								#region TCP/UDP
								{
									byte[] data = null;
									switch (DeviceType)
									{
										case DeviceTypes.TCP:
											data = Socket.ReceiveBytes();
											break;
										case DeviceTypes.UDP:
											data = SocketUDP.ReceiveBytes();
											break;
									}
									if (data == null)
									{
										Error = true;
										ErrorDescription = "デバイス種別異常";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
										break;
									}
									BL_Bytes.Join(data, ref ReceiveBytes, data.Length);
									byte[] receive_bytes = new byte[ReceiveBytes.Length];
									Array.Copy(ReceiveBytes, receive_bytes, ReceiveBytes.Length);

									if (isASCII)
									{
										#region ASCII->BINARY

										if (18 <= receive_bytes.Length)
										{
											byte[] d = new byte[receive_bytes.Length / 2];

											string s = "";
											s = Encoding.ASCII.GetString(receive_bytes, 0, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[0]);
											s = Encoding.ASCII.GetString(receive_bytes, 2, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[1]);
											s = Encoding.ASCII.GetString(receive_bytes, 4, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[2]);
											s = Encoding.ASCII.GetString(receive_bytes, 6, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[3]);
											s = Encoding.ASCII.GetString(receive_bytes, 8, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[4]);
											s = Encoding.ASCII.GetString(receive_bytes, 10, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[5]);
											s = Encoding.ASCII.GetString(receive_bytes, 12, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[6]);

											s = Encoding.ASCII.GetString(receive_bytes, 14, 4);
											int len = 0; int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out len);
											len = len / 2;
											d[7] = (byte)(len % 0x100);
											d[8] = (byte)(len / 0x100);

											receive_bytes = d;
										}
										else
										{
											if (3000 < m_swTemp.ElapsedMilliseconds)
											{
												Error = true;
												ErrorDescription = "PLC応答タイムアウト";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
												m_Step = 10;
											}
											break;
										}

										#endregion
									}

									while (11 <= receive_bytes.Length)
									{
										if (receive_bytes[0] == 0xD0 && receive_bytes[1] == 0x00)
										{
											break;
										}
										else
										{
											receive_bytes = BL_Bytes.Trim(receive_bytes, 1, receive_bytes.Length - 1);
										}
									}

									if (11 <= receive_bytes.Length)
									{
										int length = receive_bytes[7] + receive_bytes[8] * 0x100 + 9;

										if (length <= receive_bytes.Length)
										{
											data = BL_Bytes.Trim(receive_bytes, length);
											ReceiveBytes = new byte[0];

											if (data[9] == 0 && data[10] == 0)
											{
												lock (write_eventdevices)
												{
													for (int i = 0; i < write_eventdevices.Count; i++)
													{
														if (write_eventdevices[i].writing)
														{
															write_eventdevices[i].writing = false;
															write_eventdevices[i].writed = true;
															break;
														}
													}
												}

												//読み込み要求処理へ
												m_Step = 100;
												if (swWrite.IsRunning) if (0 < swWrite.ElapsedMilliseconds) write_scantimes = swWrite.ElapsedMilliseconds;
											}
											else
											{
												m_Step = 10;
												Error = true;
												ErrorDescription = "PLC書込応答エラー[" + data[10].ToString("X2") + data[9].ToString("X2") + "]";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											}
										}
									}
									else if (3000 < m_swTemp.ElapsedMilliseconds)
									{
										Error = true;
										ErrorDescription = "PLC応答タイムアウト";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
									}
								}
								#endregion
								break;

							case DeviceTypes.RS232C:
								#region RS232C
								{
									byte[] data = Com.ReceiveBytes();
									BL_Bytes.Join(data, ref ReceiveBytes, data.Length);

									if (ReceiveBytes.Length >= 6)
									{
										if (ReceiveBytes[4] == 0x47 || ReceiveBytes[5] == 0x47)
										{
											ReceiveBytes = new byte[0];

											lock (write_eventdevices)
											{
												for (int i = 0; i < write_eventdevices.Count; i++)
												{
													if (write_eventdevices[i].writing)
													{
														write_eventdevices[i].writing = false;
														write_eventdevices[i].writed = true;
														break;
													}
												}
											}

											//読み込み要求処理へ
											m_Step = 100;
											if (swWrite.IsRunning) if (0 < swWrite.ElapsedMilliseconds) write_scantimes = swWrite.ElapsedMilliseconds;
										}
										else
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC書込応答エラー[" + ReceiveBytes[7].ToString("X2") + ReceiveBytes[6].ToString("X2") + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										}
									}
									else if (3000 < m_swTemp.ElapsedMilliseconds)
									{
										Error = true;
										ErrorDescription = "PLC応答タイムアウト";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
									}
								}
								#endregion
								break;
						}
					}
					#endregion
					break;


				case 600:
					#region BIT書き込み要求コマンド送信(BIT一括書き込み)
					{
						swWrite.Restart();

						w_bitdevices.Clear();
						w_worddevices.Clear();

						lock (queWrite)
						{
							while (0 < queWrite.Count)
							{
								BL_MCProtocol.DeviceInfo v = queWrite.Dequeue();

								if (v.deviceKind == DeviceInfo.DeviceKind.BIT) w_bitdevices.Add(v);
								if (v.deviceKind == DeviceInfo.DeviceKind.WORD) w_worddevices.Add(v);
								break;
							}
						}

						if (w_bitdevices.Count == 0 && w_worddevices.Count == 0)
						{
							//書き込み要求が無ければ読み込み要求処理へ
							m_Step = 100;
							break;
						}
						else if (w_bitdevices.Count == 0 && 0 < w_worddevices.Count)
						{
							//BIT書き込み要求が無ければWORD書き込み要求処理へ
							m_Step = 700;
							break;
						}

						List<byte> data = new List<byte>();
						int datalength = 0;

						int bitcount = 0;
						foreach (var dev in w_bitdevices) bitcount += dev.dataLength;

						switch (DeviceType)
						{
							case DeviceTypes.TCP:
							case DeviceTypes.UDP:
								#region TCP/UDP
								{
									if (isASCII)
									{
										string d = "";
										d += "5000";	//サブヘッダ
										d += "00";		//ネットワーク番号
										d += "FF";		//ＰＣ番号
										d += "03FF";	//要求先ユニットＩ／Ｏ番号
										d += "00";		//要求先ユニット局番号
										d += "@len";	//要求データ長
										d += "0000";	//ＣＰＵ監視タイマ

										d += "1401";	//コマンド
										d += "0001";	//サブコマンド

										foreach (var dev in w_bitdevices)
										{
											int address = dev.deviceAddress;
											if (dev.deviceAddressSequence == DeviceInfo.DeviceAddressSequence.FLOWBY16)
											{
												address = address / 100 * 16 + address % 100;
											}

											address = KV5000 ? address % 100000 : address;

											d += dev.deviceCode.ToString().PadRight(2, '*');
											if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC) d += address.ToString("D6");
											else d += address.ToString("X6");

											d += bitcount.ToString("X4");

											for (int bit = 0; bit < dev.dataLength; bit++)
											{
												if (dev.negative)
												{
													d += dev.data[bit] != 0 ? "0" : "1";
												}
												else
												{
													d += dev.data[bit] != 0 ? "1" : "0";
												}
											}

											dev.writing = true;
											dev.writed = false;
										}

										d = d.Replace("@len", (d.Length - 18).ToString("X4"));
										byte[] dd = Encoding.ASCII.GetBytes(d);
										data.AddRange(dd);
									}
									else
									{
										#region BINARY
										
										data.Add(0x50); //サブヘッダ
										data.Add(0x00); //サブヘッダ
										data.Add(0x00); //ネットワーク番号
										data.Add(0xFF); //ＰＣ番号
										data.Add(0xFF); //要求先ユニットＩ／Ｏ番号
										data.Add(0x03); //要求先ユニットＩ／Ｏ番号
										data.Add(0x00); //要求先ユニット局番号
										data.Add(0); //7:要求データ長
										data.Add(0); //8:要求データ長

										data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ
										data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ

										data.Add(0x01); datalength++;   //コマンド
										data.Add(0x14); datalength++;   //コマンド
										data.Add(0x01); datalength++;   //サブコマンド
										data.Add(0x00); datalength++;   //サブコマンド

										data.Add((byte)bitcount); datalength++;    //ビットデバイス点数

										foreach (var dev in w_bitdevices)
										{
											for (int bit = 0; bit < dev.dataLength; bit++)
											{
												int address = dev.deviceAddress + bit;
												if (dev.deviceAddressSequence == DeviceInfo.DeviceAddressSequence.FLOWBY16)
												{
													address = dev.deviceAddress + (bit / 16) * 100 + bit % 16;
													address = address / 100 * 16 + address % 100;

													//address = 0;
													//for (int i = 0; i < dev.deviceAddress + bit; i++, address++)
													//{
													//	if (i % 100 == 15) i += 84;
													//}
												}

												address = KV5000 ? address % 100000 : address;

												data.Add((byte)(address & 0x000000FF)); datalength++;
												data.Add((byte)((address >> 8) & 0x000000FF)); datalength++;
												data.Add((byte)((address >> 16) & 0x000000FF)); datalength++;
												data.Add((byte)dev.deviceCode); datalength++;

												if (dev.negative)
												{
													data.Add((byte)(dev.data[bit] != 0 ? 0 : 1)); datalength++;
												}
												else
												{
													data.Add((byte)(dev.data[bit] != 0 ? 1 : 0)); datalength++;
												}
											}

											dev.writing = true;
											dev.writed = false;
										}

										data[7] = (byte)(datalength & 0x000000FF);
										data[8] = (byte)((datalength >> 8) & 0x000000FF);

										#endregion
									}

									ReceiveBytes = new byte[0];
									if (DeviceType == DeviceTypes.TCP)
									{
										if (!Socket.SendBytes(data.ToArray()))
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC送信エラー[" + Socket.ErrorMessage + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											return true;
										}
									}
									else if (DeviceType == DeviceTypes.UDP)
									{
										if (!SocketUDP.SendBytes(data.ToArray(), ipaddressUDP))
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC送信エラー[" + SocketUDP.ErrorMessage + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											return true;
										}
									}
								}
								#endregion
								break;

							case DeviceTypes.RS232C:
								#region RS232C
								{
									switch (CommandType)
									{
										case CommandTypes.MELSEC_A_CPU:
											{
												data.AddRange(Encoding.Default.GetBytes("00FFBT0"));
												data.AddRange(Encoding.Default.GetBytes((bitcount & 0xFF).ToString("X2")));

												foreach (var dev in w_bitdevices)
												{
													for (int pos = 0; pos < dev.dataLength; pos++)
													{
														data.Add((byte)dev.GetDeviceCode()[0]);
														if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.HEX)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("X4")));
														}
														else if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("0000")));
														}
														data.Add((byte)(dev.data[pos] == 0 ? 0x30 : 0x31));
													}
												}
											}
											break;

										case CommandTypes.MELSEC_AnA_CPU:
											{
												data.AddRange(Encoding.Default.GetBytes("00FFJT0"));
												data.AddRange(Encoding.Default.GetBytes((bitcount & 0xFF).ToString("X2")));

												foreach (var dev in w_bitdevices)
												{
													for (int pos = 0; pos < dev.dataLength; pos++)
													{
														data.Add((byte)dev.GetDeviceCode()[0]);
														if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.HEX)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("X6")));
														}
														else if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("000000")));
														}
														data.Add((byte)(dev.data[pos] == 0 ? 0x30 : 0x31));
													}
												}
											}
											break;
									}

									ReceiveBytes = new byte[0];
									if (!Com.SendBytes(data.ToArray()))
									{
										m_Step = 10;
										Error = true;
										ErrorDescription = "PLC送信エラー[" + Com.ErrorMessage + "]";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										return true;
									}
								}
								#endregion
								break;
						}

						m_Step = 610;
						m_swTemp.Restart();

						data.Clear();
						data = null;
					}
					#endregion
					break;

				case 610:
					#region BIT書き込み応答コマンド受信(BIT一括書き込み)
					{
						switch (DeviceType)
						{
							case DeviceTypes.TCP:
							case DeviceTypes.UDP:
								#region TCP/UDP
								{
									byte[] data = null;
									switch (DeviceType)
									{
										case DeviceTypes.TCP:
											data = Socket.ReceiveBytes();
											break;
										case DeviceTypes.UDP:
											data = SocketUDP.ReceiveBytes();
											break;
									}
									if (data == null)
									{
										Error = true;
										ErrorDescription = "デバイス種別異常";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
										break;
									}

									BL_Bytes.Join(data, ref ReceiveBytes, data.Length);

									byte[] receive_bytes = new byte[ReceiveBytes.Length];
									Array.Copy(ReceiveBytes, receive_bytes, ReceiveBytes.Length);

									if (isASCII)
									{
										#region ASCII->BINARY

										if (18 <= receive_bytes.Length)
										{
											byte[] d = new byte[receive_bytes.Length / 2];

											string s = "";
											s = Encoding.ASCII.GetString(receive_bytes, 0, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[0]);
											s = Encoding.ASCII.GetString(receive_bytes, 2, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[1]);
											s = Encoding.ASCII.GetString(receive_bytes, 4, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[2]);
											s = Encoding.ASCII.GetString(receive_bytes, 6, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[3]);
											s = Encoding.ASCII.GetString(receive_bytes, 8, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[4]);
											s = Encoding.ASCII.GetString(receive_bytes, 10, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[5]);
											s = Encoding.ASCII.GetString(receive_bytes, 12, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[6]);

											s = Encoding.ASCII.GetString(receive_bytes, 14, 4);
											int len = 0; int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out len);
											len = len / 2;
											d[7] = (byte)(len % 0x100);
											d[8] = (byte)(len / 0x100);

											receive_bytes = d;
										}
										else
										{
											if (3000 < m_swTemp.ElapsedMilliseconds)
											{
												Error = true;
												ErrorDescription = "PLC応答タイムアウト";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
												m_Step = 10;
											}
											break;
										}

										#endregion
									}

									while (11 <= receive_bytes.Length)
									{
										if (receive_bytes[0] == 0xD0 && receive_bytes[1] == 0x00)
										{
											break;
										}
										else
										{
											receive_bytes = BL_Bytes.Trim(receive_bytes, 1, ReceiveBytes.Length - 1);
										}
									}

									if (11 <= receive_bytes.Length)
									{
										int length = receive_bytes[7] + receive_bytes[8] * 0x100 + 9;

										if (length <= receive_bytes.Length)
										{
											data = BL_Bytes.Trim(receive_bytes, length);
											ReceiveBytes = new byte[0];

											if (data[9] == 0 && data[10] == 0)
											{
												lock (write_eventdevices)
												{
													for (int i = 0; i < write_eventdevices.Count; i++)
													{
														if (write_eventdevices[i].writing)
														{
															write_eventdevices[i].writing = false;
															write_eventdevices[i].writed = true;
															break;
														}
													}
												}

												//WORD書き込み要求があればWORD書き込みへ
												if (0 < w_worddevices.Count)
												{
													m_Step = 700;
												}
												else
												{
													m_Step = 100;
												}

												if (swWrite.IsRunning) if (0 < swWrite.ElapsedMilliseconds) write_scantimes = swWrite.ElapsedMilliseconds;
											}
											else
											{
												m_Step = 10;
												Error = true;
												ErrorDescription = "PLC書込応答エラー[" + data[10].ToString("X2") + data[9].ToString("X2") + "]";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											}
										}
									}
									else if (3000 < m_swTemp.ElapsedMilliseconds)
									{
										Error = true;
										ErrorDescription = "PLC応答タイムアウト";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
									}

									receive_bytes = null;
								}
								#endregion
								break;

							case DeviceTypes.RS232C:
								#region RS232C
								{
									byte[] data = Com.ReceiveBytes();
									BL_Bytes.Join(data, ref ReceiveBytes, data.Length);

									if (ReceiveBytes.Length >= 6)
									{
										if (ReceiveBytes[4] == 0x47 || ReceiveBytes[5] == 0x47)
										{
											ReceiveBytes = new byte[0];

											lock (write_eventdevices)
											{
												for (int i = 0; i < write_eventdevices.Count; i++)
												{
													if (write_eventdevices[i].writing)
													{
														write_eventdevices[i].writing = false;
														write_eventdevices[i].writed = true;
														break;
													}
												}
											}

											//WORD書き込み要求があればWORD書き込みへ
											if (0 < w_worddevices.Count)
											{
												m_Step = 500;
											}
											else
											{
												m_Step = 100;
											}

											if (swWrite.IsRunning) if (0 < swWrite.ElapsedMilliseconds) write_scantimes = swWrite.ElapsedMilliseconds;
										}
										else
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC書込応答エラー[" + ReceiveBytes[7].ToString("X2") + data[6].ToString("X2") + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										}
									}
									else if (3000 < m_swTemp.ElapsedMilliseconds)
									{
										Error = true;
										ErrorDescription = "PLC応答タイムアウト";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
									}
								}
								#endregion
								break;
						}
					}
					#endregion
					break;


				case 700:
					#region WORD書き込み要求コマンド送信(WORD一括書き込み)
					{
						swWrite.Restart();

						int word_size = 0;
						foreach (var dev in w_worddevices) word_size += dev.DeviceSize;

						List<byte> data = new List<byte>();
						int datalength = 0;

						switch (DeviceType)
						{
							case DeviceTypes.TCP:
							case DeviceTypes.UDP:
								#region TCP/UDP
								{
									if (isASCII)
									{
										string d = "";
										d += "5000";	//サブヘッダ
										d += "00";		//ネットワーク番号
										d += "FF";		//ＰＣ番号
										d += "03FF";	//要求先ユニットＩ／Ｏ番号
										d += "00";		//要求先ユニット局番号
										d += "@len";	//要求データ長
										d += "0000";	//ＣＰＵ監視タイマ

										d += "1401";	//コマンド
										d += "0000";	//サブコマンド

										foreach (var dev in w_worddevices)
										{
											int address = dev.deviceAddress;
											address = KV5000 ? address % 100000 : address;

											d += dev.deviceCode.ToString().PadRight(2, '*');
											if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC) d += address.ToString("D6");
											else d += address.ToString("X6");

											d += word_size.ToString("X4");

											for (int i = 0; i < dev.DeviceSize; i++)
											{
												if (dev.negative)
												{
													d += (~dev.data[i * 2 + 1]).ToString("X2");
													d += (~dev.data[i * 2]).ToString("X2");
												}
												else
												{
													d += (dev.data[i * 2 + 1]).ToString("X2");
													d += (dev.data[i * 2]).ToString("X2");
												}
											}

											dev.writing = true;
											dev.writed = false;
										}

										d = d.Replace("@len", (d.Length - 18).ToString("X4"));
										byte[] dd = Encoding.ASCII.GetBytes(d);
										data.AddRange(dd);
									}
									else
									{
										#region BINARY
										
										data.Add(0x50); //サブヘッダ
										data.Add(0x00); //サブヘッダ
										data.Add(0x00); //ネットワーク番号
										data.Add(0xFF); //ＰＣ番号
										data.Add(0xFF); //要求先ユニットＩ／Ｏ番号
										data.Add(0x03); //要求先ユニットＩ／Ｏ番号
										data.Add(0x00); //要求先ユニット局番号
										data.Add(0); //7:要求データ長
										data.Add(0); //8:要求データ長

										data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ
										data.Add(0x00); datalength++;   //ＣＰＵ監視タイマ

										data.Add(0x01); datalength++;   //コマンド
										data.Add(0x14); datalength++;   //コマンド
										data.Add(0x00); datalength++;   //サブコマンド
										data.Add(0x00); datalength++;   //サブコマンド

										//data.Add((byte)word_size); datalength++;
										//data.Add(0x00); datalength++;

										foreach (var dev in w_worddevices)
										{
											int address = dev.deviceAddress;
											address = KV5000 ? address % 100000 : address;

											data.Add((byte)(address & 0x000000FF)); datalength++;
											data.Add((byte)((address >> 8) & 0x000000FF)); datalength++;
											data.Add((byte)((address >> 16) & 0x000000FF)); datalength++;
											data.Add((byte)dev.deviceCode); datalength++;
											data.Add((byte)(dev.DeviceSize & 0x000000FF)); datalength++;
											data.Add((byte)((dev.DeviceSize >> 8) & 0x000000FF)); datalength++;

											for (int i = 0; i < dev.DeviceSize; i++)
											{
												if (dev.negative)
												{
													data.Add((byte)~dev.data[i * 2]); datalength++;
													data.Add((byte)~dev.data[i * 2 + 1]); datalength++;
												}
												else
												{
													data.Add(dev.data[i * 2]); datalength++;
													data.Add(dev.data[i * 2 + 1]); datalength++;
												}
											}

											dev.writing = true;
											dev.writed = false;
										}

										data[7] = (byte)(datalength & 0x000000FF);
										data[8] = (byte)((datalength >> 8) & 0x000000FF);

										#endregion
									}

									ReceiveBytes = new byte[0];
									if (DeviceType == DeviceTypes.TCP)
									{
										if (!Socket.SendBytes(data.ToArray()))
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC送信エラー[" + Socket.ErrorMessage + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											return true;
										}
									}
									else if (DeviceType == DeviceTypes.UDP)
									{
										if (!SocketUDP.SendBytes(data.ToArray(), ipaddressUDP))
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC送信エラー[" + SocketUDP.ErrorMessage + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											return true;
										}
									}
								}
								#endregion
								break;

							case DeviceTypes.RS232C:
								#region RS232C
								{
									switch (CommandType)
									{
										case CommandTypes.MELSEC_A_CPU:
											{
												data.AddRange(Encoding.Default.GetBytes("00FFWT0"));
												data.AddRange(Encoding.Default.GetBytes((word_size & 0xFF).ToString("X2")));

												foreach (var dev in w_worddevices)
												{
													for (int pos = 0; pos < dev.DeviceSize; pos++)
													{
														data.Add((byte)dev.GetDeviceCode()[0]);
														if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.HEX)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("X4")));
														}
														else if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("0000")));
														}

														if (pos * 2 + 1 < dev.data.Length)
														{
															data.AddRange(Encoding.Default.GetBytes(dev.data[pos * 2 + 1].ToString("X2")));
														}
														else
														{
															data.AddRange(Encoding.Default.GetBytes("00"));
														}

														data.AddRange(Encoding.Default.GetBytes(dev.data[pos * 2].ToString("X2")));
													}
												}
											}
											break;

										case CommandTypes.MELSEC_AnA_CPU:
											{
												data.AddRange(Encoding.Default.GetBytes("00FFQT0"));
												data.AddRange(Encoding.Default.GetBytes((word_size & 0xFF).ToString("X2")));

												foreach (var dev in w_worddevices)
												{
													for (int pos = 0; pos < dev.DeviceSize; pos++)
													{
														data.Add((byte)dev.GetDeviceCode()[0]);
														if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.HEX)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("X6")));
														}
														else if (dev.deviceAddresstype == DeviceInfo.DeviceAddressType.DEC)
														{
															data.AddRange(Encoding.Default.GetBytes((dev.deviceAddress + pos).ToString("000000")));
														}

														if (pos * 2 + 1 < dev.data.Length)
														{
															data.AddRange(Encoding.Default.GetBytes(dev.data[pos * 2 + 1].ToString("X2")));
														}
														else
														{
															data.AddRange(Encoding.Default.GetBytes("00"));
														}

														data.AddRange(Encoding.Default.GetBytes(dev.data[pos * 2].ToString("X2")));
													}
												}
											}
											break;
									}

									ReceiveBytes = new byte[0];
									if (!Com.SendBytes(data.ToArray()))
									{
										m_Step = 10;
										Error = true;
										ErrorDescription = "PLC送信エラー[" + Com.ErrorMessage + "]";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										return true;
									}
								}
								#endregion
								break;
						}

						m_Step = 710;
						m_swTemp.Restart();

						data.Clear();
						data = null;
					}
					#endregion
					break;

				case 710:
					#region WORD書き込み応答コマンド受信(WORD一括書き込み)
					{
						switch (DeviceType)
						{
							case DeviceTypes.TCP:
							case DeviceTypes.UDP:
								#region TCP/UDP
								{
									byte[] data = null;
									switch (DeviceType)
									{
										case DeviceTypes.TCP:
											data = Socket.ReceiveBytes();
											break;
										case DeviceTypes.UDP:
											data = SocketUDP.ReceiveBytes();
											break;
									}
									if (data == null)
									{
										Error = true;
										ErrorDescription = "デバイス種別異常";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
										break;
									}
									BL_Bytes.Join(data, ref ReceiveBytes, data.Length);
									byte[] receive_bytes = new byte[ReceiveBytes.Length];
									Array.Copy(ReceiveBytes, receive_bytes, ReceiveBytes.Length);

									if (isASCII)
									{
										#region ASCII->BINARY

										if (18 <= receive_bytes.Length)
										{
											byte[] d = new byte[receive_bytes.Length / 2];

											string s = "";
											s = Encoding.ASCII.GetString(receive_bytes, 0, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[0]);
											s = Encoding.ASCII.GetString(receive_bytes, 2, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[1]);
											s = Encoding.ASCII.GetString(receive_bytes, 4, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[2]);
											s = Encoding.ASCII.GetString(receive_bytes, 6, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[3]);
											s = Encoding.ASCII.GetString(receive_bytes, 8, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[4]);
											s = Encoding.ASCII.GetString(receive_bytes, 10, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[5]);
											s = Encoding.ASCII.GetString(receive_bytes, 12, 2); byte.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out d[6]);

											s = Encoding.ASCII.GetString(receive_bytes, 14, 4);
											int len = 0; int.TryParse(s, System.Globalization.NumberStyles.HexNumber, null, out len);
											len = len / 2;
											d[7] = (byte)(len % 0x100);
											d[8] = (byte)(len / 0x100);

											receive_bytes = d;
										}
										else
										{
											if (3000 < m_swTemp.ElapsedMilliseconds)
											{
												Error = true;
												ErrorDescription = "PLC応答タイムアウト";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
												m_Step = 10;
											}
											break;
										}

										#endregion
									}

									while (11 <= receive_bytes.Length)
									{
										if (receive_bytes[0] == 0xD0 && receive_bytes[1] == 0x00)
										{
											break;
										}
										else
										{
											receive_bytes = BL_Bytes.Trim(receive_bytes, 1, receive_bytes.Length - 1);
										}
									}

									if (11 <= receive_bytes.Length)
									{
										int length = receive_bytes[7] + receive_bytes[8] * 0x100 + 9;

										if (length <= receive_bytes.Length)
										{
											data = BL_Bytes.Trim(receive_bytes, length);
											ReceiveBytes = new byte[0];

											if (data[9] == 0 && data[10] == 0)
											{
												lock (write_eventdevices)
												{
													for (int i = 0; i < write_eventdevices.Count; i++)
													{
														if (write_eventdevices[i].writing)
														{
															write_eventdevices[i].writing = false;
															write_eventdevices[i].writed = true;
															break;
														}
													}
												}

												if (0 < queWrite.Count)
												{
													m_Step = 600;
												}
												else
												{
													//読み込み要求処理へ
													m_Step = 100;
												}

												if (swWrite.IsRunning) if (0 < swWrite.ElapsedMilliseconds) write_scantimes = swWrite.ElapsedMilliseconds;
											}
											else
											{
												m_Step = 10;
												Error = true;
												ErrorDescription = "PLC書込応答エラー[" + data[10].ToString("X2") + data[9].ToString("X2") + "]";
												if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
											}
										}
									}
									else if (3000 < m_swTemp.ElapsedMilliseconds)
									{
										Error = true;
										ErrorDescription = "PLC応答タイムアウト";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
									}

									receive_bytes = null;
								}
								#endregion
								break;

							case DeviceTypes.RS232C:
								#region RS232C
								{
									byte[] data = Com.ReceiveBytes();
									BL_Bytes.Join(data, ref ReceiveBytes, data.Length);

									if (ReceiveBytes.Length >= 6)
									{
										if (ReceiveBytes[4] == 0x47 || ReceiveBytes[5] == 0x47)
										{
											ReceiveBytes = new byte[0];

											lock (write_eventdevices)
											{
												for (int i = 0; i < write_eventdevices.Count; i++)
												{
													if (write_eventdevices[i].writing)
													{
														write_eventdevices[i].writing = false;
														write_eventdevices[i].writed = true;
														break;
													}
												}
											}

											if (0 < queWrite.Count)
											{
												m_Step = 600;
											}
											else
											{
												//読み込み要求処理へ
												m_Step = 100;
											}

											if (swWrite.IsRunning) if (0 < swWrite.ElapsedMilliseconds) write_scantimes = swWrite.ElapsedMilliseconds;
										}
										else
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC書込応答エラー[" + ReceiveBytes[7].ToString("X2") + ReceiveBytes[6].ToString("X2") + "]";
											if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										}
									}
									else if (3000 < m_swTemp.ElapsedMilliseconds)
									{
										Error = true;
										ErrorDescription = "PLC応答タイムアウト";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										m_Step = 10;
									}
								}
								#endregion
								break;
						}
					}
					#endregion
					break;

			}

			return base.DoControl(message);
		}

		#region 指定デバイスのブロッキング読み込み

		/// <summary>
		/// 指定デバイスのブロッキング読み込み
		/// </summary>
		/// <param name="timeoutMilliseconds"></param>
		/// <param name="noevent"></param>
		/// <param name="device"></param>
		/// <returns></returns>
		public string Read(int timeoutMilliseconds, bool noevent, params DeviceInfo[] device)
		{
			List<BL_MCProtocol.DeviceInfo> vlist = new List<DeviceInfo>();

			foreach(var v in device)
			{
				if (v.DeviceSize <= 960) vlist.Add(v);
				else
				{
					for (int address = v.deviceAddress; address < v.deviceAddress + v.DeviceSize; address += 960)
					{
						int devicesize = 0;
						if (address + 960 < v.deviceAddress + v.DeviceSize) devicesize = 960;
						else devicesize = v.deviceAddress + v.DeviceSize - address;

						BL_MCProtocol.DeviceInfo vv = new DeviceInfo(v.deviceCode, address, devicesize / (v.UnitSize / 2), v.valueType);
						vlist.Add(vv);
					}
				}
			}

			foreach (var v in vlist)
			{
				v.noevent = noevent;
				v.readed = false;
			}
			lock (read_eventdevices) read_eventdevices.AddRange(vlist);

			lock (queRead)
			{
				foreach (var v in vlist) queRead.Enqueue(v);
			}

			BL_Stopwatch sw = new BL_Stopwatch();
			sw.Start();

			bool allread = true;

			while (true)
			{
				if (Error) return ErrorDescription;
				if (timeoutMilliseconds < sw.ElapsedMilliseconds) break;

				allread = true;
				foreach (var v in vlist)
				{
					if (v.readed == false)
					{
						allread = false;
						break;
					}
				}
				if (allread) break;

				Sleep(m_Sleep * 2);
			}

			lock (read_eventdevices) read_eventdevices.Clear();

			vlist.Clear();
			vlist = null;

			if (!allread)
			{
				return "Timeout";
			}

			return "";
		}

		#endregion

		#region 指定デバイスのブロッキング書き込み

		/// <summary>
		/// 指定デバイスのブロッキング書き込み
		/// </summary>
		/// <param name="timeoutMilliseconds"></param>
		/// <param name="device"></param>
		/// <returns></returns>
		public string Write(int timeoutMilliseconds, params DeviceInfo[] device)
		{
			foreach (var v in device)
			{
				v.writing = false;
				v.writed = false;
			}
			lock (write_eventdevices) write_eventdevices.AddRange(device);

			lock (queWrite)
			{
				foreach (var v in device) queWrite.Enqueue(v);
			}

			BL_Stopwatch sw = new BL_Stopwatch();
			sw.Start();

			bool allwrite = true;

			while (true)
			{
				if (Error) return ErrorDescription;
				if (timeoutMilliseconds < sw.ElapsedMilliseconds) break;

				allwrite = true;
				foreach (var v in device)
				{
					if (v.writed == false)
					{
						allwrite = false;
						break;
					}
				}
				if (allwrite) break;

				Sleep(m_Sleep * 2);
			}

			lock (write_eventdevices) write_eventdevices.Clear();

			if (!allwrite)
			{
				return "Timeout";
			}

			return "";
		}

		#endregion

		/// <summary>
		/// 指定デバイス読み込み
		/// </summary>
		/// <param name="device"></param>
		public void Read(params DeviceInfo[] device)
		{
			lock (queRead)
			{
				foreach (var v in device)
				{
					queRead.Enqueue(v);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="device"></param>
		public void Write(params DeviceInfo[] device)
		{
			lock (queWrite)
			{
				foreach (var v in device)
				{
					queWrite.Enqueue(v);
				}
			}
		}


	}
}
