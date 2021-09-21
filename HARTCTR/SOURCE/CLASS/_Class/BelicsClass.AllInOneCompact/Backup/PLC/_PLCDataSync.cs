using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using BelicsClass.Common;
using BelicsClass.ObjectSync;
using BelicsClass.ProcessManage;

namespace BelicsClass.PLC
{
    /// <summary>
    /// PLC通信を行います。
    /// 複数範囲のデータメモリを定期的に共有メモリと同期します。
    /// 読み込み・書き込み双方
    /// </summary>
    public class BL_PLCDataSync : BL_ThreadController_Base
    {
        /// <summary>
        /// 
        /// </summary>
        public class Initializer : BL_FaceMemorySyncNotify
        {
            /// <summary>初期化済みフラグ</summary>
            [BL_ObjectSyncAttribute]
            public bool Initialized = false;

            /// <summary>終了要求フラグ</summary>
            [BL_ObjectSyncAttribute]
            public bool ExitRequest = false;

            /// <summary>接続中フラグ</summary>
            [BL_ObjectSyncAttribute]
            public bool IsConnected = false;
        }

        private Initializer initializer = new Initializer();

        #region データメモリ同期用共有メモリ

        /// <summary>データメモリの１範囲を同期するための共有メモリを管理します</summary>
        public class DM : BL_FaceMemorySyncNotify
        {
            /// <summary>アクセス種別</summary>
            public enum AccessType
            {
                /// <summary>非同期読み込み</summary>
                READ,
                /// <summary>非同期書き込み</summary>
                WRITE,

                /// <summary>同期読み込み</summary>
                READ_SYNC,
            }

            /// <summary>共有メモリ名</summary>
            [BL_ObjectSyncAttribute]
            public string Name = "".PadRight(32);
            /// <summary>デバイスコード</summary>
            [BL_ObjectSyncAttribute]
            public byte DeviceCode = 0x00;
            /// <summary>データメモリ開始アドレス</summary>
            [BL_ObjectSyncAttribute]
            public int Offset = 0;
            /// <summary>データメモリワード数</summary>
            [BL_ObjectSyncAttribute]
            public int WordCount = 0;


            /// <summary>通信処理制御ステップ</summary>
            [BL_ObjectSyncAttribute]
            public int Step = 0;

            /// <summary>アクセス種別</summary>
            [BL_ObjectSyncAttribute]
            public AccessType RWType = AccessType.READ;

            /// <summary>接続済みフラグ</summary>
            [BL_ObjectSyncAttribute]
            public bool Connected = false;

            /// <summary>初回のみ実施する場合 true を設定</summary>
            [BL_ObjectSyncAttribute]
            public bool IsOneshot = false;

            /// <summary>書き込み処理時に初回読み込みを行うかどうかを表すフラグ</summary>
            [BL_ObjectSyncAttribute]
            public bool IsReadBeforeWrite = true;

            /// <summary>書き込み処理時に初回読み込みが終わったかどうかを表すフラグ</summary>
            [BL_ObjectSyncAttribute]
            public bool IsReadBeforeWriteComplete = false;

            /// <summary>書き込み処理時に初回のメモリ更新が完了しているかどうかを表すフラグ</summary>
            [BL_ObjectSyncAttribute]
            public bool IsWriteDataReady = false;

            /// <summary></summary>
            [BL_ObjectSyncAttribute]
            public long RefreshedInterval = 0;

            /// <summary></summary>
            [BL_ObjectSyncAttribute]
            public int TimeoutMilliseconds = 5000;

            /// <summary></summary>
            public BL_Stopwatch swRefresh = new BL_Stopwatch();

            /// <summary>デバッグ用カウンター</summary>
            public byte debugCounter = 0;

            /// <summary>
            /// DMの同期用メモリ
            /// </summary>
            public class DataMemory
            {
                /// <summary></summary>
                [BL_ObjectSyncAttribute]
                public byte[] data = new byte[2000];

                /// <summary></summary>
                public byte[] data_temp = new byte[2000];

                /// <summary>
                /// 
                /// </summary>
                public DataMemory() { }

                /// <summary>
                /// 
                /// </summary>
                /// <param name="size"></param>
                public DataMemory(int size)
                {
                    data = new byte[size * 2];
                    data_temp = new byte[size * 2];
                }
            }

            /// <summary>データメモリをバイト配列に展開するクラスのインスタンス</summary>
            [BL_ObjectSyncAttribute]
            public DataMemory memory = new DataMemory(1000);

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return Name.Trim() + ":" + Offset.ToString() + "(" + WordCount.ToString() + ") " + base.ToString();
            }

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            public DM() { }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="name"></param>
            /// <param name="type"></param>
            /// <param name="devicecode"></param>
            /// <param name="offset"></param>
            /// <param name="wordcount"></param>
            public DM(string name, AccessType type, byte devicecode, int offset, int wordcount)
                : this(name, type, devicecode, offset, wordcount, false)
            {
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="name"></param>
            /// <param name="type"></param>
            /// <param name="devicecode"></param>
            /// <param name="offset"></param>
            /// <param name="wordcount"></param>
            /// <param name="is_oneshot"></param>
            public DM(string name, AccessType type, byte devicecode, int offset, int wordcount, bool is_oneshot)
                : this(name, type, devicecode, offset, wordcount, is_oneshot, false)
            {
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="name"></param>
            /// <param name="type"></param>
            /// <param name="devicecode"></param>
            /// <param name="offset"></param>
            /// <param name="wordcount"></param>
            /// <param name="is_oneshot"></param>
            /// <param name="first_read"></param>
            public DM(string name, AccessType type, byte devicecode, int offset, int wordcount, bool is_oneshot, bool first_read)
                : this(name, type, devicecode, offset, wordcount, is_oneshot, false, 5000)
            {
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="name"></param>
            /// <param name="type"></param>
            /// <param name="devicecode"></param>
            /// <param name="offset"></param>
            /// <param name="wordcount"></param>
            /// <param name="is_oneshot"></param>
            /// <param name="first_read"></param>
            /// <param name="timeout"></param>
            public DM(string name, AccessType type, byte devicecode, int offset, int wordcount, bool is_oneshot, bool first_read, int timeout)
            {
                Name = name.Trim().PadRight(32);
                RWType = type;
                DeviceCode = devicecode;
                Offset = offset;
                WordCount = wordcount;
                IsOneshot = is_oneshot;
                IsReadBeforeWrite = first_read;
                TimeoutMilliseconds = timeout;

                if (RWType == AccessType.WRITE)
                {
                    if (is_oneshot && first_read)
                    {
                        throw new NotSupportedException("書込定義で１回のみ書込モードと初回読込モードは併用できません");
                    }
                }

                memory = new DataMemory(wordcount);
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="name"></param>
            public DM(string name)
            {
                Name = name.Trim().PadRight(32);
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="wordcount"></param>
            public DM(int wordcount)
            {
                memory = new DataMemory(wordcount);
            }

            #endregion

            #region データ変換

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <returns></returns>
            public Int16 ToInt16(int device_address)
            {
                if (device_address < Offset || Offset + WordCount < device_address)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                Lock();
                for (int i = 0; i < 2; i++) ReadMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                Unlock();

                return BitConverter.ToInt16(memory.data, device_address * 2);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <param name="value"></param>
            public void ToInt16(int device_address, Int16 value)
            {
                if (device_address < Offset || Offset + WordCount < device_address)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                byte[] data = BitConverter.GetBytes(value);

                Lock();
                for (int i = 0; i < data.Length; i++)
                {
                    memory.data[device_address * 2 + i] = data[i];
                    WriteMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                }
                Unlock();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <returns></returns>
            public Int32 ToInt32(int device_address)
            {
                if (device_address < Offset || Offset + WordCount < device_address + 1)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                Lock();
                for (int i = 0; i < 4; i++) ReadMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                Unlock();

                return BitConverter.ToInt32(memory.data, device_address * 2);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <param name="value"></param>
            public void ToInt32(int device_address, Int32 value)
            {
                if (device_address < Offset || Offset + WordCount < device_address + 1)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                byte[] data = BitConverter.GetBytes(value);

                Lock();
                for (int i = 0; i < data.Length; i++)
                {
                    memory.data[device_address * 2 + i] = data[i];
                    WriteMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                }
                Unlock();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <returns></returns>
            public Int64 ToInt64(int device_address)
            {
                if (device_address < Offset || Offset + WordCount < device_address + 3)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                Lock();
                for (int i = 0; i < 8; i++) ReadMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                Unlock();

                return BitConverter.ToInt64(memory.data, device_address * 2);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <param name="value"></param>
            public void ToInt64(int device_address, Int64 value)
            {
                if (device_address < Offset || Offset + WordCount < device_address + 3)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                byte[] data = BitConverter.GetBytes(value);

                Lock();
                for (int i = 0; i < data.Length; i++)
                {
                    memory.data[device_address * 2 + i] = data[i];
                    WriteMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                }
                Unlock();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <returns></returns>
            public UInt16 ToUInt16(int device_address)
            {
                if (device_address < Offset || Offset + WordCount < device_address)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                Lock();
                for (int i = 0; i < 2; i++) ReadMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                Unlock();

                return BitConverter.ToUInt16(memory.data, device_address * 2);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <param name="value"></param>
            public void ToUInt16(int device_address, UInt16 value)
            {
                if (device_address < Offset || Offset + WordCount < device_address)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                byte[] data = BitConverter.GetBytes(value);

                Lock();
                for (int i = 0; i < data.Length; i++)
                {
                    memory.data[device_address * 2 + i] = data[i];
                    WriteMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                }
                Unlock();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <returns></returns>
            public UInt16 ToUInt16_BCD(int device_address)
            {
                if (device_address < Offset || Offset + WordCount < device_address)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                Lock();
                for (int i = 0; i < 2; i++) ReadMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                Unlock();

                byte b1 = BL_BitConverter.BcdToBin(memory.data[device_address * 2]);
                byte b2 = BL_BitConverter.BcdToBin(memory.data[device_address * 2 + 1]);
                UInt16 ret = (UInt16)(b2 * 100 + b1);

                return ret;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <param name="value"></param>
            public void ToUInt16_BCD(int device_address, UInt16 value)
            {
                if (device_address < Offset || Offset + WordCount < device_address)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                Lock();

                byte b1 = (byte)(value / 100);
                byte b2 = (byte)(value % 100);

                memory.data[device_address * 2 + 0] = BL_BitConverter.BinToBcd(b2);
                memory.data[device_address * 2 + 1] = BL_BitConverter.BinToBcd(b1);

                WriteMemory("memory.data[" + (device_address * 2 + 0).ToString() + "]", false);
                WriteMemory("memory.data[" + (device_address * 2 + 1).ToString() + "]", false);

                Unlock();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <returns></returns>
            public UInt32 ToUInt32(int device_address)
            {
                if (device_address < Offset || Offset + WordCount < device_address + 1)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                Lock();
                for (int i = 0; i < 4; i++) ReadMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                Unlock();

                return BitConverter.ToUInt32(memory.data, device_address * 2);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <param name="value"></param>
            public void ToUInt32(int device_address, UInt32 value)
            {
                if (device_address < Offset || Offset + WordCount < device_address + 1)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                byte[] data = BitConverter.GetBytes(value);

                Lock();
                for (int i = 0; i < data.Length; i++)
                {
                    memory.data[device_address * 2 + i] = data[i];
                    WriteMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                }
                Unlock();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <returns></returns>
            public UInt64 ToUInt64(int device_address)
            {
                if (device_address < Offset || Offset + WordCount < device_address + 3)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                Lock();
                for (int i = 0; i < 8; i++) ReadMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                Unlock();

                return BitConverter.ToUInt64(memory.data, device_address * 2);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <param name="value"></param>
            public void ToUInt64(int device_address, UInt64 value)
            {
                if (device_address < Offset || Offset + WordCount < device_address + 3)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                byte[] data = BitConverter.GetBytes(value);

                Lock();
                for (int i = 0; i < data.Length; i++)
                {
                    memory.data[device_address * 2 + i] = data[i];
                    WriteMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                }
                Unlock();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            public string ToString(int device_address, int length)
            {
                if (device_address < Offset || Offset + WordCount < device_address + (length / 2) - 1)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                Lock();
                for (int i = 0; i < length; i++) ReadMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                Unlock();

                return Encoding.Default.GetString(memory.data, device_address * 2, length).Trim('\0');
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="device_address"></param>
            /// <param name="length"></param>
            /// <param name="value"></param>
            /// <returns></returns>
            public void ToString(int device_address, int length, string value)
            {
                if (device_address < Offset || Offset + WordCount < device_address + (length / 2) - 1)
                {
                    throw new Exception("ＤＭアドレス範囲が不正です[" + device_address.ToString() + "]");
                }

                device_address -= Offset;

                byte[] data = Encoding.Default.GetBytes(value);

                Lock();
                for (int i = 0; i < length; i++)
                {
                    if (i < data.Length)
                        memory.data[device_address * 2 + i] = data[i];
                    else
                        memory.data[device_address * 2 + i] = 0;

                    WriteMemory("memory.data[" + (device_address * 2 + i).ToString() + "]", false);
                }
                Unlock();
            }

            #endregion
        }

        #endregion

        #region イベント関連

        /// <summary>
        /// スレッド実行状態変化イベント定義
        /// </summary>
        /// <param name="sender"></param>
        public delegate void Event_Handler_PlcConnectState(BL_PLCDataSync sender);

        /// <summary>
        /// スレッド実行状態変化イベント定義
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="dm"></param>
        public delegate void Event_Handler_PlcCommunicateState(BL_PLCDataSync sender, DM dm);

        /// <summary></summary>
        public virtual event Event_Handler_PlcConnectState EventConnected;
        /// <summary></summary>
        public virtual event Event_Handler_PlcConnectState EventDisconnected;
        /// <summary></summary>
        public virtual event Event_Handler_PlcConnectState EventSyncronized;
        /// <summary></summary>
        public virtual event Event_Handler_PlcConnectState EventError;
        /// <summary></summary>
        public virtual event Event_Handler_PlcCommunicateState EventReaded;
        /// <summary></summary>
        public virtual event Event_Handler_PlcCommunicateState EventWrited;

        #endregion

        #region フィールド

        /// <summary>デバイスメモリ同期用共有メモリのリスト</summary>
        public List<DM> Mem = new List<DM>();

        private string IpAddress;
        private int Port;
        private BL_PLC plc = null;
        private bool isDebug = false;

        #endregion

        #region 状態取得（プロパティ）

        /// <summary></summary>
        public bool Initialized
        {
            get
            {
                initializer.ReadMemory("Initialized");
                return initializer.Initialized;
            }
        }

        /// <summary>接続済みかどうかを取得します</summary>
        public bool Connected
        {
            get
            {
                if (plc == null) return false;
                return plc.IsConnected;
            }
        }

        /// <summary>終了要求を取得または設定します</summary>
        public bool IsExitRequest
        {
            get
            {
                initializer.ReadMemory("ExitRequest", false);
                return initializer.ExitRequest;
            }

            set
            {
                initializer.ExitRequest = value;
                initializer.WriteMemory("ExitRequest", false);
            }
        }

        /// <summary>接続中であるかどうかを取得します</summary>
        public bool IsConnected
        {
            get
            {
                initializer.ReadMemory("IsConnected");
                return initializer.IsConnected;
            }
        }

        /// <summary></summary>
        public override string ToString()
        {
            return base.ToString() + "[" + IpAddress + ":" + Port.ToString() + "]";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device_address"></param>
        /// <returns></returns>
        public DM GetReaderMemory(int device_address)
        {
            foreach (var m in Mem)
            {
                if ((m.RWType == DM.AccessType.READ || m.RWType == DM.AccessType.READ_SYNC) && m.Offset <= device_address && device_address < m.Offset + m.WordCount)
                {
                    return m;
                }
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device_address"></param>
        /// <returns></returns>
        public DM GetWriterMemory(int device_address)
        {
            foreach (var m in Mem)
            {
                if (m.RWType == DM.AccessType.WRITE && m.Offset <= device_address && device_address < m.Offset + m.WordCount)
                {
                    return m;
                }
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// 書き込みデータの準備完了をセットします
        /// </summary>
        /// <param name="device_address"></param>
        public void SetWriteDataReady(int device_address)
        {
            DM dm = GetWriterMemory(device_address);

            dm.IsWriteDataReady = true;
            dm.WriteMemory("IsWriteDataReady");
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// 
        /// </summary>
        public BL_PLCDataSync()
        {
            initializer.Initialize<Initializer>("PLCDATASYNC_INITIALIZER");
        }

        /// <summary>
        /// 
        /// </summary>
        public BL_PLCDataSync(string name)
        {
            initializer.Initialize<Initializer>(name + "_INITIALIZER");
        }

        #endregion

        #region デバイス定義追加メソッド

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="devicecode"></param>
        /// <param name="offset"></param>
        /// <param name="wordcount"></param>
        /// <param name="is_oneshot"></param>
        /// <param name="first_read"></param>
        /// <param name="timeout"></param>
        public void AddDevice(string name, DM.AccessType type, byte devicecode, int offset, int wordcount, bool is_oneshot, bool first_read, int timeout)
        {
            DM dm = new DM(name, type, devicecode, offset, wordcount, is_oneshot, first_read, timeout);

            dm.Initialize<DM>(name);
            dm.WriteMemory();

            Mem.Add(dm);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="devicecode"></param>
        /// <param name="offset"></param>
        /// <param name="wordcount"></param>
        /// <param name="is_oneshot"></param>
        /// <param name="first_read"></param>
        public void AddDevice(string name, DM.AccessType type, byte devicecode, int offset, int wordcount, bool is_oneshot, bool first_read)
        {
            AddDevice(name, type, devicecode, offset, wordcount, is_oneshot, first_read, 5000);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="devicecode"></param>
        /// <param name="offset"></param>
        /// <param name="wordcount"></param>
        /// <param name="is_oneshot"></param>
        public void AddDevice(string name, DM.AccessType type, byte devicecode, int offset, int wordcount, bool is_oneshot)
        {
            AddDevice(name, type, devicecode, offset, wordcount, is_oneshot, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="devicecode"></param>
        /// <param name="offset"></param>
        /// <param name="wordcount"></param>
        public void AddDevice(string name, DM.AccessType type, byte devicecode, int offset, int wordcount)
        {
            AddDevice(name, type, devicecode, offset, wordcount, false);
        }

        #endregion

        #region デバイス定義削除メソッド

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void RemoveDevice(string name)
        {
            for (int i = 0; i < Mem.Count; i++)
            {
                if (Mem[i].Name == name)
                {
                    Mem[i].Dispose();
                    Mem.RemoveAt(i);
                    break;
                }
            }
        }

        #endregion

        #region 通信制御開始

        /// <summary>
        /// 通信制御開始
        /// </summary>
        /// <param name="name">識別名</param>
        /// <param name="plc">PLC接続インスタンス</param>
        /// <param name="ipAddress">接続先IPアドレス</param>
        /// <param name="port">接続先ポート</param>
        /// <param name="sleep">ループ時のスリープ時間指定</param>
        /// <param name="priority">スレッド優先度</param>
        /// <returns></returns>
        public string StartControl(string name, BL_PLC plc, string ipAddress, int port, int sleep, System.Threading.ThreadPriority priority)
        {
            this.IpAddress = ipAddress;
            this.Port = port;
            this.plc = plc;

            if (plc == null || 0 <= IpAddress.ToUpper().IndexOf("DEBUG"))
            {
                isDebug = true;
            }

            m_Step = -1;

            return base.StartControl(sleep, priority);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool Wait_Initialized()
        {
            BL_Stopwatch sw = new BL_Stopwatch();
            sw.Start();

            while (true)
            {
                if (10000 < sw.ElapsedMilliseconds) break;

                if (Initialized) return true;

                BL_Win32API.Sleep(200);
            }

            return false;
        }

        #endregion

        #region 通信制御終了

        /// <summary>
        /// 通信制御終了
        /// </summary>
        public override void StopControl()
        {
            initializer.Initialized = false;
            initializer.WriteMemory();

            base.StopControl();

            foreach (DM dm in Mem)
            {
                dm.Dispose();
            }

            Mem.Clear();

            initializer.Dispose();
        }

        #endregion

        #region 通信制御スレッド

        private bool change_step = true;

        private int NextStep()
        {
            int step = m_Step;
            DM dm = null;

            for (int i = 0; i < Mem.Count; i++)
            {
                step = (step + 1) % Mem.Count;

                if (!Mem[step].EnableNotify) continue;

                dm = Mem[step];

                dm.Lock();
                dm.ReadMemory("RWType", false);
                dm.ReadMemory("IsReadBeforeWrite", false);
                dm.ReadMemory("IsWriteDataReady", false);
                dm.Unlock();

                if (dm.RWType == DM.AccessType.WRITE && !dm.IsWriteDataReady && !dm.IsReadBeforeWrite) continue;

                break;
            }

            return step;
        }

        /// <summary>
        /// 通信制御スレッド
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        protected override bool DoControl(object message)
        {
            bool canexit = true;
            if (Mem.Count == 0) return canexit;

            if (change_step)
            {
                change_step = false;

                int next_step = NextStep();

                #region 同期処理

                if (next_step <= m_Step)
                {
                    initializer.Initialized = true;
                    initializer.WriteMemory();

                    for (int count = 0; count < Mem.Count; count++)
                    {
                        Mem[count].ReadMemory("RWType");
                    }

                    for (int count = 0; count < Mem.Count; count++)
                    {
                        if (Mem[count].EnableNotify)
                        {
                            if (Mem[count].RWType == DM.AccessType.READ_SYNC)
                            {
                                Mem[count].Lock();
                            }
                        }
                    }

                    for (int count = 0; count < Mem.Count; count++)
                    {
                        if (Mem[count].EnableNotify)
                        {
                            if (Mem[count].RWType == DM.AccessType.READ_SYNC)
                            {
                                Array.Copy(Mem[count].memory.data_temp, Mem[count].memory.data, Mem[count].memory.data.Length);
                                Mem[count].WriteMemory("memory.data", false);
                            }
                        }
                    }

                    for (int count = 0; count < Mem.Count; count++)
                    {
                        if (Mem[count].EnableNotify)
                        {
                            if (Mem[count].RWType == DM.AccessType.READ_SYNC)
                            {
                                Mem[count].Unlock();
                            }
                        }
                    }

                    if (EventSyncronized != null) EventSyncronized(this);
                }

                #endregion

                m_Step = next_step;
            }

            DM dm = Mem[m_Step];

            if (dm != null)
            {
                dm.Lock();
                dm.ReadMemory("Step", false);
                dm.ReadMemory("RWType", false);
                dm.ReadMemory("Connected", false);
                dm.ReadMemory("IsOneshot", false);
                dm.ReadMemory("IsReadBeforeWrite", false);
                dm.ReadMemory("IsWriteDataReady", false);
                dm.ReadMemory("TimeoutMilliseconds", false);
                dm.Unlock();

                switch (dm.Step)
                {
                    case 0:
                        if (isDebug)

                        #region DEBUG
                        {
                            if (!initializer.IsConnected)
                            {
                                initializer.IsConnected = true;
                                initializer.WriteMemory("IsConnected");
                            }

                            if (dm.RWType == DM.AccessType.READ || dm.RWType == DM.AccessType.READ_SYNC || (dm.RWType == DM.AccessType.WRITE && dm.IsReadBeforeWrite && !dm.IsReadBeforeWriteComplete))
                            {
                                Log(dm.ToString() + " READ(DEBUG)");

                                m_swTemp.Reset(); m_swTemp.Start();
                                dm.Step = 100;
                            }
                            else if (dm.RWType == DM.AccessType.WRITE)
                            {
                                dm.ReadMemory("memory.data");
                                Array.Copy(dm.memory.data, dm.memory.data_temp, dm.memory.data.Length);

                                //DEBUG
                                {
                                    string debug = "";
                                    foreach (byte d in dm.memory.data) debug += d.ToString("X2");
                                    Log(dm.ToString() + " W(DEBUG):[" + debug + "]");
                                }

                                m_swTemp.Reset(); m_swTemp.Start();
                                dm.Step = 200;
                            }
                        }
                        #endregion

                        else if (plc.IsConnected)
                        {
                            if (dm.RWType == DM.AccessType.READ || dm.RWType == DM.AccessType.READ_SYNC || (dm.RWType == DM.AccessType.WRITE && dm.IsReadBeforeWrite && !dm.IsReadBeforeWriteComplete))
                            {
                                if (plc.ReadCommand(dm.DeviceCode, dm.Offset, dm.WordCount))
                                {
                                    m_swTemp.Reset(); m_swTemp.Start();
                                    dm.Step = 100;
                                }
                                else
                                {
                                    dm.Step = 900;
                                }
                            }
                            else if (dm.RWType == DM.AccessType.WRITE)
                            {
                                dm.ReadMemory("memory.data");
                                Array.Copy(dm.memory.data, dm.memory.data_temp, dm.memory.data.Length);

                                if (dm.IsOneshot)
                                {
                                    string debug = "";
                                    foreach (byte d in dm.memory.data_temp) debug += d.ToString("X2");
                                    Log(dm.ToString() + " WRITE ONESHOT:[" + debug + "]");
                                }

                                if (plc.WriteCommand(dm.DeviceCode, dm.Offset, dm.WordCount, dm.memory.data_temp))
                                {
                                    m_swTemp.Reset(); m_swTemp.Start();
                                    dm.Step = 200;
                                }
                                else
                                {
                                    dm.Step = 900;
                                }
                            }
                        }
                        else
                        {
                            if (plc.Connect(IpAddress, Port))
                            {
                                initializer.IsConnected = true;
                                initializer.WriteMemory("IsConnected");

                                Log("CONNECTED - [" + IpAddress + ":" + Port.ToString() + "]");

                                if (EventConnected != null) EventConnected(this);
                            }
                        }
                        break;

                    case 100:
                        if (isDebug)

                        #region DEBUG
                        {
                            dm.debugCounter = (byte)(DateTime.Now.Second % 60);

                            byte[] data = new byte[dm.WordCount * 2];
                            for (int i = 0; i < data.Length; i += 2) data[i] = dm.debugCounter;

                            if (dm.RWType == DM.AccessType.READ || dm.RWType == DM.AccessType.WRITE)
                            {
                                Array.Copy(data, dm.memory.data, data.Length);
                                dm.WriteMemory("memory.data");

                                if (EventReaded != null) EventReaded(this, dm);
                            }
                            else
                            {
                                Array.Copy(data, dm.memory.data_temp, data.Length);
                            }

                            //DEBUG
                            {
                                string debug = "";
                                foreach (byte d in data) debug += d.ToString("X2");
                                Log(dm.ToString() + " R(DEBUG):[" + debug + "]");
                            }

                            if (dm.swRefresh.IsRunning) dm.RefreshedInterval = dm.swRefresh.ElapsedMilliseconds;
                            dm.swRefresh.Restart();

                            dm.Step = 0;

                            if (dm.RWType == DM.AccessType.READ || dm.RWType == DM.AccessType.READ_SYNC)
                            {
                                if (dm.IsOneshot) dm.StopNotify();
                                change_step = true;
                            }
                            else
                            {
                                dm.IsReadBeforeWriteComplete = true;
                            }
                        }
                        #endregion

                        else
                        {
                            int readpoints = 0;
                            byte[] data = plc.ReadCommandAck(out readpoints);

                            if (readpoints > 0)
                            {
                                if (dm.RWType == DM.AccessType.READ)
                                {
                                    Array.Copy(data, dm.memory.data, data.Length);
                                    dm.WriteMemory("memory.data");

                                    if (EventReaded != null) EventReaded(this, dm);
                                }
                                else
                                {
                                    Array.Copy(data, dm.memory.data_temp, data.Length);
                                }

                                ////DEBUG
                                //{
                                //    string debug = "";
                                //    foreach (byte d in data) debug += d.ToString("X2");
                                //    Log(dm.ToString() + " R:[" + debug + "]");
                                //}

                                if (dm.swRefresh.IsRunning) dm.RefreshedInterval = dm.swRefresh.ElapsedMilliseconds;
                                dm.swRefresh.Restart();

                                dm.Step = 0;

                                if (dm.RWType == DM.AccessType.READ || dm.RWType == DM.AccessType.READ_SYNC)
                                {
                                    if (dm.IsOneshot) dm.StopNotify();
                                    change_step = true;
                                }
                                else
                                {
                                    dm.IsReadBeforeWriteComplete = true;
                                }
                            }
                            else if (dm.TimeoutMilliseconds < m_swTemp.ElapsedMilliseconds)
                            {
                                Log("TIMEOUT READ - " + dm.Name);
                                dm.Step = 900;

                                if (EventError != null) EventError(this);
                            }
                        }
                        break;

                    case 200:
                        if (isDebug)
                        {
                            if (dm.swRefresh.IsRunning) dm.RefreshedInterval = dm.swRefresh.ElapsedMilliseconds;
                            dm.swRefresh.Restart();

                            if (EventWrited != null) EventWrited(this, dm);
                            if (dm.IsOneshot) Log(dm.ToString() + " WRITED ONESHOT(DEBUG)");

                            dm.Step = 0;

                            if (dm.IsOneshot) dm.StopNotify();
                            change_step = true;
                        }
                        else
                        {
                            byte[] data = plc.WriteCommandAck();

                            if (data.Length > 0)
                            {
                                if (dm.swRefresh.IsRunning) dm.RefreshedInterval = dm.swRefresh.ElapsedMilliseconds;
                                dm.swRefresh.Restart();

                                if (EventWrited != null) EventWrited(this, dm);
                                if (dm.IsOneshot) Log(dm.ToString() + " WRITED ONESHOT");

                                dm.Step = 0;

                                if (dm.IsOneshot) dm.StopNotify();
                                change_step = true;
                            }
                            else if (dm.TimeoutMilliseconds < m_swTemp.ElapsedMilliseconds)
                            {
                                dm.Step = 900;
                                Log("TIMEOUT WRITE - " + dm.Name);

                                if (EventError != null) EventError(this);
                            }
                        }
                        break;

                    case 900:
                        {
                            initializer.IsConnected = false;
                            initializer.WriteMemory("IsConnected");

                            plc.Disconnect();
                            Log("DISCONNECTED - [" + IpAddress + ":" + Port.ToString() + "]");

                            if (EventDisconnected != null) EventDisconnected(this);

                            dm.Step = 0;
                            change_step = true;
                        }
                        break;
                }

                if (isDebug) dm.Connected = true;
                else dm.Connected = plc.IsConnected;

                dm.Lock();
                dm.WriteMemory("Step", false);
                dm.WriteMemory("Connected", false);
                dm.WriteMemory("IsReadBeforeWriteComplete", false);
                dm.WriteMemory("RefreshedInterval", false);
                dm.Unlock();
            }

            return canexit;
        }

        #endregion

        #region データ変換

        /// <summary>
        /// デバイスアドレス同期用共有メモリから符号付き１６ビット値を取得します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <returns></returns>
        public Int16 ToInt16(int device_address)
        {
            DM m = GetReaderMemory(device_address);
            if (m != null)
            {
                return m.ToInt16(device_address);
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリへ符号付き１６ビット値を設定します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <param name="value">設定値</param>
        public DM ToInt16(int device_address, Int16 value)
        {
            DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToInt16(device_address, value);
                return m;
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリから符号付き３２ビット値を取得します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <returns></returns>
        public Int32 ToInt32(int device_address)
        {
            DM m = GetReaderMemory(device_address);
            if (m != null)
            {
                return m.ToInt32(device_address);
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリへ符号付き３２ビット値を設定します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <param name="value">設定値</param>
        public DM ToInt32(int device_address, Int32 value)
        {
            DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToInt32(device_address, value);
                return m;
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリから符号付き６４ビット値を取得します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <returns></returns>
        public Int64 ToInt64(int device_address)
        {
            DM m = GetReaderMemory(device_address);
            if (m != null)
            {
                return m.ToInt64(device_address);
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリへ符号付き６４ビット値を設定します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <param name="value">設定値</param>
        public DM ToInt64(int device_address, Int64 value)
        {
            DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToInt64(device_address, value);
                return m;
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリから符号無し１６ビット値を取得します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <returns></returns>
        public UInt16 ToUInt16(int device_address)
        {
            DM m = GetReaderMemory(device_address);
            if (m != null)
            {
                return m.ToUInt16(device_address);
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリへ符号無し１６ビット値を設定します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <param name="value">設定値</param>
        public DM ToUInt16(int device_address, UInt16 value)
        {
            DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToUInt16(device_address, value);
                return m;
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリから符号無し３２ビット値を取得します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <returns></returns>
        public UInt32 ToUInt32(int device_address)
        {
            DM m = GetReaderMemory(device_address);
            if (m != null)
            {
                return m.ToUInt32(device_address);
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリへ符号無し３２ビット値を設定します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <param name="value">設定値</param>
        public DM ToUInt32(int device_address, UInt32 value)
        {
            DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToUInt32(device_address, value);
                return m;
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリから符号無し６４ビット値を取得します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <returns></returns>
        public UInt64 ToUInt64(int device_address)
        {
            DM m = GetReaderMemory(device_address);
            if (m != null)
            {
                return m.ToUInt64(device_address);
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリへ符号無し６４ビット値を設定します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <param name="value">設定値</param>
        public DM ToUInt64(int device_address, UInt64 value)
        {
            DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToUInt64(device_address, value);
                return m;
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリから文字列を取得します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <param name="length">文字列長</param>
        /// <returns></returns>
        public string ToString(int device_address, int length)
        {
            DM m = GetReaderMemory(device_address);
            if (m != null)
            {
                return m.ToString(device_address, length);
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリへ文字列を設定します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <param name="length">文字列長</param>
        /// <param name="value">設定文字列</param>
        /// <returns></returns>
        public DM ToString(int device_address, int length, string value)
        {
            DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToString(device_address, length, value);
                return m;
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        #endregion
    }
}
