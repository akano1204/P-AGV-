using BelicsClass.Common;
using BelicsClass.UI;
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
    public partial class SubForm_AgvCommunicationSetter : BelicsClass.UI.BL_SubForm_Base
    {
        List<AgvControlManager.AgvInfo> list = new List<AgvControlManager.AgvInfo>();
        BindingSource bs;

        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "", "", "", "", "", "", "", "", "", "", "[F11]:保存", "[F12]:戻る" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get { return this.Text; }
        }

        public SubForm_AgvCommunicationSetter()
        {
            InitializeComponent();
            Resizer_Initialize();
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            if (changed)
            {
                if (BL_MessageBox.Show(this, "変更されてる項目があります。保存せずに破棄しますか？", "確認", MessageBoxButtons.YesNo) != DialogResult.Yes)
                {
                    return;
                }
            }

            Close();
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Width = 180;
                column.HeaderText = "識別ID";
                column.DataPropertyName = "id";
                column.DefaultCellStyle.BackColor = Color.White;
                column.SortMode = DataGridViewColumnSortMode.Automatic;
                dataGridView1.Columns.Add(column);
            }

            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Width = 180;
                column.HeaderText = "IPアドレス";
                column.DataPropertyName = "ipaddress";
                column.DefaultCellStyle.BackColor = Color.White;
                dataGridView1.Columns.Add(column);
            }

            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Width = 160;
                column.HeaderText = "送信先ポート";
                column.DataPropertyName = "hostport";
                column.DefaultCellStyle.BackColor = Color.White;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns.Add(column);
            }

            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Width = 160;
                column.HeaderText = "受信先ポート";
                column.DataPropertyName = "clientport";
                column.DefaultCellStyle.BackColor = Color.White;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns.Add(column);
            }

            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Width = 100;
                column.HeaderText = "停止距離";
                column.DataPropertyName = "stop_distance";
                column.DefaultCellStyle.BackColor = Color.White;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns.Add(column);
            }

            {
                DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                column.Width = 100;
                column.HeaderText = "AGV対角寸法";
                column.DataPropertyName = "radius";
                column.DefaultCellStyle.BackColor = Color.White;
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.Columns.Add(column);
            }

            dataGridView1.AutoGenerateColumns = false;

            textBoxHostPort.Text = Program.ini_agv.Read("PC", "LOCAL_HOST", "");
            textBoxClientPort.Text = Program.ini_agv.Read("PC", "LOCAL_CLIENT", "");

            SortedDictionary<string, AgvControlManager.AgvInfo> l = new SortedDictionary<string, AgvControlManager.AgvInfo>();
            for (int i = 1; i < 100; i++)
            {
                AgvControlManager.AgvInfo d = new AgvControlManager.AgvInfo();
                d.id = Program.ini_agv.Read(i.ToString(), "ID", "");
                if (d.id == "") continue;

                d.ipaddress = Program.ini_agv.Read(i.ToString(), "IP", "");
                d.hostport = Program.ini_agv.Read(i.ToString(), "REMOTE_HOST", 9000);
                d.clientport = Program.ini_agv.Read(i.ToString(), "REMOTE_CLIENT", 9100);
                d.stop_distance = Program.ini_agv.Read(i.ToString(), "STOP_DISTANCE", 375);
                d.radius = Program.ini_agv.Read(i.ToString(), "RADIUS", 102);

                l[d.id] = d;
            }

            foreach (var v in l)
            {
                list.Add(v.Value);
            }

            bs = new BindingSource(list, "");
            dataGridView1.DataSource = bs;

            dataGridView1.Focus();

            Resizer_Initialize();

            //dataGridView1.DragDropEnable();
        }

        //protected override void SubForm_Base_Function07_Clicked(object sender)
        //{
        //    base.SubForm_Base_Function07_Clicked(sender);

        //    if (dataGridView1.SelectedRows.Count == 0)
        //    {
        //        foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
        //        {
        //            dataGridView1.Rows[cell.RowIndex].Selected = true;
        //        }
        //        Application.DoEvents();
        //    }

        //    SortedList<int, int> l = new SortedList<int, int>();
        //    for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
        //    {
        //        l[dataGridView1.SelectedRows[i].Index] = dataGridView1.SelectedRows[i].Index;
        //    }

        //    if (0 < l.Count)
        //    {
        //        for (int i = 0; i < l.Count; i++)
        //        {
        //            int index = l.ElementAt(i).Value;
        //            if (0 < index)
        //            {
        //                var v = list[index];
        //                list.RemoveAt(index);
        //                list.Insert(index - 1, v);
        //            }
        //        }

        //        bs.ResetBindings(false);

        //        if (0 < l.ElementAt(0).Value - 1)
        //        {
        //            dataGridView1.FirstDisplayedScrollingRowIndex = l.ElementAt(0).Value - 1;
        //        }
        //        else
        //        {
        //            dataGridView1.FirstDisplayedScrollingRowIndex = 0;
        //        }

        //        dataGridView1.ClearSelection();

        //        for (int i = 0; i < l.Count; i++)
        //        {
        //            int index = l.ElementAt(i).Value;
        //            if (0 < index)
        //            {
        //                dataGridView1.Rows[index - 1].Selected = true;
        //            }
        //        }
        //    }
        //}

        //protected override void SubForm_Base_Function08_Clicked(object sender)
        //{
        //    base.SubForm_Base_Function08_Clicked(sender);

        //    if (dataGridView1.SelectedRows.Count == 0)
        //    {
        //        foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
        //        {
        //            dataGridView1.Rows[cell.RowIndex].Selected = true;
        //        }
        //        Application.DoEvents();
        //    }

        //    SortedList<int, int> l = new SortedList<int, int>();
        //    for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
        //    {
        //        l[dataGridView1.SelectedRows[i].Index] = dataGridView1.SelectedRows[i].Index;
        //    }

        //    if (0 < l.Count)
        //    {
        //        for (int i = l.Count - 1; 0 <= i; i--)
        //        {
        //            int index = l.ElementAt(i).Value;
        //            if (0 <= index && index < list.Count - 1)
        //            {
        //                var v = list[index];
        //                list.RemoveAt(index);
        //                list.Insert(index + 1, v);
        //            }
        //        }

        //        bs.ResetBindings(false);

        //        if (l.ElementAt(0).Value < list.Count + 1)
        //        {
        //            dataGridView1.FirstDisplayedScrollingRowIndex = l.ElementAt(0).Value + 1;
        //        }
        //        else
        //        {
        //            dataGridView1.FirstDisplayedScrollingRowIndex = list.Count;
        //        }

        //        dataGridView1.ClearSelection();

        //        for (int i = l.Count - 1; 0 <= i; i--)
        //        {
        //            int index = l.ElementAt(i).Value;
        //            if (0 <= index && index < list.Count - 1)
        //            {
        //                dataGridView1.Rows[index + 1].Selected = true;
        //            }
        //        }
        //    }
        //}

        protected override void SubForm_Base_Function11_Clicked(object sender)
        {
            base.SubForm_Base_Function11_Clicked(sender);

            {
                List<string> id = new List<string>();
                bool found = false;
                int r = 0;
                foreach (var v in list)
                {
                    if (v.id.Trim() == "")
                    {
                        dataGridView1.ClearSelection();
                        dataGridView1[0, r].Selected = true;
                        BL_MessageBox.Show(this, "IDを設定してください。", "エラー");
                        return;
                    }

                    if (id.Contains(v.id))
                    {
                        found = true;
                        break;
                    }
                    id.Add(v.id);
                    r++;
                }
                if (found)
                {
                    dataGridView1.ClearSelection();
                    dataGridView1[0, r].Selected = true;
                    BL_MessageBox.Show(this, "IDが重複しています。", "エラー");
                    return;
                }
            }

            {
                List<string> ip_p1 = new List<string>();
                List<string> ip_p2 = new List<string>();
                bool found = false;
                int r = 0;
                foreach (var v in list)
                {
                    IPAddress ipaddress;
                    if (!IPAddress.TryParse(v.ipaddress.Replace("\r", ""), out ipaddress))
                    {
                        dataGridView1[1, r].Selected = true;
                        BL_MessageBox.Show(this, "正しいIPアドレスを設定してください。", "エラー");
                        return;
                    }

                    if (ip_p1.Contains(v.ipaddress + ":" + v.clientport))
                    {
                        found = true;
                        break;
                    }
                    ip_p1.Add(v.ipaddress + ":" + v.clientport);

                    if (ip_p2.Contains(v.ipaddress + ":" + v.hostport))
                    {
                        found = true;
                        break;
                    }
                    ip_p2.Add(v.ipaddress + ":" + v.hostport);

                    r++;
                }
                if (found)
                {
                    dataGridView1[1, r].Selected = true;
                    BL_MessageBox.Show(this, "IPアドレス・ポート番号が重複しています。", "エラー");
                    return;
                }
            }

            try
            {
                System.IO.File.Delete(Program.ini_agv.FullName);

                Program.ini_agv.Write("PC", "LOCAL_CLIENT", textBoxClientPort.Text);
                Program.ini_agv.Write("PC", "LOCAL_HOST", textBoxHostPort.Text);

                SortedDictionary<string, AgvControlManager.AgvInfo> l = new SortedDictionary<string, AgvControlManager.AgvInfo>();

                foreach (var v in list)
                {
                    l[v.id] = v;
                }

                int no = 1;
                foreach (var v in l)
                {
                    v.Value.no = no++;

                    Program.ini_agv.Write(v.Value.no.ToString(), "ID", v.Value.id);
                    Program.ini_agv.Write(v.Value.no.ToString(), "IP", v.Value.ipaddress);
                    Program.ini_agv.Write(v.Value.no.ToString(), "REMOTE_HOST", v.Value.hostport);
                    Program.ini_agv.Write(v.Value.no.ToString(), "REMOTE_CLIENT", v.Value.clientport);
                    Program.ini_agv.Write(v.Value.no.ToString(), "STOP_DISTANCE", v.Value.stop_distance);
                    Program.ini_agv.Write(v.Value.no.ToString(), "RADIUS", v.Value.radius);
                }

                changed = false;
                BL_MessageBox.Show(this, "保存しました。");

                list = l.Values.ToList();
                bs.DataSource = list;
                bs.ResetBindings(false);

                Program.controller.ListupConflictQR();

                var appmode = Program.controller.applicate_mode;
                int movemode = Program.controller.movemode_current;
                int interval = Program.controller.interval_current;

                Program.controller.Stop();
                Program.controller.Start(appmode, movemode, interval);
            }
            catch (Exception ex)
            {
                BL_MessageBox.Show(this, ex.Message, "エラー");
            }
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
        }

        private void dataGridView1_RowValidating(object sender, DataGridViewCellCancelEventArgs e)
        {
        }

        bool changed = false;
        string prev_data = "";

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            prev_data = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1[e.ColumnIndex, e.RowIndex].Value != null)
            {
                if (prev_data != dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString())
                {
                    changed = true;
                }
            }
        }
    }
}
