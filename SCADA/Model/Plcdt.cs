using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA.Model
{
    public class Plcdt : TBaseTable
    {
        public string Name { get; set; }

        public int Status { get; set; }
    }
}
