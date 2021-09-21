using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.IO;
using System.Drawing.Text;
using System.Diagnostics;
using Microsoft.Win32;
//using System.Runtime.InteropServices;   // For DLL importing  

using Microsoft.VisualBasic.PowerPacks;
using iTextSharp.text.pdf;

using BelicsClass.ObjectSync;

namespace BelicsClass.UI.Report
{
    /// <summary>
    /// レポートフォームを管理するコントロールの基本クラス
    /// フォームに貼り付けて使用してください。
    /// </summary>
    public partial class BL_ReportControl_Base : UserControl
    {
        #region PDF生成用フォントの処理

        /// <summary>フォント一覧</summary>
        public static Dictionary<string, string> m_dicFonts = new Dictionary<string, string>();

        /// <summary>
        /// PDF展開用のフォント一覧を列挙しておく(結構処理時間がかかります)
        /// </summary>
        /// <returns></returns>
        public static void LoadFontsFromDirectory()
        {
            string path = Path.Combine(Environment.GetEnvironmentVariable("SystemRoot"), "Fonts");
            //Dictionary<string, string> foundFonts = new Dictionary<string, string>();

            if (Directory.Exists(path))
            {
                foreach (FileInfo fi in new DirectoryInfo(path).GetFiles("*.tt*"))
                {
                    PrivateFontCollection fileFonts = new PrivateFontCollection();
                    fileFonts.AddFontFile(fi.FullName);
                    int pos = 0;
                    foreach (FontFamily ff in fileFonts.Families)
                    {
                        if (!m_dicFonts.ContainsKey(ff.Name))
                        {
                            //add the font only if this fontfamily doesnt exist yet 
                            try
                            {
                                FontFamily family = new FontFamily(ff.Name);
                                if (1 < fileFonts.Families.Length)
                                {
                                    if (fi.Name.ToLower() == "msgothic.ttc")
                                    {
                                        //MS UI Gothic/MS Pゴシック/MS ゴシックだけは列挙順がおかしいので暫定処置
                                        switch (ff.Name)
                                        {
                                            case "ＭＳ ゴシック":
                                                m_dicFonts.Add(family.Name, Path.Combine(path, fi.FullName) + ",0");
                                                break;
                                            case "ＭＳ Ｐゴシック":
                                                m_dicFonts.Add(family.Name, Path.Combine(path, fi.FullName) + ",1");
                                                break;
                                            case "MS UI Gothic":
                                                m_dicFonts.Add(family.Name, Path.Combine(path, fi.FullName) + ",2");
                                                break;
                                        }
                                    }
                                    else
                                    {
                                        m_dicFonts.Add(family.Name, Path.Combine(path, fi.FullName) + "," + pos.ToString());
                                    }
                                }
                                else
                                {
                                    m_dicFonts.Add(family.Name, Path.Combine(path, fi.FullName));
                                }
                            }
                            catch { }
                        }
                        pos++;
                    }
                }
            }

            //return foundFonts;
        }

        ///// <summary>
        ///// PDFファイルにパスワード設定
        ///// </summary>
        ///// <param name="sPdfFilePath">PDFファイルパス</param>
        ///// <param name="sReadPassword">読み取りパスワード</param>
        ///// <param name="sWritePassword">書き込みパスワード</param>
        //private static void SetPdfPassword(string sPdfFilePath, string sReadPassword, string sWritePassword)
        //{
        //    PdfReader reader = null;
        //    iTextSharp.text.Document doc = null;
        //    PdfWriter writer = null;
        //    try
        //    {
        //        // 一時ファイル取得
        //        string sTempFilePath = Path.GetTempFileName();
        //        // PDFファイルからPDFReaderオブジェクト作成
        //        reader = new PdfReader(sPdfFilePath);
        //        // 出力ファイルのDocumentオブジェクト作成
        //        doc = new iTextSharp.text.Document(reader.GetPageSize(1));
        //        // 出力ファイルのPdfWriterオブジェクト作成
        //        writer = PdfWriter.GetInstance(doc, new FileStream(sTempFilePath, FileMode.OpenOrCreate));
        //        // 出力ファイルにパスワード設定
        //        writer.Open();
        //        writer.SetEncryption(
        //            PdfWriter.STRENGTH128BITS, sReadPassword, sWritePassword,
        //            PdfWriter.AllowCopy | PdfWriter.AllowPrinting);
        //        // 出力ファイルDocumentを開く
        //        doc.Open();
        //        // アップロードPDFファイルの内容を出力ファイルに書き込む
        //        PdfContentByte content = writer.DirectContent;
        //        for (int i = 1; i <= reader.NumberOfPages; i++)
        //        {
        //            // 新ページ作成
        //            writer.NewPage();
        //            // 入力ファイルのページ取得
        //            PdfImportedPage pipPage = writer.GetImportedPage(reader, i);
        //            // 入力ファイルのページ内容をダウンロードファイルに挿入
        //            content.AddTemplate(pipPage, 0, 0);
        //        }

        //        // 文章プロパティ設定
        //        //doc.AddKeywords((string)reader.Info["Keywords"]);
        //        //doc.AddAuthor((string)reader.Info["Author"]);
        //        //doc.AddTitle((string)reader.Info["Title"]);
        //        //doc.AddCreator((string)reader.Info["Creator"]);
        //        //doc.AddSubject((string)reader.Info["Subject"]);

        //        // 出力ファイルDocumentを閉じる
        //        doc.Close();
        //        // オリジナルファイルと一時ファイルを置き換える
        //        File.Delete(sPdfFilePath);
        //        File.Move(sTempFilePath, sPdfFilePath);
        //    }
        //    finally
        //    {
        //        // 後始末
        //        if (writer != null)
        //            writer.Close();
        //        if (doc != null)
        //            doc.Close();
        //        if (reader != null)
        //            reader.Close();
        //    }
        //}

        #endregion

        #region プリントオプション

        /// <summary> プリントオプション </summary>
        public class PrintOptions
        {
            /// <summary>
            /// コンストラクタ
            /// </summary>
            public PrintOptions()
            {
                this.DocumentName = Application.ProductName;
                //this.ScalingMode = ScalingModes.Auto;
                this.FixedSiglePenStyle = new Pen(Brushes.Black, 1.0f);
                this.Fixed3DPenStyle = new Pen(Brushes.Black, 3.0f);
            }

            /// <summary>
            /// 複製
            /// </summary>
            /// <param name="src">複製元</param>
            /// <returns></returns>
            public static PrintOptions Duplicate(PrintOptions src)
            {
                PrintOptions newInstance = new PrintOptions();

                newInstance.DocumentName = src.DocumentName;
                newInstance.FixedSiglePenStyle.Dispose();
                newInstance.FixedSiglePenStyle = (Pen)src.FixedSiglePenStyle.Clone();
                newInstance.Fixed3DPenStyle.Dispose();
                newInstance.Fixed3DPenStyle = (Pen)src.Fixed3DPenStyle.Clone();
                newInstance.FilePath = src.FilePath;
                newInstance.bPdf = src.bPdf;
                newInstance.bPrinter = src.bPrinter;

                return newInstance;
            }

            /// <summary>プリンター出力指示</summary>
            public bool bPrinter = true;

            /// <summary>PDF出力指示</summary>
            public bool bPdf = false;
            /// <summary>PDF保存先ファイルパス</summary>
            public string FilePath = "";

            /// <summary>ドキュメント名</summary>
            public String DocumentName;
            /// <summary>コントロールのBorderStyleがFixedSingleのときのPen</summary>
            public Pen FixedSiglePenStyle;
            /// <summary>コントロールのBorderStyleがFixed3DのときのPen</summary>
            public Pen Fixed3DPenStyle;

            /// <summary>印刷対象となるページNoを複数ページ指定します</summary>
            public List<int> TargetPages = new List<int>();
        }

        #endregion

        #region スレッドに渡すデータ

        /// <summary> スレッドに渡すデータの塊</summary>
        protected class threadArgs
        {
            /// <summary></summary>
            public BL_ReportView_Base[] m_pages;
            /// <summary></summary>
            public int m_progress;
            /// <summary></summary>
            public PrintOptions m_options;
            /// <summary></summary>
            public PrintDocument m_document;

            /// <summary></summary>
            public iTextSharp.text.Document m_pdfdoc;
            /// <summary></summary>
            public PdfWriter m_pdf;
        }

        #endregion

        /// <summary></summary>
        protected threadArgs m_threadArgs = null;
        /// <summary></summary>
        protected System.Threading.Thread m_thread = null;

        /// <summary> 内部データです。 </summary>
        protected PrintDocument Document = null;
        
        ///// <summary></summary>
        //protected BL_ReportView_Base[] _Pages = null;

        /// <summary></summary>
        protected PrintOptions Options = null;

        /// <summary>
        /// レポート日時
        /// </summary>
        public DateTime Report_DateTime = new DateTime(0);

        /// <summary>
        /// レポートページ数
        /// </summary>
        public int Report_PageCount { get { return pages.Count; } }

        /// <summary>PDFパスワード保護</summary>
        protected string m_Userid { get; set; }

        /// <summary>PDFパスワード保護</summary>
        protected string m_Password { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_ReportControl_Base()
        {
            //try
            //{
                InitializeComponent();
            //}
            //catch (COMException ex)
            //{
            //    MessageBox.Show("Acrobat Readerをインストールしてください。", "初期化エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    throw ex;
            //}
            //catch (FileNotFoundException ex)
            //{
            //    MessageBox.Show("Acrobat Readerをインストールしてください。", "初期化エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    throw ex;
            //}

            panelPrintForm.Dock = DockStyle.Fill;
            printerPreview.Dock = DockStyle.Fill;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="collapsed"></param>
        public BL_ReportControl_Base(bool collapsed)
            : this()
        {
            splitContainer2.Panel2Collapsed = collapsed;
        }

        /// <summary>
        /// レポート設定領域の可視状態を取得・設定します
        /// </summary>
        public bool HideReportSetting
        {
            get { return splitContainer2.Panel2Collapsed; }
            set
            {
                splitContainer2.Panel2Collapsed = value;

                if (value)
                {
                    buttonSetting.Checked = false;
                }
                else
                {
                    buttonSetting.Checked = true;
                }
            }
        }

        /// <summary>
        /// レポート操作領域の可視状態を取得・設定します
        /// </summary>
        public bool HideReportControl
        {
            get { return splitContainer1.Panel1Collapsed; }
            set
            {
                splitContainer1.Panel1Collapsed = value;
            }
        }

        /// <summary>
        /// レポートのプレビュー表示／印刷処理を行います
        /// </summary>
        /// <param name="pages">レポートのページコントロール配列をページ分</param>
        /// <param name="print_options">印刷オプション</param>
        /// <param name="direct_printout">プレビュー</param>
        /// <param name="print_background"></param>
        /// <returns></returns>
        public bool PrintMe(BL_ReportView_Base[] pages, PrintOptions print_options, bool direct_printout, bool print_background)
        {
            // 以前の印刷が終わっていない時は拒絶
            if (m_thread != null)
            {
                //MessageBox.Show(owner, "以前の印刷ジョブが実行中です。", "印刷",
                //    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // ページ選択コントロールのセットアップ
            listBoxPageList.SelectedIndex = -1;
            listBoxPageList.Items.Clear();

            // １ページもないときはなにもしない
            if (pages.Length <= 0)
            {
                printerPreview.Document = null;
                return false;    // throw new Exception("印刷するページが存在しません。");
            }

            // １ページもVisibleがないときはなにもしない
            int visCnt = 0;
            foreach (var ctrl in pages)
            {
                if (ctrl.Visible)
                {
                    if (print_options.TargetPages.Count == 0 || print_options.TargetPages.Contains(ctrl.Report_PageNo))
                    {
                        visCnt++;
                        break;
                    }
                }
            }

            if (visCnt <= 0)
            {
                printerPreview.Document = null;
                return false;          // throw new Exception("印刷すべきデータが存在しません。");
            }

            // コントロールをセットアップ
            comboBoxPrinterList.Enabled = true;
            buttonPrinterSetting.Enabled = true;
            listBoxPageList.Enabled = true;
            buttonPrint.Enabled = true;
            buttonChange.Enabled = true;

            // オプション指定なしの時はデフォルト
            if (print_options == null)
            {
                print_options = new PrintOptions();
            }

            // 与えられた引数を保存
            this.pages = new List<BL_ReportView_Base>(pages);
            Options = PrintOptions.Duplicate(print_options);

            for (int i = 0; i < pages.Length; i++)
            {
                CheckState ischecked = CheckState.Checked;

                if (0 < print_options.TargetPages.Count)
                {
                    if (!print_options.TargetPages.Contains(i + 1))
                    {
                        ischecked = CheckState.Unchecked;
                    }
                }

                listBoxPageList.Items.Add((i + 1).ToString() + " ページ", ischecked);
            }
            listBoxPageList.SelectedIndex = 0;

            if (direct_printout)
            {
                this.buttonPrint_Click(buttonPrint, new EventArgs());

                if (!print_background)
                {
                    while (timPrintWatcher.Enabled || m_thread != null)
                    {
                        Application.DoEvents();
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// レポートに必要な情報を初期化します
        /// </summary>
        public void Initialize()
        {
            Initialize(true);
        }

        /// <summary>
        /// レポートに必要な情報を初期化します
        /// </summary>
        /// <param name="bUsePDF">PDF出力を使わない場合 false</param>
        public void Initialize(bool bUsePDF)
        {
            try
            {
                Document = null;
                Options = null;
                pages.Clear();

                if (bUsePDF)
                {
                    if (BL_ReportControl_Base.m_dicFonts.Count == 0)
                    {
                        //PDF印刷用フォントの列挙
                        BL_ReportControl_Base.LoadFontsFromDirectory();
                    }
                }

                // デフォルトのドキュメント
                PrintDocument defaultDoc = new PrintDocument();

                // インストールされているプリンタを列挙
                comboBoxPrinterList.SelectedIndex = -1;
                comboBoxPrinterList.Items.Clear();
                foreach (String nm in PrinterSettings.InstalledPrinters)
                {
                    comboBoxPrinterList.Items.Add(nm);
                    if (nm == defaultDoc.PrinterSettings.PrinterName)
                    {
                        comboBoxPrinterList.SelectedIndex = comboBoxPrinterList.Items.Count - 1;
                    }
                }

                // 通常使うプリンタが設定されていない時は、頭のプリンタを選択
                if ((comboBoxPrinterList.Items.Count > 0) && (comboBoxPrinterList.SelectedIndex < 0))
                {
                    comboBoxPrinterList.SelectedIndex = 0;
                }

                // 結果的になにかプリンタが選択できた時は、きちんとドキュメント生成しておく
                if (comboBoxPrinterList.SelectedIndex >= 0)
                {
                    OnSelectPrinter(null, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, ex.TargetSite.Name,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void OnSelectPrinter(object sender, System.EventArgs e)
        {
            // 必要なインスタンスのチェック
            if (comboBoxPrinterList.SelectedIndex < 0) return;
            if (pages.Count == 0) return;

            //try
            {
                // ドキュメントを生成
                PrintDocument doc = new PrintDocument();
                doc.DocumentName = Options.DocumentName;

                // プリンタを設定
                doc.PrinterSettings.PrinterName = comboBoxPrinterList.Text;

                // 制御ロジックを設定
                //doc.PrintController = new StandardPrintController();

                doc.PrintPage += OnPrintPage;
                doc.QueryPageSettings += OnQueryPageSettings;

                // ちゃんと破棄してから保存することにする
                if (Document != null)
                {
                    Document.Dispose();
                    Document = null;
                }
                Document = doc;

                this.OnSelectPage(sender, e);
            }
            //catch (Exception ex)
            //{
            //    MessageBox.Show(this, ex.Message, ex.TargetSite.Name,
            //        MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void OnSelectPage(object sender, System.EventArgs e)
        {
            // 必要なインスタンスのチェック
            if (listBoxPageList.SelectedItems.Count == 0) return;

            if (pages.Count == 0) return;
            if (Options == null) return;
            if (Document == null)
            {
                OnSelectPrinter(comboBoxPrinterList, e);
                if (Document == null) return;
            }

            //try
            {
                if (listBoxPageList.SelectedIndex >= 0)
                {
                    pages[listBoxPageList.SelectedIndex].Initialize_ReportTextBox();

                    m_threadArgs = new threadArgs();
                    m_threadArgs.m_pages = new BL_ReportView_Base[1];
                    m_threadArgs.m_pages[0] = pages[listBoxPageList.SelectedIndex];
                    m_threadArgs.m_options = PrintOptions.Duplicate(Options);
                    m_threadArgs.m_progress = 0;
                    m_threadArgs.m_document = Document;

                    printerPreview.Document = Document;
                    printerPreview.AutoZoom = pages[listBoxPageList.SelectedIndex].AutoZoom;

                    printerPreview.InvalidatePreview();
                }

                if (listBoxPageList.CheckedItems.Count > 0)
                {
                    buttonPrint.Enabled = true;
                }
                else
                {
                    buttonPrint.Enabled = false;
                }
            }
            //catch (Exception ex)
            //{
            //    MessageBox.Show(this, ex.Message, ex.TargetSite.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private void OnQueryPageSettings(object sender, QueryPageSettingsEventArgs e)
        {
            int prog;
            lock (this)
            {
                prog = m_threadArgs.m_progress;
            }

            if (0 <= prog && prog < m_threadArgs.m_pages.Length)
            {
                BL_ReportView_Base doc = m_threadArgs.m_pages[prog];
                if (doc != null)
                {
                    doc.OnQueryPageSettings(sender, e);
                }
            }
        }

        private void OnPrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            //try
            {
                // 進捗はロックして取得
                int prog;
                lock (this)
                {
                    prog = m_threadArgs.m_progress;
                }
                // 中断
                if (prog < 0)
                {
                    e.Cancel = true;
                    e.HasMorePages = false;
                    return;
                }
                // 念のため進捗インデックスをブロック
                if (prog >= m_threadArgs.m_pages.Length)
                {
                    e.HasMorePages = false;
                    return;
                }

                // 書き込み元コントロールを取得
                BL_ReportView_Base doc = m_threadArgs.m_pages[prog];
                if (doc != null)
                {
                    doc.OnPrintPage(sender, e);
                }

                // 印刷プロセス終了の判定
                prog++;
                if (prog >= m_threadArgs.m_pages.Length)
                {
                    e.HasMorePages = false;
                }
                else
                {
                    e.HasMorePages = true;
                }

                // 進捗を保存
                lock (this)
                {
                    if (m_threadArgs.m_progress >= 0)
                    {
                        m_threadArgs.m_progress = prog;
                    }
                }
            }
            //catch (System.Threading.ThreadAbortException)
            //{
            //    // スレッド終了リクエストでキャンセル
            //    e.HasMorePages = false;
            //    e.Cancel = true;
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(this, ex.Message, ex.TargetSite.Name,
            //        MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    e.HasMorePages = false;
            //    e.Cancel = true;
            //}

        }

        private void PrintProc()
        {
            //try
            {
                //プリンター出力
                if (m_threadArgs.m_options.bPrinter /*&& !m_threadArgs.m_options.bPdf*/)
                {
                    #region プリンターへ印刷

                    m_threadArgs.m_document.Print();

                    #endregion
                }
                
                //PDF出力
                if (m_threadArgs.m_options.bPdf)
                {
                    if (BL_ReportControl_Base.m_dicFonts.Count == 0)
                    {
                        //PDF印刷用フォントの列挙
                        BL_ReportControl_Base.LoadFontsFromDirectory();
                    }

                    #region PDFへ出力

                    string filepath = m_threadArgs.m_options.FilePath;
                    if (filepath.Trim() == "")
                    {
                        filepath = Path.Combine(Application.StartupPath, "pdf" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".pdf");
                    }

                    FileStream fs = new FileStream(filepath, FileMode.Create);
                    m_threadArgs.m_pdfdoc = new iTextSharp.text.Document();
                    m_threadArgs.m_pdf = PdfWriter.GetInstance(m_threadArgs.m_pdfdoc, fs);

                    bool bFirstPage = true;
                    foreach (BL_ReportView_Base page in m_threadArgs.m_pages)
                    {
                        if (!bFirstPage) m_threadArgs.m_pdfdoc.NewPage();
                        bFirstPage = false;

                        #region 用紙サイズの選択

                        BL_ReportView_Base.ScalingModes mode = page.ScalingMode;
                        if (mode == BL_ReportView_Base.ScalingModes.Auto)
                        {
                            mode = BL_ReportView_Base.ScalingModes.Portrait;
                            if (page.Height < page.Width) mode = BL_ReportView_Base.ScalingModes.Landscape;
                        }

                        if (mode == BL_ReportView_Base.ScalingModes.Landscape)
                        {
                            if (page.PaperKind == PaperKind.A2) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.A2.Rotate());
                            else if (page.PaperKind == PaperKind.A3) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.A3.Rotate());
                            else if (page.PaperKind == PaperKind.A4) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                            else if (page.PaperKind == PaperKind.A5) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.A5.Rotate());
                            else if (page.PaperKind == PaperKind.A6) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.A6.Rotate());
                            else if (page.PaperKind == PaperKind.B4) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.B4.Rotate());
                            else if (page.PaperKind == PaperKind.B5) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.B5.Rotate());
                            else throw new Exception("適合する用紙サイズを選択できません。");
                        }
                        else if (mode == BL_ReportView_Base.ScalingModes.Portrait)
                        {
                            if (page.PaperKind == PaperKind.A2) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.A2);
                            else if (page.PaperKind == PaperKind.A3) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.A3);
                            else if (page.PaperKind == PaperKind.A4) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.A4);
                            else if (page.PaperKind == PaperKind.A5) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.A5);
                            else if (page.PaperKind == PaperKind.A6) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.A6);
                            else if (page.PaperKind == PaperKind.B4) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.B4);
                            else if (page.PaperKind == PaperKind.B5) m_threadArgs.m_pdfdoc.SetPageSize(iTextSharp.text.PageSize.B5);
                            else throw new Exception("適合する用紙サイズを選択できません。");
                        }

                        #endregion

                        iTextSharp.text.Document doc = m_threadArgs.m_pdfdoc;
                        doc.SetMargins(page.Margin.Left, page.Margin.Right, page.Margin.Top, page.Margin.Bottom);
                        doc.OpenDocument();

                        {
                            if (!page.Readed && page.Report_Data != null)
                            {
                                page.Report_Data.Report_DataRead();
                                page.Readed = true;
                            }
                            page.Report_DataSet();
                            
                            Image img = page.GetImage(new Rectangle(0, 0, page.Width * 3, page.Height * 3));
                            iTextSharp.text.Image image = iTextSharp.text.Image.GetInstance(img, System.Drawing.Imaging.ImageFormat.Bmp);
                            image.ScaleToFit(doc.PageSize.Width - doc.LeftMargin - doc.RightMargin, doc.PageSize.Height - doc.TopMargin - doc.BottomMargin);
                            doc.Add(image);
                        }

                        //{
                        //    PdfContentByte cb = m_threadArgs.m_pdf.DirectContent;

                        //    float perSrc = (float)(page.Width) / (float)(page.Height);
                        //    float perDst = (float)(doc.PageSize.Width - doc.LeftMargin - doc.RightMargin) / (float)(doc.PageSize.Height - doc.TopMargin - doc.BottomMargin);
                        //    float scale = (float)(doc.PageSize.Height - doc.TopMargin - doc.BottomMargin) / (float)(doc.PageSize.Height);
                        //    if (perSrc > perDst) scale = (float)(doc.PageSize.Width - doc.LeftMargin - doc.RightMargin) / (float)(page.Width);

                        //    page.PdfScale = scale;

                        //    //cb.SetLineWidth(2.0F);
                        //    //cb.SetColorStroke(new iTextSharp.text.BaseColor(Color.Black));
                        //    //cb.MoveTo(page.X(0, doc), page.Y(0, doc));
                        //    //cb.LineTo(page.X(100, doc), page.Y(200, doc));
                        //    //cb.Rectangle(page.X(0, doc), page.Y(0, doc), page.W(100, doc), page.H(200, doc));
                        //    //cb.Ellipse(page.X(90, doc), page.Y(190, doc), page.X(110, doc), page.Y(210, doc));
                        //    //cb.Stroke();

                        //    page.PrintAllChildControls(new Size(page.Width, page.Height), page, page.Location, null, cb);
                        //}
                    }

                    m_threadArgs.m_pdfdoc.Close();

                    ////編集パスワードを設定
                    //m_threadArgs.m_pdf.SetEncryption(PdfWriter.STRENGTH128BITS, m_Userid, m_Password, PdfWriter.AllowPrinting | PdfWriter.AllowScreenReaders | PdfWriter.AllowCopy);
                    
                    fs.Close();

                    #endregion

                    #region Acrobatでのプリンター出力は使わない

                    ////プリンターへ出力
                    //if (m_threadArgs.m_options.bPrinter)
                    //{
                    //    axAcroPDF1.LoadFile(filepath);
                    //    axAcroPDF1.printAll();

                    //    #region PDFを印刷

                    //    ////string sPdfFn = "D:\\" + Path.GetFileNameWithoutExtension(filepath) + ".pdf";
                    //    ////File.Copy(filepath, sPdfFn);

                    //    //RegistryKey rKey = Registry.ClassesRoot.OpenSubKey(@"acrobat\shell\open\command");
                    //    //string rValue = (string)rKey.GetValue("");
                    //    //int pos = rValue.IndexOf("\"", 1);
                    //    //rValue = rValue.Substring(1, pos - 1);

                    //    //Process printProcess = new Process();
                    //    //printProcess.StartInfo.FileName = rValue;
                    //    //printProcess.StartInfo.Verb = "open";
                    //    //printProcess.StartInfo.Arguments = " /n /h /s /t " + filepath;// +" \\\\Network01\\HP LaserJet 8150 PCL";
                    //    ////printProcess.StartInfo.Arguments = " /n /h /s /t " + sPdfFn;

                    //    //if (m_Userid != null)
                    //    //{
                    //    //    if (m_Userid != "")
                    //    //    {
                    //    //        printProcess.StartInfo.UserName = m_Userid;
                    //    //        System.Security.SecureString ss = new System.Security.SecureString();
                    //    //        int plen = m_Password.Length;
                    //    //        char[] pwArray = m_Password.ToCharArray();
                    //    //        for (int i = 0; i < plen; ++i)
                    //    //        {
                    //    //            ss.AppendChar(pwArray[i]);
                    //    //        }

                    //    //        printProcess.StartInfo.Password = ss;
                    //    //    }
                    //    //}

                    //    ////printProcess.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    //    //printProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    //    //printProcess.StartInfo.CreateNoWindow = false;
                    //    //printProcess.StartInfo.UseShellExecute = false;
                    //    //printProcess.StartInfo.LoadUserProfile = true;
                    //    //printProcess.Start();

                    //    ////印刷終了を待機
                    //    //while (true)
                    //    //{
                    //    //    printProcess.Refresh();
                    //    //    System.Threading.Thread.Sleep(100);

                    //    //    try
                    //    //    {
                    //    //        if (printProcess.MainWindowTitle != "Adobe Reader")
                    //    //        {
                    //    //            continue;
                    //    //        }
                    //    //    }
                    //    //    catch { }

                    //    //    break;
                    //    //}
                    //    ////printProcess.WaitForExit();
                    //    ////File.Delete(sPdfFn);

                    //    //try
                    //    //{
                    //    //    //印刷終了後、Acrobat Readerを閉じる
                    //    //    printProcess.Kill();
                    //    //    printProcess.Close();
                    //    //    printProcess.Dispose();
                    //    //}
                    //    //catch { }

                    //    #endregion
                    //}

                    #endregion

                    //編集パスワードを設定
                    //fnSetPdfPassword(filepath, "", "pass");
                }
            }
            //catch (System.Threading.ThreadAbortException)
            //{
            //    // スレッド終了リクエストはそのまま終わる
            //    m_threadArgs.m_document = null;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //    MessageBox.Show(this, ex.Message, ex.TargetSite.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            //}
        }

        private iTextSharp.text.Rectangle myPaperSize(float x, float y)
        {
            // 誤差範囲10%とする
            // ※スキャン状態によって多少の誤差があるため
            float gosa = 0.9f;

            if (iTextSharp.text.PageSize.A0.Height < (y * gosa)) return iTextSharp.text.PageSize.B0;
            if (iTextSharp.text.PageSize.B1.Height < (y * gosa)) return iTextSharp.text.PageSize.A0;
            if (iTextSharp.text.PageSize.A1.Height < (y * gosa)) return iTextSharp.text.PageSize.B1;
            if (iTextSharp.text.PageSize.B2.Height < (y * gosa)) return iTextSharp.text.PageSize.A1;
            if (iTextSharp.text.PageSize.A2.Height < (y * gosa)) return iTextSharp.text.PageSize.B2;
            if (iTextSharp.text.PageSize.B3.Height < (y * gosa)) return iTextSharp.text.PageSize.A2;
            if (iTextSharp.text.PageSize.A3.Height < (y * gosa)) return iTextSharp.text.PageSize.B3;
            if (iTextSharp.text.PageSize.B4.Height < (y * gosa)) return iTextSharp.text.PageSize.A3;
            if (iTextSharp.text.PageSize.A4.Height < (y * gosa)) return iTextSharp.text.PageSize.B4;
            if (iTextSharp.text.PageSize.B5.Height < (y * gosa)) return iTextSharp.text.PageSize.A4;
            if (iTextSharp.text.PageSize.A5.Height < (y * gosa)) return iTextSharp.text.PageSize.B5;
            if (iTextSharp.text.PageSize.B6.Height < (y * gosa)) return iTextSharp.text.PageSize.A5;
            if (iTextSharp.text.PageSize.A6.Height < (y * gosa)) return iTextSharp.text.PageSize.B6;
            if (iTextSharp.text.PageSize.B7.Height < (y * gosa)) return iTextSharp.text.PageSize.A6;
            if (iTextSharp.text.PageSize.A7.Height < (y * gosa)) return iTextSharp.text.PageSize.B7;
            if (iTextSharp.text.PageSize.B8.Height < (y * gosa)) return iTextSharp.text.PageSize.A7;
            if (iTextSharp.text.PageSize.A8.Height < (y * gosa)) return iTextSharp.text.PageSize.B8;

            return iTextSharp.text.PageSize.A8;
        }

        private void buttonPrint_Click(object sender, EventArgs e)
        {
            // 必要なインスタンスのチェック
            if (pages.Count == 0) return;
            if (Options == null) return;
            if (Document == null) return;

            try
            {
                // コントロールをディスエーブル
                comboBoxPrinterList.Enabled = false;
                buttonPrinterSetting.Enabled = false;
                listBoxPageList.Enabled = false;
                buttonPrint.Enabled = false;
                buttonChange.Enabled = false;
                buttonSetting.Enabled = false;
                buttonSelectAll.Enabled = false;
                buttonUnselectAll.Enabled = false;

                // スレッドに渡すデータをセットアップ
                m_threadArgs = new threadArgs();
                m_threadArgs.m_pages = new BL_ReportView_Base[listBoxPageList.CheckedItems.Count];
                int pg = 0;
                foreach (object item in listBoxPageList.CheckedItems)
                {
                    int idx = listBoxPageList.Items.IndexOf(item);
                    m_threadArgs.m_pages[pg++] = pages[idx];
                }
                m_threadArgs.m_options = PrintOptions.Duplicate(Options);
                m_threadArgs.m_progress = 0;
                m_threadArgs.m_document = Document;

                PrintProc();
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, ex.TargetSite.Name, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // コントロールをセットアップ
            comboBoxPrinterList.Enabled = true;
            buttonPrinterSetting.Enabled = true;
            listBoxPageList.Enabled = true;
            buttonPrint.Enabled = true;
            buttonChange.Enabled = true;
            buttonSetting.Enabled = true;
            buttonSelectAll.Enabled = true;
            buttonUnselectAll.Enabled = true;

            this.Report_Preview(Options);
        }

        #region 画面操作
        
        private void timPrintWatcher_Tick(object sender, EventArgs e)
        {
            if (m_thread != null)
            {
                if (!m_thread.IsAlive)
                {
                    timPrintWatcher.Enabled = false;

                    m_thread.Join();
                    m_threadArgs = null;
                    m_thread = null;

                    // コントロールをセットアップ
                    comboBoxPrinterList.Enabled = true;
                    buttonPrinterSetting.Enabled = true;
                    listBoxPageList.Enabled = true;
                    buttonPrint.Enabled = true;
                    buttonChange.Enabled = true;
                    buttonSetting.Enabled = true;
                    buttonSelectAll.Enabled = true;
                    buttonUnselectAll.Enabled = true;
                }
            }
            else
            {
                timPrintWatcher.Enabled = false;

                // コントロールをセットアップ
                comboBoxPrinterList.Enabled = true;
                buttonPrinterSetting.Enabled = true;
                listBoxPageList.Enabled = true;
                buttonPrint.Enabled = true;
                buttonChange.Enabled = true;
                buttonSetting.Enabled = true;
                buttonSelectAll.Enabled = true;
                buttonUnselectAll.Enabled = true;
            }
        }

        private void buttonSetting_Click(object sender, EventArgs e)
        {
            if (splitContainer2.Panel2Collapsed)
            {
                splitContainer2.Panel2Collapsed = false;
            }
            else
            {
                splitContainer2.Panel2Collapsed = true;
            }
        }

        private void buttonChange_Click(object sender, EventArgs e)
        {
            if (buttonChange.Checked)
            {
                if (pages.Count == 0) return;

                int index = 0;
                if (0 <= listBoxPageList.SelectedIndex) index = listBoxPageList.SelectedIndex;
                panelPrintForm.Controls.Add(pages[index]);

                buttonCancel.Enabled = true;
                buttonChange.Text = "編集完了";
                buttonPrint.Enabled = false;
                buttonSetting.Enabled = false;
                buttonPrinterSetting.Enabled = false;
                buttonSelectAll.Enabled = false;
                buttonUnselectAll.Enabled = false;
                comboBoxPrinterList.Enabled = false;
                listBoxPageList.Enabled = false;

                panelPrintForm.Invalidate();
                Application.DoEvents();

                foreach (Control p in panelPrintForm.Controls)
                {
                    p.Top = 0;
                    p.Left = 0;
                    if (p.Width < printerPreview.Width)
                    {
                        p.Left = printerPreview.Width / 2 - p.Width / 2;
                    }
                }

                panelPrintForm.Visible = true;
                printerPreview.Visible = false;
            }
            else
            {
                if (pages.Count == 0) return;

                int index = 0;
                if (0 <= listBoxPageList.SelectedIndex) index = listBoxPageList.SelectedIndex;
                BL_ReportView_Base page = (BL_ReportView_Base)pages[index];
                page.Update_ReportTextBox();

                PrintMe(pages.ToArray(), Options, false, false);

                buttonCancel.Enabled = false;
                buttonChange.Text = "編集";
                buttonPrint.Enabled = true;
                buttonSetting.Enabled = true;
                buttonPrinterSetting.Enabled = true;
                buttonSelectAll.Enabled = true;
                buttonUnselectAll.Enabled = true;
                comboBoxPrinterList.Enabled = true;
                listBoxPageList.Enabled = true;

                foreach (Control p in panelPrintForm.Controls)
                {
                    p.Top = 0;
                    p.Left = 0;
                }

                panelPrintForm.Controls.Clear();

                panelPrintForm.Visible = false;
                printerPreview.Visible = true;

                listBoxPageList.SelectedIndex = index;
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            buttonChange.Checked = false;
            buttonCancel.Enabled = false;

            if (pages.Count == 0) return;

            int index = 0;
            if (0 <= listBoxPageList.SelectedIndex) index = listBoxPageList.SelectedIndex;
            BL_ReportView_Base page = (BL_ReportView_Base)pages[index];
            page.Cancel_ReportTextBox();

            PrintMe(pages.ToArray(), Options, false, false);

            buttonCancel.Enabled = false;
            buttonChange.Text = "編集";
            buttonPrint.Enabled = true;
            buttonSetting.Enabled = true;
            buttonPrinterSetting.Enabled = true;
            comboBoxPrinterList.Enabled = true;
            buttonSelectAll.Enabled = true;
            buttonUnselectAll.Enabled = true;
            listBoxPageList.Enabled = true;

            foreach (Control p in panelPrintForm.Controls)
            {
                p.Top = 0;
                p.Left = 0;
            }

            panelPrintForm.Controls.Clear();

            panelPrintForm.Visible = false;
            printerPreview.Visible = true;

            listBoxPageList.SelectedIndex = index;
        }

        private void buttonPrinterSetting_Click(object sender, EventArgs e)
        {
            if (Document != null)
            {
                PageSetupDialog dlg = new PageSetupDialog();
                dlg.Document = Document;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    this.OnSelectPage(sender, e);		// プレビューしなおし
                }
            }
        }

        private void buttonSelectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBoxPageList.Items.Count; i++)
            {
                listBoxPageList.SetItemCheckState(i, CheckState.Checked);
            }
            buttonPrint.Enabled = true;
        }

        private void buttonUnselectAll_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < listBoxPageList.Items.Count; i++)
            {
                listBoxPageList.SetItemCheckState(i, CheckState.Unchecked);
            }
            buttonPrint.Enabled = false;
        }

        /// <summary>
        /// マウスクリック時の動作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printerPreview_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                printerPreview.Zoom += 0.1;
            }
            else if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (0.1 < printerPreview.Zoom) printerPreview.Zoom -= 0.1;
            }
        }

        #endregion

        /// <summary>
        /// 印刷ページリスト
        /// </summary>
        protected List<BL_ReportView_Base> pages = new List<BL_ReportView_Base>();

        /// <summary>
        /// 初期化済みフラグ
        /// </summary>
        protected bool initialized = false;
        
        /// <summary>
        /// 初期化済みフラグを取得・設定します。
        /// </summary>
        public bool Initialized { get { return initialized; } set { initialized = value; } }

        /// <summary>データ抽出条件群</summary>
        protected object[] print_parameters = null;

        /// <summary>
        /// レポート初期化
        /// </summary>
        public virtual void Report_Initialize(object[] print_parameters, PrintOptions print_option)
        {
            if (buttonChange.Checked)
            {
                Report_EditCancel();
            }

            pages.Clear();

            buttonChange.Enabled = false;
            buttonCancel.Enabled = false;
            buttonPrint.Enabled = false;
            buttonSetting.Enabled = false;

            Report_FetchData(print_parameters);

            if (print_option == null) print_option = new PrintOptions();
            Report_Preview(print_option);

            foreach (BL_ReportView_Base page in pages)
            {
                page.Readed = false;

                foreach (Control ctr in page.Controls)
                {
                    if (typeof(BL_ReportLabel).IsInstanceOfType(ctr))
                    {
                        ((BL_ReportLabel)ctr).Initialize();
                    }
                }
            }
        }

        /// <summary>
        /// 複数ページの印刷データを取得します。
        /// </summary>
        /// <param name="print_parameters"></param>
        protected virtual bool Report_FetchData(object[] print_parameters)
        {
            if (print_parameters != null)
            {
                this.print_parameters = (object[])print_parameters.Clone();
            }
            else
            {
                this.print_parameters = null;
            }

            for (int pageno = 0; pageno < pages.Count; pageno++)
            {
                BL_ReportView_Base page = pages[pageno];
                page.Report_PageNo = pageno + 1;

                int pagecnt = page.Duplicate();
                pageno += pagecnt;
            }

            return true;
        }

        /// <summary>
        /// レポート表示更新
        /// </summary>
        public virtual bool Report_Preview(PrintOptions print_options)
        {
            if (buttonChange.Checked) return false;
            
            if (0 < pages.Count)
            {
                buttonChange.Enabled = true;
                buttonCancel.Enabled = false;
                buttonPrint.Enabled = true;
                buttonSetting.Enabled = true;
            }

            return PrintMe(pages.ToArray(), print_options, false, false);
        }

        /// <summary>
        /// 先頭ページをプレビューします
        /// </summary>
        /// <returns></returns>
        public bool Report_PreviewTopPage()
        {
            if (buttonChange.Checked) return false;

            if (0 < listBoxPageList.Items.Count)
            {
                listBoxPageList.SelectedIndex = 0;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 最終ページをプレビューします
        /// </summary>
        /// <returns></returns>
        public bool Report_PreviewLastPage()
        {
            if (buttonChange.Checked) return false;

            if (0 < listBoxPageList.Items.Count)
            {
                listBoxPageList.SelectedIndex = listBoxPageList.Items.Count - 1;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 次ページをプレビューします
        /// </summary>
        /// <returns></returns>
        public bool Report_PreviewNextPage()
        {
            if (buttonChange.Checked) return false;

            if (listBoxPageList.SelectedIndex + 1 < listBoxPageList.Items.Count)
            {
                listBoxPageList.SelectedIndex++;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 前ページをプレビューします
        /// </summary>
        /// <returns></returns>
        public bool Report_PreviewPreviousPage()
        {
            if (buttonChange.Checked) return false;

            if (0 <= listBoxPageList.SelectedIndex - 1)
            {
                listBoxPageList.SelectedIndex--;
                return true;
            }

            return false;
        }

        /// <summary>
        /// 印刷します
        /// </summary>
        public void Report_Printout()
        {
            Report_Printout(null);
        }

        /// <summary>
        /// 印刷します
        /// </summary>
        /// <param name="print_options"></param>
        public void Report_Printout(PrintOptions print_options)
        {
            if (buttonPrint.Enabled)
            {
                if (print_options != null)
                {
                    Options = PrintOptions.Duplicate(print_options);
                }

                buttonPrint_Click(buttonPrint, new EventArgs());
            }
        }

        /// <summary>
        /// 編集開始します
        /// </summary>
        public void Report_EditStart()
        {
            if (!buttonChange.Checked)
            {
                buttonChange.Checked = true;
                buttonChange_Click(buttonChange, new EventArgs());
            }
        }

        /// <summary>
        /// 編集終了します
        /// </summary>
        public void Report_EditEnd()
        {
            if (buttonChange.Checked)
            {
                buttonChange.Checked = false;
                buttonChange_Click(buttonChange, new EventArgs());
            }
        }

        /// <summary>
        /// 編集キャンセルします
        /// </summary>
        public void Report_EditCancel()
        {
            if (buttonCancel.Enabled)
            {
                buttonCancel_Click(buttonCancel, new EventArgs());
            }
        }

        /// <summary>
        /// ページを挿入します
        /// </summary>
        /// <param name="pageno"></param>
        /// <param name="source"></param>
        public void InsertPage(int pageno, BL_ReportView_Base source)
        {
            pages.Insert(pageno, source);
        }
    }
}
