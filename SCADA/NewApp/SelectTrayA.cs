
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
    public partial class SelectTrayA : UserControl
    {
        public SelectTrayA()
        {
            InitializeComponent();
            comboBox1.Items.Add(PRODUCTTYPE.叶轮.ToString());
            comboBox1.SelectedIndex = 0;
        }

        public ComboBox product
        {
            get { return comboBox1; }
            set { comboBox1 = value; }
        }
    }
}
