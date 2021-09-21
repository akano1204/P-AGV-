namespace LogisticAgvCommunicateTester
{
    partial class SubForm_OrderCommander
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
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxSTWait = new System.Windows.Forms.TextBox();
            this.buttonRack = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonAutoChange = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonAutoRack = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonST = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonAutoStComplete = new BelicsClass.UI.Controls.BL_FlatButton();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.textBoxPort = new System.Windows.Forms.TextBox();
            this.buttonConnect = new BelicsClass.UI.Controls.BL_FlatButton();
            this.listBoxRack = new System.Windows.Forms.CheckedListBox();
            this.listBoxStation = new System.Windows.Forms.CheckedListBox();
            this.listBoxRackFace = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Meiryo UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(1007, 514);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(47, 38);
            this.label2.TabIndex = 31;
            this.label2.Text = "秒";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.Font = new System.Drawing.Font("Meiryo UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(754, 513);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(183, 38);
            this.label1.TabIndex = 32;
            this.label1.Text = "ST待機時間";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxSTWait
            // 
            this.textBoxSTWait.Location = new System.Drawing.Point(943, 506);
            this.textBoxSTWait.Name = "textBoxSTWait";
            this.textBoxSTWait.Size = new System.Drawing.Size(58, 45);
            this.textBoxSTWait.TabIndex = 30;
            this.textBoxSTWait.Text = "5";
            this.textBoxSTWait.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // buttonRack
            // 
            this.buttonRack.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonRack.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonRack.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonRack.BackColorON = System.Drawing.Color.Lime;
            this.buttonRack.Checked = false;
            this.buttonRack.CheckMode = false;
            this.buttonRack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonRack.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonRack.FlatAppearance.BorderSize = 2;
            this.buttonRack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRack.Font = new System.Drawing.Font("Meiryo UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonRack.ForeColor = System.Drawing.Color.White;
            this.buttonRack.ForeColorNormal = System.Drawing.Color.White;
            this.buttonRack.ForeColorOFF = System.Drawing.Color.White;
            this.buttonRack.ForeColorON = System.Drawing.Color.Black;
            this.buttonRack.Location = new System.Drawing.Point(12, 557);
            this.buttonRack.Name = "buttonRack";
            this.buttonRack.Size = new System.Drawing.Size(179, 57);
            this.buttonRack.TabIndex = 25;
            this.buttonRack.Tag = false;
            this.buttonRack.Text = "棚要求";
            this.buttonRack.UseVisualStyleBackColor = false;
            this.buttonRack.Click += new System.EventHandler(this.buttonRack_Click);
            // 
            // buttonAutoChange
            // 
            this.buttonAutoChange.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonAutoChange.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonAutoChange.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonAutoChange.BackColorON = System.Drawing.Color.Lime;
            this.buttonAutoChange.Checked = false;
            this.buttonAutoChange.CheckMode = true;
            this.buttonAutoChange.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonAutoChange.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonAutoChange.FlatAppearance.BorderSize = 2;
            this.buttonAutoChange.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAutoChange.Font = new System.Drawing.Font("Meiryo UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonAutoChange.ForeColor = System.Drawing.Color.White;
            this.buttonAutoChange.ForeColorNormal = System.Drawing.Color.White;
            this.buttonAutoChange.ForeColorOFF = System.Drawing.Color.White;
            this.buttonAutoChange.ForeColorON = System.Drawing.Color.Black;
            this.buttonAutoChange.Location = new System.Drawing.Point(301, 557);
            this.buttonAutoChange.Name = "buttonAutoChange";
            this.buttonAutoChange.Size = new System.Drawing.Size(412, 57);
            this.buttonAutoChange.TabIndex = 26;
            this.buttonAutoChange.Tag = false;
            this.buttonAutoChange.Text = "順次切り替え";
            this.buttonAutoChange.UseVisualStyleBackColor = false;
            // 
            // buttonAutoRack
            // 
            this.buttonAutoRack.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonAutoRack.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonAutoRack.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonAutoRack.BackColorON = System.Drawing.Color.Lime;
            this.buttonAutoRack.Checked = false;
            this.buttonAutoRack.CheckMode = true;
            this.buttonAutoRack.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonAutoRack.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonAutoRack.FlatAppearance.BorderSize = 2;
            this.buttonAutoRack.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAutoRack.Font = new System.Drawing.Font("Meiryo UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonAutoRack.ForeColor = System.Drawing.Color.White;
            this.buttonAutoRack.ForeColorNormal = System.Drawing.Color.White;
            this.buttonAutoRack.ForeColorOFF = System.Drawing.Color.White;
            this.buttonAutoRack.ForeColorON = System.Drawing.Color.Black;
            this.buttonAutoRack.Location = new System.Drawing.Point(197, 557);
            this.buttonAutoRack.Name = "buttonAutoRack";
            this.buttonAutoRack.Size = new System.Drawing.Size(98, 57);
            this.buttonAutoRack.TabIndex = 27;
            this.buttonAutoRack.Tag = false;
            this.buttonAutoRack.Text = "自動";
            this.buttonAutoRack.UseVisualStyleBackColor = false;
            // 
            // buttonST
            // 
            this.buttonST.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonST.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonST.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonST.BackColorON = System.Drawing.Color.Lime;
            this.buttonST.Checked = false;
            this.buttonST.CheckMode = false;
            this.buttonST.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonST.Enabled = false;
            this.buttonST.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonST.FlatAppearance.BorderSize = 2;
            this.buttonST.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonST.Font = new System.Drawing.Font("Meiryo UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonST.ForeColor = System.Drawing.Color.White;
            this.buttonST.ForeColorNormal = System.Drawing.Color.White;
            this.buttonST.ForeColorOFF = System.Drawing.Color.White;
            this.buttonST.ForeColorON = System.Drawing.Color.Black;
            this.buttonST.Location = new System.Drawing.Point(719, 557);
            this.buttonST.Name = "buttonST";
            this.buttonST.Size = new System.Drawing.Size(229, 57);
            this.buttonST.TabIndex = 28;
            this.buttonST.Tag = false;
            this.buttonST.Text = "ＳＴ完了";
            this.buttonST.UseVisualStyleBackColor = false;
            this.buttonST.Click += new System.EventHandler(this.buttonST_Click);
            // 
            // buttonAutoStComplete
            // 
            this.buttonAutoStComplete.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonAutoStComplete.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonAutoStComplete.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonAutoStComplete.BackColorON = System.Drawing.Color.Lime;
            this.buttonAutoStComplete.Checked = false;
            this.buttonAutoStComplete.CheckMode = true;
            this.buttonAutoStComplete.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonAutoStComplete.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonAutoStComplete.FlatAppearance.BorderSize = 2;
            this.buttonAutoStComplete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAutoStComplete.Font = new System.Drawing.Font("Meiryo UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonAutoStComplete.ForeColor = System.Drawing.Color.White;
            this.buttonAutoStComplete.ForeColorNormal = System.Drawing.Color.White;
            this.buttonAutoStComplete.ForeColorOFF = System.Drawing.Color.White;
            this.buttonAutoStComplete.ForeColorON = System.Drawing.Color.Black;
            this.buttonAutoStComplete.Location = new System.Drawing.Point(954, 557);
            this.buttonAutoStComplete.Name = "buttonAutoStComplete";
            this.buttonAutoStComplete.Size = new System.Drawing.Size(100, 57);
            this.buttonAutoStComplete.TabIndex = 29;
            this.buttonAutoStComplete.Tag = false;
            this.buttonAutoStComplete.Text = "自動";
            this.buttonAutoStComplete.UseVisualStyleBackColor = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Meiryo UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label10.Location = new System.Drawing.Point(12, 72);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(167, 38);
            this.label10.TabIndex = 21;
            this.label10.Text = "搬送対象棚";
            // 
            // label11
            // 
            this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Meiryo UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label11.Location = new System.Drawing.Point(722, 72);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(145, 38);
            this.label11.TabIndex = 20;
            this.label11.Text = "搬送先ST";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(136, 7);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(232, 45);
            this.textBoxIP.TabIndex = 30;
            this.textBoxIP.Text = "127.0.0.1";
            this.textBoxIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.Font = new System.Drawing.Font("Meiryo UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(12, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(118, 38);
            this.label3.TabIndex = 32;
            this.label3.Text = "接続IP";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label4.Font = new System.Drawing.Font("Meiryo UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(374, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(158, 38);
            this.label4.TabIndex = 32;
            this.label4.Text = "接続ポート";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // textBoxPort
            // 
            this.textBoxPort.Location = new System.Drawing.Point(538, 7);
            this.textBoxPort.Name = "textBoxPort";
            this.textBoxPort.Size = new System.Drawing.Size(106, 45);
            this.textBoxPort.TabIndex = 30;
            this.textBoxPort.Text = "12000";
            this.textBoxPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // buttonConnect
            // 
            this.buttonConnect.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonConnect.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonConnect.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonConnect.BackColorON = System.Drawing.Color.Lime;
            this.buttonConnect.Checked = false;
            this.buttonConnect.CheckMode = true;
            this.buttonConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonConnect.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonConnect.FlatAppearance.BorderSize = 2;
            this.buttonConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonConnect.Font = new System.Drawing.Font("Meiryo UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonConnect.ForeColor = System.Drawing.Color.White;
            this.buttonConnect.ForeColorNormal = System.Drawing.Color.White;
            this.buttonConnect.ForeColorOFF = System.Drawing.Color.White;
            this.buttonConnect.ForeColorON = System.Drawing.Color.Black;
            this.buttonConnect.Location = new System.Drawing.Point(892, 7);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(162, 45);
            this.buttonConnect.TabIndex = 25;
            this.buttonConnect.Tag = false;
            this.buttonConnect.Text = "接続";
            this.buttonConnect.UseVisualStyleBackColor = false;
            // 
            // listBoxRack
            // 
            this.listBoxRack.FormattingEnabled = true;
            this.listBoxRack.IntegralHeight = false;
            this.listBoxRack.Location = new System.Drawing.Point(19, 114);
            this.listBoxRack.Name = "listBoxRack";
            this.listBoxRack.Size = new System.Drawing.Size(276, 386);
            this.listBoxRack.TabIndex = 33;
            this.listBoxRack.SelectedIndexChanged += new System.EventHandler(this.listBoxRack_SelectedIndexChanged);
            // 
            // listBoxStation
            // 
            this.listBoxStation.FormattingEnabled = true;
            this.listBoxStation.IntegralHeight = false;
            this.listBoxStation.Location = new System.Drawing.Point(726, 113);
            this.listBoxStation.Name = "listBoxStation";
            this.listBoxStation.Size = new System.Drawing.Size(328, 387);
            this.listBoxStation.TabIndex = 33;
            // 
            // listBoxRackFace
            // 
            this.listBoxRackFace.FormattingEnabled = true;
            this.listBoxRackFace.ItemHeight = 37;
            this.listBoxRackFace.Location = new System.Drawing.Point(302, 114);
            this.listBoxRackFace.Name = "listBoxRackFace";
            this.listBoxRackFace.Size = new System.Drawing.Size(199, 226);
            this.listBoxRackFace.TabIndex = 34;
            // 
            // SubForm_OrderCommander
            // 
            this._TitleString = "上位通信模擬";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1066, 626);
            this.Controls.Add(this.listBoxRackFace);
            this.Controls.Add(this.listBoxStation);
            this.Controls.Add(this.listBoxRack);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxPort);
            this.Controls.Add(this.textBoxIP);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxSTWait);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.buttonRack);
            this.Controls.Add(this.buttonAutoChange);
            this.Controls.Add(this.buttonAutoRack);
            this.Controls.Add(this.buttonST);
            this.Controls.Add(this.buttonAutoStComplete);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Font = new System.Drawing.Font("Meiryo UI", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "SubForm_OrderCommander";
            this.Text = "上位通信模擬";
            this.TitleText = "上位通信模擬";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxSTWait;
        private BelicsClass.UI.Controls.BL_FlatButton buttonRack;
        private BelicsClass.UI.Controls.BL_FlatButton buttonAutoChange;
        private BelicsClass.UI.Controls.BL_FlatButton buttonAutoRack;
        private BelicsClass.UI.Controls.BL_FlatButton buttonST;
        private BelicsClass.UI.Controls.BL_FlatButton buttonAutoStComplete;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxPort;
        private BelicsClass.UI.Controls.BL_FlatButton buttonConnect;
        private System.Windows.Forms.CheckedListBox listBoxRack;
        private System.Windows.Forms.CheckedListBox listBoxStation;
        private System.Windows.Forms.ListBox listBoxRackFace;
    }
}