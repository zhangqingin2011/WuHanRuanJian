using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using SCADA.SimensPLC;

namespace SCADA.NewApp
{
    public partial class RFIDForm : Form
    {
        public RFIDForm()
        {
            InitializeComponent();
            //InitUI();
            //InitConfig();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string rfidid = "";
            rfidid = LineMainForm.sygolefidwmsin.readRFIDUID();
            if (rfidid == null)
            {
                rfidid = LineMainForm.sygolefidwmsin.readRFIDUID();
                if (rfidid == null)
                {
                    rfidid = LineMainForm.sygolefidwmsin.readRFIDUID();
                    if (rfidid == null)
                    {
                        textBox2.Text = "RFID读取失败";
                    }
                }
                else
                {
                    textBox2.Text = rfidid;
                }
            }
            else
            {
                textBox2.Text = rfidid;
            }
        }


        private void button3_Click(object sender, EventArgs e)
        {
            string rfidid = "";
            rfidid = LineMainForm.sygolefidwmsin.readRFIDUID();
            if (rfidid == null)
            {
                rfidid = LineMainForm.sygolefidwmsin.readRFIDUID();
            }
            if (rfidid == null)
            {
                rfidid = LineMainForm.sygolefidwmsin.readRFIDUID();
                if (rfidid == null)
                {
                    label4.Text = "初始化失败！";
                    return;
                }
            }
            if (rfidid != null)
            {
                textBox1.Text = rfidid;
                if (comboBox1.SelectedIndex == 0)
                {
                    var data = Program.Repo.GetSingle<Panel>(p => p.RfidID == rfidid);
                    if (data == null)
                    {
                        var t = new Panel
                        {
                            Tray = TRAYTYPE.料盘A,
                            RfidID = rfidid,
                        };
                        Program.Repo.Insert(t);
                        label4.Text = "初始化成功！";


                    }
                    else
                    {
                        data.Tray = TRAYTYPE.料盘A;
                        Program.Repo.Update(data);
                        label4.Text = "初始化成功！";
                    }
                }
                else
                {
                    var data = Program.Repo.GetSingle<Panel>(p => p.RfidID == rfidid);
                    if (data == null)
                    {
                        var t = new Panel
                        {
                            Tray = TRAYTYPE.料盘B,
                            RfidID = rfidid,
                        };
                        Program.Repo.Insert(t);

                        label4.Text = "初始化成功！";
                    }
                    else
                    {
                        data.Tray = TRAYTYPE.料盘B;
                        Program.Repo.Update(data);
                        label4.Text = "初始化成功！";
                    }
                }

            }
        }

    }
}
