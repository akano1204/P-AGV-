using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

using BelicsClass.Common;
using BelicsClass.Network;
using BelicsClass.File;

using AgvController;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        public class AgvModeCommander : AgvRunner
        {
            public delegate void ErrorStop_EventHandler(object sender, string description);
            public event ErrorStop_EventHandler EventErrorStop;

            public AgvModeCommander(AgvRunManager manager, FloorAGV agv, AgvConnector connector)
                : base(manager, agv)
            {
                this.connector = connector;
            }

            public override void Start()
            {
                base.Start();

                if (communicator != null) Stop();
                communicator = new AgvController.AgvCommunicator(agv.id, connector.ip, connector.remote_client, connector.remote_host, connector.recv, connector.send);
                communicator.ReceiveEvent += Communicator_ReceiveEvent;
                communicator.StartControl(20);

                agv_state = false;
                agv_request = false;
                agv_stop = false;
                agv_restart = false;
            }

            public override void Stop()
            {
                #region オートレータ外に移動完了させる(未使用)

                //if (agv.autorator_in != null)
                //{
                //    if (agv.autorator_in != null)
                //    {
                //        if (agv.autorator_in.autoratorOUT_trigger)
                //        {
                //            AgvCommunicator.State sta = communicator.GetState;
                //            FloorQR exist_qr = floor.mapeditor.Exist(new PointF(sta.x, sta.y));
                //            if (exist_qr != null)
                //            {
                //                double distance = agv.autorator_in.Distance(exist_qr);
                //                if (200 < distance)
                //                {
                //                    agv.autorator_in.Trigger(false, "OUT");
                //                    agv.autorator_in = null;

                //                    foreach (var v in manager.clients)
                //                    {
                //                        string send = BL_EnumLabel.GetLabel(enCommand.AUTORATOR) + BL_EnumLabel.GetLabel(enAutorator.OUT);
                //                        communicator.Log("STEP=" + step.ToString() + "," + "S[AT_OUT:" + send + "]");
                //                        manager.Log("[" + agv.id + "] S[AT_OUT:" + send + "]");
                //                        v.Send(Encoding.ASCII.GetBytes(send));
                //                    }
                //                }
                //            }
                //        }
                //    }
                //}

                #endregion

                if (communicator != null)
                {
                    communicator.StopControl();
                    communicator = null;
                }

                agv.in_autorator = null;
                agv.out_autorator = null;

                base.Stop();
            }

            private void Communicator_ReceiveEvent(AgvCommunicator sender, AgvCommunicator.ReceiveEventArgs e)
            {
                communicator.Log("STEP=" + step.ToString() + "," + state.ToString());

                lock (agv)
                {
                    #region AGV状態更新

                    if (e.state.cmd == (ushort)AgvCommunicator.State.CMD.REQUEST)
                    {
                        agv_request = true;
                        agv_order_response = false;
                        agv_stop = false;
                        agv_restart = false;

                        communicator.SetOrder(new AgvCommunicator.Order[] { agv.Response(e.state) });
                    }
                    else if (e.state.cmd == (ushort)AgvCommunicator.State.CMD.STATE)
                    {
                        agv_request = false;
                        agv_order_response = false;

                        communicator.SetOrder(new AgvCommunicator.Order[] { agv.Response(e.state) });
                    }
                    else if (e.state.cmd == (ushort)AgvCommunicator.State.CMD.RES_ORDER)
                    {
                        agv_request = false;
                        agv_order_response = true;
                        agv_route_cancel = false;
                    }
                    else if (e.state.cmd == (ushort)AgvCommunicator.State.CMD.RES_ROUTE_CANCEL)
                    {
                        agv_route_cancel = true;
                    }

                    #endregion
                }
            }

            //bool try_rotation = false;
            int step_charge = 0;
            int cur_index = 0;
            bool async_repeat = false;

            public override void _DoControl(object message)
            {
                List<AgvCommunicator.Order> orders = new List<AgvCommunicator.Order>();
                AgvCommunicator.State sta = communicator.GetState;

                FloorQR exist_qr = null;
                if (floor.controller.map.ContainsKey(sta.map))
                {
                    exist_qr = mapeditor.Exist(new PointF(sta.x, sta.y));

                    if (exist_qr != null)
                    {
                        if (exist_qr.autorator_id == "")
                        {
                            agv.floor = floor.controller.map[sta.map];
                            exist_qr = mapeditor.Exist(new PointF(sta.x, sta.y));
                        }

                        SetLocation("", new PointF(sta.x, sta.y));
                        Degree = sta.deg;
                    }
                }
                else
                {
                    exist_qr = mapeditor.Exist(new PointF(sta.x, sta.y));
                    if (exist_qr != null)
                    {
                        SetLocation("", new PointF(sta.x, sta.y));
                        Degree = sta.deg;
                    }
                }

                if (exist_qr != null)
                {
                    #region 棚操作

                    if (sta.sta_rack)
                    {
                        if (sta.rack_deg < 999)
                        {
                            if (agv.rack == null)
                            {
                                agv.RackUp(exist_qr);

                                if (agv.rack != null)
                                {
                                    if (agv.rack.degree != sta.rack_deg)
                                    {
                                        orders.Add(agv.RouteCancelOrder());

                                        AgvCommunicator.Order order = new AgvCommunicator.Order();
                                        order.cmd = (ushort)AgvCommunicator.Order.CMD.ROUTE_CANCEL;
                                        order.x = (int)agv.Location.X;
                                        order.y = (int)agv.Location.Y;
                                        orders.Add(order);

                                        step = -1;

                                        if (EventErrorStop != null) EventErrorStop(this, "棚の向きが一致していません。\n棚の向きを修正して、リセットしてください。");
                                    }
                                }
                            }

                            if (agv.rack != null) agv.rack.degree = sta.rack_deg;
                        }
                    }
                    else
                    {
                        if (agv.rack != null && sta.rack_deg < 999) agv.rack.degree = sta.rack_deg;
                        if (agv.rack != null) agv.RackDown(exist_qr);
                    }

                    #endregion

                    #region オートレータ内に侵入完了

                    if (exist_qr.autorator_id != "")
                    {
                        if (exist_qr.autoratorIN_trigger != "")
                        {
                            agv.in_autorator = exist_qr;
                            agv.in_autorator.Trigger("", "IN");

                            //foreach (var v in manager.clients)
                            //{
                            //    string send = BL_EnumLabel.GetLabel(enCommand.AUTORATOR) + BL_EnumLabel.GetLabel(enAutorator.IN);
                            //    communicator.Log("STEP=" + step.ToString() + "," + "S[AT_IN:" + send + "]");
                            //    manager.Log("[" + agv.id + "] S[AT_IN:" + send + "]");
                            //    v.Send(Encoding.ASCII.GetBytes(send));
                            //}
                        }
                    }

                    #endregion

                    #region オートレータ外に移動完了

                    if (agv.in_autorator != null)
                    {
                        FloorQR out_qr = agv.floor.mapeditor.Exist(agv.in_autorator.Location);

                        if (out_qr.autoratorOUT_trigger != "")
                        {
                            double distance = out_qr.Distance(exist_qr);

                            //退出後1400mm以上離れた位置のQRに到達したことを持って、オートレーターへ退出通知を行う
                            //オートレーター出口から1400mmの直線が必要
                            if (140 <= distance)
                            {
                                agv.in_autorator.Trigger("", "OUT");
                                agv.in_autorator = null;

                                //foreach (var v in manager.clients)
                                //{
                                //    string send = BL_EnumLabel.GetLabel(enCommand.AUTORATOR) + BL_EnumLabel.GetLabel(enAutorator.OUT);
                                //    communicator.Log("STEP=" + step.ToString() + "," + "S[AT_OUT:" + send + "]");
                                //    manager.Log("[" + agv.id + "] S[AT_OUT:" + send + "]");
                                //    v.Send(Encoding.ASCII.GetBytes(send));
                                //}
                            }
                        }
                    }

                    #endregion
                }

                #region 充電停止指示

                switch (step_charge)
                {
                    case 0:
                        if (sta.sta_charge && manager.BATTERY_FUL <= sta.bat)
                        {
                            orders.Add(agv.ChargeStopOrder());
                            step_charge = 10;
                        }
                        break;

                    case 10:
                        if (!sta.sta_charge)
                        {
                            step_charge = 0;
                        }
                        break;
                }

                #endregion

                #region 自動運転確認

                if (!sta.sta_runmode) return;

                #endregion

                switch (step)
                {
                    case 0:
                        #region 次移動定義取得

                        if (cur_con == null)
                        {
                            if (moveMode_now != agv.moveMode && !async_repeat)
                            {
                                if (0 < agv.mode_conditions[agv.moveMode].Count)
                                {
                                    if (agv.on_qr == agv.mode_conditions[agv.moveMode][0].cur_qr)
                                    {
                                        communicator.Log("STEP=" + step.ToString() + "," + "動作モード変更[" + agv.moveMode.ToString() + "]");
                                        moveMode_now = agv.moveMode;
                                        cur_index = 0;
                                    }
                                    else
                                    {
                                    }
                                }
                                else
                                {
                                    if (0 < agv.mode_conditions[moveMode_now].Count)
                                    {
                                        agv.mode_conditions[moveMode_now][0].UnlockWay(true);
                                    }

                                    //定位置
                                    communicator.Log("STEP=" + step.ToString() + "," + "動作モード変更[" + agv.moveMode.ToString() + "]");
                                    moveMode_now = agv.moveMode;
                                    cur_index = 0;
                                }
                            }
                            else if (0 < agv.mode_conditions[moveMode_now].Count)
                            {
                                async_repeat = false;
                                //try_rotation = false;

                                if (agv.on_qr == agv.mode_conditions[moveMode_now][cur_index].cur_qr)
                                {
                                    if (agv_request || sta.sta_charge)
                                    {
                                        if (agv.mode_conditions[moveMode_now].Count <= 1)
                                        {
                                            communicator.Log("STEP=" + step.ToString() + "," + "動作モード変更待機[" + agv.mode_conditions[moveMode_now][cur_index].ToString() + "]");

                                            state = enState.STOP;

                                            //switch (moveMode_now)
                                            //{
                                            //    case 0:
                                            //        foreach (var v in manager.clients)
                                            //        {
                                            //            string send = BL_EnumLabel.GetLabel(enCommand.MODE) + "0";
                                            //            communicator.Log("STEP=" + step.ToString() + "," + "S[" + send + "]");
                                            //            manager.Log("[" + agv.id + "] S[" + send + "]");
                                            //            v.Send(Encoding.ASCII.GetBytes(send));
                                            //        }
                                            //        break;
                                            //}

                                            step = 10;
                                        }
                                        else
                                        {
                                            if (sta.sta_charge)
                                            {
                                                //充電停止指示未送信
                                                if (step_charge == 0)
                                                {
                                                    orders.Add(agv.ChargeStopOrder());
                                                }

                                                step = 20;
                                            }
                                            else
                                            {
                                                var crash_con = agv.mode_conditions[moveMode_now][cur_index].LockWay();
                                                if (crash_con != null)
                                                {
                                                    state = enState.WAIT | enState.WAIT_CRASH;
                                                    break;
                                                }

                                                cur_con = agv.mode_conditions[moveMode_now][cur_index];

                                                lock (agv)
                                                {
                                                    orders.AddRange(agv.GetOrders(moveMode_now));
                                                }

                                                communicator.Log("STEP=" + step.ToString() + "," + "次移動地点ループ[" + cur_con.ToString() + "]");

                                                swWaitTimer.Restart();

                                                step = 30;
                                            }
                                        }
                                    }
                                    //else
                                    //{
                                    //    AgvCommunicator.Order order = new AgvCommunicator.Order();
                                    //    order.cmd = (ushort)AgvCommunicator.Order.CMD.ROUTE_CANCEL;
                                    //    order.x = (int)agv.Location.X;
                                    //    order.y = (int)agv.Location.Y;

                                    //    step = 40;
                                    //}
                                }
                                else
                                {
                                    state = enState.STOP;
                                }
                            }
                            else state = enState.STOP;
                        }
                        else
                        {
                            step = 100;
                        }
                        #endregion
                        break;

                    case 10:
                        #region 動作なし(モード変更待機)

                        if (moveMode_now != agv.moveMode)
                        {
                            if (cur_con != null)
                            {
                                cur_con.UnlockWay(true);
                                cur_con = null;
                            }

                            if (agv.moveMode == 0)
                            {
                                step = 910;
                            }
                            else
                            {
                                step = 0;
                            }
                        }

                        #endregion
                        break;

                    case 20:
                        #region 充電完了待機
                        {
                            if (!sta.sta_charge)
                            {
                                step = 0;
                            }
                            else if (5000 < swWaitTimer.ElapsedMilliseconds)
                            {
                                orders.Add(agv.ChargeStopOrder());
                                swWaitTimer.Restart();
                            }
                        }
                        #endregion
                        break;

                    case 30:
                        #region 行き先要求応答待機

                        if (!agv_request)
                        {
                            state = enState.MOVE;
                            step = 100;
                        }
                        else if (5000 < swWaitTimer.ElapsedMilliseconds)
                        {
                            communicator.Log("STEP=" + step.ToString() + "," + "動作指示再送");

                            swWaitTimer.Restart();

                            cur_index = 0;
                            var crash_con = agv.mode_conditions[moveMode_now][cur_index].LockWay();
                            if (crash_con != null)
                            {
                                state = enState.WAIT | enState.WAIT_CRASH;
                                break;
                            }

                            cur_con = agv.mode_conditions[moveMode_now][cur_index];

                            lock (agv)
                            {
                                orders.AddRange(agv.GetOrders(moveMode_now));
                            }
                        }

                        #endregion
                        break;

                    //case 40:
                    //    #region ルートキャンセル応答待機

                    //    if (route_cancel)
                    //    {
                    //        step = 0;
                    //    }
                    //    else if (5000 < swWaitTimer.ElapsedMilliseconds)
                    //    {
                    //        swWaitTimer.Restart();

                    //    }
                    //    #endregion
                    //    break;

                    case 100:
                        #region 条件成立(STATION)

                        if (cur_con.FloorCode != agv.floor.code)
                        {
                            break;
                        }

                        if (cur_con.wait_station_trg)
                        {
                            if (!sta.sta_stop)
                            {
                                state = enState.WAIT_STATION;
                                break;
                            }

                            if (!cur_con.cur_qr.station_complete_trigger)
                            {
                                RouteCondition con = FindAutoratorQR(cur_con);
                                if (con != null)
                                {
                                    if ((state & enState.WAIT) == 0)
                                    {
                                        cur_con.UnlockWay(true);

                                        if ((state & enState.WAIT) == 0)
                                        {
                                            //foreach (var v in manager.clients)
                                            //{
                                            //    string send = "02" + con.FloorCode + con.next_condition.FloorCode;
                                            //    communicator.Log("STEP=" + step.ToString() + "," + "S[PICK:" + send + "]");
                                            //    manager.Log("[" + agv.id + "] S[PICK: " + send + "]");
                                            //    v.Send(Encoding.ASCII.GetBytes(send));
                                            //}

                                            //orders.Add(PauseOrder());
                                            cur_con.UnlockWay(true);
                                        }

                                        state = enState.WAIT | enState.WAIT_STATION;
                                        break;


                                        //#region ピッキング開始要求

                                        //if (cur_con.cur_qr.station_id == "PICK")
                                        //{
                                        //    string rackno = "0";
                                        //    string rackface = "0";
                                        //    if (agv.rack != null)
                                        //    {
                                        //        rackno = agv.rack.rack_id;

                                        //        int deg = (int)cur_con.cur_qr.direction_station - (int)agv.rack.degree;
                                        //        deg = (deg - 180) % 360 + 180;

                                        //        if (agv.rack.face_id.ContainsKey(deg))
                                        //        {
                                        //            rackface = agv.rack.face_id[deg];
                                        //        }
                                        //    }

                                        //    foreach (var v in manager.clients)
                                        //    {
                                        //        string send = "02" + rackno + rackface;
                                        //        communicator.Log("S[PICK:" + send + "]");
                                        //        manager.Log("[" + agv.id + "] S[PICK: " + send + "]");
                                        //        v.Send(Encoding.ASCII.GetBytes(send));
                                        //    }
                                        //}

                                        //#endregion

                                        //#region 商品補充要求

                                        //if (cur_con.cur_qr.station_id == "SUPLY")
                                        //{
                                        //    foreach (var v in manager.clients)
                                        //    {
                                        //        string send = "04";
                                        //        communicator.Log("S[SUPLY:" + send + "]");
                                        //        manager.Log("[" + agv.id + "] S[SUPLY:" + send + "]");
                                        //        v.Send(Encoding.ASCII.GetBytes(send));
                                        //    }
                                        //}

                                        //#endregion
                                    }

                                    state = enState.WAIT | enState.WAIT_STATION;
                                    break;
                                }
                            }
                            else
                            {
                                cur_con.cur_qr.Trigger("", "");
                                step = 200;
                            }
                        }
                        else step = 200;

                        #endregion
                        break;

                    case 200:
                        #region 条件成立(オートレータIN)

                        if (cur_con.wait_autorator_in_trg)
                        {
                            if (!sta.sta_stop)
                            {
                                break;
                            }

                            RouteCondition con = FindAutoratorQR(cur_con);
                            if (con != null)
                            {
                                if (con.next_condition != null)
                                {
                                    FloorQR qr = con.cur_qr;

                                    //半開で棚積載中は侵入不可
                                    //if (qr.autoratorHalfOpen && sta.sta_rack)
                                    //{
                                    //    state = enState.WAIT_AUTORATOR_IN;
                                    //    break;
                                    //}
                                    //else 
                                    if (qr.autoratorIN_trigger == "")
                                    {
                                        if ((state & enState.WAIT) == 0)
                                        {
                                            //foreach (var v in manager.clients)
                                            //{
                                            //    string send = "02" + con.FloorCode + con.next_condition.FloorCode;
                                            //    communicator.Log("STEP=" + step.ToString() + "," + "S[PICK:" + send + "]");
                                            //    manager.Log("[" + agv.id + "] S[PICK: " + send + "]");
                                            //    v.Send(Encoding.ASCII.GetBytes(send));
                                            //}

                                            //orders.Add(PauseOrder());
                                            cur_con.UnlockWay(true);
                                        }

                                        state = enState.WAIT | enState.WAIT_AUTORATOR_IN;
                                        break;
                                    }
                                    else step = 220;
                                }
                                else step = 220;
                            }
                            else step = 220;
                        }
                        else step = 220;

                        #endregion
                        break;

                    case 220:
                        #region 条件成立(オートレータOUT)

                        if (cur_con.wait_autorator_out_trg)
                        {
                            if (!sta.sta_stop)
                            {
                                break;
                            }

                            RouteCondition con = FindAutoratorQR(cur_con);
                            FloorQR qr = null;
                            if (con != null)
                            {
                                if (cur_con.cur_qr.autorator_id != "") qr = cur_con.cur_qr;
                                else qr = con.cur_qr;

                                if (qr != null)
                                {
                                    if (qr.autoratorOUT_trigger == "")
                                    {
                                        if ((state & enState.WAIT) == 0)
                                        {
                                            //orders.Add(PauseOrder());
                                            cur_con.UnlockWay(true);
                                        }

                                        state = enState.WAIT | enState.WAIT_AUTORATOR_OUT;
                                        break;
                                    }
                                    else
                                    {
                                        //フロア移動処理
                                        string floorcode = cur_con.cur_qr.autoratorOUT_trigger;
                                        agv.floor = agv.floor.controller.SelectFloor(floorcode);

                                        step = 300;
                                    }
                                }
                                else step = 300;
                            }
                            else step = 300;
                        }
                        else step = 300;

                        #endregion
                        break;

                    case 300:
                        #region 条件成立(CHARGE)

                        if (cur_con.wait_charge_trg)
                        {
                            if (!sta.sta_stop)
                            {
                                break;
                            }

                            if (sta.sta_charge)
                            {
                                if ((state & enState.WAIT) == 0)
                                {
                                    cur_con.UnlockWay(true);
                                }

                                state = enState.WAIT | enState.CHARGE;

                                step = 310;
                                break;
                            }
                        }
                        else step = 400;

                        break;

                    case 310:
                        if (cur_con.wait_charge_trg)
                        {
                            if (sta.sta_charge)
                            {
                                break;
                            }
                            else step = 400;
                        }
                        else step = 400;

                        #endregion
                        break;

                    case 400:
                        #region 時間待機

                        if (cur_con.wait_timer > 0)
                        {
                            if ((state & enState.WAIT) == 0) orders.Add(agv.RouteCancelOrder());
                            state = enState.WAIT;
                            swWaitTimer.Restart();
                            step = 410;
                        }
                        else step = 500;

                        break;

                    case 410:
                        if (cur_con.wait_timer < swWaitTimer.ElapsedMilliseconds)
                        {
                            swWaitTimer.Stop();
                            step = 500;
                        }

                        #endregion
                        break;

                    case 500:
                        #region 他AGVとの同期待機
                        {
                            if (sta.x != cur_con.Location.X || sta.y != cur_con.Location.Y)
                            {
                                step = 600;
                            }
                            else
                            {
                                bool do_next = true;
                                foreach (var v in cur_con.wait_other_agv_locations)
                                {
                                    if (!v.Key.HitTest(v.Value))
                                    {
                                        state = enState.WAIT | enState.WAIT_OTHER_AGVS;

                                        do_next = false;
                                        break;
                                    }
                                }

                                if (do_next)
                                {
                                    step = 600;
                                }
                            }
                        }
                        #endregion
                        break;

                    case 600:
                        #region 衝突検知

                        if (cur_con != null)
                        {
                            lock (agv)
                            {
                                var crash_con = cur_con.LockWay();
                                if (crash_con != null)
                                {
                                    if ((state & enState.WAIT) == 0)
                                    {
                                        communicator.Log("STEP=" + step.ToString() + "," + "AGV" + agv.id + " 衝突検知停止指示");
                                        orders.Add(agv.RouteCancelOrder());
                                    }

                                    state = enState.WAIT | enState.WAIT_CRASH;
                                    step = 610;
                                }
                                else
                                {
                                    //communicator.Log("AGV" + agv.id + " 衝突検知解消");
                                    step = 700;
                                }
                            }
                        }

                        #endregion
                        break;

                    case 610:
                        #region 衝突検知解除待機
                        {
                            RouteCondition nextCon = cur_con.next_condition;
                            if (nextCon != null)
                            {
                                if (sta.x == nextCon.Location.X && sta.y == nextCon.Location.Y)
                                {
                                    cur_con.UnlockWay(false);

                                    cur_con = nextCon;
                                    communicator.Log("STEP=" + step.ToString() + "," + "次移動地点取得[" + cur_con.ToString() + "]");
                                }
                            }

                            step = 600;
                        }
                        #endregion
                        break;

                    case 700:
                        #region 再開処理
                        {
                            if (cur_con.rack_down_arrive || cur_con.rack_down_departure || cur_con.rack_down_departure_last)
                            {
                                if (sta.sta_stop)
                                {
                                    if (agv.rack == null)
                                    {
                                        step = 0;
                                    }
                                }
                            }
                            else if (cur_con.rack_up_arrive || cur_con.rack_up_departure)
                            {
                                if (sta.sta_stop)
                                {
                                    if (agv.rack != null)
                                    {
                                        step = 0;
                                    }
                                }
                            }
                            else
                            {
                                if (sta.x != cur_con.Location.X || sta.y != cur_con.Location.Y)
                                {
                                    step = 800;
                                }
                                else if (((state & enState.WAIT) != 0) || agv_stop /*|| sta.sta_stop*/)
                                {
                                    step = 0;
                                }
                                else
                                {
                                    step = 800;
                                }
                            }
                        }
                        #endregion
                        break;

                    case 710:
                        #region 再開待機

                        if (!sta.sta_stop || agv_request)
                        {
                            step = 800;
                        }
                        else if (sta.x != cur_con.Location.X || sta.y != cur_con.Location.Y)
                        {
                            step = 800;
                        }
                        else if (5000 < swWaitTimer.ElapsedMilliseconds)
                        {
                            swWaitTimer.Stop();
                            step = 700;
                        }

                        #endregion
                        break;

                    case 800:
                        #region 移動処理

                        if (cur_con != null)
                        {
                            state = enState.MOVE;

                            RouteCondition nextCon = cur_con.next_condition;
                            if (nextCon != null)
                            {
                                if (sta.x == (int)(nextCon.Location.X + 0.5) && sta.y == (int)(nextCon.Location.Y + 0.5))
                                {
                                    cur_con.UnlockWay(false);

                                    cur_con = nextCon;
                                    communicator.Log("STEP=" + step.ToString() + "," + "次移動地点取得[" + cur_con.ToString() + "]");
                                    step = 0;
                                }
                                else
                                {
                                    if (sta.x == (int)(cur_con.Location.X + 0.5) && sta.y == (int)(cur_con.Location.Y + 0.5))
                                    {
                                        if (sta.sta_stop)
                                        {
                                            //orders.Add(ResumeOrder());
                                            step = 710;
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        RouteConditionList skipped_con = new RouteConditionList();

                                        RouteCondition con = nextCon.next_condition;
                                        while (con != null)
                                        {
                                            if (sta.x == (int)(con.Location.X + 0.5) && sta.y == (int)(con.Location.Y + 0.5))
                                            {
                                                cur_con.UnlockWay(false);
                                                nextCon.UnlockWay(false);

                                                foreach (var v in skipped_con)
                                                {
                                                    v.UnlockWay(false);
                                                }

                                                cur_con = con;
                                                communicator.Log("STEP=" + step.ToString() + "," + "次移動地点スキップ取得[" + cur_con.ToString() + "]");
                                                step = 0;

                                                break;
                                            }

                                            if (sta.sta_rack)
                                            {
                                                if (agv.rack == null && con.cur_qr.rack != null)
                                                {
                                                }
                                            }
                                            else
                                            {
                                                if (agv.rack != null && con.cur_qr.rack == null)
                                                {
                                                }
                                            }

                                            skipped_con.Add(con);
                                            con = con.next_condition;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (sta.sta_stop || agv_request)
                                {
                                    cur_con.UnlockWay(true);
                                    cur_con = null;
                                    step = 900;

                                    swWaitTimer.Restart();
                                }
                            }
                        }
                        else
                        {
                            step = 0;
                        }

                        #endregion
                        break;

                    case 900:
                        #region 作業完了応答確認
                        {
                            if (agv.in_autorator == null)
                            {
                                if (agv_request)
                                {
                                    bool allcomplete = true;
                                    foreach (var v in floor.controller.agvs)
                                    {
                                        if (v.floor != agv.floor) continue;
                                        if (v == agv) continue;

                                        if (!v.agvRunner.agv_request)
                                        {
                                            allcomplete = false;
                                            break;
                                        }
                                    }

                                    if (allcomplete)
                                    {
                                        communicator.Log("STEP=" + step.ToString() + "," + (moveModes.ContainsKey(moveMode_now) ? moveModes[moveMode_now].name : moveMode_now.ToString() + "完了"));

                                        //foreach (var v in manager.clients)
                                        //{
                                        //    string send = BL_EnumLabel.GetLabel(enCommand.MODE) + ((int)moveMode_now).ToString();
                                        //    communicator.Log("STEP=" + step.ToString() + "," + "S[" + send + "]");
                                        //    manager.Log("[" + agv.id + "] S[" + send + "]");
                                        //    v.Send(Encoding.ASCII.GetBytes(send));
                                        //}
                                    }

                                    step = 910;
                                }
                            }
                        }

                        #endregion
                        break;

                    case 910:
                        #region 全完了待機
                        {
                            bool allcomplete = true;
                            foreach (var v in floor.controller.agvs)
                            {
                                if (v.floor != floor) continue;

                                if (v == agv) continue;
                                if (v.agvRunner.Step == 0 && v.agvRunner.state == enState.STOP) continue;
                                if (!v.agvRunner.agv_request)
                                {
                                    allcomplete = false;
                                    break;
                                }
                            }

                            //非同期動作
                            if (moveModes[moveMode_now].option.Contains("ASYNC"))
                            {
                                //同一モード繰り返し
                                if (moveModes[moveMode_now].option.Contains("REPEAT"))
                                {
                                    async_repeat = true;

                                    //充電切れなら次モードへ
                                    if (moveModes[moveMode_now].option.Contains("LOWBAT_NEXT"))
                                    {
                                        if (sta.bat <= manager.BATTERY_LOW)
                                        {
                                            int nextmode = (moveMode_now + 1) % moveModes.Count;
                                            if (nextmode == 0) nextmode = 1;
                                            agv.moveMode = nextmode;
                                        }
                                    }

                                    step = 0;
                                }
                                //充電切れなら次モードへ
                                else if (moveModes[moveMode_now].option.Contains("LOWBAT_NEXT"))
                                {
                                    if (sta.bat <= manager.BATTERY_LOW)
                                    {
                                        int nextmode = (moveMode_now + 1) % moveModes.Count;
                                        if (nextmode == 0) nextmode = 1;
                                        agv.moveMode = nextmode;
                                        step = 0;
                                    }
                                    else
                                    {
                                        //繰り返さずに、モードなしへ移行する
                                        moveMode_now = agv.moveMode = 0;
                                        step = 10;
                                    }
                                }
                                //次モードへ
                                else if (moveModes[moveMode_now].option.Contains("NEXT"))
                                {
                                    async_repeat = true;

                                    int nextmode = (moveMode_now + 1) % moveModes.Count;
                                    if (nextmode == 0) nextmode = 1;
                                    agv.moveMode = nextmode;

                                    step = 0;
                                }
                                //モード終了⇒モードなしへ
                                else
                                {
                                    //繰り返さずに、モードなしへ移行する
                                    moveMode_now = agv.moveMode = 0;
                                    step = 10;
                                }
                            }
                            else
                            {
                                if (allcomplete)
                                {
                                    //同一モード繰り返し
                                    if (moveModes[moveMode_now].option.Contains("REPEAT"))
                                    {
                                        step = 0;
                                    }
                                    //充電切れなら次モードへ
                                    else if (moveModes[moveMode_now].option.Contains("LOWBAT_NEXT"))
                                    {
                                        if (sta.bat < 20)
                                        {
                                            int nextmode = (moveMode_now + 1) % moveModes.Count;
                                            if (nextmode == 0) nextmode = 1;
                                            agv.moveMode = nextmode;
                                            step = 0;
                                        }
                                        else
                                        {
                                            //繰り返さずに、モードなしへ移行する
                                            moveMode_now = agv.moveMode = 0;
                                            step = 10;
                                        }
                                    }
                                    //次モードへ
                                    else if (moveModes[moveMode_now].option.Contains("NEXT"))
                                    {
                                        int nextmode = (moveMode_now + 1) % moveModes.Count;
                                        if (nextmode == 0) nextmode = 1;
                                        agv.moveMode = nextmode;

                                        step = 0;
                                    }
                                    //モード終了⇒モードなしへ
                                    else
                                    {
                                        //繰り返さずに、モードなしへ移行する
                                        moveMode_now = agv.moveMode = 0;
                                        step = 10;
                                    }
                                }
                            }
                        }

                        #endregion
                        break;
                }

                if (0 < orders.Count)
                {
                    communicator.SetOrder(orders.ToArray());
                }
            }
        }
    }
}
