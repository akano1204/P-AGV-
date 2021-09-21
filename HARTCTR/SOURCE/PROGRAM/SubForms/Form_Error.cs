using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

using BelicsClass.Common;
using BelicsClass.UI;
using BelicsClass.ObjectSync;
using BelicsClass.File;

namespace PROGRAM
{
    public partial class Form_Error : BL_SubForm_Base
    {
        private const string ERRNAME = @"\DATA\ERRNAME.DAT";

        public int ErrCode;

        /// <summary>
        /// 異常名称ファイル構造体
        /// </summary>
        public class ERR_NAME_ST : BL_ObjectSync
        {
            #region フィールド

            /// <summary>
            /// 異常コード
            /// </summary>
            [BL_ObjectSync]
            public string code = "".PadRight(5);
            /// <summary>
            /// 異常名称
            /// </summary>
            [BL_ObjectSync]
            public string name = "".PadRight(45);
            /// <summary>
            /// コメント
            /// </summary>
            [BL_ObjectSync]
            public string comment = "".PadRight(200);
            /// <summary>
            /// 復旧方法
            /// </summary>
            [BL_ObjectSync]
            public string recover = "".PadRight(250);

            #endregion

            public ERR_NAME_ST()
            {
                Initialize();
            }
        }

        AgvControlManager.FloorAGV agv = null;
        static Dictionary<string, ERR_NAME_ST> err_masters = new Dictionary<string, ERR_NAME_ST>();

        public Form_Error(AgvControlManager.FloorAGV agv)
        {
            this.StartPosition = FormStartPosition.CenterParent;
            InitializeComponent();

            Resizer_Initialize();

            this.agv = agv;
        }

        private void Form_Error_Load(object sender, EventArgs e)
        {
            lblCode.Text = "";
            lblName.Text = "";
            lblRecovery.Text = "";

            if (err_masters.Count == 0)
            {
                ERR_NAME_ST err = new ERR_NAME_ST();
                BL_FixedFile file = new BL_FixedFile();
                if (file.Open(Path.Combine(Application.StartupPath, @"..\DATA\ERRNAME.DAT"), err.Length, "r", "rw"))
                {
                    for (int pos = 0; ; pos++)
                    {
                        byte[] data = new byte[err.Length];
                        if (file.Read(out data, pos))
                        {
                            err = new ERR_NAME_ST();
                            err.SetBytes(data);

                            err_masters[err.code] = err;
                        }
                        else break;
                    }
                }
            }

            timerClock.Enabled = true;
        }

        protected override void timerClock_Tick(object sender, EventArgs e)
        {
            //base.timerClock_Tick(sender, e);
            timerClock.Enabled = false;


            if (agv == null) return;
            if (agv.agvRunner == null) return;
            if (agv.agvRunner.communicator == null) return;
            if (agv.agvRunner.communicator.GetState.error_code == 0)
            {
                labelAGV.Text = agv.id;
                lblCode.Text = "";
                lblName.Text = "";
                lblRecovery.Text = "";
            }
            else
            {
                lblCode.Text = agv.agvRunner.communicator.GetState.error_code.ToString("00000");
                lblName.Text = "";
                lblRecovery.Text = "";

                if (err_masters.ContainsKey(lblCode.Text))
                {
                    lblName.Text = err_masters[lblCode.Text].name;
                    lblRecovery.Text = err_masters[lblCode.Text].recover;
                }
            }

            timerClock.Enabled = true;
        }
    }
}
