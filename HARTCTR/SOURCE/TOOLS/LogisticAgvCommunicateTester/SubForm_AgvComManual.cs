using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BelicsClass.Network;
using BelicsClass.UI;
using BelicsClass.UI.Controls;

namespace LogisticAgvCommunicateTester
{
    public partial class SubForm_AgvComManual : BL_SubForm_Base
    {
        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "[F1]:接続", "[F2]:AGV追加", "[F3]:AGV削除", "", "[F5]:通信ﾃｽﾄ", "[F6]:AGV模擬", "[F7]:上位通信", "", "", "", "", "[F12]:終了" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get
            {
                return this.Text;
            }
        }

        private BL_RawSocketUDP recv = null;
        private BL_RawSocketUDP send = null;

        public SubForm_AgvComManual()
        {
            InitializeComponent();
            Resizer_Initialize();
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            AttachButton_to_Functions(buttonConnect, 1);
            AttachButton_to_Functions(buttonAgvAdd, 2);
            AttachButton_to_Functions(buttonAgvRemove, 3);
            AttachButton_to_Functions(buttonDetail, 5);
            AttachButton_to_Functions(buttonAgvSim, 6);
            AttachButton_to_Functions(buttonOrderCommander, 7);
        }

        protected override void SubForm_Base_Function01_Clicked(object sender)
        {
            base.SubForm_Base_Function01_Clicked(sender);

            if (recv == null)
            {
                int local_host = 0;int.TryParse(textBoxLocalHost.Text, out local_host);
                int local_client = 0;int.TryParse(textBoxLocalClient.Text, out local_client);

                recv = new BL_RawSocketUDP(local_host);
                send = new BL_RawSocketUDP(local_client);
                recv.Open(BL_RawSocketUDP.FormatType.BL_IDSIZE);
                send.Open(BL_RawSocketUDP.FormatType.BL_IDSIZE);

                buttonConnect.Text = "[F1] 切断";
                m_Mainform.btnFunctions[1].Text = "[F1]\n切断";
            }
            else
            {
                recv.Close();
                send.Close();
                recv = null;
                send = null;

                buttonConnect.Text = "[F1] 接続";
                m_Mainform.btnFunctions[1].Text = "[F1]\n接続";
            }
        }

        protected override void SubForm_Base_Function02_Clicked(object sender)
        {
            base.SubForm_Base_Function02_Clicked(sender);

            if (recv == null || send == null) return;

            string name = textBoxNAME.Text;
            string ip = textBoxIP.Text;

            foreach (var v in listviewAgv.Items)
            {
                if ((string)v[0] == name) return;
                if ((string)v[1] == ip) return;
            }


            int remote_client = 0; if (!int.TryParse(textBoxRemoteClient.Text, out remote_client)) return;
            int remote_host = 0; if (!int.TryParse(textBoxRemoteHost.Text, out remote_host)) return;
            int local_client = 0; if (!int.TryParse(textBoxLocalClient.Text, out local_client)) return;
            int local_host = 0; if (!int.TryParse(textBoxLocalHost.Text, out local_host)) return;

            AgvController.AgvCommunicator agv = new AgvController.AgvCommunicator(name, ip, remote_client, remote_host, recv, send);
            agv.ReceiveEvent += Agv_ReceiveEvent;
            agv.StartControl(50);

            BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
            item.Add(name);
            item.Add(ip);
            item.Add(remote_host);
            item.Add(remote_client);
            item.Add("");
            item.Add("");
            item.Add("");
            item.Add("");
            item.Add("");
            item.Add("");
            item.Add("");
            item.Add("");
            item.Add("");
            item.Tag = agv;

            listviewAgv.Items.Add(item);
            listviewAgv.RefreshMe();
        }

        protected override void SubForm_Base_Function03_Clicked(object sender)
        {
            base.SubForm_Base_Function03_Clicked(sender);

            if (listviewAgv.SelectedIndices.Count == 0) return;

            AgvController.AgvCommunicator agv = (AgvController.AgvCommunicator)listviewAgv.SelectedItems[0].Tag;

            agv.StopControl();
            listviewAgv.SelectedItems[0].Tag = null;

            listviewAgv.Items.RemoveAt(listviewAgv.SelectedIndices[0]);
            listviewAgv.RefreshMe();
        }


        protected override void SubForm_Base_Function05_Clicked(object sender)
        {
            base.SubForm_Base_Function05_Clicked(sender);

            if (listviewAgv.SelectedIndices.Count == 0) return;

            AgvController.AgvCommunicator agv = (AgvController.AgvCommunicator)listviewAgv.SelectedItems[0].Tag;

            SubForm_AgvComTest sub = new SubForm_AgvComTest();
            sub.Tag = agv;
            sub.ShowMe(this);
        }

        protected override void SubForm_Base_Function06_Clicked(object sender)
        {
            base.SubForm_Base_Function06_Clicked(sender);

            SubForm_AgvStaSender sub = new SubForm_AgvStaSender();
            sub.ShowMe(this);
        }

        protected override void SubForm_Base_Function07_Clicked(object sender)
        {
            base.SubForm_Base_Function07_Clicked(sender);

            SubForm_OrderCommander sub = new SubForm_OrderCommander();
            sub.ShowMe(this);
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);
            ExitApplication();
        }

        private void Agv_ReceiveEvent(AgvController.AgvCommunicator sender, AgvController.AgvCommunicator.ReceiveEventArgs e)
        {
            MethodInvoker process = (MethodInvoker)delegate ()
            {
                foreach (var v in listviewAgv.Items)
                {
                    if (v[0].ToString() == sender.Name)
                    {
                        v[4] = e.state.cmd.ToString();
                        v[5] = e.state.sta.ToString("X4") + " HEX";
                        v[6] = e.state.bat.ToString();
                        v[7] = e.state.map.ToString();
                        v[8] = e.state.deg.ToString();
                        v[9] = e.state.x.ToString();
                        v[10] = e.state.y.ToString();
                        v[11] = e.state.rack_deg.ToString();
                        v[12] = e.state.rack_no.ToString();

                        listviewAgv.NeedRedraw = true;
                    }
                }

                listviewAgv.RefreshMe(false);
            };

            try
            {
                if (InvokeRequired) Invoke(process);
                else process.Invoke();
            }
            catch (Exception) { }
        }
    }
}
