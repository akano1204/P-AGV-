namespace PROGRAM
{
    partial class SubForm_ModeSelecter
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
            this.listviewMode = new BelicsClass.UI.Controls.BL_VirtualListView();
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listviewMode
            // 
            this.listviewMode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listviewMode.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listviewMode.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader3,
            this.columnHeader5});
            this.listviewMode.Font = new System.Drawing.Font("Meiryo UI", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.listviewMode.FullRowSelect = true;
            this.listviewMode.GridLines = true;
            this.listviewMode.HideSelection = false;
            this.listviewMode.IsStripeColored = false;
            this.listviewMode.Location = new System.Drawing.Point(134, 106);
            this.listviewMode.Name = "listviewMode";
            this.listviewMode.OwnerDraw = true;
            this.listviewMode.Size = new System.Drawing.Size(754, 475);
            this.listviewMode.StripeBackColor1 = System.Drawing.Color.LightSkyBlue;
            this.listviewMode.StripeBackColor2 = System.Drawing.Color.White;
            this.listviewMode.TabIndex = 11;
            this.listviewMode.TitleBackColor = System.Drawing.SystemColors.Control;
            this.listviewMode.TitleForeColor = System.Drawing.SystemColors.ControlText;
            this.listviewMode.UseCompatibleStateImageBehavior = false;
            this.listviewMode.View = System.Windows.Forms.View.Details;
            this.listviewMode.VirtualMode = true;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "動作モード";
            this.columnHeader3.Width = 189;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "動作条件";
            this.columnHeader5.Width = 518;
            // 
            // SubForm_A2100
            // 
            this._TitleString = "運用選択";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1022, 686);
            this.Controls.Add(this.listviewMode);
            this.Name = "SubForm_A2100";
            this.Text = "運用選択";
            this.TitleText = "運用選択";
            this.ResumeLayout(false);

        }

        #endregion

        private BelicsClass.UI.Controls.BL_VirtualListView listviewMode;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader5;
    }
}