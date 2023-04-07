﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HNCAPI
{
    interface IPublisher
    {
        void PublishEvent(SEventElement evt, Int16 clientNo);
        void PublishSample(List<List<Int32>> data,Int16 ClientNo);
    }
}
