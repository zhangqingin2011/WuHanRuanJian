using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;
using SCADA.NewApp;

namespace SCADA
{
    public class LogData
    {
        public static String[] LogDataNode0Name = { "Root", "System", "Equipment", "Network" };
        public static String[] LogDataNode1Name = { "System_security", "System_runing", "Equipment_CNC", "Equipment_ROBOT", "Equipment_PLC", "Equipment_RFID", "Equipment_CCD" };
        public static String[] LogDataNode2Attributes = { "Level", "TimeCreated", "Provider", "EventID", "Keywords", "EventData" };//来源，事件ID，等级，解释，创建时间,事件数据
        public static String[] LogDataNode2Level = { "消息", "警告", "错误", "严重", "审核"};
        public static String[] DataDataTableShowColumnStr = { "级别", "日期和时间", "来源", "事件ID", "事件描述", "事件数据" };
        int MAXLogDataNode = 10000;//大约5000条1M
        public enum Node0Name
        {
            Root = 0,
            System ,
            Equipment,
            Network
        }
        public enum Node1Name
        {
            System_security = 0,
            System_runing,
            Equipment_CNC,
            Equipment_ROBOT,
            Equipment_PLC,
            Equipment_RFID,
            Equipment_CCD
        }
        public enum Node2Attributes
        {
            Level = 0,
            TimeCreated,
            Provider,
            EventID,
            Keywords,
            EventData
        }
        public enum Node2Level
        {
            MESSAGE = 0,
            WARNING,
            ERROR,
            FAULT,
            AUDIT
        }

        private XDocument LogXmlDoc = null ;
        private String FileNmae;
        public System.Data.DataTable ShowEventDataDataTable = new System.Data.DataTable();
        private System.Threading.Thread SaveThread;
        public /*static*/ System.EventHandler<EventHandlerSendParm> AddLogMsgHandler;
        public System.EventHandler<int> AddLogMsgClick1;

        /// <summary>
        /// 创建XML全局变量并load文件进行初始化
        /// 文件不存在则创建一个新的默认文件并初始全局对象
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public char m_load(String FilePath,bool Flg)
        {
            LoadLanguage();
            ShowEventDataDataTable = new System.Data.DataTable();
            char ret = (char)0;
            FileNmae = FilePath;
            //MainForm.languagechangeEvent += LanguageChange; 
            String[] Pathstr = FilePath.Split('\\');
            Pathstr[0] = FilePath.Replace(Pathstr[Pathstr.Length - 1], "");
            Pathstr[0] = Pathstr[0].Substring(0, Pathstr[0].Length - 1);
            if (!System.IO.Directory.Exists(Pathstr[0]))
            {
                System.IO.Directory.CreateDirectory(Pathstr[0]);
            }
            if (System.IO.File.Exists(FilePath))
            {
                try
                {
                    LogXmlDoc = XDocument.Load(FilePath);
                }
                catch
                {
                    System.IO.File.Delete(FilePath + "error");
                    System.IO.File.Move(FilePath, FilePath + "error");
                    MakeXMLDefaultstructure();
                    LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.System_security;
                    SendParm.LevelIndex = (int)LogData.Node2Level.MESSAGE;
                    SendParm.EventID = ((int)LogData.Node2Level.MESSAGE).ToString();
                    SendParm.Keywords = "保存历史日志";
                    SendParm.EventData = FilePath + "格式已经被破坏，文件被备份为：" + FilePath + "error";
                    EventHandlerFuc(this, SendParm);

//                     LogXmlDoc.Save(FilePath);
//                     System.Windows.Forms.MessageBox.Show(FilePath + "格式已经被破坏，文件被备份为：" + FilePath + "errer");
                }
                ret = (char)1;
            }
            else
            {
                MakeXMLDefaultstructure();
                LogXmlDoc.Save(FilePath);
            }
            if (Flg)
            {
                SaveThread = new System.Threading.Thread(SaveFuc);
                SaveThread.Start();
            }
            AddLogMsgHandler = new EventHandler<EventHandlerSendParm>(this.EventHandlerFuc);
            return ret;
        }

        void LanguageChange(object sender, string Language)
        {
            LoadLanguage();
        }

        private void LoadLanguage()
        {
            string[] LogDatalevelEnglish = { "Message", "Warning", "Error", "Fault", "Audit" };
            string[] LogDatalevelChinese = { "消息", "警告", "错误", "严重", "审核" };
            /*if (ChangeLanguage.GetDefaultLanguage() == "English")
            {
                for (int i = 0; i < LogDataNode2Level.Length; i++)
                {
                    LogDataNode2Level[i] = LogDatalevelEnglish[i];
                }
            }
            else*/
            {
                for (int i = 0; i < LogDataNode2Level.Length; i++)
                {
                    LogDataNode2Level[i] = LogDatalevelChinese[i];
                } 
            }
        }

        private bool SaveLogXmlDoc_Flag = false;
        private object LogXmlDoc_Lock = new object();
        private System.Threading.AutoResetEvent SaveData2Xml_threaFucEvent = new System.Threading.AutoResetEvent(true);
        bool LogSaveRunning = true;
        private void SaveFuc()
        {
            while (LogSaveRunning)
            {
                if (SaveLogXmlDoc_Flag)
                {
                    if (LogXmlDoc.Element(LogDataNode0Name[(int)Node0Name.Root]).Descendants().Count() > MAXLogDataNode)//记录超过10000条时保存为历史记录
					{
                        string s = DateTime.Now.ToString();
                        s = s.Replace(" ", "");
                        s = s.Replace(":", "");
                        s = s.Replace("/", "");
                        string[] arr = FileNmae.Split('\\');
                        s = s + arr[arr.Length - 1];
                        s = FileNmae.Replace(arr[arr.Length - 1],s);
                        LogXmlDoc.Save(s);
                        MakeXMLDefaultstructure();
                        LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
                        SendParm.Node1NameIndex = (int)LogData.Node1Name.System_security;
                        SendParm.LevelIndex = (int)LogData.Node2Level.MESSAGE;
                        //SendParm.DisplayLang = SCADA.MainForm.CurrentLanguage;
                        SendParm.EventID = ((int)LogData.Node2Level.MESSAGE).ToString();
                        SendParm.Keywords = SCADA.ChangeLanguage.GetString("LogContentSaveHistoryLog");
                        SendParm.EventData = SCADA.ChangeLanguage.GetString("LogContentHistoryLog") + s;
                        LineMainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(LineMainForm.m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");

                    }

                    LogXmlDoc.Save(FileNmae);
                    SaveLogXmlDoc_Flag = false;
                }
                else
                {
                    SaveData2Xml_threaFucEvent.WaitOne();
                }
//                System.Threading.Thread.Sleep(2000);
            }
            LogXmlDoc.Save(FileNmae);//退出最后保存
        }

        public void ExitApp()
        {
            LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
            SendParm.Node1NameIndex = (int)LogData.Node1Name.System_security;
            SendParm.LevelIndex = (int)LogData.Node2Level.MESSAGE;
            SendParm.EventID = ((int)LogData.Node2Level.MESSAGE).ToString();
            SendParm.Keywords = SCADA.ChangeLanguage.GetString("LogContentTheSoftExit");
            SendParm.EventData = "EXIT";
            LineMainForm.m_Log.AddLogMsgHandler.Invoke(this, SendParm);

            LogSaveRunning = false;
            SaveData2Xml_threaFucEvent.Set();
        }
        /// <summary>
        /// 生成默认设置XML对象
        /// </summary>
        /// <param name="FilePath"></param>
        private void MakeXMLDefaultstructure()
        {
            try
            {
                LogXmlDoc = new XDocument(
                    new XElement(LogDataNode0Name[(int)Node0Name.Root],
                        new XElement(LogDataNode0Name[(int)Node0Name.System]),
                        new XElement(LogDataNode0Name[(int)Node0Name.Equipment]),
                        new XElement(LogDataNode0Name[(int)Node0Name.Network])
                        ));
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("生成默认设置文档失败！\r\n" +
                    ex.ToString());
            }
        }

        /// <summary>
        /// 添加日志入口
        /// </summary>
        /// <param name="Node0Str"></param>
        /// <param name="Node1Str"></param>
        /// <param name="Attributes_Provider"></param>
        /// <param name="Attributes_EventID"></param>
        /// <param name="Attributes_Level"></param>
        /// <param name="Attributes_Keywords"></param>
        /// <param name="Attributes_TimeCreated"></param>
        /// <param name="Attributes_EventData"></param>
        private void AddEvent2Xml(EventHandlerSendParm Param)
        {
            lock(LogXmlDoc_Lock)
            {
                int Node0index;

                if (Param.Node1NameIndex == (int)LogData.Node1Name.System_runing
                    || Param.Node1NameIndex == (int)LogData.Node1Name.System_security)
                {
                    Node0index = (int)LogData.Node0Name.System;
                }
                else if (Param.Node1NameIndex == (int)LogData.Node1Name.Equipment_CNC
                    || Param.Node1NameIndex == (int)LogData.Node1Name.Equipment_PLC
                    || Param.Node1NameIndex == (int)LogData.Node1Name.Equipment_RFID
                    || Param.Node1NameIndex == (int)LogData.Node1Name.Equipment_ROBOT
                    || Param.Node1NameIndex == (int)LogData.Node1Name.Equipment_CCD)
                {
                    Node0index = (int)LogData.Node0Name.Equipment;
                }
                else
                {
                    Node0index = (int)LogData.Node0Name.Network;
                }
                LogXmlDoc.Element(LogDataNode0Name[(int)Node0Name.Root]).Element(
                    LogData.LogDataNode0Name[Node0index]).AddFirst(
                    new XElement(LogData.LogDataNode1Name[Param.Node1NameIndex],
                    new XAttribute(LogDataNode2Attributes[(int)Node2Attributes.Level], LogData.LogDataNode2Level[Param.LevelIndex]),
                    new XAttribute(LogDataNode2Attributes[(int)Node2Attributes.TimeCreated], DateTime.Now.ToString()),
                    new XAttribute(LogDataNode2Attributes[(int)Node2Attributes.Provider], Param.Provider),
                    new XAttribute(LogDataNode2Attributes[(int)Node2Attributes.EventID], Param.EventID),
                    new XAttribute(LogDataNode2Attributes[(int)Node2Attributes.Keywords], Param.Keywords),
                    new XAttribute(LogDataNode2Attributes[(int)Node2Attributes.EventData], Param.EventData)
                    ));
            }
            SaveLogXmlDoc_Flag = true;
            SaveData2Xml_threaFucEvent.Set();
        }


        public struct EventHandlerSendParm
        {
            public int Node1NameIndex;
            public int LevelIndex;
            public String DisplayLang;
            public String Provider;
            public String EventID;
            public String Keywords;
            public String EventData;
        }
        private void EventHandlerFuc(object ob, EventHandlerSendParm Param)
        {
            try
            {
                if (Param.Provider == null && ob != null)
                {
                    Param.Provider = ob.ToString().Split(',')[0];
                }
                AddEvent2Xml(Param);
            }
            catch
            {

            }
        }


        public void AddLogMsgHandlerFinished(IAsyncResult result)
        {
            System.EventHandler<EventHandlerSendParm> handler = (System.EventHandler<EventHandlerSendParm>)((System.Runtime.Remoting.Messaging.AsyncResult)result).AsyncDelegate;
            handler.EndInvoke(result);
            Console.WriteLine(result.AsyncState);
            string sdf = result.AsyncState.ToString();
        }


        /// <summary>
        /// 从XML文件中读取一个DataTable
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="address">XML文件地址</param>
        /// <returns></returns>
        public System.Data.DataTable ReadFromXml(String ParThStr)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                /*if (ChangeLanguage.GetDefaultLanguage() == "English")
                {
                    DataDataTableShowColumnStr[0] = "Level";
                    DataDataTableShowColumnStr[1] = "DateTime";
                    DataDataTableShowColumnStr[2] = "source";
                    DataDataTableShowColumnStr[3] = "EventID";
                    DataDataTableShowColumnStr[4] = "EventDescribe";
                    DataDataTableShowColumnStr[5] = "EventData";
                    for (int ii = 0; ii < DataDataTableShowColumnStr.Length; ii++)
                    {
                        dt.Columns.Add(DataDataTableShowColumnStr[ii]);
                    }
                }
                else
                {
                    DataDataTableShowColumnStr[0] = "级别";
                    DataDataTableShowColumnStr[1] = "日期和时间";
                    DataDataTableShowColumnStr[2] = "来源";
                    DataDataTableShowColumnStr[3] = "事件";
                    DataDataTableShowColumnStr[4] = "事件描述";
                    DataDataTableShowColumnStr[5] = "事件数据";
                    for (int ii = 0; ii < DataDataTableShowColumnStr.Length; ii++)
                    {
                        dt.Columns.Add(DataDataTableShowColumnStr[ii]);
                    }
                }*/
                DataDataTableShowColumnStr[0] = ChangeLanguage.GetString("LogColumn01");
                DataDataTableShowColumnStr[1] = ChangeLanguage.GetString("LogColumn02");
                DataDataTableShowColumnStr[2] = ChangeLanguage.GetString("LogColumn03");
                DataDataTableShowColumnStr[3] = ChangeLanguage.GetString("LogColumn04");
                DataDataTableShowColumnStr[4] = ChangeLanguage.GetString("LogColumn05");
                DataDataTableShowColumnStr[5] = ChangeLanguage.GetString("LogColumn06");
                for (int ii = 0; ii < DataDataTableShowColumnStr.Length; ii++)
                {
                    dt.Columns.Add(DataDataTableShowColumnStr[ii]);
                }

                String[] StrArr = ParThStr.Split('\\');

                if (StrArr.Length == 1)//
                {
                    var n = from c in LogXmlDoc.Element(LogDataNode0Name[(int)Node0Name.Root]).Elements(StrArr[0])
                            select c;
                    foreach (var item in n)
                    {
                        foreach(var itemsub in item.Elements())
                        {
                            string[] array = new string[DataDataTableShowColumnStr.Length];
                            for (int ii = 0; ii < DataDataTableShowColumnStr.Length; ii++)
                            {
                                array[ii] = itemsub.Attribute(LogDataNode2Attributes[ii]).Value;
                            }
                            dt.Rows.Add(array);
                        }
                    }

                }
                else if (StrArr.Length == 2)
                {
                    var n = from c in LogXmlDoc.Element(LogDataNode0Name[(int)Node0Name.Root]).Elements(StrArr[0]).Elements(StrArr[1])
                            select c;
                    foreach (var item in n)
                    {
                        string[] array = new string[DataDataTableShowColumnStr.Length];
                        for (int ii = 0; ii < DataDataTableShowColumnStr.Length; ii++)
                        {
                            array[ii] = item.Attribute(LogDataNode2Attributes[ii]).Value;
                        }
                        dt.Rows.Add(array);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new System.Data.DataTable();
            }

            return dt;
        }

    }
}
