namespace BelicsClass.UI.Graph
{
    partial class BL_Graph
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
            this.splitContainerHeader = new System.Windows.Forms.SplitContainer();
            this.panelHeader = new BelicsClass.UI.Controls.BL_DoubleBufferPanel();
            this.splitContainerFooter = new System.Windows.Forms.SplitContainer();
            this.splitContainerAxisBottom = new System.Windows.Forms.SplitContainer();
            this.splitContainerAxisLeft = new System.Windows.Forms.SplitContainer();
            this.panelAxisLeft = new BelicsClass.UI.Controls.BL_DoubleBufferPanel();
            this.splitContainerAxisRight = new System.Windows.Forms.SplitContainer();
            this.splitContainerPlotLeft = new System.Windows.Forms.SplitContainer();
            this.splitContainerPlotLeftTop = new System.Windows.Forms.SplitContainer();
            this.splitContainerPlotLeftBottom = new System.Windows.Forms.SplitContainer();
            this.trackBarY = new System.Windows.Forms.TrackBar();
            this.splitContainerPlotRight = new System.Windows.Forms.SplitContainer();
            this.splitContainerPlotTop = new System.Windows.Forms.SplitContainer();
            this.trackBarX = new System.Windows.Forms.TrackBar();
            this.splitContainerPlotBottom = new System.Windows.Forms.SplitContainer();
            this.panelPlot = new BelicsClass.UI.Controls.BL_DoubleBufferPanel();
            this.panelCursorX = new BelicsClass.UI.Controls.BL_DoubleBufferPanel();
            this.checkBoxFixedX = new System.Windows.Forms.CheckBox();
            this.scrollBarX = new System.Windows.Forms.HScrollBar();
            this.splitContainerPlotRightTop = new System.Windows.Forms.SplitContainer();
            this.splitContainerPlotRightBottom = new System.Windows.Forms.SplitContainer();
            this.checkBoxFixedY = new System.Windows.Forms.CheckBox();
            this.scrollBarY = new System.Windows.Forms.VScrollBar();
            this.panelAxisRight = new BelicsClass.UI.Controls.BL_DoubleBufferPanel();
            this.splitContainerAxisBottomLeft = new System.Windows.Forms.SplitContainer();
            this.splitContainerAxisBottomRight = new System.Windows.Forms.SplitContainer();
            this.panelAxisBottom = new BelicsClass.UI.Controls.BL_DoubleBufferPanel();
            this.panelFooter = new BelicsClass.UI.Controls.BL_DoubleBufferPanel();
            this.splitContainerHeader.Panel1.SuspendLayout();
            this.splitContainerHeader.Panel2.SuspendLayout();
            this.splitContainerHeader.SuspendLayout();
            this.splitContainerFooter.Panel1.SuspendLayout();
            this.splitContainerFooter.Panel2.SuspendLayout();
            this.splitContainerFooter.SuspendLayout();
            this.splitContainerAxisBottom.Panel1.SuspendLayout();
            this.splitContainerAxisBottom.Panel2.SuspendLayout();
            this.splitContainerAxisBottom.SuspendLayout();
            this.splitContainerAxisLeft.Panel1.SuspendLayout();
            this.splitContainerAxisLeft.Panel2.SuspendLayout();
            this.splitContainerAxisLeft.SuspendLayout();
            this.splitContainerAxisRight.Panel1.SuspendLayout();
            this.splitContainerAxisRight.Panel2.SuspendLayout();
            this.splitContainerAxisRight.SuspendLayout();
            this.splitContainerPlotLeft.Panel1.SuspendLayout();
            this.splitContainerPlotLeft.Panel2.SuspendLayout();
            this.splitContainerPlotLeft.SuspendLayout();
            this.splitContainerPlotLeftTop.Panel2.SuspendLayout();
            this.splitContainerPlotLeftTop.SuspendLayout();
            this.splitContainerPlotLeftBottom.Panel1.SuspendLayout();
            this.splitContainerPlotLeftBottom.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarY)).BeginInit();
            this.splitContainerPlotRight.Panel1.SuspendLayout();
            this.splitContainerPlotRight.Panel2.SuspendLayout();
            this.splitContainerPlotRight.SuspendLayout();
            this.splitContainerPlotTop.Panel1.SuspendLayout();
            this.splitContainerPlotTop.Panel2.SuspendLayout();
            this.splitContainerPlotTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarX)).BeginInit();
            this.splitContainerPlotBottom.Panel1.SuspendLayout();
            this.splitContainerPlotBottom.Panel2.SuspendLayout();
            this.splitContainerPlotBottom.SuspendLayout();
            this.panelPlot.SuspendLayout();
            this.splitContainerPlotRightTop.Panel2.SuspendLayout();
            this.splitContainerPlotRightTop.SuspendLayout();
            this.splitContainerPlotRightBottom.Panel1.SuspendLayout();
            this.splitContainerPlotRightBottom.SuspendLayout();
            this.splitContainerAxisBottomLeft.Panel2.SuspendLayout();
            this.splitContainerAxisBottomLeft.SuspendLayout();
            this.splitContainerAxisBottomRight.Panel1.SuspendLayout();
            this.splitContainerAxisBottomRight.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainerHeader
            // 
            this.splitContainerHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerHeader.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerHeader.IsSplitterFixed = true;
            this.splitContainerHeader.Location = new System.Drawing.Point(0, 0);
            this.splitContainerHeader.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerHeader.Name = "splitContainerHeader";
            this.splitContainerHeader.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerHeader.Panel1
            // 
            this.splitContainerHeader.Panel1.Controls.Add(this.panelHeader);
            this.splitContainerHeader.Panel1MinSize = 0;
            // 
            // splitContainerHeader.Panel2
            // 
            this.splitContainerHeader.Panel2.Controls.Add(this.splitContainerFooter);
            this.splitContainerHeader.Panel2MinSize = 0;
            this.splitContainerHeader.Size = new System.Drawing.Size(682, 339);
            this.splitContainerHeader.SplitterDistance = 29;
            this.splitContainerHeader.SplitterWidth = 1;
            this.splitContainerHeader.TabIndex = 0;
            // 
            // panelHeader
            // 
            this.panelHeader.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelHeader.Location = new System.Drawing.Point(0, 0);
            this.panelHeader.Name = "panelHeader";
            this.panelHeader.Size = new System.Drawing.Size(682, 29);
            this.panelHeader.TabIndex = 3;
            // 
            // splitContainerFooter
            // 
            this.splitContainerFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerFooter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerFooter.IsSplitterFixed = true;
            this.splitContainerFooter.Location = new System.Drawing.Point(0, 0);
            this.splitContainerFooter.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerFooter.Name = "splitContainerFooter";
            this.splitContainerFooter.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerFooter.Panel1
            // 
            this.splitContainerFooter.Panel1.Controls.Add(this.splitContainerAxisBottom);
            this.splitContainerFooter.Panel1MinSize = 0;
            // 
            // splitContainerFooter.Panel2
            // 
            this.splitContainerFooter.Panel2.Controls.Add(this.panelFooter);
            this.splitContainerFooter.Panel2MinSize = 0;
            this.splitContainerFooter.Size = new System.Drawing.Size(682, 309);
            this.splitContainerFooter.SplitterDistance = 308;
            this.splitContainerFooter.SplitterWidth = 1;
            this.splitContainerFooter.TabIndex = 1;
            // 
            // splitContainerAxisBottom
            // 
            this.splitContainerAxisBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerAxisBottom.IsSplitterFixed = true;
            this.splitContainerAxisBottom.Location = new System.Drawing.Point(0, 0);
            this.splitContainerAxisBottom.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerAxisBottom.Name = "splitContainerAxisBottom";
            this.splitContainerAxisBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerAxisBottom.Panel1
            // 
            this.splitContainerAxisBottom.Panel1.Controls.Add(this.splitContainerAxisLeft);
            this.splitContainerAxisBottom.Panel1MinSize = 0;
            // 
            // splitContainerAxisBottom.Panel2
            // 
            this.splitContainerAxisBottom.Panel2.Controls.Add(this.splitContainerAxisBottomLeft);
            this.splitContainerAxisBottom.Panel2MinSize = 0;
            this.splitContainerAxisBottom.Size = new System.Drawing.Size(682, 308);
            this.splitContainerAxisBottom.SplitterDistance = 274;
            this.splitContainerAxisBottom.SplitterWidth = 1;
            this.splitContainerAxisBottom.TabIndex = 2;
            // 
            // splitContainerAxisLeft
            // 
            this.splitContainerAxisLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerAxisLeft.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerAxisLeft.IsSplitterFixed = true;
            this.splitContainerAxisLeft.Location = new System.Drawing.Point(0, 0);
            this.splitContainerAxisLeft.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerAxisLeft.Name = "splitContainerAxisLeft";
            // 
            // splitContainerAxisLeft.Panel1
            // 
            this.splitContainerAxisLeft.Panel1.Controls.Add(this.panelAxisLeft);
            this.splitContainerAxisLeft.Panel1MinSize = 0;
            // 
            // splitContainerAxisLeft.Panel2
            // 
            this.splitContainerAxisLeft.Panel2.Controls.Add(this.splitContainerAxisRight);
            this.splitContainerAxisLeft.Panel2MinSize = 0;
            this.splitContainerAxisLeft.Size = new System.Drawing.Size(682, 274);
            this.splitContainerAxisLeft.SplitterDistance = 80;
            this.splitContainerAxisLeft.SplitterWidth = 1;
            this.splitContainerAxisLeft.TabIndex = 3;
            // 
            // panelAxisLeft
            // 
            this.panelAxisLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAxisLeft.Location = new System.Drawing.Point(0, 0);
            this.panelAxisLeft.Name = "panelAxisLeft";
            this.panelAxisLeft.Size = new System.Drawing.Size(80, 274);
            this.panelAxisLeft.TabIndex = 0;
            this.panelAxisLeft.Paint += new System.Windows.Forms.PaintEventHandler(this.panelAxisLeft_Paint);
            // 
            // splitContainerAxisRight
            // 
            this.splitContainerAxisRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerAxisRight.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerAxisRight.IsSplitterFixed = true;
            this.splitContainerAxisRight.Location = new System.Drawing.Point(0, 0);
            this.splitContainerAxisRight.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerAxisRight.Name = "splitContainerAxisRight";
            // 
            // splitContainerAxisRight.Panel1
            // 
            this.splitContainerAxisRight.Panel1.Controls.Add(this.splitContainerPlotLeft);
            this.splitContainerAxisRight.Panel1MinSize = 0;
            // 
            // splitContainerAxisRight.Panel2
            // 
            this.splitContainerAxisRight.Panel2.Controls.Add(this.panelAxisRight);
            this.splitContainerAxisRight.Panel2MinSize = 0;
            this.splitContainerAxisRight.Size = new System.Drawing.Size(601, 274);
            this.splitContainerAxisRight.SplitterDistance = 560;
            this.splitContainerAxisRight.SplitterWidth = 1;
            this.splitContainerAxisRight.TabIndex = 4;
            // 
            // splitContainerPlotLeft
            // 
            this.splitContainerPlotLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerPlotLeft.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerPlotLeft.IsSplitterFixed = true;
            this.splitContainerPlotLeft.Location = new System.Drawing.Point(0, 0);
            this.splitContainerPlotLeft.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerPlotLeft.Name = "splitContainerPlotLeft";
            // 
            // splitContainerPlotLeft.Panel1
            // 
            this.splitContainerPlotLeft.Panel1.Controls.Add(this.splitContainerPlotLeftTop);
            this.splitContainerPlotLeft.Panel1MinSize = 0;
            // 
            // splitContainerPlotLeft.Panel2
            // 
            this.splitContainerPlotLeft.Panel2.Controls.Add(this.splitContainerPlotRight);
            this.splitContainerPlotLeft.Panel2MinSize = 0;
            this.splitContainerPlotLeft.Size = new System.Drawing.Size(560, 274);
            this.splitContainerPlotLeft.SplitterDistance = 24;
            this.splitContainerPlotLeft.SplitterWidth = 1;
            this.splitContainerPlotLeft.TabIndex = 4;
            // 
            // splitContainerPlotLeftTop
            // 
            this.splitContainerPlotLeftTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerPlotLeftTop.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerPlotLeftTop.IsSplitterFixed = true;
            this.splitContainerPlotLeftTop.Location = new System.Drawing.Point(0, 0);
            this.splitContainerPlotLeftTop.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerPlotLeftTop.Name = "splitContainerPlotLeftTop";
            this.splitContainerPlotLeftTop.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainerPlotLeftTop.Panel1MinSize = 0;
            // 
            // splitContainerPlotLeftTop.Panel2
            // 
            this.splitContainerPlotLeftTop.Panel2.Controls.Add(this.splitContainerPlotLeftBottom);
            this.splitContainerPlotLeftTop.Panel2MinSize = 0;
            this.splitContainerPlotLeftTop.Size = new System.Drawing.Size(24, 274);
            this.splitContainerPlotLeftTop.SplitterDistance = 24;
            this.splitContainerPlotLeftTop.SplitterWidth = 1;
            this.splitContainerPlotLeftTop.TabIndex = 6;
            // 
            // splitContainerPlotLeftBottom
            // 
            this.splitContainerPlotLeftBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerPlotLeftBottom.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerPlotLeftBottom.IsSplitterFixed = true;
            this.splitContainerPlotLeftBottom.Location = new System.Drawing.Point(0, 0);
            this.splitContainerPlotLeftBottom.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerPlotLeftBottom.Name = "splitContainerPlotLeftBottom";
            this.splitContainerPlotLeftBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerPlotLeftBottom.Panel1
            // 
            this.splitContainerPlotLeftBottom.Panel1.Controls.Add(this.trackBarY);
            this.splitContainerPlotLeftBottom.Panel1MinSize = 0;
            this.splitContainerPlotLeftBottom.Panel2MinSize = 20;
            this.splitContainerPlotLeftBottom.Size = new System.Drawing.Size(24, 249);
            this.splitContainerPlotLeftBottom.SplitterDistance = 228;
            this.splitContainerPlotLeftBottom.SplitterWidth = 1;
            this.splitContainerPlotLeftBottom.TabIndex = 6;
            // 
            // trackBarY
            // 
            this.trackBarY.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarY.Location = new System.Drawing.Point(0, 0);
            this.trackBarY.Margin = new System.Windows.Forms.Padding(0);
            this.trackBarY.Name = "trackBarY";
            this.trackBarY.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarY.Size = new System.Drawing.Size(24, 228);
            this.trackBarY.TabIndex = 0;
            this.trackBarY.TickStyle = System.Windows.Forms.TickStyle.None;
            // 
            // splitContainerPlotRight
            // 
            this.splitContainerPlotRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerPlotRight.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerPlotRight.IsSplitterFixed = true;
            this.splitContainerPlotRight.Location = new System.Drawing.Point(0, 0);
            this.splitContainerPlotRight.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerPlotRight.Name = "splitContainerPlotRight";
            // 
            // splitContainerPlotRight.Panel1
            // 
            this.splitContainerPlotRight.Panel1.Controls.Add(this.splitContainerPlotTop);
            this.splitContainerPlotRight.Panel1MinSize = 0;
            // 
            // splitContainerPlotRight.Panel2
            // 
            this.splitContainerPlotRight.Panel2.Controls.Add(this.splitContainerPlotRightTop);
            this.splitContainerPlotRight.Panel2MinSize = 20;
            this.splitContainerPlotRight.Size = new System.Drawing.Size(535, 274);
            this.splitContainerPlotRight.SplitterDistance = 514;
            this.splitContainerPlotRight.SplitterWidth = 1;
            this.splitContainerPlotRight.TabIndex = 4;
            // 
            // splitContainerPlotTop
            // 
            this.splitContainerPlotTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerPlotTop.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerPlotTop.IsSplitterFixed = true;
            this.splitContainerPlotTop.Location = new System.Drawing.Point(0, 0);
            this.splitContainerPlotTop.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerPlotTop.Name = "splitContainerPlotTop";
            this.splitContainerPlotTop.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerPlotTop.Panel1
            // 
            this.splitContainerPlotTop.Panel1.Controls.Add(this.trackBarX);
            this.splitContainerPlotTop.Panel1MinSize = 0;
            // 
            // splitContainerPlotTop.Panel2
            // 
            this.splitContainerPlotTop.Panel2.Controls.Add(this.splitContainerPlotBottom);
            this.splitContainerPlotTop.Panel2MinSize = 0;
            this.splitContainerPlotTop.Size = new System.Drawing.Size(514, 274);
            this.splitContainerPlotTop.SplitterDistance = 24;
            this.splitContainerPlotTop.SplitterWidth = 1;
            this.splitContainerPlotTop.TabIndex = 5;
            // 
            // trackBarX
            // 
            this.trackBarX.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trackBarX.Location = new System.Drawing.Point(0, 0);
            this.trackBarX.Margin = new System.Windows.Forms.Padding(0);
            this.trackBarX.Name = "trackBarX";
            this.trackBarX.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.trackBarX.Size = new System.Drawing.Size(514, 24);
            this.trackBarX.TabIndex = 0;
            this.trackBarX.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBarX.Scroll += new System.EventHandler(this.trackBarX_Scroll);
            // 
            // splitContainerPlotBottom
            // 
            this.splitContainerPlotBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerPlotBottom.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerPlotBottom.IsSplitterFixed = true;
            this.splitContainerPlotBottom.Location = new System.Drawing.Point(0, 0);
            this.splitContainerPlotBottom.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerPlotBottom.Name = "splitContainerPlotBottom";
            this.splitContainerPlotBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerPlotBottom.Panel1
            // 
            this.splitContainerPlotBottom.Panel1.Controls.Add(this.panelPlot);
            this.splitContainerPlotBottom.Panel1MinSize = 0;
            // 
            // splitContainerPlotBottom.Panel2
            // 
            this.splitContainerPlotBottom.Panel2.Controls.Add(this.checkBoxFixedX);
            this.splitContainerPlotBottom.Panel2.Controls.Add(this.scrollBarX);
            this.splitContainerPlotBottom.Panel2MinSize = 20;
            this.splitContainerPlotBottom.Size = new System.Drawing.Size(514, 249);
            this.splitContainerPlotBottom.SplitterDistance = 228;
            this.splitContainerPlotBottom.SplitterWidth = 1;
            this.splitContainerPlotBottom.TabIndex = 6;
            // 
            // panelPlot
            // 
            this.panelPlot.BackColor = System.Drawing.Color.Black;
            this.panelPlot.Controls.Add(this.panelCursorX);
            this.panelPlot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelPlot.ForeColor = System.Drawing.Color.White;
            this.panelPlot.Location = new System.Drawing.Point(0, 0);
            this.panelPlot.Name = "panelPlot";
            this.panelPlot.Size = new System.Drawing.Size(514, 228);
            this.panelPlot.TabIndex = 0;
            this.panelPlot.Paint += new System.Windows.Forms.PaintEventHandler(this.panelPlot_Paint);
            // 
            // panelCursorX
            // 
            this.panelCursorX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.panelCursorX.BackColor = System.Drawing.Color.DodgerBlue;
            this.panelCursorX.Cursor = System.Windows.Forms.Cursors.SizeWE;
            this.panelCursorX.Location = new System.Drawing.Point(3, 0);
            this.panelCursorX.Margin = new System.Windows.Forms.Padding(0);
            this.panelCursorX.Name = "panelCursorX";
            this.panelCursorX.Size = new System.Drawing.Size(3, 229);
            this.panelCursorX.TabIndex = 0;
            this.panelCursorX.Paint += new System.Windows.Forms.PaintEventHandler(this.panelCursorX_Paint);
            this.panelCursorX.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelCursorX_MouseDown);
            this.panelCursorX.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelCursorX_MouseMove);
            this.panelCursorX.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelCursorX_MouseUp);
            // 
            // checkBoxFixedX
            // 
            this.checkBoxFixedX.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxFixedX.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxFixedX.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxFixedX.Font = new System.Drawing.Font("Meiryo UI", 5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBoxFixedX.Location = new System.Drawing.Point(497, 3);
            this.checkBoxFixedX.Margin = new System.Windows.Forms.Padding(0);
            this.checkBoxFixedX.Name = "checkBoxFixedX";
            this.checkBoxFixedX.Size = new System.Drawing.Size(16, 14);
            this.checkBoxFixedX.TabIndex = 0;
            this.checkBoxFixedX.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxFixedX.UseVisualStyleBackColor = false;
            this.checkBoxFixedX.CheckedChanged += new System.EventHandler(this.checkBoxFixedX_CheckedChanged);
            // 
            // scrollBarX
            // 
            this.scrollBarX.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollBarX.LargeChange = 1;
            this.scrollBarX.Location = new System.Drawing.Point(2, 1);
            this.scrollBarX.Maximum = 0;
            this.scrollBarX.Name = "scrollBarX";
            this.scrollBarX.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.scrollBarX.Size = new System.Drawing.Size(494, 18);
            this.scrollBarX.TabIndex = 0;
            this.scrollBarX.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollBarX_Scroll);
            // 
            // splitContainerPlotRightTop
            // 
            this.splitContainerPlotRightTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerPlotRightTop.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerPlotRightTop.IsSplitterFixed = true;
            this.splitContainerPlotRightTop.Location = new System.Drawing.Point(0, 0);
            this.splitContainerPlotRightTop.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerPlotRightTop.Name = "splitContainerPlotRightTop";
            this.splitContainerPlotRightTop.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainerPlotRightTop.Panel1MinSize = 0;
            // 
            // splitContainerPlotRightTop.Panel2
            // 
            this.splitContainerPlotRightTop.Panel2.Controls.Add(this.splitContainerPlotRightBottom);
            this.splitContainerPlotRightTop.Panel2MinSize = 0;
            this.splitContainerPlotRightTop.Size = new System.Drawing.Size(20, 274);
            this.splitContainerPlotRightTop.SplitterDistance = 24;
            this.splitContainerPlotRightTop.SplitterWidth = 1;
            this.splitContainerPlotRightTop.TabIndex = 6;
            // 
            // splitContainerPlotRightBottom
            // 
            this.splitContainerPlotRightBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerPlotRightBottom.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerPlotRightBottom.IsSplitterFixed = true;
            this.splitContainerPlotRightBottom.Location = new System.Drawing.Point(0, 0);
            this.splitContainerPlotRightBottom.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerPlotRightBottom.Name = "splitContainerPlotRightBottom";
            this.splitContainerPlotRightBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainerPlotRightBottom.Panel1
            // 
            this.splitContainerPlotRightBottom.Panel1.Controls.Add(this.checkBoxFixedY);
            this.splitContainerPlotRightBottom.Panel1.Controls.Add(this.scrollBarY);
            this.splitContainerPlotRightBottom.Panel1MinSize = 0;
            this.splitContainerPlotRightBottom.Panel2MinSize = 20;
            this.splitContainerPlotRightBottom.Size = new System.Drawing.Size(20, 249);
            this.splitContainerPlotRightBottom.SplitterDistance = 228;
            this.splitContainerPlotRightBottom.SplitterWidth = 1;
            this.splitContainerPlotRightBottom.TabIndex = 6;
            // 
            // checkBoxFixedY
            // 
            this.checkBoxFixedY.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.checkBoxFixedY.BackColor = System.Drawing.Color.Transparent;
            this.checkBoxFixedY.CheckAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxFixedY.Font = new System.Drawing.Font("Meiryo UI", 5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.checkBoxFixedY.Location = new System.Drawing.Point(2, 212);
            this.checkBoxFixedY.Margin = new System.Windows.Forms.Padding(0);
            this.checkBoxFixedY.Name = "checkBoxFixedY";
            this.checkBoxFixedY.Size = new System.Drawing.Size(16, 16);
            this.checkBoxFixedY.TabIndex = 1;
            this.checkBoxFixedY.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.checkBoxFixedY.UseVisualStyleBackColor = false;
            this.checkBoxFixedY.CheckedChanged += new System.EventHandler(this.checkBoxFixedY_CheckedChanged);
            // 
            // scrollBarY
            // 
            this.scrollBarY.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.scrollBarY.LargeChange = 1;
            this.scrollBarY.Location = new System.Drawing.Point(1, 2);
            this.scrollBarY.Maximum = 0;
            this.scrollBarY.Name = "scrollBarY";
            this.scrollBarY.Size = new System.Drawing.Size(18, 212);
            this.scrollBarY.TabIndex = 0;
            this.scrollBarY.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollBarY_Scroll);
            // 
            // panelAxisRight
            // 
            this.panelAxisRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAxisRight.Location = new System.Drawing.Point(0, 0);
            this.panelAxisRight.Name = "panelAxisRight";
            this.panelAxisRight.Size = new System.Drawing.Size(40, 274);
            this.panelAxisRight.TabIndex = 1;
            this.panelAxisRight.Paint += new System.Windows.Forms.PaintEventHandler(this.panelAxisRight_Paint);
            // 
            // splitContainerAxisBottomLeft
            // 
            this.splitContainerAxisBottomLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerAxisBottomLeft.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerAxisBottomLeft.IsSplitterFixed = true;
            this.splitContainerAxisBottomLeft.Location = new System.Drawing.Point(0, 0);
            this.splitContainerAxisBottomLeft.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerAxisBottomLeft.Name = "splitContainerAxisBottomLeft";
            this.splitContainerAxisBottomLeft.Panel1MinSize = 0;
            // 
            // splitContainerAxisBottomLeft.Panel2
            // 
            this.splitContainerAxisBottomLeft.Panel2.Controls.Add(this.splitContainerAxisBottomRight);
            this.splitContainerAxisBottomLeft.Panel2MinSize = 0;
            this.splitContainerAxisBottomLeft.Size = new System.Drawing.Size(682, 33);
            this.splitContainerAxisBottomLeft.SplitterDistance = 80;
            this.splitContainerAxisBottomLeft.SplitterWidth = 1;
            this.splitContainerAxisBottomLeft.TabIndex = 4;
            // 
            // splitContainerAxisBottomRight
            // 
            this.splitContainerAxisBottomRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerAxisBottomRight.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.splitContainerAxisBottomRight.IsSplitterFixed = true;
            this.splitContainerAxisBottomRight.Location = new System.Drawing.Point(0, 0);
            this.splitContainerAxisBottomRight.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerAxisBottomRight.Name = "splitContainerAxisBottomRight";
            // 
            // splitContainerAxisBottomRight.Panel1
            // 
            this.splitContainerAxisBottomRight.Panel1.Controls.Add(this.panelAxisBottom);
            this.splitContainerAxisBottomRight.Panel1MinSize = 0;
            this.splitContainerAxisBottomRight.Panel2MinSize = 0;
            this.splitContainerAxisBottomRight.Size = new System.Drawing.Size(601, 33);
            this.splitContainerAxisBottomRight.SplitterDistance = 560;
            this.splitContainerAxisBottomRight.SplitterWidth = 1;
            this.splitContainerAxisBottomRight.TabIndex = 4;
            // 
            // panelAxisBottom
            // 
            this.panelAxisBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAxisBottom.Location = new System.Drawing.Point(0, 0);
            this.panelAxisBottom.Name = "panelAxisBottom";
            this.panelAxisBottom.Size = new System.Drawing.Size(560, 33);
            this.panelAxisBottom.TabIndex = 2;
            this.panelAxisBottom.Paint += new System.Windows.Forms.PaintEventHandler(this.panelAxisBottom_Paint);
            this.panelAxisBottom.MouseClick += new System.Windows.Forms.MouseEventHandler(this.panelAxisBottom_MouseClick);
            // 
            // panelFooter
            // 
            this.panelFooter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelFooter.Location = new System.Drawing.Point(0, 0);
            this.panelFooter.Name = "panelFooter";
            this.panelFooter.Size = new System.Drawing.Size(682, 0);
            this.panelFooter.TabIndex = 4;
            // 
            // BL_Graph
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.Controls.Add(this.splitContainerHeader);
            this.Name = "BL_Graph";
            this.Size = new System.Drawing.Size(682, 339);
            this.Resize += new System.EventHandler(this.BL_Graph_Resize);
            this.splitContainerHeader.Panel1.ResumeLayout(false);
            this.splitContainerHeader.Panel2.ResumeLayout(false);
            this.splitContainerHeader.ResumeLayout(false);
            this.splitContainerFooter.Panel1.ResumeLayout(false);
            this.splitContainerFooter.Panel2.ResumeLayout(false);
            this.splitContainerFooter.ResumeLayout(false);
            this.splitContainerAxisBottom.Panel1.ResumeLayout(false);
            this.splitContainerAxisBottom.Panel2.ResumeLayout(false);
            this.splitContainerAxisBottom.ResumeLayout(false);
            this.splitContainerAxisLeft.Panel1.ResumeLayout(false);
            this.splitContainerAxisLeft.Panel2.ResumeLayout(false);
            this.splitContainerAxisLeft.ResumeLayout(false);
            this.splitContainerAxisRight.Panel1.ResumeLayout(false);
            this.splitContainerAxisRight.Panel2.ResumeLayout(false);
            this.splitContainerAxisRight.ResumeLayout(false);
            this.splitContainerPlotLeft.Panel1.ResumeLayout(false);
            this.splitContainerPlotLeft.Panel2.ResumeLayout(false);
            this.splitContainerPlotLeft.ResumeLayout(false);
            this.splitContainerPlotLeftTop.Panel2.ResumeLayout(false);
            this.splitContainerPlotLeftTop.ResumeLayout(false);
            this.splitContainerPlotLeftBottom.Panel1.ResumeLayout(false);
            this.splitContainerPlotLeftBottom.Panel1.PerformLayout();
            this.splitContainerPlotLeftBottom.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarY)).EndInit();
            this.splitContainerPlotRight.Panel1.ResumeLayout(false);
            this.splitContainerPlotRight.Panel2.ResumeLayout(false);
            this.splitContainerPlotRight.ResumeLayout(false);
            this.splitContainerPlotTop.Panel1.ResumeLayout(false);
            this.splitContainerPlotTop.Panel1.PerformLayout();
            this.splitContainerPlotTop.Panel2.ResumeLayout(false);
            this.splitContainerPlotTop.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarX)).EndInit();
            this.splitContainerPlotBottom.Panel1.ResumeLayout(false);
            this.splitContainerPlotBottom.Panel2.ResumeLayout(false);
            this.splitContainerPlotBottom.ResumeLayout(false);
            this.panelPlot.ResumeLayout(false);
            this.splitContainerPlotRightTop.Panel2.ResumeLayout(false);
            this.splitContainerPlotRightTop.ResumeLayout(false);
            this.splitContainerPlotRightBottom.Panel1.ResumeLayout(false);
            this.splitContainerPlotRightBottom.ResumeLayout(false);
            this.splitContainerAxisBottomLeft.Panel2.ResumeLayout(false);
            this.splitContainerAxisBottomLeft.ResumeLayout(false);
            this.splitContainerAxisBottomRight.Panel1.ResumeLayout(false);
            this.splitContainerAxisBottomRight.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainerHeader;
        private System.Windows.Forms.SplitContainer splitContainerFooter;
        private System.Windows.Forms.SplitContainer splitContainerAxisBottom;
        private System.Windows.Forms.SplitContainer splitContainerAxisLeft;
        private System.Windows.Forms.SplitContainer splitContainerAxisRight;
        private System.Windows.Forms.SplitContainer splitContainerPlotLeft;
        private System.Windows.Forms.SplitContainer splitContainerPlotRight;
        private System.Windows.Forms.SplitContainer splitContainerPlotTop;
        private System.Windows.Forms.SplitContainer splitContainerPlotBottom;
        private System.Windows.Forms.SplitContainer splitContainerPlotLeftTop;
        private System.Windows.Forms.SplitContainer splitContainerPlotLeftBottom;
        private System.Windows.Forms.SplitContainer splitContainerPlotRightTop;
        private System.Windows.Forms.SplitContainer splitContainerPlotRightBottom;
        private System.Windows.Forms.HScrollBar scrollBarX;
        private System.Windows.Forms.VScrollBar scrollBarY;
        private System.Windows.Forms.TrackBar trackBarX;
        private BelicsClass.UI.Controls.BL_DoubleBufferPanel panelPlot;
        private BelicsClass.UI.Controls.BL_DoubleBufferPanel panelAxisLeft;
        private System.Windows.Forms.SplitContainer splitContainerAxisBottomLeft;
        private System.Windows.Forms.SplitContainer splitContainerAxisBottomRight;
        private BelicsClass.UI.Controls.BL_DoubleBufferPanel panelAxisRight;
        private BelicsClass.UI.Controls.BL_DoubleBufferPanel panelHeader;
        private BelicsClass.UI.Controls.BL_DoubleBufferPanel panelAxisBottom;
        private BelicsClass.UI.Controls.BL_DoubleBufferPanel panelFooter;
        private System.Windows.Forms.TrackBar trackBarY;
        private BelicsClass.UI.Controls.BL_DoubleBufferPanel panelCursorX;
        private System.Windows.Forms.CheckBox checkBoxFixedX;
        private System.Windows.Forms.CheckBox checkBoxFixedY;
    }
}
