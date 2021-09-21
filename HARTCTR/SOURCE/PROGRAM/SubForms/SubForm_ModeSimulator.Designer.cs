namespace PROGRAM
{
    partial class SubForm_ModeSimulator
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
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.panel1 = new System.Windows.Forms.Panel();
            this.textBoxCurX = new System.Windows.Forms.TextBox();
            this.textBoxCurY = new System.Windows.Forms.TextBox();
            this.textBoxSleepInterval = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxZoom = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBoxX = new System.Windows.Forms.TextBox();
            this.textBoxY = new System.Windows.Forms.TextBox();
            this.labelX = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.panelMap = new BelicsClass.UI.Controls.BL_DoubleBufferPanel();
            this.textBoxFloorCode = new System.Windows.Forms.TextBox();
            this.buttonStart = new BelicsClass.UI.Controls.BL_FlatButton();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonTrg = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonKeepState = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonStuck = new BelicsClass.UI.Controls.BL_FlatButton();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.listviewFloor = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.columnHeader10 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader11 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader12 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listviewMode = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.listviewTrg = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.label6 = new System.Windows.Forms.Label();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.listviewAGV = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.timerMover = new System.Windows.Forms.Timer(this.components);
            this.panel1.SuspendLayout();
            this.panelMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.SuspendLayout();
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "条件";
            this.columnHeader9.Width = 263;
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.BackColor = System.Drawing.Color.LightGray;
            this.panel1.Controls.Add(this.textBoxCurX);
            this.panel1.Controls.Add(this.textBoxCurY);
            this.panel1.Controls.Add(this.textBoxSleepInterval);
            this.panel1.Controls.Add(this.label5);
            this.panel1.Controls.Add(this.textBoxZoom);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.label7);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.textBoxX);
            this.panel1.Controls.Add(this.textBoxY);
            this.panel1.Controls.Add(this.labelX);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(856, 36);
            this.panel1.TabIndex = 2;
            // 
            // textBoxCurX
            // 
            this.textBoxCurX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCurX.Location = new System.Drawing.Point(69, 7);
            this.textBoxCurX.MaxLength = 5;
            this.textBoxCurX.Name = "textBoxCurX";
            this.textBoxCurX.ReadOnly = true;
            this.textBoxCurX.Size = new System.Drawing.Size(52, 24);
            this.textBoxCurX.TabIndex = 5;
            this.textBoxCurX.Text = "99999";
            this.textBoxCurX.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxCurY
            // 
            this.textBoxCurY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCurY.Location = new System.Drawing.Point(120, 7);
            this.textBoxCurY.MaxLength = 5;
            this.textBoxCurY.Name = "textBoxCurY";
            this.textBoxCurY.ReadOnly = true;
            this.textBoxCurY.Size = new System.Drawing.Size(52, 24);
            this.textBoxCurY.TabIndex = 5;
            this.textBoxCurY.Text = "99999";
            this.textBoxCurY.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // textBoxSleepInterval
            // 
            this.textBoxSleepInterval.Location = new System.Drawing.Point(459, 6);
            this.textBoxSleepInterval.Name = "textBoxSleepInterval";
            this.textBoxSleepInterval.Size = new System.Drawing.Size(43, 24);
            this.textBoxSleepInterval.TabIndex = 5;
            this.textBoxSleepInterval.Text = "5";
            this.textBoxSleepInterval.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxSleepInterval.Leave += new System.EventHandler(this.textBoxSleepInterval_Leave);
            // 
            // label5
            // 
            this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Meiryo UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(171, 12);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(24, 14);
            this.label5.TabIndex = 2;
            this.label5.Text = "cm";
            // 
            // textBoxZoom
            // 
            this.textBoxZoom.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxZoom.Location = new System.Drawing.Point(793, 6);
            this.textBoxZoom.Name = "textBoxZoom";
            this.textBoxZoom.Size = new System.Drawing.Size(43, 24);
            this.textBoxZoom.TabIndex = 5;
            this.textBoxZoom.Text = "100";
            this.textBoxZoom.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.textBoxZoom.Leave += new System.EventHandler(this.textBoxZoom_Leave);
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(3, 9);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(65, 18);
            this.label3.TabIndex = 3;
            this.label3.Text = "AGV座標";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(382, 9);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(77, 18);
            this.label7.TabIndex = 2;
            this.label7.Text = "スキャン速度";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(760, 9);
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
            this.label1.Location = new System.Drawing.Point(836, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "%";
            // 
            // textBoxX
            // 
            this.textBoxX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxX.Location = new System.Drawing.Point(618, 6);
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
            this.textBoxY.Location = new System.Drawing.Point(675, 6);
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
            this.labelX.Location = new System.Drawing.Point(538, 9);
            this.labelX.Name = "labelX";
            this.labelX.Size = new System.Drawing.Size(79, 18);
            this.labelX.TabIndex = 3;
            this.labelX.Text = "カーソル座標";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Meiryo UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label8.Location = new System.Drawing.Point(502, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(23, 14);
            this.label8.TabIndex = 2;
            this.label8.Text = "ms";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Meiryo UI", 8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(734, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(24, 14);
            this.label4.TabIndex = 2;
            this.label4.Text = "cm";
            // 
            // panelMap
            // 
            this.panelMap.BackColor = System.Drawing.Color.White;
            this.panelMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMap.Controls.Add(this.textBoxFloorCode);
            this.panelMap.Controls.Add(this.buttonStart);
            this.panelMap.Controls.Add(this.label9);
            this.panelMap.Controls.Add(this.buttonTrg);
            this.panelMap.Controls.Add(this.buttonKeepState);
            this.panelMap.Controls.Add(this.buttonStuck);
            this.panelMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMap.Location = new System.Drawing.Point(0, 0);
            this.panelMap.Name = "panelMap";
            this.panelMap.Size = new System.Drawing.Size(857, 534);
            this.panelMap.TabIndex = 1;
            this.panelMap.Paint += new System.Windows.Forms.PaintEventHandler(this.panelMap_Paint);
            this.panelMap.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseClick);
            this.panelMap.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseDown);
            this.panelMap.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseMove);
            this.panelMap.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelMap_MouseUp);
            // 
            // textBoxFloorCode
            // 
            this.textBoxFloorCode.Location = new System.Drawing.Point(60, 248);
            this.textBoxFloorCode.MaxLength = 1;
            this.textBoxFloorCode.Name = "textBoxFloorCode";
            this.textBoxFloorCode.ReadOnly = true;
            this.textBoxFloorCode.Size = new System.Drawing.Size(33, 24);
            this.textBoxFloorCode.TabIndex = 8;
            this.textBoxFloorCode.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxFloorCode.Visible = false;
            // 
            // buttonStart
            // 
            this.buttonStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonStart.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonStart.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonStart.BackColorON = System.Drawing.Color.Lime;
            this.buttonStart.Checked = false;
            this.buttonStart.CheckMode = true;
            this.buttonStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonStart.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonStart.FlatAppearance.BorderSize = 2;
            this.buttonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStart.ForeColor = System.Drawing.Color.White;
            this.buttonStart.ForeColorNormal = System.Drawing.Color.White;
            this.buttonStart.ForeColorOFF = System.Drawing.Color.White;
            this.buttonStart.ForeColorON = System.Drawing.Color.Black;
            this.buttonStart.Location = new System.Drawing.Point(3, 7);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(160, 33);
            this.buttonStart.TabIndex = 4;
            this.buttonStart.Tag = false;
            this.buttonStart.Text = "[F5] 動作開始";
            this.buttonStart.UseVisualStyleBackColor = false;
            this.buttonStart.Visible = false;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(21, 251);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(38, 18);
            this.label9.TabIndex = 2;
            this.label9.Text = "フロア";
            this.label9.Visible = false;
            // 
            // buttonTrg
            // 
            this.buttonTrg.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonTrg.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonTrg.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonTrg.BackColorON = System.Drawing.Color.Lime;
            this.buttonTrg.Checked = false;
            this.buttonTrg.CheckMode = true;
            this.buttonTrg.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonTrg.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonTrg.FlatAppearance.BorderSize = 2;
            this.buttonTrg.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonTrg.ForeColor = System.Drawing.Color.White;
            this.buttonTrg.ForeColorNormal = System.Drawing.Color.White;
            this.buttonTrg.ForeColorOFF = System.Drawing.Color.White;
            this.buttonTrg.ForeColorON = System.Drawing.Color.Black;
            this.buttonTrg.Location = new System.Drawing.Point(3, 85);
            this.buttonTrg.Name = "buttonTrg";
            this.buttonTrg.Size = new System.Drawing.Size(160, 33);
            this.buttonTrg.TabIndex = 4;
            this.buttonTrg.Tag = false;
            this.buttonTrg.Text = "[F1] トリガーON";
            this.buttonTrg.UseVisualStyleBackColor = false;
            this.buttonTrg.Visible = false;
            // 
            // buttonKeepState
            // 
            this.buttonKeepState.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonKeepState.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonKeepState.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonKeepState.BackColorON = System.Drawing.Color.Lime;
            this.buttonKeepState.Checked = false;
            this.buttonKeepState.CheckMode = true;
            this.buttonKeepState.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonKeepState.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonKeepState.FlatAppearance.BorderSize = 2;
            this.buttonKeepState.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonKeepState.ForeColor = System.Drawing.Color.White;
            this.buttonKeepState.ForeColorNormal = System.Drawing.Color.White;
            this.buttonKeepState.ForeColorOFF = System.Drawing.Color.White;
            this.buttonKeepState.ForeColorON = System.Drawing.Color.Black;
            this.buttonKeepState.Location = new System.Drawing.Point(3, 124);
            this.buttonKeepState.Name = "buttonKeepState";
            this.buttonKeepState.Size = new System.Drawing.Size(160, 33);
            this.buttonKeepState.TabIndex = 4;
            this.buttonKeepState.Tag = false;
            this.buttonKeepState.Text = "[F2] 状態保持";
            this.buttonKeepState.UseVisualStyleBackColor = false;
            this.buttonKeepState.Visible = false;
            // 
            // buttonStuck
            // 
            this.buttonStuck.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonStuck.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonStuck.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonStuck.BackColorON = System.Drawing.Color.Lime;
            this.buttonStuck.Checked = false;
            this.buttonStuck.CheckMode = true;
            this.buttonStuck.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonStuck.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonStuck.FlatAppearance.BorderSize = 2;
            this.buttonStuck.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonStuck.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonStuck.ForeColor = System.Drawing.Color.White;
            this.buttonStuck.ForeColorNormal = System.Drawing.Color.White;
            this.buttonStuck.ForeColorOFF = System.Drawing.Color.White;
            this.buttonStuck.ForeColorON = System.Drawing.Color.Black;
            this.buttonStuck.Location = new System.Drawing.Point(3, 163);
            this.buttonStuck.Name = "buttonStuck";
            this.buttonStuck.Size = new System.Drawing.Size(116, 37);
            this.buttonStuck.TabIndex = 0;
            this.buttonStuck.Tag = false;
            this.buttonStuck.Text = "グリッドに吸着";
            this.buttonStuck.UseVisualStyleBackColor = false;
            this.buttonStuck.Visible = false;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.listviewFloor);
            this.splitContainer1.Panel1.Controls.Add(this.listviewMode);
            this.splitContainer1.Panel1.Controls.Add(this.listviewTrg);
            this.splitContainer1.Panel1.Controls.Add(this.label6);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.BackColor = System.Drawing.Color.White;
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Panel2.Controls.Add(this.panel1);
            this.splitContainer1.Size = new System.Drawing.Size(1022, 686);
            this.splitContainer1.SplitterDistance = 161;
            this.splitContainer1.TabIndex = 2;
            // 
            // listviewFloor
            // 
            this.listviewFloor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listviewFloor.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listviewFloor.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader10,
            this.columnHeader11,
            this.columnHeader12});
            this.listviewFloor.FullRowSelect = true;
            this.listviewFloor.GridLines = true;
            this.listviewFloor.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.listviewFloor.HideSelection = false;
            this.listviewFloor.IsStripeColored = false;
            this.listviewFloor.Location = new System.Drawing.Point(0, 0);
            this.listviewFloor.Name = "listviewFloor";
            this.listviewFloor.OwnerDraw = true;
            this.listviewFloor.Size = new System.Drawing.Size(161, 156);
            this.listviewFloor.StripeBackColor1 = System.Drawing.Color.LightSkyBlue;
            this.listviewFloor.StripeBackColor2 = System.Drawing.Color.White;
            this.listviewFloor.TabIndex = 14;
            this.listviewFloor.TitleBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.listviewFloor.TitleForeColor = System.Drawing.SystemColors.ControlText;
            this.listviewFloor.UseCompatibleStateImageBehavior = false;
            this.listviewFloor.View = System.Windows.Forms.View.Details;
            this.listviewFloor.VirtualMode = true;
            this.listviewFloor.SelectedIndexChanged += new System.EventHandler(this.listviewFloor_SelectedIndexChanged);
            // 
            // columnHeader10
            // 
            this.columnHeader10.Width = 0;
            // 
            // columnHeader11
            // 
            this.columnHeader11.Text = "フロアコード";
            this.columnHeader11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader11.Width = 79;
            // 
            // columnHeader12
            // 
            this.columnHeader12.Text = "QR数";
            this.columnHeader12.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader12.Width = 51;
            // 
            // listviewMode
            // 
            this.listviewMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listviewMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listviewMode.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader2,
            this.columnHeader5});
            this.listviewMode.FullRowSelect = true;
            this.listviewMode.GridLines = true;
            this.listviewMode.HideSelection = false;
            this.listviewMode.IsStripeColored = false;
            this.listviewMode.Location = new System.Drawing.Point(0, 155);
            this.listviewMode.Name = "listviewMode";
            this.listviewMode.OwnerDraw = true;
            this.listviewMode.Size = new System.Drawing.Size(161, 176);
            this.listviewMode.StripeBackColor1 = System.Drawing.Color.LightSkyBlue;
            this.listviewMode.StripeBackColor2 = System.Drawing.Color.White;
            this.listviewMode.TabIndex = 9;
            this.listviewMode.TitleBackColor = System.Drawing.SystemColors.Control;
            this.listviewMode.TitleForeColor = System.Drawing.SystemColors.ControlText;
            this.listviewMode.UseCompatibleStateImageBehavior = false;
            this.listviewMode.View = System.Windows.Forms.View.Details;
            this.listviewMode.VirtualMode = true;
            this.listviewMode.SelectedIndexChanged += new System.EventHandler(this.listviewMode_SelectedIndexChanged);
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "動作モード";
            this.columnHeader2.Width = 130;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "";
            this.columnHeader5.Width = 0;
            // 
            // listviewTrg
            // 
            this.listviewTrg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listviewTrg.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listviewTrg.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8});
            this.listviewTrg.FullRowSelect = true;
            this.listviewTrg.GridLines = true;
            this.listviewTrg.HideSelection = false;
            this.listviewTrg.IsStripeColored = false;
            this.listviewTrg.Location = new System.Drawing.Point(0, 355);
            this.listviewTrg.Name = "listviewTrg";
            this.listviewTrg.OwnerDraw = true;
            this.listviewTrg.Size = new System.Drawing.Size(161, 331);
            this.listviewTrg.StripeBackColor1 = System.Drawing.Color.LightSkyBlue;
            this.listviewTrg.StripeBackColor2 = System.Drawing.Color.White;
            this.listviewTrg.TabIndex = 9;
            this.listviewTrg.TitleBackColor = System.Drawing.SystemColors.Control;
            this.listviewTrg.TitleForeColor = System.Drawing.SystemColors.ControlText;
            this.listviewTrg.UseCompatibleStateImageBehavior = false;
            this.listviewTrg.View = System.Windows.Forms.View.Details;
            this.listviewTrg.VirtualMode = true;
            this.listviewTrg.SelectedIndexChanged += new System.EventHandler(this.listviewTrg_SelectedIndexChanged);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "No";
            this.columnHeader6.Width = 0;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "ID";
            this.columnHeader7.Width = 95;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "状態";
            this.columnHeader8.Width = 41;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(3, 334);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(104, 18);
            this.label6.TabIndex = 3;
            this.label6.Text = "各種トリガー操作";
            // 
            // splitContainer2
            // 
            this.splitContainer2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer2.Location = new System.Drawing.Point(0, 35);
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
            this.splitContainer2.Size = new System.Drawing.Size(857, 651);
            this.splitContainer2.SplitterDistance = 113;
            this.splitContainer2.TabIndex = 10;
            // 
            // listviewAGV
            // 
            this.listviewAGV.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listviewAGV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listviewAGV.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader1,
            this.columnHeader9});
            this.listviewAGV.FullRowSelect = true;
            this.listviewAGV.GridLines = true;
            this.listviewAGV.HideSelection = false;
            this.listviewAGV.IsStripeColored = false;
            this.listviewAGV.Location = new System.Drawing.Point(0, 0);
            this.listviewAGV.Name = "listviewAGV";
            this.listviewAGV.OwnerDraw = true;
            this.listviewAGV.Size = new System.Drawing.Size(857, 113);
            this.listviewAGV.StripeBackColor1 = System.Drawing.Color.LightSkyBlue;
            this.listviewAGV.StripeBackColor2 = System.Drawing.Color.White;
            this.listviewAGV.TabIndex = 9;
            this.listviewAGV.TitleBackColor = System.Drawing.SystemColors.Control;
            this.listviewAGV.TitleForeColor = System.Drawing.SystemColors.ControlText;
            this.listviewAGV.UseCompatibleStateImageBehavior = false;
            this.listviewAGV.View = System.Windows.Forms.View.Details;
            this.listviewAGV.VirtualMode = true;
            this.listviewAGV.Click += new System.EventHandler(this.listviewAGV_Click);
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "No";
            this.columnHeader3.Width = 0;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "AGV ID";
            this.columnHeader4.Width = 63;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "状態";
            this.columnHeader1.Width = 526;
            // 
            // timerMover
            // 
            this.timerMover.Interval = 1;
            this.timerMover.Tick += new System.EventHandler(this.timerMover_Tick);
            // 
            // SubForm_A4200
            // 
            this._TitleString = "モード動作設定";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1022, 686);
            this.Controls.Add(this.splitContainer1);
            this.EnableSeparation = true;
            this.Name = "SubForm_A4200";
            this.Text = "モード動作設定";
            this.TitleText = "モード動作設定";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panelMap.ResumeLayout(false);
            this.panelMap.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox textBoxCurX;
        private System.Windows.Forms.TextBox textBoxCurY;
        private System.Windows.Forms.TextBox textBoxSleepInterval;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxZoom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBoxX;
        private System.Windows.Forms.TextBox textBoxY;
        private System.Windows.Forms.Label labelX;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private BelicsClass.UI.Controls.BL_DoubleBufferPanel panelMap;
        private System.Windows.Forms.TextBox textBoxFloorCode;
        private BelicsClass.UI.Controls.BL_FlatButton buttonStart;
        private System.Windows.Forms.Label label9;
        private BelicsClass.UI.Controls.BL_FlatButton buttonTrg;
        private BelicsClass.UI.Controls.BL_FlatButton buttonKeepState;
        private BelicsClass.UI.Controls.BL_FlatButton buttonStuck;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private BelicsClass.UI.Controls.BL_VirtualListView listviewFloor;
        private System.Windows.Forms.ColumnHeader columnHeader10;
        private System.Windows.Forms.ColumnHeader columnHeader11;
        private System.Windows.Forms.ColumnHeader columnHeader12;
        private BelicsClass.UI.Controls.BL_VirtualListView listviewMode;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private BelicsClass.UI.Controls.BL_VirtualListView listviewTrg;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private BelicsClass.UI.Controls.BL_VirtualListView listviewAGV;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.Timer timerMover;
    }
}