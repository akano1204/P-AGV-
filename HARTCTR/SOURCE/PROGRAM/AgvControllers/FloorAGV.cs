using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using BelicsClass.Common;
using AgvController;
using System.Net;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        #region AGV管理クラス

        public bool allselect = false;

        public class FloorAGV
        {
            #region フィールド

            //public int no = 0;
            public string id = "";
            private Rack _rack = null;

            public Rack rack
            {
                get
                {
                    return _rack;
                }

                set
                {
                    if (value == null && _rack != null)
                    {
                        _rack.loading_agv = null;
                    }

                    _rack = value;

                    if (_rack != null)
                    {
                        _rack.loading_agv = this;
                    }
                }
            }

            public FloorQR obstructing_rackqr = null;
            public double straight_cost = 0;

            public FloorMap floor = null;
            public int radius = 60;

            public PointF point = new PointF();

            public AgvRunner agvRunner = null;

            public FloorQR in_autorator = null;
            public FloorQR out_autorator = null;


            public float x
            {
                get
                {
                    if (agvRunner == null) return point.X;
                    return agvRunner.Location.X;
                }
                set
                {
                    point.X = value;
                    SetLocation("", point);
                }
            }
            public float y
            {
                get
                {
                    if (agvRunner == null) return point.Y;
                    return agvRunner.Location.Y;
                }
                set
                {
                    point.Y = value;
                    SetLocation("", point);
                }
            }


            public RectangleF rect = new RectangleF();
            public bool selected = false;
            public int degree = 0;

            public FloorQR _on_qr = null;
            public FloorQR on_qr
            {
                set { _on_qr = value; }
                get { return _on_qr; }
            }

            public Dictionary<int, RouteConditionList> mode_conditions = new Dictionary<int, RouteConditionList>();

            public int moveMode = 0;

            public RouteConditionList conditions_temp = new RouteConditionList();
            
            public bool crash_wait = false;
            public bool request_moving = false;

            public RouteConditionList conditions
            {
                get
                {
                    return mode_conditions[moveMode];
                    //return new RouteConditionList(mode_conditions[moveMode]);
                }
                set
                {
                    mode_conditions[moveMode].Clear();
                    mode_conditions[moveMode].AddRange(value);
                }
            }


            public RouteConditionList current_route = new RouteConditionList();

            //先行AGV(後行AGV回避してくれるのを待機する側)
            public FloorAGV escape_from = null;
            //後行AGV
            public FloorAGV escape_to = null;
            public FloorAGV escape_complete = null;

            public Rack placed_rack
            {
                get
                {
                    if (rack != null) return rack;
                    return on_qr.rack;
                }
            }

            /// <summary>AGV移動要求</summary>
            public bool prev_agvreq = false;
            public AgvOrderCommunicator.RequestMove req = null;
            public bool working = false;

            public BL_Stopwatch swStanbySeconds = new BL_Stopwatch();


            public SynchronizedList<FloorQR> locked_qr_list = new SynchronizedList<FloorQR>();
            public SynchronizedList<FloorQR> reserved_qr_list = new SynchronizedList<FloorQR>();

            public bool autorator_reset = false;

            public virtual List<FloorQR> routed_qr
            {
                get
                {
                    List<FloorQR> ret = new List<FloorQR>();

                    //ret.Add(on_qr);
                    //ret.AddRange(locked_qr_list);

                    for (int i = 0; i < conditions.Count; i++)
                    {
                        var v = conditions[i];

                        if (!ret.Contains(v.cur_qr)) ret.Add(v.cur_qr);
                        foreach (var vv in v.cur_qr.conflict_qr(this))
                        {
                            if (!ret.Contains(vv)) ret.Add(vv);
                        }
                    }

                    return ret;
                }
            }

            public SynchronizedList<FloorQR> on_qr_list = new SynchronizedList<FloorQR>();

            public bool communicating = false;

            public SynchronizedList<CheckConditions> check_conditions = new SynchronizedList<CheckConditions>();


            public CheckConditions GetCP(FloorQR qr)
            {
                if (0 < conditions.Count && 0 < check_conditions.Count)
                {
                    return check_conditions.Where(e => e.check_condition != null && e.check_condition.cur_qr == qr).FirstOrDefault();
                }

                return null;
            }

            public CheckConditions GetWP(FloorQR qr)
            {
                if (0 < conditions.Count && 0 < check_conditions.Count)
                {
                    return check_conditions.Where(e => e.wait_condition != null && e.wait_condition.cur_qr == qr).FirstOrDefault();
                }

                return null;
            }

            public RouteCondition reserved_autorator_con
            {
                get
                {

                    for (int i = 0; i < current_route.Count; i++)
                    {
                        var v = current_route[i];

                        if (v.cur_qr.autorator_info != null && reserved_qr_list.Contains(v.cur_qr))
                        {
                            return v;
                        }
                    }

                    return null;
                }
            }

            #endregion

            #region コンストラクタ・プロパティ

            public FloorAGV(FloorMap floor, float x, float y)
            {
                this.floor = floor;
                this.x = x;
                this.y = y;

                id = "--";

                foreach (var kv in moveModes)
                {
                    mode_conditions[kv.Key] = new RouteConditionList();
                }
            }

            public override string ToString()
            {
                return "AGV-" + id;
            }

            public PointF Location
            {
                get
                {
                    if (agvRunner == null) return point;
                    return agvRunner.Location;
                }
            }

            public void SetLocation(string floorcode, PointF location)
            {
                SetLocation(floorcode, location, 0);
            }

            public void SetLocation(string floorcode, PointF location, int rack_deg)
            {
                if (floorcode == "") floorcode = floor.code;
                if (!floor.controller.map.ContainsKey(floorcode)) return;

                PointF pre = point;
                point = location;
                floor = floor.controller.map[floorcode];

                var qr = floor.mapeditor.Exist(point);
                if (qr != null)
                {
                    if (floor.controller.floorwarp_con != null)
                    {
                        floor.controller.floorwarp_con.cur_qr.UnplaceAgv(this, rack_deg);

                        if (!floor.conditioner.is_new_agv)
                        {
                            qr.PlaceAgv(this);
                            on_qr = qr;
                        }
                    }
                    else if (on_qr != qr)
                    {
                        if (!floor.conditioner.is_new_agv)
                        {
                            if (on_qr != null && qr != null && on_qr.floor != qr.floor)
                            {
                                if (selected) floor.controller.SelectFloor(qr.floor.code);
                            }

                            FloorQR pre_qr = on_qr;

                            if (pre_qr != null)
                            {
                                on_qr = qr;
                                pre_qr.UnplaceAgv(this, rack_deg);
                                //pre_qr.Unlock(this);

                                on_qr = pre_qr;
                            }

                            qr.PlaceAgv(this);
                            on_qr = qr;
                        }
                        
                        if (floor.controller.EventPaint != null) floor.controller.EventPaint();
                    }
                    else
                    {
                        if (!floor.conditioner.is_new_agv)
                        {
                            qr.PlaceAgv(this);
                            on_qr = qr;
                        }
                    }
                }
            }

            #endregion

            #region 角度・座標取得

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

            public double Distance(PointF p1, PointF p2)
            {
                return Math.Sqrt((p2.X - p1.X) * (p2.X - p1.X) + (p2.Y - p1.Y) * (p2.Y - p1.Y));
            }

            public double Distance(PointF p1)
            {
                return Math.Sqrt((x - p1.X) * (x - p1.X) + (y - p1.Y) * (y - p1.Y));
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

            public void DrawCondition(Graphics g)
            {
                if (!selected && !floor.controller.allselect) return;

                Font font = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(24f), FontStyle.Bold);
                Font font_ = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(32f), FontStyle.Bold);
                Pen pen = new Pen(Color.Lime, floor.pW(8));
                Pen pen_ = new Pen(Color.DodgerBlue, floor.pW(24));
                Pen pen_lock = new Pen(Color.Red, floor.pW(24));
                Brush br = new SolidBrush(Color.FromArgb(180, Color.Red));

                try
                {

                    StringFormat f = new StringFormat();
                    f.Alignment = StringAlignment.Center;
                    f.LineAlignment = StringAlignment.Center;

                    for (int i = 1; i < conditions.Count; i++)
                    {
                        RouteCondition prev = conditions[i - 1];
                        RouteCondition next = conditions[i];

                        if (next.cur_qr.floor.code == floor.controller.selected_floor || floor.controller.draw_allfloor)
                        {
                            if (prev.cur_qr.floor.code == next.cur_qr.floor.code)
                            {
                                double degree = Degree(prev.cur_qr.Location, next.cur_qr.Location);
                                double distance = Distance(prev.cur_qr.Location, new PointF(next.cur_qr.x, next.cur_qr.y));
                                PointF p = AngleDistance(prev.cur_qr.Location, degree, distance / 3 * 2);

                                g.DrawLine(prev.selected ? pen_ : pen, floor.pX(prev.cur_qr.x), floor.pY(prev.cur_qr.y), floor.pX(next.cur_qr.x), floor.pY(next.cur_qr.y));
                                //floor.DrawString(g, "＞", prev.selected ? font_ : font, Brushes.Blue, floor.pX(p.X), floor.pY(p.Y), (float)degree, f);
                                floor.DrawString(g, "＞", font_, Brushes.Lime, floor.pX(p.X), floor.pY(p.Y), (float)degree, f);
                            }
                        }
                    }

                    pen.Dispose();
                    pen = new Pen(Color.Magenta, floor.pW(12));
                    pen_.Dispose();
                    pen_ = new Pen(Color.DarkMagenta, floor.pW(24));

                    if (0 < conditions_temp.Count)
                    {
                        for (int i = 1; i < conditions_temp.Count; i++)
                        {
                            RouteCondition prev = conditions_temp[i - 1];
                            RouteCondition next = conditions_temp[i];

                            if (next.cur_qr.floor.code == floor.code)
                            {
                                if (prev.cur_qr.floor.code == next.cur_qr.floor.code)
                                {
                                    double degree = Degree(prev.cur_qr.Location, next.cur_qr.Location);
                                    double distance = Distance(prev.cur_qr.Location, new PointF(next.cur_qr.x, next.cur_qr.y));
                                    PointF p = AngleDistance(prev.cur_qr.Location, degree, distance / 3 * 2);

                                    g.DrawLine(prev.selected ? pen_ : pen, floor.pX(prev.cur_qr.x), floor.pY(prev.cur_qr.y), floor.pX(next.cur_qr.x), floor.pY(next.cur_qr.y));
                                    floor.DrawString(g, "＞", prev.selected ? font_ : font, Brushes.Blue, floor.pX(p.X), floor.pY(p.Y), (float)degree, f);
                                }
                            }
                        }
                    }

                    pen.Dispose();
                    pen = new Pen(Color.Red, floor.pW(3));

                    for (int i = 1; i < conditions.Count; i++)
                    {
                        RouteCondition prev = conditions[i - 1];
                        //Condition next = conditions[i];

                        if (prev.owner_agv.selected && prev.selected)
                        {
                            foreach (var v in prev.wait_other_agv_locations)
                            {
                                g.FillEllipse(br, floor.pX(v.Value.X - radius), floor.pY(v.Value.Y + radius), floor.pW(radius * 2), floor.pH(radius * 2));
                                g.DrawEllipse(pen, floor.pX(v.Value.X - radius), floor.pY(v.Value.Y + radius), floor.pW(radius * 2), floor.pH(radius * 2));
                            }
                        }
                    }
                }
                catch { }

                br.Dispose();
                pen_lock.Dispose();
                pen.Dispose();
                pen_.Dispose();
                font.Dispose();
                font_.Dispose();
            }

            public void DrawSelectedCondition(Graphics g)
            {
                Font font = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(24f));
                Pen pen = new Pen(Color.Lime, floor.pW(3));
                StringFormat f = new StringFormat();
                f.Alignment = StringAlignment.Center;
                f.LineAlignment = StringAlignment.Center;
                Brush br = null;

                if (selected && floor.conditioner.is_new_condition && floor.controller.is_mousedrag)
                {
                    FloorQR cur_qr = floor.mapeditor.HitTest(Location);
                    if (cur_qr != null)
                    {
                        PointF point;

                        string text = "＞";
                        point = floor.controller.mousePoint;
                        degree = (int)Degree(point);

                        Color color = Color.DodgerBlue;
                        FloorQR hit = floor.mapeditor.HitTest(point);
                        if (hit == null || !cur_qr.next_way.ContainsKey(hit))
                        {
                            text = "×";
                            color = Color.Red;
                        }

                        string s = degree.ToString("#0.00");
                        SizeF sz = g.MeasureString(s, SystemFonts.DefaultFont);
                        g.DrawString(s, SystemFonts.DefaultFont, Brushes.Red, floor.pX(x) - sz.Width / 2, floor.pY(y - 12));

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
                }

                if (br != null) br.Dispose();

                pen.Dispose();
                font.Dispose();
            }

            public void Draw(Graphics g)
            {
                try
                {
                    Color color = Color.Gold;
                    Pen pen = null;
                    Brush br = null;
                    {
                        if (agvRunner != null && agvRunner.communicator != null)
                        {
                            AgvController.AgvCommunicator.State sta = agvRunner.communicator.GetState;
                            color = Color.DimGray;

                            if (agvRunner.agv_request)
                            {
                                if (sta.sta_runmode)
                                {
                                    if (sta.error_code != 0)
                                    {
                                        color = Color.Red;
                                    }
                                    else if (rack != null && rack.station_working)
                                    {
                                        color = Color.Green;
                                    }
                                    else if (sta.sta_charge)
                                    {
                                        color = Color.Orange;
                                    }
                                    else
                                    {
                                        color = Color.LightGray;

                                        if (sta.bat <= agvRunner.manager.BATTERY_LOW)
                                        {
                                            color = Color.Yellow;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (sta.error_code != 0)
                                {
                                    color = Color.Red;
                                }
                                else if (sta.sta_stop)
                                {
                                    color = Color.Magenta;
                                }
                                else if (sta.sta_charge)
                                {
                                    color = Color.Orange;
                                }
                                else if (sta.bat <= agvRunner.manager.BATTERY_LOW)
                                {
                                    color = Color.Yellow;
                                }
                                else if (sta.sta_runmode)
                                {
                                    color = Color.Lime;
                                }
                            }
                        }
                        else
                        {
                            if (floor.conditioner.new_agv == this)
                            {
                                color = Color.Orange;

                                FloorQR exist_qr = floor.mapeditor.HitTest(floor.conditioner.new_agv.Location);
                                if (exist_qr == null)
                                {
                                    color = Color.Red;
                                }
                                else
                                {
                                    FloorAGV exist_agv = floor.conditioner.HitTest(floor.conditioner.new_agv.Location);
                                    if (exist_agv != null)
                                    {
                                        color = Color.Red;
                                    }
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

                        {
                            pen = new Pen(selected ? Color.Blue : Color.DarkBlue, selected ? floor.pW(10) : floor.pW(3));
                            br = new SolidBrush(selected ? Color.DodgerBlue : Color.FromArgb(180, color));
                            g.FillEllipse(br, floor.pX(x - radius), floor.pY(y + radius), floor.pW(radius * 2), floor.pH(radius * 2));
                            g.DrawEllipse(pen, floor.pX(x - radius), floor.pY(y + radius), floor.pW(radius * 2), floor.pH(radius * 2));

                            pen.Dispose();
                            pen = new Pen(Color.Blue, floor.pW(12));
                            PointF p1 = AngleDistancePoint(Location, degree, 20);
                            PointF p2 = AngleDistancePoint(Location, degree, radius - 3);
                            g.DrawLine(pen, floor.pX(p1.X), floor.pY(p1.Y), floor.pX(p2.X), floor.pY(p2.Y));

                            br.Dispose();

                            PointF p3 = AngleDistancePoint(Location, degree, radius - 8);
                            br = new SolidBrush(Color.FromArgb(180, Color.Blue));
                            StringFormat f = new StringFormat();
                            f.Alignment = StringAlignment.Center;
                            f.LineAlignment = StringAlignment.Center;
                            Font font = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(40f), FontStyle.Bold);
                            floor.DrawString(g, "＞", font, br, floor.pX(p3.X), floor.pY(p3.Y), (float)degree, f);

                            if (agvRunner != null && agvRunner.communicator != null)
                            {
                                AgvController.AgvCommunicator.State sta = agvRunner.communicator.GetState;
                                if (sta.error_code != 0)
                                {
                                    br.Dispose();
                                    br = new SolidBrush(Color.FromArgb(180, Color.White));
                                    font.Dispose();
                                    font = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(32), FontStyle.Bold);
                                    p3 = AngleDistancePoint(Location, 0, radius - 20);
                                    floor.DrawString(g, sta.error_code.ToString(), font, br, floor.pX(p3.X), floor.pY(p3.Y), (float)-90, f);
                                }
                                else
                                {
                                    br.Dispose();
                                    br = new SolidBrush(Color.FromArgb(180, Color.Red));
                                    font.Dispose();
                                    font = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(32), FontStyle.Bold);
                                    p3 = AngleDistancePoint(Location, 180, radius - 20);
                                    floor.DrawString(g, sta.bat.ToString(), font, br, floor.pX(p3.X), floor.pY(p3.Y), (float)-90, f);
                                }
                            }

                            font.Dispose();
                            br.Dispose();

                            font = new Font(SystemFonts.DefaultFont.FontFamily, floor.pW(60), FontStyle.Bold);
                            //floor.DrawString(g, id, font, Brushes.Black, floor.pX(x), floor.pY(y), (float)degree - 90, f);
                            floor.DrawString(g, id, font, Brushes.Black, floor.pX(x), floor.pY(y), -90f, f);

                            rect.X = x - radius;
                            rect.Y = y - radius;
                            rect.Width = radius * 2;
                            rect.Height = radius * 2;

                            font.Dispose();
                        }
                    }

                    if (rack != null)
                    {
                        rack.Draw(g, floor, Location, Rack.LiftStatusType.ON_AGV);
                    }

                    if (br != null)
                    {
                        br.Dispose();
                        br = null;
                    }
                }
                catch { }
            }

            #endregion

            #region 選択操作

            public void Select(bool selected)
            {
                if (floor.conditioner.selected_agv != null)
                {
                    floor.conditioner.selected_agv.selected = false;
                }

                this.selected = selected;
                if (selected)
                {
                    floor.conditioner.selected_agv = this;
                }
                else
                {
                    floor.conditioner.selected_agv = null;
                }

                if (floor.controller.EventAgvSelected != null) floor.controller.EventAgvSelected(this);
            }

            #endregion

            #region 対象検査

            public bool HitTest(PointF mousePoint)
            {
                if (rect.Contains(mousePoint))
                {
                    return true;
                }

                return false;
            }

            #endregion

            #region 条件追加・削除操作

            public RouteCondition AddNextCondition(FloorQR qr)
            {
                RouteCondition ac = null;

                foreach (var v in mode_conditions)
                {
                    if (0 == v.Value.Count)
                    {
                        ac = new RouteCondition();
                        ac.cur_qr = on_qr;
                        ac.owner_agv = this;

                        v.Value.Add(ac);
                    }
                }

                if (qr != on_qr || floor.controller.floorwarp_con != null)
                {
                    ac = new RouteCondition();
                    ac.cur_qr = qr;
                    ac.owner_agv = this;
                    ac.prev_condition = conditions[conditions.Count - 1];
                    ac.prev_condition.next_condition = ac;
                    ac.Select(true);
                    conditions.Add(ac);

                    AutoSpeedSet();
                }

                if (ac != null)
                {
                    if (floor.controller.EventEditCondition != null) floor.controller.EventEditCondition(ac);
                }

                return ac;
            }

            public void RemoveNextCondition(FloorQR qr)
            {
                int remove_index = -1;

                for (int i = conditions.Count - 1; 0 <= i; i--)
                {
                    if (conditions[i].cur_qr == qr)
                    {
                        remove_index = i;
                        break;
                    }
                }

                if (0 <= remove_index)
                {
                    if (0 < remove_index && remove_index < conditions.Count - 1)
                    {
                        conditions[remove_index - 1].next_condition = conditions[remove_index + 1];
                        conditions[remove_index + 1].prev_condition = conditions[remove_index - 1];
                    }
                    else if (0 < remove_index)
                    {
                        conditions[remove_index - 1].next_condition = null;
                    }
                    else if (remove_index < conditions.Count - 1)
                    {
                        conditions[remove_index + 1].prev_condition = null;
                    }

                    AutoSpeedSet();

                    conditions[remove_index].Select(false);

                    conditions.RemoveAt(remove_index);

                    if (floor.controller.EventEditCondition != null) floor.controller.EventEditCondition(null);
                }
            }

            #endregion

            #region 自動速度設定

            public void AutoSpeedSet()
            {
                AutoSpeedSet(true);
            }

            public void AutoSpeedSet(bool allroute)
            {
                RouteConditionList proceed_cons = new RouteConditionList();

                if (0 < conditions.Count)
                {
                    RouteCondition ac = conditions[conditions.Count - 1];

                    while (ac.prev_condition != null)
                    {
                        if (proceed_cons.Contains(ac)) break;
                        proceed_cons.Add(ac);

                        RouteCondition now = ac;
                        double deg_pre = -999;
                        bool exit = false;
                        int straight_count = 0;

                        while (!exit)
                        {
                            if (now.prev_condition != null)
                            {
                                double deg = now.prev_condition.cur_qr.Degree(now.cur_qr);
                                if (deg_pre == -999) deg_pre = deg;
                                else if (deg_pre != deg) break;
                            }

                            if (now.prev_condition != null)
                            {
                                now = now.prev_condition;
                                straight_count++;

                                if (now.wait_autorator_in_trg) break;
                                if (now.wait_autorator_out_trg) break;
                                if (now.wait_charge_trg) break;
                                if (now.wait_station_trg) break;
                                if (0 < now.wait_timer) break;
                                if (0 < now.wait_other_agv_locations.Count) break;
                                if (now.agv_turn_arrive || now.agv_turn_departure) break;
                                if (now.rack_turn_arrive || now.rack_turn_departure) break;
                                if (now.rack_down_arrive || now.rack_down_departure || now.rack_down_departure_last) break;
                                if (now.rack_up_arrive || now.rack_up_departure) break;
                            }
                            else break;
                        }

                        if (straight_count < 4)
                        {
                            int i = 1;
                            for (RouteCondition r = now; r != ac && r != null; r = r.next_condition)
                            {
                                r.speed = enSpeed.ML;

                                if (i < straight_count)
                                {
                                    r.speed = enSpeed.M;
                                }
                                i++;
                            }
                        }
                        else
                        {
                            int i = 1;
                            for (RouteCondition r = now; r != ac && r != null; r = r.next_condition)
                            {
                                r.speed = enSpeed.ML;

                                if (i < straight_count)
                                {
                                    r.speed = enSpeed.HM;

                                    if (i < straight_count - 1)
                                    {
                                        r.speed = enSpeed.H;
                                    }
                                }
                                i++;
                            }
                        }

                        if (now == null) break;

                        if (ac == now)
                        {
                            ac = now.prev_condition;
                        }
                        else ac = now;
                    }
                }
            }

            #endregion

            #region ラック操作

            public void RackUp()
            {
                if (on_qr.rack != null)
                {
                    on_qr.rack.Load(this);
                }
            }

            public void RackDown()
            {
                if (rack != null)
                {
                    rack.Unload(this);
                }
            }

            public void RackUp(FloorQR qr)
            {
                if (qr.rack != null)
                {
                    qr.rack.Load(this, qr);
                }
            }

            public void RackDown(FloorQR qr)
            {
                if (rack != null)
                {
                    rack.Unload(this, qr);
                }
            }

            #endregion

            #region 動作コマンド取得

            public AgvController.AgvCommunicator.Order[] GetOrders(int mode)
            {
                List<AgvController.AgvCommunicator.Order> orders_all = new List<AgvController.AgvCommunicator.Order>();
                Rack rack_save = null;

                if (mode_conditions.ContainsKey(mode))
                {
                    var racks = floor.mapeditor.list.Where(e => e.rack != null);

                    if (rack != null)
                    {
                        rack_save = new Rack();
                        rack_save.loading_agv = rack.loading_agv;
                        rack_save.degree = rack.degree;
                        rack_save.rack_no = rack.rack_no;
                        rack_save.sizeW = rack.sizeW;
                        rack_save.face_id = rack.face_id;
                        rack_save.defaultLocation = rack.defaultLocation;

                        //rack.degree = 0;
                    }

                    if (floor.controller.applicate_mode == enApplicateMode.MANUAL_SIMULATOR ||
                        floor.controller.applicate_mode == enApplicateMode.MODE_SIMULATOR ||
                        floor.controller.applicate_mode == enApplicateMode.OPERATION_SIMULATOR)
                    {
                        foreach (var v in mode_conditions[mode])
                        {
                            orders_all.AddRange(v.GetOrders());
                        }
                    }
                    else
                    {
                        if (0 < mode_conditions[mode].Count)
                        {
                            point = mode_conditions[mode][0].cur_qr.Location;
                            orders_all.AddRange(mode_conditions[mode][0].GetOrders2());
                            point = mode_conditions[mode][0].cur_qr.Location;
                        }
                    }

                    if (rack_save != null && rack == null)
                    {
                        foreach (var qrx in floor.mapeditor.qrs)
                        {
                            foreach (var qry in qrx.Value)
                            {
                                if (qry.Value.rack != null)
                                {
                                    FloorQR qr = qry.Value;

                                    if (qr.rack.rack_no == rack_save.rack_no)
                                    {
                                        rack = qr.rack;
                                        rack.loading_agv = rack_save.loading_agv;
                                        rack.degree = rack_save.degree;
                                        rack.defaultLocation = rack_save.defaultLocation;
                                        qr.rack = null;
                                        break;
                                    }
                                }
                            }

                            if (rack != null) break;
                        }
                    }
                }

                if (rack_save == null)
                {
                    List<Rack> racklist = new List<Rack>();
                    var v = this;
                    {
                        if (v.rack != null)
                        {
                            var qr = floor.mapeditor.HitTest(v.rack.defaultLocation);
                            if (qr != null && qr.rack == null)
                            {
                                qr.rack = v.rack;
                                qr.rack.degree = 0;
                                v.rack = null;
                            }
                            else
                            {
                                racklist.Add(v.rack);
                                v.rack = null;
                            }
                        }

                        for (int i = 0; i < racklist.Count; i++)
                        {
                            foreach (var kvx in floor.conditioner.floor.mapeditor.qrs)
                            {
                                foreach (var kvy in kvx.Value)
                                {
                                    var qr = kvy.Value;

                                    if (qr.rack == null)
                                    {
                                        qr.rack = racklist[i];
                                        qr.rack.degree = 0;
                                    }
                                }
                            }
                        }
                        racklist.Clear();
                    }

                    //foreach (var kvx in floor.mapeditor.qrs)
                    //{
                    //    foreach (var kvy in kvx.Value)
                    //    {
                    //        if (kvy.Value.rack != null) kvy.Value.rack.degree = 0;
                    //    }
                    //}
                }

                return orders_all.ToArray();
            }

            #endregion

            #region ショットコマンド取得


            public AgvCommunicator.Order ChargeStopOrder()
            {
                AgvCommunicator.Order order = new AgvCommunicator.Order();
                order.cmd = (ushort)AgvCommunicator.Order.CMD.CHARGE_STOP;
                order.x = (int)Location.X;
                order.y = (int)Location.Y;

                return order;
            }

            public AgvCommunicator.Order RouteCancelOrder()
            {
                AgvCommunicator.Order order = new AgvCommunicator.Order();
                order.cmd = (ushort)AgvCommunicator.Order.CMD.ROUTE_CANCEL;
                order.x = (int)Location.X;
                order.y = (int)Location.Y;

                return order;
            }

            public AgvCommunicator.Order RaiseErrorOrder()
            {
                AgvCommunicator.Order order = new AgvCommunicator.Order();
                order.cmd = (ushort)AgvCommunicator.Order.CMD.RAISE_ERROR;
                order.x = (int)Location.X;
                order.y = (int)Location.Y;

                return order;
            }

            public AgvCommunicator.Order Response(AgvCommunicator.State sta)
            {
                AgvCommunicator.Order res = new AgvCommunicator.Order();
                res.cmd = (ushort)AgvCommunicator.Order.CMD.RESPONSE;
                res.mod = 0;
                res.deg = sta.deg;
                res.x = sta.x;
                res.y = sta.y;
                res.rack_deg = sta.rack_deg;
                res.seq_no = sta.seq_no;

                res.mod_uncalibratable = false;

                if (on_qr != null)
                {
                    if (on_qr.direction_charge != FloorQR.enDirection.NONE) res.mod_uncalibratable = true;
                    
                    //@@@STでキャリブレーション可能にする
                    //else if (on_qr.station_id != "") res.mod_uncalibratable = true;

                    //オートレーター周辺
                    else if (on_qr.autorator_id != "") res.mod_uncalibratable = true;
                    else if (on_qr.next_way.Where(ee => ee.Key.autorator_id != "").Count() != 0) res.mod_uncalibratable = true;
                    else if (on_qr.prev_way.Where(ee => ee.autorator_id != "").Count() != 0) res.mod_uncalibratable = true;
                }

                return res;
            }

            #endregion

            #region 進路排他・予約ルートクリア

            public void Unlock()
            {
                List<FloorQR> temp = new List<FloorQR>(locked_qr_list);

                for (int i = 0; i < temp.Count; i++)
                {
                    //これから進むルートに含まれるQRはロック解除しない
                    if (0 < conditions.Where(e => e.cur_qr == temp[i]).Count()) continue;

                    temp[i].Unlock(this);
                }
            }

            public void Unreserve(bool all)
            {
                List<FloorQR> temp = new List<FloorQR>(reserved_qr_list);

                for (int i = 0; i < temp.Count; i++)
                {
                    if (!all)
                    {
                        //これから進むルートに含まれるQRは予約解除しない
                        if (0 < conditions.Where(e => e.cur_qr == temp[i]).Count()) continue;
                        temp[i].Unreserve(this, true);
                    }
                    else
                    {
                        temp[i].Unreserve(this, false);
                    }
                }
            }

            #endregion
        }

        #endregion
    }
}
