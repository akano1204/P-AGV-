using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows.Forms;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        #region マップエディタークラス

        public class AgvMapEditor
        {
            #region フィールド

            public AgvControlManager controller = null;
            public FloorMap floor = null;

            public SortedDictionary<int, SortedDictionary<int, FloorQR>> qrs = new SortedDictionary<int, SortedDictionary<int, FloorQR>>();

            public FloorQR selected_qr = null;
            public FloorQR new_qr = null;
            public FloorQR new_qr_pre = null;

            public bool is_new_qr = false;
            public bool is_new_way = false;
            public bool is_qr_editable = false;

            public Bitmap bmp = null;
            public Bitmap bmp_rack = null;
            public Bitmap bmp_agvshadow = null;

            public PointF Offset = new PointF(float.MinValue, float.MinValue);
            public float draw_scale = float.MinValue;

            public bool redraw_map = true;
            public bool redraw_rack = true;
            public bool redraw_agvshadow = true;

            public BackGround background = null;

            #endregion

            #region コンストラクタ・プロパティ

            public AgvMapEditor(FloorMap floor)
            {
                this.floor = floor;
                this.controller = floor.controller;

                background = new BackGround(this);
            }

            public int Count
            {
                get
                {
                    int count = 0;
                    foreach (var kvx in qrs)
                    {
                        foreach (var kvy in kvx.Value)
                        {
                            count++;
                        }
                    }

                    return count;
                }
            }

            public FloorQR[] list
            {
                get
                {
                    List<FloorQR> l = new List<FloorQR>();
                    foreach (var kvx in qrs)
                    {
                        foreach (var kvy in kvx.Value)
                        {
                            l.Add(kvy.Value);
                        }
                    }
                    return l.ToArray();
                }
            }


            #endregion

            #region QR追加・削除操作

            public FloorQR AddQR(int x, int y)
            {
                if (!qrs.ContainsKey((int)x)) qrs[x] = new SortedDictionary<int, FloorQR>();
                if (!qrs[x].ContainsKey(y)) qrs[x][y] = new FloorQR(this, x, y);

                return qrs[(int)x][(int)y];
            }

            public FloorQR AddQR(FloorQR qr)
            {
                if (qr == null) return null;
                int x = qr.Location.X;
                int y = qr.Location.Y;

                if (!qrs.ContainsKey(x)) qrs[x] = new SortedDictionary<int, FloorQR>();
                if (!qrs[x].ContainsKey(y)) qrs[x][y] = qr;

                return qrs[(int)x][(int)y];
            }

            public void RemoveQR(FloorQR qr)
            {
                if (qr == null) return;
                RemoveQR(qr.Location.X, qr.Location.Y);
            }

            public void RemoveQR(int x, int y)
            {
                if (qrs.ContainsKey(x))
                {
                    if (qrs[x].ContainsKey(y))
                    {
                        if (!floor.controller.is_mousedrag)
                        {
                            foreach (var v in qrs[x][y].prev_way)
                            {
                                List<FloorQR> remove_keys = new List<FloorQR>();

                                foreach (var kv in v.next_way)
                                {
                                    if (kv.Key.on_agv != null)
                                    {
                                        kv.Key.on_agv.floor.conditioner.RemoveAGV(kv.Key.on_agv);
                                    }

                                    if (kv.Key == qrs[x][y]) remove_keys.Add(kv.Key);
                                }

                                foreach (var vv in remove_keys)
                                {
                                    v.next_way.Remove(vv);
                                }

                                remove_keys.Clear();
                            }
                        }

                        qrs[x].Remove(y);
                    }

                    if (qrs[x].Count == 0)
                    {
                        qrs.Remove(x);
                    }
                }
            }

            #endregion

            #region 対象QR取得操作

            public FloorQR HitTest(PointF location)
            {
                foreach (var kvx in qrs)
                {
                    foreach (var kvy in kvx.Value)
                    {
                        if (kvy.Value.HitTest(location))
                        {
                            return kvy.Value;
                        }
                    }
                }

                return null;
            }

            public FloorQR Exist(PointF location)
            {
                if (qrs.ContainsKey((int)location.X))
                {
                    if (qrs[(int)location.X].ContainsKey((int)location.Y))
                    {
                        return qrs[(int)location.X][(int)location.Y];
                    }
                }

                return null;
            }

            #endregion

            #region 描画

            public void Draw(Graphics g, bool transparent = false)
            {
                //if (redraw_map || controller.Offset != Offset || controller.draw_scale != draw_scale || controller.applicate_mode == enApplicateMode.MAPEDITOR)
                {
                    redraw_map = false;

                    if (!transparent)
                    {
                        g.Clear(Color.DarkGray);
                    }

                    Offset.X = controller.Offset.X;
                    Offset.Y = controller.Offset.Y;
                    draw_scale = controller.draw_scale;

                    float x = floor.pX(0);
                    float y = 0;
                    float w = controller.draw_size_pixel.Width - x;
                    float h = floor.pY(0);
                    
                    if(!transparent)
                    {
                        g.FillRectangle(Brushes.White, x, y, w, h);
                        background.Draw(g);
                    }

                    if (controller.draw_status_mode) return;

                    if (floor.controller.applicate_mode != enApplicateMode.AUTO_COMMANDER)
                    {
                        List<int> xxx = new List<int>();
                        foreach (var kvx in qrs)
                        {
                            g.DrawLine(floor.pen_grid, floor.pX(kvx.Key), 0, floor.pX(kvx.Key), floor.controller.draw_size_pixel.Height);

                            foreach (var kvy in kvx.Value)
                            {
                                if (!xxx.Contains(kvy.Key))
                                {
                                    xxx.Add(kvy.Key);
                                    g.DrawLine(floor.pen_grid, 0, floor.pY(kvy.Key), floor.controller.draw_size_pixel.Width, floor.pY(kvy.Key));
                                }
                            }
                        }
                        xxx.Clear();
                    }

                    foreach (var kvx in qrs)
                    {
                        foreach (var kvy in kvx.Value)
                        {
                            FloorQR qr = kvy.Value;
                            qr.DrawWay(g);
                        }
                    }

                    if (new_qr != null)
                    {
                        new_qr.DrawWay(g);
                    }

                    if (selected_qr != null) selected_qr.DrawSelectedWay(g);

                    lock (controller)
                    {
                        foreach (var kvx in qrs)
                        {
                            foreach (var kvy in kvx.Value)
                            {
                                FloorQR qr = kvy.Value;
                                qr.Draw(g);
                            }
                        }
                    }

                    if (new_qr != null) new_qr.Draw(g);

                    redraw_rack = true;
                    redraw_agvshadow = true;
                }

                if (controller.applicate_mode != enApplicateMode.MAPEDITOR &&
                    controller.applicate_mode != enApplicateMode.CONDITIONER &&
                    controller.applicate_mode != enApplicateMode.UNKNOWN)
                {
                    if (redraw_agvshadow)
                    {
                        redraw_agvshadow = false;

                        foreach (var kvx in qrs)
                        {
                            foreach (var kvy in kvx.Value)
                            {
                                FloorQR qr = kvy.Value;
                                qr.DrawAgvShadow(g);
                            }
                        }
                    }
                }

                if (redraw_rack)
                {
                    redraw_rack = false;

                    foreach (var kvx in qrs)
                    {
                        foreach (var kvy in kvx.Value)
                        {
                            FloorQR qr = kvy.Value;
                            qr.rack?.Draw(g, floor, qr.Location, Rack.LiftStatusType.ON_FLOOR);
                        }
                    }
                }
            }

            public void _Draw(Graphics g, bool transparent = false)
            {
                #region 初期ビットマップ生成

                if (bmp == null)
                {
                    int w = 1920;
                    int h = 1920;

                    foreach (var v in System.Windows.Forms.Screen.AllScreens)
                    {
                        if (w < v.Bounds.Width) w = v.Bounds.Width;
                        if (h < v.Bounds.Height) h = v.Bounds.Height;
                    }

                    bmp = new Bitmap(w, h);
                }

                if (bmp_rack == null)
                {
                    int w = 1920;
                    int h = 1920;

                    foreach (var v in System.Windows.Forms.Screen.AllScreens)
                    {
                        if (w < v.Bounds.Width) w = v.Bounds.Width;
                        if (h < v.Bounds.Height) h = v.Bounds.Height;
                    }

                    bmp_rack = new Bitmap(w, h);
                }

                if (bmp_agvshadow == null)
                {
                    int w = 1920;
                    int h = 1920;

                    foreach (var v in System.Windows.Forms.Screen.AllScreens)
                    {
                        if (w < v.Bounds.Width) w = v.Bounds.Width;
                        if (h < v.Bounds.Height) h = v.Bounds.Height;
                    }

                    bmp_agvshadow = new Bitmap(w, h);
                }

                #endregion

                if (redraw_map || controller.Offset != Offset || controller.draw_scale != draw_scale || controller.applicate_mode == enApplicateMode.MAPEDITOR)
                {
                    redraw_map = false;

                    Graphics gg = Graphics.FromImage(bmp);

                    Offset.X = controller.Offset.X;
                    Offset.Y = controller.Offset.Y;
                    draw_scale = controller.draw_scale;

                    gg.FillRectangle(Brushes.DarkGray, 0, 0, bmp.Width, bmp.Height);

                    float x = floor.pX(0);
                    float y = 0;
                    float w = controller.draw_size_pixel.Width - x;
                    float h = floor.pY(0);
                    gg.FillRectangle(Brushes.White, x, y, w, h);

                    if (transparent)
                    {
                        gg.Dispose();
                        bmp.MakeTransparent();
                        gg = Graphics.FromImage(bmp);
                    }
                    else
                    {
                        background.Draw(gg);
                    }

                    if (floor.controller.applicate_mode != enApplicateMode.AUTO_COMMANDER)
                    {
                        List<int> xxx = new List<int>();
                        foreach (var kvx in qrs)
                        {
                            gg.DrawLine(floor.pen_grid, floor.pX(kvx.Key), 0, floor.pX(kvx.Key), floor.controller.draw_size_pixel.Height);

                            foreach (var kvy in kvx.Value)
                            {
                                if (!xxx.Contains(kvy.Key))
                                {
                                    xxx.Add(kvy.Key);
                                    gg.DrawLine(floor.pen_grid, 0, floor.pY(kvy.Key), floor.controller.draw_size_pixel.Width, floor.pY(kvy.Key));
                                }
                            }
                        }
                        xxx.Clear();
                    }

                    foreach (var kvx in qrs)
                    {
                        foreach (var kvy in kvx.Value)
                        {
                            FloorQR qr = kvy.Value;
                            qr.DrawWay(gg);
                        }
                    }

                    if (new_qr != null)
                    {
                        new_qr.DrawWay(gg);
                    }

                    if (selected_qr != null) selected_qr.DrawSelectedWay(gg);

                    foreach (var kvx in qrs)
                    {
                        foreach (var kvy in kvx.Value)
                        {
                            FloorQR qr = kvy.Value;
                            qr.Draw(gg);
                        }
                    }

                    if (new_qr != null) new_qr.Draw(gg);

                    gg.Dispose();

                    redraw_rack = true;
                    redraw_agvshadow = true;
                }

                if (redraw_rack)
                {
                    redraw_rack = false;

                    Graphics gg = Graphics.FromImage(bmp_rack);
                    gg.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
                    gg.Dispose();
                    bmp_rack.MakeTransparent();

                    gg = Graphics.FromImage(bmp_rack);

                    foreach (var kvx in qrs)
                    {
                        foreach (var kvy in kvx.Value)
                        {
                            FloorQR qr = kvy.Value;
                            qr.rack?.Draw(gg, floor, qr.Location, Rack.LiftStatusType.ON_FLOOR);
                        }
                    }

                    gg.Dispose();
                }

                if (redraw_agvshadow)
                {
                    redraw_agvshadow = false;

                    Graphics gg = Graphics.FromImage(bmp_agvshadow);
                    gg.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
                    gg.Dispose();
                    bmp_agvshadow.MakeTransparent();

                    gg = Graphics.FromImage(bmp_agvshadow);

                    foreach (var kvx in qrs)
                    {
                        foreach (var kvy in kvx.Value)
                        {
                            FloorQR qr = kvy.Value;
                            qr.DrawAgvShadow(gg);
                        }
                    }

                    gg.Dispose();
                }

                g.DrawImage(bmp, 0, 0);

                if (controller.applicate_mode != enApplicateMode.MAPEDITOR &&
                    controller.applicate_mode != enApplicateMode.CONDITIONER &&
                    controller.applicate_mode != enApplicateMode.UNKNOWN)
                {
                    g.DrawImage(bmp_agvshadow, 0, 0);
                }

                g.DrawImage(bmp_rack, 0, 0);
            }

            #endregion

            #region マウス操作

            public void MouseMove(MouseEventArgs e)
            {
                var target_floor = controller.SelectFloor();
                if (target_floor != null)
                {
                    floor.controller.mousePoint.X = (int)Math.Round(target_floor.rX(e.Location.X));
                    floor.controller.mousePoint.Y = (int)Math.Round(target_floor.rY(e.Location.Y));
                    controller.zoom_center = floor.controller.mousePoint;

                    floor.controller.is_mousedrag = false;
                    if (floor.controller.sw_dragstart.IsRunning/* && 200 < floor.controller.sw_dragstart.ElapsedMilliseconds */&& !floor.controller.is_mousedrag)
                    {
                        if (200 < floor.controller.sw_dragstart.ElapsedMilliseconds)
                        {
                            floor.controller.is_mousedrag = true;

                            if (!is_new_qr && !is_new_way)
                            {
                                if (new_qr == null && is_qr_editable)
                                {
                                    var qr = HitTest(floor.controller.mousePoint_down);
                                    if (qr != null)
                                    {
                                        qr.Select(true);

                                        RemoveQR(qr);
                                        new_qr = qr;
                                    }
                                }
                            }
                            else if (is_new_way)
                            {
                                var qr = HitTest(floor.controller.mousePoint);
                                if (qr != null)
                                {
                                    System.Diagnostics.Debug.WriteLine(qr.ToString());
                                    if (selected_qr != null) System.Diagnostics.Debug.WriteLine(selected_qr.ToString());

                                    if (qr != selected_qr)
                                    {
                                        MouseUp(e);
                                        MouseDown(e);
                                    }
                                }
                            }
                        }
                    }

                    if (is_new_qr)
                    {
                        if (floor.controller.is_mousedrag)
                        {
                            float move_x = target_floor.pW(floor.controller.mousePoint.X - floor.controller.mousePoint_down.X);
                            float move_y = target_floor.pH(floor.controller.mousePoint.Y - floor.controller.mousePoint_down.Y);

                            controller.draw_offset_pixel.X += move_x;
                            controller.draw_offset_pixel.Y += move_y;
                        }

                        controller.DoStack();
                        if (new_qr != null)
                        {
                            new_qr.Location = floor.controller.mousePoint;
                        }
                    }
                    else if (is_new_way)
                    {
                        if (selected_qr == null)
                        {
                            if (floor.controller.is_mousedrag)
                            {
                                float move_x = target_floor.pW(floor.controller.mousePoint.X - floor.controller.mousePoint_down.X);
                                float move_y = target_floor.pH(floor.controller.mousePoint.Y - floor.controller.mousePoint_down.Y);

                                controller.draw_offset_pixel.X += move_x;
                                controller.draw_offset_pixel.Y += move_y;
                            }
                        }
                        else
                        {
                            //FloorQR qr = target_floor.selected_qr;

                            //if (qr.is_autorator && 0 < qr.prev_way.Count)
                            //{
                            //    double distance = qr.Distance(qr.Location, mousePoint);
                            //    double degree = qr.prev_way[0].next_way[qr];
                            //    mousePoint = qr.AngleDistancePoint(qr.Location, degree, distance);
                            //}
                        }
                    }
                    else if (controller.is_editing_background)
                    {
                        background.MouseMove(e);

                        controller.panel_cursor = background.cursors[background.handleState];
                    }
                    else
                    {
                        if (new_qr != null)
                        {
                            controller.DoStack();

                            if (is_qr_editable)
                            {
                                if (floor.controller.is_mousedrag)
                                {
                                    FloorQR qr = new_qr;
                                    qr.Location = floor.controller.mousePoint;

                                    qr.Normalize();
                                }
                            }
                        }
                        else
                        {
                            if (floor.controller.is_mousedrag)
                            {
                                float move_x = target_floor.pW(floor.controller.mousePoint.X - floor.controller.mousePoint_down.X);
                                float move_y = target_floor.pH(floor.controller.mousePoint.Y - floor.controller.mousePoint_down.Y);

                                controller.draw_offset_pixel.X += move_x;
                                controller.draw_offset_pixel.Y += move_y;
                            }
                            else
                            {
                                controller.DoStack();
                            }
                        }
                    }

                    controller.mousePoint_last = controller.mousePoint;
                }
            }

            public void MouseDown(MouseEventArgs e)
            {
                //System.Diagnostics.Trace.WriteLine("MouseDown - " + e.Location.ToString());

                floor.controller.sw_dragstart.Restart();

                var target_floor = controller.SelectFloor();
                if (target_floor != null)
                {
                    floor.controller.mousePoint_down.X = (int)Math.Round(target_floor.rX(e.Location.X));
                    floor.controller.mousePoint_down.Y = (int)Math.Round(target_floor.rY(e.Location.Y));

                    if (controller.is_editing_background)
                    {
                        background.MouseDown(e);
                    }

                    MouseMove(e);

                    if (is_new_way)
                    {
                        var qr = HitTest(floor.controller.mousePoint_down);
                        if (qr != null)
                        {
                            qr.Select(true);
                        }
                        else if (selected_qr != null)
                        {
                            selected_qr.Select(false);
                        }
                    }
                }
            }

            public void MouseUp(MouseEventArgs e)
            {
                //System.Diagnostics.Trace.WriteLine("MouseUp - " + e.Location.ToString());

                floor.controller.sw_dragstart.Stop();

                var target_floor = controller.SelectFloor();
                if (target_floor != null)
                {
                    if (is_new_way && floor.controller.is_mousedrag)
                    {
                        if (selected_qr != null)
                        {
                            var qr = HitTest(floor.controller.mousePoint);
                            if (qr != null)
                            {
                                if (qr != selected_qr)
                                {
                                    if (e.Button == MouseButtons.Left)
                                    {
                                        bool valid = true;

                                        //if (qr.is_autorator && 0 < qr.prev_way.Count)
                                        //{
                                        //    valid = false;
                                        //}
                                        //else if (selected_qr.is_autorator && 0 < selected_qr.next_way.Count)
                                        //{
                                        //    valid = false;
                                        //}

                                        if (valid) selected_qr.AddNextWay(qr, controller.route_type);
                                    }
                                    else if (e.Button == MouseButtons.Right)
                                    {
                                        selected_qr.RemoveNextWay(qr);
                                    }
                                }
                            }
                            else
                            {
                                double degree = selected_qr.Degree(floor.controller.mousePoint);

                                List<FloorQR> exist_ways = new List<FloorQR>();
                                foreach (var kv in selected_qr.next_way)
                                {
                                    if (kv.Value - 10 < degree && degree < kv.Value + 10)
                                    {
                                        exist_ways.Add(kv.Key);
                                        break;
                                    }
                                }

                                foreach (var r in exist_ways)
                                {
                                    selected_qr.RemoveNextWay(r);
                                }
                                exist_ways.Clear();
                            }
                        }
                    }
                    else if (!is_new_qr && floor.controller.is_mousedrag)
                    {
                        if (new_qr != null)
                        {
                            if (HitTest(floor.controller.mousePoint) == null)
                            {
                                new_qr.Location = floor.controller.mousePoint;

                                if (new_qr.on_agv != null)
                                {
                                    new_qr.on_agv.SetLocation("", new_qr.Location);
                                }

                                AddQR(new_qr);
                                new_qr.Select(true);

                                List<FloorQR> check = new List<FloorQR>();
                                foreach (var v in new_qr.prev_way)
                                {
                                    check.Add(v);
                                }

                                foreach (var v in check)
                                {
                                    v.RemoveDuplicatedNextWay();
                                }

                                new_qr = null;
                                NewQR(is_new_qr);
                            }
                        }
                    }
                }

                floor.controller.is_mousedrag = false;
            }

            public void MouseClick(MouseEventArgs e)
            {
                if (floor.controller.is_mousedrag) return;

                //if (floor.controller.sw_dragstart.IsRunning && 300 < floor.controller.sw_dragstart.ElapsedMilliseconds)
                //{
                //    return;
                //}

                //System.Diagnostics.Trace.WriteLine("MouseClick - " + e.Location.ToString());

                floor.controller.sw_dragstart.Stop();
                floor.controller.is_mousedrag = false;

                var target_floor = controller.SelectFloor();
                if (target_floor != null)
                {
                    if (!is_new_qr && new_qr == null)
                    {
                        var qr = HitTest(floor.controller.mousePoint);
                        if (qr != null)
                        {
                            qr.Select(true);
                        }
                        else if (selected_qr != null)
                        {
                            selected_qr.Select(false);
                        }
                    }
                    else
                    {
                        if (new_qr != null)
                        {
                            FloorQR exist_qr = Exist(floor.controller.mousePoint);
                            if (exist_qr != null)
                            {
                                RemoveQR(exist_qr);
                                exist_qr = null;
                            }

                            if (exist_qr == null)
                            {
                                if (e.Button == MouseButtons.Left)
                                {
                                    new_qr.Location = floor.controller.mousePoint;

                                    AddQR(new_qr);

                                    new_qr_pre = new_qr;
                                    new_qr = null;

                                    NewQR(is_new_qr);

                                    if (is_new_qr && is_new_way)
                                    {
                                        if (new_qr != null)
                                        {
                                            new_qr_pre.AddNextWay(new_qr, controller.route_type);
                                        }
                                    }
                                }
                                else if (e.Button == MouseButtons.Right)
                                {
                                    exist_qr = HitTest(floor.controller.mousePoint);
                                    if (exist_qr != null) RemoveQR(exist_qr);
                                }
                            }
                            //else
                            //{
                            //    if (e.Button == MouseButtons.Left)
                            //    {

                            //    }
                            //    else if (e.Button == MouseButtons.Right)
                            //    {
                            //        RemoveQR(exist_qr);
                            //    }
                            //}
                        }
                    }
                }
            }

            #endregion

            #region 新規QRコード配置

            public void NewQR(bool is_new_qr)
            {
                var target_floor = controller.SelectFloor();
                if (target_floor != null)
                {
                    this.is_new_qr = is_new_qr;

                    if (is_new_qr)
                    {
                        if (new_qr == null)
                        {
                            new_qr = new FloorQR(this, floor.controller.mousePoint.X, floor.controller.mousePoint.Y);
                            new_qr.Select(true);

                            if (is_new_way && new_qr_pre != null && new_qr != null)
                            {
                                new_qr_pre.AddNextWay(new_qr, controller.route_type);
                            }
                        }
                    }
                    else if (new_qr != null)
                    {
                        if (new_qr_pre != null && new_qr != null)
                        {
                            new_qr_pre.RemoveNextWay(new_qr);
                        }

                        new_qr.Select(false);
                        new_qr = null;
                        new_qr_pre = null;
                    }
                }
            }

            #endregion

            #region QRコード進路設定

            public void NewWay(bool is_new_way)
            {
                var target_floor = controller.SelectFloor();
                if (target_floor != null)
                {
                    this.is_new_way = is_new_way;

                    if (!is_new_way)
                    {
                        if (new_qr_pre != null && new_qr != null)
                        {
                            new_qr_pre.RemoveNextWay(new_qr);
                        }
                    }
                    else
                    {
                        if (new_qr_pre != null && new_qr != null)
                        {
                            new_qr_pre.AddNextWay(new_qr, controller.route_type);
                        }
                    }
                }
            }

            #endregion
        }

        #endregion

        #region 背景画像クラス

        public class BackGround : INotifyPropertyChanged
        {
            #region プロパティ変化通知

            public List<string> RegisteredEventName = new List<string>();

            public event PropertyChangedEventHandler PropertyChanged;

            private void RaisePropertyChanged(string propertyName)
                => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            private void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
            {
                if (Equals(field, value)) return;

                T original = field;

                field = value;

                try
                {
                    RaisePropertyChanged(propertyName);
                }
                catch
                {
                    field = original;
                }
            }

            #endregion

            #region フィールド

            private AgvControlManager controller = null;
            private AgvMapEditor editor = null;
            private FloorMap floor = null;

            public string fileName = "";
            public Bitmap image = null;
            private float _backGroundRX = 0f;
            private float _backGroundRY = 0f;
            private float _backGroundScaleW = 1.0f;
            private float _backGroundScaleH = 1.0f;
            private float _backGroundAngle = 0f;
            private int _backGroundOpacity = 20;
            private float _grabbedRX = 0f;
            private float _grabbedRY = 0f;
            private float _grabbedScaleW = 0f;
            private float _grabbedScaleH = 0f;

            public HandleStateType handleState = HandleStateType.NONE;
            public GrabStateType grabState = GrabStateType.NONE;
            public ScaleDirectionType scaleDirection = ScaleDirectionType.NONE;

            public Dictionary<HandleStateType, Cursor> cursors = new Dictionary<HandleStateType, Cursor>();

            #endregion

            #region プロパティ

            public float BackGroundRX
            {
                get => _backGroundRX;
                set
                {
                    var limit = -1000.0f;
                    var _value = limit <= value ? value : limit;
                    SetProperty(ref _backGroundRX, _value);
                }
            }

            public float BackGroundRY
            {
                get => _backGroundRY;
                set
                {
                    var limit = -1000.0f;
                    var _value = limit <= value ? value : limit;
                    SetProperty(ref _backGroundRY, _value);
                }
            }

            public float BackGroundScaleW
            {
                get => _backGroundScaleW;
                set
                {
                    float set()
                    {
                        var limit = 0.0001f;
                        var _value = limit <= value ? value : limit;
                        SetProperty(ref _backGroundScaleW, _value);

                        return _value;
                    }

                    //set();

                    if (!controller.is_retending_aspect)
                    {
                        set();
                    }
                    else
                    {
                        switch (scaleDirection)
                        {
                            case ScaleDirectionType.NONE:
                                {
                                    scaleDirection = ScaleDirectionType.W;

                                    var original = _backGroundScaleW;

                                    var _value = set();

                                    BackGroundScaleH = _backGroundScaleH / original * _value;
                                }
                                break;

                            case ScaleDirectionType.H:
                                {
                                    set();

                                    scaleDirection = ScaleDirectionType.NONE;
                                }
                                break;
                        }
                    }
                }
            }

            public float BackGroundScaleH
            {
                get => _backGroundScaleH;
                set
                {
                    float set()
                    {
                        var limit = 0.0001f;
                        var _value = limit <= value ? value : limit;
                        SetProperty(ref _backGroundScaleH, _value);

                        return _value;
                    }

                    //set();

                    if (!controller.is_retending_aspect)
                    {
                        set();
                    }
                    else
                    {
                        switch (scaleDirection)
                        {
                            case ScaleDirectionType.NONE:
                                {
                                    scaleDirection = ScaleDirectionType.H;

                                    var original = _backGroundScaleH;

                                    var _value = set();

                                    BackGroundScaleW = _backGroundScaleW / original * _value;
                                }
                                break;

                            case ScaleDirectionType.W:
                                {
                                    set();

                                    scaleDirection = ScaleDirectionType.NONE;
                                }
                                break;
                        }
                    }
                }
            }

            public float BackGroundAngle
            {
                get => _backGroundAngle;
                set => SetProperty(ref _backGroundAngle, value);
            }

            public int BackGroundOpacity
            {
                get => _backGroundOpacity;
                set => SetProperty(ref _backGroundOpacity, value);
            }

            public float GrabbedRX
            {
                get => _grabbedRX;
                set => SetProperty(ref _grabbedRX, value);
            }

            public float GrabbedRY
            {
                get => _grabbedRY;
                set => SetProperty(ref _grabbedRY, value);
            }

            public float GrabbedScaleW
            {
                get => _grabbedScaleW;
                set => SetProperty(ref _grabbedScaleW, value);
            }

            public float GrabbedScaleH
            {
                get => _grabbedScaleH;
                set => SetProperty(ref _grabbedScaleH, value);
            }

            public bool Editing => controller.is_editing_background;

            #endregion

            #region 座標取得

            public float pX(float x) => floor.pX(x);

            public float pY(float y) => floor.pY(y);

            public float pW(float w) => floor.pW(w);

            public float pH(float h) => floor.pH(h);

            public float rX(float x) => floor.rX(x);

            public float rY(float y) => floor.rY(y);

            public float rW(float w) => floor.rW(w);

            public float rH(float h) => floor.rH(h);

            #endregion

            #region 画像読み込み

            public bool Load(Point center)
            {
                var dir = "";

                try
                {
                    dir = Path.GetDirectoryName(this.fileName);
                }
                catch { }

                var fileName = Path.GetFileName(this.fileName);
                var filter = "Image files|*.bmp;*.jpg;*.gif;*.png;*.tif|All files|*.*";

                var ofd = new OpenFileDialog
                {
                    InitialDirectory = dir,
                    FileName = fileName,
                    Filter = filter
                };

                var res = ofd.ShowDialog();

                if (res != DialogResult.OK) return false;

                this.fileName = ofd.FileName;

                image?.Dispose();

                image = new Bitmap(this.fileName);

                BackGroundRX = rX(center.X);
                BackGroundRY = rY(center.Y);
                BackGroundScaleW = 1.0f;
                BackGroundScaleH = 1.0f;
                BackGroundAngle = 0;
                BackGroundOpacity = 20;

                return true;
            }

            #endregion

            #region 画像削除

            public void Delete()
            {
                fileName = "";

                image?.Dispose();
                image = null;

                BackGroundRX = 0.0f;
                BackGroundRY = 0.0f;
                BackGroundScaleW = 1.0f;
                BackGroundScaleH = 1.0f;
                BackGroundAngle = 0;
                BackGroundOpacity = 20;
            }

            #endregion

            #region 描画

            public void Draw(Graphics g)
            {
                if (image != null)
                {
                    var originalTransform = g.Transform;

                    float bx = pX(BackGroundRX);
                    float by = pY(BackGroundRY);
                    float bw = pW(BackGroundScaleW);
                    float bh = pH(BackGroundScaleH);
                    float hw = image.Width / 2.0f;
                    float hh = image.Height / 2.0f;

                    var s = -1;

                    g.ResetTransform();

                    g.TranslateTransform(s * hw, s * hh, MatrixOrder.Append);
                    g.ScaleTransform(bw, bh, MatrixOrder.Append);
                    g.RotateTransform(-BackGroundAngle, MatrixOrder.Append);
                    g.TranslateTransform(bx, by, MatrixOrder.Append);

                    var cm = new ColorMatrix
                    {
                        Matrix00 = 1,
                        Matrix11 = 1,
                        Matrix22 = 1,
                        Matrix33 = BackGroundOpacity / 100.0f,
                        Matrix44 = 1
                    };

                    var ia = new ImageAttributes();
                    ia.SetColorMatrix(cm);

                    g.DrawImage(image,
                        new Rectangle(0, 0, image.Width, image.Height),
                        0, 0, image.Width, image.Height,
                        GraphicsUnit.Pixel, ia);

                    if (Editing)
                    {
                        g.Transform = originalTransform;

                        var tm = Matrix.GetTranslate2(bx, by);
                        var rm = Matrix.GetRotate2(-BackGroundAngle);
                        var sm = Matrix.GetScale2(bw, bh);
                        var hm = Matrix.GetTranslate2(-hw, -hh);

                        var trans_matrix = tm * rm * sm * hm;

                        var vertices = new List<Vector>();

                        for (var r = 0; r <= 2; r++)
                        {
                            for (var c = 0; c <= 2; c++)
                            {
                                if (r == 1 && c == 1) continue;

                                vertices.Add(trans_matrix * (new Vector(c * hw, r * hh, 1.0)));
                            }
                        }

                        var frame_style = DashStyle.Dash;
                        var frame_color = Color.Gray;
                        var frame_width = 2.0f;

                        var frame_pen = new Pen(frame_color, frame_width)
                        {
                            DashStyle = frame_style,
                        };

                        var frame_points = new PointF[4]
                            {
                                new PointF((float)vertices[0][1], (float)vertices[0][2]),
                                new PointF((float)vertices[2][1], (float)vertices[2][2]),
                                new PointF((float)vertices[7][1], (float)vertices[7][2]),
                                new PointF((float)vertices[5][1], (float)vertices[5][2])
                            };

                        g.DrawPolygon(frame_pen, frame_points);

                        frame_pen.Dispose();

                        var handle_pen = new Pen(frame_color, 1.4f);

                        var radius = 10.0f;

                        foreach (var v in vertices)
                        {
                            g.DrawRectangle(handle_pen, (float)v[1] - radius / 2.0f, (float)v[2] - radius / 2.0f, radius, radius);
                        }

                        handle_pen.Dispose();
                    }

                    g.Transform = originalTransform;
                }
            }

            #endregion

            #region キー操作

            public void KeyDown(KeyEventArgs e)
            {
                float move_x = 0;
                float move_y = 0;

                switch (e.KeyCode)
                {
                    case Keys.Up: move_y = 1; break;
                    case Keys.Down: move_y = -1; break;
                    case Keys.Left: move_x = -1; break;
                    case Keys.Right: move_x = 1; break;
                }

                BackGroundRX += rW(move_x);
                BackGroundRY += rH(move_y);
            }

            #endregion

            #region マウス操作

            public void MouseMove(MouseEventArgs e)
            {
                var controller = floor.controller;
                var target_floor = controller.SelectFloor();

                var cv = new Vector(BackGroundRX, BackGroundRY);
                var ov = new Vector(controller.mousePoint_last.X, controller.mousePoint_last.Y);
                var gv = new Vector(controller.mousePoint.X, controller.mousePoint.Y);
                var dv = new Vector(controller.mousePoint_down.X, controller.mousePoint_down.Y);
                var grabbed_center_v = new Vector(GrabbedRX, GrabbedRY);

                var cov = ov - cv;
                var cgv = gv - cv;
                var dgv = gv - dv;

                void SetRotationCursor()
                {
                    var angle = Algebra.GetRoundedDegree(-cgv.AngleX + 22.5);

                    handleState = (int)(angle / 45.0) + HandleStateType.R_R;
                }

                if (controller.is_mousedrag)
                {
                    float move_x = target_floor.pW(controller.mousePoint.X - controller.mousePoint_down.X);
                    float move_y = target_floor.pH(controller.mousePoint.Y - controller.mousePoint_down.Y);

                    switch (grabState)
                    {
                        case GrabStateType.NONE:
                            {
                                controller.draw_offset_pixel.X += move_x;
                                controller.draw_offset_pixel.Y += move_y;
                            }
                            break;

                        case GrabStateType.TRANSLATE:
                            {
                                BackGroundRX = (float)(grabbed_center_v[1] + dgv[1]);
                                BackGroundRY = (float)(grabbed_center_v[2] + dgv[2]);
                            }
                            break;

                        case GrabStateType.ROTATE:
                            {
                                var angle_temp = BackGroundAngle + (float)(cgv.AngleX - cov.AngleX);
                                BackGroundAngle = (float)Algebra.GetRoundedDegree(angle_temp);

                                SetRotationCursor();
                            }
                            break;

                        case GrabStateType.S_R:
                        case GrabStateType.S_L:
                        case GrabStateType.S_B:
                        case GrabStateType.S_T:
                            {
                                var base_angle = 0.0;
                                var is_lateral = false;

                                switch (grabState)
                                {
                                    case GrabStateType.S_R: base_angle = 0.0; is_lateral = true; break;
                                    case GrabStateType.S_L: base_angle = 180.0; is_lateral = true; break;
                                    case GrabStateType.S_B: base_angle = 270.0; break;
                                    case GrabStateType.S_T: base_angle = 90.0; break;
                                }

                                var normal_vector = Vector.GetUnitVector(BackGroundAngle + base_angle);

                                var move = dgv.Dot(normal_vector);
                                var coef = 0.0;

                                if (is_lateral)
                                {
                                    BackGroundScaleW = (float)(GrabbedScaleW + move / image.Width);
                                    coef = (BackGroundScaleW - GrabbedScaleW) * image.Width / 2.0;
                                }
                                else
                                {
                                    BackGroundScaleH = (float)(GrabbedScaleH + move / image.Height);
                                    coef = (BackGroundScaleH - GrabbedScaleH) * image.Height / 2.0;
                                }

                                BackGroundRX = (float)(GrabbedRX + coef * normal_vector[1]);
                                BackGroundRY = (float)(GrabbedRY + coef * normal_vector[2]);
                            }
                            break;

                        case GrabStateType.S_BR:
                        case GrabStateType.S_BL:
                        case GrabStateType.S_TR:
                        case GrabStateType.S_TL:
                            {
                                var baseL_angle = 0.0;
                                var baseV_angle = 0.0;

                                switch (grabState)
                                {
                                    case GrabStateType.S_BR: baseL_angle = 0.0; baseV_angle = 270.0; break;
                                    case GrabStateType.S_BL: baseL_angle = 180.0; baseV_angle = 270.0; break;
                                    case GrabStateType.S_TR: baseL_angle = 0.0; baseV_angle = 90.0; break;
                                    case GrabStateType.S_TL: baseL_angle = 180.0; baseV_angle = 90.0; break;
                                }

                                var normalL_vector = Vector.GetUnitVector(BackGroundAngle + baseL_angle);
                                var normalV_vector = Vector.GetUnitVector(BackGroundAngle + baseV_angle);

                                var moveL = dgv.Dot(normalL_vector);
                                var moveV = dgv.Dot(normalV_vector);
                                var coefL = 0.0;
                                var coefV = 0.0;

                                void setScaleW()
                                {
                                    BackGroundScaleW = (float)(GrabbedScaleW + moveL / image.Width);
                                }

                                void setScaleH()
                                {
                                    BackGroundScaleH = (float)(GrabbedScaleH + moveV / image.Height);
                                }

                                if (!controller.is_retending_aspect)
                                {
                                    setScaleW();
                                    setScaleH();
                                }
                                else
                                {
                                    if (moveL / image.Width < moveV / image.Height)
                                    {
                                        setScaleH();
                                    }
                                    else
                                    {
                                        setScaleW();
                                    }
                                }

                                coefL = (BackGroundScaleW - GrabbedScaleW) * image.Width / 2.0;
                                coefV = (BackGroundScaleH - GrabbedScaleH) * image.Height / 2.0;

                                BackGroundRX = (float)(GrabbedRX + coefL * normalL_vector[1] + coefV * normalV_vector[1]);
                                BackGroundRY = (float)(GrabbedRY + coefL * normalL_vector[2] + coefV * normalV_vector[2]);
                            }
                            break;
                    }
                }
                else
                {
                    if (image != null)
                    {
                        var rx = controller.mousePoint.X;
                        var ry = controller.mousePoint.Y;

                        var v = new Vector(rx, ry, 1.0);
                        var tm = Matrix.GetTranslate2(-BackGroundRX, -BackGroundRY);
                        var rm = Matrix.GetRotate2(-BackGroundAngle);
                        var sm = Matrix.GetScale2(1.0 / BackGroundScaleW, 1.0 / BackGroundScaleH);

                        var rotated_vector = sm * rm * tm * v;

                        var mouse_x = (float)rotated_vector[1];
                        var mouse_y = (float)rotated_vector[2];

                        var hw = image.Width / 2.0f;
                        var hh = image.Height / 2.0f;

                        var quad = new QuadrangleInfo
                        {
                            point1 = new PointF(-hw, hh),
                            point2 = new PointF(hw, hh),
                            point3 = new PointF(hw, -hh),
                            point4 = new PointF(-hw, -hh)
                        };

                        var bulge1 = target_floor.rW(18.0f);

                        var bulge1_x = bulge1 / BackGroundScaleW;
                        var bulge1_y = bulge1 / BackGroundScaleH;

                        var scale_quad = new QuadrangleInfo
                        {
                            point1 = new PointF(quad.point1.X - bulge1_x, quad.point1.Y + bulge1_y),
                            point2 = new PointF(quad.point2.X + bulge1_x, quad.point2.Y + bulge1_y),
                            point3 = new PointF(quad.point3.X + bulge1_x, quad.point3.Y - bulge1_y),
                            point4 = new PointF(quad.point4.X - bulge1_x, quad.point4.Y - bulge1_y),
                        };

                        var bulge2 = target_floor.rW(40.0f);

                        var bulge2_x = bulge2 / BackGroundScaleW;
                        var bulge2_y = bulge2 / BackGroundScaleH;

                        var rotate_quad = new QuadrangleInfo
                        {
                            point1 = new PointF(scale_quad.point1.X - bulge2_x, scale_quad.point1.Y + bulge2_y),
                            point2 = new PointF(scale_quad.point2.X + bulge2_x, scale_quad.point2.Y + bulge2_y),
                            point3 = new PointF(scale_quad.point3.X + bulge2_x, scale_quad.point3.Y - bulge2_y),
                            point4 = new PointF(scale_quad.point4.X - bulge2_x, scale_quad.point4.Y - bulge2_y),
                        };

                        if (CollisionDetector.InsideQuadrangle(mouse_x, mouse_y, quad))
                        {
                            handleState = HandleStateType.TRANSLATE;

                            grabState = GrabStateType.TRANSLATE;
                        }
                        else
                        {
                            if (CollisionDetector.InsideQuadrangle(mouse_x, mouse_y, scale_quad))
                            {
                                var smaller_side = Math.Min(image.Width, image.Height);

                                var size = Math.Min(target_floor.rW(12.0f), smaller_side / 3.0f);

                                var hwm = hw - size / BackGroundScaleW;
                                var hhm = hh - size / BackGroundScaleH;

                                var rot_x = rotated_vector[1];
                                var rot_y = rotated_vector[2];

                                GrabStateType area_x = 0;
                                GrabStateType area_y = 0;

                                if (rot_x > hwm)
                                {
                                    area_x = GrabStateType.S_R;
                                }
                                else if (rot_x < -hwm)
                                {
                                    area_x = GrabStateType.S_L;
                                }
                                else
                                {
                                    area_x = 0;
                                }

                                if (rot_y > hhm)
                                {
                                    area_y = GrabStateType.S_T;
                                }
                                else if (rot_y < -hhm)
                                {
                                    area_y = GrabStateType.S_B;
                                }
                                else
                                {
                                    area_y = 0;
                                }

                                grabState = (int)area_x + area_y;

                                var area_no = 0;

                                switch (grabState)
                                {
                                    case GrabStateType.S_R: area_no = 0; break;
                                    case GrabStateType.S_BR: area_no = 1; break;
                                    case GrabStateType.S_B: area_no = 2; break;
                                    case GrabStateType.S_BL: area_no = 3; break;
                                    case GrabStateType.S_L: area_no = 4; break;
                                    case GrabStateType.S_TL: area_no = 5; break;
                                    case GrabStateType.S_T: area_no = 6; break;
                                    case GrabStateType.S_TR: area_no = 7; break;
                                }

                                var normal_angle = 45.0 * area_no;
                                var nd = 22.5;

                                normal_angle -= BackGroundAngle;
                                normal_angle = Algebra.GetRoundedDegree(normal_angle + nd);

                                var separate_angles = new double[8]
                                    {
                                        0.0,
                                        nd * 2,
                                        90.0,
                                        90.0 + nd * 2,
                                        180.0,
                                        180.0 + nd * 2,
                                        270.0,
                                        270.0 + nd * 2
                                    };

                                var range_no = 0;

                                var a_len = separate_angles.Length;

                                for (int i = 0; i < a_len; i++)
                                {
                                    var n = i != a_len - 1 ? i + 1 : 0;

                                    var a_from = separate_angles[i];
                                    var a_to = separate_angles[n];

                                    if (n == 0) a_to = 360.0;

                                    if (a_from < normal_angle && normal_angle <= a_to)
                                    {
                                        range_no = i;

                                        break;
                                    }
                                }

                                handleState = range_no % 4 + HandleStateType.S_L;
                            }
                            else if (CollisionDetector.InsideQuadrangle(mouse_x, mouse_y, rotate_quad))
                            {
                                SetRotationCursor();

                                grabState = GrabStateType.ROTATE;
                            }
                            else
                            {
                                handleState = HandleStateType.NONE;

                                grabState = GrabStateType.NONE;
                            }
                        }
                    }
                }
            }

            public void MouseDown(MouseEventArgs e)
            {
                if (grabState != GrabStateType.NONE)
                {
                    GrabbedRX = BackGroundRX;
                    GrabbedRY = BackGroundRY;
                    GrabbedScaleW = BackGroundScaleW;
                    GrabbedScaleH = BackGroundScaleH;
                }
            }

            #endregion

            #region コンストラクタ

            public BackGround(AgvMapEditor editor)
            {
                this.controller = editor.controller;
                this.editor = editor;
                this.floor = editor.floor;

                var assembly = Assembly.GetExecutingAssembly();

                foreach (HandleStateType e in Enum.GetValues(typeof(HandleStateType)))
                {
                    var bmp = (Bitmap)Properties.Resources.ResourceManager.GetObject($"{e}");

                    if (bmp != null)
                    {
                        var hIcon = bmp.GetHicon();
                        cursors[e] = new Cursor(hIcon);
                    }
                    else
                    {
                        cursors[e] = Cursors.Default;
                    }

                    //using (var s = assembly.GetManifestResourceStream($"LogisticAgvRouteEditor.Resources.Cursor.{e}.png"))
                    //{
                    //    if (s != null)
                    //    {
                    //        using (Bitmap bmp = new Bitmap(s))
                    //        {
                    //            var hIcon = bmp.GetHicon();
                    //            cursors[e] = new Cursor(hIcon);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        cursors[e] = default;
                    //    }
                    //}
                }
            }

            #endregion

            #region 列挙体

            public enum HandleStateType
            {
                NONE,
                S_L,
                S_DL,
                S_V,
                S_DR,
                R_R,
                R_BR,
                R_B,
                R_BL,
                R_L,
                R_TL,
                R_T,
                R_TR,
                TRANSLATE
            }

            public enum GrabStateType
            {
                NONE = 0,
                S_R = 1,
                S_L = 1 << 1,
                S_B = 1 << 2,
                S_T = 1 << 3,
                S_BR = S_B + S_R,
                S_BL = S_B + S_L,
                S_TR = S_T + S_R,
                S_TL = S_T + S_L,
                ROTATE,
                TRANSLATE
            }

            public enum ScaleDirectionType
            {
                NONE,
                W,
                H
            }

            #endregion
        }

        #endregion

        #region 衝突判定クラス

        public static class CollisionDetector
        {
            #region 四角形の内部に存在する座標かどうか判定

            /// <summary>
            /// 四角形の内部に存在する座標かどうか判定
            /// </summary>
            public static bool InsideQuadrangle(double mouseX, double mouseY, QuadrangleInfo quad)
            {
                bool status = false;
                double vx1, vy1, vx2, vy2;
                double ax1, ay1, ax2, ay2;
                double bx1, by1, bx2, by2;
                double cx1, cy1, cx2, cy2;
                double ans1, ans2, ans3, ans4;

                vx1 = quad.point2.X - quad.point1.X;
                vy1 = quad.point2.Y - quad.point1.Y;
                vx2 = mouseX - quad.point1.X;
                vy2 = mouseY - quad.point1.Y;
                ans1 = vx1 * vy2 - vy1 * vx2;

                ax1 = quad.point3.X - quad.point2.X;
                ay1 = quad.point3.Y - quad.point2.Y;
                ax2 = mouseX - quad.point2.X;
                ay2 = mouseY - quad.point2.Y;
                ans2 = ax1 * ay2 - ay1 * ax2;

                bx1 = quad.point4.X - quad.point3.X;
                by1 = quad.point4.Y - quad.point3.Y;
                bx2 = mouseX - quad.point3.X;
                by2 = mouseY - quad.point3.Y;
                ans3 = bx1 * by2 - by1 * bx2;

                cx1 = quad.point1.X - quad.point4.X;
                cy1 = quad.point1.Y - quad.point4.Y;
                cx2 = mouseX - quad.point4.X;
                cy2 = mouseY - quad.point4.Y;
                ans4 = cx1 * cy2 - cy1 * cx2;

                if ((ans1 < 0 && ans2 < 0 && ans3 < 0 && ans4 < 0) ||
                    (ans1 > 0 && ans2 > 0 && ans3 > 0 && ans4 > 0))
                {
                    status = true;
                }

                return status;
            }

            #endregion
        }

        public class QuadrangleInfo
        {
            public PointF point1;
            public PointF point2;
            public PointF point3;
            public PointF point4;
        }

        #endregion

        #region 線形代数

        public static class Algebra
        {
            public static double GetRoundedDegree(double degree)
            {
                return degree - 360.0 * Math.Floor(degree / 360.0);
            }
        }

        public struct Vector
        {
            private double[] _components;

            public int Size => _components.Length;

            public double Abs => Math.Sqrt(_components.Sum(x => x * x));

            public double AngleX => Angle2(1);

            public double AngleY => Angle2(2);

            // 軸間角度(deg)
            public double Angle2(int index)
            {
                if (Abs != 0.0)
                {
                    var sign = 0.0;

                    switch (index)
                    {
                        case 1: sign = 0.0 < this[2] ? 1.0 : -1.0; break;
                        case 2: sign = 0.0 < this[1] ? 1.0 : -1.0; break;
                    }

                    return sign * Math.Acos(this[index] / Abs) * 180.0 / Math.PI;
                }
                else
                {
                    return 0.0;
                }
            }

            public Vector(int size)
            {
                _components = new double[size];
            }

            public Vector(params double[] components)
            {
                _components = components;
            }

            public double this[int index]
            {
                get => _components[index - 1];
                set => _components[index - 1] = value;
            }

            public double Dot(Vector v)
            {
                if (Size == v.Size)
                {
                    var ret = 0.0;

                    for (int i = 1; i <= Size; i++)
                    {
                        ret += this[i] * v[i];
                    }

                    return ret;
                }
                else
                {
                    throw new ArithmeticException();
                }
            }

            public Vector Cross(Vector v)
            {
                if (Size == v.Size && Size == 3)
                {
                    return new Vector(this[2] * v[3] - this[3] * v[2], this[3] * v[1] - this[1] * v[3], this[1] * v[2] - this[2] * v[1]);
                }
                else
                {
                    throw new ArithmeticException();
                }
            }

            // 回転後ベクトル()
            public Vector Rot2(double degree)
            {
                if (Size != 2) throw new MethodAccessException();

                degree += AngleX;

                return Abs * GetUnitVector(degree);
            }

            public static Vector GetUnitVector(double degree)
            {
                var rad = degree * Math.PI / 180.0;

                return new Vector(Math.Cos(rad), Math.Sin(rad));
            }

            public Matrix ToMatrix()
            {
                var ret = new Matrix(Size, 1);

                for (int i = 1; i <= Size; i++)
                {
                    ret[i, 1] = this[i];
                }

                return ret;
            }

            public static Vector operator +(Vector v1, Vector v2)
            {
                if (v1.Size == v2.Size)
                {
                    var ret = new double[v1.Size];

                    for (int i = 1; i <= v1.Size; i++)
                    {
                        ret[i - 1] = v1[i] + v2[i];
                    }

                    return new Vector(ret);
                }
                else
                {
                    throw new ArithmeticException();
                }
            }

            public static Vector operator -(Vector v1, Vector v2)
            {
                if (v1.Size == v2.Size)
                {
                    var ret = new double[v1.Size];

                    for (int i = 1; i <= v1.Size; i++)
                    {
                        ret[i - 1] = v1[i] - v2[i];
                    }

                    return new Vector(ret);
                }
                else
                {
                    throw new ArithmeticException();
                }
            }

            public static Vector operator -(Vector v)
            {
                return new Vector(v._components.Select(x => -x).ToArray());
            }

            public static Vector operator *(double a, Vector v)
            {
                return new Vector(v._components.Select(x => a * x).ToArray());
            }

            public static Vector operator *(Vector v, double a)
            {
                return a * v;
            }

            public static Vector operator /(Vector v, double a)
            {
                return new Vector(v._components.Select(x => x / a).ToArray());
            }
        }

        public struct Matrix
        {
            private double[,] _components;

            public int Row => _components.GetLength(0);
            public int Column => _components.GetLength(1);

            public Matrix(int row, int column)
            {
                _components = new double[row, column];
            }

            public double this[int row, int column]
            {
                get => _components[row - 1, column - 1];
                set => _components[row - 1, column - 1] = value;
            }

            public Vector ToVector()
            {
                if (Column == 1)
                {
                    var ret = new double[Row];

                    for (int r = 1; r <= Row; r++)
                    {
                        ret[r - 1] = this[r, 1];
                    }

                    return new Vector(ret);
                }
                else
                {
                    throw new ArithmeticException();
                }
            }

            // 2次同時座標平行移動行列(deg)
            public static Matrix GetTranslate2(double x, double y)
            {
                var ret = new Matrix(3, 3);

                ret[1, 1] = 1.0;
                ret[1, 3] = x;
                ret[2, 2] = 1.0;
                ret[2, 3] = y;
                ret[3, 3] = 1.0;

                return ret;
            }

            // 2次同時座標回転行列(deg)
            public static Matrix GetRotate2(double degree)
            {
                var rad = degree * Math.PI / 180.0;

                var ret = new Matrix(3, 3);

                ret[1, 1] = Math.Cos(rad);
                ret[1, 2] = -Math.Sin(rad);
                ret[2, 1] = Math.Sin(rad);
                ret[2, 2] = Math.Cos(rad);
                ret[3, 3] = 1.0;

                return ret;
            }

            // 2次同時座標回転行列(deg)
            public static Matrix GetScale2(double x, double y)
            {
                var ret = new Matrix(3, 3);

                ret[1, 1] = x;
                ret[2, 2] = y;
                ret[3, 3] = 1.0;

                return ret;
            }

            public static Matrix operator +(Matrix m1, Matrix m2)
            {
                if (m1.Row == m2.Row && m1.Column == m2.Column)
                {
                    var ret = new Matrix(m1.Row, m1.Column);

                    for (int r = 1; r <= m1.Row; r++)
                    {
                        for (int c = 1; c <= m1.Column; c++)
                        {
                            ret[r, c] = m1[r, c] + m2[r, c];
                        }
                    }

                    return ret;
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            public static Matrix operator -(Matrix m1, Matrix m2)
            {
                if (m1.Row == m2.Row && m1.Column == m2.Column)
                {
                    var ret = new Matrix(m1.Row, m1.Column);

                    for (int r = 1; r <= m1.Row; r++)
                    {
                        for (int c = 1; c <= m1.Column; c++)
                        {
                            ret[r, c] = m1[r, c] - m2[r, c];
                        }
                    }

                    return ret;
                }
                else
                {
                    throw new ArgumentException();
                }
            }

            public static Matrix operator -(Matrix m)
            {
                for (int r = 1; r <= m.Row; r++)
                {
                    for (int c = 1; c <= m.Column; c++)
                    {
                        m[r, c] *= -1.0;
                    }
                }

                return m;
            }

            public static Matrix operator *(Matrix m1, Matrix m2)
            {
                if (m1.Column == m2.Row)
                {
                    var ret = new Matrix(m1.Row, m2.Column);

                    for (int r = 1; r <= ret.Row; r++)
                    {
                        for (int c = 1; c <= ret.Column; c++)
                        {
                            var sum = 0.0;

                            for (int c1 = 1; c1 <= m1.Column; c1++)
                            {
                                sum += m1[r, c1] * m2[c1, c];
                            }

                            ret[r, c] = sum;
                        }
                    }

                    return ret;
                }
                else
                {
                    throw new ArithmeticException();
                }
            }

            public static Vector operator *(Matrix m, Vector v)
            {
                var vm = v.ToMatrix();

                var mul = m * vm;

                return mul.ToVector();
            }

            public static Matrix operator *(Vector v, Matrix m)
            {
                var vm = v.ToMatrix();

                var mul = vm * m;

                return mul;
            }

            public static Matrix operator *(double a, Matrix m)
            {
                for (int r = 1; r <= m.Row; r++)
                {
                    for (int c = 1; c <= m.Column; c++)
                    {
                        m[r, c] *= a;
                    }
                }

                return m;
            }

            public static Matrix operator *(Matrix m, double a)
            {
                return a * m;
            }

            public static Matrix operator /(Matrix m, double a)
            {
                for (int r = 1; r <= m.Row; r++)
                {
                    for (int c = 1; c <= m.Column; c++)
                    {
                        m[r, c] /= a;
                    }
                }

                return m;
            }
        }

        #endregion
    }
}
