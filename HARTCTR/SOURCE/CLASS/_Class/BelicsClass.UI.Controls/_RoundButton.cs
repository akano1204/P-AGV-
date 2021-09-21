using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace BelicsClass.UI.Controls
{
    /// <summary>
    /// 角が丸いボタン
    /// </summary>
    [Description("角が丸いボタン")]
    [DefaultEvent("Click")]
    public partial class BL_RoundButton : UserControl
    {
        /// <summary>
        /// イベントデリゲート
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="check"></param>
        public delegate void Event_Handler_CheckedChanged(BL_RoundButton sender, bool check);

        /// <summary>
        /// イベントハンドラー
        /// </summary>
        public virtual event Event_Handler_CheckedChanged CheckChanged;


        /// <summary>
        /// 影の大きさ
        /// </summary>
        private int shadowSize = 6;

        /// <summary>
        /// コーナーの角丸のサイズ（割合）
        /// </summary>
        private double cornerP = 10;

        /// <summary>
        /// コーナーの角丸のサイズ（直径）
        /// </summary>
        private int cornerR = 1;

        /// <summary>
        /// ボタン表面の色
        /// </summary>
        private Color surfaceColor = Color.SlateGray;

        /// <summary>
        /// ボタン表面のハイライトの色
        /// </summary>
        private Color highLightColor = Color.White;

        /// <summary>
        /// ボタン上にマウスが来た時の枠の色
        /// </summary>
        private Color borderColor = Color.Orange;

        /// <summary>
        /// ボタン上にフォーカスが当たっている時の枠の色
        /// </summary>
        private Color focusColor = Color.Blue;

        ///// <summary>
        ///// ボタンの文字列
        ///// </summary>
        //private string buttonText = "RoundButton";

        /// <summary>
        /// マウスが押されている間だけ True
        /// </summary>
        private bool mouseDowning = false;

        //---------------------------------------------------------------------
        #region "デザイン時外部公開プロパティ"

        /// <summary>
        /// 角の丸さを指定します。（半径）
        /// </summary>
        [Category("Appearance")]
        [Browsable(true)]
        [Description("角の丸さを指定します。（半径）")]
        public int CornerR
        {
            get
            {
                if (0 < cornerP)
                {
                    cornerR = (int)((double)Math.Min(Width, Height) / 100.0 * cornerP);
                    if (cornerR == 0) cornerR = 1;
                }
                return (int)(cornerR / 2);
            }
            set
            {
                if (value > 0)
                    cornerR = value * 2;
                else
                    throw new ArgumentException("Corner R", "0 以上の値を入れてください。");

                RenewPadding();
                Refresh();
            }
        }

        /// <summary>
        /// 角の丸さを指定します。（割合％）
        /// </summary>
        [Category("Appearance")]
        [Browsable(true)]
        [Description("角の丸さを指定します。（割合％）")]
        public double CornerP
        {
            get
            {
                return cornerP;
            }
            set
            {
                cornerP = value;
                
                if (cornerP > 0)
                {
                    cornerR = (int)((double)Math.Min(Width, Height) / 100.0 * cornerP);
                    if (cornerR == 0) cornerR = 1;
                }

                RenewPadding();
                Refresh();
            }
        }

        /// <summary>
        /// ボタン表面のハイライトの色を指定します。
        /// </summary>
        [Category("Appearance")]
        [Browsable(true)]
        [Description("ボタン表面のハイライトの色を指定します。")]
        public Color HighLightColor
        {
            get
            {
                return highLightColor;
            }
            set
            {
                highLightColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// ボタン表面の色を指定します。
        /// </summary>
        [Category("Appearance")]
        [Browsable(true)]
        [Description("ボタン表面の色を指定します。")]
        public Color SurfaceColor
        {
            get
            {
                return surfaceColor;
            }
            set
            {
                surfaceColor = value;
                Refresh();
            }
        }

        /// <summary>
        /// ボタン上にマウスが来た時の枠の色を指定します。
        /// </summary>
        [Category("Appearance")]
        [Browsable(true)]
        [Description("ボタン上にマウスが来た時の枠の色を指定します。")]
        public Color BorderColor
        {
            get
            {
                return borderColor;
            }
            set
            {
                borderColor = value;
                //Refresh();
            }
        }

        /// <summary>
        /// フォーカスが当たった時の枠の色を指定します。
        /// </summary>
        [Category("Appearance")]
        [Browsable(true)]
        [Description("フォーカスが当たった時の枠の色を指定します。")]
        public Color FocusColor
        {
            get
            {
                return focusColor;
            }
            set
            {
                focusColor = value;
                //Refresh();
            }
        }

        /// <summary>
        /// ボタンの文字列を指定します。
        /// </summary>
        [Category("Appearance")]
        [Browsable(true)]
        [Description("ボタンの文字列を指定します。")]
        public string ButtonText
        {
            get
            {
                return Text;
            }
            set
            {
                Text = value;
                Refresh();
            }
        }

        /// <summary>
        /// 影の大きさを指定します。
        /// </summary>
        [Category("Appearance")]
        [Browsable(true)]
        [Description("影の大きさを指定します。")]
        public int ShadowSize
        {
            get
            {
                return shadowSize;
            }
            set
            {
                if (value >= 0)
                    shadowSize = value;
                else
                    throw new ArgumentException("ShadowSize", "0 以上の値を入れてください。");

                RenewPadding();
                Refresh();
            }
        }

        /// <summary>
        /// チェックモードの場合 True
        /// </summary>
        private bool checkMode = false;

        /// <summary>
        /// チェック状態の場合 True
        /// </summary>
        private bool check = false;

        /// <summary>
        /// チェックON時のボタン表面の色
        /// </summary>
        private Color surfaceColorON = Color.Lime;

        /// <summary>
        /// チェックON時のボタン表面のハイライトの色
        /// </summary>
        private Color highLightColorON = Color.White;

        /// <summary>
        /// チェックON時のボタン文字の色
        /// </summary>
        private Color foreColorON = Color.Black;

        /// <summary>
        /// チェックOFF時のボタン文字の色
        /// </summary>
        private Color foreColorOFF = Color.Black;

        /// <summary>
        /// ボタンの文字列
        /// </summary>
        private string buttonTextON = "";

        /// <summary>
        /// チェックモードを設定します。
        /// </summary>
        [Category("CheckMode")]
        [Browsable(true)]
        [Description("チェックモードを設定します。")]
        public bool CheckMode
        {
            get
            {
                return checkMode;
            }
            set
            {
                checkMode = value;
            }
        }

        /// <summary>
        /// チェック状態を指定します。
        /// </summary>
        [Category("CheckMode")]
        [Browsable(true)]
        [Description("チェック状態を指定します。")]
        public bool Checked
        {
            get { return check; }
            set
            {
                check = value;
                Refresh();
            }
        }

        /// <summary>
        /// チェックON時のボタン表面の色を指定します。
        /// </summary>
        [Category("CheckMode")]
        [Browsable(true)]
        [Description("チェックON時のボタン表面の色を指定します。")]
        public Color SurfaceColorON
        {
            get
            {
                return surfaceColorON;
            }
            set
            {
                surfaceColorON = value;
                Refresh();
            }
        }

        /// <summary>
        /// チェックON時のボタン表面のハイライトの色を指定します。
        /// </summary>
        [Category("CheckMode")]
        [Browsable(true)]
        [Description("チェックON時のボタン表面のハイライトの色を指定します。")]
        public Color HighLightColorON
        {
            get
            {
                return highLightColorON;
            }
            set
            {
                highLightColorON = value;
                Refresh();
            }
        }

        /// <summary>
        /// チェックON時の文字色を指定します。
        /// </summary>
        [Category("CheckMode")]
        [Browsable(true)]
        [Description("チェックON時の文字色を指定します。")]
        public Color ForeColorON
        {
            get
            {
                return foreColorON;
            }
            set
            {
                foreColorON = value;
                Refresh();
            }
        }

        /// <summary>
        /// ボタンの文字列を指定します。
        /// </summary>
        [Category("CheckMode")]
        [Browsable(true)]
        [Description("ボタンの文字列を指定します。")]
        public string ButtonTextON
        {
            get
            {
                return buttonTextON;
            }
            set
            {
                buttonTextON = value;
                Refresh();
            }
        }

        #endregion
        //---------------------------------------------------------------------


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_RoundButton()
        {
            InitializeComponent();

            // コントロールのサイズが変更された時に Paint イベントを発生させる
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            //SetStyle(ControlStyles., true);

            this.BackColor = Color.Transparent;

            this.DoubleBuffered = true;

            RenewPadding();
            //labelCaption.Text = "RoundButton";
        }

        /// <summary>
        /// マウス Enter イベントのオーバーライド
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);

            Refresh();
            Graphics g = this.CreateGraphics();
            DrawButtonCorner(g, borderColor);
            g.Dispose();
        }

        /// <summary>
        /// マウス Leave イベントのオーバーライド
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            Refresh();

            // マウスが離れてもフォーカスは残っていることがあるので、
            // そのときはフォーカス用の色で枠を塗る
            if (this.Focused)
            {
                Graphics g = this.CreateGraphics();
                DrawButtonCorner(g, focusColor);
                g.Dispose();
            }
        }

        /// <summary>
        /// マウス Down イベントのオーバーライド
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (!mouseDowning && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                if (checkMode)
                {
                    check = !check;
                    if (CheckChanged != null) CheckChanged(this, check);
                }

                mouseDowning = true;
                Refresh();
                Graphics g = this.CreateGraphics();
                DrawButtonSurfaceDown(g);
                //DrawButtonCorner(g, borderColor);
                g.Dispose();
            }

            base.OnMouseDown(e);
        }

        /// <summary>
        /// マウス Up イベントのオーバーライド
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseUp(MouseEventArgs e)
        {
			try
			{
            if (mouseDowning && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                mouseDowning = false;
                Refresh();
                Graphics g = this.CreateGraphics();
                DrawButtonCorner(g, borderColor);
                g.Dispose();
            }

            base.OnMouseUp(e);
        }
			catch
			{ }
		}

        /// <summary>
        /// ボタンが押された状態にします。
        /// </summary>
        public void Push()
        {
            if (!mouseDowning && !checkMode)
            {
                mouseDowning = true;
                Refresh();
                Graphics g = this.CreateGraphics();
                DrawButtonSurfaceDown(g);
                //DrawButtonCorner(g, borderColor);
                g.Dispose();
            }
        }

        /// <summary>
        /// ボタンが離された状態にします。
        /// </summary>
        public void Release()
        {
            if (mouseDowning && !checkMode)
            {
                mouseDowning = false;
                Refresh();
                Graphics g = this.CreateGraphics();
                DrawButtonCorner(g, borderColor);
                g.Dispose();
            }
        }

        /// <summary>
        /// フォーカスが当たった時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (!mouseDowning)
            {
                Graphics g = this.CreateGraphics();
                DrawButtonCorner(g, focusColor);
                g.Dispose();
            }
        }

        /// <summary>
        /// フォーカスを失った時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLostFocus(EventArgs e)
        {
            base.OnLostFocus(e);

            Refresh();
        }

        /// <summary>
        /// キーが押された時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            // スペースキーが押された時は、マウス Down と同じように処理する
            if (!mouseDowning && e.KeyCode == Keys.Space)
            {
                if (checkMode)
                {
                    check = !check;
                    if (CheckChanged != null) CheckChanged(this, check);
                }

                mouseDowning = true;
                Refresh();
                Graphics g = this.CreateGraphics();
                DrawButtonSurfaceDown(g);
                g.Dispose();
            }

            base.OnKeyDown(e);
        }

        /// <summary>
        /// キーが離された時
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyUp(KeyEventArgs e)
        {
            // スペースキーが離された時は、マウス Up と同じように処理する
            // 枠の色だけはフォーカスの色にする
            if (mouseDowning && e.KeyCode == Keys.Space)
            {
                mouseDowning = false;
                Refresh();
                Graphics g = this.CreateGraphics();
                DrawButtonCorner(g, focusColor);
                g.Dispose();
            }

            base.OnKeyUp(e);
        }

        /// <summary>
        /// Paint イベントのオーバーライド
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPaint(PaintEventArgs e)
        {
            //base.OnPaint(e);
            DrawButtonSurface(e.Graphics);
        }

        //---------------------------------------------------------------------
        #region "プライベートメソッド"

        /// <summary>
        /// Padding サイズ更新
        /// </summary>
        private void RenewPadding()
        {
            int harfCornerR = (int)(cornerR / 2);
            int adjust = (int)(Math.Cos(45 * Math.PI / 180) * harfCornerR);
            this.Padding = new Padding(harfCornerR + shadowSize - adjust);
        }

        /// <summary>
        /// ボタンの文字列を描画
        /// </summary>
        /// <param name="g"></param>
        /// <returns></returns>
        private void DrawText(Graphics g)
        {
            // 描画領域の設定
            //Rectangle rectangle = new Rectangle(this.Padding.Left,
            //                                    this.Padding.Top,
            //                                    this.Width - this.Padding.Left - this.Padding.Right,
            //                                    this.Height - this.Padding.Top - this.Padding.Bottom);
            Rectangle rectangle = new Rectangle(0,
                                                0,
                                                this.Width,
                                                this.Height);

            // 文字列が描画領域に収まるように調整
            StringBuilder sb = new StringBuilder();
            StringBuilder sbm = new StringBuilder();

            string text = ButtonText;
            if ((checkMode && check) || mouseDowning)
            {
                if (0 < buttonTextON.Length) text = buttonTextON;
            }

            foreach (char c in text)
            {
                sbm.Append(c);
                Size size = TextRenderer.MeasureText(sbm.ToString(), this.Font);

                if (size.Width > rectangle.Width - this.Font.Size)
                {
                    sbm.Remove(sbm.Length - 1, 1);
                    sbm.Append(c);
                    sbm.AppendLine("");
                    sb.Append(sbm.ToString());
                    sbm = new StringBuilder();
                }
            }
            sb.Append(sbm.ToString());

            // 調整済みの文字列を描画
            if ((checkMode && check) || mouseDowning)
            {
                TextRenderer.DrawText(g,
                    sb.ToString(),
                    this.Font,
                    rectangle,
                    this.foreColorON,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
            else
            {
                TextRenderer.DrawText(g,
                    sb.ToString(),
                    this.Font,
                    rectangle,
                    this.ForeColor,
                    TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter);
            }
        }

        /// <summary>
        /// ボタンの描画品質設定
        /// </summary>
        private void SetSmoothMode(Graphics g)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            //g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
            g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
        }

        /// <summary>
        /// 影用のパスを取得
        /// </summary>
        /// <returns></returns>
        private GraphicsPath GetShadowPath()
        {
            GraphicsPath gp = new GraphicsPath();

            int cr = cornerR;
            //int w = this.Width - cornerR;
            //int h = this.Height - cornerR;
            int w = this.Width - cr;
            int h = this.Height - cr;
            /*
            int ox = this.Width;
            int oy = this.Height;

            if (ox < oy)
            {
                float ratio = (float)shadowSize / oy;
                ox = shadowSize - (int)(ratio * ox);
                oy = 0;
            }
            else
            {
                float ratio = (float)shadowSize / ox;
                ox = 0;
                oy = shadowSize - (int)(ratio * oy);
            }

            gp.AddArc(ox, oy, cornerR, cornerR, 180, 90);
            gp.AddArc(w - ox, oy, cornerR, cornerR, 270, 90);
            gp.AddArc(w - ox, h - oy, cornerR, cornerR, 0, 90);
            gp.AddArc(ox, h - oy, cornerR, cornerR, 90, 90);
            gp.CloseFigure();
            */
            gp.AddArc(0, 0, cr, cr, 180, 90);
            gp.AddArc(w, 0, cr, cr, 270, 90);
            gp.AddArc(w, h, cr, cr, 0, 90);
            gp.AddArc(0, h, cr, cr, 90, 90);
            gp.CloseFigure();

            return gp;
        }

        /// <summary>
        /// 影用のブラシ取得
        /// </summary>
        /// <param name="graphicsPath"></param>
        /// <returns></returns>
        private PathGradientBrush GetShadowBrush(GraphicsPath graphicsPath)
        {
            PathGradientBrush brush = new PathGradientBrush(graphicsPath);
            ColorBlend colorBlend = new ColorBlend();
            float pos = 0;

            if (this.Width < this.Height)
                pos = ((float)shadowSize * 2 / this.Height);
            else
                pos = ((float)shadowSize * 2 / this.Width);

            colorBlend.Positions = new float[3] { 0.0f, pos, 1.0f };

            colorBlend.Colors = new Color[3] { 
					Color.FromArgb(0, Color.White), 
					Color.FromArgb(20, 0, 0, 0),
					Color.FromArgb(20, 0, 0, 0)
			};

            brush.CenterColor = Color.Black;
            brush.CenterPoint = new PointF(this.Width / 2, this.Height / 2);
            brush.InterpolationColors = colorBlend;

            return brush;
        }

        /// <summary>
        /// ボタンの表面描画
        /// </summary>
        /// <param name="g"></param>
        private void DrawButtonSurface(Graphics g)
        {
            // 描画品質設定
            SetSmoothMode(g);

            // 変数初期化
            int cr = cornerR;
            int offset = shadowSize;
            //int w = this.Width - cornerR;
            //int h = this.Height - cornerR;
            int w = this.Width - cr;
            int h = this.Height - cr;
            int harfHeight = (int)(this.Height / 2);

            // 影用のパス初期化
            GraphicsPath shadowPath = null;
            if (shadowSize > 0)
                shadowPath = GetShadowPath();

            // ボタンの表面のパス初期化
            GraphicsPath graphPath = new GraphicsPath();
            graphPath.AddArc(offset, offset, cr, cr, 180, 90);
            graphPath.AddArc(w - offset, offset, cr, cr, 270, 90);
            graphPath.AddArc(w - offset, h - offset, cr, cr, 0, 90);
            graphPath.AddArc(offset, h - offset, cr, cr, 90, 90);
            graphPath.CloseFigure();

            // ボタンのハイライト部分のパス初期化
            offset += 1;
            //cornerR -= 1;
            GraphicsPath graphPath2 = new GraphicsPath();
            graphPath2.AddArc(offset, offset, cr, cr, 180, 90);
            graphPath2.AddArc(w - offset, offset, cr, cr, 270, 90);
            graphPath2.AddLine(this.Width - offset, offset + (int)(cr / 2), this.Width - offset, harfHeight);
            graphPath2.AddLine(offset, harfHeight, offset, harfHeight);
            graphPath2.CloseFigure();


            // 影用のブラシ初期化
            PathGradientBrush shadowBrush = null;
            if (shadowSize > 0)
                shadowBrush = GetShadowBrush(shadowPath);

            // ボタンの表面用のブラシ初期化
            Brush fillBrush2;
            if ((checkMode && check) || mouseDowning)
            {
                fillBrush2 = new SolidBrush(surfaceColorON);
            }
            else
            {
                fillBrush2 = new SolidBrush(surfaceColor);
            }

            // ボタンのハイライト部分用のブラシ初期化
            LinearGradientBrush fillBrush;
            if ((checkMode && check) || mouseDowning)
            {
                fillBrush = new LinearGradientBrush(new Point(0, 0),
                                                        new Point(0, harfHeight + 1),
                                                        Color.FromArgb(255, highLightColorON),
                                                        Color.FromArgb(0, surfaceColorON));
            }
            else
            {
                fillBrush = new LinearGradientBrush(new Point(0, 0),
                                                        new Point(0, harfHeight + 1),
                                                        Color.FromArgb(255, highLightColor),
                                                        Color.FromArgb(0, surfaceColor));
            }

            // 影 → 表面 → ハイライトの順番でパスを塗る
            if (shadowSize > 0)
                g.FillPath(shadowBrush, shadowPath);
            g.FillPath(fillBrush2, graphPath);
            g.FillPath(fillBrush, graphPath2);

            // ボタンの文字列描画
            DrawText(g);

            // 後処理
            if (shadowSize > 0)
                shadowPath.Dispose();
            graphPath.Dispose();
            graphPath2.Dispose();

            if (shadowSize > 0)
                shadowBrush.Dispose();
            fillBrush.Dispose();
            fillBrush2.Dispose();
        }

        /// <summary>
        /// ボタンの表面描画　（マウス Down イベント時）
        /// </summary>
        /// <param name="g"></param>
        private void DrawButtonSurfaceDown(Graphics g)
        {
            // 描画品質設定
            SetSmoothMode(g);

            // 変数初期化
            int cr = cornerR;
            int offset = shadowSize;
            //int w = this.Width - cornerR;
            //int h = this.Height - cornerR;
            int w = this.Width - cr;
            int h = this.Height - cr;


            // ボタンの表面用のブラシ初期化
            Brush fillBrush = new SolidBrush(Color.FromArgb(30, Color.Black));

            GraphicsPath graphPath = new GraphicsPath();
            graphPath.AddArc(offset, offset, cr, cr, 180, 90);
            graphPath.AddArc(w - offset, offset, cr, cr, 270, 90);
            graphPath.AddArc(w - offset, h - offset, cr, cr, 0, 90);
            graphPath.AddArc(offset, h - offset, cr, cr, 90, 90);
            graphPath.CloseFigure();

            g.FillPath(fillBrush, graphPath);

            graphPath.Dispose();
            fillBrush.Dispose();
        }

        /// <summary>
        /// マウスがボタンの領域に入った時に、枠に色をつける処理
        /// と、フォーカスが当たっている時に、枠に色をつける処理用
        /// </summary>
        /// <param name="g"></param>
        /// <param name="color"></param>
        private void DrawButtonCorner(Graphics g, Color color)
        {
            // 描画品質設定
            SetSmoothMode(g);

            // 変数初期化
            int cr = cornerR;
            int offset = shadowSize;
            //int w = this.Width - cornerR;
            //int h = this.Height - cornerR;
            int w = this.Width - cr;
            int h = this.Height - cr;

            // ペンの初期化
            Pen cornerPen = new Pen(Color.FromArgb(128, color), 2);
            //Pen cornerPen = new Pen(color, 1.5f);

            // 描画領域の初期化
            GraphicsPath graphPath = new GraphicsPath();
            graphPath.AddArc(offset, offset, cr, cr, 180, 90);
            graphPath.AddArc(w - offset, offset, cr, cr, 270, 90);
            graphPath.AddArc(w - offset, h - offset, cr, cr, 0, 90);
            graphPath.AddArc(offset, h - offset, cr, cr, 90, 90);
            graphPath.CloseFigure();

            // 描画
            g.DrawPath(cornerPen, graphPath);

            // 後処理
            graphPath.Dispose();
            cornerPen.Dispose();
        }

        #endregion

        /// <summary>
        /// サイズ変更時の処理をします。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnResize(EventArgs e)
        {
            if (0 < cornerP)
            {
                cornerR = (int)((double)Math.Min(Width, Height) / 100.0 * cornerP);
                if (cornerR == 0) cornerR = 1;
                Refresh();
            }

            base.OnResize(e);
        }

        //---------------------------------------------------------------------

    }
}
