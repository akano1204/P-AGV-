using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BelicsClass.UI;

namespace PROGRAM
{
    using Algebra = PROGRAM.AgvControlManager.Algebra;
    using Vector = PROGRAM.AgvControlManager.Vector;
    using Matrix = PROGRAM.AgvControlManager.Matrix;
    using BackGround = PROGRAM.AgvControlManager.BackGround;

    public partial class SubForm_SetCoordinate : BL_SubForm_Base, INotifyPropertyChanged
    {
        #region サブフォーム用プロパティ

        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "[F12]:適用" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get
            {
                return this.Text;
            }
        }

        #endregion

        #region プロパティ変化通知

        public List<string> RegisteredEventName = new List<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string propertyName)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void SetProperty<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (Equals(field, value)) return;

            T original = field;

            field = value;

            try
            {
                RaisePropertyChanged(propertyName);
            }
            catch
            {
                field = original;
            }
        }

        #endregion

        #region フィールド・プロパティ

        #region IO

        public Bitmap Image { get; set; }

        private float _rx = 0.0f;
        public float RX
        {
            get => _rx;
            set => SetProperty(ref _rx, value);
        }

        private float _ry = 0.0f;
        public float RY
        {
            get => _ry;
            set => SetProperty(ref _ry, value);
        }

        private float _scale = 0.0f;
        public new float Scale
        {
            get => _scale;
            set => SetProperty(ref _scale, value);
        }

        private float _angle = 0.0f;
        public float Angle
        {
            get => _angle;
            set => SetProperty(ref _angle, value);
        }

        private int _opacity = 70;
        public new int Opacity
        {
            get => _opacity;
            set
            {
                SetProperty(ref _opacity, value);
                panelMap.Invalidate();
            }
        }

        public BackGround BackGround { get; set; }

        #endregion

        #region ピン止め座標

        private float _p1x = 0.0f;
        public float P1X
        {
            get => _p1x;
            set { SetProperty(ref _p1x, value); Pin(1); }
        }

        private float _p1y = 0.0f;
        public float P1Y
        {
            get => _p1y;
            set { SetProperty(ref _p1y, value); Pin(1); }
        }

        private float _p2x = 0.0f;
        public float P2X
        {
            get => _p2x;
            set { SetProperty(ref _p2x, value); Pin(2); }
        }

        private float _p2y = 0.0f;
        public float P2Y
        {
            get => _p2y;
            set { SetProperty(ref _p2y, value); Pin(2); }
        }

        private float _q1x = 0.0f;
        public float Q1X
        {
            get => _q1x;
            set { SetProperty(ref _q1x, value); Pin(1); }
        }

        private float _q1y = 0.0f;
        public float Q1Y
        {
            get => _q1y;
            set { SetProperty(ref _q1y, value); Pin(1); }
        }

        private float _q2x = 0.0f;
        public float Q2X
        {
            get => _q2x;
            set { SetProperty(ref _q2x, value); Pin(2); }
        }

        private float _q2y = 0.0f;
        public float Q2Y
        {
            get => _q2y;
            set { SetProperty(ref _q2y, value); Pin(2); }
        }

        private SolidBrush mark1;
        private SolidBrush mark2;

        private SolidBrush mark_string1;
        private SolidBrush mark_string2;

        #endregion

        #region 画面操作

        private PointF offset;

        private float scale = 1.0f;

        private float max_scale = 10.0f;
        private float min_scale = 0.1f;

        private int max_grid_base = 0;

        private PointF mpd = new PointF(0.0f, 0.0f);

        private PointF mpc = new PointF(0.0f, 0.0f);

        private bool mdown = false;

        #endregion

        #endregion

        #region コンストラクタ

        public SubForm_SetCoordinate()
		{
			InitializeComponent();

            DialogResult = DialogResult.Cancel;
		}

        #endregion

        #region ロード

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            panelMap.MouseWheel += panelMap_MouseWheel;

            trkOpacity.DataBindings.Add(new Binding
                (
                nameof(TrackBar.Value),
                this,
                nameof(Opacity),
                false,
                DataSourceUpdateMode.OnPropertyChanged
                )
            );

            var num_combs = new Dictionary<NumericUpDown, string>
            {
                { numP1X, nameof(P1X) },
                { numP1Y, nameof(P1Y) },
                { numP2X, nameof(P2X) },
                { numP2Y, nameof(P2Y) },
                { numQ1X, nameof(Q1X) },
                { numQ1Y, nameof(Q1Y) },
                { numQ2X, nameof(Q2X) },
                { numQ2Y, nameof(Q2Y) },
            };

            foreach (var comb in num_combs)
            {
                var num = comb.Key;
                var prop = comb.Value;

                num.DataBindings.Add(new Binding
                    (
                    nameof(NumericUpDown.Value),
                    this,
                    prop,
                    false,
                    DataSourceUpdateMode.OnPropertyChanged
                    )
                );
            }

            max_grid_base = GetGridDivBase(max_scale);

            var w = Image.Width;
            var h = Image.Height;

            var base_w = panelMap.Width;
            var base_h = panelMap.Height;

            var rate_w = (double)w / base_w;
            var rate_h = (double)h / base_h;

            if (1.0 < rate_h / rate_w)
            {
                scale = (float)base_h / h;
                offset.X = (int)(-(w * scale - base_w) / 2.0);
            }
            else
            {
                scale = (float)base_w / w;
                offset.Y = (int)(-(h * scale - base_h) / 2.0);
            }

            mark1 = new SolidBrush(Color.FromArgb(128, Color.Red));
            mark2 = new SolidBrush(Color.FromArgb(128, Color.Blue));

            mark_string1 = new SolidBrush(Color.FromArgb(255, Color.Red));
            mark_string2 = new SolidBrush(Color.FromArgb(255, Color.Blue));

            P1X = w / 4.0f;
            P2X = w * 3.0f / 4.0f;
            P1Y = P2Y = h / 2.0f;

            Q1X = P1X;
            Q1Y = P1Y;
            Q2X = P2X;
            Q2Y = P2Y;
        }

        protected override void SubForm_Base_FormClosing(object sender, FormClosingEventArgs e)
        {
            base.SubForm_Base_FormClosing(sender, e);

            mark1?.Dispose();
            mark2?.Dispose();
            mark_string1?.Dispose();
            mark_string2?.Dispose();

            mark1 = null;
            mark2 = null;
            mark_string1 = null;
            mark_string2 = null;
        }

        #endregion

        #region ファンクションキー操作

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            if (float.IsInfinity(Scale))
            {
                BL_MessageBox.Show(this, "拡縮率が無限大です。\nピクセル座標を見直してください。");
                return;
            }

            m_Mainform.DialogResult = DialogResult.OK;

            if (BackGround != null)
            {
                BackGround.BackGroundRX = RX;
                BackGround.BackGroundRY = RY;
                BackGround.BackGroundScaleW = Scale;
                BackGround.BackGroundScaleH = Scale;
                BackGround.BackGroundAngle = Angle;
            }

            ExitApplication();
        }

        #endregion

        #region 描画

        private void panelMap_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;

            int w = Image.Width;
            int h = Image.Height;

            g.TranslateTransform(0.0f, 0.0f, MatrixOrder.Append);
            g.ScaleTransform(scale, scale, MatrixOrder.Append);
            g.RotateTransform(0.0f, MatrixOrder.Append);
            g.TranslateTransform(offset.X, offset.Y, MatrixOrder.Append);

            var cm = new ColorMatrix
            {
                Matrix00 = 1,
                Matrix11 = 1,
                Matrix22 = 1,
                Matrix33 = Opacity / 100.0f,
                Matrix44 = 1
            };

            var ia = new ImageAttributes();
            ia.SetColorMatrix(cm);

            g.DrawImage(Image,
                new Rectangle(0, 0, w, h),
                0, 0, w, h,
                GraphicsUnit.Pixel, ia);

            var inv_y1 = h - P1Y;
            var inv_y2 = h - P2Y;

            g.FillRectangle(mark1, (int)Math.Floor(P1X), (int)Math.Floor(inv_y1), 1, 1);
            g.FillRectangle(mark2, (int)Math.Floor(P2X), (int)Math.Floor(inv_y2), 1, 1);

            var shift = 30.0f / scale;
            var size = 20.0f / scale;
            var font = new Font(SystemFonts.DefaultFont.FontFamily, size);

            g.DrawString("①", font, mark_string1, (int)Math.Floor(P1X - shift), (int)Math.Floor(inv_y1 - shift));
            g.DrawString("②", font, mark_string2, (int)Math.Floor(P2X - shift), (int)Math.Floor(inv_y2 - shift));

            var div = GetGridDivision(scale);

            using (var p = new Pen(Color.Black, 2.0f / scale))
            {
                g.DrawRectangle(p, new Rectangle(0, 0, w, h));
            }

            if (Math.Max(w, h) / div > 3)
            {
                using (var p = new Pen(Color.DimGray, 1.0f / scale))
                {
                    for (int c = 1; c <= w / div; c++)
                    {
                        var p1 = new PointF(c * div, 0);
                        var p2 = new PointF(c * div, h);

                        g.DrawLine(p, p1, p2);
                    }

                    for (int r = 1; r <= h / div; r++)
                    {
                        var p1 = new PointF(0, r * div);
                        var p2 = new PointF(w, r * div);

                        g.DrawLine(p, p1, p2);
                    }
                }
            }
        }

        private int GetGridDivBase(float scale)
        {
            var ratio = 1.3;
            return (int)Math.Floor(Math.Log(scale / min_scale, ratio));
        }

        private int GetGridDivision(float scale)
        {
            var div_base = GetGridDivBase(scale);

            return (int)Math.Pow(2.0, max_grid_base - div_base);
        }

		#endregion

		#region 画面操作イベント

		private void panelMap_MouseMove(object sender, MouseEventArgs e)
        {
            panelMap.Invalidate();

            if (mdown)
            {
                var move = new PointF(e.X - mpd.X, e.Y - mpd.Y);

                offset.X = mpc.X + move.X;
                offset.Y = mpc.Y + move.Y;
            }
        }

        private void panelMap_MouseDown(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        mdown = true;

                        mpd.X = e.X;
                        mpd.Y = e.Y;

                        mpc.X = offset.X;
                        mpc.Y = offset.Y;
                    }
                    break;
            }
        }

        private void panelMap_MouseUp(object sender, MouseEventArgs e)
        {
            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        mdown = false;
                    }
                    break;
            }
        }

        private void panelMap_MouseWheel(object sender, MouseEventArgs e)
        {
            float scale_pre = scale;

            if (0 < e.Delta)
            {
                scale = ((int)(scale * 100) * 1.1f + 0.5f) / 100;
                if (max_scale < scale) scale = max_scale;
                if (0.95f < scale && scale < 1.05f) scale = 1.0f;
            }
            else if (e.Delta < 0)
            {
                scale = ((int)(scale * 100) * 0.9f + 0.5f) / 100;
                if (scale < min_scale) scale = min_scale;
                if (0.95f < scale && scale < 1.05f) scale = 1.0f;
            }

            if (scale_pre != scale)
            {
                offset.X = e.X - (e.X - offset.X) * scale / scale_pre;
                offset.Y = e.Y - (e.Y - offset.Y) * scale / scale_pre;
            }

            panelMap.Invalidate();
        }

        private void panelMap_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var px = (e.X - offset.X) / scale;
            var py = (e.Y - offset.Y) / scale;

            var h = Image.Height;

            switch (e.Button)
            {
                case MouseButtons.Left:
                    {
                        P1X = px;
                        P1Y = (h - py);
                    }
                    break;

                case MouseButtons.Right:
                    {
                        P2X = px;
                        P2Y = (h - py);
                    }
                    break;
            }
        }

        #endregion

        #region 補助イベント

        private void num_Enter(object sender, EventArgs e)
        {
            var num = sender as NumericUpDown;

            num.Select(0, num.Text.Length);
        }

        #endregion

        #region ピン止め座標算出

        private void Pin(int no)
        {
            var w = Image.Width;
            var h = Image.Height;

            var cv = new Vector(2);
            var p1v = new Vector(2);
            var p2v = new Vector(2);
            var q1v = new Vector(2);
            var q2v = new Vector(2);

            cv[1] = w / 2;
            cv[2] = h / 2;

            p1v[1] = P1X;
            p1v[2] = P1Y;
            p2v[1] = P2X;
            p2v[2] = P2Y;
            q1v[1] = Q1X;
            q1v[2] = Q1Y;
            q2v[1] = Q2X;
            q2v[2] = Q2Y;

            var dpv = p2v - p1v;
            var dqv = q2v - q1v;
            
            Scale = (float)(dqv.Abs / dpv.Abs);
            Angle = (float)Algebra.GetRoundedDegree(dqv.AngleX - dpv.AngleX);

            var p1cv = cv - p1v;
            var rot = p1cv.Rot2(Angle);
            var shiftv = Scale * rot;

            RX = Q1X + (float)shiftv[1];
            RY = Q1Y + (float)shiftv[2];

            panelMap.Invalidate();
        }

        #endregion
    }
}
