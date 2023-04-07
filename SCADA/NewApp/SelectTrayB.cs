
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
    public partial class SelectTrayB : UserControl
    {
        public SelectTrayB()
        {
            InitializeComponent();
            comboBox1.Items.Add(PRODUCTTYPE.校徽.ToString());
            comboBox1.SelectedIndex = 0;

        }

        public ComboBox product1
        {
            get { return comboBox1; }
            set { comboBox1 = value; }
        }

        public ComboBox product2
        {
            get { return comboBox1; }
            set { comboBox1 = value; }
        }

        public ComboBox product3
        {
            get { return comboBox1; }
            set { comboBox1 = value; }
        }

        public ComboBox product4
        {
            get { return comboBox1; }
            set { comboBox1 = value; }
        }


    }
}
