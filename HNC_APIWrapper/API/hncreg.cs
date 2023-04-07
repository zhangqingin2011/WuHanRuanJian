using System;
using System.Collections.Generic;

namespace HNCAPI
{
    public enum HncRegType
    {
    	REG_TYPE_X = 0,// X寄存器 Bit8
    	REG_TYPE_Y,		// Y寄存器 Bit8
    	REG_TYPE_F,		// F寄存器 Bit16
    	REG_TYPE_G,		// G寄存器 Bit16
    	REG_TYPE_R,		// R寄存器 Bit8
    	REG_TYPE_W,		// W寄存器 Bit16
    	REG_TYPE_D,		// D寄存器 Bit32
    	REG_TYPE_B,		// B寄存器 Bit32
    	REG_TYPE_P,		// P寄存器 Bit32
    	REG_TYPE_TOTAL
    };

    public enum HncRegFGBaseType
    {
    	REG_FG_SYS_BASE = (Int32)HncRegType.REG_TYPE_TOTAL +  1,// 系统数据基址 {Get(Bit32)}
    	REG_FG_CHAN_BASE,		// 通道数据基址 {Get(Bit32)}
    	REG_FG_AXIS_BASE,		// 轴数据基址 {Get(Bit32)}
    	REG_FG_BASE_TYPE_TOTAL
    };

}
