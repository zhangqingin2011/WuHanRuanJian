using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
    public class WorkOrderData : Entity
    {
        public int OrderNO { get; set; }
        public TRAYTYPE Tray { get; set; }
        /// <summary>
        /// 料盘ID编号
        /// </summary>
        public string RfidID { get; set; }
        /// <summary>
        /// 出库仓位
        /// </summary>
        public int OUTNO { get; set; }
        public PRODUCTTYPE Product1 { get; set; }
        public PIECEQUALITY P1Quality { get; set; }
        public PRODUCTTYPE Product2 { get; set; }
        public PIECEQUALITY P2Quality { get; set; }
        public PRODUCTTYPE Product3 { get; set; }
        public PIECEQUALITY P3Quality { get; set; }
        public PRODUCTTYPE Product4 { get; set; }
        public PIECEQUALITY P4Quality { get; set; }
        public PROCESSPOSITION ProcessPosition { get; set; }

        public CLEANPOSITION CleanPosition { get; set; }
        public CHECKPOSITION CheckPosition { get; set; }
        public FITPOSITION FitPosition { get; set; }
        /// <summary>
        /// 入库仓位
        /// </summary>
        public int INNO { get; set; }
        public ORDERSTATE OrderState { get; set; }
        public string Updatetime { get; set; }
    }

    public enum PRODUCTTYPE
    {
        A料,
        B料,
        无
    }

    public enum PROCESSPOSITION
    {
        暂无,
        加工单元1定位台1,
        加工单元1定位台2,
        加工单元2定位台1,
        加工单元2定位台2,
        加工单元3定位台1,
        加工单元3定位台2,
        加工单元4定位台1,
        加工单元4定位台2,
    }

    public enum CHECKPOSITION
    {
        暂无,
        测量1单元定位台1,
        测量1单元定位台2,
        测量2单元定位台1,
        测量2单元定位台2
    }
    public enum CLEANPOSITION
    {
        暂无,
        清洗单元定位台1,
        清洗单元定位台2,
    }
    public enum FITPOSITION
    {
        暂无,
        装配单元1定位台1,
        装配单元1定位台2,
        装配单元2定位台1,
        装配单元2定位台2,
    }
    public enum ORDERSTATE
    {
        等待,
        结束,
        自动出库中,
        自动出库完成,
        AGV到出入库位置取料运输中,
        AGV到出入库位置取料运输完成,
        AGV到出入库位置取料中,
        AGV到出入库位置取料完成,
        AGV到加工单元放料运输中,
        AGV到加工单元放料运输完成,
        AGV到加工单元定位台放料中,
        AGV到加工单元定位台放料完成,
        机床加工中,
        机床加工完成,
        AGV到加工单元取料运输中,
        AGV到加工单元取料运输完成,
        AGV到加工单元定位台取料中,
        AGV到加工单元定位台取料完成,
        AGV到清洗单元放料运输中,
        AGV到清洗单元放料运输完成,
        AGV到清洗单元定位台放料中,
        AGV到清洗单元定位台放料完成,
        工件清洗中,
        工件清洗完成,
        AGV到清洗单元取料运输中,
        AGV到清洗单元取料运输完成,
        AGV到清洗单元定位台取料中,
        AGV到清洗单元定位台取料完成,
        AGV到检测单元定位台放料中,
        AGV到检测单元定位台放料完成,
        工件检测中,
        工件检测完成,
        AGV到检测单元定位台取料中,
        AGV到检测单元定位台取料完成,
        AGV到装配单元放料运输中,
        AGV到装配单元放料运输完成,
        AGV到装配单元定位台放料中,
        AGV到装配单元定位台放料完成,
        工件装配中,
        工件装配完成,
        AGV到装配单元取料运输中,
        AGV到装配单元取料运输完成,
        AGV到装配单元定位台取料中,
        AGV到装配单元定位台取料完成,
        AGV到出入库入库运输中,
        AGV到出入库元入库运输完成,
        AGV到出入库位置放料中,
        AGV到出入库位置放料完成,       
        自动入库中,
        自动入库完成
    }
}
