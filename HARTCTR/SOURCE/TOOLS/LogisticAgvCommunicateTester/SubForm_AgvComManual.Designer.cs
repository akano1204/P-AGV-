namespace LogisticAgvCommunicateTester
{
    partial class SubForm_AgvComManual
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxLocalClient = new System.Windows.Forms.TextBox();
            this.textBoxLocalHost = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.buttonConnect = new BelicsClass.UI.Controls.BL_FlatButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxIP = new System.Windows.Forms.TextBox();
            this.textBoxNAME = new System.Windows.Forms.TextBox();
            this.textBoxRemoteClient = new System.Windows.Forms.TextBox();
            this.textBoxRemoteHost = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.labelNAME = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonAgvAdd = new BelicsClass.UI.Controls.BL_FlatButton();
            this.listviewAgv = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader13 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label5 = new System.Windows.Forms.Label();
            this.buttonAgvRemove = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonDetail = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonAgvSim = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonOrderCommander = new BelicsClass.UI.Controls.BL_FlatButton();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textBoxLocalClient);
            this.groupBox2.Controls.Add(this.textBoxLocalHost);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(212, 93);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "PC側";
            // 
            // textBoxLocalClient
            // 
            this.textBoxLocalClient.Location = new System.Drawing.Point(125, 20);
            this.textBoxLocalClient.Name = "textBoxLocalClient";
            this.textBoxLocalClient.Size = new System.Drawing.Size(68, 28);
            this.textBoxLocalClient.TabIndex = 3;
            this.textBoxLocalClient.Text = "9200";
            this.textBoxLocalClient.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxLocalHost
            // 
            this.textBoxLocalHost.Location = new System.Drawing.Point(125, 53);
            this.textBoxLocalHost.Name = "textBoxLocalHost";
            this.textBoxLocalHost.Size = new System.Drawing.Size(68, 28);
            this.textBoxLocalHost.TabIndex = 4;
            this.textBoxLocalHost.Text = "9300";
            this.textBoxLocalHost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 20);
            this.label3.TabIndex = 0;
            this.label3.Text = "CLIENTポート";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(90, 20);
            this.label4.TabIndex = 0;
            this.label4.Text = "HOSTポート";
            // 
            // buttonConnect
            // 
            this.buttonConnect.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonConnect.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonConnect.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonConnect.BackColorON = System.Drawing.Color.Lime;
            this.buttonConnect.Checked = false;
            this.buttonConnect.CheckMode = false;
            this.buttonConnect.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonConnect.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonConnect.FlatAppearance.BorderSize = 2;
            this.buttonConnect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonConnect.ForeColor = System.Drawing.Color.White;
            this.buttonConnect.ForeColorNormal = System.Drawing.Color.White;
            this.buttonConnect.ForeColorOFF = System.Drawing.Color.White;
            this.buttonConnect.ForeColorON = System.Drawing.Color.Black;
            this.buttonConnect.Location = new System.Drawing.Point(230, 22);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(120, 83);
            this.buttonConnect.TabIndex = 6;
            this.buttonConnect.Tag = false;
            this.buttonConnect.Text = "[F1] 接続";
            this.buttonConnect.UseVisualStyleBackColor = false;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBoxIP);
            this.groupBox1.Controls.Add(this.textBoxNAME);
            this.groupBox1.Controls.Add(this.textBoxRemoteClient);
            this.groupBox1.Controls.Add(this.textBoxRemoteHost);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.labelNAME);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(356, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(436, 93);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "AGV側";
            // 
            // textBoxIP
            // 
            this.textBoxIP.Location = new System.Drawing.Point(93, 53);
            this.textBoxIP.Name = "textBoxIP";
            this.textBoxIP.Size = new System.Drawing.Size(129, 28);
            this.textBoxIP.TabIndex = 1;
            this.textBoxIP.Text = "127.0.0.1";
            // 
            // textBoxNAME
            // 
            this.textBoxNAME.Location = new System.Drawing.Point(93, 20);
            this.textBoxNAME.Name = "textBoxNAME";
            this.textBoxNAME.Size = new System.Drawing.Size(129, 28);
            this.textBoxNAME.TabIndex = 1;
            this.textBoxNAME.Text = "AGV1";
            // 
            // textBoxRemoteClient
            // 
            this.textBoxRemoteClient.Location = new System.Drawing.Point(350, 53);
            this.textBoxRemoteClient.Name = "textBoxRemoteClient";
            this.textBoxRemoteClient.Size = new System.Drawing.Size(68, 28);
            this.textBoxRemoteClient.TabIndex = 2;
            this.textBoxRemoteClient.Text = "9100";
            this.textBoxRemoteClient.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxRemoteHost
            // 
            this.textBoxRemoteHost.Location = new System.Drawing.Point(350, 20);
            this.textBoxRemoteHost.Name = "textBoxRemoteHost";
            this.textBoxRemoteHost.Size = new System.Drawing.Size(68, 28);
            this.textBoxRemoteHost.TabIndex = 2;
            this.textBoxRemoteHost.Text = "9000";
            this.textBoxRemoteHost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(15, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(72, 20);
            this.label7.TabIndex = 0;
            this.label7.Text = "IPアドレス";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(240, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 20);
            this.label6.TabIndex = 0;
            this.label6.Text = "CLIENTポート";
            // 
            // labelNAME
            // 
            this.labelNAME.AutoSize = true;
            this.labelNAME.Location = new System.Drawing.Point(15, 24);
            this.labelNAME.Name = "labelNAME";
            this.labelNAME.Size = new System.Drawing.Size(41, 20);
            this.labelNAME.TabIndex = 0;
            this.labelNAME.Text = "名前";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(240, 24);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "HOSTポート";
            // 
            // buttonAgvAdd
            // 
            this.buttonAgvAdd.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonAgvAdd.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonAgvAdd.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonAgvAdd.BackColorON = System.Drawing.Color.Lime;
            this.buttonAgvAdd.Checked = false;
            this.buttonAgvAdd.CheckMode = false;
            this.buttonAgvAdd.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonAgvAdd.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonAgvAdd.FlatAppearance.BorderSize = 2;
            this.buttonAgvAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAgvAdd.ForeColor = System.Drawing.Color.White;
            this.buttonAgvAdd.ForeColorNormal = System.Drawing.Color.White;
            this.buttonAgvAdd.ForeColorOFF = System.Drawing.Color.White;
            this.buttonAgvAdd.ForeColorON = System.Drawing.Color.Black;
            this.buttonAgvAdd.Location = new System.Drawing.Point(798, 22);
            this.buttonAgvAdd.Name = "buttonAgvAdd";
            this.buttonAgvAdd.Size = new System.Drawing.Size(125, 83);
            this.buttonAgvAdd.TabIndex = 6;
            this.buttonAgvAdd.Tag = false;
            this.buttonAgvAdd.Text = "[F2] AGV追加";
            this.buttonAgvAdd.UseVisualStyleBackColor = false;
            // 
            // listviewAgv
            // 
            this.listviewAgv.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9,
            this.columnHeader12,
            this.columnHeader13});
            this.listviewAgv.FullRowSelect = true;
            this.listviewAgv.GridLines = true;
            this.listviewAgv.HideSelection = false;
            this.listviewAgv.IsStripeColored = false;
            this.listviewAgv.Location = new System.Drawing.Point(12, 131);
            this.listviewAgv.Name = "listviewAgv";
            this.listviewAgv.OwnerDraw = true;
            this.listviewAgv.Size = new System.Drawing.Size(1042, 413);
            this.listviewAgv.StripeBackColor1 = System.Drawing.Color.LightSkyBlue;
            this.listviewAgv.StripeBackColor2 = System.Drawing.Color.White;
            this.listviewAgv.TabIndex = 8;
            this.listviewAgv.TitleBackColor = System.Drawing.SystemColors.Control;
            this.listviewAgv.TitleForeColor = System.Drawing.SystemColors.ControlText;
            this.listviewAgv.UseCompatibleStateImageBehavior = false;
            this.listviewAgv.View = System.Windows.Forms.View.Details;
            this.listviewAgv.VirtualMode = true;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "NAME";
            this.columnHeader1.Width = 101;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "IP";
            this.columnHeader2.Width = 122;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "H PORT";
            this.columnHeader10.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader10.Width = 72;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "C PORT";
            this.columnHeader11.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader11.Width = 73;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "CMD";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader3.Width = 64;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "STA";
            this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader4.Width = 76;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "BAT";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader5.Width = 47;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "MAP";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader6.Width = 47;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "DEG";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader7.Width = 64;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "XPOS";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader8.Width = 87;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "YPOS";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader9.Width = 77;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "RACK DEG";
            this.columnHeader12.Width = 96;
            // 
            // columnHeader13
            // 
            this.columnHeader13.Text = "RACK No";
            this.columnHeader13.Width = 85;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(123, 20);
            this.label5.TabIndex = 0;
            this.label5.Text = "管理中AGV一覧";
            // 
            // buttonAgvRemove
            // 
            this.buttonAgvRemove.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonAgvRemove.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonAgvRemove.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonAgvRemove.BackColorON = System.Drawing.Color.Lime;
            this.buttonAgvRemove.Checked = false;
            this.buttonAgvRemove.CheckMode = false;
            this.buttonAgvRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonAgvRemove.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonAgvRemove.FlatAppearance.BorderSize = 2;
            this.buttonAgvRemove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAgvRemove.ForeColor = System.Drawing.Color.White;
            this.buttonAgvRemove.ForeColorNormal = System.Drawing.Color.White;
            this.buttonAgvRemove.ForeColorOFF = System.Drawing.Color.White;
            this.buttonAgvRemove.ForeColorON = System.Drawing.Color.Black;
            this.buttonAgvRemove.Location = new System.Drawing.Point(929, 22);
            this.buttonAgvRemove.Name = "buttonAgvRemove";
            this.buttonAgvRemove.Size = new System.Drawing.Size(125, 83);
            this.buttonAgvRemove.TabIndex = 6;
            this.buttonAgvRemove.Tag = false;
            this.buttonAgvRemove.Text = "[F3] AGV削除";
            this.buttonAgvRemove.UseVisualStyleBackColor = false;
            // 
            // buttonDetail
            // 
            this.buttonDetail.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonDetail.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonDetail.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonDetail.BackColorON = System.Drawing.Color.Lime;
            this.buttonDetail.Checked = false;
            this.buttonDetail.CheckMode = false;
            this.buttonDetail.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonDetail.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonDetail.FlatAppearance.BorderSize = 2;
            this.buttonDetail.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonDetail.ForeColor = System.Drawing.Color.White;
            this.buttonDetail.ForeColorNormal = System.Drawing.Color.White;
            this.buttonDetail.ForeColorOFF = System.Drawing.Color.White;
            this.buttonDetail.ForeColorON = System.Drawing.Color.Black;
            this.buttonDetail.Location = new System.Drawing.Point(888, 550);
            this.buttonDetail.Name = "buttonDetail";
            this.buttonDetail.Size = new System.Drawing.Size(166, 64);
            this.buttonDetail.TabIndex = 6;
            this.buttonDetail.Tag = false;
            this.buttonDetail.Text = "[F5] 通信ﾃｽﾄ";
            this.buttonDetail.UseVisualStyleBackColor = false;
            // 
            // buttonAgvSim
            // 
            this.buttonAgvSim.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonAgvSim.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonAgvSim.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonAgvSim.BackColorON = System.Drawing.Color.Lime;
            this.buttonAgvSim.Checked = false;
            this.buttonAgvSim.CheckMode = false;
            this.buttonAgvSim.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonAgvSim.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonAgvSim.FlatAppearance.BorderSize = 2;
            this.buttonAgvSim.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonAgvSim.ForeColor = System.Drawing.Color.White;
            this.buttonAgvSim.ForeColorNormal = System.Drawing.Color.White;
            this.buttonAgvSim.ForeColorOFF = System.Drawing.Color.White;
            this.buttonAgvSim.ForeColorON = System.Drawing.Color.Black;
            this.buttonAgvSim.Location = new System.Drawing.Point(12, 550);
            this.buttonAgvSim.Name = "buttonAgvSim";
            this.buttonAgvSim.Size = new System.Drawing.Size(166, 64);
            this.buttonAgvSim.TabIndex = 6;
            this.buttonAgvSim.Tag = false;
            this.buttonAgvSim.Text = "[F6] AGV模擬";
            this.buttonAgvSim.UseVisualStyleBackColor = false;
            // 
            // buttonOrderCommander
            // 
            this.buttonOrderCommander.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonOrderCommander.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonOrderCommander.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonOrderCommander.BackColorON = System.Drawing.Color.Lime;
            this.buttonOrderCommander.Checked = false;
            this.buttonOrderCommander.CheckMode = false;
            this.buttonOrderCommander.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonOrderCommander.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonOrderCommander.FlatAppearance.BorderSize = 2;
            this.buttonOrderCommander.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonOrderCommander.ForeColor = System.Drawing.Color.White;
            this.buttonOrderCommander.ForeColorNormal = System.Drawing.Color.White;
            this.buttonOrderCommander.ForeColorOFF = System.Drawing.Color.White;
            this.buttonOrderCommander.ForeColorON = System.Drawing.Color.Black;
            this.buttonOrderCommander.Location = new System.Drawing.Point(184, 550);
            this.buttonOrderCommander.Name = "buttonOrderCommander";
            this.buttonOrderCommander.Size = new System.Drawing.Size(166, 64);
            this.buttonOrderCommander.TabIndex = 6;
            this.buttonOrderCommander.Tag = false;
            this.buttonOrderCommander.Text = "[F7] 上位通信模擬";
            this.buttonOrderCommander.UseVisualStyleBackColor = false;
            // 
            // SubForm_AgvComManual
            // 
            this._TitleString = "P型AGVモニタ";
            this.ClientSize = new System.Drawing.Size(1066, 626);
            this.Controls.Add(this.listviewAgv);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.buttonAgvRemove);
            this.Controls.Add(this.buttonAgvAdd);
            this.Controls.Add(this.buttonOrderCommander);
            this.Controls.Add(this.buttonAgvSim);
            this.Controls.Add(this.buttonDetail);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.groupBox2);
            this.Name = "SubForm_AgvComManual";
            this.Text = "P型AGVモニタ";
            this.TitleText = "P型AGVモニタ";
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxLocalClient;
        private System.Windows.Forms.TextBox textBoxLocalHost;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private BelicsClass.UI.Controls.BL_FlatButton buttonConnect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxNAME;
        private System.Windows.Forms.TextBox textBoxRemoteHost;
        private System.Windows.Forms.Label labelNAME;
        private System.Windows.Forms.Label label2;
        private BelicsClass.UI.Controls.BL_FlatButton buttonAgvAdd;
        private BelicsClass.UI.Controls.BL_VirtualListView listviewAgv;
        private System.Windows.Forms.Label label5;
        private BelicsClass.UI.Controls.BL_FlatButton buttonAgvRemove;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.TextBox textBoxIP;
        private System.Windows.Forms.TextBox textBoxRemoteClient;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private BelicsClass.UI.Controls.BL_FlatButton buttonDetail;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private System.Windows.Forms.ColumnHeader columnHeader13;
        private BelicsClass.UI.Controls.BL_FlatButton buttonAgvSim;
        private BelicsClass.UI.Controls.BL_FlatButton buttonOrderCommander;
    }
}