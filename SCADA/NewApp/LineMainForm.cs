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
using System.Security.Cryptography;
using System.IO;
using System.Reflection;
using System.Net.NetworkInformation;
using System.Management;//需要在项目中添加System.Management引用
using System.Management.Instrumentation;
using LineDevice;
using Collector;
using ScadaHncData;
using HNC_MacDataService;
using System.Threading;
using SCADA.SimensPLC;
using System.Text.RegularExpressions;

namespace SCADA.NewApp
{
    public partial class LineMainForm : Form
    {
        #region 注册
        /// <summary>
        /// 取得设备硬盘的卷标号
        /// </summary>
        /// <returns></returns>
        /*private string GetDiskVolumeSerialNumber()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObject disk = new ManagementObject("win32_logicaldisk.deviceid=\"c:\"");
            disk.Get();
            return disk.GetPropertyValue("VolumeSerialNumber").ToString();
        }*/
        private string GetDiskVolumeSerialNumber()
        {
            ManagementClass mc = new ManagementClass("Win32_PhysicalMedia");
            ManagementObjectCollection moc = mc.GetInstances();
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (ManagementObject mo in moc)
            {
                string tag = mo.Properties["Tag"].Value.ToString().ToLower().Trim();
                string hdId = (string)mo.Properties["SerialNumber"].Value ?? string.Empty;
                hdId = hdId.Trim();
                dict.Add(tag, hdId);
            }

            mc = new ManagementClass("Win32_OperatingSystem");
            moc = mc.GetInstances();
            string currentSysRunDisk = string.Empty;
            foreach (ManagementObject mo in moc)
            {
                currentSysRunDisk = Regex.Match(mo.Properties["Name"].Value.ToString().ToLower(), @"harddisk\d+").Value;
            }
            var results = dict.Where(x => Regex.IsMatch(x.Key, @"physicaldrive" + Regex.Match(currentSysRunDisk, @"\d+$").Value));
            string strID = "";
            if (results.Any())
            {
                strID = results.ElementAt(0).Value;
            }
            else
            {
                strID = "00000000";
            }
            strID = strID.Replace("_", "");
            int length = strID.Length;
            if (length > 10)
            {
                strID = strID.Substring(length - 10, 9);
                return strID;//844AF4D01
            }
            else
            {
                strID = strID + "00000000";
                strID = strID.Substring(0, 8);
                return strID;
            }
        }

        /// <summary>
        /// 获得CPU的序列号
        /// </summary>
        /// <returns></returns>
        private string getCpu()
        {
            string strCpu = null;
            ManagementClass myCpu = new ManagementClass("win32_Processor");
            ManagementObjectCollection myCpuConnection = myCpu.GetInstances();
            foreach (ManagementObject myObject in myCpuConnection)
            {
                strCpu = myObject.Properties["Processorid"].Value.ToString();
                break;
            }
            return strCpu;
        }

        /// <summary>
        /// 生成机器码
        /// </summary>
        /// <returns></returns>
        private string getMNum()
        {
            string strNum = getCpu() + GetDiskVolumeSerialNumber();//获得24位Cpu和硬盘序列号
            string strMNum = strNum.Substring(0, 24);//从生成的字符串中取出前24个字符做为机器码
            return strMNum;
        }
        private int[] intCode = new int[127];//存储密钥
        private int[] intNumber = new int[25];//存机器码的Ascii值
        private char[] Charcode = new char[25];//存储机器码字
        private void setIntCode()//给数组赋值小于10的数
        {
            for (int i = 1; i < intCode.Length; i++)
            {
                intCode[i] = i % 9;
            }
        }

        /// <summary>
        /// 生成注册码
        /// </summary>
        /// <returns></returns>
        private string getRNum()
        {
            setIntCode();//初始化127位数组
            string MNum = this.getMNum();//获取注册码
            for (int i = 1; i < Charcode.Length; i++)//把机器码存入数组中
            {
                Charcode[i] = Convert.ToChar(MNum.Substring(i - 1, 1));
            }
            for (int j = 1; j < intNumber.Length; j++)//把字符的ASCII值存入一个整数组中。
            {
                intNumber[j] = intCode[Convert.ToInt32(Charcode[j])] + Convert.ToInt32(Charcode[j]);
            }
            string strAsciiName = "";//用于存储注册码
            for (int j = 1; j < intNumber.Length; j++)
            {
                if (intNumber[j] >= 48 && intNumber[j] <= 57)//判断字符ASCII值是否0－9之间
                {
                    strAsciiName += Convert.ToChar(intNumber[j]).ToString();
                }
                else if (intNumber[j] >= 65 && intNumber[j] <= 90)//判断字符ASCII值是否A－Z之间
                {
                    strAsciiName += Convert.ToChar(intNumber[j]).ToString();
                }
                else if (intNumber[j] >= 97 && intNumber[j] <= 122)//判断字符ASCII值是否a－z之间
                {
                    strAsciiName += Convert.ToChar(intNumber[j]).ToString();
                }
                else//判断字符ASCII值不在以上范围内
                {
                    if (intNumber[j] > 122)//判断字符ASCII值是否大于z
                    {
                        strAsciiName += Convert.ToChar(intNumber[j] - 10).ToString();
                    }
                    else
                    {
                        strAsciiName += Convert.ToChar(intNumber[j] - 9).ToString();
                    }
                }
            }
            return strAsciiName;//返回注册码
        }
        public static String MakeSingSn(String SNCreateStr)
        {
            String str = "";
            Int32 SNCreateStrii = SNCreateStr.GetHashCode();
            for (int ii = 0; ii < SNCreateStr.Length; ii++)//求出字符串
            {
                SNCreateStrii ^= SNCreateStr.Substring(ii, 1).GetHashCode();
                str += SNCreateStrii.ToString();
                int index = ((System.Math.Abs(SNCreateStrii)) / (Int32.MaxValue / 2))
                    * (str.Length / 2);
                str = str.Substring(0, index) + SNCreateStr.Substring(ii, 1) + str.Substring(index, str.Length - index - 1);
            }
            return str;
        }

        string jiemi(string str, string encryptKey)
        {
            byte[] P_byte_key = Encoding.Unicode.GetBytes(encryptKey);//将密钥字符串转换为字节序列
            MemoryStream P_MemoryStream_temp = new MemoryStream();//创建内存流对象
            if (str != string.Empty && str != null)
            {
                byte[] P_byte_data = Convert.FromBase64String(str);//将加密后的字符串转换为字节序列
                MemoryStream P_Stream_MS = new MemoryStream(P_byte_data);//创建内存流对象并写入数据
                //创建加密流对象    
                CryptoStream P_CryptStream_Stream =
                    new CryptoStream(P_Stream_MS, new DESCryptoServiceProvider().
                    CreateDecryptor(P_byte_key, P_byte_key), CryptoStreamMode.Read);
                byte[] P_bt_temp = new byte[200];//创建字节序列对象

                int i = 0;//创建记数器
                while ((i = P_CryptStream_Stream.Read(P_bt_temp, 0, P_bt_temp.Length)) > 0)//使用while循环得到解密数据               
                {
                    P_MemoryStream_temp.Write(P_bt_temp, 0, i);//将解密后的数据放入内存流                    
                }
            }
            return Encoding.Unicode.GetString(P_MemoryStream_temp.ToArray());//方法返回解密后的字符串              
        }

        private String GetzhuceEndtime(String str)
        {
            string miyao = "HNC8";
            return jiemi(str, miyao);

        }
        private bool zhuce()
        {
            String SNpath = "..\\data\\Set\\SN.dll";
            String SNCreateStr = getRNum();
            //SNCreateStr = "EMKEMEMM333936I2K17KME8C";
            String SNCreateStr11 = MakeSingSn(SNCreateStr);
            String DT = String.Empty;

            bool isreg = false;
            if (System.IO.File.Exists(SNpath))
            {
                System.IO.StreamReader sr = new System.IO.StreamReader(SNpath, Encoding.Default);
                // System.IO.StreamWriter sr = new System.IO.StreamWriter(fs);
                String Allstr = "";
                String SN = "";
                String line = "";

                SN = sr.ReadLine();
                DT = sr.ReadLine();
                sr.Close();
                if (SN == SNCreateStr11)
                {
                    isreg = true;
                }
                else
                {
                    MessageBox.Show(string.Format("失败未注册！本机序列：{0}", SNCreateStr));
                }
            }
            else
            {
                System.IO.File.Open(SNpath, System.IO.FileMode.Create);
                /*SCADA.WindowsForm.Reg m_reg = new SCADA.WindowsForm.Reg();
                m_reg.RegMunber = SNCreateStr;
                m_reg.ShowDialog();
                if (m_reg.RegSet == SNCreateStr11)
                {
                    isreg = true;
                    System.IO.StreamWriter sw = new System.IO.StreamWriter(SNpath);
                    sw.Write(SNCreateStr11);
                    sw.Close();
                }*/
                MessageBox.Show(string.Format("失败未注册！本机序列：{0}", SNCreateStr));
            }


            if (DT == String.Empty || DT == null)
            {
                System.IO.File.Open(SNpath, System.IO.FileMode.Create);
                /*SCADA.WindowsForm.Reg m_reg = new SCADA.WindowsForm.Reg();
                m_reg.RegMunber = SNCreateStr;
                m_reg.ShowDialog();*/
                MessageBox.Show(string.Format("失败未注册！本机序列：{0}", SNCreateStr));
                isreg = false;
            }
            else
            {
                DT = GetzhuceEndtime(DT);
                DateTime dt = Convert.ToDateTime(DT); //注册到期时间
                DateTime nowtime = DateTime.Now;//当前时间
                TimeSpan u = (dt - nowtime);
                if (u.TotalMinutes < 0)
                {
                    System.IO.File.Open(SNpath, System.IO.FileMode.Create);
                    /*SCADA.WindowsForm.Reg m_reg = new SCADA.WindowsForm.Reg();
                    m_reg.RegMunber = SNCreateStr;
                    m_reg.ShowDialog();*/
                    MessageBox.Show(string.Format("失败未注册！本机序列：{0}", SNCreateStr));
                    isreg = false;
                }
            }
            return isreg;
        }
        #endregion

        private static DateTime SystemStartime;
        public static List<CNC> cnclist = null;//CNC设备列表
        public static LogData m_Log = null;//系统日志
        private String LogFilePath = "..\\data\\Log\\SystemLogNew.xml";//log日志路径
        public static string XMLSavePath = "..\\data\\Set\\SCADASetNew.xml";//设置文件的路径
        public static m_xmlDociment m_xml = null;//设置数据文件
        private CollectShare ncCollector = null;//CNC数据收集器
        public static ShareData shareData = null;//共享内存对象
        public static IntPtr mainform_Ptr = IntPtr.Zero;//保存了MainForm的句柄
        //public PLCDataShare plc = null;
        public static bool InitializeComponentFinish = false;//所有设备初始化状态
        public readonly String CNCTaskDataFilePath = "..\\data\\CNCTask\\CNCTask.xml";//派工单数据保存
        private static Form[] m_Formarr = null;
        //public static ModbusTCP Modbusclient1 = new ModbusTCP();
        //public static ModbusTCPNew ModbusTCP1 = new ModbusTCPNew();

        public static int Linebuttomcur = 2;//当前产线哪个按钮正在按下，初始认为产线停止2，产线启动1，产线复位3
        public static bool linestart = true;
        public static bool linestop = false;
        public static bool linereset = true;
        public static bool linestarting = false;
        public static bool linestoping = false;
        public static bool linereseting = false;
        public static bool MES_PLC_comfim_flage = false;
        public static int MES_PLC_comfim_count = 0;//mes发送plc启动码计时
        public static bool plcgetconfim = false;

        //public static List<Equipments> DBEquipment = new List<Equipments>();
        //public static List<CNCShowData> DBCNCShowData = new List<CNCShowData>();
        //public static List<WMSRackData> DBWMSRackData = new List<WMSRackData>();
        //public static List<Order> DBOrder = new List<Order>();
        //public static List<Tasks> DBTasks = new List<Tasks>();
        public static List<PLCAlarm> DBPLCAlarm = new List<PLCAlarm>();
        public static ControlPLC controlplc = new ControlPLC();
        public static UnitPLC unitplc1 = new UnitPLC();
        public static UnitPLC unitplc2 = new UnitPLC();
        public static UnitPLC unitplc3 = new UnitPLC();
        public static UnitPLC unitplc4 = new UnitPLC();
        public static WMSPLC wmsplc = new WMSPLC();
        public static CNCData cncdata1 = new CNCData();
        public static CNCData cncdata2 = new CNCData();
        public static CNCData cncdata3 = new CNCData();
        public static CNCData cncdata4 = new CNCData();
        public static CNCData cncdata5 = new CNCData();
        public static CNCData cncdata6 = new CNCData();
        public static CNCData cncdata7 = new CNCData();
        public static CNCData cncdata8 = new CNCData();

        public static SygoleRFID sygolefidcnc11 = new SygoleRFID();
        public static SygoleRFID sygolefidcnc12 = new SygoleRFID();
        public static SygoleRFID sygolefidcnc21 = new SygoleRFID();
        public static SygoleRFID sygolefidcnc22 = new SygoleRFID();
        public static SygoleRFID sygolefidcnc31 = new SygoleRFID();
        public static SygoleRFID sygolefidcnc32 = new SygoleRFID();
        public static SygoleRFID sygolefidcnc41 = new SygoleRFID();
        public static SygoleRFID sygolefidcnc42 = new SygoleRFID();
        public static SygoleRFID sygolefidclean1 = new SygoleRFID();
        public static SygoleRFID sygolefidclean2 = new SygoleRFID();

        public static SygoleRFID sygolefidfit11 = new SygoleRFID();
        public static SygoleRFID sygolefidfit12 = new SygoleRFID();
        public static SygoleRFID sygolefidfit21 = new SygoleRFID();
        public static SygoleRFID sygolefidfit22 = new SygoleRFID();

        public static SygoleRFID sygolefidwmsin = new SygoleRFID();
        public static SygoleRFID sygolefidwmsout = new SygoleRFID();
        public static SygoleRFID sygolefidwmsaoto = new SygoleRFID();

        public static SerialPortCommBase gauge1 = new SerialPortCommBase("COM0");
        public static SerialPortCommBase gauge2 = new SerialPortCommBase("COM1");
        public static bool COMOPENFLAGE1 = true;
        public static bool COMCLOSEFLAGE1 = false;
        public static bool COMOPENFLAGE2 = true;
        public static bool COMCLOSEFLAGE2 = false;
        //public static List<NewOrder> DBNewOrder = new List<NewOrder>();
        //public static List<DeviceBind> DBDeviceBinds = new List<DeviceBind>();


        private enum ExUnitIndex
        {
            拓展一,
            拓展二
        }
        private enum CNCIndex
        {
            拓展一车床,
            拓展一铣床,
            拓展二车床,
            拓展二铣床
        }

        private struct CNCParam
        {
            public string ip;//机床IP
            public string hezifile;//盒子加工程序
            public string gaizifile;//盖子加工程序
            public string resetfile;//复位程序
            //public int sendFile(String localName, String dstName, int ch = 0, bool progSelect = false)
            //{
            //    int ret = -4;
            //    if (PingTest(ip))
            //    {
            //        progSelect = true;
            //        int dbNo = 0;
            //        if (MacDataService.GetInstance().GetMachineDbNo(ip, ref dbNo) == 0)
            //        {
            //            ret = MacDataService.GetInstance().HNC_NetFileSend(localName, dstName, dbNo);
            //            if (ret == 0 && progSelect == true)
            //            {
            //                dstName = "/" + dstName;
            //                ret = MacDataService.GetInstance().HNC_SysCtrlSelectProg(ch, dstName, dbNo);
            //            }

            //            string filename = get_gCodeName(dbNo);
            //            if (filename == dstName.Substring(1, dstName.Length - 1))
            //                ret = 0;
            //            else
            //                ret = -1;
            //        }
            //        else
            //        {
            //            ret = -3;
            //        }
            //    }
            //    return ret;
            //}

            //public string get_gCodeName(int dbNo)
            //{
            //    string gCodeName = "";
            //    string key = "Channel:0";
            //    int ret = MacDataService.GetInstance().GetHashKeyValueString(dbNo, key, "HNC_CHAN_RUN_PROG", ref gCodeName);
            //    if (ret == 0 && gCodeName.Length > 0)
            //    {
            //        int length = gCodeName.Length - 2;

            //        string str = gCodeName.Substring(0, 2);
            //        if (str == "..")
            //        {
            //            gCodeName = gCodeName.Substring(2, length);
            //            gCodeName = "h/lnc8" + gCodeName;
            //        }
            //        else if (str.Contains("/"))
            //        {
            //            string[] strs = str.Split('/');
            //            gCodeName = strs[strs.Length - 1];
            //        }
            //        else gCodeName = gCodeName;
            //    }
            //    return gCodeName;
            //}
        }

        public LineMainForm()
        {
            SystemStartime = DateTime.Now;

            mainform_Ptr = this.Handle;
            m_Log = new LogData();//系统日志
            m_Log.m_load(LogFilePath, true);
            cnclist = new List<CNC>();//CNC设备列表
            m_xml = new m_xmlDociment();//设置数据文件
            CheckRegister();
            InitializeComponent();
            if (m_xml.m_load(XMLSavePath) == 0)
            {
                System.Windows.Forms.MessageBox.Show("配置文件不存在，已经生成默认的配置文件\r\n");
            }
           // pictureBox1.Image = Image.FromFile("..\\picture\\logo.png");
            ShareData._gShareData = shareData = new ShareData(mainform_Ptr);
            ncCollector = new CollectShare(shareData, cnclist);
            //plc = new PLCDataShare(shareData);
            LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
            try
            {
                InitEquiment();
                Initport();
                InitializeComponentFinish = true;
                if (InitializeComponentFinish)
                {
                    ncCollector.StartCollection();   //开启采集器
                }
                SendParm.Node1NameIndex = (int)LogData.Node1Name.System_runing;
                SendParm.LevelIndex = (int)LogData.Node2Level.MESSAGE;
                //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                SendParm.EventID = ((int)LogData.Node2Level.MESSAGE).ToString();
                SendParm.Keywords = ChangeLanguage.GetString("LogContentDeviceInitSuccess");
                SendParm.EventData = ChangeLanguage.GetString("LogContentAllDeviceInitSuccess");
                m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex);
                InitializeComponentFinish = false;
                SendParm.Node1NameIndex = (int)LogData.Node1Name.System_runing;
                SendParm.LevelIndex = (int)LogData.Node2Level.FAULT;
                //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                SendParm.EventID = ((int)LogData.Node2Level.FAULT).ToString();
                SendParm.Keywords = ChangeLanguage.GetString("LogContentDeviceInitFailure");
                SendParm.EventData = ex.ToString();
                m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
            }
            tabPagemain.TabPages.Remove(tabPageHome);
            m_Formarr = new Form[tabPagemain.TabCount];
            GenerateForm(0, tabPagemain);
            string formClassSTR = String.Empty;
            for (int i = 1; i < tabPagemain.TabCount; i++)
            {
                formClassSTR = tabPagemain.TabPages[i].Tag.ToString();
                m_Formarr[i] = (Form)Assembly.GetExecutingAssembly().CreateInstance(formClassSTR);
                m_Formarr[i].FormBorderStyle = FormBorderStyle.None;
                m_Formarr[i].TopLevel = false;
                m_Formarr[i].Parent = tabPagemain.TabPages[i];
                m_Formarr[i].ControlBox = false;
                m_Formarr[i].Dock = DockStyle.Fill;
            }
        }

        private void Initport()
        {

            //打开COM口
            if (!gauge1.IsOpen)//串口开启状态
            {
                gauge1.Open();//串口打开成功

            }
            if (!gauge2.IsOpen)//串口开启状态
            {
                gauge2.Open();//串口打开成功

            }

        }
        private void InitEquiment()
        {
            LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
            string get_str = m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[0]);//SUM
            for (int ii = 0; ii < int.Parse(get_str); ii++)
            {
                CNC m_cnc = new CNC();
                m_cnc.InitCNCParam(m_xml.m_Read(m_xmlDociment.PathRoot_CNC, ii, m_xmlDociment.Default_Path_str[(int)m_xmlDociment.Path_str.serial]),
                     m_xml.m_Read(m_xmlDociment.PathRoot_CNC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.workshop]),
                     m_xml.m_Read(m_xmlDociment.PathRoot_CNC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.productionline]),
                     m_xml.m_Read(m_xmlDociment.PathRoot_CNC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.system]),
                     m_xml.m_Read(m_xmlDociment.PathRoot_CNC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]),
                     m_xml.m_Read(m_xmlDociment.PathRoot_CNC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]),
                     m_xml.m_Read(m_xmlDociment.PathRoot_CNC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.SN]),
                     m_xml.m_Read(m_xmlDociment.PathRoot_CNC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.EQUIP_CODE]),
                     m_xml.m_Read(m_xmlDociment.PathRoot_CNC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.remark]),
                     CNCTaskDataFilePath);
                //net_to_redis
                if (MacDataService.GetInstance().GetMachineDbNo(m_cnc.ip, ref m_cnc.dbNo) == 0)
                    Console.WriteLine(" m_cnc.dbNo = " + m_cnc.dbNo);

                cnclist.Add(m_cnc);

                SendParm.Node1NameIndex = (int)LogData.Node1Name.System_runing;
                SendParm.LevelIndex = (int)LogData.Node2Level.MESSAGE;
                //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                SendParm.EventID = ((int)LogData.Node2Level.MESSAGE).ToString();
                SendParm.Keywords = ChangeLanguage.GetString("LogContentCNCInit");
                SendParm.EventData = m_cnc.JiTaiHao + ChangeLanguage.GetString("LogContentInitSuccess") + "；IP = " + m_cnc.ip + "  Port = " + m_cnc.port;
                m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
            }
            get_str = m_xml.m_Read(m_xmlDociment.PathRoot_PLC, -1, m_xmlDociment.Default_Attributes_str1[0]);//SUM
            if (int.Parse(get_str) > 0)
            {
                controlplc.IP = m_xml.m_Read(m_xmlDociment.PathRoot_PLC, 4, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                controlplc.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_PLC, 4, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                wmsplc.IP = m_xml.m_Read(m_xmlDociment.PathRoot_PLC,5, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                wmsplc.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_PLC, 5, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                unitplc1.IP = m_xml.m_Read(m_xmlDociment.PathRoot_PLC, 0, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                unitplc1.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_PLC, 0, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                unitplc2.IP = m_xml.m_Read(m_xmlDociment.PathRoot_PLC, 1, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                unitplc2.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_PLC, 1, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                unitplc3.IP = m_xml.m_Read(m_xmlDociment.PathRoot_PLC, 2, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                unitplc3.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_PLC, 2, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                unitplc4.IP = m_xml.m_Read(m_xmlDociment.PathRoot_PLC, 3, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                unitplc4.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_PLC,3, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidcnc11.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 0, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidcnc11.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 0, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidcnc12.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 1, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidcnc12.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 1, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidcnc21.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 2, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidcnc21.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 2, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidcnc22.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 3, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidcnc22.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 3, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidcnc31.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 4, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidcnc31.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID,4, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));
                
                sygolefidcnc32.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID,5, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidcnc32.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 5, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidcnc41.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 6, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidcnc41.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID,6, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));
               
                sygolefidcnc42.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 7, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidcnc42.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 7, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidclean1.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 8, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidclean1.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 8, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidclean2.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 9, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidclean2.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 9, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidfit11.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 14, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidfit11.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 14, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidfit12.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 15, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidfit12.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 15, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidfit21.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID,16, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidfit21.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID,16, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));
                
                sygolefidfit22.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID,17, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidfit22.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 17, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidwmsin.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 18, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidwmsin.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 18, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidwmsout.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 19, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidwmsout.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 19, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

                sygolefidwmsout.IP = m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 20, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.ip]);
                sygolefidwmsout.Port = int.Parse(m_xml.m_Read(m_xmlDociment.PathRoot_RFID, 20, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.port]));

               



        bool res = controlplc.Connecting(controlplc.IP, controlplc.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_PLC;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "总控PLC连接失败";
                    SendParm.EventData = string.Format("PLC连接失败, 请检查网络和PLC上电情况！IP={0},Port={1}", controlplc.IP, controlplc.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = wmsplc.Connecting(wmsplc.IP, wmsplc.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_PLC;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "立库PLC连接失败";
                    SendParm.EventData = string.Format("PLC连接失败, 请检查网络和PLC上电情况！IP={0},Port={1}", wmsplc.IP, wmsplc.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }

               
                res = unitplc1.Connecting(unitplc1.IP, unitplc1.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_PLC;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "单元一连接失败";
                    SendParm.EventData = string.Format("单元一连接失败, 请检查网络和单元一上电情况！IP={0},Port={1}", unitplc1.IP, unitplc1.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = unitplc2.Connecting(unitplc2.IP, unitplc2.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_PLC;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "单元二连接失败";
                    SendParm.EventData = string.Format("单元二连接失败, 请检查网络和单元二上电情况！IP={0},Port={1}", unitplc2.IP, unitplc2.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = unitplc3.Connecting(unitplc3.IP, unitplc3.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_PLC;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "单元三连接失败";
                    SendParm.EventData = string.Format("单元三连接失败, 请检查网络和单元三上电情况！IP={0},Port={1}", unitplc3.IP, unitplc3.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }

                res = unitplc4.Connecting(unitplc4.IP, unitplc4.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_PLC;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "单元四连接失败";
                    SendParm.EventData = string.Format("单元四连接失败, 请检查网络和单元四上电情况！IP={0},Port={1}", unitplc4.IP, unitplc4.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }

                res = sygolefidcnc11.Connect(sygolefidcnc11.IP, sygolefidcnc11.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "单元一左RFID连接失败";
                    SendParm.EventData = string.Format("单元一左RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidcnc11.IP, sygolefidcnc11.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = sygolefidcnc12.Connect(sygolefidcnc12.IP, sygolefidcnc12.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "单元一右RFID连接失败";
                    SendParm.EventData = string.Format("单元一右RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidcnc12.IP, sygolefidcnc12.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = sygolefidcnc21.Connect(sygolefidcnc21.IP, sygolefidcnc21.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "单元二左RFID连接失败";
                    SendParm.EventData = string.Format("单元二左RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidcnc21.IP, sygolefidcnc21.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = sygolefidcnc22.Connect(sygolefidcnc22.IP, sygolefidcnc22.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "单元二右RFID连接失败";
                    SendParm.EventData = string.Format("单元二右RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidcnc22.IP, sygolefidcnc22.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = sygolefidcnc31.Connect(sygolefidcnc31.IP, sygolefidcnc31.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "单元三左RFID连接失败";
                    SendParm.EventData = string.Format("RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidcnc31.IP, sygolefidcnc31.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = sygolefidcnc32.Connect(sygolefidcnc32.IP, sygolefidcnc32.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "单元三右RFID连接失败";
                    SendParm.EventData = string.Format("单元三右RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidcnc32.IP, sygolefidcnc32.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = sygolefidcnc41.Connect(sygolefidcnc41.IP, sygolefidcnc41.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "单元四左RFID连接失败";
                    SendParm.EventData = string.Format("单元四左RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidcnc41.IP, sygolefidcnc41.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = sygolefidcnc42.Connect(sygolefidcnc42.IP, sygolefidcnc42.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "单元四右RFID连接失败";
                    SendParm.EventData = string.Format("单元四右RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidcnc42.IP, sygolefidcnc42.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = sygolefidclean1.Connect(sygolefidclean1.IP, sygolefidclean1.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "清洗左RFID连接失败";
                    SendParm.EventData = string.Format("清洗左RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidclean1.IP, sygolefidclean1.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = sygolefidclean2.Connect(sygolefidclean2.IP, sygolefidclean2.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "清洗右RFID连接失败";
                    SendParm.EventData = string.Format("清洗右RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidclean2.IP, sygolefidclean2.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = sygolefidfit11.Connect(sygolefidfit11.IP, sygolefidfit11.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "装配一左RFID连接失败";
                    SendParm.EventData = string.Format("装配一左RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidfit11.IP, sygolefidfit11.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = sygolefidfit12.Connect(sygolefidfit12.IP, sygolefidfit12.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "装配一右RFID连接失败";
                    SendParm.EventData = string.Format("装配一右RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidfit12.IP, sygolefidfit12.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = sygolefidfit21.Connect(sygolefidfit21.IP, sygolefidfit21.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "装配二左RFID连接失败";
                    SendParm.EventData = string.Format("装配二左RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidfit21.IP, sygolefidfit21.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }
                res = sygolefidfit22.Connect(sygolefidfit22.IP, sygolefidfit22.Port);
                if (!res)
                {
                    SendParm.Node1NameIndex = (int)LogData.Node1Name.Equipment_RFID;
                    SendParm.LevelIndex = (int)LogData.Node2Level.WARNING;
                    //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                    SendParm.EventID = ((int)LogData.Node2Level.WARNING).ToString();
                    SendParm.Keywords = "装配二右RFID连接失败";
                    SendParm.EventData = string.Format("装配二右RFID连接失败, 请检查网络和RFID上电情况！IP={0},Port={1}", sygolefidfit22.IP, sygolefidfit22.Port);
                    m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
                }

                Task.Run(() => CheckToConnect());
                //Task.Run(() => CheckToConnect2(ip2, int.Parse(Port2)));
            }

        }

        private void CheckToConnect()
        {
            while (true)
            {
                Thread.Sleep(1000);
                if (PingTest(controlplc.IP) && !controlplc.GetOnlineState())
                {
                    controlplc.Disconnect();
                    controlplc.Connecting(controlplc.IP, controlplc.Port);
                }

                if (PingTest(wmsplc.IP) && !wmsplc.GetOnlineState())
                {
                    wmsplc.Disconnect();
                    wmsplc.Connecting(wmsplc.IP, wmsplc.Port);
                }
                if (PingTest(sygolefidcnc11.IP) && sygolefidcnc11.ConnectStatus == Sygole.HFReader.ConnectStatusEnum.DISCONNECTED)
                {
                    sygolefidcnc11.DisConnect();
                    sygolefidcnc11.Connect(sygolefidcnc11.IP, sygolefidcnc11.Port);
                }
            }
        }

        public static bool PingTest(string ip)
        {
            try
            {
                Ping ping = new Ping();
                PingReply pingReply = ping.Send(ip, 500);
                if (pingReply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        private void CheckRegister()
        {
            LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();

            if (!zhuce())
            {
                SendParm.Node1NameIndex = (int)LogData.Node1Name.System_security;
                SendParm.LevelIndex = (int)LogData.Node2Level.AUDIT;
                //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                SendParm.EventID = ((int)LogData.Node2Level.AUDIT).ToString();
                SendParm.Keywords = ChangeLanguage.GetString("LogContentUserRegistration");
                SendParm.EventData = ChangeLanguage.GetString("LogContentUserRegistrationFailure");
                System.Threading.Thread.Sleep(1000);
                Environment.Exit(0);
            }
            else
            {
                SendParm.Node1NameIndex = (int)LogData.Node1Name.System_security;
                SendParm.LevelIndex = (int)LogData.Node2Level.AUDIT;
                //SendParm.DisplayLang = ChangeLanguage.GetDefaultLanguage();
                SendParm.EventID = ((int)LogData.Node2Level.AUDIT).ToString();
                SendParm.Keywords = ChangeLanguage.GetString("LogContentUserRegistration");
                SendParm.EventData = ChangeLanguage.GetString("LogContentUserRegistrationSuccess");
            }

            m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm,
                new AsyncCallback(m_Log.AddLogMsgHandlerFinished), "AddLogMsgHandlerFinished!");
        }

        private void tabPagemain_DrawItem(object sender, DrawItemEventArgs e)
        {
            tabPagemain.SuspendLayout();
            //背景色
            Image backImage = Properties.Resources.titleBack;
            Rectangle rec = tabPagemain.ClientRectangle;

            StringFormat StrFormat = new StringFormat();

            StrFormat.Alignment = StringAlignment.Center;
            StrFormat.LineAlignment = StringAlignment.Center;

            SolidBrush tabBackBrush = new SolidBrush(Color.Black);
            //文字色
            SolidBrush FrontBrush = new SolidBrush(Color.White);
            //StringFormat stringF = new StringFormat();
            Font wordfont = new Font("微软雅黑", 14.25F, FontStyle.Bold);
            e.Graphics.DrawImage(backImage, 0, 0, tabPagemain.Width, tabPagemain.Height);
            for (int i = 0; i < tabPagemain.TabCount; i++)
            {
                //标签工作区
                Rectangle rec1 = tabPagemain.GetTabRect(i);
                //e.Graphics.DrawImage(backImage, 0, 0, tabPagemain.Width, tabPagemain.Height);
                e.Graphics.FillRectangle(tabBackBrush, rec1);
                ////标签头背景色
                e.Graphics.DrawString(tabPagemain.TabPages[i].Text, wordfont, FrontBrush, rec1, StrFormat);
                ////标签头文字
            }
            tabPagemain.ResumeLayout();
        }



        private void tabPagemain_SelectedIndexChanged(object sender, EventArgs e)
        {
            tabPagemain.SuspendLayout();
            GenerateForm(tabPagemain.SelectedIndex, sender);
            tabPagemain.ResumeLayout();
        }

        public void GenerateForm(int form_index, object sender)
        {
            // 反射生成窗体//只生成一次
            if (m_Formarr[form_index] == null && form_index >= 0 && form_index < tabPagemain.TabCount)
            {
                string formClassSTR = ((TabControl)sender).SelectedTab.Tag.ToString();
                m_Formarr[form_index] = (Form)Assembly.GetExecutingAssembly().CreateInstance(formClassSTR);
                // m_Formarr[10 ] = { "SCADA.VideoForm, Text: VideoForm"};
                if (m_Formarr[form_index] != null)
                {
                    //设置窗体没有边框 加入到选项卡中
                    m_Formarr[form_index].FormBorderStyle = FormBorderStyle.None;
                    m_Formarr[form_index].TopLevel = false;
                    m_Formarr[form_index].Parent = ((TabControl)sender).SelectedTab;
                    m_Formarr[form_index].ControlBox = false;
                    m_Formarr[form_index].Dock = DockStyle.Fill;
                    if (ChangeLanguage.defaultcolor != Color.White)
                    {
                        ChangeLanguage.LoadSkin(m_Formarr[form_index], ChangeLanguage.defaultcolor);
                    }
                    m_Formarr[form_index].Show();
                }
            }
            else
            {
                m_Formarr[form_index].Show();
            }
        }

        private void PLCChecktimer_Tick(object sender, EventArgs e)
        {
            if (controlplc.GetOnlineState())
            {
                if (labelconline.Text != "在线")
                    labelconline.Text = "在线";
            }
            else
            {
                if (labelconline.Text != "离线")
                    labelconline.Text = "离线";
            }
            if (wmsplc.GetOnlineState())
            {

                if (labelwmsline.Text != "在线")
                    labelwmsline.Text = "在线";
            }
            else
            {
                if (labelwmsline.Text != "离线")
                    labelwmsline.Text = "离线";
            }
            if (unitplc1.GetOnlineState())
            {

                if (labelunit1line.Text != "在线")
                    labelunit1line.Text = "在线";
            }
            else
            {
                if (labelunit1line.Text != "离线")
                    labelunit1line.Text = "离线";
            }
            if (unitplc2.GetOnlineState())
            {

                if (labelunit2line.Text != "在线")
                    labelunit2line.Text = "在线";
            }
            else
            {
                if (labelunit2line.Text != "离线")
                    labelunit2line.Text = "离线";
            }
            if (unitplc3.GetOnlineState())
            {

                if (labelunit3line.Text != "在线")
                    labelunit3line.Text = "在线";
            }
            else
            {
                if (labelunit3line.Text != "离线")
                    labelunit3line.Text = "离线";
            }
            if (unitplc4.GetOnlineState())
            {

                if (labelunit4line.Text != "在线")
                    labelunit4line.Text = "在线";
            }
            else
            {
                if (labelunit4line.Text != "离线")
                    labelunit4line.Text = "离线";
            }
            if (Unit1isRestart)
            {
                int value = -1;
                var res = LineMainForm.unitplc1.ReadsingleRegister((int)UnitPLC.REGINDEX.复位完成反馈, out value);
                if (res && value == 1)
                {
                    buttonunit1.Enabled = true;
                    buttonunit1.Text = "单元一复位";
                      LineMainForm.unitplc1.WritesingleRegister((int)UnitPLC.REGINDEX.MES复位, 0);
                    Unit1isRestart = false;
                }

            }
            if (Unit2isRestart)
            {
                int value = -1;
                var res = LineMainForm.unitplc2.ReadsingleRegister((int)UnitPLC.REGINDEX.复位完成反馈, out value);
                if (res && value == 1)
                {
                    buttonunit2.Enabled = true;
                    buttonunit2.Text = "单元二复位";
                    LineMainForm.unitplc2.WritesingleRegister((int)UnitPLC.REGINDEX.MES复位, 0);
                    Unit2isRestart = false;
                }

            }
            if (Unit3isRestart)
            {
                int value = -1;
                var res = LineMainForm.unitplc3.ReadsingleRegister((int)UnitPLC.REGINDEX.复位完成反馈, out value);
                if (res && value == 1)
                {
                    buttonunit3.Enabled = true;
                    buttonunit3.Text = "单元三复位";
                     LineMainForm.controlplc.WritesingleRegister((int)UnitPLC.REGINDEX.MES复位, 0);
                    Unit3isRestart = false;
                }

            }
            if (Unit4isRestart)
            {
                int value = -1;
                var res = LineMainForm.unitplc4.ReadsingleRegister((int)UnitPLC.REGINDEX.复位完成反馈, out value);
                if (res && value == 1)
                {
                    buttonunit4.Enabled = true;
                    buttonunit4.Text = "单元四复位";
                  LineMainForm.unitplc4.WritesingleRegister((int)UnitPLC.REGINDEX.MES复位, 0);
                    Unit4isRestart = false;
                }

            }
            if (WmsisRestart)
            {
                //int value = -1;
                //var res = LineMainForm.wmsplc.ReadsingleRegister((int)WMSPLC.DATAMEANS.料库复位, out value);
                //if (res && value == 1)
                //{
                //    buttonwms.Enabled = true;
                //    buttonwms.Text = "立库复位";
                //    LineMainForm.wmsplc.WritesingleRegister((int)WMSPLC.DATAMEANS.料库复位, 0);
                //    WmsisRestart = false;
                //}

            }
            if (LineisRestart)
            {
                int value = -1;
                var res = LineMainForm.controlplc.ReadsingleRegister((int)ControlPLC.REGINDEX.复位完成反馈, out value);
                if (res && value == 1)
                {
                    btncontrol.Enabled = true;
                    btncontrol.Text = "产线复位";
                    LineMainForm.unitplc4.WritesingleRegister((int)ControlPLC.REGINDEX.MES复位, 0);
                    LineisRestart = false;
                }

            }
            if (ssText.Text != DateTime.Now.ToString())
            {
                String[] timearr = (DateTime.Now - SystemStartime).ToString().Split('.');
                if (timearr.Length == 2)//一天内
                {
                    //SystemRuingTimes = "累计运行时间：0天 " + timearr[0];
                    totaldays.Text = "0";
                    timesText.Text = timearr[0] + "         ";
                }
                else if (timearr.Length == 3)//超过一天
                {
                    //SystemRuingTimes = "累计运行时间：" + timearr[0] + "天 " + timearr[1];
                    totaldays.Text = timearr[0];
                    timesText.Text = timearr[1] + "         ";
                }
                ssText.Text = DateTime.Now.ToString() + "         ";
                alarmshow.Text = "";
            }

            Equittimer.Enabled = true;
        }



        int changemode = 0;


        private void LineMainForm_Load(object sender, EventArgs e)
        {
            //Task.Run(() => UpdateCNCDataToDB());
            labela1.Text = "";
            labela2.Text = "";
        }




     

        static bool LineisRestart = false;
        static bool Unit1isRestart = false; 
        static bool Unit2isRestart = false;
        static bool Unit3isRestart = false;
        static bool Unit4isRestart = false;
        static bool WmsisRestart = false;

        private void buttonunit1_Click(object sender, EventArgs e)
        {
            if (Unit1isRestart)
            {
                MessageBox.Show("当前正在执行复位程序，请结束后再复位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!LineMainForm.controlplc.GetOnlineState())
            {
                MessageBox.Show("总控PLC离线, 无法复位!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 给PLC复位信号
            //LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.MES复位2单元, 1);
            buttonunit1.Enabled = false;
            buttonunit1.Text = "单元二复位中";
            Unit1isRestart = true;
        }

        private void buttonunit2_Click(object sender, EventArgs e)
        {
            if (Unit2isRestart)
            {
                MessageBox.Show("当前正在执行复位程序，请结束后再复位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!LineMainForm.controlplc.GetOnlineState())
            {
                MessageBox.Show("总控PLC离线, 无法复位!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 给PLC复位信号
            //LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.MES复位2单元, 1);
            buttonunit2.Enabled = false;
            buttonunit2.Text = "单元二复位中";
            Unit4isRestart = true;
        }

        private void buttonunit3_Click(object sender, EventArgs e)
        {
            if (Unit3isRestart)
            {
                MessageBox.Show("当前正在执行复位程序，请结束后再复位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!LineMainForm.controlplc.GetOnlineState())
            {
                MessageBox.Show("总控PLC离线, 无法复位!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 给PLC复位信号
            //LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.MES复位2单元, 1);
            buttonunit3.Enabled = false;
            buttonunit3.Text = "单元二复位中";
            Unit4isRestart = true;
        }

        private void buttonunit4_Click(object sender, EventArgs e)
        {
            if (Unit4isRestart)
            {
                MessageBox.Show("当前正在执行复位程序，请结束后再复位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!LineMainForm.controlplc.GetOnlineState())
            {
                MessageBox.Show("总控PLC离线, 无法复位!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 给PLC复位信号
            //LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.MES复位2单元, 1);
            buttonunit4.Enabled = false;
            buttonunit4.Text = "单元二复位中";
            Unit4isRestart = true;
        }

        private void buttonwms_Click(object sender, EventArgs e)
        {
            if (!LineMainForm.wmsplc.GetOnlineState())
            {
                MessageBox.Show("立体库离线, 无法复位!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (buttonwms.Text == "立体库复位中")
            {
                WmsisRestart = false;
                // 给PLC复位信号
                // LineMainForm.wmsplc.WritesingleRegister(600, 0);
                btncontrol.Text = "立体库复位";
            }
            else
            {
                WmsisRestart = false;
                // 给PLC复位信号
                // LineMainForm.wmsplc.WritesingleRegister(600, 1);
                buttonwms.Text = "立体库复位中";
            }
        }

        private void btncontrol_Click(object sender, EventArgs e)
        {
            if (LineisRestart)
            {
                MessageBox.Show("当前正在执行复位程序，请结束后再复位", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (!LineMainForm.controlplc.GetOnlineState())
            {
                MessageBox.Show("总控PLC离线, 无法复位!", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 给PLC复位信号
            //LineMainForm.controlplc.WritesingleRegister((int)ControlPLC.REGINDEX.MES复位2单元, 1);
            btncontrol.Enabled = false;
            btncontrol.Text = "产线复位中";
            LineisRestart = true;
        }
    }

    //不会闪烁的tabcontrol
    public class NoFlashTabControl : TabControl
    {
        public NoFlashTabControl()
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
    }
}
