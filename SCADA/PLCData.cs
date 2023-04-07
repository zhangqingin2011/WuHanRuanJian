using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
    class PosStation
    {
        /// <summary>
        /// 请求读取RFID
        /// </summary>
        bool RFIDReadReq;
        /// <summary>
        /// 完成读取RFID
        /// </summary>
        bool RFIDReadFinish;
        /// <summary>
        /// 请求写入RFID
        /// </summary>
        bool REIDWriteReq;
        /// <summary>
        /// 完成写入RFID
        /// </summary>
        bool RFIDWriteFinish;
        /// <summary>
        /// 定位台状态
        /// </summary>
        bool PosStationState;
        /// <summary>
        /// 1号位检测合格，0不合格，1合格
        /// </summary>
        bool Pos1Passed;
        /// <summary>
        /// 2号位检测合格，0不合格，1合格
        /// </summary>
        bool Pos2Passed;
        /// <summary>
        /// 3号位检测合格，0不合格，1合格
        /// </summary>
        bool Pos3Passed;
        /// <summary>
        /// 4号位检测合格，0不合格，1合格
        /// </summary>
        bool Pos4Passed;
        /// <summary>
        /// 1号位有料，0物料，1有料
        /// </summary>
        bool Pos1Empty;
        /// <summary>
        /// 2号位有料，0物料，1有料
        /// </summary>
        bool Pos2Empty;
        /// <summary>
        /// 3号位有料，0物料，1有料
        /// </summary>
        bool Pos3Empty;
        /// <summary>
        /// 4号位有料，0物料，1有料
        /// </summary>
        bool Pos4Empty;
        /// <summary>
        /// 请求出料
        /// </summary>
        bool SendOutReq;
        /// <summary>
        /// 允许上料
        /// </summary>
        bool LoadPermit;
        /// <summary>
        /// 上料完成
        /// </summary>
        bool LoadFinish;
        /// <summary>
        /// AGV到达定位台
        /// </summary>
        bool AGVArrived;
        /// <summary>
        /// AGV收料完成
        /// </summary>
        bool AGVReceive;
        /// <summary>
        /// 物料类型
        /// </summary>
        Int32 ProductType;
        /// <summary>
        /// 当前任务
        /// </summary>
        Int32 TaskState;
        /// <summary>
        /// 寄存器起始地址
        /// </summary>
        Int32 RegIndex;

        public PosStation()
        {
            RFIDReadReq = false;
            RFIDReadFinish = false;
            REIDWriteReq = false;
            RFIDWriteFinish = false;
            PosStationState = false;
            Pos1Passed = false;
            Pos2Passed = false;
            Pos3Passed = false;
            Pos4Passed = false;
            Pos1Empty = false;
            Pos2Empty = false;
            Pos3Empty = false;
            Pos4Empty = false;
            SendOutReq = false;
            LoadPermit = false;
            LoadFinish = false;
            AGVArrived = false;
            AGVReceive = false;
            ProductType = 0;
            TaskState = 0;
            RegIndex = 0;
        }

    }

    class RobotData
    {
        /// <summary>
        /// J1轴实时坐标值
        /// </summary>
        Int32 J1Site;
        /// <summary>
        /// J2轴实时坐标值
        /// </summary>
        Int32 J2Site;
        /// <summary>
        /// J3轴实时坐标值
        /// </summary>
        Int32 J3Site;
        /// <summary>
        /// J1轴实时坐标值
        /// </summary>
        Int32 J4Site;
        /// <summary>
        /// J4轴实时坐标值
        /// </summary>
        Int32 J5Site;
        /// <summary>
        /// J6轴实时坐标值
        /// </summary>
        Int32 J6Site;
        /// <summary>
        /// E1轴实时坐标值
        /// </summary>
        Int32 E1Site;
        /// <summary>
        /// 机器人报警，1报警
        /// </summary>
        Int32 AlarmState;
        /// <summary>
        /// 机器人home位，1原点
        /// </summary>
        Int32 Home;
        /// <summary>
        ///机器人模式 ，1手动，2自动，3外部
        /// </summary>
        Int32 Mode;
        /// <summary>
        /// 机器人运行状态，0空闲，1忙
        /// </summary>
        Int32 Busy;
        /// <summary>
        /// 寄存器起始地址
        /// </summary>
        Int32 RegIndex;
        public RobotData()
        {
            J1Site = 0;
            J2Site = 0;
            J3Site = 0;
            J4Site = 0;
            J5Site = 0;
            J6Site = 0;
            E1Site = 0;
            AlarmState = 0;
            Home = 0;
            Mode = 0;
            Busy = 0;
            RegIndex = 0;
        }
    }
   public class CNCData
    {
        bool Onlin;
        bool ClampFull;
        bool Home;
        bool RUN;
        bool Finish;
        bool Alarm;
        bool ClampOpen;
        bool ClampClose;
        bool DoorOpen;
        bool Ready;
        bool ProgramNo1;
        bool ProgramNo2;
        bool ProgramNo3;
        Int32 RegIndex;
        public CNCData()
        {
            Onlin = false;
            ClampFull = false;
            Home = false;
            RUN = false;
            Finish = false;
            Alarm = false;
            ClampOpen = false;
            ClampClose = false;
            DoorOpen = false;
            Ready = false;
            ProgramNo1 = false;
            ProgramNo2 = false;
            ProgramNo3 = false;
            Int32 RegIndex = 0;
        }
    }

}
