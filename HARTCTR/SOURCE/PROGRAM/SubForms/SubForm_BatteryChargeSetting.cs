using BelicsClass.Common;
using BelicsClass.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROGRAM
{
    public partial class SubForm_BatteryChargeSetter : BelicsClass.UI.BL_SubForm_Base
    {
        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "", "", "", "[F4]:設定", "", "", "", "", "", "", "", "[F12]:戻る" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get { return this.Text; }
        }

        public SubForm_BatteryChargeSetter()
        {
            InitializeComponent();
            Resizer_Initialize();
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            textBoxChargeNeed1.Text = Program.ini_hokusho.Get("BATTERY", "BATTERY_LOW", 10).ToString();
            textBoxChargeNeed20.Text = Program.ini_hokusho.Get("BATTERY", "PER20", 2).ToString();
            textBoxChargeNeed30.Text = Program.ini_hokusho.Get("BATTERY", "PER30", 4).ToString();
            textBoxChargeNeed40.Text = Program.ini_hokusho.Get("BATTERY", "PER40", 6).ToString();
            textBoxChargeGive.Text = Program.ini_hokusho.Get("BATTERY", "BATTERY_MID", 50).ToString();
            textBoxChargeComplete.Text = Program.ini_hokusho.Get("BATTERY", "BATTERY_FUL", 90).ToString();
            textBoxChargeStart.Text= Program.ini_hokusho.Get("BATTERY", "START", 60).ToString();
            textBoxWaitTime.Text= Program.ini_hokusho.Get("BATTERY", "WAITTIME", 5).ToString();
            textBoxAbortCharge.Text= Program.ini_hokusho.Get("BATTERY", "ABORT", 30).ToString();

            textBoxChargeNeed1.GotFocus += TextBox_GotFocus;
            textBoxChargeNeed20.GotFocus += TextBox_GotFocus;
            textBoxChargeNeed30.GotFocus += TextBox_GotFocus;
            textBoxChargeNeed40.GotFocus += TextBox_GotFocus;
            textBoxChargeGive.GotFocus += TextBox_GotFocus;
            textBoxChargeComplete.GotFocus += TextBox_GotFocus;
            textBoxChargeStart.GotFocus += TextBox_GotFocus;
            textBoxWaitTime.GotFocus += TextBox_GotFocus;
            textBoxAbortCharge.GotFocus += TextBox_GotFocus;

            labelChargeNeed1.Click += Label_Click;
            labelChargeNeed20.Click += Label_Click;
            labelChargeNeed30.Click += Label_Click;
            labelChargeNeed40.Click += Label_Click;
            labelChargeGive.Click += Label_Click;
            labelChargeComplete.Click += Label_Click;
            labelChargeStart.Click += Label_Click;
            labelWaitTime.Click += Label_Click;
            labelAbortCharge.Click += Label_Click;

            textBoxChargeNeed1.Focus();
        }

        private void Label_Click(object sender, EventArgs e)
        {
            if (sender == labelChargeNeed1) textBoxChargeNeed1.Focus();
            if (sender == labelChargeNeed20) textBoxChargeNeed20.Focus();
            if (sender == labelChargeNeed30) textBoxChargeNeed30.Focus();
            if (sender == labelChargeNeed40) textBoxChargeNeed40.Focus();
            if (sender == labelChargeGive) textBoxChargeGive.Focus();
            if (sender == labelChargeComplete) textBoxChargeComplete.Focus();
            if (sender == labelChargeStart) textBoxChargeStart.Focus();
            if (sender == labelWaitTime) textBoxWaitTime.Focus();
            if (sender == labelAbortCharge) textBoxAbortCharge.Focus();
        }

        private void TextBox_GotFocus(object sender, EventArgs e)
        {
            TextBox tb = sender as TextBox;
            if (tb == null) return;

            tb.SelectAll();
        }

        protected override void SubForm_Base_Function04_Clicked(object sender)
        {
            base.SubForm_Base_Function04_Clicked(sender);

            try
            {
                int chargeneed1 = 0; if (!int.TryParse(textBoxChargeNeed1.Text, out chargeneed1)) throw new Exception("正しい数値を入力してください。");
                int chargeneed20 = 0; if (!int.TryParse(textBoxChargeNeed20.Text, out chargeneed20)) throw new Exception("正しい数値を入力してください。");
                int chargeneed30 = 0; if (!int.TryParse(textBoxChargeNeed30.Text, out chargeneed30)) throw new Exception("正しい数値を入力してください。");
                int chargeneed40 = 0; if (!int.TryParse(textBoxChargeNeed40.Text, out chargeneed40)) throw new Exception("正しい数値を入力してください。");
                int chargegive = 0; if (!int.TryParse(textBoxChargeGive.Text, out chargegive)) throw new Exception("正しい数値を入力してください。");
                int chargecomplete = 0; if (!int.TryParse(textBoxChargeComplete.Text, out chargecomplete)) throw new Exception("正しい数値を入力してください。");

                int chargestart = 0; if(!int.TryParse(textBoxChargeStart.Text, out chargestart)) throw new Exception("正しい数値を入力してください。");
                int chargewaittime = 0; if(!int.TryParse(textBoxWaitTime.Text, out chargewaittime)) throw new Exception("正しい数値を入力してください。");
                int chargeabort = 0;if(!int.TryParse( textBoxAbortCharge.Text, out chargeabort)) throw new Exception("正しい数値を入力してください。");

                if (BL_MessageBox.Show(this, "設定を変更します。よろしいですか？", "確認", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

                bool status = true;
                status = Program.ini_hokusho.Set("BATTERY", "BATTERY_LOW", chargeneed1);
                if (status) status = Program.ini_hokusho.Set("BATTERY", "PER20", chargeneed20);
                if (status) status = Program.ini_hokusho.Set("BATTERY", "PER30", chargeneed30);
                if (status) status = Program.ini_hokusho.Set("BATTERY", "PER40", chargeneed40);
                if (status) status = Program.ini_hokusho.Set("BATTERY", "BATTERY_MID", chargegive);
                if (status) status = Program.ini_hokusho.Set("BATTERY", "BATTERY_FUL", chargecomplete);
                if (status) status = Program.ini_hokusho.Set("BATTERY", "START", chargestart);
                if (status) status = Program.ini_hokusho.Set("BATTERY", "WAITTIME", chargewaittime);
                if (status) status = Program.ini_hokusho.Set("BATTERY", "ABORT", chargeabort);

                if (status)
                {
                    BL_MessageBox.Show(this, "設定を変更しました。");
                }
                else
                {
                    BL_MessageBox.Show(this, "設定が変更できませんでした。", "エラー");
                }
            }
            catch (Exception ex)
            {
                BL_MessageBox.Show(ex.Message);
            }
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            if (textBoxChargeNeed1.Text.Trim() != Program.ini_hokusho.Get("BATTERY", "BATTERY_LOW", 10).ToString()
             || textBoxChargeNeed20.Text.Trim() != Program.ini_hokusho.Get("BATTERY", "PER20", 2).ToString()
             || textBoxChargeNeed30.Text.Trim() != Program.ini_hokusho.Get("BATTERY", "PER30", 4).ToString()
             || textBoxChargeNeed40.Text.Trim() != Program.ini_hokusho.Get("BATTERY", "PER40", 6).ToString()
             || textBoxChargeGive.Text.Trim() != Program.ini_hokusho.Get("BATTERY", "BATTERY_MID", 50).ToString()
             || textBoxChargeComplete.Text.Trim() != Program.ini_hokusho.Get("BATTERY", "BATTERY_FUL", 90).ToString()
             || textBoxChargeStart.Text.Trim() != Program.ini_hokusho.Get("BATTERY", "START", 60).ToString()
             || textBoxWaitTime.Text.Trim() != Program.ini_hokusho.Get("BATTERY", "WAITTIME", 5).ToString()
             || textBoxAbortCharge.Text.Trim() != Program.ini_hokusho.Get("BATTERY", "ABORT", 30).ToString()
             )
            {
                if (BL_MessageBox.Show(this, "設定を破棄します。よろしいですか？", "確認", MessageBoxButtons.YesNo) != DialogResult.Yes) return;
            }

            Close();
        }
    }
}
