using System;
using System.Windows.Forms;

namespace BelicsClass.UI.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class BL_DoubleBufferDataGridView : DataGridView
    {
        /// <summary>
        /// 
        /// </summary>
        public BL_DoubleBufferDataGridView()
            : base()
        {
            DoubleBuffered = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnCellFormatting(DataGridViewCellFormattingEventArgs e)
        {
            if (typeof(DateTime).IsInstanceOfType(e.Value))
            {
                if (((DateTime)e.Value).Year < 1900)
                {
                    if (((DateTime)e.Value).ToString("fff") == "000")
                    {
                        e.Value = ((DateTime)e.Value).ToString("HH:mm:ss");
                    }
                    else
                    {
                        e.Value = ((DateTime)e.Value).ToString("HH:mm:ss.fff");
                    }
                }
                else if (((DateTime)e.Value).ToString("HH:mm:ss.fff") == "00:00:00.000")
                {
                    e.Value = ((DateTime)e.Value).ToString("yyyy/MM/dd");
                }
                else if (((DateTime)e.Value).ToString("fff") == "000")
                {
                    e.Value = ((DateTime)e.Value).ToString("yyyy/MM/dd HH:mm:ss");
                }
                else
                {
                    e.Value = ((DateTime)e.Value).ToString("yyyy/MM/dd HH:mm:ss.fff");
                }
            }

            base.OnCellFormatting(e);
        }
    }
}
