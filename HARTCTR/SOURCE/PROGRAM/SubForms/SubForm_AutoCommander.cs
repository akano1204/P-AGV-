using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BelicsClass.Common;
using BelicsClass.UI;
using BelicsClass.UI.Controls;

namespace PROGRAM
{
    public partial class SubForm_AutoCommander : BelicsClass.UI.BL_SubForm_Base
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
            get
            {
                return this.Text;
            }
        }

        AgvControlManager controller = null;
        private class AgvMoving
        {
            public AgvControlManager.FloorQR qr;
            public AgvControlManager.Rack rack;
            public string rack_face = "";
            public AgvControlManager.enPlaceOperate operate = AgvControlManager.enPlaceOperate.NONE;
        }

        private Dictionary<AgvControlManager.FloorAGV, List<AgvMoving>> listMoving = new Dictionary<AgvControlManager.FloorAGV, List<AgvMoving>>();

        public SubForm_AutoCommander()
        {
            InitializeComponent();

            if (Program.ini_hokusho.Get("SCREEN", "LEFT_PANEL", true))
            {
                buttonLeftPanelOpen_Click(null, null);
            }
            else
            {
                buttonLeftPanelClose_Click(null, null);
            }

            if (Program.ini_hokusho.Get("SCREEN", "RIGHT_PANEL", true))
            {
                buttonAgvConditionsOpen_Click(null, null);
            }
            else
            {
                buttonAgvConditionsClose_Click(null, null);
            }

            this.controller = Program.controller;
            controller.allselect = true;
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            Close();
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            //controller.EventAgvMoved += Map_EventAgvMoved;
            controller.EventAgvSelected += Map_EventAgvSelected;
            panelMap.MouseWheel += PanelMap_MouseWheel;
            controller.EventTrigger += Map_EventTrigger;
            controller.EventQrCommand += Controller_EventQrCommand;
            controller.EventPaint += Controller_EventPaint;
            controller.EventQrSelected += Controller_EventQrSelected;
            //controller.EventAgvStateRefresh += Controller_EventAgvStateRefresh;

            //controller.routeManager.RegisterEvent();

            ListupAgv();
            ListupFloor();
            ListupStations();
            ListupOrders();

            //int interval = 0;
            //int.TryParse(textBoxSleepInterval.Text, out interval);
            //if (interval < 2) interval = 2;

            controller.Start(AgvControlManager.enApplicateMode.AUTO_COMMANDER, 0, 20);

            if (controller.run_manager != null)
            {
                controller.run_manager.RequestReset += Run_manager_RequestReset;
            }

            textBoxZoom.Text = ((int)(controller.draw_scale * 100)).ToString();

            timerClock.Interval = 100;
            timerClock.Enabled = true;
        }

        private void Run_manager_RequestReset(AgvControlManager.AgvRunManager sender)
        {
            MethodInvoker process = (MethodInvoker)delegate ()
            {
                reset = true;
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

        private void Controller_EventQrSelected(AgvControlManager.FloorQR qr)
        {
            if (qr.station_id != "")
            {
                listviewStations.SelectedIndices.Clear();
                
                for (int i = 0; i < listviewStations.Items.Count; i++)
                {
                    AgvControlManager.FloorQR item = listviewStations.Items[i].Tag as AgvControlManager.FloorQR;
                    if (item != null && item.station_id == qr.station_id)
                    {
                        listviewStations.SelectedIndices.Add(i);
                        break;
                    }
                }
            }
        }

        protected override void SubForm_Base_FormClosing(object sender, FormClosingEventArgs e)
        {
            base.SubForm_Base_FormClosing(sender, e);

            //controller.EventAgvMoved -= Map_EventAgvMoved;
            controller.EventAgvSelected -= Map_EventAgvSelected;
            panelMap.MouseWheel -= PanelMap_MouseWheel;
            controller.EventTrigger -= Map_EventTrigger;
            controller.EventQrCommand -= Controller_EventQrCommand;
            controller.EventPaint -= Controller_EventPaint;
            //controller.EventAgvStateRefresh -= Controller_EventAgvStateRefresh;
            //controller.Stop();

            //if (controller.filepath != "")
            //{
            //    controller.Save(controller.filepath);
            //}

            controller.SaveRack();
        }

        private void Controller_EventAgvStateRefresh(AgvControlManager.AgvRunner runner)
        {
            repaint = true;

            //MethodInvoker process = (MethodInvoker)delegate ()
            //{
            //    ListupAgv(runner);
            //};

            //try
            //{
            //    if (InvokeRequired) Invoke(process);
            //    else process.Invoke();
            //}
            //catch (Exception) { }
        }

        private void ListupStations()
        {
            listviewStations.ItemsClear();

            foreach (var v in controller.AllStationQR.OrderBy(e => e.station_id))
            {
                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                item.Tag = v;
                item.Add(v.station_id);
                item.Add(v.QrString);
                listviewStations.Items.Add(item);
            }

            listviewStations.RefreshMe();
        }

        private void ListupOrders()
        {
            if (controller.run_manager == null) return;

            listviewOrders.Items.Clear();
            listviewOrders.NeedRedraw = true;

            foreach (var v in controller.agvs)
            {
                if (v.req != null)
                {
                    AgvController.AgvOrderCommunicator.RequestMove req = v.req as AgvController.AgvOrderCommunicator.RequestMove;
                    if (req != null)
                    {
                        BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                        item.BackColor = Color.Lime;
                        item.Tag = req;
                        item.Add(v);
                        item.Add(req.station);

                        listviewOrders.Items.Add(item);
                    }
                }
            }

            foreach (var v in controller.run_manager.runners)
            {
                foreach (var vv in v.Value.requests)
                {
                    AgvController.AgvOrderCommunicator.RequestMove req = vv as AgvController.AgvOrderCommunicator.RequestMove;
                    if (req != null)
                    {
                        BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                        item.BackColor = Color.Yellow;
                        item.Tag = req;
                        item.Add(v.Key);
                        item.Add(req.station);

                        listviewOrders.Items.Add(item);
                    }
                }
            }

            foreach (var vv in AgvControlManager.AgvRunManager.requests)
            {
                AgvController.AgvOrderCommunicator.RequestMove req = vv as AgvController.AgvOrderCommunicator.RequestMove;
                if (req != null)
                {
                    BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                    item.Tag = req;

                    if (req.agv.Trim() != "")
                    {
                        AgvControlManager.FloorAGV agv = controller.agvs.Where(e => e.id.Trim() == req.agv.Trim()).FirstOrDefault();
                        if (agv != null)
                        {
                            item.Add(agv);
                        }
                        else
                        {
                            item.Add(req.agv.Trim() + "(?)");
                        }
                    }
                    else
                    {
                        item.Add("***");
                    }

                    item.Add(req.station);

                    listviewOrders.Items.Add(item);
                }
            }

            listviewOrders.RefreshMe(false);
        }

        private void ListupAgv()
        {
            ListupAgv(null);
        }

        private void ListupAgv(AgvControlManager.AgvRunner runner)
        {
            var floor = controller.SelectFloor();
            if (floor == null) return;

            listviewAGV.ItemsClear();

            foreach (var v in controller.agvs)
            {
                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();

                item.Add(v);
                item.Add(v);
                item.Add("");
                item.Add("");
                item.Add("");

                if (v.agvRunner != null)
                {
                    item[2] = v.agvRunner.state_string;

                    lock (v.agvRunner.manager)
                    {
                        if (v.agvRunner.cur_con != null)
                        {
                            item[3] = v.agvRunner.cur_con.ToString();
                        }

                        if (v.agvRunner.communicator != null)
                        {
                            item[4] = v.agvRunner.communicator.GetState.ToString();
                        }
                    }
                }

                listviewAGV.Items.Add(item);
            }

            listviewAGV.RefreshMe(false);
        }

        bool repaint = false;
        bool reset = false;

        private void Controller_EventPaint()
        {
            repaint = true;
            //panelMap.Invalidate();
        }

        protected override void timerClock_Tick(object sender, EventArgs e)
        {
            if (0 < m_Mainform.m_MessageBox.Count) return;

            //base.timerClock_Tick(sender, e);
            if (repaint)
            {
                repaint = false;
                panelMap.Invalidate();
                ListupAgv(null);
                ListupOrders();

                if (panelAgvConditions.Tag != null)
                {
                    AgvControlManager.FloorAGV agv = panelAgvConditions.Tag as AgvControlManager.FloorAGV;
                    if (agv != null)
                    {
                        listviewAgvConditions.ItemsClear();
                        if (buttonCurrentConditions.Checked)
                        {
                            foreach (var v in agv.current_route)
                            {
                                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                                item.Add(v);
                                listviewAgvConditions.Items.Add(item);
                            }
                        }
                        else
                        {
                            foreach (var v in agv.conditions)
                            {
                                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                                item.Add(v);
                                listviewAgvConditions.Items.Add(item);
                            }
                        }

                        AgvControlManager.RouteCondition ar_con = null;
                        foreach (var v in agv.conditions)
                        {
                            if (v.cur_qr.autorator_info != null)
                            {
                                ar_con = v;
                                break;
                            }
                        }

                        if (ar_con != null)
                        {
                            if (ar_con.cur_qr.autorator_info.CanExit(agv))
                            {
                                labelAutoratorState.Text = "搬出許可<=" + ar_con.cur_qr.autorator_id;
                            }
                            else if (ar_con.cur_qr.autorator_info.IsEntried(agv))
                            {
                                labelAutoratorState.Text = "搬入完了=>" + ar_con.cur_qr.autorator_id;
                            }
                            else if (ar_con.cur_qr.autorator_info.CanEntry(agv))
                            {
                                labelAutoratorState.Text = "搬入許可<=" + ar_con.cur_qr.autorator_id;
                            }
                            else if (ar_con.cur_qr.autorator_info.IsRequested(agv))
                            {
                                labelAutoratorState.Text = "搬入要求=>" + ar_con.cur_qr.autorator_id;
                            }
                            else
                            {
                                labelAutoratorState.Text = "";
                            }
                        }
                        else
                        {
                            labelAutoratorState.Text = "";
                        }

                        listviewAgvConditions.RefreshMe();
                    }
                }
            }

            if (reset)
            {
                reset = false;
                SubForm_Base_Function07_Clicked(null);
            }
        }

        private void Map_EventAgvMoved(AgvControlManager.FloorAGV agv)
        {
            if (agv != null)
            {
                MethodInvoker process = (MethodInvoker)delegate ()
                {
                    textBoxCurX.Text = ((int)agv.Location.X).ToString();
                    textBoxCurY.Text = ((int)agv.Location.Y).ToString();
                };

                try
                {
                    if (InvokeRequired) Invoke(process);
                    else process.Invoke();
                }
                catch (Exception) { }
            }
        }

        private void Map_EventAgvSelected(AgvControlManager.FloorAGV agv)
        {
            if (agv != null)
            {
                if (agv.selected)
                {
                    textBoxCurX.Text = ((int)agv.Location.X).ToString();
                    textBoxCurY.Text = ((int)agv.Location.Y).ToString();

                    listviewAgvConditions.ItemsClear();
                    if (buttonCurrentConditions.Checked)
                    {
                        foreach (var v in agv.current_route)
                        {
                            BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                            item.Add(v);
                            listviewAgvConditions.Items.Add(item);
                        }
                    }
                    else
                    {
                        foreach (var v in agv.conditions)
                        {
                            BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                            item.Add(v);
                            listviewAgvConditions.Items.Add(item);
                        }
                    }
                    listviewAgvConditions.RefreshMe();

                    AgvControlManager.RouteCondition ar_con = null;
                    foreach (var v in agv.conditions)
                    {
                        if (v.cur_qr.autorator_info != null)
                        {
                            ar_con = v;
                            break;
                        }
                    }

                    if (ar_con != null)
                    {
                        if (ar_con.cur_qr.autorator_info.CanExit(agv))
                        {
                            labelAutoratorState.Text = "搬出許可<=" + ar_con.cur_qr.autorator_id;
                        }
                        else if (ar_con.cur_qr.autorator_info.IsEntried(agv))
                        {
                            labelAutoratorState.Text = "搬入完了=>" + ar_con.cur_qr.autorator_id;
                        }
                        else if (ar_con.cur_qr.autorator_info.CanEntry(agv))
                        {
                            labelAutoratorState.Text = "搬入許可<=" + ar_con.cur_qr.autorator_id;
                        }
                        else if (ar_con.cur_qr.autorator_info.IsRequested(agv))
                        {
                            labelAutoratorState.Text = "搬入要求=>" + ar_con.cur_qr.autorator_id;
                        }
                        else
                        {
                            labelAutoratorState.Text = "";
                        }
                    }
                    else
                    {
                        labelAutoratorState.Text = "";
                    }

                    labelAgvConditions.Text = agv.ToString();
                    panelAgvConditions.Tag = agv;
                    //panelAgvConditions.Visible = true;
                }
                else
                {
                    textBoxCurX.Text = "";
                    textBoxCurY.Text = "";
                    panelAgvConditions.Tag = null;
                    //panelAgvConditions.Visible = false;
                }

                ListupAgv(agv.agvRunner);

                listviewAGV.SelectItem(agv.ToString());
            }
        }

        #region マウス操作

        private void panelMap_Paint(object sender, PaintEventArgs e)
        {
            if (listviewFloor.SelectedIndices.Count == 0) return;

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                if (floor.mapeditor.is_new_qr) floor.mapeditor.NewQR(false);
                if (floor.mapeditor.is_new_way) floor.mapeditor.NewWay(false);
            }

            string code = listviewFloor.SortedItems[listviewFloor.SelectedIndices[0]][1].ToString();
            floor = controller.SelectFloor(code);

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
                if (floor.conditioner.MouseMove(e))
                {
                    panelMap.Invalidate();
                }

                textBoxX.Text = ((int)controller.mousePoint.X).ToString();
                textBoxY.Text = ((int)controller.mousePoint.Y).ToString();

                if (floor.mapeditor.selected_qr == null)
                {
                    AgvControlManager.FloorQR qr = floor.mapeditor.HitTest(controller.mousePoint);
                    if (qr != null)
                    {
                        textBoxCurX.Text = ((int)qr.point.X).ToString();
                        textBoxCurY.Text = ((int)qr.point.Y).ToString();
                    }
                }
                else
                {
                    AgvControlManager.FloorQR qr = floor.mapeditor.selected_qr;
                    if (qr != null)
                    {
                        textBoxCurX.Text = ((int)qr.point.X).ToString();
                        textBoxCurY.Text = ((int)qr.point.Y).ToString();
                    }
                }
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

        #endregion

        private void Map_EventTrigger(AgvControlManager.FloorQR qr, string onoff, string INorOUT)
        {
            MethodInvoker process = (MethodInvoker)delegate ()
            {
            };

            try
            {
                if (InvokeRequired) Invoke(process);
                else process.Invoke();
            }
            catch (Exception) { }
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

        private void listviewFloor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listviewFloor.SelectedIndices.Count == 0) return;

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                if (floor.mapeditor.is_new_qr) floor.mapeditor.NewQR(false);
                if (floor.mapeditor.is_new_way) floor.mapeditor.NewWay(false);
            }

            string code = listviewFloor.SortedItems[listviewFloor.SelectedIndices[0]][1].ToString();
            floor = controller.SelectFloor(code);

            panelMap.Invalidate();
        }

        private void listviewAGV_Click(object sender, EventArgs e)
        {
            if (listviewAGV.SelectedItems.Count <= 0) return;
            AgvControlManager.FloorAGV agv = listviewAGV.SelectedItems[0][0] as AgvControlManager.FloorAGV;
            if (agv == null) return;

            agv.Select(true);
            panelMap.Invalidate();
        }

        protected override void SubForm_Base_Function07_Clicked(object sender)
        {
            base.SubForm_Base_Function07_Clicked(sender);

            string filepath = controller.filepath;
            if (filepath == "")
            {
                OpenFileDialog dlg = new OpenFileDialog();
                dlg.InitialDirectory = Path.GetDirectoryName(Program.ini_agv.FullName);
                dlg.AddExtension = true;
                dlg.Filter = "物流AGV経路設定ファイル (*.LAG)|*.lag|All files (*.*)|*.*";
                if (dlg.ShowDialog(this) == DialogResult.Cancel) return;

                filepath = dlg.FileName;
            }
            else
            {
                //controller.Save(filepath);
                controller.SaveRack();
            }

            controller.Stop();
            //BelicsClass.ProcessManage.BL_ThreadCollector.StopControl_All();

            Program.ini_hokusho.Write("MAP", "OFFSET_X", controller.draw_offset_pixel.X);
            Program.ini_hokusho.Write("MAP", "OFFSET_Y", controller.draw_offset_pixel.Y);
            Program.ini_hokusho.Write("MAP", "SCALE", controller.draw_scale);

            //Program.controller = null;
            Program.controller = new AgvControlManager();

            Cursor = Cursors.WaitCursor;

            Program.controller.Load(filepath);

            Program.controller.Start(AgvControlManager.enApplicateMode.AUTO_COMMANDER, 0, 20);

            Cursor = Cursors.Default;

            SubForm_AutoCommander sub = new SubForm_AutoCommander();
            sub.ShowMe(this);

            Close();
        }

        //private void textBoxSleepInterval_Leave(object sender, EventArgs e)
        //{
        //    if (controller.run_manager != null)
        //    {
        //        int interval = 0;
        //        int.TryParse(textBoxSleepInterval.Text, out interval);

        //        if (1 < interval) controller.run_manager.Interval = interval;
        //    }
        //}

        private void Controller_EventQrCommand(AgvControlManager.FloorQR qr, Point mouse_location)
        {
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                contextMenuStrip1.Tag = qr;
                contextMenuStrip2.Tag = mouse_location;

                if (qr != null && floor.conditioner.selected_agv != null)
                {
                    menuItemGo.Enabled = true;
                    menuItemRackDown.Enabled = true;
                    menuItemRackUp.Enabled = true;
                    menuItemWaitCharge.Enabled = true;
                    menuItemWaitStation.Enabled = true;

                    menuItemRackDown.Visible = true;
                    menuItemRackUp.Visible = true;
                    menuItemWaitCharge.Visible = true;
                    menuItemWaitStation.Visible = true;
                    menuItemGo.Visible = true;

                    if (qr.rack == null)
                    {
                        menuItemRackUp.Visible = false;
                    }

                    menuItemRackDown.Visible = false;
                    if (floor.conditioner.selected_agv != null)
                    {
                        if (floor.conditioner.selected_agv.rack != null)
                        {
                            menuItemRackDown.Visible = true;
                        }
                        else if (listMoving.ContainsKey(floor.conditioner.selected_agv))
                        {
                            if (0 < listMoving[floor.conditioner.selected_agv].Where(e => e.operate == AgvControlManager.enPlaceOperate.RACK_UP).Count()
                                )
                            {
                                menuItemRackDown.Visible = true;
                            }
                        }
                    }

                    menuItemWaitStation.Visible = false;
                    if (qr.station_id != "")
                    {
                        if (floor.conditioner.selected_agv != null)
                        {
                            if (listMoving.ContainsKey(floor.conditioner.selected_agv))
                            {
                                if (floor.conditioner.selected_agv.rack != null ||
                                    0 < listMoving[floor.conditioner.selected_agv].Where(e => e.operate == AgvControlManager.enPlaceOperate.RACK_UP).Count()
                                    )
                                {
                                    menuItemWaitStation.Visible = true;
                                }
                            }
                        }
                    }
                    else
                    {
                        menuItemGo.Visible = false;
                    }

                    if (qr.direction_charge == AgvControlManager.FloorQR.enDirection.NONE)
                    {
                        menuItemWaitCharge.Visible = false;
                    }


                    //if (menuItemRackDown.Visible ||
                    //    menuItemRackUp.Visible ||
                    //    menuItemWaitCharge.Visible ||
                    //    menuItemWaitStation.Visible ||
                    //    menuItemGo.Visible)
                    {
                        contextMenuStrip1.Show(panelMap, mouse_location);
                    }
                }
            }
        }

        #region コンテキストメニュー

        private void menuItemGo_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = contextMenuStrip1.Tag as AgvControlManager.FloorQR;
            if (qr == null) return;
            var floor = controller.SelectFloor();
            if (floor == null) return;
            AgvControlManager.FloorAGV agv = floor.conditioner.selected_agv;
            //if (agv == null) agv = controller.routeManager.GetNearestAGV(qr);
            if (agv == null) return;

            AgvMoving m = new AgvMoving();
            m.qr = qr;
            if (!listMoving.ContainsKey(agv)) listMoving[agv] = new List<AgvMoving>();
            listMoving[agv].Add(m);

            menuItemExecute_Click(sender, e);
        }

        private void menuItemRackUp_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = contextMenuStrip1.Tag as AgvControlManager.FloorQR;
            if (qr == null) return;
            var floor = controller.SelectFloor();
            if (floor == null) return;
            AgvControlManager.FloorAGV agv = floor.conditioner.selected_agv;
            //if (agv == null) agv = controller.routeManager.GetNearestAGV(qr);
            if (agv == null) return;
            //if (agv.rack != null) return;

            AgvMoving m = new AgvMoving();
            m.qr = qr;
            m.rack = qr.rack;
            m.operate = AgvControlManager.enPlaceOperate.RACK_UP;
            if (!listMoving.ContainsKey(agv)) listMoving[agv] = new List<AgvMoving>();
            listMoving[agv].Add(m);


            //var con = controller.routeManager.GetTakeRackConditions(agv, qr);

            //lock (agv.mode_conditions)
            //{
            //    agv.mode_conditions[0].Clear();
            //    agv.mode_conditions[0].AddRange(con);
            //}

            menuItemExecute_Click(sender, e);
        }

        private void menuItemRackDown_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = contextMenuStrip1.Tag as AgvControlManager.FloorQR;
            if (qr == null) return;
            //if (!qr.rack_setable) return;
            var floor = controller.SelectFloor();
            if (floor == null) return;
            AgvControlManager.FloorAGV agv = floor.conditioner.selected_agv;
            //if (agv == null) agv = controller.routeManager.GetNearestAGV(qr);
            if (agv == null) return;

            AgvMoving m = new AgvMoving();
            m.qr = qr;
            m.operate = AgvControlManager.enPlaceOperate.RACK_DOWN;
            if (!listMoving.ContainsKey(agv)) listMoving[agv] = new List<AgvMoving>();
            listMoving[agv].Add(m);

            menuItemExecute_Click(sender, e);
        }

        private void menuItemWaitCharge_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = contextMenuStrip1.Tag as AgvControlManager.FloorQR;
            if (qr == null) return;
            if (qr.direction_charge == AgvControlManager.FloorQR.enDirection.NONE) return;
            var floor = controller.SelectFloor();
            if (floor == null) return;
            AgvControlManager.FloorAGV agv = floor.conditioner.selected_agv;
            //if (agv == null) agv = controller.routeManager.GetNearestAGV(qr);
            if (agv == null) return;
            //if (agv.agvRunner.cur_con != null) return;

            AgvMoving m = new AgvMoving();
            m.qr = qr;
            m.operate = AgvControlManager.enPlaceOperate.CHARGE;
            if (!listMoving.ContainsKey(agv)) listMoving[agv] = new List<AgvMoving>();
            listMoving[agv].Add(m);

            menuItemExecute_Click(sender, e);
        }

        private void menuItemWaitStation_Click(object sender, EventArgs e)
        {
            var floor = controller.SelectFloor();
            if (floor == null) return;
            AgvControlManager.FloorAGV agv = floor.conditioner.selected_agv;
            if (agv == null) return;
            //if (agv.rack == null) return;
            
            menuItemFace1.Enabled = true;
            menuItemFace2.Enabled = true;
            menuItemFace3.Enabled = true;
            menuItemFace4.Enabled = true;
            if (!agv.rack.face_id.ContainsValue("1")) menuItemFace1.Enabled = false;
            if (!agv.rack.face_id.ContainsValue("2")) menuItemFace2.Enabled = false;
            if (!agv.rack.face_id.ContainsValue("3")) menuItemFace3.Enabled = false;
            if (!agv.rack.face_id.ContainsValue("4")) menuItemFace4.Enabled = false;

            contextMenuStrip2.Show(panelMap, (Point)contextMenuStrip2.Tag);
        }

        #region 面指定

        private void menuItemFace1_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = contextMenuStrip1.Tag as AgvControlManager.FloorQR;
            if (qr == null) return;
            var floor = controller.SelectFloor();
            if (floor == null) return;
            AgvControlManager.FloorAGV agv = floor.conditioner.selected_agv;
            if (agv == null) return;
            //if (agv.rack == null) return;

            AgvMoving m = new AgvMoving();
            m.qr = qr;
            m.rack_face = "1";
            m.operate = AgvControlManager.enPlaceOperate.STATION_WAIT;
            if (!listMoving.ContainsKey(agv)) listMoving[agv] = new List<AgvMoving>();
            listMoving[agv].Add(m);


            //controller.routeManager.GetMoveConditions(agv, qr, "1");
        }

        private void menuItemFace2_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = contextMenuStrip1.Tag as AgvControlManager.FloorQR;
            if (qr == null) return;
            var floor = controller.SelectFloor();
            if (floor == null) return;
            AgvControlManager.FloorAGV agv = floor.conditioner.selected_agv;
            if (agv == null) return;
            //if (agv.rack == null) return;

            AgvMoving m = new AgvMoving();
            m.qr = qr;
            m.rack_face = "2";
            m.operate = AgvControlManager.enPlaceOperate.STATION_WAIT;
            if (!listMoving.ContainsKey(agv)) listMoving[agv] = new List<AgvMoving>();
            listMoving[agv].Add(m);

            //controller.routeManager.GetMoveConditions(agv, qr, "2");
        }

        private void menuItemFace3_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = contextMenuStrip1.Tag as AgvControlManager.FloorQR;
            if (qr == null) return;
            var floor = controller.SelectFloor();
            if (floor == null) return;
            AgvControlManager.FloorAGV agv = floor.conditioner.selected_agv;
            if (agv == null) return;
            //if (agv.rack == null) return;

            AgvMoving m = new AgvMoving();
            m.qr = qr;
            m.rack_face = "3";
            m.operate = AgvControlManager.enPlaceOperate.STATION_WAIT;
            if (!listMoving.ContainsKey(agv)) listMoving[agv] = new List<AgvMoving>();
            listMoving[agv].Add(m);

            //controller.routeManager.GetMoveConditions(agv, qr, "3");
        }

        private void menuItemFace4_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = contextMenuStrip1.Tag as AgvControlManager.FloorQR;
            if (qr == null) return;
            var floor = controller.SelectFloor();
            if (floor == null) return;
            AgvControlManager.FloorAGV agv = floor.conditioner.selected_agv;
            if (agv == null) return;
            //if (agv.rack == null) return;

            AgvMoving m = new AgvMoving();
            m.qr = qr;
            m.rack_face = "4";
            m.operate = AgvControlManager.enPlaceOperate.STATION_WAIT;
            if (!listMoving.ContainsKey(agv)) listMoving[agv] = new List<AgvMoving>();
            listMoving[agv].Add(m);

            //controller.routeManager.GetMoveConditions(agv, qr, "4");
        }

        #endregion

        private void menuItemExecute_Click(object sender, EventArgs e)
        {
            foreach (var kv in listMoving)
            {
                var agv = kv.Key;
                AgvControlManager.RouteConditionList con = new AgvControlManager.RouteConditionList();

                PointF location_pre = agv.Location;
                int deg_pre = agv.degree;
                foreach (var m in kv.Value)
                {
                    AgvControlManager.RouteConditionList c = new AgvControlManager.RouteConditionList();
                    if (m.operate == AgvControlManager.enPlaceOperate.RACK_UP)
                    {
                        //c = controller.routeManager.GetTakeRackConditions(agv, m.qr);

                        if (m.qr.rack != null)
                        {
                            AgvController.AgvOrderCommunicator.RequestMove mov = new AgvController.AgvOrderCommunicator.RequestMove();
                            mov.result = "RQ";
                            mov.agv = agv.id;
                            mov.station = m.qr.station_id;
                            mov.rack_action = "1";
                            mov.run_mode = "1";
                            mov.run_music = "1";
                            mov.inner_request = true;
                            lock (agv.agvRunner.requests)
                            {
                                agv.agvRunner.requests.Add(mov);
                            }
                        }
                    }
                    else if (m.operate == AgvControlManager.enPlaceOperate.RACK_DOWN)
                    {
                        //c = controller.routeManager.GetDelivaryRackConditions(agv, m.qr);
                        //if (0 < c.Count())
                        //{
                        //    c.Last().rack_down_arrive = true;
                        //}

                        if (m.qr.rack == null)
                        {
                            AgvController.AgvOrderCommunicator.RequestMove mov = new AgvController.AgvOrderCommunicator.RequestMove();
                            mov.result = "RQ";
                            mov.agv = agv.id;
                            mov.station = m.qr.station_id;
                            mov.rack_action = "2";
                            mov.run_mode = "1";
                            mov.rack_face = "";

                            mov.run_music = "1";
                            mov.inner_request = true;
                            lock (agv.agvRunner.requests)
                            {
                                agv.agvRunner.requests.Add(mov);
                            }
                        }
                    }
                    else if (m.operate == AgvControlManager.enPlaceOperate.STATION_WAIT)
                    {
                        //c = controller.routeManager.GetMoveConditions(agv, m.qr, m.rack_face);
                    }
                    else if (m.operate == AgvControlManager.enPlaceOperate.CHARGE)
                    {
                        //c = controller.routeManager.GetMoveConditions(agv, m.qr);
                        //if (0 < c.Count)
                        //{
                        //    if (c.Last().cur_qr.direction_charge != AgvControlManager.FloorQR.enDirection.NONE) c.Last().wait_charge_trg = true;
                        //}

                        AgvController.AgvOrderCommunicator.RequestMove mov = new AgvController.AgvOrderCommunicator.RequestMove();
                        mov.result = "RQ";
                        mov.agv = agv.id;
                        mov.rack_action = "C";
                        mov.run_mode = "1";
                        mov.run_music = "1";
                        mov.inner_request = true;
                        lock (agv.agvRunner.requests)
                        {
                            agv.agvRunner.requests.Add(mov);
                        }

                    }
                    else if (m.qr.station_id.Trim() != "")
                    {
                        //c = controller.routeManager.GetMoveConditions(agv, m.qr);

                        AgvController.AgvOrderCommunicator.RequestMove mov = new AgvController.AgvOrderCommunicator.RequestMove();
                        mov.result = "RQ";
                        mov.agv = agv.id;
                        mov.station = m.qr.station_id;
                        mov.run_mode = "1";
                        mov.run_music = "1";
                        mov.inner_request = true;
                        lock (agv.agvRunner.requests)
                        {
                            agv.agvRunner.requests.Add(mov);
                        }
                    }

                    //if (0 < c.Count)
                    //{
                    //    if (0 == con.Count)
                    //    {
                    //        con.AddRange(c);
                    //    }
                    //    else
                    //    {
                    //        var cc = AgvControlManager.AgvRouteManager.ConnectConditions(con, c);
                    //        con = cc;
                    //    }

                    //    agv.SetLocation("", c.Last().Location);
                    //    if (c.Last().prev_condition != null)
                    //    {
                    //        agv.degree = (int)agv.Degree(c.Last().prev_condition.Location, c.Last().Location);
                    //    }
                    //}
                }


                //agv.SetLocation("", location_pre);
                //agv.degree= deg_pre;

                //lock (agv)
                //{
                //    agv.mode_conditions[0].Clear();
                //    agv.mode_conditions[0].AddRange(con);

                //    agv.AutoSpeedSet();
                //}
            }
            listMoving.Clear();

            ListupOrders();
        }

        private void menuItemFaceExecute_Click(object sender, EventArgs e)
        {

        }

        #endregion

        private void checkBoxAllFloor_CheckedChanged(object sender, EventArgs e)
        {
            controller.draw_allfloor = checkBoxAllFloor.Checked;
            controller.map.Values.ToList().ForEach(ee => ee.mapeditor.redraw_map = true);
            panelMap.Invalidate();
            repaint = false;
        }

        private void checkBoxAllAgvSelect_CheckedChanged(object sender, EventArgs e)
        {
            controller.allselect = checkBoxAllAgvSelect.Checked;
            panelMap.Invalidate();
            repaint = false;
        }


        //private bool drag = false;
        //private Point dragpoint = new Point();

        //private void labelAgvConditions_MouseDown(object sender, MouseEventArgs e)
        //{
        //    if (e.Button == System.Windows.Forms.MouseButtons.Left)
        //    {
        //        dragpoint = e.Location;
        //        drag = true;
        //    }
        //}

        //private void labelAgvConditions_MouseMove(object sender, MouseEventArgs e)
        //{
        //    if (drag && e.Button == System.Windows.Forms.MouseButtons.Left)
        //    {
        //        int movex = e.X - dragpoint.X;
        //        int movey = e.Y - dragpoint.Y;

        //        Point newlocation = new Point(panelAgvConditions.Location.X + movex, panelAgvConditions.Location.Y + movey);
        //        panelAgvConditions.Location = newlocation;
        //    }
        //}

        //private void labelAgvConditions_MouseLeave(object sender, EventArgs e)
        //{
        //    drag = false;
        //}

        private void buttonCurrentConditions_Click(object sender, EventArgs e)
        {
            repaint = true;
        }

        private void listviewAgvConditions_SelectedIndexChanged(object sender, EventArgs e)
        {
            //if (listviewAgvConditions.SelectedIndices.Count == 0) return;
            if (listviewAgvConditions.SelectedItems.Count == 0) return;

            var con = listviewAgvConditions.SelectedItems[0][0] as AgvControlManager.RouteCondition;
            if (con != null)
            {
                con.cur_qr.Select(true);
                panelMap.Invalidate();
            }
        }

        private void buttonLeftPanelClose_Click(object sender, EventArgs e)
        {
            splitContainer3.Panel1Collapsed = true;
            buttonLeftPanelOpen.Visible = true;

            Program.ini_hokusho.Set("SCREEN", "LEFT_PANEL", false);
        }

        private void buttonLeftPanelOpen_Click(object sender, EventArgs e)
        {
            splitContainer3.Panel1Collapsed = false;
            buttonLeftPanelOpen.Visible = false;

            Program.ini_hokusho.Set("SCREEN", "LEFT_PANEL", true);
        }

        private void buttonAgvConditionsClose_Click(object sender, EventArgs e)
        {
            splitContainer5.Panel2Collapsed = true;
            buttonAgvConditionsOpen.Visible = true;

            Program.ini_hokusho.Set("SCREEN", "RIGHT_PANEL", false);
        }

        private void buttonAgvConditionsOpen_Click(object sender, EventArgs e)
        {
            splitContainer5.Panel2Collapsed = false;
            buttonAgvConditionsOpen.Visible = false;

            Program.ini_hokusho.Set("SCREEN", "RIGHT_PANEL", true);
        }

        private void buttonChargeStart_Click(object sender, EventArgs e)
        {
            if (buttonChargeStart.Checked)
            {
                buttonRackup.Checked = false;
                buttonRackdown.Checked = false;
                buttonChargeStop.Checked = false;
            }
        }

        private void buttonChargeStop_Click(object sender, EventArgs e)
        {
            if (buttonChargeStop.Checked)
            {
                buttonRackup.Checked = false;
                buttonRackdown.Checked = false;
                buttonChargeStart.Checked = false;
            }
        }

        private void buttonRackup_Click(object sender, EventArgs e)
        {
            if (buttonRackup.Checked)
            {
                buttonRackdown.Checked = false;
                buttonChargeStart.Checked = false;
                buttonChargeStop.Checked = false;
            }
        }

        private void buttonRackdown_Click(object sender, EventArgs e)
        {
            if (buttonRackdown.Checked)
            {
                buttonRackup.Checked = false;
                buttonChargeStart.Checked = false;
                buttonChargeStop.Checked = false;
            }
        }

        private void buttonOrderAdd_Click(object sender, EventArgs e)
        {
            if (listviewStations.SelectedIndices.Count == 0) return;

            foreach (int v in listviewStations.SelectedIndices)
            {
                AgvController.AgvOrderCommunicator.RequestMove req = new AgvController.AgvOrderCommunicator.RequestMove();

                AgvControlManager.FloorQR qr = listviewStations.Items[v].Tag as AgvControlManager.FloorQR;
                if (qr == null) continue;

                if (buttonChargeStart.Checked && qr.direction_charge == AgvControlManager.FloorQR.enDirection.NONE)
                {
                    return;
                }

                req.result = "RQ";
                req.agv = controller.selected_agv != null ? controller.selected_agv.id.Trim() : "";
                req.station = qr.station_id.Trim();
                req.run_mode = "1";
                req.run_music = "1";
                req.rack_face = "0";
                req.working = "0";

                if (buttonChargeStart.Checked) req.rack_action = "C";
                else if (buttonChargeStop.Checked) req.rack_action = "c";
                else if (buttonRackup.Checked) req.rack_action = "1";
                else if (buttonRackup.Checked) req.rack_action = "2";
                else req.rack_action = "";

                req.inner_request = true;

                lock (AgvControlManager.AgvRunManager.requests)
                {
                    AgvControlManager.AgvRunManager.requests.Add(req);
                }
            }

            repaint = true;
        }

        private void buttonOrderRemove_Click(object sender, EventArgs e)
        {
            if (listviewOrders.SelectedIndices.Count == 0) return;
            if (controller.run_manager == null) return;

            foreach (int v in listviewOrders.SelectedIndices)
            {
                AgvController.AgvOrderCommunicator.RequestBase req = listviewOrders.Items[v].Tag as AgvController.AgvOrderCommunicator.RequestBase;
                if (req == null) continue;

                lock (AgvControlManager.AgvRunManager.requests)
                {
                    AgvControlManager.AgvRunManager.requests.Remove(req);
                }

                foreach (var vv in controller.run_manager.runners)
                {
                    lock (vv.Value.requests)
                    {
                        vv.Value.requests.Remove(req);
                    }
                }
            }

            repaint = true;
        }
    }
}
