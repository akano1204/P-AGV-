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

namespace PROGRAM
{
    public partial class MainForm : BelicsClass.UI.BL_MainForm_Base
    {
        public MainForm()
        {
            InitializeComponent();
        }

        protected override void MainForm_Base_Load(object sender, EventArgs e)
        {
            base.MainForm_Base_Load(sender, e);

            BL_SubForm_Base sub = new SubForm_Menu();
            sub.ShowMe(this);

            int left = Program.ini_hokusho.Read("SCREEN", "LEFT", this.Left);
            int top = Program.ini_hokusho.Read("SCREEN", "TOP", this.Top);
            int width = Program.ini_hokusho.Read("SCREEN", "WIDTH", this.Width);
            int height = Program.ini_hokusho.Read("SCREEN", "HEIGHT", this.Height);

            bool innerscreen = false;
            foreach (var v in Screen.AllScreens)
            {
                if (v.Bounds.Contains(new Point(left + 6, top + 6)) && v.Bounds.Contains(new Point(left + width, top + height)))
                {
                    innerscreen = true;
                    break;
                }
            }

            if (innerscreen)
            {
                this.Left = left;
                this.Top = top;
                this.Width = width;
                this.Height = height;
            }

            if (Program.controller.run_manager != null)
            {
                Program.controller.run_manager.RequestExit += Run_manager_RequestExit;
            }

            Program.controller.EventAgvDatabaseError += Controller_EventAgvDatabaseError;

			//var error = AgvDatabase.Initialize();

			//if (error != "")
			//{
			//	BL_MessageBox.Show(error);
			//}
		}

        private string error_message_pre = "";

        private void Controller_EventAgvDatabaseError(AgvControlManager.AgvRunner sender, string message)
        {
            MethodInvoker process = (MethodInvoker)delegate ()
            {
                Controller_EventAgvDatabaseError_func(sender, message);
            };

            try
            {
                if (InvokeRequired) Invoke(process);
                else process.Invoke();
            }
            //catch (ObjectDisposedException) { }
            //catch (InvalidOperationException) { }
            catch (Exception) { }
        }

        private void Controller_EventAgvDatabaseError_func(AgvControlManager.AgvRunner sender, string message)
        {
            if (error_message_pre != message)
            {
                error_message_pre = message;

                BL_MessageBox.Show(this, message, "DB異常");
            }
        }

        private void Run_manager_RequestExit(AgvControlManager.AgvRunManager sender)
        {
            MethodInvoker process = (MethodInvoker)delegate ()
            {
                ExitApplication();
            };

            try
            {
                if (InvokeRequired) Invoke(process);
                else process.Invoke();
            }
            //catch (ObjectDisposedException) { }
            //catch (InvalidOperationException) { }
            catch (Exception) { }
        }

        BL_SubForm_MonitorLog debug = null;

        protected override void MainForm_Base_OnDebugMode(object sender, bool isDebugMode)
        {
            base.MainForm_Base_OnDebugMode(sender, isDebugMode);

            if (isDebugMode)
            {
                if (debug == null)
                {
                    debug = new BL_SubForm_MonitorLog();
                    debug.ShowMe(this);
                }
            }
            else
            {
                if (debug != null)
                {
                    debug.Close();
                    debug.Dispose();
                    debug = null;
                }
            }
        }

        protected override void MainForm_Base_Exit_Click(object sender, EventArgs e)
        {
            //base.MainForm_Base_Exit_Click(sender, e);

            ExitApplication("終了します。よろしいですか？");
        }

        protected override void MainForm_Base_FormClosing(object sender, FormClosingEventArgs e)
        {
            base.MainForm_Base_FormClosing(sender, e);

            Program.ini_hokusho.Write("SCREEN", "LEFT", this.Left);
            Program.ini_hokusho.Write("SCREEN", "TOP", this.Top);
            Program.ini_hokusho.Write("SCREEN", "WIDTH", this.Width);
            Program.ini_hokusho.Write("SCREEN", "HEIGHT", this.Height);
        }

        protected override void MainForm_Base_Resize(object sender, EventArgs e)
        {
            base.MainForm_Base_Resize(sender, e);

            Program.controller.map.Values.ToList().ForEach(ee => { if (ee != null) ee.mapeditor.redraw_map = true; } );

            Invalidate();
        }
    }
}
