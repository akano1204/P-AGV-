using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using BelicsClass.Common;

namespace BelicsClass.UI.Controls
{
	/// <summary>
	/// 仮想リストビューを簡易表示するためのリストビュー継承クラス。
	/// 内部のコレクションに表示データを格納することで、リストビューが表示されます。
	/// </summary>
	public class BL_VirtualListView : BL_DoubleBufferListView
	{
		/// <summary>
		/// 奇数行の背景色
		/// </summary>
		public Color StripeBackColor1 { get { return stripeBackColor1; } set { stripeBackColor1 = value; } }
		private Color stripeBackColor1 = Color.LightSkyBlue;

		/// <summary>
		/// 偶数行の背景色
		/// </summary>
		public Color StripeBackColor2 { get { return stripeBackColor2; } set { stripeBackColor2 = value; } }
		private Color stripeBackColor2 = Color.White;

		/// <summary>
		/// 奇数行・偶数行で交互に色分け表示するかどうか
		/// </summary>
		public bool IsStripeColored { get { return isStripeColored; } set { isStripeColored = value; } }
		private bool isStripeColored = false;

		/// <summary></summary>
		protected const int WM_MOUSEMOVE = 0x200;
		/// <summary></summary>
		protected const int WM_MOUSEHOVER = 0x2A1;

		/// <summary>
		/// リストビューのアイテムリスト
		/// </summary>
		public class BL_VirtualListViewItemList : List<BL_VirtualListViewItem>
		{
			/// <summary>
			/// リストビューコントロール
			/// </summary>
			public BL_VirtualListView parent = null;

			/// <summary>
			/// 保持データの最大数を指定して、データを追加します
			/// </summary>
			/// <param name="item"></param>
			/// <param name="limit"></param>
			public void AddLimit(BL_VirtualListViewItem item, int limit)
			{
				int count = this.Count;
				this.Add(item);
				while (limit < this.Count) this.RemoveAt(0);
				if (count == this.Count && parent != null)
				{
					parent.SortedItems.Clear();
					parent.RefreshMe(false);
				}
			}

			/// <summary>
			/// 保持データの最大数を指定して、データ追加します
			/// </summary>
			/// <param name="items"></param>
			/// <param name="limit"></param>
			public void AddLimit(BL_VirtualListViewItem[] items, int limit)
			{
				int count = this.Count;
				foreach (var item in items) this.Add(item);
				while (limit < this.Count) this.RemoveAt(0);
				if (count == this.Count && parent != null)
				{
					parent.SortedItems.Clear();
					parent.RefreshMe(false);
				}
			}

			/// <summary>
			/// 保持データの最大数を指定して、データ追加します
			/// </summary>
			/// <param name="items"></param>
			/// <param name="limit"></param>
			public void AddLimit(SortedList<string, BL_VirtualListView.BL_VirtualListViewItem> items, int limit)
			{
				int count = this.Count;
				foreach (var item in items) this.Add(item.Value);
				while (limit < this.Count) this.RemoveAt(0);
				if (count == this.Count && parent != null)
				{
					parent.SortedItems.Clear();
					parent.RefreshMe(false);
				}
			}

			/// <summary>
			/// 保持データの最大数を指定して、データを追加します
			/// </summary>
			/// <param name="index"></param>
			/// <param name="item"></param>
			/// <param name="limit"></param>
			public void InsertLimit(int index, BL_VirtualListViewItem item, int limit)
			{
				int count = this.Count;
				this.Insert(index, item);
				while (limit < this.Count) this.RemoveAt(this.Count - 1);
				if (count == this.Count && parent != null)
				{
					parent.SortedItems.Clear();
					parent.RefreshMe(false);
				}
			}

			/// <summary>
			/// 保持データの最大数を指定して、データ追加します
			/// </summary>
			/// <param name="index"></param>
			/// <param name="items"></param>
			/// <param name="limit"></param>
			public void InsertLimit(int index, BL_VirtualListViewItem[] items, int limit)
			{
				int count = this.Count;
				foreach (var item in items) this.Insert(index, item);
				while (limit < this.Count) this.RemoveAt(this.Count - 1);
				if (count == this.Count && parent != null)
				{
					parent.SortedItems.Clear();
					parent.RefreshMe(false);
				}
			}

			/// <summary>
			/// 保持データの最大数を指定して、データ追加します
			/// </summary>
			/// <param name="index"></param>
			/// <param name="items"></param>
			/// <param name="limit"></param>
			public int InsertLimit(int index, SortedList<string, BL_VirtualListView.BL_VirtualListViewItem> items, int limit)
			{
				int count = this.Count;
				int addcount = 0;
				foreach (var item in items)
				{
					this.Insert(index, item.Value);
					addcount++;
				}

				while (limit < this.Count) this.RemoveAt(this.Count - 1);
				if (count == this.Count && parent != null)
				{
					parent.SortedItems.Clear();
					parent.RefreshMe(false);
				}

				return addcount;
			}
		}

		/// <summary>
		/// 選択アイテムリスト
		/// </summary>
		public new BL_VirtualListViewItemList SelectedItems = new BL_VirtualListViewItemList();

		private ListViewItemCollection m_Col = null;
		private SelectedIndexCollection m_Sel = null;

		/// <summary>
		/// １カラムのデータを保持するクラス
		/// 列別に色変更ができるようになります。
		/// </summary>
		public class BL_LvwCell
		{
			/// <summary>データ</summary>
			public object Data = null;
			/// <summary></summary>
			public object Tag = null;
			/// <summary>背景色</summary>
			public Color BackColor = Color.Transparent;
			/// <summary>表示色</summary>
			public Color ForeColor = Color.Transparent;
			/// <summary>表示フォント</summary>
			public Font font = null;

			/// <summary>コンストラクタ</summary>
			public BL_LvwCell() { }

			/// <summary>コンストラクタ</summary>
			/// <param name="data"></param>
			public BL_LvwCell(object data) { this.Data = data; }

			/// <summary>
			/// 
			/// </summary>
			/// <returns></returns>
			public override string ToString()
			{
				if (typeof(DateTime).IsInstanceOfType(Data))
				{
					return ((DateTime)Data).ToString("yyyy/MM/dd HH:mm:ss.fff");
				}

				return Data.ToString();
			}
		}

		/// <summary>
		/// リストビューのアイテム
		/// </summary>
		public class BL_VirtualListViewItem : List<object>
		{
			/// <summary></summary>
			public object Tag = null;

			/// <summary>
			/// 行背景色
			/// </summary>
			public Color BackColor = Color.Transparent;

			/// <summary>
			/// 行表示色
			/// </summary>
			public Color ForeColor = Color.Transparent;

			/// <summary>
			/// コンストラクタ
			/// </summary>
			public BL_VirtualListViewItem() { }

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="item"></param>
			public BL_VirtualListViewItem(object item)
			{
				Tag = item;

				if (item.GetType().IsArray)
				{
					Array arr = (Array)item;
					foreach (var element in arr)
					{
						Add(element);
					}
				}
				else
				{
					Add(item);
				}
			}
		}

		/// <summary>
		/// リスト保持データ
		/// </summary>
		new public BL_VirtualListViewItemList Items = new BL_VirtualListViewItemList();

		/// <summary>
		/// リスト保持データ
		/// </summary>
		public BL_VirtualListViewItemList SortedItems = new BL_VirtualListViewItemList();

		/// <summary>
		/// ソート適用中カラム
		/// </summary>
		public int SortedColumn = -1;

		/// <summary>
		/// 再描画が必要かどうか
		/// </summary>
		public bool NeedRedraw = false;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BL_VirtualListView()
			: base()
		{
			SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

			View = View.Details;
			VirtualMode = true;
			FullRowSelect = true;
			GridLines = true;
			HideSelection = false;
			ResizeRedraw = true;
			OwnerDraw = true;

			m_Col = new ListViewItemCollection(this);
			m_Sel = new SelectedIndexCollection(this);

			RetrieveVirtualItem += new RetrieveVirtualItemEventHandler(this.VirtualListView_RetrieveVirtualItem);

			Items.parent = this;
			SortedItems.parent = this;
		}

		/// <summary>
		/// デストラクタ
		/// </summary>
		~BL_VirtualListView()
		{
			RetrieveVirtualItem -= new RetrieveVirtualItemEventHandler(this.VirtualListView_RetrieveVirtualItem);
		}

		/// <summary>
		/// 
		/// </summary>
		public void ItemsClear()
		{
			//SortedColumn = -1;
			//this.Sorting = SortOrder.None;
			base.SelectedIndices.Clear();
			base.Items.Clear();
			this.Items.Clear();
			NeedRedraw = true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item0"></param>
		public void EnsureVisible(string item0)
		{
			for (int i = 0; i < base.Items.Count; i++)
			{
				ListViewItem lvi = base.Items[i];
				if (lvi.Text == item0)
				{
					EnsureVisible(lvi.Index);
					break;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public void EnsureVisible(BL_VirtualListViewItem item)
		{
			for (int i = 0; i < SortedItems.Count; i++)
			{
				BL_VirtualListViewItem lvi = SortedItems[i];
				if (lvi == item)
				{
					EnsureVisible(i);
					break;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item0"></param>
		public void SelectItem(string item0)
		{
			SelectedIndices.Clear();
			for (int i = 0; i < SortedItems.Count; i++)
			{
				BL_VirtualListViewItem lvi = SortedItems[i];
				if (lvi[0].ToString() == item0)
				{
					SelectedIndices.Add(i);
					break;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="item"></param>
		public void SelectItem(BL_VirtualListViewItem item)
		{
			SelectedIndices.Clear();
			for (int i = 0; i < SortedItems.Count; i++)
			{
				BL_VirtualListViewItem lvi = SortedItems[i];
				if (lvi == item)
				{
					SelectedIndices.Add(i);
					break;
				}
			}
		}

		/// <summary>
		/// カラム・リストアイテム数を再設定する
		/// </summary>
		public virtual void RefreshMe()
		{
			RefreshMe(true);
		}

		/// <summary>
		/// カラム・リストアイテム数を再設定する
		/// </summary>
		public virtual void RefreshMe(bool reset_select)
		{
			try
			{
				if (reset_select)
				{
					m_Sel.Clear();
					m_Col.Clear();
					SelectedItems.Clear();
					SelectedIndices.Clear();
				}

				//BeginUpdate();

				sort_item();

				VirtualListSize = Items.Count;
				Refresh();

				//EndUpdate();
			}
			catch { }
		}

		private SortOrder sorted = SortOrder.None;
		private int sorted_column = -1;

		private void sort_item()
		{
			if (sorted == this.Sorting && SortedItems.Count == Items.Count && sorted_column == SortedColumn && !NeedRedraw) return;

			Cursor curbak = this.Cursor;
			this.Cursor = Cursors.WaitCursor;

			NeedRedraw = false;

			int num = 0;
			int c = SortedColumn;
			bool isnum = true;

			foreach (var v in Items)
			{
				if (v.Count <= c) return;
			}

			SortedItems.Clear();
			foreach (var v in Items)
			{
				if (0 <= c)
				{
					string n = v[c].ToString().Replace(",", "").Replace("\\", "");
					if (!int.TryParse(n, out num)) isnum = false;
				}
				SortedItems.Add(v);
			}

			if (0 <= c)
			{
				if (this.Sorting == SortOrder.Ascending)
				{
					if (isnum) SortedItems.Sort(delegate(BL_VirtualListViewItem mca1, BL_VirtualListViewItem mca2) { return int.Parse(mca1[c].ToString().Replace(",", "").Replace("\\", "")) - int.Parse(mca2[c].ToString().Replace(",", "").Replace("\\", "")); });
					else SortedItems.Sort(delegate(BL_VirtualListViewItem mca1, BL_VirtualListViewItem mca2) { return string.Compare(mca1[c].ToString(), mca2[c].ToString()); });
				}
				else if (this.Sorting == SortOrder.Descending)
				{
					if (isnum) SortedItems.Sort(delegate(BL_VirtualListViewItem mca1, BL_VirtualListViewItem mca2) { return int.Parse(mca2[c].ToString().Replace(",", "").Replace("\\", "")) - int.Parse(mca1[c].ToString().Replace(",", "").Replace("\\", "")); });
					else SortedItems.Sort(delegate(BL_VirtualListViewItem mca1, BL_VirtualListViewItem mca2) { return string.Compare(mca2[c].ToString(), mca1[c].ToString()); });
				}
			}

			sorted = this.Sorting;
			sorted_column = this.SortedColumn;

			this.Cursor = curbak;
		}

		/// <summary>
		/// 描画データをセットする
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		protected virtual void VirtualListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
		{
			sort_item();

			if (e.ItemIndex < SortedItems.Count)
			{
				if (typeof(BL_VirtualListViewItem).IsInstanceOfType(SortedItems[e.ItemIndex]))
				{
					BL_VirtualListViewItem item = (BL_VirtualListViewItem)SortedItems[e.ItemIndex];

					//if (0 < item.Count)
					{
						ListViewItem lvi = new ListViewItem(item[0].ToString());
						for (int i = 1; i < this.Columns.Count; i++)
						{
							if (i < item.Count)
							{
								if (item[i] != null)
								{
									lvi.SubItems.Add(item[i].ToString());
								}
								else
								{
									lvi.SubItems.Add("");
								}
							}
							else lvi.SubItems.Add("");
						}
						e.Item = lvi;

						if (isStripeColored)
						{
							if (e.ItemIndex % 2 != 0) SortedItems[e.ItemIndex].BackColor = stripeBackColor1;
							if (e.ItemIndex % 2 == 0) SortedItems[e.ItemIndex].BackColor = stripeBackColor2;
						}
					}
				}
			}
			else
			{
				ListViewItem lvi = new ListViewItem("???");
				for (int i = 1; i < this.Columns.Count; i++)
				{
					lvi.SubItems.Add("???");
				}
				e.Item = lvi;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSelectedIndexChanged(EventArgs e)
		{
			SelectedItems.Clear();
			for (int i = 0; i < m_Sel.Count; i++)
			{
				SelectedItems.Add(SortedItems[m_Sel[i]]);
			}

			base.OnSelectedIndexChanged(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSizeChanged(EventArgs e)
		{
			if (OwnerDraw)
			{
				if (Columns.Count == 1)
				{
					Columns[0].Width = Width - SystemInformation.VerticalScrollBarWidth - SystemInformation.Border3DSize.Width * 2;
				}
			}
			base.OnSizeChanged(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			// http://social.msdn.microsoft.com/Forums/en/winforms/thread/e3490e28-7c11-4e74-8dbd-a6c6913cf884
			// kill WM_MOUSEMOVE event in listview control when it is owner drawn.
			// this does solve the issue with the extra draw event which did lead to crappy MS example code where
			// the item under the mouse was redrawn every time the mouse did move.
			if (OwnerDraw)
			{
				if (m.Msg == WM_MOUSEMOVE || m.Msg == WM_MOUSEHOVER) return;
			}

			//try
			{
				base.WndProc(ref m);
			}
			//catch { }
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnColumnClick(ColumnClickEventArgs e)
		{
			int c = e.Column;

			if (c != SortedColumn) this.Sorting = SortOrder.None;

			if (this.Sorting == SortOrder.None)
			{
				this.Sorting = SortOrder.Ascending;
				SortedColumn = c;
			}
			else if (this.Sorting == SortOrder.Ascending)
			{
				this.Sorting = SortOrder.Descending;
				SortedColumn = c;
			}
			else if (this.Sorting == SortOrder.Descending)
			{
				this.Sorting = SortOrder.None;
				SortedColumn = -1;
			}

			base.OnColumnClick(e);

			sort_item();

			RefreshMe();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
		{
			if (e.Bounds.Width == 0) return;
			e.DrawBackground();

			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			switch (e.Header.TextAlign)
			{
				case HorizontalAlignment.Left:
					sf.Alignment = StringAlignment.Near;
					break;
				case HorizontalAlignment.Center:
					sf.Alignment = StringAlignment.Center;
					break;
				case HorizontalAlignment.Right:
					sf.Alignment = StringAlignment.Far;
					break;
			}

			if (e.ColumnIndex == SortedColumn)
			{
				e.Graphics.FillRectangle(SystemBrushes.Highlight, e.Bounds);
				e.Graphics.DrawRectangle(SystemPens.ButtonHighlight, e.Bounds);

				if (this.Sorting == SortOrder.Ascending)
				{
					e.Graphics.DrawString("▴" + e.Header.Text, e.Font, SystemBrushes.HighlightText, e.Bounds, sf);
					//e.Graphics.DrawString("↓" + e.Header.Text, e.Font, Brushes.Black, e.Bounds, sf);
				}
				else if (this.Sorting == SortOrder.Descending)
				{
					e.Graphics.DrawString("▾" + e.Header.Text, e.Font, SystemBrushes.HighlightText, e.Bounds, sf);
					//e.Graphics.DrawString("↑" + e.Header.Text, e.Font, Brushes.Black, e.Bounds, sf);
				}
			}
			else
			{
				e.Graphics.FillRectangle(SystemBrushes.ButtonFace, e.Bounds);
				e.Graphics.DrawRectangle(SystemPens.ButtonHighlight, e.Bounds);

				e.Graphics.DrawString(e.Header.Text, e.Font, SystemBrushes.ControlText, e.Bounds, sf);
			}

			//base.OnDrawColumnHeader(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDrawItem(DrawListViewItemEventArgs e)
		{
			if (SelectedIndices.Contains(e.ItemIndex))
			{
				e.DrawFocusRectangle();
			}

			base.OnDrawItem(e);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e"></param>
		protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
		{
			StringFormat sf = new StringFormat();
			sf.Alignment = StringAlignment.Center;
			switch (e.Header.TextAlign)
			{
				case HorizontalAlignment.Left:
					sf.Alignment = StringAlignment.Near;
					break;
				case HorizontalAlignment.Center:
					sf.Alignment = StringAlignment.Center;
					break;
				case HorizontalAlignment.Right:
					sf.Alignment = StringAlignment.Far;
					break;
			}


			if (SelectedIndices.Contains(e.ItemIndex))
			{
				Color bc = SystemColors.Highlight;
				Color fc = SystemColors.HighlightText;
				Font font = e.SubItem.Font;

				if (e.ItemIndex < SortedItems.Count)
				{
					if (e.ColumnIndex < SortedItems[e.ItemIndex].Count)
					{
						if (typeof(BL_LvwCell).IsInstanceOfType(SortedItems[e.ItemIndex][e.ColumnIndex]))
						{
							BL_LvwCell cell = (BL_LvwCell)SortedItems[e.ItemIndex][e.ColumnIndex];
							if (cell.BackColor != Color.Transparent) bc = cell.BackColor;
							if (cell.ForeColor != Color.Transparent) fc = cell.ForeColor;
							if (cell.font != null) font = cell.font;
						}
					}
				}

				e.Graphics.FillRectangle(new SolidBrush(bc), e.Bounds);
				e.Graphics.DrawString(e.SubItem.Text, font, new SolidBrush(fc), e.Bounds, sf);
			}
			else
			{
				e.DrawBackground();

				Color bc = BackColor;
				Color fc = ForeColor;
				Font font = e.SubItem.Font;

				if (e.ItemIndex < SortedItems.Count)
				{
					if (SortedItems[e.ItemIndex].BackColor != Color.Transparent) bc = SortedItems[e.ItemIndex].BackColor;
					if (SortedItems[e.ItemIndex].ForeColor != Color.Transparent) fc = SortedItems[e.ItemIndex].ForeColor;

					if (e.ColumnIndex < SortedItems[e.ItemIndex].Count)
					{
						if (typeof(BL_LvwCell).IsInstanceOfType(SortedItems[e.ItemIndex][e.ColumnIndex]))
						{
							BL_LvwCell cell = (BL_LvwCell)SortedItems[e.ItemIndex][e.ColumnIndex];
							if (cell.BackColor != Color.Transparent) bc = cell.BackColor;
							if (cell.ForeColor != Color.Transparent) fc = cell.ForeColor;
							if (cell.font != null) font = cell.font;
						}
					}
				}

				e.Graphics.FillRectangle(new SolidBrush(bc), e.Bounds);
				e.Graphics.DrawString(e.SubItem.Text, font, new SolidBrush(fc), e.Bounds, sf);
			}

			//base.OnDrawSubItem(e);
		}

		/// <summary>
		/// 列幅の設定状態をファイルへ保存します。
		/// </summary>
		/// <param name="path"></param>
		public void SaveColumnWidth(string path)
		{
			BL_BinarySerializableList<int> widthes = new BL_BinarySerializableList<int>();

			foreach (ColumnHeader v in Columns)
			{
				widthes.Add(v.Width);
			}

			widthes.Save(path);
		}

		/// <summary>
		/// 列幅の設定状態をファイルから読み込んで復元します。
		/// </summary>
		/// <param name="path"></param>
		public void LoadColumnWidth(string path)
		{
			BL_BinarySerializableList<int> widthes = new BL_BinarySerializableList<int>();
			widthes.Load(path);

			for (int c = 0; c < widthes.Count; c++)
			{
				if (c < Columns.Count)
				{
					Columns[c].Width = widthes[c];
				}
			}

			Refresh();
		}

		/// <summary>
		/// 選択行をコピーします。
		/// </summary>
		/// <param name="e"></param>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			if ((e.Modifiers == Keys.Control && e.KeyCode == Keys.C) ||
				(e.Modifiers == Keys.Control && e.KeyCode == Keys.Insert))
			{
				if (SelectedIndices.Count == 0) return;

				string copytext = "";

				for (int c = 0; c < Columns.Count; c++)
				{
					if (copytext != "") copytext += "\t";
					copytext += Columns[c].Text;
				}
				copytext += "\n";

				foreach (int index in SelectedIndices)
				//foreach (BL_VirtualListViewItem item in SelectedIndices)
				{
					BL_VirtualListViewItem item = SortedItems[index];

					string linetext = "";
					foreach (object v in item)
					{
						if (linetext != "") linetext += "\t";
						linetext += v.ToString();
					}
					linetext += "\n";
					copytext += linetext;
				}

				Clipboard.SetText(copytext);
			}

			base.OnKeyDown(e);
		}
	}
}
