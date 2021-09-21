using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace BelicsClass.UI
{
    /// <summary>
    /// ジェスチャー機能付きメインフォーム
    /// </summary>
    public class BL_Touchable_Form_Base : Form
    {
        private bool touch_enabled = false;
        private BL_TouchControl touch = new BL_TouchControl();

        private Point beginLocation = new Point(0, 0);
        private Size baseSize;
        private Point startCenter;
        private double baseDelta;


        #region 初期化

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_Touchable_Form_Base(bool enable_gesture)
            : base()
        {
            this.touch_enabled = enable_gesture;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (touch_enabled)
            {
                touch.GestureBegin += touch_GestureBegin;
                touch.GestureEnd += touch_GestureEnd;
                touch.GesturePan += touch_GesturePan;
                touch.GestureZoom += touch_GestureZoom;

                // 基準となるパラメータを記録する
                baseSize = this.Size;
                startCenter = new Point(this.Location.X + (baseSize.Width / 2), this.Location.Y + (baseSize.Height / 2));
                baseDelta = Math.Sqrt(Math.Pow((double)baseSize.Width, 2) + Math.Pow((double)baseSize.Height, 2));

                touch.StartControl(this, true);
            }
        }

        void touch_GestureZoom(object sender, BL_TouchControl.WMGestureEventArgs e)
        {
            double sizeDelta = (baseDelta + Math.Sqrt(Math.Pow((double)(e.LocationX - beginLocation.X), 2) + Math.Pow((double)(e.LocationY - beginLocation.Y), 2))) / baseDelta;

            // 新しいサイズにする
            //this.Size = new Size((int)(baseSize.Width * sizeDelta), (int)(baseSize.Height * sizeDelta));
            this.ScaleControl(new SizeF((float)(baseSize.Width * sizeDelta), (float)(baseSize.Height * sizeDelta)), BoundsSpecified.Size);

            // 新しい中心点を得る
            this.Location = new Point(startCenter.X - (this.Size.Width / 2), startCenter.Y - (this.Size.Height / 2));
        }

        void touch_GesturePan(BL_TouchControl sender, BL_TouchControl.WMGestureEventArgs e)
        {
            
        }

        void touch_GestureEnd(BL_TouchControl sender, BL_TouchControl.WMGestureEventArgs e)
        {
            
        }

        void touch_GestureBegin(BL_TouchControl sender, BL_TouchControl.WMGestureEventArgs e)
        {
            // ジェスチャ開始位置を記録する
            beginLocation = new Point(e.LocationX, e.LocationY);
        }

        /// <summary>
        /// 解放
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (touch_enabled)
                {
                    touch.StopControl();
                }
            }
            base.Dispose(disposing);
        }

        #endregion
    }
}
