using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
    public class KeyValueData : Entity
    {
        /// <summary>
        /// 索引
        /// </summary>
        public int Key { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }

    public enum INDEX
    {
        wMSPositionBind,
        aGVBind,
        process1Position1Bind,
        process1Position2Bind,
        process2Position1Bind,
        process2Position2Bind,
        process3Position1Bind,
        process3Position2Bind,
        process4Position1Bind,
        process4Position2Bind,
        cleanPosition1Bind,
        cleanPosition2Bind,
        check1Position1Bind,
        check1Position2Bind,
        check2Position1Bind,
        check2Position2Bind,
        fit1Position1Bind,
        fit1Position2Bind,
        fit2Position1Bind,
        fit2Position2Bind,
    }
}
