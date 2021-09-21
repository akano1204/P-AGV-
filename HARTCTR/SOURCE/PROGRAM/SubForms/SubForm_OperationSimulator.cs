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
    public partial class SubForm_OperationSimulator : BelicsClass.UI.BL_SubForm_Base
    {
        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "[F1]:ﾄﾘｶﾞｰ", "[F2]:状態保持", "", "", "", "[F6]:棚要求", "[F7]:読込", "", "", "", "", "[F12]:戻る" };
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

        public SubForm_OperationSimulator()
        {
            InitializeComponent();

            this.controller = Program.controller;
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            controller.Stop();

            //controller.EventAgvMoved -= Map_EventAgvMoved;
            controller.EventAgvSelected -= Map_EventAgvSelected;
            panelMap.MouseWheel -= PanelMap_MouseWheel;
            controller.EventTrigger -= Map_EventTrigger;
            timerMover.Stop();

            Close();
        }

        protected override void SubForm_Base_Function01_Clicked(object sender)
        {
            base.SubForm_Base_Function01_Clicked(sender);

            if (listviewTrg.SelectedIndices.Count == 0) return;

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                var item = listviewTrg.SortedItems[listviewTrg.SelectedIndices[0]];
                AgvControlManager.FloorQR qr = listviewTrg.SortedItems[listviewTrg.SelectedIndices[0]].Tag as AgvControlManager.FloorQR;
                if (qr != null)
                {
                    qr.Trigger(buttonTrg.Checked ? qr.floor.code : "", item[0].ToString());
                }
            }

            buttonTrg.Text = buttonTrg.Checked ? "[F1] トリガーON" : "[F1] トリガーOFF";
        }

        protected override void SubForm_Base_Function02_Clicked(object sender)
        {
            base.SubForm_Base_Function02_Clicked(sender);

            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.conditioner.keep_state = button.Checked;
            }
        }

        //protected override void SubForm_Base_Function05_Clicked(object sender)
        //{
        //    base.SubForm_Base_Function05_Clicked(sender);

        //    var floor = controller.SelectFloor();
        //    if (floor != null)
        //    {
        //        if (buttonStart.Checked)
        //        {
        //            m_Mainform.btnFunctions[7].Enabled = false;

        //            AgvControlManager.MoveMode mode = AgvControlManager.moveModes[0];

        //            int interval = 0;
        //            int.TryParse(textBoxSleepInterval.Text, out interval);
        //            if (interval < 2) interval = 2;
        //            controller.Start(AgvControlManager.enApplicateMode.OPERATION_SIMULATOR, 0, interval);
        //            timerMover.Start();
        //        }
        //        else
        //        {
        //            controller.Stop();
        //            timerMover.Stop();

        //            m_Mainform.btnFunctions[7].Enabled = true;
        //        }

        //        panelMap.Invalidate();
        //    }
        //}

        protected override void SubForm_Base_Function06_Clicked(object sender)
        {
            base.SubForm_Base_Function06_Clicked(sender);

            if (listview_station.SelectedItems.Count == 0 || listview_rack.SelectedItems.Count == 0 || listviewRackFace.SelectedItems.Count == 0) return;
            AgvControlManager.FloorQR stQr = listview_station.SelectedItems[0].Tag as AgvControlManager.FloorQR;

            AgvControlManager.Rack rack = listview_rack.SelectedItems[0].Tag as AgvControlManager.Rack;
            string faceID = listviewRackFace.SelectedItems[0][1] as string;

            //controller.routeManager.SetRequest(stQr, rack, faceID);

            requestList.Items.Clear();
            /*
            foreach (var v in controller.routeManager.requests)
            {
                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                item.Add("");
                item.Add(v.station.station_id);
                item.Add(v.rack.rack_id);
                item.Add(v.faceID);
                requestList.Items.Add(item);
            }
            */
            requestList.RefreshMe(false);
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            AttachButton_to_Functions(buttonTrg, 1);
            AttachButton_to_Functions(buttonKeepState, 2);
            AttachButton_to_Functions(buttonStart, 5);

            //controller.EventAgvMoved += Map_EventAgvMoved;
            controller.EventAgvSelected += Map_EventAgvSelected;
            panelMap.MouseWheel += PanelMap_MouseWheel;
            controller.EventTrigger += Map_EventTrigger;

            //controller.routeManager.RegisterEvent();

            listviewAGV.ItemsClear();
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                foreach (var v in controller.agvs)
                {
                    v.SetLocation("", v.Location);

                    BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                    item.Add(v);
                    item.Add(v);
                    item.Add("");
                    item.Add("");
                    item.Add("");
                    listviewAGV.Items.Add(item);
                }
            }
            listviewAGV.RefreshMe();

            ListupFloor();
            ListupTrigger();
            ListupStation();
            ListupRack();

            int interval = 0;
            int.TryParse(textBoxSleepInterval.Text, out interval);
            if (interval < 2) interval = 2;
            controller.Start(AgvControlManager.enApplicateMode.OPERATION_SIMULATOR, 0, interval);
            timerMover.Start();
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
                }
                else
                {
                    textBoxCurX.Text = "";
                    textBoxCurY.Text = "";
                }

                listviewAGV.ItemsClear();
                BL_VirtualListView.BL_VirtualListViewItem selectitem = null;
                foreach (var v in controller.agvs)
                {
                    BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                    item.Add(v);
                    item.Add(v);
                    item.Add("");
                    item.Add("");
                    item.Add("");
                    listviewAGV.Items.Add(item);

                    if (v == agv) selectitem = item;
                }
                listviewAGV.RefreshMe();

                if (selectitem != null) listviewAGV.SelectItem(selectitem);
            }
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

        BL_Stopwatch sw = new BL_Stopwatch();

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
                    v[2] = agv.agvRunner.state_string;
                    v[3] = "";
                    if (agv.agvRunner.cur_con != null)
                    {
                        v[3] = agv.agvRunner.cur_con.ToString();
                    }
                }
                //v[4] = controller.routeManager.GetAgvStatusString(agv);
            }
            listviewAGV.RefreshMe(false);

            requestList.Items.Clear();
            /*
            foreach (var v in controller.routeManager.requests)
            {
                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                item.Add("");
                item.Add(v.station != null ? v.station.station_id : "None");
                item.Add(v.rack != null ? v.rack.rack_id : "None");
                item.Add(v.faceID);
                requestList.Items.Add(item);
            }
            */
            requestList.RefreshMe(false);
            

            /*
            foreach (var agv in controller.agvs)
            {
                controller.routeManager.UpdateStatus(agv);
            }
            controller.routeManager.UpdateStatus();
            */

            panelMap.Invalidate();
        }

        private void Map_EventTrigger(AgvControlManager.FloorQR qr, string onoff, string INorOUT)
        {
            MethodInvoker process = (MethodInvoker)delegate ()
            {
                foreach (var item in listviewTrg.SortedItems)
                {
                    if (item.Tag == qr)
                    {
                        if (item[0].ToString() == INorOUT || item[0].ToString() == "")
                        {
                            item[2] = onoff;
                            buttonTrg.Checked = item[2].ToString() != "" ? true : false;
                            buttonTrg.Text = item[2].ToString() != "" ? "[F1] トリガーON" : "[F1] トリガーOFF";
                        }
                    }
                }

                listviewTrg.RefreshMe(false);
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

            listviewFloor.Items.Sort((a, b) => string.Compare(a[0].ToString(), b[0].ToString()));
            listviewFloor.RefreshMe();
            if (0 <= select_index) listviewFloor.SelectedIndices.Add(select_index);
        }

        private void ListupTrigger()
        {
            listviewTrg.Items.Clear();

            //var floor = map.SelectFloor();

            foreach (var kv in controller.map)
            {
                var floor = kv.Value;
                var floorId = kv.Key;

                if (floor != null)
                {
                    foreach (var kvx in floor.mapeditor.qrs)
                    {
                        foreach (var kvy in kvx.Value)
                        {
                            BL_VirtualListView.BL_VirtualListViewItem item;
                            var qr = kvy.Value;

                            if (qr.autorator_id != "")
                            {
                                item = new BL_VirtualListView.BL_VirtualListViewItem();
                                item.Tag = qr;
                                item.Add("IN");
                                item.Add("AT-IN [" + qr.autorator_id + "] [" + floorId + "]");
                                item.Add(qr.autoratorIN_trigger);
                                listviewTrg.Items.Add(item);
                                listviewTrg.NeedRedraw = true;

                                item = new BL_VirtualListView.BL_VirtualListViewItem();
                                item.Tag = qr;
                                item.Add("OUT");
                                item.Add("AT-OUT [" + qr.autorator_id + "] [" + floorId + "]");
                                item.Add(qr.autoratorIN_trigger);
                                listviewTrg.Items.Add(item);
                                listviewTrg.NeedRedraw = true;
                            }
                            else if (qr.direction_station != AgvControlManager.FloorQR.enDirection.NONE)
                            {
                                item = new BL_VirtualListView.BL_VirtualListViewItem();
                                item.Tag = qr;
                                item.Add("");
                                item.Add("ST[" + qr.station_id + "] [" + floorId + "]");
                                item.Add(qr.station_complete_trigger ? "ON" : "OFF");
                                listviewTrg.Items.Add(item);
                                listviewTrg.NeedRedraw = true;
                            }
                            else if (qr.direction_charge != AgvControlManager.FloorQR.enDirection.NONE)
                            {
                                item = new BL_VirtualListView.BL_VirtualListViewItem();
                                item.Tag = qr;
                                item.Add("");
                                item.Add("CP [" + floorId + "]");
                                item.Add(qr.charge_complete_trigger ? "ON" : "OFF");
                                listviewTrg.Items.Add(item);
                                listviewTrg.NeedRedraw = true;
                            }
                        }
                    }
                }
            }

            listviewTrg.Items.Sort((a, b) => string.Compare(a[1].ToString(), b[1].ToString()));
            listviewTrg.RefreshMe(false);
        }

        protected void ListupStation()
        {
            listview_station.Items.Clear();

            //var floor = map.SelectFloor();

            foreach (var kv in controller.map)
            {
                var floor = kv.Value;

                if (floor != null)
                {
                    foreach (var kvx in floor.mapeditor.qrs)
                    {
                        foreach (var kvy in kvx.Value)
                        {
                            var qr = kvy.Value;

                            if (qr.direction_station != AgvControlManager.FloorQR.enDirection.NONE)
                            {
                                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                                item.Tag = qr;
                                item.Add("");
                                item.Add("ST[" + qr.station_id + "]");
                                listview_station.Items.Add(item);
                                listview_station.NeedRedraw = true;
                            }
                        }
                    }
                }
            }

            listview_station.Items.Sort((a, b) => string.Compare(a[1].ToString(), b[1].ToString()));
            listview_station.RefreshMe(false);
        }

        protected void ListupRack()
        {
            listview_rack.Items.Clear();
            listview_rack.NeedRedraw = true;

            SortedDictionary<string, BL_VirtualListView.BL_VirtualListViewItem> list = new SortedDictionary<string, BL_VirtualListView.BL_VirtualListViewItem>();


            foreach (var kv in controller.map)
            {
                var floor = kv.Value;

                if (floor != null)
                {
                    foreach (var kvx in floor.mapeditor.qrs)
                    {
                        foreach (var kvy in kvx.Value)
                        {
                            var qr = kvy.Value;

                            if (qr.rack != null)
                            {
                                AgvControlManager.Rack rack = qr.rack;

                                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                                item.Tag = rack; // なにを入れるのがいいのか要検討
                                item.Add("");
                                item.Add(rack.rack_no);

                                list[rack.rack_no.PadLeft(10)] = item;
                            }
                        }
                    }
                }
            }

            if (0 < list.Count)
            {
                foreach (var kv in list)
                {
                    listview_rack.Items.Add(kv.Value);
                }
            }

            listview_rack.Items.Sort((a, b) => string.Compare(a[1].ToString(), b[1].ToString()));
            listview_rack.RefreshMe(false);
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

        private void listviewTrg_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listviewTrg.SelectedIndices.Count == 0) return;

            //var floor = map.SelectFloor();

            foreach (var kv in controller.map)
            {
                var floor = kv.Value;
                if (floor != null)
                {
                    var item = listviewTrg.SortedItems[listviewTrg.SelectedIndices[0]];
                    AgvControlManager.FloorQR qr = item.Tag as AgvControlManager.FloorQR;
                    if (qr != null)
                    {
                        if (qr.autorator_id != "")
                        {
                            if (item[0].ToString() == "IN")
                            {
                                item[2] = qr.autoratorIN_trigger;
                            }
                            else if (item[0].ToString() == "OUT")
                            {
                                item[2] = qr.autoratorOUT_trigger;
                            }
                        }
                        else if (qr.direction_station != AgvControlManager.FloorQR.enDirection.NONE)
                        {
                            item[2] = qr.station_complete_trigger ? "ON" : "OFF";
                        }
                        else if (qr.direction_charge != AgvControlManager.FloorQR.enDirection.NONE)
                        {
                            item[2] = qr.charge_complete_trigger ? "ON" : "OFF";
                        }

                        buttonTrg.Checked = item[2].ToString() == "ON" ? true : false;
                        buttonTrg.Text = item[2].ToString() == "ON" ? "[F1] トリガーON" : "[F1] トリガーOFF";

                        listviewTrg.RefreshMe(false);
                    }
                }
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

        protected override void SubForm_Base_Function07_Clicked(object sender)
        {
            base.SubForm_Base_Function07_Clicked(sender);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = Path.GetDirectoryName(Program.ini_agv.FullName);
            dlg.AddExtension = true;
            dlg.Filter = "物流AGV経路設定ファイル (*.LAG)|*.lag|All files (*.*)|*.*";
            if (dlg.ShowDialog(this) == DialogResult.Cancel) return;

            //BL_IniFile ini = new BL_IniFile(Path.Combine(Path.GetDirectoryName(Program.ini_agv.FullName), Path.GetFileNameWithoutExtension(Program.ini_agv.FullName) + ".ini"));

            Cursor = Cursors.WaitCursor;

            controller.Stop();
            buttonStart.Checked = false;
            m_Mainform.btnFunctions[7].Checked = false;

            controller.Load(dlg.FileName);

            listviewAGV.ItemsClear();
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                foreach (var v in controller.agvs)
                {
                    BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                    item.Add(v);
                    item.Add(v);
                    item.Add("");
                    item.Add("");
                    item.Add("");
                    listviewAGV.Items.Add(item);
                }
            }
            listviewAGV.RefreshMe();

            ListupFloor();
            ListupTrigger();
            ListupStation();
            ListupRack();

            panelMap.Invalidate();

            Cursor = Cursors.Default;
        }

        private void textBoxSleepInterval_Leave(object sender, EventArgs e)
        {
            if (controller.run_manager != null)
            {
                int interval = 0;
                int.TryParse(textBoxSleepInterval.Text, out interval);

                if (1 < interval) controller.run_manager.Interval = interval;
            }
        }

        private void listview_rack_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listview_rack.SelectedItems.Count == 0) return;

            AgvControlManager.Rack rack = listview_rack.SelectedItems[0].Tag as AgvControlManager.Rack;

            listviewRackFace.ItemsClear();

            foreach (var face in rack.face_id)
            {
                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                item.Tag = rack; // なにを入れるのがいいのか要検討
                item.Add("");
                item.Add(face.Value);

                listviewRackFace.Items.Add(item);
            }

            listviewRackFace.Items.Sort((a, b) => string.Compare(a[1].ToString(), b[1].ToString()));
            listviewRackFace.RefreshMe();
        }
    }
}
