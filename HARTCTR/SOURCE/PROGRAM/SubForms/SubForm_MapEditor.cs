using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using BelicsClass.Common;
using BelicsClass.UI;
using BelicsClass.UI.Controls;

namespace PROGRAM
{
    public partial class SubForm_MapEditor : BL_SubForm_Base
    {
        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "[F1]:旋回可能", "[F2]:棚配置可能", "[F3]:棚配置", "[F4]:速度制限", "[F5]:ＱＲ配置", "[F6]:進路設定", "[F7]:読込", "[F8]:保存", "[F9]:ｲﾝﾎﾟｰﾄ", "", "[F11]:吸着", "[F12]:戻る" };
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

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            controller.routeManager.RefreshMap(controller);

            Close();
        }

        AgvControlManager controller = null;
        Dictionary<int, BL_FlatButton> charges = new Dictionary<int, BL_FlatButton>();
        Dictionary<int, BL_FlatButton> stations = new Dictionary<int, BL_FlatButton>();
        Dictionary<int, BL_FlatButton> racks = new Dictionary<int, BL_FlatButton>();
        BL_FlatButton buttonLastCharge = null;
        //int movemode = -1;

        #region コンストラクタ

        public SubForm_MapEditor()
        {
            InitializeComponent();
            controller = Program.controller;

            if (controller != null) controller.applicate_mode = AgvControlManager.enApplicateMode.MAPEDITOR;
        }

        #endregion

        #region フォームロード

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            charges[0] = buttonChargeNorth;
            charges[45] = buttonChargeNorthWest;
            charges[90] = buttonChargeWest;
            charges[135] = buttonChargeSouthWest;
            charges[180] = buttonChargeSouth;
            charges[225] = buttonChargeSouthEast;
            charges[270] = buttonChargeEast;
            charges[315] = buttonChargeNorthEast;

            stations[0] = buttonStationNorth;
            stations[45] = buttonStationNorthWest;
            stations[90] = buttonStationWest;
            stations[135] = buttonStationSouthWest;
            stations[180] = buttonStationSouth;
            stations[225] = buttonStationSouthEast;
            stations[270] = buttonStationEast;
            stations[315] = buttonStationNorthEast;

            racks[0] = buttonRackNorth;
            racks[45] = buttonRackNorthWest;
            racks[90] = buttonRackWest;
            racks[135] = buttonRackSouthWest;
            racks[180] = buttonRackSouth;
            racks[225] = buttonRackSouthEast;
            racks[270] = buttonRackEast;
            racks[315] = buttonRackNorthEast;

            if (controller == null) controller = new AgvControlManager();
            else
            {
                //movemode = controller.movemode_current;
                //if (controller.run_manager != null)
                //{
                //    controller.run_manager.Stop();
                //}
            }

            controller.draw_allfloor = false;
            controller.EventQrSelected += Map_EventSelected;
            controller.EventQrMoved += Map_EventMoved;
            panelMap.MouseWheel += PanelMap_MouseWheel;

            foreach (var num in new List<NumericUpDown>
            { numRX, numRY, numScaleW, numScaleH, numAngle })
            {
                num.ValueChanged += (_s, _e) => panelMap.Invalidate();
            }

            trkOpacity.ValueChanged += (_s, _e) => panelMap.Invalidate();

            panelMap.KeyDown += PanelMap_KeyDown;

            grpBackGround.DataBindings.Add(new Binding
                    (
                        nameof(GroupBox.Visible),
                        編集ToolStripMenuItem,
                        nameof(ToolStripMenuItem.Checked)
                    )
                );

            制限無しToolStripMenuItem.Checked = true;

            grpBackGround.MouseEnter += (_s, _e) =>
            {
                Cursor = Cursors.Default;
            };

            ListupFloor();
            panelMap.Invalidate();

            AttachButton_to_Functions(buttonRackRotatable, 1);
            AttachButton_to_Functions(buttonRackSetable, 2);
            AttachButton_to_Functions(buttonSpeedLimit, 4);
            AttachButton_to_Functions(buttonNewQR, 5);
            AttachButton_to_Functions(buttonNewWay, 6);
            AttachButton_to_Functions(buttonStuck, 11);

            foreach (AgvControlManager.FloorQR.enStatiionType t in Enum.GetValues(typeof(AgvControlManager.FloorQR.enStatiionType)))
            {
                comboBoxStationType.Items.Add(t);
            }

            textBoxZoom.Text = ((int)(controller.draw_scale * 100)).ToString();

            buttonStuck.Checked = true;
            controller.Stuck(buttonStuck.Checked);
            m_Mainform.btnFunctions[11].Checked = buttonStuck.Checked;
        }

        protected override void SubForm_Base_FormClosing(object sender, FormClosingEventArgs e)
        {
            base.SubForm_Base_FormClosing(sender, e);

            controller.LoadAutorator();

            //if (0 <= movemode && controller.run_manager != null)
            //{
            //    controller.run_manager.Start(movemode, controller.interval_current);
            //}
        }

        #endregion

        #region 情報リストアップ

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

                var background = v.Value.mapeditor.background;

                if (!background.RegisteredEventName.Contains(Name))
                {
                    background.PropertyChanged += background_property_changed;
                    background.RegisteredEventName.Add(Name);
                }

                count++;
            }
            listviewFloor.RefreshMe();
            if (0 <= select_index) listviewFloor.SelectedIndices.Add(select_index);
        }

        #endregion

        #region マップ操作イベント

        private void Map_EventMoved(AgvControlManager.FloorQR qr)
        {
            if (qr != null)
            {
                textBoxCurX.Text = ((int)qr.Location.X).ToString();
                textBoxCurY.Text = ((int)qr.Location.Y).ToString();
            }
        }

        private void Map_EventSelected(AgvControlManager.FloorQR qr)
        {
            if (qr != null)
            {
                if (qr.selected)
                {
                    if (escape_checkpoint != null)
                    {
                        escape_checkpoint.escape_to = qr;
                        qr.escape_from = escape_checkpoint;

                        escape_checkpoint = null;
                        panelMap.Invalidate();
                        panelMap.Cursor = Cursors.Default;
                        return;
                    }

                    panelSelectedQR.Enabled = true;
                    panelSelectedQR.Tag = qr;

                    textBoxCurX.Text = ((int)qr.Location.X).ToString();
                    textBoxCurY.Text = ((int)qr.Location.Y).ToString();

                    buttonRackRotatable.Checked = qr.rack_rotatable;
                    buttonRackSetable.Checked = qr.rack_setable;
                    buttonSpeedLimit.Checked = qr.speed_limit != AgvController.enSpeed.H;
                    buttonEscapeCheck.Checked = qr.escape_to != null;

                    //{
                    //    bool found = false;
                    //    foreach (var floor in controller.map.Values)
                    //    {
                    //        if (floor == qr.floor) continue;

                    //        foreach (var qrx in floor.mapeditor.qrs)
                    //        {
                    //            foreach (var qry in qrx.Value.Values)
                    //            {
                    //                if (qry.autorator_id == qr.autorator_id)
                    //                {
                    //                    if (qry.autorator_plc_ip != "" && qry.autorator_plc_port != 0)
                    //                    {
                    //                        qr.autorator_plc_ip = qry.autorator_plc_ip;
                    //                        qr.autorator_plc_port = qry.autorator_plc_port;
                    //                        found = true;
                    //                        break;
                    //                    }
                    //                }
                    //                if (found) break;
                    //            }
                    //            if (found) break;
                    //        }
                    //    }
                    //}

                    textBoxAutolatorID.Text = qr.autorator_id;
                    //textBoxAutolatorIP.Text = qr.autorator_plc_ip;
                    //textBoxAutolatorPort.Text = qr.autorator_plc_port.ToString();
                    buttonAutoLator.Checked = (qr.autorator_id != "");
                    buttonChargeBack.Checked = qr.charge_back;

                    {
                        bool found = false;
                        textBoxChargeDegree.Text = "";
                        foreach (var kv in charges)
                        {
                            kv.Value.Checked = (kv.Key == (int)qr.direction_charge);
                            if (kv.Value.Checked)
                            {
                                textBoxChargeDegree.Text = kv.Key.ToString();
                                found = true;
                            }
                        }

                        if (qr.direction_charge != AgvControlManager.FloorQR.enDirection.NONE)
                        {
                            if (!found) textBoxChargeDegree.Text = ((int)qr.direction_charge).ToString();
                        }
                    }

                    {
                        bool found = false;
                        textBoxStationID.Text = "";
                        textBoxStationDegree.Text = "";
                        textBoxStationIP.Text = "";
                        textBoxStationPort.Text = "";

                        foreach (var kv in stations)
                        {
                            kv.Value.Checked = (kv.Key == (int)qr.direction_station);
                            if (kv.Value.Checked)
                            {
                                textBoxStationDegree.Text = kv.Key.ToString();
                                textBoxStationID.Text = qr.station_id;
                                //textBoxStationIP.Text = qr.station_ip;
                                //textBoxStationPort.Text = qr.station_port.ToString();
                                found = true;
                            }
                        }

                        if (qr.direction_station != AgvControlManager.FloorQR.enDirection.NONE)
                        {
                            if (!found)
                            {
                                textBoxStationDegree.Text = ((int)qr.direction_station).ToString();
                                textBoxStationID.Text = qr.station_id;
                                //textBoxStationIP.Text = qr.station_ip;
                                //textBoxStationPort.Text = qr.station_port.ToString();
                            }
                        }

                        comboBoxStationType.Text = qr.station_type.ToString();
                    }

                    lock (qr)
                    {
                        if (qr.rack != null)
                        {
                            textBoxRackNo.Text = qr.rack.rack_no;

                            textBoxRackFaceNo.Text = "";
                            buttonRackDelete.Checked = false;
                            foreach (var kv in racks)
                            {
                                bool found = false;
                                foreach (var r in qr.rack.face_id)
                                {
                                    if (kv.Key == r.Key)
                                    {
                                        found = true;
                                        break;
                                    }
                                }

                                kv.Value.BackColorNormal = found ? Color.Orange : Color.FromArgb(64, 64, 64);
                            }
                        }
                        else
                        {
                            textBoxRackNo.Text = "";
                            foreach (var kv in racks)
                            {
                                kv.Value.BackColorNormal = Color.FromArgb(64, 64, 64);
                            }
                        }

                        foreach (var v in listviewFloor.Items)
                        {
                            if (v[1].ToString() == qr.floor.code)
                            {
                                v[2] = qr.floor.mapeditor.Count;
                                v[3] = "";
                                listviewFloor.NeedRedraw = true;
                            }
                        }
                    }

                    listviewFloor.RefreshMe(false);
                }
                else
                {
                    textBoxCurX.Text = "";
                    textBoxCurY.Text = "";

                    buttonAutoLator.Checked = false;
                    buttonChargeBack.Checked = false;
                    buttonRackRotatable.Checked = false;
                    buttonRackRotatable.Checked = false;
                    buttonRackSetable.Checked = false;

                    foreach (var kv in charges) kv.Value.Checked = false;
                    foreach (var kv in stations) kv.Value.Checked = false;
                    textBoxChargeDegree.Text = "";

                    textBoxStationDegree.Text = "";
                    comboBoxStationType.SelectedIndex = -1;

                    panelSelectedQR.Tag = null;
                    panelSelectedQR.Enabled = false;

                    buttonLastCharge = null;
                    //buttonLastStation = null;

                    buttonEscapeCheck.Checked = false;
                }
            }
            else
            {
            }

            m_Mainform.btnFunctions[1].Checked = buttonRackRotatable.Checked;
            m_Mainform.btnFunctions[2].Checked = buttonRackSetable.Checked;
            m_Mainform.btnFunctions[4].Checked = buttonSpeedLimit.Checked;
        }

        private void panelMap_Paint(object sender, PaintEventArgs e)
        {
            var sw = new BL_Stopwatch();
            sw.Restart();

            controller.draw_size_pixel.Width = panelMap.Width;
            controller.draw_size_pixel.Height = panelMap.Height;

            controller.Draw(e.Graphics);
        }

        #endregion

        #region コンテキストメニュー操作イベント

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {
            if (!controller.map.ContainsKey(controller.selected_floor))
            {
                e.Cancel = true;
                return;
            }

            if (buttonNewQR.Checked && !buttonNewWay.Checked)
            {
                e.Cancel = true;
                return;
            }

            if (controller.is_erasing_ways)
            {
                e.Cancel = true;
                return;
            }

            背景画像ToolStripMenuItem.Enabled = !(buttonNewQR.Checked || buttonNewWay.Checked);

            var deleteEnabled = false;

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                var background = floor.mapeditor.background;

                deleteEnabled = background.image != null && !background.Editing;
            }

            削除ToolStripMenuItem.Enabled = deleteEnabled;
        }

        #region 背景画像操作イベント

        private void PanelMap_KeyDown(object sender, KeyEventArgs e)
        {
            var floor = controller.SelectFloor();
            if (floor != null && panelMap.Focused)
            {
                var background = floor.mapeditor.background;

                if (background.Editing)
                {
                    background.KeyDown(e);
                }

                panelMap.Invalidate();
            }
        }

        private void buttonSetCoordinate_Click(object sender, EventArgs e)
        {
            SetCoordinate();
        }

        private void SetCoordinate()
        {
            var floor = controller.SelectFloor();

            if (floor?.mapeditor?.background?.image == null) return;

            var background = floor.mapeditor.background;

            var main = new MainFormDialog();
            var sub = new SubForm_SetCoordinate
            {
                Image = background.image,
                RX = background.BackGroundRX,
                RY = background.BackGroundRY,
                Scale = background.BackGroundScaleW,
                Angle = background.BackGroundAngle,
                BackGround = background
            };

            var w = 1040;
            var h = 784;

            main.Load += (s, e) =>
            {
                main.Left = m_Mainform.Left + (m_Mainform.Width - w) / 2;
                main.Top = m_Mainform.Top + (m_Mainform.Height - h) / 2;
                main.Width = w;
                main.Height = h;
                main.CaptionText = sub.TitleText;
                main.Text = sub.TitleText;

                sub.ShowMe(main);
            };

            main.Show();
        }

        private void 読込ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var center = panelMap.PointToClient(new Point(contextMenuStrip1.Left, contextMenuStrip1.Top));

            var floor = controller.SelectFloor();

            var loaded = floor?.mapeditor.background.Load(center);

            if (loaded.HasValue && loaded.Value)
            {
                編集ToolStripMenuItem.Checked = true;
                changeBackGroundEditing(true);
            }
        }

        private void 編集ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;

            var editing = item.Checked = !item.Checked;

            changeBackGroundEditing(editing);
        }

        private void 編集ToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
        {
            var item = sender as ToolStripMenuItem;

            var is_checked = item.Checked;

            foreach (var btn in m_Mainform.btnFunctions)
            {
                var enabled = string.IsNullOrEmpty(btn.Text) ? false : !is_checked;

                btn.Enabled = enabled;
            }
        }

        private void 削除ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var floor = controller.SelectFloor();

            if (floor != null)
            {
                var background = floor.mapeditor.background;

                if (background.image != null && !background.Editing)
                {
                    var result = BL_MessageBox.Show(this,
                        "背景画像を削除します。よろしいですか？",
                        "確認", MessageBoxButtons.YesNo);

                    if (result == DialogResult.Yes)
                    {
                        background.Delete();
                    }
                }
            }
        }

        private void checkBoxRetendAspect_CheckedChanged(object sender, EventArgs e)
        {
            var chk = sender as CheckBox;

            var is_checked = chk.Checked;

            controller.is_retending_aspect = is_checked;
        }

        private void changeBackGroundEditing(bool editing)
        {
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                var editor = floor.mapeditor;

                controller.is_editing_background = editing;
                if (editing) editor.selected_qr?.Select(false);
            }
        }

        private void change_background_property_bindings(AgvControlManager.FloorMap floor)
        {
            var num_combs = new Dictionary<NumericUpDown, string>
            {
                { numRX, nameof(AgvControlManager.BackGround.BackGroundRX) },
                { numRY, nameof(AgvControlManager.BackGround.BackGroundRY) },
                { numScaleW, nameof(AgvControlManager.BackGround.BackGroundScaleW) },
                { numScaleH, nameof(AgvControlManager.BackGround.BackGroundScaleH) },
                { numAngle, nameof(AgvControlManager.BackGround.BackGroundAngle) },
            };

            foreach (var num in num_combs.Keys)
            {
                num.DataBindings.Clear();
            }

            trkOpacity.DataBindings.Clear();

            if (floor != null)
            {
                foreach (var comb in num_combs)
                {
                    var num = comb.Key;
                    var prop = comb.Value;

                    num.DataBindings.Add(new Binding
                        (
                        nameof(NumericUpDown.Value),
                        floor.mapeditor.background,
                        prop,
                        false,
                        DataSourceUpdateMode.OnPropertyChanged
                        )
                    );
                }

                trkOpacity.DataBindings.Add(new Binding
                    (
                    nameof(TrackBar.Value),
                    floor.mapeditor.background,
                    nameof(AgvControlManager.BackGround.BackGroundOpacity),
                    false,
                    DataSourceUpdateMode.OnPropertyChanged
                    )
                );
            }
        }

        private void remove_background_property_changed_event()
        {

        }

        private void background_property_changed(object sender, PropertyChangedEventArgs e)
        {

        }

        #endregion

        #region 進路種別イベント

        private void RouteTypeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sender_item = sender as ToolStripMenuItem;

            sender_item.Checked = true;

            var route_type_items = new Dictionary<ToolStripMenuItem, AgvControlManager.FloorQR.RouteType>
            {
                { 制限無しToolStripMenuItem, AgvControlManager.FloorQR.RouteType.NONE },
                { 長手方向ToolStripMenuItem, AgvControlManager.FloorQR.RouteType.FAT },
                { 短手方向ToolStripMenuItem, AgvControlManager.FloorQR.RouteType.THIN }
            };

            foreach (var kv in route_type_items)
            {
                var item = kv.Key;
                var route_type = kv.Value;

                if (item == sender_item)
                {
                    controller.route_type = route_type;
                }
                else
                {
                    item.Checked = false;
                }
            }
        }

        #endregion

        #endregion

        #region 画面操作イベント

        private void buttonFloorAdd_Click(object sender, EventArgs e)
        {
            string code = textBoxFloorCode.Text;
            if (1 != code.Length)
            {
                BL_MessageBox.Show(this, "フロアコードを入力してください。", "エラー");
                return;
            }

            foreach (var v in listviewFloor.Items)
            {
                if (v[1].ToString() == code)
                {
                    BL_MessageBox.Show(this, "フロアコードが重複しています。", "エラー");
                    return;
                }
            }

            controller.AddFloor(code);
            ListupFloor();

            if (listviewFloor.SelectedIndices.Count == 0)
            {
                listviewFloor.SelectedIndices.Add(0);
            }
        }

        private void buttonFloorDel_Click(object sender, EventArgs e)
        {
            if (listviewFloor.SelectedIndices.Count == 0) return;

            if (BL_MessageBox.Show(this, "フロアを削除します。配置されているAGVも削除されます。\nよろしいですか？", "確認", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                if (floor.mapeditor.is_new_qr) floor.mapeditor.NewQR(false);
                if (floor.mapeditor.is_new_way) floor.mapeditor.NewWay(false);
            }

            buttonNewQR.Checked = false;
            buttonNewWay.Checked = false;

            string code = listviewFloor.SortedItems[listviewFloor.SelectedIndices[0]][1].ToString();
            controller.RemoveFloor(code);

            ListupFloor();

            panelMap.Invalidate();
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

        private void textBoxCurX_Leave(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr != null)
            {
                int x = 0;
                if (int.TryParse(textBoxCurX.Text, out x))
                {
                    if (qr.Location.X != x)
                    {
                        //qr.floor.mapeditor.RemoveQR(qr);
                        qr.Location = new Point(x, qr.Location.Y);
                        //qr.floor.mapeditor.AddQR(qr);
                        panelMap.Invalidate();
                    }
                }
            }
        }

        private void textBoxCurY_Leave(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr != null)
            {
                int y = 0;
                if (int.TryParse(textBoxCurY.Text, out y))
                {
                    if (qr.Location.Y != y)
                    {
                        //qr.floor.mapeditor.RemoveQR(qr);
                        qr.Location = new Point(qr.Location.X, y);
                        //qr.floor.mapeditor.AddQR(qr);
                        panelMap.Invalidate();
                    }
                }
            }
        }

        private void buttonNewQR_Click(object sender, EventArgs e)
        {
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.mapeditor.NewQR(buttonNewQR.Checked);
                panelMap.Invalidate();
            }
        }

        private void buttonNewWay_Click(object sender, EventArgs e)
        {
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.mapeditor.NewWay(buttonNewWay.Checked);
                panelMap.Invalidate();
            }

            buttonFloorWarp.Enabled = buttonNewWay.Checked;
        }

        private void buttonAutoLator_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr != null)
            {
                if (buttonAutoLator.Checked)
                {
                    string id = textBoxAutolatorID.Text.Trim();
                    if (id == "")
                    {
                        BL_MessageBox.Show(this, "オートレーターIDを入力してください。", "エラー");
                        buttonAutoLator.Checked = false;
                        return;
                    }

                    foreach (var qrx in qr.floor.mapeditor.qrs)
                    {
                        foreach (var qry in qrx.Value)
                        {
                            if (qry.Value.autorator_id == id)
                            {
                                BL_MessageBox.Show(this, "同一フロア内でオートレーターIDが重複しています。", "エラー");
                                buttonAutoLator.Checked = false;
                                return;
                            }
                        }
                    }

                    qr.autorator_id = id;

                //    string ip = textBoxAutolatorIP.Text.Trim();
                //    int port = 0; int.TryParse(textBoxAutolatorPort.Text, out port);

                //    //if (ip == "")
                //    //{
                //    //    BL_MessageBox.Show(this, "オートレーターIPアドレスを入力してください。", "エラー");
                //    //    return;
                //    //}

                //    //if (port == 0)
                //    //{
                //    //    BL_MessageBox.Show(this, "オートレーターPortを入力してください。", "エラー");
                //    //    return;
                //    //}

                //    qr.autorator_plc_ip = ip;
                //    qr.autorator_plc_port = port;

                //    //他フロア同一IDオートレータのIPアドレスとポートNoを設定する
                //    foreach (var floor in controller.map.Values)
                //    {
                //        if (floor == qr.floor) continue;

                //        foreach (var qrx in floor.mapeditor.qrs)
                //        {
                //            foreach (var qry in qrx.Value.Values)
                //            {
                //                if (qry.autorator_id == qr.autorator_id)
                //                {
                //                    if (qry.autorator_plc_ip != "" && qry.autorator_plc_port != 0)
                //                    {
                //                        if (ip == "" || port == 0)
                //                        {
                //                            textBoxAutolatorIP.Text = qry.autorator_plc_ip;
                //                            textBoxAutolatorPort.Text = qry.autorator_plc_port.ToString();
                //                            ip = qry.autorator_plc_ip;
                //                            port = qry.autorator_plc_port;
                //                        }
                //                        else
                //                        {
                //                            qry.autorator_plc_ip = ip;
                //                            qry.autorator_plc_port = port;
                //                        }
                //                    }
                //                }
                //            }
                //        }
                //    }
                }
                else
                {
                    qr.autorator_id = "";
                //    qr.autorator_plc_ip = "";
                //    qr.autorator_plc_port = 0;
                }

                //qr.is_autorator = buttonAutoLator.Checked;
                panelMap.Invalidate();
            }
        }

        private void buttonChargeBack_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr != null)
            {
                if (qr.charge_back != buttonChargeBack.Checked)
                {
                    qr.charge_back = buttonChargeBack.Checked;
                    panelMap.Invalidate();
                }
            }
        }

        private void buttonStation_Click(object sender, EventArgs e)
        {
            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr != null)
            {
                if (!button.Checked)
                {
                    //buttonLastStation = null;
                    qr.direction_station = AgvControlManager.FloorQR.enDirection.NONE;
                    qr.station_id = "";
                    //qr.station_ip = "";
                    //qr.station_port = 0;
                    qr.station_type = AgvControlManager.FloorQR.enStatiionType.STATION;
                    textBoxStationDegree.Text = "";
                }
                else
                {
                    if (textBoxStationID.Text == "")
                    {
                        BL_MessageBox.Show(this, "ステーションIDを入力してください。", "エラー");
                        button.Checked = false;
                        return;
                    }

                    {
                        bool found = false;
                        foreach (var floor in controller.map.Values)
                        {
                            foreach (var qrx in floor.mapeditor.qrs)
                            {
                                foreach (var qry in qrx.Value.Values)
                                {
                                    if (qry == qr) continue;

                                    if (qry.station_id == textBoxStationID.Text)
                                    {
                                        found = true;
                                        break;
                                    }
                                }
                                if (found) break;
                            }
                            if (found) break;
                        }

                        if (found)
                        {
                            BL_MessageBox.Show(this, "ステーションIDが重複しています。", "エラー");
                            button.Checked = false;
                            return;
                        }
                    }

                    //string ip = textBoxStationIP.Text.Trim();
                    //if (ip == "")
                    //{
                    //    BL_MessageBox.Show(this, "ステーションIPアドレスを入力してください。", "エラー");
                    //    button.Checked = false;
                    //    return;
                    //}

                    //int port = 0; int.TryParse(textBoxStationPort.Text, out port);
                    //if (port == 0)
                    //{
                    //    BL_MessageBox.Show(this, "ステーションPortを入力してください。", "エラー");
                    //    button.Checked = false;
                    //    return;
                    //}

                    //buttonLastStation = button;

                    foreach (var kv in stations)
                    {
                        if (kv.Value == button)
                        {
                            qr.direction_station = (AgvControlManager.FloorQR.enDirection)kv.Key;
                            qr.station_id = textBoxStationID.Text;
                            //qr.station_ip = ip;
                            //qr.station_port = port;
                            qr.station_type = AgvControlManager.FloorQR.enStatiionType.STATION;

                            textBoxStationDegree.Text = kv.Key.ToString();
                        }
                        else
                        {
                            kv.Value.Checked = false;
                        }
                    }
                }
                panelMap.Invalidate();
            }
        }

        private void buttonRack_Click(object sender, EventArgs e)
        {
            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr != null)
            {
                foreach (var kv in racks)
                {
                    if (kv.Value == button)
                    {
                        if (buttonRackDelete.Checked)
                        {
                            if (qr.rack != null)
                            {
                                if (qr.rack.face_id.ContainsKey(kv.Key))
                                {
                                    qr.rack.face_id.Remove(kv.Key);
                                }
                            }
                        }
                        else
                        {
                            if (textBoxRackNo.Text == "")
                            {
                                BL_MessageBox.Show(this, "棚Noを入力してください。", "エラー");
                                button.Checked = false;
                                return;
                            }

                            bool found = false;
                            foreach (var v in controller.map.Values)
                            {
                                foreach (var vv in v.mapeditor.qrs.Values)
                                {
                                    foreach (var vvv in vv.Values)
                                    {
                                        if (vvv == qr) continue;
                                        if (vvv.rack != null && vvv.rack.rack_no == textBoxRackNo.Text.Trim())
                                        {
                                            found = true;
                                            break;
                                        }
                                    }
                                    if (found) break;
                                }
                                if (found) break;
                            }

                            if (found)
                            {
                                BL_MessageBox.Show(this, "棚Noが重複しています", "エラー");
                                return;
                            }

                            if (qr.rack == null) qr.rack = new AgvControlManager.Rack();

                            if (qr.rack.face_id.ContainsKey(kv.Key))
                            {
                                textBoxRackFaceNo.Text = qr.rack.face_id[kv.Key];
                            }
                            else if (textBoxRackFaceNo.Text != "")
                            {
                                found = false;
                                foreach (var r in qr.rack.face_id)
                                {
                                    if (r.Value == textBoxRackFaceNo.Text)
                                    {
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    qr.rack.rack_no = textBoxRackNo.Text;
                                    qr.rack.face_id[kv.Key] = textBoxRackFaceNo.Text;
                                    qr.rack.can_inout[kv.Key] = true;
                                    textBoxRackFaceNo.SelectAll();
                                    textBoxRackFaceNo.Focus();
                                }
                                else
                                {
                                    BL_MessageBox.Show(this, "面Noが重複しています。", "エラー");
                                    if (qr.rack.face_id.Count == 0) qr.rack = null;
                                }
                            }
                            else
                            {
                                BL_MessageBox.Show(this, "面Noを入力してください。", "エラー");
                                if (qr.rack.face_id.Count == 0) qr.rack = null;
                            }
                        }

                        if (qr.rack != null)
                        {
                            button.BackColorNormal = qr.rack.face_id.ContainsKey(kv.Key) ? Color.Orange : Color.FromArgb(64, 64, 64);

                            if (qr.rack.face_id.Count == 0)
                            {
                                buttonRackDelete.Checked = false;
                                qr.rack = null;
                            }
                        }
                    }
                }

                panelMap.Invalidate();
            }
        }

        private void buttonRackSetable_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr != null)
            {
                if (qr.autorator_id == "")
                {
                    qr.rack_setable = buttonRackSetable.Checked;
                }
                else
                {
                    qr.rack_setable = buttonRackSetable.Checked = false;
                }

                panelMap.Invalidate();
            }
        }

        private void buttonCharge_Click(object sender, EventArgs e)
        {
            BL_FlatButton button = sender as BL_FlatButton;
            if (button == null) return;

            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr != null)
            {
                if (!button.Checked)
                {
                    buttonLastCharge = null;
                    qr.direction_charge = AgvControlManager.FloorQR.enDirection.NONE;
                    textBoxChargeDegree.Text = "";
                }
                else
                {
                    buttonLastCharge = button;

                    foreach (var kv in charges)
                    {
                        if (kv.Value == button)
                        {
                            qr.direction_charge = (AgvControlManager.FloorQR.enDirection)kv.Key;
                            textBoxChargeDegree.Text = kv.Key.ToString();
                        }
                        else
                        {
                            kv.Value.Checked = false;
                        }
                    }
                }
                panelMap.Invalidate();
            }
        }

        private void buttonSpeedLimit_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr != null)
            {
                qr.speed_limit = buttonSpeedLimit.Checked ? AgvController.enSpeed.M : AgvController.enSpeed.H;

                panelMap.Invalidate();
            }
        }

        private void textBoxChargeDegree_Leave(object sender, EventArgs e)
        {
            //if (buttonLastCharge == null) return;

            int degree = 0;
            if (!int.TryParse(textBoxChargeDegree.Text, out degree)) return;

            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr != null)
            {
                qr.direction_charge = (AgvControlManager.FloorQR.enDirection)degree;
            }
            panelMap.Invalidate();
        }

        private void textBoxStationDegree_Leave(object sender, EventArgs e)
        {
            //if (buttonLastStation == null) return;

            int degree = 0;
            if (!int.TryParse(textBoxStationDegree.Text, out degree)) return;

            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr != null)
            {
                qr.direction_station = (AgvControlManager.FloorQR.enDirection)degree;
            }
            panelMap.Invalidate();
        }

        private void listviewFloor_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listviewFloor.SelectedIndices.Count == 0) return;

            var floor = controller.SelectFloor();
            //if (floor != null)
            //{
            //    if (floor.mapeditor.is_new_qr) floor.mapeditor.NewQR(false);
            //    if (floor.mapeditor.is_new_way) floor.mapeditor.NewWay(false);
            //}

            string code = listviewFloor.SortedItems[listviewFloor.SelectedIndices[0]][1].ToString();
            
            floor = controller.SelectFloor(code);
            if (floor != null)
            {
                floor.mapeditor.is_new_qr = buttonNewQR.Checked;
                floor.mapeditor.is_new_way = buttonNewWay.Checked;
                floor.mapeditor.is_qr_editable = buttonQREditable.Checked;

                checkBoxFA.Checked = floor.is_fa;
                textBoxFloorCode.Text = floor.code;

                change_background_property_bindings(floor);
            }

            panelMap.Invalidate();
        }

        private void checkBoxFA_CheckedChanged(object sender, EventArgs e)
        {
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.is_fa = checkBoxFA.Checked;
                panelMap.Invalidate();
            }
        }

        private void buttonFloorWarp_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr != null)
            {
                if (buttonFloorWarp.Checked)
                {
                    if (controller.floorwarp_qr != null)
                    {
                    }

                    controller.floorwarp_qr = qr;
                }
                else
                {
                    controller.floorwarp_qr = null;

                    foreach (var v in qr.floorwarp_qr)
                    {
                        if (v != qr)
                        {
                            if (v.floorwarp_qr.Contains(qr))
                            {
                                v.floorwarp_qr.Remove(qr);
                            }
                        }
                    }

                    qr.floorwarp_qr.Clear();
                }
            }
            else
            {
                controller.floorwarp_qr = null;
                buttonFloorWarp.Checked = false;
            }
        }

        private void textBoxStuckDistance_TextChanged(object sender, EventArgs e)
        {
            int.TryParse(textBoxStuckDistance.Text, out controller.stuck_base);
        }

        private void buttonQREditable_Click(object sender, EventArgs e)
        {
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.mapeditor.is_qr_editable = buttonQREditable.Checked;
                panelMap.Invalidate();
            }
        }

        #endregion

        #region マウス操作イベント

        private void PanelMap_MouseWheel(object sender, MouseEventArgs e)
        {
            controller.MouseWheel(e);
            panelMap.Invalidate();

            textBoxZoom.Text = ((int)(controller.draw_scale * 100)).ToString();
        }

        private void PanelMap_MouseMove(object sender, MouseEventArgs e)
        {
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.mapeditor.MouseMove(e);

                if (controller.is_editing_background)
                {
                    Cursor = controller.panel_cursor;
                }
                else
                {
                    Cursor = Cursors.Default;
                }

                panelMap.Invalidate();

                textBoxX.Text = ((int)controller.mousePoint.X).ToString();
                textBoxY.Text = ((int)controller.mousePoint.Y).ToString();
            }
        }

        private void PanelMap_MouseDown(object sender, MouseEventArgs e)
        {
            panelMap.Focus();
            controller.is_erasing_ways = false;

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.mapeditor.MouseDown(e);
                panelMap.Invalidate();
            }
        }

        private void PanelMap_MouseUp(object sender, MouseEventArgs e)
        {
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.mapeditor.MouseUp(e);
                panelMap.Invalidate();
            }
        }

        private void PanelMap_MouseClick(object sender, MouseEventArgs e)
        {
            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.mapeditor.MouseClick(e);
                panelMap.Invalidate();
            }
        }

        #endregion

        #region 補助イベント

        private void num_Enter(object sender, EventArgs e)
        {
            var num = sender as NumericUpDown;

            num.Select(0, num.Text.Length);
        }

        #endregion

        #region ファンクションキー操作

        protected override void SubForm_Base_Function01_Clicked(object sender)
        {
            base.SubForm_Base_Function01_Clicked(sender);

            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr != null)
            {
                qr.rack_rotatable = buttonRackRotatable.Checked;
                panelMap.Invalidate();
            }
        }

        protected override void SubForm_Base_Function02_Clicked(object sender)
        {
            base.SubForm_Base_Function02_Clicked(sender);

            buttonRackSetable_Click(sender, null);
        }

        protected override void SubForm_Base_Function03_Clicked(object sender)
        {
            base.SubForm_Base_Function03_Clicked(sender);

            if (textBoxRackNo.Text == "")
            {
                BL_MessageBox.Show(this, "棚Noを入力してください。", "エラー");
                return;
            }

            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr == null) return;

            if (qr.rack == null)
            {
                bool found = false;
                foreach (var v in controller.map.Values)
                {
                    foreach (var vv in v.mapeditor.qrs.Values)
                    {
                        foreach (var vvv in vv.Values)
                        {
                            if (vvv == qr) continue;
                            if (vvv.rack != null)
                            {
                                if (vvv.rack.rack_no == textBoxRackNo.Text.Trim())
                                {
                                    found = true;
                                    break;
                                }
                            }
                        }
                        if (found) break;
                    }
                    if (found) break;
                }

                if (found)
                {
                    BL_MessageBox.Show(this, "棚Noが重複しています", "エラー");
                    return;
                }

                //qr.rack = new AgvControlManager.Rack();
                //qr.rack.rack_no = textBoxRackNo.Text.Trim();
                //qr.rack.face_id.Clear();
                //qr.rack.face_id[0] = "1";
                //qr.rack.face_id[90] = "2";
                //qr.rack.face_id[180] = "3";
                //qr.rack.face_id[270] = "4";
                //qr.rack.can_inout[0] = true;
                //qr.rack.can_inout[90] = true;
                //qr.rack.can_inout[180] = true;
                //qr.rack.can_inout[270] = true;

                var list = RackMaster.Instance.GetRackList().Where(e => e.rack_no == textBoxRackNo.Text.Trim());
                if (0 < list.Count())
                {
                    qr.rack = RackMaster.Instance.LoadRack(textBoxRackNo.Text.Trim());
                }
                else
                {
                    qr.rack = RackMaster.Instance.LoadRack(textBoxRackNo.Text.PadRight(1).Substring(0, 1));
                    qr.rack.rack_no = textBoxRackNo.Text.Trim();
                }
            }
            else
            {
                qr.rack.rack_no = "";
                qr.rack.face_id.Clear();
                qr.rack = null;
            }

            panelMap.Invalidate();
        }

        protected override void SubForm_Base_Function04_Clicked(object sender)
        {
            base.SubForm_Base_Function04_Clicked(sender);

            buttonSpeedLimit_Click(sender, null);
        }

        protected override void SubForm_Base_Function05_Clicked(object sender)
        {
            base.SubForm_Base_Function05_Clicked(sender);

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.mapeditor.NewQR(m_Mainform.btnFunctions[5].Checked);
                panelMap.Invalidate();
            }
        }

        protected override void SubForm_Base_Function06_Clicked(object sender)
        {
            base.SubForm_Base_Function06_Clicked(sender);

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.mapeditor.NewWay(m_Mainform.btnFunctions[6].Checked);
                panelMap.Invalidate();
            }

            buttonFloorWarp.Enabled = m_Mainform.btnFunctions[6].Checked;
        }

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

            //BL_IniFile ini = new BL_IniFile(Path.Combine(Path.GetDirectoryName(Program.ini_agv.FullName), Path.GetFileNameWithoutExtension(Program.ini_agv.FullName) + ".ini"));

            Cursor = Cursors.WaitCursor;

            controller.Load(dlg.FileName);

            ListupFloor();
            panelMap.Invalidate();

            var floor = controller.SelectFloor();
            if (floor != null)
            {
                floor.mapeditor.is_qr_editable = buttonQREditable.Checked;
            }

            Cursor = Cursors.Default;
        }

        protected override void SubForm_Base_Function09_Clicked(object sender)
        {
            base.SubForm_Base_Function09_Clicked(sender);

            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = Path.GetDirectoryName(Program.ini_agv.FullName);
            dlg.AddExtension = true;
            dlg.Filter = "CSVファイル (*.CSV)|*.csv|All files (*.*)|*.*";
            if (dlg.ShowDialog(this) == DialogResult.Cancel) return;

            Cursor = Cursors.WaitCursor;

            controller.Import(dlg.FileName);

            ListupFloor();

            panelMap.Invalidate();
            Cursor = Cursors.Default;
        }

        protected override void SubForm_Base_Function11_Clicked(object sender)
        {
            base.SubForm_Base_Function11_Clicked(sender);

            controller.Stuck(buttonStuck.Checked);
        }

        #endregion

        private void comboBoxStationType_SelectedIndexChanged(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr == null) return;

            if (qr.station_id == "")
            {
                comboBoxStationType.SelectedIndex = -1;
                return;
            }

            if (comboBoxStationType.SelectedItem != null) qr.station_type = (AgvControlManager.FloorQR.enStatiionType)comboBoxStationType.SelectedItem;
        }

        AgvControlManager.FloorQR escape_checkpoint = null;

        private void buttonEscapeCheck_Click(object sender, EventArgs e)
        {
            AgvControlManager.FloorQR qr = panelSelectedQR.Tag as AgvControlManager.FloorQR;
            if (qr == null) return;

            if (buttonEscapeCheck.Checked)
            {
                escape_checkpoint = qr;
                escape_checkpoint.escape_to = qr;

                panelMap.Cursor = Cursors.NoMove2D;
            }
            else
            {
                if (qr.escape_to != null)
                {
                    qr.escape_to.escape_from = null;
                    qr.escape_to = null;
                    
                    escape_checkpoint = null;
                }
            }

            panelMap.Invalidate();
        }
    }
}
