using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SCADA.NewApp
{
    public partial class BoardForm : Form
    {
        private Form[] m_Formarr = null;

        public BoardForm()
        {
            InitializeComponent();
            m_Formarr = new Form[tabControlAsi.TabCount];
            GenerateForm(0, tabControlAsi);
            string formClassSTR = String.Empty;
            for (int i = 0; i < tabControlAsi.TabCount; i++)
            {
                formClassSTR = tabControlAsi.TabPages[i].Tag.ToString();
                m_Formarr[i] = (Form)Assembly.GetExecutingAssembly().CreateInstance(formClassSTR);
                m_Formarr[i].FormBorderStyle = FormBorderStyle.None;
                m_Formarr[i].TopLevel = false;
                m_Formarr[i].Parent = tabControlAsi.TabPages[i];
                m_Formarr[i].ControlBox = false;
                m_Formarr[i].Dock = DockStyle.Fill;
            }
        }

        private void tabControlAsi_DrawItem(object sender, DrawItemEventArgs e)
        {
            //Image backImage = Image.FromFile("..\\picture\\tabBack.png");
            Rectangle rec = tabControlAsi.ClientRectangle;

            StringFormat StrFormat = new StringFormat();

            StrFormat.Alignment = StringAlignment.Center;
            StrFormat.LineAlignment = StringAlignment.Center;

            SolidBrush tabBackBrush = new SolidBrush(SystemColors.Control);
            //文字色
            SolidBrush FrontBrush = new SolidBrush(Color.DeepSkyBlue);
            //StringFormat stringF = new StringFormat();
            Font wordfont = new Font("微软雅黑", 12F, FontStyle.Bold);
            //e.Graphics.DrawImage(backImage, 0, 0, tabControlAsi.Width, tabControlAsi.Height);
            for (int i = 0; i < tabControlAsi.TabCount; i++)
            {
                //标签工作区
                Rectangle rec1 = tabControlAsi.GetTabRect(i);
                //e.Graphics.DrawImage(backImage, 0, 0, tabPagemain.Width, tabPagemain.Height);
                e.Graphics.FillRectangle(tabBackBrush, rec1);
                ////标签头背景色
                e.Graphics.DrawString(tabControlAsi.TabPages[i].Text, wordfont, FrontBrush, rec1, StrFormat);
                ////标签头文字
            }

            //tabControlAsi.TabPages.Remove(tabPageMset);
        }

        public void GenerateForm(int form_index, object sender)
        {
            // 反射生成窗体//只生成一次
            if (m_Formarr[form_index] == null && form_index >= 0 && form_index < tabControlAsi.TabCount)
            {
                string formClassSTR = ((TabControl)sender).SelectedTab.Tag.ToString();
                m_Formarr[form_index] = (Form)Assembly.GetExecutingAssembly().CreateInstance(formClassSTR);
                if (m_Formarr[form_index] != null)
                {
                    //设置窗体没有边框 加入到选项卡中
                    m_Formarr[form_index].FormBorderStyle = FormBorderStyle.None;
                    m_Formarr[form_index].TopLevel = false;
                    m_Formarr[form_index].Parent = ((TabControl)sender).SelectedTab;
                    m_Formarr[form_index].ControlBox = false;
                    m_Formarr[form_index].Dock = DockStyle.Fill;
                    m_Formarr[form_index].Show();
                }
            }
            else
            {
                m_Formarr[form_index].Show();
            }
        }

        private void tabControlAsi_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControlAsi.SuspendLayout();
            GenerateForm(tabControlAsi.SelectedIndex, sender);
            tabControlAsi.ResumeLayout();
        }
    }
}
