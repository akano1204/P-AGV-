using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.Drawing;

using BelicsClass.Common;
using Microsoft.VisualBasic.PowerPacks;
//using System.Windows.Forms.DataVisualization;
using iTextSharp.text.pdf;

namespace BelicsClass.UI.Report
{
    /// <summary>
    /// レポートビュークラス
    /// </summary>
    public class BL_ReportView_Base : System.Windows.Forms.UserControl
    {
        #region 必要なデザイナ変数です。

        /// <summary>必要なデザイナ変数です。 </summary>
        private System.ComponentModel.Container components = null;

                /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_ReportView_Base()
        {
            // この呼び出しは、Windows.Forms フォーム デザイナで必要です。
            InitializeComponent();
            FixedDotPenStyle.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_ReportView_Base(BL_ReportControl_Base report_controller, BL_ReportData_Base.BL_ReportPageParam page_parameters)
            :this()
        {
            Report_controller = report_controller;
            Report_Data = new BL_ReportData_Base(page_parameters);
        }

        /// <summary> 使用されているリソースに後処理を実行します。</summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }
            base.Dispose(disposing);
        }

        #region 使用されているリソースに後処理を実行します。

        /// <summary> デザイナ サポートに必要なメソッドです。このメソッドの内容を コード エディタで変更しないでください。 </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BL_ReportView_Base
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(20);
            this.Name = "BL_ReportView_Base";
            this.Size = new System.Drawing.Size(990, 661);
            this.ResumeLayout(false);

        }

        #endregion

        #endregion

        /// <summary> スケーリングモード列挙体 </summary>
        public enum ScalingModes
        {
            /// <summary>
            /// 縦固定
            /// </summary>
            Portrait,
            /// <summary>
            /// 横固定
            /// </summary>
            Landscape,
            /// <summary>
            /// オートスケーリング
            /// 入力されたコントロールの縦横比から用紙の縦横をオート選択
            /// </summary>
            Auto,
        }

        private ScalingModes scalingMode = ScalingModes.Portrait;
        /// <summary>スケーリングモード</summary>
        public ScalingModes ScalingMode { get { return scalingMode; } set { scalingMode = value; } }

        /// <summary>PDFの印刷スケール</summary>
        public float PdfScale = 1.0F;

        private PaperKind paperKind = PaperKind.A4;
        /// <summary>用紙サイズ</summary>
        public PaperKind PaperKind { get { return paperKind; } set { paperKind = value; } }

        private bool autoZoom = false;
        /// <summary>自動調整</summary>
        public bool AutoZoom { get { return autoZoom; } set { autoZoom = value; } }

        private double zoom = 1.0;
        /// <summary>印刷プレビューのズームを取得または設定します</summary>
        public double Zoom { get { return zoom; } set { zoom = value; } }

        private Pen fixedDotPenStyle = new Pen(Brushes.Black, 1.0f);
        /// <summary></summary>
        public Pen FixedDotPenStyle { get { return fixedDotPenStyle; } set { fixedDotPenStyle = value; } }
        private Pen fixedSinglePenStyle = new Pen(Brushes.Black, 1.0f);
        /// <summary></summary>
        public Pen FixedSinglePenStyle { get { return fixedSinglePenStyle; } set { fixedSinglePenStyle = value; } }
        private Pen fixed3DPenStyle = new Pen(Brushes.Black, 3.0f);
        /// <summary></summary>
        public Pen Fixed3DPenStyle { get { return fixed3DPenStyle; } set { fixed3DPenStyle = value; } }

        private bool pageEditable = false;
        /// <summary></summary>
        public bool PageEditable { get { return pageEditable; } set { pageEditable = value; } }

        /// <summary>
        /// レポートで利用するデータ構造本体
        /// </summary>
        public BL_ReportData_Base Report_Data = null;

        /// <summary>
        /// レポートコントローラーへの参照
        /// </summary>
        public BL_ReportControl_Base Report_controller = null;

        /// <summary>
        /// 列タイトル文字列
        /// </summary>
        private List<string> Titles = new List<string>();

        /// <summary>
        /// ヘッダーラベルコントロール
        /// </summary>
        public List<List<Label>> Headers = new List<List<Label>>();

        /// <summary>
        /// 行列ラベルコントロール
        /// </summary>
        public List<List<Label>> Rows = new List<List<Label>>();

        /// <summary>
        /// データ読み込み済みフラグ
        /// </summary>
        public bool Readed = false;

        /// <summary>
        /// レポートページ
        /// </summary>
        public int Report_PageNo = 0;

        /// <summary>
        /// レポートページ数
        /// </summary>
        public int Report_PageCount
        {
            get
            {
                if (Report_controller == null) return 0;
                return Report_controller.Report_PageCount;
            }
        }

        #region レポート初期化(自動行列展開)

                /// <summary>
        /// グリッド形式レポート用に初期化処理を行います
        /// グリッドレポート用に行列分のコントロールを複製して生成します
        /// </summary>
        protected virtual void Initialize(int rows, int fixrows)
        {
            Initialize(rows, fixrows, null);
        }

        /// <summary>
        /// グリッド形式レポート用に初期化処理を行います
        /// グリッドレポート用に行列分のコントロールを複製して生成します
        /// </summary>
        protected virtual void Initialize(int rows, int fixrows, string[] titles)
        {
            if (fixrows < 1) throw new Exception("ヘッダー行は１行以上必要です。");

            int cols = 0;
            int row = 0;
            for (; row < fixrows; row++)
            {
                List<Label> rowdata = new List<Label>();

                for (int col = 0; ; col++)
                {
                    string key = "R" + row.ToString() + "C" + col.ToString();
                    
                    if (!Controls.ContainsKey(key)) break;
                    if(!typeof(Label).IsInstanceOfType(Controls[key])) break;

                    rowdata.Add((Label)Controls[key]);

                    if (cols < col) cols = col;

                    if (titles != null)
                    {
                        if (row == 0 && col < titles.Length)
                        {
                            Titles.Add(titles[col]);
                            ((Label)Controls[key]).Text = titles[col];
                        }
                    }
                }

                if (0 < rowdata.Count)
                {
                    Headers.Add(rowdata);
                }
            }

            Panel panelHeader = null;
            if (Headers.Count == 0)
            {
                row = 0;
                string panelkey = "P" + row.ToString();
                if (Controls.ContainsKey(panelkey))
                {
                    if (1 != fixrows) throw new Exception("パネルのヘッダー行は１行でなければなりません。");
                    if (!typeof(Panel).IsInstanceOfType(Controls[panelkey])) throw new Exception("パネルのヘッダーはパネルコントロールでなければなりません。");

                    panelHeader = (Panel)Controls[panelkey];
                    List<Label> rowdata = new List<Label>();

                    for (int col = 0; ; col++)
                    {
                        string key = "R" + row.ToString() + "C" + col.ToString();

                        if (!panelHeader.Controls.ContainsKey(key)) break;
                        if (!typeof(Label).IsInstanceOfType(panelHeader.Controls[key])) break;

                        rowdata.Add((Label)panelHeader.Controls[key]);

                        if (cols < col) cols = col;

                        if (titles != null)
                        {
                            if (row == 0 && col < titles.Length)
                            {
                                Titles.Add(titles[col]);
                                ((Label)panelHeader.Controls[key]).Text = titles[col];
                            }
                        }
                    }

                    if (0 < rowdata.Count)
                    {
                        Headers.Add(rowdata);
                    }
                }
                row++;
            }

            if (panelHeader == null)
            {
                int topcolumn_maxheight = 0;
                for (int temprow = row; temprow < rows + fixrows; temprow++)
                {
                    int col = 0;
                    Label cell = this.DuplicateLabel(Headers[Headers.Count - 1][col], "R" + temprow.ToString() + "C" + col.ToString(), temprow - fixrows);
                    topcolumn_maxheight = cell.Top + cell.Height;
                }

                for (; row < rows + fixrows; row++)
                {
                    List<Label> rowdata = new List<Label>();

                    for (int col = 0; col <= cols; col++)
                    {
                        Label cell = this.DuplicateLabel(Headers[Headers.Count - 1][col], "R" + row.ToString() + "C" + col.ToString(), row - fixrows);
                        if (topcolumn_maxheight < cell.Top + cell.Height) continue;

                        this.Controls.Add(cell);
                        cell.BringToFront();

                        rowdata.Add(cell);
                    }

                    Rows.Add(rowdata);
                }
            }
            else
            {
                for (; row < rows + fixrows; row++)
                {
                    List<Label> rowdata = new List<Label>();

                    Panel panel = this.DuplucatePanel(panelHeader, "P" + row.ToString(), row - fixrows);
                    this.Controls.Add(panel);
                    panel.BringToFront();

                    for (int col = 0; col <= cols; col++)
                    {
                        if (panel.Controls.ContainsKey("R" + row.ToString() + "C" + col.ToString()))
                        {
                            if (typeof(Label).IsInstanceOfType(panel.Controls["R" + row.ToString() + "C" + col.ToString()]))
                            {
                                rowdata.Add((Label)panel.Controls["R" + row.ToString() + "C" + col.ToString()]);
                            }
                        }
                    }

                    Rows.Add(rowdata);
                }
            }
        }

        /// <summary>
        /// コントロールの複製処理
        /// </summary>
        /// <param name="source">複製元となるラベルコントロール</param>
        /// <param name="name">複製後のコントロール名称</param>
        /// <param name="row">複製先の行番号</param>
        /// <returns></returns>
        protected Label DuplicateLabel(Label source, string name, int row)
        {
            Label dest = null;
            
            if (typeof(BL_ReportLabel).IsInstanceOfType(source))
            {
                dest = new BL_ReportLabel();
                ((BL_ReportLabel)dest).ChangedColor = ((BL_ReportLabel)source).ChangedColor;
                ((BL_ReportLabel)dest).ErrorColor = ((BL_ReportLabel)source).ErrorColor;
                ((BL_ReportLabel)dest).FormatKind = ((BL_ReportLabel)source).FormatKind;
                ((BL_ReportLabel)dest).DataKind = ((BL_ReportLabel)source).DataKind;
            }
            else
            {
                dest = new Label();
            }

            dest.Top = source.Bottom + row * source.Height;
            dest.Left = source.Left;
            dest.Size = source.Size;
            dest.Font = source.Font;
            dest.BorderStyle = source.BorderStyle;
            dest.TextAlign = source.TextAlign;
            dest.Name = name;

            return dest;
        }

        /// <summary>
        /// コントロールの複製処理
        /// </summary>
        /// <param name="source">複製元となるパネルコントロール</param>
        /// <param name="name">複製後のコントロール名称</param>
        /// <param name="row">複製先の行番号</param>
        /// <returns></returns>
        protected Panel DuplucatePanel(Panel source, string name, int row)
        {
            Panel dest = new Panel();

            dest.Top = source.Bottom + row * source.Height;
            dest.Left = source.Left;
            dest.Size = source.Size;
            dest.Font = source.Font;
            dest.BorderStyle = source.BorderStyle;
            dest.Name = name;

            for (int col = 0; ; col++)
            {
                string key = "R0C" + col.ToString();
                if (source.Controls.ContainsKey(key))
                {
                    if (typeof(Label).IsInstanceOfType(source.Controls[key]))
                    {
                        Label labelsource = (Label)source.Controls[key];
                        Label labeldest = null;

                        if (typeof(BL_ReportLabel).IsInstanceOfType(labelsource))
                        {
                            labeldest = new BL_ReportLabel();
                            ((BL_ReportLabel)labeldest).ChangedColor = ((BL_ReportLabel)labelsource).ChangedColor;
                            ((BL_ReportLabel)labeldest).ErrorColor = ((BL_ReportLabel)labelsource).ErrorColor;
                            ((BL_ReportLabel)labeldest).FormatKind = ((BL_ReportLabel)labelsource).FormatKind;
                            ((BL_ReportLabel)labeldest).DataKind = ((BL_ReportLabel)labelsource).DataKind;
                            labeldest.Size = labelsource.Size;
                        }
                        else
                        {
                            labeldest = new Label();
                            labeldest.Size = labelsource.Size;
                            if (labelsource.Text == "")
                            {
                                if (labeldest.Height < 3) labeldest.Height = 1;
                                if (labeldest.Width < 3) labeldest.Width = 1;
                            }
                        }

                        labeldest.Top = labelsource.Top;
                        labeldest.Left = labelsource.Left;
                        labeldest.Font = labelsource.Font;
                        labeldest.BorderStyle = labelsource.BorderStyle;
                        labeldest.TextAlign = labelsource.TextAlign;
                        labeldest.Name = "R" + (row + 1).ToString() + "C" + col.ToString();
                        dest.Controls.Add(labeldest);
                    }
                    else break;
                }
                else break;
            }

            return dest;
        }

        #endregion

        #region 編集機能テキストボックス管理

        /// <summary>
        /// レポートが保持する編集用テキストコントロールのコレクション
        /// </summary>
        public Dictionary<Control, List<BL_ReportTextBox>> textBoxes = new Dictionary<Control, List<BL_ReportTextBox>>();

        private void create_textbox(Control ctr)
        {
            List<BL_ReportTextBox> texts = new List<BL_ReportTextBox>();

            foreach (Control child in ctr.Controls)
            {
                if (!child.Visible) continue;

                if (child.HasChildren)
                {
                    create_textbox(child);
                }

                if (typeof(BL_ReportLabel).IsInstanceOfType(child))
                {
                    BL_ReportTextBox lltb = new BL_ReportTextBox();
                    lltb.Initialize(child);

                    texts.Add(lltb);
                }
            }

            foreach (Control add in texts)
            {
                ctr.Controls.Add(add);
                add.BringToFront();
            }

            textBoxes[ctr] = texts;
        }

        private void delete_textbox()
        {
            foreach (var kv in textBoxes)
            {
                foreach (var val in kv.Value)
                {
                    kv.Key.Controls.Remove(val);
                    val.Dispose();
                }
            }

            textBoxes.Clear();
        }

        /// <summary>
        /// 編集用テキストボックスの初期化
        /// </summary>
        public void Initialize_ReportTextBox()
        {
            delete_textbox();
            create_textbox(this);
        }

        /// <summary>
        /// 編集用テキストボックスでの編集結果反映処理
        /// </summary>
        public void Update_ReportTextBox()
        {
            foreach (var kv in textBoxes)
            {
                foreach (var val in kv.Value)
                {
                    val.UpdateLabel();
                }
            }

            Report_DataGet();
        }

        /// <summary>
        /// 編集用テキストボックスの編集キャンセル処理
        /// </summary>
        public void Cancel_ReportTextBox()
        {
            Report_DataSet();
        }

        #endregion

        #region データインスタンスと画面との相互変換

        /// <summary>
        /// データインスタンスから画面データへの更新
        /// オーバーライドして処理を実装ください
        /// </summary>
        public virtual void Report_DataSet()
        {
        }

        /// <summary>
        /// 画面データからデータインスタンスへの更新
        /// オーバーライドして処理を実装ください
        /// </summary>
        public virtual bool Report_DataGet()
        {
            return false;
        }

        #endregion

        #region 描画処理

        /// <summary>
        /// イメージを描画します
        /// </summary>
        /// <param name="g"></param>
        /// <param name="targetBounds"></param>
        /// <param name="FixedRatio"></param>
        public virtual void DrawImage(Graphics g, Rectangle targetBounds, bool FixedRatio)
        {
            RectangleF bounds = this.ClientRectangle;

            float perSrc = (float)(bounds.Width) / (float)(bounds.Height);
            float perDst = (float)targetBounds.Width / (float)targetBounds.Height;
            float scale = (float)targetBounds.Height / (float)(bounds.Height);
            if (perSrc > perDst) scale = (float)targetBounds.Width / (float)(bounds.Width);

            g.ScaleTransform(scale, scale, System.Drawing.Drawing2D.MatrixOrder.Append);
            g.TranslateTransform(targetBounds.X, targetBounds.Y, System.Drawing.Drawing2D.MatrixOrder.Append);

            //Brush brush1 = new SolidBrush(SystemColors.AppWorkspace);
            //g.FillRectangle(brush1, g.ClipBounds);

            Brush brush2 = new SolidBrush(this.BackColor);
            g.FillRectangle(brush2, bounds);
            
            PrintAllChildControls(this.ClientRectangle.Size, this, new Point((int)bounds.X, (int)bounds.Y), g, null);

            g.ResetTransform();
        }

        /// <summary>
        /// イメージを描画します
        /// </summary>
        /// <param name="g"></param>
        /// <param name="targetBounds"></param>
        public void DrawImage(Graphics g, Rectangle targetBounds)
        {
            DrawImage(g, targetBounds, true);
        }

        /// <summary>
        /// イメージを描画します
        /// </summary>
        /// <param name="g"></param>
        public void DrawImage(Graphics g)
        {
            DrawImage(g, new Rectangle((int)g.ClipBounds.X, (int)g.ClipBounds.Y, (int)g.ClipBounds.Width, (int)g.ClipBounds.Height), true);
        }

        /// <summary>
        /// フォーム内の子コントロールを描画します
        /// </summary>
        /// <param name="docsize"></param>
        /// <param name="root"></param>
        /// <param name="offset"></param>
        /// <param name="g"></param>
        /// <param name="cb"></param>
        public void PrintAllChildControls(Size docsize, Control root, Point offset, Graphics g, PdfContentByte cb)
        {
            for (int i = root.Controls.Count - 1; 0 <= i; i--)
            //foreach (Control control in root.Controls)
            {
                Control control = root.Controls[i];

                // -----------------------------------
                // 見えないコントロールは無視(子供も描かない)
                if (control.Visible)
                {
                    // -----------------------------------
                    // まず自分を描いてから、
                    if (typeof(Label).IsInstanceOfType(control))
                    {
                        Label lbl = control as Label;
                        PrintAsLabel(docsize, lbl, offset, g, cb);
                    }
                    else if (typeof(ShapeContainer).IsInstanceOfType(control))
                    {
                        ShapeContainer lsp = (ShapeContainer)control;
                        foreach (Shape v in lsp.Shapes)
                        {
                            PrintAsShape(docsize, v, offset, g, cb);
                        }
                    }
                    else if (!typeof(BL_ReportTextBox).IsInstanceOfType(control))
                    {
                        PrintAsControl(docsize, control, offset, g, cb);
                    }

                    // -----------------------------------
                    // 子コントロールがあれば、再帰処理
                    if (control.HasChildren)
                    {
                        Point newofs = new Point(offset.X + control.Location.X, offset.Y + control.Location.Y);
                        this.PrintAllChildControls(docsize, control, newofs, g, cb);
                    }
                }
            }
        }

        private void PrintAsShape(Size docsize, Shape shp, Point offset, Graphics g, PdfContentByte cb)
        {
            if (!shp.Visible) return;

            if (cb == null)
            {
                LineShape ls = shp as LineShape;            //直線
                OvalShape os = shp as OvalShape;            //楕円
                RectangleShape rs = shp as RectangleShape;  //四角
                if (ls != null)  //直線
                {
                    Color bordercolor = ls.BorderColor;
                    System.Drawing.Drawing2D.DashStyle borderstype = ls.BorderStyle;
                    int borderwidth = ls.BorderWidth;
                    int x1 = ls.X1 + offset.X;
                    int y1 = ls.Y1 + offset.Y;
                    int x2 = ls.X2 + offset.X;
                    int y2 = ls.Y2 + offset.Y;

                    Pen borderPen = new Pen(bordercolor, borderwidth);
                    g.DrawLine(borderPen, x1, y1, x2, y2);
                }
                else if (os != null)  //楕円
                {
                    Color backcolor = os.BackColor;
                    BackStyle backstyle = os.BackStyle;
                    Color bordercolor = os.BorderColor;
                    System.Drawing.Drawing2D.DashStyle borderstype = os.BorderStyle;
                    int borderwidth = os.BorderWidth;
                    Color fillcolor = os.FillColor;
                    int left = os.Left + offset.X;
                    int top = os.Top + offset.Y;
                    int width = os.Width;
                    int height = os.Height;

                    if (backstyle != BackStyle.Transparent) //塗りつぶす
                    {
                        Brush bkBrush = new SolidBrush(backcolor);
                        g.FillEllipse(bkBrush, left, top, width, height);
                    }

                    Pen borderPen = new Pen(bordercolor, borderwidth);
                    g.DrawEllipse(borderPen, left, top, width, height);
                }
                else if (rs != null)  //四角
                {
                    Color backcolor = rs.BackColor;
                    BackStyle backstyle = rs.BackStyle;
                    Color bordercolor = rs.BorderColor;
                    System.Drawing.Drawing2D.DashStyle borderstype = rs.BorderStyle;
                    int borderwidth = rs.BorderWidth;
                    Color fillcolor = rs.FillColor;
                    int left = rs.Left + offset.X;
                    int top = rs.Top + offset.Y;
                    int width = rs.Width;
                    int height = rs.Height;

                    if (backstyle != BackStyle.Transparent) //塗りつぶす
                    {
                        Brush bkBrush = new SolidBrush(backcolor);
                        g.FillRectangle(bkBrush, left, top, width, height);
                    }

                    Pen borderPen = new Pen(bordercolor, borderwidth);
                    g.DrawRectangle(borderPen, left, top, width, height);
                }
            }
            else
            {
                PdfDocument doc = cb.PdfDocument;

                LineShape ls = shp as LineShape;            //直線
                OvalShape os = shp as OvalShape;            //楕円
                RectangleShape rs = shp as RectangleShape;  //四角
                if (ls != null)  //直線
                {
                    Color bordercolor = ls.BorderColor;
                    System.Drawing.Drawing2D.DashStyle borderstype = ls.BorderStyle;
                    int borderwidth = ls.BorderWidth;
                    float x1 = X(ls.X1, doc);
                    float y1 = Y(ls.Y1, doc);
                    float x2 = X(ls.X2, doc);
                    float y2 = Y(ls.Y2, doc);

                    cb.SetColorStroke(new iTextSharp.text.BaseColor(bordercolor));
                    cb.SetLineWidth(borderwidth);
                    cb.MoveTo(x1, y1);
                    cb.LineTo(x2, y2);

                    //Pen borderPen = new Pen(bordercolor, borderwidth);
                    //g.DrawLine(borderPen, x1, y1, x2, y2);
                }
                else if (os != null)  //楕円
                {
                    Color backcolor = os.BackColor;
                    BackStyle backstyle = os.BackStyle;
                    Color bordercolor = os.BorderColor;
                    System.Drawing.Drawing2D.DashStyle borderstype = os.BorderStyle;
                    int borderwidth = os.BorderWidth;
                    Color fillcolor = os.FillColor;
                    float left = X(os.Left, doc);
                    float top = Y(os.Top, doc);
                    float width = W(os.Width, doc);
                    float height = H(os.Height, doc);

                    cb.SetColorFill(new iTextSharp.text.BaseColor(0, 0, 0, 0));

                    if (backstyle != BackStyle.Transparent) //塗りつぶす
                    {

                        //Brush bkBrush = new SolidBrush(backcolor);
                        //g.FillEllipse(bkBrush, left, top, width, height);
                        cb.SetColorFill(new iTextSharp.text.BaseColor(fillcolor));
                        cb.Ellipse(left, top, left + width, top - height);
                        cb.Fill();
                    }

                    //Pen borderPen = new Pen(bordercolor, borderwidth);
                    //g.DrawEllipse(borderPen, left, top, width, height);

                    cb.SetColorStroke(new iTextSharp.text.BaseColor(bordercolor));
                    cb.SetLineWidth(borderwidth);
                    cb.Ellipse(left, top, left + width, top - height);
                    cb.Stroke();
                }
                else if (rs != null)  //四角
                {
                    Color backcolor = rs.BackColor;
                    BackStyle backstyle = rs.BackStyle;
                    Color bordercolor = rs.BorderColor;
                    System.Drawing.Drawing2D.DashStyle borderstype = rs.BorderStyle;
                    int borderwidth = rs.BorderWidth;
                    Color fillcolor = rs.FillColor;
                    float left = X(rs.Left, doc);
                    float top = Y(rs.Top, doc);
                    float width = W(rs.Width, doc);
                    float height = H(rs.Height, doc);


                    if (backstyle != BackStyle.Transparent) //塗りつぶす
                    {
                        //Brush bkBrush = new SolidBrush(backcolor);
                        //g.FillRectangle(bkBrush, left, top, width, height);

                        cb.SetColorFill(new iTextSharp.text.BaseColor(backcolor));
                        cb.Rectangle(left, top, width, height);
                        cb.Fill();
                    }

                    //Pen borderPen = new Pen(bordercolor, borderwidth);
                    //g.DrawRectangle(borderPen, left, top, width, height);

                    cb.SetLineWidth(borderwidth);
                    cb.SetColorStroke(new iTextSharp.text.BaseColor(bordercolor));
                    cb.Rectangle(left, top, width, height);
                    cb.Stroke();
                }
            }
        }

        private void PrintAsLabel(Size docsize, Label lbl, Point offset, Graphics g, PdfContentByte cb)
        {
            if (cb == null)
            {
                #region プレビュー／印刷用の描画

                // 書き込み位置を調整
                Rectangle bounds = lbl.Bounds;
                bounds.Offset(offset.X - 1, offset.Y - 1);
                //bounds.Height--;
                //bounds.Width--;


                if (bounds.Height < 2 && lbl.Text == "")
                {
                    g.DrawLine(FixedDotPenStyle, bounds.Left, bounds.Top, bounds.Right, bounds.Top);
                }
                else if (bounds.Width < 2 && lbl.Text == "")
                {
                    g.DrawLine(FixedDotPenStyle, bounds.Left, bounds.Top, bounds.Left, bounds.Bottom);
                }
                else
                {
                    // 背景を塗りつぶして、
                    Brush bkBrush = new SolidBrush(lbl.BackColor);
                    g.FillRectangle(bkBrush, bounds);

                    // 枠を描いて、
                    if (lbl.BorderStyle != BorderStyle.None)
                    {
                        Pen borderPen = FixedSinglePenStyle;
                        if (lbl.BorderStyle == BorderStyle.Fixed3D)
                        {
                            borderPen = Fixed3DPenStyle;
                        }
                        g.DrawRectangle(borderPen, bounds);
                    }
                }

                // テキストを描く。
                // まずはアライメントを解析
                StringFormat sf = new StringFormat();
                switch (lbl.TextAlign)
                {
                    case ContentAlignment.TopCenter:	// コンテンツは上端中央に配置されます。 
                        sf.LineAlignment = StringAlignment.Near;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case ContentAlignment.TopLeft:		// コンテンツは上端左寄せに配置されます。 
                        sf.LineAlignment = StringAlignment.Near;
                        sf.Alignment = StringAlignment.Near;
                        break;
                    case ContentAlignment.TopRight:		// コンテンツは上端右寄せに配置されます 
                        sf.LineAlignment = StringAlignment.Near;
                        sf.Alignment = StringAlignment.Far;
                        break;
                    case ContentAlignment.MiddleCenter:	// コンテンツは中段中央に配置されます。 
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case ContentAlignment.MiddleLeft:	// コンテンツは中段左寄せに配置されます。 
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Near;
                        break;
                    case ContentAlignment.MiddleRight:	// コンテンツは中段右寄せに配置されます。 
                        sf.LineAlignment = StringAlignment.Center;
                        sf.Alignment = StringAlignment.Far;
                        break;
                    case ContentAlignment.BottomCenter:	// コンテンツは下端中央に配置されます。 
                        sf.LineAlignment = StringAlignment.Far;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case ContentAlignment.BottomLeft:	// コンテンツは下端左寄せに配置されます。 
                        sf.LineAlignment = StringAlignment.Far;
                        sf.Alignment = StringAlignment.Near;
                        break;
                    case ContentAlignment.BottomRight:	// コンテンツは下端右寄せに配置されます。 
                        sf.LineAlignment = StringAlignment.Far;
                        sf.Alignment = StringAlignment.Far;
                        break;
                }
                // 念のためトリミングも・・・
                sf.Trimming = StringTrimming.EllipsisCharacter;
                // テキスト描画実行
                RectangleF textRect = bounds;

                textRect.Offset(lbl.Padding.Left, lbl.Padding.Top);
                textRect.Inflate(-lbl.Padding.Right, -lbl.Padding.Bottom);

                Brush textBrush = new SolidBrush(lbl.ForeColor);

                //e.Graphics.DrawString(lbl.Text, lbl.Font, textBrush, textRect, sf);
                g.DrawString(lbl.Text, lbl.Font, textBrush, textRect, sf);

                #endregion
            }
            else
            {
                #region PDF用の描画

                PdfDocument doc = cb.PdfDocument;

                // 書き込み位置を調整
                RectangleF bounds = lbl.Bounds;
                bounds.Offset(offset.X, offset.Y);

                cb.SetLineWidth(1f);

                // 枠を描いて、
                // 背景を塗りつぶして、
                if (lbl.BorderStyle != BorderStyle.None && lbl.BackColor != Color.White && lbl.BackColor != Color.Transparent)
                {
                    cb.SetColorStroke(new iTextSharp.text.BaseColor(Color.Black));

                    if (lbl.BackColor != Color.Transparent) //塗りつぶす
                    {
                        cb.SetColorFill(new iTextSharp.text.BaseColor(lbl.BackColor));
                    }

                    if (lbl.BorderStyle == BorderStyle.Fixed3D)
                    {
                        for (int w = 1; w < 3; w++)
                        {
                            cb.Rectangle(X(bounds.Left + w, doc), Y(bounds.Top + w, doc), W(bounds.Width + w * 2, doc), H(bounds.Height + w * 2, doc));
                        }
                    }
                    cb.Rectangle(X(bounds.Left, doc), Y(bounds.Top, doc), W(bounds.Width, doc), H(bounds.Height, doc));
                    cb.FillStroke();
                }
                else
                {
                    // 枠を描いて、
                    if (lbl.BorderStyle != BorderStyle.None)
                    {
                        cb.SetColorStroke(new iTextSharp.text.BaseColor(Color.Black));
                        if (lbl.BorderStyle == BorderStyle.Fixed3D)
                        {
                            for (int w = 1; w < 3; w++)
                            {
                                cb.Rectangle(X(bounds.Left + w, doc), Y(bounds.Top + w, doc), W(bounds.Width + w * 2, doc), H(bounds.Height + w * 2, doc));
                            }
                        }
                        cb.Rectangle(X(bounds.Left, doc), Y(bounds.Top, doc), W(bounds.Width, doc), H(bounds.Height, doc));
                        cb.Stroke();
                    }

                    // 背景を塗りつぶして、
                    if (lbl.BackColor != Color.White && lbl.BackColor != Color.Transparent)
                    {
                        cb.SetColorFill(new iTextSharp.text.BaseColor(lbl.BackColor));
                        cb.Rectangle(X(bounds.Left, doc), Y(bounds.Top, doc), W(bounds.Width, doc), H(bounds.Height, doc));
                        cb.Fill();
                    }
                }

                string fontname = lbl.Font.Name;
                string fontfile = "";

                int pos = 0;
                foreach (KeyValuePair<string, string> kv in BL_ReportControl_Base.m_dicFonts)
                {
                    if (kv.Key.StartsWith(fontname))
                    {
                        fontfile = kv.Value.ToString();
                        break;
                    }
                    pos++;
                }
                if (fontfile.Length == 0) fontfile = @"c:\windows\fonts\msgothic.ttc,0";
                //fontfile = @"c:\windows\fonts\msgothic.ttc,0";

                BaseFont bf = BaseFont.CreateFont(fontfile, BaseFont.IDENTITY_H, BaseFont.EMBEDDED);
                //float FontSize = lbl.Font.Size * ((float)96 / (float)72) * (float)1.095;   //DPI差を調整(+見た目で調整)
                float FontSize = lbl.Font.Size * PdfScale;

                cb.BeginText();
                cb.SetColorStroke(new iTextSharp.text.BaseColor(lbl.ForeColor));
                cb.SetColorFill(new iTextSharp.text.BaseColor(lbl.ForeColor));
                cb.SetFontAndSize(bf, FontSize);
                float x = 0;
                float y = 0;
                int align = PdfContentByte.ALIGN_LEFT;

                string[] lines = lbl.Text.Split('\n');

                switch (lbl.TextAlign)
                {
                    case ContentAlignment.TopCenter:	// コンテンツは上端中央に配置されます。 
                        align = PdfContentByte.ALIGN_CENTER;
                        x = bounds.Left + bounds.Width / 2;
                        y = bounds.Top + FontSize;
                        break;
                    case ContentAlignment.TopLeft:		// コンテンツは上端左寄せに配置されます。 
                        align = PdfContentByte.ALIGN_LEFT;
                        x = bounds.Left;
                        y = bounds.Top + FontSize;
                        break;
                    case ContentAlignment.TopRight:		// コンテンツは上端右寄せに配置されます 
                        align = PdfContentByte.ALIGN_RIGHT;
                        x = bounds.Left + bounds.Width;
                        y = bounds.Top + FontSize;
                        break;
                    case ContentAlignment.MiddleCenter:	// コンテンツは中段中央に配置されます。 
                        align = PdfContentByte.ALIGN_CENTER;
                        x = bounds.Left + bounds.Width / 2;
                        y = bounds.Top + bounds.Height / 2 + FontSize / 2 + ((lines.Length - 1) * FontSize / 2);
                        break;
                    case ContentAlignment.MiddleLeft:	// コンテンツは中段左寄せに配置されます。 
                        align = PdfContentByte.ALIGN_LEFT;
                        x = bounds.Left;
                        y = bounds.Top + bounds.Height / 2 + FontSize / 2 + ((lines.Length - 1) * FontSize / 2);
                        break;
                    case ContentAlignment.MiddleRight:	// コンテンツは中段右寄せに配置されます。 
                        align = PdfContentByte.ALIGN_RIGHT;
                        x = bounds.Left + bounds.Width;
                        y = bounds.Top + bounds.Height / 2 + FontSize / 2 + ((lines.Length - 1) * FontSize / 2);
                        break;
                    case ContentAlignment.BottomCenter:	// コンテンツは下端中央に配置されます。 
                        align = PdfContentByte.ALIGN_CENTER;
                        x = bounds.Left + bounds.Width / 2;
                        y = bounds.Top + bounds.Height + ((lines.Length - 1) * FontSize);
                        break;
                    case ContentAlignment.BottomLeft:	// コンテンツは下端左寄せに配置されます。 
                        align = PdfContentByte.ALIGN_LEFT;
                        x = bounds.Left;
                        y = bounds.Top + bounds.Height + ((lines.Length - 1) * FontSize);
                        break;
                    case ContentAlignment.BottomRight:	// コンテンツは下端右寄せに配置されます。 
                        align = PdfContentByte.ALIGN_RIGHT;
                        x = bounds.Left + bounds.Width;
                        y = bounds.Top + bounds.Height + ((lines.Length - 1) * FontSize);
                        break;
                }

                foreach (string line in lines)
                {
                    cb.ShowTextAligned(align, line, X(x, doc), Y(y, doc), 0);
                    y += FontSize;
                }
                cb.EndText();

                #endregion
            }
        }

        private void PrintAsControl(Size docsize, Control control, Point offset, Graphics g, PdfContentByte cb)
        {
            if (cb == null)
            {
                #region プレビュー／印刷用の描画

                Rectangle bounds = control.Bounds;
                bounds.Offset(offset.X - 1, offset.Y - 1);

                Bitmap bmp = new Bitmap(bounds.Width, bounds.Height);
                control.DrawToBitmap(bmp, new Rectangle(0, 0, bounds.Width, bounds.Height));
                g.DrawImage(bmp, bounds);

                if (typeof(UserControl).IsInstanceOfType(control))
                {
                    if (((UserControl)control).BorderStyle == System.Windows.Forms.BorderStyle.FixedSingle)
                    {
                        g.DrawRectangle(Pens.Black, bounds);
                    }
                    else if (((UserControl)control).BorderStyle == System.Windows.Forms.BorderStyle.Fixed3D)
                    {
                        g.DrawRectangle(Pens.Black, bounds);
                        bounds.Inflate(-1, -1);
                        g.DrawRectangle(Pens.Black, bounds);
                    }
                }

                bmp.Dispose();

                #endregion
            }
            else
            {
                #region PDF用の描画（未使用）


                #endregion
            }
        }


        #region PDF用の座標変換

        /// <summary>
        /// 
        /// </summary>
        /// <param name="y"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        protected float Y(float y, iTextSharp.text.Document doc)
        {
            float yy = doc.PageSize.Height - (doc.TopMargin + y) * PdfScale;
            return yy;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        protected float X(float x, iTextSharp.text.Document doc)
        {
            float xx = (doc.LeftMargin + x) * PdfScale;
            return xx;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="w"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        protected float W(float w, iTextSharp.text.Document doc)
        {
            float ww = w * PdfScale;
            return ww;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="h"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        protected float H(float h, iTextSharp.text.Document doc)
        {
            float hh = -h * PdfScale;
            return hh;
        }

        #endregion

        #endregion

        #region イメージ取得

        /// <summary>
        /// コントロールのイメージを取得します
        /// </summary>
        /// <param name="targetBounds">生成するイメージのサイズ</param>
        /// <param name="FixedRatio">拡大率</param>
        /// <returns>取得できたイメージ</returns>
        public Bitmap GetImage(Rectangle targetBounds, bool FixedRatio)
        {
            Bitmap img = new Bitmap(targetBounds.Width, targetBounds.Height);

            using (Graphics g = Graphics.FromImage(img))
            {
                DrawImage(g, targetBounds, FixedRatio);
            }

            return img;
        }

        /// <summary>
        /// イメージを取得します
        /// </summary>
        /// <param name="targetBounds"></param>
        /// <returns></returns>
        public Bitmap GetImage(Rectangle targetBounds)
        {
            return GetImage(targetBounds, true);
        }

        /// <summary>
        /// イメージを取得します
        /// </summary>
        /// <param name="FixedRatio"></param>
        /// <returns></returns>
        public Bitmap GetImage(bool FixedRatio)
        {
            return GetImage(this.ClientRectangle, FixedRatio);
        }

        /// <summary>
        /// イメージを取得します
        /// </summary>
        /// <returns></returns>
        public Bitmap GetImage()
        {
            return GetImage(this.ClientRectangle, true);
        }

        #endregion

        #region 印刷処理

        /// <summary>
        /// 印刷します
        /// </summary>
        public virtual void PrintOut()
        {
            PrintDocument doc = new PrintDocument();
            doc.PrintController = new StandardPrintController();

            doc.QueryPageSettings += OnQueryPageSettings;
            doc.PrintPage += OnPrintPage;

            doc.Print();

            doc.QueryPageSettings -= OnQueryPageSettings;
            doc.PrintPage -= OnPrintPage;
        }

        /// <summary>
        /// 印刷処理時に呼び出され、印刷準備を行います
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnQueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
            //用紙縦横の設定
            e.PageSettings.Landscape = false;
            if (ScalingMode == ScalingModes.Auto)
            {
                if (this.Height < this.Width)
                {
                    e.PageSettings.Landscape = true;
                }
            }
            else if (ScalingMode == ScalingModes.Landscape)
            {
                e.PageSettings.Landscape = true;
            }

            //マージンの設定
            e.PageSettings.Margins = new Margins(Margin.Left, Margin.Right, Margin.Top, Margin.Bottom);

            //用紙サイズの選択
            foreach (PaperSize ps in e.PageSettings.PrinterSettings.PaperSizes)
            {
                if (0 <= ps.PaperName.IndexOf(this.PaperKind.ToString()))
                {
                    e.PageSettings.PaperSize = ps;
                    break;
                }
            }

        }

        /// <summary>
        /// 印刷処理時に呼び出され、印刷処理を行います
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void OnPrintPage(object sender, PrintPageEventArgs e)
        {
            if (!Readed && Report_Data != null)
            {
                Report_Data.Report_DataRead();
                Readed = true;
            }
            Report_DataSet();

            Rectangle bounds = e.MarginBounds;

            if (!((PrintDocument)sender).PrintController.IsPreview)
            {
                if (e.PageSettings.Landscape)
                {
                    bounds.Width -= (int)(e.Graphics.DpiX / 15);
                    bounds.Height -= (int)(e.Graphics.DpiY / 15);
                }
                else
                {
                    bounds.Width -= (int)(e.Graphics.DpiX / 12);
                    bounds.Height -= (int)(e.Graphics.DpiY / 12);
                }
            }

            DrawImage(e.Graphics, bounds);
        }

        #endregion

        /// <summary>
        /// リストビューなどの連続データによる自ページ増殖処理
        /// </summary>
        /// <returns></returns>
        public virtual int Duplicate()
        {
            //リストビューItemsから、ページ内に印字できる行数分ずつを抜き出して、複数ページ展開を自動化する
            //（未完成）



            return 0;
        }
    }
}
