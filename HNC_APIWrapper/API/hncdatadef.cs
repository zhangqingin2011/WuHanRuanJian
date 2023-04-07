using System;
using System.Collections.Generic;

namespace HNCAPI
{
    public enum DevNcobjType
    {
    	DEV_NCOBJ_NULL_LOC = 1000,// 本地设备--非网络设备
    	DEV_NCOBJ_SPDL_LOC,
    	DEV_NCOBJ_AXIS_LOC,
    	DEV_NCOBJ_IN_LOC,
    	DEV_NCOBJ_OUT_LOC,
    	DEV_NCOBJ_AD_LOC,
    	DEV_NCOBJ_DA_LOC,
    	DEV_NCOBJ_IOMD_LOC,		// NCUC总线的IO集成模块
    	DEV_NCOBJ_MCP_LOC,
    	DEV_NCOBJ_MPG_LOC,
    	DEV_NCOBJ_NCKB_LOC,
    	DEV_NCOBJ_SENSOR_LOC,	// 传感器设备
    	DEV_NCOBJ_SERIAL_LOC,	// 串口设备
    	DEV_NCOBJ_GATHER_LOC,	// 温度采集卡
    
    	DEV_NCOBJ_NULL_NET = 2000,// 网络设备--ncuc\ethercat\syqnet...
    	DEV_NCOBJ_SPDL_NET,
    	DEV_NCOBJ_AXIS_NET,
    	DEV_NCOBJ_IN_NET,
    	DEV_NCOBJ_OUT_NET,
    	DEV_NCOBJ_AD_NET,
    	DEV_NCOBJ_DA_NET,
    	DEV_NCOBJ_IOMD_NET,		// NCUC总线的IO集成模块
    	DEV_NCOBJ_MCP_NET,
    	DEV_NCOBJ_MPG_NET,
    	DEV_NCOBJ_NCKB_NET,
    	DEV_NCOBJ_SENSOR_NET,	// 传感器
    	DEV_NCOBJ_PIDC_NET,		// 位控板
    
    	// 此处扩展新的类型
    	DEV_NCOBJ_ENCOD_NET		// 编码器
    };

    public enum ToolParaIndex
    {
    	// 刀具几何相关参数索引
    	GTOOL_DIR = 0,// 方向 
    	GTOOL_LEN1,	// 长度1(铣：刀具长度；车：X偏置)
    	GTOOL_LEN2,	// 长度2(车：Y偏置)
    	GTOOL_LEN3,	// 长度3(车：Z偏置)
    	GTOOL_LEN4,	// 长度4
    	GTOOL_LEN5,	// 长度5
    	GTOOL_RAD1,	// 半径1(铣：刀具半径；车：刀尖半径)
    	GTOOL_RAD2,	// 半径2
    	GTOOL_ANG1,	// 角度1
    	GTOOL_ANG2,	// 角度2
    
    	GTOOL_TOTAL,
    
    	// 刀具磨损相关参数索引
    	WTOOL_LEN1 = (Int32)HNCDATADEF.MAX_GEO_PARA,// (铣：长度磨损；车：Z磨损)
    	WTOOL_LEN2,	// 长度2
    	WTOOL_LEN3,	// 长度3
    	WTOOL_LEN4,	// 长度4
    	WTOOL_LEN5,	// 长度5
    	WTOOL_RAD1,	// 半径1(铣：半径磨损；车：X磨损)
    	WTOOL_RAD2,	// 半径2
    	WTOOL_ANG1,	// 角度1
    	WTOOL_ANG2,	// 角度2
    	
    	WTOOL_TOTAL,
    	
    	// 刀具工艺相关参数索引
    	TETOOL_PARA0 = (Int32)HNCDATADEF.MAX_GEO_PARA +  (Int32)HNCDATADEF.MAX_WEAR_PARA,// 工艺相关参数0～参数MAX_TECH_PARA_NUM-1
    	TETOOL_PARA1,
    	TETOOL_PARA2,
    	TETOOL_PARA3,
    	TETOOL_PARA4,
    	TETOOL_PARA5,
    	TETOOL_PARA6,
    	TETOOL_PARA7,
    	TETOOL_PARA8,
    	TETOOL_PARA9,
    	// 暂用10个，今后再加
    	
    	TETOOL_TOTAL,
    
    	// 刀具扩展参数--刀具管理参数，各刀具类型通用
    	EXTOOL_S_LIMIT = (Int32)HNCDATADEF.MAX_GEO_PARA +  (Int32)HNCDATADEF.MAX_WEAR_PARA +  (Int32)HNCDATADEF.MAX_TECH_PARA,// S转速限制
    	EXTOOL_F_LIMIT,    // F转速限制
    	EXTOOL_LARGE_LEFT,		// 大刀具干涉左刀位
    	EXTOOL_LARGE_RIGHT,		// 大刀具干涩右刀位
    
    	EXTOOL_TOTAL,
    
    	// 刀具监控参数
        MOTOOL_TYPE = (Int32)HNCDATADEF.MAX_GEO_PARA +  (Int32)HNCDATADEF.MAX_WEAR_PARA +  (Int32)HNCDATADEF.MAX_TECH_PARA +  (Int32)HNCDATADEF.MAX_TOOL_EXPARA,// 刀具监控类型，按位有效，寿命/计件/磨损，可选多种监控方式同时监控
    	MOTOOL_SEQU,		// 	优先级
    	MOTOOL_MULTI,		// 	倍率
    
    	MOTOOL_MAX_LIFE,	// 最大切削时间
    	MOTOOL_ALM_LIFE,	// 预警切削时间
    	MOTOOL_ACT_LIFE,	// 实际切削时间
    
    	MOTOOL_MAX_COUNT,	// 最大安装次数
    	MOTOOL_ALM_COUNT,	// 预警安装次数
    	MOTOOL_ACT_COUNT,	// 实际安装次数
    
    	MOTOOL_MAX_WEAR,	// 未使用
    	MOTOOL_ALM_WEAR,	// 未使用
    	MOTOOL_ACT_WEAR,	// 未使用
    
    	MOTOOL_GROUP,       // 刀具所属分组号
    
    	MOTOOL_TOTAL,
    
    	// 刀具测量参数个数
    	METOOL_PARA0 = (Int32)HNCDATADEF.MAX_GEO_PARA +  (Int32)HNCDATADEF.MAX_WEAR_PARA +  (Int32)HNCDATADEF.MAX_TECH_PARA +  (Int32)HNCDATADEF.MAX_TOOL_EXPARA +  (Int32)HNCDATADEF.MAX_TOOL_MONITOR,
    	METOOL_PARA1,
    	METOOL_PARA2,
    	METOOL_PARA3,
    	METOOL_PARA4,
    	METOOL_PARA5,
    	METOOL_PARA6,
    	METOOL_PARA7,
    	METOOL_PARA8,
    	METOOL_PARA9,
    
    	METOOL_TOTAL,
    
    	// 	刀具一般信息
    	INFTOOL_ID = (Int32)HNCDATADEF.MAX_GEO_PARA +  (Int32)HNCDATADEF.MAX_WEAR_PARA +  (Int32)HNCDATADEF.MAX_TECH_PARA +  (Int32)HNCDATADEF.MAX_TOOL_EXPARA +  (Int32)HNCDATADEF.MAX_TOOL_MONITOR +  (Int32)HNCDATADEF.MAX_TOOL_BASE,// 刀具索引号
    	INFTOOL_MAGZ,		// 	刀具所属刀库号
    	INFTOOL_CH,			// 	刀具所属通道号
    	INFTOOL_TYPE,		// 	刀具类型
    	INFTOOL_STATE,		// 	刀具状态字
    	INFTOOL_G64MODE,	//  刀具高速高精加工模式
    
    	INFTOOL_TOTAL,
    
    	TOOL_PARA_TOTAL // < MAX_TOOL_PARA
    };

    public enum MagzHeadIndex
    {
    	MAGZTAB_HEAD = 0,// 刀库表起始偏移地址（刀具号段+刀位属性段）
    	MAGZTAB_TOOL_NUM,	// 刀库表中刀具数
    	MAGZTAB_CUR_TOOL,	// 当前刀具号
    	MAGZTAB_CUR_POT,	// 当前刀位号
    	MAGZTAB_REF_TOOL,	// 标刀号
    	MAGZTAB_CHAN,		// 刀库所属通道号
    	MAGZTAB_TYPE,		// 刀库类型
    	SWAP_LEFT_TOOL,		// 机械手左刀位刀具号
    	SWAP_RIGHT_TOOL,	// 机械手右刀位刀具号
    	// 预留
    
    	MAGZTAB_TOTAL
    };

    public enum HncSampleType
    {
    	SAMPL_NULL_TYPE = 0,// 空类型
    	SAMPL_AXIS_CMD = 1,// 轴的指令位置
    	SAMPL_AXIS_ACT,		// 轴的实际位置
    	SAMPL_FOLLOW_ERR,	// 轴的跟随误差
    	SAMPL_CMD_INC,		// 轴的指令速度
    	SAMPL_ACT_VEL,		// 轴的实际速度
    	SAMPL_ACT_TRQ,		// 轴的负载电流
    	SAMPL_CMD_POS,		// 指令电机位置
    	SAMPL_CMD_PULSE,	// 指令脉冲位置
    	SAMPL_ACT_POS,		// 实际电机位置
    	SAMPL_ACT_PULSE,	// 实际脉冲位置
    	SAMPL_TOL_COMP,		// 补偿值
    	SAMPL_SYS_VAL = 101,// 系统变量
    	SAMPL_CHAN_VAL,		// 通道变量
    	SAMPL_AXIS_VAL,		// 轴变量
    	SAMPL_X_REG,		// X寄存器
    	SAMPL_Y_REG,		// Y寄存器
    	SAMPL_F_AXIS_REG,	// 轴F寄存器
    	SAMPL_G_AXIS_REG,	// 轴G寄存器
    	SAMPL_F_CHAN_REG,	// 通道F寄存器
    	SAMPL_G_CHAN_REG,	// 通道G寄存器
    	SAMPL_F_SYS_REG,	// 系统F寄存器
    	SAMPL_G_SYS_REG,	// 系统G寄存器
    	SAMPL_R_REG,		// R寄存器
    	SAMPL_B_REG,		// B寄存器
    	SAMPL_TOTAL
    };

    public class HNCDATADEF
    {
       public const Int32 SYS_CHAN_NUM =  4  ;//  系统最大通道数 
       public const Int32 SYS_AXES_NUM =  24  ;//  系统最大进给轴数 
       public const Int32 SYS_SPDL_NUM =  8  ;//  系统最大主轴数 
       public const Int32 SYS_NCBRD_NUM =  4  ;//  主控制设备数 
       public const Int32 SYS_NCOBJ_NUM =  80  ;//  从设备控制对象（部件）数 
       public const Int32 CHAN_AXES_NUM =  9  ;//  通道最大轴数 
       public const Int32 CHAN_SPDL_NUM =  4  ;//  通道最大主轴数 
       public const Int32 MAX_SMC_AXES_NUM =  16  ;//  最多运控轴数 
       public const Int32 TOTAL_AXES_NUM =  SYS_AXES_NUM +  SYS_SPDL_NUM  ;//  系统最大逻辑轴数 
       public const Int32 SYS_PART_NUM =  SYS_NCOBJ_NUM  ;//  系统设备接口数 
       public const Int32 NCU_PARAM_ID_BASE =  0 ;
       public const Int32 MAC_PARAM_ID_BASE =  10000 ;
       public const Int32 CHAN_PARAM_ID_BASE =  40000 ;
       public const Int32 AXIS_PARAM_ID_BASE =  100000 ;
       public const Int32 COMP_PARAM_ID_BASE =  300000 ;
       public const Int32 DEV_PARAM_ID_BASE =  500000 ;
       public const Int32 TABLE_PARAM_ID_BASE =  700000  ;//  数据表参数 
       public const Int32 NCU_PARAM_ID_NUM =  1000 ;
       public const Int32 MAC_PARAM_ID_NUM =  1000 ;
       public const Int32 CHAN_PARAM_ID_NUM =  1000 ;
       public const Int32 AXIS_PARAM_ID_NUM =  1000 ;
       public const Int32 COMP_PARAM_ID_NUM =  1000 ;
       public const Int32 DEV_PARAM_ID_NUM =  1000 ;
       public const Int32 TABLE_PARAM_ID_NUM =  100000  ;//  分配给数据表参数的ID数 
       public const Int32 PARAMAN_FILE_NCU =  0  ;//  NC参数 
       public const Int32 PARAMAN_FILE_MAC =  1  ;//  机床用户参数 
       public const Int32 PARAMAN_FILE_CHAN =  2  ;//  通道参数 
       public const Int32 PARAMAN_FILE_AXIS =  3  ;//  坐标轴参数 
       public const Int32 PARAMAN_FILE_ACMP =  4  ;//  误差补偿参数 
       public const Int32 PARAMAN_FILE_CFG =  5  ;//  设备接口参数 
       public const Int32 PARAMAN_FILE_TABLE =  6  ;//  数据表参数 
       public const Int32 PARAMAN_FILE_BOARD =  7  ;//  主站参数 
       public const Int32 PARAMAN_MAX_FILE_LIB =  7  ;//  参数结构文件最大分类数 
       public const Int32 PARAMAN_MAX_PARM_PER_LIB =  1000  ;//  各类参数最大条目数 
       public const Int32 PARAMAN_MAX_PARM_EXTEND =  1000  ;//  分支扩展参数最大条目数 
       public const Int32 PARAMAN_LIB_TITLE_SIZE =  16  ;//  分类名字符串最大长度 
       public const Int32 PARAMAN_REC_TITLE_SIZE =  16  ;//  子类名字符串最大长度 
       public const Int32 PARAMAN_ITEM_NAME_SIZE =  64  ;//  参数条目字符串最大长度 
       public const Int32 SERVO_PARM_START_IDX =  200  ;//  伺服参数起始参数号 
       public const Int32 SERVO_PARM_TOTAL_NUM =  100;//伺服参数个数 
       public const Int32 AX_ENCODER_MASK =  0x00FF ;
       public const Int32 AX_NC_CMD_MASK =  0x00F0 ;
       public const Int32 AX_NC_TRACK_ERR =  0x0100 ;
       public const Int32 AX_NC_CYC64_MODE =  0x1000 ;
       public const Int32 MAX_GEO_PARA =  24  ;//  刀具几何参数个数 
       public const Int32 MAX_WEAR_PARA =  24  ;//  刀具磨损参数个数 
       public const Int32 MAX_TECH_PARA =  24  ;//  刀具工艺相关参数个数 
       public const Int32 MAX_TOOL_EXPARA =  24  ;//  刀具扩展参数个数 
       public const Int32 MAX_TOOL_MONITOR =  24  ;//  刀具监控参数个数 
       public const Int32 MAX_TOOL_MEASURE =  24  ;//  刀具测量参数个数 
       public const Int32 MAX_TOOL_BASE =  24  ;//  刀具一般信息参数个数 
       public const Int32 MAX_TOOL_PARA =  200  ;//  刀具基本参数个数  (24 +  24 +  24 +  24 +  24 +  24 +  24  =  168) 
       public const Int32 MAGZ_HEAD_SIZE =  16  ;//  刀库数据表头大小 
       public const Int32 MTOOL_RAD =  (Int32)ToolParaIndex.GTOOL_RAD1  ;//  刀具半径 
       public const Int32 MTOOL_LEN =  (Int32)ToolParaIndex.GTOOL_LEN1  ;//  刀具长度 
       public const Int32 MTOOL_RAD_WEAR =  (Int32)ToolParaIndex.WTOOL_RAD1  ;//  铣刀:半径磨损补偿（径向） 
       public const Int32 MTOOL_LEN_WEAR =  (Int32)ToolParaIndex.WTOOL_LEN1  ;//  铣刀:长度磨损补偿（轴向） 
       public const Int32 LTOOL_RAD =  (Int32)ToolParaIndex.GTOOL_RAD1  ;//  刀尖半径 
       public const Int32 LTOOL_DIR =  (Int32)ToolParaIndex.GTOOL_DIR  ;//  刀尖方向 
       public const Int32 LTOOL_RAD_WEAR =  (Int32)ToolParaIndex.WTOOL_RAD1  ;//  车刀:刀具磨损值（径向）（相对值） 
       public const Int32 LTOOL_LEN_WEAR =  (Int32)ToolParaIndex.WTOOL_LEN1  ;//  车刀:刀具磨损值（轴向）（相对值） 
       public const Int32 LTOOL_XOFF =  (Int32)ToolParaIndex.GTOOL_LEN1  ;//  车刀：刀具偏置值（径向）（绝对值）  =  试切时X值  -  试切直径/2 
       public const Int32 LTOOL_YOFF =  (Int32)ToolParaIndex.GTOOL_LEN2 ;
       public const Int32 LTOOL_ZOFF =  (Int32)ToolParaIndex.GTOOL_LEN3  ;//  车刀：刀具偏置值（轴向）（绝对值）  =  试切时Z值  -  试切长度 
       public const Int32 LTOOL_XDONE =  (Int32)ToolParaIndex.TETOOL_PARA0  ;//  X试切标志 
       public const Int32 LTOOL_YDONE =  (Int32)ToolParaIndex.TETOOL_PARA0 ;
       public const Int32 LTOOL_ZDONE =  (Int32)ToolParaIndex.TETOOL_PARA1  ;//  Z试切标志 
       public const Int32 SPDL_RESOLUTION =  1000  ;//  主轴转速分辨率 
       public const Int32 MAX_SCAN_ROW_NUM_RANDOM =  1000 ;
       public const Int32 GIVEN_ROW_IDLE =  0 ;
       public const Int32 GIVEN_ROW_WAIT_PROG_OK =  1  ;//等待任意行程序准备好 
       public const Int32 GIVEN_ROW_SCANING =  2  ;//任意行扫描中 
       public const Int32 GIVEN_ROW_WAIT_SCAN_ACK =  3  ;//任意行等待界面给出应答后再向后继续扫描 
       public const Int32 GIVEN_ROW_WAIT_SUBPROG_OK =  4  ;//等待任意行子程序准备好 
       public const Int32 GIVEN_ROW_SUBPROG_OK =  5  ;//任意行子程序准备好 
       public const Int32 GIVEN_ROW_SUBPROG_EXCUTING =  6  ;//任意行子程序执行中 
       public const Int32 GIVEN_ROW_SCAN_ERR =  7  ;//任意行扫描中发现语法错误 
       public const Int32 GIVEN_ROW_TYPE1_WAIT_Z_MOVE =  8  ;//任意行扫描模式1，等待Z轴移动指令 
       public const Int32 SMPL_CHAN_NUM =  16  ;//  采样通道数 
       public const Int32 SMPL_DATA_NUM =  10000  ;//  每采样通道的采样点数 
       public const Int32 T_CTRL_CHANGE_DIRECT =  0X1 ;
       public const Int32 T_CTRL_TOOL_MODE =  0X2 ;
       public const Int32 VAR_WRITE_WAIT =  0X1 ;
       public const Int32 VAR_READ_WAIT =  0X2 ;
    }
}
