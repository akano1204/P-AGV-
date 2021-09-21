using System;
using System.Collections.Generic;
using System.Linq;
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
	public class BL_FAM3Protocol : BL_ThreadController_Base
	{

		/// <summary>
		/// 接続種別
		/// </summary>
		public enum DeviceTypes
		{
			/// <summary>未定</summary>
			Unknown = 0,
			/// <summary>TCP</summary>
			TCP = 1,
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
				X = 0x18,
				/// <summary></summary>
				Y = 0x19,
				/// <summary></summary>
				M = 0x90,
				/// <summary></summary>
				L = 0x92,
				/// <summary></summary>
				F = 0x06,
				/// <summary></summary>
				V = 0x94,
				/// <summary></summary>
				B = 0xA0,
				/// <summary></summary>
				D = 0x04,
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
						case DeviceCode.X: return DeviceInfo.DeviceKind.WORD;
						case DeviceCode.Y: return DeviceInfo.DeviceKind.WORD;
						case DeviceCode.M: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.L: return DeviceInfo.DeviceKind.BIT;
						case DeviceCode.F: return DeviceInfo.DeviceKind.WORD;
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
			public int[] idata = null;

			/// <summary>
			/// 直前の読み出しデータ／書き込みデータを保持します。
			/// </summary>
			public byte[] data_pre = null;

			/// <summary>
			/// 読み込み管理フラグ
			/// </summary>
			public bool readed = false;

			/// <summary>
			/// 書き込み管理フラグ
			/// </summary>
			public bool writing = false;

			/// <summary>
			/// 書き込み管理フラグ
			/// </summary>
			public bool writed = false;

			/// <summary>
			/// イベント不要
			/// </summary>
			public bool noevent = false;

			/// <summary>
			/// 変化フラグ
			/// </summary>
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
				dev.idata = new int[data.Length];		
				Array.Copy(data, dev.data, data.Length);
				return dev;
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
				this.idata = new int[this.dataLength];
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
			/// <returns></returns>
			public object Get(int position)
			{
				if (endian == Endian.LITTLE)
				{
					switch (valueType)
					{
						case ValueType.BIN16: return BitConverter.ToInt16(data, position * sizeof(Int16));
						case ValueType.UBIN16: return BitConverter.ToUInt16(data, position * sizeof(UInt16));
						case ValueType.BIN32: return BitConverter.ToInt32(data, position * sizeof(Int32));
						case ValueType.UBIN32: return BitConverter.ToUInt32(data, position * sizeof(UInt32));
						case ValueType.BIN64: return BitConverter.ToInt64(data, position * sizeof(Int64));
						case ValueType.UBIN64: return BitConverter.ToUInt64(data, position * sizeof(UInt64));
						case ValueType.FLOAT: return BitConverter.ToSingle(data, position * sizeof(Single));
						case ValueType.DOUBLE: return BitConverter.ToDouble(data, position * sizeof(Double));
						case ValueType.STRING: return Encoding.Default.GetString(data).Substring(position).TrimEnd('\0');
						case ValueType.BIT: return data[position] != 0 ? true : false;
					}
				}
				else
				{
					byte[] bigdata = new byte[data.Length];
					for (int i = 0; i < data.Length; i++) bigdata[i] = data[data.Length - 1 - i];

					switch (valueType)
					{
						case ValueType.BIN16: return BitConverter.ToInt16(data, position * sizeof(Int16));
						case ValueType.UBIN16: return BitConverter.ToUInt16(data, position * sizeof(UInt16));
						case ValueType.BIN32: return BitConverter.ToInt32(data, position * sizeof(Int32));
						case ValueType.UBIN32: return BitConverter.ToUInt32(data, position * sizeof(UInt32));
						case ValueType.BIN64: return BitConverter.ToInt64(data, position * sizeof(Int64));
						case ValueType.UBIN64: return BitConverter.ToUInt64(data, position * sizeof(UInt64));
						case ValueType.FLOAT: return BitConverter.ToSingle(data, position * sizeof(Single));
						case ValueType.DOUBLE: return BitConverter.ToDouble(data, position * sizeof(Double));
						case ValueType.STRING: return Encoding.Default.GetString(data).Substring(position).TrimEnd('\0');
						case ValueType.BIT: return data[position] != 0 ? true : false;
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
			/// <param name="position"></param>
			/// <returns></returns>
			public T Get<T>(int position)
			{
				T ret = default(T);

				if (endian == Endian.LITTLE)
				{
					switch (valueType)
					{
						case ValueType.BIN16: return (T)((object)BitConverter.ToInt16(data, position * sizeof(Int16)));
						case ValueType.UBIN16: return (T)((object)BitConverter.ToUInt16(data, position * sizeof(UInt16)));
						case ValueType.BIN32: return (T)((object)BitConverter.ToInt32(data, position * sizeof(Int32)));
						case ValueType.UBIN32: return (T)((object)BitConverter.ToUInt32(data, position * sizeof(UInt32)));
						case ValueType.BIN64: return (T)((object)BitConverter.ToInt64(data, position * sizeof(Int64)));
						case ValueType.UBIN64: return (T)((object)BitConverter.ToUInt64(data, position * sizeof(UInt64)));
						case ValueType.FLOAT: return (T)((object)BitConverter.ToSingle(data, position * sizeof(Single)));
						case ValueType.DOUBLE: return (T)((object)BitConverter.ToDouble(data, position * sizeof(Double)));
						case ValueType.STRING: return (T)((object)Encoding.Default.GetString(data).Substring(position).TrimEnd('\0'));
						case ValueType.BIT: return (T)((object)(data[position] != 0 ? true : false));
					}
				}
				else
				{
					byte[] bigdata = new byte[data.Length];
					for (int i = 0; i < data.Length; i++) bigdata[i] = data[data.Length - 1 - i];

					switch (valueType)
					{
						case ValueType.BIN16: return (T)((object)BitConverter.ToInt16(data, position * sizeof(Int16)));
						case ValueType.UBIN16: return (T)((object)BitConverter.ToUInt16(data, position * sizeof(UInt16)));
						case ValueType.BIN32: return (T)((object)BitConverter.ToInt32(data, position * sizeof(Int32)));
						case ValueType.UBIN32: return (T)((object)BitConverter.ToUInt32(data, position * sizeof(UInt32)));
						case ValueType.BIN64: return (T)((object)BitConverter.ToInt64(data, position * sizeof(Int64)));
						case ValueType.UBIN64: return (T)((object)BitConverter.ToUInt64(data, position * sizeof(UInt64)));
						case ValueType.FLOAT: return (T)((object)BitConverter.ToSingle(data, position * sizeof(Single)));
						case ValueType.DOUBLE: return (T)((object)BitConverter.ToDouble(data, position * sizeof(Double)));
						case ValueType.STRING: return (T)((object)Encoding.Default.GetString(data).Substring(position).TrimEnd('\0'));
						case ValueType.BIT: return (T)((object)(data[position] != 0 ? true : false));
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
						for (int i = 0; i < temp.Length; i++) bigdata[i] = temp[temp.Length - 1 - i];
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
						for (int i = 0; i < temp.Length; i++) bigdata[i] = temp[temp.Length - 1 - i];
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
						for (int i = 0; i < temp.Length; i++) bigdata[i] = temp[temp.Length - 1 - i];
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
						for (int i = 0; i < temp.Length; i++) bigdata[i] = temp[temp.Length - 1 - i];
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
						for (int i = 0; i < temp.Length; i++) bigdata[i] = temp[temp.Length - 1 - i];
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
						for (int i = 0; i < temp.Length; i++) bigdata[i] = temp[temp.Length - 1 - i];
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
						for (int i = 0; i < temp.Length; i++) bigdata[i] = temp[temp.Length - 1 - i];
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
						for (int i = 0; i < temp.Length; i++) bigdata[i] = temp[temp.Length - 1 - i];
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
					Array.Copy(temp, 0, data, 0, temp.Length);
				}
				else if (position + val.Length <= data.Length)
				{
					byte[] temp = Encoding.Default.GetBytes((string)val);
					Array.Copy(temp, 0, data, position, temp.Length);
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
					if (valueType == ValueType.BIT) data[position] = (byte)(val ? 1 : 0);
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

		///// <summary>
		///// デバイス値の変化イベントのデリゲート宣言
		///// </summary>
		///// <param name="plc"></param>
		///// <param name="deviceinfo"></param>
		//public delegate void Event_Handler_ValueChanged(BL_FAM3Protocol plc, DeviceInfo deviceinfo);

		///// <summary>
		///// デバイス値の変化イベント
		///// </summary>
		//public virtual event Event_Handler_ValueChanged EventValueChanged;

		/// <summary>
		/// PLC通信エラーイベントのデリゲート宣言
		/// </summary>
		/// <param name="plc"></param>
		/// <param name="error"></param>
		/// <param name="errordescription"></param>
		public delegate void Event_Handler_PlcError(BL_FAM3Protocol plc, bool error, string errordescription);

		/// <summary>
		/// PLC通信エラーイベント
		/// </summary>
		public virtual event Event_Handler_PlcError EventPlcError;

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
		/// <summary>接続先のIPエンドポイント</summary>
		protected IPEndPoint RemoteEP = null;
		/// <summary>ソケット読み出しバッファ</summary>
		protected byte[] ReceiveBytes = new byte[0];


		private string ipaddress = "";
		private int portno = 0;

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

		private bool SupportRandomRead = false;

		/// <summary>
		/// 読み込み処理の処理速度(ms)を取得します。
		/// </summary>
		public int ReadScanTimes { get { return read_scantimes; } }

		/// <summary>
		/// 書き込み処理の処理速度(ms)を取得します。
		/// </summary>
		public int WriteScanTimes { get { return write_scantimes; } }

		/// <summary>
		/// コンストラクタ(LAN)
		/// </summary>
		/// <param name="ipaddress">IPアドレス</param>
		/// <param name="portno">ソケットポートNo</param>
		public BL_FAM3Protocol(string ipaddress, int portno)
		{
			DeviceType = DeviceTypes.TCP;
			this.ipaddress = ipaddress;
			this.portno = portno;
		}

		/// <summary>
		/// コンストラクタ(LAN)
		/// </summary>
		/// <param name="ipaddress">IPアドレス</param>
		/// <param name="portno">ソケットポートNo</param>
		/// <param name="SupportRandomRead">複数ブロック一括読み出し可能</param>
		public BL_FAM3Protocol(string ipaddress, int portno, bool SupportRandomRead)
		{
			DeviceType = DeviceTypes.TCP;
			this.ipaddress = ipaddress;
			this.portno = portno;
			this.SupportRandomRead = SupportRandomRead;
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

			}
		}

		/// <summary>
		/// 通信処理
		/// </summary>
		/// <param name="message"></param>
		/// <returns></returns>
		/// 

		/// <summary>
		/// ステーション番号
		/// </summary>
		public string StationNo = "01";

		/// <summary>
		/// CPU番号
		/// </summary>
		public string CpuNo = "01";

		public byte GetDeviceCode(BL_FAM3Protocol.DeviceInfo.DeviceCode DeviceCode)
		{
			switch (DeviceCode)
			{
				case BL_FAM3Protocol.DeviceInfo.DeviceCode.D: return 0x04;
				case BL_FAM3Protocol.DeviceInfo.DeviceCode.F: return 0x06;
				case BL_FAM3Protocol.DeviceInfo.DeviceCode.X: return 0x18;
				case BL_FAM3Protocol.DeviceInfo.DeviceCode.Y: return 0x19;
			}
			return 0;
		}

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
								if (Socket != null) if (Socket.IsConnected) Socket.Close();
								//Socket = new BL_RawSocket(ipaddress, portno);
								Socket = new BL_RawSocket(ipaddress, portno);
								Socket.ConnectTimeOut = 100;
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
								if (Socket != null) if (Socket.IsConnected) Socket.Close();
								break;
						}
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
					#region 読み出し要求コマンド送信(WORD読み出し)
					{
						swRead.Restart();

						if (queRead.Count == 0)
						{
							lock (ReadDevices) foreach (var v in ReadDevices) queRead.Enqueue(v);
						}
						if (queRead.Count == 0 && queWrite.Count == 0) break;

						if (queRead.Count == 0 && 0 < queWrite.Count)
						{
							//読み出し要求が無ければ書き込み要求処理へ
							m_Step = 200;
							break;
						}

						r_bitdevices.Clear();
						r_worddevices.Clear();

						int size = 0;
						int blockcount = 0;
						while (0 < queRead.Count)
						{
							BL_FAM3Protocol.DeviceInfo v = queRead.Peek();
							v.changed = false;
							if (v.deviceKind == DeviceInfo.DeviceKind.WORD)
							{
								r_worddevices.Add(v);
								size += v.DeviceSize;
								blockcount++;
							}

							if (size <= 960 && blockcount <= 120) queRead.Dequeue();
							else break;

						}

						List<byte> data = new List<byte>();

						data.Add(0x11); //コマンド
						data.Add(0x01); //ＣＰＵ番号
						data.Add(0x00); //
						data.Add(0x08); //
						data.Add(0x00); //デバイス名　
						byte devprop = GetDeviceCode(r_worddevices[0].deviceCode); //コマンドのデバイス属性
						data.Add(devprop); //
						int address = r_worddevices[0].deviceAddress;
						data.Add((byte)((address >> 24) & 0x000000FF));
						data.Add((byte)((address >> 16) & 0x000000FF));
						data.Add((byte)((address >> 8) & 0x000000FF));
						data.Add((byte)(address & 0x000000FF));
						data.Add(0x00);
						data.Add((byte)(r_worddevices[0].DeviceSize & 0x000000FF));

                        ReceiveBytes = new byte[0];
                        if (!Socket.SendBytes(data.ToArray()))
						{
							m_Step = 10;
							Error = true;
							ErrorDescription = "PLC送信エラー[" + Socket.ErrorMessage + "]";
							if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
							break;
						}

						m_Step = 110;
						m_swTemp.Restart();
					}
					#endregion
					break;

				case 110:
					#region 読み出し応答コマンド受信(WORD読み出し)
					{
						byte[] data = Socket.ReceiveBytes();
						BL_Bytes.Join(data, ref ReceiveBytes, data.Length);

						if (4 <= ReceiveBytes.Length)
						{
							if (ReceiveBytes[0] == 0x91 && ReceiveBytes[1] == 0x00)
							{
								int length = ReceiveBytes[3] + ReceiveBytes[2] * 0x100;

								if (length + 4 <= ReceiveBytes.Length)
								{
									int position = 4;	//読み込みデータ開始位置
									for (int i = 0; i < r_worddevices[0].DeviceSize; i++)
									{
										int ii = data[i * 2 + position] * 0x100 + data[i * 2 + position + 1];
										string ss = ii.ToString("x");
										r_worddevices[0].idata[i] = data[i * 2 + position] * 0x100 + data[i * 2 + position + 1];
									}
									r_worddevices[0].readed = true;
									if (0 < queWrite.Count)
									{
										//書き込み要求があれば書き込み要求処理へ
										m_Step = 200;
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
									ErrorDescription = "PLC読込応答エラー[" + data[1].ToString("X2") + data[4].ToString("X2") + data[5].ToString("X2") + "]";
									if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
									m_Step = 10;
								}
							}
							else
							{
								Error = true;
								ErrorDescription = "PLC読込応答エラー[" + data[1].ToString("X2") + data[4].ToString("X2") + data[5].ToString("X2") + "]";
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

				case 200:
					#region WORD書き込み要求コマンド送信(WORD書き込み)
					{
						swWrite.Restart();

						w_bitdevices.Clear();
						w_worddevices.Clear();

						while (0 < queWrite.Count)
						{
							BL_FAM3Protocol.DeviceInfo v = queWrite.Dequeue();
							if (v.deviceKind == DeviceInfo.DeviceKind.WORD) w_worddevices.Add(v);
						}

						int word_size = 0;
						foreach (var dev in w_worddevices) word_size += dev.DeviceSize;

						List<byte> data = new List<byte>();
						int datalength = 0;

						switch (DeviceType)
						{
							case DeviceTypes.TCP:
								#region TCP
								{
									data.Add(0x12); //コマンド
									data.Add(0x01); //ＣＰＵ番号
									int dsize = w_worddevices[0].DeviceSize * 2 + 8;
									data.Add((byte)((dsize >> 8) & 0x000000FF));
									data.Add((byte)(dsize & 0x000000FF));
									data.Add(0x00); //デバイス名　
									byte devprop = GetDeviceCode(r_worddevices[0].deviceCode); //コマンドのデバイス属性
									data.Add(devprop); //
									int address =  w_worddevices[0].deviceAddress;
									data.Add((byte)((address >> 24) & 0x000000FF));
									data.Add((byte)((address >> 16) & 0x000000FF));
									data.Add((byte)((address >> 8) & 0x000000FF));
									data.Add((byte)(address & 0x000000FF));
									data.Add(0x00);
									data.Add((byte)(w_worddevices[0].DeviceSize & 0x000000FF));
									
									foreach (var dev in w_worddevices)
									{
										for (int i = 0; i < dev.DeviceSize; i++)
										{
												data.Add(dev.data[i * 2 + 1]); datalength++;
												data.Add(dev.data[i * 2]); datalength++;
										}
									}
									ReceiveBytes = new byte[0];

                                    if (!Socket.SendBytes(data.ToArray()))
									{
										m_Step = 10;
										Error = true;
										ErrorDescription = "PLC送信エラー[" + Socket.ErrorMessage + "]";
										if (EventPlcError != null) EventPlcError(this, Error, ErrorDescription);
										return true;
									}
								}
								#endregion
								break;

						}

						m_Step = 210;
						m_swTemp.Restart();
					}
					#endregion
					break;

				case 210:
					#region WORD書き込み応答コマンド受信(WORD書き込み)
					{
						switch (DeviceType)
                        {
                            case DeviceTypes.TCP:
                                #region TCP
                                {
									byte[] data = Socket.ReceiveBytes();
									BL_Bytes.Join(data, ref ReceiveBytes, data.Length);

									if (4 <= ReceiveBytes.Length)
									{
										if (data[0] == 0x92 && data[1] == 0x0)
										{
											w_worddevices[0].writed = true;
											//読み込み要求処理へ
											m_Step = 100;
											if (swWrite.IsRunning) if (0 < swWrite.ElapsedMilliseconds) write_scantimes = swWrite.ElapsedMilliseconds;
										}
										else
										{
											m_Step = 10;
											Error = true;
											ErrorDescription = "PLC書込応答エラー[" + data[1].ToString("X2") + data[4].ToString("X2") + data[5].ToString("X2") + "]";
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
			foreach (var v in device)
			{
				v.noevent = noevent;
				v.readed = false;
			}
			lock (read_eventdevices) read_eventdevices.AddRange(device);

			foreach (var v in device) queRead.Enqueue(v);

			BL_Stopwatch sw = new BL_Stopwatch();
			sw.Start();

			bool allread = true;

			while (true)
			{
				if (Error) return ErrorDescription;
				if (timeoutMilliseconds < sw.ElapsedMilliseconds) break;

				allread = true;
				foreach (var v in device)
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

			return "";
		}

		#endregion

		#region 指定デバイスのブロッキング書き込み

		/// <summary>
		/// 指定デバイスのブロッキング書き込み
		/// </summary>
		/// <param name="timeoutMilliseconds"></param>
		/// <param name="device"></param>

		public string Write(int timeoutMilliseconds, params DeviceInfo[] device)
		{
			foreach (var v in device)
			{
				v.writing = false;
				v.writed = false;
			}
			lock (write_eventdevices) write_eventdevices.AddRange(device);

			foreach (var v in device) queWrite.Enqueue(v);

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

			return "";
		}

		#endregion
	}
}
