using System;
using System.Collections.Generic;
using System.Text;

using System.Drawing;
using System.Windows.Forms;

namespace BelicsClass.UI.Controls
{
    /// <summary>
    /// 行毎に交互に背景色が異なるリストビュークラス
    /// </summary>
    public class BL_StripeListView : ListView
    {
        /// <summary>
        /// 奇数行の背景色
        /// </summary>
        public Color StripeBackColor1 = Color.LightSkyBlue;

        /// <summary>
        /// 偶数行の背景色
        /// </summary>
        public Color StripeBackColor2 = Color.White;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_StripeListView()
            : base()
        {
            this.OwnerDraw = true;
            this.DoubleBuffered = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            e.DrawDefault = true;
            base.OnDrawColumnHeader(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            if (e.ItemIndex % 2 == 0)
            {
                e.Item.BackColor = StripeBackColor2;
            }
            else
            {
                e.Item.BackColor = StripeBackColor1;
            }

            e.DrawDefault = true;
            base.OnDrawItem(e);
        }
    }
}
