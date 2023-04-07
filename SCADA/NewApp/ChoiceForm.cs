using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCADA.NewApp
{
    public partial class ChoiceForm : Form
    {
        public ChoiceForm()
        {
            InitializeComponent();
        }

        public static int TheChoose = 0; //系统选择

        private void roundButton1_Click(object sender, EventArgs e)
        {
            TheChoose = 1;
            Close();
        }

        private void roundButton2_Click(object sender, EventArgs e)
        {
            TheChoose = 2;
            Close();
        }

        private void roundButton3_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
