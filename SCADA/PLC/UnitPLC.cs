using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.SimensPLC
{
    public class UnitPLC : ModbusTCPClient
    {
        public enum REGINDEX
        {
            加工定位台左RFID状态 = 0,
            加工定位台左工件状态 = 1,
            加工定位台左任务状态 = 2,
            加工定位台左物料类型 = 4,
            加工定位台左当前任务完成情况 = 6,

            加工定位台右RFID状态 = 18,
            加工定位台右工件状态 = 19,
            加工定位台右任务状态 = 20,
            加工定位台右物料类型 = 22,
            加工定位台右当前任务完成情况 = 24,

            加工机器人J1实时坐标值 = 36,
            加工机器人J2实时坐标值 = 38,
            加工机器人J3实时坐标值 = 40,
            加工机器人J4实时坐标值 = 42,
            加工机器人J5实时坐标值 = 44,
            加工机器人J6实时坐标值 = 46,
            加工机器人E1实时坐标值 = 48,
            加工机器人状态 = 50,
            加工机器人HOME = 52,
            加工机器人模式 = 54,
            加工机器人运行状态 = 56,

            车床状态1 = 68,
            车床状态2 = 69,
            加工中心状态1 = 70,
            加工中心状态2 = 71,
            MES复位=82,
            复位完成反馈=84

        }
        public enum CNCSTATEBit1
        {
            Onlin=0,
            ClampFull,
            Home,
            RUN,
            Finish,
            Alarm,
            ClampOpen,
            ClampClose,
        }
        public enum CNCSTATEBit2
        {
            DoorOpen=0,
            Ready,
            ProgramNo1,
            ProgramNo2,
            ProgramNo3,
        }
        //public enum UNITBITINDEX
        //{
        //    RFID读请求,
        //    RFID读完成,
        //    RFID写请求,
        //    RFID写完成,
        //    执行工序请求,
        //    执行工序完成
        //}

        public enum RESETIN
        {
            复位成功复位完清除,
            车床复位启动,
            铣床复位启动
        }

        public enum RESETOUT
        {
            单元复位完成反馈,
            车床复位中,
            铣床复位中
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
