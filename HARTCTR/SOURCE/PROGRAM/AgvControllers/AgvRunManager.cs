#define ROUTECOMMANDER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Threading;
using System.Net;
using System.IO;
using System.Windows.Forms;

using BelicsClass.Common;
using BelicsClass.File;
using BelicsClass.ProcessManage;
using BelicsClass.Network;

using AgvController;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        #region enum

        public enum enState
        {
            [BL_EnumLabel("なし")]
            NONE = 0x0000,
            [BL_EnumLabel("停止")]
            STOP = 0x0001,
            [BL_EnumLabel("待機")]
            WAIT = 0x0002,
            [BL_EnumLabel("移動")]
            MOVE = 0x0004,
            [BL_EnumLabel("旋回")]
            TURN = 0x0008,
            [BL_EnumLabel("棚")]
            RACK = 0x0010,
            [BL_EnumLabel("充電")]
            CHARGE = 0x0020,

            [BL_EnumLabel("ST待機")]
            WAIT_STATION = 0x0040,
            [BL_EnumLabel("AT-IN待機")]
            WAIT_AUTORATOR_IN = 0x0080,
            [BL_EnumLabel("AT-OUT待機")]
            WAIT_AUTORATOR_OUT = 0x0100,
            [BL_EnumLabel("衝突回避待機")]
            WAIT_CRASH = 0x0200,
            [BL_EnumLabel("他AGV同期待機")]
            WAIT_OTHER_AGVS = 0x0400,
        }

        #endregion

        public class AgvRunManager : BL_ThreadController_Base
        {
            public delegate void RequestExitEventHandler(AgvRunManager sender);
            public event RequestExitEventHandler RequestExit;

            public delegate void RequestResetEventHandler(AgvRunManager sender);
            public event RequestResetEventHandler RequestReset;

            public int BATTERY_NOP = 10;
            public int BATTERY_LOW = 10;
            public int BATTERY_MID = 60;
            public int BATTERY_FUL = 90;
            public int CHARGE_START_LEVEL = 70;
            public int WAITTIME = 300;
            public int CHARGE_ABORT_LEVEL = 30;

            AgvControlManager controller = null;
            public Dictionary<FloorAGV, AgvRunner> runners = new Dictionary<FloorAGV, AgvRunner>();

            AgvConnector connector = new AgvConnector();

            public AgvOrderCommunicator order_com = null;

            private SortedDictionary<int, int> dictLevelCount = new SortedDictionary<int, int>();
            private SortedDictionary<int, List<FloorAGV>> dictLevelAgvs = new SortedDictionary<int, List<FloorAGV>>();

            //作業指示リスト
            static public List<AgvOrderCommunicator.RequestBase> requests = new List<AgvOrderCommunicator.RequestBase>();

            public Dictionary<string, AgvControlManager.AgvInfo> agvid_list = new Dictionary<string, AgvControlManager.AgvInfo>();
            public Dictionary<string, AutoratorController> dict_autorator = new Dictionary<string, AutoratorController>();


            public AgvRunManager(AgvControlManager controller)
                : base("AgvRunManager")
            {
                this.controller = controller;
            }

            #region スレッド制御

            public override string StartControl(int sleep, ThreadPriority priority)
            {
                Log("START");
                agvid_list.Clear();
                dict_autorator.Clear();

                BL_IniFile ini = Program.ini_agv;

                BATTERY_NOP = Program.ini_hokusho.Read("BATTERY", "BATTERY_LOW", BATTERY_LOW);
                BATTERY_LOW = Program.ini_hokusho.Read("BATTERY", "BATTERY_LOW", BATTERY_LOW);
                BATTERY_MID = Program.ini_hokusho.Read("BATTERY", "BATTERY_MID", BATTERY_MID);
                BATTERY_FUL = Program.ini_hokusho.Read("BATTERY", "BATTERY_FUL", BATTERY_FUL);
                CHARGE_START_LEVEL = Program.ini_hokusho.Read("BATTERY", "START", CHARGE_START_LEVEL);
                WAITTIME = Program.ini_hokusho.Read("BATTERY", "WAITTIME", WAITTIME) * 60;
                CHARGE_ABORT_LEVEL = Program.ini_hokusho.Read("BATTERY", "ABORT", CHARGE_ABORT_LEVEL);


                for (int level = 1; level <= 100; level++)
                {
                    int count = Program.ini_hokusho.Read("BATTERY", "PER" + level.ToString(), 0);
                    if (0 < count)
                    {
                        dictLevelCount[level] = count;
                        dictLevelAgvs[level] = new List<FloorAGV>();
                    }
                }

                int local_client = ini.Read("PC", "LOCAL_CLIENT", 9200);
                int local_host = ini.Read("PC", "LOCAL_HOST", 9300);
                if (connector.recv != null) connector.recv.Close();
                if (connector.send != null) connector.send.Close();
                connector.recv = new BL_RawSocketUDP(local_host);
                connector.send = new BL_RawSocketUDP(local_client);
                connector.recv.Open(BL_RawSocketUDP.FormatType.BL_IDSIZE);
                connector.send.Open(BL_RawSocketUDP.FormatType.BL_IDSIZE);

                //connector.recv.ReceiveEvent += Recv_ReceiveEvent;

                agvid_list.Clear();

                for (int i = 1; ; i++)
                {
                    AgvControlManager.AgvInfo d = new AgvControlManager.AgvInfo();
                    d.id = Program.ini_agv.Read(i.ToString(), "ID", "");
                    if (d.id == "") break;

                    d.ipaddress = Program.ini_agv.Read(i.ToString(), "IP", "");
                    d.hostport = Program.ini_agv.Read(i.ToString(), "REMOTE_HOST", 9000);
                    d.clientport = Program.ini_agv.Read(i.ToString(), "REMOTE_CLIENT", 9100);
                    d.stop_distance = Program.ini_agv.Read(i.ToString(), "STOP_DISTANCE", 500);
                    d.radius = Program.ini_agv.Read(i.ToString(), "RADIUS", 102);

                    agvid_list[d.id] = d;
                }

                foreach (var v in controller.agvs)
                {
                    var agv = agvid_list.Values.Where(e => e.id == v.id).FirstOrDefault();
                    if (agv != null)
                    {
                        v.radius = agv.radius / 2;
                    }
                }

                foreach (var v in controller.agvs)
                {
                    AgvRunnerStart(v);
                }

                order_com = new AgvOrderCommunicator(12000);
                order_com.ReceiveEvent += Order_com_ReceiveEvent;

                //オートレーター通信初期化
                {
                    dict_autorator.Clear();
                    var autorators = controller.AllAutoratorQR;
                    foreach (var v in autorators)
                    {
                        if (v.autorator_info != null)
                        {
                            v.autorator_info.StopControl();
                            v.autorator_info = null;
                        }
                    }

                    controller.LoadAutorator();

                    foreach (var v in autorators)
                    {
                        if (!dict_autorator.ContainsKey(v.autorator_id))
                        {
                            dict_autorator[v.autorator_id] = v.autorator_info;
                        }
                    }

                    foreach (var kv in dict_autorator)
                    {
                        if (kv.Value.IsDoing) kv.Value.StopControl();
                        kv.Value.StartControl(50);
                    }
                }

                return base.StartControl(sleep, priority);
            }

            private void AgvRunnerStart(FloorAGV agv)
            {
                BL_IniFile ini = Program.ini_agv;

                switch (controller.applicate_mode)
                {
                    case enApplicateMode.MODE_SIMULATOR:
                    case enApplicateMode.MANUAL_SIMULATOR:
                    case enApplicateMode.OPERATION_SIMULATOR:
                        {
                            if (0 < agv.conditions.Count)
                            {
                                agv.floor = agv.conditions[0].cur_qr.floor;
                                agv.SetLocation("", agv.conditions[0].Location);
                                agv.conditions[0].UnlockWay(true);
                            }

                            agv.agvRunner = new AgvRunner(this, agv);
                            agv.agvRunner.Start();

                            runners[agv] = agv.agvRunner;
                        }
                        break;

                    case enApplicateMode.MODE_COMMANDER:
                        {
                            if (0 < agv.conditions.Count)
                            {
                                agv.SetLocation("", agv.conditions[0].Location);
                                agv.conditions[0].UnlockWay(true);
                            }

                            if (agvid_list.ContainsKey(agv.id))
                            {
                                connector.ip = agvid_list[agv.id].ipaddress;
                                connector.remote_host = agvid_list[agv.id].hostport;
                                connector.remote_client = agvid_list[agv.id].clientport;

                                agv.agvRunner = new AgvModeCommander(this, agv, connector);
                                agv.agvRunner.Start();

                                runners[agv] = agv.agvRunner;
                            }
                        }
                        break;

                    case enApplicateMode.AUTO_COMMANDER:
                        {
                            if (0 < agv.conditions.Count)
                            {
                                agv.SetLocation("", agv.conditions[0].Location);
                                agv.conditions[0].UnlockWay(true);
                            }

                            if (agvid_list.ContainsKey(agv.id))
                            {
                                connector.ip = agvid_list[agv.id].ipaddress;
                                connector.remote_host = agvid_list[agv.id].hostport;
                                connector.remote_client = agvid_list[agv.id].clientport;

#if ROUTECOMMANDER
                                agv.agvRunner = new AgvRouteCommanderFA(this, agv, connector);
#else
                                agv.agvRunner = new AgvAutoCommander(this, agv, connector);
#endif
                                agv.agvRunner.Start();

                                runners[agv] = agv.agvRunner;
                            }
                        }
                        break;
                }
            }

            //private void Recv_ReceiveEvent(object sender, BL_RawSocketUDP.ReceiveEventArgs e)
            //{
            //    AgvCommunicator.State state = new AgvCommunicator.State();
            //    state.SetBytes(e.BytesData);

            //    var target_agvs = controller.agvs.Where(x => x.agvRunner != null && x.agvRunner.communicator.RemoteClient.ToString() == e.SenderEndPoint.ToString());
            //    if (0 == target_agvs.Count())
            //    {
            //        FloorMap floor = controller.SelectFloor(state.map);
            //        if (floor != null)
            //        {
            //            var agvids = agvid_list.Where(x => x.Value.ipaddress == e.SenderEndPoint.Address.ToString() && x.Value.clientport == e.SenderEndPoint.Port);
            //            if (0 < agvids.Count())
            //            {
            //                var agvid = agvids.First();

            //                FloorAGV newagv = new FloorAGV(floor, state.x, state.y);
            //                newagv.id = agvid.Value.id;

            //                floor.conditioner.AddAGV(newagv);

            //                AgvRunnerStart(newagv);
            //            }
            //        }
            //    }

            //    foreach (var v in target_agvs)
            //    {
            //        AgvAutoCommander commander = v.agvRunner as AgvAutoCommander;
            //        if (commander != null)
            //        {
            //            lock (commander.queState)
            //            {
            //                commander.communicator.GetState.SetBytes(e.BytesData);
            //                commander.queState.Enqueue(state);
            //            }
            //        }
            //    }
            //}

            public override void StopControl()
            {
                base.StopControl();

                if (connector.send != null)
                {
                    connector.send.Close();
                    connector.send = null;
                }

                if (connector.recv != null)
                {
                    //connector.recv.ReceiveEvent -= Recv_ReceiveEvent;
                    connector.recv.Close();
                    connector.recv = null;
                }

                if (order_com != null)
                {
                    order_com.ReceiveEvent -= Order_com_ReceiveEvent;
                    order_com.Dispose();
                }

                foreach (var kv in dict_autorator)
                {
                    kv.Value.StopControl();
                }

                dict_autorator.Clear();

                foreach (var kv in runners)
                {
                    if (0 < kv.Key.conditions.Count)
                    {
                        kv.Key.SetLocation("", kv.Key.conditions[0].cur_qr.Location);

                        kv.Key.conditions[0].UnlockWay(true);
                    }

                    if (kv.Key.agvRunner != null)
                    {
                        kv.Key.agvRunner.Stop();
                        kv.Key.agvRunner = null;
                    }
                }

                runners.Clear();

                Log("STOP");
            }

            private BL_HRTimer battery_check_timer = new BL_HRTimer();
            private BL_HRTimer request_check_timer = new BL_HRTimer();

            protected override bool DoControl(object message)
            {
                BL_HRTimer sw = new BL_HRTimer();
                sw.Restart();

                #region 全ＡＧＶのバッテリー残量チェック

                if (!battery_check_timer.IsRunning || 2000 < battery_check_timer.ElapsedMilliseconds)
                {
                    battery_check_timer.Restart();

                    BATTERY_NOP = Program.ini_hokusho.Read("BATTERY", "BATTERY_LOW", BATTERY_LOW);
                    BATTERY_LOW = Program.ini_hokusho.Read("BATTERY", "BATTERY_LOW", BATTERY_LOW);
                    BATTERY_MID = Program.ini_hokusho.Read("BATTERY", "BATTERY_MID", BATTERY_MID);
                    BATTERY_FUL = Program.ini_hokusho.Read("BATTERY", "BATTERY_FUL", BATTERY_FUL);
                    CHARGE_START_LEVEL = Program.ini_hokusho.Read("BATTERY", "START", CHARGE_START_LEVEL);
                    WAITTIME = Program.ini_hokusho.Read("BATTERY", "WAITTIME", WAITTIME) * 60;
                    CHARGE_ABORT_LEVEL = Program.ini_hokusho.Read("BATTERY", "ABORT", CHARGE_ABORT_LEVEL);

                    dictLevelCount.Clear();
                    dictLevelAgvs.Clear();
                    for (int level = 1; level <= 100; level++)
                    {
                        int count = Program.ini_hokusho.Read("BATTERY", "PER" + level.ToString(), 0);
                        if (0 < count)
                        {
                            dictLevelCount[level] = count;
                            dictLevelAgvs[level] = new List<FloorAGV>();
                        }
                    }


                    foreach (var kv in runners)
                    {
                        foreach (var kvv in dictLevelAgvs)
                        {
                            int level = kvv.Key;

                            if (kv.Value.communicator == null) continue;

                            if (kv.Value.communicator.GetState.bat < level)
                            {
                                foreach (var kvvv in dictLevelAgvs)
                                {
                                    if (level <= kvvv.Key) kvvv.Value.Add(kv.Key);
                                }
                                break;
                            }
                        }
                    }

                    foreach (var kv in dictLevelAgvs)
                    {
                        int count = dictLevelCount[kv.Key];
                        if (count <= kv.Value.Count)
                        {
                            this.BATTERY_LOW = kv.Key;
                            break;
                        }
                    }
                }

                #endregion

                #region 要求処理

                if (!request_check_timer.IsRunning || 100 <= request_check_timer.ElapsedMilliseconds)
                {
                    request_check_timer.Restart();

                    lock (requests)
                    {
                        List<AgvOrderCommunicator.RequestBase> requested = new List<AgvOrderCommunicator.RequestBase>();
                        foreach (var req in requests)
                        {
                            if (req.cmd == "FIN")
                            {
                                if (RequestExit != null) RequestExit(this);

                                requested.Add(req);
                            }
                            else if (req.cmd == "RST")
                            {
                                AgvOrderCommunicator.RequestReset r = req as AgvOrderCommunicator.RequestReset;
                                if (r != null)
                                {
                                    if (r.agv.Trim() == "")
                                    {
                                        if (RequestReset != null) RequestReset(this);

                                        req.result = "OK";
                                        OrderSend(req);
                                    }
                                    else
                                    {
                                        var reset_agv = runners.Where(e => e.Key.id.Trim() == r.agv.Trim()).FirstOrDefault();
                                        if (reset_agv.Key != null && reset_agv.Key.agvRunner != null)
                                        {
                                            if (0 < reset_agv.Key.conditions.Count)
                                            {
                                                reset_agv.Key.SetLocation("", reset_agv.Key.conditions[0].cur_qr.Location);

                                                reset_agv.Key.conditions[0].UnlockWay(true);
                                            }

                                            reset_agv.Key.conditions.Clear();

                                            if (reset_agv.Key.agvRunner != null)
                                            {
                                                if (reset_agv.Key.req != null)
                                                {
                                                    AgvDatabase.OrderComplete(reset_agv.Key, AgvDatabase.OrderCompleteReason.RESET);
                                                    reset_agv.Key.req = null;
                                                    reset_agv.Key.request_moving = false;

                                                    Sleep(1000);
                                                }
                                                reset_agv.Key.agvRunner.Stop();
                                                reset_agv.Key.agvRunner = null;
                                            }

                                            AgvRunnerStart(reset_agv.Key);

                                            req.result = "OK";
                                            OrderSend(req);
                                        }
                                        else
                                        {
                                            req.result = "NG";
                                            OrderSend(req);
                                        }
                                    }
                                }

                                requested.Add(req);
                            }
                            else
                            {
                                if (assign_request(req))
                                {
                                    requested.Add(req);
                                    break;
                                }
                            }
                        }

                        foreach (var v in requested) requests.Remove(v);
                    }
                }

                #endregion

                bool state_changed_disp = false;
                foreach (var kv in runners)
                {
                    kv.Value._DoControl(message);

                    if (kv.Value.state_changed_disp)
                    {
                        state_changed_disp = true;
                        kv.Value.state_changed_disp = false;
                    }
                }

                foreach (var v in controller.all_autorator)
                {
                    if (v.autorator_info != null && v.autorator_info.repaint)
                    {
                        state_changed_disp = true;
                        v.autorator_info.repaint = false;
                    }
                }

                #region 表示更新イベント発行

                if (state_changed_disp)
                {
                    //if (controller.EventAgvStateRefresh != null) controller.EventAgvStateRefresh(null);
                    if (controller.EventPaint != null) controller.EventPaint();
                }

                #endregion

                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log("処理時間 DoControl:" + sw.ElapsedMilliseconds.ToString() + "ms");
                }

                return base.DoControl(message);
            }

            #endregion

            #region 処理開始・停止

            public void Start(int movemode, int interval)
            {
                StartControl(interval);
            }

            public void Start(int interval)
            {
                Start(moveModes[0].mode, interval);
            }

            public void Stop()
            {
                foreach (var v in controller.agvs)
                {
                    v.moveMode = moveModes[0].mode;

                    if (v.agvRunner != null && v.agvRunner.communicator != null)
                    {
                        v.agvRunner.communicator.SetOrder(new AgvCommunicator.Order[] { v.RouteCancelOrder() });
                    }

                    if (v.req != null && v.agvRunner != null)
                    {
                        AgvDatabase.OrderComplete(v, AgvDatabase.OrderCompleteReason.RESET);
                        v.req = null;
                        v.request_moving = false;
                    }
                }

                Sleep(1000);

                foreach (var v in controller.agvs)
                {
                    if (v.rack != null)
                    {
                        var qr = v.floor.mapeditor.HitTest(v.rack.defaultLocation);
                        if (qr != null && qr.rack != null)
                        {
                            qr.rack = v.rack;
                            v.rack = null;
                        }
                        else
                        {
                            v.RackDown();
                        }
                    }
                }

                foreach (var v in controller.AllAutoratorQR)
                {
                    if (v.autorator_info != null)
                    {
                        v.autorator_info.StopControl();
                    }
                }

                foreach (var kv in runners)
                {
                    kv.Value.Stop();
                }

                StopControl();
            }

            #endregion

            #region 動作モード

            public void MoveMode(int movemode)
            {
                if (moveModes.ContainsKey(movemode))
                {
                    foreach (var agv in controller.agvs)
                    {
                        agv.moveMode = movemode;
                    }
                }
            }

            #region 各AGVが保持しているconditionsを入れ替え（未使用）

            //public void RotateConditions()
            //{
            //    Dictionary<PointF, Tuple<FloorAGV, Dictionary<enMoveMode, RouteConditionList>>> con_first = new Dictionary<PointF, Tuple<FloorAGV, Dictionary<enMoveMode, RouteConditionList>>>();

            //    foreach (var floor in map.map.Values)
            //    {
            //        foreach (var v in map.agvs)
            //        {
            //            if (v.floor != floor) continue;

            //            if (0 < v.mode_conditions[v.moveMode].Count)
            //            {
            //                con_first[v.mode_conditions[v.moveMode][0].Location] = new Tuple<FloorAGV, Dictionary<enMoveMode, RouteConditionList>>(v, v.mode_conditions);
            //            }
            //        }
            //    }

            //    bool allmatch = true;
            //    foreach (var v in conditioner.map.agvs)
            //    {
            //        if (v.floor != conditioner.floor) continue;

            //        if (con_first.ContainsKey(v.Location))
            //        {
            //            if (con_first[v.Location].Item1 == v)
            //            {
            //                allmatch = false;
            //                break;
            //            }
            //        }
            //        else
            //        {
            //            allmatch = false;
            //            break;
            //        }
            //    }

            //    if (allmatch)
            //    {
            //        Dictionary<FloorAGV, FloorAGV> switch_agv = new Dictionary<FloorAGV, FloorAGV>();

            //        foreach (var v in conditioner.map.agvs)
            //        {
            //            if (v.floor != conditioner.floor) continue;

            //            if (con_first.ContainsKey(v.Location))
            //            {
            //                v.mode_conditions = con_first[v.Location].Item2;
            //                switch_agv[con_first[v.Location].Item1] = v;
            //            }
            //        }

            //        foreach (var v in conditioner.map.agvs)
            //        {
            //            if (v.floor != conditioner.floor) continue;

            //            foreach (var mc in v.mode_conditions)
            //            {
            //                foreach (var c in mc.Value)
            //                {
            //                    c.parent = v;

            //                    Dictionary<FloorAGV, PointF> other_agv_locations = new Dictionary<FloorAGV, PointF>();
            //                    foreach (var oac in c.other_agv_locations)
            //                    {
            //                        if (switch_agv.ContainsKey(oac.Key))
            //                        {
            //                            other_agv_locations[switch_agv[oac.Key]] = oac.Value;
            //                        }
            //                        else
            //                        {
            //                            other_agv_locations[oac.Key] = oac.Value;
            //                        }
            //                    }

            //                    c.other_agv_locations.Clear();

            //                    foreach (var oac in other_agv_locations)
            //                    {
            //                        c.other_agv_locations[oac.Key] = oac.Value;
            //                    }
            //                }
            //            }
            //        }
            //    }
            //}

            #endregion

            #endregion

            #region 要求処理

            private bool Req_StationComplete(AgvController.AgvOrderCommunicator.RequestSTComplete req)
            {
                var st_qrs = controller.AllQR.Where(e => e.station_id == req.station && (e.on_agv != null || e.rack != null));
                if (0 < st_qrs.Count())
                {
                    FloorQR st_qr = st_qrs.First();
                    st_qr.Trigger(st_qr.floor.code, "");

                    return true;
                }

                return false;
            }

            public void AddRequest(AgvController.AgvOrderCommunicator.RequestBase req)
            {
                lock (requests)
                {
                    requests.Add(req);
                }

                Log("R[" + req.ToString() + "]");
            }

            private void Order_com_ReceiveEvent(AgvController.AgvOrderCommunicator sender, string cmd, AgvController.AgvOrderCommunicator.RequestBase req)
            {
                //作業要求を追加する
                AddRequest(req);
            }

            private bool assign_request(AgvOrderCommunicator.RequestBase req)
            {
                FloorAGV target_agv = null;

                {
                    var r = req as AgvOrderCommunicator.RequestDelivery;
                    if (r != null && r.cmd == AgvOrderCommunicator.enREQ.QRS.ToString())
                    {
                        if (r.result == "RQ")
                        {
                            FloorQR stationqr = controller.GetStationQR(r.station);
                            if (stationqr != null)
                            {
                                FloorQR rackqr = controller.GetRackQR(r.rack);
                                if (rackqr == null)
                                {
                                    //AGVが持っている
                                    var rack_agvs = controller.agvs.Where(e => e.rack != null && e.rack.rack_no == r.rack);
                                    if (0 < rack_agvs.Count())
                                    {
                                        target_agv = rack_agvs.First();
                                    }
                                    else
                                    {
                                        //棚が無い
                                        //...
                                    }
                                }
                                else
                                {
                                    //待機中のAGV列挙
                                    var normalagv = controller.agvs.Where(e => e.rack == null
                                                                            && !e.crash_wait
                                                                            && !e.request_moving
                                                                            && e.agvRunner.requests.Count == 0
                                                                            && e.agvRunner.can_order
                                                                            && (e.agvRunner.agv_request || e.agvRunner.communicator.GetState.sta_charge)
                                                                            && (!e.agvRunner.communicator.GetState.sta_charge || CHARGE_ABORT_LEVEL <= e.agvRunner.communicator.GetState.bat)
                                                                            && BATTERY_NOP < e.agvRunner.communicator.GetState.bat);

                                    SortedDictionary<int, FloorAGV> costed_agv = new SortedDictionary<int, FloorAGV>();

                                    foreach (var v in normalagv)
                                    {

#if ROUTECOMMANDER
                                        AgvRouteCommanderFA runner = v.agvRunner as AgvRouteCommanderFA;
#else
                                        AgvAutoCommander runner = v.agvRunner as AgvAutoCommander;
#endif
                                        if (runner != null)
                                        {
                                            List<FloorQR> except_qrs = runner.GetExceptQR(rackqr);
                                            int cost = controller.routeManager.CalcRouteConditionCost(controller.routeManager.GetMoveConditions(v, rackqr, "", except_qrs));

                                            if (!costed_agv.ContainsKey(cost))
                                            {
                                                costed_agv[cost] = v;
                                            }
                                        }
                                    }

                                    if (0 < costed_agv.Count)
                                    {
                                        target_agv = costed_agv.First().Value;
                                    }
                                    else
                                    {
                                        //作業可能なAGVがいない
                                        //...
                                    }
                                }
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }

                if (target_agv == null)
                {
                    var r = req as AgvOrderCommunicator.RequestMove;
                    if (r != null && r.cmd == AgvOrderCommunicator.enREQ.MOV.ToString())
                    {
                        if (r.result == "RQ")
                        {
                            if (!r.ack)
                            {
                                //ACK返送
                                order_com.Send(r);
                                Log("S[" + r.ToString() + "]");

                                r.ack = true;
                            }

                            string agvno = r.agv.Trim();
                            string rackno = r.rack_no.Trim();

                            r.rack_face = r.rack_face == "0" ? "" : r.rack_face.Trim();

                            if (agvno == "")
                            {
                                FloorQR stationqr = controller.GetStationQR(r.station.Trim());
                                if (stationqr != null)
                                {
                                    var normalagv = controller.agvs.Where(e => e.agvRunner != null
                                                                            && !e.crash_wait
                                                                            && !e.request_moving
                                                                            && !e.working
                                                                            && e.agvRunner.requests.Count == 0
                                                                            && e.agvRunner.can_order
                                                                            && (e.agvRunner.agv_request || e.agvRunner.communicator.GetState.sta_charge)
                                                                            && (!e.agvRunner.communicator.GetState.sta_charge || CHARGE_ABORT_LEVEL <= e.agvRunner.communicator.GetState.bat)
                                                                            && BATTERY_NOP < e.agvRunner.communicator.GetState.bat);

                                    SortedDictionary<int, FloorAGV> costed_agv = new SortedDictionary<int, FloorAGV>();

                                    foreach (var v in normalagv)
                                    {
                                        if (r.rack_action == "1" && v.rack != null) continue;
                                        if (r.rack_action == "2" && v.rack == null) continue;

#if ROUTECOMMANDER
                                        AgvRouteCommanderFA runner = v.agvRunner as AgvRouteCommanderFA;
#else
                                        AgvAutoCommander runner = v.agvRunner as AgvAutoCommander;
#endif

                                        if (runner != null)
                                        {
                                            List<FloorQR> except_qrs = runner.GetExceptQR(stationqr);

                                            int cost = controller.routeManager.CalcRouteConditionCost(controller.routeManager.GetMoveConditions_ex(v, stationqr, r.rack_face, except_qrs));

                                            if (!costed_agv.ContainsKey(cost))
                                            {
                                                costed_agv[cost] = v;
                                            }
                                        }
                                    }

                                    if (0 < costed_agv.Count)
                                    {
                                        target_agv = costed_agv.First().Value;
                                    }
                                    else
                                    {
                                        //作業可能なAGVがいない
                                        //...
                                    }
                                }
                            }
                            else
                            {
                                var find_agv = controller.agvs.Where(e => e.id.Trim() == agvno);
                                if (0 < find_agv.Count())
                                {
                                    target_agv = find_agv.First();
                                }
                                else
                                {
                                    //作業可能なAGVがいない
                                    //...
                                }
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }

                if (target_agv != null && runners.ContainsKey(target_agv))
                {
                    lock (runners[target_agv].requests)
                    {
                        runners[target_agv].requests.Add(req);
                    }

                    runners[target_agv].Log("R[" + req.ToString() + "]");
                    return true;
                }

                return false;
            }

            public bool OrderSend(AgvOrderCommunicator.RequestBase res)
            {
                Log("S[" + res.ToString() + "]");

                if (order_com != null) return order_com.Send(res);

                return false;
            }

            #endregion
        }
    }
}
