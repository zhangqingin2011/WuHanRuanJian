using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace SCADA.Model
{
    public class Agv_Task:TBaseTable
    {
        /// <summary>
        /// 数据ID，
        /// </summary>
        public string ID { get; set; }

        //public int CMD { get; set; }

        /// <summary>
        /// 动作库位 1-96
        /// </summary>
        public string P1 { get; set; }

        /// <summary>
        /// 动作，100上料，101成品（废品）下料，102出库，103入库
        /// </summary>
        public string P2 { get; set; }

        /// <summary>
        /// 物料信息，包括类型，质量，uid
        /// </summary>
        public string P3 { get; set; }

        /// <summary>
        /// 优先级，1，2，3，4
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 堆垛机执行状态，初始化只能为0
        /// </summary>
        public int Status { get; set; }

       

    }

   
}
