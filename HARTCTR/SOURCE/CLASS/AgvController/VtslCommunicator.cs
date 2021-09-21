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
    public class VtslCommunicator : BL_ThreadController_Base
    {
        protected const int TIMEOUT = 0;
        public string autorator_id = "";

        //public BL_IniFile ini = null;
        public BL_TcpP2PClient client = null;
        public string ip = "";
        public int port = 0;

        public class VtslBaseS : BL_ObjectSync
        {
            static public int seqnow = 0;

            [BL_ObjectSync(Order = 1)]
            public string seqno = "  ";

            [BL_ObjectSync(Order = 2)]
            public string header = "  ";

            public VtslBaseS()
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

        public class VtslBaseR : BL_ObjectSync
        {
            [BL_ObjectSync(Order = 1)]
            public string seqno = "  ";

            [BL_ObjectSync(Order = 2)]
            public string header = " ";

            public VtslBaseR()
                : base()
            {
                Initialize();
            }

            public override string ToString()
            {
                return Encoding.Default.GetString(GetBytes());
            }
        }

        public class VtslResponseOK : VtslBaseR
        {
            public VtslResponseOK()
                : base()
            {
                header = "o";
            }

            public VtslResponseOK(byte[] data)
                : this()
            {
                SetBytes(data);
            }
        }

        public class VtslResponseNG : VtslBaseR
        {
            [BL_ObjectSync(Order = 3)]
            public string reason = "  ";

            public VtslResponseNG()
                : base()
            {
                header = "n";
            }

            public VtslResponseNG(byte[] data)
                : this()
            {
                SetBytes(data);
            }
        }

        public class VtslTR : VtslBaseS
        {
            [BL_ObjectSync(Order = 4)]
            public string entry_floor = " ";
            [BL_ObjectSync(Order = 5)]
            public string entry_side = " ";
            [BL_ObjectSync(Order = 6)]
            public string exit_floor = " ";
            [BL_ObjectSync(Order = 7)]
            public string exit_side = " ";
            [BL_ObjectSync(Order = 8)]
            public string shutter = " ";

            public VtslTR()
                : base()
            {
                header = "TR";
            }
        }

        public class VtslIF : VtslBaseS
        {
            [BL_ObjectSync(Order = 4)]
            public string entry_floor = " ";
            [BL_ObjectSync(Order = 5)]
            public string entry_side = " ";

            public VtslIF()
                : base()
            {
                header = "IF";
            }
        }

        public class VtslOF : VtslBaseS
        {
            [BL_ObjectSync(Order = 4)]
            public string exit_floor = " ";
            [BL_ObjectSync(Order = 5)]
            public string exit_side = " ";

            public VtslOF()
                : base()
            {
                header = "OF";
            }
        }

        public class VtslStatus : VtslBaseS
        {
            public enum enTotalStatus : int
            {
                OFFLINE = 0,
                AUTORUN_OFF = 1,
                MANUAL = 2,
                ERROR = 3,
                ONLINE_WAIT = 10,
                ONLINE_BUSY = 11,
                CAN_ENTRY = 20,
                CAN_EXIT = 21,
            }

            public enum enStatus : int
            {
                NONE = 0,
                PREPARE = 1,
                BUSY = 2,
            }

            [BL_ObjectSync(Order = 4)]
            public string total_status = "  ";
            [BL_ObjectSync(Order = 5)]
            public string error_code = "    ";

            [BL_ObjectSync(Order = 6)]
            public string stop_floor = " ";

            [BL_ObjectSync(Order = 7)]
            public string entry_floor = " ";
            [BL_ObjectSync(Order = 8)]
            public string entry_side = " ";
            [BL_ObjectSync(Order = 9)]
            public string entry_shutter = " ";
            [BL_ObjectSync(Order = 10)]
            public string entry_status = " ";

            [BL_ObjectSync(Order = 11)]
            public string exit_floor = " ";
            [BL_ObjectSync(Order = 12)]
            public string exit_side = " ";
            [BL_ObjectSync(Order = 13)]
            public string exit_shutter = " ";
            [BL_ObjectSync(Order = 14)]
            public string exit_status = " ";

            [BL_ObjectSync(Order = 15)]
            public string request_clear = " ";
            [BL_ObjectSync(Order = 16)]
            public string option = "  ";


            public VtslStatus()
                : base()
            {
            }

            public VtslStatus(byte[] data)
                : this()
            {
                SetBytes(data);
            }

            public bool IsDisable
            {
                get
                {
                    if (TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.ERROR
                     || TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.MANUAL
                     || TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.OFFLINE
                     || TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.AUTORUN_OFF
                     )
                    {
                        return true;
                    }

                    return false;
                }
            }

            public enTotalStatus TotalStatus
            {
                get
                {
                    int s = 0;int.TryParse(total_status, out s);
                    enTotalStatus es = (enTotalStatus)s;
                    return es;
                }
            }

            public enStatus EntryStatus
            {
                get
                {
                    int s = 0; int.TryParse(entry_status, out s);
                    enStatus es = (enStatus)s;
                    return es;
                }
            }

            public enStatus ExitStatus
            {
                get
                {
                    int s = 0; int.TryParse(exit_status, out s);
                    enStatus es = (enStatus)s;
                    return es;
                }
            }

            public override string ToString()
            {
                string s = TotalStatus.ToString() + "\n";
                if (error_code.Trim('0', ' ') != "") s += "/異常[" + error_code + "]" + "\n";
                s += "停止階[" + stop_floor + "]" + "\n";
                s += "搬入[階" + entry_floor;
                s += ":" + "間口" + entry_side;
                s += ":" + "ﾄﾞｱ" + entry_shutter;
                s += ":" + "状態" + EntryStatus.ToString() + "]" + "\n";

                s += "搬出[階" + exit_floor;
                s += ":" + "間口" + exit_side;
                s += ":" + "ﾄﾞｱ" + exit_shutter;
                s += ":" + "状態" + ExitStatus.ToString() + "]" + "\n";

                s += "ｸﾘｱ[" + request_clear + "]" + "\n";
                s += "OP[" + option + "]";

                return s;
                //return base.ToString();
            }
        }

        public object sync = new object();
        public VtslBaseS req = null;
        public VtslBaseR res = null;

        public VtslStatus state = new VtslStatus();

        public BL_Stopwatch swTimeout = new BL_Stopwatch();

        public VtslCommunicator(string id, string ip, int port)
            : base("VtslCommunicator_" + id)
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
                            m_swTemp.Restart();

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
                        else if (!m_swTemp.IsRunning || 2000 <= m_swTemp.ElapsedMilliseconds)
                        {
                            m_swTemp.Restart();

                            req = new VtslBaseS();
                            req.header = "ST";

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
                            //読み捨て
                            byte[] d; while (client.Receive(out d)) ;


                            string seqno = Encoding.Default.GetString(data, 0, 2);

                            if (seqno == "xx" || req.seqno == seqno)
                            {
                                string header = Encoding.Default.GetString(data, 2, 1);

                                lock (sync)
                                {
                                    switch (header)
                                    {
                                        case "s":
                                            if (req.header == "ST")
                                            {
                                                if (state.Length <= data.Length)
                                                {
                                                    state.SetBytes(data);
                                                    res = null;
                                                    req = null;
                                                }
                                            }
                                            break;

                                        case "o":
                                            //if (req.header != "ST")
                                            //{
                                            res = new VtslResponseOK();
                                            if (res.Length <= data.Length)
                                            {
                                                res.SetBytes(data);
                                                req = null;
                                            }
                                            //}
                                            break;

                                        case "n":
                                            //if (req.header != "ST")
                                            //{
                                            res = new VtslResponseNG();
                                            if (res.Length <= data.Length)
                                            {
                                                res.SetBytes(data);
                                                req = null;
                                            }
                                            //}
                                            break;

                                        default:
                                            res = new VtslBaseR();
                                            break;
                                    }
                                }

                                if (res != null) Log(res.ToString());
                                else Log(state.ToString());
                            }
                            else
                            {
                                m_swTemp.Restart();
                                m_Step = 110;
                                break;
                            }

                            m_swTemp.Restart();
                            m_Step = 0;
                        }
                        else if (5000 <= swTimeout.ElapsedMilliseconds)
                        {
                            if (req.header == "ST") req = null;

                            Log("TIMEOUT");

                            m_swTemp.Restart();
                            m_Step = 0;
                        }
                    }
                    break;

                case 110:
                    if (3000 <= m_swTemp.ElapsedMilliseconds)
                    {
                        m_swTemp.Restart();
                        m_Step = 0;
                    }
                    break;
            }


            return base.DoControl(message);
        }

        /// <summary>
        /// 搬送指示
        /// </summary>
        /// <param name="entry_floor"></param>
        /// <param name="entry_side"></param>
        /// <param name="exit_floor"></param>
        /// <param name="exit_side"></param>
        /// <param name="half_shutter"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool TR(string entry_floor, string entry_side, string exit_floor, string exit_side, bool half_shutter, out string reason)
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
                        req = new VtslTR();
                        ((VtslTR)req).entry_floor = entry_floor;
                        ((VtslTR)req).entry_side = entry_side;
                        ((VtslTR)req).exit_floor = exit_floor;
                        ((VtslTR)req).exit_side = exit_side;
                        ((VtslTR)req).shutter = half_shutter ? "1" : "0";
                        sw.Stop();
                        break;
                    }
                }

                if (TIMEOUT != 0 && TIMEOUT < sw.ElapsedMilliseconds) break;
                Sleep(m_Sleep);
            }

            if (sw.IsRunning)
            {
                sw.Stop();
                sw.Reset();
                return false;
            }

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
                            reason = ((VtslResponseNG)res).reason;
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

            if (sw.IsRunning)
            {
                sw.Stop();
                sw.Reset();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 搬入完了報告
        /// </summary>
        /// <param name="entry_floor"></param>
        /// <param name="entry_side"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool IF(string entry_floor, string entry_side, out string reason)
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
                        req = new VtslIF();
                        ((VtslIF)req).entry_floor = entry_floor;
                        ((VtslIF)req).entry_side = entry_side;
                        sw.Stop();
                        break;
                    }
                }

                if (TIMEOUT != 0 && TIMEOUT < sw.ElapsedMilliseconds) break;
                Sleep(m_Sleep);
            }

            if (sw.IsRunning)
            {
                sw.Stop();
                sw.Reset();
                return false;
            }

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
                            reason = ((VtslResponseNG)res).reason;
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

            if (sw.IsRunning)
            {
                sw.Stop();
                sw.Reset();
                return false;
            }

            return true;
        }

        /// <summary>
        /// 搬出完了報告
        /// </summary>
        /// <param name="exit_floor"></param>
        /// <param name="exit_side"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public bool OF(string exit_floor, string exit_side, out string reason)
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
                        req = new VtslOF();
                        ((VtslOF)req).exit_floor = exit_floor;
                        ((VtslOF)req).exit_side = exit_side;
                        sw.Stop();
                        break;
                    }
                }

                if (TIMEOUT != 0 && TIMEOUT < sw.ElapsedMilliseconds) break;
                Sleep(m_Sleep);
            }

            if (sw.IsRunning)
            {
                sw.Stop();
                sw.Reset();
                return false;
            }

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
                            reason = ((VtslResponseNG)res).reason;
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

            if (sw.IsRunning)
            {
                sw.Stop();
                sw.Reset();
                return false;
            }

            return true;
        }

        /// <summary>
        /// クリア要求応答
        /// </summary>
        /// <returns></returns>
        public bool CL()
        {
            BL_Stopwatch sw = new BL_Stopwatch();
            sw.Restart();

            while (true)
            {
                lock (sync)
                {
                    if (req == null)
                    {
                        string cmd = "CL";
                        Log(cmd);

                        byte[] data = Encoding.Default.GetBytes(cmd);
                        if (!client.Send(data))
                        {
                            Log(cmd);
                            return false;
                        }

                        break;
                    }
                }

                if (TIMEOUT != 0 && TIMEOUT < sw.ElapsedMilliseconds) break;
                Sleep(m_Sleep);
            }

            return true;
        }
    }
}
