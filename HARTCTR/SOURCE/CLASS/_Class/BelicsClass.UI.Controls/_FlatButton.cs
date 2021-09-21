using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;

namespace BelicsClass.UI.Controls
{
    /// <summary>
    /// フラットボタンクラス
    /// </summary>
    public class BL_FlatButton : Button
    {
        private Color backColorNormal = Color.RoyalBlue;
        private Color foreColorNormal = Color.White;
        private Color backColorON = Color.Lime;
        private Color foreColorON = Color.Black;
        private Color backColorOFF = Color.FromArgb(64, 64, 64);
        private Color foreColorOFF = Color.White;

        /// <summary></summary>
        public Color BackColorNormal
        {
            get { return backColorNormal; }
            set
            {
                backColorNormal = value;
                Checked = Checked;
            }
        }

        /// <summary></summary>
        public Color ForeColorNormal
        {
            get { return foreColorNormal; }
            set
            {
                foreColorNormal = value;
                Checked = Checked;
            }
        }

        /// <summary></summary>
        public Color BackColorON
        {
            get { return backColorON; }
            set
            {
                backColorON = value;
                Checked = Checked;
            }
        }

        /// <summary></summary>
        public Color ForeColorON
        {
            get { return foreColorON; }
            set
            {
                foreColorON = value;
                Checked = Checked;
            }
        }

        /// <summary></summary>
        public Color BackColorOFF
        {
            get { return backColorOFF; }
            set
            {
                backColorOFF = value;
                Checked = Checked;
            }
        }

        /// <summary></summary>
        public Color ForeColorOFF
        {
            get { return foreColorOFF; }
            set
            {
                foreColorOFF = value;
                Checked = Checked;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_FlatButton()
        {
            this.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Tag = false;
            this.Cursor = Cursors.Hand;
            this.FlatAppearance.BorderSize = 2;
            this.FlatAppearance.BorderColor = Color.Black;

            Checked = Checked;
        }

        private bool checkMode = false;
        /// <summary>
        /// 
        /// </summary>
        public bool CheckMode
        {
            get { return checkMode; }
            set
            {
                checkMode = value;
                Checked = Checked;
            }
        }

        private bool check = false;
        /// <summary>
        /// 
        /// </summary>
        public bool Checked
        {
            get
            {
                return check;
            }

            set
            {
                if (Enabled)
                {
                    if (CheckMode)
                    {
                        check = value;
                        this.BackColor = value ? BackColorON : BackColorOFF;
                        this.ForeColor = value ? ForeColorON : ForeColorOFF;
                    }
                    else
                    {
                        this.BackColor = BackColorNormal;
                        this.ForeColor = ForeColorNormal;
                    }
                }
                else
                {
                    this.BackColor = Color.FromArgb(64, 64, 64);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnClick(EventArgs e)
        {
            Checked = !Checked;

            base.OnClick(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnEnabledChanged(EventArgs e)
        {
            Checked = Checked;

            base.OnEnabledChanged(e);
        }
    }
}
