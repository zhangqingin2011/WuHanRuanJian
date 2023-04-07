
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
    public partial class DefUnit : UserControl
    {
        public NoTray noTray = new NoTray();
        public TrayA trayA = new TrayA();
        public RackUnit trayB = new RackUnit();
        public TrayC trayC = new TrayC();

        public DefUnit()
        {
            InitializeComponent();
            groupBox1.Controls.Add(noTray);
            groupBox1.Controls.Add(trayA);
            groupBox1.Controls.Add(trayB);
            groupBox1.Controls.Add(trayC);
            noTray.Parent = groupBox1;
            noTray.Dock = DockStyle.Fill;
            trayA.Parent = groupBox1;
            trayA.Dock = DockStyle.Fill;
            trayB.Parent = groupBox1;
            trayB.Dock = DockStyle.Fill;
            trayC.Parent = groupBox1;
            trayC.Dock = DockStyle.Fill;
        }

        public void ShowNoTray()
        {
            noTray.InvokeEx(c => { c.Visible = true; });
            trayA.InvokeEx(c => { c.Visible = false; });
            trayB.InvokeEx(c => { c.Visible = false; });
            trayC.InvokeEx(c => { c.Visible = false; });
        }

        public void ShowTrayA()
        {
            noTray.InvokeEx(c => { c.Visible = false; });
            trayA.InvokeEx(c => { c.Visible = true; });
            trayB.InvokeEx(c => { c.Visible = false; });
            trayC.InvokeEx(c => { c.Visible = true; });
        }

        public void ShowTrayB()
        {
            noTray.InvokeEx(c => { c.Visible = false; });
            trayA.InvokeEx(c => { c.Visible = false; });
            trayB.InvokeEx(c => { c.Visible = true; });
            trayC.InvokeEx(c => { c.Visible = false; });
        }
        public void ShowTrayC()
        {
            noTray.InvokeEx(c => { c.Visible = false; });
            trayA.InvokeEx(c => { c.Visible = false; });
            trayB.InvokeEx(c => { c.Visible = false; });
            trayC.InvokeEx(c => { c.Visible = true; });
        }

        public void ShowContent(RackUnitData rackUnitData)
        {
            groupBox1.InvokeEx(c => { c.Text = string.Format("仓位NO{0}", rackUnitData.NO); });
            switch (rackUnitData.Tray)
            {
                case TRAYTYPE.空:
                    ShowNoTray();
                    break;
                case TRAYTYPE.叶轮料盘:
                    ShowTrayA();
                    trayA.SetData(rackUnitData.Piece1, rackUnitData.Piece1Quality, rackUnitData.RfidID);
                    break;
                case TRAYTYPE.生肖料盘:
                    ShowTrayB();
                    trayB.SetData(rackUnitData.Piece1, rackUnitData.Piece1Quality, rackUnitData.Piece2, rackUnitData.Piece2Quality,
                                  rackUnitData.Piece3, rackUnitData.Piece3Quality, rackUnitData.Piece4, rackUnitData.Piece4Quality, rackUnitData.RfidID);
                    break;
                case TRAYTYPE.校徽料盘:
                    ShowTrayC();
                    trayC.SetData(rackUnitData.Piece1, rackUnitData.Piece1Quality, rackUnitData.Piece2, rackUnitData.Piece2Quality,
                                  rackUnitData.Piece3, rackUnitData.Piece3Quality, rackUnitData.Piece4, rackUnitData.Piece4Quality, rackUnitData.RfidID);
                    break;
            }
        }
    }
}
