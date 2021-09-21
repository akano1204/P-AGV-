using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using BelicsClass.Common;
using BelicsClass.UI;
using BelicsClass.UI.Controls;

namespace PROGRAM
{
    public partial class SubForm_ModeCommander : BelicsClass.UI.BL_SubForm_Base
    {
        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "", "", "", "", "", "", "[F7]:ﾘｾｯﾄ", "", "", "", "", "[F12]:戻る" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get { return this.Text; }
        }

        public SubForm_ModeCommander()
        {
            InitializeComponent();
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            Close();
        }

        AgvControlManager controller = Program.controller;

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            listviewMode.ItemsClear();
            foreach (var v in AgvControlManager.moveModes.Values)
            {
                //if (v == AgvMapPainter.enMoveMode.NONE) continue;

                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                item.Add(v);
                item.Add(v.name);
                listviewMode.Items.Add(item);
            }
            listviewMode.RefreshMe();

            ListupFloor();
            ListupAgv();

            if (controller.applicate_mode != AgvControlManager.enApplicateMode.MODE_COMMANDER)
            {
                controller.Start(AgvControlManager.enApplicateMode.MODE_COMMANDER, 0, 20);
            }

            controller.EventQrSelected += Map_EventSelected;
            controller.EventAgvSelected += Map_EventAgvSelected;
            panelMap.MouseWheel += PanelMap_MouseWheel;

            foreach (var runner in controller.run_manager.runners)
            {
                AgvControlManager.AgvModeCommander commander = runner.Value as AgvControlManager.AgvModeCommander;
                if (commander != null)
                {
                    commander.EventErrorStop += Commander_EventErrorStop;
                }
            }

            timerMover.Enabled = true;

            textBoxZoom.Text = ((int)(controller.draw_scale * 100)).ToString();
        }

        private void Commander_EventErrorStop(object sender, string description)
        {
            MethodInvoker process = (MethodInvoker)delegate ()
            {
                BL_MessageBox.Show(this, description, "エラー");
            };

            try
            {
                if (InvokeRequired) Invoke(process);
                else process.Invoke();
            }
            //catch (ObjectDisposedException) { }
            //catch (InvalidOperationException) { }
            catch (Exception) { }
        }

        private void ListupAgv()
        {
            listviewAGV.ItemsClear();
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                listviewAGV.ItemsClear();
                foreach (var v in controller.agvs)
                {
                    BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                    item.Add(v);
                    item.Add(v);
                    item.Add("");
                    item.Add("");
                    item.Tag = v;
                    listviewAGV.Items.Add(item);
                }
            }

            listviewAGV.RefreshMe();
        }

        protected override void SubForm_Base_FormClosing(object sender, FormClosingEventArgs e)
        {
            base.SubForm_Base_FormClosing(sender, e);

            if (controller.run_manager != null)
            {
                foreach (var runner in controller.run_manager.runners)
                {
                    AgvControlManager.AgvModeCommander commander = runner.Value as AgvControlManager.AgvModeCommander;
                    if (commander != null)
                    {
                        commander.EventErrorStop -= Commander_EventErrorStop;
                    }
                }
            }

            controller.EventQrSelected -= Map_EventSelected;
            controller.EventAgvSelected -= Map_EventAgvSelected;
            panelMap.MouseWheel -= PanelMap_MouseWheel;

            //foreach (var f in map.map)
            //{
            //    var floor = f.Value;
            //    map.Stop();
            //}
        }

        private void ListupFloor()
        {
            listviewFloor.Items.Clear();
            listviewFloor.NeedRedraw = true;

            int select_index = -1;
            int count = 0;
            foreach (var v in controller.map)
            {
                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem(v.Value);
                item.Add(v.Value.code);
                item.Add(v.Value.mapeditor.Count);
                item.Add("");
                listviewFloor.Items.Add(item);

                if (v.Value.code == controller.selected_floor)
                {
                    select_index = count;
                }

                count++;
            }
            listviewFloor.RefreshMe();
            if (0 <= select_index) listviewFloor.SelectedIndices.Add(select_index);
        }

        private void panelMap_Paint(object sender, PaintEventArgs e)
        {
            controller.draw_size_pixel.Width = panelMap.Width;
            controller.draw_size_pixel.Height = panelMap.Height;

            controller.Draw(e.Graphics);
        }

        private void panelMap_MouseDown(object sender, MouseEventArgs e)
        {
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.conditioner.MouseDown(e);
                panelMap.Invalidate();
            }
        }

        private void panelMap_MouseMove(object sender, MouseEventArgs e)
        {
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.conditioner.MouseMove(e);
                panelMap.Invalidate();

                textBoxX.Text = ((int)controller.mousePoint.X).ToString();
                textBoxY.Text = ((int)controller.mousePoint.Y).ToString();
            }
        }

        private void panelMap_MouseUp(object sender, MouseEventArgs e)
        {
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.conditioner.MouseUp(e);
                panelMap.Invalidate();
            }
        }

        private void panelMap_MouseClick(object sender, MouseEventArgs e)
        {
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.conditioner.MouseClick(e);
                panelMap.Invalidate();
            }
        }

        private void PanelMap_MouseWheel(object sender, MouseEventArgs e)
        {
            controller.MouseWheel(e);
            panelMap.Invalidate();

            textBoxZoom.Text = ((int)(controller.draw_scale * 100)).ToString();
        }

        private void textBoxZoom_Leave(object sender, EventArgs e)
        {
            float scale = 0;
            if (float.TryParse(textBoxZoom.Text, out scale))
            {
                if (10 <= scale && scale <= 400)
                {
                    controller.Scale(scale / 100, new PointF(panelMap.Width / 2, panelMap.Height / 2));
                    panelMap.Invalidate();
                }
            }
        }

        //AgvMapPainter.enMoveMode mode_pre = AgvMapPainter.enMoveMode.NONE;

        private void timerMover_Tick(object sender, EventArgs e)
        {
            if (0 < m_Mainform.m_MessageBox.Count) return;

            var floor = controller.SelectFloor();
            if (floor == null) return;

            foreach (var v in listviewAGV.SortedItems)
            {
                var agv = v[0] as AgvControlManager.FloorAGV;
                if (agv.agvRunner != null)
                {
                    v[2] = "[" + agv.agvRunner.Step.ToString() + "] " + agv.agvRunner.state_string;
                    v[3] = "";
                    if (typeof(AgvControlManager.AgvModeCommander).IsInstanceOfType(agv.agvRunner))
                    {
                        v[3] = ((AgvControlManager.AgvModeCommander)agv.agvRunner).communicator.GetState.ToString();
                    }
                    else if (agv.agvRunner.cur_con != null)
                    {
                        v[3] = agv.agvRunner.cur_con.ToString();
                    }
                }
            }
            listviewAGV.RefreshMe(false);


            BL_VirtualListView.BL_VirtualListViewItem selected = null;

            if (0 < listviewMode.SelectedIndices.Count)
            {
                selected = listviewMode.SortedItems[listviewMode.SelectedIndices[0]];
            }

            //foreach (var v in listviewMode.SortedItems)
            //{
            //    if ((AgvMapPainter.enMoveMode)v[0] == floor.conditioner.moveMode)
            //    {
            //        if (mode_pre != floor.conditioner.moveMode)
            //        {
            //            if (mode_pre != AgvMapPainter.enMoveMode.RECOVERY_CHARGE)
            //            {
            //                if (floor.conditioner.moveMode == AgvMapPainter.enMoveMode.NONE)
            //                {
            //                    bool canreset = true;
            //                    foreach (var vv in floor.conditioner.agvs)
            //                    {
            //                        if (vv.agvRunner != null)
            //                        {
            //                            if (!vv.agvRunner.complete)
            //                            {
            //                                canreset = false;
            //                                break;
            //                            }
            //                        }
            //                    }

            //                    if (canreset)
            //                    {
            //                        buttonOpen_Click(null, null);
            //                        return;
            //                    }
            //                }
            //            }

            //            mode_pre = floor.conditioner.moveMode;
            //        }

            //        if (selected != v)
            //        {
            //            listviewMode.SelectItem(v);
            //        }
            //        break;
            //    }
            //}

            if (0 < listviewAGV.SelectedItems.Count)
            {
                AgvControlManager.FloorAGV agv = listviewAGV.SelectedItems[0].Tag as AgvControlManager.FloorAGV;

                if (agv != null)
                {
                    foreach (var v in listviewConditions.SortedItems)
                    {
                        if (v.Tag == agv.agvRunner.cur_con)
                        {
                            listviewConditions.SelectedIndices.Clear();
                            listviewConditions.SelectItem(v);
                            break;
                        }
                    }
                }
            }


            panelMap.Invalidate();
        }

        private void listviewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!checkBoxManageSim.Checked) return;

            if (0 < listviewMode.SelectedIndices.Count)
            {
                var floor = controller.SelectFloor();
                if (floor != null)
                {
                    AgvControlManager.MoveMode mode = listviewMode.SortedItems[listviewMode.SelectedIndices[0]][0] as AgvControlManager.MoveMode;
                    if (mode != null)
                    {
                        controller.SetMoveMode(mode.mode);
                    }
                }
                panelMap.Invalidate();
            }
        }

        private void checkBoxManageSim_CheckedChanged(object sender, EventArgs e)
        {
            checkedListBoxTrigger.Enabled = checkBoxManageSim.Checked;

            if (checkedListBoxTrigger.Enabled)
            {
                ListupTrigger();
            }
            else
            {
                checkedListBoxTrigger.Items.Clear();
            }
        }

        public class TriggerQR
        {
            public AgvControlManager.FloorQR qr = null;
            public string mode = "";
            public bool onoff = false;

            public override string ToString()
            {
                string ret = "";

                ret = "[" + (onoff ? "ON" : "OF") + "]";

                if (qr.autorator_id != "")
                {
                    ret += "AT[" + qr.autorator_id + "][" + mode + "]";
                }
                else if (qr.direction_station != AgvControlManager.FloorQR.enDirection.NONE)
                {
                    ret += "ST[" + qr.station_id + "]";
                }
                else if (qr.direction_charge != AgvControlManager.FloorQR.enDirection.NONE)
                {
                    ret += "C";
                }

                ret += "[" + qr.ToString() + "]";

                return ret;
            }
        }

        private void ListupTrigger()
        {
            checkedListBoxTrigger.Items.Clear();

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                foreach (var kvx in floor.mapeditor.qrs)
                {
                    foreach (var kvy in kvx.Value)
                    {
                        var qr = kvy.Value;

                        if (qr.autorator_id != "")
                        {
                            TriggerQR tq = new TriggerQR();
                            tq.qr = qr;
                            tq.mode = "IN";
                            tq.onoff = false;
                            checkedListBoxTrigger.Items.Add(tq);

                            tq = new TriggerQR();
                            tq.qr = qr;
                            tq.mode = "OUT";
                            tq.onoff = false;
                            checkedListBoxTrigger.Items.Add(tq);
                        }
                        else if (qr.direction_station != AgvControlManager.FloorQR.enDirection.NONE)
                        {
                            TriggerQR tq = new TriggerQR();
                            tq.qr = qr;
                            tq.mode = "";
                            tq.onoff = false;
                            checkedListBoxTrigger.Items.Add(tq);

                        }
                        else if (qr.direction_charge != AgvControlManager.FloorQR.enDirection.NONE)
                        {
                            TriggerQR tq = new TriggerQR();
                            tq.qr = qr;
                            tq.mode = "";
                            tq.onoff = false;
                            checkedListBoxTrigger.Items.Add(tq);
                        }
                    }
                }
            }
        }

        private void checkedListBoxTrigger_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            TriggerQR qr = checkedListBoxTrigger.Items[e.Index] as TriggerQR;
            if (qr != null)
            {
                if (e.NewValue == CheckState.Checked)
                {
                    qr.onoff = true;
                }
                else
                {
                    qr.onoff = false;
                }

                qr.qr.Trigger(qr.onoff ? qr.qr.floor.code : "", qr.mode);
            }
        }

        private void Map_EventAgvSelected(AgvControlManager.FloorAGV agv)
        {
            if (agv != null)
            {
                if (agv.selected)
                {
                    textBoxAgvID.Text = agv.id;
                    textBoxAgvIP.Text = Program.ini_agv.Get(agv.id, "IP", "");
                    textBoxAgvHost.Text = Program.ini_agv.Get(agv.id, "REMOTE_HOST", "");
                    textBoxAgvClient.Text = Program.ini_agv.Get(agv.id, "REMOTE_CLIENT", "");
                }
                else
                {
                    textBoxAgvID.Text = "";
                    textBoxAgvIP.Text = "";
                    textBoxAgvHost.Text = "";
                    textBoxAgvClient.Text = "";
                }

                BL_VirtualListView.BL_VirtualListViewItem selectitem = null;
                {
                    listviewAGV.ItemsClear();
                    foreach (var v in controller.agvs)
                    {
                        BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                        item.Add(v);
                        item.Add(v);
                        item.Add("");
                        item.Add("");
                        item.Tag = v;
                        listviewAGV.Items.Add(item);

                        if (v == agv) selectitem = item;
                    }
                    listviewAGV.RefreshMe();
                    if (selectitem != null) listviewAGV.SelectItem(selectitem);
                }

                {
                    listviewConditions.ItemsClear();
                    selectitem = null;
                    if (agv.mode_conditions[agv.moveMode] != null)
                    {
                        int no = 1;
                        foreach (var v in agv.mode_conditions[agv.moveMode])
                        {
                            BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                            item.Add(no);
                            item.Add(v);
                            item.Tag = v;
                            listviewConditions.Items.Add(item);
                            no++;

                            if (agv.Location == v.Location) selectitem = item;
                            //if (selectitem == null && agv.Location == v.Location) selectitem = item;
                        }
                    }
                    listviewConditions.RefreshMe();
                    if (selectitem != null) listviewConditions.SelectItem(selectitem);
                }

                {
                    AgvControlManager.RouteCondition con = null;
                    if (selectitem != null)
                    {
                        con = selectitem.Tag as AgvControlManager.RouteCondition;
                    }
                }
            }
            else
            {
                listviewConditions.ItemsClear();
                listviewConditions.RefreshMe();
            }
        }

        private void listviewAGV_Click(object sender, EventArgs e)
        {
            if (listviewAGV.SelectedIndices.Count <= 0) return;

            AgvControlManager.FloorAGV agv = listviewAGV.SortedItems[listviewAGV.SelectedIndices[0]].Tag as AgvControlManager.FloorAGV;
            if (agv == null) return;

            agv.Select(true);
            panelMap.Invalidate();
        }

        #region AGV手動指示


        protected override void SubForm_Base_Function03_Clicked(object sender)
        {
            base.SubForm_Base_Function03_Clicked(sender);

            if (listviewAGV.SelectedIndices.Count <= 0) return;

            AgvControlManager.FloorAGV agv = listviewAGV.SortedItems[listviewAGV.SelectedIndices[0]].Tag as AgvControlManager.FloorAGV;
            if (agv == null) return;

            AgvControlManager.AgvModeCommander commander = agv.agvRunner as AgvControlManager.AgvModeCommander;
            if (commander == null) return;

            AgvController.AgvCommunicator.Order order = new AgvController.AgvCommunicator.Order();
            order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.ROUTE_CANCEL;
            order.x = (int)agv.Location.X;
            order.y = (int)agv.Location.Y;
            commander.communicator.SetOrder(new AgvController.AgvCommunicator.Order[] { order });
        }

        private void Map_EventSelected(AgvControlManager.FloorQR qr)
        {
            if (qr != null)
            {
                textBoxCurX.Text = ((int)qr.Location.X).ToString();
                textBoxCurY.Text = ((int)qr.Location.Y).ToString();
            }
        }

        private void buttonMove_Click(object sender, EventArgs e)
        {
            if (listviewAGV.SelectedIndices.Count <= 0) return;

            AgvControlManager.FloorAGV agv = listviewAGV.SortedItems[listviewAGV.SelectedIndices[0]].Tag as AgvControlManager.FloorAGV;
            if (agv == null) return;

            AgvControlManager.AgvModeCommander commander = agv.agvRunner as AgvControlManager.AgvModeCommander;
            if (commander == null) return;

            //if (!commander.request) return;

            int x = 0; int.TryParse(textBoxCurX.Text, out x);
            int y = 0; int.TryParse(textBoxCurY.Text, out y);

            AgvControlManager.FloorQR qr = agv.floor.mapeditor.Exist(new PointF(x, y));
            if (qr == null) return;

            List<AgvController.AgvCommunicator.Order> orders = new List<AgvController.AgvCommunicator.Order>();
            AgvController.AgvCommunicator.Order order = null;

            double agv_deg = agv.Degree(new PointF(x, y));
            double rack_deg = commander.communicator.GetState.rack_deg;

            if (commander.communicator.GetState.deg != agv_deg)
            {
                order = new AgvController.AgvCommunicator.Order();
                order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                order.x = (int)agv.Location.X;
                order.y = (int)agv.Location.Y;
                order.deg = (short)((agv_deg + 180) % 360 - 180);
                if (order.deg == 270) order.deg = -90;
                if (order.deg == -180) order.deg = 180;
                order.mod_agv_rorate = true;

                if (commander.communicator.GetState.sta_rack)
                {
                    order.mod_rack_rorate = true;
                    order.rack_deg = (short)rack_deg;
                }
                orders.Add(order);
            }

            {
                order = new AgvController.AgvCommunicator.Order();
                order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                order.x = (int)agv.Location.X;
                order.y = (int)agv.Location.Y;
                order.mod_speed_0 = true;
                order.mod_speed_1 = true;
                order.mod_speed_2 = false;


                orders.Add(order);
            }

            if (!commander.communicator.GetState.sta_rack && agv.rack == null && qr.rack != null && checkNextRackUp.Checked)
            {
                order = new AgvController.AgvCommunicator.Order();
                order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                order.x = (int)qr.x;
                order.y = (int)qr.y;
                order.deg = (short)((agv_deg + 180) % 360 - 90);
                if (order.deg == 270) order.deg = -90;
                if (order.deg == -180) order.deg = 180;
                order.mod_agv_rorate = true;
                orders.Add(order);

                order = new AgvController.AgvCommunicator.Order();
                order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                order.x = (int)qr.x;
                order.y = (int)qr.y;
                order.mod_rack_up = true;
                orders.Add(order);

                order = new AgvController.AgvCommunicator.Order();
                order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                order.x = (int)qr.x;
                order.y = (int)qr.y;
                order.mod_rack_up = true;
                orders.Add(order);

                order = new AgvController.AgvCommunicator.Order();
                order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                order.x = (int)qr.x;
                order.y = (int)qr.y;
                order.deg = (short)((agv_deg + 90) % 360 - 180);
                if (order.deg == 270) order.deg = -90;
                if (order.deg == -180) order.deg = 180;
                order.mod_agv_rorate = true;
                order.mod_rack_rorate = true;
                order.rack_deg = (short)commander.DegreeRack(qr.rack.degree);
                orders.Add(order);
            }
            else
            {
                order = new AgvController.AgvCommunicator.Order();
                order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
                order.x = (int)qr.x;
                order.y = (int)qr.y;
                orders.Add(order);
            }

            commander.communicator.SetOrder(orders.ToArray());
        }

        protected override void SubForm_Base_Function04_Clicked(object sender)
        {
            base.SubForm_Base_Function04_Clicked(sender);

            if (listviewAGV.SelectedIndices.Count <= 0) return;

            AgvControlManager.FloorAGV agv = listviewAGV.SortedItems[listviewAGV.SelectedIndices[0]].Tag as AgvControlManager.FloorAGV;
            if (agv == null) return;

            AgvControlManager.AgvModeCommander commander = agv.agvRunner as AgvControlManager.AgvModeCommander;
            if (commander == null) return;

            AgvController.AgvCommunicator.Order order = new AgvController.AgvCommunicator.Order();
            order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.CHARGE_STOP;
            order.x = (int)agv.Location.X;
            order.y = (int)agv.Location.Y;
            commander.communicator.SetOrder(new AgvController.AgvCommunicator.Order[] { order });
        }

        private void buttonRackDown_Click(object sender, EventArgs e)
        {
            if (listviewAGV.SelectedIndices.Count <= 0) return;

            AgvControlManager.FloorAGV agv = listviewAGV.SortedItems[listviewAGV.SelectedIndices[0]].Tag as AgvControlManager.FloorAGV;
            if (agv == null) return;

            AgvControlManager.AgvModeCommander commander = agv.agvRunner as AgvControlManager.AgvModeCommander;
            if (commander == null) return;

            if (!commander.communicator.GetState.sta_rack) return;

            AgvController.AgvCommunicator.Order order = new AgvController.AgvCommunicator.Order();

            order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
            order.x = (int)agv.Location.X;
            order.y = (int)agv.Location.Y;
            order.mod_rack_down = true;

            commander.communicator.SetOrder(new AgvController.AgvCommunicator.Order[] { order });
        }

        private void buttonRackRotate_Click(object sender, EventArgs e)
        {
            if (listviewAGV.SelectedIndices.Count <= 0) return;

            AgvControlManager.FloorAGV agv = listviewAGV.SortedItems[listviewAGV.SelectedIndices[0]].Tag as AgvControlManager.FloorAGV;
            if (agv == null) return;

            AgvControlManager.AgvModeCommander commander = agv.agvRunner as AgvControlManager.AgvModeCommander;
            if (commander == null) return;

            if (!commander.communicator.GetState.sta_rack) return;

            AgvController.AgvCommunicator.Order order = new AgvController.AgvCommunicator.Order();
            if (!short.TryParse(textBoxRackDegree.Text, out order.rack_deg)) return;

            order.cmd = (ushort)AgvController.AgvCommunicator.Order.CMD.OPERATE;
            order.x = (int)agv.Location.X;
            order.y = (int)agv.Location.Y;
            order.mod_rack_rorate = true;

            commander.communicator.SetOrder(new AgvController.AgvCommunicator.Order[] { order });
        }

        #endregion

        private void listviewFloor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listviewFloor.SelectedIndices.Count == 0) return;

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                if (floor.mapeditor.is_new_qr) floor.mapeditor.NewQR(false);
                if (floor.mapeditor.is_new_way) floor.mapeditor.NewWay(false);
                if (floor.conditioner.is_new_agv) floor.conditioner.NewAGV(false);
                if (floor.conditioner.is_new_condition) floor.conditioner.NewCondition(false);
            }

            string code = listviewFloor.SortedItems[listviewFloor.SelectedIndices[0]][1].ToString();
            floor = controller.SelectFloor(code);

            panelMap.Invalidate();

            ListupAgv();
            ListupTrigger();
        }

        protected override void SubForm_Base_Function07_Clicked(object sender)
        {
            base.SubForm_Base_Function07_Clicked(sender);

            Cursor = Cursors.WaitCursor;

            
            controller.Stop();

            if (controller.filepath.Trim() == "")
            {
                Cursor = Cursors.Default;

                OpenFileDialog dlg = new OpenFileDialog();
                dlg.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                dlg.AddExtension = true;
                dlg.Filter = "物流AGV経路設定ファイル (*.LAG)|*.lag|All files (*.*)|*.*";
                if (dlg.ShowDialog(this) == DialogResult.Cancel) return;

                Cursor = Cursors.WaitCursor;
                controller.filepath = dlg.FileName;
            }

            if (!File.Exists(controller.filepath))
            {
                Cursor = Cursors.Default;
                return;
            }

            controller.Load(controller.filepath);

            BL_SubForm_Base sub = new SubForm_ModeCommander();
            sub.ShowMe(this);

            Cursor = Cursors.Default;
            Close();
        }
    }
}
