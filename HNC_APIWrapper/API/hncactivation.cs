using System;
using System.Collections.Generic;

namespace HNCAPI
{
    public enum ACTIVAT_TYPE
    {
    	ACT_NO_REG = 0,// 系统未注册
    	ACT_OVER_DATE,			// 系统注册超期
    	ACT_OK,					// 系统在注册时间范围内
    	ACT_FOREVER				// 永久注册
    };

}
