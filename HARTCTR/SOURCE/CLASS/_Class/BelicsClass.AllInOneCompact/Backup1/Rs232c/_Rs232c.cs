using System;
using System.Text;
using System.Threading;
using System.Runtime.InteropServices;

namespace BelicsClass.Rs232c
{
    /// <summary>
    /// RS-232Cクラス
    /// </summary>
    public class BL_Rs232c
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

        private Win32Control win32_control = new Win32Control();
        private bool Event_enable = false;
        private Thread Sub_thread = null;
        private FormatType Format_type;
        private int Error_code;
        private string Error_message;
        private int Receive_count = 0;
        private byte[] Receive_data = new byte[4096];

        private bool isOpened = false;
        private PortNo portno = (PortNo)0;
        private bool formatKindInclude = false;

        /// <summary>
        /// オープン中かどうかを取得します
        /// </summary>
        public bool IsOpened { get { return isOpened; } }

        /// <summary>
        /// オープン中のポートNoを取得します(1～20)
        /// 未オープンの場合は0を返します
        /// </summary>
        public PortNo OpeningPortNo { get { return portno; } }

        /// <summary>
        /// 取得データにフォーマットコードを含めるかどうかを取得または設定します
        /// </summary>
        public bool FormatKindInclude { get { return formatKindInclude; } set { formatKindInclude = value; } }

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
        public BL_Rs232c()
        {
            Event_enable = false;
        }
        /// <summary>
        /// RS-232Cクラス
        /// </summary>
        /// <param name="eventEnable">イベント型にする場合は true。それ以外の場合は false。</param>
        public BL_Rs232c(bool eventEnable)
        {
            Event_enable = eventEnable;
        }

        //====================================================================================================
        // デストラクタ
        //====================================================================================================
        /// <summary>
        ///  RS-232Cクラス
        /// </summary>
        ~BL_Rs232c()
        {
            this.Close();
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
            bool status;
            int error;

            errors();

            status = win32_control.Connect(portNo, baudRate, byteSize, parity, stopBits);

            if (status)
            {
                Format_type = formatType;

                Receive_count = 0;
                Sub_thread = null;

                if (Event_enable)
                {
                    if (Sub_thread == null)
                    {
                        Sub_thread = new Thread(new ThreadStart(receive_check));
                        Sub_thread.Start();
                    }
                }

                isOpened = true;
                portno = portNo;
            }
            else
            {
                error = win32_control.GetError();

                if (error == 0) errors(-1, "すでに使用されています。");
                else errors(error, "Win32 API Error : " + error.ToString());
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
            win32_control.Disconnect();

            if (Sub_thread != null)
            {
                Sub_thread.Abort();
                Sub_thread.Join();

                Sub_thread = null;
            }

            isOpened = false;
            portno = (PortNo)0;
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
        /// デバイスからデータを読み込みます。　フォーマットコードを含めません
        /// </summary>
        /// <returns></returns>
        public string Receive()
        {
            return Encoding.Default.GetString(ReceiveBytes());
        }

        /// <summary>
        /// デバイスからデータを読み込みます。　フォーマットコードを含めます
        /// </summary>
        /// <returns></returns>
        public string ReceiveAll()
        {
            return Encoding.Default.GetString(ReceiveBytesAll());
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
                    //send_data = new byte[data.Length + 2];

                    //Array.Copy(data, 0, send_data, 0, data.Length);
                    //send_data[data.Length] = 0x0D;
                    //send_data[data.Length + 1] = 0x0A;
                    //break;

                    //@@@@@
                    send_data = new byte[data.Length + 5];

                    send_data[0] = 0x05;  //@@@@@
                    Array.Copy(data, 0, send_data, 1, data.Length);

                    //@@@@ サムチェック
                    int sum = 0;
                    for (int ii = 0; ii < data.Length; ii++)
                    {
                        sum += data[ii];
                    }
                    string sSum = Convert.ToString(sum, 16);
                    if (sSum.Length == 1)
                    {
                        sSum = "0" + sSum;
                    }
                    else
                    {
                        sSum = sSum.Substring(sSum.Length - 2, 2);
                    }
                    sSum = sSum.ToUpper();
                    send_data[data.Length + 1] = (byte)((int)sSum[0]);
                    send_data[data.Length + 2] = (byte)((int)sSum[1]);

                    send_data[data.Length + 3] = 0x0D;
                    send_data[data.Length + 4] = 0x0A;
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
        /// デバイスからバイト配列でデータを読み込みます。フォーマットコードを含めません
        /// </summary>
        /// <param name="bytes_size">受信したバイト数。未受信のときは、-1。</param>
        /// <returns></returns>
        public byte[] ReceiveBytes(out int bytes_size)
        {
            byte[] receive_data = new byte[4096], temp;
            bool flag = false;
            int count = 0, length = 0, check = 0, pos = 0;

            lock (this)
            {
                if (!Event_enable)
                {
                    while (true)
                    {
                        count = win32_control.Read(out temp, 1);

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
        /// デバイスからバイト配列でデータを読み込みます。フォーマットコードを含めます
        /// </summary>
        /// <param name="bytes_size">受信したバイト数。未受信のときは、-1。</param>
        /// <returns></returns>
        public byte[] ReceiveBytesAll(out int bytes_size)
        {
            byte[] receive_data = ReceiveBytes(out bytes_size);
            byte[] temp = new byte[0];

            if (0 <= bytes_size)
            {
                switch (Format_type)
                {
                    case FormatType.CR:
                        temp = new byte[receive_data.Length + 1];
                        Array.Copy(receive_data, 0, temp, 0, receive_data.Length);
                        temp[receive_data.Length] = 0x0D;
                        bytes_size += 1;
                        break;

                    case FormatType.CR_LF:
                        temp = new byte[receive_data.Length + 2];
                        Array.Copy(receive_data, 0, temp, 0, receive_data.Length);
                        temp[receive_data.Length] = 0x0D;
                        temp[receive_data.Length + 1] = 0x0A;
                        bytes_size += 2;
                        break;

                    case FormatType.STX_ETX:
                        temp = new byte[receive_data.Length + 2];
                        Array.Copy(receive_data, 0, temp, 1, receive_data.Length);
                        temp[0] = 0x02;
                        temp[receive_data.Length] = 0x03;
                        bytes_size += 2;
                        break;

                    default:
                        temp = new byte[receive_data.Length];
                        Array.Copy(receive_data, 0, temp, 0, receive_data.Length);
                        break;
                }
            }

            return temp;


            //? kano:以下のコードでは、ReceiveBytesDirectと同じ結果であるため上記コードに変更しました

            //byte[] receive_data = new byte[4096], temp;
            //bool flag = false;
            //int count = 0, length = 0;

            //lock (this)
            //{
            //    if (!Event_enable)
            //    {
            //        while (true)
            //        {
            //            count = win32_control.Read(out temp, 1);

            //            if (count == 1 && Receive_data.Length > Receive_count)
            //            {
            //                Receive_data[Receive_count++] = temp[0];
            //            }
            //            else
            //            {
            //                break;
            //            }
            //        }
            //    }

            //    if (Receive_count > 0)
            //    {
            //        length = Receive_count;
            //        Array.Copy(Receive_data, 0, receive_data, 0, Receive_count);
            //        Array.Clear(Receive_data, 0, Receive_data.Length);
            //        Receive_count = 0;

            //        flag = true;
            //    }
            //}

            //if (flag) bytes_size = length;
            //else bytes_size = -1;

            //temp = new byte[length];
            //Array.Copy(receive_data, 0, temp, 0, length);

            //return temp;
        }
        /// <summary>
        /// デバイスからバイト配列でデータを読み込みます。フォーマットコードを含めません
        /// </summary>
        /// <returns></returns>
        public byte[] ReceiveBytes()
        {
            int count;

            return ReceiveBytes(out count);
        }
        /// <summary>
        /// デバイスからバイト配列でデータを読み込みます。フォーマットコードを含めます
        /// </summary>
        /// <returns></returns>
        public byte[] ReceiveBytesAll()
        {
            int count;

            return ReceiveBytesAll(out count);
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
            int count;

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

                count = win32_control.Write(temp, temp.Length);

                if (count != temp.Length)
                {
                    errors(-100, "書き込みに失敗しました。");

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
            byte[] receive_data = new byte[4096], temp;
            int count = 0, length = 0;

            lock (this)
            {
                if (!Event_enable)
                {
                    while (true)
                    {
                        count = win32_control.Read(out temp, 1);

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
        private void receive_check()
        {
            byte[] temp;
            bool check;
            int count = 0;

            while (true)
            {
                check = false;

                lock (this)
                {
                    while (true)
                    {
                        count = win32_control.Read(out temp, 1);

                        if (count == 1 && Receive_data.Length > Receive_count)
                        {
                            Receive_data[Receive_count++] = temp[0];

                            check = true;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (check)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback(ReceiveCallback));
                }

                Thread.Sleep(2);
            }
        }

        //====================================================================================================
        // 受信（コールバック）
        //====================================================================================================
        private void ReceiveCallback(Object stateInfo)
        {
            byte[] data;

            if (formatKindInclude)
            {
                data = this.ReceiveBytesAll();
            }
            else
            {
                data = this.ReceiveBytes();
            }

            if (data.Length > 0)
            {
                ReceiveEventArgs e = new ReceiveEventArgs();

                e.bytes_data = data;

                if (this.ReceiveEvent != null) this.ReceiveEvent(this, e);
            }
        }

        //====================================================================================================
        // Win32Control CLass
        //====================================================================================================
        private class Win32Control
        {
            #region Win32API
            [DllImport("kernel32", SetLastError = true)]
            private static extern IntPtr CreateFile(string filename, uint desiredAccess, uint shareMode, uint attributes, uint creationDisposition, uint flagsAndAttributes, uint templateFile);

            [DllImport("kernel32", SetLastError = true)]
            private static extern bool CloseHandle(IntPtr hFile);

            [DllImport("kernel32", SetLastError = true)]
            private static extern bool GetCommModemStatus(IntPtr hFile, ref uint lpdWord);

            [DllImport("kernel32", SetLastError = true)]
            private static extern bool GetCommState(IntPtr hFile, ref DCB dcb);

            [DllImport("kernel32", SetLastError = true)]
            private static extern bool SetCommState(IntPtr hFile, ref DCB dcb);

            [DllImport("kernel32", SetLastError = true)]
            private static extern bool SetCommTimeouts(IntPtr hFile, ref COMMTIMEOUTS commouts);

            [DllImport("kernel32", SetLastError = true)]
            private static extern bool EscapeCommFunction(IntPtr hFile, uint fdwFunc);

            [DllImport("kernel32", SetLastError = true)]
            private static extern unsafe bool ReadFile(IntPtr hFile, void* lpBuffer, int nBytesToRead, int* nBytesRead, int overlapped);

            [DllImport("kernel32", SetLastError = true)]
            private static extern unsafe bool WriteFile(IntPtr hFile, void* lpBuffer, int nBytesToRead, int* nBytesRead, int overlapped);

            [StructLayout(LayoutKind.Sequential)]
            private struct DCB
            {
                public uint DCBlength;
                public uint BaudRate;
                public uint flags;
                public ushort wReserved;
                public ushort XonLim;
                public ushort XoffLim;
                public byte ByteSize;
                public byte Parity;
                public byte StopBits;
                public byte XonChar;
                public byte XoffChar;
                public byte ErrorChar;
                public byte EofChar;
                public byte EvtChar;
                public ushort wReserved1;
            };

            [StructLayout(LayoutKind.Sequential)]
            private struct OVERLAPPED
            {
                public uint Internal;
                public uint InternalHigh;
                public uint Offset;
                public uint OffsetHigh;
                public int hEvent;
            };

            [StructLayout(LayoutKind.Sequential)]
            private struct COMMTIMEOUTS
            {
                public uint ReadIntervalTimeout;
                public uint ReadTotalTimeoutMultiplier;
                public uint ReadTotalTimeoutConstant;
                public uint WriteTotalTimeoutMultiplier;
                public uint WriteTotalTimeoutConstant;
            };
            #endregion

            private IntPtr handle = IntPtr.Zero;

            internal bool Connect(PortNo portNo, BaudRate baudRate, ByteSize byteSize, Parity parity, StopBits stopBits)
            {
                const uint GENERIC_READ = 0x80000000;
                const uint GENERIC_WRITE = 0x40000000;
                const uint OPEN_EXISTING = 3;
                const uint SETRTS = 3;
                const uint SETDTR = 5;

                DCB dcb = new DCB();
                COMMTIMEOUTS timeouts = new COMMTIMEOUTS();
                string port;

                if (handle != IntPtr.Zero) return false;

                if (portNo < PortNo._10)
                {
                    port = String.Format("COM{0:d}", (int)portNo);
                }
                else
                {
                    port = String.Format(@"\\.\COM{0:d}", (int)portNo);
                }

                handle = CreateFile(port, GENERIC_READ | GENERIC_WRITE, 0, 0, OPEN_EXISTING, 0, 0);

                if (handle == IntPtr.Zero) return false;

                if (!GetCommState(handle, ref dcb))
                {
                    Disconnect();

                    return false;
                }

                dcb.BaudRate = (uint)baudRate;
                dcb.ByteSize = (byte)byteSize;
                dcb.Parity = (byte)parity;
                dcb.StopBits = (byte)stopBits;

                if (!SetCommState(handle, ref dcb))
                {
                    Disconnect();

                    return false;
                }

                timeouts.ReadIntervalTimeout = 0xFFFFFFFF;
                timeouts.ReadTotalTimeoutMultiplier = 0;
                timeouts.ReadTotalTimeoutConstant = 0;
                timeouts.WriteTotalTimeoutMultiplier = 1;
                timeouts.WriteTotalTimeoutConstant = 200;

                SetCommTimeouts(handle, ref timeouts);

                EscapeCommFunction(handle, SETRTS);
                EscapeCommFunction(handle, SETDTR);

                return true;
            }

            internal void Disconnect()
            {
                if (handle != IntPtr.Zero)
                {
                    CloseHandle(handle);

                    handle = IntPtr.Zero;
                }
            }

            internal int GetError()
            {
                return Marshal.GetLastWin32Error();
            }

            internal unsafe int Read(out byte[] data, int count)
            {
                int length = 0;
                byte[] buffer = new byte[4096];

                data = new byte[0];

                fixed (byte* pointer = buffer)
                {
                    ReadFile(handle, pointer, count, &length, 0);
                }

                if (length > 0)
                {
                    data = new byte[length];

                    Array.Copy(buffer, 0, data, 0, length);
                }

                return length;
            }

            internal unsafe int Write(byte[] buffer, int count)
            {
                int length = 0;

                fixed (byte* pointer = buffer)
                {
                    WriteFile(handle, pointer, count, &length, 0);
                }

                return length;
            }
        }
    }
}
