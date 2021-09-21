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
    public partial class BL_MessageBox : BL_MessageBox_Base
    {
        #region 標準MessageBox共通

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        static public DialogResult Show(string message)
        {
            return Show(message, "");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        static public DialogResult Show(string message, string title)
        {
            return (new BL_MessageBox().ShowMessage(message, title, null));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        static public DialogResult Show(BL_SubForm_Base sub, string message)
        {
            return Show(sub, message, sub._TitleString);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        static public DialogResult Show(BL_SubForm_Base sub, string message, string title)
        {
            return Show(sub, message, title, MessageBoxButtons.OK);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        static public DialogResult Show(BL_SubForm_Base sub, string message, string title, MessageBoxButtons buttons)
        {
            string[] b = null;
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    break;
                case MessageBoxButtons.YesNo:
                case MessageBoxButtons.OKCancel:
                case MessageBoxButtons.RetryCancel:
                    b = new string[] { "はい", "", "いいえ" };
                    break;

                case MessageBoxButtons.YesNoCancel:
                    b = new string[] { "続行", "", "中止" };
                    break;
            }

            return (new BL_MessageBox(sub).ShowMessage(message, title, b));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        static public DialogResult Show(BL_MainForm_Base main, string message)
        {
            return Show(main, message, main.Text);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        static public DialogResult Show(BL_MainForm_Base main, string message, string title)
        {
            return Show(main, message, title, MessageBoxButtons.OK);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="main"></param>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        static public DialogResult Show(BL_MainForm_Base main, string message, string title, MessageBoxButtons buttons)
        {
            string[] b = null;
            switch (buttons)
            {
                case MessageBoxButtons.OK:
                    break;
                case MessageBoxButtons.YesNo:
                case MessageBoxButtons.OKCancel:
                case MessageBoxButtons.RetryCancel:
                    b = new string[] { "はい", "", "いいえ" };
                    break;

                case MessageBoxButtons.YesNoCancel:
                    b = new string[] { "続行", "", "中止" };
                    break;
            }

            return (new BL_MessageBox(main).ShowMessage(message, title, b));
        }

        #endregion

        /// <summary>
        /// YES/NOのボタンを生成します
        /// </summary>
        static public string[] cYesNo = new string[2];
        /// <summary>
        /// 続行/中止ボタンを生成します
        /// </summary>
        static public string[] cContinueAbort = new string[2];

        /// <summary>
        /// モーダルモードで実行中フラグ
        /// </summary>
        protected bool m_bModal = true;

        /// <summary>
        /// プログレスバーのスタイルを取得または設定します
        /// </summary>
        public ProgressBarStyle ProgressStyle { get { return progressBar1.Style; } set { progressBar1.Style = value; } }

        /// <summary>
        /// プログレスバーのスタイルを取得または設定します
        /// </summary>
        public bool ProgressBarVisible { get { return progressBar1.Visible; } set { progressBar1.Visible = value; } }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="owner">表示元子画面への参照(null禁止)</param>
        public BL_MessageBox(BL_SubForm_Base owner)
            : base(owner)
        {
            InitializeComponent();

            cYesNo = new string[] { "はい", "", "いいえ" };
            cContinueAbort = new string[] { "続行", "", "中止" };
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mainform">メインフォームへの参照</param>
        public BL_MessageBox(BL_MainForm_Base mainform)
            : base(mainform)
        {
            InitializeComponent();

            cYesNo = new string[] { "はい", "", "いいえ" };
            cContinueAbort = new string[] { "続行", "", "中止" };
        }

        /// <summary>
        /// デフォルトコンストラクタ
        /// </summary>
        public BL_MessageBox()
        {
            InitializeComponent();

            cYesNo = new string[] { "はい", "", "いいえ" };
            cContinueAbort = new string[] { "続行", "", "中止" };
        }

        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="message">メッセージ</param>
        /// <param name="title">タイトル</param>
        /// <param name="buttons">ボタン文字列の配列 [YES][OK][NO]</param>
        /// <returns></returns>
        virtual public DialogResult ShowMessage(string message, string title, string[] buttons)
        {
            return ShowMessage(message, title, buttons, true, 0, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="cancel"></param>
        /// <returns></returns>
        virtual public DialogResult ShowProgress(string message, string title, string cancel)
        {
            return ShowMessage(message, title, new string[3] { "", "", cancel }, false, 0, true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="cancel"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        virtual public DialogResult ShowAutoClose(string message, string title, string cancel, int interval)
        {
            return ShowMessage(message, title, new string[3] { "", "", cancel }, true, interval, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        /// <param name="bModal"></param>
        /// <returns></returns>
        virtual public DialogResult ShowMessage(string message, string title, string[] buttons, bool bModal)
        {
            return ShowMessage(message, title, buttons, bModal, false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        /// <param name="bModal"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        virtual public DialogResult ShowMessage(string message, string title, string[] buttons, bool bModal, bool progress)
        {
            return ShowMessage(message, title, buttons, bModal, 0, progress);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="title"></param>
        /// <param name="buttons"></param>
        /// <param name="bModal"></param>
        /// <param name="interval"></param>
        /// <param name="progress"></param>
        /// <returns></returns>
        virtual public DialogResult ShowMessage(string message, string title, string[] buttons, bool bModal, int interval, bool progress)
        {
            labelTitle.Focus();

            m_bModal = bModal;
            m_Result = DialogResult.None;

            this.Text = title;
            labelTitle.Text = title;
            labelMessage.Text = message;

            buttonYES.Visible = false;
            buttonOK.Visible = false;
            buttonCANCEL.Visible = false;

            progressBar1.Visible = progress;

            if (buttons == null)
            {
                buttons = new string[3];
                buttons[0] = "";
                buttons[1] = "確認";
                buttons[2] = "";
            }

            buttonYES.Text = buttons[0];
            buttonOK.Text = buttons[1];
            buttonCANCEL.Text = buttons[2];

            if (buttonYES.Text != "")
            {
                buttonYES.Text = "[F1] " + buttonYES.Text;
                buttonYES.Visible = true;
            }
            if (buttonOK.Text != "")
            {
                buttonOK.Text = "[F1] " + buttonOK.Text;
                buttonOK.Visible = true;
            }
            if (buttonCANCEL.Text != "")
            {
                buttonCANCEL.Text = "[ESC] " + buttonCANCEL.Text;
                buttonCANCEL.Visible = true;
            }

            base.ShowMessage(bModal, interval);

            buttons = null;

            return m_Result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void buttonYES_Click(object sender, EventArgs e)
        {
            m_Result = DialogResult.Yes;
            if (!m_bModal) Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void buttonOK_Click(object sender, EventArgs e)
        {
            m_Result = DialogResult.OK;
            if (!m_bModal) Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void buttonCANCEL_Click(object sender, EventArgs e)
        {
            m_Result = DialogResult.Cancel;
            if (!m_bModal) Close();
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
                    if (buttonYES.Text != "")
                    {
                        m_Result = DialogResult.Yes;
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

            base.MessageBox_Base_KeyUp(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void ProgressRefresh()
        {
            ProgressRefresh(5);
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void ProgressRefresh(int sleep)
        {
            progressBar1.Value = (progressBar1.Value + 1) % (progressBar1.Maximum + 1);
            labelMessage.Refresh();
            this.Refresh();
            if (0 < sleep)
            {
                this.TopLevel = true;
                Application.DoEvents();
                Thread.Sleep(sleep);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual int ProgressMaximum
        {
            get { return progressBar1.Maximum; }
            set { progressBar1.Maximum = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual int ProgressValue
        {
            get { return progressBar1.Value; }
            set { progressBar1.Value = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual int ProgressMinimum
        {
            get { return progressBar1.Minimum; }
            set { progressBar1.Minimum = value; }
        }
    }
}
