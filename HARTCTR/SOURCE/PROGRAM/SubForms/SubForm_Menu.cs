using BelicsClass.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

#pragma warning disable CS0219 // 変数は割り当てられていますが、その値は使用されていません

namespace PROGRAM
{
    public partial class SubForm_Menu : BelicsClass.UI.BL_SubForm_Base
    {

        /// <summary>
        /// ファンクションキー文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string[] FunctionStrings()
        {
            return new string[] { "", "[F1]:", "[F2]:", "[F3]:", "[F4]:", "[F5]:", "[F6]:", "[F7]:", "[F8]:", "", "", "", "[F12]:終了" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get { return this.Text; }
        }

        private Dictionary<int, List<Tuple<Control, int, string>>> dictTabControl = new Dictionary<int, List<Tuple<Control, int, string>>>();



        public SubForm_Menu()
        {
            InitializeComponent();

            dictTabControl[0] = new List<Tuple<Control, int, string>>();
            dictTabControl[1] = new List<Tuple<Control, int, string>>();
            dictTabControl[2] = new List<Tuple<Control, int, string>>();
            dictTabControl[3] = new List<Tuple<Control, int, string>>();

            //dictTabControl[0].Add(new Tuple<Control, int, string>(button011, 11, ""/*"運用選択"*/));
            dictTabControl[0].Add(new Tuple<Control, int, string>(button012, 12, "運用モニター"));
            dictTabControl[0].Add(new Tuple<Control, int, string>(button013, 13, "動作モニター"));
            dictTabControl[0].Add(new Tuple<Control, int, string>(button014, 14, "状況モニター"));
            dictTabControl[0].Add(new Tuple<Control, int, string>(button015, 15, "履歴表示"));
            dictTabControl[0].Add(new Tuple<Control, int, string>(button016, 16, "能力表示"));
            //dictTabControl[0].Add(new Tuple<Control, int, string>(button017, 17, ""));
            dictTabControl[0].Add(new Tuple<Control, int, string>(button018, 18, "TEST"));

            //dictTabControl[1].Add(new Tuple<Control, int, string>(button021, 21, "モード選択"));
            //dictTabControl[1].Add(new Tuple<Control, int, string>(button022, 22, "モード動作モニター"));
            //dictTabControl[1].Add(new Tuple<Control, int, string>(button023, 23, ""));
            //dictTabControl[1].Add(new Tuple<Control, int, string>(button024, 24, ""));
            dictTabControl[1].Add(new Tuple<Control, int, string>(button025, 25, "モード動作編集"));
            //dictTabControl[1].Add(new Tuple<Control, int, string>(button026, 26, "モード動作シミュレーション"));
            //dictTabControl[1].Add(new Tuple<Control, int, string>(button027, 27, ""));
            //dictTabControl[1].Add(new Tuple<Control, int, string>(button028, 28, ""));

            dictTabControl[2].Add(new Tuple<Control, int, string>(buttonS1, 81, "マップ編集"));
            dictTabControl[2].Add(new Tuple<Control, int, string>(buttonS2, 82, "充電動作設定"));
            //dictTabControl[2].Add(new Tuple<Control, int, string>(buttonS3, 83, ""));
            //dictTabControl[2].Add(new Tuple<Control, int, string>(buttonS4, 84, ""));
            dictTabControl[2].Add(new Tuple<Control, int, string>(buttonS5, 85, "AGV通信設定"));
            dictTabControl[2].Add(new Tuple<Control, int, string>(buttonS6, 86, "棚設定"));
            dictTabControl[2].Add(new Tuple<Control, int, string>(buttonS7, 87, "オートレーター設定"));
            dictTabControl[2].Add(new Tuple<Control, int, string>(buttonS8, 88, "ステーション設定"));

            //dictTabControl[3].Add(new Tuple<Control, int, string>(buttonM1, 91, ""/*"運用シミュレーション"*/));
            //dictTabControl[3].Add(new Tuple<Control, int, string>(buttonM2, 92, ""));
            //dictTabControl[3].Add(new Tuple<Control, int, string>(buttonM3, 93, ""));
            //dictTabControl[3].Add(new Tuple<Control, int, string>(buttonM4, 94, ""));
            //dictTabControl[3].Add(new Tuple<Control, int, string>(buttonM5, 95, ""));
            dictTabControl[3].Add(new Tuple<Control, int, string>(buttonM6, 96, "QRコード発行"));
            //dictTabControl[3].Add(new Tuple<Control, int, string>(buttonM7, 97, ""));
            //dictTabControl[3].Add(new Tuple<Control, int, string>(buttonM8, 98, ""));

            foreach (var kv in dictTabControl)
            {
                foreach (var v in kv.Value)
                {
                    v.Item1.Text = v.Item3;
                    v.Item1.Tag = v.Item2;
                }
            }

            Resizer_Initialize();

            this.Text = Program.ini_hokusho.Read("SCREEN", "TITLE", "管理メニュー");
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            base.SubForm_Base_Load(sender, e);

            labelVER.Text = Program.VER;

            tabControl1_SelectedIndexChanged(null, null);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            for (int i = 0; i < dictTabControl[tabControl1.SelectedIndex].Count; i++)
            {
                Button b = dictTabControl[tabControl1.SelectedIndex][i].Item1 as Button;
                if (b != null)
                {
                    AttachButton_to_Functions(b, i + 1);
                }
            }
        }

        private void MenuTrasition(object sender)
        {
            BL_SubForm_Base sub = null;

            Control c = sender as Control;
            if (c != null)
            {
                try
                {
                    int no = (int)c.Tag;

                    switch (no)
                    {
                        case 11: sub = new SubForm_OperationSelecter(); break;
                        case 12: sub = new SubForm_AgvMonitor(); break;
                        case 13: sub = new SubForm_AutoCommander(); break;
                        case 14: sub = new SubForm_AgvStateMonitor(); break;
                        case 15: sub = new SubForm_AgvHistory(); break;
                        case 16: sub = new SubForm_AgvAbility(); break;
                        case 17:; break;
                        case 18:; sub = new SubForm_Test(); break;

                        case 21: sub = new SubForm_ModeSelecter(); break;
                        //case 22: sub = new SubForm_ModeCommander(); break;
                        case 23:; break;
                        case 24:; break;
                        case 25: sub = new SubForm_ModeConditioner(); break;
                        //case 26: sub = new SubForm_ModeSimulator(); break;
                        case 27:; break;
                        case 28:; break;

                        case 81: sub = new SubForm_MapEditor(); break;
                        case 82: sub = new SubForm_BatteryChargeSetter(); break;
                        case 83:; break;
                        case 84:; break;
                        case 85: sub = new SubForm_AgvCommunicationSetter(); break;
                        case 86: sub = new SubForm_RackSetting(); break;
                        case 87: sub = new SubForm_AutoratorSetting(); break;
                        case 88:; break;

                        //case 91: sub = new SubForm_OperationSimulator(); break;
                        case 92:; break;
                        case 93:; break;
                        case 94:; break;
                        case 95:; break;
                        case 96: sub = new SubForm_QRPrinter(); break;
                        case 97:; break;
                        case 98:; break;
                    }
                }
                catch { }
            }

            if (sub != null) sub.ShowMe(this);
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            ExitApplication("終了します。よろしいですか？");
        }

        protected override void SubForm_Base_Function01_Clicked(object sender)
        {
            base.SubForm_Base_Function01_Clicked(sender);

            MenuTrasition(sender);
        }

        protected override void SubForm_Base_Function02_Clicked(object sender)
        {
            base.SubForm_Base_Function02_Clicked(sender);

            MenuTrasition(sender);
        }

        protected override void SubForm_Base_Function03_Clicked(object sender)
        {
            base.SubForm_Base_Function03_Clicked(sender);

            MenuTrasition(sender);
        }

        protected override void SubForm_Base_Function04_Clicked(object sender)
        {
            base.SubForm_Base_Function04_Clicked(sender);

            MenuTrasition(sender);
        }

        protected override void SubForm_Base_Function05_Clicked(object sender)
        {
            base.SubForm_Base_Function05_Clicked(sender);

            MenuTrasition(sender);
        }

        protected override void SubForm_Base_Function06_Clicked(object sender)
        {
            base.SubForm_Base_Function06_Clicked(sender);

            MenuTrasition(sender);
        }

        protected override void SubForm_Base_Function07_Clicked(object sender)
        {
            base.SubForm_Base_Function07_Clicked(sender);

            MenuTrasition(sender);
        }

        protected override void SubForm_Base_Function08_Clicked(object sender)
        {
            base.SubForm_Base_Function08_Clicked(sender);

            MenuTrasition(sender);
        }
    }
}

#pragma warning restore CS0219 // 変数は割り当てられていますが、その値は使用されていません
