namespace BelicsClass.UI
{
    /// <summary>
    /// サブフォームの基本クラスです。
    /// </summary>
    partial class BL_SubForm_Base
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
            this.timerClock = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // timerClock
            // 
            this.timerClock.Interval = 500;
            this.timerClock.Tick += new System.EventHandler(this.timerClock_Tick);
            // 
            // BL_SubForm_Base
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1022, 686);
            this.ControlBox = false;
            this.Font = new System.Drawing.Font("Meiryo UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.KeyPreview = true;
            this.Margin = new System.Windows.Forms.Padding(5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BL_SubForm_Base";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "[ここに子画面のタイトルを記述]";
            this.Activated += new System.EventHandler(this.SubForm_Base_Activated);
            this.Deactivate += new System.EventHandler(this.SubForm_Base_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SubForm_Base_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SubForm_Base_FormClosed);
            this.Load += new System.EventHandler(this.SubForm_Base_Load);
            this.Enter += new System.EventHandler(this.SubForm_Base_Enter);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SubForm_Base_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.SubForm_Base_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.SubForm_Base_KeyUp);
            this.Leave += new System.EventHandler(this.SubForm_Base_Leave);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.SubForm_Base_MouseMove);
            this.Move += new System.EventHandler(this.BL_SubForm_Base_Move);
            this.Resize += new System.EventHandler(this.SubForm_Base_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        /// <summary>タイマー</summary>        
        protected System.Windows.Forms.Timer timerClock;



    }
}