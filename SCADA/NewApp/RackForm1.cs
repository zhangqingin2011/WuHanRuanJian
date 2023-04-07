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
    public partial class RackForm : Form
    {
        public RackForm()
        {
            InitializeComponent();
            InitConfig();
            //Task.Run(() => AutoDoWork());
            //autoworker.DoWork += Autoworker_DoWork;
            //autoworker.RunWorkerAsync();
        }

        BackgroundWorker autoworker = new BackgroundWorker();
        public static List<RackUnitData> listRackData = new List<RackUnitData>();
        private List<DefUnit> unitList = new List<DefUnit>();
        string[] trayTypes;
        string[] pieceTypes;
        public delegate void RackAction(object sender, bool flag, int rackno);
        public static RackAction rackAction;
        public static bool rukuflag = false;
        public static bool chukuflag = false;
        public static bool rukumanu = false;
        public static bool chukumanu = false;
        public static MANUALIN rukustatus = MANUALIN.结束;
        public static MANUALOUT chukustatus = MANUALOUT.结束;

        public static int[] RackNoCommand = { 101, 201, 301, 401, 501, 601, 701,
                                102, 202, 302, 402, 502, 602, 702,
                                103, 203, 303, 403, 503, 603, 703,
                                104, 204, 304, 404, 504, 604, 704,
                                105, 205, 305, 405, 505, 605, 705 };

        /// <summary>
        /// 手动出入库消息缓存
        /// </summary>
        public static RackUnitData unitDatatemp = null;

        public enum MANUALIN
        {
            检查料库状态,
            发送入库命令,
            入库中,
            RFID检测中,
            RFID检测完成,
            入库完成,
            入库异常,
            结束
        }

        public enum MANUALOUT
        {
            检查料库状态,
            发送出库命令,
            出库中,
            RFID检测中,
            RFID检测完成,
            出库完成,
            出库异常,
            结束
        }
        private void OnRackAction(bool flag, int rackno)
        {
            if (rackAction != null)
            {
                rackAction(this, flag, rackno);
            }
        }

        private void InitConfig()
        {
            InitRackUI();
            InitInOutActionUI();
            InitManualUI();
            InitMessageUI();
            rackAction += ShowRackChanged;
            OrderExcuteForm.rackAction1 += ShowRack1Changed;
            OrderExcuteForm.rackAction2 += ShowRack2Changed;
            OrderExcuteForm.rackAction3 += ShowRack3Changed;
        }

        void ShowRack1Changed(object sender, bool flag, int rackno)
        {

            PushMessage(string.Format("{0}更新仓位{1}的信息", flag ? "工单出库" : "工单入库", rackno), Color.Black);
            int index = rackno - 1;
            var rackdata = listRackData.Find(s => s.NO == rackno);
            if (rackdata != null)
            {
                unitList[index].ShowContent(rackdata);
                Program.Repo.Update<RackUnitData>(rackdata);
            }


        }
        void ShowRack3Changed(object sender, bool flag, int rackno)
        {

            PushMessage(string.Format("{0}更新仓位{1}的信息", flag ? "工单出库异常" : "工单入库异常", rackno), Color.Black);
            //int index = rackno - 1;
            //var rackdata = listRackData.Find(s => s.NO == rackno);
            //if (rackdata != null)
            //{
            //    unitList[index].ShowContent(rackdata);
            //    Program.Repo.Update<RackUnitData>(rackdata);
            //}


        }

        void ShowRack2Changed(object sender, bool flag, int rackno)
        {
            PushMessage(string.Format("{0}更新仓位{1}的信息", flag ? "工单出库" : "工单入库", rackno), Color.Black);
            int index = rackno - 1;
            var rackdata = listRackData.Find(s => s.NO == rackno);
            if (rackdata != null)
            {
                unitList[index].ShowContent(rackdata);
                Program.Repo.Update<RackUnitData>(rackdata);
            }
        }



        /*private void Autoworker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                Console.WriteLine("-------------");
                InAction();
                OutAction();
            }
        }*/

        void ShowRackChanged(object sender, bool flag, int rackno)
        {
            PushMessage(string.Format("{0}更新仓位{1}的信息", flag ? "手动操作" : "手动初始化", rackno), Color.Black);
            int index = rackno - 1;
            var rackdata = listRackData.Find(s => s.NO == rackno);
            if (rackdata != null)
            {
                unitList[index].ShowContent(rackdata);
                Program.Repo.Update<RackUnitData>(rackdata);
            }
        }

        private void InitRackUI()
        {
            int number = 0;
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    number++;
                    var rackdata = listRackData.Find(s => s.NO == number);
                    DefUnit defUnit = new DefUnit();
                    defUnit.Dock = DockStyle.Fill;
                    defUnit.ShowContent(rackdata);
                    tableLayoutPanel5.Controls.Add(defUnit, j, 4 - i);
                    unitList.Add(defUnit);
                }
            }
        }

        private void InitInOutActionUI()
        {
            for (int i = 1; i <= 35; i++)
            {
                comboBox1.Items.Add(string.Format("NO{0}", i));
            }
            comboBox1.SelectedIndex = 0;
            TRAYTYPE e0 = new TRAYTYPE();
            trayTypes = Enum.GetNames(e0.GetType());
            comboBox2.Items.AddRange(trayTypes);
            comboBox2.SelectedIndexChanged += ComboBox2_SelectedIndexChanged;
            comboBox2.SelectedIndex = 0;
            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
            button3.Click += Button3_Click;
            button4.Click += Button4_Click;
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            chukuflag = false;
            chukumanu = false;
            chukustatus = MANUALOUT.结束;
            button3.Enabled = true;
            button3.Text = "手动出库";
            button4.Enabled = false;
            if (unitDatatemp != null)
            {
                PushMessage(string.Format("仓位{0}取消手动出库", unitDatatemp.NO), Color.Black);
                var rackdata = listRackData.Find(s => s.NO == unitDatatemp.NO);
                rackdata.Lock = false;
                unitDatatemp = null;
                RackForm.chukumanu = false;
            }
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (rukuflag)
            {
                //MessageBox.Show("正在出库中！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                PushMessage("正在入库中，无法手动出库！", Color.Red);
                return;
            }
            if (OrderExcuteForm.positionBind.wMSPositionBind != 0)
            {
                PushMessage("当前有自动出入库任务，无法手动出库！", Color.Red);
                return;
            }

            var rackdata = listRackData.Find(s => s.Lock == true);
            //if (rackdata != null)
            //{
            //    PushMessage("料库执行自动出入库任务，无法手动出库！", Color.Red);
            //    return;
            //}

            int no = comboBox1.SelectedIndex + 1;
            rackdata = listRackData.Find(s => s.NO == no);
            if (rackdata == null)
            {
                PushMessage("出库错误", Color.Red);
                return;
            }

            if (rackdata.Tray == TRAYTYPE.空)
            {
                PushMessage(string.Format("仓位{0}为空，无法手动出库！", rackdata.NO), Color.Red);
                return;
            }

            //if (no % 7 == 0)
            //{
            //    PushMessage(string.Format("料库硬件不支持第7列，无法出库！", no), Color.Red);
            //    return;
            //}

            if (!LineMainForm.wmsplc.GetOnlineState())
            {
                PushMessage(string.Format("料库PLC离线，无法出库！", no), Color.Red);
                return;
            }

            bool bvalue1;
            bool bvalue2;
            bool bvalue3;
            bool res1 = LineMainForm.wmsplc.ReadSingleCoil(57, out bvalue1);
            bool res2 = LineMainForm.wmsplc.ReadSingleCoil(50, out bvalue2);
            bool res3 = LineMainForm.wmsplc.ReadSingleCoil(201, out bvalue3);
            int ivalue = 0;
            bool res4 = LineMainForm.wmsplc.ReadsingleRegister(0,out ivalue);
            if (res1 && res2 && res3 && res4)
            {
                if (!bvalue1)
                {
                    PushMessage(string.Format("料库机械臂不在原点，无法出库！", no), Color.Red);
                    return;
                }

                if (!bvalue3)
                {
                    PushMessage(string.Format("料库电机不能自动运行，无法出库！", no), Color.Red);
                    return;
                }

                if (bvalue2)
                {
                    PushMessage(string.Format("料库机械臂忙，无法出库！", no), Color.Red);
                    return;
                }
                if (ivalue != 0)
                {
                    PushMessage(string.Format("料库报警，请处报警并复位料库！", no), Color.Red);
                    return;
                }
            }
            else
            {
                PushMessage(string.Format("与料库PLC通讯出错！", no), Color.Red);
                return;
            }

            if (button3.Text == "手动出库")
            {
                chukuflag = true;
                chukumanu = true;
                button3.Text = "出库中";
                button3.Enabled = false;
                button4.Enabled = true;
                PushMessage(string.Format("仓位{0}手动出库", rackdata.NO), Color.Black);
                rackdata.Lock = true;
                unitDatatemp = new RackUnitData();
                unitDatatemp.NO = rackdata.NO;
                unitDatatemp.Tray = TRAYTYPE.空;
                unitDatatemp.TrayVolume = 0;
                unitDatatemp.Piece1 = PIECETYTPE.无;
                unitDatatemp.Piece1Quality = PIECEQUALITY.待检测;
                unitDatatemp.Piece2 = PIECETYTPE.无;
                unitDatatemp.Piece2Quality = PIECEQUALITY.待检测;
                unitDatatemp.Piece3 = PIECETYTPE.无;
                unitDatatemp.Piece3Quality = PIECEQUALITY.待检测;
                unitDatatemp.Piece4 = PIECETYTPE.无;
                unitDatatemp.Piece4Quality = PIECEQUALITY.待检测;
                unitDatatemp.Lock = rackdata.Lock;
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            rukuflag = false;

            rukumanu = false;
            Console.WriteLine(rukuflag);
            rukustatus = MANUALIN.结束;
            button1.Enabled = true;
            button1.Text = "手动入库";
            button2.Enabled = false;
            if (unitDatatemp != null)
            {
                PushMessage(string.Format("仓位{0}取消手动入库", unitDatatemp.NO), Color.Black);
                var rackdata = listRackData.Find(s => s.NO == unitDatatemp.NO);
                rackdata.Lock = false;
                unitDatatemp = null;
                RackForm.rukumanu = false;
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (chukuflag)
            {
                //MessageBox.Show("正在出库中！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                PushMessage("正在出库中，无法手动入库！", Color.Red);
                return;
            }
            if (OrderExcuteForm.positionBind.wMSPositionBind != 0)
            {
                PushMessage("当前有自动出入库任务，无法手动入库！", Color.Red);
                return;
            }
            if (comboBox2.SelectedIndex == (int)TRAYTYPE.空)
            {
                PushMessage("无法手动入库空料盘！", Color.Red);
                return;
            }

            var rackdata = listRackData.Find(s => s.Lock == true);
            if (rackdata != null)
            {
                PushMessage("料库执行自动出入库任务，无法手动入库！", Color.Red);
                return;
            }
         
            int no = comboBox1.SelectedIndex + 1;
            rackdata = listRackData.Find(s => s.NO == no);
            if (rackdata.Tray != TRAYTYPE.空)
            {
                PushMessage(string.Format("仓位{0}有料，无法入库！", no), Color.Red);
                return;
            }

          

            if (!LineMainForm.wmsplc.GetOnlineState())
            {
                PushMessage(string.Format("料库PLC离线，无法入库！", no), Color.Red);
                return;
            }

            bool bvalue1;
            bool bvalue2;
            bool bvalue3;
            bool res1 = LineMainForm.wmsplc.ReadSingleCoil(57, out bvalue1);
            bool res2 = LineMainForm.wmsplc.ReadSingleCoil(50, out bvalue2);
            bool res3 = LineMainForm.wmsplc.ReadSingleCoil(201, out bvalue3);
            int ivalue = 0;
            bool res4 = LineMainForm.wmsplc.ReadsingleRegister(0,out ivalue);
            if (res1 && res2 && res3 && res4)
            {
                if (!bvalue1)
                {
                    PushMessage(string.Format("料库机械臂不在原点，无法入库！", no), Color.Red);
                    return;
                }

                if (!bvalue3)
                {
                    PushMessage(string.Format("料库电机不能自动运行，无法入库！", no), Color.Red);
                    return;
                }

                if (bvalue2)
                {
                    PushMessage(string.Format("料库机械臂忙，无法入库！", no), Color.Red);
                    return;
                }
                if (ivalue != 0)
                {
                    PushMessage(string.Format("料库报警，请处报警并复位料库！", no), Color.Red);
                    return;
                }
            }
            else
            {
                PushMessage(string.Format("与料库PLC通讯出错！", no), Color.Red);
                return;
            }

            if (button1.Text == "手动入库")
            {
                button1.Text = "入库中";
                rukuflag = true;
                RackForm.rukumanu = true;

                button1.Enabled = false;
                button2.Enabled = true;
                PushMessage(string.Format("开始手动入库到仓位{0}.", no), Color.Black);
                rackdata.Lock = true;
                unitDatatemp = new RackUnitData();
                unitDatatemp.NO = rackdata.NO;
                unitDatatemp.Tray = (TRAYTYPE)Enum.Parse(typeof(TRAYTYPE), comboBox2.Text);
                unitDatatemp.TrayVolume = (comboBox2.Text == TRAYTYPE.叶轮料盘.ToString()) ? 1 : 4;
                if(comboBox2.Text == TRAYTYPE.叶轮料盘.ToString())
                {
                    unitDatatemp.Piece1 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), comboBox3.Text);
                    unitDatatemp.Piece1Quality = PIECEQUALITY.待检测;
                    unitDatatemp.Piece2 = PIECETYTPE.无;
                    unitDatatemp.Piece2Quality = PIECEQUALITY.待检测;
                    unitDatatemp.Piece3 = PIECETYTPE.无;
                    unitDatatemp.Piece3Quality = PIECEQUALITY.待检测;
                    unitDatatemp.Piece4 = PIECETYTPE.无;
                    unitDatatemp.Piece4Quality = PIECEQUALITY.待检测;
                    unitDatatemp.Lock = rackdata.Lock;
                }
                else if (comboBox2.Text == TRAYTYPE.生肖料盘.ToString())
                {
                    unitDatatemp.Piece1 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), comboBox3.Text);
                    unitDatatemp.Piece1Quality = PIECEQUALITY.待检测;
                    unitDatatemp.Piece2 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), comboBox3.Text);
                    unitDatatemp.Piece2Quality = PIECEQUALITY.待检测;
                    unitDatatemp.Piece3 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), comboBox3.Text);
                    unitDatatemp.Piece3Quality = PIECEQUALITY.待检测;
                    unitDatatemp.Piece4 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), comboBox3.Text);
                    unitDatatemp.Piece4Quality = PIECEQUALITY.待检测;
                    unitDatatemp.Lock = rackdata.Lock;
                }
                else if (comboBox2.Text == TRAYTYPE.校徽料盘.ToString())
                {
                    unitDatatemp.Piece1 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), comboBox3.Text);
                    unitDatatemp.Piece1Quality = PIECEQUALITY.待检测;
                    unitDatatemp.Piece2 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), comboBox3.Text);
                    unitDatatemp.Piece2Quality = PIECEQUALITY.待检测;
                    unitDatatemp.Piece3 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), comboBox3.Text);
                    unitDatatemp.Piece3Quality = PIECEQUALITY.待检测;
                    unitDatatemp.Piece4 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), comboBox3.Text);
                    unitDatatemp.Piece4Quality = PIECEQUALITY.待检测;
                    unitDatatemp.Lock = rackdata.Lock;
                }
            }
        }

        private void InitManualUI()
        {
            for (int i = 1; i <= 35; i++)
            {
                comboBoxrack.Items.Add(string.Format("NO{0}", i));
            }
            comboBoxrack.SelectedIndex = 0;
            TRAYTYPE e0 = new TRAYTYPE();
            trayTypes = Enum.GetNames(e0.GetType());
            comboBoxtray.Items.AddRange(trayTypes);
            comboBoxtray.SelectedIndexChanged += ComboBoxtray_SelectedIndexChanged;
            comboBoxtray.SelectedIndex = 0;
            buttoninit.Click += Buttoninit_Click;
        }

        private void Buttoninit_Click(object sender, EventArgs e)
        {
            int no = comboBoxrack.SelectedIndex + 1;
            var rackUnitData = listRackData.Find(s => s.NO == no);
            if (rackUnitData != null && rackUnitData.Lock)
            {
                DialogResult select = MessageBox.Show(string.Format("仓位号为{0}处于锁定状态，是否强制初始化？", no), "提示", MessageBoxButtons.OK | MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (select != DialogResult.OK)
                    return;
            }

            //if (no % 7 == 0)
            //{
            //    PushMessage("当前料库硬件不支持第7列初始化！", Color.Red);
            //    return;
            //}

            if ((comboBoxtray.SelectedIndex == (int)TRAYTYPE.生肖料盘 && (comboBoxp1.Text == PIECETYTPE.无.ToString() || comboBoxp2.Text == PIECETYTPE.无.ToString() ||comboBoxp3.Text == PIECETYTPE.无.ToString() ||comboBoxp4.Text == PIECETYTPE.无.ToString()))
                || (comboBoxtray.SelectedIndex == (int)TRAYTYPE.叶轮料盘 && comboBoxp1.Text == PIECETYTPE.无.ToString()) 
                || (comboBoxtray.SelectedIndex == (int)TRAYTYPE.校徽料盘 && comboBoxp1.Text == PIECETYTPE.无.ToString()))
            {
                PushMessage("无法将仓位初始化为空料盘", Color.Red);
                return;
            }

            switch (comboBoxtray.SelectedIndex)
            {
                case (int)TRAYTYPE.空:
                    {
                        rackUnitData.Tray = TRAYTYPE.空;
                        rackUnitData.TrayVolume = 0;
                        rackUnitData.RfidID = "";
                        rackUnitData.Piece1 = PIECETYTPE.无;
                        rackUnitData.Piece1Quality = PIECEQUALITY.待检测;
                        rackUnitData.Piece2 = PIECETYTPE.无;
                        rackUnitData.Piece2Quality = PIECEQUALITY.待检测;
                        rackUnitData.Piece3 = PIECETYTPE.无;
                        rackUnitData.Piece3Quality = PIECEQUALITY.待检测;
                        rackUnitData.Piece4 = PIECETYTPE.无;
                        rackUnitData.Piece4Quality = PIECEQUALITY.待检测;
                        rackUnitData.Lock = false;
                    }
                    break;
                case (int)TRAYTYPE.叶轮料盘:
                    {
                        rackUnitData.Tray = TRAYTYPE.叶轮料盘;
                        rackUnitData.TrayVolume = 1;
                        if (comboBoxp1.Text == PIECETYTPE.叶轮毛坯.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.叶轮毛坯;
                            rackUnitData.Piece1Quality = PIECEQUALITY.待检测;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.叶轮.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.叶轮;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                        }
                        rackUnitData.Piece2 = PIECETYTPE.无;
                        rackUnitData.Piece2Quality = PIECEQUALITY.待检测;
                        rackUnitData.Piece3 = PIECETYTPE.无;
                        rackUnitData.Piece3Quality = PIECEQUALITY.待检测;
                        rackUnitData.Piece4 = PIECETYTPE.无;
                        rackUnitData.Piece4Quality = PIECEQUALITY.待检测;
                        rackUnitData.Lock = false;
                    }
                    break;
                case (int)TRAYTYPE.生肖料盘:
                    {
                        rackUnitData.Tray = TRAYTYPE.生肖料盘;
                        rackUnitData.TrayVolume = 4;

                         if (comboBoxp1.Text == PIECETYTPE.生肖毛坯.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.生肖毛坯;
                            rackUnitData.Piece1Quality = PIECEQUALITY.待检测;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.生肖鼠.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.生肖鼠;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.生肖牛.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.生肖牛;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.生肖虎.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.生肖虎;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.生肖兔.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.生肖兔;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.生肖龙.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.生肖龙;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.生肖蛇.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.生肖蛇;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.生肖马.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.生肖马;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.生肖羊.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.生肖羊;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.生肖猴.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.生肖猴;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.生肖鸡.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.生肖鸡;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.生肖狗.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.生肖狗;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.生肖猪.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.生肖猪;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                        }

                        if (comboBoxp2.Text == PIECETYTPE.生肖毛坯.ToString())
                        {
                            rackUnitData.Piece2 = PIECETYTPE.生肖毛坯;
                            rackUnitData.Piece2Quality = PIECEQUALITY.待检测;
                        }
                        else if (comboBoxp2.Text == PIECETYTPE.生肖鼠.ToString())
                        {
                            rackUnitData.Piece2 = PIECETYTPE.生肖鼠;
                            rackUnitData.Piece2Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp2.Text == PIECETYTPE.生肖牛.ToString())
                        {
                            rackUnitData.Piece2 = PIECETYTPE.生肖牛;
                            rackUnitData.Piece2Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp2.Text == PIECETYTPE.生肖虎.ToString())
                        {
                            rackUnitData.Piece2 = PIECETYTPE.生肖虎;
                            rackUnitData.Piece2Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp2.Text == PIECETYTPE.生肖兔.ToString())
                        {
                            rackUnitData.Piece2 = PIECETYTPE.生肖兔;
                            rackUnitData.Piece2Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp2.Text == PIECETYTPE.生肖龙.ToString())
                        {
                            rackUnitData.Piece2 = PIECETYTPE.生肖龙;
                            rackUnitData.Piece2Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp2.Text == PIECETYTPE.生肖蛇.ToString())
                        {
                            rackUnitData.Piece2 = PIECETYTPE.生肖蛇;
                            rackUnitData.Piece2Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp2.Text == PIECETYTPE.生肖马.ToString())
                        {
                            rackUnitData.Piece2 = PIECETYTPE.生肖马;
                            rackUnitData.Piece2Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp2.Text == PIECETYTPE.生肖羊.ToString())
                        {
                            rackUnitData.Piece2 = PIECETYTPE.生肖羊;
                            rackUnitData.Piece2Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp2.Text == PIECETYTPE.生肖猴.ToString())
                        {
                            rackUnitData.Piece2 = PIECETYTPE.生肖猴;
                            rackUnitData.Piece2Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp2.Text == PIECETYTPE.生肖鸡.ToString())
                        {
                            rackUnitData.Piece2 = PIECETYTPE.生肖鸡;
                            rackUnitData.Piece2Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp2.Text == PIECETYTPE.生肖狗.ToString())
                        {
                            rackUnitData.Piece2 = PIECETYTPE.生肖狗;
                            rackUnitData.Piece2Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp2.Text == PIECETYTPE.生肖猪.ToString())
                        {
                            rackUnitData.Piece2 = PIECETYTPE.生肖猪;
                            rackUnitData.Piece2Quality = PIECEQUALITY.合格;
                        }

                        if (comboBoxp3.Text == PIECETYTPE.生肖毛坯.ToString())
                        {
                            rackUnitData.Piece3 = PIECETYTPE.生肖毛坯;
                            rackUnitData.Piece3Quality = PIECEQUALITY.待检测;
                        }
                        else if (comboBoxp3.Text == PIECETYTPE.生肖鼠.ToString())
                        {
                            rackUnitData.Piece3 = PIECETYTPE.生肖鼠;
                            rackUnitData.Piece3Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp3.Text == PIECETYTPE.生肖牛.ToString())
                        {
                            rackUnitData.Piece3 = PIECETYTPE.生肖牛;
                            rackUnitData.Piece3Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp3.Text == PIECETYTPE.生肖虎.ToString())
                        {
                            rackUnitData.Piece3 = PIECETYTPE.生肖虎;
                            rackUnitData.Piece3Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp3.Text == PIECETYTPE.生肖兔.ToString())
                        {
                            rackUnitData.Piece3 = PIECETYTPE.生肖兔;
                            rackUnitData.Piece3Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp3.Text == PIECETYTPE.生肖龙.ToString())
                        {
                            rackUnitData.Piece3 = PIECETYTPE.生肖龙;
                            rackUnitData.Piece3Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp3.Text == PIECETYTPE.生肖蛇.ToString())
                        {
                            rackUnitData.Piece3 = PIECETYTPE.生肖蛇;
                            rackUnitData.Piece3Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp3.Text == PIECETYTPE.生肖马.ToString())
                        {
                            rackUnitData.Piece3 = PIECETYTPE.生肖马;
                            rackUnitData.Piece3Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp3.Text == PIECETYTPE.生肖羊.ToString())
                        {
                            rackUnitData.Piece3 = PIECETYTPE.生肖羊;
                            rackUnitData.Piece3Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp3.Text == PIECETYTPE.生肖猴.ToString())
                        {
                            rackUnitData.Piece3 = PIECETYTPE.生肖猴;
                            rackUnitData.Piece3Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp3.Text == PIECETYTPE.生肖鸡.ToString())
                        {
                            rackUnitData.Piece3 = PIECETYTPE.生肖鸡;
                            rackUnitData.Piece3Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp3.Text == PIECETYTPE.生肖狗.ToString())
                        {
                            rackUnitData.Piece3 = PIECETYTPE.生肖狗;
                            rackUnitData.Piece3Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp3.Text == PIECETYTPE.生肖猪.ToString())
                        {
                            rackUnitData.Piece3 = PIECETYTPE.生肖猪;
                            rackUnitData.Piece3Quality = PIECEQUALITY.合格;
                        }

                        if (comboBoxp4.Text == PIECETYTPE.生肖毛坯.ToString())
                        {
                            rackUnitData.Piece4 = PIECETYTPE.生肖毛坯;
                            rackUnitData.Piece4Quality = PIECEQUALITY.待检测;
                        }
                        else if (comboBoxp4.Text == PIECETYTPE.生肖鼠.ToString())
                        {
                            rackUnitData.Piece4 = PIECETYTPE.生肖鼠;
                            rackUnitData.Piece4Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp4.Text == PIECETYTPE.生肖牛.ToString())
                        {
                            rackUnitData.Piece4 = PIECETYTPE.生肖牛;
                            rackUnitData.Piece4Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp4.Text == PIECETYTPE.生肖虎.ToString())
                        {
                            rackUnitData.Piece4 = PIECETYTPE.生肖虎;
                            rackUnitData.Piece4Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp4.Text == PIECETYTPE.生肖兔.ToString())
                        {
                            rackUnitData.Piece4 = PIECETYTPE.生肖兔;
                            rackUnitData.Piece4Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp4.Text == PIECETYTPE.生肖龙.ToString())
                        {
                            rackUnitData.Piece4 = PIECETYTPE.生肖龙;
                            rackUnitData.Piece4Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp4.Text == PIECETYTPE.生肖蛇.ToString())
                        {
                            rackUnitData.Piece4 = PIECETYTPE.生肖蛇;
                            rackUnitData.Piece4Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp4.Text == PIECETYTPE.生肖马.ToString())
                        {
                            rackUnitData.Piece4 = PIECETYTPE.生肖马;
                            rackUnitData.Piece4Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp4.Text == PIECETYTPE.生肖羊.ToString())
                        {
                            rackUnitData.Piece4 = PIECETYTPE.生肖羊;
                            rackUnitData.Piece4Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp4.Text == PIECETYTPE.生肖猴.ToString())
                        {
                            rackUnitData.Piece4 = PIECETYTPE.生肖猴;
                            rackUnitData.Piece4Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp4.Text == PIECETYTPE.生肖鸡.ToString())
                        {
                            rackUnitData.Piece4 = PIECETYTPE.生肖鸡;
                            rackUnitData.Piece4Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp4.Text == PIECETYTPE.生肖狗.ToString())
                        {
                            rackUnitData.Piece4 = PIECETYTPE.生肖狗;
                            rackUnitData.Piece4Quality = PIECEQUALITY.合格;
                        }
                        else if (comboBoxp4.Text == PIECETYTPE.生肖猪.ToString())
                        {
                            rackUnitData.Piece4 = PIECETYTPE.生肖猪;
                            rackUnitData.Piece4Quality = PIECEQUALITY.合格;
                        }
                        rackUnitData.Lock = false;
                    }
                    break;
                case (int)TRAYTYPE.校徽料盘:
                    {
                        rackUnitData.Tray = TRAYTYPE.校徽料盘;
                        rackUnitData.TrayVolume = 1;
                     if (comboBoxp1.Text == PIECETYTPE.校徽毛坯.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.校徽毛坯;
                            rackUnitData.Piece1Quality = PIECEQUALITY.待检测;
                            rackUnitData.Piece2 = PIECETYTPE.校徽毛坯;
                            rackUnitData.Piece2Quality = PIECEQUALITY.待检测;
                            rackUnitData.Piece3 = PIECETYTPE.校徽毛坯;
                            rackUnitData.Piece3Quality = PIECEQUALITY.待检测;
                            rackUnitData.Piece4 = PIECETYTPE.校徽毛坯;
                            rackUnitData.Piece4Quality = PIECEQUALITY.待检测;
                        }
                        else if (comboBoxp1.Text == PIECETYTPE.校徽.ToString())
                        {
                            rackUnitData.Piece1 = PIECETYTPE.校徽;
                            rackUnitData.Piece1Quality = PIECEQUALITY.合格;
                            rackUnitData.Piece2 = PIECETYTPE.校徽;
                            rackUnitData.Piece2Quality = PIECEQUALITY.合格;
                            rackUnitData.Piece3 = PIECETYTPE.校徽;
                            rackUnitData.Piece3Quality = PIECEQUALITY.合格;
                            rackUnitData.Piece4 = PIECETYTPE.校徽;
                            rackUnitData.Piece4Quality = PIECEQUALITY.合格;
                        }
                        rackUnitData.Lock = false;
                    }
                    break;
            }
            OnRackAction(false, no);
        }

        private void InitMessageUI()
        {
            richTextBox1.TextChanged += RichTextBox1_TextChanged;
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rich = sender as RichTextBox;
            rich.SelectionStart = rich.Text.Length;
            rich.ScrollToCaret();
        }

        private void PushMessage(string Msg, Color color)
        {
            richTextBox1.InvokeEx(c =>
            {
                string Message = Msg + "     " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
                richTextBox1.SelectionColor = color;
                richTextBox1.AppendText(Message);
            });
        }

        private void ComboBoxtray_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxtray.SelectedIndex == (int)TRAYTYPE.空)
            {
                tableLayoutPanelp1.Visible = false;
                tableLayoutPanelp2.Visible = false;
                tableLayoutPanelp3.Visible = false;
                tableLayoutPanelp4.Visible = false;
            }
            else if (comboBoxtray.SelectedIndex == (int)TRAYTYPE.叶轮料盘)
            {
                comboBoxp1.Items.Clear();
                comboBoxp2.Items.Clear();
                comboBoxp3.Items.Clear();
                comboBoxp4.Items.Clear();
                //comboBoxp1.Items.Add(PIECETYTPE.空.ToString());
                comboBoxp1.Items.Add(PIECETYTPE.叶轮毛坯.ToString());
                comboBoxp1.Items.Add(PIECETYTPE.叶轮.ToString());
                comboBoxp1.SelectedIndex = 0;
                tableLayoutPanelp1.Visible = true;
                tableLayoutPanelp2.Visible = false;
                tableLayoutPanelp3.Visible = false;
                tableLayoutPanelp4.Visible = false;
            }
            else if (comboBoxtray.SelectedIndex == (int)TRAYTYPE.生肖料盘)
            {
                comboBoxp1.Items.Clear();
                comboBoxp2.Items.Clear();
                comboBoxp3.Items.Clear();
                comboBoxp4.Items.Clear();
                PIECETYTPE e0 = new PIECETYTPE();
                pieceTypes = Enum.GetNames(e0.GetType());
                comboBoxp1.Items.AddRange(pieceTypes);
                comboBoxp1.Items.Remove(PIECETYTPE.叶轮毛坯.ToString());
                comboBoxp1.Items.Remove(PIECETYTPE.校徽毛坯.ToString());
                comboBoxp1.Items.Remove(PIECETYTPE.叶轮.ToString());
                comboBoxp1.Items.Remove(PIECETYTPE.校徽.ToString());
                comboBoxp1.Items.Remove(PIECETYTPE.无.ToString());
                comboBoxp1.SelectedIndex = 0;
                comboBoxp2.Items.AddRange(pieceTypes);
                comboBoxp2.Items.Remove(PIECETYTPE.叶轮毛坯.ToString());
                comboBoxp2.Items.Remove(PIECETYTPE.校徽毛坯.ToString());
                comboBoxp2.Items.Remove(PIECETYTPE.叶轮.ToString());
                comboBoxp2.Items.Remove(PIECETYTPE.校徽.ToString());
                comboBoxp2.Items.Remove(PIECETYTPE.无.ToString());
                comboBoxp2.SelectedIndex = 0;
                comboBoxp3.Items.AddRange(pieceTypes);
                comboBoxp3.Items.Remove(PIECETYTPE.叶轮毛坯.ToString());
                comboBoxp3.Items.Remove(PIECETYTPE.校徽毛坯.ToString());
                comboBoxp3.Items.Remove(PIECETYTPE.叶轮.ToString());
                comboBoxp3.Items.Remove(PIECETYTPE.校徽.ToString());
                comboBoxp3.Items.Remove(PIECETYTPE.无.ToString());
                comboBoxp3.SelectedIndex = 0;
                comboBoxp4.Items.AddRange(pieceTypes);
                comboBoxp4.Items.Remove(PIECETYTPE.叶轮毛坯.ToString());
                comboBoxp4.Items.Remove(PIECETYTPE.校徽毛坯.ToString());
                comboBoxp4.Items.Remove(PIECETYTPE.叶轮.ToString());
                comboBoxp4.Items.Remove(PIECETYTPE.校徽.ToString());
                comboBoxp4.Items.Remove(PIECETYTPE.无.ToString());
                comboBoxp4.SelectedIndex = 0;
                tableLayoutPanelp1.Visible = true;
                tableLayoutPanelp2.Visible = true;
                tableLayoutPanelp3.Visible = true;
                tableLayoutPanelp4.Visible = true;
            }
            else if (comboBoxtray.SelectedIndex == (int)TRAYTYPE.校徽料盘)
            {
                comboBoxp1.Items.Clear();
                comboBoxp2.Items.Clear();
                comboBoxp3.Items.Clear();
                comboBoxp4.Items.Clear();
                PIECETYTPE e0 = new PIECETYTPE();
                pieceTypes = Enum.GetNames(e0.GetType());
                comboBoxp1.Items.Add(PIECETYTPE.校徽毛坯.ToString());
                comboBoxp1.Items.Add(PIECETYTPE.校徽.ToString());
                comboBoxp1.SelectedIndex = 1;
                tableLayoutPanelp1.Visible = true;
                tableLayoutPanelp2.Visible = false;
                tableLayoutPanelp3.Visible = false;
                tableLayoutPanelp4.Visible = false;
            }
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == (int)TRAYTYPE.空)
            {
                tableLayoutPanel9.Visible = false;
                tableLayoutPanel10.Visible = false;
                tableLayoutPanel11.Visible = false;
                tableLayoutPanel12.Visible = false;
            }
            else if (comboBox2.SelectedIndex == (int)TRAYTYPE.叶轮料盘)
            {
                comboBox3.Items.Clear();
                //comboBox3.Items.Add(PIECETYTPE.空.ToString());
                comboBox3.Items.Add(PIECETYTPE.叶轮毛坯.ToString());
                comboBox3.SelectedIndex = 0;
                tableLayoutPanel9.Visible = true;
                tableLayoutPanel10.Visible = false;
                tableLayoutPanel11.Visible = false;
                tableLayoutPanel12.Visible = false;
            }
            else if (comboBox2.SelectedIndex == (int)TRAYTYPE.生肖料盘)
            {
                comboBox3.Items.Clear();
                comboBox4.Items.Clear();
                comboBox5.Items.Clear();
                comboBox6.Items.Clear();
                comboBox3.Items.Add(PIECETYTPE.生肖毛坯.ToString());
                comboBox3.SelectedIndex = 0;
                tableLayoutPanel9.Visible = true;
                tableLayoutPanel10.Visible = false;
                tableLayoutPanel11.Visible = false;
                tableLayoutPanel12.Visible = false;
            }
            else if (comboBox2.SelectedIndex == (int)TRAYTYPE.校徽料盘)
            {
                comboBox3.Items.Clear();
                comboBox4.Items.Clear();
                comboBox5.Items.Clear();
                comboBox6.Items.Clear();
                comboBox3.Items.Add(PIECETYTPE.校徽毛坯.ToString());
                comboBox3.SelectedIndex = 0;
                tableLayoutPanel9.Visible = true;
                tableLayoutPanel10.Visible = false;
                tableLayoutPanel11.Visible = false;
                tableLayoutPanel12.Visible = false;
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            var rackdata = listRackData.Find(s => s.Lock == true);
            if (rackdata != null)
            {
                if (textBoxINOUTNo.Text != rackdata.NO.ToString())
                {
                    textBoxINOUTNo.Text = rackdata.NO.ToString();
                }
            }
            else
            {
                textBoxINOUTNo.Text = "";
            }
            if (rukuflag == false && !button1.Enabled)
            {

                button1.Enabled = true;
                button1.Text = "手动入库";
                button2.Enabled = false;
            }
            if (chukuflag == false && !button3.Enabled)
            {
                button3.Enabled = true;
                button3.Text = "手动出库";
                button4.Enabled = false;

            }
        }
    }
}
