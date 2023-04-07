using System;


namespace SCADA
{
    /// <summary>
    /// 测量项模板
    /// </summary>
    public class MeterMode : Entity
    {

        //无,
        //叶轮,
        //校徽,
        //生肖鼠,
        //生肖牛,
        //生肖虎,
        //生肖兔,
        //生肖龙,
        //生肖蛇,
        //生肖马,
        //生肖羊,
        //生肖猴,
        //生肖鸡,
        //生肖狗,
        //生肖猪
        /// <summary>
        /// 模型名称，
        /// </summary>
        //[Display(Name = "模型编号")]
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

    }

}
