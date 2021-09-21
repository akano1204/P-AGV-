namespace PROGRAM
{
    partial class SubForm_RackSetting
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
			this.dataGridViewRacks = new BelicsClass.UI.Controls.BL_DataGridViewPlus();
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewRacks)).BeginInit();
			this.SuspendLayout();
			// 
			// dataGridViewRacks
			// 
			this.dataGridViewRacks.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dataGridViewRacks.CellVisibleFix = false;
			this.dataGridViewRacks.ColumnHeadersHeight = 60;
			this.dataGridViewRacks.Location = new System.Drawing.Point(12, 12);
			this.dataGridViewRacks.Name = "dataGridViewRacks";
			this.dataGridViewRacks.RowIndexVisible = true;
			this.dataGridViewRacks.RowTemplate.Height = 21;
			this.dataGridViewRacks.Size = new System.Drawing.Size(998, 662);
			this.dataGridViewRacks.TabIndex = 0;
			this.dataGridViewRacks.CellBeginEdit += new System.Windows.Forms.DataGridViewCellCancelEventHandler(this.dataGridViewRacks_CellBeginEdit);
			this.dataGridViewRacks.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewRacks_CellEndEdit);
			this.dataGridViewRacks.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridViewRacks_DataError);
			this.dataGridViewRacks.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridViewRacks_KeyDown);
			// 
			// SubForm_RackSetting
			// 
			this._TitleString = "棚設定";
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1022, 686);
			this.Controls.Add(this.dataGridViewRacks);
			this.Font = new System.Drawing.Font("Meiryo UI", 10F);
			this.Name = "SubForm_RackSetting";
			this.Text = "棚設定";
			this.TitleText = "棚設定";
			((System.ComponentModel.ISupportInitialize)(this.dataGridViewRacks)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private BelicsClass.UI.Controls.BL_DataGridViewPlus dataGridViewRacks;
    }
}