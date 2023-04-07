using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.SimensPLC
{
    public class ExtendPLC : ModbusTCPClient
    {
        public enum REGINDEX
        { 
            拓展AGV调度命令 = 2,
            拓展AGV命令反馈
        }

        public enum AGVPOS
        {
            去拓展加工单元一 = 1,
            去拓展加工单元二,
            去拓展中转台
        }
    }
}
