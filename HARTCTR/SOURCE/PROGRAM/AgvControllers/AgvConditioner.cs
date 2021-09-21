using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PROGRAM
{
    public partial class AgvControlManager
    {
        public class AgvConditioner
        {
            #region フィールド

            public AgvControlManager controller = null;
            public FloorMap floor = null;

            public FloorAGV new_agv = null;
            public FloorAGV new_agv_pre = null;

            public bool is_new_agv = false;
            public bool is_new_condition = false;
            public bool keep_state = false;

            //public bool is_auto_route = false;

            #endregion

            #region コンストラクタ・プロパティ

            public FloorAGV selected_agv
            {
                get { return controller.selected_agv; }
                set { controller.selected_agv = value; }
            }

            public AgvConditioner(FloorMap floor)
            {
                this.floor = floor;
                this.controller = floor.controller;
            }

            //public int Count
            //{
            //    get
            //    {
            //        return agvs.Count;
            //    }
            //}

            #endregion

            #region AGV追加・削除操作

            public void AddAGV(FloorAGV agv)
            {
                if (agv == null) return;

                FloorAGV exist_agv = null;
                foreach (var v in controller.agvs)
                {
                    if (v.HitTest(agv.Location) && agv.floor.code == v.floor.code)
                    {
                        exist_agv = v;
                        break;
                    }
                }

                if (exist_agv == null)
                {
                    var qr = floor.mapeditor.HitTest(agv.Location);
                    if (qr != null)
                    {
                        agv.on_qr = qr;
                        //qr.on_agv = agv;
                        qr.PlaceAgv(agv);
                        controller.agvs.Add(agv);

                        agv.AddNextCondition(qr);
                    }
                }
            }

            public void RemoveAGV(FloorAGV agv)
            {
                if (agv == null) return;
                
                foreach (var a in controller.agvs)
                {
                    foreach (var conditions in a.mode_conditions)
                    {
                        foreach (var c in conditions.Value)
                        {
                            List<FloorAGV> remove_loc = new List<FloorAGV>();
                            foreach (var v in c.wait_other_agv_locations)
                            {
                                if (v.Key == agv)
                                {
                                    remove_loc.Add(v.Key);
                                }
                            }

                            foreach (var v in remove_loc)
                            {
                                c.wait_other_agv_locations.Remove(v);
                            }
                        }
                    }
                }

                {
                    List<FloorQR> remove_qr = new List<FloorQR>();

                    foreach (var c in agv.conditions)
                    {
                        remove_qr.Add(c.cur_qr);
                    }

                    foreach (var v in remove_qr)
                    {
                        agv.RemoveNextCondition(v);
                    }
                }

                lock (agv)
                {
                    if (agv.rack != null)
                    {
                        var exist_qr = floor.mapeditor.HitTest(agv.Location);
                        if (exist_qr != null)
                        {
                            exist_qr.rack = agv.rack;
                        }
                    }

                    if (agv.rack != null) agv.rack.Unload(agv);

                    //agv.on_qr.on_agv = null;
                    agv.on_qr.UnplaceAgv(agv);
                }

                controller.agvs.Remove(agv);
            }

            #endregion

            #region 対象AGV取得操作

            public FloorAGV HitTest(PointF location)
            {
                foreach (var v in controller.agvs)
                {
                    if (v.floor == floor)
                    {
                        if (v.HitTest(location))
                        {
                            return v;
                        }
                    }
                }

                return null;
            }

            public FloorAGV Exist(PointF location)
            {
                foreach (var v in controller.agvs)
                {
                    if (v.floor == floor)
                    {
                        if (v.x == location.X && v.y == location.Y)
                        {
                            return v;
                        }
                    }
                }

                return null;
            }

            #endregion

            #region 描画

            public void Draw(Graphics g)
            {
                if (!controller.draw_status_mode)
                {
                    foreach (var v in controller.agvs)
                    {
                        //if (v.floor == floor)
                        {
                            if (0 < v.conditions.Count)
                            {
                                v.DrawCondition(g);
                            }
                        }
                    }

                    //if (selected_agv != null) selected_agv.DrawSelectedCondition(g);

                    foreach (var v in controller.agvs)
                    {
                        if (v.floor == floor)
                        {
                            v.Draw(g);
                        }
                    }

                    if (new_agv != null) new_agv.Draw(g);
                }
                else
                {
                    foreach (var v in controller.agvs)
                    {
                        if (v.floor == floor /*&& v.selected*/)
                        {
                            v.Draw(g);
                        }
                    }
                }
            }

            #endregion

            #region マウス操作

            public bool MouseMove(MouseEventArgs e)
            {
                bool ret = false;

                var target_floor = controller.SelectFloor();
                if (target_floor != null)
                {
                    controller.mousePoint.X = (int)Math.Round(target_floor.rX(e.Location.X));
                    controller.mousePoint.Y = (int)Math.Round(target_floor.rY(e.Location.Y));
                    controller.zoom_center = controller.mousePoint;

                    controller.is_mousedrag = false;
                    if (controller.sw_dragstart.IsRunning && 200 < controller.sw_dragstart.ElapsedMilliseconds && !controller.is_mousedrag)
                    {
                        controller.is_mousedrag = true;

                        if (!is_new_agv && !is_new_condition && controller.applicate_mode == enApplicateMode.CONDITIONER)
                        {
                            if (new_agv == null)
                            {
                                var agv = HitTest(controller.mousePoint_down);
                                if (agv != null)
                                {
                                    agv.Select(true);

                                    RemoveAGV(agv);
                                    new_agv = agv;
                                }
                            }
                        }
                        else if (is_new_condition)
                        {

                        }
                    }

                    if (is_new_agv)
                    {
                        if (controller.is_mousedrag)
                        {
                            float move_x = target_floor.pW(controller.mousePoint.X - controller.mousePoint_down.X);
                            float move_y = target_floor.pH(controller.mousePoint.Y - controller.mousePoint_down.Y);

                            controller.draw_offset_pixel.X += move_x;
                            controller.draw_offset_pixel.Y += move_y;
                        }

                        controller.DoStack();
                        if (new_agv != null)
                        {
                            var qr = target_floor.mapeditor.HitTest(controller.mousePoint);
                            if (qr != null)
                            {
                                controller.mousePoint = qr.Location;
                            }

                            new_agv.SetLocation("", controller.mousePoint);
                        }
                    }
                    else if (is_new_condition)
                    {
                        if (controller.is_mousedrag)
                        {
                            if (controller.is_mousedrag)
                            {
                                float move_x = target_floor.pW(controller.mousePoint.X - controller.mousePoint_down.X);
                                float move_y = target_floor.pH(controller.mousePoint.Y - controller.mousePoint_down.Y);

                                controller.draw_offset_pixel.X += move_x;
                                controller.draw_offset_pixel.Y += move_y;
                            }
                        }
                        else if (selected_agv != null)
                        {
                            var qr = floor.mapeditor.HitTest(controller.mousePoint);
                            if (qr != null)
                            {
                                FloorQR from =
                                    (0 < selected_agv.conditions.Count)
                                    ? selected_agv.conditions[selected_agv.conditions.Count - 1].cur_qr
                                    : selected_agv.on_qr;
                                var cons = controller.routeManager.GetAutoRoute(selected_agv, from, qr);
                                if (1 < cons.Count)
                                {
                                    lock (selected_agv.conditions_temp)
                                    {
                                        selected_agv.conditions_temp.Clear();
                                        foreach (var v in cons) selected_agv.conditions_temp.Add(v);
                                    }
                                }
                                else
                                {
                                    lock (selected_agv.conditions_temp)
                                    {
                                        selected_agv.conditions_temp.Clear();
                                    }
                                }
                            }
                            else
                            {
                                lock (selected_agv.conditions_temp)
                                {
                                    selected_agv.conditions_temp.Clear();
                                }
                            }
                        }
                    }
                    else
                    {
                        if (new_agv != null)
                        {
                            controller.DoStack();

                            if (controller.is_mousedrag)
                            {
                                var qr = target_floor.mapeditor.HitTest(controller.mousePoint);
                                if (qr != null)
                                {
                                    new_agv.SetLocation("", qr.Location);
                                }
                                else
                                {
                                    new_agv.SetLocation("",  controller.mousePoint);
                                }
                            }
                        }
                        else
                        {
                            if (controller.is_mousedrag)
                            {
                                float move_x = target_floor.pW(controller.mousePoint.X - controller.mousePoint_down.X);
                                float move_y = target_floor.pH(controller.mousePoint.Y - controller.mousePoint_down.Y);

                                controller.draw_offset_pixel.X += move_x;
                                controller.draw_offset_pixel.Y += move_y;

                                ret = true;
                            }
                            else
                            {
                                controller.DoStack();
                            }
                        }
                    }
                }

                return ret;
            }

            public void MouseDown(MouseEventArgs e)
            {
                controller.sw_dragstart.Restart();

                var target_floor = controller.SelectFloor();
                if (target_floor != null)
                {
                    controller.mousePoint_down.X = (int)Math.Round(target_floor.rX(e.Location.X));
                    controller.mousePoint_down.Y = (int)Math.Round(target_floor.rY(e.Location.Y));

                    MouseMove(e);
                    if (is_new_condition)
                    {
                        //    var agv = HitTest(map.mousePoint_down);
                        //    if (agv != null)
                        //    {
                        //        agv.Select(true);
                        //    }
                        //    else if (selected_agv != null)
                        //    {
                        //        selected_agv.Select(false);
                        //    }
                    }
                    else
                    {
                    }
                }
            }

            public void MouseUp(MouseEventArgs e)
            {
                //System.Diagnostics.Trace.WriteLine("MouseUp - " + e.Location.ToString());

                controller.sw_dragstart.Stop();

                var target_floor = controller.SelectFloor();
                if (target_floor != null)
                {
                    if (is_new_condition && controller.is_mousedrag)
                    {
                        if (selected_agv != null)
                        {
                            var cur_qr = target_floor.mapeditor.HitTest(selected_agv.Location);
                            if (cur_qr != null)
                            {
                                FloorQR next_qr = null;
                                FloorQR prev_qr = null;

                                var exist_qr = target_floor.mapeditor.HitTest(controller.mousePoint);
                                if (exist_qr != null)
                                {
                                    controller.mousePoint = exist_qr.Location;
                                }

                                double degree = selected_agv.Degree(controller.mousePoint);

                                if (e.Button == MouseButtons.Left)
                                {
                                    foreach (var kv in cur_qr.next_way)
                                    {
                                        if (kv.Value - 5 < degree && degree < kv.Value + 5)
                                        {
                                            next_qr = kv.Key;
                                            break;
                                        }
                                    }

                                    if (next_qr == null)
                                    {
                                        foreach (var v in cur_qr.prev_way)
                                        {
                                            double degree2 = cur_qr.Degree(v);

                                            if (degree2 - 5 < degree && degree < degree2 + 5)
                                            {
                                                prev_qr = v;
                                                break;
                                            }
                                        }
                                    }

                                    if (next_qr != null)
                                    {
                                        selected_agv.AddNextCondition(next_qr);
                                        selected_agv.SetLocation("", next_qr.Location);
                                    }
                                }
                                else if (e.Button == MouseButtons.Right)
                                {
                                    foreach (var v in cur_qr.prev_way)
                                    {
                                        double degree2 = cur_qr.Degree(v);

                                        if (degree2 - 5 < degree && degree < degree2 + 5)
                                        {
                                            prev_qr = v;
                                            break;
                                        }
                                    }

                                    if (prev_qr != null)
                                    {
                                        selected_agv.RemoveNextCondition(cur_qr);
                                        selected_agv.SetLocation("", prev_qr.Location);
                                    }
                                }
                            }
                        }
                    }
                    else if (!is_new_agv && controller.is_mousedrag && controller.applicate_mode == enApplicateMode.CONDITIONER)
                    {
                        if (new_agv != null)
                        {
                            if (HitTest(controller.mousePoint) == null)
                            {
                                var exist_qr = floor.mapeditor.HitTest(new_agv.Location);
                                if (exist_qr != null)
                                {
                                    AddAGV(new_agv);
                                    new_agv.Select(true);

                                    new_agv = null;
                                    NewAGV(is_new_agv);
                                }
                                else
                                {
                                    new_agv = null;
                                }
                            }
                        }
                    }
                    else if (controller.applicate_mode == enApplicateMode.MODE_SIMULATOR)
                    {
                        var qr = target_floor.mapeditor.HitTest(controller.mousePoint);

                        if (target_floor.controller.EventQrSelected != null) target_floor.controller.EventQrSelected(qr);
                    }
                    else if (controller.applicate_mode == enApplicateMode.AUTO_COMMANDER)
                    {
                        if (e.Button == MouseButtons.Right)
                        {
                            var qr = target_floor.mapeditor.HitTest(controller.mousePoint);

                            if (target_floor.controller.EventQrCommand != null) target_floor.controller.EventQrCommand(qr, e.Location);
                        }
                    }
                }

                controller.is_mousedrag = false;
            }

            public void MouseClick(MouseEventArgs e)
            {
                if (controller.is_mousedrag) return;

                //if (controller.sw_dragstart.IsRunning && 200 < controller.sw_dragstart.ElapsedMilliseconds)
                //{
                //    return;
                //}

                //System.Diagnostics.Trace.WriteLine("MouseClick - " + e.Location.ToString());

                controller.sw_dragstart.Stop();
                controller.is_mousedrag = false;

                var target_floor = controller.SelectFloor();
                if (target_floor != null)
                {
                    if (controller.floorwarp_con != null)
                    {
                        if (target_floor.code != controller.floorwarp_con.FloorCode)
                        {
                            var next_qr = floor.mapeditor.HitTest(controller.mousePoint);

                            if (next_qr != null)
                            {
                                if (controller.floorwarp_con.cur_qr.autorator_id == next_qr.autorator_id)
                                //if (next_qr.Location == map.floorwarp_con.Location)
                                {
                                    var agv = controller.floorwarp_con.owner_agv;
                                    agv.floor = target_floor;

                                    agv.Select(true);

                                    selected_agv.AddNextCondition(next_qr);
                                    selected_agv.SetLocation("", next_qr.Location);

                                    controller.floorwarp_con = null;
                                }
                            }
                        }
                    }
                    else if (is_new_condition && selected_agv != null)
                    {
                        if (0 < selected_agv.conditions_temp.Count)
                        {
                            selected_agv.conditions = AgvRouteManager.ConnectConditions(selected_agv.conditions, selected_agv.conditions_temp);
                            selected_agv.AutoSpeedSet();

                            lock (selected_agv.conditions_temp)
                            {
                                selected_agv.conditions_temp.Clear();
                            }

                            selected_agv.SetLocation("", selected_agv.conditions[selected_agv.conditions.Count - 1].Location);
                            selected_agv.Select(true);
                        }
                    }
                    else if (!is_new_agv && new_agv == null)
                    {
                        //if (is_auto_route && selected_agv != null)
                        //{
                        //    if (0 < selected_agv.conditions_temp.Count)
                        //    {
                        //        selected_agv.mode_conditions[selected_agv.moveMode] = AgvRouteManager.Util.ConnectConditions(selected_agv.mode_conditions[selected_agv.moveMode], selected_agv.conditions_temp);
                        //        selected_agv.AutoSpeedSet();

                        //        selected_agv.Location = selected_agv.mode_conditions[selected_agv.moveMode][selected_agv.mode_conditions[selected_agv.moveMode].Count - 1].Location;
                        //        selected_agv.Select(true);
                        //    }

                        //    //var qr = floor.mapeditor.HitTest(map.mousePoint);
                        //    //if (qr != null)
                        //    //{
                        //    //    AgvRouteManager rm = new AgvRouteManager(map);
                        //    //    var cons = rm.GetAutoRoute(selected_agv, qr);
                        //    //    if (1 < cons.Count)
                        //    //    {
                        //    //        selected_agv.mode_conditions[selected_agv.moveMode] = AgvRouteManager.Util.ConnectConditions(selected_agv.mode_conditions[selected_agv.moveMode], cons);
                        //    //        selected_agv.AutoSpeedSet();
                        //    //        is_auto_route = false;

                        //    //        selected_agv.Location = selected_agv.mode_conditions[selected_agv.moveMode][selected_agv.mode_conditions[selected_agv.moveMode].Count - 1].Location;
                        //    //        selected_agv.Select(true);
                        //    //    }

                        //    //    lock (selected_agv.conditions_temp)
                        //    //    {
                        //    //        selected_agv.conditions_temp.Clear();
                        //    //    }
                        //    //}
                        //}
                        //else
                        {
                            if (e.Button == MouseButtons.Left)
                            {
                                var agv = HitTest(controller.mousePoint);
                                if (agv != null)
                                {
                                    agv.Select(true);
                                }
                                else if (selected_agv != null)
                                {
                                    selected_agv.Select(false);
                                }

                                var qr = floor.mapeditor.HitTest(controller.mousePoint);
                                if (qr != null)
                                {
                                    qr.Select(true);
                                }
                                else if (floor.mapeditor.selected_qr != null)
                                {
                                    floor.mapeditor.selected_qr.Select(false);
                                }
                            }
                        }
                    }
                    else
                    {
                        if (new_agv != null)
                        {
                            var exist_agv = HitTest(new_agv.Location);
                            if (exist_agv == null)
                            {
                                if (e.Button == MouseButtons.Left)
                                {
                                    var exist_qr = floor.mapeditor.HitTest(new_agv.Location);
                                    if (exist_qr != null)
                                    {
                                        AddAGV(new_agv);

                                        new_agv_pre = new_agv;
                                        new_agv = null;

                                        NewAGV(is_new_agv);
                                    }
                                }
                                else if (e.Button == MouseButtons.Right)
                                {
                                    exist_agv = HitTest(controller.mousePoint);
                                    if (exist_agv != null) RemoveAGV(exist_agv);
                                }
                            }
                            else
                            {
                                if (e.Button == MouseButtons.Left)
                                {
                                }
                                else if (e.Button == MouseButtons.Right)
                                {
                                    RemoveAGV(exist_agv);
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region 新規AGV配置

            public void NewAGV(bool is_new_agv)
            {
                var target_floor = controller.SelectFloor();
                if (target_floor != null)
                {
                    this.is_new_agv = is_new_agv;

                    if (is_new_agv)
                    {
                        if (new_agv == null)
                        {
                            new_agv = new FloorAGV(floor, controller.mousePoint.X, controller.mousePoint.Y);
                            new_agv.Select(true);
                        }
                    }
                    else
                    {
                        if (new_agv != null)
                        {
                            new_agv.Select(false);
                            new_agv = null;
                            new_agv_pre = null;
                        }
                    }
                }
            }

            #endregion

            #region 新規AGV動作設定

            public void NewCondition(bool is_new_condition)
            {
                var target_floor = controller.SelectFloor();
                if (target_floor != null)
                {
                    this.is_new_condition = is_new_condition;

                    if (!is_new_condition)
                    {
                        if (new_agv_pre != null && new_agv != null)
                        {
                            //new_agv_pre.RemoveNextCondition(new_agv.on_qr);
                        }

                        if (selected_agv != null)
                        {
                            lock (selected_agv.conditions_temp)
                            {
                                selected_agv.conditions_temp.Clear();
                            }
                        }
                    }
                    else
                    {
                        if (new_agv_pre != null && new_agv != null)
                        {
                            //new_agv_pre.AddNextCondition(new_agv.on_qr);
                        }
                    }
                }
            }

            #endregion
        }
    }
}
