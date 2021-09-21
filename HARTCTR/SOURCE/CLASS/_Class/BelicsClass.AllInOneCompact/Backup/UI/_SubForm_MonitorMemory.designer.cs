namespace BelicsClass.UI
{
    partial class BL_SubForm_MonitorMemory
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
            this.lvwField = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.checkBoxBoolean = new System.Windows.Forms.CheckBox();
            this.textBoxUpdateValue = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvwField
            // 
            this.lvwField.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6});
            this.lvwField.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwField.Font = new System.Drawing.Font("Meiryo UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lvwField.FullRowSelect = true;
            this.lvwField.GridLines = true;
            this.lvwField.HideSelection = false;
            this.lvwField.Location = new System.Drawing.Point(0, 0);
            this.lvwField.Name = "lvwField";
            this.lvwField.OwnerDraw = true;
            this.lvwField.Size = new System.Drawing.Size(835, 628);
            this.lvwField.TabIndex = 0;
            this.lvwField.UseCompatibleStateImageBehavior = false;
            this.lvwField.View = System.Windows.Forms.View.Details;
            this.lvwField.VirtualMode = true;
            this.lvwField.SelectedIndexChanged += new System.EventHandler(this.lvwField_SelectedIndexChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "";
            this.columnHeader1.Width = 0;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "No";
            this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader2.Width = 50;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "フィールド名";
            this.columnHeader3.Width = 500;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "値";
            this.columnHeader4.Width = 180;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "データ長";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "バイト配列イメージ";
            this.columnHeader6.Width = 800;
            // 
            // checkBoxBoolean
            // 
            this.checkBoxBoolean.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.checkBoxBoolean.AutoSize = true;
            this.checkBoxBoolean.Enabled = false;
            this.checkBoxBoolean.Location = new System.Drawing.Point(100, 648);
            this.checkBoxBoolean.Name = "checkBoxBoolean";
            this.checkBoxBoolean.Size = new System.Drawing.Size(66, 24);
            this.checkBoxBoolean.TabIndex = 1;
            this.checkBoxBoolean.Text = "False";
            this.checkBoxBoolean.UseVisualStyleBackColor = true;
            this.checkBoxBoolean.CheckedChanged += new System.EventHandler(this.checkBoxBoolean_CheckedChanged);
            // 
            // textBoxUpdateValue
            // 
            this.textBoxUpdateValue.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxUpdateValue.Enabled = false;
            this.textBoxUpdateValue.Location = new System.Drawing.Point(172, 646);
            this.textBoxUpdateValue.Name = "textBoxUpdateValue";
            this.textBoxUpdateValue.Size = new System.Drawing.Size(838, 28);
            this.textBoxUpdateValue.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 649);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(83, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "更新する値";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeView1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvwField);
            this.splitContainer1.Size = new System.Drawing.Size(998, 628);
            this.splitContainer1.SplitterDistance = 159;
            this.splitContainer1.TabIndex = 5;
            // 
            // treeView1
            // 
            this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeView1.Font = new System.Drawing.Font("Meiryo UI", 9.75F);
            this.treeView1.FullRowSelect = true;
            this.treeView1.HideSelection = false;
            this.treeView1.Location = new System.Drawing.Point(0, 0);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(159, 628);
            this.treeView1.TabIndex = 5;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // BL_SubForm_MonitorMemory
            // 
            this._TitleString = "共有メモリ表示";
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.ClientSize = new System.Drawing.Size(1022, 686);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxUpdateValue);
            this.Controls.Add(this.checkBoxBoolean);
            this.EnableSeparation = true;
            this.Name = "BL_SubForm_MonitorMemory";
            this.Text = "共有メモリ表示";
            this.TitleText = "共有メモリ表示";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        /// <summary></summary>
        protected Controls.BL_VirtualListView lvwField;
        /// <summary></summary>
        protected System.Windows.Forms.CheckBox checkBoxBoolean;
        /// <summary></summary>
        protected System.Windows.Forms.TextBox textBoxUpdateValue;
        /// <summary></summary>
        protected System.Windows.Forms.Label label1;
        /// <summary></summary>
        protected System.Windows.Forms.TreeView treeView1;
        /// <summary></summary>
        protected System.Windows.Forms.SplitContainer splitContainer1;

    }
}
