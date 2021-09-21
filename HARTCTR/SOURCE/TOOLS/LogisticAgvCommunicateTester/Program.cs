using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using BelicsClass.ProcessManage;

namespace LogisticAgvCommunicateTester
{
    static class Program
    {
        static public bool connect_and_autorun = false;
        static public int port_no = 0;

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            foreach (string arg in args)
            {
                if (0 <= arg.IndexOf("/P="))
                {
                    string port = arg.Replace("/P=", "");
                    if (int.TryParse(port, out port_no))
                    {
                        if (0 < port_no)
                        {
                            connect_and_autorun = true;
                        }
                    }
                }
            }

            Application.Run(new MainForm(args));

            BL_ThreadCollector.StopControl_All();
        }
    }
}
