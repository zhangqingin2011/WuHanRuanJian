using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCADA.NewApp
{
    public partial class IOShow : UserControl
    {
        public IOShow()
        {
            InitializeComponent();
        }

        public Label Description
        {
            get {
                return label1;
            }
            set {
                label1 = value;
            }
        }

        public PictureBox Picture
        {
            get {
                return pictureBox1;
            }
            set {
                pictureBox1 = value;
            }
        }
    }
}
