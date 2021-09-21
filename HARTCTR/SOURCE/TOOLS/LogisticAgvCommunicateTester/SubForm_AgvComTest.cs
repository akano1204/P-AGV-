using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BelicsClass.UI;
using BelicsClass.UI.Controls;
using BelicsClass.Common;
using BelicsClass.File;

using AgvController;

namespace LogisticAgvCommunicateTester
{
    public partial class SubForm_AgvComTest : BL_SubForm_Base
    {
        private AgvController.AgvCommunicator agv = null;
        private string execFilePath = "";

        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "[F1]:保存", "[F2]:読込", "[F3]:削除", "[F4]:追加", "", "", "[F7]:S送信", "[F8]:送信", "", "[F10]:戻る", "", "" };
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

        public SubForm_AgvComTest()
        {
            InitializeComponent();
            Resizer_Initialize();
        }

        protected override void SubForm_Base_Function10_Clicked(object sender)
        {
            base.SubForm_Base_Function10_Clicked(sender);

            agv.ReceiveEvent -= Agv_ReceiveEvent;
            Close();
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            agv = (AgvController.AgvCommunicator)Tag;
            agv.ReceiveEvent += Agv_ReceiveEvent;

            textBoxNAME.Text = agv.Name;
            textBoxIP.Text = agv.RemoteHost.Address.ToString();
            textBoxRemoteHost.Text = agv.RemoteHost.Port.ToString();
            textBoxRemoteClient.Text = agv.RemoteClient.Port.ToString();

            AttachButton_to_Functions(buttonAdd, 4);
            AttachButton_to_Functions(buttonRemove, 3);
            AttachButton_to_Functions(buttonSendShot, 7);
            AttachButton_to_Functions(buttonSend, 8);

            listBoxCMD.Items.Clear();
            foreach (var v in Enum.GetValues(typeof(AgvController.AgvCommunicator.Order.CMD)))
            {
                listBoxCMD.Items.Add(v);
            }

            listBoxMOD.Items.Clear();
            foreach (var v in Enum.GetValues(typeof(AgvController.AgvCommunicator.Order.MOD)))
            {
                listBoxMOD.Items.Add(v);
            }
        }

        Queue<AgvController.AgvCommunicator.Order> response = new Queue<AgvController.AgvCommunicator.Order>();

        private void Agv_ReceiveEvent(AgvController.AgvCommunicator sender, AgvController.AgvCommunicator.ReceiveEventArgs e)
        {
            switch ((AgvCommunicator.State.CMD)e.state.cmd)
            {
                case AgvCommunicator.State.CMD.REQUEST:
                case AgvCommunicator.State.CMD.STATE:
                    {
                        AgvCommunicator.Order order = new AgvCommunicator.Order();
                        order.cmd = (ushort)AgvCommunicator.Order.CMD.RESPONSE;
                        order.deg = e.state.deg;
                        order.x = e.state.x;
                        order.y = e.state.y;
                        order.rack_deg = e.state.rack_deg;
                        order.seq_no = e.state.seq_no;

                        response.Enqueue(order);
                    }
                    break;
            }

            MethodInvoker process = (MethodInvoker)delegate ()
            {
                textBoxAgvCMD.Text = e.state.cmd.ToString();
                textBoxAgvSTA.Text = e.state.sta.ToString("X4") + " HEX";
                textBoxAgvBAT.Text = e.state.bat.ToString();
                textBoxAgvMAP.Text = e.state.map.ToString();
                textBoxAgvDEG.Text = e.state.deg.ToString();
                textBoxAgvXPOS.Text = e.state.x.ToString();
                textBoxAgvYPOS.Text = e.state.y.ToString();
                textBoxAgvRACK_DEG.Text = e.state.rack_deg.ToString();
                textBoxAgvRACKNO.Text = e.state.rack_no.ToString();
            };

            try
            {
                if (InvokeRequired) Invoke(process);
                else process.Invoke();
            }
            catch (Exception) { }
        }

        protected override void SubForm_Base_Function01_Clicked(object sender)
        {
            base.SubForm_Base_Function01_Clicked(sender);

            // ダイアログ表示
            var dialog = new SaveFileDialog();
            string fileName = Path.GetFileName(execFilePath);
            dialog.FileName = fileName.Length == 0 ? "新しいファイル.csv" : fileName; // 初期のファイル名
            dialog.InitialDirectory = execFilePath.Length == 0 ? Directory.GetCurrentDirectory() : execFilePath; // 初期の表示するフォルダ
            dialog.Filter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*"; // フィルタ
            dialog.FilterIndex = 1; // 1番目のフィルタをデフォルトにする
            dialog.RestoreDirectory = true; // ダイアログを閉じる前にもとのディレクトリを復元する

            if (dialog.ShowDialog() == DialogResult.OK) // 保存
            {
                BL_CsvFile file = new BL_CsvFile();
                foreach (var item in listviewPending.Items)
                {
                    AgvController.AgvCommunicator.Order order = (AgvController.AgvCommunicator.Order)item.Tag;

                    BL_CommaText text = new BL_CommaText();
                    text.Add(order.cmd.ToString());
                    text.Add(order.mod.ToString());
                    text.Add(order.deg.ToString());
                    text.Add(order.x.ToString());
                    text.Add(order.y.ToString());
                    text.Add(order.rack_deg.ToString());
                    file.Add(text);
                }
                file.Save(dialog.FileName);
                execFilePath = dialog.FileName;
            }   
        }

        protected override void SubForm_Base_Function02_Clicked(object sender)
        {
            base.SubForm_Base_Function02_Clicked(sender);

            // ダイアログ表示
            var dialog = new OpenFileDialog();
            dialog.InitialDirectory = execFilePath.Length == 0 ? Directory.GetCurrentDirectory() : execFilePath; // 初期の表示するフォルダ
            dialog.Filter = "CSVファイル (*.csv)|*.csv|すべてのファイル (*.*)|*.*"; // フィルタ
            dialog.FilterIndex = 1; // 1番目のフィルタをデフォルトにする
            dialog.RestoreDirectory = true; // ダイアログを閉じる前にもとのディレクトリを復元する

            if (dialog.ShowDialog() == DialogResult.OK) // 読込
            {
                BL_CsvFile file = new BL_CsvFile();
                file.Open(dialog.FileName);

                listviewPending.Items.Clear();
                listviewPending.RefreshMe();
                for (int i = 0; i < file.Count; i++)
                {
                    BL_CommaText text = file[i];

                    if (text.Count != 6) continue;

                    AgvController.AgvCommunicator.Order order = new AgvController.AgvCommunicator.Order();

                    if (!ushort.TryParse(text[0], out order.cmd)) return;
                    if (!ushort.TryParse(text[1], out order.mod)) return;
                    if (!Int16.TryParse(text[2], out order.deg)) return;
                    if (!Int32.TryParse(text[3], out order.x)) return;
                    if (!Int32.TryParse(text[4], out order.y)) return;
                    if (!Int16.TryParse(text[5], out order.rack_deg)) return;

                    BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                    item.Tag = order;
                    item.Add(order.cmd);
                    item.Add(order.mod.ToString("X2") + " HEX");
                    item.Add(order.deg);
                    item.Add(order.x);
                    item.Add(order.y);
                    item.Add(order.rack_deg);

                    listviewPending.Items.Add(item);
                }
                listviewPending.RefreshMe();
                execFilePath = dialog.FileName;
            }
        }

        protected override void SubForm_Base_Function03_Clicked(object sender)
        {
            base.SubForm_Base_Function03_Clicked(sender);

            if (listviewPending.SelectedIndices.Count == 0) return;

            listviewPending.Items.RemoveAt(listviewPending.SelectedIndices[0]);
            listviewPending.RefreshMe();
        }

        protected override void SubForm_Base_Function04_Clicked(object sender)
        {
            base.SubForm_Base_Function04_Clicked(sender);

            if (listBoxCMD.SelectedItem == null) return;

            AgvController.AgvCommunicator.Order order = new AgvController.AgvCommunicator.Order();
            order.cmd = (UInt16)listBoxCMD.SelectedItem;
            order.mod = 0;
            foreach (var v in listBoxMOD.SelectedItems)
            {
                order.mod |= (UInt16)v;
            }

            if (!Int16.TryParse(textBoxDEG.Text, out order.deg)) return;
            if (!Int32.TryParse(textBoxXPOS.Text, out order.x)) return;
            if (!Int32.TryParse(textBoxYPOS.Text, out order.y)) return;
            if (!Int16.TryParse(textBoxRACK_DEG.Text, out order.rack_deg)) return;

            BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
            item.Tag = order;
            item.Add(order.cmd);
            item.Add(order.mod.ToString("X2") + " HEX");
            item.Add(order.deg);
            item.Add(order.x);
            item.Add(order.y);
            item.Add(order.rack_deg);

            listviewPending.Items.Add(item);
            listviewPending.RefreshMe();
        }

        protected override void SubForm_Base_Function07_Clicked(object sender)
        {
            base.SubForm_Base_Function07_Clicked(sender);

            //if (buttonConnect.Text != "切断") return;
            if (listBoxCMD.SelectedItem == null) return;

            AgvController.AgvCommunicator.Order order = new AgvController.AgvCommunicator.Order();
            order.cmd = (UInt16)listBoxCMD.SelectedItem;
            order.mod = 0;
            foreach (var v in listBoxMOD.SelectedItems)
            {
                order.mod |= (UInt16)v;
            }

            if (!Int16.TryParse(textBoxDEG.Text, out order.deg)) return;
            if (!Int32.TryParse(textBoxXPOS.Text, out order.x)) return;
            if (!Int32.TryParse(textBoxYPOS.Text, out order.y)) return;
            if (!Int16.TryParse(textBoxRACK_DEG.Text, out order.rack_deg)) return;

            List<AgvController.AgvCommunicator.Order> orders = new List<AgvController.AgvCommunicator.Order>();
            orders.Add(order);

            agv.SetOrder(orders.ToArray());
        }

        protected override void SubForm_Base_Function08_Clicked(object sender)
        {
            base.SubForm_Base_Function08_Clicked(sender);

            //if (buttonConnect.Text != "切断") return;
            if (listviewPending.Items.Count == 0) return;

            List<AgvController.AgvCommunicator.Order> orders = new List<AgvController.AgvCommunicator.Order>();
            foreach (var v in listviewPending.Items)
            {
                orders.Add((AgvController.AgvCommunicator.Order)v.Tag);
            }

            agv.SetOrder(orders.ToArray());
        }

        private void timerResponse_Tick(object sender, EventArgs e)
        {
            while (0 < response.Count)
            {
                agv.SetOrder(new AgvCommunicator.Order[] { response.Dequeue() });
            }
        }
    }
}
