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
    public partial class SubForm_ModeSelecter : BelicsClass.UI.BL_SubForm_Base
    {
        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "[F1]:ﾓｰﾄﾞ選択", "", "", "", "", "", "", "", "", "", "", "[F12]:戻る" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get { return this.Text; }
        }

        public SubForm_ModeSelecter()
        {
            InitializeComponent();
            Resizer_Initialize();
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            Close();
        }

        protected override void SubForm_Base_Function01_Clicked(object sender)
        {
            base.SubForm_Base_Function01_Clicked(sender);

            if (listviewMode.SelectedIndices.Count == 0) return;

            AgvControlManager.MoveMode moveMode = listviewMode.SortedItems[listviewMode.SelectedIndices[0]].Tag as AgvControlManager.MoveMode;
            if (moveMode == null) return;

            Program.controller.SetMoveMode(moveMode.mode);
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            if (Program.controller.run_manager == null)
            {
                m_bLoadCancel = true;
                return;
            }
                
            listviewMode.ItemsClear();
            foreach (var v in AgvControlManager.moveModes)
            {
                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                item.Add(v.Value.name);
                item.Add(v.Value.option);
                item.Tag = v.Value;

                listviewMode.Items.Add(item);
            }

            listviewMode.RefreshMe();
        }
    }
}
