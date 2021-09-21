namespace BelicsClass.UI.Report
{
    partial class BL_ReportControl_Base
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

        #region コンポーネント デザイナーで生成されたコード

        /// <summary> 
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を 
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BL_ReportControl_Base));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            //this.axAcroPDF1 = new AxAcroPDFLib.AxAcroPDF();
            this.buttonCancel = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonChange = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonPrint = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonSetting = new BelicsClass.UI.Controls.BL_FlatButton();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.panelPrintForm = new System.Windows.Forms.Panel();
            this.printerPreview = new System.Windows.Forms.PrintPreviewControl();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.listBoxPageList = new System.Windows.Forms.CheckedListBox();
            this.comboBoxPrinterList = new System.Windows.Forms.ComboBox();
            this.buttonUnselectAll = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonPrinterSetting = new BelicsClass.UI.Controls.BL_FlatButton();
            this.buttonSelectAll = new BelicsClass.UI.Controls.BL_FlatButton();
            this.timPrintWatcher = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            //((System.ComponentModel.ISupportInitialize)(this.axAcroPDF1)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Margin = new System.Windows.Forms.Padding(4);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            //this.splitContainer1.Panel1.Controls.Add(this.axAcroPDF1);
            this.splitContainer1.Panel1.Controls.Add(this.buttonCancel);
            this.splitContainer1.Panel1.Controls.Add(this.buttonChange);
            this.splitContainer1.Panel1.Controls.Add(this.buttonPrint);
            this.splitContainer1.Panel1.Controls.Add(this.buttonSetting);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);
            this.splitContainer1.Size = new System.Drawing.Size(710, 542);
            this.splitContainer1.SplitterDistance = 40;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 0;
            //// 
            //// axAcroPDF1
            //// 
            //this.axAcroPDF1.Enabled = true;
            //this.axAcroPDF1.Location = new System.Drawing.Point(130, 3);
            //this.axAcroPDF1.Name = "axAcroPDF1";
            //this.axAcroPDF1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axAcroPDF1.OcxState")));
            //this.axAcroPDF1.Size = new System.Drawing.Size(192, 192);
            //this.axAcroPDF1.TabIndex = 3;
            //this.axAcroPDF1.Visible = false;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonCancel.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonCancel.BackColorOFF = System.Drawing.Color.RoyalBlue;
            this.buttonCancel.BackColorON = System.Drawing.Color.Lime;
            this.buttonCancel.Checked = false;
            this.buttonCancel.CheckMode = false;
            this.buttonCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonCancel.Enabled = false;
            this.buttonCancel.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonCancel.FlatAppearance.BorderSize = 2;
            this.buttonCancel.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Lime;
            this.buttonCancel.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green;
            this.buttonCancel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonCancel.ForeColor = System.Drawing.Color.White;
            this.buttonCancel.ForeColorNormal = System.Drawing.Color.White;
            this.buttonCancel.ForeColorOFF = System.Drawing.Color.White;
            this.buttonCancel.ForeColorON = System.Drawing.Color.Black;
            this.buttonCancel.Location = new System.Drawing.Point(461, 3);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(120, 34);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Tag = false;
            this.buttonCancel.Text = "編集キャンセル";
            this.buttonCancel.UseVisualStyleBackColor = false;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonChange
            // 
            this.buttonChange.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonChange.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonChange.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonChange.BackColorOFF = System.Drawing.Color.RoyalBlue;
            this.buttonChange.BackColorON = System.Drawing.Color.Lime;
            this.buttonChange.Checked = false;
            this.buttonChange.CheckMode = true;
            this.buttonChange.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonChange.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonChange.FlatAppearance.BorderSize = 2;
            this.buttonChange.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Lime;
            this.buttonChange.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green;
            this.buttonChange.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonChange.ForeColor = System.Drawing.Color.White;
            this.buttonChange.ForeColorNormal = System.Drawing.Color.White;
            this.buttonChange.ForeColorOFF = System.Drawing.Color.White;
            this.buttonChange.ForeColorON = System.Drawing.Color.Black;
            this.buttonChange.Location = new System.Drawing.Point(335, 3);
            this.buttonChange.Name = "buttonChange";
            this.buttonChange.Size = new System.Drawing.Size(120, 34);
            this.buttonChange.TabIndex = 2;
            this.buttonChange.Tag = false;
            this.buttonChange.Text = "編集";
            this.buttonChange.UseVisualStyleBackColor = false;
            this.buttonChange.Click += new System.EventHandler(this.buttonChange_Click);
            // 
            // buttonPrint
            // 
            this.buttonPrint.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonPrint.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonPrint.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonPrint.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonPrint.BackColorON = System.Drawing.Color.Lime;
            this.buttonPrint.Checked = false;
            this.buttonPrint.CheckMode = false;
            this.buttonPrint.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonPrint.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonPrint.FlatAppearance.BorderSize = 2;
            this.buttonPrint.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Lime;
            this.buttonPrint.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.buttonPrint.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPrint.ForeColor = System.Drawing.Color.White;
            this.buttonPrint.ForeColorNormal = System.Drawing.Color.White;
            this.buttonPrint.ForeColorOFF = System.Drawing.Color.White;
            this.buttonPrint.ForeColorON = System.Drawing.Color.Black;
            this.buttonPrint.Location = new System.Drawing.Point(3, 3);
            this.buttonPrint.Name = "buttonPrint";
            this.buttonPrint.Size = new System.Drawing.Size(120, 34);
            this.buttonPrint.TabIndex = 1;
            this.buttonPrint.Tag = false;
            this.buttonPrint.Text = "印刷";
            this.buttonPrint.UseVisualStyleBackColor = false;
            this.buttonPrint.Click += new System.EventHandler(this.buttonPrint_Click);
            // 
            // buttonSetting
            // 
            this.buttonSetting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSetting.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonSetting.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonSetting.BackColorOFF = System.Drawing.Color.RoyalBlue;
            this.buttonSetting.BackColorON = System.Drawing.Color.Lime;
            this.buttonSetting.Checked = false;
            this.buttonSetting.CheckMode = true;
            this.buttonSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonSetting.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonSetting.FlatAppearance.BorderSize = 2;
            this.buttonSetting.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Lime;
            this.buttonSetting.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Green;
            this.buttonSetting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSetting.ForeColor = System.Drawing.Color.White;
            this.buttonSetting.ForeColorNormal = System.Drawing.SystemColors.ControlText;
            this.buttonSetting.ForeColorOFF = System.Drawing.Color.White;
            this.buttonSetting.ForeColorON = System.Drawing.Color.Black;
            this.buttonSetting.Location = new System.Drawing.Point(587, 3);
            this.buttonSetting.Name = "buttonSetting";
            this.buttonSetting.Size = new System.Drawing.Size(120, 34);
            this.buttonSetting.TabIndex = 0;
            this.buttonSetting.Tag = false;
            this.buttonSetting.Text = "印刷設定";
            this.buttonSetting.UseVisualStyleBackColor = false;
            this.buttonSetting.Click += new System.EventHandler(this.buttonSetting_Click);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer2.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainer2.Location = new System.Drawing.Point(0, 0);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.panelPrintForm);
            this.splitContainer2.Panel1.Controls.Add(this.printerPreview);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.BackColor = System.Drawing.SystemColors.AppWorkspace;
            this.splitContainer2.Panel2.Controls.Add(this.groupBox1);
            this.splitContainer2.Size = new System.Drawing.Size(710, 497);
            this.splitContainer2.SplitterDistance = 453;
            this.splitContainer2.TabIndex = 0;
            // 
            // panelPrintForm
            // 
            this.panelPrintForm.AutoScroll = true;
            this.panelPrintForm.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelPrintForm.ForeColor = System.Drawing.Color.Black;
            this.panelPrintForm.Location = new System.Drawing.Point(85, 140);
            this.panelPrintForm.Name = "panelPrintForm";
            this.panelPrintForm.Size = new System.Drawing.Size(365, 354);
            this.panelPrintForm.TabIndex = 13;
            this.panelPrintForm.Visible = false;
            // 
            // printerPreview
            // 
            this.printerPreview.AutoZoom = false;
            this.printerPreview.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.printerPreview.Location = new System.Drawing.Point(3, 4);
            this.printerPreview.Margin = new System.Windows.Forms.Padding(0);
            this.printerPreview.Name = "printerPreview";
            this.printerPreview.Size = new System.Drawing.Size(402, 312);
            this.printerPreview.TabIndex = 12;
            this.printerPreview.UseAntiAlias = true;
            this.printerPreview.Zoom = 1D;
            this.printerPreview.MouseClick += new System.Windows.Forms.MouseEventHandler(this.printerPreview_MouseClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.groupBox1.Controls.Add(this.listBoxPageList);
            this.groupBox1.Controls.Add(this.comboBoxPrinterList);
            this.groupBox1.Controls.Add(this.buttonUnselectAll);
            this.groupBox1.Controls.Add(this.buttonPrinterSetting);
            this.groupBox1.Controls.Add(this.buttonSelectAll);
            this.groupBox1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.groupBox1.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(4, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(246, 490);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "印刷設定";
            // 
            // listBoxPageList
            // 
            this.listBoxPageList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listBoxPageList.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.listBoxPageList.Cursor = System.Windows.Forms.Cursors.Hand;
            this.listBoxPageList.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.listBoxPageList.ForeColor = System.Drawing.Color.White;
            this.listBoxPageList.IntegralHeight = false;
            this.listBoxPageList.Location = new System.Drawing.Point(7, 136);
            this.listBoxPageList.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.listBoxPageList.Name = "listBoxPageList";
            this.listBoxPageList.Size = new System.Drawing.Size(232, 345);
            this.listBoxPageList.TabIndex = 5;
            this.listBoxPageList.Click += new System.EventHandler(this.OnSelectPage);
            this.listBoxPageList.SelectedIndexChanged += new System.EventHandler(this.OnSelectPage);
            // 
            // comboBoxPrinterList
            // 
            this.comboBoxPrinterList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboBoxPrinterList.BackColor = System.Drawing.Color.RoyalBlue;
            this.comboBoxPrinterList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxPrinterList.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.comboBoxPrinterList.Font = new System.Drawing.Font("Meiryo UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.comboBoxPrinterList.ForeColor = System.Drawing.Color.White;
            this.comboBoxPrinterList.Location = new System.Drawing.Point(7, 66);
            this.comboBoxPrinterList.Margin = new System.Windows.Forms.Padding(4, 6, 4, 6);
            this.comboBoxPrinterList.Name = "comboBoxPrinterList";
            this.comboBoxPrinterList.Size = new System.Drawing.Size(232, 23);
            this.comboBoxPrinterList.TabIndex = 3;
            this.comboBoxPrinterList.SelectedIndexChanged += new System.EventHandler(this.OnSelectPrinter);
            // 
            // buttonUnselectAll
            // 
            this.buttonUnselectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonUnselectAll.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonUnselectAll.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonUnselectAll.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonUnselectAll.BackColorON = System.Drawing.Color.Lime;
            this.buttonUnselectAll.Checked = false;
            this.buttonUnselectAll.CheckMode = false;
            this.buttonUnselectAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonUnselectAll.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonUnselectAll.FlatAppearance.BorderSize = 2;
            this.buttonUnselectAll.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Lime;
            this.buttonUnselectAll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.buttonUnselectAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonUnselectAll.ForeColor = System.Drawing.Color.White;
            this.buttonUnselectAll.ForeColorNormal = System.Drawing.Color.White;
            this.buttonUnselectAll.ForeColorOFF = System.Drawing.Color.White;
            this.buttonUnselectAll.ForeColorON = System.Drawing.Color.Black;
            this.buttonUnselectAll.Location = new System.Drawing.Point(126, 23);
            this.buttonUnselectAll.Name = "buttonUnselectAll";
            this.buttonUnselectAll.Size = new System.Drawing.Size(114, 34);
            this.buttonUnselectAll.TabIndex = 2;
            this.buttonUnselectAll.Tag = false;
            this.buttonUnselectAll.Text = "全解除";
            this.buttonUnselectAll.UseVisualStyleBackColor = false;
            this.buttonUnselectAll.Click += new System.EventHandler(this.buttonUnselectAll_Click);
            // 
            // buttonPrinterSetting
            // 
            this.buttonPrinterSetting.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonPrinterSetting.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonPrinterSetting.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonPrinterSetting.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonPrinterSetting.BackColorON = System.Drawing.Color.Lime;
            this.buttonPrinterSetting.Checked = false;
            this.buttonPrinterSetting.CheckMode = false;
            this.buttonPrinterSetting.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonPrinterSetting.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonPrinterSetting.FlatAppearance.BorderSize = 2;
            this.buttonPrinterSetting.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Lime;
            this.buttonPrinterSetting.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.buttonPrinterSetting.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonPrinterSetting.ForeColor = System.Drawing.Color.White;
            this.buttonPrinterSetting.ForeColorNormal = System.Drawing.Color.White;
            this.buttonPrinterSetting.ForeColorOFF = System.Drawing.Color.White;
            this.buttonPrinterSetting.ForeColorON = System.Drawing.Color.Black;
            this.buttonPrinterSetting.Location = new System.Drawing.Point(7, 98);
            this.buttonPrinterSetting.Name = "buttonPrinterSetting";
            this.buttonPrinterSetting.Size = new System.Drawing.Size(232, 34);
            this.buttonPrinterSetting.TabIndex = 2;
            this.buttonPrinterSetting.Tag = false;
            this.buttonPrinterSetting.Text = "プリンタ設定";
            this.buttonPrinterSetting.UseVisualStyleBackColor = false;
            this.buttonPrinterSetting.Click += new System.EventHandler(this.buttonPrinterSetting_Click);
            // 
            // buttonSelectAll
            // 
            this.buttonSelectAll.BackColor = System.Drawing.Color.RoyalBlue;
            this.buttonSelectAll.BackColorNormal = System.Drawing.Color.RoyalBlue;
            this.buttonSelectAll.BackColorOFF = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.buttonSelectAll.BackColorON = System.Drawing.Color.Lime;
            this.buttonSelectAll.Checked = false;
            this.buttonSelectAll.CheckMode = false;
            this.buttonSelectAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.buttonSelectAll.FlatAppearance.BorderColor = System.Drawing.Color.Black;
            this.buttonSelectAll.FlatAppearance.BorderSize = 2;
            this.buttonSelectAll.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Lime;
            this.buttonSelectAll.FlatAppearance.MouseOverBackColor = System.Drawing.Color.DodgerBlue;
            this.buttonSelectAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonSelectAll.ForeColor = System.Drawing.Color.White;
            this.buttonSelectAll.ForeColorNormal = System.Drawing.Color.White;
            this.buttonSelectAll.ForeColorOFF = System.Drawing.Color.White;
            this.buttonSelectAll.ForeColorON = System.Drawing.Color.Black;
            this.buttonSelectAll.Location = new System.Drawing.Point(6, 23);
            this.buttonSelectAll.Name = "buttonSelectAll";
            this.buttonSelectAll.Size = new System.Drawing.Size(114, 34);
            this.buttonSelectAll.TabIndex = 2;
            this.buttonSelectAll.Tag = false;
            this.buttonSelectAll.Text = "全選択";
            this.buttonSelectAll.UseVisualStyleBackColor = false;
            this.buttonSelectAll.Click += new System.EventHandler(this.buttonSelectAll_Click);
            // 
            // timPrintWatcher
            // 
            this.timPrintWatcher.Interval = 500;
            this.timPrintWatcher.Tick += new System.EventHandler(this.timPrintWatcher_Tick);
            // 
            // BL_ReportControl_Base
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "BL_ReportControl_Base";
            this.Size = new System.Drawing.Size(710, 542);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            //((System.ComponentModel.ISupportInitialize)(this.axAcroPDF1)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        /// <summary></summary>
        protected System.Windows.Forms.Panel panelPrintForm;
        /// <summary></summary>
        protected System.Windows.Forms.PrintPreviewControl printerPreview;
        /// <summary></summary>
        protected System.Windows.Forms.ComboBox comboBoxPrinterList;
        /// <summary></summary>
        protected System.Windows.Forms.CheckedListBox listBoxPageList;
        private System.Windows.Forms.Timer timPrintWatcher;
        /// <summary></summary>
        protected System.Windows.Forms.SplitContainer splitContainer1;
        /// <summary></summary>
        protected System.Windows.Forms.SplitContainer splitContainer2;
        /// <summary></summary>
        protected Controls.BL_FlatButton buttonPrint;
        /// <summary></summary>
        protected Controls.BL_FlatButton buttonSetting;
        /// <summary></summary>
        protected Controls.BL_FlatButton buttonChange;
        /// <summary></summary>
        protected Controls.BL_FlatButton buttonCancel;
        /// <summary></summary>
        protected Controls.BL_FlatButton buttonUnselectAll;
        /// <summary></summary>
        protected Controls.BL_FlatButton buttonSelectAll;
        /// <summary></summary>
        protected Controls.BL_FlatButton buttonPrinterSetting;
        ///// <summary></summary>
        //protected AxAcroPDFLib.AxAcroPDF axAcroPDF1;







    }
}
