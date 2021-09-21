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

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        public class AgvAutoCommander : AgvRunner
        {
            AgvControlManager controller = null;
            BL_HRTimer timerTimeout = new BL_HRTimer();

            List<AgvCommunicator.Order> next_orders = new List<AgvCommunicator.Order>();

            AgvCommunicator.State sta_pre = new AgvCommunicator.State();
            AgvCommunicator.Order ord_pre = new AgvCommunicator.Order();

            public Queue<AgvCommunicator.State> queState = new Queue<AgvCommunicator.State>();
            private List<RouteCondition> unlock_cons = new List<RouteCondition>();

            public override string state_string => "[" + step + "] " + agv_move_kind.ToString();

            public bool state_received = false;

            public AgvAutoCommander(AgvRunManager manager, FloorAGV agv, AgvConnector connector)
                : base(manager, agv)
            {
                this.connector = connector;
                controller = Program.controller;
            }

            public override void Start()
            {
                base.Start();
                request = false;

                Log("START");

                ClearCondition();

                if (communicator != null) Stop();
                communicator = new AgvController.AgvCommunicator(agv.id, connector.ip, connector.remote_client, connector.remote_host, connector.recv, connector.send);
                communicator.ReceiveEvent += Communicator_ReceiveEvent;
                communicator.StartControl(5);
            }

            public override void Stop()
            {
                Log("STOP");

                if (communicator != null)
                {
                    communicator.ReceiveEvent -= Communicator_ReceiveEvent;
                    communicator.StopControl();
                    communicator = null;
                }

                agv.autorator_in = null;
                
                base.Stop();
            }

            private void Communicator_ReceiveEvent(AgvCommunicator sender, AgvCommunicator.ReceiveEventArgs e)
            {
                state_received = true;

                lock (queState)
                {
                    queState.Enqueue(e.state);
                }
            }

            public void ClearCondition()
            {
                if (0 < agv.conditions.Count)
                {
                    agv.conditions[0].UnlockWay(true);
                    agv.conditions.Clear();
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
                manager.order_com.Send(res);
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
                manager.order_com.Send(res);
            }

            #endregion

            #region 情報取得

            List<FloorQR> fixed_except_qr = null;

            public List<FloorQR> GetNonObstructQRs(RouteConditionList except_qr, RouteConditionList except_route = null)
            {
                if (fixed_except_qr == null)
                {
                    fixed_except_qr = new List<FloorQR>();

                    var autorator_qrs = controller.AllQR.Where(e => e.autorator_id != "");
                    foreach (var v in autorator_qrs)
                    {
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

                //全AGVのルートを収集
                List<FloorQR> e_qr = new List<FloorQR>(fixed_except_qr);
                foreach (var v in except_qr)
                {
                    e_qr.Add(v.cur_qr);
                }
                //e_qr.Add(agv.on_qr);

                List<FloorQR> e_route = new List<FloorQR>(fixed_except_qr);
                if (except_route != null)
                {
                    foreach (var v in except_route)
                    {
                        e_route.Add(v.cur_qr);
                        e_qr.Add(v.cur_qr);
                    }
                }

                //誰も通らない最小コストQRを取得
                var open_qrs = controller.AllQR.Where(e => e.floor == agv.floor)
                                .Except(e_qr)
                                .OrderBy(e => controller.routeManager.CalcRouteConditionCost(
                                         controller.routeManager.GetMoveConditions(agv, e, "", e_route)));
                return open_qrs.ToList();
            }

            public FloorQR GetNonObstructQR(RouteConditionList except_qr, RouteConditionList except_route = null)
            {
                var open_qrs = GetNonObstructQRs(except_qr, except_route);

                if (0 < open_qrs.Count())
                {
                    return open_qrs.First();
                }

                return null;
            }

            public FloorAGV GetNearestAgv(FloorQR targetQR)
            {
                //待機中のAGV列挙
                var normalagv = controller.agvs.Where(e => e.rack == null
                                                        && e.agvRunner.can_order
                                                        && e.agvRunner.request
                                                        && !e.agvRunner.communicator.GetState.sta_charge
                                                        && manager.BATTERY_NOP < e.agvRunner.communicator.GetState.bat);

                List<FloorQR> except_qrs = GetExceptQR(targetQR);

                var nearestagv = normalagv.OrderBy(e => controller.routeManager.CalcRouteConditionCost(
                                                            controller.routeManager.GetMoveConditions(e, targetQR, "", except_qrs)));
                if (0 < nearestagv.Count())
                {
                    return nearestagv.First();
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
                var settable_qrs = controller.AllQR.Where(e => e.floor.code == agv.floor.code && e.rack_setable && e.rack == null).ToList();

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
                                                              controller.routeManager.GetMoveConditions(agv, e, "", GetExceptQR(e))));
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

            public override void _DoControl(object message)
            {
                autolator_control();

                if (state_received) state_control();

                if (mapeditor.Exist(agv.Location) != null)
                {
                    //到着検知
                    if (state_received) check_arrive();

                    //棚搬送要求
                    check_request();

                    ////棚回避
                    //obstruct_rack_check();

                    //回避動作
                    check_escape();

                    //充電動作
                    if (state_received) check_charge();

                    //棚搬送動作
                    if (state_received) check_move_rack();

                    //移動動作
                    if (state_received) check_move();

                    //動作指示
                    check_order();
                }

                //動作指示送信
                order_control();

                state_received = false;
            }

            int current_deg = -999;
            bool delailed = false;

            #region AGV状態受信・応答指示送信

            protected virtual bool state_control()
            {
                bool received = false;
                AgvCommunicator.State sta = null;

                lock (queState)
                {
                    while (0 < queState.Count)
                    {
                        sta = queState.Dequeue();

                        communicator.Log("STEP=" + step.ToString("000") + "," + sta.ToString());
                        received = true;

                        if (sta.x == 0 && sta.y == 0)
                        {
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
                                    v.lock_agv = agv;

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

                            next_orders.Add(agv.Response(sta));
                            continue;
                        }
                        else
                        {
                            if (delailed)
                            {
                                delailed = false;

                                var list = floor.mapeditor.list.Where(e => e.lock_agv == agv);
                                foreach (var v in list)
                                {
                                    v.lock_agv = null;
                                }
                            }
                        }

                        if (sta.IsChanged(sta_pre))
                        {
                            if (0 < controller.agvs.Where(e => e != agv && e.Location == sta.Location).Count())
                            {
                                Log("＊＊＊衝突＊＊＊");
                            }

                            //communicator.Log(sta.ToString());
                            sta_pre.SetBytes(sta.GetBytes());

                            state_changed = true;
                        }

                        if (sta.map.Trim() == "")
                        {
                            if (cur_con != null && cur_con.next_condition != null)
                            {
                                sta.map = cur_con.next_condition.cur_qr.floor.code;
                            }
                            else
                            {
                                sta.map = agv.on_qr.floor.code;
                            }
                        }

                        #region AGV状態遷移

                        if (sta.cmd == (ushort)AgvCommunicator.State.CMD.STATE ||
                            sta.cmd == (ushort)AgvCommunicator.State.CMD.REQUEST)
                        {
                            if (Location != sta.Location || agv.floor.code != sta.map)
                            {
                                Log("座標変化[" + sta.Location.ToString() + "]");

                                SetLocation(sta.map, sta.Location);
                            }

                            if (sta.sta_rack)
                            {
                                if (agv.rack == null)
                                {
                                    Log("棚上昇[" + sta.rack_no + "]");

                                    var rack_qr = controller.GetRackQR(sta.rack_no.ToString());

                                    if (rack_qr == null)
                                    {
                                        if (sta.rack_no != 0 && agv.on_qr.rack == null)
                                        {
                                            Log("棚存在しない→生成");

                                            //棚生成
                                            agv.on_qr.rack = new Rack();
                                            agv.on_qr.rack.rack_id = sta.rack_no.ToString();
                                            agv.on_qr.rack.face_id[0] = "1";
                                            agv.on_qr.rack.face_id[90] = "2";
                                            agv.on_qr.rack.face_id[180] = "3";
                                            agv.on_qr.rack.face_id[270] = "4";
                                            agv.on_qr.rack.can_inout[0] = true;
                                            agv.on_qr.rack.can_inout[90] = true;
                                            agv.on_qr.rack.can_inout[180] = true;
                                            agv.on_qr.rack.can_inout[270] = true;
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
                                            //Log("棚角度変化[" + sta.rack_deg + "]");
                                        }
                                        agv.rack.degree = sta.rack_deg;
                                    }
                                }
                            }
                            else
                            {
                                if (agv.rack != null)
                                {
                                    //agv.rack.obstructing_route = null;
                                    agv.rack.loading_agv = null;
                                    agv.rack.loadreserve_agv = null;

                                    //if (agv.rack.req_return != null)
                                    //{
                                    //    //棚返却完了
                                    //    Log("作業棚返却完了[" + agv.rack.req_return.ToString() + "]");

                                    //    SendRackReturn(agv.rack.req_return, AgvOrderCommunicator.enRESULT.OK);
                                    //    agv.rack.req_return = null;
                                    //}

                                    if (sta.rack_deg != 9999)
                                    {
                                        if (agv.rack.degree != sta.rack_deg)
                                        {
                                            //Log("棚角度変化[" + sta.rack_deg + "]");

                                            agv.rack.degree = sta.rack_deg;
                                        }
                                    }

                                    Log("棚下降[" + agv.rack.ToString() + "]");

                                    agv.RackDown();
                                }
                            }

                            if (Degree != sta.deg)
                            {
                                //Log("AGV角度変化[" + sta.deg + "]");

                                Degree = sta.deg;
                            }

                            if (mapeditor.Exist(sta.Location) != null)
                            {
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
                                        //if (con.rack_turn_arrive)
                                        //{
                                        //    if (sta.rack_deg != con.rack_turn_arrive_degree) break;
                                        //}
                                        //if (con.rack_turn_departure)
                                        //{
                                        //    if (sta.rack_deg != con.rack_turn_departure_degree) break;
                                        //}
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
                                                var crash_con = cur_con.LockWay(0);
                                                if (crash_con != null)
                                                {
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

                                if (agv.conditions.Count == 0 || cur_con == null)
                                {
                                    ClearCondition();
                                }
                            }

                            stop = sta.sta_stop;

                            //応答を返送
                            next_orders.Add(agv.Response(sta));
                        }

                        #endregion

                        #region AGV状態更新

                        if (sta.cmd == (ushort)AgvCommunicator.State.CMD.STATE)
                        {
                            if (!stat)
                            {
                                Log("AGV→状態報告");
                            }

                            request = false;
                            stat = true;
                        }
                        else if (sta.cmd == (ushort)AgvCommunicator.State.CMD.REQUEST)
                        {
                            if (!request)
                            {
                                Log("AGV→指示要求");

                                ClearCondition();
                            }

                            request = true;
                            stat = false;
                            route_cancel = false;
                        }
                        else if (sta.cmd == (ushort)AgvCommunicator.State.CMD.RES_ORDER)
                        {
                            if (!order)
                            {
                                Log("AGV→動作指示応答");
                            }
                            order = true;
                        }

                        #endregion
                    }
                }

                return received;
            }

            protected virtual void order_control()
            {
                List<AgvCommunicator.Order> list = new List<AgvCommunicator.Order>();

                while (0 < next_orders.Count)
                {
                    list.Clear();

                    var res = next_orders.Where(e => e.cmd == (ushort)AgvCommunicator.Order.CMD.RESPONSE);
                    if (0 < res.Count())
                    {
                        foreach (var v in res)
                        {
                            communicator.SetOrder(new AgvCommunicator.Order[] { v });
                            list.Add(v);

                            //if (v.IsChanged(ord_pre))
                            {
                                communicator.Log("STEP=" + step.ToString("000") + "," + v.ToString());
                                ord_pre.SetBytes(v.GetBytes());
                            }
                        }

                        foreach (var v in list) next_orders.Remove(v);
                    }


                    if (list.Count == 0)
                    {
                        var rca = next_orders.Where(e => e.cmd == (ushort)AgvCommunicator.Order.CMD.ROUTE_CANCEL);
                        if (0 < rca.Count())
                        {
                            foreach (var v in rca)
                            {
                                communicator.SetOrder(new AgvCommunicator.Order[] { v });
                                list.Add(v);

                                //if (v.IsChanged(ord_pre))
                                {
                                    communicator.Log("STEP=" + step.ToString("000") + "," + v.ToString());
                                    ord_pre.SetBytes(v.GetBytes());
                                }
                            }

                            foreach (var v in list) next_orders.Remove(v);

                            route_cancel = true;
                        }
                    }

                    if (list.Count == 0)
                    {
                        var ord = next_orders.Where(e => e.cmd != (ushort)AgvCommunicator.Order.CMD.RESPONSE);
                        if (0 < ord.Count())
                        {
                            order = false;

                            communicator.SetOrder(ord.ToArray());

                            foreach (var v in ord)
                            {
                                list.Add(v);

                                Log(v.ToString());
                                communicator.Log("STEP=" + step.ToString("000") + "," + v.ToString());

                                ord_pre.SetBytes(v.GetBytes());
                            }

                            Log("動作指示完了[" + list.Count() + "件]");

                            foreach (var v in list) next_orders.Remove(v);
                        }
                    }
                }
            }

            #endregion

            #region AGV制御

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

            RouteConditionList escape_from = new RouteConditionList();
            FloorQR destinationQR = null;
            RouteConditionList conditions_pre = new RouteConditionList();

            RouteConditionList next_cons = new RouteConditionList();

            /// <summary>
            /// 待避する棚の処理
            /// </summary>
            /// <returns></returns>
            private bool obstruct_rack_check()
            {
                if (!agv.request_moving && request)
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
                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// 棚搬送要求処理
            /// </summary>
            /// <returns></returns>
            private bool check_request()
            {
                bool is_requested = false;

                if (!agv.request_moving)
                {
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
                            if (agv.placed_rack != null && agv.placed_rack.rack_id == req.rack)
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
                                agv.request_moving = true;

                                rack.obstruct_route = null;
                                rack.req = req;
                                rack.req_return = null;
                                rack.loadreserve_agv = agv;
                                is_requested = true;

                                agv.floor.mapeditor.redraw_rack = true;
                                state_changed = true;

                                if (!request)
                                {
                                    next_orders.Add(agv.RouteCancelOrder());
                                }

                                break;
                            }
                        }
                    }

                    if (is_requested)
                    {
                        lock (requests)
                        {
                            requests.Remove(req);
                        }
                    }
                }

                return is_requested;
            }

            /// <summary>
            /// 充電処理
            /// </summary>
            /// <returns></returns>
            private bool check_charge()
            {
                if (!sta_pre.sta_charge)
                {
                    if (request && can_order && agv.rack == null)
                    {
                        if (sta_pre.bat <= manager.BATTERY_LOW)
                        {
                            //充電が必要

                            next_cons = get_charge_route();

                            if (0 < next_cons.Count)
                            {
                                agv_move_kind = enAgvMoveKind.AGV_CHARGE;
                                return true;
                            }
                        }
                    }
                }
                else
                {
                    //充電が必要なAGVが存在する
                    var charge_agvs = controller.agvs.Where(e => e != agv && e.agvRunner.request && e.agvRunner.communicator.GetState.bat <= manager.BATTERY_LOW)
                                                     .OrderBy(e => e.agvRunner.communicator.GetState.bat);
                    if (manager.BATTERY_MID <= sta_pre.bat && 0 < charge_agvs.Count())
                    {
                        agv_move_kind = enAgvMoveKind.AGV_STANBY;
                        Log("充電停止指示開始(譲り合い)");

                        next_orders.Add(agv.ChargeStopOrder());
                    }
                    else if (manager.BATTERY_FUL <= sta_pre.bat)
                    {
                        agv_move_kind = enAgvMoveKind.AGV_STANBY;
                        Log("充電停止指示開始");

                        next_orders.Add(agv.ChargeStopOrder());
                    }
                }

                return false;
            }

            /// <summary>
            /// 回避動作処理
            /// </summary>
            /// <returns></returns>
            private bool check_escape()
            {
                if (request && can_order)
                {
                    if (0 < escape_from.Count)
                    {
                        //回避中で回避先に到着していない
                        agv_move_kind = enAgvMoveKind.AGV_ESCAPE;

                        var con = escape_from.Where(e => e.Location == agv.Location).FirstOrDefault();

                        RouteConditionList e_route = new RouteConditionList();
                        for (var c = escape_from[0]; c != null && c.next_condition != null; c = c.next_condition)
                        {
                            if (c.Location == agv.Location) break;
                            e_route.Insert(0, c);
                        }

                        FloorQR otherway_qr = GetNonObstructQR(escape_from, e_route);

                        if (otherway_qr != null)
                        {
                            next_cons = controller.routeManager.GetMoveConditions(agv, otherway_qr);
                        }
                        escape_from.Clear();
                    }
                    else
                    {
                        if (agv.rack == null && agv.on_qr.rack == null)
                        {
                            var conflict_route_agv = controller.agvs.Where(e => e != agv && 0 < e.conditions.Where(ee => ee.cur_qr.Location == agv.Location).Count());

                            if (0 < conflict_route_agv.Count())
                            {
                                //待避が必要

                                foreach (var v in conflict_route_agv)
                                {
                                    escape_from.AddRange(v.conditions);
                                }
                            }
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// 棚搬送動作処理
            /// </summary>
            /// <returns></returns>
            private bool check_move_rack()
            {
                if (request && can_order)
                {
                    if (agv.rack != null)
                    {
                        if (agv.rack.req != null)
                        {
                            //予約している要求のある棚を持っている
                            agv_move_kind = enAgvMoveKind.RACK_REQUEST;

                            if (!agv.rack.station_working)
                            {
                                //ステーション作業中ではない棚を持っている
                                FloorQR stationqr = controller.GetStationQR(agv.placed_rack.req.station);
                                if (stationqr != null)
                                {
                                    next_cons = controller.routeManager.GetMoveConditions(agv, stationqr, agv.placed_rack.req.rackface, null, true);
                                    if (next_cons.Count() == 0)
                                    {
                                        next_cons = controller.routeManager.GetMoveConditions(agv, stationqr, agv.placed_rack.req.rackface, null, false);
                                    }
                                }
                            }
                            
                            return true;
                        }
                        
                        if (agv.rack.obstruct_route != null)
                        {
                            //予約している待避が必要な棚を持っている
                            agv_move_kind = enAgvMoveKind.RACK_ESCAPE;

                            var escapeqr = GetNonObstructQR(agv.rack.obstruct_route);
                            if (escapeqr != null)
                            {
                                next_cons = controller.routeManager.GetMoveConditions(agv, escapeqr);
                            }

                            return true;
                        }
                        
                        if (agv.rack.req_return == null)
                        {
                            //何も無い棚を持っている
                            //agv_move_kind = enAgvMoveKind.AGV_WAIT;

                            agv.rack.req_return = new AgvOrderCommunicator.RequestDelivery(AgvOrderCommunicator.enREQ.QRS, 0, AgvOrderCommunicator.enRESULT.RQ, "", agv.rack.rack_id, "");
                        }

                        if (agv.rack.req_return != null)
                        {
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

                                next_cons = controller.routeManager.GetMoveConditions(agv, returnqr, "", exeptqr, true);
                                if (next_cons.Count() == 0)
                                {
                                    next_cons = controller.routeManager.GetMoveConditions(agv, returnqr, "", exeptqr, false);
                                }

                                if (0 < next_cons.Count())
                                {
                                    next_cons.Last().rack_down_arrive = true;
                                }
                            }

                            return true;
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// 移動動作処理
            /// </summary>
            /// <returns></returns>
            private bool check_move()
            {
                if (request && can_order)
                {
                    if (agv.rack == null)
                    {
                        agv_move_kind = enAgvMoveKind.AGV_STANBY;

                        FloorQR reserved_rack_qr = controller.AllRackQR.Where(e => e.rack != null && e.rack.loadreserve_agv == agv).FirstOrDefault();

                        if (reserved_rack_qr != null)
                        {
                            if (reserved_rack_qr.rack.req != null)
                            {
                                //予約している要求のある棚までの移動
                                agv_move_kind = enAgvMoveKind.RACK_REQUEST;

                                FloorQR stationqr = controller.GetStationQR(reserved_rack_qr.rack.req.station);
                                if (stationqr != null)
                                {
                                    var cons1 = controller.routeManager.GetMoveConditions(agv, reserved_rack_qr);
                                    if (0 < cons1.Count())
                                    {
                                        cons1.Last().rack_up_arrive = true;

                                        PointF location_save = Location;
                                        string floor_save = agv.floor.code;

                                        SetLocation(cons1.Last().cur_qr.floor.code, cons1.Last().Location);
                                        agv.rack = reserved_rack_qr.rack;
                                        reserved_rack_qr.rack = null;

                                        var cons2 = controller.routeManager.GetMoveConditions(agv, stationqr, agv.rack.req.rackface, null, true);
                                        if (cons2.Count == 0)
                                        {
                                            cons2 = controller.routeManager.GetMoveConditions(agv, stationqr, agv.rack.req.rackface, null, false);
                                        }

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

                                            cons1 = controller.routeManager.GetMoveConditions(agv, reserved_rack_qr, "", except_qrs);
                                            if (0 < cons1.Count())
                                            {
                                                cons1.Last().rack_up_arrive = true;

                                                location_save = Location;
                                                floor_save = agv.floor.code;

                                                SetLocation(cons1.Last().cur_qr.floor.code, cons1.Last().Location);
                                                agv.rack = reserved_rack_qr.rack;
                                                reserved_rack_qr.rack = null;

                                                cons2 = controller.routeManager.GetMoveConditions(agv, stationqr, agv.rack.req.rackface, except_qrs, false);
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

                                return true;
                            }

                            if (reserved_rack_qr.rack.req_return != null)
                            {
                                //予約している返却する棚までの移動
                                agv_move_kind = enAgvMoveKind.RACK_RETURN;

                                var cons1 = controller.routeManager.GetMoveConditions(agv, reserved_rack_qr);
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
                                        var cons2 = controller.routeManager.GetMoveConditions(agv, freerackqr);
                                        if (0 < cons2.Count())
                                        {
                                            next_cons = AgvRouteManager.ConnectConditions(cons1, cons2);
                                        }
                                    }

                                    SetLocation(floor_save, location_save);
                                    reserved_rack_qr.rack = agv.rack;
                                    agv.rack = null;
                                }

                                return true;
                            }

                            if (reserved_rack_qr.rack.obstruct_route != null)
                            {
                                //予約している待避が必要な棚までの移動
                                agv_move_kind = enAgvMoveKind.RACK_ESCAPE;

                                RouteConditionList cons1 = new RouteConditionList();
                                if (reserved_rack_qr.rack.obstruct_route[0].owner_agv.rack != null)
                                {
                                    cons1 = controller.routeManager.GetMoveConditions(agv, reserved_rack_qr, "", new List<FloorQR>() { reserved_rack_qr.rack.obstruct_route[0].owner_agv.on_qr });
                                }
                                else
                                {
                                    cons1 = controller.routeManager.GetMoveConditions(agv, reserved_rack_qr);
                                }

                                if (0 < cons1.Count())
                                {
                                    cons1.Last().rack_up_arrive = true;

                                    PointF location_save = Location;
                                    string floor_save = agv.floor.code;

                                    SetLocation(cons1.Last().cur_qr.floor.code, cons1.Last().Location);

                                    agv.rack = reserved_rack_qr.rack;
                                    reserved_rack_qr.rack = null;

                                    var escapeqr = GetNonObstructQR(agv.rack.obstruct_route);
                                    if (escapeqr != null)
                                    {
                                        var cons2 = controller.routeManager.GetMoveConditions(agv, escapeqr);
                                        if (0 < cons2.Count())
                                        {
                                            next_cons = AgvRouteManager.ConnectConditions(cons1, cons2);
                                        }
                                    }

                                    SetLocation(floor_save, location_save);
                                    reserved_rack_qr.rack = agv.rack;
                                    agv.rack = null;
                                }

                                return true;
                            }
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// 動作指示処理
            /// </summary>
            /// <returns></returns>
            private bool check_order()
            {
                if (request && cur_con == null && 0 < agv.conditions.Count)
                {
                    agv.conditions.Clear();
                }

                if (request && can_order)
                {
                    if (0 < next_cons.Count)
                    {
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
                                if (v.cur_qr.rack != null && v.cur_qr.rack.loadreserve_agv != agv)
                                {
                                    Log("棚待避が必要1[" + v.cur_qr.ToString() + "]");

                                    //v.cur_qr.rack.obstruct_route = new RouteConditionList(rc);

                                    ClearCondition();
                                    next_cons.Clear();
                                    return false;
                                }
                            }
                            else
                            {
                                rc.Remove(v);
                            }
                        }

                        ClearCondition();
                        agv.conditions.AddRange(next_cons);

                        var crash_con = agv.conditions[0].LockWay(0);
                        if (crash_con == null)
                        {
                            var autorator_qrs = check_wait_pre_autorator(agv.conditions[0]);
                            if (autorator_qrs != null)
                            {
                                if (0 == autorator_qrs.Count())
                                {
                                    //オートレーターがロック済みで侵入不可
                                    agv.crash_wait = true;
                                    ClearCondition();
                                    return false;
                                }
                                else
                                {
                                    //controller.AllAutoratorQR.Where(e => e.autorator_id == autorator_qrs.First().autorator_id).ToList().ForEach(e => e.lock_agv = agv);
                                    autorator_qrs.ForEach(e => e.lock_agv = agv);

                                    int ar_count = -1;
                                    for (var c = agv.conditions[0]; c != null; c = c.next_condition)
                                    {
                                        if (0 <= ar_count) ar_count++;
                                        if (autorator_qrs.Contains(c.cur_qr)) ar_count = 0;

                                        if (2 < ar_count) break;

                                        c.cur_qr.lock_agv = agv;
                                    }
                                }
                            }
                            else if (check_wait_pre_crosspoint(agv.conditions[0]))
                            {
                                agv.crash_wait = true;
                                ClearCondition();
                                return false;
                            }

                            //邪魔なAGVや棚がおらず、移動を開始できる
                            agv.crash_wait = false;

                            cur_con = agv.conditions[0];

                            destinationQR = agv.conditions.Last().cur_qr;

                            var orders = agv.GetOrders(0);
                            if (0 < orders.Length)
                            {
                                next_orders.AddRange(orders);

                                Log("動作指示開始[" + agv.conditions.Count + "件]->[" + agv.conditions.Last().cur_qr.ToString() + "]");

                                if (controller.EventPaint != null) controller.EventPaint();
                            }
                            else
                            {
                                ClearCondition();
                            }

                            next_cons.Clear();
                            return true;
                        }
                        else
                        {
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
                                        return false;
                                    }
                                }

                                //if (agv.rack == null || (agv.rack.req == null && agv.rack.req_return == null && agv.rack.obstruct_route == null))
                                {
                                    //回避ルート再生成

                                    if (crash_con.cur_qr.on_agv != null)
                                    {
                                        var er = conflict_wait_or_escape_route(crash_con);
                                        if (er != null)
                                        {
                                            next_cons.Clear();
                                            next_cons.AddRange(er);
                                            return false;
                                        }
                                    }
                                    else if (crash_con.cur_qr.lock_agv != null)
                                    {
                                        var er = conflict_wait_or_escape_route(crash_con);
                                        if (er != null)
                                        {
                                            next_cons.Clear();
                                            next_cons.AddRange(er);
                                            return false;
                                        }
                                    }
                                    else if (agv.rack != null && crash_con.cur_qr.rack != null)
                                    {
                                        //棚で衝突検知
                                        next_cons.Clear();
                                        ClearCondition();

                                        return false;
                                    }
                                }

                                if (0 < next_cons.Count && next_cons.Last().cur_qr == agv.on_qr)
                                {
                                    //回避できない
                                }
                            }
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// 到着処理
            /// </summary>
            /// <returns></returns>
            private bool check_arrive()
            {
                if (request && cur_con == null)
                {
                    if (sta_pre.sta_charge)
                    {
                        //充電中


                    }
                    else if (agv.placed_rack != null && agv.placed_rack.req != null)
                    {
                        FloorQR stationqr = controller.GetStationQR(agv.placed_rack.req.station);
                        if (agv.on_qr == stationqr)
                        {
                            if (agv.placed_rack.station_working)
                            {
                                //ステーション完了待機
                                AgvOrderCommunicator.RequestSTComplete req = null;

                                lock (manager.requests)
                                {
                                    foreach (var v in manager.requests)
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
                                            var same_st_req = manager.requests.Where(e => typeof(AgvOrderCommunicator.RequestSTComplete).IsInstanceOfType(e)
                                                                                         && ((AgvOrderCommunicator.RequestSTComplete)e).station == req.station
                                                                                         && ((AgvOrderCommunicator.RequestSTComplete)e).rack == req.rack).ToList();
                                            foreach (var v in same_st_req)
                                            {
                                                manager.requests.Remove(v);
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

                                    //ステーション完了したので、棚返却要求に切り換える
                                    agv.request_moving = false;
                                    agv.placed_rack.req_return = agv.placed_rack.req;
                                    agv.placed_rack.req = null;
                                    agv.placed_rack.obstruct_route = null;

                                    //ステーション作業中OFF
                                    agv.placed_rack.station_working = false;
                                }
                            }
                            else
                            {
                                Log("ステーション到着[" + agv.placed_rack.req.ToString() + "]");

                                agv.placed_rack.obstruct_route = null;
                                agv.placed_rack.station_working = true;
                                SendRackArrive(agv.placed_rack.req);

                                var lockqrs = controller.AllQR.Where(e => e.lock_agv == agv);
                                foreach (var v in lockqrs) v.lock_agv = null;

                                destinationQR = null;
                            }

                            return true;
                        }
                    }
                    else if (agv.rack == null && agv.on_qr.rack != null && agv.on_qr.rack.req_return != null)
                    {
                        if (agv.on_qr == destinationQR)
                        {
                            Log("棚返却完了[" + agv.placed_rack.req_return.ToString() + "]");

                            if (agv.on_qr.rack.req_return.station != "")
                            {
                                SendRackReturn(agv.on_qr.rack.req_return, AgvOrderCommunicator.enRESULT.OK);
                            }

                            agv.placed_rack.obstruct_route = null;
                            agv.on_qr.rack.req_return = null;
                            agv.floor.mapeditor.redraw_rack = true;
                            state_changed = true;

                            var lockqrs = controller.AllQR.Where(e => e.lock_agv == agv);
                            foreach (var v in lockqrs) v.lock_agv = null;

                            destinationQR = null;

                            return true;
                        }
                    }
                    else if (agv.placed_rack != null && agv.placed_rack.obstruct_route != null)
                    {
                        if (agv.on_qr == destinationQR)
                        {
                            bool return_rack = true;

                            var returnqr = GetRackSettableNearestQR();
                            if (returnqr != null)
                            {
                                var cons = controller.routeManager.GetMoveConditions(agv, returnqr);
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
                            }

                            if (return_rack)
                            {
                                //回避した棚を返却する
                                agv.placed_rack.obstruct_route = null;
                                agv.placed_rack.req_return = new AgvOrderCommunicator.RequestDelivery(AgvOrderCommunicator.enREQ.QRS, 0, AgvOrderCommunicator.enRESULT.RQ, "", agv.placed_rack.rack_id, "");

                                Log("待避した棚を返却[" + agv.placed_rack.ToString() + "]");

                                var lockqrs = controller.AllQR.Where(e => e.lock_agv == agv);
                                foreach (var v in lockqrs) v.lock_agv = null;

                                destinationQR = null;

                                return true;
                            }
                        }
                    }
                    else if (0 < escape_from.Count)
                    {
                        escape_from.Clear();
                    }
                    else
                    {
                        ClearCondition();
                    }

                    step = 0;
                }

                return false;
            }

            /// <summary>
            /// オートレーター前の待機処理
            /// </summary>
            /// <param name="con"></param>
            /// <returns>null:オートレーターは存在しない／0件のリスト:他AGVが予約しているオートレーターが存在／1件以上のリスト:自AGVがロックするオートレーターQRのリスト</returns>
            private List<FloorQR> check_wait_pre_autorator(RouteCondition con)
            {
                RouteConditionList lockcons = new RouteConditionList();
                for (var c = con; c != null; c = c.next_condition)
                {
                    if (c.cur_qr.lock_agv == agv)
                    {
                        lockcons.Add(c);
                    }
                    else if (0 < lockcons.Count) break;
                }

                foreach (var lockcon in lockcons)
                {
                    var straight_route = lockcon.StraightRoute;
                    var autorator_cons = straight_route.Where(e => e.cur_qr.autorator_id != "").ToList();
                    if (0 < autorator_cons.Count())
                    {
                        //ロックしたポイントからの直線上にオートレーターがある

                        var autorator_con = autorator_cons.First();
                        if (autorator_con.cur_qr.lock_agv != agv && autorator_con.cur_qr.lock_agv != null)
                        {
                            //オートレーターは他のAGVがロックしている
                            return new List<FloorQR>();
                        }
                        else
                        {
                            //自AGVでロックするオートレーターQRを列挙

                            List<FloorQR> locked_qrs = controller.AllAutoratorQR.Where(e => e.autorator_id == autorator_con.cur_qr.autorator_id).ToList();

                            //for (var q = autorator_con; q.next_condition != null && q.cur_qr.autorator_id != ""; q = q.next_condition)
                            //{
                            //    locked_qrs.Add(q.cur_qr);
                            //}
                            return locked_qrs;
                        }
                    }
                }

                //オートレーターは存在しない
                return null;
            }

            private bool check_wait_pre_crosspoint(RouteCondition con)
            {
                RouteConditionList lockcons = new RouteConditionList();
                for (var c = con; c != null; c = c.next_condition)
                {
                    if (c.cur_qr.lock_agv == agv)
                    {
                        lockcons.Add(c);
                    }
                    else if (0 < lockcons.Count) break;
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
                                                                               !e.agvRunner.request &&
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

            private RouteConditionList get_escape_route(RouteConditionList escape_from)
            {
                RouteConditionList ret = new RouteConditionList();

                if (0 < escape_from.Count)
                {
                    //var con = escape_from.Where(e => e.Location == agv.Location).FirstOrDefault();
                    //if (con == null)
                    //{
                    //    if (0 < agv.conditions.Count) escape_from.Add(agv.conditions[0]);
                    //}

                    RouteConditionList e_route = new RouteConditionList();
                    for (var c = escape_from[0]; c != null && c.next_condition != null; c = c.next_condition)
                    {
                        if (c.Location == agv.Location) break;
                        e_route.Insert(0, c);
                    }

                    FloorQR otherway_qr = GetNonObstructQR(escape_from, e_route);

                    if (otherway_qr != null)
                    {
                        //ret = controller.routeManager.GetMoveConditions(agv, otherway_qr, "", GetExceptQR(otherway_qr));
                        ret = controller.routeManager.GetMoveConditions(agv, otherway_qr);
                    }
                }

                return ret;
            }

            private bool conflict_rack_down(RouteCondition con)
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

            private RouteConditionList conflict_wait_or_escape_route(RouteCondition con)
            {
                //自分が避けるべきか判定

                bool need_escape = false;

                FloorAGV otheragv = con.cur_qr.lock_agv != null ? con.cur_qr.lock_agv : con.cur_qr.on_agv;

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
                    else if (otheragv.placed_rack != null && !otheragv.placed_rack.station_working)
                    {
                        //相手は棚を持っているがステーション作業中ではない

                        if (0 < otheragv.conditions.Count && otheragv.conditions[0].cur_qr.prev_way.Count <= 2)
                        {
                            if (otheragv.conditions[0].cur_qr.prev_way[0].autorator_id != "")
                            {
                                //相手はオートレーターにしか戻れない

                                var escape = get_escape_route(otheragv.conditions);
                                if (0 < escape.Count)
                                {
                                    Log("回避ルート生成[" + escape.Last().ToString() + "]");

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
                                    var escape = get_escape_route(otheragv.conditions);
                                    if (1 < escape.Count)
                                    {
                                        Log("回避ルート生成[" + escape.Last().ToString() + "]");

                                        return escape;
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
                                    FloorQR escapr_qr = GetNonObstructQR(otheragv.conditions);
                                    if (escapr_qr != null)
                                    {
                                        List<FloorQR> except_route = new List<FloorQR>();
                                        except_route.Add(otheragv.on_qr);

                                        var new_cons = controller.routeManager.GetMoveConditions(agv, escapr_qr, "", except_route);

                                        if (0 < new_cons.Count)
                                        {
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

                        if (0 < otheragv.conditions.Count)
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
                            FloorQR escapr_qr = GetNonObstructQR(otheragv.conditions);
                            if (escapr_qr != null)
                            {
                                List<FloorQR> except_route = new List<FloorQR>();
                                except_route.Add(otheragv.on_qr);

                                var new_cons = controller.routeManager.GetMoveConditions(agv, escapr_qr, "", except_route);

                                if (1 < new_cons.Count)
                                {
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
                        var new_cons = controller.routeManager.GetMoveConditions(agv, agv.conditions.Last().cur_qr, agv.rack.req.rackface, null, true);
                        if (1 < new_cons.Count())
                        {
                            return new_cons;
                        }
                    }

                    foreach (var v in rack_qrs)
                    {
                        Log("棚待避が必要2[" + v.cur_qr.ToString() + "]");
                        v.cur_qr.rack.obstruct_route = agv.conditions;
                    }
                }

                return null;
            }

            private RouteConditionList get_charge_route()
            {
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
                                                                           controller.routeManager.GetMoveConditions(agv, e))).ToList();

                if (0 < free_charge_qr.Count())
                {
                    var charge_agvs = free_agvs.Where(e => e.agvRunner != null && e.agvRunner.communicator.GetState.bat <= manager.BATTERY_LOW)
                             .OrderBy(e => e.agvRunner.communicator.GetState.bat).ToList();

                    if (0 < charge_agvs.Count())
                    {
                        if (charge_agvs.First() == agv)
                        {
                            cons = controller.routeManager.GetMoveConditions(agv, free_charge_qr.First());

                            if (0 < cons.Count)
                            {
                                Log("充電動作指示開始[" + cons.Last().ToString() + "]");

                                cons.Last().wait_charge_trg = true;
                            }
                        }
                    }
                }
                //else
                //{
                //    free_charge_qr = agv.floor.mapeditor.list.Where(e => e.direction_charge != FloorQR.enDirection.NONE)
                //                                             .OrderBy(e => controller.routeManager.CalcRouteConditionCost(
                //                                                           controller.routeManager.GetMoveConditions(agv, e)));

                //    if (0 < free_charge_qr.Count())
                //    {
                //        var charge_agvs = free_agvs.Where(e => e.agvRunner.communicator.GetState.bat <= manager.BATTERY_LOW && !e.agvRunner.communicator.GetState.sta_charge)
                //                 .OrderBy(e => e.agvRunner.communicator.GetState.bat);

                //        if (0 < charge_agvs.Count())
                //        {
                //            if (charge_agvs.First() == agv)
                //            {
                //                cons = controller.routeManager.GetMoveConditions(agv, free_charge_qr.First());

                //                if (0 < cons.Count)
                //                {
                //                    Log("充電動作指示開始(他AGVは充電中の場所)[" + cons.Last().ToString() + "]");

                //                    cons.Last().wait_charge_trg = true;
                //                }
                //            }
                //        }
                //    }
                //}

                return cons;
            }

            #endregion

            #region オートレーター取り合い

            private void autolator_control()
            {



            }

            #endregion
        }
    }
}
