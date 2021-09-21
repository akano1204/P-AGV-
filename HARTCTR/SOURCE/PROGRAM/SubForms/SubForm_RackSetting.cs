using BelicsClass.Common;
using BelicsClass.UI;
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

namespace PROGRAM
{
    using Rack = AgvControlManager.Rack;
    using RackInfo = AgvControlManager.RackInfo;

    public partial class SubForm_RackSetting : BelicsClass.UI.BL_SubForm_Base
    {
        private List<RackInfo> list = new List<RackInfo>();
        private BindingSource bs;

        private RackMaster master = null;

		#region サブフォームプロパティ

		/// <summary>
		/// ファンクションキー文字列をMainFormに取得させるために必要です。
		/// </summary>
		/// <returns></returns>
		override public string[] FunctionStrings()
        {
            return new string[] { "", "", "", "", "", "", "", "", "[F8]:保存", "", "", "", "[F12]:戻る" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get { return this.Text; }
        }

        #endregion

        #region ファンクションキー操作

        protected override void SubForm_Base_Function08_Clicked(object sender)
        {
            base.SubForm_Base_Function08_Clicked(sender);

            Cursor = Cursors.WaitCursor;

            string err = Save();
            if (err != "")
            {
                BL_MessageBox.Show(this, err, this.Text);
            }
            else
            {
                BL_MessageBox.Show(this, "保存しました。", this.Text);
                changed = false;
            }

            Cursor = Cursors.Default;
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

        #endregion

        #region コンストラクタ

        public SubForm_RackSetting()
        {
            InitializeComponent();
            Resizer_Initialize();
        }

		#endregion

		#region ロード

		protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            master = RackMaster.Instance;

            #region 基本設定

            void addColumns<T>(List<Tuple<Tuple<string, string>, int>> combs) where T : DataGridViewColumn, new()
            {
                foreach (var comb in combs)
                {
                    var text = comb.Item1.Item1;
                    var property_name = comb.Item1.Item2;
                    var width = comb.Item2;

                    var column = new T
                    {
                        Name = $"clm{text}",
                        HeaderText = text,
                        DataPropertyName = property_name,
                        Width = width,
                        SortMode = DataGridViewColumnSortMode.Automatic
                    };

                    column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    dataGridViewRacks.Columns.Add(column);
                }
            }

            void addColumns2<T>(int width, List<Tuple<string, string>> _headers) where T : DataGridViewColumn, new()
            {
                var combs = new List<Tuple<Tuple<string, string>, int>>();

                foreach (var header in _headers)
                {
                    var text = header.Item1;
                    var property_name = header.Item2;

                    combs.Add(new Tuple<Tuple<string, string>, int>(new Tuple<string, string>(text, property_name), width));
                }

                addColumns<T>(combs);
            }

            var headers = new List<Tuple<string, string>>
            { new Tuple<string, string>("棚No", "no") };

            addColumns2<DataGridViewTextBoxColumn>(100, headers);

            headers = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("寸法(cm)\n幅", "sizeW"),
                new Tuple<string, string>("\n奥行", "sizeL")
            };

            addColumns2<DataGridViewTextBoxColumn>(80, headers);

            headers = new List<Tuple<string, string>>
            {
                new Tuple<string, string>("進入可能\n0°", "can_inout_0"),
                new Tuple<string, string>("\n90°", "can_inout_90"),
                new Tuple<string, string>("\n180°", "can_inout_180"),
                new Tuple<string, string>("\n270°", "can_inout_270"),
                new Tuple<string, string>("前進可能\n0°", "can_move_0"),
                new Tuple<string, string>("\n90°", "can_move_90"),
                new Tuple<string, string>("\n180°", "can_move_180"),
                new Tuple<string, string>("\n270°", "can_move_270"),
            };

            addColumns2<DataGridViewCheckBoxColumn>(75, headers);

            headers = new List<Tuple<string, string>>
            { 
                new Tuple<string, string>("面考慮", "anyface"),
                new Tuple<string, string>("ｵｰﾊﾞｰﾊﾝｸﾞ", "overhang") 
            };

            addColumns2<DataGridViewCheckBoxColumn>(80, headers);

            DeployData();

            bs = new BindingSource(list, "");
            dataGridViewRacks.DataSource = bs;

            #endregion

            Resizer_Initialize();
        }

		#endregion

		#region データグリッドイベント

		bool changed = false;
        string prev_data = "";

        private void dataGridViewRacks_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            prev_data = dataGridViewRacks[e.ColumnIndex, e.RowIndex].Value.ToString();
        }

        private void dataGridViewRacks_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridViewRacks[e.ColumnIndex, e.RowIndex].Value != null)
            {
                if (prev_data != dataGridViewRacks[e.ColumnIndex, e.RowIndex].Value.ToString())
                {
                    changed = true;
                }
            }
        }

        private void dataGridViewRacks_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            // セル入力値とデータソースの型相違エラー捕捉(特に幅、奥行行について)
        }

        private void dataGridViewRacks_KeyDown(object sender, KeyEventArgs e)
        {
            //基本クラスで実装
            //var dgv = sender as DataGridView;

            //switch (e.KeyCode)
            //{
            //    case Keys.Space:
            //        {
            //            if (dgv.SelectedCells.Count == 1)
            //            {
            //                var cell = dgv.SelectedCells[0];

            //                if (cell.FormattedValue is bool is_checked)
            //                {
            //                    cell.Value = !is_checked;
            //                }
            //            }
            //        }
            //        break;
            //}
        }

		#endregion

		#region メソッド

		private string Save()
        {
            var ret = "";

            var rack_collection = new List<Rack>();

            foreach (DataGridViewRow row in dataGridViewRacks.Rows)
            {
                var cells = row.Cells;

                foreach (DataGridViewCell cell in cells)
                {
                    if (cell.Style.BackColor == Color.Red)
                    {
                        return "入力されたデータに誤りが見つかりました。\nデータを修正してください。";
                    }
                }
            }

            foreach (var rack_info in list)
            {
                if (string.IsNullOrEmpty(rack_info.no)) continue;

                var no = rack_info.no;
                var sizeW = rack_info.sizeW;
                var sizeL = rack_info.sizeL;

                var can_inout = new Dictionary<int, bool>
                {
                    { 0, rack_info.can_inout_0 },
                    { 90, rack_info.can_inout_90 },
                    { 180, rack_info.can_inout_180 },
                    { 270, rack_info.can_inout_270 },
                };

                var can_move = new Dictionary<int, bool>
                {
                    { 0, rack_info.can_move_0 },
                    { 90, rack_info.can_move_90 },
                    { 180, rack_info.can_move_180 },
                    { 270, rack_info.can_move_270 },
                };

                var anyface = rack_info.anyface;
                var overhang = rack_info.overhang;

                var rack = new Rack
                {
                    rack_no = no,
                    can_inout = can_inout,
                    can_move = can_move,
                    sizeW = sizeW,
                    sizeL = sizeL,
                    anyface = anyface,
                    overhang = overhang,
                };

                rack_collection.Add(rack);
            }

            ret = master.Save(rack_collection);

            return ret;
        }

        private void DeployData()
        {
            var rack_collection = master.GetRackList();

            foreach (var rack in rack_collection)
            {
                var rack_info = new RackInfo
                {
                    no = rack.rack_no,
                    sizeW = rack.sizeW,
                    sizeL = rack.sizeL,
                    can_inout_0 = rack.can_inout[0],
                    can_inout_90 = rack.can_inout[90],
                    can_inout_180 = rack.can_inout[180],
                    can_inout_270 = rack.can_inout[270],
                    can_move_0 = rack.can_move[0],
                    can_move_90 = rack.can_move[90],
                    can_move_180 = rack.can_move[180],
                    can_move_270 = rack.can_move[270],
                    anyface = rack.anyface,
                    overhang = rack.overhang,
                };

                list.Add(rack_info);
            }
        }

        #endregion
    }
}
