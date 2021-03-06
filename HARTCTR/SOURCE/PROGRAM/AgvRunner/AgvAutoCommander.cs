using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

using BelicsClass.Network;
using AgvController;
using BelicsClass.Common;
using System.Threading;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BelicsClass.File;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Diagnostics.Eventing.Reader;
using BelicsClass.ObjectSync;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        public class AgvAutoCommander : AgvRunner
        {
            //public class AgvMemory : BL_FaceMemorySync
            //{
            //    [BL_ObjectSync]
            //    public AgvCommunicator.State state = new AgvCommunicator.State();

            //    public RouteCondition[] consitions = new RouteCondition[100];

            //    public AgvMemory(string name)
            //    {
            //        for (int i = 0; i < consitions.Length; i++) consitions[i] = new RouteCondition();

            //        Initialize(name);
            //    }
            //}
            //AgvMemory mem = null;

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

            enAgvMoveKind agv_move_kind = enAgvMoveKind.AGV_STANBY;
            List<FloorQR> escape_from = new List<FloorQR>();
            FloorQR escapeQR = null;
            FloorQR destinationQR = null;
            RouteConditionList conditions_pre = new RouteConditionList();
            RouteConditionList next_cons = new RouteConditionList();
            //bool is_escape = false;
            RouteCondition crash_con = null;
            bool charge_start = false;
            int escape_point_count = -1;
            BL_Stopwatch charge_stop_timer = new BL_Stopwatch();
            BL_Stopwatch reportbattery_timer = new BL_Stopwatch();
            int bat_pre = -1;
            bool sta_charge = true;
            int error_code_pre = -1;
            string map_pre = "";
            int x_pre = -1;
            int y_pre = -1;
            BL_Stopwatch swStartWait = new BL_Stopwatch();
            BL_Stopwatch swReportAgv = new BL_Stopwatch();
            BL_HRTimer timerTimeout = new BL_HRTimer();

            public override string state_string => "[" + step + "] " + agv_move_kind.ToString();

            //private AgvTracker tracker = null;

            public AgvAutoCommander(AgvRunManager manager, FloorAGV agv, AgvConnector connector)
                : base(manager, agv)
            {
                this.connector = connector;
                controller = Program.controller;
            }

            #region 制御開始・終了

            public override void Start()
            {
                //mem = new AgvMemory(connector.ip + "_" + connector.remote_host.ToString());

                base.Start();
                agv_request = false;

                Log("THREAD START");

                ClearCondition();

                if (communicator != null) Stop();
                communicator = new AgvController.AgvCommunicator(agv.id, connector.ip, connector.remote_client, connector.remote_host, connector.recv, connector.send);
                communicator.ReceiveEvent += Communicator_ReceiveEvent;
                communicator.StartControl(5);

                swStartWait.Restart();
                agv.swStanbySeconds.Restart();
            }

            public override void Stop()
            {
                Log("THREAD STOP");

                if (communicator != null)
                {
                    communicator.ReceiveEvent -= Communicator_ReceiveEvent;
                    communicator.StopControl();
                    communicator = null;
                }

                agv.in_autorator = null;
                agv.out_autorator = null;

                base.Stop();

                //mem.Dispose();
            }

            public override void _DoControl(object message)
            {
                BL_Stopwatch sw = new BL_Stopwatch();
                sw.Restart();

                state_received = false;
                state_changed = false;

                if (0 < queState.Count)
                {
                    state_received = true;

                    state_control();

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

                if (!swStartWait.IsRunning || 3000 <= swStartWait.ElapsedMilliseconds)
                {
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
                }

                order_control();

                if (agv.communicating && 3000 < swReportAgv.ElapsedMilliseconds)
                {
                    agv.communicating = false;
                }

                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log("処理時間 _DoControl:" + sw.ElapsedMilliseconds.ToString() + "ms");
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

            public virtual void ClearCondition()
            {
                if (0 < agv.conditions.Count)
                {
                    //Log("ルート解放2");
                    agv.conditions[0].UnlockWay(true);
                    agv.conditions.Clear();
                }
                else
                {
                    agv.Unlock();
                }

                cur_con = null;
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

                List<FloorQR> open_qrs = new List<FloorQR>();

                open_qrs = controller.AllQR
                                .Except(e_qr)
                                .Where(e => e.floor == agv.floor)
                                .Where(e => 0 < e.prev_way.Count)
                                .Where(e => agv.Distance(e.Location) < 500).ToList();
                ;

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
                                    .Where(e => e.floor == agv.floor)
                                    .Where(e => agv.Distance(e.Location) < 2000).ToList();
                    ;
                }

                Dictionary<FloorQR, RouteConditionList> diccost = new Dictionary<FloorQR, RouteConditionList>();


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
                    Log("処理時間 GetNonObstructQR:" + sw.ElapsedMilliseconds.ToString() + "ms");
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
                        Log("処理時間 GetNearestAgv:" + sw.ElapsedMilliseconds.ToString() + "ms");
                    }

                    return nearestagv.First();
                }


                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log("処理時間 GetNearestAgv:" + sw.ElapsedMilliseconds.ToString() + "ms");
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


            #region AGV状態受信・応答指示送信

            /// <summary>
            /// AGVからの報告を内部変数へ展開
            /// </summary>
            /// <returns></returns>
            protected virtual bool state_control()
            {
                BL_Stopwatch sw = new BL_Stopwatch();
                sw.Restart();


                bool received = false;
                //bool is_changed = false;


                lock (queState)
                {
                    while (0 < queState.Count)
                    {
                        AgvCommunicator.State sta_new = queState.Dequeue();

                        if (sta.IsChanged(sta_new))
                        {
                            state_changed = true;

                            //if (controller.EventPaint != null) controller.EventPaint();
                            //is_changed = true;
                        }
                        

                        sta.SetBytes(sta_new.GetBytes());

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
                                        lock (vv.lock_agv)
                                        {
                                            if (vv.lock_agv.Contains(agv)) vv.lock_agv.Remove(agv);
                                            vv.lock_agv.Insert(0, agv);
                                        }
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

                        if (sta.map.Trim() == "")
                        {
                            if (agv.on_qr.autorator_info != null)
                            {
                                int floorno = 0; int.TryParse(agv.on_qr.autorator_info.Status.stop_floor, out floorno);

                                FloorQR qr = agv.on_qr.autorator_info.LogicQR(floorno);
                                if (qr != null)
                                {
                                    sta.map = qr.floor.code;
                                    sta.x = qr.x;
                                    sta.y = qr.y;
                                }
                            }
                            else if (cur_con != null && cur_con.next_condition.next_condition != null
                                && cur_con.next_condition.next_condition.Location == sta.Location)
                            {
                                sta.map = cur_con.next_condition.cur_qr.floor.code;
                                sta.x = cur_con.next_condition.cur_qr.x;
                                sta.y = cur_con.next_condition.cur_qr.y;
                            }
                            else if (cur_con != null && cur_con.next_condition != null)
                            {
                                sta.map = cur_con.next_condition.cur_qr.floor.code;
                            }
                            else
                            {
                                sta.map = agv.on_qr.floor.code;
                            }
                        }

                        //if (is_changed)
                        //{
                        //    mem.state.SetBytes(sta.GetBytes());
                        //    mem.WriteMemory();
                        //}

                        if (!sta.sta_runmode)
                        {
                            agv.Unreserve(true);
                        }

                        #region AGV状態遷移

                        if (sta.cmd == (ushort)AgvCommunicator.State.CMD.STATE ||
                            sta.cmd == (ushort)AgvCommunicator.State.CMD.REQUEST)
                        {
                            if (Location != sta.Location || agv.floor.code != sta.map)
                            {
                                Log("座標変化[" + sta.map + sta.Location.ToString() + "]");


                                //@@@走行距離を加算
                                //@@@走行ポイント数を加算
                                double distance = agv.Distance(Location, sta.Location);

                                FloorQR prev_qr = agv.on_qr;

                                SetLocation(sta.map, sta.Location, sta.sta_rack ? sta.rack_deg : 9999);

                                //過去にいた場所は予約解除
                                ////チェックポイントから移動したらチェックポイントは予約解除
                                //if (prev_qr.escape_to != null)
                                {
                                    prev_qr.Unreserve(agv);
                                }

                                if (agv.in_autorator == null && agv.out_autorator != null)
                                {
                                    if (140 <= agv.Distance(agv.out_autorator.Location))
                                    {
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
                                        #region オートレーターに搬入完了

                                        if (agv.on_qr.autorator_info.CompleteEntry(agv))
                                        {
                                            agv.in_autorator = null;
                                        }

                                        #endregion
                                    }
                                }
                            }

                            //if (sta.sta_runmode)
                            {
                                if (sta.sta_rack)
                                {
                                    if (agv.rack == null)
                                    {
                                        //@@@棚上昇動作回数を加算

                                        Log("棚上昇[" + sta.rack_no + "]");

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
                                                Log("棚存在しない→生成");

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
                                            //if (agv.rack.degree != sta.rack_deg)
                                            //{
                                            //    Log("棚角度変化[" + sta.rack_deg + "]");
                                            //}
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
                                            Log("棚返却完了[" + agv.rack.req_return.ToString() + "]");

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
                                        //        //Log("棚角度変化[" + sta.rack_deg + "]");

                                        //        agv.rack.degree = sta.rack_deg;
                                        //    }
                                        //}

                                        Log("棚下降[" + agv.rack.ToString() + "]");

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

                            if (Degree != sta.deg)
                            {
                                //@@@AGV旋回角度を加算
                                int degree = Math.Abs(Degree - sta.deg);



                                Log("AGV角度変化[" + sta.deg + "]");

                                Degree = sta.deg;
                            }

                            if (mapeditor.Exist(sta.Location) != null)
                            {
                                List<RouteCondition> unlock_cons = new List<RouteCondition>();

                                for (RouteCondition con = cur_con; con != null; con = con.next_condition)
                                {
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
                                            foreach (var v in unlock_cons)
                                            {
                                                v.UnlockWay(false);
                                            }
                                            unlock_cons.Clear();

                                            if (cur_con != null) cur_con.UnlockWay(false);

                                            cur_con = con;

                                            if (cur_con != null)
                                            {
                                                //衝突検知
                                                var crash_con = cur_con.LockWay();
                                                if (crash_con != null)
                                                {
                                                    Log("衝突検知1");
                                                    agv.crash_wait = true;

                                                    next_orders.Add(agv.RouteCancelOrder());
                                                }
                                                else
                                                {
                                                    agv.crash_wait = false;
                                                }
                                            }

                                            agv.conditions.Remove(con);
                                        }

                                        break;
                                    }
                                    else
                                    {
                                        unlock_cons.Add(con);

                                        cur_con = con;
                                        agv.conditions.Remove(con);
                                    }
                                }

                                if (agv.conditions.Count == 0)
                                {
                                    ClearCondition();
                                }
                            }
                            else
                            {
                                Log("座標データ不明," + sta.Location.ToString());
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

                                Log("AGV→状態報告");
                            }

                            agv_request = false;
                            agv_state = true;
                        }
                        else if (sta.cmd == (ushort)AgvCommunicator.State.CMD.REQUEST)
                        {
                            if (!agv_request)
                            {
                                agv.swStanbySeconds.Restart();

                                Log("AGV→指示要求");

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
                                Log("AGV→動作指示応答");
                            }
                            agv_order_response = true;
                        }

                        #endregion
                    }
                }


                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log("処理時間 state_control:" + sw.ElapsedMilliseconds.ToString() + "ms");
                }

                return received;
            }

            /// <summary>
            /// AGVへのルートキャンセル・ACK返送・動作指示を送信
            /// </summary>
            protected virtual void order_control()
            {
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

                                Log(v.ToString());
                                //communicator.Log("STEP=" + step.ToString("000") + "," + v.ToString());

                                this.ord.SetBytes(v.GetBytes());
                            }

                            Log("動作指示完了[" + list.Count() + "件]");

                            foreach (var v in list) next_orders.Remove(v);
                        }
                    }
                }

                next_orders.Clear();

                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log("処理時間 order_control:" + sw.ElapsedMilliseconds.ToString() + "ms");
                }
            }

            #endregion

            #region AGV制御

            protected virtual void agv_control()
            {
                int step_save = step;
                BL_Stopwatch sw = new BL_Stopwatch();
                sw.Restart();


                switch (step)
                {
                    case 0:
                        if (agv_request && state_received)
                        {
                            #region 動作選択

                            //存在しないフロアでは制御停止
                            if (!controller.map.ContainsKey(sta.map))
                            {
                                break;
                            }

                            if (0 < agv.conditions.Count)
                            {
                                #region ルート生成済み

                                next_cons.AddRange(agv.conditions);

                                #endregion

                                step = 20;
                                break;
                            }

                            if (sta.sta_charge)
                            {
                                #region 充電中の動作選択（REQUEST状態）

                                //充電が必要なAGVが存在する
                                var charge_agvs = controller.agvs.Where(e => e != agv && e.agvRunner.agv_request && e.agvRunner.communicator.GetState.bat <= manager.BATTERY_LOW)
                                                                 .OrderBy(e => e.agvRunner.communicator.GetState.bat);
                                if (manager.BATTERY_MID <= sta.bat && 0 < charge_agvs.Count())
                                {
                                    agv_move_kind = enAgvMoveKind.AGV_STANBY;
                                    Log("充電停止指示開始(譲り合い)");

                                    next_orders.Add(agv.ChargeStopOrder());
                                    step = 5;
                                    charge_stop_timer.Restart();
                                }
                                else if (manager.BATTERY_FUL <= sta.bat)
                                {
                                    agv_move_kind = enAgvMoveKind.AGV_STANBY;
                                    Log("充電停止指示開始");

                                    next_orders.Add(agv.ChargeStopOrder());
                                    step = 5;
                                    charge_stop_timer.Restart();
                                }

                                #endregion

                                break;
                            }

                            else if (agv.rack == null &&
                                     (sta.bat <= manager.BATTERY_LOW || (agv.req != null && agv.req.rack_action.Trim() == "C"))
                                     )
                            {
                                #region 棚を持っておらず充電が必要

                                next_cons = get_charge_route();

                                if (0 < next_cons.Count)
                                {
                                    agv_move_kind = enAgvMoveKind.AGV_CHARGE;
                                    step = 20;
                                    break;
                                }

                                #endregion
                            }

                            else if (agv.req != null && agv.req.rack_action.Trim() == "c")
                            {
                                #region 上位要求による充電終了

                                agv.req.result = "OK";
                                manager.OrderSend(agv.req);

                                agv.req = null;

                                #endregion
                            }

                            else if (0 < requests.Count && charge_start && manager.CHARGE_ABORT_LEVEL <= sta.bat)
                            {
                                #region 動作可能な充電レベルでの要求受付準備

                                charge_start = false;

                                #endregion
                            }

                            else if (!agv.working && (charge_start || (/*agv.on_qr != null && agv.on_qr.station_id != "" &&*/
                                manager.WAITTIME <= agv.swStanbySeconds.ElapsedMilliseconds / 1000 &&
                                sta.bat <= manager.CHARGE_START_LEVEL && sta.bat <= manager.BATTERY_FUL)))
                            {
                                #region 充電動作の開始・充電ルート算出

                                charge_start = true;
                                next_cons = get_charge_route();

                                if (0 < next_cons.Count)
                                {
                                    agv_move_kind = enAgvMoveKind.AGV_CHARGE;
                                    step = 20;
                                    break;
                                }

                                #endregion
                            }

                            //is_escape = false;

                            if (escapeQR != null)
                            {
                                next_cons = controller.routeManager.GetMoveConditions_ex(agv, escapeQR, "");
                            }

                            else if (0 < escape_from.Count)
                            {
                                #region 回避動作中

                                //回避中で回避先に到着していない
                                agv_move_kind = enAgvMoveKind.AGV_ESCAPE;

                                //var con = escape_from.Where(e => e.Location == agv.Location).FirstOrDefault();

                                List<FloorQR> e_route = new List<FloorQR>();
                                foreach (var c in escape_from)
                                {
                                    if (c.Location == agv.Location) break;
                                    if (!e_route.Contains(c)) e_route.Insert(0, c);
                                }

                                escapeQR = GetNonObstructQR(escape_from, e_route);

                                if (escapeQR != null)
                                {
                                    next_cons = controller.routeManager.GetMoveConditions_ex(agv, escapeQR, "");
                                }

                                escape_from.Clear();

                                //is_escape = true;

                                #endregion
                            }

                            else if (agv.rack != null && agv.rack.req != null)
                            {
                                #region 棚搬送動作の開始・搬送ルート算出

                                //予約している要求のある棚を持っている
                                agv_move_kind = enAgvMoveKind.RACK_REQUEST;

                                if (!agv.rack.station_working)
                                {
                                    //ステーション作業中ではない棚を持っている
                                    FloorQR stationqr = controller.GetStationQR(agv.placed_rack.req.station);
                                    if (stationqr != null)
                                    {
                                        next_cons = controller.routeManager.GetMoveConditions_ex(agv, stationqr, agv.placed_rack.req.rackface, null, true);
                                        if (next_cons.Count() == 0)
                                        {
                                            next_cons = controller.routeManager.GetMoveConditions_ex(agv, stationqr, agv.placed_rack.req.rackface, null, false);
                                        }
                                    }
                                }

                                #endregion
                            }

                            else if (agv.rack != null && agv.rack.req_return != null)
                            {
                                #region 棚返却動作の開始・返却ルート算出

                                //if (agv.on_qr.autorator_id != "" && out_autorator_qr == null)
                                //{
                                //    out_autorator_qr = agv.on_qr;
                                //}

                                //予約している要求のある返却が必要な棚を持っている
                                agv_move_kind = enAgvMoveKind.RACK_RETURN;

                                var returnqr = GetRackSettableNearestQR();
                                if (returnqr != null)
                                {
                                    var working_agv = controller.agvs.Where(e => e.rack != null && e.rack.req != null && e.rack.req.station == agv.rack.req_return.station &&
                                                                                 0 < e.conditions.Count &&
                                                                                 0 < e.conditions[0].StraightRoute.Where(ee => ee.cur_qr.station_id == agv.rack.req_return.station).Count());

                                    List<FloorQR> exeptqr = new List<FloorQR>();
                                    foreach (var v in working_agv)
                                    {
                                        exeptqr.Add(v.on_qr);
                                    }

                                    next_cons = controller.routeManager.GetMoveConditions_ex(agv, returnqr, "", exeptqr, true);
                                    if (next_cons.Count() == 0)
                                    {
                                        next_cons = controller.routeManager.GetMoveConditions_ex(agv, returnqr, "", exeptqr, false);
                                    }

                                    if (0 < next_cons.Count())
                                    {
                                        next_cons.Last().rack_down_arrive = true;
                                    }
                                }

                                #endregion
                            }

                            else if (agv.rack != null && agv.rack.obstruct_route != null)
                            {
                                #region 持っている棚の退避動作の開始・退避ルート算出

                                //予約している待避が必要な棚を持っている
                                agv_move_kind = enAgvMoveKind.RACK_ESCAPE;

                                var escapeqr = GetNonObstructQR(agv.rack.obstruct_route.ToQrList());
                                if (escapeqr != null)
                                {
                                    next_cons = controller.routeManager.GetMoveConditions_ex(agv, escapeqr, "");
                                }

                                #endregion
                            }

                            else if (agv.rack != null && !agv.prev_agvreq)
                            {
                                #region 要求の無い棚を持っている場合、返却する

                                //何も無い棚を持っている
                                //agv_move_kind = enAgvMoveKind.AGV_WAIT;

                                agv.rack.req_return = new AgvOrderCommunicator.RequestDelivery(AgvOrderCommunicator.enREQ.QRS, 0, AgvOrderCommunicator.enRESULT.RQ, "", agv.rack.rack_no, "");

                                #endregion
                            }

                            else if (agv.req != null)
                            {
                                #region 上位からの移動要求動作の開始

                                agv_move_kind = enAgvMoveKind.RACK_REQUEST;

                                FloorQR stationqr = controller.GetStationQR(agv.req.station.Trim());
                                if (stationqr != null)
                                {
                                    next_cons = controller.routeManager.GetMoveConditions_ex(agv, stationqr, agv.req.rack_face);

                                    if (0 < next_cons.Count())
                                    {
                                        if (agv.req.rack_action.Trim() == "1")
                                        {
                                            next_cons.Last().rack_up_arrive = true;
                                        }
                                        else if (agv.req.rack_action.Trim() == "2")
                                        {
                                            next_cons.Last().rack_down_arrive = true;
                                        }
                                    }
                                    else
                                    {
                                        //ルート算出不可
                                        step = 900;
                                        break;
                                    }

                                    //ルート上のオートレーターが使用できない場合、ルートを消去
                                    var autoratorcon = next_cons.Where(e => e.cur_qr.autorator_id != "").FirstOrDefault();
                                    if (autoratorcon != null && autoratorcon.cur_qr.autorator_info != null)
                                    {
                                        if (autoratorcon.cur_qr.autorator_info.Status.TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.ERROR
                                         || autoratorcon.cur_qr.autorator_info.Status.TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.MANUAL
                                         || autoratorcon.cur_qr.autorator_info.Status.TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.OFFLINE
                                         || autoratorcon.cur_qr.autorator_info.Status.TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.AUTORUN_OFF
                                          )
                                        {
                                            //next_cons.Clear();

                                            step = 910;
                                            break;
                                        }
                                    }
                                }

                                #endregion
                            }

                            else
                            {
                                #region その他の動作

                                FloorQR reserved_rack_qr = controller.AllRackQR.Where(e => e.rack != null && e.rack.loadreserve_agv == agv).FirstOrDefault();
                                if (reserved_rack_qr != null)
                                {
                                    #region 予約している棚が存在する

                                    if (reserved_rack_qr.rack.req != null)
                                    {
                                        #region 予約している要求のある棚までの移動ルートを算出

                                        agv_move_kind = enAgvMoveKind.RACK_REQUEST;

                                        FloorQR stationqr = controller.GetStationQR(reserved_rack_qr.rack.req.station);
                                        if (stationqr != null)
                                        {
                                            var cons1 = controller.routeManager.GetMoveConditions_ex(agv, reserved_rack_qr, "");
                                            if (0 < cons1.Count())
                                            {
                                                cons1.Last().rack_up_arrive = true;

                                                PointF location_save = Location;
                                                string floor_save = agv.floor.code;

                                                SetLocation(cons1.Last().cur_qr.floor.code, cons1.Last().Location);
                                                agv.rack = reserved_rack_qr.rack;
                                                reserved_rack_qr.rack = null;

                                                var cons2 = controller.routeManager.GetMoveConditions_ex(agv, stationqr, agv.rack.req.rackface, null, false);
                                                if (0 < cons2.Count())
                                                {
                                                    next_cons = AgvRouteManager.ConnectConditions(cons1, cons2);
                                                }

                                                SetLocation(floor_save, location_save);
                                                reserved_rack_qr.rack = agv.rack;
                                                agv.rack = null;

                                                var crashwait_agvs = next_cons.Where(e => e.cur_qr.on_agv != null && e.cur_qr.on_agv != agv &&
                                                                                          e.cur_qr.on_agv.crash_wait);
                                                if (0 < crashwait_agvs.Count())
                                                {
                                                    var except_qr = crashwait_agvs.First().cur_qr;
                                                    List<FloorQR> except_qrs = new List<FloorQR>();
                                                    except_qrs.Add(except_qr);

                                                    cons1 = controller.routeManager.GetMoveConditions_ex(agv, reserved_rack_qr, "", except_qrs);
                                                    if (0 < cons1.Count())
                                                    {
                                                        cons1.Last().rack_up_arrive = true;

                                                        location_save = Location;
                                                        floor_save = agv.floor.code;

                                                        SetLocation(cons1.Last().cur_qr.floor.code, cons1.Last().Location);
                                                        agv.rack = reserved_rack_qr.rack;
                                                        reserved_rack_qr.rack = null;

                                                        cons2 = controller.routeManager.GetMoveConditions_ex(agv, stationqr, agv.rack.req.rackface, except_qrs, false);
                                                        if (0 < cons2.Count())
                                                        {
                                                            next_cons = AgvRouteManager.ConnectConditions(cons1, cons2);
                                                        }

                                                        SetLocation(floor_save, location_save);
                                                        reserved_rack_qr.rack = agv.rack;
                                                        agv.rack = null;
                                                    }
                                                }
                                            }
                                        }

                                        #endregion
                                    }

                                    else if (reserved_rack_qr.rack.req_return != null)
                                    {
                                        #region 予約している返却する棚までの移動ルートを算出

                                        agv_move_kind = enAgvMoveKind.RACK_RETURN;

                                        var cons1 = controller.routeManager.GetMoveConditions_ex(agv, reserved_rack_qr, "");
                                        if (0 < cons1.Count)
                                        {
                                            cons1.Last().rack_up_arrive = true;

                                            PointF location_save = Location;
                                            string floor_save = agv.floor.code;

                                            SetLocation(cons1.Last().cur_qr.floor.code, cons1.Last().Location);
                                            agv.rack = reserved_rack_qr.rack;
                                            reserved_rack_qr.rack = null;

                                            FloorQR freerackqr = GetRackSettableNearestQR();
                                            if (freerackqr != null)
                                            {
                                                var cons2 = controller.routeManager.GetMoveConditions_ex(agv, freerackqr, "");
                                                if (0 < cons2.Count())
                                                {
                                                    next_cons = AgvRouteManager.ConnectConditions(cons1, cons2);
                                                }
                                            }

                                            SetLocation(floor_save, location_save);
                                            reserved_rack_qr.rack = agv.rack;
                                            agv.rack = null;
                                        }

                                        #endregion
                                    }

                                    else if (reserved_rack_qr.rack.obstruct_route != null)
                                    {
                                        #region 予約している待避が必要な棚までの移動ルートを算出

                                        agv_move_kind = enAgvMoveKind.RACK_ESCAPE;

                                        RouteConditionList cons1 = new RouteConditionList();
                                        if (reserved_rack_qr.rack.obstruct_route[0].owner_agv.rack != null)
                                        {
                                            cons1 = controller.routeManager.GetMoveConditions_ex(agv, reserved_rack_qr, "", new List<FloorQR>() { reserved_rack_qr.rack.obstruct_route[0].owner_agv.on_qr });
                                        }
                                        else
                                        {
                                            cons1 = controller.routeManager.GetMoveConditions_ex(agv, reserved_rack_qr, "");
                                        }

                                        if (0 < cons1.Count())
                                        {
                                            cons1.Last().rack_up_arrive = true;

                                            PointF location_save = Location;
                                            string floor_save = agv.floor.code;

                                            SetLocation(cons1.Last().cur_qr.floor.code, cons1.Last().Location);

                                            agv.rack = reserved_rack_qr.rack;
                                            reserved_rack_qr.rack = null;

                                            var escapeqr = GetNonObstructQR(agv.rack.obstruct_route.ToQrList());
                                            if (escapeqr != null)
                                            {
                                                var cons2 = controller.routeManager.GetMoveConditions_ex(agv, escapeqr, "");
                                                if (0 < cons2.Count())
                                                {
                                                    next_cons = AgvRouteManager.ConnectConditions(cons1, cons2);
                                                }
                                            }

                                            SetLocation(floor_save, location_save);
                                            reserved_rack_qr.rack = agv.rack;
                                            agv.rack = null;
                                        }

                                        #endregion
                                    }

                                    else
                                    {
                                        #region 予約している棚には要求がない

                                        reserved_rack_qr.rack.loadreserve_agv = null;

                                        #endregion
                                    }

                                    #endregion
                                }
                                else
                                {
                                    step = 10;
                                }

                                #endregion
                            }

                            if (0 < next_cons.Count())
                            {
                                step = 20;
                            }
                            else
                            {
                                //if (!is_escape)
                                //{
                                //    agv.Unreserve(true);
                                //}

                                ClearCondition();
                            }

                            #endregion
                        }
                        else if (sta.sta_charge && state_received)
                        {
                            #region 充電中の動作選択

                            //@@@2021/6/20 add
                            agv.request_moving = false;

                            bool requested = RequestMove();

                            //充電中

                            //if (agv.req != null && agv.req.rack_action.Trim() == "C")
                            //{
                            //    agv.req.result = "OK";
                            //    manager.OrderSend(agv.req);

                            //    agv.req = null;
                            //}

                            if (!requested)
                            {
                                //充電が必要なAGVが存在する
                                var charge_agvs = controller.agvs.Where(e => e != agv && e.agvRunner != null && e.agvRunner.agv_request && e.agvRunner.communicator.GetState.bat <= manager.BATTERY_LOW)
                                                                 .OrderBy(e => e.agvRunner.communicator.GetState.bat);
                                if (manager.BATTERY_MID <= sta.bat && 0 < charge_agvs.Count())
                                {
                                    agv_move_kind = enAgvMoveKind.AGV_STANBY;
                                    Log("充電停止指示開始(譲り合い)");

                                    next_orders.Add(agv.ChargeStopOrder());
                                    charge_start = false;

                                    if (agv.req != null && agv.req.rack_action == "C") agv.req = null;
                                    step = 5;
                                    charge_stop_timer.Restart();
                                }
                                else if (manager.BATTERY_FUL <= sta.bat ||
                                    (agv.req != null && agv.req.rack_action.Trim() == "c"))
                                {
                                    agv_move_kind = enAgvMoveKind.AGV_STANBY;
                                    Log("充電停止指示開始1");

                                    next_orders.Add(agv.ChargeStopOrder());
                                    charge_start = false;

                                    if (agv.req != null && agv.req.rack_action == "C") agv.req = null;
                                    step = 5;
                                    charge_stop_timer.Restart();
                                }
                                else if (0 < requests.Count && manager.CHARGE_ABORT_LEVEL <= sta.bat)
                                {
                                    agv_move_kind = enAgvMoveKind.AGV_STANBY;
                                    Log("充電停止指示開始2");

                                    next_orders.Add(agv.ChargeStopOrder());
                                    charge_start = false;

                                    if (agv.req != null && agv.req.rack_action == "C") agv.req = null;
                                    step = 5;
                                    charge_stop_timer.Restart();
                                }
                            }

                            #endregion
                        }
                        break;

                    case 5:
                        if (!sta.sta_charge || 60 * 1000 <= charge_stop_timer.ElapsedMilliseconds)
                        {
                            #region 充電停止完了 or タイムアウト

                            step = 0;

                            #endregion
                        }
                        break;

                    case 10:
                        {
                            #region 上位要求取りだし・待機中の回避ルート生成

                            agv.request_moving = false;
                            bool is_requested = false;

                            if (!is_requested)
                            {
                                var obstruct_rack_qrs = controller.AllRackQR.Where(e => e.rack != null && e.rack.loadreserve_agv == null && e.rack.obstruct_route != null);
                                if (0 < obstruct_rack_qrs.Count())
                                {
                                    //待避が必要な棚をどかす

                                    foreach (var v in obstruct_rack_qrs)
                                    {
                                        if (GetNearestAgv(v) == agv)
                                        {
                                            v.rack.loadreserve_agv = agv;
                                            is_requested = true;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (!is_requested)
                            {
                                is_requested = RequestRack();
                            }

                            if (!is_requested)
                            {
                                is_requested = RequestMove();
                            }

                            //if (!is_requested)
                            //{
                            //    if (agv.placed_rack != null && agv.placed_rack.station_working)
                            //    {
                            //        //棚を持ってステーション作業中の場合は回避行動しない
                            //    }
                            //    else if (agv.working)
                            //    {
                            //        //要求が仕掛中の場合は回避行動しない
                            //    }
                            //    else
                            //    {
                            //        #region 待機中の回避ルート生成

                            //        var conflict_route_agv = controller.agvs.Where(e => e != agv && 0 < e.routed_qr.Where(ee => ee.Location == agv.Location).Count()).ToList();
                            //        conflict_route_agv = conflict_route_agv.Concat(controller.agvs.Where(e => e != agv && 0 < e.locked_qr_list.Where(ee => ee.Location == agv.Location).Count())).ToList();

                            //        var q = agv.on_qr.conflict_qr(agv).Where(e => e.on_agv != null && e.on_agv != agv && !e.on_agvs.Contains(agv));
                            //        foreach (var v in q)
                            //        {
                            //            conflict_route_agv.Add(v.on_agv);
                            //        }

                            //        //if (conflict_route_agv.Count() == 0)
                            //        //{
                            //        //    conflict_route_agv = conflict_route_agv.Concat(controller.agvs.Where(e => e != agv && 0 < e.reserved_qr.Where(ee => ee.Location == agv.Location).Count()));
                            //        //}

                            //        if (0 < conflict_route_agv.Count())
                            //        {
                            //            //待避が必要

                            //            foreach (var v in conflict_route_agv)
                            //            {
                            //                escape_from.AddRange(v.on_qr.conflict_qr(v));
                            //                escape_from.AddRange(v.routed_qr);
                            //            }
                            //        }

                            //        #endregion
                            //    }
                            //}

                            step = 0;

                            #endregion
                        }
                        break;

                    case 20:
                        //if (state_changed)
                        {
                            #region ルートチェック・正規化

                            step = 30;

                            for (int i = 1; i < next_cons.Count; i++)
                            {
                                var cur = next_cons[i - 1];
                                if (cur.next_condition != next_cons[i])
                                {
                                    ClearCondition();
                                    next_cons.Clear();
                                    break;
                                }
                            }

                            if (0 == next_cons.Count)
                            {
                                step = 0;
                                break;
                            }


                            foreach (var v in next_cons)
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

                            RouteConditionList rc = new RouteConditionList(next_cons);
                            bool rackup = agv.rack != null;
                            foreach (var v in next_cons)
                            {
                                if (v.rack_down_arrive || v.rack_down_departure || v.rack_down_departure_last)
                                {
                                    rackup = false;
                                }

                                if (v.rack_up_arrive || v.rack_up_departure)
                                {
                                    rackup = true;
                                }

                                if (rackup)
                                {
                                    if (agv.req == null)
                                    {
                                        if (v.cur_qr.rack != null && v.cur_qr.rack.loadreserve_agv != agv)
                                        {
                                            Log("棚待避が必要1[" + v.cur_qr.ToString() + "]");

                                            //v.cur_qr.rack.obstruct_route = new RouteConditionList(rc);

                                            ClearCondition();
                                            next_cons.Clear();
                                            step = 0;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    rc.Remove(v);
                                }
                            }

                            if (step == 0) break;

                            destinationQR = next_cons.Last().cur_qr;

                            ClearCondition();
                            agv.conditions.AddRange(next_cons);

                            #endregion
                        }
                        break;

                    case 30:
                        if (agv.conditions.Count == 0)
                        {
                            step = 0;
                            break;
                        }
                        else
                        {
                            #region マップ上のチェックポイント存在チェック

                            if (agv.req != null)
                            {
                                if (escape_point_count < 0)
                                {
                                    escape_point_count = controller.AllQR.Where(e => e.escape_to != null).Count();
                                }
                            }

                            if (escape_point_count <= 0)
                            {
                                //マップ上にチェックポイントが無い場合は、ルート予約処理しない
                                step = 32;
                                break;
                            }

                            #endregion

                            #region 次以降のチェックポイント検出

                            RouteCondition checkpoint = null;
                            int spos = 1;

                            //退避ポイントにいる場合は、退避元の次のチェックポイント検出
                            if (agv.conditions[0].cur_qr.escape_from != null)
                            {
                                foreach (var v in agv.conditions)
                                {
                                    if (v.cur_qr == agv.conditions[0].cur_qr.escape_from) break;
                                    spos++;
                                }
                            }

                            checkpoint = agv.conditions.Skip(spos).Where(e => e.cur_qr.escape_to != null).FirstOrDefault();

                            if (checkpoint == null)
                            {
                                checkpoint = agv.conditions.Last();
                            }

                            #endregion

                            #region チェックポイントまでの動作指示に縮小

                            RouteConditionList temp = new RouteConditionList();
                            foreach (var v in agv.conditions)
                            {
                                temp.Add(v);
                                if (checkpoint == v) break;
                            }

                            if (0 < temp.Count) temp.Last().next_condition = null;

                            agv.conditions.Clear();
                            agv.conditions.AddRange(temp);

                            #endregion

                            #region 回避ポイントでの動作選択

                            if (agv.on_qr != null && agv.on_qr.escape_from != null)
                            {
                                //if (agv.on_qr.escape_from.OnOtherAgv(agv) != null || 0 < agv.on_qr.escape_from.reserve_agv.Where(e => e != agv).Count())
                                if (agv.on_qr.escape_from.OnOtherAgv(agv) != null || agv.on_qr.escape_from.IsReserveOtherAgv(agv))
                                {
                                    //回避元のチェックポイントに他AGVが存在 or 回避元のチェックポイントに他AGVが予約済み
                                    break;
                                }

                                else if (agv.on_qr.escape_from.IsLockOtherAgv(agv))
                                {
                                    break;
                                }

                                else if (0 < agv.conditions.Where(e => e.cur_qr.OnOtherAgv(agv) != null).Count())
                                {
                                    break;
                                }
                                else
                                {
                                }
                            }

                            if (agv.conditions.Last().cur_qr.escape_from != null)
                            {
                                //回避ポイントへ向かう場合は、そのまま移動
                                step = 32;
                                break;
                            }

                            #endregion

                            #region ルート予約

                            RouteCondition reserved_con = null;
                            RouteCondition reserved_con2 = null;
                            foreach (var v in agv.conditions)
                            {
                                if (v.cur_qr.Reserve(agv) != 0)
                                {
                                    if (reserved_con == null)
                                    {
                                        reserved_con = v;
                                    }
                                }

                                if (checkpoint == v)
                                {
                                    if (v.cur_qr.escape_to != null)
                                    {
                                        if (0 < v.cur_qr.next_way.Keys.Concat(v.cur_qr.prev_way).Where(e => e.OnOtherAgv_Real(agv) != null).Count())
                                        {
                                            reserved_con2 = v;
                                        }
                                    }

                                    break;
                                }
                            }

                            //if (reserved_con != null && reserved_con.cur_qr.OnOtherAgv(agv) != null)
                            //{
                            //    agv.Unreserve(false);

                            //    //再度予約
                            //    foreach (var v in agv.conditions)
                            //    {
                            //        if (!v.cur_qr.Reserve(agv))
                            //        {
                            //            if (reserved_con == null)
                            //            {
                            //                reserved_con = v;
                            //            }
                            //        }
                            //    }
                            //}

                            if (reserved_con2 != null)
                            {
                                //チェックポイント周辺に他AGVが存在
                                break;
                            }

                            if (reserved_con == null)
                            {
                                step = 32;
                            }
                            else if (escapeQR != null)
                            {
                                step = 32;
                            }
                            else
                            {
                                //var other_agv = agv.conditions.Where(e => e.cur_qr.reserve_agv.Contains(agv) && e.cur_qr.OnOtherAgv(agv) != null).ToList();
                                //if (0 == other_agv.Count)
                                //{
                                //    //予約済みルート上に他のAGVの予約がある（次のポイント以降）
                                //    if (0 < agv.conditions.Skip(1).Where(e => 0 < e.cur_qr.reserve_agv.Where(ee => ee != agv).Count()).Count())
                                //    {
                                //        //進路が被るのでそのまま待機
                                //        break;
                                //    }
                                //    else
                                //    {
                                //        //進路が被らないので走行
                                //        step = 32;
                                //        break;
                                //    }
                                //}
                                //else
                                //{
                                //    //自ルート上に他のAGVが存在する場合、予約解除
                                //    agv.Unreserve(true);

                                //    //bool found = false;
                                //    //foreach (var v in other_agv)
                                //    //{
                                //    //    for (RouteCondition other_con = v.cur_qr.on_agv.conditions[0]; other_con != null; other_con = other_con.next_condition)
                                //    //    {
                                //    //        if (other_con.cur_qr == agv.on_qr)
                                //    //        {
                                //    //            found = true;
                                //    //            break;
                                //    //        }
                                //    //    }

                                //    //    if (found) break;
                                //    //}

                                //    //自AGVが他AGVのルート上に存在するか取得
                                //    var crossed_agv = other_agv.Where(e => 0 < e.cur_qr.on_agv.conditions.Where(ee => ee.cur_qr == agv.on_qr).Count());
                                //    if (0 == crossed_agv.Count())
                                //    {
                                //        //進路が被らないのでそのまま待機
                                //        break;
                                //    }

                                //    #region 回避ポイントへのルート生成

                                //    next_cons.Clear();
                                //    if (agv.on_qr.escape_to != null)
                                //    {
                                //        agv.Unreserve(true);

                                //        next_cons = controller.routeManager.GetMoveConditions_ex(agv, agv.on_qr.escape_to, "");

                                //        ClearCondition();
                                //        agv.conditions.AddRange(next_cons);

                                //        step = 32;
                                //    }
                                //    else
                                //    {
                                //        step = 0;
                                //    }

                                //    #endregion
                                //}
                            }

                            #endregion
                        }
                        break;

                    case 32:
                        {
                            #region 進路排他

                            if (agv.conditions.Count == 0)
                            {
                                step = 0;
                            }
                            else
                            {
                                crash_con = agv.conditions[0].LockWay();
                                if (crash_con == null)
                                {
                                    step = 40;
                                }
                                else
                                {
                                    step = 60;
                                }
                            }

                            #endregion
                        }
                        break;

                    case 40:
                        {
                            #region オートレーター検出・要求

                            step = 50;

                            if (agv.on_qr.autorator_id != "" && agv.out_autorator != null)
                            {
                                //オートレーターからの搬出許可待ち
                                step = 600;
                            }
                            else
                            {
                                lock (controller)
                                {
                                    if (0 < agv.conditions.Count && agv.conditions[0].wait_autorator_in_trg)
                                    {
                                        #region オートレーター手前

                                        var autorator_qrs = agv.conditions[0].RoutingAutoratorQR();
                                        if (autorator_qrs != null)
                                        {
                                            if (0 == autorator_qrs.Count())
                                            {
                                                //オートレーターがロック済みで侵入不可
                                                //crash_con.UnlockWay(true);
                                                agv.crash_wait = true;
                                                Log("ルート消去6");
                                                ClearCondition();
                                                step = 0;
                                                break;
                                            }
                                            else
                                            {
                                                //controller.AllAutoratorQR.Where(e => e.autorator_id == autorator_qrs.First().autorator_id).ToList().ForEach(e => e.lock_agv = agv);
                                                //autorator_qrs.ForEach(e => e.lock_agv = agv);

                                                //int ar_count = -1;
                                                //for (var c = agv.conditions[0]; c != null; c = c.next_condition)
                                                //{
                                                //    if (0 <= ar_count) ar_count++;
                                                //    if (autorator_qrs.Contains(c.cur_qr)) ar_count = 0;

                                                //    if (2 < ar_count) break;

                                                //    if ((c.cur_qr.lock_agv != null && c.cur_qr.lock_agv != agv) || (c.cur_qr.on_agv != null && c.cur_qr.on_agv != agv))
                                                //    {
                                                //        //オートレーター周辺がロック済みで侵入不可
                                                //        //crash_con.UnlockWay(true);
                                                //        agv.crash_wait = true;
                                                //        Log("ルート消去6");
                                                //        ClearCondition();
                                                //        step = 0;
                                                //        break;
                                                //    }

                                                //    c.cur_qr.lock_agv = agv;
                                                //}

                                                //if (step == 0) break;

                                                var ar_in = this.FindAutoratorQR(agv.conditions[0]);
                                                var ar_out = ar_in.next_condition;
                                                if (ar_in != null && ar_out != null && ar_in.cur_qr.autorator_info != null && ar_out.cur_qr.autorator_info != null)
                                                {
                                                    if (ar_in.prev_condition != null && ar_out.cur_qr.autorator_info != null && !ar_in.cur_qr.autorator_info.IsRequested(agv))
                                                    {
                                                        int in_deg = (int)agv.Degree(ar_in.Location, ar_in.prev_condition.Location);
                                                        int out_deg = (int)agv.Degree(ar_out.Location, ar_out.next_condition.Location);

                                                        string infloor = ar_in.cur_qr.autorator_info.FloorNo(ar_in.cur_qr.floor.code);
                                                        string outfloor = ar_out.cur_qr.autorator_info.FloorNo(ar_out.cur_qr.floor.code);

                                                        string inside = ar_in.cur_qr.autorator_info.Side(in_deg);
                                                        string outside = ar_out.cur_qr.autorator_info.Side(out_deg);

                                                        ar_in.cur_qr.autorator_info.RequestEntry(agv, infloor, inside, outfloor, outside);

                                                        agv.in_autorator = ar_in.cur_qr;
                                                        agv.out_autorator = ar_out.cur_qr;
                                                        step = 500;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        step = 50;
                                                        break;
                                                    }
                                                }

                                                step = 32;
                                            }
                                        }

                                        #endregion
                                    }
                                    else if (check_wait_pre_crosspoint(agv.conditions[0]))
                                    {
                                        agv.crash_wait = true;
                                        Log("ルート消去7");
                                        ClearCondition();
                                        step = 0;
                                        break;
                                    }
                                }
                            }

                            #endregion
                        }
                        break;

                    case 50:
                        {
                            #region 動作指示生成

                            if (agv.conditions.Count == 0)
                            {
                                step = 0;
                                break;
                            }

                            step = 20;

                            //邪魔なAGVや棚がおらず、移動を開始できる
                            agv.crash_wait = false;

                            cur_con = agv.conditions[0];

                            if (agv.conditions[0].cur_qr.floor.code != sta.map && agv.conditions[0].next_condition.cur_qr.floor.code == sta.map)
                            {
                                agv.conditions.RemoveAt(0);
                                agv.conditions[0].prev_condition = null;
                            }

                            var orders = agv.GetOrders(0);
                            if (0 < orders.Length)
                            {
                                if (sta.x != sta._x || sta.y != sta._y)
                                {
                                    orders[0].dist = (UInt32)agv.Distance(sta.Location, orders[0].Location);
                                }

                                next_orders.AddRange(orders);

                                Log("動作指示開始[" + agv.conditions.Count + "件]->[" + agv.conditions.Last().cur_qr.ToString() + "]");
                                foreach (var v in orders)
                                {
                                    Log(v.ToString());
                                }

                                if (controller.EventPaint != null) controller.EventPaint();

                                timerTimeout.Restart();
                                step = 100;
                            }
                            else
                            {
                                if (next_cons.Count == 1)
                                {
                                    step = 200;
                                }
                                else
                                {
                                    Log("ルート消去8");
                                    ClearCondition();
                                    step = 0;
                                }
                            }

                            next_cons.Clear();

                            #endregion
                        }
                        break;

                    case 60:
                        {
                            #region 衝突回避の待機

                            step = 20;

                            Log("衝突検知2");
                            agv.crash_wait = true;

                            if (crash_con.cur_qr.autorator_id == "" && (crash_con.next_condition == null || crash_con.next_condition.cur_qr.autorator_id == ""))
                            {
                                //衝突検知場所はオートレーターではない

                                if (agv.rack == null)
                                {
                                    var obstruct_rack_qrs = controller.AllRackQR.Where(e => e.rack != null && e.rack.loadreserve_agv == null && e.rack.obstruct_route != null).ToList();
                                    if (0 < obstruct_rack_qrs.Count())
                                    {
                                        next_cons.Clear();
                                        step = 10;
                                        break;
                                    }
                                }

                                //if (agv.rack == null || (agv.rack.req == null && agv.rack.req_return == null && agv.rack.obstruct_route == null))
                                {
                                    //回避ルート再生成

                                    if (crash_con.cur_qr.on_agv != null)
                                    {
                                        var er = conflict_wait_or_escape_route(crash_con);
                                        if (er != null && 1 < er.Count)
                                        {
                                            Log("回避ルート生成1");

                                            agv.Unreserve(true);
                                            next_cons.Clear();
                                            next_cons.AddRange(er);
                                        }
                                    }
                                    else if (crash_con.cur_qr.IsLockOtherAgv(agv))
                                    {
                                        var er = conflict_wait_or_escape_route(crash_con);
                                        if (er != null && 1 < er.Count)
                                        {
                                            Log("回避ルート生成2");

                                            next_cons.Clear();
                                            next_cons.AddRange(er);
                                        }
                                    }
                                    else if (agv.rack != null && crash_con.cur_qr.rack != null)
                                    {
                                        Log("衝突検知3(棚衝突)");
                                        //棚で衝突検知
                                        next_cons.Clear();
                                        Log("ルート消去9");
                                        ClearCondition();
                                        step = 0;
                                    }
                                }

                                if (0 < next_cons.Count && next_cons.Last().cur_qr == agv.on_qr)
                                {
                                    //回避できない
                                    Log("回避不可");

                                    step = 0;
                                }
                            }
                            else if (agv.in_autorator == null && agv.out_autorator == null)
                            {
                                lock (crash_con.cur_qr.lock_agv)
                                {
                                    if (0 < crash_con.cur_qr.lock_agv.Count && crash_con.cur_qr.IsLockOtherAgv(agv))
                                    {
                                        var escape_qr = GetNonObstructQR(crash_con.cur_qr.lock_agv[0].routed_qr);

                                        var er = controller.routeManager.GetMoveConditions_ex(agv, escape_qr, "");
                                        if (er != null && 1 < er.Count)
                                        {
                                            Log("回避ルート生成3");

                                            next_cons.Clear();
                                            next_cons.AddRange(er);
                                        }
                                    }
                                    else
                                    {
                                        var er = conflict_wait_or_escape_route(crash_con);
                                        if (er != null && 1 < er.Count)
                                        {
                                            Log("回避ルート生成2");

                                            next_cons.Clear();
                                            next_cons.AddRange(er);
                                        }
                                    }
                                }
                            }
                            else
                            {
                            }

                            #endregion
                        }
                        break;

                    case 100:
                        if (state_received)
                        {
                            #region 動作指示応答確認

                            if (agv_order_response)
                            {
                                if (!agv_request || can_order)
                                {
                                    step = 200;
                                }
                            }

                            if (agv_request && 5000 < timerTimeout.ElapsedMilliseconds)
                            {
                                //指示応答タイムアウト
                                Log("指示応答タイムアウト");
                                //next_orders.Add(agv.RouteCancelOrder());

                                //@@@2021/6/15
                                //step = 200;
                                step = 0;
                            }

                            #endregion
                        }
                        break;

                    case 200:
                        if (state_received)
                        {
                            #region 目的地到達チェック

                            if (agv_request)
                            {
                                bool clear_reserve = false;

                                if (sta.sta_charge)
                                {
                                    //充電開始した（未使用・・・充電中はREQUESTにならない）
                                }

                                else if (agv.placed_rack != null && agv.placed_rack.req != null)
                                {
                                    #region 棚搬送要求でステーション到着チェック

                                    FloorQR stationqr = controller.GetStationQR(agv.placed_rack.req.station);
                                    if (agv.on_qr == stationqr)
                                    {
                                        Log("ステーション到着[" + agv.placed_rack.req.ToString() + "]");

                                        agv.placed_rack.obstruct_route = null;
                                        agv.placed_rack.station_working = true;
                                        SendRackArrive(agv.placed_rack.req);

                                        agv.Unlock();

                                        destinationQR = null;
                                        step = 210;
                                        break;
                                    }

                                    #endregion
                                }

                                else if (agv.placed_rack != null && agv.placed_rack.obstruct_route != null)
                                {
                                    #region 棚退避要求で退避場所に到着チェック

                                    if (agv.on_qr == destinationQR)
                                    {
                                        Log("棚待避完了[" + agv.placed_rack.ToString() + "]");

                                        agv.Unlock();

                                        destinationQR = null;
                                        step = 300;
                                        break;
                                    }

                                    #endregion
                                }

                                else if (escapeQR != null && agv.on_qr == escapeQR)
                                {
                                    #region 回避ポイントに到着

                                    escapeQR = null;
                                    agv.Unreserve(true);

                                    #endregion
                                }

                                else if (0 < escape_from.Count)
                                {
                                    #region 回避場所へ向かっている途中

                                    escape_from.Clear();

                                    #endregion
                                }

                                else if (agv.req != null)
                                {
                                    #region AGV移動要求で目的地に到達チェック

                                    FloorQR stationqr = controller.GetStationQR(agv.req.station.Trim());
                                    if (agv.on_qr == stationqr)
                                    {
                                        if (agv.req.rack_action == "2" && agv.rack != null)
                                        {
                                            //目的地に到着したが、棚を降ろしていない
                                        }
                                        else if (agv.req.rack_action == "1" && agv.rack == null && !sta.sta_norack)
                                        {
                                            //目的地に到着したが、棚を上げていない
                                        }
                                        else
                                        {
                                            clear_reserve = true;

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
                                                    agv.working = false;
                                                }

                                                manager.OrderSend(agv.req);
                                            }
                                            else
                                            {
                                                agv.working = false;
                                            }

                                            agv.req = null;
                                        }
                                    }

                                    #endregion
                                }

                                else
                                {
                                    ClearCondition();
                                }

                                #region チェックポイントで停止したらルート予約を解除する

                                if (agv.on_qr.escape_to != null || clear_reserve)
                                {
                                    agv.Unreserve(true);
                                }

                                #endregion

                                step = 0;
                            }
                            else if (sta.sta_charge)
                            {
                                #region 充電開始

                                agv.Unreserve(true);

                                step = 0;

                                #endregion
                            }
                            else if (cur_con != null)
                            {
                                #region ルート進行中

                                if ((agv.rack != null && agv.rack.req_return != null) || (agv.req == null && agv.prev_agvreq))
                                {
                                    #region 棚返却動作中に棚搬送要求をチェックして、棚搬送動作に切り換える

                                    bool is_requested = false;
                                    AgvOrderCommunicator.RequestBase req = null;

                                    lock (requests)
                                    {
                                        #region 要求リストから自AGVが持っている棚の搬送要求を取得して、棚搬送動作に切り換える

                                        foreach (var v in requests)
                                        {
                                            {
                                                AgvOrderCommunicator.RequestDelivery r = null;
                                                r = v as AgvOrderCommunicator.RequestDelivery;
                                                req = r;
                                                if (r != null && r.cmd == AgvOrderCommunicator.enREQ.QRS.ToString())
                                                {
                                                    FloorQR stationqr = controller.GetStationQR(r.station);
                                                    if (stationqr != null)
                                                    {
                                                        if (agv.rack.rack_no == r.rack)
                                                        {
                                                            agv.request_moving = true;
                                                            agv.prev_agvreq = false;

                                                            agv.rack.obstruct_route = null;
                                                            agv.rack.req = r;
                                                            agv.rack.req_return = null;
                                                            agv.rack.loadreserve_agv = agv;
                                                            is_requested = true;

                                                            agv.floor.mapeditor.redraw_rack = true;
                                                            state_changed = true;
                                                            break;
                                                        }
                                                    }
                                                }
                                            }

                                            {
                                                AgvOrderCommunicator.RequestMove r = null;
                                                r = v as AgvOrderCommunicator.RequestMove;
                                                req = r;
                                                if (r != null && r.cmd == AgvOrderCommunicator.enREQ.MOV.ToString())
                                                {
                                                    FloorQR stationqr = controller.GetStationQR(r.station);
                                                    if (stationqr != null)
                                                    {
                                                        agv.request_moving = true;
                                                        agv.prev_agvreq = true;
                                                        agv.req = r;

                                                        is_requested = true;

                                                        agv.floor.mapeditor.redraw_map = true;
                                                        state_changed = true;
                                                        break;
                                                    }
                                                }
                                            }
                                        }

                                        #endregion
                                    }

                                    if (is_requested)
                                    {
                                        #region 要求リストから消去して、現状のルートはキャンセルする

                                        lock (requests)
                                        {
                                            requests.Remove(req);
                                        }

                                        next_orders.Add(agv.RouteCancelOrder());
                                        agv.Unreserve(true);

                                        #endregion

                                        step = 0;
                                        break;
                                    }

                                    if (conflict_rack_down(cur_con))
                                    {
                                        #region 棚返却中に棚を置くと他のAGVが棚を置けなくなるのでルートキャンセル

                                        conditions_pre.AddRange(agv.conditions);
                                        next_orders.Add(agv.RouteCancelOrder());
                                        agv.Unreserve(true);

                                        #endregion

                                        step = 400;
                                        break;
                                    }

                                    if (agv.crash_wait)
                                    {
                                        step = 0;
                                        break;
                                    }

                                    #endregion
                                }

                                #region オートレーターの手前でルートキャンセルさせようとしていた名残（不要かも？）

                                //var autorator_qrs = cur_con.RoutingAutoratorQR();
                                ////var autorator_qrs = check_wait_pre_autorator(cur_con);
                                //if (autorator_qrs != null)
                                //{
                                //    if (0 == autorator_qrs.Count())
                                //    {
                                //        next_orders.Add(agv.RouteCancelOrder());

                                //        step = 0;
                                //        break;
                                //    }
                                //    else
                                //    {
                                //        //controller.AllAutoratorQR.Where(e => e.autorator_id == autorator_qrs.First().autorator_id).ToList().ForEach(e => e.lock_agv = agv);
                                //        autorator_qrs.ForEach(e => e.lock_agv = agv);

                                //        int ar_count = -1;
                                //        for (var c = agv.conditions[0]; c != null; c = c.next_condition)
                                //        {
                                //            if (0 <= ar_count) ar_count++;
                                //            if (autorator_qrs.Contains(c.cur_qr)) ar_count = 0;

                                //            if (2 < ar_count) break;

                                //            c.cur_qr.lock_agv = agv;
                                //        }
                                //    }
                                //}

                                #endregion

                                else if (check_wait_pre_crosspoint(cur_con))
                                {
                                    next_orders.Add(agv.RouteCancelOrder());

                                    step = 0;
                                    break;
                                }

                                #endregion
                            }

                            #endregion
                        }
                        break;

                    case 210:
                        {
                            #region ステーション完了待機

                            AgvOrderCommunicator.RequestSTComplete req = null;

                            lock (AgvRunManager.requests)
                            {
                                foreach (var v in AgvRunManager.requests)
                                {
                                    var r = v as AgvOrderCommunicator.RequestSTComplete;
                                    if (r != null && r.cmd == AgvOrderCommunicator.enREQ.QSC.ToString())
                                    {
                                        if (r.station == agv.on_qr.station_id)
                                        {
                                            req = r;
                                            break;
                                        }
                                    }
                                }

                                if (req != null)
                                {
                                    //同一の完了報告は読み捨てる
                                    {
                                        var same_st_req = AgvRunManager.requests.Where(e => typeof(AgvOrderCommunicator.RequestSTComplete).IsInstanceOfType(e)
                                                                                     && ((AgvOrderCommunicator.RequestSTComplete)e).station == req.station
                                                                                     && ((AgvOrderCommunicator.RequestSTComplete)e).rack == req.rack).ToList();
                                        foreach (var v in same_st_req)
                                        {
                                            AgvRunManager.requests.Remove(v);
                                        }
                                    }
                                }
                            }

                            if (req != null)
                            {
                                agv.on_qr.Trigger(agv.on_qr.floor.code, "");

                                Log("ステーション完了検知[" + req.ToString() + "]");

                                //ステーション完了を検出

                                agv.on_qr.Trigger("", "");

                                Rack rack = agv.placed_rack;

                                if (rack != null && rack.req != null)
                                {
                                    //ステーション完了したので、棚返却要求に切り換える
                                    agv.request_moving = false;
                                    rack.req_return = rack.req;
                                    rack.req = null;
                                    rack.obstruct_route = null;

                                    //ステーション作業中OFF
                                    rack.station_working = false;
                                }

                                step = 0;
                            }

                            #endregion
                        }
                        break;

                    case 300:
                        {
                            #region 回避元のAGVが行きすぎるまで待機

                            bool return_rack = true;

                            if (agv.placed_rack.obstruct_route != null)
                            {
                                var returnqr = GetRackSettableNearestQR();
                                if (returnqr != null)
                                {
                                    var cons = controller.routeManager.GetMoveConditions_ex(agv, returnqr, "");
                                    var targetagv_cons = agv.placed_rack.obstruct_route[0].owner_agv.conditions;

                                    if (0 < targetagv_cons.Count)
                                    {
                                        var conflictqrs = cons.Where(e => 0 < targetagv_cons.Where(ee => e.cur_qr == ee.cur_qr).Count()).ToList();

                                        if (0 < conflictqrs.Count() || 0 < cons.Where(e => e.cur_qr == targetagv_cons[0].owner_agv.on_qr).Count())
                                        {
                                            Log("回避元のAGVが行きすぎるまで待機中");
                                            return_rack = false;
                                        }
                                    }
                                }
                                else
                                {
                                    Log("棚返却場所が無い");
                                    return_rack = false;
                                    step = 0;
                                    break;
                                }
                            }

                            if (return_rack)
                            {
                                //回避した棚を返却する
                                agv.placed_rack.obstruct_route = null;
                                agv.placed_rack.req_return = new AgvOrderCommunicator.RequestDelivery(AgvOrderCommunicator.enREQ.QRS, 0, AgvOrderCommunicator.enRESULT.RQ, "", agv.placed_rack.rack_no, "");

                                Log("待避した棚を返却[" + agv.placed_rack.ToString() + "]");
                                step = 0;
                            }

                            #endregion
                        }
                        break;

                    case 400:
                        {
                            #region 棚が置けなくなるAGVが行きすぎるまで待機

                            if (0 < conditions_pre.Count)
                            {
                                if (!conflict_rack_down(conditions_pre[0]))
                                {
                                    conditions_pre.Clear();
                                    step = 0;
                                }
                                else
                                {
                                    Log("棚が置けなくなるAGVが行きすぎるまで待機中");
                                }
                            }
                            else
                            {
                                step = 0;
                            }

                            #endregion
                        }
                        break;

                    case 500:
                        {
                            #region オートレーター搬入許可待機

                            if (agv.autorator_reset)
                            {
                                agv.autorator_reset = false;

                                agv.in_autorator = null;
                                agv.out_autorator = null;

                                step = 0;
                            }
                            else if (agv.in_autorator != null)
                            {
                                if (!agv.in_autorator.autorator_info.IsRequested(agv))
                                {
                                    step = 32;
                                }
                                else if (agv.in_autorator.autorator_info.CanEntry(agv))
                                {
                                    step = 50;
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                step = 50;
                            }

                            #endregion
                        }
                        break;

                    case 600:
                        {
                            #region オートレーターからの退出許可待機

                            if (agv.autorator_reset)
                            {
                                if (sta._map.Trim() != "")
                                {
                                    agv.autorator_reset = false;

                                    agv.in_autorator = null;
                                    agv.out_autorator = null;
                                    step = 0;
                                }
                            }
                            else if (agv.out_autorator != null)
                            {
                                if (agv.out_autorator.autorator_info.CanExit(agv))
                                {
                                    step = 50;
                                }
                            }
                            else
                            {
                                if (agv.on_qr.autorator_info != null)
                                {
                                    agv.out_autorator = agv.on_qr;
                                }
                            }

                            #endregion
                        }
                        break;

                    case 900:
                        {
                            #region ルート算出不可応答

                            if (agv.req != null)
                            {
                                agv.req.result = "NG";
                                manager.OrderSend(agv.req);

                                agv.req = null;

                                step = 0;
                            }

                            #endregion
                        }
                        break;

                    case 910:
                        {
                            #region ルート上のオートレーターの使用可能を待機

                            //ルート上のオートレーターが使用できない場合、ルートを消去
                            var autoratorcon = next_cons.Where(e => e.cur_qr.autorator_id != "").FirstOrDefault();
                            if (autoratorcon != null && autoratorcon.cur_qr.autorator_info != null)
                            {
                                if (autoratorcon.cur_qr.autorator_info.Status == null)
                                {
                                    break;
                                }

                                if (autoratorcon.cur_qr.autorator_info.Status.TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.ERROR
                                 || autoratorcon.cur_qr.autorator_info.Status.TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.MANUAL
                                 || autoratorcon.cur_qr.autorator_info.Status.TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.OFFLINE
                                 || autoratorcon.cur_qr.autorator_info.Status.TotalStatus == VtslCommunicator.VtslStatus.enTotalStatus.AUTORUN_OFF
                                  )
                                {
                                    //next_cons.Clear();
                                    break;
                                }
                            }

                            step = 20;

                            #endregion
                        }
                        break;
                }


                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log("処理時間 agv_control.[" + step_save.ToString() + "→" + step.ToString() + "]:" + sw.ElapsedMilliseconds.ToString() + "ms");
                }
            }

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
                            Log("ゴミデータ削除[" + v.ToString() + "]");
                            requests.Remove(v);
                        }
                    }
                }

                return is_requested;
            }

            #endregion

            #region 各種チェック・ルート取得

            /// <summary>
            /// 交差点への進入待機チェック
            /// 直線通過＞旋回通過
            /// </summary>
            /// <param name="con"></param>
            /// <returns>true:待機／false:進入</returns>
            protected bool check_wait_pre_crosspoint(RouteCondition con)
            {
                RouteConditionList lockcons = new RouteConditionList();
                for (var c = con; c != null; c = c.next_condition)
                {
                    lock (c.cur_qr.lock_agv)
                    {
                        if (0 < c.cur_qr.lock_agv.Count && c.cur_qr.lock_agv[0] == agv)
                        {
                            lockcons.Add(c);
                        }
                        else
                        {
                            if (0 < lockcons.Count) break;
                        }
                    }
                }

                foreach (var lockcon in lockcons)
                {
                    //チェックするポイント
                    //var check_con = lockcons.Last();

                    if (lockcon.next_condition != null && lockcon.prev_condition != null)
                    {
                        //前と次のポイントが存在する

                        int straight_deg = (int)agv.Degree(lockcon.Location, lockcon.next_condition.Location);
                        int junction_deg = (int)agv.Degree(lockcon.prev_condition.Location, lockcon.Location);

                        if (Math.Abs(junction_deg - straight_deg) % 180 != 0)
                        {
                            //自分は交差点への本線への合流側(曲がり角)

                            var except_qrs = new FloorQR[] { lockcon.prev_condition.cur_qr };


                            var crossing_ways = lockcon.cur_qr.prev_way.Except(except_qrs);

                            if (0 < crossing_ways.Count())
                            {
                                //ポイントは複数の進路が合流

                                //交差点を直進で通るAGVを列挙
                                var crossing_agvs = controller.agvs.Where(e => e != agv &&
                                                                               !e.agvRunner.agv_request &&
                                                                               0 < e.conditions.Count &&
                                                                               0 < e.conditions.Where(ee => ee.cur_qr == lockcon.cur_qr).Count() &&
                                                                               0 < e.conditions[0].StraightRoute.Where(
                                                                                        ee => crossing_ways.Contains(ee.cur_qr)).Count()).ToList();
                                if (0 < crossing_agvs.Count())
                                {
                                    return true;
                                }
                            }
                            else
                            {
                                //曲がり角の始点


                            }
                        }
                        else
                        {
                            //自分が直進側

                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// 回避ルートを取得
            /// </summary>
            /// <param name="escape_from">避けたいルート</param>
            /// <returns>回避ルート</returns>
            protected RouteConditionList get_escape_route(List<FloorQR> escape_from)
            {
                RouteConditionList ret = new RouteConditionList();

                if (0 < escape_from.Count)
                {
                    List<FloorQR> esc = new List<FloorQR>();
                    foreach (var v in escape_from)
                    {
                        esc.Add(v);
                        if (v.autorator_id != "") break;
                    }

                    //var con = escape_from.Where(e => e.Location == agv.Location).FirstOrDefault();
                    //if (con == null)
                    //{
                    //    if (0 < agv.conditions.Count) escape_from.Add(agv.conditions[0]);
                    //}

                    List<FloorQR> e_route = new List<FloorQR>();
                    foreach (var v in esc)
                    {
                        if (v.Location == agv.Location) break;
                        e_route.Insert(0, v);
                    }

                    FloorQR otherway_qr = GetNonObstructQR(esc, e_route);

                    if (otherway_qr != null)
                    {
                        //ret = controller.routeManager.GetMoveConditions_ex(agv, otherway_qr, "", GetExceptQR(otherway_qr));
                        ret = controller.routeManager.GetMoveConditions_ex(agv, otherway_qr, "");
                    }
                }

                return ret;
            }

            /// <summary>
            /// 棚を下降すると、他のAGVが棚を置けなくなるかチェック
            /// </summary>
            /// <param name="con">棚を下降するまでのルート</param>
            /// <returns>true:置けなくなるので待機／false:置いてもいい</returns>
            protected bool conflict_rack_down(RouteCondition con)
            {
                RouteConditionList listcon = new RouteConditionList();
                int count = 0;
                for (var c = con; c != null; c = c.next_condition)
                {
                    listcon.Add(c);
                    count++;
                }

                RouteCondition my_rackdown_con = null;
                var rackdown_cons = listcon.Where(e => e.rack_down_arrive).ToList();
                if (0 < rackdown_cons.Count()) my_rackdown_con = rackdown_cons.First();

                if (my_rackdown_con != null)
                {
                    //自AGVは最大2個先で棚下降動作する

                    var other_rackdown_agvs = controller.agvs.Where(e => e != agv && 0 < e.conditions.Where(ee => ee.rack_down_arrive || ee.rack_down_departure || ee.rack_down_departure_last).Count()).ToList();
                    foreach (var other_rackdown_agv in other_rackdown_agvs)
                    {
                        //これから棚を置く動作をする予定のAGV

                        var other_rackdown_qr = other_rackdown_agv.conditions.Where(ee => ee.rack_down_arrive || ee.rack_down_departure || ee.rack_down_departure_last).ToList();

                        if (other_rackdown_qr.First().cur_qr == my_rackdown_con.cur_qr)
                        {
                            return false;
                        }

                        {
                            var other_conflict_cons = other_rackdown_agv.conditions.Where(e => e.cur_qr.Location == agv.Location).ToList();

                            if (0 < other_conflict_cons.Count())
                            {
                                //他のAGVのルートに自分がいる

                                return false;
                            }
                        }

                        {
                            var other_conflict_cons = other_rackdown_agv.conditions.Where(e => e.cur_qr.Location == my_rackdown_con.cur_qr.Location).ToList();

                            if (0 < other_conflict_cons.Count())
                            {
                                //他のAGVのルートに自分が棚を置こうとしているQRが含まれる

                                int other_agv_cost = controller.routeManager.CalcRouteConditionCost(other_rackdown_agv.conditions);
                                int my_cost = controller.routeManager.CalcRouteConditionCost(listcon);
                                if (my_cost < other_agv_cost)
                                {
                                    //自分の方が近い

                                    Log("当初の予定通りに棚を置くと棚を置けなくなるAGVがいる[" + other_conflict_cons.First().ToString() + "]");

                                    //＊＊＊＊降ろしたくない
                                    return true;
                                }
                            }
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// ルートを進むと他AGVと衝突する時に、自AGVが回避ルートを取得
            /// </summary>
            /// <param name="con">目的地までのルート</param>
            /// <returns>null:回避不可／!null:回避ルート</returns>
            protected RouteConditionList conflict_wait_or_escape_route(RouteCondition con)
            {
                BL_Stopwatch sw = new BL_Stopwatch();
                sw.Restart();



                //自分が避けるべきか判定

                bool need_escape = false;

                FloorAGV otheragv = null;

                lock (con.cur_qr.lock_agv)
                {
                    otheragv = (0 < con.cur_qr.lock_agv.Count) ? con.cur_qr.lock_agv[0] : con.cur_qr.on_agv;
                }

                if (otheragv != null)
                {
                    var my_con = con;

                    if (agv.placed_rack != null && (agv.placed_rack.req != null || agv.placed_rack.station_working) && (otheragv.placed_rack == null || otheragv.placed_rack.req == null))
                    {
                        //自AGVは要求のある棚を搬送中 and (相手は棚を持っていない or 要求のない棚)
                        return null;
                    }
                    else if (0 < agv.conditions.Count && agv.conditions[0].cur_qr.prev_way.Count <= 2 && agv.conditions[0].cur_qr.prev_way[0].autorator_id != "")
                    {
                        //自AGVはオートレーターにしか戻れない
                        return null;
                    }
                    else if ((agv.placed_rack == null || agv.placed_rack.req == null) && (otheragv.placed_rack != null && (otheragv.placed_rack.req != null || otheragv.placed_rack.station_working || otheragv.placed_rack.obstruct_route != null)))
                    {
                        //自AGVは(棚を持っていない or 要求のない棚) and 相手は要求のある棚を搬送中

                        need_escape = true;
                    }
                    else if (otheragv.placed_rack != null && otheragv.placed_rack.station_working)
                    {
                        //相手は棚を持っておりステーション作業中

                        if (agv.placed_rack == null || agv.placed_rack.req == null)
                        {
                            //自分は要求のある棚を搬送していない

                            need_escape = true;
                        }
                    }
                    else if (otheragv.req != null && agv.req == null)
                    {
                        need_escape = true;
                    }
                    else if (otheragv.placed_rack != null && !otheragv.placed_rack.station_working)
                    {
                        //相手は棚を持っているがステーション作業中ではない

                        if (0 < otheragv.conditions.Count && otheragv.conditions[0].cur_qr.prev_way.Count <= 2)
                        {
                            if (otheragv.conditions[0].cur_qr.prev_way[0].autorator_id != "")
                            {
                                //相手はオートレーターにしか戻れない

                                var escape = get_escape_route(otheragv.routed_qr);
                                if (0 < escape.Count)
                                {
                                    Log("回避ルート生成[" + escape.Last().ToString() + "]");

                                    if (2000 < sw.ElapsedMilliseconds)
                                    {
                                        Log("処理時間 conflict_wait_or_escape_route:" + sw.ElapsedMilliseconds.ToString() + "ms");
                                    }

                                    return escape;
                                }
                            }
                        }

                        if (!need_escape)
                        {
                            if (agv.Distance(con.cur_qr.Location) < otheragv.Distance(con.cur_qr.Location))
                            {
                                var other_cons = otheragv.conditions.Where(e => e.Location != otheragv.Location).ToList();
                                if (0 < other_cons.Count())
                                {
                                    var other_con = other_cons.First();
                                    if (other_con != null)
                                    {
                                        need_escape = true;

                                        if (0 == agv.conditions.Where(e => e.Location == other_con.Location).Count())
                                        {
                                            //行先は別の場所
                                            Log("衝突回避不要(行先は別の場所)");
                                            need_escape = false;
                                        }
                                        else if (my_con.Direction == (int)otheragv.Degree(otheragv.Location, other_con.Location))
                                        {
                                            //行先は同じ方向
                                            Log("衝突回避不要(行先は同じ方向)");
                                            need_escape = false;
                                        }


                                        if (!need_escape)
                                        {
                                            //衝突地点まで自分が近いか？
                                            if (agv.Distance(agv.Location, my_con.Location) < otheragv.Distance(otheragv.Location, other_con.Location))
                                            {
                                                Log("衝突回避（先行）");

                                                if (2000 < sw.ElapsedMilliseconds)
                                                {
                                                    Log("処理時間 conflict_wait_or_escape_route:" + sw.ElapsedMilliseconds.ToString() + "ms");
                                                }

                                                return agv.conditions;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (need_escape)
                    {
                        foreach (var v in agv.conditions)
                        {
                            FloorQR otheragvqr = otheragv.on_qr;

                            if (agv.rack == null || (agv.rack != null && agv.rack.req == null) &&
                                (otheragv.rack != null && otheragv.rack.req != null))
                            {
                                //自分は作業要求動作中ではなく、対向相手は作業要求動作中

                                //if (escape_condition.Count == 0)
                                {
                                    //回避ルート未生成

                                    //避けるルートを生成する
                                    RouteConditionList rcl = new RouteConditionList();

                                    if (0 < otheragv.conditions.Count)
                                    {
                                        rcl = AgvRouteManager.ConnectConditions(rcl, otheragv.conditions);
                                        //rcl.AddRange(otheragv.conditions);
                                    }

                                    RouteCondition rc1 = new RouteCondition();
                                    rc1.cur_qr = otheragv.on_qr;
                                    rc1.owner_agv = otheragv;
                                    rcl.Add(rc1);

                                    RouteCondition rc2 = new RouteCondition();
                                    rc2.cur_qr = agv.on_qr;
                                    rc2.owner_agv = agv;
                                    rcl.Add(rc2);

                                    var escape = get_escape_route(rcl.ToQrList());
                                    if (0 < escape.Count)
                                    {
                                        if (0 == escape.Where(e => e.cur_qr == otheragv.on_qr).Count())
                                        {
                                            if (escape.Last().cur_qr.on_agv != null && escape.Last().cur_qr.on_agv != agv && escape.Last().cur_qr.on_agv.crash_wait)
                                            {
                                                rcl.AddRange(escape.Last().cur_qr.on_agv.conditions);
                                                escape = get_escape_route(rcl.ToQrList());
                                            }

                                            Log("回避ルート生成[" + escape.Last().ToString() + "]");

                                            if (2000 < sw.ElapsedMilliseconds)
                                            {
                                                Log("処理時間 conflict_wait_or_escape_route:" + sw.ElapsedMilliseconds.ToString() + "ms");
                                            }

                                            return escape;
                                        }
                                        else
                                        {
                                            rcl = new RouteConditionList();

                                            rc1 = new RouteCondition();
                                            rc1.cur_qr = otheragv.on_qr;
                                            rc1.owner_agv = otheragv;
                                            rcl.Add(rc1);

                                            rc2 = new RouteCondition();
                                            rc2.cur_qr = agv.on_qr;
                                            rc2.owner_agv = agv;
                                            rcl.Add(rc2);

                                            escape = get_escape_route(rcl.ToQrList());
                                            if (0 < escape.Count)
                                            {
                                                if (escape.Last().cur_qr.on_agv != null && escape.Last().cur_qr.on_agv != agv && escape.Last().cur_qr.on_agv.crash_wait)
                                                {
                                                    rcl.AddRange(escape.Last().cur_qr.on_agv.conditions);
                                                    escape = get_escape_route(rcl.ToQrList());
                                                }

                                                Log("回避ルート生成[" + escape.Last().ToString() + "]");

                                                if (2000 < sw.ElapsedMilliseconds)
                                                {
                                                    Log("処理時間 conflict_wait_or_escape_route:" + sw.ElapsedMilliseconds.ToString() + "ms");
                                                }

                                                return escape;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                ////どちらかが避ける

                                //need_escape = false;

                                //if (0 < otheragv.conditions.Count)
                                //{
                                //    if (agv.straight_cost < otheragv.straight_cost)
                                //    {
                                //        need_escape = true;
                                //    }
                                //    else if (otheragv.straight_cost == agv.straight_cost)
                                //    {
                                //        //同一コスト
                                //        bool my_agv_is_found = false;
                                //        foreach (var a in controller.agvs)
                                //        {
                                //            if (a == agv) my_agv_is_found = true;
                                //            if (a == otheragv)
                                //            {
                                //                if (my_agv_is_found)
                                //                {
                                //                    need_escape = true;
                                //                    break;
                                //                }
                                //            }
                                //        }
                                //    }
                                //}

                                //if (need_escape)
                                {
                                    FloorQR escapr_qr = GetNonObstructQR(otheragv.routed_qr);
                                    if (escapr_qr != null)
                                    {
                                        List<FloorQR> except_route = new List<FloorQR>();
                                        except_route.Add(otheragv.on_qr);

                                        var new_cons = controller.routeManager.GetMoveConditions_ex(agv, escapr_qr, "", except_route);

                                        if (0 < new_cons.Count)
                                        {
                                            if (2000 < sw.ElapsedMilliseconds)
                                            {
                                                Log("処理時間 conflict_wait_or_escape_route:" + sw.ElapsedMilliseconds.ToString() + "ms");
                                            }

                                            return new_cons;
                                        }
                                    }
                                }
                            }


                        }
                    }
                    else if (otheragv != null)
                    {
                        //どちらかが避ける

                        need_escape = false;

                        if (otheragv.crash_wait)
                        {
                            need_escape = true;
                        }
                        else if (0 < otheragv.conditions.Count)
                        {
                            if (agv.straight_cost < otheragv.straight_cost)
                            {
                                need_escape = true;
                            }
                            else if (otheragv.straight_cost == agv.straight_cost)
                            {
                                //同一コスト
                                bool my_agv_is_found = false;
                                foreach (var a in controller.agvs)
                                {
                                    if (a == agv) my_agv_is_found = true;
                                    if (a == otheragv)
                                    {
                                        if (my_agv_is_found)
                                        {
                                            need_escape = true;
                                            break;
                                        }
                                    }
                                }
                            }
                        }

                        if (need_escape)
                        {
                            RouteConditionList rcl = new RouteConditionList();
                            rcl.AddRange(otheragv.conditions);

                            if (otheragv.crash_wait) rcl.AddRange(agv.conditions);

                            FloorQR escapr_qr = GetNonObstructQR(rcl.ToQrList());
                            if (escapr_qr != null)
                            {
                                List<FloorQR> except_route = new List<FloorQR>();
                                except_route.Add(otheragv.on_qr);
                                except_route.Add(agv.on_qr);

                                var new_cons = controller.routeManager.GetMoveConditions_ex(agv, escapr_qr, "", except_route);

                                if (1 < new_cons.Count)
                                {
                                    if (2000 < sw.ElapsedMilliseconds)
                                    {
                                        Log("処理時間 conflict_wait_or_escape_route:" + sw.ElapsedMilliseconds.ToString() + "ms");
                                    }

                                    return new_cons;
                                }
                            }
                        }
                    }
                }
                else if (agv.rack != null)
                {
                    //棚が衝突検知の原因
                    var rack_qrs = agv.conditions.Where(e => e.cur_qr.rack != null && e.cur_qr.rack.obstruct_route == null);

                    if (agv.rack.req != null)
                    {
                        var new_cons = controller.routeManager.GetMoveConditions_ex(agv, agv.conditions.Last().cur_qr, agv.rack.req.rackface, null, true);
                        if (0 < new_cons.Count())
                        {
                            if (2000 < sw.ElapsedMilliseconds)
                            {
                                Log("処理時間 conflict_wait_or_escape_route:" + sw.ElapsedMilliseconds.ToString() + "ms");
                            }

                            return new_cons;
                        }
                    }

                    foreach (var v in rack_qrs)
                    {
                        Log("棚待避が必要2[" + v.cur_qr.ToString() + "]");
                        v.cur_qr.rack.obstruct_route = agv.conditions;
                    }
                }


                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log("処理時間 conflict_wait_or_escape_route:" + sw.ElapsedMilliseconds.ToString() + "ms");
                }


                return null;
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
                            Log("充電動作指示開始[" + cons.Last().ToString() + "]");

                            cons.Last().wait_charge_trg = true;
                        }
                    }
                    else
                    {
                        var charge_agvs = free_agvs.Where(e => e.agvRunner != null
                                                               && e.agvRunner.communicator.Alive
                                                               //&& e.agvRunner.communicator.GetState.bat <= manager.BATTERY_LOW
                                                               ).OrderBy(e => e.agvRunner.communicator.GetState.bat).ToList();



                        if (0 < charge_agvs.Count())
                        {
                            if (charge_agvs.First() == agv)
                            {
                                cons = controller.routeManager.GetMoveConditions_ex(agv, free_charge_qr.First(), "");

                                if (0 < cons.Count)
                                {
                                    Log("充電動作指示開始[" + cons.Last().ToString() + "]");

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
                //                    Log("充電動作指示開始(他AGVは充電中の場所)[" + cons.Last().ToString() + "]");

                //                    cons.Last().wait_charge_trg = true;
                //                }
                //            }
                //        }
                //    }
                //}


                if (2000 < sw.ElapsedMilliseconds)
                {
                    Log("処理時間 get_charge_route:" + sw.ElapsedMilliseconds.ToString() + "ms");
                }

                return cons;
            }

            #endregion

            #endregion
        }
    }
}
