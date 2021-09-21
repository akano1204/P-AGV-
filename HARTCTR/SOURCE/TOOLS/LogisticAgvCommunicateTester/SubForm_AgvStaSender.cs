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
using BelicsClass.ObjectSync;

namespace LogisticAgvCommunicateTester
{
    public partial class SubForm_AgvStaSender : BelicsClass.UI.BL_SubForm_Base
    {
        public BelicsClass.File.BL_IniFile ini = new BelicsClass.File.BL_IniFile();


        //int pitch = 100;

        string ip = "127.0.0.1";
        int remote_host = 9300;
        int local_host = 9000;
        int local_client = 9100;

        AgvController.AgvCommunicator.State state = new AgvController.AgvCommunicator.State();
        IPEndPoint remotehostEP = null;
        BL_RawSocketUDP recv = null;
        BL_RawSocketUDP send = null;

        //AgvControlManager controller = new AgvControlManager();
        //BelicsClass.File.BL_IniFile ini_hokusho = new BelicsClass.File.BL_IniFile(Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), @"..\DATA\HOKUSHO.INI"));


        public class AgvMemory : BL_FaceMemorySync
        {
            [BL_ObjectSync]
            public string map = " ";
            [BL_ObjectSync]
            public Int32 x = 0;
            [BL_ObjectSync]
            public Int32 y = 0;

            public AgvMemory(string name)
            {
                Initialize(name);
            }
        }
        AgvMemory mem = null;

        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "[F1]:接続", "", "", "[F4]:クリア", "[F5]:自動進行", "", "[F7]:S送信", "", "", "", "", "[F12]:戻る" };
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

        public SubForm_AgvStaSender()
        {
            InitializeComponent();
            Resizer_Initialize();


            //string data_dir = Path.GetDirectoryName(ini_hokusho.FullName);
            //string[] files = Directory.GetFiles(data_dir, "*.lag");
            //if (0 < files.Length)
            //{
            //    controller.Load(files[0]);
            //}
        }

        public double Degree(PointF p1, PointF p2)
        {
            double r = Math.Atan2(p1.Y - p2.Y, p1.X - p2.X);
            if (r < 0) r = r + 2 * Math.PI;
            double degree = Math.Floor(r * 360 / (2 * Math.PI));

            degree = (degree + 90) % 360;

            return degree;
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            AttachButton_to_Functions(buttonConnect, 1);
            AttachButton_to_Functions(buttonClear, 4);
            AttachButton_to_Functions(buttonAutoRun, 5);
            AttachButton_to_Functions(buttonSendShot, 7);

            listBoxStateCMD.Items.Clear();
            foreach (var v in Enum.GetValues(typeof(AgvController.AgvCommunicator.State.CMD)))
            {
                listBoxStateCMD.Items.Add(v);
            }

            listBoxStateCMD.SelectedIndex = 1;


            listBoxSTA.Items.Clear();
            foreach (var v in Enum.GetValues(typeof(AgvController.AgvCommunicator.State.STA)))
            {
                listBoxSTA.Items.Add(v);
            }

            listBoxOrderCMD.Items.Clear();
            foreach (var v in Enum.GetValues(typeof(AgvController.AgvCommunicator.Order.CMD)))
            {
                listBoxOrderCMD.Items.Add(v);
            }

            listBoxMOD.Items.Clear();
            foreach (var v in Enum.GetValues(typeof(AgvController.AgvCommunicator.Order.MOD)))
            {
                listBoxMOD.Items.Add(v);
            }


            for (int i = 0; i < listBoxStateCMD.Items.Count; i++)
            {
                AgvController.AgvCommunicator.State.CMD cmd = (AgvController.AgvCommunicator.State.CMD)listBoxStateCMD.Items[i];
                if (cmd == AgvController.AgvCommunicator.State.CMD.REQUEST)
                {
                    listBoxStateCMD.SetSelected(i, true);
                }
            }

            for (int i = 0; i < listBoxSTA.Items.Count; i++)
            {
                AgvController.AgvCommunicator.State.STA sta = (AgvController.AgvCommunicator.State.STA)listBoxSTA.Items[i];
                if (sta == AgvController.AgvCommunicator.State.STA.STOP)
                {
                    listBoxSTA.SetSelected(i, true);
                }
                else if (sta == AgvController.AgvCommunicator.State.STA.RUN_MODE)
                {
                    listBoxSTA.SetSelected(i, true);
                }
                else if (sta == AgvController.AgvCommunicator.State.STA.TURNTABLE_LOWER)
                {
                    listBoxSTA.SetSelected(i, true);
                }
            }

            textBoxInterval.Text = timerAutoRun.Interval.ToString();

            if (Program.connect_and_autorun)
            {
                textBoxLocalHost.Text = Program.port_no.ToString();
                textBoxLocalClient.Text = (9100 + Program.port_no % 100).ToString();
                buttonAutoRun.Checked = true;

                buttonWindowRestore_Click(buttonWindowRestore, null);
                SubForm_Base_Function01_Clicked(buttonAutoRun);
                SubForm_Base_Function05_Clicked(buttonAutoRun);
            }
        }

        protected override void SubForm_Base_FormClosing(object sender, FormClosingEventArgs e)
        {
            base.SubForm_Base_FormClosing(sender, e);

            ini.Write(textBoxLocalHost.Text, "WIN_LEFT", m_Mainform.Left);
            ini.Write(textBoxLocalHost.Text, "WIN_TOP", m_Mainform.Top);
            ini.Write(textBoxLocalHost.Text, "WIN_WIDTH", m_Mainform.Width);
            ini.Write(textBoxLocalHost.Text, "WIN_HEIGHT", m_Mainform.Height);

            if (mem != null) mem.Dispose();

            if (recv != null) recv.ReceiveEvent -= Recv_ReceiveEvent;
            if (recv != null) recv.Close();
            if (send != null) send.Close();
            recv = null;
            send = null;
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            if (recv != null) recv.ReceiveEvent -= Recv_ReceiveEvent;
            if (recv != null) recv.Close();
            if (send != null) send.Close();
            recv = null;
            send = null;

            Close();
        }

        protected override void SubForm_Base_Function01_Clicked(object sender)
        {
            base.SubForm_Base_Function01_Clicked(sender);

            if (recv == null)
            {
                if (!int.TryParse(textBoxLocalHost.Text, out local_host)) return;
                if (!int.TryParse(textBoxLocalClient.Text, out local_client)) return;
                if (!int.TryParse(textBoxRemoteHost.Text, out remote_host)) return;
                IPAddress ipa = IPAddress.Any;
                if (!IPAddress.TryParse(textBoxRemoteIP.Text, out ipa)) return;
                ip = textBoxRemoteIP.Text;

                remotehostEP = new IPEndPoint(IPAddress.Parse(ip), remote_host);

                recv = new BL_RawSocketUDP(local_host);
                send = new BL_RawSocketUDP(local_client);

                recv.Open(BL_RawSocketUDP.FormatType.BL_IDSIZE);
                send.Open(BL_RawSocketUDP.FormatType.BL_IDSIZE);

                recv.ReceiveEvent += Recv_ReceiveEvent;

                buttonConnect.Text = "[F1] 切断";
                m_Mainform.btnFunctions[1].Text = "[F1]\n切断";

                textBoxMAP.Text = ini.Read(textBoxLocalHost.Text, "MAP", textBoxMAP.Text);
                textBoxXPOS.Text = ini.Read(textBoxLocalHost.Text, "XPOS", textBoxXPOS.Text);
                textBoxYPOS.Text = ini.Read(textBoxLocalHost.Text, "YPOS", textBoxYPOS.Text);

                for (int i = 0; i < listBoxSTA.Items.Count; i++)
                {
                    AgvController.AgvCommunicator.State.STA s = (AgvController.AgvCommunicator.State.STA)listBoxSTA.Items[i];
                    if (s == AgvController.AgvCommunicator.State.STA.CHARGE)
                    {
                        listBoxSTA.SetSelected(i, false);
                    }
                    else if (s == AgvController.AgvCommunicator.State.STA.RACK)
                    {
                        listBoxSTA.SetSelected(i, false);
                    }
                    else if (s == AgvController.AgvCommunicator.State.STA.RUN_MODE)
                    {
                        listBoxSTA.SetSelected(i, true);
                    }
                    else if (s == AgvController.AgvCommunicator.State.STA.STOP)
                    {
                        listBoxSTA.SetSelected(i, true);
                    }
                    else if (s == AgvController.AgvCommunicator.State.STA.TURNTABLE_UPPER)
                    {
                        listBoxSTA.SetSelected(i, false);
                    }
                    else if (s == AgvController.AgvCommunicator.State.STA.TURNTABLE_LOWER)
                    {
                        listBoxSTA.SetSelected(i, true);
                    }
                }

                mem = new AgvMemory("127.0.0.1_" + local_host.ToString());
                mem.ReadMemory();


                if (buttonAutoRun.Checked)
                {
                    SubForm_Base_Function07_Clicked(sender);
                }
            }
            else
            {
                mem.Dispose();
                mem = null;

                recv.Close();
                send.Close();
                recv = null;
                send = null;

                buttonConnect.Text = "[F1] 接続";
                m_Mainform.btnFunctions[1].Text = "[F1]\n接続";
            }
        }

        private void Recv_ReceiveEvent(object sender, BL_RawSocketUDP.ReceiveEventArgs e)
        {
            MethodInvoker process = (MethodInvoker)delegate ()
            {
                Recv_ReceiveEvent_func(sender, e);
            };

            try
            {
                if (InvokeRequired) Invoke(process);
                else process.Invoke();
            }
            catch (Exception) { }
        }

        bool charge_stop = false;
        bool route_cancel = false;
        int route_cancel_count = 0;

        BelicsClass.Common.BL_Stopwatch timeout = new BelicsClass.Common.BL_Stopwatch();

        private void Recv_ReceiveEvent_func(object sender, BL_RawSocketUDP.ReceiveEventArgs e)
        {
            byte[] d = e.BytesData;
            AgvController.AgvCommunicator.Order order = new AgvController.AgvCommunicator.Order();

            

            bool order_flag = false;

            for (int pos = 0; pos < d.Length; pos += order.Length)
            {
                timeout.Restart();

                order = new AgvController.AgvCommunicator.Order();

                byte[] data = new byte[order.Length];
                Array.Copy(d, pos, data, 0, order.Length);
                order.SetBytes(data);

                if (order.cmd == (ushort)AgvController.AgvCommunicator.Order.CMD.RESPONSE)
                {
                }

                #region 未使用
                //else if (order.cmd == (ushort)AgvController.AgvCommunicator.Order.CMD.STOP)
                //{
                //    buttonSTOP.Checked = true;

                //    for (int i = 0; i < listBoxSTA.Items.Count; i++)
                //    {
                //        AgvController.AgvCommunicator.State.STA sta = (AgvController.AgvCommunicator.State.STA)listBoxSTA.Items[i];
                //        if (sta == AgvController.AgvCommunicator.State.STA.STOP)
                //        {
                //            listBoxSTA.SetSelected(i, true);
                //        }
                //    }

                //    {
                //        AgvController.AgvCommunicator.State sta = new AgvController.AgvCommunicator.State();
                //        sta.cmd = (ushort)AgvController.AgvCommunicator.State.CMD.RES_STOP;
                //        sta.sta = 0;
                //        foreach (var v in listBoxSTA.SelectedItems)
                //        {
                //            sta.sta |= (UInt16)v;
                //        }

                //        sta.map = textBoxMAP.Text.PadRight(1).Substring(0, 1);

                //        if (!byte.TryParse(textBoxBAT.Text, out sta.bat)) return;
                //        if (!Int16.TryParse(textBoxDEG.Text, out sta.deg)) return;
                //        if (!Int32.TryParse(textBoxXPOS.Text, out sta.x)) return;
                //        if (!Int32.TryParse(textBoxYPOS.Text, out sta.y)) return;
                //        if (!Int16.TryParse(textBoxRACK_DEG.Text, out sta.rack_deg)) return;
                //        if (!UInt16.TryParse(textBoxRACKNO.Text, out sta.rack_no)) return;

                //        byte[] sdata = sta.GetBytes();
                //        send.SendBytes(sdata, remotehostEP);
                //    }
                //}
                //else if (order.cmd == (ushort)AgvController.AgvCommunicator.Order.CMD.RESTART)
                //{
                //    buttonSTOP.Checked = false;

                //    for (int i = 0; i < listBoxSTA.Items.Count; i++)
                //    {
                //        AgvController.AgvCommunicator.State.STA sta = (AgvController.AgvCommunicator.State.STA)listBoxSTA.Items[i];
                //        if (sta == AgvController.AgvCommunicator.State.STA.STOP)
                //        {
                //            listBoxSTA.SetSelected(i, false);
                //        }
                //    }

                //    {
                //        AgvController.AgvCommunicator.State sta = new AgvController.AgvCommunicator.State();
                //        sta.cmd = (ushort)AgvController.AgvCommunicator.State.CMD.RES_RESTART;
                //        sta.sta = 0;
                //        foreach (var v in listBoxSTA.SelectedItems)
                //        {
                //            sta.sta |= (UInt16)v;
                //        }

                //        sta.map = textBoxMAP.Text.PadRight(1).Substring(0, 1);

                //        if (!byte.TryParse(textBoxBAT.Text, out sta.bat)) return;
                //        if (!Int16.TryParse(textBoxDEG.Text, out sta.deg)) return;
                //        if (!Int32.TryParse(textBoxXPOS.Text, out sta.x)) return;
                //        if (!Int32.TryParse(textBoxYPOS.Text, out sta.y)) return;
                //        if (!Int16.TryParse(textBoxRACK_DEG.Text, out sta.rack_deg)) return;
                //        if (!UInt16.TryParse(textBoxRACKNO.Text, out sta.rack_no)) return;

                //        byte[] sdata = sta.GetBytes();
                //        send.SendBytes(sdata, remotehostEP);
                //    }
                //}
                //else if (order.cmd == (ushort)AgvController.AgvCommunicator.Order.CMD.COMPLETE)
                //{
                //    AgvController.AgvCommunicator.State sta = new AgvController.AgvCommunicator.State();
                //    sta.cmd = (ushort)AgvController.AgvCommunicator.State.CMD.RES_COMPLETE;
                //    sta.sta = 0;
                //    foreach (var v in listBoxSTA.SelectedItems)
                //    {
                //        sta.sta |= (UInt16)v;
                //    }

                //    sta.map = textBoxMAP.Text.PadRight(1).Substring(0, 1);

                //    if (!byte.TryParse(textBoxBAT.Text, out sta.bat)) return;
                //    if (!Int16.TryParse(textBoxDEG.Text, out sta.deg)) return;
                //    if (!Int32.TryParse(textBoxXPOS.Text, out sta.x)) return;
                //    if (!Int32.TryParse(textBoxYPOS.Text, out sta.y)) return;
                //    if (!Int16.TryParse(textBoxRACK_DEG.Text, out sta.rack_deg)) return;
                //    if (!UInt16.TryParse(textBoxRACKNO.Text, out sta.rack_no)) return;

                //    byte[] sdata = sta.GetBytes();
                //    send.SendBytes(sdata, remotehostEP);

                //    listviewReceive.ItemsClear();
                //    listviewReceive.RefreshMe();

                //    for (int i = 0; i < listBoxStateCMD.Items.Count; i++)
                //    {
                //        AgvController.AgvCommunicator.State.CMD cmd = (AgvController.AgvCommunicator.State.CMD)listBoxStateCMD.Items[i];
                //        if (cmd == AgvController.AgvCommunicator.State.CMD.REQUEST)
                //        {
                //            listBoxStateCMD.SetSelected(i, true);
                //        }
                //    }

                //    SubForm_Base_Function07_Clicked(sender);
                //}
                #endregion

                else if (order.cmd == (ushort)AgvController.AgvCommunicator.Order.CMD.CHARGE_STOP)
                {
                    charge_stop = true;
                }
                else if (order.cmd == (ushort)AgvController.AgvCommunicator.Order.CMD.ROUTE_CANCEL)
                {
                    if (!route_cancel)
                    {
                        route_cancel_count = 0;
                        route_cancel = true;
                    }
                }
                else
                {
                    BL_VirtualListView.BL_VirtualListViewItem item = new BL_VirtualListView.BL_VirtualListViewItem();
                    item.Tag = order;
                    item.Add(order.cmd);
                    item.Add(order.mod.ToString("X2") + " HEX");
                    item.Add(order.deg);
                    item.Add(order.x);
                    item.Add(order.y);
                    item.Add((order.deg - order.rack_deg + 360) % 360);

                    listviewReceive.Items.Add(item);
                    listviewReceive.NeedRedraw = true;

                    if (order.dist != 0)
                    {
                        if (checkBoxDF.Checked)
                        {
                            if (order.deg == 0)
                            {
                                textBoxXPOS.Text = order.x.ToString();
                                textBoxYPOS.Text = (order.y - order.dist).ToString();
                            }
                            else if (order.deg == 180)
                            {
                                textBoxXPOS.Text = order.x.ToString();
                                textBoxYPOS.Text = (order.y + order.dist).ToString();
                            }
                            else if (order.deg == 90)
                            {
                                textBoxXPOS.Text = (order.x + order.dist).ToString();
                                textBoxYPOS.Text = order.y.ToString();
                            }
                            else if (order.deg == 270)
                            {
                                textBoxXPOS.Text = (order.x - order.dist).ToString();
                                textBoxYPOS.Text = order.y.ToString();
                            }
                        }
                    }

                    order_flag = true;
                }

                buttonSTOP.Checked = false;
            }

            if (listviewReceive.NeedRedraw) listviewReceive.RefreshMe();

            if (order_flag)
            {
                AgvController.AgvCommunicator.State sta = new AgvController.AgvCommunicator.State();
                sta.cmd = (ushort)AgvController.AgvCommunicator.State.CMD.RES_ORDER;
                sta.sta = 0;
                foreach (var v in listBoxSTA.SelectedItems)
                {
                    sta.sta |= (UInt16)v;
                }

                sta.map = textBoxMAP.Text.PadRight(1).Substring(0, 1);

                if (!byte.TryParse(textBoxBAT.Text, out sta.bat)) return;
                if (!Int16.TryParse(textBoxDEG.Text, out sta.deg)) return;
                if (!Int32.TryParse(textBoxXPOS.Text, out sta.x)) return;
                if (!Int32.TryParse(textBoxYPOS.Text, out sta.y)) return;
                if (!Int16.TryParse(textBoxRACK_DEG.Text, out sta.rack_deg)) return;
                if (!UInt16.TryParse(textBoxRACKNO.Text, out sta.rack_no)) return;

                byte[] sdata = sta.GetBytes();
                send.SendBytes(sdata, remotehostEP);

                for (int i = 0; i < listBoxStateCMD.Items.Count; i++)
                {
                    AgvController.AgvCommunicator.State.CMD cmd = (AgvController.AgvCommunicator.State.CMD)listBoxStateCMD.Items[i];
                    if (cmd == AgvController.AgvCommunicator.State.CMD.STATE)
                    {
                        listBoxStateCMD.SetSelected(i, true);
                    }
                }
            }
        }

        void ChargeStop()
        {
            AgvController.AgvCommunicator.State sta = new AgvController.AgvCommunicator.State();
            sta.cmd = (ushort)AgvController.AgvCommunicator.State.CMD.RES_CHARGE_STOP;

            for (int i = 0; i < listBoxSTA.Items.Count; i++)
            {
                AgvController.AgvCommunicator.State.STA s = (AgvController.AgvCommunicator.State.STA)listBoxSTA.Items[i];
                if (s == AgvController.AgvCommunicator.State.STA.CHARGE)
                {
                    listBoxSTA.SetSelected(i, false);
                }
            }

            if (!buttonAutoRun.Checked)
            {
                sta.sta = 0;
                foreach (var v in listBoxSTA.SelectedItems)
                {
                    sta.sta |= (UInt16)v;
                }

                sta.map = textBoxMAP.Text.PadRight(1).Substring(0, 1);

                if (!byte.TryParse(textBoxBAT.Text, out sta.bat)) return;
                if (!Int16.TryParse(textBoxDEG.Text, out sta.deg)) return;
                if (!Int32.TryParse(textBoxXPOS.Text, out sta.x)) return;
                if (!Int32.TryParse(textBoxYPOS.Text, out sta.y)) return;
                if (!Int16.TryParse(textBoxRACK_DEG.Text, out sta.rack_deg)) return;
                if (!UInt16.TryParse(textBoxRACKNO.Text, out sta.rack_no)) return;

                byte[] sdata = sta.GetBytes();
                send.SendBytes(sdata, remotehostEP);
            }
        }

        void RouteCancel()
        {
            //if (!buttonAutoRun.Checked)
            {
                AgvController.AgvCommunicator.State sta = new AgvController.AgvCommunicator.State();
                sta.cmd = (ushort)AgvController.AgvCommunicator.State.CMD.REQUEST;
                sta.sta = 0;
                foreach (var v in listBoxSTA.SelectedItems)
                {
                    sta.sta |= (UInt16)v;
                }

                sta.map = textBoxMAP.Text.PadRight(1).Substring(0, 1);

                if (!byte.TryParse(textBoxBAT.Text, out sta.bat)) return;
                if (!Int16.TryParse(textBoxDEG.Text, out sta.deg)) return;
                if (!Int32.TryParse(textBoxXPOS.Text, out sta.x)) return;
                if (!Int32.TryParse(textBoxYPOS.Text, out sta.y)) return;
                if (!Int16.TryParse(textBoxRACK_DEG.Text, out sta.rack_deg)) return;
                if (!UInt16.TryParse(textBoxRACKNO.Text, out sta.rack_no)) return;

                byte[] sdata = sta.GetBytes();
                send.SendBytes(sdata, remotehostEP);

                for (int i = 0; i < listBoxStateCMD.Items.Count; i++)
                {
                    AgvController.AgvCommunicator.State.CMD cmd = (AgvController.AgvCommunicator.State.CMD)listBoxStateCMD.Items[i];
                    if (cmd == AgvController.AgvCommunicator.State.CMD.REQUEST)
                    {
                        listBoxStateCMD.SetSelected(i, true);
                    }
                }
            }
            
            listviewReceive.ItemsClear();
            listviewReceive.RefreshMe();
        }

        protected override void SubForm_Base_Function04_Clicked(object sender)
        {
            base.SubForm_Base_Function04_Clicked(sender);

            listviewReceive.ItemsClear();
            listviewReceive.RefreshMe();

            for (int i = 0; i < listBoxStateCMD.Items.Count; i++)
            {
                AgvController.AgvCommunicator.State.CMD cmd = (AgvController.AgvCommunicator.State.CMD)listBoxStateCMD.Items[i];
                if (cmd == AgvController.AgvCommunicator.State.CMD.REQUEST)
                {
                    listBoxStateCMD.SetSelected(i, true);
                }
                
                if (cmd == AgvController.AgvCommunicator.State.CMD.STATE)
                {
                    listBoxStateCMD.SetSelected(i, false);
                }
            }
        }

        protected override void SubForm_Base_Function05_Clicked(object sender)
        {
            base.SubForm_Base_Function05_Clicked(sender);
            timerAutoRun.Enabled = buttonAutoRun.Checked;
        }

        protected override void SubForm_Base_Function07_Clicked(object sender)
        {
            base.SubForm_Base_Function07_Clicked(sender);

            if (send == null) return;

            if (listBoxStateCMD.SelectedItem == null) return;

            state.cmd = (UInt16)listBoxStateCMD.SelectedItem;
            state.sta = 0;
            foreach (var v in listBoxSTA.SelectedItems)
            {
                state.sta |= (UInt16)v;
            }

            state.map = textBoxMAP.Text.PadRight(1).Substring(0, 1);

            if (!byte.TryParse(textBoxBAT.Text, out state.bat)) return;
            if (!Int16.TryParse(textBoxDEG.Text, out state.deg)) return;
            if (!Int32.TryParse(textBoxXPOS.Text, out state.x)) return;
            if (!Int32.TryParse(textBoxYPOS.Text, out state.y)) return;

            //short rd = 0;
            //if (!Int16.TryParse(textBoxRACK_DEG.Text, out rd)) return;
            //state.rack_deg = (short)((state.deg - rd + 360) % 360);
            
            if (!Int16.TryParse(textBoxRACK_DEG.Text, out state.rack_deg)) return;


            if (!UInt16.TryParse(textBoxRACKNO.Text, out state.rack_no)) return;
            state.racktype = textBoxRackCode.Text;
            byte error_code = 0; if (byte.TryParse(textBoxERRCODE.Text, out error_code)) state.error_code = error_code;

            byte[] sdata = state.GetBytes();
            send.SendBytes(sdata, remotehostEP);

            buttonSTOP.Checked = true;
        }

        private void listviewReceive_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listviewReceive.SelectedItems.Count == 0) return;
            AgvController.AgvCommunicator.Order order = listviewReceive.SelectedItems[0].Tag as AgvController.AgvCommunicator.Order;
            if (order == null) return;

            for (int i = 0; i < listBoxOrderCMD.Items.Count; i++)
            {
                AgvController.AgvCommunicator.Order.CMD cmd = (AgvController.AgvCommunicator.Order.CMD)listBoxOrderCMD.Items[i];

                if (order.cmd == (ushort)cmd)
                {
                    listBoxOrderCMD.SetSelected(i, true);
                }
            }

            for (int i = 0; i < listBoxMOD.Items.Count; i++)
            {
                AgvController.AgvCommunicator.Order.MOD mod = (AgvController.AgvCommunicator.Order.MOD)listBoxMOD.Items[i];

                if ((order.mod & (ushort)mod) != 0)
                {
                    listBoxMOD.SetSelected(i, true);
                }
                else
                {
                    listBoxMOD.SetSelected(i, false);
                }
            }

            int xnow = 0;int.TryParse(textBoxXPOS.Text, out xnow);
            int ynow = 0;int.TryParse(textBoxYPOS.Text, out ynow);

            textBoxXPOS.Text = order.x.ToString();
            textBoxYPOS.Text = order.y.ToString();

            //if (order.mod_agv_rorate) 
            if (order.deg == 999)
            {
                short.TryParse(textBoxDEG.Text, out order.deg);
            }
            else
            {
                textBoxDEG.Text = order.deg.ToString();
            }

            //int rackdegnow = 0;int.TryParse(textBoxRACK_DEG.Text, out rackdegnow);

            //if (order.mod_rack_rorate || order.deg % 90 != 0 /*|| rackdegnow % 90 != 0*/)
            {
                //textBoxRACK_DEG.Text = order.rack_deg.ToString();
                if (order.mod_rack_rorate)
                {
                    textBoxRACK_DEG.Text = ((order.deg - order.rack_deg + 360) % 360).ToString();
                }
            }

            //if (order.mod_stop)
            //{
            //    buttonSTOP.Checked = true;

            //    for (int i = 0; i < listBoxSTA.Items.Count; i++)
            //    {
            //        AgvController.AgvCommunicator.State.STA sta = (AgvController.AgvCommunicator.State.STA)listBoxSTA.Items[i];

            //        if (sta == AgvController.AgvCommunicator.State.STA.STOP)
            //        {
            //            listBoxSTA.SetSelected(i, true);
            //        }
            //    }
            //}
            //else
            //{
            //    for (int i = 0; i < listBoxSTA.Items.Count; i++)
            //    {
            //        AgvController.AgvCommunicator.State.STA sta = (AgvController.AgvCommunicator.State.STA)listBoxSTA.Items[i];

            //        if (sta == AgvController.AgvCommunicator.State.STA.STOP)
            //        {
            //            listBoxSTA.SetSelected(i, false);
            //        }
            //    }
            //}

            if (order.mod_rack_up)
            {
                if (buttonAutoRun.Checked)
                {
                    timerAutoRun.Enabled = false;
                    System.Threading.Thread.Sleep(timerAutoRun.Interval * 5);
                    timerAutoRun.Enabled = true;
                }

                for (int i = 0; i < listBoxSTA.Items.Count; i++)
                {
                    AgvController.AgvCommunicator.State.STA sta = (AgvController.AgvCommunicator.State.STA)listBoxSTA.Items[i];
                    if (sta == AgvController.AgvCommunicator.State.STA.RACK || sta == AgvController.AgvCommunicator.State.STA.TURNTABLE_UPPER)
                    {
                        listBoxSTA.SetSelected(i, true);
                    }

                    if (sta == AgvController.AgvCommunicator.State.STA.TURNTABLE_LOWER)
                    {
                        listBoxSTA.SetSelected(i, false);
                    }
                }
            }

            if (order.mod_rack_down)
            {
                if (buttonAutoRun.Checked)
                {
                    timerAutoRun.Enabled = false;
                    System.Threading.Thread.Sleep(timerAutoRun.Interval * 5);
                    timerAutoRun.Enabled = true;
                }

                for (int i = 0; i < listBoxSTA.Items.Count; i++)
                {
                    AgvController.AgvCommunicator.State.STA sta = (AgvController.AgvCommunicator.State.STA)listBoxSTA.Items[i];
                    if (sta == AgvController.AgvCommunicator.State.STA.RACK || sta == AgvController.AgvCommunicator.State.STA.TURNTABLE_UPPER)
                    {
                        listBoxSTA.SetSelected(i, false);
                    }

                    if (sta == AgvController.AgvCommunicator.State.STA.TURNTABLE_LOWER)
                    {
                        listBoxSTA.SetSelected(i, true);
                    }
                }
            }

            if (order.mod_charge)
            {
                for (int i = 0; i < listBoxSTA.Items.Count; i++)
                {
                    AgvController.AgvCommunicator.State.STA sta = (AgvController.AgvCommunicator.State.STA)listBoxSTA.Items[i];
                    if (sta == AgvController.AgvCommunicator.State.STA.CHARGE)
                    {
                        listBoxSTA.SetSelected(i, true);
                    }
                }
            }
        }

        private void timerAutoRun_Tick(object sender, EventArgs e)
        {
            int pitch = 0; int.TryParse(textBoxPitch.Text, out pitch);

            if (!buttonAutoRun.Checked)
            {
                if (charge_stop)
                {
                    ChargeStop();
                    charge_stop = false;
                }

                if (route_cancel)
                {
                    RouteCancel();
                    route_cancel = false;
                }

                return;
            }

            if (buttonSTOP.Checked && timeout.IsRunning && 2000 < timeout.ElapsedMilliseconds)
            {
                buttonSTOP.Checked = false;
            }

            if (buttonSTOP.Checked)
            {
                return;
            }
            else if (0 < listviewReceive.Items.Count)
            {
                bool do_next = false;

                int x = 0; int.TryParse(textBoxXPOS.Text, out x);
                int y = 0; int.TryParse(textBoxYPOS.Text, out y);

                if (listviewReceive.SelectedIndices.Count == 0)
                {
                    listviewReceive.SelectedIndices.Add(0);
                    do_next = true;
                }
                else
                {
                    listviewReceive.SelectedIndices.Clear();
                    listviewReceive.SelectedIndices.Add(0);
                    do_next = true;
                }

                if (!do_next) return;

                int xn = 0; int.TryParse(textBoxXPOS.Text, out xn);
                int yn = 0; int.TryParse(textBoxYPOS.Text, out yn);
                int deg = 0; int.TryParse(textBoxDEG.Text, out deg);
                int deg_pre = deg;
                if (x < xn)
                {
                    x += pitch;
                    if (xn < x) x = xn;

                    if (deg != 270 && buttonAutoRun.Checked)
                    {
                        timerAutoRun.Enabled = false;
                        System.Threading.Thread.Sleep(timerAutoRun.Interval * 2);
                        timerAutoRun.Enabled = true;

                        route_cancel_count = 999;
                    }
                    deg = 270;
                }
                if (y < yn)
                {
                    y += pitch;
                    if (yn < y) y = yn;

                    if (deg != 0 && buttonAutoRun.Checked)
                    {
                        timerAutoRun.Enabled = false;
                        System.Threading.Thread.Sleep(timerAutoRun.Interval * 2);
                        timerAutoRun.Enabled = true;

                        route_cancel_count = 999;
                    }
                    deg = 0;
                }
                if (xn < x)
                {
                    x -= pitch;
                    if (x < xn) x = xn;

                    if (deg != 90 && buttonAutoRun.Checked)
                    {
                        timerAutoRun.Enabled = false;
                        System.Threading.Thread.Sleep(timerAutoRun.Interval * 2);
                        timerAutoRun.Enabled = true;

                        route_cancel_count = 999;
                    }
                    deg = 90;
                }
                if (yn < y)
                {
                    y -= pitch;
                    if (y < yn) y = yn;

                    if (deg != 180 && buttonAutoRun.Checked)
                    {
                        timerAutoRun.Enabled = false;
                        System.Threading.Thread.Sleep(timerAutoRun.Interval * 2);
                        timerAutoRun.Enabled = true;

                        route_cancel_count = 999;
                    }
                    deg = 180;
                }

                textBoxXPOS.Text = x.ToString();
                textBoxYPOS.Text = y.ToString();

                if (deg_pre % 90 == 0) textBoxDEG.Text = deg.ToString();

                send_command();

                if (route_cancel)
                {
                    int slipcount = 1;
                    int.TryParse(textBoxRouteCancelSlip.Text, out slipcount);

                    route_cancel_count++;
                    if (slipcount <= route_cancel_count)
                    {
                        RouteCancel();
                        route_cancel = false;
                    }
                }

                if (charge_stop)
                {
                    ChargeStop();
                    charge_stop = false;
                }
            }
            else
            {
                for (int i = 0; i < listBoxSTA.Items.Count; i++)
                {
                    AgvController.AgvCommunicator.State.STA sta = (AgvController.AgvCommunicator.State.STA)listBoxSTA.Items[i];
                    if (sta == AgvController.AgvCommunicator.State.STA.STOP)
                    {
                        listBoxSTA.SetSelected(i, true);
                    }
                }

                for (int i = 0; i < listBoxStateCMD.Items.Count; i++)
                {
                    AgvController.AgvCommunicator.State.CMD cmd = (AgvController.AgvCommunicator.State.CMD)listBoxStateCMD.Items[i];
                    if (cmd == AgvController.AgvCommunicator.State.CMD.REQUEST)
                    {
                        listBoxStateCMD.SetSelected(i, true);
                    }
                }

                SubForm_Base_Function07_Clicked(sender);
            }
        }

        private void listviewReceive_DoubleClick(object sender, EventArgs e)
        {
            if (0 < listviewReceive.Items.Count)
            {
                SubForm_Base_Function07_Clicked(sender);

                listviewReceive.Items.RemoveAt(0);
                listviewReceive.RefreshMe();

                if (listviewReceive.Items.Count == 0)
                {
                    for (int i = 0; i < listBoxSTA.Items.Count; i++)
                    {
                        AgvController.AgvCommunicator.State.STA sta = (AgvController.AgvCommunicator.State.STA)listBoxSTA.Items[i];
                        if (sta == AgvController.AgvCommunicator.State.STA.STOP)
                        {
                            listBoxSTA.SetSelected(i, true);
                        }
                    }

                    for (int i = 0; i < listBoxStateCMD.Items.Count; i++)
                    {
                        AgvController.AgvCommunicator.State.CMD cmd = (AgvController.AgvCommunicator.State.CMD)listBoxStateCMD.Items[i];
                        if (cmd == AgvController.AgvCommunicator.State.CMD.REQUEST)
                        {
                            listBoxStateCMD.SetSelected(i, true);
                        }
                    }

                    SubForm_Base_Function07_Clicked(sender);
                }
            }
        }

        private void buttonAxisSave_Click(object sender, EventArgs e)
        {
            ini.Write(textBoxLocalHost.Text, "MAP", textBoxMAP.Text);
            ini.Write(textBoxLocalHost.Text, "XPOS", textBoxXPOS.Text);
            ini.Write(textBoxLocalHost.Text, "YPOS", textBoxYPOS.Text);
        }

        private void buttonWindowRestore_Click(object sender, EventArgs e)
        {
            m_Mainform.Left = ini.Read(textBoxLocalHost.Text, "WIN_LEFT", m_Mainform.Left);
            m_Mainform.Top = ini.Read(textBoxLocalHost.Text, "WIN_TOP", m_Mainform.Top);
            m_Mainform.Width = ini.Read(textBoxLocalHost.Text, "WIN_WIDTH", m_Mainform.Width);
            m_Mainform.Height = ini.Read(textBoxLocalHost.Text, "WIN_HEIGHT", m_Mainform.Height);
        }

        private void textBoxInterval_Leave(object sender, EventArgs e)
        {
            int interval;
            if (int.TryParse(textBoxInterval.Text, out interval))
            {
                timerAutoRun.Interval = interval;
            }
            else
            {
                textBoxInterval.Text = timerAutoRun.Interval.ToString();
            }
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            int pitch = 0;int.TryParse(textBoxPitch.Text, out pitch);
            int xn = 0; int.TryParse(textBoxXPOS.Text, out xn);
            int yn = 0; int.TryParse(textBoxYPOS.Text, out yn);
            int deg = 0;

            yn += pitch;
            textBoxYPOS.Text = yn.ToString();
            textBoxDEG.Text = deg.ToString();

            send_command();
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            int pitch = 0; int.TryParse(textBoxPitch.Text, out pitch);
            int xn = 0; int.TryParse(textBoxXPOS.Text, out xn);
            int yn = 0; int.TryParse(textBoxYPOS.Text, out yn);
            int deg = 180;

            yn -= pitch;
            textBoxYPOS.Text = yn.ToString();
            textBoxDEG.Text = deg.ToString();

            send_command();
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            int pitch = 0; int.TryParse(textBoxPitch.Text, out pitch);
            int xn = 0; int.TryParse(textBoxXPOS.Text, out xn);
            int yn = 0; int.TryParse(textBoxYPOS.Text, out yn);
            int deg = 90;

            xn -= pitch;
            textBoxXPOS.Text = xn.ToString();
            textBoxDEG.Text = deg.ToString();

            send_command();
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {
            int pitch = 0; int.TryParse(textBoxPitch.Text, out pitch);
            int xn = 0; int.TryParse(textBoxXPOS.Text, out xn);
            int yn = 0; int.TryParse(textBoxYPOS.Text, out yn);
            int deg = 270;

            xn += pitch;
            textBoxXPOS.Text = xn.ToString();
            textBoxDEG.Text = deg.ToString();

            send_command();
        }

        private void send_command()
        {
            int xn = 0; int.TryParse(textBoxXPOS.Text, out xn);
            int yn = 0; int.TryParse(textBoxYPOS.Text, out yn);

            SubForm_Base_Function07_Clicked(null);

            if (0 < listviewReceive.Items.Count)
            {
                AgvController.AgvCommunicator.Order order = listviewReceive.Items[0].Tag as AgvController.AgvCommunicator.Order;
                if (order != null)
                {
                    if (order.x == xn && order.y == yn)
                    {
                        listviewReceive.Items.RemoveAt(0);
                        listviewReceive.RefreshMe();
                    }
                }
            }

            if (0 == listviewReceive.Items.Count)
            {
                for (int i = 0; i < listBoxSTA.Items.Count; i++)
                {
                    AgvController.AgvCommunicator.State.STA sta = (AgvController.AgvCommunicator.State.STA)listBoxSTA.Items[i];
                    if (sta == AgvController.AgvCommunicator.State.STA.STOP)
                    {
                        listBoxSTA.SetSelected(i, true);
                    }
                }

                for (int i = 0; i < listBoxStateCMD.Items.Count; i++)
                {
                    AgvController.AgvCommunicator.State.CMD cmd = (AgvController.AgvCommunicator.State.CMD)listBoxStateCMD.Items[i];
                    if (cmd == AgvController.AgvCommunicator.State.CMD.REQUEST)
                    {
                        listBoxStateCMD.SetSelected(i, true);
                    }
                }

                SubForm_Base_Function07_Clicked(null);
            }
        }
    }
}
