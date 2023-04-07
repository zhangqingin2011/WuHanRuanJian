using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Model
{
    public class StockStatus:TBaseTable
    {
        /// <summary>
        /// 仓位位置
        /// </summary>

        public string  PositionID { get; set; }

        /// <summary>
        /// 物料类型
        /// </summary>
        public string MetrialType { get; set; }

        /// <summary>
        /// 质量
        /// </summary>
        public string MetrialQuality { get; set; }

        /// <summary>
        /// 物料编码
        /// </summary>
        public string MetrialCode { get; set; }

        

       
    }
}
