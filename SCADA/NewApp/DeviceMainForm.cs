using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Reflection;

namespace SCADA.NewApp
{
    public partial class DeviceMainForm : Form
    {
        private Form[] m_Formarr = null;

        public DeviceMainForm()
        {
            InitializeComponent();
            m_Formarr = new Form[tabControlEQ1.TabCount];
            GenerateForm(0, tabControlEQ1);
            string formClassSTR = String.Empty;
            for (int i = 0; i < tabControlEQ1.TabCount; i++)
            {
                formClassSTR = tabControlEQ1.TabPages[i].Tag.ToString();
                m_Formarr[i] = (Form)Assembly.GetExecutingAssembly().CreateInstance(formClassSTR);
                m_Formarr[i].FormBorderStyle = FormBorderStyle.None;
                m_Formarr[i].TopLevel = false;
                m_Formarr[i].Parent = tabControlEQ1.TabPages[i];
                m_Formarr[i].ControlBox = false;
                m_Formarr[i].Dock = DockStyle.Fill;
            }
        }

        private void tabControlEQ1_DrawItem(object sender, DrawItemEventArgs e)
        {
            //Image backImage = Properties.Resources.whiteBack;
            Rectangle rec = tabControlEQ1.ClientRectangle;

            StringFormat StrFormat = new StringFormat();

            StrFormat.Alignment = StringAlignment.Center;
            StrFormat.LineAlignment = StringAlignment.Center;

            SolidBrush tabBackBrush = new SolidBrush(SystemColors.Control);
            //文字色
            SolidBrush FrontBrush = new SolidBrush(Color.DeepSkyBlue);
            //StringFormat stringF = new StringFormat();
            Font wordfont = new Font("微软雅黑", 12F, FontStyle.Bold);
            //e.Graphics.DrawImage(backImage, 0, 0, tabControlEQ1.Width, tabControlEQ1.Height);
            for (int i = 0; i < tabControlEQ1.TabCount; i++)
            {
                //标签工作区
                Rectangle rec1 = tabControlEQ1.GetTabRect(i);

                e.Graphics.FillRectangle(tabBackBrush, rec1);
                ////标签头背景色
                e.Graphics.DrawString(tabControlEQ1.TabPages[i].Text, wordfont, FrontBrush, rec1, StrFormat);
                ////标签头文字
            }
        }

        public void GenerateForm(int form_index, object sender)
        {
            // 反射生成窗体//只生成一次
            if (m_Formarr[form_index] == null && form_index >= 0 && form_index < tabControlEQ1.TabCount)
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

        private void tabControlEQ1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControlEQ1.SuspendLayout();
            GenerateForm(tabControlEQ1.SelectedIndex, sender);
            tabControlEQ1.ResumeLayout();
        }
    }
}
