﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScadaHncData
{
    [Serializable]
    public class AxisData : BaseNC
    {
        public Int32 axisNo;
        public String axisName;
        public Int32 axisType;
        public Int32 dist;
        public Int32 pulse;
        public Double actPos;
        public Double cmdPos;
        public Double followErr;
        public Double svCurrent;
        public Double loadCurrent;
        public String drvVer;
    }
}