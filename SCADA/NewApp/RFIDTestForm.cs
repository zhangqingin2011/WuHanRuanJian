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
    public partial class RFIDTestForm : Form
    {
        AutoSizeFormClass aotosize = new AutoSizeFormClass();
        public RFIDTestForm()
        {
            InitializeComponent();
            InitUI();
        }
        private void InitUI()
        {
            labelip1.Text = LineMainForm.sygolefidcnc11.IP.ToString() + "：" + LineMainForm.sygolefidcnc11.Port.ToString();
            labelip2.Text = LineMainForm.sygolefidcnc12.IP.ToString() + "：" + LineMainForm.sygolefidcnc12.Port.ToString();
            labelip3.Text = LineMainForm.sygolefidcnc21.IP.ToString() + "：" + LineMainForm.sygolefidcnc21.Port.ToString();
            labelip4.Text = LineMainForm.sygolefidcnc22.IP.ToString() + "：" + LineMainForm.sygolefidcnc22.Port.ToString();
            labelip5.Text = LineMainForm.sygolefidcnc31.IP.ToString() + "：" + LineMainForm.sygolefidcnc31.Port.ToString();
            labelip6.Text = LineMainForm.sygolefidcnc32.IP.ToString() + "：" + LineMainForm.sygolefidcnc32.Port.ToString();
            labelip7.Text = LineMainForm.sygolefidcnc41.IP.ToString() + "：" + LineMainForm.sygolefidcnc41.Port.ToString();
            labelip8.Text = LineMainForm.sygolefidcnc42.IP.ToString() + "：" + LineMainForm.sygolefidcnc42.Port.ToString();
            labelip9.Text = LineMainForm.sygolefidclean1.IP.ToString() + "：" + LineMainForm.sygolefidclean1.Port.ToString();
            labelip10.Text = LineMainForm.sygolefidclean2.IP.ToString() + "：" + LineMainForm.sygolefidclean2.Port.ToString();
            //labelip11.Text = LineMainForm.sygolefidcnc11.IP.ToString() + "：" + LineMainForm.sygolefidcnc11.Port.ToString();
            //labelip12.Text = LineMainForm.sygolefidcnc11.IP.ToString() + "：" + LineMainForm.sygolefidcnc11.Port.ToString();
            //labelip13.Text = LineMainForm.sygolefidcnc11.IP.ToString() + "：" + LineMainForm.sygolefidcnc11.Port.ToString();
            //labelip14.Text = LineMainForm.sygolefidcnc11.IP.ToString() + "：" + LineMainForm.sygolefidcnc11.Port.ToString();
            labelip15.Text = LineMainForm.sygolefidfit11.IP.ToString() + "：" + LineMainForm.sygolefidfit11.Port.ToString();
            labelip16.Text = LineMainForm.sygolefidfit12.IP.ToString() + "：" + LineMainForm.sygolefidfit12.Port.ToString();
            labelip17.Text = LineMainForm.sygolefidfit21.IP.ToString() + "：" + LineMainForm.sygolefidfit21.Port.ToString();
            labelip18.Text = LineMainForm.sygolefidfit22.IP.ToString() + "：" + LineMainForm.sygolefidfit22.Port.ToString();
            labelip19.Text = LineMainForm.sygolefidwmsin.IP.ToString() + "：" + LineMainForm.sygolefidwmsin.Port.ToString();
            labelip20.Text = LineMainForm.sygolefidwmsout.IP.ToString() + "：" + LineMainForm.sygolefidwmsout.Port.ToString();

            if(LineMainForm.sygolefidcnc11.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels1.Text = "在线";
            }
            else
            {
                labels1.Text = "离线";
            }

            if (LineMainForm.sygolefidcnc12.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels2.Text = "在线";
            }
            else
            {
                labels2.Text = "离线";
            }

            if (LineMainForm.sygolefidcnc21.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels3.Text = "在线";
            }
            else
            {
                labels3.Text = "离线";
            }
            if (LineMainForm.sygolefidcnc22.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels4.Text = "在线";
            }
            else
            {
                labels4.Text = "离线";
            }
            if (LineMainForm.sygolefidcnc31.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels5.Text = "在线";
            }
            else
            {
                labels5.Text = "离线";
            }
            if (LineMainForm.sygolefidcnc32.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels6.Text = "在线";
            }
            else
            {
                labels6.Text = "离线";
            }
            if (LineMainForm.sygolefidcnc41.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels7.Text = "在线";
            }
            else
            {
                labels7.Text = "离线";
            }
            if (LineMainForm.sygolefidcnc42.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels8.Text = "在线";
            }
            else
            {
                labels8.Text = "离线";
            }
            if (LineMainForm.sygolefidclean1.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels9.Text = "在线";
            }
            else
            {
                labels9.Text = "离线";
            }
            if (LineMainForm.sygolefidclean2.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels10.Text = "在线";
            }
            else
            {
                labels10.Text = "离线";
            }
            if (LineMainForm.sygolefidfit11.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels15.Text = "在线";
            }
            else
            {
                labels15.Text = "离线";
            }
            if (LineMainForm.sygolefidfit12.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels16.Text = "在线";
            }
            else
            {
                labels16.Text = "离线";
            }
            if (LineMainForm.sygolefidfit21.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels17.Text = "在线";
            }
            else
            {
                labels17.Text = "离线";
            }
            if (LineMainForm.sygolefidfit22.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels18.Text = "在线";
            }
            else
            {
                labels18.Text = "离线";
            }
            if (LineMainForm.sygolefidwmsin.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels19.Text = "在线";
            }
            else
            {
                labels19.Text = "离线";
            }
            if (LineMainForm.sygolefidwmsout.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.CONNECTED)
            {
                labels20.Text = "在线";
            }
            else
            {
                labels20.Text = "离线";
            }


        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidcnc11.Connect(LineMainForm.sygolefidcnc11.IP, LineMainForm.sygolefidcnc11.Port))
            {
                labels1.Text = "在线";
            }
            else
            {
                labels1.Text = "离线";
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidcnc12.Connect(LineMainForm.sygolefidcnc12.IP, LineMainForm.sygolefidcnc12.Port))
            {
                labels2.Text = "在线";
            }
            else
            {
                labels2.Text = "离线";
            }

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidcnc21.Connect(LineMainForm.sygolefidcnc21.IP, LineMainForm.sygolefidcnc21.Port))
            {
                labels3.Text = "在线";
            }
            else
            {
                labels3.Text = "离线";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidcnc22.Connect(LineMainForm.sygolefidcnc22.IP, LineMainForm.sygolefidcnc22.Port))
            {
                labels4.Text = "在线";
            }
            else
            {
                labels4.Text = "离线";
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidcnc31.Connect(LineMainForm.sygolefidcnc31.IP, LineMainForm.sygolefidcnc31.Port))
            {
                labels5.Text = "在线";
            }
            else
            {
                labels5.Text = "离线";
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidcnc32.Connect(LineMainForm.sygolefidcnc32.IP, LineMainForm.sygolefidcnc32.Port))
            {
                labels6.Text = "在线";
            }
            else
            {
                labels6.Text = "离线";
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidcnc41.Connect(LineMainForm.sygolefidcnc41.IP, LineMainForm.sygolefidcnc41.Port))
            {
                labels7.Text = "在线";
            }
            else
            {
                labels7.Text = "离线";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidcnc42.Connect(LineMainForm.sygolefidcnc42.IP, LineMainForm.sygolefidcnc42.Port))
            {
                labels8.Text = "在线";
            }
            else
            {
                labels8.Text = "离线";
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
             if (LineMainForm.sygolefidclean1.Connect(LineMainForm.sygolefidclean1.IP, LineMainForm.sygolefidclean1.Port))
            {
                labels9.Text = "在线";
            }
            else
            {
                labels9.Text = "离线";
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidclean2.Connect(LineMainForm.sygolefidclean2.IP, LineMainForm.sygolefidclean2.Port))
            {
                labels10.Text = "在线";
            }
            else
            {
                labels10.Text = "离线";
            }
        }
    

        private void button11_Click(object sender, EventArgs e)
        {

        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {

        }

        private void button15_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidfit11.Connect(LineMainForm.sygolefidfit11.IP, LineMainForm.sygolefidfit11.Port))
            {
                labels15.Text = "在线";
            }
            else
            {
                labels15.Text = "离线";
            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidfit12.Connect(LineMainForm.sygolefidfit12.IP, LineMainForm.sygolefidfit12.Port))
            {
                labels16.Text = "在线";
            }
            else
            {
                labels16.Text = "离线";
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidfit21.Connect(LineMainForm.sygolefidfit21.IP, LineMainForm.sygolefidfit21.Port))
            {
                labels17.Text = "在线";
            }
            else
            {
                labels17.Text = "离线";
            }

        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidfit22.Connect(LineMainForm.sygolefidfit22.IP, LineMainForm.sygolefidfit22.Port))
            {
                labels18.Text = "在线";
            }
            else
            {
                labels18.Text = "离线";
            }
        }

        private void button19_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidwmsin.Connect(LineMainForm.sygolefidwmsin.IP, LineMainForm.sygolefidwmsin.Port))
            {
                labels19.Text = "在线";
            }
            else
            {
                labels19.Text = "离线";
            }
        }

        private void button20_Click(object sender, EventArgs e)
        {
            if (LineMainForm.sygolefidwmsout.Connect(LineMainForm.sygolefidwmsout.IP, LineMainForm.sygolefidwmsout.Port))
            {
                labels20.Text = "在线";
            }
            else
            {
                labels20.Text = "离线";
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            InitUI();
        }

        private void RFIDTestForm_SizeChanged(object sender, EventArgs e)
        {

           // aotosize.controlAutoSize(this);
        }
    }
}
