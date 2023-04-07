 using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Data;
using System.Collections;
using System.Drawing;
namespace SCADA
{
    public static class ChangeLanguage
    {
        //默认当前语言
        public static string DefaultLanguage = "Chinese";
        public static Color defaultcolor = Color.White;
        //读取当前默认语言
        public static string GetDefaultLanguage()
        {
            string defaultLanguge = "Chinese";
            XmlReader reader = new XmlTextReader(@"DefaultLanguage.xml");
            XmlDocument doc = new XmlDocument();

            /*doc.Load(reader);
            XmlNode root = doc.DocumentElement;
            //选取节点
            XmlNode node = root.SelectSingleNode("DefaultLanguage");
            if (node != null)
            {
                //取出节点中的内容   
                defaultLanguge = node.InnerText;
            }
            reader.Close();*/
            // reader.Dispose();
            return defaultLanguge;
        }

        /// <summary>  
        /// 修改默认语言  
        /// </summary>  
        /// <param name="lang">待设置默认语言</param>  
        public static void SetDefaultLanguage(string lang)
        {
            DataSet ds = new DataSet();
            ds.ReadXml(@"DefaultLanguage.xml");
            DataTable dt = ds.Tables["Softimite"];
            dt.Rows[0]["DefaultLanguage"] = lang;
            ds.AcceptChanges();
            ds.WriteXml(@"DefaultLanguage.xml");
            DefaultLanguage = lang;
        }

        /// <summary>  
        /// 从XML文件中读取需要修改Text的內容  
        /// </summary>  
        /// <param name="frmName">窗口名，用于获取对应窗口的那部分内容</param>  
        /// <param name="lang">目标语言</param>  
        /// <returns></returns> 
        private static Hashtable ReadXmlText(string FrmName, string lang)
        {
            try
            {
                Hashtable hashResult = new Hashtable();
                XmlReader reader = null;
                //判断是否存在该语言配置
                if (!(new System.IO.FileInfo(lang + ".xml")).Exists)
                {
                    return null;
                }
                else
                {
                    reader = new XmlTextReader(lang + ".xml");
                }
                XmlDocument doc = new XmlDocument();
                doc.Load(reader);
                XmlNode root = doc.DocumentElement;
                //获取XML文件中对应该窗口的内容  
                XmlNodeList nodeList = root.SelectNodes("Form[Name='" + FrmName + "']/Controls/Control");
                foreach (XmlNode node in nodeList)
                {
                    try
                    {
                        //修改内容为控件的Text值  
                        XmlNode node1 = node.SelectSingleNode("@name");
                        XmlNode node2 = node.SelectSingleNode("@text");
                        if (node1 != null)
                        {
                            hashResult.Add(node1.InnerText.ToLower(), node2.InnerText);
                        }
                    }
                    catch { }
                }
                reader.Close();
                //reader.Dispose();  
                return hashResult;
            }
            catch
            {
                return null;
            }
        }

        public static Hashtable LoadOtherLanguage(Form form)
        {
            //获取当前默认语言  
            string language = GetDefaultLanguage();
            //根据用户选择的语言获得表的显示文字   
            Hashtable hashText = ReadXmlText(form.Name, language);
            return hashText;
        }

        public static string GetString(Hashtable hashText, string id)
        {
            string str = "NULL";
            if (hashText.Contains(id.ToLower()))
            {
                str = (string)hashText[id.ToLower()];
            }
            return str;
        }

        public static string GetString(string id)
        {
            string str = "NULL";
            //获取当前默认语言  
            string language = GetDefaultLanguage();
            //根据用户选择的语言获得表的显示文字   
            Hashtable hashText = ReadXmlText("NotOneForm", language);
            if (hashText ==null)
            {
                return str;
            }
            if (hashText.Contains(id.ToLower()))
            {
                str = (string)hashText[id.ToLower()];
            }
            return str;
        }

        /// <summary>  
        /// 加载语言  
        /// </summary>  
        /// <param name="form">加载语言的窗口</param>  
        public static void LoadLanguage(Form form)
        {
            //获取当前默认语言  
            string language = GetDefaultLanguage();
            //根据用户选择的语言获得表的显示文字   
            Hashtable hashText = ReadXmlText(form.Name, language);
            if (hashText == null)
            {
                return;
            }
            //获取当前窗口的所有控件      
            Control.ControlCollection sonControls = form.Controls;
            try
            {
                //遍历所有控件  
                foreach (Control control in sonControls)
                {
                    if (control.GetType() == typeof(Panel))     //Panel  
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    else if (control.GetType() == typeof(GroupBox))     //GroupBox  
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    else if (control.GetType() == typeof(TabControl))       //TabControl  
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    else if (control.GetType() == typeof(TabPage))      //TabPage  
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(SplitterPanel))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(SplitContainer))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(FlowLayoutPanel))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(TableLayoutPanel))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }               
                    if (control.GetType() == typeof(DataGridView))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(TreeView))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(StatusStrip))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(ToolStripStatusLabel))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    } 
                    if (control.GetType() == typeof(Button))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (hashText.Contains(control.Name.ToLower()))
                    {
                        control.Text = (string)hashText[control.Name.ToLower()];
                    }
                }
                if (hashText.Contains(form.Name.ToLower()))
                {
                    form.Text = (string)hashText[form.Name.ToLower()];
                }
            }
            catch { }
        }

        public static void LoadSkin(Form form, Color color)
        {
            defaultcolor = color;
            form.BackColor = color;
            //获取当前窗口的所有控件      
            Control.ControlCollection sonControls = form.Controls;
            try
            {
                //遍历所有控件  
                foreach (Control control in sonControls)
                {
                    GetSetSubControls(control.Controls, color);
                    if (control.GetType() != typeof(Button) && control.GetType() != typeof(PictureBox))
                    {
                        control.BackColor = color;
                    }
                }
            }
            catch { }
        }

        private static void GetSetSubControls(Control.ControlCollection controls, Color color)
        {
            try
            {
                foreach (Control control in controls)
                {
                    GetSetSubControls(control.Controls, color);
                    if (control.GetType() != typeof(Button) && control.GetType() != typeof(PictureBox))
                    {
                        control.BackColor = color;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>  
        /// 获取并设置控件中的子控件  
        /// </summary>  
        /// <param name="controls">父控件</param>  
        /// <param name="hashResult">哈希表</param>  
        private static void GetSetSubControls(Control.ControlCollection controls, Hashtable hashText)
        {
            try
            {
                foreach (Control control in controls)
                {
                    if (control.GetType() == typeof(Panel))     //Panel  
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    else if (control.GetType() == typeof(GroupBox))     //GroupBox  
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    else if (control.GetType() == typeof(TabControl))       //TabControl  
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    else if (control.GetType() == typeof(TabPage))      //TabPage  
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    else if (control.GetType() == typeof(SplitterPanel))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(SplitContainer))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(FlowLayoutPanel))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(TableLayoutPanel))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(DataGridView))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(TreeView))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(StatusStrip))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (control.GetType() == typeof(ToolStripStatusLabel))
                    {
                        GetSetSubControls(control.Controls, hashText);
                    }
                    if (hashText.Contains(control.Name.ToLower()))
                    {
                        control.Text = (string)hashText[control.Name.ToLower()];
                        //control.HeaderText = (string)hashText[control.Name.ToLower()];
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }  

    }
}
