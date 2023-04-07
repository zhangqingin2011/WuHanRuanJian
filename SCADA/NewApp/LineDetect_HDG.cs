using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HNCAPI;
using System.Threading;
using HNC_MacDataService;
using HNC.MES.Common;
using HNC.MES.Model;
using HNC.MES.BLL;

namespace SCADA.NewApp
{
  
    public partial class LineDetect_HDG : LineDetectBase<ZYLY>
    {
        private Label labelApparatusState = new Label() { Text = "测量仪状态" };
        private Label labelApparatusExceptionalState = new Label() { Text = "测量仪异常状态", Visible = false };
        private Button btnMeasureStandard = new Button() { Text = "手动标定" };
        Button buttonMea = new Button() { Text = "启动测量" };
        /// <summary>
        /// 误差（内径和外径误差范围 -0.1mm~ +0.1mm 之间需要补偿）
        /// </summary>
        private double valueError = 0.1;
        private static bool measureres = false;

        /// <summary>
        /// 内径
        /// </summary>
        public double InnerDiameter { get; private set; }

        /// <summary>
        /// 外径
        /// </summary>
        public double OuterDiameter { get; private set; }

        private BackgroundWorker PLCworker = new BackgroundWorker();
        int counter = 0;
        private string machineState;

        private ApparatusStates State;
        private ApparatusStates Statetmp;

        public LineDetect_HDG()
        {
            InitializeComponent();
            this.FormClosing += LineDetect_HDG_FormClosing;
        }

        void LineDetect_HDG_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                if (gauge != null)
                {
                    gauge.Close();
                }
            }
            catch (Exception)
            {
                //throw;
            }
        }

        protected override void InitControl()
        {
            //gauge = new ZYLY(0x03, "COM2");
            //gauge = new ZYLY(0x03, "COM4");
            gauge = new ZYLY(0x03, "COM9");
            plc = new Plc_HDG();
            base.InitControl();
            btnMeasureStandard.Click += btnMeasureStandard_Click;
            AddButton(btnMeasureStandard);
            buttonMea.Click += buttonMea_Click;
            AddButton(buttonMea);//验收时注释掉
            //buttonMea.Enabled = false;
            AddLabelState(labelApparatusState);
            AddLabelState(labelApparatusExceptionalState);
            AddColumn("内径", "InnerDiameter");
            AddColumn("外径", "OuterDiameter");
            AddColumn("说明", "explain");
            MeasureCompleted += LineDetect_HDG_MeasureCompleted;
            gauge.ApparatusStateChanged += gauge_ApparatusStateChanged;
            gauge.ApparatusExceptionalStateChanged += gauge_ApparatusExceptionalStateChanged;
            plc.ShangBiaoJian += plc_ShangBiaoJian;
            plc.ShangGongJian += plc_ShangGongJian;
            gauge.Start();
            plc.Start();
            UpdateMesEquipmentState(TEquipment.EnumState.运行);
            PLCworker.DoWork += PLCworker_DoWork;
            PLCworker.RunWorkerAsync();
        }

        async void PLCworker_DoWork(object sender, DoWorkEventArgs e)
        {
            var bw = (BackgroundWorker)sender;
            while (true)
            {
                Thread.Sleep(1000);
                counter++;
                int value = 0;
                Collector.CollectShare.GetRegValue((int)HncRegType.REG_TYPE_B, 15, out value, MainForm.plc_dbNo);
                if (value < 0)
                {
                    continue;
                }
                if ((value & 0x02) == 0x02)
                {
                    MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 1, MainForm.plc_dbNo);
                    //MacDataService.GetInstance().HNC_RegSetValue((int)HncRegType.REG_TYPE_B, 15, 0, MainForm.plc_dbNo);
                    while (machineState == Enum.GetName(typeof(ApparatusStates), ApparatusStates.就绪))
                    {
                        await gauge.SendMeasureCmd(plc.LatheNum);
                        Thread.Sleep(TimeSpan.FromSeconds(0.3));
                        Console.WriteLine("后端盖测量命令下达,状态={0}", machineState);
                    }
                    //Console.WriteLine("后端盖开始测量,状态={0}", machineState);
                    MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 1, MainForm.plc_dbNo);
                }
                else if ((value & 0x08) == 0x08)
                {
                    MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 3, MainForm.plc_dbNo);
                    //MacDataService.GetInstance().HNC_RegSetValue((int)HncRegType.REG_TYPE_B, 15, 0, MainForm.plc_dbNo);
                    await gauge.SendProofCmd(plc.LatheNum);
                    Thread.Sleep(2000);
                    MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 3, MainForm.plc_dbNo);
                }
                if (counter > 10)
                {
                    //Console.WriteLine("后端盖运行中value={0}", value);
                    counter = 0;
                }
            }
        }

        void LineDetect_HDG_MeasureCompleted(object sender, EventArgs e)
        {
            SetTextBoxMeasureResult(gauge.MeasureResultText);
            var RecData = gauge.MeasureResultData;
            //MeasureCount = Convert.ToInt32(RecData[16]); ?
            InnerDiameter = Convert.ToDouble((RecData[4] << 24) + (RecData[5] << 16) + (RecData[6] << 8) + RecData[7]) / 10000.0;
            OuterDiameter = Convert.ToDouble((RecData[8] << 24) + (RecData[9] << 16) + (RecData[10] << 8) + RecData[11]) / 10000.0;
            string resultText = ExplainMeasureResult(InnerDiameter, OuterDiameter);
            AddResultData(Tuple.Create("InnerDiameter", InnerDiameter.ToString()),
                Tuple.Create("OuterDiameter", OuterDiameter.ToString()),
                Tuple.Create("explain", resultText));
        }

        private bool hasStateChanged()
        {
            if (Statetmp == State)
            {
                return false;
            }
            else
            {
                Statetmp = State;
                return true;
            }
        }

        void UpdateMesEquipmentState(TEquipment.EnumState state)
        {
            MainForm.Mesuser.UpdateEquipmentStatus(MainForm.Mesuser.EquipmentIDs[(int)WuRuanDataOperate.EQUIPNO.HDG_MEASURE], state);
        }

        void plc_ShangBiaoJian(object sender, EventArgs e)
        {
            //gauge.SendProofCmd(plc.LatheNum);
        }

        void plc_ShangGongJian(object sender, EventArgs e)
        {
            //gauge.SendMeasureCmd(plc.LatheNum);
        }

        void gauge_ApparatusExceptionalStateChanged(object sender, ApparatusExceptionalStateChangedEventArgs e)
        {
            labelApparatusExceptionalState.InvokeEx(c =>
            {
                if(c.Text != "测量仪异常：" + e.StateText)
                    c.Text = "测量仪异常：" + e.StateText;
            });
        }

        void gauge_ApparatusStateChanged(object sender, ApparatusStateChangedEventArgs e)
        {
            labelApparatusState.InvokeEx(c =>
            {
                if (c.Text != "测量仪：" + e.StateText)
                    c.Text = "测量仪：" + e.StateText;
            });
            machineState = e.StateText;

            State = e.State;
            if (hasStateChanged())
            {
                if (State == ApparatusStates.急停)
                {
                    UpdateMesEquipmentState(TEquipment.EnumState.故障);
                }
                else
                {
                    UpdateMesEquipmentState(TEquipment.EnumState.运行);
                }
            }

            /*switch (e.State)
            {
                case ApparatusStates.自检中:
                    break;
                case ApparatusStates.就绪:
                    MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_B, 15, 0, MainForm.plc_dbNo);
                    break;
                case ApparatusStates.急停:
                    MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_B, 15, 6, MainForm.plc_dbNo);
                    MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_B, 15, 7, MainForm.plc_dbNo);
                    break;
                case ApparatusStates.测量中:
                    break;
                case ApparatusStates.校对中:
                    break;
                case ApparatusStates.复位中:
                    break;
                case ApparatusStates.请求校对:
                    MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_B, 15, 2, MainForm.plc_dbNo);
                    break;
                case ApparatusStates.请求拿走标件:
                    MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_B, 15, 5, MainForm.plc_dbNo);
                    break;
                case ApparatusStates.请求拿走工件:
                    OnMeasureCompleted();
                    MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_B, 15, 4, MainForm.plc_dbNo);
                    break;
                case ApparatusStates.请求复位:
                    //请求复位
                    break;
                case ApparatusStates.待料位没有工件_测量停止:
                    break;
                default:
                    break;
            }*/

            if (e.State == ApparatusStates.就绪)
            {
                MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_B, 15, 0, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 2, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 4, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 5, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 6, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 7, MainForm.plc_dbNo);
            }
            else if (e.State == ApparatusStates.急停)
            {
                MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_B, 15, 6, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_B, 15, 7, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 0, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 2, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 4, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 5, MainForm.plc_dbNo);
            }
            else if (e.State == ApparatusStates.请求校对)
            {
                MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_B, 15, 2, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 0, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 4, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 5, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 6, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 7, MainForm.plc_dbNo);
            }
            else if(e.State == ApparatusStates.请求拿走标件)
            {
                MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_B, 15, 5, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 0, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 2, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 4, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 6, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 7, MainForm.plc_dbNo);
            }
            else if (e.State == ApparatusStates.请求拿走工件)
            {
                OnMeasureCompleted();
                MacDataService.GetInstance().HNC_RegSetBit((int)HncRegType.REG_TYPE_B, 15, 4, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 0, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 2, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 5, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 6, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 7, MainForm.plc_dbNo);
            }
            else
            {
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 0, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 2, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 4, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 5, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 6, MainForm.plc_dbNo);
                MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 7, MainForm.plc_dbNo);
            }
        }

        private async void btnMeasureStandard_Click(object sender, EventArgs e)
        {
            (sender as Button).Enabled = false;
            await gauge.SendProofCmd(plc.LatheNum);
            (sender as Button).Enabled = true;
        }

        private async void buttonMea_Click(object sender, EventArgs e)
        {
            (sender as Button).Enabled = false;
            await gauge.SendMeasureCmd(plc.LatheNum);
            (sender as Button).Enabled = true;
        }

        public string ExplainMeasureResult(double innerDiameter, double outerDiameter)
        {
            StringBuilder result = new StringBuilder();// "测量结果有误！";
            double m_neis = 0, m_neix = 0, m_neiSt = 0, m_wais = 0, m_waix = 0, m_waiSt = 0;
            double m_Cnei = innerDiameter, m_Cwai = outerDiameter;
            int m_NeiIsHe = 0, m_WaiIsHe = 0, m_WaiIsHe2 = 0;

            //TODO获取标准值
            m_neis = Convert.ToDouble((MainForm.str_size[3].Split(','))[0]);//内径上限
            m_neix = Convert.ToDouble((MainForm.str_size[3].Split(','))[1]);//内径下限
            m_neiSt = Convert.ToDouble((MainForm.str_size[3].Split(','))[2]);
            m_wais = Convert.ToDouble((MainForm.str_size[4].Split(','))[0]);//外径上限
            m_waix = Convert.ToDouble((MainForm.str_size[4].Split(','))[1]);//内径上限
            m_waiSt = Convert.ToDouble((MainForm.str_size[4].Split(','))[2]);

            if ((m_neis - m_Cnei <= (m_neis - m_neix) && m_neis - m_Cnei > 0) || m_neis - m_Cnei == 0) //小于或等于内径上限//
            {
                m_NeiIsHe = 1;
                //result.Append("内径合格\t");
            }
            else if (m_Cnei - m_neix < 0 && m_Cnei > 0)//内径超下差//
            {
                m_NeiIsHe = 2;
                result.Append("内径超下差\t");
            }
            else if (m_Cnei - m_neis > 0.0001 && m_Cnei > 0)//内径超上差//
            {
                m_NeiIsHe = 3;
                result.Append("内径超上差\t");
            }

            if ((m_wais - m_Cwai <= (m_wais - m_waix) && m_wais - m_Cwai > 0) || m_wais - m_Cwai == 0) //小于或等于外径上限
            {
                m_WaiIsHe = 1;
                //result.Append("外径合格\t");
            }
            else if (m_Cwai - m_waix < 0 && m_Cwai > 0)//外径超下差//
            {
                m_WaiIsHe = 2;
                result.Append("外径超下差\t");
            }
            else if (m_Cwai - m_wais > 0.0001 && m_Cwai > 0)//外径超上差//
            {
                m_WaiIsHe = 3;
                result.Append("外径超上差\t");
            }

            if ((m_NeiIsHe == 1) && (m_WaiIsHe == 1))
            {
                result.Append("全部合格");
                measureres = true;
            }
            else
            {
                measureres = false;
            }
            //((MainForm)(this.MdiParent)).m_MeaOK += 1;

            if ((m_NeiIsHe >= 1 && m_WaiIsHe >= 1 && m_WaiIsHe2 >= 1))
            {
                if (toolCompensationEnabled)
                {
                    if (Math.Abs(m_neiSt - innerDiameter) < valueError)
                    {
                        plc.SetToolValue((short)plc.LatheNum, 5, 1, m_neiSt - innerDiameter);  //内径补偿到刀具4
                    }
                    if (Math.Abs(m_waiSt - outerDiameter) < valueError)
                    {
                        plc.SetToolValue((short)plc.LatheNum, 2, 1, m_waiSt - outerDiameter);  //外径1补偿到刀具2
                    }
                }
            }

            if ((m_NeiIsHe > 1 || m_WaiIsHe > 1 || m_WaiIsHe2 > 1))//?why
            {
                //HncApi.HNC_RegSetBit((int)HncRegType.REG_TYPE_B, 6, 5, 2);//不合格
            }
            //20180504
            result.Clear();
            result.Append("零件合格");
            return result.ToString();
        }

        public static bool MeasureResult()
        {
            return measureres;
        }
    }

    public class Plc_HDG : PlcBase
    {
        /// <summary>
        /// 当工件来源编号改变时发生
        /// </summary>
        public event EventHandler<LatheNumChangedEventArgs> LatheNumChanged;

        /// <summary>
        /// 触发LatheNumChanged事件
        /// </summary>
        /// <param name="latheNum">工件来源编号</param>
        protected virtual void OnLatheNumChanged(int latheNum)
        {
            if (LatheNumChanged != null)
            {
                LatheNumChanged(this, new LatheNumChangedEventArgs(latheNum));
            }
        }

        private int _latheNum;
        /// <summary>
        /// 工件来源（0-2分别表示1-3号机床）
        /// </summary>
        public int LatheNum
        {
            get
            {
                return _latheNum;
            }
            private set
            {
                if (_latheNum != value)
                {
                    _latheNum = value;
                    OnLatheNumChanged(_latheNum);
                }
            }
        }


        /// <summary>
        /// 上工件完成事件
        /// </summary>
        public event EventHandler ShangGongJian;

        /// <summary>
        /// 触发ShangGongJian事件
        /// </summary>
        protected virtual void OnShangGongJian()
        {
            if (ShangGongJian != null)
            {
                ShangGongJian(this, new EventArgs());
            }
        }

        /// <summary>
        /// 上标件完成事件
        /// </summary>
        public event EventHandler ShangBiaoJian;

        /// <summary>
        /// 触发ShangGongJian事件
        /// </summary>
        protected virtual void OnShangBiaoJian()
        {
            if (ShangBiaoJian != null)
            {
                ShangBiaoJian(this, new EventArgs());
            }
        }
        /// <summary>
        /// 任务运行
        /// </summary>
        public override void Run()
        {
            /*int value = 0;
            while (!cancellationToken.IsCancellationRequested)
            {
                Thread.Sleep(1000);
                counter++;
                Collector.CollectShare.GetRegValue((int)HncRegType.REG_TYPE_B, 15, out value, MainForm.plc_dbNo);
                if (value < 0)
                {
                    continue;
                }
                if ((value & 0x02) == 0x02)
                {
                    MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 1, MainForm.plc_dbNo);
                    //MacDataService.GetInstance().HNC_RegSetValue((int)HncRegType.REG_TYPE_B, 15, 0, MainForm.plc_dbNo);
                    for (int i = 0; i < 3; i++)
                    {
                        OnShangGongJian();
                        Console.WriteLine("后端盖测量命令下达");
                        Thread.Sleep(TimeSpan.FromSeconds(0.5));
                    }
                    //Console.WriteLine("后端盖开始测量");
                    MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 1, MainForm.plc_dbNo);
                }
                else if ((value & 0x08) == 0x08)
                {
                    //MacDataService.GetInstance().HNC_RegClrBit((int)HncRegType.REG_TYPE_B, 15, 3, MainForm.plc_dbNo);//清除不了
                    MacDataService.GetInstance().HNC_RegSetValue((int)HncRegType.REG_TYPE_B, 15, 0, MainForm.plc_dbNo);
                    OnShangBiaoJian();
                    Thread.Sleep(2000);
                }
                if (counter > 10)
                {
                    Console.WriteLine("后端盖运行中value={0}", value);
                    counter = 0;
                }
            }*/
        }
    }

}
