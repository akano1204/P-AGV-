using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace BelicsClass.UI.Controls
{
    /// <summary>
    /// 透過楕円描画
    /// </summary>
	public partial class BL_TransparentEllipse : UserControl
	{
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_TransparentEllipse()
		{
			InitializeComponent();
		}

		private void TransparentEllipse_Paint(object sender, PaintEventArgs e)
		{
			Rectangle rect = this.DisplayRectangle;
			rect.Inflate(-2, -2);
			Pen pen = new Pen(Color.Black, 2.0f);
			e.Graphics.DrawEllipse(pen, rect);
			pen.Dispose();
		}
	}
}
