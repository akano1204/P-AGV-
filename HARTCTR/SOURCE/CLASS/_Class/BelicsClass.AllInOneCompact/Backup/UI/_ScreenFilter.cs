using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using System.Drawing.Design;
using System.Drawing.Drawing2D;

namespace BelicsClass.UI
{
    /// <summary>
    /// 半透明クリック防護フォーム
    /// </summary>
    public partial class BL_ScreenFilter : Form
    {
        private Point m_ExcludeLocation = new Point();
        private Size m_ExcludeSize = new Size();

        /// <summary>
        /// 防護対象外エリアの左上座標を設定・取得します。
        /// </summary>
        public Point ExcludeLocation
        {
            get { return m_ExcludeLocation; }
            set { m_ExcludeLocation = value; }
        }

        /// <summary>
        /// 防護対象外エリアのサイズを設定・取得します。
        /// </summary>
        public Size ExcludeSize
        {
            get { return m_ExcludeSize; }
            set { m_ExcludeSize = value; }
        }

        /// <summary>
        /// コンストラクタです
        /// </summary>
        public BL_ScreenFilter()
        {
            InitializeComponent();
            //!TopMost = true;
        }

        /// <summary>
        /// フォームサイズの初期化
        /// ExcludeLocationとExcludeSizeが設定されているときにのみ有効です。
        /// </summary>
        public void SetFilerAria(Form owner)
        {
            //if( m_ExcludeSize.IsEmpty )
            //{
            //    throw new Exception( "[TransparentFilterForm:SetFilerAria()] このフォーム表示する前に必要な設定がされていません。"+
            //                        "\n\n public void ShowKeep( Form owner )メソッドを使うか、個別にプロパティ設定を行った後にSetFilerAria() をCallしてください。" );
            //}

            Form mainform = null;

            Rectangle owner_rect = owner.Bounds;
            Rectangle target_bounds = Screen.PrimaryScreen.Bounds;

            if (typeof(BL_MainForm_Base).IsInstanceOfType(owner.Owner))
            {
                mainform = (BL_MainForm_Base)owner.Owner;
            }
            else if (typeof(BL_SubForm_Base).IsInstanceOfType(owner.Owner))
            {
                mainform = ((BL_SubForm_Base)owner.Owner).m_Mainform;
            }

            if (mainform == null)
            {
                foreach (Screen scr in Screen.AllScreens)
                {
                    if (scr.Bounds.IntersectsWith(owner_rect))
                    {
                        target_bounds = scr.Bounds;
                        break;
                    }
                }

                m_ExcludeLocation = new Point(m_ExcludeLocation.X - target_bounds.Location.X, m_ExcludeLocation.Y - target_bounds.Location.Y);
            }
            else
            {
                target_bounds = mainform.Bounds;

                if (typeof(BL_SubForm_Base).IsInstanceOfType(owner.Owner))
                {
                    if (((BL_SubForm_Base)owner.Owner).m_Functions != null)
                    {
                        target_bounds = owner.Owner.Bounds;
                    }
                }

                m_ExcludeLocation = new Point(m_ExcludeLocation.X - target_bounds.Location.X, m_ExcludeLocation.Y - target_bounds.Location.Y);
            }

            // 画面全体サイズを登録
            this.Location = target_bounds.Location;
            this.Size = target_bounds.Size;

            // Region用のグラフィックパスを作成
            GraphicsPath gp = new GraphicsPath();
            // 防護エリア全体の矩形を登録
            gp.AddRectangle(new Rectangle(0, 0, target_bounds.Width, target_bounds.Height));
            // 別ウィンドウのサイズを登録(再登録した重複領域は、グラフィックス上で除外エリアとなる)
            gp.AddRectangle(new Rectangle(m_ExcludeLocation, m_ExcludeSize));

            // フォームの領域セット
            this.Region = new Region(gp);

            // グラフィックパス破棄
            gp.Dispose();
        }

        /// <summary>
        /// 防護フォームを表示します
        /// </summary>
        /// <param name="owner">防護対象のオーナーフォーム</param>
        /// <param name="backColor">防護フィルタの背景色</param>
        /// <param name="Opacity">防護フィルタの不透明度</param>
        public void ShowKeep(Form owner, Color backColor, double Opacity)
        {
            m_ExcludeLocation = owner.Location;
            m_ExcludeSize = owner.Size;

            this.BackColor = backColor;
            this.Opacity = Opacity;

            SetFilerAria(owner);

            this.Show(owner);
        }

        /// <summary>
        /// 防護フォームを表示します。
        /// </summary>
        /// <param name="owner">防護対象のオーナーフォーム</param>
        public void ShowKeep(Form owner)
        {
            ShowKeep(owner, this.BackColor, this.Opacity);
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        private void TransparentFilterForm_Load(object sender, EventArgs e)
        {
            //if( m_ExcludeSize.IsEmpty )
            //{
            //    throw new Exception( "[TransparentFilterForm:Form_Load()] このフォーム表示する前に必要な設定がされていません。"+
            //                        "\n\n public void ShowKeep( Form owner )メソッドを使うか、個別にプロパティ設定を行った後にSetFilerAria() をCallしてください。" );
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            if (Owner != null) Owner.Focus();
        }

        int debug_step = 0;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseDoubleClick(MouseEventArgs e)
        {
            switch (debug_step)
            {
                case 0:
                    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        this.Opacity = 0.6D;
                        debug_step++;
                    }
                    else
                    {
                        this.Opacity = 0.8D;
                        debug_step = 0;
                    }
                    break;

                case 1:
                    if (e.Button == System.Windows.Forms.MouseButtons.Right)
                    {
                        this.Opacity = 0.4D;
                        debug_step++;
                    }
                    else
                    {
                        this.Opacity = 0.8D;
                        debug_step = 0;
                    }
                    break;

                case 2:
                    if (e.Button == System.Windows.Forms.MouseButtons.Left)
                    {
                        this.Opacity = 0.2D;
                        debug_step++;
                    }
                    else
                    {
                        this.Opacity = 0.8D;
                        debug_step = 0;
                    }
                    break;

                default:
                    this.Opacity = 0.8D;
                    debug_step = 0;
                    break;
            }

            if (debug_step == 3)
            {
                this.Hide();
            }

            base.OnMouseDoubleClick(e);
        }
    }
}
