using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using BelicsClass.Common;
using BelicsClass.ObjectSync;

namespace BelicsClass.PLC
{
    /// <summary>
    /// PLCデータメモリと同期している共有メモリにアクセスするためのクラスです
    /// </summary>
    public class BL_PLCDataAccesser
    {
        private BL_PLCDataSync.Initializer initializer = new BL_PLCDataSync.Initializer();

        /// <summary></summary>
        public bool Initialized
        {
            get
            {
                initializer.ReadMemory("Initialized");
                return initializer.Initialized;
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
        public List<BL_PLCDataSync.DM> Mem = new List<BL_PLCDataSync.DM>();

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public BL_PLCDataAccesser()
        {
            initializer.Initialize<BL_PLCDataSync.Initializer>("PLCDATASYNC_INITIALIZER");
        }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public BL_PLCDataAccesser(string name)
        {
            initializer.Initialize<BL_PLCDataSync.Initializer>(name + "_INITIALIZER");
        }

        /// <summary>
        /// DMメモリ定義を追加します
        /// </summary>
        /// <param name="name"></param>
        /// <param name="wordcount"></param>
        public BL_PLCDataSync.DM AddDevice(string name, int wordcount)
        {
            BL_PLCDataSync.DM dm = new BL_PLCDataSync.DM(wordcount);
            Mem.Add(dm);

            dm.Initialize<BL_PLCDataSync.DM>(name);
            return dm;
        }

        /// <summary>
        /// DMメモリ定義を削除します
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
                    i--;
                }
            }
        }

        /// <summary>
        /// DMメモリ定義を取得します
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public BL_PLCDataSync.DM this[int index]
        {
            get
            {
                return Mem[index];
            }
        }

        /// <summary>
        /// DM定義を全て破棄します
        /// </summary>
        public void Dispose()
        {
            foreach (BL_PLCDataSync.DM dm in Mem)
            {
                dm.Dispose();
            }

            Mem.Clear();

            initializer.Dispose();
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

        /// <summary>
        /// 書き込みデータの準備完了をセットします
        /// </summary>
        /// <param name="device_address"></param>
        public void SetWriteDataReady(int device_address)
        {
            BL_PLCDataSync.DM dm = GetWriterMemory(device_address);

            dm.IsWriteDataReady = true;
            dm.WriteMemory("IsWriteDataReady");
        }

        #region データ変換

        /// <summary>
        /// 
        /// </summary>
        /// <param name="device_address"></param>
        /// <returns></returns>
        public BL_PLCDataSync.DM GetReaderMemory(int device_address)
        {
            foreach (var m in Mem)
            {
                if ((m.RWType == BL_PLCDataSync.DM.AccessType.READ || m.RWType == BL_PLCDataSync.DM.AccessType.READ_SYNC) && m.Offset <= device_address && device_address < m.Offset + m.WordCount)
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
        public BL_PLCDataSync.DM GetWriterMemory(int device_address)
        {
            foreach (var m in Mem)
            {
                if (m.RWType == BL_PLCDataSync.DM.AccessType.WRITE && m.Offset <= device_address && device_address < m.Offset + m.WordCount)
                {
                    return m;
                }
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        /// <summary>
        /// デバイスアドレス同期用共有メモリから符号付き１６ビット値を取得します。
        /// </summary>
        /// <param name="device_address">デバイスアドレス</param>
        /// <returns></returns>
        public Int16 ToInt16(int device_address)
        {
            BL_PLCDataSync.DM m = GetReaderMemory(device_address);
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
        public void ToInt16(int device_address, Int16 value)
        {
            BL_PLCDataSync.DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToInt16(device_address, value);
                return;
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
            BL_PLCDataSync.DM m = GetReaderMemory(device_address);
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
        public void ToInt32(int device_address, Int32 value)
        {
            BL_PLCDataSync.DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToInt32(device_address, value);
                return;
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
            BL_PLCDataSync.DM m = GetReaderMemory(device_address);
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
        public void ToInt64(int device_address, Int64 value)
        {
            BL_PLCDataSync.DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToInt64(device_address, value);
                return;
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
            BL_PLCDataSync.DM m = GetReaderMemory(device_address);
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
        public void ToUInt16(int device_address, UInt16 value)
        {
            BL_PLCDataSync.DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToUInt16(device_address, value);
                return;
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
            BL_PLCDataSync.DM m = GetReaderMemory(device_address);
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
        public void ToUInt32(int device_address, UInt32 value)
        {
            BL_PLCDataSync.DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToUInt32(device_address, value);
                return;
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
            BL_PLCDataSync.DM m = GetReaderMemory(device_address);
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
        public void ToUInt64(int device_address, UInt64 value)
        {
            BL_PLCDataSync.DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToUInt64(device_address, value);
                return;
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
            BL_PLCDataSync.DM m = GetReaderMemory(device_address);
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
        public void ToString(int device_address, int length, string value)
        {
            BL_PLCDataSync.DM m = GetWriterMemory(device_address);
            if (m != null)
            {
                m.ToString(device_address, length, value);
                return;
            }

            throw new Exception("対象アドレスの設定が存在しません");
        }

        #endregion
    }
}
