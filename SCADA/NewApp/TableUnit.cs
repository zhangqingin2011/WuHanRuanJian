using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCADA.NewApp
{
    public partial class TableUnit : UserControl
    {
        public TableUnit()
        {
            InitializeComponent();
        }

        public GroupBox Description
        {
            get
            {
                return groupBox1;
            }
            set
            {
                groupBox1 = value;
            }
        }



        public IOShow ShowID1
        {
            get
            {
                return ioShowid1;
            }
            set
            {
                ioShowid1 = value;
            }
        }
        public IOShow ShowID2
        {
            get
            {
                return ioShowid2;
            }
            set
            {
                ioShowid2 = value;
            }
        }
        public IOShow ShowIP1
        {
            get
            {
                return ioShowp1;
            }
            set
            {
                ioShowp1 = value;
            }
        }
        public IOShow ShowIP2
        {
            get
            {
                return ioShowp2;
            }
            set
            {
                ioShowp2 = value;
            }
        }
        public IOShow ShowS11
        {
            get
            {
                return ioShows11;
            }
            set
            {
                ioShows11 = value;
            }
        }
        public IOShow ShowS12
        {
            get
            {
                return ioShows12;
            }
            set
            {
                ioShows12 = value;
            }
        }
        public IOShow ShowS13
        {
            get
            {
                return ioShows13;
            }
            set
            {
                ioShows13 = value;
            }
        }
        public IOShow ShowS14
        {
            get
            {
                return ioShows14;
            }
            set
            {
                ioShows14 = value;
            }
        }

        public IOShow ShowS24
        {
            get
            {
                return ioShows24;
            }
            set
            {
                ioShows24 = value;
            }
        }
        public IOShow ShowS21
        {
            get
            {
                return ioShows21;
            }
            set
            {
                ioShows21= value;
            }
        }
        public IOShow ShowS22
        {
            get
            {
                return ioShows22;
            }
            set
            {
                ioShows22 = value;
            }
        }
        public IOShow ShowS23
        {
            get
            {
                return ioShows23;
            }
            set
            {
                ioShows23 = value;
            }
        }
    }
}
