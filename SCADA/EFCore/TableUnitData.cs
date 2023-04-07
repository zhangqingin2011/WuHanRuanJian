using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
  public   class TableUnitData : Entity
    {
        /// <summary>
        /// 定位台编号
        /// </summary>
        public int NO { get; set; }
        /// <summary>
        ///定位台位置
        /// </summary>
        public int PNO { get; set; }
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
        /// 工件1状态
        /// </summary>
        public PIECESTATE Piece1State { get; set; }
        /// <summary>
        /// 工件2
        /// </summary>
        public PIECETYTPE Piece2 { get; set; }
        /// <summary>
        /// 工件2状态
        /// </summary>
        public PIECESTATE Piece2State { get; set; }
        /// <summary>
        /// 工件3
        /// </summary>
        public PIECETYTPE Piece3 { get; set; }
        /// <summary>
        /// 工件3状态
        /// </summary>
        public PIECESTATE Piece3State { get; set; }
        /// <summary>
        /// 工件4
        /// </summary>
        public PIECETYTPE Piece4 { get; set; }
        /// <summary>
        /// 工件4状态
        /// </summary>
        public PIECESTATE Piece4State{ get; set; }
        /// <summary>
        /// 仓位锁定
        /// </summary>
        public bool Lock { get; set; }
    }
 public enum PIECESTATE
    {
        待加工,
        加工中,
        加工完成,
        异常
    }
}

