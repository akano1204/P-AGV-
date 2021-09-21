using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BelicsClass.Common;
using BelicsClass.UI;
using BelicsClass.File;
using BelicsClass.UI.Controls;

namespace PROGRAM
{
    using DB = AgvDatabase;

    public partial class SubForm_AgvStateMonitor : BelicsClass.UI.BL_SubForm_Base
    {
        AgvControlManager controller = null;

        #region サブフォーム

        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "", "", "", "", "[F5]:再表示", "", "", "", "", "", "", "[F12]:戻る" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get { return this.Text; }
        }

        protected override void SubForm_Base_Function05_Clicked(object sender)
        {
            base.SubForm_Base_Function05_Clicked(sender);

            Disp();
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            Close();
        }

        #endregion

		#region コンストラクタ

		public SubForm_AgvStateMonitor()
        {
            InitializeComponent();

            this.controller = Program.controller;

            SetHeaders();

            Resizer_Initialize();
        }

        #endregion

        #region ロード

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            panelMap.MouseWheel += PanelMap_MouseWheel;
            panelMap.Paint += panelMap_Paint;
            panelMap.MouseDown += panelMap_MouseDown;
            panelMap.MouseMove += panelMap_MouseMove;
            panelMap.MouseUp += panelMap_MouseUp;
            controller.EventAgvSelected += Map_EventAgvSelected;

            controller.draw_status_mode = true;
            controller.applicate_mode = AgvControlManager.enApplicateMode.AUTO_COMMANDER;
        }

        #endregion

        protected override void SubForm_Base_FormClosing(object sender, FormClosingEventArgs e)
        {
            panelMap.MouseWheel -= PanelMap_MouseWheel;
            panelMap.Paint -= panelMap_Paint;
            panelMap.MouseDown -= panelMap_MouseDown;
            panelMap.MouseMove -= panelMap_MouseMove;
            panelMap.MouseUp -= panelMap_MouseUp;
            controller.EventAgvSelected -= Map_EventAgvSelected;

            base.SubForm_Base_FormClosing(sender, e);

            controller.draw_status_mode = false;
        }

        private void Map_EventAgvSelected(AgvControlManager.FloorAGV agv)
        {
            if (agv != null)
            {
            }
        }

        #region メソッド

        private void SetHeaders()
        {
            LV1.Items.Clear();
            LV1.Columns.Clear();

            LV1.Columns.AddRange
                (new ColumnHeader[]
                {
                    new ColumnHeader { Text = "NONE", Width = 0 },
                    new ColumnHeader { Text = "NO", Width = 50 },
                    new ColumnHeader { Text = "最終ｺﾏﾝﾄﾞ時間", Width = 170 },
                    new ColumnHeader { Text = "運航状態", Width = 72, TextAlign = HorizontalAlignment.Center },
                    new ColumnHeader { Text = "指示親番", Width = 70, TextAlign = HorizontalAlignment.Right },
                    new ColumnHeader { Text = "指示子番", Width = 70, TextAlign = HorizontalAlignment.Right },
                    new ColumnHeader { Text = "充電残量", Width = 70, TextAlign = HorizontalAlignment.Right },
                    new ColumnHeader { Text = "ﾊﾞｯﾃﾘｰ状態", Width = 83, TextAlign = HorizontalAlignment.Center },
                    new ColumnHeader { Text = "動作ﾓｰﾄﾞ", Width = 66, TextAlign = HorizontalAlignment.Center },
                    new ColumnHeader { Text = "現在ﾏｯﾌﾟ", Width = 66, TextAlign = HorizontalAlignment.Center },
                    new ColumnHeader { Text = "X", Width = 60, TextAlign = HorizontalAlignment.Right },
                    new ColumnHeader { Text = "Y", Width = 60, TextAlign = HorizontalAlignment.Right },
                    new ColumnHeader { Text = "棚ID", Width = 60 },
                    new ColumnHeader { Text = "異常ｺｰﾄﾞ", Width = 68, TextAlign = HorizontalAlignment.Center },
                }
                );

            Disp();
        }

        void Disp()
        {
            var error = "";

            LV1.ItemsClear();

            error = DB.AGV_STATE.Select(out List<DB.AGV_STATE> rs);

            foreach (var r in rs)
            {
                var item = new BL_VirtualListView.BL_VirtualListViewItem(r);

                BL_VirtualListView.BL_LvwCell cell;

                cell = new BL_VirtualListView.BL_LvwCell(r.AGV_ID);
                item.Add(cell);
                cell = new BL_VirtualListView.BL_LvwCell(r.COMMAND_TIME.ToString());
                item.Add(cell);
                cell = new BL_VirtualListView.BL_LvwCell(BL_EnumLabel.GetLabel(r.ACTION));
                item.Add(cell);
                cell = new BL_VirtualListView.BL_LvwCell(r.ORDER_NO.ToString());
                item.Add(cell);
                cell = new BL_VirtualListView.BL_LvwCell(r.ORDER_SUB_NO.ToString());
                item.Add(cell);
                cell = new BL_VirtualListView.BL_LvwCell(r.BATTERY.ToString());
                item.Add(cell);
                cell = new BL_VirtualListView.BL_LvwCell(BL_EnumLabel.GetLabel(r.BATTERY_STATE));
                item.Add(cell);
                cell = new BL_VirtualListView.BL_LvwCell(BL_EnumLabel.GetLabel(r.ACTION_MODE));
                item.Add(cell);
                cell = new BL_VirtualListView.BL_LvwCell(r.MAP);
                item.Add(cell);
                cell = new BL_VirtualListView.BL_LvwCell(r.LOCATION_X);
                item.Add(cell);
                cell = new BL_VirtualListView.BL_LvwCell(r.LOCATION_Y);
                item.Add(cell);
                cell = new BL_VirtualListView.BL_LvwCell(r.RACK_ID);
                item.Add(cell);
                cell = new BL_VirtualListView.BL_LvwCell(r.ERR_CODE == 0 ? "" : r.ERR_CODE.ToString());
                cell.BackColor = r.ERR_CODE != 0 ? Color.Red : Color.White;
                cell.ForeColor = r.ERR_CODE != 0 ? Color.White : Color.Black;
                item.Add(cell);

                item.Tag = r;
                LV1.Items.Add(item);
            }

            LV1.RefreshMe(false);

            panelMap.Invalidate();
        }

        #endregion

        private void LV1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (0 < LV1.SelectedItems.Count)
            {
                DB.AGV_STATE r = LV1.SelectedItems[0].Tag as DB.AGV_STATE;
                AgvControlManager.FloorAGV agv = controller.agvs.Where(ee => ee.id.Trim() == r.AGV_ID.Trim()).FirstOrDefault();
                if (agv != null)
                {
                    agv.Select(true);

                    AgvControlManager.FloorMap floor = floor = agv.floor;

                    float fw = floor.rW(panelMap.Width);
                    float fh = floor.rH(panelMap.Height);

                    float fx = floor.pX(0);
                    float fy = -floor.pY(fh);

                    float px = floor.pX(agv.Location.X);
                    float py = floor.pY(agv.Location.Y);


                    controller.draw_offset_pixel.X = fx - px + panelMap.Width / 2;
                    controller.draw_offset_pixel.Y = fy + py - panelMap.Height / 2;
                }
            }

            panelMap.Invalidate();
        }

        #region マウス操作

        private void panelMap_Paint(object sender, PaintEventArgs e)
        {
            AgvControlManager.FloorMap floor = null;

            controller.draw_size_pixel.Width = panelMap.Width;
            controller.draw_size_pixel.Height = panelMap.Height;

            if (0 < LV1.SelectedItems.Count)
            {
                DB.AGV_STATE r = LV1.SelectedItems[0].Tag as DB.AGV_STATE;
                AgvControlManager.FloorAGV agv = controller.agvs.Where(ee => ee.id.Trim() == r.AGV_ID.Trim()).FirstOrDefault();
                if (agv != null)
                {
                    agv.Select(true);

                    floor = agv.floor;
                }
            }

            if (floor == null)
            {
                floor = controller.SelectFloor();
            }

            if (floor != null)
            {
                if (floor.mapeditor.is_new_qr) floor.mapeditor.NewQR(false);
                if (floor.mapeditor.is_new_way) floor.mapeditor.NewWay(false);
            }

            floor = controller.SelectFloor(floor.code);

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

                //textBoxX.Text = ((int)controller.mousePoint.X).ToString();
                //textBoxY.Text = ((int)controller.mousePoint.Y).ToString();

                //if (floor.mapeditor.selected_qr == null)
                //{
                //    AgvControlManager.FloorQR qr = floor.mapeditor.HitTest(controller.mousePoint);
                //    if (qr != null)
                //    {
                //        textBoxCurX.Text = ((int)qr.point.X).ToString();
                //        textBoxCurY.Text = ((int)qr.point.Y).ToString();
                //    }
                //}
                //else
                //{
                //    AgvControlManager.FloorQR qr = floor.mapeditor.selected_qr;
                //    if (qr != null)
                //    {
                //        textBoxCurX.Text = ((int)qr.point.X).ToString();
                //        textBoxCurY.Text = ((int)qr.point.Y).ToString();
                //    }
                //}
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

        //private void panelMap_MouseClick(object sender, MouseEventArgs e)
        //{
        //    var floor = controller.SelectFloor();
        //    if (floor != null)
        //    {
        //        floor.conditioner.MouseClick(e);
        //        panelMap.Invalidate();
        //    }
        //}

        private void PanelMap_MouseWheel(object sender, MouseEventArgs e)
        {
            controller.MouseWheel(e);
            panelMap.Invalidate();

            //textBoxZoom.Text = ((int)(controller.draw_scale * 100)).ToString();
        }

        //private void textBoxZoom_Leave(object sender, EventArgs e)
        //{
        //    float scale = 0;
        //    if (float.TryParse(textBoxZoom.Text, out scale))
        //    {
        //        if (10 <= scale && scale <= 400)
        //        {
        //            controller.Scale(scale / 100, new PointF(panelMap.Width / 2, panelMap.Height / 2));
        //            panelMap.Invalidate();
        //        }
        //    }
        //}

        #endregion
    }
}
