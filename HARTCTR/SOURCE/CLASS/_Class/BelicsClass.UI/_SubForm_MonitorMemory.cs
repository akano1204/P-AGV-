using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using BelicsClass.Common;
using BelicsClass.ObjectSync;
using BelicsClass.UI.Controls;

namespace BelicsClass.UI
{
    /// <summary>
    /// 
    /// </summary>
    public partial class BL_SubForm_MonitorMemory : BelicsClass.UI.BL_SubForm_Base
    {
        private bool noreadmemory = false;
        private bool resizer = true;

        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "", "[F2]:ﾛｸﾞ", "", "[F4]:前へ", "[F5]:次へ", "", "", "[F8]:変更", "", "", "", "[F12]:閉じる" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get
            {
                if (mem != null) return this.Text + "[" + mem.SharememName + "]";
                return this.Text;
            }
        }

        /// <summary></summary>
        protected BL_FaceMemorySyncNotify mem = null;

        /// <summary>
        /// 
        /// </summary>
        public BL_SubForm_MonitorMemory()
            : this(null, true, false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public BL_SubForm_MonitorMemory(bool resizer)
            : this(null, resizer, false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mem"></param>
        public BL_SubForm_MonitorMemory(BL_FaceMemorySyncNotify mem)
            : this(mem, true, false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mem"></param>
        /// <param name="resizer"></param>
        public BL_SubForm_MonitorMemory(BL_FaceMemorySyncNotify mem, bool resizer)
            : this(mem, resizer, false)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="mem"></param>
        /// <param name="resizer"></param>
        /// <param name="noreadmemory"></param>
        public BL_SubForm_MonitorMemory(BL_FaceMemorySyncNotify mem, bool resizer, bool noreadmemory)
        {
            InitializeComponent();

            this.resizer = resizer;
            if (resizer) Resizer_Initialize();

            this.mem = mem;
            this.noreadmemory = noreadmemory;
        }

        private void NodeAdd(TreeNode node, Dictionary<string, BL_ObjectSync.BL_ObjectInformation> info)
        {
            if (mem == null) return;

            foreach (var v in info)
            {
                string[] field = v.Key.Split('.');

                TreeNode subnode = node;
                for (int i = 0; i < field.Length - 1; i++)
                {
                    string f = field[i];
                    if (!subnode.Nodes.ContainsKey(f))
                    {
                        subnode = subnode.Nodes.Add(f, f);
                    }
                    else
                    {
                        subnode = subnode.Nodes[f];
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            if (m_Mainform != null)
            {
                m_Mainform.Title = _TitleString;
            }
            else
            {
                this.ShowInTaskbar = true;
                this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
                this.ControlBox = true;
                this.MinimizeBox = true;
                this.MaximizeBox = true;
            }

            if (mem != null)
            {
                TreeNode node = treeView1.Nodes.Add("", "All");
                NodeAdd(node, mem.GetFieldDictionary());

                make_list();
                if (0 < treeView1.Nodes.Count) treeView1.Nodes[0].Expand();

                mem.Owner = this;
                mem.EventModified += mem_EventModified;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void SubForm_Base_Activated(object sender, EventArgs e)
        {
            base.SubForm_Base_Activated(sender, e);

            mem_EventModified(mem, mem.GetAllFieldNames());
        }

        private void make_list()
        {
            lvwField.Items.Clear();
            lvwField.RefreshMe();

            int no = 0;
            foreach (var v in mem.GetFieldDictionary())
            {
                if (target_node != "")
                {
                    string[] fields = v.Key.Split('.');
                    if (fields.Length < 2) continue;

                    string[] targets = target_node.Split('.');
                    if (fields.Length - 1 != targets.Length) continue;

                    bool ignore = false;
                    for (int i = 0; i < targets.Length; i++)
                    {
                        if (fields[i] != targets[i])
                        {
                            ignore = true;
                            break;
                        }
                    }

                    if (ignore) continue;
                }
                
                no++;
                BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();

                item.Add("");
                item.Add(no);
                item.Add(v.Key);
                if (typeof(DateTime).IsInstanceOfType(v.Value.GetData()))
                {
                    item.Add(((DateTime)v.Value.GetData()).ToString("yyyy/MM/dd HH:mm:ss.fff"));
                }
                else if (typeof(Color).IsInstanceOfType(v.Value.GetData()))
                {
                    BL_VirtualListView.BL_LvwCell cell = new BL_VirtualListView.BL_LvwCell((Color)v.Value.GetData());
                    cell.BackColor = (Color)cell.Data;
                    item.Add(cell);
                }
                else
                {
                    item.Add(v.Value.GetData().ToString());
                }
                item.Add(v.Value.Length);

                byte[] buffer = v.Value.GetBuffer();
                string str = "";
                foreach (byte b in buffer)
                {
                    if (str != "") str += " ";
                    str += b.ToString("X2");
                }
                item.Add(str);
                item.Tag = v;

                lvwField.Items.Add(item);
            }

            lvwField.RefreshMe();

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected override void SubForm_Base_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mem != null)
            {
                mem.Owner = null;
                mem.EventModified -= mem_EventModified;
            }

            base.SubForm_Base_FormClosing(sender, e);
        }

        void mem_EventModified(BL_FaceMemorySyncNotify sender, string[] field_names)
        {
            MethodInvoker process = (MethodInvoker)delegate()
            {
                foreach (string field_name in field_names)
                {
                    foreach (var item in lvwField.SortedItems)
                    {
                        var kv = (KeyValuePair<string, BL_ObjectSync.BL_ObjectInformation>)item.Tag;

                        if (kv.Key == field_name)
                        {
                            if (!noreadmemory) sender.ReadMemory(field_name, false);
                            
                            if (typeof(DateTime).IsInstanceOfType(kv.Value.GetData()))
                            {
                                item[3] = ((DateTime)kv.Value.GetData()).ToString("yyyy/MM/dd HH:mm:ss.fff");
                            }
                            else if (typeof(Color).IsInstanceOfType(kv.Value.GetData()))
                            {
                                BL_VirtualListView.BL_LvwCell cell = new BL_VirtualListView.BL_LvwCell((Color)kv.Value.GetData());
                                cell.BackColor = (Color)kv.Value.GetData();
                                item[3] = cell;
                            }
                            else
                            {
                                item[3] = kv.Value.GetData().ToString();
                            }

                            byte[] buffer = kv.Value.GetBuffer();
                            string str = "";
                            foreach (byte b in buffer)
                            {
                                if (str != "") str += " ";
                                str += b.ToString("X2");
                            }
                            item[5] = str;
                        }
                    }
                }

                if (0 < field_names.Length) lvwField.RefreshMe(false);
            };

            try
            {
                if (this.InvokeRequired) this.Invoke(process);
                else process.Invoke();
            }
            catch (ObjectDisposedException) { }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected override void SubForm_Base_Function02_Clicked(object sender)
        {
            base.SubForm_Base_Function02_Clicked(sender);


            if (m_Mainform != null) m_Mainform.IsCompareTitleEnable = true;
            BL_SubForm_Base sub = new BL_SubForm_MonitorLog(mem);
            sub.ShowMe(this);
            if (m_Mainform != null) m_Mainform.IsCompareTitleEnable = false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected override void SubForm_Base_Function08_Clicked(object sender)
        {
            if (lvwField.SelectedItems.Count == 0) return;

            KeyValuePair<string, BL_ObjectSync.BL_ObjectInformation> kv = (KeyValuePair<string, BL_ObjectSync.BL_ObjectInformation>)lvwField.SelectedItems[0].Tag;

            if (kv.Value.GetData().GetType() == typeof(bool))
            {
                bool val = checkBoxBoolean.Checked;
                kv.Value.SetData(val);
            }
            else if (kv.Value.GetData().GetType() == typeof(byte))
            {
                byte val;
                if (byte.TryParse(textBoxUpdateValue.Text, out val))
                {
                    kv.Value.SetData(val);
                }
            }
            else if (kv.Value.GetData().GetType() == typeof(char))
            {
                if (0 < textBoxUpdateValue.Text.Length)
                {
                    kv.Value.SetData(textBoxUpdateValue.Text[0]);
                }
            }
            else if (kv.Value.GetData().GetType() == typeof(short))
            {
                short val;
                if (short.TryParse(textBoxUpdateValue.Text, out val))
                {
                    kv.Value.SetData(val);
                }
            }
            else if (kv.Value.GetData().GetType() == typeof(ushort))
            {
                ushort val;
                if (ushort.TryParse(textBoxUpdateValue.Text, out val))
                {
                    kv.Value.SetData(val);
                }
            }
            else if (kv.Value.GetData().GetType() == typeof(int))
            {
                int val;
                if (int.TryParse(textBoxUpdateValue.Text, out val))
                {
                    kv.Value.SetData(val);
                }
            }
            else if (kv.Value.GetData().GetType() == typeof(uint))
            {
                uint val;
                if (uint.TryParse(textBoxUpdateValue.Text, out val))
                {
                    kv.Value.SetData(val);
                }
            }
            else if (kv.Value.GetData().GetType() == typeof(long))
            {
                long val;
                if (long.TryParse(textBoxUpdateValue.Text, out val))
                {
                    kv.Value.SetData(val);
                }
            }
            else if (kv.Value.GetData().GetType() == typeof(ulong))
            {
                ulong val;
                if (ulong.TryParse(textBoxUpdateValue.Text, out val))
                {
                    kv.Value.SetData(val);
                }
            }
            else if (kv.Value.GetData().GetType() == typeof(float))
            {
                float val;
                if (float.TryParse(textBoxUpdateValue.Text, out val))
                {
                    kv.Value.SetData(val);
                }
            }
            else if (kv.Value.GetData().GetType() == typeof(double))
            {
                double val;
                if (double.TryParse(textBoxUpdateValue.Text, out val))
                {
                    kv.Value.SetData(val);
                }
            }
            else if (kv.Value.GetData().GetType() == typeof(String))
            {
                kv.Value.SetData(textBoxUpdateValue.Text);
            }
            else if (kv.Value.GetData().GetType() == typeof(DateTime))
            {
                long ticks = 0;
                DateTime dt = new DateTime();
                if (DateTime.TryParse(textBoxUpdateValue.Text, out dt))
                {
                    kv.Value.SetData(dt);
                }
                else if (long.TryParse(textBoxUpdateValue.Text, out ticks))
                {
                    dt = new DateTime(ticks);
                    kv.Value.SetData(dt);
                }
            }
            else if (kv.Value.GetData().GetType() == typeof(TimeSpan))
            {
                int millis = 0;
                TimeSpan ts = new TimeSpan();
                if (TimeSpan.TryParse(textBoxUpdateValue.Text, out ts))
                {
                    kv.Value.SetData(ts);
                }
                else if (int.TryParse(textBoxUpdateValue.Text, out millis))
                {
                    ts = new TimeSpan(0, 0, 0, 0, millis);
                    kv.Value.SetData(ts);
                }
            }
            else if (kv.Value.GetData().GetType() == typeof(BL_BitOperator))
            {
                BL_BitOperator bit = new BL_BitOperator();
                for (int i = 0; i < textBoxUpdateValue.Text.Trim().Length; i++)
                {
                    int pos = textBoxUpdateValue.Text.Trim().Length - i - 1;
                    if (textBoxUpdateValue.Text.Trim()[pos] != '0')
                    {
                        bit[i] = true;
                    }
                }
                kv.Value.SetData(bit);
            }
            else if (kv.Value.GetData().GetType() == typeof(Color))
            {
                Color col = Color.FromName(textBoxUpdateValue.Text);

                if (col.A==0 && col.R == 0 && col.G == 0 && col.B == 0)
                {
                    string sc = textBoxUpdateValue.Text.Replace("Color [", "");
                    sc = sc.Replace("]", "");
                    col = Color.FromName(sc);
                }

                if (col.A == 0 && col.R == 0 && col.G == 0 && col.B == 0)
                {
                    string sc = textBoxUpdateValue.Text.Replace("Color [A=", "");
                    sc = sc.Replace(" R=", "");
                    sc = sc.Replace(" G=", "");
                    sc = sc.Replace(" B=", "");
                    sc = sc.Replace("]", "");
                    string[] cols = sc.Split(',');

                    int A = 0xff;
                    int R = 0;
                    int G = 0;
                    int B = 0;

                    if (4 <= cols.Length)
                    {
                        for (int i = 0; i < cols.Length; i++)
                        {
                            switch (i)
                            {
                                case 0: int.TryParse(cols[i], out A); break;
                                case 1: int.TryParse(cols[i], out R); break;
                                case 2: int.TryParse(cols[i], out G); break;
                                case 3: int.TryParse(cols[i], out B); break;
                            }
                        }
                        col = Color.FromArgb(A, R, G, B);
                    }
                    else if (3 <= cols.Length)
                    {
                        for (int i = 0; i < cols.Length; i++)
                        {
                            switch (i)
                            {
                                case 0: int.TryParse(cols[i], out R); break;
                                case 1: int.TryParse(cols[i], out G); break;
                                case 2: int.TryParse(cols[i], out B); break;
                            }
                        }
                        col = Color.FromArgb(R, G, B);
                    }
                }

                kv.Value.SetData(col);
            }
            else return;

            if (!noreadmemory) mem.WriteMemory(kv.Key);

            mem_EventModified(mem, new string[] { kv.Key });

            base.SubForm_Base_Function08_Clicked(sender);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);
            if (m_Mainform != null) if (1 < m_Mainform.SubForms_Count) Close();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxBoolean_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxBoolean.Checked) checkBoxBoolean.Text = "True";
            else checkBoxBoolean.Text = "False";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lvwField_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lvwField.SelectedItems.Count == 0)
            {
                textBoxUpdateValue.Enabled = false;
                checkBoxBoolean.Enabled = false;
                return;
            }

            KeyValuePair<string, BL_ObjectSync.BL_ObjectInformation> kv = (KeyValuePair<string, BL_ObjectSync.BL_ObjectInformation>)lvwField.SelectedItems[0].Tag;

            if (kv.Value.GetData().GetType() == typeof(bool))
            {
                textBoxUpdateValue.Enabled = false;
                checkBoxBoolean.Enabled = true;

                checkBoxBoolean.Checked = (bool)kv.Value.GetData();
            }
            else if (kv.Value.GetData().GetType() == typeof(Color))
            {
                textBoxUpdateValue.Enabled = true;
                checkBoxBoolean.Enabled = false;

                textBoxUpdateValue.Text = ((Color)kv.Value.GetData()).ToString();
            }
            else
            {
                textBoxUpdateValue.Enabled = true;
                checkBoxBoolean.Enabled = false;

                textBoxUpdateValue.Text = kv.Value.GetData().ToString();
            }
        }


        private string target_node = "";
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = e.Node;
            target_node = "";
            for (; node != null; node = node.Parent)
            {
                if (node.Parent != null)
                {
                    if (target_node != "") target_node = "." + target_node;
                    target_node = node.Text + target_node;
                }
            }

            make_list();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected override void SubForm_Base_Function04_Clicked(object sender)
        {
            base.SubForm_Base_Function04_Clicked(sender);

            for (int i = BelicsClass.ProcessManage.BL_ThreadCollector.Count - 1; 0 <= i; i--)
            {
                BelicsClass.ProcessManage.BL_ThreadController_Base thread = BelicsClass.ProcessManage.BL_ThreadCollector.Get(i);
                if (typeof(BelicsClass.ObjectSync.BL_FaceMemorySyncNotify.threadNotify).IsInstanceOfType(thread))
                {
                    BelicsClass.ObjectSync.BL_FaceMemorySyncNotify m = ((BelicsClass.ObjectSync.BL_FaceMemorySyncNotify.threadNotify)thread).source;
                    if (m == mem)
                    {
                        for (int ii = BelicsClass.ProcessManage.BL_ThreadCollector.Count - 1; 0 <= ii; ii--)
                        {
                            i--;
                            if (i < 0) i = BelicsClass.ProcessManage.BL_ThreadCollector.Count - 1;

                            thread = BelicsClass.ProcessManage.BL_ThreadCollector.Get(i);
                            if (typeof(BelicsClass.ObjectSync.BL_FaceMemorySyncNotify.threadNotify).IsInstanceOfType(thread))
                            {
                                Close();

                                thread = BelicsClass.ProcessManage.BL_ThreadCollector.Get(i);
                                m = ((BelicsClass.ObjectSync.BL_FaceMemorySyncNotify.threadNotify)thread).source;

                                BelicsClass.UI.BL_SubForm_Base sub = new BL_SubForm_MonitorMemory(m, resizer, noreadmemory);
                                sub.ShowMe(this);
                                return;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        protected override void SubForm_Base_Function05_Clicked(object sender)
        {
            base.SubForm_Base_Function05_Clicked(sender);

            for (int i = 0; i < BelicsClass.ProcessManage.BL_ThreadCollector.Count; i++)
            {
                BelicsClass.ProcessManage.BL_ThreadController_Base thread = BelicsClass.ProcessManage.BL_ThreadCollector.Get(i);
                if (typeof(BelicsClass.ObjectSync.BL_FaceMemorySyncNotify.threadNotify).IsInstanceOfType(thread))
                {
                    BelicsClass.ObjectSync.BL_FaceMemorySyncNotify m = ((BelicsClass.ObjectSync.BL_FaceMemorySyncNotify.threadNotify)thread).source;
                    if (m == mem)
                    {
                        for (int ii = 0; ii < BelicsClass.ProcessManage.BL_ThreadCollector.Count; ii++)
                        {
                            i++;
                            if (BelicsClass.ProcessManage.BL_ThreadCollector.Count <= i) i = 0;

                            thread = BelicsClass.ProcessManage.BL_ThreadCollector.Get(i);
                            if (typeof(BelicsClass.ObjectSync.BL_FaceMemorySyncNotify.threadNotify).IsInstanceOfType(thread))
                            {
                                Close();

                                thread = BelicsClass.ProcessManage.BL_ThreadCollector.Get(i);
                                m = ((BelicsClass.ObjectSync.BL_FaceMemorySyncNotify.threadNotify)thread).source;

                                BelicsClass.UI.BL_SubForm_Base sub = new BL_SubForm_MonitorMemory(m, resizer, noreadmemory);
                                sub.ShowMe(this);
                                return;
                            }
                        }
                    }
                }
            }
        }
    }
}
