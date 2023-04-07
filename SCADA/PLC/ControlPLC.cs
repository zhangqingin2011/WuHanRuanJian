using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.SimensPLC
{
    public class ControlPLC : ModbusTCPClient
    {
        public enum RFIDReg
        {
            RFIDReadReq = 0,
            RFIDReadFinish,
            REIDWriteReq,
            RFIDWriteFinish,
            PosStationState,
        }
        public enum ProductReg
        {
            Pos1Passed = 0,
            Pos2Passed,
            Pos3Passed,
            Pos4Passed,
            Pos1Empty,
            Pos2Empty,
            Pos3Empty,
            Pos4Empty,
        }
        public enum TaskReg
        {
            SendOutReq = 0,
            LoadPermit,
            LoadFinish,
            AGVArrived,
            AGVReceive,
        }

        public enum ALARMINDEX
        {
            急停报警 = 0,
            清洗定位台左上下料报警,
            清洗定位台右上下料报警,
            装配1定位台左上下料报警,
            装配1定位台右上下料报警,
            装配2定位台左上下料报警,
            装配2定位台右上下料报警,
            检测1定位台左上下料报警,
            检测1定位台右上下料报警,
            检测2定位台左上下料报警,
            检测2定位台右上下料报警,
            清洗机器人报警,
            检测1机器人报警,
            检测2机器人报警,
            装配1机器人报警,
            装配2机器人报警,
            总控报警,
            单元一报警,
            单元二报警,
            单元三报警,
            单元四报警,
        }

        public enum REGINDEX
        {
            清洗定位台左RFID状态 = 0,
            清洗定位台左工件状态 = 1,
            清洗定位台左任务状态 = 2,
            清洗定位台左物料类型 = 4,
            清洗定位台左当前任务完成情况 = 6,

            清洗定位台右RFID状态 = 18,
            清洗定位台右工件状态 = 19,
            清洗定位台右任务状态 = 20,
            清洗定位台右物料类型 = 22,
            清洗定位台右当前任务完成情况 = 24,

            检测1定位台左RFID状态 = 36,
            检测1定位台左工件状态 = 37,
            检测1定位台左任务状态 = 38,
            检测1定位台左物料类型 = 40,
            检测1定位台左当前任务完成情况 = 42,

            检测1定位台右RFID状态 = 54,
            检测1定位台右工件状态 = 55,
            检测1定位台右任务状态 = 56,
            检测1定位台右物料类型 = 58,
            检测1定位台右当前任务完成情况 = 60,

            检测2定位台左RFID状态 = 72,
            检测2定位台左工件状态 = 73,
            检测2定位台左任务状态 = 74,
            检测2定位台左物料类型 = 76,
            检测2定位台左当前任务完成情况 = 78,

            检测2定位台右RFID状态 = 90,
            检测2定位台右工件状态 = 91,
            检测2定位台右任务状态 = 92,
            检测2定位台右物料类型 = 94,
            检测2定位台右当前任务完成情况 = 96,

            装配1定位台左RFID状态 = 108,
            装配1定位台左工件状态 = 109,
            装配1定位台左任务状态 = 110,
            装配1定位台左物料类型 = 112,
            装配1定位台左当前任务完成情况 = 114,

            装配1定位台右RFID状态 = 126,
            装配1定位台右工件状态 = 127,
            装配1定位台右任务状态 = 128,
            装配1定位台右物料类型 = 130,
            装配1定位台右当前任务完成情况 = 132,

            装配2定位台左RFID状态 = 144,
            装配2定位台左工件状态 = 145,
            装配2定位台左任务状态 = 146,
            装配2定位台左物料类型 = 148,
            装配2定位台左当前任务完成情况 = 150,

            装配2定位台右RFID状态 = 162,
            装配2定位台右工件状态 = 163,
            装配2定位台右任务状态 = 164,
            装配2定位台右物料类型 = 166,
            装配2定位台右当前任务完成情况 = 168,

            清洗机器人1J1实时坐标值 = 180,
            清洗机器人1J2实时坐标值 = 182,
            清洗机器人1J3实时坐标值 = 184,
            清洗机器人1J4实时坐标值 = 186,
            清洗机器人1J5实时坐标值 = 188,
            清洗机器人1J6实时坐标值 = 190,
            清洗机器人1E1实时坐标值 = 192,
            清洗机器人1状态 = 194,
            清洗机器人1HOME = 196,
            清洗机器人1模式 = 198,
            清洗机器人1运行状态 = 200,



            检测机器人1J12实时坐标值 = 212,
            检测机器人1J2实时坐标值 = 214,
            检测机器人1J3实时坐标值 = 216,
            检测机器人1J4实时坐标值 = 218,
            检测机器人1J5实时坐标值 = 220,
            检测机器人1J6实时坐标值 = 222,
            检测机器人1E1实时坐标值 = 224,
            检测机器人1状态 = 226,
            检测机器人1HOME = 228,
            检测机器人1模式 = 230,
            检测机器人1运行状态 = 232,

            检测机器人2J1实时坐标值 = 244,
            检测机器人2J2实时坐标值 = 246,
            检测机器人2J3实时坐标值 = 248,
            检测机器人2J4实时坐标值 = 250,
            检测机器人2J5实时坐标值 = 252,
            检测机器人2J6实时坐标值 = 254,
            检测机器人2E1实时坐标值 = 256,
            检测机器人2状态 = 258,
            检测机器人2HOME = 260,
            检测机器人2模式 = 262,
            检测机器人2运行状态 = 264,

            装配机器人1J1实时坐标值 = 276,
            装配机器人1J2实时坐标值 = 278,
            装配机器人1J3实时坐标值 = 280,
            装配机器人1J4实时坐标值 = 282,
            装配机器人1J5实时坐标值 = 284,
            装配机器人1J6实时坐标值 = 286,
            装配机器人1E1实时坐标值 = 288,
            装配测机器人1状态 = 290,
            装配机器人1HOME = 292,
            装配机器人1模式 = 294,
            装配机器人1运行状态 = 296,

            装配机器人2J1实时坐标值 = 308,
            装配机器人2J2实时坐标值 = 310,
            装配机器人2J3实时坐标值 = 312,
            装配机器人2J4实时坐标值 = 314,
            装配机器人2J5实时坐标值 = 316,
            装配机器人2J6实时坐标值 = 318,
            装配机器人2E1实时坐标值 = 320,
            装配测机器人2状态 = 322,
            装配机器人2HOME = 324,
            装配机器人2模式 = 326,
            装配机器人2运行状态 = 328,

            定位台报警信息1 = 340,
            定位台报警信息2 = 341,
            机器人报警信息 = 342,
            总控报警 = 344,
            单元1报警 = 346,
            单元2报警 = 348,
            单元3报警 = 350,
            单元4报警 = 352,
            复位完成反馈 = 360,
            MES复位 = 376

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
