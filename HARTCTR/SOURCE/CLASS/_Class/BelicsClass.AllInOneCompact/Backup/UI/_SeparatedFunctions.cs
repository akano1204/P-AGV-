using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BelicsClass.UI.Controls;

namespace BelicsClass.UI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class BL_SeparatedFunctions : Form
    {
        BL_SubForm_Base target = null;

        #region 自動スケーリングオブジェクト

        /// <summary>
        /// 自動スケーリングオブジェクト
        /// </summary>
        protected BL_MainForm_Base.ViewResize ViewResizer = null;

        #endregion

        /// <summary>
        /// アプリケーション終了操作を無効にする
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassStyle = cp.ClassStyle | 0x200;
                return cp;
            }
        }

        /// <summary>
        /// ファンクションキー文字列を保持します。
        /// </summary>
        public BL_FlatButton[] btnFunctions = new BL_FlatButton[13];

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public BL_SeparatedFunctions(BL_SubForm_Base sub)
        {
            InitializeComponent();

            ViewResizer = new BL_MainForm_Base.ViewResize(this);

            target = sub;
            sub.m_Functions = this;

            btnFunctions[0] = buttonFunction00;
            btnFunctions[1] = buttonFunction01;
            btnFunctions[2] = buttonFunction02;
            btnFunctions[3] = buttonFunction03;
            btnFunctions[4] = buttonFunction04;
            btnFunctions[5] = buttonFunction05;
            btnFunctions[6] = buttonFunction06;
            btnFunctions[7] = buttonFunction07;
            btnFunctions[8] = buttonFunction08;
            btnFunctions[9] = buttonFunction09;
            btnFunctions[10] = buttonFunction10;
            btnFunctions[11] = buttonFunction11;
            btnFunctions[12] = buttonFunction12;

            for (int i = 0; i < btnFunctions.Length; i++)
            {
                btnFunctions[i].Click += new EventHandler(BL_SubForm_Functions_ButtonClicked);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void BL_SubForm_Functions_KeyUp(object sender, KeyEventArgs e)
        {
            if (DesignMode) return;

            e.Handled = true;

            do
            {
                if (!target.key_input_accept) return;
                target.key_input_accept = false;

                int function_no = -1;

                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        function_no = 0;
                        break;
                    case Keys.F1:
                        function_no = 1;
                        break;
                    case Keys.F2:
                        function_no = 2;
                        break;
                    case Keys.F3:
                        function_no = 3;
                        break;
                    case Keys.F4:
                        function_no = 4;
                        break;
                    case Keys.F5:
                        function_no = 5;
                        break;
                    case Keys.F6:
                        function_no = 6;
                        break;
                    case Keys.F7:
                        function_no = 7;
                        break;
                    case Keys.F8:
                        function_no = 8;
                        break;
                    case Keys.F9:
                        function_no = 9;
                        break;
                    case Keys.F10:
                        function_no = 10;
                        break;
                    case Keys.F11:
                        function_no = 11;
                        break;
                    case Keys.F12:
                        function_no = 12;
                        break;
                }

                if (0 <= function_no && function_no <= 12)
                {
                    if (target.dictFunctions.ContainsKey(function_no))
                    {
                        if (typeof(Button).IsInstanceOfType(target.dictFunctions[function_no]))
                        {
                            Button button = (Button)target.dictFunctions[function_no];
                            if (button.Tag != null && typeof(BL_FlatButton).IsInstanceOfType(button))
                            {
                                if (typeof(bool).IsInstanceOfType(button.Tag))
                                {
                                    if (((BL_FlatButton)button).CheckMode) ((BL_FlatButton)button).Checked = !((BL_FlatButton)button).Checked;
                                }
                            }
                        }
                    }

                    if (btnFunctions[function_no].Enabled) target.btnFunctions_Click(btnFunctions[function_no], e);
                    e.Handled = true;
                }

                target.key_input_accept = true;
            }
            while (false);
        }

        private void BL_SubForm_Functions_ButtonClicked(object sender, EventArgs e)
        {
            if (DesignMode) return;

            if (typeof(Button).IsInstanceOfType(sender))
            {
                Button btn = (Button)sender;
                int num;
                if (int.TryParse(btn.Name.Substring(btn.Name.Length - 2), out num))
                {
                    switch (num)
                    {
                        case 0:
                            BL_SubForm_Functions_KeyUp(sender, new KeyEventArgs(Keys.Escape));
                            break;
                        case 1:
                            BL_SubForm_Functions_KeyUp(sender, new KeyEventArgs(Keys.F1));
                            break;
                        case 2:
                            BL_SubForm_Functions_KeyUp(sender, new KeyEventArgs(Keys.F2));
                            break;
                        case 3:
                            BL_SubForm_Functions_KeyUp(sender, new KeyEventArgs(Keys.F3));
                            break;
                        case 4:
                            BL_SubForm_Functions_KeyUp(sender, new KeyEventArgs(Keys.F4));
                            break;
                        case 5:
                            BL_SubForm_Functions_KeyUp(sender, new KeyEventArgs(Keys.F5));
                            break;
                        case 6:
                            BL_SubForm_Functions_KeyUp(sender, new KeyEventArgs(Keys.F6));
                            break;
                        case 7:
                            BL_SubForm_Functions_KeyUp(sender, new KeyEventArgs(Keys.F7));
                            break;
                        case 8:
                            BL_SubForm_Functions_KeyUp(sender, new KeyEventArgs(Keys.F8));
                            break;
                        case 9:
                            BL_SubForm_Functions_KeyUp(sender, new KeyEventArgs(Keys.F9));
                            break;
                        case 10:
                            BL_SubForm_Functions_KeyUp(sender, new KeyEventArgs(Keys.F10));
                            break;
                        case 11:
                            BL_SubForm_Functions_KeyUp(sender, new KeyEventArgs(Keys.F11));
                            break;
                        case 12:
                            BL_SubForm_Functions_KeyUp(sender, new KeyEventArgs(Keys.F12));
                            break;
                    }
                }
            }
        }

        private void BL_SubForm_Functions_Load(object sender, EventArgs e)
        {
            if (this.ControlBox) this.Text = "ファンクション操作ウィンドウ [" + target.TitleString() + "]";
            string[] func = target.FunctionStrings();


            for (int i = 0; i < func.Length; i++)
            {
                btnFunctions[i].Tag = null;
                btnFunctions[i].Text = func[i].Replace(":", "\n");
                if (btnFunctions[i].Text.Trim().Length == 0)
                {
                    btnFunctions[i].Enabled = false;
                    btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                }
                else
                {
                    btnFunctions[i].Enabled = true;
                    btnFunctions[i].BackColor = btnFunctions[i].BackColorNormal;

                    if (0 <= btnFunctions[i].Text.IndexOf("終了"))
                    {
                        btnFunctions[i].Enabled = false;
                        btnFunctions[i].BackColor = btnFunctions[i].BackColorOFF;
                    }
                }
            }

            Left = target.Left;
            Top = target.Top + target.Height;
            Width = target.Width;
        }

        private void BL_SubForm_Functions_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (ViewResizer != null)
            {
                ViewResizer.Clear();
                ViewResizer = null;
            }

            target.m_Functions = null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="functionno"></param>
        /// <param name="enabled"></param>
        public void Function_Enabled(int functionno, bool enabled)
        {
            if (functionno < 0 || 12 < functionno) return;
            btnFunctions[functionno].Enabled = enabled;
        }

        private void BL_SubForm_Functions_Resize(object sender, EventArgs e)
        {
            if (ViewResizer != null)
            {
                ViewResizer.Resize(this);
            }
        }
    }
}
