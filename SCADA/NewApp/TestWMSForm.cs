using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SCADA.SimensPLC;
using System.Threading;

namespace SCADA.NewApp
{
    public partial class TestWMSForm : Form
    {
        public TestWMSForm()
        {
            InitializeComponent();
            Task.Run(()=>AutoToCheck(textBox1.Text, int.Parse(textBox2.Text)));
        }

        WMSPLC mSPLC = new WMSPLC();

        void AutoToCheck(string ip, int port)
        {
            while (true)
            {
                Thread.Sleep(800);
                if (!LineMainForm.PingTest(ip))
                    continue;

                if (!mSPLC.GetOnlineState())
                {
                    mSPLC.Disconnect();
                    mSPLC = new WMSPLC();
                    mSPLC.Connecting(ip, port);
                }
            }
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            if (mSPLC.GetOnlineState())
            {
                label3.Text = "在线";
                label3.BackColor = Color.DarkTurquoise;



                if (InComplete())
                {
                    label18.Text = "入库完成";
                }
                else
                {
                    label18.Text = "";
                }
            }
            else
            {
                label3.Text = "离线";
                label3.BackColor = Color.LightCoral;
            }

            if (AGVToDDJ())
            {
                label16.Text = "已到堆垛机";
            }
            else
            {
                label16.Text = "";
            }

            if (AGVToStation())
            {
                label17.Text = "已到中转台";
            }
            else
            {
                label17.Text = "";
            }

            OUTAction();
            INAction();
        }

        private bool AGVToDDJ()
        {
            bool res = false;

            return res;
        }

        private bool AGVToStation()
        {
            bool res = false;

            return res;
        }

        private void MachineReset()
        {

        }

        private void SetMode(int mode)
        {

        }

        private bool GetResetComplete()
        {
            bool res = false;
            bool result = false;

            return result;
        }

        private void OUTAction()
        {
            if (outflag)
            {
                if (outstate == OUTSTATE.结束)
                {
                    outstate = OUTSTATE.堆垛机复位中;
                    SetMode(1);
                    MachineReset();
                    label12.Text = outstate.ToString();
                }
                else if (outstate == OUTSTATE.堆垛机复位中)
                {
                    if(GetResetComplete())
                        outstate = OUTSTATE.堆垛机复位完成;
                    label12.Text = outstate.ToString();
                }
                else if (outstate == OUTSTATE.堆垛机复位完成)
                {
                    outstate = OUTSTATE.出库指令发送;
                    SendCommand(int.Parse(textBox4.Text));
                    label12.Text = outstate.ToString();
                }
                else if (outstate == OUTSTATE.出库指令发送)
                {
                    outstate = OUTSTATE.结束;
                    label12.Text = outstate.ToString();
                    outflag = false;
                    button7.Text = "出库";
                    button7.Enabled = true;
                }
            }
        }

        private void SendCommand(int no)
        {

        }

        private void INAction()
        {
            if (inflag)
            {
                if (instate == INSTATE.结束)
                {
                    instate = INSTATE.堆垛机复位中;
                    SetMode(2);
                    MachineReset();
                    label14.Text = instate.ToString();
                }
                else if (instate == INSTATE.堆垛机复位中)
                {
                    if(GetResetComplete())
                        instate = INSTATE.堆垛机复位完成;
                    label14.Text = instate.ToString();
                }
                else if (instate == INSTATE.堆垛机复位完成)
                {
                    instate = INSTATE.入库指令发送;
                    SendCommand(int.Parse(textBox5.Text));
                    label14.Text = instate.ToString();
                }
                else if (instate == INSTATE.入库指令发送)
                {
                    if(InComplete())
                        instate = INSTATE.入库完成;
                    label14.Text = instate.ToString();
                }
                else if (instate == INSTATE.入库完成)
                {
                    instate = INSTATE.结束;
                    label14.Text = instate.ToString();
                    inflag = false;
                    button8.Text = "入库";
                    button8.Enabled = true;
                }
            }
        }

        private bool InComplete()
        {
            bool result = false;

            return result;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            /*int result;
            if (!int.TryParse(textBox3.Text, out result))
            {
                MessageBox.Show("仓位号不正确");
                return;
            }

            if (mSPLC.GetOnlineState())
            {
                int value;
                bool res = mSPLC.ReadsingleRegister((int)WMSPLC.REGINDEX.启动停止控制信号, out value);
                if (res)
                {
                    int value2 = WMSPLC.SetBoolValue(value, 8);
                    mSPLC.WritesingleRegister((int)WMSPLC.REGINDEX.启动停止控制信号, value2);
                    mSPLC.WritesingleRegister((int)WMSPLC.REGINDEX.仓位号, result);
                }
            }*/
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            /*if (mSPLC.GetOnlineState())
            {
                int value;
                bool res = mSPLC.ReadsingleRegister((int)WMSPLC.REGINDEX.启动停止控制信号, out value);
                if (res)
                {
                    int value2 = WMSPLC.SetBoolValue(value, 9);
                    mSPLC.WritesingleRegister((int)WMSPLC.REGINDEX.启动停止控制信号, value2);
                }
            }*/
        }

        private void Button3_Click(object sender, EventArgs e)
        {

        }

        private void Button4_Click(object sender, EventArgs e)
        {

        }

        private void Button5_Click(object sender, EventArgs e)
        {

        }

        private void Button6_Click(object sender, EventArgs e)
        {

        }

        private void Button2_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void Button2_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void Button3_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void Button3_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void Button1_MouseDown(object sender, MouseEventArgs e)
        {

        }

        private void Button1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private enum OUTSTATE
        {
            堆垛机复位中,
            堆垛机复位完成,
            出库指令发送,
            结束
        }

        private enum INSTATE
        {
            堆垛机复位中,
            堆垛机复位完成,
            入库指令发送,
            入库完成,
            结束
        }

        OUTSTATE outstate = OUTSTATE.结束;
        INSTATE instate = INSTATE.结束;
        bool outflag = false;
        bool inflag = false;

        private void Button7_Click(object sender, EventArgs e)
        {
            if (!mSPLC.GetOnlineState())
            {
                MessageBox.Show("料仓PLC离线！");
                return;
            }

            int result;
            if (!int.TryParse(textBox4.Text, out result))
            {
                MessageBox.Show("仓位号不正确");
                return;
            }

            outflag = true;
            button7.Text = "出库中";
            button7.Enabled = false;
        }

        private void Button8_Click(object sender, EventArgs e)
        {
            if (!mSPLC.GetOnlineState())
            {
                MessageBox.Show("料仓PLC离线！");
                return;
            }

            int result;
            if (!int.TryParse(textBox5.Text, out result))
            {
                MessageBox.Show("仓位号不正确");
                return;
            }
            inflag = true;
            button8.Text = "入库中";
            button8.Enabled = false;
        }

        private void Button9_Click(object sender, EventArgs e)
        {

        }

        private void Button10_Click(object sender, EventArgs e)
        {

        }

        private void Button11_Click(object sender, EventArgs e)
        {

        }

        private void Button12_Click(object sender, EventArgs e)
        {

        }

        private void Button13_Click(object sender, EventArgs e)
        {

        }
    }
}
