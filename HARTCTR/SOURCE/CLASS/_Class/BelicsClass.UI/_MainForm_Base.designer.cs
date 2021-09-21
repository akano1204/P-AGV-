namespace BelicsClass.UI
{
    /// <summary>
    /// メインフォームの基本クラスです。
    /// </summary>
    partial class BL_MainForm_Base
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
            this.components = new System.ComponentModel.Container();
            this.labelTitle = new System.Windows.Forms.Label();
            this.buttonFunction01 = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonFunction02 = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonFunction03 = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonFunction04 = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonFunction05 = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonFunction06 = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonFunction07 = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonFunction08 = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonFunction09 = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonFunction10 = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonFunction11 = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonFunction12 = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonFunction00 = new BelicsClass.UI.Controls.BL_FlatButton();
            this.timer100ms = new System.Windows.Forms.Timer(this.components);
            this.panelSubForm = new System.Windows.Forms.Panel();
            this.notifyIconMinimum = new System.Windows.Forms.NotifyIcon(this.components);
            this.timerFunctionEnabler = new System.Windows.Forms.Timer(this.components);
            this.labelTimenow = new System.Windows.Forms.Label();
            this.timer1Second = new System.Windows.Forms.Timer(this.components);
            this.labelOther = new System.Windows.Forms.Label();
            this.flatButtonMinimum = new BelicsClass.UI.Controls.BL_FlatButton();
            this.flatButtonExit = new BelicsClass.UI.Controls.BL_FlatButton();
            this.flatButtonSeparate = new BelicsClass.UI.Controls.BL_FlatButton();
            this.SuspendLayout();
            // 
            // labelTitle
            // 
            this.labelTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelTitle.BackColor = System.Drawing.SystemColors.Highlight;
            this.labelTitle.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.labelTitle.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.labelTitle.Location = new System.Drawing.Point(163, 0);
            this.labelTitle.Name = "labelTitle";
            this.labelTitle.Size = new System.Drawing.Size(696, 40);
            this.labelTitle.TabIndex = 14;
            this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTitle.DoubleClick += new System.EventHandler(this.MainForm_Base_Title_DoubleClick);
            this.labelTitle.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labelTitle_MouseDown);
            this.labelTitle.MouseMove += new System.Windows.Forms.MouseEventHandler(this.labelTitle_MouseMove);
            this.labelTitle.MouseUp += new System.Windows.Forms.MouseEventHandler(this.labelTitle_MouseUp);
            // 
            // buttonFunction01
            // 
            this.buttonFunction01.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFunction01.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFunction01.BackColorNormal = System.Drawing.SystemColors.Control;
            this.buttonFunction01.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonFunction01.BackColorON = System.Drawing.Color.Lime;
            this.buttonFunction01.Checked = false;
            this.buttonFunction01.CheckMode = false;
            this.buttonFunction01.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFunction01.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFunction01.Font = new System.Drawing.Font("Meiryo UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFunction01.ForeColor = System.Drawing.Color.Black;
            this.buttonFunction01.ForeColorNormal = System.Drawing.Color.Black;
            this.buttonFunction01.ForeColorOFF = System.Drawing.Color.Silver;
            this.buttonFunction01.ForeColorON = System.Drawing.Color.Black;
            this.buttonFunction01.Location = new System.Drawing.Point(93, 727);
            this.buttonFunction01.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFunction01.Name = "buttonFunction01";
            this.buttonFunction01.Size = new System.Drawing.Size(76, 40);
            this.buttonFunction01.TabIndex = 2;
            this.buttonFunction01.TabStop = false;
            this.buttonFunction01.Tag = false;
            this.buttonFunction01.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFunction01.UseVisualStyleBackColor = false;
            // 
            // buttonFunction02
            // 
            this.buttonFunction02.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFunction02.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFunction02.BackColorNormal = System.Drawing.SystemColors.Control;
            this.buttonFunction02.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonFunction02.BackColorON = System.Drawing.Color.Lime;
            this.buttonFunction02.Checked = false;
            this.buttonFunction02.CheckMode = false;
            this.buttonFunction02.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFunction02.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFunction02.Font = new System.Drawing.Font("Meiryo UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFunction02.ForeColor = System.Drawing.Color.Black;
            this.buttonFunction02.ForeColorNormal = System.Drawing.Color.Black;
            this.buttonFunction02.ForeColorOFF = System.Drawing.Color.Silver;
            this.buttonFunction02.ForeColorON = System.Drawing.Color.Black;
            this.buttonFunction02.Location = new System.Drawing.Point(168, 727);
            this.buttonFunction02.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFunction02.Name = "buttonFunction02";
            this.buttonFunction02.Size = new System.Drawing.Size(76, 40);
            this.buttonFunction02.TabIndex = 3;
            this.buttonFunction02.TabStop = false;
            this.buttonFunction02.Tag = false;
            this.buttonFunction02.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFunction02.UseVisualStyleBackColor = false;
            // 
            // buttonFunction03
            // 
            this.buttonFunction03.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFunction03.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFunction03.BackColorNormal = System.Drawing.SystemColors.Control;
            this.buttonFunction03.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonFunction03.BackColorON = System.Drawing.Color.Lime;
            this.buttonFunction03.Checked = false;
            this.buttonFunction03.CheckMode = false;
            this.buttonFunction03.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFunction03.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFunction03.Font = new System.Drawing.Font("Meiryo UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFunction03.ForeColor = System.Drawing.Color.Black;
            this.buttonFunction03.ForeColorNormal = System.Drawing.Color.Black;
            this.buttonFunction03.ForeColorOFF = System.Drawing.Color.Silver;
            this.buttonFunction03.ForeColorON = System.Drawing.Color.Black;
            this.buttonFunction03.Location = new System.Drawing.Point(243, 727);
            this.buttonFunction03.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFunction03.Name = "buttonFunction03";
            this.buttonFunction03.Size = new System.Drawing.Size(76, 40);
            this.buttonFunction03.TabIndex = 4;
            this.buttonFunction03.TabStop = false;
            this.buttonFunction03.Tag = false;
            this.buttonFunction03.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFunction03.UseVisualStyleBackColor = false;
            // 
            // buttonFunction04
            // 
            this.buttonFunction04.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFunction04.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFunction04.BackColorNormal = System.Drawing.SystemColors.Control;
            this.buttonFunction04.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonFunction04.BackColorON = System.Drawing.Color.Lime;
            this.buttonFunction04.Checked = false;
            this.buttonFunction04.CheckMode = false;
            this.buttonFunction04.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFunction04.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFunction04.Font = new System.Drawing.Font("Meiryo UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFunction04.ForeColor = System.Drawing.Color.Black;
            this.buttonFunction04.ForeColorNormal = System.Drawing.Color.Black;
            this.buttonFunction04.ForeColorOFF = System.Drawing.Color.Silver;
            this.buttonFunction04.ForeColorON = System.Drawing.Color.Black;
            this.buttonFunction04.Location = new System.Drawing.Point(318, 727);
            this.buttonFunction04.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFunction04.Name = "buttonFunction04";
            this.buttonFunction04.Size = new System.Drawing.Size(76, 40);
            this.buttonFunction04.TabIndex = 5;
            this.buttonFunction04.TabStop = false;
            this.buttonFunction04.Tag = false;
            this.buttonFunction04.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFunction04.UseVisualStyleBackColor = false;
            // 
            // buttonFunction05
            // 
            this.buttonFunction05.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFunction05.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFunction05.BackColorNormal = System.Drawing.SystemColors.Control;
            this.buttonFunction05.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonFunction05.BackColorON = System.Drawing.Color.Lime;
            this.buttonFunction05.Checked = false;
            this.buttonFunction05.CheckMode = false;
            this.buttonFunction05.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFunction05.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFunction05.Font = new System.Drawing.Font("Meiryo UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFunction05.ForeColor = System.Drawing.Color.Black;
            this.buttonFunction05.ForeColorNormal = System.Drawing.Color.Black;
            this.buttonFunction05.ForeColorOFF = System.Drawing.Color.Silver;
            this.buttonFunction05.ForeColorON = System.Drawing.Color.Black;
            this.buttonFunction05.Location = new System.Drawing.Point(404, 727);
            this.buttonFunction05.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFunction05.Name = "buttonFunction05";
            this.buttonFunction05.Size = new System.Drawing.Size(76, 40);
            this.buttonFunction05.TabIndex = 6;
            this.buttonFunction05.TabStop = false;
            this.buttonFunction05.Tag = false;
            this.buttonFunction05.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFunction05.UseVisualStyleBackColor = false;
            // 
            // buttonFunction06
            // 
            this.buttonFunction06.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFunction06.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFunction06.BackColorNormal = System.Drawing.SystemColors.Control;
            this.buttonFunction06.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonFunction06.BackColorON = System.Drawing.Color.Lime;
            this.buttonFunction06.Checked = false;
            this.buttonFunction06.CheckMode = false;
            this.buttonFunction06.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFunction06.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFunction06.Font = new System.Drawing.Font("Meiryo UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFunction06.ForeColor = System.Drawing.Color.Black;
            this.buttonFunction06.ForeColorNormal = System.Drawing.Color.Black;
            this.buttonFunction06.ForeColorOFF = System.Drawing.Color.Silver;
            this.buttonFunction06.ForeColorON = System.Drawing.Color.Black;
            this.buttonFunction06.Location = new System.Drawing.Point(479, 727);
            this.buttonFunction06.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFunction06.Name = "buttonFunction06";
            this.buttonFunction06.Size = new System.Drawing.Size(76, 40);
            this.buttonFunction06.TabIndex = 7;
            this.buttonFunction06.TabStop = false;
            this.buttonFunction06.Tag = false;
            this.buttonFunction06.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFunction06.UseVisualStyleBackColor = false;
            // 
            // buttonFunction07
            // 
            this.buttonFunction07.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFunction07.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFunction07.BackColorNormal = System.Drawing.SystemColors.Control;
            this.buttonFunction07.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonFunction07.BackColorON = System.Drawing.Color.Lime;
            this.buttonFunction07.Checked = false;
            this.buttonFunction07.CheckMode = false;
            this.buttonFunction07.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFunction07.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFunction07.Font = new System.Drawing.Font("Meiryo UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFunction07.ForeColor = System.Drawing.Color.Black;
            this.buttonFunction07.ForeColorNormal = System.Drawing.Color.Black;
            this.buttonFunction07.ForeColorOFF = System.Drawing.Color.Silver;
            this.buttonFunction07.ForeColorON = System.Drawing.Color.Black;
            this.buttonFunction07.Location = new System.Drawing.Point(554, 727);
            this.buttonFunction07.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFunction07.Name = "buttonFunction07";
            this.buttonFunction07.Size = new System.Drawing.Size(76, 40);
            this.buttonFunction07.TabIndex = 8;
            this.buttonFunction07.TabStop = false;
            this.buttonFunction07.Tag = false;
            this.buttonFunction07.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFunction07.UseVisualStyleBackColor = false;
            // 
            // buttonFunction08
            // 
            this.buttonFunction08.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFunction08.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFunction08.BackColorNormal = System.Drawing.SystemColors.Control;
            this.buttonFunction08.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonFunction08.BackColorON = System.Drawing.Color.Lime;
            this.buttonFunction08.Checked = false;
            this.buttonFunction08.CheckMode = false;
            this.buttonFunction08.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFunction08.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFunction08.Font = new System.Drawing.Font("Meiryo UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFunction08.ForeColor = System.Drawing.Color.Black;
            this.buttonFunction08.ForeColorNormal = System.Drawing.Color.Black;
            this.buttonFunction08.ForeColorOFF = System.Drawing.Color.Silver;
            this.buttonFunction08.ForeColorON = System.Drawing.Color.Black;
            this.buttonFunction08.Location = new System.Drawing.Point(629, 727);
            this.buttonFunction08.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFunction08.Name = "buttonFunction08";
            this.buttonFunction08.Size = new System.Drawing.Size(76, 40);
            this.buttonFunction08.TabIndex = 9;
            this.buttonFunction08.TabStop = false;
            this.buttonFunction08.Tag = false;
            this.buttonFunction08.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFunction08.UseVisualStyleBackColor = false;
            // 
            // buttonFunction09
            // 
            this.buttonFunction09.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFunction09.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFunction09.BackColorNormal = System.Drawing.SystemColors.Control;
            this.buttonFunction09.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonFunction09.BackColorON = System.Drawing.Color.Lime;
            this.buttonFunction09.Checked = false;
            this.buttonFunction09.CheckMode = false;
            this.buttonFunction09.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFunction09.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFunction09.Font = new System.Drawing.Font("Meiryo UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFunction09.ForeColor = System.Drawing.Color.Black;
            this.buttonFunction09.ForeColorNormal = System.Drawing.Color.Black;
            this.buttonFunction09.ForeColorOFF = System.Drawing.Color.Silver;
            this.buttonFunction09.ForeColorON = System.Drawing.Color.Black;
            this.buttonFunction09.Location = new System.Drawing.Point(715, 727);
            this.buttonFunction09.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFunction09.Name = "buttonFunction09";
            this.buttonFunction09.Size = new System.Drawing.Size(76, 40);
            this.buttonFunction09.TabIndex = 10;
            this.buttonFunction09.TabStop = false;
            this.buttonFunction09.Tag = false;
            this.buttonFunction09.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFunction09.UseVisualStyleBackColor = false;
            // 
            // buttonFunction10
            // 
            this.buttonFunction10.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFunction10.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFunction10.BackColorNormal = System.Drawing.SystemColors.Control;
            this.buttonFunction10.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonFunction10.BackColorON = System.Drawing.Color.Lime;
            this.buttonFunction10.Checked = false;
            this.buttonFunction10.CheckMode = false;
            this.buttonFunction10.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFunction10.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFunction10.Font = new System.Drawing.Font("Meiryo UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFunction10.ForeColor = System.Drawing.Color.Black;
            this.buttonFunction10.ForeColorNormal = System.Drawing.Color.Black;
            this.buttonFunction10.ForeColorOFF = System.Drawing.Color.Silver;
            this.buttonFunction10.ForeColorON = System.Drawing.Color.Black;
            this.buttonFunction10.Location = new System.Drawing.Point(790, 727);
            this.buttonFunction10.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFunction10.Name = "buttonFunction10";
            this.buttonFunction10.Size = new System.Drawing.Size(76, 40);
            this.buttonFunction10.TabIndex = 11;
            this.buttonFunction10.TabStop = false;
            this.buttonFunction10.Tag = false;
            this.buttonFunction10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFunction10.UseVisualStyleBackColor = false;
            // 
            // buttonFunction11
            // 
            this.buttonFunction11.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFunction11.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFunction11.BackColorNormal = System.Drawing.SystemColors.Control;
            this.buttonFunction11.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonFunction11.BackColorON = System.Drawing.Color.Lime;
            this.buttonFunction11.Checked = false;
            this.buttonFunction11.CheckMode = false;
            this.buttonFunction11.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFunction11.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFunction11.Font = new System.Drawing.Font("Meiryo UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFunction11.ForeColor = System.Drawing.Color.Black;
            this.buttonFunction11.ForeColorNormal = System.Drawing.Color.Black;
            this.buttonFunction11.ForeColorOFF = System.Drawing.Color.Silver;
            this.buttonFunction11.ForeColorON = System.Drawing.Color.Black;
            this.buttonFunction11.Location = new System.Drawing.Point(865, 727);
            this.buttonFunction11.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFunction11.Name = "buttonFunction11";
            this.buttonFunction11.Size = new System.Drawing.Size(76, 40);
            this.buttonFunction11.TabIndex = 12;
            this.buttonFunction11.TabStop = false;
            this.buttonFunction11.Tag = false;
            this.buttonFunction11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFunction11.UseVisualStyleBackColor = false;
            // 
            // buttonFunction12
            // 
            this.buttonFunction12.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFunction12.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFunction12.BackColorNormal = System.Drawing.SystemColors.Control;
            this.buttonFunction12.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonFunction12.BackColorON = System.Drawing.Color.Lime;
            this.buttonFunction12.Checked = false;
            this.buttonFunction12.CheckMode = false;
            this.buttonFunction12.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFunction12.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFunction12.Font = new System.Drawing.Font("Meiryo UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFunction12.ForeColor = System.Drawing.Color.Black;
            this.buttonFunction12.ForeColorNormal = System.Drawing.Color.Black;
            this.buttonFunction12.ForeColorOFF = System.Drawing.Color.Silver;
            this.buttonFunction12.ForeColorON = System.Drawing.Color.Black;
            this.buttonFunction12.Location = new System.Drawing.Point(940, 727);
            this.buttonFunction12.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFunction12.Name = "buttonFunction12";
            this.buttonFunction12.Size = new System.Drawing.Size(76, 40);
            this.buttonFunction12.TabIndex = 13;
            this.buttonFunction12.TabStop = false;
            this.buttonFunction12.Tag = false;
            this.buttonFunction12.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFunction12.UseVisualStyleBackColor = false;
            // 
            // buttonFunction00
            // 
            this.buttonFunction00.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.buttonFunction00.BackColor = System.Drawing.SystemColors.Control;
            this.buttonFunction00.BackColorNormal = System.Drawing.SystemColors.Control;
            this.buttonFunction00.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonFunction00.BackColorON = System.Drawing.Color.Lime;
            this.buttonFunction00.Checked = false;
            this.buttonFunction00.CheckMode = false;
            this.buttonFunction00.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonFunction00.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonFunction00.Font = new System.Drawing.Font("Meiryo UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFunction00.ForeColor = System.Drawing.Color.Black;
            this.buttonFunction00.ForeColorNormal = System.Drawing.Color.Black;
            this.buttonFunction00.ForeColorOFF = System.Drawing.Color.Silver;
            this.buttonFunction00.ForeColorON = System.Drawing.Color.Black;
            this.buttonFunction00.Location = new System.Drawing.Point(8, 727);
            this.buttonFunction00.Margin = new System.Windows.Forms.Padding(0);
            this.buttonFunction00.Name = "buttonFunction00";
            this.buttonFunction00.Size = new System.Drawing.Size(76, 40);
            this.buttonFunction00.TabIndex = 3;
            this.buttonFunction00.TabStop = false;
            this.buttonFunction00.Tag = false;
            this.buttonFunction00.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.buttonFunction00.UseVisualStyleBackColor = false;
            // 
            // timer100ms
            // 
            this.timer100ms.Enabled = true;
            this.timer100ms.Tick += new System.EventHandler(this.MainForm_Base_timer100ms_Tick);
            // 
            // panelSubForm
            // 
            this.panelSubForm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelSubForm.BackColor = System.Drawing.SystemColors.Control;
            this.panelSubForm.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.panelSubForm.Location = new System.Drawing.Point(1, 41);
            this.panelSubForm.Name = "panelSubForm";
            this.panelSubForm.Size = new System.Drawing.Size(1022, 686);
            this.panelSubForm.TabIndex = 0;
            this.panelSubForm.ControlAdded += new System.Windows.Forms.ControlEventHandler(this.panelSubForm_ControlAdded);
            this.panelSubForm.ControlRemoved += new System.Windows.Forms.ControlEventHandler(this.panelSubForm_ControlRemoved);
            this.panelSubForm.Resize += new System.EventHandler(this.panelSubForm_Resize);
            // 
            // notifyIconMinimum
            // 
            this.notifyIconMinimum.BalloonTipClicked += new System.EventHandler(this.MainForm_Base_notifyIconMinimum_BalloonTipClicked);
            this.notifyIconMinimum.BalloonTipClosed += new System.EventHandler(this.MainForm_Base_notifyIconMinimum_BalloonTipClosed);
            this.notifyIconMinimum.BalloonTipShown += new System.EventHandler(this.MainForm_Base_notifyIconMinimum_BalloonTipShown);
            this.notifyIconMinimum.MouseClick += new System.Windows.Forms.MouseEventHandler(this.MainForm_Base_notifyIconMinimum_MouseClick);
            this.notifyIconMinimum.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.MainForm_Base_notifyIconMinimum_MouseDoubleClick);
            // 
            // timerFunctionEnabler
            // 
            this.timerFunctionEnabler.Enabled = true;
            this.timerFunctionEnabler.Interval = 250;
            this.timerFunctionEnabler.Tick += new System.EventHandler(this.timerFunctionEnabler_Tick);
            // 
            // labelTimenow
            // 
            this.labelTimenow.BackColor = System.Drawing.SystemColors.Highlight;
            this.labelTimenow.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelTimenow.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.labelTimenow.Location = new System.Drawing.Point(1, 0);
            this.labelTimenow.Name = "labelTimenow";
            this.labelTimenow.Size = new System.Drawing.Size(167, 40);
            this.labelTimenow.TabIndex = 0;
            this.labelTimenow.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.labelTimenow.DoubleClick += new System.EventHandler(this.MainForm_Base_Title_DoubleClick);
            this.labelTimenow.MouseDown += new System.Windows.Forms.MouseEventHandler(this.labelTitle_MouseDown);
            this.labelTimenow.MouseMove += new System.Windows.Forms.MouseEventHandler(this.labelTitle_MouseMove);
            this.labelTimenow.MouseUp += new System.Windows.Forms.MouseEventHandler(this.labelTitle_MouseUp);
            // 
            // timer1Second
            // 
            this.timer1Second.Enabled = true;
            this.timer1Second.Interval = 1000;
            this.timer1Second.Tick += new System.EventHandler(this.MainForm_Base_timer1Second_Tick);
            // 
            // labelOther
            // 
            this.labelOther.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.labelOther.BackColor = System.Drawing.SystemColors.Highlight;
            this.labelOther.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Bold);
            this.labelOther.ForeColor = System.Drawing.SystemColors.HighlightText;
            this.labelOther.Location = new System.Drawing.Point(856, 0);
            this.labelOther.Name = "labelOther";
            this.labelOther.Size = new System.Drawing.Size(167, 40);
            this.labelOther.TabIndex = 0;
            this.labelOther.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // flatButtonMinimum
            // 
            this.flatButtonMinimum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flatButtonMinimum.BackColor = System.Drawing.Color.RoyalBlue;
            this.flatButtonMinimum.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.flatButtonMinimum.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.flatButtonMinimum.BackColorON = System.Drawing.Color.Lime;
            this.flatButtonMinimum.Checked = false;
            this.flatButtonMinimum.CheckMode = false;
            this.flatButtonMinimum.Cursor = System.Windows.Forms.Cursors.Hand;
            this.flatButtonMinimum.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.flatButtonMinimum.FlatAppearance.BorderSize = 2;
            this.flatButtonMinimum.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.flatButtonMinimum.Font = new System.Drawing.Font("Meiryo UI", 14F, System.Drawing.FontStyle.Bold);
            this.flatButtonMinimum.ForeColor = System.Drawing.Color.White;
            this.flatButtonMinimum.ForeColorNormal = System.Drawing.Color.White;
            this.flatButtonMinimum.ForeColorOFF = System.Drawing.Color.White;
            this.flatButtonMinimum.ForeColorON = System.Drawing.Color.Black;
            this.flatButtonMinimum.Location = new System.Drawing.Point(885, 0);
            this.flatButtonMinimum.Name = "flatButtonMinimum";
            this.flatButtonMinimum.Size = new System.Drawing.Size(50, 40);
            this.flatButtonMinimum.TabIndex = 16;
            this.flatButtonMinimum.TabStop = false;
            this.flatButtonMinimum.Tag = false;
            this.flatButtonMinimum.Text = "＿";
            this.flatButtonMinimum.UseVisualStyleBackColor = false;
            this.flatButtonMinimum.Visible = false;
            this.flatButtonMinimum.Click += new System.EventHandler(this.MainForm_Base_flatButtonMinimum_Click);
            // 
            // flatButtonExit
            // 
            this.flatButtonExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flatButtonExit.BackColor = System.Drawing.Color.Red;
            this.flatButtonExit.BackColorNormal = System.Drawing.Color.Red;
            this.flatButtonExit.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.flatButtonExit.BackColorON = System.Drawing.Color.Lime;
            this.flatButtonExit.Checked = false;
            this.flatButtonExit.CheckMode = false;
            this.flatButtonExit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.flatButtonExit.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.flatButtonExit.FlatAppearance.BorderSize = 2;
            this.flatButtonExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.flatButtonExit.Font = new System.Drawing.Font("Meiryo UI", 14F, System.Drawing.FontStyle.Bold);
            this.flatButtonExit.ForeColor = System.Drawing.Color.White;
            this.flatButtonExit.ForeColorNormal = System.Drawing.Color.White;
            this.flatButtonExit.ForeColorOFF = System.Drawing.Color.White;
            this.flatButtonExit.ForeColorON = System.Drawing.Color.Black;
            this.flatButtonExit.Location = new System.Drawing.Point(933, 0);
            this.flatButtonExit.Name = "flatButtonExit";
            this.flatButtonExit.Size = new System.Drawing.Size(90, 40);
            this.flatButtonExit.TabIndex = 17;
            this.flatButtonExit.TabStop = false;
            this.flatButtonExit.Tag = false;
            this.flatButtonExit.Text = "終了";
            this.flatButtonExit.UseVisualStyleBackColor = false;
            this.flatButtonExit.Visible = false;
            this.flatButtonExit.Click += new System.EventHandler(this.MainForm_Base_Exit_Click);
            // 
            // flatButtonSeparate
            // 
            this.flatButtonSeparate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.flatButtonSeparate.BackColor = System.Drawing.Color.RoyalBlue;
            this.flatButtonSeparate.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.flatButtonSeparate.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.flatButtonSeparate.BackColorON = System.Drawing.Color.Lime;
            this.flatButtonSeparate.Checked = false;
            this.flatButtonSeparate.CheckMode = false;
            this.flatButtonSeparate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.flatButtonSeparate.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.flatButtonSeparate.FlatAppearance.BorderSize = 2;
            this.flatButtonSeparate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.flatButtonSeparate.Font = new System.Drawing.Font("Meiryo UI", 10F, System.Drawing.FontStyle.Bold);
            this.flatButtonSeparate.ForeColor = System.Drawing.Color.White;
            this.flatButtonSeparate.ForeColorNormal = System.Drawing.Color.White;
            this.flatButtonSeparate.ForeColorOFF = System.Drawing.Color.White;
            this.flatButtonSeparate.ForeColorON = System.Drawing.Color.Black;
            this.flatButtonSeparate.Location = new System.Drawing.Point(859, 0);
            this.flatButtonSeparate.Name = "flatButtonSeparate";
            this.flatButtonSeparate.Size = new System.Drawing.Size(28, 40);
            this.flatButtonSeparate.TabIndex = 16;
            this.flatButtonSeparate.TabStop = false;
            this.flatButtonSeparate.Tag = false;
            this.flatButtonSeparate.Text = "▷";
            this.flatButtonSeparate.UseVisualStyleBackColor = false;
            this.flatButtonSeparate.Visible = false;
            this.flatButtonSeparate.Click += new System.EventHandler(this.flatButtonSeparate_Click);
            // 
            // BL_MainForm_Base
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.ControlBox = false;
            this.Controls.Add(this.flatButtonSeparate);
            this.Controls.Add(this.labelTitle);
            this.Controls.Add(this.flatButtonMinimum);
            this.Controls.Add(this.flatButtonExit);
            this.Controls.Add(this.labelOther);
            this.Controls.Add(this.labelTimenow);
            this.Controls.Add(this.buttonFunction00);
            this.Controls.Add(this.buttonFunction12);
            this.Controls.Add(this.buttonFunction11);
            this.Controls.Add(this.buttonFunction10);
            this.Controls.Add(this.buttonFunction09);
            this.Controls.Add(this.buttonFunction08);
            this.Controls.Add(this.buttonFunction07);
            this.Controls.Add(this.buttonFunction06);
            this.Controls.Add(this.buttonFunction05);
            this.Controls.Add(this.buttonFunction04);
            this.Controls.Add(this.buttonFunction03);
            this.Controls.Add(this.buttonFunction02);
            this.Controls.Add(this.buttonFunction01);
            this.Controls.Add(this.panelSubForm);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.MinimizeBox = false;
            this.Name = "BL_MainForm_Base";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Activated += new System.EventHandler(this.MainForm_Base_Activated);
            this.Deactivate += new System.EventHandler(this.MainForm_Base_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_Base_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Base_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainForm_Base_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.MainForm_Base_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MainForm_KeyUp);
            this.Resize += new System.EventHandler(this.MainForm_Base_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary></summary>
        public System.Windows.Forms.Label labelTitle;
        /// <summary></summary>
        public BelicsClass.UI.Controls.BL_FlatButton buttonFunction01;
        /// <summary></summary>
        public BelicsClass.UI.Controls.BL_FlatButton buttonFunction02;
        /// <summary></summary>
        public BelicsClass.UI.Controls.BL_FlatButton buttonFunction03;
        /// <summary></summary>
        public BelicsClass.UI.Controls.BL_FlatButton buttonFunction04;
        /// <summary></summary>
        public BelicsClass.UI.Controls.BL_FlatButton buttonFunction05;
        /// <summary></summary>
        public BelicsClass.UI.Controls.BL_FlatButton buttonFunction06;
        /// <summary></summary>
        public BelicsClass.UI.Controls.BL_FlatButton buttonFunction07;
        /// <summary></summary>
        public BelicsClass.UI.Controls.BL_FlatButton buttonFunction08;
        /// <summary></summary>
        public BelicsClass.UI.Controls.BL_FlatButton buttonFunction09;
        /// <summary></summary>
        public BelicsClass.UI.Controls.BL_FlatButton buttonFunction10;
        /// <summary></summary>
        public BelicsClass.UI.Controls.BL_FlatButton buttonFunction11;
        /// <summary></summary>
        public BelicsClass.UI.Controls.BL_FlatButton buttonFunction12;
        /// <summary></summary>
        public BelicsClass.UI.Controls.BL_FlatButton buttonFunction00;
        /// <summary></summary>
        public System.Windows.Forms.NotifyIcon notifyIconMinimum;
        /// <summary></summary>
        public System.Windows.Forms.Panel panelSubForm;
        private System.Windows.Forms.Timer timerFunctionEnabler;
        /// <summary></summary>
        protected System.Windows.Forms.Timer timer1Second;
        /// <summary></summary>
        protected System.Windows.Forms.Timer timer100ms;
        /// <summary></summary>
        public System.Windows.Forms.Label labelOther;
        /// <summary></summary>
        public System.Windows.Forms.Label labelTimenow;
        /// <summary></summary>
        protected Controls.BL_FlatButton flatButtonMinimum;
        /// <summary></summary>
        protected Controls.BL_FlatButton flatButtonExit;
        /// <summary></summary>
        protected Controls.BL_FlatButton flatButtonSeparate;
    }
}
