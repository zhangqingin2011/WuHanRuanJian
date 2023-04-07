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
    public partial class SYSSetForm : Form
    {
        public SYSSetForm()
        {
            InitializeComponent();
        }

        private static Form[] m_Formarre1 = null;

        private void tabControlSYS_DrawItem(object sender, DrawItemEventArgs e)
        {
            //Image backImage = Properties.Resources.whiteBack;
            Rectangle rec = tabControlSYS.ClientRectangle;

            StringFormat StrFormat = new StringFormat();

            StrFormat.Alignment = StringAlignment.Center;
            StrFormat.LineAlignment = StringAlignment.Center;

            SolidBrush tabBackBrush = new SolidBrush(SystemColors.Control);
            //文字色
            SolidBrush FrontBrush = new SolidBrush(Color.DeepSkyBlue);
            //StringFormat stringF = new StringFormat();
            Font wordfont = new Font("微软雅黑", 12F, FontStyle.Bold);
            //e.Graphics.DrawImage(backImage, 0, 0, tabControlSYS.Width, tabControlSYS.Height);
            for (int i = 0; i < tabControlSYS.TabCount; i++)
            {
                //标签工作区
                Rectangle rec1 = tabControlSYS.GetTabRect(i);
                //e.Graphics.DrawImage(backImage, 0, 0, tabPagemain.Width, tabPagemain.Height);
                e.Graphics.FillRectangle(tabBackBrush, rec1);
                ////标签头背景色
                e.Graphics.DrawString(tabControlSYS.TabPages[i].Text, wordfont, FrontBrush, rec1, StrFormat);
                ////标签头文字
            }
        }

        private void SYSSetForm_Load(object sender, EventArgs e)
        {
            m_Formarre1 = new Form[tabControlSYS.TabCount];

            GenerateForm(0, tabControlSYS);
        }

        public void GenerateForm(int form_index, object sender)
        {
            // 反射生成窗体//只生成一次
            if (m_Formarre1[form_index] == null && form_index >= 0 && form_index < tabControlSYS.TabCount)
            {
                string formClassSTR = ((TabControl)sender).SelectedTab.Tag.ToString();
                m_Formarre1[form_index] = (Form)Assembly.GetExecutingAssembly().CreateInstance(formClassSTR);
                if (m_Formarre1[form_index] != null)
                {
                    //设置窗体没有边框 加入到选项卡中
                    m_Formarre1[form_index].FormBorderStyle = FormBorderStyle.None;
                    m_Formarre1[form_index].TopLevel = false;
                    m_Formarre1[form_index].Parent = ((TabControl)sender).SelectedTab;
                    m_Formarre1[form_index].ControlBox = false;
                    m_Formarre1[form_index].Dock = DockStyle.Fill;
                    if (ChangeLanguage.defaultcolor != Color.White)
                    {
                        ChangeLanguage.LoadSkin(m_Formarre1[form_index], ChangeLanguage.defaultcolor);
                    }
                    m_Formarre1[form_index].Show();
                }
            }
        }

        private void tabControlSYS_SelectedIndexChanged(object sender, EventArgs e)
        {
            GenerateForm(tabControlSYS.SelectedIndex, sender);
        }
    }
}
