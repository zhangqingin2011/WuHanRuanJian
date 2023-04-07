﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScadaHncData
{
    [Serializable]
    public class ChannelData : BaseNC
    {
        public Int32 chNo;
        public Int32 macType;
        public Int32 axisMask;
        public String chName;
        public Double cmdFeedrate;
        public Double actFeedrate;
        public Int32 feedOverride;
        public Int32 spindleOverride;
        public Int32 rapidOverride;
        public Int32 workMode;
        public Int32 isCycling;
        public Int32 isMdi;
        public Int32 isHolding;
        public Int32 isEstop;
        public Int32 isHoming;
        public Int32 isThreading;
        public Int32 isProgSel;
        public Int32 isProgEnd;
        public Int32 isRewinded;
        public Int32 isReseting;
        public Int32 runLine;
        public String gCodeName;
        public Int32 partNum;
		public Int32 toolUse;
		public Double spdlSpeed;
        public Int32 TOOLPOS_NUM;//刀位总数 
        public Int32 TOOL_NUM;//当前刀具数  
        public long TOOL_NO;//当前刀具号

    }
}