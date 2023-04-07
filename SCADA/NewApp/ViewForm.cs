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
    public partial class ViewForm : Form
    {
        public ViewForm()
        {
            InitializeComponent();
            NVRForm.jpegpic += JpegShow;
        }

        NVRForm nvrs;
        void JpegShow(object sender, int index, string jpegpath)
        {
            string path = jpegpath.Replace("/", "\\");
            if (index == 0)
            {
                textBox1.Text = path;
                pictureBox1.Load(path);
            }
            else if (index == 1)
            {
                textBox2.Text = path;
                pictureBox2.Load(path);
            }
            else if (index == 2)
            {
                textBox3.Text = path;
                pictureBox3.Load(path);
            }
            else if (index == 3)
            {
                textBox4.Text = path;
                pictureBox4.Load(path);
            }
            else if (index == 4)
            {
                textBox5.Text = path;
                pictureBox5.Load(path);
            }
            else if (index == 5)
            {
                textBox6.Text = path;
                pictureBox6.Load(path);
            }
            else if (index == 6)
            {
                textBox7.Text = path;
                pictureBox7.Load(path);
            }
            else if (index == 7)
            {
                textBox8.Text = path;
                pictureBox8.Load(path);
            }
        }

        private void ButtonOpen_Click(object sender, EventArgs e)
        {
            if (nvrs != null)
                nvrs.Dispose();
            nvrs = new NVRForm();
            nvrs.Show();
        }

        private void ViewForm_Load(object sender, EventArgs e)
        {
            //groupBox5.Visible = false;
            //groupBox6.Visible = false;
            //groupBox7.Visible = false;
            //groupBox8.Visible = false;
        }
    }
}
