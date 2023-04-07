using System;
using System.Collections.Generic;

namespace HNCAPI
{
    public enum SysConfigType
    {
    	HNC_SYS_CFG_BIN_PATH = 0,
    	HNC_SYS_CFG_BMP_PATH,
    	HNC_SYS_CFG_DATA_PATH,
    	HNC_SYS_CFG_HELP_PATH,
    	HNC_SYS_CFG_PARM_PATH,
    	HNC_SYS_CFG_PLC_PATH,
    	HNC_SYS_CFG_PROG_PATH,
    	HNC_SYS_CFG_TEMP_PATH,
    
    	HNC_SYS_CFG_PATH_TOTAL,
    
    	HNC_SYS_CFG_PARM_FILE,
    	HNC_SYS_CFG_PLC_FILE,
    
    	HNC_SYS_CFG_LOG_PATH,
    
    	HNC_SYS_CONFIG_TOTAL
    };

    public enum InitType
    {
    	INIT_READCFG = 0,//	读系统配置文件
    	INIT_KERNEL,		//	内核初始化
    	INIT_PARM,			//	参数初始化
    	INIT_FILECHECK,
    	INIT_PASS,			//	密码初始化
    	INIT_NET,			//	网络初始化
    	INIT_PROGMAN,		//	程序管理初始化(必须要在参数初始化之后，因为要根据通道数来初始化)
    	INIT_MDI,			//	MDI初始化
    	INIT_PLC,			//	PLC初始化
    	INIT_ALARM,			//	报警初始化
    	INIT_FILELOAD,		//	数据文件导入
    	INIT_BUS,			//	总线初始化
    };

    public class HNCSYSCTRL
    {
       public const Int32 MSFTCOUNT =  10 ;
       public const Int32 IJKRCOUNT =  4 ;
    }
}
