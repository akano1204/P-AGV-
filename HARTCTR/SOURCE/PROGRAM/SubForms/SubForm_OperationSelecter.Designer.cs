namespace PROGRAM
{
    partial class SubForm_OperationSelecter
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
            this.buttonStart = new System.Windows.Forms.Button();
            this.buttonFinish = new System.Windows.Forms.Button();
            this.buttonLineup = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // buttonStart
            // 
            this.buttonStart.Font = new System.Drawing.Font("Meiryo UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonStart.Location = new System.Drawing.Point(336, 171);
            this.buttonStart.Name = "buttonStart";
            this.buttonStart.Size = new System.Drawing.Size(362, 74);
            this.buttonStart.TabIndex = 4;
            this.buttonStart.Text = "運用開始";
            this.buttonStart.UseVisualStyleBackColor = true;
            // 
            // buttonFinish
            // 
            this.buttonFinish.Font = new System.Drawing.Font("Meiryo UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonFinish.Location = new System.Drawing.Point(336, 294);
            this.buttonFinish.Name = "buttonFinish";
            this.buttonFinish.Size = new System.Drawing.Size(362, 74);
            this.buttonFinish.TabIndex = 4;
            this.buttonFinish.Text = "運用終了";
            this.buttonFinish.UseVisualStyleBackColor = true;
            // 
            // buttonLineup
            // 
            this.buttonLineup.Font = new System.Drawing.Font("Meiryo UI", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.buttonLineup.Location = new System.Drawing.Point(336, 419);
            this.buttonLineup.Name = "buttonLineup";
            this.buttonLineup.Size = new System.Drawing.Size(362, 74);
            this.buttonLineup.TabIndex = 4;
            this.buttonLineup.Text = "全AGV整列";
            this.buttonLineup.UseVisualStyleBackColor = true;
            // 
            // SubForm_A2000
            // 
            this._TitleString = "運用選択";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1022, 686);
            this.Controls.Add(this.buttonLineup);
            this.Controls.Add(this.buttonFinish);
            this.Controls.Add(this.buttonStart);
            this.Name = "SubForm_A2000";
            this.Text = "運用選択";
            this.TitleText = "運用選択";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button buttonStart;
        private System.Windows.Forms.Button buttonFinish;
        private System.Windows.Forms.Button buttonLineup;
    }
}