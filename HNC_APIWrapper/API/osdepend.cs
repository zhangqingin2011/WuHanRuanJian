using System;
using System.Collections.Generic;

namespace HNCAPI
{
    public enum FIND_TYPE {FIND_OVERFLOW = -2, FIND_ERR = -1, FIND_OK = 0};
    public class OSDEPEND
    {
       public const Int32 FILE_DATE_LEN =  18  ;//  文件日期字符串长度 
       public const Int32 FILE_SIZE_LEN =  12  ;//  文件大小字符串长度 
       public const Int32 FILE_NAME_LEN =  48  ;//  文件名（不含路径）长度 
       public const Int32 PATH_NAME_LEN =  128  ;//  路径名（含文件名）长度 
    }
}
