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
    public partial class TrayC : UserControl
    {
        public TrayC()
        {
            InitializeComponent();
        }

        public void SetData(PIECETYTPE p1type, PIECEQUALITY p1quality, PIECETYTPE p2type, PIECEQUALITY p2quality,
                    PIECETYTPE p3type, PIECEQUALITY p3quality, PIECETYTPE p4type, PIECEQUALITY p4quality,string ID)
        {
            if (p1quality == PIECEQUALITY.待检测 && p2quality == PIECEQUALITY.待检测 && p3quality == PIECEQUALITY.待检测 && p4quality == PIECEQUALITY.待检测)
            {
                tableLayoutPanel1.InvokeEx(c => { c.BackColor = Color.Silver; });
            }
            else
            {
                if (p1quality == PIECEQUALITY.不合格 || p2quality == PIECEQUALITY.不合格 || p3quality == PIECEQUALITY.不合格 || p4quality == PIECEQUALITY.不合格)
                {
                    tableLayoutPanel1.InvokeEx(c => { c.BackColor = Color.LightCoral; });
                }
                else
                {
                    tableLayoutPanel1.InvokeEx(c => { c.BackColor = Color.GreenYellow; });
                }
            }

            if (p1quality == PIECEQUALITY.不合格)
                label1.InvokeEx(c => { c.Text = p1type.ToString(); c.ForeColor = Color.BlueViolet; });
            else
                label1.InvokeEx(c => { c.Text = p1type.ToString(); c.ForeColor = Color.Black; });

            if (p2quality == PIECEQUALITY.不合格)
                label2.InvokeEx(c => { c.Text = p2type.ToString(); c.ForeColor = Color.BlueViolet; });
            else
                label2.InvokeEx(c => { c.Text = p2type.ToString(); c.ForeColor = Color.Black; });

            if (p3quality == PIECEQUALITY.不合格)
                label3.InvokeEx(c => { c.Text = p3type.ToString(); c.ForeColor = Color.BlueViolet; });
            else
                label3.InvokeEx(c => { c.Text = p3type.ToString(); c.ForeColor = Color.Black; });

            if (p4quality == PIECEQUALITY.不合格)
                label4.InvokeEx(c => { c.Text = p4type.ToString(); c.ForeColor = Color.BlueViolet; });
            else
                label4.InvokeEx(c => { c.Text = p4type.ToString(); c.ForeColor = Color.Black; });

            labelID.InvokeEx(c => { c.Text = ID; c.ForeColor = Color.Black; });
        }
    }
}
