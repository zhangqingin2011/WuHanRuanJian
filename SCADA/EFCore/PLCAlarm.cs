using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
    public class PLCAlarm : Entity
    {
        /// <summary>
        /// PLC的ID
        /// </summary>
        public PLCID plcid { get; set; }
        /// <summary>
        /// 报警号
        /// </summary>
        public int AlarmNO { get; set; }
        /// <summary>
        /// 报警信息
        /// </summary>
        public string AlarmInfo { get; set; }
        /// <summary>
        /// 是否有报警
        /// </summary>
        public bool Occur { get; set; }
    }

    public enum PLCID
    {
        WMSPLC,
        ControlPLC
    }

    public enum WMSPLCALARM
    {
        堆垛机未上电,
        料仓急停,
        货叉未回零点,
        货叉运行超时,
        X轴变频器报警,
        Y轴变频器报警,
        Z轴变频器报警,
        超限位报警
    }

    public enum CONTROLPLCALARM
    {
        加工单元1机器人报警,
        加工单元1急停,
        //加工单元1车床不在原点,
        //加工单元1铣床不在原点,
        加工单元2机器人报警,
        加工单元2急停,
        //加工单元2车床不在原点,
        //加工单元2铣床不在原点,
        库卡机器人报警,
        kuka位托盘顶升气缸上升未到位,
        kuka位托盘顶升气缸下降未到位,
        总控急停按下,
        中转台夹紧未到位,
        中转台松开未到位,
        倍速链前端上升未到位,
        倍速链前端下降未到位,
        倍速链后端上升未到位,
        倍速链后端下降未到位,
        倍速链1号顶升气缸上升未到位,
        倍速链1号顶升气缸下降未到位,
        倍速链1号托盘气缸上升未到位,
        倍速链1号托盘气缸下降未到位,
        倍速链1号夹紧气缸夹紧未到位,
        倍速链1号夹紧气缸松开未到位,
        倍速链2号顶升气缸上升未到位,
        倍速链2号顶升气缸下降未到位,
        倍速链2号托盘气缸上升未到位,
        倍速链2号托盘气缸下降未到位,
        倍速链2号夹紧气缸夹紧未到位,
        倍速链2号夹紧气缸松开未到位,
        倍速链3号顶升气缸上升未到位,
        倍速链3号顶升气缸下降未到位,
        倍速链3号托盘气缸上升未到位,
        倍速链3号托盘气缸下降未到位,
        倍速链3号夹紧气缸夹紧未到位,
        倍速链3号夹紧气缸松开未到位,
        倍速链4号顶升气缸上升未到位,
        倍速链4号顶升气缸下降未到位,
        倍速链4号托盘气缸上升未到位,
        倍速链4号托盘气缸下降未到位,
        倍速链4号夹紧气缸夹紧未到位,
        倍速链4号夹紧气缸松开未到位
    }
}
