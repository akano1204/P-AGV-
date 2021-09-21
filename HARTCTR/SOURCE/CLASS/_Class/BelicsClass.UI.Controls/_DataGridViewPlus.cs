using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Data;
using System.Security.Cryptography;
using System.Drawing;

namespace BelicsClass.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public class BL_DataGridViewPlus : BL_DoubleBufferDataGridView
    {
        private Rectangle dragBoxFromMouseDown;     // 座標用
        private int rowIndexFromMouseDown;          // 移動元Index用
        private int rowIndexOfItemUnderMouseToDrop; // 移動先Index用

        /// <summary>
        /// 
        /// </summary>
        public class ColWidths
        {
            /// <summary>
            /// 
            /// </summary>
            public int[] Widths;
        }

        /// <summary>
        /// 
        /// </summary>
        public void SaveColWidths()
        {
            try
            {
                String ExePath = System.AppDomain.CurrentDomain.BaseDirectory;
                String XmlPath = ExePath + "\\" + this.Parent.Name + "_" + this.Name + ".xml";
                StreamWriter sw = new StreamWriter(XmlPath, false, Encoding.Default);
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ColWidths));
                    ColWidths colw = new ColWidths();
                    Array.Resize<int>(ref colw.Widths, this.Columns.Count);
                    for (int i = 0; i <= this.Columns.Count - 1; i++)
                    {
                        colw.Widths[i] = this.Columns[i].Width;
                    }
                    serializer.Serialize(sw, colw);
                }
                catch { }
                finally
                {
                    if (sw != null) sw.Close();
                }
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReadColWidths()
        {
            try
            {
                String ExePath = System.AppDomain.CurrentDomain.BaseDirectory;
                String XmlPath = ExePath + "\\" + this.Parent.Name + "_" + this.Name + ".xml";
                StreamReader sr = new StreamReader(XmlPath, Encoding.Default);
                try
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(ColWidths));
                    ColWidths colw = new ColWidths();
                    colw = (ColWidths)(serializer.Deserialize(sr));
                    for (int i = 0; i <= this.Columns.Count - 1; i++)
                    {
                        this.Columns[i].Width = colw.Widths[i];
                    }
                }
                catch { }
                finally
                {
                    if (sr != null) sr.Close();
                }
            }
            catch { }
        }

        /// <summary>
        /// 
        /// </summary>
        public String[] ColumnChars = new String[] { };
        private int _editingColumn;
        private DataGridViewTextBoxEditingControl _editingCtrl;

        /// <summary>
        /// 
        /// </summary>
        public bool CellVisibleFix { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool RowIndexVisible { get { return _rowindexvisible; } set { _rowindexvisible = value; } }
        private bool _rowindexvisible = true;

        /// <summary>
        /// 
        /// </summary>
        public BL_DataGridViewPlus()
            : base()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            ((System.ComponentModel.ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // DataGridViewPlus
            // 
            this.RowTemplate.Height = 21;
            this.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridViewPlus_CellEndEdit);
            this.EditingControlShowing += new System.Windows.Forms.DataGridViewEditingControlShowingEventHandler(this.DataGridViewPlus_EditingControlShowing);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DataGridViewPlus_KeyDown);
            ((System.ComponentModel.ISupportInitialize)(this)).EndInit();
            this.ResumeLayout(false);

        }

        #region ドラッグドロップでの並び替え操作

        /// <summary>
        /// 
        /// </summary>
        public void DragDropEnable()
        {
            this.AllowDrop = true;
            this.MouseMove += BL_DataGridViewPlus_MouseMove;
            this.MouseDown += BL_DataGridViewPlus_MouseDown;
            this.DragOver += BL_DataGridViewPlus_DragOver;
            this.DragDrop += BL_DataGridViewPlus_DragDrop;
        }

        /// <summary>
        /// 
        /// </summary>
        public void DragDropDisable()
        {
            this.AllowDrop = false;
            this.MouseMove -= BL_DataGridViewPlus_MouseMove;
            this.MouseDown -= BL_DataGridViewPlus_MouseDown;
            this.DragOver -= BL_DataGridViewPlus_DragOver;
            this.DragDrop -= BL_DataGridViewPlus_DragDrop;
        }

        private void BL_DataGridViewPlus_DragDrop(object sender, DragEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            // データグリッドのポイントを取得
            Point clientPoint = PointToClient(new Point(e.X, e.Y));
            // 移動先INDEX
            rowIndexOfItemUnderMouseToDrop = HitTest(clientPoint.X, clientPoint.Y).RowIndex;

            // ドラッグ＆ドロップ効果【移動】の場合・INDEX範囲内の場合
            if (e.Effect == DragDropEffects.Move && rowIndexOfItemUnderMouseToDrop > -1)
            {
                BindingSource bs = dgv.DataSource as BindingSource;
                if (bs == null)
                {
                    // 移動データ退避
                    DataTable Dt = dgv.DataSource as DataTable;
                    if (Dt != null)
                    {
                        Object[] rowArray = Dt.Rows[rowIndexFromMouseDown].ItemArray;
                        DataRow row = Dt.NewRow();
                        row.ItemArray = rowArray;

                        // 移動元削除
                        Dt.Rows.RemoveAt(rowIndexFromMouseDown);
                        // 移動先新規行挿入
                        Dt.Rows.InsertAt(row, rowIndexOfItemUnderMouseToDrop);
                    }
                }
                else
                {
                    object d = bs.List[rowIndexFromMouseDown];
                    bs.RemoveAt(rowIndexFromMouseDown);
                    if (bs.List.Count <= rowIndexOfItemUnderMouseToDrop) rowIndexOfItemUnderMouseToDrop = bs.List.Count - 1;
                    bs.Insert(rowIndexOfItemUnderMouseToDrop, d);
                }
            }
        }

        private void BL_DataGridViewPlus_DragOver(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Move;
        }

        private void BL_DataGridViewPlus_MouseDown(object sender, MouseEventArgs e)
        {
            rowIndexFromMouseDown = HitTest(e.X, e.Y).RowIndex;

            // ヘッダー以外
            if (rowIndexFromMouseDown > -1)
            {
                var dragSize = SystemInformation.DragSize;
                // ドラッグ操作が開始されない範囲を取得
                dragBoxFromMouseDown = new Rectangle(new Point(e.X - dragSize.Width / 2, e.Y - dragSize.Height / 2), dragSize);
            }
            else
            {
                dragBoxFromMouseDown = Rectangle.Empty;
            }
        }

        private void BL_DataGridViewPlus_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (dragBoxFromMouseDown != Rectangle.Empty && !(dragBoxFromMouseDown.Contains(e.X, e.Y)))
                {
                    DragDropEffects dropEffect = DoDragDrop(Rows[rowIndexFromMouseDown], DragDropEffects.Move);
                }
            }
        }

        #endregion

        /// <summary>
        /// セルが編集中になった時の処理
        /// </summary>
        /// <param name="sender">イベントの発生元</param>
        /// <param name="e">イベントの情報</param>
        /// <remarks>編集中のTextBoxEditingControlにKeyPressイベント設定</remarks>
        private void DataGridViewPlus_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            // 編集中のカラム番号を保存
            _editingColumn = ((DataGridView)(sender)).CurrentCellAddress.X;
            try
            {
                // 編集中のTextBoxEditingControlにKeyPressイベント設定
                _editingCtrl = ((DataGridViewTextBoxEditingControl)(e.Control));
                _editingCtrl.KeyPress += new KeyPressEventHandler(DataGridViewPlus_CellKeyPress);
            }
            catch { }
        }

        /// <summary>
        /// セルの編集が終わった時の処理
        /// </summary>
        /// <param name="sender">イベントの発生元</param>
        /// <param name="e">イベントの情報</param>
        /// <remarks>編集中のTextBoxEditingControlからKeyPressイベント削除</remarks>
        private void DataGridViewPlus_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (_editingCtrl != null)
            {
                // 編集中のTextBoxEditingControlからKeyPressイベント削除
                _editingCtrl.KeyPress -= new KeyPressEventHandler(DataGridViewPlus_CellKeyPress);
                _editingCtrl = null;
            }
        }

        /// <summary>
        /// 編集中のTextBoxEditingControlのKeyPressの処理
        /// </summary>
        /// <param name="sender">イベントの発生元</param>
        /// <param name="e">イベントの情報</param>
        /// <remarks>力可能文字の判定</remarks>
        private void DataGridViewPlus_CellKeyPress(object sender, KeyPressEventArgs e)
        {
            // カラムへの入力可能文字を指定するための配列が指定されているかチェック
            if (ColumnChars.GetType().IsArray)
            {
                // カラムへの入力可能文字を指定するための配列数チェック
                if (ColumnChars.GetLength(0) - 1 >= _editingColumn)
                {
                    // カラムへの入力可能文字が指定されているかチェック
                    if (ColumnChars[_editingColumn] != "")
                    {
                        //カラムへの入力可能文字かチェック
                        if (ColumnChars[_editingColumn].IndexOf(e.KeyChar) < 0 && e.KeyChar != (char)Keys.Back)
                        {
                            // カラムへの入力可能文字では無いので無効
                            e.Handled = true;
                        }
                    }
                }
            }
        }

        private void DataGridViewPlus_KeyDown(object sender, KeyEventArgs e)
        {
            DataGridView dgv = (DataGridView)sender;

            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                // セルの内容を消去
                foreach (DataGridViewCell v in dgv.SelectedCells)
                {
                    if (typeof(string).IsInstanceOfType(v.Value)) v.Value = "";
                    else v.Value = 0;
                }
            }
            else if ((e.Modifiers & Keys.Control) == Keys.Control && e.KeyCode == Keys.C)
            {
                DataObject d = GetClipboardContent();
                Clipboard.SetDataObject(d);
                e.Handled = true;
            }
            else if ((e.Modifiers & Keys.Control) == Keys.Control && e.KeyCode == Keys.V)
            {
                try
                {
                    string CopiedContent = Clipboard.GetText();
                    string[] Lines = CopiedContent.Split(new string[] { "\n" }, StringSplitOptions.RemoveEmptyEntries);
                    int StartingRow = dgv.CurrentCell.RowIndex;
                    int StartingColumn = dgv.CurrentCell.ColumnIndex;

                    int r = dgv.FirstDisplayedScrollingRowIndex;
                    int c = dgv.FirstDisplayedScrollingColumnIndex;


                    BindingSource bs = dgv.DataSource as BindingSource;

                    if (bs == null)
                    {

                    }
                    else
                    {
                        dgv.DataSource = null;

                        foreach (var line in Lines)
                        {
                            object obj = null;

                            if (line.Trim() == "") continue;

                            if (AllowUserToAddRows && bs.Count <= StartingRow)
                            {
                                obj = bs.AddNew();
                            }

                            if (StartingRow < bs.Count)
                            {
                                string[] cells = line.Split('\t');
                                int ColumnIndex = StartingColumn;

                                DataRowView row = bs[StartingRow] as DataRowView;
                                if (row != null)
                                {
                                    for (int i = 0; i < cells.Length && ColumnIndex < row.Row.ItemArray.Length; i++)
                                    {
                                        if (i == 0 && cells[i].Trim() == "") continue;

                                        row = bs[StartingRow] as DataRowView;
                                        if (row != null)
                                        {
                                            DataGridViewComboBoxCell cell = null;
                                            try
                                            {
                                                cell = dgv.Columns[ColumnIndex].CellTemplate as DataGridViewComboBoxCell;
                                            }
                                            catch { }

                                            try
                                            {
                                                if (cell != null)
                                                {
                                                    cell.Value = cells[i];
                                                }
                                                else
                                                {
                                                    row[ColumnIndex] = cells[i];
                                                }
                                            }
                                            catch { }

                                            ColumnIndex++;
                                        }
                                    }
                                }
                                else
                                {
                                    dgv.DataSource = bs;

                                    for (int i = 0; i < cells.Length && ColumnIndex < dgv.Columns.Count; i++)
                                    {
                                        if (i == 0 && cells[i].Trim() == "") continue;

                                        try
                                        {
                                            if (0 <= dgv[ColumnIndex, StartingRow].ValueType.Name.IndexOf("String"))
                                            {
                                                dgv[ColumnIndex, StartingRow].Value = cells[i];
                                            }
                                            else if (0 <= dgv[ColumnIndex, StartingRow].ValueType.Name.IndexOf("Int16"))
                                            {
                                                Int16 n = 0; Int16.TryParse(cells[i].Replace(@"¥", "").Replace(",", ""), out n);
                                                dgv[ColumnIndex, StartingRow].Value = n;
                                            }
                                            else if (0 <= dgv[ColumnIndex, StartingRow].ValueType.Name.IndexOf("Int32"))
                                            {
                                                Int32 n = 0; Int32.TryParse(cells[i].Replace(@"¥", "").Replace(",", ""), out n);
                                                dgv[ColumnIndex, StartingRow].Value = n;
                                            }
                                            else if (0 <= dgv[ColumnIndex, StartingRow].ValueType.Name.IndexOf("Int64"))
                                            {
                                                Int64 n = 0; Int64.TryParse(cells[i].Replace(@"¥", "").Replace(",", ""), out n);
                                                dgv[ColumnIndex, StartingRow].Value = n;
                                            }
                                            else if (0 <= dgv[ColumnIndex, StartingRow].ValueType.Name.IndexOf("UInt16"))
                                            {
                                                UInt16 n = 0; UInt16.TryParse(cells[i].Replace(@"¥", "").Replace(",", ""), out n);
                                                dgv[ColumnIndex, StartingRow].Value = n;
                                            }
                                            else if (0 <= dgv[ColumnIndex, StartingRow].ValueType.Name.IndexOf("UInt32"))
                                            {
                                                UInt32 n = 0; UInt32.TryParse(cells[i].Replace(@"¥", "").Replace(",", ""), out n);
                                                dgv[ColumnIndex, StartingRow].Value = n;
                                            }
                                            else if (0 <= dgv[ColumnIndex, StartingRow].ValueType.Name.IndexOf("UInt64"))
                                            {
                                                UInt64 n = 0; UInt64.TryParse(cells[i].Replace(@"¥", "").Replace(",", ""), out n);
                                                dgv[ColumnIndex, StartingRow].Value = n;
                                            }
                                            else if (0 <= dgv[ColumnIndex, StartingRow].ValueType.Name.IndexOf("Single"))
                                            {
                                                Single n = 0; Single.TryParse(cells[i].Replace(@"¥", "").Replace(",", ""), out n);
                                                dgv[ColumnIndex, StartingRow].Value = n;
                                            }
                                            else if (0 <= dgv[ColumnIndex, StartingRow].ValueType.Name.IndexOf("Double"))
                                            {
                                                Double n = 0; Double.TryParse(cells[i].Replace(@"¥", "").Replace(",", ""), out n);
                                                dgv[ColumnIndex, StartingRow].Value = n;
                                            }
                                            else
                                            {
                                            }
                                            ColumnIndex++;
                                        }
                                        catch //(Exception ex)
                                        {
                                        }
                                    }

                                    dgv.DataSource = null;
                                }
                            }

                            StartingRow++;
                        }

                        dgv.DataSource = bs;
                        dgv.FirstDisplayedScrollingRowIndex = r;
                        dgv.FirstDisplayedScrollingColumnIndex = c;
                        dgv.CurrentCell = dgv[StartingColumn, StartingRow];
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnRowPostPaint(DataGridViewRowPostPaintEventArgs e)
        {
            base.OnRowPostPaint(e);

            if (RowIndexVisible)
            {
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(e.RowBounds.Location.X, e.RowBounds.Location.Y, this.RowHeadersWidth, e.RowBounds.Height);
                TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(), this.RowHeadersDefaultCellStyle.Font, rect, this.RowHeadersDefaultCellStyle.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.Right);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCurrentCellChanged(EventArgs e)
        {
            base.OnCurrentCellChanged(e);

            if (CellVisibleFix)
            {
                int headerWidth = Rows[0].HeaderCell.Size.Width;
                int cellWidth = ClientSize.Width - headerWidth;
                if (CurrentCell != null)
                {
                    int colIndex = CurrentCell.ColumnIndex;

                    // カレントセルが隠れる場合は、右端に表示されるようにする
                    for (int i = colIndex; 0 <= i; i--)
                    {
                        cellWidth -= Columns[i].Width;
                        if (cellWidth <= 0)
                        {
                            FirstDisplayedScrollingColumnIndex = i + 1;
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyData"></param>
        /// <returns></returns>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags = System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & Keys.KeyCode) == Keys.Enter)
            {
                return ProcessTabKey(keyData);
            }

            return base.ProcessDialogKey(keyData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.Demand, Flags = System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
        protected override bool ProcessDataGridViewKey(KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                return ProcessTabKey(e.KeyCode);
            }

            return base.ProcessDataGridViewKey(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Space:
                    {
                        if (SelectedCells.Count == 1)
                        {
                            var cell = SelectedCells[0];

                            if (cell.FormattedValue is bool is_checked)
                            {
                                cell.Value = !is_checked;
                            }
                        }
                    }
                    break;
            }

            base.OnKeyDown(e);
        }
    }
}