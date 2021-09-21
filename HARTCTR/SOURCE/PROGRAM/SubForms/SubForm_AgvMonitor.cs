using BelicsClass.Common;
using BelicsClass.UI;
using BelicsClass.UI.Controls;
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

namespace PROGRAM
{
    public partial class SubForm_AgvMonitor : BelicsClass.UI.BL_SubForm_Base
    {
        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "[F12]:戻る" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get { return this.Text; }
        }

        AgvControlManager controller = null;

        public SubForm_AgvMonitor()
        {
            this.controller = Program.controller;
            InitializeComponent();
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            Close();
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            controller.EventPaint += V_EventPaint;
            set_LV();

            Resizer_Initialize();

            data_disp();
        }

        protected override void SubForm_Base_FormClosing(object sender, FormClosingEventArgs e)
        {
            base.SubForm_Base_FormClosing(sender, e);

            controller.EventPaint -= V_EventPaint;
        }

        void set_LV()
        {
            LV1.Items.Clear();

            //GridLine表示
            LV1.GridLines = true;
            //ヘッダー定義
            ColumnHeader column1 = new ColumnHeader
            {
                Text = "AGV番号",
                Width = 100
            };
            ColumnHeader column2 = new ColumnHeader
            {
                Text = "AGV名",
                Width = 180
            };
            ColumnHeader column3 = new ColumnHeader
            {
                Text = "IPアドレス",
                Width = 180
            };
            ColumnHeader column4 = new ColumnHeader
            {
                Text = "通信状態",
                Width = 120
            };
            ColumnHeader column5 = new ColumnHeader
            {
                Text = "運行状態",
                Width = 120
            };
            ColumnHeader column6 = new ColumnHeader
            {
                Text = "充電残量",
                Width = 120
            };
            ColumnHeader column7 = new ColumnHeader
            {
                Text = "現在座標",
                Width = 200
            };
            ColumnHeader column8 = new ColumnHeader
            {
                Text = "エラーコード",
                Width = 0
            };
            LV1.View = View.Details;

            ColumnHeader[] colHeaderRegValue = { column1, column2, column3, column4, column5, column6, column7 };
            //ヘッダー追加
            LV1.Columns.AddRange(colHeaderRegValue);
        }

        private void V_EventPaint()
        {
            MethodInvoker process = (MethodInvoker)delegate ()
            {
                data_disp();
            };

            try
            {
                if (this.InvokeRequired)
                {
                    this.BeginInvoke(process);
                }
                else
                {
                    data_disp();
                }
            }
            catch (ObjectDisposedException) { }
            catch (InvalidOperationException) { }
        }

        void data_disp()
        {
            try
            {

                LV1.Items.Clear();
                var sorted_agvs = Program.controller.agvs.OrderBy(a => a.id);
                foreach (var agv in sorted_agvs)
                {
                    if (agv.agvRunner != null && agv.agvRunner.communicator != null)
                    {
                        ListViewItem item = new ListViewItem();
                        item.Text = agv.id;
                        item.SubItems.Add("AGV" + agv.id + "号機");
                        item.SubItems.Add(Program.ini_agv.Read(int.Parse(agv.id).ToString(), "IP", ""));
                        string map = agv.agvRunner.communicator.GetState.map;
                        Point location = agv.agvRunner.communicator.GetState.Location;
                        if (map.Trim() == "" && location.X == 0 && location.Y == 0)
                        {
                            item.SubItems.Add("通信状態不明");
                        }
                        else
                        {
                            item.SubItems.Add("正常");
                        }

                        ushort cmd = agv.agvRunner.communicator.GetState.cmd;

                        if (agv.agvRunner.communicator.GetState.error_code != 0)
                        {
                            item.SubItems.Add("異常中");
                        }
                        else if (agv.agvRunner.communicator.GetState.sta_charge == true)
                        {
                            item.SubItems.Add("充電中");
                        }
                        else if (cmd == (ushort)AgvController.AgvCommunicator.State.CMD.STATE)
                        {
                            item.SubItems.Add("走行中");
                        }
                        else if (cmd == (ushort)AgvController.AgvCommunicator.State.CMD.REQUEST)
                        {
                            item.SubItems.Add("待機中");
                        }
                        else
                        {
                            item.SubItems.Add("");
                        }
                        item.SubItems.Add(agv.agvRunner.communicator.GetState.bat.ToString() + "%");
                        item.SubItems.Add(map + "." + location.X.ToString() + "." + location.Y.ToString());
                        item.SubItems.Add(agv.agvRunner.communicator.GetState.error_code.ToString());
                        LV1.Items.Add(item);
                    }
                }

                for (int ii = 0; ii < LV1.Items.Count; ii++)
                {

                    if (LV1.Items[ii].SubItems[3].Text == "正常")
                    {
                        LV1.Items[ii].UseItemStyleForSubItems = false;
                        LV1.Items[ii].SubItems[3].BackColor = Color.Lime;
                    }
                    else
                    {
                        LV1.Items[ii].UseItemStyleForSubItems = false;
                        LV1.Items[ii].SubItems[3].BackColor = Color.Red;
                    }
                    if (LV1.Items[ii].SubItems[4].Text == "走行中")
                    {
                        LV1.Items[ii].UseItemStyleForSubItems = false;
                        LV1.Items[ii].SubItems[4].BackColor = Color.Lime;
                    }
                    else if (LV1.Items[ii].SubItems[4].Text == "待機中")
                    {
                        LV1.Items[ii].UseItemStyleForSubItems = false;
                        LV1.Items[ii].SubItems[4].BackColor = Color.Orange;
                    }
                    else if (LV1.Items[ii].SubItems[4].Text == "充電中")
                    {
                        LV1.Items[ii].UseItemStyleForSubItems = false;
                        LV1.Items[ii].SubItems[4].BackColor = Color.Yellow;
                    }
                    else if (LV1.Items[ii].SubItems[4].Text == "異常中")
                    {
                        LV1.Items[ii].UseItemStyleForSubItems = false;
                        LV1.Items[ii].SubItems[4].BackColor = Color.Red;
                    }
                    else
                    {
                        LV1.Items[ii].UseItemStyleForSubItems = false;
                        LV1.Items[ii].SubItems[4].BackColor = Color.LightSlateGray;
                    }
                }
            }
            catch { }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            data_disp();
        }


        private void button2_Click(object sender, EventArgs e)
        {
            int idx = 0;
            idx = LV1.SelectedItems[0].Index;
            if (LV1.Items[idx].SubItems[4].Text == "異常中")
            {
                //Form_Error sub = new Form_Error();
                //string errcode = LV1.Items[idx].SubItems[7].Text;
                //sub.Err_code = errcode;
                //sub.ShowDialog();
            }
        }
    }

}
