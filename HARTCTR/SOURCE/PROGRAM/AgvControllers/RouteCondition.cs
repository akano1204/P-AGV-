#define VER12

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using AgvController;

using BelicsClass.Common;
using BelicsClass.ObjectSync;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        #region 進路情報クラス

        public class RouteConditionList : SynchronizedList<RouteCondition>
        {
            public RouteConditionList()
                :base()
            {
            }

            public RouteConditionList(RouteConditionList list)
                : base()
            {
                foreach (var v in list)
                {
                    this.Add(v);
                }
            }

            public new RouteCondition this[int index]
            {
                get
                {
                    if (0 <= index && index < base.Count)
                    {
                        return base[index];
                    }
                    return null;
                }
                set
                {
                    if (0 <= index && index < base.Count)
                    {
                        base[index] = value;
                    }
                }
            }

            public List<FloorQR> ToQrList()
            {
                List<FloorQR> ret = new List<FloorQR>();
                for (RouteCondition v = this[0]; v != null; v = v.next_condition)
                {
                    ret.Add(v.cur_qr);
                }

                return ret;
            }

            public void Normalization()
            {
                for (int i = 0; i < Count; i++)
                {
                    this[i].next_condition = null;
                    if (i + 1 < Count) this[i].next_condition = this[i + 1];

                    this[i].prev_condition = null;
                    if (0 < i) this[i].prev_condition = this[i - 1];
                }
            }
        }

        public class CheckConditions
        {
            public enum enCheckPointType
            {
                EXCLUSION,
                FIFO,
            }

            public RouteCondition wait_condition = null;
            public RouteCondition check_condition = null;
            public CheckConditions next_check_condition = null;
            public CheckConditions prev_check_condition = null;

            public Dictionary<FloorQR, double> escape_points = new Dictionary<FloorQR, double>();

            public enCheckPointType check_point_type
            {
                get
                {
                    if (escape_points.Count == 0)
                    {
                        //回避不可チェックポイント（単独予約）

                        return enCheckPointType.EXCLUSION;
                    }
                    else
                    {
                        //回避可能チェックポイント（順番待ち予約）

                        return enCheckPointType.FIFO;
                    }

                    //if (check_condition != null && check_condition.prev_condition != null)
                    //{
                    //    if (check_condition.prev_condition.IsRoundWay(check_condition.cur_qr))
                    //    {
                    //        //双方向で進入

                    //    }
                    //    else
                    //    {
                    //        //一方通行で進入
                    //    }
                    //}

                    //return enCheckPointTYpe.UNKNOWN;
                }
            }


            public override string ToString()
            {
                if (wait_condition != null)
                {
                    return check_condition.cur_qr.ToString() + " W:" + wait_condition.cur_qr.ToString();
                }

                return check_condition.cur_qr.ToString() + "W:(non)";
            }
        }

        public class RouteCondition : BL_ObjectSync
        {
            #region フィールド

            public FloorAGV owner_agv = null;

            public bool selected = false;

            [BL_ObjectSync]
            public FloorQR cur_qr = null;

            public RouteCondition next_condition = null;
            public RouteCondition prev_condition = null;

            [BL_ObjectSync]
            public enSpeed speed = enSpeed.ML;

            [BL_ObjectSync]
            public bool agv_turn_arrive = false;
            [BL_ObjectSync]
            public int agv_turn_arrive_degree = 0;

            [BL_ObjectSync]
            public bool rack_turn_arrive = false;
            [BL_ObjectSync]
            public int rack_turn_arrive_degree = 0;

            [BL_ObjectSync]
            public bool rack_up_arrive = false;
            [BL_ObjectSync]
            public bool rack_down_arrive = false;

            public bool agv_turn_departure = false;
            public int agv_turn_departure_degree = 0;
            public bool rack_turn_departure = false;
            public int rack_turn_departure_degree = 0;
            public bool rack_up_departure = false;
            public bool rack_down_departure = false;
            public bool rack_down_departure_last = false;

            public int wait_timer = 0;
            public bool wait_station_trg = false;
            public bool wait_autorator_in_trg = false;
            public bool wait_autorator_out_trg = false;
            public bool wait_charge_trg = false;

            public bool wait_autorator_in_pretrg = false;

            public Dictionary<FloorAGV, PointF> wait_other_agv_locations = new Dictionary<FloorAGV, PointF>();

            [BL_ObjectSync]
            public bool rack_regulation = false;
            [BL_ObjectSync]
            public byte l_pat = 0;
            [BL_ObjectSync]
            public byte m_pat = 1;

            [BL_ObjectSync]
            public int distance = 0;

            public int cur_rack_degree
            {
                get
                {
                    if (owner_agv.rack != null)
                    {
                        for (RouteCondition cur = this; cur != null; cur = cur.prev_condition)
                        {
                            if (cur.rack_down_arrive) return 9999;
                            if (cur.rack_turn_arrive) return (cur.rack_turn_arrive_degree + cur.agv_turn_arrive_degree) % 360;

                            if (cur.Direction % 90 != 0)
                            {
                                return (cur.rack_turn_arrive_degree + cur.agv_turn_arrive_degree) % 360;
                            }
                            else if (cur.prev_condition != null && cur.prev_condition.Direction % 90 != 0)
                            {
                                return (cur.rack_turn_arrive_degree + cur.agv_turn_arrive_degree) % 360;
                            }
                        }

                        return (int)owner_agv.rack.degree;
                    }
                    else
                    {
                        return 9999;
                    }
                }
            }

            #endregion

            #region プロパティ

            public PointF Location { get { return cur_qr.Location; } }
            public string FloorCode { get { return cur_qr.floor.code; } }

            public override string ToString()
            {
                string s = cur_qr.ToString();
                if (0 < wait_other_agv_locations.Count) s += "{W}";
                //s += "[" + speed.ToString() + "]";
                s += " [";
                //if (agv_turn_arrive) s += "A" + agv_turn_arrive_degree.ToString();
                if (agv_turn_arrive)
                {
                    //s += "*";
                    s += "A" + agv_turn_arrive_degree.ToString();
                }
                //if (rack_turn_arrive) s += "R" + rack_turn_arrive_degree.ToString();
                if (rack_turn_arrive)
                {
                    //s += "*";
                    s += "R" + rack_turn_arrive_degree.ToString();
                }
                //if (rack_stay_turn_arrive) s += "S";
                if (rack_up_arrive) s += "U";
                if (rack_down_arrive) s += "D";
                s += "]";

                //s += " D[";
                //if (agv_turn_departure) s += "A" + agv_turn_departure_degree.ToString();
                //if (rack_turn_departure) s += "R" + rack_turn_departure_degree.ToString();
                ////if (rack_stay_turn_departure) s += "S";
                //if (rack_up_departure) s += "U";
                //if (rack_down_departure) s += "D";
                //if (rack_down_departure_last) s += "L";
                //s += "]";

                s += "<";
                if (rack_regulation) s += "G";
                if (wait_timer != 0) s += "T" + wait_timer.ToString();
                if (wait_station_trg) s += "S";
                if (wait_autorator_in_pretrg) s += "i";
                if (wait_autorator_in_trg) s += "I";
                if (wait_autorator_out_trg) s += "O";
                if (wait_charge_trg) s += "C";

                if (0 < owner_agv.check_conditions.Where(e => e.check_condition == this).Count()) s += "CP";
                if (0 < owner_agv.check_conditions.Where(e => e.wait_condition == this).Count()) s += "WP";

                s += ">";

                return s;
            }

            #endregion

            #region 進路選択

            public void Select(bool selected)
            {
                if (selected)
                {
                    foreach (var v in owner_agv.conditions)
                    {
                        v.selected = false;
                    }

                    this.selected = selected;

                    owner_agv.floor = owner_agv.floor.controller.SelectFloor(cur_qr.floor.code);
                    owner_agv.SetLocation("", Location);

                    //foreach (var kv in other_agv_locations)
                    //{
                    //    kv.Key.Location = kv.Value;
                    //}
                }
                else
                {
                }
            }

            #endregion

            #region AGV走行経路排他

            public RouteCondition LockWay()
            {
                FloorQR stopqr = null;
                if (0 < owner_agv.current_route.Count) stopqr = owner_agv.current_route.Last().cur_qr;

                bool found = false;
                RouteConditionList list = new RouteConditionList();
                foreach (var v in owner_agv.conditions)
                {
                    if (v.cur_qr == this.cur_qr) found = true;

                    if (found)
                    {
                        list.Add(v);
                    }

                    if (stopqr != null && v.cur_qr == stopqr) break;
                }

                int lock_distance = 375;
                if (owner_agv.agvRunner.manager.agvid_list.ContainsKey(owner_agv.id))
                {
                    lock_distance = owner_agv.agvRunner.manager.agvid_list[owner_agv.id].stop_distance;
                }

                int predeg = Direction;
                int locked_distance = 0;
                var conpre = list[0];
                for (int i = 0; i < list.Count; i++)
                {
                    var con = list[i];

                    if (0 < lock_distance && lock_distance <= locked_distance) break;

                    if (!con.cur_qr.Lock(owner_agv))
                    {
                        owner_agv.Unlock();
                        return con;
                    }

                    if (0 < lock_distance && predeg != con.Direction) break;
                    if (con != list[0] && con.cur_qr.escape_to != null) break;
                    if (con != list[0] && con.wait_autorator_in_trg) break;

                    locked_distance += (int)owner_agv.Distance(conpre.Location, con.Location);
                    conpre = con;
                    predeg = con.Direction;
                }

                return null;
            }

            public void UnlockWay(bool all)
            {
                if (!all)
                {
                    cur_qr.Unlock(owner_agv);
                }
                else
                {
                    owner_agv.Unlock();
                }
            }

            #region

            //public RouteCondition LockWay(int mode)
            //{
            //    int lockways = 4;

            //    RouteConditionList cons = new RouteConditionList();
            //    for (var c = this; c != null; c = c.next_condition) cons.Add(c);

            //    if (cons.Count == 0) return null;
            //    if (0 < wait_other_agv_locations.Count) return null;

            //    RouteCondition autorator_cur = null;

            //    int lock_count = 0;
            //    int change_degree = 0;
            //    RouteCondition con_pre = null;
            //    RouteCondition con = this;
            //    int deg_pre = 9999;
            //    RouteCondition locked_con = null;

            //    for (int ways = 0; ways <= lockways; ways++)
            //    {
            //        if (con.cur_qr.autorator_id.Trim() != "" && autorator_cur == null)
            //        {
            //            autorator_cur = con;
            //        }

            //        if (deg_pre != 9999 && con.next_condition != null)
            //        {
            //            double deg = con.cur_qr.Degree(con.next_condition.cur_qr);
            //            if (deg_pre != deg)
            //            {
            //                change_degree = 1;
            //            }
            //        }

            //        if (con.cur_qr.on_agv != null && con.cur_qr.on_agv != owner_agv)
            //        {
            //            locked_con = con;
            //            break;
            //        }
            //        if (con.cur_qr.lock_agv != null && con.cur_qr.lock_agv != owner_agv)
            //        {
            //            locked_con = con;
            //            break;
            //        }

            //        if (owner_agv.rack != null && con.cur_qr.rack != null)
            //        {
            //            locked_con = con;
            //            break;
            //        }

            //        if (ways < 3)
            //        {
            //            if (owner_agv.rack != null && con.cur_qr.rack_rotatable && (con.rack_turn_arrive || con.rack_turn_departure))
            //            {
            //                foreach (var v in con.cur_qr.rack_rotate_conflict)
            //                {
            //                    if (v.on_agv != null && v.on_agv != owner_agv /*&& v.on_agv.rack != null*/)
            //                    {
            //                        locked_con = con;
            //                        break;
            //                    }

            //                    if (v.lock_agv != null && v.lock_agv != owner_agv /*&& v.lock_agv.rack != null*/)
            //                    {
            //                        locked_con = con;
            //                        break;
            //                    }
            //                }

            //                if (locked_con != null) break;
            //            }
            //        }

            //        //if (0 < con.wait_other_agv_locations.Count) break;
            //        //if (con.wait_station_trg) break;
            //        //if (con.wait_autorator_in_trg) break;
            //        //if (con.wait_autorator_out_trg) break;
            //        //if (con.wait_charge_trg) break;
            //        if (con.agv_turn_arrive || con.agv_turn_departure) break;
            //        if (con != this && con.rack_turn_arrive || con.rack_turn_departure) break;
            //        if (con.rack_down_arrive || con.rack_down_departure || con.rack_down_departure_last) break;
            //        if (con.rack_up_arrive || con.rack_up_departure) break;

            //        lock_count++;

            //        if (0 < change_degree)
            //        {
            //            //if (1 < change_degree) break;
            //            //change_degree++;
            //            break;
            //        }

            //        con_pre = con;
            //        con = con.next_condition;

            //        if (con == null)
            //        {
            //            //周回する
            //            if (owner_agv.on_qr == this.cur_qr && owner_agv.moveMode != 0) con = this;
            //            if (con == null) break;
            //        }
            //        else
            //        {
            //            deg_pre = (int)con_pre.cur_qr.Degree(con.cur_qr);
            //        }
            //    }

            //    if (locked_con == null)
            //    {
            //        //棚を持っている場合、ルート上の棚の配置状態を確認
            //        if (owner_agv.rack != null)
            //        {
            //            var rack_cons = cons.Where(e => e.cur_qr.rack != null);
            //            if (0 < rack_cons.Count())
            //            {
            //                locked_con = rack_cons.First();
            //            }
            //        }
            //    }

            //    if (locked_con == null && con != null)
            //    {
            //        if (autorator_cur != null)
            //        {
            //            var arqrs = owner_agv.floor.controller.AllAutoratorQR.Where(e => e.autorator_id == con.cur_qr.autorator_id).ToList();
            //            if (0 < arqrs.Where(e => e.lock_agv != null && e.lock_agv != owner_agv).Count())
            //            {
            //                locked_con = autorator_cur;
            //            }
            //            else
            //            {
            //                int ar_count = 0;
            //                for (var c = autorator_cur; c != null; c = c.next_condition)
            //                {
            //                    ar_count++;
            //                    if (2 < ar_count) break;

            //                    if (c.cur_qr.lock_agv != null && c.cur_qr.lock_agv != owner_agv)
            //                    {
            //                        locked_con = autorator_cur;
            //                        break;
            //                    }
            //                }
            //            }
            //        }
            //    }

            //    if (locked_con != null)
            //    {
            //        if (owner_agv.agvRunner != null && owner_agv.agvRunner.request)
            //        {
            //            owner_agv.agvRunner.Log("ルート解放1");
            //            UnlockWay(true);
            //        }
            //        else if (owner_agv.agvRunner != null)
            //        {
            //            owner_agv.agvRunner.Log("走行中ルート解放拒否");
            //        }

            //        return locked_con;
            //    }


            //    lock_count = 0;
            //    change_degree = 0;
            //    con_pre = null;
            //    con = this;
            //    autorator_cur = null;
            //    deg_pre = 9999;

            //    for (int ways = 0; ways <= lockways; ways++)
            //    {
            //        if (con.cur_qr.autorator_id.Trim() != "" && autorator_cur == null)
            //        {
            //            autorator_cur = con;
            //        }

            //        if (deg_pre != 9999 && con.next_condition != null)
            //        {
            //            double deg = con.cur_qr.Degree(con.next_condition.cur_qr);
            //            if (deg_pre != deg)
            //            {
            //                change_degree = 1;
            //            }
            //        }

            //        con.cur_qr.lock_agv = owner_agv;

            //        if (ways < 3)
            //        {
            //            //棚旋回可能QRの150cmピッチ以下のQRを排他する
            //            if (owner_agv.rack != null && con.cur_qr.rack_rotatable && (con.rack_turn_arrive || con.rack_turn_departure))
            //            {
            //                foreach (var v in con.cur_qr.rack_rotate_conflict)
            //                {
            //                    v.lock_agv = owner_agv;
            //                }
            //            }
            //        }

            //        //if (0 < con.wait_other_agv_locations.Count) break;
            //        //if (con.wait_station_trg) break;
            //        //if (con.wait_autorator_in_trg) break;
            //        //if (con.wait_autorator_out_trg) break;
            //        //if (con.wait_charge_trg) break;
            //        if (con.agv_turn_arrive || con.agv_turn_departure) break;
            //        if (con != this && con.rack_turn_arrive || con.rack_turn_departure) break;
            //        if (con.rack_down_arrive || con.rack_down_departure || con.rack_down_departure_last) break;
            //        if (con.rack_up_arrive || con.rack_up_departure) break;

            //        lock_count++;

            //        if (0 < change_degree)
            //        {
            //            //if (1 < change_degree) break;
            //            //change_degree++;
            //            break;
            //        }

            //        con_pre = con;
            //        con = con.next_condition;

            //        if (con == null)
            //        {
            //            //周回する
            //            if (owner_agv.on_qr == this.cur_qr && owner_agv.moveMode != 0) con = this;
            //            if (con == null) break;
            //        }
            //        else
            //        {
            //            deg_pre = (int)con_pre.cur_qr.Degree(con.cur_qr);
            //        }
            //    }

            //    if (autorator_cur != null && con != null)
            //    {
            //        var arqrs = owner_agv.floor.controller.AllAutoratorQR.Where(e => e.autorator_id == con.cur_qr.autorator_id).ToList();
            //        arqrs.ForEach(e => e.lock_agv = owner_agv);

            //        int ar_count = 0;
            //        for (var c = autorator_cur; c != null; c = c.next_condition)
            //        {
            //            ar_count++;
            //            if (2 < ar_count) break;

            //            c.cur_qr.lock_agv = owner_agv;
            //        }
            //    }

            //    if (owner_agv.floor.controller.EventPaint != null) owner_agv.floor.controller.EventPaint();
            //    return null;
            //}

            //public void UnlockWay(bool all)
            //{
            //    bool repaint = false;

            //    if (!all)
            //    {
            //        if (next_condition != null && next_condition.cur_qr.rack_rotatable && (next_condition.rack_turn_arrive || next_condition.rack_turn_departure))
            //        {
            //        //    cur_qr.lock_agv = owner_agv;
            //            return;
            //        }

            //        if (cur_qr.rack_rotatable && (rack_turn_arrive || rack_turn_departure))
            //        {
            //            foreach (var v in cur_qr.rack_rotate_conflict)
            //            {
            //                if (v.lock_agv == owner_agv)
            //                {
            //                    v.lock_agv = null;
            //                    repaint = true;
            //                }
            //            }
            //        }

            //        if (cur_qr.lock_agv == owner_agv)
            //        {
            //            if (cur_qr.autorator_id != "")
            //            {
            //                owner_agv.floor.controller.AllAutoratorQR.Where(e => e.autorator_id == cur_qr.autorator_id).ToList().ForEach(e => e.lock_agv = null);
            //                repaint = true;
            //            }

            //            cur_qr.lock_agv = null;
            //            repaint = true;
            //        }
            //    }
            //    else
            //    {
            //        var lockqrs = owner_agv.floor.controller.AllQR.Where(e => e.lock_agv == owner_agv).ToList();

            //        //if (0 == lockqrs.Where(e => e.autorator_id != "").Count())
            //        if (owner_agv.agvRunner == null || (owner_agv.in_autorator == null && owner_agv.out_autorator == null))
            //        {
            //            foreach (var v in lockqrs)
            //            {
            //                //全オートレーターをロック解除
            //                owner_agv.floor.controller.AllAutoratorQR.Where(e => e.autorator_id == v.autorator_id).ToList().ForEach(e => e.lock_agv = null);
            //                v.lock_agv = null;

            //                repaint = true;
            //            }
            //        }

            //        owner_agv.on_qr.on_agv = owner_agv;

            //        //for (var c = this; c != null; c = c.next_condition)
            //        //{
            //        //    if (c.cur_qr.lock_agv == owner_agv)
            //        //    {
            //        //        if (cur_qr.rack_rotatable)
            //        //        {
            //        //            foreach (var v in cur_qr.rack_rotate_conflict)
            //        //            {
            //        //                if (v.lock_agv == owner_agv)
            //        //                {
            //        //                    v.lock_agv = null;
            //        //                }
            //        //            }
            //        //        }
            //        //
            //        //        if (c.cur_qr.autorator_id != "")
            //        //        {
            //        //            //全オートレーターをロック解除
            //        //            owner_agv.floor.controller.AllAutoratorQR.Where(e => e.autorator_id == c.cur_qr.autorator_id).ToList().ForEach(e => e.lock_agv = null);
            //        //        }
            //        //
            //        //        repaint = true;
            //        //        c.cur_qr.lock_agv = null;
            //        //    }
            //        //}
            //        //
            //        //owner_agv.floor.controller.AllQR.Where(e => e.lock_agv == owner_agv).ToList().ForEach(e => e.lock_agv = null);
            //    }

            //    if (repaint)
            //    {
            //        if (owner_agv.floor.controller.EventPaint != null) owner_agv.floor.controller.EventPaint();
            //    }
            //}

            #endregion

            #endregion

            #region 各種検出処理

            /// <summary>
            /// 次のカーブまでの残り直線距離
            /// </summary>
            /// <returns></returns>
            public double RemainStraightDistance()
            {
                if (next_condition == null) return 0;

                double distance = 0;
                int degree = (int)(owner_agv.Degree(cur_qr.Location, next_condition.cur_qr.Location));

                for (var v = this.next_condition; v.next_condition != null; v = v.next_condition)
                {
                    distance += owner_agv.Distance(v.cur_qr.Location, v.next_condition.cur_qr.Location);

                    int deg = (int)(owner_agv.Degree(v.cur_qr.Location, v.next_condition.cur_qr.Location));
                    if (deg != degree) break;
                }

                return distance;
            }

            public FloorQR EscapePoint(RouteCondition avoidCon)
            {
                if (next_condition == null) return null;

                List<FloorQR> avoid_qrs = new List<FloorQR>();
                for (var v = avoidCon; avoidCon.next_condition != null; v = v.next_condition)
                {
                    avoid_qrs.Add(v.cur_qr);
                }

                List<FloorQR> conflict_qrs = new List<FloorQR>();
                for (var v = this.next_condition; v.next_condition != null; v = v.next_condition)
                {
                    if (avoid_qrs.Contains(v.cur_qr))
                    {
                        conflict_qrs.Add(v.cur_qr);
                    }
                }

                FloorQR escape_qr = null;
                foreach (var v in conflict_qrs)
                {
                    foreach (var vv in v.next_way)
                    {
                        if (!avoid_qrs.Contains(vv.Key))
                        {
                            escape_qr = vv.Key;
                            break;
                        }
                    }
                }

                return escape_qr;
            }

            public int Direction
            {
                get
                {
                    //if (prev_condition != null)
                    //{
                    //    return (int)parent.Degree(prev_condition.Location, Location);
                    //}
                    //else 
                    if (next_condition != null)
                    {
                        return (int)owner_agv.Degree(Location, next_condition.Location);
                    }
                    
                    return -999;
                }
            }

            public RouteConditionList StraightRoute_Reverse
            {
                get
                {
                    RouteConditionList ret = new RouteConditionList();

                    if (prev_condition == null) return ret;
                    RouteCondition con = prev_condition;
                    int degree = (int)owner_agv.Degree(con.Location, con.next_condition.Location);

                    for (; con.prev_condition != null; con = con.prev_condition)
                    {
                        ret.Add(con);

                        int deg = (int)owner_agv.Degree(con.prev_condition.Location, con.Location);
                        if (deg != degree) break;
                    }

                    return ret;
                }
            }

            public RouteConditionList StraightRoute
            {
                get
                {
                    RouteConditionList ret = new RouteConditionList();

                    if (next_condition == null) return ret;
                    RouteCondition con = this;
                    int degree = (int)owner_agv.Degree(con.Location, con.next_condition.Location);

                    for (; con != null; con = con.next_condition)
                    {
                        ret.Add(con);

                        if (con.next_condition != null)
                        {
                            int deg = (int)owner_agv.Degree(con.Location, con.next_condition.Location);
                            if (deg != degree) break;
                        }
                    }

                    return ret;
                }
            }

            /// <summary>
            /// 現ポイントから最大棚サイズ分先にあるポイントの直線上にあるオートレーターQRを取得する
            /// </summary>
            /// <returns></returns>
            public List<FloorQR> RoutingAutoratorQR()
            {
                return RoutingAutoratorQR(RackMaster.RackSizeMax);
            }

            /// <summary>
            /// 現ポイントからｎcm先の直線上にあるオートレーターQRを取得する
            /// </summary>
            public List<FloorQR> RoutingAutoratorQR(int distance)
            {
                RouteConditionList ahead_cons = new RouteConditionList();

                double run_distance = 0;
                for (var c = this; c != null; c = c.next_condition)
                {
                    ahead_cons.Add(c);

                    if(c != this) run_distance += owner_agv.Distance(c.prev_condition.Location, c.Location);
                    if (distance <= run_distance) break;
                }

                foreach (var ahead_con in ahead_cons)
                {
                    var straight_route = ahead_con.StraightRoute.GetRange(0, System.Math.Min(3, ahead_con.StraightRoute.Count));
                    var in_autorator_con = straight_route.Where(e => e.cur_qr.autorator_id != "").FirstOrDefault();
                    if (in_autorator_con != null)
                    {
                        var out_autorator_con = in_autorator_con.next_condition;

                        if (out_autorator_con != null)
                        {
                            //in_autorator_con.cur_qr.Lock(owner_agv);
                            //out_autorator_con.cur_qr.Lock(owner_agv);

                            if (in_autorator_con.cur_qr.IsLockOtherAgv(owner_agv) || out_autorator_con.cur_qr.IsLockOtherAgv(owner_agv))
                            {
                                //オートレーターは他のAGVがロックしている
                                return new List<FloorQR>();
                            }
                            else
                            {
                                //自AGVでロックするオートレーターQRを列挙
                                return (new FloorQR[] { in_autorator_con.cur_qr, out_autorator_con.cur_qr }).ToList();
                            }
                        }
                        else
                        {
                            return new List<FloorQR>();
                        }
                    }
                }

                //オートレーターは存在しない
                return null;
            }

            public List<FloorQR> PrevRoutingAutoratorQR
            {
                get
                {
                    RouteConditionList lockcons = new RouteConditionList();

                    //for (var c = this; c != null; c = c.next_condition)
                    //{
                    //    if (250 < owner_agv.Distance(c.cur_qr.Location)) break;

                    //    if (c.cur_qr.lock_agv == owner_agv)
                    //    {
                    //        lockcons.Add(c);
                    //    }
                    //    else if (0 < lockcons.Count) break;
                    //}

                    lockcons.Add(this);

                    foreach (var lockcon in lockcons)
                    {
                        var straight_route = lockcon.StraightRoute_Reverse.GetRange(0, System.Math.Min(2, lockcon.StraightRoute_Reverse.Count));
                        var autorator_cons = straight_route.Where(e => e.cur_qr.autorator_id != "").ToList();
                        if (0 < autorator_cons.Count())
                        {
                            var autorator_con = autorator_cons.First();
                            List<FloorQR> locked_qrs = this.owner_agv.floor.controller.AllAutoratorQR.Where(e => e.autorator_id == autorator_con.cur_qr.autorator_id).ToList();
                            if (0 < locked_qrs.Where(e => e.IsLockOtherAgv(owner_agv)).Count())
                            {
                                //オートレーターは他のAGVがロックしている
                                return new List<FloorQR>();
                            }
                            else
                            {
                                //自AGVでロックするオートレーターQRを列挙
                                return locked_qrs;
                            }
                        }
                    }

                    //オートレーターは存在しない
                    return null;
                }
            }

            /// <summary>
            /// 双方向移動可能かチェック
            /// </summary>
            /// <param name="stop_qr"></param>
            /// <returns></returns>
            public bool IsRoundWay(FloorQR stop_qr)
            {
                bool roundway = true;

                for (RouteCondition v = this; v != null; v = v.next_condition)
                {
                    if (stop_qr == v.cur_qr) break;

                    if (v.next_condition != null)
                    {
                        if (v.next_condition.cur_qr.next_way.Keys.Contains(v.cur_qr) ||
                            (v.cur_qr.autorator_id != "" && v.cur_qr.autorator_id == v.next_condition.cur_qr.autorator_id))
                        {
                            //戻れるルート
                        }
                        else
                        {
                            roundway = false;
                            break;
                        }
                    }
                }

                return roundway;
            }

            //public bool CanReserve(FloorAGV agv, FloorQR stop_qr)
            //{
            //    bool canreserve = true;

            //    do
            //    {
                    
            //        try
            //        {
            //            bool roundway = this.IsRoundWay(stop_qr);

            //            if (stop_qr.on_agv != null && stop_qr.on_agv != agv && roundway)
            //            {
            //                //他AGVがチェックポイント上にいる(3)
            //                canreserve = false;
            //            }
            //            else
            //            {
            //                for (RouteCondition v = this; v != null; v = v.next_condition)
            //                {
            //                    if (stop_qr == v.cur_qr && v.next_condition != null) break;

            //                    FloorAGV otheragv = v.cur_qr.OnOtherAgv(agv);
            //                    if (otheragv != null)
            //                    {
            //                        //ルート上に他AGVを検出

            //                        if (agv.check_point_con != null && otheragv.check_point_con == agv.check_point_con && roundway)
            //                        {
            //                            //同じチェックポイントを目指す(2)
            //                            canreserve = false;
            //                        }
            //                        else if (otheragv.req == null)
            //                        {
            //                            //他AGVが指示を持っていない(4)
            //                            canreserve = false;
            //                        }
            //                        else if (otheragv.on_qr.station_id.Trim() != "" && roundway)
            //                        {
            //                            //他AGVがST上にいる(5)
            //                            canreserve = false;
            //                        }
            //                        else if (0 < otheragv.current_route.Where(e => e.cur_qr == agv.on_qr).Count())
            //                        {
            //                            //自分の現在値を含む(1)
            //                            canreserve = false;
            //                        }
            //                    }
            //                }
            //            }
            //        }
            //        catch
            //        {
            //            continue;
            //        }
            //    }
            //    while (false);

            //    return canreserve;
            //}

            #endregion

            #region 通信コマンド取得

            public AgvController.AgvCommunicator.Order[] GetOrders()
            {
                List<AgvController.AgvCommunicator.Order> orders = new List<AgvController.AgvCommunicator.Order>();
                AgvController.AgvCommunicator.Order order = null;

                #region 到着時旋回処理

                if (agv_turn_arrive || rack_turn_arrive)
                {
                    owner_agv.degree = agv_turn_arrive_degree;

                    order = new AgvController.AgvCommunicator.Order();
                    order.x = (int)Location.X;
                    order.y = (int)Location.Y;
                    order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;

                    if (agv_turn_arrive)
                    {
                        order.mod_agv_rorate = true;
                        order.deg = (short)DegreeAGV(agv_turn_arrive_degree);
                    }

                    if (rack_turn_arrive)
                    {
                        if (owner_agv.rack != null)
                        {
                            owner_agv.rack.degree = rack_turn_arrive_degree;
                            order.mod_rack_rorate = true;
                            order.rack_deg = (short)DegreeRack(owner_agv.rack.degree);
                        }
                    }

                    SetUncalibratable(cur_qr, order);
                    orders.Add(order);
                }

                #endregion

                #region 到着時棚操作

                if (rack_up_arrive)
                {
                    order = new AgvController.AgvCommunicator.Order();
                    order.x = (int)Location.X;
                    order.y = (int)Location.Y;
                    order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                    order.mod_rack_up = true;
                    orders.Add(order);

                    owner_agv.RackUp(cur_qr);

                    order = new AgvController.AgvCommunicator.Order();
                    order.x = (int)Location.X;
                    order.y = (int)Location.Y;
                    order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                    order.stop = true;
                    SetUncalibratable(cur_qr, order);
                    orders.Add(order);
                }
                else if (rack_down_arrive)
                {
                    order = new AgvController.AgvCommunicator.Order();
                    order.x = (int)Location.X;
                    order.y = (int)Location.Y;
                    order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                    order.mod_rack_down = true;
                    orders.Add(order);

                    owner_agv.RackDown(cur_qr);

                    order = new AgvController.AgvCommunicator.Order();
                    order.x = (int)Location.X;
                    order.y = (int)Location.Y;
                    order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                    order.stop = true;
                    SetUncalibratable(cur_qr, order);
                    orders.Add(order);
                }

                #endregion

                #region 条件成立待機

                if (wait_charge_trg)
                {
                    if (cur_qr.direction_charge != FloorQR.enDirection.NONE)
                    {
                        order = new AgvController.AgvCommunicator.Order();
                        order.x = (int)Location.X;
                        order.y = (int)Location.Y;
                        order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                        order.mod_charge = true;
                        SetUncalibratable(cur_qr, order);
                        orders.Add(order);
                    }
                }

                if (wait_autorator_in_trg || wait_autorator_out_trg || wait_station_trg || 0 < wait_timer || wait_charge_trg)
                {
                    order = new AgvController.AgvCommunicator.Order();
                    order.x = (int)Location.X;
                    order.y = (int)Location.Y;
                    order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                    order.stop = true;
                    SetUncalibratable(cur_qr, order);
                    orders.Add(order);
                }

                #endregion

                #region 出発時棚操作

                if (rack_up_departure)
                {
                    order = new AgvController.AgvCommunicator.Order();
                    order.x = (int)Location.X;
                    order.y = (int)Location.Y;
                    order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                    order.mod_rack_up = true;

                    SetUncalibratable(cur_qr, order);
                    orders.Add(order);
                    
                    owner_agv.RackUp(cur_qr);
                }
                else if (rack_down_departure)
                {
                    order = new AgvController.AgvCommunicator.Order();
                    order.x = (int)Location.X;
                    order.y = (int)Location.Y;
                    order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                    order.mod_rack_down = true;

                    SetUncalibratable(cur_qr, order);
                    orders.Add(order);
                    
                    owner_agv.RackDown(cur_qr);
                }

                #endregion

                #region 出発時旋回処理

                if (agv_turn_departure || rack_turn_departure)
                {
                    owner_agv.degree = agv_turn_departure_degree;

                    order = new AgvController.AgvCommunicator.Order();
                    order.x = (int)Location.X;
                    order.y = (int)Location.Y;
                    order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;

                    if (agv_turn_departure)
                    {
                        order.mod_agv_rorate = true;
                        order.deg = (short)DegreeAGV(agv_turn_departure_degree);
                    }

                    if (rack_turn_departure)
                    {
                        if (owner_agv.rack != null /*&& cur_qr.rack_rotatable*/)
                        {
                            if (owner_agv.rack.degree != rack_turn_departure_degree)
                            {
                                owner_agv.rack.degree = rack_turn_departure_degree;
                                order.mod_rack_rorate = true;
                                order.rack_deg = (short)DegreeRack(owner_agv.rack.degree);
                            }
                        }
                    }

                    SetUncalibratable(cur_qr, order);
                    orders.Add(order);
                }

                #endregion

                #region 出発直前に棚下降

                if (rack_down_departure_last && owner_agv.rack != null)
                {
                    owner_agv.RackDown(cur_qr);

                    order = new AgvController.AgvCommunicator.Order();
                    order.x = (int)Location.X;
                    order.y = (int)Location.Y;
                    order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                    order.mod_rack_down = true;
                    SetUncalibratable(cur_qr, order);
                    orders.Add(order);

                    order = new AgvController.AgvCommunicator.Order();
                    order.x = (int)Location.X;
                    order.y = (int)Location.Y;
                    order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                    order.stop = true;
                    SetUncalibratable(cur_qr, order);
                    orders.Add(order);
                }

                #endregion

                #region 移動指示

                if (next_condition != null)
                {
                    if (0 < wait_other_agv_locations.Count)
                    {
                        order = new AgvController.AgvCommunicator.Order();
                        order.x = (int)Location.X;
                        order.y = (int)Location.Y;
                        order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                        order.stop = true;
                        SetUncalibratable(cur_qr, order);
                        orders.Add(order);
                    }

                    if (Location != next_condition.Location)
                    {
                        order = new AgvController.AgvCommunicator.Order();
                        order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                        order.x = (int)Location.X;
                        order.y = (int)Location.Y;
                        order.deg = (short)DegreeAGV(owner_agv.Degree(Location, next_condition.Location));

                        owner_agv.point = next_condition.Location;

                        if (next_condition.cur_qr.direction_charge != FloorQR.enDirection.NONE)
                        {
                            order.mod_agv_back = cur_qr.charge_back;
                        }

                        SetSpeed(order);
                        SetUncalibratable(cur_qr, order);
                        orders.Add(order);
                    }
                }
                else
                {
                    order = new AgvController.AgvCommunicator.Order();
                    order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;

                    order.x = (int)Location.X;
                    order.y = (int)Location.Y;

                    owner_agv.point = Location;

                    if (cur_qr.direction_charge != FloorQR.enDirection.NONE)
                    {
                        order.mod_charge = true;
                    }

                    SetUncalibratable(cur_qr, order);
                    orders.Add(order);
                }

                #endregion

                return orders.ToArray();
            }

            public AgvController.AgvCommunicator.Order[] GetOrders2(RouteCondition stop_con = null)
            {
                List<AgvController.AgvCommunicator.Order> orders = new List<AgvController.AgvCommunicator.Order>();
                RouteConditionList rclist = new RouteConditionList();
                AgvController.AgvCommunicator.Order order = null;

                for (RouteCondition c = this; c != null; c = c.next_condition)
                {
                    c.speed = c.cur_qr.speed_limit;

                    rclist.Add(c);
                    if (stop_con != null && c.cur_qr == stop_con.cur_qr) break;
                }

                short degree_cur = 999;
                FloorQR qr_prev = cur_qr;
                int pitch_cur = 125;
                enSpeed speed_pre = (enSpeed)0;
                //short degree_pre = 999;
                short rack_degree_pre = 999;

                foreach (RouteCondition c in rclist)
                {
                    bool is_pre_autorator = c.cur_qr.autorator_id != "";

                    RouteCondition cn = c.next_condition;
                    if (stop_con != null && c.cur_qr == stop_con.cur_qr) cn = null;

                    #region オートレーター周辺の移動速度をMにする

                    if (cn != null && !is_pre_autorator) is_pre_autorator = cn.cur_qr.autorator_id != "";

                    //RouteCondition cnn = null; if (cn != null) cnn = cn.next_condition;
                    //if (cnn != null && !is_pre_autorator) is_pre_autorator = cnn.cur_qr.autorator_id != "";

                    RouteCondition cp = c.prev_condition;
                    if (cp != null && !is_pre_autorator) is_pre_autorator = cp.cur_qr.autorator_id != "";

                    RouteCondition cpp = null; if (cp != null) cpp = cp.prev_condition;
                    if (cpp != null && !is_pre_autorator) is_pre_autorator = cpp.cur_qr.autorator_id != "";

                    if (is_pre_autorator) c.speed = enSpeed.M;
                    
                    #endregion

                    if (cn == null)
                    {
                        if (degree_cur != 999)
                        {
                            //目的地までの移動指示
                            order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, false, degree_cur, false, false, false, (short)c.rack_turn_arrive_degree, c.cur_qr.charge_back, false, c.rack_up_arrive || c.rack_up_departure);
                            //SetUncalibratable(qr_prev, order);
                            order.speed = (System.UInt16)speed_pre;
                            if (order.speed == 0) SetSpeed(order);
                            orders.Add(order);
                        }

                        if (c.wait_charge_trg)
                        {
                            //充電指示
                            order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, false, degree_cur, false, false, false, (short)c.rack_turn_arrive_degree, false, true, false);
                            //SetUncalibratable(c.cur_qr, order);
                            orders.Add(order);
                        }
                        else if (c.rack_down_arrive || c.rack_down_departure || c.rack_down_departure_last)
                        {
                            if (c.rack_turn_arrive)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                      , c.Location, false, degree_cur, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_arrive_degree, false, false, false);
                                //SetUncalibratable(c.cur_qr, order);
                                orders.Add(order);
                            }
                            else if (c.rack_turn_departure)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                      , c.Location, false, degree_cur, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_departure_degree, false, false, false);
                                //SetUncalibratable(c.cur_qr, order);
                                orders.Add(order);
                            }

                            //棚下降
                            order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, false, degree_cur, false, true, false, (short)c.rack_turn_arrive_degree, false, false, false);
                            //SetUncalibratable(c.cur_qr, order);
                            orders.Add(order);
                        }
                        else if (c.rack_up_arrive || c.rack_up_departure)
                        {
                            //棚上昇
                            order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, false, degree_cur, true, false, false, (short)c.rack_turn_arrive_degree, false, false, c.rack_up_arrive || c.rack_up_departure);
                            //SetUncalibratable(c.cur_qr, order);
                            orders.Add(order);

                            if (c.rack_turn_arrive)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                      , c.Location, false, degree_cur, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_arrive_degree, false, false, c.rack_up_arrive || c.rack_up_departure);
                                //SetUncalibratable(c.cur_qr, order);
                                orders.Add(order);
                            }
                            else if (c.rack_turn_departure)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                      , c.Location, false, degree_cur, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_departure_degree, false, false, c.rack_up_arrive || c.rack_up_departure);
                                //SetUncalibratable(c.cur_qr, order);
                                orders.Add(order);
                            }
                        }

                        if (order != null)
                        {
                            if (1 < rclist.Count)
                            {
                                if (rclist[0].cur_qr.autorator_id != "" || rclist[1].cur_qr.autorator_id != "")
                                {
                                    if (1 < orders.Count)
                                    {
                                        order.dist = (uint)owner_agv.Distance(orders[orders.Count - 1].Location, c.Location);
                                    }
                                    else
                                    {
                                        if (rclist.First().FloorCode != rclist.Last().FloorCode)
                                        {
                                            order.dist = (uint)owner_agv.Distance(rclist[1].Location, c.Location);
                                        }
                                        else
                                        {
                                            order.dist = (uint)owner_agv.Distance(this.Location, c.Location);
                                        }
                                    }
                                }
                                else if (0 < rclist.Count && rclist[rclist.Count - 1].cur_qr.autorator_id != "")
                                {
                                    FloorQR realqr = rclist[rclist.Count - 1].cur_qr.autorator_info.RealQR();
                                    if (realqr != rclist[rclist.Count - 1].cur_qr)
                                    {
                                        order.Location = realqr.Location;

                                        if (1 < orders.Count)
                                        {
                                            order.dist = (uint)owner_agv.Distance(orders[0].Location, c.Location);
                                        }
                                        else
                                        {
                                            order.dist = (uint)owner_agv.Distance(this.Location, c.Location);
                                        }
                                    }
                                }
                            }
                        }

                        break;
                    }
                    else
                    {
                        if (c == rclist[0] && c.FloorCode != cn.FloorCode)
                        {

                        }

                        int pitch_next = c.FloorCode == cn.FloorCode ? (int)owner_agv.Distance(c.Location, cn.Location) : 0;

                        short degree_next = (short)owner_agv.Degree(c.Location, cn.Location);
                        if (c.cur_qr.autorator_id != "" && cn.cur_qr.autorator_id != "")
                        {
                            //オートレーターの中の場合、直前の角度を維持
                            degree_next = degree_cur;
                        }
                        
                        if (c.prev_condition != null && c.prev_condition.cur_qr.autorator_id == "" && c.cur_qr.autorator_id != "")
                        {
                            FloorQR realqr = c.cur_qr;
                            if (c.cur_qr.autorator_info != null)
                            {
                                realqr = c.cur_qr.autorator_info.RealQR();
                                if (realqr == null) realqr = c.cur_qr;
                            }

                            //目的地までの移動指示
                            order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                              , realqr.Location, (degree_cur != degree_next), degree_next, false, false, false, (short)c.rack_turn_arrive_degree, false, false, false);
                            //SetUncalibratable(qr_prev, order);
                            order.speed = (System.UInt16)speed_pre;
                            if (order.speed == 0) SetSpeed(order);

                            if (realqr != c.cur_qr)
                            {
                                if (0 < orders.Count)
                                {
                                    order.dist = (uint)owner_agv.Distance(orders[orders.Count - 1].Location, c.Location);
                                }
                                else
                                {
                                    order.dist = (uint)owner_agv.Distance(this.Location, c.Location);
                                }
                            }

                            orders.Add(order);

                            break;
                        }
                        else if (c.wait_autorator_in_trg && c != this && c.owner_agv.in_autorator == null)
                        {
                            //目的地までの移動指示
                            order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                              , c.Location, false, degree_cur, false, false, false, (short)c.rack_turn_arrive_degree, false, false, false);
                            //SetUncalibratable(qr_prev, order);
                            order.speed = (System.UInt16)speed_pre;
                            if (order.speed == 0) SetSpeed(order);
                            orders.Add(order);

                            break;
                        }
                        else if (c.rack_up_arrive || c.rack_up_departure)
                        {
                            //目的地までの移動指示
                            if (degree_cur != 999)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, (degree_cur != degree_next), degree_next, false, false, false, (short)c.rack_turn_arrive_degree, false, false, c.rack_up_arrive || c.rack_up_departure);
                                //SetUncalibratable(qr_prev, order);
                                order.speed = (System.UInt16)speed_pre;
                                if (order.speed == 0) SetSpeed(order);
                                orders.Add(order);
                            }

                            //次ポイントは旋回が必要？
                            if (degree_cur != degree_next)
                            {
                                //先に方向転換を指示する
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                      , c.Location, true, degree_next, false, false, false, (short)c.rack_turn_arrive_degree, false, false, c.rack_up_arrive || c.rack_up_departure);
                                //SetUncalibratable(c.cur_qr, order);
                                orders.Add(order);
                            }

                            //棚上昇
                            order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, false, degree_next, true, false, false, (short)c.rack_turn_arrive_degree, false, false, c.rack_up_arrive || c.rack_up_departure);
                            //SetUncalibratable(c.cur_qr, order);
                            orders.Add(order);

                            if (c.rack_turn_arrive)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                      , c.Location, (degree_cur != degree_next), degree_next, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_arrive_degree, false, false, false);
                                //SetUncalibratable(c.cur_qr, order);
                                orders.Add(order);
                            }
                            else if (c.rack_turn_departure)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                      , c.Location, (degree_cur != degree_next), degree_next, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_departure_degree, false, false, false);
                                //SetUncalibratable(c.cur_qr, order);
                                orders.Add(order);
                            }

                            break;
                        }
                        else if (c.rack_down_arrive || c.rack_down_departure || c.rack_down_departure_last)
                        {
                            //目的地までの移動指示
                            if (degree_cur != 999)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, (degree_cur != degree_next), degree_next, false, false, false, (short)c.rack_turn_arrive_degree, false, false, false);
                                //SetUncalibratable(qr_prev, order);
                                order.speed = (System.UInt16)speed_pre;
                                if (order.speed == 0) SetSpeed(order);
                                orders.Add(order);
                            }

                            if (c.rack_turn_arrive)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                      , c.Location, (degree_cur != degree_next), degree_next, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_arrive_degree, false, false, false);
                                //SetUncalibratable(c.cur_qr, order);
                                orders.Add(order);
                            }
                            else if (c.rack_turn_departure)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                      , c.Location, (degree_cur != degree_next), degree_next, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_departure_degree, false, false, false);
                                //SetUncalibratable(c.cur_qr, order);
                                orders.Add(order);
                            }

                            //棚下降
                            order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, false, degree_cur, false, true, false, (short)c.rack_turn_arrive_degree, false, false, false);
                            //SetUncalibratable(c.cur_qr, order);
                            orders.Add(order);

                            break;
                        }
                        else if (c.wait_charge_trg)
                        {
                            //目的地までの移動指示
                            if (degree_cur != 999)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, (degree_cur != degree_next), degree_cur, false, false, false, (short)c.rack_turn_arrive_degree, c.cur_qr.charge_back, false, false);
                                //SetUncalibratable(qr_prev, order);
                                order.speed = (System.UInt16)speed_pre;
                                if (order.speed == 0) SetSpeed(order);
                                orders.Add(order);
                            }

                            //充電指示
                            order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, true, degree_cur, false, false, false, (short)c.rack_turn_arrive_degree, false, true, false);
                            //SetUncalibratable(c.cur_qr, order);
                            orders.Add(order);
                        }
                        else if ((degree_cur != 999 && degree_cur != degree_next) || /*pitch_cur != pitch_next ||*/ cn.cur_qr.charge_back)
                        {
                            //目的地までの移動指示
                            if (degree_cur != 999)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, false, degree_cur, false, false, false, rack_degree_pre, false, false, false);
                                //SetUncalibratable(qr_prev, order);
                                order.speed = (System.UInt16)speed_pre;
                                if (order.speed == 0) SetSpeed(order);
                                orders.Add(order);
                            }

                            if (c.rack_turn_arrive)
                            {
                                if (degree_next % 90 == 0)
                                {
                                    order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                          , c.Location, (degree_cur != degree_next), degree_next, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_arrive_degree, false, false, false);
                                    //SetUncalibratable(c.cur_qr, order);
                                    orders.Add(order);
                                }
                                else
                                {
                                    order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                          , c.Location, false, degree_next, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_arrive_degree, false, false, false);
                                    //SetUncalibratable(c.cur_qr, order);
                                    orders.Add(order);

                                    //order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                    //      , c.Location, true, degree_next, false, false, false, (short)c.rack_turn_arrive_degree, false, false, false);
                                    //SetUncalibratable(c.cur_qr, order);
                                    //orders.Add(order);
                                }

                                //if (c != this) break;
                            }
                            else if (c.rack_turn_departure)
                            {
                                if (degree_next % 90 == 0)
                                {
                                    order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                          , c.Location, (degree_cur != degree_next), degree_next, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_departure_degree, false, false, false);
                                    //SetUncalibratable(c.cur_qr, order);
                                    orders.Add(order);
                                }
                                else
                                {
                                    order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                          , c.Location, false, degree_next, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_departure_degree, false, false, false);
                                    //SetUncalibratable(c.cur_qr, order);
                                    orders.Add(order);

                                    //order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                    //      , c.Location, true, degree_next, false, false, false, (short)c.rack_turn_departure_degree, false, false, false);
                                    //SetUncalibratable(c.cur_qr, order);
                                    //orders.Add(order);
                                }

                                //if (c != this) break;
                            }
                        }
                        else if ((int)speed_pre != 0 && speed_pre != c.speed)
                        {
                            //目的地までの移動指示
                            if (degree_cur != 999)
                            {
                                if (qr_prev != owner_agv.on_qr)
                                {
                                    order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                      , qr_prev.Location, false, degree_cur, false, false, false, (short)c.rack_turn_arrive_degree, false, false, false);
                                    //SetUncalibratable(qr_prev, order);

                                    order.speed = (System.UInt16)speed_pre;
                                    if (order.speed == 0) SetSpeed(order);
                                    orders.Add(order);
                                }
                            }

                            //if (c.rack_turn_arrive)
                            //{
                            //    if (degree_next % 90 == 0)
                            //    {
                            //        order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                            //          , qr_prev.Location, false, degree_cur, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_arrive_degree, false, false, false);
                            //        SetUncalibratable(c.cur_qr, order);
                            //        orders.Add(order);
                            //    }
                            //    else
                            //    {
                            //        order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                            //          , qr_prev.Location, false, degree_cur, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_arrive_degree, false, false, false);
                            //        SetUncalibratable(c.cur_qr, order);
                            //        orders.Add(order);

                            //        //order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                            //        //  , qr_prev.Location, true, degree_next, false, false, false, (short)c.rack_turn_arrive_degree, false, false, false);
                            //        //SetUncalibratable(c.cur_qr, order);
                            //        //orders.Add(order);
                            //    }
                            //}
                            //else if (c.rack_turn_departure)
                            //{
                            //    if (degree_next % 90 == 0)
                            //    {
                            //        order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                            //          , qr_prev.Location, false, degree_cur, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_departure_degree, false, false, false);
                            //        SetUncalibratable(c.cur_qr, order);
                            //        orders.Add(order);
                            //    }
                            //    else
                            //    {
                            //        order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                            //          , qr_prev.Location, false, degree_cur, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_departure_degree, false, false, false);
                            //        SetUncalibratable(c.cur_qr, order);
                            //        orders.Add(order);

                            //        //order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                            //        //  , qr_prev.Location, false, degree_cur, false, false, false, (short)c.rack_turn_departure_degree, false, false, false);
                            //        //SetUncalibratable(c.cur_qr, order);
                            //        //orders.Add(order);
                            //    }
                            //}
                        }
                        else if (c.rack_turn_arrive)
                        {
                            //目的地までの移動指示
                            if (degree_cur != 999)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, false, degree_cur, false, false, false, (short)rack_degree_pre, false, false, false);
                                //SetUncalibratable(qr_prev, order);
                                order.speed = (System.UInt16)speed_pre;
                                if (order.speed == 0) SetSpeed(order);
                                orders.Add(order);
                            }

                            order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, (degree_cur != degree_next), degree_next, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_arrive_degree, false, false, false);
                            //SetUncalibratable(c.cur_qr, order);
                            orders.Add(order);

                            //if (c != this) break;
                        }
                        else if (c.rack_turn_departure)
                        {
                            //目的地までの移動指示
                            if (degree_cur != 999)
                            {
                                order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, false, degree_cur, false, false, false, (short)c.rack_turn_departure_degree, false, false, false);
                                //SetUncalibratable(qr_prev, order);
                                order.speed = (System.UInt16)speed_pre;
                                if (order.speed == 0) SetSpeed(order);
                                orders.Add(order);
                            }

                            order = new AgvController.AgvCommunicator.Order(AgvController.AgvCommunicator.Order.CMD.OPERATE
                                  , c.Location, (degree_cur != degree_next), degree_next, false, false, c.cur_qr.rack_rotatable, (short)c.rack_turn_departure_degree, false, false, false);
                            //SetUncalibratable(c.cur_qr, order);
                            orders.Add(order);

                            //if (c != this) break;
                        }

                        pitch_cur = pitch_next;

                        
                        //if (degree_cur != 999 && degree_cur != degree_next) break;

                        rack_degree_pre = (short)c.rack_turn_arrive_degree;
                        //degree_pre = degree_cur;
                        degree_cur = degree_next;
                        qr_prev = c.cur_qr;
                        speed_pre = c.speed;
                    }

                    if (c == stop_con) break;
                }

                if (0 < orders.Count)
                {
                    //PointF location = orders.Last().Location;
                    foreach (var v in orders)
                    {
                        //v.dist = (uint)cur_qr.Distance(owner_agv.Location, location);
                        v.l_pat = this.l_pat;
                        v.m_pat = this.m_pat;

                        if (owner_agv.on_qr != null) SetUncalibratable(owner_agv.on_qr, v);
                    }
                }

                return orders.ToArray();
            }

            public double DegreeRack(double deg)
            {
                if (owner_agv.rack != null)
                {
                    deg = (deg + 360) % 360;
                    owner_agv.rack.degree = (owner_agv.rack.degree + 360) % 360;
                    if (deg == -180) deg = 180;

                    return deg;
                }

                return -999;
            }

            public double DegreeAGV(double deg)
            {
                deg = (deg + 360) % 360;
                owner_agv.degree = (owner_agv.degree + 360) % 360;
                if (deg == -180) deg = 180;

                return deg;
            }

            public void SetSpeed(AgvController.AgvCommunicator.Order order)
            {
                if (order.cmd == (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE)
                {
                    order.speed = (ushort)speed;

                    //switch (speed)
                    //{
                    //    case enSpeed.ML:
                    //        order.mod_speed_0 = true;
                    //        order.mod_speed_1 = true;
                    //        break;

                    //    case enSpeed.M:
                    //        order.mod_speed_1 = true;
                    //        break;

                    //    case enSpeed.HM:
                    //        order.mod_speed_1 = true;
                    //        order.mod_speed_2 = true;
                    //        break;

                    //    case enSpeed.H:
                    //        order.mod_speed_2 = true;
                    //        break;
                    //}
                }
            }

            public void SetUncalibratable(FloorQR qr, AgvController.AgvCommunicator.Order order)
            {
                if (qr.direction_charge != FloorQR.enDirection.NONE) order.mod_uncalibratable = true;
                else if (qr.station_id != "" && !qr.rack_setable) order.mod_uncalibratable = true;

                //オートレーター周辺
                else if (qr.autorator_id != "") order.mod_uncalibratable = true;
                else if (qr.next_way.Where(ee => ee.Key.autorator_id != "").Count() != 0) order.mod_uncalibratable = true;
                else if (qr.prev_way.Where(ee => ee.autorator_id != "").Count() != 0) order.mod_uncalibratable = true;
            }

            #endregion

            #region クローン

            public RouteCondition Clone()
            {
                RouteCondition r = this.MemberwiseClone() as RouteCondition;
                return r;

                //RouteCondition ret = new RouteCondition();
                //ret.owner_agv = r.owner_agv;
                //ret.cur_qr = r.cur_qr;
                //ret.next_condition = r.next_condition;
                //ret.prev_condition = r.prev_condition;
                //ret.speed = r.speed;
                //ret.agv_turn_arrive = r.agv_turn_arrive;
                //ret.agv_turn_arrive_degree = r.agv_turn_arrive_degree;
                //ret.rack_turn_arrive = r.rack_turn_arrive;
                //ret.rack_turn_arrive_degree = r.rack_turn_arrive_degree;
                //ret.agv_turn_departure = r.agv_turn_departure;
                //ret.agv_turn_departure_degree = r.agv_turn_departure_degree;
                //ret.rack_turn_departure = r.rack_turn_departure;
                //ret.rack_turn_departure_degree = r.rack_turn_departure_degree;
                //ret.rack_down_departure = r.rack_down_departure;
                //ret.rack_down_departure_last = r.rack_down_departure_last;
                //ret.wait_timer = r.wait_timer;
                //ret.wait_station_trg = r.wait_station_trg;
                //ret.wait_autorator_in_trg = r.wait_autorator_in_trg;
                //ret.wait_autorator_out_trg = r.wait_autorator_out_trg;
                //ret.wait_charge_trg = r.wait_charge_trg;
                //ret.wait_other_agv_locations = new Dictionary<FloorAGV, PointF>();
                //foreach (var v in r.wait_other_agv_locations)
                //{
                //    ret.wait_other_agv_locations[v.Key] = v.Value;
                //}
                //ret.rack_regulation = r.rack_regulation;
                //ret.l_pat = r.l_pat;
                //ret.m_pat = r.m_pat;
                //ret.distance = r.distance;

                //return ret;
            }

            #endregion
        }

        #endregion
    }
}
