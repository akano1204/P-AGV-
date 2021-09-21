using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BelicsClass.Common;
using BelicsClass.UI;

namespace PROGRAM
{
    public partial class SubForm_Test : BelicsClass.UI.BL_SubForm_Base
    {

        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要で   す。
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
            get { return this.Text; }
        }

        public SubForm_Test()
        {
            InitializeComponent();
        }

        private void cmb_set()
        {
            comboBox1.Items.Add("走行開始");
            comboBox1.Items.Add("走行停止");
            comboBox1.Items.Add("ラック上昇");
            comboBox1.Items.Add("ラック下降");
            comboBox1.Items.Add("ステーション作業開始");
            comboBox1.Items.Add("ステーション作業完了");
            comboBox1.Items.Add("充電開始");
            comboBox1.Items.Add("充電終了");
            comboBox1.Items.Add("衝突検知");
            comboBox1.SelectedIndex = 0;
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.Text)
            {
                case "走行開始":
                    lblPoint.Visible = true;
                    txtPointX.Visible = true;
                    txtPointY.Visible = true;
                    lblTanaID.Visible = true;
                    txtTanaID.Visible = true;
                    lblMenID.Visible = false;
                    txtMenID.Visible = false;
                    lblStationID.Visible = false;
                    txtStationID.Visible = false;
                    lblSoukou.Visible = false;
                    txtSoukou.Visible = false;
                    lblTuukaQR.Visible = false;
                    txtTuukaQR.Visible = false;
                    lblByou.Visible = false;
                    txtByou.Visible = false;
                    lblBattZan.Visible = true;
                    txtBattZan.Visible = true;
                    lblShoutouID.Visible = false;
                    txtShoutouID.Visible = false;
                    break;
                case "走行停止":
                    lblPoint.Visible = true;
                    txtPointX.Visible = true;
                    txtPointY.Visible = true;
                    lblTanaID.Visible = true;
                    txtTanaID.Visible = true;
                    lblMenID.Visible = false;
                    txtMenID.Visible = false;
                    lblStationID.Visible = false;
                    txtStationID.Visible = false;
                    lblSoukou.Visible = true;
                    txtSoukou.Visible = true;
                    lblTuukaQR.Visible = true;
                    txtTuukaQR.Visible = true;
                    lblByou.Visible = true;
                    txtByou.Visible = true;
                    lblBattZan.Visible = true;
                    txtBattZan.Visible = true;
                    lblShoutouID.Visible = false;
                    txtShoutouID.Visible = false;
                    break;
                case "ラック上昇":
                    lblPoint.Visible = true;
                    txtPointX.Visible = true;
                    txtPointY.Visible = true;
                    lblTanaID.Visible = true;
                    txtTanaID.Visible = true;
                    lblMenID.Visible = false;
                    txtMenID.Visible = false;
                    lblStationID.Visible = false;
                    txtStationID.Visible = false;
                    lblSoukou.Visible = false;
                    txtSoukou.Visible = false;
                    lblTuukaQR.Visible = false;
                    txtTuukaQR.Visible = false;
                    lblByou.Visible = false;
                    txtByou.Visible = false;
                    lblBattZan.Visible = true;
                    txtBattZan.Visible = true;
                    lblShoutouID.Visible = false;
                    txtShoutouID.Visible = false;
                    break;
                case "ラック下降":
                    lblPoint.Visible = true;
                    txtPointX.Visible = true;
                    txtPointY.Visible = true;
                    lblTanaID.Visible = true;
                    txtTanaID.Visible = true;
                    lblMenID.Visible = false;
                    txtMenID.Visible = false;
                    lblStationID.Visible = false;
                    txtStationID.Visible = false;
                    lblSoukou.Visible = false;
                    txtSoukou.Visible = false;
                    lblTuukaQR.Visible = false;
                    txtTuukaQR.Visible = false;
                    lblByou.Visible = false;
                    txtByou.Visible = false;
                    lblBattZan.Visible = true;
                    txtBattZan.Visible = true;
                    lblShoutouID.Visible = false;
                    txtShoutouID.Visible = false;
                    break;
                case "ステーション作業開始":
                    lblPoint.Visible = true;
                    txtPointX.Visible = true;
                    txtPointY.Visible = true;
                    lblTanaID.Visible = true;
                    txtTanaID.Visible = true;
                    lblMenID.Visible = true;
                    txtMenID.Visible = true;
                    lblStationID.Visible = true;
                    txtStationID.Visible = true;
                    lblSoukou.Visible = false;
                    txtSoukou.Visible = false;
                    lblTuukaQR.Visible = false;
                    txtTuukaQR.Visible = false;
                    lblByou.Visible = false;
                    txtByou.Visible = false;
                    lblBattZan.Visible = false;
                    txtBattZan.Visible = false;
                    lblShoutouID.Visible = false;
                    txtShoutouID.Visible = false;
                    break;
                case "ステーション作業完了":
                    lblPoint.Visible = true;
                    txtPointX.Visible = true;
                    txtPointY.Visible = true;
                    lblTanaID.Visible = true;
                    txtTanaID.Visible = true;
                    lblMenID.Visible = true;
                    txtMenID.Visible = true;
                    lblStationID.Visible = true;
                    txtStationID.Visible = true;
                    lblSoukou.Visible = false;
                    txtSoukou.Visible = false;
                    lblTuukaQR.Visible = false;
                    txtTuukaQR.Visible = false;
                    lblByou.Visible = true;
                    txtByou.Visible = true;
                    lblBattZan.Visible = false;
                    txtBattZan.Visible = false;
                    lblShoutouID.Visible = false;
                    txtShoutouID.Visible = false;
                    break;
                case "充電開始":
                    lblPoint.Visible = true;
                    txtPointX.Visible = true;
                    txtPointY.Visible = true;
                    lblTanaID.Visible = false;
                    txtTanaID.Visible = false;
                    lblMenID.Visible = false;
                    txtMenID.Visible = false;
                    lblStationID.Visible = false;
                    txtStationID.Visible = false;
                    lblSoukou.Visible = false;
                    txtSoukou.Visible = false;
                    lblTuukaQR.Visible = false;
                    txtTuukaQR.Visible = false;
                    lblByou.Visible = false;
                    txtByou.Visible = false;
                    lblBattZan.Visible = true;
                    txtBattZan.Visible = true;
                    lblShoutouID.Visible = false;
                    txtShoutouID.Visible = false;
                    break;
                case "充電終了":
                    lblPoint.Visible = true;
                    txtPointX.Visible = true;
                    txtPointY.Visible = true;
                    lblTanaID.Visible = false;
                    txtTanaID.Visible = false;
                    lblMenID.Visible = false;
                    txtMenID.Visible = false;
                    lblStationID.Visible = false;
                    txtStationID.Visible = false;
                    lblSoukou.Visible = false;
                    txtSoukou.Visible = false;
                    lblTuukaQR.Visible = false;
                    txtTuukaQR.Visible = false;
                    lblByou.Visible = true;
                    txtByou.Visible = true;
                    lblBattZan.Visible = true;
                    txtBattZan.Visible = true;
                    lblShoutouID.Visible = false;
                    txtShoutouID.Visible = false;
                    break;
                case "衝突検知":
                    lblPoint.Visible = true;
                    txtPointX.Visible = true;
                    txtPointY.Visible = true;
                    lblTanaID.Visible = false;
                    txtTanaID.Visible = false;
                    lblMenID.Visible = false;
                    txtMenID.Visible = false;
                    lblStationID.Visible = false;
                    txtStationID.Visible = false;
                    lblSoukou.Visible = false;
                    txtSoukou.Visible = false;
                    lblTuukaQR.Visible = false;
                    txtTuukaQR.Visible = false;
                    lblByou.Visible = false;
                    txtByou.Visible = false;
                    lblBattZan.Visible = false;
                    txtBattZan.Visible = false;
                    lblShoutouID.Visible = true;
                    txtShoutouID.Visible = true;
                    break;
            }
        }

        private void SubForm_Test_Load(object sender, EventArgs e)
        {
            cmb_set();
            txtAGVID.Text = "1";
            txtPointX.Text = "100";
            txtPointY.Text = "200";
            txtTanaID.Text = "2";
            txtMenID.Text = "3";
            txtStationID.Text = "1";
            txtSoukou.Text = "300";
            txtTuukaQR.Text = "10";
            txtByou.Text = "30";
            txtBattZan.Text = "100";
            txtShoutouID.Text = "2";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Point po = new Point();
            //DateTime OccurTime = DateTime.Now;

            //switch (comboBox1.Text)
            //{
            //    case "走行開始":
            //        po.X = int.Parse(txtPointX.Text);
            //        po.Y = int.Parse(txtPointY.Text);
            //        Program.reporter.StartRun(OccurTime, txtAGVID.Text, po, txtTanaID.Text, int.Parse(txtBattZan.Text));
            //        break;
            //    case "走行停止":
            //        po.X = int.Parse(txtPointX.Text);
            //        po.Y = int.Parse(txtPointY.Text);
            //        Program.reporter.StopRun(OccurTime, txtAGVID.Text, po, txtTanaID.Text, int.Parse(txtBattZan.Text), float.Parse(txtSoukou.Text), int.Parse(txtTuukaQR.Text), int.Parse(txtByou.Text));
            //        break;
            //    case "ラック上昇":
            //        po.X = int.Parse(txtPointX.Text);
            //        po.Y = int.Parse(txtPointY.Text);
            //        Program.reporter.RackUp(txtAGVID.Text, po, txtTanaID.Text,int.Parse(txtBattZan.Text));
            //        break;
            //    case "ラック下降":
            //        po.X = int.Parse(txtPointX.Text);
            //        po.Y = int.Parse(txtPointY.Text);
            //        Program.reporter.RackDown(txtAGVID.Text, po, txtTanaID.Text, int.Parse(txtBattZan.Text));
            //        break;
            //    case "ステーション作業開始":
            //        po.X = int.Parse(txtPointX.Text);
            //        po.Y = int.Parse(txtPointY.Text);
            //        Program.reporter.StartStation(txtAGVID.Text, po, txtTanaID.Text, txtMenID.Text, txtStationID.Text);
            //        break;
            //    case "ステーション作業完了":
            //        po.X = int.Parse(txtPointX.Text);
            //        po.Y = int.Parse(txtPointY.Text);
            //        Program.reporter.FinishStation(txtAGVID.Text, po, txtTanaID.Text, txtMenID.Text, txtStationID.Text, int.Parse(txtByou.Text));
            //        break;
            //    case "充電開始":
            //        po.X = int.Parse(txtPointX.Text);
            //        po.Y = int.Parse(txtPointY.Text);
            //        Program.reporter.StartCharge(txtAGVID.Text, po, int.Parse(txtBattZan.Text));
            //        break;
            //    case "充電終了":
            //        po.X = int.Parse(txtPointX.Text);
            //        po.Y = int.Parse(txtPointY.Text);
            //        Program.reporter.StopCharge(txtAGVID.Text, po, int.Parse(txtBattZan.Text), int.Parse(txtByou.Text));
            //        break;
            //    case "衝突検知":
            //        po.X = int.Parse(txtPointX.Text);
            //        po.Y = int.Parse(txtPointY.Text);
            //        Program.reporter.CrashStop(txtAGVID.Text, po, txtShoutouID.Text);
            //        break;

            //}
        }
        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int iwk = 0;
            if(int.TryParse(textBox1.Text, out iwk))
            {
                //Program.reporter.Reset_Integrated(iwk,DateTime.Now);
            }
        }
    }
}
