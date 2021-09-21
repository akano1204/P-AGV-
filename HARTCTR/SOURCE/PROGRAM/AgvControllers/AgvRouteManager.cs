using BelicsClass.Common;
using BelicsClass.PLC;
using BelicsClass.File;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        public class AgvRouteManager
        {
            #region ENUM
            /// <summary>
            /// コンディション作成の条件
            /// </summary>
            protected enum ConditionRule
            {
                TakeRack,       // 棚取得
                RackToStation,  // 棚をSTへ
                RemoveRack,     // 棚をどかす
                ReturnRack,     // 棚を返す
                Charge,         // 充電
                Evacuation,     // 退避
                Move,           // 移動
            }

            /// <summary>
            /// RouteConditionに設定されるRACK_DEGの基準
            /// </summary>
            public enum RackDegCalculationType
            {
                [BL_EnumLabel("MAP基準")]
                MAP_REFERENCE_TYPE,
                [BL_EnumLabel("AGV基準")]
                AGV_REFERENCE_TYPE
            }
            #endregion

            #region フィールド

            protected AgvControlManager controller = null;

            protected IReadOnlyDictionary<int, int> face2degree = new Dictionary<int, int>()
            {
                {0, 999}, {1, 0}, {2, 90}, {3, 180}, {4, 270},
            };

            protected IReadOnlyDictionary<int, int> degree2face = new Dictionary<int, int>()
            {
                {999, 0}, {0, 1}, {90, 2}, {180, 3}, {270, 4},
            };

            protected BL_Log _log = null;
            protected bool output_log = false;
            #endregion

            #region プロパティ
            public RackDegCalculationType CalculationType { get; set; } = RackDegCalculationType.AGV_REFERENCE_TYPE;
            #endregion

            #region コンストラクタ・デストラクタ
            public AgvRouteManager(AgvControlManager controller)
            {
                _log = new BL_Log("", "AgvRouteManager");
                RefreshMap(controller);
            }
            ~AgvRouteManager()
            {
                if (_log != null) _log.Dispose();
            }
            #endregion

            #region マップのリフレッシュ
            /// <summary>
            /// マップの読込時にリストの初期化・更新等を行う
            /// </summary>
            public void RefreshMap(AgvControlManager controller)
            {
                this.controller = controller;
            }
            #endregion

            #region 最短経路の取得

            /// <summary>
            /// 最短経路検索の条件
            /// </summary>
            protected enum SearchRule
            {
                /// <summary>
                /// 条件なし
                /// </summary>
                None,
                /// <summary>
                /// 可能な限り棚を避ける
                /// </summary>
                AvoidRack,
                /// <summary>
                /// 完全に棚を回避する
                /// </summary>
                ExclusionRack,
            }

            protected class CostQR
            {
                public FloorQR qr = null;
                public int cost { get; private set; }

                public CostQR prev { get; private set; }

                public int rackDegree { get; private set; }

                public int agvDegree { get; private set; }

                private double deg = 999;
                private int straightCount = 0;
                private int depth = 0;

                private List<CostQR> nexts = new List<CostQR>();

                public CostQR(FloorQR qr, CostQR prev, int agv_deg, int rack_deg, SearchRule rule)
                {
                    this.qr = qr;
                    this.prev = prev;
                    this.rackDegree = rack_deg;
                    this.agvDegree = agv_deg;

                    if (prev == null)
                    {
                        deg = 999;
                        straightCount = 0;
                        cost = 0;
                        depth = 1;
                    }
                    else
                    {
                        depth = prev.depth + 1;

                        const int LowSpeedCoeff = 3;
                        const int AccelCost = 2 * LowSpeedCoeff;
                        int disCost = 0;
                        int rollCost = 0;
                        int rackCost = 0;
                        int returnCost = 0;
                        int autoratorCost = 0;

                        if (prev.qr.next_way.ContainsKey(qr))
                        {
                            deg = prev.qr.next_way[qr];
                            if (prev.deg != deg)
                            {
                                straightCount = 1;
                            }
                            else
                            {
                                straightCount = prev.straightCount + 1;
                            }
                        }

                        if (straightCount < 3)
                        {
                            disCost = depth * LowSpeedCoeff;
                        }
                        //else if (straightCount >= 3)
                        else
                        {
                            disCost = (depth - 2) + AccelCost;
                        }
                        //else
                        //{
                        //    disCost = (depth - 2);
                        //}

                        if (prev.deg != deg)
                        {
                            rollCost = (int)(Math.Abs(prev.deg - deg) / 90) * 2;
                        }

                        if (rule == SearchRule.AvoidRack && qr.rack != null)
                        {
                            rackCost = 20000;
                        }

                        //if (rule == SearchRule.AvoidRack || rule == SearchRule.ExclusionRack)
                        //{
                        //    if (prev.qr.rack != null && qr.next_way.ContainsKey(prev.qr))
                        //    {
                        //        returnCost = -10000;
                        //    }
                        //}

                        if (prev.qr.autorator_id != string.Empty && qr.autorator_id != string.Empty)
                        {
                            int div_floor = Math.Abs(qr.floor.FloorNo - prev.qr.floor.FloorNo);
                            autoratorCost = 30000 + 100 * div_floor;
                        }

                        cost = this.prev.cost + disCost + rollCost + rackCost + returnCost;
                    }
                }

                public void SetNextQR(CostQR next)
                {
                    nexts.Add(next);
                }

                public CostQR GetNextQR(CostQR targetQR)
                {
                    return nexts.FirstOrDefault(x => x == targetQR);
                }

                public void ResetRackDegree(int rack_degree)
                {
                    this.rackDegree = rack_degree;
                }

                public override string ToString()
                {
                    return qr.ToString() + " cost=" + cost + " depth=" + depth;
                }
            }

            /// <summary>
            /// 最短経路の取得
            /// </summary>
            /// <param name="start">開始地点</param>
            /// <param name="goal">目的地</param>
            /// <param name="rule">最短経路検索の条件</param>
            /// <param name="exceptRoute">除外したいQRのリスト</param>
            /// <param name="agv">対象のAGV</param>
            /// <param name="rack">運ぶ棚</param>
            /// <param name="rackDirectionConsider">棚の向きを考慮してルートを生成するフラグ true:向きを考慮する false:向きを考慮しない</param>
            /// <returns>最短経路</returns>
            protected List<FloorQR> GetShortestRoute(FloorQR start, FloorQR goal, SearchRule rule, IReadOnlyList<FloorQR> exceptRoute, FloorAGV agv, Rack rack, bool rackDirectionConsider)
            {
                List<CostQR> open = new List<CostQR>();
                List<CostQR> close = new List<CostQR>();

                List<FloorQR> except = new List<FloorQR>();
                if (exceptRoute != null) except.AddRange(exceptRoute);

                bool haveRack = rack != null;

                Dictionary<FloorQR.RouteType, List<int>> way_type_deg = new Dictionary<FloorQR.RouteType, List<int>>();
                way_type_deg[FloorQR.RouteType.NONE] = new List<int>();
                way_type_deg[FloorQR.RouteType.FAT] = new List<int>();
                way_type_deg[FloorQR.RouteType.THIN] = new List<int>();

                if (haveRack)
                {
                    int now_rack_deg = (int)rack.degree;
                    if (now_rack_deg % 90 != 0)
                    {
                        //角度誤差の吸収
                        if (now_rack_deg <= 2 || 358 <= now_rack_deg) now_rack_deg = 0;
                        if (88 <= now_rack_deg && now_rack_deg <= 92) now_rack_deg = 90;
                        if (178 <= now_rack_deg && now_rack_deg <= 182) now_rack_deg = 180;
                        if (268 <= now_rack_deg && now_rack_deg <= 272) now_rack_deg = 270;
                    }

                    foreach (var v in rack.can_move)
                    {
                        if (v.Value)
                        {
                            way_type_deg[FloorQR.RouteType.NONE].Add(v.Key);

                            if (rack.sizeL < rack.sizeW)
                            {
                                if (v.Key == 0 || v.Key == 180) way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                                else if (v.Key == 90 || v.Key == 270) way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                            }
                            else if (rack.sizeW < rack.sizeL)
                            {
                                if (v.Key == 0 || v.Key == 180) way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                else if (v.Key == 90 || v.Key == 270) way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                            }
                            else
                            {
                                way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                            }
                        }
                    }

                    open.Add(new CostQR(start, null, agv.degree, now_rack_deg, rule));
                }
                else
                {
                    open.Add(new CostQR(start, null, agv.degree, 999, rule));
                }
                

                CostQR qr = null;

                while (open.Count > 0)
                {
                    open.Sort((a, b) => a.cost - b.cost);
                    qr = open[0];
                    open.RemoveAt(0);

                    if (qr.qr == goal)
                    {
                        break;
                    }
                    else
                    {
                        close.Add(qr);

                        List<FloorQR> next_way = new List<FloorQR>();
                        List<int> exceptionDeg = new List<int>();

                        // 今いるQRでの棚脱出可能な角度を取得
                        if (qr.qr.rack != null)
                        {
                            // 入出チェック
                            Dictionary<int, bool> can_inout = new Dictionary<int, bool>();
                            can_inout[0] = false;
                            can_inout[90] = false;
                            can_inout[180] = false;
                            can_inout[270] = false;

                            foreach (var p in qr.qr.rack.can_inout)
                            {
                                if (can_inout.ContainsKey(p.Key)) can_inout[p.Key] = p.Value;
                            }

                            foreach (var p in can_inout)
                            {
                                if (!p.Value)
                                {
                                    int deg = (int)((p.Key + qr.qr.rack.degree) % 360);
                                    exceptionDeg.Add(deg);
                                }
                            }

                            // 棚を持っているときに移動不可能な方向を算出
                            if (haveRack)
                            {
                                Dictionary<int, bool> can_move = new Dictionary<int, bool>();
                                can_move[0] = false;
                                can_move[90] = false;
                                can_move[180] = false;
                                can_move[270] = false;

                                foreach (var p in qr.qr.rack.can_move)
                                {
                                    if (can_move.ContainsKey(p.Key)) can_move[p.Key] = p.Value;
                                }

                                foreach (var p in can_move)
                                {
                                    if (!p.Value && !qr.qr.rack_rotatable)
                                    {
                                        int deg = (int)((p.Key + qr.qr.rack.degree) % 360);
                                        exceptionDeg.Add(deg);
                                    }
                                }
                            }
                        }
                        // 次のQRへの進入可能かを確認
                        foreach(var v in qr.qr.next_way)
                        {
                            if (exceptionDeg.Contains((int)(v.Value))) continue;

                            var r = v.Key.rack;
                            if (r == null)
                            {
                                next_way.Add(v.Key);
                            }
                            else
                            {
                                Dictionary<int, bool> can_inout = new Dictionary<int, bool>();
                                can_inout[0] = false;
                                can_inout[90] = false;
                                can_inout[180] = false;
                                can_inout[270] = false;

                                foreach (var p in r.can_inout)
                                {
                                    if (can_inout.ContainsKey(p.Key)) can_inout[p.Key] = p.Value;
                                }

                                foreach (var p in can_inout)
                                {
                                    if (((int)v.Value) == (((p.Key + r.degree) % 360 + 180) % 360))
                                    {
                                        if (p.Value) next_way.Add(v.Key);
                                    }
                                }
                            }
                        }

                        // オートレータの次QRを追加
                        if (qr.qr.autorator_id != string.Empty)
                        {
                            var con_rator = controller.AllQR.Where(e => e.autorator_id != string.Empty
                                                                     && e.autorator_id == qr.qr.autorator_id
                                                                     && e.autorator_online
                                                                     && e != qr.qr
                                                                   );
                            next_way.AddRange(con_rator);
                        }

                        if (qr.prev != null) next_way.Remove(qr.prev.qr);

                        foreach (var v in next_way)
                        {
                            if (!open.Exists(e => e.qr == v && e.prev.qr == qr.prev.qr) && !close.Exists(e => e.qr == v) && !except.Contains(v) && (haveRack ? qr.qr.direction_charge == FloorQR.enDirection.NONE : true))
                            {
                                int agvdeg = qr.agvDegree;
                                int rackdeg = qr.rackDegree;

                                bool existAGV = v.on_agv != null;
                                bool existAgvRunner = existAGV ? v.on_agv.agvRunner != null : false;
                                bool existCommunicator = existAgvRunner ? v.on_agv.agvRunner.communicator != null : false;

                                // AGVがいる かつ AgvRunnerがある かつ communicatorがある場合のみ
                                // 上記以外は異常なしとして通常処理へ
                                if (existAGV && existAgvRunner && existCommunicator)
                                {
                                    // error_code != 0 であれば異常あり、0 であれば通常処理へ
                                    if (v.on_agv.agvRunner.communicator.GetState.error_code != 0)
                                    {
                                        // 異常のあるAGVがいるQRを除外
                                        continue;
                                    }
                                }

                                // オートレータの状態確認
                                // offlineの場合は除外
                                if (v.autorator_id != string.Empty && !v.autorator_online) continue;

                                // 棚持ち時の処理
                                if (haveRack && rackDirectionConsider)
                                {
                                    // 棚を動かさずに行けるところまで行く
                                    // もし棚を回転させないといけない場合はルート候補には追加しない
                                    // ゴールするまで実施
                                    var next_way_type = qr.qr.next_way_type[v];
                                    var next_way_degree = (int)qr.qr.Degree(v);

                                    if (next_way_degree % 90 != 0)
                                    {
                                        //角度誤差の吸収
                                        if ((0 <= next_way_degree && next_way_degree < 45) || 315 <= next_way_degree) next_way_degree = 0;
                                        if (45 <= next_way_degree && next_way_degree < 135) next_way_degree = 90;
                                        if (135 <= next_way_degree && next_way_degree < 225) next_way_degree = 180;
                                        if (225 <= next_way_degree && next_way_degree < 315) next_way_degree = 270;
                                    }

                                    var rack_face_deg = ((next_way_degree - rackdeg) + 360) % 360;

                                    if (!way_type_deg[next_way_type].Contains(rack_face_deg)) 
                                    {
                                        // 含まれない場合は除外
                                        continue;
                                    }
                                }

                                if (rule == SearchRule.ExclusionRack)
                                {
                                    if (v.rack == null)
                                    {
                                        var next_qr = new CostQR(v, qr, agvdeg, rackdeg, rule);
                                        qr.SetNextQR(next_qr);
                                        open.Add(next_qr);
                                    }
                                }
                                else
                                {
                                    var next_qr = new CostQR(v, qr, agvdeg, rackdeg, rule);
                                    qr.SetNextQR(next_qr);
                                    open.Add(next_qr);
                                }
                            }
                        }
                    }
                }

                List<FloorQR> route = new List<FloorQR>();

                while (qr != null)
                {
                    route.Add(qr.qr);
                    qr = qr.prev;
                }
                route.Reverse();

                return route;
            }
            #endregion

            #region コンディション関連

            /// <summary>
            /// コンディションの接続
            /// </summary>
            /// <param name="a">結合先のコンディションのリスト</param>
            /// <param name="b">結合元のコンディションのリスト</param>
            /// <returns>結合後のコンディションのリスト</returns>
            public static RouteConditionList ConnectConditions(RouteConditionList a, RouteConditionList b)
            {
                if (a.Count() == 0) return b;

                RouteConditionList con1 = new RouteConditionList(a);
                RouteConditionList con2 = new RouteConditionList(b);

                // cons1のゴール = cons2のスタートなのでcons2のスタートは飛ばす
                con2.RemoveAt(0);

                var con1_last = con1.LastOrDefault();
                var con2_first = con2.FirstOrDefault();
                if (con1_last != null)
                {
                    con1_last.next_condition = con2_first;
                }
                if (con2_first != null)
                {
                    con2_first.prev_condition = con1_last;
                }

                con1.AddRange(con2);

                return con1;
            }

            /// <summary>
            /// コンディションの作成
            /// </summary>
            /// <param name="agv">対象AGV</param>
            /// <param name="route">使用するルート</param>
            /// <param name="rule">コンディション作成の条件</param>
            /// <param name="operate">棚操作指示</param>
            /// <returns>コンディションのリスト</returns>
            protected RouteConditionList GenerateConditions(FloorAGV agv, IReadOnlyList<FloorQR> route, ConditionRule rule, enPlaceOperate operate)
            {
                RouteConditionList conditions = new RouteConditionList();
                RouteCondition prev = null;

                Rack rack = agv.rack;
                int rack_deg = 999;
                if (rack != null) rack_deg = (int)rack.degree;

                if (route.Count == 0) return conditions;

                int count = 0;
                RouteCondition rackRotateCondition = null;

                string floorCode = route.First().floor.code;

                //foreach (var qr in route)
                for (int i = 0; i < route.Count; i++)
                {
                    var qr = route[i];
                    RouteCondition condition = new RouteCondition();

                    condition.owner_agv = agv;
                    condition.cur_qr = qr;
                    condition.prev_condition = prev;
                    if (prev != null)
                    {
                        prev.next_condition = condition;
                    }

                    // オートレーターの出入り
                    //@@@kano オートレーターへの進入は、最低でもオートレータQRの進入2コマ手前に必要
                    //@@@     qr.autorator_id!=""の2コマ手前のQRで「condition.wait_autorator_in_pretrg = true;」を設定したい
                    //オートレーターの退出は、オートレータ－内

                    if (qr.floor.code != floorCode && qr.autorator_id != "")
                    {
                        // 退出
                        condition.wait_autorator_out_trg = true;
                    }
                    //else if (/*qr.floor.code == floorCode && */condition.next_condition != null && condition.next_condition.next_condition != null)
                    else if (i + 2 < route.Count)
                    {
                        // 進入
                        if (route[i + 2].floor.code == floorCode && route[i + 2].autorator_id != "")
                        {
                            condition.wait_autorator_in_pretrg = true;
                        }
                    }

                    // 条件ごとの生成
                    if (rule == ConditionRule.TakeRack)
                    {
                        if (count == route.Count - 1)
                        {
                            condition.rack_up_arrive = true;
                        }
                    }
                    else if (rule == ConditionRule.RackToStation)
                    {
                        if (qr.rack_rotatable)
                        {
                            rackRotateCondition = condition;
                        }

                        if (count == route.Count - 1 && qr.direction_station != FloorQR.enDirection.NONE)
                        {
                            if (rackRotateCondition != null)
                            {
                                rackRotateCondition.rack_turn_arrive = true;
                            }

                            if (qr.rack_setable)
                            {
                                if (operate == enPlaceOperate.RACK_DOWN)
                                {
                                    condition.rack_down_arrive = true;
                                    condition.wait_station_trg = false;
                                }
                            }
                            else
                            {
                                condition.rack_down_arrive = false;

                                if (operate == enPlaceOperate.STATION_WAIT)
                                {
                                    condition.wait_station_trg = true;
                                }
                            }
                        }
                    }
                    else if (rule == ConditionRule.RemoveRack || rule == ConditionRule.ReturnRack)
                    {
                        if (count == route.Count - 1)
                        {
                            condition.rack_down_arrive = true;
                        }
                    }

                    //if (qr.floor.code != floorCode)
                    //{
                        floorCode = qr.floor.code;
                    //}

                    conditions.Add(condition);
                    prev = condition;
                    count++;
                }

                return conditions;
            }

            /// <summary>
            /// コンディションの作成
            /// </summary>
            /// /// <param name="agv">対象AGV</param>
            /// <param name="route">使用するルート</param>
            /// <param name="rule">コンディション作成の条件</param>
            /// <returns>コンディションのリスト</returns>
            protected RouteConditionList GenerateConditions(FloorAGV agv, IReadOnlyList<FloorQR> route, ConditionRule rule)
            {
                return GenerateConditions(agv, route, rule, enPlaceOperate.NONE);
            }
            #endregion

            #region ルート生成・棚待避情報取得・対象情報取得・AGV回避行動

            #region ルート生成
            /// <summary>
            /// 指定したAGVを指定したQRまで移動するまでRouteConditionリストを作成
            /// 棚面が指定されている かつ 指定したQRがSTの場合に棚の回転処理も行う
            /// </summary>
            /// <param name="targetAgv">操作対象のAGV</param>
            /// <param name="desinationQR">目的地のQR</param>
            /// <param name="faceID">棚面のID</param>
            /// <param name="exceptQR">ルート生成から除外したいQR</param>
            /// <param name="blockRack">ルート生成時に棚を壁として扱うかを設定するフラグ true:扱う false:扱わない</param>
            /// <param name="rackDirectionConsider">棚の向きを考慮してルートを生成するフラグ true:向きを考慮する false:向きを考慮しない</param>
            /// <returns>RouteConditionリスト</returns>
            public RouteConditionList GetMoveConditions(FloorAGV targetAgv, FloorQR desinationQR, string faceID, IReadOnlyList<FloorQR> exceptQR = null, bool blockRack = true, bool rackDirectionConsider = true)
            {
                BL_Stopwatch sw = new BL_Stopwatch();
                sw.Restart();


                bool needRotable = false;
                int degree = 999;
                var rule = GetRouteSearchRule(targetAgv, blockRack);
                List<FloorQR> route = null;
                bool haveRack = targetAgv.rack != null;

                if (haveRack && desinationQR.station_id.Trim() != string.Empty && faceID.Trim() != string.Empty)
                {
                    var face = targetAgv.rack.face_id.Where(e => e.Value == faceID).FirstOrDefault();
                    degree = CalcRackRotateDegree((int)desinationQR.direction_station, face.Key);

                    //@@@AGVが斜め向きじゃない時だけ棚の向きを考慮させる
                    if (targetAgv.degree % 90 == 0)
                    {
                        if (!targetAgv.rack.degree_received || (degree + 360) % 360 != targetAgv.rack.degree)
                        {
                            //実際の棚の向きが反映されていない場合、とにかく棚旋回を必要とする
                            needRotable = true;
                        }
                    }
                }

                route = GetShortestRoute(targetAgv.on_qr, desinationQR, rule, exceptQR, targetAgv, targetAgv.rack, rackDirectionConsider);

                if (0 == route.Count() || route.Last().Location != desinationQR.Location)
                {
                    //目的地に到達できてない
                    return new RouteConditionList();
                }

                // STに合わせて棚を回転
                if (needRotable)
                {
                    if (route.Where(e => e.rack_rotatable).Count() == 0)
                    {
                        var all_rotable_qrs = controller.AllQR.Where(e => e.rack_rotatable);
                        var rotable_qr = controller.AllQR.Where(e => e.rack_rotatable)
                                                         .OrderBy(e => CalcRouteQrCost(
                                                                    GetShortestRoute(targetAgv.on_qr, e, rule, exceptQR, targetAgv, targetAgv.rack, rackDirectionConsider),
                                                                    targetAgv,
                                                                    blockRack)
                                                         ).FirstOrDefault();
                        if (rotable_qr == null || !all_rotable_qrs.Contains(rotable_qr)) return new RouteConditionList(); // 回転可能なQRに到達できない

                        var r1 = GetShortestRoute(targetAgv.on_qr, rotable_qr, rule, exceptQR, targetAgv, targetAgv.rack, rackDirectionConsider);
                        var r2 = GetShortestRoute(rotable_qr, desinationQR, rule, exceptQR, targetAgv, targetAgv.rack, rackDirectionConsider);
                        r2.RemoveAt(0);
                        route = new List<FloorQR>(r1);
                        route.AddRange(r2);
                    }
                }
                //else
                //{
                //    route = GetShortestRoute(targetAgv.on_qr, desinationQR, rule, exceptQR, targetAgv, targetAgv.rack);
                //}

                var cons = GenerateConditions(targetAgv, route, ConditionRule.RackToStation);

                if (needRotable)
                {
                    var con = cons.Where(e => e.rack_turn_arrive == true || e.rack_turn_departure == true).LastOrDefault();

                    if (con != null)
                    {
                        if (con.rack_turn_arrive) con.rack_turn_arrive_degree = degree;
                        if (con.rack_turn_departure) con.rack_turn_departure_degree = degree;
                    }
                }
                else
                {
                    var con = cons.Where(e => e.rack_turn_arrive == true || e.rack_turn_departure == true).LastOrDefault();

                    if (con != null)
                    {
                        con.rack_turn_arrive = false;
                        con.rack_turn_departure = false;
                    }
                }

                int rack_size_max = 125;
                var racklist = RackMaster.Instance.GetRackList();
                var racklist_max = racklist.OrderByDescending(e => e.SizeMax).FirstOrDefault();
                if (racklist_max != null)
                {
                    rack_size_max = racklist_max.SizeMax;
                }

                for (int i = 0; i < cons.Count; i++)
                {
                    var v = cons[i];

                    if (v.RoutingAutoratorQR() != null)
                    {
                        v.wait_autorator_in_trg = true;
                        break;
                    }
                }

                //for (int i = cons.Count - 1; 0 <= i; i--)
                //{
                //    var v = cons[i];
                //    if (v.PrevRoutingAutoratorQR != null)
                //    {
                //        v.wait_autorator_out_trg = true;
                //        break;
                //    }
                //}

                if (haveRack && needRotable)
                {
                    var rotate_err = false;

                    Dictionary<FloorQR.RouteType, List<int>> way_type_deg = new Dictionary<FloorQR.RouteType, List<int>>();
                    way_type_deg[FloorQR.RouteType.NONE] = new List<int>();
                    way_type_deg[FloorQR.RouteType.FAT] = new List<int>();
                    way_type_deg[FloorQR.RouteType.THIN] = new List<int>();
                    foreach (var v in targetAgv.rack.can_move)
                    {
                        if (v.Value)
                        {
                            way_type_deg[FloorQR.RouteType.NONE].Add(v.Key);

                            if (targetAgv.rack.sizeL < targetAgv.rack.sizeW)
                            {
                                if (v.Key == 0 || v.Key == 180) way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                                else if (v.Key == 90 || v.Key == 270) way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                            }
                            else if (targetAgv.rack.sizeW < targetAgv.rack.sizeL)
                            {
                                if (v.Key == 0 || v.Key == 180) way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                else if (v.Key == 90 || v.Key == 270) way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                            }
                            else
                            {
                                way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                            }
                        }
                    }

                    int now_deg = targetAgv.degree;

                    int now_rack_deg = (int)(now_deg - targetAgv.rack.degree + 360) % 360;
                    if (now_rack_deg % 90 != 0)
                    {
                        //角度誤差の吸収
                        if (now_rack_deg <= 2 || 358 <= now_rack_deg) now_rack_deg = 0;
                        if (88 <= now_rack_deg && now_rack_deg <= 92) now_rack_deg = 90;
                        if (178 <= now_rack_deg && now_rack_deg <= 182) now_rack_deg = 180;
                        if (268 <= now_rack_deg && now_rack_deg <= 272) now_rack_deg = 270;
                    }

                    int rack_rotatable_pos = -1;
                    int need_deg_rack = 999;
                    int need_deg_agv = 999;

                    for (int i = 0; i < cons.Count; i++)
                    {
                        var v = cons[i];

                        if (v.cur_qr.rack_rotatable) rack_rotatable_pos = i;

                        if (v.next_condition == null)
                        {
                            //v.agv_turn_arrive_degree = now_deg;
                            //v.rack_turn_arrive_degree = now_rack_deg;

                            break;
                        }
                        else
                        {
                            int next_deg = v.Direction;
                            int next_rack_deg = now_rack_deg;

                            if (now_deg != next_deg)
                            {
                                int diff = (next_deg - now_deg + 360) % 360;
                                next_rack_deg = (now_rack_deg + diff + 360) % 360;

                                if (diff % 90 != 0)
                                {
                                    next_rack_deg = (next_rack_deg - diff + 360) % 360;
                                    //v.rack_turn_arrive = true;
                                }
                            }

                            if (need_deg_rack != 999)
                            {
                                int diff = (need_deg_agv - now_deg + 360) % 360;
                                next_rack_deg = (need_deg_rack - diff + 360) % 360;
                            }

                            if (v.cur_qr.floor.code == v.next_condition.cur_qr.floor.code)
                            {
                                var next_way_type = v.cur_qr.next_way_type[v.next_condition.cur_qr];
                                List<int> canmove_rack_deg = way_type_deg[next_way_type];

                                if (!canmove_rack_deg.Contains(next_rack_deg))
                                {
                                    if (!v.cur_qr.rack_rotatable)
                                    {
                                        if (0 <= rack_rotatable_pos)
                                        {
                                            i = rack_rotatable_pos - 1;
                                            need_deg_rack = canmove_rack_deg[0];
                                            need_deg_agv = next_deg;
                                            continue;
                                        }
                                        else
                                        {
                                            rotate_err = true;
                                            break;
                                        }
                                    }
                                    else if (!canmove_rack_deg.Contains((v.rack_turn_arrive_degree + 360) % 360))
                                    {
                                        rotate_err = true;
                                        break;
                                    }

                                    now_rack_deg = canmove_rack_deg[0];
                                    //v.rack_turn_arrive = true;
                                }
                                else
                                {
                                    if (need_deg_rack != 999)
                                    {
                                        //v.rack_turn_arrive = true;
                                        rack_rotatable_pos = -1;
                                    }

                                    now_rack_deg = next_rack_deg;
                                }

                                need_deg_rack = 999;

                                now_deg = next_deg;
                            }

                            //v.agv_turn_arrive_degree = now_deg;
                            //v.rack_turn_arrive_degree = now_rack_deg;
                        }
                    }

                    // ゴールに到達不可
                    if (rotate_err)
                        return new RouteConditionList();


                    if (0 < cons.Count())
                    {
                        //目的地に到達できてないなら全消去
                        if (cons.Last().Location != desinationQR.Location)
                        {
                            cons.Clear();
                        }
                    }
                }
                else if (haveRack)
                {
                    var rotate_err = false;

                    if (CalculationType == RackDegCalculationType.MAP_REFERENCE_TYPE)
                    {
                        #region MAP上0°を基準としてRACK_DEGを算出
                        var agv_deg = (int)targetAgv.degree;
                        var rack = targetAgv.rack;

                        var rack_deg = (360 - (int)rack.degree) % 360;

                        if (0 <= rack.degree && rack.degree <= 45 || 315 < rack.degree) rack_deg = 0;
                        if (45 < rack.degree && rack.degree <= 135) rack_deg = (360 - 90) % 360;
                        if (135 < rack.degree && rack.degree <= 225) rack_deg = (360 - 180) % 360;
                        if (225 < rack.degree && rack.degree <= 315) rack_deg = (360 - 270) % 360;

                        List<RouteCondition> prevs = new List<RouteCondition>();

                        foreach (var con in cons)
                        {
                            var next_con = con.next_condition;
                            var next_qr = next_con != null ? next_con.cur_qr : null;

                            var qr = con.cur_qr;

                            var prev_con = con.prev_condition;
                            var prev = prev_con != null ? prev_con.cur_qr : null;

                            if (next_qr == null) continue; // 終端
                            if (!qr.next_way.ContainsKey(next_qr)) continue; // オートレータ

                            bool rightAngle = qr.next_way[next_qr] % 90 == 0;
                            bool prev_rightAngle = prev != null ? prev.next_way[qr] % 90 == 0 : true;
                            var way_type = qr.next_way_type.ContainsKey(next_qr) ? qr.next_way_type[next_qr] : FloorQR.RouteType.NONE;
                            var prev_way_type = prev != null ? (prev.next_way_type.ContainsKey(qr) ? prev.next_way_type[qr] : FloorQR.RouteType.NONE) : FloorQR.RouteType.NONE;

                            // 前回が斜めであるなら、棚は進む角度を向いている
                            if (!prev_rightAngle)
                            {
                                if ((int)qr.next_way[next_qr] % 180 == 0) rack_deg = (int)qr.next_way[next_qr] + (rack_deg % 180 == 0 ? 180 : 0);
                                else rack_deg = (int)qr.next_way[next_qr];
                            }

                            int way_deg = rightAngle ? (int)qr.next_way[next_qr] : agv_deg % 90 == 0 ? agv_deg : rack_deg;
                            int face_deg = (rack_deg + way_deg) % 360;

                            int diffdeg = rightAngle ? (way_deg - 180) % 180 : 0;

                            int fdeg_000 = face_deg;
                            int fdeg_090 = (face_deg + 90) % 360;
                            int fdeg_180 = (face_deg + 180) % 360;
                            int fdeg_270 = (face_deg + 270) % 360;

                            int fatSize = rack.sizeL >= rack.sizeW ? rack.sizeL : rack.sizeW;
                            int thinSize = rack.sizeL < rack.sizeW ? rack.sizeL : rack.sizeW;
                            int nowSize = face_deg % 180 == 0 ? rack.sizeL : rack.sizeW;

                            // 進もうとしている方向を基準にcan_moveを取得
                            bool c_000 = rack.can_move.ContainsKey(fdeg_000) ? rack.can_move[fdeg_000] : false; // 前
                            bool c_090 = rack.can_move.ContainsKey(fdeg_090) ? rack.can_move[fdeg_090] : false; // 左
                            bool c_180 = rack.can_move.ContainsKey(fdeg_180) ? rack.can_move[fdeg_180] : false; // 後
                            bool c_270 = rack.can_move.ContainsKey(fdeg_270) ? rack.can_move[fdeg_270] : false; // 右

                            // 回転
                            int rot_deg = 999;
                            RouteCondition cur_con = con;
                            if (qr != null && qr.next_way_type.Count() > 0 && qr.next_way_type.ContainsKey(next_qr))
                            {
                                if (way_type == FloorQR.RouteType.FAT)
                                {
                                    if (nowSize != fatSize && rightAngle)
                                    {
                                        if (!qr.rack_rotatable)
                                        {
                                            var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                            if (rot_con == null) rotate_err = true;
                                            else cur_con = rot_con;
                                        }

                                        if (c_090)
                                        {
                                            rot_deg = fdeg_090;
                                        }
                                        else if (c_270)
                                        {
                                            rot_deg = fdeg_270;
                                        }
                                        else rotate_err = true;
                                    }
                                    else if (nowSize != fatSize && !rightAngle)
                                    {
                                        if (!qr.rack_rotatable)
                                        {
                                            var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                            if (rot_con == null) rotate_err = true;
                                            else cur_con = rot_con;
                                        }

                                        rot_deg = agv_deg;
                                    }
                                    else
                                    {
                                        if (!c_000)
                                        {
                                            if (!qr.rack_rotatable)
                                            {
                                                var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                                if (rot_con == null) rotate_err = true;
                                                else cur_con = rot_con;
                                            }

                                            if (!c_180) rotate_err = true;
                                            rot_deg = fdeg_180 - rack_deg == 180 ? 180 : 0;
                                        }
                                    }
                                }
                                else if (way_type == FloorQR.RouteType.THIN)
                                {
                                    if (nowSize != thinSize && rightAngle)
                                    {
                                        if (!qr.rack_rotatable)
                                        {
                                            var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                            if (rot_con == null) rotate_err = true;
                                            else cur_con = rot_con;
                                        }

                                        if (c_090)
                                        {
                                            rot_deg = fdeg_090;
                                        }
                                        else if (c_270)
                                        {
                                            rot_deg = fdeg_270;
                                        }
                                        else rotate_err = true;
                                    }
                                    else if (nowSize != thinSize && !rightAngle)
                                    {
                                        if (!qr.rack_rotatable)
                                        {
                                            var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                            if (rot_con == null) rotate_err = true;
                                            else cur_con = rot_con;
                                        }

                                        rot_deg = agv_deg;
                                    }
                                    else
                                    {
                                        if (!c_000)
                                        {
                                            if (!qr.rack_rotatable)
                                            {
                                                var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                                if (rot_con == null) rotate_err = true;
                                                else cur_con = rot_con;
                                            }

                                            if (!c_180) rotate_err = true;
                                            rot_deg = fdeg_180 - rack_deg == 180 ? 180 : 0;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!c_000)
                                    {
                                        if (!qr.rack_rotatable)
                                        {
                                            var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                            if (rot_con == null) rotate_err = true;
                                            else cur_con = rot_con;
                                        }

                                        if (c_090)
                                        {
                                            rot_deg = fdeg_090;
                                        }
                                        else if (c_270)
                                        {
                                            rot_deg = fdeg_270;
                                        }
                                        else if (c_180)
                                        {
                                            rot_deg = fdeg_180 - rack_deg == 180 ? 180 : 0;
                                        }
                                        else rotate_err = true;
                                    }
                                }
                            }
                            else
                            {
                                if (!c_000)
                                {
                                    if (!qr.rack_rotatable)
                                    {
                                        var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                        if (rot_con == null) rotate_err = true;
                                        else cur_con = rot_con;
                                    }

                                    if (c_090)
                                    {
                                        rot_deg = fdeg_090;
                                    }
                                    else if (c_270)
                                    {
                                        rot_deg = fdeg_270;
                                    }
                                    else if (c_180)
                                    {
                                        rot_deg = fdeg_180 - rack_deg == 180 ? 180 : 0;
                                    }
                                    else rotate_err = true;
                                }
                            }

                            if (rot_deg != 999 && !(prev_way_type == way_type && !rightAngle))
                            {
                                cur_con.rack_turn_arrive = true;
                                cur_con.rack_turn_arrive_degree = (360 - (rot_deg + diffdeg)) % 360;
                                rack_deg = (rot_deg + diffdeg) % 360;
                            }

                            agv_deg = way_deg;
                            prevs.Add(con);
                        }
                        #endregion
                    }
                    else if (CalculationType == RackDegCalculationType.AGV_REFERENCE_TYPE)
                    {
                        Dictionary<FloorQR.RouteType, List<int>> way_type_deg = new Dictionary<FloorQR.RouteType, List<int>>();
                        way_type_deg[FloorQR.RouteType.NONE] = new List<int>();
                        way_type_deg[FloorQR.RouteType.FAT] = new List<int>();
                        way_type_deg[FloorQR.RouteType.THIN] = new List<int>();
                        foreach (var v in targetAgv.rack.can_move)
                        {
                            if (v.Value)
                            {
                                way_type_deg[FloorQR.RouteType.NONE].Add(v.Key);

                                if (targetAgv.rack.sizeL < targetAgv.rack.sizeW)
                                {
                                    if (v.Key == 0 || v.Key == 180) way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                                    else if (v.Key == 90 || v.Key == 270) way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                }
                                else if (targetAgv.rack.sizeW < targetAgv.rack.sizeL)
                                {
                                    if (v.Key == 0 || v.Key == 180) way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                    else if (v.Key == 90 || v.Key == 270) way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                                }
                                else
                                {
                                    way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                    way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                                }
                            }
                        }

                        int now_deg = targetAgv.degree;

                        int now_rack_deg = (int)(now_deg - targetAgv.rack.degree + 360) % 360;
                        if (now_rack_deg % 90 != 0)
                        {
                            //角度誤差の吸収
                            if (now_rack_deg <= 2 || 358 <= now_rack_deg) now_rack_deg = 0;
                            if (88 <= now_rack_deg && now_rack_deg <= 92) now_rack_deg = 90;
                            if (178 <= now_rack_deg && now_rack_deg <= 182) now_rack_deg = 180;
                            if (268 <= now_rack_deg && now_rack_deg <= 272) now_rack_deg = 270;
                        }

                        int rack_rotatable_pos = -1;
                        int need_deg_rack = 999;
                        int need_deg_agv = 999;

                        for (int i = 0; i < cons.Count; i++)
                        {
                            var v = cons[i];

                            if (v.cur_qr.rack_rotatable) rack_rotatable_pos = i;

                            if (v.next_condition == null)
                            {
                                v.agv_turn_arrive_degree = now_deg;
                                v.rack_turn_arrive_degree = now_rack_deg;

                                break;
                            }
                            else
                            {
                                int next_deg = v.Direction;
                                int next_rack_deg = now_rack_deg;

                                if (now_deg != next_deg)
                                {
                                    int diff = (next_deg - now_deg + 360) % 360;
                                    next_rack_deg = (now_rack_deg + diff + 360) % 360;

                                    if (diff % 90 != 0)
                                    {
                                        next_rack_deg = (next_rack_deg - diff + 360) % 360;
                                        //v.rack_turn_arrive = true;
                                    }
                                }

                                if (need_deg_rack != 999)
                                {
                                    int diff = (need_deg_agv - now_deg + 360) % 360;
                                    next_rack_deg = (need_deg_rack - diff + 360) % 360;
                                }

                                if (v.cur_qr.floor.code == v.next_condition.cur_qr.floor.code)
                                {
                                    var next_way_type = v.cur_qr.next_way_type[v.next_condition.cur_qr];
                                    List<int> canmove_rack_deg = way_type_deg[next_way_type];

                                    if (!canmove_rack_deg.Contains(next_rack_deg))
                                    {
                                        if (!v.cur_qr.rack_rotatable)
                                        {
                                            if (0 <= rack_rotatable_pos)
                                            {
                                                i = rack_rotatable_pos - 1;
                                                need_deg_rack = canmove_rack_deg[0];
                                                need_deg_agv = next_deg;
                                                continue;
                                            }
                                            else
                                            {
                                                rotate_err = true;
                                                break;
                                            }
                                        }

                                        now_rack_deg = canmove_rack_deg[0];
                                        v.rack_turn_arrive = true;
                                    }
                                    else
                                    {
                                        if (need_deg_rack != 999)
                                        {
                                            v.rack_turn_arrive = true;
                                            rack_rotatable_pos = -1;
                                        }

                                        now_rack_deg = next_rack_deg;
                                    }

                                    need_deg_rack = 999;

                                    now_deg = next_deg;
                                }

                                v.agv_turn_arrive_degree = now_deg;
                                v.rack_turn_arrive_degree = now_rack_deg;
                            }
                        }
                    }

                    // 棚を回転できない＝ゴールに到達不可
                    if (rotate_err)
                        return new RouteConditionList();


                    if (0 < cons.Count())
                    {
                        //目的地に到達できてないなら全消去
                        if (cons.Last().Location != desinationQR.Location)
                        {
                            cons.Clear();
                        }
                    }
                }


                if (2000 < sw.ElapsedMilliseconds)
                {
                    targetAgv.agvRunner.Log("処理時間 GetMoveConditions:" + sw.ElapsedMilliseconds.ToString() + "ms");
                }

                return cons;
            }

            /// <summary>
            /// 指定したAGVを指定したQRまで移動するまでRouteConditionリストを作成
            /// </summary>
            /// <param name="targetAgv">操作対象のAGV</param>
            /// <param name="desinationQR">目的地のQR</param>
            /// <returns></returns>
            public RouteConditionList GetMoveConditions(FloorAGV targetAgv, FloorQR desinationQR)
            {
                return GetMoveConditions(targetAgv, desinationQR, string.Empty);
            }

            /// <summary>
            /// 指定したAGVを指定したQRまで移動し、棚上げ動作をおこなうまでのRouteConditionリストを作成
            /// </summary>
            /// <param name="targetAgv">操作対象のAGV</param>
            /// <param name="desinationQR">目的地のQR</param>
            /// <returns>RouteContionリスト</returns>
            public RouteConditionList GetTakeRackConditions(FloorAGV targetAgv, FloorQR desinationQR)
            {
                var route = GetShortestRoute(targetAgv.on_qr, desinationQR, targetAgv.rack == null ? SearchRule.None : SearchRule.AvoidRack, null, targetAgv, null, false);
                var cons = GenerateConditions(targetAgv, route, ConditionRule.TakeRack);

                return cons;
            }

            /// <summary>
            /// 指定したAGVを指定したQRまで移動し、棚下げ動作を行うまでのRouteConditionリストを作成
            /// </summary>
            /// <param name="targetAgv"></param>
            /// <param name="desinationQR"></param>
            /// <returns></returns>
            public RouteConditionList GetDelivaryRackConditions(FloorAGV targetAgv, FloorQR desinationQR)
            {
                var route = GetShortestRoute(targetAgv.on_qr, desinationQR, targetAgv.rack == null ? SearchRule.None : SearchRule.AvoidRack, null, targetAgv, null, false);
                var cons = GenerateConditions(targetAgv, route, ConditionRule.RackToStation, enPlaceOperate.RACK_DOWN);

                return cons;
            }

            /// <summary>
            /// 指定したAGVをfromからtoへ移動するRouteConditionを生成する
            /// </summary>
            /// <param name="agv">操作対象のAGV</param>
            /// <param name="from">開始地点</param>
            /// <param name="to">到着地点</param>
            /// <returns>RouteConditionリスト</returns>
            public RouteConditionList GetAutoRoute(FloorAGV agv, FloorQR from, FloorQR to)
            {
                List<FloorQR> qrs = GetShortestRoute(from, to, SearchRule.None, null, agv, null, false);
                RouteConditionList route = GenerateConditions(agv, qrs, ConditionRule.Move, enPlaceOperate.NONE);

                return route;
            }

            /// <summary>
            /// ステーションの方向へ棚面を回転させる角度の計算
            /// </summary>
            /// <param name="stationDegree">ステーション角度</param>
            /// <param name="rackDegree">棚面角度</param>
            /// <returns></returns>
            private static int CalcRackRotateDegree(int stationDegree, int rackDegree)
            {
                int rotateDeg = stationDegree - rackDegree;
                if (rotateDeg > 180) rotateDeg = rotateDeg - 360;
                return rotateDeg;
            }

            /// <summary>
            /// 与えられた角度に最も近い直角（0, 90, 180, 270)を返す
            /// </summary>
            /// <param name="degree">角度</param>
            /// <returns>直角（0, 90, 180, 270)</returns>
            private int CalcDegreeNearRightAngle(double degree)
            {
                var deg = 0;
                if (0 <= degree && degree <= 45 || 315 < degree) deg = 0;
                if (45 < degree && degree <= 135) deg = 90;
                if (135 < degree && degree <= 225) deg = 180;
                if (225 < degree && degree <= 315) deg = 270;

                return deg;
            }

            /// <summary>
            /// 移動方向に向く面を計算する
            /// </summary>
            /// <param name="now_face">進行方向の面ID</param>
            /// <param name="next_deg">次のQRへの角度</param>
            /// <param name="prev_deg">前のQRへの角度</param>
            /// <returns>向く面ID</returns>
            private int CalcRunFace(int now_face, int next_deg, int prev_deg)
            {
                if (now_face == 0) return 0;

                var deg = face2degree[now_face];
                var diffdeg = ((180 - CalcDegreeNearRightAngle(prev_deg)) - CalcDegreeNearRightAngle(next_deg)) % 360;
                var need_face_degree = (deg + diffdeg) % 360;

                if (need_face_degree < 0) need_face_degree = (360 + need_face_degree) % 360;

                return degree2face[need_face_degree];
            }

            #endregion

            #region 対象情報取得

            /// <summary>
            /// 目的QR位置まで最短でたどり着けるAGVを取得する
            /// </summary>
            /// <param name="targetQR">対象の位置</param>
            /// <returns></returns>
            public FloorAGV GetNearestAGV(FloorQR targetQR)
            {
                List<FloorQR> route;
                return GetNearestAGV(targetQR, out route);
            }

            /// <summary>
            /// 目的QR位置まで最短でたどり着けるAGVを取得する
            /// </summary>
            /// <param name="targetQR">対象の位置</param>
            /// <returns>AGVインスタンス</returns>
            protected FloorAGV GetNearestAGV(FloorQR targetQR, out List<FloorQR> route)
            {
                FloorAGV agv = null;
                route = null;

                var openAgvs = controller.agvs.Where(e => e.agvRunner.agv_request);

                foreach (var a in openAgvs)
                {
                    var r = GetShortestRoute(a.on_qr, targetQR, SearchRule.None, null, a, null, false);
                    if (route == null || route.Count > r.Count)
                    {
                        route = r;
                        agv = a;
                    }
                }

                return agv;
            }

            /// <summary>
            /// 指定されたRouteConditionのリストのコストを計算する
            /// </summary>
            /// <param name="conditions">計算したいRouteConditionのリスト</param>
            /// <param name="blockRack">ルート生成時に棚を壁として扱うかを設定するフラグ true:扱う false:扱わない</param>
            /// <returns>コスト</returns>
            public int CalcRouteConditionCost(RouteConditionList conditions, bool blockRack = true)
            {
                if (conditions.Count == 0) return int.MaxValue;

                var owner_agv = conditions.First().owner_agv;
                SearchRule rule = GetRouteSearchRule(owner_agv, blockRack);

                CostQR qr = null;
                foreach(var c in conditions)
                {
                    qr = new CostQR(c.cur_qr, qr, owner_agv.degree, 999, rule);
                }

                if (qr == null) return int.MaxValue;
                return qr.cost;
            }

            /// <summary>
            /// 指定されたルートのコストを計算する
            /// </summary>
            /// <param name="route">計算したいルート</param>
            /// <param name="targetAGV">対象となるAGV</param>
            /// <param name="blockRack">ルート生成時に棚を壁として扱うかを設定するフラグ true:扱う false:扱わない</param>
            /// <returns></returns>
            public int CalcRouteQrCost(IReadOnlyList<FloorQR> route, FloorAGV targetAGV, bool blockRack = true)
            {
                SearchRule rule = GetRouteSearchRule(targetAGV, blockRack);

                CostQR qr = null;
                foreach(var q in route)
                {
                    qr = new CostQR(q, qr, targetAGV.degree, 999, rule);
                }

                if (qr == null) return int.MaxValue;
                return qr.cost;
            }

            /// <summary>
            /// 対象AGVのルート検索のルールを取得する
            /// </summary>
            /// <param name="targetAGV">対象AGV</param>
            /// <param name="blockRack">ルート生成時に棚を壁として扱うかを設定するフラグ true:扱う false:扱わない</param>
            /// <returns></returns>
            private SearchRule GetRouteSearchRule(FloorAGV targetAGV, bool blockRack)
            {
                if (targetAGV == null) return SearchRule.None;
                SearchRule rule = SearchRule.None;

                if (blockRack && targetAGV.rack != null)
                {
                    rule = SearchRule.ExclusionRack;
                }
                else if (!blockRack && targetAGV.rack != null)
                {
                    rule = SearchRule.AvoidRack;
                }

                return rule;
            }
            #endregion

            #region AGV回避行動

            /// <summary>
            /// ２機のAGVのどちらを操作対象とするかを選択する
            /// </summary>
            /// <param name="a">対象のAGV a</param>
            /// <param name="b">対象のAGV b</param>
            /// <returns>操作対象として選択されたAGV</returns>
            public FloorAGV SelectOperateAGV(FloorAGV a, FloorAGV b)
            {
                if (a.rack == null && b.rack != null)
                {
                    return a;
                }
                else if (a.rack != null && b.rack == null)
                {
                    return b;
                }

                var straight_a = StraightNum(a);
                var straight_b = StraightNum(b);
                if (straight_a > straight_b)
                {
                    return b;
                }
                else if (straight_a < straight_b)
                {
                    return a;
                }

                return a;
            }

            /// <summary>
            /// 指定したAGVからの最大直進距離（NOT QR数）を取得する
            /// </summary>
            /// <param name="agv">対象のAGV</param>
            /// <returns>最大直進距離</returns>
            private double StraightNum(FloorAGV agv)
            {
                int deg_a = 999;
                double dis_a = 0;
                double max_a = 0;

                var cons = agv.conditions;

                foreach(var con in cons)
                {
                    if (con.next_condition != null)
                    {
                        var curqr = con.cur_qr;
                        var nexqr = con.next_condition.cur_qr;

                        var deg = (int)curqr.Degree(nexqr);
                        if (deg_a == 999)
                        {
                            deg_a = deg;
                            dis_a += curqr.Distance(nexqr);
                        }
                        else if (deg_a == deg)
                        {
                            dis_a += curqr.Distance(nexqr);
                        }
                        else if (deg_a != deg)
                        {
                            if (max_a < dis_a)
                            {
                                max_a = dis_a;
                            }
                            dis_a = 0;
                        }
                    }
                }

                return max_a;
            }
            #endregion

            #endregion


            #region 実験的経路探索

            #region CostNode
            /// <summary>
            /// コスト付きノード
            /// </summary>
            protected class RouteSerchNode
            {
                public FloorQR qr = null;
                public int cost { get; private set; }

                public RouteSerchNode prev { get; private set; }

                public int rack_degree { get; private set; }

                private double deg = 999;
                private int straightCount = 0;
                public int depth { get; private set; }

                public bool rack_rotated { get; private set; }

                public RouteSerchNode(FloorQR qr, RouteSerchNode prev, int rack_deg, SearchRule rule)
                {
                    this.qr = qr;
                    this.prev = prev;
                    this.rack_degree = rack_deg;

                    if (prev == null)
                    {
                        deg = 999;
                        straightCount = 0;
                        cost = 0;
                        depth = 1;
                    }
                    else
                    {
                        depth = prev.depth + 1;

                        const int LowSpeedCoeff = 3;
                        const int AccelCost = 2 * LowSpeedCoeff;
                        int disCost = 0;
                        int rollCost = 0;
                        int rackCost = 0;
                        int returnCost = 0;
                        int autoratorCost = 0;
                        int rackRollCost = 0;

                        if (prev.qr.next_way.ContainsKey(qr))
                        {
                            deg = prev.qr.next_way[qr];
                            if (prev.deg != deg)
                            {
                                straightCount = 1;
                            }
                            else
                            {
                                straightCount = prev.straightCount + 1;
                            }
                        }

                        if (straightCount < 3)
                        {
                            disCost = depth * LowSpeedCoeff;
                        }
                        //else if (straightCount >= 3)
                        else
                        {
                            disCost = (depth - 2) + AccelCost;
                        }
                        //else
                        //{
                        //    disCost = (depth - 2);
                        //}

                        if (prev.deg != deg && prev.deg != 999)
                        {
                            rollCost = (int)(Math.Abs(prev.deg - deg) / 90) * 20;
                        }

                        rack_rotated = false;
                        if (prev.rack_degree != rack_degree)
                        {
                            rackRollCost = Math.Abs((prev.rack_degree - rack_degree) % 180);
                            rack_rotated = true;
                        }

                        if (rule == SearchRule.AvoidRack && qr.rack != null)
                        {
                            rackCost = 20000;
                        }

                        //if (rule == SearchRule.AvoidRack || rule == SearchRule.ExclusionRack)
                        //{
                        //    if (prev.qr.rack != null && qr.next_way.ContainsKey(prev.qr))
                        //    {
                        //        returnCost = -10000;
                        //    }
                        //}

                        if (prev.qr.autorator_id != string.Empty && qr.autorator_id != string.Empty)
                        {
                            int div_floor = Math.Abs(qr.floor.FloorNo - prev.qr.floor.FloorNo);
                            autoratorCost = 30000 + 100 * div_floor;
                        }

                        cost = this.prev.cost + disCost + rollCost + rackCost + returnCost + rackRollCost + autoratorCost;
                    }
                }

                public override string ToString()
                {
                    return qr.ToString() + " cost=" + cost + " depth=" + depth + " rack degree=" + rack_degree + " prev=" + (prev == null ? "null" : prev.qr.ToString());
                }
            }
            #endregion

            /// <summary>
            /// 最短経路の取得
            /// </summary>
            /// <param name="start">開始地点</param>
            /// <param name="goal">目的地</param>
            /// <param name="rule">最短経路検索の条件</param>
            /// <param name="exceptRoute">除外したいQRのリスト</param>
            /// <param name="rack">運ぶ棚</param>
            /// <param name="faceID">面指定</param>
            /// <param name="rackDirectionConsider">棚の向きを考慮してルートを生成するフラグ true:向きを考慮する false:向きを考慮しない</param>
            /// <returns>最短経路</returns>
            protected List<FloorQR> GetShortestRoute_ex(FloorQR start, FloorQR goal, SearchRule rule, IReadOnlyList<FloorQR> exceptRoute, Rack rack, string faceID, bool rackDirectionConsider)
            {
                List<RouteSerchNode> open = new List<RouteSerchNode>();
                List<RouteSerchNode> close = new List<RouteSerchNode>();

                List<FloorQR> except = new List<FloorQR>();
                if (exceptRoute != null) except.AddRange(exceptRoute);

                bool haveRack = rack != null;

                var goal_rack_degree = int.MaxValue;

                if (haveRack && rackDirectionConsider && goal.station_id.Trim() != string.Empty && faceID.Trim() != string.Empty)
                {
                    var face = rack.face_id.Where(e => e.Value == faceID).FirstOrDefault();
                    var face_degree = face.Key;
                    var st_degree = (int)goal.direction_station;

                    var degree = ((st_degree - face_degree) + 360) % 360;
                    goal_rack_degree = degree;
                }

                // way_typeごとの移動可能角度を取得
                Dictionary<FloorQR.RouteType, List<int>> way_type_deg = new Dictionary<FloorQR.RouteType, List<int>>();
                way_type_deg[FloorQR.RouteType.NONE] = new List<int>();
                way_type_deg[FloorQR.RouteType.FAT] = new List<int>();
                way_type_deg[FloorQR.RouteType.THIN] = new List<int>();

                if (haveRack)
                {
                    int now_rack_deg = (int)rack.degree;

                    //角度誤差の吸収
                    if ((0 <= now_rack_deg && now_rack_deg < 45) || 315 <= now_rack_deg) now_rack_deg = 0;
                    if (45 <= now_rack_deg && now_rack_deg < 135) now_rack_deg = 90;
                    if (135 <= now_rack_deg && now_rack_deg < 225) now_rack_deg = 180;
                    if (225 <= now_rack_deg && now_rack_deg < 315) now_rack_deg = 270;

                    foreach (var v in rack.can_move)
                    {
                        if (v.Value)
                        {
                            way_type_deg[FloorQR.RouteType.NONE].Add(v.Key);

                            if (rack.sizeL < rack.sizeW)
                            {
                                if (v.Key == 0 || v.Key == 180) way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                                else if (v.Key == 90 || v.Key == 270) way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                            }
                            else if (rack.sizeW < rack.sizeL)
                            {
                                if (v.Key == 0 || v.Key == 180) way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                else if (v.Key == 90 || v.Key == 270) way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                            }
                            else
                            {
                                way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                            }
                        }
                    }

                    open.Add(new RouteSerchNode(start, null, now_rack_deg, rule));
                }
                else
                {
                    open.Add(new RouteSerchNode(start, null, 999, rule));
                }


                RouteSerchNode cqr = null;

                Log("start=" + start.ToString() + ", goal=" + goal.ToString() + ", face=" + faceID);

                while (open.Count > 0)
                {
                    open.Sort((a, b) => a.cost - b.cost);
                    cqr = open[0];
                    open.RemoveAt(0);

                    //log.Add(cqr.ToString());
                    string open_list_str = "";
                    open.ForEach(i => open_list_str += i.ToString() + "->");
                    Log(open_list_str);

                    if (cqr.qr == goal)
                    {
                        /*
                        if (haveRack && faceID != "" && goal_rack_degree != int.MaxValue)
                        {
                            if (cqr.rack_degree == goal_rack_degree)
                            {
                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                        */
                        break;
                    }
                    else
                    {
                        close.Add(cqr);

                        List<FloorQR> next_way = new List<FloorQR>();
                        List<int> exceptionDeg = new List<int>();

                        // 今いるQRでの棚脱出可能な角度を取得
                        if (cqr.qr.rack != null)
                        {
                            // 入出チェック
                            Dictionary<int, bool> can_inout = new Dictionary<int, bool>();
                            can_inout[0] = false;
                            can_inout[90] = false;
                            can_inout[180] = false;
                            can_inout[270] = false;

                            foreach (var p in cqr.qr.rack.can_inout)
                            {
                                if (can_inout.ContainsKey(p.Key)) can_inout[p.Key] = p.Value;
                            }

                            foreach (var p in can_inout)
                            {
                                if (!p.Value)
                                {
                                    int deg = (int)((p.Key + cqr.qr.rack.degree) % 360);
                                    exceptionDeg.Add(deg);
                                }
                            }

                            // 棚を持っているときに移動不可能な方向を算出
                            if (haveRack)
                            {
                                Dictionary<int, bool> can_move = new Dictionary<int, bool>();
                                can_move[0] = false;
                                can_move[90] = false;
                                can_move[180] = false;
                                can_move[270] = false;

                                foreach (var p in cqr.qr.rack.can_move)
                                {
                                    if (can_move.ContainsKey(p.Key)) can_move[p.Key] = p.Value;
                                }

                                foreach (var p in can_move)
                                {
                                    if (!p.Value && !cqr.qr.rack_rotatable)
                                    {
                                        int deg = (int)((p.Key + cqr.qr.rack.degree) % 360);
                                        exceptionDeg.Add(deg);
                                    }
                                }
                            }
                        }
                        // 次のQRへの進入可能かを確認
                        foreach (var v in cqr.qr.next_way)
                        {
                            if (exceptionDeg.Contains((int)(v.Value))) continue;

                            var r = v.Key.rack;
                            if (r == null)
                            {
                                next_way.Add(v.Key);
                            }
                            else
                            {
                                Dictionary<int, bool> can_inout = new Dictionary<int, bool>();
                                can_inout[0] = false;
                                can_inout[90] = false;
                                can_inout[180] = false;
                                can_inout[270] = false;

                                foreach (var p in r.can_inout)
                                {
                                    if (can_inout.ContainsKey(p.Key)) can_inout[p.Key] = p.Value;
                                }

                                foreach (var p in can_inout)
                                {
                                    if (((int)v.Value) == (((p.Key + r.degree) % 360 + 180) % 360))
                                    {
                                        if (p.Value) next_way.Add(v.Key);
                                    }
                                }
                            }
                        }

                        // オートレータの次QRを追加
                        if (cqr.qr.autorator_id != string.Empty)
                        {
                            var con_rator = controller.AllQR.Where(e => e.autorator_id != string.Empty
                                                                     && e.autorator_id == cqr.qr.autorator_id
                                                                     && e.autorator_online
                                                                     && e != cqr.qr
                                                                   );
                            next_way.AddRange(con_rator);
                        }

                        if (cqr.prev != null) next_way.Remove(cqr.prev.qr);


                        #region CostQRの生成
                        List<RouteSerchNode> next_qrs = new List<RouteSerchNode>();
                        {
                            foreach (var v in next_way)
                            {
                                bool existAGV = v.on_agv != null;
                                bool existAgvRunner = existAGV ? v.on_agv.agvRunner != null : false;
                                bool existCommunicator = existAgvRunner ? v.on_agv.agvRunner.communicator != null : false;

                                // AGVがいる かつ AgvRunnerがある かつ communicatorがある場合のみ
                                // 上記以外は異常なしとして通常処理へ
                                if (existAGV && existAgvRunner && existCommunicator)
                                {
                                    // error_code != 0 であれば異常あり、0 であれば通常処理へ
                                    if (v.on_agv.agvRunner.communicator.GetState.error_code != 0)
                                    {
                                        // 異常のあるAGVがいるQRを除外
                                        continue;
                                    }
                                }

                                // オートレータの状態確認
                                // offlineの場合は除外
                                if (v.autorator_id != string.Empty && !v.autorator_online) continue;

                                int rackdeg = cqr.rack_degree;
                                if (haveRack && rackDirectionConsider)
                                {
                                    if (!cqr.qr.next_way.ContainsKey(v))
                                    {
                                        if (cqr.qr.autorator_id.Trim() != "")
                                        {
                                            // オートレーター
                                            next_qrs.Add(new RouteSerchNode(v, cqr, rackdeg, rule));
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        var next_way_type = cqr.qr.next_way_type[v];
                                        var next_way_degree = (int)cqr.qr.Degree(v);

                                        if (next_way_degree % 90 != 0)
                                        {
                                            //角度誤差の吸収
                                            if ((0 <= next_way_degree && next_way_degree < 45) || 315 <= next_way_degree) next_way_degree = 0;
                                            if (45 <= next_way_degree && next_way_degree < 135) next_way_degree = 90;
                                            if (135 <= next_way_degree && next_way_degree < 225) next_way_degree = 180;
                                            if (225 <= next_way_degree && next_way_degree < 315) next_way_degree = 270;
                                        }


                                        if (cqr.qr.rack_rotatable) // 棚を回転加野であれば総当たりで回転させる
                                        {
                                            for (int diff = 0; diff <= 270; diff += 90)
                                            {
                                                var target_rack_degree = (rackdeg + diff) % 360;
                                                var rack_face_deg = ((next_way_degree - target_rack_degree) + 360) % 360;

                                                if (way_type_deg[next_way_type].Contains(rack_face_deg))
                                                {
                                                    next_qrs.Add(new RouteSerchNode(v, cqr, target_rack_degree, rule));
                                                }
                                            }
                                        }
                                        else
                                        {
                                            var rack_face_deg = ((next_way_degree - rackdeg) + 360) % 360;

                                            if (way_type_deg[next_way_type].Contains(rack_face_deg))
                                            {
                                                next_qrs.Add(new RouteSerchNode(v, cqr, rackdeg, rule));
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    next_qrs.Add(new RouteSerchNode(v, cqr, rackdeg, rule));
                                }
                            }
                        }
                        #endregion

                        foreach (var next_qr in next_qrs)
                        {
                            if (!open.Exists(e => e.qr == next_qr.qr && ((e.prev == null || cqr.prev == null) ? true : e.prev.qr == cqr.prev.qr)) &&
                                !close.Exists(e => e.qr == next_qr.qr && (e.prev == null ? true : e.prev.qr == next_qr.prev.qr) && e.rack_degree == next_qr.rack_degree) &&
                                !except.Contains(next_qr.qr) &&
                                (haveRack ? cqr.qr.direction_charge == FloorQR.enDirection.NONE : true))
                            {
                                if (rule == SearchRule.ExclusionRack)
                                {
                                    if (next_qr.qr.rack == null)
                                    {
                                        open.Add(next_qr);
                                    }
                                }
                                else
                                {
                                    open.Add(next_qr);
                                }
                            }
                        }
                    }
                }

                List<FloorQR> route = new List<FloorQR>();

                if (cqr.qr.Location == goal.Location)
                { 
                    while (cqr != null)
                    {
                        Log(cqr.ToString());
                        route.Add(cqr.qr);
                        cqr = cqr.prev;
                    }
                    route.Reverse();
                }

                return route;
            }

            public RouteConditionList GetMoveConditions_ex(FloorAGV targetAgv, FloorQR desinationQR, string faceID, IReadOnlyList<FloorQR> exceptQR = null, bool blockRack = true, bool rackDirectionConsider = true)
            {
                return GetMoveConditions_ex(targetAgv, targetAgv.on_qr, desinationQR, faceID, exceptQR, blockRack, rackDirectionConsider);
            }

            /// <summary>
            /// 指定したAGVを指定したQRまで移動するまでRouteConditionリストを作成
            /// 棚面が指定されている かつ 指定したQRがSTの場合に棚の回転処理も行う
            /// </summary>
            /// <param name="targetAgv">操作対象のAGV</param>
            /// <param name="desinationQR">目的地のQR</param>
            /// <param name="faceID">棚面のID</param>
            /// <param name="exceptQR">ルート生成から除外したいQR</param>
            /// <param name="blockRack">ルート生成時に棚を壁として扱うかを設定するフラグ true:扱う false:扱わない</param>
            /// <param name="rackDirectionConsider">棚の向きを考慮してルートを生成するフラグ true:向きを考慮する false:向きを考慮しない</param>
            /// <returns>RouteConditionリスト</returns>
            public RouteConditionList GetMoveConditions_ex(FloorAGV targetAgv, FloorQR fromQR, FloorQR desinationQR, string faceID, IReadOnlyList<FloorQR> exceptQR = null, bool blockRack = true, bool rackDirectionConsider = true)
            {
                bool needRotable = false;
                int degree = 999;
                var rule = GetRouteSearchRule(targetAgv, blockRack);
                List<FloorQR> route = null;
                bool haveRack = targetAgv.rack != null;

                if (haveRack && desinationQR.station_id.Trim() != string.Empty && faceID.Trim() != string.Empty)
                {
                    var face = targetAgv.rack.face_id.Where(e => e.Value == faceID).FirstOrDefault();
                    degree = CalcRackRotateDegree((int)desinationQR.direction_station, face.Key);

                    //@@@AGVが斜め向きじゃない時だけ棚の向きを考慮させる
                    if (targetAgv.degree % 90 == 0)
                    {
                        if (!targetAgv.rack.degree_received || (degree + 360) % 360 != targetAgv.rack.degree)
                        {
                            //実際の棚の向きが反映されていない場合、とにかく棚旋回を必要とする
                            needRotable = true;
                        }
                    }
                }

                
                route = GetShortestRoute_ex(fromQR, desinationQR, rule, exceptQR, targetAgv.rack, faceID, rackDirectionConsider);

                if (0 == route.Count() || route.Last().Location != desinationQR.Location)
                {
                    //目的地に到達できてない
                    return new RouteConditionList();
                }

                // STに合わせて棚を回転
                if (needRotable)
                {
                    if (route.Where(e => e.rack_rotatable).Count() == 0)
                    {
                        var all_rotable_qrs = controller.AllQR.Where(e => e.rack_rotatable);
                        var rotable_qr = controller.AllQR.Where(e => e.rack_rotatable)
                                                         .OrderBy(e => CalcRouteQrCost(
                                                                    GetShortestRoute_ex(fromQR, e, rule, exceptQR, targetAgv.rack, faceID, rackDirectionConsider),
                                                                    targetAgv,
                                                                    blockRack)
                                                         ).FirstOrDefault();
                        if (rotable_qr == null || !all_rotable_qrs.Contains(rotable_qr)) return new RouteConditionList(); // 回転可能なQRに到達できない

                        var r1 = GetShortestRoute_ex(fromQR, rotable_qr, rule, exceptQR, targetAgv.rack, faceID, rackDirectionConsider);
                        var r2 = GetShortestRoute_ex(rotable_qr, desinationQR, rule, exceptQR, targetAgv.rack, faceID, rackDirectionConsider);
                        r2.RemoveAt(0);
                        route = new List<FloorQR>(r1);
                        route.AddRange(r2);
                    }
                }

                var cons = GenerateConditions(targetAgv, route, ConditionRule.RackToStation);

                if (needRotable)
                {
                    var con = cons.Where(e => e.rack_turn_arrive == true || e.rack_turn_departure == true).LastOrDefault();

                    if (con != null)
                    {
                        if (con.rack_turn_arrive) con.rack_turn_arrive_degree = degree;
                        if (con.rack_turn_departure) con.rack_turn_departure_degree = degree;
                    }
                }
                else
                {
                    var con = cons.Where(e => e.rack_turn_arrive == true || e.rack_turn_departure == true).LastOrDefault();

                    if (con != null)
                    {
                        con.rack_turn_arrive = false;
                        con.rack_turn_departure = false;
                    }
                }

                for (int i = 0; i < cons.Count; i++)
                {
                    var v = cons[i];

                    if (v.RoutingAutoratorQR() != null)
                    {
                        v.wait_autorator_in_trg = true;
                        break;
                    }
                }

                var rotate_err = false;

                if (haveRack && needRotable)
                {
                    Dictionary<FloorQR.RouteType, List<int>> way_type_deg = new Dictionary<FloorQR.RouteType, List<int>>();
                    way_type_deg[FloorQR.RouteType.NONE] = new List<int>();
                    way_type_deg[FloorQR.RouteType.FAT] = new List<int>();
                    way_type_deg[FloorQR.RouteType.THIN] = new List<int>();
                    foreach (var v in targetAgv.rack.can_move)
                    {
                        if (v.Value)
                        {
                            way_type_deg[FloorQR.RouteType.NONE].Add(v.Key);

                            if (targetAgv.rack.sizeL < targetAgv.rack.sizeW)
                            {
                                if (v.Key == 0 || v.Key == 180) way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                                else if (v.Key == 90 || v.Key == 270) way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                            }
                            else if (targetAgv.rack.sizeW < targetAgv.rack.sizeL)
                            {
                                if (v.Key == 0 || v.Key == 180) way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                else if (v.Key == 90 || v.Key == 270) way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                            }
                            else
                            {
                                way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                            }
                        }
                    }

                    int now_deg = targetAgv.degree;

                    int now_rack_deg = (int)(now_deg - targetAgv.rack.degree + 360) % 360;
                    if (now_rack_deg % 90 != 0)
                    {
                        //角度誤差の吸収
                        if (now_rack_deg <= 2 || 358 <= now_rack_deg) now_rack_deg = 0;
                        if (88 <= now_rack_deg && now_rack_deg <= 92) now_rack_deg = 90;
                        if (178 <= now_rack_deg && now_rack_deg <= 182) now_rack_deg = 180;
                        if (268 <= now_rack_deg && now_rack_deg <= 272) now_rack_deg = 270;
                    }

                    int rack_rotatable_pos = -1;
                    int need_deg_rack = 999;
                    int need_deg_agv = 999;

                    for (int i = 0; i < cons.Count; i++)
                    {
                        var v = cons[i];

                        if (v.cur_qr.rack_rotatable) rack_rotatable_pos = i;

                        if (v.next_condition == null)
                        {
                            //v.agv_turn_arrive_degree = now_deg;
                            //v.rack_turn_arrive_degree = now_rack_deg;

                            break;
                        }
                        else
                        {
                            int next_deg = v.Direction;
                            int next_rack_deg = now_rack_deg;

                            if (now_deg != next_deg)
                            {
                                int diff = (next_deg - now_deg + 360) % 360;
                                next_rack_deg = (now_rack_deg + diff + 360) % 360;

                                if (diff % 90 != 0)
                                {
                                    next_rack_deg = (next_rack_deg - diff + 360) % 360;
                                    //v.rack_turn_arrive = true;
                                }
                            }

                            if (need_deg_rack != 999)
                            {
                                int diff = (need_deg_agv - now_deg + 360) % 360;
                                next_rack_deg = (need_deg_rack - diff + 360) % 360;
                            }

                            if (v.cur_qr.floor.code == v.next_condition.cur_qr.floor.code)
                            {
                                var next_way_type = v.cur_qr.next_way_type[v.next_condition.cur_qr];
                                List<int> canmove_rack_deg = way_type_deg[next_way_type];

                                if (!canmove_rack_deg.Contains(next_rack_deg))
                                {
                                    if (!v.cur_qr.rack_rotatable)
                                    {
                                        if (0 <= rack_rotatable_pos)
                                        {
                                            i = rack_rotatable_pos - 1;
                                            need_deg_rack = canmove_rack_deg[0];
                                            need_deg_agv = next_deg;
                                            continue;
                                        }
                                        else
                                        {
                                            rotate_err = true;
                                            break;
                                        }
                                    }
                                    else if (!canmove_rack_deg.Contains((v.rack_turn_arrive_degree + 360) % 360))
                                    {
                                        rotate_err = true;
                                        break;
                                    }

                                    now_rack_deg = canmove_rack_deg[0];
                                    //v.rack_turn_arrive = true;
                                }
                                else
                                {
                                    if (need_deg_rack != 999)
                                    {
                                        //v.rack_turn_arrive = true;
                                        rack_rotatable_pos = -1;
                                    }

                                    now_rack_deg = next_rack_deg;
                                }

                                need_deg_rack = 999;

                                now_deg = next_deg;
                            }

                            //v.agv_turn_arrive_degree = now_deg;
                            //v.rack_turn_arrive_degree = now_rack_deg;
                        }
                    }
                }
                else if (haveRack)
                {
                    if (CalculationType == RackDegCalculationType.MAP_REFERENCE_TYPE)
                    {
                        #region MAP上0°を基準としてRACK_DEGを算出
                        var agv_deg = (int)targetAgv.degree;
                        var rack = targetAgv.rack;

                        var rack_deg = (360 - (int)rack.degree) % 360;

                        if (0 <= rack.degree && rack.degree <= 45 || 315 < rack.degree) rack_deg = 0;
                        if (45 < rack.degree && rack.degree <= 135) rack_deg = (360 - 90) % 360;
                        if (135 < rack.degree && rack.degree <= 225) rack_deg = (360 - 180) % 360;
                        if (225 < rack.degree && rack.degree <= 315) rack_deg = (360 - 270) % 360;

                        List<RouteCondition> prevs = new List<RouteCondition>();

                        foreach (var con in cons)
                        {
                            var next_con = con.next_condition;
                            var next_qr = next_con != null ? next_con.cur_qr : null;

                            var qr = con.cur_qr;

                            var prev_con = con.prev_condition;
                            var prev = prev_con != null ? prev_con.cur_qr : null;

                            if (next_qr == null) continue; // 終端
                            if (!qr.next_way.ContainsKey(next_qr)) continue; // オートレータ

                            bool rightAngle = qr.next_way[next_qr] % 90 == 0;
                            bool prev_rightAngle = prev != null ? prev.next_way[qr] % 90 == 0 : true;
                            var way_type = qr.next_way_type.ContainsKey(next_qr) ? qr.next_way_type[next_qr] : FloorQR.RouteType.NONE;
                            var prev_way_type = prev != null ? (prev.next_way_type.ContainsKey(qr) ? prev.next_way_type[qr] : FloorQR.RouteType.NONE) : FloorQR.RouteType.NONE;

                            // 前回が斜めであるなら、棚は進む角度を向いている
                            if (!prev_rightAngle)
                            {
                                if ((int)qr.next_way[next_qr] % 180 == 0) rack_deg = (int)qr.next_way[next_qr] + (rack_deg % 180 == 0 ? 180 : 0);
                                else rack_deg = (int)qr.next_way[next_qr];
                            }

                            int way_deg = rightAngle ? (int)qr.next_way[next_qr] : agv_deg % 90 == 0 ? agv_deg : rack_deg;
                            int face_deg = (rack_deg + way_deg) % 360;

                            int diffdeg = rightAngle ? (way_deg - 180) % 180 : 0;

                            int fdeg_000 = face_deg;
                            int fdeg_090 = (face_deg + 90) % 360;
                            int fdeg_180 = (face_deg + 180) % 360;
                            int fdeg_270 = (face_deg + 270) % 360;

                            int fatSize = rack.sizeL >= rack.sizeW ? rack.sizeL : rack.sizeW;
                            int thinSize = rack.sizeL < rack.sizeW ? rack.sizeL : rack.sizeW;
                            int nowSize = face_deg % 180 == 0 ? rack.sizeL : rack.sizeW;

                            // 進もうとしている方向を基準にcan_moveを取得
                            bool c_000 = rack.can_move.ContainsKey(fdeg_000) ? rack.can_move[fdeg_000] : false; // 前
                            bool c_090 = rack.can_move.ContainsKey(fdeg_090) ? rack.can_move[fdeg_090] : false; // 左
                            bool c_180 = rack.can_move.ContainsKey(fdeg_180) ? rack.can_move[fdeg_180] : false; // 後
                            bool c_270 = rack.can_move.ContainsKey(fdeg_270) ? rack.can_move[fdeg_270] : false; // 右

                            // 回転
                            int rot_deg = 999;
                            RouteCondition cur_con = con;
                            if (qr != null && qr.next_way_type.Count() > 0 && qr.next_way_type.ContainsKey(next_qr))
                            {
                                if (way_type == FloorQR.RouteType.FAT)
                                {
                                    if (nowSize != fatSize && rightAngle)
                                    {
                                        if (!qr.rack_rotatable)
                                        {
                                            var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                            if (rot_con == null) rotate_err = true;
                                            else cur_con = rot_con;
                                        }

                                        if (c_090)
                                        {
                                            rot_deg = fdeg_090;
                                        }
                                        else if (c_270)
                                        {
                                            rot_deg = fdeg_270;
                                        }
                                        else rotate_err = true;
                                    }
                                    else if (nowSize != fatSize && !rightAngle)
                                    {
                                        if (!qr.rack_rotatable)
                                        {
                                            var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                            if (rot_con == null) rotate_err = true;
                                            else cur_con = rot_con;
                                        }

                                        rot_deg = agv_deg;
                                    }
                                    else
                                    {
                                        if (!c_000)
                                        {
                                            if (!qr.rack_rotatable)
                                            {
                                                var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                                if (rot_con == null) rotate_err = true;
                                                else cur_con = rot_con;
                                            }

                                            if (!c_180) rotate_err = true;
                                            rot_deg = fdeg_180 - rack_deg == 180 ? 180 : 0;
                                        }
                                    }
                                }
                                else if (way_type == FloorQR.RouteType.THIN)
                                {
                                    if (nowSize != thinSize && rightAngle)
                                    {
                                        if (!qr.rack_rotatable)
                                        {
                                            var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                            if (rot_con == null) rotate_err = true;
                                            else cur_con = rot_con;
                                        }

                                        if (c_090)
                                        {
                                            rot_deg = fdeg_090;
                                        }
                                        else if (c_270)
                                        {
                                            rot_deg = fdeg_270;
                                        }
                                        else rotate_err = true;
                                    }
                                    else if (nowSize != thinSize && !rightAngle)
                                    {
                                        if (!qr.rack_rotatable)
                                        {
                                            var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                            if (rot_con == null) rotate_err = true;
                                            else cur_con = rot_con;
                                        }

                                        rot_deg = agv_deg;
                                    }
                                    else
                                    {
                                        if (!c_000)
                                        {
                                            if (!qr.rack_rotatable)
                                            {
                                                var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                                if (rot_con == null) rotate_err = true;
                                                else cur_con = rot_con;
                                            }

                                            if (!c_180) rotate_err = true;
                                            rot_deg = fdeg_180 - rack_deg == 180 ? 180 : 0;
                                        }
                                    }
                                }
                                else
                                {
                                    if (!c_000)
                                    {
                                        if (!qr.rack_rotatable)
                                        {
                                            var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                            if (rot_con == null) rotate_err = true;
                                            else cur_con = rot_con;
                                        }

                                        if (c_090)
                                        {
                                            rot_deg = fdeg_090;
                                        }
                                        else if (c_270)
                                        {
                                            rot_deg = fdeg_270;
                                        }
                                        else if (c_180)
                                        {
                                            rot_deg = fdeg_180 - rack_deg == 180 ? 180 : 0;
                                        }
                                        else rotate_err = true;
                                    }
                                }
                            }
                            else
                            {
                                if (!c_000)
                                {
                                    if (!qr.rack_rotatable)
                                    {
                                        var rot_con = prevs.LastOrDefault(x => x.cur_qr.rack_rotatable);
                                        if (rot_con == null) rotate_err = true;
                                        else cur_con = rot_con;
                                    }

                                    if (c_090)
                                    {
                                        rot_deg = fdeg_090;
                                    }
                                    else if (c_270)
                                    {
                                        rot_deg = fdeg_270;
                                    }
                                    else if (c_180)
                                    {
                                        rot_deg = fdeg_180 - rack_deg == 180 ? 180 : 0;
                                    }
                                    else rotate_err = true;
                                }
                            }

                            if (rot_deg != 999 && !(prev_way_type == way_type && !rightAngle))
                            {
                                cur_con.rack_turn_arrive = true;
                                cur_con.rack_turn_arrive_degree = (360 - (rot_deg + diffdeg)) % 360;
                                rack_deg = (rot_deg + diffdeg) % 360;
                            }

                            agv_deg = way_deg;
                            prevs.Add(con);
                        }
                        #endregion
                    }
                    else if (CalculationType == RackDegCalculationType.AGV_REFERENCE_TYPE)
                    {
                        Dictionary<FloorQR.RouteType, List<int>> way_type_deg = new Dictionary<FloorQR.RouteType, List<int>>();
                        way_type_deg[FloorQR.RouteType.NONE] = new List<int>();
                        way_type_deg[FloorQR.RouteType.FAT] = new List<int>();
                        way_type_deg[FloorQR.RouteType.THIN] = new List<int>();
                        foreach (var v in targetAgv.rack.can_move)
                        {
                            if (v.Value)
                            {
                                way_type_deg[FloorQR.RouteType.NONE].Add(v.Key);

                                if (targetAgv.rack.sizeL < targetAgv.rack.sizeW)
                                {
                                    if (v.Key == 0 || v.Key == 180) way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                                    else if (v.Key == 90 || v.Key == 270) way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                }
                                else if (targetAgv.rack.sizeW < targetAgv.rack.sizeL)
                                {
                                    if (v.Key == 0 || v.Key == 180) way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                    else if (v.Key == 90 || v.Key == 270) way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                                }
                                else
                                {
                                    way_type_deg[FloorQR.RouteType.FAT].Add(v.Key);
                                    way_type_deg[FloorQR.RouteType.THIN].Add(v.Key);
                                }
                            }
                        }

                        int now_deg = targetAgv.degree;

                        int now_rack_deg = (int)(now_deg - targetAgv.rack.degree + 360) % 360;
                        if (now_rack_deg % 90 != 0)
                        {
                            //角度誤差の吸収
                            if (now_rack_deg <= 2 || 358 <= now_rack_deg) now_rack_deg = 0;
                            if (88 <= now_rack_deg && now_rack_deg <= 92) now_rack_deg = 90;
                            if (178 <= now_rack_deg && now_rack_deg <= 182) now_rack_deg = 180;
                            if (268 <= now_rack_deg && now_rack_deg <= 272) now_rack_deg = 270;
                        }

                        int rack_rotatable_pos = -1;
                        int need_deg_rack = 999;
                        int need_deg_agv = 999;

                        for (int i = 0; i < cons.Count; i++)
                        {
                            var v = cons[i];

                            if (v.cur_qr.rack_rotatable) rack_rotatable_pos = i;

                            if (v.next_condition == null)
                            {
                                v.agv_turn_arrive_degree = now_deg;
                                v.rack_turn_arrive_degree = now_rack_deg;

                                break;
                            }
                            else
                            {
                                int next_deg = v.Direction;
                                int next_rack_deg = now_rack_deg;

                                if (now_deg != next_deg)
                                {
                                    int diff = (next_deg - now_deg + 360) % 360;
                                    next_rack_deg = (now_rack_deg + diff + 360) % 360;

                                    if (diff % 90 != 0)
                                    {
                                        next_rack_deg = (next_rack_deg - diff + 360) % 360;
                                        //v.rack_turn_arrive = true;
                                    }
                                }

                                if (need_deg_rack != 999)
                                {
                                    int diff = (need_deg_agv - now_deg + 360) % 360;
                                    next_rack_deg = (need_deg_rack - diff + 360) % 360;
                                }

                                if (v.cur_qr.floor.code == v.next_condition.cur_qr.floor.code)
                                {
                                    var next_way_type = v.cur_qr.next_way_type[v.next_condition.cur_qr];
                                    List<int> canmove_rack_deg = way_type_deg[next_way_type];

                                    if (!canmove_rack_deg.Contains(next_rack_deg))
                                    {
                                        if (!v.cur_qr.rack_rotatable)
                                        {
                                            if (0 <= rack_rotatable_pos)
                                            {
                                                i = rack_rotatable_pos - 1;
                                                need_deg_rack = canmove_rack_deg[0];
                                                need_deg_agv = next_deg;
                                                continue;
                                            }
                                            else
                                            {
                                                rotate_err = true;
                                                break;
                                            }
                                        }

                                        now_rack_deg = canmove_rack_deg[0];
                                        v.rack_turn_arrive = true;
                                    }
                                    else
                                    {
                                        if (need_deg_rack != 999)
                                        {
                                            v.rack_turn_arrive = true;
                                            rack_rotatable_pos = -1;
                                        }

                                        now_rack_deg = next_rack_deg;
                                    }

                                    need_deg_rack = 999;

                                    now_deg = next_deg;
                                }

                                v.agv_turn_arrive_degree = now_deg;
                                v.rack_turn_arrive_degree = now_rack_deg;
                            }
                        }
                    }  
                }

                // 棚を回転できない＝ゴールに到達不可
                if (rotate_err)
                    return new RouteConditionList();


                if (0 < cons.Count())
                {
                    //目的地に到達できてないなら全消去
                    if (cons.Last().Location != desinationQR.Location)
                    {
                        cons.Clear();
                    }
                }

                //cons.ForEach(i => Console.WriteLine(i.ToString()));

                return cons;
            }
            #endregion

            #region logger
            /// <summary>
            /// ログファイルにログを出力
            /// </summary>
            /// <param name="msg">書き込む文字列</param>
            protected void Log(string msg)
            {
                if (output_log) _log.Add(msg);
            }
            #endregion
        }
    }
}
