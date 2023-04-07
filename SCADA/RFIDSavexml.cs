﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SCADA
{
    public class RFIDSavexml
    {
        public XmlDocument XML_doc;
        public static String[] Default_Path_str = { "Root", "siguRFID", "serial", "Item" };
        //string[] RFIDMessage = { "SUM", "TimeStamp", "UID", "TagData", "R/W", "MaterialNo", "ProcessRequire", "Process1", "Process1", "QualityCheckInfo", " " };
        public static String[] RFIDMessage = { "SUM", "时间戳", "标签识别码", "标签数据", "读或写", "物料编号", "工序要求", "工序1", "工序2", "质检信息" };
        public static String[] Default_RFIDMessage_value = { "1", "null", "null", "null", "null", "null", "null", "null", "null", "null", "null" };


        public enum Path_str
        {
            Root = 0,
            siguRFID,
            serial,
            Item
        }

        public static String PathRoot_siguRFID = Default_Path_str[(int)Path_str.Root] + "/" + Default_Path_str[(int)Path_str.siguRFID];

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

                    for (int ii = 0; ii < RFIDMessage.Length; ii++)
                    {
                        Default_RFIDMessage_value[ii] = ((XmlElement)XML_doc.SelectSingleNode(PathRoot_siguRFID)).Attributes[ii].Value;
                    }
                    return (char)1;
                }
                catch
                {
                    MakeDefaultSetXML(FilePath);
                    SaveXml2File(FilePath);
                    System.IO.File.Delete(FilePath + "error");
                    System.IO.File.Move(FilePath, FilePath + "error");
                    System.Windows.Forms.MessageBox.Show(FilePath + "格式已经被破坏，文件被备份为：" + FilePath + "error");
                    return (char)0;
                }
            }
            else
            {
                MakeDefaultSetXML(FilePath);
                SaveXml2File(FilePath);
                return (char)0;
            }
        }

        /// <summary>
        /// 生成默认设置XML对象
        /// </summary>
        /// <param name="FilePath"></param>
        private void MakeDefaultSetXML(String FilePath)
        {
            try
            {
                XmlDeclaration dec = XML_doc.CreateXmlDeclaration("1.0", "utf-8", null);
                XML_doc.AppendChild(dec);
                XmlNode root = XML_doc.CreateElement(Default_Path_str[(int)Path_str.Root]);//一级

                ///思谷RFID
                XmlNode node = XML_doc.CreateElement(Default_Path_str[(int)Path_str.siguRFID]);//二级
                XmlAttribute xmlat;
                int jj = 0;
                for (jj = 0; jj < RFIDMessage.Length; jj++)
                {
                    xmlat = XML_doc.CreateAttribute(RFIDMessage[jj]);
                    xmlat.Value = Default_RFIDMessage_value[jj];
                    node.Attributes.Append(xmlat);
                }
                XmlNode Element = XML_doc.CreateElement(Default_Path_str[(int)Path_str.Item] + "0");//三级
                node.AppendChild(Element);
                root.AppendChild(node);


                XML_doc.AppendChild(root);

                InserDefaulAttributes(PathRoot_siguRFID, 0);
                m_UpdateAttribute(PathRoot_siguRFID, -1, RFIDMessage[0], XML_doc.SelectSingleNode(PathRoot_siguRFID).ChildNodes.Count.ToString());

            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("生成默认设置文档失败！\r\n" +
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
                    xmlat = XML_doc.CreateAttribute(Default_Path_str[(int)Path_str.serial]);
                    xmlat.Value = serial.ToString();
                    xe.Attributes.Append(xmlat);

                    foreach (XmlAttribute xa in XML_doc.SelectSingleNode(path).Attributes)
                    {
                        if (xa.Name != RFIDMessage[0] && xa.Name != Default_Path_str[(int)Path_str.serial])
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
                System.Windows.Forms.MessageBox.Show(path + "添加默认属性失败！\r\n" + ex.ToString());
            }
        }

        /// <summary>
        /// 保存xml对象到文件
        /// </summary>
        /// <param name="FilePath"></param>
        public void SaveXml2File(String FilePath)
        {
            try
            {
                XML_doc.Save(FilePath);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(FilePath + "保存失败！\r\n" + ex.ToString());
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
                string itemname = Default_Path_str[(int)Path_str.Item] + XML_doc.SelectSingleNode(path).ChildNodes.Count.ToString();
                XmlNode m_node = XML_doc.CreateElement(itemname);
                string path3 = "";

                XML_doc.SelectSingleNode(path).AppendChild(m_node);
                InserDefaulAttributes(path, XML_doc.SelectSingleNode(path).ChildNodes.Count - 1);
                if (path3 != "")
                {
                    InserDefaulAttributes(path3 + "/" + itemname + "/" + Default_Path_str[6], 0);
                    InserDefaulAttributes(path3 + "/" + itemname + "/" + Default_Path_str[7], 0);
                }

                m_UpdateAttribute(path, -1, RFIDMessage[0], XML_doc.SelectSingleNode(path).ChildNodes.Count.ToString());
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(path + "插入失败！\r\n" +
                    ex.ToString());
            }
        }

        /// <summary>
        /// 在serial中跟新（属性存在）或者插入一个属性（属性不存在），按照默认属性插入
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

        public void Change_Attributes(String Default_Attributes_str, String value)
        {
            for (int ii = -1; ii < XML_doc.SelectSingleNode(PathRoot_siguRFID).ChildNodes.Count; ii++)
            {
                m_UpdateAttribute(PathRoot_siguRFID, ii, Default_Attributes_str, value);
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

    }
}
