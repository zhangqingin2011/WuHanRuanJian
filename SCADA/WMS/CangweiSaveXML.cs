using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SCADA
{
    public class CangweiSaveXML
    {
        private XmlDocument XML_doc;
        public static String[] cangweiInfo_Attributes_str1 = { "number", "typeIndex", "status", "materialflag" };
        private String[] cangweiInfo_Value_str1 = { "1", "0", "0", "0" };

        public int count = 96; //仓位个数
        string sum = "SUM"; //总个数字段
        string item = "Item";
        public static String PathRoot_cangwei = "CangweiInfo" + "/" + "cangwei";
        /// <summary>
        /// 生成默认设置XML对象
        /// </summary>
        /// <param name="FilePath"></param>
        public void MakeDefaultSetXML(String FilePath)
        {
            XML_doc = new XmlDocument();
            try
            {
                XmlDeclaration dec = XML_doc.CreateXmlDeclaration("1.0", "utf-8", null);
                XML_doc.AppendChild(dec);
                XmlNode root = XML_doc.CreateElement("CangweiInfo");//一级

                XmlNode node;
                XmlAttribute xmlat;
                ///每个仓位信息
                node = XML_doc.CreateElement("cangwei");//二级
                xmlat = XML_doc.CreateAttribute(sum);
                xmlat.Value = "0";
                node.Attributes.Append(xmlat);
                for (int jj = 0; jj < cangweiInfo_Attributes_str1.Length; jj++)
                {
                    xmlat = XML_doc.CreateAttribute(cangweiInfo_Attributes_str1[jj]);
                    xmlat.Value = cangweiInfo_Value_str1[jj];
                    node.Attributes.Append(xmlat);
                }
                //InserNode(PathRoot_cangwei);
                XmlNode Element = XML_doc.CreateElement(item + "0");//三级
                node.AppendChild(Element);
                root.AppendChild(node);
                XML_doc.AppendChild(root);

                InserDefaulAttributes(PathRoot_cangwei, 0);
                m_UpdateAttribute(PathRoot_cangwei, -1, sum, XML_doc.SelectSingleNode(PathRoot_cangwei).ChildNodes.Count.ToString());
                for (int i = 1; i < count; i++)
                {
                    InserNode(PathRoot_cangwei);
                }

            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("生成默认设置文档失败！\r\n" +
                    ex.ToString());
            }
        }

        /// <summary>
        /// 创建XML全局变量并load文件进行初始化
        /// 文件不存在则创建一个新的默认文件并初始全局对象
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public char m_load(String FilePath)
        {
            XML_doc = new XmlDocument();//配置文件XML文件对象实例
            String[] Pathstr = FilePath.Split('\\');
            String FileDirectory = FilePath.Replace(Pathstr[Pathstr.Length - 1], "");
            FileDirectory = FileDirectory.Substring(0, FileDirectory.Length - 1);
            if (!System.IO.Directory.Exists(FileDirectory))
            {
                System.IO.Directory.CreateDirectory(FileDirectory);
            }
            if (System.IO.File.Exists(FilePath))
            {
                try
                {
                    XML_doc.Load(FilePath);

                    return (char)1;
                }
                catch
                {
                    MakeDefaultSetXML(FilePath);
                    SaveXmlfile(FilePath);
                    System.IO.File.Delete(FilePath + "error");
                    System.IO.File.Move(FilePath, FilePath + "error");
                    System.Windows.Forms.MessageBox.Show(FilePath + "格式已经被破坏，文件被备份为：" + FilePath + "error");
                    return (char)0;
                }
            }
            else
            {
                MakeDefaultSetXML(FilePath);
                SaveXmlfile(FilePath);
                return (char)0;
            }
        }

        /// <summary>
        /// 保存xml对象到文件
        /// </summary>
        /// <param name="FilePath"></param>
        public void SaveXmlfile(String FilePath)
        {
            try
            {
                XML_doc.Save(FilePath);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(FilePath + "保存失败！\r\n" +
                    ex.ToString());
            }
        }

        /// <summary>
        /// 插入默认属性
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <param name="serial">节点序号</param>
        public void InserDefaulAttributes(String path, int serial)
        {
            XmlAttribute xmlat;
            try
            {
                XmlNodeList m_xmllist = XML_doc.SelectSingleNode(path).ChildNodes;
                if (m_xmllist.Count > serial)
                {
                    XmlElement xe = (XmlElement)m_xmllist.Item(serial);
                    xmlat = XML_doc.CreateAttribute("serial");
                    xmlat.Value = serial.ToString();
                    xe.Attributes.Append(xmlat);

                    foreach (XmlAttribute xa in XML_doc.SelectSingleNode(path).Attributes)
                    {
                        if (xa.Name == cangweiInfo_Attributes_str1[0])
                        {
                            xmlat = XML_doc.CreateAttribute(xa.Name);
                            xmlat.Value = (serial + 1).ToString();
                            xe.Attributes.Append(xmlat);
                        }
                        else if (xa.Name != sum && xa.Name != "serial")
                        {
                            xmlat = XML_doc.CreateAttribute(xa.Name);
                            xmlat.Value = xa.Value;
                            xe.Attributes.Append(xmlat);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(path + "添加默认属性失败！\r\n" +
                    ex.ToString());
            }
        }

        /// <summary>
        /// 在serial中更新（属性存在）或者插入一个属性（属性不存在，按照默认属性插入）
        /// </summary>
        /// <param name="serial">节点序号</param>
        /// <param name="attribute">节点的属性</param>
        /// <param name="value">属性值</param>
        /// <returns></returns>
        public void m_UpdateAttribute(String path, int serial, String attribute, String value)//serial == -1是节点本身,serial == -2是子节点最后一个节点
        {
            try
            {
                XmlNodeList m_xmllist = XML_doc.SelectSingleNode(path).ChildNodes;
                if (serial == -1)//路径本身节点
                {
                    XmlElement m_element = (XmlElement)XML_doc.SelectSingleNode(path);
                    m_element.SetAttribute(attribute, value);
                }
                else if (m_xmllist.Count > serial)
                {
                    XmlElement xe;

                    if (serial == -2)//子节点最后一个节点
                    {
                        xe = (XmlElement)m_xmllist.Item(m_xmllist.Count - 1);
                    }
                    else
                    {
                        xe = (XmlElement)m_xmllist.Item(serial);
                    }
                    if (xe.GetAttribute(attribute).Equals(null))//属性不存在
                    {
                    }
                    else
                    {
                        xe.SetAttribute(attribute, value);
                    }

                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("属性修改失败！\r\n" +
                    ex.ToString());
            }
        }
        /// <summary>
        /// element节点列表最后面插入一个节点
        /// </summary>
        /// <param name="path">插入路径</param>
        /// <param name="elementname">插入的节点名称</param>
        public void InserNode(String path)
        {
            try
            {
                string itemname = item + XML_doc.SelectSingleNode(path).ChildNodes.Count.ToString();
                XmlNode m_node = XML_doc.CreateElement(itemname);

                XML_doc.SelectSingleNode(path).AppendChild(m_node);
                InserDefaulAttributes(path, XML_doc.SelectSingleNode(path).ChildNodes.Count - 1);

                m_UpdateAttribute(path, -1, sum, XML_doc.SelectSingleNode(path).ChildNodes.Count.ToString());
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(path + "插入失败！\r\n" +
                    ex.ToString());
            }
        }

        /// <summary>
        /// 读取XML_doc中的节点的属性
        /// </summary>
        /// <param name="path">XML_doc中节点路径</param>
        /// <param name="serial">节点序号</param>
        /// <param name="attribute">节点的属性</param>
        /// <returns>读到的值</returns>
        public String m_Read(String path, int serial, String attribute)
        {
            string str = "";
            try
            {
                XmlNodeList m_xmllist = XML_doc.SelectSingleNode(path).ChildNodes;
                if (serial == -1)
                {
                    XmlElement m_element = (XmlElement)XML_doc.SelectSingleNode(path);
                    str = m_element.GetAttribute(attribute);
                }
                else if (m_xmllist.Count > serial)
                {
                    XmlElement xe = (XmlElement)m_xmllist.Item(serial);
                    str = xe.GetAttribute(attribute);//读取对应节点的对应属性
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(path + "序号为：" + serial.ToString() + "属性为：" + attribute
                    + "读取失败！\r\n" +
                    ex.ToString());
            }
            return str;
        }

        public void GetCangweiInfo(int index, ref int typeIndex, ref int status, ref bool materialflag)
        {
            string type = m_Read(PathRoot_cangwei, index, cangweiInfo_Attributes_str1[1]);
            string Status = m_Read(PathRoot_cangwei, index, cangweiInfo_Attributes_str1[2]);
            string Flag = m_Read(PathRoot_cangwei, index, cangweiInfo_Attributes_str1[3]);
            typeIndex = int.Parse(type);
            status = int.Parse(Status);
            materialflag = (Flag == "0") ? false : true;
        }

        public void SetCangweiInfo(int index, int typeIndex, int status, bool materialflag)
        {
            m_UpdateAttribute(PathRoot_cangwei, index, cangweiInfo_Attributes_str1[1], typeIndex.ToString());
            m_UpdateAttribute(PathRoot_cangwei, index, cangweiInfo_Attributes_str1[2], status.ToString());
            if (materialflag)
            {
                m_UpdateAttribute(PathRoot_cangwei, index, cangweiInfo_Attributes_str1[3], "1");
            }
            else
            {
                m_UpdateAttribute(PathRoot_cangwei, index, cangweiInfo_Attributes_str1[3], "0");
            }
        }
    }
}
