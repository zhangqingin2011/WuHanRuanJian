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
    public partial class CncUnit : UserControl
    {
        public CncUnit()
        {
            InitializeComponent();
            InitUI();
        }

        private void InitUI()
        {
            ioShow1.Description.Text = "联机";
            ioShow1.Picture.Image = Image.FromFile("top_bar_black.png");
            ioShow2.Description.Text = "自动门关";
            ioShow2.Picture.Image = Image.FromFile("top_bar_black.png");
            ioShow3.Description.Text = "卡盘夹紧";
            ioShow3.Picture.Image = Image.FromFile("top_bar_black.png");
            ioShow4.Description.Text = "加工中";
            ioShow4.Picture.Image = Image.FromFile("top_bar_black.png");
            ioShow5.Description.Text = "报警";
            ioShow5.Picture.Image = Image.FromFile("top_bar_black.png");
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

        public GroupBox Description
        {
            get {
                return groupBox1;
            }
            set {
                groupBox1 = value;
            }
        }

        public IOShow OnlineIO
        {
            get
            {
                return ioShow1;
            }
            set
            {
                ioShow1 = value;
            }
        }

        public IOShow DoorIO
        {
            get
            {
                return ioShow2;
            }
            set
            {
                ioShow2 = value;
            }
        }

        public IOShow ChuckIO
        {
            get
            {
                return ioShow3;
            }
            set
            {
                ioShow3 = value;
            }
        }

        public IOShow WorkIO
        {
            get
            {
                return ioShow4;
            }
            set
            {
                ioShow4 = value;
            }
        }

        public IOShow AlarmIO
        {
            get
            {
                return ioShow5;
            }
            set
            {
                ioShow5 = value;
            }
        }
    }
}
