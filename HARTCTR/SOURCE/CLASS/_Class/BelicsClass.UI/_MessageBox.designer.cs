namespace BelicsClass.UI
{
    partial class BL_MessageBox
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
            this.buttonCANCEL = new System.Windows.Forms.Button();
            this.buttonYES = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.labelMessage = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.buttonCANCEL);
            this.panel1.Controls.Add(this.buttonYES);
            this.panel1.Controls.Add(this.buttonOK);
            this.panel1.Controls.Add(this.labelMessage);
            this.panel1.Location = new System.Drawing.Point(13, 82);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Size = new System.Drawing.Size(694, 263);
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(12, 12);
            this.panel2.Size = new System.Drawing.Size(695, 56);
            // 
            // labelTitle
            // 
            this.labelTitle.Size = new System.Drawing.Size(671, 27);
            // 
            // buttonCANCEL
            // 
            this.buttonCANCEL.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCANCEL.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonCANCEL.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonCANCEL.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonCANCEL.Location = new System.Drawing.Point(442, 195);
            this.buttonCANCEL.Margin = new System.Windows.Forms.Padding(4);
            this.buttonCANCEL.Name = "buttonCANCEL";
            this.buttonCANCEL.Size = new System.Drawing.Size(172, 54);
            this.buttonCANCEL.TabIndex = 3;
            this.buttonCANCEL.Text = "@button1";
            this.buttonCANCEL.UseVisualStyleBackColor = false;
            this.buttonCANCEL.Visible = false;
            this.buttonCANCEL.Click += new System.EventHandler(this.buttonCANCEL_Click);
            // 
            // buttonYES
            // 
            this.buttonYES.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonYES.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonYES.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonYES.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonYES.Location = new System.Drawing.Point(82, 195);
            this.buttonYES.Margin = new System.Windows.Forms.Padding(4);
            this.buttonYES.Name = "buttonYES";
            this.buttonYES.Size = new System.Drawing.Size(172, 54);
            this.buttonYES.TabIndex = 1;
            this.buttonYES.Text = "@button1";
            this.buttonYES.UseVisualStyleBackColor = false;
            this.buttonYES.Visible = false;
            this.buttonYES.Click += new System.EventHandler(this.buttonYES_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonOK.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.buttonOK.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonOK.ForeColor = System.Drawing.SystemColors.ControlText;
            this.buttonOK.Location = new System.Drawing.Point(262, 195);
            this.buttonOK.Margin = new System.Windows.Forms.Padding(4);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(172, 54);
            this.buttonOK.TabIndex = 2;
            this.buttonOK.Text = "@button1";
            this.buttonOK.UseVisualStyleBackColor = false;
            this.buttonOK.Visible = false;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // labelMessage
            // 
            this.labelMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelMessage.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelMessage.ForeColor = System.Drawing.Color.Black;
            this.labelMessage.Location = new System.Drawing.Point(12, 17);
            this.labelMessage.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelMessage.Name = "labelMessage";
            this.labelMessage.Size = new System.Drawing.Size(670, 174);
            this.labelMessage.TabIndex = 2;
            this.labelMessage.Text = "メッセージ内容";
            // 
            // progressBar1
            // 
            this.progressBar1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.progressBar1.ForeColor = System.Drawing.Color.Lime;
            this.progressBar1.Location = new System.Drawing.Point(13, 68);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(694, 14);
            this.progressBar1.Step = 1;
            this.progressBar1.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.progressBar1.TabIndex = 4;
            this.progressBar1.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(699, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(0, 0);
            this.button1.TabIndex = 0;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // BL_MessageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.ClientSize = new System.Drawing.Size(720, 357);
            this.Controls.Add(this.progressBar1);
            this.Name = "BL_MessageBox";
            this.ShowInTaskbar = true;
            this.Text = "";
            this.TitleText = "";
            this.Controls.SetChildIndex(this.panel1, 0);
            this.Controls.SetChildIndex(this.panel2, 0);
            this.Controls.SetChildIndex(this.progressBar1, 0);
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary></summary>
        public System.Windows.Forms.Label labelMessage;
        private System.Windows.Forms.Button button1;

        /// <summary></summary>
        protected System.Windows.Forms.ProgressBar progressBar1;
        /// <summary></summary>
        protected System.Windows.Forms.Button buttonCANCEL;
        /// <summary></summary>
        protected System.Windows.Forms.Button buttonYES;
        /// <summary></summary>
        protected System.Windows.Forms.Button buttonOK;
    }
}
