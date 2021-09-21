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
    public partial class MainFormDialog : BelicsClass.UI.BL_MainForm_Base
    {
        public MainFormDialog()
        {
            InitializeComponent();
        }

        protected override void MainForm_Base_Load(object sender, EventArgs e)
        {
            base.MainForm_Base_Load(sender, e);
        }

        protected override void MainForm_Base_Resize(object sender, EventArgs e)
        {
            base.MainForm_Base_Resize(sender, e);

            Invalidate();
        }
    }
}
