using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using BelicsClass.UI;

namespace LogisticAgvCommunicateTester
{
    public partial class MainForm : BL_MainForm_Base
    {
        public MainForm(string[] args)
        {
            InitializeComponent();
        }

        protected override void MainForm_Base_Load(object sender, EventArgs e)
        {
            base.MainForm_Base_Load(sender, e);

            if (Program.connect_and_autorun)
            {
                var sub = new SubForm_AgvComManual();
                sub.ShowMe(this);

                var sub2 = new SubForm_AgvStaSender();
                sub2.ShowMe(sub);
            }
            else
            {
                var sub = new SubForm_AgvComManual();
                sub.ShowMe(this);
            }
        }
    }
}
