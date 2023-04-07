﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScadaHncData
{

//   enum ToolParaIndex
//{
//    // 刀具几何相关参数索引
//    GTOOL_DIR = 0,	// 方向 
//    GTOOL_LEN1,	// 长度1(铣：刀具长度；车：X偏置)
//    GTOOL_LEN2,	// 长度2(车：Y偏置)
//    GTOOL_LEN3,	// 长度3(车：Z偏置)
//    GTOOL_LEN4,	// 长度4
//    GTOOL_LEN5,	// 长度5
//    GTOOL_RAD1,	// 半径1(铣：刀具半径；车：刀尖半径)
//    GTOOL_RAD2,	// 半径2
//    GTOOL_ANG1,	// 角度1
//    GTOOL_ANG2,	// 角度2

//    GTOOL_TOTAL,

//    // 刀具磨损相关参数索引
//    WTOOL_LEN1 = MAX_GEO_PARA, // (铣：长度磨损；车：Z磨损)
//    WTOOL_LEN2,	// 长度2
//    WTOOL_LEN3,	// 长度3
//    WTOOL_LEN4,	// 长度4
//    WTOOL_LEN5,	// 长度5
//    WTOOL_RAD1,	// 半径1(铣：半径磨损；车：X磨损)
//    WTOOL_RAD2,	// 半径2
//    WTOOL_ANG1,	// 角度1
//    WTOOL_ANG2,	// 角度2
	
//    WTOOL_TOTAL,
	
//    // 刀具工艺相关参数索引
//    TETOOL_PARA0 = MAX_GEO_PARA+MAX_WEAR_PARA, // 工艺相关参数0～参数MAX_TECH_PARA_NUM-1
//    TETOOL_PARA1,
//    TETOOL_PARA2,
//    TETOOL_PARA3,
//    TETOOL_PARA4,
//    TETOOL_PARA5,
//    TETOOL_PARA6,
//    TETOOL_PARA7,
//    TETOOL_PARA8,
//    TETOOL_PARA9,
//    // 暂用10个，今后再加
	
//    TETOOL_TOTAL,

//    // 刀具扩展参数--刀具管理参数，各刀具类型通用
//    EXTOOL_S_LIMIT = MAX_GEO_PARA+MAX_WEAR_PARA+MAX_TECH_PARA,    // S转速限制
//    EXTOOL_F_LIMIT,    // F转速限制
//    EXTOOL_LARGE_LEFT,		// 大刀具干涉左刀位
//    EXTOOL_LARGE_RIGHT,		// 大刀具干涩右刀位

//    EXTOOL_TOTAL,

//    // 刀具监控参数
//    MOTOOL_TYPE = MAX_GEO_PARA+MAX_WEAR_PARA+MAX_TECH_PARA+MAX_TOOL_EXPARA, // 刀具监控类型，按位有效，寿命/计件/磨损，可选多种监控方式同时监控
//    MOTOOL_SEQU,		// 	优先级
//    MOTOOL_MULTI,		// 	倍率

//    MOTOOL_MAX_LIFE,	// 最大寿命
//    MOTOOL_ALM_LIFE,	// 预警寿命
//    MOTOOL_ACT_LIFE,	// 实际寿命

//    MOTOOL_MAX_COUNT,	// 最大计件数
//    MOTOOL_ALM_COUNT,	// 预警计件数
//    MOTOOL_ACT_COUNT,	// 实际计件数

//    MOTOOL_MAX_WEAR,	// 最大磨损
//    MOTOOL_ALM_WEAR,	// 预警磨损
//    MOTOOL_ACT_WEAR,	// 实际磨损

//    MOTOOL_TOTAL,

//    // 刀具测量参数个数
//    METOOL_PARA0 = MAX_GEO_PARA+MAX_WEAR_PARA+MAX_TECH_PARA+MAX_TOOL_EXPARA+MAX_TOOL_MONITOR,
//    METOOL_PARA1,
//    METOOL_PARA2,
//    METOOL_PARA3,
//    METOOL_PARA4,
//    METOOL_PARA5,
//    METOOL_PARA6,
//    METOOL_PARA7,
//    METOOL_PARA8,
//    METOOL_PARA9,

//    METOOL_TOTAL,

//    // 	刀具一般信息
//    INFTOOL_ID = MAX_GEO_PARA+MAX_WEAR_PARA+MAX_TECH_PARA+MAX_TOOL_EXPARA+MAX_TOOL_MONITOR+MAX_TOOL_BASE, // 刀具索引号
//    INFTOOL_MAGZ,		// 	刀具所属刀库号
//    INFTOOL_CH,			// 	刀具所属通道号
//    INFTOOL_TYPE,		// 	刀具类型
//    INFTOOL_STATE,		// 	刀具状态字

//    INFTOOL_TOTAL,

//    TOOL_PARA_TOTAL // < MAX_TOOL_PARA
//};
    [Serializable]
    public class toolComp
    {
        public int TOOL_TYPE { get; set; }
        public double TOOL_POS{ get; set; }
        public double GTOOL_DIR{ get; set; }

        public double GTOOL_LEN1{ get; set; }
        public double GTOOL_LEN2{ get; set; }
        public double GTOOL_LEN3{ get; set; }
        public double GTOOL_LEN4{ get; set; }
        public double GTOOL_LEN5{ get; set; }
        public double GTOOL_RAD1{ get; set; }
        public double GTOOL_RAD2{ get; set; }
        public double GTOOL_ANG1{ get; set; }
        public double GTOOL_ANG2{ get; set; }        
        public double WTOOL_LEN1{ get; set; }
        public double WTOOL_LEN2{ get; set; }
        public double WTOOL_LEN3{ get; set; }
        public double WTOOL_LEN4{ get; set; }
        public double WTOOL_LEN5{ get; set; }
        
        public double WTOOL_RAD1{ get; set; }
        public double WTOOL_RAD2{ get; set; }
        public double WTOOL_ANG1{ get; set; }
        public double WTOOL_ANG2{ get; set; }
        
        public double TETOOL_PARA0{ get; set; }
        public double TETOOL_PARA1{ get; set; }
        public double TETOOL_PARA2{ get; set; }
        public double TETOOL_PARA3{ get; set; }
        public double TETOOL_PARA4{ get; set; }
        public double TETOOL_PARA5{ get; set; }
        public double TETOOL_PARA6{ get; set; }
        public double TETOOL_PARA7{ get; set; }
        public double TETOOL_PARA8{ get; set; }
        public double TETOOL_PARA9{ get; set; }
        public double EXTOOL_S_LIMIT{ get; set; }
        public double EXTOOL_F_LIMIT{ get; set; }
        
        public double EXTOOL_LARGE_LEFT{ get; set; }
        public double EXTOOL_LARGE_RIGHT{ get; set; }
        
        public double MOTOOL_TYPE{ get; set; }
        public double MOTOOL_SEQU{ get; set; }
        
        public double MOTOOL_MULTI{ get; set; }
        public double MOTOOL_MAX_LIFE{ get; set; }
        public double MOTOOL_ALM_LIFE{ get; set; }
        public double MOTOOL_ACT_LIFE{ get; set; }
        public double MOTOOL_MAX_COUNT{ get; set; }
        public double MOTOOL_ALM_COUNT{ get; set; }
        public double MOTOOL_ACT_COUNT{ get; set; }
        
        public double METOOL_PARA0{ get; set; }
        public double METOOL_PARA1{ get; set; }
        public double METOOL_PARA2{ get; set; }
        public double METOOL_PARA3{ get; set; }
        public double METOOL_PARA4{ get; set; }
        public double METOOL_PARA5{ get; set; }
        public double METOOL_PARA6{ get; set; }
        public double METOOL_PARA7{ get; set; }
        public double METOOL_PARA8{ get; set; }
        public double METOOL_PARA9{ get; set; }
        
        public double INFTOOL_ID{ get; set; }
        public double INFTOOL_MAGZ{ get; set; }
        public double INFTOOL_CH{ get; set; }
        public string INFTOOL_STATE{ get; set; }
        public double INFTOOL_G64MODE{ get; set; }
        public int MOTOOL_GROUP{ get; set; }
    }
}
