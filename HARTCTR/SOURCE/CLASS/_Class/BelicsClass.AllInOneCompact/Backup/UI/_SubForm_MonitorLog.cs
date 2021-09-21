using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BelicsClass.Common;
using BelicsClass.ProcessManage;
using BelicsClass.UI;
using BelicsClass.UI.Controls;
using BelicsClass.ObjectSync;

namespace BelicsClass.UI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class BL_SubForm_MonitorLog : BelicsClass.UI.BL_SubForm_Base
    {
        private bool noreadmemory = false;

        /// <summary></summary>
        protected BL_FaceMemorySyncNotify mem = null;

        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            if (mem != null)
            {
                return new string[] { "", "[F1]:ﾒﾓﾘ", "", "", "", "", "", "", "[F8]:ﾛｸﾞ停止", "[F9]:ﾛｸﾞ再開", "", "", "[F12]:閉じる" };
            }
            
            return new string[] { "", "", "", "", "", "", "", "", "[F8]:ﾛｸﾞ停止", "[F9]:ﾛｸﾞ再開", "", "", "[F12]:閉じる" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get { return this.Text; }
        }

        /// <summary>
        /// 
        /// </summary>
        public BL_SubForm_MonitorLog()
        {
            InitializeComponent();
            Resizer_Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mem"></param>
        public BL_SubForm_MonitorLog(BL_FaceMemorySyncNotify mem)
            : this(mem, false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mem"></param>
        /// <param name="noreadmemory"></param>
        public BL_SubForm_MonitorLog(BL_FaceMemorySyncNotify mem, bool noreadmemory)
            : this()
        {
            this.mem = mem;
            this.noreadmemory = noreadmemory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void timerClock_Tick(object sender, EventArgs e)
        {
            int addcount = 0;
            BL_ThreadController_Base.BL_Histories lastlog = null;
            SortedList<string, BL_VirtualListView.BL_VirtualListViewItem> list = new SortedList<string, BL_VirtualListView.BL_VirtualListViewItem>();

            for (int i = 0; i < BL_ThreadCollector.Count; i++)
            {
                BL_ThreadController_Base thread = BL_ThreadCollector.Get(i);

                while (true)
                {
                    BL_ThreadController_Base.BL_Histories log = thread.LogGet();
                    if (log == null) break;

                    BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                    item.Tag = log;
                    item.Add(log.timestamp.ToString("yyyy/MM/dd HH:mm:ss.fff"));
                    item.Add(log.callfrom);
                    item.Add(log.description);

                    list.Add(log.timestamp.ToString("yyyy/MM/dd HH:mm:ss.fff - ") + addcount.ToString(), item);
                    addcount++;
                    lastlog = log;
                }

                {
                    bool found = false;
                    for (int ii = 0; ii < lvwThread.SortedItems.Count; ii++)
                    {
                        if (thread == lvwThread.SortedItems[ii].Tag)
                        {
                            BL_VirtualListView.BL_VirtualListViewItem item = lvwThread.SortedItems[ii];
                            item[1] = thread.Step;
                            if (thread.IsSuspend)
                            {
                                item[2] = 0;
                                item[3] = "停止中";
                            }
                            else
                            {
                                item[2] = thread.PassedCount;
                                item[3] = (thread.ScanTime).ToString("0.0") + " ms";
                            }
                            item[4] = thread.CanExit ? "○" : "×";
                            item[5] = thread.IsDoing ? "○" : "×";

                            found = true;
                        }
                    }

                    if (!found)
                    {
                        BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                        item.Tag = thread;
                        item.Add(thread);
                        item.Add(thread.Step);
                        if (thread.IsSuspend)
                        {
                            item.Add(0);
                            item.Add("停止中");
                        }
                        else
                        {
                            item.Add(thread.PassedCount);
                            item.Add((thread.ScanTime).ToString("0.0") + " ms");
                        }
                        item.Add(thread.CanExit ? "○" : "×");
                        item.Add(thread.IsDoing ? "○" : "×");
                        lvwThread.Items.Add(item);
                    }
                }
            }
            
            lvwThread.RefreshMe(false);

            if (0 < addcount)
            {
                m_Mainform.notifyIconMinimum.BalloonTipText = lastlog.timestamp.ToString("yyyy/MM/dd HH:mm:ss.fff") + "\n" + lastlog.description;

                lvwLog.Items.InsertLimit(0, list, 1000);
                lvwLog.RefreshMe(false);
            }

            timerClock.Interval = 100;
            base.timerClock_Tick(sender, e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected override void SubForm_Base_Function01_Clicked(object sender)
        {
            base.SubForm_Base_Function01_Clicked(sender);

            if (mem != null)
            {
                m_Mainform.IsCompareTitleEnable = true;
                BL_SubForm_Base sub = new BL_SubForm_MonitorMemory(mem, true, noreadmemory);
                sub.ShowMe(this);
                m_Mainform.IsCompareTitleEnable = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected override void SubForm_Base_Function08_Clicked(object sender)
        {
            base.SubForm_Base_Function08_Clicked(sender);
            timerClock.Enabled = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected override void SubForm_Base_Function09_Clicked(object sender)
        {
            base.SubForm_Base_Function09_Clicked(sender);
            timerClock.Enabled = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);
            if (1 < m_Mainform.SubForms_Count) Close();
        }
    }
}
