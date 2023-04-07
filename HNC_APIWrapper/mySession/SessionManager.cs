﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HNCAPI
{
    public class SessionManager:IPublisher
    {
        private List<Session> _SessionList;
        private SampleSet _smplSet;
        private SamplDatacollector _SmpCollector;
        private EventDataCollector _EvtCollector;
        private static SessionManager _Instance;
        private object _lock;
        
        private SessionManager()
        {
            _lock = new object();
        }
        public static SessionManager Instance()
        {
            if (_Instance == null)
            {
                _Instance = new SessionManager();
                
            }
            return _Instance;                  
        }
        public int InitNet(String ip, UInt16 port)
        {
            lock (_lock)
            {
                if (_smplSet != null) return 0;
                Int32 ret = HncApi.HNC_NetInit(ip, port);
                if (ret == 0 && _smplSet == null)
                {
                    _smplSet = new SampleSet();
                    _smplSet.ReadSamplConfigFile();
                    _SmpCollector = new SamplDatacollector();
                    _EvtCollector = new EventDataCollector();
                    _SmpCollector.Start();
                    _EvtCollector.Start();

                }
                return ret;
            }
           
        }
        public  Session CreateSession() 
        {
            
            if (_SessionList == null)
                _SessionList = new List<Session>();
            Session s = new Session();
            s.SmpSet = _smplSet;
            s.CloseEvent += new Session.CloseHandler(s_CloseEvent);
            _SessionList.Add(s);
            return s;
        }

        void s_CloseEvent(Session currentSession)
        {
            if (this._SessionList.Contains(currentSession))
            {
                _SessionList.Remove(currentSession);
            }
            if(_SessionList.Count==0)
            {
                if (_SmpCollector != null)
                {
                    _SmpCollector.Stop();
                }
                if(_EvtCollector!=null)
                    _EvtCollector.Stop();
                _smplSet = null;
                HncApi.HNC_NetExit();
            }
        }


        public void PublishEvent(SEventElement evt, Int16 ClientNo)
        {
            if (_SessionList == null) return;
            for (int i = 0; i < _SessionList.Count; i++)
            {
                _SessionList[i].ReceiveEvent(evt,ClientNo);
            }
        }

        public void PublishSample(List<List<Int32>> data, Int16 ClientNo)
        {
            //System.Windows.Forms.MessageBox.Show("PublishSample");
            if (_SessionList == null) return;
            for (int i = 0; i < _SessionList.Count; i++)
            {
                _SessionList[i].ReceiveSample(data, ClientNo);
            }
        }
    }
}
