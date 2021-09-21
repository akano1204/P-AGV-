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
    public partial class SubForm_AutoratorSetting : BelicsClass.UI.BL_SubForm_Base
    {
        List<AgvControlManager.AutoratorController> list = new List<AgvControlManager.AutoratorController>();
        BindingSource bs;
        BindingSource bs2;
        BindingSource bs3;

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

        public SubForm_AutoratorSetting()
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
                var targetgrid = dataGridView1;
                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.Width = 234;
                    column.HeaderText = "ID";
                    column.DataPropertyName = "id";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.SortMode = DataGridViewColumnSortMode.Automatic;
                    column.ReadOnly = true;
                    targetgrid.Columns.Add(column);
                }

                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.Width = 200;
                    column.HeaderText = "IPアドレス";
                    column.DataPropertyName = "ipaddress";
                    column.DefaultCellStyle.BackColor = Color.White;
                    targetgrid.Columns.Add(column);
                }

                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.Width = 140;
                    column.HeaderText = "ポートNo";
                    column.DataPropertyName = "hostport";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    targetgrid.Columns.Add(column);
                }

                {
                    DataGridViewCheckBoxColumn column = new DataGridViewCheckBoxColumn();
                    column.Width = 140;
                    column.HeaderText = "アシスタ有";
                    column.DataPropertyName = "is_assister";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    targetgrid.Columns.Add(column);
                }

                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.Width = 180;
                    column.HeaderText = "アシスタポートNo";
                    column.DataPropertyName = "hostport_assister";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    targetgrid.Columns.Add(column);
                }


                targetgrid.AutoGenerateColumns = false;

                var autorators = Program.controller.AllAutoratorQR;
                SortedDictionary<string, AgvControlManager.AutoratorController> l = new SortedDictionary<string, AgvControlManager.AutoratorController>();

                foreach (var v in autorators)
                {
                    if (!l.ContainsKey(v.autorator_id))
                    {
                        l[v.autorator_id] = v.autorator_info;
                    }
                }

                foreach (var v in l)
                {
                    list.Add(v.Value);
                }

                bs = new BindingSource(list, "");
                targetgrid.DataSource = bs;

                targetgrid.Focus();
            }

            {
                var targetgrid = dataGridView2;
                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.Width = 120;
                    column.HeaderText = "方向";
                    column.DataPropertyName = "degree";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.SortMode = DataGridViewColumnSortMode.Automatic;
                    column.ReadOnly = true;
                    targetgrid.Columns.Add(column);
                }

                {

                    DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
                    column.Width = 120;
                    column.HeaderText = "搬入口";
                    column.DataPropertyName = "code";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.Items.Add("F");
                    column.Items.Add("B");
                    targetgrid.Columns.Add(column);
                }

                targetgrid.AutoGenerateColumns = false;
            }

            {
                var targetgrid = dataGridView3;
                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.Width = 140;
                    column.HeaderText = "フロアコード";
                    column.DataPropertyName = "code";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.SortMode = DataGridViewColumnSortMode.Automatic;
                    column.ReadOnly = true;
                    targetgrid.Columns.Add(column);
                }

                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.Width = 100;
                    column.HeaderText = "階";
                    column.DataPropertyName = "no";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.SortMode = DataGridViewColumnSortMode.Automatic;
                    targetgrid.Columns.Add(column);
                }

                {
                    DataGridViewCheckBoxColumn column = new DataGridViewCheckBoxColumn();
                    column.Width = 120;
                    column.HeaderText = "代表QR";
                    column.DataPropertyName = "is_qrcode";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    targetgrid.Columns.Add(column);
                }

                targetgrid.AutoGenerateColumns = false;
            }

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

                    if (ip_p2.Contains(v.ipaddress + ":" + v.hostport))
                    {
                        found = true;
                        break;
                    }
                    ip_p2.Add(v.ipaddress + ":" + v.hostport);

                    if (ip_p2.Contains(v.ipaddress + ":" + v.hostport_assister))
                    {
                        found = true;
                        break;
                    }
                    ip_p2.Add(v.ipaddress + ":" + v.hostport_assister);

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
                System.IO.File.Delete(Program.ini_autorator.FullName);

                SortedDictionary<string, AgvControlManager.AutoratorController> l = new SortedDictionary<string, AgvControlManager.AutoratorController>();

                foreach (var v in list)
                {
                    l[v.id] = v;
                }

                int no = 1;
                foreach (var v in l)
                {
                    Program.ini_autorator.Write("ID", no.ToString(), v.Value.id);
                    Program.ini_autorator.Write(v.Value.id, "IP", v.Value.ipaddress);
                    Program.ini_autorator.Write(v.Value.id, "PORT", v.Value.hostport);
                    Program.ini_autorator.Write(v.Value.id, "ASSISTER", v.Value.is_assister);
                    Program.ini_autorator.Write(v.Value.id, "PORT_ASSISTER", v.Value.hostport_assister);

                    foreach (var vv in v.Value.sideinfo)
                    {
                        Program.ini_autorator.Write(v.Value.id, "SIDE_CODE_" + vv.degree.ToString(), vv.code);
                    }

                    foreach (var vv in v.Value.floorinfo)
                    {
                        Program.ini_autorator.Write(v.Value.id, "FLOOR_CODE_" + vv.code, vv.no);
                        Program.ini_autorator.Write(v.Value.id, "QR_CODE" + vv.code, vv.is_qrcode);
                    }

                    no++;
                }

                changed = false;
                BL_MessageBox.Show(this, "保存しました。");

                list = l.Values.ToList();
                bs.DataSource = list;
                bs.ResetBindings(false);

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

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell.RowIndex < 0 || list.Count <= dataGridView1.CurrentCell.RowIndex) return;

            AgvControlManager.AutoratorController info = list[dataGridView1.CurrentCell.RowIndex];

            {
                var targetgrid = dataGridView2;
                targetgrid.DataSource = null;
                targetgrid.Columns.Clear();

                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.Width = 120;
                    column.HeaderText = "方向";
                    column.DataPropertyName = "degree";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.SortMode = DataGridViewColumnSortMode.Automatic;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    column.ReadOnly = true;
                    targetgrid.Columns.Add(column);
                }

                {
                    DataGridViewComboBoxColumn column = new DataGridViewComboBoxColumn();
                    column.Width = 120;
                    column.HeaderText = "搬入口";
                    column.DataPropertyName = "code";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.Items.Add("F");
                    column.Items.Add("B");
                    targetgrid.Columns.Add(column);
                }

                targetgrid.AutoGenerateColumns = false;

                List<AgvControlManager.AutoratorController.SideInfo> sideinfo = new List<AgvControlManager.AutoratorController.SideInfo>();
                sideinfo.AddRange(info.sideinfo.OrderBy(ee => ee.degree));
                info.sideinfo.Clear();
                info.sideinfo.AddRange(sideinfo);

                bs2 = new BindingSource(info.sideinfo, "");
                targetgrid.DataSource = bs2;
            }

            {
                var targetgrid = dataGridView3;
                targetgrid.DataSource = null;
                targetgrid.Columns.Clear();

                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.Width = 180;
                    column.HeaderText = "フロアコード";
                    column.DataPropertyName = "code";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.SortMode = DataGridViewColumnSortMode.Automatic;
                    column.ReadOnly = true;
                    targetgrid.Columns.Add(column);
                }

                {
                    DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
                    column.Width = 80;
                    column.HeaderText = "階";
                    column.DataPropertyName = "no";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.SortMode = DataGridViewColumnSortMode.Automatic;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                    targetgrid.Columns.Add(column);
                }

                {
                    DataGridViewCheckBoxColumn column = new DataGridViewCheckBoxColumn();
                    column.Width = 120;
                    column.HeaderText = "代表QR";
                    column.DataPropertyName = "is_qrcode";
                    column.DefaultCellStyle.BackColor = Color.White;
                    column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    targetgrid.Columns.Add(column);
                }

                targetgrid.AutoGenerateColumns = false;

                List<AgvControlManager.AutoratorController.FloorInfo> floorinfo = new List<AgvControlManager.AutoratorController.FloorInfo>();
                floorinfo.AddRange(info.floorinfo.OrderByDescending(ee => ee.no));
                info.floorinfo.Clear();
                info.floorinfo.AddRange(floorinfo);

                bs3 = new BindingSource(info.floorinfo, "");
                targetgrid.DataSource = bs3;
            }
        }

        private void dataGridView3_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            BL_MessageBox.Show(this, "値が不正です。");
        }
    }
}
