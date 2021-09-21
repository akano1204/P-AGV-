using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.PowerPacks;
using System.IO;

using BelicsClass.ProcessManage;
using BelicsClass.UI.Controls;
using BelicsClass.UI.Graph;
using BelicsClass.Common;
using System.Security.Permissions;

namespace BelicsClass.UI
{
    /// <summary>
    /// メインフォームの基本クラスです。
    /// </summary>
    public partial class BL_MainForm_Base : Form
    {
        #region 自動スケーリング機能

        /// <summary>
        /// フォームの自動スケーリング機能を提供します
        /// </summary>
        public class ViewResize
        {
            /// <summary>
            /// コントロールの基準値を保持します
            /// </summary>
            public class ControlContainer
            {
                /// <summary></summary>
                public Rectangle bounds;
                /// <summary></summary>
                public Font font;
                /// <summary></summary>
                public Padding margin;
                /// <summary></summary>
                public Padding padding;
                /// <summary></summary>
                public Size tabitemsize;
                /// <summary></summary>
                public List<int> columnsizes = new List<int>();
                /// <summary></summary>
				public int columnheaderheight;
                /// <summary></summary>
                public int rowheaderwidth;
                /// <summary></summary>
                public int rowheight;
                /// <summary></summary>
                public int splitwidth;
                /// <summary></summary>
                public int splitdistance;
            }

            private Control parent = null;

            bool horizontal = true;
            bool virtical = true;

            #region コンストラクタ

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="frmWidth">フォーム幅初期値</param>
            /// <param name="frmHeight">フォーム高さ初期値</param>
            public ViewResize(int frmWidth, int frmHeight)
            {
                FrmWidth = frmWidth;
                FrmHeight = frmHeight;

                if (FrmWidth <= 0) FrmWidth = 1;
                if (FrmHeight <= 0) FrmHeight = 1;

                Controls = new Dictionary<object, ControlContainer>();
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="parent"></param>
            /// <param name="horizontal"></param>
            /// <param name="virtical"></param>
			public ViewResize(Control parent, bool horizontal, bool virtical)
            {
                this.parent = parent;
                this.horizontal = horizontal;
                this.virtical = virtical;

                FrmWidth = parent.ClientSize.Width;
                FrmHeight = parent.ClientSize.Height;

                if (FrmWidth <= 0) FrmWidth = 1;
                if (FrmHeight <= 0) FrmHeight = 1;

                Controls = new Dictionary<object, ControlContainer>();

                Add(parent.Controls);
            }

            #endregion

            #region フィールド

            /// <summary>
            /// フォーム幅初期値
            /// </summary>
            private int FrmWidth;
            /// <summary>
            /// フォーム高さ初期値
            /// </summary>
            private int FrmHeight;

            private Dictionary<object, ControlContainer> Controls;

            /// <summary></summary>
            public double lastScaleWidth = 0.0;

            /// <summary></summary>
            public double lastScaleHeight = 0.0;

            #endregion

            #region メソッド

            /// <summary>
            /// サイズ調整をするコントロールを登録します
            /// </summary>
            /// <param name="controls">サイズ調整を行うコントロールコレクション</param>
            /// <returns></returns>
            public void Add(Control.ControlCollection controls)
            {
                if (controls == null) return;
                foreach (Control ctl in controls) { this.Add(ctl); }
            }

            /// <summary>
            /// サイズ調整をするコントロールを登録します
            /// </summary>
            /// <param name="control">サイズ調整を行うコントロール</param>
            /// <returns></returns>
            public void Add(object control)
            {
                if (control == null) return;

                if (typeof(ShapeContainer).IsInstanceOfType(control))
                {
                    ShapeContainer lsp = (ShapeContainer)control;
                    foreach (Shape shp in lsp.Shapes)
                    {
                        if (!Controls.ContainsKey(shp))
                        {
                            ControlContainer container = new ControlContainer();

                            LineShape ls = shp as LineShape;            //直線
                            OvalShape os = shp as OvalShape;            //楕円
                            RectangleShape rs = shp as RectangleShape;  //四角

                            if (ls != null)
                            {
                                container.bounds = new Rectangle(ls.X1, ls.Y1, ls.X2 - ls.X1, ls.Y2 - ls.Y1);
                                Controls[shp] = container;
                            }
                            else if (os != null)
                            {
                                container.bounds = new Rectangle(os.Left, os.Top, os.Width, os.Height);
                                Controls[shp] = container;
                            }
                            else if (rs != null)  //四角
                            {
                                container.bounds = new Rectangle(rs.Left, rs.Top, rs.Width, rs.Height);
                                Controls[shp] = container;
                            }
                        }
                    }
                }
                else if (typeof(Control).IsInstanceOfType(control))
                {
                    Control ctl = (Control)control;

                    if (!Controls.ContainsKey(ctl))
                    {
                        bool add = true;
                        ControlContainer container = new ControlContainer();

                        if (ResizedWidth != -1 && ResizedHeight != -1)
                        {
                            Point orgPos = new Point((int)(ctl.Location.X / this.ScaleWidth(this.ResizedWidth)), (int)(ctl.Location.Y / this.ScaleHeight(this.ResizedHeight)));
                            Size orgSize = new Size((int)(ctl.Width / this.ScaleWidth(this.ResizedWidth)), (int)(ctl.Height / this.ScaleHeight(this.ResizedHeight)));
                            container.bounds = new Rectangle(orgPos, orgSize);
                        }
                        else
                        {
                            container.bounds = new Rectangle(ctl.Location, ctl.Size);
                        }

                        container.font = ctl.Font;
                        container.margin = ctl.Margin;
                        container.padding = ctl.Padding;

                        if (typeof(TabControl).IsInstanceOfType(ctl))
                        {
                            container.tabitemsize = ((TabControl)ctl).ItemSize;
                        }
                        else if (typeof(ListView).IsInstanceOfType(ctl))
                        {
                            foreach (ColumnHeader col in ((ListView)ctl).Columns)
                            {
                                container.columnsizes.Add(col.Width);
                            }
                        }
                        else if (typeof(DataGridView).IsInstanceOfType(ctl))
                        {
                            foreach (DataGridViewColumn col in ((DataGridView)ctl).Columns)
                            {
                                container.columnsizes.Add(col.Width);
                            }
                            container.columnheaderheight = ((DataGridView)ctl).ColumnHeadersHeight;
                            container.rowheaderwidth = ((DataGridView)ctl).RowHeadersWidth;
                            container.rowheight = ((DataGridView)ctl).RowTemplate.Height;
                        }
                        else if (typeof(SplitContainer).IsInstanceOfType(ctl))
                        {
                            container.splitwidth = ((SplitContainer)ctl).SplitterWidth;
                            container.splitdistance = ((SplitContainer)ctl).SplitterDistance;
                            if (ctl.Controls.Count == 0) add = false;
                        }

                        if (add)
                        {
                            Controls[ctl] = container;

                            if (ctl.GetType().Name == "BL_Chart")
                            {
                                //BL_Chartは子コントロールを制御しない

                            }
                            else
                            {
                                if (ctl.Controls.Count > 0) this.Add(ctl.Controls);
                            }
                        }
                    }
                }
            }

            /// <summary>
            /// サイズ調整をするコントロールリストから削除します
            /// </summary>
            /// <param name="controls">削除するコントロールコレクション</param>
            /// <returns></returns>
            public void Remove(Control.ControlCollection controls)
            {
                if (controls == null) return;
                foreach (Control ctl in controls) { this.Remove(ctl); }
            }

            /// <summary>
            /// サイズ調整をするコントロールリストから削除します
            /// </summary>
            /// <param name="control">削除するコントロール</param>
            /// <returns></returns>
            public void Remove(Control control)
            {
                if (control == null) return;

                if (control.Controls.Count > 0) this.Remove(control.Controls);

                if (typeof(ShapeContainer).IsInstanceOfType(control))
                {
                    ShapeContainer lsp = (ShapeContainer)control;
                    foreach (Shape shp in lsp.Shapes)
                    {
                        if (!Controls.ContainsKey(shp))
                        {
                            Controls.Remove(shp);
                        }
                    }
                }
                else if (typeof(Control).IsInstanceOfType(control))
                {
                    if (Controls.ContainsKey(control))
                    {
                        Controls.Remove(control);
                    }
                }
            }

            /// <summary>
            /// サイズ調整登録したコントロールをクリアします
            /// </summary>
            public void Clear()
            {
                Controls.Clear();
            }

            /// <summary>
            /// 横スケール率を取得します
            /// </summary>
            /// <param name="frmWidth"></param>
            /// <returns></returns>
            public double ScaleWidth(int frmWidth)
            {
                if (frmWidth == FrmWidth) return 1.0;
                return (double)frmWidth / (double)FrmWidth;
            }

            /// <summary>
            /// 縦スケール率を取得します
            /// </summary>
            /// <param name="frmHeight"></param>
            /// <returns></returns>
            public double ScaleHeight(int frmHeight)
            {
                if (frmHeight == FrmHeight) return 1.0;
                return (double)frmHeight / (double)FrmHeight;
            }

            /// <summary>
            /// コントロールのサイズ調整を行います
            /// </summary>
            /// <param name="parent"></param>
            public void Resize(Control parent)
            {
                if (typeof(BL_MainForm_Base).IsInstanceOfType(parent)) if (!parent.Visible) return;

                Resize(parent.ClientSize.Width, parent.ClientSize.Height);
            }

            private int ResizedWidth = -1;
            private int ResizedHeight = -1;

            /// <summary>
            /// コントロールのサイズ調整を行います
            /// </summary>
            /// <param name="frmWidth">変更後のフォーム幅</param>
            /// <param name="frmHeight">変更後のフォーム高さ</param>
            public void Resize(int frmWidth, int frmHeight)
            {
                if (0 == frmWidth || 0 == frmHeight) return;

                if (ResizedHeight < 0 || ResizedWidth < 0)
                {
                    if (Controls.Count == 0)
                    {
                        Add(parent.Controls);
                    }
                }

                if (ResizedWidth == frmWidth && ResizedHeight == frmHeight) return;
                ResizedWidth = frmWidth;
                ResizedHeight = frmHeight;

                double w_percent, h_percent;

                w_percent = (double)frmWidth / (double)FrmWidth;
                h_percent = (double)frmHeight / (double)FrmHeight;

                if (!horizontal) w_percent = 1.0;
                if (!virtical) h_percent = 1.0;

                lastScaleWidth = w_percent;
                lastScaleHeight = h_percent;

                if (w_percent <= 0 || h_percent <= 0) return;

                if (parent != null) BeginUpdate(parent);

                foreach (var v in Controls)
                {
                    Point pt = new Point(v.Value.bounds.Location.X, v.Value.bounds.Location.Y);
                    Size size = new Size(v.Value.bounds.Size.Width, v.Value.bounds.Size.Height);
                    Size itemsize = new Size(v.Value.tabitemsize.Width, v.Value.tabitemsize.Height);
                    Padding margin = new Padding(v.Value.margin.Left, v.Value.margin.Top, v.Value.margin.Right, v.Value.margin.Bottom);
                    Padding padding = new Padding(v.Value.padding.Left, v.Value.padding.Top, v.Value.padding.Right, v.Value.padding.Bottom);

                    pt.X = (int)((double)pt.X * w_percent);
                    size.Width = (int)((double)size.Width * w_percent);
                    itemsize.Width = (int)((double)(itemsize.Width - 2) * w_percent);
                    margin.Left = (int)((double)(margin.Left) * w_percent);
                    margin.Right = (int)((double)(margin.Right) * w_percent);
                    padding.Left = (int)((double)(padding.Left) * w_percent);
                    padding.Right = (int)((double)(padding.Right) * w_percent);

                    pt.Y = (int)((double)pt.Y * h_percent);
                    size.Height = (int)((double)size.Height * h_percent);
                    itemsize.Height = (int)((double)(itemsize.Height - 2) * h_percent);
                    margin.Top = (int)((double)(margin.Top) * h_percent);
                    margin.Bottom = (int)((double)(margin.Bottom) * h_percent);
                    padding.Top = (int)((double)(padding.Top) * h_percent);
                    padding.Bottom = (int)((double)(padding.Bottom) * h_percent);

                    if (typeof(Control).IsInstanceOfType(v.Key))
                    {
                        if (!typeof(BL_Chart).IsInstanceOfType(v.Key))
                        {
                            if (h_percent < w_percent)
                            {
                                float sz = v.Value.font.Size * (float)h_percent;
                                if (sz < 1) sz = 1f;
                                ((Control)v.Key).Font = new Font(v.Value.font.Name, sz);
                            }
                            else
                            {
                                ((Control)v.Key).Font = new Font(v.Value.font.Name, v.Value.font.Size * (float)w_percent);
                            }
                        }

                        ((Control)v.Key).Location = pt;
                        ((Control)v.Key).Size = size;
                        ((Control)v.Key).Margin = margin;
                        ((Control)v.Key).Padding = padding;

                        if (typeof(TabControl).IsInstanceOfType(v.Key))
                        {
                            if (0 < itemsize.Width && 0 < itemsize.Height)
                            {
                                ((TabControl)v.Key).ItemSize = itemsize;
                            }
                        }
                        else if (typeof(ListView).IsInstanceOfType(v.Key))
                        {
                            ListView lvw = (ListView)v.Key;
                            int i = 0;
                            foreach (ColumnHeader column in lvw.Columns)
                            {
                                if (i < v.Value.columnsizes.Count)
                                {
                                    column.Width = (int)((double)v.Value.columnsizes[i] * w_percent);
                                    i++;
                                }
                            }
                        }
                        else if (typeof(DataGridView).IsInstanceOfType(v.Key))
                        {
                            DataGridView dg = (DataGridView)v.Key;
                            dg.RowTemplate.Height = (int)((double)v.Value.rowheight * h_percent);

                            int i = 0;
                            foreach (DataGridViewColumn column in dg.Columns)
                            {
                                if (i < v.Value.columnsizes.Count)
                                {
                                    column.Width = (int)((double)v.Value.columnsizes[i] * w_percent);
                                    i++;
                                }
                            }
                            dg.ColumnHeadersHeight = (int)((double)v.Value.columnheaderheight * h_percent);
                            dg.RowHeadersWidth = (int)((double)v.Value.rowheaderwidth * w_percent);
                            for (int j = 0; j < dg.Rows.Count; j++)
                            {
                                dg.Rows[j].Height = (int)((double)v.Value.rowheight * h_percent);
                            }

                        }
                        else if (typeof(SplitContainer).IsInstanceOfType(v.Key))
                        {
                            //Control p = (SplitContainer)v.Key;
                            //while (p != null)
                            //{
                            //    if (p.GetType().Name == "BL_Chart")
                            //    {
                            //        break;
                            //    }
                            //    p = p.Parent;
                            //}
                            //if (p != null) continue;


                            if (((SplitContainer)v.Key).Orientation == Orientation.Horizontal)
                            {
                                //if (!((SplitContainer)v.Key).IsSplitterFixed)
                                {
                                    int width = (int)((double)v.Value.splitwidth * h_percent);
                                    if (width < 1) width = 1;
                                    ((SplitContainer)v.Key).SplitterWidth = width;
                                    int distance = (int)((double)v.Value.splitdistance * h_percent);
                                    if (distance < 1) distance = 1;
                                    ((SplitContainer)v.Key).SplitterDistance = distance;
                                }
                            }
                            else
                            {
                                //if (!((SplitContainer)v.Key).IsSplitterFixed)
                                {
                                    int width = (int)((double)v.Value.splitwidth * w_percent);
                                    if (width < 1) width = 1;
                                    ((SplitContainer)v.Key).SplitterWidth = width;
                                    int distance = (int)((double)v.Value.splitdistance * w_percent);
                                    if (distance < 1) distance = 1;
                                    ((SplitContainer)v.Key).SplitterDistance = distance;
                                }
                            }
                        }
                        else if (typeof(Panel).IsInstanceOfType(v.Key))
                        {
                            ((Panel)v.Key).AutoScrollPosition = new Point(0, 0);
                        }

                    }
                    else if (typeof(Shape).IsInstanceOfType(v.Key))
                    {
                        LineShape ls = v.Key as LineShape;            //直線
                        OvalShape os = v.Key as OvalShape;            //楕円
                        RectangleShape rs = v.Key as RectangleShape;  //四角

                        if (ls != null)
                        {
                            ls.X1 = pt.X;
                            ls.X2 = pt.X + size.Width;
                            ls.Y1 = pt.Y;
                            ls.Y2 = pt.Y + size.Height;
                        }
                        else if (os != null)
                        {
                            os.Location = pt;
                            os.Size = size;
                        }
                        else if (rs != null)
                        {
                            rs.Location = pt;
                            rs.Size = size;
                        }
                    }
                }

                if (parent != null) EndUpdate(parent);
            }

            /// <summary>
            /// コントロールの基準値を取得します
            /// </summary>
            /// <param name="ctr"></param>
            /// <returns></returns>
            public ControlContainer Get(Control ctr)
            {
                if (Controls.ContainsKey(ctr))
                {
                    return Controls[ctr];
                }
                return null;
            }

            #endregion

            #region プロパティ

            /// <summary>
            /// フォーム幅基準値を設定または取得します
            /// </summary>
            public int BaseWidth
            {
                get
                {
                    return FrmWidth;
                }
            }

            /// <summary>
            /// フォーム高さ基準値を設定または取得します
            /// </summary>
            public int BaseHeight
            {
                get
                {
                    return FrmHeight;
                }
            }

            /// <summary>
            /// 制御対象のコントロール数を取得します
            /// </summary>
            public int Count { get { return Controls.Count; } }



            #endregion
        }

        /// <summary>
        /// 自動スケーリングオブジェクト
        /// </summary>
        public ViewResize ViewResizer = null;

        /// <summary>
        /// メインフォームの自動スケーリング機能を開始します
        /// </summary>
        public void Resizer_Initialize()
        {
            Resizer_Initialize(true, true);
        }

        /// <summary>
        /// メインフォームの自動スケーリング機能を開始します
        /// </summary>
        public void Resizer_Initialize(bool horizontal, bool virtical)
        {
            if (ViewResizer == null)
            {
                ViewResizer = new ViewResize(this, horizontal, virtical);
            }
        }

        /// <summary>
        /// メインフォームの自動スケーリング機能を終了します
        /// スケーリング状態は現在のままで終了されます
        /// </summary>
        public void Resizer_Finalize()
        {
            ViewResizer = null;
        }

        /// <summary></summary>
        public double Resizer_LastScaleWidth { get { return ViewResizer.lastScaleWidth; } }

        /// <summary></summary>
        public double Resizer_LastScaleHeight { get { return ViewResizer.lastScaleHeight; } }

        #endregion

        #region 共通ログ生成機能

        /// <summary>
        /// 
        /// </summary>
        public class FormLogger : BL_ThreadController_Base
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="filename"></param>
            public FormLogger(string filename)
                : base(filename)
            {
                m_bSuspend = true;
            }
        }

        private FormLogger logger;

        /// <summary>
        /// ログを追加します
        /// </summary>
        /// <param name="description"></param>
        public void Log(string description)
        {
            if (logger == null) return;
            logger.Log(description, 0);
        }

        /// <summary>
        /// ログを追加します
        /// </summary>
        /// <param name="description"></param>
        /// <param name="level"></param>
        public void Log(string description, int level)
        {
            if (logger == null) return;
            logger.Log(description, level);
        }

        #endregion

        #region IME制御

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <returns></returns>
        [DllImport("Imm32.dll")]
#pragma warning disable CA1401 // P/Invokes should not be visible
        public static extern IntPtr ImmGetContext(IntPtr hWnd);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hIMC"></param>
        /// <returns></returns>
        [DllImport("Imm32.dll")]
        public static extern bool ImmGetOpenStatus(IntPtr hIMC);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="hIMC"></param>
        /// <returns></returns>
        [DllImport("Imm32.dll")]
        public static extern bool ImmReleaseContext(IntPtr hWnd, IntPtr hIMC);
#pragma warning restore CA1401 // P/Invokes should not be visible

        private bool m_ImeFunctionEnable = true;

        /// <summary>
        /// IMEのON/OFF状態によってファンクション表示を変化させるか否かを取得または設定します
        /// </summary>
        public bool ImeFunctionEnable { get { return m_ImeFunctionEnable; } set { m_ImeFunctionEnable = value; } }

        #endregion

        #region デバッグモード

        private bool m_bDebugMode = false;

        /// <summary>
        /// デバッグモード有効
        /// </summary>
        public bool IsDebugMode { get { return m_bDebugMode; } /*set { m_bDebugMode = value; }*/ }

        /// <summary>
        /// デバッグモード開始
        /// </summary>
        public void StartDebugMode()
        {
            Log(this.Text + " - デバッグモード開始");

            m_bDebugMode = true;
            m_DebugStep = 0;

            if (labelColor == null) labelColor = labelTitle.BackColor;
            labelTitle.BackColor = Color.Red;

            MainForm_Base_OnDebugMode(this, m_bDebugMode);

            if (0 < panelSubForm.Controls.Count)
            {
                if (((BL_SubForm_Base)panelSubForm.Controls[0]).EnableSeparation)
                {
                    flatButtonSeparate.Visible = true;
                }
            }
        }

        /// <summary>
        /// デバッグモード終了
        /// </summary>
        public void EndDebugMode()
        {
            Log(this.Text + " - デバッグモード終了");

            m_bDebugMode = false;
            m_DebugStep = 0;

            if (labelColor != null)
            {
                labelTitle.BackColor = (Color)labelColor;
                labelColor = null;
            }

            flatButtonSeparate.Visible = false;

            foreach (BL_SubForm_Base sub in separatedForms)
            {
                if (!sub.IsDisposed)
                {
                    sub.Left = 0;
                    sub.Top = 0;

                    sub.TopLevel = false;
                    sub.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    sub.ShowInTaskbar = false;
                    sub.MinimizeBox = false;
                    sub.MaximizeBox = false;
                    sub.ControlBox = false;
                    sub.Visible = false;
                    sub.Parent = this;
                    //sub.m_Mainform = this;
                    sub.Show();
                    sub.SetWindowBounds(panelSubForm.ClientRectangle);
                    panelSubForm.Controls.Add(sub);

                    if (sub.m_Functions != null) sub.m_Functions.Close();
                }
            }

            separatedForms.Clear();

            MainForm_Base_OnDebugMode(this, m_bDebugMode);
        }

        #endregion

        #region ファンクションキー操作

        /// <summary>
        /// ファンクションキー文字列を保持します。
        /// </summary>
        public BL_FlatButton[] btnFunctions = new BL_FlatButton[13];
        private BL_TouchControl[] func_touch = new BL_TouchControl[13];

        ///// <summary></summary>
        //public Color FunctionBackColor = SystemColors.Control;
        ///// <summary></summary>
        //public Color FunctionForeColor = SystemColors.ControlText;
        ///// <summary></summary>
        //public Color OnColor = SystemColors.ActiveCaption;
        ///// <summary></summary>
        //public Color OffColor = SystemColors.InactiveCaption;

        /// <summary></summary>
        private bool functionControl = true;
        /// <summary></summary>
        public bool FunctionControl { get { return functionControl; } set { functionControl = value; } }

        #endregion

        #region メインフォーム制御

        /// <summary>
        /// 表示中メッセージボックス類インスタンス(参照)
        /// </summary>
        public List<BL_MessageBox_Base> m_MessageBox = new List<BL_MessageBox_Base>();

        //private bool canexit = false;

        /// <summary>
        /// 終了(ウィンドウ右上の×ボタン)を有効するか否かを保持します
        /// </summary>
        public bool isExitableWindow = false;

        /// <summary>
        /// 終了(ウィンドウ右上の×ボタン)を有効するか否かを取得および設定します
        /// </summary>
        public bool IsExitableWindow { get { return isExitableWindow; } set { isExitableWindow = value; } }

        /// <summary></summary>
        public bool HideAtBoottime { get; set; }

        #endregion

        #region ジェスチャー関連

        private bool EnableGesture = false;
        private BL_TouchControl touch = new BL_TouchControl();

        private Point beginLocation = new Point(0, 0);
        private Size baseSize;
        private Point startCenter;
        private double baseDelta;

        void touch_GestureZoom(object sender, BL_TouchControl.WMGestureEventArgs e)
        {
            double sizeDelta = (baseDelta + Math.Sqrt(Math.Pow((double)(e.LocationX - beginLocation.X), 2) + Math.Pow((double)(e.LocationY - beginLocation.Y), 2))) / baseDelta;

            // 新しいサイズにする
            Size size = new Size((int)(baseSize.Width * sizeDelta), (int)(baseSize.Height * sizeDelta));
            touch.control.Size = size;

            // 新しい中心点を得る
            touch.control.Location = new Point(startCenter.X - (touch.control.Size.Width / 2), startCenter.Y - (touch.control.Size.Height / 2));
            //baseDelta = Math.Sqrt(Math.Pow((double)panelSubForm.Width, 2) + Math.Pow((double)panelSubForm.Height, 2));
        }

        void touch_GesturePan(object sender, BL_TouchControl.WMGestureEventArgs e)
        {

        }

        void touch_GestureEnd(object sender, BL_TouchControl.WMGestureEventArgs e)
        {

        }

        void touch_GestureBegin(object sender, BL_TouchControl.WMGestureEventArgs e)
        {
            // ジェスチャ開始位置を記録する
            beginLocation = new Point(e.LocationX, e.LocationY);

            baseSize = touch.control.Size;
            startCenter = new Point(touch.control.Location.X + (baseSize.Width / 2), touch.control.Location.Y + (baseSize.Height / 2));
            baseDelta = Math.Sqrt(Math.Pow((double)baseSize.Width, 2) + Math.Pow((double)baseSize.Height, 2));
        }

        #endregion

        #region コンストラクタ

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_MainForm_Base() : this(false) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_MainForm_Base(bool enable_gesture)
        {
            EnableGesture = enable_gesture;
            DoubleBuffered = true;
            isExitableWindow = true;

            InitializeComponent();

            if (DesignMode) return;

            if (this.StartPosition == FormStartPosition.CenterScreen)
            {
                Location = new Point(Screen.PrimaryScreen.Bounds.X + Screen.PrimaryScreen.Bounds.Width / 2 - this.Width / 2, Screen.PrimaryScreen.Bounds.Y + Screen.PrimaryScreen.Bounds.Height / 2 - this.Height / 2);
            }

            btnFunctions[0] = buttonFunction00;
            btnFunctions[1] = buttonFunction01;
            btnFunctions[2] = buttonFunction02;
            btnFunctions[3] = buttonFunction03;
            btnFunctions[4] = buttonFunction04;
            btnFunctions[5] = buttonFunction05;
            btnFunctions[6] = buttonFunction06;
            btnFunctions[7] = buttonFunction07;
            btnFunctions[8] = buttonFunction08;
            btnFunctions[9] = buttonFunction09;
            btnFunctions[10] = buttonFunction10;
            btnFunctions[11] = buttonFunction11;
            btnFunctions[12] = buttonFunction12;

            for (int i = 0; i < btnFunctions.Length; i++)
            {
                btnFunctions[i].Click += new EventHandler(MainForm_ButtonClicked);
                btnFunctions[i].MouseDown += BL_MainForm_Base_MouseDown;
                btnFunctions[i].MouseUp += BL_MainForm_Base_MouseUp;

                if (EnableGesture)
                {
                    func_touch[i] = new BL_TouchControl();
                    func_touch[i].Touchdown += BL_MainForm_Base_Touchdown;
                    func_touch[i].Touchup += BL_MainForm_Base_Touchup;
                    func_touch[i].StartControl(btnFunctions[i], false);
                }
            }

            if (EnableGesture)
            {
                touch.GestureBegin += touch_GestureBegin;
                touch.GestureEnd += touch_GestureEnd;
                touch.GesturePan += touch_GesturePan;
                touch.GestureZoom += touch_GestureZoom;
                touch.StartControl(panelSubForm, true);
            }
        }

        private void BL_MainForm_Base_Touchup(BL_TouchControl sender, BL_TouchControl.WMTouchEventArgs e)
        {
            if (!this.IsDisposed)
            {
                if (0 < panelSubForm.Controls.Count)
                {
                    BL_SubForm_Base sub = (BL_SubForm_Base)panelSubForm.Controls[0];
                    if (sub != null) sub.btnFunctions_MouseUp(sender.control, new EventArgs());
                }
            }
        }

        private void BL_MainForm_Base_Touchdown(BL_TouchControl sender, BL_TouchControl.WMTouchEventArgs e)
        {
            if (!this.IsDisposed)
            {
                if (0 < panelSubForm.Controls.Count)
                {
                    BL_SubForm_Base sub = (BL_SubForm_Base)panelSubForm.Controls[0];
                    if (sub != null) sub.btnFunctions_MouseDown(sender.control, new EventArgs());
                }
            }
        }

        private void BL_MainForm_Base_MouseUp(object sender, MouseEventArgs e)
        {
            if (!this.IsDisposed)
            {
                if (0 < panelSubForm.Controls.Count)
                {
                    BL_SubForm_Base sub = (BL_SubForm_Base)panelSubForm.Controls[0];
                    if (sub != null) sub.btnFunctions_MouseUp(sender, e);
                }
            }
        }

        private void BL_MainForm_Base_MouseDown(object sender, MouseEventArgs e)
        {
            if (!this.IsDisposed)
            {
                if (0 < panelSubForm.Controls.Count)
                {
                    BL_SubForm_Base sub = (BL_SubForm_Base)panelSubForm.Controls[0];
                    if (sub != null) sub.btnFunctions_MouseDown(sender, e);
                }
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_MainForm_Base(Bitmap wallpaper) : this(wallpaper, false) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_MainForm_Base(Bitmap wallpaper, bool enable_gesture)
            : this(enable_gesture)
        {
            if (DesignMode) return;

            if (wallpaper != null) this.panelSubForm.BackgroundImage = wallpaper;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_MainForm_Base(Icon icon) : this(icon, false) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_MainForm_Base(Icon icon, bool enable_gesture)
            : this(enable_gesture)
        {
            if (DesignMode) return;

            isExitableWindow = false;

            if (icon != null)
            {
                notifyIconMinimum.Icon = icon;
                notifyIconMinimum.Visible = true;
            }

            flatButtonExit.Enabled = false;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_MainForm_Base(Bitmap wallpaper, Icon icon) : this(wallpaper, icon, false) { }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_MainForm_Base(Bitmap wallpaper, Icon icon, bool enable_gesture)
            : this(enable_gesture)
        {
            if (DesignMode) return;

            isExitableWindow = false;

            if (wallpaper != null) this.panelSubForm.BackgroundImage = wallpaper;
            if (icon != null)
            {
                notifyIconMinimum.Icon = icon;
                notifyIconMinimum.Visible = true;
            }

            flatButtonExit.Enabled = false;
        }

        #endregion

        private bool isCompareTitleEnable = false;

        /// <summary>タイトル文字列でのサブフォーム識別を有効にするかどうかを取得または設定します。</summary>
        public bool IsCompareTitleEnable { get { return isCompareTitleEnable; } set { isCompareTitleEnable = value; } }

        private bool logEnable = true;
        /// <summary>ログを有効かどうかを取得または設定します</summary>
        public bool LogEnable { get { return logEnable; } set { logEnable = value; } }

        private string _captiontext = "";
        
        /// <summary>
        /// 
        /// </summary>
        [Category("表示")]
        [Description("タスクバーに表示するタイトルを設定します")]
        [DefaultValue("")]
        public string CaptionText { get { return _captiontext; } set { _captiontext = value; } }

        /// <summary>
        /// アプリケーション終了操作を無効にする
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;

                if (!DesignMode)
                {
                    if (!isExitableWindow)
                    {
                        cp.ClassStyle = cp.ClassStyle | 0x200;
                    }

                    if (CaptionText != "")
                    {
                        new SecurityPermission(SecurityPermissionFlag.UnmanagedCode).Demand();

                        cp.Style &= ~0xC00000; //WS_CAPTION;
                        cp.Caption = CaptionText;
                    }
                }
                return cp;
            }
        }

        /// <summary>
        /// フォームロード時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            if (LogEnable)
            {
                logger = new FormLogger("MainForm - " + Path.GetFileNameWithoutExtension(Application.ExecutablePath));
                logger.StartControl(300);
                Log(this.Text + " - ログ収集開始", 1);
            }

            if (this.StartPosition == FormStartPosition.CenterScreen ||
                this.StartPosition == FormStartPosition.CenterParent)
            {
                Rectangle rect = Screen.GetBounds(this);
                this.Left = rect.Width / 2 - this.Width / 2;
                this.Top = rect.Height / 2 - this.Height / 2;
            }

            if (notifyIconMinimum.Icon != null)
            {
                if (notifyIconMinimum.BalloonTipText == "") notifyIconMinimum.BalloonTipText = "no message";
                notifyIconMinimum.Visible = true;

                if (HideAtBoottime)
                {
                    this.WindowState = FormWindowState.Minimized;
                    Visible = true;
                }

                if (this.WindowState == FormWindowState.Minimized)
                {
                    ShowInTaskbar = false;
                }
            }


            //frmReportManager.m_dicFonts = frmReportManager.LoadFontsFromDirectory();

            //継承クラスでオーバーロードして、フォームの初期化処理を記述します。
            //

            if (ViewResizer != null) ViewResizer.Resize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_KeyUp(object sender, KeyEventArgs e)
        {
            if (DesignMode) return;

            e.Handled = true;

            do
            {
                if (0 < m_MessageBox.Count) break;

                if (0 < panelSubForm.Controls.Count)
                {
                    if (typeof(BL_SubForm_Base).IsInstanceOfType(panelSubForm.Controls[0]))
                    {
                        BL_SubForm_Base sub = (BL_SubForm_Base)panelSubForm.Controls[0];

                        if (ImeFunctionEnable)
                        {
                            if (sub.ActiveControl != null)
                            {
                                var hIMC = ImmGetContext(sub.ActiveControl.Handle);
                                var status = ImmGetOpenStatus(hIMC);
                                ImmReleaseContext(sub.ActiveControl.Handle, hIMC);
                                if (status) break;
                            }
                        }

                        if (!sub.key_input_accept) return;
                        sub.key_input_accept = false;

                        int function_no = -1;

                        switch (e.KeyCode)
                        {
                            case Keys.Escape:
                                function_no = 0;
                                break;
                            case Keys.F1:
                                function_no = 1;
                                break;
                            case Keys.F2:
                                function_no = 2;
                                break;
                            case Keys.F3:
                                function_no = 3;
                                break;
                            case Keys.F4:
                                function_no = 4;
                                break;
                            case Keys.F5:
                                function_no = 5;
                                break;
                            case Keys.F6:
                                function_no = 6;
                                break;
                            case Keys.F7:
                                function_no = 7;
                                break;
                            case Keys.F8:
                                function_no = 8;
                                break;
                            case Keys.F9:
                                function_no = 9;
                                break;
                            case Keys.F10:
                                function_no = 10;
                                break;
                            case Keys.F11:
                                function_no = 11;
                                break;
                            case Keys.F12:
                                function_no = 12;
                                break;

                            case Keys.Enter:
                                if (IsDebugMode)
                                {
                                    if (e.Modifiers == Keys.Alt)
                                    {
                                        if (this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.None)
                                        {
                                            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                                        }
                                        else if (this.FormBorderStyle == System.Windows.Forms.FormBorderStyle.Sizable)
                                        {
                                            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                                        }
                                    }
                                }
                                break;
                        }

                        if (0 <= function_no && function_no <= 12)
                        {
                            if (((BL_SubForm_Base)panelSubForm.Controls[0]).dictFunctions.ContainsKey(function_no))
                            {
                                if (typeof(Button).IsInstanceOfType(((BL_SubForm_Base)panelSubForm.Controls[0]).dictFunctions[function_no]))
                                {
                                    Button button = (Button)((BL_SubForm_Base)panelSubForm.Controls[0]).dictFunctions[function_no];
                                    if (button.Tag != null && typeof(BL_FlatButton).IsInstanceOfType(button))
                                    {
                                        if (typeof(bool).IsInstanceOfType(button.Tag))
                                        {
                                            if (((BL_FlatButton)button).CheckMode) ((BL_FlatButton)button).Checked = !((BL_FlatButton)button).Checked;
                                        }
                                    }
                                }
                            }

                            if (btnFunctions[function_no].Enabled)
                            {
                                if (!clicked)
                                {
                                    if (btnFunctions[function_no].CheckMode) btnFunctions[function_no].Checked = !btnFunctions[function_no].Checked;
                                }

                                sub.btnFunctions_Click(btnFunctions[function_no], e);
                            }

                            e.Handled = true;
                        }

                        sub.key_input_accept = true;
                    }
                }
            }
            while (false);

            //Application.DoEvents();

            if (ActiveSubForm != null) ActiveSubForm.SubForm_Base_KeyUp(sender, e);

        }

        bool clicked = false;
        private void MainForm_ButtonClicked(object sender, EventArgs e)
        {
            if (DesignMode) return;

            if (typeof(Button).IsInstanceOfType(sender))
            {
                if (0 < m_MessageBox.Count) return;

                Button btn = (Button)sender;
                int num;
                if (int.TryParse(btn.Name.Substring(btn.Name.Length - 2), out num))
                {
                    clicked = true;

                    switch (num)
                    {
                        case 0:
                            MainForm_KeyUp(sender, new KeyEventArgs(Keys.Escape));
                            break;
                        case 1:
                            MainForm_KeyUp(sender, new KeyEventArgs(Keys.F1));
                            break;
                        case 2:
                            MainForm_KeyUp(sender, new KeyEventArgs(Keys.F2));
                            break;
                        case 3:
                            MainForm_KeyUp(sender, new KeyEventArgs(Keys.F3));
                            break;
                        case 4:
                            MainForm_KeyUp(sender, new KeyEventArgs(Keys.F4));
                            break;
                        case 5:
                            MainForm_KeyUp(sender, new KeyEventArgs(Keys.F5));
                            break;
                        case 6:
                            MainForm_KeyUp(sender, new KeyEventArgs(Keys.F6));
                            break;
                        case 7:
                            MainForm_KeyUp(sender, new KeyEventArgs(Keys.F7));
                            break;
                        case 8:
                            MainForm_KeyUp(sender, new KeyEventArgs(Keys.F8));
                            break;
                        case 9:
                            MainForm_KeyUp(sender, new KeyEventArgs(Keys.F9));
                            break;
                        case 10:
                            MainForm_KeyUp(sender, new KeyEventArgs(Keys.F10));
                            break;
                        case 11:
                            MainForm_KeyUp(sender, new KeyEventArgs(Keys.F11));
                            break;
                        case 12:
                            MainForm_KeyUp(sender, new KeyEventArgs(Keys.F12));
                            break;
                    }

                    clicked = false;
                }
            }
        }

        private BL_SubForm_Base firstSubForm = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void panelSubForm_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (0 < panelSubForm.Controls.Count)
            {
                BL_SubForm_Base sub = (BL_SubForm_Base)panelSubForm.Controls[0];

                labelTitle.Text = sub._TitleString;
                if (this.ControlBox) this.Text = sub._TitleString;

                //    if (functionControl)
                //    {
                //        string[] func = sub.FunctionStrings();

                //        for (int i = 0; i < func.Length; i++)
                //        {
                //            btnFunctions[i].Tag = null;
                //            btnFunctions[i].Text = func[i].Replace(":", "\n");
                //        //    if (btnFunctions[i].Text.Trim().Length == 0)
                //        //    {
                //        //        btnFunctions[i].Enabled = false;
                //        //        //btnFunctions[i].BackColor = Color.FromArgb(64, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //        //        btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                //        //    }
                //        //    else
                //        //    {
                //        //        btnFunctions[i].Enabled = true;
                //        //        //btnFunctions[i].BackColor = Color.FromArgb(255, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //        //        btnFunctions[i].BackColor = btnFunctions[i].BackColorNormal;
                //        //        if (notifyIconMinimum.Visible && notifyIconMinimum.Icon != null)
                //        //        {
                //        //            if (!canexit && 0 <= btnFunctions[i].Text.IndexOf("終了"))
                //        //            {
                //        //                btnFunctions[i].Enabled = false;
                //        //                //btnFunctions[i].BackColor = Color.FromArgb(64, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //        //                btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                //        //            }
                //        //        }
                //        //    }
                //        }
                //    }

                flatButtonSeparate.Visible = sub.EnableSeparation & IsDebugMode;

                sub.BringToFront();
                sub.Focus();

                sub.SubForm_Base_Activated(this, e);

                //    timerFunctionEnabler_Tick(sender, e);
            }
            //else
            //{
            //    for (int i = 0; i < btnFunctions.Length; i++)
            //    {
            //        btnFunctions[i].Tag = null;
            //        btnFunctions[i].Text = "";
            //        btnFunctions[i].Enabled = false;
            //        btnFunctions[i].BackColor = SystemColors.AppWorkspace;
            //        labelTitle.Text = "";
            //    }

            //    ////this.Close();
            //    ////throw new Exception("表示する子画面が無くなりました。");

            //    //BL_MessageBox sb = new BL_MessageBox(this);
            //    //DialogResult mrc = sb.ShowMessage("表示する子画面が無くなりました。\n終了しますか？", "確認", BL_MessageBox.cYesNo);
            //    //if (mrc == DialogResult.Yes)
            //    //{
            //    //    Close();
            //    //    //Application.Exit();
            //    //}
            //    //else
            //    //{
            //    //    firstSubForm.ShowMe(this);
            //    //}

            //    ////Application.Exit();
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void panelSubForm_ControlAdded(object sender, ControlEventArgs e)
        {
            //if (e.Control.Name.Substring(0, 3) == "frm")
            {
                BL_SubForm_Base sub = (BL_SubForm_Base)e.Control;

                if (firstSubForm == null) firstSubForm = sub;

                labelTitle.Text = sub._TitleString;
                if (this.ControlBox) this.Text = sub._TitleString;

                //if (functionControl)
                //{
                //    string[] func = sub.FunctionStrings();

                //    for (int i = 0; i < func.Length; i++)
                //    {
                //        btnFunctions[i].Tag = null;
                //        btnFunctions[i].Text = func[i].Replace(":", "\n");
                //    //    if (btnFunctions[i].Text.Trim().Length == 0)
                //    //    {
                //    //        btnFunctions[i].Enabled = false;
                //    //        btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                //    //    }
                //    //    else
                //    //    {
                //    //        btnFunctions[i].Enabled = true;
                //    //        btnFunctions[i].BackColor = btnFunctions[i].BackColorNormal;

                //    //        if (notifyIconMinimum.Visible && notifyIconMinimum.Icon != null)
                //    //        {
                //    //            if (!canexit && 0 <= btnFunctions[i].Text.IndexOf("終了"))
                //    //            {
                //    //                btnFunctions[i].Enabled = false;
                //    //                //btnFunctions[i].BackColor = Color.FromArgb(64, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //    //                btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                //    //            }
                //    //        }
                //    //    }
                //    }
                //}

                ////for (int i = 0; i < btnFunctions.Length; i++)
                ////{
                ////    btnFunctions[i].Click += new EventHandler(sub.btnFunctions_Click);
                ////    btnFunctions[i].Enabled = true;
                ////}

                flatButtonSeparate.Visible = sub.EnableSeparation & IsDebugMode;

                sub.BringToFront();
                sub.Activate();

                timerFunctionEnabler_Tick(sender, e);
            }
        }

        /// <summary>
        /// 他のサブフォームを閉じて、指定のフォームの単独表示を行います。
        /// </summary>
        /// <param name="sender">アクションを起こすフォーム</param>
        /// <param name="onlyform">単独表示を行うフォーム</param>
        public virtual BL_SubForm_Base CloseOtherForms(BL_SubForm_Base sender, BL_SubForm_Base onlyform)
        {
            BL_SubForm_Base fmenu = null;
            for (int i = 0; i < panelSubForm.Controls.Count; i++)
            {
                BL_SubForm_Base f = (BL_SubForm_Base)panelSubForm.Controls[i];
                if (f.Name != onlyform.Name)
                {
                    if (f != sender)
                    {
                        f.Close();
                    }
                }
                else
                {
                    fmenu = f;
                }
            }

            if (fmenu != null)
            {
                fmenu.BringToFront();
                if (onlyform != null)
                {
                    onlyform.Dispose();
                    onlyform = null;
                }
                onlyform = fmenu;
                fmenu.SubForm_Base_Activated(this, new EventArgs());
            }
            else
            {
                onlyform.ShowMe(this);
            }

            if (sender != null)
            {
                sender.Close();
            }

            return onlyform;
        }

        /// <summary>
        /// フォームクローズ前の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (DesignMode) return;

            //継承クラスでオーバーロードして、終了を検証するコードを記述します。
            //if (終了できない)
            //{
            //    e.Cancel = true;
            //}

            string canexit = "";
            for (int i = 0; i < panelSubForm.Controls.Count; i++)
            {
                BL_SubForm_Base f = (BL_SubForm_Base)panelSubForm.Controls[i];
                canexit = f.CanExit;
                if (canexit != "") break;
            }

            if (canexit != "")
            {
                if (logger != null) logger.Log(this.Text + " - " + canexit);

                if (0 <= canexit.IndexOf("?") || 0 <= canexit.IndexOf("？"))
                {
                    if (BL_MessageBox.Show(this, canexit, "終了確認", MessageBoxButtons.YesNo) == System.Windows.Forms.DialogResult.Yes)
                    {
                        canexit = "";
                    }
                    else
                    {
                        e.Cancel = true;
                    }
                }
                else
                {
                    e.Cancel = true;
                    BL_MessageBox.Show(this, canexit, "終了不可");
                }
            }

            if (canexit == "")
            {
                foreach (var v in func_touch)
                {
                    if (v != null) v.StopControl();
                }

                for (int i = 0; i < panelSubForm.Controls.Count; i++)
                {
                    BL_SubForm_Base f = (BL_SubForm_Base)panelSubForm.Controls[i];
                    f.Close();
                }

                if (logger != null)
                {
                    logger.Log(this.Text + " - ログ収集停止");
                    logger.StopControl();
                    logger = null;
                }
            }
        }

        #region PeekMessage/TranslateMessage/DispatchMessage

        // Win32 APIのインポート
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PeekMessage(
          ref MSG lpMsg,
          Int32 hwnd,
          Int32 wMsgFilterMin,
          Int32 wMsgFilterMax,
          PeekMsgOption wRemoveMsg);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool TranslateMessage(ref MSG lpMsg);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern Int32 DispatchMessage(ref MSG lpMsg);

        // メッセージの処理方法オプション
        private enum PeekMsgOption
        {
            PM_NOREMOVE = 0,  // 処理後、メッセージをキューから削除しない
            PM_REMOVE      // 処理後、メッセージをキューから削除する　
        }

        // メッセージ構造体
        [StructLayout(LayoutKind.Sequential)]
        struct MSG
        {
            public Int32 HWnd;    // ウィンドウ・ハンドル
            public Int32 Msg;    // メッセージID
            public Int32 WParam;  // WParamフィールド（メッセージIDごとに違う）
            public Int32 LParam;  // LParamフィールド（メッセージIDごとに違う）
            public Int32 Time;    // 時間
            public POINTAPI Pt;    // カーソル位置（スクリーン座標）
        }
        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public Int32 x;      // x座標
            public Int32 y;      // y座標
        }
        const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;

        #endregion

        /// <summary>デバッグ処理検出変数</summary>
        protected int m_DebugStep = 0;
        private object labelColor = null;

        /// <summary>
        /// キー押し下げ時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (DesignMode) return;

            if (!m_bDebugMode)
            {
                switch (m_DebugStep)
                {
                    default: m_DebugStep = 0; break;
                    case 0: if (e.KeyChar == 'D') m_DebugStep++; else m_DebugStep = 0; break;
                    case 1: if (e.KeyChar == 'E') m_DebugStep++; else m_DebugStep = 0; break;
                    case 2: if (e.KeyChar == 'B') m_DebugStep++; else m_DebugStep = 0; break;
                    case 3: if (e.KeyChar == 'U') m_DebugStep++; else m_DebugStep = 0; break;
                    case 4: if (e.KeyChar == 'G') m_DebugStep++; else m_DebugStep = 0; break;
                }
            }
            else
            {
                switch (m_DebugStep)
                {
                    default: m_DebugStep = 0; break;
                    case 0: if (e.KeyChar == 'D') m_DebugStep++; else m_DebugStep = 0; break;
                    case 1: if (e.KeyChar == 'E') m_DebugStep++; else m_DebugStep = 0; break;
                    case 2: if (e.KeyChar == 'B') m_DebugStep++; else m_DebugStep = 0; break;
                    case 3: if (e.KeyChar == 'U') m_DebugStep++; else m_DebugStep = 0; break;
                    case 4: if (e.KeyChar == 'G') m_DebugStep++; else m_DebugStep = 0; break;
                }
            }

            //継承クラスでオーバーロードして、下記のように記述します。
            if (m_DebugStep == 5)
            {
                if (!m_bDebugMode)
                {
                    StartDebugMode();
                }
                else
                {
                    EndDebugMode();
                }
            }

            if (ActiveSubForm != null) ActiveSubForm.SubForm_Base_KeyPress(sender, e);
        }

        private void MainForm_Base_KeyDown(object sender, KeyEventArgs e)
        {
            if (ActiveSubForm != null) ActiveSubForm.SubForm_Base_KeyDown(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="isDebugMode"></param>
        protected virtual void MainForm_Base_OnDebugMode(object sender, bool isDebugMode)
        {
        }

        /// <summary>
        /// 100ms定周期処理を行います。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_timer100ms_Tick(object sender, EventArgs e)
        {
            if (DesignMode) return;

            //継承クラスでオーバーロードして、100ms定周期処理を記述します。
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void panelSubForm_Resize(object sender, EventArgs e)
        {
            if (DesignMode) return;

            if (ActiveSubForm != null)
            {
                ActiveSubForm.SetWindowBounds(new Rectangle(0, 0, panelSubForm.Width, panelSubForm.Height));
            }

            //for (int i = 0; i < panelSubForm.Controls.Count; i++)
            //{
            //    if (typeof(BL_SubForm_Base).IsInstanceOfType(panelSubForm.Controls[i]))
            //    {
            //        BL_SubForm_Base f = (BL_SubForm_Base)panelSubForm.Controls[i];
            //        f.SetWindowBounds(new Rectangle(0, 0, panelSubForm.Width, panelSubForm.Height));
            //    }
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subform"></param>
        /// <returns></returns>
        public BL_SubForm_Base CheckSubForm(BL_SubForm_Base subform)
        {
            BL_SubForm_Base sub = null;
            for (int i = 0; i < panelSubForm.Controls.Count; i++)
            {
                BL_SubForm_Base ctr = (BL_SubForm_Base)panelSubForm.Controls[i];
                if (ctr == subform || (ctr.TitleString() == subform.TitleString() && isCompareTitleEnable))
                {
                    sub = ctr;
                    break;
                }
            }

            if (sub == null)
            {
                sub = subform;
            }

            return sub;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subform"></param>
        /// <param name="activated"></param>
        public BL_SubForm_Base AddSubForm(BL_SubForm_Base subform, bool activated)
        {
            BL_SubForm_Base sub = null;
            for (int i = 0; i < panelSubForm.Controls.Count; i++)
            {
                BL_SubForm_Base ctr = (BL_SubForm_Base)panelSubForm.Controls[i];
                if (ctr == subform || (ctr.TitleString() == subform.TitleString() && isCompareTitleEnable))
                {
                    sub = ctr;
                    break;
                }
            }

            if (sub == null)
            {
                if (0 < panelSubForm.Controls.Count && panelSubForm.Controls[0] != subform)
                {
                    if (activated) ((BL_SubForm_Base)panelSubForm.Controls[0]).SubForm_Base_Deactivate(this, new EventArgs());
                }

                panelSubForm.Controls.Add(subform);

                if (activated) subform.SubForm_Base_Activated(this, new EventArgs());

                sub = subform;
            }
            else
            {
                if (activated) ActivateSubform(sub);
            }

            return sub;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subform"></param>
        public void RemoveSubForm(BL_SubForm_Base subform)
        {
            int count = panelSubForm.Controls.Count;
            panelSubForm.Controls.Remove(subform);

            separatedForms.Remove(subform);

            bool ife = ImeFunctionEnable;
            ImeFunctionEnable = true;
            timerFunctionEnabler_Tick(null, null);
            ImeFunctionEnable = ife;

            //foreach (var v in btnFunctions)
            //{
            //    v.Enabled = true;
            //    v.BackColorNormal = SystemColors.Control;
            //    v.ForeColorNormal = SystemColors.ControlText;
            //    v.BackColor = v.BackColorNormal;
            //    v.ForeColor = v.ForeColorNormal;
            //    v.Checked = false;
            //    v.CheckMode = false;
            //}

            if (count != panelSubForm.Controls.Count)
            {
                if (0 < panelSubForm.Controls.Count && panelSubForm.Controls[0] != subform)
                {
                    BL_SubForm_Base sub = panelSubForm.Controls[0] as BL_SubForm_Base;
                    if (sub != null)
                    {
                        sub.SubForm_Base_Activated(this, new EventArgs());

                        //panelSubForm_ControlAdded(this, new ControlEventArgs(sub));
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="title"></param>
        public void RemoveSubForm(string title)
        {
            int count = panelSubForm.Controls.Count;

            for (int i = 0; i < panelSubForm.Controls.Count; i++)
            {
                if (((BL_SubForm_Base)panelSubForm.Controls[i]).TitleString() == title)
                {
                    panelSubForm.Controls.RemoveAt(i);
                    i--;
                }
            }

            if (count != panelSubForm.Controls.Count)
            {
                if (0 < panelSubForm.Controls.Count)
                {
                    ((BL_SubForm_Base)panelSubForm.Controls[0]).SubForm_Base_Activated(this, new EventArgs());
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public int SubForms_Count { get { return panelSubForm.Controls.Count; } }

        /// <summary>
        /// 
        /// </summary>
        public void TopSubForm_Focus()
        {
            if (0 < panelSubForm.Controls.Count)
            {
                //try
                {
                    panelSubForm.Controls[0].Focus();
                    panelSubForm.Controls[0].BringToFront();
                    //((SubForm_Base)panelSubForm.Controls[0]).SubForm_Base_Activated(this, new EventArgs());
                }
                //catch { }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Size SubForm_Size { get { return panelSubForm.Size; } }

        //delegate Size GetSize_Delegate();
        ///// <summary>
        ///// 
        ///// </summary>
        //public Size GetSize()
        //{
        //    if (InvokeRequired)
        //    {
        //        // 別スレッドから呼び出された場合
        //        Size s = (Size)Invoke(new GetSize_Delegate(GetSize));
        //        return s;
        //    }

        //    return Size;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_Resize(object sender, EventArgs e)
        {
            if (DesignMode) return;

            if (ModifierKeys == Keys.LButton) return;

            if (notifyIconMinimum.Visible && notifyIconMinimum.Icon != null)
            {
                if (WindowState == FormWindowState.Minimized)
                {
                    ShowInTaskbar = false;
                }
                else
                {
                    ShowInTaskbar = true;
                }
            }

            if (ViewResizer != null)
            {
                ViewResizer.Resize(this);
            }
        }

        #region 通知アイコン関連処理

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_notifyIconMinimum_MouseDoubleClick(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                if (0 < panelSubForm.Controls.Count) ((BL_SubForm_Base)panelSubForm.Controls[0]).SubForm_Base_Activated(this, new EventArgs());
            }
            this.Activate();

            flatButtonExit.Enabled = false;

            //canexit = false;

            if (functionControl)
            {
                //for (int i = 0; i < btnFunctions.Length; i++)
                //{
                //    if (btnFunctions[i].Text.Trim().Length == 0)
                //    {
                //        btnFunctions[i].Enabled = false;
                //        btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                //    }
                //    else
                //    {
                //        btnFunctions[i].Enabled = true;
                //        btnFunctions[i].BackColor = btnFunctions[i].BackColorNormal;
                //        if (!canexit && 0 <= btnFunctions[i].Text.IndexOf("終了"))
                //        {
                //            btnFunctions[i].Enabled = false;
                //            //btnFunctions[i].BackColor = Color.FromArgb(64, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //            btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                //        }
                //    }
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_notifyIconMinimum_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (notifyIconMinimum.Visible && notifyIconMinimum.Icon != null && notifyIconMinimum.BalloonTipText != "")
                {
                    notifyIconMinimum.ShowBalloonTip(3000);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_notifyIconMinimum_BalloonTipClicked(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
            {
                WindowState = FormWindowState.Normal;
                if (0 < panelSubForm.Controls.Count) ((BL_SubForm_Base)panelSubForm.Controls[0]).SubForm_Base_Activated(this, new EventArgs());
            }
            this.Activate();

            flatButtonExit.Enabled = false;

            //canexit = false;

            if (functionControl)
            {
                //for (int i = 0; i < btnFunctions.Length; i++)
                //{
                //    if (btnFunctions[i].Text.Trim().Length == 0)
                //    {
                //        btnFunctions[i].Enabled = false;
                //        //btnFunctions[i].BackColor = Color.FromArgb(64, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //        btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                //    }
                //    else
                //    {
                //        btnFunctions[i].Enabled = true;
                //        //btnFunctions[i].BackColor = Color.FromArgb(255, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //        btnFunctions[i].BackColor = btnFunctions[i].BackColorNormal;
                //        if (!canexit && 0 <= btnFunctions[i].Text.IndexOf("終了"))
                //        {
                //            btnFunctions[i].Enabled = false;
                //            //btnFunctions[i].BackColor = Color.FromArgb(64, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //            btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                //        }
                //    }
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_notifyIconMinimum_BalloonTipClosed(object sender, EventArgs e)
        {
            if (notifyIconMinimum.Visible && notifyIconMinimum.Icon != null)
            {
                flatButtonExit.Enabled = false;

                ////canexit = false;

                //if (functionControl)
                //{
                //    //for (int i = 0; i < btnFunctions.Length; i++)
                //    //{
                //    //    if (btnFunctions[i].Text.Trim().Length == 0)
                //    //    {
                //    //        btnFunctions[i].Enabled = false;
                //    //        //btnFunctions[i].BackColor = Color.FromArgb(64, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //    //        btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                //    //    }
                //    //    else
                //    //    {
                //    //        btnFunctions[i].Enabled = true;
                //    //        //btnFunctions[i].BackColor = Color.FromArgb(255, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //    //        btnFunctions[i].BackColor = btnFunctions[i].BackColorNormal;
                //    //        if (!canexit && 0 <= btnFunctions[i].Text.IndexOf("終了"))
                //    //        {
                //    //            btnFunctions[i].Enabled = false;
                //    //            //btnFunctions[i].BackColor = Color.FromArgb(64, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //    //            btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                //    //        }
                //    //    }
                //    //}
                //}
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_notifyIconMinimum_BalloonTipShown(object sender, EventArgs e)
        {
            if (notifyIconMinimum.Visible && notifyIconMinimum.Icon != null)
            {
                flatButtonExit.Enabled = true;

                //canexit = true;

                //if (functionControl)
                //{
                //    //for (int i = 0; i < btnFunctions.Length; i++)
                //    //{
                //    //    if (btnFunctions[i].Text.Trim().Length == 0)
                //    //    {
                //    //        btnFunctions[i].Enabled = false;
                //    //        //btnFunctions[i].BackColor = Color.FromArgb(64, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //    //        btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                //    //    }
                //    //    else
                //    //    {
                //    //        btnFunctions[i].Enabled = true;
                //    //        //btnFunctions[i].BackColor = Color.FromArgb(255, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //    //        btnFunctions[i].BackColor = btnFunctions[i].BackColorNormal;
                //    //        if (!canexit && 0 <= btnFunctions[i].Text.IndexOf("終了"))
                //    //        {
                //    //            btnFunctions[i].Enabled = false;
                //    //            //btnFunctions[i].BackColor = Color.FromArgb(64, btnFunctions[i].BackColor.R, btnFunctions[i].BackColor.G, btnFunctions[i].BackColor.B);
                //    //            btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                //    //        }
                //    //    }
                //    //}
                //}
            }
        }
        #endregion

        private class FuncSave
        {
            public string text = "";
            public Color back = Color.Transparent;
            public Color fore = Color.Transparent;

            public FuncSave() { }
            public FuncSave(string text, Color back, Color fore)
            {
                this.text = text;
                this.back = back;
                this.fore = fore;
            }
        }
        private List<FuncSave> func_save = new List<FuncSave>();

        private void timerFunctionEnabler_Tick(object sender, EventArgs e)
        {
            if (DesignMode) return;

            timerFunctionEnabler.Enabled = false;

            if (ImeFunctionEnable && functionControl)
            {
                if (0 < panelSubForm.Controls.Count)
                {
                    if (((BL_SubForm_Base)panelSubForm.Controls[0]).ActiveControl != null)
                    {
                        var hIMC = ImmGetContext(((BL_SubForm_Base)panelSubForm.Controls[0]).ActiveControl.Handle);
                        var status = ImmGetOpenStatus(hIMC);
                        ImmReleaseContext(((BL_SubForm_Base)panelSubForm.Controls[0]).ActiveControl.Handle, hIMC);

                        #region IMEの制御によるファンクション切り替え

                        if (status)
                        {
                            if (func_save.Count == 0)
                            {
                                foreach (var v in btnFunctions)
                                {
                                    func_save.Add(new FuncSave(v.Text, v.BackColor, v.ForeColor));
                                }

                                btnFunctions[0].Text = "取消";
                                btnFunctions[1].Text = "";
                                btnFunctions[2].Text = "";
                                btnFunctions[3].Text = "";
                                btnFunctions[4].Text = "";
                                btnFunctions[5].Text = "";
                                btnFunctions[6].Text = "かな変換";
                                btnFunctions[7].Text = "カナ変換";
                                btnFunctions[8].Text = "半角変換";
                                btnFunctions[9].Text = "全角英数";
                                btnFunctions[10].Text = "半角英数";
                                btnFunctions[11].Text = "";
                                btnFunctions[12].Text = "";

                                for (int i = 0; i < btnFunctions.Length; i++)
                                {
                                    btnFunctions[i].BackColor = Color.Red;
                                    if (btnFunctions[i].Text == "")
                                    {
                                        btnFunctions[i].Enabled = false;
                                    }
                                    else
                                    {
                                        btnFunctions[i].Enabled = true;
                                    }
                                }
                            }

                            timerFunctionEnabler.Enabled = true;
                            return;
                        }
                        else
                        {
                            if (0 < func_save.Count)
                            {
                                for (int i = 0; i < btnFunctions.Length; i++)
                                {
                                    btnFunctions[i].Text = func_save[i].text;
                                    btnFunctions[i].BackColor = func_save[i].back;
                                    btnFunctions[i].ForeColor = func_save[i].fore;

                                    //btnFunctions[i].BackColor = (Color)FunctionBackColor;
                                    //btnFunctions[i].ForeColor = (Color)FunctionForeColor;

                                    if (btnFunctions[i].Text == "")
                                    {
                                        btnFunctions[i].Enabled = false;
                                    }
                                    else
                                    {
                                        btnFunctions[i].Enabled = true;
                                    }
                                }
                                func_save.Clear();
                            }
                        }

                        #endregion
                    }
                }
            }

            if (functionControl)
            {
                for (int i = 0; i < btnFunctions.Length; i++)
                {
                    BL_FlatButton mbutton = btnFunctions[i];
                    BL_FlatButton fbutton = null;
                    Button button = null;
                    string text = "";

                    if (0 == panelSubForm.Controls.Count)
                    {
                        mbutton.Text = "";
                        mbutton.BackColor = mbutton.BackColorOFF;
                        mbutton.ForeColor = mbutton.ForeColorOFF;
                        mbutton.Enabled = false;
                    }
                    else
                    {
                        text = ((BL_SubForm_Base)panelSubForm.Controls[0]).FunctionStrings()[i];
                        mbutton.Text = text.Replace("]:", "]\n");

                        if (text.Trim() == "")
                        {
                            mbutton.BackColor = mbutton.BackColorOFF;
                            mbutton.ForeColor = mbutton.ForeColorOFF;
                            mbutton.Enabled = false;
                        }
                        else
                        {
                            mbutton.Enabled = true;

                            if (((BL_SubForm_Base)panelSubForm.Controls[0]).dictFunctions.ContainsKey(i))
                            {
                                if (typeof(BL_FlatButton).IsInstanceOfType(((BL_SubForm_Base)panelSubForm.Controls[0]).dictFunctions[i]))
                                {
                                    fbutton = (BL_FlatButton)((BL_SubForm_Base)panelSubForm.Controls[0]).dictFunctions[i];
                                }
                                else if (typeof(Button).IsInstanceOfType(((BL_SubForm_Base)panelSubForm.Controls[0]).dictFunctions[i]))
                                {
                                    button = (Button)((BL_SubForm_Base)panelSubForm.Controls[0]).dictFunctions[i];
                                }
                            }

                            if (fbutton != null)
                            {
                                mbutton.CheckMode = fbutton.CheckMode;
                                mbutton.Checked = fbutton.Checked;
                                if (fbutton.CheckMode && fbutton.Checked)
                                {
                                    mbutton.BackColor = fbutton.BackColorON;
                                    mbutton.ForeColor = fbutton.ForeColorON;
                                }
                                else if (fbutton.CheckMode && !fbutton.Checked)
                                {
                                    mbutton.BackColor = fbutton.BackColorOFF;
                                    mbutton.ForeColor = fbutton.ForeColorOFF;
                                }
                                else
                                {
                                    mbutton.BackColor = fbutton.BackColorNormal;
                                    mbutton.ForeColor = fbutton.ForeColorNormal;
                                }

                                mbutton.Enabled = fbutton.Enabled;
                            }
                            else if (button != null)
                            {
                                mbutton.Enabled = button.Enabled;
                                if (button.Enabled)
                                {
                                    mbutton.BackColor = mbutton.BackColorNormal;
                                    mbutton.ForeColor = mbutton.ForeColorNormal;
                                }
                                else
                                {
                                    mbutton.BackColor = mbutton.BackColorOFF;
                                    mbutton.ForeColor = mbutton.ForeColorOFF;
                                }
                            }
                            else
                            {
                                mbutton.CheckMode = false;
                                mbutton.Checked = false;
                                mbutton.BackColor = mbutton.BackColorNormal;
                                mbutton.ForeColor = mbutton.ForeColorNormal;
                            }
                        }
                    }
                }

                //timerFunctionEnabler.Enabled = true;
            }
        }

        /// <summary>
        /// サブフォームが最前面かどうか調査する
        /// </summary>
        /// <param name="subform"></param>
        /// <returns></returns>
        public bool IsTopLevelSubForm(BL_SubForm_Base subform)
        {
            if (0 < m_MessageBox.Count) return false;
            if (0 < panelSubForm.Controls.Count)
            {
                if (panelSubForm.Controls[0] == subform) return true;
            }
            return false;
        }

        delegate void CloseAllMessageBox_Delegate();
        /// <summary>
        /// 全てのメッセージボックスを閉じます
        /// </summary>
        public void CloseAllMessageBox()
        {
            if (InvokeRequired)
            {
                Invoke(new CloseAllMessageBox_Delegate(CloseAllMessageBox));
                return;
            }

            while (0 < m_MessageBox.Count)
            {
                m_MessageBox[0].m_Result = System.Windows.Forms.DialogResult.Ignore;
                m_MessageBox[0].Close();
            }

            m_MessageBox.Clear();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_timer1Second_Tick(object sender, EventArgs e)
        {
            if (DesignMode) return;

            labelTimenow.Text = DateTime.Now.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        public BL_SubForm_Base ActiveSubForm
        {
            get
            {
                if (0 < panelSubForm.Controls.Count) return (BL_SubForm_Base)panelSubForm.Controls[0];
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public enum EdgeLocation
        {
            /// <summary></summary>
            TOP,
            /// <summary></summary>
            BOTTOM,
            /// <summary></summary>
            LEFT,
            /// <summary></summary>
            RIGHT
        }

        /// <summary>
        /// タスクバーの位置を取得します
        /// </summary>
        /// <returns></returns>
        public EdgeLocation GetTaskBarLocation()
        {
            EdgeLocation taskBarLocation = EdgeLocation.BOTTOM;
            bool taskBarOnTopOrBottom = (Screen.PrimaryScreen.WorkingArea.Width == Screen.PrimaryScreen.Bounds.Width);
            if (taskBarOnTopOrBottom)
            {
                if (Screen.PrimaryScreen.WorkingArea.Top > 0) taskBarLocation = EdgeLocation.TOP;
            }
            else
            {
                if (Screen.PrimaryScreen.WorkingArea.Left > 0)
                {
                    taskBarLocation = EdgeLocation.LEFT;
                }
                else
                {
                    taskBarLocation = EdgeLocation.RIGHT;
                }
            }
            return taskBarLocation;
        }

        /// <summary>
        /// タスクバーのサイズを取得します
        /// </summary>
        /// <param name="ctl"></param>
        /// <returns></returns>
        public List<Rectangle> FindDockedTaskBars(Control ctl)
        {
            int LeftDockedWidth = 0;
            int TopDockedHeight = 0;
            int RightDockedWidth = 0;
            int BottomDockedHeight = 0;
            List<Rectangle> DockedRects = new List<Rectangle>();

            if (ctl != null)
            {
                LeftDockedWidth = Math.Abs(Math.Abs(Screen.FromControl(ctl).Bounds.Left) - Math.Abs(Screen.FromControl(ctl).WorkingArea.Left));
                TopDockedHeight = Math.Abs(Math.Abs(Screen.FromControl(ctl).Bounds.Top) - Math.Abs(Screen.FromControl(ctl).WorkingArea.Top));
                RightDockedWidth = (Screen.FromControl(ctl).Bounds.Width - LeftDockedWidth) - Screen.FromControl(ctl).WorkingArea.Width;
                BottomDockedHeight = (Screen.FromControl(ctl).Bounds.Height - TopDockedHeight) - Screen.FromControl(ctl).WorkingArea.Height;

                if (!Screen.FromControl(ctl).Bounds.Equals(Screen.FromControl(ctl).WorkingArea))
                {

                    if (LeftDockedWidth > 0)
                    {
                        Rectangle rect = new Rectangle();
                        rect.X = Screen.FromControl(ctl).Bounds.Left;
                        rect.Y = Screen.FromControl(ctl).Bounds.Top;
                        rect.Width = LeftDockedWidth;
                        rect.Height = Screen.FromControl(ctl).Bounds.Height;
                        DockedRects.Add(rect);
                    }
                    if (RightDockedWidth > 0)
                    {
                        Rectangle rect = new Rectangle();
                        rect.X = Screen.FromControl(ctl).WorkingArea.Right;
                        rect.Y = Screen.FromControl(ctl).Bounds.Top;
                        rect.Width = RightDockedWidth;
                        rect.Height = Screen.FromControl(ctl).Bounds.Height;
                        DockedRects.Add(rect);
                    }
                    if (TopDockedHeight > 0)
                    {
                        Rectangle rect = new Rectangle();
                        rect.X = Screen.FromControl(ctl).WorkingArea.Left;
                        rect.Y = Screen.FromControl(ctl).Bounds.Top;
                        rect.Width = Screen.FromControl(ctl).WorkingArea.Width;
                        rect.Height = TopDockedHeight;
                        DockedRects.Add(rect);
                    }
                    if (BottomDockedHeight > 0)
                    {
                        Rectangle rect = new Rectangle();
                        rect.X = Screen.FromControl(ctl).WorkingArea.Left;
                        rect.Y = Screen.FromControl(ctl).WorkingArea.Bottom;
                        rect.Width = Screen.FromControl(ctl).WorkingArea.Width;
                        rect.Height = BottomDockedHeight;
                        DockedRects.Add(rect);
                    }
                }
            }
            else
            {
                foreach (Screen TmpScrn in Screen.AllScreens)
                {
                    LeftDockedWidth = Math.Abs(Math.Abs(TmpScrn.Bounds.Left) - Math.Abs(TmpScrn.WorkingArea.Left));
                    TopDockedHeight = Math.Abs(Math.Abs(TmpScrn.Bounds.Top) - Math.Abs(TmpScrn.WorkingArea.Top));
                    RightDockedWidth = (TmpScrn.Bounds.Width - LeftDockedWidth) - TmpScrn.WorkingArea.Width;
                    BottomDockedHeight = (TmpScrn.Bounds.Height - TopDockedHeight) - TmpScrn.WorkingArea.Height;

                    if (!TmpScrn.Bounds.Equals(TmpScrn.WorkingArea))
                    {

                        if (LeftDockedWidth > 0)
                        {
                            Rectangle rect = new Rectangle();
                            rect.X = TmpScrn.Bounds.Left;
                            rect.Y = TmpScrn.Bounds.Top;
                            rect.Width = LeftDockedWidth;
                            rect.Height = TmpScrn.Bounds.Height;
                            DockedRects.Add(rect);
                        }
                        if (RightDockedWidth > 0)
                        {
                            Rectangle rect = new Rectangle();
                            rect.X = TmpScrn.WorkingArea.Right;
                            rect.Y = TmpScrn.Bounds.Top;
                            rect.Width = RightDockedWidth;
                            rect.Height = TmpScrn.Bounds.Height;
                            DockedRects.Add(rect);
                        }
                        if (TopDockedHeight > 0)
                        {
                            Rectangle rect = new Rectangle();
                            rect.X = TmpScrn.WorkingArea.Left;
                            rect.Y = TmpScrn.Bounds.Top;
                            rect.Width = TmpScrn.WorkingArea.Width;
                            rect.Height = TopDockedHeight;
                            DockedRects.Add(rect);
                        }
                        if (BottomDockedHeight > 0)
                        {
                            Rectangle rect = new Rectangle();
                            rect.X = TmpScrn.WorkingArea.Left;
                            rect.Y = TmpScrn.WorkingArea.Bottom;
                            rect.Width = TmpScrn.WorkingArea.Width;
                            rect.Height = BottomDockedHeight;
                            DockedRects.Add(rect);
                        }
                    }
                }
            }

            return DockedRects;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="edge"></param>
        public void SetEdgeLocation(EdgeLocation edge)
        {
            int X = 0;
            int Y = 0;

            List<Rectangle> docked = FindDockedTaskBars(this);
            if (0 < docked.Count)
            {
                foreach (var rect in docked)
                {
                    if (rect.Height == Screen.GetWorkingArea(this).Height) if (rect.X == 0) X += rect.Width;
                    if (rect.Width == Screen.GetWorkingArea(this).Width) if (rect.Y == 0) Y += rect.Height;
                }
            }

            int left = 0;
            int top = 0;
            switch (edge)
            {
                case EdgeLocation.TOP:
                    left = Screen.GetWorkingArea(this).Width / 2 - this.Width / 2 + X;
                    top = Screen.GetWorkingArea(this).Top + Y;
                    break;
                case EdgeLocation.BOTTOM:
                    left = Screen.GetWorkingArea(this).Width / 2 - this.Width / 2 + X;
                    top = Screen.GetWorkingArea(this).Height - this.Height;
                    break;
                case EdgeLocation.LEFT:
                    left = Screen.GetWorkingArea(this).Left;
                    top = Screen.GetWorkingArea(this).Height / 2 - this.Height / 2 + Y;
                    break;
                case EdgeLocation.RIGHT:
                    left = X + Screen.GetWorkingArea(this).Width - this.Width;
                    top = Screen.GetWorkingArea(this).Height / 2 - this.Height / 2 + Y;
                    break;
            }

            this.SetBounds(left, top, Width, Height);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subform"></param>
        public virtual void ActivateSubform(BL_SubForm_Base subform)
        {
            if (panelSubForm.Controls.Contains(subform))
            {
                BL_SubForm_Base pre = (BL_SubForm_Base)panelSubForm.Controls[0];

                pre.SubForm_Base_Deactivate(this, new EventArgs());

                if (firstSubForm == null)
                {
                    firstSubForm = subform;
                }

                labelTitle.Text = subform._TitleString;
                if (this.ControlBox) this.Text = subform._TitleString;

                if (functionControl)
                {
                    string[] func = subform.FunctionStrings();

                    for (int i = 0; i < func.Length; i++)
                    {
                        btnFunctions[i].Tag = null;
                        btnFunctions[i].Text = func[i].Replace(":", "\n");
                    }
                }

                subform.BringToFront();
                subform.SubForm_Base_Activated(this, new EventArgs());

                timerFunctionEnabler_Tick(null, null);
            }
        }

        /// <summary>
        /// メインフォームがアクティブになった時にアクティブなサブフォームもアクティブを通知します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_Activated(object sender, EventArgs e)
        {
            BL_SubForm_Base active = ActiveSubForm;
            if (active != null)
            {
                active.SubForm_Base_MainformActivated(this, new EventArgs());
            }
        }

        /// <summary>
        /// メインフォームが非アクティブになった時にアクティブなサブフォームも非アクティブを通知します。
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_Deactivate(object sender, EventArgs e)
        {
            BL_SubForm_Base active = ActiveSubForm;
            if (active != null)
            {
                active.SubForm_Base_MainformDeactivate(this, new EventArgs());
            }
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        /// <summary>
        /// ウィンドウの位置とサイズを変更する
        /// </summary>
        /// <param name="rect">変更後のウィンドウの位置とサイズ</param>
        public void SetWindowBounds(Rectangle rect)
        {
            if (this.Location.X != rect.Location.X || this.Location.Y != rect.Location.Y || this.Size.Width != rect.Size.Width || this.Size.Height != rect.Size.Height)
            {
                //MaximumSizeを大きくしておく
                if (this.MaximumSize.Width < rect.Width) this.MaximumSize = new Size(rect.Width, this.MaximumSize.Height);
                if (this.MaximumSize.Height < rect.Height) this.MaximumSize = new Size(this.MaximumSize.Width, rect.Height);

                MoveWindow(this.Handle, rect.X, rect.Y, rect.Width, rect.Height, true);
                this.UpdateBounds();
            }
        }

        #region 再描画制御

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
#pragma warning disable CA1401 // P/Invokes should not be visible
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
#pragma warning restore CA1401 // P/Invokes should not be visible
        /// <summary>
        /// 
        /// </summary>
        public const int WM_SETREDRAW = 0x000B;

        /// <summary>
        /// コントロール(子コントロールも含む)の描画を停止します。
        /// </summary>
        /// <param name="control">対象コントロール</param>
        public static void BeginUpdate(Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, 0, 0);
        }

        /// <summary>
        /// コントロール(子コントロールも含む)の描画を開始します。
        /// </summary>
        /// <param name="control">対象コントロール</param>
        public static void EndUpdate(Control control)
        {
            SendMessage(control.Handle, WM_SETREDRAW, 1, 0);
            control.Refresh();
        }

        #endregion

        #region アプリケーション終了

        /// <summary>
        /// アプリケーションを終了します。
        /// </summary>
        virtual public void ExitApplication()
        {
            ExitApplication("");
        }

        /// <summary>
        /// アプリケーションを終了します。
        /// </summary>
        virtual public void ExitApplication(string message)
        {
            string canexit = "";

            foreach (var v in panelSubForm.Controls)
            {
                if (typeof(BL_SubForm_Base).IsInstanceOfType(v))
                {
                    string s = ((BL_SubForm_Base)v).CanExit;
                    if (s != "")
                    {
                        canexit = s;
                        break;
                    }
                }
            }

            if (message == "" && (0 <= canexit.IndexOf("?") || 0 <= canexit.IndexOf("？")))
            {
                message = canexit;
                canexit = "";
            }

            if (canexit == "")
            {
                if (message != "")
                {
                    BL_MessageBox sb = new BL_MessageBox(this);
                    DialogResult mrc = sb.ShowMessage(message, "確認", BL_MessageBox.cYesNo);
                    if (mrc == DialogResult.Yes)
                    {
                        BeginUpdate(panelSubForm);
                        Close();
                    }
                }
                else
                {
                    BeginUpdate(panelSubForm);
                    Close();
                }
            }
            else
            {
                BL_MessageBox sb = new BL_MessageBox(this);
                DialogResult mrc = sb.ShowMessage(canexit, "確認", null);
            }
        }

        /// <summary>
        /// アプリケーションを終了します。
        /// パスワード確認機能付き
        /// </summary>
        /// <param name="message"></param>
        /// <param name="password"></param>
        virtual public void ExitApplication(string message, string password)
        {
            string canexit = "";
            foreach (var v in panelSubForm.Controls)
            {
                if (typeof(BL_SubForm_Base).IsInstanceOfType(v))
                {
                    string s = ((BL_SubForm_Base)v).CanExit;
                    if (s != "")
                    {
                        canexit = s;
                        break;
                    }
                }
            }

            if (message == "" && (0 <= canexit.IndexOf("?") || 0 <= canexit.IndexOf("？")))
            {
                message = canexit;
                canexit = "";
            }

            if (canexit == "")
            {
                if (message != "")
                {
                    bool ret = ShowCheckPassword(message, password);
                    if (ret)
                    {
                        BeginUpdate(this);
                        Close();
                    }
                }
                else
                {
                    BeginUpdate(this);
                    Close();
                }
            }
            else
            {
                BL_MessageBox sb = new BL_MessageBox(this);
                DialogResult mrc = sb.ShowMessage(canexit, "確認", null);
            }
        }

        /// <summary>
        /// パスワードチェック
        /// </summary>
        virtual public bool ShowCheckPassword(string message, string password)
        {
            BL_InputBox sb = new BL_InputBox(this);
            DialogResult mrc = sb.ShowCheckPassword(message, "確認", BL_MessageBox.cYesNo, password);
            if (mrc == DialogResult.Yes)
            {
                return true;
            }
            else if (mrc == DialogResult.Abort)
            {
                BL_MessageBox sb2 = new BL_MessageBox(this);
                sb2.ShowMessage("パスワードが違います。", "確認", null);
            }

            return false;
        }

        #endregion

        /// <summary>
        /// サブフォームのタイトル変更
        /// </summary>
        public virtual string Title
        {
            get { return labelTitle.Text; }
            set
            {
                labelTitle.Text = value;
                if (this.ControlBox) this.Text = value;
            }
        }

        private bool movableWindow = false;

        /// <summary>
        /// タイトル表示部分のドラッグでウィンドウ移動できるかどうかを取得または設定します。
        /// </summary>
        public bool IsMovableWindow { get { return movableWindow; } set { movableWindow = value; } }

        private int xxx = 0;
        private int yyy = 0;
        private bool mouseCapture = false;
        private void labelTitle_MouseDown(object sender, MouseEventArgs e)
        {
            if (movableWindow && !mouseCapture && e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                xxx = e.X;
                yyy = e.Y;
                mouseCapture = true;
            }
        }

        private void labelTitle_MouseMove(object sender, MouseEventArgs e)
        {
            if (movableWindow && mouseCapture)
            {
                this.Left += e.X - xxx;
                this.Top += e.Y - yyy;
            }
        }

        private void labelTitle_MouseUp(object sender, MouseEventArgs e)
        {
            if (movableWindow && mouseCapture)
            {
                this.Left += e.X - xxx;
                this.Top += e.Y - yyy;

                mouseCapture = false;
            }
        }

        /// <summary>
        /// フォーム上の最小化ボタン処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_flatButtonMinimum_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        /// <summary>
        /// フォーム上の終了ボタン処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_Exit_Click(object sender, EventArgs e)
        {
            ExitApplication();
        }

        /// <summary></summary>
        public List<BL_SubForm_Base> separatedForms = new List<BL_SubForm_Base>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sub"></param>
        public void SeparateSubForm(BL_SubForm_Base sub)
        {
            if (!sub.EnableSeparation) return;
            if (sub.m_Functions != null) return;

            sub.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            sub.IsExitableWindow = true;
            sub.MaximumSize = new Size(0, 0);
            sub.ShowInTaskbar = true;
            sub.MinimizeBox = true;
            sub.MaximizeBox = true;
            sub.ControlBox = true;
            sub.Visible = false;
            sub.Owner = null;
            sub.Parent = null;
            //sub.m_Mainform = null;
            sub.TopLevel = true;
            sub.Show();

            sub.Left = sub.m_Mainform.Left;
            sub.Top = sub.m_Mainform.Top;
            //sub.Width = sub.m_Mainform.Width;
            //sub.Height = sub.m_Mainform.Height;

            BL_SeparatedFunctions func = new BL_SeparatedFunctions(sub);
            func.ShowInTaskbar = false;
            func.Show(sub);

            separatedForms.Add(sub);

            flatButtonSeparate.Visible = false;

            if (0 < panelSubForm.Controls.Count)
            {
                if (typeof(BL_SubForm_Base).IsInstanceOfType(panelSubForm.Controls[0]))
                {
                    sub = (BL_SubForm_Base)panelSubForm.Controls[0];
                    if (sub.EnableSeparation)
                    {
                        flatButtonSeparate.Visible = true;
                    }
                }
            }
        }

        /// <summary>
        /// サブフォーム切り離しボタン処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void flatButtonSeparate_Click(object sender, EventArgs e)
        {
            if (panelSubForm.Controls.Count == 0) return;

            if (typeof(BL_SubForm_Base).IsInstanceOfType(panelSubForm.Controls[0]))
            {
                BL_SubForm_Base sub = (BL_SubForm_Base)panelSubForm.Controls[0];
                SeparateSubForm(sub);
            }
        }

        /// <summary>
        /// タイトルバーをダブルクリックした時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MainForm_Base_Title_DoubleClick(object sender, EventArgs e)
        {
            if (this.IsMovableWindow && this.FormBorderStyle != System.Windows.Forms.FormBorderStyle.None)
            {
                if (this.WindowState != FormWindowState.Maximized)
                {
                    this.WindowState = FormWindowState.Maximized;
                }
                else
                {
                    this.WindowState = FormWindowState.Normal;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionno"></param>
        /// <param name="enabled"></param>
        public void Function_Enabled(int functionno, bool enabled)
        {
            if (functionno < 0 || 12 < functionno) return;
            btnFunctions[functionno].Enabled = enabled;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="name_suffix"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Panel DuplucatePanel(Panel source, string name_suffix, int x, int y)
        {
            Panel dest = new Panel();

            dest.Left = x;
            dest.Top = y;
            dest.Size = source.Size;
            dest.Font = source.Font;
            dest.BorderStyle = source.BorderStyle;
            dest.Name = source.Name + name_suffix;
            dest.Dock = source.Dock;
            dest.Anchor = source.Anchor;

            foreach (Control v in source.Controls)
            {
                do
                {
                    {
                        Label control_source = v as Label;
                        if (control_source != null)
                        {
                            Label control_dest = new Label();
                            control_dest.Size = control_source.Size;
                            control_dest.Top = control_source.Top;
                            control_dest.Left = control_source.Left;
                            control_dest.Font = control_source.Font;
                            control_dest.BorderStyle = control_source.BorderStyle;
                            control_dest.TextAlign = control_source.TextAlign;
                            control_dest.Name = control_source.Name + name_suffix;
                            control_dest.Anchor = control_source.Anchor;
                            control_dest.BackColor = control_source.BackColor;
                            control_dest.ForeColor = control_source.ForeColor;
                            control_dest.FlatStyle = control_source.FlatStyle;
                            control_dest.Visible = control_source.Visible;
                            control_dest.Dock = control_source.Dock;
                            dest.Controls.Add(control_dest);
                            break;
                        }
                    }

                    {
                        TextBox control_source = v as TextBox;
                        if (control_source != null)
                        {
                            TextBox control_dest = new TextBox();
                            control_dest.Size = control_source.Size;
                            control_dest.Top = control_source.Top;
                            control_dest.Left = control_source.Left;
                            control_dest.Font = control_source.Font;
                            control_dest.BorderStyle = control_source.BorderStyle;
                            control_dest.TextAlign = control_source.TextAlign;
                            control_dest.Name = control_source.Name + name_suffix;
                            control_dest.Anchor = control_source.Anchor;
                            control_dest.BackColor = control_source.BackColor;
                            control_dest.ForeColor = control_source.ForeColor;
                            control_dest.HideSelection = control_source.HideSelection;
                            control_dest.MaxLength = control_source.MaxLength;
                            control_dest.Multiline = control_source.Multiline;
                            control_dest.ReadOnly = control_source.ReadOnly;
                            control_dest.WordWrap = control_source.WordWrap;
                            control_dest.Visible = control_source.Visible;
                            control_dest.Dock = control_source.Dock;
                            dest.Controls.Add(control_dest);
                            break;
                        }
                    }

                    {
                        BL_FlatButton control_source = v as BL_FlatButton;
                        if (control_source != null)
                        {
                            BL_FlatButton control_dest = new BL_FlatButton();
                            control_dest.Size = control_source.Size;
                            control_dest.Top = control_source.Top;
                            control_dest.Left = control_source.Left;
                            control_dest.Font = control_source.Font;
                            control_dest.TextAlign = control_source.TextAlign;
                            control_dest.Name = control_source.Name + name_suffix;
                            control_dest.Anchor = control_source.Anchor;
                            control_dest.BackColor = control_source.BackColor;
                            control_dest.ForeColor = control_source.ForeColor;
                            control_dest.CheckMode = control_source.CheckMode;
                            //control_dest.Checked = false;
                            control_dest.BackColorOFF = control_source.BackColorOFF;
                            control_dest.BackColorON = control_source.BackColorON;
                            control_dest.BackColorNormal = control_source.BackColorNormal;
                            control_dest.ForeColorOFF = control_source.ForeColorOFF;
                            control_dest.ForeColorON = control_source.ForeColorON;
                            control_dest.ForeColorNormal = control_source.ForeColorNormal;
                            control_dest.Visible = control_source.Visible;
                            control_dest.Dock = control_source.Dock;
                            dest.Controls.Add(control_dest);
                            break;
                        }
                    }

                    {
                        Button control_source = v as Button;
                        if (control_source != null)
                        {
                            Button control_dest = new Button();
                            control_dest.Size = control_source.Size;
                            control_dest.Top = control_source.Top;
                            control_dest.Left = control_source.Left;
                            control_dest.Font = control_source.Font;
                            control_dest.TextAlign = control_source.TextAlign;
                            control_dest.Name = control_source.Name + name_suffix;
                            control_dest.Anchor = control_source.Anchor;
                            control_dest.BackColor = control_source.BackColor;
                            control_dest.ForeColor = control_source.ForeColor;
                            control_dest.Visible = control_source.Visible;
                            control_dest.Dock = control_source.Dock;
                            dest.Controls.Add(control_dest);
                            break;
                        }
                    }
                }
                while (false);
            }

            return dest;
        }
    }
}
