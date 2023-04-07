using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using System.Windows.Forms;

namespace SCADA
{
    public class ConfigXmlReader
    {
        public Dictionary<string, string> configDictionary = new Dictionary<string, string>();
        public void ReadXml(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("文件：" + path + "不存在！");
                return;
            }
            FileStream stream = File.OpenRead(path);
            var doc = new XmlDocument();
            doc.Load(stream);
            XmlNode node = doc.SelectSingleNode("/Root/appsetting");
            foreach (XmlNode item in node.ChildNodes)
            {
                if (item.Attributes != null)
                {
                    if (!configDictionary.ContainsKey(item.Attributes[0].InnerText))
                    {
                        configDictionary.Add(item.Attributes[0].InnerText, item.Attributes[1].InnerText);
                        Console.WriteLine(item.Attributes[0].InnerText + ":" + item.Attributes[1].InnerText);
                    }
                }
            }
        }
    }
}
