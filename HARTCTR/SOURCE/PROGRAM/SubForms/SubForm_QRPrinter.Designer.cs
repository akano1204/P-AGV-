namespace PROGRAM
{
	partial class SubForm_QRPrinter
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
			this.listviewQR = new BelicsClass.UI.Controls.BL_VirtualListView();
			this.clmNo = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.clmQR = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
			this.label1 = new System.Windows.Forms.Label();
			this.textBoxIP = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioButtonRack = new System.Windows.Forms.RadioButton();
			this.radioButtonAll = new System.Windows.Forms.RadioButton();
			this.textBoxQR = new System.Windows.Forms.TextBox();
			this.buttonPrintTextBox = new BelicsClass.UI.Controls.BL_FlatButton();
			this.buttonPrintListView = new BelicsClass.UI.Controls.BL_FlatButton();
			this.radioButtonFloor = new System.Windows.Forms.RadioButton();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// listviewQR
			// 
			this.listviewQR.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listviewQR.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.listviewQR.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.clmNo,
            this.clmQR});
			this.listviewQR.Font = new System.Drawing.Font("Meiryo UI", 12F);
			this.listviewQR.FullRowSelect = true;
			this.listviewQR.GridLines = true;
			this.listviewQR.HideSelection = false;
			this.listviewQR.IsStripeColored = false;
			this.listviewQR.Location = new System.Drawing.Point(239, 208);
			this.listviewQR.Name = "listviewQR";
			this.listviewQR.OwnerDraw = true;
			this.listviewQR.Size = new System.Drawing.Size(461, 457);
			this.listviewQR.StripeBackColor1 = System.Drawing.Color.LightSkyBlue;
			this.listviewQR.StripeBackColor2 = System.Drawing.Color.White;
			this.listviewQR.TabIndex = 11;
			this.listviewQR.TitleBackColor = System.Drawing.SystemColors.Control;
			this.listviewQR.TitleForeColor = System.Drawing.SystemColors.ControlText;
			this.listviewQR.UseCompatibleStateImageBehavior = false;
			this.listviewQR.View = System.Windows.Forms.View.Details;
			this.listviewQR.VirtualMode = true;
			this.listviewQR.SelectedIndexChanged += new System.EventHandler(this.listviewQR_SelectedIndexChanged);
			// 
			// clmNo
			// 
			this.clmNo.Text = "No";
			// 
			// clmQR
			// 
			this.clmQR.Text = "QR";
			this.clmQR.Width = 200;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 20);
			this.label1.TabIndex = 12;
			this.label1.Text = "IPアドレス";
			// 
			// textBoxIP
			// 
			this.textBoxIP.Location = new System.Drawing.Point(90, 15);
			this.textBoxIP.Name = "textBoxIP";
			this.textBoxIP.Size = new System.Drawing.Size(183, 28);
			this.textBoxIP.TabIndex = 13;
			this.textBoxIP.TextChanged += new System.EventHandler(this.textBoxIP_TextChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioButtonFloor);
			this.groupBox1.Controls.Add(this.radioButtonRack);
			this.groupBox1.Controls.Add(this.radioButtonAll);
			this.groupBox1.Location = new System.Drawing.Point(48, 208);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(142, 124);
			this.groupBox1.TabIndex = 14;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "リスト表示";
			// 
			// radioButtonRack
			// 
			this.radioButtonRack.AutoSize = true;
			this.radioButtonRack.Location = new System.Drawing.Point(13, 87);
			this.radioButtonRack.Name = "radioButtonRack";
			this.radioButtonRack.Size = new System.Drawing.Size(43, 24);
			this.radioButtonRack.TabIndex = 1;
			this.radioButtonRack.Text = "棚";
			this.radioButtonRack.UseVisualStyleBackColor = true;
			this.radioButtonRack.CheckedChanged += new System.EventHandler(this.radioButtonView_CheckedChanged);
			// 
			// radioButtonAll
			// 
			this.radioButtonAll.AutoSize = true;
			this.radioButtonAll.Checked = true;
			this.radioButtonAll.Location = new System.Drawing.Point(13, 27);
			this.radioButtonAll.Name = "radioButtonAll";
			this.radioButtonAll.Size = new System.Drawing.Size(56, 24);
			this.radioButtonAll.TabIndex = 0;
			this.radioButtonAll.TabStop = true;
			this.radioButtonAll.Text = "全て";
			this.radioButtonAll.UseVisualStyleBackColor = true;
			this.radioButtonAll.CheckedChanged += new System.EventHandler(this.radioButtonView_CheckedChanged);
			// 
			// textBoxQR
			// 
			this.textBoxQR.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBoxQR.Location = new System.Drawing.Point(239, 107);
			this.textBoxQR.Name = "textBoxQR";
			this.textBoxQR.Size = new System.Drawing.Size(461, 28);
			this.textBoxQR.TabIndex = 15;
			// 
			// buttonPrintTextBox
			// 
			this.buttonPrintTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonPrintTextBox.BackColor = System.Drawing.Color.RoyalBlue;
			this.buttonPrintTextBox.BackColorNormal = System.Drawing.Color.RoyalBlue;
			this.buttonPrintTextBox.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.buttonPrintTextBox.BackColorON = System.Drawing.Color.Lime;
			this.buttonPrintTextBox.Checked = false;
			this.buttonPrintTextBox.CheckMode = false;
			this.buttonPrintTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
			this.buttonPrintTextBox.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonPrintTextBox.FlatAppearance.BorderSize = 2;
			this.buttonPrintTextBox.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonPrintTextBox.ForeColor = System.Drawing.Color.White;
			this.buttonPrintTextBox.ForeColorNormal = System.Drawing.Color.White;
			this.buttonPrintTextBox.ForeColorOFF = System.Drawing.Color.White;
			this.buttonPrintTextBox.ForeColorON = System.Drawing.Color.Black;
			this.buttonPrintTextBox.Location = new System.Drawing.Point(778, 102);
			this.buttonPrintTextBox.Name = "buttonPrintTextBox";
			this.buttonPrintTextBox.Size = new System.Drawing.Size(189, 37);
			this.buttonPrintTextBox.TabIndex = 16;
			this.buttonPrintTextBox.Tag = false;
			this.buttonPrintTextBox.Text = "テキストから発行";
			this.buttonPrintTextBox.UseVisualStyleBackColor = false;
			this.buttonPrintTextBox.Click += new System.EventHandler(this.buttonPrintTextBox_Click);
			// 
			// buttonPrintListView
			// 
			this.buttonPrintListView.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonPrintListView.BackColor = System.Drawing.Color.RoyalBlue;
			this.buttonPrintListView.BackColorNormal = System.Drawing.Color.RoyalBlue;
			this.buttonPrintListView.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			this.buttonPrintListView.BackColorON = System.Drawing.Color.Lime;
			this.buttonPrintListView.Checked = false;
			this.buttonPrintListView.CheckMode = false;
			this.buttonPrintListView.Cursor = System.Windows.Forms.Cursors.Hand;
			this.buttonPrintListView.FlatAppearance.BorderColor = System.Drawing.Color.Black;
			this.buttonPrintListView.FlatAppearance.BorderSize = 2;
			this.buttonPrintListView.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonPrintListView.ForeColor = System.Drawing.Color.White;
			this.buttonPrintListView.ForeColorNormal = System.Drawing.Color.White;
			this.buttonPrintListView.ForeColorOFF = System.Drawing.Color.White;
			this.buttonPrintListView.ForeColorON = System.Drawing.Color.Black;
			this.buttonPrintListView.Location = new System.Drawing.Point(778, 208);
			this.buttonPrintListView.Name = "buttonPrintListView";
			this.buttonPrintListView.Size = new System.Drawing.Size(189, 37);
			this.buttonPrintListView.TabIndex = 17;
			this.buttonPrintListView.Tag = false;
			this.buttonPrintListView.Text = "リストから発行";
			this.buttonPrintListView.UseVisualStyleBackColor = false;
			this.buttonPrintListView.Click += new System.EventHandler(this.buttonPrintListView_Click);
			// 
			// radioButtonFloor
			// 
			this.radioButtonFloor.AutoSize = true;
			this.radioButtonFloor.Location = new System.Drawing.Point(13, 57);
			this.radioButtonFloor.Name = "radioButtonFloor";
			this.radioButtonFloor.Size = new System.Drawing.Size(43, 24);
			this.radioButtonFloor.TabIndex = 2;
			this.radioButtonFloor.Text = "床";
			this.radioButtonFloor.UseVisualStyleBackColor = true;
			this.radioButtonFloor.CheckedChanged += new System.EventHandler(this.radioButtonView_CheckedChanged);
			// 
			// SubForm_QRPrinter
			// 
			this._TitleString = "QRコード発行";
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1022, 686);
			this.Controls.Add(this.buttonPrintListView);
			this.Controls.Add(this.buttonPrintTextBox);
			this.Controls.Add(this.textBoxQR);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.textBoxIP);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listviewQR);
			this.Name = "SubForm_QRPrinter";
			this.Text = "QRコード発行";
			this.TitleText = "QRコード発行";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private BelicsClass.UI.Controls.BL_VirtualListView listviewQR;
		private System.Windows.Forms.ColumnHeader clmQR;
		private System.Windows.Forms.ColumnHeader clmNo;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textBoxIP;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioButtonRack;
		private System.Windows.Forms.RadioButton radioButtonAll;
		private System.Windows.Forms.TextBox textBoxQR;
		private BelicsClass.UI.Controls.BL_FlatButton buttonPrintTextBox;
		private BelicsClass.UI.Controls.BL_FlatButton buttonPrintListView;
		private System.Windows.Forms.RadioButton radioButtonFloor;
	}
}