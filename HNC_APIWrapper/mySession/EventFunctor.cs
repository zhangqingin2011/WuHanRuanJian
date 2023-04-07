﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HNCAPI
{
    class EventFunctor
    {
        Machine _mac;
        SEventElement _e;
        public EventFunctor(SEventElement e,Machine mac)
        {
            _e = e;
            _mac = mac;
        }
        public void fireEvent()
        {
            _mac.EventFunc(_e, _mac.ClientNo);

        }
    }
}
