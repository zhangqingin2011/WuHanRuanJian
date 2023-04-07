
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
    public partial class OrderSelect : UserControl
    {
        public SelectTrayA selectTrayA = new SelectTrayA();
        public SelectTrayB selectTrayB = new SelectTrayB();
        public SelectTrayB selectTrayC= new SelectTrayB();

        public OrderSelect()
        {
            InitializeComponent();
            panel1.Controls.Add(selectTrayA);
            panel1.Controls.Add(selectTrayB);
            selectTrayA.Parent = panel1;
            selectTrayA.Dock = DockStyle.Fill;
            selectTrayB.Parent = panel1;
            selectTrayB.Dock = DockStyle.Fill;
            comboBox1.Items.Add(TRAYTYPE.叶轮料盘.ToString());
            comboBox1.Items.Add(TRAYTYPE.生肖料盘.ToString());
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            comboBox1.SelectedIndex = 0;
        }

        public ComboBox traySelect
        {
            get { return comboBox1; }
            set { comboBox1 = value; }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox1.Text == TRAYTYPE.叶轮料盘.ToString())
            {
                ShowA();
            }
            else if (comboBox1.Text == TRAYTYPE.生肖料盘.ToString())
            {
                ShowB();
            }
            else if (comboBox1.Text == TRAYTYPE.校徽料盘.ToString())
            {
                ShowC();
            }
        }

        public void ShowA()
        {
            selectTrayA.InvokeEx(c => { c.Visible = true; });
            selectTrayB.InvokeEx(c => { c.Visible = false; });
            selectTrayC.InvokeEx(c => { c.Visible = false; });
        }

        public void ShowB()
        {
            selectTrayA.InvokeEx(c => { c.Visible = false; });
            selectTrayB.InvokeEx(c => { c.Visible = true; });
            selectTrayC.InvokeEx(c => { c.Visible = false; });
        }
        public void ShowC()
        {
            selectTrayA.InvokeEx(c => { c.Visible = false; });
            selectTrayB.InvokeEx(c => { c.Visible = false; });
            selectTrayC.InvokeEx(c => { c.Visible = true; });
        }
    }
}
