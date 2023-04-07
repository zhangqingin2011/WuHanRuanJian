using System;
using System.Collections.Generic;

namespace HNCAPI
{
    public enum HncVarType
    {
    	VAR_TYPE_AXIS = 0,// 轴变量 {Get(Bit32), Set(Bit32)}
    	VAR_TYPE_CHANNEL,	// 通道变量 {Get(Bit32), Set(Bit32)}
    	VAR_TYPE_SYSTEM,	// 系统变量 {Get(Bit32), Set(Bit32)}
    	VAR_TYPE_SYSTEM_F,	// 浮点类型的系统变量 {Get(fBit64), Set(fBit64)}
    	VAR_TYPE_TOTAL
    };

}
