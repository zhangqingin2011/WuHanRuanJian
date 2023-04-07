using System;
using System.Collections.Generic;

namespace HNCAPI
{
    public enum ParaPropType
    {
    	PARA_PROP_VALUE,	// 参数值 参数结构文件定义
    	PARA_PROP_MAXVALUE,	// 最大值 参数结构文件定义
    	PARA_PROP_MINVALUE,	// 最小值 参数结构文件定义
    	PARA_PROP_DFVALUE,	// 缺省值 参数结构文件定义
    	PARA_PROP_NAME,		// 名字  STRING
    	PARA_PROP_ACCESS,	// 权限  INT
    	PARA_PROP_ACT,		// 生效方式 INT
    	PARA_PROP_STORE,	// 存储类型  INT
    	PARA_PROP_ID,		// 参数编号 INT
    	PARA_PROP_SIZE		// 大小 INT
    };

    public enum ParaActType
    {
    	PARA_ACT_SAVE, // 保存生效
    	PARA_ACT_NOW,  // 立即生效
    	PARA_ACT_RST,  // 复位生效
    	PARA_ACT_PWR,  // 重启生效
    	PARA_ACT_HIDE  // 隐藏未启用
    };

    public enum ParaSubClassProp
    {
    	SUBCLASS_NAME,		// 子类名
    	SUBCLASS_ROWNUM,	// 子类行数
    	SUBCLASS_NUM,		// 子类数
    	SUBCLASS_MAXNUM     //最大行数
    };

    public class HNCPARAMAN
    {
       public const Int32 DATA_STRING_LEN =  64 ;
       public const Int32 PARA_FILE_OPEN_FAIL =  -1  ;//  文件打开失败 
       public const Int32 PARA_FILE_SEEK_FAIL =  -2  ;//  文件seek失败 
       public const Int32 PARA_FILE_READ_FAIL =  -3  ;//  文件读失败 
       public const Int32 PARA_FILE_WRITE_FAIL =  -4  ;//  文件写失败 
       public const Int32 PARA_FILE_DATA_FAIL =  -5  ;//  文件数据错误 
       public const Int32 PARA_POINT_NULL_ERR =  -6  ;//  空指针 
       public const Int32 PARA_FILENO_ERR =  -7  ;//  参数类别错误 
       public const Int32 PARA_SUBNO_ERR =  -8  ;//  子类号越界 
       public const Int32 PARA_ROWNO_ERR =  -9  ;//  行号越界 
       public const Int32 PARA_ROWXNO_ERR =  -10  ;//  总行号越界 
       public const Int32 PARA_INDEX_ERR =  -11  ;//  索引越界 
       public const Int32 PARA_STRING_LIMIT =  -12  ;//  字符串太长 
       public const Int32 PARA_PARMNO_ERR =  -13  ;//  参数号越界 
       public const Int32 PARA_PARMANTYPE_ERR =  -14  ;//  参数类型错误 
       public const Int32 PARM_MAPINDEX_ERR =  -15  ;//  参数索引号错 
       public const Int32 PARM_MAPROWNO_ERR =  -16  ;//  参数行号错 
       public const Int32 PARM_SVWRITE_ERR =  -17  ;//  写伺服参数失败 
       public const Int32 PARM_SVSAVE_ERR =  -18  ;//  保存伺服参数失败 
       public const Int32 PAEM_SVSTATE_ERR =  -19  ;//  总线通讯未准备好,无法写入伺服参数 
       public const Int32 PAEM_SVRESET_ERR =  -20  ;//  总线未成功复位 
    }
}
