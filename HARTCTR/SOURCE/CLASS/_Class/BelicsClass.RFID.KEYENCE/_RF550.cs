using System;
using System.Collections.Generic;
using System.Text;

using BelicsClass.Network;

namespace BelicsClass.RFID
{
    /// <summary>
    /// KEYENCE RDIDコントローラ RF550のコマンド送受信を行う機能を提供します
    /// </summary>
    public class BL_RF550
    {
        #region 列挙型
        
        private enum DeviceType
        {
            NONE,
            LAN,
        }

        #endregion

        #region フィールド
        
        private DeviceType Device = DeviceType.NONE;
        private BL_RawSocket Socket;
        private int Error_code;
        private string Error_message;

        #endregion

        #region RFIDと接続します。

        /// <summary>
		/// RFIDと接続します。
		/// </summary>
		/// <param name="ip_address">接続するRFIDのIPアドレス。</param>
		/// <param name="port">接続するRFIDのポート№。</param>
		/// <returns></returns>
		public bool Connect(string ip_address, int port)
		{
			bool	status = false;

			errors();

			if (Device == DeviceType.NONE)
			{
                Socket = new BL_RawSocket(ip_address, port);

                status = Socket.Open(BL_RawSocket.OpenMode.Client, BL_RawSocket.FormatType.CR);
			
				if (status)
				{
					Device = DeviceType.LAN;
                    while (Socket.Receive() != "") ;
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

        #region RFIDとの接続を解除します。
        
        /// <summary>
		/// PLCとの接続を解除します。
		/// </summary>
		public void Disconnect()
		{
			if (Device == DeviceType.LAN)
			{
				Socket.Close();

				Socket = null;
			}

			Device = DeviceType.NONE;
		}

        #endregion

        #region 接続状態プロパティ
        
        /// <summary>
        /// ソケットの接続状態を取得します。接続している場合は true。それ以外の場合は false。
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (Device == DeviceType.LAN)
                {
                    return Socket.IsConnected;
                }
                else
                {
                    return false;
                }
            }
        }

        #endregion

        #region 異常コードプロパティ
        
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

        #region 異常内容プロパティ
		 
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

        #region 読み込みコマンドの登録
        
        /// <summary>
        /// 読み込みコマンドの登録
        /// </summary>
        /// <param name="address"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public bool ReadCommand(int address, int size)
        {
            bool status = false;

            if (Device == DeviceType.LAN)
            {
                string send = "RD" + address.ToString("0000") + size.ToString("0000");
                send = (send.Length + 4).ToString("0000") + send;

                status = Socket.Send(send);

                if (!status)
                {
                    errors(Socket.ErrorCode, Socket.ErrorMessage);
                }
            }
            return status;
        }

        #endregion

        #region 書き込みコマンドの登録
        
        /// <summary>
        /// 書き込みコマンドの登録
        /// </summary>
        /// <param name="address"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool WriteCommand(int address, string data)
        {
            bool status = false;

            if (Device == DeviceType.LAN)
            {
                string send = "WD" + address.ToString("0000") + (data.Length).ToString("0000") + data;
                send = (send.Length + 4).ToString("0000") + send;

                status = Socket.Send(send);

                if (!status)
                {
                    errors(Socket.ErrorCode, Socket.ErrorMessage);
                }
            }
            return status;
        }

        #endregion

        #region レスポンスデータの評価と登録
        
        /// <summary>
        /// レスポンスデータの評価と登録
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public string CommandAck(out int length)
        {
            length = -1;
            string data = "";

            if (Device == DeviceType.LAN)
            {
                data = Socket.Receive();

                if (2 == data.Length)
                {
                    length = data.Length;
                    return data;
                }
                if (4 <= data.Length)
                {
                    string hlen = data.Substring(0,4);
                    int len = 0;
                    if (!int.TryParse(hlen, out len))
                    {
                        length = data.Length;
                        return data;
                    }

                    len -= 4;
                    string hdata = data.Substring(4);

                    if (hdata.Substring(0, 2) == "RD")
                    {
                        if (hdata.Substring(2, 2) == "00")
                        {
                            data = hdata.Substring(8);
                            length = data.Length;
                            return data;
                        }
                        else
                        {
                            length = hdata.Length;
                            return hdata;
                        }
                    }
                    if (hdata.Substring(0, 2) == "WD")
                    {
                        data = hdata.Substring(2);
                        length = data.Length;
                        return data;
                    }

                    //length = hdata.Length;
                    //data = hdata;
                    //return data;
                    return "";
                }
            }

            return data;
        }

        #endregion

    }
}
