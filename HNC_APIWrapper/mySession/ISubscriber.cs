﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HNCAPI
{
    interface ISubscriber
    {
        void ReceiveEvent(SEventElement evt, Int16 clientNo);
        void ReceiveSample(List<List<Int32>> data,Int16 ClientNo);
    }
}
