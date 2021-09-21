using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BelicsClass.Common;
using BelicsClass.Network;
using BelicsClass.ObjectSync;

namespace AgvController
{
    public class AgvOrderCommunicator : IDisposable
    {
        public delegate void ReceiveEventHandler(AgvOrderCommunicator sender, string cmd, RequestBase req);
        public event ReceiveEventHandler ReceiveEvent;

        public enum enREQ
        {
            /// <summary>棚→ステーション搬送要求</summary>
            QRS,
            /// <summary>ＡＧＶ移動要求</summary>
            MOV,
            /// <summary>ステーション完了</summary>
            QSC,

            /// <summary>ステーション到着報告</summary>
            ASA,
            /// <summary>棚返却報告</summary>
            ARR,
            /// <summary>バッテリー残量報告</summary>
            BAT,
            /// <summary>アプリケーション終了要求</summary>
            FIN,
            /// <summary>アラーム要求</summary>
            ALM,
            /// <summary>リセット要求</summary>
            RST,
        }

        public enum enRESULT
        {
            /// <summary>要求</summary>
            RQ,
            /// <summary>到着</summary>
            OK,
            /// <summary>到着不可</summary>
            NG,
        }

        public class RequestBase : BL_ObjectSync
        {
            /// <summary>要求種別</summary>
            [BL_ObjectSync(Order = 1)]
            public string cmd = "".PadRight(3);

            /// <summary>電文No</summary>
            [BL_ObjectSync(Order = 2)]
            public string seqno = "".PadRight(5);

            /// <summary>応答／結果  OK:到着／NG：到達不可</summary>
            [BL_ObjectSync(Order = 3)]
            public string result = "".PadRight(2);

            public bool ack = false;

            public RequestBase()
            {
                Initialize();
            }

            public RequestBase(enREQ cmd, int seqno, enRESULT result)
            {
                Initialize();

                this.cmd = cmd.ToString();
                this.seqno = seqno.ToString("00000");
                this.result = result.ToString();
            }

            public override string ToString()
            {
                return cmd + "," + seqno + "," + result;
            }

            public virtual RequestBase Clone()
            {
                RequestBase r = new RequestBase();
                r.SetBytes(GetBytes());
                r.ack = ack;

                return r;
            }
        }

        public class RequestDelivery : RequestBase
        {
            /// <summary>目的地ステーション</summary>
            [BL_ObjectSync]
            public string station = "".PadRight(10);

            /// <summary>搬送対象棚No</summary>
            [BL_ObjectSync]
            public string rack = "".PadRight(10);

            /// <summary>横付け面No</summary>
            [BL_ObjectSync]
            public string rackface = "".PadRight(10);

            public RequestDelivery()
                : base()
            {
                //Initialize();
            }

            public RequestDelivery(enREQ cmd, int seqno, enRESULT result, string station, string rack, string rackface)
                : base(cmd, seqno, result)
            {
                //Initialize();

                this.station = station.PadRight(10);
                this.rack = rack.PadRight(10);
                this.rackface = rackface.PadRight(10);
            }

            public override string ToString()
            {
                return base.ToString() + "," + station + "," + rack + "," + rackface;
            }

            public override RequestBase Clone()
            {
                RequestDelivery r = base.Clone() as RequestDelivery;
                return r;
            }
        }

        public class RequestMove : RequestBase
        {
            /// <summary>ＡＧＶNo</summary>
            [BL_ObjectSync]
            public string agv = "".PadRight(10);

            /// <summary>目的地ステーションID</summary>
            [BL_ObjectSync]
            public string station = "".PadRight(10);

            /// <summary>棚上昇下降動作 "0":なし／"1":上昇／"2":下降／"C":充電開始／"c":充電終了</summary>
            [BL_ObjectSync]
            public string rack_action = "".PadRight(1);

            /// <summary>棚No</summary>
            [BL_ObjectSync]
            public string rack_no = "".PadRight(10);

            /// <summary>棚面指定</summary>
            [BL_ObjectSync]
            public string rack_face = "".PadRight(1);

            /// <summary>走行モード</summary>
            [BL_ObjectSync]
            public string run_mode = "".PadRight(1);

            /// <summary>走行ミュージック</summary>
            [BL_ObjectSync]
            public string run_music = "".PadRight(1);

            /// <summary>作業中</summary>
            [BL_ObjectSync]
            public string working = "".PadRight(1);


            /// <summary>動作指示親番</summary>
            public int order_no = 0;

            /// <summary>動作指示子番</summary>
            public int order_sub_no = 0;

            /// <summary>内部的に発生した要求</summary>
            public bool inner_request = false;

            public RequestMove()
                : base()
            {
                //Initialize();
                cmd = "MOV";
            }

            public override string ToString()
            {
                return base.ToString() + "," + agv + "," + station + "," + rack_action + "," + rack_no + "," + rack_face + "," + run_mode + "," + run_music + "," + working;
            }

            public override RequestBase Clone()
            {
                RequestMove r = base.Clone() as RequestMove;
                r.order_no = order_no;
                r.order_sub_no = order_sub_no;
                r.inner_request = inner_request;
                return r;
            }
        }

        public class RequestSTComplete : RequestBase
        {
            /// <summary>完了ステーション</summary>
            [BL_ObjectSync]
            public string station = "".PadRight(10);

            /// <summary>返却完了棚No（要求時は空白）</summary>
            [BL_ObjectSync]
            public string rack = "".PadRight(10);

            /// <summary>棚返却先ＱＲコード（要求時は空白）</summary>
            [BL_ObjectSync]
            public string qrcode = "".PadRight(20);

            public RequestSTComplete()
                : base()
            {
                //Initialize();
            }

            public RequestSTComplete(enREQ cmd, int seqno, enRESULT result, string station, string rack, string qrcode)
                : base(cmd, seqno, result)
            {
                //Initialize();

                this.station = station.PadRight(10);
                this.rack = rack.PadRight(10);
                this.qrcode = qrcode.PadRight(20);
            }

            public override string ToString()
            {
                return base.ToString() + "," + station.Trim() + "," + rack.Trim() + "," + qrcode.Trim();
            }

            public override RequestBase Clone()
            {
                RequestSTComplete r = base.Clone() as RequestSTComplete;
                return r;
            }
        }

        public class ReportBattery : RequestBase
        {
            /// <summary>ＡＧＶNo</summary>
            [BL_ObjectSync]
            public string agv = "".PadRight(10);

            /// <summary>バッテリー残量</summary>
            [BL_ObjectSync]
            public string bat = "".PadRight(3);

            /// <summary>異常コード</summary>
            [BL_ObjectSync]
            public string err = "".PadRight(3);

            /// <summary>フロア</summary>
            [BL_ObjectSync]
            public string map = "".PadRight(1);

            /// <summary>X</summary>
            [BL_ObjectSync]
            public string x = "".PadRight(5);

            /// <summary>Y</summary>
            [BL_ObjectSync]
            public string y = "".PadRight(5);

            public ReportBattery()
                : base()
            {
                //Initialize();
            }

            public ReportBattery(string agv, int bat, bool charging, int err, string map, int x, int y)
                : base(enREQ.BAT, 0, enRESULT.OK)
            {
                //Initialize();

                this.result = charging ? "1 " : "0 ";
                this.seqno = "".PadRight(5);
                this.agv = agv.PadRight(10);
                this.bat = bat.ToString("000");
                this.err = err.ToString("000");
                this.map = map;
                this.x = x.ToString("00000");
                this.y = y.ToString("00000");
            }

            public override string ToString()
            {
                return base.ToString() + "," + agv.Trim() + "," + bat.Trim() + "," + result.Trim() + "," + err.Trim();
            }

            public override RequestBase Clone()
            {
                ReportBattery r = base.Clone() as ReportBattery;
                return r;
            }
        }

        public class RequestAlarm : RequestBase
        {
            /// <summary>ＡＧＶNo</summary>
            [BL_ObjectSync]
            public string agv = "".PadRight(10);

            /// <summary>アラーム種別</summary>
            [BL_ObjectSync]
            public string alarm_type = "".PadRight(1);

            public RequestAlarm()
                : base()
            {
                cmd = "ALM";
            }

            public override string ToString()
            {
                return base.ToString() + "," + agv + "," + alarm_type;
            }

            public override RequestBase Clone()
            {
                RequestAlarm r = base.Clone() as RequestAlarm;
                return r;
            }
        }

        public class RequestReset : RequestBase
        {
            /// <summary>ＡＧＶNo</summary>
            [BL_ObjectSync]
            public string agv = "".PadRight(10);

            public RequestReset()
                : base()
            {
                cmd = "RST";
            }

            public override string ToString()
            {
                return base.ToString() + "," + agv;
            }

            public override RequestBase Clone()
            {
                RequestReset r = base.Clone() as RequestReset;
                return r;
            }
        }

        BL_TcpP2PMultiServer server;
        List<BL_TcpP2P> clients = new List<BL_TcpP2P>();

        public AgvOrderCommunicator(int host_port)
        {
            server = new BL_TcpP2PMultiServer(IPAddress.Any, host_port, 10, 10, BL_TcpP2P.BL_FormatType.STX_ETX);
            server.EventReceived += Server_EventReceived;
            server.EventConnected += Server_EventConnected;
            server.EventClosed += Server_EventClosed;
            server.StartLink();
        }

        private void Server_EventClosed(BL_TcpP2P sender)
        {
            if (clients.Contains(sender)) clients.Remove(sender);
        }

        private void Server_EventConnected(BL_TcpP2P sender)
        {
            if (!clients.Contains(sender)) clients.Add(sender);
        }

        private void Server_EventReceived(BL_TcpP2P sender)
        {
            byte[] data = sender.Receive();
            if (3 <= data.Length)
            {
                RequestBase req = null;

                string req_string = Encoding.ASCII.GetString(data, 0, 3).Trim();

                foreach (enREQ v in Enum.GetValues(typeof(enREQ)))
                {
                    if (v.ToString() == req_string)
                    {
                        switch (v)
                        {
                            case enREQ.FIN:
                                req = new RequestBase();
                                req.SetBytes(data);

                                req.result = "OK";
                                Send(req);

                                break;

                            case enREQ.QRS:
                                req = new RequestDelivery();
                                req.SetBytes(data);
                                break;

                            case enREQ.MOV:
                                req = new RequestMove();
                                req.SetBytes(data);
                                break;

                            case enREQ.QSC:
                                req = new RequestSTComplete();
                                req.SetBytes(data);
                                break;

                            case enREQ.ALM:
                                req = new RequestAlarm();
                                req.SetBytes(data);
                                break;

                            case enREQ.RST:
                                req = new RequestReset();
                                req.SetBytes(data);
                                break;
                        }

                        if (req != null)
                        {
                            if (ReceiveEvent != null) ReceiveEvent(this, req_string, req);
                        }
                    }
                }
            }
        }

        public void Dispose()
        {
            if (server != null)
            {
                server.EventReceived -= Server_EventReceived;

                server.EndLink();
                server.Dispose();
                server = null;
            }
        }

        public bool Send(RequestBase res)
        {
            bool ret = true;
            byte[] data = res.GetBytes();
            foreach (var v in clients)
            {
                ret &= v.Send(data);
            }

            return ret;
        }
    }
}
