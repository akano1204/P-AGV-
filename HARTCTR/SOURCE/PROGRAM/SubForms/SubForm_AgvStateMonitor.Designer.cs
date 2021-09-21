namespace PROGRAM
{
    partial class SubForm_AgvStateMonitor
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.LV1 = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.panelMap = new BelicsClass.UI.Controls.BL_DoubleBufferPanel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // LV1
            // 
            this.LV1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LV1.Font = new System.Drawing.Font("Meiryo UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LV1.FullRowSelect = true;
            this.LV1.GridLines = true;
            this.LV1.HideSelection = false;
            this.LV1.IsStripeColored = false;
            this.LV1.Location = new System.Drawing.Point(0, 0);
            this.LV1.Name = "LV1";
            this.LV1.OwnerDraw = true;
            this.LV1.Size = new System.Drawing.Size(998, 472);
            this.LV1.StripeBackColor1 = System.Drawing.Color.LightSkyBlue;
            this.LV1.StripeBackColor2 = System.Drawing.Color.White;
            this.LV1.TabIndex = 0;
            this.LV1.TitleBackColor = System.Drawing.SystemColors.Control;
            this.LV1.TitleForeColor = System.Drawing.SystemColors.ControlText;
            this.LV1.UseCompatibleStateImageBehavior = false;
            this.LV1.View = System.Windows.Forms.View.Details;
            this.LV1.VirtualMode = true;
            this.LV1.SelectedIndexChanged += new System.EventHandler(this.LV1_SelectedIndexChanged);
            // 
            // panelMap
            // 
            this.panelMap.BackColor = System.Drawing.Color.White;
            this.panelMap.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panelMap.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelMap.Location = new System.Drawing.Point(0, 0);
            this.panelMap.Name = "panelMap";
            this.panelMap.Size = new System.Drawing.Size(998, 186);
            this.panelMap.TabIndex = 2;
            // 
            // splitContainer1
            // 
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(12, 12);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.LV1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.panelMap);
            this.splitContainer1.Size = new System.Drawing.Size(998, 662);
            this.splitContainer1.SplitterDistance = 472;
            this.splitContainer1.TabIndex = 3;
            // 
            // SubForm_AgvStateMonitor
            // 
            this._TitleString = "状況モニター";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1022, 686);
            this.Controls.Add(this.splitContainer1);
            this.EnableSeparation = true;
            this.Font = new System.Drawing.Font("Meiryo UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "SubForm_AgvStateMonitor";
            this.Text = "状況モニター";
            this.TitleText = "状況モニター";
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private BelicsClass.UI.Controls.BL_VirtualListView LV1;
        private BelicsClass.UI.Controls.BL_DoubleBufferPanel panelMap;
        private System.Windows.Forms.SplitContainer splitContainer1;
    }
}