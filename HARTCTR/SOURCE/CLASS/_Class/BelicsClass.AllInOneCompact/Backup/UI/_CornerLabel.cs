using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace BelicsClass.UI.Controls
{
    /// <summary>
    /// コーナー部分の丸みを再現するためのラベルクラス
    /// </summary>
    public partial class BL_CornerLabel : UserControl
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_CornerLabel()
        {
            InitializeComponent();

            DoubleBuffered = true;

            this.SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //this.BackColor = Color.White; // 透明
            //this.BackColor = Color.FromArgb(100, 255, 255, 255); // 半透明
            //this.BorderStyle = BorderStyle.None;
            //this.BackColor = this.Parent.BackColor;
            //this.BackColor = Color.Transparent;
            //pe.Graphics.Clear(this.BackColor);
        }

        /// <summary>
        /// 形状
        /// </summary>
        public enum LabelStyle
        {
            /// <summary>
            /// 方形
            /// </summary>
            Flat,
            /// <summary>
            /// 左上
            /// </summary>
            LeftTop,
            /// <summary>
            /// 左下
            /// </summary>
            LeftBottom,
            /// <summary>
            /// 右上
            /// </summary>
            RightTop,
            /// <summary>
            /// 右下
            /// </summary>
            RightBottom
        }

        private int BlockNo; // = 0;
        /// <summary>
        /// ブロック番号
        /// </summary>
        public int Ex_BlockNo
        {
            get
            {
                return BlockNo;
            }
            set
            {
                BlockNo = value;
                this.Invalidate();
            }
        }

        private int Monitor = 1;

        /// <summary>
        /// モニタタイプ
        /// 0:モニタ無し 1:モニタ有り
        /// </summary>
        public int Ex_Monitor
        {
            get
            {
                return Monitor;
            }
            set
            {
                Monitor = value;
                this.Invalidate();
            }
        }

        private LabelStyle LabelShape = LabelStyle.Flat;
        /// <summary>
        /// 形状
        /// </summary>
        public LabelStyle Ex_LabelShape
        {
            get
            {
                return LabelShape;
            }
            set
            {
                LabelShape = value;
                this.Invalidate();
            }
        }

        private int Radius = 1;
        /// <summary>
        /// 内側円弧の内径
        /// </summary>
        public int Ex_Radius
        {
            get
            {
                return Radius;
            }
            set
            {
                Radius = value;
                this.Invalidate();
            }
        }

        private Color ShapeColor = Color.Green;
        /// <summary>
        ///形状塗りつぶし色 
        /// </summary>
        public Color Ex_ShapeColor
        {
            get
            {
                return ShapeColor;
            }
            set
            {
                ShapeColor = value;
                this.Invalidate();
            }
        }

        private int LineWidth = 1;
        /// <summary>
        /// 輪郭描画時の線幅
        /// </summary>
        public int Ex_LineWith
        {
            get
            {
                return LineWidth;
            }
            set
            {
                LineWidth = value;
            }
        }

        private Color LineColor = Color.Black;
        /// <summary>
        /// 輪郭描画時の線色
        /// </summary>
        public Color Ex_LineColor
        {
            get
            {
                return LineColor;
            }
            set
            {
                LineColor = value;
                this.Invalidate();
            }
        }

        private bool AllowTransparency; // = false;
        /// <summary>
        /// 背景が透明な時、背面のコントロールを描画するか否か
        /// </summary>
        public bool Ex_AllowTransparency
        {
            get
            {
                return AllowTransparency;
            }
            set
            {
                if (AllowTransparency == value)
                {
                    return;
                }
                AllowTransparency = value;
                this.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaint(PaintEventArgs pevent)
        {
            this.DrawCornerLabel(pevent);
            base.OnPaint(pevent);
        }

        //---------------------------------------------------------------------------------------------------
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pevent"></param>
        protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
        {
            base.OnPaintBackground(pevent);
            // 親がいない場合は無視
            if (this.Parent == null) return;
            Point offset = new Point(this.Left, this.Top);
            // 原点を親コントロールの座標へ
            pevent.Graphics.TranslateTransform(-offset.X, -offset.Y);
            // 親コントロールを描画
            this.InvokePaintBackground(this.Parent, pevent);
            this.InvokePaint(this.Parent, pevent);
            // 原点の座標を戻す
            pevent.Graphics.TranslateTransform(offset.X, offset.Y);
            // 各背面コントロールを描画
            for (int i = this.Parent.Controls.Count - 1; i >= 0; --i)
            {
                Control c = this.Parent.Controls[i];
                if (c == this) break; // 背面コントロールの描画終わり
                if (!c.Visible) continue; // 対象のコントロールが非表示
                // 対象のコントロールが描画領域に含まれているか
                if (c.Bounds.IntersectsWith(this.Bounds))
                {
                    // 背面コントロールを描画
                    DrawControl(c, pevent);
                }
            }
        }

        private void DrawControl(Control c, PaintEventArgs pevent)
        {
            Point offset = new Point(this.Left - c.Left, this.Top - c.Top);
            // 原点を背面コントロールの座標へ
            pevent.Graphics.TranslateTransform(-offset.X, -offset.Y);
            // コントロールを描画
            this.InvokePaintBackground(c, pevent);
            this.InvokePaint(c, pevent);
            // 子コントロールを描画
            for (int j = c.Controls.Count - 1; j >= 0; --j)
            {
                Control child = c.Controls[j];
                if (!child.Visible) continue; // 対象のコントロールが非表示
                DrawControl(child, pevent);
            }
            // 原点の座標を戻す
            pevent.Graphics.TranslateTransform(offset.X, offset.Y);
        }

        /// <summary>
        /// コントロールの背景を描画する
        /// </summary>
        /// <param name="pevent">描画先のコントロールに関連する情報</param>
        private void DrawBackground(System.Windows.Forms.PaintEventArgs pevent)
        {
            // 背景色
            using (SolidBrush sb = new SolidBrush(this.BackColor))
            {
                pevent.Graphics.FillRectangle(sb, this.ClientRectangle);
            }
            // 背景画像
            if (this.BackgroundImage != null)
            {
                this.DrawBackgroundImage(pevent.Graphics, this.BackgroundImage, this.BackgroundImageLayout);
            }
        }

        /// <summary>
        /// コントロールの背景画像を描画する
        /// </summary>
        /// <param name="g">描画に使用するグラフィックス オブジェクト</param>
        /// <param name="img">描画する画像</param>
        /// <param name="layout">画像のレイアウト</param>
        private void DrawBackgroundImage(Graphics g, Image img, ImageLayout layout)
        {
            Size imgSize = img.Size;
            switch (layout)
            {
                case ImageLayout.None:
                    g.DrawImage(img, 0, 0, imgSize.Width, imgSize.Height);
                    break;
                case ImageLayout.Tile:
                    int xCount = Convert.ToInt32(Math.Ceiling((double)this.ClientRectangle.Width / (double)imgSize.Width));
                    int yCount = Convert.ToInt32(Math.Ceiling((double)this.ClientRectangle.Height / (double)imgSize.Height));
                    for (int x = 0; x <= xCount - 1; x++)
                    {
                        for (int y = 0; y <= yCount - 1; y++)
                        {
                            g.DrawImage(img, imgSize.Width * x, imgSize.Height * y, imgSize.Width, imgSize.Height);
                        }
                    }
                    break;
                case ImageLayout.Center:
                    {
                        int x = 0;
                        if (this.ClientRectangle.Width > imgSize.Width)
                        {
                            x = (int)Math.Floor((double)(this.ClientRectangle.Width - imgSize.Width) / 2.0);
                        }
                        int y = 0;
                        if (this.ClientRectangle.Height > imgSize.Height)
                        {
                            y = (int)Math.Floor((double)(this.ClientRectangle.Height - imgSize.Height) / 2.0);
                        }
                        g.DrawImage(img, x, y, imgSize.Width, imgSize.Height);
                        break;
                    }
                case ImageLayout.Stretch:
                    g.DrawImage(img, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);
                    break;
                case ImageLayout.Zoom:
                    {
                        double xRatio = (double)this.ClientRectangle.Width / (double)imgSize.Width;
                        double yRatio = (double)this.ClientRectangle.Height / (double)imgSize.Height;
                        double minRatio = Math.Min(xRatio, yRatio);
                        Size zoomSize = new Size(Convert.ToInt32(Math.Ceiling(imgSize.Width * minRatio)),
                        Convert.ToInt32(Math.Ceiling(imgSize.Height * minRatio)));
                        int x = 0;
                        if (this.ClientRectangle.Width > zoomSize.Width)
                        {
                            x = (int)Math.Floor((double)(this.ClientRectangle.Width - zoomSize.Width) / 2.0);
                        }
                        int y = 0;
                        if (this.ClientRectangle.Height > zoomSize.Height)
                        {
                            y = (int)Math.Floor((double)(this.ClientRectangle.Height - zoomSize.Height) / 2.0);
                        }
                        g.DrawImage(img, x, y, zoomSize.Width, zoomSize.Height);
                        break;
                    }
            }
        }

        /// <summary>
        /// 親コントロールと背面にあるコントロールを描画する
        /// </summary>
        /// <param name="pevent">描画先のコントロールに関連する情報</param>
        private void DrawParentWithBackControl(System.Windows.Forms.PaintEventArgs pevent)
        {
            // 親コントロールを描画
            this.DrawParentControl(this.Parent, pevent);
            // 親コントロールとの間のコントロールを親側から描画
            for (int i = this.Parent.Controls.Count - 1; i >= 0; i--)
            {
                Control c = this.Parent.Controls[i];
                if (c == this)
                {
                    break;
                }
                if (this.Bounds.IntersectsWith(c.Bounds) == false)
                {
                    continue;
                }
                this.DrawBackControl(c, pevent);
            }
        }

        /// <summary>
        /// 親コントロールを描画する
        /// </summary>
        /// <param name="c">親コントロール</param>
        /// <param name="pevent">描画先のコントロールに関連する情報</param>
        private void DrawParentControl(Control c, System.Windows.Forms.PaintEventArgs pevent)
        {
            using (Bitmap bmp = new Bitmap(c.Width, c.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    using (PaintEventArgs p = new PaintEventArgs(g, c.ClientRectangle))
                    {
                        this.InvokePaintBackground(c, p);
                        this.InvokePaint(c, p);
                    }
                }
                int offsetX = this.Left + (int)Math.Floor((double)(this.Bounds.Width - this.ClientRectangle.Width) / 2.0);
                int offsetY = this.Top + (int)Math.Floor((double)(this.Bounds.Height - this.ClientRectangle.Height) / 2.0);
                pevent.Graphics.DrawImage(bmp, this.ClientRectangle, new Rectangle(offsetX, offsetY, this.ClientRectangle.Width, this.ClientRectangle.Height), GraphicsUnit.Pixel);
            }
        }

        /// <summary>
        /// 背面のコントロールを描画する
        /// </summary>
        /// <param name="c">背面のコントロール</param>
        /// <param name="pevent">描画先のコントロールに関連する情報</param>
        private void DrawBackControl(Control c, System.Windows.Forms.PaintEventArgs pevent)
        {
            using (Bitmap bmp = new Bitmap(c.Width, c.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
            {
                c.DrawToBitmap(bmp, new Rectangle(0, 0, c.Width, c.Height));
                int offsetX = (c.Left - this.Left) - (int)Math.Floor((double)(this.Bounds.Width - this.ClientRectangle.Width) / 2.0);
                int offsetY = (c.Top - this.Top) - (int)Math.Floor((double)(this.Bounds.Height - this.ClientRectangle.Height) / 2.0);
                pevent.Graphics.DrawImage(bmp, offsetX, offsetY, c.Width, c.Height);
            }
        }

        //---------------------------------------------------------------------------------------------------

        /// <summary>
        /// CornerLabelを描画する
        /// </summary>
        /// <param name="pevent">描画先のコントロールに関連する情報</param>
        private void DrawCornerLabel(PaintEventArgs pevent)
        {
            float _x1, _x2;
            float _y1, _y2;
            float _w1, _h1;
            float _rad = (float)this.Ex_Radius;
            float _Width = (float)this.Width;
            float _Height = (float)this.Height;

            float _PenWidthHalf = (float)this.LineWidth / 2.0f;
            this.BorderStyle = BorderStyle.None; //.None; //.FixedSingle;
            this.BackColor = Color.Transparent;

            this.SetBounds(this.Left, this.Top, this.Width, this.Height, BoundsSpecified.Size);
            using (System.Drawing.Drawing2D.GraphicsPath _path = new System.Drawing.Drawing2D.GraphicsPath())
            {

                switch (LabelShape)
                {
                    case LabelStyle.Flat:
                        _x1 = 0 + _PenWidthHalf;
                        _y1 = 0;
                        _x2 = 0 + _PenWidthHalf;
                        _y2 = _Height;
                        _path.AddLine(_x1, _y1, _x2, _y2);

                        _x1 = 0;
                        _y1 = _Height - _PenWidthHalf;
                        _x2 = _Width;
                        _y2 = _Height - _PenWidthHalf;
                        _path.AddLine(_x1, _y1, _x2, _y2);

                        _x1 = _Width - _PenWidthHalf;
                        _y1 = _Height;
                        _x2 = _Width - _PenWidthHalf;
                        _y2 = 0;
                        _path.AddLine(_x1, _y1, _x2, _y2);

                        _x1 = _Width;
                        _y1 = 0 + _PenWidthHalf;
                        _x2 = 0;
                        _y2 = 0 + _PenWidthHalf;
                        _path.AddLine(_x1, _y1, _x2, _y2);

                        using (SolidBrush _sb = new SolidBrush(this.ShapeColor))
                        {
                            using (Pen _p = new Pen(this.LineColor, (float)this.LineWidth))
                            {
                                pevent.Graphics.FillPath(_sb, _path);
                                pevent.Graphics.DrawPath(_p, _path);
                            }
                        }

                        break;

                    case LabelStyle.LeftTop:
                        _x1 = 0 + _PenWidthHalf;
                        _y1 = 0 + _PenWidthHalf;
                        _w1 = (_Width - _PenWidthHalf) * 2;
                        _h1 = (_Height - _PenWidthHalf) * 2;
                        _path.AddArc(_x1, _y1, _w1, _h1, 180, 90);

                        _x1 = _Width - _PenWidthHalf;
                        _y1 = 0 + _PenWidthHalf;
                        _x2 = _Width - _PenWidthHalf;
                        _y2 = _Height - _rad - _PenWidthHalf;
                        _path.AddLine(_x1, _y1, _x2, _y2);

                        _x1 = _Width - _rad - _PenWidthHalf;
                        _y1 = _Height - _rad - _PenWidthHalf;
                        _w1 = (_rad - _PenWidthHalf) * 2;
                        _h1 = (_rad - _PenWidthHalf) * 2;
                        _path.AddArc(_x1, _y1, _w1, _h1, 270, -90);


                        _x1 = _Width - _rad - _PenWidthHalf;
                        _y1 = _Height - _PenWidthHalf;
                        _x2 = 0 + _PenWidthHalf;
                        _y2 = _Height - _PenWidthHalf;
                        _path.AddLine(_x1, _y1, _x2, _y2);

                        using (SolidBrush _sb = new SolidBrush(this.ShapeColor))
                        {
                            using (Pen _p = new Pen(this.LineColor, (float)this.LineWidth))
                            {
                                pevent.Graphics.FillPath(_sb, _path);
                                pevent.Graphics.DrawPath(_p, _path);
                            }
                        }

                        break;

                    case LabelStyle.LeftBottom:

                        _x1 = 0 + _PenWidthHalf;
                        _y1 = 0 - _Height + _PenWidthHalf;
                        _w1 = (_Width - _PenWidthHalf) * 2;
                        _h1 = (_Height - _PenWidthHalf) * 2;

                        _path.AddArc(_x1, _y1, _w1, _h1, 90, 90);

                        _x1 = 0 + _PenWidthHalf;
                        _y1 = 0 + _PenWidthHalf;
                        _x2 = _Width - _rad - _PenWidthHalf;
                        _y2 = 0 + _PenWidthHalf;
                        _path.AddLine(_x1, _y1, _x2, _y2);

                        _x1 = _Width - _rad + _PenWidthHalf;
                        _y1 = 0 - _rad + _PenWidthHalf;
                        _w1 = (_rad - _PenWidthHalf) * 2;
                        _h1 = (_rad - _PenWidthHalf) * 2;
                        _path.AddArc(_x1, _y1, _w1, _h1, 180, -90);

                        _x1 = _Width - _PenWidthHalf;
                        _y1 = _Height;
                        _x2 = _Width - _PenWidthHalf;
                        _y2 = _Height - _rad - _PenWidthHalf;
                        _path.AddLine(_x1, _y1, _x2, _y2);

                        using (SolidBrush _sb = new SolidBrush(this.ShapeColor))
                        {
                            using (Pen _p = new Pen(this.LineColor, (float)this.LineWidth))
                            {
                                pevent.Graphics.FillPath(_sb, _path);
                                pevent.Graphics.DrawPath(_p, _path);
                            }
                        }

                        break;

                    case LabelStyle.RightTop:
                        _x1 = -_Width + _PenWidthHalf;
                        _y1 = 0 + _PenWidthHalf;
                        _w1 = (_Width - _PenWidthHalf) * 2;
                        _h1 = (_Height - _PenWidthHalf) * 2;
                        _path.AddArc(_x1, _y1, _w1, _h1, 270, 90);

                        _x1 = _Width - _PenWidthHalf;
                        _y1 = _Height - _PenWidthHalf;
                        _x2 = _Width - _rad - _PenWidthHalf;
                        _y2 = _Height - _PenWidthHalf;
                        _path.AddLine(_x1, _y1, _x2, _y2);

                        _x1 = 0 - _rad + _PenWidthHalf;
                        _y1 = _Height - _rad + _PenWidthHalf;
                        _w1 = (_rad - _PenWidthHalf) * 2;
                        _h1 = (_rad - _PenWidthHalf) * 2;
                        _path.AddArc(_x1, _y1, _w1, _h1, 360, -90);

                        _x1 = 0 + _PenWidthHalf;
                        _y1 = _Height - _rad - _PenWidthHalf;
                        _x2 = 0 + _PenWidthHalf;
                        _y2 = 0 + _PenWidthHalf;
                        _path.AddLine(_x1, _y1, _x2, _y2);

                        using (SolidBrush _sb = new SolidBrush(this.ShapeColor))
                        {
                            using (Pen _p = new Pen(this.LineColor, (float)this.LineWidth))
                            {
                                pevent.Graphics.FillPath(_sb, _path);
                                pevent.Graphics.DrawPath(_p, _path);
                            }
                        }

                        break;

                    case LabelStyle.RightBottom:
                        _x1 = -_Width + _PenWidthHalf;
                        _y1 = -_Height + _PenWidthHalf;
                        _w1 = (_Width - _PenWidthHalf) * 2;
                        _h1 = (_Height - _PenWidthHalf) * 2;
                        _path.AddArc(_x1, _y1, _w1, _h1, 0, 90);

                        _x1 = 0 + _PenWidthHalf;
                        _y1 = _Height - _PenWidthHalf;
                        _x2 = 0 + _PenWidthHalf;
                        _y2 = _Height - _rad + _PenWidthHalf;
                        _path.AddLine(_x1, _y1, _x2, _y2);

                        _x1 = -_rad + _PenWidthHalf;
                        _y1 = -_rad + _PenWidthHalf;
                        _w1 = (_rad - _PenWidthHalf) * 2;
                        _h1 = (_rad - _PenWidthHalf) * 2;

                        _path.AddArc(_x1, _y1, _w1, _h1, 90, -90);

                        _x1 = _Width - _rad + _PenWidthHalf;
                        _y1 = 0 + _PenWidthHalf;
                        _x2 = _Width - _PenWidthHalf;
                        _y2 = 0 + _PenWidthHalf;
                        _path.AddLine(_x1, _y1, _x2, _y2);

                        using (SolidBrush _sb = new SolidBrush(this.ShapeColor))
                        {
                            using (Pen _p = new Pen(this.LineColor, (float)this.LineWidth))
                            {
                                pevent.Graphics.FillPath(_sb, _path);
                                pevent.Graphics.DrawPath(_p, _path);
                            }
                        }

                        break;

                }
            }

        }

        /// <summary>
        /// 指定したブロック番号の背景色を変更する
        /// </summary>
        /// <param name="form">コントロールのコレクション</param>
        /// <param name="block_no">ブロック</param>
        /// <param name="color">背景色</param>
        public static void GroupColorChange(Form form, int block_no, Color color)
        {
            foreach (Control control in form.Controls)
            {
                if (control is BL_CornerLabel)
                {
                    if (((BL_CornerLabel)control).Ex_BlockNo == block_no && ((BL_CornerLabel)control).Ex_Monitor == 1)
                    {
                        ((BL_CornerLabel)control).Ex_ShapeColor = color;
                    }
                }
            }
        }

    }
}
