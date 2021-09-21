namespace BelicsClass.UI
{
    partial class BL_MessageBox_Base
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.labelTitle = new System.Windows.Forms.Label();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.SystemColors.Control;
            this.panel1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.panel1.Location = new System.Drawing.Point(14, 73);
            this.panel1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(608, 194);
            this.panel1.TabIndex = 0;
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // panel2
            // 
            this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel2.BackColor = System.Drawing.SystemColors.HotTrack;
            this.panel2.Controls.Add(this.labelTitle);
            this.panel2.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.panel2.Location = new System.Drawing.Point(14, 15);
            this.panel2.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(608, 58);
            this.panel2.TabIndex = 0;
            this.panel2.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel2_MouseMove);
            // 
            // labelTitle
            // 
            this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTitle.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelTitle.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.labelTitle.Location = new System.Drawing.Point(11, 16);
            this.labelTitle.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(584, 27);
            this.labelTitle.TabIndex = 0;
            this.labelTitle.Text = "メッセージタイトル";
            // 
            // BL_MessageBox_Base
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Highlight;
            this.ClientSize = new System.Drawing.Size(637, 283);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Meiryo UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "BL_MessageBox_Base";
            this.ShowInTaskbar = true;
            this.Load += new System.EventHandler(this.MessageBox_Base_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MessageBox_Base_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MessageBox_Base_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MessageBox_Base_KeyUp);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.MessageBox_Base_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.MessageBox_Base_MouseMove);
            this.panel2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary></summary>
        protected System.Windows.Forms.Panel panel1;
        /// <summary></summary>
        protected System.Windows.Forms.Panel panel2;
        /// <summary></summary>
        public System.Windows.Forms.Label labelTitle;

    }
}