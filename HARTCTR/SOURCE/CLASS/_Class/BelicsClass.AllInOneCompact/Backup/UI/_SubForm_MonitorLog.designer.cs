namespace BelicsClass.UI
{
    partial class BL_SubForm_MonitorLog
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
            this.lvwLog = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.lvwThread = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lvwLog
            // 
            this.lvwLog.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader3,
            this.columnHeader2});
            this.lvwLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwLog.FullRowSelect = true;
            this.lvwLog.GridLines = true;
            this.lvwLog.HideSelection = false;
            this.lvwLog.Location = new System.Drawing.Point(0, 0);
            this.lvwLog.Name = "lvwLog";
            this.lvwLog.OwnerDraw = true;
            this.lvwLog.Size = new System.Drawing.Size(1022, 473);
            this.lvwLog.TabIndex = 0;
            this.lvwLog.UseCompatibleStateImageBehavior = false;
            this.lvwLog.View = System.Windows.Forms.View.Details;
            this.lvwLog.VirtualMode = true;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "日時";
            this.columnHeader1.Width = 230;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "発生モジュール名";
            this.columnHeader3.Width = 156;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "通信内容";
            this.columnHeader2.Width = 610;
            // 
            // lvwThread
            // 
            this.lvwThread.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader4,
            this.columnHeader5,
            this.columnHeader6,
            this.columnHeader7,
            this.columnHeader8,
            this.columnHeader9});
            this.lvwThread.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lvwThread.FullRowSelect = true;
            this.lvwThread.GridLines = true;
            this.lvwThread.HideSelection = false;
            this.lvwThread.Location = new System.Drawing.Point(0, 0);
            this.lvwThread.Name = "lvwThread";
            this.lvwThread.OwnerDraw = true;
            this.lvwThread.Size = new System.Drawing.Size(1022, 209);
            this.lvwThread.TabIndex = 0;
            this.lvwThread.UseCompatibleStateImageBehavior = false;
            this.lvwThread.View = System.Windows.Forms.View.Details;
            this.lvwThread.VirtualMode = true;
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "スレッド名";
            this.columnHeader4.Width = 517;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "ステップ";
            this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader5.Width = 106;
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "処理回数／sec";
            this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader6.Width = 129;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "スキャンタイム";
            this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeader7.Width = 108;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "終了許可";
            this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader8.Width = 76;
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "処理中";
            this.columnHeader9.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.columnHeader9.Width = 61;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.lvwLog);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.lvwThread);
            this.splitContainer1.Size = new System.Drawing.Size(1022, 686);
            this.splitContainer1.SplitterDistance = 473;
            this.splitContainer1.TabIndex = 1;
            // 
            // BL_SubForm_MonitorLog
            // 
            this._TitleString = "ログ表示";
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.ClientSize = new System.Drawing.Size(1022, 686);
            this.Controls.Add(this.splitContainer1);
            this.EnableSeparation = true;
            this.Name = "BL_SubForm_MonitorLog";
            this.Text = "ログ表示";
            this.TitleText = "ログ表示";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BelicsClass.UI.Controls.BL_VirtualListView lvwLog;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private Controls.BL_VirtualListView lvwThread;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
    }
}
