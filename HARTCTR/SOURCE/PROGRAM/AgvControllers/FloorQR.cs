using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using BelicsClass.UI.Controls;
using AgvController;

using BelicsClass.Common;
using BelicsClass.ObjectSync;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        #region オートレーター情報クラス

        //public class AutoratorInfo
        //{
        //    public VtslCommunicator comm = null;
        //    public List<FloorQR> autorator_qrs = new List<FloorQR>();

        //    public AutoratorInfo(FloorQR qr)
        //    {
        //        autorator_qrs.Add(qr);
        //        comm = new VtslCommunicator(qr.autorator_id, qr.autorator_plc_ip, qr.autorator_plc_port);
        //    }
        //}

        #endregion

        #region 予約情報保持クラス

        public class ReserveQR : SynchronizedList<FloorAGV>
        {
            FloorQR qr;

            public ReserveQR(FloorQR qr)
            {
                this.qr = qr;
            }
        }


        #endregion

        #region フロア内位置情報クラス

        public class FloorQR : BL_ObjectSync
        {
            #region 方向

            public enum enDirection : int
            {
                NONE = -999,
                NORTH = 0,
                WEST = 90,
                SOUTH = 180,
                EAST = 270,
            }

            #endregion

            #region ステーションタイプ

            public enum enStatiionType : int
            {
                /// <summary></summary>
                NONE = 0,
                /// <summary>ステーション</summary>
                STATION = 1,
                /// <summary>回収</summary>
                COLLECT = 2,
                /// <summary>待機</summary>
                WAIT = 3,
                /// <summary>退避</summary>
                ESCAPE = 4,
            }

            #endregion

            #region 進路タイプ

            public enum RouteType
            {
                /// <summary>制限なし</summary>
                NONE,
                /// <summary>細い方向のみ</summary>
                THIN,
                /// <summary>太い方向のみ</summary>
                FAT,
            }

            #endregion

            #region フィールド

            [BL_ObjectSync]
            private Rack _rack = null;
            
            public Rack rack
            {
                get
                {
                    lock (this)
                    {
                        return _rack;
                    }
                }
                set
                {
                    lock (this)
                    {
                        if (_rack != value)
                        {
                            mapeditor.redraw_rack = true;
                        }

                        _rack = value;
                    }
                }
            }

            //private BL_QREncode qr = new BL_QREncode();

            public AgvMapEditor mapeditor = null;
            public FloorMap floor = null;

            [BL_ObjectSync]
            public int _x = 0;

            [BL_ObjectSync]
            public int _y = 0;

            public Point point
            {
                get { return new Point(_x, _y); }
                set { _x = value.X; _y = value.Y; }
            }

            public int x
            {
                get { return _x; }
                set
                {
                    _x = value;
                    Location = point;
                }
            }
            public int y
            {
                get { return _y; }
                set
                {
                    _y = value;
                    Location = point;
                }
            }
            public int radius = 60;
            public int autorator_radius = 240;

            public RectangleF rect = new RectangleF();
            public bool selected = false;

            public Dictionary<FloorQR, double> next_way = new Dictionary<FloorQR, double>();
            
            /// <summary>通路タイプ</summary>
            public Dictionary<FloorQR, RouteType> next_way_type = new Dictionary<FloorQR, RouteType>();

            public List<FloorQR> prev_way = new List<FloorQR>();


            [BL_ObjectSync]
            private bool _rack_rotatable = false;

            public bool rack_rotatable
            {
                get { return _rack_rotatable; }
                set
                {
                    if (_rack_rotatable != value)
                    {
                        _rack_rotatable = value;

                        //if (_rack_rotatable)
                        //{
                        //    //最大棚の回転時サイズ未満で隣接するQRを列挙
                        //    var list = floor.mapeditor.list.Where(e => e != this && Distance(e) < RackMaster.RackRotateSize);
                        //    foreach (var v in list)
                        //    {
                        //        rack_rotate_conflict_qr.Add(v);
                        //    }
                        //}
                        //else
                        //{
                        //    rack_rotate_conflict_qr.Clear();
                        //}
                    }
                }
            }

            //public List<FloorQR> rack_rotate_conflict_qr = new List<FloorQR>();

            [BL_ObjectSync]
            public string autorator_id = "          ";

            //public string autorator_plc_ip = "";
            //public int autorator_plc_port = 0;
            
            public bool autorator_online
            {
                get
                {
                    if (autorator_info != null && autorator_info.Status != null)
                    {
                        return !autorator_info.Status.IsDisable;
                    }

                    return false;
                }
            }

            public AutoratorController autorator_info = null;


            public enDirection direction_station = enDirection.NONE;
            [BL_ObjectSync]
            public string station_id = "          ";

            //public string station_ip = "";
            //public int station_port = 0;
            public enStatiionType station_type = enStatiionType.STATION;

            [BL_ObjectSync]
            public enDirection direction_charge = enDirection.NONE;
            public bool charge_back = false;

            public List<FloorAGV> on_agvs = new List<FloorAGV>();
            public bool PlaceAgv(FloorAGV agv)
            {
                bool ret = false;
                lock (this)
                {
                    var v = this;
                    if (!v.on_agvs.Contains(agv)) v.on_agvs.Add(agv);
                    if (!agv.on_qr_list.Contains(v)) agv.on_qr_list.Add(v);

                    if (0 < on_agvs.Count && on_agvs[0] == agv)
                    {
                        ret = true;
                    }
                }

                return ret;
            }

            public void UnplaceAgv(FloorAGV agv)
            {
                UnplaceAgv(agv, 9999);
            }

            public void UnplaceAgv(FloorAGV agv, int rack_deg)
            {
                lock (this)
                {
                    foreach (var v in agv.on_qr_list)
                    {
                        v.on_agvs.Remove(agv);
                    }
                    agv.on_qr_list.Clear();

                    Unlock(agv);
                    Unreserve(agv, true, rack_deg);
                }
            }

            public FloorAGV on_agv
            {
                get
                {
                    lock (this)
                    {
                        if (0 < on_agvs.Count) return on_agvs[0];
                    }
                    return null;
                }
            }

            public FloorAGV OnOtherAgv(FloorAGV agv)
            {
                if (on_agv != null && on_agv != agv) return on_agv;
                return null;
            }

            public FloorAGV OnOtherAgv_Real(FloorAGV agv)
            {
                if (on_agv != null && on_agv != agv && Location == on_agv.Location) return on_agv;
                return null;
            }

            public FloorAGV OnOtherAGV_ConflictAround(FloorAGV agv)
            {
                FloorAGV otheragv = null;
                foreach (var v in conflict_qr(agv))
                {
                    otheragv = v.OnOtherAgv(agv);
                    if (otheragv != null) break;
                }

                return otheragv;
            }

            public FloorAGV on_agv_ = null;

            //public FloorAGV __on_agv
            //{
            //    get
            //    {
            //        lock (this)
            //        {
            //            return on_agv_;
            //        }
            //    }

            //    set
            //    {
            //        lock (this)
            //        {
            //            var on_agv_pre = on_agv_;

            //            if (on_agv_pre != null)
            //            {
            //                foreach (var v in on_agv_pre.on_qr_list)
            //                {
            //                    v.on_agv_ = null;
            //                }
            //                on_agv_pre.on_qr_list.Clear();
            //            }

            //            if (on_agv_pre != value && value != null)
            //            {
            //                on_agv_ = value;
            //                if (!on_agv_.on_qr_list.Contains(this)) on_agv_.on_qr_list.Add(this);

            //                if (on_agv_ != null)
            //                {
            //                    foreach (var v in conflict_qr(value))
            //                    {
            //                        if (!on_agv_.on_qr_list.Contains(v)) on_agv_.on_qr_list.Add(v);
            //                        v.on_agv_ = on_agv_;
            //                    }
            //                }

            //                mapeditor.redraw_agvshadow = true;
            //                if (floor.controller.EventPaint != null) floor.controller.EventPaint();
            //            }
            //        }
            //    }
            //}

            //public FloorAGV lock_agv_ = null;
            //public FloorAGV lock_agv
            //{
            //    get
            //    {
            //        lock (this)
            //        {
            //            return lock_agv_;
            //        }
            //    }

            //    set
            //    {
            //        lock (this)
            //        {
            //            if (value == null)
            //            {
            //                if (rack_rotatable)
            //                {
            //                    foreach (var v in rack_rotate_conflict)
            //                    {
            //                        if (v.lock_agv == on_agv_) v.lock_agv = value;
            //                    }
            //                }
            //            }
            //            else
            //            {
            //                if (rack_rotatable && rack != null)
            //                {
            //                    foreach (var v in rack_rotate_conflict)
            //                    {
            //                        if (v.lock_agv == null)
            //                        {
            //                            v.lock_agv = value;
            //                        }
            //                        else if (v.lock_agv != value)
            //                        {

            //                        }
            //                    }
            //                }

            //                if (lock_agv_ != null && lock_agv != value)
            //                {

            //                }
            //            }

            //            lock_agv_ = value;
            //        }
            //        mapeditor.redraw_agvshadow = true;
            //        if (floor.controller.EventPaint != null) floor.controller.EventPaint();
            //    }
            //}

            public string autoratorIN_trigger = "";
            public string autoratorOUT_trigger = "";
            public bool station_complete_trigger = false;
            public bool charge_complete_trigger = false;
            //public bool autoratorHalfOpen = true;

            public List<FloorQR> floorwarp_qr = new List<FloorQR>();

            [BL_ObjectSync]
            public bool rack_setable = false;

            [BL_ObjectSync]
            public enSpeed speed_limit = enSpeed.H;

            public FloorQR escape_to = null;
            public FloorQR escape_from = null;
            
            public SynchronizedList<FloorAGV> lock_agv = new SynchronizedList<FloorAGV>();
            public ReserveQR reserve_agv = null;


            public CheckConditions GetCP
            {
                get
                {
                    if (on_agv != null)
                    {
                        var cp = on_agv.GetCP(this);
                        if (cp != null) return cp;
                    }

                    foreach (var v in floor.controller.agvs)
                    {
                        if (v == on_agv) continue;
                        var cp = v.GetCP(this);
                        if (cp != null) return cp;
                    }

                    return null;
                }
            }

            public CheckConditions GetWP
            {
                get
                {
                    if (on_agv != null)
                    {
                        var wp = on_agv.GetWP(this);
                        if (wp != null) return wp;
                    }

                    foreach (var v in floor.controller.agvs)
                    {
                        if (v == on_agv) continue;
                        var wp = v.GetWP(this);
                        if (wp != null) return wp;
                    }

                    return null;
                }
            }

            public List<FloorQR> conflict_qr(FloorAGV agv)
            {
                List<FloorQR> ret = new List<FloorQR>();

                ret.Add(this);

                if (agv.rack != null)
                {
                    //棚寸法が重なる周囲のQRを選択する

                    FloorQR.TRectangle rect = new FloorQR.TRectangle(this, agv.rack.sizeW, agv.rack.sizeL, agv.rack.degree);
                    var list = this.mapeditor.list.Where(e => Distance(e) <= 300).ToList();
                    foreach (var v in list)
                    {
                        if (v == this) continue;

                        if (this.HitRectAngle(rect, v.Location))
                        {
                            if (!ret.Contains(v)) ret.Add(v);
                        }
                    }
                }
                else
                {
                    ret = this.mapeditor.list.Where(e => Distance(e) <= agv.radius * 2).ToList();
                }

                return ret;
            }

            public bool IsLocked(FloorAGV agv)
            {
                if (on_agv != null && on_agv != agv) return false;

                lock (lock_agv)
                {
                    if (0 < lock_agv.Count && lock_agv[0] == agv) return true;
                }

                return false;
            }

            public bool IsLockOtherAgv(FloorAGV agv)
            {
                List<FloorQR> checkqr = new List<FloorQR>();
                checkqr.Add(this);

                foreach (var v in checkqr)
                {
                    if (v.on_agv != null && v.on_agv != agv) return true;
                    
                    lock (v.lock_agv)
                    {
                        if (0 < v.lock_agv.Count && v.lock_agv[0] != agv) return true;
                    }
                }

                return false;
            }

            public bool Lock(FloorAGV agv)
            {
                List<FloorQR> checkqr = new List<FloorQR>(conflict_qr(agv));

                bool exist_otheragv = false;

                foreach (var v in checkqr)
                {
                    lock (v.lock_agv)
                    {
                        if (v.on_agv != null && v.on_agv != agv) exist_otheragv = true;
                        if (0 < v.lock_agv.Count && 0 < v.lock_agv.Where(e => e != agv).Count()) exist_otheragv = true;
                    }
                }

                if (!exist_otheragv)
                {
                    lock (lock_agv)
                    {
                        if (!lock_agv.Contains(agv)) lock_agv.Add(agv);
                        if (!agv.locked_qr_list.Contains(this)) agv.locked_qr_list.Add(this);
                    }
                }

                return !exist_otheragv;
            }

            public void Unlock(FloorAGV agv)
            {
                lock (lock_agv)
                {
                    lock_agv.Remove(agv);
                    agv.locked_qr_list.Remove(this);
                }
            }

            public bool IsReserveOtherAgv(FloorAGV agv)
            {
                List<FloorQR> checkqr = new List<FloorQR>();
                //checkqr.AddRange(conflict_qr(agv));
                checkqr.Add(this);

                lock (reserve_agv)
                {
                    foreach (var v in checkqr)
                    {
                        if (v.OnOtherAgv(agv) != null) return true;
                        if (0 < v.reserve_agv.Count && v.reserve_agv[0] != agv) return true;
                    }
                }

                return false;
            }

            public FloorAGV GetTopReservedAgv()
            {
                lock (reserve_agv)
                {
                    if (0 < reserve_agv.Count)
                    {
                        return reserve_agv[0];
                    }
                }

                return null;
            }

            public int ReserveOrder_ConflictAround(FloorAGV agv)
            {
                List<FloorQR> checkqr = new List<FloorQR>();
                checkqr.AddRange(conflict_qr(agv));

                lock (reserve_agv)
                {
                    return checkqr.Max(e => e.reserve_agv.IndexOf(agv));
                }
            }

            public int Reserve(FloorAGV agv)
            {
                return Reserve(agv, false, -1);
            }

            public int Reserve(FloorAGV agv, int insertat)
            {
                return Reserve(agv, false, insertat);
            }

            public int Reserve(FloorAGV agv, bool cp)
            {
                return Reserve(agv, cp, -1);
            }

            public int Reserve(FloorAGV agv, bool cp, int insertat)
            {
                List<FloorQR> checkqr = new List<FloorQR>();
                if (cp) checkqr.Add(this);
                else checkqr.AddRange(conflict_qr(agv));

                //checkqr.AddRange(conflict_qr(agv));

                lock (reserve_agv)
                {
                    int reserved_index = -1;

                    foreach (var v in checkqr)
                    {
                        if (v.OnOtherAgv(agv) != null)
                        {
                            reserved_index = 9999;
                        }
                    }

                    if (reserved_index < 9999)
                    {
                        //予約する
                        foreach (var v in checkqr)
                        {
                            if (!v.reserve_agv.Contains(agv))
                            {
                                if (insertat < 0) v.reserve_agv.Add(agv);
                                else v.reserve_agv.Insert(insertat, agv);
                            }
                            if (!agv.reserved_qr_list.Contains(v)) agv.reserved_qr_list.Add(v);
                        }

                        //範囲内での最大予約順を取得
                        //if (cp)
                        {
                            reserved_index = reserve_agv.IndexOf(agv);
                        }
                        //else
                        //{
                        //    reserved_index = ReserveOrder_ConflictAround(agv);
                        //}

                        //for (int i = 0; i < reserve_agv.Count; i++)
                        //{
                        //    if (reserve_agv[i] == agv)
                        //    {
                        //        if (reserved_index < i)
                        //        {
                        //            reserved_index = i;
                        //        }
                        //    }
                        //}
                    }

                    if (reserved_index == 9999) reserved_index = -1;

                    return reserved_index;
                }
            }

            public void Unreserve(FloorAGV agv)
            {
                Unreserve(agv, true, 9999, null);
            }

            public void Unreserve(FloorAGV agv, List<FloorQR> keep_reserve)
            {
                Unreserve(agv, false, 9999, keep_reserve);
            }

            public void Unreserve(FloorAGV agv, bool routing)
            {
                Unreserve(agv, routing, 9999, null);
            }

            public void Unreserve(FloorAGV agv, bool routing, int rack_deg)
            {
                Unreserve(agv, routing, rack_deg, null);
            }

            public void Unreserve(FloorAGV agv, bool routing, int rack_deg, List<FloorQR> keep_reserve)
            {
                float rackdeg_org = 9999;
                if (agv.rack != null && agv.rack.degree != rack_deg && rack_deg != 9999)
                {
                    rackdeg_org = agv.rack.degree;
                }

                for (int i = 0; i < 2; i++)
                {
                    if (rackdeg_org != 9999)
                    {
                        if (i == 1) agv.rack.degree = rack_deg;
                    }

                    List<FloorQR> curqrs = new List<FloorQR>();

                    if (routing)
                    {
                        if (0 < agv.conditions.Count)
                        {
                            //動作指示を持っている場合は、これから進むルート上にかかる部分の予約を解除しない
                            bool start = false;
                            foreach (var v in agv.conditions)
                            {
                                if (agv.on_qr == v.cur_qr) start = true;

                                if (start)
                                {
                                    curqrs.AddRange(v.cur_qr.conflict_qr(agv));
                                }
                            }
                        }
                        else
                        {
                            //動作指示を持っていない場合は、現在ポイントの予約を解除しない
                            curqrs.AddRange(agv.on_qr.conflict_qr(agv));
                        }
                    }

                    if (keep_reserve != null)
                    {
                        curqrs.AddRange(keep_reserve);
                    }

                    List<FloorQR> checkqr = new List<FloorQR>();
                    checkqr.AddRange(conflict_qr(agv));

                    lock (reserve_agv)
                    {
                        foreach (var v in checkqr)
                        {
                            if (!curqrs.Contains(v) || agv.conditions.Count <= 1)
                            {
                                v.reserve_agv.Remove(agv);
                                agv.reserved_qr_list.Remove(v);
                            }
                        }
                    }

                    if (rackdeg_org == 9999) break;
                }

                if (rackdeg_org != 9999) agv.rack.degree = rackdeg_org;
            }

            #endregion

            #region コンストラクタ・プロパティ

            public FloorQR(AgvMapEditor mapeditor, int x, int y)
            {
                this.mapeditor = mapeditor;
                this.floor = mapeditor.floor;
                this.x = x;
                this.y = y;
                this.autorator_id = "";
                this.station_id = "";

                this.reserve_agv = new ReserveQR(this);
            }

            public override string ToString()
            {
                string ret = floor.code + ((int)x).ToString("00000") + ((int)y).ToString("00000") +
                    (rack != null ? " R:" + rack.rack_no : "") +
                    (station_id != "" ? " ST:" + station_id : "");

                string r = "";

                lock (reserve_agv)
                {
                    var ra = reserve_agv.Where(e => e != null).ToList();

                    if (0 < ra.Count)
                    {
                        foreach (var v in ra)
                        {
                            if (r != "") r += ",";
                            r += v.id;
                        }
                        r = "{" + r + "}";
                    }
                }
                return ret + r;
            }

            public string QrString
            {
                get
                {
                    return floor.code + ((int)x).ToString("00000") + ((int)y).ToString("00000");
                }
            }

            public Point Location
            {
                get { return point; }
                set
                {
                    point = value;

                    Dictionary<FloorQR, double> refresh_qr = new Dictionary<FloorQR, double>();

                    foreach (var v in next_way)
                    {
                        refresh_qr[v.Key] = v.Value;
                    }
                    next_way.Clear();

                    foreach (var kv in refresh_qr)
                    {
                        next_way[kv.Key] = Degree(kv.Key);
                    }

                    refresh_qr.Clear();

                    foreach (var v in prev_way)
                    {
                        foreach (var kv in v.next_way)
                        {
                            if (kv.Key == this)
                            {
                                refresh_qr[v] = kv.Value;
                            }
                        }
                    }

                    foreach (var kv in refresh_qr)
                    {
                        kv.Key.next_way.Remove(kv.Key);
                        kv.Key.next_way[this] = kv.Key.Degree(this);

                    }

                    if (floor.controller.EventQrMoved != null) floor.controller.EventQrMoved(this);
                }
            }

            #endregion

            #region 角度・座標取得

            public double Degree(FloorQR qr)
            {
                return Degree(qr.Location);
            }

            public double Degree(PointF location)
            {
                return Degree(Location, location);
            }

            public double Degree(PointF p1, PointF p2)
            {
                double r = Math.Atan2(p1.Y - p2.Y, p1.X - p2.X);
                if (r < 0) r = r + 2 * Math.PI;
                double degree = Math.Floor(r * 360 / (2 * Math.PI));

                degree = (degree + 90) % 360;

                return degree;
            }

            public double Distance(FloorQR qr)
            {
                return Distance(Location, qr.Location);
            }

            public double Distance(PointF p1, PointF p2)
            {
                return Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
            }

            public PointF AngleDistance(PointF p, double degree, double distance)
            {
                PointF ret = new PointF();

                degree = (degree + 90 + 360) % 360;
                double radian = degree * Math.PI / 180;

                ret.Y = p.Y + (float)(Math.Sin(radian) * distance);
                ret.X = p.X + (float)(Math.Cos(radian) * distance);

                return ret;
            }

            public PointF AngleDistancePoint(PointF point, double degree, double distance)
            {
                degree = (degree + 90 + 360) % 360;
                double radian = degree * Math.PI / 180;

                PointF vertex = new PointF();
                vertex.X = (float)(point.X + (distance * Math.Cos(radian)));
                vertex.Y = (float)(point.Y + (distance * Math.Sin(radian)));

                return vertex;
            }

            #endregion

            #region 描画

            public virtual void DrawWay(Graphics g)
            {
                Font font = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(38f), FontStyle.Bold);
                Pen pen = null;
                StringFormat f = new StringFormat();
                f.Alignment = StringAlignment.Center;
                f.LineAlignment = StringAlignment.Center;

                foreach (var kv in next_way)
                {
                    FloorQR next = kv.Key;
                    double degree = kv.Value;

                    #region

                    //bool invalid_way = false;
                    //if (autorator_id != "" && 0 < prev_way.Count)
                    //{
                    //    if (prev_way[0].next_way[this] - 5 <= degree && degree <= prev_way[0].next_way[this] + 5
                    //        || prev_way[0].next_way[this] - 185 <= degree && degree <= prev_way[0].next_way[this] - 175
                    //        || prev_way[0].next_way[this] + 175 <= degree && degree <= prev_way[0].next_way[this] + 185)
                    //    {
                    //    }
                    //    else
                    //    {
                    //        invalid_way = true;
                    //    }
                    //}

                    //if (invalid_way)
                    //{
                    //    pen = new Pen(Color.Red, floor.pW(3));
                    //}
                    //else

                    //bool straight_line = false;

                    //foreach (var v in prev_way)
                    //{
                    //    if (v.next_way.ContainsKey(this))
                    //    {
                    //        if (v.next_way[this] == degree)
                    //        {
                    //            foreach (var kvv in next.next_way)
                    //            {
                    //                if (kvv.Value == degree)
                    //                {
                    //                    if (kvv.Key.next_way.ContainsValue(degree))
                    //                    {
                    //                        straight_line = true;
                    //                        break;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}

                    //if (straight_line)
                    //{
                    //    pen = new Pen(Color.DarkGreen, floor.pW(12));
                    //}
                    //else
                    //{
                    //    pen = new Pen(Color.DarkGreen, floor.pW(12));
                    //}

                    #endregion

                    Color color = Color.Black;

                    switch (next_way_type[next])
                    {
                        case RouteType.NONE: color = Color.DarkGreen; break;
                        case RouteType.FAT: color = Color.Blue; break;
                        case RouteType.THIN: color = Color.Red; break;
                    }

                    {
                        pen = new Pen(color, floor.pW(12));

                        g.DrawLine(pen, floor.pX(x), floor.pY(y), floor.pX(next.x), floor.pY(next.y));

                        double distance = Distance(Location, new PointF(next.x, next.y));
                        PointF p = AngleDistance(Location, degree, distance / 3 * 2);

                        //floor.DrawString(g, "＞", font, invalid_way ? Brushes.Red : Brushes.Lime, floor.pX(p.X), floor.pY(p.Y), (float)degree, f);
                        floor.DrawString(g, "＞", font, new SolidBrush(color), floor.pX(p.X), floor.pY(p.Y), (float)degree, f);

                        pen.Dispose();
                        pen = null;
                    }

                    //if (0 < next.reserve_agv.Count)
                    //{
                    //    var a = next.reserve_agv[0];
                    //    if (a != null)
                    //    {
                    //        font.Dispose();
                    //        font = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(30f), FontStyle.Bold);

                    //        color = Color.FromArgb(80, Color.Magenta);
                    //        pen = new Pen(color, floor.pW(a.radius));

                    //        g.DrawLine(pen, floor.pX(x), floor.pY(y), floor.pX(next.x), floor.pY(next.y));

                    //        //double distance = Distance(Location, new PointF(next.x, next.y));
                    //        //PointF p = AngleDistance(Location, degree, distance / 3 * 2);
                    //        ////floor.DrawString(g, "＞", font, invalid_way ? Brushes.Red : Brushes.Lime, floor.pX(p.X), floor.pY(p.Y), (float)degree, f);
                    //        //floor.DrawString(g, "＞", font, new SolidBrush(color), floor.pX(p.X), floor.pY(p.Y), (float)degree, f);

                    //        var br = new SolidBrush(Color.FromArgb(255, Color.Magenta));
                    //        PointF loc = new PointF(Location.X + (next.Location.X - Location.X) / 2, Location.Y + (next.Location.Y - Location.Y) / 2);
                    //        PointF p3 = AngleDistancePoint(loc, degree + 90, radius - radius / 4);
                    //        floor.DrawString(g, a.id, font, br, floor.pX(p3.X), floor.pY(p3.Y), 0, f);

                    //        br.Dispose();
                    //        br = null;

                    //        pen.Dispose();
                    //        pen = null;
                    //    }
                    //}
                }

                if (pen != null)
                {
                    pen.Dispose();
                    pen = null;
                }
                font.Dispose();
            }

            public virtual void DrawSelectedWay(Graphics g)
            {
                Font font = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(24f));
                Pen pen = new Pen(Color.Lime, floor.pW(3));
                StringFormat f = new StringFormat();
                f.Alignment = StringAlignment.Center;
                f.LineAlignment = StringAlignment.Center;
                Brush br = null;

                if (selected && floor.mapeditor.is_new_way && floor.controller.is_mousedrag)
                {
                    PointF point;
                    double degree;

                    string text = "＞";
                    point = floor.controller.mousePoint;
                    degree = Degree(point);

                    FloorQR hit = floor.mapeditor.HitTest(point);
                    FloorQR exist_way = null;

                    foreach (var kv in next_way)
                    {
                        if (kv.Value - 10 < degree && degree < kv.Value + 10)
                        {
                            exist_way = kv.Key;
                            break;
                        }
                    }

                    Color color = Color.DodgerBlue;
                    if (hit != null && exist_way != null && hit == exist_way)
                    {
                        color = Color.Red;
                        point = hit.Location;
                        text = "×";
                    }
                    else if (hit != null && exist_way == null)
                    {
                        point = hit.Location;
                    }
                    else if (hit == null && exist_way == null)
                    {
                        color = Color.Red;
                        text = "×";
                    }
                    else if (exist_way != null)
                    {
                        color = Color.Red;
                        point = exist_way.Location;
                        text = "×";
                    }

                    degree = Degree(point);

                    //string s = degree.ToString("#0.00");
                    //SizeF sz = g.MeasureString(s, SystemFonts.DefaultFont);
                    //g.DrawString(s, SystemFonts.DefaultFont, Brushes.Red, floor.pX(x) - sz.Width / 2, floor.pY(y - 12));

                    br = new SolidBrush(color);

                    pen.Dispose();
                    pen = new Pen(color, floor.pW(3));

                    g.DrawLine(pen, floor.pX(x), floor.pY(y), floor.pX(point.X), floor.pY(point.Y));

                    if (text != "")
                    {
                        double distance = Distance(Location, point);
                        PointF p = AngleDistance(Location, degree, distance / 3 * 2);

                        floor.DrawString(g, text, font, br, floor.pX(p.X), floor.pY(p.Y), (float)degree, f);
                    }
                }

                if (br != null) br.Dispose();

                pen.Dispose();
                font.Dispose();
            }

            public virtual void Draw(Graphics g)
            {
                if (floor.controller.applicate_mode != enApplicateMode.MAPEDITOR)
                {
                    if (this.next_way.Count == 0 && this.prev_way.Count == 0 && this.reserve_agv.Count == 0) return;
                }
                Color color = Color.Black;
                Pen pen = null;
                Brush br = null;
                Brush br_cross = null;
                Font font = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(24f));
                StringFormat f = new StringFormat();

                do
                {
                    try
                    {
                        if (floor.controller.applicate_mode == enApplicateMode.CONDITIONER)
                        {
                            if (floor.controller.floorwarp_con != null)
                            {
                                if (HitTest(floor.controller.mousePoint))
                                {
                                    if (floor.controller.floorwarp_con.FloorCode == floor.code)
                                    {
                                        color = Color.Red;
                                    }
                                    else
                                    {
                                        if (floor.controller.floorwarp_con.cur_qr.autorator_id == autorator_id)
                                        //if (floor.map.floorwarp_con.Location == Location)
                                        {
                                            color = Color.DodgerBlue;
                                        }
                                        else
                                        {
                                            color = Color.Red;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (floor.mapeditor.new_qr == this)
                            {
                                color = Color.Orange;

                                FloorQR exist_qr = mapeditor.HitTest(floor.mapeditor.new_qr.Location);
                                if (exist_qr != null)
                                {
                                    color = Color.Red;
                                }
                            }
                            else
                            {
                                if (selected)
                                {
                                    if (HitTest(floor.controller.mousePoint))
                                    {
                                        color = Color.Magenta;
                                    }
                                    else
                                    {
                                        color = Color.Orange;
                                    }
                                }
                                else if (HitTest(floor.controller.mousePoint))
                                {
                                    color = Color.DodgerBlue;
                                }
                            }
                        }

                        if (autorator_id != "")
                        {
                            //rect.X = x - autorator_radius / 2;
                            //rect.Y = y - autorator_radius / 2;
                            //rect.Width = autorator_radius;
                            //rect.Height = autorator_radius;

                            #region オートレーター描画

                            Color arc = color;
                            if (autorator_info == null)
                            {
                                pen = new Pen(arc);
                            }
                            else if (autorator_info.Status != null)
                            {
                                if (autorator_info.Status.error_code != "0000")
                                {
                                    arc = Color.Red;
                                    pen = new Pen(arc, 3);
                                }
                                else if (!autorator_info.Status.IsDisable)
                                {
                                    arc = Color.Green;
                                    pen = new Pen(arc, 3);
                                }
                                else
                                {
                                    arc = Color.DarkGray;
                                    pen = new Pen(arc, 3);
                                }
                            }
                            else
                            {
                                pen = new Pen(arc);
                            }

                            br = new SolidBrush(Color.FromArgb(200, arc));

                            double degree = 45;
                            if (0 < prev_way.Count)
                            {
                                degree = Degree(prev_way[0]) + 45;
                            }

                            PointF p1 = AngleDistancePoint(Location, degree + 0, autorator_radius / 2);
                            PointF p2 = AngleDistancePoint(Location, degree + 90, autorator_radius / 2);
                            PointF p3 = AngleDistancePoint(Location, degree + 180, autorator_radius / 2);
                            PointF p4 = AngleDistancePoint(Location, degree + -90, autorator_radius / 2);

                            g.DrawLine(pen, floor.pX(p1.X), floor.pY(p1.Y), floor.pX(p2.X), floor.pY(p2.Y));
                            g.DrawLine(pen, floor.pX(p2.X), floor.pY(p2.Y), floor.pX(p3.X), floor.pY(p3.Y));
                            g.DrawLine(pen, floor.pX(p3.X), floor.pY(p3.Y), floor.pX(p4.X), floor.pY(p4.Y));
                            g.DrawLine(pen, floor.pX(p4.X), floor.pY(p4.Y), floor.pX(p1.X), floor.pY(p1.Y));

                            g.DrawLine(pen, floor.pX(p1.X), floor.pY(p1.Y), floor.pX(p3.X), floor.pY(p3.Y));
                            g.DrawLine(pen, floor.pX(p2.X), floor.pY(p2.Y), floor.pX(p4.X), floor.pY(p4.Y));

                            p1 = AngleDistancePoint(Location, 45, autorator_radius / 2);
                            p4 = AngleDistancePoint(Location, -45, autorator_radius / 2);
                            SizeF textsize = g.MeasureString(autorator_id, font);
                            g.DrawString(autorator_id, font, br, floor.pX(p1.X + (p4.X - p1.X) / 2) - textsize.Width / 2, floor.pY(p1.Y - 4));

                            if (autorator_info != null && autorator_info.Status != null)
                            {
                                p2 = AngleDistancePoint(Location, 135, autorator_radius / 2);

                                g.DrawString(autorator_info.Status.ToString(), font, br, floor.pX(p2.X), floor.pY(p2.Y - 4));

                                if (0 < reserve_agv.Count)
                                {
                                    var agv = reserve_agv[0];

                                    if (autorator_info.CanExit(agv))
                                    {
                                        g.DrawString("搬出許可" + agv.ToString(), font, br, floor.pX(p1.X), floor.pY(p1.Y + 32));
                                    }
                                    else if (autorator_info.IsEntried(agv))
                                    {
                                        g.DrawString("搬入" + agv.ToString(), font, br, floor.pX(p1.X), floor.pY(p1.Y + 32));
                                    }
                                    else if (autorator_info.CanEntry(agv))
                                    {
                                        g.DrawString("搬入許可" + agv.ToString(), font, br, floor.pX(p1.X), floor.pY(p1.Y + 32));
                                    }
                                    else if (autorator_info.IsRequested(agv))
                                    {
                                        g.DrawString("要求" + agv.ToString(), font, br, floor.pX(p1.X), floor.pY(p1.Y + 32));
                                    }
                                    else
                                    {

                                    }
                                }

                                if (autorator_info.is_assister)
                                {
                                    p1 = AngleDistancePoint(Location, degree + 0, autorator_radius / 3);
                                    p2 = AngleDistancePoint(Location, degree + 90, autorator_radius / 3);
                                    p3 = AngleDistancePoint(Location, degree + 180, autorator_radius / 3);
                                    p4 = AngleDistancePoint(Location, degree + -90, autorator_radius / 3);

                                    g.DrawLine(pen, floor.pX(p1.X), floor.pY(p1.Y), floor.pX(p2.X), floor.pY(p2.Y));
                                    g.DrawLine(pen, floor.pX(p2.X), floor.pY(p2.Y), floor.pX(p3.X), floor.pY(p3.Y));
                                    g.DrawLine(pen, floor.pX(p3.X), floor.pY(p3.Y), floor.pX(p4.X), floor.pY(p4.Y));
                                    g.DrawLine(pen, floor.pX(p4.X), floor.pY(p4.Y), floor.pX(p1.X), floor.pY(p1.Y));
                                }
                            }



                            br.Dispose();
                            pen.Dispose();

                            #endregion
                        }

                        //else
                        {
                            rect.X = x - radius / 2;
                            rect.Y = y - radius / 2;
                            rect.Width = radius;
                            rect.Height = radius;
                        }

                        #region 棚配置可能場所描画

                        if (rack_setable)
                        {
                            Rack r = new Rack();
                            float xs = (Math.Max(r.sizeW, r.sizeL) / 2) / (float)Math.Sqrt(2);
                            br = new SolidBrush(Color.FromArgb(80, Color.Yellow));
                            g.FillRectangle(br, floor.pX(x - xs), floor.pY(y + xs), floor.pW(xs * 2), floor.pH(xs * 2));
                            br.Dispose();
                        }

                        #endregion

                        #region 速度制限描画

                        if (speed_limit != enSpeed.H)
                        {
                            br = new SolidBrush(Color.FromArgb(80, Color.DarkGray));
                            g.FillEllipse(br, floor.pX(x - radius), floor.pY(y + radius), floor.pW(radius * 2), floor.pH(radius * 2));
                        }

                        #endregion

                        br = new SolidBrush(color);
                        pen = new Pen(Color.FromArgb(80, color), floor.pW(3));

                        #region QRポイント描画

                        //if (0.6f <= floor.map.draw_scale)
                        //{
                        //    qr.Draw(g, new PointF(floor.pX(x), floor.pY(y)), 1f, 1, 0, true, -1, this.ToString(), 0, floor.map.draw_scale, br);
                        //}
                        //else
                        {
                            //if (1 < this.prev_way.Count && floor.controller.applicate_mode != enApplicateMode.MAPEDITOR)
                            //{
                            //    br_cross = new SolidBrush(Color.FromArgb(255, Color.Red));
                            //    g.FillRectangle(br_cross, floor.pX(x) - floor.pW(8), floor.pY(y) - floor.pH(8), floor.pW(16), floor.pW(16));
                            //}
                            //else
                            {
                                if (floor.controller.applicate_mode == enApplicateMode.MAPEDITOR)
                                {
                                    g.DrawEllipse(pen, floor.pX(x - radius), floor.pY(y + radius), floor.pW(radius * 2), floor.pH(radius * 2));
                                    g.FillRectangle(br, floor.pX(x) - floor.pW(5), floor.pY(y) - floor.pH(5), floor.pW(10), floor.pW(10));
                                }
                                else
                                {
                                    if (selected)
                                    {
                                        Pen pen2 = new Pen(Color.FromArgb(150, Color.DodgerBlue), floor.pW(8));
                                        g.DrawEllipse(pen2, floor.pX(x - radius), floor.pY(y + radius), floor.pW(radius * 2), floor.pH(radius * 2));
                                        pen2.Dispose();
                                    }

                                    ////g.FillRectangle(br, floor.pX(x) - 5, floor.pY(y) - 5, 10, 10);
                                    //g.FillRectangle(br, floor.pX(x) - floor.pW(5), floor.pY(y) - floor.pH(5), floor.pW(10), floor.pW(10));
                                }

                                //if (escape_to != null)
                                //{
                                //    Pen pen2 = new Pen(Color.FromArgb(80, Color.Red), floor.pW(radius / 3));
                                //    g.DrawEllipse(pen2, floor.pX(x - radius / 2), floor.pY(y + radius / 2), floor.pW(radius), floor.pH(radius));
                                //    pen2.Dispose();
                                //}

                                //if (escape_from != null)
                                //{
                                //    Pen pen2 = new Pen(Color.FromArgb(80, Color.Lime), floor.pW(radius / 3));
                                //    g.DrawEllipse(pen2, floor.pX(x - radius / 3 * 2), floor.pY(y + radius / 3 * 2), floor.pW(radius / 3 * 4), floor.pH(radius / 3 * 4));
                                //    pen2.Dispose();
                                //}

                                {
                                    var ra = reserve_agv.Where(e => e != null).ToList();
                                    if (0 < ra.Count)
                                    {
                                        Pen pen2 = new Pen(Color.FromArgb(120, Color.Magenta), floor.pW(radius / 4));
                                        g.DrawEllipse(pen2, floor.pX(x - radius / 4), floor.pY(y + radius / 4), floor.pW(radius / 2), floor.pH(radius / 2));
                                        pen2.Dispose();

                                        string r = "";
                                        for (int i = 0; i < ra.Count; i++)
                                        {
                                            if (r != "") r += ",";
                                            r = r + ra[i].id;
                                        }

                                        var font2 = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(30f), FontStyle.Bold);
                                        var br2 = new SolidBrush(Color.FromArgb(255, Color.Magenta));

                                        floor.DrawString(g, r, font2, br2, floor.pX(x + 10), floor.pY(y - 10), 270, f);

                                        font2.Dispose();
                                        br2.Dispose();
                                    }

                                    //List<CheckConditions> cc = new List<CheckConditions>();
                                    //foreach (var v in mapeditor.controller.agvs)
                                    //{
                                    //    if (!v.selected && !floor.controller.allselect) continue;

                                    //    foreach (var vv in v.check_conditions) cc.Add(vv);
                                    //}

                                    ////if (reserved_agv != null)
                                    ////{
                                    ////    if (!reserved_agv.selected && !floor.controller.allselect)
                                    ////    {
                                    ////    }
                                    ////    else
                                    ////    {
                                    ////        var con = reserved_agv.check_conditions.Where(e => e.check_condition.cur_qr == this).FirstOrDefault();

                                    ////        if (con != null)
                                    ////        {
                                    ////            cc.Remove(con);

                                    ////            if (con.check_condition.cur_qr == this)
                                    ////            {
                                    ////                switch (con.check_point_type)
                                    ////                {
                                    ////                    case CheckConditions.enCheckPointTYpe.EXCLUSION:
                                    ////                        {
                                    ////                            Pen pen2 = new Pen(Color.FromArgb(80, Color.Red), floor.pW(radius / 3));
                                    ////                            g.DrawEllipse(pen2, floor.pX(x - radius / 2), floor.pY(y + radius / 2), floor.pW(radius), floor.pH(radius));
                                    ////                            pen2.Dispose();
                                    ////                        }
                                    ////                        break;

                                    ////                    case CheckConditions.enCheckPointTYpe.FIFO:
                                    ////                        {
                                    ////                            Pen pen2 = new Pen(Color.FromArgb(80, Color.Lime), floor.pW(radius / 3));
                                    ////                            g.DrawEllipse(pen2, floor.pX(x - radius / 3 * 2), floor.pY(y + radius / 3 * 2), floor.pW(radius / 3 * 4), floor.pH(radius / 3 * 4));
                                    ////                            pen2.Dispose();
                                    ////                        }
                                    ////                        break;
                                    ////                }
                                    ////            }
                                    ////        }
                                    ////    }
                                    ////}

                                    //foreach (var v in cc)
                                    //{
                                    //    if (v.check_condition.cur_qr == this)
                                    //    {
                                    //        switch (v.check_point_type)
                                    //        {
                                    //            case CheckConditions.enCheckPointTYpe.EXCLUSION:
                                    //                {
                                    //                    Pen pen2 = new Pen(Color.FromArgb(80, Color.Red), floor.pW(radius / 3));
                                    //                    g.DrawEllipse(pen2, floor.pX(x - radius / 2), floor.pY(y + radius / 2), floor.pW(radius), floor.pH(radius));
                                    //                    pen2.Dispose();
                                    //                }
                                    //                break;

                                    //            case CheckConditions.enCheckPointTYpe.FIFO:
                                    //                {
                                    //                    Pen pen2 = new Pen(Color.FromArgb(80, Color.Lime), floor.pW(radius / 3));
                                    //                    g.DrawEllipse(pen2, floor.pX(x - radius / 3 * 2), floor.pY(y + radius / 3 * 2), floor.pW(radius / 3 * 4), floor.pH(radius / 3 * 4));
                                    //                    pen2.Dispose();
                                    //                }
                                    //                break;
                                    //        }
                                    //    }
                                    //}

                                    {
                                        CheckConditions cc = GetCP;
                                        if (cc != null)
                                        {
                                            switch (cc.check_point_type)
                                            {
                                                case CheckConditions.enCheckPointType.EXCLUSION:
                                                    {
                                                        Pen pen2 = new Pen(Color.FromArgb(80, Color.Red), floor.pW(radius / 3));
                                                        g.DrawEllipse(pen2, floor.pX(x - radius / 2), floor.pY(y + radius / 2), floor.pW(radius), floor.pH(radius));
                                                        pen2.Dispose();
                                                    }
                                                    break;

                                                case CheckConditions.enCheckPointType.FIFO:
                                                    {
                                                        Pen pen2 = new Pen(Color.FromArgb(80, Color.Lime), floor.pW(radius / 3));
                                                        g.DrawEllipse(pen2, floor.pX(x - radius / 3 * 2), floor.pY(y + radius / 3 * 2), floor.pW(radius / 3 * 4), floor.pH(radius / 3 * 4));
                                                        pen2.Dispose();
                                                    }
                                                    break;
                                            }
                                        }
                                    }

                                    {
                                        CheckConditions cc = GetWP;
                                        if (cc != null)
                                        {
                                            Pen pen2 = new Pen(Color.FromArgb(80, Color.Cyan), floor.pW(radius / 3));
                                            g.DrawEllipse(pen2, floor.pX(x - radius / 2), floor.pY(y + radius / 2), floor.pW(radius), floor.pH(radius));
                                            pen2.Dispose();
                                        }
                                    }
                                }
                            }
                        }

                        #endregion

                        #region 棚旋回可能描画

                        if (rack_rotatable)
                        {
                            Rack r = new Rack();
                            int size = Math.Max(r.sizeW, r.sizeL);
                            g.DrawEllipse(pen, floor.pX(x - size / 2), floor.pY(y + size / 2), floor.pW(size), floor.pH(size));

                            {
                                double degree = 45;
                                if (0 < next_way.Count)
                                {
                                    degree = Degree(next_way.First().Key) + 45;
                                }

                                PointF p1 = AngleDistancePoint(Location, degree + 0, size / 2);
                                PointF p2 = AngleDistancePoint(Location, degree + 90, size / 2);
                                PointF p3 = AngleDistancePoint(Location, degree + 180, size / 2);
                                PointF p4 = AngleDistancePoint(Location, degree + -90, size / 2);
                                g.DrawLine(pen, floor.pX(p1.X), floor.pY(p1.Y), floor.pX(p2.X), floor.pY(p2.Y));
                                g.DrawLine(pen, floor.pX(p2.X), floor.pY(p2.Y), floor.pX(p3.X), floor.pY(p3.Y));
                                g.DrawLine(pen, floor.pX(p3.X), floor.pY(p3.Y), floor.pX(p4.X), floor.pY(p4.Y));
                                g.DrawLine(pen, floor.pX(p4.X), floor.pY(p4.Y), floor.pX(p1.X), floor.pY(p1.Y));
                            }

                            {
                                double degree = 0;
                                if (0 < prev_way.Count)
                                {
                                    degree = Degree(prev_way[0]);
                                }

                                PointF p1 = AngleDistancePoint(Location, degree + 0, size / 2);
                                PointF p2 = AngleDistancePoint(Location, degree + 90, size / 2);
                                PointF p3 = AngleDistancePoint(Location, degree + 180, size / 2);
                                PointF p4 = AngleDistancePoint(Location, degree + -90, size / 2);
                                g.DrawLine(pen, floor.pX(p1.X), floor.pY(p1.Y), floor.pX(p2.X), floor.pY(p2.Y));
                                g.DrawLine(pen, floor.pX(p2.X), floor.pY(p2.Y), floor.pX(p3.X), floor.pY(p3.Y));
                                g.DrawLine(pen, floor.pX(p3.X), floor.pY(p3.Y), floor.pX(p4.X), floor.pY(p4.Y));
                                g.DrawLine(pen, floor.pX(p4.X), floor.pY(p4.Y), floor.pX(p1.X), floor.pY(p1.Y));
                            }

                            //rect.X = x - r.sizeW / 2;
                            //rect.Y = y - r.sizeL / 2;
                            //rect.Width = r.sizeW;
                            //rect.Height = r.sizeL;
                        }

                        #endregion

                        #region 充電ポイント描画

                        if (direction_charge != enDirection.NONE)
                        {
                            f.Alignment = StringAlignment.Center;
                            f.LineAlignment = StringAlignment.Center;

                            PointF p = AngleDistancePoint(Location, (double)direction_charge, radius - 20);
                            floor.DrawString(g, "充電", font, br, floor.pX(p.X), floor.pY(p.Y), (float)direction_charge - 90, f);
                            g.DrawEllipse(pen, floor.pX(x - radius), floor.pY(y + radius), floor.pW(radius * 2), floor.pH(radius * 2));
                        }

                        #endregion

                        #region ステーション描画

                        if (direction_station != enDirection.NONE)
                        {
                            f.Alignment = StringAlignment.Center;
                            f.LineAlignment = StringAlignment.Near;

                            Rack r = new Rack();
                            PointF p = AngleDistancePoint(Location, (double)direction_station, r.sizeW / 2 + 20);
                            floor.DrawString(g, "ST[" + station_id + "]", font, br, floor.pX(p.X), floor.pY(p.Y), (float)direction_station - 90, f);

                            PointF p1 = AngleDistancePoint(Location, (float)direction_station + 45f, r.sizeW / 2 + 10);
                            PointF p2 = AngleDistancePoint(Location, (float)direction_station - 45f, r.sizeW / 2 + 10);
                            PointF p3 = AngleDistancePoint(p1, (float)direction_station, 50);
                            PointF p4 = AngleDistancePoint(p2, (float)direction_station, 50);
                            g.DrawLine(pen, floor.pX(p1.X), floor.pY(p1.Y), floor.pX(p2.X), floor.pY(p2.Y));
                            g.DrawLine(pen, floor.pX(p1.X), floor.pY(p1.Y), floor.pX(p3.X), floor.pY(p3.Y));
                            g.DrawLine(pen, floor.pX(p2.X), floor.pY(p2.Y), floor.pX(p4.X), floor.pY(p4.Y));
                            g.DrawEllipse(pen, floor.pX(x - radius), floor.pY(y + radius), floor.pW(radius * 2), floor.pH(radius * 2));
                        }

                        #endregion

                        #region 他フロア接続ポイント描画

                        font.Dispose();
                        font = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(20f));
                        if (0 < floorwarp_qr.Count)
                        {
                            //PointF p = AngleDistancePoint(Location, 45, autorator_radius / 2);

                            //PointF p = AngleDistancePoint(Location, 0, radius/3*4);
                            string s = "";
                            foreach (var v in floorwarp_qr) s += "⇔" + v.QrString + "\n";
                            floor.DrawString(g, s, font, Brushes.Blue, floor.pX(Location.X - autorator_radius / 3), floor.pY(Location.Y - 16), -90, f);
                        }

                        #endregion

                    }
                    catch
                    {
                        continue;
                    }
                }
                while (false);

                font.Dispose();
                if (br != null) br.Dispose();
                if (br_cross != null) br_cross.Dispose();
                if (pen != null) pen.Dispose();
            }

            public virtual void DrawAgvShadow(Graphics g)
            {
                Color color = Color.Black;
                Brush br = null;
                Font font = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(30), FontStyle.Bold);
                Pen pen = new Pen(Color.FromArgb(80, color));

                try
                {
                    StringFormat f = new StringFormat();
                    f.Alignment = StringAlignment.Center;
                    f.LineAlignment = StringAlignment.Center;

                    if (floor.controller.applicate_mode != enApplicateMode.MAPEDITOR &&
                        floor.controller.applicate_mode != enApplicateMode.CONDITIONER &&
                        floor.controller.applicate_mode != enApplicateMode.UNKNOWN
                        )
                    {
                        //lock (this)
                        {
                            if (on_agv != null)
                            {
                                //if (on_agv.selected)
                                {
                                    br = new SolidBrush(Color.FromArgb(40, Color.Blue));
                                    g.FillEllipse(br, floor.pX(x - radius), floor.pY(y + radius), floor.pW(radius * 2), floor.pH(radius * 2));
                                    g.DrawEllipse(pen, floor.pX(x - radius), floor.pY(y + radius), floor.pW(radius * 2), floor.pH(radius * 2));
                                }
                            }

                            //else
                            {
                                //lock (lock_agv)
                                {
                                    if (0 < lock_agv.Count)
                                    {
                                        var a = lock_agv[0];
                                        //if (lock_agv.selected)
                                        {
                                            br = new SolidBrush(Color.FromArgb(80, Color.Red));
                                            g.FillEllipse(br, floor.pX(x - radius/2 - 5), floor.pY(y + radius/2 + 5), floor.pW(radius + 10), floor.pH(radius + 10));
                                            g.DrawEllipse(pen, floor.pX(x - radius/2 - 5), floor.pY(y + radius/2 + 5), floor.pW(radius + 10), floor.pH(radius + 10));
                                            br.Dispose();

                                            //br = new SolidBrush(Color.FromArgb(180, Color.Red));
                                            //PointF p3 = AngleDistancePoint(Location, a.degree - 90, radius - radius / 4);
                                            //floor.DrawString(g, a.id, font, br, floor.pX(p3.X), floor.pY(p3.Y), 0, f);
                                        }
                                    }
                                    //else
                                    //{
                                    //    g.DrawEllipse(pen, floor.pX(x - radius), floor.pY(y + radius), floor.pW(radius * 2), floor.pH(radius * 2));
                                    //}
                                }
                            }
                        }
                    }
                    //else
                    //{
                    //    g.DrawEllipse(pen, floor.pX(x - radius), floor.pY(y + radius), floor.pW(radius * 2), floor.pH(radius * 2));
                    //}
                }
                catch { }

                font.Dispose();
                if (br != null) br.Dispose();
                if (pen != null) pen.Dispose();
            }

            #endregion

            #region 選択操作

            public void Select(bool selected)
            {
                if (floor.mapeditor.selected_qr != null)
                {
                    floor.mapeditor.selected_qr.selected = false;
                }

                this.selected = selected;
                if (selected)
                {
                    floor.mapeditor.selected_qr = this;
                }
                else
                {
                    floor.mapeditor.selected_qr = null;
                }

                if (floor.controller.floorwarp_qr != null)
                {
                    if (!floor.controller.floorwarp_qr.floorwarp_qr.Contains(this))
                    {
                        floor.controller.floorwarp_qr.floorwarp_qr.Add(this);
                    }

                    if (!floorwarp_qr.Contains(floor.controller.floorwarp_qr))
                    {
                        floorwarp_qr.Add(floor.controller.floorwarp_qr);
                    }
                }

                if (floor.controller.EventQrSelected != null) floor.controller.EventQrSelected(this);
            }

            #endregion

            #region 対象検査

            public bool HitTest(PointF mousePoint)
            {
                if (mapeditor.controller.is_editing_background) return false;

                if (rect.Contains(mousePoint))
                {
                    return true;
                }

                return false;
            }

            public class TRectangle
            {
                public float cx;
                public float cy;
                public int Width;
                public int Height;
                public float r;

                public float Left { get { return cx - Width / 2; } }
                public float Top { get { return cy - Height / 2; } }
                public float Right { get { return cx + Width / 2; } }
                public float Bottom { get { return cy + Height / 2; } }

                public TRectangle(FloorQR qr, int w, int l, float degree)
                {
                    cx = qr.Location.X;
                    cy = qr.Location.Y;
                    Width = w * 2;
                    Height = l * 2;
                    r = (float)((degree + 360) % 360 * (Math.PI / 180));
                }
            }

            /// <summary>
            /// 点が回転している矩形内に存在するかテスト
            /// </summary>
            /// <param name="rect">矩形情報</param>
            /// <param name="pt">検査する点</param>
            /// <returns>true:矩形内／false:矩形外</returns>
            public bool HitRectAngle(TRectangle rect, PointF pt)
            {
                double l = Math.Sqrt(Math.Pow(pt.X - rect.cx, 2) + Math.Pow(pt.Y - rect.cy, 2));

                PointF pt2 = new PointF(pt.X - rect.cx, pt.Y - rect.cy);

                double r1;
                if (pt2.X != 0) r1 = Math.Atan(pt2.Y / pt2.X);
                else r1 = Math.PI / 2;
                double r2 = r1 - rect.r;

                PointF pt3 = new PointF((float)(l * Math.Cos(r2)), (float)(l * Math.Sin(r2)));
                pt3.X += rect.cx;
                pt3.Y += rect.cy;

                if (rect.Left <= pt3.X && pt3.X <= rect.Right &&
                    rect.Top <= pt3.Y && pt3.Y <= rect.Bottom)
                {
                    return true;
                }

                return false;
            }

            #endregion

            #region 経路正規化

            public void Normalize()
            {
                bool doprev = false;
                foreach (var v in prev_way)
                {
                    if (v.autorator_id != "")
                    {
                        doprev = true;
                        break;
                    }
                }

                List<FloorQR> source = new List<FloorQR>();
                if (doprev) Normalize_prevway(source);
                else Normalize_nextway(source);
                source.Clear();
            }

            public void Normalize_nextway(List<FloorQR> source)
            {
                if (source == null) source = new List<FloorQR>();
                if (source.Contains(this)) return;
                source.Add(this);

                //if (is_autorator && 0 < next_way.Count && 0 < prev_way.Count)
                //{
                //    FloorQR next_qr = next_way.First().Key;
                //    if (!source.Contains(next_qr))
                //    {
                //        RemoveNextWay(next_qr);
                //        floor.mapeditor.RemoveQR(next_qr);

                //        double distance = Distance(next_qr);
                //        double degree = prev_way[0].next_way[this];

                //        PointF p = AngleDistancePoint(Location, degree, distance);
                //        next_qr.x = p.X;
                //        next_qr.y = p.Y;

                //        floor.mapeditor.AddQR(next_qr);
                //        AddNextWay(next_qr);
                //    }
                //}

                Dictionary<FloorQR, double> temp = new Dictionary<FloorQR, double>();
                //foreach (var kv in next_way)
                //{
                //    if (kv.Key.is_autorator)
                //    {
                //        double degree = Degree(kv.Key);
                //        temp[kv.Key] = degree;

                //        if (!source.Contains(kv.Key) && kv.Key != this)
                //        {
                //            kv.Key.Normalize_nextway(source);
                //        }
                //    }
                //}
                foreach (var kv in next_way)
                {
                    //if (!kv.Key.is_autorator)
                    {
                        double degree = Degree(kv.Key);
                        temp[kv.Key] = degree;

                        if (!source.Contains(kv.Key) && kv.Key != this)
                        {
                            kv.Key.Normalize_nextway(source);
                        }
                    }
                }

                //next_way.Clear();
                //foreach (var kv in temp)
                //{
                //    next_way[kv.Key] = kv.Value;
                //}

                //foreach (var v in prev_way)
                //{
                //    if (v.is_autorator)
                //    {
                //        if (!source.Contains(v) && v != this)
                //        {
                //            v.Normalize_prevway(source);
                //        }
                //    }
                //}

                foreach (var v in prev_way)
                {
                    //if (!v.is_autorator)
                    {
                        if (!source.Contains(v) && v != this)
                        {
                            v.Normalize_prevway(source);
                        }
                    }
                }
            }

            public void Normalize_prevway(List<FloorQR> source)
            {
                if (source == null) source = new List<FloorQR>();
                if (source.Contains(this)) return;
                source.Add(this);

                //if (is_autorator && 0 < next_way.Count && 0 < prev_way.Count)
                //{
                //    FloorQR prev_qr = prev_way[0];
                //    if (!source.Contains(prev_qr))
                //    {
                //        prev_qr.RemoveNextWay(this);
                //        floor.mapeditor.RemoveQR(prev_qr);

                //        double distance = prev_qr.Distance(this);
                //        double degree = next_way.First().Key.Degree(this);

                //        PointF p = AngleDistancePoint(Location, degree, distance);
                //        prev_qr.x = p.X;
                //        prev_qr.y = p.Y;

                //        floor.mapeditor.AddQR(prev_qr);
                //        prev_qr.AddNextWay(this);
                //    }
                //}

                //foreach (var v in prev_way)
                //{
                //    if (v.is_autorator)
                //    {
                //        if (!source.Contains(v) && v != this)
                //        {
                //            v.Normalize_prevway(source);
                //        }
                //    }
                //}
                foreach (var v in prev_way)
                {
                    //if (!v.is_autorator)
                    {
                        if (!source.Contains(v) && v != this)
                        {
                            v.Normalize_prevway(source);
                        }
                    }
                }

                Dictionary<FloorQR, double> temp = new Dictionary<FloorQR, double>();
                //foreach (var kv in next_way)
                //{
                //    if (kv.Key.is_autorator)
                //    {
                //        double degree = Degree(kv.Key);
                //        temp[kv.Key] = degree;

                //        if (!source.Contains(kv.Key) && kv.Key != this)
                //        {
                //            kv.Key.Normalize_nextway(source);
                //        }
                //    }
                //}
                foreach (var kv in next_way)
                {
                    //if (!kv.Key.is_autorator)
                    {
                        double degree = Degree(kv.Key);
                        temp[kv.Key] = degree;

                        if (!source.Contains(kv.Key) && kv.Key != this)
                        {
                            kv.Key.Normalize_nextway(source);
                        }
                    }
                }
                next_way.Clear();
                foreach (var kv in temp)
                {
                    next_way[kv.Key] = kv.Value;
                }
            }

            #endregion

            #region 経路追加・削除操作

            public void AddNextWay(FloorQR qr, RouteType route_type)
            {
                AddNextWay(qr, Degree(qr), route_type);
            }

            public void AddNextWay(FloorQR qr, double degree, RouteType route_type)
            {
                next_way[qr] = degree;

                if (qr.next_way_type.ContainsKey(this))
                {
                    route_type = qr.next_way_type[this];
                }

                next_way_type[qr] = route_type;

                if (!qr.prev_way.Contains(this))
                {
                    qr.prev_way.Add(this);
                }
            }

            public void RemoveNextWay(FloorQR qr)
            {
                mapeditor.controller.is_erasing_ways = true;

                if (next_way.ContainsKey(qr))
                {
                    if (qr.prev_way.Contains(this))
                    {
                        qr.prev_way.Remove(this);
                    }

                    if (next_way_type.ContainsKey(qr))
                    {
                        next_way_type.Remove(qr);
                    }

                    next_way.Remove(qr);
                }
            }

            public void RemoveDuplicatedNextWay()
            {
                List<FloorQR> remove_way = new List<FloorQR>();
                Dictionary<int, FloorQR> checker = new Dictionary<int, FloorQR>();

                foreach (var kv in next_way)
                {
                    if (!checker.ContainsKey((int)kv.Value))
                    {
                        checker[(int)kv.Value] = kv.Key;
                    }
                    else
                    {
                        if (Distance(checker[(int)kv.Value]) < Distance(kv.Key))
                        {
                            remove_way.Add(kv.Key);
                        }
                        else
                        {
                            remove_way.Add(checker[(int)kv.Value]);
                            checker[(int)kv.Value] = kv.Key;
                        }
                    }
                }

                foreach (var v in remove_way)
                {
                    RemoveNextWay(v);
                }
            }

            #endregion

            #region 各種トリガー操作

            public void Trigger(string id, string INorOUT)
            {
                if (autorator_id != "")
                {
                    if (INorOUT == "IN")
                    {
                        if (autoratorIN_trigger != id)
                        {
                            autoratorIN_trigger = id;
                            if (floor.controller.EventTrigger != null) floor.controller.EventTrigger(this, id, "IN");
                        }
                    }
                    else if (INorOUT == "OUT")
                    {
                        if (autoratorOUT_trigger != id)
                        {
                            autoratorOUT_trigger = id;
                            if (floor.controller.EventTrigger != null) floor.controller.EventTrigger(this, id, "OUT");
                        }
                    }
                }
                else if (direction_station != FloorQR.enDirection.NONE)
                {
                    station_complete_trigger = (id != "");
                    if (floor.controller.EventTrigger != null) floor.controller.EventTrigger(this, id, "");
                }
                else if (direction_charge != FloorQR.enDirection.NONE)
                {
                    charge_complete_trigger = (id != "");
                    if (floor.controller.EventTrigger != null) floor.controller.EventTrigger(this, id, "");
                }
            }

            #endregion
        }

        #endregion
    }
}
