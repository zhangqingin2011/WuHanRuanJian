
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
    public partial class RackUnit : UserControl
    {
        public RackUnit()
        {
            InitializeComponent();
        }



        public GroupBox Description
        {
            get
            {
                return groupBox1;
            }
            set
            {
                groupBox1 = value;
            }
        }



        public IOShow IOSHOW1
        {
            get
            {
                return ioShow1;
            }
            set
            {
                ioShow1 = value;
            }
        }
        public IOShow IOSHOW2
        {
            get
            {
                return ioShow2;
            }
            set
            {
                ioShow2 = value;
            }
        }
        public IOShow IOSHOW4
        {
            get
            {
                return ioShow4;
            }
            set
            {
                ioShow4 = value;
            }
        }
        public IOShow IOSHOW3
        {
            get
            {
                return ioShow3;
            }
            set
            {
                ioShow3 = value;
            }
        }
    
        public IOShow IOSHOW5
        {
            get
            {
                return ioShow5;
            }
            set
            {
                ioShow5= value;
            }
        }

        public void SetData(int No,TRAYTYPE type, PIECEQUALITY p1quality,  PIECEQUALITY p2quality,
                    PIECEQUALITY p3quality,  PIECEQUALITY p4quality,string ID )
        {
          
            if (type == TRAYTYPE.空)
            {
                Description.InvokeEx(c => { c.Text = "仓位 "+ No.ToString() + ":"+type.ToString(); });
                tableLayoutPanel1.InvokeEx(c => { c.BackColor = Color.White; });

                ioShow1.Description.Text = p1quality.ToString();
                ioShow2.Description.Text = p2quality.ToString();
                ioShow3.Description.Text = p3quality.ToString();
                ioShow4.Description.Text = p4quality.ToString();
                ioShow5.Description.Text = ID;


            }
            else
            {
                Description.InvokeEx(c => { c.Text = "仓位 " + No.ToString() + ":" + type.ToString(); });
                ioShow1.Description.Text = p1quality.ToString();
                ioShow2.Description.Text = p2quality.ToString();
                ioShow3.Description.Text = p3quality.ToString();
                ioShow4.Description.Text = p4quality.ToString();
                ioShow5.Description.Text = ID;

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
                    ioShow1.Description.ForeColor = Color.Red;

                if (p2quality == PIECEQUALITY.不合格)

                    ioShow2.Description.ForeColor = Color.Red;

                if (p3quality == PIECEQUALITY.不合格)

                    ioShow3.Description.ForeColor = Color.Red;

                if (p4quality == PIECEQUALITY.不合格)

                    ioShow4.Description.ForeColor = Color.Red;

            }
            
        }
    }
}
