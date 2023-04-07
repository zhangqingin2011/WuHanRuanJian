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
    public partial class OrderMainForm : Form
    {
        public OrderMainForm()
        {
            InitializeComponent();
            m_Formarr = new Form[tabControlOR.TabCount];
            GenerateForm(0, tabControlOR);
            string formClassSTR = String.Empty;
            for (int i = 1; i < tabControlOR.TabCount; i++)
            {
                formClassSTR = tabControlOR.TabPages[i].Tag.ToString();
                m_Formarr[i] = (Form)Assembly.GetExecutingAssembly().CreateInstance(formClassSTR);
                m_Formarr[i].FormBorderStyle = FormBorderStyle.None;
                m_Formarr[i].TopLevel = false;
                m_Formarr[i].Parent = tabControlOR.TabPages[i];
                m_Formarr[i].ControlBox = false;
                m_Formarr[i].Dock = DockStyle.Fill;
            }
        }

        private Form[] m_Formarr = null;

        private void tabControlOR_DrawItem(object sender, DrawItemEventArgs e)
        {
            //Image backImage = Properties.Resources.whiteBack;
            Rectangle rec = tabControlOR.ClientRectangle;

            StringFormat StrFormat = new StringFormat();

            StrFormat.Alignment = StringAlignment.Center;
            StrFormat.LineAlignment = StringAlignment.Center;

            SolidBrush tabBackBrush = new SolidBrush(SystemColors.Control);
            //文字色
            SolidBrush FrontBrush = new SolidBrush(Color.DeepSkyBlue);
            //StringFormat stringF = new StringFormat();
            Font wordfont = new Font("微软雅黑", 12F, FontStyle.Bold);
            //e.Graphics.DrawImage(backImage, 0, 0, tabControlOR.Width, tabControlOR.Height);
            for (int i = 0; i < tabControlOR.TabCount; i++)
            {
                //标签工作区
                Rectangle rec1 = tabControlOR.GetTabRect(i);

                e.Graphics.FillRectangle(tabBackBrush, rec1);
                ////标签头背景色
                e.Graphics.DrawString(tabControlOR.TabPages[i].Text, wordfont, FrontBrush, rec1, StrFormat);
                ////标签头文字
            }
        }

        private void tabControlOR_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControlOR.SuspendLayout();
            GenerateForm(tabControlOR.SelectedIndex, sender);
            tabControlOR.ResumeLayout();
        }

        public void GenerateForm(int form_index, object sender)
        {
            // 反射生成窗体//只生成一次
            if (m_Formarr[form_index] == null && form_index >= 0 && form_index < tabControlOR.TabCount)
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
    }
}
