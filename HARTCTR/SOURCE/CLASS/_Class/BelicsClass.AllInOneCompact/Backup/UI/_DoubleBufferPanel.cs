using System;
using System.Collections.Generic;
using System.Text;

using System.Windows.Forms;

namespace BelicsClass.UI.Controls
{
    #region ダブルバッファーパネルを設定するパネルクラスです。

    /// <summary>
    /// ダブルバッファーパネルを設定するパネルクラスです。
    /// </summary>
    public class BL_DoubleBufferPanel : Panel
    {
        /// <summary>
        /// コンストラクタです。
        /// </summary>
        public BL_DoubleBufferPanel()
        {
            this.DoubleBuffered = true;

            this.SetStyle(ControlStyles.AllPaintingInWmPaint
                | ControlStyles.OptimizedDoubleBuffer
                | ControlStyles.ResizeRedraw
                | ControlStyles.UserPaint
                , true);
            this.UpdateStyles();
        }
    }

    #endregion
}
