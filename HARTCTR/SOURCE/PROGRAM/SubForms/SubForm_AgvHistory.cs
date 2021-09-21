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
    using DB = AgvDatabase;

    public partial class SubForm_AgvHistory : BelicsClass.UI.BL_SubForm_Base
    {
		#region サブフォーム

		/// <summary>
		/// ファンクションキー文字列をMainFormに取得させるために必要です。
		/// </summary>
		/// <returns></returns>
		override public string[] FunctionStrings()
        {
            return new string[] { "", "[F1]:異常", "[F2]:搬送", "", "", "", "", "", "", "", "", "", "[F12]:戻る" };
        }
        /// <summary>
        /// ウィンドウタイトル文字列をMainFormに取得させるために必要です。
        /// </summary>
        /// <returns></returns>
        override public string _TitleString
        {
            get { return this.Text; }
        }

        protected override void SubForm_Base_Function01_Clicked(object sender)
        {
            if (Header == HEADER_TYPE.ERR) return;

            Header = HEADER_TYPE.ERR;
            SetHeaders();

            base.SubForm_Base_Function01_Clicked(sender);
        }

        protected override void SubForm_Base_Function02_Clicked(object sender)
        {
            if (Header == HEADER_TYPE.WORK) return;

            Header = HEADER_TYPE.WORK;
            SetHeaders();

            base.SubForm_Base_Function02_Clicked(sender);
        }

        protected override void SubForm_Base_Function12_Clicked(object sender)
        {
            base.SubForm_Base_Function12_Clicked(sender);

            Close();
        }

        #endregion

        #region 列挙体

        private enum HEADER_TYPE
        {
            ERR,
            WORK,
        }

        #endregion

        #region フィールド

        private HEADER_TYPE Header = HEADER_TYPE.WORK;

		#endregion

		#region コンストラクタ

		public SubForm_AgvHistory()
        {
            InitializeComponent();
            Resizer_Initialize();
        }

		#endregion

		#region ロード

		protected override void SubForm_Base_Load(object sender, EventArgs e)
        {
            LV1.GridLines = true;
            LV1.View = View.Details;

            Header = HEADER_TYPE.ERR;
            SetHeaders();

            Resizer_Initialize();
        }

		#endregion

		#region メソッド

		private void SetHeaders()
        {
            LV1.Items.Clear();
            LV1.Columns.Clear();
            
            switch (Header)
            {
                case HEADER_TYPE.ERR:
                    {
                        SetTitleString("異常履歴表示");
                        
                        LV1.Columns.AddRange
                            (new ColumnHeader[]
                            {
                                new ColumnHeader { Text = "ID", Width = 0 },
                                new ColumnHeader { Text = "完了フラグ", Width = 80 },
                                new ColumnHeader { Text = "日付", Width = 100 },
                                new ColumnHeader { Text = "AGV番号", Width = 100 },
                                new ColumnHeader { Text = "異常コード", Width = 100 },
                                new ColumnHeader { Text = "異常名称", Width = 200 },
                                new ColumnHeader { Text = "最終床QR", Width = 130 },
                                new ColumnHeader { Text = "発生時刻", Width = 150 },
                                new ColumnHeader { Text = "復旧時刻", Width = 150 },
                            }
                            );
                    }

                    break;

                case HEADER_TYPE.WORK:
                    {
                        SetTitleString("搬送履歴表示");

                        LV1.Columns.AddRange
                            (new ColumnHeader[]
                            {
                                new ColumnHeader { Text = "ID", Width = 0 },
                                new ColumnHeader { Text = "日付", Width = 100 },
                                new ColumnHeader { Text = "指示親番", Width = 100 },
                                new ColumnHeader { Text = "指示子番", Width = 100 },
                                new ColumnHeader { Text = "最終マーク", Width = 100 },
                                new ColumnHeader { Text = "AGV番号", Width = 100 },
                                new ColumnHeader { Text = "開始時刻", Width = 150 },
                                new ColumnHeader { Text = "終了時刻", Width = 150 },
                                new ColumnHeader { Text = "現在地QR", Width = 130 },
                                new ColumnHeader { Text = "目的地QR", Width = 130 },
                                new ColumnHeader { Text = "目的ST", Width = 130 },
                                new ColumnHeader { Text = "目的地動作", Width = 130 },
                                new ColumnHeader { Text = "ラックID", Width = 130 },
                                new ColumnHeader { Text = "ラック到着面", Width = 130 },
                                new ColumnHeader { Text = "センサー", Width = 130 },
                                new ColumnHeader { Text = "ミュージック", Width = 130 },
                                new ColumnHeader { Text = "動作OP1", Width = 100 },
                                new ColumnHeader { Text = "動作OP2", Width = 100 },
                                new ColumnHeader { Text = "動作OP3", Width = 100 },
                                new ColumnHeader { Text = "動作OP4", Width = 100 },
                                new ColumnHeader { Text = "動作OP5", Width = 100 },
                                new ColumnHeader { Text = "動作情報1", Width = 150 },
                                new ColumnHeader { Text = "動作情報2", Width = 150 },
                                new ColumnHeader { Text = "動作情報3", Width = 150 },
                                new ColumnHeader { Text = "動作情報4", Width = 150 },
                                new ColumnHeader { Text = "動作情報5", Width = 150 },
                            }
                            );
                    }

                    break;
            }

            Disp();
        }

        void Disp()
        {
            var error = "";

            LV1.Items.Clear();

            switch (Header)
            {
                case HEADER_TYPE.ERR:
                    {
                        error = DB.AGV_ERR_VIEW.Select(out List<DB.AGV_ERR_VIEW> rs);

                        foreach (var r in rs)
                        {
                            var item = new ListViewItem
                            {
                                Text = r.ID.ToString(),
                            };

                            var end_time = r.END_TIME;

                            item.SubItems.AddRange
                                (new string[]
                                {
                                    BL_EnumLabel.GetLabel(r.FINISH),
                                    r.MAKE_DATE,
                                    r.AGV_ID,
                                    r.ERR_CODE.ToString(),
                                    r.ERR_NAME,
                                    r.FLOOR_QR,
                                    r.START_TIME.ToString(),
                                    end_time == null ? "" : end_time.ToString(),
                                }
                                );

                            LV1.Items.Add(item);
                        }
                    }

                    break;

                case HEADER_TYPE.WORK:
                    {
                        error = DB.AGV_WORK_VIEW.Select(out List<DB.AGV_WORK_VIEW> rs);

                        foreach (var r in rs)
                        {
                            var item = new ListViewItem
                            {
                                Text = r.ID.ToString(),
                            };

                            var end_time = r.END_TIME;

                            item.SubItems.AddRange
                                (new string[]
                                {
                                    r.MAKE_DATE,
                                    r.ORDER_NO.ToString(),
                                    r.ORDER_SUB_NO.ToString(),
                                    BL_EnumLabel.GetLabel(r.ORDER_MARK),
                                    r.AGV_ID,
                                    r.START_TIME.ToString(),
                                    end_time == null ? "" : end_time.ToString(),
                                    r.FROM_QR,
                                    r.TO_QR,
                                    r.ST_TO,
                                    BL_EnumLabel.GetLabel(r.ST_ACTION),
                                    r.RACK_ID,
                                    BL_EnumLabel.GetLabel(r.RACK_ANGLE),
                                    r.SENSOR.ToString(),
                                    BL_EnumLabel.GetLabel(r.MUSIC),
                                    r.ORDER_OP1.ToString(),
                                    r.ORDER_OP2.ToString(),
                                    r.ORDER_OP3.ToString(),
                                    r.ORDER_OP4.ToString(),
                                    r.ORDER_OP5.ToString(),
                                    r.O_INFO1,
                                    r.O_INFO2,
                                    r.O_INFO3,
                                    r.O_INFO4,
                                    r.O_INFO5,
                                }
                                );

                            LV1.Items.Add(item);
                        }
                    }

                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Disp();
        }

        #endregion
    }
 }
