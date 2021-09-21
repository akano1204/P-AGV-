namespace PROGRAM
{
	partial class SubForm_SetCoordinate
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
			this.panel1 = new BelicsClass.UI.Controls.BL_DoubleBufferPanel();
			this.trkOpacity = new System.Windows.Forms.TrackBar();
			this.label18 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label14 = new System.Windows.Forms.Label();
			this.numQ2Y = new System.Windows.Forms.NumericUpDown();
			this.numQ2X = new System.Windows.Forms.NumericUpDown();
			this.label7 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.numP2Y = new System.Windows.Forms.NumericUpDown();
			this.numP2X = new System.Windows.Forms.NumericUpDown();
			this.label9 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label13 = new System.Windows.Forms.Label();
			this.numQ1Y = new System.Windows.Forms.NumericUpDown();
			this.numQ1X = new System.Windows.Forms.NumericUpDown();
			this.label5 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.numP1Y = new System.Windows.Forms.NumericUpDown();
			this.numP1X = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.panelMap = new BelicsClass.UI.Controls.BL_DoubleBufferPanel();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkOpacity)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numQ2Y)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numQ2X)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numP2Y)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numP2X)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numQ1Y)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numQ1X)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numP1Y)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numP1X)).BeginInit();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.Silver;
			this.panel1.Controls.Add(this.trkOpacity);
			this.panel1.Controls.Add(this.label18);
			this.panel1.Controls.Add(this.groupBox2);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
			this.panel1.Location = new System.Drawing.Point(803, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(219, 686);
			this.panel1.TabIndex = 2;
			// 
			// trkOpacity
			// 
			this.trkOpacity.AutoSize = false;
			this.trkOpacity.Location = new System.Drawing.Point(98, 366);
			this.trkOpacity.Maximum = 100;
			this.trkOpacity.Name = "trkOpacity";
			this.trkOpacity.Size = new System.Drawing.Size(104, 21);
			this.trkOpacity.TabIndex = 15;
			this.trkOpacity.Value = 1;
			// 
			// label18
			// 
			this.label18.AutoSize = true;
			this.label18.Location = new System.Drawing.Point(15, 362);
			this.label18.Name = "label18";
			this.label18.Size = new System.Drawing.Size(64, 18);
			this.label18.TabIndex = 14;
			this.label18.Text = "不透明度";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label14);
			this.groupBox2.Controls.Add(this.numQ2Y);
			this.groupBox2.Controls.Add(this.numQ2X);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.numP2Y);
			this.groupBox2.Controls.Add(this.numP2X);
			this.groupBox2.Controls.Add(this.label9);
			this.groupBox2.Controls.Add(this.label10);
			this.groupBox2.Controls.Add(this.label11);
			this.groupBox2.Controls.Add(this.label12);
			this.groupBox2.Location = new System.Drawing.Point(7, 183);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 165);
			this.groupBox2.TabIndex = 10;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "二点目";
			// 
			// label14
			// 
			this.label14.AutoSize = true;
			this.label14.ForeColor = System.Drawing.Color.Blue;
			this.label14.Location = new System.Drawing.Point(22, 20);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(160, 18);
			this.label14.TabIndex = 11;
			this.label14.Text = "右ボタンダブルクリックで設定";
			// 
			// numQ2Y
			// 
			this.numQ2Y.DecimalPlaces = 2;
			this.numQ2Y.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numQ2Y.Location = new System.Drawing.Point(85, 133);
			this.numQ2Y.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numQ2Y.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
			this.numQ2Y.Name = "numQ2Y";
			this.numQ2Y.Size = new System.Drawing.Size(98, 24);
			this.numQ2Y.TabIndex = 9;
			this.numQ2Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numQ2Y.Enter += new System.EventHandler(this.num_Enter);
			// 
			// numQ2X
			// 
			this.numQ2X.DecimalPlaces = 2;
			this.numQ2X.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numQ2X.Location = new System.Drawing.Point(85, 105);
			this.numQ2X.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numQ2X.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
			this.numQ2X.Name = "numQ2X";
			this.numQ2X.Size = new System.Drawing.Size(98, 24);
			this.numQ2X.TabIndex = 8;
			this.numQ2X.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numQ2X.Enter += new System.EventHandler(this.num_Enter);
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(62, 135);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(17, 18);
			this.label7.TabIndex = 7;
			this.label7.Text = "Y";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(62, 108);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(17, 18);
			this.label8.TabIndex = 6;
			this.label8.Text = "X";
			// 
			// numP2Y
			// 
			this.numP2Y.DecimalPlaces = 2;
			this.numP2Y.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numP2Y.Location = new System.Drawing.Point(85, 68);
			this.numP2Y.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numP2Y.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
			this.numP2Y.Name = "numP2Y";
			this.numP2Y.Size = new System.Drawing.Size(98, 24);
			this.numP2Y.TabIndex = 5;
			this.numP2Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numP2Y.Enter += new System.EventHandler(this.num_Enter);
			// 
			// numP2X
			// 
			this.numP2X.DecimalPlaces = 2;
			this.numP2X.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numP2X.Location = new System.Drawing.Point(85, 40);
			this.numP2X.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numP2X.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
			this.numP2X.Name = "numP2X";
			this.numP2X.Size = new System.Drawing.Size(98, 24);
			this.numP2X.TabIndex = 4;
			this.numP2X.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numP2X.Enter += new System.EventHandler(this.num_Enter);
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(62, 70);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(17, 18);
			this.label9.TabIndex = 3;
			this.label9.Text = "Y";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(62, 43);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(17, 18);
			this.label10.TabIndex = 2;
			this.label10.Text = "X";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(6, 121);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(50, 18);
			this.label11.TabIndex = 1;
			this.label11.Text = "実座標";
			// 
			// label12
			// 
			this.label12.AutoSize = true;
			this.label12.Location = new System.Drawing.Point(6, 55);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(50, 18);
			this.label12.TabIndex = 0;
			this.label12.Text = "ピクセル";
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label13);
			this.groupBox1.Controls.Add(this.numQ1Y);
			this.groupBox1.Controls.Add(this.numQ1X);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.numP1Y);
			this.groupBox1.Controls.Add(this.numP1X);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(7, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 165);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "一点目";
			// 
			// label13
			// 
			this.label13.AutoSize = true;
			this.label13.ForeColor = System.Drawing.Color.Red;
			this.label13.Location = new System.Drawing.Point(22, 20);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(160, 18);
			this.label13.TabIndex = 10;
			this.label13.Text = "左ボタンダブルクリックで設定";
			// 
			// numQ1Y
			// 
			this.numQ1Y.DecimalPlaces = 2;
			this.numQ1Y.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numQ1Y.Location = new System.Drawing.Point(85, 133);
			this.numQ1Y.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numQ1Y.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
			this.numQ1Y.Name = "numQ1Y";
			this.numQ1Y.Size = new System.Drawing.Size(98, 24);
			this.numQ1Y.TabIndex = 9;
			this.numQ1Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numQ1Y.Enter += new System.EventHandler(this.num_Enter);
			// 
			// numQ1X
			// 
			this.numQ1X.DecimalPlaces = 2;
			this.numQ1X.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numQ1X.Location = new System.Drawing.Point(85, 105);
			this.numQ1X.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numQ1X.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
			this.numQ1X.Name = "numQ1X";
			this.numQ1X.Size = new System.Drawing.Size(98, 24);
			this.numQ1X.TabIndex = 8;
			this.numQ1X.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numQ1X.Enter += new System.EventHandler(this.num_Enter);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(62, 135);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(17, 18);
			this.label5.TabIndex = 7;
			this.label5.Text = "Y";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(62, 108);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(17, 18);
			this.label6.TabIndex = 6;
			this.label6.Text = "X";
			// 
			// numP1Y
			// 
			this.numP1Y.DecimalPlaces = 2;
			this.numP1Y.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numP1Y.Location = new System.Drawing.Point(85, 68);
			this.numP1Y.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numP1Y.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
			this.numP1Y.Name = "numP1Y";
			this.numP1Y.Size = new System.Drawing.Size(98, 24);
			this.numP1Y.TabIndex = 5;
			this.numP1Y.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numP1Y.Enter += new System.EventHandler(this.num_Enter);
			// 
			// numP1X
			// 
			this.numP1X.DecimalPlaces = 2;
			this.numP1X.Increment = new decimal(new int[] {
            1,
            0,
            0,
            131072});
			this.numP1X.Location = new System.Drawing.Point(85, 40);
			this.numP1X.Maximum = new decimal(new int[] {
            1000000,
            0,
            0,
            0});
			this.numP1X.Minimum = new decimal(new int[] {
            100000,
            0,
            0,
            -2147483648});
			this.numP1X.Name = "numP1X";
			this.numP1X.Size = new System.Drawing.Size(98, 24);
			this.numP1X.TabIndex = 4;
			this.numP1X.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this.numP1X.Enter += new System.EventHandler(this.num_Enter);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(62, 70);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(17, 18);
			this.label4.TabIndex = 3;
			this.label4.Text = "Y";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(62, 43);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(17, 18);
			this.label3.TabIndex = 2;
			this.label3.Text = "X";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 121);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50, 18);
			this.label2.TabIndex = 1;
			this.label2.Text = "実座標";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 55);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(50, 18);
			this.label1.TabIndex = 0;
			this.label1.Text = "ピクセル";
			// 
			// panelMap
			// 
			this.panelMap.BackColor = System.Drawing.Color.DarkGray;
			this.panelMap.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelMap.Location = new System.Drawing.Point(0, 0);
			this.panelMap.Name = "panelMap";
			this.panelMap.Size = new System.Drawing.Size(803, 686);
			this.panelMap.TabIndex = 3;
			this.panelMap.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMap_Paint);
			this.panelMap.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseDoubleClick);
			this.panelMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseDown);
			this.panelMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseMove);
			this.panelMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseUp);
			// 
			// SubForm_SetCoordinate
			// 
			this._TitleString = "実座標設定";
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1022, 686);
			this.Controls.Add(this.panelMap);
			this.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("Meiryo UI", 10F);
			this.Margin = new System.Windows.Forms.Padding(4);
			this.Name = "SubForm_SetCoordinate";
			this.Text = "実座標設定";
			this.TitleText = "実座標設定";
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.trkOpacity)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numQ2Y)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numQ2X)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numP2Y)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numP2X)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.numQ1Y)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numQ1X)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numP1Y)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numP1X)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private BelicsClass.UI.Controls.BL_DoubleBufferPanel panel1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.NumericUpDown numQ2Y;
		private System.Windows.Forms.NumericUpDown numQ2X;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.NumericUpDown numP2Y;
		private System.Windows.Forms.NumericUpDown numP2X;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.NumericUpDown numQ1Y;
		private System.Windows.Forms.NumericUpDown numQ1X;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.NumericUpDown numP1Y;
		private System.Windows.Forms.NumericUpDown numP1X;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TrackBar trkOpacity;
		private System.Windows.Forms.Label label18;
		private BelicsClass.UI.Controls.BL_DoubleBufferPanel panelMap;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label13;
	}
}