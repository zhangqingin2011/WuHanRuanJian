using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
    public class StaticsData : Entity
    {
        public PRODUCTTYPE Product { get; set; }
        public PIECEQUALITY PieceQuality { get; set; }
    }
}
