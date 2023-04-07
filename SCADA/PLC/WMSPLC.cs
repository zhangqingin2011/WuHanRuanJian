using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.SimensPLC
{
    public class WMSPLC : ModbusTCPClient
    {
        public enum DATAINDEX
        {
            BinState1=0,
            BinState2,
            BinState3,
            BinState4,
            BinState5,
            RackState = 6,//0未就绪，1空闲，2运行中，3故障中
            RackErrCode = 7,//立库错误码0正常 1Z轴驱动器通讯超时 4X轴故障 5Y轴故障 6Z轴故障 13超宽报警 14急停
            RackFunCode =8,//立库流程码
            TaskStateBit = 9,//任务状态
            AGVTask = 10,//AGV当前任务,1#-9#对接台编号
            AGVelectricity = 11,//AGV当前电量,百分比0-100
            AGVErrCode = 12,//AGV故障代码
            AGVLandMark =13,//AGV当前地标号码
            AGVArriveed =14,//AGV到达状态
            AGVSpeed =15,//AGV当前速度
            XPosition = 16,//X轴位置
            XSpeed =17,//X轴速度
            YPosition = 18,//Y轴位置
            YSpeed = 19,//Y轴速度
            ZPosition = 20,//Z轴位置
            ZSpeed = 21,//Z轴速度

            WMS_On = 26,//启动标志(1入库，2出库，3中转)
            WMSInBinNo = 27,//入库库位号
            WMSOutBinNo = 28,//出库库位号
            WMSInStation =29,//入库台编号=1倍速链入库 =2中转台入库
            WMSOutStation =30,//出库台编号=1倍速链出库 =2中转台出库
            SYSHeartBeat = 31,//系统心跳，0-999变化
            SYSTaskBit = 32,//系统下发位，主控下发位
            AGVTaskCMD = 33,//AGV下发任务，1#-9#对接台编号
            AGVStateCMD =34,//AGV启停控制，1 前进启动 2后退启动 3停止
            AGVRollCMD = 35,//AGV辊筒控制  1正转 2反转
        }

        public enum TaskStateBit1
        {
            AGVRollIning,//AGV辊筒进料中
            AGVRollOuting,//AGV辊筒出料中
            StationRollIning,//中转台辊筒进料中
            StationRollOuting,//中转台辊筒出料中
            RackINChainInGoodsCheck,//入库倍速链入口物料检测
            RackINChainOutGoodsCheck,//入库倍速链出口物料检测
            RackOutChainInGoodsCheck,//出库倍速链入口物料检测
            RackOutChainOutGoodsCheck,//出库倍速链出口物料检测
            RackIning,//入库中
            RackOuting,//出库中
            RackMoveing,//移库中
            AGVGoodsCheck,//AGV物料检测
            RackInFinish,//入库完成
            RackOutFinish,//出库完成
            RackMoveFinish,//移库完成
            StationGoodsCheck,//中转台物料检测
        }
       
        public enum SYSTaskBit
        {
            RackSuspend = 4,//立体库暂停
            RackStop = 5,//立体库停止
        }
        public enum DATAMEANS
        {
            仓位信息1,
            仓位信息2,
            仓位信息3,
            仓位信息4,
            仓位信息5,
            立体库状态,
            立体库错误码,
            立体库流程字,
            立体库状态位1,
            立体库状态位2,
            AGV当前任务,
            AGV当前电量,
            AGV故障代码,
            AGV当前地标号码,
            AGV到达状态,
            AGV当前速度,
            X轴位置,
            X轴速度,
            Y轴位置,
            Y轴速度,
            Z轴位置,
            Z轴速度,
            启动标志,
            入库库位号,
            出库库位号,
            入库台编号,
            出库台编号,
            系统心跳,
            系统下发位,
            AGV下发任务,
            AGV启停控制,
            AGV辊筒控制,
          
        }
        public enum ALARMINDEX
        {

        }
        public enum StationINDEX
        {
            UnitCNC1Station1,
            UnitCNC1Station2,
            UnitCNC2Station1,
            UnitCNC2Station2,
            UnitCNC3Station1,
            UnitCNC3Station2,
            UnitCNC4Station1,
            UnitCNC4Station2,
            UnitCleanStation1,
            UnitCleanStation2,
            UnitCheck1Station1,
            UnitCheck1Station2,
            UnitCheck2Station1,
            UnitCheck2Station2,
            UnitFit1Station1,
            UnitFit1Station2,
            UnitFit2Station1,
            UnitFit2Station2,
            UnitRackStation,
        }

        private object writebitlocker = new object();
        public bool GetRegIndexBit(int rindex, int bindex, out bool bvalue)
        {
            int value;
            bvalue = false;
            bool res = ReadsingleRegister(rindex, out value);
            if (res && value >= 0)
            {
                if (bindex >= 0 && bindex < 8)
                    bvalue = GetBoolValue(value, bindex + 8);
                else if (bindex >= 8 && bindex < 16)
                    bvalue = GetBoolValue(value, bindex - 8);
            }
            return res;
        }

        public bool SetRegIndexBit(int rindex, int bindex, bool bvalue)
        {
            bool res;
            int value;
            lock (writebitlocker)
            {
                res = ReadsingleRegister(rindex, out value);
                if (res && value >= 0)
                {
                    if (bvalue)
                    {
                        if (bindex >= 0 && bindex < 8)
                            value = SetBoolValue(value, bindex + 8);
                        else if (bindex >= 8 && bindex < 16)
                            value = SetBoolValue(value, bindex - 8);
                    }
                    else
                    {
                        if (bindex >= 0 && bindex < 8)
                            value = ClrBoolValue(value, bindex + 8);
                        else if (bindex >= 8 && bindex < 16)
                            value = ClrBoolValue(value, bindex - 8);
                    }
                    res = WritesingleRegister(rindex, value);
                }
            }
            return res;
        }
    }
}
