﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Model
{
    public class PositionStatus:TBaseTable

    {
        public string PositionID { get; set; }

        public int  StatusCode { get; set; }

        public string  MetrialType { get; set; }
    }
}
