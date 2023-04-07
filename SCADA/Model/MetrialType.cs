using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Model
{
    public class MetrialTypeAndQuality
    {
        enum t{
           
        }
        public  enum Type
        {
            Nonee, //空位
            Front,//前端盖
            TBack, //后端盖
            Axles, //轴类
        }

        public enum Quality
        {
            Nonee, //无，当仓位上没有物料时
            Blank, //毛坯
            Worth, //合格
            Waste //不合格

        }
    }
}
