﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HNCAPI
{

    public class JobSection
    {
        private System.Collections.Generic.Dictionary<Int32, ComponentSample> _SampleDic;
        private String _JobID;
        public System.Collections.Generic.Dictionary<Int32, ComponentSample> SampleDic
        {
            get
            {
                if (_SampleDic == null)
                {
                    _SampleDic = new Dictionary<int, ComponentSample>();
                }
                return _SampleDic;
            }
            set { _SampleDic = value; }
        }
        public string JobID
        {
            get
            {
                return _JobID;
            }

            set
            {
                _JobID = value;
            }
        }
        public JobSection()
        {
            _JobID = "";
        }
    }
}
