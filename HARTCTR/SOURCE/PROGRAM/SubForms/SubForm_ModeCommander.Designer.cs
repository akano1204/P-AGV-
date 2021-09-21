namespace PROGRAM
{
    partial class SubForm_ModeCommander
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
            this.components = new System.ComponentModel.Container();
            this.textBoxAgvClient = new System.Windows.Forms.TextBox();
            this.textBoxAgvHost = new System.Windows.Forms.TextBox();
            this.textBoxAgvIP = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxZoom = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxX = new System.Windows.Forms.TextBox();
            this.textBoxY = new System.Windows.Forms.TextBox();
            this.labelX = new System.Windows.Forms.Label();
            this.listviewAGV = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label6 = new System.Windows.Forms.Label();
            this.textBoxAgvID = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panelMap = new BelicsClass.UI.Controls.BL_DoubleBufferPanel();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panel1 = new System.Windows.Forms.Panel();
            this.timerMover = new System.Windows.Forms.Timer(this.components);
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBoxCurX = new System.Windows.Forms.TextBox();
            this.textBoxRackDegree = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.textBoxCurY = new System.Windows.Forms.TextBox();
            this.buttonMove = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonRackRotate = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonRackDown = new BelicsClass.UI.Controls.BL_FlatButton();
            this.checkNextRackUp = new BelicsClass.UI.Controls.BL_FlatButton();
            this.listviewMode = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.checkedListBoxTrigger = new System.Windows.Forms.CheckedListBox();
            this.checkBoxManageSim = new System.Windows.Forms.CheckBox();
            this.listviewConditions = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listviewFloor = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxAgvClient
            // 
            this.textBoxAgvClient.Location = new System.Drawing.Point(301, 7);
            this.textBoxAgvClient.Name = "textBoxAgvClient";
            this.textBoxAgvClient.ReadOnly = true;
            this.textBoxAgvClient.Size = new System.Drawing.Size(47, 24);
            this.textBoxAgvClient.TabIndex = 9;
            this.textBoxAgvClient.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxAgvHost
            // 
            this.textBoxAgvHost.Location = new System.Drawing.Point(255, 7);
            this.textBoxAgvHost.Name = "textBoxAgvHost";
            this.textBoxAgvHost.ReadOnly = true;
            this.textBoxAgvHost.Size = new System.Drawing.Size(47, 24);
            this.textBoxAgvHost.TabIndex = 10;
            this.textBoxAgvHost.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxAgvIP
            // 
            this.textBoxAgvIP.Location = new System.Drawing.Point(137, 7);
            this.textBoxAgvIP.Name = "textBoxAgvIP";
            this.textBoxAgvIP.ReadOnly = true;
            this.textBoxAgvIP.Size = new System.Drawing.Size(119, 24);
            this.textBoxAgvIP.TabIndex = 11;
            this.textBoxAgvIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label10.Location = new System.Drawing.Point(114, 10);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(22, 18);
            this.label10.TabIndex = 8;
            this.label10.Text = "IP";
            // 
            // textBoxZoom
            // 
            this.textBoxZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxZoom.Location = new System.Drawing.Point(678, 6);
            this.textBoxZoom.Name = "textBoxZoom";
            this.textBoxZoom.Size = new System.Drawing.Size(43, 24);
            this.textBoxZoom.TabIndex = 5;
            this.textBoxZoom.Text = "100";
            this.textBoxZoom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxZoom.Leave += new System.EventHandler(this.textBoxZoom_Leave);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(645, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "縮尺";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Meiryo UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(721, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "%";
            // 
            // textBoxX
            // 
            this.textBoxX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxX.Location = new System.Drawing.Point(503, 6);
            this.textBoxX.Name = "textBoxX";
            this.textBoxX.ReadOnly = true;
            this.textBoxX.Size = new System.Drawing.Size(58, 24);
            this.textBoxX.TabIndex = 5;
            this.textBoxX.Text = "0";
            this.textBoxX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxY
            // 
            this.textBoxY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxY.Location = new System.Drawing.Point(560, 6);
            this.textBoxY.Name = "textBoxY";
            this.textBoxY.ReadOnly = true;
            this.textBoxY.Size = new System.Drawing.Size(58, 24);
            this.textBoxY.TabIndex = 5;
            this.textBoxY.Text = "0";
            this.textBoxY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // labelX
            // 
            this.labelX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelX.AutoSize = true;
            this.labelX.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelX.Location = new System.Drawing.Point(423, 9);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(79, 18);
            this.labelX.TabIndex = 3;
            this.labelX.Text = "カーソル座標";
            // 
            // listviewAGV
            // 
            this.listviewAGV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader11});
            this.listviewAGV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listviewAGV.FullRowSelect = true;
            this.listviewAGV.GridLines = true;
            this.listviewAGV.HideSelection = false;
            this.listviewAGV.IsStripeColored = false;
            this.listviewAGV.Location = new System.Drawing.Point(0, 0);
            this.listviewAGV.Name = "listviewAGV";
            this.listviewAGV.OwnerDraw = true;
            this.listviewAGV.Size = new System.Drawing.Size(743, 114);
            this.listviewAGV.StripeBackColor1 = System.Drawing.Color.LightSkyBlue;
            this.listviewAGV.StripeBackColor2 = System.Drawing.Color.White;
            this.listviewAGV.TabIndex = 15;
            this.listviewAGV.TitleBackColor = System.Drawing.SystemColors.Control;
            this.listviewAGV.TitleForeColor = System.Drawing.SystemColors.ControlText;
            this.listviewAGV.UseCompatibleStateImageBehavior = false;
            this.listviewAGV.View = System.Windows.Forms.View.Details;
            this.listviewAGV.VirtualMode = true;
            this.listviewAGV.Click += new System.EventHandler(this.listviewAGV_Click);
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "No";
            this.columnHeader4.Width = 0;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "AGV ID";
            this.columnHeader6.Width = 79;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "状態";
            this.columnHeader7.Width = 188;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "STATE";
            this.columnHeader11.Width = 1963;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(4, 10);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(58, 18);
            this.label6.TabIndex = 13;
            this.label6.Text = "AGV ID";
            // 
            // textBoxAgvID
            // 
            this.textBoxAgvID.Location = new System.Drawing.Point(62, 7);
            this.textBoxAgvID.MaxLength = 5;
            this.textBoxAgvID.Name = "textBoxAgvID";
            this.textBoxAgvID.ReadOnly = true;
            this.textBoxAgvID.Size = new System.Drawing.Size(46, 24);
            this.textBoxAgvID.TabIndex = 12;
            this.textBoxAgvID.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Meiryo UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(619, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 14);
            this.label4.TabIndex = 2;
            this.label4.Text = "cm";
            // 
            // panelMap
            // 
            this.panelMap.BackColor = System.Drawing.Color.White;
            this.panelMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMap.Location = new System.Drawing.Point(0, 0);
            this.panelMap.Name = "panelMap";
            this.panelMap.Size = new System.Drawing.Size(743, 533);
            this.panelMap.TabIndex = 0;
            this.panelMap.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMap_Paint);
            this.panelMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseClick);
            this.panelMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseDown);
            this.panelMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseMove);
            this.panelMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseUp);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(-1, 34);
            this.splitContainer2.Name = "splitContainer2";
            this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.listviewAGV);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.panelMap);
            this.splitContainer2.Size = new System.Drawing.Size(743, 651);
            this.splitContainer2.SplitterDistance = 114;
            this.splitContainer2.TabIndex = 16;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.LightGray;
            this.panel1.Controls.Add(this.label6);
            this.panel1.Controls.Add(this.textBoxAgvID);
            this.panel1.Controls.Add(this.textBoxAgvClient);
            this.panel1.Controls.Add(this.textBoxAgvHost);
            this.panel1.Controls.Add(this.textBoxAgvIP);
            this.panel1.Controls.Add(this.label10);
            this.panel1.Controls.Add(this.textBoxZoom);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBoxX);
            this.panel1.Controls.Add(this.textBoxY);
            this.panel1.Controls.Add(this.labelX);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Location = new System.Drawing.Point(-1, -1);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(741, 36);
            this.panel1.TabIndex = 1;
            // 
            // timerMover
            // 
            this.timerMover.Interval = 1;
            this.timerMover.Tick += new System.EventHandler(this.timerMover_Tick);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "QR数";
            this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader3.Width = 51;
            // 
            // splitContainer1
            // 
            this.splitContainer1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            this.splitContainer1.Panel1.Controls.Add(this.listviewMode);
            this.splitContainer1.Panel1.Controls.Add(this.checkedListBoxTrigger);
            this.splitContainer1.Panel1.Controls.Add(this.checkBoxManageSim);
            this.splitContainer1.Panel1.Controls.Add(this.listviewConditions);
            this.splitContainer1.Panel1.Controls.Add(this.listviewFloor);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(1022, 686);
            this.splitContainer1.SplitterDistance = 275;
            this.splitContainer1.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.textBoxCurX);
            this.groupBox1.Controls.Add(this.textBoxRackDegree);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.textBoxCurY);
            this.groupBox1.Controls.Add(this.buttonMove);
            this.groupBox1.Controls.Add(this.buttonRackRotate);
            this.groupBox1.Controls.Add(this.buttonRackDown);
            this.groupBox1.Controls.Add(this.checkNextRackUp);
            this.groupBox1.Location = new System.Drawing.Point(2, 562);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(269, 122);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "手動操作";
            // 
            // textBoxCurX
            // 
            this.textBoxCurX.Location = new System.Drawing.Point(37, 19);
            this.textBoxCurX.MaxLength = 5;
            this.textBoxCurX.Name = "textBoxCurX";
            this.textBoxCurX.Size = new System.Drawing.Size(57, 24);
            this.textBoxCurX.TabIndex = 20;
            this.textBoxCurX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxRackDegree
            // 
            this.textBoxRackDegree.Location = new System.Drawing.Point(93, 87);
            this.textBoxRackDegree.MaxLength = 5;
            this.textBoxRackDegree.Name = "textBoxRackDegree";
            this.textBoxRackDegree.Size = new System.Drawing.Size(57, 24);
            this.textBoxRackDegree.TabIndex = 21;
            this.textBoxRackDegree.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(40, 90);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(50, 18);
            this.label3.TabIndex = 19;
            this.label3.Text = "棚角度";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label7.Location = new System.Drawing.Point(5, 22);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(31, 18);
            this.label7.TabIndex = 19;
            this.label7.Text = "X,Y";
            // 
            // textBoxCurY
            // 
            this.textBoxCurY.Location = new System.Drawing.Point(93, 19);
            this.textBoxCurY.MaxLength = 5;
            this.textBoxCurY.Name = "textBoxCurY";
            this.textBoxCurY.Size = new System.Drawing.Size(57, 24);
            this.textBoxCurY.TabIndex = 21;
            this.textBoxCurY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // buttonMove
            // 
            this.buttonMove.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonMove.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonMove.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonMove.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonMove.BackColorON = System.Drawing.Color.Lime;
            this.buttonMove.Checked = false;
            this.buttonMove.CheckMode = false;
            this.buttonMove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonMove.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonMove.FlatAppearance.BorderSize = 2;
            this.buttonMove.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonMove.ForeColor = System.Drawing.Color.White;
            this.buttonMove.ForeColorNormal = System.Drawing.Color.White;
            this.buttonMove.ForeColorOFF = System.Drawing.Color.White;
            this.buttonMove.ForeColorON = System.Drawing.Color.Black;
            this.buttonMove.Location = new System.Drawing.Point(152, 15);
            this.buttonMove.Name = "buttonMove";
            this.buttonMove.Size = new System.Drawing.Size(115, 33);
            this.buttonMove.TabIndex = 7;
            this.buttonMove.Tag = false;
            this.buttonMove.Text = "移動指示";
            this.buttonMove.UseVisualStyleBackColor = false;
            // 
            // buttonRackRotate
            // 
            this.buttonRackRotate.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonRackRotate.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonRackRotate.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonRackRotate.BackColorON = System.Drawing.Color.Lime;
            this.buttonRackRotate.Checked = false;
            this.buttonRackRotate.CheckMode = false;
            this.buttonRackRotate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonRackRotate.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonRackRotate.FlatAppearance.BorderSize = 2;
            this.buttonRackRotate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRackRotate.ForeColor = System.Drawing.Color.White;
            this.buttonRackRotate.ForeColorNormal = System.Drawing.Color.White;
            this.buttonRackRotate.ForeColorOFF = System.Drawing.Color.White;
            this.buttonRackRotate.ForeColorON = System.Drawing.Color.Black;
            this.buttonRackRotate.Location = new System.Drawing.Point(152, 80);
            this.buttonRackRotate.Name = "buttonRackRotate";
            this.buttonRackRotate.Size = new System.Drawing.Size(115, 36);
            this.buttonRackRotate.TabIndex = 7;
            this.buttonRackRotate.Tag = false;
            this.buttonRackRotate.Text = "棚旋回";
            this.buttonRackRotate.UseVisualStyleBackColor = false;
            // 
            // buttonRackDown
            // 
            this.buttonRackDown.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonRackDown.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonRackDown.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonRackDown.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonRackDown.BackColorON = System.Drawing.Color.Lime;
            this.buttonRackDown.Checked = false;
            this.buttonRackDown.CheckMode = false;
            this.buttonRackDown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonRackDown.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonRackDown.FlatAppearance.BorderSize = 2;
            this.buttonRackDown.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonRackDown.ForeColor = System.Drawing.Color.White;
            this.buttonRackDown.ForeColorNormal = System.Drawing.Color.White;
            this.buttonRackDown.ForeColorOFF = System.Drawing.Color.White;
            this.buttonRackDown.ForeColorON = System.Drawing.Color.Black;
            this.buttonRackDown.Location = new System.Drawing.Point(152, 46);
            this.buttonRackDown.Name = "buttonRackDown";
            this.buttonRackDown.Size = new System.Drawing.Size(115, 36);
            this.buttonRackDown.TabIndex = 7;
            this.buttonRackDown.Tag = false;
            this.buttonRackDown.Text = "棚下降";
            this.buttonRackDown.UseVisualStyleBackColor = false;
            // 
            // checkNextRackUp
            // 
            this.checkNextRackUp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkNextRackUp.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.checkNextRackUp.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.checkNextRackUp.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.checkNextRackUp.BackColorON = System.Drawing.Color.Lime;
            this.checkNextRackUp.Checked = false;
            this.checkNextRackUp.CheckMode = true;
            this.checkNextRackUp.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkNextRackUp.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.checkNextRackUp.FlatAppearance.BorderSize = 2;
            this.checkNextRackUp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkNextRackUp.ForeColor = System.Drawing.Color.White;
            this.checkNextRackUp.ForeColorNormal = System.Drawing.Color.White;
            this.checkNextRackUp.ForeColorOFF = System.Drawing.Color.White;
            this.checkNextRackUp.ForeColorON = System.Drawing.Color.Black;
            this.checkNextRackUp.Location = new System.Drawing.Point(36, 46);
            this.checkNextRackUp.Name = "checkNextRackUp";
            this.checkNextRackUp.Size = new System.Drawing.Size(115, 36);
            this.checkNextRackUp.TabIndex = 7;
            this.checkNextRackUp.Tag = false;
            this.checkNextRackUp.Text = "次QR棚上昇";
            this.checkNextRackUp.UseVisualStyleBackColor = false;
            // 
            // listviewMode
            // 
            this.listviewMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listviewMode.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader9,
            this.columnHeader10});
            this.listviewMode.FullRowSelect = true;
            this.listviewMode.GridLines = true;
            this.listviewMode.HideSelection = false;
            this.listviewMode.IsStripeColored = false;
            this.listviewMode.Location = new System.Drawing.Point(-1, 134);
            this.listviewMode.Name = "listviewMode";
            this.listviewMode.OwnerDraw = true;
            this.listviewMode.Size = new System.Drawing.Size(275, 154);
            this.listviewMode.StripeBackColor1 = System.Drawing.Color.LightSkyBlue;
            this.listviewMode.StripeBackColor2 = System.Drawing.Color.White;
            this.listviewMode.TabIndex = 14;
            this.listviewMode.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.listviewMode.TitleForeColor = System.Drawing.SystemColors.ControlText;
            this.listviewMode.UseCompatibleStateImageBehavior = false;
            this.listviewMode.View = System.Windows.Forms.View.Details;
            this.listviewMode.VirtualMode = true;
            this.listviewMode.SelectedIndexChanged += new System.EventHandler(this.listviewMode_SelectedIndexChanged);
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "動作モード";
            this.columnHeader9.Width = 246;
            // 
            // columnHeader10
            // 
            this.columnHeader10.Text = "";
            this.columnHeader10.Width = 0;
            // 
            // checkedListBoxTrigger
            // 
            this.checkedListBoxTrigger.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkedListBoxTrigger.Enabled = false;
            this.checkedListBoxTrigger.FormattingEnabled = true;
            this.checkedListBoxTrigger.IntegralHeight = false;
            this.checkedListBoxTrigger.Location = new System.Drawing.Point(-1, 470);
            this.checkedListBoxTrigger.Name = "checkedListBoxTrigger";
            this.checkedListBoxTrigger.Size = new System.Drawing.Size(276, 90);
            this.checkedListBoxTrigger.TabIndex = 0;
            // 
            // checkBoxManageSim
            // 
            this.checkBoxManageSim.AutoSize = true;
            this.checkBoxManageSim.Location = new System.Drawing.Point(3, 112);
            this.checkBoxManageSim.Name = "checkBoxManageSim";
            this.checkBoxManageSim.Size = new System.Drawing.Size(100, 22);
            this.checkBoxManageSim.TabIndex = 16;
            this.checkBoxManageSim.Text = "統括PC模擬";
            this.checkBoxManageSim.UseVisualStyleBackColor = true;
            this.checkBoxManageSim.CheckedChanged += new System.EventHandler(this.checkBoxManageSim_CheckedChanged);
            // 
            // listviewConditions
            // 
            this.listviewConditions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listviewConditions.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader5,
            this.columnHeader8});
            this.listviewConditions.FullRowSelect = true;
            this.listviewConditions.GridLines = true;
            this.listviewConditions.HideSelection = false;
            this.listviewConditions.IsStripeColored = false;
            this.listviewConditions.Location = new System.Drawing.Point(-1, 287);
            this.listviewConditions.Name = "listviewConditions";
            this.listviewConditions.OwnerDraw = true;
            this.listviewConditions.Size = new System.Drawing.Size(275, 184);
            this.listviewConditions.StripeBackColor1 = System.Drawing.Color.LightSkyBlue;
            this.listviewConditions.StripeBackColor2 = System.Drawing.Color.White;
            this.listviewConditions.TabIndex = 13;
            this.listviewConditions.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.listviewConditions.TitleForeColor = System.Drawing.SystemColors.ControlText;
            this.listviewConditions.UseCompatibleStateImageBehavior = false;
            this.listviewConditions.View = System.Windows.Forms.View.Details;
            this.listviewConditions.VirtualMode = true;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Width = 0;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "全動作条件";
            this.columnHeader8.Width = 300;
            // 
            // listviewFloor
            // 
            this.listviewFloor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listviewFloor.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3});
            this.listviewFloor.FullRowSelect = true;
            this.listviewFloor.GridLines = true;
            this.listviewFloor.HideSelection = false;
            this.listviewFloor.IsStripeColored = false;
            this.listviewFloor.Location = new System.Drawing.Point(-1, -1);
            this.listviewFloor.Name = "listviewFloor";
            this.listviewFloor.OwnerDraw = true;
            this.listviewFloor.Size = new System.Drawing.Size(276, 112);
            this.listviewFloor.StripeBackColor1 = System.Drawing.Color.LightSkyBlue;
            this.listviewFloor.StripeBackColor2 = System.Drawing.Color.White;
            this.listviewFloor.TabIndex = 6;
            this.listviewFloor.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.listviewFloor.TitleForeColor = System.Drawing.SystemColors.ControlText;
            this.listviewFloor.UseCompatibleStateImageBehavior = false;
            this.listviewFloor.View = System.Windows.Forms.View.Details;
            this.listviewFloor.VirtualMode = true;
            this.listviewFloor.SelectedIndexChanged += new System.EventHandler(this.listviewFloor_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "フロアコード";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader2.Width = 79;
            // 
            // SubForm_ModeCommander
            // 
            this._TitleString = "運用選択";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1022, 686);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "SubForm_ModeCommander";
            this.Text = "運用選択";
            this.TitleText = "運用選択";
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxAgvClient;
        private System.Windows.Forms.TextBox textBoxAgvHost;
        private System.Windows.Forms.TextBox textBoxAgvIP;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxZoom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxX;
        private System.Windows.Forms.TextBox textBoxY;
        private System.Windows.Forms.Label labelX;
        private BelicsClass.UI.Controls.BL_VirtualListView listviewAGV;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBoxAgvID;
        private System.Windows.Forms.Label label4;
        private BelicsClass.UI.Controls.BL_DoubleBufferPanel panelMap;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Timer timerMover;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBoxCurX;
        private System.Windows.Forms.TextBox textBoxRackDegree;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox textBoxCurY;
        private BelicsClass.UI.Controls.BL_FlatButton buttonMove;
        private BelicsClass.UI.Controls.BL_FlatButton buttonRackRotate;
        private BelicsClass.UI.Controls.BL_FlatButton buttonRackDown;
        private BelicsClass.UI.Controls.BL_FlatButton checkNextRackUp;
        private BelicsClass.UI.Controls.BL_VirtualListView listviewMode;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.CheckedListBox checkedListBoxTrigger;
        private System.Windows.Forms.CheckBox checkBoxManageSim;
        private BelicsClass.UI.Controls.BL_VirtualListView listviewConditions;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private BelicsClass.UI.Controls.BL_VirtualListView listviewFloor;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
    }
}