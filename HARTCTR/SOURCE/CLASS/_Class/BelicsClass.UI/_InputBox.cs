using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace BelicsClass.UI
{
    /// <summary>
    /// 標準メッセージボックスクラスです
    /// </summary>
    public partial class BL_InputBox : BL_MessageBox
    {
        static string sPassword = "";

        /// <summary>
        /// 入力されたテキスト
        /// </summary>
        public string sInputText = "";

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="owner">表示元子画面への参照(null禁止)</param>
        public BL_InputBox(BL_SubForm_Base owner)
            : base(owner)
        {
            InitializeComponent();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="main">メインフォームへの参照</param>
        public BL_InputBox(BL_MainForm_Base main)
            : base(main)
        {
            InitializeComponent();
        }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public BL_InputBox()
        {
            InitializeComponent();
        }

        /// <summary>
        /// パスワード確認メッセージボックスを表示します。
        /// </summary>
        /// <param name="password"></param>
        /// <returns>
        /// パスワード一致の場合:DialogResult.Yes
        /// 不一致の場合:DialogResult.Abort
        /// キャンセルの場合:DialogResult.Cancel
        /// </returns>
        virtual public DialogResult ShowCheckPassword(string password)
        {
            return ShowCheckPassword("パスワードを入力してください。", password);
        }

        /// <summary>
        /// パスワード確認メッセージボックスを表示します。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="password"></param>
        /// <returns>
        /// パスワード一致の場合:DialogResult.Yes
        /// 不一致の場合:DialogResult.Abort
        /// キャンセルの場合:DialogResult.Cancel
        /// </returns>
        virtual public DialogResult ShowCheckPassword(string message, string password)
        {
            string title = "パスワード確認";
            return ShowCheckPassword(message, title, password);
        }

        /// <summary>
        /// パスワード確認メッセージボックスを表示します。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="password"></param>
        /// <returns>
        /// パスワード一致の場合:DialogResult.Yes
        /// 不一致の場合:DialogResult.Abort
        /// キャンセルの場合:DialogResult.Cancel
        /// </returns>
        virtual public DialogResult ShowCheckPassword(string message, string title, string password)
        {
            return ShowCheckPassword(message, title, cContinueAbort, password);
        }

        /// <summary>
        /// パスワード確認メッセージボックスを表示します。
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        /// <param name="password"></param>
        /// <returns>
        /// パスワード一致の場合:DialogResult.Yes
        /// 不一致の場合:DialogResult.Abort
        /// キャンセルの場合:DialogResult.Cancel
        /// </returns>
        virtual public DialogResult ShowCheckPassword(string message, string title, string[] buttons, string password)
        {
            textBoxInput.Focus();
            sPassword = password;
            return ShowMessage(message, title, buttons, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void buttonYES_Click(object sender, EventArgs e)
        {
            sInputText = textBoxInput.Text;
            
            m_Result = DialogResult.Yes;

            if (textBoxInput.Text != sPassword && sPassword != "")
            {
                m_Result = DialogResult.Abort;
            }

            if (!m_bModal) Close();

            //base.buttonYES_Click(sender, e);
        }

        /// <summary>
        /// キー押し下げリリース時の処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void MessageBox_Base_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = true;

            switch (e.KeyCode)
            {
                case Keys.F1:

                    sInputText = textBoxInput.Text;

                    if (buttonYES.Text != "")
                    {
                        m_Result = DialogResult.Yes;

                        if (textBoxInput.Text != sPassword && sPassword != "")
                        {
                            m_Result = DialogResult.Abort;
                        }

                        if (!m_bModal) Close();
                    }

                    if (buttonOK.Text != "")
                    {
                        m_Result = DialogResult.OK;
                        if (!m_bModal) Close();
                    }
                    break;

                case Keys.F2:
                    break;
                case Keys.F3:
                    break;
                case Keys.F4:
                    break;
                case Keys.F5:
                    break;
                case Keys.F6:
                    break;
                case Keys.F7:
                    break;
                case Keys.F8:
                    break;
                case Keys.F9:
                    break;
                case Keys.F10:
                    break;
                case Keys.F11:
                    break;
                case Keys.F12:
                    break;

                case Keys.Escape:
                    if (buttonCANCEL.Visible)
                    {
                        m_Result = DialogResult.Cancel;
                        if (!m_bModal) Close();
                    }
                    break;
            }

            //base.MessageBox_Base_KeyUp(sender, e);
        }
    }
}
