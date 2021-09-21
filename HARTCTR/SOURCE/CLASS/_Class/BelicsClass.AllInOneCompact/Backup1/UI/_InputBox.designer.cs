namespace BelicsClass.UI
{
    partial class BL_InputBox
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // labelMessage
            // 
            this.labelMessage.Size = new System.Drawing.Size(670, 119);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textBoxInput);
            this.panel1.Controls.SetChildIndex(this.textBoxInput, 0);
            this.panel1.Controls.SetChildIndex(this.labelMessage, 0);
            this.panel1.Controls.SetChildIndex(this.buttonOK, 0);
            this.panel1.Controls.SetChildIndex(this.buttonYES, 0);
            this.panel1.Controls.SetChildIndex(this.buttonCANCEL, 0);
            // 
            // textBoxInput
            // 
            this.textBoxInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxInput.Font = new System.Drawing.Font("Meiryo UI", 24.25F);
            this.textBoxInput.Location = new System.Drawing.Point(14, 139);
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(667, 49);
            this.textBoxInput.TabIndex = 0;
            // 
            // BL_InputBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.ClientSize = new System.Drawing.Size(720, 357);
            this.Name = "BL_InputBox";
            this.ProgressBarVisible = true;
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public System.Windows.Forms.TextBox textBoxInput;

    }
}
