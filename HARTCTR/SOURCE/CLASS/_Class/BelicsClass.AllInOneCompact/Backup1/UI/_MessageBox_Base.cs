using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;

namespace BelicsClass.UI
{
    /// <summary>
    /// メッセージボックスの基本クラスです。
    /// </summary>
    public partial class BL_MessageBox_Base : BL_SubForm_Base
    {
        /// <summary>スクリーンフィルター</summary>
        protected BL_ScreenFilter m_Filter = null;

        /// <summary>選択結果を保持します。</summary>
        public DialogResult m_Result = DialogResult.None;

        /// <summary>自画面の親となる子画面への参照を保持します。</summary>
        protected BL_SubForm_Base m_Owner = null;

        /// <summary>
        /// コンストラクタ(使用不可)
        /// </summary>
        public BL_MessageBox_Base()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="owner">自画面の親となる子画面(無ければnull)</param>
        public BL_MessageBox_Base(BL_SubForm_Base owner)
        {
            InitializeComponent();
            DoubleBuffered = true;
            if (owner != null)
            {
                m_Owner = owner;
                m_Mainform = owner.m_Mainform;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mainform">自画面の親となる親画面(無ければnull)</param>
        public BL_MessageBox_Base(BL_MainForm_Base mainform)
        {
            InitializeComponent();
            DoubleBuffered = true;
            
            if (mainform != null)
            {
                m_Owner = mainform.ActiveSubForm;
                m_Mainform = mainform;
            }
        }

        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <returns></returns>
        virtual public DialogResult ShowMessage()
        {
            return ShowMessage(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        virtual public DialogResult ShowProgress()
        {
            return ShowMessage(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bModal"></param>
        /// <returns></returns>
        virtual public DialogResult ShowMessage(bool bModal)
        {
            return ShowMessage(bModal, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        virtual public DialogResult ShowMessage(int interval)
        {
            if (0 < interval) return ShowMessage(true, interval);
            else return ShowMessage(false, interval);
        }

        /// <summary>
        /// メッセージボックスを表示します。
        /// </summary>
        /// <param name="bModal"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        virtual protected DialogResult ShowMessage(bool bModal, int interval)
        {
            bool located = false;

            if (m_Mainform != null)
            {
                m_Mainform.m_MessageBox.Add(this);
            }
            this.BringToFront();

            this.Visible = false;

            if (m_Owner != null)
            {
                Show(m_Owner);

                if (m_Owner.m_Functions != null)
                {
                    Location = new Point(m_Owner.Bounds.Location.X + m_Owner.Bounds.Width / 2 - Bounds.Width / 2, m_Owner.Bounds.Location.Y + m_Owner.Bounds.Height / 2 - Bounds.Height / 2);
                    located = true;
                }
            }
            else
            {
                Show(m_Mainform);
            }

            if (!located)
            {
                if (m_Mainform != null)
                {
                    Location = new Point(m_Mainform.Bounds.Location.X + m_Mainform.Bounds.Width / 2 - Bounds.Width / 2, m_Mainform.Bounds.Location.Y + m_Mainform.Bounds.Height / 2 - Bounds.Height / 2);
                }
                else
                {
                    Rectangle owner_rect = DesktopBounds;
                    if (m_Mainform != null) owner_rect = m_Mainform.DesktopBounds;

                    Screen target_desktop = Screen.PrimaryScreen;
                    foreach (Screen scr in Screen.AllScreens)
                    {
                        if (scr.Bounds.IntersectsWith(owner_rect))
                        {
                            target_desktop = scr;
                            break;
                        }
                    }

                    Location = new Point(target_desktop.Bounds.X + target_desktop.Bounds.Width / 2 - Width / 2, target_desktop.Bounds.Y + target_desktop.Bounds.Height / 2 - Height / 2);
                }
            }

            this.Visible = true;

            if (m_Owner == null)
            {
                //防護フィルターを生成
                m_Filter = new BL_ScreenFilter();
                m_Filter.ShowKeep(this);
            }
            else if (m_Owner.m_Functions == null)
            {
                //防護フィルターを生成
                m_Filter = new BL_ScreenFilter();
                m_Filter.ShowKeep(this);
            }

            //this.TopMost = true;

            this.TopLevel = true;
            this.Focus();
            this.BringToFront();

            if (bModal)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                while (m_Result == DialogResult.None)
                {
                    Application.DoEvents();
                    //Thread.Sleep(100);

                    if (0 < interval && interval < sw.ElapsedMilliseconds)
                    {
                        m_Result = DialogResult.Ignore;
                        break;
                    }
                }

                sw.Stop();
                sw = null;

                Close();
            }
            else
            {
                Refresh();
            }

            return m_Result;
        }

        private void MessageBox_Base_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void MessageBox_Base_KeyDown(object sender, KeyEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected virtual void MessageBox_Base_KeyUp(object sender, KeyEventArgs e)
        {

        }

        private void MessageBox_Base_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                SendKeys.Send("{TAB}");
                e.Handled = true;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        virtual protected void MessageBox_Base_MouseMove(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        virtual protected void panel1_MouseMove(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        virtual protected void panel2_MouseMove(object sender, MouseEventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void SubForm_Base_FormClosed_Func(object sender, FormClosedEventArgs e)
        {
            if (m_Filter != null)
            {
                m_Filter.Close();
                m_Filter = null;
            }

            if (m_Mainform != null)
            {
                m_Mainform.m_MessageBox.Remove(this);
                if (m_Owner != null)
                {
                    m_Owner.Focus();
                    m_Owner.BringToFront();
                }
                else
                {
                    if (0 < m_Mainform.SubForms_Count)
                    {
                        m_Mainform.TopSubForm_Focus();
                    }
                }
            }

            if (m_Mainform != null)
            {
                m_Mainform.RemoveSubForm(this);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void SubForm_Base_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!this.IsHandleCreated) return;

            MethodInvoker process = (MethodInvoker)delegate()
            {
                SubForm_Base_FormClosed_Func(sender, e);
            };

            if (this.InvokeRequired)
            {
                this.Invoke(process);
            }
            else
            {
                process.Invoke();
            }

            //base.SubForm_Base_FormClosed(sender, e);
        }

        private void MessageBox_Base_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            this.BringToFront();
            this.Focus();

            //this.TopLevel = true;
            //!this.TopMost = true;
        }
    }
}
