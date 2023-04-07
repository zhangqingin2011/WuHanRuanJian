using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
    public class RackUnitData : Entity
    {
        /// <summary>
        /// 仓位号
        /// </summary>
        public int NO { get; set; }
        /// <summary>
        /// 料盘类型
        /// </summary>
        public TRAYTYPE Tray { get; set; }
        /// <summary>
        /// RFID编号
        /// </summary>
        public string RfidID { get; set; }
        /// <summary>
        /// 料盘容量
        /// </summary>
        public int TrayVolume { get; set; }
        /// <summary>
        /// 工件1
        /// </summary>
        public PIECETYTPE Piece1 { get; set; }
        /// <summary>
        /// 工件1质量
        /// </summary>
        public PIECEQUALITY Piece1Quality { get; set; }
        /// <summary>
        /// 工件2
        /// </summary>
        public PIECETYTPE Piece2 { get; set; }
        /// <summary>
        /// 工件2质量
        /// </summary>
        public PIECEQUALITY Piece2Quality { get; set; }
        /// <summary>
        /// 工件3
        /// </summary>
        public PIECETYTPE Piece3 { get; set; }
        /// <summary>
        /// 工件3质量
        /// </summary>
        public PIECEQUALITY Piece3Quality { get; set; }
        /// <summary>
        /// 工件4
        /// </summary>
        public PIECETYTPE Piece4 { get; set; }
        /// <summary>
        /// 工件4质量
        /// </summary>
        public PIECEQUALITY Piece4Quality { get; set; }
        /// <summary>
        /// 仓位锁定
        /// </summary>
        public bool Lock { get; set; }
        /// <summary>
        /// 定位台编号
        /// </summary>
        public int StationNo { get; set; }
    }

    public enum TRAYTYPE
    {
        空,
        料盘A,//
        料盘B,
    }

    public enum PIECETYTPE
    {
        无,
        毛坯A,
        毛坯B,
        工件A,
        工件B,
    }

    public enum PIECEQUALITY
    {
        待检测,
        合格,
        不合格,
        异常
    }
}
