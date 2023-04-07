using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SCADA.Model;
using HNCAPI;
using HNC_MacDataService;
using Sygole.HFReader;
using HFAPItest;
using System.Threading;
using System.Collections;
using static SCADA.SimensPLC.WMSPLC;
using System.Reflection.Emit;
using SCADA.SimensPLC;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

/*********************************************************************************************************
 * 宏友的PLC信号定义如下：
 * dt4001~dt4096           库位状态                  1：表示有料盘      0：表示没有料盘             交互规则：PLC更新状态，上位机总控软件读取
 * dt4099                       立体库状态                1:自动运行中  2:自动中任务执行完成  3:手动   4:堆垛机X轴故障  5:堆垛机Y轴故障  6:堆垛机S轴故障  7:堆垛机任务执行失败   8:急停  9:自动中空闲  10:人工故障复位确认
 * dt4100                    立体库运行目标起始点        上位机总控软件写入     1~96 对应库位位置
 * dt4101                    立体库运行终点目标点        上位机总控软件写入     100:倍数链取料站   101:倍数链放料站  102:中转台出库   103:中转台入库
 * dt4102                    立体库开始执行任务            交互规则：上位机总控软件填入1，PLC使用后清0
 * dt4109                   料仓平台允许入库信号，接受任务，中转台物料检测反馈                        1有效
 * dt4110                   产品状态 不合格NG/合格OK                                                                 1:OK    2:NG              总控软件写入，PLC读取
 * dt4111                  入库倍数链，RFID触发                                                                          1有效
 * dt4112                  入库倍数链正转触发                                                                              1有效
 * dt4113                  入库倍速链物料检测反馈                                                                       1有效
 * 
 * 
 * 总控软件获取和设置宏友PLC的信号是通过plcdt的mysql数据库表来通信的
 * 总控软件往PLC里设置信号值   用以_i结尾的dt信号
 * 总控软件获取PLC的信号值      用以_o结尾的dt信号
 * *******************************************************************************************************/


namespace SCADA.NewApp
{
    public enum RackTaskState
    {
        空闲,
        手动入库,
        手动出库,
        自动入库,
        自动出库,
        任务正在取消,

    }
    public partial class StockBinForm : Form
    {
        Dictionary<int, RackUnit> rackunitDic = new Dictionary<int, RackUnit>();
        static public bool[] rackchanged = new bool[40];
        static public int CurNOLock = 0;
        static public bool CurLockState = false;//
        static public int CurOutNOLock = 0;
        static public int CurNO = 0;
        static public bool CurINState = false;//
        static public RackTaskState racktaskstate = RackTaskState.空闲;
        public static RackUnitData unitDatatemp = null;
        public delegate void RackAction(object sender, bool flag, int rackno);
        public static RackAction rackAction;
        public static List<RackUnitData> listRackData = new List<RackUnitData>();
        public static RackUnit[] RackUnitArry = new RackUnit[40];

        public static MANUALIN rukustatus = MANUALIN.结束;
        public static MANUALOUT chukustatus = MANUALOUT.结束;

        public static bool rukuflag = false;
        public static bool chukuflag = false;
        public static bool rukumanu = false;
        public static bool chukumanu = false;


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


        public StockBinForm()
        {
            InitializeComponent();
            InitUI();
            InitMessageUI();

            rackAction += ShowRackChanged;
            OrderExcuteForm.rackAction1 += ShowRack1Changed;
            OrderExcuteForm.rackAction2 += ShowRack2Changed;
            for (int i = 1; i < 41; i++)
            {
                comboBoxNo1.Items.Add(string.Format("NO{0}", i));
                comboBoxNo2.Items.Add(string.Format("NO{0}", i));
                comboBoxNo3.Items.Add(string.Format("NO{0}", i));
            }
            //rackAction+= ShowRackChanged;
            //OrderExcuteForm.rackAction1 += ShowRack1Changed;

            Task.Run(() => AutoDoWork());
        }
        void ShowRack1Changed(object sender, bool flag, int rackno)
        {

            PushMessage(string.Format("更新仓位{0}的信息", rackno), Color.Black);
            int index = rackno - 1;
            var rackdata = listRackData.Find(s => s.NO == rackno);
            if (rackdata != null)
            {
                var t = rackunitDic[index];
                t .SetData(rackno, rackdata.Tray, rackdata.Piece1Quality, rackdata.Piece2Quality, rackdata.Piece3Quality, rackdata.Piece4Quality, rackdata.RfidID);
              
            }


        }
        void ShowRackChanged(object sender, bool flag, int rackno)
        {
            PushMessage(string.Format("更新仓位{0}的信息", rackno), Color.Black);
            int index = rackno - 1;
            var rackdata = listRackData.Find(s => s.NO == rackno);
            if (rackdata != null)
            {

                rackunitDic[index].SetData(rackno, rackdata.Tray, rackdata.Piece1Quality, rackdata.Piece2Quality, rackdata.Piece3Quality, rackdata.Piece4Quality, rackdata.RfidID);
                Program.Repo.Update<RackUnitData>(rackdata);
            }
        }
        void ShowRack2Changed(object sender, bool flag, int rackno)
        {
            PushMessage(string.Format("更新仓位{0}的信息",  rackno), Color.Black);
            int index = rackno - 1;
            var rackdata = listRackData.Find(s => s.NO == rackno);
            if (rackdata != null)
            {

                rackunitDic[index].SetData(rackno, rackdata.Tray, rackdata.Piece1Quality, rackdata.Piece2Quality, rackdata.Piece3Quality, rackdata.Piece4Quality, rackdata.RfidID);
                Program.Repo.Update<RackUnitData>(rackdata);
            }
        }

        //void ShowRackChanged(object sender, bool flag, int rackno)
        //{
        //    PushMessage(string.Format("{0}更新仓位{1}的信息", flag ? "手动操作" : "手动初始化", rackno), Color.Black);
        //    int index = rackno - 1;
        //    UpdateRackeDataShow(index);

        //}

        //void ShowRack1Changed(object sender, bool flag, int rackno)
        //{

        //    PushMessage(string.Format("{0}更新仓位{1}的信息", flag ? "手动操作" : "手动初始化", rackno), Color.Black);
        //    int index = rackno - 1;
        //    UpdateRackeDataShow(index);

        //}

        private void InitMessageUI()
        {
            richTextBox_Msg.TextChanged += richTextBox_Msg_TextChanged;
        }
        public void InitUI()
        {
            for (int i = 0; i < 40; i++)
            {
                var rauit = new RackUnit();
                var rackunitdata = Program.Repo.GetSingle<RackUnitData>(p => p.NO == i);
                if (rackunitdata != null)
                {
                    rauit.SetData(i+1, rackunitdata.Tray, rackunitdata.Piece1Quality, rackunitdata.Piece2Quality, rackunitdata.Piece3Quality, rackunitdata.Piece4Quality, rackunitdata.RfidID);

                }
                RackUnitArry[i] = rauit;
                Program.Repo.Update<RackUnitData>(rackunitdata);
                rackunitDic.Add(i, rauit);

                this.tableLayoutPanel6.Controls.Add(this.rackunitDic[i], i % 8, i / 8);
            }

        }
        private void AutoDoWork()
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                for (int i = 0; i < 40; i++)
                {
                    //if (rackchanged[i] == true)
                    //{

                    //    UpdateRackeDataShow(i+1);
                    //    rackchanged[i] = false;
                    //}
                }
            }
        }

        private void UpdateRackeDataShow(int i)
        {

            var rackunitdata = Program.Repo.GetSingle<RackUnitData>(p => p.NO == i);
            if (rackunitdata != null)
            {

                PushMessage(string.Format("更新仓位{0}的信息", i), Color.Black);

                rackunitDic[i - 1].SetData(i, rackunitdata.Tray, rackunitdata.Piece1Quality, rackunitdata.Piece2Quality, rackunitdata.Piece3Quality, rackunitdata.Piece4Quality, rackunitdata.RfidID);
                Program.Repo.Update<RackUnitData>(rackunitdata);
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //当前任务状态刷新
            if (CurNO != 0)
            {
                label1.Text = CurNO.ToString();
            }

            else
            {
                label1.Text = "";
            }

            label2.Text = racktaskstate.ToString();
            if (label1.Text == "" && racktaskstate == RackTaskState.空闲)
            {
                if (button1.BackColor == Color.DarkTurquoise)
                {
                    button1.BackColor = Color.Gray;
                    button1.Enabled = false;
                }

            }
            if (label1.Text != "" && racktaskstate != RackTaskState.任务正在取消)
            {
                if (button1.BackColor == Color.Gray)
                {
                    button1.BackColor = Color.DarkTurquoise;
                    button1.Enabled = true;
                }
            }
            //检查料仓状态
            //if (true)
            //{
            //    if (racktaskstate == RackTaskState.任务正在取消)
            //    {
            //        racktaskstate = RackTaskState.空闲;
            //    }

            //}

            //手动出入库按键
            if (CurNO == 0 && rukuflag == false&& !BtnManIn.Enabled)
            {
                BtnManIn.Enabled = true;

                BtnManIn.BackColor = Color.DarkTurquoise;
                BtnManIn.Text = "入库";
                BtnInCansl.Enabled = false;
                BtnInCansl.BackColor = Color.Gray;
            }
            if (CurNO == 0 && chukuflag == false&&!BtnManOut.Enabled)
            {
                BtnManOut.Enabled = true;
                BtnManOut.BackColor = Color.DarkTurquoise;
                BtnManOut.Text = "出库";
                BtnOutCansl.Enabled = false;
                BtnOutCansl.BackColor = Color.Gray;
            }
            for (int i = 0; i < 40; i++)
            {
                if (rackchanged[i] == true)
                {
                    if(CurNO ==  i+1)
                    {
                        CurNO = 0;
                        CurNOLock = 0;
                        CurOutNOLock = 0;
                    }
                    UpdateRackeDataShow(i + 1);
                    rackchanged[i] = false;
                }
            }

        }

        //取消任务
        private void BtnCansl_Click(object sender, EventArgs e)
        {
            if (racktaskstate != RackTaskState.任务正在取消)
            {
                rukuflag = false;

                rukumanu = false;
                rukustatus = MANUALIN.结束;
                BtnManIn.Enabled = true;
                BtnManIn.Text = "入库";
                BtnInCansl.Enabled = false;
                BtnManIn.BackColor = Color.DarkTurquoise;
            }

            if (unitDatatemp != null)
            {
                PushMessage(string.Format("仓位{0}任务取消", unitDatatemp.NO), Color.Black);
                var rackdata = listRackData.Find(s => s.NO == unitDatatemp.NO);
                rackdata.Lock = false;
                unitDatatemp = null;
                int curtaskno = 0;
                if (CurNO != 0)
                {
                    curtaskno = CurNO;
                    CurNO = 0;
                }

                CurNOLock = 0;

                racktaskstate = RackTaskState.任务正在取消;
                rackchanged[curtaskno - 1] = true;
            }
        }


        private void BtnInit_Click(object sender, EventArgs e)
        {
            int no = comboBoxNo1.SelectedIndex;
            var rackUnitData = listRackData.Find(s => s.NO == no+1);
            if (rackUnitData != null && rackUnitData.Lock)
            {
                DialogResult select = MessageBox.Show(string.Format("仓位号为{0}处于锁定状态，是否强制初始化？", no), "提示", MessageBoxButtons.OK | MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (select != DialogResult.OK)
                    return;
            }
            //            A料毛坯
            //B料毛坯
            //A料空盘
            //B料空盘
            //空仓位
            switch (comboBoxT1.SelectedIndex)
            {
                case 0://A料毛坯
                    {
                        rackUnitData.Tray = TRAYTYPE.料盘A;
                        rackUnitData.TrayVolume = 0;
                        rackUnitData.RfidID = "";
                        rackUnitData.Piece1 = PIECETYTPE.毛坯A;
                        rackUnitData.Piece1Quality = PIECEQUALITY.待检测;
                        rackUnitData.Piece2 = PIECETYTPE.毛坯A;
                        rackUnitData.Piece2Quality = PIECEQUALITY.待检测;
                        rackUnitData.Piece3 = PIECETYTPE.毛坯A;
                        rackUnitData.Piece3Quality = PIECEQUALITY.待检测;
                        rackUnitData.Piece4 = PIECETYTPE.毛坯A;
                        rackUnitData.Piece4Quality = PIECEQUALITY.待检测;
                        rackUnitData.Lock = false;
                    }
                    break;
                case 1://B料毛坯
                    {
                        rackUnitData.Tray = TRAYTYPE.料盘B;
                        rackUnitData.TrayVolume = 4;
                        rackUnitData.Piece1 = PIECETYTPE.毛坯B;
                        rackUnitData.Piece1Quality = PIECEQUALITY.待检测;
                        rackUnitData.Piece2 = PIECETYTPE.毛坯B;
                        rackUnitData.Piece2Quality = PIECEQUALITY.待检测;
                        rackUnitData.Piece3 = PIECETYTPE.毛坯B;
                        rackUnitData.Piece3Quality = PIECEQUALITY.待检测;
                        rackUnitData.Piece4 = PIECETYTPE.毛坯B;
                        rackUnitData.Piece4Quality = PIECEQUALITY.待检测;
                        rackUnitData.Lock = false;
                    }
                    break;
                case 2://A料空盘
                    {
                        rackUnitData.Tray = TRAYTYPE.料盘A;
                        rackUnitData.TrayVolume = 4;
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
                case 3://B料空盘
                    {
                        rackUnitData.Tray = TRAYTYPE.料盘B;
                        rackUnitData.TrayVolume = 4;
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
                case 4://空仓
                    {
                        rackUnitData.Tray = TRAYTYPE.空;
                        rackUnitData.TrayVolume = 4;
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
            }

            rackchanged[comboBoxNo1.SelectedIndex] = true;
            no = no+1;
            OnRackAction(false, no);
        }
   
        private void BtnManIn_Click(object sender, EventArgs e)
        {
            if (CurNO != 0 || CurNOLock != 0||chukuflag ||rukuflag)
            {
                PushMessage(string.Format("仓位{0}正在出库或者入库，无法手动入库！", CurNO), Color.Red);
                return;
            }
         
         
            var rackdata = listRackData.Find(s => s.Lock == true);
            if (rackdata != null)
            {
                PushMessage("料库执行自动出入库任务，无法手动入库！", Color.Red);
                return;
            }
            int no = comboBoxNo2.SelectedIndex + 1;
            rackdata = listRackData.Find(s => s.NO == no);
            if (rackdata.Tray != TRAYTYPE.空)
            {
                PushMessage(string.Format("仓位{0}有料，无法入库！", no), Color.Red);
                return;
            }

         
            int reg = no / 16;
            int bit = no % 16;
            bool res3 = false;
            int ivalue3 = 0;
            if (bit < 8)//
            {
                LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.BinState1 + reg, out ivalue3);
                res3 = ControlPLC.GetBoolValue(ivalue3, bit + 8);//查询到入库完成信号
            }
            else
            {
                LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.BinState1 + reg, out ivalue3);
                res3 = ControlPLC.GetBoolValue(ivalue3, bit - 8);//查询到入库完成信号
            }
            if (res3)
            {
                PushMessage("入库位置已经有料盘，不能入库", Color.Red);
                return;
            }


            if (!LineMainForm.wmsplc.GetOnlineState())
            {
                PushMessage(string.Format("料库PLC离线，无法入库！", no), Color.Red);
                return;
            }

            if (!LineMainForm.wmsplc.GetOnlineState())
            {
                PushMessage(string.Format("与料库PLC通讯出错！", no), Color.Red);
                return;  
            }
            //判断料库状态，是否可以接受任务
            int ivalue1;
            int ivalue2;
         
            bool res1 = LineMainForm.wmsplc.ReadsingleRegister((int)DATAINDEX.RackState, out ivalue1);//1空闲等待
            bool res2 = LineMainForm.wmsplc.ReadsingleRegister((int)DATAINDEX.RackErrCode, out ivalue2);//0无错误
             res3 = LineMainForm.wmsplc.ReadsingleRegister((int)DATAINDEX.TaskStateBit, out ivalue3);//1当前有任务在执行
            if(res1 && res2 && res3)
            {
                if (ivalue1 != 1)
                {
                    PushMessage(string.Format("料库不是空闲状态，无法入库！", no), Color.Red);
                    return;
                }

                if (ivalue2!=0)
                {
                    PushMessage(string.Format("料库有故障，无法入库！", no), Color.Red);
                    return;
                }

                if (ivalue3!=0)
                {
                    PushMessage(string.Format("料库当前有任务，无法入库！", no), Color.Red);
                    return;
                }
            }

            if (BtnManIn.Text == "入库")
            {
                BtnManIn.Text = "入库中";
                CurNO = comboBoxNo2.SelectedIndex + 1;

                rukuflag = true;
                rukumanu = true;

                BtnManIn.Enabled = false;
                BtnInCansl.Enabled = true;
                

                PushMessage(string.Format("开始手动入库到仓位{0}.", no), Color.Black);
                rackdata.Lock = true;
                unitDatatemp = new RackUnitData();
                unitDatatemp.NO = rackdata.NO;

                switch (comboBoxT2.SelectedIndex)
                {
                    case 0://A料毛坯
                        {
                            unitDatatemp.Tray = TRAYTYPE.料盘A;
                            unitDatatemp.TrayVolume = 4;

                            unitDatatemp.Piece1 = PIECETYTPE.毛坯A;
                            unitDatatemp.Piece1Quality = PIECEQUALITY.待检测;
                            unitDatatemp.Piece2 = PIECETYTPE.毛坯A;
                            unitDatatemp.Piece2Quality = PIECEQUALITY.待检测;
                            unitDatatemp.Piece3 = PIECETYTPE.毛坯A;
                            unitDatatemp.Piece3Quality = PIECEQUALITY.待检测;
                            unitDatatemp.Piece4 = PIECETYTPE.毛坯A;
                            unitDatatemp.Piece4Quality = PIECEQUALITY.待检测;
                            unitDatatemp.Lock = rackdata.Lock;
                        }
                        break;
                    case 1://B料毛坯
                        {
                            unitDatatemp.Tray = TRAYTYPE.料盘B;
                            unitDatatemp.TrayVolume = 4;

                            unitDatatemp.Piece1 = PIECETYTPE.毛坯B;
                            unitDatatemp.Piece1Quality = PIECEQUALITY.待检测;
                            unitDatatemp.Piece2 = PIECETYTPE.毛坯B;
                            unitDatatemp.Piece2Quality = PIECEQUALITY.待检测;
                            unitDatatemp.Piece3 = PIECETYTPE.毛坯B;
                            unitDatatemp.Piece3Quality = PIECEQUALITY.待检测;
                            unitDatatemp.Piece4 = PIECETYTPE.毛坯B;
                            unitDatatemp.Piece4Quality = PIECEQUALITY.待检测;
                            unitDatatemp.Lock = rackdata.Lock;
                        }
                        break;
                    case 2://A料空盘
                        {
                            unitDatatemp.Tray = TRAYTYPE.料盘A;
                            unitDatatemp.TrayVolume = 4;

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
                        break;
                    case 3://B料空盘
                        {
                            unitDatatemp.Tray = TRAYTYPE.料盘B;
                            unitDatatemp.TrayVolume = 4;

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

                        break;
                }
            }
        }

        private void BtnManOut_Click(object sender, EventArgs e)
        {
            if (CurNO != 0 || CurNOLock != 0 || chukuflag || rukuflag)
            {
                PushMessage(string.Format("仓位{0}正在出库或者入库，无法手动出库！", CurNO), Color.Red);
                return;
            }
            
            var rackdata = listRackData.Find(s => s.Lock == true);
            if (rackdata != null)
            {
                PushMessage("料库执行自动出入库任务，无法手动出库！", Color.Red);
                return;
            }
            int no = comboBoxNo3.SelectedIndex + 1;
            rackdata = listRackData.Find(s => s.NO == no);
            if (rackdata.Tray == TRAYTYPE.空)
            {
                
                PushMessage(string.Format("仓位{0}有料，无法出库！", no), Color.Red);
                return;
            }



            if (!LineMainForm.wmsplc.GetOnlineState())
            {
                PushMessage(string.Format("料库PLC离线，无法出库！", no), Color.Red);
                return;
            }

            if (!LineMainForm.wmsplc.GetOnlineState())
            {
                PushMessage(string.Format("与料库PLC通讯出错！", no), Color.Red);
                return;
            }
           
            //判断料库状态，是否可以接受任务
            int ivalue1;
            int ivalue2;
            int ivalue3;
            bool res1 = LineMainForm.wmsplc.ReadsingleRegister((int)DATAINDEX.RackState, out ivalue1);//1空闲等待
            bool res2 = LineMainForm.wmsplc.ReadsingleRegister((int)DATAINDEX.RackErrCode, out ivalue2);//0无错误
            bool res3 = LineMainForm.wmsplc.ReadsingleRegister((int)DATAINDEX.TaskStateBit, out ivalue3);//1当前有任务在执行
            if (res1 && res2 && res3)
            {
                if (ivalue1 != 1)
                {
                    PushMessage(string.Format("料库不是空闲状态，无法出库！", no), Color.Red);
                    return;
                }

                if (ivalue2 != 0)
                {
                    PushMessage(string.Format("料库有故障，无法出库！", no), Color.Red);
                    return;
                }

                if (ivalue3 != 0)
                {
                    PushMessage(string.Format("料库当前有任务，无法出库！", no), Color.Red);
                    return;
                }
            }

            if (BtnManOut.Text == "出库")
            {
                chukuflag = true;
                chukumanu = true;
                BtnManOut.Text = "出库中";
                CurNO = comboBoxNo3.SelectedIndex + 1;

                BtnManOut.Enabled = false;
                BtnOutCansl.Enabled = true;
                PushMessage(string.Format("仓位{0}开始出库.", no), Color.Black);
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

        private void BtnInitAll_Click(object sender, EventArgs e)
        {

            int start = 0;
            int end = 0;

            if (comboBoxrow.SelectedIndex == 5)
            {
                start = 0;
                end = 40;
            }
            else
            {
                start = comboBoxrow.SelectedIndex * 8;
                end = start + 8;

            }
            for (int i = start; i < end; i++)
            {
                int no = i;
                var rackUnitData = listRackData.Find(s => s.NO == no);
                if (rackUnitData != null && rackUnitData.Lock)
                {
                    DialogResult select = MessageBox.Show(string.Format("仓位号为{0}处于锁定状态，是否强制初始化？", no), "提示", MessageBoxButtons.OK | MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                    if (select != DialogResult.OK)
                        return;
                }
                switch (comboBoxT3.SelectedIndex)
                {
                    case 0://A料毛坯
                        {
                            rackUnitData.Tray = TRAYTYPE.料盘A;
                            rackUnitData.TrayVolume = 0;
                            rackUnitData.RfidID = "";
                            rackUnitData.Piece1 = PIECETYTPE.毛坯A;
                            rackUnitData.Piece1Quality = PIECEQUALITY.待检测;
                            rackUnitData.Piece2 = PIECETYTPE.毛坯A;
                            rackUnitData.Piece2Quality = PIECEQUALITY.待检测;
                            rackUnitData.Piece3 = PIECETYTPE.毛坯A;
                            rackUnitData.Piece3Quality = PIECEQUALITY.待检测;
                            rackUnitData.Piece4 = PIECETYTPE.毛坯A;
                            rackUnitData.Piece4Quality = PIECEQUALITY.待检测;
                            rackUnitData.Lock = false;
                        }
                        break;
                    case 1://B料毛坯
                        {
                            rackUnitData.Tray = TRAYTYPE.料盘B;
                            rackUnitData.TrayVolume = 4;
                            rackUnitData.Piece1 = PIECETYTPE.毛坯B;
                            rackUnitData.Piece1Quality = PIECEQUALITY.待检测;
                            rackUnitData.Piece2 = PIECETYTPE.毛坯B;
                            rackUnitData.Piece2Quality = PIECEQUALITY.待检测;
                            rackUnitData.Piece3 = PIECETYTPE.毛坯B;
                            rackUnitData.Piece3Quality = PIECEQUALITY.待检测;
                            rackUnitData.Piece4 = PIECETYTPE.毛坯B;
                            rackUnitData.Piece4Quality = PIECEQUALITY.待检测;
                            rackUnitData.Lock = false;
                        }
                        break;
                    case 2://A料空盘
                        {
                            rackUnitData.Tray = TRAYTYPE.料盘A;
                            rackUnitData.TrayVolume = 4;
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
                    case 3://B料空盘
                        {
                            rackUnitData.Tray = TRAYTYPE.料盘B;
                            rackUnitData.TrayVolume = 4;
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
                    case 4://空仓
                        {
                            rackUnitData.Tray = TRAYTYPE.空;
                            rackUnitData.TrayVolume = 4;
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
                }

                rackchanged[no] = true;
                no = no + 1;
                OnRackAction(false, no);
            }

        }

        private void BtnInCansl_Click(object sender, EventArgs e)
        {
            if (racktaskstate != RackTaskState.任务正在取消)
            {
                rukuflag = false;
                rukumanu = false;
                rukustatus = MANUALIN.结束;
                BtnManIn.Enabled = true;
                BtnManIn.Text = "入库";
                BtnManIn.BackColor = Color.DarkTurquoise;
                BtnInCansl.Enabled = false;
            }

            if (unitDatatemp != null)
            {
                PushMessage(string.Format("仓位{0}取消手动入库", unitDatatemp.NO), Color.Black);
                var rackdata = listRackData.Find(s => s.NO == unitDatatemp.NO);
                rackdata.Lock = false;
                unitDatatemp = null;
                int curtaskno = 0;
                if (CurNO != 0)
                {
                    curtaskno = CurNO;
                    CurNO = 0;
                    rukumanu = false;
                }
                CurNOLock = 0;
                rackchanged[curtaskno - 1] = true;
            }

            racktaskstate = RackTaskState.任务正在取消;

        }

        private void BtnOutCansl_Click(object sender, EventArgs e)
        {
            if (racktaskstate != RackTaskState.任务正在取消)
            {
                chukuflag = false;
                chukumanu = false;
                chukustatus = MANUALOUT.结束;
                BtnManOut.Enabled = true;
                BtnManOut.Text = "出库";
                BtnOutCansl.Enabled = false;
            }

            if (unitDatatemp != null)
            {
                PushMessage(string.Format("仓位{0}取消手动入库", unitDatatemp.NO), Color.Black);
                var rackdata = listRackData.Find(s => s.NO == unitDatatemp.NO);
                rackdata.Lock = false;
                unitDatatemp = null;
                int curtaskno = 0;
                if (CurNO != 0)
                {
                    curtaskno = CurNO;
                    CurNO = 0;

                    chukumanu = false;
                }

                CurNOLock = 0;

                rackchanged[curtaskno - 1] = true;
            }


            racktaskstate = RackTaskState.任务正在取消;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            BtnCansl_Click(sender, e);
        }

        private void richTextBox_Msg_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rich = sender as RichTextBox;
            rich.SelectionStart = rich.Text.Length;
            rich.ScrollToCaret();
        }
        private void PushMessage(string Msg, Color color)
        {
            richTextBox_Msg.InvokeEx(c =>
            {
                string Message = Msg + "     " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n";
                richTextBox_Msg.SelectionColor = color;
                richTextBox_Msg.AppendText(Message);
            });
        }


    }

    //public partial class StockBinForm : Form
    //{
    //    private List<RackUnit> rackunitlist = new List<RackUnit>();


    //    AutoSizeFormClass asc = new AutoSizeFormClass();
    //    public const string ConnectStr = "Server=127.0.0.1;User Id=HNC;Database=mediandatabase;password=hnc123;Allow User Variables=true";

    //    public static PublishNewTask publishTask;
    //    public TaskReceiver taskReceiver;
    //    public static int taskID;

    //    private int dt4111Value = 0; //记录dt1111的值
    //    private int dt4111Temp = 0;//与dt4111Value配合使用，获取dt4111的值改变（上升沿或下降沿）

    //    private int dt4109Value = 0; //记录dt4109的值
    //    private int dt4109Temp = 0;//与dt4109Value配合使用，获取dt4109的值改变（上升沿或下降沿）
    //    /// <summary>
    //    /// RFID读类型上告仿真软件
    //    /// </summary>
    //    public static event RfidForm.SimulationEventHandle RFIDReadType2;
    //    public void OnRFIDReadType(string type, int RFIDNO)
    //    {
    //        if (RFIDReadType2 != null)
    //        {
    //            RFIDReadType2(this, type, RFIDNO);
    //        }
    //    }

    //    public static string shangliaoType;

    //    public StockBinForm()
    //    {
    //        InitializeComponent();
    //        int width = this.panel1.Width / 12 ;
    //        int heigth = this.panel1.Height / 8 ;

    //        publishTask = new PublishNewTask(); //监控新任务
    //        taskReceiver = new TaskReceiver(ConnectStr); //接收新任务
    //        publishTask.NewTask += taskReceiver.ExcuteTask;           

    //        for (int i = 0; i < 12; i++) 
    //        {
    //            for (int j = 0; j < 8; j++) 
    //            {
    //                Button btn = new Button();
    //                btn.Size = new Size(width - 5, heigth - 5);
    //                btn.Location = new Point(width * i + 5, heigth * j + 5);
    //                btn.Text = ((8-j)+8*i).ToString();
    //                btn.TabIndex = (8 - j) + 8 * i + 300;
    //                btn.BackgroundImageLayout = ImageLayout.Stretch;
    //                btn.Enabled = false;
    //                this.panel1.Controls.Add(btn);
    //            }
    //        }

    //        try
    //        {
    //            string maxID = ExcuteDBAction.FindMaxTaskID(ConnectStr).Result;
    //            taskID = int.Parse(maxID) + 1;
    //            System.Threading.Thread.Sleep(200);

    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBox.Show(ex.ToString());
    //            richTextBox_Msg.AppendText("数据库未连接");
    //        }

    //        timer1.Interval = 2000;
    //        timer1.Enabled = true;

    //    }

    //    private string RFIDReadType(int index, ref string ReadUID)
    //    {
    //        try
    //        {
    //            byte[] UID = new byte[9];
    //            byte blockStart = 0, blockCnt = 2;
    //            Antenna_enum ant = Antenna_enum.ANT_1;
    //            Opcode_enum Opcode = new Opcode_enum();
    //            byte[] datas = new byte[9];
    //            int pos = 0;
    //            int length = datas.Length - 1;
    //            byte ReaderID = 0x00;//读写器ID
    //            if (MainForm.SGreader[index].Inventory(ReaderID, ref UID) == Status_enum.SUCCESS)
    //            {
    //                ReadUID = ConvertMethod.ByteToHexString(UID, pos, length);
    //            }

    //            byte len = 0;
    //            if (Status_enum.SUCCESS == MainForm.SGreader[index].ReadMBlock(0, Opcode, UID, blockStart, blockCnt, ref datas, ref len, ant))
    //            {
    //                //获取标签内存
    //                string str = tool.ByteToHexString(datas, pos, length);
    //                //获取工件类型
    //                string type = str.Substring(2, 1); //工件类型：6表示前端盖，7表示后端盖，8表示轴类
    //                return type;
    //            }
    //            else
    //            {
    //                return null;
    //            }
    //        }
    //        catch (Exception)
    //        {
    //            return null;
    //        }
    //    }

    //private async void StockBinForm_Load(object sender, EventArgs e)
    //{
    //    RfidForm.RukuAction += RfidForm_RukuAction;
    //    await ExcuteDBAction.ExecuteStartEvent(ConnectStr);
    //    await ExcuteDBAction.UnusualExecutedToCheck(ConnectStr);
    //}

    //async void RfidForm_RukuAction(object sender)
    //{
    //    string msg = await ExcuteDBAction.WriteToPlcdt(ConnectStr, "dt4114_i", 1);
    //}

    //private void StockBinForm_SizeChanged(object sender, EventArgs e)
    //{
    //    asc.controlAutoSize(this);
    //}

    //    private bool hasDT4111Changed()
    //    {
    //        if (dt4111Temp == dt4111Value)
    //        {
    //            return false;
    //        }
    //        else
    //        {
    //            dt4111Temp = dt4111Value;
    //            return true;
    //        }
    //    }

    //    private bool hasDT4109Changed()
    //    {
    //        if (dt4109Temp == dt4109Value)
    //        {
    //            return false;
    //        }
    //        else
    //        {
    //            dt4109Temp = dt4109Value;
    //            return true;
    //        }
    //    }

    //    public bool ShowRFIDConnectstatus(int index)
    //    {
    //        bool connect;
    //        ConnectStatusEnum rfidstatus = MainForm.SGreader[index].ConnectStatus;
    //        if (rfidstatus == ConnectStatusEnum.CONNECTED)
    //        {
    //            connect = true;
    //        }
    //        else
    //        {
    //            connect = false;
    //        }
    //        return connect;
    //    }

    //    // bool 
    //    private async void timer1_Tick(object sender, EventArgs e)
    //    {
    //        timer1.Stop();
    //        try
    //        {
    //            await LayoutStochBinForm.LayoutButton(this.panel1);

    //            dt4111Value = await ExcuteDBAction.ReadFromPldt(ConnectStr, "dt4111_o");

    //            int dt4113Value = await ExcuteDBAction.ReadFromPldt(ConnectStr, "dt4113_o");
    //            if (dt4113Value == 1)
    //            {
    //                string msg = await ExcuteDBAction.WriteToPlcdt(ConnectStr, "dt4112_i", 0);
    //                string msg2 = await ExcuteDBAction.WriteToPlcdt(ConnectStr, "dt4113_i", 0);
    //            }

    //            if (hasDT4111Changed())
    //            {
    //                if (dt4111Value == 1)
    //                {
    //                    string initType = string.Empty;
    //                    string readuid = null;
    //                    string workpieceid = null;

    //                    string[] Attributesstr = new String[m_xmlDociment.Default_Attributes_RFID.Length];
    //                    Attributesstr[13] = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_RFID, (int)RfidForm.IDENTIFY._11_RFID1, m_xmlDociment.Default_Attributes_RFID[13]);//读写器IP地址
    //                    Attributesstr[14] = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_RFID, (int)RfidForm.IDENTIFY._11_RFID1, m_xmlDociment.Default_Attributes_RFID[14]);//读写器端口

    //                    bool Rfidflag = MainForm.SGreader[(int)RfidForm.IDENTIFY._11_RFID1].Connect(Attributesstr[13], int.Parse(Attributesstr[14]));
    //                    if (!Rfidflag)
    //                    {
    //                        Thread.Sleep(50);
    //                        Rfidflag = MainForm.SGreader[(int)RfidForm.IDENTIFY._11_RFID1].Connect(Attributesstr[13], int.Parse(Attributesstr[14]));
    //                        if (!Rfidflag)
    //                        {
    //                            richTextBox_Msg.InvokeEx(c =>
    //                            {
    //                                richTextBox_Msg.Clear();
    //                                richTextBox_Msg.AppendText("入库RFID连接失败！检查RFID是否通电或IP和端口是否正确。");
    //                            });
    //                            throw new Exception("dddddddddddd");
    //                        }
    //                    }

    //                    string type = RFIDReadType((int)RfidForm.IDENTIFY._11_RFID1, ref readuid);
    //                    if (MainForm.simulationflag)
    //                        OnRFIDReadType(type, (int)simulationMessage.DDJ_ANO.DDJ_dt4116);
    //                    if (type == "6")
    //                    {
    //                        Thread.Sleep(100);
    //                        bool initFront = RFIDInit("3361100200000000", (int)RfidForm.IDENTIFY._11_RFID1);
    //                        initType = MType.Front.ToString();
    //                        workpieceid = MainForm.Mesuser.CreateWorkpieceProcess(MainForm.LocationID, MainForm.Mesuser.WorkpieceIDs[(int)WuRuanDataOperate.WPType.QDG],
    //                                      MainForm.Mesuser.ManufactureIDs[(int)WuRuanDataOperate.WPType.QDG]);
    //                        shangliaoType = initType;
    //                    }
    //                    else if (type == "7")
    //                    {
    //                        Thread.Sleep(100);
    //                        bool initblack = RFIDInit("3371100200000000", (int)RfidForm.IDENTIFY._11_RFID1);
    //                        initType = MType.Tback.ToString();
    //                        workpieceid = MainForm.Mesuser.CreateWorkpieceProcess(MainForm.LocationID, MainForm.Mesuser.WorkpieceIDs[(int)WuRuanDataOperate.WPType.HQG],
    //                                      MainForm.Mesuser.ManufactureIDs[(int)WuRuanDataOperate.WPType.HQG]);
    //                        shangliaoType = initType;
    //                    }
    //                    else if (type == "8")
    //                    {
    //                        Thread.Sleep(100);
    //                        bool initRollo = RFIDInit("3381100200000000", (int)RfidForm.IDENTIFY._11_RFID1);
    //                        initType = MType.Rollo.ToString();
    //                        workpieceid = MainForm.Mesuser.CreateWorkpieceProcess(MainForm.LocationID, MainForm.Mesuser.WorkpieceIDs[(int)WuRuanDataOperate.WPType.ZL],
    //                                      MainForm.Mesuser.ManufactureIDs[(int)WuRuanDataOperate.WPType.ZL]);
    //                        shangliaoType = initType;
    //                    }
    //                    else
    //                    {
    //                        richTextBox_Msg.InvokeEx(c => {
    //                            richTextBox_Msg.Clear();
    //                            richTextBox_Msg.AppendText("RFID初始化失败");
    //                        });
    //                        //string msg = await ExcuteDBAction.WriteToPlcdt(ConnectStr, "dt4111_i", 0);
    //                        throw new Exception("dddddddddddd");
    //                    }
    //                    Console.WriteLine("initType = {0}", initType);
    //                    string positionID = await ExcuteDBAction.FindPosition(ConnectStr);                        
    //                    if (!string.IsNullOrEmpty(positionID) && ExcuteDBAction.PositonCanConvertToInt(positionID))
    //                    {
    //                        bool canLoad = await ExcuteDBAction.CanLoadToStock(ConnectStr, positionID);
    //                        if (canLoad)
    //                        {
    //                            Agv_Task newTask = new Agv_Task
    //                            {
    //                                ID = taskID.ToString(),
    //                                P1 = positionID,
    //                                P2 = "100",
    //                                P3 = initType + "," + MQuality.Blank.ToString() + "," + "nonee",
    //                                Priority = 2,
    //                                Status = 0
    //                            };
    //                            taskID++;
    //                            string msg1 = await ExcuteDBAction.WriteToPlcdt(ConnectStr, "dt4112_i", 1);
    //                            publishTask.NewTaskHasComed(newTask);

    //                            if (workpieceid != null && readuid != null)
    //                            {
    //                                if (MainForm.Mesuser.dic_WorkpieceprocessIDs.ContainsKey(readuid))
    //                                {
    //                                    MainForm.Mesuser.dic_WorkpieceprocessIDs.Remove(readuid);
    //                                }
    //                                //添加键值对UID-ID
    //                                try
    //                                {
    //                                    MainForm.Mesuser.dic_WorkpieceprocessIDs.Add(readuid, workpieceid);
    //                                }
    //                                catch (Exception ex)
    //                                {
    //                                    Console.WriteLine(ex.Message + "line:" + ex.StackTrace);
    //                                }
    //                            }
    //                        }

    //                    }
    //                    else
    //                    {
    //                        richTextBox_Msg.InvokeEx(c => {
    //                            richTextBox_Msg.Clear();
    //                            richTextBox_Msg.AppendText("上料故障：未找到上料位置");
    //                        });
    //                    }
    //                    if (MainForm.simulationflag)
    //                        OnRFIDReadType("0", (int)simulationMessage.DDJ_ANO.DDJ_dt4116);
    //                }
    //            }
    //            //允许入库
    //            dt4109Value = await ExcuteDBAction.ReadFromPldt(ConnectStr, "dt4109_o");
    //            if (hasDT4109Changed())
    //            {
    //                if (dt4109Value == 1)
    //                {
    //                    Console.WriteLine(DateTime.Now.ToString() + " dt4109变1");
    //                    int type = 0;
    //                    int quality = 0;
    //                    MacDataService.GetInstance().HNC_GetRegValue((int)HNCAPI.HncRegType.REG_TYPE_B, 30, out type, SCADA.MainForm.plc_dbNo);
    //                    MacDataService.GetInstance().HNC_GetRegValue((int)HNCAPI.HncRegType.REG_TYPE_B, 31, out quality, SCADA.MainForm.plc_dbNo);
    //                    string inPositionId = await ExcuteDBAction.FindPosition(ConnectStr);
    //                    if (!string.IsNullOrEmpty(inPositionId) && ExcuteDBAction.PositonCanConvertToInt(inPositionId))
    //                    {
    //                        /*string MetrialType = string.Empty;
    //                        string MetrialQuality = string.Empty;
    //                        if (type == 1)
    //                        {
    //                            MetrialType = MType.Front.ToString();
    //                        }
    //                        else if (type == 2)
    //                        {
    //                            MetrialType = MType.Tback.ToString();
    //                        }
    //                        else if (type == 3)
    //                        {
    //                            MetrialType = MType.Rollo.ToString();
    //                        }
    //                        if (quality == 1)
    //                        {
    //                            MetrialQuality = MQuality.Worth.ToString();
    //                        }
    //                        else if (quality == 2)
    //                        {
    //                            MetrialQuality = MQuality.Waste.ToString();
    //                        }*/
    //                        bool flag = await ExcuteDBAction.WarehousinghasnotSend(ConnectStr, inPositionId);
    //                        Console.WriteLine("flag = {0},time={1}, MetrialType={2}, MetrialQuality={3}", flag, DateTime.Now.ToString(), RfidForm.MetrialType, RfidForm.MetrialQuality);
    //                        if (flag && RfidForm.MetrialType != string.Empty && RfidForm.MetrialQuality != null)
    //                        {
    //                            Agv_Task newtask = new Agv_Task
    //                            {
    //                                ID = taskID.ToString(),
    //                                P1 = inPositionId,
    //                                P2 = "103",
    //                                P3 = RfidForm.MetrialType + "," + RfidForm.MetrialQuality + "," + "nonee",
    //                                Priority = 1,
    //                                Status = 0
    //                            };
    //                            taskID++;
    //                            publishTask.NewTaskHasComed(newtask);
    //                            MacDataService.GetInstance().HNC_RegSetValue((int)HncRegType.REG_TYPE_B, 30, 0, MainForm.plc_dbNo);
    //                            MacDataService.GetInstance().HNC_RegSetValue((int)HncRegType.REG_TYPE_B, 31, 0, MainForm.plc_dbNo);
    //                            await ExcuteDBAction.WriteToPlcdt(ConnectStr, "dt4114_i", 0);
    //                            RfidForm.MetrialQuality = null;
    //                            RfidForm.MetrialType = null;
    //                        }
    //                        else
    //                        {
    //                            richTextBox_Msg.InvokeEx(c => {
    //                                richTextBox_Msg.AppendText("\n入库错误：物料类型或质量有误,type = " + RfidForm.MetrialType + ",quality = " + RfidForm.MetrialQuality + ",flag=" + flag + DateTime.Now.ToString());
    //                            });
    //                        }
    //                    }
    //                    else
    //                    {
    //                        richTextBox_Msg.InvokeEx(c => {
    //                            richTextBox_Msg.Clear();
    //                            richTextBox_Msg.AppendText("入库错误：没有找到位置");
    //                        });
    //                    }
    //                }
    //            }

    //            //下单
    //            while(UserOrderTask.orderqueue.Count>0)
    //            {
    //                Tuple<List<string>,string, string, string> order = UserOrderTask.orderqueue.Peek();
    //                string type = order.Item3;
    //                int count = int.Parse(order.Item4);
    //                //List<string>postionList = await ExcuteDBAction.FindMetriales(ConnectStr,type,count);
    //                List<string> positionList = order.Item1;
    //                int oderNumber = int.Parse(order.Item2);

    //                if(positionList!=null)
    //                {
    //                    UserOrderTask.orderqueue.Dequeue();
    //                    for(int i=0;i<positionList.Count;i++)
    //                    {
    //                        Agv_Task newTask = new Agv_Task
    //                        {
    //                            ID = taskID.ToString(),
    //                            P1 = positionList[i],
    //                            P2 = Convert.ToInt32(CommandCode.MetrialDelivery).ToString(),
    //                            P3 = type + ",Blank,Nonee",
    //                            Priority = 2,
    //                            Status = 0
    //                        };
    //                        lock(this)
    //                        {
    //                            taskID++;
    //                        }                            
    //                        publishTask.NewTaskHasComed(newTask);
    //                    }
    //                }                    
    //            }
    //        }
    //        catch(Exception ex)
    //        {
    //            string s = ex.ToString();
    //            timer1.Enabled = true;
    //        }

    //        timer1.Start();
    //    }

    //    private bool RFIDInit(string initRFID, int index)
    //    {
    //        Opcode_enum Opcode = new Opcode_enum();
    //        byte ReaderID = 0x00;//读写器ID
    //        byte blockStart = 0, blockCnt = 2;
    //        byte[] UID = new byte[9];
    //        byte BlockSize = 4;//块大小   
    //        string tmpInit = initRFID;

    //        byte[] BlockDatas = tool.HexStringToByteArray(tmpInit);

    //        if (Status_enum.SUCCESS == MainForm.SGreader[index].WriteMBlock(ReaderID, Opcode, UID, blockStart, blockCnt, BlockSize, BlockDatas))
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }

    //    private async void Btn_ClearAll_Click(object sender, EventArgs e)
    //    {
    //        DialogResult MsgboxResult;
    //        MsgboxResult = MessageBox.Show("请确保实际立体库已无料盘,否则清空数据后再次运行会出错！","提示", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);
    //        if (MsgboxResult == System.Windows.Forms.DialogResult.Yes)
    //            await ExcuteDBAction.ClearAllStatus(ConnectStr);
    //    }

    //    /// <summary>
    //    /// 上料
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private async void Btn_Blank_IN_Click(object sender, EventArgs e)
    //    {
    //        try
    //        {
    //            string positionID = await ExcuteDBAction.FindPosition(ConnectStr);
    //            bool canLoad = await ExcuteDBAction.CanLoadToStock(ConnectStr, positionID);
    //            if(canLoad && !string.IsNullOrEmpty(positionID)&&ExcuteDBAction.PositonCanConvertToInt(positionID))
    //            {                    
    //                string tempP3 = string.Empty;
    //                //string tempP2 = string.Empty;
    //                //int tempPriority = 0;
    //                if(sender==Btn_ClearAll)
    //                {                        
    //                    tempP3 = MType.Front.ToString() + "," + MQuality.Blank + "," + "Nonee";
    //                    bool initOk = RFIDInit("3361100200000000", (int)RfidForm.IDENTIFY._11_RFID1);
    //                    if (!initOk)
    //                    {
    //                        MessageBox.Show("RFID初始化前端盖失败！");
    //                        return;
    //                    }
    //                    //rfid初始化,前端盖
    //                }
    //                else if(sender == Btn_ErrortoCheck)
    //                {
    //                    tempP3 = MType.Tback.ToString() + "," + MQuality.Blank + "," + "Nonee";
    //                    bool initOk =  RFIDInit("3371100200000000", (int)RfidForm.IDENTIFY._11_RFID1);
    //                    if (!initOk)
    //                    {
    //                        MessageBox.Show("RFID初始化后端盖失败！");
    //                        return;
    //                    }
    //                    //rfid初始化，后端盖
    //                }
    //                else if(sender == Btn_RolloBlank_IN)
    //                {
    //                    tempP3 = MType.Rollo.ToString() + "," + MQuality.Blank + "," + "Nonee";
    //                    bool initOk = RFIDInit("3381100200000000", (int)RfidForm.IDENTIFY._11_RFID1);
    //                    if (!initOk)
    //                    {
    //                        MessageBox.Show("RFID初始化轴类失败！");
    //                        return;
    //                    }
    //                    //rfid初始化，轴类
    //                }

    //                Agv_Task task = new Agv_Task
    //                {
    //                    ID = taskID.ToString(),
    //                    P1 = positionID,
    //                    P2 = Convert.ToInt32(CommandCode.BlankInput).ToString(),
    //                    P3 = tempP3,
    //                    Priority = 2,
    //                    Status = 0

    //                };
    //                taskID++;
    //                publishTask.NewTaskHasComed(task);
    //                MessageBox.Show("下料任务已发送:库位号" + positionID + "");
    //            }
    //            else
    //            {
    //                MessageBox.Show("请先执行完当前上料任务");
    //            }
    //        }
    //        catch(Exception ex)
    //        {
    //            MessageBox.Show(ex.ToString());
    //        }
    //    }

    //    /// <summary>
    //    /// 前端盖下料(一次下一个)
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private async void Btn_Front_Out_Click(object sender, EventArgs e)
    //    {
    //        try
    //        {
    //            List<string> positionidlist = new List<string>();
    //            string tempP3 = string.Empty;
    //            bool positionIsOk = false;
    //            string MetrialType = string.Empty;
    //            string Quality = string.Empty;
    //            if (sender == Btn_FrontBlank_Out)
    //            {
    //                MetrialType = MType.Front.ToString();
    //                Quality = MQuality.Blank.ToString();
    //                positionidlist = await ExcuteDBAction.FindMetrialPostitionList(ConnectStr, MetrialType, Quality);
    //                if (positionidlist.Count == 0)
    //                {
    //                    //richTextBox_Msg.Clear();
    //                    richTextBox_Msg.AppendText("下料故障：找不到前端盖毛坯物料!\r\n");
    //                    return;
    //                }
    //            }
    //            else if (sender == Btn_FrontWorth_Out)
    //            {
    //                MetrialType = MType.Front.ToString();
    //                Quality = MQuality.Worth.ToString();
    //                positionidlist = await ExcuteDBAction.FindMetrialPostitionList(ConnectStr, MetrialType, Quality);
    //                if (positionidlist.Count == 0)
    //                {
    //                    //richTextBox_Msg.Clear();
    //                    richTextBox_Msg.AppendText("下料故障：找不到前端盖合格物料!\r\n");
    //                    return;
    //                }
    //            }
    //            else if (sender == Btn_FrontWaste_Out)
    //            {
    //                MetrialType = MType.Front.ToString();
    //                Quality = MQuality.Waste.ToString();
    //                positionidlist = await ExcuteDBAction.FindMetrialPostitionList(ConnectStr, MetrialType, Quality);
    //                if (positionidlist.Count == 0)
    //                {
    //                    //richTextBox_Msg.Clear();
    //                    richTextBox_Msg.AppendText("下料故障：找不到前端盖不合格物料!\r\n");
    //                    return;
    //                }
    //            }
    //            int count = 0;
    //            for (int i = 0; i < positionidlist.Count; i++)
    //            {
    //                string positionid = positionidlist[i];
    //                positionIsOk = await ExcuteDBAction.PositionIsOccupied(ConnectStr, positionid);
    //                if (!string.IsNullOrEmpty(positionid) && ExcuteDBAction.PositonCanConvertToInt(positionid) && positionIsOk)
    //                {
    //                    tempP3 = MetrialType + "," + Quality + "," + "Nonee";
    //                    count++;
    //                }
    //                if(count == 1)
    //                {
    //                    Agv_Task task = new Agv_Task
    //                    {
    //                        ID = taskID.ToString(),
    //                        P1 = positionid,
    //                        P2 = Convert.ToInt32(CommandCode.MetrialOut).ToString(),
    //                        P3 = tempP3,
    //                        Priority = 1,
    //                        Status = 0

    //                    };
    //                    taskID++;
    //                    publishTask.NewTaskHasComed(task);
    //                    if (sender == Btn_FrontBlank_Out)
    //                    {
    //                        richTextBox_Msg.AppendText("前端盖毛坯下料任务已发送:库位号" + positionid + "\r\n");
    //                    }
    //                    else if (sender == Btn_FrontWorth_Out)
    //                    {
    //                        richTextBox_Msg.AppendText("前端盖合格下料任务已发送:库位号" + positionid + "\r\n");
    //                    }
    //                    else if (sender == Btn_FrontWaste_Out)
    //                    {
    //                        richTextBox_Msg.AppendText("前端盖不合格下料任务已发送:库位号" + positionid + "\r\n");
    //                    }
    //                    return;
    //                }
    //            }

    //            if (sender == Btn_FrontBlank_Out)
    //            {
    //                if (count == 0)
    //                {
    //                    richTextBox_Msg.AppendText("前端盖毛坯物料任务已全部下完!\r\n");
    //                }
    //            }
    //            else if (sender == Btn_FrontWorth_Out)
    //            {
    //                if (count == 0)
    //                {
    //                    richTextBox_Msg.AppendText("前端盖合格物料任务已全部下完!\r\n");
    //                } 
    //            }
    //            else if (sender == Btn_FrontWaste_Out)
    //            {
    //                if (count == 0)
    //                {
    //                    richTextBox_Msg.AppendText("前端盖不合格物料任务已全部下完!\r\n");
    //                }  
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBox.Show("Btn_Front_Out failed:" + ex.ToString());
    //        }
    //    }

    //    /// <summary>
    //    /// 后端盖下料(一次下一个)
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private async void Btn_Tback_Out_Click(object sender, EventArgs e)
    //    {
    //        try
    //        {
    //            List<string> positionidlist = new List<string>();
    //            string tempP3 = string.Empty;
    //            bool positionIsOk = false;
    //            string MetrialType = string.Empty;
    //            string Quality = string.Empty;
    //            if (sender == Btn_TbackBlank_Out)
    //            {
    //                MetrialType = MType.Tback.ToString();
    //                Quality = MQuality.Blank.ToString();
    //                positionidlist = await ExcuteDBAction.FindMetrialPostitionList(ConnectStr, MetrialType, Quality);
    //                if (positionidlist.Count == 0)
    //                {
    //                    //richTextBox_Msg.Clear();
    //                    richTextBox_Msg.AppendText("下料故障：找不到后端盖毛坯物料!\r\n");
    //                    return;
    //                }
    //            }
    //            else if (sender == Btn_TbackWorth_Out)
    //            {
    //                MetrialType = MType.Tback.ToString();
    //                Quality = MQuality.Worth.ToString();
    //                positionidlist = await ExcuteDBAction.FindMetrialPostitionList(ConnectStr, MetrialType, Quality);
    //                if (positionidlist.Count == 0)
    //                {
    //                    //richTextBox_Msg.Clear();
    //                    richTextBox_Msg.AppendText("下料故障：找不到后端盖合格物料!\r\n");
    //                    return;
    //                }
    //            }
    //            else if (sender == Btn_TbackWaste_Out)
    //            {
    //                MetrialType = MType.Tback.ToString();
    //                Quality = MQuality.Waste.ToString();
    //                positionidlist = await ExcuteDBAction.FindMetrialPostitionList(ConnectStr, MetrialType, Quality);
    //                if (positionidlist.Count == 0)
    //                {
    //                    //richTextBox_Msg.Clear();
    //                    richTextBox_Msg.AppendText("下料故障：找不到后端盖不合格物料!\r\n");
    //                    return;
    //                }
    //            }
    //            int count = 0;
    //            for (int i = 0; i < positionidlist.Count; i++)
    //            {
    //                string positionid = positionidlist[i];
    //                positionIsOk = await ExcuteDBAction.PositionIsOccupied(ConnectStr, positionid);
    //                if (!string.IsNullOrEmpty(positionid) && ExcuteDBAction.PositonCanConvertToInt(positionid) && positionIsOk)
    //                {
    //                    tempP3 = MetrialType + "," + Quality + "," + "Nonee";
    //                    count++;
    //                }
    //                if (count == 1)
    //                {
    //                    Agv_Task task = new Agv_Task
    //                    {
    //                        ID = taskID.ToString(),
    //                        P1 = positionid,
    //                        P2 = Convert.ToInt32(CommandCode.MetrialOut).ToString(),
    //                        P3 = tempP3,
    //                        Priority = 1,
    //                        Status = 0

    //                    };
    //                    taskID++;
    //                    publishTask.NewTaskHasComed(task);
    //                    if (sender == Btn_TbackBlank_Out)
    //                    {
    //                        richTextBox_Msg.AppendText("后端盖毛坯下料任务已发送:库位号" + positionid + "\r\n");
    //                    }
    //                    else if (sender == Btn_TbackWorth_Out)
    //                    {
    //                        richTextBox_Msg.AppendText("后端盖合格下料任务已发送:库位号" + positionid + "\r\n");
    //                    }
    //                    else if (sender == Btn_TbackWaste_Out)
    //                    {
    //                        richTextBox_Msg.AppendText("后端盖不合格下料任务已发送:库位号" + positionid + "\r\n");
    //                    }
    //                    return;
    //                }
    //            }

    //            if (sender == Btn_TbackBlank_Out)
    //            {
    //                if (count == 0)
    //                {
    //                    richTextBox_Msg.AppendText("后端盖毛坯物料任务已全部下完!\r\n");
    //                }
    //            }
    //            else if (sender == Btn_TbackWorth_Out)
    //            {
    //                if (count == 0)
    //                {
    //                    richTextBox_Msg.AppendText("后端盖合格物料任务已全部下完!\r\n");
    //                }
    //            }
    //            else if (sender == Btn_TbackWaste_Out)
    //            {
    //                if (count == 0)
    //                {
    //                    richTextBox_Msg.AppendText("后端盖不合格物料任务已全部下完!\r\n");
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBox.Show("Btn_Tback_Out failed:" + ex.ToString());
    //        }
    //    }

    //    /// <summary>
    //    /// 轴类下料(一次下完一个)
    //    /// </summary>
    //    /// <param name="sender"></param>
    //    /// <param name="e"></param>
    //    private async void Btn_Rollo_Out_Click(object sender, EventArgs e)
    //    {
    //        try
    //        {
    //            List<string> positionidlist = new List<string>();
    //            string tempP3 = string.Empty;
    //            bool positionIsOk = false;
    //            string MetrialType = string.Empty;
    //            string Quality = string.Empty;
    //            if (sender == Btn_RolloBlank_Out)
    //            {
    //                MetrialType = MType.Rollo.ToString();
    //                Quality = MQuality.Blank.ToString();
    //                positionidlist = await ExcuteDBAction.FindMetrialPostitionList(ConnectStr, MetrialType, Quality);
    //                if (positionidlist.Count == 0)
    //                {
    //                    //richTextBox_Msg.Clear();
    //                    richTextBox_Msg.AppendText("下料故障：找不到轴类毛坯物料!\r\n");
    //                    return;
    //                }
    //            }
    //            else if (sender == Btn_RolloWorth_Out)
    //            {
    //                MetrialType = MType.Rollo.ToString();
    //                Quality = MQuality.Worth.ToString();
    //                positionidlist = await ExcuteDBAction.FindMetrialPostitionList(ConnectStr, MetrialType, Quality);
    //                if (positionidlist.Count == 0)
    //                {
    //                    //richTextBox_Msg.Clear();
    //                    richTextBox_Msg.AppendText("下料故障：找不到轴类合格物料!\r\n");
    //                    return;
    //                }
    //            }
    //            else if (sender == Btn_RolloWaste_Out)
    //            {
    //                MetrialType = MType.Rollo.ToString();
    //                Quality = MQuality.Waste.ToString();
    //                positionidlist = await ExcuteDBAction.FindMetrialPostitionList(ConnectStr, MetrialType, Quality);
    //                if (positionidlist.Count == 0)
    //                {
    //                    //richTextBox_Msg.Clear();
    //                    richTextBox_Msg.AppendText("下料故障：找不到轴类不合格物料!\r\n");
    //                    return;
    //                }
    //            }
    //            int count = 0;
    //            for (int i = 0; i < positionidlist.Count; i++)
    //            {
    //                string positionid = positionidlist[i];
    //                positionIsOk = await ExcuteDBAction.PositionIsOccupied(ConnectStr, positionid);
    //                if (!string.IsNullOrEmpty(positionid) && ExcuteDBAction.PositonCanConvertToInt(positionid) && positionIsOk)
    //                {
    //                    tempP3 = MetrialType + "," + Quality + "," + "Nonee";
    //                    count++;
    //                }

    //                if (count == 1)
    //                {
    //                    Agv_Task task = new Agv_Task
    //                    {
    //                        ID = taskID.ToString(),
    //                        P1 = positionid,
    //                        P2 = Convert.ToInt32(CommandCode.MetrialOut).ToString(),
    //                        P3 = tempP3,
    //                        Priority = 1,
    //                        Status = 0

    //                    };
    //                    taskID++;
    //                    publishTask.NewTaskHasComed(task);
    //                    if (sender == Btn_RolloBlank_Out)
    //                    {
    //                        richTextBox_Msg.AppendText("轴类毛坯下料任务已发送:库位号" + positionid + "\r\n"); 
    //                    }
    //                    else if (sender == Btn_RolloWorth_Out)
    //                    {
    //                        richTextBox_Msg.AppendText("轴类合格下料任务已发送:库位号" + positionid + "\r\n"); 
    //                    }
    //                    else if (sender == Btn_RolloWaste_Out)
    //                    {
    //                        richTextBox_Msg.AppendText("轴类不合格下料任务已发送:库位号" + positionid + "\r\n");
    //                    }
    //                    return;
    //                }
    //            }

    //            if (sender == Btn_RolloBlank_Out)
    //            {
    //                if (count == 0)
    //                {
    //                    richTextBox_Msg.AppendText("轴类毛坯物料任务已全部下完!\r\n");
    //                }
    //            }
    //            else if (sender == Btn_RolloWorth_Out)
    //            {
    //                if (count == 0)
    //                {
    //                    richTextBox_Msg.AppendText("轴类合格物料任务已全部下完!\r\n");
    //                }
    //            }
    //            else if (sender == Btn_RolloWaste_Out)
    //            {
    //                if (count == 0)
    //                {
    //                    richTextBox_Msg.AppendText("轴类不合格物料任务已全部下完!\r\n");
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            MessageBox.Show("Btn_Rollo_Out failed:" + ex.ToString());
    //        }
    //    }

    //    private void StockBinForm_KeyDown(object sender, KeyEventArgs e)
    //    {
    //        switch (e.KeyCode)
    //        {
    //            case Keys.O:
    //                Btn_ClearAll.Enabled = true;
    //                break;
    //            case Keys.C:
    //                Btn_ClearAll.Enabled = false;
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //    private void richTextBox_Msg_TextChanged(object sender, EventArgs e)
    //    {
    //        richTextBox_Msg.SelectionStart = richTextBox_Msg.Text.Length;
    //        richTextBox_Msg.ScrollToCaret();
    //    }

    //    private void FindErrorRackNoToReset()
    //    {
    //        int errorRackNo = 0;
    //        //根据宏友给的仓位信号赋值给errorRackNo
    //        errorRackNo = ExcuteDBAction.ReadFromPldtQ(ConnectStr, "dt4120_o");
    //        if (errorRackNo > 0)
    //        {
    //            //根据errorRackNo找agv_task表里的对应任务的ID
    //            string ID = ExcuteDBAction.FindErrorRackNoTaskListHeaderID(ConnectStr, errorRackNo);
    //            if (ID != String.Empty)
    //            {
    //                int completeno = 0;
    //                //根据宏友给的取放料完成信号赋值给completeno
    //                completeno = ExcuteDBAction.ReadFromPldtQ(ConnectStr, "dt4119_o");
    //                if (completeno == 2)
    //                {
    //                    //将对应ID的任务status值更新为2，表示堆垛机已完成取放料到仓位的动作
    //                    ExcuteDBAction.UpdateTaskStatusByID(ConnectStr, ID, 2);
    //                }
    //                else
    //                {
    //                    //将对应ID的任务status值更新为1，表示堆垛机未完成取放料到仓位的动作
    //                    string p2 = ExcuteDBAction.GetP2ByID(ConnectStr, ID);
    //                    if (p2 == "102") //下单任务未完成则还原
    //                    {
    //                        ExcuteDBAction.UpdateUnloadStatusByagv_taskID(ConnectStr, ID);
    //                    }
    //                    ExcuteDBAction.UpdateTaskStatusByID(ConnectStr, ID, 1);
    //                }
    //            }
    //            //清信号
    //            ExcuteDBAction.WriteToPlcdtQ(ConnectStr, "dt4120_i", 0);
    //            ExcuteDBAction.WriteToPlcdtQ(ConnectStr, "dt4120_o", 0);
    //            ExcuteDBAction.WriteToPlcdtQ(ConnectStr, "dt4119_i", 0);
    //            ExcuteDBAction.WriteToPlcdtQ(ConnectStr, "dt4119_o", 0);
    //        }
    //    }

    //    private async void Btn_ErrortoCheck_Click(object sender, EventArgs e)
    //    {
    //        Btn_ErrortoCheck.Enabled = false;
    //        ExcuteDBAction.UpdateUnloadStatusByagv_taskP2andstatus(ConnectStr);
    //        ExcuteDBAction.UnexecutedToCheck(ConnectStr);
    //        FindErrorRackNoToReset();
    //        richTextBox_Msg.AppendText("料仓堆垛机故障，将已发等待未执行任务清空!\r\n");
    //        bool res = ExcuteDBAction.FindExecutingTask(ConnectStr);
    //        if (res)
    //        {
    //            await ExcuteDBAction.ExecutingToCheck(ConnectStr);
    //            richTextBox_Msg.AppendText("料仓堆垛机故障，将已发执行中的任务清空!\r\n");
    //        }
    //        res = ExcuteDBAction.FindUnusualExecutedTask(ConnectStr);
    //        if (res)
    //        {
    //            await ExcuteDBAction.UnusualExecutedToCheck(ConnectStr);
    //            richTextBox_Msg.AppendText("料仓堆垛机故障，将已发异常反馈的任务清空!\r\n");
    //        }
    //        Btn_ErrortoCheck.Enabled = true;
    //    }
    //}


    //public class LayoutStochBinForm
    //{
    //    public const string ConnectStr = "Server=127.0.0.1;User Id=HNC;Database=mediandatabase;password=hnc123;Allow User Variables=true";
    //    public static async Task LayoutButton(Panel p)
    //    {
    //        List<Tuple<string, string, string, string>> statusList = await ExcuteDBAction.GetAllStatus(ConnectStr);
    //        foreach (Button btn in p.Controls)
    //        {
    //            foreach (var tempTuple in statusList)
    //            {
    //                if (int.Parse(tempTuple.Item1) == btn.TabIndex - 300)
    //                {
    //                    string key = ExcuteDBAction.ConvertFirstToUpper(tempTuple.Item2) + ExcuteDBAction.ConvertFirstToUpper(tempTuple.Item3);
    //                    btn.InvokeEx(c =>
    //                    {
    //                        btn.BackgroundImage = MetrialMap.MetrialMaps[key];
    //                    });
    //                }
    //            }
    //        }
    //    }
    //}

}
