using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace BelicsClass.UI.Report
{
    /// <summary>
    /// レポート用のラベルクラス
    /// </summary>
    public class BL_ReportLabel : Label
    {
        /// <summary>
        /// 編集時テキストボックス
        /// </summary>
        public BL_ReportTextBox textBox = null;

        private string initialText = null;

        /// <summary>
        /// 編集種別
        /// </summary>
        public enum _DataKind
        {
            /// <summary>編集不可</summary>
            Label,
            /// <summary>任意編集</summary>
            Free,
            /// <summary>フォーマット編集(未実装)</summary>
            Data,
        }

        /// <summary>
        /// 編集フォーマット種別
        /// </summary>
        public enum _FormatKind
        {
            /// <summary>任意</summary>
            Any,
            /// <summary>±整数</summary>
            Integer,
            /// <summary>＋整数</summary>
            PositiveInteger,
            /// <summary>－整数</summary>
            NegativeInteger,
            /// <summary>±数値</summary>
            Decimal,
            /// <summary>＋数値</summary>
            PositiveDecimal,
            /// <summary>－数値</summary>
            NegativeDecimal,
            /// <summary>日時</summary>
            DateTime,
            /// <summary>日付</summary>
            Date,
            /// <summary>時刻</summary>
            Time,
            /// <summary>年</summary>
            Year,
            /// <summary>月</summary>
            Month,
            /// <summary>日</summary>
            Day,
            /// <summary>時</summary>
            Hour,
            /// <summary>分</summary>
            Minutes,
            /// <summary>秒</summary>
            Seconds,
        }

        /// <summary>
        /// 初期データ
        /// </summary>
        public string InitialText
        {
            get { return initialText; }
            set { initialText = value; }
        }

        /// <summary>
        /// 複数行編集の可不可
        /// </summary>
        public bool Multiline { get; set; }

        /// <summary>
        /// フォーマット種別
        /// </summary>
        public _FormatKind FormatKind { get; set; }

        /// <summary>
        /// 編集種別
        /// </summary>
        public _DataKind DataKind { get; set; }

        /// <summary>
        /// データ変更時の表示色
        /// </summary>
        public Color ChangedColor { get; set; }

        /// <summary>
        /// データ通常時の表示色
        /// </summary>
        public Color NormalColor { get; set; }

        /// <summary>
        /// 不正データ入力時の表示色
        /// </summary>
        public Color ErrorColor { get; set; }

            /// <summary>
        /// コンストラクタ
        /// 編集不可・変更時青・エラー時赤・初期データnull で初期化されます
        /// </summary>
        public BL_ReportLabel()
            : base()
        {
            FormatKind = _FormatKind.Any;
            DataKind = _DataKind.Label;
            ChangedColor = Color.Blue;
            ErrorColor = Color.Red;

            Initialize();
        }

        ///// <summary>
        ///// テキストが変更された時の処理です
        ///// </summary>
        ///// <param name="e"></param>
        //protected override void OnTextChanged(EventArgs e)
        //{
        //    if (DesignMode) return;

        //    if (DataKind != _DataKind.Label)
        //    {
        //        if (textBox != null) textBox.Text = Text;

        //        if (InitialText == null)
        //        {
        //            if (this.Parent != null)
        //            {
        //                ForeColor = NormalColor;
        //                InitialText = Text;
        //            }
        //        }
        //        else if (InitialText.Trim() != Text.Trim())
        //        {
        //            ForeColor = ChangedColor;
        //        }
        //        else
        //        {
        //            ForeColor = NormalColor;
        //        }
        //    }

        //    base.OnTextChanged(e);
        //}

        /// <summary>
        /// 初期化処理
        /// 初期データnull・表示色コントロール標準で初期化されます
        /// </summary>
        public virtual void Initialize()
        {
            InitialText = null;
            ForeColor = NormalColor;
        }
    }
}
