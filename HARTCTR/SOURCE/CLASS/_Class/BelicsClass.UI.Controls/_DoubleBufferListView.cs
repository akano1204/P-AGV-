using System;
using System.Windows.Forms;

namespace BelicsClass.UI.Controls
{
    /// <summary>
    /// ダブルバッファリングを有効にするためのリストビュー継承クラス。
    /// ダブルバッファリングを実現できる点を除き、通常のListView機能に違いはありません。
    /// </summary>
    public class BL_DoubleBufferListView : ListView
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_DoubleBufferListView()
            : base()
        {
            DoubleBuffered = true;
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // DoubleBufferListView
            // 
            this.FullRowSelect = true;
            this.ResumeLayout(false);
        }
    }
}