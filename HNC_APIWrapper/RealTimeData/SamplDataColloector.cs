﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace HNCAPI
{
    class SamplDatacollector
    {
        private bool _samplThreadStart;
        private Thread _samplCollectThread;

        public SamplDatacollector()
        {
            _samplThreadStart = false;
            _samplCollectThread = new Thread(new ThreadStart(SamplCollectFunc));
        }

        private void SamplCollectFunc()
        {
            const Int32 ARR_SIZE = 5120;
            Int32 ret = 0;
            Int32[] arrData = new Int32[ARR_SIZE];
            UInt16 chNum = 0;
            UInt16 numPerCh = 0;
            Int16 clientNo = -1;
            
            
            while (_samplThreadStart)
            {
                try
                {
                    ret = HncApi.HNC_SamplDataRecv(arrData, ref chNum, ref numPerCh, ref clientNo);
                    if (ret != 0)
                    {
                        continue;
                    }

                    List<List<Int32>> listData = new List<List<Int32>>();

                    for (Int32 i = 0; i < chNum; i++)
                    {
                        listData.Add(new List<Int32>());
                        for (Int32 j = 0; j < numPerCh; j++)
                        {
                            listData[i].Add(arrData[i * numPerCh + j]);
                        }
                    }

                    SessionManager.Instance().PublishSample(listData, clientNo);
                }
                catch(Exception ee)
                {
                    Console.WriteLine("Exception in SampleFunction:" + ee.Message);
                }
            }
        }

        public void Start()
        {
            _samplThreadStart = true;
            _samplCollectThread.Start();
        }

        public void Stop()
        {
            _samplThreadStart = false;
            _samplCollectThread.Join();
        }
    }
}
