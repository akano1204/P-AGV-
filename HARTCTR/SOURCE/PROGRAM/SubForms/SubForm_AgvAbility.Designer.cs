namespace PROGRAM
{
    partial class SubForm_AgvAbility
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
            this.LV1 = new System.Windows.Forms.ListView();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // LV1
            // 
            this.LV1.HideSelection = false;
            this.LV1.Location = new System.Drawing.Point(12, 54);
            this.LV1.Name = "LV1";
            this.LV1.Size = new System.Drawing.Size(1164, 620);
            this.LV1.TabIndex = 0;
            this.LV1.UseCompatibleStateImageBehavior = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1060, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(116, 36);
            this.button1.TabIndex = 5;
            this.button1.Text = "再表示";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // SubForm_AgvAbility
            // 
            this._TitleString = "能力表示";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1188, 686);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LV1);
            this.Font = new System.Drawing.Font("Meiryo UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "SubForm_AgvAbility";
            this.Text = "能力表示";
            this.TitleText = "能力表示";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ListView LV1;
        private System.Windows.Forms.Button button1;
    }
}