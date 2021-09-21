using System;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;

namespace BelicsClass.UI.Report
{
    /// <summary>
    /// 
    /// </summary>
    public class BL_PrinterPreviewControl : PrintPreviewControl
    {
        Point _oldposition;
        Point _currentposition;
        Type _type;
        MethodInfo _minfo;
        FieldInfo _finfo;

        /// <summary>
        /// 
        /// </summary>
        public BL_PrinterPreviewControl()
        {
            AutoZoom = false;

            _type = typeof(System.Windows.Forms.PrintPreviewControl);
            _finfo = _type.GetField("position", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.ExactBinding);
            _minfo = _type.GetMethod("SetPositionNoInvalidate", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.ExactBinding);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Cursor = Cursors.SizeAll;
                
                Point newpoint = new Point(_currentposition.X, _currentposition.Y);
                newpoint.Offset(_oldposition);
                newpoint.Offset(new Point(-e.X, -e.Y));
                _minfo.Invoke(this, new object[] { newpoint });
            }
            else
            {
                this.Cursor = Cursors.Default;

                _oldposition = new Point(e.X, e.Y);
                _currentposition = (Point)_finfo.GetValue(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseWheel(MouseEventArgs e)
        {
            base.OnMouseWheel(e);

            if (ModifierKeys == Keys.Control)
            {
                if (e.Delta < 0) this.Zoom = this.Zoom * 0.9;
                else this.Zoom = this.Zoom * 1.1;
            }
            else
            {
                Point poi = (Point)_finfo.GetValue(this);
                poi.Y -= e.Delta;
                _minfo.Invoke(this, new object[] { poi });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            base.OnMouseDoubleClick(e);

            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                this.Zoom = this.Zoom * 1.1;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                this.Zoom = this.Zoom * 0.9;
            }
        }

        //protected override void OnKeyDown(KeyEventArgs e)
        //{
        //    base.OnKeyDown(e);

        //    if (e.KeyCode == Keys.Down)
        //    {
        //        Point poi = (Point)_finfo.GetValue(this);
        //        poi.Y += 30;
        //        _minfo.Invoke(this, new object[] { poi });
        //    }
        //    else if (e.KeyCode == Keys.Up)
        //    {
        //        Point poi = (Point)_finfo.GetValue(this);
        //        poi.Y -= 30;
        //        _minfo.Invoke(this, new object[] { poi });
        //    }

        //    else if (e.KeyCode == Keys.Left)
        //    {
        //        Point poi = (Point)_finfo.GetValue(this);
        //        poi.X -= 30;
        //        _minfo.Invoke(this, new object[] { poi });
        //    }

        //    else if (e.KeyCode == Keys.Right)
        //    {
        //        Point poi = (Point)_finfo.GetValue(this);
        //        poi.X += 30;
        //        _minfo.Invoke(this, new object[] { poi });
        //    }
        //}
    }
}
