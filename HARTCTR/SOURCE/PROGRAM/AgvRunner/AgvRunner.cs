using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Net;
using System.Text;

using BelicsClass.Common;
using BelicsClass.File;
using BelicsClass.ProcessManage;
using BelicsClass.Network;

using AgvController;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        public class AgvConnector
        {
            public BL_RawSocketUDP send = null;
            public BL_RawSocketUDP recv = null;

            public string ip = "127.0.0.1";
            public int remote_host = 9000;
            public int remote_client = 9100;

            public IPEndPoint RemoteClientEP { get { return new IPEndPoint(IPAddress.Parse(ip), remote_client); } }
        }

        public class AgvRunner : BL_ThreadController_Base
        {
            #region フィールド・コンストラクタ

            public PointF current = new PointF();
            public float x
            {
                get { return current.X; }
                set
                {
                    current.X = value;
                    SetLocation("", current);
                }
            }
            public float y
            {
                get { return current.Y; }
                set
                {
                    current.Y = value;
                    SetLocation("", current);
                }
            }

            public PointF Location
            {
                get { return current; }
            }

            public void SetLocation(string floorcode, PointF location)
            {
                SetLocation(floorcode, location, 9999);
            }

            public void SetLocation(string floorcode, PointF location, int rack_deg)
            {
                current = location;

                agv.SetLocation(floorcode, current, rack_deg);
            }

            public int Degree
            {
                get { return agv.degree; }
                set { agv.degree = value; }
            }
            
            public RouteCondition cur_con = null;
            protected FloorAGV agv = null;
            protected FloorMap floor
            {
                get
                {
                    if (agv != null) return agv.floor;
                    return null;
                }
            }

            protected AgvMapEditor mapeditor
            {
                get
                {
                    if (agv != null) return agv.floor.mapeditor;
                    return null;
                }
            }
            protected AgvConditioner conditioner
            {
                get
                {
                    if (agv != null) return agv.floor.conditioner;
                    return null;
                }
            }

            public int moveMode_now = 0;

            double degree_agv = -999;
            double degree_rack = -999;

            public enState state = enState.STOP;
            public AgvRunManager manager = null;

            public bool agv_stop = false;
            public bool agv_restart = false;
            public bool agv_request = false;
            public bool agv_order_response = false;
            public bool agv_state = false;
            public bool agv_route_cancel = false;

            public bool state_changed = false;
            public bool state_changed_disp = false;
            public bool state_received = false;

            public List<AgvOrderCommunicator.RequestBase> requests = new List<AgvOrderCommunicator.RequestBase>();

            public bool can_order
            {
                get
                {
                    return cur_con == null && agv.conditions.Count == 0;
                }
            }

            public AgvConnector connector = null;
            public AgvCommunicator communicator = null;

            public virtual string state_string
            {
                get
                {
                    string s = step.ToString() + " ";

                    if (state == enState.NONE)
                    {
                        s += BL_EnumLabel.GetLabel(enState.STOP);
                    }
                    else
                    {
                        if ((state & enState.STOP) != 0)
                        {
                            if (s != "") s += "|";
                            s += BL_EnumLabel.GetLabel(enState.STOP);
                        }

                        if ((state & enState.WAIT) != 0)
                        {
                            if (s != "") s += "|";
                            s += BL_EnumLabel.GetLabel(enState.WAIT);
                        }

                        if ((state & enState.MOVE) != 0)
                        {
                            if (s != "") s += "|";
                            s += BL_EnumLabel.GetLabel(enState.MOVE);
                        }

                        if ((state & enState.TURN) != 0)
                        {
                            if (s != "") s += "|";
                            s += BL_EnumLabel.GetLabel(enState.TURN);
                        }

                        if ((state & enState.RACK) != 0)
                        {
                            if (s != "") s += "|";
                            s += BL_EnumLabel.GetLabel(enState.RACK);
                        }

                        if ((state & enState.CHARGE) != 0)
                        {
                            if (s != "") s += "|";
                            s += BL_EnumLabel.GetLabel(enState.CHARGE);
                        }

                        if ((state & enState.WAIT_STATION) != 0)
                        {
                            if (s != "") s += "|";
                            s += BL_EnumLabel.GetLabel(enState.WAIT_STATION);
                        }

                        if ((state & enState.WAIT_AUTORATOR_IN) != 0)
                        {
                            if (s != "") s += "|";
                            s += BL_EnumLabel.GetLabel(enState.WAIT_AUTORATOR_IN);
                        }

                        if ((state & enState.WAIT_AUTORATOR_OUT) != 0)
                        {
                            if (s != "") s += "|";
                            s += BL_EnumLabel.GetLabel(enState.WAIT_AUTORATOR_OUT);
                        }

                        if ((state & enState.WAIT_CRASH) != 0)
                        {
                            if (s != "") s += "|";
                            s += BL_EnumLabel.GetLabel(enState.WAIT_CRASH);
                        }

                        if ((state & enState.WAIT_OTHER_AGVS) != 0)
                        {
                            if (s != "") s += "|";
                            s += BL_EnumLabel.GetLabel(enState.WAIT_OTHER_AGVS);
                        }
                    }

                    if (agv_order_response)
                    {
                        if (s != "") s += "|";
                        s += "指示";
                    }

                    if (agv_request)
                    {
                        if (s != "") s += "|";
                        s += "要求";
                    }

                    if (agv_stop)
                    {
                        if (s != "") s += "|";
                        s += "停止";
                    }

                    if (agv_restart)
                    {
                        if (s != "") s += "|";
                        s += "再開";
                    }

                    if (agv_route_cancel)
                    {
                        if (s != "") s += "|";
                        s += "ｸﾘｱ";
                    }

                    return s;
                }
            }

            public AgvDatabaseManager db_man = null;

            public AgvRunner(AgvRunManager manager, FloorAGV agv)
                : base("AgvRunner_" + agv.id)
            {
                this.manager = manager;
                this.agv = agv;

                current = agv.Location;
            }

            //protected int step = 0;
            public int step
            {
                get { return m_Step; }
                set { m_Step = value; }
            }

            protected BL_Stopwatch swWaitTimer = new BL_Stopwatch();

            #endregion

            public virtual void Start()
            {
                m_Log = new BL_Log("", m_ID.ToString());

                if (!typeof(AgvAutoCommander).IsInstanceOfType(this)
                 && !typeof(AgvRouteCommanderFA).IsInstanceOfType(this)
                 && !typeof(AgvModeCommander).IsInstanceOfType(this))
                {
                    //シミュレーター動作の場合は、要求状態を初期状態にする
                    agv_request = true;
                }

                //base.StartControl(200);
            }

            public virtual void Stop()
            {
                if (m_Log != null)
                {
                    m_Log.Dispose();
                    m_Log = null;
                }

                //base.StopControl();
            }

            protected override bool DoControl(object message)
            {
                _DoControl(message);

                return base.DoControl(message);
            }

            public virtual void _DoControl(object message)
            {
                switch (Step)
                {
                    case 0:
                        #region 次移動定義取得

                        if (cur_con == null)
                        {
                            if (moveMode_now != agv.moveMode)
                            {
                                if (0 < agv.mode_conditions[agv.moveMode].Count)
                                {
                                    if (agv.on_qr == agv.mode_conditions[agv.moveMode][0].cur_qr)
                                    {
                                        agv_request = false;

                                        manager.Log("動作モード変更[" + agv.moveMode.ToString() + "]");
                                        moveMode_now = agv.moveMode;
                                    }
                                    else state = enState.STOP;
                                }
                                else
                                {
                                    manager.Log("動作モード変更[" + agv.moveMode.ToString() + "]");
                                    moveMode_now = agv.moveMode;
                                    state = enState.STOP;
                                }
                            }
                            else if (1 < agv.mode_conditions[moveMode_now].Count)
                            {
                                if (agv.on_qr == agv.mode_conditions[moveMode_now][0].cur_qr)
                                {
                                    agv_request = false;
                                    cur_con = agv.mode_conditions[moveMode_now][0];

                                    lock (agv)
                                    {
                                        cur_con.LockWay();
                                    }

                                    manager.Log("次移動地点ループ[" + cur_con.ToString() + "]");
                                    step = 100;
                                }
                                else
                                {
                                    if (moveMode_now == 0) agv.mode_conditions[moveMode_now].Clear();

                                    agv_request = true;
                                    state = enState.STOP;
                                }
                            }
                            else
                            {
                                agv_request = true;
                                state = enState.STOP;
                            }
                        }
                        else
                        {
                            lock (agv)
                            {
                                if (cur_con != null)
                                {
                                    var crash_con = cur_con.LockWay();
                                    if (crash_con != null)
                                    {
                                        state = enState.WAIT;
                                        break;
                                    }

                                    state = enState.MOVE;
                                }
                            }

                            step = 100;
                        }

                        #endregion
                        break;

                    case 100:
                        #region 到着時旋回処理

                        if (cur_con != null)
                        {
                            degree_agv = -999;
                            degree_rack = -999;

                            if (cur_con.agv_turn_arrive && cur_con.rack_turn_arrive)
                            {
                                degree_agv = DegreeAGV(cur_con.agv_turn_arrive_degree);
                                degree_rack = DegreeRack(cur_con.rack_turn_arrive_degree);

                                step = 110;
                            }
                            else if (cur_con.agv_turn_arrive)
                            {
                                degree_agv = DegreeAGV(cur_con.agv_turn_arrive_degree);

                                step = 110;
                            }
                            else if (cur_con.rack_turn_arrive)
                            {
                                degree_rack = DegreeRack(cur_con.rack_turn_arrive_degree);

                                step = 110;
                            }
                            else
                            {
                                step = 200;
                            }
                        }
                        else step = 0;
                        break;

                    case 110:
                        if (cur_con != null)
                        {
                            state = enState.TURN;
                            if (Turn(agv, degree_agv, degree_rack))
                            {
                                step = 200;
                            }
                        }
                        else step = 0;

                        #endregion
                        break;

                    case 200:
                        #region 到着時棚操作

                        if (cur_con != null)
                        {
                            if (cur_con.rack_down_arrive)
                            {
                                state = enState.RACK;
                                agv.RackDown();
                            }
                            else if (cur_con.rack_up_arrive)
                            {
                                state = enState.RACK;
                                agv.RackUp();
                            }

                            step = 300;
                        }
                        else step = 0;

                        #endregion
                        break;

                    case 300:
                        #region 条件成立待機
                        {
                            if (cur_con.wait_station_trg)
                            {
                                if (!cur_con.cur_qr.station_complete_trigger)
                                {
                                    state = enState.WAIT | enState.WAIT_STATION;
                                    cur_con.UnlockWay(true);
                                    break;
                                }
                            }

                            if (cur_con.wait_autorator_in_trg)
                            {
                                RouteCondition con = FindAutoratorQR(cur_con);

                                if (con != null)
                                {
                                    FloorQR qr_target = con.cur_qr;
                                    if (qr_target.autoratorIN_trigger == "")
                                    {
                                        state = enState.WAIT | enState.WAIT_AUTORATOR_IN;
                                        cur_con.UnlockWay(true);
                                        break;
                                    }
                                }
                            }

                            if (cur_con.wait_autorator_out_trg)
                            {
                                RouteCondition con = FindAutoratorQR(cur_con);

                                if (con != null)
                                {
                                    FloorQR qr_target = con.cur_qr;
                                    if (qr_target.autoratorOUT_trigger == "")
                                    {
                                        state = enState.WAIT | enState.WAIT_AUTORATOR_OUT;
                                        cur_con.UnlockWay(true);
                                        break;
                                    }
                                }
                            }

                            if (cur_con.wait_charge_trg)
                            {
                                if (!cur_con.cur_qr.charge_complete_trigger)
                                {
                                    state = enState.WAIT | enState.CHARGE;
                                    cur_con.UnlockWay(true);
                                    break;
                                }
                            }
                        }

                        if (!conditioner.keep_state)
                        {
                            if (cur_con.wait_autorator_in_trg)
                            {
                                RouteCondition con = FindAutoratorQR(cur_con);

                                if (con != null)
                                {
                                    FloorQR qr_target = con.cur_qr;
                                    qr_target.autoratorIN_trigger = "";
                                    if (floor.controller.EventTrigger != null) floor.controller.EventTrigger(qr_target, "", "IN");
                                }
                            }
                            if (cur_con.wait_autorator_out_trg)
                            {
                                RouteCondition con = FindAutoratorQR(cur_con);

                                if (con != null)
                                {
                                    FloorQR qr_target = con.cur_qr;
                                    qr_target.autoratorOUT_trigger = "";
                                    if (floor.controller.EventTrigger != null) floor.controller.EventTrigger(qr_target, "", "OUT");
                                }
                            }
                            if (cur_con.wait_station_trg)
                            {
                                cur_con.cur_qr.station_complete_trigger = false;
                                if (floor.controller.EventTrigger != null) floor.controller.EventTrigger(cur_con.cur_qr, "", "");
                            }
                            if (cur_con.wait_charge_trg)
                            {
                                cur_con.cur_qr.charge_complete_trigger = false;
                                if (floor.controller.EventTrigger != null) floor.controller.EventTrigger(cur_con.cur_qr, "", "");
                            }
                        }

                        if (0 < cur_con.wait_timer)
                        {
                            state = enState.WAIT | enState.WAIT;
                            swWaitTimer.Restart();
                            step = 310;
                            break;
                        }

                        step = 400;
                        break;

                    case 310:
                        if (cur_con.wait_timer < swWaitTimer.ElapsedMilliseconds)
                        {
                            swWaitTimer.Stop();
                            step = 400;
                        }

                        #endregion
                        break;

                    case 400:
                        #region 出発時旋回処理(到着時に棚下降していたら棚上昇する)

                        if (cur_con != null)
                        {
                            if (cur_con.rack_turn_departure && cur_con.rack_down_arrive && agv.rack == null && cur_con.cur_qr.rack != null)
                            {
                                state = enState.RACK;
                                agv.RackUp();
                            }

                            degree_agv = -999;
                            degree_rack = -999;

                            if (cur_con.agv_turn_departure && cur_con.rack_turn_departure)
                            {
                                degree_agv = DegreeAGV(cur_con.agv_turn_departure_degree);
                                degree_rack = DegreeRack(cur_con.rack_turn_departure_degree);

                                step = 410;
                            }
                            else if (cur_con.agv_turn_departure)
                            {
                                degree_agv = DegreeAGV(cur_con.agv_turn_departure_degree);

                                step = 410;
                            }
                            else if (cur_con.rack_turn_departure)
                            {
                                degree_rack = DegreeRack(cur_con.rack_turn_departure_degree);

                                step = 410;
                            }
                            else
                            {
                                step = 500;
                            }
                        }
                        else step = 0;
                        break;

                    case 410:
                        if (cur_con != null)
                        {
                            state = enState.TURN;
                            if (Turn(agv, degree_agv, degree_rack))
                            {
                                step = 500;
                            }
                        }
                        else step = 0;

                        #endregion
                        break;

                    case 500:
                        #region 出発時棚操作

                        if (cur_con != null)
                        {
                            if (cur_con.rack_down_departure)
                            {
                                state = enState.RACK;
                                agv.RackDown();
                            }
                            else if (cur_con.rack_up_departure)
                            {
                                state = enState.RACK;
                                agv.RackUp();
                            }

                            step = 600;
                        }
                        else step = 0;

                        #endregion
                        break;

                    case 600:
                        #region 出発時旋回処理

                        if (cur_con != null)
                        {
                            degree_rack = -999;
                            degree_agv = -999;

                            if (cur_con.next_condition != null)
                            {
                                if (agv.floor.code != cur_con.next_condition.FloorCode)
                                {
                                    step = 700;
                                    break;
                                }
                                
                                degree_agv = DegreeAGV(agv.on_qr.Degree(cur_con.next_condition.Location));
                            }

                            if (cur_con.rack_turn_departure)
                            {
                                degree_rack = DegreeRack(cur_con.rack_turn_departure_degree);
                            }

                            step = 610;
                        }
                        else step = 0;
                        break;

                    case 610:
                        if (cur_con != null)
                        {
                            state = enState.TURN;
                            if (Turn(agv, degree_agv, degree_rack))
                            {
                                if (cur_con.rack_down_departure_last)
                                {
                                    agv.RackDown();
                                }

                                step = 700;
                            }
                        }
                        else step = 0;

                        #endregion
                        break;

                    case 700:
                        #region 次移動地点取得

                        if (cur_con != null)
                        {
                            bool do_next = true;
                            foreach (var v in cur_con.wait_other_agv_locations)
                            {
                                if (!v.Key.HitTest(v.Value))
                                {
                                    do_next = false;
                                    break;
                                }
                            }

                            if (do_next)
                            {
                                if (cur_con.next_condition == null)
                                {
                                    cur_con.UnlockWay(true);
                                    cur_con = null;


                                    //同一モード繰り返し
                                    if (moveModes[moveMode_now].option.Contains("NEXT"))
                                    {
                                        int nextmode = (moveMode_now + 1) % moveModes.Count;
                                        if (nextmode == 0) nextmode = 1;
                                        agv.moveMode = nextmode;
                                    }
                                    else if (moveModes[moveMode_now].option.Contains("REPEAT"))
                                    {
                                    }
                                    else
                                    {
                                        agv.moveMode = moveModes[0].mode;
                                    }

                                    step = 0;
                                }
                                else
                                {
                                    cur_con.UnlockWay(false);

                                    cur_con = cur_con.next_condition;
                                    manager.Log("次移動地点取得[" + cur_con.ToString() + "]");
                                    step = 800;
                                }
                            }
                            else
                            {
                                state = enState.WAIT;
                                cur_con.UnlockWay(true);
                            }
                        }
                        else step = 0;

                        #endregion
                        break;

                    case 800:
                        #region 移動処理

                        if (cur_con != null)
                        {
                            state = enState.MOVE;

                            if (cur_con.cur_qr.floor != agv.floor)
                            {
                                agv.floor = cur_con.cur_qr.floor;
                                step = 900;
                            }
                            else
                            {
                                step = 810;
                            }
                        }
                        break;

                    case 810:

                        if (cur_con != null)
                        {
                            state = enState.MOVE;

                            double distance = agv.Distance(cur_con.Location);
                            if (distance < 2.0)
                            {
                                step = 900;
                            }
                            else
                            {
                                enSpeed speed = enSpeed.ML;
                                if (cur_con.prev_condition != null) speed = cur_con.prev_condition.speed;

                                degree_agv = agv.Degree(cur_con.Location);

                                if (distance < 10)
                                {
                                    SetLocation("", agv.AngleDistancePoint(Location, degree_agv, 1));
                                }
                                else if (speed == enSpeed.HM)
                                {
                                    if (distance < 50)
                                    {
                                        SetLocation("", agv.AngleDistancePoint(Location, degree_agv, (int)((double)enSpeed.M * (agv.rack != null ? 1.0 : 0.75))));
                                    }
                                    else
                                    {
                                        SetLocation("", agv.AngleDistancePoint(Location, degree_agv, (int)((double)enSpeed.HM * (agv.rack != null ? 1.0 : 0.75))));
                                    }
                                }
                                else if (speed == enSpeed.ML)
                                {
                                    if (distance < 20)
                                    {
                                        SetLocation("", agv.AngleDistancePoint(Location, degree_agv, 0.5));
                                    }
                                    else if (distance < 50)
                                    {
                                        SetLocation("", agv.AngleDistancePoint(Location, degree_agv, (int)((double)enSpeed.ML * (agv.rack != null ? 1.0 : 0.75))));
                                    }
                                    else
                                    {
                                        SetLocation("", agv.AngleDistancePoint(Location, degree_agv, (int)((double)enSpeed.M * (agv.rack != null ? 1.0 : 0.75))));
                                    }
                                }
                                else
                                {
                                    SetLocation("", agv.AngleDistancePoint(Location, degree_agv, (int)((double)speed * (agv.rack != null ? 1.0 : 0.75))));
                                }
                            }
                        }
                        else step = 0;

                        #endregion
                        break;

                    case 900:
                        #region 次移動地点到着

                        if (cur_con != null)
                        {
                            agv.SetLocation("", cur_con.Location);

                            lock (agv)
                            {
                                if (cur_con != null)
                                {
                                    var crash_con = cur_con.LockWay();
                                    if (crash_con != null)
                                    {
                                        state = enState.WAIT;
                                        break;
                                    }

                                    state = enState.MOVE;
                                }
                            }

                            manager.Log("次移動地点到着[" + cur_con.ToString() + "]");

                            step = 0;
                        }
                        else step = 0;

                        #endregion
                        break;
                }
            }

            public double DegreeRack(double deg)
            {
                lock (agv)
                {
                    if (agv.rack != null)
                    {
                        degree_rack = (deg + 360) % 360;
                        agv.rack.degree = (agv.rack.degree + 360) % 360;
                        if (180 < degree_rack - agv.rack.degree) agv.rack.degree += 360;
                        if (degree_rack - agv.rack.degree < -180) degree_rack += 360;

                        return degree_rack;
                    }
                }

                return -999;
            }

            public double DegreeAGV(double deg)
            {
                degree_agv = (deg + 360) % 360;
                agv.degree = (agv.degree + 360) % 360;
                if (180 < degree_agv - agv.degree) agv.degree += 360;
                if (degree_agv - agv.degree < -180) degree_agv += 360;

                return degree_agv;
            }

            public bool Turn(FloorAGV agv, double agv_degree, double rack_degree)
            {
                if (agv.rack == null)
                {
                    if ((int)degree_agv == agv.degree || degree_agv <= -900)
                    {
                        return true;
                    }
                    else
                    {
                        if (degree_agv < agv.degree)
                        {
                            agv.degree--;
                        }
                        else
                        {
                            agv.degree++;
                        }
                    }
                }
                else
                {
                    if (!cur_con.cur_qr.rack_rotatable)
                    {
                        if ((int)degree_agv == agv.degree || degree_agv <= -900)
                        {
                            return true;
                        }
                        else
                        {
                            if (degree_agv < agv.degree)
                            {
                                agv.degree--;
                            }
                            else
                            {
                                agv.degree++;
                            }
                        }
                    }
                    else
                    {
                        if (((int)degree_rack == agv.rack.degree || degree_rack <= -900) && ((int)degree_agv == agv.degree || degree_agv <= -900))
                        {
                            return true;
                        }
                        else if ((int)degree_rack == agv.rack.degree || degree_rack <= -900)
                        {
                            if (degree_agv < agv.degree)
                            {
                                agv.degree--;
                            }
                            else
                            {
                                agv.degree++;
                            }
                        }
                        else if ((int)degree_agv == agv.degree || degree_agv <= -900)
                        {
                            if (degree_rack < agv.rack.degree)
                            {
                                agv.rack.degree--;
                            }
                            else
                            {
                                agv.rack.degree++;
                            }
                        }
                        else
                        {
                            if (degree_agv < agv.degree)
                            {
                                agv.degree--;
                                //if (!stay_turn) agv.rack.degree--;
                            }
                            else
                            {
                                agv.degree++;
                                //if (!stay_turn) agv.rack.degree++;
                            }

                            if (degree_rack < agv.rack.degree)
                            {
                                agv.rack.degree--;
                            }
                            else
                            {
                                agv.rack.degree++;
                            }
                        }
                    }
                }

                return false;
            }

            public RouteCondition FindAutoratorQR(RouteCondition cur_con)
            {
                RouteCondition con_target = null;
                //FloorAGV.Condition con = cur_con.next_condition;
                RouteCondition con = cur_con;

                for (int ways = 0; ways < agv.mode_conditions[moveMode_now].Count; ways++)
                {
                    if (con == null)
                    {
                        con = agv.mode_conditions[moveMode_now][0];
                    }

                    if (con == null) break;

                    if (con.cur_qr.autorator_id != "")
                    {
                        con_target = con;
                        break;
                    }

                    con = con.next_condition;
                }

                return con_target;
            }
        }
    }
}
