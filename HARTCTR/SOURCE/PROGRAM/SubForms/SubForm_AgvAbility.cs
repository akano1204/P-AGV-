using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BelicsClass.Common;
using BelicsClass.UI;
using BelicsClass.File;

namespace PROGRAM
{
    public partial class SubForm_AgvAbility : BelicsClass.UI.BL_SubForm_Base
    {
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
            get { return this.Text; }
        }

        public SubForm_AgvAbility()
        {
            InitializeComponent();
            Resizer_Initialize();
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            Close();
        }

        protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            LV1.Items.Clear();

            //GridLine表示
            LV1.GridLines = true;
            //ヘッダー定義
            ColumnHeader column1 = new ColumnHeader
            {
                Text = "AGV番号",
                Width = 100
            };
            ColumnHeader column2 = new ColumnHeader
            {
                Text = "AGV名",
                Width = 150
            };
            ColumnHeader column3 = new ColumnHeader
            {
                Text = "走行時間",
                Width = 130
            };
            ColumnHeader column4 = new ColumnHeader
            {
                Text = "充電時間",
                Width = 130
            };
            ColumnHeader column5 = new ColumnHeader
            {
                Text = "搬送時間",
                Width = 130
            };
            ColumnHeader column6 = new ColumnHeader
            {
                Text = "搬送回数",
                Width = 130
            };
            ColumnHeader column7 = new ColumnHeader
            {
                Text = "搬送能力",
                Width = 130
            };
            ColumnHeader column8 = new ColumnHeader
            {
                Text = "待機時間",
                Width = 150
            };
            LV1.View = View.Details;

            ColumnHeader[] colHeaderRegValue = { column1, column2, column3, column4, column5, column6, column7, column8 };
            //ヘッダー追加
            LV1.Columns.AddRange(colHeaderRegValue);

            //data_disp();
            Resizer_Initialize();
        }

        //void data_disp()
        //{
        //    LV1.Items.Clear();
        //    DateTime day = DateTime.Now;
        //    string sRtn = "";
        //    if (Program.DborFile == 1)
        //    {
        //        if (Program.reporter.Dbcon == true)
        //        {
        //            string sql = "select * from table_Ability";
        //            sql += " order by agvid ";
        //            if (!Program.reporter.Db.Execute(sql, "table_Ability"))
        //            {
        //                string err = "SQL実行エラー[" + Program.reporter.Db.ErrorMessage + "]";
        //                Log(sql);
        //                Log(err);
        //                sRtn = err;
        //            }
        //            else
        //            {

        //                if (Program.reporter.Db["table_Ability"].Rows.Count != 0)
        //                {
        //                    foreach (DataRow row in Program.reporter.Db["table_Ability"].Rows)
        //                    {
        //                        ListViewItem item = new ListViewItem();
        //                        item.Text = row["AGVID"].ToString();
        //                        item.SubItems.Add("AGV" + row["AGVID"].ToString() + "号機");
        //                        int iwk = 0;
        //                        int.TryParse(row["走行時間"].ToString(), out iwk);
        //                        TimeSpan time = TimeSpan.FromSeconds(iwk);
        //                        item.SubItems.Add(time.ToString(@"hh\:mm\:ss"));
        //                        iwk = 0;
        //                        int.TryParse(row["充電時間"].ToString(), out iwk);
        //                        time = TimeSpan.FromSeconds(iwk);
        //                        item.SubItems.Add(time.ToString(@"hh\:mm\:ss"));
        //                        iwk = 0;
        //                        int.TryParse(row["搬送時間"].ToString(), out iwk);
        //                        time = TimeSpan.FromSeconds(iwk);
        //                        item.SubItems.Add(time.ToString(@"hh\:mm\:ss"));

        //                        item.SubItems.Add(row["搬送回数"].ToString());
        //                        item.SubItems.Add(row["搬送能力"].ToString());
        //                        item.SubItems.Add(row["待機時間"].ToString());
        //                        LV1.Items.Add(item);
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    else
        //    {
        //        BL_IniFile t_Ability = null;
        //        t_Ability = new BL_IniFile(Path.Combine(Application.StartupPath, @"..\DATA\t_Ability.ini"));
        //        for (int ii = 1; ii < 100; ii++)
        //        {
        //            int iwk = 0; 
        //            bool bRtn=int.TryParse(t_Ability.Read(ii.ToString(),"走行時間", "").Trim(), out iwk);
        //            if(bRtn==true)
        //            {
        //                ListViewItem item = new ListViewItem();
        //                item.Text =iwk.ToString();
        //                item.SubItems.Add("AGV" + iwk.ToString() + "号機");
        //                TimeSpan time = TimeSpan.FromSeconds(iwk);
        //                item.SubItems.Add(time.ToString(@"hh\:mm\:ss"));
        //                iwk = 0;int.TryParse(t_Ability.Read(ii.ToString(), "充電時間", "").Trim(), out iwk);
        //                time = TimeSpan.FromSeconds(iwk);
        //                item.SubItems.Add(time.ToString(@"hh\:mm\:ss"));
        //                iwk = 0;
        //                iwk = 0; int.TryParse(t_Ability.Read(ii.ToString(), "搬送時間", "").Trim(), out iwk);
        //                time = TimeSpan.FromSeconds(iwk);
        //                item.SubItems.Add(time.ToString(@"hh\:mm\:ss"));

        //                item.SubItems.Add(t_Ability.Read(ii.ToString(), "搬送回数", ""));
        //                item.SubItems.Add(t_Ability.Read(ii.ToString(), "搬送能力", ""));
        //                item.SubItems.Add(t_Ability.Read(ii.ToString(), "待機時間", ""));
        //                LV1.Items.Add(item);
        //            }
        //        }
        //    }
        //}

        private void button1_Click(object sender, EventArgs e)
        {
            //data_disp();
        }
    }
 }
