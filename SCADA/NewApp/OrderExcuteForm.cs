
using HncDataInterfaces;
using Newtonsoft.Json.Linq;
using SCADA.SimensPLC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using System.Xml;
using static SCADA.SimensPLC.WMSPLC;

namespace SCADA.NewApp
{
    public partial class OrderExcuteForm : Form
    {
        public OrderExcuteForm()
        {
            InitializeComponent();
            InitConfig();
            comboBox1.SelectedIndex = 0;
            Task.Run(() => AutoDoWork());
        }

        int Number = 0;
        public static List<WorkOrderData> workOrderTasks = new List<WorkOrderData>();
        public static PositionBind positionBind = new PositionBind();
        private bool TaskRun = false;
        private int testProcesstime = 45;
        private int testChecktime = 33;
        private int process1Pos1count = 0;
        private int process1Pos2count = 0;
        private int process2Pos1count = 0;
        private int process2Pos2count = 0;
        private int checkPos1count = 0;
        private int checkPos2count = 0;
        public static Guid Order_Meter_latest = new Guid();
        public static StockBinForm.RackAction rackAction1;
        public static StockBinForm.RackAction rackAction2;
        public static StockBinForm.RackAction rackAction3;
        public static List<KeyValueData> listKeyValueData = new List<KeyValueData>();

        private void OnRackAction2(bool flag, int rackno)
        {
            if (rackAction2 != null)
            {
                rackAction2(this, flag, rackno);
            }
        }

        private void OnRackAction1(bool flag, int rackno)
        {
            if (rackAction1 != null)
            {
                rackAction1(this, flag, rackno);
            }
        }
        private void OnRackAction3(bool flag, int rackno)
        {
            if (rackAction3 != null)
            {
                rackAction3(this, flag, rackno);
            }
        }
        public void InitConfig()
        {
            InitOrderControl();
            InitDataFromDB();
        }

        private void InitDataFromDB()
        {
            KeyValueData valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.wMSPositionBind);
            positionBind.wMSPositionBind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.aGVBind);
            positionBind.aGVBind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process1Position1Bind);
            positionBind.process1Position1Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process1Position2Bind);
            positionBind.process1Position2Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process2Position1Bind);
            positionBind.process2Position1Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process2Position2Bind);
            positionBind.process2Position2Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process3Position1Bind);
            positionBind.process3Position1Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process3Position2Bind);
            positionBind.process3Position2Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process4Position1Bind);
            positionBind.process4Position1Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process4Position2Bind);
            positionBind.process4Position2Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.check1Position1Bind);
            positionBind.check1Position1Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.check1Position2Bind);
            positionBind.check1Position2Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.check2Position1Bind);
            positionBind.check2Position1Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.check2Position2Bind);
            positionBind.check2Position2Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.fit1Position1Bind);
            positionBind.fit1Position1Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.fit1Position2Bind);
            positionBind.fit1Position2Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.fit2Position1Bind);
            positionBind.fit2Position1Bind = int.Parse(valueData.Value);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.fit2Position2Bind);
            positionBind.fit2Position2Bind = int.Parse(valueData.Value);
        }

        private void UpdatePositionDataToDB()
        {
            KeyValueData valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.wMSPositionBind);
            valueData.Value = positionBind.wMSPositionBind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.aGVBind);
            valueData.Value = positionBind.aGVBind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process1Position1Bind);
            valueData.Value = positionBind.process1Position1Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process1Position2Bind);
            valueData.Value = positionBind.process1Position2Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process2Position1Bind);
            valueData.Value = positionBind.process2Position1Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process2Position2Bind);
            valueData.Value = positionBind.process2Position2Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);

            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process3Position1Bind);
            valueData.Value = positionBind.process3Position1Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process3Position2Bind);
            valueData.Value = positionBind.process3Position2Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);

            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process4Position1Bind);
            valueData.Value = positionBind.process4Position1Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.process4Position2Bind);
            valueData.Value = positionBind.process4Position2Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);

            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.cleanPosition1Bind);
            valueData.Value = positionBind.cleanPosition1Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.cleanPosition2Bind);
            valueData.Value = positionBind.cleanPosition2Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);

            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.check1Position1Bind);
            valueData.Value = positionBind.check1Position1Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.check1Position2Bind);
            valueData.Value = positionBind.check1Position2Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.check2Position1Bind);
            valueData.Value = positionBind.check2Position1Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.check2Position2Bind);
            valueData.Value = positionBind.check2Position2Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);


            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.fit1Position1Bind);
            valueData.Value = positionBind.fit1Position1Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.fit1Position2Bind);
            valueData.Value = positionBind.fit1Position2Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.fit2Position1Bind);
            valueData.Value = positionBind.fit2Position1Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
            valueData = listKeyValueData.Find(s => s.Key == (int)INDEX.fit2Position2Bind);
            valueData.Value = positionBind.fit2Position2Bind.ToString();
            Program.Repo.Update<KeyValueData>(valueData);
        }

        private void AutoDoWork()
        {
            while (true)
            {
                Thread.Sleep(TimeSpan.FromSeconds(2));
                WorkOrderDoing();
                FreshUIData();
                UpdatePositionDataToDB();
                if (LineMainForm.wmsplc.GetOnlineState())
                {
                    InAction();
                    OutAction();
                }
            }
        }

        public void InitOrderControl()
        {
            buttoniniorder.Click += Button1_Click;
            buttonstart.Click += Button2_Click;
            buttonstop.Click += Button3_Click;
            buttondelet.Click += Button4_Click;
            richTextBox1.TextChanged += RichTextBox1_TextChanged;
        }

        private void RichTextBox1_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rich = sender as RichTextBox;
            rich.SelectionStart = rich.Text.Length;
            rich.ScrollToCaret();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            if (TaskRun)
            {
                TaskRun = false;
                buttonstop.Text = "暂停中";
                buttonstop.Enabled = false;
                buttonstop.BackColor = Color.LightPink;
                buttonstart.Enabled = true;
                buttonstart.Text = "启动";
                PushMessage("暂停工单执行.", Color.Black);
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (!TaskRun)
            {
                TaskRun = true;
                buttonstart.Text = "启动中";
                buttonstart.Enabled = false;
                buttonstop.Enabled = true;
                buttonstop.Text = "暂停";
                buttonstop.BackColor = SystemColors.Control;
                PushMessage("启动工单执行.", Color.Black);
            }
        }

        private void FreshUIData()
        {
            if (dataGridView1.RowCount != workOrderTasks.Count)
            {
                dataGridView1.InvokeEx(c =>
                {
                    c.Rows.Clear();
                    if (workOrderTasks.Count > 0)
                        c.Rows.Add(workOrderTasks.Count);
                });
            }
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                UpdateDataGridTask(i, i);
            }
            // UpdatePositionBind();

        }

        private void Button4_Click(object sender, EventArgs e)
        {
            int result;
            if (!int.TryParse(textBox1.Text, out result))
            {
                MessageBox.Show("输入的订单号无法识别！");
                return;
            }

            if (result <= 0)
            {
                MessageBox.Show("输入的订单号为非正数！");
                return;
            }

            if (TaskRun)
            {
                MessageBox.Show("只能在工单暂停状态删除工单！");
                return;
            }

            var order = workOrderTasks.Find(s => s.OrderNO == result && s.IsDeleted == false);
            if (order == null)
            {
                MessageBox.Show("未找到输入的订单号！");
                return;
            }
            //Program.Repo.RealDelete<WorkOrderData>(order);
            Program.Repo.Delete<WorkOrderData>(order);
            workOrderTasks.Remove(order);
            ResetPositionBind(result);
            UpdatePositionDataToDB();
            PushMessage(string.Format("工单号为{0}的工单被删除", result), Color.Black);
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            WorkOrderData workdata = null;
            var list = Program.Repo.GetDelet<WorkOrderData>();
            //var list1 =  Program.Repo.GetOrderByASC< WorkOrderData >( "OrderNo");
            if (list == null || list.Count == 0)
            {
                Number = 0;
            }
            else
            {
                foreach (var t in list)
                {
                    if (Number <= t.OrderNO)
                        Number = t.OrderNO + 1;
                }

            }
            if (Number > 10000)
            {
                Program.Repo.EmptyData<WorkOrderData>();
                Program.Repo.EmptyData<MeterResult>();
                Number = 0;
            }
            if (comboBox1.SelectedIndex == 0)
            {
                workdata = new WorkOrderData
                {
                    OrderNO = Number,
                    Tray = TRAYTYPE.料盘A,
                    OUTNO = 0,
                    Product1 = PRODUCTTYPE.A料,
                    P1Quality = PIECEQUALITY.待检测,
                    Product2 = PRODUCTTYPE.A料,
                    P2Quality = PIECEQUALITY.待检测,
                    Product3 = PRODUCTTYPE.A料,
                    P3Quality = PIECEQUALITY.待检测,
                    Product4 = PRODUCTTYPE.A料,
                    P4Quality = PIECEQUALITY.待检测,
                    ProcessPosition = PROCESSPOSITION.暂无,
                    CheckPosition = CHECKPOSITION.暂无,
                    INNO = 0,
                    OrderState = ORDERSTATE.等待,
                    Updatetime = DateTime.Now.ToString()
                };
            }
            else if (comboBox1.SelectedIndex == 1)
            {
                workdata = new WorkOrderData
                {
                    OrderNO = Number,
                    Tray = TRAYTYPE.料盘B,
                    OUTNO = 0,
                    Product1 = PRODUCTTYPE.B料,
                    P1Quality = PIECEQUALITY.待检测,
                    Product2 = PRODUCTTYPE.B料,
                    P2Quality = PIECEQUALITY.待检测,
                    Product3 = PRODUCTTYPE.B料,
                    P3Quality = PIECEQUALITY.待检测,
                    Product4 = PRODUCTTYPE.B料,
                    P4Quality = PIECEQUALITY.待检测,
                    ProcessPosition = PROCESSPOSITION.暂无,
                    CheckPosition = CHECKPOSITION.暂无,
                    INNO = 0,
                    OrderState = ORDERSTATE.等待,
                    Updatetime = DateTime.Now.ToString()
                };
            }

            if (workdata != null)
            {
                workOrderTasks.Add(workdata);
                Program.Repo.Insert<WorkOrderData>(workdata);
            }
            Number++;
        }

        private void PushMessage(string Msg, Color color)
        {
            richTextBox1.InvokeEx(c =>
            {
                string Message = Msg + "     " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + Environment.NewLine;
                richTextBox1.SelectionColor = color;
                richTextBox1.AppendText(Message);
            });
        }

        private void UpdateDataGridTask(int rowindex, int listindex)
        {
            dataGridView1.InvokeEx(c =>
            {
                c.Rows[rowindex].Cells[0].Value = rowindex + 1;
                c.Rows[rowindex].Cells[1].Value = workOrderTasks[listindex].OrderNO;
                c.Rows[rowindex].Cells[2].Value = workOrderTasks[listindex].Product1.ToString();
                c.Rows[rowindex].Cells[3].Value = workOrderTasks[listindex].OUTNO;
                c.Rows[rowindex].Cells[4].Value = workOrderTasks[listindex].ProcessPosition.ToString();
                c.Rows[rowindex].Cells[5].Value = workOrderTasks[listindex].CleanPosition.ToString();
                c.Rows[rowindex].Cells[6].Value = workOrderTasks[listindex].CheckPosition.ToString();
                c.Rows[rowindex].Cells[7].Value = workOrderTasks[listindex].FitPosition.ToString();
                c.Rows[rowindex].Cells[8].Value = workOrderTasks[listindex].INNO;
                c.Rows[rowindex].Cells[9].Value = workOrderTasks[listindex].OrderState.ToString();
                c.Rows[rowindex].Cells[10].Value = workOrderTasks[listindex].Updatetime;
                c.ClearSelection();
            });
        }

        private void ResetPositionBind(int orderno)
        {
            if (positionBind.wMSPositionBind == orderno)
            {
                positionBind.wMSPositionBind = 0;
            }
            MeasureForm.MeasureOrderNO = 0;
            MeasureForm.MeasurePicecNo = 0;
            if (positionBind.aGVBind == orderno)
            {
                positionBind.aGVBind = 0;
            }

            if (positionBind.process1Position1Bind == orderno)
            {
                positionBind.process1Position1Bind = 0;
                positionBind.process1Position1BindLock = 0;
            }

            if (positionBind.process1Position2Bind == orderno)
            {
                positionBind.process1Position2Bind = 0;
                positionBind.process1Position2BindLock = 0;
            }

            if (positionBind.process2Position1Bind == orderno)
            {
                positionBind.process2Position1Bind = 0;
                positionBind.process2Position1BindLock = 0;
            }

            if (positionBind.process2Position2Bind == orderno)
            {
                positionBind.process2Position2Bind = 0;
                positionBind.process2Position2BindLock = 0;
            }
            if (positionBind.process3Position1Bind == orderno)
            {
                positionBind.process3Position1Bind = 0;
                positionBind.process3Position1BindLock = 0;
            }
            if (positionBind.process3Position2Bind == orderno)
            {
                positionBind.process3Position2Bind = 0;
                positionBind.process3Position2BindLock = 0;
            }
            if (positionBind.process4Position1Bind == orderno)
            {
                positionBind.process4Position1Bind = 0;
                positionBind.process4Position1BindLock = 0;
            }
            if (positionBind.process4Position2Bind == orderno)
            {
                positionBind.process4Position2Bind = 0;
                positionBind.process4Position2BindLock = 0;
            }

            if (positionBind.check1Position1Bind == orderno)
            {
                positionBind.check1Position1Bind = 0;
            }

            if (positionBind.check1Position2Bind == orderno)
            {
                positionBind.check1Position2Bind = 0;
            }

            if (positionBind.check2Position1Bind == orderno)
            {
                positionBind.check2Position1Bind = 0;
            }

            if (positionBind.check2Position2Bind == orderno)
            {
                positionBind.check2Position2Bind = 0;
            }
            if (positionBind.cleanPosition1Bind == orderno)
            {
                positionBind.cleanPosition1Bind = 0;
            }

            if (positionBind.cleanPosition2Bind == orderno)
            {
                positionBind.cleanPosition2Bind = 0;
            }

            if (positionBind.fit2Position1Bind == orderno)
            {
                positionBind.fit2Position1Bind = 0;
            }

            if (positionBind.fit2Position2Bind == orderno)
            {
                positionBind.fit2Position2Bind = 0;
            }
            if (positionBind.fit1Position1Bind == orderno)
            {
                positionBind.fit1Position1Bind = 0;
            }

            if (positionBind.fit1Position2Bind == orderno)
            {
                positionBind.fit1Position2Bind = 0;
            }
        }

        //private void UpdatePositionBind()
        //{
        //    tBwms.InvokeEx(c => { c.Text = positionBind.wMSPositionBind.ToString(); });
        //    tBagv.InvokeEx(c => { c.Text = positionBind.aGVBind.ToString(); });
        //    textBox2.InvokeEx(c => { c.Text = positionBind.process1Position1Bind.ToString(); });
        //    textBox3.InvokeEx(c => { c.Text = positionBind.process1Position2Bind.ToString(); });
        //    textBox4.InvokeEx(c => { c.Text = positionBind.process2Position1Bind.ToString(); });
        //    textBox5.InvokeEx(c => { c.Text = positionBind.process2Position2Bind.ToString(); });
        //    tBcheck1.InvokeEx(c => { c.Text = positionBind.checkPosition1Bind.ToString(); });
        //    tBcheck2.InvokeEx(c => { c.Text = positionBind.checkPosition2Bind.ToString(); });
        //}

        private int state = Convert.ToInt32(ORDERSTATE.结束);
        private int state1 = Convert.ToInt32(StockBinForm.MANUALIN.结束);
        private int state2 = Convert.ToInt32(StockBinForm.MANUALOUT.结束);
        private void WorkOrderDoing()
        {
            if (!TaskRun || dataGridView1.Rows.Count == 0)
                return;

            if (!LineMainForm.controlplc.GetOnlineState())
            {
                PushMessage(string.Format("总控PLC未连接！IP:{0}", LineMainForm.controlplc.IP), Color.Red);
                //return;
            }


            // WorkOrderToProcessDoing();
            WorkOrderToProcessDoing();
            WorkOrderProcessDoing();
            WorkOrderToCleanDoing();
            WorkOrderToFitDoing();
            WorkOrderToWMS();
            // WorkOrderToCheckDoing();
            //WorkOrderToWMSDoing();
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState != ORDERSTATE.等待)
                {
                    if (Convert.ToInt32(workOrderTasks[i].OrderState) != state)
                    {
                        string t = workOrderTasks[i].OrderNO.ToString() + "状态" + workOrderTasks[i].OrderState.ToString();
                        PushMessage(string.Format(t, LineMainForm.controlplc.IP), Color.Red);
                    }
                }

            }
            if (state1 != Convert.ToInt32(StockBinForm.rukustatus))
            {
                state1 = Convert.ToInt32(StockBinForm.rukustatus);
                string t = "入库状态" + StockBinForm.rukustatus.ToString();
                PushMessage(string.Format(t, LineMainForm.controlplc.IP), Color.Red);
            }
            if (state2 != Convert.ToInt32(StockBinForm.chukustatus))
            {
                state2 = Convert.ToInt32(StockBinForm.chukustatus);
                string t = "出库状态" + StockBinForm.chukustatus.ToString();
                PushMessage(string.Format(t, LineMainForm.controlplc.IP), Color.Red);
            }
            // }

            UpdateWorkOrderDataToDB();
        }

        private void UpdateWorkOrderDataToDB()
        {
            foreach (var order in workOrderTasks)
            {
                Program.Repo.Update<WorkOrderData>(order);
            }
        }
        /// <summary>
        /// 从料库出库到加工单元
        /// </summary>
        private void WorkOrderToProcessDoing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;
                if (positionBind.aGVBind != 0)
                {
                    return;
                }
                int stationNo = 0;
                if (workOrderTasks[i].OrderState == ORDERSTATE.等待 && StockBinForm.unitDatatemp == null)
                {

                    bool processpositionisempty = false;

                    if (positionBind.process1Position1Bind == 0 && (positionBind.process1Position1BindLock == 0 || positionBind.process1Position1BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.process1Position1BindLock = workOrderTasks[i].OrderNO;
                        stationNo =(int) StationINDEX.UnitCNC1Station1;
                        processpositionisempty = true;
                    }
                    else if (positionBind.process1Position2Bind == 0 && (positionBind.process1Position2BindLock == 0 || positionBind.process1Position2BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.process1Position2BindLock = workOrderTasks[i].OrderNO;
                        stationNo = (int)StationINDEX.UnitCNC1Station2;
                        processpositionisempty = true;
                    }
                    else if (positionBind.process2Position1Bind == 0 && (positionBind.process2Position1BindLock == 0 || positionBind.process2Position1BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.process2Position1BindLock = workOrderTasks[i].OrderNO;
                        stationNo = (int)StationINDEX.UnitCNC2Station1;
                        processpositionisempty = true;
                    }
                    else if (positionBind.process2Position2Bind == 0 && (positionBind.process2Position2BindLock == 0 || positionBind.process2Position2BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.process2Position2BindLock = workOrderTasks[i].OrderNO;
                        stationNo = (int)StationINDEX.UnitCNC2Station2;
                        processpositionisempty = true;
                    }

                    else if (positionBind.process3Position2Bind == 0 && (positionBind.process3Position1BindLock == 0 || positionBind.process3Position1BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.process3Position2BindLock = workOrderTasks[i].OrderNO;
                        stationNo = (int)StationINDEX.UnitCNC3Station2;
                        processpositionisempty = true;
                    }
                    else if (positionBind.process3Position1Bind == 0 && (positionBind.process3Position2BindLock == 0 || positionBind.process3Position2BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.process3Position1BindLock = workOrderTasks[i].OrderNO;
                        stationNo = (int)StationINDEX.UnitCNC3Station1;
                        processpositionisempty = true;
                    }
                    else if (positionBind.process4Position1Bind == 0 && (positionBind.process4Position1BindLock == 0 || positionBind.process4Position1BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.process4Position1BindLock = workOrderTasks[i].OrderNO;
                        stationNo = (int)StationINDEX.UnitCNC4Station1;
                        processpositionisempty = true;
                    }
                    else if (positionBind.process4Position2Bind == 0 && (positionBind.process4Position2BindLock == 0 || positionBind.process4Position2BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.process4Position2BindLock = workOrderTasks[i].OrderNO;
                        stationNo = (int)StationINDEX.UnitCNC4Station2;
                        processpositionisempty = true;
                    }


                    //料库空闲，且加工单元有空位置
                    if (positionBind.wMSPositionBind == 0 && processpositionisempty)
                    {
                        var condition1 = PIECETYTPE.无;
                        var condition2 = PIECETYTPE.无;
                        var condition3 = PIECETYTPE.无;
                        var condition4 = PIECETYTPE.无;
                        //  找符合订单需求的库位
                        if (workOrderTasks[i].Product1 == PRODUCTTYPE.A料)
                        {
                            condition1 = condition2 = condition3 = condition4 = PIECETYTPE.毛坯A;

                        }
                        else if (workOrderTasks[i].Product1 == PRODUCTTYPE.B料)
                        {
                            condition1 = condition2 = condition3 = condition4 = PIECETYTPE.毛坯B;
                        }

                        var rackdata = StockBinForm.listRackData.Find(s => s.Tray == workOrderTasks[i].Tray && s.Piece1 == condition1 && s.Piece1Quality == PIECEQUALITY.待检测 && s.Piece2 == condition2 && s.Piece2Quality == PIECEQUALITY.待检测 && s.Piece3 == condition3 && s.Piece3Quality == PIECEQUALITY.待检测 && s.Piece4 == condition4 && s.Piece4Quality == PIECEQUALITY.待检测);

                        if (rackdata != null && StockBinForm.unitDatatemp == null)
                        {
                            rackdata.Lock = true;
                            //初始化出库消息缓存
                            StockBinForm.unitDatatemp = new RackUnitData();
                            StockBinForm.unitDatatemp.NO = rackdata.NO;
                            StockBinForm.unitDatatemp.Tray = rackdata.Tray;
                            StockBinForm.unitDatatemp.TrayVolume = 0;
                            StockBinForm.unitDatatemp.RfidID = rackdata.RfidID;
                            StockBinForm.unitDatatemp.Piece1 = rackdata.Piece1;
                            StockBinForm.unitDatatemp.Piece1Quality = rackdata.Piece1Quality;
                            StockBinForm.unitDatatemp.Piece2 = rackdata.Piece2;
                            StockBinForm.unitDatatemp.Piece2Quality = rackdata.Piece2Quality;
                            StockBinForm.unitDatatemp.Piece3 = rackdata.Piece3;
                            StockBinForm.unitDatatemp.Piece3Quality = rackdata.Piece3Quality;
                            StockBinForm.unitDatatemp.Piece4 = rackdata.Piece4;
                            StockBinForm.unitDatatemp.Piece4Quality = rackdata.Piece4Quality;
                            StockBinForm.unitDatatemp.Lock = rackdata.Lock;
                            StockBinForm.unitDatatemp.StationNo = stationNo;
                            if (!StockBinForm.chukuflag)
                            {
                                StockBinForm.chukuflag = true;
                            }



                            StockBinForm.chukumanu = false;

                            workOrderTasks[i].RfidID = rackdata.RfidID;
                            // 出库检查指令下发
                            workOrderTasks[i].OUTNO = rackdata.NO;
                            positionBind.wMSPositionBind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].OrderState = ORDERSTATE.自动出库中;
                            positionBind.aGVBind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        }
                    }
                    else
                    {
                        PushMessage(string.Format("在料库中未找到工单号{0}的工件毛坯需求！", workOrderTasks[i].OrderNO), Color.Red);
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.自动出库中)
                {
                    // 等待出库完成
                    int ivalue = 0;
                    LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.TaskStateBit, out ivalue);

                    var res2 = ControlPLC.GetBoolValue(ivalue, (int)WMSPLC.TaskStateBit1.RackOutFinish);//查询到出库完成信号

                    //检测出库完成信号
                    if (res2)
                    {
                        var rackdata = StockBinForm.listRackData.Find(s => s.NO == workOrderTasks[i].OUTNO);

                        rackdata.Tray = TRAYTYPE.空;
                        rackdata.TrayVolume = 0;
                        rackdata.Piece1 = PIECETYTPE.无;
                        rackdata.Piece1Quality = PIECEQUALITY.待检测;
                        rackdata.Piece2 = PIECETYTPE.无;
                        rackdata.Piece2Quality = PIECEQUALITY.待检测;
                        rackdata.Piece3 = PIECETYTPE.无;
                        rackdata.Piece3Quality = PIECEQUALITY.待检测;
                        rackdata.Piece4 = PIECETYTPE.无;
                        rackdata.Piece4Quality = PIECEQUALITY.待检测;
                        rackdata.Lock = false;

                        OnRackAction1(true, rackdata.NO);
                        workOrderTasks[i].OrderState = ORDERSTATE.自动出库完成;

                        //LineMainForm.wmsplc.WriteSingleCoil(2104, false);
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.自动出库完成)
                {
                    if (positionBind.aGVBind != workOrderTasks[i].OrderNO)
                    {
                        return;
                    }
                    // 计算料盘需要派送到的加工位置
                    if (positionBind.process1Position1Bind == 0 && positionBind.process1Position1BindLock == workOrderTasks[i].OrderNO)
                    {
                        positionBind.process1Position1Bind = workOrderTasks[i].OrderNO;
                        positionBind.process1Position1BindLock = 0;
                        workOrderTasks[i].ProcessPosition = PROCESSPOSITION.加工单元1定位台1;

                    }
                    else if (positionBind.process1Position2Bind == 0 && positionBind.process1Position2BindLock == workOrderTasks[i].OrderNO)
                    {
                        positionBind.process1Position2Bind = workOrderTasks[i].OrderNO;
                        positionBind.process1Position2BindLock = 0;
                        workOrderTasks[i].ProcessPosition = PROCESSPOSITION.加工单元1定位台2;
                    }

                    if (positionBind.process2Position1Bind == 0 && positionBind.process2Position1BindLock == workOrderTasks[i].OrderNO)
                    {
                        positionBind.process2Position1Bind = workOrderTasks[i].OrderNO;
                        positionBind.process2Position1BindLock = 0;
                        workOrderTasks[i].ProcessPosition = PROCESSPOSITION.加工单元2定位台2;
                    }
                    else if (positionBind.process2Position2Bind == 0 && positionBind.process2Position2BindLock == workOrderTasks[i].OrderNO)
                    {
                        positionBind.process2Position2Bind = workOrderTasks[i].OrderNO;
                        positionBind.process2Position2BindLock = 0;
                        workOrderTasks[i].ProcessPosition = PROCESSPOSITION.加工单元2定位台2;
                    }

                    if (positionBind.process3Position1Bind == 0 && positionBind.process3Position1BindLock == workOrderTasks[i].OrderNO)
                    {
                        positionBind.process3Position1Bind = workOrderTasks[i].OrderNO;
                        positionBind.process3Position1BindLock = 0;
                        workOrderTasks[i].ProcessPosition = PROCESSPOSITION.加工单元3定位台1;
                    }
                    else if (positionBind.process3Position2Bind == 0 && positionBind.process3Position2BindLock == workOrderTasks[i].OrderNO)
                    {
                        positionBind.process3Position2Bind = workOrderTasks[i].OrderNO;
                        positionBind.process3Position2BindLock = 0;
                        workOrderTasks[i].ProcessPosition = PROCESSPOSITION.加工单元3定位台2;
                    }

                    if (positionBind.process4Position1Bind == 0 && positionBind.process4Position1BindLock == workOrderTasks[i].OrderNO)
                    {
                        positionBind.process4Position1Bind = workOrderTasks[i].OrderNO;
                        positionBind.process4Position1BindLock = 0;
                        workOrderTasks[i].ProcessPosition = PROCESSPOSITION.加工单元4定位台1;
                    }
                    else if (positionBind.process4Position2Bind == 0 && positionBind.process4Position2BindLock == workOrderTasks[i].OrderNO)
                    {

                        positionBind.process4Position2Bind = workOrderTasks[i].OrderNO;
                        positionBind.process4Position2BindLock = 0;
                        workOrderTasks[i].ProcessPosition = PROCESSPOSITION.加工单元4定位台2;
                    }

                    if (workOrderTasks[i].ProcessPosition != PROCESSPOSITION.暂无)
                    {
                        //下发AGV送料到加工单元的任务，待补充
                        //LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.自动出库到加工单元指派, 4);
                        //LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.AGV到加工单元定位台放料, 1);
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到出入库位置取料运输中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;


                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到出入库位置取料运输中)
                {
                    //检测agv信号
                    int ivalue = 0;
                     LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.AGVArriveed, out ivalue);
                    if (ivalue == 1)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元放料运输完成;
                      //  workOrderTasks[i].OrderState = ORDERSTATE.AGV到出入库位置取料运输完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                //else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到出入库位置取料运输完成)
                //{
                //    //检测agv信号
                //    if (true)
                //    {

                //        workOrderTasks[i].OrderState = ORDERSTATE.AGV到出入库位置取料中;
                //        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                //    }
                //    break;
                //}
                //else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到出入库位置取料中)
                //{
                //    //检测agv信号
                //    if (true)
                //    {
                //        workOrderTasks[i].OrderState = ORDERSTATE.AGV到出入库位置取料完成;
                //        workOrderTasks[i].Updatetime = DateTime.Now.ToString();


                //    }
                //    break;
                //}
                //else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到出入库位置取料完成)
                //{
                //    //检测agv信号
                //    if (true)
                //    {
                //        if (positionBind.wMSPositionBind == workOrderTasks[i].OrderNO)
                //            positionBind.wMSPositionBind = 0;
                //        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元放料运输中;
                //        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                //        StockBinForm.unitDatatemp = null;
                //    }

                //    break;
                //}
                //else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元放料运输中)
                //{
                //    //检测agv信号
                //    if (true)
                //    {

                //        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元放料运输完成;
                //        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                //    }
                //    break;
                //}
            }
        }


        private void WorkOrderProcessDoing()
        {
            WorkOrderProStation1P1Doing();
            WorkOrderProStation1P2Doing();
            WorkOrderProStation2P1Doing();
            WorkOrderProStation2P2Doing();
            WorkOrderProStation3P1Doing();
            WorkOrderProStation3P2Doing();
            WorkOrderProStation4P1Doing();
            WorkOrderProStation4P2Doing();
        }

        private void WorkOrderProStation1P1Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].ProcessPosition != PROCESSPOSITION.加工单元1定位台1)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元放料运输完成)
                {

                   
                    //set单元定位台AGV到站
                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, true);
                    //查询单元定位台AGV是否允许上料
                    bool value;
                    bool res = LineMainForm.unitplc1.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 1, out value);

                    if (!res)
                    {
                        return;
                    }
                    //启动AGV辊筒上料
                    LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.AGVRollCMD,1);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料中)
                {
                    //如果检测到利库辊筒进料中，清除辊筒任务
                    bool value;
                    bool res = LineMainForm.wmsplc.GetRegIndexBit((int)WMSPLC.DATAINDEX.TaskStateBit, 0, out value);
                    if(res)
                    {
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.AGVRollCMD, 0);
                    }

                    //如果收到上料完成信号
                
                    res = LineMainForm.unitplc1.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 2, out value);
                    if (res && value)
                    {
                        //清除AGV任务
                        int ivalue = 0;
                        res = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.AGVTask, out ivalue);
                        if (res && ivalue > 0)
                        {
                           
                            LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.AGVTask, 0);
                        }
                        //清除AGV到站信号
                        LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, false);                      
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.unitplc1.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidcnc11.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidcnc11.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidcnc11.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidcnc11.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.unitplc1.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台左物料类型, (int)table.Piece1);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 0, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 1, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 2, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 3, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 4, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 5, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 6, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 7, true);

                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 1, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc11.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {

                                LineMainForm.unitplc1.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台左物料类型, (int)table.Piece1);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 0, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 1, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 2, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 3, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 4, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 5, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 6, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 7, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 1, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc11.IP), Color.Red);
                            }
                        }
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工中)
                {
                    //process1Pos1count++;
                    //if (process1Pos1count >= testProcesstime)
                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.unitplc1.ReadsingleRegister((int)UnitPLC.REGINDEX.加工定位台左当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.机床加工完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工完成)
                {
                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.unitplc1.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 0, out value2);
                    //获取清洗定位台状态

                    bool cleanpositionisempty = false;



                    if (positionBind.aGVBind == 0 && positionBind.cleanPosition1Bind == 0 && positionBind.cleanPosition1BindLock == 0)
                    {
                        positionBind.cleanPosition1BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    else if (positionBind.aGVBind == 0 && positionBind.cleanPosition2Bind == 0 && positionBind.cleanPosition2BindLock == 0)
                    {
                        positionBind.cleanPosition2BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    if (cleanpositionisempty)
                    {
                        //发送AGV取料任务
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输中)
                {
                    //发送AGV运输任务
                    positionBind.aGVBind = workOrderTasks[i].OrderNO;
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输完成)
                {
                    //AGV到位
                    //set单元定位台AGV到站
                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料中)
                {
                    //AGV有空，清洗1工位
                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO && (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO || positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        if (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition1Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台1;
                        }
                        else if (positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition2Bind = workOrderTasks[i].OrderNO;

                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台2;
                        }

                        //获取AGV收料完成信号
                        if (true)
                        {

                            //set单元定位台AGV收料完成
                            LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 4, true);

                            workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料完成;
                            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        }

                        break;
                    }

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料完成)
                {
                    //AGV送料任务

                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, false);
                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 4, false);

                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元放料运输中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    positionBind.process1Position1Bind = 0;
                }

                continue;
            }
        }

        private void WorkOrderProStation1P2Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].ProcessPosition != PROCESSPOSITION.加工单元1定位台2)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元放料运输完成)
                {
                    //set单元定位台AGV到站
                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料中)
                {
                    //如果收到上料完成信号
                    bool value;
                    bool res = LineMainForm.unitplc1.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 2, out value);
                    if (res && value)
                    {
                        //清除AGV到站信号
                        LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 3, false);
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    else
                    {
                        //如果收到上料允许信号，给AGV滚动信号

                        res = LineMainForm.unitplc1.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 1, out value);
                        if (res && value)
                        {
                            //给AGV滚动信号
                        }
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.unitplc1.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidcnc12.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidcnc12.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidcnc12.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidcnc12.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.unitplc1.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台右物料类型, (int)table.Piece1);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 0, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 1, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 2, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 3, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 4, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 5, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 6, true);
                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 7, true);

                                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 1, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc12.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {

                                LineMainForm.unitplc1.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台右物料类型, (int)table.Piece1);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 0, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 1, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 2, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 3, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 4, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 5, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 6, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 7, true);
                                LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 1, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc12.IP), Color.Red);
                            }
                        }
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工中)
                {
                    //process1Pos1count++;
                    //if (process1Pos1count >= testProcesstime)
                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.unitplc1.ReadsingleRegister((int)UnitPLC.REGINDEX.加工定位台右当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.机床加工完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工完成)
                {
                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.unitplc1.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 0, out value2);
                    //获取清洗定位台状态

                    bool cleanpositionisempty = false;



                    if (positionBind.aGVBind == 0 && positionBind.cleanPosition1Bind == 0 && positionBind.cleanPosition1BindLock == 0)
                    {
                        positionBind.cleanPosition1BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    if (positionBind.aGVBind == 0 && positionBind.cleanPosition2Bind == 0 && positionBind.cleanPosition2BindLock == 0)
                    {
                        positionBind.cleanPosition2BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    if (cleanpositionisempty)
                    {
                        //发送AGV取料任务
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输中)
                {
                    //发送AGV运输任务
                    positionBind.aGVBind = workOrderTasks[i].OrderNO;
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输完成)
                {
                    //AGV到位
                    //set单元定位台AGV到站
                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料中)
                {
                    //AGV有空，清洗1工位
                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO && (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO || positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        if (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition1Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台1;
                        }
                        else if (positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition2Bind = workOrderTasks[i].OrderNO;

                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台2;
                        }

                        //获取AGV收料完成信号
                        if (true)
                        {

                            //set单元定位台AGV收料完成
                            LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 4, true);

                            workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料完成;
                            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            positionBind.process1Position2Bind = 0;
                        }

                        break;
                    }

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料完成)
                {
                    //AGV送料任务

                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 3, false);
                    LineMainForm.unitplc1.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 4, false);

                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元放料运输中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }

                continue;
            }
        }

        private void WorkOrderProStation2P1Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].ProcessPosition != PROCESSPOSITION.加工单元2定位台1)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元放料运输完成)
                {
                    //set单元定位台AGV到站
                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料中)
                {
                    //如果收到上料完成信号
                    bool value;
                    bool res = LineMainForm.unitplc2.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 2, out value);
                    if (res && value)
                    {
                        //清除AGV到站信号
                        LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, false);
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    else
                    {
                        //如果收到上料允许信号，给AGV滚动信号

                        res = LineMainForm.unitplc2.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 1, out value);
                        if (res && value)
                        {
                            //给AGV滚动信号
                        }
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.unitplc2.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidcnc21.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidcnc21.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidcnc21.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidcnc21.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.unitplc2.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台左物料类型, (int)table.Piece1);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 0, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 1, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 2, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 3, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 4, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 5, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 6, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 7, true);

                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 1, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc21.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {

                                LineMainForm.unitplc2.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台左物料类型, (int)table.Piece1);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 0, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 1, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 2, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 3, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 4, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 5, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 6, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 7, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 1, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc21.IP), Color.Red);
                            }
                        }
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工中)
                {
                    //process1Pos1count++;
                    //if (process1Pos1count >= testProcesstime)
                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.unitplc2.ReadsingleRegister((int)UnitPLC.REGINDEX.加工定位台左当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.机床加工完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工完成)
                {
                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.unitplc2.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 0, out value2);
                    //获取清洗定位台状态

                    bool cleanpositionisempty = false;



                    if (positionBind.aGVBind == 0 && positionBind.cleanPosition1Bind == 0 && positionBind.cleanPosition1BindLock == 0)
                    {
                        positionBind.cleanPosition1BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    else if (positionBind.aGVBind == 0 && positionBind.cleanPosition2Bind == 0 && positionBind.cleanPosition2BindLock == 0)
                    {
                        positionBind.cleanPosition2BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    if (cleanpositionisempty)
                    {
                        //发送AGV取料任务
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输中)
                {
                    //发送AGV运输任务
                    positionBind.aGVBind = workOrderTasks[i].OrderNO;
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输完成)
                {
                    //AGV到位
                    //set单元定位台AGV到站
                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料中)
                {
                    //AGV有空，清洗1工位
                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO && (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO || positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        if (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition1Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台1;
                        }
                        else if (positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition2Bind = workOrderTasks[i].OrderNO;

                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台2;
                        }

                        //获取AGV收料完成信号
                        if (true)
                        {

                            //set单元定位台AGV收料完成
                            LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 4, true);

                            workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料完成;
                            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            positionBind.process2Position1Bind = 0;
                        }

                        break;
                    }

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料完成)
                {
                    //AGV送料任务

                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, false);
                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 4, false);

                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元放料运输中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }

                continue;
            }
        }

        private void WorkOrderProStation2P2Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].ProcessPosition != PROCESSPOSITION.加工单元2定位台2)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元放料运输完成)
                {
                    //set单元定位台AGV到站
                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料中)
                {
                    //如果收到上料完成信号
                    bool value;
                    bool res = LineMainForm.unitplc2.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 2, out value);
                    if (res && value)
                    {
                        //清除AGV到站信号
                        LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 3, false);
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    else
                    {
                        //如果收到上料允许信号，给AGV滚动信号

                        res = LineMainForm.unitplc2.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 1, out value);
                        if (res && value)
                        {
                            //给AGV滚动信号
                        }
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.unitplc2.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidcnc22.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidcnc22.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidcnc22.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidcnc22.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.unitplc2.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台右物料类型, (int)table.Piece1);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 0, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 1, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 2, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 3, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 4, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 5, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 6, true);
                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 7, true);

                                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 1, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc12.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {

                                LineMainForm.unitplc2.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台右物料类型, (int)table.Piece1);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 0, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 1, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 2, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 3, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 4, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 5, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 6, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 7, true);
                                LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 1, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc12.IP), Color.Red);
                            }
                        }
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工中)
                {
                    //process1Pos1count++;
                    //if (process1Pos1count >= testProcesstime)
                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.unitplc2.ReadsingleRegister((int)UnitPLC.REGINDEX.加工定位台右当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.机床加工完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工完成)
                {
                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.unitplc2.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 0, out value2);
                    //获取清洗定位台状态

                    bool cleanpositionisempty = false;



                    if (positionBind.aGVBind == 0 && positionBind.cleanPosition1Bind == 0 && positionBind.cleanPosition1BindLock == 0)
                    {
                        positionBind.cleanPosition1BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    if (positionBind.aGVBind == 0 && positionBind.cleanPosition2Bind == 0 && positionBind.cleanPosition2BindLock == 0)
                    {
                        positionBind.cleanPosition2BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }

                    if (cleanpositionisempty)
                    {
                        //发送AGV取料任务
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输中)
                {
                    //发送AGV运输任务
                    positionBind.aGVBind = workOrderTasks[i].OrderNO;
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输完成)
                {
                    //AGV到位
                    //set单元定位台AGV到站
                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料中)
                {
                    //AGV有空，清洗1工位
                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO && (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO || positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        if (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition1Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台1;
                        }
                        else if (positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition2Bind = workOrderTasks[i].OrderNO;

                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台2;
                        }

                        //获取AGV收料完成信号
                        if (true)
                        {

                            //set单元定位台AGV收料完成
                            LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 4, true);

                            workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料完成;
                            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            positionBind.process2Position2Bind = 0;
                        }

                        break;
                    }

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料完成)
                {
                    //AGV送料任务

                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 3, false);
                    LineMainForm.unitplc2.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 4, false);

                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元放料运输中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }

                continue;
            }
        }

        private void WorkOrderProStation3P1Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].ProcessPosition != PROCESSPOSITION.加工单元3定位台1)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元放料运输完成)
                {
                    //set单元定位台AGV到站
                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料中)
                {
                    //如果收到上料完成信号
                    bool value;
                    bool res = LineMainForm.unitplc3.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 2, out value);
                    if (res && value)
                    {
                        //清除AGV到站信号
                        LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, false);
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    else
                    {
                        //如果收到上料允许信号，给AGV滚动信号

                        res = LineMainForm.unitplc3.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 1, out value);
                        if (res && value)
                        {
                            //给AGV滚动信号
                        }
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.unitplc3.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidcnc31.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidcnc31.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidcnc31.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidcnc31.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.unitplc3.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台左物料类型, (int)table.Piece1);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 0, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 1, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 2, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 3, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 4, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 5, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 6, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 7, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 1, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc31.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {

                                LineMainForm.unitplc3.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台左物料类型, (int)table.Piece1);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 0, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 1, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 2, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 3, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 4, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 5, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 6, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 7, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 1, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc21.IP), Color.Red);
                            }
                        }
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工中)
                {
                    //process1Pos1count++;
                    //if (process1Pos1count >= testProcesstime)
                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.unitplc3.ReadsingleRegister((int)UnitPLC.REGINDEX.加工定位台左当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.机床加工完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工完成)
                {
                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.unitplc3.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 0, out value2);
                    //获取清洗定位台状态

                    bool cleanpositionisempty = false;



                    if (positionBind.aGVBind == 0 && positionBind.cleanPosition1Bind == 0 && positionBind.cleanPosition1BindLock == 0)
                    {
                        positionBind.cleanPosition1BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    else if (positionBind.aGVBind == 0 && positionBind.cleanPosition2Bind == 0 && positionBind.cleanPosition2BindLock == 0)
                    {
                        positionBind.cleanPosition2BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    if (cleanpositionisempty)
                    {
                        //发送AGV取料任务
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输中)
                {
                    //发送AGV运输任务
                    positionBind.aGVBind = workOrderTasks[i].OrderNO;
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输完成)
                {
                    //AGV到位
                    //set单元定位台AGV到站
                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料中)
                {
                    //AGV有空，清洗1工位
                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO && (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO || positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        if (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition1Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台1;
                        }
                        else if (positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition2Bind = workOrderTasks[i].OrderNO;

                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台2;
                        }

                        //获取AGV收料完成信号
                        if (true)
                        {

                            //set单元定位台AGV收料完成
                            LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 4, true);

                            workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料完成;
                            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            positionBind.process3Position1Bind = 0;
                        }

                        break;
                    }

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料完成)
                {
                    //AGV送料任务

                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, false);
                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 4, false);

                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元放料运输中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }

                continue;
            }
        }
        private void WorkOrderProStation3P2Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].ProcessPosition != PROCESSPOSITION.加工单元3定位台2)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元放料运输完成)
                {
                    //set单元定位台AGV到站
                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料中)
                {
                    //如果收到上料完成信号
                    bool value;
                    bool res = LineMainForm.unitplc3.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 2, out value);
                    if (res && value)
                    {
                        //清除AGV到站信号
                        LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 3, false);
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    else
                    {
                        //如果收到上料允许信号，给AGV滚动信号

                        res = LineMainForm.unitplc3.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 1, out value);
                        if (res && value)
                        {
                            //给AGV滚动信号
                        }
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.unitplc3.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidcnc32.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidcnc32.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidcnc32.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidcnc32.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.unitplc3.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台右物料类型, (int)table.Piece1);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 0, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 1, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 2, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 3, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 4, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 5, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 6, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 7, true);
                                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 1, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc12.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {

                                LineMainForm.unitplc3.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台右物料类型, (int)table.Piece1);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 0, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 1, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 2, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 3, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 4, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 5, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 6, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 7, true);
                                LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 1, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc12.IP), Color.Red);
                            }
                        }
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工中)
                {
                    //process1Pos1count++;
                    //if (process1Pos1count >= testProcesstime)
                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.unitplc3.ReadsingleRegister((int)UnitPLC.REGINDEX.加工定位台右当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.机床加工完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工完成)
                {
                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.unitplc3.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 0, out value2);
                    //获取清洗定位台状态

                    bool cleanpositionisempty = false;



                    if (positionBind.aGVBind == 0 && positionBind.cleanPosition1Bind == 0 && positionBind.cleanPosition1BindLock == 0)
                    {
                        positionBind.cleanPosition1BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    if (positionBind.aGVBind == 0 && positionBind.cleanPosition2Bind == 0 && positionBind.cleanPosition2BindLock == 0)
                    {
                        positionBind.cleanPosition2BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    if (cleanpositionisempty)
                    {
                        //发送AGV取料任务
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输中)
                {
                    //发送AGV运输任务
                    positionBind.aGVBind = workOrderTasks[i].OrderNO;
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输完成)
                {
                    //AGV到位
                    //set单元定位台AGV到站
                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料中)
                {
                    //AGV有空，清洗1工位
                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO && (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO || positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        if (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition1Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台1;
                        }
                        else if (positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition2Bind = workOrderTasks[i].OrderNO;

                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台2;
                        }

                        //获取AGV收料完成信号
                        if (true)
                        {

                            //set单元定位台AGV收料完成
                            LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 4, true);

                            workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料完成;
                            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            positionBind.process3Position2Bind = 0;
                        }

                        break;
                    }

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料完成)
                {
                    //AGV送料任务

                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 3, false);
                    LineMainForm.unitplc3.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 4, false);

                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元放料运输中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }

                continue;
            }
        }
        private void WorkOrderProStation4P1Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].ProcessPosition != PROCESSPOSITION.加工单元4定位台1)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元放料运输完成)
                {
                    //set单元定位台AGV到站
                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料中)
                {
                    //如果收到上料完成信号
                    bool value;
                    bool res = LineMainForm.unitplc4.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 2, out value);
                    if (res && value)
                    {
                        //清除AGV到站信号
                        LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, false);
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    else
                    {
                        //如果收到上料允许信号，给AGV滚动信号

                        res = LineMainForm.unitplc4.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 1, out value);
                        if (res && value)
                        {
                            //给AGV滚动信号
                        }
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.unitplc4.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidcnc41.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidcnc41.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidcnc41.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidcnc41.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.unitplc4.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台左物料类型, (int)table.Piece1);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 0, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 1, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 2, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 3, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 4, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 5, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 6, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 7, true);

                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 1, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc21.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {

                                LineMainForm.unitplc4.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台左物料类型, (int)table.Piece1);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 0, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 1, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 2, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 3, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 4, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左工件状态, 5, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 6, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 7, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左RFID状态, 1, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc21.IP), Color.Red);
                            }
                        }
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工中)
                {
                    //process1Pos1count++;
                    //if (process1Pos1count >= testProcesstime)
                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.unitplc4.ReadsingleRegister((int)UnitPLC.REGINDEX.加工定位台左当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.机床加工完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工完成)
                {
                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.unitplc4.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 0, out value2);
                    //获取清洗定位台状态

                    bool cleanpositionisempty = false;



                    if (positionBind.aGVBind == 0 && positionBind.cleanPosition1Bind == 0 && positionBind.cleanPosition1BindLock == 0)
                    {
                        positionBind.cleanPosition1BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    else if (positionBind.aGVBind == 0 && positionBind.cleanPosition2Bind == 0 && positionBind.cleanPosition2BindLock == 0)
                    {
                        positionBind.cleanPosition2BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    if (cleanpositionisempty)
                    {
                        //发送AGV取料任务
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输中)
                {
                    //发送AGV运输任务
                    positionBind.aGVBind = workOrderTasks[i].OrderNO;
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    positionBind.process4Position1Bind = 0;

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输完成)
                {
                    //AGV到位
                    //set单元定位台AGV到站
                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料中)
                {
                    //AGV有空，清洗1工位
                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO && (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO || positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        if (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition1Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台1;
                        }
                        else if (positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition2Bind = workOrderTasks[i].OrderNO;

                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台2;
                        }

                        //获取AGV收料完成信号
                        if (true)
                        {

                            //set单元定位台AGV收料完成
                            LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 4, true);

                            workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料完成;
                            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        }

                        break;
                    }

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料完成)
                {
                    //AGV送料任务

                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, false);
                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 4, false);

                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元放料运输中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }

                continue;
            }
        }
        private void WorkOrderProStation4P2Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].ProcessPosition != PROCESSPOSITION.加工单元4定位台2)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元放料运输完成)
                {
                    //set单元定位台AGV到站
                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料中)
                {
                    //如果收到上料完成信号
                    bool value;
                    bool res = LineMainForm.unitplc4.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 2, out value);
                    if (res && value)
                    {
                        //清除AGV到站信号
                        LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 3, false);
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    else
                    {
                        //如果收到上料允许信号，给AGV滚动信号

                        res = LineMainForm.unitplc4.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 1, out value);
                        if (res && value)
                        {
                            //给AGV滚动信号
                        }
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.unitplc4.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidcnc42.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidcnc42.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidcnc42.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidcnc42.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.unitplc4.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台右物料类型, (int)table.Piece1);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 0, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 1, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 2, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 3, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 4, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 5, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 6, true);
                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 7, true);

                                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 1, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc12.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {

                                LineMainForm.unitplc4.WritesingleRegister((int)UnitPLC.REGINDEX.加工定位台右物料类型, (int)table.Piece1);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 0, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 1, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 2, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 3, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 4, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右工件状态, 5, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 6, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 7, true);
                                LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右RFID状态, 1, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.机床加工中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidcnc12.IP), Color.Red);
                            }
                        }
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工中)
                {
                    //process1Pos1count++;
                    //if (process1Pos1count >= testProcesstime)
                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.unitplc4.ReadsingleRegister((int)UnitPLC.REGINDEX.加工定位台右当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.机床加工完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.机床加工完成)
                {
                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.unitplc4.GetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 0, out value2);
                    //获取清洗定位台状态

                    bool cleanpositionisempty = false;



                    if (positionBind.aGVBind == 0 && positionBind.cleanPosition1Bind == 0 && positionBind.cleanPosition1BindLock == 0)
                    {
                        positionBind.cleanPosition1BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }

                    if (positionBind.aGVBind == 0 && positionBind.cleanPosition2Bind == 0 && positionBind.cleanPosition2BindLock == 0)
                    {
                        positionBind.cleanPosition2BindLock = workOrderTasks[i].OrderNO;
                        cleanpositionisempty = true;
                    }
                    if (cleanpositionisempty)
                    {
                        //发送AGV取料任务
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输中)
                {
                    //发送AGV运输任务
                    positionBind.aGVBind = workOrderTasks[i].OrderNO;
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元取料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元取料运输完成)
                {
                    //AGV到位
                    //set单元定位台AGV到站
                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料中)
                {
                    //AGV有空，清洗1工位
                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO && (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO || positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO))
                    {
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        if (positionBind.cleanPosition1BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition1Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台1;
                        }
                        else if (positionBind.cleanPosition2BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition2Bind = workOrderTasks[i].OrderNO;

                            workOrderTasks[i].CleanPosition = CLEANPOSITION.清洗单元定位台2;
                        }

                        //获取AGV收料完成信号
                        if (true)
                        {

                            //set单元定位台AGV收料完成

                            workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料完成;
                            workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                            positionBind.process4Position2Bind = 0;
                        }

                        break;
                    }

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料完成)
                {
                    //AGV送料任务

                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 3, false);
                    LineMainForm.unitplc4.SetRegIndexBit((int)UnitPLC.REGINDEX.加工定位台右任务状态, 4, false);

                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元放料运输中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }

                continue;
            }
        }
        //private int[] ConvertMetiral(PRODUCTTYPE p1, PRODUCTTYPE p2, PRODUCTTYPE p3, PRODUCTTYPE p4)
        //{
        //    int[] values = new int[4];
        //    values[0] = (int)((ControlPLC.MATIRALNO)Enum.Parse(typeof(ControlPLC.MATIRALNO), p1.ToString()));
        //    values[1] = (int)((ControlPLC.MATIRALNO)Enum.Parse(typeof(ControlPLC.MATIRALNO), p2.ToString()));
        //    values[2] = (int)((ControlPLC.MATIRALNO)Enum.Parse(typeof(ControlPLC.MATIRALNO), p3.ToString()));
        //    values[3] = (int)((ControlPLC.MATIRALNO)Enum.Parse(typeof(ControlPLC.MATIRALNO), p4.ToString()));
        //    return values;
        //}

        private void WorkOrderToCleanDoing()
        {
            WorkOrderCleanStationP1Doing();
            WorkOrderCleanStationP2Doing();
        }

        private void WorkOrderToFitDoing()
        {
            WorkOrderFitStation1P1Doing();
            WorkOrderFitStation1P2Doing();
            WorkOrderFitStation2P1Doing();
            WorkOrderFitStation2P2Doing();
        }
        private void WorkOrderFitStation1P1Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].FitPosition != FITPOSITION.装配单元1定位台1)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元放料运输中)
                {
                    //检测AGV送料到位
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元放料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元放料运输完成)
                {
                    //给PLCAGV到站信号
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台放料中)
                {
                    //检测是够有AGV上料完成信号
                    if (true)
                    {
                        //给PLC上料完成信号完成
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 2, true);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    else
                    {
                        //检测PLC是否给出上料允许信号
                        bool value;
                        bool res = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 1, out value);
                        if (res && value)
                        {
                            //给AGV滚动料盘信号

                        }
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidfit11.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidfit11.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidfit11.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidfit11.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.装配1定位台左物料类型, (int)table.Piece1);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 0, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 1, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 2, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 3, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 4, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 5, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 6, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 7, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.工件装配中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidfit11.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {
                                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.装配1定位台左物料类型, (int)table.Piece1);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 0, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 1, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 2, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 3, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 4, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 5, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 6, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 7, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 1, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.工件装配中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidfit11.IP), Color.Red);
                            }
                        }
                    }
                    break;

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.工件装配中)
                {

                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.装配1定位台左当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        //更新料盘物料信息，A料盘与B料盘组装后只有一个料盘有组合料，另一个料盘为空料盘
                        workOrderTasks[i].OrderState = ORDERSTATE.工件装配完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;


                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.工件装配完成)
                {

                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 0, out value2);
                    //入库定位台是否空闲（）
                    bool wmsinpositionisempty = false;

                    if (positionBind.aGVBind == 0 && (positionBind.wMSPositionBind == 0 || positionBind.wMSPositionBind == workOrderTasks[i].OrderNO) && (positionBind.wMSPositionBindLock == 0 || positionBind.wMSPositionBindLock == workOrderTasks[i].OrderNO))
                    {

                        var rackdata1 = StockBinForm.listRackData.Find(s => s.Tray == TRAYTYPE.空);

                        if (rackdata1 != null && StockBinForm.unitDatatemp == null)
                        {
                            rackdata1.Lock = true;
                            workOrderTasks[i].INNO = rackdata1.NO;

                            //初始化r入库消息缓存
                            StockBinForm.unitDatatemp = new RackUnitData();
                            StockBinForm.unitDatatemp.NO = rackdata1.NO;
                            StockBinForm.unitDatatemp.Tray = workOrderTasks[i].Tray;
                            StockBinForm.unitDatatemp.RfidID = workOrderTasks[i].RfidID;
                            StockBinForm.unitDatatemp.TrayVolume = (workOrderTasks[i].Tray == TRAYTYPE.料盘A) ? 1 : 2;
                            StockBinForm.unitDatatemp.Piece1 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product1.ToString());
                            StockBinForm.unitDatatemp.Piece1Quality = workOrderTasks[i].P1Quality;
                            StockBinForm.unitDatatemp.Piece2 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product2.ToString());
                            StockBinForm.unitDatatemp.Piece2Quality = workOrderTasks[i].P2Quality;
                            StockBinForm.unitDatatemp.Piece3 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product3.ToString());
                            StockBinForm.unitDatatemp.Piece3Quality = workOrderTasks[i].P3Quality;
                            StockBinForm.unitDatatemp.Piece4 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product4.ToString());
                            StockBinForm.unitDatatemp.Piece4Quality = workOrderTasks[i].P4Quality;
                            StockBinForm.unitDatatemp.Lock = rackdata1.Lock;
                            //if (!  StockBinForm.rukuflag)
                            //{
                            //      StockBinForm.rukuflag = true;
                            //      StockBinForm.rukumanu = false;
                            //}
                            //入库检查指令下发
                            workOrderTasks[i].INNO = rackdata1.NO;
                            AddStaticsDataToDB(rackdata1);
                            positionBind.wMSPositionBindLock = workOrderTasks[i].OrderNO;
                            //发送AGV取料任务
                            workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元取料运输中;
                            //发送AGV运输任务
                            positionBind.aGVBind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                        }

                    }

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元取料运输中)
                {
                    //检测AGV取料任务到位
                    if (true)
                    {

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元取料运输完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元取料运输完成)
                {  //AGV到位
                    //给立体库
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                }

                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台取料中)
                {
                    //AGV有空，检索目标装配工位

                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO && positionBind.wMSPositionBindLock == workOrderTasks[i].OrderNO)
                    {
                        positionBind.wMSPositionBind = workOrderTasks[i].OrderNO;

                    }

                    //获取AGV收料完成信号
                    if (true)
                    {

                        //set单元定位台AGV收料完成
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 4, true);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台取料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台取料完成)
                {
                   
                    //查询料库允许入库
                    int ivalue1 = 0;
                    int ivalue2 = 0;
                    bool res1 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackState, out ivalue1);
                    bool res2 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackErrCode, out ivalue2);
                    if (ivalue1 != 1|| ivalue2 > 0)
                    {
                        PushMessage("料库状态异常", Color.Red);
                        return;
                    }
                    int no = StockBinForm.unitDatatemp.NO-1;

                    //目标库位是否占据t no = StockBinForm.unitDatatemp.NO;
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
             

                    var rackdata1 = StockBinForm.listRackData.Find(s => s.Tray == TRAYTYPE.空);

                    if (rackdata1 != null && StockBinForm.unitDatatemp == null)
                    {
                        rackdata1.Lock = true;
                        workOrderTasks[i].INNO = rackdata1.NO;

                        //初始化r入库消息缓存
                        StockBinForm.unitDatatemp = new RackUnitData();
                        StockBinForm.unitDatatemp.NO = rackdata1.NO;
                        StockBinForm.unitDatatemp.Tray = workOrderTasks[i].Tray;
                        StockBinForm.unitDatatemp.RfidID = workOrderTasks[i].RfidID;
                        StockBinForm.unitDatatemp.TrayVolume = (workOrderTasks[i].Tray == TRAYTYPE.料盘A) ? 4: 4;
                        StockBinForm.unitDatatemp.Piece1 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product1.ToString());
                        StockBinForm.unitDatatemp.Piece1Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Piece2 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product2.ToString());
                        StockBinForm.unitDatatemp.Piece2Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Piece3 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product3.ToString());
                        StockBinForm.unitDatatemp.Piece3Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Piece4 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product4.ToString());
                        StockBinForm.unitDatatemp.Piece4Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Lock = rackdata1.Lock;
                       
                        //入库检查指令下发
                        workOrderTasks[i].INNO = rackdata1.NO;
                        AddStaticsDataToDB(rackdata1);
                        positionBind.wMSPositionBind = workOrderTasks[i].OrderNO;

                        
                        //下发AGV送料任务
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMS_On, 1);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInStation, 2);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInBinNo, StockBinForm.unitDatatemp.NO-1);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.AGVTask, (int)WMSPLC.StationINDEX.UnitRackStation);


                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 3, false);
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 4, false);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到出入库入库运输中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();



                    }
                    else
                    {
                        PushMessage(string.Format("料库已满，工单号{0}的工件无法入库！", workOrderTasks[i].OrderNO), Color.Red);
                    }


                    
                    break;
                }
            }
        }
        private void WorkOrderFitStation1P2Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].FitPosition != FITPOSITION.装配单元1定位台2)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元放料运输中)
                {
                    //检测AGV送料到位
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元放料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元放料运输完成)
                {
                    //给PLCAGV到站信号
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台放料中)
                {
                    //检测是够有AGV上料完成信号
                    if (true)
                    {
                        //给PLC上料完成信号完成
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右任务状态, 2, true);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    else
                    {
                        //检测PLC是否给出上料允许信号
                        bool value;
                        bool res = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右任务状态, 1, out value);
                        if (res && value)
                        {
                            //给AGV滚动料盘信号

                        }
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidfit12.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidfit12.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidfit12.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidfit12.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.装配1定位台左物料类型, (int)table.Piece1);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 0, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 1, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 2, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 3, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 4, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 5, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 6, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左工件状态, 7, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.工件装配中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidfit12.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {
                                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.装配1定位台右物料类型, (int)table.Piece1);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右工件状态, 0, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右工件状态, 1, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右工件状态, 2, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右工件状态, 3, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右工件状态, 4, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右工件状态, 5, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右工件状态, 6, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右工件状态, 7, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右工件状态, 1, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.工件装配中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidfit12.IP), Color.Red);
                            }
                        }
                    }
                    break;

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.工件装配中)
                {

                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.装配1定位台右当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        //更新料盘物料信息，A料盘与B料盘组装后只有一个料盘有组合料，另一个料盘为空料盘
                        workOrderTasks[i].OrderState = ORDERSTATE.工件装配完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;


                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.工件装配完成)
                {

                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右任务状态, 0, out value2);
                    //入库定位台是否空闲（）
                    bool wmsinpositionisempty = false;

                    if (positionBind.aGVBind == 0 && positionBind.wMSPositionBind == 0 && positionBind.wMSPositionBindLock == 0)
                    {
                        positionBind.wMSPositionBindLock = workOrderTasks[i].OrderNO;
                        wmsinpositionisempty = true;
                    }

                    if (wmsinpositionisempty)
                    {
                        //发送AGV取料任务
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元取料运输中;
                        //发送AGV运输任务
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    }
                    break;

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元取料运输中)
                {
                    //检测AGV取料任务到位
                    if (true)
                    {

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元取料运输完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元取料运输完成)
                {  //AGV到位
                    //给立体库
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                }

                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台取料中)
                {
                    //AGV有空，检索目标装配工位

                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO && positionBind.wMSPositionBindLock == workOrderTasks[i].OrderNO)
                    {
                        positionBind.wMSPositionBind = workOrderTasks[i].OrderNO;

                    }

                    //获取AGV收料完成信号
                    if (true)
                    {

                        //set单元定位台AGV收料完成
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台右任务状态, 4, true);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台取料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台取料完成)
                {

                    //查询料库允许入库
                    int ivalue1 = 0;
                    int ivalue2 = 0;
                    bool res1 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackState, out ivalue1);
                    bool res2 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackErrCode, out ivalue2);
                    if (ivalue1 != 1 || ivalue2 > 0)
                    {
                        PushMessage("料库状态异常", Color.Red);
                        return;
                    }
                    int no = StockBinForm.unitDatatemp.NO - 1;

                    //目标库位是否占据t no = StockBinForm.unitDatatemp.NO;
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


                    var rackdata1 = StockBinForm.listRackData.Find(s => s.Tray == TRAYTYPE.空);

                    if (rackdata1 != null && StockBinForm.unitDatatemp == null)
                    {
                        rackdata1.Lock = true;
                        workOrderTasks[i].INNO = rackdata1.NO;

                        //初始化r入库消息缓存
                        StockBinForm.unitDatatemp = new RackUnitData();
                        StockBinForm.unitDatatemp.NO = rackdata1.NO;
                        StockBinForm.unitDatatemp.Tray = workOrderTasks[i].Tray;
                        StockBinForm.unitDatatemp.RfidID = workOrderTasks[i].RfidID;
                        StockBinForm.unitDatatemp.TrayVolume = (workOrderTasks[i].Tray == TRAYTYPE.料盘A) ? 4 : 4;
                        StockBinForm.unitDatatemp.Piece1 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product1.ToString());
                        StockBinForm.unitDatatemp.Piece1Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Piece2 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product2.ToString());
                        StockBinForm.unitDatatemp.Piece2Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Piece3 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product3.ToString());
                        StockBinForm.unitDatatemp.Piece3Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Piece4 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product4.ToString());
                        StockBinForm.unitDatatemp.Piece4Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Lock = rackdata1.Lock;

                        //入库检查指令下发
                        workOrderTasks[i].INNO = rackdata1.NO;
                        AddStaticsDataToDB(rackdata1);
                        positionBind.wMSPositionBind = workOrderTasks[i].OrderNO;


                        //下发AGV送料任务
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMS_On, 1);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInStation, 2);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInBinNo, StockBinForm.unitDatatemp.NO - 1);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.AGVTask, (int)WMSPLC.StationINDEX.UnitRackStation);


                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 3, false);
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 4, false);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到出入库入库运输中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();



                    }
                    else
                    {
                        PushMessage(string.Format("料库已满，工单号{0}的工件无法入库！", workOrderTasks[i].OrderNO), Color.Red);
                    }

                    break;
                }
            }
        }
        private void WorkOrderFitStation2P1Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].FitPosition != FITPOSITION.装配单元2定位台1)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元放料运输中)
                {
                    //检测AGV送料到位
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元放料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元放料运输完成)
                {
                    //给PLCAGV到站信号
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台放料中)
                {
                    //检测是够有AGV上料完成信号
                    if (true)
                    {
                        //给PLC上料完成信号完成
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左任务状态, 2, true);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    else
                    {
                        //检测PLC是否给出上料允许信号
                        bool value;
                        bool res = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左任务状态, 1, out value);
                        if (res && value)
                        {
                            //给AGV滚动料盘信号

                        }
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidfit21.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidfit21.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidfit21.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidfit21.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.装配2定位台左物料类型, (int)table.Piece1);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 0, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 1, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 2, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 3, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 4, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 5, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 6, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 7, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.工件装配中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidfit21.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {
                                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.装配2定位台左物料类型, (int)table.Piece1);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 0, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 1, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 2, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 3, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 4, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 5, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 6, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 7, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左工件状态, 1, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.工件装配中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidfit21.IP), Color.Red);
                            }
                        }
                    }
                    break;

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.工件装配中)
                {

                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.装配2定位台左当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        //更新料盘物料信息，A料盘与B料盘组装后只有一个料盘有组合料，另一个料盘为空料盘
                        workOrderTasks[i].OrderState = ORDERSTATE.工件装配完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;


                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.工件装配完成)
                {

                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左任务状态, 0, out value2);
                    //入库定位台是否空闲（）
                    bool wmsinpositionisempty = false;

                    if (positionBind.aGVBind == 0 && positionBind.wMSPositionBind == 0 && positionBind.wMSPositionBindLock == 0)
                    {
                        positionBind.wMSPositionBindLock = workOrderTasks[i].OrderNO;
                        wmsinpositionisempty = true;
                    }

                    if (wmsinpositionisempty)
                    {
                        //发送AGV取料任务
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元取料运输中;
                        //发送AGV运输任务
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    }
                    break;

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元取料运输中)
                {
                    //检测AGV取料任务到位
                    if (true)
                    {

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元取料运输完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元取料运输完成)
                {  //AGV到位
                    //给立体库
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                }

                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台取料中)
                {
                    //AGV有空，检索目标装配工位

                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO && positionBind.wMSPositionBindLock == workOrderTasks[i].OrderNO)
                    {
                        positionBind.wMSPositionBind = workOrderTasks[i].OrderNO;

                    }

                    //获取AGV收料完成信号
                    if (true)
                    {
                        //查询料库允许入库
                        int value = 0;
                        LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackState, out value);
                        if (value != 1)
                        {
                            return;
                        }
                        //查询AGV上订单号是否一致

                        //下发AGV送料任务
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMS_On, 1);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInStation, 2);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInBinNo, StockBinForm.unitDatatemp.NO -1);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.AGVTask, (int)WMSPLC.StationINDEX.UnitRackStation);
                        //set单元定位台AGV收料完成
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台左任务状态, 4, true);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台取料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台取料完成)
                {
                    //查询料库允许入库
                    int ivalue1 = 0;
                    int ivalue2 = 0;
                    bool res1 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackState, out ivalue1);
                    bool res2 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackErrCode, out ivalue2);
                    if (ivalue1 != 1 || ivalue2 > 0)
                    {
                        PushMessage("料库状态异常", Color.Red);
                        return;
                    }
                    int no = StockBinForm.unitDatatemp.NO - 1;

                    //目标库位是否占据t no = StockBinForm.unitDatatemp.NO;
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


                    var rackdata1 = StockBinForm.listRackData.Find(s => s.Tray == TRAYTYPE.空);

                    if (rackdata1 != null && StockBinForm.unitDatatemp == null)
                    {
                        rackdata1.Lock = true;
                        workOrderTasks[i].INNO = rackdata1.NO;

                        //初始化r入库消息缓存
                        StockBinForm.unitDatatemp = new RackUnitData();
                        StockBinForm.unitDatatemp.NO = rackdata1.NO;
                        StockBinForm.unitDatatemp.Tray = workOrderTasks[i].Tray;
                        StockBinForm.unitDatatemp.RfidID = workOrderTasks[i].RfidID;
                        StockBinForm.unitDatatemp.TrayVolume = (workOrderTasks[i].Tray == TRAYTYPE.料盘A) ? 4 : 4;
                        StockBinForm.unitDatatemp.Piece1 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product1.ToString());
                        StockBinForm.unitDatatemp.Piece1Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Piece2 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product2.ToString());
                        StockBinForm.unitDatatemp.Piece2Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Piece3 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product3.ToString());
                        StockBinForm.unitDatatemp.Piece3Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Piece4 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product4.ToString());
                        StockBinForm.unitDatatemp.Piece4Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Lock = rackdata1.Lock;

                        //入库检查指令下发
                        workOrderTasks[i].INNO = rackdata1.NO;
                        AddStaticsDataToDB(rackdata1);
                        positionBind.wMSPositionBind = workOrderTasks[i].OrderNO;


                        //下发AGV送料任务
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMS_On, 1);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInStation, 2);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInBinNo, StockBinForm.unitDatatemp.NO - 1);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.AGVTask, (int)WMSPLC.StationINDEX.UnitRackStation);


                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 3, false);
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 4, false);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到出入库入库运输中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();



                    }
                    else
                    {
                        PushMessage(string.Format("料库已满，工单号{0}的工件无法入库！", workOrderTasks[i].OrderNO), Color.Red);
                    }



                    break;
                }
            }
        }
        private void WorkOrderFitStation2P2Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].FitPosition != FITPOSITION.装配单元2定位台2)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元放料运输中)
                {
                    //检测AGV送料到位
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元放料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元放料运输完成)
                {
                    //给PLCAGV到站信号
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台放料中)
                {
                    //检测是够有AGV上料完成信号
                    if (true)
                    {
                        //给PLC上料完成信号完成
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右任务状态, 2, true);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    else
                    {
                        //检测PLC是否给出上料允许信号
                        bool value;
                        bool res = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右任务状态, 1, out value);
                        if (res && value)
                        {
                            //给AGV滚动料盘信号

                        }
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidfit22.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidfit22.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidfit22.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidfit22.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.装配2定位台右物料类型, (int)table.Piece1);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 0, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 1, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 2, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 3, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 4, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 5, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 6, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 7, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.工件装配中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidfit22.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {
                                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.装配2定位台右物料类型, (int)table.Piece1);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 0, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 1, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 2, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 3, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 4, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 5, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 6, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右工件状态, 7, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.工件装配中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidfit22.IP), Color.Red);
                            }
                        }
                    }
                    break;

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.工件装配中)
                {

                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.装配2定位台右当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        //更新料盘物料信息，A料盘与B料盘组装后只有一个料盘有组合料，另一个料盘为空料盘
                        workOrderTasks[i].OrderState = ORDERSTATE.工件装配完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;


                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.工件装配完成)
                {

                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右任务状态, 0, out value2);
                    //入库定位台是否空闲（）
                    bool wmsinpositionisempty = false;

                    if (positionBind.aGVBind == 0 && positionBind.wMSPositionBind == 0 && positionBind.wMSPositionBindLock == 0)
                    {
                        positionBind.wMSPositionBindLock = workOrderTasks[i].OrderNO;
                        wmsinpositionisempty = true;
                    }

                    if (wmsinpositionisempty)
                    {
                        //发送AGV取料任务
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元取料运输中;
                        //发送AGV运输任务
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    }
                    break;

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元取料运输中)
                {
                    //检测AGV取料任务到位
                    if (true)
                    {

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元取料运输完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元取料运输完成)
                {  //AGV到位
                    //给立体库
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                }

                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台取料中)
                {
                    //AGV有空，检索目标装配工位

                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO && positionBind.wMSPositionBindLock == workOrderTasks[i].OrderNO)
                    {
                        positionBind.wMSPositionBind = workOrderTasks[i].OrderNO;

                    }

                    //获取AGV收料完成信号
                    if (true)
                    {

                        //set单元定位台AGV收料完成
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配2定位台右任务状态, 4, true);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元定位台取料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到装配单元定位台取料完成)
                {

                    //查询料库允许入库
                    int ivalue1 = 0;
                    int ivalue2 = 0;
                    bool res1 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackState, out ivalue1);
                    bool res2 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackErrCode, out ivalue2);
                    if (ivalue1 != 1 || ivalue2 > 0)
                    {
                        PushMessage("料库状态异常", Color.Red);
                        return;
                    }
                    int no = StockBinForm.unitDatatemp.NO - 1;

                    //目标库位是否占据t no = StockBinForm.unitDatatemp.NO;
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


                    var rackdata1 = StockBinForm.listRackData.Find(s => s.Tray == TRAYTYPE.空);

                    if (rackdata1 != null && StockBinForm.unitDatatemp == null)
                    {
                        rackdata1.Lock = true;
                        workOrderTasks[i].INNO = rackdata1.NO;

                        //初始化r入库消息缓存
                        StockBinForm.unitDatatemp = new RackUnitData();
                        StockBinForm.unitDatatemp.NO = rackdata1.NO;
                        StockBinForm.unitDatatemp.Tray = workOrderTasks[i].Tray;
                        StockBinForm.unitDatatemp.RfidID = workOrderTasks[i].RfidID;
                        StockBinForm.unitDatatemp.TrayVolume = (workOrderTasks[i].Tray == TRAYTYPE.料盘A) ? 4 : 4;
                        StockBinForm.unitDatatemp.Piece1 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product1.ToString());
                        StockBinForm.unitDatatemp.Piece1Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Piece2 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product2.ToString());
                        StockBinForm.unitDatatemp.Piece2Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Piece3 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product3.ToString());
                        StockBinForm.unitDatatemp.Piece3Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Piece4 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product4.ToString());
                        StockBinForm.unitDatatemp.Piece4Quality = PIECEQUALITY.合格;
                        StockBinForm.unitDatatemp.Lock = rackdata1.Lock;

                        //入库检查指令下发
                        workOrderTasks[i].INNO = rackdata1.NO;
                        AddStaticsDataToDB(rackdata1);
                        positionBind.wMSPositionBind = workOrderTasks[i].OrderNO;


                        //下发AGV送料任务
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMS_On, 1);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInStation, 2);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInBinNo, StockBinForm.unitDatatemp.NO - 1);
                        LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.AGVTask, (int)WMSPLC.StationINDEX.UnitRackStation);


                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 3, false);
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.装配1定位台左任务状态, 4, false);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到出入库入库运输中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();



                    }
                    else
                    {
                        PushMessage(string.Format("料库已满，工单号{0}的工件无法入库！", workOrderTasks[i].OrderNO), Color.Red);
                    }
                    break;
                }
            }
        }
        private void WorkOrderCleanStationP1Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].CleanPosition != CLEANPOSITION.清洗单元定位台1)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元放料运输中)
                {
                    //检测AGV送料到位
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元放料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元放料运输完成)
                {
                    //给PLCAGV到站信号
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元定位台放料中)
                {
                    //检测是够有AGV上料完成信号
                    if (true)
                    {
                        //给PLC上料完成信号完成
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左任务状态, 2, true);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    else
                    {
                        //检测PLC是否给出上料允许信号
                        bool value;
                        bool res = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左任务状态, 1, out value);
                        if (res && value)
                        {
                            //给AGV滚动料盘信号

                        }
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidclean1.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidclean1.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidclean1.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidclean1.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.清洗定位台左物料类型, (int)table.Piece1);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 0, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 1, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 2, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 3, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 4, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 5, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 6, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 7, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.工件清洗中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                                    {
                                        positionBind.aGVBind = 0;
                                    }
                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidclean1.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {
                                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.清洗定位台左物料类型, (int)table.Piece1);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 0, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 1, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 2, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 3, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 4, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 5, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 6, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左工件状态, 7, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左RFID状态, 1, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.工件清洗中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidclean1.IP), Color.Red);
                            }
                        }
                    }
                    break;

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.工件清洗中)
                {

                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.清洗定位台左当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.工件清洗完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;


                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.工件清洗完成)
                {

                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左任务状态, 0, out value2);
                    //检测装配定位台是否空闲（左边料盘A，右边料盘B，）
                    bool fitpositionisempty = false;

                    if (workOrderTasks[i].Product1 == PRODUCTTYPE.A料)
                    {
                        if (positionBind.aGVBind == 0 && positionBind.fit1Position1Bind == 0 && (positionBind.fit1Position1BindLock == 0 || positionBind.fit1Position2BindLock == workOrderTasks[i].OrderNO))
                        {
                            positionBind.fit1Position1BindLock = workOrderTasks[i].OrderNO;
                            fitpositionisempty = true;
                        }
                        else if (positionBind.aGVBind == 0 && positionBind.fit2Position1Bind == 0 && (positionBind.fit2Position1BindLock == 0 || positionBind.fit2Position1BindLock == workOrderTasks[i].OrderNO))
                        {
                            positionBind.fit2Position1BindLock = workOrderTasks[i].OrderNO;
                            fitpositionisempty = true;
                        }
                    }
                    else if (workOrderTasks[i].Product1 == PRODUCTTYPE.B料)
                    {
                        if (positionBind.aGVBind == 0 && positionBind.fit1Position2Bind == 0 && (positionBind.fit1Position2BindLock == 0 || positionBind.fit1Position2BindLock == workOrderTasks[i].OrderNO))
                        {
                            positionBind.fit1Position2BindLock = workOrderTasks[i].OrderNO;
                            fitpositionisempty = true;
                        }
                        else if (positionBind.aGVBind == 0 && positionBind.fit2Position2Bind == 0 && (positionBind.fit2Position2BindLock == 0 || positionBind.fit2Position2BindLock == workOrderTasks[i].OrderNO))
                        {
                            positionBind.fit2Position2BindLock = workOrderTasks[i].OrderNO;
                            fitpositionisempty = true;
                        }
                    }

                    if (fitpositionisempty)
                    {
                        //发送AGV取料任务
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元取料运输中;
                        //发送AGV运输任务
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    }
                    break;

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元取料运输中)
                {
                    //检测AGV取料任务到位
                    if (true)
                    {

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元取料运输完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元取料运输完成)
                {  //AGV到位
                    //set单元定位台AGV到站
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                }

                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元定位台取料中)
                {
                    //AGV有空，检索目标装配工位
                    if (workOrderTasks[i].Product1 == PRODUCTTYPE.A料)
                    {
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO && positionBind.fit1Position1BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.fit1Position1Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].FitPosition = FITPOSITION.装配单元1定位台1;

                        }
                        else if (positionBind.aGVBind == workOrderTasks[i].OrderNO && positionBind.fit2Position1BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.fit2Position1Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].FitPosition = FITPOSITION.装配单元2定位台1;

                        }
                    }
                    else if (workOrderTasks[i].Product1 == PRODUCTTYPE.B料)
                    {
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO && positionBind.fit1Position2BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.fit1Position2Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].FitPosition = FITPOSITION.装配单元1定位台2;

                        }
                        else if (positionBind.aGVBind == workOrderTasks[i].OrderNO && positionBind.fit2Position2BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.fit2Position2Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].FitPosition = FITPOSITION.装配单元2定位台2;

                        }
                    }


                    //获取AGV收料完成信号
                    if (true)
                    {

                        //set单元定位台AGV收料完成
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左任务状态, 4, true);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元定位台取料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元定位台取料完成)
                {
                    //AGV送料任务

                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左任务状态, 3, false);
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台左任务状态, 4, false);

                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元放料运输中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    if (positionBind.cleanPosition1Bind == workOrderTasks[i].OrderNO)
                    {
                        positionBind.cleanPosition1Bind = 0;

                        positionBind.cleanPosition1BindLock = 0;
                    }

                    break;
                }

            }
        }
        private void WorkOrderCleanStationP2Doing()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
                    continue;

                if (workOrderTasks[i].CleanPosition != CLEANPOSITION.清洗单元定位台2)
                    continue;

                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元放料运输中)
                {
                    //检测AGV送料到位
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元放料运输完成;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元放料运输完成)
                {
                    //给PLCAGV到站信号
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元定位台放料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元定位台放料中)
                {
                    //检测是够有AGV上料完成信号
                    if (true)
                    {
                        //给PLC上料完成信号完成
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右任务状态, 2, true);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元定位台放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                            positionBind.aGVBind = 0;
                    }
                    else
                    {
                        //检测PLC是否给出上料允许信号
                        bool value;
                        bool res = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右任务状态, 1, out value);
                        if (res && value)
                        {
                            //给AGV滚动料盘信号

                        }
                    }

                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元定位台放料完成)
                {
                    //如果收到请求读取RFID信号
                    bool value;
                    bool res = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右RFID状态, 0, out value);
                    if (res && value)
                    {
                        string rfidid = "";
                        rfidid = LineMainForm.sygolefidclean2.readRFIDUID();
                        if (rfidid == null)
                        {
                            rfidid = LineMainForm.sygolefidclean2.readRFIDUID();
                            if (rfidid == null)
                            {
                                rfidid = LineMainForm.sygolefidclean2.readRFIDUID();
                                if (rfidid == null)
                                {
                                    PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidclean2.IP), Color.Red);
                                }
                            }
                            else
                            {
                                //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                                var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                                if (table != null)
                                {

                                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.清洗定位台右物料类型, (int)table.Piece1);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 0, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 1, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 2, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 3, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 4, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 5, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 6, true);
                                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 7, true);
                                    workOrderTasks[i].OrderState = ORDERSTATE.工件清洗中;
                                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                                }
                                else
                                {
                                    PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidclean2.IP), Color.Red);
                                }

                            }
                        }
                        else
                        {
                            //通过RFID获取料盘信息，完成RFID读取，将物料信息写给PLC
                            var table = Program.Repo.GetSingle<TableUnitData>(p => p.RfidID == rfidid);
                            if (table != null)
                            {
                                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.清洗定位台右物料类型, (int)table.Piece1);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 0, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 1, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 2, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 3, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 4, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 5, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 6, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右工件状态, 7, true);
                                LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右RFID状态, 1, true);
                                workOrderTasks[i].OrderState = ORDERSTATE.工件清洗中;
                                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                            }
                            else
                            {
                                PushMessage(string.Format("未查询到料盘信息", LineMainForm.sygolefidclean2.IP), Color.Red);
                            }
                        }
                    }
                    break;

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.工件清洗中)
                {

                    int value1;
                    bool value2;
                    bool res1 = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.清洗定位台右当前任务完成情况, out value1);
                    if (res1 && value1 == 4)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.工件清洗完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                    break;


                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.工件清洗完成)
                {

                    //请求出料

                    bool value2;
                    bool res2 = LineMainForm.controlplc.GetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右任务状态, 0, out value2);
                    //检测装配定位台是否空闲（左边料盘A，右边料盘B，）
                    bool fitpositionisempty = false;

                    if (workOrderTasks[i].Product1 == PRODUCTTYPE.A料)
                    {
                        if (positionBind.aGVBind == 0 && positionBind.fit1Position1Bind == 0 && (positionBind.fit1Position1BindLock == 0 || positionBind.fit1Position1BindLock == workOrderTasks[i].OrderNO))
                        {
                            positionBind.fit1Position1BindLock = workOrderTasks[i].OrderNO;
                            fitpositionisempty = true;
                        }
                        else if (positionBind.aGVBind == 0 && positionBind.fit2Position1Bind == 0 && (positionBind.fit2Position1BindLock == 0 || positionBind.fit2Position1BindLock == workOrderTasks[i].OrderNO))
                        {
                            positionBind.fit2Position1BindLock = workOrderTasks[i].OrderNO;
                            fitpositionisempty = true;
                        }
                    }
                    else if (workOrderTasks[i].Product1 == PRODUCTTYPE.B料)
                    {
                        if (positionBind.aGVBind == 0 && positionBind.fit1Position2Bind == 0 && (positionBind.fit1Position2BindLock == 0 || positionBind.fit1Position2BindLock == workOrderTasks[i].OrderNO))
                        {
                            positionBind.fit1Position2BindLock = workOrderTasks[i].OrderNO;
                            fitpositionisempty = true;
                        }
                        else if (positionBind.aGVBind == 0 && positionBind.fit2Position2Bind == 0 && (positionBind.fit2Position2BindLock == 0 || positionBind.fit2Position2BindLock == workOrderTasks[i].OrderNO))
                        {
                            positionBind.fit2Position2BindLock = workOrderTasks[i].OrderNO;
                            fitpositionisempty = true;
                        }
                    }

                    if (fitpositionisempty)
                    {
                        //发送AGV取料任务
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元取料运输中;
                        //发送AGV运输任务
                        positionBind.aGVBind = workOrderTasks[i].OrderNO;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    }
                    break;

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元取料运输中)
                {
                    //检测AGV取料任务到位
                    if (true)
                    {

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元取料运输完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元取料运输完成)
                {  //AGV到位
                    //set单元定位台AGV到站
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右任务状态, 3, true);
                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元定位台取料中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                }

                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元定位台取料中)
                {
                    //AGV有空，检索目标装配工位
                    if (workOrderTasks[i].Product1 == PRODUCTTYPE.A料)
                    {
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO && positionBind.fit1Position1BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.fit1Position1Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].FitPosition = FITPOSITION.装配单元1定位台1;

                        }
                        else if (positionBind.aGVBind == workOrderTasks[i].OrderNO && positionBind.fit2Position1BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.fit2Position1Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].FitPosition = FITPOSITION.装配单元2定位台1;

                        }
                    }
                    else if (workOrderTasks[i].Product1 == PRODUCTTYPE.B料)
                    {
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO && positionBind.fit1Position2BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.fit1Position2Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].FitPosition = FITPOSITION.装配单元1定位台2;

                        }
                        else if (positionBind.aGVBind == workOrderTasks[i].OrderNO && positionBind.fit2Position2BindLock == workOrderTasks[i].OrderNO)
                        {
                            positionBind.fit2Position2Bind = workOrderTasks[i].OrderNO;
                            workOrderTasks[i].FitPosition = FITPOSITION.装配单元2定位台2;

                        }
                    }


                    //获取AGV收料完成信号
                    if (true)
                    {

                        //set单元定位台AGV收料完成
                        LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右任务状态, 4, true);

                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到清洗单元定位台取料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.cleanPosition2Bind == workOrderTasks[i].OrderNO)
                        {
                            positionBind.cleanPosition2Bind = 0;
                            positionBind.cleanPosition2BindLock = 0;
                        }
                    }
                    break;
                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到清洗单元定位台取料完成)
                {
                    //AGV送料任务

                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右任务状态, 3, false);
                    LineMainForm.controlplc.SetRegIndexBit((int)ControlPLC.REGINDEX.清洗定位台右任务状态, 4, false);

                    workOrderTasks[i].OrderState = ORDERSTATE.AGV到装配单元放料运输中;
                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                    break;
                }

            }
        }

        private void WorkOrderToWMS()
        {
            for (int i = 0; i < workOrderTasks.Count; i++)
            {
                if (workOrderTasks[i].OrderState == ORDERSTATE.等待)
                    continue;



                if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到出入库入库运输中)
                {

                    //检测AGV送料到入库位置
                    int value = 0;
                    int value1 = 0;
                    LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.AGVArriveed,out value);
                    //AGV收到任务后，MES清除任务子
                    LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.AGVTask, out value1);
                    if (value1>0)
                    {

                        LineMainForm.wmsplc.WriteSingleRegister((int)WMSPLC.DATAINDEX.AGVTaskCMD, 0);
                    }
                    if (value==1)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到出入库元入库运输完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到出入库元入库运输完成)
                {
                    //查询AGBV滚动出料中
                    bool bvalue = false;
                    LineMainForm.wmsplc.GetRegIndexBit((int)WMSPLC.DATAINDEX.TaskStateBit,1, out bvalue);
                    if (bvalue)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到出入库位置放料中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到出入库位置放料中)
                {
                    //查询AGBV滚动出料中
                    bool bvalue1 = false;
                    bool bvalue2 = false;
                    LineMainForm.wmsplc.GetRegIndexBit((int)WMSPLC.DATAINDEX.TaskStateBit, 1, out bvalue1);//辊筒出料中
                    LineMainForm.wmsplc.GetRegIndexBit((int)WMSPLC.DATAINDEX.TaskStateBit, 11, out bvalue1);//AGV物料检测
                    if (!bvalue1&&!bvalue2)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.AGV到出入库位置放料完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                        if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
                        {
                            positionBind.aGVBind = 0;
                        }
                    }

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到出入库位置放料完成)
                {
                    //给PLCAGV到站信号
                    bool bvalue = false;
                    LineMainForm.wmsplc.GetRegIndexBit((int)WMSPLC.DATAINDEX.TaskStateBit, 8, out bvalue);//入库中信号
                    if (bvalue)
                    {
                        if (positionBind.wMSPositionBind == workOrderTasks[i].OrderNO)
                        {
                            positionBind.wMSPositionBind = 0;
                        }
                        if (!StockBinForm.rukuflag)
                        {
                            StockBinForm.rukuflag = true;
                            StockBinForm.rukumanu = false;
                        }
                        workOrderTasks[i].OrderState = ORDERSTATE.自动入库中;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }


                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.自动入库中)
                {
                    //检测入库完成信号

                    bool bvalue = false;
                    LineMainForm.wmsplc.GetRegIndexBit((int)WMSPLC.DATAINDEX.TaskStateBit, 12, out bvalue);//入库完成信号
                    if (bvalue)
                    {
                        workOrderTasks[i].OrderState = ORDERSTATE.自动入库完成;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();
                    }

                }
                else if (workOrderTasks[i].OrderState == ORDERSTATE.自动入库完成)
                {                    
                        workOrderTasks[i].OrderState = ORDERSTATE.结束;
                        workOrderTasks[i].Updatetime = DateTime.Now.ToString();

                }
            }
        }
        //private void WorkOrderToCheckDoing()
        //{
        //    WorkOrderCheStationP1Doing();
        //    WorkOrderCheStationP2Doing();
        //    WorkOrderChekReadP1Doing();
        //    WorkOrderChekReadP2Doing();
        //}
        //private void WorkOrderChekReadP1Doing()
        //{
        //    for (int i = 0; i < workOrderTasks.Count; i++)
        //    {
        //        if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
        //            continue;

        //        if (workOrderTasks[i].CheckPosition != CHECKPOSITION.洗测单元定位台1)
        //            continue;

        //        if (workOrderTasks[i].OrderState == ORDERSTATE.工件洗测中)
        //        {
        //            int value1;
        //            bool res1 = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.三坐标加工完成状态, out value1);
        //            int value2;
        //            bool res2 = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.测量报告MES待读取, out value2);
        //            if (value1 == 1 && value2 == 1)
        //            {
        //                if (workOrderTasks[i].Product1 != PRODUCTTYPE.无 && workOrderTasks[i].P1Quality == PIECEQUALITY.待检测)
        //                {
        //                    //料位1报告读取
        //                    UpdateCoordMeterReasult(workOrderTasks[i], 1);
        //                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.测量报告MES待读取, 0);
        //                    MeasureForm.MeasureOrderNO = workOrderTasks[i].OrderNO;
        //                    MeasureForm.MeasurePicecNo = 1;
        //                }
        //                else if (workOrderTasks[i].Product2 != PRODUCTTYPE.无 && workOrderTasks[i].P2Quality == PIECEQUALITY.待检测)
        //                {
        //                    //料位2报告读取

        //                    UpdateCoordMeterReasult(workOrderTasks[i], 2);
        //                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.测量报告MES待读取, 0);
        //                    MeasureForm.MeasureOrderNO = workOrderTasks[i].OrderNO;
        //                    MeasureForm.MeasurePicecNo = 2;
        //                }
        //                else if (workOrderTasks[i].Product3 != PRODUCTTYPE.无 && workOrderTasks[i].P3Quality == PIECEQUALITY.待检测)
        //                {
        //                    //料位3报告读取

        //                    UpdateCoordMeterReasult(workOrderTasks[i], 3);
        //                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.测量报告MES待读取, 0);
        //                    MeasureForm.MeasureOrderNO = workOrderTasks[i].OrderNO;
        //                    MeasureForm.MeasurePicecNo = 3;
        //                }

        //                else if (workOrderTasks[i].Product4 != PRODUCTTYPE.无 && workOrderTasks[i].P4Quality == PIECEQUALITY.待检测)
        //                {
        //                    //料位4报告读取

        //                    UpdateCoordMeterReasult(workOrderTasks[i], 4);
        //                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.测量报告MES待读取, 0);
        //                    MeasureForm.MeasureOrderNO = workOrderTasks[i].OrderNO;
        //                    MeasureForm.MeasurePicecNo = 4;
        //                }
        //                else
        //                {
        //                    //无待检测料
        //                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.测量报告MES待读取, 0);
        //                }
        //            }

        //        }
        //    }
        //}
        //private void WorkOrderChekReadP2Doing()
        //{
        //    for (int i = 0; i < workOrderTasks.Count; i++)
        //    {
        //        if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
        //            continue;

        //        if (workOrderTasks[i].CheckPosition != CHECKPOSITION.洗测单元定位台2)
        //            continue;

        //        if (workOrderTasks[i].OrderState == ORDERSTATE.工件洗测中)
        //        {
        //            int value1;
        //            bool res1 = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.三坐标加工完成状态, out value1);
        //            int value2;
        //            bool res2 = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.测量报告MES待读取, out value2);
        //            if (value1 == 1 && value2 == 1)
        //            {
        //                if (workOrderTasks[i].Product1 != PRODUCTTYPE.无 && workOrderTasks[i].P1Quality == PIECEQUALITY.待检测)
        //                {
        //                    //料位1报告读取
        //                    //   UpdateCoordMeterReasult(workOrderTasks[i], 1);
        //                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.测量报告MES待读取, 0);
        //                    MeasureForm.MeasureOrderNO = workOrderTasks[i].OrderNO;
        //                    MeasureForm.MeasurePicecNo = 1;
        //                }
        //                else if (workOrderTasks[i].Product2 != PRODUCTTYPE.无 && workOrderTasks[i].P2Quality == PIECEQUALITY.待检测)
        //                {
        //                    //料位2报告读取

        //                    // UpdateCoordMeterReasult(workOrderTasks[i], 2);
        //                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.测量报告MES待读取, 0);
        //                    MeasureForm.MeasureOrderNO = workOrderTasks[i].OrderNO;
        //                    MeasureForm.MeasurePicecNo = 2;
        //                }
        //                else if (workOrderTasks[i].Product3 != PRODUCTTYPE.无 && workOrderTasks[i].P3Quality == PIECEQUALITY.待检测)
        //                {
        //                    //料位3报告读取

        //                    //   UpdateCoordMeterReasult(workOrderTasks[i], 3);
        //                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.测量报告MES待读取, 0);
        //                    MeasureForm.MeasureOrderNO = workOrderTasks[i].OrderNO;
        //                    MeasureForm.MeasurePicecNo = 3;
        //                }

        //                else if (workOrderTasks[i].Product4 != PRODUCTTYPE.无 && workOrderTasks[i].P4Quality == PIECEQUALITY.待检测)
        //                {
        //                    //料位4报告读取

        //                    //  UpdateCoordMeterReasult(workOrderTasks[i], 4);
        //                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.测量报告MES待读取, 0);
        //                    MeasureForm.MeasureOrderNO = workOrderTasks[i].OrderNO;
        //                    MeasureForm.MeasurePicecNo = 4;
        //                }
        //                else
        //                {
        //                    //无待检测料
        //                    LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.测量报告MES待读取, 0);
        //                    MeasureForm.MeasureOrderNO = 0;
        //                    MeasureForm.MeasurePicecNo = 0;
        //                }
        //            }

        //        }
        //    }
        //}

        //private void WorkOrderCheStationP1Doing()
        //{
        //    for (int i = 0; i < workOrderTasks.Count; i++)
        //    {
        //        if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
        //            continue;

        //        if (workOrderTasks[i].CheckPosition != CHECKPOSITION.洗测单元定位台1)
        //            continue;

        //        if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料中)
        //        {
        //            int value;
        //            bool res = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.AGV加工单元定位台取料完成, out value);
        //            if (res && value == 1)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.AGV到加工单元定位台取料, 0);
        //                workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料完成;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //                if (positionBind.process1Position1Bind == workOrderTasks[i].OrderNO)
        //                    positionBind.process1Position1Bind = 0;
        //                else if (positionBind.process1Position2Bind == workOrderTasks[i].OrderNO)
        //                    positionBind.process1Position2Bind = 0;
        //                else if (positionBind.process2Position1Bind == workOrderTasks[i].OrderNO)
        //                    positionBind.process2Position1Bind = 0;
        //                else if (positionBind.process2Position2Bind == workOrderTasks[i].OrderNO)
        //                    positionBind.process2Position2Bind = 0;
        //            }
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料完成)
        //        {
        //            workOrderTasks[i].OrderState = ORDERSTATE.洗测单元定位台允许放料;
        //            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.洗测单元定位台允许放料)
        //        {
        //            LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.AGV到清洗单元定位台放料, 1);
        //            workOrderTasks[i].OrderState = ORDERSTATE.AGV到洗测单元定位台放料中;
        //            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到洗测单元定位台放料中)
        //        {
        //            int value;
        //            bool res = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.AGV清洗单元定位台放料完成, out value);
        //            if (res && value == 1)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.AGV到清洗单元定位台放料, 0);
        //                workOrderTasks[i].OrderState = ORDERSTATE.AGV到洗测单元定位台放料完成;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //                if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
        //                    positionBind.aGVBind = 0;
        //            }
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到洗测单元定位台放料完成)
        //        {
        //            if (workOrderTasks[i].Product1 == PRODUCTTYPE.叶轮)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.清测单元定位台1料盘类型, 1);
        //            }
        //            else if (workOrderTasks[i].Product1 == PRODUCTTYPE.校徽 || workOrderTasks[i].Product2 == PRODUCTTYPE.校徽
        //                || workOrderTasks[i].Product3 == PRODUCTTYPE.校徽 || workOrderTasks[i].Product4 == PRODUCTTYPE.校徽)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.清测单元定位台1料盘类型, 3);
        //            }
        //            else
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.清测单元定位台1料盘类型, 2);
        //            }
        //            LineMainForm.controlplc.WriteMultiRegisters((int)ControlPLC.REGINDEX.清测单元定位台1上物料1,

        //                ConvertMetiral(workOrderTasks[i].Product1, workOrderTasks[i].Product2, workOrderTasks[i].Product3, workOrderTasks[i].Product4));
        //            workOrderTasks[i].OrderState = ORDERSTATE.工件洗测中;
        //            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.工件洗测中)
        //        {
        //            //checkPos1count++;
        //            //if (checkPos1count >= testChecktime)
        //            int value;
        //            bool res = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.清测单元定位台1全部完成情况, out value);
        //            if (res && value == 1)
        //            {
        //                //checkPos1count = 0;
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.清测单元定位台1料盘类型, 0);
        //                int[] values = { 0, 0, 0, 0 };
        //                LineMainForm.controlplc.WriteMultiRegisters((int)ControlPLC.REGINDEX.清测单元定位台1上物料1, values);
        //                workOrderTasks[i].OrderState = ORDERSTATE.工件洗测完成;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //                MeasureForm.MeasureOrderNO = 0;
        //                MeasureForm.MeasurePicecNo = 0;
        //            }
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.工件洗测完成)
        //        {
        //            workOrderTasks[i].OrderState = ORDERSTATE.洗测单元定位台允许取料;
        //            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.洗测单元定位台允许取料)
        //        {
        //            bool bvalue = false;
        //            bool res = LineMainForm.wmsplc.ReadSingleCoil(2106, out bvalue);
        //            //AGV有空，且出入库位置是空的,短线无料
        //            if (positionBind.aGVBind == 0 && positionBind.wMSPositionBind == 0 && res && !bvalue &&   StockBinForm.chukumanu == false &&   StockBinForm.rukumanu == false)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.AGV到清洗单元定位台取料, 1);
        //                positionBind.aGVBind = workOrderTasks[i].OrderNO;
        //                positionBind.wMSPositionBind = workOrderTasks[i].OrderNO;
        //                workOrderTasks[i].OrderState = ORDERSTATE.AGV到洗测单元定位台取料中;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            }
        //            break;
        //        }
        //    }
        //}

        //private void WorkOrderCheStationP2Doing()
        //{
        //    for (int i = 0; i < workOrderTasks.Count; i++)
        //    {
        //        if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
        //            continue;

        //        if (workOrderTasks[i].CheckPosition != CHECKPOSITION.洗测单元定位台2)
        //            continue;

        //        if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料中)
        //        {
        //            int value;
        //            bool res = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.AGV加工单元定位台取料完成, out value);
        //            if (res && value == 1)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.AGV到加工单元定位台取料, 0);
        //                workOrderTasks[i].OrderState = ORDERSTATE.AGV到加工单元定位台取料完成;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //                if (positionBind.process1Position1Bind == workOrderTasks[i].OrderNO)
        //                    positionBind.process1Position1Bind = 0;
        //                else if (positionBind.process1Position2Bind == workOrderTasks[i].OrderNO)
        //                    positionBind.process1Position2Bind = 0;
        //                else if (positionBind.process2Position1Bind == workOrderTasks[i].OrderNO)
        //                    positionBind.process2Position1Bind = 0;
        //                else if (positionBind.process2Position2Bind == workOrderTasks[i].OrderNO)
        //                    positionBind.process2Position2Bind = 0;
        //            }
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到加工单元定位台取料完成)
        //        {

        //            workOrderTasks[i].OrderState = ORDERSTATE.洗测单元定位台允许放料;
        //            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.洗测单元定位台允许放料)
        //        {
        //            LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.AGV到清洗单元定位台放料, 2);
        //            workOrderTasks[i].OrderState = ORDERSTATE.AGV到洗测单元定位台放料中;
        //            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到洗测单元定位台放料中)
        //        {
        //            int value;
        //            bool res = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.AGV清洗单元定位台放料完成, out value);
        //            if (res && value == 1)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.AGV到清洗单元定位台放料, 0);
        //                workOrderTasks[i].OrderState = ORDERSTATE.AGV到洗测单元定位台放料完成;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //                if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
        //                    positionBind.aGVBind = 0;
        //            }
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到洗测单元定位台放料完成)
        //        {
        //            if (workOrderTasks[i].Product1 == PRODUCTTYPE.叶轮)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.清测单元定位台2料盘类型, 1);
        //            }
        //            else if (workOrderTasks[i].Product1 == PRODUCTTYPE.校徽 || workOrderTasks[i].Product2 == PRODUCTTYPE.校徽
        //                || workOrderTasks[i].Product3 == PRODUCTTYPE.校徽 || workOrderTasks[i].Product4 == PRODUCTTYPE.校徽)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.清测单元定位台2料盘类型, 3);
        //            }
        //            else
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.清测单元定位台2料盘类型, 2);
        //            }
        //            LineMainForm.controlplc.WriteMultiRegisters((int)ControlPLC.REGINDEX.清测单元定位台2上物料1,
        //                ConvertMetiral(workOrderTasks[i].Product1, workOrderTasks[i].Product2, workOrderTasks[i].Product3, workOrderTasks[i].Product4));
        //            workOrderTasks[i].OrderState = ORDERSTATE.工件洗测中;
        //            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            MeasureForm.MeasureOrderNO = 0;
        //            MeasureForm.MeasurePicecNo = 0;
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.工件洗测中)
        //        {
        //            //checkPos2count++;
        //            //if (checkPos2count >= testChecktime)
        //            int value;
        //            bool res = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.清测单元定位台2全部完成情况, out value);
        //            if (res && value == 1)
        //            {
        //                //checkPos2count = 0;
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.清测单元定位台2料盘类型, 0);
        //                int[] values = { 0, 0, 0, 0 };
        //                LineMainForm.controlplc.WriteMultiRegisters((int)ControlPLC.REGINDEX.清测单元定位台2上物料1, values);
        //                workOrderTasks[i].OrderState = ORDERSTATE.工件洗测完成;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            }
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.工件洗测完成)
        //        {
        //            workOrderTasks[i].OrderState = ORDERSTATE.洗测单元定位台允许取料;
        //            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.洗测单元定位台允许取料)
        //        {
        //            bool bvalue = false;
        //            bool res = LineMainForm.wmsplc.ReadSingleCoil(2106, out bvalue);
        //            //AGV有空，且出入库位置是空的,短线无料
        //            if (positionBind.aGVBind == 0 && positionBind.wMSPositionBind == 0 && res && !bvalue &&   StockBinForm.chukumanu == false &&   StockBinForm.rukumanu == false)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.AGV到清洗单元定位台取料, 2);
        //                positionBind.aGVBind = workOrderTasks[i].OrderNO;
        //                positionBind.wMSPositionBind = workOrderTasks[i].OrderNO;
        //                workOrderTasks[i].OrderState = ORDERSTATE.AGV到洗测单元定位台取料中;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            }
        //            break;
        //        }
        //    }
        //}

        //private void WorkOrderToWMSDoing()
        //{
        //    for (int i = 0; i < workOrderTasks.Count; i++)
        //    {
        //        if (workOrderTasks[i].OrderState == ORDERSTATE.结束)
        //            continue;

        //        if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到洗测单元定位台取料中)
        //        {
        //            int value;
        //            bool res = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.AGV清洗单元定位台取料完成, out value);
        //            if (res && value == 1)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.AGV到清洗单元定位台取料, 0);
        //                workOrderTasks[i].OrderState = ORDERSTATE.AGV到洗测单元定位台取料完成;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //                if (positionBind.checkPosition1Bind == workOrderTasks[i].OrderNO)
        //                { positionBind.checkPosition1Bind = 0; }

        //                else if (positionBind.checkPosition2Bind == workOrderTasks[i].OrderNO)
        //                { positionBind.checkPosition2Bind = 0; }

        //            }
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到洗测单元定位台取料完成)
        //        {
        //            bool value;
        //            bool res = LineMainForm.wmsplc.ReadSingleCoil(2106, out value);
        //            if (res && !value)
        //            {
        //                workOrderTasks[i].OrderState = ORDERSTATE.出入库位置允许放料;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            }

        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.出入库位置允许放料)
        //        {
        //            var rackdata1 =   StockBinForm.listRackData.Find(s => s.Tray == TRAYTYPE.空);

        //            if (rackdata1 != null &&   StockBinForm.unitDatatemp == null)
        //            {
        //                rackdata1.Lock = true;
        //                workOrderTasks[i].INNO = rackdata1.NO;

        //                //初始化r入库消息缓存
        //                  StockBinForm.unitDatatemp = new RackUnitData();
        //                  StockBinForm.unitDatatemp.NO = rackdata1.NO;
        //                  StockBinForm.unitDatatemp.Tray = workOrderTasks[i].Tray;
        //                  StockBinForm.unitDatatemp.RfidID = workOrderTasks[i].RfidID;
        //                  StockBinForm.unitDatatemp.TrayVolume = (workOrderTasks[i].Tray == TRAYTYPE.叶轮料盘) ? 1 : 4;
        //                  StockBinForm.unitDatatemp.Piece1 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product1.ToString());
        //                  StockBinForm.unitDatatemp.Piece1Quality = PIECEQUALITY.合格;
        //                  StockBinForm.unitDatatemp.Piece2 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product2.ToString());
        //                  StockBinForm.unitDatatemp.Piece2Quality = PIECEQUALITY.合格;
        //                  StockBinForm.unitDatatemp.Piece3 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product3.ToString());
        //                  StockBinForm.unitDatatemp.Piece3Quality = PIECEQUALITY.合格;
        //                  StockBinForm.unitDatatemp.Piece4 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product4.ToString());
        //                  StockBinForm.unitDatatemp.Piece4Quality = PIECEQUALITY.合格;
        //                  StockBinForm.unitDatatemp.Lock = rackdata1.Lock;
        //                //if (!  StockBinForm.rukuflag)
        //                //{
        //                //      StockBinForm.rukuflag = true;
        //                //      StockBinForm.rukumanu = false;
        //                //}
        //                //入库检查指令下发
        //                workOrderTasks[i].INNO = rackdata1.NO;
        //                AddStaticsDataToDB(rackdata1);
        //                positionBind.wMSPositionBind = workOrderTasks[i].OrderNO;

        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.AGV到料库中转放料, 1);
        //                int index =   StockBinForm.unitDatatemp.NO - 1;
        //                LineMainForm.wmsplc.WritesingleRegister(512,   StockBinForm.RackNoCommand[index]);


        //                LineMainForm.wmsplc.WritesingleCoil(401, false);//入库要求送料前给400信号，入库位false
        //                LineMainForm.wmsplc.WritesingleCoil(400, false);//入库要求送料前给400信号，入库位false

        //                workOrderTasks[i].OrderState = ORDERSTATE.AGV到出入库位置中;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();


        //            }
        //            else
        //            {
        //                PushMessage(string.Format("料库已满，工单号{0}的工件无法入库！", workOrderTasks[i].OrderNO), Color.Red);
        //            }


        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到出入库位置中)
        //        {
        //            int value;
        //            bool res = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.自动入库AGV到达料库中转台, out value);
        //            if (res && value == 1)
        //            {
        //                ///
        //                workOrderTasks[i].OrderState = ORDERSTATE.AGV到达出入库位置;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            }
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV到达出入库位置)
        //        {
        //            //AGV 送料到达

        //            LineMainForm.wmsplc.WritesingleCoil(2100, true);
        //            int ivalue = 0;
        //            bool res3 = LineMainForm.wmsplc.ReadsingleRegister(0, out ivalue);

        //            bool res1 = LineMainForm.wmsplc.WritesingleCoil(50, true);
        //            bool value;
        //            bool res = LineMainForm.wmsplc.ReadSingleCoil(2107, out value);
        //            if (res && value && res3 && ivalue == 0)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.料库中转台允许上料, 1);
        //                workOrderTasks[i].OrderState = ORDERSTATE.AGV在出入库位置放料中;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            }

        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV在出入库位置放料中)
        //        {
        //            bool value;
        //            bool res = LineMainForm.wmsplc.ReadSingleCoil(2101, out value);//料框是否离开AGV了
        //            if (res && value)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.料库中转台上料完成, 1);
        //                workOrderTasks[i].OrderState = ORDERSTATE.AGV在出入库位置放料完成;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            }
        //            break;
        //        }

        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.AGV在出入库位置放料完成)
        //        {
        //            int value;
        //            bool res = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.AGV料库中转放料完成, out value);
        //            if (res && value == 1)
        //            {
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.AGV到料库中转放料, 0);
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.料库中转台允许上料, 0);
        //                LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.料库中转台上料完成, 0);

        //                // LineMainForm.wmsplc.WritesingleCoil(2100, false);
        //                if (workOrderTasks[i].INNO > 00)
        //                {
        //                    if (!  StockBinForm.rukuflag)
        //                    {
        //                          StockBinForm.rukuflag = true;
        //                          StockBinForm.rukumanu = false;
        //                    }
        //                    //入库检查指令下发


        //                    if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
        //                        positionBind.aGVBind = 0;
        //                    workOrderTasks[i].OrderState = ORDERSTATE.自动入库中;
        //                    workOrderTasks[i].Updatetime = DateTime.Now.ToString();

        //                }
        //                else
        //                {
        //                    PushMessage(string.Format("未找到匹配库位信息！", workOrderTasks[i].OrderNO), Color.Red);
        //                }
        //                //var rackdata1 =   StockBinForm.listRackData.Find(s => s.Tray == TRAYTYPE.空 /*&&s.NO%7!=0*/);

        //                //if (rackdata1 != null)
        //                //{
        //                //    rackdata1.Lock = true;
        //                //    workOrderTasks[i].INNO = rackdata1.NO;

        //                //    //初始化r入库消息缓存
        //                //      StockBinForm.unitDatatemp = new RackUnitData();
        //                //      StockBinForm.unitDatatemp.NO = rackdata1.NO;
        //                //      StockBinForm.unitDatatemp.Tray = workOrderTasks[i].Tray;
        //                //      StockBinForm.unitDatatemp.TrayVolume = (workOrderTasks[i].Tray == TRAYTYPE.料盘A) ? 1 : 4;
        //                //      StockBinForm.unitDatatemp.Piece1 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product1.ToString());
        //                //      StockBinForm.unitDatatemp.Piece1Quality = PIECEQUALITY.合格;
        //                //      StockBinForm.unitDatatemp.Piece2 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product2.ToString());
        //                //      StockBinForm.unitDatatemp.Piece2Quality = PIECEQUALITY.合格;
        //                //      StockBinForm.unitDatatemp.Piece3 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product3.ToString());
        //                //      StockBinForm.unitDatatemp.Piece3Quality = PIECEQUALITY.合格;
        //                //      StockBinForm.unitDatatemp.Piece4 = (PIECETYTPE)Enum.Parse(typeof(PIECETYTPE), workOrderTasks[i].Product4.ToString());
        //                //      StockBinForm.unitDatatemp.Piece4Quality = PIECEQUALITY.合格;
        //                //      StockBinForm.unitDatatemp.Lock = rackdata1.Lock;
        //                //    if (!  StockBinForm.rukuflag)
        //                //    {
        //                //          StockBinForm.rukuflag = true;
        //                //          StockBinForm.rukumanu = false;
        //                //    }
        //                //    //入库检查指令下发
        //                //    workOrderTasks[i].INNO = rackdata1.NO;
        //                //    AddStaticsDataToDB(rackdata1);
        //                //    positionBind.wMSPositionBind = workOrderTasks[i].OrderNO;
        //                //    if (positionBind.aGVBind == workOrderTasks[i].OrderNO)
        //                //        positionBind.aGVBind = 0;
        //                //    workOrderTasks[i].OrderState = ORDERSTATE.自动入库中;
        //                //    workOrderTasks[i].Updatetime = DateTime.Now.ToString();

        //                //}
        //                //else
        //                //{
        //                //    PushMessage(string.Format("料库已满，工单号{0}的工件无法入库！", workOrderTasks[i].OrderNO), Color.Red);
        //                //}

        //            }
        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.自动入库中)
        //        {
        //            if (  StockBinForm.unitDatatemp == null)
        //            {
        //                workOrderTasks[i].OrderState = ORDERSTATE.自动入库完成;
        //                workOrderTasks[i].Updatetime = DateTime.Now.ToString();

        //            }

        //            break;
        //        }
        //        else if (workOrderTasks[i].OrderState == ORDERSTATE.自动入库完成)
        //        {
        //            if (positionBind.wMSPositionBind == workOrderTasks[i].OrderNO)
        //            {
        //                positionBind.wMSPositionBind = 0;
        //            }
        //            LineMainForm.wmsplc.WritesingleCoil(2100, false);
        //            workOrderTasks[i].OrderState = ORDERSTATE.结束;
        //            workOrderTasks[i].Updatetime = DateTime.Now.ToString();
        //            break;
        //        }
        //    }
        //}

        private void AddStaticsDataToDB(RackUnitData unitData)
        {
            if (unitData.Tray == TRAYTYPE.料盘A)
            {
                if (unitData.Piece1 != PIECETYTPE.无 && unitData.Piece1 != PIECETYTPE.毛坯A)
                {
                    var data = new StaticsData
                    {
                        Product = (PRODUCTTYPE)Enum.Parse(typeof(PRODUCTTYPE), unitData.Piece1.ToString()),
                        PieceQuality = unitData.Piece1Quality
                    };
                    Program.Repo.Insert<StaticsData>(data);
                }
                if (unitData.Piece2 != PIECETYTPE.无 && unitData.Piece2 != PIECETYTPE.毛坯A)
                {
                    var data = new StaticsData
                    {
                        Product = (PRODUCTTYPE)Enum.Parse(typeof(PRODUCTTYPE), unitData.Piece1.ToString()),
                        PieceQuality = unitData.Piece2Quality
                    };
                    Program.Repo.Insert<StaticsData>(data);
                }
                if (unitData.Piece3 != PIECETYTPE.无 && unitData.Piece3 != PIECETYTPE.毛坯A)
                {
                    var data = new StaticsData
                    {
                        Product = (PRODUCTTYPE)Enum.Parse(typeof(PRODUCTTYPE), unitData.Piece1.ToString()),
                        PieceQuality = unitData.Piece3Quality
                    };
                    Program.Repo.Insert<StaticsData>(data);
                }
                if (unitData.Piece4 != PIECETYTPE.无 && unitData.Piece4 != PIECETYTPE.毛坯A)
                {
                    var data = new StaticsData
                    {
                        Product = (PRODUCTTYPE)Enum.Parse(typeof(PRODUCTTYPE), unitData.Piece1.ToString()),
                        PieceQuality = unitData.Piece4Quality
                    };
                    Program.Repo.Insert<StaticsData>(data);
                }
            }
            else if (unitData.Tray == TRAYTYPE.料盘B)
            {
                if (unitData.Piece1 != PIECETYTPE.无 && unitData.Piece1 != PIECETYTPE.毛坯B)
                {
                    var data = new StaticsData
                    {
                        Product = (PRODUCTTYPE)Enum.Parse(typeof(PRODUCTTYPE), unitData.Piece1.ToString()),
                        PieceQuality = unitData.Piece1Quality
                    };
                    Program.Repo.Insert<StaticsData>(data);
                }

                if (unitData.Piece2 != PIECETYTPE.无 && unitData.Piece2 != PIECETYTPE.毛坯B)
                {
                    var data = new StaticsData
                    {
                        Product = (PRODUCTTYPE)Enum.Parse(typeof(PRODUCTTYPE), unitData.Piece2.ToString()),
                        PieceQuality = unitData.Piece2Quality
                    };
                    Program.Repo.Insert<StaticsData>(data);
                }

                if (unitData.Piece3 != PIECETYTPE.无 && unitData.Piece3 != PIECETYTPE.毛坯B)
                {
                    var data = new StaticsData
                    {
                        Product = (PRODUCTTYPE)Enum.Parse(typeof(PRODUCTTYPE), unitData.Piece3.ToString()),
                        PieceQuality = unitData.Piece3Quality
                    };
                    Program.Repo.Insert<StaticsData>(data);
                }

                if (unitData.Piece4 != PIECETYTPE.无 && unitData.Piece4 != PIECETYTPE.毛坯B)
                {
                    var data = new StaticsData
                    {
                        Product = (PRODUCTTYPE)Enum.Parse(typeof(PRODUCTTYPE), unitData.Piece4.ToString()),
                        PieceQuality = unitData.Piece4Quality
                    };
                    Program.Repo.Insert<StaticsData>(data);
                }
            }

        }

        //private bool UpdateCoordMeterReasult(WorkOrderData aorder, int index)
        //{
        //    bool certificate_all = true;
        //    if (index < 1 || index > 4)
        //    {
        //        return false;
        //    }
        //    string mode = "";

        //    switch (index)
        //    {
        //        case 1:
        //            mode = aorder.Product1.ToString();
        //            break;
        //        case 2:
        //            mode = aorder.Product2.ToString();
        //            break;
        //        case 3:
        //            mode = aorder.Product3.ToString();
        //            break;
        //        case 4:
        //            mode = aorder.Product4.ToString();
        //            break;
        //    }
        //    if (mode == "无")
        //    {
        //        switch (index)
        //        {
        //            case 1:
        //                aorder.P1Quality = PIECEQUALITY.异常;
        //                break;
        //            case 2:
        //                aorder.P2Quality = PIECEQUALITY.异常;
        //                break;
        //            case 3:
        //                aorder.P3Quality = PIECEQUALITY.异常;
        //                break;
        //            case 4:
        //                aorder.P4Quality = PIECEQUALITY.异常;
        //                break;
        //        }
        //        Program.Repo.Update(aorder);
        //        return false;
        //    }


        //    var modelisttemp = Program.Repo.Get<MeterMode>(p => p.ModeSN == mode);
        //    string path = @"\\192.168.8.31\celiangjieguo\JIEGUO.txt"; //"C:\\Users\\Public\\MESFile\\MeterResult\\xmlReport.xml";
        //    FileStream stream = File.OpenRead(path);
        //    var doc = new XmlDocument();
        //    doc.Load(stream);
        //    XmlNode node = doc.SelectSingleNode("/root/Results/values");
        //    if (node == null)
        //    {
        //        MessageBox.Show("测量报告读取错误！");
        //        stream.Close();
        //        switch (index)
        //        {
        //            case 1:
        //                aorder.P1Quality = PIECEQUALITY.异常;
        //                break;
        //            case 2:
        //                aorder.P2Quality = PIECEQUALITY.异常;
        //                break;
        //            case 3:
        //                aorder.P3Quality = PIECEQUALITY.异常;
        //                break;
        //            case 4:
        //                aorder.P4Quality = PIECEQUALITY.异常;
        //                break;
        //        }

        //        Program.Repo.Update(aorder);
        //        return false;
        //    }
        //    var rank = 0;
        //    if (modelisttemp.Count < 1)
        //    {
        //        MessageBox.Show("无测量模板数据！");
        //        stream.Close();
        //        switch (index)
        //        {
        //            case 1:
        //                aorder.P1Quality = PIECEQUALITY.异常;
        //                break;
        //            case 2:
        //                aorder.P2Quality = PIECEQUALITY.异常;
        //                break;
        //            case 3:
        //                aorder.P3Quality = PIECEQUALITY.异常;
        //                break;
        //            case 4:
        //                aorder.P4Quality = PIECEQUALITY.异常;
        //                break;
        //        }
        //        Program.Repo.Update(aorder);
        //        return false;
        //    }
        //    if (node.ChildNodes.Count != modelisttemp.Count)
        //    {
        //        MessageBox.Show("测量结果与测量模板检测数量不匹配！");
        //        stream.Close();
        //        switch (index)
        //        {
        //            case 1:
        //                aorder.P1Quality = PIECEQUALITY.异常;
        //                break;
        //            case 2:
        //                aorder.P2Quality = PIECEQUALITY.异常;
        //                break;
        //            case 3:
        //                aorder.P3Quality = PIECEQUALITY.异常;
        //                break;
        //            case 4:
        //                aorder.P4Quality = PIECEQUALITY.异常;
        //                break;
        //        }
        //        Program.Repo.Update(aorder);
        //        return false;
        //    }

        //    foreach (XmlNode item in node.ChildNodes)
        //    {
        //        rank++;
        //        var modetemp = Program.Repo.GetSingle<MeterMode>(p => p.ModeSN == mode && p.Rank == rank);

        //        bool get = false;
        //        foreach (XmlAttribute attri in item.Attributes)
        //        {
        //            double value = 0.0;
        //            switch (attri.Name.ToUpper())
        //            {
        //                case "MEASURE":
        //                    string values = attri.Value.ToString();
        //                    value = Convert.ToDouble(values);
        //                    values = value.ToString("F3");
        //                    value = Convert.ToDouble(values);
        //                    get = true;
        //                    break;
        //                default:
        //                    break;
        //            }
        //            if (get)
        //            {
        //                if (Program.Repo.Exist<MeterResult>(p => p.OrderId == aorder.Id && p.Rank == rank && p.ModeSN == mode && p.WorkpieceNumber == index))
        //                {
        //                    var data = Program.Repo.GetSingle<MeterResult>(p => p.OrderId == aorder.Id && p.Rank == rank && p.ModeSN == mode && p.WorkpieceNumber == index);
        //                    data.SizeValue = value;
        //                    if (value < modetemp.SizeDataStandard + modetemp.SizeDataMin || value > modetemp.SizeDataStandard + modetemp.SizeDataMax)
        //                    {
        //                        data.IsCertificate = false;
        //                        certificate_all = false;
        //                    }
        //                    else data.IsCertificate = true;
        //                    Program.Repo.Update(data);
        //                }
        //                else
        //                {
        //                    bool IsCertificate = false;
        //                    if (value < modetemp.SizeDataStandard + modetemp.SizeDataMin || value > modetemp.SizeDataStandard + modetemp.SizeDataMax)
        //                    {
        //                        IsCertificate = false;

        //                        certificate_all = false;
        //                    }
        //                    else IsCertificate = true;
        //                    //插入一条检测记录
        //                    var data = new MeterResult
        //                    {
        //                        OrderId = aorder.Id,
        //                        ModeSN = mode,
        //                        NUM = modetemp.NUM,
        //                        Rank = rank,
        //                        IsCertificate = IsCertificate,
        //                        SizeDataMin = modetemp.SizeDataMin,
        //                        SizeDataMax = modetemp.SizeDataMax,
        //                        SizeDataStandard = modetemp.SizeDataStandard,
        //                        SizeValue = value,
        //                    };

        //                    Program.Repo.Insert(data);
        //                }
        //            }

        //        }

        //        if (certificate_all == false)
        //        {
        //            switch (index)
        //            {
        //                case 1:
        //                    aorder.P1Quality = PIECEQUALITY.不合格;
        //                    break;
        //                case 2:
        //                    aorder.P2Quality = PIECEQUALITY.不合格;
        //                    break;
        //                case 3:
        //                    aorder.P3Quality = PIECEQUALITY.不合格;
        //                    break;
        //                case 4:
        //                    aorder.P4Quality = PIECEQUALITY.不合格;
        //                    break;
        //            }
        //        }
        //        else
        //        {
        //            switch (index)
        //            {
        //                case 1:
        //                    aorder.P1Quality = PIECEQUALITY.合格;
        //                    break;
        //                case 2:
        //                    aorder.P2Quality = PIECEQUALITY.合格;
        //                    break;
        //                case 3:
        //                    aorder.P3Quality = PIECEQUALITY.合格;
        //                    break;
        //                case 4:
        //                    aorder.P4Quality = PIECEQUALITY.合格;
        //                    break;
        //            }
        //        }
        //        Order_Meter_latest = aorder.Id;
        //        Program.Repo.Update(aorder);
        //    }
        //    stream.Close();
        //    return true;
        //}

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
                    temp = temp.Trim();
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
            string path = @"\\192.168.8.31\celiangjieguo\JIEGUO.txt";
            //FileStream stream = File.OpenRead(path);
            //var doc = new XmlDocument();
            //doc.Load(stream);
            //XmlNode node = doc.SelectSingleNode("/root/Results/values");
            string[] content;
            lock (MeasureForm.readder)
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
                    OrderExcuteForm.Order_Meter_latest = aorder.Id;
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
                        ModeSN = mode,
                        NUM = modetemp.NUM,
                        Rank = rank,
                        WorkpieceNumber = index,
                        IsCertificate = IsCertificate,
                        SizeDataMin = modetemp.SizeDataMin,
                        SizeDataMax = modetemp.SizeDataMax,
                        SizeDataStandard = modetemp.SizeDataStandard,
                        SizeValue = value,
                    };
                    OrderExcuteForm.Order_Meter_latest = aorder.Id;
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

            OrderExcuteForm.Order_Meter_latest = aorder.Id;
            Program.Repo.Update(aorder);
            return true;
        }



        private void InAction()
        {
            if (!StockBinForm.rukuflag)
                return;

            Console.WriteLine("{0}, {1}, {2}", StockBinForm.rukuflag, DateTime.Now, StockBinForm.rukustatus);
            
            switch (StockBinForm.rukustatus)
            {
                case StockBinForm.MANUALIN.结束:
                    {
                        if (StockBinForm.unitDatatemp != null && LineMainForm.wmsplc.GetOnlineState())
                        {

                            StockBinForm.rukustatus = StockBinForm.MANUALIN.检查料库状态;
                        }
                    }
                    break;
                case StockBinForm.MANUALIN.检查料库状态:
                    {
                        int ivalue1 = 0;
                        int ivalue2 = 0;
                        bool res1 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackState, out ivalue1);
                        bool res2 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackErrCode, out ivalue2);

                        int no = StockBinForm.unitDatatemp.NO;
                        int reg = no / 16;
                        int bit = no % 16;
                        bool res3 = false;
                        int ivalue3 = 0;
                        if(bit< 8 )//
                        {
                             LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.BinState1 + reg, out ivalue3);
                             res3 = ControlPLC.GetBoolValue(ivalue3, bit+8);//查询到入库完成信号
                        }
                       else
                        {
                            LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.BinState1 +reg , out ivalue3);
                            res3 = ControlPLC.GetBoolValue(ivalue3, bit-8);//查询到入库完成信号
                        }
                        if(res3)
                        {
                            StockBinForm.rukustatus = StockBinForm.MANUALIN.入库异常;
                            PushMessage("入库位置已经有料盘，不能入库", Color.Red);
                        }
                        if (res1 && res2 && ivalue1 == 1 && ivalue2 == 0)
                        {

                            StockBinForm.rukustatus = StockBinForm.MANUALIN.RFID检测中;
                        }
                        else
                        {
                            StockBinForm.rukustatus = StockBinForm.MANUALIN.入库异常;
                            PushMessage("料库状态异常", Color.Red);

                        }
                    }
                    break;
                case StockBinForm.MANUALIN.RFID检测中:
                    {
                        if (StockBinForm.rukumanu == true)//手动入库料盘RFID芯片绑定
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
                                        StockBinForm.unitDatatemp.RfidID = "123456789";//2023测试用
                                        StockBinForm.rukustatus = StockBinForm.MANUALIN.RFID检测完成;

                                        //PushMessage(string.Format("RFID读取失败", LineMainForm.sygolefidwmsin.IP), Color.Red);
                                        //StockBinForm.rukustatus = StockBinForm.MANUALIN.入库异常;
                                    }
                                }
                                else
                                {
                                    StockBinForm.unitDatatemp.RfidID = rfidid;
                                    StockBinForm.rukustatus = StockBinForm.MANUALIN.RFID检测完成;

                                }
                            }
                            else
                            {
                                StockBinForm.unitDatatemp.RfidID = rfidid;
                                StockBinForm.rukustatus = StockBinForm.MANUALIN.RFID检测完成;
                            }
                        }   //RFID的ID号与入库盘绑定
                        else//RFIDID与料盘绑定的RFID信息比对
                        {
                            StockBinForm.rukustatus = StockBinForm.MANUALIN.RFID检测完成;

                        }
                    }
                    break;
                case StockBinForm.MANUALIN.RFID检测完成:
                    {
                        int ivalue1 = 0;
                        int ivalue2 = 0;
                        bool res1 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackState, out ivalue1);
                        bool res2 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackErrCode, out ivalue2);
                        if (res1 && ivalue1 == 1 && res2 && ivalue2 == 0)
                        {

                            LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMS_On, 1);
                            LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInBinNo, StockBinForm.unitDatatemp.NO);
                            if (StockBinForm.rukumanu == true)
                            {
                                LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInStation, 1);
                            }
                            else
                            {
                                LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInStation, 2);
                            }
                            StockBinForm.rukustatus = StockBinForm.MANUALIN.入库中;
                        }
                        else
                        {

                            PushMessage("料库状态异常", Color.Red);
                            StockBinForm.rukustatus = StockBinForm.MANUALIN.入库异常;
                        }
                    }
                    break;
                case StockBinForm.MANUALIN.入库中:
                    {
                        int ivalue = 0;

                        //LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.TaskStateBit, out ivalue);
                        bool bit1 = false;
                        bool bit2 = false;
                        LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.TaskStateBit, out ivalue);
                        //bool value;
                        LineMainForm.wmsplc.GetRegIndexBit((int)WMSPLC.DATAINDEX.TaskStateBit, (int)WMSPLC.TaskStateBit1.RackIning, out bit1);
                        LineMainForm.wmsplc.GetRegIndexBit((int)WMSPLC.DATAINDEX.TaskStateBit, (int)WMSPLC.TaskStateBit1.RackInFinish, out bit2);
                        //var res1 = ControlPLC.GetBoolValue(ivalue, (int)WMSPLC.TaskStateBit1.RackIning);//查询到入库信号

                        //var res2 = ControlPLC.GetBoolValue(ivalue, (int)WMSPLC.TaskStateBit1.RackInFinish);//查询到入库完成信号

                        if (bit1)//清除入库任务
                        {
                            LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMS_On, 0);
                            LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInBinNo, 0);
                            LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSInStation, 0);
                        }
                        if (bit2)
                        {
                            StockBinForm.rukustatus = StockBinForm.MANUALIN.入库完成;
                        }
                    }
                    break;
                case StockBinForm.MANUALIN.入库异常:
                    {
                        if (StockBinForm.unitDatatemp != null)
                        {
                            StockBinForm.unitDatatemp.Lock = false;
                            var rackdata = StockBinForm.listRackData.Find(s => s.NO == StockBinForm.unitDatatemp.NO);
                            rackdata.NO = StockBinForm.unitDatatemp.NO;
                            rackdata.RfidID = StockBinForm.unitDatatemp.RfidID;
                            rackdata.Tray = StockBinForm.unitDatatemp.Tray;
                            rackdata.TrayVolume = StockBinForm.unitDatatemp.TrayVolume;
                            rackdata.Piece1 = StockBinForm.unitDatatemp.Piece1;
                            rackdata.Piece1Quality = StockBinForm.unitDatatemp.Piece1Quality;
                            rackdata.Piece2 = StockBinForm.unitDatatemp.Piece2;
                            rackdata.Piece2Quality = StockBinForm.unitDatatemp.Piece2Quality;
                            rackdata.Piece3 = StockBinForm.unitDatatemp.Piece3;
                            rackdata.Piece3Quality = StockBinForm.unitDatatemp.Piece3Quality;
                            rackdata.Piece4 = StockBinForm.unitDatatemp.Piece4;
                            rackdata.Piece4Quality = StockBinForm.unitDatatemp.Piece4Quality;
                            rackdata.Lock = StockBinForm.unitDatatemp.Lock;
                            //unitList[unitDatatemp.NO - 1].ShowContent(unitDatatemp);

                            OnRackAction1(false, rackdata.NO);

                            StockBinForm.rukuflag = false;
                            StockBinForm.rukumanu = false;
                            StockBinForm.rukustatus = StockBinForm.MANUALIN.结束;
                            StockBinForm.unitDatatemp = null;
                            Console.WriteLine("{0}, {1}, {2}", StockBinForm.chukuflag, DateTime.Now, StockBinForm.rukustatus);
                        }
                    }
                    break;
                case StockBinForm.MANUALIN.入库完成:
                    {
                        if (StockBinForm.unitDatatemp != null)
                        {
                            StockBinForm.unitDatatemp.Lock = false;
                            var rackdata = StockBinForm.listRackData.Find(s => s.NO == StockBinForm.unitDatatemp.NO);
                            rackdata.NO = StockBinForm.unitDatatemp.NO;
                            rackdata.Tray = StockBinForm.unitDatatemp.Tray;
                            rackdata.RfidID = StockBinForm.unitDatatemp.RfidID;
                            rackdata.TrayVolume = StockBinForm.unitDatatemp.TrayVolume;
                            rackdata.Piece1 = StockBinForm.unitDatatemp.Piece1;
                            rackdata.Piece1Quality = StockBinForm.unitDatatemp.Piece1Quality;
                            rackdata.Piece2 = StockBinForm.unitDatatemp.Piece2;
                            rackdata.Piece2Quality = StockBinForm.unitDatatemp.Piece2Quality;
                            rackdata.Piece3 = StockBinForm.unitDatatemp.Piece3;
                            rackdata.Piece3Quality = StockBinForm.unitDatatemp.Piece3Quality;
                            rackdata.Piece4 = StockBinForm.unitDatatemp.Piece4;
                            rackdata.Piece4Quality = StockBinForm.unitDatatemp.Piece4Quality;
                            rackdata.Lock = StockBinForm.unitDatatemp.Lock;
                            //unitList[unitDatatemp.NO - 1].ShowContent(unitDatatemp);

                            StockBinForm.rackchanged[rackdata.NO-1]=true;
                            //OnRackAction1(false, rackdata.NO);
                         
                            StockBinForm.rukuflag = false;
                            StockBinForm.rukumanu = false;
                            StockBinForm.rukustatus = StockBinForm.MANUALIN.结束;

                            StockBinForm.unitDatatemp = null;
                            Console.WriteLine("{0}, {1}, {2}", StockBinForm.chukuflag, DateTime.Now, StockBinForm.rukustatus);

                           // PushMessage(string.Format("手动入库到仓位{0}完成.", rackdata.NO), Color.Black);
                        }
                    }
                    break;
            }
        }
        private void OutAction()
        {
            if (!StockBinForm.chukuflag)
            return;

            Console.WriteLine("{0}, {1}, {2}", StockBinForm.chukuflag, DateTime.Now, StockBinForm.chukustatus);
            switch (StockBinForm.chukustatus)
            {
                case StockBinForm.MANUALOUT.结束:
                    {
                        if (StockBinForm.unitDatatemp != null && LineMainForm.wmsplc.GetOnlineState())
                        {
                            StockBinForm.chukustatus = StockBinForm.MANUALOUT.检查料库状态;
                        }
                    }
                    break;
                case StockBinForm.MANUALOUT.检查料库状态:
                    {
                        int ivalue1 = 0;
                        int ivalue2 = 0;
                        bool res1 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackState, out ivalue1);
                        bool res2 = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.RackErrCode, out ivalue2);

                        if (res1 && res2 && ivalue1 == 1 && ivalue2 == 0)
                        {
                            //出库任务，给出出库位、出库、及目标定位台编号
                            LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMS_On,2);
                            LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSOutBinNo, StockBinForm.unitDatatemp.NO);
                            LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.AGVTaskCMD, StockBinForm.unitDatatemp.StationNo);
                            if (StockBinForm.chukumanu == true)
                            {
                                LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSOutStation, 1);
                            }
                            else
                            {
                                LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSOutStation, 2);
                            }
                            StockBinForm.chukustatus = StockBinForm.MANUALOUT.出库中;
                        }
                        else
                        {
                            StockBinForm.chukustatus = StockBinForm.MANUALOUT.出库异常;
                            PushMessage("料库状态异常", Color.Red);
                        }
                    }
                    break;
                case StockBinForm.MANUALOUT.出库中:
                    {
                        int ivalue = 0;
                        bool res = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.AGVTask, out ivalue);
                        if (res&& ivalue>0)
                        {
                            //清除AGV任务
                            LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.AGVTask, 0);
                        }
                       

                        bool bit1 = false;
                        bool bit2 = false;
                        LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAINDEX.TaskStateBit, out ivalue);
                        //bool value;
                        LineMainForm.wmsplc.GetRegIndexBit((int)WMSPLC.DATAINDEX.TaskStateBit, (int)WMSPLC.TaskStateBit1.RackOuting, out bit1);
                        LineMainForm.wmsplc.GetRegIndexBit((int)WMSPLC.DATAINDEX.TaskStateBit, (int)WMSPLC.TaskStateBit1.RackOutFinish, out bit2);


                        if (bit1)//清除出库任务
                        {
                            LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMS_On, 0);
                            LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSOutBinNo, 0);
                            LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAINDEX.WMSOutStation, 0);
                        }
                        if (bit2)
                        {
                            StockBinForm.chukustatus = StockBinForm.MANUALOUT.出库完成;
                        }
                    }
                    break;
                case StockBinForm.MANUALOUT.出库异常:
                    {
                        if (StockBinForm.unitDatatemp != null)
                        {
                            StockBinForm.unitDatatemp.Lock = false;
                            var rackdata = StockBinForm.listRackData.Find(s => s.NO == StockBinForm.unitDatatemp.NO);

                            if (rackdata == null)
                            {
                                Console.WriteLine("未找到对应的仓位信息");
                                break;
                            }

                            rackdata.NO = StockBinForm.unitDatatemp.NO;
                            rackdata.Tray = TRAYTYPE.空;
                            rackdata.TrayVolume = 0;
                            rackdata.RfidID = "";
                            rackdata.Piece1 = PIECETYTPE.无;
                            rackdata.Piece1Quality = PIECEQUALITY.待检测;
                            rackdata.Piece2 = PIECETYTPE.无;
                            rackdata.Piece2Quality = PIECEQUALITY.待检测;
                            rackdata.Piece3 = PIECETYTPE.无;
                            rackdata.Piece3Quality = PIECEQUALITY.待检测;
                            rackdata.Piece4 = PIECETYTPE.无;
                            rackdata.Piece4Quality = PIECEQUALITY.待检测;
                            rackdata.Lock = false;

                            StockBinForm.rackchanged[rackdata.NO - 1] = true;
                            //OnRackAction1(true, rackdata.NO);

                            StockBinForm.chukuflag = false;
                            StockBinForm.chukumanu = false;
                            StockBinForm.chukustatus = StockBinForm.MANUALOUT.结束;

                            StockBinForm.unitDatatemp = null;
                            Console.WriteLine("{0}, {1}, {2}", StockBinForm.chukuflag, DateTime.Now, StockBinForm.chukustatus);
                        }
                    }
                    break;
                case StockBinForm.MANUALOUT.出库完成:
                    {
                        if (StockBinForm.unitDatatemp != null)
                        {
                            StockBinForm.unitDatatemp.Lock = false;
                            var rackdata = StockBinForm.listRackData.Find(s => s.NO == StockBinForm.unitDatatemp.NO);

                            if (rackdata == null)
                            {
                                Console.WriteLine("未找到对应的仓位信息");
                                break;
                            }
                            rackdata.NO = StockBinForm.unitDatatemp.NO;
                            rackdata.Tray = TRAYTYPE.空;
                            rackdata.TrayVolume = 0;
                            rackdata.RfidID = "";
                            rackdata.Piece1 = PIECETYTPE.无;
                            rackdata.Piece1Quality = PIECEQUALITY.待检测;
                            rackdata.Piece2 = PIECETYTPE.无;
                            rackdata.Piece2Quality = PIECEQUALITY.待检测;
                            rackdata.Piece3 = PIECETYTPE.无;
                            rackdata.Piece3Quality = PIECEQUALITY.待检测;
                            rackdata.Piece4 = PIECETYTPE.无;
                            rackdata.Piece4Quality = PIECEQUALITY.待检测;
                            rackdata.Lock = false;
                            //unitList[unitDatatemp.NO - 1].ShowContent(unitDatatemp);


                            StockBinForm.rackchanged[rackdata.NO - 1] = true;
                          //  OnRackAction1(true, rackdata.NO);

                            StockBinForm.chukuflag = false;
                            StockBinForm.chukumanu = false;
                            StockBinForm.chukustatus = StockBinForm.MANUALOUT.结束;

                            StockBinForm.unitDatatemp = null;
                            Console.WriteLine("{0}, {1}, {2}", StockBinForm.chukuflag, DateTime.Now, StockBinForm.chukustatus);
                        }
                    }
                    break;
            }
        }
     

        private void button5_Click(object sender, EventArgs e)
        {
            if (positionBind.wMSPositionBind != 0)
            {
                positionBind.wMSPositionBind = 0;
                StockBinForm.chukumanu = false;
                StockBinForm.rukumanu = false;
            }
        }

        private void buttonagv_Click(object sender, EventArgs e)
        {
            if (positionBind.aGVBind != 0)
            {
                positionBind.aGVBind = 0;
            }
        }

        private void buttonxice1_Click(object sender, EventArgs e)
        {
            if (positionBind.check1Position1Bind != 0)
            {
                positionBind.check1Position1Bind = 0;
            }
        }

        private void buttonxice2_Click(object sender, EventArgs e)
        {
            if (positionBind.check2Position2Bind != 0)
            {
                positionBind.check2Position2Bind = 0;
            }
        }

        private void buttonjiagong11_Click(object sender, EventArgs e)
        {
            if (positionBind.process1Position1Bind != 0)
            {
                positionBind.process1Position1BindLock = 0;
                positionBind.process1Position1Bind = 0;
            }
        }

        private void buttonjaigong12_Click(object sender, EventArgs e)
        {
            if (positionBind.process1Position2Bind != 0)
            {
                positionBind.process1Position2BindLock = 0;
                positionBind.process1Position2Bind = 0;
            }
        }

        private void buttonjaigong21_Click(object sender, EventArgs e)
        {
            if (positionBind.process2Position1Bind != 0)
            {
                positionBind.process2Position1BindLock = 0;
                positionBind.process2Position1Bind = 0;
            }
        }

        private void buttonjiagong22_Click(object sender, EventArgs e)
        {
            if (positionBind.process2Position2Bind != 0)
            {
                positionBind.process2Position2BindLock = 0;
                positionBind.process2Position2Bind = 0;
            }
        }


    }
}
