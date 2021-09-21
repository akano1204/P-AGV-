using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;

using BelicsClass.Common;
using BelicsClass.UI;
using BelicsClass.Network;
using BelicsClass.UI.Controls;

using AgvController;
using PROGRAM;

namespace LogisticAgvCommunicateTester
{
    public partial class SubForm_OrderCommander : BelicsClass.UI.BL_SubForm_Base
    {
        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "[F1]:接続", "", "", "", "", "", "", "", "", "", "", "[F12]:戻る" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get { return this.Text; }
        }

        BL_TcpP2PClient client = null;
        int seqno = 0;

        BelicsClass.File.BL_IniFile ini_hokusho = new BelicsClass.File.BL_IniFile(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"..\DATA\HOKUSHO.INI"));
        AgvControlManager controller = null;

        public SubForm_OrderCommander()
        {
            InitializeComponent();
            Resizer_Initialize();
        }

        class ListRack
        {
            public AgvControlManager.Rack rack = null;

            public override string ToString()
            {
                return rack.rack_no;
            }
        }

        class ListStation
        {
            public AgvControlManager.FloorQR qr = null;

            public override string ToString()
            {
                return qr.station_id;
            }
        }

        private void Client_EventReceived(BL_TcpP2P sender)
        {
            byte[] data = sender.Receive();
            if (3 <= data.Length)
            {
                string req_string = Encoding.ASCII.GetString(data, 0, 3).Trim();
                Log("R[" + req_string + "]");

                foreach (AgvOrderCommunicator.enREQ v in Enum.GetValues(typeof(AgvOrderCommunicator.enREQ)))
                {
                    if (v.ToString() == req_string)
                    {
                        MethodInvoker process = (MethodInvoker)delegate ()
                        {
                            switch (v)
                            {
                                case AgvOrderCommunicator.enREQ.ASA:
                                    {
                                        AgvOrderCommunicator.RequestDelivery req = new AgvOrderCommunicator.RequestDelivery();
                                        req.SetBytes(data);

                                        Log("R[" + req.ToString() + "]");

                                        if (req.result == AgvOrderCommunicator.enRESULT.OK.ToString())
                                        {
                                            if (0 < listBoxStation.SelectedItems.Count)
                                            {
                                                ListStation ls = listBoxStation.SelectedItems[0] as ListStation;

                                                AgvControlManager.FloorQR stQr = ls.qr as AgvControlManager.FloorQR;
                                                if (stQr != null && stQr.station_id == req.station)
                                                {
                                                    buttonST.Enabled = true;

                                                    if (buttonAutoStComplete.Checked)
                                                    {
                                                        int waitsec = 0;
                                                        if (int.TryParse(textBoxSTWait.Text, out waitsec))
                                                        {
                                                            BL_Win32API.Sleep(waitsec * 1000);
                                                        }

                                                        buttonST_Click(buttonST, null);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    break;

                                case AgvOrderCommunicator.enREQ.ARR:
                                    {
                                        AgvOrderCommunicator.RequestSTComplete req = new AgvOrderCommunicator.RequestSTComplete();
                                        req.SetBytes(data);

                                        Log("R[" + req.ToString() + "]");

                                        if (req.result == AgvOrderCommunicator.enRESULT.OK.ToString())
                                        {
                                            ListRack lr = listBoxRack.SelectedItems[0] as ListRack;

                                            AgvControlManager.Rack rack = lr.rack as AgvControlManager.Rack;
                                            if (rack != null && rack.rack_no == req.rack)
                                            {
                                                if (buttonAutoChange.Checked)
                                                {
                                                    if (listBoxStation.SelectedIndices.Count == 0) listBoxStation.SelectedIndices.Add(0);
                                                    else
                                                    {
                                                        int index = 0;
                                                        for (int i = 0; i < listBoxStation.Items.Count; i++)
                                                        {
                                                            index = (listBoxStation.SelectedIndices[0] + 1) % listBoxStation.Items.Count;
                                                            listBoxStation.SelectedIndices.Clear();
                                                            listBoxStation.SelectedIndices.Add(index);

                                                            if (listBoxStation.CheckedIndices.Contains(listBoxStation.SelectedIndices[0])) break;
                                                        }

                                                        ////if (index == 0)
                                                        ////{
                                                        ////    if (listviewRackFace.SelectedIndices.Count == 0) listviewRackFace.SelectedIndices.Add(0);
                                                        ////    else
                                                        ////    {
                                                        ////        index = (listviewRackFace.SelectedIndices[0] + 1) % listviewRackFace.Items.Count;
                                                        ////        listviewRackFace.SelectedIndices.Clear();
                                                        ////        listviewRackFace.SelectedIndices.Add(index);

                                                        //if (index == 0)
                                                        {
                                                            if (listBoxRack.SelectedIndices.Count == 0)
                                                            {
                                                                if (0 < listBoxRack.CheckedItems.Count)
                                                                {
                                                                    listBoxRack.SelectedIndices.Add(listBoxRack.CheckedIndices[0]);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                for (int i = 0; i < listBoxRack.Items.Count; i++)
                                                                {
                                                                    index = (listBoxRack.SelectedIndices[0] + 1) % listBoxRack.Items.Count;
                                                                    listBoxRack.SelectedIndices.Clear();
                                                                    listBoxRack.SelectedIndices.Add(index);

                                                                    if (listBoxRack.CheckedIndices.Contains(listBoxRack.SelectedIndices[0])) break;
                                                                }
                                                            }
                                                        }
                                                        //    }
                                                        //}
                                                    }
                                                }

                                                if (buttonAutoRack.Checked)
                                                {
                                                    BL_Win32API.Sleep(1000);

                                                    buttonRack_Click(buttonRack, null);
                                                }
                                            }
                                        }
                                    }
                                    break;
                            }
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
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            if (client != null)
            {
                client.EventReceived -= Client_EventReceived;
                client.EndLink();
            }
            Close();
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            controller = new AgvControlManager();

            string data_dir = Path.GetDirectoryName(ini_hokusho.FullName);
            string[] files = Directory.GetFiles(data_dir, "*.lag");
            if (0 < files.Length)
            {
                controller.Load(files[0]);
            }

            ListupStation();
            ListupRack();

            AttachButton_to_Functions(buttonConnect, 1);

            buttonConnect.Checked = true;
            SubForm_Base_Function01_Clicked(buttonConnect);
        }

        protected override void SubForm_Base_Function01_Clicked(object sender)
        {
            base.SubForm_Base_Function01_Clicked(sender);

            IPAddress ip;
            if (!IPAddress.TryParse(textBoxIP.Text, out ip)) return;
            int port;
            if (!int.TryParse(textBoxPort.Text, out port)) return;

            buttonConnect.Text = "接続...";

            if (client == null)
            {
                client = new BL_TcpP2PClient(IPAddress.Any, 0, ip, port, 10, 10, BL_TcpP2P.BL_FormatType.STX_ETX);
                client.EventReceived += Client_EventReceived;
                client.EventConnected += Client_EventConnected;
                client.EventClosed += Client_EventClosed;
                client.StartLink();
            }
            else
            {
                client.EndLink();
                client.Dispose();
                client = null;
            }
        }

        private void Client_EventClosed(BL_TcpP2P sender)
        {
            MethodInvoker process = (MethodInvoker)delegate ()
            {
                buttonConnect.Text = "未接続";
            };

            try
            {
                if (InvokeRequired) Invoke(process);
                else process.Invoke();
            }
            catch (Exception) { }
        }

        private void Client_EventConnected(BL_TcpP2P sender)
        {
            MethodInvoker process = (MethodInvoker)delegate ()
            {
                buttonConnect.Text = "接続済";
            };

            try
            {
                if (InvokeRequired) Invoke(process);
                else process.Invoke();
            }
            catch (Exception) { }
        }

        protected void ListupStation()
        {
            listBoxStation.Items.Clear();

            //var floor = map.SelectFloor();

            foreach (var kv in controller.map)
            {
                var floor = kv.Value;

                if (floor != null)
                {
                    foreach (var kvx in floor.mapeditor.qrs)
                    {
                        foreach (var kvy in kvx.Value)
                        {
                            var qr = kvy.Value;

                            if (qr.direction_station != AgvControlManager.FloorQR.enDirection.NONE)
                            {
                                ListStation st = new ListStation();
                                st.qr = qr;
                                listBoxStation.Items.Add(st);
                            }
                        }
                    }
                }
            }

            //listBoxStation.Items.Sort((a, b) => string.Compare(a[1].ToString(), b[1].ToString()));
            //listBoxStation.RefreshMe(false);
        }

        protected void ListupRack()
        {
            listBoxRack.Items.Clear();
            //listBoxRack.NeedRedraw = true;

            SortedDictionary<string, ListRack> list = new SortedDictionary<string, ListRack>();
            List<AgvControlManager.Rack> racks = new List<AgvControlManager.Rack>();

            var r = controller.AllQR.Where(e => e.rack != null);
            foreach (var v in r) racks.Add(v.rack);
            var agv = controller.agvs.Where(e => e.rack != null);
            foreach (var v in agv) racks.Add(v.rack);

            foreach (var rack in racks)
            {
                ListRack ra = new ListRack();
                ra.rack = rack;
                list[rack.rack_no.PadLeft(10)] = ra;
            }

            if (0 < list.Count)
            {
                foreach (var kv in list)
                {
                    listBoxRack.Items.Add(kv.Value);
                }
            }

            //listBoxRack.Items.Sort((a, b) => string.Compare(a[1].ToString(), b[1].ToString()));
            //listBoxRack.RefreshMe(false);
        }

        private void buttonRack_Click(object sender, EventArgs e)
        {
            base.SubForm_Base_Function04_Clicked(sender);

            if (client == null) return;

            if (listBoxStation.SelectedIndices.Count == 0 || listBoxRack.SelectedIndices.Count == 0 || listBoxRackFace.SelectedIndices.Count == 0) return;

            foreach (int rackindex in listBoxRack.SelectedIndices)
            {
                var rackitem = listBoxRack.Items[rackindex] as ListRack;

                AgvControlManager.Rack rack = rackitem.rack as AgvControlManager.Rack;

                foreach (int stindex in listBoxStation.SelectedIndices)
                {
                    var stitem = listBoxStation.Items[stindex] as ListStation;

                    AgvControlManager.FloorQR stQr = stitem.qr as AgvControlManager.FloorQR;

                    string faceID = listBoxRackFace.SelectedItems[0].ToString();

                    AgvOrderCommunicator.RequestBase req = new AgvOrderCommunicator.RequestDelivery(
                        AgvOrderCommunicator.enREQ.QRS,
                        ++seqno, AgvOrderCommunicator.enRESULT.RQ, stQr.station_id, rack.rack_no, faceID);
                    client.Send(req.GetBytes());

                    Log("S[" + req.ToString() + "]");
                }
            }
        }

        private void buttonST_Click(object sender, EventArgs e)
        {
            base.SubForm_Base_Function01_Clicked(sender);

            if (client == null) return;

            if (listBoxStation.SelectedItems.Count == 0) return;

            ListStation ls = listBoxStation.SelectedItems[0] as ListStation;
            AgvControlManager.FloorQR stQr = ls.qr;

            AgvOrderCommunicator.RequestBase req = new AgvOrderCommunicator.RequestSTComplete(
                AgvOrderCommunicator.enREQ.QSC,
                ++seqno, AgvOrderCommunicator.enRESULT.RQ, stQr.station_id, "", "");
            client.Send(req.GetBytes());

            Log("S[" + req.ToString() + "]");

            buttonST.Enabled = false;
        }

        private void listBoxRack_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBoxRack.SelectedItems.Count == 0) return;

            ListRack lr = listBoxRack.SelectedItems[0] as ListRack;

            listBoxRackFace.Items.Clear();
            foreach (var face in lr.rack.face_id)
            {
                listBoxRackFace.Items.Add(face.Value);
            }

            //listBoxRackFace.Items.Sort((a, b) => string.Compare(a[1].ToString(), b[1].ToString()));
            //listBoxRackFace.RefreshMe();

            if (0 < listBoxRackFace.Items.Count) listBoxRackFace.SelectedIndex = 0;
        }
    }
}
