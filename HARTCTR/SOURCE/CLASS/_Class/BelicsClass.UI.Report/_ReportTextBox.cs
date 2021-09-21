using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace BelicsClass.UI.Report
{
    /// <summary>
    /// レポート用の編集テキストクラス
    /// </summary>
    public class BL_ReportTextBox : TextBox
    {
        private BL_ReportLabel linkLabel;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_ReportTextBox()
            : base()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            do
            {
                if (linkLabel == null) break;

                if (e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Enter || e.KeyChar == (char)Keys.Delete) break;
                //if (e.KeyChar == (char)Keys.Back || e.KeyChar == (char)Keys.Enter) break;

                switch (linkLabel.FormatKind)
                {
                    case BL_ReportLabel._FormatKind.Any:
                        break;

                    case BL_ReportLabel._FormatKind.Integer:
                        if (('0' <= e.KeyChar && e.KeyChar <= '9') || e.KeyChar == '-') { }
                        else e.Handled = true;
                        break;

                    case BL_ReportLabel._FormatKind.PositiveInteger:
                        if ('0' <= e.KeyChar && e.KeyChar <= '9') { }
                        else e.Handled = true;
                        break;

                    case BL_ReportLabel._FormatKind.NegativeInteger:
                        if ('0' <= e.KeyChar && e.KeyChar <= '9' || e.KeyChar == '-') { }
                        else e.Handled = true;
                        break;

                    case BL_ReportLabel._FormatKind.Decimal:
                        if (('0' <= e.KeyChar && e.KeyChar <= '9') || e.KeyChar == '-' || e.KeyChar == '.') { }
                        else e.Handled = true;
                        break;

                    case BL_ReportLabel._FormatKind.PositiveDecimal:
                        if (('0' <= e.KeyChar && e.KeyChar <= '9') || e.KeyChar == '.') { }
                        else e.Handled = true;
                        break;

                    case BL_ReportLabel._FormatKind.NegativeDecimal:
                        if (('0' <= e.KeyChar && e.KeyChar <= '9') || e.KeyChar == '-' || e.KeyChar == '.') { }
                        else e.Handled = true;
                        break;

                    case BL_ReportLabel._FormatKind.Time:
                        if (('0' <= e.KeyChar && e.KeyChar <= '9') || e.KeyChar == ':') { }
                        else e.Handled = true;
                        break;

                }
            }
            while (false);

            if (!e.Handled)
            {
                //this.ForeColor = Color.Blue;
                base.OnKeyPress(e);
            }
        }

        /// <summary>
        /// 初期化処理
        /// 引数は、BL_ReportLabelからの派生クラスオブジェクトでなければなりません
        /// </summary>
        /// <param name="label">変更内容を反映させる対象となるラベルコントロール</param>
        public void Initialize(Control label)
        {
            if (!typeof(BL_ReportLabel).IsInstanceOfType(label)) return;

            linkLabel = (BL_ReportLabel)label;

            if (linkLabel.InitialText == null)
            {
                linkLabel.ForeColor = linkLabel.NormalColor;

                //if (linkLabel.Text != "")
                {
                    linkLabel.InitialText = linkLabel.Text;
                }
            }

            linkLabel.textBox = this;

            this.Text = linkLabel.Text;
            this.Multiline = linkLabel.Multiline;

            this.Font = linkLabel.Font;
            this.Name = "lltb_" + linkLabel.Name;
            this.TabIndex = linkLabel.TabIndex;

            this.Location = new System.Drawing.Point(linkLabel.Location.X, linkLabel.Location.Y);
            this.Size = new System.Drawing.Size(linkLabel.Size.Width, linkLabel.Size.Height);
            
            if (this.Height != linkLabel.Height)
            {
                this.Top = linkLabel.Top + linkLabel.Height / 2 - this.Height / 2;
            }

            if ((linkLabel.TextAlign == ContentAlignment.TopLeft) || (linkLabel.TextAlign == ContentAlignment.MiddleLeft) || (linkLabel.TextAlign == ContentAlignment.BottomLeft))
            {
                this.TextAlign = HorizontalAlignment.Left;
            }
            else if ((linkLabel.TextAlign == ContentAlignment.TopCenter) || (linkLabel.TextAlign == ContentAlignment.MiddleCenter) || (linkLabel.TextAlign == ContentAlignment.BottomCenter))
            {
                this.TextAlign = HorizontalAlignment.Center;
            }
            else if ((linkLabel.TextAlign == ContentAlignment.TopRight) || (linkLabel.TextAlign == ContentAlignment.MiddleRight) || (linkLabel.TextAlign == ContentAlignment.BottomRight))
            {
                this.TextAlign = HorizontalAlignment.Right;
            }

            if (linkLabel.DataKind == BL_ReportLabel._DataKind.Free)
            {
                this.Visible = true;
                this.BackColor = Color.LemonChiffon;
            }
            else if (linkLabel.DataKind == BL_ReportLabel._DataKind.Data)
            {
                this.Visible = true;
                this.BackColor = Color.PowderBlue;
            }
            else
            {
                this.Visible = false;
            }
        }

        /// <summary>
        /// 編集内容を確定して、ラベルコントロールを更新します
        /// </summary>
        public void UpdateLabel()
        {
            if (linkLabel == null) return;

            if (linkLabel.Text.Trim() == this.Text.Trim())
            {
                linkLabel.ForeColor = linkLabel.NormalColor;
            }
            else
            {
                linkLabel.ForeColor = linkLabel.ChangedColor;
            }

            linkLabel.Text = this.Text;
            linkLabel.InitialText = this.Text;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTextChanged(EventArgs e)
        {
            if (linkLabel.Text.Trim() == this.Text.Trim())
            {
                this.ForeColor = linkLabel.NormalColor;
            }
            else
            {
                this.ForeColor = linkLabel.ChangedColor;
            }

            base.OnTextChanged(e);
        }
    }
}
