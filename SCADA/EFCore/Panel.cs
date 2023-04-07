using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
    class Panel : Entity
    {
       
        /// <summary>
        /// 料盘类型
        /// </summary>
        public TRAYTYPE Tray { get; set; }
        /// <summary>
        /// RFID编号
        /// </summary>
        public string RfidID { get; set; }
    }
}
