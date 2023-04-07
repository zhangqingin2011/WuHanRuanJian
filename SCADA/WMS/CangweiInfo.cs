using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
    public class CangweiInfo
    {
        public int number { get; private set; }  //仓位号
        public int typeIndex;  //工件类型索引   0:前端盖 1：后端盖 2：轴类
        public int status;  //工件状态索引      0:未加工 1：合格 2：不合格
        public bool materialflag;  //有料标志

        public CangweiInfo(int number)
        {
            this.number = number;
            typeIndex = 0;
            status = 0;
            materialflag = false;
        }
    }

    public class CangweiInfoChangedEventArgs : EventArgs
    {
        /// <summary>
        /// 仓位信息
        /// </summary>
        public CangweiInfo Info { get; private set; }

        public CangweiInfoChangedEventArgs(CangweiInfo GetInfo)
        {
            Info = GetInfo;
        }
    }
}
