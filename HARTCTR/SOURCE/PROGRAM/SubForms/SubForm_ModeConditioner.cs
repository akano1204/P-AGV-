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
using BelicsClass.File;

namespace PROGRAM
{
    public partial class SubForm_ModeConditioner : BelicsClass.UI.BL_SubForm_Base
    {
        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            //return new string[] { "", "[F1]:中徐", "[F2]:中", "[F3]:高中", "[F4]:高", "[F5]:AGV配置", "[F6]:動作設定", "[F7]:読込", "[F8]:保存", "", "", "[F11]:吸着", "[F12]:戻る" };
            return new string[] { "", "", "", "", "", "[F5]:AGV配置", "[F6]:動作設定", "[F7]:読込", "[F8]:保存", "", "", "[F11]:吸着", "[F12]:戻る" };
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
        Dictionary<int, BL_FlatButton> agvturn = new Dictionary<int, BL_FlatButton>();
        Dictionary<int, BL_FlatButton> rackturn = new Dictionary<int, BL_FlatButton>();

        BL_IniFile agv_ini = new BL_IniFile(Path.Combine(Path.GetDirectoryName(Program.ini_agv.FullName), "AGV.ini"));

        public SubForm_ModeConditioner()
        {
            InitializeComponent();

            this.controller = Program.controller;

            controller.applicate_mode = AgvControlManager.enApplicateMode.CONDITIONER;
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            //controller.EventAgvMoved -= Map_EventAgvMoved;
            controller.EventAgvSelected -= Map_EventAgvSelected;
            controller.applicate_mode = AgvControlManager.enApplicateMode.UNKNOWN;

            Close();
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            textBoxFloorCode.Text = controller.selected_floor;

            controller.draw_allfloor = false;

            //controller.EventAgvMoved += Map_EventAgvMoved;
            controller.EventAgvSelected += Map_EventAgvSelected;
            controller.EventEditCondition += Map_EventEditCondition;
            panelMap.MouseWheel += PanelMap_MouseWheel;

            agvturn[0] = buttonAgvTurnNorth;
            agvturn[45] = buttonAgvTurnNorthWest;
            agvturn[90] = buttonAgvTurnWest;
            agvturn[135] = buttonAgvTurnSouthWest;
            agvturn[180] = buttonAgvTurnSouth;
            agvturn[-135] = buttonAgvTurnSouthEast;
            agvturn[-90] = buttonAgvTurnEast;
            agvturn[-45] = buttonAgvTurnNorthEast;

            rackturn[0] = buttonRackTurnNorth;
            rackturn[45] = buttonRackTurnNorthWest;
            rackturn[90] = buttonRackTurnWest;
            rackturn[135] = buttonRackTurnSouthWest;
            rackturn[180] = buttonRackTurnSouth;
            rackturn[-135] = buttonRackTurnSouthEast;
            rackturn[-90] = buttonRackTurnEast;
            rackturn[-45] = buttonRackTurnNorthEast;

            ListupFloor();
            ListupMode();
            ListupAgv();

            AttachButton_to_Functions(buttonNewAGV, 5);
            AttachButton_to_Functions(buttonNewCondition, 6);
            AttachButton_to_Functions(buttonStuck, 11);

            AttachButton_to_Functions(buttonPassML, 1);
            AttachButton_to_Functions(buttonPassM, 2);
            AttachButton_to_Functions(buttonPassHM, 3);
            AttachButton_to_Functions(buttonPassH, 4);

            comboBoxMode1.SelectedIndex = 0;
            comboBoxMode2.SelectedIndex = 0;

            textBoxZoom.Text = ((int)(controller.draw_scale * 100)).ToString();
        }

        private void ListupAgv()
        {
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
                    item.Tag = v;
                    listviewAGV.Items.Add(item);
                }
                listviewAGV.RefreshMe();

                if (0 < listviewAGV.Items.Count)
                {
                    listviewAGV.SelectItem(0);
                }
            }
        }

        bool with_event = false;

        private void Map_EventEditCondition(AgvControlManager.RouteCondition con)
        {
            with_event = true;
            listviewConditions.ItemsClear();

            BL_VirtualListView.BL_VirtualListViewItem selected_item = null;

            if (con != null && con.owner_agv != null)
            {
                int no = 1;
                foreach (var v in con.owner_agv.mode_conditions[con.owner_agv.moveMode])
                {
                    BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                    item.Add(no);
                    item.Add(v);
                    item.Tag = v;
                    if (v == con) selected_item = item;
                    listviewConditions.Items.Add(item);
                    no++;
                }
            }
            else
            {
                panelSelectedAGV.Tag = null;
                panelSelectedAGV.Enabled = false;
            }

            listviewConditions.RefreshMe();

            if (selected_item != null) listviewConditions.SelectItem(selected_item);

            with_event = false;
            listviewConditions_SelectedIndexChanged(listviewConditions, null);
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
                    textBoxAgvID.Text = agv.id;
                    textBoxCurX.Text = ((int)agv.Location.X).ToString();
                    textBoxCurY.Text = ((int)agv.Location.Y).ToString();
                }
                else
                {
                    textBoxAgvID.Text = "";
                    textBoxCurX.Text = "";
                    textBoxCurY.Text = "";
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
                        item.Tag = v;
                        listviewAGV.Items.Add(item);

                        if (v == agv) selectitem = item;
                    }
                    listviewAGV.RefreshMe();
                    if (selectitem != null)
                    {
                        listviewAGV.SelectItem(selectitem);

                        if (0 < listviewMode.SelectedIndices.Count)
                        {
                            var floor = controller.SelectFloor();
                            if (floor != null)
                            {
                                AgvControlManager.MoveMode mode = listviewMode.SortedItems[listviewMode.SelectedIndices[0]].Tag as AgvControlManager.MoveMode;
                                if (mode != null)
                                {
                                    //floor.conditioner.MoveMode(mode);

                                    foreach (var v in controller.agvs)
                                    {
                                        if (v.selected)
                                        {
                                            v.moveMode = mode.mode;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
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

                    checkedListBoxOtherAGV.BeginUpdate();
                    item_set = true;
                    {
                        checkedListBoxOtherAGV.Items.Clear();

                        foreach (var v in controller.agvs)
                        {
                            if (v != agv)
                            {
                                if (con == null || con.wait_other_agv_locations.ContainsKey(v))
                                {
                                    checkedListBoxOtherAGV.Items.Add(v, true);
                                }
                                else
                                {
                                    checkedListBoxOtherAGV.Items.Add(v, false);
                                }
                            }
                        }
                    }
                    item_set = false;
                    checkedListBoxOtherAGV.EndUpdate();
                }
            }
            else
            {
                listviewConditions.ItemsClear();
                listviewConditions.RefreshMe();
                checkedListBoxOtherAGV.Items.Clear();
            }
        }

        #region マウス操作

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

                if (controller.floorwarp_con == null)
                {
                    buttonFloorWarp.Checked = false;
                }

                panelMap.Invalidate();
            }
        }

        private void PanelMap_MouseWheel(object sender, MouseEventArgs e)
        {
            controller.MouseWheel(e);
            panelMap.Invalidate();

            textBoxZoom.Text = ((int)(controller.draw_scale * 100)).ToString();
        }

        #endregion

        #region 画面操作

        private void panelMap_Paint(object sender, PaintEventArgs e)
        {
            controller.draw_size_pixel.Width = panelMap.Width;
            controller.draw_size_pixel.Height = panelMap.Height;

            controller.Draw(e.Graphics);
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

        private void buttonStuck_Click(object sender, EventArgs e)
        {
            controller.Stuck(buttonStuck.Checked);
        }

        private void panelSelectedAGV_EnabledChanged(object sender, EventArgs e)
        {
            buttonPassML.Enabled = buttonPassM.Enabled = buttonPassHM.Enabled = buttonPassH.Enabled = panelSelectedAGV.Enabled;
        }

        private void buttonModeAdd_Click(object sender, EventArgs e)
        {
            int mode = 0; int.TryParse(textBoxMode.Text, out mode);
            if (mode == 0) return;

            if (!AgvControlManager.moveModes.ContainsKey(mode)) AgvControlManager.moveModes[mode] = new AgvControlManager.MoveMode();
            AgvControlManager.moveModes[mode].mode = mode;
            AgvControlManager.moveModes[mode].name = textBoxModeName.Text;
            AgvControlManager.moveModes[mode].option = "";
            AgvControlManager.moveModes[mode].option += comboBoxMode1.Text;
            AgvControlManager.moveModes[mode].option += "+";
            AgvControlManager.moveModes[mode].option += comboBoxMode2.Text;

            foreach (var v in controller.agvs)
            {
                if (!v.mode_conditions.ContainsKey(mode)) v.mode_conditions[mode] = new AgvControlManager.RouteConditionList();

                v.AddNextCondition(v.on_qr);
            }

            ListupMode();
        }

        private void buttonModeRemove_Click(object sender, EventArgs e)
        {
            int mode = 0; int.TryParse(textBoxMode.Text, out mode);
            if (mode == 0) return;

            if (AgvControlManager.moveModes.ContainsKey(mode)) AgvControlManager.moveModes.Remove(mode);

            ListupMode();
        }

        private void ListupMode()
        {
            listviewMode.ItemsClear();
            foreach (var v in AgvControlManager.moveModes.Values)
            {
                if (v.mode == 0) continue;

                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                item.Add(v);
                item.Add(v.name);
                item.Tag = v;
                listviewMode.Items.Add(item);
            }
            listviewMode.RefreshMe();
        }

        private void textBoxAgvID_Leave(object sender, EventArgs e)
        {
            if (listviewAGV.SelectedIndices.Count <= 0) return;
            AgvControlManager.FloorAGV agv = listviewAGV.SortedItems[listviewAGV.SelectedIndices[0]].Tag as AgvControlManager.FloorAGV;
            if (agv == null) return;

            agv.id = textBoxAgvID.Text.Trim();
        }

        #endregion

        #region AGV配置・動作設定

        protected override void SubForm_Base_Function05_Clicked(object sender)
        {
            base.SubForm_Base_Function05_Clicked(sender);

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                if (buttonNewAGV.Checked)
                {
                    buttonNewCondition.Checked = false;
                    floor.conditioner.NewCondition(buttonNewCondition.Checked);
                }

                floor.conditioner.NewAGV(buttonNewAGV.Checked);
                panelMap.Invalidate();
            }
        }

        protected override void SubForm_Base_Function06_Clicked(object sender)
        {
            base.SubForm_Base_Function06_Clicked(sender);

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                if (buttonNewCondition.Checked)
                {
                    buttonFloorWarp.Enabled = true;
                    buttonNewAGV.Checked = false;
                    floor.conditioner.NewAGV(buttonNewAGV.Checked);
                }
                else
                {
                    buttonFloorWarp.Enabled = false;
                }

                floor.conditioner.NewCondition(buttonNewCondition.Checked);
                panelMap.Invalidate();
            }
        }

        #endregion

        private void listviewMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listviewMode.SelectedIndices.Count <= 0) return;

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                AgvControlManager.MoveMode mode = listviewMode.SortedItems[listviewMode.SelectedIndices[0]].Tag as AgvControlManager.MoveMode;
                if (mode != null)
                {
                    controller.SetMoveMode(mode.mode);

                    textBoxMode.Text = mode.mode.ToString();
                    textBoxModeName.Text = mode.name;

                    for (int i = 0; i < comboBoxMode1.Items.Count; i++)
                    {
                        if (mode.option.Contains(comboBoxMode1.Items[i].ToString()))
                        {
                            comboBoxMode1.SelectedIndex = i;
                        }
                    }

                    for (int i = 0; i < comboBoxMode2.Items.Count; i++)
                    {
                        if (mode.option.Contains(comboBoxMode2.Items[i].ToString()))
                        {
                            comboBoxMode2.SelectedIndex = i;
                        }
                    }

                    foreach (var v in controller.agvs)
                    {
                        if (v.selected)
                        {
                            v.moveMode = mode.mode;
                            Map_EventAgvSelected(v);
                            break;
                        }
                    }
                }

                panelMap.Invalidate();
            }
        }

        private void listviewAGV_Click(object sender, EventArgs e)
        {
            if (listviewAGV.SelectedIndices.Count <= 0) return;
            AgvControlManager.FloorAGV agv = listviewAGV.SortedItems[listviewAGV.SelectedIndices[0]].Tag as AgvControlManager.FloorAGV;
            if (agv == null) return;

            agv.Select(true);
            panelMap.Invalidate();

            textBoxAgvID.Text = agv.id;

            checkedListBoxOtherAGV.BeginUpdate();
            item_set = true;
            {
                checkedListBoxOtherAGV.Items.Clear();

                foreach (var v in controller.agvs)
                {
                    if (v != agv)
                    {
                        checkedListBoxOtherAGV.Items.Add(v, true);
                    }
                }
            }
            item_set = false;
            checkedListBoxOtherAGV.EndUpdate();
        }

        private void listviewConditions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (with_event) return;
            if (listviewConditions.SelectedIndices.Count <= 0) return;

            AgvControlManager.RouteCondition con = listviewConditions.SortedItems[listviewConditions.SelectedIndices[0]].Tag as AgvControlManager.RouteCondition;
            if (con == null) return;

            con.Select(true);
            panelMap.Invalidate();

            if (controller.selected_floor != con.FloorCode)
            {
                ListupFloor();
            }

            panelSelectedAGV.Tag = con;
            panelSelectedAGV.Enabled = true;

            buttonPassML.Checked = con.speed == AgvController.enSpeed.ML;
            buttonPassM.Checked = con.speed == AgvController.enSpeed.M;
            buttonPassHM.Checked = con.speed == AgvController.enSpeed.HM;
            buttonPassH.Checked = con.speed == AgvController.enSpeed.H;

            m_Mainform.btnFunctions[1].Checked = buttonPassML.Checked;
            m_Mainform.btnFunctions[2].Checked = buttonPassM.Checked;
            m_Mainform.btnFunctions[3].Checked = buttonPassHM.Checked;
            m_Mainform.btnFunctions[4].Checked = buttonPassH.Checked;

            if (con.wait_timer == 0)
            {
                textBoxWaitTime.Text = "";
                buttonWait.Checked = false;
            }
            else
            {
                textBoxWaitTime.Text = con.wait_timer.ToString();
                buttonWait.Checked = true;
            }

            if (buttonAGVArrive.Checked)
            {
                if (con.agv_turn_arrive)
                {
                    bool found = false;
                    textBoxTurnDegree.Text = "";
                    foreach (var kv in agvturn)
                    {
                        kv.Value.Checked = (kv.Key == (int)con.agv_turn_arrive_degree);
                        if (kv.Value.Checked)
                        {
                            textBoxTurnDegree.Text = kv.Key.ToString();
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        textBoxTurnDegree.Text = ((int)con.agv_turn_arrive_degree).ToString();
                    }
                }
                else
                {
                    textBoxTurnDegree.Text = "";
                    foreach (var kv in agvturn) kv.Value.Checked = false;
                }

                if (con.rack_turn_arrive)
                {
                    bool found = false;
                    textBoxRackTurnDegree.Text = "";
                    foreach (var kv in rackturn)
                    {
                        kv.Value.Checked = (kv.Key == (int)con.rack_turn_arrive_degree);
                        if (kv.Value.Checked)
                        {
                            textBoxRackTurnDegree.Text = kv.Key.ToString();
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        textBoxRackTurnDegree.Text = ((int)con.rack_turn_arrive_degree).ToString();
                    }
                }
                else
                {
                    textBoxRackTurnDegree.Text = "";
                    foreach (var kv in rackturn) kv.Value.Checked = false;
                }

                //buttonRackStayTurn.Checked = con.rack_stay_turn_arrive;
                buttonRackUp.Checked = con.rack_up_arrive;
                buttonRackDown.Checked = con.rack_down_arrive;
            }
            else
            {
                if (con.agv_turn_departure)
                {
                    bool found = false;
                    textBoxTurnDegree.Text = "";
                    foreach (var kv in agvturn)
                    {
                        kv.Value.Checked = (kv.Key == (int)con.agv_turn_departure_degree);
                        if (kv.Value.Checked)
                        {
                            textBoxTurnDegree.Text = kv.Key.ToString();
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        textBoxTurnDegree.Text = ((int)con.agv_turn_departure_degree).ToString();
                    }
                }
                else
                {
                    textBoxTurnDegree.Text = "";
                    foreach (var kv in agvturn) kv.Value.Checked = false;
                }

                if (con.rack_turn_departure)
                {
                    bool found = false;
                    textBoxRackTurnDegree.Text = "";
                    foreach (var kv in rackturn)
                    {
                        kv.Value.Checked = (kv.Key == (int)con.rack_turn_departure_degree);
                        if (kv.Value.Checked)
                        {
                            textBoxRackTurnDegree.Text = kv.Key.ToString();
                            found = true;
                        }
                    }

                    if (!found)
                    {
                        textBoxRackTurnDegree.Text = ((int)con.rack_turn_departure_degree).ToString();
                    }
                }
                else
                {
                    textBoxRackTurnDegree.Text = "";
                    foreach (var kv in rackturn) kv.Value.Checked = false;
                }

                //buttonRackStayTurn.Checked = con.rack_stay_turn_departure;
                buttonRackUp.Checked = con.rack_up_departure;
                buttonRackDown.Checked = con.rack_down_departure;
            }

            buttonRackRegulation.Checked = con.rack_regulation;

            buttonStationTrg.Checked = con.wait_station_trg;
            buttonAutoratorInTrg.Checked = con.wait_autorator_in_trg;
            buttonAutoratorOutTrg.Checked = con.wait_autorator_out_trg;
            buttonChargeTrg.Checked = con.wait_charge_trg;
            buttonRackDownDeparture.Checked = con.rack_down_departure_last;

            buttonLocation.Checked = 0 < con.wait_other_agv_locations.Count;

            checkedListBoxOtherAGV.BeginUpdate();
            item_set = true;
            {
                checkedListBoxOtherAGV.Items.Clear();

                foreach (var v in controller.agvs)
                {
                    if (v != con.owner_agv)
                    {
                        if (con == null || con.wait_other_agv_locations.ContainsKey(v))
                        {
                            checkedListBoxOtherAGV.Items.Add(v, true);
                        }
                        else
                        {
                            checkedListBoxOtherAGV.Items.Add(v, false);
                        }
                    }
                }
            }
            item_set = false;
            checkedListBoxOtherAGV.EndUpdate();
        }

        #region 速度設定操作

        protected override void SubForm_Base_Function01_Clicked(object sender)
        {
            //base.SubForm_Base_Function01_Clicked(sender);

            //m_Mainform.btnFunctions[1].Checked = true;
            //m_Mainform.btnFunctions[2].Checked = false;
            //m_Mainform.btnFunctions[3].Checked = false;
            //m_Mainform.btnFunctions[4].Checked = false;

            //buttonPassML.Checked = true;
            //buttonPassM.Checked = false;
            //buttonPassHM.Checked = false;
            //buttonPassH.Checked = false;

            //AgvControllers.RouteCondition con = panelSelectedAGV.Tag as AgvControllers.RouteCondition;
            //if (con != null) con.speed = AgvController.enSpeed.ML;

            //listviewConditions.Focus();
        }

        protected override void SubForm_Base_Function02_Clicked(object sender)
        {
            //base.SubForm_Base_Function02_Clicked(sender);

            //m_Mainform.btnFunctions[1].Checked = false;
            //m_Mainform.btnFunctions[2].Checked = true;
            //m_Mainform.btnFunctions[3].Checked = false;
            //m_Mainform.btnFunctions[4].Checked = false;

            //buttonPassML.Checked = false;
            //buttonPassM.Checked = true;
            //buttonPassHM.Checked = false;
            //buttonPassH.Checked = false;

            //AgvControllers.RouteCondition con = panelSelectedAGV.Tag as AgvControllers.RouteCondition;
            //if (con != null) con.speed = AgvController.enSpeed.M;

            //listviewConditions.Focus();
        }

        protected override void SubForm_Base_Function03_Clicked(object sender)
        {
            //base.SubForm_Base_Function03_Clicked(sender);

            //m_Mainform.btnFunctions[1].Checked = false;
            //m_Mainform.btnFunctions[2].Checked = false;
            //m_Mainform.btnFunctions[3].Checked = true;
            //m_Mainform.btnFunctions[4].Checked = false;

            //buttonPassML.Checked = false;
            //buttonPassM.Checked = false;
            //buttonPassHM.Checked = true;
            //buttonPassH.Checked = false;

            //AgvControllers.RouteCondition con = panelSelectedAGV.Tag as AgvControllers.RouteCondition;
            //if (con != null) con.speed = AgvController.enSpeed.HM;

            //listviewConditions.Focus();
        }

        protected override void SubForm_Base_Function04_Clicked(object sender)
        {
            //base.SubForm_Base_Function04_Clicked(sender);

            //m_Mainform.btnFunctions[1].Checked = false;
            //m_Mainform.btnFunctions[2].Checked = false;
            //m_Mainform.btnFunctions[3].Checked = false;
            //m_Mainform.btnFunctions[4].Checked = true;

            //buttonPassML.Checked = false;
            //buttonPassM.Checked = false;
            //buttonPassHM.Checked = false;
            //buttonPassH.Checked = true;

            //AgvControllers.RouteCondition con = panelSelectedAGV.Tag as AgvControllers.RouteCondition;
            //if (con != null) con.speed = AgvController.enSpeed.H;

            //listviewConditions.Focus();
        }

        #endregion

        #region 動作設定操作

        private void buttonWait_Click(object sender, EventArgs e)
        {
            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                if (!button.Checked)
                {
                    con.wait_timer = 0;
                }
                else
                {
                    int.TryParse(textBoxWaitTime.Text, out con.wait_timer);
                }
            }
        }

        private void buttonAgvTurn_Click(object sender, EventArgs e)
        {
            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                if (!button.Checked)
                {
                    if (buttonAGVArrive.Checked)
                    {
                        con.agv_turn_arrive = false;
                        con.agv_turn_arrive_degree = 0;
                    }
                    else
                    {
                        con.agv_turn_departure = false;
                        con.agv_turn_departure_degree = 0;
                    }
                    textBoxTurnDegree.Text = "";
                }
                else
                {
                    foreach (var kv in agvturn)
                    {
                        if (kv.Value == button)
                        {
                            if (buttonAGVArrive.Checked)
                            {
                                con.agv_turn_arrive = true;
                                con.agv_turn_arrive_degree = kv.Key;
                            }
                            else
                            {
                                con.agv_turn_departure = true;
                                con.agv_turn_departure_degree = kv.Key;
                            }

                            textBoxTurnDegree.Text = kv.Key.ToString();
                        }
                        else
                        {
                            kv.Value.Checked = false;
                        }
                    }
                }
                panelMap.Invalidate();
            }
            else
            {
                button.Checked = false;
            }
        }

        private void buttonRackTurn_Click(object sender, EventArgs e)
        {
            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                if (!button.Checked)
                {
                    if (buttonAGVArrive.Checked)
                    {
                        con.rack_turn_arrive = false;
                        con.rack_turn_arrive_degree = 0;
                    }
                    else
                    {
                        con.rack_turn_departure = false;
                        con.rack_turn_departure_degree = 0;
                    }
                    textBoxRackTurnDegree.Text = "";
                }
                else
                {
                    foreach (var kv in rackturn)
                    {
                        if (kv.Value == button)
                        {
                            if (buttonAGVArrive.Checked)
                            {
                                con.rack_turn_arrive = true;
                                con.rack_turn_arrive_degree = kv.Key;
                            }
                            else
                            {
                                con.rack_turn_departure = true;
                                con.rack_turn_departure_degree = kv.Key;
                            }
                            textBoxRackTurnDegree.Text = kv.Key.ToString();
                        }
                        else
                        {
                            kv.Value.Checked = false;
                        }
                    }
                }
                panelMap.Invalidate();
            }
            else
            {
                button.Checked = false;
            }
        }

        private void buttonRackUp_Click(object sender, EventArgs e)
        {
            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                if (!button.Checked)
                {
                    if (buttonAGVArrive.Checked)
                    {
                        con.rack_up_arrive = false;
                    }
                    else
                    {
                        con.rack_up_departure = false;
                    }
                }
                else
                {
                    buttonRackDown.Checked = false;

                    if (buttonAGVArrive.Checked)
                    {
                        con.rack_up_arrive = true;
                        con.rack_down_arrive = false;
                    }
                    else
                    {
                        con.rack_up_departure = true;
                        con.rack_down_departure = false;
                    }
                }
            }
            panelMap.Invalidate();
        }

        private void buttonRackDown_Click(object sender, EventArgs e)
        {
            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                if (!button.Checked)
                {
                    if (buttonAGVArrive.Checked)
                    {
                        con.rack_down_arrive = false;
                    }
                    else
                    {
                        con.rack_down_departure = false;
                    }
                }
                else
                {
                    buttonRackUp.Checked = false;

                    if (buttonAGVArrive.Checked)
                    {
                        con.rack_up_arrive = false;
                        con.rack_down_arrive = true;
                    }
                    else
                    {
                        con.rack_up_departure = false;
                        con.rack_down_departure = true;
                    }
                }
            }
            panelMap.Invalidate();
        }

        private void buttonRackRegulation_Click(object sender, EventArgs e)
        {
            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                con.rack_regulation = buttonRackRegulation.Checked;
            }
        }

        private void buttonStationTrg_Click(object sender, EventArgs e)
        {
            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                if (!button.Checked)
                {
                    con.wait_station_trg = false;
                }
                else
                {
                    con.wait_station_trg = true;
                }
            }

            con.owner_agv.AutoSpeedSet();

            panelMap.Invalidate();
        }

        private void buttonAutoratorInTrg_Click(object sender, EventArgs e)
        {
            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                if (!button.Checked)
                {
                    con.wait_autorator_in_trg = false;
                }
                else
                {
                    con.wait_autorator_in_trg = true;
                }
            }

            con.owner_agv.AutoSpeedSet();

            panelMap.Invalidate();
        }

        private void buttonAutoratorOutTrg_Click(object sender, EventArgs e)
        {
            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                if (!button.Checked)
                {
                    con.wait_autorator_out_trg = false;
                }
                else
                {
                    con.wait_autorator_out_trg = true;
                }
            }

            con.owner_agv.AutoSpeedSet();

            panelMap.Invalidate();
        }

        private void buttonChargeTrg_Click(object sender, EventArgs e)
        {
            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                if (!button.Checked)
                {
                    con.wait_charge_trg = false;
                }
                else
                {
                    con.wait_charge_trg = true;
                }
            }

            con.owner_agv.AutoSpeedSet();

            panelMap.Invalidate();
        }

        private void buttonLocation_Click(object sender, EventArgs e)
        {
            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                con.wait_other_agv_locations.Clear();

                if (button.Checked)
                {
                    foreach (var v in checkedListBoxOtherAGV.CheckedItems)
                    {
                        AgvControlManager.FloorAGV agv = v as AgvControlManager.FloorAGV;

                        if (agv != con.owner_agv)
                        {
                            con.wait_other_agv_locations[agv] = agv.Location;
                        }
                    }
                }
            }

            con.owner_agv.AutoSpeedSet();

            panelMap.Invalidate();
        }

        private void buttonAGVArrive_Click(object sender, EventArgs e)
        {
            buttonAGVDeparture.Checked = !buttonAGVArrive.Checked;

            UpdateArriveDepartureState();
        }

        private void buttonAGVDeparture_Click(object sender, EventArgs e)
        {
            buttonAGVArrive.Checked = !buttonAGVDeparture.Checked;

            UpdateArriveDepartureState();
        }

        void UpdateArriveDepartureState()
        {
            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                if (buttonAGVArrive.Checked)
                {
                    if (con.agv_turn_arrive)
                    {
                        bool found = false;
                        textBoxTurnDegree.Text = "";
                        foreach (var kv in agvturn)
                        {
                            kv.Value.Checked = (kv.Key == (int)con.agv_turn_arrive_degree);
                            if (kv.Value.Checked)
                            {
                                textBoxTurnDegree.Text = kv.Key.ToString();
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            textBoxTurnDegree.Text = ((int)con.agv_turn_arrive_degree).ToString();
                        }
                    }
                    else
                    {
                        textBoxTurnDegree.Text = "";
                        foreach (var kv in agvturn) kv.Value.Checked = false;
                    }

                    if (con.rack_turn_arrive)
                    {
                        bool found = false;
                        textBoxRackTurnDegree.Text = "";
                        foreach (var kv in rackturn)
                        {
                            kv.Value.Checked = (kv.Key == (int)con.rack_turn_arrive_degree);
                            if (kv.Value.Checked)
                            {
                                textBoxRackTurnDegree.Text = kv.Key.ToString();
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            textBoxRackTurnDegree.Text = ((int)con.rack_turn_arrive_degree).ToString();
                        }
                    }
                    else
                    {
                        textBoxRackTurnDegree.Text = "";
                        foreach (var kv in rackturn) kv.Value.Checked = false;
                    }

                    //buttonRackStayTurn.Checked = con.rack_stay_turn_arrive;
                    buttonRackUp.Checked = con.rack_up_arrive;
                    buttonRackDown.Checked = con.rack_down_arrive;
                }
                else
                {
                    if (con.agv_turn_departure)
                    {
                        bool found = false;
                        textBoxTurnDegree.Text = "";
                        foreach (var kv in agvturn)
                        {
                            kv.Value.Checked = (kv.Key == (int)con.agv_turn_departure_degree);
                            if (kv.Value.Checked)
                            {
                                textBoxTurnDegree.Text = kv.Key.ToString();
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            textBoxTurnDegree.Text = ((int)con.agv_turn_departure_degree).ToString();
                        }
                    }
                    else
                    {
                        textBoxTurnDegree.Text = "";
                        foreach (var kv in agvturn) kv.Value.Checked = false;
                    }

                    if (con.rack_turn_departure)
                    {
                        bool found = false;
                        textBoxRackTurnDegree.Text = "";
                        foreach (var kv in rackturn)
                        {
                            kv.Value.Checked = (kv.Key == (int)con.rack_turn_departure_degree);
                            if (kv.Value.Checked)
                            {
                                textBoxRackTurnDegree.Text = kv.Key.ToString();
                                found = true;
                            }
                        }

                        if (!found)
                        {
                            textBoxRackTurnDegree.Text = ((int)con.rack_turn_departure_degree).ToString();
                        }
                    }
                    else
                    {
                        textBoxRackTurnDegree.Text = "";
                        foreach (var kv in rackturn) kv.Value.Checked = false;
                    }

                    //buttonRackStayTurn.Checked = con.rack_stay_turn_departure;
                    buttonRackUp.Checked = con.rack_up_departure;
                    buttonRackDown.Checked = con.rack_down_departure;
                }
            }
        }

        private void buttonRackDownDeparture_Click(object sender, EventArgs e)
        {
            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                con.rack_down_departure_last = buttonRackDownDeparture.Checked;
                con.owner_agv.AutoSpeedSet();
            }
        }

        bool item_set = false;

        private void checkedListBoxOtherAGV_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (item_set) return;

            if (e.Index < 0) return;
            if (listviewConditions.SelectedIndices.Count == 0) return;

            AgvControlManager.RouteCondition con = listviewConditions.SortedItems[listviewConditions.SelectedIndices[0]].Tag as AgvControlManager.RouteCondition;
            if (con == null) return;

            AgvControlManager.FloorAGV agv = checkedListBoxOtherAGV.Items[e.Index] as AgvControlManager.FloorAGV;
            if (agv != null)
            {
                if (con.wait_other_agv_locations.ContainsKey(agv))
                {
                    if (e.NewValue != CheckState.Checked)
                    {
                        con.wait_other_agv_locations.Remove(agv);
                    }
                    else
                    {
                        con.wait_other_agv_locations[agv] = agv.Location;
                    }
                }

                con.owner_agv.AutoSpeedSet();

                panelMap.Invalidate();
            }
        }

        #endregion

        #region フロア操作

        private void listviewFloor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listviewFloor.SelectedIndices.Count == 0) return;

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                if (floor.conditioner.is_new_agv) floor.conditioner.NewAGV(false);
                if (floor.conditioner.is_new_condition) floor.conditioner.NewCondition(false);
            }

            buttonNewAGV.Checked = false;
            buttonNewCondition.Checked = false;

            string code = listviewFloor.SortedItems[listviewFloor.SelectedIndices[0]][1].ToString();
            floor = controller.SelectFloor(code);

            textBoxFloorCode.Text = floor.code;

            if (controller.floorwarp_con != null)
            {
                floor.conditioner.NewCondition(true);
                buttonFloorWarp.Checked = true;
            }

            panelMap.Invalidate();

            ListupAgv();
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

        private void buttonFloorWarp_Click(object sender, EventArgs e)
        {
            AgvControlManager.RouteCondition con = panelSelectedAGV.Tag as AgvControlManager.RouteCondition;
            if (con != null)
            {
                if (buttonFloorWarp.Checked)
                {
                    controller.floorwarp_con = con;
                }
                else
                {
                    controller.floorwarp_con = null;
                }
            }
            else
            {
                controller.floorwarp_con = null;
                buttonFloorWarp.Checked = false;
            }
        }

        #endregion

        #region 読込・保存

        protected override void SubForm_Base_Function08_Clicked(object sender)
        {
            base.SubForm_Base_Function08_Clicked(sender);

            SaveFileDialog dlg = new SaveFileDialog();
            dlg.InitialDirectory = Path.GetDirectoryName(Program.ini_agv.FullName);
            dlg.OverwritePrompt = true;
            dlg.AddExtension = true;
            dlg.Filter = "物流AGV経路設定ファイル (*.LAG)|*.lag|All files (*.*)|*.*";
            if (dlg.ShowDialog(this) == DialogResult.Cancel) return;

            Cursor = Cursors.WaitCursor;

            string err = controller.Save(dlg.FileName);
            if (err != "")
            {
                BL_MessageBox.Show(this, err, this.Text);
            }

            Cursor = Cursors.Default;
        }

        protected override void SubForm_Base_Function07_Clicked(object sender)
        {
            base.SubForm_Base_Function07_Clicked(sender);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = Path.GetDirectoryName(Program.ini_agv.FullName);
            dlg.AddExtension = true;
            dlg.Filter = "物流AGV経路設定ファイル (*.LAG)|*.lag|All files (*.*)|*.*";
            if (dlg.ShowDialog(this) == DialogResult.Cancel) return;

            //BL_IniFile ini = new BL_IniFile(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), Path.GetFileNameWithoutExtension(Application.ExecutablePath) + ".ini"));

            Cursor = Cursors.WaitCursor;

            controller.Load(dlg.FileName);

            ListupFloor();
            ListupMode();

            panelMap.Invalidate();

            Cursor = Cursors.Default;
        }

        #endregion

        #region AGV情報設定操作


        #endregion
    }
}
