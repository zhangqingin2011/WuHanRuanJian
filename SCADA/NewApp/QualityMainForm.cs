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
    public partial class QualityMainForm : Form
    {
        private Form[] m_Formarr = null;
        public QualityMainForm()
        {
            InitializeComponent();
            m_Formarr = new Form[tabControl1.TabCount];
            GenerateForm(0, tabControl1);
            string formClassSTR = String.Empty;
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                formClassSTR = tabControl1.TabPages[i].Tag.ToString();
                m_Formarr[i] = (Form)Assembly.GetExecutingAssembly().CreateInstance(formClassSTR);
                m_Formarr[i].FormBorderStyle = FormBorderStyle.None;
                m_Formarr[i].TopLevel = false;
                m_Formarr[i].Parent = tabControl1.TabPages[i];
                m_Formarr[i].ControlBox = false;
                m_Formarr[i].Dock = DockStyle.Fill;
            }
           
        }

        private void tabControl1_DrawItem(object sender, DrawItemEventArgs e)
        {
            //Image backImage = Properties.Resources.whiteBack;
            Rectangle rec = tabControl1.ClientRectangle;

            StringFormat StrFormat = new StringFormat();

            StrFormat.Alignment = StringAlignment.Center;
            StrFormat.LineAlignment = StringAlignment.Center;

            SolidBrush tabBackBrush = new SolidBrush(SystemColors.Control);
            //文字色
            SolidBrush FrontBrush = new SolidBrush(Color.DeepSkyBlue);
            //StringFormat stringF = new StringFormat();
            Font wordfont = new Font("微软雅黑", 12F, FontStyle.Bold);
            //e.Graphics.DrawImage(backImage, 0, 0, tabControlEQ1.Width, tabControlEQ1.Height);
            for (int i = 0; i < tabControl1.TabCount; i++)
            {
                //标签工作区
                Rectangle rec1 = tabControl1.GetTabRect(i);

                e.Graphics.FillRectangle(tabBackBrush, rec1);
                ////标签头背景色
                e.Graphics.DrawString(tabControl1.TabPages[i].Text, wordfont, FrontBrush, rec1, StrFormat);
                ////标签头文字
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabControl1.SuspendLayout();
            GenerateForm(tabControl1.SelectedIndex, sender);
            tabControl1.ResumeLayout();
        }
        public void GenerateForm(int form_index, object sender)
        {
            // 反射生成窗体//只生成一次
            if (m_Formarr[form_index] == null && form_index >= 0 && form_index < tabControl1.TabCount)
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
