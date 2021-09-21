using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;
using System.Net;

using BelicsClass.Common;
using BelicsClass.File;
using BelicsClass.Network;
using BelicsClass.ProcessManage;
using System.Threading;
using BelicsClass.ObjectSync;
using System.Security.Policy;

namespace AgvController
{
    public partial class VtaslCommunicator : BL_ThreadController_Base
    {
        protected const int TIMEOUT = 0;
        public string autorator_id = "";

        //public BL_IniFile ini = null;
        public BL_TcpP2PClient client = null;
        public string ip = "";
        public int port = 0;

        public class VtaslBaseS : BL_ObjectSync
        {
            static public int seqnow = 0;

            [BL_ObjectSync(Order = 1)]
            public string seqno = "  ";

            [BL_ObjectSync(Order = 2)]
            public string header = "  ";

            public VtaslBaseS()
                : base()
            {
                Initialize();

                seqnow = (seqnow + 1) % 100;
                if (seqnow == 0) seqnow = 1;

                seqno = seqnow.ToString("00");
            }

            public override string ToString()
            {
                return Encoding.Default.GetString(GetBytes());
            }
        }

        public class VtaslBaseR : BL_ObjectSync
        {
            [BL_ObjectSync(Order = 1)]
            public string seqno = "  ";

            [BL_ObjectSync(Order = 2)]
            public string header = " ";

            public VtaslBaseR()
                : base()
            {
                Initialize();
            }

            public override string ToString()
            {
                return Encoding.Default.GetString(GetBytes());
            }
        }

        public class VtaslResponseOK : VtaslBaseR
        {
            public VtaslResponseOK()
                : base()
            {
                header = "o";
            }

            public VtaslResponseOK(byte[] data)
                : this()
            {
                SetBytes(data);
            }
        }

        public class VtaslResponseNG : VtaslBaseR
        {
            [BL_ObjectSync(Order = 3)]
            public string reason = "  ";

            public VtaslResponseNG()
                : base()
            {
                header = "n";
            }

            public VtaslResponseNG(byte[] data)
                : this()
            {
                SetBytes(data);
            }
        }

        public class VtaslAI : VtaslBaseS
        {
            [BL_ObjectSync(Order = 4)]
            public string entry_floor = " ";
            [BL_ObjectSync(Order = 5)]
            public string entry_side = " ";
            [BL_ObjectSync(Order = 6)]
            public string exit_floor = " ";
            [BL_ObjectSync(Order = 7)]
            public string exit_side = " ";

            public VtaslAI()
                : base()
            {
                header = "AI";
            }
        }

        public class VtaslIF_Req : VtaslBaseS
        {
            [BL_ObjectSync(Order = 4)]
            public string entry_floor = " ";
            [BL_ObjectSync(Order = 5)]
            public string entry_side = " ";

            public VtaslIF_Req()
                : base()
            {
                header = "IF";
            }
        }

        public class VtaslIF_Res : VtaslBaseR
        {
            [BL_ObjectSync(Order = 4)]
            public string paletteno = "  ";

            public VtaslIF_Res()
                : base()
            {
            }
        }

        public class VtaslOC : VtaslBaseS
        {
            [BL_ObjectSync(Order = 4)]
            public string exit_floor = " ";
            [BL_ObjectSync(Order = 5)]
            public string exit_side = " ";
            [BL_ObjectSync(Order = 6)]
            public string paletteno = "  ";

            public VtaslOC()
                : base()
            {
                header = "OC";
            }
        }

        public class VtaslOF : VtaslBaseS
        {
            [BL_ObjectSync(Order = 4)]
            public string floor = " ";
            [BL_ObjectSync(Order = 5)]
            public string side = " ";
            [BL_ObjectSync(Order = 6)]
            public string paletteno = "  ";

            public VtaslOF()
                : base()
            {
                header = "OF";
            }
        }


        public object sync = new object();
        public VtaslBaseS req = null;
        public VtaslBaseR res = null;

        public BL_Stopwatch swTimeout = new BL_Stopwatch();

        public VtaslCommunicator(string id, string ip, int port)
            : base("VtaslCommunicator_" + id)
        {
            this.autorator_id = id;
            this.ip = ip;
            this.port = port;
        }

        public override string StartControl(int sleep, ThreadPriority priority)
        {
            IPAddress ipa;
            if (IPAddress.TryParse(ip, out ipa) && port != 0)
            {
                client = new BL_TcpP2PClient(IPAddress.Any, 0, ipa, port, 16, 16, BL_TcpP2P.BL_FormatType.STX_ETX);
                //client.EventReceived += Client_EventReceived;
                client.StartLink();
            }

            base.StartControl(sleep, priority);
            Log("START");
            return "";
        }

        //private void Client_EventReceived(BL_TcpP2P sender)
        //{
        //}

        public override void StopControl()
        {
            if (client != null)
            {
                client.EndLink();
                client.Dispose();
                client = null;
            }

            Log("STOP");
            base.StopControl();
        }

        protected override bool DoControl(object message)
        {
            if (client == null) return true;
            if (!client.Connected) return true;

            switch (m_Step)
            {
                case 0:
                    lock (sync)
                    {
                        if (req != null)
                        {
                            Log(req.ToString());

                            if (client.Send(req.GetBytes()))
                            {
                                swTimeout.Restart();
                                m_Step = 100;
                            }
                            else
                            {
                                req = null;
                            }
                        }
                    }
                    break;

                case 100:
                    {
                        byte[] data;
                        if (client.Receive(out data))
                        {
                            string seqno = Encoding.Default.GetString(data, 0, 2);

                            if (seqno == "xx" || req.seqno == seqno)
                            {
                                string header = Encoding.Default.GetString(data, 2, 1);

                                lock (sync)
                                {
                                    switch (header)
                                    {
                                        case "o":
                                            if (typeof(VtaslIF_Req).IsInstanceOfType(req))
                                            {
                                                res = new VtaslIF_Res();
                                                if (res.Length <= data.Length)
                                                {
                                                    res.SetBytes(data);
                                                    req = null;
                                                }
                                                else
                                                {
                                                    res = new VtaslResponseOK();
                                                    if (res.Length <= data.Length)
                                                    {
                                                        res.SetBytes(data);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                res = new VtaslResponseOK();
                                                if (res.Length <= data.Length)
                                                {
                                                    res.SetBytes(data);
                                                    req = null;
                                                }
                                            }
                                            break;

                                        case "n":
                                            res = new VtaslResponseNG();
                                            if (res.Length <= data.Length)
                                            {
                                                res.SetBytes(data);
                                                req = null;
                                            }
                                            break;

                                        default:
                                            res = new VtaslBaseR();
                                            break;
                                    }
                                }

                                Log(res.ToString());
                            }
                            else
                            {
                                Log("SEQNO MISSMATCH ["+Encoding.Default.GetString(data)+"]");

                                m_swTemp.Restart();
                                m_Step = 110;
                                break;
                            }

                            m_Step = 0;
                        }
                        else if (5000 <= swTimeout.ElapsedMilliseconds)
                        {
                            Log("TIMEOUT");
                            m_Step = 0;
                        }
                    }
                    break;

                case 110:
                    if (3000 <= m_swTemp.ElapsedMilliseconds)
                    {
                        m_Step = 0;
                    }
                    break;
            }


            return base.DoControl(message);
        }

        /// <summary>
        /// アシスタ搬入要求
        /// </summary>
        /// <param name="entry_floor"></param>
        /// <param name="entry_side"></param>
        /// <param name="exit_floor"></param>
        /// <param name="exit_side"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool AI(string entry_floor, string entry_side, string exit_floor, string exit_side, out string reason)
        {
            reason = "";
            BL_Stopwatch sw = new BL_Stopwatch();

            sw.Restart();
            while (true)
            {
                lock (sync)
                {
                    if (req == null && m_Step == 0)
                    {
                        req = new VtaslAI();
                        ((VtaslAI)req).entry_floor = entry_floor;
                        ((VtaslAI)req).entry_side = entry_side;
                        ((VtaslAI)req).exit_floor = exit_floor;
                        ((VtaslAI)req).exit_side = exit_side;
                        sw.Stop();
                        break;
                    }
                }

                if (TIMEOUT != 0 && TIMEOUT < sw.ElapsedMilliseconds) break;
                Sleep(m_Sleep);
            }

            if (sw.IsRunning) return false;

            sw.Restart();
            while (true)
            {
                Sleep(m_Sleep);

                lock (sync)
                {
                    if (req == null)
                    {
                        if (res != null && res.header == "o")
                        {
                            sw.Stop();
                            break;
                        }
                        else if (res != null && res.header == "n")
                        {
                            reason = ((VtaslResponseNG)res).reason;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (TIMEOUT != 0 && TIMEOUT < sw.ElapsedMilliseconds) break;
            }

            if (sw.IsRunning) return false;

            return true;
        }

        /// <summary>
        /// アシスタ搬入完了報告
        /// </summary>
        /// <param name="entry_floor"></param>
        /// <param name="entry_side"></param>
        /// <param name="paletteno"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool IF(string entry_floor, string entry_side, out string paletteno, out string reason)
        {
            paletteno = "";
            reason = "";
            BL_Stopwatch sw = new BL_Stopwatch();

            sw.Restart();
            while (true)
            {
                lock (sync)
                {
                    if (req == null && m_Step == 0)
                    {
                        req = new VtaslIF_Req();
                        ((VtaslIF_Req)req).entry_floor = entry_floor;
                        ((VtaslIF_Req)req).entry_side = entry_side;
                        sw.Stop();
                        break;
                    }
                }

                if (TIMEOUT != 0 && TIMEOUT < sw.ElapsedMilliseconds) break;
                Sleep(m_Sleep);
            }

            if (sw.IsRunning) return false;

            sw.Restart();
            while (true)
            {
                Sleep(m_Sleep);

                lock (sync)
                {
                    if (req == null)
                    {
                        if (res != null && res.header == "o")
                        {
                            if (typeof(VtaslIF_Res).IsInstanceOfType(res))
                            {
                                paletteno = ((VtaslIF_Res)res).paletteno;
                            }

                            sw.Stop();
                            break;
                        }
                        else if (res != null && res.header == "n")
                        {
                            reason = ((VtaslResponseNG)res).reason;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (TIMEOUT != 0 && TIMEOUT < sw.ElapsedMilliseconds) break;
            }

            if (sw.IsRunning) return false;

            return true;
        }

        /// <summary>
        /// 搬出許可確認
        /// </summary>
        /// <param name="exit_floor"></param>
        /// <param name="exit_side"></param>
        /// <param name="paletteno"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool OC(string exit_floor, string exit_side, string paletteno, out string reason)
        {
            reason = "";
            BL_Stopwatch sw = new BL_Stopwatch();

            sw.Restart();
            while (true)
            {
                lock (sync)
                {
                    if (req == null && m_Step == 0)
                    {
                        req = new VtaslOC();
                        ((VtaslOC)req).exit_floor = exit_floor;
                        ((VtaslOC)req).exit_side = exit_side;
                        ((VtaslOC)req).paletteno = paletteno;
                        sw.Stop();
                        break;
                    }
                }

                if (TIMEOUT != 0 && TIMEOUT < sw.ElapsedMilliseconds) break;
                Sleep(m_Sleep);
            }

            if (sw.IsRunning) return false;

            sw.Restart();
            while (true)
            {
                Sleep(m_Sleep);

                lock (sync)
                {
                    if (req == null)
                    {
                        if (res != null && res.header == "o")
                        {
                            sw.Stop();
                            break;
                        }
                        else if (res != null && res.header == "n")
                        {
                            reason = ((VtaslResponseNG)res).reason;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (TIMEOUT != 0 && TIMEOUT < sw.ElapsedMilliseconds) break;
            }

            if (sw.IsRunning) return false;

            return true;
        }

        /// <summary>
        /// 搬出完了報告
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool OF(string exit_floor, string exit_side, string paletteno, out string reason)
        {
            reason = "";
            BL_Stopwatch sw = new BL_Stopwatch();

            sw.Restart();
            while (true)
            {
                lock (sync)
                {
                    if (req == null && m_Step == 0)
                    {
                        req = new VtaslOF();
                        ((VtaslOF)req).floor = exit_floor;
                        ((VtaslOF)req).side = exit_side;
                        ((VtaslOF)req).paletteno = paletteno;
                        sw.Stop();
                        break;
                    }
                }

                if (TIMEOUT != 0 && TIMEOUT < sw.ElapsedMilliseconds) break;
                Sleep(m_Sleep);
            }

            if (sw.IsRunning) return false;

            sw.Restart();
            while (true)
            {
                Sleep(m_Sleep);

                lock (sync)
                {
                    if (req == null)
                    {
                        if (res != null && res.header == "o")
                        {
                            sw.Stop();
                            break;
                        }
                        else if (res != null && res.header == "n")
                        {
                            reason = ((VtaslResponseNG)res).reason;
                            break;
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if (TIMEOUT != 0 && TIMEOUT < sw.ElapsedMilliseconds) break;
            }

            if (sw.IsRunning) return false;

            return true;
        }
    }
}
