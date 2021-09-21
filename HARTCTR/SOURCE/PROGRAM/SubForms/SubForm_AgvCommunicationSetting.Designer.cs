namespace PROGRAM
{
    partial class SubForm_AgvCommunicationSetter
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxHostPort = new System.Windows.Forms.TextBox();
            this.textBoxClientPort = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.dataGridView1 = new BelicsClass.UI.Controls.BL_DataGridViewPlus();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(130, 28);
            this.label5.TabIndex = 1;
            this.label5.Text = "受信元ポート";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(278, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(130, 28);
            this.label6.TabIndex = 1;
            this.label6.Text = "送信元ポート";
            // 
            // textBoxHostPort
            // 
            this.textBoxHostPort.Location = new System.Drawing.Point(148, 39);
            this.textBoxHostPort.Name = "textBoxHostPort";
            this.textBoxHostPort.Size = new System.Drawing.Size(90, 35);
            this.textBoxHostPort.TabIndex = 2;
            this.textBoxHostPort.Text = "9200";
            this.textBoxHostPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxClientPort
            // 
            this.textBoxClientPort.Location = new System.Drawing.Point(414, 39);
            this.textBoxClientPort.Name = "textBoxClientPort";
            this.textBoxClientPort.Size = new System.Drawing.Size(90, 35);
            this.textBoxClientPort.TabIndex = 2;
            this.textBoxClientPort.Text = "9300";
            this.textBoxClientPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label7.Location = new System.Drawing.Point(12, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(215, 28);
            this.label7.TabIndex = 1;
            this.label7.Text = "AGV⇒PC【このポート】";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label8.Location = new System.Drawing.Point(278, 9);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(215, 28);
            this.label8.TabIndex = 1;
            this.label8.Text = "PC【このポート】⇒AGV";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label9.Location = new System.Drawing.Point(777, 49);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(215, 28);
            this.label9.TabIndex = 1;
            this.label9.Text = "AGV【このポート】⇒PC";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            this.label10.Location = new System.Drawing.Point(547, 49);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(215, 28);
            this.label10.TabIndex = 1;
            this.label10.Text = "PC⇒AGV【このポート】";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowDrop = true;
            this.dataGridView1.CellVisibleFix = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Yellow;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Meiryo UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(12, 80);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowIndexVisible = true;
            this.dataGridView1.RowTemplate.Height = 32;
            this.dataGridView1.ShowCellErrors = false;
            this.dataGridView1.ShowCellToolTips = false;
            this.dataGridView1.ShowRowErrors = false;
            this.dataGridView1.Size = new System.Drawing.Size(998, 594);
            this.dataGridView1.TabIndex = 3;
            this.dataGridView1.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_CellBeginEdit);
            this.dataGridView1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellEndEdit);
            this.dataGridView1.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.dataGridView1_CellValidating);
            this.dataGridView1.RowValidating += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridView1_RowValidating);
            // 
            // SubForm_A7100
            // 
            this._TitleString = "AGV通信設定";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1022, 686);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.textBoxClientPort);
            this.Controls.Add(this.textBoxHostPort);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Font = new System.Drawing.Font("Meiryo UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "SubForm_A7100";
            this.Text = "AGV通信設定";
            this.TitleText = "AGV通信設定";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxHostPort;
        private System.Windows.Forms.TextBox textBoxClientPort;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private BelicsClass.UI.Controls.BL_DataGridViewPlus dataGridView1;
    }
}