using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace ScadaHncData
{    
  // 生产派工单表
    [Serializable]
    public class MES_DISPATCH
    {
        public string ORDER_CODE { get; set; } // VARCHAR2(50)
        public string DISPATCH_CODE { get; set; } // VARCHAR2(50)       //派工单号
        public string MATERIAL_CODE { get; set; } // VARCHAR2(50)
        public int? QTY { get; set; }  // NUMBER
        public DateTime? PLAN_START_DATE { get; set; } // DATE
        public DateTime? PLAN_END_DATE { get; set; } // DATE
        public string NC_ID { get; set; } // VARCHAR2(50)
        public string NC_VER { get; set; } // VARCHAR2(50)
        public string OP_DOC { get; set; } // VARCHAR2(50)
        public string OP_DOC_VER { get; set; } // VARCHAR2(50)
        public string LINE { get; set; } // VARCHAR2(50)
        public string OP_CODE { get; set; } // VARCHAR2(50)
        public string EQUIP_CODE { get; set; } // VARCHAR2(50)
        public string EQUIP_GRP_CODE { get; set; } // VARCHAR2(50)
        public sbyte? FLAG { get; set; } // NUMBER (1,0)
        public decimal SN { get; set; } // NUMBER
        public DateTime? MARK_TIME { get; set; } // TIMESTAMP(6)
        public string MARK_DATE { get; set; } // VARCHAR2(10)
        public string NC_PATH { get; set; } // VARCHAR2(500)
        public string OP_DOC_PATH { get; set; } // VARCHAR2(500)
        public int PLAN_SENDED { get; set; }  ///-1忽略的工单，0无操作，  1 失败，  2成功，  2015.11.30 bool 为报工改
        public String Short_name { get; set; }///物料标识
    }

    // CNC设备控制接口数据
    [Serializable]
    public class MES_EQUIP_CMD
    {
        public string EQUIP_CODE { get; set; } // 设备ID VARCHAR2(50)
        public string WORK_CELL { get; set; } // 工位编码 VARCHAR2(50)
        public string LINE { get; set; } // 产线  VARCHAR2(50)
        public int? EQUIP_CMD { get; set; }  // 设备指令  NUMBER 1：开机，2：停机
        public sbyte? FLAG { get; set; } // NUMBER (1,0)
        public decimal SN { get; set; } // NUMBER
        public DateTime? MARK_TIME { get; set; } // TIMESTAMP(6)
    }


    //小料盘
    [Serializable]
    public class STRAY_M
    {
        public string STRAY_ID { get; set; } // VARCHAR2(50)
        public string STRAY_TYPE { get; set; } // VARCHAR2(50)
        public string MATERIAL_CODE { get; set; } // VARCHAR2(50)
        public string OP_CODE { get; set; } // VARCHAR2(50)
        public string EQUIP_GRP_CODE { get; set; } // VARCHAR2(50)
        public string LINE { get; set; } // VARCHAR2(50)
        public string PRODUCT_SN { get; set; } // VARCHAR2(50)
        public string SEQ_NO { get; set; } // VARCHAR2(50)
        public sbyte QUALITY_STATE { get; set; } // NUMBER (1,0)
        public string EQUIP_CODE { get; set; } // VARCHAR2(50)
        public sbyte IS_EMPTY { get; set; } // NUMBER (1,0)
        public sbyte IS_DONE { get; set; } // NUMBER (1,0)
        public sbyte FLAG { get; set; } // NUMBER (1,0)
        public decimal SN { get; set; } // NUMBER
        public DateTime MARK_TIME { get; set; } // TIMESTAMP(6)
        public string MARK_DATE { get; set; } // VARCHAR2(10)
        public int PLAN_SENDED { get; set; }  ///0无操作  1 失败  2成功  2015.11.30 bool 为报工改
    }



    ////组盘信息，将小料盘和大料盘关联，上报
    //[Serializable]
    //public class TRAY_GROUP
    //{
    //    public string BTRAY_ID { get; set; } // VARCHAR2(50)
    //    public string LINE { get; set; } // VARCHAR2(50)
    //    public decimal STRAY_QTY { get; set; } // NUMBER
    //    public string STRAY_ID { get; set; } // VARCHAR2(50)
    //    public string STRAY_TYPE { get; set; } // VARCHAR2(50)
    //    public sbyte IS_DONE { get; set; } // NUMBER (1,0)
    //    public sbyte FLAG { get; set; } // NUMBER (1,0)
    //    public decimal SN { get; set; } // NUMBER
    //    public DateTime MARK_TIME { get; set; } // TIMESTAMP(6)
    //    public string MARK_DATE { get; set; } // VARCHAR2(10)
    //}


    //质量检测参考参数，下发
    [Serializable]
    public class QUALTY_PARA
    {
        public string CHECK_CODE { get; set; } // VARCHAR2(50)
        public string PARA_CODE { get; set; } // VARCHAR2(50)
        public string PARA_NAME { get; set; } // VARCHAR2(50)
        public decimal PARA_VALUE { get; set; } // FLOAT(126)
        public DateTime CREATE_DATE { get; set; } // DATE
        public string CREATE_ID { get; set; } // VARCHAR2(32)
        public sbyte FLAG { get; set; } // NUMBER (1,0)
        public decimal SN { get; set; } // NUMBER
        public DateTime MARK_TIME { get; set; } // TIMESTAMP(6)
        public string MARK_DATE { get; set; } // VARCHAR2(10)
        public int PLAN_SENDED { get; set; }  ///0无操作  1 失败  2成功  2015.11.30 bool 为报工改
    }


    #region 质量检测结果，上传
    [Serializable]
    public class QUALITY_RESULT
    {
        public string EQUIP_CODE { get; set; } // VARCHAR2(50)
        public string ORDER_CODE { get; set; } // VARCHAR2(50)
        public string MATERIAL_CODE { get; set; } // VARCHAR2(50)
        public string OP_CODE { get; set; } // VARCHAR2(50)
        public string PRODUCT_SN { get; set; } // VARCHAR2(50)
        public sbyte IS_QUALIFIED { get; set; } // NUMBER (1,0)
        public DateTime CHECK_TIME { get; set; } // DATE
        public sbyte FLAG { get; set; } // NUMBER (1,0)
        public decimal SN { get; set; } // NUMBER
        public DateTime MARK_TIME { get; set; } // TIMESTAMP(6)
        public string MARK_DATE { get; set; } // VARCHAR2(10)
    }
    #endregion

    // 生产报工单，上报
    [Serializable]
    public class PROD_REPORT
    {
        public string EQUIP_CODE { get; set; } // VARCHAR2(50)
        public string ASSIGN_CODE { get; set; } // VARCHAR2(50)
        public string MATERIAL_CODE { get; set; } // VARCHAR2(50)
        public string OP_CODE { get; set; } // VARCHAR2(50)
        public string PRODUCT_SN { get; set; } // VARCHAR2(50)
        public int? QTY { get; set; } // NUMBER
        public DateTime START_TIME { get; set; } // DATE
        public DateTime END_TIME { get; set; } // DATE
        public sbyte FLAG { get; set; } // NUMBER (1,0)
        public decimal SN { get; set; } // NUMBER
        public DateTime MARK_TIME { get; set; } // TIMESTAMP(6)
        public string MARK_DATE { get; set; } // VARCHAR2(10)
    }

    /// <summary>
    /// 刀具信息，上传
    /// </summary>
    [Serializable]
    public class SYGOLE_TOOL_INFO
    {
        public string TOOL_CODE { get; set; } // VARCHAR2(50)
        public string TOOL_TYPE_CODE { get; set; } // VARCHAR2(50)
        public string TOOL_NAME { get; set; } // VARCHAR2(100)
        public string TOOL_TYPE { get; set; } // VARCHAR2(50)
        public string TOOL_SPEC { get; set; } // VARCHAR2(50)
        public sbyte? TOOL_STATE { get; set; } // NUMBER (1,0)
        public sbyte? FLAG { get; set; } // NUMBER (1,0)
        public decimal SN { get; set; } // NUMBER
        public string MARK_DATE { get; set; } // VARCHAR2(10)
        public DateTime? MARK_TIME { get; set; } // TIMESTAMP(3)
    }


}
