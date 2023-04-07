using System;

namespace SCADA
{
    //测量结果与订单是一对多关系，一个订单有多个工件，并且每一个工件有自己的检测模板编号，
    //因此工件与检测结果匹配需要匹配订单编号、工件编号才能获取检测结果，检测结果需要需工件的模板编号匹配，金鼎结果判断
    public class MeterResult : Entity
    {  /// <summary>
       /// 订单ID号，不是订单详情ID号
       /// </summary>
        //[Display(Name = "")]
        public Guid OrderId { get; set; }

        /// <summary>
        /// 工件编号，1-4，一个订单一个料盘，一个料盘支持4个工件
        /// </summary>
        public int WorkpieceNumber { get; set; }
        /// <summary>
        /// 测量模板编号
        /// </summary>
        //[Display(Name = "测量模板编号")]
        //[JsonIgnore]
        //[Required]
        public string ModeSN { get; set; }

        /// <summary>
        /// 测量总数
        /// </summary>
        //[Display(Name = "总数")]
        public int NUM { get; set; }

        /// <summary>
        /// 测量序号
        /// </summary>
        //[Display(Name = "序号")]
        public int Rank { get; set; }
        /// <summary>
        /// 单位
        /// </summary>
        //[Display(Name = "单位")]
        public string SizeDataUnit { get; set; }

        /// <summary>
        /// 最小值
        /// </summary>
        //[Display(Name = "最小值")]
        public double SizeDataMin { get; set; }

        /// <summary>
        /// 最大值
        /// </summary>
        //[Display(Name = "最大值")]
        public double SizeDataMax { get; set; }

        /// <summary>
        /// 标准值
        /// </summary>
        //[Display(Name = "标准值")]
        public double SizeDataStandard { get; set; }

        /// <summary>
        /// 测量尺寸
        /// </summary>
        public double SizeValue { get; set; }

        /// <summary>
        /// 此项检验合格true，不合格fasle
        /// </summary>
        public bool IsCertificate { get; set; }

    }
}
