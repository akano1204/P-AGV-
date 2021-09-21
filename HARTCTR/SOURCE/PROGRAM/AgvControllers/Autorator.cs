using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using BelicsClass.Common;
using BelicsClass.ProcessManage;

using AgvController;
using System.Threading;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        public class AutoratorController : BL_ThreadController_Base
        {
            #region 内部クラス

            public class SideInfo
            {
                public int degree { get; set; } = 0;

                public string code { get; set; } = "";
            }

            public class FloorInfo
            {
                public int no { get; set; } = 0;
                public string code { get; set; } = "";

                public bool is_qrcode { get; set; } = false;
            }

            public class AutoratorInAgv
            {
                public FloorAGV agv = null;

                public string entry_floor = "";
                public string entry_side = "";

                public string exit_floor = "";
                public string exit_side = "";

                public string palette_no = "";

                public AutoratorInAgv() { }

                public AutoratorInAgv(FloorAGV agv, string entry_floor, string entry_side, string exit_floor, string exit_side)
                {
                    this.agv = agv;
                    this.entry_floor = entry_floor;
                    this.entry_side = entry_side;
                    this.exit_floor = exit_floor;
                    this.exit_side = exit_side;
                }

                public override string ToString()
                {
                    return agv.ToString() + "," + entry_floor.Trim() + "," + entry_side.Trim() + "," + exit_floor.Trim() + "," + exit_side.Trim() + "," + palette_no.Trim();
                }
            }

            #endregion

            #region プロパティ

            public string id { get; set; } = "";
            public string ipaddress { get; set; } = "";
            public int hostport { get; set; } = 0;

            public bool is_assister { get; set; } = false;

            public int hostport_assister { get; set; } = 0;

            #endregion

            #region オートレーター設定

            public List<FloorQR> autorator_qrs = new List<FloorQR>();

            public List<SideInfo> sideinfo = new List<SideInfo>();

            public List<FloorInfo> floorinfo = new List<FloorInfo>();

            #endregion

            protected VtslCommunicator vtsl = null;

            public bool repaint = false;

            public VtslCommunicator.VtslStatus Status
            {
                get
                {
                    if (vtsl == null) return null;
                    return vtsl.state;
                }
            }

            protected Queue<AutoratorInAgv> que_waitrequest = new Queue<AutoratorInAgv>();
            
            protected List<AutoratorInAgv> que_waitentry = new List<AutoratorInAgv>();
            protected List<AutoratorInAgv> que_canentry = new List<AutoratorInAgv>();
            protected List<AutoratorInAgv> que_entried = new List<AutoratorInAgv>();

            protected List<AutoratorInAgv> que_waitexit = new List<AutoratorInAgv>();
            protected List<AutoratorInAgv> que_canexit = new List<AutoratorInAgv>();
            protected List<AutoratorInAgv> que_exited = new List<AutoratorInAgv>();
            
            public AutoratorController()
                :base("*")
            {
            }

            public string FloorNo(string floor_code)
            {
                var info = floorinfo.Where(e => e.code == floor_code).FirstOrDefault();
                if (info != null)
                {
                    return info.no.ToString();
                }

                return "";
            }

            public string Side(int degree)
            {
                var info = sideinfo.Where(e => e.degree == degree).FirstOrDefault();
                if (info != null)
                {
                    return info.code;
                }

                return "";
            }

            virtual protected void Clear()
            {
                lock (this)
                {
                    foreach (var v in que_waitrequest.ToList()
                        .Concat(que_canentry)
                        .Concat(que_entried)
                        .Concat(que_waitexit)
                        .Concat(que_canexit)
                        .Concat(que_exited))
                    {
                        Log("CLEAR[" + v.agv.ToString() + "]");
                        v.agv.autorator_reset = true;
                    }

                    que_waitrequest.Clear();
                    que_waitentry.Clear();
                    que_canentry.Clear();
                    que_entried.Clear();

                    que_waitexit.Clear();
                    que_canexit.Clear();
                    que_exited.Clear();
                }

                vts_step = 0;
                repaint = true;
            }

            public override string StartControl(int sleep, ThreadPriority priority)
            {
                base.StartControl(sleep, priority);

                if (m_ID.ToString() == "*")
                {
                    if (m_Log != null) m_Log.Dispose();
                    base.m_ID = "VTS_" + id;
                    m_Log = new BelicsClass.File.BL_Log("", m_ID.ToString() + ".LOG");
                }

                Log("VTS ,START");

                Clear();

                if (vtsl != null) vtsl.StopControl();

                vtsl = new VtslCommunicator(id, ipaddress, hostport);
                vtsl.StartControl(sleep, priority);

                return "";
            }

            public override void StopControl()
            {
                if (vtsl != null)
                {
                    vtsl.StopControl();
                    vtsl = null;

                    Clear();
                    Log("VTS ,STOP");
                }

                base.StopControl();
            }

            string response_clear = "0";

            int vts_step = 0;

            BL_Stopwatch swClearTimeout = new BL_Stopwatch();

            VtslCommunicator.VtslStatus state_pre = new VtslCommunicator.VtslStatus();

            protected override bool DoControl(object message)
            {
                //クリア要求処理
                if (Status != null)
                {
                    if (Status.request_clear == "1" && response_clear != "1")
                    {
                        if (vtsl.CL())
                        {
                            swClearTimeout.Restart();

                            Clear();

                            response_clear = "1";

                            repaint = true;
                        }
                    }
                    else if (Status.request_clear == "0" && response_clear != "0")
                    {
                        response_clear = "0";
                        swClearTimeout.Stop();

                        repaint = true;
                    }
                    else if (swClearTimeout.IsRunning && 2000 <= swClearTimeout.ElapsedMilliseconds)
                    {
                        response_clear = "0";
                        swClearTimeout.Stop();

                        repaint = true;
                    }

                    byte[] b = Status.GetBytes();
                    if (!state_pre.GetBytes().SequenceEqual(b))
                    {
                        repaint = true;

                        state_pre.SetBytes(b);
                    }
                }

                lock (this)
                {
                    switch (vts_step)
                    {
                        case 0:
                            //搬入要求待ち
                            if (0 < que_waitrequest.Count)
                            {
                                var ea = que_waitrequest.Peek();

                                if (Status.TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.ONLINE_WAIT
                                    //&& Status.EntryStatus == VtslCommunicator.VtslStatus.enStatus.NONE
                                    //&& Status.ExitStatus == VtslCommunicator.VtslStatus.enStatus.NONE
                                    )
                                {
                                    Log("VTS ,S[TR:" + ea.ToString() + "]");

                                    string reason = "";
                                    if (vtsl.TR(ea.entry_floor, ea.entry_side, ea.exit_floor, ea.exit_side, ea.agv.rack == null, out reason))
                                    {
                                        repaint = true;
                                        Log("VTS ,R[TR:o]");

                                        que_waitrequest.Dequeue();
                                        que_waitentry.Add(ea);
                                        vts_step = 100;
                                    }
                                    else
                                    {
                                        Log("VTS ,R[TR:n][" + reason + "]");

                                        m_swTemp.Restart();
                                        vts_step = 10;
                                    }
                                }
                            }
                            break;

                        case 10:
                            if (1000 <= m_swTemp.ElapsedMilliseconds)
                            {
                                vts_step = 0;
                            }
                            break;

                        case 100:
                            //搬入許可待ち
                            {
                                if (Status.TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.CAN_ENTRY)
                                {
                                    var wa = que_waitentry.Where(e => e.entry_floor == Status.entry_floor && e.entry_side == Status.entry_side).FirstOrDefault();
                                    if (wa != null)
                                    {
                                        repaint = true;
                                        Log("VTS ,搬入可[" + wa.ToString() + "]");

                                        que_waitentry.Remove(wa);
                                        que_canentry.Add(wa);

                                        vts_step = 200;
                                    }
                                }
                            }
                            break;

                        case 200:
                            //搬入完了待ち
                            {
                                var ea = que_entried.Where(e => e.entry_floor == Status.entry_floor && e.entry_side == Status.entry_side).FirstOrDefault();
                                if (ea != null)
                                {
                                    Log("VTS ,S[IF:" + ea.ToString() + "]");

                                    string reason = "";
                                    if (vtsl.IF(ea.entry_floor, ea.entry_side, out reason))
                                    {
                                        repaint = true;
                                        Log("VTS ,R[IF:o]");

                                        que_entried.Remove(ea);
                                        que_waitexit.Add(ea);

                                        vts_step = 300;
                                    }
                                    else
                                    {
                                        Log("VTS ,R[IF:n][" + reason + "]");

                                        m_swTemp.Restart();
                                        vts_step = 210;
                                    }
                                }
                            }
                            break;

                        case 210:
                            if (1000 <= m_swTemp.ElapsedMilliseconds)
                            {
                                vts_step = 200;
                            }
                            break;

                        case 300:
                            //搬出可能待ち
                            {
                                if (Status.TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.CAN_EXIT)
                                {
                                    var ea = que_waitexit.Where(e => e.exit_floor == Status.exit_floor && e.exit_side == Status.exit_side).FirstOrDefault();
                                    if (ea != null)
                                    {
                                        repaint = true;
                                        Log("VTS ,搬出可[" + ea.ToString() + "]");

                                        que_waitexit.Remove(ea);
                                        que_canexit.Add(ea);

                                        vts_step = 400;
                                    }
                                }
                            }
                            break;

                        case 400:
                            //搬出完了待ち
                            {
                                var ea = que_exited.Where(e => e.exit_floor == Status.exit_floor && e.exit_side == Status.exit_side).FirstOrDefault();
                                if (ea != null)
                                {
                                    Log("VTS ,S[OF:" + ea.ToString() + "]");

                                    string reason = "";
                                    if (vtsl.OF(ea.exit_floor, ea.exit_side, out reason))
                                    {
                                        repaint = true;
                                        Log("VTS ,S[OF:o]");

                                        que_exited.Remove(ea);
                                        vts_step = 0;
                                    }
                                    else
                                    {
                                        Log("VTS ,S[OF:n][" + reason + "]");

                                        m_swTemp.Restart();
                                        vts_step = 410;
                                    }
                                }
                            }
                            break;

                        case 410:
                            if (1000 <= m_swTemp.ElapsedMilliseconds)
                            {
                                vts_step = 400;
                            }
                            break;
                    }
                }

                return base.DoControl(message);
            }

            virtual public bool RequestEntry(FloorAGV agv, string entry_floor, string entry_side, string exit_floor, string exit_side)
            {
                lock (this)
                {
                    if (0 < que_waitrequest.ToList().Concat(que_waitentry).Concat(que_canentry).Concat(que_entried).Concat(que_waitexit).Concat(que_canexit).Concat(que_exited).Where(e => e.agv == agv).Count())
                    {
                        return true;
                    }

                    var req = new AutoratorInAgv(agv, entry_floor, entry_side, exit_floor, exit_side);
                    Log("VTS ,搬入要求[" + req.ToString() + "]");

                    que_waitrequest.Enqueue(req);
                }

                return true;
            }

            virtual public bool CanEntry(FloorAGV agv)
            {
                return 0 < que_canentry.Where(e => e.agv == agv).Count();
            }

            virtual public bool CompleteEntry(FloorAGV agv)
            {
                lock (this)
                {
                    var entried_agv = que_canentry.Where(e => e.agv == agv).ToList();
                    if (0 < entried_agv.Count)
                    {
                        var a = entried_agv[0];

                        que_canentry.Remove(a);
                        if (!que_entried.Contains(a)) que_entried.Add(a);

                        return true;
                    }
                }

                return false;
            }

            virtual public bool CanExit(FloorAGV agv)
            {
                return 0 < que_canexit.Where(e => e.agv == agv).Count();
            }

            virtual public bool CompleteExit(FloorAGV agv)
            {
                lock (this)
                {
                    var exited_agv = que_canexit.Where(e => e.agv == agv).ToList();
                    if (0 < exited_agv.Count)
                    {
                        var ea = exited_agv[0];

                        que_canexit.Remove(ea);
                        if (!que_exited.Contains(ea)) que_exited.Add(ea);
                        return true;
                    }
                }

                return true;
            }

            virtual public bool IsEntried(FloorAGV agv)
            {
                return 0 < que_entried
                                .Concat(que_waitexit)
                                .Concat(que_canexit)
                                .Concat(que_exited)
                                .Where(e => e.agv == agv).Count();
            }

            virtual public bool IsRequested(FloorAGV agv)
            {
                return 0 < que_waitrequest
                                .Concat(que_waitentry)
                                .Concat(que_canentry)
                                .Concat(que_entried)
                                .Concat(que_waitexit)
                                .Concat(que_canexit)
                                .Concat(que_exited)
                                .Where(e => e.agv == agv).Count();
            }

            /// <summary>
            /// 定義上のQRポイントを取得
            /// </summary>
            /// <param name="floorno"></param>
            /// <returns></returns>
            public FloorQR LogicQR(int floorno)
            {
                var fi = floorinfo.Where(e => e.no == floorno).FirstOrDefault();
                if (fi != null)
                {
                    return autorator_qrs.Where(e => e.floor.code == fi.code).FirstOrDefault();
                }

                return null;
            }

            /// <summary>
            /// 実際に床に貼られているQRポイントを取得
            /// </summary>
            /// <returns></returns>
            public FloorQR RealQR()
            {
                var fi = floorinfo.Where(e => e.is_qrcode).FirstOrDefault();
                if (fi == null) fi = floorinfo.OrderBy(e => e.no).FirstOrDefault();

                if (fi != null)
                {
                    return autorator_qrs.Where(e => e.floor.code == fi.code).FirstOrDefault();
                }

                return null;
            }
        }


        public class AutoratorController_withAssister : AutoratorController
        {
            public VtaslCommunicator vtasl = null;

            public Queue<AutoratorInAgv> que_assister_waitrequest = new Queue<AutoratorInAgv>();
            
            public List<AutoratorInAgv> que_assister_waitentry = new List<AutoratorInAgv>();
            public List<AutoratorInAgv> que_assister_canentry = new List<AutoratorInAgv>();
            public List<AutoratorInAgv> que_assister_entried = new List<AutoratorInAgv>();

            public List<AutoratorInAgv> que_assister_waitexit = new List<AutoratorInAgv>();
            public List<AutoratorInAgv> que_assister_canexit = new List<AutoratorInAgv>();
            public List<AutoratorInAgv> que_assister_exited = new List<AutoratorInAgv>();


            public AutoratorController_withAssister()
                : base()
            {
            }

            public override string StartControl(int sleep, ThreadPriority priority)
            {
                if (m_ID.ToString() == "*")
                {
                    if (m_Log != null) m_Log.Dispose();
                    base.m_ID = "VTAS_" + id;
                    m_Log = new BelicsClass.File.BL_Log("", m_ID.ToString() + ".LOG");
                }

                base.StartControl(sleep, priority);
                Log("VTAS,start");

                if (vtasl != null) vtasl.StopControl();
                vtasl = new VtaslCommunicator(id, ipaddress, hostport_assister);
                vtasl.StartControl(sleep);

                return "";
            }

            public override void StopControl()
            {
                if (vtasl != null)
                {
                    vtasl.StopControl();
                    vtasl = null;

                    Log("VTAS,stop");
                }

                base.StopControl();
            }

            protected override void Clear()
            {
                lock (this)
                {
                    foreach (var v in que_assister_waitrequest.ToList()
                        .Concat(que_assister_canentry)
                        .Concat(que_assister_entried)
                        .Concat(que_assister_waitexit)
                        .Concat(que_assister_canexit)
                        .Concat(que_assister_exited))
                    {
                        Log("CLEAR[" + v.agv.ToString() + "]");
                        v.agv.autorator_reset = true;
                    }

                    que_assister_waitrequest.Clear();
                    que_assister_waitentry.Clear();
                    que_assister_canentry.Clear();
                    que_assister_entried.Clear();
                    que_assister_waitexit.Clear();
                    que_assister_canexit.Clear();
                    que_assister_exited.Clear();
                }

                base.Clear();

                vtas_step = 0;
            }

            int vtas_step = 0;

            protected override bool DoControl(object message)
            {
                lock (this)
                {
                    switch (vtas_step)
                    {
                        case 0:
                            if (0 < que_assister_waitrequest.Count)
                            {
                                var ea = que_assister_waitrequest.Peek();

                                Log("VTAS,s[AI:" + ea.ToString() + "]");

                                string reason = "";
                                if (vtasl.AI(ea.entry_floor, ea.entry_side, ea.exit_floor, ea.exit_side, out reason))
                                {
                                    repaint = true;
                                    Log("VTAS,r[AI:o]");

                                    que_assister_waitrequest.Dequeue();
                                    que_assister_canentry.Add(ea);
                                    vtas_step = 100;
                                }
                                else
                                {
                                    Log("VTAS,r[AI:n][" + reason + "]");

                                    m_swTemp.Restart();
                                    vtas_step = 10;
                                }
                            }
                            break;

                        case 10:
                            if (3000 <= m_swTemp.ElapsedMilliseconds)
                            {
                                vtas_step = 0;
                            }
                            break;

                        case 100:
                            {
                                var ea = 0 < que_assister_entried.Count ? que_assister_entried[0] : null;
                                if (ea != null)
                                {
                                    Log("VTAS,s[IF:" + ea.ToString() + "]");

                                    string reason = "";
                                    string paletteno = "";
                                    if (vtasl.IF(ea.entry_floor, ea.entry_side, out paletteno, out reason))
                                    {
                                        repaint = true;
                                        ea.palette_no = paletteno;
                                        Log("VTAS,r[IF:o][" + paletteno + "]");

                                        que_assister_entried.Remove(ea);
                                        que_assister_waitexit.Add(ea);

                                        vtas_step = 200;
                                    }
                                    else
                                    {
                                        Log("VTAS,r[IF:n][" + reason + "]");

                                        m_swTemp.Restart();
                                        vtas_step = 110;
                                    }
                                }
                            }
                            break;

                        case 110:
                            if (1000 <= m_swTemp.ElapsedMilliseconds)
                            {
                                vtas_step = 100;
                            }
                            break;

                        case 200:
                            {
                                var ea = 0 < que_assister_waitexit.Count ? que_assister_waitexit[0] : null;
                                if (ea != null)
                                {
                                    base.RequestEntry(ea.agv, ea.entry_floor, ea.entry_side, ea.exit_floor, ea.exit_side);

                                    vtas_step = 210;
                                }
                            }
                            break;

                        case 210:
                            {
                                var ea = 0 < que_assister_waitexit.Count ? que_assister_waitexit[0] : null;
                                if (ea != null)
                                {
                                    if (base.CanEntry(ea.agv))
                                    {
                                        base.CompleteEntry(ea.agv);
                                        vtas_step = 220;
                                    }
                                }
                            }
                            break;

                        case 220:
                            {
                                var ea = 0 < que_assister_waitexit.Count ? que_assister_waitexit[0] : null;
                                if (ea != null)
                                {
                                    if (base.CanExit(ea.agv))
                                    {
                                        base.CompleteExit(ea.agv);
                                        vtas_step = 300;
                                    }
                                }
                            }
                            break;

                        case 300:
                            {
                                var ea = 0 < que_assister_waitexit.Count ? que_assister_waitexit[0] : null;
                                if (ea != null)
                                {
                                    Log("VTAS,s[OC:" + ea.ToString() + "]");

                                    string reason = "";
                                    if (vtasl.OC(ea.exit_floor, ea.exit_side, ea.palette_no, out reason))
                                    {
                                        repaint = true;
                                        Log("VTAS,r[OC:o]");

                                        que_assister_waitexit.Remove(ea);
                                        que_assister_canexit.Add(ea);

                                        vtas_step = 400;
                                    }
                                    else
                                    {
                                        Log("VTAS,r[OC:n][" + reason + "]");

                                        m_swTemp.Restart();
                                        vtas_step = 310;
                                    }
                                }
                            }
                            break;

                        case 310:
                            if (1000 <= m_swTemp.ElapsedMilliseconds)
                            {
                                vtas_step = 300;
                            }
                            break;

                        case 400:
                            {
                                var ea = 0 < que_assister_exited.Count ? que_assister_exited[0] : null;
                                if (ea != null)
                                {
                                    Log("VTAS,s[OF:" + ea.ToString() + "]");

                                    string reason = "";
                                    if (vtasl.OF(ea.exit_floor, ea.exit_side, ea.palette_no, out reason))
                                    {
                                        repaint = true;
                                        Log("VTAS,r[OF:o]");
                                        que_assister_exited.Remove(ea);

                                        vtas_step = 0;
                                    }
                                    else
                                    {
                                        Log("VTAS,r[OF:n][" + reason + "]");
                                        m_swTemp.Restart();
                                        vtas_step = 410;
                                    }
                                }
                            }
                            break;

                        case 410:
                            if (1000 <= m_swTemp.ElapsedMilliseconds)
                            {
                                vtas_step = 400;
                            }
                            break;
                    }
                }

                return base.DoControl(message);
            }



            override public bool RequestEntry(FloorAGV agv, string entry_floor, string entry_side, string exit_floor, string exit_side)
            {
                lock (this)
                {
                    if (0 < que_assister_waitrequest.ToList()
                        .Concat(que_assister_canentry)
                        .Concat(que_assister_entried)
                        .Concat(que_assister_waitexit)
                        .Concat(que_assister_canexit)
                        .Concat(que_assister_exited)
                        .Where(e => e.agv == agv).Count())
                    {
                        return true;
                    }

                    que_assister_waitrequest.Enqueue(new AutoratorInAgv(agv, entry_floor, entry_side, exit_floor, exit_side));
                }

                return true;
            }

            override public bool CanEntry(FloorAGV agv)
            {
                lock (this)
                {
                    return 0 < que_assister_canentry.Where(e => e.agv == agv).Count();
                }
            }

            override public bool CompleteEntry(FloorAGV agv)
            {
                lock (this)
                {
                    var ea = que_assister_canentry.Where(e => e.agv == agv).FirstOrDefault();
                    if (ea == null) return false;

                    que_assister_entried.Add(ea);
                    que_assister_canentry.Remove(ea);
                }

                return true;
            }

            override public bool CanExit(FloorAGV agv)
            {
                return 0 < que_assister_canexit.Where(e => e.agv == agv).Count();
            }

            override public bool CompleteExit(FloorAGV agv)
            {
                lock (this)
                {
                    var ea = que_assister_canexit.Where(e => e.agv == agv).FirstOrDefault();
                    if (ea == null) return false;

                    que_assister_exited.Add(ea);
                    que_assister_canexit.Remove(ea);
                }

                return true;
            }

            override public bool IsEntried(FloorAGV agv)
            {
                return 0 < que_assister_entried
                            .Concat(que_assister_waitexit)
                            .Concat(que_assister_canexit)
                            .Concat(que_assister_exited)
                            .Where(e => e.agv == agv).Count();
            }

            public override bool IsRequested(FloorAGV agv)
            {
                return 0 < que_assister_waitrequest
                            .Concat(que_assister_waitentry)
                            .Concat(que_assister_canentry)
                            .Concat(que_assister_entried)
                            .Concat(que_assister_waitexit)
                            .Concat(que_assister_canexit)
                            .Concat(que_assister_exited)
                            .Where(e => e.agv == agv).Count();
            }
        }
    }
}
