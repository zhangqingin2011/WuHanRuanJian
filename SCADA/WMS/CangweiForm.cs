using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;

namespace SCADA
{
    public partial class CangweiForm : Form
    {
        public CangweiForm()
        {
            InitializeComponent();
        }

        ToolTip m_tooltip = new ToolTip();
        CangweiInfo[] cangweiArray = new CangweiInfo[96];
        public event EventHandler<CangweiInfoChangedEventArgs> CangweiInfoChanged;
        private void OnCangweiInfoChanged(CangweiInfo Info)
        {
            if (CangweiInfoChanged != null)
            {
                CangweiInfoChanged(this, new CangweiInfoChangedEventArgs(Info));
            }
        }

        public static CangweiSaveXML m_xml;
        public static string XMLSavePath = "..\\data\\Set\\CangweiSave.xml";//设置文件的路径
        Hashtable m_Hashtable;
        String[] translatestr = new String[10];

        private void PictureboxImagechange(CangweiInfo Info)
        {
            int index = Info.number - 1;
            String PictureboxName = "pictureBox" + (index + 1).ToString();
            Control[] items2 = tableLayoutPanelform.Controls.Find(PictureboxName, true);
            if (PictureboxName == items2[0].Name)
            {
                if (Info.materialflag)
                {
                    if (Info.typeIndex == 0 && Info.status == 0)
                    {
                        ((PictureBox)items2[0]).Image = global::SCADA.Properties.Resources.前端盖未加工;
                    }
                    else if (Info.typeIndex == 0 && Info.status == 1)
                    {
                        ((PictureBox)items2[0]).Image = global::SCADA.Properties.Resources.前端盖合格;
                    }
                    else if (Info.typeIndex == 0 && Info.status == 2)
                    {
                        ((PictureBox)items2[0]).Image = global::SCADA.Properties.Resources.前端盖不合格;
                    }
                    else if (Info.typeIndex == 1 && Info.status == 0)
                    {
                        ((PictureBox)items2[0]).Image = global::SCADA.Properties.Resources.后端盖未加工;
                    }
                    else if (Info.typeIndex == 1 && Info.status == 1)
                    {
                        ((PictureBox)items2[0]).Image = global::SCADA.Properties.Resources.后端盖合格;
                    }
                    else if (Info.typeIndex == 1 && Info.status == 2)
                    {
                        ((PictureBox)items2[0]).Image = global::SCADA.Properties.Resources.后端盖不合格;
                    }
                    else if (Info.typeIndex == 2 && Info.status == 0)
                    {
                        ((PictureBox)items2[0]).Image = global::SCADA.Properties.Resources.轴类未加工;
                    }
                    else if (Info.typeIndex == 2 && Info.status == 1)
                    {
                        ((PictureBox)items2[0]).Image = global::SCADA.Properties.Resources.轴类合格;
                    }
                    else if (Info.typeIndex == 2 && Info.status == 0)
                    {
                        ((PictureBox)items2[0]).Image = global::SCADA.Properties.Resources.轴类不合格;
                    }
                }
                else
                {
                    ((PictureBox)items2[0]).Image = global::SCADA.Properties.Resources.无料;
                }
            }
        }

        private void Loadlanguage()
        {
            m_Hashtable = ChangeLanguage.LoadOtherLanguage(this);
            translatestr[0] = ChangeLanguage.GetString(m_Hashtable, "Info01");
            translatestr[1] = ChangeLanguage.GetString(m_Hashtable, "Info02");
            translatestr[2] = ChangeLanguage.GetString(m_Hashtable, "Info03");
            translatestr[3] = ChangeLanguage.GetString(m_Hashtable, "Info04");
            translatestr[4] = ChangeLanguage.GetString(m_Hashtable, "Info05");
            translatestr[5] = ChangeLanguage.GetString(m_Hashtable, "Info06");
            translatestr[6] = ChangeLanguage.GetString(m_Hashtable, "Info07");
            translatestr[7] = ChangeLanguage.GetString(m_Hashtable, "Info08");
            translatestr[8] = ChangeLanguage.GetString(m_Hashtable, "Info09");
            translatestr[9] = ChangeLanguage.GetString(m_Hashtable, "Info10");
        }

        private void CangweiForm_Load(object sender, EventArgs e)
        {
            ChangeLanguage.LoadLanguage(this);
            Loadlanguage();
            MainForm.languagechangeEvent += LanguageChange;
            m_xml = new CangweiSaveXML();
            m_xml.m_load(XMLSavePath);
            for (int i = 0; i < m_xml.count; i++)
            {
                cangweiArray[i] = new CangweiInfo(i + 1);
                m_xml.GetCangweiInfo(i, ref cangweiArray[i].typeIndex, ref cangweiArray[i].status, ref cangweiArray[i].materialflag);
                String buttonName = "button" + (i + 1).ToString();
                String PictureboxName = "pictureBox" + (i + 1).ToString();
                Control[] items = tableLayoutPanelform.Controls.Find(buttonName, true);
                Control[] items2 = tableLayoutPanelform.Controls.Find(PictureboxName, true);
                if (buttonName == items[0].Name)
                {
                    ((Button)items[0]).MouseEnter += buttonArry_MouseEnter;
                    ((Button)items[0]).MouseLeave += buttonArry_MouseLeave;
                }
                if (PictureboxName == items2[0].Name)
                {
                    //((PictureBox)items2[0]).Image = global::WMS.Properties.Resources.无料;
                    PictureboxImagechange(cangweiArray[i]);
                }
            }
            CangweiInfoChanged += Form_CangweiInfoChanged;
        }

        void LanguageChange(object sender, string Language)
        {
            Loadlanguage();
        }

        void Form_CangweiInfoChanged(object sender, CangweiInfoChangedEventArgs e)
        {
            PictureboxImagechange(e.Info);
        }

        void buttonArry_MouseLeave(object sender, EventArgs e)
        {
            m_tooltip.Hide((Control)sender);
        }

        void buttonArry_MouseEnter(object sender, EventArgs e)
        {
            Button sr = sender as Button;

            string ButtonName = sr.Name.ToString();
            int serial = int.Parse(ButtonName.Substring(6));
            CangweiInfo info = cangweiArray[serial - 1];

            string str = translatestr[0] + info.number.ToString() + "\r\n";
            if (info.materialflag)
            {
                if (info.typeIndex == 0)
                {
                    str += translatestr[1] + translatestr[3];
                }
                else if (info.typeIndex == 1)
                {
                    str += translatestr[1] + translatestr[4];
                }
                else if (info.typeIndex == 2)
                {
                    str += translatestr[1] + translatestr[5];
                }
                str += "\r\n";
                if (info.status == 0)
                {
                    str += translatestr[2] + translatestr[6];
                }
                else if (info.status == 1)
                {
                    str += translatestr[2] + translatestr[7];
                }
                else if (info.status == 2)
                {
                    str += translatestr[2] + translatestr[8];
                }
            }
            else
            {
                str += translatestr[9];
            }
            m_tooltip.Show(str, (Control)sender);
        }

    }
}
