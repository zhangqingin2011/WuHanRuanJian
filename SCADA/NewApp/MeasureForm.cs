using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCADA.NewApp
{

    public partial class MeasureForm : Form//: LineDetectBase<ZYLY>

    {
        private DataTable dtResult = new DataTable();
      
        
        static public int MeasureOrderNO = 0;
        static public int MeasurePicecNo = 0;
        static public string MeasurePath = @"\\192.168.8.31\celiangjieguo\JIEGUO.txt";
        //static public string MeasurePath = @"JIEGUO.txt";
        string measureIP = "192.168.8.31";

        static public object readder = new object();
        public MeasureForm()
        {
            InitializeComponent();
        }

        private void MeasureForm_Load(object sender, EventArgs e)
        {
            CheckConnect();
        
            textBoxOrderNO1.Text = "0";
            textBoxPNO1.Text = "0";
        }

        private void btnFresh_Click(object sender, EventArgs e)
        {
            richTextBox2.InvokeEx(c =>
            {
                c.Text = ReadMeasureDataAll();
            });
          //  UpdateCoordMeterReasult(OrderExcuteForm.workOrderTasks[0], 1);
        }

        private string[] ReadMeasureDataLine()
        {
            if (!LineMainForm.PingTest(measureIP) || !File.Exists(MeasurePath))
                return null;

            string[] content;
            lock (readder)
            {
                content = File.ReadAllLines(MeasurePath);
            }
            return content;
        }

        private string ReadMeasureDataAll()
        {
            if (!LineMainForm.PingTest(measureIP) || !File.Exists(MeasurePath))
            {
                PushMessage("测量报告文件不存在！", Color.Red);
                return "";
            }

            string content;
            lock (readder)
            {
                content = File.ReadAllText(MeasurePath, System.Text.Encoding.Default);
                //byte[] byteArray = System.Text.Encoding.BigEndianUnicode.GetBytes(content);
                //content = System.Text.Encoding.UTF32.GetString(byteArray);
            }
            return content;
        }

        private void CheckConnect()
        {
            //打开COM口
            if (!LineMainForm.gauge1.IsOpen)//串口开启状态
            {
                if(LineMainForm.gauge1.Open())//串口打开成功
                {
                    buttonclose1.Enabled = true;
                    buttonclose1.BackColor = Color.DarkTurquoise;
                    buttonopen1.Enabled = false;
                    buttonopen1.BackColor = Color.Gray;
                    PushMessage(richTextBox1, "与测量仪1通讯成功！", Color.Black);
                }
                else{
                    buttonclose2.Enabled = false;
                    buttonclose2.BackColor = Color.DarkTurquoise;
                    buttonopen2.Enabled = false;
                    buttonopen2.BackColor = Color.Gray;
                    PushMessage(richTextBox1, "与测量仪1通讯失败！", Color.Black);
                }



            }
            if (!LineMainForm.gauge2.IsOpen)//串口开启状态
            {
                if (LineMainForm.gauge2.Open())//串口打开成功
                {
                    buttonclose2.Enabled = true;
                    buttonclose2.BackColor = Color.DarkTurquoise;
                    buttonopen2.Enabled = false;
                    buttonopen2.BackColor = Color.Gray;
                    PushMessage(richTextBox2, "与测量仪2通讯成功！", Color.Black);
                }
                else
                {
                    buttonclose2.Enabled = false;
                    buttonclose2.BackColor = Color.DarkTurquoise;
                    buttonopen2.Enabled = false;
                    buttonopen2.BackColor = Color.Gray;
                    PushMessage(richTextBox2, "与测量仪2通讯失败！", Color.Black);
                }
            }
     
        }

        private void PushMessage(string Msg, Color color)
        {
            richTextBox2.InvokeEx(c =>
            {
                string Message = Msg + "     " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
                richTextBox2.SelectionColor = color;
                richTextBox2.AppendText(Message);
            });
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            CheckConnect();
            bool ReFrichTextBox2 = false;
            if (textBoxOrderNO1.Text != MeasureOrderNO.ToString())
            {
                textBoxOrderNO1.Text = MeasureOrderNO.ToString();
                ReFrichTextBox2 = true;
            }
            if (textBoxPNO1.Text != MeasurePicecNo.ToString())
            {
                textBoxPNO1.Text = MeasurePicecNo.ToString();
                ReFrichTextBox2 = true;
            }
            if (ReFrichTextBox2)
            {
                richTextBox2.InvokeEx(c =>
                {
                    c.Text = ReadMeasureDataAll();
                });
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rich = sender as RichTextBox;
            rich.SelectionStart = rich.Text.Length;
            rich.ScrollToCaret();
        }

        private string[] readeLinetostring(string text)
        {
            List<string> ListS = new List<string>();
            
            int index = 0;
            string temp = text.Trim();

            while (temp.Length > 0)
            {
                int i = temp.IndexOf(" ");
                if (i == -1)
                {
                    ListS.Add(temp);
                    temp = "";
                }
                else
                {
                    ListS.Add(temp.Substring(0, i));
                    temp = temp.Substring(i, temp.Length - i);
                    temp =  temp.Trim();
                }
            }
            return ListS.ToArray();
        }
        private bool UpdateCoordMeterReasult(WorkOrderData aorder, int index)
        {
            bool certificate_all = true;
            if (index < 1 || index > 4)
            {
                return false;
            }
            string mode = "";

            switch (index)
            {
                case 1:
                    mode = aorder.Product1.ToString();
                    break;
                case 2:
                    mode = aorder.Product2.ToString();
                    break;
                case 3:
                    mode = aorder.Product3.ToString();
                    break;
                case 4:
                    mode = aorder.Product4.ToString();
                    break;
            }
            if (mode == "无")
            {
                switch (index)
                {
                    case 1:
                        aorder.P1Quality = PIECEQUALITY.异常;
                        break;
                    case 2:
                        aorder.P2Quality = PIECEQUALITY.异常;
                        break;
                    case 3:
                        aorder.P3Quality = PIECEQUALITY.异常;
                        break;
                    case 4:
                        aorder.P4Quality = PIECEQUALITY.异常;
                        break;
                }
                Program.Repo.Update(aorder);
                return false;
            }


            var modelisttemp = Program.Repo.Get<MeterMode>(p => p.ModeSN == mode);
            string path = "C:\\Users\\Public\\MESFile\\MeterResult\\JIEGUO.TXT";//@"\\192.168.8.31\celiangjieguo\JIEGUO.txt";
            //FileStream stream = File.OpenRead(path);
            //var doc = new XmlDocument();
            //doc.Load(stream);
            //XmlNode node = doc.SelectSingleNode("/root/Results/values");
            string[] content;
            lock (readder)
            {
                content = File.ReadAllLines(path, System.Text.Encoding.Default);
                //byte[] byteArray = System.Text.Encoding.BigEndianUnicode.GetBytes(content);
                //content = System.Text.Encoding.UTF32.GetString(byteArray);
            }
            if (content == null)
            {
                MessageBox.Show("测量报告读取错误！");

                switch (index)
                {
                    case 1:
                        aorder.P1Quality = PIECEQUALITY.异常;
                        break;
                    case 2:
                        aorder.P2Quality = PIECEQUALITY.异常;
                        break;
                    case 3:
                        aorder.P3Quality = PIECEQUALITY.异常;
                        break;
                    case 4:
                        aorder.P4Quality = PIECEQUALITY.异常;
                        break;
                }

                Program.Repo.Update(aorder);
                return false;
            }

            if (modelisttemp.Count < 1)
            {
                MessageBox.Show("无测量模板数据！");

                switch (index)
                {
                    case 1:
                        aorder.P1Quality = PIECEQUALITY.异常;
                        break;
                    case 2:
                        aorder.P2Quality = PIECEQUALITY.异常;
                        break;
                    case 3:
                        aorder.P3Quality = PIECEQUALITY.异常;
                        break;
                    case 4:
                        aorder.P4Quality = PIECEQUALITY.异常;
                        break;
                }
                Program.Repo.Update(aorder);
                return false;
            }
            if (content.Count() < 6)
            {
                MessageBox.Show("无测量模板数据！");

                switch (index)
                {
                    case 1:
                        aorder.P1Quality = PIECEQUALITY.异常;
                        break;
                    case 2:
                        aorder.P2Quality = PIECEQUALITY.异常;
                        break;
                    case 3:
                        aorder.P3Quality = PIECEQUALITY.异常;
                        break;
                    case 4:
                        aorder.P4Quality = PIECEQUALITY.异常;
                        break;
                }
                Program.Repo.Update(aorder);
                return false;
            }

            int recordrank = (content.Count() - 4) / 2;

            for (int i = 0; i < recordrank; i++)
            {
                int rank = i + 1;
                var modetemp = Program.Repo.GetSingle<MeterMode>(p => p.ModeSN == mode && p.Rank == rank);

                string rankstring = content[4 + i * 2];
                var arriitem = readeLinetostring(rankstring);

                string values = arriitem[2].ToString();
                double value = Convert.ToDouble(values);
                if (Program.Repo.Exist<MeterResult>(p => p.OrderId == aorder.Id && p.Rank == rank && p.ModeSN == mode && p.WorkpieceNumber == index))
                {
                    var data = Program.Repo.GetSingle<MeterResult>(p => p.OrderId == aorder.Id && p.Rank == rank && p.ModeSN == mode && p.WorkpieceNumber == index);
                    data.SizeValue = value;
                  
                    if (value < modetemp.SizeDataStandard + modetemp.SizeDataMin || value > modetemp.SizeDataStandard + modetemp.SizeDataMax)
                    {
                        data.IsCertificate = false;
                        certificate_all = false;
                    }
                    else data.IsCertificate = true;
                    Program.Repo.Update(data);
                }
                else
                {
                    bool IsCertificate = false;
                    if (value < modetemp.SizeDataStandard + modetemp.SizeDataMin || value > modetemp.SizeDataStandard + modetemp.SizeDataMax)
                    {
                        IsCertificate = false;

                        certificate_all = false;
                    }
                    else IsCertificate = true;
                    //插入一条检测记录
                    var data = new MeterResult
                    {
                        OrderId = aorder.Id,
                        WorkpieceNumber = index,
                        ModeSN = mode,
                        NUM = modetemp.NUM,
                        Rank = rank,
                        IsCertificate = IsCertificate,
                        SizeDataMin = modetemp.SizeDataMin,
                        SizeDataMax = modetemp.SizeDataMax,
                        SizeDataStandard = modetemp.SizeDataStandard,
                        SizeValue = value,
                    };

                    Program.Repo.Insert(data);
                }
            }


            if (certificate_all == false)
            {
                switch (index)
                {
                    case 1:
                        aorder.P1Quality = PIECEQUALITY.不合格;
                        break;
                    case 2:
                        aorder.P2Quality = PIECEQUALITY.不合格;
                        break;
                    case 3:
                        aorder.P3Quality = PIECEQUALITY.不合格;
                        break;
                    case 4:
                        aorder.P4Quality = PIECEQUALITY.不合格;
                        break;
                }
            }
            else
            {
                switch (index)
                {
                    case 1:
                        aorder.P1Quality = PIECEQUALITY.合格;
                        break;
                    case 2:
                        aorder.P2Quality = PIECEQUALITY.合格;
                        break;
                    case 3:
                        aorder.P3Quality = PIECEQUALITY.合格;
                        break;
                    case 4:
                        aorder.P4Quality = PIECEQUALITY.合格;
                        break;
                }
            }
            Program.Repo.Update(aorder);
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged_1(object sender, EventArgs e)
        {
            RichTextBox rich = sender as RichTextBox;
            rich.SelectionStart = rich.Text.Length;
            rich.ScrollToCaret();
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rich = sender as RichTextBox;
            rich.SelectionStart = rich.Text.Length;
            rich.ScrollToCaret();
        }
        private void PushMessage(RichTextBox richtext,string Msg, Color color)
        {
            richtext.InvokeEx(c =>
            {
                string Message = Msg + "     " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
                richtext.SelectionColor = color;
                richtext.AppendText(Message);
            });
        }

        private void buttonclose1_Click(object sender, EventArgs e)
        {
            if(LineMainForm.gauge1.IsOpen)
            {
                LineMainForm.gauge1.Close();
                buttonclose1.Enabled = false;
                buttonclose1.BackColor = Color.Gray;
                buttonopen1.Enabled = true;
                buttonopen1.BackColor = Color.DarkTurquoise;
                PushMessage(richTextBox1, "串口关闭成功！", Color.Black);
            }
            else
            {
                PushMessage(richTextBox1, "串口关闭失败！", Color.Red);
            }

        }

        private void buttonclose2_Click(object sender, EventArgs e)
        {
            if (LineMainForm.gauge2.IsOpen)
            {
                LineMainForm.gauge2.Close();
                buttonclose2.Enabled = false;
                buttonclose2.BackColor = Color.Gray;
                buttonopen2.Enabled = true;
                buttonopen2.BackColor = Color.DarkTurquoise;
                PushMessage(richTextBox2, "串口关闭成功！", Color.Black);
            }
            else
            {
                PushMessage(richTextBox2, "串口关闭失败！", Color.Red);
            }
        }

        private void buttonopen1_Click(object sender, EventArgs e)
        {
            if (!LineMainForm.gauge1.IsOpen)
            {
              if(LineMainForm.gauge1.Open())
                {
                    buttonclose1.Enabled = false;
                    buttonclose1.BackColor = Color.DarkTurquoise;
                    buttonopen1.Enabled = false;
                    buttonopen1.BackColor = Color.Gray;
                    PushMessage(richTextBox1, "串口开启成功！", Color.Black);
                }
                else
                {
                    PushMessage(richTextBox1, "串口开启失败！", Color.Red);
                }
               
            }
        }

        private void buttonopen2_Click(object sender, EventArgs e)
        {
            if (!LineMainForm.gauge2.IsOpen)
            {
                if (LineMainForm.gauge1.Open())
                  {
                    buttonclose2.Enabled = false;
                    buttonclose2.BackColor = Color.DarkTurquoise;
                    buttonopen2.Enabled = false;
                    buttonopen2.BackColor = Color.Gray;
                    PushMessage(richTextBox2, "串口开启成功！", Color.Black);
                }
                else
                {
                    PushMessage(richTextBox2, "串口开启失败！", Color.Red);
                }

            }

        }

        private async void buttonset1_Click(object sender, EventArgs e)
        {
            (sender as Button).Enabled = false;
        //    await LineMainForm.gauge1.SendProofCmd(2);
            (sender as Button).Enabled = true;
        }
        //PushMessage("任务正在执行，无法手动入库！", Color.Red);
    }
}
