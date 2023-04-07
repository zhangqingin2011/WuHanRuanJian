using System;
using System.Collections.Generic;

namespace HNCAPI
{
    public class HNCDATATYPE
    {
       public const Int32 PARAM_STR_LEN =  8 ;
       public const Int32 DTYPE_NULL =  0  ;//  空类型 
       public const Int32 DTYPE_INTEGER =  1  ;//  整型 
       public const Int32 DTYPE_INT =  DTYPE_INTEGER ;
       public const Int32 DTYPE_FLOAT =  2  ;//  实型 
       public const Int32 DTYPE_EXPR =  3  ;//  表达式 
       public const Int32 DTYPE_VAR =  4  ;//  简单变量 
       public const Int32 DTYPE_STRING =  5  ;//  字符串 
       public const Int32 DTYPE_UINT =  6  ;//  无符号整形 
       public const Int32 DTYPE_BOOL =  7  ;//  布尔型 
       public const Int32 DTYPE_FUNC =  8  ;//  函数表达式 
       public const Int32 DTYPE_ARR =  9  ;//  数组表达式 
       public const Int32 DTYPE_HEX4 =  10  ;//  十六进制格式 
       public const Int32 DTYPE_BYTE =  11  ;//  十六进制格式 
       public const Int32 DTYPE_DOT =  DTYPE_FLOAT ;
       public const double PI =  3.14159265358979312 ;
       public const double EE =  2.71828182845924523 ;
    }
}
