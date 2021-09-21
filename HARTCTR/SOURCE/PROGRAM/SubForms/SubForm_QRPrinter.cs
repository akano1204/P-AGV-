using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using BelicsClass.UI;
using BelicsClass.UI.Controls;

using HokushoClass.Use.Labeler;

namespace PROGRAM
{
    public partial class SubForm_QRPrinter : BelicsClass.UI.BL_SubForm_Base
    {
		#region SubFormProperties

		/// <summary>
		/// ファンクションキー文字列をMainFormに取得させるために必要です。
		/// </summary>
		/// <returns></returns>
		override public string[] FunctionStrings()
        {
            return new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "[F12]:戻る" };
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

        #endregion

        AgvControlManager controller = null;

        public SubForm_QRPrinter()
        {
            InitializeComponent();

            this.controller = Program.controller;
        }

        #region イベント

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            Close();
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            textBoxIP.Text = Program.ini_hokusho.Read("QR_LABELER", "IP_ADDRESS", "");

            list_type_changed();
        }

        private void textBoxIP_TextChanged(object sender, EventArgs e)
        {
            if (IPAddress.TryParse(textBoxIP.Text, out IPAddress _))
            {
                textBoxIP.BackColor = Color.White;
            }
            else
            {
                textBoxIP.BackColor = Color.LightPink;
            }
        }

        private void radioButtonView_CheckedChanged(object sender, EventArgs e)
        {
            var rdo = sender as RadioButton;

            if (rdo.Checked)
            {
                list_type_changed();
            }
        }

        private void listviewQR_SelectedIndexChanged(object sender, EventArgs e)
        {
            var indices = listviewQR.SelectedIndices;

            if (0 < indices.Count)
            {
                var item = listviewQR.Items[indices[0]];

                var code = item[1].ToString();

                textBoxQR.Text = code;
            }
        }

        private void buttonPrintTextBox_Click(object sender, EventArgs e)
        {
            var code = textBoxQR.Text;

            if (code != "")
            {
                var codes = new List<string> { code };

                print(codes);
            }
            else
            {
                BL_MessageBox.Show("QRコードを入力してください。", "エラー");
            }
        }

        private void buttonPrintListView_Click(object sender, EventArgs e)
        {
            var indices = listviewQR.SelectedIndices.Cast<int>().ToList();

            var codes = indices.Select(x => listviewQR.Items[x][1].ToString()).ToList();

            if (0 < codes.Count)
            {
                print(codes);
            }
            else
            {
                BL_MessageBox.Show("QRコードを選択してください。", "エラー");
            }
        }

        #endregion

        #region 印刷

        private void print(List<string> codes)
        {
            var cmds = make_command(codes);

            var lbr = new SATO();

            var ip = textBoxIP.Text;

            if (IPAddress.TryParse(ip, out IPAddress _))
            {
                if (lbr.Open(ip))
                {
                    foreach (var cmd in cmds)
                    {
                        lbr.SendBytes(cmd.BytesData);
                    }

                    lbr.Close();
                }
                else
                {
                    BL_MessageBox.Show("ラベラーのオープンに失敗しました。", "エラー");
                }
            }
            else
            {
                BL_MessageBox.Show("IPアドレスを正しく設定してください。", "エラー");
            }
        }

        #endregion

        #region ラベルコマンド生成

        private List<SATO_Commands> make_command(List<string> codes)
        {
            var ret = new List<SATO_Commands>();

            foreach (var code in codes)
            {
                var cmd = new SATO_Commands();

                cmd.Start();
                cmd.LabelSize(68, 53);
                cmd.Offset(0, 0);

                var id_11 = code.PadRight(11).Substring(0, 11);

                //QR
                //*****************************************
                cmd.Pos(37, 0, true, false);
                cmd.Line(0, 12, 2);

                cmd.Pos(37, 45, true, false);
                cmd.Line(0, 10, 2);

                cmd.Pos(0, 28, false, true);
                cmd.Line(21, 0, 2);

                cmd.Pos(54, 28, false, true);
                cmd.Line(15, 0, 2);

                cmd.Pos(22, 13);
                cmd.FormatData("2D30", ",H,10,0,0");
                cmd.FormatData("DS", "2," + id_11);

                string point;

                //point = id_11.Substring(0, 1) + ",";
                //point += id_11.Substring(1, 5) + ",";
                //point += id_11.Substring(6, 5);

                point = id_11;

                cmd.Pos(7, 6);
                cmd.FontSize(1, 1);
                cmd.FormatData("M", point);

                cmd.Pos(36, 5);
                cmd.FontSize(1, 1);
                cmd.FormatData("K9B", "▲");

                cmd.Pos(68, 27);
                cmd.FontSize(1, 1);
                cmd.FormatData("%3", "");
                cmd.FormatData("K9B", "▲");

                //*****************************************

                //QR150%
                //*****************************************
                //cmd.Pos(14, 6);
                //cmd.FormatData("2D30", ",H,15,0,0");
                //cmd.FormatData("DS", "2," + id_11);                    
                //*****************************************


                //QR75%
                //*****************************************
                //cmd.Pos(37, 0, true, false);
                //cmd.Line(0, 12, 2);

                //cmd.Pos(37, 45, true, false);
                //cmd.Line(0, 10, 2);

                //cmd.Pos(0, 28, false, true);
                //cmd.Line(21, 0, 2);

                //cmd.Pos(54, 28, false, true);
                //cmd.Line(15, 0, 2);

                //cmd.Pos(22, 13);
                //cmd.FormatData("2D30", ",H,7,0,0");
                //cmd.FormatData("DS", "2," + id_11);

                //cmd.Pos(7, 6);
                //cmd.FontSize(1, 1);
                //cmd.FormatData("M", id_11);
                //*****************************************


                //MATRIX
                //*****************************************
                //cmd.Pos(37, 0, true, false);
                //cmd.Line(0, 12, 2);

                //cmd.Pos(37, 45, true, false);
                //cmd.Line(0, 10, 2);

                //cmd.Pos(0, 28, false, true);
                //cmd.Line(21, 0, 2);

                //cmd.Pos(54, 28, false, true);
                //cmd.Line(15, 0, 2);

                //cmd.Pos(22, 13);
                //cmd.FormatData("2D50", ",18,18,000,000");
                //cmd.FormatData("DN", "0011," + id_11);

                //cmd.Pos(7, 6);
                //cmd.FontSize(1, 1);
                //cmd.FormatData("M", id_11);
                //*****************************************

                //cmd.FormatData("~", "0");

                if (code == codes.Last())
                {
                    cmd.FormatData("CT", "1");
                }

                cmd.End();

                ret.Add(cmd);
            }

            return ret;
        }

        #endregion

        #region リスト表示

        private void list_type_changed()
        {
            var listType = enListType.ALL;

            if (radioButtonFloor.Checked)
            {
                listType = enListType.FLOOR;
            }
            else if (radioButtonRack.Checked)
            {
                listType = enListType.RACK;
            }

            list_view(listType);
        }

        private void list_view(enListType listType)
        {
            listviewQR.Items.Clear();

            int no = 0;

            void add(string qr)
            {
                var item = new BL_VirtualListView.BL_VirtualListViewItem(++no);

                item.Add(qr);

                listviewQR.Items.Add(item);
            }

            if (listType == enListType.ALL || listType == enListType.FLOOR)
            {
                foreach (var v in controller.AllQR)
                {
                    add(v.QrString);
                }
            }

            if (listType == enListType.ALL || listType == enListType.RACK)
            {
                foreach (var v in controller.AllRackQR)
                {
                    var qr_string = $"R{v.rack.rack_no.PadLeft(10, '0')}";

                    add(qr_string);
                }
            }

            listviewQR.RefreshMe();
        }

        private enum enListType
        {
            /// <summary>全て</summary>
            ALL,
            /// <summary>床</summary>
            FLOOR,
            /// <summary>棚</summary>
            RACK,
        }

        #endregion
    }
}
