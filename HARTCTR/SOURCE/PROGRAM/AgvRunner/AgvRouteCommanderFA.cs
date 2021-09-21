using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics.Eventing.Reader;
using System.Threading;
using System.Windows.Forms;

using BelicsClass.Network;
using BelicsClass.Common;
using BelicsClass.File;
using BelicsClass.ObjectSync;

using AgvController;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        public class AgvRouteCommanderFA : AgvRunner
        {
            const int TIMEOUT = 5000;

            protected AgvControlManager controller = null;
            protected Queue<AgvCommunicator.State> queState = new Queue<AgvCommunicator.State>();
            protected List<AgvCommunicator.Order> next_orders = new List<AgvCommunicator.Order>();
            protected AgvCommunicator.State sta = new AgvCommunicator.State();
            protected AgvCommunicator.Order ord = new AgvCommunicator.Order();

            protected Dictionary<FloorQR, Rack> rack_save = new Dictionary<FloorQR, Rack>();
            protected int current_deg = -999;
            protected bool delailed = false;

            public enum enAgvMoveKind
            {
                AGV_STANBY,
                AGV_WAIT,
                AGV_CHARGE,
                RACK_ESCAPE,
                AGV_ESCAPE,
                RACK_REQUEST,
                RACK_RETURN,
            }

            int lock_distance = 375;

            AgvTracker tracker = null;

            BL_Stopwatch swStartWait = new BL_Stopwatch();
            BL_Stopwatch reportbattery_timer = new BL_Stopwatch();
            int bat_pre = -1;
            bool sta_charge = true;
            int error_code_pre = -1;
            string map_pre = "";
            int x_pre = -1;
            int y_pre = -1;
            BL_Stopwatch swReportAgv = new BL_Stopwatch();
            BL_Stopwatch swDatabaseUpdateAgv = new BL_Stopwatch();

            float rack_deg_pre = 9999;

            string _description = "";
            string description 
            {
                get { return _description; }
                set
                {
                    if (_description.Trim() != value.Trim())
                    {
                        _description = value;
                        state_changed_disp = true;
                    }
                }
            }

            public override string state_string => "[" + step + "][" + description + "]";


            bool routed = false;

            public AgvRouteCommanderFA(AgvRunManager manager, FloorAGV agv, AgvConnector connector)
                : base(manager, agv)
            {
                this.connector = connector;
                controller = Program.controller;

                if (manager.agvid_list.ContainsKey(agv.id))
                {
                    lock_distance = manager.agvid_list[agv.id].stop_distance;
                }
            }

            #region 制御開始・終了

            public override void Start()
            {
                //mem = new AgvMemory(connector.ip + "_" + connector.remote_host.ToString());

                base.Start();
                agv_request = false;

                Log(step + ",THREAD START");

                ClearCondition();

                if (communicator != null) Stop();
                communicator = new AgvController.AgvCommunicator(agv.id, connector.ip, connector.remote_client, connector.remote_host, connector.recv, connector.send);
                communicator.ReceiveEvent += Communicator_ReceiveEvent;
                communicator.StartControl(5);

                swStartWait.Restart();
                agv.swStanbySeconds.Restart();


                if (db_man != null) db_man.StopControl();
                db_man = new AgvDatabaseManager(agv);
                db_man.Event_AgvOrder += Db_man_Event_AgvOrder;
                db_man.Event_AgvOrderDown += Db_man_Event_AgvOrderDown;
                db_man.Event_AgvDatabaseError += Db_man_Event_AgvDatabaseError;

                db_man.StartControl(100);
            }

            private void Db_man_Event_AgvDatabaseError(AgvDatabaseManager sender, string message)
            {
                if (controller.EventAgvDatabaseError != null)
                {
                    controller.EventAgvDatabaseError(this, message);
                }
            }

            private void Db_man_Event_AgvOrderDown(AgvDatabaseManager sender, AgvDatabase.AGV_SYS_ORDER sys_order)
            {
                if (sys_order.ORDER_TYPE == AgvDatabase.OrderType.RESET)
                {
                    AgvOrderCommunicator.RequestReset req = new AgvOrderCommunicator.RequestReset();
                    req.agv = sys_order.AGV_ID.Trim();
                    manager.AddRequest(req);
                }
                else if (sys_order.ORDER_TYPE == AgvDatabase.OrderType.REGISTER_DOWN)
                {

                }
                else if (sys_order.ORDER_TYPE == AgvDatabase.OrderType.CANCEL_DOWN)
                {

                }
            }

            private void Db_man_Event_AgvOrder(AgvDatabaseManager sender, AgvOrderCommunicator.RequestBase req)
            {
                manager.AddRequest(req);
            }

            public override void Stop()
            {
                Log(step + ",THREAD STOP");

                agv.in_autorator = null;
                agv.out_autorator = null;

                base.Stop();

                if (db_man != null)
                {
                    db_man.StopControl();
                    db_man.Event_AgvOrder -= Db_man_Event_AgvOrder;
                    db_man.Event_AgvOrderDown -= Db_man_Event_AgvOrderDown;
                    db_man.Event_AgvDatabaseError -= Db_man_Event_AgvDatabaseError;
                    db_man = null;
                }

                if (communicator != null)
                {
                    communicator.ReceiveEvent -= Communicator_ReceiveEvent;
                    communicator.StopControl();
                    communicator = null;
                }

                //mem.Dispose();
            }

            public override void _DoControl(object message)
            {
                BL_Stopwatch sw = new BL_Stopwatch();
                sw.Restart();

                lock (controller)
                {
                    state_received = false;
                    state_changed = false;

                    if (0 < queState.Count)
                    {
                        state_received = true;

                        state_control();

                        //上位報告
                        if (!reportbattery_timer.IsRunning || 3000 <= reportbattery_timer.ElapsedMilliseconds ||
                            bat_pre != (int)sta.bat || sta_charge != sta.sta_charge || error_code_pre != sta.error_code ||
                            map_pre != sta.map || x_pre != sta.x || y_pre != sta.y)
                        {
                            reportbattery_timer.Restart();

                            bat_pre = (int)sta.bat;
                            sta_charge = sta.sta_charge;
                            error_code_pre = (int)sta.error_code;
                            map_pre = sta.map;
                            x_pre = sta.x;
                            y_pre = sta.y;

                            manager.OrderSend(new AgvOrderCommunicator.ReportBattery(agv.id, (int)sta.bat, sta.sta_charge, (int)sta.error_code, sta.map, sta.x, sta.y));
                        }
                    }

                    //上位報告(DB)
                    if (db_man != null && command_received)
                    {
                        if (!swDatabaseUpdateAgv.IsRunning || 1000 < swDatabaseUpdateAgv.ElapsedMilliseconds)
                        {
                            swDatabaseUpdateAgv.Restart();

                            db_man.SetState(sta);
                        }
                    }

                    if (swStartWait.IsRunning && swStartWait.ElapsedMilliseconds < 3000)
                    {
                        description = "起動待機";

                        //起動時3秒待機
                        return;
                    }

                    swStartWait.Stop();

                    if (agv.on_qr != null)
                    {
                        #region 棚情報更新

                        List<FloorQR> placed_rack = new List<FloorQR>();
                        foreach (var v in rack_save)
                        {
                            if (v.Key.on_agv == null)
                            {
                                v.Key.rack = v.Value;
                                placed_rack.Add(v.Key);
                            }
                        }

                        foreach (var v in placed_rack)
                        {
                            rack_save.Remove(v);
                        }

                        #endregion

                        //常に、棚搬送要求ではなく移動要求として動作させる
                        agv.prev_agvreq = true;

                        //充電中はSTATE状態とする（AGV模擬用）
                        if (sta.sta_charge) agv_request = false;

                        agv_control();
                    }

                    order_control();

                    if (agv.communicating && TIMEOUT < swReportAgv.ElapsedMilliseconds)
                    {
                        agv.communicating = false;
                    }
                }

                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log(step + ",処理時間 _DoControl:" + sw.ElapsedMilliseconds.ToString() + "ms");
                }
            }

            #endregion

            private void Communicator_ReceiveEvent(AgvCommunicator sender, AgvCommunicator.ReceiveEventArgs e)
            {
                agv.communicating = true;
                swReportAgv.Restart();

                lock (queState)
                {
                    queState.Enqueue(e.state);
                }


                //即座に応答を返す
                if (e.state.cmd == (ushort)AgvCommunicator.State.CMD.REQUEST || e.state.cmd == (ushort)AgvCommunicator.State.CMD.STATE)
                {
                    communicator.SetOrder(new AgvCommunicator.Order[] { agv.Response(e.state) });
                }


                //if (tracker == null)
                //{
                //    tracker = new AgvTracker(agv, e.state);
                //}
                //else
                //{
                //    tracker.ChangeState(e.state);
                //}
            }

            public void ClearCondition()
            {
                //lock (manager)
                {
                    if (agv.req == null) agv.Unreserve(true);

                    agv.Unlock();

                    if (0 < agv.conditions.Count && agv.on_qr == agv.conditions.Last().cur_qr)
                    {
                        agv.conditions.Clear();
                    }

                    RouteConditionList remove_cons = new RouteConditionList();
                    if (agv.current_route != null)
                    {
                        foreach (var v in agv.current_route)
                        {
                            if (agv.conditions.Where(e => e.cur_qr == v.cur_qr).Count() == 0) remove_cons.Add(v);
                        }
                        foreach (var v in remove_cons) agv.current_route.Remove(v);
                    }

                    cur_con = null;
                }
            }

            #region 上位報告

            /// <summary>
            /// 棚返却報告
            /// </summary>
            /// <param name="req"></param>
            /// <param name="result"></param>
            public void SendRackReturn(AgvOrderCommunicator.RequestDelivery req, AgvOrderCommunicator.enRESULT result)
            {
                //作業要求のある棚返却完了通知
                int seqno = 0; int.TryParse(req.seqno, out seqno);

                AgvOrderCommunicator.RequestSTComplete res = new AgvOrderCommunicator.RequestSTComplete(
                                                                AgvOrderCommunicator.enREQ.ARR,
                                                                seqno,
                                                                result, "",
                                                                req.rack,
                                                                agv.on_qr.floor.code + agv.on_qr.x.ToString("00000") + agv.on_qr.y.ToString("00000"));
                manager.OrderSend(res);
            }

            /// <summary>
            /// 棚到着報告
            /// </summary>
            /// <param name="req"></param>
            public void SendRackArrive(AgvOrderCommunicator.RequestDelivery req)
            {
                int seqno = 0; int.TryParse(req.seqno, out seqno);

                AgvOrderCommunicator.RequestDelivery res = new AgvOrderCommunicator.RequestDelivery(
                                                                AgvOrderCommunicator.enREQ.ASA,
                                                                seqno,
                                                                AgvOrderCommunicator.enRESULT.OK,
                                                                req.station,
                                                                req.rack,
                                                                req.rackface);
                manager.OrderSend(res);
            }

            #endregion

            #region AGV状態受信・応答指示送信

            bool command_received = false;

            /// <summary>
            /// AGVからの報告を内部変数へ展開
            /// </summary>
            /// <returns></returns>
            protected bool state_control()
            {
                BL_Stopwatch sw = new BL_Stopwatch();
                sw.Restart();


                bool received = false;
                //bool is_changed = false;

                state_changed = false;

                lock (queState)
                {
                    while (0 < queState.Count)
                    {
                        if (agv.id == "02")
                        {
                        }

                        command_received = true;
                        AgvCommunicator.State sta_new = queState.Dequeue();

                        #region 状態報告取得・変化検出

                        //@@@模擬のため
                        if (sta_new.sta_charge) sta_new.cmd = (ushort)AgvCommunicator.State.CMD.STATE;

                        if (sta.IsChanged(sta_new))
                        {
                            state_changed = true;
                            state_changed_disp = true;

                            if (tracker == null) tracker = new AgvTracker(agv, sta_new);
                            tracker.ChangeState(sta_new);
                        }

                        sta.SetBytes(sta_new.GetBytes());
                        sta.last_time = sta_new.last_time;

                        received = true;

                        if (sta.x == 0 && sta.y == 0)
                        {
                            #region AGVが座標を見失った場合、周囲をロック

                            if (!delailed)
                            {
                                delailed = true;
                                ClearCondition();

                                //直前までいた位置から周囲３ピッチ分(375cm)を排他して、通行不可とする
                                //...

                                //0度90度180度270度で隣り合うピッチ150cm未満のQRを列挙

                                agv.degree = sta.deg;
                                int deg = (agv.degree + 360) % 360;

                                var list = floor.mapeditor.list.Where(e => agv.on_qr.Distance(e) < 375);
                                foreach (var v in list)
                                {
                                    v.Lock(agv);

                                    foreach (var vv in agv.locked_qr_list)
                                    {
                                        vv.Lock(agv);

                                        //lock (vv.lock_agv)
                                        //{
                                        //    if (vv.lock_agv.Contains(agv)) vv.lock_agv.Remove(agv);
                                        //    vv.lock_agv.Insert(0, agv);
                                        //}
                                    }

                                    //int qr_deg = (int)(agv.Degree(v.Location) + 360) % 360;

                                    //if (deg - 45 < 0)
                                    //{
                                    //    if (qr_deg <= deg + 360 - 45 && qr_deg <= deg + 45)
                                    //    {
                                    //        v.lock_agv = agv;
                                    //    }
                                    //}
                                    //else if (360 <= deg + 45)
                                    //{


                                    //}
                                    //else if (deg - 45 <= qr_deg && qr_deg <= deg + 45)
                                    //{
                                    //    v.lock_agv = agv;
                                    //}
                                }
                            }

                            #endregion

                            //next_orders.Add(agv.Response(sta));
                            continue;
                        }
                        else
                        {
                            if (delailed)
                            {
                                delailed = false;
                                agv.Unlock();
                            }
                        }

                        sta._map = sta.map;
                        sta._x = sta.x;
                        sta._y = sta.y;

                        #endregion

                        #region オートレーター内にいる場合の座標変換

                        if (sta.map.Trim() == "" || agv.on_qr.autorator_info != null)
                        {
                            if (agv.on_qr.autorator_info != null && 0 < agv.on_qr.autorator_info.autorator_qrs.Where(e => e.Location == sta.Location).Count())
                            {
                                int floorno = 0; int.TryParse(agv.on_qr.autorator_info.Status.stop_floor, out floorno);

                                FloorQR qr = agv.on_qr.autorator_info.LogicQR(floorno);
                                if (qr != null)
                                {
                                    if (agv.on_qr != qr)
                                    {
                                        agv.on_qr.Unreserve(agv);
                                    }

                                    sta.map = qr.floor.code;
                                    sta.x = qr.x;
                                    sta.y = qr.y;
                                }
                            }
                            else
                            {
                                foreach (var v in agv.conditions)
                                {
                                    if (v.Location == sta_new.Location)
                                    {
                                        sta.map = v.cur_qr.floor.code;
                                        sta.x = v.cur_qr.x;
                                        sta.y = v.cur_qr.y;

                                        break;
                                    }
                                }
                            }
                            //else if (cur_con != null && cur_con.next_condition != null && cur_con.next_condition.next_condition != null
                            //    && cur_con.next_condition.next_condition.Location == sta.Location)
                            //{
                            //    sta.map = cur_con.next_condition.cur_qr.floor.code;
                            //    sta.x = cur_con.next_condition.cur_qr.x;
                            //    sta.y = cur_con.next_condition.cur_qr.y;
                            //}
                            //else if (cur_con != null && cur_con.next_condition != null
                            //    && cur_con.next_condition.Location == sta.Location)
                            //{
                            //    sta.map = cur_con.next_condition.cur_qr.floor.code;
                            //    sta.x = cur_con.next_condition.cur_qr.x;
                            //    sta.y = cur_con.next_condition.cur_qr.y;
                            //}
                            //else if (sta.map.Trim() == "")
                            //{
                            //    if (cur_con != null && cur_con.next_condition != null)
                            //    {
                            //        sta.map = cur_con.next_condition.cur_qr.floor.code;
                            //    }
                            //    else
                            //    {
                            //        sta.map = agv.on_qr.floor.code;
                            //    }
                            //}
                        }

                        #endregion

                        #region AGV状態遷移

                        if (sta.cmd == (ushort)AgvCommunicator.State.CMD.STATE ||
                            sta.cmd == (ushort)AgvCommunicator.State.CMD.REQUEST)
                        {
                            #region 座標反映

                            string floor_pre = "";
                            FloorQR prev_qr = null;

                            if (Location != sta.Location || agv.floor.code != sta.map)
                            {
                                if (agv.floor.code != sta.map)
                                {
                                    floor_pre = agv.floor.code;
                                }

                                Log(step + ",座標変化[" + sta.map + sta.Location.ToString() + "] step[" + step.ToString() + "]");


                                //@@@走行距離を加算
                                //@@@走行ポイント数を加算

                                double distance = agv.Distance(Location, sta.Location);
                                if (floor_pre != "") distance = 0;

                                prev_qr = agv.on_qr;

                                SetLocation(sta.map, sta.Location, sta.sta_rack ? sta.rack_deg : 9999);

                                ////過去にいた場所は予約解除
                                //////チェックポイントから移動したらチェックポイントは予約解除
                                ////if (prev_qr.escape_to != null)
                                //{
                                //    prev_qr.Unreserve(agv);
                                //}

                                if (agv.in_autorator == null && agv.out_autorator != null)
                                {
                                    if (140 <= agv.on_qr.Distance(agv.out_autorator))
                                    {
                                        description = "オートレーター搬出完了";

                                        #region オートレーターから搬出完了

                                        if (agv.out_autorator.autorator_info.CompleteExit(agv))
                                        {
                                            agv.out_autorator = null;
                                        }

                                        #endregion
                                    }
                                }
                                else if (prev_qr.autorator_id == "" && agv.on_qr.autorator_id != "")
                                {
                                    if (agv.on_qr.autorator_info != null)
                                    {
                                        description = "オートレーター搬入完了";

                                        #region オートレーターに搬入完了

                                        if (agv.on_qr.autorator_info.CompleteEntry(agv))
                                        {
                                            agv.in_autorator = null;
                                        }

                                        #endregion
                                    }
                                }
                            }

                            #endregion

                            #region 棚状態の反映

                            //if (sta.sta_runmode)
                            {
                                if (sta.sta_rack)
                                {
                                    if (agv.rack == null)
                                    {
                                        //@@@棚上昇動作回数を加算

                                        Log(step + ",棚上昇[" + sta.rack_no + "]");

                                        if (agv.req != null)
                                        {
                                            if (agv.on_qr.rack != null)
                                            {
                                                rack_save[agv.on_qr] = agv.on_qr.rack;
                                                agv.on_qr.rack = null;
                                            }
                                            else
                                            {
                                                rack_save.Remove(agv.on_qr);
                                            }
                                        }

                                        var rack_qr = controller.GetRackQR(sta.rack_no.ToString());

                                        if (rack_qr == null)
                                        {
                                            if (sta.rack_no != 0 /*&& agv.on_qr.rack == null*/)
                                            {
                                                Log(step + ",棚存在しない→生成");

                                                //Rack rack = RackMaster.Instance.LoadRack(sta.rack_no.ToString());
                                                //Rack rack = RackMaster.Instance.LoadRack(sta.racktype + sta.rack_no.ToString());
                                                Rack rack = RackMaster.Instance.LoadRack(sta.racktype);
                                                rack.rack_no = sta.racktype + sta.rack_no.ToString();
                                                agv.on_qr.rack = rack;
                                            }
                                        }
                                        else
                                        {
                                            if (rack_qr != agv.on_qr)
                                            {
                                                agv.on_qr.rack = rack_qr.rack;
                                                rack_qr.rack = null;
                                            }
                                        }

                                        agv.RackUp();

                                        if (agv.rack != null)
                                        {
                                            agv.rack.loading_agv = agv;
                                            agv.rack.loadreserve_agv = null;
                                        }
                                    }

                                    if (agv.rack != null)
                                    {
                                        if (sta.rack_deg != 9999)
                                        {
                                            if (agv.rack.degree != sta.rack_deg)
                                            {
                                                Log(step + ",棚角度変化[" + sta.rack_deg + "]");

                                                if (prev_qr != null)
                                                {
                                                    //棚角度変化によって周囲を予約しなおす

                                                    
                                                }
                                            }
                                            
                                            agv.rack.degree = sta.rack_deg;
                                        }
                                    }
                                }
                                else
                                {
                                    if (agv.rack != null)
                                    {
                                        //@@@棚下降動作回数を加算

                                        agv.rack.loading_agv = null;
                                        agv.rack.loadreserve_agv = null;
                                        agv.floor.mapeditor.redraw_rack = true;

                                        if (agv.rack.req_return != null)
                                        {
                                            Log(step + ",棚返却完了[" + agv.rack.req_return.ToString() + "]");

                                            if (agv.rack.req_return.station != "")
                                            {
                                                SendRackReturn(agv.rack.req_return, AgvOrderCommunicator.enRESULT.OK);
                                            }

                                            agv.rack.obstruct_route = null;
                                            agv.rack.req_return = null;
                                        }

                                        //if (sta.rack_deg != 9999)
                                        //{
                                        //    if (agv.rack.degree != sta.rack_deg)
                                        //    {
                                        //        //Log(step + ",棚角度変化[" + sta.rack_deg + "]");

                                        //        agv.rack.degree = sta.rack_deg;
                                        //    }
                                        //}

                                        Log(step + ",棚下降[" + agv.rack.ToString() + "]");

                                        agv.RackDown();

                                        if (agv.req != null && agv.req.rack_action == "2")
                                        {
                                            //棚を置いたら棚を消す
                                            agv.on_qr.rack = null;
                                            agv.floor.mapeditor.redraw_rack = true;
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region 角度変化検出

                            if (Degree != sta.deg)
                            {
                                //@@@AGV旋回角度を加算
                                int degree = Math.Abs(Degree - sta.deg);

                                Log(step + ",AGV角度変化[" + sta.deg + "]");

                                Degree = sta.deg;
                            }

                            #endregion

                            if (mapeditor.Exist(sta.Location) != null)
                            {
                                if (0 < agv.conditions.Count)
                                {
                                    #region ルートの進捗更新

                                    List<RouteCondition> unlock_cons = new List<RouteCondition>();

                                    for (int i = 0; i < agv.conditions.Count; i++)
                                    {
                                        if (agv.conditions[i].FloorCode == floor_pre)
                                        {
                                            agv.conditions.RemoveAt(i);
                                            i--;
                                            continue;
                                        }

                                        if (cur_con == null) break;
                                        RouteCondition con = agv.conditions[i];

                                        if (con.Location == Location)
                                        {
                                            #region AGVや棚の角度を厳密にチェックする

                                            //if (con.agv_turn_arrive)
                                            //{
                                            //    if (sta.deg != con.agv_turn_arrive_degree) break;
                                            //}
                                            //if (con.agv_turn_departure)
                                            //{
                                            //    if (sta.deg != con.agv_turn_departure_degree) break;
                                            //}
                                            if (con.rack_turn_arrive)
                                            {
                                                if (sta.rack_deg != con.rack_turn_arrive_degree) break;
                                            }
                                            if (con.rack_turn_departure)
                                            {
                                                if (sta.rack_deg != con.rack_turn_departure_degree) break;
                                            }
                                            if (con.rack_down_arrive || con.rack_down_departure || con.rack_down_departure_last)
                                            {
                                                if (sta.sta_rack) break;
                                            }
                                            if (con.rack_up_arrive || con.rack_up_departure)
                                            {
                                                if (!sta.sta_rack) break;
                                            }

                                            #endregion

                                            int next_deg = (int)agv.Degree(cur_con.Location, con.Location);
                                            if (current_deg != next_deg)
                                            {
                                                current_deg = next_deg;
                                                agv.straight_cost = cur_con.RemainStraightDistance();
                                            }

                                            if (cur_con.cur_qr != con.cur_qr)
                                            {
                                                var match_cp = agv.check_conditions.Where(e => e.check_condition.cur_qr == cur_con.cur_qr).FirstOrDefault();
                                                if (match_cp != null)
                                                {
                                                    agv.check_conditions.Remove(match_cp);
                                                }

                                                unlock_cons.Add(cur_con);

                                                foreach (var v in unlock_cons)
                                                {
                                                    v.UnlockWay(false);
                                                    //v.cur_qr.Unreserve(agv);

                                                    var rem = agv.conditions.Where(e => e.cur_qr == v.cur_qr).ToList();
                                                    foreach (var vv in rem) agv.conditions.Remove(v);
                                                    rem = agv.current_route.Where(e => e.cur_qr == v.cur_qr).ToList();
                                                    foreach (var vv in rem) agv.current_route.Remove(v);
                                                }
                                                unlock_cons.Clear();

                                                //通り過ぎたルートで不要な予約ポイントを解除
                                                {
                                                    SynchronizedList<FloorQR> qrlist = new SynchronizedList<FloorQR>();
                                                    foreach (var vv in agv.reserved_qr_list) qrlist.Add(vv);

                                                    foreach (var vv in qrlist)
                                                    {
                                                        if (!agv.routed_qr.Contains(vv))
                                                        {
                                                            vv.Unreserve(agv, true);
                                                        }
                                                    }
                                                }

                                                cur_con = con;

                                                if (cur_con != null)
                                                {
                                                    //衝突検知
                                                    RouteCondition crash_con = cur_con.LockWay();

                                                    if (crash_con != null)
                                                    {
                                                        Log(step + ",衝突検知1");
                                                        agv.crash_wait = true;

                                                        //next_orders.Add(agv.RouteCancelOrder());
                                                    }
                                                    else
                                                    {
                                                        agv.crash_wait = false;
                                                    }
                                                }

                                                //agv.conditions.Remove(con);

                                                //チェックポイントに着いたら、チェックポイントから外す
                                                var rc = agv.check_conditions.Where(e => e.check_condition.cur_qr == con.cur_qr).FirstOrDefault();
                                                if (rc != null) agv.check_conditions.Remove(rc);
                                            }

                                            break;
                                        }
                                        else
                                        {
                                            unlock_cons.Add(con);

                                            cur_con = con;
                                            //agv.conditions.Remove(con);
                                        }
                                    }

                                    if (agv.conditions.Count == 0 /*|| 0 < agv.conditions.Count && agv.on_qr == agv.conditions.Last().cur_qr*/)
                                    {
                                        ClearCondition();
                                    }

                                    #endregion
                                }
                            }
                            else
                            {
                                Log(step + ",座標データ不明," + sta.Location.ToString());
                            }

                            agv_stop = sta.sta_stop;

                            ////応答を返送
                            //next_orders.Add(agv.Response(sta));
                        }

                        #endregion

                        #region AGV状態更新

                        if (sta.cmd == (ushort)AgvCommunicator.State.CMD.STATE)
                        {
                            if (!agv_state)
                            {
                                agv.swStanbySeconds.Stop();
                                agv.swStanbySeconds.Reset();

                                Log(step + ",AGV→報告");
                            }

                            agv_request = false;
                            agv_state = true;
                        }
                        else if (sta.cmd == (ushort)AgvCommunicator.State.CMD.REQUEST)
                        {
                            if (!agv_request)
                            {
                                agv.swStanbySeconds.Restart();

                                Log(step + ",AGV→要求");

                                RouteCondition con = new RouteCondition();
                                ClearCondition();
                            }

                            agv_request = true;
                            agv_state = false;
                            agv_route_cancel = false;
                        }
                        else if (sta.cmd == (ushort)AgvCommunicator.State.CMD.RES_ORDER)
                        {
                            if (!agv_order_response)
                            {
                                Log(step + ",AGV→動作指示応答");
                            }
                            agv_order_response = true;
                        }

                        #endregion
                    }
                }

                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log(step + ",処理時間 state_control:" + sw.ElapsedMilliseconds.ToString() + "ms");
                }

                return received;
            }

            /// <summary>
            /// AGVへのルートキャンセル・ACK返送・動作指示を送信
            /// </summary>
            protected void order_control()
            {
                if (communicator == null) return;
                if (0 == next_orders.Count) return;

                BL_Stopwatch sw = new BL_Stopwatch();
                sw.Restart();


                List<AgvCommunicator.Order> list = new List<AgvCommunicator.Order>();

                if (0 < next_orders.Count)
                {
                    var rca = next_orders.Where(e => e.cmd == (ushort)AgvCommunicator.Order.CMD.ROUTE_CANCEL);
                    if (0 < rca.Count())
                    {
                        foreach (var v in rca)
                        {
                            communicator.SetOrder(new AgvCommunicator.Order[] { v });
                            list.Add(v);

                            if (v.IsChanged(ord))
                            {
                                //communicator.Log("STEP=" + step.ToString("000") + "," + v.ToString());
                                ord.SetBytes(v.GetBytes());
                            }
                        }

                        foreach (var v in list) next_orders.Remove(v);

                        agv_route_cancel = true;
                    }

                    //if (list.Count == 0)
                    //{
                    //    var res = next_orders.Where(e => e.cmd == (ushort)AgvCommunicator.Order.CMD.RESPONSE);
                    //    if (0 < res.Count())
                    //    {
                    //        foreach (var v in res)
                    //        {
                    //            communicator.SetOrder(new AgvCommunicator.Order[] { v });
                    //            list.Add(v);

                    //            if (v.IsChanged(ord))
                    //            {
                    //                //communicator.Log("STEP=" + step.ToString("000") + "," + v.ToString());
                    //                ord.SetBytes(v.GetBytes());
                    //            }
                    //        }

                    //        foreach (var v in list) next_orders.Remove(v);
                    //    }
                    //}

                    if (list.Count == 0)
                    {
                        var ord = next_orders.Where(e => e.cmd != (ushort)AgvCommunicator.Order.CMD.RESPONSE);
                        if (0 < ord.Count())
                        {
                            agv_order_response = false;

                            communicator.SetOrder(ord.ToArray());

                            foreach (var v in ord)
                            {
                                list.Add(v);

                                Log(step + "," + v.ToString());
                                //communicator.Log("STEP=" + step.ToString("000") + "," + v.ToString());

                                this.ord.SetBytes(v.GetBytes());
                            }

                            Log(step + ",動作指示完了[" + list.Count() + "件][" + sta.ToString() + "]");

                            foreach (var v in list) next_orders.Remove(v);
                        }
                    }
                }

                next_orders.Clear();

                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log(step + ",処理時間 order_control:" + sw.ElapsedMilliseconds.ToString() + "ms");
                }
            }

            #endregion

            #region 上位要求を取出

            /// <summary>
            /// 棚搬送要求の取出
            /// </summary>
            /// <returns></returns>
            protected bool RequestRack()
            {
                bool is_requested = false;

                AgvOrderCommunicator.RequestDelivery req = null;
                List<AgvOrderCommunicator.RequestDelivery> req_list = new List<AgvOrderCommunicator.RequestDelivery>();

                var rack_qrs = controller.AllRackQR.Where(e => e.rack != null && e.rack.loadreserve_agv == null && e.rack.req != null && e.rack.obstruct_route == null).ToList();

                foreach (var v in rack_qrs)
                {
                    req = v.rack.req as AgvOrderCommunicator.RequestDelivery;
                    if (req != null && req.cmd == AgvOrderCommunicator.enREQ.QRS.ToString())
                    {
                        req_list.Add(req);
                    }
                }

                lock (requests)
                {
                    foreach (var v in requests)
                    {
                        req = v as AgvOrderCommunicator.RequestDelivery;
                        if (req != null && req.cmd == AgvOrderCommunicator.enREQ.QRS.ToString())
                        {
                            req_list.Add(req);
                        }
                    }
                }

                foreach (var v in req_list)
                {
                    req = v as AgvOrderCommunicator.RequestDelivery;
                    if (req != null && req.cmd == AgvOrderCommunicator.enREQ.QRS.ToString())
                    {
                        FloorQR rackqr = null;
                        Rack rack = null;
                        if (agv.placed_rack != null && agv.placed_rack.rack_no == req.rack)
                        {
                            rackqr = agv.on_qr;
                            rack = agv.placed_rack;
                        }
                        if (rackqr == null)
                        {
                            rackqr = controller.GetRackQR(req.rack);
                            if (rackqr != null) rack = rackqr.rack;
                        }

                        FloorQR stationqr = controller.GetStationQR(req.station);
                        if (rackqr != null && stationqr != null)
                        {
                            //if (GetNearestAgv(rackqr) == agv)
                            {
                                agv.request_moving = true;
                                agv.prev_agvreq = false;

                                rack.obstruct_route = null;
                                rack.req = req;
                                rack.req_return = null;
                                rack.loadreserve_agv = agv;
                                is_requested = true;

                                agv.floor.mapeditor.redraw_rack = true;
                                state_changed = true;
                                break;
                            }
                        }
                    }
                }

                if (is_requested)
                {
                    lock (requests)
                    {
                        requests.Remove(req);
                    }

                    //@@@棚搬送要求受理
                    string seqno = req.seqno;
                    string rackno = req.rack;
                    string rackface = req.rackface;
                    string station = req.station;
                }

                return is_requested;
            }

            /// <summary>
            /// AGV移動要求の取出
            /// </summary>
            /// <returns></returns>
            protected bool RequestMove()
            {
                bool is_requested = false;

                AgvOrderCommunicator.RequestMove req = null;
                List<AgvOrderCommunicator.RequestMove> req_list = new List<AgvOrderCommunicator.RequestMove>();
                List<AgvOrderCommunicator.RequestMove> remove_list = new List<AgvOrderCommunicator.RequestMove>();

                lock (requests)
                {
                    foreach (var v in requests)
                    {
                        req = v as AgvOrderCommunicator.RequestMove;
                        if (req != null && req.cmd == AgvOrderCommunicator.enREQ.MOV.ToString())
                        {
                            if (req.station.Trim() == "" && req.rack_action.Trim() == "")
                            {
                                remove_list.Add(req);
                            }
                            else
                            {
                                req_list.Add(req);
                            }
                        }
                    }
                }

                foreach (var v in req_list)
                {
                    req = v as AgvOrderCommunicator.RequestMove;
                    if (req != null && req.cmd == AgvOrderCommunicator.enREQ.MOV.ToString())
                    {
                        if (req.rack_action == "C" || req.rack_action == "c")
                        {
                            agv.request_moving = true;
                            agv.prev_agvreq = true;
                            agv.req = req;
                            agv.req.agv = agv.id;
                            agv.working = false;

                            state_changed = true;
                            is_requested = true;
                            break;
                        }
                        else if (!sta.sta_charge)
                        {
                            //FloorQR stationqr = controller.GetStationQR(req.station.Trim());
                            //if (stationqr != null)
                            //{
                            agv.request_moving = true;
                            agv.prev_agvreq = true;
                            agv.req = req;
                            agv.req.agv = agv.id;
                            if (agv.req.working != "0") agv.working = true;
                            agv.escape_complete = null;
                            agv.escape_to = null;
                            agv.escape_from = null;

                            state_changed = true;
                            is_requested = true;
                            break;
                            //}
                        }
                    }
                }

                if (is_requested)
                {
                    lock (requests)
                    {
                        requests.Remove(req);
                    }

                    //@@@棚搬送要求受理
                    string seqno = req.seqno;
                    string station = req.station;
                    string rackno = req.rack_no;
                }

                if (0 < remove_list.Count)
                {
                    lock (requests)
                    {
                        foreach (var v in remove_list)
                        {
                            Log(step + ",ゴミデータ削除[" + v.ToString() + "]");
                            requests.Remove(v);
                        }
                    }
                }

                return is_requested;
            }

            #endregion

            private bool agv_request_pre = false;
            private bool agv_state_pre = false;
            //private FloorQR escape_agv_qr_pre = null;

            protected void agv_control()
            {
                int step_pre = step;

                agv_order_control();

                if (agv_request)
                {
                    if (!agv_request_pre)
                    {
                        agv.Unlock();
                        ClearCondition();
                        step = 0;
                    }

                    agv_request_control();
                }
                else if (agv_state)
                {
                    if (!agv_state_pre)
                    {
                        step = 0;
                    }

                    agv_state_control();
                }

                agv_request_pre = agv_request;
                agv_state_pre = agv_state;

                //if (step != step_pre)
                //{
                //    Log("STEP[" + step_pre + "]->[" + step + "]");
                //}
            }

            protected virtual void agv_order_control()
            {
                if (agv.req == null && (agv_request || (agv_state && sta.sta_charge)) && manager.BATTERY_LOW < sta.bat)
                {
                    RequestMove();
                }

                if (agv.req != null)
                {
                    if ((agv.req.rack_action == "C" && sta.sta_charge) ||
                        (agv.req.rack_action == "c" && !sta.sta_charge))
                    {
                        //不正
                        agv.req.result = "NG";
                        manager.OrderSend(agv.req);

                        if (sta.sta_charge)
                        {
                            AgvDatabase.OrderComplete(agv, AgvDatabase.OrderCompleteReason.ALREADY_CHARGE_STARTED);
                        }
                        else
                        {
                            AgvDatabase.OrderComplete(agv, AgvDatabase.OrderCompleteReason.ALREADY_CHARGE_STOPPED);
                        }

                        agv.request_moving = false;
                        agv.req = null;
                    }
                }
            }

            protected virtual void agv_request_control()
            {
                switch (step)
                {
                    case 0:

                        #region ルート生成
                        if (state_received)
                        {
                            RouteConditionList next_route = new RouteConditionList();

                            if (agv.req == null)
                            {
                                if (!agv.working && agv_request && agv.rack == null && requests.Count == 0)
                                {
                                    ////回避行動の判断
                                    next_route = get_escape_route();

                                    if (next_route.Count == 0)
                                    {
                                        agv.escape_complete = null;

                                        if (agv.rack == null)
                                        {
                                            if (manager.WAITTIME <= agv.swStanbySeconds.ElapsedMilliseconds / 1000 &&
                                                sta.bat <= manager.CHARGE_START_LEVEL && sta.bat < manager.BATTERY_FUL)
                                            {
                                                //充電開始動作

                                                description = "充電開始動作1";

                                                next_route = get_charge_route();
                                            }
                                        }
                                    }
                                    else
                                    {
                                    }
                                }
                            }
                            else
                            {
                                #region ルート生成

                                if (agv.rack == null && agv.req.rack_action == "C" && agv.req.station.Trim() == "")
                                {
                                    #region 充電開始動作指示

                                    if (!sta.sta_charge)
                                    {
                                        //充電開始動作

                                        description = "充電開始動作2";

                                        next_route = get_charge_route();
                                    }

                                    #endregion
                                }
                                else if (agv.req.station.Trim() != "")
                                {
                                    if (agv.on_qr.autorator_id.Trim() != "" && 0 < agv.conditions.Count)
                                    {
                                        //オートレーター内ではルート生成せず、既存ルートを再利用
                                        next_route.AddRange(agv.conditions);
                                    }
                                    else if (agv.escape_to == null)
                                    {
                                        #region 動作指示

                                        FloorQR stationqr = controller.GetStationQR(agv.req.station.Trim());
                                        if (stationqr != null)
                                        {
                                            //ルート算出時の棚向きを保持
                                            if (agv.rack != null) rack_deg_pre = agv.rack.degree;

                                            if (agv.req.rack_face.Trim() == "0") agv.req.rack_face = "";
                                            next_route = controller.routeManager.GetMoveConditions_ex(agv, stationqr, agv.req.rack_face);

                                            #region 不正ルートチェック

                                            for (int i = 1; i < next_route.Count; i++)
                                            {
                                                var cur = next_route[i - 1];
                                                if (cur.next_condition != next_route[i])
                                                {
                                                    description = "ルート算出不可(不正ルート1)";

                                                    next_route.Clear();

                                                    agv.req.result = "NG";
                                                    manager.OrderSend(agv.req);

                                                    AgvDatabase.OrderComplete(agv, AgvDatabase.OrderCompleteReason.ROUTING_ERROR);
                                                    
                                                    agv.request_moving = false;
                                                    agv.req = null;

                                                    break;
                                                }

                                                if (cur.cur_qr.autorator_id.Trim() == "" && !cur.cur_qr.next_way.ContainsKey(cur.next_condition.cur_qr))
                                                {
                                                    description = "ルート算出不可(不正ルート2)";

                                                    next_route.Clear();

                                                    agv.req.result = "NG";
                                                    manager.OrderSend(agv.req);

                                                    AgvDatabase.OrderComplete(agv, AgvDatabase.OrderCompleteReason.ROUTING_ERROR);

                                                    agv.request_moving = false;
                                                    agv.req = null;

                                                    break;
                                                }
                                            }

                                            #endregion

                                            #region オートレーター使用不可チェック

                                            if (0 < next_route.Count())
                                            {
                                                //ルート上のオートレーターが使用できない場合、ルートを消去
                                                var autoratorcon = next_route.Where(e => e.cur_qr.autorator_info != null).FirstOrDefault();
                                                if (autoratorcon != null &&
                                                    autoratorcon.cur_qr.autorator_info.Status != null &&
                                                    autoratorcon.cur_qr.autorator_info.Status.IsDisable
                                                    )
                                                {
                                                    description = "オートレーター使用不可";

                                                    next_route.Clear();

                                                    agv.req.result = "NG";
                                                    manager.OrderSend(agv.req);

                                                    AgvDatabase.OrderComplete(agv, AgvDatabase.OrderCompleteReason.AUTORATOR_OFFLINE);

                                                    agv.request_moving = false;
                                                    agv.req = null;
                                                }
                                            }

                                            #endregion

                                            #region 棚操作モードセット

                                            if (0 < next_route.Count())
                                            {
                                                if (agv.escape_complete != null)
                                                {
                                                    agv.escape_complete.escape_from = null;
                                                    agv.escape_complete = null;
                                                }
                                                if (agv.escape_to != null)
                                                {
                                                    agv.escape_to.escape_from = null;
                                                    agv.escape_to = null;
                                                }

                                                if (agv.req.rack_action.Trim() == "1")
                                                {
                                                    description = "ルート算出->[" + stationqr + "][棚上昇]";
                                                    next_route.Last().rack_up_arrive = true;
                                                }
                                                else if (agv.req.rack_action.Trim() == "2")
                                                {
                                                    description = "ルート算出->[" + stationqr + "][棚下降]";
                                                    next_route.Last().rack_down_arrive = true;
                                                }
                                                else if (agv.req.rack_action.Trim() == "C")
                                                {
                                                    description = "ルート算出->[" + stationqr + "][充電]";
                                                    next_route.Last().wait_charge_trg = true;
                                                }
                                                else
                                                {
                                                    description = "ルート算出->[" + stationqr + "]";
                                                }
                                            }

                                            #endregion
                                        }
                                        else
                                        {
                                            //不明ステーション

                                        }

                                        #endregion
                                    }
                                    else
                                    {
                                        description = "回避行動中(" + agv.escape_to + ")";

                                        //lock (manager)
                                        {
                                            if (agv.conditions.Count == 0)
                                            {
                                                //目的地到着
                                                step = 400;
                                                break;
                                            }
                                            else
                                            {
                                                next_route.AddRange(agv.conditions);
                                            }
                                        }
                                    }
                                }

                                #endregion

                                if (0 == next_route.Count)
                                {
                                    //ルート算出不可
                                    step = 900;
                                    break;
                                }
                            }

                            if (0 < next_route.Count)
                            {
                                routed = true;

                                if (agv.escape_complete != null)
                                {
                                    if (0 < next_route.Where(e => e.cur_qr == agv.escape_complete.on_qr).Count())
                                    {
                                        m_swTemp.Restart();
                                        step = 10;
                                        break;
                                    }
                                    else
                                    {
                                        agv.escape_complete = null;
                                    }
                                }

                                #region 算出済みルート設定

                                //lock (manager)
                                {
                                    agv.conditions.Clear();
                                    agv.conditions.AddRange(next_route);
                                    agv.check_conditions.Clear();
                                }

                                m_swTemp.Restart();
                                step = 100;

                                #endregion
                            }

                            if (step == 0)
                            {
                                step = 10;
                                m_swTemp.Restart();
                            }
                        }
                        #endregion

                        break;

                    case 10:

                        #region 処理待機

                        if (200 < m_swTemp.ElapsedMilliseconds)
                        {
                            step = 0;
                        }

                        #endregion

                        break;

                    case 100:

                        #region 動作指示送信

                        //lock (manager)
                        {
                            if (agv.current_route.Count <= 1 && agv.conditions.Count <= 1)
                            {
                                if (agv.req != null && agv.req.station.Trim() != null && agv.on_qr.station_id.Trim() == agv.req.station.Trim())
                                {
                                    if (agv.req.rack_action == "2" && agv.rack != null)
                                    {
                                        //目的地に到着したが、棚を降ろしていない
                                        description = "目的地到着(棚下降中)";
                                    }
                                    else if (agv.req.rack_action == "1" && agv.rack == null && !sta.sta_norack)
                                    {
                                        description = "目的地到着(棚上昇中)";
                                        //目的地に到着したが、棚を上げていない
                                    }
                                    else
                                    {
                                        step = 400;
                                        break;
                                    }
                                }
                                else
                                {
                                    step = 400;
                                    break;
                                }
                            }

                            if (state_received || routed)
                            {
                                routed = false;

                                if (create_next_orders())
                                {
                                    description = "走行指示";

                                    m_swTemp.Restart();
                                    step = 200;
                                }
                                else
                                {
                                    if (agv.conditions.Count == 0)
                                    {
                                        //再ルート生成が必要
                                        m_swTemp.Restart();
                                        step = 10;
                                    }
                                    else
                                    {
                                        //var esc_route = get_escape_route();
                                        //if (0 < esc_route.Count)
                                        //{
                                        //    agv.conditions.Clear();
                                        //    agv.conditions.AddRange(esc_route);
                                        //    agv.check_conditions.Clear();

                                        //    break;
                                        //}

                                        m_swTemp.Restart();
                                        step = 110;
                                    }
                                }
                            }
                        }

                        #endregion

                        break;

                    case 110:

                        #region 再指示待機

                        if (200 <= m_swTemp.ElapsedMilliseconds)
                        {
                            step = 100;
                        }

                        #endregion

                        break;

                    case 200:

                        #region 動作指示応答確認

                        if (agv_order_response)
                        {
                            m_swTemp.Restart();
                            step = 300;
                        }
                        else if (TIMEOUT <= m_swTemp.ElapsedMilliseconds)
                        {
                            description = "指示応答タイムアウト";
                            Log(step + ",指示応答タイムアウト");

                            //再送
                            step = 100;
                        }

                        #endregion

                        break;

                    case 300:

                        #region 状態変化タイムアウト

                        if (200 <= m_swTemp.ElapsedMilliseconds)
                        {
                            description = "状態変化タイムアウト";

                            //next_orders.Add(agv.RouteCancelOrder());
                            step = 100;
                        }

                        #endregion

                        break;

                    case 400:

                        #region 目的地到着時処理
                        {
                            agv.check_conditions.Clear();
                            agv.current_route.Clear();
                            agv.Unreserve(true);

                            state_changed_disp = true;

                            if (agv.escape_to != null)
                            {
                                //escape_agv_qr_pre = agv.escape_to.on_qr;
                                step = 500;
                                break;
                            }

                            agv.escape_complete = null;

                            if (agv.escape_from != null)
                            {
                                agv.escape_from.escape_to = null;
                                agv.escape_from.escape_complete = null;
                                agv.escape_from = null;
                            }

                            if (agv.req == null)
                            {
                                step = 0;
                            }
                            else
                            {
                                //controller.agvs.Where(e => e.escape_from == agv).ToList().ForEach(e => e.escape_from = null);

                                if (agv.req.station.Trim() != null && agv.on_qr.station_id.Trim() == agv.req.station.Trim())
                                {
                                    if (agv.req.rack_action == "2" && agv.rack != null)
                                    {
                                        //目的地に到着したが、棚を降ろしていない
                                        description = "目的地到着(棚下降中)";
                                    }
                                    else if (agv.req.rack_action == "1" && agv.rack == null && !sta.sta_norack)
                                    {
                                        description = "目的地到着(棚上昇中)";
                                        //目的地に到着したが、棚を上げていない
                                    }
                                    else
                                    {
                                        if (!agv.req.inner_request)
                                        {
                                            agv.req.result = "OK";
                                            agv.req.agv = agv.id;
                                            agv.req.rack_no = "";

                                            if (agv.req.rack_action == "1")
                                            {
                                                if (agv.rack != null)
                                                {
                                                    agv.req.rack_no = agv.rack.rack_no;
                                                }
                                                else
                                                {
                                                    agv.req.result = "NG";
                                                }
                                            }

                                            if (agv.req.working == "0")
                                            {
                                                description = "目的地到着(作業完了)";
                                                agv.working = false;
                                            }
                                            else
                                            {
                                                description = "目的地到着(継続アリ)";
                                            }

                                            manager.OrderSend(agv.req);
                                        }
                                        else
                                        {
                                            description = "目的地到着";
                                            agv.working = false;
                                        }

                                        AgvDatabase.OrderComplete(agv, AgvDatabase.OrderCompleteReason.SUCCESS);

                                        agv.request_moving = false;
                                        agv.req = null;

                                        agv.conditions.Clear();

                                        step = 0;
                                    }
                                }
                                else
                                {
                                    step = 0;
                                }
                            }
                        }
                        #endregion

                        break;

                    case 500:

                        #region 回避後にルート復帰する待機

                        if (agv.escape_to != null)
                        {
                            description = "回避完了⇒ルート復帰";

                            agv.escape_to.escape_from = null;

                            agv.escape_complete = agv.escape_to;
                            agv.escape_to = null;

                            step = 0;
                        }
                        //else if (agv.escape_complete == null)
                        //{
                        //    step = 0;
                        //}

                        #endregion

                        break;

                    case 900:

                        #region ルート算出不可応答

                        description = "ルート算出不可";

                        if (agv.req != null)
                        {
                            agv.req.result = "NG";
                            manager.OrderSend(agv.req);

                            AgvDatabase.OrderComplete(agv, AgvDatabase.OrderCompleteReason.ROUTING_ERROR);

                            agv.request_moving = false;
                            agv.req = null;
                        }

                        step = 0;

                        #endregion

                        break;
                }
            }

            protected virtual void agv_state_control()
            {
                switch (step)
                {
                    case 0:

                        if (state_received)
                        {
                            if (sta.sta_charge)
                            {
                                #region 充電停止

                                if (agv.req != null && agv.req.rack_action == "c")
                                {
                                    if (sta.sta_charge)
                                    {
                                        description = "充電停止(上位要求)";

                                        //充電停止動作
                                        step = 800;
                                        break;
                                    }
                                    else
                                    {
                                        //不正
                                        agv.req.result = "NG";
                                        manager.OrderSend(agv.req);

                                        AgvDatabase.OrderComplete(agv, AgvDatabase.OrderCompleteReason.ALREADY_CHARGE_STOPPED);

                                        agv.request_moving = false;
                                        agv.req = null;
                                    }
                                }
                                else
                                {
                                    if (manager.BATTERY_FUL <= sta.bat)
                                    {
                                        description = "充電停止(フル充電)";

                                        //充電フル
                                        step = 800;
                                    }
                                    else if (manager.CHARGE_ABORT_LEVEL <= sta.bat)
                                    {
                                        //自AGVが充電終了可能レベル

                                        if (0 < requests.Count)
                                        {
                                            description = "充電停止(動作指示)";

                                            //動作指示がある

                                            step = 800;
                                        }
                                        else
                                        {
                                            var charge_agvs = controller.agvs.Where(e => e != agv && e.agvRunner != null && e.agvRunner.agv_request && e.agvRunner.communicator.GetState.bat <= manager.BATTERY_LOW)
                                                                             .OrderBy(e => e.agvRunner.communicator.GetState.bat).ToList();
                                            if (0 < charge_agvs.Count())
                                            {
                                                description = "充電停止(譲り合い)";

                                                //充電が必要な他AGVが存在する

                                                step = 800;
                                            }
                                        }
                                    }
                                }

                                #endregion
                            }
                            else
                            {
                                #region 充電停止不可

                                if (agv.req != null && agv.req.rack_action == "c")
                                {
                                    //不正
                                    agv.req.result = "NG";
                                    manager.OrderSend(agv.req);

                                    AgvDatabase.OrderComplete(agv, AgvDatabase.OrderCompleteReason.ALREADY_CHARGE_STOPPED);

                                    agv.request_moving = false;
                                    agv.req = null;
                                    break;
                                }

                                #endregion

                                step = 100;
                            }
                        }

                        break;

                    case 100:

                        #region 走行中処理

                        if (state_received)
                        {
                            //lock (manager)
                            {
                                if (sta.sta_charge)
                                {
                                    description = "充電開始";

                                    agv.current_route.Clear();
                                    step = 0;
                                }
                                else if (0 < agv.current_route.Count)
                                {
                                    List<RouteCondition> remove_cons = new List<RouteCondition>();
                                    foreach (var v in agv.current_route)
                                    {
                                        if (0 == agv.conditions.Where(e => e.cur_qr == v.cur_qr).Count())
                                        {
                                            remove_cons.Add(v);
                                        }
                                        else break;
                                    }
                                    foreach (var v in remove_cons) agv.current_route.Remove(v);

                                    if (0 < agv.current_route.Count)
                                    {
                                        description = "走行中";

                                        RouteCondition lastcon = agv.current_route.Last();

                                        if (lastcon != agv.conditions.Last())
                                        {
                                            //最終目的地では無い場合、追加指示を出すかどうか

                                            if (lastcon.cur_qr.IsLocked(agv))
                                            {
                                                if (agv.rack != null && rack_deg_pre != agv.rack.degree)
                                                {
                                                    //走行中に棚の向きが変化したら追加指示しない

                                                    rack_deg_pre = agv.rack.degree;
                                                }
                                                else
                                                {
                                                    //動作指示をしている最終ポイントに進路排他をかけたら続きの動作指示

                                                    if (create_next_orders())
                                                    {
                                                        description = "走行指示追加";

                                                        m_swTemp.Restart();
                                                        step = 200;
                                                    }
                                                    else
                                                    {
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {

                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        break;

                    case 200:

                        #region 動作指示応答確認

                        if (agv_order_response)
                        {
                            step = 100;
                        }
                        else if (TIMEOUT <= m_swTemp.ElapsedMilliseconds)
                        {
                            Log(step + ",指示応答タイムアウト");

                            //再送
                            step = 100;
                        }

                        #endregion

                        break;

                    case 800:

                        #region 充電停止動作

                        if (state_received)
                        {
                            //充電停止指示
                            if (sta.sta_charge)
                            {
                                description = "充電停止指示";
                                next_orders.Add(agv.ChargeStopOrder());
                            }

                            m_swTemp.Restart();
                            step = 810;
                        }
                        else if (TIMEOUT <= m_swTemp.ElapsedMilliseconds)
                        {
                            if (agv.req != null)
                            {
                                //タイムアウト
                                agv.req.result = "NG";
                                manager.OrderSend(agv.req);

                                agv.request_moving = false;
                                agv.req = null;

                                step = 0;
                            }
                        }

                        #endregion

                        break;

                    case 810:

                        #region 充電停止待機

                        if (!sta.sta_charge)
                        {
                            description = "充電停止完了";

                            if (agv.req != null && agv.req.rack_action == "C") agv.req = null;
                            step = 0;   
                        }
                        else if (TIMEOUT <= m_swTemp.ElapsedMilliseconds)
                        {
                            step = 800;
                        }

                        #endregion

                        break;
                }
            }


            RouteCondition last_order_con = null;

            /// <summary>
            /// 追加指示の生成
            /// </summary>
            /// <returns></returns>
            protected bool create_next_orders()
            {
                RouteConditionList next_route = null;

                if (agv_request)
                {
                    last_order_con = null;
                    agv.current_route.Clear();
                }
                else
                {
                    if (last_order_con != null)
                    {
                        if (agv.on_qr == last_order_con.cur_qr)
                        {
                            //指示を送信した最終ポイントに到達
                            last_order_con = null;
                            agv.current_route.Clear();
                        }
                    }
                }

                if (0 < agv.conditions.Where(e => e.cur_qr == agv.on_qr).Count())
                {
                    //現在地までに至るルートが残っている場合、現在地より前を削除
                    List<RouteCondition> remove_cons = new List<RouteCondition>();
                    foreach (var v in agv.conditions)
                    {
                        if (v.cur_qr == agv.on_qr) break;
                        if (remove_cons != null) remove_cons.Add(v);

                    }
                    foreach (var v in remove_cons)
                    {
                        agv.conditions.Remove(v);
                    }
                }
                else
                {
                    //AGVの現在地が予定していたルートではない
                    agv.conditions.Clear();
                }

                if (agv.conditions.Count == 0)
                {
                    //再ルート生成が必要

                    last_order_con = null;
                    agv.current_route.Clear();

                    return false;
                }

                if (1 < agv.conditions.Count)
                {
                    agv.conditions.Normalization();
                    next_route = reserve_route(agv.conditions);
                }
                else
                {
                    next_route = new RouteConditionList();
                    next_route.AddRange(agv.conditions);
                }

                #region オートレーター処理

                if (agv.in_autorator == null && agv.out_autorator != null)
                {
                    //オートレーター退出待ち

                    if (agv.out_autorator.autorator_info != null)
                    {
                        if (!agv.out_autorator.autorator_info.CanExit(agv))
                        {
                            description = "オートレーター搬出不可";

                            next_route.Clear();
                        }
                        else
                        {
                            description = "オートレーター搬出可";

                            RouteConditionList excepts = new RouteConditionList();
                            var qrs = DistancePoint(agv.out_autorator, 140, excepts);

                            if (0 < qrs.Count)
                            {
                                RouteConditionList rem = new RouteConditionList();
                                bool found_ar_trg = false;
                                foreach (var v in next_route)
                                {
                                    if (agv.floor.code != v.FloorCode) rem.Add(v);

                                    if (found_ar_trg) rem.Add(v);
                                    if (qrs.ContainsKey(v.cur_qr)) found_ar_trg = true;
                                }

                                foreach (var v in rem) next_route.Remove(v);
                            }
                        }
                    }
                }
                else if (agv.in_autorator != null && agv.out_autorator != null)
                {
                    //オートレーター進入中

                    if (!agv.in_autorator.autorator_info.CanEntry(agv))
                    {
                        description = "オートレーター搬入不可";

                        //進入不可・・・オート進入待機位置までのルート
                        RouteConditionList rem = new RouteConditionList();
                        bool found_ar_trg = false;
                        foreach (var v in next_route)
                        {
                            if (found_ar_trg) rem.Add(v);
                            if (v.wait_autorator_in_pretrg) found_ar_trg = true;
                        }
                        foreach (var v in rem) next_route.Remove(v);
                    }
                    else if (0 < next_route.Count && next_route[0].wait_autorator_in_pretrg)
                    {
                        description = "オートレーター搬入可";

                        //進入可&オート進入待機位置・・・オート内までのルート
                        RouteConditionList rem = new RouteConditionList();
                        bool found_ar_trg = false;
                        foreach (var v in next_route)
                        {
                            if (found_ar_trg) rem.Add(v);
                            if (v.wait_autorator_out_trg) found_ar_trg = true;
                            if (v != next_route[0] && v.next_condition != null && v.FloorCode != v.next_condition.FloorCode) found_ar_trg = true;
                        }
                        foreach (var v in rem) next_route.Remove(v);
                    }
                    else
                    {
                        description = "オートレーター搬入可";

                        //進入可&オート進入待機位置手前・・・オート進入待機位置までのルート
                        RouteConditionList rem = new RouteConditionList();
                        bool found_ar_trg = false;
                        foreach (var v in next_route)
                        {
                            if (found_ar_trg) rem.Add(v);
                            if (v.wait_autorator_in_pretrg) found_ar_trg = true;
                            if (v.cur_qr.autorator_id != "") found_ar_trg = true;
                        }
                        foreach (var v in rem) next_route.Remove(v);
                    }
                }

                var ar_in = next_route.Where(e => e.cur_qr.autorator_info != null && !e.wait_autorator_out_trg).FirstOrDefault();
                var ar_out = next_route.Where(e => e.cur_qr.autorator_info != null && e.wait_autorator_out_trg).FirstOrDefault();

                if (ar_in != null && ar_in.prev_condition != null && ar_in.cur_qr.reserve_agv.IndexOf(agv) == 0)
                {
                    //オートレーターに予約をかけた

                    if (!ar_in.cur_qr.autorator_info.IsRequested(agv))
                    {
                        //搬入要求未送信

                        if (ar_out != null && ar_out.cur_qr.autorator_info != null)
                        {
                            int in_deg = ((int)ar_in.prev_condition.cur_qr.next_way[ar_in.cur_qr] + 180) % 360;
                            int out_deg = (int)ar_out.cur_qr.next_way.First().Value;

                            string infloor = ar_in.cur_qr.autorator_info.FloorNo(ar_in.cur_qr.floor.code);
                            string outfloor = ar_out.cur_qr.autorator_info.FloorNo(ar_out.cur_qr.floor.code);

                            string inside = ar_in.cur_qr.autorator_info.Side(in_deg);
                            string outside = ar_out.cur_qr.autorator_info.Side(out_deg);

                            if (ar_in.cur_qr.autorator_info.RequestEntry(agv, infloor, inside, outfloor, outside))
                            {
                                description = "オートレーター搬入要求";

                                agv.in_autorator = ar_in.cur_qr;
                                agv.out_autorator = ar_out.cur_qr;
                            }
                        }

                        RouteConditionList rem = new RouteConditionList();
                        bool found_ar_trg = false;
                        foreach (var v in next_route)
                        {
                            if (found_ar_trg) rem.Add(v);
                            if (v.wait_autorator_in_pretrg) found_ar_trg = true;
                        }
                        foreach (var v in rem) next_route.Remove(v);
                    }
                }

                #endregion

                if (next_route != null && 0 < next_route.Count)
                {
                    if (last_order_con != null)
                    {
                        List<RouteCondition> remove_cons = new List<RouteCondition>();
                        foreach (var v in next_route)
                        {
                            if (v.cur_qr == last_order_con.cur_qr) break;
                            if (remove_cons != null) remove_cons.Add(v);
                        }
                        
                        foreach (var v in remove_cons)
                        {
                            next_route.Remove(v);
                        }
                    }

                    if (0 < next_route.Count)
                    {
                        next_route.Normalization();

                        #region 走行モード・音楽パターン設定

                        foreach (var v in next_route)
                        {
                            if (agv.rack != null)
                            {
                                if (!agv.rack.overhang)
                                {
                                    if (agv.req == null || agv.req.run_mode == "0") v.l_pat = 1;
                                    else v.l_pat = 2;
                                }
                                else
                                {
                                    if (agv.req == null || agv.req.run_mode == "0") v.l_pat = 3;
                                    else v.l_pat = 4;
                                }
                            }

                            if (agv.req == null) v.m_pat = 1;
                            else
                            {
                                byte.TryParse(agv.req.run_music, out v.m_pat);
                            }
                        }

                        #endregion

                        agv.current_route.Clear();
                        agv.current_route.AddRange(next_route);

                        if (cur_con == null) cur_con = agv.current_route[0];

                        RouteCondition crash_con = null;
                        crash_con = cur_con.LockWay();

                        if (crash_con == null)
                        {
                            var orders = agv.current_route[0].GetOrders2();

                            PointF from_location = last_order_con != null ? last_order_con.Location : agv.Location;

                            if (agv.on_qr.autorator_info != null)
                            {
                                //オートレーターから退出する時

                                var realqr = agv.on_qr.autorator_info.RealQR();
                                if (agv.on_qr != realqr)
                                {
                                    from_location = realqr.Location;
                                }
                            }

                            foreach (var v in orders)
                            {
                                v.LocationFrom = from_location;
                                from_location = v.Location;
                            }

                            last_order_con = next_route.Last();

                            next_orders.Clear();
                            next_orders.AddRange(orders);

                            if (0 < next_orders.Count)
                            {
                                Log(step + ",指示データ作成[" + sta.ToString() + "]->[" + next_orders.Last().ToString() + "]");
                                return true;
                            }
                        }
                        else
                        {
                            //衝突検知

                            description = "衝突検知2";
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// 回避行動の判断と回避ルート生成
            /// </summary>
            /// <returns>除外ポイント</returns>
            protected RouteConditionList get_escape_route()
            {
                List<FloorQR> escape_from = new List<FloorQR>();

                #region 待機中の回避ルート生成

                while (true)
                {
                    try
                    {
                        var conflict_route_agv = controller.agvs.Where(e => e != agv && 0 < e.routed_qr.Where(ee => ee == agv.on_qr).Count()).ToList();
                        conflict_route_agv = conflict_route_agv.Concat(controller.agvs.Where(e => e != agv && 0 < e.locked_qr_list.Where(ee => ee == agv.on_qr).Count())).ToList();

                        var q = agv.on_qr.conflict_qr(agv).Where(e => e.on_agv != null && e.on_agv != agv && !e.on_agvs.Contains(agv));
                        foreach (var v in q)
                        {
                            conflict_route_agv.Add(v.on_agv);
                        }

                        if (0 < conflict_route_agv.Count())
                        {
                            //待避が必要

                            foreach (var v in conflict_route_agv)
                            {
                                escape_from.AddRange(v.on_qr.conflict_qr(v));
                                escape_from.AddRange(v.routed_qr);
                            }

                            List<FloorQR> e_route = new List<FloorQR>();
                            foreach (var c in conflict_route_agv)
                            {
                                foreach (var vv in c.conditions)
                                {
                                    e_route.AddRange(vv.cur_qr.conflict_qr(c));
                                }
                            }

                            var escapeQR = GetNonObstructQR(escape_from, e_route);
                            if (escapeQR != null)
                            {
                                agv.escape_to = conflict_route_agv[0];
                                agv.escape_complete = null;
                                conflict_route_agv[0].escape_from = agv;


                                var route = controller.routeManager.GetMoveConditions_ex(agv, escapeQR, "");
                                if (0 < route.Where(e => 0 < conflict_route_agv.Where(ee => ee.on_qr == e.cur_qr).Count()).Count())
                                {
                                    foreach (var v in route)
                                    {
                                        escape_from.Add(v.cur_qr);
                                    }

                                    continue;
                                }

                                return route;
                            }
                        }

                        break;
                    }
                    catch
                    {
                        Sleep(50);
                    }
                }

                #endregion

                return new RouteConditionList();
            }

            /// <summary>
            /// ルート予約処理
            /// </summary>
            /// <param name="route"></param>
            /// <returns></returns>
            protected RouteConditionList reserve_route(RouteConditionList route)
            {
                RouteConditionList new_route = new RouteConditionList();
                RouteConditionList reserved_route = new RouteConditionList();

                //lock (manager)
                {
                    Dictionary<RouteCondition, Dictionary<FloorQR, double>> escape_points = get_escape_point(route);
                    SynchronizedList<CheckConditions> checkconditions = get_check_conditions(route, escape_points);

                    agv.check_conditions.Clear();
                    agv.check_conditions.AddRange(checkconditions);

                    double run_distance = 0;
                    PointF run_position = agv.Location;

                    bool escape = false;
                    bool swap_order_run = false;
                    bool wait = false;


                    //最後に検出したWP
                    CheckConditions last_wp = null;
                    RouteConditionList reserved_cons = new RouteConditionList();

                    float rack_deg_org = agv.rack != null ? agv.rack.degree : 9999;
                    FloorMap floor_org = agv.floor;

                    foreach (var con in route)
                    {
                        FloorMap floor_pre = agv.floor;
                        agv.floor = con.cur_qr.floor;

                        if (agv.rack != null) agv.rack.degree = con.cur_rack_degree;

                        bool break_out = false;

                        if (con.cur_qr.OnOtherAGV_ConflictAround(agv) != null)
                        {
                            //他AGVが存在

                            if (last_wp == null)
                            {
                                //最初のWPに到達していない

                                if (route[0].IsRoundWay(con.cur_qr))
                                {
                                    //始点から現在地まで双方向通路

                                    //...始点から動かない
                                    new_route.Clear();

                                    //全予約解除
                                    agv.Unreserve(true);
                                }
                                else
                                {
                                    //始点から現在地まで一方通行

                                    //...他AGVを追走
                                }
                            }
                            else
                            {
                                if (last_wp.check_condition.IsRoundWay(con.cur_qr))
                                {
                                    //最後のWPから現在地まで双方向

                                    RouteConditionList rem_route = new RouteConditionList();
                                    bool rem_start = false;

                                    #region 予約を解除

                                    if (last_wp.check_point_type == CheckConditions.enCheckPointType.EXCLUSION)
                                    {
                                        //単独予約CP
                                        //...CPを含めて予約解除

                                        foreach (var rem_con in reserved_cons)
                                        {
                                            if (last_wp.check_condition == rem_con)
                                            {
                                                rem_start = true;
                                            }
                                            if (rem_start)
                                            {
                                                rem_con.cur_qr.Unreserve(agv, false);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //順番待ちCP
                                        //...CPは予約解除しない

                                        List<FloorQR> keep_reserve = new List<FloorQR>();

                                        foreach (var rem_con in reserved_cons)
                                        {
                                            if (last_wp.check_condition == rem_con)
                                            {
                                                rem_start = true;

                                                foreach (var v in rem_con.cur_qr.conflict_qr(agv))
                                                {
                                                    keep_reserve.Remove(v);
                                                }

                                                keep_reserve.Add(rem_con.cur_qr);
                                            }
                                            else if (rem_start)
                                            {
                                                rem_con.cur_qr.Unreserve(agv, keep_reserve);
                                            }
                                            else
                                            {
                                                keep_reserve.AddRange(rem_con.cur_qr.conflict_qr(agv));
                                            }
                                        }
                                    }

                                    #endregion

                                    #region ルート指示をキャンセル

                                    //WPで動作停止
                                    if (last_wp.wait_condition == null)
                                    {
                                        new_route.Clear();
                                    }
                                    else
                                    {
                                        rem_route = new RouteConditionList();
                                        rem_start = false;
                                        foreach (var rem_con in new_route)
                                        {
                                            if (last_wp.wait_condition == rem_con)
                                            {
                                                rem_start = true;
                                            }
                                            else if (rem_start)
                                            {
                                                rem_route.Add(rem_con);
                                            }
                                        }

                                        foreach (var rem_con in rem_route)
                                        {
                                            new_route.Remove(rem_con);
                                        }
                                    }

                                    #endregion
                                }
                                else
                                {
                                    //直前のWPから現在地まで一方通行
                                    //...他AGVを追走
                                }
                            }

                            break;
                        }
                        else
                        {
                            //他AGVが存在しない

                            new_route.Add(con);
                            int reserved_order = con.cur_qr.Reserve(agv);
                            reserved_cons.Add(con);

                            //探査距離加算
                            if (con.prev_condition != null && floor_pre == agv.floor)
                            {
                                run_distance += con.prev_condition.cur_qr.Distance(con.cur_qr);
                            }

                            var cps = agv.check_conditions.Where(e => e.wait_condition != null && e.wait_condition.cur_qr == con.cur_qr).ToList();
                            if (0 < cps.Count)
                            {
                                escape = false;
                                swap_order_run = false;
                                wait = false;

                                for (int cp_count = 0; cp_count < cps.Count; cp_count++)
                                {
                                    CheckConditions cur_cp = cps[cp_count];

                                    if (last_wp == null)
                                    {
                                        //WP未検出

                                        CheckConditions near_cp = agv.check_conditions.Where(e => e.check_condition == con && e.wait_condition == null).FirstOrDefault();

                                        if (near_cp != null && near_cp.check_condition != route.Last())
                                        {
                                            //初回WPより先に、WPの無いCPがある場合、CPを探査対象とする

                                            foreach (var v in route)
                                            {
                                                if (cur_cp != null && v == cur_cp.check_condition)
                                                {
                                                    break;
                                                }

                                                if (v == near_cp.check_condition)
                                                {
                                                    cur_cp = near_cp;
                                                    break;
                                                }
                                            }
                                        }
                                    }

                                    if (cur_cp == null && (last_wp != null && last_wp.wait_condition == null))
                                    {
                                        cur_cp = last_wp.next_check_condition;
                                    }

                                    if (cur_cp != null)
                                    {
                                        //とあるCPの待機ポイントを検出

                                        var cur_cp_con = cur_cp.check_condition;
                                        var next_cp = cur_cp.next_check_condition;
                                        var next_cp_con = next_cp != null ? next_cp.check_condition : null;
                                        var next_wp_con = next_cp != null && next_cp.wait_condition != null ? next_cp.wait_condition : null;

                                        //とあるCPの次CPまで双方向
                                        bool nextcp_is_roundway = next_cp_con != null ? cur_cp_con.IsRoundWay(next_cp_con.cur_qr) : false;

                                        //とあるCPへの進入は双方向
                                        bool curcp_is_roundin = cur_cp_con.prev_condition.IsRoundWay(cur_cp_con.cur_qr);

                                        int reserved_order_cp = -1;

                                        if (cur_cp_con.cur_qr.OnOtherAgv(agv) == null)
                                        {
                                            //次のCPを予約してみて順番を得る

                                            reserved_order_cp = cur_cp_con.cur_qr.Reserve(agv, true);
                                            reserved_cons.Add(cur_cp_con);
                                        }
                                        else
                                        {
                                            //CPに他AGVが存在

                                            //CPの予約を解除
                                            foreach (var cp in cps)
                                            {
                                                cp.check_condition.cur_qr.Unreserve(agv, false);
                                            }

                                            break_out = true;
                                            break;
                                        }

                                        if (!break_out && !wait && !swap_order_run && !escape)
                                        {
                                            if (cur_cp.check_point_type == CheckConditions.enCheckPointType.EXCLUSION)
                                            {
                                                //単独予約CP


                                                if (reserved_order_cp != 0)
                                                {
                                                    //CPは予約済み・・・CPは予約解除して現ポイントで停止

                                                    cur_cp_con.cur_qr.Unreserve(agv, false);
                                                    break_out = true;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                //順番待ちCP

                                                var first_agv = cur_cp_con.cur_qr.GetTopReservedAgv();

                                                if (reserved_order_cp == 0)
                                                {
                                                    //CPの予約順が先頭

                                                    if (nextcp_is_roundway)
                                                    {
                                                        //次のCPまでに他AGVが存在しない、予約がないか
                                                        bool on_other_agv = false;
                                                        for (var v = cur_cp.check_condition.next_condition; v != null; v = v.next_condition)
                                                        {
                                                            if (v.cur_qr.OnOtherAGV_ConflictAround(agv) != null)
                                                            {
                                                                on_other_agv = true;
                                                                break;
                                                            }

                                                            if (v == cur_cp.next_check_condition.check_condition) break;
                                                        }

                                                        if (on_other_agv)
                                                        {
                                                            //次のCPまでに他AGVが存在する

                                                            break_out = true;
                                                        }
                                                    }
                                                }
                                                else if (reserved_order_cp == 1)
                                                {
                                                    //CPの予約順が2番目
                                                    RouteConditionList firstagv_cons = new RouteConditionList(first_agv.conditions);

                                                    if (0 < firstagv_cons.Where(e => e.cur_qr.conflict_qr(first_agv).Contains(agv.on_qr)).Count())
                                                    {
                                                        //予約順番1番目のルートが自分の現在地を含む

                                                        if (0 < agv.conditions.Where(e => e.cur_qr.conflict_qr(first_agv).Contains(first_agv.on_qr)).Count())
                                                        {
                                                            //自分のルートが予約順番1番目の現在地を含む

                                                            escape = true;
                                                        }
                                                        else
                                                        {
                                                            RouteConditionList first_agv_current_route = new RouteConditionList(first_agv.current_route);

                                                            if (0 < agv.conditions.Where(e => first_agv.on_qr.conflict_qr(first_agv).Contains(e.cur_qr) || 0 < first_agv_current_route.Where(ee => ee.cur_qr.conflict_qr(first_agv).Contains(e.cur_qr)).Count()).Count())
                                                            {
                                                                //自分のルートが予約順番1番目の現在地(指示済みルートも)を含む

                                                                escape = true;
                                                            }
                                                            else
                                                            {
                                                                //自分のルートが予約順番1番目の現在地を含まない
                                                                swap_order_run = true;
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        //予約順番1番目のルートが自分の現在地を含まない

                                                        //予約順番1番目のAGVが行きすぎるまで待機
                                                        wait = true;

                                                        var first_agv_current_route = new RouteConditionList(first_agv.current_route);
                                                        if (0 == first_agv_current_route.Where(e => e.cur_qr.conflict_qr(first_agv).Contains(cur_cp_con.cur_qr)).Count())
                                                        {
                                                            RouteConditionList first_route = new RouteConditionList();
                                                            foreach (var v in firstagv_cons)
                                                            {
                                                                first_route.Add(v);
                                                                if (v.cur_qr == cur_cp_con.cur_qr) break;
                                                            }

                                                            int firstagv_cost = controller.routeManager.CalcRouteConditionCost(first_route);

                                                            RouteConditionList second_route = new RouteConditionList();
                                                            foreach (var v in agv.conditions)
                                                            {
                                                                second_route.Add(v);
                                                                if (v.cur_qr == cur_cp_con.cur_qr) break;
                                                            }

                                                            int secondagv_cost = controller.routeManager.CalcRouteConditionCost(second_route);


                                                            if (secondagv_cost + 200 < firstagv_cost)
                                                            {
                                                                //予約順番1番目のAGVがまだ遠いので、自AGVが先行する
                                                                swap_order_run = true;
                                                            }
                                                        }

                                                        if (wait)
                                                        {
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    //CPの予約順が3番目以降

                                                    wait = true;
                                                }

                                                if (wait)
                                                {
                                                    if (last_wp == null || (last_wp != null && last_wp.wait_condition == null))
                                                    {
                                                        //最初のWPに到達していない

                                                        if (route[0].IsRoundWay(con.cur_qr))
                                                        {
                                                            //始点から現在地まで双方向通路

                                                            //...始点から動かない
                                                            new_route.Clear();
                                                        }
                                                        else
                                                        {
                                                            //始点から現在地まで一方通行

                                                            //...他AGVを追走
                                                        }
                                                    }
                                                    else if (last_wp != null)
                                                    {
                                                        if (last_wp.wait_condition.IsRoundWay(con.cur_qr))
                                                        {
                                                            //最後のWPから現在地まで双方向

                                                            RouteConditionList rem_route = new RouteConditionList();
                                                            bool rem_start = false;

                                                            #region 予約を解除

                                                            if (last_wp.check_point_type == CheckConditions.enCheckPointType.EXCLUSION)
                                                            {
                                                                //単独予約CP
                                                                //...CPを含めて予約解除

                                                                List<FloorQR> keep_reserve = new List<FloorQR>();

                                                                foreach (var rem_con in reserved_cons)
                                                                {
                                                                    if (last_wp.check_condition == rem_con)
                                                                    {
                                                                        rem_start = true;
                                                                    }

                                                                    if (rem_start)
                                                                    {
                                                                        rem_con.cur_qr.Unreserve(agv, keep_reserve);
                                                                    }
                                                                    else
                                                                    {
                                                                        keep_reserve.AddRange(rem_con.cur_qr.conflict_qr(agv));
                                                                    }
                                                                }
                                                            }
                                                            else
                                                            {
                                                                //順番待ちCP
                                                                //...CPは予約解除しない

                                                                List<FloorQR> keep_reserve = new List<FloorQR>();

                                                                foreach (var rem_con in reserved_cons)
                                                                {
                                                                    if (last_wp.wait_condition == rem_con)
                                                                    {
                                                                        rem_start = true;

                                                                        foreach (var v in rem_con.cur_qr.conflict_qr(agv))
                                                                        {
                                                                            keep_reserve.Remove(v);
                                                                        }

                                                                        keep_reserve.Add(rem_con.cur_qr);
                                                                    }
                                                                    else if (rem_start)
                                                                    {
                                                                        if (rem_con.cur_qr.GetCP == null)
                                                                        {
                                                                            rem_con.cur_qr.Unreserve(agv, keep_reserve);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        keep_reserve.AddRange(rem_con.cur_qr.conflict_qr(agv));
                                                                    }
                                                                }
                                                            }

                                                            #endregion

                                                            #region ルート指示をキャンセル

                                                            rem_start = false;

                                                            //WPで動作停止
                                                            foreach (var rem_con in new_route)
                                                            {
                                                                if (last_wp.wait_condition == rem_con)
                                                                {
                                                                    rem_start = true;
                                                                }
                                                                else if (rem_start)
                                                                {
                                                                    rem_route.Add(rem_con);
                                                                }
                                                            }

                                                            foreach (var rem_con in rem_route)
                                                            {
                                                                new_route.Remove(rem_con);
                                                            }

                                                            #endregion
                                                        }
                                                        else
                                                        {
                                                            //直前のWPから現在地まで一方通行
                                                            //...他AGVを追走
                                                        }
                                                    }

                                                    break_out = true;
                                                }
                                                else if (swap_order_run)
                                                {
                                                    //予約2番目を予約先頭へ移動

                                                    foreach (var v in cps)
                                                    {
                                                        swap_reserve_to_top(v.check_condition.cur_qr, agv);
                                                    }
                                                    break_out = true;
                                                }
                                                else if (escape)
                                                {
                                                    change_escape_route(first_agv, agv, new_route, cur_cp_con, cur_cp);

                                                    break_out = true;
                                                }
                                            }
                                        }

                                        if (last_wp == null || !cur_cp.check_condition.cur_qr.conflict_qr(agv).Contains(last_wp.check_condition.cur_qr))
                                        {
                                            last_wp = cur_cp;
                                        }
                                    }
                                }

                                if (lock_distance <= run_distance)
                                {
                                    //WPは一定距離以上先なら、今回の走行指示はここまで
                                    break_out = true;
                                    break;
                                }
                            }
                            else
                            {
                                //待機ポイントではない

                                if (reserved_order != 0 && 0 < run_distance)
                                {
                                    //他AGVが予約済み

                                    con.cur_qr.Unreserve(agv, false);

                                    if (last_wp == null || last_wp.wait_condition == null)
                                    {
                                        //最初のWPに到達していない

                                        if (route[0].IsRoundWay(con.cur_qr))
                                        {
                                            //始点から現在地まで双方向通路

                                            //...始点から動かない
                                            new_route.Clear();
                                        }
                                        else
                                        {
                                            //始点から現在地まで一方通行

                                            //...他AGVを追走
                                        }
                                    }
                                    else
                                    {
                                        if (last_wp.wait_condition != null && last_wp.wait_condition.IsRoundWay(con.cur_qr))
                                        {
                                            //最後のWPから現在地まで双方向

                                            RouteConditionList rem_route = new RouteConditionList();
                                            bool rem_start = false;

                                            #region 予約を解除

                                            if (last_wp.check_point_type == CheckConditions.enCheckPointType.EXCLUSION)
                                            {
                                                //単独予約CP
                                                //...CPを含めて予約解除

                                                List<FloorQR> keep_reserve = new List<FloorQR>();

                                                foreach (var rem_con in reserved_cons)
                                                {
                                                    if (last_wp.check_condition == rem_con)
                                                    {
                                                        rem_start = true;
                                                    }

                                                    if (rem_start)
                                                    {
                                                        if (rem_con.cur_qr.GetCP == null)
                                                        {
                                                            rem_con.cur_qr.Unreserve(agv, keep_reserve);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        keep_reserve.AddRange(rem_con.cur_qr.conflict_qr(agv));
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                //順番待ちCP
                                                //...CPは予約解除しない

                                                List<FloorQR> keep_reserve = new List<FloorQR>();

                                                foreach (var rem_con in reserved_cons)
                                                {
                                                    if (last_wp.check_condition == rem_con)
                                                    {
                                                        rem_start = true;

                                                        foreach (var v in rem_con.cur_qr.conflict_qr(agv))
                                                        {
                                                            keep_reserve.Remove(v);
                                                        }

                                                        keep_reserve.Add(rem_con.cur_qr);
                                                    }
                                                    else if (rem_start)
                                                    {
                                                        if (rem_con.cur_qr.GetCP == null)
                                                        {
                                                            rem_con.cur_qr.Unreserve(agv, keep_reserve);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        keep_reserve.AddRange(rem_con.cur_qr.conflict_qr(agv));
                                                    }
                                                }
                                            }

                                            #endregion

                                            #region ルート指示をキャンセル

                                            //WPで動作停止
                                            foreach (var rem_con in new_route)
                                            {
                                                if (last_wp.wait_condition == rem_con)
                                                {
                                                    rem_start = true;
                                                }
                                                else if (rem_start)
                                                {
                                                    rem_route.Add(rem_con);
                                                }
                                            }

                                            foreach (var rem_con in rem_route)
                                            {
                                                new_route.Remove(rem_con);
                                            }

                                            #endregion
                                        }
                                        else
                                        {
                                            //直前のWPから現在地まで一方通行
                                            //...他AGVを追走
                                        }
                                    }

                                    break_out = true;
                                    break;
                                }
                            }

                            if (break_out) break;
                        }
                    }

                    if (agv.rack != null) agv.rack.degree = rack_deg_org;
                    agv.floor = floor_org;
                }

                return new_route;
            }

            /// <summary>
            /// 予約順を先頭へ移動する
            /// </summary>
            /// <param name="con"></param>
            /// <param name="agv"></param>
            protected void swap_reserve_to_top(FloorQR qr, FloorAGV agv)
            {
                //予約2番目を予約先頭へ移動

                lock (qr)
                {
                    qr.Unreserve(agv, false);
                    qr.Reserve(agv, 0);
                }
            }

            protected bool change_escape_route(FloorAGV first_agv, FloorAGV agv, RouteConditionList new_route, RouteCondition cur_cp_con, CheckConditions cur_cp)
            {
                bool break_out = false;

                #region 回避ルートに置き換え

                if (agv.escape_from == null)
                {
                    first_agv.escape_from = agv;
                    agv.escape_to = first_agv;
                    agv.escape_complete = null;

                    //回避先を選択

                    bool can_escape = false;
                    CheckConditions cc = cur_cp;

                    for (int i = 0; cc != null; i++)
                    {
                        if (0 < i)
                        {
                            //現在のCPで回避できない場合、手前のCPで回避を試みる
                            cc = cc.prev_check_condition;
                        }

                        foreach (var vv in cc.escape_points)
                        {
                            double escape_degree = vv.Value;
                            FloorQR escape_qr = cc.check_condition.cur_qr.next_way.Where(e => e.Value == escape_degree).FirstOrDefault().Key;

                            if (0 == first_agv.routed_qr.Where(e => e == vv.Key).Count())
                            {
                                float agv_rack_degree = agv.degree + agv.rack.degree;

                                //自AGVが回避した後の棚向きを事前に取得する
                                var escape_route_temp = controller.routeManager.GetMoveConditions_ex(agv, escape_qr, "");
                                var rack_turn_con = escape_route_temp.Where(e => e.rack_turn_arrive).LastOrDefault();
                                if (rack_turn_con != null)
                                {
                                    agv_rack_degree = (rack_turn_con.agv_turn_arrive_degree + rack_turn_con.rack_turn_arrive_degree) % 360;
                                }

                                double escape_distance = agv.radius + first_agv.radius;
                                double firstagv_throw_degree = 0;
                                RouteCondition firstagv_cp_con = first_agv.conditions.Where(e => e.cur_qr == cc.check_condition.cur_qr).FirstOrDefault();
                                if (firstagv_cp_con == null)
                                {
                                    break_out = true;
                                    break;
                                }
                                else if (firstagv_cp_con.prev_condition != null)
                                {
                                    firstagv_throw_degree = firstagv_cp_con.prev_condition.cur_qr.next_way[firstagv_cp_con.cur_qr];
                                }

                                if (agv.rack != null && first_agv.rack != null)
                                {
                                    double pass_degree1 = (vv.Value + 180 + agv_rack_degree) % 360;
                                    if (pass_degree1 % 180 == 0)
                                    {
                                        //SizeL側を通過
                                        escape_distance = Math.Max(escape_distance, agv.rack.sizeL);
                                    }
                                    else
                                    {
                                        //SizeW側を通過
                                        escape_distance = Math.Max(escape_distance, agv.rack.sizeW);
                                    }

                                    double pass_degree2 = (vv.Value + 180 + firstagv_cp_con.cur_rack_degree) % 360;
                                    if (pass_degree2 % 180 == 0)
                                    {
                                        //SizeL側を通過
                                        escape_distance = Math.Max(escape_distance, first_agv.rack.sizeL);
                                    }
                                    else
                                    {
                                        //SizeW側を通過
                                        escape_distance = Math.Max(escape_distance, first_agv.rack.sizeW);
                                    }
                                }
                                else if (agv.rack != null && first_agv.rack == null)
                                {
                                    double pass_degree = (vv.Value + 180 + agv_rack_degree) % 360;
                                    if (pass_degree % 180 == 0)
                                    {
                                        //SizeL側を通過
                                        escape_distance = Math.Max(escape_distance, agv.rack.sizeL);
                                    }
                                    else
                                    {
                                        //SizeW側を通過
                                        escape_distance = Math.Max(escape_distance, agv.rack.sizeW);
                                    }
                                }
                                else if (agv.rack == null && first_agv.rack != null)
                                {
                                    double pass_degree = (vv.Value + 180 + first_agv.rack.degree) % 360;

                                    if (pass_degree % 180 == 0)
                                    {
                                        //SizeL側を通過
                                        escape_distance = Math.Max(escape_distance, first_agv.rack.sizeL);
                                    }
                                    else
                                    {
                                        //SizeW側を通過
                                        escape_distance = Math.Max(escape_distance, first_agv.rack.sizeW);
                                    }
                                }
                                else if (first_agv.rack != null)
                                {
                                    escape_distance = Math.Max(escape_distance, first_agv.rack.SizeMax);
                                }

                                //回避先を再選択
                                var escapes = DistancePoint(cc.check_condition.cur_qr, escape_distance, null, vv.Value);
                                if (0 < escapes.Count)
                                {
                                    escape_qr = escapes.First().Key;

                                    var escape_route = controller.routeManager.GetMoveConditions_ex(agv, escape_qr, "");

                                    //STが含まれるルートは除外
                                    bool station_include = false;
                                    bool start = false;
                                    foreach (var v in escape_route)
                                    {
                                        if (v.cur_qr == cc.check_condition.cur_qr) start = true;
                                        if (start)
                                        {
                                            if (v.cur_qr.station_id.Trim() != "")
                                            {
                                                station_include = true;
                                                break;
                                            }
                                        }
                                    }
                                    if (station_include) continue;

                                    if (1 < escape_route.Count)
                                    {
                                        //CPより手前までの予約を解除
                                        foreach (var v in new_route)
                                        {
                                            if (v == cc.check_condition) break;
                                            v.cur_qr.Unreserve(agv, false);
                                        }

                                        //var curqr = escape_route.Where(e => e.cur_qr == agv.on_qr).ToList();
                                        //foreach (var vvv in curqr) escape_route.Remove(vvv);

                                        agv.check_conditions.Clear();
                                        agv.conditions.Clear();
                                        agv.conditions.AddRange(escape_route);

                                        if (cur_con != null)
                                        {
                                            foreach (var v in agv.conditions)
                                            {
                                                if (v.cur_qr == cur_con.cur_qr)
                                                {
                                                    cur_con = v;
                                                    break;
                                                }
                                            }
                                        }

                                        new_route = reserve_route(agv.conditions);
                                        can_escape = true;

                                        //回避のためCPの予約順を先頭へ
                                        swap_reserve_to_top(cur_cp_con.cur_qr, agv);

                                        break;
                                    }
                                }
                            }
                        }

                        if (break_out) break;
                        if (can_escape) break;
                        if (cc.prev_check_condition == null) break;
                    }
                }

                #endregion

                return break_out;
            }

            protected SynchronizedList<CheckConditions> get_check_conditions(RouteConditionList route, Dictionary<RouteCondition, Dictionary<FloorQR, double>> escape_points)
            {
                SynchronizedList<CheckConditions> ret = new SynchronizedList<CheckConditions>();

                #region チェックポイントを列挙

                //ルート上の全チェックポイントを検出(目的地を含む)
                for (int i = 0; i < escape_points.Count; i++)
                {
                    bool add_cp = true;

                    var kv = escape_points.ElementAt(i);
                    if (ret.Where(e => e.check_condition.cur_qr == kv.Key.cur_qr).Count() == 0)
                    {
                        CheckConditions cc = new CheckConditions();
                        cc.check_condition = kv.Key;

                        if (kv.Key != escape_points.Last().Key)
                        {
                            var dup_cp = ret.Where(e => e.wait_condition != null && cc.check_condition.cur_qr.conflict_qr(agv).Contains(e.check_condition.cur_qr)).ToList();
                            if (0 < dup_cp.Count)
                            {
                                cc.wait_condition = dup_cp.First().wait_condition;
                            }
                            else
                            {
                                double dist = 0;
                                PointF dist_pre = cc.check_condition.Location;

                                for (RouteCondition v = cc.check_condition.prev_condition; v != null; v = v.prev_condition)
                                {
                                    dist += agv.Distance(dist_pre, v.Location);
                                    dist_pre = v.Location;

                                    //待機ポイントは最大棚幅分を空けたポイント
                                    int need_distance = RackMaster.RackSizeMax;

                                    if (need_distance < dist)
                                    {
                                        cc.wait_condition = v;
                                        break;
                                    }
                                }

                                if (ret.Count == 0 && cc.wait_condition == null)
                                {
                                    cc.wait_condition = agv.conditions.First();
                                }
                            }

                            if (!add_cp) continue;

                            //if (cc.wait_condition != null)
                            {
                                //棚最大幅 or 突き当たりのQRを取得
                                foreach (var vv in kv.Value)
                                {
                                    FloorQR qq = kv.Key.cur_qr;
                                    double distance = 0;
                                    while (true)
                                    {
                                        var qq2 = qq.next_way.Where(e => e.Value == vv.Value).FirstOrDefault();
                                        if (qq2.Key == null) break;
                                        if (qq2.Key.station_id.Trim() != "") break;

                                        distance += qq.Distance(qq2.Key);
                                        qq = qq2.Key;

                                        if (RackMaster.RackSizeMax < distance) break;
                                    }

                                    cc.escape_points[qq] = vv.Value;
                                }
                            }
                        }
                        else
                        {
                            //cc.wait_condition = kv.Key;
                        }

                        //if (cc.wait_condition != null)
                        {
                            ret.Add(cc);
                        }
                    }
                }

                for (int i = 0; i < ret.Count; i++)
                {
                    if (i < ret.Count - 1)
                    {
                        ret[i].next_check_condition = ret[i + 1];
                    }

                    if (0 < i)
                    {
                        ret[i].prev_check_condition = ret[i - 1];
                    }
                }

                if (0 < ret.Count)
                {
                    CheckConditions cur_check = ret[0];

                    if (route[0].cur_qr == cur_check.check_condition.cur_qr && cur_check.next_check_condition != null)
                    {
                        cur_check = cur_check.next_check_condition;
                    }

                    List<FloorQR> checkedqr = new List<FloorQR>();

                    foreach (var v in route)
                    {
                        if (cur_check.check_condition != null)
                        {
                            if (!route.Contains(cur_check.check_condition))
                            {
                                checkedqr.Add(cur_check.check_condition.cur_qr);

                                cur_check = cur_check.next_check_condition;
                            }

                            if (v.cur_qr == cur_check.check_condition.cur_qr && cur_check.next_check_condition != null || checkedqr.Contains(cur_check.check_condition.cur_qr))
                            {
                                checkedqr.Add(cur_check.check_condition.cur_qr);

                                cur_check = cur_check.next_check_condition;
                            }
                        }

                        //v.check_con = cur_check;
                    }
                }

                #endregion

                return ret;
            }

            /// <summary>
            /// 回避ポイントを検索
            /// </summary>
            /// <param name="route"></param>
            /// <param name="escape_qr"></param>
            /// <returns></returns>
            protected Dictionary<RouteCondition, Dictionary<FloorQR, double>> get_escape_point(RouteConditionList route)
            {
                var escape_qrs = new Dictionary<RouteCondition, Dictionary<FloorQR, double>>();
                RouteConditionList branch_cons = new RouteConditionList();

                do
                {
                    //try
                    {
                        //ルート上で分岐可能なポイントを抽出
                        foreach (var v in route)
                        {
                            if (v.prev_condition != null && !v.prev_condition.IsRoundWay(v.cur_qr))
                            {
                                //ルート上で一方通行で進入＆合流ポイントを抽出
                                if (3 <= v.cur_qr.prev_way.Count)
                                {
                                    //一方通行で進入＆合流ポイントは回避不可チェックポイントとする
                                    escape_qrs[v] = new Dictionary<FloorQR, double>();
                                }
                            }

                            List<FloorQR> branch_ways = new List<FloorQR>();
                            foreach (var vv in v.cur_qr.next_way)
                            {
                                if (route.Where(e => e.cur_qr == vv.Key).Count() == 0)
                                {
                                    branch_ways.Add(vv.Key);
                                }
                            }

                            if (0 < branch_ways.Count && v.prev_condition != null)
                            {
                                branch_cons.Add(v);
                            }
                        }

                        if (branch_cons.Count == 0 && 0 < route.Count)
                        {
                            branch_cons.Add(route.Last());
                        }

                        //分岐可能ポイントからルート以外のポイントへ
                        foreach (var v in branch_cons)
                        {
                            if (v.prev_condition != null && !v.prev_condition.IsRoundWay(v.cur_qr))
                            {
                                //一方通行で進入

                                if (!escape_qrs.ContainsKey(v)) escape_qrs[v] = new Dictionary<FloorQR, double>();
                            }
                            else
                            {
                                foreach (var vv in v.cur_qr.next_way)
                                {
                                    //ルート上の次ポイントは除外
                                    if (v.next_condition != null && vv.Key == v.next_condition.cur_qr) continue;
                                    //ルート上の前ポイントは除外
                                    if (v.prev_condition != null && vv.Key == v.prev_condition.cur_qr) continue;
                                    ////STは除外
                                    //if (vv.Key.station_id.Trim() != "") continue;

                                    int from_direction = (v.prev_condition.Direction + 180) % 360;
                                    if (0 <= from_direction)
                                    {
                                        int dir_diff = (int)((from_direction - vv.Value + 360) % 360);
                                        if (dir_diff <= 85)
                                        {
                                            //進入角度との差が90度以上ない分岐は除外
                                            continue;
                                        }
                                    }

                                    int next_direction = v.Direction;
                                    if (0 <= next_direction)
                                    {
                                        int dir_diff = (int)((next_direction - vv.Value + 360) % 360);
                                        if (dir_diff <= 85)
                                        {
                                            //進行角度との差が90度以上ない分岐は除外
                                            continue;
                                        }
                                    }

                                    if (0 <= from_direction && 0 <= next_direction && (next_direction - from_direction + 360) % 360 <= 85)
                                    {
                                        //進入角度と進行角度の差が90度以上ない分岐は除外
                                        continue;
                                    }

                                    Dictionary<FloorQR, double> qrs = null;

                                    if (agv.rack == null)
                                    {
                                        qrs = DistancePoint(v.cur_qr, agv.radius * 2, route);
                                    }
                                    else
                                    {
                                        //int rack_size_max = RackMaster.RackSizeMax;
                                        //qrs = DistancePoint(v.cur_qr, rack_size_max, route);

                                        int rack_deg = v.cur_rack_degree;
                                        if ((vv.Value + rack_deg) % 180 == 0)
                                        {
                                            qrs = DistancePoint(v.cur_qr, agv.rack.sizeL, route);
                                        }
                                        else
                                        {
                                            qrs = DistancePoint(v.cur_qr, agv.rack.sizeW, route);
                                        }
                                    }

                                    //現在地での棚とAGVの向きを保持
                                    float rack_deg_org = agv.rack != null ? agv.rack.degree : 9999;
                                    int agv_deg_org = agv.degree;

                                    //一時的にCPでの向きにする
                                    agv.degree = v.Direction;

                                    foreach (var vvv in qrs)
                                    {
                                        //棚を持っている場合、CPでの棚向きを一時的に設定
                                        if (agv.rack != null)
                                        {
                                            agv.rack.degree = v.cur_rack_degree;

                                            //回避先がSTにかぶる場合は除外
                                            if (0 < vvv.Key.conflict_qr(agv).Where(e => e.station_id != "").Count()) continue;
                                        }

                                        ////回避先の周囲に他AGVがいる場合は除外
                                        //if (0 < vvv.Key.conflict_qr(agv).Where(e => e.OnOtherAgv(agv) != null && e.OnOtherAgv(agv) != agv.escape_to).Count()) continue;

                                        var rtemp1 = controller.routeManager.GetMoveConditions_ex(agv, v.cur_qr, vvv.Key, "");
                                        if (0 < rtemp1.Count)
                                        {
                                            ////STが含まれるルートは除外
                                            //if (0 < rtemp1.Where(e => e.cur_qr.station_id.Trim() != "").Count()) continue;

                                            //オートレーターが含まれるルートは除外
                                            if (0 < rtemp1.Where(e => 0 < e.cur_qr.conflict_qr(agv).Where(
                                                                ee => ee.autorator_id != "").Count()).Count()) continue;

                                            //CPを通過後に棚回転するルートは除外
                                            {
                                                bool start = false;
                                                bool rack_rotate_after_cp = false;
                                                foreach (var x in rtemp1)
                                                {
                                                    if (x.cur_qr == v.cur_qr) start = true;
                                                    else if (start)
                                                    {
                                                        //if (v.cur_rack_degree != x.cur_rack_degree)
                                                        if (x.rack_turn_arrive)
                                                        {
                                                            rack_rotate_after_cp = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                                if (rack_rotate_after_cp) continue;
                                            }

                                            if (agv.rack != null)
                                            {
                                                agv.rack.degree = rtemp1.Last().cur_rack_degree;
                                            }

                                            var rtemp2 = controller.routeManager.GetMoveConditions_ex(agv, vvv.Key, v.cur_qr, "");

                                            if (rtemp1.Count == rtemp2.Count)
                                            {
                                                //if (0 == rtemp1.Where(e => e.rack_turn_arrive).Count() && 0 == rtemp2.Where(e => e.rack_turn_arrive).Count())
                                                {
                                                    //同じルートで行き来可能
                                                    if (!escape_qrs.ContainsKey(v)) escape_qrs[v] = new Dictionary<FloorQR, double>();

                                                    if (!escape_qrs[v].Contains(vvv)) escape_qrs[v][vvv.Key] = vvv.Value;
                                                }
                                            }
                                        }
                                    }

                                    if (agv.rack != null) agv.rack.degree = rack_deg_org;
                                    agv.degree = agv_deg_org;
                                }
                            }
                        }

                        //目的地を回避不可チェックポイントとする
                        escape_qrs[route.Last()] = new Dictionary<FloorQR, double>();
                    }
                    //catch
                    //{
                    //    continue;
                    //}
                }
                while (false);

                return escape_qrs;
            }

            /// <summary>
            /// 一定距離以上離れたポイントを取得
            /// </summary>
            /// <param name="from"></param>
            /// <param name="distance"></param>
            /// <param name="except_route"></param>
            /// <returns></returns>
            protected Dictionary<FloorQR, double> DistancePoint(FloorQR from, double distance, RouteConditionList except_route, double way_degree = double.NaN, List<FloorQR> checked_qrs = null)
            {
                if (except_route == null) except_route = new RouteConditionList();
                if (checked_qrs == null) checked_qrs = new List<FloorQR>();
                Dictionary<FloorQR, double> ret = new Dictionary<FloorQR, double>();

                checked_qrs.Add(from);

                double way_deg;

                foreach (var v in from.next_way)
                {
                    way_deg = way_degree;

                    //斜めは探査挙外
                    if (v.Value % 90 != 0) continue;

                    //次ポイントが除外ルート上の場合は除外
                    if (0 < except_route.Where(e => e.cur_qr == v.Key).Count()) continue;

                    //チェック済みQRは除外
                    if (checked_qrs.Contains(v.Key)) continue;
                    checked_qrs.Add(v.Key);

                    if (double.IsNaN(way_deg))
                    {
                        //探査初回の向きを保持
                        way_deg = v.Value;
                    }
                    else if (way_deg != v.Value)
                    {
                        //向きが変わったら探査しない
                        continue;
                    }

                    double v_distance = from.Distance(v.Key);
                    if (distance < v_distance)
                    {
                        //所定距離に到達
                        ret[v.Key] = way_deg;
                    }
                    else
                    {
                        //所定距離に未到達なので、さらに次のポイントを検索
                        double next_distance = distance - v_distance;

                        foreach (var kv in DistancePoint(v.Key, next_distance, except_route, way_deg, checked_qrs))
                        {
                            ret[kv.Key] = kv.Value;
                        }
                    }
                }

                return ret;
            }

            protected FloorQR AngleDistancePoint_or_DeadEnd(FloorQR from, double distance, double degree)
            {
                FloorQR ret = from;
                double run_distance = 0;
                while (true)
                {
                    var qq2 = ret.next_way.Where(e => e.Value == degree).FirstOrDefault();
                    if (qq2.Key == null) break;
                    if (qq2.Key.station_id.Trim() != "") break;

                    run_distance += ret.Distance(qq2.Key);
                    ret = qq2.Key;

                    if (distance < run_distance) break;
                }

                return ret;
            }

            /// <summary>
            /// 充電場所までのルート取得
            /// </summary>
            /// <returns>充電場所までのルート</returns>
            protected RouteConditionList get_charge_route()
            {
                BL_Stopwatch sw = new BL_Stopwatch();
                sw.Restart();


                RouteConditionList cons = new RouteConditionList();

                List<FloorAGV> free_agvs = new List<FloorAGV>();

                //充電しようとしている他のＡＧＶの充電場所を列挙
                List<FloorQR> reserve_charge_qr = new List<FloorQR>();
                foreach (var v in controller.agvs)
                {
                    if (v == this.agv)
                    {
                        free_agvs.Add(v);
                        continue;
                    }

                    var charge_target = v.conditions.Where(e => e.wait_charge_trg).ToList();
                    if (0 < charge_target.Count())
                    {
                        foreach (var vv in charge_target)
                        {
                            reserve_charge_qr.Add(vv.cur_qr);
                        }
                    }
                    else
                    {
                        free_agvs.Add(v);
                    }
                }

                //空いている充電場所を探す
                var free_charge_qr = agv.floor.mapeditor.list.Where(e => e.direction_charge != FloorQR.enDirection.NONE
                                                                     && (e.on_agv == null || !e.on_agv.agvRunner.communicator.GetState.sta_charge))
                                                             .Except(reserve_charge_qr)
                                                             .OrderBy(e => controller.routeManager.CalcRouteConditionCost(
                                                                           controller.routeManager.GetMoveConditions_ex(agv, e, ""))).ToList();

                if (0 < free_charge_qr.Count())
                {
                    if (agv.req != null && agv.req.rack_action == "C")
                    {
                        cons = controller.routeManager.GetMoveConditions_ex(agv, free_charge_qr.First(), "");

                        if (0 < cons.Count)
                        {
                            Log(step + ",充電動作指示開始[" + cons.Last().ToString() + "]");

                            cons.Last().wait_charge_trg = true;
                        }
                    }
                    else
                    {
                        var charge_agvs = free_agvs.Where(e => e.agvRunner != null
                                                               && e.rack == null
                                                               && !e.working
                                                               && e.agvRunner != null
                                                               && e.agvRunner.communicator != null
                                                               && e.agvRunner.communicator.Alive
                                                               && e.agvRunner.communicator.GetState != null
                                                               //&& e.agvRunner.communicator.GetState.bat <= manager.BATTERY_LOW
                                                               ).OrderBy(e => e.agvRunner.communicator.GetState.bat).ToList();



                        if (0 < charge_agvs.Count())
                        {
                            if (charge_agvs.First() == agv)
                            {
                                cons = controller.routeManager.GetMoveConditions_ex(agv, free_charge_qr.First(), "");

                                if (0 < cons.Count)
                                {
                                    Log(step + ",充電動作指示開始[" + cons.Last().ToString() + "]");

                                    cons.Last().wait_charge_trg = true;
                                }
                            }
                        }
                    }
                }
                //else
                //{
                //    free_charge_qr = agv.floor.mapeditor.list.Where(e => e.direction_charge != FloorQR.enDirection.NONE)
                //                                             .OrderBy(e => controller.routeManager.CalcRouteConditionCost(
                //                                                           controller.routeManager.GetMoveConditions_ex(agv, e, "")));

                //    if (0 < free_charge_qr.Count())
                //    {
                //        var charge_agvs = free_agvs.Where(e => e.agvRunner.communicator.GetState.bat <= manager.BATTERY_LOW && !e.agvRunner.communicator.GetState.sta_charge)
                //                 .OrderBy(e => e.agvRunner.communicator.GetState.bat);

                //        if (0 < charge_agvs.Count())
                //        {
                //            if (charge_agvs.First() == agv)
                //            {
                //                cons = controller.routeManager.GetMoveConditions_ex(agv, free_charge_qr.First(), "");

                //                if (0 < cons.Count)
                //                {
                //                    Log(step + ",充電動作指示開始(他AGVは充電中の場所)[" + cons.Last().ToString() + "]");

                //                    cons.Last().wait_charge_trg = true;
                //                }
                //            }
                //        }
                //    }
                //}


                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log(step + ",処理時間 get_charge_route:" + sw.ElapsedMilliseconds.ToString() + "ms");
                }

                return cons;
            }

            #region 情報取得

            List<FloorQR> fixed_except_qr = null;

            public FloorQR GetNonObstructQR(List<FloorQR> except_qr, List<FloorQR> except_route = null)
            {
                BL_Stopwatch sw = new BL_Stopwatch();
                sw.Restart();


                if (fixed_except_qr == null)
                {
                    fixed_except_qr = new List<FloorQR>();

                    #region オートレーター周辺へは回避不可
                    {
                        var autorator_qrs = controller.AllAutoratorQR;
                        foreach (var v in autorator_qrs)
                        {
                            fixed_except_qr.Add(v);

                            foreach (var vv in v.next_way.Keys)
                            {
                                if (!fixed_except_qr.Contains(vv))
                                {
                                    fixed_except_qr.Add(vv);
                                }
                            }

                            foreach (var vv in v.prev_way)
                            {
                                if (!fixed_except_qr.Contains(vv))
                                {
                                    fixed_except_qr.Add(vv);
                                }
                            }
                        }
                    }
                    #endregion

                    #region 棚配置のないステーションへは回避不可
                    {
                        var station_qrs = controller.AllStationQR.Where(e => e.rack == null).ToList();
                        foreach (var v in station_qrs)
                        {
                            fixed_except_qr.Add(v);

                            ////ステーションの近隣も回避不可
                            //foreach (var vv in v.next_way.Keys)
                            //{
                            //    if (!fixed_except_qr.Contains(vv))
                            //    {
                            //        fixed_except_qr.Add(vv);
                            //    }
                            //}

                            //foreach (var vv in v.prev_way)
                            //{
                            //    if (!fixed_except_qr.Contains(vv))
                            //    {
                            //        fixed_except_qr.Add(vv);
                            //    }
                            //}
                        }
                    }
                    #endregion

                    #region 充電場所へは回避不可
                    {
                        var charge_qrs = controller.AllQR.Where(e => e.direction_charge != FloorQR.enDirection.NONE);
                        foreach (var v in charge_qrs)
                        {
                            fixed_except_qr.Add(v);

                            ////充電場所の近隣も回避不可
                            //foreach (var vv in v.next_way.Keys)
                            //{
                            //    if (!fixed_except_qr.Contains(vv))
                            //    {
                            //        fixed_except_qr.Add(vv);
                            //    }
                            //}

                            //foreach (var vv in v.prev_way)
                            //{
                            //    if (!fixed_except_qr.Contains(vv))
                            //    {
                            //        fixed_except_qr.Add(vv);
                            //    }
                            //}
                        }
                    }
                    #endregion
                }

                //全AGVのルートを収集
                List<FloorQR> e_qr = new List<FloorQR>(fixed_except_qr);
                foreach (var v in except_qr)
                {
                    e_qr.Add(v);

                    //ルート周囲を除外してしまうと、2倍の距離を回避してしまう
                    //foreach (var vv in v.conflict_qr(agv))
                    //{
                    //    if (!e_qr.Contains(vv)) e_qr.Add(vv);
                    //}
                }

                if (!e_qr.Contains(agv.on_qr)) e_qr.Add(agv.on_qr);

                List<FloorQR> e_route = new List<FloorQR>(fixed_except_qr);
                if (except_route != null)
                {
                    foreach (var v in except_route)
                    {
                        if (!e_route.Contains(v)) e_route.Add(v);
                        if (!e_qr.Contains(v)) e_qr.Add(v);
                    }
                }

                #region 除外QRを除く同一フロア全QRから最小コストQRを取得

                Dictionary<FloorQR, RouteConditionList> diccost = new Dictionary<FloorQR, RouteConditionList>();

                List<FloorQR> open_qrs = new List<FloorQR>();
                List<FloorQR> openqrs_pre = new List<FloorQR>();
                

                open_qrs = controller.AllQR
                                .Except(e_qr)
                                .Where(e => e.floor == agv.floor)
                                .Where(e => 0 < e.prev_way.Count)
                                .Where(e => agv.Distance(e.Location) < 500).ToList();
                ;

                foreach (var v in open_qrs)
                {
                    RouteConditionList vv = null;

                    vv = controller.routeManager.GetMoveConditions_ex(agv, v, "", e_route, agv.rack != null);

                    if (0 < vv.Count)
                    {
                        diccost[v] = vv;
                    }
                }

                if (diccost.Count == 0)
                {
                    openqrs_pre.AddRange(open_qrs);
                    open_qrs.Clear();
                }

                if (open_qrs.Count == 0)
                {
                    if (except_route != null)
                    {
                        foreach (var v in except_route)
                        {
                            foreach (var vv in v.next_way.Keys)
                            {
                                if (e_qr.Contains(vv)) continue;
                                else
                                {
                                    if (!v.conflict_qr(agv).Contains(vv))
                                    {
                                        open_qrs.Add(vv);
                                    }
                                    else
                                    {
                                        foreach (var vvv in vv.next_way.Keys)
                                        {
                                            if (!v.conflict_qr(agv).Contains(vvv))
                                            {
                                                open_qrs.Add(vv);
                                            }
                                            else
                                            {

                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (open_qrs.Count == 0)
                {
                    open_qrs = controller.AllQR
                                    .Except(e_qr)
                                    .Except(openqrs_pre)
                                    .Where(e => e.floor == agv.floor)
                                    .Where(e => agv.Distance(e.Location) < 2000).ToList();
                    ;
                }



                foreach (var v in open_qrs)
                {
                    RouteConditionList vv = null;

                    vv = controller.routeManager.GetMoveConditions_ex(agv, v, "", e_route, agv.rack != null);

                    if (0 < vv.Count)
                    {
                        diccost[v] = vv;
                    }
                }

                if (diccost.Count == 0)
                {
                    foreach (var v in open_qrs)
                    {
                        RouteConditionList vv = null;

                        vv = controller.routeManager.GetMoveConditions_ex(agv, v, "", null, agv.rack != null);

                        if (0 < vv.Count)
                        {
                            diccost[v] = vv;
                        }
                    }
                }


                //bool crossload = false;
                //foreach (var v in open_qrs)
                //{
                //    RouteConditionList vv = null;

                //    if (!crossload)
                //    {
                //        vv = controller.routeManager.GetMoveConditions_ex(agv, v, "", e_route, agv.rack != null);

                //        if (0 == vv.Count)
                //        {
                //            crossload = true;
                //        }
                //    }

                //    if (crossload)
                //    {
                //        vv = controller.routeManager.GetMoveConditions_ex(agv, v, "", null, agv.rack != null);
                //    }

                //    if (0 < vv.Count)
                //    {
                //        diccost[v] = vv;
                //    }
                //}


                Dictionary<FloorQR, int> costorder = new Dictionary<FloorQR, int>();

                foreach (var v in diccost)
                {
                    costorder[v.Key] = controller.routeManager.CalcRouteConditionCost(v.Value);
                }

                #endregion

                var kvvalues = costorder.OrderBy(e => e.Value).FirstOrDefault();


                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log(step + ",処理時間 GetNonObstructQR:" + sw.ElapsedMilliseconds.ToString() + "ms");
                }

                return kvvalues.Key;
            }

            //public FloorQR GetNonObstructQR(List<FloorQR> except_qr, List<FloorQR> except_route = null)
            //{
            //    var open_qrs = GetNonObstructQRs(except_qr, except_route);

            //    if (0 < open_qrs.Count())
            //    {
            //        return open_qrs.First();
            //    }

            //    return null;
            //}

            public FloorAGV GetNearestAgv(FloorQR targetQR)
            {
                BL_Stopwatch sw = new BL_Stopwatch();
                sw.Restart();


                //待機中のAGV列挙
                var normalagv = controller.agvs.Where(e => e.rack == null
                                                        && e.agvRunner.can_order
                                                        && e.agvRunner.agv_request
                                                        && !e.agvRunner.communicator.GetState.sta_charge
                                                        && manager.BATTERY_NOP < e.agvRunner.communicator.GetState.bat);

                List<FloorQR> except_qrs = GetExceptQR(targetQR);

                var nearestagv = normalagv.OrderBy(e => controller.routeManager.CalcRouteConditionCost(
                                                            controller.routeManager.GetMoveConditions_ex(e, targetQR, "", except_qrs)));
                if (0 < nearestagv.Count())
                {
                    if (2000 < sw.ElapsedMilliseconds)
                    {
                        Log(step + ",処理時間 GetNearestAgv:" + sw.ElapsedMilliseconds.ToString() + "ms");
                    }

                    return nearestagv.First();
                }


                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log(step + ",処理時間 GetNearestAgv:" + sw.ElapsedMilliseconds.ToString() + "ms");
                }

                return null;
            }

            public List<FloorQR> GetExceptQR(FloorQR destination = null)
            {
                List<FloorQR> except_qrs = new List<FloorQR>();
                foreach (var v in controller.agvs)
                {
                    if (v == agv) continue;

                    if (v.rack != null && v.rack.station_working)
                    {
                        except_qrs.Add(v.on_qr);
                    }
                    else if (v.rack != null && v.rack.req != null)
                    {
                        foreach (var qr in v.conditions)
                        {
                            except_qrs.Add(qr.cur_qr);
                        }
                    }
                }

                except_qrs.Remove(destination);

                return except_qrs;
            }

            public FloorQR GetRackSettableNearestQR()
            {
                var settable_qrs = controller.AllQR.Where(e => e.floor.code == agv.floor.code && e.rack_setable && e.rack == null && 0 < e.prev_way.Count).ToList();

                if (0 == settable_qrs.Count())
                {
                    settable_qrs = controller.AllQR.Where(e => e.rack_setable && e.rack == null).ToList();
                }

                if (0 < settable_qrs.Count())
                {
                    //棚を置きに行こうとしているＡＧＶを列挙
                    var racksetting_agvs = controller.agvs.Where(e => e != agv
                                                                && e.rack != null
                                                                && 0 < e.conditions.Where(
                                                                    ee => (ee.rack_down_arrive || ee.rack_down_departure || ee.rack_down_departure_last)
                                                                            && ee.cur_qr.rack_setable).Count());
                    //他のＡＧＶが置こうとしているＱＲを除外
                    foreach (var v in racksetting_agvs)
                    {
                        var con = v.conditions.Where(e => e.rack_down_arrive || e.rack_down_departure || e.rack_down_departure_last);
                        foreach (var vv in con)
                        {
                            settable_qrs.Remove(vv.cur_qr);
                        }
                    }

                    var stay_rackagv = controller.agvs.Where(e => e != agv && e.crash_wait);
                    foreach (var v in stay_rackagv)
                    {
                        settable_qrs.Remove(v.on_qr);
                    }

                    var nearestQR = settable_qrs.OrderBy(e => controller.routeManager.CalcRouteConditionCost(
                                                              controller.routeManager.GetMoveConditions_ex(agv, e, "", GetExceptQR(e))));
                    if (0 < nearestQR.Count())
                    {
                        FloorQR return_qr = nearestQR.First();

                        //連続する棚設置可能場所の最奥を目的地とする
                        {
                            List<FloorQR> checked_qr = new List<FloorQR>();
                            checked_qr.Add(return_qr);
                            while (true)
                            {
                                var nextreturnqr = return_qr.next_way.Where(e => e.Key.rack_setable && e.Key.rack == null
                                                                            //&& !(e.Key.on_agv != null && e.Key.on_agv.rack != null)
                                                                            && !checked_qr.Contains(e.Key));
                                if (0 < nextreturnqr.Count() && settable_qrs.Contains(nextreturnqr.First().Key))
                                {
                                    return_qr = nextreturnqr.First().Key;
                                }
                                else break;

                                checked_qr.Add(return_qr);
                            }
                        }

                        return return_qr;
                    }
                }

                return null;
            }

            #endregion
        }
    }
}
