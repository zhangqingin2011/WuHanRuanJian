﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using HNCAPI;

namespace HNCAPI
{
    class EventDataCollector
    {
        private bool _eventThreadStart;
        private Thread _eventCollectThread;
        
        public EventDataCollector()
        {
            _eventThreadStart = false;
            _eventCollectThread = new Thread(new ThreadStart(EventCollectFunc));
        }

        private void EventCollectFunc()
        {
            Int32 ret = 0;
            Int16 clientNo = -1;
            SEventElement ev = new SEventElement();

            while (_eventThreadStart)
            {
                ret = HncApi.HNC_EventGetSysEv(ref ev);
                if (ret != 0)
                {
                    continue;
                }

                short[] info = new short[2];
                Buffer.BlockCopy(ev.buf, 0, info, 0, info.Length);
                clientNo = info[0];

                SessionManager.Instance().PublishEvent(ev, clientNo);
            }
        }

        public void Start()
        {
            _eventThreadStart = true;
            _eventCollectThread.Start();
        }

        public void Stop()
        {
            _eventThreadStart = false;
            _eventCollectThread.Join();
        }
    }
}
