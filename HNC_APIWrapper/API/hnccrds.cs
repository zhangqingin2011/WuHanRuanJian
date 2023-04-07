using System;
using System.Collections.Generic;

namespace HNCAPI
{
    public enum HncCRDS
    {
    	HNC_CRDS_CH_G5X_ZERO = 0,// 指定通道轴坐标系零点
    	HNC_CRDS_CH_G5X_ID,       // 指定通道轴坐标系ID
    	HNC_CRDS_CH_WCS_ZERO,     // 指定通道轴工件坐标系零点
    	HNC_CRDS_CH_REL_ZERO,     // 指定通道轴相对坐标系零点
    	HNC_CRDS_CH_FRAME_ZERO,   // 指定通道轴基架坐标系零点
    	HNC_CRDS_G68_PNT1,        // 特性坐标系辅助点1坐标
    	HNC_CRDS_G68_PNT2,        // 特性坐标系辅助点2坐标
    	HNC_CRDS_G68_ZERO,        // 特性坐标系零点
    	HNC_CRDS_G68_VCT,         // 特性坐标系向量 fBit64
    	HNC_CRDS_CH_G5X_OFFSET_ZERO,  //指定通道轴坐标系偏置零点
    	HNC_CRDS_TOTAL
    };

    public enum HncCrdsMaxNum
    {
    	G5EXT_MAX_NUM,		//	G54.X扩展坐标系的数目
    	CHG_WCS_MAX_NUM,	//	进给保持时的临时工件坐标系数目
    	TCS_MAX_NUM			//	TCS坐标系数目
    };

}
