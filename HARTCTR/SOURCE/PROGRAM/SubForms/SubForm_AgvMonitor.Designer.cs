namespace PROGRAM
{
    partial class SubForm_AgvMonitor
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
            this.button1 = new System.Windows.Forms.Button();
            this.LV1 = new System.Windows.Forms.ListView();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(894, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(116, 36);
            this.button1.TabIndex = 4;
            this.button1.Text = "再表示";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LV1
            // 
            this.LV1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LV1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1});
            this.LV1.FullRowSelect = true;
            this.LV1.GridLines = true;
            this.LV1.HideSelection = false;
            this.LV1.Location = new System.Drawing.Point(12, 12);
            this.LV1.MultiSelect = false;
            this.LV1.Name = "LV1";
            this.LV1.Size = new System.Drawing.Size(998, 633);
            this.LV1.TabIndex = 6;
            this.LV1.UseCompatibleStateImageBehavior = false;
            // 
            // button2
            // 
            this.button2.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button2.Location = new System.Drawing.Point(12, 651);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(96, 33);
            this.button2.TabIndex = 7;
            this.button2.Text = "異常内容";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Font = new System.Drawing.Font("Meiryo UI", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.button3.Location = new System.Drawing.Point(122, 651);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(96, 33);
            this.button3.TabIndex = 8;
            this.button3.Text = "現在地ﾏｯﾌﾟ";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "AGV";
            this.columnHeader1.Width = 100;
            // 
            // SubForm_AgvMonitor
            // 
            this._TitleString = "運用モニター";
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1022, 686);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.LV1);
            this.Font = new System.Drawing.Font("Meiryo UI", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Name = "SubForm_AgvMonitor";
            this.Text = "運用モニター";
            this.TitleText = "運用モニター";
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListView LV1;
        private System.Windows.Forms.Button button2;
        protected System.Windows.Forms.Button button3;
        private System.Windows.Forms.ColumnHeader columnHeader1;
    }
}