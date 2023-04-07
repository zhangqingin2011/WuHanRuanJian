using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HNC_MacDataService;
using HNCAPI;

namespace SCADA
{
    /// <summary>
    /// 测量仪状态
    /// </summary>
    public enum ApparatusStates { 自检中 = 0, 就绪 = 1, 急停 = 2, 测量中 = 3, 校对中 = 4, 复位中 = 5, 请求校对 = 6, 请求拿走标件 = 7, 请求拿走工件 = 8, 请求复位 = 9, 待料位没有工件_测量停止 = 10 }

    public class ApparatusStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 测量仪状态
        /// </summary>
        public ApparatusStates State { get; private set; }

        /// <summary>
        /// 状态说明
        /// </summary>
        public string StateText { get; private set; }

        public ApparatusStateChangedEventArgs(ApparatusStates apparatusState)
        {
            State = apparatusState;
            StateText = Enum.GetName(typeof(ApparatusStates), apparatusState);
        }
    }

    /// <summary>
    /// 测量仪异常状态
    /// </summary>
    public enum ApparatusExceptionalStates { 测量仪正常 = -1, 规格异常 = 0, 翻转异常 = 1, 气爪异常 = 2, 定位异常 = 3, 升降异常 = 4, 气源压力异常 = 5, 测量工位有工件 = 6, 急停按钮按下 = 7 }

    public class ApparatusExceptionalStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 测量仪异常状态
        /// </summary>
        public ApparatusExceptionalStates State { get; private set; }

        /// <summary>
        /// 状态说明
        /// </summary>
        public string StateText { get; set; }

        public ApparatusExceptionalStateChangedEventArgs(ApparatusExceptionalStates apparatusExceptionalState)
        {
            State = apparatusExceptionalState;
            StateText = Enum.GetName(typeof(ApparatusExceptionalStates), apparatusExceptionalState);
        }
    }

    /// <summary>
    /// 中原量仪
    /// </summary>
    public sealed class ZYLY : SerialPortCommBase
    {
        /// <summary>
        /// 当测量仪状态改变时发生
        /// </summary>
        public event EventHandler<ApparatusStateChangedEventArgs> ApparatusStateChanged;

        /// <summary>
        /// 触发ApparatusStateChanged事件
        /// </summary>
        /// <param name="state"></param>
        private void OnApparatusStateChanged(ApparatusStates state)
        {
            if (ApparatusStateChanged != null)
            {
                ApparatusStateChanged(this, new ApparatusStateChangedEventArgs(state));
            }
        }

        private ApparatusStates _apparatusState;
        /// <summary>
        /// 测量仪状态
        /// </summary>
        public ApparatusStates ApparatusState
        {
            get
            {
                return _apparatusState;
            }
            private set
            {
                if (_apparatusState != value || _apparatusState == ApparatusStates.就绪)
                {
                    _apparatusState = value;
                    OnApparatusStateChanged(_apparatusState);
                }
            }
        }

        /// <summary>
        /// 当测量仪异常状态改变时发生
        /// </summary>
        public event EventHandler<ApparatusExceptionalStateChangedEventArgs> ApparatusExceptionalStateChanged;

        /// <summary>
        /// 触发ApparatusExceptionalStateChanged事件
        /// </summary>
        /// <param name="state"></param>
        private void OnApparatusExceptionalStateChanged(ApparatusExceptionalStates state)
        {
            if (ApparatusExceptionalStateChanged != null)
            {
                ApparatusExceptionalStateChanged(this, new ApparatusExceptionalStateChangedEventArgs(state));
            }
        }

        private ApparatusExceptionalStates _apparatusExceptionalState;
        /// <summary>
        /// 测量仪异常状态
        /// </summary>
        public ApparatusExceptionalStates ApparatusExceptionalState
        {
            get
            {
                return _apparatusExceptionalState;
            }
            private set
            {
                if (_apparatusExceptionalState != value)
                {
                    _apparatusExceptionalState = value;
                    OnApparatusExceptionalStateChanged(_apparatusExceptionalState);
                }
            }
        }

        private ushort[] RecData = new ushort[32];
        /// <summary>
        /// 测量结果数据
        /// </summary>
        public ushort[] MeasureResultData { get { return RecData; } }
        /// <summary>
        /// 测量结果数据
        /// </summary>
        public string MeasureResultText
        {
            get
            {
                if (RecData != null && RecData.Length > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    for (int i = 0; i < RecData.Length; i++)
                    {
                        sb.Append(RecData[i] + " ");
                    }
                    return sb.ToString().Trim('\0');
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        private CancellationTokenSource ctsModBus;
        private Task taskModBus;

        /// <summary>
        /// 是否正在运行
        /// </summary>
        public bool IsRunning { get; private set; }

        private byte address;

        /// <summary>
        /// 初始化 中原量仪（ZYLY） 类的新实例
        /// </summary>
        public ZYLY(byte address = 0x02, string portName = "COM1", int baudRate = 9600, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
            : base(portName, baudRate, parity, dataBits, stopBits)
        {
            this.address = address;
        }

        /// <summary>
        /// 启动测量仪交互
        /// </summary>
        public void Start()
        {
            if (!IsRunning)
            {
                ctsModBus = new CancellationTokenSource();
                taskModBus = new Task(o => Run(), ctsModBus, TaskCreationOptions.LongRunning);
                taskModBus.Start();
                IsRunning = true;
            }
        }

        /// <summary>
        /// 停止测量仪交互
        /// </summary>
        public void Stop()
        {
            if (ctsModBus != null && !ctsModBus.IsCancellationRequested)
            {
                ctsModBus.Cancel();
                while (!taskModBus.IsCompleted) ;
                IsRunning = false;
            }
        }

        /// <summary>
        /// 测量仪运行
        /// </summary>
        private void Run()
        {
            string result = string.Empty;
            while (!ctsModBus.IsCancellationRequested)
            {
                Thread.Sleep(500);
                SendFc03(address, 0x32, 0x0A, ref RecData, ref result);//取值
                if (address == 0x02)
                {
                    Console.WriteLine("前端盖测量仪状态: {0}", RecData[2]);
                }
                else
                {
                    Console.WriteLine("后端盖测量仪状态: {0}", RecData[2]);
                }
                if (RecData[2] >= 0 && RecData[2] <= 10)
                {
                    ApparatusState = (ApparatusStates)RecData[2];
                }
                //Console.WriteLine(DateTime.Now.ToString() + " " + (address == 0x02 ? "前端盖测量仪" : "后端盖测量仪") + "正在运行");
            }
            //Console.WriteLine((address == 0x02 ? "前端盖测量仪" : "后端盖测量仪") + "结束运行");
        }

        /// <summary>
        /// 发送测量指令
        /// </summary>
        /// <param name="latheSerialNumber">机床编号</param>
        public async Task SendMeasureCmd(int latheSerialNumber)
        {
            await Task.Run(() => { SendCmd(latheSerialNumber, ApparatusCmdType.测量); });
        }

        public bool SendMeasureCommond(int latheSerialNumber)
        {
            bool res = SendCmd(latheSerialNumber, ApparatusCmdType.测量);
            return res;
        }

        /// <summary>
        /// 发送校对指令
        /// </summary>
        /// <param name="latheSerialNumber">机床编号</param>
        public async Task SendProofCmd(int latheSerialNumber)
        {
            await Task.Run(() => { SendCmd(latheSerialNumber, ApparatusCmdType.校对); });
        }

        /// <summary>
        /// 测量仪命令类型
        /// 复位命令待确认！
        /// </summary>
        private enum ApparatusCmdType { 测量 = 1, 校对 = 2, 复位 = 3 }

        /// <summary>
        /// 向测量仪发送命令
        /// </summary>
        /// <param name="latheSerialNumber">机床编号</param>
        /// <param name="cmdType">命令类型</param>
        private bool SendCmd(int latheSerialNumber, ApparatusCmdType cmdType = ApparatusCmdType.测量)
        {
            bool res = false;
            if (RecData[2] == 1) // 测量仪在就绪状态才能发测量指令
            {
                ushort[] DeleData = new ushort[2];
                if (RecData != null && RecData.Length > 1 && RecData[0] >= 1 && RecData[0] <= 3)
                {
                    DeleData[0] = (ushort)((RecData[0] << 8) + (int)cmdType);
                }
                else
                {
                    DeleData[0] = (ushort)cmdType;
                }
                /*待确认*/
                if (cmdType == ApparatusCmdType.复位)
                {
                    DeleData[0] = 0x0003;
                }
                /**/
                if (latheSerialNumber == 0)
                {
                    DeleData[1] = 0x0000;
                }
                else if (latheSerialNumber == 1)
                {
                    DeleData[1] = 0x0200;
                }
                else if (latheSerialNumber == 2)
                {
                    DeleData[1] = 0x0200;
                }
                else
                {
                    DeleData[1] = 0x0000;
                }
                string result = string.Empty;
                res = SendFc10(address, 0x4B, 0x01, DeleData, ref result);
                if (!res)
                    Console.WriteLine("测量指令error：{0}", result);
            }
            return res;
        }
    }
}
