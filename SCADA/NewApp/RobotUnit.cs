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
    public partial class RobotUnit : UserControl
    {
        public RobotUnit()
        {
            InitializeComponent();
        }

        public PictureBox Picture
        {
            get
            {
                return pictureBox;
            }
            set
            {
                pictureBox = value;
            }
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



        public IOShow HomeIO
        {
            get
            {
                return iohome;
            }
            set
            {
                iohome = value;
            }
        }

        public IOShow AlarmIO
        {
            get
            {
                return ioalarm;
            }
            set
            {
                ioalarm = value;
            }
        }
        public IOShow AotoIO
        {
            get
            {
                return ioaoto;
            }
            set
            {
                ioaoto = value;
            }
        }
        public IOShow BusyIO
        {
            get
            {
                return iobusy;
            }
            set
            {
                iobusy = value;
            }
        }

        public IOShow ShowJ1
        {
            get
            {
                return ioShowJ1;
            }
            set
            {
                ioShowJ1 = value;
            }
        }

        public IOShow ShowJ2
        {
            get
            {
                return ioShowJ2;
            }
            set
            {
                ioShowJ2 = value;
            }
        }

        public IOShow ShowJ3
        {
            get
            {
                return ioShowJ3;
            }
            set
            {
                ioShowJ3 = value;
            }
        }

        public IOShow ShowJ4
        {
            get
            {
                return ioShowJ4;
            }
            set
            {
                ioShowJ4 = value;
            }
        }

        public IOShow ShowJ5
        {
            get
            {
                return ioShowJ5;
            }
            set
            {
                ioShowJ5 = value;
            }
        }

        public IOShow ShowJ6
        {
            get
            {
                return ioShowJ6;
            }
            set
            {
                ioShowJ6 = value;
            }
        }

        public IOShow ShowJ7
        {
            get
            {
                return ioShowJ7;
            }
            set
            {
                ioShowJ7 = value;
            }
        }
    }
}
