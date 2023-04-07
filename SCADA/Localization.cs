 using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.Data;
namespace SCADA
{
    public static class Localization
    {
        #region Property
        public static string Lang { get; private set; }
        
        public static bool HasLang { get; set; }
        #endregion //Property

        #region Attribute
        private static Dictionary<string, Dictionary<string, string>> forms = new Dictionary<string, Dictionary<string, string>>();
        private static Dictionary<string, string> menu = new Dictionary<string, string>();
        private static Dictionary<string, string> toolbar = new Dictionary<string, string>();
        private static Dictionary<string, string> dialog = new Dictionary<string, string>();
        #endregion //Attribute

        #region Method
        public static void AddForm(string formName)
        {
          //  foreach (Dictionary<string, string> form in forms.Values)
              //  form.Clear();
         
            forms.Add(formName, new Dictionary<string, string>());
            //formMap.Add(formName, count++);
        }

        //读默认语言
        public static string ReadDefaultLanguage()
        {
            // XmlReader reader = new XmlTextReader("LanguageDefine.xml");
            XmlDocument doc = new XmlDocument();
            doc.Load("LanguageDefine.xml");
            //  doc.Load("Resources/LanguageDefine.xml");
            XmlNode root = doc.DocumentElement;
            //选取DefaultLangugae节点 
            XmlNode node = root.SelectSingleNode("Language/DefaultLanguage");
            string result = "zh";
            if (node != null)
                //取出节点中的内容 
                result = node.InnerText;

            //reader.Close();
            return result;
        }

        //修改默认语言 
        public static void WriteDefaultLanguage(string lang)
        {
            DataSet ds = new DataSet();
            ds.ReadXml("LanguageDefine.xml");
            DataTable dt = ds.Tables["Language"];

            dt.Rows[0]["DefaultLanguage"] = lang;
            ds.AcceptChanges();
            ds.WriteXml("LanguageDefine.xml");
        }

        //读默认皮肤
        public static string ReadDefaultSkin()
        {
            
            XmlDocument doc = new XmlDocument();
            doc.Load("SkinDefine.xml");          
            XmlNode root = doc.DocumentElement;
            XmlNode node = root.SelectSingleNode("Skin/DefaultSkin");
            string result = "WaveColor2";
            if (node != null)
                //取出节点中的内容 
                result = node.InnerText;

            //reader.Close();
            return result;
        }

        //修改默认皮肤
        public static void WriteDefaultSkin(string skin)
        {
            DataSet ds = new DataSet();
            ds.ReadXml("SkinDefine.xml");
            DataTable dt = ds.Tables["Skin"];

            dt.Rows[0]["DefaultSkin"] = skin;
            ds.AcceptChanges();
            ds.WriteXml("SkinDefine.xml");
        }
        /**/
        /// <summary>
        /// 加载语言文件
        /// </summary>
        /// <param name="lang">语言</param>
        /// <returns></returns>
        public static bool Load(string lang)
        {
            string path = "";
          //  Localization.Lang = "English";

            menu.Clear();
            toolbar.Clear();
            dialog.Clear();
          //    exception.Clear();
            foreach (Dictionary<string, string> form in forms.Values)
                form.Clear();

            switch (lang)
            {
                case "zh":
                    path = @"lang-zh.xml";
                    Localization.Lang = "zh";
                    break;
                case "en":
                    path = @"lang-en.xml";
                    Localization.Lang = "en";
                    break;
                default:
                    path = @"lang-zh.xml";
                    Localization.Lang = "zh";
                    break;
            }

            return readLanguage(path);
        }
        #endregion //Method

        #region Function
        private static bool readLanguage(string path)
        {
            // Read the language file
            XmlReader reader;
            try
            {
                reader = XmlReader.Create(path);
            }
            catch (Exception)
            {
                return false;
            }

            // Begin to parase
            try
            {
                reader.ReadToFollowing("AirControl");
            //    Localization.Lang = reader.GetAttribute("language");

               paraseXml(reader, "Menu", menu);
            //    paraseXml(reader, "Toolbar", toolbar);

                foreach (string formName in forms.Keys)
                {
                    paraseXml(reader, formName, forms[formName]);
                }
                paraseXml(reader, "Dialog", dialog);
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        private static void paraseXml(XmlReader reader, string item, Dictionary<string, string> obj)
        {
            // Get the attribute key & value 
            reader.ReadToFollowing(item);

            XmlReader subreader = reader.ReadSubtree();
            while (subreader.Read())
            {
                if (subreader.NodeType == XmlNodeType.Element && subreader.Name == "Item")
                    obj.Add(subreader.GetAttribute("key"), subreader.GetAttribute("value"));
            }
        }
        #endregion //Function

        #region Property
        public static Dictionary<string, string> Menu
        {
            get
            {
                return menu;
            }
            private set
            { }
        }

        public static Dictionary<string, string> Toolbar
        {
            get
            {
                return toolbar;
            }
            private set
            { }
        }

        public static Dictionary<string, Dictionary<string, string>> Forms
        {
            get
            {
                return forms;
            }
            private set
            { }
        }

        public static Dictionary<string, string> Dialog
        {
            get
            {
                return dialog;
            }
            private set
            { }
        }
        #endregion //Property

        public static void RefreshLanguage(Form form)
        {
            form.Text = Localization.Forms[form.Name][form.Name];
          
            SetControlsLanguage(form, Localization.Forms[form.Name]);
        }

        public static void SetControlsLanguage(Control control, Dictionary<string, string> obj)
        {
            foreach (Control ctrl in control.Controls)
            {
                // set the control which one's key in the dictionary
                string text = "";
                if (obj.TryGetValue(ctrl.Name, out text))
                    ctrl.Text = text;

                if (ctrl.HasChildren)
                    SetControlsLanguage(ctrl, obj);
            }
        }
    }
}
